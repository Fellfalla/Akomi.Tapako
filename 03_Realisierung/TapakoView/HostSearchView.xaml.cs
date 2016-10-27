using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tapako.ViewModel;

namespace Tapako.View
{
    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    public partial class HostSearchView : UserControl
    {
        public HostSearchView(IHostSearchViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }

        public HostSearchView()
        {
            InitializeComponent();
            Style = (Style)FindResource(typeof(UserControl)); // Set Global Font styles etc.
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
            IDeviceTapakoViewModel selected = (IDeviceTapakoViewModel)mi.DataContext;

            if (selected != null && selected.DeviceModel != null)
            {
                Clipboard.SetText(selected.DeviceModel.ToString(true));
            }
        }

        private void KeyCopyLogExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox != null && listBox.SelectedItems != null)
            {
                string clipboardText = string.Empty;
                foreach (DeviceTapakoViewModel selectedItem in listBox.SelectedItems)
                {
                    if (selectedItem != null && selectedItem.DeviceModel != null)
                    {
                        clipboardText += selectedItem.DeviceModel.ToString(true) + Environment.NewLine;
                    }
                }
                
                Clipboard.SetData("Text", clipboardText);
            }
        }
    }
}
