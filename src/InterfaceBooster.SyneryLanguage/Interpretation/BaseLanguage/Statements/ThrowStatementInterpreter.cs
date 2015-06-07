using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Common;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.Utilities.SyneryLanguage;

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Statements
{
    public class ThrowStatementInterpreter : IInterpreter<SyneryParser.ThrowStatementContext>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }

        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public void Run(SyneryParser.ThrowStatementContext context)
        {
            // get the thrown exception value
            IValue exceptionValue = Controller.Interpret<SyneryParser.ExpressionContext, IValue>(context.expression());

            // try to get the exception record from the given value
            IRecord exceptionRecord = TryToGetExceptionRecordFromValue(context, exceptionValue);
            
            // get the event record type from the given value
            IRecordType exceptionRecordType = exceptionRecord.RecordType;

            // search for a HANDLE-block that handles the thrown exception
            IEnumerable<IHandleBlockData> listOfHandleBlocks = GetHandleBlocks(exceptionRecordType.FullName);

            bool isFirst = true;

            foreach (var handleBlock in listOfHandleBlocks)
            {
                EventHelper.InterpretHandleBlock(Controller, handleBlock, exceptionValue);

                // mark the exception as handled after the first handle block is executed
                if (isFirst)
                {
                    isFirst = false;
                    exceptionRecord.SetFieldValue("IsHandled", new TypedValue(TypeHelper.BOOL_TYPE, true));
                }
            }
        }

        #endregion

        #region INTERNAL METHODS

        private IRecord TryToGetExceptionRecordFromValue(SyneryParser.ThrowStatementContext context, IValue value)
        {
            if (value.Type.UnterlyingDotNetType == typeof(IRecord))
            {
                IRecord record = (IRecord)value.Value;

                if (record.RecordType.IsType(SystemRecordTypeFactory.EXCEPTION_NAME))
                {
                    return record;
                }
                else
                {
                    throw new SyneryInterpretationException(context, String.Format(
                        "The record type '{0}' is not assignable from '{1}'.",
                        record.RecordType.FullName,
                        SystemRecordTypeFactory.EXCEPTION_NAME));
                }
            }
            else
            {
                throw new SyneryInterpretationException(context, String.Format(
                    "The given value is not a record type. THROW needs as parameter a record type that is assignable from '{0}'.",
                    SystemRecordTypeFactory.EXCEPTION_NAME));
            }
        }

        private IEnumerable<IHandleBlockData> GetHandleBlocks(string recordTypeName)
        {
            List<IHandleBlockData> listOfHandleBlocks = new List<IHandleBlockData>();

            foreach (IScope scope in Memory.Scopes)
            {
                INestedScope nestedScope = scope as INestedScope;

                if (nestedScope != null)
                {
                    // mark the block as terminated anyway
                    nestedScope.IsTerminated = true;

                    // check whether the scope is from an OBSERVE-block

                    IObserveScope observeScope = scope as IObserveScope;

                    if (observeScope != null)
                    {
                        // check whether the OBSERVE-block handles the thrown exception

                        var listOfMatchingHandleBlocks = from b in observeScope.HandleBlocks
                                                         where b.HandledRecordType.IsType(recordTypeName)
                                                         select b;

                        listOfHandleBlocks.AddRange(listOfMatchingHandleBlocks);
                    }
                }
            }

            return listOfHandleBlocks;
        }

        #endregion
    }
}
