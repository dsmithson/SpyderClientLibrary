using Knightware.Primitives;
using Spyder.Client.Common;
using System;
using System.Xml.Linq;

namespace Spyder.Client.IO
{
    public class SpyderXmlDeserializer : XmlDeserializer
    {
        public TimeCode Read(XElement parent, string elementName, TimeCode defaultValue)
        {
            XElement element = ReadElement(parent, elementName);
            if (element == null)
                return defaultValue;

            return new TimeCode(
                ReadEnum(element, "FPS", FieldRate.FR_29_97),
                Read(element, "Hours", 0),
                Read(element, "Minutes", 0),
                Read(element, "Seconds", 0),
                Read(element, "Frames", 0));
        }

        public Color Read(XElement parent, string elementName, Color defaultValue)
        {
            return Read(parent, elementName, defaultValue, (value) =>
            {
                string[] parts = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 3)
                {
                    if (byte.TryParse(parts[0], out byte r) && byte.TryParse(parts[1], out byte g) && byte.TryParse(parts[2], out byte b))
                        return new Color(r, g, b);
                }
                else if (parts.Length == 4)
                {
                    if (byte.TryParse(parts[0], out byte a) && byte.TryParse(parts[1], out byte r) && byte.TryParse(parts[2], out byte g) && byte.TryParse(parts[3], out byte b))
                        return new Color(a, r, g, b);
                }
                return ReturnDefaultValue(elementName, defaultValue);
            });
        }
    }
}
