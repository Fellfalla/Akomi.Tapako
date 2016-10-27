using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using Akomi.InformationModel.Component.Presentation;
using Akomi.InformationModel.Datatypes;
using Tapako.Design.Extensions;

namespace Tapako.Design.Converter
{
    public class HmiImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Image)
            {
                var converter = new ImageToImageSourceConverter();
                return converter.Convert(value, targetType, parameter, culture);
            }
            else if (value is byte[])
            {
                var converter = new ByteArrayToImageSourceConverter();
                return converter.Convert(value, targetType, parameter, culture);
            }
            else if (value is ExternalDataType)
            {
                var converter = new ExternalDataTypeToImageSourceConverter();
                return converter.Convert(value, targetType, parameter, culture);
            }
            else if (value is HmiImage)
            {
                var converter = new HmiImageToImageSourceConverter();
                return converter.Convert(value, targetType, parameter, culture);
            }

            else
            {
                Debug.WriteLine("Cannot convert \"{0}\" to ImageSource.");
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts imaga data from  <see cref="T:byte[]" /> format to an <see cref="ImageSource"/>
    /// </summary>
    public class ByteArrayToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte[] byteArray = value as byte[];
            if (byteArray != null)
            {
                // Create Image
                MemoryStream mStream = new MemoryStream(byteArray);
                Image image = Image.FromStream(mStream);
                return image.ToBitmapSource();
            }
            else
            {
                Debug.WriteLine("Cannot convert \"{0}\" to ImageSource.");
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts imaga data from <see cref="ExternalDataType"/> format to an <see cref="ImageSource"/>
    /// </summary>
    public class ExternalDataTypeToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ExternalDataType externalDataType = (ExternalDataType) value;
            if (externalDataType.Data != null && externalDataType.Data.Any())
            {
               
                // Create Image
                MemoryStream mStream = new MemoryStream(externalDataType.Data);
                try
                {
                    Image image = Image.FromStream(mStream);
                    return image.ToBitmapSource();
                }
                catch (ArgumentException)
                {
                    Debug.WriteLine("No valid image data in \"{0}\"");
                    return null;
                }

            }
            else
            {
                Debug.WriteLine("Cannot convert \"{0}\" to ImageSource.");
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts imaga data from <see cref="Image"/> format to an <see cref="ImageSource"/>
    /// </summary>
    public class ImageToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Image image = value as Image;
            if (image != null)
            {
                return image.ToBitmapSource();
            }
            else
            {
                Debug.WriteLine("Cannot convert \"{0}\" to ImageSource.");
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts imaga data from <see cref="HmiImage"/> format to an <see cref="ImageSource"/>
    /// </summary>
    public class HmiImageToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            HmiImage hmiImage = value as HmiImage;
            if (hmiImage != null)
            {
                var converter = new ExternalDataTypeToImageSourceConverter();
                return converter.Convert(hmiImage.Data, targetType, parameter, culture);
            }
            else
            {
                Debug.WriteLine("Cannot convert \"{0}\" to ImageSource.");
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
