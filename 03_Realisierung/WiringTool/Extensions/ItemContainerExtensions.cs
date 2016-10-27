using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Tapako.Utilities.WiringTool.Extensions
{
    /// <summary>
    /// Source: http://stackoverflow.com/questions/3788337/how-to-get-item-under-cursor-in-wpf-listview
    /// </summary>
    public static class ItemContainerExtensions
    {
        public static object GetObjectAtPoint<TItemContainer>(this ItemsControl control, Point p)
                                     where TItemContainer : DependencyObject
        {
            // ItemContainer - can be ListViewItem, or TreeViewItem and so on(depends on control)
            TItemContainer obj = GetContainerAtPoint<TItemContainer>(control, p);
            if (obj == null)
                return null;

            return control.ItemContainerGenerator.ItemFromContainer(obj);
        }

        public static TItemContainer GetContainerAtPoint<TItemContainer>(this ItemsControl control, Point p)
                                 where TItemContainer : DependencyObject
        {
            HitTestResult result = VisualTreeHelper.HitTest(control, p);
            if (result == null)
            {
                return null;
            }
            DependencyObject obj = result.VisualHit;

            while (VisualTreeHelper.GetParent(obj) != null && !(obj is TItemContainer))
            {
                obj = VisualTreeHelper.GetParent(obj);
            }

            // Will return null if not found
            return obj as TItemContainer;
        }

        public static object GetObjectAtPoint(this ItemsControl control, Point p)
        {
            // ItemContainer - can be ListViewItem, or TreeViewItem and so on(depends on control)
            ContentPresenter obj = GetContainerAtPoint<ContentPresenter>(control, p);
            if (obj == null)
                return null;

            return control.ItemContainerGenerator.ItemFromContainer(obj);
        }

    }
}
