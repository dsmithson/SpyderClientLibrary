using Knightware.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.IO
{
    public static class ByteArrayExtensions
    {
        public static void AddShort(this byte[] destination, ref int index, params short[] values)
        {
            int count = values.Length;
            for (int i = 0; i < count; i++)
            {
                destination[index++] = (byte)(values[i] & 0xff);
                destination[index++] = (byte)((values[i] >> 8) & 0xff);
            }
        }

        public static short GetShort(this byte[] Source, ref int Index)
        {
            byte low = Source[Index++];
            short val = (short)((Source[Index++] << 8) | low);
            return val;
        }

        public static void AddFloat(this byte[] destination, ref int index, params float[] values)
        {
            int count = values.Length;
            for (int i = 0; i < count; i++)
            {
                byte[] bytes = BitConverter.GetBytes(values[i]);
                int byteCount = bytes.Length;
                for (int j = 0; j < byteCount; j++)
                    destination[index++] = bytes[j];
            }
        }

        public static float GetFloat(this byte[] source, ref int index)
        {
            float response = BitConverter.ToSingle(source, index);
            index += BitConverter.GetBytes(response).Length;
            return response;
        }

        public static void AddRectangle(this byte[] destination, ref int index, params Rectangle[] rects)
        {
            if (rects != null && rects.Length > 0)
            {
                foreach (Rectangle rect in rects)
                    AddInt(destination, ref index, rect.X, rect.Y, rect.Width, rect.Height);
            }
        }

        public static Rectangle GetRectangle(this byte[] source, ref int index)
        {
            Rectangle rect = new Rectangle();
            rect.X = GetInt(source, ref index);
            rect.Y = GetInt(source, ref index);
            rect.Width = GetInt(source, ref index);
            rect.Height = GetInt(source, ref index);
            return rect;
        }

        public static void AddInt(this byte[] destination, ref int index, params int[] values)
        {
            int count = values.Length;
            for (int i = 0; i < count; i++)
            {
                destination[index++] = (byte)(values[i] & 0xff);
                destination[index++] = (byte)((values[i] >> 8) & 0xff);
                destination[index++] = (byte)((values[i] >> 16) & 0xff);
                destination[index++] = (byte)((values[i] >> 24) & 0xff);
            }
        }

        public static int GetInt(this byte[] source, ref int index)
        {
            int response = 0;
            for (int i = 0; i < 4; i++)
                response |= ((int)source[index++] << (i * 8));

            return response;
        }

        public static void AddLong(System.Int64 Value, byte[] destination, ref int index)
        {
            destination[index++] = (byte)(Value & 0xff);
            destination[index++] = (byte)((Value >> 8) & 0xff);
            destination[index++] = (byte)((Value >> 16) & 0xff);
            destination[index++] = (byte)((Value >> 24) & 0xff);
            destination[index++] = (byte)((Value >> 32) & 0xff);
            destination[index++] = (byte)((Value >> 40) & 0xff);
            destination[index++] = (byte)((Value >> 48) & 0xff);
            destination[index++] = (byte)((Value >> 56) & 0xff);
        }

        public static long GetLong(this byte[] source, ref int index)
        {
            long response = 0L;
            for (int i = 0; i < 8; i++)
                response |= ((long)source[index++] << (i * 8));

            return response;
        }

        public static void AddString(this byte[] destination, ref int index, params string[] values)
        {
            int count = values.Length;
            for (int i = 0; i < count; i++)
            {
                if (!string.IsNullOrEmpty(values[i]))
                    index += Encoding.UTF8.GetBytes(values[i], 0, values[i].Length, destination, index);

                //Add a string termination character at the end
                destination[index++] = 0x00;
            }
        }

        public static string GetString(this byte[] source, ref int index)
        {
            //Check for empty string
            if (source[index] == 0x00)
            {
                index++;
                return string.Empty;
            }

            //Read bytes until we get the null character (string termination)
            StringBuilder builder = new StringBuilder(25);
            while (true)
            {
                if (source[index] != 0x00)
                    builder.Append((char)source[index++]);
                else
                {
                    //Increment counter past the terminator and exit
                    index++;
                    break;
                }
            }

            //Return our string value
            return builder.ToString();
        }

    }
}
