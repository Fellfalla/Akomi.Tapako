using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Practices.Unity;
using Tapako.ViewModel;

namespace Tapako.View
{
    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    public partial class DeviceView : UserControl
    {
        private IDeviceTapakoViewModel _tapakoViewModel;

        public IDeviceTapakoViewModel TapakoViewModel
        {
            get { return _tapakoViewModel; }
            set
            {
                _tapakoViewModel = value;
                DataContext = value;
            }
        }


        [InjectionConstructor]
        public DeviceView()
        {
            InitializeComponent();
            Style = (Style)FindResource(typeof(UserControl)); // Set Global Font styles etc.
        }

        public DeviceView(IDeviceTapakoViewModel tapakoViewModel) : this()
        {
            TapakoViewModel = tapakoViewModel;
        }

        private async void Delete(object sender, ExecutedRoutedEventArgs e)
        {
            await TapakoViewModel.DeleteDeviceCommand.Execute();
        }
    }
}
