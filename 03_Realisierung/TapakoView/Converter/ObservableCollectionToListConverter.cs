using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Documents;
using Akomi.InformationModel.Device;

namespace Tapako.View.Converter
{
    public class ObservableCollectionToListConverter: IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(List))
            {
                if (value.GetType() == typeof (ObservableCollection<IDevice>))
                {
                    return ((ObservableCollection<IDevice>) value).ToList();
                }
            }
            return null;
            //throw new ArgumentException("Wrong Parameter Type");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(ObservableCollection<IDevice>))
            {
                if (value.GetType() == typeof(List))
                {
                    return new ObservableCollection<IDevice>(((List<IDevice>)value).AsEnumerable());
                }
            }
            throw new ArgumentException("Wrong Parameter Type");
        }
    }
}
