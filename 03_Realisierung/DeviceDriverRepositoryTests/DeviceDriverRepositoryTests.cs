using System.Collections.Generic;
using Akomi.InformationModel.Component.Identification;
using Akomi.InformationModel.Device;
using Akomi.InformationModel.Skills.SkillCatalogue;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tapako.DeviceInformationManagement.InformationSources;
using Tapako.Repositories.DeviceDriverRepository;

namespace DeviceDriverRepositoryTests
{
    [TestClass()]
    [DeploymentItem(@"TestDaten/MacRepository.txt")]
    [DeploymentItem(@"DriverRepository/BeckhoffPlcDriver.dll", "TestRepositoryFolder")]
    public class DeviceDriverRepositoryTests
    {
        private DeviceDriverRepository _sut;

        [TestInitialize]
        public void Init()
        {
            _sut = new DeviceDriverRepository();
            _sut.RepositoryFolder = "TestRepositoryFolder";
            _sut.MacListRepository = MacListRepository.GetInstance("MacRepository.txt");
        }

        [TestMethod()]
        public void GetArrayOfPlcSearchDriverPathsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void LoadPlcSearchDriverTest()
        {
            var beckhoffDeivce = new DeviceBase();
            beckhoffDeivce.Identification = new Identification();
            beckhoffDeivce.Identification.PhysicalAddress = TestConstants.AkomiSpsMac;
            var completeDevice = _sut.GetDeviceInformations(beckhoffDeivce);
            Assert.IsNotNull(completeDevice.Skills.GetSkill<SkillSearchForSubdevicesBase>());
            //Assert.IsNotNull(completeDevice.SearchForSubDevices);
        }

        [TestMethod()]
        public void LoadCommunicationChannelDriverTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetDeviceInformationTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetDeviceInformationWithMacAddressTest()
        {
            Assert.Fail();
        }

        [TestMethod]
        [Timeout(1000)]
        public void ReplaceIllegalCharactersTest()
        {
            List<string> testPaths = new List<string>()
            {
                "MLV41-8-H-500-RT-IO/65b/92/136",
                @"?:!\/- "
            };
            List<string> testResults = new List<string>()
            {
                "MLV41_8_H_500_RT_IO_65b_92_136",
                @"_______"

            };
            string result;
            for (int i = 0; i < testPaths.Count; i++)
            {
                result = RepositoryBase.ReplaceIllegalCharacters(testPaths[i]);
                Assert.AreEqual(result, testResults[i]);
            }

            Assert.AreNotEqual(testPaths[0], testResults[0]);

            result = RepositoryBase.ReplaceIllegalCharacters(testPaths[0]);
            Assert.AreNotEqual(result, testPaths[0]);

        }

        [TestMethod]
        public void LoadDeviceDriverShallNotCrashWithEmptyDevice()
        {
            DeviceBase device = new DeviceBase();

            _sut.MacListRepository.RepositoryName = "MacRepository.txt";

            var driver = _sut.GetDeviceInformations(device) as IDevice;

        }

        [TestMethod]
        public void LoadDeviceDriverShallNotCrashWithEmptyDeviceAndWithoutMacListRepository()
        {
            DeviceBase device = new DeviceBase();

            var driver = _sut.GetDeviceInformations(device) as IDevice;

        }

        [TestMethod]
        public void LoadDeviceDriverShallNotCrashWithoutPhysicalAddress()
        {
            DeviceBase device = new DeviceBase();

            device.Identification = new Identification();
            device.Description.ClassName = "TestModel";
            device.Identification.SerialNumber = "anyNumber";

            _sut.MacListRepository.RepositoryName = "MacRepository.txt";


            var driver = _sut.GetDeviceInformations(device) as IDevice;

        }

        [TestMethod]
        public void LoadDeviceDriverShallNotCrashWithoutSerialNumber()
        {
            DeviceBase device = new DeviceBase();

            device.Identification = new Identification();
            device.Description.ClassName = "TestModel";
            device.Identification.PhysicalAddress = "00000000";

            _sut.MacListRepository.RepositoryName = "MacRepository.txt";
            var driver = _sut.GetDeviceInformations(device) as IDevice;

        }

    }
}