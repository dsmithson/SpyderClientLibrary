using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Spyder.Client.Net
{
    /// <summary>
    /// Wraps the input and output streams from a socket into a single .Net stream
    /// </summary>
    public class SocketStream : Stream
    {
        private Stream inputStream;
        private Stream outputStream;

        public SocketStream(IInputStream inputStream, IOutputStream outputStream)
        {
            this.inputStream = inputStream.AsStreamForRead();
            this.outputStream = outputStream.AsStreamForWrite();
        }

        public override bool CanRead
        {
            get { return inputStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return inputStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return outputStream.CanWrite; }
        }

        public override void Flush()
        {
            outputStream.Flush();
        }

        public override long Length
        {
            get { return inputStream.Length; }
        }

        public override long Position
        {
            get
            {
                return inputStream.Position;
            }
            set
            {
                inputStream.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return inputStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return inputStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            outputStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            outputStream.Write(buffer, offset, count);
            outputStream.Flush();
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return inputStream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await outputStream.WriteAsync(buffer, offset, count, cancellationToken);
            await outputStream.FlushAsync();
        }
    }
}
