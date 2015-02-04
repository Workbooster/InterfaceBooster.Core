using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.BaseLanguage.Expressions.ExpressionInterpreter_Test
{
    [TestFixture]
    public class Casting_Null_Values_Works : BaseTest
    {
        [Test]
        public void Executing_Cast_Expression_On_STRING_And_NULL_Works()
        {
            string code = "STRING test = (STRING)NULL;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(typeof(string), variable.Type.UnterlyingDotNetType);
            Assert.AreEqual(null, variable.Value);
        }

        [Test]
        public void Executing_Cast_Expression_On_BOOL_And_NULL_Works()
        {
            string code = "BOOL test = (BOOL)NULL;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(typeof(bool), variable.Type.UnterlyingDotNetType);
            Assert.AreEqual(null, variable.Value);
        }

        [Test]
        public void Executing_Cast_Expression_On_INT_And_NULL_Works()
        {
            string code = "INT test = (INT)NULL;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(typeof(int), variable.Type.UnterlyingDotNetType);
            Assert.AreEqual(null, variable.Value);
        }

        [Test]
        public void Executing_Cast_Expression_On_DECIMAL_And_NULL_Works()
        {
            string code = "DECIMAL test = (DECIMAL)NULL;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(typeof(decimal), variable.Type.UnterlyingDotNetType);
            Assert.AreEqual(null, variable.Value);
        }

        [Test]
        public void Executing_Cast_Expression_On_DOUBLE_And_NULL_Works()
        {
            string code = "DOUBLE test = (DOUBLE)NULL;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(typeof(double), variable.Type.UnterlyingDotNetType);
            Assert.AreEqual(null, variable.Value);
        }

        [Test]
        public void Executing_Cast_Expression_On_CHAR_And_NULL_Works()
        {
            string code = "CHAR test = (CHAR)NULL;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(typeof(char), variable.Type.UnterlyingDotNetType);
            Assert.AreEqual(null, variable.Value);
        }

        [Test]
        public void Executing_Cast_Expression_On_DATETIME_And_NULL_Works()
        {
            string code = "DATETIME test = (DATETIME)NULL;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(typeof(DateTime), variable.Type.UnterlyingDotNetType);
            Assert.AreEqual(null, variable.Value);
        }
    }
}
