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


namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.BaseLanguage.Expressions.ExpressionInterpreter_Test
{
    public class Executing_Logical_Expression_Works : BaseTest
    {
        #region AND

        [Test]
        public void Executing_Simple_And_Expression_With_Static_True_Values_Works()
        {
            string code = "BOOL test = TRUE AND TRUE;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(true, variable.Value);
        }

        [Test]
        public void Executing_Simple_And_Expression_With_Static_True_And_False_Values_Works()
        {
            string code = "BOOL test = FALSE AND TRUE;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(false, variable.Value);
        }

        [Test]
        public void Executing_Complex_And_Expression_With_Calculated_True_Values_Works()
        {
            string code = "BOOL test = 15 == 15 AND \"Bla\" == \"Bla\";";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(true, variable.Value);
        }

        [Test]
        public void Executing_Complex_And_Expression_With_Calculated_True_And_False_Values_Works()
        {
            string code = "BOOL test = 15 == 15 AND \"First\" == \"Second\";";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(false, variable.Value);
        }

        [Test]
        public void Executing_Complex_And_Expression_With_Multiple_Calculated_True_Values_Works()
        {
            string code = "BOOL test = 22.99999999M == 22.99999999M AND 15.987654 != 22.1 AND 15 == 15 AND \"Bla\" == \"Bla\" AND \"First\" != \"Second\";";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(true, variable.Value);
        }

        [Test]
        public void Executing_Complex_And_Expression_With_Multiple_Calculated_True_And_False_Values_Works()
        {
            string code = "BOOL test = 22.99999999M == 22.99999999M AND 15.987654 == 22.1 AND 15 == 15 AND \"Bla\" == \"Bla\" AND \"First\" != \"Second\";";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(false, variable.Value);
        }

        #endregion

        #region OR

        [Test]
        public void Executing_Simple_Or_Expression_With_Static_True_Values_Works()
        {
            string code = "BOOL test = TRUE OR TRUE;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(true, variable.Value);
        }

        [Test]
        public void Executing_Simple_Or_Expression_With_Static_True_Or_False_Values_Works()
        {
            string code = "BOOL test = FALSE OR TRUE;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(true, variable.Value);
        }

        [Test]
        public void Executing_Complex_Or_Expression_With_Calculated_True_Values_Works()
        {
            string code = "BOOL test = 15 == 15 OR \"Bla\" == \"Bla\";";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(true, variable.Value);
        }

        [Test]
        public void Executing_Complex_Or_Expression_With_Calculated_True_Or_False_Values_Works()
        {
            string code = "BOOL test = 15 == 15 OR \"First\" == \"Second\";";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(true, variable.Value);
        }

        [Test]
        public void Executing_Complex_Or_Expression_With_Multiple_Calculated_True_Values_Works()
        {
            string code = "BOOL test = 22.99999999M == 22.99999999M OR 15.987654 != 22.1 OR 15 == 15 OR \"Bla\" == \"Bla\" OR \"First\" != \"Second\";";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(true, variable.Value);
        }

        [Test]
        public void Executing_Complex_Or_Expression_With_Multiple_Calculated_True_Or_False_Values_Works()
        {
            string code = "BOOL test = 22.99999999M == 22.99999999M OR 15.987654 == 22.1 OR 15 == 15 OR \"Bla\" == \"Bla\" OR \"First\" != \"Second\";";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(true, variable.Value);
        }

        [Test]
        public void Executing_Complex_Or_Expression_With_Multiple_Calculated_False_Values_Works()
        {
            string code = "BOOL test = 22.7M == 22.99999999M OR 15.987654 == 22.1 OR 22 == 15 OR \"Bla\" != \"Bla\" OR \"First\" == \"Second\";";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(false, variable.Value);
        }

        #endregion
    }
}
