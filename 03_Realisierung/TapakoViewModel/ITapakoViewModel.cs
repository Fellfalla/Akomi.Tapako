using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Akomi.InformationModel.Device;
using Akomi.Logger;
using Prism.Commands;

namespace Tapako.ViewModel
{

    public interface ITapakoViewModel : INotifyPropertyChanged
    {
        #region Properties
        ObservableCollection<Message> LogMessages { get; set; }

        ICollectionView FilteredLogMessages { get; }

        ObservableCollection<IDeviceTapakoViewModel> HostDeviceList { get; }

        bool FilterInfoMessages { get; set; }
        bool FilterDebugMessages { get; set; }
        bool FilterWarningMessages { get; set; }
        bool FilterErrorMessages { get; set; }

        /// <summary>
        /// Dieser String spezifiziert den IP-Bereich, in dem nach NGD gesucht werden soll
        /// </summary>
        //string Subnet { get; set; }

        /// <summary>
        /// Indicates if some commands are currently running
        /// </summary>
        bool IsBusy { get; }

        /// <summary>
        /// Aktuell ausgewähltes HostDevice
        /// </summary>
        IDeviceTapakoViewModel SelectedHostDeviceTapako { get; set; }

        //ObservableCollection<IDevice> MasterDevices { get; }

        #region ViewModels

        HostSearchViewModel HostSearchViewModel { get; }

        #endregion ViewModels

        #endregion

        #region Commands


        /// <summary>
        /// Lädt ein serialisiertes <see cref="IDevice"/>.
        /// </summary>
        DelegateCommand LoadDeviceCommand { get; }

        #endregion

        #region Methods


        #endregion
    }
}