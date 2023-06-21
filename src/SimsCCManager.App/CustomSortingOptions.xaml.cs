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
using SimsCCManager.Packages.Sorting;
using SimsCCManager.SortingUIResults;

namespace SimsCCManager.App.CustomSortingOptions
{
    public partial class SortingOptionsWindow : Window {
        FilesSort filesSort = new();
        public static System.Windows.Controls.ListView listView = new();
        public static System.Windows.Controls.TextBox termBox = new();
        public static System.Windows.Controls.TextBox folderBox = new();
        public static System.Windows.Controls.TextBox typeBox = new();
        public static Grid savedGrid = new();

        public SortingOptionsWindow(){   
            filesSort.InitializeSortingRules();
            InitializeComponent();             
            listView = SortingOptionsView;
            termBox = TermBox;
            typeBox = TypeBox;
            folderBox = FolderBox;
            savedGrid = SavedGrid;
            DataContext = new SortingOptionsViewModel();
        }

        public void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        public void CloseWindow_Click(object sender, EventArgs e){
            this.Close();
        }
        public void CancelChanges_Click(object sender, EventArgs e){
            this.Close();
        }


    }


    public class SortingOptionsViewModel : INotifyPropertyChanged{

        private ICollectionView _RulesView;
        public event PropertyChangedEventHandler PropertyChanged;
        private SortingOptionsViewModel _selectedItem;
        private bool EditingItem = false;
        private SortingRules EditingRule = new();
        private int EditIDX = 0;

        public ICollectionView Options
        {
            get {return _RulesView; }
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

        public object Resources { get; private set; }

        ObservableCollection<SortingRules> sortingRules = new();

        public SortingOptionsViewModel(){
            sortingRules = FilesSort.SortingRules;
            _RulesView = CollectionViewSource.GetDefaultView(sortingRules);
            //SortingOptionsWindow.listView.ItemsSource = _RulesView;            
        }  
        public void RefreshList(){
            _RulesView = CollectionViewSource.GetDefaultView(sortingRules);
            SortingOptionsWindow.listView.ItemsSource = _RulesView;  
        }

        public SortingOptionsViewModel SelectedFileInfo  
        {  
            get { return _selectedItem; }  
            set 
            {  
                if (value != _selectedItem)  
                {  
                    _selectedItem = value;  
                    this.OnPropertyChanged("SelectedFileInfo");  
                }  
            }  
        }

        public bool Ascending = false;
        public string SortedBy = "MatchTerm";

        public ICommand HeaderMatchTerm {
            get {return new DelegateCommand(this.Sort_MatchTerm);}
        }
        public ICommand HeaderFolder {
            get {return new DelegateCommand(this.Sort_Folder);}
        }
        public ICommand HeaderType {
            get {return new DelegateCommand(this.Sort_Type);}
        }

        private void Sort_MatchTerm(){
            if (SortedBy != "MatchTerm"){
                Ascending = false;                
            }
            SortedBy = "MatchTerm";
            if (Ascending == true){
                Ascending = false;
                var rules = sortingRules.OrderByDescending(x => x.MatchTerm).ToList();
                sortingRules = new(rules);
            } else {
                Ascending = true;
                var rules = sortingRules.OrderBy(x => x.MatchTerm).ToList();
                sortingRules = new(rules);
            }
            RefreshList();
        }
        
        private void Sort_Folder(){
            if (SortedBy != "Folder"){
                Ascending = false;                
            }
            SortedBy = "Folder";
            if (Ascending == true){
                Ascending = false;
                var rules = sortingRules.OrderByDescending(x => x.Folder).ToList();
                sortingRules = new(rules);
            } else {
                Ascending = true;
                var rules = sortingRules.OrderBy(x => x.Folder).ToList();
                sortingRules = new(rules);
            }            
            RefreshList();
        }
        
        private void Sort_Type(){
            if (SortedBy != "Folder"){
                Ascending = false;                
            }
            SortedBy = "Folder";
            if (Ascending == true){
                Ascending = false;
                var rules = sortingRules.OrderByDescending(x => x.MatchType).ToList();
                sortingRules = new(rules);
            } else {
                Ascending = true;
                var rules = sortingRules.OrderBy(x => x.MatchType).ToList();
                sortingRules = new(rules);
            }
            RefreshList();
        }

        public ICommand SaveRules {
            get {return new DelegateCommand(this.SaveUpdatedRules);}
        }

        private void SaveUpdatedRules(){
            FilesSort.SortingRules = sortingRules;
            using (StreamWriter file = File.CreateText(GlobalVariables.CustomSortingOptions))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, sortingRules);
            }
            SortingOptionsWindow.savedGrid.Visibility = Visibility.Visible;
        }

        public ICommand CloseSavedWindow
        {
            get {return new DelegateCommand(this.CloseSWindow);}
        }

        public void CloseSWindow(){
            SortingOptionsWindow.savedGrid.Visibility = Visibility.Hidden;
        }

        
        public ICommand CancelChanges {
            get {return new DelegateCommand(this.CancelUpdating);}
        }

        private void CancelUpdating(){
            sortingRules = FilesSort.SortingRules;
            _RulesView = CollectionViewSource.GetDefaultView(sortingRules);
            SortingOptionsWindow.listView.ItemsSource = _RulesView;
        }

        public ICommand SubmitRule
        {
            get { return new DelegateCommand(this.AddRule); }  
        }

        private void AddRule()
        {   
            if (EditingItem == true){
                if (!string.IsNullOrEmpty(SortingOptionsWindow.termBox.Text) && !string.IsNullOrWhiteSpace(SortingOptionsWindow.termBox.Text) && !string.IsNullOrEmpty(SortingOptionsWindow.folderBox.Text) && !string.IsNullOrWhiteSpace(SortingOptionsWindow.folderBox.Text)){
                    sortingRules[EditIDX].MatchTerm = SortingOptionsWindow.termBox.Text;
                    sortingRules[EditIDX].MatchType = SortingOptionsWindow.typeBox.Text;
                    sortingRules[EditIDX].Folder = SortingOptionsWindow.folderBox.Text;
                    RefreshList();
                    RefreshList();
                    EditingItem = false;
                    SortingOptionsWindow.termBox.Text = "";
                    SortingOptionsWindow.typeBox.Text = "";
                    SortingOptionsWindow.folderBox.Text = "";
                    MoveToBottom();
                }
                
            } else {
                if (!string.IsNullOrEmpty(SortingOptionsWindow.termBox.Text) && !string.IsNullOrWhiteSpace(SortingOptionsWindow.termBox.Text) && !string.IsNullOrEmpty(SortingOptionsWindow.folderBox.Text) && !string.IsNullOrWhiteSpace(SortingOptionsWindow.folderBox.Text)){
                    sortingRules.Add(new SortingRules(){MatchTerm = SortingOptionsWindow.termBox.Text, Folder = SortingOptionsWindow.folderBox.Text, MatchType = SortingOptionsWindow.typeBox.Text});
                    RefreshList();
                    SortingOptionsWindow.termBox.Text = "";
                    SortingOptionsWindow.typeBox.Text = "";
                    SortingOptionsWindow.folderBox.Text = "";
                    MoveToBottom();
                }                
            }            
        }

        void MoveToBottom(){
            SortingOptionsWindow.listView.SelectedIndex = SortingOptionsWindow.listView.Items.Count -1;
            SortingOptionsWindow.listView.ScrollIntoView(SortingOptionsWindow.listView.SelectedItem) ;
        }

        public ICommand DeleteItem
        {
            get { return new DelegateCommand(this.DeleteAnItem); }
        }

        private void DeleteAnItem(){
            SortingRules rule = (SortingRules) _RulesView.CurrentItem;
            int idx = sortingRules.IndexOf(rule);
            sortingRules.RemoveAt(idx);
            RefreshList();
        }
        public ICommand EditItem
        {
            get { return new DelegateCommand(this.EditAnItem); }
        }
        private void EditAnItem(){
            EditingItem = true;            
            EditingRule = (SortingRules) _RulesView.CurrentItem;
            int EditIDX = sortingRules.IndexOf(EditingRule);
            SortingOptionsWindow.termBox.Text = EditingRule.MatchTerm;
            SortingOptionsWindow.typeBox.Text = EditingRule.MatchType;
            SortingOptionsWindow.folderBox.Text = EditingRule.Folder;
        }
        public ICommand DuplicateItem
        {
            get { return new DelegateCommand(this.DuplicateAnItem); }
        }
        private void DuplicateAnItem(){
            var item = (SortingRules) _RulesView.CurrentItem;
            sortingRules.Add(new SortingRules(){MatchTerm = item.MatchTerm, Folder = item.Folder, MatchType = item.MatchType});
            RefreshList();
            MoveToBottom();
        }
        public ICommand ResetDefault
        {
            get { return new DelegateCommand(this.DefaultRules); }
        }
        private void DefaultRules(){
            using (StreamReader file = File.OpenText(GlobalVariables.SortingOptionsDefault))
            {
                JsonSerializer serializer = new JsonSerializer();
                sortingRules = (ObservableCollection<SortingRules>)serializer.Deserialize(file, typeof(ObservableCollection<SortingRules>));
            }
            RefreshList();
        }
    }
}