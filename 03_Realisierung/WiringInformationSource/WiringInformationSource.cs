using System;
using System.IO;
using System.Linq;
using Akomi.InformationModel.Device;
using Akomi.Logger;
using Tapako.DeviceInformationManagement.InformationSources;

namespace Tapako.Repositories.WiringInformationSource
{
    /// <summary>
    /// Diese Informationsquelle ist zum abspeichern von Connections, damit nicht alle primitive Geräte immer neu vernetzt werden müssen
    /// </summary>
    public class WiringInformationSource : RepositoryBase
    {
        private SourcePriority _sourcePriority = SourcePriority.Low;

        public static string DefaultRepositoryFolder = "Wirings";

        public WiringInformationSource() : base(DefaultRepositoryFolder) {}

        public WiringInformationSource(string repositoryFolder) : base(repositoryFolder) { }

        #region Interface Members

        protected override IDevice InnerGetDeviceInformations(IDevice device)
        {
            // todo: serialized wirings with Json
            //string filePath = GetFilePath(device);
            //if (!File.Exists(filePath))
            //{
            //    Logger.Debug("{0} does not exist", filePath);
            //    return null;
            //}

            //// Create empty Device instance
            //IDevice newDevice = new DeviceBase();
            //newDevice.Connections = new List<Connection>();

            //using (Stream stream = File.Open(filePath, FileMode.Open))
            //{
            //    BinaryFormatter bin = new BinaryFormatter();

            //    newDevice.Connections = (IList<Connection>)bin.Deserialize(stream);
            //    Logger.Info("Loaded {0} connections from wiring information source", newDevice.Connections.Count);
            //}
            //return newDevice;
            Logger.Error("Loading device connections is no implemented.");
            return null;
        }

        protected override void InnerStoreDeviceInformations(IDevice device)
        {
            string filepath = GetFilePath(device);
            string directory = Path.GetDirectoryName(filepath);
            
            // return if nothing is savable
            if (device == null)
            {
                Logger.Info("IDevice is null. No connections will be saved");
                return;
            }
            if (device.Connections == null || !device.Connections.Any())
            {
                Logger.Info("No connections to save for {0}", device);
                return;
            }
            if (string.IsNullOrEmpty(filepath))
            {
                string deviceInformation;
                if (device is DeviceBase)
                {
                    deviceInformation = ((DeviceBase)device).ToString(true);
                }
                else
                {
                    deviceInformation = device.ToString();
                }
                Logger.Info("Could not generate filepath for {0}", deviceInformation);
                return;
            }

            // create directory if it doesn't exist
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Save connections
            Logger.Error("Saving connections is no implemented.");

            //using (Stream stream = File.Open(GetFilePath(device), FileMode.Create))
            //{
            //    Logger.Info("Saving {0} connections for {1}", device.Connections.Count, device);
            //    BinaryFormatter bin = new BinaryFormatter();
            //    bin.Serialize(stream, device.Connections);
            //}
        }

        public override void RestoreDeviceInformations(IDevice iDevice)
        {
            throw new NotImplementedException();
        }

        public override void RestoreInformationSource()
        {
            throw new NotImplementedException();
        }

        public override bool HasDeviceDriver(IDevice iDevice)
        {
            return File.Exists(GetFilePath(iDevice));
        }

        public override string Name
        {
            get
            {
                return "Wiring information source";
            }
        }

 
        public override int Id
        {
            get
            {
                return 201;
            }
        }

        public override SourcePriority SourcePriority
        {
            get { return _sourcePriority; }
            set { _sourcePriority = value; }
        }
        #endregion

        private string GetFilePath(IDevice device)
        {
            string fileName = GetFileName(device);

            return (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(RepositoryFolder)) ? null : Path.Combine(RepositoryFolder, fileName);
        }

        private string GetFileName(IDevice device)
        {
            if (device == null)
            {
                Logger.Debug("Could not generatre filePath due to device is null.");
                return null;
            }
            
            if (device.Identification == null)
            {
                Logger.Debug("Could not generate filePath for {0} due to Identification is null", device);
                return null;
            } 
            
            if (string.IsNullOrEmpty(device.Identification.ModelNumber))
            {
                Logger.Debug("Could not generate filePath for {0} due to ModelNumber is null", device);
            }
            
            if (string.IsNullOrEmpty(device.Identification.SerialNumber))
            {
                Logger.Debug("Could not generate filePath for {0} due to SerialNumber is null", device);
            }

            else
            {
                string name = device.Identification.ModelNumber + "_" + device.Identification.SerialNumber;
                string extension = ".wiring";
                return ReplaceIllegalCharacters(name + extension);
            }

            return null;
        }
    }
}
