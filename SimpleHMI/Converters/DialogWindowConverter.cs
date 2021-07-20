using MaterialDesignThemes.Wpf;
using SimpleHMI.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SimpleHMI.Converters
{
    public class EnumDialogWindowModeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            EnumDialogWindowMode edw = (EnumDialogWindowMode)value;
            Brush brush = null;

            string sValue = string.Empty;
            switch (edw)
            {
                case EnumDialogWindowMode.None: brush = new SolidColorBrush(Colors.Transparent); break;
                case EnumDialogWindowMode.Question: brush = new SolidColorBrush(Colors.Orange); break;
                case EnumDialogWindowMode.Warning: brush = new SolidColorBrush(Colors.Red); break;
                case EnumDialogWindowMode.Success: brush = new SolidColorBrush(Colors.ForestGreen); break;
                case EnumDialogWindowMode.Info: brush = new SolidColorBrush(Colors.LightSkyBlue); break;
            }
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// For DialogWindow: converts from EnumDialogWindowMode to Material Design Pack Icon
    /// </summary>
    public class EnumDialogWindowModeToPackIconKindConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            EnumDialogWindowMode edw = (EnumDialogWindowMode)value;
            PackIconKind kind = PackIconKind.None;

            string sValue = string.Empty;
            switch (edw)
            {
                case EnumDialogWindowMode.None: kind = PackIconKind.None; break;
                case EnumDialogWindowMode.Question: kind = PackIconKind.QuestionMarkCircle; break;
                case EnumDialogWindowMode.Warning: kind = PackIconKind.Warning; break;
                case EnumDialogWindowMode.Success: kind = PackIconKind.CheckboxMarkedCircle; break;
                case EnumDialogWindowMode.Info: kind = PackIconKind.InfoCircle; break;
            }
            return kind;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EnumDialogWindowModeToVisibilityConverter : IValueConverter

    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            EnumDialogWindowMode edw = (EnumDialogWindowMode)value;
            return (edw == EnumDialogWindowMode.None) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Shows a button only if the caption is not empty
    /// </summary>
    public class ButtonTextToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string sValue = (string)value;
            //int iParam = (int)parameter;
            //return ((iValue & iParam) == iParam) ? Visibility.Visible : Visibility.Hidden;
            return (sValue == String.Empty) ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
