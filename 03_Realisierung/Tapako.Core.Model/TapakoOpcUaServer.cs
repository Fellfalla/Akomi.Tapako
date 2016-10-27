using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Akomi.InformationModel.Device;
using Akomi.InformationModel.Enums;
using Akomi.InformationModel.Skills;
using Akomi.Logger;
using Akomi.Server;
using ExtensionMethodsCollection;
using Tapako.Framework;

namespace Tapako.Model
{
    /// <summary>
    /// How To Use: http://documentation.unified-automation.com/uasdkdotnet/2.1.0/html/L1ServerTutorials.html
    /// Diese Klasse dient als Vermittlungsschicht, um Befehle zwischen der Logik und dem echten OpcUaServer zu vermitteln
    /// todo: Show whe maximum recursion depth has exceeded
    /// todo: Test what takes so long for finishing, after objects have been registered
    /// </summary>
    public class TapakoOpcUaServer
    {
        private OpcUaServer _opcUaServer;

        private Stopwatch _startupStopwatch = new Stopwatch(); // Object for time measures

        public event EventHandler StartupCompleted;

        public event EventHandler StartupInvoked;

        public event EventHandler ShutdownCompleted;

        public event EventHandler ShutdownInvoked;

        public event EventHandler StatusChanged;

        public event EventHandler<object> InvokedObjectRegistration;

        public event EventHandler<object> FinishedObjectRegistration;

        public event EventHandler<uint> CurrentRecursionDepthChanged;

        /// <summary>
        /// This event is fired when a new node is created.
        /// Eventargs contains the new count of the nodes
        /// </summary>
        public event EventHandler<int> NodeCreated;

        /// <summary>
        /// This event is fired when a new method is created.
        /// Eventargs contains the new count of the methods.
        /// </summary>
        public event EventHandler<int> MethodCreated;

        /// <summary>
        /// This event is fired when a new variable is created.
        /// Eventargs contains the new count of the variables.
        /// </summary>
        public event EventHandler<int> VariableCreated;


        //public static readonly uint DefaultPort = 48030;

        //public static readonly string DefaultApplicationName = "Tapako Server";

        //public static readonly uint DefaultRecursionDepth = 10;

        //public static readonly bool DefaultIgnoreNullObjects = false;

        //public static readonly bool SuppressConsoleOutputDefault = true;

        [DefaultValue(48030)]
        public uint Port { get; set; }

        [DefaultValue("Tapako Server")]
        public string ApplicationName { get; set; }

        [DefaultValue(15)]
        public uint MaximumRecursionDepth { get; set; }

        [DefaultValue(false)]
        public bool IgnoreNullObjects { get; set; }

        [DefaultValue(true)]
        public bool SuppressConsoleOutput { get; set; }

        private static TapakoOpcUaServer _instance;

        /// <summary>
        /// Implementation of Singleton Pattern
        /// </summary>
        public static TapakoOpcUaServer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TapakoOpcUaServer();
                }
                return _instance;
            }
        }

        public TapakoOpcUaServer()
        {
            this.AssignDefaultValueAttributes();

            // Set progress events
            StartupCompleted += (sender, args) => TapakoProgress.SetProgressStep(ProgressStep.Publication, ProgressState.Finished);
            ShutdownCompleted += (sender, args) => TapakoProgress.SetProgressStep(ProgressStep.Publication, ProgressState.Outstanding);
            StartupInvoked += (sender, args) => TapakoProgress.SetProgressStep(ProgressStep.Publication, ProgressState.InProgress);
            ShutdownInvoked += (sender, args) => TapakoProgress.SetProgressStep(ProgressStep.Publication, ProgressState.InProgress);
        }

        //private void SetDefaultValues()
        //{
        //    Port = DefaultPort;
        //    ApplicationName = DefaultApplicationName;
        //    IgnoreNullObjects = DefaultIgnoreNullObjects;
        //    MaximumRecursionDepth = DefaultRecursionDepth;
        //    SuppressConsoleOutput = 
        //}

        private OpcUaServer OpcUaServer
        {
            get
            {
                if (_opcUaServer == null)
                {
                    // hier findet erst die Instanziierung statt, damit der Bootvorgang schneller geht (Auswirkungen wurden nie getestet)
                    OpcUaServer = new OpcUaServer();
                }
                return _opcUaServer;
            }
            set
            {
                if (!ReferenceEquals(_opcUaServer, value))
                {
                    //unregister old events
                    if (_opcUaServer != null)
                    {
                        UnregisterEvents(_opcUaServer);
                    }

                    _opcUaServer = value;
                    RegisterEvents(_opcUaServer);
                }
            }
        }

        private void UnregisterEvents(OpcUaServer server)
        {
            _opcUaServer.NodeCountChanged -= this.NodeCreated;
            _opcUaServer.MethodCountChanged -= this.MethodCreated;
            _opcUaServer.VariableCountChanged -= this.VariableCreated;
            _opcUaServer.InvokedObjectRegistration -= InvokedObjectRegistration;
            _opcUaServer.FinishedObjectRegistration -= FinishedObjectRegistration;
            _opcUaServer.CurrentRecursionDepthChanged -= CurrentRecursionDepthChanged;

            // shutdown completed
            _opcUaServer.ShutdownCompleted -= RaiseShutdownCompleted;
            _opcUaServer.ShutdownCompleted -= LogShutdownCompleted;

            // shutdown invoked
            _opcUaServer.ShutdownInvoked -= RaiseShutdownInvoked;
            _opcUaServer.ShutdownInvoked -= LogShutdownInvoked;

            // startup invoked
            _opcUaServer.StartupInvoked -= RaiseStartupInvoked;
            _opcUaServer.StartupInvoked -= LogStartupInvoked;

            // startup completed
            _opcUaServer.StartupCompleted -= RaiseStartupCompleted;
            _opcUaServer.StartupCompleted -= LogStartupCompleted;

            // status changed
            _opcUaServer.StatusChanged -= RaiseStatusChanged;
        }

        private void RegisterEvents(OpcUaServer server)
        {
            // register new events
            _opcUaServer.NodeCountChanged += this.NodeCreated;
            _opcUaServer.MethodCountChanged += this.MethodCreated;
            _opcUaServer.VariableCountChanged += this.VariableCreated;
            _opcUaServer.InvokedObjectRegistration += InvokedObjectRegistration;
            _opcUaServer.FinishedObjectRegistration += FinishedObjectRegistration;
            _opcUaServer.CurrentRecursionDepthChanged += CurrentRecursionDepthChanged;

            // shutdown completed
            _opcUaServer.ShutdownCompleted += RaiseShutdownCompleted;
            _opcUaServer.ShutdownCompleted += LogShutdownCompleted;

            // shutdown invoked
            _opcUaServer.ShutdownInvoked += RaiseShutdownInvoked;
            _opcUaServer.ShutdownInvoked += LogShutdownInvoked;

            // startup invoked
            _opcUaServer.StartupInvoked += RaiseStartupInvoked;
            _opcUaServer.StartupInvoked += LogStartupInvoked;

            // startup completed
            _opcUaServer.StartupCompleted += RaiseStartupCompleted;
            _opcUaServer.StartupCompleted += LogStartupCompleted;

            // status changed
            _opcUaServer.StatusChanged += RaiseStatusChanged;
        }

        private OpcUaStartupParameters CreateServerParameterObject(string rootNodeName = "HostDevice")
        {

            OpcUaStartupParameters serverParameter = new OpcUaStartupParameters()
            {
                ApplicationName = ApplicationName,
                RootNodeName = rootNodeName,
                InheritedObjectSimplification = new List<Type>() { typeof(IDevice) },
                CurrentDepth = 0,
                MaximumDepth = MaximumRecursionDepth,
                TryUseAppConfigFile = false,
                UserCanSearchForConfigurationFile = true,
                IgnoreNullPointer = IgnoreNullObjects,
                Ports = new[] { (int)Port },
                CreateNewApplicationInstance = true,
                RestartTimer = 3500,
                RestartServer = false,
                HideConsoleOutput = SuppressConsoleOutput,
            };

            // Simplify all Skills to their base type
            serverParameter.InheritedObjectSimplification.AddRange(SkillHelper.GetAllSkillBaseTypes());

            return serverParameter;
        }


        public void StartOpcUaServer(object Object, string rootNodeName = "HostDevice")
        {
            if (StartupInvoked != null) StartupInvoked(this, EventArgs.Empty);

            if (Object != null)
            {
                rootNodeName = Object.ToString();
            }

            var serverParameter = CreateServerParameterObject(rootNodeName);

            Logger.Info("Start OPC-UA-Server with {0}", serverParameter.ToString(true));

            if (Object == null)
            {
                Logger.Warning("No objects available.\nEmpty server is starting");
                OpcUaServer.StartServer(new object(), serverParameter);                
            }
            else
            {

                var serverThread = Task.Factory.StartNew(
                   opcRegistrationObject =>
                   {
                       OpcUaServer.StartServer(opcRegistrationObject, serverParameter);
                   }, Object,
                   CancellationToken.None,
                   TaskCreationOptions.None,
                   TaskScheduler.Default);
                // todo: Server should refresh Status, if exception is thrown
                serverThread.ContinueWith((Action<Task>) FunctionCollection.TaskExceptionThrower, TaskContinuationOptions.OnlyOnFaulted);
            }

            if (StatusChanged != null) StatusChanged(this, EventArgs.Empty);
        }

        public void StartOpcUaServer()
        {
            // todo: make async
            StartOpcUaServer(new object());
        }

        public void StopOpcUaServer()
        {
            if (ShutdownInvoked != null) ShutdownInvoked(this, EventArgs.Empty);

            var task = Task.Run(() => OpcUaServer.StopServer());
            task.ContinueWith(FunctionCollection.TaskExceptionThrower, TaskContinuationOptions.OnlyOnFaulted);

            if (StatusChanged != null) StatusChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Wartet bis die aktuelle OPCUA Serverinstanz beendet wurde
        /// </summary>
        public void WaitForServerShutdown()
        {
            //if (_opcUaServer != null) OpcUaServer.WaitForServer();
            if (_opcUaServer != null) OpcUaServer.WaitForServerShutdown();
        }

        public bool IsServerRunning
        {
            get
            {
             return OpcUaServer.IsServerRunning();
            }
        }

        public int RegisteredVariables
        {
            get { return _opcUaServer.VariableCount; }
        }

        public int RegisteredNodes
        {
            get { return _opcUaServer.NodeCount; }
        }

        public int RegisteredMethods
        {
            get { return _opcUaServer.MethodCount; }
        }

        /// <summary>
        /// Beendet die OpcUa server Instanz
        /// </summary>
        public void Dispose()
        {
            _instance._opcUaServer.StopServer();
            _instance._opcUaServer = null;
            _instance = null;
        }

        /// <summary>
        /// Wartet bis der Server läuft. 
        /// Aber maximal <paramref name="timeout"/> millisekunden lange.
        /// </summary>
        /// <param name="timeout">Timeout in Millisekunden</param>
        /// <returns>Gibt false zurück, falls der Server innerhalb der angegeben Zeit nicht läuft, andernfalls true</returns>
        public bool WaitForServerStartup(uint timeout = 0)
        {
            //return OpcUaServer.WaitForServerStartup(timeout);
            while (!OpcUaServer.IsServerRunning())
            {
                Task.WaitAny(Task.Delay(100));
            }
            return OpcUaServer.IsServerRunning();

            //return OpcUaServer.WaitForServerStartup(timeout);
        }

        // todo: dynamische Statusabfrage zu Logging hinzufügen
        private void LogStartupCompleted(object sender, EventArgs eventArgs)
        {
            _startupStopwatch.Stop();
            Logger.Info("OPC Ua Server startup completed in {0:0.00} seconds.", _startupStopwatch.Elapsed.TotalSeconds);
        }

        private void LogStartupInvoked(object sender, EventArgs eventArgs)
        {
            _startupStopwatch.Reset();
            _startupStopwatch.Start();
            Logger.Info("OPC Ua Server startup invoked...");
        }

        private void LogShutdownInvoked(object sender, EventArgs eventArgs)
        {
            Logger.Info("OPC Ua Server shutdown invoked...");
        }

        private void LogShutdownCompleted(object sender, EventArgs eventArgs)
        {
            Logger.Info("OPC Ua Server shutdown completed...");
        }


        private void RaiseStatusChanged(object sender, EventArgs eventArgs)
        {
            if (StatusChanged != null) StatusChanged(sender, eventArgs);
        }
        private void RaiseStartupCompleted(object sender, EventArgs eventArgs)
        {
            if (StartupCompleted != null) StartupCompleted(sender, eventArgs);
        }
        private void RaiseStartupInvoked(object sender, EventArgs eventArgs)
        {
            if (StartupInvoked != null) StartupInvoked(sender, eventArgs);
        }
        private void RaiseShutdownInvoked(object sender, EventArgs eventArgs)
        {
            if (ShutdownInvoked != null) ShutdownInvoked(sender, eventArgs);
        }

        private void RaiseShutdownCompleted(object sender, EventArgs eventArgs)
        {
            if (ShutdownCompleted != null) ShutdownCompleted(sender, eventArgs);
        }

    }
}
