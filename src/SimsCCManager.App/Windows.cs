using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using CCSorter.Controls;
using CCSorter.Funcs;
using SSAGlobals;

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
        LoggingGlobals loggingGlobals = new LoggingGlobals();
        GlobalVariables globalVars = new GlobalVariables();
        ControlOverview controls = new ControlOverview();
        ProcessSelectedFolder processFolder = new ProcessSelectedFolder();
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

        private void exitButton_Click(object sender, EventArgs e)
        {
            statement = "Closing application.";
            loggingGlobals.MakeLog(statement, false);
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
                    loggingGlobals.MakeLog(statement, false); 
                    if (GlobalVariables.debugMode) {
                        statement = "Application running in debug mode.";
                        loggingGlobals.MakeLog(statement, false);
                    } else {
                        statement = "Application is not running in debug mode.";
                        loggingGlobals.MakeLog(statement, false);
                    }
                    processFolder.IdentifyPackages();
                } else {
                    LocationBoxValue(SelectedFolder);
                }
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
                    loggingGlobals.MakeLog(statement, false);
                } else {
                    statement = "Searching for broken packages.";
                loggingGlobals.MakeLog(statement, false);
                completionAlertValue("Starting search.");
                    controls.FindPackagesToRemove();
                }                
            }
            statement = "Search for broken packages complete.";
            loggingGlobals.MakeLog(statement, false);
            completionAlertValue("Search completed."); 
        }

        private void renameSims2Packages_Click(object sender, EventArgs e) {
            if (SelectedFolder == "") {
                System.Windows.Forms.MessageBox.Show("Please select the folder containing your package files.");
            } else {    
                completionAlertValue("Renaming packages.");            
                statement = "Renaming Sims 2 packages.";
                loggingGlobals.MakeLog(statement, false);
                controls.RenameS2Packages();
                completionAlertValue("Complete.");             
            }
        }

        private void checkGame() {
            statement = "Checking which game is ticked.";
            loggingGlobals.MakeLog(statement, true);
            if ((bool)radioButton_Sims2.IsChecked) {
                statement = "Sims 2 picked.";
                loggingGlobals.MakeLog(statement, true);
                gameNum = 2;
                //get info
            } else if ((bool)radioButton_Sims3.IsChecked) {
                statement = "Sims 3 picked.";
                loggingGlobals.MakeLog(statement, true);
                gameNum = 3;
                //get info
            } else if ((bool)radioButton_Sims4.IsChecked) {
                statement = "Sims 4 picked.";
                loggingGlobals.MakeLog(statement, true);
                gameNum = 4;
                //get info
            } else {
                System.Windows.Forms.MessageBox.Show("Please select a game.");
                statement = "No game picked.";
                loggingGlobals.MakeLog(statement, true);
            }
        }

        public void changeTestText(string text){
            statement = "Changing the report text box.";
            loggingGlobals.MakeLog(statement, true);
            completionAlertValue(text);
        }

        private void testbutton_Click(object sender, EventArgs e) {
            statement = "Dev test button clicked.";
            loggingGlobals.MakeLog(statement, true);
        }
    }
}
