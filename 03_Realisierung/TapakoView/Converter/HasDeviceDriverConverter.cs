using System;
using System.Globalization;
using System.Windows.Data;
using Tapako.PublicClasses;

namespace Tapako.View.Converter
{

    public class HasDeviceDriverConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">Klasse welche die Method enthält</param>
        /// <param name="targetType"></param>
        /// <param name="parameter">MethodName</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var tapakoDevice = value as TapakoDevice;
            if (tapakoDevice != null)
            {
                return tapakoDevice.IsDriverAvailable;
            }
            return false;
        
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
