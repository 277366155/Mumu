using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Tax.Common.Logs
{
    public class GrayLogHelper
    {
        static string Host = BaseCore.AppSetting["graylogHost"];
        static int Port = Convert.ToInt32(BaseCore.AppSetting["graylogPort"]);
        static string app = BaseCore.AppSetting["appName"];
        public static void Log(string shortMsg,string msg)
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(Host), Port);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            var model = new GrayLogModel(shortMsg, msg);
            var json = JsonConvert.SerializeObject(model,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new LowPropertyNamesContractResolver()
            }).Replace("\\r", "").Replace("\\n", "").Trim();
            byte[] data = Encoding.UTF8.GetBytes(json);
            server.SendTo(data, data.Length, SocketFlags.None, ipep);//将数据发送到指定的终结点
        }
    }
    public class LowPropertyNamesContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower();
        }
    }

    public class GrayLogModel
    {
        public GrayLogModel(string shortMsg, string fullMsg)
        {
            this.short_message = shortMsg;
            this.full_message = fullMsg;
        }

        public string version = "1.1";

        public string short_message { get; set; }

        public string full_message { get; set; }

        public string Host
        {
            get
            {
                return Dns.GetHostName(); 
            }
        }


        public long timestamp
        {
            get
            {
                return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            }
        }
    }
}
