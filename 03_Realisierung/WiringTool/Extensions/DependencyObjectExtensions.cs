using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock.Controls;

namespace Tapako.Utilities.WiringTool.Extensions
{
    public static class DependencyObjectExtensions
    {

        /// <summary>
        /// Source: http://stackoverflow.com/questions/636383/how-can-i-find-wpf-controls-by-name-or-type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public static T FindChild<T>(this DependencyObject parent, string childName) where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        public static Point GetMiddlePoint(this FrameworkElement element, Visual relative)
        {
            if (element == null) return default(Point);

            var transform = element.TransformToVisual(relative);
            Point lineStartPoint = transform.Transform(new Point(element.ActualHeight / 2, element.ActualWidth / 2));
            return lineStartPoint;
        }

        public static ContentControl GetMainControl(this DependencyObject control)
        {
            ContentControl result = control.FindLogicalAncestor<UserControl>();
            if (result == null)
            {
                result = control.FindLogicalAncestor<Window>();
            }
            return result;
        }
    }
}
