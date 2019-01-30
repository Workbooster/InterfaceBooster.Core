using InterfaceBooster.ProviderPluginApi.Common;
using InterfaceBooster.ProviderPluginApi.Communication;
using InterfaceBooster.ProviderPluginApi.Data;
using InterfaceBooster.ProviderPluginApi.Data.Filter;
using InterfaceBooster.ProviderPluginApi.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Test.Dummy.ProviderPluginDummy.V1.Endpoints
{
    public class ArticleEndpoint : IReadEndpoint, ICreateEndpoint, IUpdateEndpoint, ISaveEndpoint, IDeleteEndpoint
    {
        #region MEMBERS

        private DummyData _Data;

        #endregion

        #region PROPERTIES

        public LocalizedText Description
        {
            get { return "Represents an article from an ERP system"; }
        }

        public string Name
        {
            get { return "Articles"; }
        }

        public string[] Path
        {
            get { return new string[] { "Tables", "LAG", }; }
        }

        #endregion

        #region PUBLIC METHODS

        public ArticleEndpoint(DummyData data)
        {
            _Data = data;
        }

        #region READ

        public ReadResource GetReadResource()
        {
            return new ReadResource()
            {
                Schema = _Data.ArticleSchema,
                SubResources = new List<Resource>()
                {
                    new ReadResource() {
                        Name = "Manufacturer",
                        Schema = _Data.ManufacturerSchema,
                    }
                },
                FilterDefinitions = new List<FilterDefinition>()
                {
                    new FilterDefinition("ArticleNumber", typeof(string), new FilterTypeEnum[] { FilterTypeEnum.Equal })
                },
            };
        }

        public ReadResponse RunReadRequest(IReadRequest request)
        {
            var responseRecordSet = _Data.ArticleRecordSet;
            var articleNumberFilter = request.Filters as SingleValueFilterCondition;

            if (articleNumberFilter!= null
                && articleNumberFilter.Definition.Name == "ArticleNumber")
            {
                responseRecordSet = new RecordSet(responseRecordSet.Schema, responseRecordSet.Where(r => articleNumberFilter.Value.ToString().Equals(r["ArticleNumber"])));
            }

            var response = new ReadResponse(request)
            {
                RecordSet = DataHelper.RemoveUnrequestedFields(responseRecordSet, request.RequestedFields)
            };
            
            // MANUFACTURER SUB-REQUEST

            IReadRequest manufacturerRequest = request.SubRequests.FindRequestByResourceName<IReadRequest>("Manufacturer");

            if (manufacturerRequest != null)
            {
                ReadResponse subResponse = new ReadResponse(manufacturerRequest)
                {
                    RecordSet = DataHelper.RemoveUnrequestedFields(_Data.ManufacturerRecordSet, manufacturerRequest.RequestedFields)
                };

                response.SubResponses.Add(subResponse);
            }

            return response;
        }

        #endregion

        #region CREATE

        public CreateResource GetCreateResource()
        {
            return new CreateResource()
            {
                Schema = _Data.ArticleSchema,
                SubResources = new List<Resource>()
                {
                    /*
                    new SaveResource() {
                        Name = "Manufacturer",
                        Schema = _Data.ManufacturerSchema,
                    }*/
                }
            };
        }

        public CreateResponse RunCreateRequest(ICreateRequest request)
        {
            _Data.ArticleRecordSet = DataHelper.CreateRecords(_Data.ArticleRecordSet, request.RecordSet);

            return new CreateResponse(request);
        }

        #endregion

        #region UPDATE

        public UpdateResource GetUpdateResource()
        {
            return new UpdateResource()
            {
                Schema = _Data.ArticleSchema,
                SubResources = new List<Resource>()
                {
                    /*
                    new SaveResource() {
                        Name = "Manufacturer",
                        Schema = _Data.ManufacturerSchema,
                    }*/
                }
            };
        }

        public UpdateResponse RunUpdateRequest(IUpdateRequest request)
        {
            _Data.ArticleRecordSet = DataHelper.UpdateRecords(_Data.ArticleRecordSet, request.RecordSet);

            return new UpdateResponse(request);
        }

        #endregion

        #region SAVE

        public SaveResource GetSaveResource()
        {
            return new SaveResource()
            {
                Schema = _Data.ArticleSchema,
                SubResources = new List<Resource>()
                {
                    /*
                    new SaveResource() {
                        Name = "Manufacturer",
                        Schema = _Data.ManufacturerSchema,
                    }*/
                }
            };
        }

        public SaveResponse RunSaveRequest(ISaveRequest request)
        {
            _Data.ArticleRecordSet = DataHelper.SaveRecords(_Data.ArticleRecordSet, request.RecordSet);

            return new SaveResponse(request);
        }

        #endregion

        #region DELETE

        public DeleteResource GetDeleteResource()
        {
            return new DeleteResource()
            {
                Schema = _Data.ArticleDeleteSchema,
                SubResources = new List<Resource>() { }
            };
        }

        public DeleteResponse RunDeleteRequest(IDeleteRequest request)
        {
            _Data.ArticleRecordSet = DataHelper.DeleteRecords(_Data.ArticleRecordSet, request.RecordSet);

            return new DeleteResponse(request);
        }

        #endregion

        #endregion
    }
}
