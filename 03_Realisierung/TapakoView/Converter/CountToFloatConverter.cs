using System;
using System.Globalization;
using System.Windows.Data;

namespace Tapako.View.Converter
{
    public class CountToFloatConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter">split parameters with "|". 
        /// First parameter: count to min float.
        /// Second parameter: min float.
        /// Third parameter: max float.
        /// </param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float minValue = 0.1f;
            float maxValue = 0.5f;
            float steps = 5f;
            float inputValue;
            float result;
            string parameterString = parameter as string;
            string[] parameters;

            if (!string.IsNullOrEmpty(parameterString))
            {
                parameters = parameterString.Split(new char[] { '|' });
                // Now do something with the parameters
            }
            else
            {
                parameters = new string [] { "5", "0.1", "0.5" };
            }

            float.TryParse(parameters[0], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out steps);
            float.TryParse(parameters[1], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out minValue);
            float.TryParse(parameters[2], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out maxValue);

            if (value == null)
            {
                return maxValue;
            }


            if (float.TryParse(value.ToString(), out inputValue))
            {
                float difference = maxValue - minValue;
                result = maxValue - difference * (inputValue / steps);
            }
            else
            {
                result = minValue;
            }

            if (result > maxValue)
            {
                result = maxValue;
            }
            else if (result < minValue)
            {
                result = minValue;
            }

            return result;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
