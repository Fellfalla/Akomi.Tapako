using System;
using System.Reflection;
using ExtensionMethodsCollection;
using IAkomiDevice.fIDevice;
using IAkomiDevice.fIDevice.fDocumentation;
using IAkomiDevice.fIDevice.fHmiImage;
using IAkomiDevice.fIDevice.fIdentification;

namespace Tapako.Services
{
    /// <summary>
    /// Die Deviceklasse soll alle Geräte represäntieren können
    /// TapakoDevice dient als z.t. Adapter für DeviceBase (Adapter-Pattern)
    /// </summary>
    [Serializable()]
    public class TapakoDevice : DeviceBase
    {

        //public string DevicePath { get; set; }

        //public string Parent { get; set; }

        //public readonly List<IDevice> DriverDevices = new List<IDevice>();

        //public bool IsDriverAvailable = false;

        public TapakoDevice()
        {
            SetHmiImage("Resources/Unknown.png");
        }

        //public string[] Type
        //{
        //    get
        //    {
        //        if (Identification == null)
        //        {
        //            return new string[0];
        //        }
        //        else
        //        {
        //            return Identification.DeviceType;
        //        }
        //    }
        //    set
        //    {
        //        if (Identification == null)
        //        {
        //            Identification = new Identification();
        //        }
        //        Identification.DeviceType = value;
        //    }
        //}
        //[XmlIgnore]
        //public string IpAddress
        //{
        //    get
        //    {
        //        if (Identification == null)
        //        {
        //            return null;
        //        }
        //        return Identification.IpAddress;
        //    }
        //    set
        //    {
        //        if (Identification == null)
        //        {
        //            Identification = new Identification();
        //        }
        //        Identification.IpAddress = value;
        //    }
        //}

        [XmlIgnore]
        public string MacAddress
        {
            get
            {
                if (Identification == null)
                {
                    return null;
                }
                else
                {
                        return Identification.PhysicalAddress;
                }
            }
            set
            {
                if (Identification == null)
                {
                    Identification = new Identification();
                }
                if (value != null)
                {
                    Identification.PhysicalAddress = value;
                }
            }
        }

        public string Name
        {
            get
            {
                if (Identification == null)
                {
                    return string.Empty;
                }
                else
                {
                    return Identification.BrowseName;
                }
            }
            set
            {
                if (Identification == null)
                {
                    Identification = new Identification();
                }
                Identification.BrowseName = value;
            }
        }

        public string Manufacturer { get; set; }
        //public string DriverName { get; set; }

        public void SetHmiImage(string path)
        {
            //Image bmp = new Bitmap(image);
            //MemoryStream memoryStream = new MemoryStream();
            //bmp.Save(memoryStream, ImageFormat.Bmp);
            var resource = Assembly.GetExecutingAssembly().GetResourceStream(path);
            HmiImage = new HmiImage(new ExternalDataType(ImageFormat.Bmp.ToString(), resource.ReadFully()));
        }

    }
}