using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DBModel;
using SimpleHMI.Models;

namespace Translator
{
    /// <summary>
    /// Class to manage the translations
    /// The items are stored in a sqlite3 database
    /// </summary>
    public class TranslationService : Dictionary<string, TranslationItem>, ITranslationService, INotifyCollectionChanged, INotifyPropertyChanged
    {
        #region "Members"
        private string _currLanguage;                   // current language
        private readonly HMIEntities _entities;          // db context
        private string _localLanguage;                  // the hmi is shipped in italian, engligh and the local language
        private CultureInfo _cultureInfo;               // culture info object for decimal separators and regional settings in general
        #endregion

        public event EventHandler<string> LanguageChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        #region "Constructors"
        /// <summary>
        /// Passing an Entity Framework object
        /// </summary>
        /// <param name="entities"></param>
        public TranslationService(HMIEntities entities)
        {
            _entities = entities;

            // loads the local language (the third language other than italian and english
            var localLang = entities.Settings
                                .Single(x => x.Name == "LocalLanguage");

            if (localLang?.Value != null)
                _localLanguage = localLang.Value;
            
            // loads the current language
            var lang = entities.Settings
                                .Single(x => x.Name == "Language");
            if (lang?.Value != null)
                _currLanguage = lang.Value;

            ChangeLanguage(_currLanguage);
        }
        #endregion

        #region Properties

        public List<string> AvailableLanguages
        {
            get => null;
        }


        public string CurrentLanguage
        {
            get => _currLanguage;
        }

        public string LocalLanguage
        {
            get => _localLanguage;
        }

        public CultureInfo CultureInfo
        {
            get => _cultureInfo;
        }
        #endregion

        /// <summary>
        /// Gets the whole language from the database and stores it in a dictionary
        /// The HMI comes with four languages (italian, english, local1 (i.e. spanish) and local2 (i.e. català)
        /// </summary>
        /// <param name="lang">Language identifier</param>
        public void ChangeLanguage(string lang)
        {
            bool firstTime = false;
            var v = new { ID = 0, IT = "" };

            //[TODO] controllo errore se non riesce a caricare cultureinfo
            //[TODO] convertire "local" nel linguaggio giusto
            //var cultureInfo = CultureInfo.GetCultures(CultureTypes.AllCultures).Where(c => c.Name == lang.ToLower()).ToList();
            var cultureInfo = CultureInfo.GetCultures(CultureTypes.AllCultures).Where(c => c.ThreeLetterWindowsLanguageName == lang).ToList();
            _cultureInfo = (CultureInfo)cultureInfo[0];

            CultureInfo.DefaultThreadCurrentCulture = _cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = _cultureInfo;

            ////CultureInfo.CurrentCulture = _cultureInfo;


            firstTime = (base.Count == 0);

            /*
            SELECT dbo.TranslationMaster.Name, dbo.Translations.Text
            FROM dbo.TranslationMaster LEFT OUTER JOIN
            dbo.Translations ON dbo.TranslationMaster.ID = dbo.Translations.IDString AND dbo.Translations.IDLanguage IN
            (SELECT ID FROM dbo.Languages WHERE (Name = 'ITA'))
            */

            // Linq can get as freaking messy as Sql, but it's MODERN!!!
            var result =    (from TranslationMaster in _entities.TranslationMaster
                            join Translations in _entities.Translations
                            on new {
                                TranslationMaster.ID,
                                IDLanguage = (
                                ((from Languages in _entities.Languages
                                where
                                    Languages.Name == lang
                                select new
                                {
                                    Languages.ID
                                }).FirstOrDefault().ID))
                            }
                            equals new { ID = Translations.IDString, Translations.IDLanguage } into Translations_join
                            from Translations in Translations_join.DefaultIfEmpty()
                            select new {
                                TranslationMaster.Name,
                                Text = Translations.Text
                            }).ToDictionary(t => t.Name, t => t.Text);

            // fills the base dictionary
            foreach (var f in result)
            {
                if (firstTime)
                    base.Add(f.Key, new TranslationItem(f.Value));
                else
                    base[f.Key].Value = f.Value;
            }

            #region Cancellami
            /*

            // loads the dictionary based on the language selected
            switch (lang)
            {
                case "ITA":
                    var dict = _entities.Strings.Select(t => new { t.Name, t.IT })
                                                    .ToDictionary(t => t.Name, t => t);
                    //base.Clear();
                    foreach (var f in dict)
                    {
                        if (firstTime)
                            base.Add(f.Key, new TranslationItem(f.Value.IT));
                        else
                            base[f.Key].Value = f.Value.IT;

                    }

                    //base.Add(f.Key, new TranslationItem(f.Value.IT));

                    break;

                case "ENG":
                    var dict2 = _entities.Strings.Select(t => new { t.Name, t.EN })
                                                    .ToDictionary(t => t.Name, t => t);
                    //base.Clear();
                    foreach (var f in dict2)
                    {
                        if (firstTime)
                            base.Add(f.Key, new TranslationItem(f.Value.EN));
                        else
                            base[f.Key].Value = f.Value.EN;
                    }
                    //base.Add(f.Key, new TranslationItem(f.Value.EN));

                    break;

                default:
                    var dict3 = _entities.Strings.Select(t => new { t.Name, t.local })
                                                    .ToDictionary(t => t.Name, t => t);
                    //base.Clear();
                    foreach (var f in dict3)
                    {
                        if (firstTime)
                            base.Add(f.Key, new TranslationItem(f.Value.local));
                        else
                            base[f.Key].Value = f.Value.local;

                    }
                    //base.Add(f.Key, new TranslationItem(f.Value.local));
                    break;
                    /*
                case "local2":
                    var dict4 = _entities.Languages.Select(t => new { t.ID, t.local2 })
                                .ToDictionary(t => t.ID, t => t);
                    base.Clear();
                    foreach (var f in dict4)
                        base.Add(f.Key, f.Value.local2);
                    break;
            }

            */
            #endregion 

            _currLanguage = lang;

            // saves on the database
            var langu = _entities.Settings
                                .Single(x => x.Name == "Language");
            if (langu != null) {
                langu.Value = _currLanguage;
                _entities.SaveChanges();
            }

            OnCollectionChanged(_currLanguage);
            OnPropertyChanged(new PropertyChangedEventArgs("Value"));

            OnLanguageChanged(_currLanguage);

        }

        /// <summary>
        /// Loads the list of available languages
        /// </summary>
        private void LoadAvailableLanguages()
        {

        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        public void OnCollectionChanged(string e)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
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
        /// Redefines the default return value routine;
        /// returns the key if the value in not inside the dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new string this[string key]
        {
            get
            {
                //string t;
                TranslationItem t;
                // returns the key if it's not in the dictionary
                return base.TryGetValue(key, out t) ? t.Value : "##" + key;
            }
            set { base[key] = new TranslationItem(value); }
        }
    }

}
