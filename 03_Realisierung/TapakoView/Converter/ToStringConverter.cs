using System;
using System.Globalization;
using System.Windows.Data;
using ExtensionMethodsCollection;

namespace Tapako.View.Converter
{
    class ToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value.GetType().IsEnum)
                {
                    return Enum.GetName(value.GetType(), value);
                }
                var values = value as Array;
                if (values != null)
                {
                    return ArraytoString(values);
                }

                return value.ToString();
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private string ArraytoString(Array array)
        {
            if (array == null || !array.Any())
            {
                return string.Empty;
            }

            string result = string.Empty;
            foreach (var item in array)
            {
                result += item +", ";
            }
            return result.Substring(0, result.Length - 2); // return without ", "
        }
        
    }
}
