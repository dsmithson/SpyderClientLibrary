using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spyder.Client
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Checks for sequence equality and compares differences in null
        /// </summary>
        /// <returns>True if both sides are null or not null and their contents match</returns>
        public static bool SequenceEqualSafe<T>(this IEnumerable<T> a, IEnumerable<T> b)
        {
            return ReferenceEquals(a, b) ||
                   (a is not null && b is not null && a.SequenceEqual(b));
        }
    }
}
