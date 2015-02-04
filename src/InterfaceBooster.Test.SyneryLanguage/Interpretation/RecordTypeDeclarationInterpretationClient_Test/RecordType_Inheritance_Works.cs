using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.RecordTypeDeclarationInterpretationClient_Test
{
    [TestFixture]
    class RecordType_Inheritance_Works
    {
        private RecordTypeDeclarationInterpretationClient _Client;
        private ISyneryMemory _SyneryMemory;
        private string _Code;
        private string _CodeOne;
        private string _CodeTwo;
        private IDictionary<string, string> _IncludeCode;

        /*
         * INHERITANCE ORDER:
         * 
         *  - Food(Weight)<1>
         *      - Meal(IsEatenCold)<1>
         *          - Cheese(Year)<1>
         *          - Meat(Animal)<0>
         *      - Beverage(Liter)<2>
         *          - Water(IsSparkling)<2>
         *          - Wine(Year, CountryOfOrigin)<2>
         *              - WhiteWine<0>
         *              - RoseWine<0>
         *              - RedWine<0>
         */

        [SetUp]
        public void SetupTest()
        {
            _SyneryMemory = new SyneryMemory(null, null, null);
            _Client = new RecordTypeDeclarationInterpretationClient(_SyneryMemory);
            _Code = @"
#Meat(STRING Animal) : #One.Meal;
#WhiteWine() : #Two.Wine;
#RoseWine() : #Two.Wine;
#RedWine() : #Two.Wine;
";

            _CodeOne = @"
#Cheese(INT Year) : #Meal;
#Food(INT Weight);
#Meal(BOOL IsEatenCold) : #Food;
";

            _CodeTwo = @"
#Beverage(DOUBLE Liter) : #One.Food;
#Water(BOOL IsSparkling) : #Beverage;
#Wine(INT Year, STRING CountryOfOrigin) : #Beverage;
";

            _IncludeCode = new Dictionary<string, string>();
            _IncludeCode.Add("One", _CodeOne);
            _IncludeCode.Add("Two", _CodeTwo);
        }

        [Test]
        public void Loading_RecordTypes_Works()
        {
            _Client.Run(_Code, _IncludeCode);

            Assert.AreEqual(10, _SyneryMemory.RecordTypes.Count);
        }

        [Test]
        public void Inheritance_Works_If_Base_Is_Defined_After_Child_Type()
        {
            _Client.Run(_Code, _IncludeCode);

            IRecordType type = (from r in _SyneryMemory.RecordTypes
                                where r.Value.FullName == "One.Cheese"
                                select r.Value).FirstOrDefault();

            Assert.AreEqual("One.Meal", type.BaseRecordType.FullName);
        }

        [Test]
        public void Loading_All_Fields_Works_Over_Multiple_Levels_And_Code_Files()
        {
            _Client.Run(_Code, _IncludeCode);

            IRecordType type = (from r in _SyneryMemory.RecordTypes
                                where r.Value.FullName == "WhiteWine"
                                select r.Value).FirstOrDefault();

            Assert.AreEqual(4, type.Fields.Count);
            Assert.AreEqual(true, type.Fields.Select(f => f.Name).Contains("Year"));
            Assert.AreEqual(true, type.Fields.Select(f => f.Name).Contains("CountryOfOrigin"));
            Assert.AreEqual(true, type.Fields.Select(f => f.Name).Contains("Liter"));
            Assert.AreEqual(true, type.Fields.Select(f => f.Name).Contains("Weight"));
        }
    }
}
