using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Tapako.Design.Extensions;
using Tapako.Utilities.WiringTool.Extensions;
using Xceed.Wpf.AvalonDock.Controls;

namespace Tapako.Utilities.WiringTool.View
{
    /// <summary>
    ///     Interaction logic for ListConnectorView.xaml
    /// </summary>
    public partial class ListConnectorView : UserControl
    {
        public static readonly DependencyProperty LeftListProperty = DependencyProperty.Register(
            "LeftList", typeof(IEnumerable), typeof(ListConnectorView),
            new PropertyMetadata(default(IEnumerable), DrawExistingConnectionsCallback));

        public static readonly DependencyProperty RightListProperty = DependencyProperty.Register(
            "RightList", typeof(IEnumerable), typeof(ListConnectorView),
            new PropertyMetadata(default(IEnumerable), DrawExistingConnectionsCallback));

        private static void DrawExistingConnectionsCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var listConnectorView = dependencyObject as ListConnectorView;

            if (listConnectorView != null)
            {
                listConnectorView.DrawExistingConnections();
                listConnectorView.DrawExistingConnections();
            }
        }

        public static readonly DependencyProperty WiringObjectsProperty = DependencyProperty.Register(
            "WiringObjects", typeof(ObservableCollection<Wiring>), typeof(ListConnectorView),
            new PropertyMetadata(default(ObservableCollection<Wiring>), NewConnectionList));

        private readonly Dictionary<Wiring, PathFigure> _wiringToFigureRegister = new Dictionary<Wiring, PathFigure>();

        private readonly Dictionary<PathFigure, PathGeometry> _figureToGeometryRegister =
            new Dictionary<PathFigure, PathGeometry>();

        private readonly Dictionary<PathGeometry, Path> _geometryToPathRegister = new Dictionary<PathGeometry, Path>();

        public static readonly DependencyProperty DraggingStrokeThicknessProperty = DependencyProperty.Register(
            "DraggingStrokeThickness", typeof(double), typeof(ListConnectorView), new PropertyMetadata((double) 1.2));

        public double DraggingStrokeThickness
        {
            get { return (double) GetValue(DraggingStrokeThicknessProperty); }
            set { SetValue(DraggingStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty ConnectedStrokeThicknessProperty = DependencyProperty.Register(
            "ConnectedStrokeThickness", typeof(double), typeof(ListConnectorView), new PropertyMetadata((double) 2));

        public double ConnectedStrokeThickness
        {
            get { return (double) GetValue(ConnectedStrokeThicknessProperty); }
            set { SetValue(ConnectedStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty HoveringStrokeThicknessProperty = DependencyProperty.Register(
            "HoveringStrokeThickness", typeof(double), typeof(ListConnectorView), new PropertyMetadata((double) 3));

        public double HoveringStrokeThickness
        {
            get { return (double) GetValue(HoveringStrokeThicknessProperty); }
            set { SetValue(HoveringStrokeThicknessProperty, value); }
        }

        public ListConnectorView()
        {
            InitializeComponent();

            WiringObjects = new ObservableCollection<Wiring>();

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                // Design-mode specific functionality
                CreateDesignTimeData();
            }
        }

        //public void RaiseWiringsChanged()
        //{
        //    if (WiringsChanged != null) WiringsChanged(this, null);
        //}

        public void DrawExistingConnections()
        {
            if (LeftListView == null || RightListView == null || LeftList == null || RightList == null || WiringObjects == null)
            {
                return;
            }
            //LeftListView.ItemsSource = CollectionViewSource.GetDefaultView(LeftList);
            //RightListView.ItemsSource = CollectionViewSource.GetDefaultView(RightList);

            //((ICollectionView)(LeftListView.ItemsSource)).Refresh();
            //((ICollectionView)(RightListView.ItemsSource)).Refresh();

            var hiddenConnections =
                WiringObjects.Where(
                    wiring => wiring.Visual == null || wiring.Visual.Item1 == null || wiring.Visual.Item2 == null);

            foreach (var hiddenConnection in hiddenConnections)
            {
                FrameworkElement visual1 = null, visual2 = null;

                if (hiddenConnection.Visual == null)
                {
                    hiddenConnection.Visual = VisualWiring.Create(null, null);
                }

                // Get visual1 for logical1
                if (hiddenConnection.Visual.Item1 == null)
                {
                    visual2 = hiddenConnection.Visual.Item2;
                    visual1 = GetVisualOfLogical(hiddenConnection.Logical.Item1);
                }

                hiddenConnection.Visual = VisualWiring.Create(visual1, visual2);

                if (hiddenConnection.Visual.Item2 == null)
                {
                    visual1 = hiddenConnection.Visual.Item1;
                    visual2 = GetVisualOfLogical(hiddenConnection.Logical.Item2);
                }
                else
                {
                    // both are not null (this is code cannot be reached due to creation of hiddenConnections)
                }

                hiddenConnection.Visual = VisualWiring.Create(visual1, visual2);
                RefreshConnection(hiddenConnection);
            }
        }

        private FrameworkElement GetVisualOfLogical(object logical)
        {
            FrameworkElement result = null;
            var container = LeftListView.ItemContainerGenerator.ContainerFromItem(logical) as FrameworkElement;
            if (container == null)
            {
                container = RightListView.ItemContainerGenerator.ContainerFromItem(logical) as FrameworkElement;
            }

            if (container != null) result = container.FindChild<Rectangle>("Node");

            //result = container;
            return result;
        }

        public IEnumerable LeftList
        {
            get { return (IEnumerable) GetValue(LeftListProperty); }
            set
            {
                SetValue(LeftListProperty, value);
                DrawExistingConnections();
            }
        }

        public IEnumerable RightList
        {
            get { return (IEnumerable) GetValue(RightListProperty); }
            set
            {
                SetValue(RightListProperty, value);
                DrawExistingConnections();
            }
        }

        public ObservableCollection<Wiring> WiringObjects
        {
            get { return (ObservableCollection<Wiring>) GetValue(WiringObjectsProperty); }
            set { SetValue(WiringObjectsProperty, value); }
        }

        private void CreateDesignTimeData()
        {
            var first = new List<object>();

            for (var i = 0; i < 30; i++)
            {
                var con = "ParentTestConnection" + i;
                first.Add(con);
            }

            var second = new List<object>();
            for (var i = 0; i < 3; i++)
            {
                var con = "ChildTestConnection" + i;
                second.Add(con);
            }

            LeftList = first;
            RightList = second;

            //var dummyConnection = new List<LogicalWiring>();
            //dummyConnection.Add(new LogicalWiring(first.FirstOrDefault(), second.FirstOrDefault()));
            //dummyConnection.Add(new LogicalWiring(first.ElementAt(1), second.ElementAt(1)));

            //LogicalWiringObjects = dummyConnection.ToObservableCollection();
        }

        private static void NewConnectionList(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var c = (ListConnectorView) dependencyObject;
            dependencyPropertyChangedEventArgs.AssignOnCollectionChangedHandler(c.WiringsChanged);
            dependencyPropertyChangedEventArgs.AssignOnCollectionChangedHandler(c.OnWiringListChanged);

            // erase all removed Wirings
            var oldCollection = dependencyPropertyChangedEventArgs.OldValue as IEnumerable<Wiring>;
            if (oldCollection != null)
            {
                foreach (var oldWiring in oldCollection)
                {
                    c.EraseConnection(oldWiring);
                }
            }

            DrawExistingConnectionsCallback(dependencyObject, dependencyPropertyChangedEventArgs);

            // draw all new existing logical connections
            var newCollection = dependencyPropertyChangedEventArgs.NewValue as IEnumerable<Wiring>;
            if (newCollection != null)
            {
                foreach (var newWiring in newCollection)
                {
                    if (newWiring.Logical == null || newWiring.Logical.Item1 == null || newWiring.Logical.Item2 == null)
                    {
                        Trace.TraceWarning("Initial Logical Wirings contain null references!");
                        continue;
                    }

                    // Search for Visual nodes if no null items are wired
                    FrameworkElement anchor1, anchor2;
                    //c.GetVisualAnchors(newWiring.Logical, out anchor1, out anchor2);
                    c.DrawConnection(newWiring);
                }
            }
        }

        public event NotifyCollectionChangedEventHandler WiringsChanged;

        private void OnWiringListChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Trace.TraceInformation("Connections have changed");
            if (e.NewItems != null)
            {
                foreach (var newItem in e.NewItems.OfType<Wiring>())
                {
                    DrawConnection(newItem);
                    //AddDataWiring(newItem); todo: Add Data wiring
                }
            }
            if (e.OldItems != null)
            {
                foreach (var oldItem in e.OldItems.OfType<Wiring>())
                {
                    EraseConnection(oldItem);
                    //RemoveDataWiring(newItem);
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (var erasedWiring in _wiringToFigureRegister.Keys.ToArray())
                    // To Array because the dictionary with change in this loop
                {
                    EraseConnection(erasedWiring);
                    //RemoveDataWiring(newItem);
                }
            }
        }

        //public void Reset(object sender, EventArgs e)
        //{
        //    WiringObjects.Clear();
        //    MainCanvas.Children.Clear();

        //}


        /// <summary>
        /// Removes and redraws the visual connection line
        /// </summary>
        /// <param name="wiring"></param>
        private void RefreshConnection(Wiring wiring)
        {
            try
            {
                EraseConnection(wiring);
                DrawConnection(wiring);
            }
            catch (KeyNotFoundException)
            {
            }
        }

        private void EraseConnection(Wiring wiring)
        {
            var figure = _wiringToFigureRegister[wiring];
            var gemoetry = _figureToGeometryRegister[figure];
            var path = _geometryToPathRegister[gemoetry];
            MainCanvas.Children.Remove(path);

            UnregisterWiringPath(wiring);
        }

        private void DrawConnection(Wiring wiring)
        {
            FrameworkElement source;
            FrameworkElement target;
            if (wiring == null || wiring.Visual == null)
            {
                return;
            }

            wiring.Visual.GetItems(out source, out target);
            //var targetListView = GetObjectAtPoint<ListView>(_parent, Mouse.GetPosition(_parent));
            //var endPoint = e.GetPosition(targetListView);

            //ConnectObjectsLogical(source, target);
            // Get the EndPoint


            //var window = GetMainControl();
            //var nodeContainerOfSource = window.FindChildsByBoundObject<ListBox>(source).FirstOrDefault();
            //var nodeContainerOfTarget = window.FindChildsByBoundObject<ListBox>(target).FirstOrDefault();

            //var lineStartPoint = GetMiddlePointofRectangleOfNodeContainer(nodeContainerOfSource);
            //var lineEndPoint = GetMiddlePointofRectangleOfNodeContainer(nodeContainerOfTarget);

            var lineStartPoint = source.GetMiddlePoint(MainCanvas);
            var lineEndPoint = target.GetMiddlePoint(MainCanvas);

            //_sourceItem = source;
            PathFigure figure;
            PathGeometry geometry;
            Path path;

            var addedToCanvas = CreateNewConnector(out figure, out geometry, out path);
            figure.StartPoint = lineStartPoint;
            figure.ConnectConnectorWithPoint(lineEndPoint, 30d);

            path.IsMouseDirectlyOverChanged += PathOnIsMouseDirectlyOverChanged;
            path.PreviewMouseMove += HigherStrokeThickness;
            path.PreviewMouseDown += LowerStrokeThickness;

            // Add the Visual Objects to the register for possible deletions
            RegisterWiringPath(wiring, figure, geometry, path);

            // Aktiviere das Entfernen der Connection
            //_activePath.PreviewMouseRightButtonDown += DisconnectObjects;
            //_activePath.MouseEnter += HigherStrokeThickness;
            //_activePath.PreviewMouseMove += HigherStrokeThickness;
            //_activePath.MouseLeave += LowerStrokeThickness;

            //ResetConnector();
        }

        private void PathOnIsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if ((bool) dependencyPropertyChangedEventArgs.NewValue)
            {
                HigherStrokeThickness(sender, null);
            }
            else
            {
                LowerStrokeThickness(sender, null);
            }
        }

        private void LowerStrokeThickness(object sender, MouseEventArgs e)
        {
            var path = sender as Path;
            if (path != null) path.StrokeThickness = DraggingStrokeThickness;
        }

        private void HigherStrokeThickness(object sender, MouseEventArgs mouseEventArgs)
        {
            var path = sender as Path;
            if (path != null) path.StrokeThickness = 25;
        }


        private void RegisterWiringPath(Wiring wiring, PathFigure figure, PathGeometry geometry, Path path)
        {
            _wiringToFigureRegister[wiring] = figure;
            _figureToGeometryRegister[figure] = geometry;
            _geometryToPathRegister[geometry] = path;
        }

        private void UnregisterWiringPath(Wiring wiring)
        {
            var figure = _wiringToFigureRegister[wiring];
            var geometry = _figureToGeometryRegister[figure];

            _geometryToPathRegister.Remove(geometry);
            _figureToGeometryRegister.Remove(figure);
            _wiringToFigureRegister.Remove(wiring);
        }

        /// <summary>
        /// Returns fales if there is no Canvas available for the <paramref name="path"/> to be added.
        /// </summary>
        /// <param name="figure"></param>
        /// <param name="geometry"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool CreateNewConnector(out PathFigure figure, out PathGeometry geometry, out Path path)
        {
            figure = new PathFigure();
            geometry = new PathGeometry();

            geometry.Figures.Add(figure);

            path = new Path
            {
                Stroke = Brushes.Black,
                StrokeThickness = ConnectedStrokeThickness,
                Data = geometry,
                IsHitTestVisible = false
            };
            //_pathList.Add(_activePath);
            if (MainCanvas == null)
            {
                Console.WriteLine("No Canvas added for Connection Behavior");
                return false;
            }
            else
            {
                MainCanvas.Children.Add(path);
            }

            return true;
        }

        //private static FrameworkElement GetVisualAnchor(FrameworkElement element, object context)
        //{
        //    return element.FindChildsByBoundObject<FrameworkElement>(context).FirstOrDefault();
        //}

        //private ContentControl GetMainControl()
        //{
        //    ContentControl result = this.FindLogicalAncestor<UserControl>();
        //    if (result == null)
        //    {
        //        result = this.FindLogicalAncestor<Window>();
        //    }
        //    return result;
        //}


        private void OnConnectionListsScrolled(object sender, ScrollChangedEventArgs e)
        {
            if (WiringObjects != null)
                foreach (var visualWiringObject in WiringObjects)
                {
                    RefreshConnection(visualWiringObject);
                }
        }

        private void MainCanvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            var canvas = sender as Canvas;
            if (canvas == null)
            {
                return;
            }

            // reset thickness of all Connections
            foreach (var child in canvas.Children.OfType<Path>())
            {
                child.StrokeThickness = ConnectedStrokeThickness;
            }


            var point = e.GetPosition(canvas);
            var line = VisualTreeHelper.HitTest(canvas, point).VisualHit as Path;
            if (line != null)
            {
                line.StrokeThickness = HoveringStrokeThickness;
            }
        }

        private void MainCanvas_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var canvas = sender as Canvas;
            if (canvas == null)
            {
                return;
            }

            var point = e.GetPosition(canvas);
            var line = VisualTreeHelper.HitTest(canvas, point).VisualHit as Path;
            if (line != null)
            {
                var geometry = _geometryToPathRegister.First(pair => Equals(pair.Value, line)).Key;

                var figure = _figureToGeometryRegister.First(pair => Equals(pair.Value, geometry)).Key;

                var wiring = _wiringToFigureRegister.First(pair => Equals(pair.Value, figure)).Key;

                WiringObjects.Remove(wiring);
            }

        }
    }
}