using MaterialDesignThemes.Wpf;
using System.Globalization;
using System.Windows.Data;

namespace StudentMS.UI.Converters
{
    /// <summary>
    /// Converts bool ShowPassword → Eye / EyeOff PackIconKind for the password toggle button.
    /// Usage: Converter={x:Static converters:BoolToPackIconKindConverter.Instance}
    /// </summary>
    public class BoolToPackIconKindConverter : IValueConverter
    {
        public static readonly BoolToPackIconKindConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b && b ? PackIconKind.EyeOff : PackIconKind.Eye;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
