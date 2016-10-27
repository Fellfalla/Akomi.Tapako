using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Akomi.InformationModel.Device;
using Prism.Commands;
using RandomExtension;

namespace Tapako.ViewModel.DesignTime
{
    public class DeviceTapakoDesignViewModel : DeviceTapakoViewModel
    {
        private readonly Random _randomizer = new Random();
        private IDevice _deviceModel;


        public new IDevice DeviceModel
        {
            get
            {
                if (_deviceModel == null)
                {
                    _deviceModel = CreateTapakoDevice();
                };
                return _deviceModel;
            }
            set { _deviceModel = value; }
        }

        public new ObservableCollection<IDeviceTapakoViewModel> SubDeviceViewModels
        {
            get
            {
                var result = new ObservableCollection<IDeviceTapakoViewModel>();
                foreach (var subDevice in DeviceModel.SubDevices)
                {
                    var viewModel = new DeviceTapakoViewModel();
                    viewModel.DeviceModel = subDevice;
                    result.Add(viewModel);
                }
                return result;
            }
            set
            {

            }
        }

        public new OpcUaServerControlViewModel ServerViewModel
        {
            get
            {
                return new OpcUaServerControlViewModel(this);
            }
        }



        public new IEnumerable<DelegateCommandBase> GetCommands()
        {
            return new[] {AnalyseDeviceCommand,
                CancelDeviceAnalysisCommand,
            RunPrimitiveCommunicationChannelDriverCommand};
        }

        public new bool HasDeviceDriver(IDevice device)
        {
            return true;
        }

        public new string BrowseName
        {
            get
            {
                if (DeviceModel != null)
                {
                    return DeviceModel.ToString();
                }
                return "No DeviceModel";
            }
        }

        private DeviceBase CreateTapakoDevice()
        {
            
            Random random = new Random(25);

            var dev = random.GetRandom<DeviceBase>();
            //dev.Skills.Add(new SkillDetectIdDefault());
            //dev.Skills.Add(new SkillShowDefault());
            return dev;
        }
    }
}
