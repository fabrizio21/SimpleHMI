using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Drivers;

namespace Service.Plc
{

    /// <summary>
    /// Resemble a pmac service
    /// </summary>
    public class PmacService : 
    {
        #region Attributes
        private readonly BackgroundWorker _mainWorker;          // background worker for controller access
        //private readonly BackgroundWorker _secondaryWorker;     // for less important stuff

        private DateTime _lastScanTime;                         // last scan time
        private PmacConnection _conn;                           // connection parameters
        private ConnectionStatus _connectionStatus;             // connection status
        private TimeSpan _scanTime;                             // tempo di scan valori
        private int _refreshRate;                               // tempo di refresh letture plc (ms)
        private bool[] _alarms;                                 // list of alarms
        private bool[] _warnings;                               // list of warnings
        private bool bRun;                                      // worker(s) running bit (or termination bit)
        private string[] _rawValues;                            // raw values from controller

        private int _counter;
        #endregion

        #region Properties
        public ConnectionStatus ConnectionStatus {
            get => _connectionStatus; 
            set => _connectionStatus = value; 
        }

        public PmacConnection Connection {
            get => _conn; 
            set => _conn = value; 
        }

        public int RefreshRate {
            get => _refreshRate; 
            set => _refreshRate = value; 
        }

        public TimeSpan ScanTime {
            get => _scanTime; 
            set => _scanTime = value;
        }

        public bool[] Alarms {
            get { return _alarms; }
            set { _alarms = value; }
        }

        public bool[] Warnings {
            get => _warnings; 
            set => _warnings = value; 
        }

        public int Counter {
            get => _counter;
            set => _counter = value;
        }

        public string[] RawValues {
            get => _rawValues;
        }

        #endregion

        #region Events
        public event EventHandler ValuesRefreshed;
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
            _mainWorker.RunWorkerAsync();
        }

        /// <summary>
        /// The read cycle fires the progress changed event because it can be used across threads
        /// Otherwise I have to call App.Current.Dispatcher.Invoke(new Action(() => ... )
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWorkerProgressChanged(object sender, ProgressChangedEventArgs e) {
            OnValuesRefreshed();
        }

        public PmacService(PmacConnection connection, int refreshRate)
        {
            //[TODO] definire una strategia
            _alarms = new bool[20];
            _warnings = new bool[10];
            _rawValues = new string[200];

            _conn = connection;
            _refreshRate = refreshRate;
            _mainWorker = new BackgroundWorker();
            _mainWorker.DoWork += MainWorker;
            _mainWorker.WorkerReportsProgress = true;
            _mainWorker.ProgressChanged += MainWorkerProgressChanged;
            bRun = true;
            _mainWorker.RunWorkerAsync();
        }
        #endregion

        #region "Methods"
        /// <summary>
        /// Main reading loop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWorker(object sender, DoWorkEventArgs e)
        {
            Random r = new Random();
            //int i = 0, c = 0;
            int b = 0, d = 0;
            int count = 5;
            var worker = sender as BackgroundWorker;
            StringBuilder response = new StringBuilder(1000);
            string command;

            command =
"axis_maxPos(0)..axismaxPos(2) " +      // 3
"home_pose(0)..home_pose(2) " +         // 3 (6)
"X_max_stroke " +                       // 1 (7)
"Y_max_stroke " +                       // 1 (8)
"ROT_max_stroke " +                     // 1 (9)
"X_max_speed " +                        // 1 (10)
"Y_max_speed " +                        // 1 (11)
"ROT_max_speed " +                      // 1 (12)
"X_max_acc " +                          // 1 (13)
"Y_max_acc " +                          // 1 (14)
"ROT_max_acc " +                        // 1 (15)
"ctrlState " +                          // 1 (16)
"ctrlMode " +                           // 1 (17)
"modeState " +                          // 1 (18)
"ERROR_CODE " +                         // 1 (19)
"axis_maxSpeed(0)..axis_maxSpeed(2) " +     // 3 (22)
"axis_slowSpeed(0)..axis_slowSpeed(2) " +   // 3 (25)
"INT_REF_sineLimitsViolated " +         // 1 (26)
"INT_REF_sweepConstParam_pre(0)..INT_REF_sweepConstParam_pre(2) " +     // 3 (29)
"INT_REF_sweepLimitsViolated " +        // 1 (30)
"EXT_REF_syncState(0)..EXT_REF_syncState(2) " +               // 3 (33)
"demoState " +                          // 1 (34)
"axis_FB(0),3 " +                       // 3 (37)
"axis_maxpos(0),3 " +                   // 3 (40)
"axis_maxAcc(0),3 " +                   // 3 (43)
"mot_sw(j) " +                          //
"EXT_REF_processErr ";                  //


            while (bRun) {
                if ((worker.CancellationPending == true)) {
                    e.Cancel = true;
                    break;
                }

                try {
                    _scanTime = DateTime.Now - _lastScanTime;


                    PmacDriver.GetResponse(command, response, response.Capacity);

                    RefreshValues();

                    _alarms[0] = (b >= 0 && b <= count);
                    _alarms[1] = _alarms[0];
                    _alarms[2] = _alarms[0];
                    _alarms[3] = _alarms[0];
                    _alarms[4] = _alarms[0];
                    _warnings[1] = !_alarms[0];

                    for (d = 0; d < 200; d++) {
                        _rawValues[d] = (r.Next(0, 100) + d*1000).ToString();
                    }

                    _counter++;

                    if (b >= count / 2)
                        this.ConnectionStatus = ConnectionStatus.Offline;
                    else
                        this.ConnectionStatus = ConnectionStatus.Online;

                        /*
                        for (i = 0; i < 10; i++)
                        {
                            _alarms[i] = (r.Next(0, 100) >= 50);
                            c += (_alarms[i] ? 1 : 0);
                        }

                        for (i = 0; i < 10; i++)
                        {
                            _warnings[i] = (r.Next(0, 100) >= 50);
                            c += (_warnings[i] ? 1 : 0);
                        }
                        */

                        //Console.WriteLine("{0}, {1}: [{2}]", _counter, b, string.Join(", ", _alarms));


                        b++;
                    if (b == count * 2) b = 0;


                    

                    // generates the report progress event to notify the viewmodels on data change
                    worker.ReportProgress(0);
                    //OnValuesRefreshed();
                }
                finally
                {
                    Thread.Sleep(_refreshRate);
                }
                _lastScanTime = DateTime.Now;
            }
        }




        public void Connect()
        {
            ConnectionStatus = ConnectionStatus.Connecting;

            /*
            try {
                // tries to connect
                if (!PmacDriver.PPmacConnect_sendgetsends(_conn.IpAddress, "root", "deltatau", "22", 1))
                {
                    // connection error
                    ConnectionStatus = ConnectionStatus.Offline;
                }

                if (!PmacDriver.PPmacConnect(_conn.IpAddress, "root", "deltatau", "22"))
                {
                    // connection error, dammit
                    ConnectionStatus = ConnectionStatus.Offline;
                }
                else
                {

                }

            } catch(Exception ex) {
                Debug.Print(ex.Message);
            }
            */


            //-----solo per test, tirare su nel ramo della connessione con successo

            ConnectionStatus = ConnectionStatus.Online;
            _alarms = new bool[20];
            _warnings = new bool[10];

            // connection successful, starts the reading timer
            OnValuesRefreshed();
        }

        public void Disconnect() {
            //_worker.
            bRun = false;
            ConnectionStatus = ConnectionStatus.Offline;
            OnValuesRefreshed();
        }
        #endregion

        /// <summary>
        /// Refreshes the tags
        /// </summary>
        private void RefreshValues() {


            /*
            StringBuilder strResponse = new StringBuilder(1000);
            // qui fa le letture dal controllore
            PmacDriver.GetResponse("p10", strResponse, strResponse.Capacity);
            */
        }

        /// <summary>
        /// Broadcasts the values to the listeners
        /// </summary>
        private void OnValuesRefreshed() {
            ValuesRefreshed?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Generic write command (Command=value)
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task Write(string Command, string value)
        {
            return Task.Run(() =>
            {
                StringBuilder response = new StringBuilder(20);
                PmacDriver.GetResponse(Command + "=" + value, response, response.Capacity);
                Console.WriteLine("ciao!");
            });
        }
    }
}
