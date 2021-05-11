using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;

namespace Tax.Common
{
    public static class ConsulRegister
    {

        public static void ConsulRegist(this IConfiguration configuration)
        {
            var client = new ConsulClient(c =>
            {
                c.Address = new Uri(configuration["Consul:ConsulServiceAddress"]);
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
                    HTTP = $"http://{ip}:{port}/healthCheck", //检测地址
                    Timeout = TimeSpan.FromSeconds(5), //多少秒为超时
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5) //在遇到异常后关闭自身服务通道
                }
            }).Wait();
        }
        public static IApplicationBuilder RegisterConsul(this IApplicationBuilder app, IHostApplicationLifetime lifetime, ServiceEntity serviceEntity)
        {
            var consulClient = new ConsulClient(x => x.Address = new Uri(serviceEntity.ConsulServiceAddress));//请求注册的 Consul 地址
            var httpCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),//服务启动多久后注册
                Interval = TimeSpan.FromSeconds(10),//健康检查时间间隔，或者称为心跳间隔
                HTTP = $"http://{serviceEntity.IP}:{serviceEntity.Port}/healthcheck",//健康检查地址
                Timeout = TimeSpan.FromSeconds(5)
            };

            // Register service with consul
            var registration = new AgentServiceRegistration()
            {
                Checks = new[] { httpCheck },
                ID = Guid.NewGuid().ToString(),
                Name = serviceEntity.ServiceName,
                Address = serviceEntity.IP,
                Port = serviceEntity.Port,
                Tags = new[] { $"urlprefix-/{serviceEntity.ServiceName}" }//添加 urlprefix-/servicename 格式的 tag 标签，以便 Fabio 识别
            };

            consulClient.Agent.ServiceRegister(registration).Wait();//服务启动时注册，内部实现其实就是使用 Consul API 进行注册（HttpClient发起）
            lifetime.ApplicationStopping.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();//服务停止时取消注册
            });

            return app;
        }
    }
}
