using DBModel;
using Prism.Mvvm;
using SimpleHMI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SimpleHMI.Services
{
    /// <summary>
    /// Machine setup managament. Inherits from an observable collection of SetupItem
    /// </summary>
    public class SettingsService : ObservableCollection<SetupItem> {
        #region Attributes
        private readonly HMIEntities _entities;              // database context
        private int _pageSize;                              // items per page
        private int _currentPage;                           // current page (from 0!)
        private int _numPages;                              // total number of pages
        private List<SetupItem> _originalCollection;        // list of all the settings
        private bool _canMoveNext;
        private bool _canMovePrev;
        private CultureInfo _cultureInfo;                   // per conversioni decimali
        #endregion

        #region Properties
        public int PageSize {
            get { return _pageSize; }
            set {
                if (value >= 0) {
                    _pageSize = value;
                    RecalculateThePageItems();
                    OnPropertyChanged(new PropertyChangedEventArgs("PageSize"));
                }
            }
        }

        /// <summary>
        /// Inizia da 0
        /// </summary>
        public int CurrentPage {
            get { return _currentPage; }
            set {
                Stopwatch sw = new Stopwatch();
                if ((value >= 0) && (value < _numPages)) {
                    _currentPage = value;
                    
                    CanMovePrev = _currentPage > 0 && _numPages > 1;
                    CanMoveNext = _currentPage < _numPages - 1;

                    sw.Start();
                    RecalculateThePageItems();
                    sw.Stop();
                    Console.WriteLine("RecalculateThePageItems: " + sw.ElapsedMilliseconds);
                    OnPropertyChanged(new PropertyChangedEventArgs("CurrentPage"));
                    OnPropertyChanged(new PropertyChangedEventArgs("PageSize"));
                }
            }
        }

        public int NumPages {
            get { return _numPages; }
            set {
                _numPages = value;
                OnPropertyChanged(new PropertyChangedEventArgs("NumPages"));
            }
        }

        /// <summary>
        /// Può muovere indietro se non sono a pagina 1 e ho più pagine
        /// </summary>
        public bool CanMovePrev {
            get => _canMovePrev;
            set {
                _canMovePrev = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CanMovePrev"));
            }
        }

        public bool CanMoveNext{
            get => _canMoveNext;
            set {
                _canMoveNext = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CanMoveNext"));
            }
        }

        #endregion

        #region Constructors
        public SettingsService() { }

        public SettingsService(HMIEntities entities, int pageSize) {

            //_cultureInfo = cultureInfo;
            _entities = entities;
            _pageSize = pageSize;

            List<SetupItem> list = new List<SetupItem>();

            // loads the whole list from the database and stores it in a collection
            list = (List<SetupItem>)LoadSettings();
            _originalCollection = new List<SetupItem>();
            ////_originalCollection = (List<SetupItem>)LoadSettings();
            foreach (SetupItem itm in list)
                ////foreach (SetupItem itm in _originalCollection)
                Add(itm);

            //[TODO] questo è qui per far scattare il property change di NumPages!!!
            PageSize = pageSize;

        }
        #endregion

        #region Private
        public void Reload() {
            
            List<SetupItem> list = new List<SetupItem>();

            Clear();

            // loads the whole list from the database and stores it in a collection
            list = (List<SetupItem>)LoadSettings();
            _originalCollection = new List<SetupItem>();
            ////_originalCollection = (List<SetupItem>)LoadSettings();
            foreach (SetupItem itm in list)
                ////foreach (SetupItem itm in _originalCollection)
                Add(itm);

            CurrentPage = 0;
        }


        private void RecalculateThePageItems() {
            Clear();

            // calculates the number of pages
            NumPages = (int)Math.Ceiling((double)_originalCollection.Count / _pageSize);

            int startIndex = _currentPage * _pageSize;

            for (int i = startIndex; i < startIndex + _pageSize; i++) {
                if (_originalCollection.Count > i)
                    base.InsertItem(i - startIndex, _originalCollection[i]);
            }
        }
        #endregion


        #region Overrides

        protected override void InsertItem(int index, SetupItem item) {
            int startIndex = _currentPage * _pageSize;
            int endIndex = startIndex + _pageSize;

            //Check if the Index is with in the current Page then add to the collection as below. And add to the originalCollection also
            if ((index >= startIndex) && (index < endIndex)) {
                base.InsertItem(index - startIndex, item);

                if (Count > _pageSize)
                    base.RemoveItem(endIndex);
            }

            if (index >= Count)
                _originalCollection.Add(item);
            else
                _originalCollection.Insert(index, item);
        }

        protected override void RemoveItem(int index)
        {
            int startIndex = _currentPage * _pageSize;
            int endIndex = startIndex + _pageSize;
            //Check if the Index is with in the current Page range then remove from the collection as bellow. And remove from the originalCollection also
            if ((index >= startIndex) && (index < endIndex)) {
                base.RemoveAt(index - startIndex);

                if (Count <= _pageSize)
                    base.InsertItem(endIndex - 1, _originalCollection[index + 1]);
            }

            _originalCollection.RemoveAt(index);
        }

        #endregion

        /// <summary>
        /// Returns the datatype from an integer value
        /// The int value MUST follow the database rule
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        private Type GetTypeValue(int Type){
            switch (Type) {
                case 1: return typeof(Int32);
                case 2: return typeof(Double);
                case 3: return typeof(Boolean);
                case 4: return typeof(String);
                default: throw new ArgumentOutOfRangeException("Type not supported or not existent!");
            }
        }

        /// <summary>
        /// Loads the settings from database (entity)
        /// </summary>
        /// <returns></returns>
        private IEnumerable<SetupItem> LoadSettings() {
            return _entities.Settings
                .ToArray()                              // this has to be done to allow the next select
                .Select(x => new SetupItem()
                {
                    Name = x.Name,
                    Description = x.Description,
                    MinValue = x.Min,
                    MaxValue = x.Max,
                    TypeValue = GetTypeValue(x.IDType),
                    UnitMeasure = x.UM,
                    IsReadOnly = x.ReadOnly,
                    IsVisible = x.Visible,
                    WriteToPlc = x.WriteToPlc,
                    Format = x.Format,
                    Category = x.IDCategory,
                    Value = x.Value,                         // alla fine perchè viene validato con min e max
                    Criteria = x.Criteria,
                    Default = x.DefaultValue
                })
                .Where(x => x.IsVisible == true)            // seleziona solo i visibili, gli altri sono ad uso interno
                .ToArray()
                .ToList();
        }

        #region Properties
        /// <summary>
        /// Retrieve a value from the settings source
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetValue<T>(string key, T defaultValue) {
            T result;
            Type paramType = typeof(T);
            // loads the settings value
            var val = _entities.Settings.Single(x => x.Name == key);

            if (val == null) {
                // the key is not in the settings table
                //result = default(T);
                result = defaultValue;
                //throw new KeyNotFoundException();
            }
            else {
                try {
                    // tries to convert the value into the desided type
                    result = (T)Convert.ChangeType(val.Value, typeof(T));
                }
                catch (Exception ex) {
                    result = default(T);
                    throw new InvalidCastException();
                }
            }
            return result;
        }

        /// <summary>
        /// Retrieve a value from the settings source
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetValue<T>(string key)
        {
            T result;
            Type paramType = typeof(T);
            // loads the settings value
            var val = _entities.Settings.Single(x => x.Name == key);

            if (val == null)
            {
                // the key is not in the settings table
                //result = default(T);
                result = default(T);
                //throw new KeyNotFoundException();
            }
            else
            {
                try
                {
                    // tries to convert the value into the desided type
                    result = (T)Convert.ChangeType(val.Value, typeof(T));
                }
                catch (Exception ex)
                {
                    result = default(T);
                    throw new InvalidCastException();
                }
            }
            return result;
        }

        /// <summary>
        /// Save a value in the database
        /// [TODO] gestire il separatore decimale
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetValue(string key, SetupItem value) {
            string s;
            // cambio var in arrivo dall'interfaccia, controllo se è in range
            var result = _entities.Settings.Single(b => b.Name == key);
            if (result != null) {

                if (value.TypeValue.Name == "Double") {
                    // converto il decimale corrente nel punto (il db vuole il punto)
                    s = value.Value.Replace(CultureInfo.DefaultThreadCurrentCulture.NumberFormat.NumberDecimalSeparator, ".");
                }
                else
                    s = value.Value;

                result.Value = s; //value.Value;
                // saves in the database
                _entities.SaveChanges();

                // aggiorna la collection 
                var found = base.Items.FirstOrDefault(x => x.Name == key);
                int i = base.Items.IndexOf(found);
                base.Items[i] = value;

                // non spare il PropertyChanged in caso di modifica, devo forzarlo
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        /// <summary>
        /// Set an item to its default value
        /// </summary>
        /// <param name="item"></param>
        public void SetDefault(SetupItem item) {
            // sets the default value and saves in the database
            item.SetDefault();
            var result = _entities.Settings.Single(b => b.Name == item.Name);
            result.Value = item.Value;
            _entities.SaveChanges();
            // notifies
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        #endregion
    }
}

