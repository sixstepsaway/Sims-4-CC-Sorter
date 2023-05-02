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
        private IProgress<double> _progress;
        private int _jobsFinished = 0;
        private int _totalJobs = 1000;
        ResultsWindow resultsWindow = new ResultsWindow();
        S2PackageSearch s2packs = new S2PackageSearch();
        LoggingGlobals log = new LoggingGlobals();
        GlobalVariables globalVars = new GlobalVariables();
        InitialProcessing initialprocess = new InitialProcessing();
        ParallelOptions parallelSettings = new ParallelOptions() { MaxDegreeOfParallelism = 200};
        List<Task> TaskList = new List<Task>();
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

        #region Taskworkers 

        

        #endregion   
        
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




        #region Management Area

        private void ManageOldFolder_Click(object sender, EventArgs e) {
            if (SelectedFolder == "") {
                System.Windows.Forms.MessageBox.Show("Please select the folder containing your package files.");
            } else {
                log.MakeLog("Managing old folder.", true);
                ManageOldFolder();
            }            
        }

        private void SortNewFolder_Click(object sender, EventArgs e) {
            if (SelectedFolder == "") {
                System.Windows.Forms.MessageBox.Show("Please select the folder containing your package files.");
            } else {
                log.MakeLog("Sorting new folder.", true);
                //ManageOldFolder();
            }   
        }

        public static int progresstracker = 0;

        public static int maxi = 0;
        //MainWindow window = new MainWindow();

        public void ManageOldFolder(){
            log.MakeLog("Checking for broken packages.", true);
            completionAlert.Visibility = Visibility.Visible;
            completionAlertValue("Checking for broken packages.");
            mainProgressBar.Visibility = Visibility.Visible;
            BrokenChecks();
            log.MakeLog("Broken check complete.", true);
            mainProgressBar.Value = 0;
            
            completionAlertValue("Identifying package versions.");
            log.MakeLog("Identifying game.", true);
            GameChecks();
            log.MakeLog("Matched all packages to games.", true);
            mainProgressBar.Value = 0;

            completionAlertValue("Searching Sims 2 packages for details.");
            log.MakeLog("Parsing Sims 2 packages.", true);
            Sims2Info();
            log.MakeLog("Done parsing Sims 2 packages.", true);     
            mainProgressBar.Value = 0;   


            //await Task.WhenAll(brokenPacks, packagesGames, sims2checker);
            completionAlertValue("Done!");
            mainProgressBar.Value = maxi;
            //resultsWindow.Show();
            //this.Hide();
            
        }

        public async void BrokenChecks(){
            log.MakeLog("Running FindBrokenPackageFiles", true);
            await Task.Run(new Action(FindBrokenPackageFiles));
        }

        public async void GameChecks(){
            log.MakeLog("Running MatchPackagesToGames", true);
            await Task.Run(new Action(MatchPackagesToGames));
        }

        public async void Sims2Info(){
            log.MakeLog("Running GoThroughSims2Packages", true);
            await Task.Run(new Action(GoThroughSims2Packages));
        }

        public void FindBrokenPackageFiles(){        
            log.MakeLog("[FBPF] Setting maximum.", true);         
            maxi = GlobalVariables.justPackageFiles.Count;
            log.MakeLog("[FBPF] Setting progress bar maximum.", true); 
            mainProgressBar.Maximum = maxi;    
            log.MakeLog("[FBPF] Running through files.", true);        
            Task task1 = Task.Run(() => Parallel.For(0, GlobalVariables.justPackageFiles.Count, i => {                
                var file = (GlobalVariables.justPackageFiles[i]).FullName;
                log.MakeLog("[FBPF] Checking " + file, true);
                progresstracker++;
                initialprocess.FindBrokenPackages(file);  
                this.Dispatcher.Invoke(new Action(() => mainProgressBar.Value++));
            }));
            log.MakeLog("[FBPF] Done! Maxing out progress bar.", true);
            mainProgressBar.Value = maxi;
        }

        public void MatchPackagesToGames(){
            log.MakeLog("[MPTG] Setting maximum.", true);         
            maxi = GlobalVariables.workingPackageFiles.Count;
            log.MakeLog("[MPTG] Setting progress bar maximum.", true); 
            mainProgressBar.Maximum = maxi;    
            log.MakeLog("[MPTG] Running through files.", true);     
            Task task1 = Task.Run(() => Parallel.For(0, GlobalVariables.workingPackageFiles.Count, i => {                
                var file = (GlobalVariables.workingPackageFiles[i]).Location;
                log.MakeLog("[MPTG] Checking " + file, true);
                initialprocess.IdentifyGames(file);
                this.Dispatcher.Invoke(new Action(() => mainProgressBar.Value++));
            }));
            log.MakeLog("[MPTG] Game identification complete.", true); 
            mainProgressBar.Value = 0;
        }

        public void GoThroughSims2Packages(){
            log.MakeLog("[GTS2] Setting maximum.", true);         
            maxi = GlobalVariables.gamesPackages.Count;
            log.MakeLog("[GTS2] Setting progress bar maximum.", true); 
            mainProgressBar.Maximum = maxi;    
            log.MakeLog("[GTS2] Running through files.", true);     
            Task task1 = Task.Run(() => Parallel.For(0, GlobalVariables.gamesPackages.Count, i => {
                var file = (GlobalVariables.gamesPackages[i]).Location;
                log.MakeLog("[GTS2] Checking " + file, true);
                if (GlobalVariables.gamesPackages[i].Game == 2) {
                    s2packs.SearchS2Packages(file);
                }                
                this.Dispatcher.Invoke(new Action(() => mainProgressBar.Value++));
            }));
        }
        
        private void SortNewFolder(){
            
        }

        #endregion


















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

        
              
        
        
        
        private void testbutton_Click(object sender, EventArgs e) {
            statement = "Dev test button clicked.";
            log.MakeLog(statement, true);            
        }        
    }
}
