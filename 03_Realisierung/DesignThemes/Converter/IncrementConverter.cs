using System;
using System.Globalization;
using System.Windows.Data;

namespace Tapako.Design.Converter
{
    /// <summary>
    /// Increments the value
    /// </summary>
    public class IncrementConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter">Value is increased with this value</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float increment;
            float inputValue;
            float result;

            float.TryParse(parameter.ToString(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out increment);

            if (float.TryParse(value.ToString(), out inputValue))
            {
                result = inputValue + increment;
            }
            else
            {
                return value;
            }

            return result;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
