using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using Akomi.InformationModel.Device;
using Akomi.InformationModel.Enums;
using Akomi.Logger;
using Akomi.Utilities;
using ExtensionMethodsCollection;
using Prism.Commands;
using Prism.Logging;
using Prism.Mvvm;
using Tapako.DeviceInformationManagement;
using Tapako.DeviceInformationManagement.IO;
using Tapako.Framework;
using Tapako.Repositories.DeviceDriverRepository;
using Tapako.Repositories.VirtualRepresentationRepository;
using Tapako.Utilities.UniversalHostSearch;

namespace Tapako.ViewModel
{
    /// <summary>
    /// The main ViewModel of Tapako. It Handles Informations between the Tapako BL and the Tapako GUI
    /// More Informations on http://www.wpftutorial.net
    /// </summary>
    public class TapakoViewModel : BindableBase, ITapakoViewModel
    {
        #region Fields

        /// <summary>
        /// Observable Collection, welche alle LogMessages enthält
        /// </summary>
        private ObservableCollection<Message> _logMessages;
        //private DispatchingObservableCollection<Message> _debugMessages = new DispatchingObservableCollection<Message>();
        //private DispatchingObservableCollection<Message> _infoMessages = new DispatchingObservableCollection<Message>();
        //private DispatchingObservableCollection<Message> _warnMessages = new DispatchingObservableCollection<Message>();
        //private DispatchingObservableCollection<Message> _errorMessages = new DispatchingObservableCollection<Message>();

        /// <summary>
        /// Das aktuell selektierte HostDevice
        /// </summary>
        private IDeviceTapakoViewModel _selectedHostDeviceTapako;

        /// <summary>
        /// Main TapakoViewModel Dispatcher
        /// </summary>
        private readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;

        private bool _filterInfoMessages;
        private bool _filterDebugMessages = true;
        private bool _filterWarningMessages;
        private bool _filterErrorMessages;

        private List<DelegateCommandBase> _commands = new List<DelegateCommandBase>();
        private HostSearchViewModel _hostSearchViewModel;

        #endregion

        #region Properties

        public bool FilterInfoMessages
        {
            get { return _filterInfoMessages; }
            set
            {
                SetProperty(ref _filterInfoMessages, value);
                RefreshFilteredLogMessages();
            }
        }

        public bool FilterDebugMessages
        {
            get { return _filterDebugMessages; }
            set
            {
                SetProperty(ref _filterDebugMessages, value);
                RefreshFilteredLogMessages();
            }
        }

        public bool FilterWarningMessages
        {
            get { return _filterWarningMessages; }
            set
            {
                SetProperty(ref _filterWarningMessages, value);
                RefreshFilteredLogMessages();
            }
        }

        public bool FilterErrorMessages
        {
            get { return _filterErrorMessages; }
            set
            {
                SetProperty(ref _filterErrorMessages, value);
                RefreshFilteredLogMessages();
            }
        }

        public ObservableCollection<Message> LogMessages
        {
            get { return _logMessages; }
            set
            {
                lock (_logMessagesLock)
                {
                    SetProperty(ref _logMessages, value);
                    RefreshFilteredLogMessages();
                }
           
                //if (FilteredLogMessages != null) FilteredLogMessages.Refresh();
            }
        }

        private void RefreshFilteredLogMessages()
        {
            if (FilteredLogMessages == null)
            {
                return;
            }

            FilteredLogMessages.Refresh();
        }

        /// <summary>
        /// Diese Property ermöglicht es die LogMessages zu filtern
        /// Source:
        /// </summary>
        public ICollectionView FilteredLogMessages { get; set; }


        public ObservableCollection<IDeviceTapakoViewModel> HostDeviceList
        {
            get { return HostSearchViewModel.NetworkDevices; }
            //set { SetProperty(ref HostSearchViewModel.NetworkDevices, value); }
        }

        public bool IsBusy
        {
            get
            {
                // return true if any command is running, any progress step is in progress, or the load device command is running
                return Commands.Any(command => command.IsActive) ||
                       TapakoProgress.Steps.Any(pair => pair.Value.Equals(ProgressState.InProgress)) ||
                       LoadDeviceCommand.IsActive;
            }
        }

        /// <summary>
        /// Aktuell ausgewähltes HostDevice
        /// </summary>
        public IDeviceTapakoViewModel SelectedHostDeviceTapako
        {
            get { return _selectedHostDeviceTapako; }
            set { SetProperty(ref _selectedHostDeviceTapako, value); }
        }

        public HostSearchViewModel HostSearchViewModel
        {
            get { return _hostSearchViewModel; }
            set
            {
                if (_hostSearchViewModel != null)
                {
                    _hostSearchViewModel.NetworkDevices.CollectionChanged -= HostDeviceListChanged;
                }

                _hostSearchViewModel = value;

                if (_hostSearchViewModel != null)
                {
                    _hostSearchViewModel.NetworkDevices.CollectionChanged += HostDeviceListChanged;
                }

            }
        }

        private void HostDeviceListChanged(object sender, EventArgs args)
        {
            OnPropertyChanged(Nameof<TapakoViewModel>.Property(x => x.HostDeviceList));
        }

        public DelegateCommand LoadDeviceCommand { get; set; }

        public UniversalHostSearchViewModel UniversalHostSearchViewModel { get; set; }

        #endregion

        private object _logMessagesLock = new object();

        /// <summary>
        /// Default Constructor
        /// </summary>
        public TapakoViewModel(HostSearchViewModel hostSearchViewModel)
        {
            HostSearchViewModel = hostSearchViewModel;
            hostSearchViewModel.SearchViewModel.Subnet = Constants.DefaultSubnet;
            hostSearchViewModel.HostDeviceSelected += (sender, viewModel) => SelectedHostDeviceTapako = viewModel;

            // register IsBusy for all Commands
            HostDeviceList.CollectionChanged += RefreshCommands;

            // Setup Logger
            //LogMessages = new ObservableCollection<Message>();

            LogMessages = new ObservableCollection<Message>();
            
            //var context = SynchronizationContext.Current;
            //context.IsWaitNotificationRequired();
            Logger.NewLogMessage += OnNewLogMessage;
            //Logger.NewLogMessage += (sender, message) => { context.Post(delegate {LogMessages.Add(message);}, null);};
            //BindingOperations.EnableCollectionSynchronization(LogMessages, LogMessages.Lock); // This is very importan for thread-safety of log messages
            BindingOperations.CollectionRegistering += BindingOperationsOnCollectionRegistering;
            FilteredLogMessages = CollectionViewSource.GetDefaultView(LogMessages);
            FilteredLogMessages.Filter += FilteredLogMessagesOnFilter;
            
            // Setup progress system
            TapakoProgress.ProgressChanged += (sender, args) => _dispatcher.DoDispatchedAction(RaiseBusyStateChanged);

            // Set view State depending on server States
            //var timer = new Timer(100);
            //timer.Elapsed += (sender, args) => OnNewLogMessage(null, new Message() {Value = "Test Message", MessageType = Category.Info});
            //timer.Enabled = true;
            //timer.Start();
            // Assign Commands
            LoadDeviceCommand = new DelegateCommand(LoadDevice);
            #region Register InformationSources

            DeviceInformationManager.RegisterInformationSource(
                new DeviceDriverRepository(Constants.DeviceDriverRepository));

            DeviceInformationManager.RegisterInformationSource(
                new VirtualRepresentationRepository(Constants.DeviceDriverRepository));

            //DeviceInformationManager.RegisterOnlineSources();

            #endregion Register InformationSources
        }

        private void BindingOperationsOnCollectionRegistering(object sender, CollectionRegisteringEventArgs collectionRegisteringEventArgs)
        {
            if (object.ReferenceEquals(collectionRegisteringEventArgs.Collection, LogMessages))
            {
                BindingOperations.EnableCollectionSynchronization((IEnumerable) LogMessages, _logMessagesLock);
            }
        }

        private async void OnNewLogMessage(object sender, Message message)
        {
            //switch (message.MessageType) // no idea for what this is good atm
            //{
            //    case Category.Debug:
            //        DebugMessages.Add(message);
            //        break;
            //    case Category.Info:
            //        InfoMessages.Add(message);
            //        break;
            //    case Category.Warn:
            //        WarnMessages.Add(message);
            //        break;
            //    case Category.Exception:
            //        ErrorMessages.Add(message);
            //        break;
            //}

            //var syncThread = new Thread(new ThreadStart(() => LogMessages.Add(message)));
            //syncThread.IsBackground = true;

            //syncThread.Start();

            //LogMessages.Add(message);
            //await Task.Run(() => _dispatcher.Invoke((() => LogMessages.Add(message)))).ConfigureAwait(false);

            await Task.Factory.StartNew(() =>
            {
                lock (_logMessagesLock)
                {
                    _logMessages.Add(message);
                }
            });

            //if (_dispatcher.CheckAccess())
            //{
            //    LogMessages.Add(message);
            //}
            //else
            //{
            //    await _dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,(Action) (() => LogMessages.Add(message)));
            //}
            //var a = _dispatcher.InvokeAsync(() => LogMessages.Add(message));
            //await a.Task.ConfigureAwait(false);
        }


        private void LoadDevice()
        {
            try
            {
                //var device = StorageModule.LoadFromFile<DeviceBase>();
                IDevice device;
                var task = Task.Factory.StartNew(() =>
                {
                    LoadDeviceCommand.IsActive = true;
                    RaiseBusyStateChanged();
                    device = StorageModule.LoadFromFile<DeviceBase>();
                    if (device == null) return;
                    Application.Current.Dispatcher.BeginInvoke(new Action(() => AddNewDevice(device)));
                });
                task.ContinueWith(t =>
                {
                    LoadDeviceCommand.IsActive = false;
                    RaiseBusyStateChanged();
                });
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void AddNewDevice(IDevice device)
        {
            var viewModel = new DeviceTapakoViewModel(device);
            //viewModel.DeletionRequest += RemoveDevice; // todo: Add Deletion Handler
            HostDeviceList.Add(viewModel);
        }

        /// <summary>
        /// Aktualisiert <see cref="Commands"/> auf basis der Event Argumente
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="notifyCollectionChangedEventArgs"></param>
        private void RefreshCommands(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            // todo: Möglichkeit hinzufügen Subdevices zu subscripten
            if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var deviceViewModel in notifyCollectionChangedEventArgs.NewItems.OfType<IDeviceTapakoViewModel>())
                {
                    foreach (var command in deviceViewModel.GetCommands())
                    {
                        command.IsActiveChanged += RaiseBusyStateChanged;
                        Commands.Add(command);
                    }
                }
            }
            else if (notifyCollectionChangedEventArgs.OldItems != null)
            {
                foreach (var deviceViewModel in notifyCollectionChangedEventArgs.OldItems.OfType<IDeviceTapakoViewModel>())
                {
                    foreach (var command in deviceViewModel.GetCommands())
                    {
                        command.IsActiveChanged -= RaiseBusyStateChanged;
                        Commands.Remove(command);
                    }
                }
            }

            //manuell hinzugefügt, damit beim Laden eines Devices Progressbar animiert wird
            Commands.Add(LoadDeviceCommand);
        }

        public List<DelegateCommandBase> Commands
        {
            get { return _commands; }
            set { _commands = value; }
        }

        //public DispatchingObservableCollection<Message> DebugMessages
        //{
        //    get { return _debugMessages; }
        //    set { SetProperty(ref _debugMessages, value); }
        //}

        //public DispatchingObservableCollection<Message> InfoMessages
        //{
        //    get { return _infoMessages; }
        //    set { SetProperty(ref _infoMessages, value); }
        //}

        //public DispatchingObservableCollection<Message> WarnMessages
        //{
        //    get { return _warnMessages; }
        //    set { SetProperty(ref _warnMessages, value); }
        //}

        //public DispatchingObservableCollection<Message> ErrorMessages
        //{
        //    get { return _errorMessages; }
        //    set { SetProperty(ref _errorMessages, value); }
        //}

        #region private Methods

        private void RaiseBusyStateChanged()
        {
            OnPropertyChanged(Nameof<TapakoViewModel>.Property((obj) => obj.IsBusy));
        }

        private void RaiseBusyStateChanged(object sender, EventArgs args)
        {
            OnPropertyChanged(Nameof<TapakoViewModel>.Property((obj) => obj.IsBusy));
        }

        private bool FilteredLogMessagesOnFilter(object item)
        {
            if (item != null)
            {
                switch (((Message) item).MessageType)
                {
                    case Category.Info:
                        return !FilterInfoMessages;
                    case Category.Debug:
                        return !FilterDebugMessages;
                    case Category.Warn:
                        return !FilterWarningMessages;
                    case Category.Exception:
                        return !FilterErrorMessages;
                }
            }
            return false;
        }

        private static void ExceptionHandler(Task task)
        {
            var exception = task.Exception;
            Console.WriteLine(exception);
        }

        #endregion


    }


}