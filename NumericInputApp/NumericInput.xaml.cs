using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NumericInputApp
{
    /// <summary>
    /// Logica di interazione per NumericInput.xaml
    /// </summary>
    public partial class NumericInput : UserControl
    {
        #region Attributes
        public static string REGEX_FLOAT = @"[-+]?(?<![0-9][<DecimalSeparator>])[<DecimalSeparator>]?[0-9]+(?:[<DecimalSeparator>\s][0-9]+)*[<DecimalSeparator>]?[0-9]?(?:[eE][-+]?[0-9]+)?(?!\.[0-9])";
        private string _internalString;
        #endregion

        #region Properties
        public double? Minimum
        {
            get { return (double?)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public double? Maximum
        {
            get { return (double?)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public double? Value
        {
            get { return (double?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public string UnitOfMeasure
        {
            get { return (string)GetValue(UnitOfMeasureProperty); }
            set { SetValue(UnitOfMeasureProperty, value); }
        }

        public CultureInfo Culture
        {
            get { return (CultureInfo)GetValue(CultureProperty); }
            set { SetValue(CultureProperty, value); }
        }
        #endregion

        #region Wrapper Properties
        // Using a DependencyProperty as the backing store for Maximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double?), typeof(NumericInput), new PropertyMetadata(new PropertyChangedCallback(OnMaximumPropertyChanged)));

        // Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double?), typeof(NumericInput), new PropertyMetadata(new PropertyChangedCallback(OnMinimumPropertyChanged)));

        // Using a DependencyProperty as the backing store for UnitOfMeasure.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitOfMeasureProperty =
            DependencyProperty.Register("UnitOfMeasure", typeof(string), typeof(NumericInput), new PropertyMetadata(new PropertyChangedCallback(OnUnitOfMeasurePropertyChanged)));

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double?), typeof(NumericInput), new PropertyMetadata(new PropertyChangedCallback(OnValuePropertyChanged)));

        // Using a DependencyProperty as the backing store for Culture.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CultureProperty =
            DependencyProperty.Register("Culture", typeof(CultureInfo), typeof(NumericInput), new PropertyMetadata(new PropertyChangedCallback(OnCulturePropertyChanged)));

        private static void OnMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            NumericInput n = d as NumericInput;
            n?.OnValueChanged(n.Value, n.Value);
        }
        #endregion

        #region Property changed handlers
        private static void OnMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericInput n = d as NumericInput;
            n?.OnValueChanged(n.Value, n.Value);
        }
        private static void OnUnitOfMeasurePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericInput n = d as NumericInput;
            n?.OnValueChanged(n.Value, n.Value);
        }
        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // è cambiato il valore
            if (e.OldValue != e.NewValue)
            {
                (d as NumericInput)?.OnValueChanged((double?)e.OldValue, (double?)e.NewValue);
            }
        }
        private static void OnCulturePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericInput n = d as NumericInput;
            n?.OnValueChanged(n.Value, n.Value);
        }

        private void OnValueChanged(double? oldValue, double? newValue) {

            double val;
            if (Minimum != null && newValue != null)
                if (newValue < Minimum)
                    val = (double)Minimum;
            if (Maximum != null && newValue > Maximum)
                    val = (double)Maximum;
            Value = newValue;

            numText.Text = String.Format(Culture, "{0:F2} {1}", newValue, UnitOfMeasure);
        }
        #endregion

        #region Contructor
        public NumericInput() {
            InitializeComponent();
            Culture = System.Globalization.CultureInfo.CurrentCulture;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Checks if the inserted digits are ok
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numText_PreviewTextInput(object sender, TextCompositionEventArgs e) {

            string decPoint = Culture.NumberFormat.NumberDecimalSeparator;

            //[-+]?(?<![0-9][<dec>])[<dec>]?[0-9]+(?:[.\s][0-9]+)*[.]?[0-9]?(?:[eE][-+]?[0-9]+)?(?!\.[0-9])

            // controllo che sia un numero
            //Regex regex = new Regex("[+-]?([0-9]*[" + decPoint + "])?[0-9]+");
            //Regex regex = new Regex("[+-]?([0-9]*[.])?[0-9]+");
            //Regex regex = new Regex ("[-+]?[0 - 9]*\\.[0-9]");
            //e.Handled = regex.IsMatch(e.Text);
            _internalString = e.Text;
        }

        private void numText_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                double val;
                double.TryParse((sender as TextBox).Text, out val);
                SetValue(val);
            }
        }
        #endregion

        private void numText_LostFocus(object sender, RoutedEventArgs e) {
            double val;
            double.TryParse((sender as TextBox).Text, out val);
            SetValue(val);
        }

        private void SetValue(double? value) {
            double val;
            double.TryParse(numText.Text, out val);
            if (Minimum != null && value != null)
                if (value < Minimum)
                    val = (double)Minimum;
            if (Maximum != null && value != null)
                if(value > Maximum)
                    val = (double)Maximum;
            Value = val;
        }
    }
}

