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
        string SelectedFolder = "";
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
                    //controls.FindPackages();
                } else {
                    LocationBoxValue(SelectedFolder);
                }
            }            
        }        

        private void findBroken_Click(object sender, EventArgs e) {
            /*completionAlertValue("Starting search.");
            if (SelectedFolder == "") {
                System.Windows.Forms.MessageBox.Show("Please select the folder containing your package files.");
            } else {
                checkGame();
                if (gameNum is 0){
                    //
                } else {
                    
                    controls.FindPackagesToRemove(gameNum, SelectedFolder);
                }                
            }
            completionAlertValue("Search completed.");
          */  
        }

        private void renameSims2Packages_Click(object sender, EventArgs e) {
            //controls.RenameS2Packages();
        }

        private void checkGame() {
            /*if ((bool)radioButton_Sims2.IsChecked) {
                gameNum = 2;
            } else if ((bool)radioButton_Sims3.IsChecked) {
                gameNum = 3;
            } else if ((bool)radioButton_Sims4.IsChecked) {
                gameNum = 4;
            } else {
                System.Windows.Forms.MessageBox.Show("Please select a game.");
            }*/
        }

        public void changeTestText(string text){
            completionAlertValue(text);
        }

        private void testbutton_Click(object sender, EventArgs e) {
            
        }

    }
}
