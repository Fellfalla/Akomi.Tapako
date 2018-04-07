using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Akomi.InformationModel.Component.Connection;
using Akomi.InformationModel.Device;
using Akomi.InformationModel.Device.Description;
using Akomi.Logger;
using Tapako.DeviceInformationManagement.InformationSources;
using Tapako.DeviceInformationManagement.IO;

namespace Tapako.Repositories.SubdeviceStorage
{
    /// <summary>
    /// This Repository is used to save subdevices of specified classifications of the given Device
    /// </summary>
    public class SubDeviceRepository : RepositoryBase
    {

        [DefaultValue(
    DeviceClassification.NextGeneration | DeviceClassification.Standard | DeviceClassification.Basic |
    DeviceClassification.Field | DeviceClassification.Primitive)]
        public DeviceClassification TargetClassifications { get; set; }

        private string _extension = ".device";

        public SubDeviceRepository() : base()
        {
            RepositoryFolder = Path.Combine(Directory.GetCurrentDirectory(), Name);
        }

        /// <summary>
        /// This method has to be overridden by <see cref="RepositoryBase"/> derived classes.
        /// In this method the specific information collecting logic should be located.
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        protected override IDevice InnerGetDeviceInformations(IDevice device)
        {
            foreach (var driverUri in GetDriverUris(device))
            {
                var loadedDevice = StorageModule.LoadFromFile<DeviceBase>(driverUri.OriginalString);
                if (HasTargetClassification(loadedDevice))
                {
                    if (device.SubDevices == null)
                    {
                        device.SubDevices = new List<IDevice>();
                    }

                    device.SubDevices.Add(loadedDevice);
                    this.GetDeviceInformations(loadedDevice);
                }
            }
            return device;
        }

        private bool HasTargetClassification(IDevice device)
        {
            if (device != null && device.Description != null)
            {
                return device.Description.DeviceClassification == TargetClassifications.ToString();
            }
            return false;
        }

        /// <summary>
        /// Template Method for storing device informations
        /// </summary>
        /// <param name="device"></param>
        protected override void InnerStoreDeviceInformations(IDevice device)
        {
            foreach (var currentDevice in device.ForEach())
            {
                var deviceFolder = GetDeviceInformationFolder(currentDevice);

                // create serialized file for each subdevice
                foreach (var subDevice in currentDevice.SubDevices)
                {
                    if (HasTargetClassification(subDevice))
                    {
                        var fileName = GetDeviceIdentifier(subDevice);
                        if (deviceFolder != null && fileName != null)
                        {
                            string filePath = Path.Combine(deviceFolder, fileName + _extension);

                            if (File.Exists(filePath))
                            {
                                Logger.Warning("Existing file \"{0}\" will be overridden", filePath);
                            }

                            StorageModule.SaveToFile(subDevice, filePath);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method for reverting custom information changes to factory default of a specific Device.
        /// </summary>
        /// <param name="iDevice"></param>
        public override void RestoreDeviceInformations(IDevice iDevice)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method for doing the same as <see cref="RepositoryBase.RestoreDeviceInformations"/> for all available devices.
        /// </summary>
        public override void RestoreInformationSource()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// How a repository is called.
        /// </summary>
        public override string Name { get { return "SubDeviceRepository"; } }

        /// <summary>
        /// Unique id of the derived class.
        /// </summary>
        public override int Id { get { return 456; } }

        /// <summary>
        /// Call this Method to find out if a driver for <paramref name="iDevice"/> exists.
        /// </summary>
        /// <param name="iDevice"></param>
        /// <returns>true if driver is found otherwise false.</returns>
        public override bool HasDeviceDriver(IDevice iDevice)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// This methods returns the <see cref="System.Uri"/> of all driver files.
        /// </summary>
        /// <param name="device">If this param is null, all file uris will be returned</param>
        /// <returns></returns>
        public override IEnumerable<Uri> GetDriverUris(IDevice device)
        {
            var folder = GetDeviceInformationFolder(device);
            if (folder != null && Directory.Exists(folder))
            {
                return Directory.GetFiles(folder).Select((file) => new Uri(file));
            }
            return new Uri[]{};
        }

        private string GetDeviceInformationFolder(IDevice device)
        {
            var deviceIdentifier = GetDeviceIdentifier(device);
            if (deviceIdentifier != null)
            {
                return Path.Combine(RepositoryFolder, deviceIdentifier);
            }
            else
            {
                Logger.Warning("No device identifier for \"{0}\" in order to be able to save subdevices", device);
            }
            return null;
        }

        private string GetDeviceIdentifier(IDevice device)
        {
            var identifier = string.Empty;

            if (device == null || device.Identification == null )
            {
                return identifier;
            }

            // add serial number
            if (!string.IsNullOrWhiteSpace(device.Identification.SerialNumber))
            {
                identifier += ReplaceIllegalCharacters(device.Identification.SerialNumber);
            }

            // add model number
            if (!string.IsNullOrWhiteSpace(device.Identification.ModelNumber))
            {
                identifier += ReplaceIllegalCharacters(device.Identification.ModelNumber);
            }

            // add mac address
            if (!string.IsNullOrWhiteSpace(device.Identification.PhysicalAddress))
            {
                identifier += ReplaceIllegalCharacters(device.Identification.PhysicalAddress);
            }

            // add information from connection
            if (device.Ports != null && device.Ports.Any())
            {
                foreach (var connection in device.Ports)
                {
                    string connectionMessage = ConnectionToString(connection);
                    if (string.IsNullOrWhiteSpace(connectionMessage))
                    {
                        identifier += connectionMessage;
                    }
                }
            }

            return identifier;
        }

        private string ConnectionToString(ComponentPort connection)
        {
            if (connection == null || 
                connection.Communication == null || 
                connection.Communication.ConnectionPoints == null ||
                !connection.Communication.ConnectionPoints.Any())
            {
                return null;
            }

            return string.Join("_", connection.Communication.ConnectionPoints);
        }


        /// <summary>
        /// The SourcePriority of this <see cref="IInformationSource"/>.
        /// For further informations look at <see cref="SourcePriority"/>
        /// </summary>
        public override SourcePriority SourcePriority { get; set; }
    }
}
