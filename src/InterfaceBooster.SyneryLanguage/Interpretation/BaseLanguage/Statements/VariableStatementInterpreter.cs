using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Expressions;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Statements
{
    /// <summary>
    /// Interprets the declaration and the assignment of a variable.
    /// </summary>
    public class VariableStatementInterpreter : IInterpreter<SyneryParser.VariableStatementContext>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public void Run(SyneryParser.VariableStatementContext context)
        {
            if (context.variableDeclartion() != null)
            {
                InterpretDeclaration(context.variableDeclartion());
            }

            if (context.variableAssignment() != null)
            {
                InterpretAssignment(context.variableAssignment());
            }
        }

        #endregion

        #region INETRNAL METHODS

        private void InterpretDeclaration(SyneryParser.VariableDeclartionContext context)
        {
            SyneryType type = Controller.Interpret<SyneryParser.TypeContext, SyneryType>(context.type());

            foreach (var assignment in context.variableAssignment())
            {
                if (assignment.variableReference() != null)
                {
                    string name = assignment.variableReference().GetText();

                    // create new variable in scope
                    Memory.CurrentScope.DeclareVariable(name, type);

                    InterpretAssignment(assignment);
                }
            }
        }

        private void InterpretAssignment(SyneryParser.VariableAssignmentContext context)
        {
            if (context.variableInitializer() != null)
            {
                if (context.variableReference() != null)
                {
                    string name = context.variableReference().GetText();
                    IValue value = Controller.Interpret<SyneryParser.ExpressionContext, IValue>(context.variableInitializer().expression());

                    Memory.CurrentScope.AssignVariable(name, value.Value);
                }
                else if (context.complexReference() != null)
                {
                    IValue value = Controller.Interpret<SyneryParser.ExpressionContext, IValue>(context.variableInitializer().expression());

                    string complexReference = context.complexReference().GetText();
                    string[] parts = IdentifierHelper.ParseComplexIdentifier(complexReference);
                    string[] recordPath = parts.Take(parts.Count() - 1).ToArray();
                    string fieldName = parts.Last();

                    IValue recordValue = Controller.Interpret<SyneryParser.ComplexReferenceContext, IValue, string[]>(context.complexReference(), recordPath);

                    if (recordValue.Type.UnterlyingDotNetType == typeof(IRecord))
                    {
                        IRecord record = ((IRecord)recordValue.Value);
                        record.SetFieldValue(fieldName, value);
                    }
                }
            }
        }

        #endregion
    }
}
