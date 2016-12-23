using System;
using Akomi.InformationModel.Component.Connection;
using Akomi.InformationModel.Component.Presentation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WiringToolTests
{
    [TestClass()]
    [DeploymentItem(@"Images/parent.png")]
    [DeploymentItem(@"Images/child.png")]
    [Ignore]
    public class WiringToolTests
    {
        [TestMethod()]
        [Timeout(1000)]
        public void ConnectionTest()
        {
            var parentList = new ConnectionList();
            var childList = new ConnectionList();
            for (int i = 0; i < 7; i++)
            {
                var parentCon = new Connection {Name = "ParentConnection" + i};

                var childCon = new Connection {Name = "ChildConnection" + i};

                parentList.Add(parentCon);
                childList.Add(childCon);
            }

            var result = Tapako.Utilities.WiringTool.WiringTool.Connect(parentList, childList);
            Console.WriteLine("Results: ");
            result.ForEach(r => Console.WriteLine(r.Item1 + " connected to " + r.Item2));
        }

        [TestMethod()]
        public void ConnectionTestWithImage()
        {
            var parentList = CreateConnections(10, "ParentConnection");
            var childList = CreateConnections(5, "ChildConnection");

            // todo: Teste falsches Bildformat
            // todo: Teste falschen Bildpfad
            HmiImage parentHmiImage = new HmiImage("png", "parent.png");
            HmiImage childHmiImage = new HmiImage("png", "child.png");

            var result = Tapako.Utilities.WiringTool.WiringTool.Connect(parentList, childList, parentHmiImage, childHmiImage);
            Console.WriteLine("Results: ");
            result.ForEach(r => Console.WriteLine(r.Item1 + " connected to " + r.Item2));
        }

        [TestMethod()]
        [Ignore]
        public void ConnectTestWithLotsOfPins()
        {
            var parentList = new ConnectionList();
            var childList = new ConnectionList();
            for (int i = 0; i < 70; i++)
            {
                var parentCon = new Connection {Name = "ParentConnection" + i};

                var childCon = new Connection {Name = "ChildConnection" + i};

                parentList.Add(parentCon);
                childList.Add(childCon);
            }

            var result = Tapako.Utilities.WiringTool.WiringTool.Connect(parentList, childList);
            Console.WriteLine("Results: ");
            result.ForEach(r => Console.WriteLine(r.Item1 + " connected to " + r.Item2));
        }

        [TestMethod()]
        [Ignore]
        public void SimpleTest()
        {
            var parentList = new ConnectionList();
            var childList = new ConnectionList();

            var parentCon = new Connection {Name = "Tobi ist "};

            var childCon = new Connection {Name = "dumm "};

            parentList.Add(parentCon);
            childList.Add(childCon);
            var result = Tapako.Utilities.WiringTool.WiringTool.Connect(parentList, childList);
            Console.WriteLine("Results: ");
            result.ForEach(r => Console.WriteLine(r.Item1.Name + r.Item2.Name));
        }

        private ConnectionList CreateConnections(int count, string name)
        {
            var connectionList = new ConnectionList();
            for (int i = 0; i < count; i++)
            {
                var connection = new Connection { Name = name + i };
                connectionList.Add(connection);
            }
            return connectionList;
        } 
    }
}