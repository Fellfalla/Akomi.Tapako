using System;
using System.IO;
using System.Linq;
using System.Reflection;
using IAkomiDevice.fIDevice;
using TapakoInterfaces;
using TapakoServices;

namespace TapakoModel
{
    /// <summary>
    /// Diese Klasse hat informationen darüber "wo" und "wie" das TapakoDevice Driver-Repository aufgebaut ist.
    /// Das DeviceDriverRepository kann mithilfe der Modellnummer eines Devices vorhandene Treiber lokalisieren
    /// </summary>
    public class DeviceDriverRepository: IInformationSource
    {

        /// <summary>
        /// Sucht im Repository nach allen vorhandenen PLCSearchDriver und gibt ein Array mit relativen Dateipfaden zurück
        /// </summary>
        /// <returns></returns>
        public static String[] GetArrayOfPlcSearchDriverPaths()
        {
            String[] files = Directory.GetFiles(Constants.PathPlcSearcherDriverRepository);
            return files;
        }

        /// <summary>
        /// Öffnet die DLL-Datei unter übergebenem Pfad, erstellt daraus eine PLC-SearchDriver Instanz und gibt diese zurück
        /// </summary>
        /// <param name="searchDriver">Dateipfad der SearchDriver-DLL-Datei</param>
        /// <returns>Type der geladenen DLL-Klasse</returns>
        public static Type LoadPlcSearchDriver(String searchDriver)
        {
            //todo: implement Update: oder auch nicht wenn keine PlcSearchDriver verwendet werden
            return typeof(object);
        }

        public static Type LoadCommunicationChannelDriver(String id)//id?
        {
            return null;
        }

        /// <summary>
        /// Lädt die angegebene Datei
        /// </summary>
        /// <param name="vendorName"></param>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        private static IDevice LoadDeviceDriver(string vendorName, string deviceName)
        {
            string vendor = ReplaceIllegalCharacters(vendorName);
            string device = ReplaceIllegalCharacters(deviceName);
            string path = Path.Combine(Constants.DeviceDriverRepository, vendor, device + ".dll");
            if (File.Exists(path))
            {
                var driver = DllLoader.Load<IDevice>(path);
                return driver;
            }

            return null;
        }

        /// <summary>
        /// Replaces special characters with an "_". Because filename shouldn't contain "?\/|:"...
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReplaceIllegalCharacters(string path)
        {
            return Constants.ValidCharactersRegex.Replace(path, Constants.ReplacingChar.ToString());
        }

        /// <summary>
        /// Instanziiert ein Objekt, welche informationen über das übergeben TapakoDevice enthält
        /// </summary>
        /// <param name="iDevice"></param>
        /// <returns></returns>
        public IDevice GetDeviceInformation(IDevice iDevice)
        {
            return DllLoader.Load<IDevice>(GetFilePath(iDevice)); 
        }

        /// <summary>
        /// Ermittelt aus den informationen eines IDevices den Pfad, den der Treiber im Repository hat
        /// </summary>
        /// <param name="iDevice"></param>
        /// <returns></returns>
        private string GetFilePath(IDevice iDevice)
        {
            string deviceName = iDevice.Identification.SerialNumber;
            string device = ReplaceIllegalCharacters(deviceName);
            string path = Path.Combine(Constants.DeviceDriverRepository, device + ".dll");
            return path;
        }

    }
}
