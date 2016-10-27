using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using HostSearch;
using TapakoInterfaces;
using TapakoPublicClasses;

namespace HostSearchGUI
{
    public partial class PlcSelectionGui : Window
    {
        public PlcSelectionGui()
        {
            ActiveDevices = new ObservableCollection<HostDevice>();
            List<string> knownMacAddresses = new List<string>();
            knownMacAddresses.Add("00-01-05-18-25-a2");
            knownMacAddresses.Add("00-01-05-18-25-a3");

            InitializeComponent();
            PlcDevices.ItemsSource = ActiveDevices;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //Task task1 = Task.Factory.StartNew(setPlcDeviceList);
            //activeDevices = new ObservableCollection<HostDevice>(getManualPlcDeviceList());
            progressBar.IsEnabled = true;
            IncreaseProgressBar(10);
            var list = await Task.Factory.StartNew((subnet) => getPlcDeviceList(subnet.ToString()), TextBoxSubnet.Text); // Markus: Dieser weg war Notwendig, um string Parameter der GUI zu übergeben
            //var list = await Task.Run((subnet) => getPlcDeviceList(subnet), TextBoxSubnet.Text);
            foreach (HostDevice plc in list) //getManualPlcDeviceList())
            {
                ActiveDevices.Add(plc);
            }

        }   

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            bool status = !Username.IsEnabled;

            Username.IsEnabled = status;
            Password.IsEnabled = status;
            Save_Password.IsEnabled = status;
        }

        private void IncreaseProgressBar(int time)
        {
            Thread timerThread = new Thread(() => 
            {
                for (int i = 0; i <= 100; ++i)
                {
                    Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate {
                            progressBar.Value += 1;
                        }));
                    Thread.Sleep(1000*time/100);
                }
            });
            timerThread.IsBackground = true;
            timerThread.Start();
        }

        private List<ITapakoDevice> getPlcDeviceList(String subnet)
        {
            List<ITapakoDevice> activeDevices = UniversalHostSearcher.SearchForSubSystems(subnet); //Subnetz IWB
            //List<HostDevice> ActiveDevices = UniversalHostSearcher.SearchForSubSystems("192.168.1."); //Subnetz Home
            //List<HostDevice> ActiveDevices = UniversalHostSearcher.GetManualPlcDeviceList("dummystring");
            //Console.WriteLine("Main");
            //foreach (HostDevice activeDevice in ActiveDevices)
            //{
            //    Console.WriteLine(activeDevice.Name);
            //    Console.WriteLine(activeDevice.IpAddress);
            //    Console.WriteLine(activeDevice.MacAddress + Environment.NewLine);
            //}
            //Console.ReadKey();
            return activeDevices;

        }


        public ObservableCollection<HostDevice> ActiveDevices { get; set; }
    }
}