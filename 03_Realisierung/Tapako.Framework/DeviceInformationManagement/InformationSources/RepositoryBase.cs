using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Akomi.InformationModel.Device;
using Akomi.Logger;
using Tapako.Framework;

namespace Tapako.DeviceInformationManagement.InformationSources
{
    /// <summary>
    /// Base class for <see cref="IInformationSource"/>s.
    /// This Class defines also a frameworks to box specific <see cref="IInformationSource"/>-Logic in common Methods.
    /// The Template Method Pattern is adopted??.
    /// </summary>
    public abstract class RepositoryBase : IInformationSource
    {
        private string _repositoryFolder;

        /// <summary>
        /// Constructor which takes a string to definde the default <see cref="RepositoryFolder"/>
        /// </summary>
        /// <param name="repositoryFolder"></param>
        protected RepositoryBase(string repositoryFolder) 
        {
            RepositoryFolder = repositoryFolder;
        }

        /// <summary>
        /// default constructor.
        /// <see cref="RepositoryFolder"/> will be set to the current directory.
        /// </summary>
        protected RepositoryBase()
        {
            RepositoryFolder = Directory.GetCurrentDirectory();
        }

        /// <summary>
        ///     Erzeugt aus einem übergebenen Devicenamen den Pfad, an dem sich die .dll Datei befindet
        /// </summary>
        protected string GenerateFilePath(string fileName)
        {
            fileName = ReplaceIllegalCharacters(fileName);
            string result = string.Empty;
            IEnumerable<string> files = new string[0];
            if (Directory.Exists(RepositoryFolder))
            {
                files = Directory.GetFiles(RepositoryFolder);
            }
            else
            {
                Logger.Warning("The expected driver repository {0} doesn't exist", RepositoryFolder);
            }
            //files = files.Concat(Directory.GetFiles(Directory.GetCurrentDirectory()));

            if (Path.HasExtension(fileName)) // Suche exakte Datei mit passender Dateiendung
            {
                result = files.FirstOrDefault(item =>
                {
                    var itemWithExtension = Path.GetFileName(item);
                    return itemWithExtension != null && itemWithExtension.Equals(fileName);
                });
            }

            if(string.IsNullOrWhiteSpace(result))//|| !File.Exists(Path.Combine(RepositoryFolder, result))) // Keine Dateiendung vorhanden -> suche nach passendem Dateinamen und beliebiger endung
            {
                result = files.FirstOrDefault(item =>
                {
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(item);
                    return (fileNameWithoutExtension != null && fileNameWithoutExtension.Equals(fileName));
                });
            }

            return result;
        }

        /// <summary>
        /// Generates the path at which the informations are stored.
        /// </summary>
        /// <param name="fileName">This string will be used to look for a specific driver file</param>
        /// <param name="directory">The directory in which the driver files are located</param>
        /// <returns></returns>
        protected virtual string GetFilePath(string fileName, string directory)
        {
            fileName = ReplaceIllegalCharacters(fileName);

            IEnumerable<string> files;
            if (Directory.Exists(directory))
            {
                files = Directory.GetFiles(directory);
            }
            else
            {
                Logger.Debug("The expected driver repository {0} doesn't exist", directory);
                return null;
            }

            if (Path.HasExtension(fileName)) // Suche exakte Datei mit passender Dateiendung
            {
                return files.FirstOrDefault(item =>
                {
                    var itemWithExtension = Path.GetFileName(item);
                    return itemWithExtension != null && itemWithExtension.Equals(fileName);
                });
            }
            else // Keine Dateiendung vorhanden -> suche nach passendem Dateinamen und beliebiger endung
            {
                return files.FirstOrDefault(item =>
                {
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(item);
                    return (fileNameWithoutExtension != null && fileNameWithoutExtension.Equals(fileName));
                });
            }
        }

        /// <summary>
        /// The absolute path of the folder which contains almost all source files for the <see cref="IInformationSource"/>
        /// </summary>
        public string RepositoryFolder
        {
            get
            {
                if (Path.IsPathRooted(_repositoryFolder))
                {
                    return _repositoryFolder;
                }
                return Path.Combine(Directory.GetCurrentDirectory(), _repositoryFolder);
            }
            set { _repositoryFolder = value; }
        }


        /// <summary>
        ///     Gibt Rückmeldung über alle verfügbaren Informationen, wenn das Laden des Treibers nicht geklappt hat
        /// </summary>
        protected void LogFailedAttempt(IDevice device, string dllPath)
        {
            var physicalAddres = "null";
            var modelNumber = "null";
            var deviceName = "null";
            var identification = "null";
            // Get Log Informations
            if (device != null)
            {
                deviceName = device.ToString(true);
            }
            if (device != null && device.Identification != null)
            {
                identification = device.Identification.ToString();
                modelNumber = device.Identification.ModelNumber;
                physicalAddres = device.Identification.PhysicalAddress;
            }

            var message = "";
            message += string.Format("RepositoryFolder: \t\"{0}\"\n", RepositoryFolder);
            message += string.Format("Device: \t\"{0}\"\n", deviceName);
            message += string.Format("\tIdentification: \t\"{0}\"\n", identification);
            message += string.Format("\tPhysicalAddress: \t\"{0}\"\n", physicalAddres);
            message += string.Format("\tModelNumber: \t\"{0}\"\n", modelNumber);
            message += string.Format("\tDriverPath: \t\"{0}\"\n", dllPath);

            Logger.Debug("Could not load driver for following context:\n{0}", message);
        }

        /// <summary>
        ///     Replaces special characters with an "_". Because filename shouldn't contain "?\/|:"...
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReplaceIllegalCharacters(string path)
        {
            return FunctionCollection.ReplaceIllegalPathCharacters(path, Constants.ReplacingChar.ToString());
        }

        /// <summary>
        /// Loads stored informations for <paramref name="device"/>.
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public IDevice GetDeviceInformations(IDevice device)
        {
            Logger.Debug("Getting device informations for {0} from {1}.", device, Name);
            var result = InnerGetDeviceInformations(device);
            if (result == null)
            {
                Logger.Debug("{0} returned no informations.", Name);
            }
            return result;
        }

        /// <summary>
        /// This method has to be overridden by <see cref="RepositoryBase"/> derived classes.
        /// In this method the specific information collecting logic should be located.
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        protected abstract IDevice InnerGetDeviceInformations(IDevice device);

        /// <summary>
        /// Stores the current status of <paramref name="device"></paramref> in the <see cref="IInformationSource"/>.
        /// This can made undone by calling <see cref="RestoreDeviceInformations"/>
        /// </summary>
        /// <param name="device"></param>
        public void StoreDeviceInformations(IDevice device)
        {
            Logger.Debug("Storing informations for \"{0}\" in {1}", device, Name);
            InnerStoreDeviceInformations(device);
        }

        /// <summary>
        /// Template Method for storing device informations
        /// </summary>
        /// <param name="device"></param>
        protected abstract void InnerStoreDeviceInformations(IDevice device);

        /// <summary>
        /// Method for reverting custom information changes to factory default of a specific Device.
        /// </summary>
        /// <param name="iDevice"></param>
        public abstract void RestoreDeviceInformations(IDevice iDevice);

        /// <summary>
        /// Method for doing the same as <see cref="RestoreDeviceInformations"/> for all available devices.
        /// </summary>
        public abstract void RestoreInformationSource();

        /// <summary>
        /// How a repository is named.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Unique id of the derived class.
        /// </summary>
        public abstract int Id { get; }

        /// <summary>
        /// This methods return the <see cref="Uri"/> of the driver file.
        /// 
        /// </summary>
        /// <param name="device">If this param is null, all file uris will be returned</param>
        /// <returns></returns>
        public virtual IEnumerable<Uri> GetDriverUris(IDevice device)
        {
            if (device == null)
            {
                foreach (var file in Directory.GetFiles(RepositoryFolder))
                {
                    yield return new Uri(file);
                }
            }
        }

        /// <summary>
        /// The SourcePriority of this <see cref="IInformationSource"/>.
        /// For further informations look at <see cref="SourcePriority"/>
        /// </summary>
        public abstract SourcePriority SourcePriority { get; set; }

        /// <summary>
        /// Call this Method to find out if a driver for <paramref name="iDevice"/> exists.
        /// </summary>
        /// <param name="iDevice"></param>
        /// <returns>true if driver is found otherwise false.</returns>
        public abstract bool HasDeviceDriver(IDevice iDevice);

        /// <summary>
        /// Custom ToString() implementation for classes of type <see cref="RepositoryBase"/>s
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return !string.IsNullOrEmpty(Name) ? Name : base.ToString();
        }

        public virtual void RemoveDeviceInformations(IDevice iDevice)
        {
            throw new NotSupportedException();
        }
    }
}
