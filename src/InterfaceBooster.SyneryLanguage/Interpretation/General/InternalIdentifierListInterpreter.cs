using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Interpretation.General
{
    public class InternalIdentifierListInterpreter : IInterpreter<SyneryParser.InternalIdentifierListContext, IList<string>>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public IList<string> RunWithResult(SyneryParser.InternalIdentifierListContext context)
        {
            List<string> listOfIdentifiers = new List<string>();

            if (context.internalIdentifierListItem() != null)
            {
                // loop threw all identifiers and get the text

                foreach (var item in context.internalIdentifierListItem())
                {
                    listOfIdentifiers.Add(item.GetText());
                }
            }

            return listOfIdentifiers;
        }

        #endregion
    }
}
