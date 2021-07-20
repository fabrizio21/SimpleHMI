using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Drivers;

namespace Service.Plc
{
    /// <summary>
    /// Class that contains the data to connect to the Pmac
    /// </summary>
    public class PmacConnection
    {
        #region "Members"
        public string IpAddress { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Port { get; private set; }
        public int BufferLen { get; private set; }
        #endregion

        #region "Contructors"
        public PmacConnection() {
            IpAddress = "192.168.0.200";
            Username = "root";
            Password = "deltatau";
            Port = "22";
            BufferLen = 1;
        }

        public PmacConnection(string ipAddr, string username, string password, string port, int bufferLen = 1)
        {
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
    public class PmacServiceX
    {

        private readonly System.Timers.Timer _timer;    // read timer
        private DateTime _lastScanTime;                 // last scan time
        private PmacConnection  _conn;                  // connection parameters
        private ConnectionStatus _connectionStatus;     // connection status
        private TimeSpan _scanTime;                     // tempo di scan valori
        private int _refreshRate;                       // tempo di refresh letture plc
        private bool[] _alarms;                         // list of alarms
        private bool[] _warnings;                       // list of warnings

        #region "Properties"
        public ConnectionStatus ConnectionStatus
        {
            get { return _connectionStatus; }
            set { _connectionStatus = value; }
        }

        public PmacConnection  Connection
        {
            get { return _conn; }
            set { _conn = value; }
        }

        public int RefreshRate {
            get { return _refreshRate; }
            set { _refreshRate = value; } 
        }

        public TimeSpan ScanTime { 
            get { return _scanTime; }
            set { _scanTime = value;  } 
        }

        public bool[] Alarms {
            get { return _alarms; }
            set { _alarms = value; }
        }

        public bool[] Warnings
        {
            get { return _warnings; }
            set { _warnings = value; }
        }

        #endregion

        #region "Events"
        public event EventHandler ValuesRefreshed;
        #endregion

        #region "Constructors"
        public PmacServiceX()
        {
            _timer = new System.Timers.Timer();
            _timer.Interval = 1000;
            _timer.Elapsed += OnTimerElapsed;
        }

        public PmacServiceX(PmacConnection connection, int refreshRate) {
            _conn = connection;
            _refreshRate = refreshRate;
            _timer = new System.Timers.Timer();
            _timer.Interval = _refreshRate;
            _timer.Elapsed += OnTimerElapsed;
        }
        #endregion

        #region "Methods"
        public void Connect() {
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

            _timer.Interval = RefreshRate;
            _timer.Start();
            OnValuesRefreshed();


        }

        public void Disconnect()
        {
            _timer.Stop();
            ConnectionStatus = ConnectionStatus.Offline;
            OnValuesRefreshed();
        }
        #endregion

        /// <summary>
        /// Timer for reading from plc
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Random r = new Random();
            try
            {
                _timer.Stop();
                
                _scanTime = DateTime.Now - _lastScanTime;


                int i,c=0;
                for (i = 0; i < 20; i++) { 
                    _alarms[i] = (r.Next(0, 100) >= 50);
                    c += (_alarms[i] ? 1 : 0);
                }
                for (i = 0; i < 10; i++)
                {
                    _warnings[i] = (r.Next(0, 100) >= 50);
                    c += (_warnings[i] ? 1 : 0);
                }
                Console.WriteLine("[{0}]", string.Join(", ", _alarms));
                Console.WriteLine("[{0}]", string.Join(", ", _warnings));
                //Alarms = _alarms;
                //Warnings = _warnings;


                RefreshValues();
                OnValuesRefreshed();
            }
            finally
            {
                _timer.Start();
            }
            _lastScanTime = DateTime.Now;
        }

        /// <summary>
        /// Refreshes the tags
        /// </summary>
        private void RefreshValues()
        {
            StringBuilder strResponse = new StringBuilder(1000);
            // qui fa le letture dal controllore
            PmacDriver.GetResponse("p10", strResponse, strResponse.Capacity);


        }

        /// <summary>
        /// Broadcasts the values to who is listening
        /// </summary>
        private void OnValuesRefreshed()
        {
            ValuesRefreshed?.Invoke(this, new EventArgs());
            Debug.Print(DateTime.Now.ToString() + " - " + ScanTime.ToString());
        }

        /*
        public async Task Write(string Command) {
            StringBuilder strResponse = new StringBuilder(1000);
            PmacDriver.GetResponse(Command, strResponse, strResponse.Capacity);
        }
        */
        

    }
}
