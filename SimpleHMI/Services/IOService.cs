using SimpleHMI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Translator;

namespace SimpleHMI.Services
{
    /// <summary>
    /// List of system inputs/outputs
    /// </summary>
    class InputService : ObservableCollection<SetupItem>, IPageable {
        #region Attributes
        private int _numIO;                                             // total number of alarms (coming from the configuration)
        private bool[] _bAlarms;                                        // list of alarms (copy of the booleans from controller)
        //private ObservableCollection<Alarm> _alarmList;               // list of alarms
        private readonly ITranslationService _translationService;       // to change the alarm message on screen
        private readonly string[] _translationKeys;                     // dictionary keys (for translation)
        private List<SetupItem> _originalCollection;        // list of all the settings

        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool CanMoveNext { get; set; }
        public bool CanMovePrev { get; set; }

        public void RecalculatePageItems()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Overrides

        protected override void InsertItem(int index, SetupItem item)
        {
            int startIndex = CurrentPage * PageSize;
            int endIndex = startIndex + PageSize;

            //Check if the Index is with in the current Page then add to the collection as below. And add to the originalCollection also
            if ((index >= startIndex) && (index < endIndex))
            {
                base.InsertItem(index - startIndex, item);

                if (Count > PageSize)
                    base.RemoveItem(endIndex);
            }

            if (index >= Count)
                _originalCollection.Add(item);
            else
                _originalCollection.Insert(index, item);
        }

        protected override void RemoveItem(int index)
        {
            int startIndex = CurrentPage * PageSize;
            int endIndex = startIndex + PageSize;
            //Check if the Index is with in the current Page range then remove from the collection as bellow. And remove from the originalCollection also
            if ((index >= startIndex) && (index < endIndex))
            {
                base.RemoveAt(index - startIndex);

                if (Count <= PageSize)
                    base.InsertItem(endIndex - 1, _originalCollection[index + 1]);
            }

            _originalCollection.RemoveAt(index);
        }

        #endregion
    }
}
