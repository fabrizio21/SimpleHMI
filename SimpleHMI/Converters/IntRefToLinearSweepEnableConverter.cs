using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SimpleHMI.Converters
{
    class IntRefToLinearSweepEnableConverter : IMultiValueConverter
    {
        /// <summary>
        /// True se INT_REF_type_pre = 1  (LINEAR SWEEP) e INT_REF_syncState = NO_MOTION (0) e PAUSED (-1)
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool retValue = false;
            if (values != null)
            {
                int mode = (int)values[0];
                int state = (int)values[1];

                retValue = (mode == 1) && (state == 0 || state == -1);
            }

            return retValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
