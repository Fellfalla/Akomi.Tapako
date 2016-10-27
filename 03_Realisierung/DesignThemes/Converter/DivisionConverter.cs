using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Tapako.Design.Converter
{
    class DivisionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double parsedValue;
            double parsedDivisor;
            double? result = null;

            if (values != null && values.Count() >= 2 && double.TryParse(values.First().ToString(), out parsedValue) && double.TryParse(values[1].ToString(), out parsedDivisor))
            {
                result = (int) (parsedValue / parsedDivisor);
            }

            if (parameter != null)
            {
                int minus;
                int.TryParse(parameter.ToString(), out minus);
                result = result - minus;
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
