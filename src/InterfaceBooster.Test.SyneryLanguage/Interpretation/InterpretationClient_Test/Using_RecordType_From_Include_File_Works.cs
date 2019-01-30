using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Test.SyneryLanguage.Interpretation.BaseLanguage;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.InterpretationClient_Test
{
    [TestFixture]
    public class Using_RecordType_From_Include_File_Works : BaseTest
    {
        [Test]
        public void Using_A_RecordType_Declared_In_An_IncludeFile_Workgs()
        {
            string includeFileCode = @"
#MyFirstRecord(INT Id, STRING Name);
";
            string code = @"
#file01.MyFirstRecord rec = #file01.MyFirstRecord(15, ""Bla"");

INT intValue = rec.Id;
STRING stringValue = rec.Name;
";

            // prepare a list of include files
            Dictionary<string, string> includeFiles = new Dictionary<string, string>();
            includeFiles.Add("empty01", "// nothing to include...");
            includeFiles.Add("file01", includeFileCode);
            includeFiles.Add("empty02", "// nothing to include...");

            _SyneryClient.Run(code, includeFiles);

            IValue intVar = _SyneryClient.Memory.CurrentScope.ResolveVariable("intValue");
            IValue stringVar = _SyneryClient.Memory.CurrentScope.ResolveVariable("stringValue");

            Assert.AreEqual(15, intVar.Value);
            Assert.AreEqual("Bla", stringVar.Value);
        }

        [Test]
        public void Getting_RecordType_Instance_From_An_IncludeFile_Works()
        {
            string includeFileCode = @"
#MyFirstRecord(INT Id, STRING Name);

#MyFirstRecord GetRecord()
    #MyFirstRecord rec = #MyFirstRecord(15, ""Bla"");
    RETURN rec;
END
";
            string code = @"
#file01.MyFirstRecord rec = file01.GetRecord();

INT intValue = rec.Id;
STRING stringValue = rec.Name;
";

            // prepare a list of include files
            Dictionary<string, string> includeFiles = new Dictionary<string, string>();
            includeFiles.Add("empty01", "// nothing to include...");
            includeFiles.Add("file01", includeFileCode);
            includeFiles.Add("empty02", "// nothing to include...");

            _SyneryClient.Run(code, includeFiles);

            IValue intVar = _SyneryClient.Memory.CurrentScope.ResolveVariable("intValue");
            IValue stringVar = _SyneryClient.Memory.CurrentScope.ResolveVariable("stringValue");

            Assert.AreEqual(15, intVar.Value);
            Assert.AreEqual("Bla", stringVar.Value);
        }

        [Test]
        public void Inherit_RecordType_From_An_IncludeFile_Works()
        {
            string includeFileCode = @"
#Person(INT Id, STRING Firstname, STRING Lastname);
";
            string code = @"
#Employee(STRING Title) : #file01.Person;

#Employee rec = #Employee(Id = 15, Title = ""Bla"");

INT intValue = rec.Id;
STRING stringValue = rec.Title;
";

            // prepare a list of include files
            Dictionary<string, string> includeFiles = new Dictionary<string, string>();
            includeFiles.Add("empty01", "// nothing to include...");
            includeFiles.Add("file01", includeFileCode);
            includeFiles.Add("empty02", "// nothing to include...");

            _SyneryClient.Run(code, includeFiles);

            IValue intVar = _SyneryClient.Memory.CurrentScope.ResolveVariable("intValue");
            IValue stringVar = _SyneryClient.Memory.CurrentScope.ResolveVariable("stringValue");

            Assert.AreEqual(15, intVar.Value);
            Assert.AreEqual("Bla", stringVar.Value);
        }

        [Test]
        public void Using_An_Inherited_RecordType_Declared_In_An_IncludeFile_Workgs()
        {
            string includeFileCode = @"
#Person(INT Id, STRING Firstname, STRING Lastname);
#Employee(STRING Title) : #Person;
";
            string code = @"
#file01.Employee rec = #file01.Employee(Id = 15, Title = ""Bla"");

INT intValue = rec.Id;
STRING stringValue = rec.Title;
";

            // prepare a list of include files
            Dictionary<string, string> includeFiles = new Dictionary<string, string>();
            includeFiles.Add("empty01", "// nothing to include...");
            includeFiles.Add("file01", includeFileCode);
            includeFiles.Add("empty02", "// nothing to include...");

            _SyneryClient.Run(code, includeFiles);

            IValue intVar = _SyneryClient.Memory.CurrentScope.ResolveVariable("intValue");
            IValue stringVar = _SyneryClient.Memory.CurrentScope.ResolveVariable("stringValue");

            Assert.AreEqual(15, intVar.Value);
            Assert.AreEqual("Bla", stringVar.Value);
        }

        [Test]
        public void Calling_Function_With_Instance_Of_RecordType_Declared_In_An_IncludeFile_Works()
        {
            string includeFileCode = @"
#Employee(INT Id, STRING Firstname, STRING Lastname, STRING Title);

INT GetId(#Employee emp)
    RETURN emp.Id;
END
";
            string code = @"
#file01.Employee rec = #file01.Employee(Id = 15, Title = ""Bla"");

INT intValue = file01.GetId(rec);
";

            // prepare a list of include files
            Dictionary<string, string> includeFiles = new Dictionary<string, string>();
            includeFiles.Add("empty01", "// nothing to include...");
            includeFiles.Add("file01", includeFileCode);
            includeFiles.Add("empty02", "// nothing to include...");

            _SyneryClient.Run(code, includeFiles);

            IValue intVar = _SyneryClient.Memory.CurrentScope.ResolveVariable("intValue");

            Assert.AreEqual(15, intVar.Value);
        }

        [Test]
        public void Calling_Function_With_Instance_Of_Inherited_RecordType_Declared_In_An_IncludeFile_Works()
        {
            string includeFileCode = @"
#Person(INT Id, STRING Firstname, STRING Lastname);
#Employee(STRING Title) : #Person;

INT GetId(#Employee emp)
    RETURN emp.Id;
END
";
            string code = @"
#file01.Employee rec = #file01.Employee(Id = 15, Title = ""Bla"");

INT intValue = file01.GetId(rec);
";

            // prepare a list of include files
            Dictionary<string, string> includeFiles = new Dictionary<string, string>();
            includeFiles.Add("empty01", "// nothing to include...");
            includeFiles.Add("file01", includeFileCode);
            includeFiles.Add("empty02", "// nothing to include...");

            _SyneryClient.Run(code, includeFiles);

            IValue intVar = _SyneryClient.Memory.CurrentScope.ResolveVariable("intValue");

            Assert.AreEqual(15, intVar.Value);
        }

        [Test]
        public void Calling_Function_With_Instance_Of_Base_RecordType_Declared_In_An_IncludeFile_Works()
        {
            string includeFileCode = @"
#Person(INT Id, STRING Firstname, STRING Lastname);
#Employee(STRING Title) : #Person;

INT GetId(#Person p)
    RETURN p.Id;
END
";
            string code = @"
#file01.Employee rec = #file01.Employee(Id = 15, Title = ""Bla"");

INT intValue = file01.GetId(rec);
";

            // prepare a list of include files
            Dictionary<string, string> includeFiles = new Dictionary<string, string>();
            includeFiles.Add("empty01", "// nothing to include...");
            includeFiles.Add("file01", includeFileCode);
            includeFiles.Add("empty02", "// nothing to include...");

            _SyneryClient.Run(code, includeFiles);

            IValue intVar = _SyneryClient.Memory.CurrentScope.ResolveVariable("intValue");

            Assert.AreEqual(15, intVar.Value);
        }

        [Test]
        public void Calling_Function_With_Instance_Of_Inherited_RecordType_Declared_Based_On_RecordType_From_IncludeFile_Works()
        {
            string includeFileCode = @"
#Person(INT Id, STRING Firstname, STRING Lastname);

INT GetId(#Person p)
    RETURN p.Id;
END
";
            string code = @"
#Employee(STRING Title) : #file01.Person;
#Employee rec = #Employee(Id = 15, Title = ""Bla"");

INT intValue = file01.GetId(rec);
";

            // prepare a list of include files
            Dictionary<string, string> includeFiles = new Dictionary<string, string>();
            includeFiles.Add("empty01", "// nothing to include...");
            includeFiles.Add("file01", includeFileCode);
            includeFiles.Add("empty02", "// nothing to include...");

            _SyneryClient.Run(code, includeFiles);

            IValue intVar = _SyneryClient.Memory.CurrentScope.ResolveVariable("intValue");

            Assert.AreEqual(15, intVar.Value);
        }

        [Test]
        public void Get_Inherited_RecordType_Instance_As_Function_Return_Declared_In_An_IncludeFile_Works()
        {
            string includeFileCode = @"
#Person(INT Id, STRING Firstname, STRING Lastname);
#Employee(STRING Title) : #Person;

#Employee GetEmployee(INT id)
    RETURN #Employee(Id = id, Title = ""Bla"");
END
";
            string code = @"
#file01.Employee rec = file01.GetEmployee(15);

INT intValue = rec.Id;
STRING stringValue = rec.Title;
";

            // prepare a list of include files
            Dictionary<string, string> includeFiles = new Dictionary<string, string>();
            includeFiles.Add("empty01", "// nothing to include...");
            includeFiles.Add("file01", includeFileCode);
            includeFiles.Add("empty02", "// nothing to include...");

            _SyneryClient.Run(code, includeFiles);

            IValue intVar = _SyneryClient.Memory.CurrentScope.ResolveVariable("intValue");
            IValue stringVar = _SyneryClient.Memory.CurrentScope.ResolveVariable("stringValue");

            Assert.AreEqual(15, intVar.Value);
            Assert.AreEqual("Bla", stringVar.Value);
        }
    }
}
