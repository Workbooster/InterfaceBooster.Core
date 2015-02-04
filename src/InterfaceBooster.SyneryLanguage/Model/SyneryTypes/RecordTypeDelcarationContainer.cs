using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.SyneryLanguage.Model.SyneryTypes
{
    public class RecordTypeDelcarationContainer
    {
        public string Name { get; set; }
        public string CodeFileAlias { get; set; }
        public string FullName
        {
            get
            {
                // check whether an alias is available -> return CodeFileAlias.Name
                if (!string.IsNullOrEmpty(CodeFileAlias))
                    return String.Format("{0}.{1}", CodeFileAlias, Name);

                // no alias -> only return Name
                return Name;
            }
        }
        public string BaseRecordFullName { get; set; }
        public SyneryParser.RecordTypeDeclarationContext RecordTypeDeclarationContext { get; set; }
    }
}
