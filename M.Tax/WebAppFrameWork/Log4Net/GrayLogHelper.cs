using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;

namespace WebAppFrameWork.Log4Net
{
    public class GrayLogHelper
    {
        static string Host = ConfigurationManager.AppSettings["graylogHost"];
        static int Port = Convert.ToInt32(ConfigurationManager.AppSettings["graylogPort"]);

        public static void Write(string msg)
        {
            IPHostEntry hostInfo = Dns.GetHostEntry(Host);
            // Get the DNS IP addresses associated with the host.
            IPAddress[] ipAddresses = hostInfo.AddressList;
            IPEndPoint ipep = new IPEndPoint(ipAddresses[0], Port);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            GrayLogEntity model = new GrayLogEntity()
            {
                TraceId = "test111111",
                Duration = "12ms",
                Ip = "127.0.0.1",
                Url = "test.com"
            };
            model.SetSource(Dns.GetHostName());
            model.SetApp("test.boo");
            model.SetLevel(1);
            model.AppendContent(msg);
            model.SetShortMessage("Testlog");
           // var model = new GrayLogModel("shortMsgtest",msg) {   };
            var json = JsonConvert.SerializeObject(model,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new LowPropertyNamesContractResolver()
            }).Replace("\\r", "").Replace("\\n", "").Replace(" ", "").Trim();
            byte[] data = Encoding.UTF8.GetBytes(json);
            server.SendTo(data, data.Length, SocketFlags.None, ipep);//将数据发送到指定的终结点
        }


        public class GrayLogModel
        {
            public GrayLogModel()
            { }
            public GrayLogModel(string shortMsg, string fullMsg)
            {
                this.short_message = shortMsg;
                this.full_message = fullMsg;
            }

            public string version = "1.1";
            public string host
            {
                get { return Dns.GetHostName(); }
            }
            public string short_message { get; set; }

            public string full_message { get; set; }
            public long timestamp
            {
                get
                {
                    return DateTime.Now.Ticks - new DateTime(1970, 1, 1).Ticks;
                }
            }
        }

        public class GrayLogEntity
        {
            /// <summary>
            /// 构造方法
            /// </summary>
            /// <param name="id"></param>
            /// <param name="app"></param>
            /// <param name="source"></param>
            /// <param name="name"></param>
            /// <param name="data"></param>
            /// <param name="level"></param>
            /// <param name="clientSource"></param>
            /// <param name="version"></param>
            public GrayLogEntity()
            {
                this.Timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
                this.data = new StringBuilder();
                this.Version = "v1.1";
            }


            /// <summary>
            /// 序列化
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    ContractResolver = new LowPropertyNamesContractResolver()
                };
                var json = JsonConvert.SerializeObject(this, jsonSerializerSettings);
                return json;
            }

            /// <summary>
            /// 跟踪号
            /// </summary>
            public string TraceId { get; set; }

            /// <summary>
            /// 单号
            /// </summary>
            public string OrderId { get; set; }

            public int Level { get; private set; }

            public GrayLogEntity SetLevel(int level)
            {
                this.Level = level;
                return this;
            }

            /// <summary>
            /// 时间
            /// </summary>
            public long Timestamp { get; private set; }

            /// <summary>
            /// 执行时间
            /// </summary>
            public string Duration { get; set; }

            /// <summary>
            /// IP
            /// </summary>
            public string Ip { get; set; }


            /// <summary>
            /// 浏览器
            /// </summary>
            public string Browser { get; set; }

            /// <summary>
            /// 请求地址
            /// </summary>
            public string Url { get; set; }

            /// <summary>
            /// 租户
            /// </summary>
            public string Tenant { get; set; }


            /// <summary>
            /// 操作员
            /// </summary>
            public string UserId { get; set; }

            /// <summary>
            /// 应用来源
            /// </summary>
            public string App { get; private set; }

            /// <summary>
            /// 设置app
            /// </summary>
            /// <param name="app"></param>
            public GrayLogEntity SetApp(string app)
            {
                this.App = app;
                return this;
            }

            /// <summary>
            /// 消息内容
            /// </summary>
            [JsonProperty("full_message")]
            public string Data
            {
                get
                {
                    //return this.data.ToString().Replace("\\r", "").Replace("\\n","").Replace(" ","").Trim();
                    int maxLength = 20000;
                    var value = this.data.ToString().Replace("\\r", "").Replace("\\n", "").Replace(" ", "").Trim();
                    if (value.Length > maxLength)
                    {
                        return value.Substring(0, maxLength);
                    }
                    return value;
                }
            }

            private StringBuilder data;

            /// <summary>
            /// 添加数据
            /// </summary>
            /// <param name="message"></param>
            public GrayLogEntity AppendContent(string message)
            {
                data.AppendLine(message);
                return this;
            }

            /// <summary>
            /// 来源
            /// </summary>
            public string Source { get; private set; }

            /// <summary>
            /// 设置来源
            /// </summary>
            /// <param name="source"></param>
            public GrayLogEntity SetSource(string source)
            {
                this.Source = source;
                return this;
            }


            /// <summary>
            /// 简介
            /// </summary>
            [JsonProperty("short_message")]
            public string ShortMessage { get; private set; }

            public GrayLogEntity SetShortMessage(string message)
            {
                this.ShortMessage = message;
                return this;
            }

            /// <summary>
            /// 版本
            /// </summary>
            public string Version { get; private set; }

            /// <summary>
            /// 设置版本
            /// </summary>
            /// <param name="version"></param>
            /// <returns></returns>
            public GrayLogEntity SetVersion(string version)
            {
                this.Version = version;
                return this;
            }

            /// <summary>
            /// 业务类型
            /// </summary>
            public string BusinessId { get; private set; }

            /// <summary>
            /// 设置业务类型
            /// </summary>
            /// <param name="businessId"></param>
            /// <returns></returns>
            public GrayLogEntity SetBusinessId(string businessId)
            {
                this.BusinessId = businessId;
                return this;
            }
        }
        public class LowPropertyNamesContractResolver : DefaultContractResolver
        {
            protected override string ResolvePropertyName(string propertyName)
            {
                return propertyName.ToLower();
            }
        }
    }
}