using System;
using Akomi.Logger;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tapako.Framework;

namespace Implementationstests
{
    /// <summary>
    /// Nice to now: 
    /// Internals werden durch [assembly: InternalsVisible("TestProjectName")] in AssemblyInfo.cs des zu Testenden Projects sichtbar
    /// </summary>
    [TestClass]
    public class MessageTest
    {
        private readonly string _informationString = "test";
        private Message _sut; // System Under Test
        private const int FixedYear = 2015;
        private const int FixedMonth = 1;
        private const int FixedDay = 1;
        private const int FixedHour = 1;
        private const int FixedMinute = 1;
        private const int FixedSecond = 1;

        [TestInitialize] // wird vor jedem Test aufgrufen
        public void Init()  
        {
            //using (ShimsContext.Create())
            //{
            //    //todo: Shim reparieren
            //    //// Arrange:
            //    //// Shim DateTime.Now to return a fixed date:
            //    //System.Fakes.ShimDateTime.NowGet =
            //    //() =>
            //    //{ return new DateTime(FixedYear, FixedMonth, FixedDay, FixedHour, FixedMinute, FixedSecond); };
            //    //// Instantiate the component under test:
            //    //_sut = new Message(_informationString, Category.Info);

            //}

        }

        [TestCleanup] // wird nach jedem Test aufgerufen
        public void Cleanup()   
        {
            _sut = null;
        }


        [TestMethod]
        [Owner("Markus")]
        [Ignore]
        public void MessageInformationStoredToValues()
        {
            Assert.AreEqual(_sut.Value, _informationString);
        }

        [TestMethod]
        [Owner("Markus")]
        [Ignore]
        public void MessageDateIsNotNull()
        {
            Assert.AreNotEqual(_sut.TimestampDateTime, null);
        }

        [TestMethod]
        [Owner("Markus")]
        [Ignore]
        public void MessageDateTimestampProperty()
        {
            var accessor = new PrivateObject(_sut);
            DateTime fixedDateTime = new DateTime(FixedYear, FixedMonth, FixedDay, FixedHour, FixedMinute, FixedSecond);

            accessor.SetProperty(FunctionCollection.GetPropertyInfo(_sut, u => u.TimestampDateTime).Name, fixedDateTime);

            Assert.AreEqual(fixedDateTime, _sut.TimestampDateTime);
        }
    }



}
