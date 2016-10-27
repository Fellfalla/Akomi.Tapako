using System.Collections.Generic;
using System.Collections.ObjectModel;
using Akomi.InformationModel.Device;
using Tapako.Utilities.UniversalHostSearch;

namespace Tapako.ViewModel.DesignTime
{
    public class HostSearchDesignViewModel : HostSearchViewModel
    {

        public new ObservableCollection<IDeviceTapakoViewModel> NetworkDevices
        {
            get
            {
                var deviceList = new ObservableCollection<IDeviceTapakoViewModel>();
                for (int i = 0; i < 10; i++)
                {
                    var newDevice = CreateHostDevice();
                    newDevice.SubDevices = new List<IDevice>();
                    for (int j = 0; j < 3; j++)
                    {
                        newDevice.SubDevices.Add(DesignDataGenerator.CreateDummyDevice());
                    }
                    deviceList.Add(new DeviceTapakoViewModel(newDevice));
                }
                return deviceList;
            }
            set { return; }
        }


        private static DeviceBase CreateHostDevice()
        {
            var host = DesignDataGenerator.CreateDummyDevice();
            return host;
        }

        public HostSearchDesignViewModel() : base(new UniversalHostSearchViewModel())
        {
        }
    }
}
