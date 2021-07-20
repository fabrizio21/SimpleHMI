using SimpleHMI.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace SimpleHMI.Converters
{
    public class ConnectionStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ConnectionStatus? Value = (ConnectionStatus?)value;
            Brush brush = null;

            switch (Value) {
                case ConnectionStatus.Idle:
                    brush = new SolidColorBrush(System.Windows.Media.Colors.LightGray);
                    break;
                case ConnectionStatus.Connecting:
                    brush = new SolidColorBrush(System.Windows.Media.Colors.Yellow);
                    break;
                case ConnectionStatus.Online:
                    brush = new SolidColorBrush(System.Windows.Media.Colors.ForestGreen);
                    break;
                case ConnectionStatus.Offline:
                    brush = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    break;
            }

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
