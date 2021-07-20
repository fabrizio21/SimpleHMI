using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using SimpleHMI.Infrastructures;
using SimpleHMI.Models;
using SimpleHMI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Translator;

namespace SimpleHMI.ViewModels
{
    public class SideBarViewModel : BindableBase
    {
        #region Members
        private readonly IRegionManager _regionManager;
        private ITranslationService _translationService;
        private readonly PmacService _pmacService;
        #endregion

        #region Properties
        public ITranslationService Translation {
            get { return _translationService; }
        }

        private int _demoState;
        public int DemoState
        {
            get { return _demoState; }
            set { SetProperty(ref _demoState, value); }
        }

        private int _ctrlState;
        public int CtrlState
        {
            get { return _ctrlState; }
            set { SetProperty(ref _ctrlState, value); }
        }

        private int _ctrlMode;
        public int CtrlMode
        {
            get { return _ctrlMode; }
            set { SetProperty(ref _ctrlMode, value); }
        }
        private int _modeState;
        public int ModeState
        {
            get { return _modeState; }
            set { SetProperty(ref _modeState, value); }
        }
        #endregion

        #region Commands
        public ICommand NavigateToMainPageCommand { get; private set; }
        public ICommand NavigateToSettingsPageCommand { get; private set; }
        public ICommand NavigateToAlarmsPageCommand { get; private set; }
        public ICommand NavigateToIoPageCommand { get; private set; }

        // Ctrl State
        public ICommand EnableCommand { get; private set; }
        public ICommand DisableCommand { get; private set; }
        public ICommand ResetErrorCommand { get; private set; }
        public DelegateCommand<object> DemoModeCommand { get; private set; }

        // Ctrl Mode
        public ICommand JogAxisModeCommand { get; private set; }
        public ICommand MoveAxisModeCommand { get; private set; }
        public ICommand IntRefModeCommand { get; private set; }
        public ICommand ExtRefModeCommand { get; private set; }

        #endregion

        #region Constructors
        public SideBarViewModel() { }

        public SideBarViewModel(IRegionManager regionManager,
                                PmacService pmacService,
                                IDialogService dialogService,
                                ITranslationService translationService)
        {
            _translationService = translationService;
            _translationService.LanguageChanged += OnLanguageChanged;

            _pmacService = pmacService;
            _pmacService.ValuesRefreshed += OnValuesRefreshed;

            _regionManager = regionManager;

            // commands
            NavigateToMainPageCommand       = new DelegateCommand(() => NavigateTo("MainPage"));
            NavigateToSettingsPageCommand   = new DelegateCommand(() => NavigateTo("SettingsPage"));
            NavigateToAlarmsPageCommand     = new DelegateCommand(() => NavigateTo("AlarmsPage"));
            NavigateToIoPageCommand         = new DelegateCommand(() => NavigateTo("IOPage"));

            // Ext Ref Commands

            // sub menu
            EnableCommand = new DelegateCommand(async () => await ExecuteWriteCommand("actualStateReq=ENABLE_"));
            DisableCommand = new DelegateCommand(async () => await ExecuteWriteCommand("actualStateReq=DISABLE_"));
            ResetErrorCommand = new DelegateCommand(async () => await ExecuteWriteCommand("actualStateReq=RESET_ERROR_"));
            DemoModeCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteCommand("actualDemoReq", o));


            // Mode Commands
            JogAxisModeCommand = new DelegateCommand(async () => await ExecuteWriteCommand("actualModeReq=MODE_JOG_AXIS"));
            MoveAxisModeCommand = new DelegateCommand(async () => await ExecuteWriteCommand("actualModeReq=MODE_MOVE_AXIS"));
            IntRefModeCommand = new DelegateCommand(async () => await ExecuteWriteCommand("actualModeReq=MODE_INT_REF"));
            ExtRefModeCommand = new DelegateCommand(async () => await ExecuteWriteCommand("actualModeReq=MODE_EXT_REF"));

        }
        #endregion

        #region Methods
        /// <summary>
        /// Data coming from the controller
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnValuesRefreshed(object sender, EventArgs e)
        {
            DemoState = DataRepository.demoState;
            ModeState = DataRepository.modeState;
            CtrlState = DataRepository.ctrlState;
            CtrlMode = DataRepository.ctrlMode;
        }

        private void OnLanguageChanged(object sender, string e){
            RaisePropertyChanged("Translation");
        }
        

        /// <summary>
        /// Navigate to a region
        /// </summary>
        /// <param name="url">Destination region</param>
        private void NavigateTo(string url) {
            _regionManager.RequestNavigate(Regions.ContentRegion, url);
        }
        #endregion

        #region Command handlers
        /// <summary>
        /// Writes a command to the controller
        /// </summary>
        /// <param name="v">i.e. actualReq=1</param>
        /// <returns></returns>
        private async Task ExecuteWriteCommand(string v) {
            await _pmacService.Write(v);
        }

        /// <summary>
        /// Writes a command to the controller with a parameter
        /// </summary>
        /// <param name="v">actualReq</param>
        /// <param name="param">1</param>
        /// <returns></returns>
        private async Task ExecuteWriteCommand(string v, object param)
        {
            //[TODO] occhio alle conversioni di numeri a virgola mobile
            string cmd = param.ToString();
            cmd = v + "=" + cmd;
            await _pmacService.Write(cmd);
        }

        /// <summary>
        /// Writes a command to an indexes variable
        /// </summary>
        /// <param name="v">actualReq</param>
        /// <param name="param">is the index of variable</param>
        /// <returns></returns>
        private async Task ExecuteWriteCommandWithIndex(string v, object param)
        {
            // i.e. from INT_REF_axisCmd({0})=1 to INTREF_axisCmd(2)=1
            string cmd = String.Format(v, param.ToString());
            //cmd = v + "=" + cmd;
            await _pmacService.Write(cmd);
        }

        /// <summary>
        /// Sets the demo mode
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private async Task DemoModeCommandHandler(string v, object param)
        {
            string cmd = (int)param == 0 ? "DEMO_ON" : "DEMO_OFF";
            cmd = v + "=" + cmd;
            await _pmacService.Write(cmd);
        }
        #endregion 
    }
}
