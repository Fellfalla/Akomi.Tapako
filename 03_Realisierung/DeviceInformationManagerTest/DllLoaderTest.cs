using System;
using Akomi.InformationModel.Device;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tapako.DeviceInformationManagement.InformationSources;
using Tapako.Framework;
using System.Reflection;
using Akomi.InformationModel.Component.Identification;

namespace DeviceInformationManagerTests
{
    [TestClass]
    public class DllLoaderTest
    {

        [TestInitialize]
        public void Init()
        {
            //DllLoader is a static class
        }

        [TestMethod]
        [Timeout(1000)]
        public void LoadDriverTakesNull()
        {
            DllLoader.Load<object>(null);
        }


        [TestMethod]
        //[Timeout(1000)]
        public void LoadTestDll()
        {
            string filePath = Assembly.GetExecutingAssembly().Location;
            //string filePath = "DeploymentData/DeviceCompletement.dll";
            var factory = DllLoader.Load<IDeviceCompletement>(filePath);

            IDevice testClass = new DeviceBase();
            testClass.Identification = new Identification();

            factory.CompleteDeviceDriver(ref testClass);

            Assert.AreEqual("Dummy1", testClass.Identification.SerialNumber);
        }
    }

    public abstract class DummyCompletementBase : IDeviceCompletement
    {
        public abstract IDevice CompleteDeviceDriver(ref IDevice deviceRoot);
    }

    public class DummyCompletement : DummyCompletementBase
    {
        public override IDevice CompleteDeviceDriver(ref IDevice deviceRoot)
        {
            deviceRoot.Identification.SerialNumber = "Dummy1";
            return deviceRoot;
        }
    }
}
