using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppTest.Hubs
{
    public class ChatHub:Hub
    {
        public async Task SendMsg(string user, string msg)
        {
            await Clients.All.SendAsync("recived",user,msg);
        }
    }
}
