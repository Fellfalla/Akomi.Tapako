using System;
using System.Collections.ObjectModel;
using System.Runtime.Versioning;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tapako.Design.Extensions;

namespace Tapako.Design.Tests
{
    [TestClass]
    public class ExtensionTests
    {
        [TestMethod]
        public void AttachNotifyCollectionChangedEventHandler()
        {
            var property = DependencyProperty.Register("Test", typeof(int), typeof(ExtensionTests), null);

            var newCollection = new ObservableCollection<int>();
            var args = new DependencyPropertyChangedEventArgs(property, null, newCollection);
            bool handlerHasBeenFired = false;


            args.AssignOnCollectionChangedHandler((sender, eventArgs) => { handlerHasBeenFired = true; });

            Assert.IsFalse(handlerHasBeenFired);
            newCollection.Add(1);
            Assert.IsTrue(handlerHasBeenFired);
        }
    }
}
