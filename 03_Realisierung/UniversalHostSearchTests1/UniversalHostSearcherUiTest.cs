using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tapako.Utilities.UniversalHostSearch;

namespace UniversalHostSearchTests1
{
    [TestClass]
    public class UniversalHostSearcherUiTest
    {
        [TestMethod]
        [Ignore]
        public void SmokeTest()
        {
            var window = new Window();
            var viewModel = new UniversalHostSearchViewModel();
            viewModel.Subnet = "192.168.1";
            window.Content = new UniversalHostSearchView(viewModel);
            window.SizeToContent = SizeToContent.WidthAndHeight;

            window.ShowDialog();

            //viewModel.NewNetworkDeviceFound.ForEach(Console.WriteLine);
        }
    }
}
