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
    public class CtrlModeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //string sVal = (string)value;
            string sParam = (string)parameter;
            //int iVal; 
            int iParam;

            int.TryParse(sParam, out iParam);
            //int.TryParse(sParam, out iVal);

            return ((int)value == iParam)? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
