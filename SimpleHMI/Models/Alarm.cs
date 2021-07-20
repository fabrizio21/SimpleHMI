using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHMI.Models
{
    /// <summary>
    /// Give shape to a single alarm
    /// </summary>
    public class Alarm : BindableBase {

        #region Attributes
        private string _key;            // i.e. Alarm01
        private string _message;        // i.e. Emergency button armed
        private DateTime _startDate;    // i.e. 17-03-1982 21:14:00
        private bool _isWarning;        // 
        #endregion

        #region Constructors
        public Alarm() { }

        public Alarm(string message) {
            _message = message;
            _startDate = DateTime.Now;
            _isWarning = false;
        }

        public Alarm(string message, bool isWarning) {
            _message = message;
            _startDate = DateTime.Now;
            _isWarning = isWarning;
        }

        public Alarm(string key, string message, DateTime startDate) {
            _key = key;
            _message = message;
            _startDate = startDate;
        }
        #endregion

        #region Properties
        public string Key
        {
            get { return _key; }
            set { SetProperty(ref _key, value); }
        }

        public string Message {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public DateTime StartDate {
            get { return _startDate; }
            set { SetProperty(ref _startDate, value); }
        }

        public bool IsWarning {
            get { return _isWarning; }
            set { SetProperty(ref _isWarning, value); }
        }
        #endregion
    }
}
