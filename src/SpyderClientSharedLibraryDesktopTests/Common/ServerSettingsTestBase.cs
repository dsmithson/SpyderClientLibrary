using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Spyder.Client.Common
{
    [TestClass]
    public abstract class ServerSettingsTestBase
    {
        protected abstract Stream GetTestSystemSettingsStream();

        protected string[] propertiesToIgnore;

        [TestMethod]
        public void LoadTest()
        {
            var settings = new MockServerSettings();
            Assert.IsTrue(settings.Load(GetTestSystemSettingsStream()), "Failed to load settings");

            //Remove any properties that we explicitly want to ignore
            if(propertiesToIgnore != null)
            {
                foreach(string propertyToIgnore in propertiesToIgnore)
                {
                    if (settings.ReadPropertiesFailed.Contains(propertyToIgnore))
                        settings.ReadPropertiesFailed.Remove(propertyToIgnore);
                }
            }

            if (settings.ReadPropertiesFailed.Count > 0)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("The following properties failed to deserialize:");
                foreach (string property in settings.ReadPropertiesFailed)
                {
                    builder.AppendLine(property);
                }
                Assert.Fail(builder.ToString());
            }
        }
    }
}
