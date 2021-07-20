using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SimpleHMI.Converters
{
    public class IntRefConstModeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val = (int)value;
            string retVal = string.Empty;

            switch (val)
            {
                case 0: retVal = "SWEEP_CONST_P"; break;
                case 1: retVal = "SWEEP_CONST_V"; break;
                case 2: retVal = "SWEEP_CONST_A"; break;
            }
            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
