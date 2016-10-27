using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Akomi.InformationModel.Device;
using Akomi.InformationModel.Skills;
using Akomi.Logger;
using Tapako.DeviceInformationManagement.InformationSources;
using Tapako.Framework;

namespace Tapako.Repositories.DeviceDriverRepository
{
    /// <summary>
    ///     Diese Klasse hat informationen darüber "wo" und "wie" das TapakoDevice Driver-Repository aufgebaut ist.
    ///     Das DeviceDriverRepository kann mithilfe der Modellnummer eines Devices vorhandene Treiber lokalisieren
    /// </summary>
    public class DeviceDriverRepository : RepositoryBase
    {
        private MacListRepository _macListRepository;

        private SourcePriority _sourcePriority = SourcePriority.High;

        public DeviceDriverRepository() : base() {}

        public DeviceDriverRepository(string repositoryFolder) : base(repositoryFolder) {}

        public MacListRepository MacListRepository
        {
            get { return _macListRepository ?? (_macListRepository = MacListRepository.GetInstance()); }
            set { _macListRepository = value; }
        }

        public void LoadAllDriverAssemblies()
        {
            foreach (var file in Directory.GetFiles(RepositoryFolder))
            {
                if (file.EndsWith(".dll"))
                {
                    // todo: Load Assembly, not load type
                    Assembly assembly = Assembly.LoadFrom(file);
                    //DllLoader.Load<IDevice>(file);
                }
            }
        }

        public Type LoadType(string typename)
        {
            foreach (var file in Directory.GetFiles(RepositoryFolder))
            {
                if (file.EndsWith(".dll"))
                {
                    // todo: Load Assembly, not load type
                    Assembly assembly = Assembly.LoadFrom(file);
                    try
                    {
                        var result = assembly.GetType(typename);
                        if (result != null)
                        {
                            return result;
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }

            return null;
        }

        /// <summary>
        ///     Instanziiert ein Objekt, welche informationen über das übergeben TapakoDevice enthält
        /// </summary>
        /// <param name="iDevice"></param>
        /// <returns></returns>
        protected override IDevice InnerGetDeviceInformations(IDevice iDevice)
        {
            string filePath = GetFilePath(iDevice);
            if (filePath == null)
            {
                Logger.Warning("{0}: Could not find driver for {1}", Name, iDevice.ToString(true));
                return null;
            }
            IDevice driverObject;

            // At first try to resolve over idevice factory
            var factory = DllLoader.Load<IDeviceCompletement>(filePath);
            if (factory != null)
            {
                driverObject = factory.CompleteDeviceDriver(ref iDevice);
            }

            // if there is no factory try to resolve by directly searching for IDeivce type
            else
            {
                driverObject = DllLoader.Load<IDevice>(filePath);
            }

            return driverObject;
        }

        protected override void InnerStoreDeviceInformations(IDevice iDevice)
        {
            return; // Logic is not saveable
        }

        public override void RestoreDeviceInformations(IDevice iDevice)
        {
            return; // Logic is not saveable
        }

        public override void RestoreInformationSource()
        {
            return; // Logic is not saveable
        }

        public override string Name
        {
            get { return "Device Driver Repository"; }
        }

        public override int Id
        {
            get { return 1; }
        }

        public override bool HasDeviceDriver(IDevice iDevice)
        {
            return File.Exists(GetFilePath(iDevice));
        }

        public override IEnumerable<Uri> GetDriverUris(IDevice device)
        {
            
            if (device == null)
            {
                return base.GetDriverUris(null);
            }
            string path = GetFilePath(device);

            // falls nichts gescheites im string ist und das device eine MAC adrresse hat, versuche Treiber per Mac adresse zu finden
            if (string.IsNullOrWhiteSpace(path) && device.Identification != null && string.IsNullOrWhiteSpace(device.Identification.PhysicalAddress))
            {
                path = GetFilePathFromMac(device.Identification.PhysicalAddress);
            }

            if (!string.IsNullOrWhiteSpace(path))
            {
                return new Uri[] {new System.Uri(path)};
            }

            return null;
        }

        public override SourcePriority SourcePriority
        {
            get { return _sourcePriority; }
            set { _sourcePriority = value; }
        }

        /// <summary>
        ///     Sucht im Repository nach allen vorhandenen PLCSearchDriver und gibt ein Array mit relativen Dateipfaden zurück
        /// </summary>
        /// <returns></returns>
        public static string[] GetArrayOfPlcSearchDriverPaths()
        {
            var files = Directory.GetFiles(Constants.PathPlcSearcherDriverRepository);
            return files;
        }

        /// <summary>
        ///     Öffnet die DLL-Datei unter übergebenem Pfad, erstellt daraus eine PLC-SearchDriver Instanz und gibt diese zurück
        /// </summary>
        /// <param name="searchDriver">Dateipfad der SearchDriver-DLL-Datei</param>
        /// <returns>Type der geladenen DLL-Klasse</returns>
        public static Type LoadPlcSearchDriver(string searchDriver)
        {
            //todo: implement Update or delete
            return typeof (object);
        }

        public static Type LoadCommunicationChannelDriver(string id) //id?
        {
            return null;
        }

        /// <summary>
        ///     Lädt die angegebene Datei
        /// </summary>
        /// <param name="vendorName"></param>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        private static IDevice LoadDeviceDriver(string vendorName, string deviceName)
        {
            var vendor = ReplaceIllegalCharacters(vendorName);
            var device = ReplaceIllegalCharacters(deviceName);
            var path = Path.Combine(Constants.DeviceDriverRepository, vendor, device + ".dll");
            if (File.Exists(path))
            {
                var driver = DllLoader.Load<IDevice>(path);
                return driver;
            }

            return null;
        }


        /// <summary>
        ///     Ermittelt aus den informationen eines IDevices den Pfad, den der Treiber im Repository hat
        /// </summary>
        /// <param name="iDevice"></param>
        /// <returns></returns>
        private string GetFilePath(IDevice iDevice)
        {
            var filePath = string.Empty;

            if (iDevice != null && iDevice.Identification != null &&
                (!string.IsNullOrEmpty(iDevice.Identification.ModelNumber) ||
                 !string.IsNullOrEmpty(iDevice.Identification.PhysicalAddress)))
            {
                // Versuche einen existierenden filePath zu generieren
                if (File.Exists(filePath = GenerateFilePath(iDevice.Identification.ModelNumber)))
                {
                    return filePath;
                }
                if (!string.IsNullOrEmpty(iDevice.Identification.PhysicalAddress) &&
                    File.Exists(filePath = GetFilePathFromMac(iDevice.Identification.PhysicalAddress)))
                {
                    // Wenn der erste Versuch nicht geklappt hat, versuche Die Datei anhand der Mac-Addresse zu finden
                    return filePath;
                }
            }

            if (!File.Exists(filePath))
            {
                LogFailedAttempt(iDevice, filePath);
                return null;
            }
            return filePath;
        }

        private string GetFilePathFromMac(string physicalAddress)
        {
            var fileName = MacListRepository.GetDllName(physicalAddress);

            // Dateinamen ermitteln
            if (string.IsNullOrEmpty(fileName)) // im DLL repository wurde nichts gefunden
            {
                Logger.Warning(
                    "DDR: Device with MAC identification {0} was not found through MAC Repository. Maybe no MAC Repository was loaded.",
                    physicalAddress);
                return null;
            }

            // Dateipfad erstellen
            var filePath = GenerateFilePath(fileName);
            if (!File.Exists(filePath))
            {
                Logger.Warning("DDR: Dll-File \"" + filePath + "\" does not exist");
            }
            else
            {
                Logger.Debug("DDR: DLL-Filename: " + fileName);
            }

            return filePath;
        }
    }
}