using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Tools.Data.Array
{
    public class ArrayEqualityComparer<T> : IEqualityComparer<T[]>
    {
        public bool Equals(T[] x, T[] y)
        {
            return ArrayEqualityComparer.Equals(x, y);
        }

        public int GetHashCode(T[] obj)
        {
            return ArrayEqualityComparer.GetHashCode(obj);
        }
    }

    public static class ArrayEqualityComparer
    {
        public static bool Equals<T>(T[] x, T[] y)
        {
            if (x == y)
                return true;
            if ((x == null) || (y == null))
                return false;
            if (x.Length != y.Length)
                return false;

            if (GetHashCode(x) == GetHashCode(y))
                return true;

            return false;
        }

        public static int GetHashCode<T>(T[] obj)
        {
            int hash = 17;

            foreach (var item in obj)
            {
                hash = hash * 23 + item.GetHashCode();
            }

            return hash;
        }
    }
}
