using System.Threading.Tasks;

namespace Spyder.Client.Net.Sockets
{
    public interface ISocket
    {
        bool IsRunning { get; }

        string ServerIP { get; }

        int ServerPort { get; }

        Task<bool> Startup(string serverIP, int serverPort);

        void Shutdown();
    }
}
