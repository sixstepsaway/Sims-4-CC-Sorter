using System;
using System.Collections.Generic;
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
using SSAGlobals;
using SimsCCManager.Packages.Containers;



namespace SimsCCManager.SortingUIFunctions {

    public partial class ResultsWindow : Window {
        
        public ResultsWindow() 
        {
            InitializeComponent(); 
            ResultsDataGrid.ItemsSource = resultspackages.populateResultsList(); 
        }

    }

    public class resultspackages : INotifyPropertyChanged {
        public static SynchronizedCollection<SimsPackage> resultspackageslist = new SynchronizedCollection<SimsPackage>();
        private bool selected; 
        
        public bool Selected { 
            get { return Selected; } 
            set { 
                Selected = value; 
                RaiseProperChanged(); 
            } 
        }  
        
       public static SynchronizedCollection<SimsPackage> populateResultsList() 
        {
            
            
            foreach (SimsPackage pack in Containers.allSims2Packages){
                resultspackageslist.Add(new SimsPackage{ Title = pack.Title, Description = pack.Description, Location = pack.Location, PackageName = pack.PackageName, Type = pack.Type, Game = pack.Game, DBPF = pack.DBPF, InstanceIDs = pack.InstanceIDs, Major = pack.Major, Minor = pack.Minor, DateCreated = pack.DateCreated, DateModified = pack.DateModified, IndexMajorVersion = pack.IndexMajorVersion, IndexCount = pack.IndexCount, IndexOffset = pack.IndexOffset, IndexSize = pack.IndexSize, HolesCount = pack.HolesCount, HolesOffset = pack.HolesOffset, HolesSize = pack.HolesSize, IndexMinorVersion = pack.IndexMinorVersion, XMLType = pack.XMLType, XMLSubtype = pack.XMLSubtype, XMLCategory = pack.XMLCategory, XMLModelName = pack.XMLModelName, ObjectGUID = pack.ObjectGUID, XMLCreator = pack.XMLCreator, XMLAge = pack.XMLAge, XMLGender = pack.XMLGender, RequiredEPs = pack.RequiredEPs, Function = pack.Function, FunctionSubcategory = pack.FunctionSubcategory, RoomSort = pack.RoomSort, Entries = pack.Entries, Mesh = pack.Mesh, Recolor = pack.Recolor, Orphan = pack.Orphan });
            }
            foreach (SimsPackage pack in Containers.allSims3Packages){
                resultspackageslist.Add(new SimsPackage{ Title = pack.Title, Description = pack.Description, Location = pack.Location, PackageName = pack.PackageName, Type = pack.Type, Game = pack.Game, DBPF = pack.DBPF, InstanceIDs = pack.InstanceIDs, Major = pack.Major, Minor = pack.Minor, DateCreated = pack.DateCreated, DateModified = pack.DateModified, IndexMajorVersion = pack.IndexMajorVersion, IndexCount = pack.IndexCount, IndexOffset = pack.IndexOffset, IndexSize = pack.IndexSize, HolesCount = pack.HolesCount, HolesOffset = pack.HolesOffset, HolesSize = pack.HolesSize, IndexMinorVersion = pack.IndexMinorVersion, XMLType = pack.XMLType, XMLSubtype = pack.XMLSubtype, XMLCategory = pack.XMLCategory, XMLModelName = pack.XMLModelName, ObjectGUID = pack.ObjectGUID, XMLCreator = pack.XMLCreator, XMLAge = pack.XMLAge, XMLGender = pack.XMLGender, RequiredEPs = pack.RequiredEPs, Function = pack.Function, FunctionSubcategory = pack.FunctionSubcategory, RoomSort = pack.RoomSort, Entries = pack.Entries, Mesh = pack.Mesh, Recolor = pack.Recolor, Orphan = pack.Orphan });
            }
            foreach (SimsPackage pack in Containers.allSims4Packages){
                resultspackageslist.Add(new SimsPackage{ Title = pack.Title, Description = pack.Description, Location = pack.Location, PackageName = pack.PackageName, Type = pack.Type, Game = pack.Game, DBPF = pack.DBPF, InstanceIDs = pack.InstanceIDs, Major = pack.Major, Minor = pack.Minor, DateCreated = pack.DateCreated, DateModified = pack.DateModified, IndexMajorVersion = pack.IndexMajorVersion, IndexCount = pack.IndexCount, IndexOffset = pack.IndexOffset, IndexSize = pack.IndexSize, HolesCount = pack.HolesCount, HolesOffset = pack.HolesOffset, HolesSize = pack.HolesSize, IndexMinorVersion = pack.IndexMinorVersion, XMLType = pack.XMLType, XMLSubtype = pack.XMLSubtype, XMLCategory = pack.XMLCategory, XMLModelName = pack.XMLModelName, ObjectGUID = pack.ObjectGUID, XMLCreator = pack.XMLCreator, XMLAge = pack.XMLAge, XMLGender = pack.XMLGender, RequiredEPs = pack.RequiredEPs, Function = pack.Function, FunctionSubcategory = pack.FunctionSubcategory, RoomSort = pack.RoomSort, Entries = pack.Entries, Mesh = pack.Mesh, Recolor = pack.Recolor, Orphan = pack.Orphan });
            }
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