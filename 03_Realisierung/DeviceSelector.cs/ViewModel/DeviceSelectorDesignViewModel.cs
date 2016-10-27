using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Akomi.InformationModel.Component.Presentation;
using Akomi.InformationModel.Datatypes;
using Akomi.InformationModel.Device;
using RandomExtension;

namespace Tapako.Utilities.DeviceSelector.ViewModel
{
    class DeviceSelectorDesignViewModel : IDeviceSelectorViewModel
    {
        public string ModelNumber
        {
            get { return "It's a very long ding ding dong"; } 
            set {}
        }

        public string SerialNumber
        {
            get { return "SDGADR32462345z45zZTRHERTH"; }
            set { }
        }


        public IEnumerable GetDeviceModelNames(Type type)
        {
            return DeviceModels;
        }

        public IEnumerable<string> ModelSuggestions { get; set; }
        public Dictionary<string, List<string>> SerialNumberSuggestions { get; set; }
        public bool OverrideModelSuggestions { get; set; }

        public IDevice ParentDevice
        {
            get
            {
              Random random = new Random();
               var device = random.GetRandom<DeviceBase>();

                device.PresentationData.HmiImage = new HmiImage(new ExternalDataType("bmp", GetRandomImageByteArray()));
                return device;
            } 
            set { }
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }


        public IEnumerable<string> DeviceModels
        {
            get
            {
                return new List<string>()
                {
                    "test1",
                    "test2"
                };
            }
            
        }

        private byte[] GetRandomImageByteArray()
        {
            int ch = 3; //number of channels (ie. assuming 24 bit RGB in this case)
            Random rnd = new Random();

            int width = 500;
            int height = 500;

            int imageByteSize = width * height * ch;

            byte[] imageData = new byte[imageByteSize]; //your image data buffer
            rnd.NextBytes(imageData);       //Fill with random bytes;

            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            BitmapData bmData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            IntPtr pNative = bmData.Scan0;
            Marshal.Copy(imageData, 0, pNative, imageByteSize);
            bitmap.UnlockBits(bmData);

            MemoryStream memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, ImageFormat.Bmp);

            return memoryStream.GetBuffer();
        }
    }
}
