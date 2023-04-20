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
using System.Diagnostics;
using CCSorterControls;
using CCSorter;

namespace Sims_CC_Sorter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class CCSorterApp : Window
    {
        public CCSorterApp()
        {
            InitializeComponent();
        }
        
        private void exitButton_Click(object sender, EventArgs e) {
            //this.Close();
            System.Windows.Application.Current.Shutdown();
        }

        public void LocationBoxValue(string value) {
            PickedLocation.Text = value;
        }

        public void completionAlertValue(string value) {
            completionAlert.Text = value;
        }

        string SelectedFolder = "";
        int gameNum = 0;
        public string proof = "Proof this works?";

        private void browseLocation_Click(object sender, EventArgs e) {
            using(var GetFolder = new FolderBrowserDialog())
            {
                DialogResult result = GetFolder.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK) {
                    SelectedFolder = GetFolder.SelectedPath;
                    LocationBoxValue(SelectedFolder);
                } else {
                    LocationBoxValue(SelectedFolder);
                }
            }
        }

        BrokenIncorrect packageSearch = new BrokenIncorrect();

        private void findBroken_Click(object sender, EventArgs e) {
            completionAlertValue("Starting search.");
            if (SelectedFolder == "") {
                System.Windows.Forms.MessageBox.Show("Please select the folder containing your package files.");
            } else {
                checkGame();
                if (gameNum is 0){
                    //
                } else {
                    packageSearch.FindPackagesToRemove(gameNum, SelectedFolder);
                }                
            }
        }

        private void checkGame() {
            if ((bool)radioButton_Sims2.IsChecked) {
                gameNum = 2;
            } else if ((bool)radioButton_Sims3.IsChecked) {
                gameNum = 3;
            } else if ((bool)radioButton_Sims4.IsChecked) {
                gameNum = 4;
            } else {
                System.Windows.Forms.MessageBox.Show("Please select a game.");
            }
        }

        public void changeTestText(string text){
            completionAlertValue(text);
        }
    }
}
