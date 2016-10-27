using System.Collections.Specialized;
using System.Windows;

namespace Tapako.Design.Extensions
{
    public static class EventArgsExtensions
    {
        /// <summary>
        /// Manages registering and unregistering Events from New and old Value of the changed Dependency Property
        /// for notifying list changes.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="handler"></param>
        public static void AssignOnCollectionChangedHandler(this DependencyPropertyChangedEventArgs args, NotifyCollectionChangedEventHandler handler)
        {
            var newConnection =
                args.NewValue as INotifyCollectionChanged;

            var oldConnection =
                args.OldValue as INotifyCollectionChanged;

            if (newConnection != null)
            {
                newConnection.CollectionChanged += handler;
            }
            if (oldConnection != null)
            {
                oldConnection.CollectionChanged -= handler;
            }
        }
    }
}
