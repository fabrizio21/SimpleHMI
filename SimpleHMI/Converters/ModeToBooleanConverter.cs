using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SimpleHMI.Converters
{
    public class ModeToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // CtrlState: 0 disable, 1 enable, 2 error, -2 reset error
            // CtrlMode: 1 jog axis, 2 move axis, 3 int ref, 4 ext ref
            string sParam = (string)parameter;
            int iParam;

            int.TryParse(sParam, out iParam);

            return ((int)value == iParam);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
