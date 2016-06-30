using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spyder.Client.IO;

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
