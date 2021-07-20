using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHMI.Models
{
    /// <summary>
    /// Generica classe per un tag
    /// </summary>
    public class Tag : BindableBase {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private Type _type;
        public Type Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }

        private string _value;
        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        public Tag(string name, Type type) {
            _name = name;
            _type = type;
        }

        //string _memArea;
        /*
        public T Value {
            get { return _value; }
            set { _value = value; }
        }
        */
    }
}
