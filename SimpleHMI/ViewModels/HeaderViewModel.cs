
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;

using SimpleHMI.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Translator;
using SimpleHMI.Views;
using SimpleHMI.Services;
using System.Threading.Tasks;
using SimpleHMI.Models;
using System.Collections.ObjectModel;

namespace SimpleHMI.ViewModels
{
    public class HeaderViewModel : BindableBase
    {
        #region Attributes
        private readonly IRegionManager         _regionManager;
        private readonly IDialogService         _dialogService;
        private readonly ITranslationService    _translationService;
        private readonly PmacService            _pmacService;
        private readonly AlarmsService          _alarmService;
        private readonly SecurityService        _securityService;
        private readonly SettingsService        _settingsService;
        #endregion 

        #region Properties
        public int  ActiveAlarms {
            get { return _alarmService.ActiveAlarmsCount; }
        }

        public AlarmsService Alarms {
            get { return _alarmService; }
        }

        private int _activeWarnings;
        public int ActiveWarnings {
            get { return _activeWarnings; }
            set { SetProperty(ref _activeWarnings, value); }
        }

        public ITranslationService Translation {
            get { return _translationService; }
        }

        private int _errorCode;
        public int ErrorCode {
            get { return _errorCode; }
            set { SetProperty(ref _errorCode, value); }
        }

        public SecurityService Security { 
            get { return _securityService; }
        }

        private bool _loginPopupOpen;
        public bool LoginPopupOpen
        {
            get { return _loginPopupOpen; }
            set { SetProperty(ref _loginPopupOpen, value); }
        }
        #endregion

        #region Commands
        public ICommand QuitCommand { get; private set; }
        public ICommand NavigateToSettingsPageCommand { get; private set; }
        public ICommand NavigateToAlarmsPageCommand { get; private set; }
        public ICommand ChangeLanguageCommand { get; private set; }
        public ICommand PanicButtonCommand { get; private set; }

        public ICommand OpenErrorDocumentCommand { get; private set; }

        public ICommand OpenLoginWindowCommand { get; private set; }
        public ICommand LogoutCommand { get; private set; }
        #endregion

        #region Constructors
        public HeaderViewModel() { }

        public HeaderViewModel( IRegionManager regionManager,  
                                IDialogService dialogService,
                                ITranslationService translationService,
                                PmacService pmacService,
                                AlarmsService alarmsService,
                                SecurityService securityService,
                                IDialogService loginWindow,
                                SettingsService settingsService)
        {
            _pmacService = pmacService;
            _pmacService.ValuesRefreshed += OnValuesRefreshed;

            _translationService = translationService;
            _alarmService = alarmsService;

            _regionManager = regionManager;
            _dialogService = dialogService;

            _securityService = securityService;

            _settingsService = settingsService;

            // commands definition
            NavigateToSettingsPageCommand = new DelegateCommand(() => ExecuteNavigateCommand("SettingsPage"));
            NavigateToAlarmsPageCommand = new DelegateCommand(() => ExecuteNavigateCommand("AlarmsPage"));
            ChangeLanguageCommand = new DelegateCommand<string>(ExecuteChangeLanguageCommand);
            PanicButtonCommand = new DelegateCommand(async () => { await ExecutePanicButtonCommand(); });

            OpenErrorDocumentCommand = new DelegateCommand(ExecuteOpenErrorDocumentCommand);
            
            OpenLoginWindowCommand = new DelegateCommand(ExcecuteOpenLoginWindowCommand);
            LogoutCommand = new DelegateCommand(ExecuteLogoutCommand);

            QuitCommand = new DelegateCommand(ExecuteQuitCommand);
        }


        #endregion

        #region Methods
        private void OnValuesRefreshed(object sender, EventArgs e) {
            ErrorCode = DataRepository.ERROR_CODE;
        }

        private void OnLanguageChanged(object sender, string e) {
            //T = (ITranslationService)sender;
        }
        #endregion

        #region Command Handlers

        private void ExecuteOpenErrorDocumentCommand() {
            // opens the default pdf reader
            string path = _settingsService.GetValue<string>("ErrorsPdfPath");
            System.Diagnostics.Process.Start(@path);

            /*
            Process myProcess = new Process();
            myProcess.StartInfo.FileName = "acroRd32.exe"; //not the full application path
            myProcess.StartInfo.Arguments = "/A \"page=2=OpenActions\" C:\\example.pdf";
            myProcess.Start();
            */
        }

        private void ExecuteQuitCommand() {
            var parameters = new DialogParameters
                    {
                        { "title", Translation["CloseApp"] },
                        { "mode", "question" },
                        { "message", Translation["CloseAppQuestion"] },
                        { "button1Text", Translation["Yes"]},
                        { "button2Text", string.Empty},
                        { "button3Text", Translation["No"]}
                    };

            _dialogService.ShowDialog("DialogWindow", parameters, r =>
            {
                if (r.Result == ButtonResult.Yes)
                    System.Environment.Exit(1);
            });

        }

        private void ExcecuteOpenLoginWindowCommand() {
            LoginPopupOpen = false;
            // opens the login window
            var parameters = new DialogParameters
            {
                { "Ok", Translation["Ok"] },
                { "Cancel", Translation["Cancel"] }
            };

            _dialogService.ShowDialog("LoginWindow", parameters, OnLoginWindowClosed);
        }

        private void ExecuteLogoutCommand() {
            _securityService.Logout();
            LoginPopupOpen = false;
        }

        private void OnLoginWindowClosed(IDialogResult obj)
        {
            var result = obj.Result;
            IDialogParameters parameters = obj.Parameters;
            if (result == ButtonResult.OK) {
                // controlla se i parametri sono giusti
                if (!_securityService.Login(    parameters.GetValue<string>("user"),
                                                parameters.GetValue<string>("pwd"))) {
                    // [TODO]login errato, qui bisogna visualizzare un messaggio di errore o riaprire la schermata di login

                }
            }
        }

        private void ExecuteChangeLanguageCommand(string ID) {
            _translationService.ChangeLanguage(ID);
        }

        /// <summary>
        /// Navigate to a region
        /// </summary>
        /// <param name="url">Destination region</param>
        private void ExecuteNavigateCommand(string url) {
            _regionManager.RequestNavigate(Regions.ContentRegion, url);
        }

        private async Task ExecutePanicButtonCommand() {
            await _pmacService.Write("paniqReq=1");
        }
        #endregion
    }
}