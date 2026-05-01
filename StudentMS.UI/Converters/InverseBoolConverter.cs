using System.Globalization;
using System.Windows.Data;

namespace StudentMS.UI.Converters
{
    /// <summary>
    /// Inverts a boolean value. Useful for IsEnabled bindings opposite to IsBusy.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b ? !b : value;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b ? !b : value;
    }
}
