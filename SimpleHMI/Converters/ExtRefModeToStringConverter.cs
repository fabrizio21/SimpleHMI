using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SimpleHMI.Converters
{
    /// <summary>
    /// Int ref selection mode: from 0 -> DIGITAL, 1 -> ANALOG, 2 -> MEMORY 
    /// </summary>
    public class ExtRefModeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val = (int)value;
            string retVal = string.Empty;

            switch (val)
            {
                case 0: retVal = "DIGITAL"; break;
                case 1: retVal = "ANALOG"; break;
                case 2: retVal = "MEMORY"; break;
            }
            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
