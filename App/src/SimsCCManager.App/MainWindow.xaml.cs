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
using System.IO;
using System.Diagnostics;
using SSAGlobals;
using SimsCCManager.Packages.Initial;
using SimsCCManager.Search_All;
using SimsCCManager.App.Controls;
using SimsCCManager.Packages.Sims2Search;
using SimsCCManager.Packages.Containers;

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
        LoggingGlobals log = new LoggingGlobals();
        GlobalVariables globalVars = new GlobalVariables();
        InitialProcessing initialprocess = new InitialProcessing();
        ParallelOptions parallelSettings = new ParallelOptions() { MaxDegreeOfParallelism = 200};
        S2PackageSearch s2packs = new S2PackageSearch();
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
            if (GlobalVariables.debugMode == true)
            {                        
                foreach (FileInfo file in GlobalVariables.PackageFiles) {
                    initialprocess.IdentifyGames(file);
                }
            }
            else 
            {
                Parallel.ForEach(GlobalVariables.PackageFiles, parallelSettings, file => 
                {
                    initialprocess.IdentifyGames(file);
                });
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

        private void renameSims2Packages_Click(object sender, EventArgs e) {
            if (SelectedFolder == "") {
                System.Windows.Forms.MessageBox.Show("Please select the folder containing your package files.");
            } else {
                completionAlertValue("Searching for sims 2 packages.");
                Thread t = new Thread( S2PackThread );
                t.IsBackground = true;
                t.Start();
            }
        }

        private void S2PackThread(){
            log.MakeLog("Searching through Sims 2 packages.", false);
            var sw = Stopwatch.StartNew();
            IdentifyPacks();
            if (GlobalVariables.debugMode) {
                foreach (PackageFile package in GlobalVariables.AllPackages){
                    if (package.Game == 2){
                        s2packs.SearchS2Packages(package.Location);
                    }
                }
            } else {
                Parallel.ForEach(GlobalVariables.AllPackages, parallelSettings, package =>
                {
                    if (package.Game == 2){
                        s2packs.SearchS2Packages(package.Location);
                    }
                });                    
            }
            sw.Stop();                    
            log.MakeLog("Sims 2 package processing took " + sw.Elapsed.TotalSeconds.ToString("#,##0.00 'seconds'"), true);
            completionAlertValue("Sims 2 package processing took " + sw.Elapsed.TotalSeconds.ToString("#,##0.00 'seconds'"));
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
