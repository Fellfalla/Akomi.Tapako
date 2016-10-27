using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akomi.InformationModel.Device;
using Akomi.InformationModel.Enums;
using Akomi.Logger;
using Prism.Commands;
using Prism.Mvvm;
using Tapako.Framework;
using Tapako.Framework.ExtensionMethods;

namespace Tapako.Utilities.UniversalHostSearch
{
    public class UniversalHostSearchViewModel : BindableBase
    {
        public UniversalHostSearchViewModel()
        {
            ScanNetworkForHostsCommand = DelegateCommand<string>.FromAsyncHandler(ScanNetworkForHosts, CanScanNetworkForHosts);
            //HostDeviceList = new List<IDevice>();

            // Register events
            SubnetStringChangedEvent += ScanNetworkForHostsCommand.RaiseCanExecuteChanged;
            //NewNetworkDeviceFound += (obj, device) => HostDeviceList.Add(device);
            //NetwokDeviceDisappeared += (obj, device) => HostDeviceList.Remove(device);
        }


        public DelegateCommand<string> ScanNetworkForHostsCommand { get; set; }

        public event EventHandler<IDevice> NewNetworkDeviceFound;

        //public event EventHandler<IDevice> NetwokDeviceDisappeared;



        public string Subnet
        {
            get { return _subnet; }
            set
            {
                SetProperty(ref _subnet, value);
                if (SubnetStringChangedEvent != null) SubnetStringChangedEvent();
            }
        }

        /// <summary>
        /// delegate für Funktionen ohne Parameter
        /// </summary>
        private delegate void NoParameterDelegate();

        /// <summary>
        /// Event: SubnetString wurde geändert
        /// </summary>
        private event NoParameterDelegate SubnetStringChangedEvent;

        //public List<IDevice> HostDeviceList { get; set; }

        //private Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;
        private string _subnet;

        /// <summary>
        /// Bedingung, um die Netzwerksuche nach Hosts ausführen zu können
        /// </summary>
        /// <param name="subnetString"></param>
        /// <returns></returns>
        private bool CanScanNetworkForHosts(string subnetString)
        {
            subnetString = UniversalHostSearcher.ParseForSubnetString(subnetString);
            return !string.IsNullOrEmpty(subnetString);
        }

        public async Task ScanNetworkForHosts(string subnetString)
        {
            await Logger.Info("Network search was started").ConfigureAwait(false);

            //TapakoProgress.NextGenerationDeviceScan = ProgressState.InProgress;

            ScanNetworkForHostsCommand.IsActive = true;
            using (var progressReverter = TapakoProgress.SetProgressStep(ProgressStep.NextGenerationScan, ProgressState.InProgress))
            {
                progressReverter.SetDisposedState(ProgressState.Failed);

                List<Task<IDevice>> tasks = new List<Task<IDevice>>();

                foreach (var ipAddress in UniversalHostSearcher.GetIpListFromSubnet(subnetString))
                {
                    var task = UniversalHostSearcher.ProcessNetworkAddress(ipAddress);

                    var _ = task.ContinueWith(EvaluateNetworkScanTask, TaskContinuationOptions.NotOnFaulted);
                    //await task;
                    tasks.Add(task);
                }

                await tasks.WhenAllTasks();
            
                await Logger.Debug("Ping addresses has taken " + UniversalHostSearcher.PingStopwatch.Elapsed.TotalSeconds + " seconds.").ConfigureAwait(false);
                await Logger.Debug("Mac address search has taken " + UniversalHostSearcher.MacStopwatch.Elapsed.TotalSeconds + " seconds.").ConfigureAwait(false);
                await Logger.Debug("Host name search has taken " + UniversalHostSearcher.HostnameStopwatch.Elapsed.TotalSeconds + " seconds.").ConfigureAwait(false);

                progressReverter.SetDisposedState(ProgressState.Finished);
            }

            await Logger.Info("Network search was finished").ConfigureAwait(false);

            ScanNetworkForHostsCommand.IsActive = false;
        }

        private void EvaluateNetworkScanTask(Task<IDevice> scanTask)
        {
            BroadcastFoundDevice(scanTask.Result);
        }

        /// <summary>
        /// Fügt ein neues Host-TapakoDevice zur Liste hinzu, falls dessen MAC-Adresse darin noch nicht enthalten ist
        /// </summary>
        /// <param name="iDevice"></param>
        private void BroadcastFoundDevice(IDevice iDevice)
        {
            if (iDevice == null || iDevice.Identification == null)
            {
                return;
            }
            if (NewNetworkDeviceFound != null) NewNetworkDeviceFound(this, iDevice);

            // für jedes neue TapakoDevice
            //if (!HostDeviceList.Any(existingDevice => existingDevice.HasEqualNetworkInformation(iDevice)))
            //{
            //    if (NewNetworkDeviceFound != null) NewNetworkDeviceFound(this, iDevice);
            //}
        }
    }

    public static class Extensions
    {
        /// <summary>
        /// Tests if the informations, which can be gathered from network requests, are equal.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool HasEqualNetworkInformation(this IDevice first, IDevice second)
        {
            if (first != null && first.Identification != null && second != null && second.Identification != null)
            {
                return first.Identification.PhysicalAddress.Equals(second.Identification.PhysicalAddress) &&
                       first.Identification.IpAddress.Equals(second.Identification.IpAddress);
            }
            else
            {
                return false;
            }

        }
    }
}
