using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Akomi.InformationModel.Device;
using Akomi.InformationModel.Skills;
using Akomi.Visuals;
using Tapako.ViewModel;

namespace Tapako.View
{
    /// <summary>
    /// Interaktionslogik für PeripheryTree.xaml
    /// todo: Popup bei MouseOver -> Bild und einige weitere Informationen Anzeigen
    /// </summary>
    public partial class AnalysisView : UserControl
    {
        //private Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;
        private ITapakoViewModel viewModel;

        public AnalysisView()
        {
            InitializeComponent();
            Style = (Style)FindResource(typeof(UserControl)); // Set Global Font styles etc.
        }


        [ImportingConstructor]
        public AnalysisView(ITapakoViewModel viewModel) : this()
        {
            this.viewModel = viewModel;
            DataContext = viewModel;

            //viewModel.PropertyChanged += (sender, args) => _dispatcher.DoDispatchedAction(DeviceTree.UpdateLayout);
            //this.PreviewMouseDown += PopupDeviceInformations;
            //this.MouseMove += DismissPopupWindow;
        }

        private void OnDeviceTreeSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue == null)
            {
                viewModel.SelectedHostDeviceTapako = null;
            }
            if (e.NewValue is IDeviceViewModel)
            {
              viewModel.SelectedHostDeviceTapako = (IDeviceTapakoViewModel) e.NewValue;
            }
            if (e.NewValue is IDevice)
            {
              viewModel.SelectedHostDeviceTapako = new DeviceTapakoViewModel((IDevice) e.NewValue);
            }


        }

        private void OpenPopupOnItemClick(object sender, MouseButtonEventArgs e)
        {
            if (sender == null)
            {
                return;
            }

            var item = sender as TreeViewItem;
            if (item != null)
            {
                ClosePopup();
                OpenPopup(item.DataContext);
                e.Handled = true;
            }
        }


        private void OpenPopup(object dataContext)
        {
            IDeviceViewModel viewModel;
            if (dataContext is IDevice)
            {
                viewModel = new DeviceViewModel((IDevice) dataContext);
            }else if (dataContext is IDeviceViewModel)
            {
                viewModel = (IDeviceViewModel) dataContext;
            }
            else
            {
                return;
            }

            if (Popup != null)
            {
                //Application.Current.MainWindow.LostFocus += DismissPopupWindow;
                //Application.Current.MainWindow.MouseDown += DismissPopupWindow;
                Popup.Child = new DeviceContextView(viewModel);;
                Popup.Visibility = Visibility.Visible;
                Popup.IsOpen = true;
            }
            else
            {
                throw new NullReferenceException("Popup is null");
            }
        }

        private void ClosePopup()
        {
            if (Popup != null)
            {
                //Application.Current.MainWindow.LostFocus -= DismissPopupWindow;
                //Application.Current.MainWindow.MouseDown -= DismissPopupWindow;
                Popup.Visibility = Visibility.Collapsed;
                Popup.DataContext = null;
                Popup.IsOpen = false;
            }
        }

        private void DismissPopupWindow(object sender, RoutedEventArgs e)
        {
            ClosePopup();
        }

        private void DismissPopupWindow(object sender, MouseEventArgs mouseEventArgs)
        {
            ClosePopup();
        }

        private void Popup_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        //private async void UIElement_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    // execute Skill, otherwise command won't be fired, no idea why
        //    IDeviceViewModel deviceViewModel = (IDeviceViewModel)Popup.DataContext;
        //    await deviceViewModel.ExecuteSkillCommand.Execute(((FrameworkElement)sender).DataContext as ISkill);
        //    e.Handled = true;
        //}
    }


}
