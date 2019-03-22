using System;
using System.Collections.Generic;
using System.Text;

namespace Spyder.Client.Common
{
    public enum ContentType {  Source, Still }

    /// <summary>
    /// Abstraction over still our source content type
    /// </summary>
    public class Content
    {
        public ContentType Type { get; set; }

        public string Name { get; set; }
    }
}
