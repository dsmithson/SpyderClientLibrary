using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Knightware.Diagnostics;
using Knightware.Primitives;
using Spyder.Client.Common;
using Knightware.Core;

namespace Spyder.Client.IO
{
    public delegate void XmlReadFailedHandler(object sender, string propertyName);

    public class SpyderXmlDeserializer : XmlDeserializer
    {
        public TimeCode Read(XElement parent, string elementName, TimeCode defaultValue)
        {
            XElement element = ReadElement(parent, elementName);
            if (element == null)
                return defaultValue;

            return new TimeCode()
            {
                Hours = Read(element, "Hours", 0),
                Minutes = Read(element, "Minutes", 0),
                Seconds = Read(element, "Seconds", 0),
                Frames = Read(element, "Frames", 0),
                FieldRate = ReadEnum(element, "FPS", FieldRate.FR_29_97)
            };
        }

        public Color Read(XElement parent, string elementName, Color defaultValue)
        {
            return Read(parent, elementName, defaultValue, (value) =>
            {
                string[] parts = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 3)
                {
                    byte r, g, b;
                    if (byte.TryParse(parts[0], out r) && byte.TryParse(parts[1], out g) && byte.TryParse(parts[2], out b))
                        return new Color(r, g, b);
                }
                else if (parts.Length == 4)
                {
                    byte a, r, g, b;
                    if (byte.TryParse(parts[0], out a) && byte.TryParse(parts[1], out r) && byte.TryParse(parts[2], out g) && byte.TryParse(parts[3], out b))
                        return new Color(a, r, g, b);
                }
                return ReturnDefaultValue(elementName, defaultValue);
            });
        }
    }
}
