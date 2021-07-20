using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Translator
{
    /// <summary>
    /// Generic interface for a translator (usually he derives
    /// </summary>
    public interface ITranslationService : INotifyCollectionChanged, INotifyPropertyChanged
    {

        #region Events
        event EventHandler<string> LanguageChanged;     // event fired when the language changes
        #endregion

        #region Properties
        List<string> AvailableLanguages { get;  }       // returns the list of available languages
        string CurrentLanguage { get; }                 // returns the current language
        CultureInfo CultureInfo { get;  }               // CultureInfo objext (for decimal separators, ...)
        #endregion

        #region Methods
        void ChangeLanguage(string lang);               // loads the new language
        //void LoadAvailableLanguages();                // loads the list of available languages
        void OnLanguageChanged(string e);
        #endregion

        string this[string index] { get; set; }         // indexer declaration
    }
}
