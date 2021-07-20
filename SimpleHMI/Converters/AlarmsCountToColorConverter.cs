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
    public class AlarmsCountToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            int? iValue = (int?)value;

            // set to red if more than one alarm active, gray otherwise
            if (iValue > 0)
                return new SolidColorBrush(System.Windows.Media.Colors.Red);
            else
                return new SolidColorBrush(System.Windows.Media.Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
        }
    }
}
