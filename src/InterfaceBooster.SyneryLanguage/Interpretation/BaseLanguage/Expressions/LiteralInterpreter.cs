using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.Utilities.SyneryLanguage;

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Expressions
{
    /// <summary>
    /// Interprets literal values that are specified directly inside of the Synery code.
    /// Examples:
    ///     INT someNumber = 15;
    ///     STRING someText = "blaa";
    /// </summary>
    public class LiteralInterpreter : IInterpreter<SyneryParser.LiteralContext, IValue>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public IValue Run(SyneryParser.LiteralContext context)
        {
            SyneryType type = null;
            object literalValue = null;

            if (context.StringLiteral() != null)
            {
                type = TypeHelper.STRING_TYPE;
                literalValue = LiteralHelper.ParseStringLiteral(context.StringLiteral());
            }
            else if (context.VerbatimStringLiteral() != null)
            {
                type = TypeHelper.STRING_TYPE;
                literalValue = LiteralHelper.ParseVerbatimStringLiteral(context.VerbatimStringLiteral());
            }
            else if (context.IntegerLiteral() != null)
            {
                type = TypeHelper.INT_TYPE;
                literalValue = Convert.ChangeType(context.GetText(), typeof(int));
            }
            else if (context.DoubleLiteral() != null)
            {
                type = TypeHelper.DOUBLE_TYPE;
                literalValue = Convert.ToDouble(context.GetText());
            }
            else if (context.DecimalLiteral() != null)
            {
                type = TypeHelper.DECIMAL_TYPE;
                string decimalValue = context.GetText().TrimEnd(new char[] { 'M', 'm' });

                literalValue = Convert.ToDecimal(decimalValue);
            }
            else if (context.CharLiteral() != null)
            {
                type = TypeHelper.CHAR_TYPE;
                char[] chars = context.GetText().ToCharArray(1, 1);

                if (chars.Length == 1)
                {
                    literalValue = Convert.ToChar(chars[0]);
                }
                else
                {
                    throw new SyneryInterpretationException(context, String.Format("The value '{0}' is not a valid char literal.", context.GetText()));
                }
            }
            else if (context.BooleanLiteral() != null)
            {
                type = TypeHelper.BOOL_TYPE;
                if (context.BooleanLiteral().GetText() == "TRUE")
                {
                    literalValue = true;
                }
                else if (context.BooleanLiteral().GetText() == "FALSE")
                {
                    literalValue = false;
                }
                else
                {
                    throw new SyneryInterpretationException(context, String.Format("The value '{0}' is not a valid boolean literal.", context.GetText()));
                }
            }
            else if (context.dateTimeLiteral() != null)
            {
                type = TypeHelper.DATETIME_TYPE;
                literalValue = Controller.Interpret<SyneryParser.DateTimeLiteralContext, DateTime>(context.dateTimeLiteral());
            }
            else if (context.NullLiteral() != null)
            {
                type = null;
                literalValue = null;
            }
            else
            {
                throw new SyneryInterpretationException(context, 
                    String.Format("Unknown Literal type in {0} for the value '{0}'.", this.GetType().Name, context.GetText()));
            }

            return new TypedValue(type, literalValue);
        }

        #endregion
    }
}
