using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Tapako.Utilities.UniversalHostSearch
{
    /// <summary>
    /// Interaction logic for UniversalHostSearchView.xaml
    /// </summary>
    public partial class UniversalHostSearchView : UserControl
    {
        public UniversalHostSearchViewModel ViewModel;

        public UniversalHostSearchView(UniversalHostSearchViewModel viewModel)
        {
            this.ViewModel = viewModel;
            DataContext = this.ViewModel;
            InitializeComponent();

            var suggestions = UniversalHostSearcher.GetIpAddressSuggestions();
            var suggestionsArray = suggestions as string[] ?? suggestions.ToArray();
            SubnetBox.ItemsSource = suggestionsArray;
            SubnetBox.Text = suggestionsArray.FirstOrDefault();

            //SubnetBox.GotFocus += OpenDropDownMenu;
        }

        private static void OpenDropDownMenu(object sender, RoutedEventArgs e)
        {
            ((ComboBox) sender).IsDropDownOpen = true;
        }

        public UniversalHostSearchView()
        {
            new UniversalHostSearchView(new UniversalHostSearchViewModel());
        }
    }
}
