using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Resort.Web.ChatHub
{
   // [Authorize]
    public class ChatHubs : Hub
    {
        public void SendMessage(string name, string message)
        {
            //call db and save chats if u need that 
            Clients.All.SendAsync("myNewMessage", name, message);
        }

        [HubMethodName("joinGroup")] // u can give another name and call it by JS 
        public void JoinGroup(string gName, string name)
        {
            //call db and save chats if u need that 
            //......
            // add to group
            Groups.AddToGroupAsync(Context.ConnectionId, gName);


            // broadcast to other
            Clients.OthersInGroup(gName).SendAsync("newMember", name, gName);
        }
        public void SendToGroup(string name, string msg, string gName)
        {
            // save in db if u need
            Clients.Group(gName).SendAsync("newMessageGroup", name, msg, gName);
        }

        public override Task OnConnectedAsync()
        {
            string conId = Context.ConnectionId;
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
