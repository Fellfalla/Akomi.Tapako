using System;
using System.Collections.Generic;
using System.Windows;
using Akomi.InformationModel.Component.Identification;
using Akomi.InformationModel.Device;
using Akomi.Logger;
using Tapako.DeviceInformationManagement;
using Tapako.Repositories.WiringInformationSource;
using Tapako.Utilities.DeviceSelector.View;
using Tapako.Utilities.DeviceSelector.ViewModel;

namespace Tapako.Utilities.DeviceSelector
{
    /// <summary>
    /// Diese Klasse bietet Zugriff auf eine Userschnittstelle.
    /// Sie wird benutzt, um den User wählen zu lassen, welches DeviceModell an dem angegebenen Systempunkt angeschlossen ist.
    /// todo: Popup fenster zu Behavior machen -> und ins Protokoll als wichtigen Punkt der Softwarearchitektur schreiben
    /// </summary>
    public class DeviceSelector
    {

        /// <summary>
        /// Wires the Connections of these 2 Connection Lists
        /// </summary>
        public static Tuple<string, string> SelectDevice()
        {
            return SelectDevice(null);
        }        
        
        /// <summary>
        /// Wires the Connections of these 2 Connection Lists
        /// </summary>
        public static Tuple<string,string> SelectDevice(List<string> modelSuggestions, bool overrideDefaultSuggestions = false, IDevice connectionParent = null)
        {
            var showSelectorWindow = new Func<Tuple<string, string>>(() => RunSelectorWindow(modelSuggestions, overrideDefaultSuggestions, connectionParent));

            // Start Selector Window in the UI Thread
            if (Application.Current != null)
            {
                return Application.Current.Dispatcher.Invoke(showSelectorWindow);
            }
            else
            {
                return showSelectorWindow.Invoke();
            }
        }

        private static Tuple<string, string> RunSelectorWindow(List<string> modelSuggestions, bool overrideDefaultSuggestions, IDevice connectionParent)
        {
            var viewModel = new DeviceSelectorViewModel { ParentDevice = connectionParent };
            if (modelSuggestions != null)
            {
                viewModel.ModelSuggestions = modelSuggestions;
                viewModel.OverrideModelSuggestions = overrideDefaultSuggestions;
            }

            var mainWindow = new DeviceSelectorView(viewModel);

            mainWindow.Activate();

            return mainWindow.ShowDialog();
        } 

        public static List<IDevice> StartSelectDevice(IDevice parent)
        {
            var newDeviceList = new List<IDevice>();

            var wiringInformationSource = new WiringInformationSource();

            var newDevice = SelectNewDevice(parent);
            while (newDevice != null)
            {
                if (wiringInformationSource.HasDeviceDriver(newDevice))
                {
                    Logger.Debug("Loading connections for \"{0}\"", newDevice);
                    var wiredDevice = wiringInformationSource.GetDeviceInformations(newDevice);
                    if (wiredDevice != null)
                    {
                        newDevice.Connections = wiredDevice.Connections;
                    }
                }
                else
                {
                    newDevice = WiringTool.WiringTool.WireConnections(newDevice, parent);
                    wiringInformationSource.StoreDeviceInformations(newDevice);
                }

                Logger.Info("New primitive device added: \"{0}\"", newDevice);
                newDeviceList.Add(newDevice);

                //next Device
                newDevice = SelectNewDevice(parent);
            }

            return newDeviceList;
        }

        public static DeviceBase SelectNewDevice(IDevice parent)
        {
            var instanceInformations = SelectDevice(new List<string>(), connectionParent: parent);

            if (string.IsNullOrEmpty(instanceInformations.Item1))
            {
                return null;
            }
            var newDevice = new DeviceBase
            {
                Identification = new Identification()
                {
                    ModelNumber = instanceInformations.Item1,
                    SerialNumber = instanceInformations.Item2
                }
            };

            newDevice = DeviceInformationManager.CompleteDeviceInformation(newDevice);
            return newDevice;
        }
    }
}

