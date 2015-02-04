using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.SyneryLanguage.Model.SyneryTypes;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Records
{
    public class RecordInitializerInterpreter : IInterpreter<SyneryParser.RecordInitializerContext, IValue>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public IValue Run(SyneryParser.RecordInitializerContext context)
        {
            IRecord record;
            string recordTypeName = RecordHelper.ParseRecordTypeName(context.recordType().GetText());

            KeyValuePair<SyneryType, IRecordType> recordTypeDefinition = (from r in Memory.RecordTypes
                                                                where r.Key.Name == recordTypeName
                                                                select r).FirstOrDefault();

            // check record type is known

            if (recordTypeDefinition.Key == null)
                throw new SyneryInterpretationException(context, String.Format(
                    "A record type with name='{0}' wasn't found.",
                    recordTypeName));

            // create record

            record = new Record(recordTypeDefinition.Value);

            if (context.expressionList() != null)
            {
                IList<IValue> listOfValues = Controller.Interpret<SyneryParser.ExpressionListContext, IList<IValue>>(context.expressionList());

                record = InitializeRecordByExpressionList(context, record, listOfValues);
            }
            else if (context.keyValueList() != null)
            {
                IDictionary<string[], IValue> listOfValues = Controller.Interpret<SyneryParser.KeyValueListContext, IDictionary<string[], IValue>>(context.keyValueList());

                record = InitializeRecordByKeyValueList(context, record, listOfValues);
            }

            return new TypedValue(recordTypeDefinition.Key, record);
        }

        #endregion

        #region INTERNAL METHODS

        private IRecord InitializeRecordByExpressionList(SyneryParser.RecordInitializerContext context, IRecord record, IList<IValue> listOfValues)
        {
            if (listOfValues.Count <= record.RecordType.Fields.Count)
            {
                for (int i = 0; i < listOfValues.Count; i++)
                {
                    string fieldName = record.RecordType.Fields[i].Name;
                    IValue value = listOfValues[i];

                    try
                    {
                        record.SetFieldValue(fieldName, value);
                    }
                    catch (Exception ex)
                    {
                        throw new SyneryInterpretationException(context, String.Format(
                            "Error while initializing the field '{0}' of record type='{1}'. Expected value-type: {2} / Given value-type: {3}.",
                            fieldName,
                            record.RecordType.FullName,
                            record.RecordType.Fields[i].Type.PublicName,
                            value.Type.PublicName), ex);
                    }
                }
            }
            else
            {
                throw new SyneryInterpretationException(context, String.Format(
                    "The expression list contains more values than the record can take. Expected: {0}/ Given: {1}.",
                    record.RecordType.Fields.Count, listOfValues.Count));
            }

            return record;
        }

        private IRecord InitializeRecordByKeyValueList(SyneryParser.RecordInitializerContext context, IRecord record, IDictionary<string[], IValue> listOfValues)
        {
            foreach (var item in listOfValues)
            {
                if (item.Key.Count() == 1)
                {
                    string fieldName = item.Key[0];
                    IValue value = item.Value;

                    try
                    {
                        record.SetFieldValue(fieldName, value);
                    }
                    catch (Exception ex)
                    {
                        throw new SyneryInterpretationException(context, String.Format(
                            "Error while initializing the field '{0}' of record type='{1}'. Given value-type: {2}.",
                            fieldName,
                            record.RecordType.FullName,
                            value.Type.PublicName), ex);
                    }
                }
                else
                {
                    throw new SyneryInterpretationException(context, String.Format(
                        "Error while initializing the record type='{0}'. No complex identifers allowed (given: '{1}').",
                        record.RecordType.FullName,
                        String.Join(".", item.Key)));
                }
            }

            return record;
        }

        #endregion
    }
}
