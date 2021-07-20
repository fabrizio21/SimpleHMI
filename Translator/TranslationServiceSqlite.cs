using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Translator;

namespace Service
{
    /// <summary>
    /// Class to manage the translations
    /// The items are stored in a sqlite3 database
    /// </summary>
    public class TranslationServiceSqlite : Dictionary<string,string>, ITranslationService
    {
        #region "Members"
        private List<string> _availableLanguages;       // list of available languages (taken from database columns)
        private string _connString;                     // database connection string
        private SQLiteConnection _conn;                 // connection to sqlite database
        private string _currLang;                       // current language
        #endregion

        public event EventHandler<string> LanguageChanged;

        #region "Constructors"
        public TranslationServiceSqlite() : this(@"Data Source =.\Hmi.db; Version=3", "it-it") { }

        /// <summary>
        /// Constructor with connection to to sqlite database
        /// </summary>
        /// <param name="connection"></param>
        public TranslationServiceSqlite(string connectionString, string Language)
        {
            _connString = connectionString;
            _conn = new SQLiteConnection(connectionString);
            _availableLanguages = new List<string>();
            LoadAvailableLanguages();
            LoadLanguage(Language);
        }
        #endregion

        #region Properties
        public List<string> AvailableLanguages { 
            get { return _availableLanguages; } 
        }

        public string CurrentLanguage{
            get { return _currLang; }
        }
        #endregion

        

        /// <summary>
        /// Gets the whole language from the database and stores it in a dictionary
        /// </summary>
        /// <param name="lang">Language identifier</param>
        public void LoadLanguage(string lang) {
            string key, value;
            _conn.Open();

            // gets the current language from the database
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT ID, [" + lang + "] FROM Languages;", _conn))
            {
                // loads the values into the dictionary
                using (SQLiteDataReader rd = cmd.ExecuteReader())
                {
                    base.Clear();
                    //_dic.Clear();
                    while (rd.Read())
                    {
                        key = rd.GetString(0);
                        if (!rd.IsDBNull(1))
                            value = rd.GetString(1);
                        else
                            value = string.Empty;
                        base.Add(key, value);
                    }
                }
                _currLang = lang;
                // fires the event
                OnLanguageChanged(_currLang);
            }
            _conn.Close();
        }

        /// <summary>
        /// Loads the list of available languages
        /// </summary>
        private void LoadAvailableLanguages() {
            _conn.Open();
            // gets the current language from the database
            using (SQLiteCommand cmd = new SQLiteCommand("PRAGMA table_info('Languages');", _conn))
            {
                // loads the values and puts them in the dictionary
                using (SQLiteDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        if (rd.GetString(1) != "ID")
                            _availableLanguages.Add(rd.GetString(1));
                    }
                }
            }
            _conn.Close();
        }

        /// <summary>
        /// Fires the event when the language changes
        /// </summary>
        /// <param name="e"></param>
        public void OnLanguageChanged(string e)
        {
            LanguageChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Redefines the default routine that returns the value,
        /// that returns the key if the value in not inside the dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new string this[string key]
        {
            get {
                string t;
                // returns the key if it's not in the dictionary
                return base.TryGetValue(key, out t) ? t : "##" + key;
            }
            set { base[key] = value; }
        }
    }
}
