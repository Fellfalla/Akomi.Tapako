using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Tapako.Utilities.WiringTool.Behaviors;
using Tapako.Utilities.WiringTool.ViewModel;
using Xceed.Wpf.Toolkit;

namespace Tapako.Utilities.WiringTool.View
{
    /// <summary>
    /// Interaction logic for WiringToolView.xaml
    /// Wiring: https://denisvuyka.wordpress.com/2007/10/21/wpf-diagramming-drawing-a-connection-line-between-two-elements-with-mouse/
    /// </summary>
    public partial class WiringToolView
    {

        //private readonly List<PathFigure> _connectors = new List<PathFigure>();
        //private Canvas _canvas = new Canvas();




        public WiringToolView()
        {
            InitializeComponent();

            if (Owner == null && Application.Current != null)
            {
                Owner = Application.Current.MainWindow;
            }

            PreviewKeyDown += HandleEsc;

            //Grid.SetRow(_canvas, 1);
            //Grid.SetColumn(_canvas, 1);

            //Root.Children.Add(_canvas);
            //ConnectionBehavior.SetMainCanvas(this,_canvas);    // Stelle dieses Canvas den Connection Behaviors zur verfügung
            MyMagnifier.Visibility = Visibility.Hidden;

            //DataContextChanged += (sender, args) => this.InvalidateProperty(WiringsProperty);
            DataContextChanged += (sender, args) => ListConnector.DrawExistingConnections();

            this.ContentRendered += (sender, args) => ListConnector.DrawExistingConnections();

            //OnPropertyChanged(Wirings);
            //Initialized += (sender, args) => RefreshWirings();
        }

        //public void RefreshWirings()
        //{
        //    if (Wirings != null) Wirings.Clear();

        //    var newWirings = ViewModel.WiredConnections.Select((connection) => Wiring.Create(connection.Item1, connection.Item2));

        //    Wirings = new ObservableCollection<Wiring>(newWirings);
        //}


        internal WiringToolView(WiringToolViewModel viewModel): this()
        {
            DataContext = viewModel;
        }

        private WiringToolViewModel ViewModel
        {
            get
            {
                return DataContext as WiringToolViewModel;
            }
            set { DataContext = value; }
        }

        private void ClickOk(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
            //Application.Current.Shutdown();
        }

        private void ClickReset(object sender, RoutedEventArgs e)
        {
            //this.ListConnector.Reset(this, EventArgs.Empty);

            var viewModel = DataContext as WiringToolViewModel;
            if (viewModel != null) viewModel.Reset();
            Wirings.Clear();
        }

        private void ClickCancel(object sender, RoutedEventArgs e)
        {
            ClickReset(sender,e);
            Close();
            //Application.Current.Shutdown();
        }


        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Escape) return;

            ClickReset(sender, e);
            Close();
        }

        public static readonly DependencyProperty WiringsProperty = DependencyProperty.Register(
            "Wirings", typeof(ObservableCollection<Wiring>), typeof(WiringToolView), new PropertyMetadata(new ObservableCollection<Wiring>()));

        public ObservableCollection<Wiring> Wirings
        {
            get { return (ObservableCollection<Wiring>) GetValue(WiringsProperty); }
            set { SetValue(WiringsProperty, value); }
        }


        private void ActivateMagnifierOnClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && !MyMagnifier.IsVisible) // Create Magnifier
            {
                PopupMagnifier();
            }
            else if (e.ChangedButton == MouseButton.Right && MyMagnifier.IsVisible) // Delete Magnifier
            {
                ClosePopupMagnifier();
            }
        }

        private void ActivateMagnifierOnZoom(object sender, MouseWheelEventArgs e)
        {
            if (!MyMagnifier.IsVisible)
            {
                PopupMagnifier();
            }
            Zoom(sender, e);
        }


        private void Zoom(object sender, MouseWheelEventArgs mouseWheelEventArgs)
        {
            const double min = .1;
            const double max = 1;
            const double coefficient = 0.0004;
            var delta = mouseWheelEventArgs.Delta * coefficient;
            if (MyMagnifier.ZoomFactor - delta < min)
            {
                MyMagnifier.ZoomFactor = min;
            }
            else if (MyMagnifier.ZoomFactor - delta > max)
            {
                MyMagnifier.ZoomFactor = max;
            }
            else
            {
                MyMagnifier.ZoomFactor -= delta;
            }
        }

        private void DeactivateMagnifierOnMouseLeave(object sender, MouseEventArgs e)
        {
            ClosePopupMagnifier();
        }

        private void PopupMagnifier()
        {
            double speedRatio = 4;


            DoubleAnimation animation = new DoubleAnimation()
            {
                From = 0,
                SpeedRatio = speedRatio,
                DecelerationRatio = 1
            };

            ThicknessAnimation borderAnimation = new ThicknessAnimation()
            {
                To = new Thickness(4),
                SpeedRatio = speedRatio,
                DecelerationRatio = 1
            };

            MyMagnifier.BeginAnimation(Magnifier.RadiusProperty, animation);
            MyMagnifier.BeginAnimation(BorderThicknessProperty, borderAnimation);
            MyMagnifier.Visibility = Visibility.Visible;
        }

        private void ClosePopupMagnifier()
        {
            double speedRatio = 4;

            DoubleAnimation shrinkAnimation = new DoubleAnimation()
            {
                To = 0,
                SpeedRatio = speedRatio,
                DecelerationRatio = 1
            };

            ThicknessAnimation borderAnimation = new ThicknessAnimation()
            {
                To = new Thickness(0),
                SpeedRatio = speedRatio,
                DecelerationRatio = 1
            };

            shrinkAnimation.Completed += (sender, args) => MyMagnifier.Visibility = Visibility.Hidden;
            MyMagnifier.BeginAnimation(Magnifier.RadiusProperty, shrinkAnimation);
            MyMagnifier.BeginAnimation(BorderThicknessProperty, borderAnimation);

        }

        private bool _clockwiseState;

        private void ChangeVisualState(object sender, EventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element != null)
            {
                VisualStateManager.GoToElementState(element, "RotateCounterClockwise", true);
                _clockwiseState = false;
            }
        }

        private void ToggleRotation(object sender, EventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element != null)
            {
                if (_clockwiseState)
                {
                    VisualStateManager.GoToElementState(element, RotateCounterClockwise.Name, true);
                    _clockwiseState = false;
                }
                else
                {
                    VisualStateManager.GoToElementState(element, RotateClockwise.Name, false);
                    _clockwiseState = true;
                }
            }
        }

        //private void ListConnector_OnWiringsChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    if (e == null)
        //    {
        //        return;
        //    }

        //    if (e.NewItems != null)
        //    {
        //        foreach (var newItem in e.NewItems.OfType<Wiring>())
        //        {
        //            ViewModel.WiredConnections.Add(newItem.Logical);
        //            //AddDataWiring(newItem); todo: Add Data wiring
        //        }
        //    }
        //    if (e.OldItems != null)
        //    {
        //        foreach (var oldItem in e.OldItems.OfType<Wiring>())
        //        {
        //            ViewModel.WiredConnections.Remove(oldItem.Logical);
        //            //RemoveDataWiring(newItem);
        //        }
        //    }   
        //}


        private void ClickRefresh(object sender, RoutedEventArgs e)
        {
            ListConnector.DrawExistingConnections();
        }
    }
}
