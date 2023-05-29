using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SSAGlobals;
using SimsCCManager.Packages.Containers;
using SimsCCManager.App.PreviewImage;
using System.Data.SQLite;
using System.Data;

namespace SimsCCManager.SortingUIResults {
    /// <summary>
    /// Results window; a datagrid with all of the package files. (Later, will include ts3packs and ts2packs as well.)
    /// </summary>

    public partial class ResultsWindow : Window {
        LoggingGlobals log = new LoggingGlobals();
        public int gameNum = 0;
        private bool showallfiles = false;
        private DataTable packagesDataTable = new DataTable();
        private DataTable allFilesDataTable = new DataTable();
        //public static ObservableCollection<SimsPackage> resultspackageslist = new ObservableCollection<SimsPackage>();
        
        public ResultsWindow() 
        {
            log.MakeLog("Initializing results window.", true);
            InitializeComponent(); 
            log.MakeLog("Running results grid method.", true); 
            

            var con = new SQLiteConnection(GlobalVariables.PackagesReadDS);
            try
            {
                con.Open();
                SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM Packages ";
                using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(cmd.CommandText, con))
                {
                    
                    dataAdapter.Fill(packagesDataTable);
                    ResultsDataGridPackages.ItemsSource = packagesDataTable.AsDataView();

                }
                cmd.CommandText = "SELECT * FROM AllFiles ";
                using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(cmd.CommandText, con))
                {
                   
                    dataAdapter.Fill(allFilesDataTable);   
                    ResultsDataGridAllFiles.ItemsSource = allFilesDataTable.AsDataView();
                }
            }
            catch (Exception exp)
            {
                System.Windows.Forms.MessageBox.Show(exp.Message);
            }

            //var dbc = new SQLite.SQLiteConnection(GlobalVariables.PackagesRead);
            //var pquery = dbc.Query<SimsPackage>("SELECT * FROM Packages");

             
            //ResultsDataGrid.DataContext = pquery; 
            //if (GlobalVariables.loadedSaveData == false) {
            //    CacheData();
            //}  
        }

        private void showallfiles_Click(object sender, EventArgs e){
            if (showallfiles == false){
                ShowAllBt.Content = "Show Only Packages";
                ShowAllBt.Background = Brushes.CadetBlue;
                ResultsDataGridPackages.Visibility = Visibility.Hidden;
                ResultsDataGridAllFiles.Visibility = Visibility.Visible;
                showallfiles = true;
            } else {
                ShowAllBt.Content = "Show All Files";
                ShowAllBt.Background = Brushes.Beige;
                ResultsDataGridPackages.Visibility = Visibility.Visible;
                ResultsDataGridAllFiles.Visibility = Visibility.Hidden;
                showallfiles = false;
            }
            
        }

        private void CacheData(){
            log.MakeLog("Turning data into json file.", true);
            using (StreamWriter file = File.CreateText(SaveData.mainSaveData))
            {
                Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                serializer.Serialize(file, resultspackages.resultspackageslist);
            }
        }

        private void ResultsDataGridPackages_SelectionChanged(object sender, EventArgs e){
            ResultsPreviewImage rpi = new ResultsPreviewImage();
            if( ResultsDataGridPackages.SelectedCells.Count == 1 )
            {                
                DataGrid dg = sender as DataGrid;
                var row = ResultsDataGridPackages.SelectedValue;
                rpi.Show();
            } else if ((ResultsDataGridPackages.SelectedCells.Count == 0) || (ResultsDataGridPackages.SelectedCells.Count > 1)) {
                rpi.Hide();
            }
        }
        private void ResultsDataGridAllFiles_SelectionChanged(object sender, EventArgs e){
            ResultsPreviewImage rpi = new ResultsPreviewImage();
            if( ResultsDataGridAllFiles.SelectedCells.Count == 1 )
            {                
                DataGrid dg = sender as DataGrid;
                var row = ResultsDataGridAllFiles.SelectedValue;
                rpi.Show();
            } else if ((ResultsDataGridAllFiles.SelectedCells.Count == 0) || (ResultsDataGridAllFiles.SelectedCells.Count > 1)) {
                rpi.Hide();
            }
        }
        

        //if selected --> right click "make otg > lights" --> add thing to package

        public void SearchEntered(object sender, TextChangedEventArgs args)
        {   /*string cs = string.Format("Data Source={0}", GlobalVariables.PackagesRead);
            using (var dataConnection = new SQLiteConnection(cs))
            try
            {
                string sql = "SELECT * FROM *";
            }
            catch (Exception e) {
                System.Windows.Forms.MessageBox.Show("Search Error: " + e.Message.ToString(),
                "Error Message: Results Window Search Failure",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dataConnection.Close();
            }*/
        }

        private void menu_Click(object sender, EventArgs e)
        {
            log.MakeLog("Closing application.", false);
            GlobalVariables.DatabaseConnection.Commit();
            GlobalVariables.DatabaseConnection.Close();
            System.Windows.Application.Current.Shutdown();
        }

        /*private void ResultsDataGridPackages_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenu m = new ContextMenu();
                m.MenuItems.Add(new MenuItem("Cut"));
                m.MenuItems.Add(new MenuItem("Copy"));
                m.MenuItems.Add(new MenuItem("Paste"));

                int currentMouseOverRow = ResultsDataGridPackages.HitTest(e.X,e.Y).RowIndex;

                if (currentMouseOverRow >= 0)
                {
                    m.MenuItems.Add(new MenuItem(string.Format("Do something to row {0}", currentMouseOverRow.ToString())));
                }

                m.Show(ResultsDataGridPackages, new Point(e.X, e.Y));

            }
        }

        private void ResultsDataGridAllFiles_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenu m = new ContextMenu();
                m.MenuItems.Add(new MenuItem("Cut"));
                m.MenuItems.Add(new MenuItem("Copy"));
                m.MenuItems.Add(new MenuItem("Paste"));

                int currentMouseOverRow = ResultsDataGridAllFiles.HitTest(e.X,e.Y).RowIndex;

                if (currentMouseOverRow >= 0)
                {
                    m.MenuItems.Add(new MenuItem(string.Format("Do something to row {0}", currentMouseOverRow.ToString())));
                }

                m.Show(ResultsDataGridAllFiles, new Point(e.X, e.Y));

            }
        }*/

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

        private void loadErrorFix_Click(object sender, EventArgs e){
            
            bool gameChecked = checkGame();
            if (gameChecked == true){
                if (System.Windows.Forms.MessageBox.Show
                ("There is no guarantee this will fix all load errors, but it should. This will move all incorrect (wrong for your game version) or broken packages to a separate folder. Nothing will be deleted. Should we go ahead?", "Fix Load Errors",
                System.Windows.Forms.MessageBoxButtons.YesNo, 
                System.Windows.Forms.MessageBoxIcon.Question)
                ==System.Windows.Forms.DialogResult.Yes) {
                    //fix load errors o.o
                }
            } else {
                //do nothing, a message will have done the job
            }
            
        }

        private bool checkGame() {
            bool gamepicked = false;

            log.MakeLog("Checking which game is ticked.", true);
            if ((bool)radioButton_Sims2.IsChecked) {
                log.MakeLog("Sims 2 picked.", true);
                gameNum = 2;
                gamepicked = true;
                return gamepicked;
            } else if ((bool)radioButton_Sims3.IsChecked) {
                log.MakeLog("Sims 3 picked.", true);
                gameNum = 3;
                gamepicked = true;
                return gamepicked;
            } else if ((bool)radioButton_Sims4.IsChecked) {
                log.MakeLog("Sims 4 picked.", true);
                gameNum = 4;
                gamepicked = true;
                return gamepicked;
            } else {
                System.Windows.Forms.MessageBox.Show("Please select a game.");
                log.MakeLog("No game picked.", true);
                gamepicked = false;
                return gamepicked;
            }
        }


    }

        
    public class resultspackages : INotifyPropertyChanged {
        LoggingGlobals log = new LoggingGlobals();
        public static ObservableCollection<SimsPackage> resultspackageslist = new ObservableCollection<SimsPackage>();
        private bool selected; 
        
        public bool Selected { 
            get { return Selected; } 
            set { 
                Selected = value; 
                RaiseProperChanged(); 
            } 
        }
        
       public static ObservableCollection<SimsPackage> populateResultsList() 
        {   LoggingGlobals log = new LoggingGlobals();
            
                

            /*if (GlobalVariables.loadedSaveData == true) {
                log.MakeLog("Retrieving save data and putting it into grid.", true);
                foreach (SimsPackage pack in GlobalVariables.loadedData){
                    resultspackageslist.Add(new SimsPackage{ Title = pack.Title, Description = pack.Description, Location = pack.Location, PackageName = pack.PackageName, Type = pack.Type, Game = pack.Game, DBPF = pack.DBPF, InstanceIDs = pack.InstanceIDs, Major = pack.Major, Minor = pack.Minor, DateCreated = pack.DateCreated, DateModified = pack.DateModified, IndexMajorVersion = pack.IndexMajorVersion, IndexCount = pack.IndexCount, IndexOffset = pack.IndexOffset, IndexSize = pack.IndexSize, HolesCount = pack.HolesCount, HolesOffset = pack.HolesOffset, HolesSize = pack.HolesSize, IndexMinorVersion = pack.IndexMinorVersion, XMLType = pack.XMLType, XMLSubtype = pack.XMLSubtype, XMLCategory = pack.XMLCategory, XMLModelName = pack.XMLModelName, ObjectGUID = pack.ObjectGUID, XMLCreator = pack.XMLCreator, XMLAge = pack.XMLAge, XMLGender = pack.XMLGender, RequiredEPs = pack.RequiredEPs, Function = pack.Function, FunctionSubcategory = pack.FunctionSubcategory, RoomSort = pack.RoomSort, Entries = pack.Entries, Mesh = pack.Mesh, Recolor = pack.Recolor, Orphan = pack.Orphan, GameVersion = pack.GameVersion });
                }
                log.MakeLog("Returning.", true);
                return resultspackageslist; 
            } else {
                log.MakeLog("Putting Sims 2 packages into grid.", true);
                foreach (SimsPackage pack in Containers.allSims2Packages){
                    resultspackageslist.Add(new SimsPackage{ Title = pack.Title, Description = pack.Description, Location = pack.Location, PackageName = pack.PackageName, Type = pack.Type, Game = pack.Game, DBPF = pack.DBPF, InstanceIDs = pack.InstanceIDs, Major = pack.Major, Minor = pack.Minor, DateCreated = pack.DateCreated, DateModified = pack.DateModified, IndexMajorVersion = pack.IndexMajorVersion, IndexCount = pack.IndexCount, IndexOffset = pack.IndexOffset, IndexSize = pack.IndexSize, HolesCount = pack.HolesCount, HolesOffset = pack.HolesOffset, HolesSize = pack.HolesSize, IndexMinorVersion = pack.IndexMinorVersion, XMLType = pack.XMLType, XMLSubtype = pack.XMLSubtype, XMLCategory = pack.XMLCategory, XMLModelName = pack.XMLModelName, ObjectGUID = pack.ObjectGUID, XMLCreator = pack.XMLCreator, XMLAge = pack.XMLAge, XMLGender = pack.XMLGender, RequiredEPs = pack.RequiredEPs, Function = pack.Function, FunctionSubcategory = pack.FunctionSubcategory, RoomSort = pack.RoomSort, Entries = pack.Entries, Mesh = pack.Mesh, Recolor = pack.Recolor, Orphan = pack.Orphan, GameVersion = "Sims 2" });
                }
                log.MakeLog("Putting Sims 3 packages into grid.", true);
                foreach (SimsPackage pack in Containers.allSims3Packages){
                    resultspackageslist.Add(new SimsPackage{ Title = pack.Title, Description = pack.Description, Location = pack.Location, PackageName = pack.PackageName, Type = pack.Type, Game = pack.Game, DBPF = pack.DBPF, InstanceIDs = pack.InstanceIDs, Major = pack.Major, Minor = pack.Minor, DateCreated = pack.DateCreated, DateModified = pack.DateModified, IndexMajorVersion = pack.IndexMajorVersion, IndexCount = pack.IndexCount, IndexOffset = pack.IndexOffset, IndexSize = pack.IndexSize, HolesCount = pack.HolesCount, HolesOffset = pack.HolesOffset, HolesSize = pack.HolesSize, IndexMinorVersion = pack.IndexMinorVersion, XMLType = pack.XMLType, XMLSubtype = pack.XMLSubtype, XMLCategory = pack.XMLCategory, XMLModelName = pack.XMLModelName, ObjectGUID = pack.ObjectGUID, XMLCreator = pack.XMLCreator, XMLAge = pack.XMLAge, XMLGender = pack.XMLGender, RequiredEPs = pack.RequiredEPs, Function = pack.Function, FunctionSubcategory = pack.FunctionSubcategory, RoomSort = pack.RoomSort, Entries = pack.Entries, Mesh = pack.Mesh, Recolor = pack.Recolor, Orphan = pack.Orphan, GameVersion = "Sims 3" });
                }
                log.MakeLog("Putting Sims 4 packages into grid.", true);
                foreach (SimsPackage pack in Containers.allSims4Packages){
                    resultspackageslist.Add(new SimsPackage{ Title = pack.Title, Description = pack.Description, Location = pack.Location, PackageName = pack.PackageName, Type = pack.Type, Game = pack.Game, DBPF = pack.DBPF, InstanceIDs = pack.InstanceIDs, Major = pack.Major, Minor = pack.Minor, DateCreated = pack.DateCreated, DateModified = pack.DateModified, IndexMajorVersion = pack.IndexMajorVersion, IndexCount = pack.IndexCount, IndexOffset = pack.IndexOffset, IndexSize = pack.IndexSize, HolesCount = pack.HolesCount, HolesOffset = pack.HolesOffset, HolesSize = pack.HolesSize, IndexMinorVersion = pack.IndexMinorVersion, XMLType = pack.XMLType, XMLSubtype = pack.XMLSubtype, XMLCategory = pack.XMLCategory, XMLModelName = pack.XMLModelName, ObjectGUID = pack.ObjectGUID, XMLCreator = pack.XMLCreator, XMLAge = pack.XMLAge, XMLGender = pack.XMLGender, RequiredEPs = pack.RequiredEPs, Function = pack.Function, FunctionSubcategory = pack.FunctionSubcategory, RoomSort = pack.RoomSort, Entries = pack.Entries, Mesh = pack.Mesh, Recolor = pack.Recolor, Orphan = pack.Orphan, GameVersion = "Sims 4" });
                }
                log.MakeLog("Returning.", true);
                return resultspackageslist; 
            }*/
            return resultspackageslist;
            
        }
        public event PropertyChangedEventHandler PropertyChanged; 
	
        private void RaiseProperChanged([CallerMemberName] string caller = "") { 
        
            if (PropertyChanged != null) { 
                PropertyChanged(this, new PropertyChangedEventArgs(caller)); 
            } 
        }  
        
    }   
}