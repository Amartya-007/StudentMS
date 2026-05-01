using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace StudentMS.UI.Converters
{
    /// <summary>
    /// Converts bool -> Visibility. True = Visible, False = Collapsed.
    /// Set Parameter="Inverse" to flip the logic.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = value is bool b && b;
            bool inverse = parameter is string s && s.Equals("Inverse", StringComparison.OrdinalIgnoreCase);
            return (flag ^ inverse) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool inverse = parameter is string s && s.Equals("Inverse", StringComparison.OrdinalIgnoreCase);
            bool visible = value is Visibility v && v == Visibility.Visible;
            return visible ^ inverse;
        }
    }
}
