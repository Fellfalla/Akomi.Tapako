//using System.Net.Sockets;
//using System.Threading.Tasks;
//using AkomiServer;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Tapako.Model;

//namespace OpcUaServerTest
//{
//    [TestClass]
//    public class TapakoOpcUaServerTest
//    {
//        TapakoOpcUaServer _sut = TapakoOpcUaServer.Instance;

//        [TestInitialize]
//        public void Init()
//        {
//            _sut = TapakoOpcUaServer.Instance;
//        }

//        [TestCleanup]
//        public void Cleanup()
//        {
//            _sut.Dispose();
//        }

//        [TestMethod]
//        public void ServerShouldStart()   //Methode muss Task zurückgeben, damit sie async ausgeführt werden kann, den Rest übernimmt das Framework
//        {
//            _sut.StartOpcUaServer();
//            PrivateObject privateObject = new PrivateObject(_sut);
//            _sut.WaitForServerShutdown();
//            Assert.IsFalse(((Task)privateObject.GetProperty("ServerThread")).IsFaulted);
//            //_sut.WaitForServerShutdown();
//        }

//        [TestMethod]
//        public void StartOpcUaServerShallNotThrowException()
//        {

//            _sut.StartOpcUaServer();
                       
//            Assert.IsTrue(_sut.IsServerRunning);
//            TcpClient client = new TcpClient("localhost", 48030);

//        }

//        [TestMethod]
//        [Timeout(10000)]
//        public void ServerShouldRunWithTinyArray()
//        {
//            //IDevice testDevice = new Mlv418H500RtIo65B92136();
//            //TapakoServerStarter.StartAkomiServer(testDevice);
//            int[] littleIntArray = new int[2];
//            for (int i = 0; i < littleIntArray.Length; i++)
//            {
//                littleIntArray[i] = i;
//            }
//            _sut.StartOpcUaServer(littleIntArray);
//            _sut.WaitForServerStartup();

//            TcpClient client = new TcpClient("localhost",48030);

//        }


//        [TestMethod]
//        [Timeout(10000)]
//        public void ServerShouldRunWithBigArray()
//        {
//            //IDevice testDevice = new Mlv418H500RtIo65B92136();
//            //TapakoServerStarter.StartAkomiServer(testDevice);
//            int[] BigIntArray = new int[50];
//            for (int i = 0; i < BigIntArray.Length; i++)
//            {
//                BigIntArray[i] = i;
//            }
//            _sut.StartOpcUaServer(BigIntArray);
//            _sut.WaitForServerStartup();

//            TcpClient client = new TcpClient("localhost",48030);

//        }

//        [TestMethod]
//        public async Task OpcUaServerDllTest()
//        {
//            int[] intArray = new int[50];
//            for (int i = 0; i < intArray.Length; i++)
//            {
//                intArray[i] = i;
//            }
//            _sut.StartOpcUaServer(intArray);


//            var server = new OpcUaServer();
//            server.StartServer(intArray, "TestServer - Die heilige Tomacko", 0, 4, loadConfigurationFile: true);
//            server.WaitForServer();
//        }

//        [TestMethod]
//        public void OpcUaTestWithFilleTapakoDevice()
//        {
//            int[] intArray = new int[50];
//            for (int i = 0; i < intArray.Length; i++)
//            {
//                intArray[i] = i;
//            }
//            _sut.StartOpcUaServer(intArray);


//            var server = new OpcUaServer();
//            server.StartServer(intArray, "TestServer - Die heilige Tomacko", 0, 4, loadConfigurationFile: true);
//            server.WaitForServer();
//        }
//    }
//}
