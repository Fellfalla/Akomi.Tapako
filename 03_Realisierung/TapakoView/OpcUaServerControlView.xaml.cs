using System.Windows;
using System.Windows.Controls;

namespace Tapako.View
{
    /// <summary>
    /// Interaktionslogik für OpcUaServerControlView.xaml
    /// </summary>
    public partial class OpcUaServerControlView : UserControl
    {
        public OpcUaServerControlView()
        {
            InitializeComponent();
            Style = (Style)FindResource(typeof(UserControl)); // Set Global Font styles etc.
        }
    }
}
