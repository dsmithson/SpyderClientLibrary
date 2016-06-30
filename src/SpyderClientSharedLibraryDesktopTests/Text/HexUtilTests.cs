using Knightware.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Text
{
    [TestClass]
    public class HexUtilTests
    {
        [TestMethod]
        public void GetStringTest()
        {
            var expected = GenerateFullTestArray();
            string actual = HexUtil.GetString(expected.Item2);
            Assert.AreEqual(expected.Item1.Length, actual.Length, "Response length didn't match");
            for (int i = 0; i < actual.Length / 2; i++)
            {
                string expectedSubString = expected.Item1.Substring(i * 2, 2);
                string actualSubString = actual.Substring(i * 2, 2);
                Assert.AreEqual(expectedSubString, actualSubString, "Response string didn't match for byte offset " + i);
            }
        }

        [TestMethod]
        public void GetBytesTest()
        {
            var expected = GenerateFullTestArray();
            byte[] actual = HexUtil.GetBytes(expected.Item1);
            Assert.AreEqual(expected.Item2.Length, actual.Length, "Length of byte[] response was incorrect");
            for (int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(expected.Item2[i], actual[i], "Byte value was incorrect at offset {0}", i);
            }
        }

        private Tuple<string, byte[]> GenerateFullTestArray()
        {
            List<char> hexChars = new List<char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
            MemoryStream stream = new MemoryStream();
            StringBuilder builder = new StringBuilder();

            foreach (char firstChar in hexChars)
            {
                foreach (char secondChar in hexChars)
                {
                    builder.Append(firstChar);
                    builder.Append(secondChar);

                    byte b = (byte)((hexChars.IndexOf(firstChar) << 4) | (hexChars.IndexOf(secondChar)));
                    stream.WriteByte(b);
                }
            }

            var response = new Tuple<string, byte[]>(builder.ToString(), stream.ToArray());
            stream.Dispose();

            return response;
        }
    }
}
