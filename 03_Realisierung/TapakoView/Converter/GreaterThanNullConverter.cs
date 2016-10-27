using System;
using System.Globalization;
using System.Windows.Data;

namespace Tapako.View.Converter
{
    class GreaterThanNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int castedValue;
            if (Int32.TryParse(value.ToString(), out castedValue))
            {
                return castedValue > 0;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
