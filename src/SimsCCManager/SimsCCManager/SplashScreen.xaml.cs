using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using SSAGlobals;
using SimsCCManager;
using SimsCCManager.Packages.Containers;
using SimsCCManager.UI.Utilities;
using SimsCCManager.Settings;
using SimsCCManager.Manager;

namespace SimsCCManager.SplashScreen
{
    
    public partial class SplashScreenWindow : Window
    {
        SplashViewModel _viewModel = new();
        public SplashScreenWindow(){
            InitializeComponent();
            this.Show();
            base.DataContext = _viewModel; 
        }
    }

    public class SplashViewModel : INotifyPropertyChanged
    {
        GlobalVariables globals = new();
        LoggingGlobals log = new();

        public Visibility _windowvisible;
        public Visibility WindowVisible{
            get
            {
                return _windowvisible;
            }
            set
            {
                _windowvisible = value;
                RaisePropertyChanged("WindowVisible");
            }
        }

        public Visibility _hidebar;
        public Visibility HideBar{
            get
            {
                return _hidebar;
            }
            set
            {
                _hidebar = value;
                RaisePropertyChanged("HideBar");
            }
        }

        private int _pbarmax;
        public int PBarMax{
            get
            {
                return _pbarmax;
            }
            set
            {
                _pbarmax = value;
                RaisePropertyChanged("PBarMax");
            }
        }

        private int _pbarval;
        public int PBarVal{
            get
            {
                return _pbarval;
            }
            set
            {
                _pbarval = value;
                RaisePropertyChanged("PBarVal");
            }
        }

        private string _pbartext;
        public string PBarText{
            get
            {
                return _pbartext;
            }
            set
            {
                _pbartext = value;
                RaisePropertyChanged("PBarText");
            }
        }

        bool splashing = false;
        bool hasdata = false;

        public SplashViewModel(){
            //for my own ability to know i counted it all! 
            WindowVisible = Visibility.Visible;
            PBarMax = 250;
            PBarVal = 250;
            HideBar = Visibility.Hidden;            
            PBarText = "Loading Sims CC Manager";
            Task update = Task.Run(() => {
                RunStuff();
            });
        }

        public async void RunStuff(){
            splashing = true;
            new Thread(() => Splash()){IsBackground = true}.Start();
            new Thread(() => Pgrss()){IsBackground = true}.Start();
        }

        public async Task Pgrss(){
            Random random = new();
            while (splashing && PBarVal != 0){
                PBarVal--;
                Thread.Sleep(random.Next(10, 35));
            }
        }

        public void Splash(){    
            LoggingGlobals log = new LoggingGlobals();
            if (File.Exists(SettingsFile.SettingFile)){
                hasdata = true;                     
            }
            
            PBarText = "Initializing Components";
            GlobalVariables.Initialize();
            //log.InitializeLog();

            PBarText = "Connecting To Internal Database";
            globals.ConnectDatabase();

            PBarText = "Initializing Internal Database";
            globals.InitializeVariables();         
            
            PBarText = "Setting Settings";
            if (File.Exists(SettingsFile.SettingFile)) {
                SettingsFile.LoadSettingsFile();
                GlobalVariables.alreadyloaded = true;
                log.MakeLog("Finding Settings.", true);

                string lastinstance = SettingsFile.LoadSetting("LastInstance");
                if (lastinstance != null){
                    SettingsFile.LastInstance = lastinstance;
                }
            } else {
                log.MakeLog("No settings found.", true);
            }
            
            PBarText = "Checking for Splines";
            if (hasdata == true) {
                                
            } else {
                log.MakeLog("No data to check.", true);
            }

            log.MakeLog("Loading actual application.", true);
            splashing = false;
            if (PBarVal > 0){
                for (int i = 0; i < PBarVal; i++){
                    PBarVal--;
                }
            }
            PBarText = "Loading Manager";
            Thread.Sleep(120);
            if (File.Exists(SettingsFile.SettingFile)){
                string linstance = SettingsFile.Settings.Where(x => x.SettingName == "LastInstance").First().SettingValue;
                if (Directory.Exists(linstance)){
                    try {
                        Application.Current.Dispatcher.Invoke((Action)delegate{ 
                            var managerWindow = new ManagerWindow();
                            managerWindow.Show();
                            WindowVisible = Visibility.Hidden;
                        });
                    } catch (Exception e) {
                        Console.WriteLine("Caught exception opening Manager Window: " + e.Message + " - " + e.StackTrace);
                    }
                } else {
                    try {
                        Application.Current.Dispatcher.Invoke((Action)delegate{
                            var mainWindow = new MainWindow();
                            mainWindow.Show();
                            WindowVisible = Visibility.Hidden;
                        });
                    } catch (Exception e) {
                        Console.WriteLine("Caught exception opening Main Window: " + e.Message + " - " + e.StackTrace);
                    }
                }                                  
            } else {
                try {
                    Application.Current.Dispatcher.Invoke((Action)delegate{
                        var mainWindow = new MainWindow();
                        mainWindow.Show();
                        WindowVisible = Visibility.Hidden;
                    });
                } catch (Exception e) {
                    Console.WriteLine("Caught exception opening Main Window: " + e.Message + " - " + e.StackTrace);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}