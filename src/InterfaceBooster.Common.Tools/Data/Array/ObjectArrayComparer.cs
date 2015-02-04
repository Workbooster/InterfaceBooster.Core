using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Tools.Data.Array
{
    /// <summary>
    /// Contains some methods for comparing the content of two object arrays.
    /// </summary>
    public class ObjectArrayComparer : IComparer<object[]>
    {
        /// <summary>
        /// Compares two arrays containing primitive data types and return the rating (higher = 1, lower = -1 or equal = 0).
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(object[] x, object[] y)
        {
            if (x != null && y != null && x.Length == y.Length)
            {
                for (int i = 0; i < x.Length; i++)
                {
                    int result = TypedComparer(x[i], y[i]);

                    // one of the elements was rated higher than the other
                    // so the result can be returned without continuing the comparison
                    if (result != 0) return result;
                }
            }

            return 0;
        }

        private int TypedComparer(object x, object y)
        {
            if (x == null && y != null) return -1;
            if (y == null && x != null) return 1;
            if (x == null && y == null) return 0;

            // different types cannot be compared
            if (x.GetType() != y.GetType()) return 0;

            if (x is string) return String.Compare((string)x, (string)y);
            if (x is bool) return ((bool)x).CompareTo((bool)y);
            if (x is int) return ((int)x).CompareTo((int)y);
            if (x is decimal) return ((decimal)x).CompareTo((decimal)y);
            if (x is double) return ((double)x).CompareTo((double)y);
            if (x is char) return ((char)x).CompareTo((char)y);
            if (x is DateTime) return ((DateTime)x).CompareTo((DateTime)y);
            if (x is byte) return ((byte)x).CompareTo((byte)y);

            return 0;
        }
    }
}
