using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Validation
{
    public interface IValidationResultMessage
    {
        ValidationResultMessageCategoryEnum Category { get; set; }
        int DefaultOrder { get; set; }
        string Message { get; set; }
        int Line { get; set; }
        int Column { get; set; }
        string CodeFilePath { get; set; }
    }
}
