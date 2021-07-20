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
    /// Ritorna vero se il valore in ingresso è uguale al parametro
    /// </summary>
    public class ValueToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val = (int)value;
            int param;
            int.TryParse((string)parameter, out param);

            return (val == param);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
