using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.LibraryPlugin.Information.ReflectionData;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.SyneryLanguage.Model.SyneryTypes.SyneryRecords;
using InterfaceBooster.Common.Tools.Data.ExceptionHandling;

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Statements
{
    public class LibraryPluginVariableStatementInterpreter : IInterpreter<SyneryParser.LibraryPluginVariableStatementContext>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public void Run(SyneryParser.LibraryPluginVariableStatementContext context)
        {
            if (context.libraryPluginVariableAssignment() != null)
            {

                string libraryPluginIdentifier;
                string variableIdentifier;
                string fullIdentifier = context.libraryPluginVariableAssignment().ExternalLibraryIdentifier().GetText();

                // split the identifier

                bool success = FunctionHelper.FragmentLibraryPluginIdentifier(fullIdentifier, out libraryPluginIdentifier, out variableIdentifier);

                if (success == false)
                    throw new SyneryInterpretationException(context, "Wasn't able to fragment the library plugin variable identifier. It doesn't have the expected format.");

                // validate the identifier

                if (String.IsNullOrEmpty(libraryPluginIdentifier))
                    throw new SyneryInterpretationException(context, "The library plugin identifier is empty.");
                if (String.IsNullOrEmpty(variableIdentifier))
                    throw new SyneryInterpretationException(context, "The variable identifier is empty.");

                // find the variable declaration
                IStaticExtensionVariableData variableData = Memory.LibraryPluginManager.GetStaticVariableDataByIdentifier(libraryPluginIdentifier, variableIdentifier);

                // interpret the set-value

                IValue value = Controller.Interpret<SyneryParser.ExpressionContext, IValue>(context.libraryPluginVariableAssignment().variableInitializer().expression());

                try
                {
                    //  set the value

                    Memory.LibraryPluginManager.SetStaticVariableWithPrimitiveReturn(variableData, value);
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
            }
        }

        #endregion
    }
}
