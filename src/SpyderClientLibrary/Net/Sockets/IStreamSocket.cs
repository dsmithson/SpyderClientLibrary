using System.Threading.Tasks;

namespace Spyder.Client.Net.Sockets
{
    public interface IStreamSocket : ISocket
    {
        bool IsConnected { get; }

        Task<int> ReadAsync(byte[] buffer, int offset, int length);

        Task<int> ReadAsync(byte[] buffer, int offset, int length, int timeout);

        Task WriteAsync(byte[] buffer, int offset, int length);

        Task WriteAsync(byte[] buffer, int offset, int length, int timeout);
    }
}
