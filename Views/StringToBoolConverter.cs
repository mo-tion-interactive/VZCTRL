using System.Globalization;
using System.Windows.Data;

namespace Ventuz.Views
{
    public class StringToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;

            return value.ToString()?.ToLower() switch
            {
                "1" => true,
                "true" => true,
                _ => false
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return b ? "1" : "0";

            return "0";
        }
    }
}