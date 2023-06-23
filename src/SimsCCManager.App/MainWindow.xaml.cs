/*

Code cannibalized and learned from has been acquired in a number of places. Top credits:

- Delphy's Download Organizer

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;
using System.Diagnostics;
using SSAGlobals;
using SimsCCManager.Packages.Initial;
using SimsCCManager.Packages.Sims2Search;
using SimsCCManager.Packages.Sims3Search;
using SimsCCManager.Packages.Sims4Search;
using SimsCCManager.Packages.Containers;
using SimsCCManager.SortingUIResults;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SQLite;
using SQLiteNetExtensions;
using SQLiteNetExtensions.Extensions;
using SQLiteNetExtensions.Attributes;
using SQLitePCL;
using SQLiteNetExtensions.Extensions.TextBlob;
using System.Data.SQLite;
using SimsCCManager.Misc.TrayReader;
using System.Runtime;
using SimsCCManager.Packages.Sorting;
using System.Threading.Tasks.Dataflow;
using SimsCCManager.Packages.Orphans;
using SQLiteNetExtensions.Extensions;
using System.IO.Packaging;
using System.Windows.Media.Animation;
using MoreLinq;
using FontAwesome.WPF;
using SimsCCManager.App.CustomSortingOptions;
using System.Data.Common;

namespace SimsCCManager.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window    

    {
        LoggingGlobals log = new LoggingGlobals();
        GlobalVariables globalVars = new GlobalVariables();
        InitialProcessing initialprocess = new InitialProcessing();
        ParallelOptions parallelSettings = new ParallelOptions();
        FilesSort filesSort = new FilesSort();
        FindOrphans orphanhunt = new FindOrphans();        

        public Stopwatch sw = new Stopwatch();
        string SelectedFolder = "";
        string statement = "";
        bool hascancelled = false;
        bool stop = false;
        bool eatenturecpu = true;

        private bool FindNewAdditions;
        public static bool dataExists;
        public bool runprogress = false;
        private bool fullystopped = false;
        public int countprogress = 0;
        int processors = Environment.ProcessorCount;
        int threadstouse = 0;
        bool complete = false;
        
        private List<PackageFile> s2pending = new List<PackageFile>();
        private List<PackageFile> s3pending = new List<PackageFile>();
        private List<PackageFile> s4pending = new List<PackageFile>();
        private List<PackageFile> allpending = new List<PackageFile>();
        private List<FileInfo> sims3pack = new List<FileInfo>();
        private List<FileInfo> sims2pack = new List<FileInfo>();
        private List<FileInfo> sims4script = new List<FileInfo>();
        private List<FileInfo> other = new List<FileInfo>();
        private List<FileInfo> packages = new List<FileInfo>();
        private List<FileInfo> compressed = new List<FileInfo>();
        private List<FileInfo> trayitems = new List<FileInfo>();

        private List<AllFiles> notpack = new List<AllFiles>();
        private List<FileInfo> files = new List<FileInfo>(); 
        private List<AllFiles> allfiles = new List<AllFiles>();
        private List<PackageFile> packagefiles = new List<PackageFile>();

        private CancellationTokenSource cts = new CancellationTokenSource();    
        private BackgroundWorker packagereader = new BackgroundWorker();
        private ConcurrentBag<BackgroundWorker> backgroundWorkers = new ConcurrentBag<BackgroundWorker>();
        private int completedreads = 0;
        private int runningthreads = 0;
        private int workerthreads = 0;
        private int iothreads = 0;
        private bool _runthreads = true;
        private int databaseBatchSize = 100;
        private int packageReaderBatchSize = 100;

        private bool CurrentlyMovingAP = false;
        private bool CurrentlyMovingRP = false;
        private bool CurrentlyMovingPR = false;
        private bool CurrentlyMovingAF = false;
        private bool CurrentlyMovingIRS2 = false;
        private bool CurrentlyMovingIRS3 = false;
        private bool CurrentlyMovingIRS4 = false;
        private bool CurrentlyMovingIMS2 = false;
        private bool CurrentlyMovingIMS3 = false;
        private bool CurrentlyMovingIMS4 = false;

        private bool Processing = false;

        public static int maxi = 0;
        
        
       
        public MainWindow()
        {           
            InitializeComponent();
            
            new Thread(() => StartIt()) {IsBackground = true}.Start();
        }

        private void StartIt(){
            
            ThreadPool.GetMaxThreads(out int workerThreadsCount, out int ioThreadsCount);
            workerthreads = workerThreadsCount;
            iothreads = ioThreadsCount;
            eatenturecpu = GlobalVariables.eatentirecpu;


            if (eatenturecpu == true){
                threadstouse = iothreads - 2;
            } else {
                threadstouse = (iothreads - 2) / 2;
            }
            parallelSettings.MaxDegreeOfParallelism = threadstouse;
                        
            if (GlobalVariables.debugMode) {
                Dispatcher.Invoke(new Action(() => testButton.Visibility = Visibility.Visible));
            } else {
                Dispatcher.Invoke(new Action(() => testButton.Visibility = Visibility.Hidden));
            }            
            if (dataExists == true) 
            {
                Dispatcher.Invoke(new Action(() => StartOverButton.Visibility = Visibility.Visible));
                Dispatcher.Invoke(new Action(() => LoadButton.Visibility = Visibility.Visible));
                Dispatcher.Invoke(new Action(() => NewFolder.Content = "Find New Items"));
            } else {
                Dispatcher.Invoke(new Action(() => StartOverButton.Visibility = Visibility.Collapsed));
                Dispatcher.Invoke(new Action(() => LoadButton.Visibility = Visibility.Collapsed));
                Dispatcher.Invoke(new Action(() => NewFolder.Content = "Find Content"));
            }
        }   

        private void OptionsMenu_Click(object sender, RoutedEventArgs e){
            Sims2Folder.Text = GlobalVariables.Sims2DocumentsFolder;
            Sims3Folder.Text = GlobalVariables.Sims3DocumentsFolder;
            Sims4Folder.Text = GlobalVariables.Sims4DocumentsFolder;
            OptionsMenuGrid.Visibility = Visibility.Visible;
        }  
         
        private void CloseOptionsMenu_Click(object sender, RoutedEventArgs e){
            OptionsMenuGrid.Visibility = Visibility.Hidden;
        }

        public void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        public void CloseWindow_Click(object sender, EventArgs e){
            this.Close();
        }

        private void CustomizeSortingOptions_Click(object sender, RoutedEventArgs e){
            SortingOptionsWindow sow = new();
            filesSort.InitializeSortingRules();
            Dispatcher.Invoke(new Action(() => sow.Show()));
        }

        private void browseS2Location_Click(object sender, EventArgs e) {
            using(var GetFolder = new FolderBrowserDialog())
            {
                DialogResult result = GetFolder.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK) {
                    SelectedFolder = GetFolder.SelectedPath;
                    GlobalVariables.Sims2DocumentsFolder = SelectedFolder;
                    Sims2Folder.Text = GlobalVariables.Sims2DocumentsFolder;
                    var s2 = GlobalVariables.Settings.Where(x => x.SettingName == "Sims2Folder").ToList();
                    if (s2.Any()){
                        GlobalVariables.Settings.Remove(s2[0]);
                    }
                    GlobalVariables.Settings.Add(new Setting(){SettingName = "Sims2Folder", SettingValue = GlobalVariables.Sims2DocumentsFolder});                    
                    globalVars.SaveSettings();
                }
            }
        }
        private void browseS3Location_Click(object sender, EventArgs e) {
            using(var GetFolder = new FolderBrowserDialog())
            {
                DialogResult result = GetFolder.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK) {
                    SelectedFolder = GetFolder.SelectedPath;
                    GlobalVariables.Sims3DocumentsFolder = SelectedFolder;
                    Sims3Folder.Text = GlobalVariables.Sims3DocumentsFolder;
                    var s3 = GlobalVariables.Settings.Where(x => x.SettingName == "Sims3Folder").ToList();
                    if (s3.Any()){
                        GlobalVariables.Settings.Remove(s3[0]);
                    }
                    GlobalVariables.Settings.Add(new Setting(){SettingName = "Sims3Folder", SettingValue = GlobalVariables.Sims3DocumentsFolder});                    
                    globalVars.SaveSettings();
                }
            }
        }
        private void browseS4Location_Click(object sender, EventArgs e) {
            using(var GetFolder = new FolderBrowserDialog())
            {
                DialogResult result = GetFolder.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK) {
                    SelectedFolder = GetFolder.SelectedPath;
                    GlobalVariables.Sims4DocumentsFolder = SelectedFolder;
                    Sims4Folder.Text = GlobalVariables.Sims4DocumentsFolder;
                    var s4 = GlobalVariables.Settings.Where(x => x.SettingName == "Sims4Folder").ToList();
                    if (s4.Any()){
                        GlobalVariables.Settings.Remove(s4[0]);
                    }
                    GlobalVariables.Settings.Add(new Setting(){SettingName = "Sims4Folder", SettingValue = GlobalVariables.Sims4DocumentsFolder});                    
                    globalVars.SaveSettings();
                }
            }
        }

        
        private void RestrictCPU_Check(object sender, EventArgs e) {
            eatenturecpu = false;
            GlobalVariables.Settings.Add(new Setting(){SettingName = "RestrictCPU", SettingValue = "True"});
            globalVars.SaveSettings();
        }
        
        private void RestrictCPU_Uncheck(object sender, EventArgs e) {
            eatenturecpu = true;
            GlobalVariables.Settings.Add(new Setting(){SettingName = "RestrictCPU", SettingValue = "False"});
            globalVars.SaveSettings();
        }

        







        
        private void App_Loaded(object sender, RoutedEventArgs e){
             
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        #region Load 

        private void loadData_Click(object sender, RoutedEventArgs e){            
            new Thread(() => GetResults()) {IsBackground = true}.Start();
        }

        private bool DetectHalfRun(){
            bool value = false;
            if (!File.Exists(GlobalVariables.PackagesRead)){
                value = false;
            } else {
                FileInfo pr = new FileInfo(GlobalVariables.PackagesRead);
                log.MakeLog(string.Format("Cache size: {0}", pr.Length), true);
                if (pr.Length != 0){                    
                    var packagesQuery = GlobalVariables.DatabaseConnection.Query<PackageFile>("SELECT * FROM Processing_Reader");
                    if (packagesQuery.Count > 0){
                        value = true;
                    }
                }                
            }
            return value;
        }

        #endregion   

        #region Buttons and Dials

        private void noeatcpu_Check(object sender, RoutedEventArgs e){
            eatenturecpu = false;
            threadstouse = (workerthreads - 2) / 2;
            parallelSettings.MaxDegreeOfParallelism = threadstouse;
            log.MakeLog(string.Format("Threads set to {0}", threadstouse), true);
        }
        private void eatcpu_Uncheck(object sender, RoutedEventArgs e){
            eatenturecpu = true;
            threadstouse = workerthreads - 2;
            parallelSettings.MaxDegreeOfParallelism = threadstouse;
            log.MakeLog(string.Format("Threads set to {0}", threadstouse), true);
        }  
        private void sortonthego_Check(object sender, RoutedEventArgs e){
            GlobalVariables.sortonthego = true;
            log.MakeLog("Sorting on the go!", true);
        }
        private void sortonthego_Uncheck(object sender, RoutedEventArgs e){
            GlobalVariables.sortonthego = false;
            log.MakeLog("No longer planning to sort on the go.", true);
        }  

        private void Kofi_Click(object sender, EventArgs e){
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
        private void Git_Click(object sender, EventArgs e){
            if (System.Windows.Forms.MessageBox.Show
            ("Open the Sims CC Manager Github Repo?", "Opening External URL",
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
        private void Twitter_Click(object sender, EventArgs e){
            if (System.Windows.Forms.MessageBox.Show
            ("Open SinfulSimming's Twitter?", "Opening External URL",
            System.Windows.Forms.MessageBoxButtons.YesNo, 
            System.Windows.Forms.MessageBoxIcon.Question)
            ==System.Windows.Forms.DialogResult.Yes)
                {
                    Process.Start(new ProcessStartInfo("https://twitter.com/sinfulsimming") { UseShellExecute = true });
                }

            else
                {
                //React as needed.
                }
        }
        private void Discord_Click(object sender, EventArgs e){
            if (System.Windows.Forms.MessageBox.Show
            ("Open Discord?", "Opening External URL",
            System.Windows.Forms.MessageBoxButtons.YesNo, 
            System.Windows.Forms.MessageBoxIcon.Question)
            ==System.Windows.Forms.DialogResult.Yes)
                {
                    Process.Start(new ProcessStartInfo("https://discord.gg/M6vnf842Fp") { UseShellExecute = true });
                    ReportABugGrid.Visibility = Visibility.Hidden;  
                }

            else
                {
                //React as needed.
                }
                          
        }

        private void ReportBug_Click(object sender, EventArgs e){
            ReportABugGrid.Visibility = Visibility.Visible;
        }

        private void CancelIssue_Click(object sender, EventArgs e){
            ReportABugGrid.Visibility = Visibility.Hidden;
        }

        private void GitIssues_Click(object sender, EventArgs e){
            if (System.Windows.Forms.MessageBox.Show
            ("Open the Sims CC Manager Github Repo Issue Tracker?", "Opening External URL",
            System.Windows.Forms.MessageBoxButtons.YesNo, 
            System.Windows.Forms.MessageBoxIcon.Question)
            ==System.Windows.Forms.DialogResult.Yes)
                {
                    Process.Start(new ProcessStartInfo("https://github.com/sixstepsaway/Sims-CC-Manager/issues") { UseShellExecute = true });
                    ReportABugGrid.Visibility = Visibility.Hidden;
                }

            else
                {
                //React as needed.
                }
        }


        private void CancelScan_Click(object sender, EventArgs e) {
            stop = true;
            cts.Cancel();
            Dispatcher.Invoke(new Action(() => CancelButton.Background = Brushes.LightGray));
            runprogress = false;
            hascancelled = true;
            _runthreads = false;
            Dispatcher.Invoke(new Action(() => Progressing.Visibility = Visibility.Hidden ));
            Dispatcher.Invoke(new Action(() => Cancelling.Visibility = Visibility.Visible ));
            Task ct = Task.Run(() => {
                CancellingTick();
            });
            ct.Wait();
            ct.Dispose();
            // Cancellation should have happened, so call Dispose.
            HideProgressGrid();            
        }

        private async void CancellingTick(){
            while (fullystopped == false){                
                Task one = Task.Run(() => {
                    Dispatcher.Invoke(new Action(() => CancelLabel.Content = "Cancelling, please wait."));
                    Thread.Sleep(15);
                });
                one.Wait();
                one.Dispose();
                Task two = Task.Run(() => {
                    Dispatcher.Invoke(new Action(() => CancelLabel.Content = "Cancelling, please wait.."));
                    Thread.Sleep(15);
                });
                two.Wait();
                two.Dispose();
                Task three = Task.Run(() => {
                    Dispatcher.Invoke(new Action(() => CancelLabel.Content = "Cancelling, please wait..."));
                    Thread.Sleep(15);
                });
                three.Wait();
                three.Dispose();
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            statement = "Closing application.";
            log.MakeLog(statement, false);
            GlobalVariables.InstancesCacheConnection.Commit();
            GlobalVariables.InstancesCacheConnection.Close();
            GlobalVariables.S4FunctionTypesConnection.Commit();
            GlobalVariables.S4FunctionTypesConnection.Close();
            GlobalVariables.S4OverridesConnection.Commit();
            GlobalVariables.S4OverridesConnection.Close();
            GlobalVariables.S4SpecificOverridesConnection.Commit();
            GlobalVariables.S4SpecificOverridesConnection.Close();
            GlobalVariables.DatabaseConnection.Commit();
            GlobalVariables.DatabaseConnection.Close();
            System.Windows.Application.Current.Shutdown();
        }

        public void LocationBoxValue(string value) {
            Dispatcher.Invoke(new Action(() => PickedLocation.Text = value));
        }

        public void completionAlertValue(string value) {
            Dispatcher.Invoke(new Action(() => completionAlert.Text = value));
        }

        private void StartOver_Click(object sender, EventArgs e) {
            if (SelectedFolder == "") {
                System.Windows.Forms.MessageBox.Show("Please select the folder containing your package files.");
            } else {
                log.MakeLog("Starting over.", true);
                FindNewAdditions = false;
                new Thread(() => SortNewPrep()) {IsBackground = true}.Start();    
            }            
        }

        private void FindNewItems_Click(object sender, EventArgs e) {            
            if (SelectedFolder == "") {
                System.Windows.Forms.MessageBox.Show("Please select the folder containing your package files.");
            } else {
                log.MakeLog("Sorting new folder.", true);
                if (dataExists == true){
                    FindNewAdditions = true;
                    Dispatcher.Invoke(new Action(() => NewFolder.IsEnabled = false));
                    new Thread(() => SortNewFolder(cts.Token)) {IsBackground = true}.Start();
                } else {
                    FindNewAdditions = false;
                    Dispatcher.Invoke(new Action(() => NewFolder.IsEnabled = false));
                    new Thread(() => SortNewPrep()) {IsBackground = true}.Start();
                }                
                               
            }   
        }

        private void SetProgressBar(){
            log.MakeLog("Setting progress bar!", true);
            log.MakeLog("Setting progress bar value to 0", true);
            Dispatcher.Invoke(new Action(() => mainProgressBar.Value = 0));
            log.MakeLog("Setting progress bar max to maxi.", true);
            Dispatcher.Invoke(new Action(() => mainProgressBar.Maximum = maxi));
            log.MakeLog("Setting current text pk visibility to visible.", true);
            Dispatcher.Invoke(new Action(() => textCurrentPk.Visibility = Visibility.Visible));            
        }

        //hi note to self: try adding thisPackage into an observable collection (maybe one reflecting a list or something idk) and then add that list periodically to the db from one place only?

        private void SetProgressBarMax(){
             Dispatcher.Invoke(new Action(() => mainProgressBar.Value = maxi));
        }

        private void ShowProgressGrid(){
            Dispatcher.Invoke(new Action(() => Progressing.Visibility = Visibility.Visible));
            try {
                Dispatcher.Invoke(new Action(() => ProgressGrid.Visibility = Visibility.Visible));
                Dispatcher.Invoke(new Action(() => NewFolder.IsEnabled = true));
            } catch (Exception e) {
                Console.WriteLine("Show Progress Grid Failed: " + e.Message);
            }

            try {
                Dispatcher.Invoke(new Action(() => MainMenuGrid.Visibility = Visibility.Hidden));
                Dispatcher.Invoke(new Action(() => NewFolder.IsEnabled = true));
            } catch (Exception e) {
                Console.WriteLine("Hide Main Menu Grid Failed: " + e.Message);
            }
            try {
                Dispatcher.Invoke(new Action(() => completionAlert.Visibility = Visibility.Visible)); 
                Dispatcher.Invoke(new Action(() => NewFolder.IsEnabled = true));
            } catch (Exception e) {
                Console.WriteLine("Show Completion Alert Failed: " + e.Message);
            } 

            try {
                Dispatcher.Invoke(new Action(() => mainProgressBar.Visibility = Visibility.Visible));
                Dispatcher.Invoke(new Action(() => NewFolder.IsEnabled = true));
            } catch (Exception e) {
                Console.WriteLine("Show Main Progress Bar Failed: " + e.Message);
            }
        }

        private void HideProgressGrid(){
            Dispatcher.Invoke(new Action(() => ProgressGrid.Visibility = Visibility.Hidden));
            Dispatcher.Invoke(new Action(() => Progressing.Visibility = Visibility.Hidden));
            Dispatcher.Invoke(new Action(() => MainMenuGrid.Visibility = Visibility.Visible));            
            Dispatcher.Invoke(new Action(() => Cancelling.Visibility = Visibility.Hidden ));
        }

        private void SetTextCurrentPkText(string txt){
            Dispatcher.Invoke(new Action(() => textCurrentPk.Text = ""));
        }

        private void browseLocation_Click(object sender, EventArgs e) {
            using(var GetFolder = new FolderBrowserDialog())
            {
                DialogResult result = GetFolder.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK) {
                    SelectedFolder = GetFolder.SelectedPath;
                    GlobalVariables.ModFolder = SelectedFolder;
                    GlobalVariables.logfile = System.IO.Path.Combine(SelectedFolder, "SimsCCSorter.log");
                    LocationBoxValue(GlobalVariables.ModFolder);
                    GlobalVariables.Initialize(SelectedFolder);
                    log.InitializeLog();
                    log.MakeLog(string.Format("Application initiated. ModFolder found at {0}", GlobalVariables.ModFolder), true); 
                    if (GlobalVariables.debugMode) {
                        statement = "Application running in debug mode.";
                        log.MakeLog(statement, true);
                    } else {
                        statement = "Application is not running in debug mode.";
                        log.MakeLog(statement, true);
                    }
                } else {
                    LocationBoxValue(SelectedFolder);
                }
            }
        }

        #endregion

        #region Sort New


        private void SortNewPrep(){
            log.MakeLog("Prepping to sort folder.", true);            

            GlobalVariables.DatabaseConnection.Close();
            try{
                globalVars.ConnectDatabase(true);
            } catch (Exception e){
                System.Windows.Forms.MessageBox.Show(string.Format("Exception caught connecting database: {0}", e));
            }
            
            log.MakeLog("Creating database.", true);

            

            var command = GlobalVariables.DatabaseConnection.CreateCommand("CREATE TABLE AllFiles (Type TEXT, Location TEXT, Name TEXT, Status TEXT)");
            command.ExecuteNonQuery();
            GlobalVariables.DatabaseConnection.CreateTable <PackageFile>();
            log.MakeLog("Making SimsPackages table", true);
            try {
                GlobalVariables.DatabaseConnection.CreateTable <SimsPackage>();
            } catch (Exception e){
                log.MakeLog(string.Format("Ran into an error making SimsPackages table: {0}", e), true);
            }            
            
            log.MakeLog("Making AgeGenderFlags table", true);  
            try {
                GlobalVariables.DatabaseConnection.CreateTable <AgeGenderFlags>();              
            } catch (Exception e){
                log.MakeLog(string.Format("Ran into an error making AgeGenderFlags table: {0}", e), true);
            }            
            log.MakeLog("Making TypeCounter table", true);
            try {
                GlobalVariables.DatabaseConnection.CreateTable <TypeCounter>();            
            } catch (Exception e){
                log.MakeLog(string.Format("Ran into an error making TypeCounter table: {0}", e), true);
            }            
            log.MakeLog("Making Tagslist table", true);
            try {
                GlobalVariables.DatabaseConnection.CreateTable <TagsList>();
            } catch (Exception e){
                log.MakeLog(string.Format("Ran into an error making Tagslist table: {0}", e), true);
            }
            log.MakeLog("Making FileHasList table", true);
            try {
                GlobalVariables.DatabaseConnection.CreateTable <fileHasList>();
            } catch (Exception e){
                log.MakeLog(string.Format("Ran into an error making File Has table: {0}", e), true);
            }              
            log.MakeLog("Making Overridden table", true);
            try {
                GlobalVariables.DatabaseConnection.CreateTable <OverriddenList>();
            } catch (Exception e){
                log.MakeLog(string.Format("Ran into an error making Overridden List table: {0}", e), true);
            } 

            string making = "Package Instance";
            log.MakeLog(string.Format("Making {0} table", making), true);
            try {
                GlobalVariables.DatabaseConnection.CreateTable <PackageInstance>();
            } catch (Exception e){
                log.MakeLog(string.Format("Ran into an error making {0} table: {1}", making, e), true);
            }

            making = "Package GUID";
            log.MakeLog(string.Format("Making {0} table", making), true);
            try {
                GlobalVariables.DatabaseConnection.CreateTable <PackageGUID>();
            } catch (Exception e){
                log.MakeLog(string.Format("Ran into an error making {0} table: {1}", making, e), true);
            }

            making = "Required EPs";
            log.MakeLog(string.Format("Making {0} table", making), true);
            try {
                GlobalVariables.DatabaseConnection.CreateTable <PackageRequiredEPs>();
            } catch (Exception e){
                log.MakeLog(string.Format("Ran into an error making {0} table: {1}", making, e), true);
            }

            making = "Mesh Keys";
            log.MakeLog(string.Format("Making {0} table", making), true);
            try {
                GlobalVariables.DatabaseConnection.CreateTable <PackageMeshKeys>();
            } catch (Exception e){
                log.MakeLog(string.Format("Ran into an error making {0} table: {1}", making, e), true);
            }

            making = "CAS Part Keys";
            log.MakeLog(string.Format("Making {0} table", making), true);
            try {
                GlobalVariables.DatabaseConnection.CreateTable <PackageCASPartKeys>();
            } catch (Exception e){
                log.MakeLog(string.Format("Ran into an error making {0} table: {1}", making, e), true);
            }

            making = "Matching Recolors";
            log.MakeLog(string.Format("Making {0} table", making), true);
            try {
                GlobalVariables.DatabaseConnection.CreateTable <PackageMatchingRecolors>();
            } catch (Exception e){
                log.MakeLog(string.Format("Ran into an error making {0} table: {1}", making, e), true);
            }

            making = "Conflicts";
            log.MakeLog(string.Format("Making {0} table", making), true);
            try {
                GlobalVariables.DatabaseConnection.CreateTable <PackageConflicts>();
            } catch (Exception e){
                log.MakeLog(string.Format("Ran into an error making {0} table: {1}", making, e), true);
            }

            making = "Duplicates";
            log.MakeLog(string.Format("Making {0} table", making), true);
            try {
                GlobalVariables.DatabaseConnection.CreateTable <PackageDuplicates>();
            } catch (Exception e){
                log.MakeLog(string.Format("Ran into an error making {0} table: {1}", making, e), true);
            }

            making = "Room Sort";
            log.MakeLog(string.Format("Making {0} table", making), true);
            try {
                GlobalVariables.DatabaseConnection.CreateTable <PackageRoomSort>();
            } catch (Exception e){
                log.MakeLog(string.Format("Ran into an error making {0} table: {1}", making, e), true);
            }

            making = "Package Components";
            log.MakeLog(string.Format("Making {0} table", making), true);
            try {
                GlobalVariables.DatabaseConnection.CreateTable <PackageComponent>();
            } catch (Exception e){
                log.MakeLog(string.Format("Ran into an error making {0} table: {1}", making, e), true);
            }
            making = "Flags";
            log.MakeLog(string.Format("Making {0} table", making), true);
            try {
                GlobalVariables.DatabaseConnection.CreateTable <PackageFlag>();
            } catch (Exception e){
                log.MakeLog(string.Format("Ran into an error making {0} table: {1}", making, e), true);
            }
            making = "OBJD Keys";
            log.MakeLog(string.Format("Making {0} table", making), true);
            try {
                GlobalVariables.DatabaseConnection.CreateTable <PackageOBJDKeys>();
            } catch (Exception e){
                log.MakeLog(string.Format("Ran into an error making {0} table: {1}", making, e), true);
            }
            making = "Thumbnails";
            log.MakeLog(string.Format("Making {0} table", making), true);
            try {
                GlobalVariables.DatabaseConnection.CreateTable <PackageThumbnail>();
            } catch (Exception e){
                log.MakeLog(string.Format("Ran into an error making {0} table: {1}", making, e), true);
            }

            
             
            var pragmas = new List<string>(){
                "PRAGMA journal_mode=WAL",
                "PRAGMA synchronous=OFF",
                "PRAGMA foreign_keys = ON",
                "PRAGMA auto_vacuum=FULL",
                "PRAGMA journal_size_limit=5000",
                "PRAGMA default_cache_size=200"
            };
            foreach (string pragma in pragmas){
                log.MakeLog(string.Format("Executing: {0}", pragma), true);
                var result=GlobalVariables.DatabaseConnection.ExecuteScalar<string>(pragma);
            }   
            
            new Thread(() => SortNewFolder(cts.Token)) {IsBackground = true}.Start();
            
        }

        private void SortNewFolder(object obj){
            CancellationToken token = (CancellationToken)obj;  
            parallelSettings.CancellationToken = token; 
            
            sw.Start();
            ShowProgressGrid();
            if (FindNewAdditions == true){
                if (hascancelled == true){
                    GlobalVariables.DatabaseConnection.Close();
                }
                globalVars.ConnectDatabase(false);
            }            
            if(!token.IsCancellationRequested){
                Task ff = Task.Run(() => {
                    FindFiles(token);
                }, token);
                ff.Wait(token);
                ff.Dispose();
                log.MakeLog("Finished finding files.", true);
            
            }

            if(!token.IsCancellationRequested){
                sw.Stop();
                Task stopwatch = Task.Run(() => {
                    ElapsedProcessing("Categorizing files"); 
                }, token);
                stopwatch.Wait(token);
                stopwatch.Dispose();
            }
            if(!token.IsCancellationRequested){
                sw.Restart();                        
                Task countdata = Task.Run(() => {
                    CountDatabase(token);
                }, token);
                countdata.Wait(token);  
                countdata.Dispose(); 
                List<AllFiles> allp = new();
                Task getPackages = Task.Run(() => {
                    allp = GlobalVariables.DatabaseConnection.Query<AllFiles>("SELECT * FROM AllFiles where Type='package'");
                    log.MakeLog(string.Format("Packages to read count is {0}.", allp.Count), true);
                    maxi = allp.Count;
                    GlobalVariables.PackageCount = allp.Count;
                    log.MakeLog(string.Format("Packages to read count is {0}.", allp.Count), true);
                    SetProgressBar();
                    completionAlertValue("Reading packages.");
                    countprogress = 0;
                    runprogress = true;            
                });
                getPackages.Wait(token);
                getPackages.Dispose();

                new Thread(() => RunUpdateElapsed(sw, token)) {IsBackground = true}.Start();
                new Thread(() => RunUpdateProgressBar(token)) {IsBackground = true}.Start();

                Task rp = Task.Run(() => {
                    ReadPackagesPFE(allp);
                }, token);
                rp.Wait(token);
                rp.Dispose();

                runprogress = false;
                List<SimsPackage> allPackages4 = new();
                
                Task matchPrep = Task.Run(() => { 
                    allPackages4 = GlobalVariables.DatabaseConnection.Query<SimsPackage>("SELECT * FROM Packages where Game=4");
                    maxi = allPackages4.Count;
                    GlobalVariables.PackageCount = allPackages4.Count;
                    SetProgressBar();
                    completionAlertValue("Identifying orphans.");
                    countprogress = 0;
                }, token);
                matchPrep.Wait(token);
                matchPrep.Dispose();

                new Thread(() => RunUpdateElapsed(sw, token)) {IsBackground = true}.Start();
                new Thread(() => RunUpdateProgressBar(token)) {IsBackground = true}.Start();

                Task findMatches = Task.Run(() => { 
                    IdentifyOrphans(allPackages4);
                }, token);
                findMatches.Wait(token);
                findMatches.Dispose();
                Processing = false;

                string elapsedtime = "";
                
                sw.Stop();
                TimeSpan ts = sw.Elapsed;
                if (ts.Days != 0){
                    elapsedtime = String.Format("{4} days, {0:00}:{1:00}:{2:00}.{3:00}",
                                    ts.Hours, ts.Minutes, ts.Seconds,
                                    ts.Milliseconds / 10, ts.Days);
                } else {
                    elapsedtime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                    ts.Hours, ts.Minutes, ts.Seconds,
                                    ts.Milliseconds / 10);
                }
                
                
                complete = true;  

                if (complete == true){
                    runprogress = false;
                }
                log.MakeLog(string.Format("Reading {0} packages took {1}", maxi, elapsedtime), true);
               
                //updatesw.Dispose();
                //new Thread(() => ) {IsBackground = true}.Start();
            }   
            
            if (!token.IsCancellationRequested) {
                Task CheckCollections = Task.Run(() => {
                    CheckObservableCollections();
                });
                CheckCollections.Wait();    
            }
                               

            if (!token.IsCancellationRequested) {
                sw.Stop();
                completionAlertValue("Done!");
                SetTextCurrentPkText("");
                SetProgressBarMax();
                ElapsedProcessing("identifying orphans");
                GetResults();
            } else {
                cts.Dispose();
                stop = false;
                fullystopped = true;
                runprogress = true;
                log.MakeLog("Fully cancelled.", true);
            }
        }

        #endregion

        #region Methods of Processing

        private async void IdentifyOrphans(List<SimsPackage> packages){
            Task doMath1 = Task.Run(async () => {
                if (packages.Count > 100000){
                    databaseBatchSize = (packages.Count / 1000) / 2;
                } else if (packages.Count > 10000){
                    databaseBatchSize = (packages.Count / 100) / 2;
                } else if (packages.Count > 1000){
                    databaseBatchSize = (packages.Count / 10) / 2;
                } else {
                    databaseBatchSize = 25;
                }

                if (packages.Count > 100000){
                    packageReaderBatchSize = (packages.Count / 1000) / 2;
                } else if (packages.Count > 10000){
                    packageReaderBatchSize = (packages.Count / 100) / 2;
                } else if (packages.Count > 1000){
                    packageReaderBatchSize = (packages.Count / 10) / 2;
                } else {
                    packageReaderBatchSize = 25;
                }
            });
            doMath1.Wait();

            int packageBatchesLow = 0;
            int packageBatchesHigh = 0;
            int filesReadNoOverflow = 0;
            int packageOverflow = 0;
            
            Task doMath2 = Task.Run(async () => {
                double packageBatchesMath = (double)packages.Count / packageReaderBatchSize;
            
                packageBatchesLow = (int)packageBatchesMath;
                packageBatchesHigh = (int)Math.Ceiling(packageBatchesMath);
                filesReadNoOverflow = packageBatchesLow * packageReaderBatchSize;
                packageOverflow = packages.Count - filesReadNoOverflow;
                log.MakeLog(string.Format("There will be {0} batches of packages.", packageBatchesHigh), true);
                log.MakeLog(string.Format("The final batch will contain {0} files.", packageOverflow), true);
                log.MakeLog(string.Format("Making batches list."), true);
            });
            doMath2.Wait();
            
            

            var PackageBatches = new List<List<SimsPackage>>();
                    
            for (int i = 0; i < packageBatchesHigh; i++)
            {
                PackageBatches.Add(packages.Skip(i * packageReaderBatchSize).Take(packageReaderBatchSize).ToList());
            }

            log.MakeLog("Listing batches.", true);            
            log.MakeLog(string.Format("There are {0} batches to run.", PackageBatches.Count), true);

            List<SimsPackage> list1 = new();
            List<PackageFile> list2 = new();
            List<PackageFile> list3 = new();
            List<AllFiles> list4 = new();
            List<InstancesRecolorsS2> list5 = new();
            List<InstancesRecolorsS3> list6 = new();
            List<InstancesRecolorsS4> list7 = new();
            List<InstancesMeshesS2> list8 = new();
            List<InstancesMeshesS3> list9 = new();
            List<InstancesMeshesS4> list10 = new();
            List<PackageThumbnail> list11 = new();
            int batchnum = -1;
            int totalitems = -1;
            foreach (var batch in PackageBatches){
                batchnum++;
                Task reader = Task.Run(() => {
                    int itemnum = -1;
                    Parallel.ForEach(batch, parallelSettings, p => {
                        itemnum++;
                        totalitems++;
                        log.MakeLog(string.Format("Processing item {0} of batch {1}. Total items processed: {2}/{3}.", itemnum, batchnum, totalitems, packages.Count), true);
                        if (p.Game == 4){
                            orphanhunt.FindMatchesS4(p);
                        }                      
                    });
                });
                reader.Wait();

                Task UpdateDatabase = Task.Run(() => {
                    log.MakeLog(string.Format("Getting items produced by batch {0}.", batchnum), true);
                    list1 = GlobalVariables.AddPackages.ToList();                    
                    GlobalVariables.AddPackages.Clear();
                    log.MakeLog(string.Format("Batch {0}: AddPackages cleared.", batchnum), true);
                    list2 = GlobalVariables.RemovePackages.ToList();
                    GlobalVariables.RemovePackages.Clear();
                    log.MakeLog(string.Format("Batch {0}: RemovePackages cleared.", batchnum), true);
                    list3 = GlobalVariables.ProcessingReader.ToList();
                    GlobalVariables.ProcessingReader.Clear();                   
                    log.MakeLog(string.Format("Batch {0}: ProcessingReader cleared.", batchnum), true);
                    list4 = GlobalVariables.AllFiles.ToList();
                    GlobalVariables.AllFiles.Clear();
                    log.MakeLog(string.Format("Batch {0}: AllFiles cleared.", batchnum), true);
                    list5 = GlobalVariables.InstancesRecolorsS2Col.ToList();
                    GlobalVariables.InstancesRecolorsS2Col.Clear();
                    log.MakeLog(string.Format("Batch {0}: InstancesRecolorsS2 cleared.", batchnum), true);
                    list6 = GlobalVariables.InstancesRecolorsS3Col.ToList();
                    GlobalVariables.InstancesRecolorsS3Col.Clear();
                    log.MakeLog(string.Format("Batch {0}: InstancesRecolorsS3 cleared.", batchnum), true);
                    list7 = GlobalVariables.InstancesRecolorsS4Col.ToList();
                    GlobalVariables.InstancesRecolorsS4Col.Clear();
                    log.MakeLog(string.Format("Batch {0}: InstancesRecolorsS4 cleared.", batchnum), true);
                    list8 = GlobalVariables.InstancesMeshesS2Col.ToList();
                    GlobalVariables.InstancesMeshesS2Col.Clear();
                    log.MakeLog(string.Format("Batch {0}: InstancesMeshesS2 cleared.", batchnum), true);
                    list9 = GlobalVariables.InstancesMeshesS3Col.ToList();
                    GlobalVariables.InstancesMeshesS3Col.Clear();
                    log.MakeLog(string.Format("Batch {0}: InstancesMeshesS3 cleared.", batchnum), true);
                    list10 = GlobalVariables.InstancesMeshesS4Col.ToList();
                    GlobalVariables.InstancesMeshesS4Col.Clear();
                    log.MakeLog(string.Format("Batch {0}: InstancesMeshesS4 cleared.", batchnum), true);
                    log.MakeLog(string.Format("Batch {0}: Finished getting items.", batchnum), true);
                });                
                UpdateDatabase.Wait();
                log.MakeLog(string.Format("Sending items off for processing.", batchnum), true);
                UpdateDatabases(list1, list2, list3, list4, list5, list6, list7, list8, list9, list10, batchnum);
                list1 = new();
                list2 = new();
                list3 = new();
                list4 = new();
                list5 = new();
                list6 = new();
                list7 = new();
                list8 = new();
                list9 = new();
                list10 = new();                
            }         
        }
        
        
        private async void ReadPackagesPFE(List<AllFiles> allp){
            Task doMath1 = Task.Run(async () => {
                if (allp.Count > 100000){
                    databaseBatchSize = (allp.Count / 1000) / 2;
                } else if (allp.Count > 10000){
                    databaseBatchSize = (allp.Count / 100) / 2;
                } else if (allp.Count > 1000){
                    databaseBatchSize = (allp.Count / 10) / 2;
                } else {
                    databaseBatchSize = 25;
                }

                if (allp.Count > 100000){
                    packageReaderBatchSize = (allp.Count / 1000) / 2;
                } else if (allp.Count > 10000){
                    packageReaderBatchSize = (allp.Count / 100) / 2;
                } else if (allp.Count > 1000){
                    packageReaderBatchSize = (allp.Count / 10) / 2;
                } else {
                    packageReaderBatchSize = 25;
                }
            });
            doMath1.Wait();

            int packageBatchesLow = 0;
            int packageBatchesHigh = 0;
            int filesReadNoOverflow = 0;
            int packageOverflow = 0;
            
            Task doMath2 = Task.Run(async () => {
                double packageBatchesMath = (double)allp.Count / packageReaderBatchSize;
            
                packageBatchesLow = (int)packageBatchesMath;
                packageBatchesHigh = (int)Math.Ceiling(packageBatchesMath);
                filesReadNoOverflow = packageBatchesLow * packageReaderBatchSize;
                packageOverflow = allp.Count - filesReadNoOverflow;
                log.MakeLog(string.Format("There will be {0} batches of packages.", packageBatchesHigh), true);
                log.MakeLog(string.Format("The final batch will contain {0} files.", packageOverflow), true);
                log.MakeLog(string.Format("Making batches list."), true);
            });
            doMath2.Wait();
            
            

            var PackageBatches = new List<List<AllFiles>>();
                    
            for (int i = 0; i < packageBatchesHigh; i++)
            {
                PackageBatches.Add(allp.Skip(i * packageReaderBatchSize).Take(packageReaderBatchSize).ToList());
            }

            log.MakeLog("Listing batches.", true);            
            log.MakeLog(string.Format("There are {0} batches to run.", PackageBatches.Count), true);

            List<SimsPackage> list1 = new();
            List<PackageFile> list2 = new();
            List<PackageFile> list3 = new();
            List<AllFiles> list4 = new();
            List<InstancesRecolorsS2> list5 = new();
            List<InstancesRecolorsS3> list6 = new();
            List<InstancesRecolorsS4> list7 = new();
            List<InstancesMeshesS2> list8 = new();
            List<InstancesMeshesS3> list9 = new();
            List<InstancesMeshesS4> list10 = new();

            int batchnum = -1;
            int totalitems = -1;
            foreach (var batch in PackageBatches){
                batchnum++;
                Task reader = Task.Run(() => {
                    int itemnum = -1;
                    Parallel.ForEach(batch, parallelSettings, p => {
                        itemnum++;
                        totalitems++;
                        log.MakeLog(string.Format("Processing item {0} of batch {1}. Total items processed: {2}/{3}.", itemnum, batchnum, totalitems, allp.Count), true);
                        initialprocess.CheckThrough(p.Location);                        
                    });
                });
                reader.Wait();

                Task UpdateDatabase = Task.Run(() => {
                    log.MakeLog(string.Format("Getting items produced by batch {0}.", batchnum), true);
                    list1 = GlobalVariables.AddPackages.ToList();                    
                    GlobalVariables.AddPackages.Clear();
                    log.MakeLog(string.Format("Batch {0}: AddPackages cleared.", batchnum), true);
                    list2 = GlobalVariables.RemovePackages.ToList();
                    GlobalVariables.RemovePackages.Clear();
                    log.MakeLog(string.Format("Batch {0}: RemovePackages cleared.", batchnum), true);
                    list3 = GlobalVariables.ProcessingReader.ToList();
                    GlobalVariables.ProcessingReader.Clear();                   
                    log.MakeLog(string.Format("Batch {0}: ProcessingReader cleared.", batchnum), true);
                    list4 = GlobalVariables.AllFiles.ToList();
                    GlobalVariables.AllFiles.Clear();
                    log.MakeLog(string.Format("Batch {0}: AllFiles cleared.", batchnum), true);
                    list5 = GlobalVariables.InstancesRecolorsS2Col.ToList();
                    GlobalVariables.InstancesRecolorsS2Col.Clear();
                    log.MakeLog(string.Format("Batch {0}: InstancesRecolorsS2 cleared.", batchnum), true);
                    list6 = GlobalVariables.InstancesRecolorsS3Col.ToList();
                    GlobalVariables.InstancesRecolorsS3Col.Clear();
                    log.MakeLog(string.Format("Batch {0}: InstancesRecolorsS3 cleared.", batchnum), true);
                    list7 = GlobalVariables.InstancesRecolorsS4Col.ToList();
                    GlobalVariables.InstancesRecolorsS4Col.Clear();
                    log.MakeLog(string.Format("Batch {0}: InstancesRecolorsS4 cleared.", batchnum), true);
                    list8 = GlobalVariables.InstancesMeshesS2Col.ToList();
                    GlobalVariables.InstancesMeshesS2Col.Clear();
                    log.MakeLog(string.Format("Batch {0}: InstancesMeshesS2 cleared.", batchnum), true);
                    list9 = GlobalVariables.InstancesMeshesS3Col.ToList();
                    GlobalVariables.InstancesMeshesS3Col.Clear();
                    log.MakeLog(string.Format("Batch {0}: InstancesMeshesS3 cleared.", batchnum), true);
                    list10 = GlobalVariables.InstancesMeshesS4Col.ToList();
                    GlobalVariables.InstancesMeshesS4Col.Clear();
                    log.MakeLog(string.Format("Batch {0}: InstancesMeshesS4 cleared.", batchnum), true);
                    log.MakeLog(string.Format("Batch {0}: Finished getting items.", batchnum), true);
                });
                UpdateDatabase.Wait();
                log.MakeLog(string.Format("Sending items off for processing.", batchnum), true);
                UpdateDatabases(list1, list2, list3, list4, list5, list6, list7, list8, list9, list10, batchnum);
                list1 = new();
                list2 = new();
                list3 = new();
                list4 = new();
                list5 = new();
                list6 = new();
                list7 = new();
                list8 = new();
                list9 = new();
                list10 = new();                
            }         
        }

        private async Task UpdateDatabases(List<SimsPackage> list1, List<PackageFile> list2, List<PackageFile> list3, List<AllFiles> list4, List<InstancesRecolorsS2> list5, List<InstancesRecolorsS3> list6, List<InstancesRecolorsS4> list7, List<InstancesMeshesS2> list8, List<InstancesMeshesS3> list9, List<InstancesMeshesS4> list10, int batchnum){
                        
            GlobalVariables.DatabaseConnection.InsertAllWithChildren(list1.ApostropheFix(), recursive: true);
            foreach (SimsPackage package in list1.ApostropheFix()) {
                GlobalVariables.DatabaseConnection.UpdateWithChildren(package);
            }
            log.MakeLog(string.Format("Batch {0}: {1} Items in AddPackages added to Database.", batchnum, list1.Count), true);
            
            GlobalVariables.DatabaseConnection.DeleteAll(list2.ApostropheFix(), true);
            log.MakeLog(string.Format("Batch {0}: {1} Items in RemovePackages added to Database.", batchnum, list2.Count), true);

            GlobalVariables.DatabaseConnection.InsertOrReplaceAllWithChildren(list3.ApostropheFix(), true);
            log.MakeLog(string.Format("Batch {0}: {1} Items in processingReader added to Database.", batchnum, list3.Count), true);

            GlobalVariables.DatabaseConnection.InsertOrReplaceAllWithChildren(list4.ApostropheFix(), true);
            log.MakeLog(string.Format("Batch {0}: {1} Items in AllFiles added to Database.", batchnum, list4.Count), true);

            GlobalVariables.InstancesCacheConnection.InsertOrReplaceAllWithChildren(list5.ApostropheFix(), true);
            log.MakeLog(string.Format("Batch {0}: {1} Items in InstancesRecolorsS2 added to Database.", batchnum, list5.Count), true);

            GlobalVariables.InstancesCacheConnection.InsertOrReplaceAllWithChildren(list6.ApostropheFix(), true);
            log.MakeLog(string.Format("Batch {0}: {1} Items in InstancesRecolorsS3 added to Database.", batchnum, list6.Count), true);
            
            GlobalVariables.InstancesCacheConnection.InsertOrReplaceAllWithChildren(list7.ApostropheFix(), true);
            log.MakeLog(string.Format("Batch {0}: {1} Items in InstancesRecolorsS4 added to Database.", batchnum, list7.Count), true);
            
            GlobalVariables.InstancesCacheConnection.InsertOrReplaceAllWithChildren(list8.ApostropheFix(), true);
            log.MakeLog(string.Format("Batch {0}: {1} Items in InstancesMeshesS2 added to Database.", batchnum, list8.Count), true);
            
            GlobalVariables.InstancesCacheConnection.InsertOrReplaceAllWithChildren(list9.ApostropheFix(), true);
            log.MakeLog(string.Format("Batch {0}: {1} Items in InstancesMeshesS3 added to Database.", batchnum, list9.Count), true);
            
            GlobalVariables.InstancesCacheConnection.InsertOrReplaceAllWithChildren(list10.ApostropheFix(), true);
            log.MakeLog(string.Format("Batch {0}: {1} Items in InstancesMeshesS4 added to Database.", batchnum, list10.Count), true);
            
            
        }

        private async void CheckObservableCollections(){
            Task checker = Task.Run(() => {
                if (!GlobalVariables.AddPackages.IsEmpty){
                    GlobalVariables.DatabaseConnection.InsertOrReplaceAllWithChildren(GlobalVariables.AddPackages.ToList().ApostropheFix());
                }                
                if (!GlobalVariables.RemovePackages.IsEmpty){
                    GlobalVariables.DatabaseConnection.Delete(GlobalVariables.RemovePackages.ToList().ApostropheFix());
                }
                if (!GlobalVariables.ProcessingReader.IsEmpty){
                    GlobalVariables.DatabaseConnection.InsertOrReplaceAllWithChildren(GlobalVariables.ProcessingReader.ToList().ApostropheFix());
                }
                if (!GlobalVariables.AllFiles.IsEmpty){
                    GlobalVariables.DatabaseConnection.InsertOrReplaceAllWithChildren(GlobalVariables.AllFiles.ToList().ApostropheFix());
                }
                if (!GlobalVariables.InstancesRecolorsS2Col.IsEmpty){
                    GlobalVariables.InstancesCacheConnection.InsertOrReplaceAllWithChildren(GlobalVariables.InstancesRecolorsS2Col.ToList().ApostropheFix());
                }
                if (!GlobalVariables.InstancesRecolorsS3Col.IsEmpty){
                    GlobalVariables.InstancesCacheConnection.InsertOrReplaceAllWithChildren(GlobalVariables.InstancesRecolorsS3Col.ToList().ApostropheFix());
                }
                if (!GlobalVariables.InstancesRecolorsS4Col.IsEmpty){
                    GlobalVariables.InstancesCacheConnection.InsertOrReplaceAllWithChildren(GlobalVariables.InstancesRecolorsS4Col.ToList().ApostropheFix());
                }
                if (!GlobalVariables.InstancesMeshesS2Col.IsEmpty){
                    GlobalVariables.InstancesCacheConnection.InsertOrReplaceAllWithChildren(GlobalVariables.InstancesMeshesS2Col.ToList().ApostropheFix());
                }
                if (!GlobalVariables.InstancesMeshesS3Col.IsEmpty){
                    GlobalVariables.InstancesCacheConnection.InsertOrReplaceAllWithChildren(GlobalVariables.InstancesMeshesS3Col.ToList().ApostropheFix());
                }
                if (!GlobalVariables.InstancesMeshesS4Col.IsEmpty){
                    GlobalVariables.InstancesCacheConnection.InsertOrReplaceAllWithChildren(GlobalVariables.InstancesMeshesS4Col.ToList().ApostropheFix());
                }
            });
            checker.Wait();
        }



        
        private void FindOrphans(CancellationToken token){
            var packagefiles = GlobalVariables.DatabaseConnection.Query<SimsPackage>("SELECT * from Packages");
            maxi = packagefiles.Count;            
            SetProgressBar();
            completionAlertValue("Reading packages.");
            new Thread(() => RunUpdateElapsed(sw, token)) {IsBackground = true}.Start();
            new Thread(() => RunUpdateProgressBar(token)) {IsBackground = true}.Start();

            Task findOrphansPass1 = Task.Run(() => {
                foreach (SimsPackage package in packagefiles){
                    if (package.Game == 2){
                        orphanhunt.FindMatchesS2(package);
                    } else if (package.Game == 3){
                        //orphanhunt.FindMatchesS4(package);
                    } else if (package.Game == 4){
                        orphanhunt.FindMatchesS4(package);
                    }
                }
            }, token);
            findOrphansPass1.Wait();  
            Task findOrphansPass2 = Task.Run(() => {
                foreach (SimsPackage package in packagefiles){
                    if (package.Game == 2){
                        orphanhunt.FindMatchesS2(package);
                    } else if (package.Game == 3){
                        //orphanhunt.FindMatchesS4(package);
                    } else if (package.Game == 4){
                        orphanhunt.FindMatchesS4(package);
                    }
                }
            }, token);
            findOrphansPass2.Wait();            
        }


        private void FindFiles(CancellationToken token){
            var directory = new DirectoryInfo(GlobalVariables.ModFolder);
            var VilesS = directory.EnumerateFiles("*.*", SearchOption.AllDirectories).Where(file => !file.DirectoryName.Contains("_SORTED"));;
            List<FileInfo> ffiles = new();
            ffiles.AddRange(VilesS);
            string[] filesS = new string[ffiles.Count];
            for (int f = 0; f < ffiles.Count; f++){
                if(token.IsCancellationRequested) return;
                filesS[f] = ffiles[f].FullName;
            }
            if(token.IsCancellationRequested) return;
            Task task0 = Task.Run(() => {
                log.MakeLog("Sorting packages files from non-package files.", true);
                maxi = filesS.Length + 7;             
                log.MakeLog(string.Format("Maxi set to {0}.", maxi), true);                        
                log.MakeLog("Setting progress bar.", true);
                SetProgressBar();
                completionAlertValue("Sorting package files from non-package files.");
            }, token);
            task0.Wait(token);  

            if (FindNewAdditions == true){
                Task taskfna = Task.Run(() => {
                    //log.MakeLog("Getting packagesPending.", true);
                    //var packagesPendingV = GlobalVariables.DatabaseConnection.Query<PackageFile>("SELECT * FROM Processing_Reader where Status = 'Pending'");
                    //packagesPending.AddRange(packagesPendingV);
                    //log.MakeLog("Getting packagesProcessing.", true);
                    //var packagesProcessingV = GlobalVariables.DatabaseConnection.Query<PackageFile>("SELECT * FROM Processing_Reader where Status = 'Processing'");
                    //packagesProcessing.AddRange(packagesPendingV);
                    //log.MakeLog("Getting packagesDone.", true);
                    //var packagesDoneV = GlobalVariables.DatabaseConnection.Query<SimsPackage>("SELECT * FROM Packages");
                    //packagesDone.AddRange(packagesDoneV);
                    //log.MakeLog("Getting notpack.", true);
                    var notpackV = GlobalVariables.DatabaseConnection.Query<AllFiles>("SELECT * FROM AllFiles");
                    notpack.AddRange(notpackV);
                });
                taskfna.Wait(token);
                taskfna.Dispose();
            }                 
            if(token.IsCancellationRequested) return;

            Task task1 = Task.Run(() => {
                int c = 0;
                foreach (string file in filesS){   
                    if (token.IsCancellationRequested)
                    {
                        log.MakeLog("Process cancelled.", true);
                        stop = true;
                        return;
                    }
                    if (FindNewAdditions == true){
                        Task taskfna = Task.Run(() => {
                            var packagesPending = GlobalVariables.DatabaseConnection.Query<PackageFile>(string.Format("SELECT * FROM Processing_Reader where Location = '{0}'", file));
                            var packagesDone = GlobalVariables.DatabaseConnection.Query<SimsPackage>(string.Format("SELECT * FROM Packages where Location = '{0}'", file));
                            var notPack = GlobalVariables.DatabaseConnection.Query<AllFiles>(string.Format("SELECT * FROM AllFiles where Location = '{0}'", file));
                            if (packagesPending.Any()){
                                log.MakeLog(string.Format("File {1}: File {0} already exists in database and is awaiting processing.", file, c), true);
                            } else if (packagesDone.Any()){
                                log.MakeLog(string.Format("File {1}: File {0} already exists in database and has been read.", file, c), true);                                
                            } else if (notPack.Any()){
                                log.MakeLog(string.Format("File {1}: File {0} already exists in database isn't a package.", file, c), true);
                            } else {
                                files.Add(new FileInfo(file));
                                UpdateProgressBar(file, "Acquiring");
                                //log.MakeLog(string.Format("File {1}: Adding {0} to list.", file, c), true); 
                                //log.MakeLog(string.Format("'files' contains {0} items.", files.Count), true);
                            }
                        });
                    } else {
                        files.Add(new FileInfo(file));
                        UpdateProgressBar(file, "Acquiring");
                        //log.MakeLog(string.Format("File {1}: Adding {0} to list.", file, c), true); 
                        //log.MakeLog(string.Format("'files' contains {0} items.", files.Count), true);
                    }
                    c++;
                }
            }, token);
            task1.Wait(token);
            if(token.IsCancellationRequested) return;
            log.MakeLog(string.Format("'files' contains {0} items.", files.Count), true); 
            
            Task task2 = Task.Run(() => {
                if (token.IsCancellationRequested)
                {
                    log.MakeLog("Process cancelled.", true);
                    stop = true;
                    return;
                }
                log.MakeLog("Finding sims3packs.", true);
                var sims3packs = files.Where(x => x.Extension == ".Sims3Pack" || x.Extension == ".sims3pack");
                sims3pack.AddRange(sims3packs);      
                Parallel.ForEach(sims3pack, s3 => {
                    if (token.IsCancellationRequested)
                    {
                        log.MakeLog("Process cancelled.", true);
                        stop = true;
                        return;
                    }
                    if (GlobalVariables.sortonthego == true){
                        string newloc = System.IO.Path.Combine(FilesSort.S3PacksFolder, s3.Name);
                        allfiles.Add(filesSort.MoveFile(s3.FullName, newloc, FilesSort.S3PacksFolder, new AllFiles { Name = s3.Name, Location = newloc, Type = "sims3pack", Status = "Fine"}));
                    } else {
                        allfiles.Add(new AllFiles { Name = s3.Name, Location = s3.FullName, Type = "sims3pack", Status = "Fine"});
                    }
                    log.MakeLog(string.Format("{0} is not a duplicate, handling.", s3.Name), true);
                });
                UpdateProgressBar("sims3packs", "Sorting");
            }, token);

            log.MakeLog(string.Format("'files' contains {0} items.", files.Count), true); 
            
            Task task3 = Task.Run(() => {
                if (token.IsCancellationRequested)
                {
                    log.MakeLog("Process cancelled.", true);
                    stop = true;
                    return;
                }
                log.MakeLog("Finding sims2packs.", true);
                var sims2packs = files.Where(x => x.Extension == ".Sims2Pack" || x.Extension == ".sims2pack");
                sims2pack.AddRange(sims2packs);
                Parallel.ForEach(sims2pack, s2 => { 
                    if (token.IsCancellationRequested)
                    {
                        log.MakeLog("Process cancelled.", true);
                        stop = true;
                        return;
                    }
                    if (GlobalVariables.sortonthego == true){
                        string newloc = System.IO.Path.Combine(FilesSort.S2PacksFolder, s2.Name);                        
                        allfiles.Add(filesSort.MoveFile(s2.FullName, newloc, FilesSort.S2PacksFolder, new AllFiles { Name = s2.Name, Location = newloc, Type = "sims2pack", Status = "Fine"}));
                    } else {
                        allfiles.Add(new AllFiles { Name = s2.Name, Location = s2.FullName, Type = "sims2pack", Status = "Fine"});
                    }
                    log.MakeLog(string.Format("{0} is not a duplicate, handling.", s2.Name), true);
                });
                UpdateProgressBar("sims2packs", "Sorting");                
            }, token);

            log.MakeLog(string.Format("'files' contains {0} items.", files.Count), true); 
            
            Task task5 = Task.Run(() => {
                if (token.IsCancellationRequested)
                {
                    log.MakeLog("Process cancelled.", true);
                    stop = true;
                    return;
                }
                log.MakeLog("Finding ts4scripts.", true);
                var sims4scripts = files.Where(x => x.Extension == ".ts4script" || x.Extension == ".TS4Script" || x.Extension == ".ts4Script");
                sims4script.AddRange(sims4scripts);
                Parallel.ForEach(sims4script, t4 => {  
                    if (token.IsCancellationRequested)
                    {
                        log.MakeLog("Process cancelled.", true);
                        stop = true;
                        return;
                    }
                    if (GlobalVariables.sortonthego == true){
                        string newloc = System.IO.Path.Combine(FilesSort.S4ScriptsFolder, t4.Name);
                        File.Move(t4.FullName, newloc);
                        allfiles.Add(filesSort.MoveFile(t4.FullName, newloc, FilesSort.S4ScriptsFolder, new AllFiles { Name = t4.Name, Location = newloc, Type = "ts4script", Status = "Fine"}));
                    } else {
                        allfiles.Add(new AllFiles { Name = t4.Name, Location = t4.FullName, Type = "ts4script", Status = "Fine"});
                    }
                    log.MakeLog(string.Format("{0} is not a duplicate, adding to database.", t4.Name), true);
                    countprogress++;
                });
                UpdateProgressBar("sims4scripts", "Sorting");
            }, token);  

            log.MakeLog(string.Format("'files' contains {0} items.", files.Count), true);           

            Task task6 = Task.Run(() => {
                if (token.IsCancellationRequested)
                {
                    log.MakeLog("Process cancelled.", true);
                    stop = true;
                    return;
                }
                log.MakeLog("Finding compressed files.", true);
                var compresseds = files.Where(x => x.Extension == ".zip" || x.Extension == ".rar" || x.Extension == ".7z" || x.Extension == ".pkg");
                compressed.AddRange(compresseds);
                Parallel.ForEach(compressed, c => { 
                    if (token.IsCancellationRequested)
                    {
                        log.MakeLog("Process cancelled.", true);
                        stop = true;
                        return;
                    }
                    if (GlobalVariables.sortonthego == true){
                        string newloc = System.IO.Path.Combine(FilesSort.ZipFilesFolder, c.Name);
                        allfiles.Add(filesSort.MoveFile(c.FullName, newloc, FilesSort.ZipFilesFolder, new AllFiles { Name = c.Name, Location = newloc, Type = "compressed file", Status = "Fine"}));
                    } else {
                        allfiles.Add(new AllFiles { Name = c.Name, Location = c.FullName, Type = "compressed file", Status = "Fine"});
                    }
                    log.MakeLog(string.Format("{0} is not a duplicate, adding to database.", c.Name), true);
                }); 
                UpdateProgressBar("compressed files", "Sorting");
            }, token);
            if(token.IsCancellationRequested) return;
            log.MakeLog(string.Format("'files' contains {0} items.", files.Count), true); 

            Task task7 = Task.Run(() => {
                if (token.IsCancellationRequested)
                {
                    log.MakeLog("Process cancelled.", true);
                    stop = true;
                    return;
                }
                log.MakeLog("Finding tray files.", true);
                var traystuff = files.Where(x => x.Extension == ".householdbinary" || x.Extension == ".trayitem" || x.Extension == ".hhi" || x.Extension == ".sgi");
                trayitems.AddRange(traystuff);
                Parallel.ForEach(trayitems, t => {   
                    if (token.IsCancellationRequested)
                    {
                        log.MakeLog("Process cancelled.", true);
                        stop = true;
                        return;
                    }  
                    if (GlobalVariables.sortonthego == true){
                        string newloc = System.IO.Path.Combine(FilesSort.TrayFilesFolder, t.Name);
                        allfiles.Add(filesSort.MoveFile(t.FullName, newloc, FilesSort.TrayFilesFolder, new AllFiles { Name = t.Name, Location = newloc, Type = "tray file", Status = "Fine"}));
                    } else {
                        allfiles.Add(new AllFiles { Name = t.Name, Location = t.FullName, Type = "tray file", Status = "Fine"});
                    }
                    log.MakeLog(string.Format("{0} is not a duplicate, adding to database.", t.Name), true);
                });
                UpdateProgressBar("tray files", "Sorting");
            }, token);     

            log.MakeLog(string.Format("'files' contains {0} items.", files.Count), true);          

            Task task8 = Task.Run(() => {
                if (token.IsCancellationRequested)
                {
                    log.MakeLog("Process cancelled.", true);
                    stop = true;
                    return;
                }
                log.MakeLog("Finding everything else.", true);
                var others = files.Where(x => x.Extension != ".zip" && x.Extension != ".rar" && x.Extension != ".7z" && x.Extension != ".pkg" && x.Extension != ".ts4script" && x.Extension != ".TS4Script" && x.Extension != ".ts4Script" && x.Extension != ".Sims2Pack" && x.Extension != ".sims2pack" && x.Extension != ".Sims3Pack" && x.Extension != ".sims3pack" && x.Extension != ".package" && x.Extension == ".householdbinary" && x.Extension == ".trayitem" && x.Extension == ".hhi" && x.Extension == ".sgi");
                other.AddRange(others);
                Parallel.ForEach(other, o => {  
                    if (token.IsCancellationRequested)
                    {
                        log.MakeLog("Process cancelled.", true);
                        stop = true;
                        return;
                    }
                    if (GlobalVariables.sortonthego == true){
                        string newloc = System.IO.Path.Combine(FilesSort.NonSimsFiles, o.Name);
                        allfiles.Add(filesSort.MoveFile(o.FullName, newloc, FilesSort.NonSimsFiles, new AllFiles { Name = o.Name, Location = newloc, Type = "other", Status = "Fine"}));
                    } else {
                        allfiles.Add(new AllFiles { Name = o.Name, Location = o.FullName, Type = "other", Status = "Fine"});
                    }
                    log.MakeLog(string.Format("{0} is not a duplicate, adding to database.", o.Name), true);
                });
                UpdateProgressBar("other files", "Sorting");
            }, token);  
            if(token.IsCancellationRequested) return;
            log.MakeLog(string.Format("'files' contains {0} items.", files.Count), true); 
                
            Task task9 = Task.Run(() => {
                if (token.IsCancellationRequested)
                {
                    log.MakeLog("Process cancelled.", true);
                    stop = true;
                    return;
                }
                log.MakeLog("Finding packages.", true);
                var package = files.Where(x => x.Extension == ".package");
                log.MakeLog(string.Format("There are {0} files in 'package', and {1} files in 'packages'.", package.Count().ToString(), packages.Count), true);
                packages.AddRange(package);
                log.MakeLog(string.Format("There are now {0} files in 'packages'.", packages.Count), true);
                int tempcount = 0;
                Parallel.ForEach(packages, p => { 
                    tempcount++;
                    if (token.IsCancellationRequested)
                    {
                        log.MakeLog("Process cancelled.", true);
                        stop = true;
                        return;
                    }
                    log.MakeLog(string.Format("Item {0}: {1} is not a duplicate, adding to database.", tempcount, p.Name), true);
                    allfiles.Add(new AllFiles { Name = p.Name, Location = p.FullName, Type = "package", Status = "Fine"});
                });
                UpdateProgressBar("package files", "Sorting");
            }, token);
            if(token.IsCancellationRequested) return;

            task2.Wait(token);
            task2.Dispose();
            task3.Wait(token);
            task3.Dispose();
            task5.Wait(token);
            task5.Dispose();
            task6.Wait(token);
            task6.Dispose();
            task7.Wait(token);
            task7.Dispose();
            task8.Wait(token);
            task8.Dispose();
            task9.Wait(token);  
            task9.Dispose();  
            log.MakeLog(string.Format("Found {0} sims3packs.", sims3pack.Count), true);
            log.MakeLog(string.Format("Found {0} sims2packs.", sims2pack.Count), true);
            log.MakeLog(string.Format("Found {0} sims4scripts.", sims4script.Count), true);
            log.MakeLog(string.Format("Found {0} compressed files.", compressed.Count), true);
            log.MakeLog(string.Format("Found {0} tray item files.", trayitems.Count), true);
            log.MakeLog(string.Format("Found {0} other files.", other.Count), true);
            log.MakeLog(string.Format("Found {0} package files.", packages.Count), true);
            if(token.IsCancellationRequested) return;
            Task task10 = Task.Run(() => { 
                if (token.IsCancellationRequested)
                {
                    log.MakeLog("Process cancelled.", true);
                    stop = true;
                    return;
                }                       
                log.MakeLog("Inserting, please wait.", true);
                try {
                    log.MakeLog(string.Format("Inserting 'allfiles' with {0} items.", allfiles.Count), true);
                    GlobalVariables.DatabaseConnection.InsertAll(allfiles);
                } catch (SQLite.SQLiteException e) {
                    Console.WriteLine(string.Format("Inserting sorted files to database failed. Exception: {0}", e.Message));
                }                    
            }, token);
            task10.Wait(token);
            task10.Dispose();
            files = new List<FileInfo>();
            sims3pack = new List<FileInfo>();
            sims2pack = new List<FileInfo>();
            sims4script = new List<FileInfo>();
            other = new List<FileInfo>();
            packages = new List<FileInfo>();
            compressed = new List<FileInfo>();
            trayitems = new List<FileInfo>();
            allfiles = new List<AllFiles>();
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect();
            return;
        }              

        private void ReadPackages(CancellationToken token){
            if(token.IsCancellationRequested) return;
            List<AllFiles> allp = new List<AllFiles>();
            Task prepPackages = Task.Run(() => {
                allp = GlobalVariables.DatabaseConnection.Query<AllFiles>("SELECT * FROM AllFiles where Type='package'");
                log.MakeLog(string.Format("Packages to read count is {0}.", allp.Count), true);
                maxi = allp.Count;
                GlobalVariables.PackageCount = allp.Count();
                SetProgressBar();
                completionAlertValue("Getting Packages.");
                countprogress = 0;
                runprogress = true;            
            });
            prepPackages.Wait(token);
            prepPackages.Dispose();
            if(token.IsCancellationRequested) return;
            ConcurrentQueue<Task> ReadList = new ConcurrentQueue<Task>();
            Task makeList = Task.Run(() => {
                foreach (AllFiles p in allp){
                    if(token.IsCancellationRequested){
                        break;
                    }
                    ReadList.Enqueue(
                        new Task(()=> {
                            ReadPackage(p.Location);
                            UpdateProgressBar("package files for reading", "Getting");
                        }, token)
                    );
                }
            });
            makeList.Wait();
            makeList.Dispose();
            if(token.IsCancellationRequested) return;
            Task getPackages = Task.Run(() => {
                log.MakeLog(string.Format("Packages to read count is {0}.", allp.Count), true);
                SetProgressBar();
                completionAlertValue("Reading packages.");
                countprogress = 0;
                runprogress = true;            
            });
            getPackages.Wait(token);
            getPackages.Dispose();
            if(token.IsCancellationRequested) return;
            new Thread(() => RunUpdateElapsed(sw, token)) {IsBackground = true}.Start();
            new Thread(() => RunUpdateProgressBar(token)) {IsBackground = true}.Start();

            Task readpackages = Task.Run(async () => {
                foreach (Task t in ReadList){
                    if(token.IsCancellationRequested) return;
                    t.Start();
                }
                Task.WaitAll(ReadList.ToArray());
            }, token);
            readpackages.Wait();                
        }


        private async void ReadPackage(string location){            
            complete = initialprocess.CheckThrough(location);
        }

        private void WatchAddPackages(){
            log.MakeLog("Reading AddPackages.", true);
            bool logged = false;
            int interval = 20*1000;
            DateTime dueTime = DateTime.Now.AddMilliseconds(interval);            
            while (Processing == true){   
                if (logged == false){
                    log.MakeLog("Processing is true, so we're reading!", true);
                    logged = true;
                }
                if(DateTime.Now >= dueTime){
                    if (GlobalVariables.AddPackages.Count > databaseBatchSize){
                        string term = "AP";
                        log.MakeLog(string.Format("Found {0} files in {1}!", GlobalVariables.AddPackages.Count, term),true);
                        if (CurrentlyMovingAP == true) {
                            log.MakeLog(string.Format("It seems {0} is already processing, so we don't need to do anything.", term),true);
                        } else {
                            CurrentlyMovingAP = true;
                            new Thread(() => {
                                List<SimsPackage> sp = new();
                                for (int i = 0; i < GlobalVariables.AddPackages.Count; i++){                        
                                    GlobalVariables.AddPackages.TryDequeue(out var item);
                                    log.MakeLog(string.Format("Adding {0} to {1} list.", item.PackageName, term),true);
                                    sp.Add(item);
                                }
                                GlobalVariables.DatabaseConnection.InsertAllWithChildren(sp, true);
                                log.MakeLog(string.Format("Added {0} items to {1} database.", sp.Count, term), true);
                                 
                            }){IsBackground = true}.Start();
                            CurrentlyMovingAP = false;
                        }                    
                    }
                }
                else {
                    Thread.Sleep(1);
                } 
            }
        }

        private void WatchRemovePackages(){
            log.MakeLog("Reading RemovePackages.", true);
            bool logged = false;
            int interval = 25*1000;
            DateTime dueTime = DateTime.Now.AddMilliseconds(interval);            
            while (Processing == true){   
                if (logged == false){
                    log.MakeLog("Processing is true, so we're reading!", true);
                    logged = true;
                }
                if(DateTime.Now >= dueTime){
                    if (GlobalVariables.RemovePackages.Count > databaseBatchSize){
                        string term = "RP";
                        log.MakeLog(string.Format("Found {0} files in {1}!", GlobalVariables.RemovePackages.Count, term),true);
                        if (CurrentlyMovingRP == true)  {
                            log.MakeLog(string.Format("It seems {0} is already processing, so we don't need to do anything.", term),true);
                        } else {
                            CurrentlyMovingRP = true; 
                            new Thread(() => {
                                List<PackageFile> pf = new();
                                for (int i = 0; i < GlobalVariables.RemovePackages.Count; i++){
                                    GlobalVariables.RemovePackages.TryDequeue(out var item); 
                                    log.MakeLog(string.Format("Adding {0} to {1} list.", item.Name, term),true);
                                    pf.Add(item);                         
                                }
                                GlobalVariables.DatabaseConnection.DeleteAll(pf, true);
                                log.MakeLog(string.Format("Added {0} items to {1} database.", pf.Count, term), true);
                            }){IsBackground = true}.Start();
                            CurrentlyMovingRP = false; 
                        }
                    }
                } else {
                    Thread.Sleep(1);
                }      
            }
        }

        private void WatchProcessingReader(){
            log.MakeLog("Reading ProcessingReader.", true);
            bool logged = false;
            int interval = 30*1000;
            DateTime dueTime = DateTime.Now.AddMilliseconds(interval);            
            while (Processing == true){   
                if (logged == false){
                    log.MakeLog("Processing is true, so we're reading!", true);
                    logged = true;
                }
                if(DateTime.Now >= dueTime){
                    if (GlobalVariables.ProcessingReader.Count > databaseBatchSize){
                        string term = "PR";
                        log.MakeLog(string.Format("Found {0} files in {1}!", GlobalVariables.ProcessingReader.Count, term),true);
                        if (CurrentlyMovingPR == true)  {
                            log.MakeLog(string.Format("It seems {0} is already processing, so we don't need to do anything.", term),true);
                        } else {
                            CurrentlyMovingPR = true;
                            new Thread(() => {
                                List<PackageFile> pf = new();
                                for (int i = 0; i < GlobalVariables.ProcessingReader.Count; i++){
                                    GlobalVariables.ProcessingReader.TryDequeue(out var item);
                                    log.MakeLog(string.Format("Adding {0} to {1} list.", item.Name, term),true);
                                    pf.Add(item);
                                }
                                GlobalVariables.DatabaseConnection.InsertAllWithChildren(pf, true);
                                log.MakeLog(string.Format("Added {0} items to {1} database.", pf.Count, term), true);       
                            }){IsBackground = true}.Start();
                            CurrentlyMovingPR = false;
                        }
                    }
                } else {
                    Thread.Sleep(1);
                } 
            }
        }

        private void WatchAllFiles(){
            log.MakeLog("Reading AF.", true);
            bool logged = false;
            int interval = 35*1000;
            DateTime dueTime = DateTime.Now.AddMilliseconds(interval);            
            while (Processing == true){                   
                if (logged == false){
                    log.MakeLog("Processing is true, so we're reading!", true);
                    logged = true;
                }
                if(DateTime.Now >= dueTime){
                    if (GlobalVariables.AllFiles.Count > databaseBatchSize){
                        string term = "AF";
                        log.MakeLog(string.Format("Found {0} files in {1}!", GlobalVariables.AllFiles.Count, term),true);
                        if (CurrentlyMovingAF == true) {
                            log.MakeLog(string.Format("It seems {0} is already processing, so we don't need to do anything.", term),true);
                        } else {
                            CurrentlyMovingAF = true;
                            new Thread(() => {
                                List<AllFiles> af = new();
                                for (int i = 0; i < GlobalVariables.AllFiles.Count; i++){
                                    GlobalVariables.AllFiles.TryDequeue(out var item);
                                    log.MakeLog(string.Format("Adding {0} to {1} list.", item.Name, term),true);
                                    af.Add(item);
                                }
                                GlobalVariables.DatabaseConnection.InsertAllWithChildren(af, true);
                                log.MakeLog(string.Format("Added {0} items to {1} database.", af.Count, term), true);
                            }){IsBackground = true}.Start();
                            CurrentlyMovingAF = false;
                        }
                    }
                } else {
                    Thread.Sleep(1);
                }
            }            
        }

        private void WatchInstancesRecolorsS2Col(){
            log.MakeLog("Reading IRS2.", true);
            bool logged = false;
            int interval = 40*1000;
            DateTime dueTime = DateTime.Now.AddMilliseconds(interval);            
            while (Processing == true){   
                if (logged == false){
                    log.MakeLog("Processing is true, so we're reading!", true);
                    logged = true;
                }
                if(DateTime.Now >= dueTime){
                    if (GlobalVariables.InstancesRecolorsS2Col.Count > databaseBatchSize){
                        string term = "IRS2";
                        log.MakeLog(string.Format("Found {0} files in {1}!", GlobalVariables.InstancesRecolorsS2Col.Count, term),true);
                        if (CurrentlyMovingIRS2 == true) {
                            log.MakeLog(string.Format("It seems {0} is already processing, so we don't need to do anything.", term),true);
                        } else {
                            CurrentlyMovingIRS2 = true;
                            new Thread(() => {
                                List<InstancesRecolorsS2> irs2 = new();
                                for (int i = 0; i < GlobalVariables.InstancesRecolorsS2Col.Count; i++){
                                    GlobalVariables.InstancesRecolorsS2Col.TryDequeue(out var item);
                                    log.MakeLog(string.Format("Adding {0} to {1} list.", item.PackageName, term),true);
                                    irs2.Add(item);
                                }
                                GlobalVariables.InstancesCacheConnection.InsertAllWithChildren(irs2, true);
                                log.MakeLog(string.Format("Added {0} items to {1} database.", irs2.Count, term), true);
                                
                            }){IsBackground = true}.Start();
                            CurrentlyMovingIRS2 = false;
                        }
                    }
                } else {
                    Thread.Sleep(1);
                }
            }            
        }

        private void WatchInstancesRecolorsS3Col(){
            log.MakeLog("Reading IRS3.", true);
            bool logged = false;
            int interval = 45*1000;
            DateTime dueTime = DateTime.Now.AddMilliseconds(interval);            
            while (Processing == true){   
                if (logged == false){
                    log.MakeLog("Processing is true, so we're reading!", true);
                    logged = true;
                }
                if(DateTime.Now >= dueTime){
                    if (GlobalVariables.InstancesRecolorsS3Col.Count > databaseBatchSize){
                        string term = "IRS3";
                        log.MakeLog(string.Format("Found {0} files in {1}!", GlobalVariables.InstancesRecolorsS3Col.Count, term),true);
                        if (CurrentlyMovingIRS3 == true) {
                            log.MakeLog(string.Format("It seems {0} is already processing, so we don't need to do anything.", term),true);
                        } else {
                            CurrentlyMovingIRS3 = true;
                            new Thread(() => {
                                List<InstancesRecolorsS3> irs3 = new();
                                for (int i = 0; i < GlobalVariables.InstancesRecolorsS3Col.Count; i++){
                                    GlobalVariables.InstancesRecolorsS3Col.TryDequeue(out var item);
                                    log.MakeLog(string.Format("Adding {0} to {1} list.", item.PackageName, term),true);
                                    irs3.Add(item);
                                }
                                GlobalVariables.InstancesCacheConnection.InsertAllWithChildren(irs3, true);
                                log.MakeLog(string.Format("Added {0} items to {1} database.", irs3.Count, term), true);
                                
                            }){IsBackground = true}.Start();
                            CurrentlyMovingIRS3 = false;
                        }                    
                    }
                } else {
                    Thread.Sleep(1);
                }
            }            
        }

        private void WatchInstancesRecolorsS4Col(){
            log.MakeLog("Reading IRS4.", true);
            bool logged = false;
            int interval = 50*1000;
            DateTime dueTime = DateTime.Now.AddMilliseconds(interval);            
            while (Processing == true){   
                if (logged == false){
                    log.MakeLog("Processing is true, so we're reading!", true);
                    logged = true;
                }
                if(DateTime.Now >= dueTime){
                    if (GlobalVariables.InstancesRecolorsS4Col.Count > databaseBatchSize){
                        string term = "IRS4";
                        log.MakeLog(string.Format("Found {0} files in {1}!", GlobalVariables.InstancesRecolorsS4Col.Count, term),true);
                        if (CurrentlyMovingIRS4 == true)  {
                            log.MakeLog(string.Format("It seems {0} is already processing, so we don't need to do anything.", term),true);
                        } else {
                            CurrentlyMovingIRS4 = true;
                            new Thread(() => {
                                List<InstancesRecolorsS4> irs4 = new();
                                for (int i = 0; i < GlobalVariables.InstancesRecolorsS4Col.Count; i++){
                                    GlobalVariables.InstancesRecolorsS4Col.TryDequeue(out var item);
                                    log.MakeLog(string.Format("Adding {0} to {1} list.", item.PackageName, term),true);
                                    irs4.Add(item);
                                }
                                GlobalVariables.InstancesCacheConnection.InsertAllWithChildren(irs4, true);
                                log.MakeLog(string.Format("Added {0} items to {1} database.", irs4.Count, term), true);
                            }){IsBackground = true}.Start();
                            CurrentlyMovingIRS4 = false;
                        }
                        
                    }
                } else {
                    Thread.Sleep(1);
                }
            }            
        }

        private void WatchInstancesMeshesS2Col(){
            log.MakeLog("Reading IMS2.", true);
            bool logged = false;
            int interval = 55*1000;
            DateTime dueTime = DateTime.Now.AddMilliseconds(interval);            
            while (Processing == true){   
                if (logged == false){
                    log.MakeLog("Processing is true, so we're reading!", true);
                    logged = true;
                }
            }
            if(DateTime.Now >= dueTime){
                if (GlobalVariables.InstancesMeshesS2Col.Count > databaseBatchSize){
                    string term = "IMS2"; 
                    log.MakeLog(string.Format("Found {0} files in {1}!", GlobalVariables.InstancesMeshesS2Col.Count, term),true);
                    if (CurrentlyMovingIMS2 == true) {
                        log.MakeLog(string.Format("It seems {0} is already processing, so we don't need to do anything.", term),true);
                    } else {
                        CurrentlyMovingIMS2 = true;
                        new Thread(() => {                            
                            List<InstancesMeshesS2> ims2 = new();
                            for (int i = 0; i < GlobalVariables.InstancesMeshesS2Col.Count; i++){
                                GlobalVariables.InstancesMeshesS2Col.TryDequeue(out var item);
                                log.MakeLog(string.Format("Adding {0} to {1} list.", item.PackageName, term),true);
                                ims2.Add(item);                        
                            }
                            GlobalVariables.InstancesCacheConnection.InsertAllWithChildren(ims2, true);
                            log.MakeLog(string.Format("Added {0} items to {1} database.", ims2.Count, term), true);
                            
                        }){IsBackground = true}.Start();
                        CurrentlyMovingIMS2 = false;
                    }                    
                } 
            } else {
                Thread.Sleep(1);
            }
        }

        private void WatchInstancesMeshesS3Col(){
            log.MakeLog("Reading IMS3.", true);
            bool logged = false;
            int interval = 60*1000;
            DateTime dueTime = DateTime.Now.AddMilliseconds(interval);            
            while (Processing == true){   
                if (logged == false){
                    log.MakeLog("Processing is true, so we're reading!", true);
                    logged = true;
                }
                if(DateTime.Now >= dueTime){
                    if (GlobalVariables.InstancesMeshesS3Col.Count > databaseBatchSize){ 
                        string term = "IMS3"; 
                        log.MakeLog(string.Format("Found {0} files in {1}!", GlobalVariables.InstancesMeshesS3Col.Count, term),true);
                        if (CurrentlyMovingIMS3 == true)  {
                            log.MakeLog(string.Format("It seems {0} is already processing, so we don't need to do anything.", term),true);
                        } else {
                            CurrentlyMovingIMS3 = true;
                            new Thread(() => {                                
                                List<InstancesMeshesS3> ims3 = new();
                                for (int i = 0; i < GlobalVariables.InstancesMeshesS3Col.Count; i++){
                                    GlobalVariables.InstancesMeshesS3Col.TryDequeue(out var item);
                                    log.MakeLog(string.Format("Adding {0} to {1} list.", item.PackageName, term),true);
                                    ims3.Add(item);
                                }
                                GlobalVariables.InstancesCacheConnection.InsertAllWithChildren(ims3, true);
                                log.MakeLog(string.Format("Added {0} items to {1} database.", ims3.Count, term), true);
                            }){IsBackground = true}.Start();
                            CurrentlyMovingIMS3 = false;
                        }                    
                    }
                } else {
                    Thread.Sleep(1);
                }
            }            
        }

        private void WatchInstancesMeshesS4Col(){
            log.MakeLog("Reading IMS4.", true);
            bool logged = false;
            int interval = 65*1000;
            DateTime dueTime = DateTime.Now.AddMilliseconds(interval);            
            while (Processing == true){   
                if (logged == false){
                    log.MakeLog("Processing is true, so we're reading!", true);
                    logged = true;
                }
                if(DateTime.Now >= dueTime){
                    if (GlobalVariables.InstancesMeshesS4Col.Count > databaseBatchSize){ 
                        string term = "IMS4"; 
                        log.MakeLog(string.Format("Found {0} files in {1}!", GlobalVariables.InstancesMeshesS4Col.Count, term),true);
                        if (CurrentlyMovingIMS4 == true) {
                            log.MakeLog(string.Format("It seems {0} is already processing, so we don't need to do anything.", term),true);
                        } else {
                            CurrentlyMovingIMS4 = true;
                            new Thread(() => {
                                List<InstancesMeshesS4> ims4 = new();
                                for (int i = 0; i < GlobalVariables.InstancesMeshesS4Col.Count; i++){
                                    GlobalVariables.InstancesMeshesS4Col.TryDequeue(out var item);
                                    log.MakeLog(string.Format("Adding {0} to {1} list.", item.PackageName, term),true);
                                    ims4.Add(item);
                                }
                                GlobalVariables.InstancesCacheConnection.InsertAllWithChildren(ims4, true);
                                log.MakeLog(string.Format("Added {0} items to {1} database.", ims4.Count, term), true);
                            }){IsBackground = true}.Start();
                            CurrentlyMovingIMS4 = false;
                        }                   
                    }
                } else {
                    Thread.Sleep(1);
                }
            }
        }

        
        private void FindConflicts(){
            //GlobalVariables.DatabaseConnection.Query("select Packages.InstanceID from Packages INNER JOIN Ingredient_Recipe ON recipe_ID = Ingredient_Recipe.recipe_ID
            //Where Ingredient_ID = "oregano")            
        }

        private void GetResults(){
            runprogress = false;
            HideProgressGrid();
            ShowLoadingResultsGrid();            
            OpenResultsWindow();            
        }

        private void OpenResultsWindow(){
            Dispatcher.Invoke(new Action(() => {
                try {
                    ResultsWindow resultsWindow = new ResultsWindow(cts);
                    resultsWindow.Show();
                } catch (Exception e) {
                    Console.WriteLine("Failed to open results window. Error: " + e.Message);
                    throw;
                }                
                this.Close();
            }));
        }

        private void ShowLoadingResultsGrid(){
            //Dispatcher.Invoke(new Action(() => LoadingResultsGrid.Visibility = Visibility.Visible));
        }

        #endregion

        #region Utilities

        private int CountItems(string type) {
            log.MakeLog("Checking count of items.", true);
            int value;
            string cmdtext = string.Format("SELECT count(*) FROM AllFiles where type = '{0}'", type);
            var sqm = GlobalVariables.DatabaseConnection.CreateCommand(cmdtext);
            value = sqm.ExecuteScalar<int>();
            log.MakeLog("Counted! Returning.", true);
            return value;
        }

        private int CountDatabase(CancellationToken token){
            log.MakeLog("Checking count of packages database.", true);
            
            var command = GlobalVariables.DatabaseConnection.CreateCommand("SELECT count(*) FROM Processing_Reader");
            log.MakeLog(string.Format("There are {0} packages to process.", command.ExecuteScalar<Int32>()), true);
            var packagesQuery = GlobalVariables.DatabaseConnection.Query<PackageFile>("SELECT * FROM Processing_Reader ORDER BY Name ASC");
            GlobalVariables.PackageCount = packagesQuery.Count();
            allpending.AddRange(packagesQuery);
            var pending2 = GlobalVariables.DatabaseConnection.Query<PackageFile>("SELECT * FROM Processing_Reader where Game = 2 ORDER BY Name ASC");
            s2pending.AddRange(pending2);
            log.MakeLog(string.Format("Found {0} pending Sims 2 files.", pending2.Count), true);

            var pending3 = GlobalVariables.DatabaseConnection.Query<PackageFile>("SELECT * FROM Processing_Reader where Game = 3 ORDER BY Name ASC");
            s3pending.AddRange(pending3);
            log.MakeLog(string.Format("Found {0} pending Sims 3 files.", pending3.Count), true);

            var pending4 = GlobalVariables.DatabaseConnection.Query<PackageFile>("SELECT * FROM Processing_Reader where Game = 4 ORDER BY Name ASC");
            s4pending.AddRange(pending4);
            log.MakeLog(string.Format("Found {0} pending Sims 4 files.", pending4.Count), true);

            return 0;
        }

        private void ElapsedProcessing(string thing){
            TimeSpan ts = sw.Elapsed;
            string elapsedtime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                ts.Hours, ts.Minutes, ts.Seconds,
                                ts.Milliseconds / 10);
            log.MakeLog(string.Format("Processing {0} took {1}", thing, elapsedtime), true);
        }  

        public void UpdateProgressBar(string name, string verb){
            Dispatcher.Invoke(new Action(() => textCurrentPk.Text = string.Format("{0}/{1} - {2} {3}", GlobalVariables.packagesRead, maxi, verb, name)));
            Dispatcher.Invoke(new Action(() => mainProgressBar.Value++));
        }

        private void RunUpdateProgressBar(CancellationToken token){  
            int interval = 5*1000;
            DateTime dueTime = DateTime.Now.AddMilliseconds(interval);            
            Dispatcher.Invoke(new Action(() => textCurrentPk.Text = string.Format("Preparing to read {0} packages.", maxi)));
            while (runprogress == true)
            {
                if(DateTime.Now >= dueTime){
                    if (GlobalVariables.packagesRead >= 0 && (!String.IsNullOrWhiteSpace(GlobalVariables.currentpackage))){
                        new Thread(() => AutoUpdateProgressBar()) {IsBackground = true}.Start();
                    }
                    dueTime = DateTime.Now.AddMilliseconds(interval);
                } else {
                    Thread.Sleep(1);
                }
            }
        }

        public void AutoUpdateProgressBar(){
            Dispatcher.Invoke(new Action(() => {
                textCurrentPk.Text = string.Format("{0}/{1} - {2}", GlobalVariables.packagesRead, maxi, GlobalVariables.currentpackage);
                mainProgressBar.Value = GlobalVariables.packagesRead;
            }));
        }

        public void UpdateElasped(Stopwatch sw){
            new Thread(() => {
                List<string> times = new List<string>();
                TimeSpan Elapsed = sw.Elapsed;
                TimeSpan remaining = ((Elapsed / GlobalVariables.packagesRead) * maxi) - Elapsed;
                string sofar = String.Format("{0:00}:{1:00}:{2:00}",
                                Elapsed.Hours, Elapsed.Minutes, Elapsed.Seconds);

                string togo = String.Format("{0:00}:{1:00}:{2:00}",
                                remaining.Hours, remaining.Minutes, remaining.Seconds);
                times.Add(sofar);
                times.Add(togo);
                Dispatcher.Invoke(new Action(() => timeRemaining.Text = string.Format("Elapsed: {0} | Remaining: {1}", times[0], times[1])));
            }){IsBackground = true}.Start();            
        }

        private void RunUpdateElapsed(Stopwatch sw, CancellationToken token){
            //log.MakeLog("Running run update elapsed.", true);
            int interval = 1*1000;
            DateTime dueTime = DateTime.Now.AddMilliseconds(interval);            
            Dispatcher.Invoke(new Action(() => timeRemaining.Visibility = Visibility.Visible));
            while (runprogress == true)
            {
                if(DateTime.Now >= dueTime){
                    if (GlobalVariables.packagesRead >= 0 && (!String.IsNullOrWhiteSpace(GlobalVariables.currentpackage))){
                        new Thread(() => UpdateElasped(sw)) {IsBackground = true}.Start();
                    }
                dueTime = DateTime.Now.AddMilliseconds(interval);
                }
                else {
                    Thread.Sleep(1);
                }
            }       
        }

        #endregion
              

        private void testbutton_Click(object sender, EventArgs e) {
            //filesSort.InitializeSortingRules();            
        }        
    }

}
