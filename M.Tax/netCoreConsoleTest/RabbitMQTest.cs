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
        static RabbitMqOptions opt = new RabbitMqOptions();
        static int num = 0;
        static RabbitMQTest()
        {
            opt = BaseCore.Configuration.GetSection("RabbitMqOptions").Get<RabbitMqOptions>();
            publisher = new Publisher(opt, "booTestChannel");

            //Task.Run(() =>
            //{
            //    for (var i = 0; i < 20; i++)
            //    {
            //        var consumerName = "consumer_" + i;
            //        consumer = new Consumer(opt, "booTestChannel" + i);
            //        consumer.ConsumeStart<object>(a =>
            //        {
            //            Thread.Sleep(1000);
            //            Console.WriteLine($"[{consumerName}]接收到消息：" + a.ToJson());
            //            num += 1;
            //            return num % 2 == 0 ? true : false;
            //        });
            //    }
            //});
        }

      

        public static void PublisherTest()
        {
            for (var i = 1; i > 0; i++)
            {
                try
                {
                    var data = new
                    {
                        id = "test.Msg" + i,
                        msg = @"{""Bill"":null,""BillCommitStatus"":0,""ClientSubdivisionType"":0,""HandleServerName"":""iZpi5vojzbfttlZ"",""BillList"":[{""HexId"":null,""BillNo"":""8888302012071648300001"",""ClientCode"":""30"",""SaleWay"":0,""SaleMoney"":5.0,""HexMemberId"":""218"",""MemberCode"":""01"",""MemberName"":""李四"",""RemainScore"":0.0,""SavingRemainAmt"":null,""SourceHexId"":null,""SourceBillNo"":null,""WechatMallSheetNo"":null,""Contacts"":null,""Phone"":null,""Address"":null,""OperatorId"":4,""OperatorCode"":""1001"",""SalemanCode"":null,""SalemanName"":null,""SaleTag"":null,""OperDate"":""2020-12-07 16:48:30"",""Memo"":null,""SaleFlows"":[{""HexId"":null,""RowNo"":0,""BrandId"":0,""CategoryId"":0,""CategoryCode"":null,""VendorId"":0,""HexItemId"":""3e"",""OriginalPrice"":10.0,""Price"":5.0,""Qty"":1.0,""WeightQty"":0.0,""Amount"":5.0,""AllowMemberDiscount"":false,""AllowPromotion"":false,""AllowDiscount"":false,""AllowGive"":false,""AllowChangePrice"":false,""DiscountType"":1,""DiscountType1"":0,""DiscountSheetNo"":"""",""GiftType"":0,""SalesmanId"":0,""MemberTimesCardId"":0,""SalesmanAmt"":null,""ReturnQty"":null,""Memo"":null,""ItemCode"":""6100000000029"",""ItemName"":null,""MinPrice"":0.0,""Specification"":null,""UnitName"":null,""DeductType"":""N"",""DeductValue"":0.0,""IsStock"":null,""IsSerialNumberManagement"":null,""SerialNumber"":"""",""ItemType"":null,""ImagePath"":null,""IsScore"":null,""ScoreValue"":0}],""PayFlows"":[{""RowNo"":1,""PayFlag"":0,""PayAmt"":5.0,""PayScore"":0.0,""PayMemberId"":536,""PayCardNo"":""01"",""PayOrderNo"":null,""Memo"":null,""PaymentId"":30,""CurrencyId"":1,""CurrencyRate"":1.0,""PaymentCode"":""SAV"",""PaymentName"":""储值卡"",""CurrencyCode"":""RMB"",""CurrencyName"":""人民币"",""PayCardType"":null}],""PosFlows"":null,""IsCountScore"":null,""OpenID"":null,""TenantName"":null}],""AppName"":null,""AppVersion"":null,""PKV"":null,""TenantCode"":""256139|4|4|2020-12-07 16:48:33.516"",""SessionKey"":null,""IdentificationCode"":null}"
                    };
                    publisher.Send(data);
                    //Console.WriteLine("发送消息："+data.ToString());
                    //Thread.Sleep(10);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"error:" + ex);
                }
            }
        }

    }
}
