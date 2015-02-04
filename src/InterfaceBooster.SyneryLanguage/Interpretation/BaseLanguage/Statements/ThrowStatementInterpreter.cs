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

            // try to get the event record type from the given value
            IRecordType eventRecordType = TryToGetExceptionRecordTypeFromValue(context, exceptionValue);

            // search for a HANDLE-block that handles the thrown exception
            IHandleBlockData throwHandleBlockData = TryToFindTheHandleBlock(eventRecordType.FullName);

            if (throwHandleBlockData != null)
            {
                EventHelper.InterpretHandleBlock(Controller, throwHandleBlockData, exceptionValue);
            }
            else
            {
                throw new SyneryInterpretationException(context, "No handle block found");
            }
        }

        #endregion

        #region INTERNAL METHODS

        private IRecordType TryToGetExceptionRecordTypeFromValue(SyneryParser.ThrowStatementContext context, IValue value)
        {
            if (value.Type.UnterlyingDotNetType == typeof(IRecord))
            {
                IRecord record = (IRecord)value.Value;

                if (record.RecordType.IsType(SystemRecordTypeFactory.EXCEPTION_NAME))
                {
                    return record.RecordType;
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

        private IHandleBlockData TryToFindTheHandleBlock(string recordTypeName)
        {
            IHandleBlockData throwHandleBlockData = null;

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

                        throwHandleBlockData = (from b in observeScope.HandleBlocks
                                                where b.HandledRecordType.IsType(recordTypeName)
                                                select b).FirstOrDefault();

                        if (throwHandleBlockData != null)
                        {
                            return throwHandleBlockData;
                        }
                    }
                }
            }

            return null;
        }

        #endregion
    }
}
