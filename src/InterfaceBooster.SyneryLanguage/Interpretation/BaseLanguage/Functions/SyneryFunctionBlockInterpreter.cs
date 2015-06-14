using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Common;
using InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Expressions;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Functions
{
    public class SyneryFunctionBlockInterpreter : IInterpreter<SyneryParser.SyneryFunctionBlockContext, IFunctionData, string>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public IFunctionData RunWithResult(SyneryParser.SyneryFunctionBlockContext context, string codeFileAlias = null)
        {
            IFunctionData functionData = new FunctionData();

            functionData.CodeFileAlias = codeFileAlias;
            functionData.Context = context.block();
            functionData.FunctionDefinition = new FunctionDefinition();
            functionData.FunctionDefinition.Name = context.Identifier().GetText();
            functionData.FunctionDefinition.Parameters = new List<IFunctionParameterDefinition>();

            if (context.type() != null)
            {
                functionData.FunctionDefinition.ReturnType = Controller.Interpret<SyneryParser.TypeContext, SyneryType>(context.type());
            }
            else
            {
                functionData.FunctionDefinition.HasReturnValue = false;
            }

            if (context.parameterDeclartions() != null
                && context.parameterDeclartions().parameterDeclartion() != null)
            {
                foreach (SyneryParser.ParameterDeclartionContext parameterDeclaration in context.parameterDeclartions().parameterDeclartion())
                {
                    IFunctionParameterDefinition parameter = new FunctionParameterDefinition();
                    parameter.Type = Controller.Interpret<SyneryParser.TypeContext, SyneryType>(parameterDeclaration.type());
                    parameter.Name = parameterDeclaration.Identifier().GetText();

                    if (parameterDeclaration.parameterInitializer() != null)
                    {
                        if (parameterDeclaration.parameterInitializer().literal() != null)
                        {
                            parameter.DefaultValue = Controller.Interpret<SyneryParser.LiteralContext, IValue>(parameterDeclaration.parameterInitializer().literal());

                            // validate declared type and the type of the given default value
                            if (parameter.Type != parameter.DefaultValue.Type
                                && parameter.DefaultValue.Value != null)
                            {
                                string message = String.Format(
                                    "Declared type '{0}' and given value type '{1}' don't match.",
                                    parameter.Type.PublicName,
                                    parameter.DefaultValue.Type.PublicName);
                                throw new SyneryInterpretationException(parameterDeclaration, message);
                            }
                        }
                    }

                    functionData.FunctionDefinition.Parameters.Add(parameter);
                }
            }

            return functionData;
        }

        #endregion
    }
}
