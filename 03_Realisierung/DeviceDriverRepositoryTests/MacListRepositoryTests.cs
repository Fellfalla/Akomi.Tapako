using System.Net.NetworkInformation;
using Akomi.InformationModel.Component.Identification;
using Akomi.InformationModel.Device;
using Akomi.InformationModel.Skills.SkillCatalogue;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tapako.Repositories.DeviceDriverRepository;

namespace DeviceDriverRepositoryTests
{
    [TestClass()]
    [DeploymentItem(@"TestDaten/MacRepository.txt")]
    [DeploymentItem(@"DriverRepository/BeckhoffPlcDriver.dll")]
    public class MacListRepositoryTests
    {

        private MacListRepository _sut; // System Under Test
        private static readonly string TestMacAddressString = "08002700FC78";
        private static readonly PhysicalAddress TestMacAddress = PhysicalAddress.Parse("08002700FC78");
        private readonly string _testDllName = "Test";

        private static readonly string PlcMacAddressString = "0001051825A2";
        private static readonly PhysicalAddress PlcMacAddress = PhysicalAddress.Parse(PlcMacAddressString);
        private readonly string _plcDriverName = "BeckhoffPlcDriver.dll";


        [TestInitialize] // wird vor jedem Test aufgrufen
        public void Init()
        {
            _sut = MacListRepository.GetInstance(TestConstants.MAC_REPOSITORY);
        }

        [TestCleanup] // wird nach jedem Test aufgerufen
        public void Cleanup()
        {
            _sut = null;
        }

        [TestMethod]
        [Timeout(400)]
        [Owner("Markus")]
        public void GetDllNameFromMacString()
        {
            string actual = _sut.GetDllName(TestMacAddressString);
            Assert.AreEqual(_testDllName, actual);
        }

        [TestMethod]
        [Owner("Markus")]
        [Timeout(1000)]
        public void GetDllNameFromMac()
        {
            string actual = _sut.GetDllName(TestMacAddress);
            Assert.AreEqual(_testDllName, actual);
        }
    

        [TestMethod()]
        public void GetPlcDllNameFromPlcMac()
        {
            string actual = _sut.GetDllName(PlcMacAddress);
            Assert.AreEqual(_plcDriverName, actual);
        }

        [TestMethod()]
        public void GetDeviceInformationTest()
        {

            var beckhoffDeivce = new DeviceBase();
            beckhoffDeivce.Identification = new Identification();
            beckhoffDeivce.Identification.PhysicalAddress = (TestConstants.AkomiSpsMac);
            var completeDevice = _sut.GetDeviceInformation(beckhoffDeivce);
            Assert.IsNotNull(completeDevice.Skills.GetSkill<SkillSearchForSubdevicesBase>());
            //Assert.IsNotNull(completeDevice.SearchForSubDevices);
        }
    }
}