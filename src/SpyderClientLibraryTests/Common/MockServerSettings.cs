using Spyder.Client.IO;
using System.Collections.Generic;
using System.IO;

namespace Spyder.Client.Common
{
    public class MockServerSettings : ServerSettings
    {
        public List<string> ReadPropertiesFailed { get; private set; }

        public MockServerSettings()
        {
            ReadPropertiesFailed = new List<string>();
        }

        public override bool Load(Stream systemSettingsStream)
        {
            var deserializer = new SpyderXmlDeserializer();
            deserializer.ElementReadFailed += (sender, propertyName) => ReadPropertiesFailed.Add(propertyName);
            return base.Load(systemSettingsStream, deserializer);
        }
    }
}
