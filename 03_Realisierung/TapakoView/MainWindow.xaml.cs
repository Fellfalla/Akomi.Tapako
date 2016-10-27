using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ExtensionMethodsCollection;
using Tapako.ViewModel;

namespace Tapako.View
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// Learn WPF with http://www.wpftutorial.net
    /// todo: Credits for symbold from flaticon.com -> credits to freepik, ses licence in assets folder
    /// </summary>
    public partial class MainWindow
    {
        private readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;

        public MainWindow(ITapakoViewModel view)
        {
            InitializeComponent();
            Style = (Style) FindResource(typeof (Window)); // Set Global Font styles etc.
            DataContext = view;
        }


        /// <summary>
        /// TitleBar_MouseDown - Drag if single-click, resize if double-click
        /// </summary>
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                if (e.ClickCount == 2)
                {
                    AdjustWindowSize();
                }
                else
                {
                    Application.Current.MainWindow.DragMove();
                }
        }

        /// <summary>
        /// CloseButton_Clicked
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// MaximizedButton_Clicked
        /// </summary>
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            AdjustWindowSize();
        }

        /// <summary>
        /// Minimized Button_Clicked
        /// </summary>
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Adjusts the WindowSize to correct parameters when Maximize button is clicked
        /// </summary>
        private void AdjustWindowSize()
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                //MaximizeButton.Content = "1";
            }
            else
            {
                WindowState = WindowState.Maximized;
                //MaximizeButton.Content = "2";
            }
        }


        //private void AnimateGifIfBusy(object sender, RoutedEventArgs e)
        //{
        //    Image image = sender as Image;
        //    if (sender != null)
        //    {
        //        ImageAnimationController animationController = ImageBehavior.GetAnimationController(image);

        //        if (((ITapakoViewModel) DataContext).IsBusy && animationController.IsPaused ||
        //            animationController.IsComplete)
        //        {
        //            animationController.Play();
        //        }
        //        else
        //        {
        //            animationController.Pause();
        //        }
        //    }
        //}

        //private void StartAnimation(object sender, RoutedEventArgs args = null)
        //{
        //    Image image = sender as Image;
        //    if (sender != null)
        //    {
        //        ImageAnimationController animationController = ImageBehavior.GetAnimationController(image);
        //        //animationController.Play();
        //        _dispatcher.DoDispatchedAction(animationController.Play);
        //    }
        //}

        //private void PauseAnimation(object sender, RoutedEventArgs args = null)
        //{
        //    Image image = sender as Image;
        //    if (sender != null)
        //    {
        //        ImageAnimationController animationController = ImageBehavior.GetAnimationController(image);
        //        animationController.CurrentFrameChanged += PauseOnFrame;
        //    }
        //}

        //private void PauseAnimation(object sender, EventArgs eventArgs)
        //{
        //    var controller = sender as ImageAnimationController;
        //    if (controller != null && controller.CurrentFrame <= 1)
        //    {
        //        //controller.Pause()
        //        _dispatcher.DoDispatchedAction(controller.Pause);
        //        controller.CurrentFrameChanged -= PauseAnimation;
        //    }
        //}

        //private void RegisterIsBusyEvent(object sender, RoutedEventArgs e)
        //{
        //    Image image = sender as Image;
        //    ImageAnimationController controller = ImageBehavior.GetAnimationController(image);
        //    if (sender != null && controller != null)
        //    {
        //        controller.Pause();
        //        ((ITapakoViewModel) DataContext).PropertyChanged += (o, args) =>
        //        {
        //            if (args.PropertyName == "IsBusy")
        //            {
        //                if ((bool) o.GetType().GetProperty(args.PropertyName).GetValue(o))
        //                {
        //                    //ImageBehavior.RemoveAnimationCompletedHandler(image, PauseAnimation);
        //                    StartAnimation(image);

        //                    ImageBehavior.AddAnimationCompletedHandler(image, StartAnimation);
        //                }
        //                else
        //                {
        //                    ImageBehavior.RemoveAnimationCompletedHandler(image, StartAnimation);
        //                    controller.CurrentFrameChanged += PauseAnimation;
        //                    //Action pauseAction = () => PauseAnimation(sender, e);
        //                    //controller.CurrentFrameChanged += (s, arg) => _dispatcher.BeginInvoke(DispatcherPriority.DataBind, pauseAction);
        //                    //ImageBehavior.AddAnimationCompletedHandler(image, PauseAnimation);
        //                }
        //            }
        //        };
        //    }
        //}

        private void DeleteDevice(object sender, ExecutedRoutedEventArgs e)
        {
            var content = DeviceView.Content as UserControl;
            if (content != null)
            {
                var deviceView = content.DataContext as DeviceTapakoViewModel;
                if (deviceView != null) deviceView.DeleteDeviceCommand.Execute();
            }
        }
    }
}