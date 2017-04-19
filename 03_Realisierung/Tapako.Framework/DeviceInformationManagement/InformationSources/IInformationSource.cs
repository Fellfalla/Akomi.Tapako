using System;
using System.Collections.Generic;
using Akomi.InformationModel.Device;

namespace Tapako.DeviceInformationManagement.InformationSources
{
    /// <summary>
    /// Allgemeine Schnittstelle für eine Informationsquelle
    /// todo: Add abstract method um equal implementieren zu können : ID sollte verglichen werden
    /// </summary>
    public interface IInformationSource
    {
        /// <summary>
        /// Gibt eine neue Instanz eines iDevices mit neuen Informationen zu dem übergebenen iDevice zurück
        /// </summary>
        /// <param name="iDevice"></param>
        /// <returns>null, falls kein Treiber vorhanden ist</returns>
        IDevice GetDeviceInformations(IDevice iDevice);

        /// <summary>
        /// Speichert zum informationSource passende informationen einer Instanz ab
        /// </summary>
        /// <param name="iDevice"></param>
        /// <returns>null, falls kein Treiber vorhanden ist</returns>
        void StoreDeviceInformations(IDevice iDevice);

        /// <summary>
        /// Stellt die Ursprungsinformationen des gegebenen iDevice wieder her
        /// </summary>
        /// <param name="iDevice"></param>
        void RestoreDeviceInformations(IDevice iDevice);

        /// <summary>
        /// Stellt den Ursprungszustand des kompletten Information Sources wieder her
        /// </summary>
        void RestoreInformationSource();

        /// <summary>
        /// Removes all informations the Repository has about <paramref name="iDevice"/>
        /// </summary>
        /// <param name="iDevice"></param>
        void RemoveDeviceInformations(IDevice iDevice);

        /// <summary>
        /// Name of the Repository
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Unique Id to identify the Repository
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Überprüft ob Informationen für das übergebene Device im Information Source vorhanden sind.
        /// </summary>
        /// <param name="iDevice"></param>
        /// <returns>true if informations are available</returns>
        bool HasDeviceDriver(IDevice iDevice);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <returns><see cref="Uri"/> for locating the driver file of <paramref name="device"/></returns>
        IEnumerable<Uri> GetDriverUris(IDevice device);

        /// <summary>
        /// Die Priorität gibt an, in welcher Reihenfolge Device Informationen bevorzugt geladen werden.
        /// High bedeutet vorranging.
        /// Medium bedeutet zweitrangig.
        /// Low bedeutet drittrangig.
        /// </summary>
        SourcePriority SourcePriority { get; set; }
    }
}
