using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Common;
using InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Records;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.SyneryLanguage.Model.SyneryTypes;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;

namespace InterfaceBooster.SyneryLanguage.Interpretation
{
    public class RecordTypeDeclarationInterpretationClient : ISyneryClient<bool>
    {
        #region PROPERTIES

        public IInterpretationController Controller { get; private set; }

        public ISyneryMemory Memory { get; set; }

        public IAntlrErrorListener<int> LexerErrorListener { get; set; }
        
        public IAntlrErrorListener<IToken> ParserErrorListener { get; set; }

        #endregion

        #region PUBLIC METHODS

        public RecordTypeDeclarationInterpretationClient(ISyneryMemory memory, IAntlrErrorListener<int> lexerErrorListener = null, IAntlrErrorListener<IToken> parserErrorListener = null)
        {
            Memory = memory;
            LexerErrorListener = lexerErrorListener;
            ParserErrorListener = parserErrorListener;

            IInterpreterFactory factory = InterpreterFactory.GetDefaultInterpreterFactory();
            factory.SetInterpreter(new RecordTypeDeclarationInterpreter());

            Controller = new InterpretationController(factory, memory);
        }

        public bool Run(string code, IDictionary<string, string> includeFiles = null)
        {
            ExtractRecordTypes(code, includeFiles);

            return true;
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region INTERNAL METHODS

        private void ExtractRecordTypes(string code, IDictionary<string, string> includeCode = null)
        {
            int endlessLoopPreventionCounter = 0;
            IList<RecordTypeDelcarationContainer> listOfDeclrartionContainer = GetRecordTypeContexts(code, includeCode);

            // assure that there is a dictionary
            if (Memory.RecordTypes == null)
                Memory.RecordTypes = new Dictionary<SyneryType, IRecordType>();

            // extract all RecordTypes without a base type

            var listOfDeclarationsWithoutBaseType = (from d in listOfDeclrartionContainer
                                                     where d.BaseRecordFullName == null
                                                     select d).ToList();

            foreach (var item in listOfDeclarationsWithoutBaseType)
            {
                IRecordType recordType = Controller
                    .Interpret<SyneryParser.RecordTypeDeclarationContext, IRecordType, RecordTypeDelcarationContainer>(item.RecordTypeDeclarationContext, item);

                SyneryType syneryType = new SyneryType(typeof(IRecord), recordType.FullName);

                // set the alias of the included code file
                if (item.CodeFileAlias != null)
                    recordType.CodeFileAlias = item.CodeFileAlias;

                Memory.RecordTypes.Add(syneryType, recordType);
                listOfDeclrartionContainer.Remove(item);
            }

            // loop threw the list of RecordTypes with a base type
            // assure that a RecordType only is loaded if the base type already is available

            while (listOfDeclrartionContainer.Count != 0)
            {
                var listOfAvailableTypes = Memory.RecordTypes.Select(r => r.Value.FullName);
                var listOfDeclarationsWithAvailableBaseTypes = (from d in listOfDeclrartionContainer
                                                                where listOfAvailableTypes.Contains(d.BaseRecordFullName)
                                                                select d).ToList();
                
                // extract all RecordTypes for which the base type already is available

                foreach (var item in listOfDeclarationsWithAvailableBaseTypes)
                {
                    IRecordType recordType = Controller
                        .Interpret<SyneryParser.RecordTypeDeclarationContext, IRecordType, RecordTypeDelcarationContainer>(item.RecordTypeDeclarationContext, item);

                    SyneryType syneryType = new SyneryType(typeof(IRecord), recordType.FullName);

                    // set the alias of the included code file
                    if (item.CodeFileAlias != null)
                        recordType.CodeFileAlias = item.CodeFileAlias;

                    Memory.RecordTypes.Add(syneryType, recordType);
                    listOfDeclrartionContainer.Remove(item);
                }

                // prevent an endless loop:
                // if the counter reaches the 1000-mark the loop is stopped by throwing an exception

                endlessLoopPreventionCounter++;

                if (endlessLoopPreventionCounter > 1000)
                {
                    // throw an exception that contains the name(s) of the left declarations that couldn't be resolved

                    throw new SyneryException(String.Format(
                        "The base type for the following record types couldn't be resolved: {0}",
                        String.Join(", ", listOfDeclrartionContainer.Select(d => String.Format("'{0} : {1}'", d.FullName, d.BaseRecordFullName)))));
                }
            }
        }

        /// <summary>
        /// Collects all RecordTypeDeclarationContexts from all given code files and extracts the RecordType's full name and the name of the base record.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="includeCode"></param>
        /// <returns></returns>
        private IList<RecordTypeDelcarationContainer> GetRecordTypeContexts(string code, IDictionary<string, string> includeCode = null)
        {
            List<RecordTypeDelcarationContainer> listOfRecordTypes = new List<RecordTypeDelcarationContainer>();

            // get contexts from main code
            listOfRecordTypes.AddRange(GetRecordTypeContextsFromCode(code));

            if (includeCode != null)
            {
                // get contexts from included code files

                foreach (var item in includeCode)
                {
                    listOfRecordTypes.AddRange(GetRecordTypeContextsFromCode(item.Value, item.Key));
                }
            }

            return listOfRecordTypes;
        }

        /// <summary>
        /// Gets all RecordTypeDeclarationContexts from the given code and extracts the RecordType's full name and the name of the base record.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        private IList<RecordTypeDelcarationContainer> GetRecordTypeContextsFromCode(string code, string alias = null)
        {
            IList<RecordTypeDelcarationContainer> listOfRecordTypeDeclarations = new List<RecordTypeDelcarationContainer>();

            SyneryParser.ProgramContext programContext = ParserHelper.GetProgramAstFromCode(code, LexerErrorListener, ParserErrorListener);

            if (programContext != null && programContext.recordTypeDeclaration() != null)
            {
                // loop threw all RecordType declarations and collect the needed information

                foreach (var recordTypeDeclarationContext in programContext.recordTypeDeclaration())
                {
                    if (recordTypeDeclarationContext.RecordTypeIdentifier() != null)
                    {
                        // create a container for the collected information about the declaration
                        RecordTypeDelcarationContainer container = new RecordTypeDelcarationContainer();
                        // store the context
                        container.RecordTypeDeclarationContext = recordTypeDeclarationContext;

                        // get the RecordType's name and remove the leading hashtag
                        string name = RecordHelper.ParseRecordTypeName(recordTypeDeclarationContext.RecordTypeIdentifier().GetText());

                        container.Name = name;
                        container.CodeFileAlias = alias;

                        if (recordTypeDeclarationContext.recordTypeDeclarationBaseType() != null
                            && recordTypeDeclarationContext.recordTypeDeclarationBaseType().recordType() != null)
                        {
                            // the RecordType inherits from another RecordType -> store the base RecordType's name

                            string baseTypeName = RecordHelper.ParseRecordTypeName(
                                recordTypeDeclarationContext.recordTypeDeclarationBaseType().recordType().GetText());

                            if (alias != null && baseTypeName.IndexOf('.') == -1)
                            {
                                container.BaseRecordFullName = String.Format("{0}.{1}", alias, baseTypeName);
                            }
                            else
                            {
                                container.BaseRecordFullName = baseTypeName;
                            }
                        }

                        // verify that the FullName of the current record set is unique:
                        // try to find a record type that uses the same FullName

                        RecordTypeDelcarationContainer recordTypeWithTheSameName =
                            listOfRecordTypeDeclarations.FirstOrDefault(r => r.FullName == container.FullName);

                        if (recordTypeWithTheSameName != null)
                        {
                            throw new SyneryInterpretationException(recordTypeDeclarationContext, String.Format(
                                "A record type with the name '{0}' has already been specified at line '{1}'.",
                                container.FullName,
                                recordTypeWithTheSameName.RecordTypeDeclarationContext.Start.Line));
                        }

                        // the record type FullName is unique
                        // append it to the list

                        listOfRecordTypeDeclarations.Add(container);
                    }
                    else
                    {
                        throw new SyneryInterpretationException(recordTypeDeclarationContext, "Record type may no be empty.");
                    }
                }
            }

            return listOfRecordTypeDeclarations;
        }

        #endregion
    }
}
