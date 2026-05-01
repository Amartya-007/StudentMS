using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace StudentMS.UI.Converters
{
    /// <summary>
    /// Converts a string to Visibility.
    /// Non-null, non-empty string → Visible.
    /// Null or empty string       → Collapsed.
    /// Set Parameter="Inverse" to flip.
    /// </summary>
    [ValueConversion(typeof(string), typeof(Visibility))]
    public class NullOrEmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool hasValue = value is string s && !string.IsNullOrEmpty(s);
            bool inverse  = parameter is string p && p.Equals("Inverse", StringComparison.OrdinalIgnoreCase);
            return (hasValue ^ inverse) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
