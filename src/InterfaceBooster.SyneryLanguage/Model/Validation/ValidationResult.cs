using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Validation;

namespace InterfaceBooster.SyneryLanguage.Model.Validation
{
    public class ValidationResult : IValidationResult
    {
        #region MEMBERS

        private IList<IValidationResultMessage> _Messages;

        #endregion

        #region PROPERTIES

        public bool IsValid { get; set; }
        public IEnumerable<IValidationResultMessage> Messages
        {
            get
            {
                return _Messages.AsEnumerable();
            }
        }

        #endregion

        #region PUBLIC METHODS

        public ValidationResult()
        {
            _Messages = new List<IValidationResultMessage>();
        }

        public void AddMessage(IValidationResultMessage validationMessage)
        {
            validationMessage.DefaultOrder = _Messages.Count + 1;
            _Messages.Add(validationMessage);
        }

        public void AddMessage(ValidationResultMessageCategoryEnum category, string message, int line, int column, string codeFilePath)
        {
            IValidationResultMessage validationMessage = new ValidationResultMessage()
            {
                DefaultOrder = _Messages.Count + 1,
                Category = category,
                Message = message,
                Line = line,
                Column = column,
                CodeFilePath = codeFilePath,
            };

            _Messages.Add(validationMessage);
        }

        #endregion
    }
}
