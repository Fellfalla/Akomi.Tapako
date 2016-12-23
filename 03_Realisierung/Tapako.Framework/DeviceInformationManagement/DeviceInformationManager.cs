using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Akomi.InformationModel.Device;
using Akomi.Logger;
using ExtensionMethodsCollection;
using Tapako.DeviceInformationManagement.InformationSources;
using Tapako.Framework;
using Tapako.ObjectMerger;

namespace Tapako.DeviceInformationManagement
{
    /// <summary>
    /// Diese Klasse nimmt IDevices an und vervollständigt die darin enthaltenen Informationen mithilfe verschiedener Wissensquellen
    /// 
    /// Das Übergebene Device wird rekursiv durchgegangen mit folgenden Regeln:
    /// - alle neuen Primitiven Datentypen werden kopiert
    /// - alle noch nicht vorhandenen Höheren Datentypen werden kopiert
    /// - alle bereits vorhandenen Listen werden verkettet
    /// - alle bereits vorhandenen Höheren Datentypen werden rekursiv kombiniert
    /// 
    /// todo: Erzeuge Variable Bitmaske um das Merge-Verhalten von außen steuern zu können ( ausschließen von felder, nur public, private, etc.) 
    /// todo: IsMergeable auf false soll Listen als true ansehen
    /// todo: Astübergreifende und Zyklische referenzen Erkennen und ersetzen. Problem sind Unmergable objecte (diese durchsuchen) und Mergeable, wg. mehrmaligem Merging
    /// </summary>
    public static class DeviceInformationManager
    {
        static DeviceInformationManager()
        {
            InformationSources = new List<IInformationSource>();
            ObjectMergerLogger.SetInstance(new ObjectMergerLoggerAdapter());
        }
        #region Fields
        private static Stopwatch stopwatch = new Stopwatch();

        #endregion

        #region Properties

        /// <summary>
        /// Liste der instanziierten und registrierten Informationsquellen
        /// </summary>
        public static List<IInformationSource> InformationSources { get; private set; }


        #endregion

        /// <summary>
        /// Gibt an, ob ein Treiber zum gegebenen Device in einer der Informationsources vorhanden ist
        /// </summary>
        /// <returns>true, fall ein Treiber existiert</returns>
        public static bool IsDriverAvailable<T>(T device) where T : IDevice
        {
            using (new MutedLogger())
            {
                return InformationSources.Any(informationSource => informationSource.HasDeviceDriver(device));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <returns>All <see cref="Uri"/>s for drivers of <paramref name="device"/></returns>
        public static IEnumerable<Uri> GetDriverUris(IDevice device)
        {
            foreach (var informationSource in InformationSources)
            {
                //IEnumerable<Uri> result = null;
                //try
                //{
                    foreach (var result in informationSource.GetDriverUris(device))
                    {
                        if (result != null)
                        {
                            yield return result;
                        }
                    }
                //}
                //catch (NotImplementedException) { }
               
                
            }
        }

        /// <summary>
        /// Invokes storing device data of all registered <see cref="IInformationSource"/>s.
        /// Each <see cref="IInformationSource"/> will decide which Informations will be stored by itself
        /// </summary>
        /// <param name="device"></param>
        /// <typeparam name="T"></typeparam>
        public static void StoreDeviceInformations<T>(T device) where T : IDevice
        {
            foreach (var informationSource in InformationSources)
            {
                try
                {
                    informationSource.StoreDeviceInformations(device);
                }
                catch (NotImplementedException)
                {
                    Logger.Info("{0} is not implemented in {0}", FunctionCollection.GetCurrentMethod(), informationSource);
                }
            }
        }

        /// <summary>
        /// Restores the informations about the devie in the <see cref="IInformationSource"/>.
        /// This action will undo all user customization on device data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="device"></param>
        public static void RestoreDeviceInformations<T>(T device) where T : IDevice
        {
            foreach (var informationSource in InformationSources)
            {
                try
                {
                    informationSource.RestoreDeviceInformations(device);
                }
                catch (NotImplementedException)
                {
                    Logger.Info("{0} is not implemented in {0}", FunctionCollection.GetCurrentMethod(),
                        informationSource);
                }
            }
        }

        /// <summary>
        /// Restores factory default data of all registered <see cref="IInformationSource"/>s
        /// </summary>
        public static void RestoreInformationSources()
        {
            foreach (var informationSource in InformationSources)
            {
                try
                {
                    informationSource.RestoreInformationSource();
                }
                catch (NotImplementedException)
                {
                    Logger.Info("{0} is not implemented in {0}", FunctionCollection.GetCurrentMethod(),
                        informationSource);
                }
            }
        }

        /// <summary>
        /// Runs <see cref="CompleteDeviceInformation{T}"/> asynchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="device"></param>
        /// <returns></returns>
        public static async Task<T> AsyncCompleteDeviceInformation<T>(T device) where T: IDevice
        {
            return await Task.Run(() => CompleteDeviceInformation(device));
        }

        /// <summary>
        /// Vervollständigt das übergebene Device mit Zusatzinformationen aus den Registrierten informationsquellen
        /// </summary>
        /// <param name="iDevice"></param>
        /// todo: call with ref iDevice -> caller variable can change reference
        /// <returns></returns>
        public static T CompleteDeviceInformation<T>(T iDevice) where T : IDevice // todo: auf allgemeine Datentypen erweitern
        {
            if (iDevice == null)
            {
                return default(T);
            }
            stopwatch.Start();
            foreach (var informationSource in InformationSources)
            {
                try
                {
                    var newIDevice = informationSource.GetDeviceInformations(iDevice);

                    // Merge if information source was of instanziating type
                    if (!ReferenceEquals(newIDevice, iDevice)) 
                    {

                        iDevice = ObjectMerger.ObjectMerger.MergeObjects(iDevice, newIDevice);
                    }
                }
                catch (NotImplementedException e)
                {
                    Logger.Warning(e.ToString());
                }
                catch (TargetParameterCountException exception)
                {
                    Logger.Error("TargetParameterCountException in method {0}: {1}", exception.TargetSite.Name, exception.ToString(true));
                }
                catch (Exception exception)
                {
                    Logger.Error(exception.ToString(true));
                }

                //iDevice = MergeObjects<T>(iDevice, newIDevice);
                // Hier ist die reihenfolge kritisch - Denn unmergeable Strategy hält eine referenz auf sein parent - Stand vom 01.10.2015
                // Stand vom 19.10.2015 -> Referenz auf DeviceParent wird per parameter übergeben -> keine referenzumbiegung notwendig, Property aktuell noch in Strategy
            }
            stopwatch.Stop();
            Logger.Debug("Completing \"{0}\" took {1} ms", iDevice, stopwatch.ElapsedMilliseconds);
            return iDevice;
        }



        /// <summary>
        /// Nimmt eine neue InformationsQuelle in die Liste der informationsquelle auf
        /// </summary>
        /// <param name="informationSource"></param>
        /// <returns>true if the source has bee registered</returns>
        public static bool RegisterInformationSource(IInformationSource informationSource)
        {
            if (InformationSources.All(registeredSource => registeredSource.Id != informationSource.Id && !registeredSource.Name.Equals(informationSource.Name, StringComparison.OrdinalIgnoreCase)))
            {
                InformationSources.Add(informationSource);

                // Die Liste wird nun Sortiert, damit IInformationSources mit hoher Priorität zuerst ausgewertet werden
                InformationSources.Sort(new InformationSourceComparer());
                return true;
            }
            Logger.Debug("The information source {0} with name \"{1}\" and id \"{2}\" has not been registered correctly. Maybe the same source is already registered.", informationSource ,informationSource.Name, informationSource.Id);
            return false;
        }

        /// <summary>
        /// Entfernt eine vorhandene InformationsQuelle aus der Liste der informationsquellen
        /// </summary>
        /// <param name="informationSource"></param>
        public static void UnregisterInformationSource(IInformationSource informationSource)
        {
            if (InformationSources.Contains(informationSource))
            {
                InformationSources.Remove(informationSource);
            }
        }

        /// <summary>
        /// Entfernt alle vorhandenen InformationsQuelle aus der Liste der informationsquellen
        /// </summary>
        public static void UnregisterAllInformationSources()
        {
            InformationSources.Clear();
        }

        /// <summary>
        /// Unregisters all registered <see cref="IInformationSource"/>s
        /// </summary>
        public static void CleanUp()
        {
            UnregisterAllInformationSources();
        }

    }
}