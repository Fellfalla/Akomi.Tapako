using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Akomi.InformationModel.Component.Connection;
using Akomi.InformationModel.Component.Presentation;
using Akomi.InformationModel.Device;
using ExtensionMethodsCollection;
using Tapako.Utilities.WiringTool.Extensions;
using Tapako.Utilities.WiringTool.View;
using Tapako.Utilities.WiringTool.ViewModel;

namespace Tapako.Utilities.WiringTool
{
    public class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //AppDomain.CurrentDomain.AppendPrivatePath(@"bin\DLLs");

            base.OnStartup(e);
        }
    }

    /// <summary>
    /// Tool for Connecting the Connections of a primitive Device to a primitive Device CCD
    /// todo: Bildgrößen anpassten (bei 1 Connection sehr kleines bild nur, auch bei 4 z.b. bei der Steckkarte)
    /// </summary>
    public class WiringTool
    {
        /// <summary>
        /// Wires the Connections of these 2 Connection Lists
        /// Item 1 will be Element from first List, 
        /// Item 2 willl be Element from second List
        /// !!! Attetion: Lists can be wired to Items from same List !!!
        /// </summary>
        public static List<Tuple<T, T>> Connect<T>(ICollection<T> parentConnections, ICollection<T> childConnections,
            IHmiImage parentHmiImage = null, IHmiImage childHmiImage = null, string parentName = "Parent", string childName = "Child")
        {
            var showWiringWindow = new Func<List<Tuple<T, T>>>(() => RunConnectionWiring<T>(parentConnections, childConnections, parentHmiImage, childHmiImage, parentName, childName));

            // Start Selector Window in the UI Thread
            if (Application.Current != null)
            {
                return Application.Current.Dispatcher.Invoke(showWiringWindow);
            }
            else
            {
                return showWiringWindow.Invoke();
            }

        }

        private static List<Tuple<T, T>> RunConnectionWiring<T>(ICollection<T> parentConnections, ICollection<T> childConnections,
            IHmiImage parentHmiImage, IHmiImage childHmiImage, string parentName, string childName)
        {
            var viewModel = new WiringToolViewModel(parentConnections, childConnections, parentName: parentName, childName: childName)
            {
                ChildHmiImage = childHmiImage,
                ParentHmiImage = parentHmiImage
            };

            var mainWindow = new WiringToolView(viewModel);

            //mainWindow.Topmost = true; // bring view on top
            //mainWindow.Topmost = false; // make it possible to hide window again
            mainWindow.Activate();
            var returnValue = mainWindow.ShowDialog();//toDo: if return value is not true, ignore wiring AND don't save device

            return OrderTuples<T>(parentConnections, viewModel.LogicalConnections.Select(tuple => tuple.Cast<T, T>())).ToList();
        }

        /// <summary>
        /// Wires the Connections of these 2 Connection Lists
        /// </summary>
        public static List<Tuple<Connection, Connection>> Connect(IDevice parentDevice, IDevice childDevice)
        {
            var result = Connect(parentDevice.Connections, childDevice.Connections, parentDevice.PresentationData.HmiImage,
                childDevice.PresentationData.HmiImage, parentDevice.ToString(), childDevice.ToString());

            return OrderTuples(parentDevice.Connections, result).ToList();
        }

        private static IEnumerable<Tuple<T, T>> OrderTuples<T>(ICollection<T> firstTupleItemsCollection,
            IEnumerable<Tuple<T, T>> tuplesToOrder)
        {
            foreach (var tuple in tuplesToOrder)
            {
                if (firstTupleItemsCollection.Contains((T) tuple.Item1))
                {
                    yield return tuple;
                }
                else // wrong order
                {
                    yield return Tuple.Create(tuple.Item2, tuple.Item1);
                }
            }
        }

        public static DeviceBase WireConnections(DeviceBase device, IDevice parent)
        {
            if (parent == null) return null;

            //if (parent.Connections == null) parent.WiringObjects = new ConnectionList();
            if (parent.PresentationData == null)
            {
                parent.PresentationData = new PresentationData();
            }

            if (parent.PresentationData.HmiImage == null) parent.PresentationData.HmiImage = new HmiImage();


            var result = Connect(parent, device);

            foreach (var tuple in result)
            {
                Connection deviceConnection = tuple.Item2;
                Connection ccdConnection = tuple.Item1;

                if (tuple.HasANullObject()) continue;

                if (ccdConnection.Communication == null || ccdConnection.Communication.ConnectionPoints == null) continue;

                if (deviceConnection.Communication == null) deviceConnection.Communication = new Communication();

                if (deviceConnection.Communication.ConnectionPoints == null)
                    deviceConnection.Communication.ConnectionPoints = new string[0];

                deviceConnection.Communication.ConnectionPoints =
                    deviceConnection.Communication.ConnectionPoints.Concat(ccdConnection.Communication.ConnectionPoints)
                        .ToArray();
            }
            return device;
        }


    }
}