﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Tools.Data.ExceptionHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;
using InterfaceBooster.Common.Interfaces.Utilities.SyneryLanguage;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.SyneryLanguage.Model.SyneryTypes.SyneryRecords;

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

        public void HandleSyneryEvent(Antlr4.Runtime.ParserRuleContext context, IValue eventValue)
        {

            if (eventValue.Value is ExecutionExceptionRecord)
            {
                // if it is an #.ExecutionException or a derived type
                // set the information about the context the exception has occurred
                ((ExecutionExceptionRecord)eventValue.Value).Line = context.Start.Line;
                ((ExecutionExceptionRecord)eventValue.Value).CharPosition = context.Start.Column;
            }

            // try to get the event record from the given value
            IRecord eventRecord = TryToGetEventRecordFromValue(context, eventValue);

            // get the event record type from the given value
            IRecordType eventRecordType = eventRecord.RecordType;

            // check whether it is an exception
            bool isException = eventRecordType.IsType(ExceptionRecord.RECORD_TYPE_NAME);

            // search for a HANDLE-block that handles the emitted/thrown event
            IEnumerable<IHandleBlockData> listOfHandleBlocks = GetHandleBlocks(eventRecord, eventRecordType.FullName);

            if (isException == true && listOfHandleBlocks.Count() == 0)
            {
                // no handle block found for the given exception
                // throw a real exception

                string message = "Unhandled Exception. Data:";

                foreach (var field in eventRecord.Data)
                {
                    message += String.Format("{0}{0}{1}={2}", Environment.NewLine, field.Key, (field.Value.Value ?? "NULL"));
                }

                throw new InterfaceBoosterException(message);
            }
            else
            {
                bool isFirst = true;

                foreach (var handleBlock in listOfHandleBlocks)
                {
                    EventHelper.InterpretHandleBlock(this, handleBlock, eventValue);

                    // mark the event as handled after the first handle block is executed
                    if (isFirst)
                    {
                        isFirst = false;
                        eventRecord.SetFieldValue("IsHandled", new TypedValue(TypeHelper.BOOL_TYPE, true));
                    }
                }
            }
        }

        #endregion

        #region INTERNAL METHOS

        #region EVENT HANDLING

        /// <summary>
        /// Tries to get the event record from the given value.
        /// If the values doesn't contain a record type or if the record type isn't assignable from the event 
        /// record type an exception is thrown.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private IRecord TryToGetEventRecordFromValue(Antlr4.Runtime.ParserRuleContext context, IValue value)
        {
            if (value.Type.UnterlyingDotNetType == typeof(IRecord))
            {
                IRecord record = (IRecord)value.Value;

                if (record.RecordType.IsType(EventRecord.RECORD_TYPE_NAME))
                {
                    return record;
                }
                else
                {
                    throw new SyneryInterpretationException(context, String.Format(
                        "The record type '{0}' is not assignable from '{1}'.",
                        record.RecordType.FullName,
                        EventRecord.RECORD_TYPE_NAME));
                }
            }
            else
            {
                throw new SyneryInterpretationException(context, String.Format(
                    "The given value is not a record type. HANDLE needs as parameter a record type that is assignable from '{0}'.",
                    EventRecord.RECORD_TYPE_NAME));
            }
        }

        /// <summary>
        /// Gets all matching handle blocks for the given record type.
        /// </summary>
        /// <param name="recordTypeName">The name of the record type that should be handled.</param>
        /// <returns>A list of matching handle blocks.</returns>
        private IEnumerable<IHandleBlockData> GetHandleBlocks(IRecord eventRecord, string recordTypeName)
        {
            // check whether it is an exception
            bool isException = eventRecord.RecordType.IsType(ExceptionRecord.RECORD_TYPE_NAME);

            List<IHandleBlockData> listOfHandleBlocks = new List<IHandleBlockData>();

            foreach (IScope scope in Memory.Scopes)
            {
                INestedScope nestedScope = scope as INestedScope;

                // check whether the scope is a block-scope that not is terminated

                if (nestedScope != null && nestedScope.IsTerminated == false)
                {
                    // check whether the scope is from an OBSERVE-block

                    IObserveScope observeScope = scope as IObserveScope;

                    if (observeScope != null)
                    {
                        // check whether the OBSERVE-block handles the thrown exception

                        var listOfMatchingHandleBlocks = from b in observeScope.HandleBlocks
                                                         where RecordHelper.IsDerivedType(Memory, recordTypeName, b.HandledRecordType.Name)
                                                         select b;

                        listOfHandleBlocks.AddRange(listOfMatchingHandleBlocks);
                    }


                    // The behavior for exceptions is different than the behavior for events:
                    // 1. An exception terminates all parent blocks until a HANDLE-block is found.
                    // 2. Only one matching HANDLE-block is required to stop the exception.

                    if (isException == true)
                    {
                        // mark the block as terminated
                        nestedScope.IsTerminated = true;

                        // if a HANDLE-block for the exception was found stop the search for other 
                        // HANDLE-blocks because an exception only breaks the current OBSERVE-block.
                        if (listOfHandleBlocks.Count != 0)
                        {
                            return listOfHandleBlocks;
                        }
                    }
                }
            }

            return listOfHandleBlocks;
        }

        #endregion

        /// <summary>
        /// Handles a .NET Exception that occurred during the interpretation of Synery code.
        /// </summary>
        /// <param name="exception">The thrown .NET Exception</param>
        /// <param name="context">The interpretation context that caused the Exception (e.g. used to print the line number).</param>
        /// <returns>
        /// false = Exception hasn't been handled, so please re-throw it.
        /// true = Exception handled. No further action is required. Continue interpretation. 
        /// </returns>
        private bool HandleException(Exception exception, Antlr4.Runtime.ParserRuleContext context)
        {

            INestedScope nestedScope = Memory.Scopes.OfType<INestedScope>().FirstOrDefault();

            if (nestedScope != null && nestedScope.IsTerminated == true
                && !(context is SyneryParser.ThrowStatementContext)
                && !(exception is InterfaceBoosterException))
            {
                // The current block has already been terminated.
                // Therefore don't throw or create any new exceptions
                return true;
            }
            else
            {
                if (context is SyneryParser.ObserveBlockContext)
                {
                    // We're now in an OBSERVE-block.
                    // Try to find a handle block that catches the exception.

                    ExecutionExceptionRecord executionException = new ExecutionExceptionRecord();
                    executionException.Message = exception.Message;
                    executionException.Line = context.Start.Line;
                    executionException.CharPosition = context.Start.Column;

                    this.HandleSyneryEvent(context, executionException.GetAsSyneryValue());

                    return true;
                }
                if (exception is SyneryInterpretationException)
                {
                    // The exception already is an interpretation exception.
                    // Continue re-throwing it.
                    return false;
                }

                /* 
                 * It seems that the exception is not an interpretation exception.
                 * Create a new exception and enrich it with the parser-context and  a
                 * new message that contains all messages from the nested exceptions. 
                 */

                string message = string.Format("An unexpected error occurred while interpreting '{1}' on line {2}: {0}{3}",
                    Environment.NewLine,
                    context.GetText(),
                    context.Start.Line,
                    ExceptionHelper.GetNestedExceptionMessages(exception));

                throw new SyneryInterpretationException(context, message, exception);
            }
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
