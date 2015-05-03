using InterfaceBooster.ProviderPluginApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Test.Dummy.ProviderPluginDummy.V1
{
    public class DummyData
    {
        #region MEMBERS

        private RecordSet _ArticleRecordSet;

        private RecordSet _ManufacturerRecordSet;

        private RecordSet _WebShopRecordSet;

        #endregion

        #region PROPERTIES

        public Schema ArticleSchema
        {
            get
            {
                return _ArticleRecordSet.Schema;
            }
        }

        public Schema ArticleDeleteSchema
        {
            get
            {
                return Schema.New("Article",
                    new Field[] { 
                        new Field("ArticleNumber", typeof(string)) { IsNullable = false, IsKey = true },
                    });
            }
        }

        public Schema ManufacturerSchema
        {
            get
            {
                return _ManufacturerRecordSet.Schema;
            }
        }

        public Schema ManufacturerDeleteSchema
        {
            get
            {
                return Schema.New("Manufacturer",
                    new Field[] { 
                        new Field("ManufacturerNumber", typeof(int)) { IsNullable = false, IsKey = true },
                    });
            }
        }

        public Schema WebShopSchema
        {
            get
            {
                return _WebShopRecordSet.Schema;
            }
        }

        public Schema WebShopDeleteSchema
        {
            get
            {
                return Schema.New("ZWebShop",
                    new Field[] { 
                        new Field("ArticleNumber", typeof(string)) { IsNullable = false, IsKey = true },
                    });
            }
        }

        public RecordSet ArticleRecordSet
        {
            get { return _ArticleRecordSet; }
            set { _ArticleRecordSet = value; }
        }

        public RecordSet ManufacturerRecordSet
        {
            get { return _ManufacturerRecordSet; }
            set { _ManufacturerRecordSet = value; }
        }

        public RecordSet WebShopRecordSet
        {
            get { return _WebShopRecordSet; }
            set { _WebShopRecordSet = value; }
        }

        #endregion

        #region PUBLIC METHODS

        public DummyData()
        {
            // create dummy data

            _ArticleRecordSet =
               new RecordSet(Schema.New("Article",
                               new Field[] { 
                                    new Field("ArticleNumber", typeof(string)) { IsNullable = false, IsKey = true },
                                    Field.New<string>("Name1", false),
                                    Field.New<string>("Name2", true),
                                    Field.New<decimal>("Price1", false),
                                    Field.New<string>("UnitInternal", false),
                                    Field.New<string>("UnitInvoice", false),
                                    Field.New<int>("ManufacturerNumber", true),
                                }))
               .AppendRecord("Test01", "Test01-Name1", "Test01-Name2", 1M, "PCS", "PCS", 1)
               .AppendRecord("Test02", "Test02-Name1", "Test02-Name2", 2M, "PCS", "PCS", 2)
               .AppendRecord("Test03", "Test03-Name1", "Test03-Name2", 3M, "PCS", "PCS", 3);


            _ManufacturerRecordSet =
                new RecordSet(Schema.New("Manufacturer",
                                new Field[] { 
                                    new Field("ManufacturerNumber", typeof(int)) { IsNullable = false, IsKey = true },
                                    Field.New<string>("Company", false),
                                    Field.New<string>("Address", true),
                                    Field.New<string>("PostalCode", false),
                                    Field.New<string>("City", false),
                                    Field.New<string>("Phone", true),
                                    Field.New<string>("EMail", true),
                                }))
                    .AppendRecord(1, "Test01-Company", "Test01-Address", "Test01-PostalCode", "Test01-City", "044 111 11 11", "one@first.ch")
                    .AppendRecord(2, "Test02-Company", "Test02-Address", "Test02-PostalCode", "Test01-City", "044 222 22 22", "two@second.ch")
                    .AppendRecord(3, "Test03-Company", "Test03-Address", "Test03-PostalCode", "Test01-City", "044 333 33 33", "three@third.ch");


            _WebShopRecordSet =
                new RecordSet(Schema.New("ZWebShop",
                                new Field[] { 
                                    new Field("ArticleNumber", typeof(string)) { IsNullable = false, IsKey = true },
                                    Field.New<string>("Name", false),
                                    Field.New<string>("Description", true),
                                    Field.New<decimal>("Price", false),
                                }))
                    .AppendRecord("Test01", "Test01-Name", "Test01-Description", 1M)
                    .AppendRecord("Test02", "Test02-Name", "Test02-Description", 2M)
                    .AppendRecord("Test03", "Test03-Name", "Test03-Description", 3M);
        }


        #endregion
    }
}
