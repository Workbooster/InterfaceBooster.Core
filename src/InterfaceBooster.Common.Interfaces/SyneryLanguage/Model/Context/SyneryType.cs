using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context
{
    public class SyneryType
    {
        #region PROPERTIES

        /// <summary>
        /// Gets the synery internal type name. This value is NULL if it is a default primitive .NET type
        /// (e.g. string, int) without a special use in synery.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a public name for the type that for example can be used in exceptions or log messages.
        /// </summary>
        public string PublicName { get { return Name ?? UnterlyingDotNetType.Name; } }

        /// <summary>
        /// Gets the corresponding .NET type for the related value. For example this information can be used for type-casts.
        /// </summary>
        public Type UnterlyingDotNetType { get; private set; }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// This is a container for types in synery that directly or indirectly are related to a .NET type.
        /// </summary>
        /// <param name="underlyingDotNetType"></param>
        /// <param name="name"></param>
        public SyneryType(Type underlyingDotNetType, string name = null)
        {
            UnterlyingDotNetType = underlyingDotNetType;
            Name = name;
        }

        #region EQUALITY

        public static implicit operator SyneryType(Type type)
        {
            SyneryType syneryType = new SyneryType(type);

            return syneryType;
        }

        public static bool operator ==(SyneryType a, SyneryType b)
        {
            // If both are null, or both are same instance, return true

            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false

            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Execute specific equality comparison

            return a.Equals(b);
        }

        public static bool operator !=(SyneryType a, SyneryType b)
        {
            return !(a == b);
        }

        public bool Equals(Type t)
        {
            // If parameter is null return false.
            if (t == null)
            {
                return false;
            }

            return UnterlyingDotNetType == t && Name == null;
        }

        public bool Equals(SyneryType syneryType)
        {
            // If parameter is null return false.
            if (syneryType == null)
            {
                return false;
            }

            // Return true if the fields match:
            return UnterlyingDotNetType == syneryType.UnterlyingDotNetType && Name == syneryType.Name;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + UnterlyingDotNetType.GetHashCode();
            hash = (hash * 7) + Name.GetHashCode();

            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj != null
                && obj is SyneryType)
            {
                Equals((SyneryType)obj);
            }

            return false;
        }

        #endregion

        #endregion
    }
}
