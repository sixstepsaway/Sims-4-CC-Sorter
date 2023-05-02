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
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SSAGlobals;
using SimsCCManager.Packages.Containers;



namespace SimsCCManager.SortingUIFunctions {

        public partial class ResultsWindow : Window {
        LoggingGlobals log = new LoggingGlobals();
        
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
                    resultspackageslist.Add(new SimsPackage{ Title = pack.Title, Description = pack.Description, Location = pack.Location, PackageName = pack.PackageName, Type = pack.Type, Game = pack.Game, DBPF = pack.DBPF, InstanceIDs = pack.InstanceIDs, Major = pack.Major, Minor = pack.Minor, DateCreated = pack.DateCreated, DateModified = pack.DateModified, IndexMajorVersion = pack.IndexMajorVersion, IndexCount = pack.IndexCount, IndexOffset = pack.IndexOffset, IndexSize = pack.IndexSize, HolesCount = pack.HolesCount, HolesOffset = pack.HolesOffset, HolesSize = pack.HolesSize, IndexMinorVersion = pack.IndexMinorVersion, XMLType = pack.XMLType, XMLSubtype = pack.XMLSubtype, XMLCategory = pack.XMLCategory, XMLModelName = pack.XMLModelName, ObjectGUID = pack.ObjectGUID, XMLCreator = pack.XMLCreator, XMLAge = pack.XMLAge, XMLGender = pack.XMLGender, RequiredEPs = pack.RequiredEPs, Function = pack.Function, FunctionSubcategory = pack.FunctionSubcategory, RoomSort = pack.RoomSort, Entries = pack.Entries, Mesh = pack.Mesh, Recolor = pack.Recolor, Orphan = pack.Orphan });
                }
                log.MakeLog("Returning.", true);
                return resultspackageslist; 
            } else {
                log.MakeLog("Putting Sims 2 packages into grid.", true);
                foreach (SimsPackage pack in Containers.allSims2Packages){
                    resultspackageslist.Add(new SimsPackage{ Title = pack.Title, Description = pack.Description, Location = pack.Location, PackageName = pack.PackageName, Type = pack.Type, Game = pack.Game, DBPF = pack.DBPF, InstanceIDs = pack.InstanceIDs, Major = pack.Major, Minor = pack.Minor, DateCreated = pack.DateCreated, DateModified = pack.DateModified, IndexMajorVersion = pack.IndexMajorVersion, IndexCount = pack.IndexCount, IndexOffset = pack.IndexOffset, IndexSize = pack.IndexSize, HolesCount = pack.HolesCount, HolesOffset = pack.HolesOffset, HolesSize = pack.HolesSize, IndexMinorVersion = pack.IndexMinorVersion, XMLType = pack.XMLType, XMLSubtype = pack.XMLSubtype, XMLCategory = pack.XMLCategory, XMLModelName = pack.XMLModelName, ObjectGUID = pack.ObjectGUID, XMLCreator = pack.XMLCreator, XMLAge = pack.XMLAge, XMLGender = pack.XMLGender, RequiredEPs = pack.RequiredEPs, Function = pack.Function, FunctionSubcategory = pack.FunctionSubcategory, RoomSort = pack.RoomSort, Entries = pack.Entries, Mesh = pack.Mesh, Recolor = pack.Recolor, Orphan = pack.Orphan });
                }
                log.MakeLog("Putting Sims 3 packages into grid.", true);
                foreach (SimsPackage pack in Containers.allSims3Packages){
                    resultspackageslist.Add(new SimsPackage{ Title = pack.Title, Description = pack.Description, Location = pack.Location, PackageName = pack.PackageName, Type = pack.Type, Game = pack.Game, DBPF = pack.DBPF, InstanceIDs = pack.InstanceIDs, Major = pack.Major, Minor = pack.Minor, DateCreated = pack.DateCreated, DateModified = pack.DateModified, IndexMajorVersion = pack.IndexMajorVersion, IndexCount = pack.IndexCount, IndexOffset = pack.IndexOffset, IndexSize = pack.IndexSize, HolesCount = pack.HolesCount, HolesOffset = pack.HolesOffset, HolesSize = pack.HolesSize, IndexMinorVersion = pack.IndexMinorVersion, XMLType = pack.XMLType, XMLSubtype = pack.XMLSubtype, XMLCategory = pack.XMLCategory, XMLModelName = pack.XMLModelName, ObjectGUID = pack.ObjectGUID, XMLCreator = pack.XMLCreator, XMLAge = pack.XMLAge, XMLGender = pack.XMLGender, RequiredEPs = pack.RequiredEPs, Function = pack.Function, FunctionSubcategory = pack.FunctionSubcategory, RoomSort = pack.RoomSort, Entries = pack.Entries, Mesh = pack.Mesh, Recolor = pack.Recolor, Orphan = pack.Orphan });
                }
                log.MakeLog("Putting Sims 4 packages into grid.", true);
                foreach (SimsPackage pack in Containers.allSims4Packages){
                    resultspackageslist.Add(new SimsPackage{ Title = pack.Title, Description = pack.Description, Location = pack.Location, PackageName = pack.PackageName, Type = pack.Type, Game = pack.Game, DBPF = pack.DBPF, InstanceIDs = pack.InstanceIDs, Major = pack.Major, Minor = pack.Minor, DateCreated = pack.DateCreated, DateModified = pack.DateModified, IndexMajorVersion = pack.IndexMajorVersion, IndexCount = pack.IndexCount, IndexOffset = pack.IndexOffset, IndexSize = pack.IndexSize, HolesCount = pack.HolesCount, HolesOffset = pack.HolesOffset, HolesSize = pack.HolesSize, IndexMinorVersion = pack.IndexMinorVersion, XMLType = pack.XMLType, XMLSubtype = pack.XMLSubtype, XMLCategory = pack.XMLCategory, XMLModelName = pack.XMLModelName, ObjectGUID = pack.ObjectGUID, XMLCreator = pack.XMLCreator, XMLAge = pack.XMLAge, XMLGender = pack.XMLGender, RequiredEPs = pack.RequiredEPs, Function = pack.Function, FunctionSubcategory = pack.FunctionSubcategory, RoomSort = pack.RoomSort, Entries = pack.Entries, Mesh = pack.Mesh, Recolor = pack.Recolor, Orphan = pack.Orphan });
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