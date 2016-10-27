using System.Collections.Generic;
using System.Linq;
using Akomi.InformationModel.Device;
using Akomi.InformationModel.Enums;
using Akomi.InformationModel.Skills.SkillCatalogue;
using Tapako.Framework;
using Tapako.Utilities.DeviceSelector;

namespace Tapako.Services
{
    /// <summary>
    /// Default primitive CCD for usaged in all projects which use the Tapako framework.
    /// todo: embedd specific dlls with fody
    /// </summary>
    public class DefaultPrimitiveCommunicationChannelDriver : CommunicationChannelDriverBase
    {
        public override CommunicationChannelType CommunicationChannelType
        {
            get { return CommunicationChannelType.Primitive; }
        }

        public override List<IDevice> SearchForSubSystems(IDevice device)
        {
            return AddPrimitiveSubsystems(device);
        }

        /// <summary>
        /// Common Method to search and add primitive devices to given device
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public  static List<IDevice> AddPrimitiveSubsystems(IDevice device)
        {
            TapakoProgress.SetProgressStep(ProgressStep.PrimitveScan, ProgressState.InProgress);
            var newDeviceList = SelectDevices(device).ToList();
            device.SubDevices = device.SubDevices.Concat(newDeviceList).ToList();
            
            //foreach (var newDevice in newDeviceList.Where(newDevice => newDevice.SearchForSubDevices != null))
            foreach (var newDevice in newDeviceList.Where(newDevice => newDevice.Skills.GetSkill<SkillSearchForSubdevicesBase>() != null))
            {
                var searchSkill = newDevice.Skills.GetSkill<SkillSearchForSubdevicesBase>();

                searchSkill.Execute();

                foreach (var subDevice in searchSkill.OutputParam.SubDevices)
                {
                    newDevice.SubDevices.Add(subDevice);
                }

                //newDevice.SearchForSubDevices.InputArguments[StrategyBase.ArgumentKeywords.ParentObject.ToString()] =
                //    newDevice;
                //newDevice.SearchForSubDevices.Execute();
                //foreach (var device1 in newDevice.SearchForSubDevices.Result)
                //{
                //    newDevice.SubDevices.Add(device1);
                //}
            }
            TapakoProgress.SetProgressStep(ProgressStep.PrimitveScan, ProgressState.Finished);
            return newDeviceList;
        }

        private static IEnumerable<IDevice> SelectDevices(IDevice device)
        {
            var newDeviceList = DeviceSelector.StartSelectDevice(device);

            return newDeviceList;//.Select(DeviceInformationManager.CompleteDeviceInformation);
        }

        ///// <summary>
        ///// Ruft das Eingabefenster auf, in dem die angeschlossenen Geräte ausgewählt werden
        ///// </summary>
        //private static List<IDevice> AddSubsystemsManually(IDevice device)
        //{
        //    var newDeviceList = new List<IDevice>();

        //    var wiringInformationSource = new WiringInformationSource();

        //    var newDevice = ChooseSubsystem(device);

        //    newDevice = DeviceInformationManager.CompleteDeviceInformation(newDevice);

        //    while (newDevice != null)
        //    {
        //        if (!wiringInformationSource.HasDeviceDriver(newDevice)) // Falls keine wirings abgespeichert wurden
        //        {
        //            var result = Tapako.Utilities.WiringTool.WiringTool.Connect(device, newDevice);
        //            ResolveConnections(result);
        //        }
        //        else
        //        {
        //            // Lade gespeicherte Connections
        //            Logger.Debug("Loading connections for \"{0}\"", newDevice);
        //            newDevice.Connections = wiringInformationSource.GetDeviceInformations(newDevice).Connections;
        //        }

        //        newDeviceList.Add(newDevice);
        //        wiringInformationSource.StoreDeviceInformations(newDevice);

        //        newDevice = ChooseSubsystem(device); // neues Gerät hinzufügen
        //        newDevice = DeviceInformationManager.CompleteDeviceInformation(newDevice);

        //        if (newDevice == null) break;
        //        Logger.Info("New primitive device added: \"{0}\"", newDevice);
        //    }
        //    return newDeviceList;
        //}

        ///// <summary>
        ///// Wählt aus eine Liste (z.B. Dropdown-Liste) ein Gerätemodell aus, welches als Subsystem der SPS hinzugefügt wird
        ///// </summary>
        ///// <returns></returns>
        //private static IDevice ChooseSubsystem(IDevice device)
        //{
        //    Tuple<string, string> instanceInformations =
        //        Tapako.Utilities.DeviceSelector.DeviceSelector.SelectDevice(new List<string>(), connectionParent: device);

        //    if (string.IsNullOrEmpty(instanceInformations.Item1))
        //    {
        //        return null;
        //    }

        //    var newDevice = new DeviceBase()
        //    {
        //        Identification =
        //            new Identification
        //            {
        //                ModelNumber = instanceInformations.Item1,
        //                SerialNumber = instanceInformations.Item2
        //            }
        //    };
        //    return newDevice;
        //}

        ///// <summary>
        ///// Schreibe die ConnectionPoints des ccds in die entsprechenden ConnectionPoints der jeweils angeschlossenen Connection des devices
        ///// </summary>
        ///// <param name="result"></param>
        //private static void ResolveConnections(List<Tuple<Connection, Connection>> result)
        //{
        //    foreach (var tuple in result)
        //    {
        //        if (tuple.Item1 == null || tuple.Item2 == null)
        //        {
        //            Logger.Error(new ArgumentException("Eine connection des tuples ist null"));
        //            continue;
        //        }

        //        // Copy connection points in first ConnectionsObjects
        //        tuple.Item1.CounterpartConnections.Add(tuple.Item2);

        //        // Copy connection points in second ConnectionsObjects
        //        tuple.Item2.CounterpartConnections.Add(tuple.Item1);
        //    }
        //}
    }
}