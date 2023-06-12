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
using System.Threading;
using SQLiteNetExtensions.Extensions;

namespace SimsCCManager.SortingUIResults {
    /// <summary>
    /// Results window; a datagrid with all of the package files. (Later, will include ts3packs and ts2packs as well.)
    /// </summary>

    public partial class ResultsWindow : Window {
        LoggingGlobals log = new LoggingGlobals();
        public static int gameNum = 0;
        private bool showallfiles = false;
        private DataTable packagesDataTable = new DataTable();
        private DataTable allFilesDataTable = new DataTable();
        //public static ObservableCollection<SimsPackage> resultspackageslist = new ObservableCollection<SimsPackage>();
        CancellationTokenSource cts = new CancellationTokenSource();
        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;
        public static System.Windows.Controls.ListView resultsView = new System.Windows.Controls.ListView();
        ContextMenu contextmenu = new ContextMenu();
        public static System.Windows.Controls.TextBox searchbox = new System.Windows.Controls.TextBox();
        public static System.Windows.Controls.ComboBox comboBox = new System.Windows.Controls.ComboBox();
        public static System.Windows.Controls.ListView tagslist = new System.Windows.Controls.ListView();
        public static Grid tagsgrid = new Grid();
        public static Dictionary<string, string> comboboxoptions = new Dictionary<string, string>();
        public static int game = 0;
        
        public ResultsWindow(CancellationTokenSource cts) 
        {
            this.cts = cts;
            log.MakeLog("Initializing results window.", true);
            InitializeComponent(); 
            log.MakeLog("Running results grid method.", true); 
            Loaded += ResultsWindow_Loaded;
            resultsView = ResultsView;
            tagslist = TagsListBox;
            tagsgrid = this.TagsList;
            contextmenu = ResultsView.ContextMenu;
            DataContext = new PackagesViewModel(); 
            searchbox = SearchBox;
            comboBox = this.ComboBoxSearch;
            List<string> comboboxsearch = new List<string>();
            comboboxoptions.Add("Package Name", "PackageName");
            comboboxoptions.Add("Title", "Title");
            comboboxoptions.Add("Description", "Description");
            comboboxoptions.Add("InstanceIDs", "InstanceIDsBlobbed");
            comboboxoptions.Add("Catalog Tags", "CatalogTags");
            comboboxoptions.Add("Type", "Type");
            comboboxoptions.Add("Game", "Game");
            comboboxoptions.Add("Category", "Category");
            comboboxoptions.Add("Age", "Age");
            comboboxoptions.Add("Gender", "Gender");
            comboboxoptions.Add("Function", "Function");
            comboboxoptions.Add("Function Subcategory", "FunctionSubcategory");
            comboboxoptions.Add("Required EPs", "RequiredEpsBlob");
            comboboxoptions.Add("Overridden Instances", "OverridenInstancesBlobbed");
            comboboxoptions.Add("Overridden Items", "OverriddenItemsBlobbed");
            comboboxoptions.Add("Conflicts", "MatchingConflictsBlobbed");
            comboboxoptions.Add("Matching Recolors", "MatchingRecolorsBlobbed");
            comboboxoptions.Add("Matching Meshes", "MatchingMesh");
            foreach (KeyValuePair<string,string> kvp in comboboxoptions){
                comboboxsearch.Add(kvp.Key);
            }
            this.ComboBoxSearch.ItemsSource = comboboxsearch;
        }     

        private void ResultsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void GameCheck(object sender, RoutedEventArgs e) {
            System.Windows.Controls.RadioButton rb = sender as System.Windows.Controls.RadioButton; 
            if (rb.Name == "radioButton_None"){
                gameNum = 0;
            }
            if (rb.Name == "radioButton_Sims2"){
                gameNum = 2;
            }
            if (rb.Name == "radioButton_Sims3"){
                gameNum = 3;
            }
            if (rb.Name == "radioButton_Sims4"){
                gameNum = 4;
            }
            //Console.WriteLine("Game number is: " + gameNum); 
        }
        private void SearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			CollectionViewSource.GetDefaultView(ResultsWindow.resultsView.ItemsSource).Refresh();
		}


        private void Kofi_Click(object sender, EventArgs e){
            if (System.Windows.Forms.MessageBox.Show
            ("Open Kofi?", "Opening External URL",
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
        private void Git_Click(object sender, EventArgs e){
            if (System.Windows.Forms.MessageBox.Show
            ("Open the Sims CC Manager Github Repo?", "Opening External URL",
            System.Windows.Forms.MessageBoxButtons.YesNo, 
            System.Windows.Forms.MessageBoxIcon.Question)
            ==System.Windows.Forms.DialogResult.Yes)
                {
                    Process.Start(new ProcessStartInfo("https://github.com/sixstepsaway/Sims-CC-Manager") { UseShellExecute = true });
                }

            else
                {
                //React as needed.
                }
        }
        private void Twitter_Click(object sender, EventArgs e){
            if (System.Windows.Forms.MessageBox.Show
            ("Open SinfulSimming's Twitter?", "Opening External URL",
            System.Windows.Forms.MessageBoxButtons.YesNo, 
            System.Windows.Forms.MessageBoxIcon.Question)
            ==System.Windows.Forms.DialogResult.Yes)
                {
                    Process.Start(new ProcessStartInfo("https://twitter.com/sinfulsimming") { UseShellExecute = true });
                }

            else
                {
                //React as needed.
                }
        }
        private void Discord_Click(object sender, EventArgs e){
            if (System.Windows.Forms.MessageBox.Show
            ("Open Discord?", "Opening External URL",
            System.Windows.Forms.MessageBoxButtons.YesNo, 
            System.Windows.Forms.MessageBoxIcon.Question)
            ==System.Windows.Forms.DialogResult.Yes)
                {
                    Process.Start(new ProcessStartInfo("https://discord.gg/M6vnf842Fp") { UseShellExecute = true });  
                }

            else
                {
                //React as needed.
                }
                          
        }

        
        private void showallfiles_Click(object sender, EventArgs e){
                        
        }        

        private void menu_Click(object sender, EventArgs e)
        {
            log.MakeLog("Closing application.", false);
            cts.Cancel();
            Thread.Sleep(2000);
            GlobalVariables.InstancesCacheConnection.Commit();
            GlobalVariables.InstancesCacheConnection.Close();
            GlobalVariables.S4FunctionTypesConnection.Commit();
            GlobalVariables.S4FunctionTypesConnection.Close();
            GlobalVariables.S4OverridesConnection.Commit();
            GlobalVariables.S4OverridesConnection.Close();
            GlobalVariables.S4SpecificOverridesConnection.Commit();
            GlobalVariables.S4SpecificOverridesConnection.Close();
            GlobalVariables.DatabaseConnection.Commit();
            GlobalVariables.DatabaseConnection.Close();
            System.Windows.Application.Current.Shutdown();
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
        
        private void CloseTagsList_Click(object sender, EventArgs e){
            
            tagsgrid.Visibility = Visibility.Hidden;            
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

        private void ListViewHeader_Click (object sender, RoutedEventArgs e){
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    var columnBinding = headerClicked.Column.DisplayMemberBinding as System.Windows.Data.Binding;
                    var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;

                    Sort(sortBy, direction);

                    if (direction == ListSortDirection.Ascending)
                    {
                        headerClicked.Column.HeaderTemplate =
                        Resources["HeaderTemplateArrowUp"] as DataTemplate;
                    }
                    else
                    {
                        headerClicked.Column.HeaderTemplate =
                        Resources["HeaderTemplateArrowDown"] as DataTemplate;
                    }

                    // Remove arrow from previously sorted header
                    if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                    {
                        _lastHeaderClicked.Column.HeaderTemplate = null;
                    }

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }            
        }
        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView =
            CollectionViewSource.GetDefaultView(ResultsView.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }
    }    

    public class PackagesViewModel : INotifyPropertyChanged{
        private ICollectionView _packagesView;
        private ICollectionView _tagsView;
        public event PropertyChangedEventHandler PropertyChanged;
        private PackagesViewModel _selectedFile;  

        public ICollectionView Packages
        {
            get {return _packagesView;}
        }
        public ICollectionView Tags
        {
            get {return _tagsView;}
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public object Resources { get; private set; }
        ObservableCollection<SimsPackage> packages = new ObservableCollection<SimsPackage>();
        public static ObservableCollection<TagsList> tags = new ObservableCollection<TagsList>();

        public PackagesViewModel(){
            packages = new (GlobalVariables.DatabaseConnection.GetAllWithChildren<SimsPackage>());
            _tagsView = CollectionViewSource.GetDefaultView(tags);
            _packagesView = CollectionViewSource.GetDefaultView(packages);
            _packagesView.Filter = SearchFilter;            
        }

        public PackagesViewModel SelectedFileInfo  
        {  
            get { return _selectedFile; }  
            set 
            {  
                if (value != _selectedFile)  
                {  
                    _selectedFile = value;  
                    this.OnPropertyChanged("SelectedFileInfo");  
                }  
            }  
        }

        private void RefreshResults(){
            packages = new (GlobalVariables.DatabaseConnection.GetAllWithChildren<SimsPackage>());
            ResultsWindow.resultsView.ItemsSource = _packagesView;
            _packagesView = CollectionViewSource.GetDefaultView(packages);
        }

        private void RefreshTagViewer(){
            _tagsView = CollectionViewSource.GetDefaultView(tags);
            ResultsWindow.tagslist.ItemsSource = _tagsView;
        }

        private bool SearchFilter(object item)
		{
            SimsPackage package = (SimsPackage)item;
            string gameSearch = "";
            if (ResultsWindow.gameNum == 2){
                gameSearch = "2";
            }
            if (ResultsWindow.gameNum == 3){
                gameSearch = "3";
            }
            if (ResultsWindow.gameNum == 4){
                gameSearch = "4";
            }
            if (ResultsWindow.gameNum == 0){
                gameSearch = "";
            }

            List<string> notnull = new List<string>();

            if (!String.IsNullOrWhiteSpace(package.PackageName)) notnull.Add("PackageName");
            if (!String.IsNullOrWhiteSpace(package.Type)) notnull.Add("Type");
            if (!String.IsNullOrWhiteSpace(package.GameString)) notnull.Add("GameString");
            if (!String.IsNullOrWhiteSpace(package.Description)) notnull.Add("Description");
            if (!String.IsNullOrWhiteSpace(package.Title)) notnull.Add("Title");
            if (package.InstanceIDs != null) notnull.Add("InstancesBlobbed");
            if (!String.IsNullOrWhiteSpace(package.Category)) notnull.Add("Category");
            if (!String.IsNullOrWhiteSpace(package.ModelName)) notnull.Add("ModelName");
            if (package.GUIDs != null) notnull.Add("GuidsBlobbed");
            if (!String.IsNullOrWhiteSpace(package.Creator)) notnull.Add("Creator");
            if (!String.IsNullOrWhiteSpace(package.Age)) notnull.Add("Age");
            if (!String.IsNullOrWhiteSpace(package.Gender)) notnull.Add("Gender");
            if (package.RequiredEPs != null) notnull.Add("RequiredEPsBlob");
            if (!String.IsNullOrWhiteSpace(package.Function)) notnull.Add("Function");
            if (!String.IsNullOrWhiteSpace(package.FunctionSubcategory)) notnull.Add("FunctionSubcategory");
            if (package.RoomSort != null) notnull.Add("RoomsBlobbed");
            if (package.Flags != null) notnull.Add("FlagsBlobbed");
            if (package.CatalogTags != null) notnull.Add("CatalogTags");
            if (package.OverriddenInstances != null) notnull.Add("OverriddenInstancesBlobbed");
            if (package.OverriddenItems != null) notnull.Add("OverriddenItemsBlobbed");
            if (package.MatchingRecolors != null) notnull.Add("MatchingRecolorsBlobbed");
            if (package.MatchingConflicts != null) notnull.Add("MatchingConflictsBlobbed");
            if (!String.IsNullOrWhiteSpace(package.MatchingMesh)) notnull.Add("MatchingMesh");

            string searchcriteria = ResultsWindow.comboBox.Text;
            
			if(String.IsNullOrEmpty(ResultsWindow.searchbox.Text)){
                return true;
            } else {
                if (searchcriteria != null || String.IsNullOrEmpty(searchcriteria) || String.IsNullOrWhiteSpace(searchcriteria)){
                    foreach (string property in notnull){
                        if (property == "CatalogTags"){                            
                            foreach (TagsList tag in package.GetPropertyTagsList(property)){
                                if ((tag.TypeID.IndexOf(ResultsWindow.searchbox.Text, StringComparison.OrdinalIgnoreCase) >= 0 || tag.Description.IndexOf(ResultsWindow.searchbox.Text, StringComparison.OrdinalIgnoreCase)>= 0) && package.GameString.IndexOf(gameSearch, StringComparison.OrdinalIgnoreCase) >= 0){
                                    return true;
                                }                               
                            }                            
                        } else {
                            if ((package.GetPropertyString(property).IndexOf    (ResultsWindow.searchbox.Text, StringComparison.OrdinalIgnoreCase)>= 0) && package.GameString.IndexOf(gameSearch, StringComparison.OrdinalIgnoreCase) >= 0){
                                return true;
                            }
                        }  
                    }
                    return false;
                }
                foreach (KeyValuePair<string,string> criteria in ResultsWindow.comboboxoptions){
                    if (searchcriteria == criteria.Key){
                        if (criteria.Value == "CatalogTags"){
                            foreach (TagsList tag in package.GetPropertyTagsList(criteria.Value)){
                                if ((tag.TypeID.IndexOf(ResultsWindow.searchbox.Text, StringComparison.OrdinalIgnoreCase) >= 0 || tag.Description.IndexOf(ResultsWindow.searchbox.Text, StringComparison.OrdinalIgnoreCase)>= 0) && package.GameString.IndexOf(gameSearch, StringComparison.OrdinalIgnoreCase) >= 0){
                                    return true;
                                }
                            }
                        } else {
                            if ((package.GetPropertyString(criteria.Value).IndexOf(ResultsWindow.searchbox.Text, StringComparison.OrdinalIgnoreCase) >= 0 ) && package.GameString.IndexOf(gameSearch, StringComparison.OrdinalIgnoreCase) >= 0) {
                                return true;
                            }
                        }                        
                    }
                }
                return false;
            }                
		}



        public ICommand RenameFile  
        {  
            get { return new DelegateCommand(this.OnRename); }  
        }  
        private void OnRename()  
        {  
            string stuff = "";
            string movefolder = "";
            string sourcefile = ((SimsPackage)_packagesView.CurrentItem).Location;
            FileInfo sf = new FileInfo(((SimsPackage)_packagesView.CurrentItem).Location);
            string filename = ((SimsPackage)_packagesView.CurrentItem).PackageName;
            string destination = "";
            int selection = ResultsWindow.resultsView.SelectedItems.Count;
            if (selection == 1){                
                using(var MoveFolder = new FolderBrowserDialog())
                {
                    string newname = ((SimsPackage)_packagesView.CurrentItem).Title;
                    destination = System.IO.Path.Combine(sf.DirectoryName, newname);
                    if (File.Exists(sourcefile)){
                        SimsPackage item = (SimsPackage)_packagesView.CurrentItem;
                        item.Location = destination;
                        File.Move(sourcefile, destination);                        
                        GlobalVariables.DatabaseConnection.UpdateWithChildren(item);
                        RefreshResults();
                    } else {
                        System.Windows.Forms.MessageBox.Show(string.Format("File {0} not found at source. Did it get deleted?", sourcefile));    
                    }
                    System.Windows.Forms.MessageBox.Show(string.Format("Renamed {0}!", ((SimsPackage)_packagesView.CurrentItem).PackageName));
                    
                }            
            } else if (selection != 1 && selection != 0){
                foreach (var item in ResultsWindow.resultsView.SelectedItems){
                    SimsPackage thing = (SimsPackage)item;
                    if (String.IsNullOrEmpty(stuff)){
                        stuff = string.Format("{0}", thing.PackageName);
                    } else {
                        stuff += string.Format("\n {0}", thing.PackageName);
                    }                
                }
                using(var MoveFolder = new FolderBrowserDialog())
                {
                    foreach (var thing in ResultsWindow.resultsView.SelectedItems){
                        sf = new FileInfo(((SimsPackage)_packagesView.CurrentItem).Location);
                        SimsPackage item = ((SimsPackage)thing);
                        string newname = ((SimsPackage)_packagesView.CurrentItem).Title;
                        destination = System.IO.Path.Combine(sf.DirectoryName, newname);
                        filename = item.PackageName;
                        destination = System.IO.Path.Combine(destination, newname);
                        sourcefile = item.Location;
                        if (File.Exists(sourcefile)){                            
                            item.Location = destination;
                            File.Move(sourcefile, destination);                        
                            GlobalVariables.DatabaseConnection.UpdateWithChildren(item);
                            RefreshResults();
                        } else {
                            System.Windows.Forms.MessageBox.Show(string.Format("File not found: {0}", item.PackageName));    
                        }
                    }
                    System.Windows.Forms.MessageBox.Show(string.Format("Renamed {0} packages", ResultsWindow.resultsView.SelectedItems.Count));
                    
                }
                
            }
        }  









        public ICommand ShowTags  
        {  
            get { return new DelegateCommand(this.OnShowTags); }  
        } 
        
 
        private void OnShowTags()  
        {   
            SimsPackage item = (SimsPackage)_packagesView.CurrentItem;
            Console.WriteLine("Showing tags for " + item.PackageName);            
            foreach (TagsList tag in item.CatalogTags){
                Console.WriteLine(tag.TypeID + " | " + tag.Description);
                tags.Add(tag);
            }
            RefreshTagViewer();
            ResultsWindow.tagsgrid.Visibility = Visibility.Visible;
        }
 
        public ICommand DeleteFile  
        {  
            get { return new DelegateCommand(this.OnDelete); }  
        } 
        
 
        private void OnDelete()  
        {   
            string sourcefile = "";
            string package = "";
            string stuff = "";
            int selection = ResultsWindow.resultsView.SelectedItems.Count;
            if (selection == 1){
                SimsPackage item = (SimsPackage)_packagesView.CurrentItem;
                sourcefile = item.Location;
                package = item.PackageName;
                if (System.Windows.Forms.MessageBox.Show
                (string.Format("Are you sure you want to delete: {0}", ((SimsPackage)_packagesView.CurrentItem).PackageName), "Confirm Delete",
                System.Windows.Forms.MessageBoxButtons.YesNo, 
                System.Windows.Forms.MessageBoxIcon.Question)
                ==System.Windows.Forms.DialogResult.Yes){
                    if (File.Exists(sourcefile)){                        
                        File.Delete(sourcefile);                        
                        GlobalVariables.DatabaseConnection.Delete(item);                        
                        packages = new (GlobalVariables.DatabaseConnection.GetAllWithChildren<SimsPackage>());
                        RefreshResults();
                        System.Windows.Forms.MessageBox.Show(string.Format("Deleted {0}!", package));
                    } else {
                        System.Windows.Forms.MessageBox.Show(string.Format("File {0} not found at source. Did it get deleted?", sourcefile));    
                    }                       
                } else {
                    //do not delete
                }
            } else if (selection != 1 && selection != 0){
                foreach (var item in ResultsWindow.resultsView.SelectedItems){
                    SimsPackage thing = (SimsPackage)item;
                    if (String.IsNullOrEmpty(stuff)){
                        stuff = string.Format("{0}", thing.PackageName);
                    } else {
                        stuff += string.Format("\n {0}", thing.PackageName);
                    }                
                }
                if (System.Windows.Forms.MessageBox.Show
                (string.Format("Are you sure you want to delete: {0}", stuff), "Confirm Delete",
                System.Windows.Forms.MessageBoxButtons.YesNo, 
                System.Windows.Forms.MessageBoxIcon.Question)
                ==System.Windows.Forms.DialogResult.Yes){
                    
                    if (System.Windows.Forms.MessageBox.Show
                    (string.Format("Are you sure you want to delete: {0}", ((SimsPackage)_packagesView.CurrentItem).PackageName), "Confirm Delete",
                    System.Windows.Forms.MessageBoxButtons.YesNo, 
                    System.Windows.Forms.MessageBoxIcon.Question)
                    ==System.Windows.Forms.DialogResult.Yes){
                        foreach (var selecteditem in ResultsWindow.resultsView.SelectedItems){
                            SimsPackage item = (SimsPackage)selecteditem;
                            sourcefile = item.Location;
                            package = item.PackageName;
                            if (File.Exists(sourcefile)){                        
                                File.Delete(sourcefile);                        
                                GlobalVariables.DatabaseConnection.Delete(item);                        
                                RefreshResults();                                
                            } else {
                                System.Windows.Forms.MessageBox.Show(string.Format("File not found: {0}", sourcefile));    
                            }
                        } 
                        System.Windows.Forms.MessageBox.Show(string.Format("Deleted {0}", stuff));
                    } else {
                        //do not delete
                    }
                }
            } 
        } 
        public ICommand MoveFile  
        {  
            get { return new DelegateCommand(this.OnMove); }  
        }  
        private void OnMove()  
        {  
            string stuff = "";
            string movefolder = "";
            string sourcefile = ((SimsPackage)_packagesView.CurrentItem).Location;
            string filename = ((SimsPackage)_packagesView.CurrentItem).PackageName;
            string destination = "";
            int selection = ResultsWindow.resultsView.SelectedItems.Count;
            if (selection == 1){                
                using(var MoveFolder = new FolderBrowserDialog())
                {
                    DialogResult result = MoveFolder.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK) {
                        movefolder = MoveFolder.SelectedPath;
                        destination = System.IO.Path.Combine(movefolder, filename);
                        if (File.Exists(sourcefile)){
                            SimsPackage item = (SimsPackage)_packagesView.CurrentItem;
                            item.Location = destination;
                            File.Move(sourcefile, destination);                        
                            GlobalVariables.DatabaseConnection.UpdateWithChildren(item);
                            RefreshResults();
                        } else {
                            System.Windows.Forms.MessageBox.Show(string.Format("File {0} not found at source. Did it get deleted?", sourcefile));    
                        }
                        System.Windows.Forms.MessageBox.Show(string.Format("Moved {0}!", ((SimsPackage)_packagesView.CurrentItem).PackageName));
                    } else {
                        System.Windows.Forms.MessageBox.Show(string.Format("Please pick somewhere to move file."));
                    }
                }            
            } else if (selection != 1 && selection != 0){
                foreach (var item in ResultsWindow.resultsView.SelectedItems){
                    SimsPackage thing = (SimsPackage)item;
                    if (String.IsNullOrEmpty(stuff)){
                        stuff = string.Format("{0}", thing.PackageName);
                    } else {
                        stuff += string.Format("\n {0}", thing.PackageName);
                    }                
                }
                using(var MoveFolder = new FolderBrowserDialog())
                {
                    DialogResult result = MoveFolder.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK) {
                        movefolder = MoveFolder.SelectedPath;
                        foreach (var thing in ResultsWindow.resultsView.SelectedItems){
                            SimsPackage item = ((SimsPackage)thing);
                            filename = item.PackageName;
                            destination = System.IO.Path.Combine(movefolder, filename);
                            sourcefile = item.Location;
                            if (File.Exists(sourcefile)){                            
                                item.Location = destination;
                                File.Move(sourcefile, destination);                        
                                GlobalVariables.DatabaseConnection.UpdateWithChildren(item);
                                RefreshResults();
                            } else {
                                System.Windows.Forms.MessageBox.Show(string.Format("File not found: {0}", item.PackageName));    
                            }
                        }
                        System.Windows.Forms.MessageBox.Show(string.Format("Moved {0}", stuff));
                    } else {
                        System.Windows.Forms.MessageBox.Show(string.Format("Please pick somewhere to move file."));
                    }
                }
                
            }
        }  
  
        protected virtual void OnPropertyChanged(string propertyName)  
        {  
            PropertyChangedEventHandler handler = this.PropertyChanged;  
            if (handler != null)  
            {  
                var e = new PropertyChangedEventArgs(propertyName);  
                handler(this, e);  
            }  
        }      
        
    }

     public class DelegateCommand : ICommand  
    {  
        public delegate void SimpleEventHandler();  
        private SimpleEventHandler handler;  
        private bool isEnabled = true;  
 
        public event EventHandler CanExecuteChanged;  
 
        public DelegateCommand(SimpleEventHandler handler)  
        {  
            this.handler = handler;  
        }  
 
        private void OnCanExecuteChanged()  
        {  
            if (this.CanExecuteChanged != null)  
            {  
                this.CanExecuteChanged(this, EventArgs.Empty);  
            }  
        }  
 
        bool ICommand.CanExecute(object arg)  
        {  
            return this.IsEnabled;  
        }  
 
        void ICommand.Execute(object arg)  
        {  
            this.handler();  
        }  
 
        public bool IsEnabled  
        {  
            get { return this.isEnabled; }  
 
            set 
            {  
                this.isEnabled = value;  
                this.OnCanExecuteChanged();  
            }  
        }  
    } 
}