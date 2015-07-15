using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Expressions;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.SyneryLanguage.Model.QueryLanguage;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.LibraryPlugin;
using InterfaceBooster.Common.Interfaces.LibraryPlugin.Information.ReflectionData;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.QueryLanguage;
using InterfaceBooster.Common.Interfaces.Utilities.SyneryLanguage;

namespace InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Expressions
{
    public class RequestSingleValueInterpreter : IInterpreter<SyneryParser.RequestSingleValueContext, IExpressionValue, QueryMemory>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public IExpressionValue RunWithResult(SyneryParser.RequestSingleValueContext context, QueryMemory queryMemory)
        {
            if (context.literal() != null)
            {
                Expression expression;
                IValue value = Controller.Interpret<SyneryParser.LiteralContext, IValue>(context.literal());

                if (value.Type != null)
                {
                    expression = Expression.Constant(value.Value, value.Type.UnterlyingDotNetType);
                }
                else
                {
                    expression = Expression.Constant(null);
                }

                return new ExpressionValue(
                    expression: expression,
                    resultType: value.Type
                );
            }
            else if (context.libraryPluginVariableReference() != null)
            {
                return GetLibraryPluginVariableValueExpression(context.libraryPluginVariableReference());
            }
            else if (context.requestFieldReference() != null)
            {
                return Controller.Interpret<SyneryParser.RequestFieldReferenceContext, KeyValuePair<string, IExpressionValue>, QueryMemory>(context.requestFieldReference(), queryMemory).Value;
            }
            else if (context.requestFunctionCall() != null)
            {
                return Controller.Interpret<SyneryParser.RequestFunctionCallContext, IExpressionValue, QueryMemory>(context.requestFunctionCall(), queryMemory);
            }

            throw new SyneryInterpretationException(context,
                string.Format("Unknown single value expression in {0}: '{1}'", this.GetType().Name, context.GetText()));
        }

        #endregion

        #region INTERNAL METHODS

        private ExpressionValue GetLibraryPluginVariableValueExpression(SyneryParser.LibraryPluginVariableReferenceContext context)
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

            // create an expression that calls a method on LibraryPluginManager that looks like these:
            // Memory.LibraryPluginManager.GetStaticVariableWithPrimitiveReturn(variableData)

            MethodInfo executeFunctionMethod = typeof(ILibraryPluginManager)
                .GetMethod("GetStaticVariableWithPrimitiveReturn", new Type[] { typeof(IStaticExtensionVariableData) });

            Expression libraryPluginManagerExpression = Expression.Constant(Memory.LibraryPluginManager);

            Expression paramFunctionDataExpression = Expression.Constant(variableData, typeof(IStaticExtensionVariableData));

            Expression methodCallExpression = Expression.Call(
                libraryPluginManagerExpression,
                executeFunctionMethod,
                paramFunctionDataExpression);

            SyneryType resultType = TypeHelper.GetSyneryType(variableData.Type);

            return new ExpressionValue(
                expression: methodCallExpression,
                resultType: resultType);
        }

        #endregion
    }
}
