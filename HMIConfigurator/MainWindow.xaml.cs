using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using DBModel;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.ComponentModel;

namespace HMIConfigurator
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public HMIEntities _dbEntities;

        private IList<Languages> _langs;
        public IList<DataTypes> _types;

        private ObservableCollection<DBModel.Strings> _strings;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<DBModel.Strings> Strings { 
            get => _strings; 
        }

        public MainWindow()
        {
            InitializeComponent();
           
            _dbEntities = new HMIEntities();

            _dbEntities.Strings.Load();
            _dbEntities.Tags.Load();
            _dbEntities.DataTypes.Load();

            dgStrings.ItemsSource = _dbEntities.Strings.Local;
            cbDataTypes.ItemsSource = _dbEntities.DataTypes.Local;

            LoadStrings();

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            TabControl tab = sender as TabControl;

            switch (tab.SelectedIndex) {
                case 0:
                    // strings, carica i linguaggi di sistema
                    try
                    {


                        //bindingList = ((IListSource)context.Person).GetList()asIBindingList;
                        //var langs = _dbEntities.Languages.Local;//
                        //listBox1.ItemsSource = bindingList;

                        
                    }
                    catch (Exception ex) {
                        int i;
                        i = 12;
                    }
                    
                    
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;

            }
        }

        private void lbLangs_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            //_dbEntities.Languages lang;

            //var lang = lbLangs.SelectedItem as Languages;
            
            //if(lang != null) { 
   



            /*
            var data = _dbEntities.StringsList
        .Join(
            _dbEntities.StringsDef,
            StringListID => StringListID.ID,
            StringDefID => StringDefID.IDString,
            (author, book) => new
            {
                BookId = book.BookId,
                AuthorName = author.Name,
                BookTitle = book.Title
            }
        )
        .Where(StringDefID.)
        .ToList();


            --carica tutte le lingue
SELECT StringsList.Name, StringsDef.Value FROM Stringslist
LEFT JOIN StringsDef ON StringsList.ID = StringsDef.IDString AND IDLanguage = 2
            */
            /*
            var strings = (from p in _dbEntities.StringsList
                           join q in _dbEntities.StringsDef
                           on p.ID equals q.IDString
                           where q.IDLanguage == 1
                           select new
                           {
                               Name = p.Name,
                               Value = q.Value
                           }).ToList();
            //}
            */

            // carico la lista di stringhe per il linguaggio corrente

            // carica l'elemento selezionato
            int i = 21;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {

        }

        private void btnSaveLang_Click(object sender, RoutedEventArgs e) {
            try
            {
                // saves the languages
                _dbEntities.SaveChanges();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.InnerException.Message);
            }
        }

        private void LoadStrings() {

            var localLang = (from TranslationMaster in _dbEntities.TranslationMaster
                            join ITA in _dbEntities.Translations
                                  on new { TranslationMaster.ID, IDLanguage = 1 }
                              equals new { ID = ITA.IDString, ITA.IDLanguage } into ITA_join
                            from ITA in ITA_join.DefaultIfEmpty()
                            join ENG in _dbEntities.Translations
                                  on new { TranslationMaster.ID, IDLanguage = 2 }
                              equals new { ID = ENG.IDString, ENG.IDLanguage } into ENG_join
                            from ENG in ENG_join.DefaultIfEmpty()
                            join Local1 in _dbEntities.Translations
                                  on new { TranslationMaster.ID, IDLanguage = 3 }
                              equals new { ID = Local1.IDString, Local1.IDLanguage } into Local1_join
                            from Local1 in Local1_join.DefaultIfEmpty()
                            join Local2 in _dbEntities.Translations
                                  on new { TranslationMaster.ID, IDLanguage = 4 }
                              equals new { ID = Local2.IDString, Local2.IDLanguage } into Local2_join
                            from Local2 in Local2_join.DefaultIfEmpty()
                             select new TranslationItem
                             {
                                 ID = TranslationMaster.ID,
                                 Name = TranslationMaster.Name,
                                 Ita = ITA.Text,
                                 Eng = ENG.Text,
                                 Local = Local1.Text,
                                 Local2 = Local2.Text
                             }).ToList<TranslationItem>();
            /*
            select new 
            {
                //TranslationMaster.ID,
                TranslationMaster.Name,
                ITATranslation = ITA.Text,
                ENGTranslation = ENG.Text,
                LocalTranslation = Local1.Text,
                Local2Translation = Local2.Text
            }).ToList();
            */
            dgStrings.ItemsSource = localLang;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _dbEntities.SaveChanges();
        }

        private void dgStrings_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e) {
            // devo aggiungere o aggiornare nel database la nuova stringa
            DataGrid dg = sender as DataGrid;
            
            // aggiunge nuovo elemento se non esiste
            // aggiorna elemento se esiste
            // cambia dicitura etichetta

        }

        private void dgStrings_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            int i = 21;
        }
    }
}
