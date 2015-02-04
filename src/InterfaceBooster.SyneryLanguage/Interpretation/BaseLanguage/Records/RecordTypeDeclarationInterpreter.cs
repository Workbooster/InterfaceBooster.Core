using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Model.SyneryTypes;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Records
{
    public class RecordTypeDeclarationInterpreter : IInterpreter<SyneryParser.RecordTypeDeclarationContext, IRecordType, RecordTypeDelcarationContainer>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public IRecordType Run(SyneryParser.RecordTypeDeclarationContext context, RecordTypeDelcarationContainer declaration)
        {
            IRecordType recordType;
            IRecordType baseRecordType = null;

            string name = declaration.Name;
            string baseRecordFullName = declaration.BaseRecordFullName;

            if (String.IsNullOrEmpty(baseRecordFullName) == false)
            {
                baseRecordType = (from r in Memory.RecordTypes
                                  where r.Value.FullName == baseRecordFullName
                                  select r.Value).FirstOrDefault();

                if (baseRecordType == null)
                    throw new SyneryInterpretationException(context, String.Format(
                        "Base record type with name='{0}' not found while initializing the declaration for '{1}'", baseRecordFullName, name));
            }

            recordType = new RecordType(name, baseRecordType);

            if (context.parameterDeclartions() != null)
            {
                IEnumerable<IFunctionParameterDefinition> listOfParameterDefinitions = Controller
                    .Interpret<SyneryParser.ParameterDeclartionsContext, IEnumerable<IFunctionParameterDefinition>>(context.parameterDeclartions());

                foreach (var parameterDefinition in listOfParameterDefinitions)
                {
                    recordType.AddField(parameterDefinition.Name, parameterDefinition.Type, parameterDefinition.DefaultValue);
                }
            }

            return recordType;
        }

        #endregion
    }
}
