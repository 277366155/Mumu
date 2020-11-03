using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tax.Common;

namespace ConsoleApp1
{
    class Program
    {
       static  string type = ConfigurationManager.AppSettings["type"];
        static void Main(string[] args)
        {

        }


        static void MQTest()
        {
            while (true)
            {
                Console.WriteLine("1-查询队列消息数量\r\n2-发送队列测试消息\r\n");
                var inputNum = Console.ReadLine();

                if (inputNum == "1")
                {
                    MSMQTestCount();
                }
                else if (inputNum == "2")
                {
                    MSMQTest();
                }
            }
        }
        static void MSMQTestCount()
        {
            string path = ".\\private$\\td365_offcommitdata_"+ type;
            var mq = new MessageQueue(path);
            while (true)
            {
                var msList=mq.GetAllMessages();

                Console.WriteLine(msList.Count());
                System.GC.Collect();
                Thread.Sleep(3000);
            }
        }
        static void MSMQTest()
        {
            MessageQueue mq;
            string path = ".\\private$\\td365_offcommitdata_" + type;
            if (MessageQueue.Exists(path))
            {
                mq = new MessageQueue(path);
            }
            else
            {
                mq = MessageQueue.Create(path);
            }
            mq.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });

            Trustee tr = new Trustee("Everyone");
            MessageQueueAccessControlEntry entry = new MessageQueueAccessControlEntry(
                       tr, MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow
                   );
            mq.SetPermissions(entry);

            mq.Send("test");
        }

    }
}
