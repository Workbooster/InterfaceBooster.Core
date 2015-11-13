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
using InterfaceBooster.SyneryLanguage.Model.SyneryTypes.SyneryRecords;
using InterfaceBooster.Common.Tools.Data.ExceptionHandling;

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Functions
{
    public class LibraryPluginFunctionCallInterpreter : IInterpreter<SyneryParser.LibraryPluginFunctionCallContext, IValue>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }

        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public IValue RunWithResult(SyneryParser.LibraryPluginFunctionCallContext context)
        {
            string libraryPluginIdentifier;
            string functionIdentifier;
            string fullIdentifier = context.libraryPluginFunctionCallIdentifier().ExternalLibraryIdentifier().GetText();

            // split the identifier

            bool success = FunctionHelper.FragmentLibraryPluginIdentifier(fullIdentifier, out libraryPluginIdentifier, out functionIdentifier);

            if (success == false)
                throw new SyneryInterpretationException(context.libraryPluginFunctionCallIdentifier(), "Wasn't able to fragment the library plugin function identifier. It doesn't have the expected format.");

            // validate the identifier

            if (String.IsNullOrEmpty(libraryPluginIdentifier))
                throw new SyneryInterpretationException(context, "The library plugin identifier is empty.");
            if (String.IsNullOrEmpty(functionIdentifier))
                throw new SyneryInterpretationException(context, "The function identifier is empty.");

            // get the parameters

            IList<IValue> listOfParameters = FunctionHelper.GetListOfParameterValues(Controller, context.expressionList());
            Type[] listOfParameterTypes = listOfParameters.Select(p => p.Type.UnterlyingDotNetType).ToArray();
            object[] listOfParameterValues = listOfParameters.Select(p => p.Value).ToArray();

            // find the function declaration

            IStaticExtensionFunctionData functionDeclaration = Memory.LibraryPluginManager.GetStaticFunctionDataBySignature(libraryPluginIdentifier, functionIdentifier, listOfParameterTypes);

            object result = null;

            try
            {
                // execute the function

                result = Memory.LibraryPluginManager.CallStaticFunctionWithPrimitiveReturn(functionDeclaration, listOfParameterValues);
            }
            catch (Exception ex)
            {
                // create a Synery exception that can be caught by the Synery developer

                LibraryPluginExceptionRecord syneryException = new LibraryPluginExceptionRecord();
                syneryException.Message = ExceptionHelper.GetNestedExceptionMessages(ex);
                syneryException.FullIdentifier = fullIdentifier;
                syneryException.LibraryPluginIdentifier = libraryPluginIdentifier;

                Controller.HandleSyneryEvent(context, syneryException.GetAsSyneryValue());
            }

            return new TypedValue(TypeHelper.GetSyneryType(functionDeclaration.ReturnType), result);
        }

        #endregion
    }
}
