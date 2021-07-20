using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHMI.Models
{
    public class User : BindableBase {

        #region Attributes
        private string _name;
        private string _password;
        private int _level;
        private uint _accessMask;       // set of 32 bits that represent the privileged areas
        #endregion

        #region Properties
        public string Name {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public string Password {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        /// <summary>
        /// The higher the number the lower the privilege
        /// </summary>
        public int Level {
            get { return _level; }
            set { SetProperty(ref _level, value); }
        }

        /// <summary>
        /// Set of 32 bits of access areas
        /// </summary>
        public uint AccessMask {
            get { return _accessMask; }
            set { SetProperty(ref _accessMask, value); }
        }
        #endregion

        #region Methods
        public bool isAreaEnabled(int area) {
            return (_accessMask >> area == 1);

        }

        public void Reset() {
            Name = string.Empty;
            Password = string.Empty;
            Level = -1;
            AccessMask = 0;
        }
        #endregion
    }
}
