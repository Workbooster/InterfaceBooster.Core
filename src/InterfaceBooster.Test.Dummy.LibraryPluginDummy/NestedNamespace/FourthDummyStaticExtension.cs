using InterfaceBooster.LibraryPluginApi.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Test.Dummy.LibraryPluginDummy.NestedNamespace
{
    public class FourthDummyStaticExtension : IStaticExtension
    {
        public string Namespace { get { return "FirstLevelNamespace.SecondLevelNamespace"; } }

        [StaticVariable]
        public string FirstVariable { get; private set; }

        [StaticVariable]
        public string SecondVariable { get; set; }

        public FourthDummyStaticExtension()
        {
            FirstVariable = "FirstVariable";
            SecondVariable = "SecondVariable";
        }

        [StaticFunction]
        public string FirstMethod(string one)
        {
            return one.ToUpper() + "1";
        }

        [StaticFunction("FirstMethodWithAlternativeName")]
        public string SecondMethod(string one)
        {
            return one.ToUpper() + "2";
        }

        [StaticFunction("SecondMethodWithAlternativeName")]
        public string ThirdMethod(string one)
        {
            return one.ToUpper() + "3";
        }
    }
}
