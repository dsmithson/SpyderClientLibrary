using System;
using System.Threading.Tasks;

namespace Spyder.Client.Net.Sockets
{
    /// <summary>
    /// Generic interface for a socket supporting synchronous communication
    /// </summary>
    public interface IUDPSocket : ISocket
    {
        Task<byte[]> RetrieveDataAsync(byte[] txBuffer, int startIndex, int length, TimeSpan timeout);

        Task<bool> SendDataAsync(byte[] buffer, int startIndex, int length);
    }
}
