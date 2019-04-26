
using System.Threading.Tasks;
using gbac_baseball.web.Model;

namespace gbac_baseball.web.Hubs
{
    public interface IBaseball
    {
        Task Broadcast(string msg);
        Task Echo(string message);
        Task SendEvent(string team, GameEvent evt);
        Task FinalScore(string msg);
    }
}