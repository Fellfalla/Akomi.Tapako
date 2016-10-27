using System;
using System.Threading;
using System.Threading.Tasks;
using Akomi.Logger;
using AkomiServer;
using IAkomiDevice.fIDevice;
using Tapako.Services;

namespace Tapako.OpcUaServer
{
    /// <summary>
    /// How To Use: http://documentation.unified-automation.com/uasdkdotnet/2.1.0/html/L1ServerTutorials.html
    /// Diese Klasse dient als Vermittlungsschicht, um Befehle zwischen der Logik und dem echten OpcUaServer zu vermitteln
    /// </summary>
    public class TapakoOpcUaServer
    {
        private AkomiServer.OpcUaServer _opcUaServer;

        public event EventHandler StartupCompleted;

        public event EventHandler StartupInvoked;

        public event EventHandler ShutdownCompleted;

        public event EventHandler ShutdownInvoked;

        public event EventHandler StatusChanged;

        public TapakoOpcUaServer()
        {
            OpcUaServer.ShutdownCompleted += RaiseShutdownCompleted;
            OpcUaServer.ShutdownCompleted += LogShutdownCompleted;


            OpcUaServer.ShutdownInvoked += RaiseShutdownInvoked;
            OpcUaServer.ShutdownInvoked += LogShutdownInvoked;


            OpcUaServer.StartupInvoked += RaiseStartupInvoked;
            OpcUaServer.StartupInvoked += LogStartupInvoked;


            OpcUaServer.StartupCompleted += RaiseStartupCompleted;
            OpcUaServer.StartupCompleted += LogStartupCompleted;


            OpcUaServer.StatusChanged += RaiseStatusChanged;
        }

        private AkomiServer.OpcUaServer OpcUaServer
        {
            get
            {
                if (_opcUaServer == null)
                {
                    // hier findet erst die Instanziierung statt, damit der Bootvorgang schneller geht (Auswirkungen wurden nie getestet)
                    _opcUaServer = new AkomiServer.OpcUaServer();
                }
                return _opcUaServer;
            }
            set { _opcUaServer = value; }
        }

        public void StartOpcUaServer(object Object, int maxLvl=4, string rootNodeName = "TapakoMaster")
        {
            if (StartupInvoked != null) StartupInvoked(this, EventArgs.Empty);

            

            OpcUaStartupParameters serverParameter = new OpcUaStartupParameters()
            {
                RootNodeName = rootNodeName,
                InheritedObjectSimplification = new []{typeof(IDevice)},
                CurrentDepth = 0,
                MaximumDepth = 10,
                TryUseAppConfigFile = false,
                UserCanSearchForConfigurationFile = false,
                IgnoreNullPointer = false,
            };

            Logger.Info("Start OPC-UA-Server with {0}", serverParameter.ToString(true));

            if (Object == null)
            {
                Logger.Warning("No objects available.\nEmpty server is starting");
                OpcUaServer.StartServer(new object(), serverParameter);                
                //OpcUaServer.StartServer(new object(), "TapakoMaster", 0, 10, loadConfigurationFile:true);
            }
            else
            {

                var serverThread = Task.Factory.StartNew(
                   (opcRegistrationObject) =>
                   {
                       OpcUaServer.StartServer(opcRegistrationObject, serverParameter);
                       //OpcUaServer.StartServer(opcRegistrationObject, "TapakoMaster", 0, 10, loadConfigurationFile: true);
                   }, Object,
                   CancellationToken.None,
                   TaskCreationOptions.None,
                   TaskScheduler.Default);
                // todo: Server should refresh Status, if exception is thrown
                serverThread.ContinueWith((Action<Task>) FunctionCollection.TaskExceptionThrower, TaskContinuationOptions.OnlyOnFaulted);
            }

            if (StatusChanged != null) StatusChanged(this, EventArgs.Empty);
        }

        //public static void SearchAppConfigFile()
        //{
        //    AppConfigChanger.FindConfigFilePath("AkomiServer.dll.config", true);
        //}


        public void StartOpcUaServer()
        {
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

        public bool IsServerRunning()
        {
            return OpcUaServer.IsServerRunning();
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
            Logger.Info("OPC Ua Server startup completed");
        }

        private void LogStartupInvoked(object sender, EventArgs eventArgs)
        {
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
