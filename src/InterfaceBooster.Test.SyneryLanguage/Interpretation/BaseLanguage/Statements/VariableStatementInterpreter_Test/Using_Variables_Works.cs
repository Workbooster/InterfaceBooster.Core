using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Core.ProviderPlugins;
using InterfaceBooster.SyneryLanguage.Interpretation;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.ProviderPlugin.Interfaces;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.BaseLanguage.Statements.VariableStatementInterpreter_Test
{
    [TestFixture]
    public class Using_Variables_Works : BaseTest
    {

        #region TEST DECLARATION

        [Test]
        public void Declare_STRING_Works()
        {
            string code = @"STRING test;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(typeof(string), variable.Type.UnterlyingDotNetType);
        }

        [Test]
        public void Declare_BOOL_Works()
        {
            string code = @"BOOL test;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(typeof(bool), variable.Type.UnterlyingDotNetType);
        }

        [Test]
        public void Declare_INT_Works()
        {
            string code = @"INT test;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(typeof(int), variable.Type.UnterlyingDotNetType);
        }

        [Test]
        public void Declare_DECIMAL_Works()
        {
            string code = @"DECIMAL test;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(typeof(decimal), variable.Type.UnterlyingDotNetType);
        }

        [Test]
        public void Declare_DOUBLE_Works()
        {
            string code = @"DOUBLE test;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(typeof(double), variable.Type.UnterlyingDotNetType);
        }

        [Test]
        public void Declare_CHAR_Works()
        {
            string code = @"CHAR test;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(typeof(char), variable.Type.UnterlyingDotNetType);
        }

        [Test]
        public void Declare_DATETIME_Works()
        {
            string code = @"DATETIME test;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(typeof(DateTime), variable.Type.UnterlyingDotNetType);
        }

        #endregion

        #region ASSIGN TEST

        [Test]
        public void Assign_STRING_Works()
        {
            string code = "STRING test = \"some text\";";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual("some text", variable.Value);
        }

        [Test]
        public void Assign_Newline_STRING_Works()
        {
            string code = @"
STRING testNR = ""\r\n"";
STRING testN = ""\n"";
STRING testR = ""\r"";
STRING testTab = ""\t"";
";

            _SyneryClient.Run(code);

            IValue variableNR = _SyneryClient.Memory.CurrentScope.ResolveVariable("testNR");
            IValue variableN = _SyneryClient.Memory.CurrentScope.ResolveVariable("testN");
            IValue variableR = _SyneryClient.Memory.CurrentScope.ResolveVariable("testR");
            IValue variableTab = _SyneryClient.Memory.CurrentScope.ResolveVariable("testTab");

            Assert.AreEqual(Environment.NewLine, variableNR.Value);
            Assert.AreEqual("\n", variableN.Value);
            Assert.AreEqual("\r", variableR.Value);
            Assert.AreEqual("\t", variableTab.Value);
        }

        [Test]
        public void Assign_STRING_With_Escape_Sequence_Works()
        {
            // STRING test = "these \"special chars\" shouldn't be a problem";
            string code = "STRING test = \"these \\\"special chars\\\" shouldn't be a problem\";";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual("these \\\"special chars\\\" shouldn't be a problem", variable.Value);
        }

        [Test]
        public void Assign_STRING_With_Multiline_Text_Works()
        {
            // STRING test = "these \"special chars\" shouldn't be a problem";
            string code = String.Format("STRING test = @\"First line{0}Second line\";", Environment.NewLine);

            _SyneryClient.Run(code);
            
            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(String.Format("First line{0}Second line", Environment.NewLine), variable.Value);
        }

        [Test]
        public void Assign_BOOL_Works()
        {
            string code = @"BOOL test = TRUE;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(true, variable.Value);
        }

        [Test]
        public void Assign_INT_Works()
        {
            string code = @"INT test = 15;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(15, variable.Value);
        }

        [Test]
        public void Assign_DECIMAL_Works()
        {
            string code = @"DECIMAL test = 17.312646623M;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(17.312646623M, variable.Value);
        }

        [Test]
        public void Assign_DOUBLE_Works()
        {
            string code = @"DOUBLE test = 17.312646623;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(17.312646623, variable.Value);
        }

        [Test]
        public void Assign_CHAR_Works()
        {
            string code = @"CHAR test = 'R';";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual('R', variable.Value);
        }

        [Test]
        public void Assign_DATETIME_With_Integers_Works()
        {
            string code = @"DATETIME test = DATETIME(2014,3,17,20,0,0);";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(new DateTime(2014, 3, 17, 20, 0, 0), variable.Value);
        }

        [Test]
        public void Assign_DATETIME_With_String_Works()
        {
            string code = "DATETIME test = DATETIME(\"17.03.2014 20:00:00\", \"de-ch\");";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(new DateTime(2014, 3, 17, 20, 0, 0), variable.Value);
        }

        #endregion

    }
}
