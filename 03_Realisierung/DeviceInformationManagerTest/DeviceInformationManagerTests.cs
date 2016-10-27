using System;
using Akomi.InformationModel.Component.Identification;
using Akomi.InformationModel.Component.ProductionData;
using Akomi.InformationModel.Datatypes;
using Akomi.InformationModel.Device;
using Akomi.InformationModel.Skills.SkillCatalogue.SearchForSubdevices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tapako.DeviceInformationManagement;
using Tapako.DeviceInformationManagement.InformationSources;
using Tapako.Framework.ExtensionMethods;
using Tapako.Repositories.DeviceDriverRepository;
using Tapako.Repositories.VirtualRepresentationRepository;

namespace DeviceInformationManagerTests
{
    /// <summary>
    /// </summary>
    [TestClass]
    public class DeviceInformationManagerTests
    {
        private PrivateType _privateSut;
        private IInformationSource _iSource1;
        private IInformationSource _iSource2;

        [TestInitialize]
        public void Init()
        {
            //Device Information Manager is a static class
            _privateSut = new PrivateType(typeof(DeviceInformationManager));
            _iSource1 = new DeviceDriverRepository();
            _iSource2 = new VirtualRepresentationRepository();
        }

        [TestCleanup]
        public void Cleanup()
        {
            DeviceInformationManager.CleanUp();
        }

        private void RegisterInformationSource(IInformationSource iSource)
        {
            DeviceInformationManager.RegisterInformationSource(iSource);
        }

        private void RegisterFistInformationSource()
        {
            RegisterInformationSource(_iSource1);
        }

        private void RegisterSecondInformationSource()
        {
            RegisterInformationSource(_iSource2);
        }

        private void RegisterManyInformationSources()
        {
            RegisterFistInformationSource();
            RegisterSecondInformationSource();
        }

        /// <summary>
        /// Dieses Device bietet ergänzungen zum ersten Device
        /// </summary>
        /// <returns></returns>
        private IDevice CreateSecondNotEmptyIDevice()
        {
            var notEmptyDevice = new DeviceBase();
            {
                //Type = new[] { "Lichschranke" },
              
            };
            notEmptyDevice.SetBrowseName("someValue");
            return notEmptyDevice;

        }

        /// <summary>
        /// Dieses Device bietet veränderungen des ersten devices
        /// todo: rekursive Objekte hinzufügen
        /// </summary>
        /// <returns></returns>
        private IDevice CreateThirdNotEmptyIDevice()
        {
            var notEmptyDevice = new DeviceBase()
            {
                // Alles Gleich wie im Device 1
                //DevicePath = "Testpfad",
                //DriverName = "lustiger Name",

                // Andere Werte als im Device 1
                Identification = new Identification()
                {
                    IpAddress = ("192.168.1.2"),
                    PhysicalAddress = "000000000001",
                },

                ProductionData = new ProductionData()
                {
                    ManufacturerAddress = new Address() { Name = "der traurige Mann" }
                }

            };

            return notEmptyDevice;
        }


        [TestMethod]
        public void RegisterOneIInformationSourceTest()
        {
            RegisterFistInformationSource();
            Assert.IsTrue(DeviceInformationManager.InformationSources.Contains(_iSource1));
            Assert.AreEqual(1, DeviceInformationManager.InformationSources.Count);
        }

        [TestMethod]
        public void RegisterTwoIInformationSourcesTest()
        {
            RegisterFistInformationSource();
            RegisterSecondInformationSource();

            Assert.IsTrue(DeviceInformationManager.InformationSources.Contains(_iSource1));
            Assert.IsTrue(DeviceInformationManager.InformationSources.Contains(_iSource2));
            Assert.AreEqual(2, DeviceInformationManager.InformationSources.Count);
        }


        [TestMethod]
        public void CleanupShallWork()
        {
            RegisterFistInformationSource();
            RegisterSecondInformationSource();

            DeviceInformationManager.CleanUp();
            Assert.AreEqual(0, DeviceInformationManager.InformationSources.Count);
        }



        [TestMethod]
        public void ShallNotDoubleRegisterSameIInformationSourceTest()
        {
            RegisterFistInformationSource();
            RegisterFistInformationSource();
            Assert.IsTrue(DeviceInformationManager.InformationSources.Contains(_iSource1));
            Assert.AreEqual(1, DeviceInformationManager.InformationSources.Count);

        }

        [TestMethod]
        public void LoadBeckhoffPlcDriverShallWork()
        {
            DeviceInformationManager.RegisterInformationSource(new DeviceDriverRepository());
            var beckhoffDeivce = new DeviceBase();
            beckhoffDeivce.Identification = new Identification();
            beckhoffDeivce.Identification.PhysicalAddress = "0001051825A2";
            var completeDevice = DeviceInformationManager.CompleteDeviceInformation<IDevice>(beckhoffDeivce);
            Assert.IsNotNull(completeDevice.Skills.GetSkill<SkillSearchForSubdevicesBase>());
            //Assert.IsNotNull(completeDevice.SearchForSubDevices);
        }

        [TestMethod]
        public void ShallNotDoubleRegisterEqualIInformationSourceTest()
        {
            RegisterFistInformationSource();
            var iSourceEqualToSource1 = Activator.CreateInstance(_iSource1.GetType());
            Assert.IsTrue(DeviceInformationManager.InformationSources.Contains(_iSource1));

            //Assert.AreEqual(_iSource1, iSourceEqualToSource1);

            RegisterInformationSource(iSourceEqualToSource1 as IInformationSource);
            Assert.AreEqual(1, DeviceInformationManager.InformationSources.Count);

        }

        [TestMethod]
        public void UnregisterIInformationSourceShallUnregisterObjects()
        {
            RegisterFistInformationSource();
            RegisterSecondInformationSource();

            DeviceInformationManager.UnregisterInformationSource(_iSource1);
            Assert.IsFalse(DeviceInformationManager.InformationSources.Contains(_iSource1));
            Assert.IsTrue(DeviceInformationManager.InformationSources.Contains(_iSource2));
            Assert.AreEqual(1, DeviceInformationManager.InformationSources.Count);

            DeviceInformationManager.UnregisterInformationSource(_iSource2);
            Assert.IsFalse(DeviceInformationManager.InformationSources.Contains(_iSource1));
            Assert.IsFalse(DeviceInformationManager.InformationSources.Contains(_iSource2));
            Assert.AreEqual(0, DeviceInformationManager.InformationSources.Count);

        }


        [TestMethod]
        public void UnregisterIInformationSourceShallUnregisterUnregisteredObjects()
        {
            RegisterFistInformationSource();
            RegisterSecondInformationSource();

            DeviceInformationManager.UnregisterInformationSource(_iSource1);
            DeviceInformationManager.UnregisterInformationSource(_iSource1);
            Assert.AreEqual(1, DeviceInformationManager.InformationSources.Count);

            DeviceInformationManager.UnregisterInformationSource(_iSource2);
            DeviceInformationManager.UnregisterInformationSource(_iSource2);

            Assert.IsFalse(DeviceInformationManager.InformationSources.Contains(_iSource1));
            Assert.IsFalse(DeviceInformationManager.InformationSources.Contains(_iSource2));
            Assert.AreEqual(0, DeviceInformationManager.InformationSources.Count);
        }

        [TestMethod]
        public void CompleteDeviceInformationWithoutInformationSourceShallNotThrowError()
        {
            var iDevice1 = new DeviceBase();

            Assert.AreEqual(0, DeviceInformationManager.InformationSources.Count);

            DeviceInformationManager.CompleteDeviceInformation(iDevice1);
        }

        [TestMethod]
        public void CompleteDeviceInformationWithInformationSourcesShallNotThrowError()
        {
            var iDevice1 = new DeviceBase();
            RegisterManyInformationSources();

            Assert.AreNotEqual(0, DeviceInformationManager.InformationSources.Count);

            DeviceInformationManager.CompleteDeviceInformation(iDevice1);
        }

        [TestMethod]
        public void NewTest()
        {
            DeviceInformationManager.RegisterInformationSource(new DeviceDriverRepository());
            var beckhoffDeivce = new DeviceBase();
            beckhoffDeivce.Identification = new Identification();
            beckhoffDeivce.Identification.PhysicalAddress = ("0001051825A2");
            var completeDevice = DeviceInformationManager.CompleteDeviceInformation(beckhoffDeivce);
            //Assert.IsNotNull(completeDevice.SearchForSubDevices);
            Assert.IsNotNull(completeDevice.Skills.GetSkill<SkillSearchForSubdevicesBase>());

        }



    }

}
