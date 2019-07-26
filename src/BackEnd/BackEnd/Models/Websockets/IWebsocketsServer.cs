using System.Threading.Tasks;

namespace BackEnd.Models.Websockets
{
    public interface IWebsocketsServer
    {
        void Start();
        bool TurnOff(string mac);
        bool TurnOn(string mac);
        void NotifyUserAdded(User newUser);
    }
}