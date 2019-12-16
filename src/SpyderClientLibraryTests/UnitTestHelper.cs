using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace Spyder.Client
{
    public static class UnitTestHelper
    {
        /// <summary>
        /// List of types and associated property names that should be ignored during get/set and property changed testing
        /// </summary>
        private static readonly SortedList<System.Type, List<string>> PropertiesToIgnoreByType;

        static UnitTestHelper()
        {
            PropertiesToIgnoreByType = new SortedList<Type, List<string>>();
            //PropertiesToIgnoreByType.Add(typeof(ObjectTypeToIgnoreHere), new List<string>() { "Property Name(s) to ignore" });
        }

        private static List<string> getIngoreList(System.Type type)
        {
            foreach (var listType in PropertiesToIgnoreByType.Keys)
            {
                if (listType.IsAssignableFrom(type))
                    return PropertiesToIgnoreByType[listType];
            }
            return new List<string>();
        }

        public static void TestForPropertyChanged(INotifyPropertyChanged testObject)
        {
            Type testType = testObject.GetType();

            List<string> propertyNamesChanged = new List<string>();
            testObject.PropertyChanged += (sender, args) =>
                {
                    propertyNamesChanged.Add(args.PropertyName);
                };

            var propertiesToIgnore = getIngoreList(testType);
            foreach (PropertyInfo property in testType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!property.CanWrite || propertiesToIgnore.Contains(property.Name))
                    continue;

                //Check for a property requiring an indexer or key, and ignore these
                if (property.Name == "Item")
                {
                    MethodInfo[] getMethods = property.GetAccessors();
                    if (getMethods.Length == 0 || !getMethods.Any(method => method.GetParameters().Length == 0))
                    {
                        //Not valid to try and test an indexed 'this' property for INotifyPropertyChanged
                        continue;
                    }
                }

                propertyNamesChanged.Clear();
                object previousValue = property.GetValue(testObject, null);
                object expectedValue = GetNewValue(property.PropertyType, previousValue);

                property.SetValue(testObject, expectedValue, null);
                Assert.IsTrue(propertyNamesChanged.Count > 0, "{0} did not fire a PropertyChanged event for property: {1}", testType, property.Name);
                Assert.IsTrue(propertyNamesChanged.Contains(property.Name), "Name of property changed was not correct on {0} property: {1}", testType, property.Name);
            }
        }

        public static void TestPropertyGetSet(object testObject)
        {
            Type testType = testObject.GetType();
            var propertiesToIgnore = getIngoreList(testType);

            foreach (PropertyInfo property in testType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!property.CanWrite || propertiesToIgnore.Contains(property.Name) || property.Name == "Item")
                    continue;

                object previousValue = property.GetValue(testObject, null);
                object expectedValue = GetNewValue(property.PropertyType, previousValue);
                property.SetValue(testObject, expectedValue, null);

                object actualValue = property.GetValue(testObject, null);
                Assert.AreEqual(expectedValue, actualValue, "Value retrieved didn't match set value for object type {0}, property: {1}", testType, property.Name);
            }
        }

        public static object CreateObject(Type targetType, bool randomize = true)
        {
            var constructor = targetType.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
                return null;

            object response = constructor.Invoke(null);
            if (randomize)
                RandomizeObject(response);

            return response;
        }

        public static void RandomizeObject(object target)
        {
            Type targetType = target.GetType();

            //Change Property Values
            foreach (PropertyInfo property in targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!property.CanWrite)
                    continue;

                object currentValue = property.GetValue(target, null);
                object expectedValue = GetNewValue(property.PropertyType, currentValue);
                property.SetValue(target, expectedValue, null);
            }

            //Change Field Values
            foreach (FieldInfo field in targetType.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (field.IsInitOnly)
                    continue;

                object currentValue = field.GetValue(target);
                object newValue = GetNewValue(field.FieldType, currentValue);
                field.SetValue(target, newValue);
            }
        }

        public static object GetNewValue(Type valueType, object currentValue)
        {
            if (valueType.IsEnum)
            {
                //Note: In some scenarios where the enum is of a flags type, there may be multiple display values with the same underlying values.  Need
                //to check for this
                Array values = Enum.GetValues(valueType);
                Type underlyingType = Enum.GetUnderlyingType(valueType);
                foreach (object value in values)
                {
                    if (underlyingType == typeof(byte))
                    {
                        if ((byte)value != (byte)currentValue)
                            return value;
                    }
                    else
                    {
                        if ((int)value != (int)currentValue)
                            return value;
                    }
                }
            }
            else if (valueType == typeof(System.Drawing.Color))
            {
                var color = (System.Drawing.Color)currentValue;
                return System.Drawing.Color.FromArgb(
                    (color.A == 255 ? 0 : color.A + 1),
                    (color.R == 255 ? 0 : color.R + 1),
                    (color.G == 255 ? 0 : color.G + 1),
                    (color.B == 255 ? 0 : color.B + 1));
            }
            else if (valueType == typeof(System.Windows.Media.Color))
            {
                var color = (System.Windows.Media.Color)currentValue;
                return System.Windows.Media.Color.FromArgb(
                    (byte)(color.A == 255 ? 0 : color.A + 1),
                    (byte)(color.R == 255 ? 0 : color.R + 1),
                    (byte)(color.G == 255 ? 0 : color.G + 1),
                    (byte)(color.B == 255 ? 0 : color.B + 1));
            }
            else if (valueType == typeof(Knightware.Primitives.Color))
            {
                var color = (Knightware.Primitives.Color)currentValue;
                return new Knightware.Primitives.Color(
                    (byte)(color.A == 255 ? 0 : color.A + 1),
                    (byte)(color.R == 255 ? 0 : color.R + 1),
                    (byte)(color.G == 255 ? 0 : color.G + 1),
                    (byte)(color.B == 255 ? 0 : color.B + 1));
            }
            else if (valueType == typeof(System.Drawing.Rectangle))
            {
                var rect = (System.Drawing.Rectangle)currentValue;
                return new System.Drawing.Rectangle(
                    (rect.X == int.MaxValue ? 1 : rect.X + 1),
                    (rect.Y == int.MaxValue ? 1 : rect.Y + 1),
                    (rect.Width == int.MaxValue ? 1 : rect.Width + 1),
                    (rect.Height == int.MaxValue ? 1 : rect.Height + 1));
            }
            else if (valueType == typeof(System.Windows.Rect))
            {
                var rect = (System.Windows.Rect)currentValue;
                return new System.Windows.Rect(
                    (rect.X == double.MaxValue || double.IsInfinity(rect.X) ? 0 : rect.X + 1),
                    (rect.Y == double.MaxValue || double.IsInfinity(rect.Y) ? 0 : rect.Y + 1),
                    (rect.Width == double.MaxValue || double.IsInfinity(rect.Width) ? 0 : rect.Width + 1),
                    (rect.Height == double.MaxValue || double.IsInfinity(rect.Height) ? 0 : rect.Height + 1));
            }
            else if (valueType == typeof(Knightware.Primitives.Rectangle))
            {
                var rect = (Knightware.Primitives.Rectangle)currentValue;
                return new Knightware.Primitives.Rectangle(
                    (rect.X == int.MaxValue ? 1 : rect.X + 1),
                    (rect.Y == int.MaxValue ? 1 : rect.Y + 1),
                    (rect.Width == int.MaxValue ? 1 : rect.Width + 1),
                    (rect.Height == int.MaxValue ? 1 : rect.Height + 1));
            }
            else if (valueType == typeof(System.Windows.Size))
            {
                var size = (System.Windows.Size)currentValue;
                return new System.Windows.Size(
                    (size.Width == double.MaxValue ? 1 : size.Width + 1),
                    (size.Height == double.MaxValue ? 1 : size.Height + 1));
            }
            else if (valueType == typeof(Knightware.Primitives.Size))
            {
                var size = (Knightware.Primitives.Size)currentValue;
                return new Knightware.Primitives.Size(
                    (size.Width == int.MaxValue ? 1 : size.Width + 1),
                    (size.Height == int.MaxValue ? 1 : size.Height + 1));
            }
            else if (valueType == typeof(System.Drawing.Point))
            {
                var point = (System.Drawing.Point)currentValue;
                return new System.Drawing.Point(
                    ((point.X == int.MaxValue) ? 0 : point.X + 1),
                    ((point.Y == int.MaxValue) ? 0 : point.Y + 1));
            }
            else if (valueType == typeof(System.Windows.Point))
            {
                var point = (System.Windows.Point)currentValue;
                return new System.Windows.Point(
                    (point.X == double.MaxValue || double.IsInfinity(point.X) ? 0 : point.X + 1),
                    (point.Y == double.MaxValue || double.IsInfinity(point.Y) ? 0 : point.Y + 1));
            }
            else if (valueType == typeof(Knightware.Primitives.Point))
            {
                var point = (Knightware.Primitives.Point)currentValue;
                return new Knightware.Primitives.Point(
                    ((point.X == int.MaxValue) ? 0 : point.X + 1),
                    ((point.Y == int.MaxValue) ? 0 : point.Y + 1));
            }
            else if (valueType == typeof(Knightware.Primitives.Thickness))
            {
                var thickness = (Knightware.Primitives.Thickness)currentValue;
                return new Knightware.Primitives.Thickness(
                    (thickness.Left == double.MaxValue ? 0 : thickness.Left + 1),
                    (thickness.Top == double.MaxValue ? 0 : thickness.Top + 1),
                    (thickness.Right == double.MaxValue ? 0 : thickness.Right + 1),
                    (thickness.Bottom == double.MaxValue ? 0 : thickness.Bottom + 1));
            }
            else if (valueType == typeof(bool))
            {
                return !(bool)currentValue;
            }
            else if (valueType == typeof(byte))
            {
                byte current = (byte)currentValue;
                return (byte)(current == byte.MaxValue ? 0 : current + 1);
            }
            else if (valueType == typeof(UInt16))
            {
                return (UInt16)((UInt16)currentValue + 1);
            }
            else if (valueType == typeof(int))
            {
                return (int)currentValue + 1;
            }
            else if (valueType == typeof(long))
            {
                return (long)currentValue + 1;
            }
            else if (valueType == typeof(double))
            {
                return (double)currentValue + 1;
            }
            else if (valueType == typeof(float))
            {
                return (float)currentValue + 1;
            }
            else if (valueType == typeof(string))
            {
                if (currentValue == null)
                    return "1";
                else
                    return (string)currentValue + "1";
            }
            else if (valueType == typeof(TimeSpan))
            {
                return ((TimeSpan)currentValue).Add(TimeSpan.FromSeconds(5));
            }
            else if (valueType == typeof(DateTime))
            {
                return ((DateTime)currentValue).AddSeconds(5);
            }
            else if (valueType == typeof(Guid))
            {
                return Guid.NewGuid();
            }
            else if (valueType == typeof(Type))
            {
                if (currentValue is string)
                    return typeof(int);
                else
                    return typeof(string);
            }
            else if (valueType == typeof(System.Windows.Point))
            {
                var currentPoint = (System.Windows.Point)currentValue;
                return new System.Windows.Point()
                {
                    X = currentPoint.X + 1,
                    Y = currentPoint.Y = 1
                };
            }
            else if (valueType == typeof(IPAddress))
            {
                IPAddress currentAddress = currentValue as IPAddress;
                if (currentAddress == null)
                    return IPAddress.Parse("1.2.3.4");
                else
                {
                    byte[] bytes = currentAddress.GetAddressBytes();
                    bytes[0] = (byte)(bytes[0] == 0 ? 1 : 0);
                    return new IPAddress(bytes);
                }
            }
            else if (valueType == typeof(Stream))
            {
                return new MemoryStream(new byte[] { 0xF2, 0xE1 });
            }
            else if (valueType == typeof(bool?))
            {
                return currentValue == null ? true : (bool?)null;
            }
            else if (valueType.IsInterface)
            {
                //Look for an implementation of this interface with a default constructor, and then randomize and return it
                foreach (Type type in Assembly.GetAssembly(valueType).GetTypes())
                {
                    if (valueType.IsAssignableFrom(type))
                    {
                        var constructor = type.GetConstructor(Type.EmptyTypes);
                        if (constructor != null)
                        {
                            object instance = constructor.Invoke(null);
                            RandomizeObject(instance);
                            return instance;
                        }
                    }
                }

                Assert.Fail("Failed to find a class implementing the supplied interface: " + valueType.Name);
            }
            else if (valueType.IsClass || !valueType.IsValueType)
            {
                Type typeToConstruct = valueType;
                if (valueType.IsAbstract)
                {
                    //Find a derived type that we can instantiate
                    foreach (var assemblyType in valueType.Assembly.GetTypes())
                    {
                        if (!assemblyType.IsAbstract && valueType.IsAssignableFrom(assemblyType))
                        {
                            typeToConstruct = assemblyType;
                            break;
                        }
                    }
                }
                else if (valueType.IsArray)
                {
                    Array currentValueArray = currentValue as Array;
                    int length = (currentValue == null ? 1 : currentValueArray.Length);

                    Type elementType = valueType.GetElementType();
                    Array newArray = Array.CreateInstance(elementType, length);

                    //Randomize array contents
                    for (int i = 0; i < length; i++)
                    {
                        object arrayPreviousValue = newArray.GetValue(i);
                        if (currentValueArray != null && currentValueArray.Length > i)
                        {
                            arrayPreviousValue = currentValueArray.GetValue(i);
                        }

                        object arrayNewValue = GetNewValue(elementType, arrayPreviousValue);
                        newArray.SetValue(arrayNewValue, i);
                    }
                    return newArray;
                }

                //See if we can construct a new one and pass it back randomized
                object response = null;
                ConstructorInfo constructor = typeToConstruct.GetConstructor(new System.Type[0]);
                if (constructor == null)
                {
                    //Need to provide one or more constructor parameters to instantiate this object
                    var constructorArgs = new List<object>();
                    constructor = typeToConstruct.GetConstructors(BindingFlags.Public)[0];
                    foreach (var parameter in constructor.GetParameters())
                    {
                        object parameterValue = Activator.CreateInstance(parameter.ParameterType);
                        RandomizeObject(parameterValue);
                        constructorArgs.Add(parameterValue);
                    }
                    response = constructor.Invoke(constructorArgs.ToArray());
                }
                else
                {
                    //Invoke the null constructor
                    response = constructor.Invoke(null);
                }

                if (!(response is ICollection))
                    RandomizeObject(response);

                return response;
            }

            throw new NotSupportedException("Specified type not registered: " + valueType.Name);
        }

        public static void MemberwiseCompareAssert(object expected, object actual, string errorMessage = "Objects did not match", MemberFilterProvider memberFilter = null)
        {
            string failMessage;
            Assert.IsTrue(MemberwiseCompare(expected, actual, out failMessage, memberFilter), errorMessage + ": " + failMessage);
        }

        public static bool MemberwiseCompare(object expected, object actual, out string failMessage, MemberFilterProvider memberFilter = null)
        {
            failMessage = string.Empty;

            //If both are null, then they match
            if (expected == null && actual == null)
                return true;

            //If only one is null, then they do not match
            if (expected == null || actual == null)
            {
                failMessage = "One of the objects being compared was null";
                return false;
            }

            //If the types are different, then they do not match
            Type objectType = expected.GetType();
            if (objectType != actual.GetType())
            {
                failMessage = string.Format("Types did not match.  Expected {0} but found {1}.", objectType.Name, actual.GetType().Name);
                return false;
            }

            //If this is a collection, iterate it
            if (objectType.IsPrimitive || objectType == typeof(string))
            {
                return expected.Equals(actual);
            }
            else if (typeof(ICollection).IsAssignableFrom(objectType))
            {
                ICollection collection1 = (ICollection)expected;
                ICollection collection2 = (ICollection)actual;
                if (collection1.Count != collection2.Count)
                {
                    failMessage = "Collection counts were different";
                    return false;
                }

                var enumerator1 = collection1.GetEnumerator();
                var enumerator2 = collection2.GetEnumerator();
                for (int i = 0; i < collection1.Count; i++)
                {
                    enumerator1.MoveNext();
                    enumerator2.MoveNext();

                    if (!MemberwiseCompare(enumerator1.Current, enumerator2.Current, out failMessage, memberFilter))
                    {
                        failMessage = "Collection Item Compare Fail: " + failMessage;
                        return false;
                    }
                }

                //Match
                return true;
            }
            else if (objectType.IsClass)
            {
                //IP Address has an issue with checking it's scopeID, so we'll force a equals on IPAddress directly
                if (objectType == typeof(IPAddress))
                    return expected.Equals(actual);

                //Compare Property values
                foreach (PropertyInfo property in objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    //Check for property filter
                    if (memberFilter != null && !memberFilter.ShouldPropertyBeIncluded(property))
                    {
                        //Ignore filter
                        continue;
                    }

                    //Check for a property requiring an indexer or key, and continue on these
                    MethodInfo[] getMethods = property.GetAccessors();
                    if (getMethods.Length == 0)
                    {
                        //Unable to test this property without a getter
                    }
                    else if (getMethods.Any(method => method.GetParameters().Length == 0))
                    {
                        //Do a typical property comparison
                        object object1Value = property.GetValue(expected, null);
                        object object2Value = property.GetValue(actual, null);
                        if (!MemberwiseCompare(object1Value, object2Value, out failMessage, memberFilter))
                        {
                            failMessage = string.Format("{0} Property on {1}: {2}", property.Name, objectType.Name, failMessage);
                            return false;
                        }
                    }
                    else
                    {
                        //This property requires a getter with an argument.  If this type is an enum, test all possibilities on the enum.
                        var enumGetMethods = getMethods.Where(method => method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType.IsEnum).ToArray();
                        if (enumGetMethods != null && enumGetMethods.Length > 0)
                        {
                            foreach (MethodInfo method in enumGetMethods)
                            {
                                Type enumType = method.GetParameters()[0].ParameterType;
                                foreach (object enumValue in Enum.GetValues(enumType))
                                {
                                    object object1Value = method.Invoke(expected, new object[] { enumValue });
                                    object object2Value = method.Invoke(actual, new object[] { enumValue });
                                    if (!MemberwiseCompare(object1Value, object2Value, out failMessage, memberFilter))
                                    {
                                        failMessage = string.Format("{0} Property on {1} (Enum parameter of {2}): {3}", property.Name, objectType.Name, enumValue, failMessage);
                                        return false;
                                    }
                                }
                            }
                        }

                        //TODO:  Wire up a similar loop for other indexers
                        if (enumGetMethods.Length != getMethods.Length)
                        {
                            failMessage = string.Format("Failed to test all getters for property {0} on {1}", property.Name, objectType.Name);
                            return false;
                        }
                    }
                }

                //Change Field Values
                foreach (FieldInfo field in objectType.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (memberFilter != null && !memberFilter.ShouldFieldBeIncluded(field))
                    {
                        //Ignore
                        continue;
                    }

                    object object1Value = field.GetValue(expected);
                    object object2Value = field.GetValue(actual);
                    if (!MemberwiseCompare(object1Value, object2Value, out failMessage, memberFilter))
                    {
                        failMessage = string.Format("{0} Field on {1}: {2}", field.Name, objectType.Name, failMessage);
                        return false;
                    }
                }

                //Match
                return true;
            }
            else
            {
                return expected.Equals(actual);
            }
        }

        public static void CompareFilesAssert(string file1, string file2)
        {
            string message;
            Assert.IsTrue(CompareFiles(file1, file2, out message), message);
        }

        public static bool CompareFiles(string file1, string file2, out string message)
        {
            using (FileStream file1Stream = File.OpenRead(file1))
            {
                using (FileStream file2Stream = File.OpenRead(file2))
                {
                    return CompareFiles(file1Stream, file2Stream, out message);
                }
            }
        }

        public static void CompareFilesAssert(Stream file1, Stream file2)
        {
            string message;
            Assert.IsTrue(CompareFiles(file1, file2, out message), message);
        }

        public static bool CompareFiles(Stream file1, Stream file2, out string message)
        {
            if (file1.Length != file2.Length)
            {
                message = string.Format("File lengths differ: File 1 = {0}, File 2 = {1}", file1.Length, file2.Length);
                return false;
            }

            const int bufferSize = 8192;
            byte[] buffer1 = new byte[bufferSize];
            byte[] buffer2 = new byte[bufferSize];

            message = string.Empty;
            while (file1.Position < file1.Length)
            {
                int count1 = file1.Read(buffer1, 0, buffer1.Length);
                int count2 = file2.Read(buffer2, 0, buffer2.Length);
                if (count1 != count2)
                {
                    message = "Read count failed between the two provided streams";
                    return false;
                }

                string loopMessage = string.Empty;
                Parallel.For(0, count1, (i, loop) =>
                    {
                        byte expected = buffer1[i];
                        byte actual = buffer2[i];
                        if (expected != actual)
                        {
                            loopMessage = string.Format("Offset {0} of buffer differed.  Expected {1:X2}, received {2:X2}", i, expected, actual);
                            loop.Stop();
                        }
                    });

                //Was an error detected?
                if (!string.IsNullOrEmpty(loopMessage))
                {
                    message = loopMessage;
                    break;
                }
            }

            //Was there an error?
            return string.IsNullOrEmpty(message);
        }

        public static void CreateStillImage(string fileName, int width = 640, int height = 480)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);

            using (Bitmap bitmap = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.Blue);
                }
                bitmap.Save(fileName);
            }
        }

        /// <summary>
        /// Determines if a specified type explicitly defines an interface type, ignoring inherited interfaces
        /// </summary>
        public static bool IsInterfaceDeclared(Type type, Type interfaceType)
        {
            var definedTypes = GetDefinedInterfaces(type);
            bool response = definedTypes.Any(definedType => definedType.Equals(interfaceType));
            return response;
        }

        /// <summary>
        /// Gets interfaces defined explicitly on the supplied type, ignoring any inherited interfaces
        /// </summary>
        public static IEnumerable<Type> GetDefinedInterfaces(Type type)
        {
            Type[] interfaces = type.GetInterfaces();
            if (interfaces != null)
            {
                foreach (Type interfaceType in interfaces)
                {
                    InterfaceMapping mapping = type.GetInterfaceMap(interfaceType);
                    if (mapping.TargetMethods.Length > 0 && mapping.TargetMethods[0].DeclaringType == type)
                        yield return interfaceType;
                }
            }
        }

        public static void CopyFromAssert(Type type)
        {
            //Look for a CopyFrom method
            var methods = type.GetMethods().Where(m => m.Name == "CopyFrom" && m.IsPublic).ToList();
            if (methods.Count > 0)
            {
                foreach (var method in methods)
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length == 1)
                    {
                        var parameter = parameters[0];
                        if (parameter.ParameterType.IsAssignableFrom(type))
                        {
                            object actual = CreateObject(type, false);
                            object expected = CreateObject(type, true);
                            method.Invoke(actual, new object[] { expected });
                            UnitTestHelper.MemberwiseCompareAssert(expected, actual);
                        }
                    }
                }
            }
        }
    }
}
