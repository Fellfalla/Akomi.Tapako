using System.Collections.Generic;
using System.IO;
using System.Linq;
using Akomi.Logger;
using IAkomiDevice.fIDevice;
using Tapako.Core.DeviceInformationManagement;
using Tapako.Services;
using Constants = Tapako.Services.Constants;

namespace Tapako.Repositories.RepositoryBase
{
    public abstract class RepositoryBase : IInformationSource
    {
        private string _repositoryFolder;

        protected RepositoryBase(string repositoryFolder)
        {
            RepositoryFolder = repositoryFolder;
        }

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

            IEnumerable<string> files = new string[0];
            if (Directory.Exists(RepositoryFolder))
            {
                files = Directory.GetFiles(RepositoryFolder);
            }
            else
            {
                Logger.Warning("The expected driver repository {0} doesn't exist", RepositoryFolder);
            }
            files = files.Concat(Directory.GetFiles(Directory.GetCurrentDirectory()));

            if (Path.HasExtension(fileName)) // Suche exakte Datei mit passender Dateiendung
            {
                return files.FirstOrDefault((item) =>
                {
                    var itemWithExtension = Path.GetFileName(item);
                    return itemWithExtension != null && itemWithExtension.Equals(fileName);
                });
            }
            else // Keine Dateiendung vorhanden -> suche nach passendem Dateinamen und beliebiger endung
            {
                return files.FirstOrDefault((item) =>
                {
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(item);
                    return (fileNameWithoutExtension != null && fileNameWithoutExtension.Equals(fileName));
                });
            }
        }

        protected virtual string GetFilePath(string fileName, string directory)
        {
            fileName = ReplaceIllegalCharacters(fileName);

            IEnumerable<string> files = new string[0];
            if (Directory.Exists(directory))
            {
                files = Directory.GetFiles(directory);
            }
            else
            {
                Logger.Warning("The expected driver repository {0} doesn't exist", directory);
                return null;
            }

            if (Path.HasExtension(fileName)) // Suche exakte Datei mit passender Dateiendung
            {
                return files.FirstOrDefault((item) =>
                {
                    var itemWithExtension = Path.GetFileName(item);
                    return itemWithExtension != null && itemWithExtension.Equals(fileName);
                });
            }
            else // Keine Dateiendung vorhanden -> suche nach passendem Dateinamen und beliebiger endung
            {
                return files.FirstOrDefault((item) =>
                {
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(item);
                    return (fileNameWithoutExtension != null && fileNameWithoutExtension.Equals(fileName));
                });
            }
        }

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
                deviceName = device.ToString();
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

            Logger.Warning("Could not load driver for following context:\n{0}", message);
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

        public abstract IDevice GetDeviceInformations(IDevice iDevice);

        public abstract void StoreDeviceInformations(IDevice iDevice);

        public abstract void RestoreDeviceInformations(IDevice iDevice);

        public abstract void RestoreInformationSource();

        public abstract string Name { get; }

        public abstract int Id { get; }

        public abstract Priority Priority { get; set; }

        public abstract bool HasDeviceDriver(IDevice iDevice);

        public override string ToString()
        {
            return !string.IsNullOrEmpty(this.Name) ? Name : base.ToString();
        }
    }
}
