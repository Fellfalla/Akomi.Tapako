using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Threading;
using Akomi.InformationModel.Device;
using Akomi.Logger;
using ExtensionMethodsCollection;
using Prism.Mvvm;
using Tapako.DeviceInformationManagement;
using Tapako.Utilities.UniversalHostSearch;

namespace Tapako.ViewModel
{
    public class HostSearchViewModel : BindableBase, IHostSearchViewModel
    {
        private readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;
        private IDeviceTapakoViewModel _selectedDeviceTapakoViewModel;
        private ObservableCollection<IDeviceTapakoViewModel> _networkDevices;

        public HostSearchViewModel(UniversalHostSearchViewModel searchViewModel)
        {
            Intialize();

            SearchViewModel = searchViewModel;
            // Register Events

            // Add and remove found devices from right thread
            SearchViewModel.NewNetworkDeviceFound += (sender, device) =>
            {
                _dispatcher.DoDispatchedAction(() => AddDevice(device));
            };
            //SearchViewModel.NewNetworkDeviceFound += (sender, Device) => AddDevice(Device);
            //SearchViewModel.NetwokDeviceDisappeared += (sender, device) => _dispatcher.DoDispatchedAction(RemoveDevice, device);
            //SearchViewModel.NetwokDeviceDisappeared += (sender, Device) => RemoveDevice(Device);
            
       }

        public void Intialize()
        {
            NetworkDevices = new ObservableCollection<IDeviceTapakoViewModel>();
        }

        public void AddDevice(IDevice device)
        {
            if (NetworkDevices.Any(existingDevice => existingDevice.DeviceModel.HasEqualNetworkInformation(device)))
            {
                return; // Do not add, if device with equal network information is already added
            }

            var newViewModel = new DeviceTapakoViewModel(device);

            using (new MutedLogger())
            {
                //device = DeviceInformationManager.CompleteDeviceInformation(device);     
                CompleteVirtualRepresentation(newViewModel);
            }

            NetworkDevices.Add(newViewModel);
            
            //if (NetworkDeviceAdded != null) NetworkDeviceAdded(this, newViewModel);

        }

        private async void CompleteVirtualRepresentation(IDeviceTapakoViewModel tapakoViewModel) // this method has heavy performance impact
        {
            await DeviceInformationManager.AsyncCompleteDeviceInformation(tapakoViewModel.DeviceModel);
            var index = NetworkDevices.IndexOf(tapakoViewModel);

            if (index == -1) // Device was already removed
            {
                return;
            }

            // Remove and add to update the View through CollectionChangedEvent
            NetworkDevices.Remove(tapakoViewModel);
            NetworkDevices.Insert(index, tapakoViewModel);
        }


        public void RemoveDevice(IDevice device)
        {
            // Iterate over all models and find thoose who wer not found
            // Conversion ToArray is important for iterating over non-changing collection
            foreach (var viewModel in NetworkDevices.ToArray().Where((viewModel) => viewModel.DeviceModel == device))
            {
                NetworkDevices.Remove(viewModel);
                //if (NetworkDeviceRemoved != null) NetworkDeviceRemoved(this, viewModel);
            }
        }


        public ObservableCollection<IDeviceTapakoViewModel> NetworkDevices
        {
            get { return _networkDevices; }
            private set
            {
                if (_networkDevices != null)
                {
                    _networkDevices.CollectionChanged -= OnNetworkDeviceCollectionChanged;
                }

                _networkDevices = value;

                if (_networkDevices != null)
                {
                    _networkDevices.CollectionChanged += OnNetworkDeviceCollectionChanged;
                }
            }
        }

        private void OnNetworkDeviceCollectionChanged (object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.NewItems != null && args.NewItems.Any())
            {
                foreach (var newItem in args.NewItems.OfType<IDeviceTapakoViewModel>())
                {
                    newItem.DeletionRequest += RemoveDevice; // establish deletion of devices
                    if (NetworkDeviceAdded != null) NetworkDeviceAdded(this, newItem);
                }
            }
            if (args.OldItems != null && args.OldItems.Any())
            {
                foreach (var oldItem in args.OldItems.OfType<IDeviceTapakoViewModel>())
                {
                    if (NetworkDeviceRemoved != null) NetworkDeviceRemoved(this, oldItem);
                }
            }

        }

        private void RemoveDevice(object sender, EventArgs e)
        {
            var vm = sender as IDeviceTapakoViewModel;
            if (vm != null)
            {
                vm.DeletionRequest -= RemoveDevice; // Unregister
                NetworkDevices.Remove(vm);
            }
        }

        public IDeviceTapakoViewModel SelectedDeviceTapakoViewModel
        {
            get { return _selectedDeviceTapakoViewModel; }
            set
            {
                SetProperty(ref _selectedDeviceTapakoViewModel, value);
                if (HostDeviceSelected != null) HostDeviceSelected(this, value);
            }
        }

        public event EventHandler<IDeviceTapakoViewModel> NetworkDeviceAdded;

        public event EventHandler<IDeviceTapakoViewModel> NetworkDeviceRemoved;

        public event EventHandler<IDeviceTapakoViewModel> HostDeviceSelected;

        public UniversalHostSearchViewModel SearchViewModel { get; private set; }


    }
}
