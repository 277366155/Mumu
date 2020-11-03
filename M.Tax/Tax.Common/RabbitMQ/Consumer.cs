using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tax.Common.Extention;

namespace Tax.Common.RabbitMQ
{
    public class Consumer : BaseRabbitMQFactory, IDisposable
    {
        private static ConnectionFactory _connFac;
        private static IConnection _connection;
        private static Dictionary<string, IModel> _channelDic = new Dictionary<string, IModel>();

        private EventingBasicConsumer consumer;
        private EventingBasicConsumer failConsumer;
        private string _consumerTag;

        public Consumer(RabbitMqOptions options,string channelName) : base(options, channelName)
        {
            Channel.BasicQos(0, 1, false);
            consumer = new EventingBasicConsumer(Channel);
            failConsumer = new EventingBasicConsumer(Channel);
        }

        public void ConsumeStart<T>(Func<T, bool> func)
        {
            consumer.Received += (m, e) =>
            {
                var msg = Encoding.UTF8.GetString(e.Body.ToArray());
                var headers = e.BasicProperties.Headers;
                var death = new Dictionary<string,object>();
                if (headers != null && headers.ContainsKey("x-death"))
                {
                    //death.Add("x-death", headers["x-death"]);
                    death = (Dictionary<string, object>)(headers["x-death"] as List<object>)[0];
                }
                Action act = () => {
                    var retryCount = (long)(death.ContainsKey("count")? death ["count"] : 0L);
                    if (retryCount >= 2)
                    {
                        //消费失败三次，不再重放队列中。
                        Channel.BasicNack(e.DeliveryTag, false, false);
                    }
                    else
                    {
                        var interval = (retryCount + 1) * 10;//首次重新投递时间为10s,第二次20s,第三次30s..
                        e.BasicProperties.Expiration = (interval * 1000).ToString();
                        //消息投递到retryExchange中。(death次数会自动增加)
                        Channel.BasicPublish(_options.RetryExchangeName, _options.RoutingKey, e.BasicProperties, e.Body);
                        Channel.BasicAck(e.DeliveryTag, false);
                    }
                };
                if (!string.IsNullOrWhiteSpace(msg))
                {
                    try
                    {
                        var result = func(msg.ToObj<T>());
                        if (result && !_options.QueueAutoAck)
                        {
                            //消费成功，发出应答
                            Channel.BasicAck(e.DeliveryTag, false);
                        }
                        else if (!result && !_options.QueueAutoAck)
                        {
                            //消费失败，重放队列中（可能引起死循环）.todo:或者记录到特定表中
                            act();
                            Console.WriteLine($"result= false引起的Nack, msg = {msg} , e.DeliveryTag ="+ e.DeliveryTag);
                        }
                    }
                    catch (Exception ex)
                    {
                        act();
                        Console.WriteLine($"error引起的Nack .ex= {ex}. ");
                    }
                }
            };
            //BasicConsume要在Received事件注册之后进行，否则可能导致——刚建立连接但还未注册事件时，就已经接收到消息了
            _consumerTag = Channel.BasicConsume(_options.QueueName, _options.QueueAutoAck, consumer);

            //#region 失败消息消费者
            //failConsumer.Received += (s, e) => {
            //    var msg = Encoding.UTF8.GetString(e.Body.ToArray());
            //    Console.WriteLine($"failConsumer = {msg} ");
            //};
            //Channel.BasicConsume(_options.FailQueueName,false,failConsumer);
            //#endregion 失败消息消费者
        }

        /// <summary>
        ///每隔10s拉取n条消息批量消费
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="length"></param>
        public void ConsumerPull<T>(Func<List<T>, bool> func ,  int length=10)
        {           
            Task.Run(()=> {
                var msgList = new List<T>();
                ulong maxDeliveryTag = 0;
                while (true)
                {
                    Thread.Sleep(1000);
                    try
                    {
                        for (var i = 0; i < length; i++)
                        {
                            var data = Channel.BasicGet(_options.QueueName, _options.QueueAutoAck);
                            if (data == null || data.Body.Length == 0)
                            {
                                break;
                            }
                            maxDeliveryTag = data.DeliveryTag;
                            var msg = Encoding.UTF8.GetString(data.Body.ToArray());
                            Trace.WriteLine($"Channelname = [{this.ChannelName}] ,pull到消息：[{msg}], DeliveryTag={data.DeliveryTag}");
                            msgList.Add(JsonConvert.DeserializeObject<T>(msg));
                        }
                        if (msgList == null || msgList.Count == 0)
                        {
                            //一条消息都没有，就多休眠10s
                            Thread.Sleep(1000 * 10);
                            continue;
                        }
                        var funcResult = func.Invoke(msgList);
                        if (funcResult && !_options.QueueAutoAck)
                        {
                            Channel.BasicAck(maxDeliveryTag, true);
                        }
                        else if (!funcResult && !_options.QueueAutoAck)
                        {
                            Channel.BasicNack(maxDeliveryTag, true, true);
                        }
                        msgList.Clear();
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine($"channelName = {this.ChannelName},basicGet error : {ex.Message},errorTag={maxDeliveryTag}");
                        Channel.BasicNack(maxDeliveryTag,true,true);
                    }
                } 
            });
            
        }
        private static object LockConnObj = new object();

        public override IConnection RabbitConnection
        {
            get
            {
                if (_connection == null)
                {
                    lock (LockConnObj)
                    {
                        if (_connFac == null)
                        {
                            _connFac = new ConnectionFactory()
                            {
                                RequestedHeartbeat = TimeSpan.FromSeconds(30),
                                RequestedConnectionTimeout = TimeSpan.FromSeconds(300),
                                AutomaticRecoveryEnabled = true,

                                HostName = _options.HostName,
                                UserName = _options.UserName,
                                Password = _options.Password,
                                VirtualHost = _options.VirtualHost,
                                Port = _options.Port
                            };
                        }
                        if (_connection == null)
                        {
                            _connection = _connFac.CreateConnection();
                        }
                    }
                }
                return _connection;
            }
        }

        private static object LockChannelObj = new object();
        public override IModel Channel
        {
            get
            {
                if (!_channelDic.ContainsKey(ChannelName))
                {
                    lock (LockChannelObj)
                    {
                        if (!_channelDic.ContainsKey(ChannelName))
                        {
                            _channelDic.Add(ChannelName, RabbitConnection.CreateModel());
                        }
                    }
                }
                return _channelDic[ChannelName];
            }
        }

        public new void Dispose()
        {
            //consumer.OnCancel();
            if (Channel != null && !Channel.IsClosed)
            {
                Channel.BasicCancel(_consumerTag);
            }

            base.Dispose();
        }

        ~Consumer()
        {
            Console.WriteLine("consumer 析构函数");
            Dispose();
        }
    }
}
