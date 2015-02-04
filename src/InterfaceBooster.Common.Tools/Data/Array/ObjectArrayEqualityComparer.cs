using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Tools.Data.Array
{
    /// <summary>
    /// <![CDATA[
    /// sometimes LINQ needs an explicit objecr array comparer and it's not possible to use a ArrayEqualityComparer<object[]>()
    /// ]]>
    /// </summary>
    public class ObjectArrayEqualityComparer : IEqualityComparer<object[]>
    {
        public bool Equals(object[] x, object[] y)
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

        public int GetHashCode(object[] obj)
        {
            int hash = 17;

            foreach (var item in obj)
            {
                if (item != null)
                    hash = hash * 23 + item.GetHashCode();
            }

            return hash;
        }
    }
}
