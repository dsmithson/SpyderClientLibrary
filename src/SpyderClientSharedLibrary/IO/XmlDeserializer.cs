﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Spyder.Client.Diagnostics;
using Spyder.Client.Primitives;
using Spyder.Client.Common;

namespace Spyder.Client.IO
{
    public delegate void XmlReadFailedHandler(object sender, string propertyName);

    public class XmlDeserializer
    {
        /// <summary>
        /// Event raised when a read request fails, returning a default value
        /// </summary>
        public event XmlReadFailedHandler ElementReadFailed;
        protected void OnElementReadFailed(string elementName)
        {
            if (ElementReadFailed != null)
                ElementReadFailed(this, elementName);
        }

        public XDocument GetXDocument(Stream xmlFileStream, bool skipXmlDeclaration = true)
        {
            if (skipXmlDeclaration)
            {
                //HACK:  Pass over the header (this passes the XML declaration which specifies an encoding of 'us-ascii', which isn't supported on Windows Phone
                // <?xml version="1.0" encoding="us-ascii"?>
                xmlFileStream.Seek(42, SeekOrigin.Begin);
            }

            try
            {
                return XDocument.Load(xmlFileStream);
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(null, TracingLevel.Warning, "{0} occurred while loading file stream: {1}",
                    ex.GetType().Name, ex.Message);

                return null;
            }
        }

        public int Read(XElement parent, string elementName, int defaultValue)
        {
            return Read(parent, elementName, defaultValue, (value) =>
                {
                    int response;
                    return int.TryParse(value, out response) ? response : ReturnDefaultValue(elementName, defaultValue);
                });
        }

        public float Read(XElement parent, string elementName, float defaultValue)
        {
            return Read(parent, elementName, defaultValue, (value) =>
            {
                float response;
                return float.TryParse(value, out response) ? response : ReturnDefaultValue(elementName, defaultValue);
            });
        }

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

        public bool Read(XElement parent, string elementName, bool defaultValue)
        {
            return Read(parent, elementName, defaultValue, (value) =>
            {
                bool response;
                return bool.TryParse(value, out response) ? response : ReturnDefaultValue(elementName, defaultValue);
            });
        }

        public string Read(XElement parent, string elementName, string defaultValue = "")
        {
            return Read(parent, elementName, defaultValue, (value) => value);
        }

        public TEnum ReadEnum<TEnum>(XElement parent, string elementName, TEnum defaultValue)
            where TEnum : struct
        {
            return Read(parent, elementName, defaultValue, (value) =>
                {
                    TEnum response;
                    return Enum.TryParse(value, out response) ? response : ReturnDefaultValue(elementName, defaultValue);
                });
        }

        private T Read<T>(XElement parent, string elementName, T defaultValue, Func<string, T> parser)
        {
            try
            {
                XElement element = ReadElement(parent, elementName);
                if (element != null)
                    return parser(element.Value);
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(null, TracingLevel.Warning, "{0} occurred while deserializing '{1}'.  Returning default value.",
                    ex.GetType().Name, ex.Message);
            }
            return ReturnDefaultValue(elementName, defaultValue);
        }

        public XElement ReadElement(XElement parent, string elementName)
        {
            if (parent == null)
                return null;

            try
            {
                return parent.Element(elementName);                
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(null, TracingLevel.Warning, "{0} occurred while deserializing '{1}'.  Returning default value.",
                    ex.GetType().Name, ex.Message);

                return null;
            }
        }

        private T ReturnDefaultValue<T>(string elementName, T defaultValue)
        {
            OnElementReadFailed(elementName);
            return defaultValue;
        }
    }
}