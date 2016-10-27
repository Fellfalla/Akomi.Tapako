using System.Net.NetworkInformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacListRepositoryTests
{
    [TestClass()]
    [DeploymentItem(@"TestDaten/MacRepository.txt")]
    public class MacListRepositoryTests
    {

        private MacListRepository.MacListRepository _sut; // System Under Test
        private readonly string _testMacAddressString = "08002700FC78";
        private readonly PhysicalAddress _testMacAddress = PhysicalAddress.Parse("08002700FC78");
        private readonly string _testDllName = "Test";
        [TestInitialize] // wird vor jedem Test aufgrufen
        public void Init()
        {
            _sut = MacListRepository.MacListRepository.GetInstance(TestConstants.MAC_REPOSITORY);
        }

        [TestCleanup] // wird nach jedem Test aufgerufen
        public void Cleanup()
        {
            _sut = null;
        }

        [TestMethod]
        [Timeout(400)]
        [Owner("Markus")]
        public void TestMacRepositoryReadFromMacString()
        {
            string actual = _sut.GetDllName(_testMacAddressString);
            Assert.AreEqual(_testDllName, actual);
        }

        [TestMethod]
        [Owner("Markus")]
        [Timeout(1000)]
        public void TestMacRepositoryReadFromMac()
        {
            string actual = _sut.GetDllName(_testMacAddress);
            Assert.AreEqual(_testDllName, actual);
        }
    

        [TestMethod()]
        public void MacListRepositoryTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void MacListRepositoryTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetInstanceTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetDllNameTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetDllNameTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetDeviceInformationTest()
        {
            Assert.Fail();
        }
    }
}