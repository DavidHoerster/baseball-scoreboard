using System.Threading.Tasks;
using gbac_baseball.web.Model;
using Microsoft.AspNetCore.SignalR;

namespace gbac_baseball.web.Hubs
{

    public class BaseballHub : Hub<IBaseball>
    {
        public async Task Broadcast(string msg) =>
            await Clients.All.Broadcast(msg);

        public async Task FollowTeam(string team)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, team);
            await Clients.Caller.Echo($"You are now following {team}");
        }

        public async Task UnfollowTeam(string team)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, team);
            await Clients.Caller.Echo($"You are now not following {team}");
        }

        public async Task SendEvent(string team, GameEvent evt) =>
            await Clients.Group(team).SendEvent(team, evt);

        public async Task Echo(string message)
        {
            await Clients.All.Echo(message + " (echo from server)");
        }
    }
}