using System;
using Akomi.InformationModel.Component.Identification;
using Akomi.InformationModel.Component.ManufacturingData;
using Akomi.InformationModel.Datatypes;
using Akomi.InformationModel.Device;
using Tapako.Framework.ExtensionMethods;

namespace Tapako.ViewModel.DesignTime
{
    public static class DesignDataGenerator
    {
        private static readonly Random _randomizer = new Random();

        public static DeviceBase CreateDummyDevice()
        {
            var device = new DeviceBase();
            device.Identification = new Identification();
            device.ManufacturingData = new ManufacturingData();
            device.ManufacturingData.ManufacturerAddress = new Address() { Name = "Manufacturer " + _randomizer.Next(500) };
            device.SetBrowseName("Hostname " + _randomizer.Next(500));
            device.Identification.PhysicalAddress = _randomizer.Next(99999999).ToString();
            device.Identification.IpAddress = "127.0.0." + _randomizer.Next(250);
            return device;
        }

    }
}
