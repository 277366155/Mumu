using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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

            consumer = new Consumer(opt, "booTestChannel");
            consumer.ConsumeStart<object>(a=> {
                num += 1;
                return num % 2 == 0 ? true : false;
            });
        }

        public static void PublisherTest()
        {
            for (var i = 1; i > 0; i++)
            {
                try
                {
                    publisher.Send(new { msg = "test.Msg" + i });
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"error:"+ex);
                }
            }
        }
        
    }
}
