using System;
using System.Collections.Generic;
using Akomi.InformationModel.Device;
using KellermanSoftware.CompareNetObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RandomExtension;
using Tapako.DeviceInformationManagement.IO;

namespace DeviceInformationManagerTests.IO
{
    [TestClass()]
    public class TapakoSerializerTests
    {
        Random random = new Random();
        public static TapakoSerializer sut = new TapakoSerializer();

        [TestMethod()]
        [Ignore]
        public void TapakoSerializerTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SerializeTest()
        {

            DeviceBase device = random.GetRandom<DeviceBase>();
            sut.Serialize(device);
        }

        [TestMethod()]
        [Ignore]
        public void DeserializeTest()
        {
            DeviceBase device = random.GetRandom<DeviceBase>();
            var data = sut.Serialize(device);
            var deserialized = sut.Deserialize<DeviceBase>(data);

            CompareLogic compareLogic = new CompareLogic();
            var compareResult = compareLogic.Compare(device, deserialized);
            Assert.IsTrue(compareResult.AreEqual, compareResult.DifferencesString);
        }


        [TestMethod()]
        public void DeserializeTest2()
        {
            DeviceBase device = new DeviceBase();
            device.SubDevices = new List<IDevice>()
            {
                new DeviceBase(),
                new DeviceBase(),
                new DeviceBase(),
                new DeviceBase(),
                null,
                null
            };
            var data = sut.Serialize(device);
            var deserialized = sut.Deserialize<DeviceBase>(data);

            CompareLogic compareLogic = new CompareLogic();
            var compareResult = compareLogic.Compare(device, deserialized);
            Assert.IsTrue(compareResult.AreEqual, compareResult.DifferencesString);
        }




        [TestMethod()]
        public void DeserializeTestIList()
        {
            IList<int> toSerialize = new List<int>() {1,1,1,2,3};
            var data = sut.Serialize(toSerialize);
            var deserialized = sut.Deserialize<IList<int>>(data);

            CompareLogic compareLogic = new CompareLogic();
            var compareResult = compareLogic.Compare(toSerialize, deserialized);
            Assert.IsTrue(compareResult.AreEqual, compareResult.DifferencesString);
        }

        [TestMethod()]
        public void DeserializeTestArray()
        {
            var toSerialize = new int[] {1,1,1,2,3};
            var data = sut.Serialize(toSerialize);
            var deserialized = sut.Deserialize<int[]>(data);

            CompareLogic compareLogic = new CompareLogic();
            var compareResult = compareLogic.Compare(toSerialize, deserialized);
            Assert.IsTrue(compareResult.AreEqual, compareResult.DifferencesString);
        }



    }
}