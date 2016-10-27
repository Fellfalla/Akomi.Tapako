using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Akomi.InformationModel.Device;
using Akomi.InformationModel.Device.Description;
using Akomi.Logger;
using Tapako.DeviceInformationManagement.InformationSources;
using Tapako.DeviceInformationManagement.IO;

namespace Tapako.Repositories.SerializedDeviceRepository
{
    public class SerializedDeviceRepository : RepositoryBase
    {
        private SourcePriority _sourcePriority = SourcePriority.Low;
        private bool _fileDialogToChooseSaveFile = false;

        public static string FileExtension = "device";

        [DefaultValue(
            DeviceClassification.NextGeneration | DeviceClassification.Standard | DeviceClassification.Basic |
            DeviceClassification.Field | DeviceClassification.Primitive)]
        public DeviceClassification ClassificationsToSave { get; set; }


        public bool FileDialogToChooseSaveFile
        {
            get { return _fileDialogToChooseSaveFile; }
            set { _fileDialogToChooseSaveFile = value; }
        }

        public SerializedDeviceRepository(string respository) : base(respository)
        {
        }

        public SerializedDeviceRepository() : base()
        {
        }

        /// <summary>
        /// Deserialized all devices of the given device class <paramref name="deviceClassification"/>
        /// </summary>
        /// <param name="deviceClassification"></param>
        /// <returns></returns>
        public IEnumerable<IDevice> GetDevicesOfDeviceClassification(DeviceClassification deviceClassification)
        {
            foreach (var file in Directory.GetFiles(GetDirectory(deviceClassification)))
            {
                yield return GetDeviceInformations(StorageModule.LoadFromFile<IDevice>(file));
            }
        }

        protected override IDevice InnerGetDeviceInformations(IDevice device)
        {
            IDevice result;
            string filePath = GetFilePath(device);
            if (!File.Exists(filePath))
            {
                return null;
            }

            if (!(device is DeviceBase))
            {
                Logger.Error("Device is not castable to serializable type \"DeviceBase\"");
                return null;
            }

            result = StorageModule.LoadFromFile<DeviceBase>(filePath);
            return result;
        }

        protected override void InnerStoreDeviceInformations(IDevice device)
        {
            string filepath = GetFilePath(device);
            string directory = Path.GetDirectoryName(filepath);
            // return if nothing is savable
            if (device == null)
            {
                Logger.Info("IDevice is null. Nothing will be stored.");
                return;
            }

            if (!(device is DeviceBase))
            {
                Logger.Error("Device is not castable to serializable type \"DeviceBase\"");
                return;
            }

            DeviceBase castedDevice = (DeviceBase)device;

            if (string.IsNullOrWhiteSpace(filepath))
            {
                Logger.Info("Could not generate filepath for {0}", castedDevice.ToString(true));
                return;
            }

            // create directory if it doesn't exist
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            StorageModule.SaveToFile(castedDevice, filepath);
            //SerializeDevice(device);
        }

        public override void RestoreDeviceInformations(IDevice iDevice)
        {
            throw new NotImplementedException();
        }

        public override void RestoreInformationSource()
        {
            throw new NotImplementedException();
        }

        public override string Name
        {
            get { return "Serialized device repository"; }
        }

        public override int Id
        {
            get { return 202; }
        }

        public override bool HasDeviceDriver(IDevice iDevice)
        {
            return File.Exists(GetFilePath(iDevice));
        }

        public override SourcePriority SourcePriority
        {
            get { return _sourcePriority; }
            set { _sourcePriority = value; }
        }

        private string GetFilePath(IDevice device)
        {
            if (device == null)
            {
                Logger.Info("Could resolve device folder due to null device");
                return null;
            }

            if (device.Identification == null)
            {
                Logger.Info("Could resolve device folder for \"{0}\", because device has no identification", device);
                return null;
            }

            if (string.IsNullOrWhiteSpace(device.Description.DeviceClassification))
            {
                Logger.Info("Could resolve device folder for \"{0}\" due to missing DeviceClassification", device);
                return null;
            }

            if (FileDialogToChooseSaveFile)
            {
                return StorageModule.ChooseFile(device, StorageModule.Mode.Save);
            }
            else
            {
                string folder = GetDirectory(device);
                return Path.Combine(folder, GetFileName(device));
            }
        }

        private string GetDirectory(IDevice device)
        {
            return GetDirectory(device.Description.DeviceClassification);
        }

        private string GetDirectory(DeviceClassification deviceClassification)
        {
            return GetDirectory(deviceClassification.ToString());
        }

        private string GetDirectory(string deviceClassification)
        {
            string rawPath = Path.Combine(RepositoryFolder, deviceClassification);
            return rawPath; //ReplaceIllegalCharacters(rawPath);
        }

        private string GetFileName(IDevice device)
        {
            if (device == null || device.Identification == null ||
                (string.IsNullOrWhiteSpace(device.Identification.ModelNumber) ||
                 string.IsNullOrWhiteSpace(device.Identification.SerialNumber)))
            {
                return null;
            }
            else
            {
                string name = device.Identification.ModelNumber + device.Identification.SerialNumber;
                return ReplaceIllegalCharacters(name + "." + FileExtension);
            }
        }
    }
}