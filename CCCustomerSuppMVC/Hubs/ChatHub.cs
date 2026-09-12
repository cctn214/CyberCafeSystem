using Microsoft.AspNetCore.SignalR;

namespace CCCustomerSuppMVC.Hubs
{
    public class ChatHub : Hub
    {
        public async Task ServerRecieveMessageEvent(string user, string message)
        {
            await Clients.All.SendAsync("ClientRecieveMessageEvent", user, message);
        }
    }
}
