using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Prism.Logging;
using Tapako.ViewModel;
using Image = System.Windows.Controls.Image;

namespace Tapako.View
{
    /// <summary>
    /// Interaktionslogik für MessageTemplate.xaml
    /// </summary>
    public partial class LoggerView : UserControl
    {
        private readonly ITapakoViewModel _viewModel;
        public LoggerView(ITapakoViewModel viewModel)
        {
            InitializeComponent();
            Style = (Style)FindResource(typeof(UserControl)); // Set Global Font styles etc.

            DataContext = viewModel;
            _viewModel = viewModel;
            var a = new ObservableCollection<int>();
            
            StringFormat.GenericDefault.SetTabStops(36f,new []{10f});

    }


        public void ToggleMessageFilter(object sender, MouseButtonEventArgs e)
        {
            Image filterImage = sender as Image;
            if (filterImage != null)
            {
                switch ((Category) filterImage.Tag)
                {
                       case Category.Info:
                        _viewModel.FilterInfoMessages ^= true; // toggle value
                        break;
                       case Category.Debug:
                        _viewModel.FilterDebugMessages ^= true; // toggle value
                        break;
                       case Category.Exception:
                        _viewModel.FilterErrorMessages ^= true; // toggle value
                        break;
                       case Category.Warn:
                        _viewModel.FilterWarningMessages ^= true; // toggle value
                        break;
                }
                e.Handled = true;
            }
        }

        private void CommandBinding_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Method to copy the selected item to clipboard
        /// source: http://stackoverflow.com/questions/19161397/copy-multiple-items-in-listbox-to-clipboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RightClickCopyCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;

            var builder = new StringBuilder();

            GetTextBlockStringFromDependencyObject(mi.DataContext as FrameworkElement, builder);

            Clipboard.SetText(builder.ToString());
        }

        private void KeyCopyLogExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox == null)
            {
                return;
            }

            var builder = new StringBuilder();
            foreach (var selectedItem in listBox.SelectedItems)
            {
                var depObj = listBox.ItemContainerGenerator.ContainerFromItem(selectedItem);
                GetTextBlockStringFromDependencyObject(depObj, builder);
            }

            Clipboard.SetText(builder.ToString());
        }

        private StringBuilder GetTextBlockStringFromDependencyObject(DependencyObject obj, StringBuilder builder = null)
        {
            if (obj != null)
            {
                if (builder == null)
                {
                    builder = new StringBuilder();
                }

                foreach (var textBox in FindChildControl<TextBlock>(obj).OfType<TextBlock>())
                {
                    builder.AppendLine(textBox.Text);
                }
            }
            return builder;
        }


        private IEnumerable<DependencyObject> FindChildControl<T>(DependencyObject control)
        {
            int childNumber = VisualTreeHelper.GetChildrenCount(control);
            for (int i = 0; i < childNumber; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(control, i);
                FrameworkElement fe = child as FrameworkElement;
                // Not a framework element or is null
                if (fe == null) break;

                if (child is T)
                {
                    // Found the control so return
                    yield return child;
                }
                else
                {
                    // Not found it - search children
                    foreach (var subChild in FindChildControl<T>(child))
                    {
                        yield return subChild;
                    }
                }
            }
            yield break;
        }

    }
}
