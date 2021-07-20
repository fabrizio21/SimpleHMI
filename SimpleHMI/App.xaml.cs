using SimpleHMI.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;
using SimpleHMI.ViewModels;
using System.Configuration;
using SimpleHMI.Properties;
using Translator;
using SimpleHMI.Models;
using System.Data.Entity;
using System.Linq;
using System;
using SimpleHMI.Services;
using System.Threading.Tasks;
using System.Globalization;
using DBModel;
using Prism.Events;

namespace SimpleHMI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        //SplashScreen splash = new SplashScreen();

        protected override void OnStartup(StartupEventArgs e) {
            // qui dovrei caricare la Splahs screen
            base.OnStartup(e);
        }

        protected override Window CreateShell() {

            return Container.Resolve<MainWindow>();
        }

        /// <summary>
        /// Register types, such as services (controllers communication) and types for navigation
        /// </summary>
        /// <param name="containerRegistry"></param>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
            PmacConnection pmacConn;                                    // controller connection parameters
            PmacService pmac;                                           // controller
            ITranslationService translationService;                     // object for translations
            HMIEntities entities = new HMIEntities();                   // Entity Framework for db management
            AlarmsService alarmService;
            SecurityService securityService;
            //IEventAggregator eventAggregator;
            //DataServer dataServer;

            DataRepository.Init(3);

            // initialize the translation service getting the data through Entity Framework
            translationService = new TranslationService(entities);
            securityService = new SecurityService(entities);

            SettingsService settingsManager = new SettingsService(entities, 12);     // configurator

            string ipAddr   = settingsManager.GetValue<string>("PmacIPAddress", "192.168.0.210");  // 192.168.0.210
            string user     = settingsManager.GetValue<string>("PmacUsername", "root");            // root
            string pwd      = settingsManager.GetValue<string>("PmacPwd", "deltatau");             // deltatau
            string port     = settingsManager.GetValue<string>("PmacPort", "22");                  // 22
            int bufLen      = settingsManager.GetValue<int>("PmacBufferLen", 3);                   // 3
            int mainRefreshCycle = settingsManager.GetValue<int>("MainCycleTime", 1000);      // 1000;            // main readings clock
            int secondaryRefreshRate = settingsManager.GetValue<int>("SecondaryCycleTime", 5001);  // 5001;        // refresh time for unsolicited log messages


            string s = settingsManager.GetValue<string>("ErrorsPdfPath");

            // initialize the controller
            pmacConn = new PmacConnection(ipAddr, user, pwd, port, bufLen);
            pmac = new PmacService(pmacConn, mainRefreshCycle, secondaryRefreshRate);
            pmac.Connect();

            // for keeping track of alarms
            
            //alarmService = new AlarmsService(translationService, alr, 20);
            alarmService = new AlarmsService(pmac, translationService, entities, 2000);

            //
            // Register the type as singleton, otherwise it will create a different one for each view model
            //
            containerRegistry.RegisterInstance(pmac);                       // controller manager
            containerRegistry.RegisterInstance(entities);                   // underlying db manager
            containerRegistry.RegisterInstance(translationService);         // language switcher
            containerRegistry.RegisterInstance(alarmService);               // alarm manager
            containerRegistry.RegisterInstance(settingsManager);            // settings management
            containerRegistry.RegisterInstance(securityService);            // login/logout

            //
            // Registers types for navigation
            //
            containerRegistry.RegisterForNavigation<Header, HeaderViewModel>("Header");
            containerRegistry.RegisterForNavigation<SideBar, SideBarViewModel>("SideBar");
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>("MainPage");
            //containerRegistry.RegisterForNavigation<SettingsPage, SettingsPageViewModel>("SettingsPage");
            containerRegistry.RegisterForNavigation<StatusBar, StatusBarViewModel>("StatusBar");
            containerRegistry.RegisterForNavigation<AlarmsPage, AlarmsPageViewModel>("AlarmsPage");
            containerRegistry.RegisterForNavigation<IOPage, IOPageViewModel>("IOPage");
            //containerRegistry.RegisterForNavigation<IOPage, IOPageViewModel>("AlarmsPage");

            //
            // Registers dialog windows
            //
            containerRegistry.RegisterDialog<DialogWindow, DialogWindowViewModel>("DialogWindow");
            containerRegistry.RegisterDialog<LoginWindow, LoginWindowViewModel>("LoginWindow");

        }
    }
}
