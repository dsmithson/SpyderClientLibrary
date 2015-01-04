using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Net.Sockets
{
    public delegate void UDPDataReceivedHandler(object sender, DataReceivedEventArgs e);

    public interface IMulticastListener
    {
        bool IsRunning { get; }
        string MulticastIP { get; }
        int MulticastPort { get; }
        
        event UDPDataReceivedHandler DataReceived;

        Task<bool> Startup(string multicastIP, int multicastPort);
        void Shutdown();
    }
}
