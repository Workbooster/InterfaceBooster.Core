using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Expressions
{
    public class DateTimeLiteralInterpreter : IInterpreter<SyneryParser.DateTimeLiteralContext, DateTime>
    {

        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public DateTime RunWithResult(SyneryParser.DateTimeLiteralContext context)
        {
            Type parameterType = null;
            List<IValue> listOfDateParts = new List<IValue>();

            // extract all string values from the expressions

            foreach (var expression in context.expression())
            {
                IValue value = Controller.Interpret<SyneryParser.ExpressionContext, IValue>(expression);

                if (parameterType == null || parameterType == value.Type.UnterlyingDotNetType)
                {
                    listOfDateParts.Add(value);
                    parameterType = value.Type.UnterlyingDotNetType;
                }
                else
                {
                    string message = String.Format(
                        "Expression in DateTime litral at position {0} does not return an {1} value.",
                        listOfDateParts.Count + 1,
                        parameterType.Name);

                    throw new SyneryInterpretationException(expression, message);
                }
            }

            try
            {
                if (parameterType == typeof(string))
                {
                    if (listOfDateParts.Count == 1)
                    {
                        return DateTime.Parse((string)listOfDateParts[0].Value);
                    }
                    else if (listOfDateParts.Count == 2)
                    {
                        return DateTime.Parse((string)listOfDateParts[0].Value, new CultureInfo((string)listOfDateParts[1].Value));
                    }
                }
                else if (parameterType == typeof(int))
                {
                    switch (listOfDateParts.Count)
                    {
                        case 1:
                            return new DateTime((int)listOfDateParts[0].Value, 1, 1);
                        case 2:
                            return new DateTime((int)listOfDateParts[0].Value, (int)listOfDateParts[1].Value, 1);
                        case 3:
                            return new DateTime((int)listOfDateParts[0].Value, (int)listOfDateParts[1].Value, (int)listOfDateParts[2].Value);
                        case 4:
                            return new DateTime((int)listOfDateParts[0].Value, (int)listOfDateParts[1].Value, (int)listOfDateParts[2].Value, (int)listOfDateParts[3].Value, 0, 0);
                        case 5:
                            return new DateTime((int)listOfDateParts[0].Value, (int)listOfDateParts[1].Value, (int)listOfDateParts[2].Value, (int)listOfDateParts[3].Value, (int)listOfDateParts[4].Value, 0);
                        case 6:
                            return new DateTime((int)listOfDateParts[0].Value, (int)listOfDateParts[1].Value, (int)listOfDateParts[2].Value, (int)listOfDateParts[3].Value, (int)listOfDateParts[4].Value, (int)listOfDateParts[5].Value);
                        case 7:
                            return new DateTime((int)listOfDateParts[0].Value, (int)listOfDateParts[1].Value, (int)listOfDateParts[2].Value, (int)listOfDateParts[3].Value, (int)listOfDateParts[4].Value, (int)listOfDateParts[5].Value, (int)listOfDateParts[6].Value);
                    }
                }

            }
            catch (Exception ex)
            {
                string message = String.Format("An error occured while initializing the DateTime literal from {0} value(s). Message: '{1}'", parameterType.Name, ex.Message);
                throw new SyneryInterpretationException(context, message);
            }

            throw new SyneryInterpretationException(context, "Unknown DateTime string definition");
        }

        #endregion
    }
}
