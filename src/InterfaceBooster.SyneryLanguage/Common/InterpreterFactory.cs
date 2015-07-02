using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage;
using InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Blocks;
using InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Expressions;
using InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Functions;
using InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Records;
using InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Statements;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.SyneryLanguage.Interpretation.ProviderPlugins.Commands;
using InterfaceBooster.SyneryLanguage.Interpretation.ProviderPlugins.Statements;
using InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Commands;
using InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Expressions;
using InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Functions;
using InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Statements;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;

namespace InterfaceBooster.SyneryLanguage.Common
{
    public class InterpreterFactory : IInterpreterFactory
    {
        #region INTERNAL STRUCTURES

        struct Signature
        {
            public Type ContextType { get; set; }
            public Type ResultType { get; set; }
            public Type ParameterType { get; set; }
        }

        #endregion

        #region MEMBERS

        private Dictionary<Signature, object> _Interpreters;

        #endregion

        #region PROPERTOES

        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public InterpreterFactory()
        {
            _Interpreters = new Dictionary<Signature, object>();
        }

        #region IMPLEMENTATION OF IInterpreterFactory

        public void SetInterpreter(IInterpreter interpreter)
        {
            // get all interfaces that are implemented by the given interpreter

            Type[] listOfAssignedInterfaces = interpreter.GetType().GetInterfaces();

            foreach (Type assignedInterface in listOfAssignedInterfaces)
            {
                // ignore all interfaces that don't inherit from IInterpreter

                if (typeof(IInterpreter).IsAssignableFrom(assignedInterface))
                {
                    Signature signature;

                    // Search for the generic types.
                    // Those types make the signature of the interpreter.

                    Type[] listOfGenericArguments = assignedInterface.GetGenericArguments();

                    if (listOfGenericArguments.Count() == 1)
                    {
                        signature = new Signature { ContextType = listOfGenericArguments[0], };
                        _Interpreters.Add(signature, interpreter);
                    }
                    else if (listOfGenericArguments.Count() == 2)
                    {
                        signature = new Signature { ContextType = listOfGenericArguments[0], ResultType = listOfGenericArguments[1], };
                        _Interpreters.Add(signature, interpreter);
                    }
                    else if (listOfGenericArguments.Count() == 3)
                    {
                        signature = new Signature { ContextType = listOfGenericArguments[0], ResultType = listOfGenericArguments[1], ParameterType = listOfGenericArguments[2], };
                        _Interpreters.Add(signature, interpreter);
                    }
                }
            }
        }

        public IInterpreter<contextT> GetInterpreter<contextT>() where contextT : Antlr4.Runtime.ParserRuleContext
        {
            Signature signature = new Signature { ContextType = typeof(contextT), };

            if (_Interpreters.ContainsKey(signature))
            {
                return (IInterpreter<contextT>)_Interpreters[signature];
            }

            throw new Exception(
                String.Format("No interpreter with the given signature found. Context type: '{0}'."
                , typeof(contextT).Name));
        }

        public IInterpreter<contextT, resultT> GetInterpreter<contextT, resultT>() where contextT : Antlr4.Runtime.ParserRuleContext
        {
            Signature signature = new Signature { ContextType = typeof(contextT), ResultType = typeof(resultT), };

            if (_Interpreters.ContainsKey(signature))
            {
                return (IInterpreter<contextT, resultT>)_Interpreters[signature];
            }

            throw new Exception(
                String.Format("No interpreter with the given signature found. Context type: '{0}' / result type: '{1}'."
                , typeof(contextT).Name
                , typeof(resultT).Name));
        }

        public IInterpreter<contextT, resultT, paramT> GetInterpreter<contextT, resultT, paramT>() where contextT : Antlr4.Runtime.ParserRuleContext
        {
            Signature signature = new Signature { ContextType = typeof(contextT), ResultType = typeof(resultT), ParameterType = typeof(paramT), };

            if (_Interpreters.ContainsKey(signature))
            {
                return (IInterpreter<contextT, resultT, paramT>)_Interpreters[signature];
            }

            throw new Exception(
                String.Format("No interpreter with the given signature found. Context type: '{0}' / result type: '{1}' / parameter type: '{2}'."
                , typeof(contextT).Name
                , typeof(resultT).Name
                , typeof(paramT).Name));
        }

        #endregion

        #region DEFAULT FACTORY

        public static IInterpreterFactory GetDefaultInterpreterFactory()
        {
            IInterpreterFactory factory = new InterpreterFactory();
            factory.SetInterpreter(new BlockInterpreter());
            factory.SetInterpreter(new BlockUnitInterpreter());
            factory.SetInterpreter(new ObserveBlockInterpreter());
            factory.SetInterpreter(new HandleBlockInterpreter());
            factory.SetInterpreter(new ComplexReferenceInterpreter());
            factory.SetInterpreter(new DateTimeLiteralInterpreter());
            factory.SetInterpreter(new ExpressionInterpreter());
            factory.SetInterpreter(new ExpressionListInterpreter());
            factory.SetInterpreter(new LiteralInterpreter());
            factory.SetInterpreter(new PrimaryInterpreter());
            factory.SetInterpreter(new SingleValueInterpreter());
            factory.SetInterpreter(new RecordInitializerInterpreter());
            factory.SetInterpreter(new FunctionCallInterpreter());
            factory.SetInterpreter(new LibraryPluginFunctionCallInterpreter());
            factory.SetInterpreter(new SyneryFunctionCallInterpreter());
            factory.SetInterpreter(new RecordTypeInterpreter());
            factory.SetInterpreter(new EachStatementInterpreter());
            factory.SetInterpreter(new ElseStatementInterpreter());
            factory.SetInterpreter(new EmitStatementInterpreter());
            factory.SetInterpreter(new IfStatementInterpreter());
            factory.SetInterpreter(new ReturnStatementInterpreter());
            factory.SetInterpreter(new LibraryPluginVariableStatementInterpreter());
            factory.SetInterpreter(new LogStatementInterpreter());
            factory.SetInterpreter(new TableStatementInterpreter());
            factory.SetInterpreter(new TableAddStatementInterpreter());
            factory.SetInterpreter(new TableDropStatementInterpreter());
            factory.SetInterpreter(new ThrowStatementInterpreter());
            factory.SetInterpreter(new VariableStatementInterpreter());
            factory.SetInterpreter(new ProgramInterpreter());
            factory.SetInterpreter(new ProgramUnitInterpreter());
            factory.SetInterpreter(new InternalIdentifierListInterpreter());
            factory.SetInterpreter(new KeyValueAssignmentInterpreter());
            factory.SetInterpreter(new KeyValueListInterpreter());
            factory.SetInterpreter(new ParameterDeclartionInterpreter());
            factory.SetInterpreter(new TypeInterpreter());
            factory.SetInterpreter(new FieldsCommandInterpreter());
            factory.SetInterpreter(new FromCommandInterpreter());
            factory.SetInterpreter(new GetCommandInterpreter());
            factory.SetInterpreter(new SetCommandInterpreter());
            factory.SetInterpreter(new ToCommandInterpreter());
            factory.SetInterpreter(new ProviderPluginConnectStatementInterpreter());
            factory.SetInterpreter(new ProviderPluginCreateStatementInterpreter());
            factory.SetInterpreter(new ProviderPluginDataExchangeStatementInterpreter());
            factory.SetInterpreter(new ProviderPluginDeleteStatementInterpreter());
            factory.SetInterpreter(new ProviderPluginExecuteStatementInterpreter());
            factory.SetInterpreter(new ProviderPluginReadStatementInterpreter());
            factory.SetInterpreter(new ProviderPluginSaveStatementInterpreter());
            factory.SetInterpreter(new ProviderPluginStatementInterpreter());
            factory.SetInterpreter(new ProviderPluginUpdateStatementInterpreter());
            factory.SetInterpreter(new RequestDistinctCommandInterpreter());
            factory.SetInterpreter(new RequestJoinCommandInterpreter());
            factory.SetInterpreter(new RequestLeftJoinCommandInterpreter());
            factory.SetInterpreter(new RequestOrderByCommandInterpreter());
            factory.SetInterpreter(new RequestSelectCommandInterpreter());
            factory.SetInterpreter(new RequestWhereCommandInterpreter());
            factory.SetInterpreter(new RequestExpressionInterpreter());
            factory.SetInterpreter(new RequestExpressionListInterpreter());
            factory.SetInterpreter(new RequestComplexReferenceInterpreter());
            factory.SetInterpreter(new RequestFieldReferenceInterpreter());
            factory.SetInterpreter(new RequestPrimaryInterpreter());
            factory.SetInterpreter(new RequestSelectItemInterpreter());
            factory.SetInterpreter(new RequestSingleValueInterpreter());
            factory.SetInterpreter(new RequestFunctionCallInterpreter());
            factory.SetInterpreter(new RequestSyneryFunctionCallInterpreter());
            factory.SetInterpreter(new RequestLibraryPluginFunctionCallInterpreter());
            factory.SetInterpreter(new RequestStatementInterpreter());

            return factory;
        }

        #endregion

        #endregion
    }
}
