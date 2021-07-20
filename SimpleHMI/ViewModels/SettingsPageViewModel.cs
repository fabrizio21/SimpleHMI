using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using SimpleHMI.Models;
using SimpleHMI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Translator;

namespace SimpleHMI.ViewModels
{
    public class SettingsPageViewModel : BindableBase
    {
        #region Attributes
        private readonly PmacService _pmacService;
        private readonly ITranslationService _translationService;
        #endregion

        #region Commands
        public ICommand SaveSettingsCommand { get; private set; }
        public ICommand SaveDefaultValueCommand { get; private set; }
        public ICommand NextPageCommand { get; private set; }
        public ICommand PreviousPageCommand { get; private set; }
        #endregion

        #region Properties
        /*
        public SettingsService Settings {
            get { return _dataServer.SettingsService; }
            //set { SetProperty(ref _settings, value); }
        }

        public TagService Tag
        {
            get { return _dataServer.TagService; }
        }


        public int CurrentPage {
            get { return _dataServer.SettingsService.CurrentPage + 1; }
        }

        public int NumPages { 
            get { return _dataServer.SettingsService.NumPages; }
        }
        
        
        /// <summary>
        /// Può muovere indietro se non sono a pagina 1 e ho più pagine
        /// </summary>
        public bool CanMovePrev {
            get { return _dataServer.SettingsService.CanMovePrev; }
        }

        /// <summary>
        /// Può andare avanti solo se non sono sull'ultima pagina
        /// </summary>
        public bool CanMoveNext {
            get { return _dataServer.SettingsService.CanMoveNext; }
        }


        public ITranslationService Translation
        {
            get { return _dataServer.TranslationService; }
            
            //set
            //{
            //    bool flag;
            //    flag = SetProperty(ref _translationService, value);
            //    //[!!!] ho dovuto usare questo perchè altrimenti non lancia l'aggiornamento verso la View

            //    this.RaisePropertyChanged("T");
            //}
            
        }*/
    
        #endregion

        #region Constructors
        public SettingsPageViewModel() { }

        public SettingsPageViewModel(PmacService pmacService,
                                     ITranslationService translationService) {

            //[TODO] Tirare via questa merdaccia!
            // E' stata messa perchè ci sono un sacco di errori di binding e rallentano tutto
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;

            _pmacService = pmacService;
            _translationService = translationService;
            _translationService.LanguageChanged += OnLanguageChanged;

            //_settings = settings;
            
            SaveSettingsCommand = new DelegateCommand<SetupItem>(o => SaveSettings(o as SetupItem));
            SaveDefaultValueCommand = new DelegateCommand<SetupItem>(o => SaveDefault(o as SetupItem));
            NextPageCommand = new DelegateCommand(() => ChangePage(1));
            PreviousPageCommand = new DelegateCommand(() => ChangePage(-1));

            ChangePage(0);
        }

        #endregion

        /// <summary>
        /// Saves the new value in the collection, in the database and writes to the plc if enabled
        /// A fare le cose fatte bene bisognerebbe scrivere al plc; se la scrittura è ok scrivere anche nel db 
        /// </summary>
        /// <param name="setupItem"></param>
        private void SaveSettings(SetupItem item) {
            /*
            if (item.IsValid(Translation.CultureInfo)) {
                _dataServer.SettingsService.SetValue(item.Name, item);
            }
            else {
                // qui deve caricare un valore di default
                // sarebbe il top caricare quello precedente
                _dataServer.SettingsService.SetDefault(item);
            }
            // scrive al plc (se abilitato) e salva nei settings
            if ((bool)item.WriteToPlc) {
                //_pmacService.Write(item.ReadVar, item.Value);
            }*/
        }

        private void SaveDefault(SetupItem item) {
            //_dataServer.SettingsService.SetDefault(item);
        }

        /// <summary>
        /// Asks the service to change page (+1 next page, -1 previous page)
        /// </summary>
        /// <param name="movement"></param>
        private void ChangePage(int movement) {
            //_dataServer.SettingsService.CurrentPage += movement;

           
            // altrimenti non aggiorna stati e pagina
            RaisePropertyChanged("CurrentPage");
            RaisePropertyChanged("CanMoveNext");
            RaisePropertyChanged("CanMovePrev");
            
        }

        /// <summary>
        /// Event fired by Translator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLanguageChanged(object sender, string e)
        {
            
            //[TODO] non mi piace questa roba qua sotto. L'aggiornamento della struttura deve essere fatto dallo stesso thread
            // dove è stata creata

            //App.Current.Dispatcher.Invoke(new Action(() => ActiveAlarmsList.Refresh(_pmacService.Alarms, true)));
        }
    }
}
