using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Records
{
    public class RecordTypeInterpreter : IInterpreter<SyneryParser.RecordTypeContext, KeyValuePair<SyneryType, IRecordType>>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public KeyValuePair<SyneryType, IRecordType> RunWithResult(SyneryParser.RecordTypeContext context)
        {
            string recordTypeName = RecordHelper.ParseRecordTypeName(context.GetText());

            KeyValuePair<SyneryType, IRecordType> recordTypeDefinition = (from r in Memory.RecordTypes
                                                                          where r.Key.Name == recordTypeName
                                                                          select r).FirstOrDefault();

            // check record type is known

            if (recordTypeDefinition.Key == null)
                throw new SyneryInterpretationException(context, String.Format(
                    "A record type with name='{0}' wasn't found.",
                    recordTypeName));

            return recordTypeDefinition;
        }

        #endregion
    }
}
