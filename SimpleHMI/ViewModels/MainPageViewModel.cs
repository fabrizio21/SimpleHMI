using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using SimpleHMI.Models;
using SimpleHMI.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Translator;

namespace SimpleHMI.ViewModels
{
    public class MainPageViewModel : BindableBase, INavigationAware
    {
        #region Attributes
        IRegionManager _regionManager;
        private readonly PmacService _pmacService;
        private readonly ITranslationService _translationService;
        private readonly SecurityService _securityService;
        private int _counter;
        #endregion

        #region Commands

        // Axis commands
        public DelegateCommand<object> WriteXRestPose { get; private set; }
        public DelegateCommand<object> WriteYRestPose { get; private set; }
        public DelegateCommand<object> WriteRotRestPose { get; private set; }

        public DelegateCommand<object> WriteXStroke { get; private set; }
        public DelegateCommand<object> WriteYStroke { get; private set; }
        public DelegateCommand<object> WriteRotStroke { get; private set; }

        public DelegateCommand<object> WriteXSpeed { get; private set; }
        public DelegateCommand<object> WriteYSpeed { get; private set; }
        public DelegateCommand<object> WriteRotSpeed { get; private set; }

        public DelegateCommand<object> WriteXAcceleration { get; private set; }
        public DelegateCommand<object> WriteYAcceleration { get; private set; }
        public DelegateCommand<object> WriteRotAcceleration { get; private set; }

        // comandi modo JOG AXIS
        public DelegateCommand<object> JogAxisToggleBitCommand { get; private set; }
        public DelegateCommand<object> JogAxisMouseDownCommand { get; private set; }
        public DelegateCommand<object> JogAxisMouseUpCommand { get; private set; }
        public DelegateCommand JogAxisBackToHomeCommand { get; private set; }

        // comandi modo MOVE AXIS
        public DelegateCommand<object> MoveAxisCommand { get; private set; }
        public DelegateCommand MoveAxisBackToHomeCommand { get; private set; }

        public DelegateCommand<object> WriteTranslationSpeedCommand { get; private set; }
        public DelegateCommand<object> WriteRotationSpeedCommand { get; private set; }

        public DelegateCommand<object> MoveAxisWriteXTargetCommand { get; private set; }
        public DelegateCommand<object> MoveAxisWriteYTargetCommand { get; private set; }
        public DelegateCommand<object> MoveAxisWriteRotTargetCommand { get; private set; }

        public DelegateCommand<object> WriteXJogSpeedCommand { get; private set; }
        public DelegateCommand<object> WriteYJogSpeedCommand { get; private set; }
        public DelegateCommand<object> WriteRotJogSpeedCommand { get; private set; }

        // comandi INT REF
        public DelegateCommand<object> IntRefToggleBitCommand { get; private set; }
        public DelegateCommand IntRefGlobalStartCommand { get; private set; }
        public DelegateCommand IntRefGlobalPauseCommand { get; private set; }
        public DelegateCommand<object> IntRefAxisStartCommand { get; private set; }
        public DelegateCommand<object> IntRefAxisPauseCommand { get; private set; }

        // INT_REF_type_pre(i)=SINE_WAVE oppure LIN_SWEEP oppure CONST_OFFSET
        public DelegateCommand<object> IntRefXModeSelectionChangedCommand { get; private set; }
        public DelegateCommand<object> IntRefYModeSelectionChangedCommand { get; private set; }
        public DelegateCommand<object> IntRefRotModeSelectionChangedCommand { get; private set; }

        // axis selection
        public DelegateCommand<object> IntRefXAxisSelectionChangedCommand { get; private set; }
        public DelegateCommand<object> IntRefYAxisSelectionChangedCommand { get; private set; }
        public DelegateCommand<object> IntRefRotAxisSelectionChangedCommand { get; private set; }

        public DelegateCommand<object> IntRefWriteXAmplitudeCommand { get; private set; }
        public DelegateCommand<object> IntRefWriteYAmplitudeCommand { get; private set; }
        public DelegateCommand<object> IntRefWriteRotAmplitudeCommand { get; private set; }

        public DelegateCommand<object> IntRefWriteXFrequencyCommand { get; private set; }
        public DelegateCommand<object> IntRefWriteYFrequencyCommand { get; private set; }
        public DelegateCommand<object> IntRefWriteRotFrequencyCommand { get; private set; }

        public DelegateCommand<object> IntRefWriteXOffsetCommand { get; private set; }
        public DelegateCommand<object> IntRefWriteYOffsetCommand { get; private set; }
        public DelegateCommand<object> IntRefWriteRotOffsetCommand { get; private set; }

        public DelegateCommand<object> IntRefWriteStartFrequencyCommand { get; private set; }
        public DelegateCommand<object> IntRefWriteFrequencySlopeCommand { get; private set; }

        public DelegateCommand<object> IntRefWriteXSweepConstCommand { get; private set; }
        public DelegateCommand<object> IntRefWriteYSweepConstCommand { get; private set; }
        public DelegateCommand<object> IntRefWriteRotSweepConstCommand { get; private set; }

        public DelegateCommand<object> IntRefXStopModeSelectionChangedCommand { get; private set; }
        public DelegateCommand<object> IntRefYStopModeSelectionChangedCommand { get; private set; }
        public DelegateCommand<object> IntRefRotStopModeSelectionChangedCommand { get; private set; }

        public DelegateCommand<object> IntRefXConstModeSelectionChangedCommand { get; private set; }
        public DelegateCommand<object> IntRefYConstModeSelectionChangedCommand { get; private set; }
        public DelegateCommand<object> IntRefRotConstModeSelectionChangedCommand { get; private set; }

        // comandi EXT REF
        public DelegateCommand<object> ExtRefModeSelectionChangedCommand { get; private set; }
        // stop mode changed
        public DelegateCommand<object> ExtRefXStopModeSelectionChangedCommand { get; private set; }
        public DelegateCommand<object> ExtRefYStopModeSelectionChangedCommand { get; private set; }
        public DelegateCommand<object> ExtRefRotStopModeSelectionChangedCommand { get; private set; }

        public DelegateCommand ExtRefGlobalStartCommand { get; private set; }
        public DelegateCommand ExtRefGlobalPauseCommand { get; private set; }

        public DelegateCommand<object> ExtRefXSelectionChangedCommand { get; private set; }
        public DelegateCommand<object> ExtRefYSelectionChangedCommand { get; private set; }
        public DelegateCommand<object> ExtRefRotSelectionChangedCommand { get; private set; }

        public DelegateCommand<object> ExtRefAxisStartCommand { get; private set; }
        public DelegateCommand<object> ExtRefAxisPauseCommand { get; private set; }

        // unsolicited log commands
        public ICommand SaveUnsolicitedLogCommand { get; private set; }
        public ICommand ClearUnsolicitedLogCommand { get; private set; }
        #endregion

        #region Constructors
        public MainPageViewModel() { }

        public MainPageViewModel(IRegionManager regionManager,
                                 PmacService pmacService,
                                 ITranslationService translationService,
                                 SecurityService securityService)
        {
            _pmacService = pmacService;
            _pmacService.ValuesRefreshed += OnValuesRefreshed;
            _pmacService.UnsolicitedLogChanged += OnUnsolicitedLogChanged;

            _regionManager = regionManager;
            _translationService = translationService;
            _translationService.LanguageChanged += OnLanguageChanged;

            _securityService = securityService;

            _axisList = new AxisList();

            // commands definitions
            // rest pose for X(0), Y(1), Rot(2)
            WriteXRestPose = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("rest_pose(0)", o));
            WriteYRestPose = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("rest_pose(1)", o));
            WriteRotRestPose = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("rest_pose(2)", o));
            
            // strokes
            WriteXStroke = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("X_op_max_stroke", o));
            WriteYStroke = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("Y_op_max_stroke", o));
            WriteRotStroke = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("ROT_op_max_stroke", o));

            // speeds
            WriteXSpeed = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("X_op_max_speed", o));
            WriteYSpeed = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("Y_op_max_speed", o));
            WriteRotSpeed = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("ROT_op_max_speed", o));

            // accelerations
            WriteXAcceleration = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("X_op_max_acc", o));
            WriteYAcceleration = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("Y_op_max_acc", o));
            WriteRotAcceleration = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("ROT_op_max_acc", o));

            // JOG AXIS mode
            JogAxisToggleBitCommand = new DelegateCommand<object>(async (b) => await JogAxisToggleBitCommandHandler(b as object));            
            JogAxisMouseDownCommand = new DelegateCommand<object>(async (b) => await JogMouseDownCommandHandler(b as object), (b) => JogAxisControlWord > 0);
            JogAxisMouseUpCommand = new DelegateCommand<object>(async (b) => await JogMouseUpCommandHandler(b as object), (b) => JogAxisControlWord > 0);

            JogAxisBackToHomeCommand = new DelegateCommand(async () => await WriteCommandHandler("JOG_AXIS_moveCmd=2"));

            WriteXJogSpeedCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("JOG_AXIS_XVel", o));
            WriteYJogSpeedCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("JOG_AXIS_YVel", o));
            WriteRotJogSpeedCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("JOG_AXIS_AVel", o));

            // MOVE AXIS mode
            MoveAxisCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("MOVE_AXIS_moveCmd", o));
            MoveAxisBackToHomeCommand = new DelegateCommand(async () => await WriteCommandHandler("MOVE_AXIS_back2home=1"));
            
            WriteTranslationSpeedCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("MOVE_AXIS_tSpeed", o));
            WriteRotationSpeedCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("MOVE_AXIS_rSpeed", o));

            MoveAxisWriteXTargetCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("MOVE_AXIS_axisRef(0)",o));
            MoveAxisWriteYTargetCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("MOVE_AXIS_axisRef(1)", o));
            MoveAxisWriteRotTargetCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("MOVE_AXIS_axisRef(2)", o));

            // Int Ref Commands
            IntRefToggleBitCommand = new DelegateCommand<object>(async (b) => await IntRefToggleBitCommandHandler(b as object));
            IntRefGlobalStartCommand = new DelegateCommand(async () => await WriteCommandHandler("INT_REF_cmd=1"));
            IntRefGlobalPauseCommand = new DelegateCommand(async () => await WriteCommandHandler("INT_REF_cmd=-1"));

            IntRefAxisStartCommand = new DelegateCommand<object>(async (o) => await ExecuteIndexedCommand("INT_REF_axisCmd({0})=1",o));
            IntRefAxisPauseCommand = new DelegateCommand<object>(async (o) => await ExecuteIndexedCommand("INT_REF_axisCmd({0})=-1",o));

            IntRefWriteXAmplitudeCommand    = new DelegateCommand<object>(async(o) => await ExecuteWriteValueCommand("INT_REF_A_pre(0)", o));
            IntRefWriteYAmplitudeCommand    = new DelegateCommand<object>(async(o) => await ExecuteWriteValueCommand("INT_REF_A_pre(1)", o));
            IntRefWriteRotAmplitudeCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("INT_REF_A_pre(2)", o));

            IntRefWriteXFrequencyCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("INT_REF_F_pre(0)", o));
            IntRefWriteYFrequencyCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("INT_REF_F_pre(1)", o));
            IntRefWriteRotFrequencyCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("INT_REF_F_pre(2)", o));

            IntRefWriteXOffsetCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("INT_REF_O_pre(0)", o));
            IntRefWriteYOffsetCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("INT_REF_O_pre(1)", o));
            IntRefWriteRotOffsetCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("INT_REF_O_pre(2)", o));

            IntRefWriteStartFrequencyCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("INT_REF_startF", o));
            IntRefWriteFrequencySlopeCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("INT_REF_sweepSlope", o));

            // mode selection
            IntRefXModeSelectionChangedCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("INT_REF_type_pre(0)", o));
            IntRefYModeSelectionChangedCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("INT_REF_type_pre(1)", o));
            IntRefRotModeSelectionChangedCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("INT_REF_type_pre(2)", o));

            // axis selection
            IntRefXAxisSelectionChangedCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("INT_REF_activeAxis(0)", o));
            IntRefYAxisSelectionChangedCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("INT_REF_activeAxis(1)", o));
            IntRefRotAxisSelectionChangedCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("INT_REF_activeAxis(2)", o));

            IntRefWriteXSweepConstCommand   = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("INT_REF_sweepConstParam_pre(0)", o)); 
            IntRefWriteYSweepConstCommand   = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("INT_REF_sweepConstParam_pre(1)", o));
            IntRefWriteRotSweepConstCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("INT_REF_sweepConstParam_pre(2)", o));

            IntRefXStopModeSelectionChangedCommand      = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("INT_REF_stopPosSelect(0)", o)); 
            IntRefYStopModeSelectionChangedCommand      = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("INT_REF_stopPosSelect(1)", o)); 
            IntRefRotStopModeSelectionChangedCommand    = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("INT_REF_stopPosSelect(2)", o)); 

            IntRefXConstModeSelectionChangedCommand     = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("INT_REF_sweepConstSelect(0)", o)); 
            IntRefYConstModeSelectionChangedCommand     = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("INT_REF_sweepConstSelect(1)", o)); 
            IntRefRotConstModeSelectionChangedCommand   = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("INT_REF_sweepConstSelect(2)", o));

            // EXT REF Commands
            ExtRefModeSelectionChangedCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("EXT_REF_source_pre", o));

            ExtRefXStopModeSelectionChangedCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("EXT_REF_stopPosSelect(0)", o));
            ExtRefYStopModeSelectionChangedCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("EXT_REF_stopPosSelect(1)", o));
            ExtRefRotStopModeSelectionChangedCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("EXT_REF_stopPosSelect(2)", o));
            
            ExtRefGlobalStartCommand = new DelegateCommand(async () => await WriteCommandHandler("EXT_REF_cmd=1"));
            ExtRefGlobalPauseCommand = new DelegateCommand(async () => await WriteCommandHandler("EXT_REF_cmd=-1"));

            ExtRefXSelectionChangedCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("EXT_REF_activeAxis(0)", o));
            ExtRefYSelectionChangedCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("EXT_REF_activeAxis(1)", o));
            ExtRefRotSelectionChangedCommand = new DelegateCommand<object>(async (o) => await ExecuteWriteValueCommand("EXT_REF_activeAxis(2)", o));

            ExtRefAxisStartCommand = new DelegateCommand<object>(async (o) => await ExecuteIndexedCommand("EXT_REF_axisCmd({0})=1", o));
            ExtRefAxisPauseCommand = new DelegateCommand<object>(async (o) => await ExecuteIndexedCommand("EXT_REF_axisCmd({0})=-1", o));

            // Unsolicited buffer commands
            SaveUnsolicitedLogCommand = new DelegateCommand(ExecuteSaveUnsolicitedLogCommand);
            ClearUnsolicitedLogCommand = new DelegateCommand(ExecuteClearUnsolicitedLogCommand);
        }



        private void OnUnsolicitedLogChanged(object sender, EventArgs e) {
            UnsolicitedLog = DataRepository.Unsolicited;
        }
        #endregion

        #region Properties
        private AxisList _axisList;
        public AxisList AxisList
        {
            get { return _axisList; }
            set { SetProperty(ref _axisList, value); }
        }

        private int _demoState;
        public int DemoState
        {
            get { return _demoState; }
            set { SetProperty(ref _demoState, value); }
        }
        private int _modeState;
        public int ModeState
        {
            get { return _modeState; }
            set { SetProperty(ref _modeState, value); }
        }
        private int _ctrState;
        public int CtrlState
        {
            get { return _ctrState; }
            set { SetProperty(ref _ctrState, value); }
        }
        private int _ctrlMode;
        public int CtrlMode
        {
            get { return _ctrlMode; }
            set { SetProperty(ref _ctrlMode, value); }
        }
        private int _errorCode;
        public int ErrorCode
        {
            get { return _errorCode; }
            set { SetProperty(ref _errorCode, value); }
        }
        public int Counter {
            get { return _counter; }
            set { SetProperty(ref _counter, value); }
        }
        public ITranslationService Translation {
            get { return _translationService; }
        }
        public SecurityService Security {
            get { return _securityService; }
        }
        // word per abilitazione assi (JOG AXIS mode)
        private int _jogAxisControlWord;
        public int JogAxisControlWord
        {
            get { return _jogAxisControlWord; }
            set { 
                if(SetProperty(ref _jogAxisControlWord, value)) {
                    // per design Prism non osserva le proprietà legate all'esecuzione dei comandi
                    // quindi devo forzare il refresh (o usare ObserveProperty durante la creazione del comando)
                    JogAxisMouseDownCommand.RaiseCanExecuteChanged();
                    JogAxisMouseUpCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string _unsolicitedLog;
        public string UnsolicitedLog
        {
            get { return _unsolicitedLog; }
            set { SetProperty(ref _unsolicitedLog, value); }
        }

        private double _translationSpeedMoveAxis;
        public double TranslationSpeedMoveAxis
        {
            get { return _translationSpeedMoveAxis; }
            set { SetProperty(ref _translationSpeedMoveAxis, value); }
        }

        private double _rotationSpeedMoveAxis;
        public double RotationSpeedMoveAxis
        {
            get { return _rotationSpeedMoveAxis; }
            set { SetProperty(ref _rotationSpeedMoveAxis, value); }
        }

        private int _extRefSelectedMode;
        public int ExtRefSelectedMode
        {
            get { return _extRefSelectedMode; }
            set { SetProperty(ref _extRefSelectedMode, value); }
        }

        // ext ref stopmode selection for x axis
        private int _extRefXStopModeSelected;
        public int ExtRefXStopModeSelected
        {
            get { return _extRefXStopModeSelected; }
            set { SetProperty(ref _extRefXStopModeSelected, value); }
        }

        // ext ref stopmode selection for y axis
        private int _extRefYStopModeSelected;
        public int ExtRefYStopModeSelected
        {
            get { return _extRefYStopModeSelected; }
            set { SetProperty(ref _extRefYStopModeSelected, value); }
        }

        // ext ref stopmode selection for rot axis
        private int _extRefRotStopModeSelected;
        public int ExtRefRotStopModeSelected
        {
            get { return _extRefRotStopModeSelected; }
            set { SetProperty(ref _extRefRotStopModeSelected, value); }
        }
        /// <summary>
        /// Linear sweep start frequency
        /// </summary>
        private double _INT_REF_startF;
        public double INT_REF_startF
        {
            get { return _INT_REF_startF; }
            set { SetProperty(ref _INT_REF_startF, value); }
        }

        private double _intRefStartFrequency;
        public double IntRefStartFrequency
        {
            get { return _intRefStartFrequency; }
            set { SetProperty(ref _intRefStartFrequency, value); }
        }

        private double _intRefFrequencySlope;
        public double IntRefFrequencySlope
        {
            get { return _intRefFrequencySlope; }
            set { SetProperty(ref _intRefFrequencySlope, value); }
        }
        #endregion

        #region Command handlers
        /// <summary>
        /// Writes a command to the controller
        /// </summary>
        /// <param name="v">i.e. actualReq=1</param>
        /// <returns></returns>
        private async Task WriteCommandHandler(string v) {
            await _pmacService.Write(v);
        }

        /// <summary>
        /// Writes a command to an indexes variable
        /// </summary>
        /// <param name="v">actualReq</param>
        /// <param name="param">is the index of variable</param>
        /// <returns></returns>
        private async Task ExecuteIndexedCommand(string v, object param)
        {
            // i.e. from INT_REF_axisCmd({0})=1 to INTREF_axisCmd(2)=1
            string cmd = String.Format(v, param.ToString());
            //cmd = v + "=" + cmd;
            await _pmacService.Write(cmd);
        }

        /// <summary>
        /// Write a numeric value
        /// </summary>
        /// <param name="v"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        private async Task ExecuteWriteValueCommand(string v, object param) {
            CultureInfo culture;
            double dVal;
            string cmd, sVal;
            Type typ = param.GetType();

            switch (typ.Name) {
                case "Int32":   sVal = param.ToString();
                    break;
                case "String": 
                    sVal = (string)param;
                    break;
                case "Double":
                    // la pmac vuole il punto, quindi metto nel formato degli Stati Uniti
                    culture = CultureInfo.CreateSpecificCulture("en-US");
                    dVal = (double)param;
                    sVal = dVal.ToString(culture);
                    break;
                case "Boolean":
                    sVal = (bool)param ? "1" : "0";
                    break;
                default:
                    throw new Exception("Not supported datatype!");
            }
            cmd = v + "=" + sVal;
            await _pmacService.Write(cmd);
        }


        /// <summary>
        /// Jog button mouse down handler
        /// </summary>
        /// <param name="b"></param>
        private async Task JogMouseDownCommandHandler(object b) {
            if (b != null) {
                string sParam = (string)b;
                string cmd = "JOG_AXIS_moveCmd=";
                if (sParam == "+")
                    cmd += "1";     // mouse down jog +
                else
                    cmd += "-1";    // mouse down jog -
                await _pmacService.Write(cmd);
            }
        }

        /// <summary>
        /// Jog button mouse up handler
        /// </summary>
        /// <param name="b"></param>
        private async Task JogMouseUpCommandHandler(object b)
        {
            if (b != null) {
                await _pmacService.Write("JOG_AXIS_moveCmd=0");
            }
        }

        /// <summary>
        /// Set/unset the axis bit
        /// </summary>
        /// <param name="b"></param>
        private async Task JogAxisToggleBitCommandHandler(object b) {
            if (b != null) {
                string sBit = (string)b;
                int iBit;
                if (int.TryParse(sBit, out iBit)) { 
                    int val = (1 << iBit);
                    // legge il valore attuale della word e inverte il bit
                    if ((_jogAxisControlWord & val) == val)
                        JogAxisControlWord -= val;
                    else
                        JogAxisControlWord += val;
                    await _pmacService.Write("JogAxis_controlWord=" + _jogAxisControlWord.ToString());
                }
            }
        }

        /// <summary>
        /// Set/unset the axis bit
        /// </summary>
        /// <param name="b"></param>
        private async Task IntRefToggleBitCommandHandler(object b)
        {
            if (b != null)
            {
                string sBit = (string)b;
                int iBit;
                if (int.TryParse(sBit, out iBit))
                {
                    int val = (1 << iBit);
                    // legge il valore attuale della word e inverte il bit
                    if ((_jogAxisControlWord & val) == val)
                        JogAxisControlWord -= val;
                    else
                        JogAxisControlWord += val;
                    await _pmacService.Write("JogAxis_controlWord=" + _jogAxisControlWord.ToString());
                }
            }
        }

        /// <summary>
        /// Clears the unsolicited log 
        /// </summary>
        private void ExecuteClearUnsolicitedLogCommand() {
            UnsolicitedLog = string.Empty;
        }

        /// <summary>
        /// Saves the unsolicited log into a txt file
        /// </summary>
        private void ExecuteSaveUnsolicitedLogCommand() {
            string selPath;
            if (UnsolicitedLog != string.Empty)
            {
                // saves the log in a file
                var dialog = new Microsoft.Win32.SaveFileDialog { InitialDirectory = @"c:\tmp" };
                if (dialog.ShowDialog() == true)
                {
                    selPath = dialog.FileName;
                    //[TODO] selPath must be a valid file path!
                    using (var writer = new System.IO.StreamWriter(selPath))
                    {
                        writer.Write(UnsolicitedLog);
                        writer.Flush();
                    }
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// The data repository has just been refreshed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnValuesRefreshed(object sender, EventArgs e) {

            Random r = new Random();
            int i;

            AxisList.A0.MaxPos = DataRepository.axis_maxPos[0];
            AxisList.A1.MaxPos = DataRepository.axis_maxPos[1];
            AxisList.A2.MaxPos = DataRepository.axis_maxPos[2];

            AxisList.A0.MaxSpeed = DataRepository.axis_maxSpeed[0];
            AxisList.A1.MaxSpeed = DataRepository.axis_maxSpeed[1];
            AxisList.A2.MaxSpeed = DataRepository.axis_maxSpeed[2];

            AxisList.A0.SlowSpeed = DataRepository.axis_slowSpeed[0];
            AxisList.A1.SlowSpeed = DataRepository.axis_slowSpeed[1];
            AxisList.A2.SlowSpeed = DataRepository.axis_slowSpeed[2];

            AxisList.A0.MaxStroke = DataRepository.X_max_stroke;
            AxisList.A1.MaxStroke = DataRepository.Y_max_stroke;
            AxisList.A2.MaxStroke = DataRepository.ROT_max_stroke;

            AxisList.A0.MaxSpeed = DataRepository.X_max_speed;
            AxisList.A1.MaxSpeed = DataRepository.Y_max_speed;
            AxisList.A2.MaxSpeed = DataRepository.ROT_max_speed;

            AxisList.A0.MaxAcceleration = DataRepository.X_max_acc;
            AxisList.A1.MaxAcceleration = DataRepository.Y_max_acc;
            AxisList.A2.MaxAcceleration = DataRepository.ROT_max_acc;

            AxisList.A0.EXT_REF_syncState = DataRepository.EXT_REF_syncState[0];
            AxisList.A1.EXT_REF_syncState = DataRepository.EXT_REF_syncState[1];
            AxisList.A2.EXT_REF_syncState = DataRepository.EXT_REF_syncState[2];

            AxisList.A0.INT_REF_syncState = DataRepository.INT_REF_syncState[0];
            AxisList.A1.INT_REF_syncState = DataRepository.INT_REF_syncState[1];
            AxisList.A2.INT_REF_syncState = DataRepository.INT_REF_syncState[2];

            AxisList.A0.INT_REF_A_pre = DataRepository.INT_REF_A_pre[0];
            AxisList.A1.INT_REF_A_pre = DataRepository.INT_REF_A_pre[1];
            AxisList.A2.INT_REF_A_pre = DataRepository.INT_REF_A_pre[2];

            AxisList.A0.INT_REF_F_pre = DataRepository.INT_REF_F_pre[0];
            AxisList.A1.INT_REF_F_pre = DataRepository.INT_REF_F_pre[1];
            AxisList.A2.INT_REF_F_pre = DataRepository.INT_REF_F_pre[2];

            AxisList.A0.INT_REF_O_pre = DataRepository.INT_REF_O_pre[0];
            AxisList.A1.INT_REF_O_pre = DataRepository.INT_REF_O_pre[1];
            AxisList.A2.INT_REF_O_pre = DataRepository.INT_REF_O_pre[2];

            DemoState = DataRepository.demoState;       //
            ModeState =  DataRepository.modeState;      // PREOP_=0, READY_=1, RUNNING_=2, PAUSE_=3, STOP_=4
            CtrlState =  DataRepository.ctrlState;      // DISABLE=0, ENABLE=1, ERROR=2, RESET ERROR=-2
            CtrlMode = DataRepository.ctrlMode;         // MODE_JOG_AXIS=1, MODE_MOVE_AXIS=2, MODE_INT_REF=3, MODE_EXT_REF=4
            ErrorCode = DataRepository.ERROR_CODE;      //

            JogAxisControlWord = DataRepository.JOG_AXIS_controlWord;

        }

        private void OnLanguageChanged(object sender, string e){
            // aggiorna i binding
            // [TODO] Non si può fare in un modo più silenzioso
            RaisePropertyChanged("Translation");
        }
        #endregion

        #region Navigation
        public void OnNavigatedTo(NavigationContext navigationContext) {
            // I'm getting here
            Console.WriteLine("OnNavigatedTo {0}", navigationContext.Uri);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) {
            Console.WriteLine("IsNavigationTarget {0}", navigationContext.Uri);
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext) {
            // Getting away
            Console.WriteLine("OnNavigatedFrom {0}", navigationContext.Uri);
        }
        #endregion
    }
}
 