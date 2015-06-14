using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Tools.Data.ExceptionHandling;

namespace InterfaceBooster.SyneryLanguage.Common
{
    public class InterpretationController : IInterpretationController
    {
        #region EVENTS

        public event StateChangedEventHandler OnStateChanged;

        public event EnterContextEventHandler OnEnterContext;

        public event ExitContextEventHandler OnExitContext;

        #endregion

        #region MEMBERS

        private InterpreationStateEnum _State;

        #endregion

        #region PROPERTIES

        public IInterpreterFactory Factory { get; set; }

        public ISyneryMemory Memory { get; set; }

        public InterpreationStateEnum State
        {
            get { return _State; }
            set
            {
                // inform the observers of the state
                if (OnStateChanged != null)
                    OnStateChanged(_State, value);

                _State = value;
            }
        }

        public Antlr4.Runtime.ParserRuleContext CurrentContext { get; private set; }

        #endregion

        #region PUBLIC METHODS

        public InterpretationController(IInterpreterFactory factory, ISyneryMemory memory)
        {
            Factory = factory;
            Memory = memory;

            State = InterpreationStateEnum.Ready;
        }

        public void Interpret<contextT>(contextT context) where contextT : Antlr4.Runtime.ParserRuleContext
        {
            StartInterpreation(context);

            // get the requested interpreter from the factory and set/update the needed properties
            IInterpreter<contextT> interpreter = Factory.GetInterpreter<contextT>();
            interpreter.Controller = this;
            interpreter.Memory = Memory;

            try
            {
                interpreter.Run(context);
            }
            catch (Exception ex)
            {
                if (!HandleException(ex, context))
                {
                    throw;
                }
            }

            EndInterpreation(context);
        }

        public resultT Interpret<contextT, resultT>(contextT context) where contextT : Antlr4.Runtime.ParserRuleContext
        {
            StartInterpreation(context);

            // create a local variable for the value to return
            resultT returnValue;

            // get the requested interpreter from the factory and set/update the needed properties
            IInterpreter<contextT, resultT> interpreter = Factory.GetInterpreter<contextT, resultT>();
            interpreter.Controller = this;
            interpreter.Memory = Memory;

            try
            {
                returnValue = interpreter.RunWithResult(context);
            }
            catch (Exception ex)
            {
                if (!HandleException(ex, context))
                {
                    throw;
                }

                throw new SystemException("Untreated error in InterpretationController.Interpret");
            }

            EndInterpreation(context);

            return returnValue;
        }

        public resultT Interpret<contextT, resultT, paramT>(contextT context, paramT parameter) where contextT : Antlr4.Runtime.ParserRuleContext
        {
            StartInterpreation(context);

            // create a local variable for the value to return
            resultT returnValue;

            // get the requested interpreter from the factory and set/update the needed properties
            IInterpreter<contextT, resultT, paramT> interpreter = Factory.GetInterpreter<contextT, resultT, paramT>();
            interpreter.Controller = this;
            interpreter.Memory = Memory;

            try
            {
                returnValue = interpreter.RunWithResult(context, parameter);
            }
            catch (Exception ex)
            {
                if (!HandleException(ex, context))
                {
                    throw;
                }

                throw new SystemException("Untreated error in InterpretationController.Interpret");
            }

            EndInterpreation(context);

            return returnValue;
        }

        #endregion

        #region INTERNAL METHOS

        private bool HandleException(Exception exception, Antlr4.Runtime.ParserRuleContext context)
        {
            if (exception is InterfaceBoosterException)
            {
                // exception has already been handled
                return false;
            }

            string message = string.Format("An unexpected error occured while interpreting '{1}' on line {2}: {0}{3}",
                Environment.NewLine,
                context.GetText(),
                context.Start.Line,
                ExceptionHelper.GetNestedExceptionMessages(exception));

            // it seems to be an unknown exception
            // create a new one and print all the exception messages
            throw new SyneryInterpretationException(context, message, exception);
        }

        private void StartInterpreation(Antlr4.Runtime.ParserRuleContext context)
        {
            CurrentContext = context;
            State = InterpreationStateEnum.Running;

            if (OnEnterContext != null)
                OnEnterContext(context);
        }

        private void EndInterpreation(Antlr4.Runtime.ParserRuleContext context)
        {
            if (OnExitContext != null)
                OnExitContext(context);
        }

        #endregion
    }
}
