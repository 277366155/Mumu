using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Tax.AdminWeb.Hubs
{
    /// <summary>
    ///  Hub 类管理连接、组和消息。
    /// </summary>
    public class ChatHub:Hub
    {
        public async Task SendMsg(string user, string msg)
        {
            await Clients.All.SendAsync("ReciveMsg",$"{user},connId=[{Context.ConnectionId}]",msg);
        }
    }
}
