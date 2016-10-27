using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Akomi.InformationModel.Device;
using Akomi.Logger;
using Prism.Commands;
using Prism.Logging;
using Tapako.Utilities.UniversalHostSearch;

namespace Tapako.ViewModel.DesignTime
{
    [DesignTimeVisible(true)]
    public class TapakoDesignViewModel : TapakoViewModel
    {

        private static readonly Random _randomizer = new Random();

        private string _dummyText =
            @"Dies ist ein Dummytext.
Mich gibt es nur, da mein Entwickler nicht die nötige Phantasie hat, um ohne einen Dummytext zu wissen, was er fabriziert.";

        public new ObservableCollection<Message> LogMessages
        {
            get { return CreateDummyMessages(); }
            set { return; }
        }

        //public Progress Progress
        //{
        //    get { return Progress; }
        //    set {  }
        //}
        private ObservableCollection<Message> CreateDummyMessages()
        {
            var lm = new ObservableCollection<Message>();

            StringBuilder tabbedString = new StringBuilder();
            tabbedString.AppendLine("VerLongNameAtTheBeginningString\t:\tValueToShow");
            tabbedString.AppendLine("Short\t:\tValueToShow");
            tabbedString.AppendLine("MiddleLon\t:\tWithAVeryLongValueToShow");
            lm.Add(new Message(tabbedString.ToString(), Category.Info));

            for (int i = 0; i < 1; i++)
            {
                lm.Add(new Message("Exception " + i + ": " + _dummyText, Category.Exception));
                lm.Add(new Message("Info " + i + ": " + _dummyText, Category.Info));
                lm.Add(new Message("Warn " + i + ": " + _dummyText, Category.Warn));
                lm.Add(new Message("Debug " + i + ": " + _dummyText, Category.Debug));

            };
            lm.Add(new Message(tabbedString.ToString(), Category.Info));

            return lm;
        }

        public new ICollectionView FilteredLogMessages
        {
            get
            {
                var lm = CreateDummyMessages();
                ICollectionView source = new ListCollectionView(lm);
                return source;
            }
            set { return; }
        }

        //public string Subnet { get; set; }

        public new bool IsBusy
        {
            get { return true; }
            set { }
        }


        public new IDeviceTapakoViewModel SelectedHostDeviceTapako
        {
            get
            {
                var designViewModel = new DeviceTapakoDesignViewModel
                {
                    DeviceModel = DesignDataGenerator.CreateDummyDevice()
                };
                return designViewModel;
            }
            set { }
        }

        public new HostSearchViewModel HostSearchViewModel
        {
            get
            {
                return new HostSearchViewModel(new UniversalHostSearchViewModel());
            }
        }

        public new DelegateCommand LoadDeviceCommand { get; private set; }

        //public DelegateCommand<string> ScanNetworkForHostsCommand { get; }

        public bool HasDeviceDriver(IDevice device)
        {
            return true;
        }


        public new ObservableCollection<IDeviceTapakoViewModel> HostDeviceList
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
                SelectedHostDeviceTapako = deviceList.First();
                return deviceList;
            }
            set { return; }
        }

        private IDevice CreateHostDevice()
        {
            var host = DesignDataGenerator.CreateDummyDevice();
            return host;
        }

        public TapakoDesignViewModel() : base(new HostSearchDesignViewModel())
        {

        }
    }

}
