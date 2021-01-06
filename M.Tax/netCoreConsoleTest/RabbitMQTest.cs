using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tax.Common;
using Tax.Common.RabbitMQ;

namespace netCoreConsoleTest
{
   public class RabbitMQTest
    {
        static Publisher publisher;
        static Consumer consumer;
        static RabbitMqOptions opt= new RabbitMqOptions();
        static int num = 0;
        static RabbitMQTest()
        {
            opt = BaseCore.Configuration.GetSection("RabbitMqOptions").Get<RabbitMqOptions>();
            publisher = new Publisher(opt, "booTestChannel");

                Task.Run(() =>
                {
                    for (var i = 0; i < 20; i++)
                    {
                        var consumerName = "consumer_" + i;
                        consumer = new Consumer(opt, "booTestChannel"+i);
                        consumer.ConsumeStart<object>(a =>
                        {
                            Thread.Sleep(1000);
                            Console.WriteLine($"[{consumerName}]接收到消息：" + a.ToJson());
                            num += 1;
                            return num % 2 == 0 ? true : false;
                        });
                    }
                });
        }

        public static void PublisherTest()
        {
            for (var i = 1; i > 0; i++)
            {
                try
                {
                    var data = new { msg = "test.Msg" + i };
                    publisher.Send(data);
                    //Console.WriteLine("发送消息："+data.ToString());
                    Thread.Sleep(200);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"error:"+ex);
                }
            }
        }
        
    }
}
