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
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
            if (File.Exists(SaveData.mainSaveData)) 
            {
                LoadButton.Visibility = Visibility.Visible;
            } else {
                LoadButton.Visibility = Visibility.Collapsed;
            }

        }

        #region Load 

        private void loadData_Click(object sender, RoutedEventArgs e){
            
            using (StreamReader file = File.OpenText(SaveData.mainSaveData))
            {
                JsonSerializer serializer = new JsonSerializer();
                GlobalVariables.loadedData = (List<SimsPackage>)serializer.Deserialize(file, typeof(List<SimsPackage>));
            }
            GlobalVariables.loadedSaveData = true;
            GetResults();
        }

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

        private async Task ManageOldFolder(){
            MainWindow window = new MainWindow();
            log.MakeLog("Checking for broken packages.", true);
            completionAlert.Visibility = Visibility.Visible;
            
            completionAlertValue("Checking for broken packages.");
            int i = 0;
            mainProgressBar.Visibility = Visibility.Visible;
            int maxi = GlobalVariables.justPackageFiles.Count; 
            mainProgressBar.Maximum = maxi;     
            textCurrentPk.Visibility = Visibility.Visible;
            Task task1 = Task.Run(() => Parallel.For(0, GlobalVariables.justPackageFiles.Count, i => {                
                var file = (GlobalVariables.justPackageFiles[i]).FullName;
                log.MakeLog("Checking " + file, true);
                progresstracker++;
                initialprocess.FindBrokenPackages(file);  
                window.Dispatcher.Invoke(new Action(() => mainProgressBar.Value++));
                window.Dispatcher.Invoke(new Action(() => textCurrentPk.Text = "Reading " + GlobalVariables.justPackageFiles[i].Name));
            }));
            log.MakeLog("Awaiting finding broken packages to finish.", true);
            await(task1);
            log.MakeLog("Broken check complete.", true);
            mainProgressBar.Value = 0;
            completionAlertValue("Identifying package versions.");
            log.MakeLog("Identifying game.", true);
            maxi = GlobalVariables.workingPackageFiles.Count;
            mainProgressBar.Maximum = maxi;
            Task task2 = Task.Run(() => Parallel.For(0, GlobalVariables.workingPackageFiles.Count, i => {                
                var file = (GlobalVariables.workingPackageFiles[i]).Location;
                log.MakeLog("Checking " + file, true);
                initialprocess.IdentifyGames(file);
                window.Dispatcher.Invoke(new Action(() => mainProgressBar.Value++));
                window.Dispatcher.Invoke(new Action(() => textCurrentPk.Text = "Reading " + GlobalVariables.workingPackageFiles[i].Name));
            }));
            log.MakeLog("Awaiting game ID to finish.", true);
            await(task2);
            log.MakeLog("Game identification complete.", true); 
            mainProgressBar.Value = 0;
            completionAlertValue("Searching packages for details.");
            log.MakeLog("Parsing Sims 2 packages.", true);
            maxi = GlobalVariables.gamesPackages.Count;
            mainProgressBar.Maximum = maxi;
            Task task3 = Task.Run(() => Parallel.For(0, GlobalVariables.gamesPackages.Count, i => {
                var file = (GlobalVariables.gamesPackages[i]).Location;
                log.MakeLog("Checking " + file, true);
                if (GlobalVariables.gamesPackages[i].Game == 2) {
                    s2packs.SearchS2Packages(file);
                }
                window.Dispatcher.Invoke(new Action(() => mainProgressBar.Value++));
                window.Dispatcher.Invoke(new Action(() => textCurrentPk.Text = "Reading " + GlobalVariables.gamesPackages[i].Name));
            }));
            
            log.MakeLog("Awaiting Sims 2 package reading to finish.", true);
            await(task3);
            completionAlertValue("Done!");
            textCurrentPk.Text = "";
            mainProgressBar.Value = maxi;
            GetResults();
        }

        private void GetResults(){
            ResultsWindow resultsWindow = new ResultsWindow();
            resultsWindow.Show();
            //this.Hide();
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
