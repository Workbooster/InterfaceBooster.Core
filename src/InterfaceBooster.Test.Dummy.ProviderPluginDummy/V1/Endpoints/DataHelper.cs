using InterfaceBooster.ProviderPluginApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Test.Dummy.ProviderPluginDummy.V1.Endpoints
{
    static class DataHelper
    {
        public static RecordSet RemoveUnrequestedFields(RecordSet recordSet, IEnumerable<Field> listOfRequestedFields)
        {
            RecordSet newRecordSet = recordSet.Clone();
            Field[] listOfAvailableFields = recordSet.Schema.Fields.ToArray();

            foreach (var field in recordSet.Schema.Fields)
            {
                if (listOfRequestedFields.Count(f => f.Name == field.Name) == 0)
                {
                    newRecordSet.RemoveField(field);
                }
            }

            return newRecordSet;
        }

        public static RecordSet CreateRecords(RecordSet existingRecordSet, RecordSet requestRecordSet)
        {
            foreach (var requestRecord in requestRecordSet)
            {
                Record newRecord = new Record(existingRecordSet.Schema);

                for (int i = 0; i < existingRecordSet.Schema.Fields.Count; i++)
                {
                    Field field = existingRecordSet.Schema.Fields[i];
                    int requestFieldPosition = requestRecordSet.Schema.Fields.IndexOf(field);

                    if (requestFieldPosition != -1)
                    {
                        newRecord[i] = requestRecord[requestFieldPosition];
                    }
                }

                existingRecordSet.Add(newRecord);
            }

            return existingRecordSet;
        }

        public static RecordSet UpdateRecords(RecordSet existingRecordSet, RecordSet requestRecordSet)
        {
            List<Record> listOfRecords = new List<Record>(existingRecordSet);

            Field keyField = (from f in existingRecordSet.Schema.Fields
                              where f.IsKey == true
                              select f).FirstOrDefault();

            if (keyField == null)
                throw new Exception("No key field found.");

            int keyFieldPosition = existingRecordSet.Schema.Fields.IndexOf(keyField);

            int requestKeyFieldPosition = requestRecordSet.Schema.Fields.IndexOf(keyField);

            if (requestKeyFieldPosition == -1)
                throw new Exception("Key field in request record set is missing.");

            foreach (var requestRecord in requestRecordSet)
            {
                object keyValue = requestRecord[requestKeyFieldPosition];
                Record changedRecord = (from e in listOfRecords
                                        where e[keyFieldPosition].Equals(keyValue)
                                        select e).FirstOrDefault();

                if (changedRecord == null)
                    throw new Exception(String.Format("Record with key '{0}' not found in '{1}'", keyValue, existingRecordSet.Schema.InternalName));

                for (int i = 0; i < existingRecordSet.Schema.Fields.Count; i++)
                {
                    Field field = existingRecordSet.Schema.Fields[i];
                    int requestFieldPosition = requestRecordSet.Schema.Fields.IndexOf(field);

                    if (requestFieldPosition != -1)
                    {
                        changedRecord[i] = requestRecord[requestFieldPosition];
                    }
                }
            }

            existingRecordSet.SetData(listOfRecords);

            return existingRecordSet;
        }

        public static RecordSet SaveRecords(RecordSet existingRecordSet, RecordSet requestRecordSet)
        {
            Field keyField = (from f in existingRecordSet.Schema.Fields
                              where f.IsKey == true
                              select f).FirstOrDefault();

            if (keyField == null)
                throw new Exception("No key field found.");

            int keyFieldPosition = existingRecordSet.Schema.Fields.IndexOf(keyField);

            int requestKeyFieldPosition = requestRecordSet.Schema.Fields.IndexOf(keyField);

            if (requestKeyFieldPosition == -1)
                throw new Exception("Key field in request record set is missing.");

            foreach (var requestRecord in requestRecordSet)
            {
                object keyValue = requestRecord[requestKeyFieldPosition];
                Record changedRecord = (from e in existingRecordSet
                                        where e[keyFieldPosition].Equals(keyValue)
                                        select e).FirstOrDefault();

                if (changedRecord != null)
                {
                    // it's an existing record -> update it

                    for (int i = 0; i < existingRecordSet.Schema.Fields.Count; i++)
                    {
                        Field field = existingRecordSet.Schema.Fields[i];
                        int requestFieldPosition = requestRecordSet.Schema.Fields.IndexOf(field);

                        if (requestFieldPosition != -1)
                        {
                            changedRecord[i] = requestRecord[requestFieldPosition];
                        }
                    }
                }
                else
                {
                    // no existing record found -> create a new one

                    Record newRecord = new Record(existingRecordSet.Schema);

                    for (int i = 0; i < existingRecordSet.Schema.Fields.Count; i++)
                    {
                        Field field = existingRecordSet.Schema.Fields[i];
                        int requestFieldPosition = requestRecordSet.Schema.Fields.IndexOf(field);

                        if (requestFieldPosition != -1)
                        {
                            newRecord[i] = requestRecord[requestFieldPosition];
                        }
                    }

                    existingRecordSet.Add(newRecord);
                }
            }

            return existingRecordSet;
        }

        public static RecordSet DeleteRecords(RecordSet existingRecordSet, RecordSet requestRecordSet)
        {
            string keyFieldName = (from f in existingRecordSet.Schema.Fields
                                   where f.IsKey == true
                                   select f.Name).FirstOrDefault();

            foreach (var recordToDelete in requestRecordSet)
            {
                var recordToRemove = (from r in existingRecordSet
                                      where r[keyFieldName].Equals(recordToDelete[keyFieldName])
                                      select r).FirstOrDefault();

                if (recordToRemove != null)
                {
                    existingRecordSet.Remove(recordToRemove);
                }
            }

            return existingRecordSet;
        }
    }
}
