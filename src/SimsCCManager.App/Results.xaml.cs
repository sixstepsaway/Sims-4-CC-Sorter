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
        public int gameNum = 0;
        private bool showallfiles = false;
        private DataTable packagesDataTable = new DataTable();
        private DataTable allFilesDataTable = new DataTable();
        //public static ObservableCollection<SimsPackage> resultspackageslist = new ObservableCollection<SimsPackage>();
        CancellationTokenSource cts = new CancellationTokenSource();
        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;
        
        public ResultsWindow(CancellationTokenSource cts) 
        {
            this.cts = cts;
            log.MakeLog("Initializing results window.", true);
            InitializeComponent(); 
            log.MakeLog("Running results grid method.", true); 
            Loaded += ResultsWindow_Loaded;
            DataContext = new PackagesViewModel(); 
        }     

        private void ResultsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
        }



        private void showallfiles_Click(object sender, EventArgs e){
            /*if (showallfiles == false){
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
            }*/
            
        }

        private void CacheData(){
            
        }
        /*
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
        
        */
        //if selected --> right click "make otg > lights" --> add thing to package

        

        private void menu_Click(object sender, EventArgs e)
        {
            log.MakeLog("Closing application.", false);
            cts.Cancel();
            Thread.Sleep(2000);
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
        public event PropertyChangedEventHandler PropertyChanged;
        private PackagesViewModel _selectedFile;  

        public ICollectionView Packages
        {
            get {return _packagesView;}
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

        public PackagesViewModel(){
            packages = new (GlobalVariables.DatabaseConnection.GetAllWithChildren<SimsPackage>());
            _packagesView = CollectionViewSource.GetDefaultView(packages);
            
            /*this.Packages.SortDescriptions.Add(new SortDescription("PackageName", ListSortDirection.Ascending));
            this.Packages.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));
            this.Packages.SortDescriptions.Add(new SortDescription("Game", ListSortDirection.Ascending));
            this.Packages.SortDescriptions.Add(new SortDescription("Location", ListSortDirection.Ascending));
            this.Packages.SortDescriptions.Add(new SortDescription("Type", ListSortDirection.Ascending));
            this.Packages.SortDescriptions.Add(new SortDescription("Size", ListSortDirection.Ascending));
            this.Packages.SortDescriptions.Add(new SortDescription("Function", ListSortDirection.Ascending));
            this.Packages.SortDescriptions.Add(new SortDescription("Tags", ListSortDirection.Ascending));
            this.Packages.SortDescriptions.Add(new SortDescription("Override", ListSortDirection.Ascending));
            _packagesView.CurrentChanged += PackagesSelectionChanged;*/
            
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
 
        public ICommand DeleteFile  
        {  
            get { return new DelegateCommand(this.OnDelete); }  
        } 
 
        public ICommand MoveFile  
        {  
            get { return new DelegateCommand(this.OnMove); }  
        }  
 
        private void OnDelete()  
        {   
            var selection = _selectedFile.Cast<SimsPackage>().Count();
            var items = _packagesView.Cast<SimsPackage>().ToList();
            if (selection == 1){
                System.Windows.Forms.MessageBox.Show(string.Format("You will be deleting one item: {0}", ((SimsPackage)_packagesView.CurrentItem).PackageName));
            } else if (selection != 1 && selection != 0){
                string stuff = "";
                foreach (SimsPackage item in items){
                    if (String.IsNullOrEmpty(stuff)){
                        stuff = string.Format("{0}", item.PackageName);
                    } else {
                        stuff += string.Format("\n {0}", item.PackageName);
                    }
                }
                System.Windows.Forms.MessageBox.Show(string.Format("You will be deleting {1} items: {0}", stuff, selection));
            }
            System.Windows.Forms.MessageBox.Show(string.Format("Deleted {0}!", ((SimsPackage)_packagesView.CurrentItem).PackageName));
        }  
 
        private void OnMove()  
        {  
            string movefolder = "";
            string sourcefile = ((SimsPackage)_packagesView.CurrentItem).Location;
            string filename = ((SimsPackage)_packagesView.CurrentItem).PackageName;
            string destination = "";
            
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
                        packages = new (GlobalVariables.DatabaseConnection.GetAllWithChildren<SimsPackage>());
                        _packagesView = CollectionViewSource.GetDefaultView(packages);
                    } else {
                        System.Windows.Forms.MessageBox.Show(string.Format("File {0} not found at source. Did it get deleted?", sourcefile));    
                    }
                    
                } else {
                    System.Windows.Forms.MessageBox.Show(string.Format("Please pick somewhere to move file."));
                }
            }
            System.Windows.Forms.MessageBox.Show(string.Format("Moved {0}!", ((SimsPackage)_packagesView.CurrentItem).PackageName));

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