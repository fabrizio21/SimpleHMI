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
    public class ModeToButtonColorConverter : IValueConverter
    {
        private Brush COLOR_ON = new SolidColorBrush(Color.FromRgb(71,156,178));
        private Brush COLOR_OFF = new SolidColorBrush(Color.FromRgb(247, 247, 247));    // colore sfondo pulsanti

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            // 0 disable, 1 enable, 2 error, -2 reset error
            string sParam = (string)parameter;
            int iParam;

            int.TryParse(sParam, out iParam);

            return ((int)value == iParam) ? COLOR_ON: COLOR_OFF;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
