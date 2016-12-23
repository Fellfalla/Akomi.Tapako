using System;
using System.Linq;
using Akomi.InformationModel.Component.Identification;
using Akomi.InformationModel.Device;
using Akomi.InformationModel.Device.Description;
using ExtensionMethodsCollection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RandomExtension;
using Tapako.Repositories.SerializedDeviceRepository;

namespace SerializedDeviceRepositoryTests
{
    [TestClass()]
    public class SerializedDeviceRepositoryTests
    {

        private SerializedDeviceRepository _sut;

        private Random random;

        [TestInitialize]
        public void Init()
        {
            _sut = new SerializedDeviceRepository();
            random = new Random();
        }

        [TestMethod()]
        [Ignore]
        public void GetDevicesOfDeviceClassificationTest()
        {
            var device = GetRandomDevice(DeviceClassification.Primitive);

            // store
            _sut.StoreDeviceInformations(device);

            var result = _sut.GetDeviceInformations(device);

            Assert.IsNotNull(result);
            
            Assert.IsTrue(typeof(IDevice).AllValuesAreEqual(device, result));
        }

        [TestMethod()]
        [Ignore]
        public void GetDeviceInformationsTest()
        {
            StoreDeviceInformationsTest();

            var deviceList = _sut.GetDevicesOfDeviceClassification(DeviceClassification.Primitive);

            Assert.IsNotNull(deviceList);
            Assert.IsTrue(deviceList.Any());

            foreach (var device in deviceList)
            {
                Console.WriteLine(device);
            }

        }

        [TestMethod()]
        [Ignore]
        public void StoreDeviceInformationsTest()
        {
            var device = GetRandomDevice(DeviceClassification.Primitive);

            // single store
            _sut.StoreDeviceInformations(device);

            // store twice
            _sut.StoreDeviceInformations(device);

            // store others
            _sut.StoreDeviceInformations(GetRandomDevice(DeviceClassification.Primitive));
            _sut.StoreDeviceInformations(GetRandomDevice(DeviceClassification.Primitive));
        }

        [TestMethod()]
        [Ignore]
        public void RestoreDeviceInformationsTest()
        {
            var device = GetRandomDevice(DeviceClassification.Primitive);
            //device.SubDevices.Clear();
            _sut.StoreDeviceInformations(device);

            var result = _sut.GetDeviceInformations(device);
            //Assert.AreEqual(1,result.Count(), string.Join("\n", result));

            Assert.AreEqual(device.ToString(), result.ToString());
            Assert.AreEqual(device.SubDevices.Count(), result.SubDevices.Count());
        }

        [TestMethod()]
        [Ignore]
        public void RestoreInformationSourceTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void HasDeviceDriverTest()
        {
            Assert.Fail();
        }

        public IDevice GetDevice(string model, string serial)
        {
            IDevice device = new DeviceBase();
            device.Identification = new Identification()
            {
                ModelNumber = model,
                SerialNumber = serial,
            };

                device.Description.DeviceClassification = DeviceClassification.Primitive.ToString();

            return device;
        }

        public IDevice GetRandomDevice(DeviceClassification deviceClassification)
        {
            IDevice device = random.GetRandom<DeviceBase>();

            device.Description.DeviceClassification = deviceClassification.ToString();

            int maxChars = 12;

            if (device.Identification.ModelNumber.Length > maxChars)
            {
                device.Identification.ModelNumber = device.Identification.ModelNumber.Substring(0, maxChars);
            }

            if (device.Identification.SerialNumber.Length > maxChars)
            {
                device.Identification.SerialNumber = device.Identification.SerialNumber.Substring(0, maxChars);
            }

            return device;
        }

    }
}