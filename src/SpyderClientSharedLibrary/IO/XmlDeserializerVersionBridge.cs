using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Spyder.Client.IO
{
    /// <summary>
    /// Class to offer extension methods for linq queries on Spyder XML files between version 4 and 5 which may or may not include 'Value' sub-nodes
    /// </summary>
    public static class XmlDeserializerVersionBridge
    {
        //
        // Summary:
        //     Projects each element of a sequence to an System.Collections.Generic.IEnumerable`1
        //     and flattens the resulting sequences into one sequence.
        //
        // Parameters:
        //   source:
        //     A sequence of values to project.
        //
        //   selector:
        //     A transform function to apply to each element.
        //
        // Type parameters:
        //   TSource:
        //     The type of the elements of source.
        //
        //   TResult:
        //     The type of the elements of the sequence returned by selector.
        //
        // Returns:
        //     An System.Collections.Generic.IEnumerable`1 whose elements are the result of
        //     invoking the one-to-many transform function on each element of the input sequence.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     source or selector is null.
        public static IEnumerable<XElement> SelectDescendantsByValueResult(this IEnumerable<XElement> source)
        {
            var response = source.SelectMany(item => item.Descendants("Value")).ToList();
            if (response?.Count > 0)
                return response;

            //Grab all descendants - no Value items may be present
            response = source.SelectMany(item => item.Descendants("Item")).ToList();
            return response;
        }
    }
}
