using System;
using System.Linq;
using System.Net;
using Akomi.InformationModel.Device;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tapako.Utilities.UniversalHostSearch;


namespace UniversalHostSearchTests1
{
    [TestClass]
    public class UniversalHostSearcherTest
    {
        private string subnetWithDot = "192.168.1.";
        private string subnetWithoutDot = "192.168.1";
        private string ownIpAddressString = "192.168.1.3";
        private readonly IPAddress _ownIpAddress = IPAddress.Parse("192.168.1.3");

        [TestInitialize]
        public void Init()
        {
        }

        [TestCleanup]
        public void Cleanup()
        {
        }

        [TestMethod]
        [Timeout(30000)]
        public void UniversualHostSearcherSearch()
        {
            //const string input = "10.0.0.";
            const string input = "10.176.9.";
            var result = UniversalHostSearcher.SearchForSubSystems(input).ToList();
            
           
            foreach (IDevice device in result)
            {
                if (device.Identification.IpAddress == "10.0.0.30" )
                //if (Device.Name=="AKOMI") // GetHostName geht nicht immer
                {
                    Console.WriteLine("Da ist das Ding");
               }
            }
           result.ForEach(Console.WriteLine);


        }

        [TestMethod]
        [Timeout(500)]
        public void UniversalHostSearcherShouldNotCrashWithWrongInput()
        {
            string wrongInput = "idiotic input";
            UniversalHostSearcher.SearchForSubSystems(wrongInput).ToList().ForEach(Console.WriteLine);
        }

        [TestMethod]
        [Timeout(500)]
        public void UniversalHostSearcherShouldNotCrashWithEmptyInput()
        {
            string emptyInput = "";
            UniversalHostSearcher.SearchForSubSystems(emptyInput).ToList().ForEach(Console.WriteLine); 
        }


        [TestMethod]
        [Timeout(500)]
        public void UniversalHostSearcherShouldTakeOwnSubnetWithoutParameter()
        {

            Assert.AreEqual(UniversalHostSearcher.SearchForSubSystems(ownIpAddressString),
                UniversalHostSearcher.SearchForSubSystems());
        }

        [TestMethod]
        [Timeout(500)]
        public void UniversalHostSearcherCanParseSubnetStrings()
        {

            var accessor = new PrivateType(typeof(UniversalHostSearcher));

            Assert.AreEqual(subnetWithDot, accessor.InvokeStatic("ParseSubnetString", subnetWithoutDot));
            Assert.AreEqual(subnetWithDot, accessor.InvokeStatic("ParseSubnetString", ownIpAddressString));
        }

        [TestMethod]
        [Timeout(500)]
        public void UniversalHostSearcherShouldFindOwnIpAddress()
        {

            //using (ShimsContext.Create())
            //{


            //        // Arrange:
            //        //StubDns.GetHostAdresses =
            //        //() =>
            //        //{ return Dns.GetHostAddresses(Dns.GetHostName()); ; };
            //        throw new NotImplementedException();
            //    // Instantiate the component under test:

            //}


            var accessor = new PrivateType(typeof(UniversalHostSearcher));

            Assert.AreEqual(_ownIpAddress, accessor.InvokeStatic("GetOwnIpAddress"));
        }

    }



}
