using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Translator
{
    /// <summary>
    /// Generic interface for a translator (usually he derives
    /// </summary>
    public interface ITranslationService {

        #region Events
        event EventHandler<string> LanguageChanged;     // event fired when language changes
        #endregion

        #region Properties
        List<string> AvailableLanguages { get;  }       // returns the list of available languages
        string CurrentLanguage { get; }                 // returns the current language
        #endregion

        #region Methods
        void LoadLanguage(string lang);                 // loads the new language
        //void LoadAvailableLanguages();                  // loads the list of available languages
        void OnLanguageChanged(string e);
        #endregion

        string this[string index] { get; set; }         // indexer declaration
    }
}
