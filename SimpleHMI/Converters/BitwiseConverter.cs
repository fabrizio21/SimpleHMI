using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SimpleHMI.Converters
{
    public class BitwiseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            int iValue = (int)value;
            int iBit;
            
            string sParam = parameter.ToString();
            int.TryParse(sParam, out iBit);
            int val = (1 << iBit);


            return ((iValue & val) == val);
            

            //return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
