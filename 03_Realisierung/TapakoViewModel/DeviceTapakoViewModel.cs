using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Akomi.InformationModel.Component.Presentation;
using Akomi.InformationModel.Device;
using Akomi.InformationModel.Skills;
using Akomi.InformationModel.Skills.SkillCatalogue;
using Akomi.Logger;
using Akomi.Utilities.Extended;
using Prism.Commands;
using Tapako.DeviceInformationManagement;
using Tapako.DeviceInformationManagement.IO;
using Tapako.Services;
using ExtensionMethodsCollection;
using Prism.Mvvm;

namespace Tapako.ViewModel
{
    public class DeviceTapakoViewModel : BindableBase, IDeviceTapakoViewModel
    {
        public DeviceTapakoViewModel()
        {
            _subDeviceViewModels = new ObservableCollection<IDeviceTapakoViewModel>();

            _componentDictionary = new Dictionary<string, object>();

            AnalyseDeviceCommand = DelegateCommand.FromAsyncHandler(AnalyseDevice, DeviceModelIsNotNull);
            
            CancelDeviceAnalysisCommand = new DelegateCommand(CancelAnalysis, CanInvokeCancelDeviceAnalysisCommand);

            SaveDeviceOnLocalDiscCommand = new DelegateCommand(SaveDevice, DeviceModelIsNotNull);

            SaveDeviceDataInInformationSourcesCommand = new DelegateCommand(SaveToInformationSources, DeviceModelIsNotNull);

            RunPrimitiveCommunicationChannelDriverCommand = new DelegateCommand(RunPrimitiveCommunicationChannelDriver, DeviceModelIsNotNull);

            ExecuteSkillCommand = new DelegateCommand<ISkill>(ExecuteSkill);

            ExecuteSkillAsyncCommand = DelegateCommand<ISkill>.FromAsyncHandler(ExecuteSkillAsync);

            DeleteDeviceCommand = new DelegateCommand(DeleteDevice);

        }

        private void SaveToInformationSources()
        {
            DeviceInformationManager.StoreDeviceInformations(DeviceModel);
        }

        private void DeleteDevice()
        {
            if (DeletionRequest != null) DeletionRequest(this, EventArgs.Empty);
        }

        public IEnumerable<ISkill> DeviceSkills
        {
            get { return DeviceModel.Skills; }
        }

        public event EventHandler DeletionRequest;


        private void ExecuteSkill(ISkill skill)
        {
            if (skill != null)
            {
                //var cast = skill as ReferencingDeviceSkillBase;
                //if (cast != null)
                //{
                //    cast.DeviceConstraints.Device = DeviceModel;
                //}

                Logger.Info("Executing skill \"{0}\"", skill.Name);
                //var task = Task.Factory.StartNew(skill.Execute);
                skill.Execute();
            }
            else
            {
                Logger.Warning("Skill is null");
            }
        }

        private async Task ExecuteSkillAsync(ISkill skill)
        {
            if (skill != null)
            {
                //var cast = skill as ReferencingDeviceSkillBase;
                //if (cast != null)
                //{
                //    cast.DeviceConstraints.Device = DeviceModel;
                //}

                await Logger.Info("Executing skill \"{0}\" asynchronously.", skill.Name);
                //var task = Task.Factory.StartNew(skill.Execute);
                //await skill.ExecuteAsync();

                Exception ex = null;
                try
                {
                    await skill.ExecuteAsync();
                }
                catch (Exception e)
                {
                    ex = e;
                }

                if (ex != null)
                {
                    await Logger.Error(ex.ToString(true));
                }

            }
            else
            {
                await Logger.Warning("Skill is null");
            }
        }

        private void RunPrimitiveCommunicationChannelDriver()
        {
            DefaultPrimitiveCommunicationChannelDriver.AddPrimitiveSubsystems
                (DeviceModel);
            OnPropertyChanged(string.Empty);
        }


        public DeviceTapakoViewModel(IDevice device) : this()
        {
            DeviceModel = device;
        }

        #region fields

        /// <summary>
        /// Ein Dictionary, welches allen instanziierten Objekten einen string zuweist.
        /// Dieses Dictionary wird dazu benutzt, um InputArguments dem SearchForSubDevice-Strategy mitzugeben
        /// </summary>
        private readonly Dictionary<string, object> _componentDictionary;

        /// <summary>
        /// Main TapakoViewModel Dispatcher
        /// </summary>
        //private readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;

        private IDevice _deviceModel;

        private ObservableCollection<IDeviceTapakoViewModel> _subDeviceViewModels; 

        private OpcUaServerControlViewModel _serverViewModel;

        #endregion

        #region events
        public event EventHandler ModelChanged;
        //public event EventHandler StatusChanged;
        #endregion

        #region properties

        public IDevice DeviceModel
        {
            get { return _deviceModel; }
            set
            {
                if (value != null)
                {
                    value.PropertyChanged += (sender, args) => OnPropertyChanged(string.Empty);
                }

                ServerViewModel.ObjectToPublish = value;
                SetProperty(ref _deviceModel, value);

                if (ModelChanged != null) ModelChanged(this, EventArgs.Empty);
                
            }
        }

        public ObservableCollection<IDeviceTapakoViewModel> SubDeviceViewModels
        {
            get
            {
                return RefreshSubdeviceViewModels();
            }
        }

        private ObservableCollection<IDeviceTapakoViewModel> RefreshSubdeviceViewModels()
        {
            if (DeviceModel == null)
            {
                //Logger.Debug("Device in \"{0}\" is null", this);
                return null;
            }

            // Add new ViewModels
            foreach (var subDevice in DeviceModel.SubDevices.ToArray())
            {
                // Add new ViewModel if subDeviceModel is not existing
                if (_subDeviceViewModels.All(viewModel => viewModel.DeviceModel != subDevice))
                {
                    var newViewModel = new DeviceTapakoViewModel(subDevice);
                    newViewModel.DeletionRequest += OnRemoveRequest;
                    // Register Events
                    //newViewModel.PropertyChanged += (sender, args) => OnPropertyChanged();
                    _subDeviceViewModels.Add(newViewModel);
                    
                }
            }

            // Delete all viewModels, which are not existing anymore
            foreach (var subDeviceViewModel in _subDeviceViewModels.ToArray()) // To Array, because this list will eventually change
            {
                if (subDeviceViewModel.DeviceModel == null || !DeviceModel.SubDevices.Contains(subDeviceViewModel.DeviceModel))
                {
                    _subDeviceViewModels.Remove(subDeviceViewModel);
                }
            }
            return _subDeviceViewModels;
        }

        private void OnRemoveRequest(object sender, EventArgs e)
        {
            var vm = sender as IDeviceTapakoViewModel;
            if (vm != null)
            {
                vm.DeletionRequest -= OnRemoveRequest; // Unregister
                _subDeviceViewModels.Remove(vm);
                DeviceModel.SubDevices.Remove(vm.DeviceModel); // Remove removed viewModel also from its parents subdevices
            }
        }

        public OpcUaServerControlViewModel ServerViewModel
        {
            get
            {
                if (_serverViewModel == null)
                {
                    // Give the correlated Device for publishing
                    _serverViewModel = new OpcUaServerControlViewModel(DeviceModel);
                }
                return _serverViewModel;
            }
            set { SetProperty(ref _serverViewModel, value); }
        }

        public DelegateCommand AnalyseDeviceCommand { get; private set; }
        public DelegateCommand CancelDeviceAnalysisCommand { get; private set; }
        public DelegateCommand SaveDeviceOnLocalDiscCommand { get; private set; }
        public DelegateCommand SaveDeviceDataInInformationSourcesCommand { get; private set; }

        //public DelegateCommand SavePrimitivesCommand { get; private set; }
        //public DelegateCommand LoadPrimitivesCommand { get; private set; }
        public DelegateCommand DeleteDeviceCommand { get; private set; }

        /// <summary>
        /// Lädt alle zuvor gespeichertern primitiven Geräte in eine gegebene Gerätehierarchie
        /// </summary>
        public DelegateCommand<ISkill> ExecuteSkillCommand { get; private set; }
        public DelegateCommand<ISkill> ExecuteSkillAsyncCommand { get; private set; }
        public DelegateCommand RunPrimitiveCommunicationChannelDriverCommand { get; private set; }
        #endregion
        

        #region Can Methods

        public bool CanInvokeCancelDeviceAnalysisCommand()
        {
            return AnalyseDeviceCommand.IsActive;
        }

        public bool DeviceModelIsNotNull()
        {
            return DeviceModel != null;
        }

        #endregion

        #region Command Implementations
        private void CancelAnalysis()
        {
            Logger.Debug("No Cancellation Token implemented.").ConfigureAwait(false);
        }

        public async Task AnalyseDevice()
        {
            //IsBusy = true;
            AnalyseDeviceCommand.IsActive = true;

            if (DeviceModel == null)
            {
                await Logger.Info("No Device was selected").ConfigureAwait(false);
            }
            else
            {
                await Logger.Info("Scan for " + DeviceModel + " was started").ConfigureAwait(false);

                var deviceInstance = await DeviceInformationManager.AsyncCompleteDeviceInformation(DeviceModel as IDevice);
                //var deviceInstance = DeviceInformationManager.CompleteDeviceInformation(DeviceModel as IDevice);                
                //var deviceInstance = result.Result;

                

                if (deviceInstance != null && deviceInstance.Skills.GetSkill<SkillSearchForSubdevicesBase>() != null)
                {
                    //// Standard Argumente
                    //SetCustomArgument(deviceInstance.SearchForSubDevices, StrategyBase.ArgumentKeywords.ParentObject, deviceInstance);

                    //// Verbindliche Argumente
                    //SetMandatoryArguments(deviceInstance.SearchForSubDevices);

                    //// Optionale Argumente
                    //SetOptionalArguments(deviceInstance.SearchForSubDevices);

                    //// Suche durchführen
                    //await deviceInstance.SearchForSubDevices.ExecuteAsync();

                    var searchSkill = deviceInstance.Skills.GetSkill<SkillSearchForSubdevicesBase>();

                    await searchSkill.ExecuteAsync();

                    // Manuelles Eventauslösen damit GUI aktualisiert wird
                    OnPropertyChanged(string.Empty);

                }
                else
                {
                    await Logger.Warning(deviceInstance +
                                    " loaded no SearchForSubDevices-Logic. The searching task will terminate").ConfigureAwait(false);
                }

                //_dispatcher.Invoke(() => MasterDevices.Add(deviceInstance), DispatcherPriority.DataBind);
                //return ((TapakoDevice)selectedHostDevice).SubDevices;
                AnalyseDeviceCommand.IsActive = false;
            }
            //IsBusy = false;


        }

        private void SaveDevice()
        {
            try
            {
                SaveDeviceOnLocalDiscCommand.IsActive = true;
                var task = Task.Factory.StartNew(dev => StorageModule.SaveToFile(dev), DeviceModel);

                task.ContinueWith(t =>
                {
                    SaveDeviceOnLocalDiscCommand.IsActive = false;
                    //Logger.Info("Device saved");
                });
                //task.Start(TaskScheduler.Current);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        #endregion Command Implementations

        #region public Methods


        //public void LoadDevice()
        //{
        //    try
        //    {
        //        DeviceModel = StorageModule.LoadFromFile<IDevice>();
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Error(e);
        //    }
        //    //HostDeviceList.Add(loadeDevice);
        //}

        public IEnumerable<DelegateCommandBase> GetCommands()
        {
            return
                GetType()
                    .GetProperties()
                    .Where(property => typeof (DelegateCommandBase).IsAssignableFrom(property.PropertyType))
                    .Select(properyInfo => (DelegateCommandBase) properyInfo.GetValue(this));
        }

        public bool HasDeviceDriver(IDevice device)
        {
            return DeviceInformationManager.IsDriverAvailable(device);
        }

        public string BrowseName
        {
            get
            {
                return DeviceModel != null ? DeviceModel.ToString() : null;
            }
        }

        public IHmiImage PresentationImage
        {
            get { return DeviceModel.GetHmiImage(); }
        }

        #endregion

    }

}
