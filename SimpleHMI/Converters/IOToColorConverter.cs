using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SimpleHMI.Converters
{
    class IOToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = (string)value;
            Brush brush = null;
            
            if(value != null) {
                if (val == "1")
                    brush = new RadialGradientBrush(Colors.LimeGreen, Colors.ForestGreen);
                else
                    brush = new SolidColorBrush(Colors.LightGray);
            }
            return brush;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}