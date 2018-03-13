using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spyder.Client.IO;
using System.Drawing;

namespace Spyder.Client.Drawing
{
    public static class BitmapHelper
    {
        public static Stream GenerateSolidColorBitmap(Color color, int width = 1024, int height = 1024)
        {
            Stream stream = new MemoryStream();
            GenerateSolidColorBitmap(stream, color, width, height);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public static void GenerateSolidColorBitmap(Stream stream, Color color, int width = 1024, int height = 1024)
        {
            const int bytesPerPixel = 3;
            const int bmpHeaderSize = 14;  //14 bytes
            const int dibHeaderSize = 40;
            const int bitsPerPixel = 24;

            //4-byte alignment required
            int strideBytes = width * bytesPerPixel;
            int padding = 0;
            while (strideBytes % 4 != 0)
            {
                strideBytes++;
                padding++;
            }

            int fileSize = bmpHeaderSize + dibHeaderSize + (strideBytes * height);
            int imageStartPosition = bmpHeaderSize + dibHeaderSize;
            
            //Write out BMP Header
            stream.WriteByte((byte)'B');
            stream.WriteByte((byte)'M');
            WriteInt(stream, fileSize);
            WriteInt(stream, 0);    //Reserved bytes
            WriteInt(stream, imageStartPosition);

            //Write out DIB Header (40 bytes)
            WriteInt(stream, dibHeaderSize);  //Number of bytes in dib header
            WriteInt(stream, width);  //Width of image in pixels
            WriteInt(stream, height); //Height of image in pixels (bottom to top formatting)
            WriteShort(stream, 1);    //Number of planes being used
            WriteShort(stream, bitsPerPixel); //Bit depth
            WriteInt(stream, 0);  //No compression
            WriteInt(stream, strideBytes * height);    //Number of bytes in the pixel array (+ padding)
            WriteInt(stream, 2835);   //Horizontal resolution of the image (hard-coding to 2,835 pixels/meter based on sample)
            WriteInt(stream, 2835);   //Vertical resolution of the image (hard-coding to 2,835 pixels/meter based on sample)
            WriteInt(stream, 0);  //Number of colors in the palette
            WriteInt(stream, 0);  //Number of important colors (0 means all are important)

            //Write pixel data now
            byte[] line = new byte[strideBytes];
            int index = 0;
            for(int i=0 ; i<width ; i++)
            {
                line[index++] = color.B;
                line[index++] = color.G;
                line[index++] = color.R;
            }

            for(int i=0 ; i<height ; i++)
            {
                stream.Write(line, 0, line.Length);
            }

        }

        private static void WriteInt(Stream stream, int value)
        {
            stream.WriteByte((byte)(value));
            stream.WriteByte((byte)(value >> 8));
            stream.WriteByte((byte)(value >> 16));
            stream.WriteByte((byte)(value >> 24));
        }

        private static void WriteShort(Stream stream, short value)
        {
            stream.WriteByte((byte)(value));
            stream.WriteByte((byte)(value >> 8));
        }
    }
}
