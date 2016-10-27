using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using Akomi.InformationModel.Enums;

namespace Tapako.View.Converter
{
    class StaticDictionaryConverter:IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Dictionary<string, ProgressState> data = (Dictionary<string, ProgressState>)value;

            if (parameter != null)
            {
                string key = (string)parameter;
                return data[key];
            }
            return data;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
