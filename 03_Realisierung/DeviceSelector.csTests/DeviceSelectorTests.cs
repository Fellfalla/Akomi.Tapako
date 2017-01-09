using System;
using System.Collections.Generic;
using Akomi.InformationModel.Component.Identification;
using Akomi.InformationModel.Device;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tapako.Framework.ExtensionMethods;

namespace DeviceSelectorTests
{
    [TestClass()]
    public class DeviceSelectorTests
    {

        [TestMethod()]
        [Ignore]
        public void SelectDeviceTest()
        {
        
            var result = Tapako.Utilities.DeviceSelector.DeviceSelector.SelectDevice();
            Console.WriteLine("Results: " + result);
        }

        [TestMethod()]
        [Ignore]
        public void SelectDeviceWithCustomSuggestionsTest()
        {
            IDevice device = new DeviceBase();
            device.SetBrowseName("Test Gerät");
            var result = Tapako.Utilities.DeviceSelector.DeviceSelector.SelectDevice(new List<string>(){"tobi", "markus", "müll"},connectionParent: new DeviceBase());
            Console.WriteLine("Results: " + result);
        }

        [TestMethod()]
        [Ignore]
        public void SelectDeviceWithOverriddenSuggestions()
        {

            // Call just one static method
            var result = Tapako.Utilities.DeviceSelector.DeviceSelector.SelectDevice(new List<string>() { "a", "ab", "abcd", "Hello", "World", "abcdefghijklmnopqrstuvwxyz"}, true);

            Console.WriteLine("Model Number: {0}\nSerial Number:{1} ", result.Item1, result.Item2);
        }
    }
}