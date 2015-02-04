using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Validation
{
    public interface IValidationResult
    {
        bool IsValid { get; set; }
        IEnumerable<IValidationResultMessage> Messages { get; }

        void AddMessage(IValidationResultMessage message);
        void AddMessage(ValidationResultMessageCategoryEnum category, string message, int line, int column, string codeFilePath);
    }
}
