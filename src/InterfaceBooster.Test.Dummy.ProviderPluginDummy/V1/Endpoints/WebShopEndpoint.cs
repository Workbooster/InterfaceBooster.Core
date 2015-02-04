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
    public class WebShopEndpoint : IReadEndpoint, ICreateEndpoint, IUpdateEndpoint, IDeleteEndpoint
    {
        #region MEMBERS

        private DummyData _Data;

        #endregion

        #region PROPERTIES

        public LocalizedText Description
        {
            get { return "Represents an additional table with the prefix 'Z'."; }
        }

        public string Name
        {
            get { return "ZWebShop"; }
        }

        public string[] Path
        {
            get { return new string[] { "Tables", "AdditionalTables", }; }
        }

        #endregion

        #region PUBLIC METHODS

        public WebShopEndpoint(DummyData data)
        {
            _Data = data;
        }

        #region READ

        public ReadResource GetReadResource()
        {
            return new ReadResource()
            {
                Schema = _Data.WebShopSchema,
                SubResources = new List<Resource>() { }
            };
        }

        public ReadResponse RunReadRequest(IReadRequest request)
        {
            return new ReadResponse(request)
            {
                RecordSet = DataHelper.RemoveUnrequestedFields(_Data.WebShopRecordSet, request.RequestedFields)
            };
        }

        #endregion

        #region CREATE

        public CreateResource GetCreateResource()
        {
            return new CreateResource()
            {
                Schema = _Data.WebShopSchema,
            };
        }

        public CreateResponse RunCreateRequest(ICreateRequest request)
        {
            _Data.WebShopRecordSet = DataHelper.CreateRecords(_Data.WebShopRecordSet, request.RecordSet);

            return new CreateResponse(request);
        }

        #endregion

        #region UPDATE

        public UpdateResource GetUpdateResource()
        {
            return new UpdateResource()
            {
                Schema = _Data.WebShopSchema,
            };
        }

        public UpdateResponse RunUpdateRequest(IUpdateRequest request)
        {
            _Data.WebShopRecordSet = DataHelper.UpdateRecords(_Data.WebShopRecordSet, request.RecordSet);

            return new UpdateResponse(request);
        }

        #endregion

        #region DELETE

        public DeleteResource GetDeleteResource()
        {
            return new DeleteResource()
            {
                Schema = _Data.WebShopDeleteSchema,
            };
        }

        public DeleteResponse RunDeleteRequest(IDeleteRequest request)
        {
            _Data.WebShopRecordSet = DataHelper.DeleteRecords(_Data.WebShopRecordSet, request.RecordSet);

            return new DeleteResponse(request);
        }

        #endregion

        #endregion
    }
}
