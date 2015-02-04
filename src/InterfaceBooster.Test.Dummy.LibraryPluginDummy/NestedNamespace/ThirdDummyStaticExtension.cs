using InterfaceBooster.LibraryPluginApi.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Test.Dummy.LibraryPluginDummy.NestedNamespace
{
    public class ThirdDummyStaticExtension : IStaticExtension
    {
        public string Namespace { get { return "FirstLevelNamespace"; } }

        [StaticVariable]
        public string FirstVariable { get; private set; }

        [StaticVariable]
        public string SecondVariable { get; set; }

        public ThirdDummyStaticExtension()
        {
            FirstVariable = "FirstVariable";
            SecondVariable = "SecondVariable";
        }

        [StaticFunction]
        public string FirstMethod(string one)
        {
            return one.ToUpper();
        }
    }
}
