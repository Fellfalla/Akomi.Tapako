using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ExtensionMethodsCollection;
using Tapako.Utilities.DeviceSelector.ViewModel;

namespace Tapako.Utilities.DeviceSelector.View
{
    /// <summary>
    /// Interaction logic for DeviceSelectorView.xaml
    /// </summary>
    public partial class DeviceSelectorView
    {
        private readonly IDeviceSelectorViewModel _viewModel;

        internal DeviceSelectorView(IDeviceSelectorViewModel viewModel)
        {
            _viewModel = viewModel;
            DataContext = _viewModel;

            InitializeComponent();

            if (Owner == null)
            {
                Dispatcher.DoDispatchedAction(() => Owner = Application.Current.MainWindow);
            }

            DeviceModelBox.ItemsSource = _viewModel.ModelSuggestions.ToList();
            SerialNumberBox.ItemsSource = GetSerialNumberSuggestions();

            DeviceModelBox.LostFocus += UpdateSerialNumberSuggestions;

            PreviewKeyDown += HandleEsc;

            DeviceModelBox.GotFocus += DropDownOpen;
            SerialNumberBox.GotFocus += DropDownOpen;

            //Ergänzen des Device Models
            SerialNumberBox.SelectionChanged += SerialNumberSelectionChanged;
            //SerialNumberBox.DropDownClosed += CompleteDeviceModel;
            SerialNumberBox.PreviewTextInput += TextChanged;

            //Starteinstellungen
            SetFocusOnComboBox(SerialNumberBox);

        }

        private void SetFocusOnComboBox(ComboBox comboBox)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,
                new Action(delegate
                {
                    var textBox =
                        (comboBox.Template.FindName("PART_EditableTextBox", comboBox) as TextBox);
                    if (textBox != null)
                    {
                        textBox.Focus();
                        textBox.SelectionStart = textBox.Text.Length;
                    }
                    comboBox.IsDropDownOpen = false;
                }));
        }

        private void TextChanged(object sender, TextCompositionEventArgs e)
        {
            CompleteDeviceModel(((ComboBox) sender).Text);
        }

        private void SerialNumberSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedValue = ((ComboBox) sender).SelectedValue;
            if (selectedValue == null) return;
            CompleteDeviceModel(selectedValue.ToString());
        }

        private void DropDownOpen(object sender, RoutedEventArgs e)
        {
            ((ComboBox) sender).IsDropDownOpen = true;
        }

        private void CompleteDeviceModel(string enteredSerialNumber)
        {
            //nur wenn noch kein DeviceModel eingetragen ist
            //if (string.IsNullOrWhiteSpace(DeviceModelBox.Text))
            {
                foreach (var modelNumber in _viewModel.SerialNumberSuggestions)
                {
                    foreach (var serialNumber in modelNumber.Value)
                    {
                        if (serialNumber == enteredSerialNumber)
                        {
                            DeviceModelBox.Text = modelNumber.Key;
                        }
                    }
                }
            }
        }

        public new Tuple<string, string> ShowDialog()
        {
            base.ShowDialog();
            return new Tuple<string, string>(_viewModel.ModelNumber, _viewModel.SerialNumber);
        }

        private void window_MouseDown(object sender, MouseButtonEventArgs args)
        {
            Keyboard.ClearFocus();
        }


        private void UpdateSerialNumberSuggestions(object sender, RoutedEventArgs args)
        {
            SerialNumberBox.ItemsSource = GetSerialNumberSuggestions(DeviceModelBox.Text);
        }

        private List<string> GetSerialNumberSuggestions(string typedModelNumber = "")
        {
            List<string> suggestions;

            if (string.IsNullOrEmpty(typedModelNumber))
            {
                suggestions = _viewModel.SerialNumberSuggestions.SelectMany(x => x.Value).ToList();
            }
            else
            {
                _viewModel.SerialNumberSuggestions.TryGetValue(typedModelNumber, out suggestions);
            }
            return suggestions;
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        /// <summary>
        /// Beendet das Fenster und behälft die aktuelle eingabe im viewModel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickOk(object sender, RoutedEventArgs e)
        {
            Close();
            //Application.Current.Shutdown();   
        }


        private void ClickCancel(object sender, RoutedEventArgs e)
        {
            Reset();
            Close();
            //Application.Current.Shutdown();   
        }

        private void Reset()
        {
            _viewModel.Reset();
        }

    }
}