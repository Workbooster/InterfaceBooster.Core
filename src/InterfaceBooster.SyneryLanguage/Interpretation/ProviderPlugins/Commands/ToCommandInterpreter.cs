﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Interpretation.ProviderPlugins.Commands
{
    public class ToCommandInterpreter : IInterpreter<SyneryParser.ToCommandContext, string>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public string RunWithResult(SyneryParser.ToCommandContext context)
        {
            return context.tableIdentifier().GetText();
        }

        #endregion
    }
}
