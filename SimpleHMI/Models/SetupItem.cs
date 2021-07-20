using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHMI.Models
{
    public class SetupItem : BindableBase
    {
        #region Attributes
        private string _name;
        private String _description;
        private String _varValue;               // value as string (coming from database or from user input)
        private Type _typeValue;                // datatype
        private Double? _minValue;
        private Double? _maxValue;
        private String _readValue;
        private String _unitMeasure;
        private bool? _isReadOnly;
        private bool? _isVisible;
        private bool? _writeToPlc;              // writes the value to the plc if changed or loaded
        private String _readCode;
        private string _format;                 // formato del numero
        private int? _category;
        private string _default;
        private string _criteria;
        #endregion

        #region Properties
        public String ReadValueString {
            get {
                string s;

                try {
                    switch (this.TypeValue.Name) {
                        case "Double":
                            if (_format != null)
                                //s = Double.Parse(this.ReadValue).ToString(_format);
                                // nel db sono salvati tutti con le convenzioni inglesi (.), qui converte al separatore decimale corretto
                                s = Double.Parse(this.Value, CultureInfo.DefaultThreadCurrentCulture.NumberFormat).ToString(_format, CultureInfo.DefaultThreadCurrentCulture);
                            else
                                s = Double.Parse(this.Value).ToString("F1");
                            break;
                        case "TimeSpan":
                            //s = TimeSpan.Parse(this.ReadValue).TotalMilliseconds.ToString();
                            s = TimeSpan.Parse(this.Value).TotalMilliseconds.ToString();
                            break;
                        default:
                            //s = this?.ReadValue?.ToString();
                            s = this?.Value?.ToString();
                            break;
                    }
                }
                catch { return "[ERROR]"; }
                return s;
            }
        }

        public string Name {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public String ReadCode {
            get { return this._readCode; }
            set { SetProperty(ref _readCode, value); }
        }

        public bool? IsReadOnly
        {
            get { return this._isReadOnly; }
            set { SetProperty(ref _isReadOnly, value); }
        }

        public bool? IsVisible
        {
            get { return this._isVisible; }
            set { SetProperty(ref _isVisible, value); }
        }

        public String UnitMeasure
        {
            get { return this._unitMeasure; }
            set { SetProperty(ref _unitMeasure, value); }
        }

        public String ReadValue {
            get { return this._readValue; }
            set { SetProperty(ref _readValue, value); }
        }

        public Type TypeValue {
            get { return this._typeValue; }
            set { SetProperty(ref _typeValue, value); }
        }

        public Double? MaxValue {
            get { return this._maxValue; }
            set { SetProperty(ref _maxValue, value); }
        }

        public Double? MinValue {
            get { return this._minValue; }
            set { SetProperty(ref _minValue, value); }
        }

        public String Value {
            get { return this._varValue; }
            set {
                if (this.TypeValue.Name == "Double") {
                    // sostituisco il punto con il sepratore decimale corretto (se sono inglese sostituisco il . con il .)
                    //value = value.Replace(".", CultureInfo.DefaultThreadCurrentCulture.NumberFormat.NumberDecimalSeparator);
                    value = value.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                }
                SetProperty(ref _varValue, value); 
            }
        }

        public String Description {
            get { return this._description; }
            set { SetProperty(ref _description, value); }
        }

        public bool? WriteToPlc {
            get { return _writeToPlc; }
            set {
                if (value == null)
                    value = false;
                SetProperty(ref _writeToPlc, value); 
            }
        }

        public int? Category {
            get { return _category; }
            set { SetProperty(ref _category, value); }
        }

        public string Default
        {
            get { return _default; }
            set { SetProperty(ref _default, value); }
        }

        public string Criteria
        {
            get { return _criteria; }
            set { SetProperty(ref _criteria, value); }
        }
        /// <summary>
        /// Number/date format
        /// </summary>
        public string Format {
            get { return _format; }
            set { SetProperty(ref _format, value); }
        }
        #endregion

        #region Functions
        public bool IsNumber => this._typeValue == typeof(Double) || this._typeValue == typeof(Int32) || this._typeValue == typeof(TimeSpan);

        public String ReadVar => String.IsNullOrEmpty(this._readCode) ? this._name : this._readCode;

        /// <summary>
        /// Check if the value is valid
        /// </summary>
        /// <param name="cultureInfo">I need this to avoid mistakes with decimal separators</param>
        /// <returns></returns>
        public bool IsValid(CultureInfo cultureInfo) {
            bool check = false;
            bool minCheck;
            bool maxCheck;

            switch (_typeValue.Name) {
                case "Int32":
                    int iVal;
                    if (int.TryParse(_varValue, out iVal)) {
                        if (_minValue != null)
                            minCheck = iVal >= _minValue;
                        else
                            minCheck = true;
                        if (_maxValue != null)
                            maxCheck = iVal <= _maxValue;
                        else
                            maxCheck = true;
                        check = minCheck & maxCheck;
                    }
                    break;
                case "String":
                    check = true;       // aggiungere regexp come criterio di controllo
                    break;
                case "Double":
                    double dVal;
                    if (double.TryParse(_varValue, out dVal))
                    {
                        if (_minValue != null)
                            minCheck = dVal >= _minValue;
                        else
                            minCheck = true;
                        if (_maxValue != null)
                            maxCheck = dVal <= _maxValue;
                        else
                            maxCheck = true;
                        check = minCheck & maxCheck;
                    }
                    break;
                case "Boolean":
                    check = true;       //[capire cosa arriva qui] validi solo 0/1 o True/False
                    break;
            }
            return check;
        }

        /// <summary>
        /// Force the value between min and max
        /// </summary>
        /// <returns></returns>
        public bool Coerce() {
            return true;
        }

        public void SetDefault() {
            Value = Default;
        }
        #endregion
    }
}
