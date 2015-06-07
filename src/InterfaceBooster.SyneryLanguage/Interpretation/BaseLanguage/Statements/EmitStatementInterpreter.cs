using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Common;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;
using InterfaceBooster.Common.Interfaces.Utilities.SyneryLanguage;

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Statements
{
    public class EmitStatementInterpreter : IInterpreter<SyneryParser.EmitStatementContext>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }

        public IInterpretationController Controller { get; set; }
         
        #endregion

        #region PUBLIC METHODS

        public void Run(SyneryParser.EmitStatementContext context)
        {
            // get the emitted value
            IValue emitValue = Controller.Interpret<SyneryParser.ExpressionContext, IValue>(context.expression());

            // try to get the event record from the given value
            IRecord eventRecord = TryToGetEventRecordFromValue(context, emitValue);

            // get the event record type from the given value
            IRecordType eventRecordType = eventRecord.RecordType;

            // search for a HANDLE-block that handles the emited event
            IEnumerable<IHandleBlockData> listOfHandleBlocks = GetHandleBlocks(eventRecordType.FullName);

            bool isFirst = true;

            foreach (var handleBlock in listOfHandleBlocks)
            {
                EventHelper.InterpretHandleBlock(Controller, handleBlock, emitValue);

                // mark the exception as handled after the first handle block is executed
                if (isFirst)
                {
                    isFirst = false;
                    eventRecord.SetFieldValue("IsHandled", new TypedValue(TypeHelper.BOOL_TYPE, true));
                }
            }
        }

        #endregion

        #region INTERNAL METHODS

        /// <summary>
        /// Tries to get the event record from the given value.
        /// If the values doesn't contain a record type or if the record type isn't assignable from the event 
        /// record type an exception is thrown.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private IRecord TryToGetEventRecordFromValue(SyneryParser.EmitStatementContext context, IValue value)
        {
            if (value.Type.UnterlyingDotNetType == typeof(IRecord))
            {
                IRecord record = (IRecord)value.Value;

                if (record.RecordType.IsType(SystemRecordTypeFactory.EVENT_NAME))
                {
                    return record;
                }
                else
                {
                    throw new SyneryInterpretationException(context, String.Format(
                        "The record type '{0}' is not assignable from '{1}'.",
                        record.RecordType.FullName,
                        SystemRecordTypeFactory.EVENT_NAME));
                }
            }
            else
            {
                throw new SyneryInterpretationException(context, String.Format(
                    "The given value is not a record type. EMIT needs as parameter a record type that is assignable from '{0}'.",
                    SystemRecordTypeFactory.EVENT_NAME));
            }
        }

        /// <summary>
        /// Gets all matching handle blocks for the given record type.
        /// </summary>
        /// <param name="recordTypeName">The name of the record type that should be handled.</param>
        /// <returns>A list of matching handle blocks.</returns>
        private IEnumerable<IHandleBlockData> GetHandleBlocks(string recordTypeName)
        {
            List<IHandleBlockData> listOfHandleBlocks = new List<IHandleBlockData>();

            foreach (var scope in Memory.Scopes)
            {
                IObserveScope observerScope = scope as IObserveScope;

                if (observerScope != null)
                {
                    var listOfMatchingHandleBlocks = from b in observerScope.HandleBlocks
                                                     where b.HandledRecordType.IsType(recordTypeName)
                                                     select b;

                    listOfHandleBlocks.AddRange(listOfMatchingHandleBlocks);
                }
            }

            return listOfHandleBlocks;
        }

        #endregion
    }
}
