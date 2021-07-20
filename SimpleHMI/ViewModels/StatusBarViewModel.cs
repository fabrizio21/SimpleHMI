using Prism.Mvvm;
using Prism.Regions;
using System;
using Translator;
using SimpleHMI.Services;

namespace SimpleHMI.ViewModels
{
    public class StatusBarViewModel : BindableBase
    {
        #region "Members"
        private readonly PmacService _pmacService;
        private ConnectionStatus _connStatus;
        private TimeSpan _scanTime;
        private ITranslationService _translationService;
        #endregion

        #region "Properties"
        public ConnectionStatus ConnectionStatus {
            get { return _connStatus; }
            set {
                SetProperty(ref _connStatus, value);
            }
        }

        public TimeSpan ScanTime {
            get { return _scanTime; }
            set {
                SetProperty(ref _scanTime, value);
            }
        }

        public ITranslationService Translation {
            get { return _translationService;  }
        }
        #endregion

        #region "Constructors"
        public StatusBarViewModel() { }

        /// <summary>
        /// Constructors with controller object
        /// </summary>
        /// <param name="pmacService"></param>
        public StatusBarViewModel(  IRegionManager regionManager,
                                    ITranslationService translationService,
                                    PmacService pmacService)
        {
            _pmacService = pmacService;
            _pmacService.ValuesRefreshed += OnValuesRefreshed;
            translationService.LanguageChanged += OnLanguageChanged;
        }

        private void OnLanguageChanged(object sender, string e) {
            RaisePropertyChanged("Translation");
        }
        #endregion

        /// <summary>
        /// Data coming from the controller
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnValuesRefreshed(object sender, EventArgs e) {
            ConnectionStatus = _pmacService.ConnectionStatus;
            ScanTime = _pmacService.ScanTime;
            //Console.WriteLine("{0} StatusBar_OnValuesRefreshed", DateTime.Now);
        }
        
    }
}
