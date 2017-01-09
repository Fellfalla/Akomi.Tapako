using System.Linq;
using Akomi.InformationModel.Component.Connection;
using Akomi.InformationModel.Component.Identification;
using Akomi.InformationModel.Device;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tapako.Repositories.WiringInformationSource;

namespace WiringInformationSourceTests
{
    [TestClass()]
    [Ignore]
    public class WiringInformationSourceTests
    {
        private WiringInformationSource _sut;

        [TestInitialize]
        public void Init()
        {
            _sut = new WiringInformationSource();
        }

        [TestMethod()]
        public void GetDeviceInformationsTest()
        {
            IDevice device = GetTestDevice();
            StoreDeviceInformationsTest();

            var result = _sut.GetDeviceInformations(device);

            Assert.AreEqual(device.Connections.Count(), result.Connections.Count());

            Assert.AreEqual(device.Connections.First().Name, result.Connections.First().Name);
        }

        [TestMethod()]
        public void StoreDeviceInformationsTest()
        {

            _sut.StoreDeviceInformations(GetTestDevice());

        }

        [TestMethod()]
        public void RestoreDeviceInformationsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RestoreInformationSourceTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void HasDeviceDriverTest()
        {
            Assert.Fail();
        }

        private IDevice GetTestDevice()
        {
            var device = new DeviceBase();
            device.Identification = new Identification();
            device.Identification.ModelNumber = "TestModelNumber";
            device.Identification.SerialNumber = "TestSerialNumber";
            device.Connections = new ConnectionList();

            for (int i = 0; i < 10; i++)
            {
                device.Connections.Add(CreateConnection("TestConnection" + i));
            }

            return device;
        }

        private Connection CreateConnection(string name)
        {
            return new Connection() { Name = name };
        }
    }
}