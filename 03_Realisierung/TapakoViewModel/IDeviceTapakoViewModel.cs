using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Akomi.InformationModel.Device;
using Akomi.InformationModel.Skills;
using Akomi.Visuals;
using Prism.Commands;

namespace Tapako.ViewModel
{
    public interface IDeviceTapakoViewModel : IDeviceViewModel, INotifyPropertyChanged
    {
        /// <summary>
        /// Returns new ViewModels for all Subdevices of current <see cref="DeviceModel"/>
        /// </summary>
        ObservableCollection<IDeviceTapakoViewModel> SubDeviceViewModels { get; }

        /// <summary>
        /// The ViewModel of the attached server
        /// </summary>
        OpcUaServerControlViewModel ServerViewModel { get; }

        /// <summary>
        /// Used to add primitive devices
        /// </summary>
        DelegateCommand RunPrimitiveCommunicationChannelDriverCommand { get; }

        /// <summary>
        /// Saves changed IR Data to the corresponding information sources by calling standard interfaces.
        /// </summary>
        DelegateCommand SaveDeviceDataInInformationSourcesCommand { get; }

        /// <summary>
        /// Scannt alle am Host angeschlossenen Geräte
        /// </summary>
        DelegateCommand AnalyseDeviceCommand { get; }

        /// <summary>
        /// Scannt alle am Host angeschlossenen Geräte
        /// </summary>
        DelegateCommand CancelDeviceAnalysisCommand { get; }

        /// <summary>
        /// Speichert das Device im serialisierten zustand ab
        /// </summary>
        DelegateCommand SaveDeviceOnLocalDiscCommand { get; }

        /// <summary>
        /// Gibt alle commands des ViewModels zurück
        /// </summary>
        /// <returns></returns>
        IEnumerable<DelegateCommandBase> GetCommands();

        bool HasDeviceDriver(IDevice device);


    }
}
