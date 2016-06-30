using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client
{
    /// <summary>
    /// Provides filter methods for properties and fields
    /// </summary>
    public class MemberFilterProvider
    {
        /// <summary>
        /// Function returning true 
        /// </summary>
        public Func<PropertyInfo, bool> ShouldPropertyBeIncludedHandler { get; set; }
        public Func<FieldInfo, bool> ShouldFieldBeIncludedHandler { get; set; }

        public bool ShouldPropertyBeIncluded(PropertyInfo property)
        {
            if (ShouldPropertyBeIncludedHandler == null || property == null)
                return true;

            return ShouldPropertyBeIncludedHandler(property);
        }

        public bool ShouldFieldBeIncluded(FieldInfo field)
        {
            if (ShouldFieldBeIncludedHandler == null || field == null)
                return true;

            return ShouldFieldBeIncludedHandler(field);
        }

        public static MemberFilterProvider ExcludeMembersContainingAttribute(Type attributeType)
        {
            return new MemberFilterProvider()
            {
                ShouldFieldBeIncludedHandler = (fieldInfo) => fieldInfo.GetCustomAttribute(attributeType) == null,
                ShouldPropertyBeIncludedHandler = (propertyInfo) => propertyInfo.GetCustomAttribute(attributeType) == null
            };
        }
        
        public static MemberFilterProvider ExcludeMembersByName(params string[] memberNames)
        {
            if(memberNames == null || memberNames.Length == 0)
                return new MemberFilterProvider();

            return new MemberFilterProvider()
            {
                ShouldFieldBeIncludedHandler = (fieldInfo) => memberNames.All(member => member != fieldInfo.Name),
                ShouldPropertyBeIncludedHandler = (propertyInfo) => memberNames.All(member => member != propertyInfo.Name)
            };
        }
    }
}
