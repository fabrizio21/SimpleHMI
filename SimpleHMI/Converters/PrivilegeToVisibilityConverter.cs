using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SimpleHMI.Converters
{
    /// <summary>
    /// Turns the control visible based on the privilege
    /// </summary>
    public class PrivilegeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int iVal, iParam;
            iVal = (int)value;
            int.TryParse((string)parameter, out iParam);

            return (iVal == iParam) ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
