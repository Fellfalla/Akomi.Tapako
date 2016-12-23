using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Akomi.InformationModel.Component.Connection;
using Akomi.InformationModel.Component.Presentation;
using Akomi.InformationModel.Datatypes;
using ExtensionMethodsCollection;
using Prism.Mvvm;
using Tapako.Utilities.WiringTool.View;

namespace Tapako.Utilities.WiringTool.ViewModel
{
    [DesignTimeVisible(true)]
    public class WiringToolDesignViewModel : WiringToolViewModel
    {
        public WiringToolDesignViewModel()
        {
            ParentConnections = new List<object>();
            for (var i = 0; i < 30; i++)
            {
                var con = new Connection { Name = "ParentTestConnection" + i };
                ParentConnections.Add(con);
            }

            ChildConnections = new List<object>();
            for (var i = 0; i < 3; i++)
            {
                var con = new Connection { Name = "ChildTestConnection" + i };
                ChildConnections.Add(con);
            }

            if (Wirings == null) Wirings = new ObservableCollection<Wiring>();
            if (Wirings.IsNullOrEmpty())
            {
                Wirings.Add(Wiring.Create(ParentConnections.FirstOrDefault(), ChildConnections.FirstOrDefault()));
                Wirings.Add(Wiring.Create(ParentConnections.ElementAt(1), ChildConnections.ElementAt(1)));
            }

        }
        public string TestString
        {
            get { return "DesignModelTest"; }
        }

        public new string ParentName
        {
            get { return "Parent Design Device"; }
        }

        public new string ChildName
        {
            get { return "Child Design Device"; }
        }

        public new IHmiImage ParentHmiImage
        {
            get { return new HmiImage(new ExternalDataType("bmp", GetRandomImageByteArray())); }
        }

        public new IHmiImage ChildHmiImage
        {
            get { return new HmiImage(new ExternalDataType("bmp", GetRandomImageByteArray())); }
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
