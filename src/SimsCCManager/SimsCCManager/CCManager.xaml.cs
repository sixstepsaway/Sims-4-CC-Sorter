using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using SimsCCManager.UI.Utilities;
using System.ComponentModel;
using SimsCCManager.Settings;
using System.IO;
using SimsCCManager.Packages.Initial;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using System.Dynamic;
using System.Windows.Input;
using System.Windows.Forms;
using SSAGlobals;
using System.ServiceModel;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Windows.Xps.Packaging;
using System.Runtime.Versioning;
using System.Windows.Data;
using System.Windows.Threading;
using System.Windows.Media;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms.VisualStyles;
using System.Reflection.Metadata.Ecma335;
using System.Windows.Controls;
using System.Collections;
using MoreLinq;
using SimsCCManager.Packages.Containers;
using Skybrud.Colors;
using System.Text;

namespace SimsCCManager.Manager
{
    public partial class ManagerWindow : Window
    {
        public ManagerWindow(){            
            base.DataContext = new ManagerWindowViewModel();
            InitializeComponent();
            GlobalVariables.WindowsOpen++;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {     
            if (GlobalVariables.BackgroundTasksRunning == true){
                var result = System.Windows.MessageBox.Show("There are background tasks running. Are you sure you want to close?",  "Background Tasks Running", MessageBoxButton.YesNo);
                if (result != MessageBoxResult.Yes)
                {
                    e.Cancel = true;
                } else {
                    GlobalVariables.WindowsOpen--;
                    Console.WriteLine("Windows Open: {0}", GlobalVariables.WindowsOpen);
                    if (GlobalVariables.WindowsOpen == 0) System.Windows.Application.Current.Shutdown();
                }
            } else {
                GlobalVariables.WindowsOpen--;
                Console.WriteLine("Windows Open: {0}", GlobalVariables.WindowsOpen);
                if (GlobalVariables.WindowsOpen == 0) System.Windows.Application.Current.Shutdown();
            }
            
            
        }

        void SwapInstances_Click(object sender, RoutedEventArgs e){
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();            
        }
    }

    public class ManagerWindowViewModel : INotifyPropertyChanged
    {
        LoggingGlobals log = new();
        private Setting InstanceSettings = new();
        private InstanceData _thisinstance;
        
        //get over here this INSTANCE >:(
        
        public InstanceData ThisInstance{
            get { return _thisinstance; }
            set { _thisinstance = value;
                RaisePropertyChanged("ThisInstance");
                ActiveExe = value.ActiveExe;
                if (Profiles != null) ThisProfile = Profiles.Where(x => x.Name == ThisInstance.ActiveProfile).First();
                log.MakeLog("Property changed: ThisInstance", true); }
        }

        private string _activeexe;
        public string ActiveExe {
            get { return _activeexe; }
            set { _activeexe = value;
                RaisePropertyChanged("ActiveExe");}
        }

        private List<ModInfo> _instancemods;
        public List<ModInfo> InstanceMods {
            get { return _instancemods; }
            set { _instancemods = value;
                RaisePropertyChanged("InstanceMods");
                if (initiallyloaded == true) ThingsChangedUpdate();
                log.MakeLog("Property changed: InstanceMods", true);}
        }

        private List<SimsPackage> _detailedmods;
        public List<SimsPackage> DetailedMods{
            get { return _detailedmods; }
            set { _detailedmods = value;
                RaisePropertyChanged("InstanceMods");
                //if (initiallyloaded == true) ThingsChangedUpdate();
                //log.MakeLog("Property changed: InstanceMods", true);
                }
        }

        private List<ModInfo> _enabledmods;
        public List<ModInfo> EnabledMods {
            get { return _enabledmods; }
            set { _enabledmods = value;
                RaisePropertyChanged("EnabledMods");
                if (initiallyloaded == true) EnabledModsCheck();
                if (initiallyloaded == true) SaveProfileData();                
                log.MakeLog("Property changed: EnabledMods", true);}
        }

        private List<NewDownloadInfo> _downloads;
        public List<NewDownloadInfo> Downloads {
            get { return _downloads; }
            set { _downloads = value;
                RaisePropertyChanged("Downloads");    
                log.MakeLog("Property changed: Downloads", true);}
        }

        private List<Executable> _executables;
        public List<Executable> Executables{
            get { return _executables; }
            set { _executables = value;
                RaisePropertyChanged("Executables");
                log.MakeLog("Property changed: Executables", true);}
        }

        private Profile _currentprofile;
        public Profile CurrentProfile {
            get { return _currentprofile; }
            set { _currentprofile = value;
                RaisePropertyChanged("CurrentProfile");
                if (initiallyloaded == true) LoadProfileData();
                log.MakeLog("Property changed: CurrentProfile", true);}
        }

        private List<Profile> _profiles;
        public List<Profile> Profiles {
            get { return _profiles; }
            set { _profiles = value;
                RaisePropertyChanged("Profiles");
                log.MakeLog("Property changed: Profiles", true);}
        }

        private Profile _profile;
        public Profile ThisProfile {
            get { return _profile; }
            set{ _profile = value;
                RaisePropertyChanged("ThisProfile");
                if (initiallyloaded == true) LoadProfileData();
                log.MakeLog("Property changed: ThisProfile", true);}
        }

        private List<CreatorInfo> _creators;
        public List<CreatorInfo> Creators {
            get { return _creators; }
            set { _creators = value; 
            RaisePropertyChanged("Creators");
            SaveCreators();}
        }


        private double _smallw;
        public double SmallSizeW{
            get { return _smallw;}
            set { _smallw = value; 
                RaisePropertyChanged("SmallSizeW");
                log.MakeLog("Property changed: SmallSizeW", true);}
        }
        private double _smallh;
        public double SmallSizeH{
            get { return _smallh;}
            set { _smallh = value; 
                RaisePropertyChanged("SmallSizeH");
                log.MakeLog("Property changed: SmallSizeH", true);}
        }  

        private int _border;
        public int Border{
            get { return _border;}
            set { _border = value; 
                RaisePropertyChanged("Border");
                log.MakeLog("Property changed: Border", true);}
        }           
        
        
        private bool _maximized;
        public bool Maximized{
            get { return _maximized; }
            set {_maximized = value;
                RaisePropertyChanged("Maximized");
                log.MakeLog("Property changed: ThisInstance", true);}
        }

        private double _wwidth;
        public double WindowWidth{
            get { return _wwidth; }
            set { _wwidth = value;
                RaisePropertyChanged("WindowWidth");
                log.MakeLog("Property changed: WindowWidth", true);}
        }

        private double _wheight;
        public double WindowHeight{
            get { return _wheight; }
            set { _wheight = value;
                RaisePropertyChanged("WindowHeight"); 
                log.MakeLog("Property changed: WindowHeight", true);}
        }
        
        private System.Windows.Point _loc;
        public System.Windows.Point Location
        {
            get { return _loc; }
            set { _loc = value;
                RaisePropertyChanged("Location"); 
                log.MakeLog("Property changed: Location", true);}
        }

        private Visibility _windowvisible;
        public Visibility WindowVisible{
            get { return _windowvisible;}
            set { _windowvisible = value;
            RaisePropertyChanged("WindowVisible");}
        }
        private Visibility _foundzipsgridvis;
        public Visibility FoundZipsGridVis{
            get { return _foundzipsgridvis;}
            set { _foundzipsgridvis = value;
            RaisePropertyChanged("FoundZipsGridVis");}
        }

        public string _gameicon;
        public string GameIcon{
            get { return _gameicon;}
            set { _gameicon = value;
            RaisePropertyChanged("GameIcon");}
        }      

        public string _searcham;
        public string SearchAM{
            get { return _searcham;}
            set { _searcham = value;
            RaisePropertyChanged("SearchAM");
            InstanceModsCV.Refresh();}
        }   
        public string _searchem;
        public string SearchEM{
            get { return _searchem;}
            set { _searchem = value;
            RaisePropertyChanged("SearchEM");
            EnabledModsCV.Refresh();}
        }    
        public string _searchdl;
        public string SearchDL{
            get { return _searchdl;}
            set { _searchdl = value;
            RaisePropertyChanged("SearchDL");
            DownloadsCV.Refresh();}
        }  
        private ModInfo _cellclicked;

        public ModInfo CellClicked{
            get { return _cellclicked;}
            set { _cellclicked = value;
            RaisePropertyChanged("CellClicked");
            }
        }

        private Visibility _editscreenvis;
        public Visibility EditScreenVis{
            get { return _editscreenvis;}
            set { _editscreenvis = value;
                RaisePropertyChanged("EditScreenVis");
            }
        }
        private Visibility _popupgreyed;
        public Visibility PopupGreyed{
            get { return _popupgreyed;}
            set { _popupgreyed = value;
                RaisePropertyChanged("PopupGreyed");
            }
        }
        private Visibility _progressbargridvis;
        public Visibility ProgressBarGridVis{
            get { return _progressbargridvis;}
            set { _progressbargridvis = value;
                RaisePropertyChanged("ProgressBarGridVis");
            }
        }

        private bool _progindet;
        public bool ProgIndet{
            get { return _progindet;}
            set { _progindet = value;
            RaisePropertyChanged("ProgIndet");
            }
        }

        private ModInfo _editingmod;
        public ModInfo EditingMod{
            get { return _editingmod;}
            set { _editingmod = value;
                RaisePropertyChanged("EditingMod");
            }
        }

        private List<CategoryType> _categories;
        public List<CategoryType> Categories{
            get { return _categories;}
            set { _categories = value;
                RaisePropertyChanged("Categories");
                if (initiallyloaded == true) SaveInstanceInfo();
            }
        }

        private Visibility _newcategoryvis;
        public Visibility NewCategoryVis{
            get { return _newcategoryvis;}
            set { _newcategoryvis = value;
                RaisePropertyChanged("NewCategoryVis");
            }
        }
        private string _newcategoryname;
        public string NewCategoryName{
            get { return _newcategoryname;}
            set { _newcategoryname = value;
                RaisePropertyChanged("NewCategoryName");
            }
        }
        private string _newcategorydescription;
        public string NewCategoryDescription{
            get { return _newcategorydescription;}
            set { _newcategorydescription = value;
                RaisePropertyChanged("NewCategoryDescription");
            }
        }

        private string _newcategorycolor;
        public string NewCategoryColor{
            get { return _newcategorycolor;}
            set { _newcategorycolor = value;
                RaisePropertyChanged("NewCategoryColor");
            }
        }
        private string _newcategoryaltcolor;
        public string NewCategoryAltColor{
            get { return _newcategoryaltcolor;}
            set { _newcategoryaltcolor = value;
                RaisePropertyChanged("NewCategoryAltColor");
            }
        }

        private string NewCategoryFG = "";
        private string NewCategoryAltFG = "";

        private System.Windows.Media.Color _newcategorycolorbrush;
        public System.Windows.Media.Color NewCategoryColorBrush{
            get { return _newcategorycolorbrush;}
            set { _newcategorycolorbrush = value;
                RaisePropertyChanged("NewCategoryColorBrush");
            }
        }
        private System.Windows.Media.Color _newcategoryaltcolorbrush;
        public System.Windows.Media.Color NewCategoryAltColorBrush{
            get { return _newcategoryaltcolorbrush;}
            set { _newcategoryaltcolorbrush = value;
                RaisePropertyChanged("NewCategoryAltColorBrush");
            }
        }

        private RgbColor _newcategoryrgb;
        public RgbColor NewCategoryRGB{
            get { return _newcategoryrgb; }
            set { _newcategoryrgb = value;
            RaisePropertyChanged("NewCategoryRGB");}
        }


        private Visibility _categoryscreenvis;
        public Visibility CategoryScreenVis{
            get { return _categoryscreenvis;}
            set { _categoryscreenvis = value;
                RaisePropertyChanged("CategoryScreenVis");
            }
        }

        private string _newcatlabel;
        public string NewCatLabel{
            get { return _newcatlabel;}
            set { _newcatlabel = value;
                RaisePropertyChanged("NewCatLabel");
            }
        }

        

        private Visibility _newprofilevis;
        public Visibility NewProfileVis{
            get { return _newprofilevis;}
            set { _newprofilevis = value;
                RaisePropertyChanged("NewProfileVis");
            }
        }
        private string _newprofilename;
        public string NewProfileName{
            get { return _newprofilename;}
            set { _newprofilename = value;
                RaisePropertyChanged("NewProfileName");
            }
        }

        private Visibility _profilescreenvis;
        public Visibility ProfileScreenVis{
            get { return _profilescreenvis;}
            set { _profilescreenvis = value;
                RaisePropertyChanged("ProfileScreenVis");
            }
        }

        private string _newprofilelabel;
        public string NewProfileLabel{
            get { return _newprofilelabel;}
            set { _newprofilelabel = value;
                RaisePropertyChanged("NewProfileLabel");
            }
        }

        private bool _newprofilelocalsavescheck;
        public bool NewProfileLocalSavesCheck{
            get { return _newprofilelocalsavescheck;}
            set { _newprofilelocalsavescheck = value;
                RaisePropertyChanged("NewProfileLocalSavesCheck");
            }
        }
        private bool _newprofilelocalsettingscheck;
        public bool NewProfileLocalSettingsCheck{
            get { return _newprofilelocalsettingscheck;}
            set { _newprofilelocalsettingscheck = value;
                RaisePropertyChanged("NewProfileLocalSettingsCheck");
            }
        }
        
        private bool _newprofilelocaltraycheck;
        public bool NewProfileLocalTrayCheck{
            get { return _newprofilelocaltraycheck;}
            set { _newprofilelocaltraycheck = value;
                RaisePropertyChanged("NewProfileLocalTrayCheck");
            }
        }
        private bool _newprofilelocalscreenshotscheck;
        public bool NewProfileLocalScreenshotsCheck{
            get { return _newprofilelocalscreenshotscheck;}
            set { _newprofilelocalscreenshotscheck = value;
                RaisePropertyChanged("NewProfileLocalScreenshotsCheck");
            }
        }
        
        private string _backgroundtask;
        public string BackgroundTask{
            get { return _backgroundtask;}
            set { _backgroundtask = value;
                RaisePropertyChanged("BackgroundTask");
            }
        }

        private int _infoboxheight;
        public int InfoBoxHeight {
            get { return _infoboxheight;}
            set { _infoboxheight = value;
                RaisePropertyChanged("InfoBoxHeight");
            }
        }

        private CategoryType _categorypick;
        public CategoryType CategoryPick{
            get { return _categorypick;}
            set { _categorypick = value;
                RaisePropertyChanged("CategoryPick");
            }
        }

        private IList _selectedmods = new List<ModInfo>();
        public IList SelectedMods {
            get { return _selectedmods;}
            set { _selectedmods = value;
                RaisePropertyChanged("SelectedMods");
                IMSelectedModChanged();
            }
        }
        private IList _selectedmodsdetailed = new List<SimsPackage>();
        public IList SelectedModsDetailed {
            get { return _selectedmodsdetailed;}
            set { _selectedmodsdetailed = value;
                RaisePropertyChanged("SelectedModsDetailed");
            }
        }

        private string _namechange;
        public string NameChange {
            get { return _namechange;}
            set { _namechange = value;
                RaisePropertyChanged("NameChange");
            }
        }
        private string _progbarlabel;
        public string ProgBarLabel {
            get { return _progbarlabel;}
            set { _progbarlabel = value;
                RaisePropertyChanged("ProgBarLabel");
            }
        }

        private List<ModInfo> _groups;
        public List<ModInfo> Groups{
            get { return _groups; }
            set {
                _groups = value;
                RaisePropertyChanged("Groups");
            }
        }

        
        private string _usedcat;
        public string UsedCat{
            get { return _usedcat; }
            set {
                _usedcat = value;
                RaisePropertyChanged("UsedCat");
            }
        }
        
        private string _viewtoggletxt;
        public string ViewToggleText{
            get { return _viewtoggletxt;}
            set { _viewtoggletxt = value;
                RaisePropertyChanged("ViewToggleText");
            }
        }

        private Visibility _detailedviewvis;
        public Visibility DetailedViewVis{
            get {return _detailedviewvis; }
            set {_detailedviewvis = value;
            RaisePropertyChanged("DetailedViewVis");}
        }
        private Visibility _simpleviewvis;
        public Visibility SimpleViewVis{
            get {return _simpleviewvis; }
            set {_simpleviewvis = value;
            RaisePropertyChanged("SimpleViewVis");}
        }

        private bool AltColor = false;



        private int editingmodnum = -1;



        //CollectionViews

        public ICollectionView InstanceModsCV
        {    
            get { return CollectionViewSource.GetDefaultView(InstanceMods); } 
            set {
                InstanceModsCV.CurrentChanged += new EventHandler(IMSelectionChanged);
            }   
        }  

        public ICollectionView DetailedModsCV
        {    
            get { return CollectionViewSource.GetDefaultView(DetailedMods); } 
            set {
                DetailedModsCV.CurrentChanged += new EventHandler(DMSelectionChanged);
            }   
        } 

        public ICollectionView EnabledModsCV
        {    
            get { return CollectionViewSource.GetDefaultView(EnabledMods); }   
            set {
                EnabledModsCV.CurrentChanged += new EventHandler(EMSelectionChanged);
            }  
        }
        public ICollectionView DownloadsCV
        {    
            get { return CollectionViewSource.GetDefaultView(Downloads); }    
            set {
                DownloadsCV.CurrentChanged += new EventHandler(DLSelectionChanged);
            } 
        }
        public ICollectionView CategoriesCV
        {    
            get { return CollectionViewSource.GetDefaultView(Categories); }    
            set {
                CategoriesCV.CurrentChanged += new EventHandler(CategoriesChanged);
            } 
        }
        public ICollectionView ProfilesCV
        {    
            get { return CollectionViewSource.GetDefaultView(Profiles); }    
            set {
                ProfilesCV.CurrentChanged += new EventHandler(ProfilesChanged);
            } 
        }



        //Assorted variables for tracking  

        private string profilesfile {get; set;}
        private string settingsfile {get; set;}
        private string modlistfile {get; set;}
        private string executablesfile {get; set;}
        private string instancefile {get; set;}
        private List<string> extensions = new(){".package", ".sims2pack", ".sims3pack", ".ts4script"};
        private List<string> compressedext = new(){".rar", ".zip", ".7z", ".pkg"};
        private bool AutoMoveCompressed = false;
        private bool AskedAboutMove = false;
        private List<string> enabledmodslist {get; set;}
        private bool initiallyloaded {get; set;}
        InitialProcessing ip = new();
        private int infoboxopen = 200;







        #region Main View Model

        public ManagerWindowViewModel(){
            Console.WriteLine("Initializing manager view model.");
            Begin();
        }

        private void Begin(){
            initiallyloaded = false;
            FoundZipsGridVis = Visibility.Hidden;
            PopupGreyed = Visibility.Hidden;
            if(SettingsFile.LoadSetting("AutoMoveCompressedFiles") == "true") AutoMoveCompressed = true;
            if(SettingsFile.LoadSetting("AskedAboutMove") == "true") AskedAboutMove = true;
            ThisInstance = new();
            ActiveExe = "exe";
            Border = 5;
            SmallSizeW = 1000;
            SmallSizeH = 600;
            WindowHeight = SmallSizeH;
            WindowWidth = SmallSizeW;   
            ThisProfile = new(); 
            ThisInstance = new();                    
            Profiles = new();
            InstanceMods = new();
            EnabledMods = new();
            Downloads = new();
            enabledmodslist = new();
            Executables = new();
            Categories = new();
            EditScreenVis = Visibility.Hidden;
            NewCategoryVis = Visibility.Hidden;
            NewProfileVis = Visibility.Hidden;
            ProfileScreenVis = Visibility.Hidden;
            CategoryScreenVis = Visibility.Hidden;
            SimpleViewVis = Visibility.Visible;
            DetailedViewVis = Visibility.Hidden;
            ViewToggleText = "Simple";
            InfoBoxHeight = 0;
            Groups = new();
            Location = new System.Windows.Point(SystemParameters.PrimaryScreenWidth / 4, SystemParameters.PrimaryScreenHeight / 4);
            string instance = SettingsFile.LoadSetting("LastInstance");
            ProgIndet = true;
            ProgressBarGridVis = Visibility.Hidden;
            if (!File.Exists(Path.Combine(instance, @"data\modlist.ini"))){
                CreateFileTree(instance);                
            }
            LoadInstance(instance);            
            CreateFilters();
            MakeRelays();
            initiallyloaded = true;
        }

        #endregion

        #region CollectionView Handling

        private void CreateFilters(){
            InstanceModsCV.Filter = new Predicate<object>(o => AMFilter(o as ModInfo));
            EnabledModsCV.Filter = new Predicate<object>(o => EMFilter(o as ModInfo));
            DownloadsCV.Filter = new Predicate<object>(o => DLFilter(o as ModInfo));
            InstanceModsCV.CurrentChanged += IMSelectionChanged;
            EnabledModsCV.CurrentChanged += EMSelectionChanged;
            DownloadsCV.CurrentChanged += DLSelectionChanged;
        }

        private bool AMFilter(ModInfo mod)    
        {    
            return SearchAM == null    
                || GetSearchIndex(mod.Name, SearchAM) != -1    
                || GetSearchIndex(mod.Location, SearchAM) != -1    
                || GetSearchIndex(mod.Game, SearchAM) != -1;    
        }

        private bool EMFilter(ModInfo mod)    
        {    
            return SearchEM == null    
                || GetSearchIndex(mod.Name, SearchEM) != -1    
                || GetSearchIndex(mod.Location, SearchEM) != -1    
                || GetSearchIndex(mod.Game, SearchEM) != -1;    
        }
        
        private bool DLFilter(ModInfo mod)    
        {    
            return SearchDL == null    
                || GetSearchIndex(mod.Name, SearchDL) != -1    
                || GetSearchIndex(mod.Location, SearchDL) != -1    
                || GetSearchIndex(mod.Game, SearchDL) != -1;    
        }

        private int GetSearchIndex(dynamic input, string match){
            //var inp = input;
            if (input != null) {
                return input.IndexOf(match, StringComparison.OrdinalIgnoreCase);
            } else {
                return -1;
            }
        }
        private void AddToInstanceModsCollection(ModInfo modInfo){
            if (!InstanceMods.Contains(modInfo)){
                ModInfo minf = modInfo;
                minf.EditingName = false;
                if (AltColor == true){
                    minf.AltColor = true;
                    AltColor = false;
                } else {
                    minf.AltColor = false;
                    AltColor = true;
                }
                InstanceMods.Add(minf);
                InstanceModsCV.Refresh();
            }
        }
        private void AddToEnabledModsCollection(ModInfo modInfo){            
            if (!EnabledMods.Contains(modInfo)){
                modInfo.DateEnabled = DateTime.Now;
                EnabledMods.Add(modInfo);
                EnabledModsCV.Refresh();
            }
            if (!ThisProfile.EnabledMods.Contains(modInfo)){
                ThisProfile.EnabledMods.Add(modInfo);
            }               
        }

        bool IMascending = true;

        private void IM_SortByName(){
            if (IMascending){
                IMascending = false;
                InstanceModsCV.SortDescriptions.Clear();
                InstanceModsCV.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Descending));
            } else {                
                InstanceModsCV.SortDescriptions.Clear();
                InstanceModsCV.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                IMascending = true;
            }            
        }
        
        bool EMascending = true;
        private void EM_SortByName(){
            if (EMascending){
                EMascending = false;
                EnabledModsCV.SortDescriptions.Clear();
                EnabledModsCV.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Descending));
            } else {                
                EnabledModsCV.SortDescriptions.Clear();
                EnabledModsCV.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                EMascending = true;
            }           
        }
        
        bool DLascending = true;
        private void DL_SortByName(){
            if (DLascending){
                DLascending = false;
                DownloadsCV.SortDescriptions.Clear();
                DownloadsCV.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Descending));
            } else {                
                DownloadsCV.SortDescriptions.Clear();
                DownloadsCV.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                DLascending = true;
            }           
        }

        private void SelectionClick(){
            Console.WriteLine("Selection changed via check!");
            OpenPanel();
        }

        private void IMSelectionChanged(object sender, EventArgs e)
        {               
            /*ModInfo mod = InstanceModsCV.CurrentItem as ModInfo;  
            int x = InstanceMods.IndexOf(mod);
            ModInfo m = InstanceMods[x];
            if (m.IsSelected == true){
                InstanceMods[x].IsSelected = false;
            } else {
                InstanceMods[x].IsSelected = true;
            }
            Console.WriteLine("Selection changed via click!");
            OpenPanel();*/
        }

        private void DMSelectionChanged(object sender, EventArgs e){

        }

        private void OpenPanel(){
            /*int selectednum = InstanceMods.Where(x => x.IsSelected).Count();
            Console.WriteLine("Number of selected items: {0}", selectednum);
            if (selectednum == 0) {
                InfoBoxHeight = 0;
            } else if (selectednum == 1){
                ModInfo selectedmod = InstanceMods.Where(x => x.IsSelected).First();
                if (selectedmod.Root == true){
                    RootCheckbox = true;
                } else {
                    RootCheckbox = false;
                }
                if (selectedmod.OutOfDate == true){
                    OODCheckbox = true;
                } else {
                    OODCheckbox = false;
                }
                if (selectedmod.Fave == true){
                    FaveCheckbox = true;
                } else {
                    FaveCheckbox = false;
                }
                CategoryPick = Categories.Where(x => x == selectedmod.Category).First();
                InfoBoxHeight = infoboxopen;
            } else if (selectednum > 1){
                List<ModInfo> selected = InstanceMods.Where(x => x.IsSelected).ToList();
                CategoryType acat = selected[0].Category;
                ModInfo item = selected[0];
                if (selected.Where(x => x.Root == item.Root).Count() != selectednum){
                    RootCheckbox = null;
                } else {
                    RootCheckbox = selected[0].Root;
                }
                if (selected.Where(x => x.OutOfDate == item.OutOfDate).Count() != selectednum){
                    OODCheckbox = null;
                } else {
                    OODCheckbox = selected[0].OutOfDate;
                }
                if (selected.Where(x => x.Fave == item.Fave).Count() != selectednum){
                    FaveCheckbox = null;
                } else {
                    FaveCheckbox = selected[0].Fave;
                }                
                if (selected.Where(x => x.Category == acat).Count() != selectednum){
                    CategoryPick = new CategoryType();
                } else {
                    CategoryPick = acat;
                }
                InfoBoxHeight = infoboxopen;                
            }*/
        }        

        private void EMSelectionChanged(object sender, EventArgs e)
        {
        // React to the changed selection
        }

        private void DLSelectionChanged(object sender, EventArgs e)
        {
        // React to the changed selection
        }
        private void CategoriesChanged(object sender, EventArgs e)
        {
        // React to the changed selection
        }

        private void ProfilesChanged(object sender, EventArgs e)
        {
        // React to the changed selection
        }
        private void AMDoubleClick(object sender, EventArgs e){

        }

        private void RefreshAllCollections(){
            InstanceModsCV.Refresh();
            EnabledModsCV.Refresh();
            DownloadsCV.Refresh(); 
        }

        #endregion




        #region Variable Handling

        
        









        #endregion


        #region Property Changed Handling

        private void IMSelectedModChanged(){
            if (SelectedMods != null){
                Console.WriteLine("Selected: {0}", SelectedMods.Count);
                if (SelectedMods.Count == 1){
                    ModInfo mod = SelectedMods[0] as ModInfo;
                    Console.WriteLine("Selected mod color: {0}", mod.Category.ColorHex);
                    Console.WriteLine("Selected mod alt color: {0}", mod.Category.ColorHexAlt);
                    Console.WriteLine("Selected mod font color: {0}", mod.Category.ColorHexFG);
                    Console.WriteLine("Selected mod alt color: {0}", mod.Category.ColorHexAltFG);                    
                }
                if (SelectedMods.Count == 0){
                    if (InstanceMods.Where(x => x.EditingName == true).Any()){
                        List<ModInfo> en = InstanceMods.Where(x => x.EditingName == true).ToList();
                        foreach (ModInfo info in en){
                            int idx = InstanceMods.IndexOf(info);
                            InstanceMods[idx].EditingName = false;
                        }
                    }
                } else if (SelectedMods.Count == 1){
                    ModInfo sm = SelectedMods[0] as ModInfo;
                    ModInfo m = InstanceMods.Where(x => x.Name == sm.Name).First();
                    int idx = InstanceMods.IndexOf(m);
                    if (InstanceMods.Where(x => x.EditingName == true).Any()){
                        List<ModInfo> en = InstanceMods.Where(x => x.EditingName == true).ToList();
                        foreach (ModInfo info in en){
                            int ix = InstanceMods.IndexOf(info);
                            if (ix != idx){
                                InstanceMods[ix].EditingName = false;
                            }                            
                        }
                    }
                    if (InstanceMods.Where(x => x.Selected == true).Any()){
                        List<ModInfo> en = InstanceMods.Where(x => x.Selected == true).ToList();
                        en = InstanceMods.Where(x => x.Selected == true).ToList();
                        foreach (ModInfo info in en){
                            int ix = InstanceMods.IndexOf(info);
                            if (ix != idx){
                                InstanceMods[ix].Selected = false;
                            }                            
                        }
                    }
                    InstanceMods[idx].Selected = true;                    
                } else {
                    List<int> indxs = new();
                    foreach (ModInfo mm in SelectedMods){
                        indxs.Add(InstanceMods.IndexOf(mm));
                    }
                    if (InstanceMods.Where(x => x.EditingName == true).Any()){
                        List<ModInfo> en = InstanceMods.Where(x => x.EditingName == true).ToList();
                        foreach (ModInfo info in en){
                            int ix = InstanceMods.IndexOf(info);
                            if (!indxs.Contains(ix)){
                                InstanceMods[ix].EditingName = false;
                            }
                        }
                    }
                    if (InstanceMods.Where(x => x.Selected == true).Any()){
                        List<ModInfo> en = InstanceMods.Where(x => x.Selected == true).ToList();
                        en = InstanceMods.Where(x => x.Selected == true).ToList();
                        foreach (ModInfo info in en){
                            int ix = InstanceMods.IndexOf(info);
                            if (!indxs.Contains(ix)){
                                InstanceMods[ix].Selected = false;
                            }                            
                        }
                    }
                    foreach (int idx in indxs){
                        InstanceMods[idx].Selected = true;
                    }                    
                }
            }            
            /*datagrid selectionversion*/
            
        }

        private void EnabledModsCheck(){
            foreach (ModInfo mod in EnabledMods){
                ModInfo m = InstanceMods.Where(x => x.Name == mod.Name).First();
                int idx = InstanceMods.IndexOf(m);
                InstanceMods[idx].Enabled = true;
            }

            foreach (ModInfo mod in InstanceMods){
                if (mod.Enabled == false){
                    if (EnabledMods.Contains(mod)){
                        EnabledMods.Remove(mod);
                    }
                }
            }
        }

        void OnModListChange(object sender, PropertyChangedEventArgs e){
            if (e.PropertyName == "Enabled"){
                EnabledModsChanged();
            } else if (e.PropertyName == "IsSelected"){
                RaisePropertyChanged("InstanceMods");
            } else if (e.PropertyName == "Root"){
                RootChanged();
                RaisePropertyChanged("InstanceMods");
            } else if (e.PropertyName == "Name"){
                ModInfo mod = sender as ModInfo;
                RenameMod(mod);
            } else if (e.PropertyName == "Category"){
                RaisePropertyChanged("InstanceMods");
            } else {
                RaisePropertyChanged("InstanceMods");
            }
        }

        public void RenameMod(ModInfo mod){
            string dir = "";
            string newname = "";
            string ininame = "";
            if (IsDirectory(mod.Location)){
                DirectoryInfo modi = new DirectoryInfo(mod.Location);
                dir = modi.Parent.FullName;
                newname = Path.Combine(dir, mod.Name);
                ininame = Path.Combine(dir, string.Format("{0}.ini", mod.Name));
                Directory.Move(mod.Location, newname);
            } else {
                FileInfo modi = new FileInfo(mod.Location);
                dir = modi.DirectoryName;
                string nameext = string.Format("{0}{1}", mod.Name, modi.Extension);
                newname = Path.Combine(dir, nameext);
                ininame = Path.Combine(dir, string.Format("{0}.ini", mod.Name));
                File.Move(mod.Location, newname);
            }            
            File.Move(FileToIni(mod.Location), ininame);
            var modv = InstanceMods.Where(x => x.Location == mod.Location).First();
            int idx = InstanceMods.IndexOf(modv);
            InstanceMods[idx].Location = newname;
            ModInfoToFile(InstanceMods[idx], FileToIni(InstanceMods[idx].Location));
            RaisePropertyChanged("InstanceMods");
        }

        void EnabledModsChanged(){
            if (initiallyloaded){
                var list = InstanceMods.Where(x => x.Enabled);
                EnabledMods.Clear(); 
                foreach (ModInfo mod in list){
                    AddToEnabledModsCollection(mod);
                }
                ThisProfile.EnabledMods = EnabledMods;
                SaveProfileData();    
            }
        }

        void RootChanged(){
            RaisePropertyChanged("InstanceMods");
        }
        
        void ThingsChangedUpdate(){
            if (EnabledMods != null){
                if (EnabledMods.Count != 0){
                    foreach (ModInfo mod in EnabledMods){
                        int idxIM = InstanceMods.IndexOf(mod);    
                        int idxEM = EnabledMods.IndexOf(mod);
                        EnabledMods[idxEM] = InstanceMods[idxIM];
                    }
                }
            }
            
            if (InstanceMods != null){
                if (InstanceMods.Count != 0){
                    foreach (ModInfo mod in InstanceMods){
                        ModInfoToFile(mod, FileToIni(mod.Location));
                    }
                }
            }
        }



        #endregion


        #region Instance Handling

        private void LoadInstance(string instance){
            GetInstanceInfo(Path.Combine(instance, "instance.ini"));
            Console.WriteLine("Loading instance.");         
            ReadProfilesFile(Path.Combine(instance, @"data\profiles.ini"));
            CurrentProfile = Profiles.Where(x => x.Name == ThisInstance.ActiveProfile).First();
            RefreshModlist();
            LoadProfileData();
            EnabledModsCheck();  
            RefreshAllCollections();          
        }

        private void CreateFileTree(string instance){ 
            GetInstanceInfo(Path.Combine(instance, "instance.ini"));                       
            ThisInstance.InstanceModFolder = Path.Combine(instance, "mods");
            if (!Directory.Exists(ThisInstance.InstanceModFolder)){
                Directory.CreateDirectory(ThisInstance.InstanceModFolder);
            }
            ThisInstance.InstanceDataFolder = Path.Combine(instance, "data"); 
            if (!Directory.Exists(ThisInstance.InstanceDataFolder)){
                Directory.CreateDirectory(ThisInstance.InstanceDataFolder);
            }
            ThisInstance.InstanceDownloadsFolder = Path.Combine(instance, "downloads"); 
            if (!Directory.Exists(ThisInstance.InstanceDownloadsFolder)){
                Directory.CreateDirectory(ThisInstance.InstanceDownloadsFolder);
            }
            ThisInstance.InstanceProfilesFolder = Path.Combine(instance, "profiles"); 
            if (!Directory.Exists(ThisInstance.InstanceProfilesFolder)){
                Directory.CreateDirectory(ThisInstance.InstanceProfilesFolder);
            }
            File.WriteAllText(Path.Combine(ThisInstance.Location, @"data\modlist.ini"), "");     
            File.WriteAllText(Path.Combine(ThisInstance.Location, @"data\profiles.ini"), "Default");  
            File.WriteAllText(Path.Combine(ThisInstance.Location, @"data\executables.ini"), "");  
            File.WriteAllText(Path.Combine(ThisInstance.Location, @"data\settings.ini"), "");
            string wfile = Path.Combine(ThisInstance.Location, @"data\profile_default.ini");
            Categories.Add(new CategoryType() { Name="Default", ColorHex = "#EBEDEF", ColorHexAlt = "#C9D1D9"});
            using (FileStream fs = File.Create(wfile)){
                using (StreamWriter sw = new(fs)){
                    sw.WriteLine("ProfileName=Default");
                    sw.Flush();
                    sw.Close();
                }
                fs.Close(); fs.Dispose();
            }    
            ThisInstance.ActiveProfile = "Default";
            ThisProfile.Name = "Default";
            SaveInstanceInfo();
            SaveProfileData();
        }

        private void ReadProfilesFile(string profileini){
            using (FileStream fs = new FileStream(profileini, FileMode.Open, FileAccess.Read)){
                using (StreamReader ss = new StreamReader(fs)){
                    bool eos = false;                
                        while (eos == false){
                            if(!ss.EndOfStream){
                                string line = ss.ReadLine();
                                Profiles.Add(LoadProfileData(line));
                            } else {
                                eos = true;
                            }
                        }
                    ss.Close();
                }
                fs.Close(); fs.Dispose();
            }            
        }



        private void GetInstanceInfo(string instanceini){            
            using (FileStream fs = new FileStream(instanceini, FileMode.Open, FileAccess.Read)){
                using (StreamReader sr = new StreamReader(fs)) {
                    bool eos = false;
                    bool exes = false;  
                    bool cats = false;              
                        while (eos == false){
                            if(!sr.EndOfStream){
                                string line = sr.ReadLine();
                                if (line.Contains("=")){
                                    var split = line.Split("=");
                                    if(split[0] == "InstanceName" && !string.IsNullOrWhiteSpace(split[1])) ThisInstance.Name = split[1];
                                    if(split[0] == "Game" && !string.IsNullOrWhiteSpace(split[1])) ThisInstance.Game = split[1];
                                    if(split[0] == "InstallLocation" && !string.IsNullOrWhiteSpace(split[1])) ThisInstance.InstallLocation = split[1];
                                    if(split[0] == "InstanceFolder" && !string.IsNullOrWhiteSpace(split[1])) ThisInstance.Location = split[1];
                                    if(split[0] == "GameModsFolder" && !string.IsNullOrWhiteSpace(split[1])) ThisInstance.GameModFolder = split[1];
                                    if(split[0] == "ModsFolder" && !string.IsNullOrWhiteSpace(split[1])) ThisInstance.InstanceModFolder = split[1];
                                    if(split[0] == "DataFolder" && !string.IsNullOrWhiteSpace(split[1])) ThisInstance.InstanceDataFolder = split[1];
                                    if(split[0] == "DownloadsFolder" && !string.IsNullOrWhiteSpace(split[1])) ThisInstance.InstanceDownloadsFolder = split[1];
                                    if(split[0] == "ActiveExe" && !string.IsNullOrWhiteSpace(split[1])){
                                        ThisInstance.ActiveExe = split[1];
                                        this.ActiveExe = split[1];
                                    }
                                    if(split[0] == "ActiveProfile" && !string.IsNullOrWhiteSpace(split[1])) ThisInstance.ActiveProfile = split[1];
                                    if(split[0] == "ActiveExe" && !string.IsNullOrWhiteSpace(split[1])) ThisInstance.ActiveExe = split[1];
                                    if(split[0] == "GameExe" && !string.IsNullOrWhiteSpace(split[1])) ThisInstance.GameEXE = split[1];
                                    if(split[0] == "Categories" && !string.IsNullOrWhiteSpace(split[1])) StringToCategories(split[1]);
                                } else if (line.Contains("[EXECUTABLES]")){ 
                                    cats = false;
                                    exes = true;
                                } else if (line.Contains("[CATEGORIES]")){ 
                                    cats = true;
                                    exes = false;
                                } else if (exes){                                    
                                    Executable exe = new();
                                    List<string> split = line.Split(";").ToList();
                                    exe.Name = split[0];
                                    exe.Destination = split[1];
                                    if (split[2] != null) exe.Arguments = split[2];
                                    Executables.Add(exe);
                                } else if (cats){
                                    CategoryType cat = new();
                                    List<string> split = line.Split(";").ToList();
                                    cat.Name = split[0];
                                    cat.ColorHex = split[1];
                                    cat.ColorHexAlt = split[2];
                                    if (split[3] != null) cat.Description = split[3];
                                    Categories.Add(cat);
                                }
                            } else {
                                eos = true;
                            }
                        }
                    sr.Close();
                }
                fs.Close(); fs.Dispose();
            }
            profilesfile = Path.Combine(ThisInstance.Location, @"data\profiles.ini"); 
            settingsfile = Path.Combine(ThisInstance.Location, @"data\settings.ini");
            modlistfile = Path.Combine(ThisInstance.Location, @"data\modlist.ini");  
            executablesfile = Path.Combine(ThisInstance.Location, @"data\executables.ini"); 
            instancefile = Path.Combine(ThisInstance.Location, @"instance.ini");             

            if (Executables.Count == 0){
                string exe = "";
                if (ThisInstance.Game == "Sims4"){
                    exe = Path.Combine(ThisInstance.InstallLocation, string.Format(@"Game\Bin\{0}", ThisInstance.GameEXE));
                } else if (ThisInstance.Game == "Sims2"){                    
                    exe = Path.Combine(ThisInstance.InstallLocation, @"The Sims 2 Mansion and Garden Stuff\TSBin\Sims2EP9RPC.exe");
                    if (!File.Exists(exe)){
                        exe = Path.Combine(ThisInstance.InstallLocation, @"The Sims 2 Mansion and Garden Stuff\TSBin\Sims2RPC.exe");
                        if (!File.Exists(exe)){
                            exe = Path.Combine(ThisInstance.InstallLocation, string.Format(@"The Sims 2 Mansion and Garden Stuff\TSBin\{0}", ThisInstance.GameEXE));
                        }
                    }
                } else if (ThisInstance.Game == "Sims3"){
                    exe = Path.Combine(ThisInstance.InstallLocation, string.Format(@"Game\Bin\{0}", ThisInstance.GameEXE));
                }
                Executables.Add(new Executable(){Name = ThisInstance.GameEXE, Destination = exe});
                ThisInstance.ActiveExe = ThisInstance.GameEXE;
            }

            if (ThisInstance.Game == "Sims2"){
                GameIcon = "/img/s2.png";
                ThisInstance.GameIcon = "/img/s2.png";
            } else if (ThisInstance.Game == "Sims3"){
                GameIcon = "/img/s3.png";
                ThisInstance.GameIcon = "/img/s3.png";
            } else if (ThisInstance.Game == "Sims4"){
                GameIcon = "/img/s4.png";
                ThisInstance.GameIcon = "/img/s4.png";
            }
        }

        private void SaveInstanceInfo(){
            string location = Path.Combine(ThisInstance.Location, "instance.ini");
            using (FileStream fs = File.Create(location)){
                using (StreamWriter sw = new(fs)){
                    sw.WriteLineAsync(string.Format("InstanceName={0}", ThisInstance.Name));
                    sw.WriteLineAsync(string.Format("Game={0}", ThisInstance.Game));
                    sw.WriteLineAsync(string.Format("InstallLocation={0}", ThisInstance.InstallLocation));
                    sw.WriteLineAsync(string.Format("InstallLocation={0}", ThisInstance.Version));
                    sw.WriteLineAsync(string.Format("GameExe={0}", ThisInstance.GameEXE));
                    sw.WriteLineAsync(string.Format("GameModsFolder={0}", ThisInstance.GameModFolder));
                    sw.WriteLineAsync(string.Format("InstanceFolder={0}", ThisInstance.Location));
                    sw.WriteLineAsync(string.Format("DataFolder={0}", ThisInstance.InstanceDataFolder));
                    sw.WriteLineAsync(string.Format("ModsFolder={0}", ThisInstance.InstanceModFolder));
                    sw.WriteLineAsync(string.Format("DownloadsFolder={0}", ThisInstance.InstanceDownloadsFolder));
                    sw.WriteLineAsync(string.Format("ActiveProfile={0}", ThisInstance.ActiveProfile));
                    sw.WriteLineAsync(string.Format("ActiveExe={0}", ThisInstance.ActiveExe));
                    sw.WriteLineAsync(string.Format("[CATEGORIES]"));
                    foreach (CategoryType cat in Categories){
                        sw.WriteLineAsync(string.Format("{0};{2};{3};{1}", cat.Name, cat.Description, cat.ColorHex, cat.ColorHexAlt));
                    }
                    sw.WriteLineAsync(string.Format("[EXECUTABLES]"));
                    foreach (Executable exe in Executables){
                        sw.WriteLineAsync(string.Format("{0};{1};{2}", exe.Name, exe.Destination, exe.Arguments));
                    }
                    sw.Flush();
                    sw.Close();
                }
                fs.Close(); fs.Dispose();
            }
        }

        private void GetGameVersion(){
            if (ThisInstance.Game == "Sims4"){
                string versiontxt = Path.Combine(ThisInstance.GameModFolder, "GameVersion.txt");
                using (FileStream fs = new FileStream(versiontxt, FileMode.Open, FileAccess.Read)){
                    using (StreamReader ss = new StreamReader(fs)){
                        bool eos = false;                
                            while (eos == false){
                                if(!ss.EndOfStream){
                                    string line = ss.ReadLine();
                                    if (!string.IsNullOrWhiteSpace(line)){
                                        ThisInstance.Version = line;
                                    }
                                } else {
                                    eos = true;
                                }
                            }
                        ss.Close();
                    }
                    fs.Close(); fs.Dispose();
                }
            }
        }

        #endregion


        #region Profile Handling              


        private void SaveProfileData(){
            if(ThisProfile != null && ThisInstance.InstanceDataFolder != null){
                if (ThisProfile.Name == null){
                    ThisProfile.Name = ThisInstance.ActiveProfile;
                }
                string file = string.Format("profile_{0}.ini", ThisProfile.Name);
                string fileloc = Path.Combine(ThisInstance.InstanceDataFolder, file);

                using (FileStream fs = File.Create(fileloc)){
                    using (StreamWriter sw = new StreamWriter(fs)){
                        sw.WriteLine(string.Format("[GENERAL]"));
                        sw.WriteLine(string.Format("Name={0}", ThisProfile.Name));
                        sw.WriteLine(string.Format("LocalSaves={0}", BoolToString(ThisProfile.LocalSaves)));
                        sw.WriteLine(string.Format("LocalSettings={0}", BoolToString(ThisProfile.LocalSettings)));
                        sw.WriteLine(string.Format("LocalScreenshots={0}", BoolToString(ThisProfile.LocalScreenshots)));
                        sw.WriteLine(string.Format("LocalTray={0}", BoolToString(ThisProfile.LocalTray)));
                        sw.WriteLine(string.Format("[ENABLED MODS]"));
                        foreach (ModInfo mod in ThisProfile.EnabledMods){
                            sw.WriteLine(string.Format("{0};{1}", mod.Name, mod.DateEnabled.ToString()));
                        }
                    }
                }
            }
            
        }  
        private void SaveProfileData(Profile profile){
            string file = string.Format("profile_{0}.ini", profile.Name);
            string fileloc = Path.Combine(ThisInstance.InstanceDataFolder, file);

            using (FileStream fs = File.Create(fileloc)){
                using (StreamWriter sw = new StreamWriter(fs)){
                    sw.WriteLine(string.Format("[GENERAL]"));
                    sw.WriteLine(string.Format("Name={0}", profile.Name));
                    sw.WriteLine(string.Format("LocalSaves={0}", BoolToString(profile.LocalSaves)));
                    sw.WriteLine(string.Format("LocalSettings={0}", BoolToString(profile.LocalSettings)));
                    sw.WriteLine(string.Format("LocalScreenshots={0}", BoolToString(profile.LocalScreenshots)));
                    sw.WriteLine(string.Format("LocalTray={0}", BoolToString(profile.LocalTray)));
                    sw.WriteLine(string.Format("[ENABLED MODS]"));
                    foreach (ModInfo mod in ThisProfile.EnabledMods){
                        sw.WriteLine(string.Format("{0};{1}", mod.Name, mod.DateEnabled.ToString()));
                    }
                }
            }            
        }       

        private void LoadProfileData(){
            string fileloc = Path.Combine(ThisInstance.InstanceDataFolder, string.Format("profile_{0}.ini", ThisInstance.ActiveProfile));
            enabledmodslist.Clear();
            using (FileStream fs = new FileStream(fileloc, FileMode.Open, FileAccess.Read)){
                using (StreamReader ss = new StreamReader(fs)){
                    bool eos = false;
                    bool mods = false;
                    while (eos == false){
                        if(!ss.EndOfStream){
                            string line = ss.ReadLine();
                            if (line.Contains("=")){
                                string[] split = line.Split("=");
                                if(split[0] == "Name") ThisProfile.Name = split[1];
                                if(split[0] == "LocalSaves") ThisProfile.LocalSaves = StringToBool(split[1]);
                                if(split[0] == "LocalSettings") ThisProfile.LocalSettings = StringToBool(split[1]);
                                if(split[0] == "LocalScreenshots") ThisProfile.LocalScreenshots = StringToBool(split[1]);
                                if(split[0] == "LocalTray") ThisProfile.LocalTray = StringToBool(split[1]);
                            } else if (line.Contains("[ENABLED MODS]")){
                                mods = true;
                            } else if (mods) {
                                string[] split = line.Split(";");
                                if (InstanceMods.Where(x => x.Name == split[0]).Any()){
                                    ModInfo md = InstanceMods.Where(x => x.Name == split[0]).First();
                                    int idx = InstanceMods.IndexOf(md);
                                    InstanceMods[idx].DateEnabled = DateTime.Parse(split[1]);
                                    if (!EnabledMods.Contains(md)){
                                        
                                        EnabledMods.Add(InstanceMods[idx]);
                                    }               
                                    if (!ThisProfile.EnabledMods.Contains(md)){
                                        ThisProfile.EnabledMods.Add(InstanceMods[idx]);
                                    }                        
                                }
                            }
                        } else {
                            eos = true;
                        }
                    }
                    ss.Close();
                }
                fs.Close(); fs.Dispose();
            }
            SaveProfileData();
        }
        private Profile LoadProfileData(string profile){
            Profile prof = new();
            string fileloc = Path.Combine(ThisInstance.InstanceDataFolder, string.Format("profile_{0}.ini", profile));
            using (FileStream fs = new FileStream(fileloc, FileMode.Open, FileAccess.Read)){
                using (StreamReader ss = new StreamReader(fs)){
                    bool eos = false;
                    bool mods = false;
                    while (eos == false){
                        if(!ss.EndOfStream){
                            string line = ss.ReadLine();
                            if (line.Contains("=")){
                                string[] split = line.Split("=");
                                if(split[0] == "Name") prof.Name = split[1];
                                if(split[0] == "LocalSaves") prof.LocalSaves = StringToBool(split[1]);
                                if(split[0] == "LocalSettings") prof.LocalSettings = StringToBool(split[1]);
                                if(split[0] == "LocalScreenshots") prof.LocalScreenshots = StringToBool(split[1]);
                                if(split[0] == "LocalTray") prof.LocalTray = StringToBool(split[1]);
                            } else if (line.Contains("[ENABLED MODS]")){
                                mods = true;
                            } else if (mods) {
                                
                            }
                        } else {
                            eos = true;
                        }
                    }
                    ss.Close();
                }
                fs.Close(); fs.Dispose();
            }
            return prof;
        }

        #endregion



        #region Folder Handlers

        private void RefreshListClick(){
            Task t = Task.Run(() => {
                ProgBarLabel = "Refreshing Modlist";
                PopupGreyed = Visibility.Visible;
                ProgressBarGridVis = Visibility.Visible;
            });
            t.Wait();
            new Thread(() => RefreshModlist()){IsBackground = true}.Start();
        }

        private void RefreshModlist(){
            ReadModFolder();   
            ReadDownloadsFolder();         
            //ModsFromModList();
            LoadProfileData();    
            RefreshAllCollections();
            PopupGreyed = Visibility.Hidden;
            ProgressBarGridVis = Visibility.Hidden;
        }

        private void ReadModFolder(){
            //get the directoryinfo
            DirectoryInfo mf = new(ThisInstance.InstanceModFolder);
            if (mf.GetDirectories().Length != 0){ //there are subfolders in our mods folder
                //get a list of the subfolders
                List<string> Directories = Directory.GetDirectories(mf.FullName).ToList();
                foreach (string dir in Directories){
                    ProcessDirectory(dir);
                }
            }
            if (mf.GetFiles().Length != 0){
                ProcessFiles(ThisInstance.InstanceModFolder);
            }
            if (Groups.Count != 0){
                foreach (ModInfo group in Groups){
                    DirectoryInfo g = new(group.Location);
                    if (g.GetDirectories().Length != 0){
                        List<DirectoryInfo> dirs = g.GetDirectories().ToList();
                        if (dirs.Where(x => x.Name.Contains("___Group")).Any()){
                            foreach (DirectoryInfo dir in dirs.Where(x => x.Name.Contains("___Group")).ToList()){
                                MakeGroupHolder(dir);
                            }
                        }
                    }
                }
            }
            if (Groups.Count != 0){
                foreach (ModInfo group in Groups){
                    ReadGroupsFolder(group);
                }
            }
        }

        private void ReadGroupsFolder(ModInfo group){
            DirectoryInfo g = new(group.Location);
            if (g.GetFiles().Length != 0){
                ProcessFiles(g.FullName);
            }
            if (g.GetDirectories().Length != 0){
                List<string> Directories = Directory.GetDirectories(g.FullName).ToList();
                foreach (string dir in Directories){
                    ProcessDirectory(dir);
                }
            }
        }

        private void ReadDownloadsFolder(){
            DirectoryInfo mf = new(ThisInstance.InstanceDownloadsFolder);            
            if (mf.GetFiles().Length != 0){
                List<string> subfiles = Directory.GetFiles(ThisInstance.InstanceDownloadsFolder, "", SearchOption.AllDirectories).ToList();
                foreach (string file in subfiles){
                    NewDownloadInfo ndi = new();
                    FileInfo fileInfo = new(file);
                    string namenoex = fileInfo.Name.Replace(fileInfo.Extension, "");
                    string ini = string.Format("{0}.ini", fileInfo.Name);
                    string iniloc = Path.Combine(ThisInstance.InstanceDownloadsFolder, ini);
                    if (fileInfo.Extension == ".ts4script" && File.Exists(Path.Combine(fileInfo.DirectoryName, string.Format("{0}.package", namenoex)))){

                    } else if (fileInfo.Extension != ".ini") {
                        if (File.Exists(iniloc)){
                            ndi = DownloadInfoFromFile(iniloc);
                            if (!Downloads.Contains(ndi)){
                               Downloads.Add(ndi); 
                            }                            
                        } else {
                            ndi.Name = namenoex;
                            ndi.Location = fileInfo.FullName;
                            ndi.Extension = fileInfo.Extension.Replace(".", "");
                            if (File.Exists(Path.Combine(fileInfo.DirectoryName, string.Format("{0}.ts4script", namenoex)))){
                                ndi.HasScript = true;
                            } else {
                                ndi.HasScript = false;
                            }
                            ndi.DateAdded = DateTime.Now;
                            ndi.Installed = false;
                            if (!Downloads.Contains(ndi)){
                               Downloads.Add(ndi); 
                            }
                            DownloadInfoToFile(iniloc, ndi);
                        }
                    }
                    
                }
            }            
        }

        private NewDownloadInfo DownloadInfoFromFile(string ini){
            NewDownloadInfo ndi = new();
            using (FileStream fs = new FileStream(ini, FileMode.Open, FileAccess.Read)){
                using (StreamReader sr = new StreamReader(fs)) {
                    bool eos = false;              
                        while (eos == false){
                            if(!sr.EndOfStream){
                                string line = sr.ReadLine();
                                if (line.Contains("=")){
                                    var split = line.Split("=");
                                    if(split[0] == "Name" && !string.IsNullOrWhiteSpace(split[1])) ndi.Name = split[1];
                                    if(split[0] == "Source" && !string.IsNullOrWhiteSpace(split[1])) ndi.Source = split[1];
                                    if(split[0] == "Location" && !string.IsNullOrWhiteSpace(split[1])) ndi.Location = split[1];
                                    if(split[0] == "Extension" && !string.IsNullOrWhiteSpace(split[1])) ndi.Extension = split[1];
                                    if(split[0] == "HasScript" && !string.IsNullOrWhiteSpace(split[1])) ndi.HasScript = StringToBool(split[1]);
                                    if(split[0] == "Installed" && !string.IsNullOrWhiteSpace(split[1])) ndi.Installed = StringToBool(split[1]);
                                    if(split[0] == "DateAdded" && !string.IsNullOrWhiteSpace(split[1])) ndi.DateAdded = DateTime.Parse(split[1]);
                                    
                                }
                            } else {
                                eos = true;
                            }
                        }
                    sr.Close();
                }
                fs.Close(); fs.Dispose();
            }
            return ndi;
        }
        private void DownloadInfoToFile(string ini, NewDownloadInfo newDownload){
            using (FileStream fs = File.Create(ini)){
                using (StreamWriter sw = new(fs)){
                    sw.WriteLine(string.Format("Name={0}", newDownload.Name));
                    sw.WriteLine(string.Format("Location={0}", newDownload.Location));
                    sw.WriteLine(string.Format("Extension={0}", newDownload.Extension));
                    sw.WriteLine(string.Format("Source={0}", newDownload.Source));
                    sw.WriteLine(string.Format("HasScript={0}", BoolToString(newDownload.HasScript)));
                    sw.WriteLine(string.Format("Installed={0}", BoolToString(newDownload.Installed)));
                    sw.WriteLine(string.Format("DateAdded={0}", newDownload.DateAdded.ToString()));
                    sw.Flush();
                    sw.Close();                
                }
                fs.Close(); fs.Dispose();
            }
        }

        private void ProcessFolder(string directory){
            //get the directoryinfo
            DirectoryInfo mf = new(directory);
            if (mf.GetDirectories().Length != 0){ //there are subfolders in our mods folder
                //get a list of the subfolders
                List<string> Directories = Directory.GetDirectories(mf.FullName).ToList();
                foreach (string dir in Directories){
                    ProcessDirectory(dir);
                }
            }
            if (mf.GetFiles().Length != 0){
                ProcessFiles(directory);
            }
        }

        private void ProcessDirectory(string directory){            
            DirectoryInfo dir = new(directory);
            if (!dir.Name.EndsWith("___Group")){ //_Group is a sorted folder, this is a single mod
                ModInfo modInfo = new();
                modInfo.Name = dir.Name;
                modInfo.Location = dir.FullName;
                string ini = FileToIni(dir.Name);
                string iniext = Path.Combine(dir.Parent.FullName, ini);
                if (File.Exists(iniext)){
                    modInfo = ModInfoFromFile(iniext);
                    modInfo.PropertyChanged += OnModListChange;
                    AddToInstanceModsCollection(modInfo);
                    AddModToList(modInfo);
                } else {
                    modInfo.DateAdded = DateTime.Now;
                    modInfo.OutOfDate = false;
                    modInfo.DateUpdated = DateTime.Now;
                    modInfo.Processed = true;
                    modInfo.Combined = true;
                    modInfo.Category = Categories.Where(x => x.Name == "Default").First();
                    modInfo.New = true;
                    List<string> subfiles = Directory.GetFiles(dir.FullName, "", SearchOption.AllDirectories).ToList();
                    foreach (string subfile in subfiles){
                        string reg = subfile.Replace(directory, "");
                        modInfo.Files.Add(reg);
                    }
                    InitialCheck ic = ip.CheckThrough(subfiles[0]);
                    if (ic.Unidentifiable == true){
                        modInfo.Broken = true;
                    }
                    if (ic.Game == "Sims2"){
                        modInfo.Game = "The Sims 2";
                    } else if (ic.Game == "Sims3"){
                        modInfo.Game = "The Sims 3";
                    } else if (ic.Game == "Sims4"){
                        modInfo.Game = "The Sims 4";
                    } else if (ic.Game == "SC5"){
                        modInfo.Game = "SimCity 5";
                    } else if (ic.Unidentifiable == true){
                        modInfo.Game = "Unknown";
                    } else {
                        modInfo.Game = "Other";
                    }
                    if (subfiles.Contains("ts4script")){
                        modInfo.HasScript = true;
                    }
                    modInfo.PropertyChanged += OnModListChange;
                    AddToInstanceModsCollection(modInfo);
                    AddModToList(modInfo);
                    ModInfoToFile(modInfo, iniext);
                }
            } else if (dir.Name.EndsWith("___Group")) { //This is a sorted folder! So we have to treat this like our mod folder
                MakeGroupHolder(dir);
                //ProcessFolder(dir.FullName);
            }
        }

        private void MakeGroupHolder(DirectoryInfo dir){
            ModInfo modInfo = new();
            modInfo.Location = dir.FullName;
            modInfo.IsGroup = true;            
            string groupName = dir.Name.Replace("___Group", "");
            modInfo.Name = groupName;
            InstanceMods.Add(modInfo);
            Groups.Add(modInfo);
        }

        private void ProcessFiles(string directory){
            List<string> files = Directory.GetFiles(directory).ToList();
            if (files.Count != 0){
                List<FileInfo> fiFiles = new();
                foreach (string file in files){
                    FileInfo fileinfo = new FileInfo(file);
                    fiFiles.Add(fileinfo);
                }
                foreach (FileInfo fileinfo in fiFiles){
                    if (extensions.Contains(fileinfo.Extension)){
                        ModInfo modInfo = new();
                        string filedir = fileinfo.DirectoryName;
                        string filename = fileinfo.Name;
                        string basename = filename.Replace(fileinfo.Extension, "");
                        string ini = FileToIni(filename);
                        string iniext = Path.Combine(directory, ini);
                        string scriptf = string.Format("{0}.ts4script", basename);
                        string packf = string.Format("{0}.package", basename);
                        modInfo.Name = basename;
                        if (File.Exists(Path.Combine(directory, ini))){
                            modInfo = ModInfoFromFile(iniext);                            
                            modInfo.PropertyChanged += OnModListChange;
                            AddToInstanceModsCollection(modInfo);
                            AddModToList(modInfo);
                        } else {
                            modInfo = ProcessModFile(modInfo, fileinfo, fiFiles);
                            ModInfoToFile(modInfo, iniext);
                            AddToInstanceModsCollection(modInfo);
                            AddModToList(modInfo);
                        }
                    } else if (extensions.Contains(fileinfo.Extension)) {
                        if (AskedAboutMove == false){
                            FoundZipsGridVis = Visibility.Visible;
                            PopupGreyed = Visibility.Visible;
                        } else if (AutoMoveCompressed == true){
                            File.Move(fileinfo.FullName, Path.Combine(ThisInstance.InstanceDownloadsFolder, fileinfo.Name));
                        }
                    }
                }
            }
        }

        private ModInfo ProcessModFile(ModInfo modInfo, FileInfo fileinfo, List<FileInfo> fiFiles){
            string filedir = fileinfo.DirectoryName;
            string filename = fileinfo.Name;
            string basename = filename.Replace(fileinfo.Extension, "");
            string ini = FileToIni(filename);
            string iniext = Path.Combine(fileinfo.DirectoryName, ini);
            string scriptf = string.Format("{0}.ts4script", basename);
            string packf = string.Format("{0}.package", basename);
            modInfo.Location = fileinfo.FullName;
            modInfo.Name = basename;
            modInfo.DateAdded = DateTime.Now;
            modInfo.OutOfDate = false;
            modInfo.DateUpdated = DateTime.Now;
            modInfo.Category = Categories.Where(x => x.Name == "Default").First();
            modInfo.New = true;
            modInfo.Size = fileinfo.Length;
            if (fileinfo.DirectoryName.Contains("___Group")){
                DirectoryInfo dir = new(fileinfo.DirectoryName);
                modInfo.Group = dir.Name;
            }
            if (fileinfo.Extension == ".package"){
                modInfo.Type = GFileType.Package;
            } else if (fileinfo.Extension == ".sims2pack"){
                modInfo.Type = GFileType.Sims2Pack;
                modInfo.Game = "The Sims 2";
            } else if (fileinfo.Extension == ".sims3pack"){
                modInfo.Type = GFileType.Sims3Pack;
                modInfo.Game = "The Sims 3";
            } else if (fileinfo.Extension == ".ts4script"){
                modInfo.Type = GFileType.TS4Script;
            } else if (fileinfo.Extension == ".zip" || fileinfo.Extension == ".rar" || fileinfo.Extension == ".7z" || fileinfo.Extension == ".pkg"){
                modInfo.Type = GFileType.Compressed;
            } else if (fileinfo.Extension == ".trayitem" || fileinfo.Extension == ".bpi" || fileinfo.Extension == ".hhi" || fileinfo.Extension == ".sgi" || fileinfo.Extension == ".householdbinary"){
                modInfo.Type = GFileType.Tray;
            }
            if (modInfo.Type == GFileType.Package){
                InitialCheck ic = ip.CheckThrough(fileinfo.FullName);
                if (ic.Unidentifiable == true){
                    modInfo.Broken = true;
                }
                if (ic.Game == "Sims2"){
                    modInfo.Game = "The Sims 2";
                } else if (ic.Game == "Sims3"){
                    modInfo.Game = "The Sims 3";
                } else if (ic.Game == "Sims4"){
                    modInfo.Game = "The Sims 4";
                } else if (ic.Game == "SC5"){
                    modInfo.Game = "SimCity 5";
                } else if (ic.Unidentifiable == true){
                    modInfo.Game = "Unknown";
                } else {
                    modInfo.Game = "Other";
                }
                if (File.Exists(Path.Combine(filedir, scriptf))){
                    modInfo.HasScript = true;
                }

            } else if (modInfo.Type != GFileType.TS4Script){
                modInfo.Size = fileinfo.Length;
                modInfo.Processed = true; 
            } else if (modInfo.Type == GFileType.TS4Script){
                if (!File.Exists(Path.Combine(filedir, packf))){
                    modInfo.LooseScript = true;
                } else {
                    modInfo.Combined = true;
                    modInfo.Files.Add(Path.Combine(filedir, packf));
                    modInfo.Files.Add(Path.Combine(filedir, scriptf));
                }
            } else if (modInfo.Type == GFileType.Tray){
                List<string> comb = new();
                string[] identi = basename.Split("!");
                string ident = identi[0];
                var result = fiFiles.Where(x => x.Name.Contains(ident)).ToList();
                comb.AddRange(result.Select(x => x.FullName));
                modInfo.Files = comb;
            } else if (modInfo.Type == GFileType.Compressed){
                if (fileinfo.Extension == ".zip"){
                    modInfo.CompressionType = CompressedType.Zip;
                } else if (fileinfo.Extension == ".rar"){
                    modInfo.CompressionType = CompressedType.Rar;
                } else if (fileinfo.Extension == ".7z"){
                    modInfo.CompressionType = CompressedType.SevenZip;
                } else if (fileinfo.Extension == ".pkg"){
                    modInfo.CompressionType = CompressedType.Pkg;
                } else {
                    modInfo.CompressionType = CompressedType.Unknown;
                }
            }

            return modInfo;
        }


        private void SaveCreators(){
            string creatorinfo = Path.Combine(ThisInstance.InstanceDataFolder, "creators.txt");
            StringBuilder stringBuilder = new();
            foreach (CreatorInfo creator in Creators){
                string info = string.Format("{0}: {1}, {2}", creator.CreatorName, creator.CreatorURL, creator.Fave.ToString());
                stringBuilder.AppendLine(info);
            }
            using (FileStream fs = File.Create(creatorinfo)){
                using (StreamWriter sw = new(fs)){
                    sw.Write(stringBuilder);
                    sw.Flush();
                    sw.Close();
                }
                fs.Close(); fs.Dispose();
            }
        }

        private void LoadCreators(){
            string creatorinfo = Path.Combine(ThisInstance.InstanceDataFolder, "creators.txt");
            Creators.Clear();
            using (FileStream fs = new FileStream(creatorinfo, FileMode.Open, FileAccess.Read)){
                using (StreamReader ss = new StreamReader(fs)){
                    bool eos = false;                
                        while (eos == false){
                            if(!ss.EndOfStream){
                                string line = ss.ReadLine();
                                Creators.Add(LoadCreatorLine(line));
                            } else {
                                eos = true;
                            }
                        }
                    ss.Close();
                }
                fs.Close(); fs.Dispose();
            }
        }

        private CreatorInfo LoadCreatorLine(string line){
            CreatorInfo cr = new();
            string[] name = line.Split(": ");
            string[] url = name[1].Split(", ");
            cr.CreatorName = name[0];
            cr.CreatorURL = url[0];
            if (url[1] == "true"){
                cr.Fave = true;
            } else {
                cr.Fave = false;
            }
            return cr;
        }


        #endregion



        #region File Handlers

        private ModInfo ModInfoFromFile(string ini){            
            ModInfo mod = new();
            using (FileStream fs = new FileStream(ini, FileMode.Open, FileAccess.Read)){
                using (StreamReader sr = new(fs)){
                    bool eos = false;                
                        while (eos == false){
                            if(!sr.EndOfStream){
                                string line = sr.ReadLine();
                                if (line.Contains("=")){
                                    string[] split = line.Split("=");
                                    if(split[0] == "Name") mod.Name = split[1];
                                    if(split[0] == "Location") mod.Location = split[1];
                                    if(split[0] == "Creator") mod.Creator = split[1];
                                    if(split[0] == "Game") mod.Game = split[1];
                                    if(split[0] == "Size") mod.Size = Convert.ToInt64(split[1]);
                                    if(split[0] == "Type") mod.Type = TypeFromString(split[1]);
                                    if(split[0] == "CompressionType") mod.CompressionType = CompressedTypeFromString(split[1]);
                                    if(split[0] == "OutOfDate") mod.OutOfDate = StringToBool(split[1]);
                                    if(split[0] == "Conflicts") mod.Conflicts = StringToBool(split[1]);
                                    if(split[0] == "Orphan") mod.Orphan = StringToBool(split[1]);
                                    if(split[0] == "LooseScript") mod.LooseScript = StringToBool(split[1]);
                                    if(split[0] == "Processed") mod.Processed = StringToBool(split[1]);
                                    if(split[0] == "Scanned") mod.Scanned = StringToBool(split[1]);
                                    if(split[0] == "HasScript") mod.HasScript = StringToBool(split[1]);
                                    if(split[0] == "Broken") mod.Broken = StringToBool(split[1]);
                                    if(split[0] == "Combined") mod.Combined = StringToBool(split[1]);
                                    if(split[0] == "Root") mod.Root = StringToBool(split[1]);
                                    if(split[0] == "DateAdded") mod.DateAdded = DateTime.Parse(split[1]);
                                    if(split[0] == "DateUpdated") mod.DateUpdated = DateTime.Parse(split[1]);
                                    if(split[0] == "Files") mod.Files = StringToStringList(split[1]);
                                    if(split[0] == "Notes") mod.Notes = split[1];
                                    if(split[0] == "Source") mod.Source = split[1];
                                    if(split[0] == "Version") mod.Version = split[1];
                                    if(split[0] == "Category") mod.Category = Categories.Where(x => x.Name == split[1]).First();
                                }
                            } else {
                                eos = true;
                            }
                        }
                    sr.Close();
                }
                fs.Close(); fs.Dispose();
            }
            return mod;
        }       

        private void ModInfoToFile(ModInfo mod, string inifile){
            string modloc;  
            if (IsDirectory(mod.Location)){
                modloc = mod.Location;
                DirectoryInfo dinfo = new(modloc);
                modloc = dinfo.Parent.ToString();
            } else {
                FileInfo modf = new(mod.Location); 
                modloc = modf.DirectoryName;
            }    

            using (FileStream fs = File.Create(inifile)){
                using (StreamWriter sw = new(fs)){
                    sw.WriteLine(string.Format("Name={0}", mod.Name));
                    sw.WriteLine(string.Format("Location={0}", mod.Location));
                    sw.WriteLine(string.Format("Creator={0}", mod.Creator));
                    sw.WriteLine(string.Format("Version={0}", mod.Version));
                    sw.WriteLine(string.Format("Source={0}", mod.Source));
                    sw.WriteLine(string.Format("Category={0}", mod.Category.Name));
                    sw.WriteLine(string.Format("OutOfDate={0}", BoolToString(mod.OutOfDate)));
                    sw.WriteLine(string.Format("Conflicts={0}", BoolToString(mod.Conflicts)));
                    sw.WriteLine(string.Format("Orphan={0}", BoolToString(mod.Orphan)));
                    sw.WriteLine(string.Format("LooseScript={0}", BoolToString(mod.LooseScript)));
                    sw.WriteLine(string.Format("DateAdded={0}", mod.DateAdded.ToString()));
                    sw.WriteLine(string.Format("DateUpdated={0}", mod.DateUpdated.ToString()));
                    sw.WriteLine(string.Format("Game={0}", mod.Game));
                    sw.WriteLine(string.Format("Size={0}", ((int)mod.Size).ToString()));
                    sw.WriteLine(string.Format("Processed={0}", BoolToString(mod.Processed)));
                    sw.WriteLine(string.Format("Scanned={0}", BoolToString(mod.Scanned)));
                    sw.WriteLine(string.Format("HasScript={0}", BoolToString(mod.HasScript)));
                    sw.WriteLine(string.Format("Type={0}", StringFromType(mod.Type)));
                    sw.WriteLine(string.Format("Broken={0}", BoolToString(mod.Broken)));
                    sw.WriteLine(string.Format("Combined={0}", BoolToString(mod.Combined)));
                    sw.WriteLine(string.Format("Root={0}", BoolToString(mod.Root)));
                    sw.WriteLine(string.Format("Notes={0}", mod.Notes));
                    if(mod.Files.Count != 0) sw.WriteLine(string.Format("Files={0}", StringListToString(mod.Files)));
                    if(mod.Type == GFileType.Compressed) sw.WriteLine(string.Format("CompressionType={0}", CompressedTypeToString(mod.CompressionType)));
                    sw.WriteLine(string.Format("[SCAN DATA]"));
                    sw.Flush();
                    sw.Close();
                }
                fs.Close(); fs.Dispose();
            }
        }

        private void AddModToList(ModInfo mod){
            var modlist = Path.Combine(ThisInstance.InstanceDataFolder, "modlist.ini");
            List<string> mods = new();
            using (FileStream fs = new FileStream(modlist, FileMode.Open, FileAccess.Read)){
                using (StreamReader sr = new(fs)){
                    bool eos = false;                
                        while (eos == false){
                            if(!sr.EndOfStream){
                                string line = sr.ReadLine();
                                mods.Add(line);
                            } else {
                                eos = true;
                            }
                        }
                    sr.Close();
                }
                fs.Close(); fs.Dispose();
            }
            if (!mods.Contains(mod.Location)){
                mods.Add(mod.Location);
            }
            using (FileStream fs = File.Create(modlist)){
                using (StreamWriter sw = new(fs)){
                    foreach (string md in mods){
                        sw.WriteLine(md);
                    }
                    sw.Flush();
                    sw.Close();
                }
                fs.Close(); fs.Dispose();
            }
        }

        #endregion

        #region Ini Handlers

        /*private void ModsFromModList(){
            var modlist = Path.Combine(ThisInstance.InstanceDataFolder, "modlist.ini");
            List<string> mods = new();
            using (FileStream fs = new FileStream(modlist, FileMode.Open, FileAccess.Read)){
                using (StreamReader sr = new(fs)){
                    bool eos = false;                
                        while (eos == false){
                            if(!sr.EndOfStream){
                                string line = sr.ReadLine();
                                mods.Add(line);
                            } else {
                                eos = true;
                            }
                        }
                    sr.Close();
                }
                fs.Close(); fs.Dispose();
            }
            InstanceMods = new();
            foreach (string md in mods){
                var info = ModInfoFromFile(FileToIni(md));
                info.PropertyChanged += OnModListChange;                
                AddToInstanceModsCollection(info);
            }
        }*/

        private void EnabledModsFromProfile(){
            foreach (string mod in enabledmodslist){
                if(InstanceMods.Where(m => m.Name.Equals(mod)) != null){                    
                    ModInfo modInfo = InstanceMods.Where(m => m.Name.Equals(mod)).First();
                    int idx = InstanceMods.IndexOf(modInfo);
                    modInfo.Enabled = true;
                    AddToEnabledModsCollection(modInfo);
                    InstanceMods[idx] = modInfo;
                }
            }
        }
        

        #endregion


        #region RelayCommands

        public void MakeRelays(){
            EditDetails = new RelayCommand(this.EditMod);
            AddToCategoryClick = new RelayCommand(this.AddToCategory);
        }
        public RelayCommand EditDetails {get; set;}
        public RelayCommand AddToCategoryClick {get; set;}
        /*ScanFile
        MarkOOD
        AddCreator
        AddSource
        RenameFile
        MoveFileToSubfolder
        MoveFileToInstance
        DeleteFile
        AddNote
        AddToCategoryClick*/

        private void EditMod(object param){
            var modin = param as IEnumerable<object>;
            EditingMod = SelectedMods[0] as ModInfo;
            EditScreenVis = Visibility.Visible;
            PopupGreyed = Visibility.Visible;            
        }

        private void AddToCategory(object param){
            CategoryType pickedcat = param as CategoryType;
            List<int> modindxs = new();
            foreach (var m in SelectedMods){
                var mm = m as ModInfo;
                modindxs.Add(InstanceMods.IndexOf(mm));
            } 
            foreach (int x in modindxs){
                InstanceMods[x].Category = pickedcat;
                ModInfoToFile(InstanceMods[x], FileToIni(InstanceMods[x].Location));
            }            
        }
        
        private void MakeModRoot(){
            if (SelectedMods != null){                
                if (SelectedMods.Count == 0){
                    
                } else if (SelectedMods.Count == 1){
                    ModInfo sm = SelectedMods[0] as ModInfo;
                    ModInfo m = InstanceMods.Where(x => x.Name == sm.Name).First();
                    int idx = InstanceMods.IndexOf(m);
                    if (InstanceMods[idx].Root == true){
                        InstanceMods[idx].Root = false;                        
                    } else {
                        InstanceMods[idx].Root = true; 
                    }
                    ModInfoToFile(InstanceMods[idx], FileToIni(InstanceMods[idx].Location));
                } else {
                    List<int> indxs = new();
                    foreach (ModInfo mm in SelectedMods){
                        indxs.Add(InstanceMods.IndexOf(mm));
                    }                    
                    foreach (int idx in indxs){
                        if (InstanceMods[idx].Root == true){
                            InstanceMods[idx].Root = false;  
                        } else {
                            InstanceMods[idx].Root = true; 
                        }
                        ModInfoToFile(InstanceMods[idx], FileToIni(InstanceMods[idx].Location));
                    }
                }
            }
        }
        private void MarkModOOD(){
            if (SelectedMods != null){                
                if (SelectedMods.Count == 0){
                    
                } else if (SelectedMods.Count == 1){
                    ModInfo sm = SelectedMods[0] as ModInfo;
                    ModInfo m = InstanceMods.Where(x => x.Name == sm.Name).First();
                    int idx = InstanceMods.IndexOf(m);
                    if (InstanceMods[idx].OutOfDate == true){
                        InstanceMods[idx].OutOfDate = false;  
                    } else {
                        InstanceMods[idx].OutOfDate = true; 
                    }
                    ModInfoToFile(InstanceMods[idx], FileToIni(InstanceMods[idx].Location));
                } else {
                    List<int> indxs = new();
                    foreach (ModInfo mm in SelectedMods){
                        indxs.Add(InstanceMods.IndexOf(mm));
                    }                    
                    foreach (int idx in indxs){
                        if (InstanceMods[idx].OutOfDate == true){
                            InstanceMods[idx].OutOfDate = false;  
                        } else {
                            InstanceMods[idx].OutOfDate = true; 
                        }
                        ModInfoToFile(InstanceMods[idx], FileToIni(InstanceMods[idx].Location));
                    }
                }
            }
        }

        #endregion
        


        #region ICommands

        public ICommand MakeRoot
        {
            get { return new DelegateCommand(this.MakeModRoot); }
        }
        public ICommand MarkOOD
        {
            get { return new DelegateCommand(this.MarkModOOD); }
        }

        public ICommand NameEditEnterKey
        {
            get { return new DelegateCommand(this.EditingNameEnter); }
        }
        public ICommand NameEditEscapeKey
        {
            get { return new DelegateCommand(this.EditingNameEscape); }
        }

        public ICommand EditingNameClick
        {
            get { return new DelegateCommand(this.EditingNameCommand); }
        }
        public ICommand ViewProfiles
        {
            get { return new DelegateCommand(this.ViewProfilesClick); }
        }

        public ICommand NewProfile
        {
            get { return new DelegateCommand(this.NewProfileClick); }
        }
        public ICommand EditProfile
        {
            get { return new DelegateCommand(this.EditProfileClick); }
        }
        public ICommand DeleteProfile
        {
            get { return new DelegateCommand(this.DeleteProfileClick); }
        }
        public ICommand DuplicateProfile
        {
            get { return new DelegateCommand(this.DuplicateProfileClick); }
        }
        
        public ICommand CloseProfileWindow
        {
            get { return new DelegateCommand(this.CloseProfileWindowClick); }
        }
        
        public ICommand SaveProfile
        {
            get { return new DelegateCommand(this.SaveProfileClick); }
        }
        public ICommand CancelProfile
        {
            get { return new DelegateCommand(this.CancelProfileClick); }
        }
        public ICommand ViewCategories
        {
            get { return new DelegateCommand(this.ViewCategoriesClick); }
        }

        public ICommand NewCategory
        {
            get { return new DelegateCommand(this.NewCategoryClick); }
        }
        public ICommand EditCategory
        {
            get { return new DelegateCommand(this.EditCategoriesClick); }
        }
        public ICommand DeleteCategory
        {
            get { return new DelegateCommand(this.DeleteCategoryClick); }
        }
        
        public ICommand CloseCategoryWindow
        {
            get { return new DelegateCommand(this.CloseCategoryWindowClick); }
        }
        
        public ICommand SaveCategory
        {
            get { return new DelegateCommand(this.SaveCategoryClick); }
        }
        public ICommand CancelCategory
        {
            get { return new DelegateCommand(this.CancelCategoryClick); }
        }
        public ICommand CategoryPickColor
        {
            get { return new DelegateCommand(this.CategoryPickColorClick); }
        }

        public ICommand SaveModEdit
        {
            get { return new DelegateCommand(this.SaveModEditClick); }
        }
        
        public ICommand CancelModEdit
        {
            get { return new DelegateCommand(this.CancelModEditClick); }
        }
        
        public ICommand IMHeaderName
        {
            get { return new DelegateCommand(this.IM_SortByName); }
        }
        public ICommand EMHeaderName
        {
            get { return new DelegateCommand(this.EM_SortByName); }
        }
        public ICommand DLHeaderName
        {
            get { return new DelegateCommand(this.DL_SortByName); }
        }

        public ICommand MoveZipsYes
        {
            get { return new DelegateCommand(this.MoveZipsYesDecision); }
        }
        public ICommand MoveZipsNo
        {
            get { return new DelegateCommand(this.MoveZipsNoDecision); }
        }

        public ICommand RefreshList
        {
            get { return new DelegateCommand(this.RefreshListClick); }
        }
        public ICommand EnabledClick
        {
            get { return new DelegateCommand(this.RefreshModlist); }
        }
        public ICommand SelectedClick
        {
            get { return new DelegateCommand(this.RefreshModlist); }
        }
        public ICommand SortFiles
        {
            get { return new DelegateCommand(this.SortFilesClick); }
        }

        public ICommand AddMods
        {
            get { return new DelegateCommand(this.AddModsToInstance); } 
        }

        public ICommand AddFolder
        {
            get { return new DelegateCommand(this.AddModsFolderToInstance); } 
        }

        public ICommand ViewToggleClick
        {
            get { return new DelegateCommand(this.ViewToggle); }
        }

        #endregion


        bool editing = false;
        CategoryType editingcat = new();
        Profile editingprofile = new();

        #region User Controls

        private void ViewToggle(){
            if (ViewToggleText == "Simple"){
                ViewToggleText = "Detailed";
                DetailedViewVis = Visibility.Visible;
                SimpleViewVis = Visibility.Hidden;
            } else if (ViewToggleText == "Detailed"){
                ViewToggleText = "Simple";                
                SimpleViewVis = Visibility.Visible;
                DetailedViewVis = Visibility.Hidden;
            }
        }

        private void EditingNameCommand(){
            int idx = 0;
            if (SelectedMods != null){
                if (SelectedMods.Count == 0){
                    
                } else if (SelectedMods.Count == 1){
                    ModInfo sm = SelectedMods[0] as ModInfo;
                    ModInfo m = InstanceMods.Where(x => x.Name == sm.Name).First();
                    idx = InstanceMods.IndexOf(m);                    
                } else {
                    ModInfo sm = SelectedMods[0] as ModInfo;
                    ModInfo m = InstanceMods.Where(x => x.Name == sm.Name).First();
                    idx = InstanceMods.IndexOf(m);
                }

                if (InstanceMods[idx].EditingName == false){   
                    NameChange = InstanceMods[idx].Name;
                    InstanceMods[idx].EditingName = true;
                    editingmodnum = idx;                    
                } else {
                    InstanceMods[idx].EditingName = true;
                    editingmodnum = -1;
                }
            }
        }

        private void EditingNameEnter(){
            if (editingmodnum != -1){                
                InstanceMods[editingmodnum].EditingName = false;
                InstanceMods[editingmodnum].Name = NameChange;
            }
        }

        private void EditingNameEscape(){
            ModInfo mi = InstanceMods.Where(x => x.EditingName == true).First();
            int idx = InstanceMods.IndexOf(mi);
            ModInfo sm = SelectedMods[0] as ModInfo;
            int indx = InstanceMods.IndexOf(sm);
            if (idx == indx){
                InstanceMods[idx].EditingName = false;
                editingmodnum = -1;
            } else {
                InstanceMods[idx].EditingName = false;
                InstanceMods[indx].EditingName = false;
                editingmodnum = -1;
            }
        }

        private void ViewProfilesClick(){
            ProfilesCV.Refresh();
            ProfileScreenVis = Visibility.Visible;
            PopupGreyed = Visibility.Visible;
        }

        private void NewProfileClick(){
            NewProfileVis = Visibility.Visible;
            NewProfileLabel = "Create New Profile";
            NewProfileName = "";
            NewProfileLocalSavesCheck = true;
            NewProfileLocalSettingsCheck = true;
            NewProfileLocalScreenshotsCheck = true;
            NewProfileLocalTrayCheck = true;
        }

        private void EditProfileClick(){
            if (CategoriesCV.CurrentItem == null){
                System.Windows.Forms.MessageBox.Show("You must select a profile to edit.");
            } else {
                editing = true;
                editingprofile = ProfilesCV.CurrentItem as Profile;
                NewProfileVis = Visibility.Visible;
                NewProfileLabel = "Edit Profile";
                NewProfileName = editingprofile.Name;
                NewProfileLocalSavesCheck = editingprofile.LocalSaves;
                NewProfileLocalSettingsCheck = editingprofile.LocalSettings;
            }
        }

        private void DeleteProfileClick(){
            editingprofile = ProfilesCV.CurrentItem as Profile;
            Profiles.Remove(editingprofile);
            DeleteProfileIni(editingprofile.Name);
            ProfilesCV.Refresh();
        }

        private void DuplicateProfileClick(){
            editingprofile = ProfilesCV.CurrentItem as Profile;
            string profname = string.Format("{0}-Copy", editingprofile.Name);
            Profiles.Add(new() {Name = profname});
            CreateProfileIni(profname, editingprofile);
            ProfilesCV.Refresh();
        }
        private void CloseProfileWindowClick(){
            editing = false;
            ProfileScreenVis = Visibility.Hidden;
            PopupGreyed = Visibility.Hidden;
        }

        private void SaveProfileClick(){
            Profile prof = new();
            prof.Name = NewProfileName;
            prof.LocalSettings = NewProfileLocalSettingsCheck;
            prof.LocalSaves = NewProfileLocalSavesCheck;
            prof.LocalScreenshots = NewProfileLocalScreenshotsCheck;
            prof.LocalTray = NewProfileLocalTrayCheck;            

            if (editing == true){
                if (string.IsNullOrWhiteSpace(NewProfileName)){
                    System.Windows.Forms.MessageBox.Show("Profile must have a name.");
                } else if (Profiles.Where(x => x.Name == NewProfileName).Any() && NewProfileName != editingprofile.Name) {
                    System.Windows.Forms.MessageBox.Show("A profile with this name already exists.");
                } else {
                    int idx = Profiles.IndexOf(editingprofile);                    
                    Profiles.Remove(editingprofile);
                    Profiles.Add(prof);
                    if (prof.Name != editingprofile.Name){
                        string profileini = Path.Combine(ThisInstance.InstanceDataFolder, string.Format("profile_{0}.ini", prof.Name));
                        string ogprofileini = Path.Combine(ThisInstance.InstanceDataFolder, string.Format("profile_{0}.ini", editingprofile.Name));
                        File.Move(ogprofileini, profileini);
                    }
                    if (prof.LocalSettings == true || prof.LocalSaves == true || prof.LocalScreenshots == true || prof.LocalTray == true){                
                        if (!Directory.Exists(Path.Combine(ThisInstance.InstanceProfilesFolder, prof.Name))){
                            Directory.CreateDirectory(Path.Combine(ThisInstance.InstanceProfilesFolder, prof.Name));
                        }
                    }
                    SaveProfileData(prof);
                }
            } else {
                if (string.IsNullOrWhiteSpace(NewProfileName)){
                    System.Windows.Forms.MessageBox.Show("Profile must have a name.");
                } else if (Profiles.Where(x => x.Name == NewProfileName).Any()) {
                    System.Windows.Forms.MessageBox.Show("A profile with this name already exists.");
                } else {
                    Profiles.Add(prof);
                    CreateProfileIni(prof.Name);
                    if (prof.LocalSettings == true || prof.LocalSaves == true || prof.LocalScreenshots == true || prof.LocalTray == true){                
                        if (!Directory.Exists(Path.Combine(ThisInstance.InstanceProfilesFolder, prof.Name))){
                            Directory.CreateDirectory(Path.Combine(ThisInstance.InstanceProfilesFolder, prof.Name));
                        }                            
                    }
                    NewProfileVis = Visibility.Hidden;
                }
                SaveProfileData(prof);
            }            
            NewProfileVis = Visibility.Hidden;
            ProfilesCV.Refresh();            
        }

        private void CancelProfileClick(){            
            NewProfileVis = Visibility.Hidden;
        }
        
        private void CreateProfileIni(string name, Profile copyfile){
            string profileini = Path.Combine(ThisInstance.InstanceDataFolder, string.Format("profile_{0}.ini", name));
            string ogprofileini = Path.Combine(ThisInstance.InstanceDataFolder, string.Format("profile_{0}.ini", copyfile.Name));
            File.Copy(ogprofileini, profileini);
            using (FileStream fs = new FileStream(profilesfile, FileMode.Open, FileAccess.Read)){
                using (StreamWriter sw = new(fs)){
                    sw.WriteLine(name);
                    sw.Flush();
                    sw.Close();
                }
                fs.Close(); fs.Dispose();
            }
        }
        private void CreateProfileIni(string name){
            string profileini = Path.Combine(ThisInstance.InstanceDataFolder, string.Format("profile_{0}.ini", name));
            File.WriteAllText(profileini, "");
            using (FileStream fs = new FileStream(profilesfile, FileMode.Open, FileAccess.Read)){
                using (StreamWriter sw = new(fs)){
                    sw.WriteLine(name);
                    sw.Flush();
                    sw.Close();
                }
                fs.Close(); fs.Dispose();
            }    
        }

        private void DeleteProfileIni(string name){
            string profileini = Path.Combine(ThisInstance.InstanceDataFolder, string.Format("profile_{0}.ini", name));
            File.Delete(profileini);
        }

        private void WriteProfilesFile(){
            using (FileStream fs = File.Create(profilesfile)){
                using (StreamWriter sw = new(fs)){
                    foreach (Profile profile in Profiles){
                        sw.WriteLine(string.Format("ProfileName=Defaul{0}", profile.Name));
                    }                    
                    sw.Flush();
                    sw.Close();
                }
                fs.Close(); fs.Dispose();
            } 
        }






        private void ViewCategoriesClick(){
            CategoriesCV.Refresh();
            CategoryScreenVis = Visibility.Visible;
            PopupGreyed = Visibility.Visible;
        }


        private void EditCategoriesClick(){
            if (CategoriesCV.CurrentItem == null){
                System.Windows.Forms.MessageBox.Show("You must select a category to edit.");
            } else {
                NewCatLabel = "Edit Category";
                editing = true;
                editingcat = CategoriesCV.CurrentItem as CategoryType;
                NewCategoryDescription = "";
                if (editingcat.Description != null) NewCategoryDescription = "";
                NewCategoryName = editingcat.Name; 
                NewCategoryColor = editingcat.ColorHex;
                NewCategoryColorBrush = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(NewCategoryColor);
                NewCategoryVis = Visibility.Visible;
                PopupGreyed = Visibility.Hidden;
            }
        }

        private void DeleteCategoryClick(){
            editingcat = CategoriesCV.CurrentItem as CategoryType;
            int idx = Categories.IndexOf(editingcat);
            new Thread(() => RemoveFromCategories(editingcat)){IsBackground = true}.Start();
            Categories.Remove(editingcat);
            CategoriesCV.Refresh();            
            InstanceModsCV.Refresh();
        }

        private void RemoveFromCategories(CategoryType category){
            List<ModInfo> mods = InstanceMods.Where(x => x.Category == category).ToList();
            if (mods.Count != 0){
                foreach (ModInfo mod in mods){
                    int idx = InstanceMods.IndexOf(mod);
                    InstanceMods[idx].Category = Categories[0];
                    ModInfoToFile(InstanceMods[idx], FileToIni(InstanceMods[idx].Location));
                }
            }
        }

        private void CloseCategoryWindowClick(){
            CategoryScreenVis = Visibility.Hidden;
            PopupGreyed = Visibility.Hidden;
        }

        private void NewCategoryClick(){
            NewCatLabel = "Create a New Category";
            NewCategoryVis = Visibility.Visible;
            PopupGreyed = Visibility.Hidden;   
            NewCategoryDescription = "";
            NewCategoryName = "";
            NewCategoryColor = "#ffffff";
            NewCategoryColorBrush = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(NewCategoryColor);
        }
        private void CategoryPickColorClick(){
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                NewCategoryRGB = MakeRGBColor(colorDialog.Color);
                NewCategoryColor = NewCategoryRGB.ToHex();
                HsvColor alt = NewCategoryRGB.ToHsv();
                if (alt.Value <= 50){
                    alt.Value += 0.1;
                    alt.Saturation -= 0.1;
                } else {
                    alt.Value -= 0.1;
                    alt.Saturation += 0.1;
                }
                NewCategoryAltColor = alt.ToHex();
                NewCategoryColorBrush = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(NewCategoryColor);
                NewCategoryAltColorBrush = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(NewCategoryAltColor);
            }
        }

        private void CancelCategoryClick(){
            NewCategoryVis = Visibility.Hidden;
            editing = false;
        }

        private void SaveCategoryClick(){
            if (editing == true){
                if (string.IsNullOrWhiteSpace(NewCategoryName)){
                    System.Windows.Forms.MessageBox.Show("Category must have a name.");
                } else if (Categories.Where(x => x.Name == NewCategoryName).Any() && NewCategoryName != editingcat.Name) {
                    System.Windows.Forms.MessageBox.Show("A profile with this name already exists.");
                } else {
                    int idx = Categories.IndexOf(editingcat);
                    CategoryType newcat = new(){ Name = NewCategoryName, Description = NewCategoryDescription, ColorHex = NewCategoryColor, ColorBrush = NewCategoryColorBrush, ColorHexAlt = NewCategoryAltColor, ColorBrushAlt = NewCategoryAltColorBrush};
                    Categories.Remove(editingcat);
                    Categories.Add(newcat);
                }
            } else {
                if (string.IsNullOrWhiteSpace(NewCategoryName)){
                    System.Windows.Forms.MessageBox.Show("Category must have a name.");
                } else if (Categories.Where(x => x.Name == NewCategoryName).Any()) {
                    System.Windows.Forms.MessageBox.Show("A category with this name already exists.");
                } else {
                    Categories.Add(new CategoryType() { Name = NewCategoryName, Description = NewCategoryDescription, ColorHex = NewCategoryColor, ColorBrush = NewCategoryColorBrush, ColorHexAlt = NewCategoryAltColor, ColorBrushAlt = NewCategoryAltColorBrush});
                    NewCategoryVis = Visibility.Hidden;
                }
            }
            NewCategoryVis = Visibility.Hidden;
            CategoriesCV.Refresh();
            SaveInstanceInfo();
        }        

        private void SaveModEditClick(){            
            EditScreenVis = Visibility.Hidden;
            PopupGreyed = Visibility.Hidden;
            var mod = InstanceMods.Where(x => x.Location == EditingMod.Location).First();
            int idx = InstanceMods.IndexOf(mod);
            InstanceMods[idx] = EditingMod;
        }

        private void CancelModEditClick(){            
            EditScreenVis = Visibility.Hidden;
            PopupGreyed = Visibility.Hidden;
        }
        
        
        private void MoveZipsYesDecision(){
            SettingsFile.SaveSetting("AutoMoveCompressedFiles", "true");
            SettingsFile.SaveSetting("AskedAboutMove", "true");
            AutoMoveCompressed = true;
            AskedAboutMove = true;
            FoundZipsGridVis = Visibility.Hidden;
            PopupGreyed = Visibility.Hidden;
            Task move = Task.Run(() => MoveAllCompressed());
            move.Wait();
            RefreshModlist();
        }
        private void MoveZipsNoDecision(){
            SettingsFile.SaveSetting("AutoMoveCompressedFiles", "false");
            SettingsFile.SaveSetting("AskedAboutMove", "true");
            AutoMoveCompressed = false;
            AskedAboutMove = true;
            FoundZipsGridVis = Visibility.Hidden;
            PopupGreyed = Visibility.Hidden;
        }        

        private void SortFilesClick(){
            //temp dev button
            var m = InstanceMods.Where(x => x.Name == "sunblind").First();            
            Console.WriteLine("{0}: EditingName is {1}", m.Name, m.EditingName);
        }

        private void AddModsToInstance(){
            using (var GetMods = new OpenFileDialog()){
                GetMods.InitialDirectory = ThisInstance.InstanceModFolder;
                GetMods.Multiselect = true;
                GetMods.Title = "Select Mods";
                GetMods.Filter = "Processable Files (*.package;*.sims2pack;*.sims3pack;*.zip;*.rar;*.7z;*.pkg;)|*.package;*.sims2pack;*.sims3pack;*.zip;*.rar;*.7z;*.pkg;";
                DialogResult result = GetMods.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK) {
                    if (GetMods.FileNames.Length == 1){
                        File.Move(GetMods.FileName, Path.Combine(ThisInstance.InstanceModFolder, GetMods.SafeFileName));
                    } else {
                        for (int i = 0; i < GetMods.FileNames.Length; i++){
                            File.Move(GetMods.FileNames[i], Path.Combine(ThisInstance.InstanceModFolder, GetMods.SafeFileNames[i]));
                        } 
                    }
                }                
            }
            RefreshModlist();
        }        

        private void AddModsFolderToInstance(){
            using (var GetMods = new FolderBrowserDialog()){
                GetMods.InitialDirectory = ThisInstance.InstanceModFolder;
                GetMods.ShowNewFolderButton = true;
                GetMods.Description = "Select Folder Containing Mods";
                DialogResult result = GetMods.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK) {
                    DirectoryInfo di = new(GetMods.SelectedPath);
                    Directory.Move(GetMods.SelectedPath, Path.Combine(ThisInstance.InstanceModFolder, di.Name));
                }                
            }
            RefreshModlist();
        }        

        private void MoveAllCompressed(){
            List<string> files = Directory.GetFiles(ThisInstance.InstanceModFolder, "", SearchOption.AllDirectories).ToList();            
            List<FileInfo> filesinf = new();
            foreach (string file in files){
                filesinf.Add(new FileInfo(file));
            }
            foreach (FileInfo file in filesinf){
                if (compressedext.Contains(file.Extension)){
                    File.Move(file.FullName, Path.Combine(ThisInstance.InstanceDownloadsFolder, file.Name));
                }
            }            
        }       

        #endregion





        #region Extensions       

        private void StringToCategories(string input){
            List<string> splitv = input.Split(";").ToList();
            foreach (string split in splitv){
                string[] item = split.Split(": ");
                Categories.Add(new CategoryType() {Name = item[0], ColorHex = item[1]});
            }
        }

        private static bool IsDirectory(string path){
            FileAttributes attributes = File.GetAttributes(path);
            if (attributes.HasFlag(FileAttributes.Directory)){
                return true;
            } else {
                return false;
            }
        }

        private static GFileType TypeFromFile(string input){
            if (input == "Package"){
                return GFileType.Package;
            } else if (input == "TS4Script"){
                return GFileType.TS4Script;
            }  else if (input == "Sims2Pack"){
                return GFileType.Sims2Pack;
            }  else if (input == "Sims3Pack"){
                return GFileType.Sims3Pack;
            }  else if (input == "Compressed"){
                return GFileType.Compressed;
            }  else if (input == "Tray File"){
                return GFileType.Tray;
            } else {
                return GFileType.Other;
            }
        }

        private string TypeToFile(GFileType input){
            if (input == GFileType.Package){
                return "Package";
            } else if (input == GFileType.TS4Script){
                return "TS4Script";
            } else if (input == GFileType.Sims2Pack){
                return "Sims2Pack";
            } else if (input == GFileType.Sims3Pack){
                return "Sims3Pack";
            } else if (input == GFileType.Compressed){
                return "Compressed";
            } else if (input == GFileType.Tray){
                return "Tray File";
            } else if (input == GFileType.Other){
                return "Other";
            } else {
                return "Other";
            }
        }

        static readonly string[] SizeSuffixes = 
                   { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        static string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            //From https://stackoverflow.com/a/14488941 
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeSuffix(-value, decimalPlaces); } 
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}", 
                adjustedSize, 
                SizeSuffixes[mag]);
        }

        private static DateTime DateTimeFromString(string input){
            
            string[] dt = input.Split("-");
            string[] hm = dt[3].Split(":");
            
            DateTime output = new();
            output.AddYears(Convert.ToInt32(dt[0]));
            output.AddMonths(Convert.ToInt32(dt[1]));
            output.AddDays(Convert.ToInt32(dt[2]));
            output.AddHours(Convert.ToInt32(hm[0]));
            output.AddMinutes(Convert.ToInt32(hm[1]));
            return output;
        }

        private static bool StringToBool(string input){
            if (input == "true") return true;
            return false;
        }

        private static string BoolToString(bool input){
            if (input == true) return "true";
            return "false";
        }

        private static string StringFromType(GFileType input){
            if (input == GFileType.Package) return "Package";
            if (input == GFileType.Sims2Pack) return "Sims2Pack";
            if (input == GFileType.Sims3Pack) return "Sims3Pack";
            if (input == GFileType.TS4Script) return "TS4Script";
            if (input == GFileType.Compressed) return "Compressed";
            if (input == GFileType.Tray) return "Tray";            
            if (input == GFileType.Other) return "Other";
            return "Other";
        }

        private static GFileType TypeFromString(string input){
            if (input == "Package") return GFileType.Package;
            if (input == "TS4Script") return GFileType.TS4Script;
            if (input == "Sims2Pack") return GFileType.Sims2Pack;
            if (input == "Sims3Pack") return GFileType.Sims3Pack;
            if (input == "Compressed") return GFileType.Compressed;
            if (input == "Tray") return GFileType.Tray;
            if (input == "Other") return GFileType.Other;
            return GFileType.Other;
        }

        private static string CompressedTypeToString(CompressedType input){
            if (input == CompressedType.Zip) return "Zip";
            if (input == CompressedType.Rar) return "Rar";
            if (input == CompressedType.SevenZip) return "7z";
            if (input == CompressedType.Pkg) return "Pkg";
            if (input == CompressedType.Unknown) return "Unknown";
            return "Unknown";
        }

        private static CompressedType CompressedTypeFromString(string input){
            if (input == "Zip") return CompressedType.Zip;
            if (input == "Rar") return CompressedType.Rar;
            if (input == "7z") return CompressedType.SevenZip;
            if (input == "Pkg") return CompressedType.Pkg;
            if (input == "Unknown") return CompressedType.Unknown;
            return CompressedType.Unknown;
        }

        private static string StringListToString(List<string> list){
            string ss = "";
            foreach (string s in list){
                if (string.IsNullOrEmpty(ss)){
                    ss = s;
                } else {
                    ss += string.Format(",{0}", s);
                }
            }
            return ss;
        }

        private static List<string> StringToStringList(string input){
            List<string> split = input.Split(",").ToList();
            return split;
        }

        private static string FileToIni(string file){
            FileInfo fi = new FileInfo(file);
            string ex = fi.Extension;
            if(string.IsNullOrEmpty(ex)){
                return string.Format("{0}.ini", file);
            } else {
                return file.Replace(ex, ".ini");
            }
        }

        private static string StripExtension(string file){
            FileInfo fi = new FileInfo(file);
            string ex = fi.Extension;
            string ff = fi.Name.Replace(ex, "");
            return ff;
        }

        public static RgbColor MakeRGBColor(System.Drawing.Color color){
            RgbColor rgbColor = new(color.R, color.G, color.B);
            return rgbColor;
        }

        #endregion


        #region Things to Run the Actual Game


        private void RunExe(string ExeName, string arguments){
            // Prepare the process to run
            ProcessStartInfo start = new ProcessStartInfo();
            // Enter in the command line arguments, everything you would enter after the executable name itself
            start.Arguments = arguments; 
            // Enter the executable to run, including the complete path
            start.FileName = ExeName;
            // Do you want to show a console window?
            start.WindowStyle = ProcessWindowStyle.Hidden;
            start.CreateNoWindow = true;
            int exitCode;


            // Run the external process & wait for it to finish
            using (Process proc = Process.Start(start))
            {
                proc.WaitForExit();

                // Retrieve the app's exit code
                exitCode = proc.ExitCode;
            }
        }



        #endregion











        #region Event Handlers

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion


    }

    public class InstanceData{
        public string Name {get; set;}
        public string Location {get; set;}
        public string Version {get; set;}
        public string InstallLocation {get; set;}
        public string GameEXE {get; set;}
        public string Game {get; set;}
        public string GameModFolder {get; set;}
        public string InstanceModFolder {get; set;}
        public string InstanceDataFolder {get; set;}
        public string InstanceDownloadsFolder {get; set;}
        public string InstanceProfilesFolder {get; set;}
        public string ActiveExe {get; set;}
        public string ActiveProfile {get; set;}
        public string GameIcon {get; set;}
        public List<string> Profiles {get; set;}

        public InstanceData(){
            Profiles = new();
        }

    }

    public class Profile : INotifyPropertyChanged{
        public string Name {get; set;}
        public bool LocalSaves {get; set;}
        public bool LocalSettings {get; set;}
        public bool LocalTray {get; set;}
        public bool LocalScreenshots {get; set;}
        private List<ModInfo> _EnabledMods;
        public List<ModInfo> EnabledMods {get { return _EnabledMods; }
            set {
                _EnabledMods = value;
                PropertyChanged?.Invoke(this, 
                    new PropertyChangedEventArgs(nameof(EnabledMods)));
            }
        }


        public Profile(){
            EnabledMods = new();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class Executable {
        public string Name {get; set;}
        public string Destination {get; set;}
        public string Arguments {get; set;}
    }

    public class ModInfo : INotifyPropertyChanged {
        private string _name;
        public string Name {
            get { return _name; }
            set {
                _name = value;
                PropertyChanged?.Invoke(this, 
                    new PropertyChangedEventArgs(nameof(Name)));
            }
        }
        public string Location {get; set;}
        private bool _outofdate;        
        public bool OutOfDate {
            get { return _outofdate; }
            set {
                _outofdate = value;
                PropertyChanged?.Invoke(this, 
                    new PropertyChangedEventArgs(nameof(OutOfDate)));
                    if (value == true){
                        OODtext = "Mark as Updated";
                    } else {
                        OODtext = "Mark as Out of Date";
                    }
            }
        }
        private string _oodtext;
        public string OODtext {
            get { return _oodtext; }
            set {
                _oodtext = value;
                PropertyChanged?.Invoke(this, 
                    new PropertyChangedEventArgs(nameof(OutOfDate)));
            }
        }
        public string Version {get; set;}
        public bool Conflicts {get; set;} = false; 
        public bool New {get; set;} = false;
        public bool Orphan {get; set;} = false;
        public bool LooseScript {get; set;} = false;
        public DateTime DateAdded {get; set;}
        public DateTime DateUpdated {get; set;}
        public DateTime DateEnabled {get; set;}
        public string Game {get; set;}
        private long _size;
        public long Size {
            get { return _size; }
            set { _size = value; 
            SizeString = Converters.SizeSuffix(Size, 2);
            }
        }
        public string SizeString {get; set;}
        public bool Processed {get; set;} = false;
        private bool _scanned;
        public bool Scanned {
            get { return _scanned; }
            set {
                _scanned = value;
                PropertyChanged?.Invoke(this, 
                    new PropertyChangedEventArgs(nameof(Scanned)));
                    if (value == true){
                        ScanVis = Visibility.Collapsed;
                    } else {
                        ScanVis = Visibility.Visible;
                    }
            }
        }
        public Visibility ScanVis {get; set;} = Visibility.Visible;
        private string _creator;
        public string Creator {
            get { return _creator; }
            set {
                _creator = value;
                PropertyChanged?.Invoke(this, 
                    new PropertyChangedEventArgs(nameof(Creator)));
            }
        }
        private string _source;
        public string Source {
            get { return _source; }
            set {
                _source = value;
                PropertyChanged?.Invoke(this, 
                    new PropertyChangedEventArgs(nameof(Source)));
            }
        }
        public bool HasScript {get; set;} = false;
        public GFileType Type {get; set;} 
        public bool WrongGame {get; set;} = false;
        public bool Override {get; set;} = false;
        public bool Fave {get; set;} = false;
        public bool Broken {get; set;} = false;
        public bool Combined {get; set;} = false;
        private bool _root;
        public bool Root {
            get { return _root; }
            set { _root = value;
                if (value == true){
                    RootIcon = Visibility.Visible;
                } else {
                    RootIcon = Visibility.Collapsed;
                }
                PropertyChanged?.Invoke(this, 
                    new PropertyChangedEventArgs(nameof(Root)));
            }
        }
        public Visibility ShowScanMenu {get; set;}
        public Visibility RootIcon {get; set;} = Visibility.Collapsed;
        public Visibility OutOfDateIcon {get; set;} = Visibility.Collapsed;
        public Visibility BrokenIcon {get; set;} = Visibility.Collapsed;
        public Visibility ConflictsIcon {get; set;} = Visibility.Collapsed;
        public Visibility WrongGameIcon {get; set;} = Visibility.Collapsed;
        public Visibility OverrideIcon {get; set;} = Visibility.Collapsed;
        public Visibility OrphanIcon {get; set;} = Visibility.Collapsed;
        public Visibility FaveIcon {get; set;} = Visibility.Collapsed;
        public Visibility EnabledIcon {get; set;} = Visibility.Collapsed;
        public List<string> Files {get; set;} 
        public CompressedType CompressionType {get; set;}
        private bool _enabled;
        public bool Enabled {
            get { return _enabled; }
            set {
                _enabled = value;
                PropertyChanged?.Invoke(this, 
                    new PropertyChangedEventArgs(nameof(Enabled)));
            }
        }
        private bool _selected;
        public bool Selected {
            get { return _selected; }
            set {
                _selected = value;
                PropertyChanged?.Invoke(this, 
                    new PropertyChangedEventArgs(nameof(Selected)));
            }
        }
        private bool _editingname;
        public bool EditingName {
            get { return _editingname; }
            set {
                _editingname = value;
                PropertyChanged?.Invoke(this, 
                    new PropertyChangedEventArgs(nameof(EditingName)));
            }
        }
        private string _notes;
        public string Notes {
            get { return _notes; }
            set {
                _notes = value;
                PropertyChanged?.Invoke(this, 
                    new PropertyChangedEventArgs(nameof(Notes)));
            }
        }
        private CategoryType _category;
        public CategoryType Category {
            get { return _category; }
            set {
                _category = value;
                PropertyChanged?.Invoke(this, 
                    new PropertyChangedEventArgs(nameof(Category)));
            }
        }
        
        private bool _isgroup;
        public bool IsGroup {
            get { return _isgroup; }
            set {
                _isgroup = value;
                PropertyChanged?.Invoke(this, 
                    new PropertyChangedEventArgs(nameof(IsGroup)));
            }
        }

        private string _group;
        public string Group {
            get { return _group; }
            set {
                _group = value;
                PropertyChanged?.Invoke(this, 
                    new PropertyChangedEventArgs(nameof(Group)));
            }
        }

        private bool _altcolor;
        public bool AltColor {
            get {return _altcolor;}
            set {_altcolor = value;
            PropertyChanged?.Invoke(this,
            new PropertyChangedEventArgs(nameof(AltColor)));}
        }


        public ImageSource Thumbnail {get; set;}

        public event PropertyChangedEventHandler PropertyChanged;

        public ModInfo(){
            Files = new();
            Category = new() {Name = "Default", ColorHex = "#EBEDEF", ColorHexAlt = "#C9D1D9"};
            EditingName = false;
        }
    }

    public class CategoryType{
        public string Name {get; set;}
        private string _colorhex;
        public string ColorHex {
            get {return _colorhex; }
            set { _colorhex = value; 
            ColorBrush = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(_colorhex);
            ColorHexFG = GetFGColor(value);
            RaisePropertyChanged("ColorHex");}
        }
        private string _colorhexalt;
        public string ColorHexAlt {
            get {return _colorhexalt; }
            set { _colorhexalt = value; 
            ColorBrushAlt = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(_colorhexalt);
            ColorHexAltFG = GetFGColor(value);
            RaisePropertyChanged("ColorHexAlt");}
        }

        private string _colorhexfg;
        public string ColorHexFG{
            get { return _colorhexfg; }
            set { _colorhexfg = value; 
            ColorBrushFG = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(value));
            }
        }
        private string _colorhexaltfg;
        public string ColorHexAltFG{
            get { return _colorhexaltfg; }
            set { _colorhexaltfg = value; 
            ColorBrushAltFG = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(value));
            }
        }
        
        private System.Windows.Media.Brush _colorbrushfg;
        public System.Windows.Media.Brush ColorBrushFG{
            get { return _colorbrushfg; }
            set { _colorbrushfg = value; }
        }
        private System.Windows.Media.Brush _colorbrushaltfg;
        public System.Windows.Media.Brush ColorBrushAltFG{
            get { return _colorbrushaltfg; }
            set { _colorbrushaltfg = value; }
        }

        private static string GetFGColor(string hexinput){
            System.Drawing.Color color = ColorTranslator.FromHtml(hexinput);
            RgbColor rgb = new(color.R, color.G, color.B);
            /*HsvColor hsv = rgb.ToHsv();
            Console.WriteLine("Color: {0}, {1}, {2}", hsv.Hue, hsv.Saturation, hsv.Value);
            if (hsv.Value <= 0.50){
                hsv.Value = hsv.Value * 2;
            } else {
                hsv.Value = 0.15;
            }
            if (hsv.Saturation <= 0.50){
                hsv.Saturation = 0.85;
            } else {
                hsv.Saturation = 0.15;
            }*/
            double newcolor = 0;
            double luminance = (0.299 * rgb.R + 0.587 * rgb.G + 0.114 * rgb.B)/255;
            if (luminance > 0.5){
                newcolor = 0; // bright colors - black font
            } else {
                newcolor = 255; // dark colors - white font
            }
            RgbColor newc = new()
            {
                Red = (byte)newcolor,
                Blue = (byte)newcolor,
                Green = (byte)newcolor
            };

            return newc.ToHex();
        }



        private System.Windows.Media.Color _colorbrush;
        public System.Windows.Media.Color ColorBrush {
            set { _colorbrush = value;
                RaisePropertyChanged("ColorBrush");}
            get { return _colorbrush;}
        }

        private System.Windows.Media.Color _colorbrushalt;
        public System.Windows.Media.Color ColorBrushAlt {
            set { _colorbrushalt = value;
                RaisePropertyChanged("ColorBrushAlt");}
            get { return _colorbrushalt;}
        }

        public string Description {get; set;}
        
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
        public string RemoveHash(string hex){
            string nohash = hex.Replace("#", "");
            return nohash;
        }

        public override string ToString()
        {
            return this.Name;
        }

    }    

    public class NewDownloadInfo{
        public string Name {get; set;}
        public string Location {get; set;}
        public string Extension {get; set;}
        public string Source {get; set;}
        public bool HasScript {get; set;}
        public DateTime DateAdded {get; set;}
        public bool Installed {get; set;}
    }

    public enum GFileType {
        Package,
        TS4Script,
        Sims2Pack,
        Sims3Pack,
        Compressed,
        Tray,
        Other
    }

    public enum CompressedType {
        Zip,
        Rar,
        SevenZip,
        Pkg,
        Unknown
    }

    public class InitialCheck {
        public bool Broken {get; set;}
        public bool Unidentifiable {get; set;}
        public string Game {get; set;}
        public uint Major {get; set;}
        public uint Minor {get; set;}

    }

    public class CreatorInfo {
        public string CreatorName {get; set;}
        public string CreatorURL {get; set;}
        public bool Fave {get; set;}
    }
}