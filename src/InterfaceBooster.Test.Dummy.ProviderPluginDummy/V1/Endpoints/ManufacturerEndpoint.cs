using InterfaceBooster.ProviderPluginApi.Communication;
using InterfaceBooster.ProviderPluginApi.Data;
using InterfaceBooster.ProviderPluginApi.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Test.Dummy.ProviderPluginDummy.V1.Endpoints
{
    public class ManufacturerEndpoint : IReadEndpoint, ICreateEndpoint, IUpdateEndpoint, ISaveEndpoint, IDeleteEndpoint, IExecuteEndpoint
    {
        #region MEMBERS

        private DummyData _Data;

        #endregion

        #region PROPERTIES

        public LocalizedText Description
        {
            get { return "Represents a manufacturer record from an ERP system"; }
        }

        public string Name
        {
            get { return "Manufacturers"; }
        }

        public string[] Path
        {
            get { return new string[] { "Tables", "LAG", }; }
        }

        #endregion

        #region PUBLIC METHODS

        public ManufacturerEndpoint(DummyData data)
        {
            _Data = data;
        }

        #region READ

        public ReadResource GetReadResource()
        {
            return new ReadResource()
            {
                Schema = _Data.ManufacturerSchema,
                SubResources = new List<Resource>() { }
            };
        }

        public ReadResponse RunReadRequest(IReadRequest request)
        {
            return new ReadResponse(request)
            {
                RecordSet = DataHelper.RemoveUnrequestedFields(_Data.ManufacturerRecordSet, request.RequestedFields)
            };
        }

        #endregion

        #region CREATE

        public CreateResource GetCreateResource()
        {
            return new CreateResource()
            {
                Schema = _Data.ManufacturerSchema,
            };
        }

        public CreateResponse RunCreateRequest(ICreateRequest request)
        {
            _Data.ManufacturerRecordSet = DataHelper.CreateRecords(_Data.ManufacturerRecordSet, request.RecordSet);

            return new CreateResponse(request);
        }

        #endregion

        #region UPDATE

        public UpdateResource GetUpdateResource()
        {
            return new UpdateResource()
            {
                Schema = _Data.ManufacturerSchema,
            };
        }

        public UpdateResponse RunUpdateRequest(IUpdateRequest request)
        {
            _Data.ManufacturerRecordSet = DataHelper.UpdateRecords(_Data.ManufacturerRecordSet, request.RecordSet);

            return new UpdateResponse(request);
        }

        #endregion

        #region SAVE

        public SaveResource GetSaveResource()
        {
            return new SaveResource()
            {
                Schema = _Data.ManufacturerSchema,
            };
        }

        public SaveResponse RunSaveRequest(ISaveRequest request)
        {
            _Data.ManufacturerRecordSet = DataHelper.SaveRecords(_Data.ManufacturerRecordSet, request.RecordSet);

            return new SaveResponse(request);
        }

        #endregion

        #region DELETE

        public DeleteResource GetDeleteResource()
        {
            return new DeleteResource()
            {
                Schema = _Data.ManufacturerDeleteSchema,
            };
        }

        public DeleteResponse RunDeleteRequest(IDeleteRequest request)
        {
            _Data.ManufacturerRecordSet = DataHelper.DeleteRecords(_Data.ManufacturerRecordSet, request.RecordSet);

            return new DeleteResponse(request);
        }

        #endregion

        #region EXECUTE

        public ExecuteResource GetExecuteResource()
        {
            return new ExecuteResource()
            {
                ReturnValues = new List<ValueDefinition>() {
                    new ValueDefinition("NextManufacturerNumber", typeof(int)),
                }
            };
        }

        public ExecuteResponse RunExecuteRequest(IExecuteRequest request)
        {
            ExecuteResponse response = new ExecuteResponse(request);

            foreach (var requestValue in request.RequestedValues)
            {
                if (requestValue == "NextManufacturerNumber")
                {
                    int currentMaxNumber;

                    if (_Data.ManufacturerRecordSet.Count > 0)
                    {
                        currentMaxNumber = _Data.ManufacturerRecordSet.Max(r => (int)r["ManufacturerNumber"]);
                    }
                    else
                    {
                        currentMaxNumber = 0;
                    }

                    response.Values.Add("NextManufacturerNumber", currentMaxNumber + 1);
                }
            }

            return response;
        }

        #endregion

        #endregion
    }
}
