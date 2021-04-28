using Consul;
using Microsoft.Extensions.Configuration;
using System;

namespace Tax.Common
{
    public static class ConsulRegister
    {

        public static void ConsulRegist(this IConfiguration configuration)
        {
            var client = new ConsulClient(c=> {
                c.Address = new Uri(configuration["Consul:ServiceAddress"]);
                c.Datacenter = "dc1";
            });
            //从命令行中获取 ip port weight(权重) 目的是重复反向代理
            string ip = configuration["Consul:IP"];
            int port = int.Parse(configuration["Consul:Port"]);
            //当为空时权重为 10
            int weight = string.IsNullOrWhiteSpace(configuration["Consul:Weight"]) ? 10 : int.Parse(configuration["Consul:Weight"]);
            client.Agent.ServiceRegister(new AgentServiceRegistration()
            {
                ID = "service" + Guid.NewGuid(),//唯一的
                Name = configuration["Consul:ServiceName"],//服务组名称
                Address = ip,//ip需要改动
                Port = port,//不同实例
                Tags = new string[] { weight.ToString() }, //权重设置
                Check = new AgentServiceCheck() //健康检测
                {
                    Interval = TimeSpan.FromSeconds(12), //每隔多久检测一次
                    HTTP = $"http://{ip}:{port}/api/Health/Index", //检测地址
                    Timeout = TimeSpan.FromSeconds(5), //多少秒为超时
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5) //在遇到异常后关闭自身服务通道
                }
            });
        }
    }
}
