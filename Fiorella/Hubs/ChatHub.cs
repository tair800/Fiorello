using Fiorella.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Fiorella.Hubs
{
    public class ChatHub : Hub
    {
        private readonly UserManager<AppUser> _userManager;

        public ChatHub(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message, DateTime.Now.ToString("dd.mm.yyyy"));
        }
        public override Task OnConnectedAsync()
        {
            //Result await in alternatividi
            var user = _userManager.FindByNameAsync(Context.User.Identity.Name).Result;
            //if (user == null) return NotFound();
            user.ConnectionId = Context.ConnectionId;
            var updateResult = _userManager.UpdateAsync(user).Result;
            Clients.All.SendAsync("UserConnected", user.Id);

            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            var user = _userManager.FindByNameAsync(Context.User.Identity.Name).Result;
            user.ConnectionId = null;
            var updateResult = _userManager.UpdateAsync(user).Result;
            Clients.All.SendAsync("UserDisConnected", user.Id);

            return base.OnDisconnectedAsync(exception);
        }
    }
}
