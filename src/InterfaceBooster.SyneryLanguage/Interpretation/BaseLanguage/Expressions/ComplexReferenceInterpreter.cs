using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Expressions
{
    public class ComplexReferenceInterpreter : IInterpreter<SyneryParser.ComplexReferenceContext, IValue>, IInterpreter<SyneryParser.ComplexReferenceContext, IValue, string[]>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public IValue RunWithResult(SyneryParser.ComplexReferenceContext context)
        {
            string complexReference = context.GetText();
            string[] parts = IdentifierHelper.ParseComplexIdentifier(complexReference);

            return RunWithResult(context, parts);
        }

        public IValue RunWithResult(SyneryParser.ComplexReferenceContext context, string[] parts)
        {
            string complexReference = context.GetText();

            string varName = parts[0];

            if (Memory.CurrentScope.DoesVariableExists(varName))
            {
                IValue currentValue = Memory.CurrentScope.ResolveVariable(varName);

                for (int i = 1; i < parts.Count(); i++)
                {
                    if (currentValue.Type.UnterlyingDotNetType == typeof(IRecord))
                    {
                        string fieldName = parts[i];
                        IRecord record = ((IRecord)currentValue.Value);

                        if (record.DoesFieldExists(fieldName))
                        {
                            currentValue = record.GetFieldValue(fieldName);
                        }
                        else
                        {
                            throw new SyneryInterpretationException(context, String.Format(
                                "The complex identifier '{0}' couldn't be resolved. The field '{1}' doesn't exists",
                                complexReference, fieldName));
                        }
                    }
                    else
                    {
                        throw new SyneryInterpretationException(context, String.Format(
                            "The complex identifier '{0}', couldn't be resolved. The item at level {1} is not a record.",
                            complexReference, i + 1));
                    }
                }

                return currentValue;
            }
            else
            {
                throw new SyneryInterpretationException(context, String.Format(
                    "The variable with name '{0}' wasn't found.",
                    String.Join(".", parts)));
            }
        }

        #endregion
    }
}
