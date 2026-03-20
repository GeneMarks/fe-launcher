using System.Globalization;
using System.Windows.Data;

namespace FELauncher.UI.Desktop.Views.Converters
{
    internal class ProcessPathConverter : IValueConverter
    {
        private const string EmptyPathFallback = "--";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var path = (string)value;
            return String.IsNullOrEmpty(path) ? EmptyPathFallback : path;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
