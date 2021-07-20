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
    public class ModeStateToStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {

            string retValue = string.Empty;
            int index;
            Dictionary<string, TranslationItem> dic;

            try { 
                // values[0] è il dizionario
                // values[1] è il valore numerico
                if (values != null) {
                    if(values.Length==2) { 
                        dic = (Dictionary<string, TranslationItem>)values[0];
                        index = (int)values[1];

                        switch(index) {
                            case 0: retValue = dic["PreOp"].Value.ToUpper();    break;
                            case 1: retValue = dic["Ready"].Value.ToUpper();    break;
                            case 2: retValue = dic["Running"].Value.ToUpper();  break;
                            case 3: retValue = dic["Pause"].Value.ToUpper();    break;
                            case 4: retValue = dic["Stop"].Value.ToUpper();     break;
                            default: retValue = "<ERR>";                        break;
                        }
                    }
                }
            } catch (Exception ex){
                //[TODO] messa perché in sede di design dà errore "Riferimento a un oggetto non impostato su un'istanza di oggetto"
                retValue = string.Empty;

            }

            return retValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            //throw new NotImplementedException();
            return null;
        }
    }
}
