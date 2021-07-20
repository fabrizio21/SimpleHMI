#define DEMO

using Drivers;
using Prism.Mvvm;
using SimpleHMI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace SimpleHMI.Services
{
    /// <summary>
    /// Class to connect to pmac controller
    /// </summary>
    public class PmacConnection
    {
        #region Members
        public string IpAddress { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Port { get; private set; }
        public int BufferLen { get; private set; }
        #endregion

        #region Contructors
        public PmacConnection() {
            IpAddress = "192.168.0.200";
            Username = "root";
            Password = "deltatau";
            Port = "22";
            BufferLen = 1;
        }

        public PmacConnection(string ipAddr, string username, string password, string port, int bufferLen = 1) {
            IpAddress = ipAddr;
            Username = username;
            Password = password;
            Port = port;
            BufferLen = 1;
        }
        #endregion
    }

    /// <summary>
    /// Resemble a pmac service
    /// </summary>
    public class PmacService
    {
        #region Attributes
        private readonly BackgroundWorker _mainWorker;          // background worker for controller access
        private readonly BackgroundWorker _secondaryWorker;     // low priority worker for unsolicited log messages

        private DateTime _lastScanTime;                         // last scan time
        private PmacConnection _conn;                           // connection parameters
        
        private TimeSpan _scanTime;                             // tempo di scan valori

        private bool[] _alarms;                                 // list of alarms
        private bool[] _warnings;                               // list of warnings
        private bool bRun;                                      // worker(s) running bit (or termination bit)

        private string _command;
        private StringBuilder _response;
        private readonly char[] delimiters = new char[] { '\r', '\n', ',' };

        #endregion

        #region Properties
        public ConnectionStatus ConnectionStatus { get; private set; }
        public PmacConnection Connection { get; private set; }
        public int MainRefreshRate { get; private set; }
        public int SecondaryRefreshRate { get; private set; }


        public TimeSpan ScanTime
        {
            get => _scanTime;
            set => _scanTime = value;
        }

        public bool[] Alarms
        {
            get { return _alarms; }
            set { _alarms = value; }
        }

        public bool[] Warnings
        {
            get => _warnings;
            set => _warnings = value;
        }
        #endregion

        #region Events
        public event EventHandler ValuesRefreshed;                  // fires every time the service reads from the controller
        //public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler UnsolicitedLogChanged;            // fires when the unsolicited log changes
        #endregion

        #region Constructors
        public PmacService() {

            _alarms = new bool[10];
            _warnings = new bool[10];

            _mainWorker = new BackgroundWorker();
            _mainWorker.WorkerReportsProgress = true;
            _mainWorker.DoWork += MainWorker;
            _mainWorker.ProgressChanged += MainWorkerProgressChanged;
            bRun = true;
        }

        public PmacService(PmacConnection connection, int mainRefreshRate, int secondaryRefreshRate)
        {
            string space = " ";

            //[TODO] definire una strategia
            _alarms = new bool[200];
            _warnings = new bool[10];

            // prepares the command
            _command = "ctrlState" + space +                            // 1   (1)
                        "ctrlMode" + space +                            // 2   (1)
                        "modeState" + space +                           // 3   (1)
                        "demoState" + space +                           // 4   (1)
                        "axis_FB(0),3" + space +                        // 7   (3)
                        "axis_maxPos(0),3" + space +                    // 10  (3)
                        "axis_maxSpeed(0),3" + space +                  // 13  (3)
                        "axis_maxAcc(0),3" + space +                    // 16  (3)
                        "INT_REF_A_pre(0),3" + space +                  // 19  (3)
                        "INT_REF_F_pre(0),3" + space +                  // 22  (3)
                        "INT_REF_O_pre(0),3" + space +                  // 25  (3)
                        "INT_REF_P_pre(0),3" + space +                  // 28  (3)
                        "INT_REF_syncState(0),3" + space +              // 31  (3)
                        "INT_REF_sweepConstParam_pre(0),3" + space +    // 34  (3)
                        "INT_REF_sineLimitsViolated" + space +          // 35  (1)
                        "INT_REF_sweepLimitsViolated" + space +         // 36  (1)
                        "EXT_REF_syncState(0),3" + space +              // 39  (3)
                        "ERROR_CODE" + space +                          // 40  (1)
                        "mot_SW(0),8" + space +                         // 48  (8)
                        "EXT_REF_processErr" + space +                  // 49  (1)
                        "home_pose(0),3" + space +                      // 52  (3)
                        "X_max_stroke" + space +                        // 53  (1)
                        "Y_max_stroke" + space +                        // 54  (1)
                        "ROT_max_stroke" + space +                      // 55  (1)
                        "X_max_speed_limit" + space +                   // 56  (1)
                        "Y_max_speed_limit" + space +                   // 57  (1)
                        "ROT_max_speed_limit" + space +                 // 58  (1)
                        "X_max_acc_limit" + space +                     // 59  (1)
                        "Y_max_acc_limit" + space +                     // 60  (1)
                        "ROT_max_acc_limit" + space +                   // 61  (1)
                        "panicReq" + space +                            // 62  (1)
                        "axis_slowSpeed(0),3" + space +                 // 65  (3)
                        "JOG_AXIS_controlWord";                         // 66  (1)

            _response = new StringBuilder(1000);

            _conn = connection;
            MainRefreshRate = mainRefreshRate;
            SecondaryRefreshRate = secondaryRefreshRate;

            // main worker, to read main data from the controller
            _mainWorker = new BackgroundWorker();
            _mainWorker.DoWork += MainWorker;
            _mainWorker.WorkerReportsProgress = true;
            _mainWorker.ProgressChanged += MainWorkerProgressChanged;

            // secondary worker, for low priority readings, like Unsolicited messages log
            _secondaryWorker = new BackgroundWorker();
            _secondaryWorker.DoWork += SecondaryWorker;
            _secondaryWorker.WorkerReportsProgress = true;
            _secondaryWorker.ProgressChanged += SecondaryWorkerProgressChanged;

            // runs the workers
            bRun = true;
            _mainWorker.RunWorkerAsync();
            _secondaryWorker.RunWorkerAsync();
        }

        #endregion

        #region "Methods"

        /// <summary>
        /// The read cycle fires the progress changed event because it can be used across threads
        /// Otherwise I have to call App.Current.Dispatcher.Invoke(new Action(() => ... )
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            OnValuesRefreshed();
        }

        private void SecondaryWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            OnUnsolicitedLogChanged();
        }

        /// <summary>
        /// Low priority worker for unsolicited buffers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SecondaryWorker(object sender, DoWorkEventArgs e)
        {
            StringBuilder response = new StringBuilder(1000);
            BackgroundWorker worker = (BackgroundWorker)sender;

            while (bRun)
            {
                if ((worker.CancellationPending == true))
                {
                    e.Cancel = true;
                    break;
                }
                try
                {
#if DEMO
                    Random r = new Random();
                    int i = r.Next(0, 4);

                    DataRepository.Unsolicited = "ciao!";

                    worker.ReportProgress(0);
#else
                    //[TODO] mettere un blocco se sta facnedo le letture nel main worker?
                    if (PmacDriver.PPmacRead_sendgetsends(response, response.Capacity, 500) > 0){
                        DataRepository.Unsolicited = response.ToString();
                        worker.ReportProgress(0);
                    }
#endif
                }
                finally
                {
                    // catches some breath
                    Thread.Sleep(SecondaryRefreshRate);
                }
            }
        }

        /// <summary>
        /// Main reading loop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWorker(object sender, DoWorkEventArgs e) {
            BackgroundWorker worker = (BackgroundWorker)sender;

            while (bRun) {
                if ((worker.CancellationPending == true)) {
                    e.Cancel = true;
                    break;
                }
                try {
                    ScanTime = DateTime.Now - _lastScanTime;
#if DEMO
                    RefreshValuesDemo();
#else
                    RefreshValues();
#endif
                    // generates the report progress event to notify the viewmodels on data change
                    worker.ReportProgress(0);
                }
                finally {
                    // catches some breath
                    Thread.Sleep(MainRefreshRate);
                }
                _lastScanTime = DateTime.Now;
            }
        }

        public void Connect() {
            ConnectionStatus = ConnectionStatus.Connecting;
#if DEMO
            ConnectionStatus = ConnectionStatus.Online;
#else
            try {
                // tries to connect
                //if (!PmacDriver.PPmacConnect_sendgetsends(_conn.IpAddress, "root", "deltatau", "22", 3)) {
                if (!PmacDriver.PPmacConnect_sendgetsends(_conn.IpAddress, _conn.Username, _conn.Password, _conn.Port, _conn.BufferLen)) {
                    // connection error
                    ConnectionStatus = ConnectionStatus.Offline;
                }

                //if (!PmacDriver.PPmacConnect(_conn.IpAddress, "root", "deltatau", "22")) {
                if (!PmacDriver.PPmacConnect(_conn.IpAddress, _conn.Username, _conn.Password, _conn.Port))
                {
                    // connection error, dammit
                    ConnectionStatus = ConnectionStatus.Offline;
                }
                else
                {
                    ConnectionStatus = ConnectionStatus.Online;
                    _mainWorker.RunWorkerAsync();
                }

            } catch(Exception ex) {
                //Debug.Print(ex.Message);
                Console.WriteLine(ex.Message);
            }
#endif
        }

        public void Disconnect() {
            //_worker.
            bRun = false;
            ConnectionStatus = ConnectionStatus.Offline;
            OnValuesRefreshed();
        }
#endregion


        /// <summary>
        /// Loads the values from the controllers and fills the data repository
        /// </summary>
        private void RefreshValues() {

            int i;

            try
            {
                PmacDriver.GetResponse(_command, _response, _response.Capacity);

                string[] strPos = _response.ToString().Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                if (strPos.Length >= 66)
                {
                    // general data
                    DataRepository.ctrlState = GetIntVal(strPos[0]);
                    DataRepository.ctrlMode = GetIntVal(strPos[1]);
                    DataRepository.modeState = GetIntVal(strPos[2]);
                    DataRepository.demoState = GetIntVal(strPos[3]);

                    DataRepository.ERROR_CODE = GetIntVal(strPos[39]);

                    DataRepository.X_max_stroke = GetDoubleVal(strPos[52]);
                    DataRepository.Y_max_stroke = GetDoubleVal(strPos[53]);
                    DataRepository.ROT_max_stroke = GetDoubleVal(strPos[54]);
                    DataRepository.X_max_speed = GetDoubleVal(strPos[55]);
                    DataRepository.Y_max_speed = GetDoubleVal(strPos[56]);
                    DataRepository.ROT_max_speed = GetDoubleVal(strPos[57]);
                    DataRepository.X_max_acc = GetDoubleVal(strPos[58]);
                    DataRepository.Y_max_acc = GetDoubleVal(strPos[59]);
                    DataRepository.ROT_max_acc = GetDoubleVal(strPos[60]);

                    DataRepository.JOG_AXIS_controlWord = GetIntVal(strPos[65]);

                    // axes values
                    for (i = 0; i < 3; i++)
                    {
                        DataRepository.axis_FB[i] = GetDoubleVal(strPos[4 + i]);
                        DataRepository.axis_maxPos[i] = GetDoubleVal(strPos[7 + i]);
                        DataRepository.axis_maxSpeed[i] = GetDoubleVal(strPos[10 + i]);
                        DataRepository.axis_maxAcc[i] = GetDoubleVal(strPos[13 + i]);
                        DataRepository.INT_REF_A_pre[i] = GetDoubleVal(strPos[16 + i]);
                        DataRepository.INT_REF_F_pre[i] = GetDoubleVal(strPos[19 + i]);
                        DataRepository.INT_REF_O_pre[i] = GetDoubleVal(strPos[22 + i]);
                        DataRepository.INT_REF_P_pre[i] = GetDoubleVal(strPos[25 + i]);
                        DataRepository.INT_REF_syncState[i] = GetIntVal(strPos[28 + i]);
                        DataRepository.INT_REF_sweepConstParam_pre[i] = GetIntVal(strPos[31 + i]);
                        DataRepository.EXT_REF_syncState[i] = GetIntVal(strPos[36 + i]);
                        DataRepository.homePose[i] = GetDoubleVal(strPos[49 + i]);
                        DataRepository.axis_slowSpeed[i] = GetDoubleVal(strPos[62 + i]);
                    }

                    // ALARMS

                    // motors alarms
                    // bit0->Amplifier fault
                    // bit1->Encoder loss detection
                    // bit2->Fatal following error
                    // bit3->Integrated current limit
                    // bit4->Software positive stroke limit
                    // bit5->Software negative stroke limit
                    // bit6->Hardware positive stroke limit
                    // bit7->Hardware negative stroke limit
                    // bit8->Auxiliary fault trigger

                    int value;
                    int j;

                    // ogni word ha 9 bit => 8*9=72
                    for (i = 0; i < 8; i++)
                    {
                        value = GetIntVal(strPos[40 + i]);
                        DataRepository.mot_SW[i] = value;
                        for (j = 0; j < 9; j++)
                            _alarms[i * 8 + j] = (value >> j + 1 == 1);
                    }

                    // nibble
                    // bit0->buffer active control failure
                    // bit1->ASC failure on axis X
                    // bit2->ASC failure on axis Y
                    // bit3->ASC failure on axis ROT
                    value = GetIntVal(strPos[48]);
                    DataRepository.EXT_REF_processErr = value;

                    for (j = 0; j < 4; j++)
                        _alarms[72 + j] = (value >> j + 1 == 1);

                    // Bit0(valore 1) : X, violato limite di stroke
                    // Bit1(valore 2) : X, violato limite di speed
                    // Bit2(valore 4) : X, violato limite di acc
                    // Bit3(Bit0<< 3, valore 8) : Y, violato limite di stroke
                    // Bit4(Bit1<< 3, valore 16): Y, violato limite di speed
                    // Bit5(Bit2<< 3, valore 32): Y, violato limite di acc
                    // Bit6(Bit0<< 6, valore 64) : ROT, violato limite di stroke
                    // Bit7(Bit1<< 6, valore 128): ROT, violato limite di speed
                    // Bit8(Bit2<< 6, valore 256): ROT, violato limite di acc
                    value = GetIntVal(strPos[34]);
                    DataRepository.INT_REF_sineLimitsViolated = value;
                    for (j = 0; j < 9; j++)
                        _alarms[76 + j] = (value >> j + 1 == 1);

                    // Bit0(valore 1) : X, violato limite di stroke
                    // Bit1(valore 2) : X, violato limite di speed
                    // Bit2(valore 4) : X, violato limite di acc
                    // Bit3(Bit0<< 3, valore 8) : Y, violato limite di stroke
                    // Bit4(Bit1<< 3, valore 16): Y, violato limite di speed
                    // Bit5(Bit2<< 3, valore 32): Y, violato limite di acc
                    // Bit6(Bit0<< 6, valore 64) : ROT, violato limite di stroke
                    // Bit7(Bit1<< 6, valore 128): ROT, violato limite di speed
                    // Bit8(Bit2<< 6, valore 256): ROT, violato limite di acc
                    value = GetIntVal(strPos[35]);
                    DataRepository.INT_REF_sweepLimitsViolated = value;
                    for (j = 0; j < 9; j++)
                        _alarms[85 + j] = (value >> j + 1 == 1);

                    // Total alarms 72+4+9+9 = 94

                }
            }
            catch (Exception ex) {
                int iii = 21;
            }
        }


        /// <summary>
        /// Refreshes the tags
        /// </summary>
        private void RefreshValuesDemo() {
            Random r = new Random();
            int i = 0;

            //_scanTime = DateTime.Now - _lastScanTime;
            ScanTime = DateTime.Now - _lastScanTime;

            DataRepository.ctrlState = r.Next(-2, 2);               // DISABLE=0, ENABLE=1, ERROR=2, RESET ERROR=-2
            DataRepository.ctrlMode = 3;// r.Next(0, 4);            // MODE_JOG_AXIS=1, MODE_MOVE_AXIS=2, MODE_INT_REF=3, MODE_EXT_REF=4
            DataRepository.modeState = r.Next(0, 4);                // PREOP_ = 0, READY_ = 1, RUNNING_ = 2, PAUSE_ = 3, STOP_ = 4
            i = r.Next(0, 100);
            DataRepository.demoState = (i>50)?1:0;
            DataRepository.JOG_AXIS_controlWord = 7;
            DataRepository.ERROR_CODE = r.Next(0,21);

            // motors alarms
            for (i = 0; i < 8; i++)
                DataRepository.mot_SW[i] = r.Next(0,16);

            // axes values
            for (i = 0; i < 3; i++) {
                DataRepository.axis_FB[i] = r.Next(0, 100);
                DataRepository.axis_maxPos[i] = 100;// r.Next(0, 100);
                DataRepository.axis_maxSpeed[i] = 100;
                DataRepository.axis_maxAcc[i] = r.Next(0, 100);
                DataRepository.INT_REF_A_pre[i] = r.Next(0, 100);
                DataRepository.INT_REF_F_pre[i] = r.Next(0, 100);
                DataRepository.INT_REF_O_pre[i] = r.Next(0, 100);
                DataRepository.INT_REF_P_pre[i] = r.Next(0, 100);
                DataRepository.INT_REF_syncState[i] = r.Next(-2, 2);                // NO_MOTION (0), SYNC (1), RAMP_UP (2), RAMP_DOWN (-2), PAUSED (-1)
                DataRepository.INT_REF_sweepConstParam_pre[i] = r.Next(0, 100); 
                DataRepository.EXT_REF_syncState[i] = r.Next(-2, 2); 
                DataRepository.homePose[i] = r.Next(0, 100);
                DataRepository.axis_slowSpeed[i] = 20;// r.Next(0, 100);
            }

            DataRepository.X_max_stroke = 100;
            DataRepository.Y_max_stroke = 100;
            DataRepository.ROT_max_stroke = 100;

            DataRepository.X_max_speed = 100;
            DataRepository.Y_max_speed = 100;
            DataRepository.ROT_max_speed = 100;

            DataRepository.X_max_acc = 100;
            DataRepository.Y_max_acc = 100;
            DataRepository.ROT_max_acc = 100;

            DataRepository.Unsolicited = "Message " + r.Next(0, 2).ToString();

            ConnectionStatus = ConnectionStatus.Online;
        }

        /// <summary>
        /// Broadcasts the values to the listeners
        /// </summary>
        private void OnValuesRefreshed() {
            ValuesRefreshed?.Invoke(this, new EventArgs());
        }

        private void OnUnsolicitedLogChanged() {
            UnsolicitedLogChanged?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Generic write command (Command=value)
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task Write(string command, string value) {
            return Task.Run(() =>
            {
                StringBuilder response = new StringBuilder(20);
                //[TODO] convertire , in .
                PmacDriver.GetResponse(command + "=" + value, response, response.Capacity);
                Console.WriteLine("{0}: {1}={2} Written!", DateTime.Now, command, value);
            });
        }

        /// <summary>
        /// Generic write command (Command=value)
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task Write(string Command) {
            return Task.Run(() =>
            {
                StringBuilder response = new StringBuilder(20);
                PmacDriver.GetResponse(Command, response, response.Capacity);
                Console.WriteLine("{0}: {1} Written!", DateTime.Now,Command);
            });
        }


        double GetDoubleVal(string str){
            double dVal = 0;

            if (str != null)
                double.TryParse(str, out dVal);

            return dVal;
        }

        int GetIntVal(string str) {
            int iVal = 0;

            if (str != null)
                int.TryParse(str, out iVal);

            return iVal;
        }
    }
}
