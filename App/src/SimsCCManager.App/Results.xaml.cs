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



namespace SimsCCManager.SortingUIResults {

    public partial class ResultsWindow : Window {
        LoggingGlobals log = new LoggingGlobals();
        public int gameNum = 0;
        
        public ResultsWindow() 
        {
            log.MakeLog("Initializing results window.", true);
            InitializeComponent(); 
            log.MakeLog("Running results grid method.", true);
            ResultsDataGrid.ItemsSource = resultspackages.populateResultsList(); 
            if (GlobalVariables.loadedSaveData == false) {
                CacheData();
            }  
        }

        private void CacheData(){
            log.MakeLog("Turning data into json file.", true);
            using (StreamWriter file = File.CreateText(SaveData.mainSaveData))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, resultspackages.resultspackageslist);
            }
        }

        //if selected --> right click "make otg > lights" --> add thing to package

        private void menu_Click(object sender, EventArgs e)
        {
            log.MakeLog("Closing application.", false);
            System.Windows.Application.Current.Shutdown();
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
            if (GlobalVariables.loadedSaveData == true) {
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
            }
            
        }
        public event PropertyChangedEventHandler PropertyChanged; 
	
        private void RaiseProperChanged([CallerMemberName] string caller = "") { 
        
            if (PropertyChanged != null) { 
                PropertyChanged(this, new PropertyChangedEventArgs(caller)); 
            } 
        }  
        
    }   
}