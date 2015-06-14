using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Blocks
{
    public class HandleBlockInterpreter : IInterpreter<SyneryParser.HandleBlockContext, IHandleBlockData>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }

        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public IHandleBlockData RunWithResult(SyneryParser.HandleBlockContext context)
        {
            string parameterName = context.Identifier().GetText();
            string recordTypeName = RecordHelper.ParseRecordTypeName(context.recordType().GetText());

            IRecordType recordType = (from r in Memory.RecordTypes
                                      where r.Key.Name == recordTypeName
                                      select r.Value).FirstOrDefault();

            // check record type is known

            if (recordType == null)
                throw new SyneryInterpretationException(context, String.Format(
                    "A record type with name='{0}' wasn't found.",
                    recordTypeName));

            return new HandleBlockData()
            {
                HandledRecordType = recordType,
                ParameterName = parameterName,
                Context = context.block(),
                ParentScope = Memory.CurrentScope,
            };
        }

        #endregion
    }
}
