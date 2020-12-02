using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace Tax.Common.RabbitMQ
{
    public abstract class BaseRabbitMQFactory : IDisposable
    {
        protected static RabbitMqOptions _options;

        public string ChannelName;

        public BaseRabbitMQFactory(RabbitMqOptions options, string channelName)
        {
            _options = options;
            ChannelName = channelName;
            Initialize();
        }

        public abstract IModel Channel { get; }
        public abstract IConnection RabbitConnection { get; }

        protected virtual void Initialize()
        {
            var queueArgs = new Dictionary<string, object>();
            queueArgs.Add("x-dead-letter-exchange", _options.FailExchangeName);
            //创建业务转发器和队列
            Channel.ExchangeDeclare(_options.ExchangeName, _options.ExchangeType, _options.ExchangeDurable, _options.ExchangeAutoDelete, null);
            Channel.QueueDeclare(_options.QueueName, _options.QueueDurable, _options.QueueExclusive, _options.QueueAutoDelete, queueArgs);

            //创建重试消息和失败消息的转发器与队列
            var retryArgs = new Dictionary<string, object>();
            retryArgs.Add("x-dead-letter-exchange", _options.ExchangeName);//设置死信转发器，队列中消息超时时，会被转发到业务转发器
            retryArgs.Add("x-message-ttl", 60000);//定义retry的消息最大的停留时间
            if (!string.IsNullOrWhiteSpace(_options.RetryExchangeName) && !string.IsNullOrWhiteSpace(_options.RetryQueueName))
            {
                Channel.ExchangeDeclare(_options.RetryExchangeName, _options.ExchangeType, _options.ExchangeDurable, _options.ExchangeAutoDelete, null);
                Channel.QueueDeclare(_options.RetryQueueName, _options.QueueDurable, _options.QueueExclusive, _options.QueueAutoDelete, retryArgs);
            }
            if (!string.IsNullOrWhiteSpace(_options.FailExchangeName) && !string.IsNullOrWhiteSpace(_options.FailQueueName))
            {
                Channel.ExchangeDeclare(_options.FailExchangeName, _options.ExchangeType, _options.ExchangeDurable, _options.ExchangeAutoDelete, null);
                Channel.QueueDeclare(_options.FailQueueName, _options.QueueDurable, _options.QueueExclusive, _options.QueueAutoDelete, null);
            }

            //Channel.ConfirmSelect();
            ////设置应答超时时间
            //Channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(3));

            //绑定Exchange与Queue的路由关系
            Channel.QueueBind(_options.QueueName, _options.ExchangeName, _options.RoutingKey);
            Channel.QueueBind(_options.RetryQueueName, _options.RetryExchangeName, _options.RoutingKey);
            Channel.QueueBind(_options.FailQueueName, _options.FailExchangeName, _options.RoutingKey);
        }

        public virtual void Dispose()
        {
            if (Channel != null && !Channel.IsClosed)
            {
                Channel.Dispose();
            }
            if (RabbitConnection != null && RabbitConnection.IsOpen)
            {
                RabbitConnection.Dispose();
            }
        }
    }
}
