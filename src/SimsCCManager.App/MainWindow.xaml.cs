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


namespace Sims_CC_Sorter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window    

    {
        S2PackageSearch s2packs = new S2PackageSearch();
        S4PackageSearch s4packs = new S4PackageSearch();
        LoggingGlobals log = new LoggingGlobals();
        GlobalVariables globalVars = new GlobalVariables();
        InitialProcessing initialprocess = new InitialProcessing();
        ParallelOptions parallelSettings = new ParallelOptions();
        ConcurrentQueue<Task> TaskList = new ConcurrentQueue<Task>();
        public Stopwatch sw = new Stopwatch();
        string SelectedFolder = "";
        string statement = "";
        int gameNum = 0;
        bool keepgoing = true;
        bool continuing = true;
        bool stop = false;
        bool eatenturecpu = true;

        private bool FindNewAdditions;
        public static bool dataExists;
        private bool ContinuePrevious;
        private bool ManageFolder;
        private bool SortFolder;
        public bool DetectedHalfRun;
        public bool runprogress = false;
        private int workerThreads;
        private int portThreads;
        public int countprogress = 0;
        private int batchsize = 250;
        private int totalbatches = 0;
        int threads = Environment.ProcessorCount;
        int threadstouse = 0;
        bool complete = false;
        public string currentpackage;
        
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

        private List<PackageFile> packagesPending = new List<PackageFile>();
        private List<PackageFile> packagesProcessing = new List<PackageFile>();
        private List<SimsPackage> packagesDone = new List<SimsPackage>();
        private List<AllFiles> notpack = new List<AllFiles>();
        private List<FileInfo> files = new List<FileInfo>(); 
        private List<AllFiles> allfiles = new List<AllFiles>();
        private List<PackageFile> packagefiles = new List<PackageFile>();

        private CancellationTokenSource cts = new CancellationTokenSource();        
        public static int progresstracker = 0;
        public static int maxi = 0;

        public MainWindow()
        {           
            InitializeComponent();
            Task.Run(() => {
                StartIt();
            });
        }

        private void StartIt(){
            if (eatenturecpu == true){
                threadstouse = threads - 2;
            } else {
                threadstouse = (threads - 2) / 2;
            }
            parallelSettings.MaxDegreeOfParallelism = threadstouse;
            if (GlobalVariables.debugMode) {
                Dispatcher.Invoke(new Action(() => testButton.Visibility = Visibility.Visible));
            } else {
                Dispatcher.Invoke(new Action(() => testButton.Visibility = Visibility.Hidden));
            }            
            if (dataExists == true) 
            {
                Dispatcher.Invoke(new Action(() => LoadButton.Visibility = Visibility.Visible));
            } else {
                Dispatcher.Invoke(new Action(() => LoadButton.Visibility = Visibility.Collapsed));
            }
        }      
        
        private void App_Loaded(object sender, RoutedEventArgs e){
             
        }

        #region Load 

        private void loadData_Click(object sender, RoutedEventArgs e){
            Task.Run(() => {
               GetResults(); 
            });
            
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

        private void Kofi_Click(object sender, EventArgs e){
            if (System.Windows.Forms.MessageBox.Show
            ("Are you sure you want to go to Kofi?", "Opening External URL",
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

        private void CancelScan_Click(object sender, EventArgs e) {
            stop = true;
            cts.Cancel();
            CancelButton.Background = Brushes.LightGray;
            runprogress = false;
            while (stop == false){
                Thread.Sleep(1);
            }            
            // Cancellation should have happened, so call Dispose.
            
            HideProgressGrid();
            
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

        private void ManageOldFolder_Click(object sender, EventArgs e) {
            if (SelectedFolder == "") {
                System.Windows.Forms.MessageBox.Show("Please select the folder containing your package files.");
            } else {
                log.MakeLog("Managing old folder.", true);
                ManageFolder = true;
                //System.Windows.Forms.MessageBox.Show("Not yet implemented.");
                //ManageOldFolder();
            }            
        }

        private void SortNewFolder_Click(object sender, EventArgs e) {
            
            if (SelectedFolder == "") {
                System.Windows.Forms.MessageBox.Show("Please select the folder containing your package files.");
            } else {
                log.MakeLog("Sorting new folder.", true);
                SortFolder = true;
                DetectedHalfRun = DetectHalfRun();
                if (DetectedHalfRun == true){
                    Dispatcher.Invoke(new Action(() => FoundPastItems.Visibility = Visibility.Visible));
                    Dispatcher.Invoke(new Action(() => ContinueQuestion.Visibility = Visibility.Visible));
                } else {
                    SortNewPrep();
                }                
            }   
        }
        
        private void ContinueSearch_Click(object sender, EventArgs e) {
            Dispatcher.Invoke(new Action(() => ContinueQuestion.Visibility = Visibility.Hidden));
            Dispatcher.Invoke(new Action(() => FindNewItemsQuestion.Visibility = Visibility.Visible));
            ContinuePrevious = true;
        }
        private void RestartSearch_Click(object sender, EventArgs e) {
            ContinuePrevious = false;
            FindNewAdditions = true;
            Dispatcher.Invoke(new Action(() => FoundPastItems.Visibility = Visibility.Hidden));
            Task.Run(() => {
                SortNewPrep();
            });            
        }
        private void CancelSearch_Click(object sender, EventArgs e) {
            Dispatcher.Invoke(new Action(() => FoundPastItems.Visibility = Visibility.Hidden));
        }
        private void YesFindNewItems_Click(object sender, EventArgs e) {
            Dispatcher.Invoke(new Action(() => ContinueQuestion.Visibility = Visibility.Hidden));
            Dispatcher.Invoke(new Action(() => FindNewItemsQuestion.Visibility = Visibility.Hidden));
            Dispatcher.Invoke(new Action(() => FoundPastItems.Visibility = Visibility.Hidden));
            FindNewAdditions = true;
            Task.Run(() => {
                SortNewFolder(cts.Token);
            });            
        }
        private void NoDontFindNew_Click(object sender, EventArgs e) {
            Dispatcher.Invoke(new Action(() => ContinueQuestion.Visibility = Visibility.Hidden));
            Dispatcher.Invoke(new Action(() => FindNewItemsQuestion.Visibility = Visibility.Hidden));
            Dispatcher.Invoke(new Action(() => FoundPastItems.Visibility = Visibility.Hidden));
            FindNewAdditions = false;
            Task.Run(() => {
                SortNewFolder(cts.Token);
            });
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
            try {
                Dispatcher.Invoke(new Action(() => ProgressGrid.Visibility = Visibility.Visible));
            } catch (Exception e) {
                Console.WriteLine("Show Progress Grid Failed: " + e.Message);
            }
            try {
                Dispatcher.Invoke(new Action(() => MainMenuGrid.Visibility = Visibility.Hidden));
            } catch (Exception e) {
                Console.WriteLine("Hide Main Menu Grid Failed: " + e.Message);
            }
            try {
                Dispatcher.Invoke(new Action(() => completionAlert.Visibility = Visibility.Visible)); 
            } catch (Exception e) {
                Console.WriteLine("Show Completion Alert Failed: " + e.Message);
            } 

            try {
                Dispatcher.Invoke(new Action(() => mainProgressBar.Visibility = Visibility.Visible));
            } catch (Exception e) {
                Console.WriteLine("Show Main Progress Bar Failed: " + e.Message);
            }
        }

        private void HideProgressGrid(){
            Dispatcher.Invoke(new Action(() => ProgressGrid.Visibility = Visibility.Hidden));
            Dispatcher.Invoke(new Action(() => MainMenuGrid.Visibility = Visibility.Visible));
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
            GlobalVariables.DatabaseConnection.CreateTable <SimsPackage>();
            log.MakeLog("Making AgeGenderFlags table", true);
            GlobalVariables.DatabaseConnection.CreateTable <AgeGenderFlags>();
            log.MakeLog("Making TypeCounter table", true);
            GlobalVariables.DatabaseConnection.CreateTable <TypeCounter>();
            log.MakeLog("Making Tagslist table", true);
            GlobalVariables.DatabaseConnection.CreateTable <TagsList>();
            log.MakeLog("Making BrokenChecked table", true);
            GlobalVariables.DatabaseConnection.CreateTable <BrokenChecked>();
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
            Task.Run(() => {
               SortNewFolder(cts.Token); 
            });      
            
        }

        private void SortNewFolder(object obj){
            CancellationToken token = (CancellationToken)obj;  
            parallelSettings.CancellationToken = token; 
            sw.Start();
            ShowProgressGrid();
            if (ContinuePrevious == true){
                globalVars.ConnectDatabase(false);
            }

            Task ff = Task.Run(() => {
                FindFiles(token);
            }, token);
            ff.Wait();
            log.MakeLog("Finished finding files.", true);

            sw.Stop();
            Task stopwatch = Task.Run(() => {
                ElapsedProcessing("Categorizing files"); 
            }, token);
            stopwatch.Wait();
            sw.Restart();

                        
            Task countdata = Task.Run(() => {
               CountDatabase(token);
            }, token);
            countdata.Wait();   

            Task rp = Task.Run(() => {
                ReadPackages(token);
            }, token);
            rp.Wait();
            
            sw.Stop();
            Task updatesw = Task.Run(() => {
                complete= true;
                ElapsedProcessing("Reading packages");                 
            }, token);

            if (complete == true){
                runprogress = false;
            }

            //new Thread(() => ) {IsBackground = true}.Start();

            if (!token.IsCancellationRequested) {
                sw.Stop();
                ElapsedProcessing("reading packages");
                completionAlertValue("Done!");
                SetTextCurrentPkText("");
                SetProgressBarMax();
                ElapsedProcessing("reading packages");
                GetResults();
            } else {
                cts.Dispose();
                stop = false;
                runprogress = true;
                log.MakeLog("Fully cancelled.", true);
            }
        }

        #endregion

        #region Methods of Processing

        private void FindFiles(CancellationToken token){
            string[] filesS = Directory.GetFiles(GlobalVariables.ModFolder, "*", SearchOption.AllDirectories);
            Task task0 = Task.Run(() => {
                log.MakeLog("Sorting packages files from non-package files.", true);
                log.MakeLog(string.Format("Setting maxi to {0}", filesS.Length), true);
                maxi = (filesS.Length + 7);             
                log.MakeLog(string.Format("Maxi set to {0}.", maxi), true);                        
                log.MakeLog("Setting progress bar.", true);
                SetProgressBar();
                completionAlertValue("Sorting package files from non-package files.");
                log.MakeLog("Getting packagesPending.", true);
                var packagesPendingV = GlobalVariables.DatabaseConnection.Query<PackageFile>("SELECT * FROM Processing_Reader where Status = 'Pending'");
                packagesPending.AddRange(packagesPendingV);
                log.MakeLog("Getting packagesProcessing.", true);
                var packagesProcessingV = GlobalVariables.DatabaseConnection.Query<PackageFile>("SELECT * FROM Processing_Reader where Status = 'Processing'");
                packagesProcessing.AddRange(packagesPendingV);
                log.MakeLog("Getting packagesDone.", true);
                var packagesDoneV = GlobalVariables.DatabaseConnection.Query<SimsPackage>("SELECT * FROM Packages");
                packagesDone.AddRange(packagesDoneV);
                log.MakeLog("Getting notpack.", true);
                var notpackV = GlobalVariables.DatabaseConnection.Query<AllFiles>("SELECT * FROM AllFiles");
                notpack.AddRange(notpackV);
            }, token);
            task0.Wait();

            Task task1 = Task.Run(() => {
                int c = 0;
                foreach (string file in filesS){   
                    if (token.IsCancellationRequested)
                    {
                        log.MakeLog("Process cancelled.", true);
                        stop = true;
                        return;
                    }                     
                    files.Add(new FileInfo(file));
                    UpdateProgressBar(file, "Acquiring");
                    log.MakeLog(string.Format("Adding {0} to list.", file), true);
                    c++;
                }
            }, token);
            task1.Wait();
            
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
                    bool foundfile = false;                 
                    if (FindNewAdditions == true){                        
                        var isinpending = from pending in packagesPending
                            where pending.Location == s3.FullName
                            select pending.Location;
                            foundfile = isinpending.Any();
                        var isinprocessing = from processing in packagesProcessing
                            where processing.Location == s3.FullName
                            select processing.Location;
                            foundfile = isinprocessing.Any();
                        var isindone = from done in packagesDone
                            where done.Location == s3.FullName
                            select done.Location;
                            foundfile = isindone.Any();
                        var isinnp = from np in notpack
                            where np.Location == s3.FullName
                            select np.Location;
                            foundfile = isinnp.Any();
                    }

                    if (foundfile == true){
                        log.MakeLog(string.Format("File {0} already exists in database.", s3.Name), true);
                    } 
                    else 
                    { 
                        allfiles.Add(new AllFiles { Name = s3.Name, Location = s3.FullName, Type = "sims3pack", Status = "Fine"});
                        log.MakeLog(string.Format("{0} is not a duplicate, adding to database.", s3.Name), true);
                    }
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
                    bool foundfile = false;                 
                    if (FindNewAdditions == true){                        
                        var isinpending = from pending in packagesPending
                            where pending.Location == s2.FullName
                            select pending.Location;
                            foundfile = isinpending.Any();
                        var isinprocessing = from processing in packagesProcessing
                            where processing.Location == s2.FullName
                            select processing.Location;
                            foundfile = isinprocessing.Any();
                        var isindone = from done in packagesDone
                            where done.Location == s2.FullName
                            select done.Location;
                            foundfile = isindone.Any();
                        var isinnp = from np in notpack
                            where np.Location == s2.FullName
                            select np.Location;
                            foundfile = isinnp.Any();
                    }

                    if (foundfile == true){
                        log.MakeLog(string.Format("File {0} already exists in database.", s2.Name), true);
                    } 
                    else 
                    { 
                        allfiles.Add(new AllFiles { Name = s2.Name, Location = s2.FullName, Type = "sims2pack", Status = "Fine"});                        
                        log.MakeLog(string.Format("{0} is not a duplicate, adding to database.", s2.Name), true);
                    }
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
                    bool foundfile = false;                 
                    if (FindNewAdditions == true){                        
                        var isinpending = from pending in packagesPending
                            where pending.Location == t4.FullName
                            select pending.Location;
                            foundfile = isinpending.Any();
                        var isinprocessing = from processing in packagesProcessing
                            where processing.Location == t4.FullName
                            select processing.Location;
                            foundfile = isinprocessing.Any();
                        var isindone = from done in packagesDone
                            where done.Location == t4.FullName
                            select done.Location;
                            foundfile = isindone.Any();
                        var isinnp = from np in notpack
                            where np.Location == t4.FullName
                            select np.Location;
                            foundfile = isinnp.Any();
                    }

                    if (foundfile == true){
                        log.MakeLog(string.Format("File {0} already exists in database.", t4.Name), true);
                    }
                    else 
                    {                          
                        allfiles.Add(new AllFiles { Name = t4.Name, Location = t4.FullName, Type = "ts4script", Status = "Fine"});  
                        log.MakeLog(string.Format("{0} is not a duplicate, adding to database.", t4.Name), true);
                        countprogress++;
                    }
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
                    bool foundfile = false;                 
                    if (FindNewAdditions == true){                        
                        var isinpending = from pending in packagesPending
                            where pending.Location == c.FullName
                            select pending.Location;
                            foundfile = isinpending.Any();
                        var isinprocessing = from processing in packagesProcessing
                            where processing.Location == c.FullName
                            select processing.Location;
                            foundfile = isinprocessing.Any();
                        var isindone = from done in packagesDone
                            where done.Location == c.FullName
                            select done.Location;
                            foundfile = isindone.Any();
                        var isinnp = from np in notpack
                            where np.Location == c.FullName
                            select np.Location;
                            foundfile = isinnp.Any();
                    }

                    if (foundfile == true){
                        log.MakeLog(string.Format("File already exists in database.", c.Name), true);
                    } 
                    else 
                    {                         
                    allfiles.Add(new AllFiles { Name = c.Name, Location = c.FullName, Type = "compressed file", Status = "Fine"});
                    log.MakeLog(string.Format("{0} is not a duplicate, adding to database.", c.Name), true);
                    }
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
                    bool foundfile = false;                 
                    if (FindNewAdditions == true){                        
                        var isinpending = from pending in packagesPending
                            where pending.Location == t.FullName
                            select pending.Location;
                            foundfile = isinpending.Any();
                        var isinprocessing = from processing in packagesProcessing
                            where processing.Location == t.FullName
                            select processing.Location;
                            foundfile = isinprocessing.Any();
                        var isindone = from done in packagesDone
                            where done.Location == t.FullName
                            select done.Location;
                            foundfile = isindone.Any();
                        var isinnp = from np in notpack
                            where np.Location == t.FullName
                            select np.Location;
                            foundfile = isinnp.Any();
                    }

                    if (foundfile == true){
                        log.MakeLog(string.Format("File {0} already exists in database.", t.Name), true);
                    } 
                    else 
                    { 
                        allfiles.Add(new AllFiles { Name = t.Name, Location = t.FullName, Type = "tray file", Status = "Fine"});
                        log.MakeLog(string.Format("{0} is not a duplicate, adding to database.", t.Name), true);
                    }
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
                    bool foundfile = false;                 
                    if (FindNewAdditions == true){                        
                        var isinpending = from pending in packagesPending
                            where pending.Location == o.FullName
                            select pending.Location;
                            foundfile = isinpending.Any();
                        var isinprocessing = from processing in packagesProcessing
                            where processing.Location == o.FullName
                            select processing.Location;
                            foundfile = isinprocessing.Any();
                        var isindone = from done in packagesDone
                            where done.Location == o.FullName
                            select done.Location;
                            foundfile = isindone.Any();
                        var isinnp = from np in notpack
                            where np.Location == o.FullName
                            select np.Location;
                            foundfile = isinnp.Any();
                    }

                    if (foundfile == true){
                        log.MakeLog(string.Format("File {0} already exists in database.", o.Name), true);
                    } 
                    else 
                    {                           
                        allfiles.Add(new AllFiles { Name = o.Name, Location = o.FullName, Type = "other", Status = "Fine"});           
                        log.MakeLog(string.Format("{0} is not a duplicate, adding to database.", o.Name), true);
                    }
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
                    }      
                    bool foundfile = false;                 
                    if (FindNewAdditions == true){                        
                        var isinpending = from pending in packagesPending
                            where pending.Location == p.FullName
                            select pending.Location;
                            foundfile = isinpending.Any();
                        var isinprocessing = from processing in packagesProcessing
                            where processing.Location == p.FullName
                            select processing.Location;
                            foundfile = isinprocessing.Any();
                        var isindone = from done in packagesDone
                            where done.Location == p.FullName
                            select done.Location;
                            foundfile = isindone.Any();
                        var isinnp = from np in notpack
                            where np.Location == p.FullName
                            select np.Location;
                            foundfile = isinnp.Any();
                    }

                    if (foundfile == true){
                        log.MakeLog(string.Format("File {0} already exists in database.", p.Name), true);
                    } 
                    else 
                    { 
                        log.MakeLog(string.Format("{0} is not a duplicate, adding to database.", p.Name), true);
                        packagefiles.Add(new PackageFile { Name = p.Name, Location = p.FullName, Status = "Pending"});
                    }
                });
                UpdateProgressBar("package files", "Sorting");
            }, token);


            task2.Wait();
            task2.Dispose();
            task3.Wait();
            task3.Dispose();
            task5.Wait();
            task5.Dispose();
            task6.Wait();
            task6.Dispose();
            task7.Wait();
            task7.Dispose();
            task8.Wait();
            task8.Dispose();
            task9.Wait();  
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
            task10.Wait();
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
            getPackages.Wait();            
            new Thread(() => RunUpdateElapsed(sw)) {IsBackground = true}.Start();
            new Thread(() => RunUpdateProgressBar()) {IsBackground = true}.Start();
            Task ReadPackages = Task.Run(() => {                
                if (GlobalVariables.debugMode == true){
                    foreach (PackageFile p in allp){
                        if (token.IsCancellationRequested)
                        {
                            log.MakeLog("Process cancelled.", true);
                            stop = true;
                            return;
                        }
                        currentpackage = p.Name;
                        Task read = Task.Run(() => {
                            initialprocess.CheckThrough(p.Location);
                        });
                        read.Wait();                        
                    };
                } else {
                    Parallel.ForEach(allp, p => {
                        if (token.IsCancellationRequested)
                        {
                            log.MakeLog("Process cancelled.", true);
                            stop = true;
                            return;
                        }
                        currentpackage = p.Name;
                        initialprocess.CheckThrough(p.Location);
                    });
                }
            }, token);
            ReadPackages.Wait();
        }

        private void GetResults(){
            Dispatcher.Invoke(new Action(() => {
                ResultsWindow resultsWindow = new ResultsWindow();
                resultsWindow.Show();
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
            Dispatcher.Invoke(new Action(() => textCurrentPk.Text = string.Format("{0}/{1} - {2} {3}", countprogress, maxi, verb, name)));
            Dispatcher.Invoke(new Action(() => mainProgressBar.Value++));
        }

        private async void RunUpdateProgressBar(){            
            while (runprogress == true)
            {   
                if (GlobalVariables.packagesRead >= 0 && (!String.IsNullOrWhiteSpace(currentpackage))){
                    new Thread(() => AutoUpdateProgressBar()) {IsBackground = true}.Start();
                }
                else 
                {
                    Dispatcher.Invoke(new Action(() => textCurrentPk.Text = string.Format("Preparing to read {0} packages.", maxi)));
                }         
                
            }
        }

        public void AutoUpdateProgressBar(){
            //for the reading of the packages
            Dispatcher.Invoke(new Action(() => textCurrentPk.Text = string.Format("{0}/{1} - {2}", GlobalVariables.packagesRead, maxi, currentpackage)));
            Dispatcher.Invoke(new Action(() => mainProgressBar.Value = GlobalVariables.packagesRead));
        }

        public void UpdateElasped(Stopwatch sw){
            //log.MakeLog("Running update elapsed.", true);
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
            log.MakeLog("Running run update elapsed.", true);
            Dispatcher.Invoke(new Action(() => timeRemaining.Visibility = Visibility.Visible));
            while (runprogress == true)
            {
                if (GlobalVariables.packagesRead != 0){
                    new Thread(() => UpdateElasped(sw)) {IsBackground = true}.Start();
                }
            }
        }


        #endregion
              
      
        private void ManageOldFolder(){
            
        }

        private void testbutton_Click(object sender, EventArgs e) {
            statement = "Dev test button clicked.";
            log.MakeLog(statement, true);
                      

        }        
    }
}
