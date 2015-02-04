using InterfaceBooster.LibraryPluginApi.Static;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Test.Dummy.LibraryPluginDummy
{
    public class FirstDummyStaticExtension : IStaticExtension
    {
        private string _LibraryPluginRuntimeTestPath;

        public string Namespace { get { return null; } }

        [StaticVariable]
        public string FirstVariable { get; private set; }

        [StaticVariable]
        public string SecondVariable { get; set; }

        public FirstDummyStaticExtension()
        {
            string solutionDirectoryPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())));
            _LibraryPluginRuntimeTestPath = Path.Combine(solutionDirectoryPath, @"_TestData\InterfaceBooster\LibraryPluginRuntimeTest");

            FirstVariable = "First Variable Value";
            SecondVariable = "Second Variable Value";
        }

        [StaticFunction]
        public void FirstMethod()
        {
            string methodName = MethodBase.GetCurrentMethod().Name;

            string fileName = String.Format("{0}.txt", methodName);
            string filePath = Path.Combine(_LibraryPluginRuntimeTestPath, fileName);

            File.WriteAllText(filePath, "");
        }

        [StaticFunction]
        public string SecondMethod()
        {
            return "SecondMethod result";
        }

        [StaticFunction]
        public string ThirdMethod(string one)
        {
            return one.ToUpper();
        }

        [StaticFunction]
        public string FourthMethod(string one, string two)
        {
            return one + " " + two;
        }

        [StaticFunction]
        public string FifthMethod(string one, string two, string three = "optional")
        {
            return one + " " + two + " " + three;
        }

        [StaticFunction]
        public string SixthMethod(string one, string two, string three = "optional", string four = "optional 2")
        {
            return one + " " + two + " " + three + " " + four;
        }

        [StaticFunction]
        public string OverloadedMethod(bool one)
        {
            return "bool: " + one.ToString();
        }

        [StaticFunction]
        public string OverloadedMethod(int one)
        {
            return "int: " + one.ToString();
        }

        [StaticFunction]
        public string OverloadedMethod(decimal one)
        {
            return "decimal: " + one.ToString();
        }

        [StaticFunction]
        public string OverloadedMethod(double one)
        {
            return "double: " + one.ToString();
        }

        [StaticFunction]
        public string OverloadedMethod(char one)
        {
            return "char: " + one.ToString();
        }

        [StaticFunction]
        public string OverloadedMethod(DateTime one)
        {
            return "DateTime: " + one.ToString();
        }
    }
}
