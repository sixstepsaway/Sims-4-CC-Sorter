/*

Code cannibalized and learned from has been acquired in a number of places. Top credits:

- TASKS: https://stackoverflow.com/questions/1333058/how-to-wait-correctly-until-backgroundworker-completes/41765420#41765420

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
using SimsCCManager.Search_All;
using SimsCCManager.App.Controls;
using SimsCCManager.Packages.Sims2Search;
using SimsCCManager.Packages.Containers;
using SimsCCManager.SortingUIFunctions;

namespace Sims_CC_Sorter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class SplashScreen : Window    
    {
        

    }

    public partial class MainWindow : Window    

    {
        ResultsWindow resultsWindow = new ResultsWindow();
        LoggingGlobals log = new LoggingGlobals();
        GlobalVariables globalVars = new GlobalVariables();
        InitialProcessing initialprocess = new InitialProcessing();
        ParallelOptions parallelSettings = new ParallelOptions() { MaxDegreeOfParallelism = 200};
        private ObservableCollection<Task> s2tasks = new ObservableCollection<Task>();
        S2PackageSearch s2packs = new S2PackageSearch();
        public Stopwatch sw = new Stopwatch();
        string SelectedFolder = "";
        string statement = "";
        int gameNum = 0;

        public MainWindow()
        {    
            InitializeComponent();
            if (GlobalVariables.debugMode) {
                testButton.Visibility = Visibility.Visible;
            } else {
                testButton.Visibility = Visibility.Hidden;
            }
        }

        private void App_Loaded(object sender, RoutedEventArgs e){
             
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            statement = "Closing application.";
            log.MakeLog(statement, false);
            System.Windows.Application.Current.Shutdown();
        }

        public void LocationBoxValue(string value) {
            PickedLocation.Text = value;
        }

        public void completionAlertValue(string value) {
            completionAlert.Text = value;
        }

        private void browseLocation_Click(object sender, EventArgs e) {
            using(var GetFolder = new FolderBrowserDialog())
            {
                DialogResult result = GetFolder.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK) {
                    SelectedFolder = GetFolder.SelectedPath;
                    globalVars.Initialize(gameNum, SelectedFolder);
                    LocationBoxValue(GlobalVariables.ModFolder);
                    statement = "Application initiated. ModFolder found at " + GlobalVariables.ModFolder;
                    log.MakeLog(statement, false); 
                    if (GlobalVariables.debugMode) {
                        statement = "Application running in debug mode.";
                        log.MakeLog(statement, true);
                    } else {
                        statement = "Application is not running in debug mode.";
                        log.MakeLog(statement, true);
                    }
                    initialprocess.IdentifyPackages();                    
                } else {
                    LocationBoxValue(SelectedFolder);
                }
            }
        }        

        public void IdentifyPacks(){
            foreach (FileInfo file in GlobalVariables.PackageFiles) {
                Task t = new Task(() => {
                    log.MakeLog("Adding identification task to tasks.", true);
                    initialprocess.IdentifyGames(file);
                });
                s2tasks.Add(t);
            }
        }

        private void findBroken_Click(object sender, EventArgs e) {            
            if (SelectedFolder == "") {
                System.Windows.Forms.MessageBox.Show("Please select the folder containing your package files.");
            } else {                
                checkGame();
                if (gameNum is 0){
                    System.Windows.Forms.MessageBox.Show("Please specify the game you are running and try again.");
                    statement = "Game not specified.";
                    log.MakeLog(statement, false);
                } else {
                    statement = "Searching for broken and incorrect packages packages.";
                    log.MakeLog(statement, false);
                    string[] files = Directory.GetFiles(GlobalVariables.ModFolder, "*.*", SearchOption.AllDirectories);
                    var sw = Stopwatch.StartNew();
                    IdentifyPacks();
                    if (GlobalVariables.debugMode == true)
                    {                        
                        foreach (string file in files) {
                            FileInfo package = new FileInfo(file);
                            initialprocess.FindBrokenPackages(package);
                        }
                    }
                    else 
                    {
                        Parallel.ForEach(files, parallelSettings, file => 
                        {
                            FileInfo package = new FileInfo(file);
                            initialprocess.FindBrokenPackages(package);
                        });
                    }
                    sw.Stop();                    
                    log.MakeLog("Processing took " + sw.Elapsed.TotalSeconds.ToString("#,##0.00 'seconds'"), true);
                    completionAlertValue("Processing took " + sw.Elapsed.TotalSeconds.ToString("#,##0.00 'seconds'"));
                }
            }
            statement = "Search for broken packages complete.";
            log.MakeLog(statement, false);
            completionAlertValue("Search completed."); 
        }
        private BackgroundWorker s2worker = new BackgroundWorker();
        private void renameSims2Packages_Click(object sender, EventArgs e) {
            if (SelectedFolder == "") {
                System.Windows.Forms.MessageBox.Show("Please select the folder containing your package files.");
            } else {
                sw.Start();
                log.MakeLog("Searching through Sims 2 packages.", false);                
                completionAlertValue("Searching for sims 2 packages.");
                s2tasks.CollectionChanged += s2tasks_CollectionChanged;
                s2worker.WorkerReportsProgress = true;
                s2worker.WorkerSupportsCancellation = true;
                s2worker.ProgressChanged += s2worker_ProgressChanged;
                s2worker.DoWork += s2worker_DoWork;
                s2worker.RunWorkerCompleted += s2worker_RunWorkerCompleted;
                IdentifyPacks();
                s2gothroughpackages();
            }
        }

        void s2worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (s2tasks.Count != 0)
            {   
                log.MakeLog("Running tasks.", true);
                s2worker.RunWorkerAsync();
            } else {
                sw.Stop();
                completionAlertValue("Sims 2 package processing took " + sw.Elapsed.TotalSeconds.ToString("#,##0.00 'seconds'"));
                log.MakeLog("Sims 2 package processing took " + sw.Elapsed.TotalSeconds.ToString("#,##0.00 'seconds'"), true);
                sw.Reset();
                mainProgressBar.Value = 100;
                resultsWindow.Show();
            }
        }

        private void s2gothroughpackages(){
            foreach (PackageFile package in GlobalVariables.AllPackages)
            {
                if (package.Game == 2){
                    Task t = new Task(() => {
                        log.MakeLog("Adding sims 2 parse task to tasks.", true);
                        s2packs.SearchS2Packages(package.Location);
                    });
                    s2tasks.Add(t);
                }
            }          
        }
        
        private void s2worker_DoWork(object sender, DoWorkEventArgs e){  
            log.MakeLog("Doing work.", true); 
            int i = 0;         
            try
            {
                foreach (Task t in s2tasks)
                {                    
                    t.RunSynchronously();
                    s2tasks.Remove(t);
                    i++;
                }
            }
            catch
            {
                i = 0;
                log.MakeLog("Cancelling async.", true);
                s2worker.CancelAsync();
            }
        }
        void s2tasks_CollectionChanged(object sender,
        System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            log.MakeLog("Collection has changed.", true);
            if (!s2worker.IsBusy)
            {
                s2worker.RunWorkerAsync();
            }
        }

        private void s2worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            log.MakeLog("Progress has changed.", true);
            mainProgressBar.Value = e.ProgressPercentage;
        }

        private void checkGame() {
            statement = "Checking which game is ticked.";
            log.MakeLog(statement, true);
            if ((bool)radioButton_Sims2.IsChecked) {
                statement = "Sims 2 picked.";
                log.MakeLog(statement, true);
                gameNum = 2;
                //get info
            } else if ((bool)radioButton_Sims3.IsChecked) {
                statement = "Sims 3 picked.";
                log.MakeLog(statement, true);
                gameNum = 3;
                //get info
            } else if ((bool)radioButton_Sims4.IsChecked) {
                statement = "Sims 4 picked.";
                log.MakeLog(statement, true);
                gameNum = 4;
                //get info
            } else {
                System.Windows.Forms.MessageBox.Show("Please select a game.");
                statement = "No game picked.";
                log.MakeLog(statement, true);
            }
        }

        public void changeTestText(string text){
            statement = "Changing the report text box.";
            log.MakeLog(statement, true);
            completionAlertValue(text);
        }

        private void testbutton_Click(object sender, EventArgs e) {
            statement = "Dev test button clicked.";
            log.MakeLog(statement, true);
            
        }        
    }
}
