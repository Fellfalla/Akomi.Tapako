using System;
using System.Collections.ObjectModel;
using Tapako.Utilities.UniversalHostSearch;

namespace Tapako.ViewModel
{
    public interface IHostSearchViewModel
    {
        ObservableCollection<IDeviceTapakoViewModel> NetworkDevices { get; }

        IDeviceTapakoViewModel SelectedDeviceTapakoViewModel { get; set; }

        event EventHandler<IDeviceTapakoViewModel> NetworkDeviceAdded;

        event EventHandler<IDeviceTapakoViewModel> NetworkDeviceRemoved;

        event EventHandler<IDeviceTapakoViewModel> HostDeviceSelected;

        UniversalHostSearchViewModel SearchViewModel { get; }
    }
}
