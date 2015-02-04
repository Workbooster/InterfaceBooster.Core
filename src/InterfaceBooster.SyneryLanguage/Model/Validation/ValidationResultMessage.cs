using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Validation;

namespace InterfaceBooster.SyneryLanguage.Model.Validation
{
    public class ValidationResultMessage : IValidationResultMessage
    {
        public ValidationResultMessageCategoryEnum Category { get; set; }
        public int DefaultOrder { get; set; }
        public string Message { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
        public string CodeFilePath { get; set; }
    }
}
