using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.SyneryLanguage.Model.QueryLanguage;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.QueryLanguage;

namespace InterfaceBooster.SyneryLanguage.Interpretation.General
{
    public static class FunctionHelper
    {
        #region PUBLIC METHODS

        #region GENERAL

        public static IList<IValue> GetListOfParameterValues(IInterpretationController controller, SyneryParser.ExpressionListContext context)
        {
            if (context != null)
            {
                return controller.Interpret<SyneryParser.ExpressionListContext, IList<IValue>>(context);
            }
            else
            {
                return new List<IValue>();
            }
        }

        public static IEnumerable<IExpressionValue> GetListOfParameterExpressionValues(IInterpretationController controller, QueryMemory queryMemory, SyneryParser.RequestExpressionListContext context)
        {
            if (context != null)
            {
                return controller.Interpret<SyneryParser.RequestExpressionListContext, IList<IExpressionValue>, QueryMemory>(context, queryMemory);
            }
            else
            {
                return new List<IExpressionValue>();
            }
        }

        #endregion

        #region SYNERY FUNCTIONS

        /// <summary>
        /// Tries to find the function declaration with the matching function signature. It also considers that a parameter 
        /// isn't set because a default value is available
        /// 
        /// EXAMPLE:
        /// 
        /// myFunc(string first, string second = "default")
        ///     // do something...
        /// END
        /// 
        /// // call with one parameter
        /// myFunc("first parameter");
        /// </summary>
        /// <param name="memory">the SyneryMemory that contains a list with all available functions.</param>
        /// <param name="identifier">the name of the function</param>
        /// <param name="listOfParameterTypes">the types of all available parameters</param>
        /// <returns></returns>
        public static IFunctionData FindSyneryFunctionDeclaration(ISyneryMemory memory, string identifier, SyneryType[] listOfParameterTypes)
        {
            // try to find function(s) with the same name with at least the same number of parameters as given
            IEnumerable<IFunctionData> listOfFunctionData =
                from f in memory.Functions
                where f.FullName == IdentifierHelper.GetIdentifierBasedOnFunctionScope(memory, identifier)
                && f.FunctionDefinition.Parameters.Count >= listOfParameterTypes.Count()
                select f;

            // try to find the matching function signature
            // also consider that a parameter isn't set because a default value is available
            foreach (IFunctionData data in listOfFunctionData)
            {
                bool isMatching = true;

                for (int i = 0; i < data.FunctionDefinition.Parameters.Count; i++)
                {
                    if (listOfParameterTypes.Count() > i && listOfParameterTypes[i] != null)
                    {
                        SyneryType expectedType = data.FunctionDefinition.Parameters[i].Type;
                        SyneryType givenType = listOfParameterTypes[i];

                        // compare the types of the expected parameter and the given value
                        if (givenType != expectedType)
                        {
                            // maybe these are two record-types which derive from each other
                            if (givenType.UnterlyingDotNetType != typeof(IRecord)
                                || expectedType.UnterlyingDotNetType != typeof(IRecord))
                            {
                                isMatching = false;
                            }
                            else
                            {
                                string expectedTypeName = IdentifierHelper.GetFullName(expectedType.Name, data.CodeFileAlias);

                                if (!RecordHelper.IsDerivedType(memory, givenType.Name, expectedTypeName))
                                {
                                    isMatching = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        // no value given for the parameter: check whether there is a default value
                        if (data.FunctionDefinition.Parameters[i].DefaultValue == null)
                        {
                            isMatching = false;
                        }
                    }
                }

                // the current function declaration seems to match -> return it
                if (isMatching == true)
                    return data;
            }

            // no matching function declaration found
            return null;
        }

        /// <summary>
        /// Executes the function from the given <paramref name="functionData"/> using the given <paramref name="controller"/> and by setting the <paramref name="listOfParameters"/> as local scope variables.
        /// </summary>
        /// <param name="controller">the controller that is used to interpret the function</param>
        /// <param name="functionData">contains the data for the function that should be executed</param>
        /// <param name="listOfParameters"></param>
        /// <param name="functionCallContext">the synery context where the function is called (only used in exceptions)</param>
        /// <returns>the return value may be null if the function definition says HasReturnValue=false</returns>
        public static IValue ExecuteSyneryFunction(IInterpretationController controller, IFunctionData functionData, object[] listOfParameters, Antlr4.Runtime.ParserRuleContext functionCallContext)
        {
            IValue returnValue;
            IDictionary<string, IValue> scopeVariables = new Dictionary<string, IValue>();
            int sizeOfSignatureParameters = functionData.FunctionDefinition.Parameters.Count;
            int sizeOfListOfParameters = listOfParameters.Count();

            // prepare the parameters as local scope variables

            for (int i = 0; i < sizeOfSignatureParameters; i++)
            {
                object variableValue = null;
                string variableName = functionData.FunctionDefinition.Parameters[i].Name;
                SyneryType variableType = functionData.FunctionDefinition.Parameters[i].Type;

                if (sizeOfListOfParameters >= (i + 1)
                    && listOfParameters[i] != null)
                {
                    variableValue = listOfParameters[i];
                }
                else if (functionData.FunctionDefinition.Parameters[i].DefaultValue != null)
                {
                    variableValue = functionData.FunctionDefinition.Parameters[i].DefaultValue.Value;
                }

                scopeVariables.Add(variableName, new TypedValue(variableType, variableValue));
            }

            // start executing the function context

            if (functionData.Context is SyneryParser.BlockContext)
            {
                /* 1. Create scope for the function block with the given parameters set as variables.
                 *    Set the global scope as parent to avoid accessing variables of this scope.
                 * 2. Execute the function block.
                 * 3. Get the return value from the function block.
                 */

                IFunctionScope functionScope = new FunctionScope(controller.Memory.GlobalScope, functionData, scopeVariables);

                controller.Interpret<SyneryParser.BlockContext, INestedScope, INestedScope>((SyneryParser.BlockContext)functionData.Context, functionScope);

                returnValue = functionScope.ReturnValue;
            }
            else
            {
                throw new SyneryInterpretationException(functionData.Context, "Unknown function content cannot be interpreted.");
            }

            // validate the return value

            if (functionData.FunctionDefinition.HasReturnValue == true && returnValue == null)
            {
                string message = String.Format(
                    "Function has no return value but is expected to return '{0}'. Function called on line: '{1}'.",
                    functionData.FunctionDefinition.ReturnType.PublicName,
                    functionCallContext.Start.Line);

                throw new SyneryInterpretationException(functionData.Context, message);
            }
            else if (functionData.FunctionDefinition.HasReturnValue == true
                && functionData.FunctionDefinition.ReturnType != returnValue.Type)
            {
                string message = String.Format(
                    "Function has wrong return value. Expected: '{0}' / Given: '{1}'. Function called on line: '{2}'.",
                    functionData.FunctionDefinition.ReturnType.PublicName,
                    returnValue.Type.PublicName,
                    functionCallContext.Start.Line);
                throw new SyneryInterpretationException(functionData.Context, message);
            }

            return returnValue;
        }

        #endregion

        #region LIBRARY PLUGIN FUNCTIONS

        public static bool FragmentLibraryPluginIdentifier(string fullIdentifier, out string libraryPluginIdentifier, out string identifier)
        {
            /*
             * The LibraryPlugin identifiers are formated like this:
             * $LibraryName.String.ToUpper
             * 
             * -> So we have to fragment this string into LibraryPLugin identifier and function identifier.
             */

            string trimmedIdentifier = fullIdentifier.TrimStart('$');

            if (trimmedIdentifier.Contains('.'))
            {
                // Find the position of the first dot "."

                int positionOfFirstDot = trimmedIdentifier.IndexOf('.');

                libraryPluginIdentifier = trimmedIdentifier.Substring(0, positionOfFirstDot);
                identifier = trimmedIdentifier.Substring(positionOfFirstDot + 1);

                // success!
                return true;
            }

            // couldn't fragment the identifier
            libraryPluginIdentifier = null;
            identifier = null;

            return false;
        }

        #endregion

        #endregion
    }
}
