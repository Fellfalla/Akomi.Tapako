using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tapako.Framework;

namespace DeviceInformationManagerTests
{
    [TestClass]
    public class DllLoaderTest
    {

        [TestInitialize]
        public void Init()
        {
            //DllLoader is a static class
        }

        [TestMethod]
        [Timeout(1000)]
        public void LoadDriverTakesNull()
        {
            DllLoader.Load<object>(null);
        }
    }
}
