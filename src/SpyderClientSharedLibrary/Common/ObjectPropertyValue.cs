using System;
using System.Collections.Generic;
using System.Text;

namespace Spyder.Client.Common
{
    /// <summary>
    /// Generic object for storing a properties name, type, and current value
    /// </summary>
    public class ObjectPropertyValue
    {
        public string PropertyName { get; set; }
        public string PropertyType { get; set; }
        public string ValueString { get; set; }

        public override string ToString()
        {
            string propName = string.IsNullOrEmpty(PropertyName) ? "<Unknown>" : PropertyName;
            string value = string.IsNullOrEmpty(ValueString) ? "<Empty>" : ValueString;
            return $"{propName} = {value}";
        }
    }

    public class OutputPropertyValue : ObjectPropertyValue
    {

    }

    public class KeyframePropertyValue : ObjectPropertyValue
    {

    }

    public class InputPropertyValue : ObjectPropertyValue
    {

    }
}
