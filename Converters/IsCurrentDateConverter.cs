using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace CalendarHabitsApp.Converters
{
    public class IsCurrentDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((DateTime)value).ToString("MM/dd/yyyy") == DateTime.Now.ToString("MM/dd/yyyy");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
