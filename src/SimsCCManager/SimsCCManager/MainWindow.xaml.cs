using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Windows.Shapes;
using SimsCCManager.UI.Utilities;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Text;
using SSAGlobals;
using SimsCCManager.Settings;
using SimsCCManager.Manager;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Controls.Ribbon.Primitives;

namespace SimsCCManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {           
        public bool maximized = false;
        public static bool notready = true;
        CCManagerViewModel _viewModel = new CCManagerViewModel();
        public MainWindow()
        {
            InitializeComponent();
            GlobalVariables.WindowsOpen++;
            base.DataContext = _viewModel;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {       
            GlobalVariables.WindowsOpen--;
            Console.WriteLine("Windows Open: {0}", GlobalVariables.WindowsOpen);
            if (GlobalVariables.WindowsOpen < 1) System.Windows.Application.Current.Shutdown();
        }

        void PgButtonNext_Click(object sender, RoutedEventArgs e){
            new Thread (() => {while (notready){

                }
                    Dispatcher.Invoke(new Action(() => OpenCCM()));
            }){IsBackground = true}.Start();
        }

        void OpenCCM(){
            ManagerWindow mw = new ManagerWindow();
            mw.Show();
            this.Close();
        }
    }

    public class CCManagerViewModel : INotifyPropertyChanged
    {
        
        private Visibility _windowvisible;

        public Visibility WindowVisible{
            get { return _windowvisible; }
            set { _windowvisible = value; 
            RaisePropertyChanged("WindowVisible"); }
        }
        private double _smallw;
        public double SmallSizeW{
            get { return _smallw;}
            set { _smallw = value; 
            RaisePropertyChanged("SmallSizeW");}
        }
        private double _smallh;
        public double SmallSizeH{
            get { return _smallh;}
            set { _smallh = value; 
            RaisePropertyChanged("SmallSizeH");}
        }  

        private int _border;
        public int Border{
            get { return _border;}
            set { _border = value; 
            RaisePropertyChanged("Border");}
        }           
        
        
        private bool _maximized;
        public bool Maximized{
            get
            {
                return _maximized;
            }
            set
            {
                _maximized = value;
                RaisePropertyChanged("Maximized");
            }
        }

        private double _wwidth;
        public double WindowWidth{
            get
            {
                return _wwidth;
            }
            set
            {
                _wwidth = value;
                RaisePropertyChanged("WindowWidth");
            }
        }

        private double _wheight;
        public double WindowHeight{
            get
            {
                return _wheight;
            }
            set
            {
                _wheight = value;
                RaisePropertyChanged("WindowHeight");
            }
        }
        
        private Point _loc;
        public Point Location{
            get
            {
                return _loc;
            }
            set
            {
                _loc = value;
                RaisePropertyChanged("Location");
            }
        }

        private Visibility _showbuttonsgrid;

        public Visibility ShowButtonsGrid{
            get { return _showbuttonsgrid; }
            set { _showbuttonsgrid = value; 
            RaisePropertyChanged("ShowButtonsGrid"); }
        }

        private Visibility _showsetupgrid;

        public Visibility ShowSetupGrid{
            get { return _showsetupgrid; }
            set { _showsetupgrid = value; 
            RaisePropertyChanged("ShowSetupGrid"); }
        }

        private Visibility _showinfogrid;

        public Visibility ShowInfoGrid{
            get { return _showinfogrid; }
            set { _showinfogrid = value; 
            RaisePropertyChanged("ShowInfoGrid"); }
        }

        private Visibility _showsettingsgrid;

        public Visibility ShowSettingsGrid{
            get { return _showsettingsgrid; }
            set { _showsettingsgrid = value; 
            RaisePropertyChanged("ShowSettingsGrid"); }
        }


        private Visibility _ShowSetupPg1;

        public Visibility ShowSetupPg1{
            get { return _ShowSetupPg1; }
            set { _ShowSetupPg1 = value; 
            RaisePropertyChanged("ShowSetupPg1"); }
        }

        private Visibility _ShowSetupPg2;

        public Visibility ShowSetupPg2{
            get { return _ShowSetupPg2; }
            set { _ShowSetupPg2 = value; 
            RaisePropertyChanged("ShowSetupPg2"); }
        }
        
        private Visibility _ShowMainGrid;

        public Visibility ShowMainGrid{
            get { return _ShowMainGrid; }
            set { _ShowMainGrid = value; 
            RaisePropertyChanged("ShowMainGrid"); }
        }
        private Visibility _ShowProgressingGrid;

        public Visibility ShowProgressingGrid{
            get { return _ShowProgressingGrid; }
            set { _ShowProgressingGrid = value; 
            RaisePropertyChanged("ShowProgressingGrid"); }
        }

        private Visibility _ShowReportBugGrid;

        public Visibility ShowReportBugGrid{
            get { return _ShowReportBugGrid; }
            set { _ShowReportBugGrid = value; 
            RaisePropertyChanged("ShowReportBugGrid"); }
        }

        private Visibility _DebugModeVis;

        public Visibility DebugModeVis{
            get { return _DebugModeVis; }
            set { _DebugModeVis = value; 
            RaisePropertyChanged("DebugModeVis"); }
        }
        private Visibility _chooseinstancevis;

        public Visibility ChooseInstanceVis{
            get { return _chooseinstancevis; }
            set { _chooseinstancevis = value; 
            RaisePropertyChanged("ChooseInstanceVis"); }
        }
        
        private GameSetup _currentgame;
        public GameSetup CurrentGame{
            get { return _currentgame; }
            set { _currentgame = value; 
            RaisePropertyChanged("CurrentGame"); }
        }

        private ObservableCollection<GameEntry> _gamelist;
        public ObservableCollection<GameEntry> GameList{
            get { return _gamelist; }
            set { _gamelist = value;
            RaisePropertyChanged("GameList");}
        }
        
        private GameEntry _selectedGame;
        public GameEntry SelectedGame
        {
            get { return _selectedGame; }
            set
            {
                if (_selectedGame != value)
                {
                    _selectedGame = value;
                    GameChoice = value.Name;
                    RaisePropertyChanged("SelectedGame");
                }
            }
        }

        private Instance _selectedinstance;
        public Instance SelectedInstance{
            get { return _selectedinstance; }
            set
            {
                if (_selectedinstance != value)
                {
                    _selectedinstance = value;
                    RaisePropertyChanged("SelectedInstance");
                }
            }
        }

        private GameSetup _setupGame;
        public GameSetup SetupGame
        {
            get { return _setupGame; }
            set
            {
                _setupGame = value;
                RaisePropertyChanged("SetupGame");
            }
        }

        private string _gamechoice;
        public string GameChoice{
            get { return _gamechoice; }
            set { _gamechoice = value;
            RaisePropertyChanged("GameChoice");}
        }

        private string _installloc;
        public string InstallLoc{
            get { return _installloc; }
            set { _installloc = value;
            RaisePropertyChanged("InstallLoc");}
        }private string _docloc;
        public string DocLoc{
            get { return _docloc; }
            set { _docloc = value;
            RaisePropertyChanged("DocLoc");}
        }
        private string _gmloc;
        public string GMLoc{
            get { return _gmloc; }
            set { _gmloc = value;
            RaisePropertyChanged("GMLoc");
            SetDirectories();}
        }
        private string _gmlocmods;
        public string GMLocMODS{
            get { return _gmlocmods; }
            set { _gmlocmods = value;
            RaisePropertyChanged("GMLocMODS");}
        }
        private string _gmlocdownloads;
        public string GMLocDOWNLOADS{
            get { return _gmlocdownloads; }
            set { _gmlocdownloads = value;
            RaisePropertyChanged("GMLocDOWNLOADS");}
        }
        private string _gmlocdata;
        public string GMLocDATA{
            get { return _gmlocdata; }
            set { _gmlocdata = value;
            RaisePropertyChanged("GMLocDATA");}
        }
        
        private string _instancename;
        public string InstanceName{
            get { return _instancename; }
            set { _instancename = value;
            RaisePropertyChanged("InstanceName");}
        }

        private List<Instance> _instances;
        private List<Instance> Instances{
            get { return _instances; }
            set { _instances = value;
            RaisePropertyChanged("Instances");}
        }


        private Point locholder = new Point(0,0);
        
        public CCManagerViewModel(){
            new Thread(() => {
                //Console.WriteLine("Instansiating view model.");
                if (GlobalVariables.debugMode == true){
                    DebugModeVis = Visibility.Visible;
                } else {
                    DebugModeVis = Visibility.Hidden;
                }
                ChooseInstanceVis = Visibility.Hidden;
                ShowMainGrid = Visibility.Visible;
                Border = 5;
                SmallSizeW = 550;
                SmallSizeH = 700;
                WindowHeight = SmallSizeH;
                WindowWidth = SmallSizeW;
                Location = new Point(SystemParameters.PrimaryScreenWidth / 4, SystemParameters.PrimaryScreenHeight / 4);

                ShowReportBugGrid = Visibility.Hidden;
                ShowSettingsGrid = Visibility.Hidden;
                ShowInfoGrid = Visibility.Hidden;
                ShowSetupGrid = Visibility.Hidden;
                ShowButtonsGrid = Visibility.Visible;
                ShowSetupPg1 = Visibility.Visible;
                ShowSetupPg2 = Visibility.Hidden;
                ShowProgressingGrid = Visibility.Hidden;
                GameList = new()
                {
                    new GameEntry() { Name = "The Sims 2", Icon = "../../img/s2.png" },
                    new GameEntry() { Name = "The Sims 3", Icon = "../../img/s3.png" },
                    new GameEntry() { Name = "The Sims 4", Icon = "../../img/s4.png" }
                };
                SetupGame = new();
            }){IsBackground = true}.Start();
            
        }

        public ICommand DevTest  
        {  
            get { return new DelegateCommand(this.DevTestClick); }  
        }  
        private void DevTestClick()  
        {
            
        }

        public ICommand ManageCC  
        {  
            get { return new DelegateCommand(this.ManageCCClick); }  
        }  
        private void ManageCCClick()  
        {
            if (SettingsFile.LoadSetting("Instances") == null) {
                ShowSetupGrid = Visibility.Visible;
                ShowButtonsGrid = Visibility.Hidden;
            } else {
                string v = SettingsFile.LoadSetting("Instances");
                List<string> ins = new();
                if (v.Contains(";")){
                    ins = v.Split(";").ToList();
                } else {
                    ins.Add(v);
                }                
                foreach (string instanceloc in ins){
                    string insinf = System.IO.Path.Combine(instanceloc, "instance.ini");
                    Instance instance = new();
                    instance.Location = instanceloc;
                    using (FileStream fs = new FileStream(insinf, FileMode.Open, FileAccess.Read)){
                        using (StreamReader sr = new(fs)){
                            bool eos = false;                
                                while (eos == false){
                                    if(!sr.EndOfStream){
                                        string line = sr.ReadLine();
                                        if (line.Contains("=")){
                                            string[] split = line.Split("=");
                                            if(split[0] == "Name") instance.Name = split[1];
                                            if(split[0] == "Game"){
                                                if (split[1] == "Sims4"){
                                                    instance.Icon = "../../img/s4.png";
                                                } else if (split[1] == "Sims3"){
                                                    instance.Icon = "../../img/s3.png";
                                                } else if (split[1] == "Sims2"){
                                                    instance.Icon = "../../img/s2.png";
                                                }
                                            } 
                                        }
                                    } else {
                                        eos = true;
                                    }
                                }
                            sr.Close();
                        }
                        fs.Close();
                    }
                    Instances.Add(instance);
                }
                ShowButtonsGrid = Visibility.Hidden;
                ChooseInstanceVis = Visibility.Visible;
            }            
        }

        public ICommand Pg0MakeNew{
            get { return new DelegateCommand(this.MakeNewClick); }  
        }
        private void MakeNewClick(){
            ChooseInstanceVis = Visibility.Hidden;
            ShowSetupGrid = Visibility.Visible;
        }

        public ICommand Pg0Select{
            get { return new DelegateCommand(this.Pg0SelectClick); }  
        }
        private void Pg0SelectClick(){             
            Task s = Task.Run(() => { 
                ChooseInstanceVis = Visibility.Hidden;
                ShowProgressingGrid = Visibility.Visible;
            });
            s.Wait();
            Task t = Task.Run(() => LoadInstance(SelectedInstance.Location));
            t.Wait();                
            //ManagerWindow mw = new ManagerWindow();
            //mw.Show();
            MainWindow.notready = false;
            //WindowVisible = Visibility.Hidden;
        }

        public ICommand Pg0Back{
            get { return new DelegateCommand(this.Pg0BackClick); }  
        }
        private void Pg0BackClick(){
            ShowButtonsGrid = Visibility.Visible;
            ChooseInstanceVis = Visibility.Hidden;
        }

        private void LoadInstance(string value){
            SettingsFile.SaveSetting("LastInstance", value);
        }





        public ICommand OpenSettings  
        {  
            get { return new DelegateCommand(this.OpenSettingsClick); }  
        }  
        private void OpenSettingsClick()  
        {
            ShowSetupGrid = Visibility.Visible;
            ShowSettingsGrid = Visibility.Hidden;
        }
        public ICommand OpenHelp  
        {  
            get { return new DelegateCommand(this.OpenHelpClick); }  
        }  
        private void OpenHelpClick()  
        {
            ShowSetupGrid = Visibility.Visible;
            ShowInfoGrid = Visibility.Hidden;
        }
        public ICommand Exit  
        {  
            get { return new DelegateCommand(this.ExitClick); }  
        }  
        private void ExitClick()  
        {
            System.Windows.Application.Current.Shutdown();
        }


        
        public ICommand TwitterButton  
        {  
            get { return new DelegateCommand(this.TwitterButtonClick); }  
        }  
        private void TwitterButtonClick()  
        {
            if (System.Windows.Forms.MessageBox.Show
            ("Open Twitter?", "Opening External URL",
            System.Windows.Forms.MessageBoxButtons.YesNo, 
            System.Windows.Forms.MessageBoxIcon.Question)
            ==System.Windows.Forms.DialogResult.Yes)
                {
                    Process.Start(new ProcessStartInfo("http://twitter.com/sinfulsimming") { UseShellExecute = true });
                }

            else
                {
                //React as needed.
                }
        }
        
        public ICommand KofiButton  
        {  
            get { return new DelegateCommand(this.KofiButtonClick); }  
        }  
        private void KofiButtonClick()  
        {
            if (System.Windows.Forms.MessageBox.Show
            ("Open Kofi?", "Opening External URL",
            System.Windows.Forms.MessageBoxButtons.YesNo, 
            System.Windows.Forms.MessageBoxIcon.Question)
            ==System.Windows.Forms.DialogResult.Yes)
                {
                    Process.Start(new ProcessStartInfo("http://ko-fi.com/sinfulsimming") { UseShellExecute = true });
                }

            else
                {
                //React as needed.
                }
        }
        public ICommand DiscordButton  
        {  
            get { return new DelegateCommand(this.DiscordButtonClick); }  
        }  
        private void DiscordButtonClick()  
        {
            if (System.Windows.Forms.MessageBox.Show
            ("Open support Discord server?", "Opening External URL",
            System.Windows.Forms.MessageBoxButtons.YesNo, 
            System.Windows.Forms.MessageBoxIcon.Question)
            ==System.Windows.Forms.DialogResult.Yes)
                {
                    Process.Start(new ProcessStartInfo("https://discord.gg/M6vnf842Fp") { UseShellExecute = true });
                }

            else
                {
                //React as needed.
                }
        }
        public ICommand GitButton  
        {  
            get { return new DelegateCommand(this.GitButtonClick); }  
        }  
        private void GitButtonClick()  
        {
            if (System.Windows.Forms.MessageBox.Show
            ("Open Github repository?", "Opening External URL",
            System.Windows.Forms.MessageBoxButtons.YesNo, 
            System.Windows.Forms.MessageBoxIcon.Question)
            ==System.Windows.Forms.DialogResult.Yes)
                {
                    Process.Start(new ProcessStartInfo("https://github.com/sixstepsaway/Sims-CC-Manager") { UseShellExecute = true });
                }

            else
                {
                //React as needed.
                }
        }
        
        
        public ICommand ReportABug  
        {  
            get { return new DelegateCommand(this.ReportABugClick); }  
        }  
        private void ReportABugClick()  
        {
            ShowReportBugGrid = Visibility.Visible;
        }

        public ICommand GitIssues  
        {  
            get { return new DelegateCommand(this.GitIssuesClick); }  
        }  
        private void GitIssuesClick()  
        {
            if (System.Windows.Forms.MessageBox.Show
            ("Open Github issues tracker?", "Opening External URL",
            System.Windows.Forms.MessageBoxButtons.YesNo, 
            System.Windows.Forms.MessageBoxIcon.Question)
            ==System.Windows.Forms.DialogResult.Yes)
            {
                Process.Start(new ProcessStartInfo("https://github.com/sixstepsaway/Sims-CC-Manager/issues") { UseShellExecute = true });
            }

            else
            {
                //React as needed.
            }
            ShowReportBugGrid = Visibility.Hidden;
        }
        public ICommand DiscordIssueButton  
        {  
            get { return new DelegateCommand(this.DiscordIssueButtonClick); }  
        }  
        private void DiscordIssueButtonClick()  
        {
            if (System.Windows.Forms.MessageBox.Show
            ("Open support Discord server?", "Opening External URL",
            System.Windows.Forms.MessageBoxButtons.YesNo, 
            System.Windows.Forms.MessageBoxIcon.Question)
            ==System.Windows.Forms.DialogResult.Yes)
            {
                Process.Start(new ProcessStartInfo("https://discord.gg/M6vnf842Fp") { UseShellExecute = true });
            }

            else
            {
            //React as needed.
            }
            ShowReportBugGrid = Visibility.Hidden;
        }
        public ICommand CancelIssue  
        {  
            get { return new DelegateCommand(this.CancelIssueClick); }  
        }  
        private void CancelIssueClick()  
        {
            ShowReportBugGrid = Visibility.Hidden;
        }


        public ICommand Pg1ButtonPrev  
        {  
            get { return new DelegateCommand(this.Pg1ButtonPrevClick); }  
        }  
        private void Pg1ButtonPrevClick()  
        {
            ShowSetupGrid = Visibility.Hidden;
            ShowButtonsGrid = Visibility.Visible;
        }
        public ICommand Pg1ButtonNext  
        {  
            get { return new DelegateCommand(this.Pg1ButtonNextClick); }  
        }  
        private void Pg1ButtonNextClick()  
        {   string mdocloc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); 
            if (GameChoice == null){
                System.Windows.Forms.MessageBox.Show("You must select a game.");
            } else {                
                if (!string.IsNullOrEmpty(InstanceName)){
                    SetupGame.Name = InstanceName;
                } else {
                    SetupGame.Name = GameChoice;
                }
                ShowSetupPg1 = Visibility.Hidden;
                ShowSetupPg2 = Visibility.Visible;
                //Console.WriteLine("Game is {0}", GameChoice);
                if (GameChoice == "The Sims 2"){                
                    SetupGame.Game = Game.Sims2;
                    string gameloc = @"SOFTWARE\WOW6432Node\EA GAMES\The Sims 2";  
                    string loc = GetPathForExe(gameloc);
                    DirectoryInfo locloc = new(loc);
                    string location = locloc.Parent.FullName;
                    string docloc = System.IO.Path.Combine(mdocloc, @"EA Games\The Sims 2");
                    if (Directory.Exists(location)){
                        SetupGame.GameInstall = location;
                    }
                    if (Directory.Exists(docloc)){
                        SetupGame.GameModsFolder = docloc;
                    } else {
                        docloc = System.IO.Path.Combine(mdocloc, @"EA Games\The Sims 2 Ultimate Collection");
                        if (Directory.Exists(docloc)){
                            SetupGame.GameModsFolder = docloc;
                        } else {
                            docloc = System.IO.Path.Combine(mdocloc, @"EA Games\The Sims 2™ Ultimate Collection");
                            if (Directory.Exists(docloc)){
                                SetupGame.GameModsFolder = docloc;
                            }
                        }
                    }                    
                } else if (GameChoice == "The Sims 3"){
                    SetupGame.Game = Game.Sims3;
                    string gameloc = @"SOFTWARE\WOW6432Node\Sims\The Sims 3";  
                    string location = GetPathForExe(gameloc);              
                    string docloc = System.IO.Path.Combine(mdocloc, @"Electronic Arts\The Sims 3");
                    if (Directory.Exists(location)){
                        SetupGame.GameInstall = location;
                    }
                    if (Directory.Exists(docloc)){
                        SetupGame.GameModsFolder = docloc;
                    }
                } else {
                    SetupGame.Game = Game.Sims4;
                    string gameloc = @"SOFTWARE\Maxis\The Sims 4";  
                    string location = GetPathForExe(gameloc);              
                    string docloc = System.IO.Path.Combine(mdocloc, @"Electronic Arts\The Sims 4");
                    if (Directory.Exists(location)){
                        SetupGame.GameInstall = location;
                    }
                    if (Directory.Exists(docloc)){
                        SetupGame.GameModsFolder = docloc;
                    }
                }
            }
            InstallLoc = SetupGame.GameInstall;
            DocLoc = SetupGame.GameModsFolder;
            GMLoc = System.IO.Path.Combine(mdocloc, string.Format(@"Sims CC Manager\Instances\{0}", SetupGame.Name));
            SetDirectories();            
        }

        public void SetDirectories(){
            GMLocMODS = System.IO.Path.Combine(GMLoc, "mods");
            GMLocDATA = System.IO.Path.Combine(GMLoc, "data");
            GMLocDOWNLOADS = System.IO.Path.Combine(GMLoc, "downloads");
            SetupGame.GMData = GMLocDATA;
            SetupGame.GMDownloads = GMLocDOWNLOADS;
            SetupGame.GMMods = GMLocMODS;
            if (SetupGame.Game == Game.Sims2) {
                SetupGame.GameExe = "Sims2EP9.exe";
            } else if (SetupGame.Game == Game.Sims3){
                SetupGame.GameExe = "TS3W.exe";
            } else if (SetupGame.Game == Game.Sims4){
                SetupGame.GameExe = "TS4_x64.exe";
            }
            SetupGame.GMFolder = GMLoc;
        }

        public ICommand Pg2ButtonPrev  
        {  
            get { return new DelegateCommand(this.Pg2ButtonPrevClick); }  
        }  
        private void Pg2ButtonPrevClick()  
        {
            ShowSetupPg2 = Visibility.Hidden;
            ShowSetupPg1 = Visibility.Visible;
        }
        public ICommand Pg2ButtonNext  
        {  
            get { return new RelayCommand(this.Pg2ButtonNextClick); }  
        }  
        private void Pg2ButtonNextClick(object sender)  
        {
            string infofile = System.IO.Path.Combine(SetupGame.GMFolder, "instance.ini");
            if (File.Exists(infofile)){
                System.Windows.Forms.MessageBox.Show("Game build already exists. Please use a different name, or folder.");
            } else {
                ShowSetupPg2 = Visibility.Hidden;
                ShowProgressingGrid = Visibility.Visible;
                new Thread(() => SetupEnvironment()){IsBackground = true}.Start();
            }
        }
        
        public ICommand BrowseGameLoc  
        {  
            get { return new DelegateCommand(this.BrowseGameLocClick); }  
        }  
        private void BrowseGameLocClick()  
        {
            using(var GetFolder = new FolderBrowserDialog())
            {
                DialogResult result = GetFolder.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK) {
                    InstallLoc = GetFolder.SelectedPath;                    
                }
            }
        }
        public ICommand BrowseDocLoc  
        {  
            get { return new DelegateCommand(this.BrowseDocLocClick); }  
        }  
        private void BrowseDocLocClick()  
        {
            using(var GetFolder = new FolderBrowserDialog())
            {
                DialogResult result = GetFolder.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK) {
                    DocLoc = GetFolder.SelectedPath;                    
                }
            }
        }
        public ICommand BrowseGMLoc  
        {  
            get { return new DelegateCommand(this.BrowseGMClick); }  
        }  
        private void BrowseGMClick()  
        {
            using(var GetFolder = new FolderBrowserDialog())
            {
                DialogResult result = GetFolder.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK) {
                    GMLoc = GetFolder.SelectedPath;   
                    SetDirectories();
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

        private static string GetPathForExe(string registryKey)
        {
            string InstallPath = "";
            RegistryKey regKey = Registry.LocalMachine.OpenSubKey(registryKey);

            if (regKey != null)
            {
                InstallPath = regKey.GetValue("Install Dir").ToString();
            }
            //Console.WriteLine("Install path is {0}", InstallPath);
            return InstallPath;
        }

        private void SetupEnvironment(){            
            string infofile = System.IO.Path.Combine(SetupGame.GMFolder, "instance.ini");         
            if (!Directory.Exists(SetupGame.GMFolder)){
                Directory.CreateDirectory(SetupGame.GMFolder);
            } 
            using (StreamWriter streamWriter = new StreamWriter(infofile)){
                streamWriter.WriteLine(string.Format("InstanceName={0}", SetupGame.Name));
                streamWriter.WriteLine(string.Format("Game={0}", SetupGame.Game));
                streamWriter.WriteLine(string.Format("InstallLocation={0}", SetupGame.GameInstall));
                streamWriter.WriteLine(string.Format("GameExe={0}", SetupGame.GameExe));
                streamWriter.WriteLine(string.Format("GameModsFolder={0}", SetupGame.GameModsFolder));
                streamWriter.WriteLine(string.Format("InstanceFolder={0}", SetupGame.GMFolder));
                streamWriter.WriteLine(string.Format("DataFolder={0}", SetupGame.GMData));
                streamWriter.WriteLine(string.Format("ModsFolder={0}", SetupGame.GMMods));
                streamWriter.WriteLine(string.Format("DownloadsFolder={0}", SetupGame.GMDownloads));
                streamWriter.Close();
                streamWriter.Dispose();
            }
            SettingsFile.SaveSetting("LastInstance", SetupGame.GMFolder);
            SettingsFile.AddInstance(SetupGame.GMFolder);
            Directory.CreateDirectory(SetupGame.GMData);
            Directory.CreateDirectory(SetupGame.GMMods);
            Directory.CreateDirectory(SetupGame.GMDownloads);
            MainWindow.notready = false;
        }
    }

    public class GameSetup {
        public Game Game {get; set;}
        public string GameInstall {get; set;}
        public string GameModsFolder {get; set;}
        public string GameExe {get; set;}
        public string Name {get; set;}
        public string GMFolder {get; set;}
        public string GMMods {get; set;}
        public string GMDownloads {get; set;}
        public string GMData {get; set;}
    }

    public class GameEntry {
        public string Name {get; set;}
        public string Icon {get; set;}
    }

    public class Instance {
        public string Name {get; set;}
        public string Icon {get; set;}
        public string Location {get; set;}
    }

    public enum Game {
        Sims2,
        Sims3,
        Sims4
    }
}
