using System.Windows;
using System.Windows.Controls;

namespace Tapako.View
{
    /// <summary>
    /// Interaction logic for ProgressView.xaml
    /// </summary>
    public partial class ProgressView : UserControl
    {
        public ProgressView()
        {
            InitializeComponent();
            Style = (Style)FindResource(typeof(UserControl)); // Set Global Font styles etc.

        }

        //private void FrameworkElement_OnSourceUpdated(object sender, DataTransferEventArgs e)
        //{
        //    Storyboard sb = this.FindResource("AppearAnimation") as Storyboard;
        //    Storyboard.SetTarget(sb, e.TargetObject);
        //    sb.Begin();
        //}

        //private void Freezable_OnChanged(object sender, EventArgs e)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
