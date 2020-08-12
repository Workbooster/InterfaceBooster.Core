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
            if ((x == null) || (y == null))
                return false;
            if (x.Length != y.Length)
                return false;

            for (int i = 0; i < x.Length; i++)
            {
                // check for both values to be NULL
                if (!(x[i] == null && y[i] == null))
                {
                    // check if one of both values is null or if the HashCodes don't match
                    if ((x[i] == null && y[i] != null)
                        || (y[i] == null && x[i] != null)
                         || (!x[i].Equals(y[i])))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public int GetHashCode(object[] obj)
        {
            if (obj == null)
                return 0;

            int hash = 17;

            foreach (var item in obj)
            {
                if (item != null)
                    hash = unchecked(hash * 1031 + item.GetHashCode());
            }

            return hash;
        }
    }
}
