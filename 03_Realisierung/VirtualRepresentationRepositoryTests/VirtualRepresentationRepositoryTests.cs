using System.IO;
using Akomi.InformationModel.Component.Identification;
using Akomi.InformationModel.Device;
using Akomi.InformationModel.Device.Description;
using Akomi.InformationModel.Device.Parametrization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tapako.DeviceInformationManagement;
using Tapako.Repositories.DeviceDriverRepository;
using Tapako.Repositories.SerializedDeviceRepository;
using Tapako.Repositories.VirtualRepresentationRepository;
using Tapako.Repositories.WiringInformationSource;
using VirtualRepresentationRepositoryTests.Classes;

namespace VirtualRepresentationRepositoryTests
{
    [TestClass()]
    [DeploymentItem(@"VR Repository", "TestRepositoryFolder")]

    public class VirtualRepresentationRepositoryTests
    {
        [TestMethod()]
        public void GetDeviceInformationTest()
        {
            var repository = new VirtualRepresentationRepository("TestRepositoryFolder");
            
            IDevice device = new DummyDevice();
            device.Identification = new Identification();

            device.Identification.SerialNumber = "seriennummer1";
            device.Identification.ModelNumber = "TestModel";

            //SerializeDevice(device);

            var result = repository.GetDeviceInformations(device);
            Assert.IsNotNull(result);
            Assert.AreEqual("Utzenbichl", result.Documentation.Location);
        }



        public void SerializeDevice(IDevice device)
        {
            var b = new DummyDevice();
            b.Identification = new Identification();
            b.Identification.ModelNumber = "ModelNumber";


            var writer = new System.Xml.Serialization.XmlSerializer(typeof(DummyDevice));
            var wfile = new System.IO.StreamWriter(@"temp.xml");
            writer.Serialize(wfile, b);
            wfile.Close();
        }

        [TestMethod]
        public void SaveBeckhoffDriver()
        {
            var sut = CreateNewHostDevice("129.187.158.64");
            sut.Parametrization = new Parametrization();

            sut.Identification.ModelNumber = "TestBeckhoffDevice";
            sut.Identification.SerialNumber = "1234";
            sut.Description.DeviceClassification = DeviceClassification.NextGeneration.ToString();

            sut.Parametrization.AddParameter(new DeviceParameter());
            sut.Parametrization.AddParameter(new DeviceParameter());
            sut.Parametrization.AddParameter(new DeviceParameter());

            var repository = Directory.GetCurrentDirectory();
            var serializingRepository = new SerializedDeviceRepository(repository);
            serializingRepository.FileDialogToChooseSaveFile = false;
            serializingRepository.ClassificationsToSave = DeviceClassification.Basic |
                                                          DeviceClassification.NextGeneration;

            DeviceInformationManager.RegisterInformationSource(serializingRepository);
            DeviceInformationManager.RegisterInformationSource(new VirtualRepresentationRepository(repository));
            DeviceInformationManager.RegisterInformationSource(new DeviceDriverRepository(repository));
            DeviceInformationManager.RegisterInformationSource(new WiringInformationSource(repository));

            DeviceInformationManager.StoreDeviceInformations(sut);
        }

        private IDevice CreateNewHostDevice(string s)
        {
            var device = new DeviceBase();
            device.Identification = new Identification();
            return device;
        }

    }
}