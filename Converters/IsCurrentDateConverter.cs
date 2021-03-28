using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CalendarHabitsApp.Converters
{
    public class IsCurrentDateConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == DependencyProperty.UnsetValue || values[0] == null ||
                   values[1] == DependencyProperty.UnsetValue || values[1] == null)
            {
                return false;
            }

            return ((DateTime)values[0]).ToString("MM/dd/yyyy") == ((DateTime)values[1]).ToString("MM/dd/yyyy");
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
