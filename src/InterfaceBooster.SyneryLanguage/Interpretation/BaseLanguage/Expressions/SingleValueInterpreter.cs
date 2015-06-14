using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.LibraryPlugin.Information.ReflectionData;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.Utilities.SyneryLanguage;

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Expressions
{
    public class SingleValueInterpreter : IInterpreter<SyneryParser.SingleValueContext, IValue>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public IValue RunWithResult(SyneryParser.SingleValueContext context)
        {
            if (context.literal() != null)
            {
                return Controller.Interpret<SyneryParser.LiteralContext, IValue>(context.literal());
            }
            else if (context.variableReference() != null)
            {
                string varName = context.variableReference().GetText();
                return Memory.CurrentScope.ResolveVariable(varName);
            }
            else if (context.complexReference() != null)
            {
                return Controller.Interpret<SyneryParser.ComplexReferenceContext, IValue>(context.complexReference());
            }
            else if (context.libraryPluginVariableReference() != null)
            {
                return GetLibraryPluginVariableValue(context.libraryPluginVariableReference());
            }
            else if (context.functionCall() != null)
            {
                return Controller.Interpret<SyneryParser.FunctionCallContext, IValue>(context.functionCall());
            }

            throw new SyneryInterpretationException(context, 
                string.Format("Unknown single value expression in {0}: '{1}'", this.GetType().Name, context.GetText()));
        }

        #endregion

        #region INTERNAL METHODS

        private IValue GetLibraryPluginVariableValue(SyneryParser.LibraryPluginVariableReferenceContext context)
        {
            string libraryPluginIdentifier;
            string variableIdentifier;
            string fullIdentifier = context.ExternalLibraryIdentifier().GetText();

            // split the identifier

            bool success = FunctionHelper.FragmentLibraryPluginIdentifier(fullIdentifier, out libraryPluginIdentifier, out variableIdentifier);

            if (success == false)
                throw new SyneryInterpretationException(context, "Wasn't able to fragment the library plugin variable identifier. It doesn't have the expected format.");

            // validate the identifer

            if (String.IsNullOrEmpty(libraryPluginIdentifier))
                throw new SyneryInterpretationException(context, "The library plugin identifer is empty.");
            if (String.IsNullOrEmpty(variableIdentifier))
                throw new SyneryInterpretationException(context, "The variable identifer is empty.");

            // find the variable declaration
            IStaticExtensionVariableData variableData = Memory.LibraryPluginManager.GetStaticVariableDataByIdentifier(libraryPluginIdentifier, variableIdentifier);

            // get the value

            object result = Memory.LibraryPluginManager.GetStaticVariableWithPrimitiveReturn(variableData);

            return new TypedValue(TypeHelper.GetSyneryType(variableData.Type), result);
        }

        #endregion
    }
}
