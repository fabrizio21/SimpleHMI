using Prism.Mvvm;
using Prism.Regions;
using SimpleHMI.Infrastructures;
using SimpleHMI.Views;
using Prism.Services.Dialogs;
using Translator;
using System.Collections.ObjectModel;
using SimpleHMI.Models;
using System.Data;
using SimpleHMI.Services;

namespace SimpleHMI.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        //private readonly DataServer _dataServer;
        private string _title = "Simple HMI";

        private PmacService _pmacService;
        private ITranslationService _tranlationService;

        private IDialogService _dialogService;


        #region Properties
        /// <summary>
        /// Translator
        /// </summary>
        public ITranslationService T
        {
            get { return _tranlationService; }
            set { SetProperty(ref _tranlationService, value); }
        }

        /// <summary>
        /// Application title
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        #endregion

        #region Constructors
        public MainWindowViewModel() { }

        public MainWindowViewModel( IRegionManager regionManager, 
                                    //PmacService pmacService, 
                                    //ITranslationService translationService, 
                                    IDialogService dialogService)
        {
            //_pmacService = pmacService;
            //_tranlationService = translationService;
            _regionManager = regionManager;

            // register the region for navigation
            _regionManager.RegisterViewWithRegion(Regions.SideBarRegion, typeof(SideBar));
            _regionManager.RegisterViewWithRegion(Regions.ContentRegion, typeof(MainPage));
            _regionManager.RegisterViewWithRegion(Regions.StatusBarRegion, typeof(StatusBar));
            _regionManager.RegisterViewWithRegion(Regions.HeaderRegion, typeof(Header));
            _regionManager.RegisterViewWithRegion(Regions.ContentRegion, typeof(SettingsPage));
            _regionManager.RegisterViewWithRegion(Regions.ContentRegion, typeof(AlarmsPage));

            _dialogService = dialogService;
        }
        #endregion
    }
}
