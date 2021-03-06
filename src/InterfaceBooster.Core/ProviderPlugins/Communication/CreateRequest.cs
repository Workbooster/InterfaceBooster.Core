﻿using InterfaceBooster.ProviderPluginApi.Communication;
using InterfaceBooster.ProviderPluginApi.Data;
using InterfaceBooster.ProviderPluginApi.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Core.ProviderPlugins.Communication
{
    public class CreateRequest : Request, ICreateRequest
    {
        #region PROPERTIES

        public AnswerList Answers { get; set; }
        public RecordSet RecordSet { get; set; }
        new public CreateResource Resource { get; set; }

        #endregion

        #region PUBLIC METHODS

        public CreateRequest() : base(RequestTypeEnum.Create) { }

        #endregion
    }
}
