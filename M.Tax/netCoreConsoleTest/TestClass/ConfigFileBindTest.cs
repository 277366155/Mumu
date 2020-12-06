using Microsoft.Extensions.Configuration;
using System;
using Tax.Common;

namespace netCoreConsoleTest
{
    public class ConfigFileBindTest
    {
        //需引用nuget包：Microsoft.Extensions.Configuration.Binder
        public static void BinderTest()
        {
            var imgPath = new ImgPath();
            Action act = () =>
            {
                //通过bind将配置节点绑定到实体上
                BaseCore.Configuration.GetSection("ImgPath")
                .Bind(imgPath,
                //通过设置BindOptions，使对象私有属性也可以被绑定
                    opt => opt.BindNonPublicProperties = true);
                Console.WriteLine(imgPath.ToJson());
            };
            act.Invoke();
            BaseCore.ConfigurationOnChange(act);
        }
    }

    public class ImgPath
    {
        public string tempPath { get; set; }
        public string savePath { get; private set; }
    }
}
