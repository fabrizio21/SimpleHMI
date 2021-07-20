using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using SimpleHMI.Models;
using SimpleHMI.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Translator;

namespace SimpleHMI.ViewModels
{
    public class AlarmsPageViewModel : BindableBase, INavigationAware
    {
        #region Attributes
        private readonly AlarmsService _alarmService;
        private readonly ITranslationService _translationService;
        private readonly IRegionManager _regionManager;
        private readonly IDialogService _dialogService;
        #endregion

        #region Properties        
        public AlarmsService ActiveAlarmsList
        {
            get { return _alarmService; }
            //set { SetProperty(ref _dataServer.AlarmService, value); }
        }

        public ITranslationService Translation
        {
            get { return _translationService; }
        }

        #endregion

        #region Constructors
        public AlarmsPageViewModel() { }

        public AlarmsPageViewModel( IRegionManager regionManager,
                                    AlarmsService alarmService,
                                    ITranslationService translationService,
                                    IDialogService dialogService)
        {
            _alarmService = alarmService;
            _translationService = translationService;
            _translationService.LanguageChanged += OnLanguageChanged;
            _regionManager = regionManager;
            _dialogService = dialogService;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Event fired by Translator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLanguageChanged(object sender, string e) {
            // dictionary is not observable, so I have to raise the property changed
            RaisePropertyChanged("Translation");
        }

        public void OnNavigatedTo(NavigationContext navigationContext) {
            // I'm getting here
            Console.WriteLine("OnNavigatedTo {0}", navigationContext.Uri);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) {
            Console.WriteLine("IsNavigationTarget {0}", navigationContext.Uri);
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext) {
            // I'm going away
            Console.WriteLine("OnNavigatedFrom {0}", navigationContext.Uri);
        }
        #endregion
    }
}
