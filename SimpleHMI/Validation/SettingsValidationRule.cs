using Prism.Mvvm;
using Prism.Unity;
using SimpleHMI.Models;
using SimpleHMI.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace SimpleHMI.Validation
{
    public class SettingsValidationRule : ValidationRule
    {
        public int Min { get; set; }    // devono essere dipendency property per poter essere bindable!
        public int Max { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {  
            return new ValidationResult(false, "errore!");
        }
    }
}