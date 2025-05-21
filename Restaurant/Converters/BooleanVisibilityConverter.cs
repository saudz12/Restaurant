using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Restaurant.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool boolValue)
                return Visibility.Collapsed;

            bool visible = boolValue;

            if (parameter is string paramString && paramString.ToLower() == "inverse")
            {
                visible = !visible;
            }

            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Visibility visibility)
                return false;

            bool result = visibility == Visibility.Visible;

            if (parameter is string paramString && paramString.ToLower() == "inverse")
            {
                result = !result;
            }

            return result;
        }
    }
}
