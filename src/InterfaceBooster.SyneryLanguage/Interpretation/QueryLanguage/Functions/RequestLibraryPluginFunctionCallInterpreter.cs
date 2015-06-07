using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.SyneryLanguage.Model.QueryLanguage;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.LibraryPlugin;
using InterfaceBooster.Common.Interfaces.LibraryPlugin.Information.ReflectionData;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.QueryLanguage;
using InterfaceBooster.Common.Interfaces.Utilities.SyneryLanguage;

namespace InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Functions
{
    public class RequestLibraryPluginFunctionCallInterpreter : IInterpreter<SyneryParser.RequestLibraryPluginFunctionCallContext, IExpressionValue, QueryMemory>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public IExpressionValue Run(SyneryParser.RequestLibraryPluginFunctionCallContext context, QueryMemory queryMemory)
        {
            string libraryPluginIdentifier;
            string functionIdentifier;
            string fullIdentifier = context.libraryPluginFunctionCallIdentifier().ExternalLibraryIdentifier().GetText();

            // split the identifier

            bool success = FunctionHelper.FragmentLibraryPluginIdentifier(fullIdentifier, out libraryPluginIdentifier, out functionIdentifier);

            if (success == false)
                throw new SyneryInterpretationException(context.libraryPluginFunctionCallIdentifier(), "Wasn't able to fragment the library plugin function identifier. It doesn't have the expected format.");

            // validate the identifer

            if (String.IsNullOrEmpty(libraryPluginIdentifier))
                throw new SyneryInterpretationException(context, "The library plugin identifer is empty.");
            if (String.IsNullOrEmpty(functionIdentifier))
                throw new SyneryInterpretationException(context, "The function identifer is empty.");

            // get the parameters

            IEnumerable<IExpressionValue> listOfParameters = FunctionHelper.GetListOfParameterExpressionValues(Controller, queryMemory, context.requestExpressionList());
            Type[] listOfParameterTypes = listOfParameters.Select(p => p.ResultType.UnterlyingDotNetType).ToArray();
            IEnumerable<Expression> listOfParameterExpressions = listOfParameters.Select(p => p.Expression);

            // find the function declaration

            IStaticExtensionFunctionData functionDeclaration = Memory.LibraryPluginManager.GetStaticFunctionDataBySignature(libraryPluginIdentifier, functionIdentifier, listOfParameterTypes);

            // check whether the function has a return
            // functions without a return value aren't supported inside of a request statement

            if (functionDeclaration.ReturnType == null)
            {
                throw new SyneryInterpretationException(context, String.Format("Cannot call a function without a return from a query. Please chose a function instead of '{0}.{1}' that has a return value.", libraryPluginIdentifier, functionIdentifier));
            }

            // build the LINQ expression for calling the function from inside the LINQ expression tree

            Expression functionCallExpression = BuildExpression(functionDeclaration, listOfParameterExpressions);

            return new ExpressionValue(
                expression: functionCallExpression,
                resultType: TypeHelper.GetSyneryType(functionDeclaration.ReturnType)
            );
        }

        #endregion

        #region INTERNAL METHODS

        /// <summary>
        /// Builds an expression that looks like this:
        /// Memory.LibraryPluginManager.CallStaticFunctionWithPrimitiveReturn(functionDeclaration, listOfParameterValues)
        /// </summary>
        /// <param name="functionData"></param>
        /// <param name="listOfParameterExpressions"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private Expression BuildExpression(IStaticExtensionFunctionData functionData, IEnumerable<Expression> listOfParameterExpressions)
        {
            MethodInfo executeFunctionMethod = typeof(ILibraryPluginManager)
                .GetMethod("CallStaticFunctionWithPrimitiveReturn", new Type[] { typeof(IStaticExtensionFunctionData), typeof(object[]) });

            Expression libraryPluginManagerExpression = Expression.Constant(Memory.LibraryPluginManager);

            Expression paramFunctionDataExpression = Expression.Constant(functionData, typeof(IStaticExtensionFunctionData));
            Expression paramListOfParametersExpression = Expression.NewArrayInit(typeof(object), listOfParameterExpressions);

            Expression methodCallExpression = Expression.Call(
                libraryPluginManagerExpression,
                executeFunctionMethod,
                paramFunctionDataExpression,
                paramListOfParametersExpression);

            return methodCallExpression;
        }

        #endregion
    }
}
