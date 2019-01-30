using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Test.SyneryLanguage.Interpretation.BaseLanguage;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.InterpretationClient_Test
{
    [TestFixture]
    public class Calling_Function_From_Include_File_Works : BaseTest
    {
        [Test]
        public void Getting_Return_Value_From_Include_File_Function_Works()
        {
            string includeFileCode = @"
INT GetFive()
    RETURN 5;
END
";
            string code = @"
INT value = 1;

value = file01.GetFive();
";

            // prepare a list of include files
            Dictionary<string, string> includeFiles = new Dictionary<string, string>();
            includeFiles.Add("empty01", "// nothing to include...");
            includeFiles.Add("file01", includeFileCode);
            includeFiles.Add("empty02", "// nothing to include...");

            _SyneryClient.Run(code, includeFiles);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("value");

            Assert.AreEqual(5, variable.Value);

        }

        [Test]
        public void Function_In_IncludeFile_Calling_Another_Function_From_The_IncludeFile_Works()
        {
            string includeFileCode = @"
INT GetFive()
    RETURN 5;
END
INT GetTen()
    INT five = GetFive();
    RETURN five * 2;
END
";
            string code = @"
INT value = 1;

value = file01.GetTen();
";

            // prepare a list of include files
            Dictionary<string, string> includeFiles = new Dictionary<string, string>();
            includeFiles.Add("empty01", "// nothing to include...");
            includeFiles.Add("file01", includeFileCode);
            includeFiles.Add("empty02", "// nothing to include...");

            _SyneryClient.Run(code, includeFiles);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("value");

            Assert.AreEqual(10, variable.Value);

        }
    }
}
