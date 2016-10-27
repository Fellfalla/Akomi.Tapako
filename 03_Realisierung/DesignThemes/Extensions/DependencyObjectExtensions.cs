using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Tapako.Design.Extensions
{
    public static class DependencyObjectExtensions
    {

        public static IEnumerable<T> FindChildsByBoundObject<T>(this DependencyObject parent, object boundObject)
            where T: FrameworkElement
        {
            if (parent == null) yield break;

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                var o = child as T;
                if (o != null && o.DataContext == boundObject)
                {
                    yield return o;
                }

                foreach (var childOfChild in FindChildsByBoundObject<T>(child, boundObject))
                {
                    yield return childOfChild;
                }
            }
        }
    }
}
