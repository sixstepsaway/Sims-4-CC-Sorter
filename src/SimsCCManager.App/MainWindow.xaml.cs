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
using SQLiteNetExtensions.Attributes;
using SQLitePCL;
using SQLiteNetExtensions.Extensions.TextBlob;
using System.Data.SQLite;
using SimsCCManager.Misc.TrayReader;
using System.Runtime;
using SimsCCManager.Packages.Sorting;

namespace Sims_CC_Sorter
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
        int threads = Environment.ProcessorCount;
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

        public static int maxi = 0;

        public MainWindow()
        {           
            InitializeComponent();
            
            new Thread(() => StartIt()) {IsBackground = true}.Start();
        }

        private void StartIt(){
            if (eatenturecpu == true){
                threadstouse = threads - 2;
            } else {
                threadstouse = (threads - 2) / 2;
            }
            parallelSettings.MaxDegreeOfParallelism = threadstouse;

            ThreadPool.SetMaxThreads(threadstouse, 0);
            ThreadPool.SetMinThreads(threadstouse, 0);


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
        
        private void App_Loaded(object sender, RoutedEventArgs e){
             
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
                    var packagesQuery = GlobalVariables.DatabaseConnection.Query<PackageFile>("SELECT * FROM Processing_Reader where Status = 'Pending'");
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
            threadstouse = threads / 3;
            parallelSettings.MaxDegreeOfParallelism = threadstouse;
            log.MakeLog(string.Format("Threads set to {0}", threadstouse), true);
        }
        private void eatcpu_Uncheck(object sender, RoutedEventArgs e){
            eatenturecpu = true;
            threadstouse = threads;
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
            log.MakeLog("Making BrokenChecked table", true);
            try {
                GlobalVariables.DatabaseConnection.CreateTable <BrokenChecked>();
            } catch (Exception e){
                log.MakeLog(string.Format("Ran into an error making BrokenChecked table: {0}", e), true);
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
            
            
            var pragmas = new List<string>(){
                "PRAGMA journal_mode=MEMORY",
                "PRAGMA synchronous=EXTRA",
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

            Task ff = Task.Run(() => {
                FindFiles(token);
            }, token);
            ff.Wait(token);
            ff.Dispose();
            log.MakeLog("Finished finding files.", true);

            sw.Stop();
            Task stopwatch = Task.Run(() => {
                ElapsedProcessing("Categorizing files"); 
            }, token);
            stopwatch.Wait(token);
            stopwatch.Dispose();
            sw.Restart();

                        
            Task countdata = Task.Run(() => {
               CountDatabase(token);
            }, token);
            countdata.Wait(token);  
            countdata.Dispose(); 

            Task rp = Task.Run(() => {
                ReadPackages(token);
            }, token);
            rp.Wait(token);
            rp.Dispose();
            
            sw.Stop();
            Task updatesw = Task.Run(() => {
                complete= true;               
            }, token);

            if (complete == true){
                runprogress = false;
            }
            updatesw.Dispose();
            //new Thread(() => ) {IsBackground = true}.Start();

            if (!token.IsCancellationRequested) {
                sw.Stop();
                completionAlertValue("Done!");
                SetTextCurrentPkText("");
                SetProgressBarMax();
                ElapsedProcessing("reading packages");
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

        private void FindFiles(CancellationToken token){
            var directory = new DirectoryInfo(GlobalVariables.ModFolder);
            var VilesS = directory.EnumerateFiles("*.*", SearchOption.AllDirectories).Where(file => !file.DirectoryName.Contains("_SORTED"));;
            List<FileInfo> files = new();
            files.AddRange(VilesS);
            string[] filesS = new string[files.Count];
            for (int f = 0; f < files.Count; f++){
                filesS[f] = files[f].FullName;
            }
            
            Task task0 = Task.Run(() => {
                log.MakeLog("Sorting packages files from non-package files.", true);
                log.MakeLog(string.Format("Setting maxi to {0}", filesS.Length), true);
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
                                log.MakeLog(string.Format("File {0} already exists in database and is awaiting processing.", file), true);
                            } else if (packagesDone.Any()){
                                log.MakeLog(string.Format("File {0} already exists in database and has been read.", file), true);                                
                            } else if (notPack.Any()){
                                log.MakeLog(string.Format("File {0} already exists in database isn't a package.", file), true);
                            } else {
                                files.Add(new FileInfo(file));
                                UpdateProgressBar(file, "Acquiring");
                                log.MakeLog(string.Format("Adding {0} to list.", file), true); 
                            }
                        });
                    } else {
                        files.Add(new FileInfo(file));
                        UpdateProgressBar(file, "Acquiring");
                        log.MakeLog(string.Format("Adding {0} to list.", file), true); 
                    }
                    c++;
                }
            }, token);
            task1.Wait(token);
            
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
                
            Task task9 = Task.Run(() => {
                if (token.IsCancellationRequested)
                {
                    log.MakeLog("Process cancelled.", true);
                    stop = true;
                    return;
                }
                log.MakeLog("Finding packages.", true);
                var package = files.Where(x => x.Extension == ".package");
                packages.AddRange(package);
                Parallel.ForEach(packages, p => { 
                    if (token.IsCancellationRequested)
                    {
                        log.MakeLog("Process cancelled.", true);
                        stop = true;
                        return;
                    }log.MakeLog(string.Format("{0} is not a duplicate, adding to database.", p.Name), true);
                    packagefiles.Add(new PackageFile { Name = p.Name, Location = p.FullName, Status = "Pending"});
                    allfiles.Add(new AllFiles { Name = p.Name, Location = p.FullName, Type = "package", Status = "Fine"});
                });
                UpdateProgressBar("package files", "Sorting");
            }, token);


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

            Task task10 = Task.Run(() => { 
                if (token.IsCancellationRequested)
                {
                    log.MakeLog("Process cancelled.", true);
                    stop = true;
                    return;
                }                       
                log.MakeLog("Inserting, please wait.", true);
                try {
                    GlobalVariables.DatabaseConnection.InsertAll(allfiles);
                    GlobalVariables.DatabaseConnection.InsertAll(packagefiles);
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
            List<PackageFile> allp = new List<PackageFile>();
            Task getPackages = Task.Run(() => {
                var dbqc = GlobalVariables.DatabaseConnection.Query<PackageFile>("SELECT * FROM Processing_Reader where Status = 'Pending' ORDER BY Name ASC");
                allp.AddRange(dbqc); 
                maxi = allp.Count;
                GlobalVariables.PackageCount = allp.Count();
                SetProgressBar();
                completionAlertValue("Reading packages.");
                countprogress = 0;
                runprogress = true;            
            });
            getPackages.Wait(token);
            getPackages.Dispose();
            new Thread(() => RunUpdateElapsed(sw)) {IsBackground = true}.Start();
            new Thread(() => RunUpdateProgressBar(token)) {IsBackground = true}.Start();
            Task ReadPackages = Task.Run(() => {                
                if (GlobalVariables.debugMode == false){
                    foreach (PackageFile p in allp){
                        if (token.IsCancellationRequested)
                        {
                            log.MakeLog("Process cancelled.", true);
                            stop = true;
                            return;
                        }
                        Task read = Task.Run(() => {
                            try {
                                if (File.Exists(p.Location)){
                                    initialprocess.CheckThrough(p.Location);
                                } else {
                                    log.MakeLog(string.Format("File {0} could not be found.", p.Name), true);
                                }                                
                            } catch (Exception e) {
                                log.MakeLog(string.Format("Caught exception running initial process on {1}: {0}", e.Message, p.Location), true);
                            }
                        });
                        read.Wait(token);                        
                    };
                } else {
                    Parallel.ForEach(allp, p => {
                        if (token.IsCancellationRequested)
                        {
                            log.MakeLog("Process cancelled.", true);
                            stop = true;
                            return;
                        }
                        try {
                            if (File.Exists(p.Location)){
                                initialprocess.CheckThrough(p.Location);
                            } else {
                                log.MakeLog(string.Format("File {0} could not be found.", p.Name), true);
                            }
                        } catch (Exception e) {
                            log.MakeLog(string.Format("Caught exception running initial process on {1}: {0}", e.Message, p.Location), true);
                        }
                    });
                }
            }, token);
            ReadPackages.Wait(token);
            ReadPackages.Dispose();
        }

        private void FindConflicts(){
            //GlobalVariables.DatabaseConnection.Query("select Packages.InstanceID from Packages INNER JOIN Ingredient_Recipe ON recipe_ID = Ingredient_Recipe.recipe_ID
            //Where Ingredient_ID = "oregano")            
        }

        private void GetResults(){
            runprogress = false;
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

        #endregion

        #region Utilities

        private int CountItems(string type) {
            log.MakeLog("Checking count of items.", true);
            int value;
            string cmdtext = string.Format("SELECT count(*) FROM AllFiles where type = '{0}'", type);
            var sqm = GlobalVariables.DatabaseConnection.CreateCommand(cmdtext);
            value = sqm.ExecuteScalar<Int32>();
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

        private async void RunUpdateProgressBar(CancellationToken token){  
            Task rp = Task.Run(() => {
               while (runprogress == true)
                {   
                    if (GlobalVariables.packagesRead >= 0 && (!String.IsNullOrWhiteSpace(GlobalVariables.currentpackage))){
                        new Thread(() => AutoUpdateProgressBar()) {IsBackground = true}.Start();
                    }
                    else 
                    {
                        Dispatcher.Invoke(new Action(() => textCurrentPk.Text = string.Format("Preparing to read {0} packages.", maxi)));
                    }                
                } 
            });
            rp.Wait(token);
            rp.Dispose();
            return;            
        }

        public void AutoUpdateProgressBar(){
            //for the reading of the packages
            //log.MakeLog("Updating progress bar.", true);
            Dispatcher.Invoke(new Action(() => textCurrentPk.Text = string.Format("{0}/{1} - {2}", GlobalVariables.packagesRead, maxi, GlobalVariables.currentpackage)));
            Dispatcher.Invoke(new Action(() => mainProgressBar.Value = GlobalVariables.packagesRead));
        }

        public void UpdateElasped(Stopwatch sw){
            //log.MakeLog("Updating elapsed.", true);
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
        }

        private async void RunUpdateElapsed(Stopwatch sw){
            Task rue = Task.Run(() => {
                //log.MakeLog("Running run update elapsed.", true);
                Dispatcher.Invoke(new Action(() => timeRemaining.Visibility = Visibility.Visible));
                while (runprogress == true)
                {
                    if (GlobalVariables.packagesRead != 0){
                        new Thread(() => UpdateElasped(sw)) {IsBackground = true}.Start();
                    }
                }
            });
            rue.Wait();
            rue.Dispose();
            return;            
        }


        #endregion
              

        private void testbutton_Click(object sender, EventArgs e) {

        }        
    }

}
