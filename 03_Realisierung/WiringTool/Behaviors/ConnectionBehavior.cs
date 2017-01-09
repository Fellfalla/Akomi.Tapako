using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Shapes;
using Tapako.Design.Extensions;
using Tapako.Utilities.WiringTool.Extensions;
using Tapako.Utilities.WiringTool.View;
using Xceed.Wpf.AvalonDock.Controls;

namespace Tapako.Utilities.WiringTool.Behaviors
{
    /// <summary>
    /// Das Verhalten, welches benötigt wird, um 2 Elemente per Drag & Drop in einer TupleList zu verknüpfen
    /// How to Drag & Drop Between Lists http://www.zagstudio.com/blog/488
    /// </summary>
    public class GenericConnectionBehavior<T> : Behavior<ItemsControl>
        where T: FrameworkElement
    {
        #region Fields

        private ContentControl _parent;

        private Point _startPoint;
        private PathFigure _activeConnectorLine;
        private Path _activePath;

        private bool _isConnectionProcessStarted;

        private T _sourceItem;
        private T _targetItem;

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty MainCanvasProperty = DependencyProperty.Register(
            "MainCanvas", typeof(Canvas), typeof(GenericConnectionBehavior<T>), new PropertyMetadata(default(Canvas)));

        public Canvas MainCanvas
        {
            get { return (Canvas) GetValue(MainCanvasProperty); }
            set { SetValue(MainCanvasProperty, value); }
        }

        public static readonly DependencyProperty DefaultAnchorNameProperty = DependencyProperty.Register(
            "DefaultAnchorName", typeof(string), typeof(GenericConnectionBehavior<T>), new PropertyMetadata("Node"));

        public string DefaultAnchorName
        {
            get { return (string) GetValue(DefaultAnchorNameProperty); }
            set { SetValue(DefaultAnchorNameProperty, value); }
        }

        public static readonly DependencyProperty ConnectionsProperty = DependencyProperty.Register(
            "Connections", typeof(IList<Wiring>), typeof(GenericConnectionBehavior<T>), new PropertyMetadata(default(IList<Wiring>)));

        public IList<Wiring> Connections
        {
            get { return (IList<Wiring>) GetValue(ConnectionsProperty); }
            set { SetValue(ConnectionsProperty, value); }
        }

        public static readonly DependencyProperty HorizontalBezierOffsetProperty = DependencyProperty.RegisterAttached(
            "HorizontalBezierOffset", typeof(double), typeof(GenericConnectionBehavior<T>), new PropertyMetadata(30d));

        public static void SetHorizontalBezierOffset(DependencyObject element, double value)
        {
            element.SetValue(HorizontalBezierOffsetProperty, value);
        }

        public static double GetHorizontalBezierOffset(DependencyObject element)
        {
            return (double) element.GetValue(HorizontalBezierOffsetProperty);
        }

        #endregion

        protected override void OnAttached()
        {
            _parent = GetMainControl();

            AssociatedObject.PreviewMouseLeftButtonDown += SetStartPoint;

            AssociatedObject.PreviewMouseLeftButtonDown += RegisterDragInitializingMethods;

            AssociatedObject.PreviewMouseLeftButtonUp -= UnregisterDragInitializingMethods;
        }

        private void UnregisterDragInitializingMethods(object sender, MouseButtonEventArgs e)
        {
            AssociatedObject.PreviewMouseMove -= VerifyDragConditionsAndInitializeDrag;
            AssociatedObject.PreviewMouseLeftButtonUp -= FinishConnectionProcess;
        }

        private void RegisterDragInitializingMethods(object sender, MouseButtonEventArgs e)
        {
            AssociatedObject.PreviewMouseMove += VerifyDragConditionsAndInitializeDrag;
            AssociatedObject.PreviewMouseLeftButtonUp += FinishConnectionProcess;
        }

        private void VerifyDragConditionsAndInitializeDrag(object sender, MouseEventArgs mouseEventArgs)
        {
            // Get the current mouse position
            Point mousePos = mouseEventArgs.GetPosition(AssociatedObject);
            Vector diff = _startPoint - mousePos;

            if (!_isConnectionProcessStarted &&
                mouseEventArgs.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                 Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                InitiateConnectionProcess();
                AssociatedObject.PreviewMouseMove -= VerifyDragConditionsAndInitializeDrag;
            }
        }


        /// <summary>
        /// Wird unter den folgenden bedingungen ausgeführt:
        ///     - Die linke Maustaste wurde gedrückt
        ///     - Der Mauszeiger hat sich bei gedrückter Maustaste um einen Minimalwert bewegt
        /// </summary>
        private void InitiateConnectionProcess()
        {
            if (!_isConnectionProcessStarted)
            {
                _isConnectionProcessStarted = true;

                // Erstelle Connection-Animation und Registriere Mausverfolgung
                _parent.PreviewMouseMove += FollowMouseWithConnector;

                // Speichere Quell-Daten in lokaler variable
                _sourceItem = GetObjectAtPoint<ListBoxItem>(AssociatedObject, _startPoint) as T;
            }
        }


        /// <summary>
        /// Wird unter folgenden Bedingungen ausgeführt:
        ///     - Die linke Maustaste wird losgelassen
        ///     - Ein Connection Prozess war im gange
        /// </summary>
        private void FinishConnectionProcess(object sender, MouseEventArgs e)
        {
            if (_isConnectionProcessStarted)
            {
                // lokale Variablen
                var targetListView = GetObjectAtPoint<ItemsControl>(_parent, Mouse.GetPosition(_parent));
                var endPoint = e.GetPosition(targetListView);

                // Detektiere Drop-Ziel
                if (targetListView != null)
                {
                    _targetItem = GetObjectAtPoint<ListBoxItem>(targetListView, endPoint) as T;
                }
                _parent.PreviewMouseMove -= FollowMouseWithConnector;

                MainCanvas.Children.Remove(_activePath);
                if (_sourceItem != null && _targetItem != null && !ReferenceEquals(_sourceItem, _targetItem))
                    // Beide ConnectionPoints müsse ungleich null und verschieden sein
                {
         

                    // Verknüpfe Quellobjekt und Zielobjekt
                    // logisch: // -->> Ausgelagert
                    //ConnectObjectsLogical(_sourceItem, _targetItem);

                    // graphisch:
                    var wiring = ConnectObjectsVisual(_sourceItem, _targetItem);

                    if (wiring != default(Wiring))
                    {
                        ConnectObjectsLogical(wiring);
                        Connections.Add(wiring);
                    }
                }
                //else
                //{
                //    // eines der beiden objekte ist null, somit macht das connecten keinen sinn
                //    Canvas.Children.Remove(_activePath);
                //}


                // Reset Connektor, lokale Objekte und Events
                ResetConnector();

                _isConnectionProcessStarted = false;
            }
        }

        private void ConnectObjectsLogical(Wiring wiring)
        {
            // Get the associated data of the connected visual elements
            var data1 = wiring.Visual.Item1.DataContext;
            var data2 = wiring.Visual.Item2.DataContext;

            wiring.Logical = LogicalWiring.Create(data1, data2);
        }

        /// <summary>
        /// Registriert 2 Verbundene Objekte im ViewModel
        /// </summary>
        /// <param name="sourceItem"></param>
        /// <param name="targetItem"></param>
        private Wiring ConnectObjectsVisual<TFrameEle>(TFrameEle sourceItem, TFrameEle targetItem) where TFrameEle: FrameworkElement
        {
            var connections = Connections;
            Wiring wiring = default(Wiring);
            if (connections == null)
            {
                throw new NullReferenceException("Visual Connections have not been bound");
                //// set Connections and gather new value
                //SetVisualConnections(this, new List<VisualWiring>());
                //connections = GetVisualConnections(this);
            }

            var sourceNode = GetElementAnchor<Rectangle>(sourceItem);
            var targetNode = GetElementAnchor<Rectangle>(targetItem);
            bool isNotConnected;
            try
            {
                isNotConnected = connections.Any(tuple => tuple.Visual.Contains(sourceNode, targetNode)) == false;
            }
            catch (NullReferenceException)
            {
                // If Property Visual or Logical in Wiring is null, there is no existing connection
                isNotConnected = true;
            }

            if (sourceItem != null && targetItem != null && isNotConnected)
            {
                wiring = Wiring.Create(sourceNode, targetNode);
                //connections.Add(wiring);
                Console.WriteLine("Created wiring between " + sourceItem + " and " + targetItem);
            }

            return wiring;
        }

        private FrameworkElement GetElementAnchor<TAnchror>(FrameworkElement element) where TAnchror : FrameworkElement
        {
            if (_sourceItem is TAnchror)
            {
                return element;
            }
            else
            {
                return element.FindChild<TAnchror>(DefaultAnchorName);
            }

        }

        private void SetStartPoint(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _startPoint = mouseButtonEventArgs.GetPosition(AssociatedObject);
        }

        #region visual Reactions

        private void FollowMouseWithConnector(object sender, MouseEventArgs e)
        {
            if (MainCanvas == null) return;

            // translate the Start Point to Canvas
            if (_activeConnectorLine == null)
            {
                _activeConnectorLine = CreateNewConnector();

                // Get the Start Point
                var nodeContainer = GetObjectAtPoint<ListViewItem>(AssociatedObject, _startPoint);
                _activeConnectorLine.StartPoint = GetMiddlePointofRectangleOfNodeContainer(nodeContainer);
            }


            _activeConnectorLine.ConnectConnectorWithPoint(e.GetPosition(MainCanvas), GetHorizontalBezierOffset(this));
        }

        private Point GetMiddlePointofRectangleOfNodeContainer(DependencyObject nodeContainer)
        {
            var node = nodeContainer.FindChild<Rectangle>(DefaultAnchorName);
            return node.GetMiddlePoint(MainCanvas);
        }

        private PathFigure CreateNewConnector()
        {
            PathFigure connector = new PathFigure();
            //connector.StartPoint = _startPoint;

            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(connector);

            _activePath = new Path
            {
                Stroke = Brushes.Black,
                StrokeThickness = (double) ListConnectorView.DraggingStrokeThicknessProperty.GetMetadata(typeof(ListConnectorView)).DefaultValue,
                Data = pathGeometry,
                IsHitTestVisible = false
            };
            //_pathList.Add(_activePath);

            var c = MainCanvas;

            if (c == null)
            {
                Console.WriteLine("No Canvas added for Connection Behavior");
            }
            else
            {
                var childs = c.Children;
                childs.Add(_activePath);
            }

            return connector;
        }

        private void ResetConnector()
        {
            _activeConnectorLine = null;
            _targetItem = null;
            _sourceItem = null;
            _activePath = null;

            _isConnectionProcessStarted = false;
        }

        #endregion

        /// <summary>
        /// Gibt das erste Objekt des entsprechenden Typs unter dem Mauszeiger zurück.
        /// Vorraussetzung ist, dass die property IshitTestVisible = true ist.
        /// </summary>
        /// <typeparam name="TDepObj"></typeparam>
        /// <returns></returns>
        private TDepObj GetObjectAtPoint<TDepObj>(Visual referenceVisual, Point point) where TDepObj : DependencyObject
        {
            DependencyObject current = null; // = result.VisualHit;

            VisualTreeHelper.HitTest(
                referenceVisual,
                HitTestFilterInvisible,
                hitTestResult =>
                {
                    current = hitTestResult.VisualHit;
                    return HitTestResultBehavior.Stop;
                },
                new PointHitTestParameters(point));
            //VisualTreeHelper.HitTest(_parent, FilterCallback, ResultCallback, new PointHitTestParameters(Mouse.GetPosition(_parent)));

            while (current != null)
            {
                var atPoint = current as TDepObj;
                if (atPoint != null)
                {
                    return atPoint;
                }
                current = VisualTreeHelper.GetParent(current) as UIElement;
            }
            return null;
        }

        /// <summary>
        /// Workararound, because the property IshitTestVisible is not evaluated by VisualTreeHelper.HitTest
        /// Source: http://stackoverflow.com/questions/4813434/wpf-uielement-ishittestvisible-false-still-returning-hits
        /// </summary>
        /// <param name="potentialHitTestTarget"></param>
        /// <returns></returns>
        public static HitTestFilterBehavior HitTestFilterInvisible(DependencyObject potentialHitTestTarget)
        {
            bool isVisible = false;
            bool isHitTestVisible = false;

            var uiElement = potentialHitTestTarget as UIElement;
            if (uiElement != null)
            {
                isVisible = uiElement.IsVisible;
                if (isVisible)
                {
                    isHitTestVisible = uiElement.IsHitTestVisible;
                }
            }
            else
            {
                UIElement3D uiElement3D = potentialHitTestTarget as UIElement3D;
                if (uiElement3D != null)
                {
                    isVisible = uiElement3D.IsVisible;
                    if (isVisible)
                    {
                        isHitTestVisible = uiElement3D.IsHitTestVisible;
                    }
                }
            }

            if (isVisible)
            {
                return isHitTestVisible ? HitTestFilterBehavior.Continue : HitTestFilterBehavior.ContinueSkipSelf;
            }

            return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
        }

        private ContentControl GetMainControl()
        {
            //ContentControl result = AssociatedObject.FindLogicalAncestor<UserControl>();
            //if (result == null)
            //{
            //     result = AssociatedObject.FindLogicalAncestor<Window>();
            //}
            //return result;

            return AssociatedObject.GetMainControl();
        }
    }
}