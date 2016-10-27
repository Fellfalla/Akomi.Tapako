using System;
using System.Collections;
using System.Collections.Generic;
using Akomi.InformationModel.Device;

namespace Tapako.Utilities.DeviceSelector.ViewModel
{
    interface IDeviceSelectorViewModel
    {
        /// <summary>
        /// Modellnummer, welche angibt, welches Gerätemodell angegeben wird
        /// </summary>
        string ModelNumber { get; set; }        
        
        /// <summary>
        /// Seriennummer, welche angibt, welches Geräteinstanz angegeben wird.
        /// Diese Nummer identifiziert eine reale Instanz eines Gerätes eindeutig
        /// </summary>
        string SerialNumber { get; set; }

        IEnumerable GetDeviceModelNames(Type type);

        IEnumerable<string> ModelSuggestions { get; set; } 

        Dictionary<string, List<string>> SerialNumberSuggestions { get; set; }

        bool OverrideModelSuggestions { get; set; }

        IDevice ParentDevice { get; set; }

        void Reset();
    }
}
