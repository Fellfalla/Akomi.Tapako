using System;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tapako.Utilities.WiringTool.View;
using Tapako.Utilities.WiringTool.ViewModel;

namespace WiringToolTests
{
    [TestClass()]
    public class WiringToolViewTests
    {
        [TestMethod()]
        public void WiringToolViewTest()
        {
            WiringToolView window = new WiringToolView();
            //{
            //    Title = "Wiring Tool",
            //    Content = new WiringToolView(),
            //    SizeToContent = SizeToContent.WidthAndHeight
            //};
            var dataContext = new WiringToolDesignViewModel();
            window.DataContext = dataContext;
            window.ShowDialog();

            foreach (var wiredConnection in dataContext.LogicalConnections)
            {
                Console.WriteLine("Connected {0} with {1}.", wiredConnection.Item1, wiredConnection.Item2);
            }
            
        }
    }
}