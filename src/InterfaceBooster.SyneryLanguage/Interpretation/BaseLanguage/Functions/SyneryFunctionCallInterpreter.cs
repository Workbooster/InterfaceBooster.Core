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

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Functions
{
    public class SyneryFunctionCallInterpreter : IInterpreter<SyneryParser.SyneryFunctionCallContext, IValue>
    {
        #region PROPERTIES
        
        public ISyneryMemory Memory { get; set; }

        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public IValue Run(SyneryParser.SyneryFunctionCallContext context)
        {
            string identifier = null;
            IList<IValue> listOfParameters;

            if (context.syneryFunctionInternalIdentifier() != null)
            {
                // the function call references a local function from the current code file
                // example: functionName(param1, param2);
                identifier = context.syneryFunctionInternalIdentifier().GetText();
            }
            else if (context.syneryFunctionExternalIdentifier() != null)
            {
                // the function call references an external function from a different code file
                // example: secondFileAlias.functionName(param1, param2);
                identifier = context.syneryFunctionExternalIdentifier().GetText();
            }
            else
            {
                throw new SyneryInterpretationException(context, "Wrong or unknown identifier.");
            }

            // get the parameter values from the function call

            listOfParameters = FunctionHelper.GetListOfParameterValues(Controller, context.expressionList());

            // convert the list of IValue objects into an object-array and a type-array

            object[] parametersAsObjectArray = listOfParameters.Select(p => p.Value).ToArray();
            SyneryType[] parameterTypesAsArray = listOfParameters.Select(p => p.Type).ToArray();

            // now its time to search for a matching function signature

            IFunctionData functionData = FunctionHelper.FindSyneryFunctionDeclaration(Memory, identifier, parameterTypesAsArray);

            // check whether a matching function signature was found

            if (functionData == null)
            {
                // prepare helpfull exception message
                string paramTypes = string.Join(",", listOfParameters.Select(v => v.Type.PublicName));
                string message = String.Format("No matching function signature found: {0}({1})", identifier, paramTypes);

                throw new SyneryInterpretationException(context, message);
            }

            // execute the function and return the result value

            return FunctionHelper.ExecuteSyneryFunction(Controller, functionData, parametersAsObjectArray, context);
        }

        #endregion
    }
}
