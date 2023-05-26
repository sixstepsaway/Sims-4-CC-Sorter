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
        private int workerThreads;
        private int portThreads;
        public int countprogress = 0;
        private int batchsize = 100;
        private int totalbatches = 0;
        int threads = Environment.ProcessorCount;
        int threadstouse = 0;

        CancellationTokenSource cts = new CancellationTokenSource();

        public MainWindow()
        {               
            InitializeComponent();
            
            if (eatenturecpu == true){
                threadstouse = threads - 2;
            } else {
                threadstouse = (threads - 2) / 2;
            }
            parallelSettings.MaxDegreeOfParallelism = threadstouse;
            if (GlobalVariables.debugMode) {
                testButton.Visibility = Visibility.Visible;
            } else {
                testButton.Visibility = Visibility.Hidden;
            }            
            if (dataExists == true) 
            {
                LoadButton.Visibility = Visibility.Visible;
            } else {
                LoadButton.Visibility = Visibility.Collapsed;
            }
        }


        #region Load 

        private void loadData_Click(object sender, RoutedEventArgs e){
            GetResults();
        }

        #endregion   

        private void noeatcpu_Check(object sender, RoutedEventArgs e){
            eatenturecpu = false;
            threadstouse = threads / 2;
            parallelSettings.MaxDegreeOfParallelism = threadstouse;
        }
        private void eatcpu_Uncheck(object sender, RoutedEventArgs e){
            eatenturecpu = true;
            threadstouse = threads;
            parallelSettings.MaxDegreeOfParallelism = threadstouse;
        }        
        
        private void App_Loaded(object sender, RoutedEventArgs e){
             
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
            while (stop == false){
                Thread.Sleep(1);
            }            
            // Cancellation should have happened, so call Dispose.
            
            ProgressGrid.Visibility = Visibility.Hidden;
            MainMenuGrid.Visibility = Visibility.Visible;
            
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
            PickedLocation.Text = value;
        }

        public void completionAlertValue(string value) {
            completionAlert.Text = value;
        }




        #region Management Area

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
                    FoundPastItems.Visibility = Visibility.Visible;
                    ContinueQuestion.Visibility = Visibility.Visible;
                } else {
                    SortNewPrep();
                }                
            }   
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


        private void ContinueSearch_Click(object sender, EventArgs e) {
            ContinueQuestion.Visibility = Visibility.Hidden;
            FindNewItemsQuestion.Visibility = Visibility.Visible;
            ContinuePrevious = true;            
        }
        private void RestartSearch_Click(object sender, EventArgs e) {
            ContinuePrevious = false;
            FoundPastItems.Visibility = Visibility.Hidden;
            SortNewPrep();
        }
        private void CancelSearch_Click(object sender, EventArgs e) {
            FoundPastItems.Visibility = Visibility.Hidden;
        }
        private void YesFindNewItems_Click(object sender, EventArgs e) {
            ContinueQuestion.Visibility = Visibility.Hidden;
            FindNewItemsQuestion.Visibility = Visibility.Hidden;
            FoundPastItems.Visibility = Visibility.Hidden;
            FindNewAdditions = true;
            SortNewFolder(cts.Token);
        }
        private void NoDontFindNew_Click(object sender, EventArgs e) {
            ContinueQuestion.Visibility = Visibility.Hidden;
            FindNewItemsQuestion.Visibility = Visibility.Hidden;
            FoundPastItems.Visibility = Visibility.Hidden;
            FindNewAdditions = false;
            SortNewFolder(cts.Token);
        }       


        public static int progresstracker = 0;

        public static int maxi = 0;

        private void SortNewPrep(){
            log.MakeLog("Prepping to sort folder.", true);

            GlobalVariables.DatabaseConnection.Close();
            globalVars.ConnectDatabase(true);
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
            SortNewFolder(cts.Token);
        }

        private int CountItems(string type) {
            log.MakeLog("Checking count of database.", true);
            int value;
            string cmdtext = string.Format("SELECT count(*) FROM AllFiles where type = '{0}'", type);
            var sqm = GlobalVariables.DatabaseConnection.CreateCommand(cmdtext);
            value = sqm.ExecuteScalar<Int32>();
            log.MakeLog("Counted! Returning.", true);
            return value;
        }

        private async Task SortNewFolder(object obj){ 
            CancellationToken token = (CancellationToken)obj;           
            MainWindow window = new MainWindow();
            sw.Start();           
        
            ProgressGrid.Visibility = Visibility.Visible;
            MainMenuGrid.Visibility = Visibility.Hidden;
            completionAlert.Visibility = Visibility.Visible;
            
            mainProgressBar.Visibility = Visibility.Visible;
            if (ContinuePrevious == true){
                globalVars.ConnectDatabase(false);
            }

            if (ContinuePrevious == false || FindNewAdditions == true){
                string[] filesS = Directory.GetFiles(GlobalVariables.ModFolder, "*", SearchOption.AllDirectories);
                log.MakeLog("Sorting packages files from non-package files.", true);
                int maxi = filesS.Length;             
                mainProgressBar.Value = 0;
                mainProgressBar.Maximum = maxi;
                textCurrentPk.Visibility = Visibility.Visible;
                completionAlertValue("Sorting package files from non-package files.");
                int countprogress = 0;
                List<FileInfo> files = new List<FileInfo>();

                List<FileInfo> sims3pack = new List<FileInfo>();
                List<FileInfo> sims2pack = new List<FileInfo>();
                List<FileInfo> sims4script = new List<FileInfo>();
                List<FileInfo> other = new List<FileInfo>();
                List<FileInfo> packages = new List<FileInfo>();
                List<FileInfo> compressed = new List<FileInfo>();
                List<FileInfo> trayitems = new List<FileInfo>();

                log.MakeLog("Getting packagesPending.", true);
                var packagesPending = GlobalVariables.DatabaseConnection.Query<PackageFile>("SELECT * FROM Processing_Reader where Status = 'Pending'");
                log.MakeLog("Getting packagesProcessing.", true);
                var packagesProcessing = GlobalVariables.DatabaseConnection.Query<PackageFile>("SELECT * FROM Processing_Reader where Status = 'Processing'");
                log.MakeLog("Getting packagesDone.", true);
                var packagesDone = GlobalVariables.DatabaseConnection.Query<SimsPackage>("SELECT * FROM Packages");
                log.MakeLog("Getting notpack.", true);
                var notpack = GlobalVariables.DatabaseConnection.Query<AllFiles>("SELECT * FROM AllFiles");

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
                        log.MakeLog(string.Format("Adding {0} to list.", file), true);
                        c++;
                    }
                });
                await(task1);
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
                });
                await(task2);

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
                });
                await(task3);

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
                });
                await(task5);

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
                });
                await(task6);

                Task task7 = Task.Run(() => {
                    if (token.IsCancellationRequested)
                    {
                        log.MakeLog("Process cancelled.", true);
                        stop = true;
                        return;
                    }
                    log.MakeLog("Finding compressed files.", true);
                    var traystuff = files.Where(x => x.Extension == ".householdbinary" || x.Extension == ".trayitem" || x.Extension == ".hhi" || x.Extension == ".sgi");
                    trayitems.AddRange(traystuff);
                });
                await(task7);

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
                });
                await(task8);
                    
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
                });
                await(task9);

                task1.Dispose();
                task2.Dispose();
                task3.Dispose();
                //task4.Dispose();
                task5.Dispose();
                task6.Dispose();
                task7.Dispose();
                task8.Dispose();
                task9.Dispose();
                
                ConcurrentQueue<Task> IdentifyPackagesList = new ConcurrentQueue<Task>();
                List<AllFiles> allfiles = new List<AllFiles>();
                countprogress = 0;
                
                foreach (FileInfo s3 in sims3pack){
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
                        IdentifyPackagesList.Enqueue(Task.Run(() => {
                            allfiles.Add(new AllFiles { Name = s3.Name, Location = s3.FullName, Type = "sims3pack", Status = "Fine"});
                            window.Dispatcher.Invoke(new Action(() => textCurrentPk.Text = string.Format("{0}/{1} - Sorting {2}", countprogress, maxi, s3.Name)));
                            log.MakeLog(string.Format("{0}/{1} - Sorting {2}", countprogress, maxi, s3.Name), true);
                            window.Dispatcher.Invoke(new Action(() => countprogress++));
                            window.Dispatcher.Invoke(new Action(() => mainProgressBar.Value++));
                        }));
                        
                    }
                }
                
                foreach (FileInfo s2 in sims2pack){  
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
                        IdentifyPackagesList.Enqueue(Task.Run(() => {
                            allfiles.Add(new AllFiles { Name = s2.Name, Location = s2.FullName, Type = "sims2pack", Status = "Fine"});                        
                            window.Dispatcher.Invoke(new Action(() => textCurrentPk.Text = string.Format("{0}/{1} - Sorting {2}", countprogress, maxi, s2.Name)));
                            log.MakeLog(string.Format("{0}/{1} - Sorting {2}", countprogress, maxi, s2.Name), true);
                            window.Dispatcher.Invoke(new Action(() => countprogress++));
                            window.Dispatcher.Invoke(new Action(() => mainProgressBar.Value++));
                        }));  
                    }
                }
                
                
                foreach (FileInfo t4 in sims4script){  
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
                        IdentifyPackagesList.Enqueue(Task.Run(() => {
                            allfiles.Add(new AllFiles { Name = t4.Name, Location = t4.FullName, Type = "ts4script", Status = "Fine"});  
                            window.Dispatcher.Invoke(new Action(() => textCurrentPk.Text = string.Format("{0}/{1} - Sorting {2}", countprogress, maxi, t4.Name)));
                            log.MakeLog(string.Format("{0}/{1} - Sorting {2}", countprogress, maxi, t4.Name), true);
                            window.Dispatcher.Invoke(new Action(() => countprogress++));
                            window.Dispatcher.Invoke(new Action(() => mainProgressBar.Value++));
                        }));
                    }
                }
                
                foreach (FileInfo c in compressed){ 
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
                        IdentifyPackagesList.Enqueue(Task.Run(() => {
                            allfiles.Add(new AllFiles { Name = c.Name, Location = c.FullName, Type = "compressed file", Status = "Fine"});
                            window.Dispatcher.Invoke(new Action(() => textCurrentPk.Text = string.Format("{0}/{1} - Sorting {2}", countprogress, maxi, c.Name)));
                            log.MakeLog(string.Format("{0}/{1} - Sorting {2}", countprogress, maxi, c.Name), true);
                            window.Dispatcher.Invoke(new Action(() => countprogress++));
                            window.Dispatcher.Invoke(new Action(() => mainProgressBar.Value++));
                        }));
                    }
                }                    
                foreach (FileInfo o in other){  
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
                        IdentifyPackagesList.Enqueue(Task.Run(() => {
                            allfiles.Add(new AllFiles { Name = o.Name, Location = o.FullName, Type = "other", Status = "Fine"});           
                            window.Dispatcher.Invoke(new Action(() => textCurrentPk.Text = string.Format("{0}/{1} - Sorting {2}", countprogress, maxi, o.Name)));
                            log.MakeLog(string.Format("{0}/{1} - Sorting {2}", countprogress, maxi, o.Name), true);
                            window.Dispatcher.Invoke(new Action(() => countprogress++));
                            window.Dispatcher.Invoke(new Action(() => mainProgressBar.Value++));
                        }));
                    }
                }
                foreach (FileInfo p in packages){ 
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
                        IdentifyPackagesList.Enqueue(Task.Run(() => {                               
                            window.Dispatcher.Invoke(new Action(() => countprogress++));
                            window.Dispatcher.Invoke(new Action(() => textCurrentPk.Text = string.Format("{0}/{1} - Sorting {2}", countprogress, maxi, p.Name)));
                            log.MakeLog(string.Format("{0}/{1} - Sorting {2}", countprogress, maxi, p.Name), true);
                            window.Dispatcher.Invoke(new Action(() => mainProgressBar.Value++));
                            allfiles.Add(new AllFiles { Name = p.Name, Location = p.FullName, Type = "package", Status = "Fine"});
                        }));
                    }
                }
                foreach (FileInfo t in trayitems){   
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
                        IdentifyPackagesList.Enqueue(Task.Run(() => {                               
                            window.Dispatcher.Invoke(new Action(() => countprogress++));
                            window.Dispatcher.Invoke(new Action(() => textCurrentPk.Text = string.Format("{0}/{1} - Sorting {2}", countprogress, maxi, t.Name)));
                            log.MakeLog(string.Format("{0}/{1} - Sorting {2}", countprogress, maxi, t.Name), true);
                            window.Dispatcher.Invoke(new Action(() => mainProgressBar.Value++));
                            allfiles.Add(new AllFiles { Name = t.Name, Location = t.FullName, Type = "tray file", Status = "Fine"});
                        }));
                    }
                }
                
                
                
                await Task.WhenAll(IdentifyPackagesList);
                foreach (Task t in IdentifyPackagesList){
                    t.Dispose();
                }

                log.MakeLog(string.Format("There are {0} items in allfiles. Preparing to add to database.", allfiles.Count), true);

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
                    } catch (SQLite.SQLiteException e) {
                        Console.WriteLine(string.Format("Inserting sorted files to database failed. Exception: {0}", e.Message));
                    }                    
                });
                await(task10);
                task10.Dispose();

                log.MakeLog(string.Format("sims3packs found: {0}", CountItems("sims3pack")), true);
                log.MakeLog(string.Format("sims3packs found: {0}", CountItems("sims2pack")), true);
                log.MakeLog(string.Format("sims3packs found: {0}", CountItems("ts4script")), true);
                log.MakeLog(string.Format("sims3packs found: {0}", CountItems("compressed file")), true);
                log.MakeLog(string.Format("sims3packs found: {0}", CountItems("other")), true);
                log.MakeLog(string.Format("sims3packs found: {0}", CountItems("package")), true);
                files = new List<FileInfo>();

                sims3pack = new List<FileInfo>();
                sims2pack = new List<FileInfo>();
                sims4script = new List<FileInfo>();
                other = new List<FileInfo>();
                packages = new List<FileInfo>();
                compressed = new List<FileInfo>();
                trayitems = new List<FileInfo>();
                allfiles = new List<AllFiles>();
                //GlobalVariables.DatabaseConnection.Commit();
            
                

                List<AllFiles> allp = new List<AllFiles>();                
                var dbqc = GlobalVariables.DatabaseConnection.Query<AllFiles>("SELECT * FROM AllFiles where Type = 'package' ORDER BY Name ASC");
                allp.AddRange(dbqc);                
                
                maxi = allp.Count;
                mainProgressBar.Maximum = maxi;
                mainProgressBar.Value = 0;
                completionAlertValue("Identifying packages.");
                countprogress = 0;

                List<Task> FindBrokenList = new List<Task>();
                

                foreach (AllFiles file in allp){
                    if (token.IsCancellationRequested)
                    {
                        log.MakeLog("Process cancelled.", true);
                        stop = true;
                        return;
                    }
                    FindBrokenList.Add(Task.Run(() => {
                        window.Dispatcher.Invoke(new Action(() => countprogress++));
                        log.MakeLog(string.Format("{0}/{1} - Checking {2}", countprogress, maxi, file.Name), true);
                        window.Dispatcher.Invoke(new Action(() => textCurrentPk.Text = string.Format("{0}/{1} - Reading {2}", countprogress, maxi, file.Name)));                        
                        window.Dispatcher.Invoke(new Action(() => mainProgressBar.Value++));   
                        initialprocess.CheckThrough(file.Location);
                    }));
                }
                while (FindBrokenList.Any())
                {
                    Task finishedTask0 = await Task.WhenAny(FindBrokenList);                    
                    FindBrokenList.Remove(finishedTask0);
                    finishedTask0.Dispose();
                }

                                

                log.MakeLog("Awaiting finding broken packages to finish.", true);
                await Task.WhenAll(FindBrokenList);

                Task task11 = Task.Run(() => { 
                    if (token.IsCancellationRequested)
                    {
                        log.MakeLog("Process cancelled.", true);
                        stop = true;
                        return;
                    }                       
                    log.MakeLog("Inserting into database, please wait.", true);
                    try {
                        GlobalVariables.DatabaseConnection.InsertAll(Containers.identifiedPackages);
                    } catch (SQLite.SQLiteException e) {
                        Console.WriteLine(string.Format("Inserting identified files to database failed. Exception: {0}", e.Message));
                    }                    
                });
                await(task11);
                task11.Dispose();
                Containers.identifiedPackages = new SynchronizedCollection<PackageFile>();
                FindBrokenList = new List<Task>();
                log.MakeLog("Broken check complete.", true);
                foreach (Task t in FindBrokenList){
                    t.Dispose();
                }
                //GlobalVariables.DatabaseConnection.Commit();

            }            

            log.MakeLog("Checking count of packages database.", true);
            
            string cmdtext = string.Format("SELECT count(*) FROM Processing_Reader where game = '{0}'", 2);
            var command = GlobalVariables.DatabaseConnection.CreateCommand(cmdtext);
            log.MakeLog(string.Format("There are {0} Sims 2 packages to process.", command.ExecuteScalar<Int32>()), true);
            cmdtext = string.Format("SELECT count(*) FROM Processing_Reader where game = '{0}'", 3);
            command = GlobalVariables.DatabaseConnection.CreateCommand(cmdtext);
            log.MakeLog(string.Format("There are {0} Sims 3 packages to process.", command.ExecuteScalar<Int32>()), true);
            cmdtext = string.Format("SELECT count(*) FROM Processing_Reader where game = '{0}'", 4);
            command = GlobalVariables.DatabaseConnection.CreateCommand(cmdtext);
            log.MakeLog(string.Format("There are {0} Sims 4 packages to process.", command.ExecuteScalar<Int32>()), true);

            List<PackageFile> s2pending = new List<PackageFile>();
            List<PackageFile> s3pending = new List<PackageFile>();
            List<PackageFile> s4pending = new List<PackageFile>();

            var packagesQuery = GlobalVariables.DatabaseConnection.Query<PackageFile>("SELECT * FROM Processing_Reader where Status = 'Pending' ORDER BY Name ASC");

            var pending2 = GlobalVariables.DatabaseConnection.Query<PackageFile>("SELECT * FROM Processing_Reader where Status = 'Pending' AND Game = 2 ORDER BY Name ASC");
            s2pending.AddRange(pending2);
            log.MakeLog(string.Format("Found {0} pending Sims 2 files.", pending2.Count), true);

            var pending3 = GlobalVariables.DatabaseConnection.Query<PackageFile>("SELECT * FROM Processing_Reader where Status = 'Pending' AND Game = 3 ORDER BY Name ASC");
            s3pending.AddRange(pending3);
            log.MakeLog(string.Format("Found {0} pending Sims 3 files.", pending3.Count), true);

            var pending4 = GlobalVariables.DatabaseConnection.Query<PackageFile>("SELECT * FROM Processing_Reader where Status = 'Pending' AND Game = 4 ORDER BY Name ASC");
            s4pending.AddRange(pending4);
            log.MakeLog(string.Format("Found {0} pending Sims 4 files.", pending4.Count), true);

            string packagelogfolder = "packagelogs";
            string logspot = System.IO.Path.Combine(LoggingGlobals.internalLogFolder, packagelogfolder);
            
            if(!Directory.Exists(logspot)) {
                Directory.CreateDirectory(logspot);
            }
            
            SemaphoreSlim maxThread = new SemaphoreSlim(threadstouse);
            mainProgressBar.Value = 0;
            completionAlertValue("Searching packages for details.");
            log.MakeLog("Parsing Sims packages.", true);
            maxi = packagesQuery.Count;
            mainProgressBar.Maximum = maxi;
            countprogress = 0;                
            ConcurrentQueue<Task> ProcessPackages = new ConcurrentQueue<Task>();

            foreach (PackageFile file in pending2) {
                if (token.IsCancellationRequested)
                {
                    log.MakeLog("Process cancelled.", true);
                    stop = true;
                    return;
                }
                log.MakeLog(string.Format("Processing Sims 2 file: {0}", file.Name), true);
                Task task = RunLimitedNumberAtATime(threadstouse, Enumerable.Range(1, 100), x => Task.Factory.StartNew(() => {
                    window.Dispatcher.Invoke(new Action(() => textCurrentPk.Text = string.Format("{0}/{1} - Reading {2}", countprogress, maxi, file.Name)));
                    window.Dispatcher.Invoke(new Action(() => countprogress++));
                    window.Dispatcher.Invoke(new Action(() => mainProgressBar.Value++));
                    s2packs.SearchS2Packages(file.Location);                        
                }, TaskCreationOptions.LongRunning));
            }
            foreach (PackageFile file in pending3) {
                if (token.IsCancellationRequested)
                {
                    log.MakeLog("Process cancelled.", true);
                    stop = true;
                    return;
                }
                log.MakeLog(string.Format("Processing Sims 3 file: {0}", file.Name), true);
                /*ProcessPackages.Enqueue(Task.Run(() => {
                    window.Dispatcher.Invoke(new Action(() => textCurrentPk.Text = string.Format("{0}/{1} - Reading {2}", countprogress, maxi, file.Name)));
                    window.Dispatcher.Invoke(new Action(() => countprogress++));
                    window.Dispatcher.Invoke(new Action(() => mainProgressBar.Value++));
                    s3packs.SearchS3Packages(file.Location);                        
                }));*/                    
            }
            foreach (PackageFile file in pending4) {
                if (token.IsCancellationRequested)
                {
                    log.MakeLog("Process cancelled.", true);
                    stop = true;
                    return;
                }
                log.MakeLog(string.Format("Processing Sims 4 file: {0}", file.Name), true);
                Task task = RunLimitedNumberAtATime(threadstouse, Enumerable.Range(1, 100), x => Task.Factory.StartNew(() => {
                    window.Dispatcher.Invoke(new Action(() => textCurrentPk.Text = string.Format("{0}/{1} - Reading {2}", countprogress, maxi, file.Name)));
                    window.Dispatcher.Invoke(new Action(() => countprogress++));
                    window.Dispatcher.Invoke(new Action(() => mainProgressBar.Value++));
                    s4packs.SearchS4Packages(file.Location, false);                        
                }, TaskCreationOptions.LongRunning));
            }
            
            
            if (!token.IsCancellationRequested) {
                log.MakeLog("Awaiting package reading to finish.", true);
                //await(processpackages);
                completionAlertValue("Done!");
                textCurrentPk.Text = "";
                mainProgressBar.Value = maxi;
                sw.Stop();
                TimeSpan ts = sw.Elapsed;
                string elapsedtime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                    ts.Hours, ts.Minutes, ts.Seconds,
                                    ts.Milliseconds / 10);
                log.MakeLog(string.Format("Processing took {0}", elapsedtime), true);
                GetResults();
            } else {
                cts.Dispose();
                stop = false;
                log.MakeLog("Fully cancelled.", true);
            }
        }
        private void GetResults(){
            ResultsWindow resultsWindow = new ResultsWindow();
            resultsWindow.Show();
            this.Close();
        }
        
        private void ManageOldFolder(){
            
        }

        #endregion

        private void browseLocation_Click(object sender, EventArgs e) {
            using(var GetFolder = new FolderBrowserDialog())
            {
                DialogResult result = GetFolder.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK) {
                    SelectedFolder = GetFolder.SelectedPath;
                    GlobalVariables.ModFolder = SelectedFolder;
                    GlobalVariables.logfile = System.IO.Path.Combine(SelectedFolder, "\\SimsCCSorter.log");
                    LocationBoxValue(GlobalVariables.ModFolder);
                    log.MakeLog(string.Format("Application initiated. ModFolder found at {0}", GlobalVariables.ModFolder), false); 
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

        public static async Task RunLimitedNumberAtATime<T>(int numberOfTasksConcurrent, 
        IEnumerable<T> inputList, Func<T, Task> asyncFunc)
        {
            Queue<T> inputQueue = new Queue<T>(inputList);
            List<Task> runningTasks = new List<Task>(numberOfTasksConcurrent);
            for (int i = 0; i < numberOfTasksConcurrent && inputQueue.Count > 0; i++)
                runningTasks.Add(asyncFunc(inputQueue.Dequeue()));

            while (inputQueue.Count > 0)
            {
                Task task = await Task.WhenAny(runningTasks);
                runningTasks.Remove(task);
                runningTasks.Add(asyncFunc(inputQueue.Dequeue()));
            }

            await Task.WhenAll(runningTasks);
        }

        private void testbutton_Click(object sender, EventArgs e) {
            statement = "Dev test button clicked.";
            log.MakeLog(statement, true);
                      

        }        
    }
}
