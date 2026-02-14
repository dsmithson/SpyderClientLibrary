using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spyder.Client.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Spyder.Client
{
    [TestClass]
    public class ReflectionTests
    {
        private IEnumerable<Type> GetTypes()
        {
            return typeof(KeyFrame).Assembly.GetTypes()
                .Where(t => t.GetConstructor(Type.EmptyTypes) != null);
        }

        [TestMethod]
        public void CopyFromTest()
        {
            foreach (Type type in GetTypes().Where(t => t.GetMethods().Any(m => m.Name == "CopyFrom")))
            {
                UnitTestHelper.CopyFromAssert(type);
            }
        }

        [TestMethod]
        public void IEquatableTest()
        {
            foreach (Type type in GetTypes().Where(t => t.GetInterfaces().Any(i => i.Name == "IEquatable")))
            {

            }
        }

        [TestMethod]
        public void PropertyChangedTest()
        {
            foreach (Type type in GetTypes().Where(t => t.GetInterfaces().Contains(typeof(INotifyPropertyChanged))))
            {
                //Create a new instance
                var instance = UnitTestHelper.CreateObject(type, false) as INotifyPropertyChanged;
                if (instance != null)
                {
                    //Register for property changed
                    List<string> propertiesChanged = new List<string>();
                    instance.PropertyChanged += (s, e) => propertiesChanged.Add(e.PropertyName);

                    //Iterate all readable/writeable properties
                    foreach (var propertyInfo in type.GetProperties().Where(p => p.CanRead && p.CanWrite))
                    {
                        propertiesChanged.Clear();
                        object existingValue = propertyInfo.GetValue(instance);
                        propertyInfo.SetValue(instance, existingValue);
                        Assert.IsEmpty(propertiesChanged, $"PropertyChanged should not have been fired when setting same value, but '{string.Join(", ", propertiesChanged)}' was fired");

                        object newValue = UnitTestHelper.GetNewValue(propertyInfo.PropertyType, existingValue);
                        propertyInfo.SetValue(instance, newValue);
                        Assert.AreNotEqual(0, propertiesChanged.Count, $"PropertyChanged was not raised for '{propertyInfo.Name}'");
                        Assert.Contains(propertyInfo.Name, propertiesChanged, $"Expected PropertyChanged event '{propertyInfo.Name}' raised but observed: {string.Join(", ", propertiesChanged)}");
                    }
                }
            }
        }
    }
}
