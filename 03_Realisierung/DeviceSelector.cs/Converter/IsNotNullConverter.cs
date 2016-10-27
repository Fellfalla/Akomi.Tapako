using System;
using System.Globalization;
using System.Windows.Data;

namespace Tapako.Utilities.DeviceSelector.Converter
{
    public class IsNullConverter : IValueConverter
    {
        /// <summary>
        /// return true if the given string is null or empty 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return (string.IsNullOrEmpty(value.ToString()));
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("IsNullConverter can only be used OneWay.");
        }
    }
}
