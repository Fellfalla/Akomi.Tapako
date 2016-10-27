using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tapako.Framework;

namespace Implementationstests
{
    [TestClass]
    public class FunctionCollectionTest
    {


        [TestMethod]
        [Timeout(1000)]
        public void TestGetPropertyInfo()
        {
            TestKlasse testKlasse = new TestKlasse();
            PropertyInfo excpectedPropertyInfo = typeof(TestKlasse).GetProperty("TestStringProperty");

            PropertyInfo actual = FunctionCollection.GetPropertyInfo(testKlasse, klasse => (klasse.TestStringProperty));

            Assert.AreEqual(excpectedPropertyInfo, actual);
        }


        private class TestKlasse
        {
            public string TestStringProperty { get; set; } 
        }


        [TestMethod]
        public void ReplaceIllegalPathCharactersTest()
        {
            const string orginalPath = "EL1018 8K. Dig. Eingang 24V, 10µs";
            const string probablyOutcomingPath = "EL1018_8K._Dig._Eingang_24V__10_s";
            var outcomingPath = FunctionCollection.ReplaceIllegalPathCharacters(orginalPath, Constants.ReplacingChar.ToString());

            Assert.AreEqual(probablyOutcomingPath, outcomingPath);
        }

        [TestMethod]
        public void ReplaceIllegalPathCharactersTestwithOwnInput()
        {
            var orginalPath = "EL6224 (IO Link Master)";
            var outcomingPath = FunctionCollection.ReplaceIllegalPathCharacters(orginalPath, Constants.ReplacingChar.ToString());
            Console.WriteLine(outcomingPath);
            
        }
    }
}
