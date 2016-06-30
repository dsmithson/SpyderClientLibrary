using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vista.Timers;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Spyder.Client
{
    public delegate Task TestHandler();

    /// <summary>
    /// Handles performance metrics and reporting for derived test classes.
    /// </summary>
    [TestClass]
    public abstract class TestBase<T>
    {
        protected virtual T CreateNewInstance(bool randomize = true)
        {
            Type type = typeof(T);
            var constructor = type.GetConstructor(Type.EmptyTypes);
            Assert.IsNotNull(constructor, "No default constructor found for type: {0}", type.Name);

            T response = (T)constructor.Invoke(null);
            if (randomize)
                UnitTestHelper.RandomizeObject(response);

            return response;
        }

        [TestMethod]
        public void CopyFromTest()
        {
            //Look for a CopyFrom method
            Type type = this.GetType();
            var methods = type.GetMethods().Where(m => m.Name == "CopyFrom" && m.IsPublic).ToList();
            if (methods.Count > 0)
            {
                foreach (var method in methods)
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length != 1)
                        Assert.Inconclusive("Not setup to handle copyfrom operations with more than one parameter");

                    var parameter = parameters[0];
                    if (parameter.ParameterType.IsAssignableFrom(type))
                    {
                        object actual = CreateNewInstance(false);
                        object expected = CreateNewInstance(true);
                        method.Invoke(actual, new object[] { expected });
                        UnitTestHelper.MemberwiseCompareAssert(expected, actual);
                    }
                }
            }
        }
    }
}
