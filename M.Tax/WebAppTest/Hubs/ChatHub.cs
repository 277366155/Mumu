using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppTest.Hubs
{
    public class ChatHub:Hub
    {
        private ILogger<ChatHub> logger1;
        public ChatHub(ILogger<ChatHub> logger)
        {
            logger1 = logger;
        }
        public async Task SendMsg(string user, string msg)
        {
            logger1.LogDebug($"【{user}】 发送了消息：（{msg}）");
            await Clients.All.SendAsync("recived",user,msg);
        }
    }
}
