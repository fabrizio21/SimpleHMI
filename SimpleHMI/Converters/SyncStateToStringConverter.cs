using SimpleHMI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SimpleHMI.Converters
{
    public class SyncStateToStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string retValue = string.Empty;
            Dictionary<string, TranslationItem> dic;
            int index;

            if (values != null) { 
                // values[0] è il dizionario
                // values[1] è il valore numerico
                dic = (Dictionary<string, TranslationItem>)values[0];
                index = (int)values[1];

                switch (index) {
                    case -2: retValue = dic["RampDown"].Value?.ToUpper(); break;
                    case -1: retValue = dic["Pause"].Value?.ToUpper(); break;
                    case 0: retValue = dic["NoMotion"].Value?.ToUpper(); break;
                    case 1: retValue = dic["Sync"].Value?.ToUpper();     break;
                    case 2: retValue = dic["RampUp"].Value?.ToUpper();   break;
                    
                    default: retValue = string.Empty;                   break;
                }
            }
            return retValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
