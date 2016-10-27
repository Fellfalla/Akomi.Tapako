using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace Tapako.View.Converter
{
    public class BoolToCursorConverter : IValueConverter
    {
        private readonly Cursor _trueCursor;
        private readonly Cursor _falseCursor;

        public BoolToCursorConverter()
        {
            _trueCursor = Cursors.AppStarting;
            _falseCursor = Cursors.Arrow;

        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                return (bool)value ? _trueCursor : _falseCursor;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cursor = value as Cursor;
            if (cursor != null)
                return cursor == _trueCursor;
            return false;
        }
    }
}
