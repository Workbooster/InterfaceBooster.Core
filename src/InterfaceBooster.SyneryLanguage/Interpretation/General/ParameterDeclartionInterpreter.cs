using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Interpretation.General
{
    public class ParameterDeclartionInterpreter : IInterpreter<SyneryParser.ParameterDeclartionsContext, IEnumerable<IFunctionParameterDefinition>>
    {
        #region PROPERTIES
        
        public ISyneryMemory Memory { get; set; }

        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public IEnumerable<IFunctionParameterDefinition> RunWithResult(SyneryParser.ParameterDeclartionsContext context)
        {
            List<IFunctionParameterDefinition> listOfParameters = new List<IFunctionParameterDefinition>();

            if (context != null
                && context.parameterDeclartion() != null)
            {
                foreach (SyneryParser.ParameterDeclartionContext parameterDeclaration in context.parameterDeclartion())
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

                    listOfParameters.Add(parameter);
                }
            }

            return listOfParameters;
        }

        #endregion
    }
}
