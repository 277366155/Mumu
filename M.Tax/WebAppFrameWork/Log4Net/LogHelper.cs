using log4net;
using System.Configuration;
using System.Web.Services.Protocols;

namespace WebAppFrameWork.Log4Net
{
    public class LogHelper
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger("loginfo");
        public static readonly string logType = ConfigurationManager.AppSettings["logType"];
        public static void Info(string msg)
        {
            if (logType == "graylog")
            {
                GrayLogHelper.Write(msg);
            }
            else
            {
                log.Info(msg);
            }
        }

        public static void Debug(string msg)
        {
            log.Debug(msg);
        }

        public static void Error(string msg)
        {
            log.Error(msg);
        }
    }
}