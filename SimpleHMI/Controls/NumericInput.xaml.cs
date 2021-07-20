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

namespace SimpleHMI.Controls
{
    /// <summary>
    /// Logica di interazione per NumericInput.xaml
    /// </summary>
    public partial class NumericInput : UserControl
    {
        #region Attributes
        public static string REGEX_FLOAT = @"[-+]?(?<![0-9][<DecimalSeparator>])[<DecimalSeparator>]?[0-9]+(?:[<DecimalSeparator>\s][0-9]+)*[<DecimalSeparator>]?[0-9]?(?:[eE][-+]?[0-9]+)?(?!\.[0-9])";
        #endregion

        #region Properties
        public bool IsReadyOnly
        {
            get { return (bool)GetValue(IsReadyOnlyProperty); }
            set { SetValue(IsReadyOnlyProperty, value); }
        }

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

        public string StringFormat
        {
            get { return (string)GetValue(StringFormatProperty); }
            set { SetValue(StringFormatProperty, value); }
        }

        // not used yet
        public double? DefaultValue
        {
            get { return (double?)GetValue(DefaultValueProperty); }
            set { SetValue(DefaultValueProperty, value); }
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

        // Using a DependencyProperty as the backing store for IsReadyOnly.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReadyOnlyProperty =
            DependencyProperty.Register("IsReadyOnly", typeof(bool), typeof(NumericInput), new PropertyMetadata(new PropertyChangedCallback(OnIsReadOnlyPropertyChanged)));

        // Using a DependencyProperty as the backing store for StringFormat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StringFormatProperty =
            DependencyProperty.Register("StringFormat", typeof(string), typeof(NumericInput), new PropertyMetadata(new PropertyChangedCallback(OnStringFormatPropertyChanged)));

        // Using a DependencyProperty as the backing store for DefaultValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefaultValueProperty =
            DependencyProperty.Register("DefaultValue", typeof(double?), typeof(NumericInput), new PropertyMetadata(new PropertyChangedCallback(OnDefaultValuePropertyChanged)));
        #endregion

        #region Property changed handlers
        private static void OnMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            NumericInput n = d as NumericInput;
            n?.OnValueChanged(n.Value, n.Value);
        }
        private static void OnMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            NumericInput n = d as NumericInput;
            n?.OnValueChanged(n.Value, n.Value);
        }
        private static void OnDefaultValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            //throw new NotImplementedException();
        }
        private static void OnUnitOfMeasurePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            NumericInput n = d as NumericInput;
            n?.OnValueChanged(n.Value, n.Value);
        }
        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            // è cambiato il valore
            if (e.OldValue != e.NewValue)
                (d as NumericInput)?.OnValueChanged((double?)e.OldValue, (double?)e.NewValue);
        }
        private static void OnCulturePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            NumericInput n = d as NumericInput;
            n?.OnValueChanged(n.Value, n.Value);
        }
        private static void OnIsReadOnlyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            NumericInput n = d as NumericInput;
            n.numText.IsReadOnly = (bool)e.NewValue;
        }

        private void OnValueChanged(double? oldValue, double? newValue) {
            string format;

            // scrive anche l'unità di misura se presente
            format = (UnitOfMeasure != null) ? "{0:" + StringFormat + "} {1}" : "{0:" + StringFormat + "}";
            numText.Text = String.Format(Culture, format, newValue, UnitOfMeasure);

            if (oldValue != newValue) {
                // fires the event to the outer world
                this.RaiseEvent(new RoutedPropertyChangedEventArgs<double?>(oldValue, newValue, ValueChangedEvent));
            }
        }

        /// <summary>
        /// The string format changes, refreshes the value 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnStringFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            NumericInput n = d as NumericInput;
            n?.OnValueChanged(n.Value, n.Value);
        }
        #endregion

        #region Contructor
        public NumericInput() {
            InitializeComponent();
            
            // default values
            StringFormat = "F2";
            Culture = System.Globalization.CultureInfo.CurrentCulture;
            
            // adds the paste event handler
            DataObject.AddPastingHandler(numText, OnPaste);
        }
        #endregion

        #region Events
        /// <summary>Identifies the <see cref="ValueChanged"/> routed event.</summary>
        public static readonly RoutedEvent ValueChangedEvent
            = EventManager.RegisterRoutedEvent(nameof(ValueChanged),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedPropertyChangedEventHandler<double?>),
                                               typeof(NumericInput));
        #endregion

        /// <summary>
        /// Add / Remove ValueChangedEvent handler
        /// Event which will be fired from this NumericUpDown when its value has been changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double?> ValueChanged {
            add => this.AddHandler(ValueChangedEvent, value);
            remove => this.RemoveHandler(ValueChangedEvent, value);
        }

        #region Methods

        /// <summary>
        /// The user has juste pasted some text inside the box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPaste(object sender, DataObjectPastingEventArgs e) {
            TextBox box = sender as TextBox;
            bool result;
            NumberStyles style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;
            double val;

            result = double.TryParse(box.Text, style, this.Culture, out val);
            if (result)
                SetValue(val, true);
            else {
                // ripristina a video il valore corrente
                OnValueChanged(this.Value, this.Value);
            }
        }

        /// <summary>
        /// Checks if the inserted digits are ok
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numText_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            string decPoint = Culture.NumberFormat.NumberDecimalSeparator;
            //bool ok = false;
            TextBox box = sender as TextBox;
            char digit = e.Text.ToCharArray()[0];
            bool isDigit = Char.IsDigit(digit);
            bool isDecSep = Char.Equals(digit, Culture.NumberFormat.NumberDecimalSeparator.ToCharArray()[0]);
            bool isSign = Char.Equals(digit, '-') || Char.Equals(digit, '+');

            if (isDigit || isDecSep || isSign) {
                if (isDecSep) {
                    // controllo che non sia già nella stringa e non sia in prima posizione
                    e.Handled = (box.Text == string.Empty || box.Text.Contains(e.Text));
                }
                if (isSign)
                    e.Handled = (box.Text == string.Empty);
            } else {
                e.Handled = true;
            }
        }

        private void numText_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                double val;
                val = Convert((sender as TextBox).Text);
                SetValue(val, true);
            }
        }
        #endregion

        private void numText_LostFocus(object sender, RoutedEventArgs e) {
            double val;
            val = Convert((sender as TextBox).Text);
            SetValue(val, true);
        }

        private double Convert(string textValue) {
            double val;
            bool result;
            string sValue = textValue.Trim();
            
            //double.TryParse((sender as TextBox).Text, out val);
            NumberStyles style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;
            result = double.TryParse(sValue, style, this.Culture, out val);

            return val;
        }

        private void SetValue(double? value, bool manualInput) {
            double val = (double)value;

            if (Minimum != null && value != null)
                if (value < Minimum)
                    val = (double)Minimum;
            if (Maximum != null && value != null)
                if (value > Maximum)
                    val = (double)Maximum;
            Value = val;
        }
    }
}