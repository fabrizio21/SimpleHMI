using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SimpleHMI.Converters
{
    class InverseNumberConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type typ;
            // check if it's nullable
            typ = Nullable.GetUnderlyingType(targetType);
            if (typ == null)
                typ = targetType;
            
            switch (typ.Name) {
                case "Int32":
                    return (int)value * -1;
                case "Double":
                    return (double)value * -1;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
