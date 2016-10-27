using System;
using System.Globalization;
using System.Windows.Data;

namespace Tapako.Design.Converter
{
    class Scaler : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float input;
            float castedValue;

            float.TryParse(parameter.ToString(), out input);
            float.TryParse(value.ToString(), out castedValue);


            return (int)(castedValue * input);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
