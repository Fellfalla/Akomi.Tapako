using Akomi.Logger;
using Prism.Commands;
using Prism.Mvvm;
using Tapako.Model;

namespace Tapako.ViewModel
{
    public class OpcUaServerControlViewModel : BindableBase
    {
        private object _objectToPublish;
        private TapakoOpcUaServer _serverModel;
        private object _registeringObject;
        private uint _currentRecursionDepth;

        private TapakoOpcUaServer ServerModel
        {
            get
            {
                if (_serverModel == null)
                {
                    // hier findet erst die Instanziierung statt, damit der Bootvorgang schneller geht (Auswirkungen wurden nie getestet)
                    ServerModel = new TapakoOpcUaServer();
                }
                return _serverModel;
            }
            set
            {
                if (!ReferenceEquals(_serverModel, value))
                {
                    //unregister old events
                    if (_serverModel != null)
                    {
                        UnregisterServerEvents(_serverModel);
                    }

                    _serverModel = value;
                    RegisterServerEvents(_serverModel);
                }
            }
        }

        public OpcUaServerControlViewModel()
        {
            InitializeVariables();

            StartOpcUaServerCommand = new DelegateCommand(StartOpcUaServer, CanInvokeStartOpcUaServerCommand);

            StopOpcUaServerCommand = new DelegateCommand(StopOpcUaServer, CanInvokeStopOpcUaServerCommand);

            //_serverModel = new TapakoOpcUaServer();

            //RegisterServerNotifications();
        }



        public OpcUaServerControlViewModel(object objectToPublish) : this()
        {
            ObjectToPublish = objectToPublish;
        }

        private void InitializeVariables()
        {
            IsServerReady = true;
        }

        private void RegisterServerEvents(TapakoOpcUaServer server)
        {
            server.StartupInvoked += (sender, args) => IsServerReady = false;
            server.ShutdownInvoked += (sender, args) => IsServerReady = false;

            server.StartupCompleted += (sender, args) => IsServerReady = true;
            server.ShutdownCompleted += (sender, args) => IsServerReady = true;
            server.StartupCompleted += (sender, args) => OnPropertyChanged("IsServerStarted");
            server.ShutdownCompleted += (sender, args) => OnPropertyChanged("IsServerStarted");

            server.NodeCreated += (sender, i) => OnPropertyChanged("RegisteredNodeCount");
            server.MethodCreated += (sender, i) => OnPropertyChanged("RegisteredMethodCount");
            server.VariableCreated += (sender, i) => OnPropertyChanged("RegisteredVariableCount");

            server.InvokedObjectRegistration += (sender, obj) => RegisteringObject = obj;

            server.FinishedObjectRegistration += (sender, obj) => RegisteringObject = null;

            server.CurrentRecursionDepthChanged += (sender, depth) => CurrentRecursionDepth = depth;
        }

        private void UnregisterServerEvents(TapakoOpcUaServer server)
        {
            Logger.Warning("Unregistering old server events due to server instance changing is not supported yet!");  
            //server.StartupInvoked -= (sender, args) => IsServerReady = false;
            //server.ShutdownInvoked -= (sender, args) => IsServerReady = false;

            //server.StartupCompleted += (sender, args) => IsServerReady = true;
            //server.ShutdownCompleted += (sender, args) => IsServerReady = true;
            //server.StartupCompleted += (sender, args) => OnPropertyChanged("IsServerStarted");
            //server.ShutdownCompleted += (sender, args) => OnPropertyChanged("IsServerStarted");
        }

        public object ObjectToPublish
        {
            get { return _objectToPublish; }
            set { SetProperty(ref _objectToPublish, value); }
        }

        public DelegateCommand StartOpcUaServerCommand { get; private set; }
        public DelegateCommand StopOpcUaServerCommand { get; private set; }

        public uint Port
        {
            get { return ServerModel.Port; }
            set { ServerModel.Port = value; }
        }

        public string ApplicationName
        {
            get { return ServerModel.ApplicationName; }
            set { ServerModel.ApplicationName = value; }
        }

        public bool IgnoreNullObjects
        {
            get { return ServerModel.IgnoreNullObjects; }
            set { ServerModel.IgnoreNullObjects = value; }
        }

        public bool SuppressConsoleOutput
        {
            get { return ServerModel.SuppressConsoleOutput; }
            set { ServerModel.SuppressConsoleOutput = value; }
        }

        public uint MaxRecursionDepth
        {
            get { return ServerModel.MaximumRecursionDepth; }
            set { ServerModel.MaximumRecursionDepth = value; }
        }


        public bool CanInvokeStartOpcUaServerCommand()
        {
            return !ServerModel.IsServerRunning;
        }

        public bool CanInvokeStopOpcUaServerCommand()
        {
            return ServerModel.IsServerRunning;
        }


        /// <summary>
        /// Startet den OPCUaServer in einem definierten ThreadSheduler
        /// </summary>
        public void StartOpcUaServer()
        {
            StartOpcUaServerCommand.IsActive = true;
            //hingefügt damit Progressbar animiert wird, wenn nicht über DeviceViewModel gestartet sondern direkt
            OnPropertyChanged("IsBusy");

            ServerModel.StartOpcUaServer(ObjectToPublish);

            StartOpcUaServerCommand.IsActive = false;
            OnPropertyChanged("IsBusy");
        }

        /// <summary>
        /// Stop OpcUaServer
        /// </summary>
        /// <returns></returns>
        public void StopOpcUaServer()
        {
            StopOpcUaServerCommand.IsActive = true;
            ServerModel.StopOpcUaServer();
            StopOpcUaServerCommand.IsActive = false;
        }

        public bool IsServerStarted
        {
            get { return ServerModel.IsServerRunning; }
        }

        // todo: Add IsServerReadyProperty
        public bool IsServerReady { get; private set; }

        public object RegisteredNodeCount
        {
            get { return ServerModel.RegisteredNodes; }
        }

        public object RegisteredMethodCount
        {
            get { return ServerModel.RegisteredMethods; }
        }

        public object RegisteredVariableCount
        {
            get { return ServerModel.RegisteredVariables; }
        }

        public object RegisteringObject
        {
            get { return _registeringObject; }
            private set { SetProperty(ref _registeringObject, value); }
        }
        public uint CurrentRecursionDepth
        {
            get { return _currentRecursionDepth; }
            private set { SetProperty(ref _currentRecursionDepth, value); }
        }
    }

}
