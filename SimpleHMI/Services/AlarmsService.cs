using DBModel;
using Prism.Mvvm;
using SimpleHMI.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Data;
using System.Windows.Threading;
using Translator;

namespace SimpleHMI.Services
{
    /// <summary>
    /// Service to refresh the list of alarms
    /// Recieves an array of booleans and builds the alarms table
    /// </summary>
    public class AlarmsService : ObservableCollection<Alarm> //, ICollectionView (per gestire la paginazione
    {
        #region Attributes
        private readonly HMIEntities _entities;                         // for alarms translation
        private int _activeAlarmsCount;                                 // number of active alarms
        private int _numAlarms;                                         // total number of alarms (coming from the configuration)
        private bool[] _bAlarms;                                        // list of alarms (copy of the booleans from controller)
        private readonly ITranslationService _translationService;       // to change the alarm message on screen
        private string[] _translationKeys;                              // dictionary keys (for translation)
        private readonly PmacService _pmacService;                      // to read alarm values
        private DispatcherTimer _timer;                                 // timer to read alarms booleans
        #endregion

        #region Properties
        /// <summary>
        /// Number 
        /// </summary>
        public int ActiveAlarmsCount {
            get { return _activeAlarmsCount; }
            set {
                _activeAlarmsCount = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ActiveAlarmsCount"));
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Deafult constructor
        /// </summary>
        /// <param name="translationService">Dictionary with translation to handle the language switch</param>
        /// <param name="translationKeys">List of alarm translations key in the dictionary</param>
        /// <param name="numAlarms">Total number of alarms</param>
        public AlarmsService(PmacService pmacService,
                            ITranslationService translationService,
                            HMIEntities entities,
                            int refreshInterval = 2000)
        {
            _entities = entities;
            _pmacService = pmacService;
            //_pmacService.ValuesRefreshed += OnValuesRefreshed;
            _translationService = translationService;
            _translationService.LanguageChanged += OnLanguageChanged;

            LoadTranslationKeys();

            if(_translationKeys != null)
                _numAlarms = _translationKeys.Length;
            _bAlarms = new bool[_numAlarms];
            //_alarmList = new ObservableCollection<Alarm>();

            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, refreshInterval / 1000);
            _timer.Tick += OnTick;
            _timer.Start();
        }
        #endregion

        private void LoadTranslationKeys() {
            
            // select IT FROM Strings RIGHT JOIN Alarms ON Strings.ID = Alarms.IDString
            var result = (from Alarms in _entities.Alarms
                              //join Strings in _entities.Strings on new { ID = Alarms.IDString } equals new { ID = Strings.ID } into Strings_join
                          join Strings in _entities.Strings on Alarms.IDString equals Strings.ID into Strings_join
                          from Strings in Strings_join.DefaultIfEmpty()
                         select new
                         {
                             IT = Strings.IT
                         }).ToArray();

            //_translationKeys = result;

            /*
            var alr = _entities.Alarms.Select(y => y.Name).ToArray();
            var str = _entities.Strings.Select(y => y.ID);
            _translationKeys = alr;
            */
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e) {
            // timer di lettura allarmi
            _timer.Stop();
            Refresh(_pmacService.Alarms, false);
            _timer.Start();
        }

        /// <summary>
        /// Checks the new alarms
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTick(object sender, EventArgs e) {
            Refresh(_pmacService.Alarms, false);
        }

        /// <summary>
        /// Language has changed, needs to refresh the alarms
        /// [TODO] Instead of listening to event, I could use a timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLanguageChanged(object sender, string e) {
            RefreshTranslations();
        }

        /*
        /// <summary>
        /// Data coming from the controller
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnValuesRefreshed(object sender, EventArgs e) {
            Refresh(_pmacService.Alarms,false);
        }
        */

        /*
        /// <summary>
        /// This is overriden to let another thread to update the collection
        /// https://www.codeproject.com/Tips/414407/Thread-Safe-Improvement-for-ObservableCollection
        /// 
        /// Otherwise I would have to implement 
        /// App.Current.Dispatcher.Invoke(new Action(() => _alarmsService.Refresh(_pmacService.Alarms, true)));
        /// in the ViewModel
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            // Be nice - use BlockReentrancy like MSDN said
            using (BlockReentrancy())
            {
                var eventHandler = CollectionChanged;
                if (eventHandler != null) {
                    Delegate[] delegates = eventHandler.GetInvocationList();
                    // Walk thru invocation list
                    foreach (NotifyCollectionChangedEventHandler handler in delegates) {
                        var dispatcherObject = handler.Target as DispatcherObject;
                        // If the subscriber is a DispatcherObject and different thread
                        if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                            // Invoke handler in the target dispatcher's thread
                            dispatcherObject.Dispatcher.Invoke(DispatcherPriority.DataBind, handler, this, e);
                        else // Execute handler as is
                            handler(this, e);
                    }
                }
            }
        }
        */

        /// <summary>
        /// Rebuilds the alarm list
        /// </summary>
        /// <param name="alarms">List of booleans</param>
        /// <param name="force">Forces the alarms refresh</param>
        public void Refresh(bool[] alarms, bool force)
        {
            int i;
            string key;
            bool there;
            int activeAlarms;
            Collection<Alarm> colAlr = new Collection<Alarm>();

            // if array are not the same as before or the refresh is forced (i.e. translations)
            if (!_bAlarms.SequenceEqual(alarms) || force)
            {
                //Debug.Print(String.Format("{0}: alarms updated", DateTime.Now));

                // [TODO] forse questa non bisogna farla se il refresh è forzato
                Array.Copy(alarms, _bAlarms, _bAlarms.Length);
                //_alarms = alarms;       

                activeAlarms = 0;

                //base.Clear();
                // ricostruisce la tabella degli allarmi
                for (i = 0; i < _numAlarms - 1; i++)
                {
                    // gets the translation key and tries to find the item in the current list
                    // [TODO] passare lista delle label di allarme soltanto quando cambia lingua, in modo che non debba ricaricare dal dizionario 
                    // quando cambiano gli allarmi, ma solo quando 
                    key = _translationKeys[i];
                    there = base.Items.Any(a => a.Key == key);

                    if (_bAlarms[i]) {
                        // the alarm is active; adds it to the collection if it isn't already in the list
                        if (!there) {
                            try {
                                base.Add(new Alarm() { StartDate = DateTime.Now, Key = key, Message = _translationService[key] });
                                activeAlarms++;
                            }
                            catch (Exception es) {
                                int ii = 121;
                            }
                        }
                    }
                    else {
                        // the alarm is not active, check if is in the list and removes it
                        if (there)
                            base.Remove(base.Items.Where(b => b.Key == key).Single());
                    }
                }
                // refreshes the properties
                ActiveAlarmsCount = activeAlarms;
                // notifica le modifiche alla collection
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        /// <summary>
        /// The language has recently changed, refreshes the list
        /// </summary>
        public void RefreshTranslations() {
            int i;
            for (i = 0; i < base.Count; i++) {
                base[i].Message = _translationService[base[i].Key];
            }
            // notification of the collection change as if it was a reset
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}

