using System;
using System.Globalization;
using System.Windows.Data;
using Akomi.InformationModel.Device;

namespace Tapako.Utilities.WiringTool.Converter
{
    /// <summary>
    /// Eine Converter Klasse, die ein IDevice in einen String umwandel
    /// todo: parameter für standardrückgabe übergeben
    /// todo: Diese Klasse kann gelöscht werden
    /// </summary>
    class DeviceToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(string))
            {
                var device = value as IDevice;
                if (device != null)
                {
                    return device.ToString();
                }
                else if (value is string)
                {
                    return value;
                }
                else if (parameter is string)
                {
                    return parameter;
                }
                else
                {
                    return "unknow Component";
                }
            }
            return null;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
