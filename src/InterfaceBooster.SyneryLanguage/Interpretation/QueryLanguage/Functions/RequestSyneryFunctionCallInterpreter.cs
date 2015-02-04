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
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.QueryLanguage;

namespace InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Functions
{
    public class RequestSyneryFunctionCallInterpreter : IInterpreter<SyneryParser.RequestSyneryFunctionCallContext, IExpressionValue, QueryMemory>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public IExpressionValue Run(SyneryParser.RequestSyneryFunctionCallContext context, QueryMemory queryMemory)
        {
            string identifier = null;
            IEnumerable<IExpressionValue> listOfParameters;

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

            listOfParameters = FunctionHelper.GetListOfParameterExpressionValues(Controller, queryMemory, context.requestExpressionList());

            // convert the list of IExpressionValue objects into a list of Expressions and a type-array

            IEnumerable<Expression> listOfParameterExpressions = listOfParameters.Select(p => p.Expression).ToArray();
            SyneryType[] parameterTypesAsArray = listOfParameters.Select(p => p.ResultType).ToArray();

            // now its time to search for a matching function signature

            IFunctionData functionData = FunctionHelper.FindSyneryFunctionDeclaration(Memory, identifier, parameterTypesAsArray);

            // check whether a matching function signature was found

            if (functionData == null)
            {
                // prepare helpfull exception message
                string paramTypes = string.Join(",", listOfParameters.Select(v => v.ResultType.PublicName));
                string message = String.Format("No matching function signature found: {0}({1})", identifier, paramTypes);

                throw new SyneryInterpretationException(context, message);
            }

            // check whether the function has a return
            // functions without a return value aren't supported inside of a request statement

            if (functionData.FunctionDefinition.HasReturnValue == false)
            {
                throw new SyneryInterpretationException(context, String.Format("Cannot call a function without a return from a query. Please make sure the function '{0}' has a return value.", identifier));
            }

            // build the LINQ expression for calling the function from inside the LINQ expression tree

            Expression functionCallExpression = BuildExpression(functionData, listOfParameterExpressions, context);

            return new ExpressionValue(
                expression: functionCallExpression,
                resultType: functionData.FunctionDefinition.ReturnType
            );
        }

        #endregion

        #region INTERNAL METHODS

        /// <summary>
        /// Builds an expression that looks like this:
        /// FunctionHelper.ExecuteSyneryFunction(functionData, listOfParameters, context)
        /// </summary>
        /// <param name="functionData"></param>
        /// <param name="listOfParameterExpressions"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private Expression BuildExpression(IFunctionData functionData, IEnumerable<Expression> listOfParameterExpressions, SyneryParser.RequestSyneryFunctionCallContext context)
        {
            MethodInfo executeFunctionMethod = typeof(FunctionHelper)
                .GetMethod("ExecuteSyneryFunction", new Type[] { typeof(IInterpretationController), typeof(IFunctionData), typeof(object[]), typeof(Antlr4.Runtime.ParserRuleContext) });

            
            Expression paramControllerExpression = Expression.Constant(this.Controller, typeof(IInterpretationController));
            Expression paramFunctionDataExpression = Expression.Constant(functionData, typeof(IFunctionData));
            Expression paramListOfParametersExpression = Expression.NewArrayInit(typeof(object), listOfParameterExpressions);
            Expression paramContextExpression = Expression.Constant(context, typeof(Antlr4.Runtime.ParserRuleContext));

            Expression methodCallExpression = Expression.Call(executeFunctionMethod, 
                paramControllerExpression,
                paramFunctionDataExpression,
                paramListOfParametersExpression,
                paramContextExpression);

            Expression accessValueProperty = Expression.Property(methodCallExpression, "Value");

            return accessValueProperty;
        }

        #endregion
    }
}
