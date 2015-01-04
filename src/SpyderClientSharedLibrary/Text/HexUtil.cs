using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Text
{
    public static class HexUtil
    {
        public static bool IsValidHexCharLength(string hexString)
        {
            return hexString.Length % 2 == 0;
        }

        public static byte[] GetBytes(string hexString)
        {
            if (string.IsNullOrEmpty(hexString))
                return new byte[0];

            if (!IsValidHexCharLength(hexString))
                throw new ArgumentException("Character count for hex string must be divisible by 2 to be valid", "hexString");
            
            byte[] response = new byte[hexString.Length / 2];

            int index = 0;
            int parseIndex = 0;
            while(parseIndex < hexString.Length)
            {
                string subString = hexString.Substring(parseIndex, 2);
                response[index++] = byte.Parse(subString, System.Globalization.NumberStyles.HexNumber);
                parseIndex += 2;
            }
            return response;
        }

        public static string GetString(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("X2"));
            }
            return builder.ToString();
        }
    }
}
