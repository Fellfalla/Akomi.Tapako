using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akomi.InformationModel.Component.Presentation;
using Akomi.InformationModel.Device;

namespace Tapako.Framework.ExtensionMethods
{
    [Obsolete("Please use extension methods in Akomi.Utilities.Etended")]
    public static class IDeviceExtensions
    {
        public static void SetBrowseName(this IDevice device, string name)
        {
            if (device.PresentationData == null)
            {
                device.PresentationData = new PresentationData();
            }

            device.PresentationData.BrowseName = name;
        }

        public static string GetBrowseName(this IDevice device)
        {
            if (device.PresentationData == null)
            {
                device.PresentationData = new PresentationData();
            }

            return device.PresentationData.BrowseName;
        }
    }
}
