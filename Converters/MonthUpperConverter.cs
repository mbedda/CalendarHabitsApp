using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace CalendarHabitsApp.Converters
{
    /// <summary>
    /// Converts List Count to Control.Visibility values
    /// </summary>
    public class MonthUpperConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((DateTime)value).ToString("MMMM").ToUpper();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
