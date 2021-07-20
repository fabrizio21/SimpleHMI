using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleHMI.Models;

namespace SimpleHMI.Services
{
    /// <summary>
    /// List of system tags in fomr of a dictionary
    /// </summary>
    public class TagService : Dictionary<string,Tag>, INotifyCollectionChanged, INotifyPropertyChanged
    {

        public TagService() {
            int i;
            string var;
            for (i = 0; i < 200; i++) {
                var = "var" + i;
                base.Add(var, new Tag(var, typeof(Int32)));
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        /*
        public void Add<T>(string key, T value) where T : class {
            base.Add(key, value);
        }

        public T GetValue<T>(string key) where T : class {
            return base[key] as T;
        }
        */

        /*
        public void SetValue(string key, string value) {
            base[key].Value = value;
            //OnPropertyChanged(new PropertyChangedEventArgs("Value"));
        }
        */

        /*
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
        */
    }
}
