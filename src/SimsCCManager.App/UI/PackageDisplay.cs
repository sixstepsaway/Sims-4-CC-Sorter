using Godot;
using Microsoft.VisualBasic;
using SimsCCManager.Containers;
using SimsCCManager.Debugging;
using SimsCCManager.Globals;
using SimsCCManager.Packages.Containers;
using SimsCCManager.Settings.Loaded;
using SimsCCManager.Settings.SettingsSystem;
using SimsCCManager.UI.Containers;
using SimsCCManager.UI.Utilities;
using SSA.VirtualFileSystem;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public partial class PackageDisplay : MarginContainer
{
	//nodes
	Control ExeChoiceControl;
	Control FloatingItemsContainer;
	HBoxContainer wronggame_sims2;
	HBoxContainer wronggame_sims3;
	HBoxContainer wronggame_sims4;
	LineEdit AMSearch;
	LineEdit DLSearch;
	MarginContainer DataGridAllMods;
	MarginContainer DataGridDownloads;
	MarginContainer ErrorsListing;
	MarginContainer brokenfiles;
	MarginContainer deletefiles;
	MarginContainer renamefileswindow;
	MarginContainer rightclickcatcher;
	MarginContainer wronggamefiles;
	VBoxContainer AllModsRows;
	VBoxContainer DownloadsRows;
	VBoxContainer packagelist;
	

	//Custom nodes
	ApplicationStarter applicationStarter;
	CustomDataGrid AllModsGrid = new();
	CustomDataGrid DownloadedModsGrid = new();
	ExeChoicePopupPanel exeChoicePopupPanel;
	PackageScanner scanner;
	PackageViewer CurrentPackageViewer;
	RightClickMenu rightclickmenu;
	ViewErrors_Container viewerrorscontainer;
	ExeChoicePopupPanel ExeChoicePanel;

	//packed scenes
	PackedScene DataGrid = GD.Load<PackedScene>("res://UI/CustomDataGrid/CustomDataGrid.tscn");
	PackedScene PackageInformation = GD.Load<PackedScene>("res://UI/PackageDisplay_Elements/package_viewer.tscn");
	PackedScene PackageListItem = GD.Load<PackedScene>("res://UI/PackageDisplay_Elements/package_list_item.tscn");
	PackedScene RightClickMenu = GD.Load<PackedScene>("res://UI/PackageDisplay_Elements/right_click_menu.tscn");
	PackedScene LinkFilesWindow = GD.Load<PackedScene>("res://UI/PackageDisplay_Elements/subwindows/LinkFilesWindow.tscn");

	//lists
	List<Category> Categories = new();
	List<DataGridHeaderCell> AMHeaders = new();
	List<DataGridHeaderCell> DLHeaders = new();
	List<Executable> Executables = new();
	List<SimsDownload> _downloads = new();
	public List<SimsDownload> downloads = new();
	List<SimsDownload> unsorteddownloads = new();
	List<SimsPackage> _packages = new();
	public List<SimsPackage> packages = new();
	List<SimsPackage> unsortedpackages = new();
	List<string> Cachefiles = new();
	List<string> Profiles = new();
	List<string> ScannedPackages = new();
	List<string> Thumbs = new();

	//instance information
	GameInstanceBase simsInstance {
		get { return ReturnInstance(); }
		set {}
	}
	Sims2Instance sims2Instance = new();
	Sims3Instance sims3Instance = new();
	Sims4Instance sims4Instance = new();
	public Instance ThisInstance = new();
	ProfileInfo Profile = new();
	Games Game = Games.Null;	
	SortingOptions AMSortingRule = SortingOptions.NotSorted;
	SortingOptions DLSortingRule = SortingOptions.NotSorted;
	StringBuilder errorslist;
	
	//bools
	bool brokenmodsvisible = false;
	bool canstart = false;
	bool dontmovemybrokenfiles = false;
	bool dontmovemywrongfiles = false;
	bool downloadssortingorderchanged = false;
	bool firstrundownloads = false;
	bool firstrunpackages = false;
	bool holdingctrl = false;
	bool holdingshift = false;
	bool mouseinAM = false;
	bool mouseinDL = false;
	bool packagedisplayvisible = false;
	bool packagessortingorderchanged = false;
	bool populatingdownloads = false;
	bool populatingpackages = false;
	bool readingdownloads = false;
	bool readingpackages = false;
	bool wronggamefilesvisible = false;

	//ints
	int NumErrors = 0;
	int _numerrors = 0;
	int amlastselected = -1;
	int dllastselected = -1;
	int filesindownloadsfolder = 0;
	int filesinpackagesfolder = 0;
	int packagedisplayed = -1;
	int paralellism = 0;
	int rcdownload = -1;
	int rcpackage = -1;

	//strings
	string AMSortBy = "";
	string DLSortBy = "";
	string brokenmodsfolder = "";
	string downloadsfolder = "";
	string downloadssortingorder = "";
	string instancedatafolder = "";
	string instancefolder = "";
	string modsfolder = "";
	string othergamesmoveloc = "";
	string packagessortingorder = "";
	string s2moveloc = "";
	string s3moveloc = "";
	string s4moveloc = "";
	string wronggamefolder = "";
	string gamemodsfolder = "";

	//events
	public delegate void DoneLoadingEvent();
	public delegate void HidePbarEvent();
	public delegate void IncrementPbarEvent();
	public delegate void PropertyChangedEvent();
	public delegate void ResetPbarValueEvent();
	public delegate void SetPbarMaxEvent(int value);
	public delegate void ShowPbarEvent();
	private event PropertyChangedEvent DownloadsChanged;
	private event PropertyChangedEvent PackagesChanged;
	public DoneLoadingEvent DoneLoading;
	public HidePbarEvent HidePbar;
	public IncrementPbarEvent IncrementPbar;
	public ResetPbarValueEvent ResetPbarValue;
	public SetPbarMaxEvent SetPbarMax;
	public ShowPbarEvent ShowPbar;
	RightClickMenu CurrentRightClickMenu;
	Category defaultcat = new();
	ConcurrentBag<string> linked = new();
	bool dlready = false;
	bool amready = false;
	bool dlloaded = false;
	bool amloaded = false;
	FileDialog AddFolderDiag;
	FileDialog AddFilesDiag;
	string gameversion = "";
	bool canreadpackages = true;
	


	public override void _Ready()
	{
		
		ThreadPool.GetMaxThreads(out int workerThreadsCount, out int ioThreadsCount);
		if (LoadedSettings.SetSettings.LimitCPU){
			paralellism = (ioThreadsCount - 2) / 2;
		} else {
			paralellism = ioThreadsCount - 2;
		}

		ConnectThings();

		
		
		new Thread (() => {
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog("Starting Package Display.");
			if (ThisInstance.Game == "Sims2") {
				if (GlobalVariables.DebugMode) Logging.WriteDebugLog("Sims 2!");
				Game = Games.Sims2;
				string path = Path.Combine(ThisInstance.InstanceLocation, "Instance.xml");
				if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Loading from path {0}.", path));
				sims2Instance = Utilities.LoadS2Instance(path);
				Executables = sims2Instance.Executables;
				Thumbs = sims2Instance.ThumbnailsFiles;
				Categories = sims2Instance.Categories;
				Profiles = sims2Instance.Profiles;
				instancedatafolder = sims2Instance.InstanceDataFolder;
				modsfolder = sims2Instance.InstancePackagesFolder;
				downloadsfolder = sims2Instance.InstanceDownloadsFolder;
				instancefolder = sims2Instance.InstanceFolder;
				string ver = Utilities.GetGameVersion(Game, sims2Instance.GameDocumentsFolder);
				if (ver != sims2Instance.GameVersion) {
					sims2Instance.GameVersion = ver;
					sims2Instance.WriteXML();
				}
				gameversion = ver;
				GlobalVariables.thisinstance = sims2Instance;
			} else if (ThisInstance.Game == "Sims3"){
				if (GlobalVariables.DebugMode) Logging.WriteDebugLog("Sims 3!");
				Game = Games.Sims3;
				sims3Instance = Utilities.LoadS3Instance(Path.Combine(ThisInstance.InstanceLocation, "Instance.xml"));
				Executables = sims3Instance.Executables;
				Thumbs = sims3Instance.ThumbnailsFiles;
				Categories = sims3Instance.Categories;
				Profiles = sims3Instance.Profiles;
				instancedatafolder = sims3Instance.InstanceDataFolder;
				if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Instance folder: {0}", instancedatafolder));
				modsfolder = sims3Instance.InstancePackagesFolder;
				downloadsfolder = sims3Instance.InstanceDownloadsFolder;		
				instancefolder = sims3Instance.InstanceFolder;
				string ver = Utilities.GetGameVersion(Game, sims3Instance.GameDocumentsFolder);
				if (ver != sims3Instance.GameVersion) {
					sims3Instance.GameVersion = ver;
					sims3Instance.WriteXML();
				}
				gameversion = ver;
				GlobalVariables.thisinstance = sims3Instance;				
			} else if (ThisInstance.Game == "Sims4"){
				if (GlobalVariables.DebugMode) Logging.WriteDebugLog("Sims 4!");
				Game = Games.Sims4;
				sims4Instance = Utilities.LoadS4Instance(Path.Combine(ThisInstance.InstanceLocation, "Instance.xml"));
				Executables = sims4Instance.Executables;
				Thumbs = sims4Instance.ThumbnailsFiles;
				Categories = sims4Instance.Categories;
				Profiles = sims4Instance.Profiles;
				instancedatafolder = sims4Instance.InstanceDataFolder;
				if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Instance folder: {0}", instancedatafolder));
				modsfolder = sims4Instance.InstancePackagesFolder;
				downloadsfolder = sims4Instance.InstanceDownloadsFolder;	
				instancefolder = sims4Instance.InstanceFolder;	
				string ver = Utilities.GetGameVersion(Game, sims4Instance.GameDocumentsFolder);
				if (ver != sims4Instance.GameVersion) {
					sims4Instance.GameVersion = ver;
					sims4Instance.WriteXML();
				}
				gameversion = ver;
				GlobalVariables.thisinstance = sims4Instance;
			}
			brokenmodsfolder = Path.Combine(instancefolder, "Broken Packages");
			wronggamefolder = Path.Combine(instancefolder, "Incorrect Game Files");
			GlobalVariables.CurrentInstance = ThisInstance;
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Game version: {0}", gameversion));
			SetupDisplay();
			UpdateExecutableList();			
			//var x = WaitLoad();			
		}){IsBackground = true}.Start();		
	}

	private GameInstanceBase ReturnInstance(){
		if (Game == Games.Sims2) return sims2Instance;
		if (Game == Games.Sims3) return sims3Instance;
		return sims4Instance;
	}

	private void ConnectThings(){
		PackagesChanged += () => OnPackagesListChanged();
		DownloadsChanged += () => OnDownloadsListChanged();
		AddFilesDiag = GetNode<FileDialog>("AddFileDialog");
		AddFolderDiag = GetNode<FileDialog>("AddFolderDialog");

		AddFilesDiag.FilesSelected += (files) => AddFilesSelected(files);
		AddFolderDiag.DirSelected += (dir) => AddFolderSelected(dir);

		GetNode<CustomCheckButton>("MainWindowSizer/MainPanels/HSplitContainer/AllMods_Frame_Container/ContainerHeader/TogglePositioner/CustomCheckButton").CheckToggled += () => DetailedViewToggled();		

		viewerrorscontainer = GetNode<ViewErrors_Container>("MainWindowSizer/TopPanels/SettingsAndHelpControls/HBoxContainer/ViewErrors_Container");
		viewerrorscontainer.GetNode<topbar_button>("View Errors_SettingsHelpButtons").ButtonClicked += () => ViewErrors();
		ErrorsListing = GetNode<MarginContainer>("ViewErrors");
		//GetNode<Button>("").Pressed += () => ConfirmMoveIncorrectFiles();
		GetNode<Button>("ViewErrors/MarginContainer/VBoxContainer/HBoxContainer/FixIncorrect_Button").Pressed += () => WrongGameDetected();
		GetNode<Button>("ViewErrors/MarginContainer/VBoxContainer/HBoxContainer/FixBroken_Button").Pressed += () => BrokenPackagesDetected();
		GetNode<Button>("ViewErrors/MarginContainer/VBoxContainer/HBoxContainer/Cancel_Button").Pressed += () => CancelViewErrors();

		GetNode<Button>("MoveWrongGameFiles/MarginContainer/VBoxContainer/HBoxContainer/Move_Button").Pressed += () => ConfirmMoveIncorrectFiles();
		GetNode<Button>("MoveWrongGameFiles/MarginContainer/VBoxContainer/HBoxContainer/Cancel_Button").Pressed += () => CancelMoveIncorrectFiles();
		GetNode<Button>("BrokenFiles/MarginContainer/VBoxContainer/HBoxContainer/Move_Button").Pressed += () => ConfirmMoveBrokenFiles();
		GetNode<Button>("BrokenFiles/MarginContainer/VBoxContainer/HBoxContainer/Cancel_Button").Pressed += () => CancelMoveBrokenFiles();
		wronggame_sims2 = GetNode<HBoxContainer>("MoveWrongGameFiles/MarginContainer/VBoxContainer/ScrollContainer/VBoxContainer/Sims2Wrong_Location");
		wronggame_sims3 = GetNode<HBoxContainer>("MoveWrongGameFiles/MarginContainer/VBoxContainer/ScrollContainer/VBoxContainer/Sims3Wrong_Location");
		wronggame_sims4 = GetNode<HBoxContainer>("MoveWrongGameFiles/MarginContainer/VBoxContainer/ScrollContainer/VBoxContainer/Sims4Wrong_Location");
		brokenfiles = GetNode<MarginContainer>("BrokenFiles");
		wronggamefiles = GetNode<MarginContainer>("MoveWrongGameFiles");
		
		wronggame_sims2.GetNode<LineEdit>("Sims2Wrong_Location_LineEdit").TextChanged += (text) => WrongGameSims2Loc_TxtChanged(text);		
		wronggame_sims3.GetNode<LineEdit>("Sims3Wrong_Location_LineEdit").TextChanged += (text) => WrongGameSims3Loc_TxtChanged(text);
		wronggame_sims4.GetNode<LineEdit>("Sims4Wrong_Location_LineEdit").TextChanged += (text) => WrongGameSims4Loc_TxtChanged(text);
		GetNode<LineEdit>("MoveWrongGameFiles/MarginContainer/VBoxContainer/ScrollContainer/VBoxContainer/OtherWrong_Location/OtherWrong_Location_LineEdit").TextChanged += (text) => WrongGameOtherLoc_TxtChanged(text);
		
		AMSearch = GetNode<LineEdit>("MainWindowSizer/MainPanels/HSplitContainer/AllMods_Frame_Container/ContainerHeader/SearchPositioner/AllMods_SearchBox");
		DLSearch = GetNode<LineEdit>("MainWindowSizer/MainPanels/HSplitContainer/VSplitContainer/NewDownloads_Frame_Container/ContainerHeader/SearchPositioner/NewDL_SearchBox");
		AMSearch.TextChanged += (text) => AMPackagesSearched(text);
		exeChoicePopupPanel = GetNode<ExeChoicePopupPanel>("MainWindowSizer/TopPanels/GameStartControls/ExeChoice_PopupPanel_Control/ExeChoice_PopupPanel");
		exeChoicePopupPanel.PickedExe += (exe) => _on_exe_choice_popup_panel_picked_exe(exe);
		exeChoicePopupPanel.ExeIcon += (texture, exename, exe) => ExeIconEvent(texture, exename, exe);
		deletefiles = GetNode<MarginContainer>("DeleteItemsBox");
		deletefiles.Visible = false;
		GetNode<Button>("DeleteItemsBox/MarginContainer/VBoxContainer/HBoxContainer/Delete_Button").Pressed += () => ConfirmDeleteFilesButton();
		GetNode<Button>("DeleteItemsBox/MarginContainer/VBoxContainer/HBoxContainer/Cancel_Button").Pressed += () => CancelDeleteFilesButton();
		packagelist = GetNode<VBoxContainer>("RenameItemsBox/MarginContainer/VBoxContainer/ScrollContainer/VBoxContainer");
		renamefileswindow = GetNode<MarginContainer>("RenameItemsBox");
		renamefileswindow.Visible = false;
		GetNode<Button>("RenameItemsBox/MarginContainer/VBoxContainer/HBoxContainer/Rename_Button").Pressed += () => Rename_RenamePressed();
		GetNode<Button>("RenameItemsBox/MarginContainer/VBoxContainer/HBoxContainer/Cancel_Button").Pressed += () => Rename_CancelPressed();
		GetNode<Button>("RenameItemsBox/MarginContainer/VBoxContainer/HBoxContainer2/GetAllNamesBox/Button").Pressed += () => GetAllPackageNames();
		FloatingItemsContainer = GetNode<Control>("FloatingItemsContainer");
		rightclickcatcher = GetNode<MarginContainer>("FloatingItemsContainer/RightClickCatcher");
		scanner = GetNode<PackageScanner>("PackageScanner");
		scanner.PackageScanned += (package) => PackageScanned(package);
		GetNode<MarginContainer>("MainWindowSizer/MainPanels/HSplitContainer/VSplitContainer/PackageViewer_Frame_Container").Visible = packagedisplayvisible;
		applicationStarter = GetNode<ApplicationStarter>("ApplicationStarter");
		LoadedSettings.SetSettings.LastInstanceLoaded = LoadedSettings.SetSettings.CurrentInstance.InstanceLocation;
		ExeChoiceControl = GetNode<Control>("MainWindowSizer/TopPanels/GameStartControls/ExeChoice_PopupPanel_Control");
		ExeChoicePanel = GetNode<ExeChoicePopupPanel>("MainWindowSizer/TopPanels/GameStartControls/ExeChoice_PopupPanel_Control/ExeChoice_PopupPanel");
		ExeChoiceControl.Visible = false;
		DataGridAllMods = GetNode<MarginContainer>("MainWindowSizer/MainPanels/HSplitContainer/AllMods_Frame_Container/AllMods_Container/GridContainer");
		DataGridDownloads = GetNode<MarginContainer>("MainWindowSizer/MainPanels/HSplitContainer/VSplitContainer/NewDownloads_Frame_Container/NewDL_Container/GridContainer");


		GetNode<topbar_button>("MainWindowSizer/TopPanels/Profiles/HBoxContainer/Profiles_Button").ButtonClicked += () => ProfilesButtonClicked();
		GetNode<topbar_button>("MainWindowSizer/TopPanels/Buttons/HBoxContainer/AddFiles_Topbar_Button").ButtonClicked += () => AddFilesButtonClicked();
		GetNode<topbar_button>("MainWindowSizer/TopPanels/Buttons/HBoxContainer/AddFolder_Topbar_Button").ButtonClicked += () => AddFolderButtonClicked();
		GetNode<topbar_button>("MainWindowSizer/TopPanels/Buttons/HBoxContainer/Refresh_Topbar_Button").ButtonClicked += () => RefreshPackagesButtonClicked();
		GetNode<topbar_button>("MainWindowSizer/TopPanels/Buttons/HBoxContainer/SortSubfolders_Topbar_Button").ButtonClicked += () => SortIntoSubfoldersButtonClicked();
		GetNode<topbar_button>("MainWindowSizer/TopPanels/Buttons/HBoxContainer/ScanAll_Topbar_Button").ButtonClicked += () => ScanAllButtonClicked();
		GetNode<topbar_button>("MainWindowSizer/TopPanels/Buttons/HBoxContainer/ManageCats_Topbar_Button").ButtonClicked += () => ManageCategoriesClicked();
		GetNode<topbar_button>("MainWindowSizer/TopPanels/Buttons/HBoxContainer/EditExes_Topbar_Button").ButtonClicked += () => EditExesClicked();

		
	}

    private void DetailedViewToggled()
    {
        throw new NotImplementedException();
    }

    public override void _Process(double delta)
    {
		if (canstart){
			if (!readingpackages){
				readingpackages = true;
				new Thread(() => {
					if ((Directory.GetFiles(modsfolder).Count() + Directory.GetDirectories(modsfolder).Count()) != filesinpackagesfolder){
						ReadPackages();
					} else {
						readingpackages = false;
					}
								
				}){IsBackground = true}.Start();

				new Thread(() => {
					if (!dontmovemybrokenfiles){
						if (!wronggamefilesvisible && !brokenmodsvisible){
							if (packages.Where(x => x.Broken).Any()){
								CallDeferred(nameof(BrokenPackagesDetected));
							}
						}
					}														
				}){IsBackground = true}.Start();

				new Thread(() => {
					if (!dontmovemywrongfiles){
						if (!wronggamefilesvisible && !brokenmodsvisible){
							if (packages.Where(x => x.WrongGame).Any()){
								CallDeferred(nameof(WrongGameDetected));
							}
						}
					}
				}){IsBackground = true}.Start();
			}

			if (!readingdownloads){
				new Thread(() => {
					readingdownloads = true;
					if (Directory.GetFiles(downloadsfolder).ToList().Count != filesindownloadsfolder){
						ReadDownloads();
					} else {
						readingdownloads = false;
					}
					
				}){IsBackground = true}.Start();
			}
			new Thread(() => {
				if (packagedisplayvisible){				
					if (packages.Where(x => x.Selected).Count() != 1){					
						CallDeferred(nameof(HidePackageDisplay));
					} else if (packages.IndexOf(packages.Where(x => x.Selected).First()) != packagedisplayed){
						CallDeferred(nameof(HidePackageDisplay));
						CallDeferred(nameof(ShowPackageDisplay));
					}
				} else if (!packagedisplayvisible){
					if (packages.Where(x => x.Selected).Count() == 1){
						CallDeferred(nameof(ShowPackageDisplay));
					}
				}
			}){IsBackground = true}.Start();

			new Thread(() => {
				int broken = packages.Where(x => x.Broken == true).Count();
				int wronggame = packages.Where(x => x.WrongGame == true).Count();
				NumErrors = broken + wronggame;
				if (NumErrors != _numerrors && NumErrors != 0){
					if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("There are {0} errors", NumErrors));
					_numerrors = NumErrors;
					errorslist = new();
					List<SimsPackage> brokenpackages = packages.Where(x => x.Broken == true).ToList();
					List<SimsPackage> wronggamepackages = packages.Where(x => x.WrongGame == true && x.Broken == false).ToList();
					foreach (SimsPackage package in brokenpackages){
						errorslist.AppendLine(string.Format("{0} is broken.", package.FileName));
					}
					foreach (SimsPackage package in wronggamepackages){
						string gm = "an unknown game.";
						if (package.Game == Games.Sims1) gm = "Sims 1";
						if (package.Game == Games.Sims2) gm = "Sims 2";
						if (package.Game == Games.Sims3) gm = "Sims 3";
						if (package.Game == Games.Sims4) gm = "Sims 4";
						if (package.Game == Games.SimsMedieval) gm = "Sims Medieval";
						if (package.Game == Games.Spore) gm = "Spore";
						if (package.Game == Games.SimCity5) gm = "SimCity 5";
						errorslist.AppendLine(string.Format("{0} is for {1}.", package.FileName, gm));
					}
					viewerrorscontainer.HasNotifications = true;
					viewerrorscontainer.Errors = errorslist.ToString();
					if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Updating error display to {0}", errorslist.ToString()));
				} else if (NumErrors != _numerrors && NumErrors == 0) {
					_numerrors = NumErrors;
					if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("No errors! What a life!"));
					viewerrorscontainer.HasNotifications = false;
					viewerrorscontainer.Errors = "";
				}
			}){IsBackground = true}.Start();
		}
		
    }

	private async Task WaitLoad(){
		
    	//await ToSignal(RenderingServer.Singleton, RenderingServerInstance.SignalName.FramePostDraw);
		//if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Package display size: {0}.", DataGridAllMods.Size.Y));
	}




	//data handlers

	private void UpdateExecutableList(){
		ExeChoicePanel.instancedatafolder = instancedatafolder;
		ExeChoicePanel.Executables = Executables;
		CallDeferred(nameof(UpdateExes));
	}
	
	private void PackageScanned(string package){
		ScannedPackages.Add(package);
	}
	



	//package handlers

	public bool ReadPackages(){
		if (!canreadpackages) return false;
		defaultcat = Categories.Where(x => x.Name == "Default").First();
		ConcurrentBag<SimsPackage> packagesbag = new();
		List<string> files = Directory.GetFiles(modsfolder).ToList();
		List<string> directories = Directory.GetDirectories(modsfolder).ToList();
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("There are {0} files to read in Packages", files.Count));
		List<string> infofiles = files.Where(x => x.EndsWith(".info", StringComparison.OrdinalIgnoreCase)).ToList();
		List<string> packagefiles = files.Where(x => !infofiles.Contains(x)).ToList();
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("There are {0} files to read that are not Info Files", packagefiles.Count));

		int pbar = packagefiles.Count + directories.Count;
		pbar -= packages.Count;

		if (directories.Count != 0){
			CallDeferred(nameof(_ShowPbar));
			CallDeferred(nameof(_SetPbarMax), pbar);
			CallDeferred(nameof(_ResetPbarValue));
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Found {0} directories", directories.Count));
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("List of Directories: {0}", Utilities.ListToString(directories)));
			Parallel.ForEach(directories, new ParallelOptions { MaxDegreeOfParallelism = paralellism }, file => {
				
				SimsPackage simsPackage = new();				
				bool infoexists = simsPackage.GetInfo(file, true);
				if (infoexists){
					simsPackage = Utilities.LoadPackageFile(simsPackage);
					if (simsPackage.InstalledForVersion != gameversion){
						simsPackage.OutOfDate = true;
					} else {
						if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Package {0} installed for version {1}, current version is {2}", simsPackage.FileName, simsPackage.InstalledForVersion, gameversion));
					}
					DateTime creationtime = File.GetCreationTime(file);
					DateTime modified = File.GetLastWriteTime(file);
					if (simsPackage.DateUpdated < creationtime){
						simsPackage.DateUpdated = creationtime;
						simsPackage.OutOfDate = false;
					} else if (simsPackage.DateUpdated < modified){
						simsPackage.DateUpdated = modified;
						simsPackage.OutOfDate = false;
					}					
				} else {
					simsPackage.InstalledForVersion = simsInstance.GameVersion;
					simsPackage.Folder = true;
					simsPackage.Category = defaultcat;
					simsPackage.MakeInfo(file, true);
					List<string> filesinfolder = Directory.GetFiles(simsPackage.Location).ToList();
					if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Found {0} files in directory {1}", filesinfolder.Count, file));
					simsPackage.LinkedFiles.AddRange(filesinfolder);
					foreach (string f in filesinfolder){
						GlobalVariables.AddNoInfoFile(f);
						linked.Add(f);
					}
					List<string> foldersinfolder = Directory.GetDirectories(simsPackage.Location).ToList();
					if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Found {0} folders in directory {1}", foldersinfolder.Count, file));
					if (foldersinfolder.Count != 0){
						foreach (string folder in foldersinfolder){
							if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Processing {0} in directory {1}", folder, file));
							GlobalVariables.AddNoInfoFile(folder);
							linked.Add(folder);
							simsPackage.LinkedFolders.Add(GetFolderInfo(folder));						
						}
					}
				}
				if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Checking if this file ({0}) is actually a linked file.", simsPackage.Location));
				if (simsPackage.LinkedFiles.Any()){
					foreach (string ff in simsPackage.LinkedFiles){
						GlobalVariables.AddNoInfoFile(ff);
						linked.Add(ff);
					}
				}
				if (linked.Where(x => x.Equals(simsPackage.Location)).Any()) {					
					if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Looks like {0} exists in the linked bag.", simsPackage.Location));
					/*if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Currently in linked bag:"));
					foreach (string s in linked){
						if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format(s));
					}*/
				}  else {
					if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Can't find {0} in the linked bag.", simsPackage.Location));
					/*if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Currently in linked bag:"));
					foreach (string s in linked){
						if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format(s));
					}*/
					simsPackage.WriteInfoFile();
					simsPackage.PackageChanged += () => PackagesChanged.Invoke();
					packagesbag.Add(simsPackage);
					CallDeferred(nameof(_IncrementPbar));
				}
			});
		}

		if (packagefiles.Count != 0){	
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Found {0} files", packagefiles.Count));		
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("List of Files: {0}", Utilities.ListToString(packagefiles)));
			Parallel.ForEach(packagefiles, new ParallelOptions { MaxDegreeOfParallelism = paralellism }, file => {
				SimsPackage simsPackage = new();				
				FileInfo f = new FileInfo(file);				
				if (f.Extension == ".ts4script"){
					string package = string.Format("{0}.package", f.Name.Replace(".ts4script", ""));
					package = Path.Combine(f.DirectoryName, package);
					string packageinfo = string.Format("{0}.info", package);
					if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Checking if {0} exists", package));		
					if (File.Exists(package) && !File.Exists(packageinfo)){
						return;
					}
				}
				bool infoexists = simsPackage.GetInfo(file);				
				if (infoexists){
					simsPackage = Utilities.LoadPackageFile(simsPackage);
					if (simsPackage.InstalledForVersion != gameversion){
						simsPackage.OutOfDate = true;
					} else {
						if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Package {0} installed for version {1}, current version is {2}", simsPackage.FileName, simsPackage.InstalledForVersion, gameversion));
					}
					DateTime creationtime = File.GetCreationTime(file);
					DateTime modified = File.GetLastWriteTime(file);
					if (simsPackage.DateUpdated < creationtime){
						simsPackage.DateUpdated = creationtime;
						simsPackage.OutOfDate = false;
					} else if (simsPackage.DateUpdated < modified){
						simsPackage.DateUpdated = modified;
						simsPackage.OutOfDate = false;
					}
					if (simsPackage.FileType == FileTypes.TS4Script){
						simsPackage.Game = Games.Sims4;
					}
					simsPackage.WriteInfoFile();
				} else {
					if (f.Extension == ".ts4script"){
						simsPackage.Game = Games.Sims4;
					}
					simsPackage.MakeInfo(file);
					simsPackage.InstalledForVersion = simsInstance.GameVersion;
					DateTime now = DateTime.Now;
					if (simsPackage.FileType == FileTypes.Package) {
						if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Going to scan {0}.", simsPackage.Location));					
						scanner.Scan(simsPackage);
						while (!ScannedPackages.Contains(simsPackage.Identifier.ToString())){
							//wait.
						}
						if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Looks like {0} finished! \n             Started: {1}\n             Finished: {2}.", simsPackage.Location, now, DateTime.Now));
					}
					/*if (infoexists){
						simsPackage = Utilities.LoadPackageFile(simsPackage);
					}*/
				}
				if (simsPackage.Game != Game && !simsPackage.Misc && simsPackage.Game != Games.Null){
					simsPackage.WrongGame = true;
				}
				if (simsPackage.LinkedFiles.Any()){
					foreach (string ff in simsPackage.LinkedFiles){
						GlobalVariables.AddNoInfoFile(ff);
						linked.Add(ff);
					}
				}
				if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("There are {0} packages in the bag.", packagesbag.Count));
				if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("This package ({0}) is for Sims {1}.", simsPackage.FileName, simsPackage.Game));
				if (linked.Where(x => x.Equals(simsPackage.Location)).Any()) {					
					if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Looks like {0} exists in the linked bag.", simsPackage.Location));
					/*if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Currently in linked bag:"));
					foreach (string s in linked){
						if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format(s));
					}*/
				}  else {
					if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Can't find {0} in the linked bag.", simsPackage.Location));
					/*if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Currently in linked bag:"));
					foreach (string s in linked){
						if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format(s));
					}*/
					simsPackage.WriteInfoFile();
					simsPackage.PackageChanged += () => PackagesChanged.Invoke();
					packagesbag.Add(simsPackage);
					CallDeferred(nameof(_IncrementPbar));
				} 			
			});	
			packages = packagesbag.ToList();
			unsortedpackages = packages;
			if (!packagessortingorderchanged) packages = packages.OrderBy(x => x.FileName).ToList();
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("There are {0} packages in packages.", packages.Count));
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("There are {0} ordered packages in packages.", packages.Count));
					
		}
		filesinpackagesfolder = Directory.GetFiles(modsfolder).Count() + Directory.GetDirectories(modsfolder).Count();
		
		
		if (firstrunpackages) PackagesChanged.Invoke();
		
		readingpackages = false;
		return true;
	}

	private SimsPackageSubfolder GetFolderInfo(string folder){
		SimsPackageSubfolder subfolder = new();
		DirectoryInfo f = new(folder);
		List<string> files = Directory.GetFiles(folder).ToList();
		foreach (string fl in files){
			GlobalVariables.AddNoInfoFile(fl);
			linked.Add(fl);
		}
		subfolder.Subfiles.AddRange(files);
		List<string> folders = Directory.GetDirectories(folder).ToList();
		if (folders.Count != 0){
			foreach (string fold in folders){
				GlobalVariables.AddNoInfoFile(fold);
				subfolder.Subfolders.Add(GetFolderInfo(fold));
				linked.Add(fold);
			}
		}
		return subfolder;
	}

	private void MakePackageInformation(){
		CurrentPackageViewer = PackageInformation.Instantiate() as PackageViewer;
		SimsPackage package = packages.Where(x => x.Selected).First();
		packagedisplayed = packages.IndexOf(package);
		ScanData packagedata = package.ScanData;
		if (packagedata != null) {
			if (packagedata.ThumbnailLocation != null){
				CurrentPackageViewer.hasthumbnail = true;
				CurrentPackageViewer.thumbnail = GD.Load<Texture2D>(packagedata.ThumbnailLocation);
			}
			CurrentPackageViewer.PackageName = package.FileName;
			StringBuilder sb = packagedata.PackageInformationDump();	
			CurrentPackageViewer.packageinfo = sb.ToString();	
		}
		
		GetNode<MarginContainer>("MainWindowSizer/MainPanels/HSplitContainer/VSplitContainer/PackageViewer_Frame_Container/PackageViewer_Container").AddChild(CurrentPackageViewer);
	}

	

	//downloads handlers

	public bool ReadDownloads(){
		ConcurrentBag<SimsDownload> downloadsbag = new();
		List<string> files = Directory.GetFiles(downloadsfolder).ToList();
		if (files.Count != 0){
			Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = paralellism }, file => {
				SimsDownload simsDownload = new();
				bool infoexists = simsDownload.GetInfo(file);
				if (infoexists){
					simsDownload = Utilities.LoadDownloadFile(simsDownload);
					downloadsbag.Add(simsDownload);	
				} else {
					FileInfo fi = new(file);
					if (GlobalVariables.SimsFileExtensions.Contains(fi.Extension.ToLower())){
						File.Delete(simsDownload.InfoFile);
						File.Move(fi.FullName, Path.Combine(modsfolder, fi.Name));
					} else {
						downloadsbag.Add(simsDownload);
					}
										
				}				
			});
			downloads = downloadsbag.ToList();
			unsorteddownloads = downloads;
		}
		readingdownloads = false;
		filesindownloadsfolder = Directory.GetFiles(downloadsfolder).Count();
		if (firstrundownloads) DownloadsChanged.Invoke();
		if (downloads.Count == 0){
			DLLoaded();
		}
		return true;
	}



	//data grids

	private void GridConnections(){
			AllModsGrid.SelectedItem += (item, idx) => AllModsItemSelected(item, idx);
			AllModsGrid.UnselectedItem += (item, idx) => AllModsItemUnselected(item, idx);
			AllModsGrid.EnabledItem += (item, idx) => AllModsItemEnabled(item, idx);
			AllModsGrid.DisabledItem += (item, idx) => AllModsItemDisabled(item, idx);		
			AllModsGrid.HeaderSortedSignal += (idx, sortingrule) => AMHeaderSorted(idx, sortingrule);
			AllModsGrid.LoadOrderHasChanged += (ident, val) => LoadOrderItemChanged(ident, val);
			//AllModsGrid = DataGridAllMods.GetChild(0) as CustomDataGrid;
			AllModsRows = AllModsGrid.FindChild("DataGrid_Rows") as VBoxContainer;
			AllModsGrid.MouseAffectingGrid += (inside, idx) => AMMouseEvent(inside, idx);
			DownloadedModsGrid.SelectedItem += (item, idx) => DownloadsItemSelected(item, idx);
			DownloadedModsGrid.UnselectedItem += (item, idx) => DownloadsItemUnselected(item, idx);
			DownloadedModsGrid.HeaderSortedSignal += (idx, sortingrule) => DLHeaderSorted(idx, sortingrule);
			DownloadsRows = DownloadedModsGrid.FindChild("DataGrid_Rows") as VBoxContainer;
			DownloadedModsGrid.MouseAffectingGrid += (inside, idx) => DLMouseEvent(inside, idx);					
			AllModsGrid.GridReady += () => AMReady();
			DownloadedModsGrid.GridReady += () => DLReady();
			AllModsGrid.DoneLoading += () => AMLoaded();
			DownloadedModsGrid.DoneLoading += () => DLLoaded();
	}

	private void AMReady(){
		amready = true;
		if (amready && dlready){
			SetupDisplayPt2();
		}
	}

	private void DLReady(){
		dlready = true;
		if (amready && dlready){
			SetupDisplayPt2();
		}
	}
	private void AMLoaded(){
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("AMGrid loaded."));
		amloaded = true;
		if (amloaded && dlloaded){
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("AMGrid and DLgrid are loaded."));
			EmitDoneLoading();
		}
	}

	private void DLLoaded(){
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("DLGrid loaded."));
		dlloaded = true;
		if (amloaded && dlloaded){
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("AMGrid and DLgrid are loaded."));
			EmitDoneLoading();
		}
	}

	private void SetupDisplay(){		
		new Thread (() => {
			AllModsGrid = DataGrid.Instantiate() as CustomDataGrid;	
			AllModsGrid.Headers = GetDefaultAMHeaders();
			//AllModsGrid.TreeEntered += () => EmitDoneLoading();
			DownloadedModsGrid = DataGrid.Instantiate() as CustomDataGrid;	
			DownloadedModsGrid.Headers = GetDefaultDownloadsHeaders();
			GridConnections();
			CallDeferred(nameof(AddGrid), DataGridAllMods, AllModsGrid);
			CallDeferred(nameof(AddGrid), DataGridDownloads, DownloadedModsGrid);			
		}){IsBackground = true}.Start();
	}

	private void SetupDisplayPt2(){
		new Thread (() => {						
			if (Directory.GetFiles(modsfolder).Length != 0){
				new Thread(() => {
					readingpackages = true;
					ReadPackages();
				}){IsBackground = true}.Start();
			} else {
				AMLoaded();
			}
			if (Directory.GetFiles(downloadsfolder).Length != 0){
				new Thread(() => {
					readingdownloads = true;
					ReadDownloads();
				}){IsBackground = true}.Start();
			} else {
				DLLoaded();
			}		
	
			while (readingpackages || readingdownloads){
				//Thread.Sleep(1);
			}

			if (packages.Count != 0){
				populatingpackages = true;
				AllModsDisplayPackages();
			}

			if (downloads.Count != 0){
				populatingdownloads = true;			
				DownloadsDisplayPackages();
			}			
			
			while (populatingpackages || populatingdownloads){
				//Thread.Sleep(1);
			}
			CallDeferred(nameof(FinishSetupDisplay));
		}){IsBackground = true}.Start();
	}

    

    private void FinishSetupDisplay(){
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Finishing setting up the displays."));
		//DataGridAllMods.AddChild(AllModsGrid);
		//DataGridDownloads.AddChild(DownloadedModsGrid);
		DLGetHeaders();
		AMGetHeaders();
		canstart = true;
		//UIUtilities.UpdateTheme(GetTree());
		firstrundownloads = true;
		firstrunpackages = true;
		//EmitDoneLoading();
	}

	private void AllModsDisplayPackages(){
		populatingpackages = true;	

		List<List<string>> linkedfiles = packages.Where(x => x.LinkedFiles.Any()).Select(x => x.LinkedFiles).ToList();

		if (linkedfiles.Any()){
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("There are linked files to remove."));
			foreach (List<string> l in linkedfiles){
				foreach (string s in l){
					if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Removing {0}.", s));
					if (packages.Where(x => x.Location == s).Any()){
						int package = packages.IndexOf(packages.Where(x => x.Location == s).First());
						if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Found the package to remove: {0}.", package));
						packages.RemoveAt(package);
					}
					string info = string.Format("{0}.info", s);
					if (File.Exists(info)){
						File.Delete(info);
					}
				}
				
			}
		}


		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("There are {0} packages.", packages.Count));
		if (packages.Count != 0){				
			List<CellContent> griddata = new();
			int rowitem = 0;
			if (packages.Where(x => x.Selected).Count() != 0){
				List<SimsPackage> selected = packages.Where(x => x.Selected).ToList();
				foreach (SimsPackage p in selected){
					packages[packages.IndexOf(p)].Selected = false;
				}
			}			
			foreach (SimsPackage package in packages){
				if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Adding row {0}: {1}", rowitem, package.FileName));
				int columnnumber = 0;
				foreach (HeaderInformation inf in AllModsGrid.Headers){
					CellContent content = new();
					string data = "";
					Type type = typeof(string);
					if (inf.ContentType == DataGridContentType.Int){
						content = new CellContent(){
							RowNum = rowitem,
							ColumnNum = columnnumber,
							RowIdentifier = package.Identifier.ToString()
						};
						if (package.GetProperty(inf.ColumnData) != "0"){
							type = package.GetProperty(inf.ColumnData).GetType();
							data = package.GetProperty(inf.ColumnData).ToString();
						} else {
							type = package.GetProperty(inf.ColumnData).GetType();
							data = "";
						}
						content.CellType = CellOptions.Int;
						if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Row {0} has Load Order of: {1}", rowitem, data));
						content.Content = data;
						content.Selected = package.Selected;
					} else if (inf.ContentType == DataGridContentType.Icons){
						content = new CellContent(){
							RowNum = rowitem,
							ColumnNum = columnnumber,
							RowIdentifier = package.Identifier.ToString()
						};
						bool linked = false;
						if (package.LinkedFiles.Any()) linked = true;
						IconOptions icons = new()
						{
							Root = package.RootMod,
							OutOfDate = package.OutOfDate,
							Fave = package.Fave,
							WrongGame = package.WrongGame,
							Orphan = package.Orphan,
							Broken = package.Broken,
							Conflicts = package.Conflicts.Any(),
							Override = package.Override,
							Folder = package.Folder,
							LoadAsFolder = package.LoadAsFolder,
							LinkedFiles = linked,
							MiscFile = package.Misc
						};
						if (package.GetProperty(inf.ColumnData) != null){
							type = package.GetProperty(inf.ColumnData).GetType();
							data = package.GetProperty(inf.ColumnData).ToString();
						}
						content.Icons = true;
						content.IconOptions = icons;
						content.CellType = CellOptions.Icons;
						content.Content = data;
						content.Selected = package.Selected;
					} else {					
						if (package.GetProperty(inf.ColumnData) != null){
							type = package.GetProperty(inf.ColumnData).GetType();
							data = package.GetProperty(inf.ColumnData).ToString();
						}
						content = new CellContent(){
							RowNum = rowitem,
							ColumnNum = columnnumber,
							Content = data,
							RowIdentifier = package.Identifier.ToString()
						};
						if (type == typeof(string)){
							content.CellType = CellOptions.Text;
						} else if (type == typeof(bool)){
							content.CellType = CellOptions.TrueFalse;						
						} else {
							content.CellType = CellOptions.Text;
						}
						content.ContentType = inf.ColumnData;
					}
					package.Category ??= defaultcat;
					if (package.Category.Name != "Default"){
						content.BackgroundColor = package.Category.Background;
					}
					package.WriteInfoFile();
					griddata.Add(content);
					columnnumber++;
				}
				rowitem++;
			}
			AllModsGrid.Data = griddata;
			AllModsGrid.RowsFromData();
			_packages = packages;
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("_packages count: {0}, packages count: {1}.", _packages.Count, packages.Count));
		}		
		populatingpackages = false;
		_HidePbar();
	}

	private void DownloadsDisplayPackages(){
		populatingdownloads = true;
		if (downloads.Count != 0){				
			List<CellContent> griddata = new();
			int rowitem = 0;
			foreach (SimsDownload download in downloads){
				if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Adding row {0}: {1}", rowitem, download.FileName));
				int columnnumber = 0;
				foreach (HeaderInformation inf in DownloadedModsGrid.Headers){
					CellContent content = new();
					string data = "";
					Type type = typeof(string);
										
					if (download.GetProperty(inf.ColumnData) != null){
						type = download.GetProperty(inf.ColumnData).GetType();
						data = download.GetProperty(inf.ColumnData).ToString();
					}
					content = new CellContent(){
						RowNum = rowitem,
						ColumnNum = columnnumber,
						Content = data,
						RowIdentifier = download.Identifier.ToString()
					};
					if (type == typeof(string)){
						content.CellType = CellOptions.Text;
					} else if (type == typeof(bool)){
						content.CellType = CellOptions.TrueFalse;						
					} else {
						content.CellType = CellOptions.Text;
					}
					content.ContentType = inf.ColumnData;
					content.Selected = download.Selected;
					
					griddata.Add(content);
					columnnumber++;
				}
				rowitem++;
			}
			DownloadedModsGrid.Data = griddata;
			DownloadedModsGrid.RowsFromData();
			_downloads = downloads;
		}
		populatingdownloads = false;
	}

	private void DLHeaderSorted(int idx, SortingOptions sortingrule)
    {
        List<SimsDownload> sorted = new();
		string data = DownloadedModsGrid.Headers[idx].ColumnData;
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Packages are being sorted by {0} and are {1}.", data, sortingrule));
		if (DLSortBy == ""){
			DLSortingRule = SortingOptions.Ascending;
			DLSortBy = data;
		} else if (DLSortBy == data){
			if (DLSortingRule == SortingOptions.Ascending){
				DLSortingRule = SortingOptions.Descending;
			} else if (DLSortingRule == SortingOptions.Descending){
				DLSortingRule = SortingOptions.NotSorted;
			} else if (DLSortingRule == SortingOptions.NotSorted){
				DLSortingRule = SortingOptions.Ascending;
			}
		} else {
			DLSortingRule = SortingOptions.Ascending;
			DLSortBy = data;
		}
				
		for (int i = 0; i < DLHeaders.Count; i++){
			if (i != idx){
				(DLHeaders[i] as DataGridHeaderCell).ResetSorting();
			}
		}
		if (DLSortingRule == SortingOptions.Ascending){
			sorted = downloads.OrderBy(x => x.GetSortingProperty(DLSortBy)).ToList();		
		} else if (DLSortingRule == SortingOptions.Descending){			
			sorted = downloads.OrderByDescending(x => x.GetSortingProperty(DLSortBy)).ToList();
		} else if (DLSortingRule == SortingOptions.NotSorted){
			sorted = unsorteddownloads;
		}
		downloads = sorted;
		DownloadsChanged.Invoke();
    }


    private void AMHeaderSorted(int idx, SortingOptions sortingrule)
    {
        List<SimsPackage> sorted = new();
		string data = AllModsGrid.Headers[idx].ColumnData;
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Packages are being sorted by {0} and are {1}.", data, sortingrule));
		if (AMSortBy == ""){
			AMSortingRule = SortingOptions.Ascending;
			AMSortBy = data;
		} else if (AMSortBy == data){
			if (AMSortingRule == SortingOptions.Ascending){
				AMSortingRule = SortingOptions.Descending;
			} else if (AMSortingRule == SortingOptions.Descending){
				AMSortingRule = SortingOptions.NotSorted;
			} else if (AMSortingRule == SortingOptions.NotSorted){
				AMSortingRule = SortingOptions.Ascending;
			}
		} else {
			AMSortingRule = SortingOptions.Ascending;
			AMSortBy = data;
		}		
		for (int i = 0; i < AMHeaders.Count; i++){
			if (i != idx){
				(AMHeaders[i] as DataGridHeaderCell).ResetSorting();
			}
		}

		if (AMSortingRule == SortingOptions.Ascending){
			if (AMSortBy == "LoadOrder"){
				List<SimsPackage> enabled = packages.Where(x => x.Enabled == true).ToList();
				List<SimsPackage> disabled = packages.Where(x => x.Enabled == false).ToList();
				enabled = enabled.OrderBy(x => x.LoadOrder).ToList();
				sorted = new();
				sorted.AddRange(enabled);
				sorted.AddRange(disabled);
			} else if (AMSortBy == "Enabled") {
				sorted = packages.OrderByDescending(x => x.GetSortingProperty(AMSortBy)).ToList();
			} else {
				sorted = packages.OrderBy(x => x.GetSortingProperty(AMSortBy)).ToList();
			}			
		} else if (AMSortingRule == SortingOptions.Descending){
			if (AMSortBy == "LoadOrder"){
				List<SimsPackage> enabled = packages.Where(x => x.Enabled == true).ToList();
				List<SimsPackage> disabled = packages.Where(x => x.Enabled == false).ToList();
				enabled = enabled.OrderByDescending(x => x.LoadOrder).ToList();
				sorted = new();
				sorted.AddRange(enabled);
				sorted.AddRange(disabled);
			} else if (AMSortBy == "Enabled") {
				sorted = packages.OrderBy(x => x.GetSortingProperty(AMSortBy)).ToList();
			} else {
				sorted = packages.OrderByDescending(x => x.GetSortingProperty(AMSortBy)).ToList();
			}
		} else if (AMSortingRule == SortingOptions.NotSorted){
			sorted = unsortedpackages;
		}
		packages = sorted;
		PackagesChanged.Invoke();
    }

	private void LoadOrderItemChanged(string ident, int val)
    {
		Guid identifier = Guid.Parse(ident);
		List<SimsPackage> enabled = packages.Where(x => x.Enabled).OrderBy(x => x.LoadOrder).ToList();
		int idx = packages.IndexOf(packages.Where(x => x.Identifier == identifier).First());
		if (enabled.Where(x => x.LoadOrder == val).Any()){
			int dupe = packages.IndexOf(enabled.Where(x => x.LoadOrder == val).First());
			packages[dupe].LoadOrder = packages[idx].LoadOrder;
			packages[dupe].WriteInfoFile();
			AllModsGrid.UpdateLoadOrder(dupe, packages[dupe].LoadOrder);
		}
		packages[idx].LoadOrder = val;
		packages[idx].WriteInfoFile();
		AllModsGrid.UpdateLoadOrder(idx, packages[idx].LoadOrder);
    }

    private void AllModsItemEnabled(string item, int idx){
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Item {0} enabled at index {1}.", item, idx));
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Item is: {0}.", packages[idx].FileName));
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Item enabled... There are {0} items selected.", packages.Where(x => x.Selected).Count()));
		if (packages.Where(x => x.Selected).Count() > 1){
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Enabling {0} files.", packages.Where(x => x.Selected).Count()));
			List<SimsPackage> selected = packages.Where(x => x.Selected).ToList();			
			foreach (SimsPackage package in selected){
				idx = packages.IndexOf(package);
				int max = packages.Where(x => x.Enabled == true).Count();
				if (max > 0){
					package.LoadOrder = max+1;
				} else {
					package.LoadOrder = 1;
				}
				package.Enabled = true;
				(AllModsRows.GetChild(idx) as DataGridRow).LoadOrder = package.LoadOrder;
				(AllModsRows.GetChild(idx) as DataGridRow).ToggleEnabled(true);
				package.WriteInfoFile();
			}
		} else {			
			int max = packages.Where(x => x.Enabled == true).Count();
			if (max > 0){
				packages[idx].LoadOrder = max+1;
			} else {
				packages[idx].LoadOrder = 1;
			}
			packages[idx].Enabled = true;
			(AllModsRows.GetChild(idx) as DataGridRow).LoadOrder = packages[idx].LoadOrder;
			(AllModsRows.GetChild(idx) as DataGridRow).ToggleEnabled(true);
			packages[idx].WriteInfoFile();
		}
	}
	private void AllModsItemDisabled(string item, int idx){
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Item {0} disabled at index {1}.", item, idx));
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Item is: {0}.", packages[idx].FileName));
		if (packages.Where(x => x.Selected).Count() > 1){
			List<SimsPackage> selected = packages.Where(x => x.Selected).ToList();
			foreach (SimsPackage package in selected){
				idx = packages.IndexOf(package);
				package.LoadOrder = -1;
				(AllModsRows.GetChild(idx) as DataGridRow).LoadOrder = package.LoadOrder;
				(AllModsRows.GetChild(idx) as DataGridRow).ToggleEnabled(false);
				package.Enabled = false;
				package.WriteInfoFile();
			}			
		} else {
			packages[idx].LoadOrder = -1;
			(AllModsRows.GetChild(idx) as DataGridRow).LoadOrder = packages[idx].LoadOrder;
			(AllModsRows.GetChild(idx) as DataGridRow).ToggleEnabled(false);
			packages[idx].Enabled = false;
			packages[idx].WriteInfoFile();
		}
		List<SimsPackage> enabled = packages.Where(x => x.Enabled).OrderBy(x => x.LoadOrder).ToList();
		int lo = 1;
		foreach (SimsPackage package in enabled){
			idx = packages.IndexOf(package);
			packages[idx].LoadOrder = lo;
			(AllModsRows.GetChild(idx) as DataGridRow).LoadOrder = lo;
			(AllModsRows.GetChild(idx) as DataGridRow).ToggleEnabled(true);
			package.WriteInfoFile();
			lo++;
		}
	}

	private void AllModsItemSelected(string item, int idx){
		if (holdingctrl || holdingshift){
			if (holdingctrl){
				(AllModsRows.GetChild(idx) as DataGridRow).Selected = true;
				packages.Where(x => x.Identifier == Guid.Parse(item)).First().Selected = true;								
			} else if (holdingshift){
				AMShiftSelection(item, idx);
			}
		} else {
			for (int i = 0; i < AllModsRows.GetChildCount(); i++){
				DataGridRow row = AllModsRows.GetChild(i) as DataGridRow;
				if (row.Selected){
					packages[i].Selected = false;
					row.Selected = false;
				}
			}
			(AllModsRows.GetChild(idx) as DataGridRow).Selected = true;
			packages[idx].Selected = true;
		}
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("There are {0} packages selected", packages.Where(x => x.Selected == true).Count()));
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Selected packages:"));
		foreach (SimsPackage package in packages.Where(x => x.Selected == true)){
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(package.FileName);
		}
		amlastselected = idx;
	}
	private void AllModsItemUnselected(string item, int idx){		
		if (holdingctrl){		
			(AllModsRows.GetChild(idx) as DataGridRow).Selected = false;		
			packages[idx].Selected = false;			
		} else if (holdingshift) {
			AMShiftSelection(item, idx);
 		} else {
			if (packages.Where(x => x.Selected).ToList().Count >= 1){
				for (int i = 0; i < AllModsRows.GetChildCount(); i++){
					DataGridRow row = AllModsRows.GetChild(i) as DataGridRow;
					packages[i].Selected = false;
					row.Selected = false;
				}
				(AllModsRows.GetChild(idx) as DataGridRow).Selected = true;		
				packages[idx].Selected = true;
			} else {
				for (int i = 0; i < AllModsRows.GetChildCount(); i++){
					DataGridRow row = AllModsRows.GetChild(i) as DataGridRow;
					packages[i].Selected = false;
					row.Selected = false;
				}
				packages[idx].Selected = false;
			}			
		}
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("There are {0} packages selected", packages.Where(x => x.Selected == true).Count()));
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Selected packages:"));
		foreach (SimsPackage package in packages.Where(x => x.Selected == true)){
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(package.FileName);
		}
		amlastselected = idx;
	}

	private void AMShiftSelection(string item, int idx){
		if (packages.Where(x => x.Selected == true).Any()){
			int r_idx_last = packages.IndexOf(packages.Where(x => x.Selected).Last());
			int r_idx_first = packages.IndexOf(packages.Where(x => x.Selected).First());
			for (int i = 0; i < packages.Count; i++){
				packages[i].Selected = false;
				(AllModsRows.GetChild(i) as DataGridRow).Selected = false;
			}

			if (r_idx_first == r_idx_last){
				int both = r_idx_first;
				if (both > idx){
					for (int i = idx; i <= both; i++){
						packages[i].Selected = true;
						(AllModsRows.GetChild(i) as DataGridRow).Selected = true;
					}
				} else if (idx > both){
					for (int i = both; i <= idx; i++){
						packages[i].Selected = true;
						(AllModsRows.GetChild(i) as DataGridRow).Selected = true;
					}
				}
			} else if (amlastselected == r_idx_last){
				if (idx > r_idx_last){
					for (int i = r_idx_first; i <= idx; i++){
						packages[i].Selected = true;
						(AllModsRows.GetChild(i) as DataGridRow).Selected = true;
					}
				} else if (idx < r_idx_last && idx > r_idx_first){
					for (int i = r_idx_first; i <= idx; i++){
						packages[i].Selected = true;
						(AllModsRows.GetChild(i) as DataGridRow).Selected = true;
					}
				} else if (idx < r_idx_last && idx < r_idx_first){
					for (int i = idx; i <= r_idx_first; i++){
						packages[i].Selected = true;
						(AllModsRows.GetChild(i) as DataGridRow).Selected = true;
					}
				}
			} else if (amlastselected == r_idx_first){
				if (idx > r_idx_first){
					for (int i = r_idx_first; i <= idx; i++){
						packages[i].Selected = true;
						(AllModsRows.GetChild(i) as DataGridRow).Selected = true;
					}
				} else if (idx < r_idx_first){
					for (int i = idx; i <= r_idx_first; i++){
						packages[i].Selected = true;
						(AllModsRows.GetChild(i) as DataGridRow).Selected = true;
					}
				}
			}
			
			/*else if (idx > r_idx_last){
				for (int i = r_idx_first; i < idx; i++){
					packages[i-1].Selected = true;
					(AllModsRows.GetChild(i) as DataGridRow).Selected = true;
				}
			} else if (idx < r_idx_first){
				for (int i = idx; i < r_idx_last; i++){
					packages[i-1].Selected = true;
					(AllModsRows.GetChild(i) as DataGridRow).Selected = true;
				}
			}*/
		} else {
			(AllModsRows.GetChild(idx) as DataGridRow).Selected = true;
			packages[idx].Selected = true;				
		}
		amlastselected = idx;
	}

	private void DLShiftSelection(string item, int idx){
		if (downloads.Where(x => x.Selected == true).Any()){
			int r_idx_last = downloads.IndexOf(downloads.Where(x => x.Selected).Last());
			int r_idx_first = downloads.IndexOf(downloads.Where(x => x.Selected).First());
			for (int i = 0; i < downloads.Count; i++){
				downloads[i].Selected = false;
				(DownloadsRows.GetChild(i) as DataGridRow).Selected = false;
			}

			if (r_idx_first == r_idx_last){
				int both = r_idx_first;
				if (both > idx){
					for (int i = idx; i <= both; i++){
						downloads[i].Selected = true;
						(DownloadsRows.GetChild(i) as DataGridRow).Selected = true;
					}
				} else if (idx > both){
					for (int i = both; i <= idx; i++){
						downloads[i].Selected = true;
						(DownloadsRows.GetChild(i) as DataGridRow).Selected = true;
					}
				}
			} else if (amlastselected == r_idx_last){
				if (idx > r_idx_last){
					for (int i = r_idx_first; i <= idx; i++){
						downloads[i].Selected = true;
						(DownloadsRows.GetChild(i) as DataGridRow).Selected = true;
					}
				} else if (idx < r_idx_last && idx > r_idx_first){
					for (int i = r_idx_first; i <= idx; i++){
						downloads[i].Selected = true;
						(DownloadsRows.GetChild(i) as DataGridRow).Selected = true;
					}
				} else if (idx < r_idx_last && idx < r_idx_first){
					for (int i = idx; i <= r_idx_first; i++){
						downloads[i].Selected = true;
						(DownloadsRows.GetChild(i) as DataGridRow).Selected = true;
					}
				}
			} else if (amlastselected == r_idx_first){
				if (idx > r_idx_first){
					for (int i = r_idx_first; i <= idx; i++){
						downloads[i].Selected = true;
						(DownloadsRows.GetChild(i) as DataGridRow).Selected = true;
					}
				} else if (idx < r_idx_first){
					for (int i = idx; i <= r_idx_first; i++){
						downloads[i].Selected = true;
						(DownloadsRows.GetChild(i) as DataGridRow).Selected = true;
					}
				}
			}
			
			/*else if (idx > r_idx_last){
				for (int i = r_idx_first; i < idx; i++){
					downloads[i-1].Selected = true;
					(DownloadsRows.GetChild(i) as DataGridRow).Selected = true;
				}
			} else if (idx < r_idx_first){
				for (int i = idx; i < r_idx_last; i++){
					downloads[i-1].Selected = true;
					(DownloadsRows.GetChild(i) as DataGridRow).Selected = true;
				}
			}*/
		} else {
			(DownloadsRows.GetChild(idx) as DataGridRow).Selected = true;
			downloads[idx].Selected = true;				
		}
		dllastselected = idx;
	}

	private void DownloadsItemSelected(string item, int idx){
		if (holdingctrl || holdingshift){
			if (holdingctrl){
				(DownloadsRows.GetChild(idx) as DataGridRow).Selected = true;
				downloads.Where(x => x.Identifier == Guid.Parse(item)).First().Selected = true;				
			} else if (holdingshift){
				DLShiftSelection(item, idx);
			}
		} else {
			for (int i = 0; i < DownloadsRows.GetChildCount(); i++){
				DataGridRow row = DownloadsRows.GetChild(i) as DataGridRow;
				if (row.Selected){
					downloads[i].Selected = false;
					row.Selected = false;
				}
			}
			(DownloadsRows.GetChild(idx) as DataGridRow).Selected = true;
			downloads[idx].Selected = true;			
		}
		dllastselected = idx;
	}
	private void DownloadsItemUnselected(string item, int idx){
		if (holdingctrl){		
			(DownloadsRows.GetChild(idx) as DataGridRow).Selected = false;		
			downloads[idx].Selected = false;
		} else if (holdingshift) {
			DLShiftSelection(item, idx);
 		} else {
			if (downloads.Where(x => x.Selected).ToList().Count > 1){
				for (int i = 0; i < DownloadsRows.GetChildCount(); i++){
					DataGridRow row = DownloadsRows.GetChild(i) as DataGridRow;
					if (row.Selected){
						downloads[i].Selected = false;
						row.Selected = false;
					}
				}
				(DownloadsRows.GetChild(idx) as DataGridRow).Selected = true;		
				downloads[idx].Selected = true;
			} else {
				for (int i = 0; i < DownloadsRows.GetChildCount(); i++){
					DataGridRow row = DownloadsRows.GetChild(i) as DataGridRow;
					if (row.Selected){
						row.Selected = false;
					}
				}
				downloads[idx].Selected = false;
			}			
		}
		dllastselected = idx;
	}

	private void AMPackagesSearched(string search){
		List<SimsPackage> searchresults = new();
		List<Node> rows = AllModsRows.GetChildren().ToList();
		new Thread(() => {
			searchresults.AddRange(packages.Where(x => x.FileName.Contains(search)).ToList());
			searchresults.AddRange(packages.Where(x => x.Location.Contains(search)).ToList());
			searchresults.AddRange(packages.Where(x => x.FileType.ToString().Contains(search)).ToList());
			searchresults.AddRange(packages.Where(x => x.Notes.ToString().Contains(search)).ToList());
			searchresults.AddRange(packages.Where(x => x.DateAdded.ToString().Contains(search)).ToList());
			searchresults.AddRange(packages.Where(x => x.DateEnabled.ToString().Contains(search)).ToList());
			searchresults.AddRange(packages.Where(x => x.DateUpdated.ToString().Contains(search)).ToList());
			searchresults = searchresults.Distinct().ToList();
			List<int> idxs = new();
			foreach (SimsPackage result in searchresults){
				idxs.Add(packages.IndexOf(result));				
			}
			for (int i = 0; i > rows.Count; i++){
				if (!idxs.Contains(i)){
					(rows[i] as DataGridRow).Visible = false;
				}
			}
		}){IsBackground = true}.Start();
	}	
	
    private void AMCategorySelected(string catname, bool selected)
    {
		new Thread(() => {
			List<int> packageindexes = new();
			if (selected){
				if (!packages.Where(x => x.Selected == true).Any()){
					packages[rcpackage].Category = Categories.Where(x => x.Name == catname).First();
					packageindexes.Add(rcpackage);
				} else {
					List<SimsPackage> selecteditems = packages.Where(x => x.Selected == true).ToList();
					foreach (SimsPackage package in selecteditems){
						int idx = packages.IndexOf(package);
						packages[idx].Category = Categories.Where(x => x.Name == catname).First();
						packageindexes.Add(idx);
					}
				}
			} else {
				if (!packages.Where(x => x.Selected == true).Any()){
					packages[rcpackage].Category = Categories.Where(x => x.Name == "Default").First();
					packageindexes.Add(rcpackage);
				} else {
					List<SimsPackage> selecteditems = packages.Where(x => x.Selected == true).ToList();
					foreach (SimsPackage package in selecteditems){
						int idx = packages.IndexOf(package);
						packages[idx].Category = Categories.Where(x => x.Name == "Default").First();
						packageindexes.Add(idx);
					}
				}
			}	
		PackagesChanged.Invoke();
		}){IsBackground = true}.Start();		
    }

	private List<HeaderInformation> GetDefaultAMHeaders(){
		List<HeaderInformation> allmodsheaders = new()
        {
            new HeaderInformation()
            {
                HeaderTitle = "Enabled",
                ContentType = DataGridContentType.Enabled,
				Blank = true,
				Resizeable = false,
                ColumnData = "Enabled"
            },
            new HeaderInformation()
            {
                HeaderTitle = "Load Order",
                ContentType = DataGridContentType.Int,
                ColumnData = "LoadOrder",
				Resizeable = false,
				Blank = true
            },
            new HeaderInformation()
            {
                HeaderTitle = "Name",
                ContentType = DataGridContentType.Text,
                ColumnData = "FileName",
				Resizeable = true
            },
            new HeaderInformation()
            {
                HeaderTitle = "Icons",
                ContentType = DataGridContentType.Icons,
                ColumnData = "Icons",
				Blank = true,
				Resizeable = false
            },
            new HeaderInformation()
            {
                HeaderTitle = "Creator",
                ContentType = DataGridContentType.Text,
                ColumnData = "Creator",
				Resizeable = true
            },
            new HeaderInformation()
            {
                HeaderTitle = "Added",
                ContentType = DataGridContentType.Date,
                ColumnData = "DateAdded",
				Resizeable = true
            },
            new HeaderInformation()
            {
                HeaderTitle = "Size",
                ContentType = DataGridContentType.Text,
                ColumnData = "FileSize",
				Resizeable = true
            },
            new HeaderInformation()
            {
                HeaderTitle = "Updated",
                ContentType = DataGridContentType.Date,
                ColumnData = "DateUpdated",
				Resizeable = true
            },
            new HeaderInformation()
            {
                HeaderTitle = "Game",
                ContentType = DataGridContentType.Text,
                ColumnData = "Game",
				Resizeable = true
            },
            new HeaderInformation()
            {
                HeaderTitle = "Scanned",
                ContentType = DataGridContentType.Bool,
                ColumnData = "Scanned",
				Resizeable = true
            },
            new HeaderInformation()
            {
                HeaderTitle = "ScriptMod",
                ContentType = DataGridContentType.Bool,
                ColumnData = "ScriptMod",
				Resizeable = true
            },
            new HeaderInformation()
            {
                HeaderTitle = "File Type",
                ContentType = DataGridContentType.Text,
                ColumnData = "FileType",
				Resizeable = true
            }
        };
		return allmodsheaders;
	}
	
	private List<HeaderInformation> GetDefaultDownloadsHeaders(){
		List<HeaderInformation> downloadheaders = new()
        {
            new HeaderInformation()
            {
                HeaderTitle = "File Name",
                ContentType = DataGridContentType.Text,
                ColumnData = "FileName",
				Resizeable = true
            },
            new HeaderInformation()
            {
                HeaderTitle = "Location",
                ContentType = DataGridContentType.Text,
                ColumnData = "Location",
				Resizeable = true
            },
            new HeaderInformation()
            {
                HeaderTitle = "Added",
                ContentType = DataGridContentType.Date,
                ColumnData = "DateAdded",
				Resizeable = true
            },
            new HeaderInformation()
            {
                HeaderTitle = "Size",
                ContentType = DataGridContentType.Text,
                ColumnData = "FileSize",
				Resizeable = true
            },
            new HeaderInformation()
            {
                HeaderTitle = "Updated",
                ContentType = DataGridContentType.Date,
                ColumnData = "DateUpdated",
				Resizeable = true
            },
            new HeaderInformation()
            {
                HeaderTitle = "Installed",
                ContentType = DataGridContentType.Bool,
                ColumnData = "Installed",
				Resizeable = true
            }
        };
		return downloadheaders;
	}

	private void DLGetHeaders(){
		if (DownloadedModsGrid.headersready){
			DLHeaders = new();
			HBoxContainer headerrow = DownloadedModsGrid.GetNode<HBoxContainer>("VBoxContainer/HBoxContainer/MarginContainer/GridContainer/VBoxContainer/DataGrid_HeaderRow/DataGrid_HeaderRow/Row");
			foreach (DataGridHeaderCell cell in headerrow.GetChildren()){
				DLHeaders.Add(cell);
			}
		} else {
			DLGetHeaders();
		}	
	}

	private void AMGetHeaders(){
		if (AllModsGrid.headersready){
			AMHeaders = new();
			HBoxContainer headerrow = AllModsGrid.GetNode<HBoxContainer>("VBoxContainer/HBoxContainer/MarginContainer/GridContainer/VBoxContainer/DataGrid_HeaderRow/DataGrid_HeaderRow/Row");
			foreach (DataGridHeaderCell cell in headerrow.GetChildren()){
				AMHeaders.Add(cell);
			}
		} else {
			AMGetHeaders();
		}		
	}


	//event listeners

	private void ExeIconEvent(Texture2D texture, string exename, string exe)
    {
        GetNode<TextureRect>("MainWindowSizer/TopPanels/GameStartControls/HBoxContainer/ExeIcon_Container/HBoxContainer/MarginContainer/ExeIcon_Image").Texture = texture;
		GetNode<Label>("MainWindowSizer/TopPanels/GameStartControls/HBoxContainer/Name_Container/VBoxContainer/ExeName_Label").Text = exename;
		GetNode<Label>("MainWindowSizer/TopPanels/GameStartControls/HBoxContainer/Name_Container/VBoxContainer/ExeExe_Label").Text = exe;
    }

	private void OnPackagesListChanged(){
		if (firstrunpackages) AllModsDisplayPackages();
		new Thread(() => {
			Profile.Packages.Clear();
			List<SimsPackage> EnabledPackages = packages.Where(x => x.Enabled).ToList();
			foreach (SimsPackage package in EnabledPackages){
				Profile.Packages.Add(new(){ PackageFile = package.Location, LoadOrder = package.LoadOrder, Identifier = package.Identifier});
			}
		}){IsBackground = true}.Start();
	}

	private void OnDownloadsListChanged(){
		if (firstrunpackages) DownloadsDisplayPackages();
	}





	//event callers

	private void EmitDoneLoading(){
		DoneLoading.Invoke();
	}





	//ui handlers

	private void HidePackageDisplay(){
		packagedisplayvisible = false;
		GetNode<MarginContainer>("MainWindowSizer/MainPanels/HSplitContainer/VSplitContainer/PackageViewer_Frame_Container").Visible = packagedisplayvisible;
		if (CurrentPackageViewer != null) CurrentPackageViewer.QueueFree();
	}

	private void ShowPackageDisplay(){
		packagedisplayvisible = true;		
		GetNode<MarginContainer>("MainWindowSizer/MainPanels/HSplitContainer/VSplitContainer/PackageViewer_Frame_Container").Visible = packagedisplayvisible;
		MakePackageInformation();
	}

	private void _on_hide_popup_button_pressed(){
		ExeChoiceControl.Visible = false;
	}

	private void UpdateExes(){
		ExeChoicePanel.UpdateExes();
	}

	private bool AddGrid(Control box, CustomDataGrid grid){
		box.AddChild(grid);
		return true;
	}

	private void AMMouseEvent(bool inside, int idx){
		if (inside){
			mouseinAM = true;
			rcpackage = idx;
		} else {
			mouseinAM = false;
			rcpackage = -1;
		}
	}
	private void DLMouseEvent(bool inside, int idx){
		if (inside){
			mouseinDL = true;
			rcdownload = idx;
		} else {
			mouseinDL = false;
			rcdownload = -1;
		}
	}

	private void _on_exe_choice_popup_panel_picked_exe(string ExeID){
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Exe choice: {0}", ExeID));
		ExeChoiceControl.Visible = false;		
		foreach (Executable exe in Executables){
			if (exe.Selected == true){
				Executables[Executables.IndexOf(exe)].Selected = false;
			}
		}
		Executables[Executables.IndexOf(Executables.Where(x => x.Name == ExeID).First())].Selected = true;
	}

	

	private void _on_play_button_button_clicked(){
		InstantiateVirtualFiles();

		Executable selected = Executables.Where(x => x.Selected == true).First();
		string path = Path.Combine(selected.Path, selected.Exe);
		GetNode<MarginContainer>("GameRunning").Visible = true;
		string args = string.Format("{0} {1}", selected.Arguments, "-noOrigin");
		applicationStarter.Start(path, selected.Arguments, packages.Where(x => x.Enabled == true).ToList(), Game);
	}

	private void _on_disconnect_button_pressed(){
		GetNode<MarginContainer>("GameRunning").Visible = false;
		PurgeSymlinks();
	}

	private void _on_application_starter_application_closed(){
		GetNode<MarginContainer>("GameRunning").Visible = false;
		PurgeSymlinks();
	}

	private void _on_swap_instances_settings_help_buttons_button_clicked(){
		(GetParent() as MainWindow).ShowMainMenu();
		QueueFree();
	}

	private void _on_game_choice_dropdown_pressed(){
		ExeChoiceControl.Visible = true;
	}

	private void _on_package_scanner_package_scanned(string identifier){
		//SimsPackage package = packages.Where(x => x.Identifier == Guid.Parse(identifier)).First();
		//package.GetInfo(package.InfoFile);
		//ReadPackages();
	}

	private void _IncrementPbar(){
		IncrementPbar.Invoke();
		//EmitSignal("IncrementPbar");
	}

	private void _SetPbarMax(int max){
		SetPbarMax.Invoke(max);
		//EmitSignal("SetPbarMax, max");
	}

	private void _ResetPbarValue(){
		ResetPbarValue.Invoke();
		//EmitSignal("ResetPbarValue");
	}
	private void _HidePbar(){
		HidePbar.Invoke();
		//EmitSignal("HidePbar");
	}
	private void _ShowPbar(){
		ShowPbar.Invoke();
		//EmitSignal("ShowPbar");
	}

	public override void _Input(InputEvent @event)
    {
		if (@event.IsActionPressed("Shift")){
			holdingshift = true;
		}
		if (@event.IsActionReleased("Shift")){
			holdingshift = false;
		}
		if (@event.IsActionPressed("Ctrl")){
			holdingctrl = true;
		}
		if (@event.IsActionReleased("Ctrl")){
			holdingctrl = false;
		}
		if (@event.IsActionPressed("RightClick")){
			if (mouseinAM){
				AddAMRightClickMenu();			
			}
		}
    }



	//right click menu controls

	private void AddAMRightClickMenu(){		
		if (FloatingItemsContainer.GetChildCount() > 1) CurrentRightClickMenu.QueueFree();
		
		CurrentRightClickMenu = RightClickMenu.Instantiate() as RightClickMenu;
		Vector2 pos = GetViewport().GetMousePosition();	
		float y = AllModsGrid.Size.Y;
		float yhalf = y / 2;
		if (pos.Y > yhalf) {
			float perc = (pos.Y - yhalf) * 0.75f;
			pos = new(pos.X, pos.Y - perc);
		}

		CurrentRightClickMenu.Position = pos;
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Placing right click menu at {0}.", pos.ToString()));					
						

		CurrentRightClickMenu.RightClickMenuClicked += (item) => AMRCMenuPressed(item);
		List<SimsPackage> selected = packages.Where(x => x.Selected == true).ToList();
		
		if (selected.Where(x => x.OutOfDate).Any()){
			CurrentRightClickMenu.someoutofdate = true;
		}
		if (selected.Count > 0){
			CurrentRightClickMenu.multiplefiles = true;
		}
		//rcm.GetNode<Button>("MarginContainer/VBoxContainer/Linked/AddLinked_Button").Pressed += () => AMRCMenuPressed(0);			
		//rcm.GetNode<Button>("MarginContainer/VBoxContainer/MoveFile/MoveFile_Button").Pressed += () => AMRCMenuPressed(1);			
		//rcm.GetNode<Button>("MarginContainer/VBoxContainer/Delete/DeleteFile_Button").Pressed += () => AMRCMenuPressed(2);			
		//rcm.GetNode<Button>("MarginContainer/VBoxContainer/Rename/RenameFile_Button").Pressed += () => AMRCMenuPressed(3);			
		//rcm.GetNode<Button>("MarginContainer/VBoxContainer/FilesFromFolder/FilesFromFolder_Button").Pressed += () => AMRCMenuPressed(4);
		List<Category> categoriesforlist = Categories;
		if (packages.Where(x => x.Selected == true).Count() > 1){			
			List<Category> cats = selected.Select(x => x.Category).ToList();
			cats = cats.Distinct().ToList();
			if (cats.Count > 1){
				//more than one category								
				CurrentRightClickMenu.OpenCats.AddRange(cats.Select(x => x.Name));
			} else if (cats.Count == 1){
				if (selected.Where(x => x.Category == cats[0]).Count() != selected.Count){
					CurrentRightClickMenu.OpenCats.Add(cats[0].Name);
				} else if (selected.Where(x => x.Category == cats[0]).Count() == selected.Count){
					CurrentRightClickMenu.TickedCat = cats[0].Name;
				}
			}			
		}
		CurrentRightClickMenu.CategorySelectedMenu += (catname, selected) => AMCategorySelected(catname, selected);
		
		CurrentRightClickMenu.Categories = Categories;
		FloatingItemsContainer.AddChild(CurrentRightClickMenu);
		rightclickcatcher.Visible = true;
	}

	private void AMRCMenuPressed(int button)
    {
		switch (button){
			case 0: 
				//add linked
				EditLinked();
				break;
			case 1:
				RCM_FlipProperty("RootMod");
				break;
			case 2:
				RCM_FlipProperty("Fave");
				break;
			case 3:
				RCM_FlipProperty("OutOfDate");
				break;
			case 4:
				//files from folder
				break;
			case 5: 
				//rename
				RenameFiles();
				break;
			case 6: 
				//add creator
				break;
			case 7: 
				//add source link
				break;
			case 8:
				//movefile
				break;
			case 9:
				//delete file
				RemoveRightClickMenu();
				DeleteFilesPressed();
				break;
			case 10:
				RCM_FlipProperty("LoadAsFolder");
				break;
			case 11: 
				RCM_FlipProperty("WrongGame");
				break;
		}
		RemoveRightClickMenu();
    }

	private void EditLinked(){
		if (CurrentRightClickMenu.multiplefiles){
			LinkFiles();
		} else {
			EditLinkedFiles();
		}
	}

	private void LinkFiles(){
		LinkFilesWindow linkwindow = LinkFilesWindow.Instantiate() as LinkFilesWindow;
		linkwindow.FilesLinked += (linked, primary) => FilesLinked(linked, primary);
		List<string> files = packages.Where(x => x.Selected).Select(x => x.FileName).ToList();
		linkwindow.Files = files;
		AddChild(linkwindow);
	}

	private void FilesLinked(List<string> linked, string primary){
		canreadpackages = false;
		SimsPackage package = packages.Where(x => x.FileName == primary).First();
		
		foreach (string file in linked){					
			SimsPackage f = packages.Where(x => x.FileName == file).First();
			packages[packages.IndexOf(package)].LinkedFiles.Add(f.Location);	
			string info = string.Format("{0}.info", f.Location);
			if (f.LinkedFiles.Any()){
				packages[packages.IndexOf(package)].LinkedFiles.AddRange(f.LinkedFiles);
			}
			if (f.LinkedFolders.Any()){
				packages[packages.IndexOf(package)].LinkedFolders.AddRange(f.LinkedFolders);
			}
			packages.RemoveAt(packages.IndexOf(f));
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Removing info file if it exists: {0}", info));			
			if (File.Exists(info)){	
				if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Info file to be deleted: {0}", info));						
				File.Delete(info);
			}
			GlobalVariables.AddNoInfoFile(f.Location);
		}
		canreadpackages = true;
		packages[packages.IndexOf(package)].WriteInfoFile();
		packages[packages.IndexOf(package)].PackageChanged.Invoke();
	}

	private void EditLinkedFiles(){

	}

	private void RCM_FlipProperty(string property){
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Flipping property {0}.", property));
		bool flip = false;	
		Type t = typeof(SimsPackage);         
    	PropertyInfo prop = t.GetProperty(property);
		List<SimsPackage> selected = packages.Where(x => x.Selected == true).ToList();		
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("{0} packages selected.", selected.Count()));
		if (selected.Count == 0){
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Flipping last selected."));
			packages[rcpackage].Fave = !packages[rcpackage].Fave;
		} else {
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Multiple selected to flip!"));
			int tr = selected.Where(x => (bool)prop.GetValue(x) == true).Count();
			int fls = selected.Where(x => (bool)prop.GetValue(x) == false).Count();
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("{0} trues and {1} falses for property {2}", tr, fls, property));
			if (tr > fls){
				flip = false;
			} else if (tr < fls){
				flip = true;
			} else if (tr == fls){
				flip = false;
			}
			foreach (SimsPackage package in selected){
				packages[packages.IndexOf(package)].FlipBool(property, flip);
			}
		}
	}

	
	private void MakeFave(){
		bool Fave = false;
		if (packages.Where(x => x.Selected == true).Count() == 0){
			packages[rcpackage].Fave = !packages[rcpackage].Fave;
		} else {
			List<SimsPackage> selected = packages.Where(x => x.Selected == true).ToList();
			List<SimsPackage> selectedFave = selected.Where(x => x.Fave).ToList();
			if (selectedFave.Count() != selected.Count()){
				if (selectedFave.Count > selected.Count()){
					Fave = !selectedFave[0].Fave;
				} else {
					Fave = selectedFave[0].Fave;
				}
			} else {
				Fave = !selectedFave[0].Fave;
			}
			foreach (SimsPackage package in selected){
				packages[packages.IndexOf(package)].Fave = Fave;
			}
		}
		PackagesChanged.Invoke();
	}

	
	private void MakeLoadAsFolder(){
		bool laf = false;
		if (packages.Where(x => x.Selected == true).Count() == 0){
			packages[rcpackage].LoadAsFolder = !packages[rcpackage].LoadAsFolder;
		} else {
			List<SimsPackage> selected = packages.Where(x => x.Selected == true).ToList();
			List<SimsPackage> selectedlaf = selected.Where(x => x.LoadAsFolder).ToList();
			if (selectedlaf.Count() != selected.Count()){
				if (selectedlaf.Count > selected.Count()){
					laf = !selectedlaf[0].LoadAsFolder;
				} else {
					laf = selectedlaf[0].LoadAsFolder;
				}
			} else {
				laf = !selectedlaf[0].LoadAsFolder;
			}
			foreach (SimsPackage package in selected){
				packages[packages.IndexOf(package)].LoadAsFolder = laf;
			}
		}
		PackagesChanged.Invoke();
	}

	private void MakeRoot(){
		bool root = false;
		if (packages.Where(x => x.Selected == true).Count() == 0){
			packages[rcpackage].RootMod = !packages[rcpackage].RootMod;
		} else {
			List<SimsPackage> selected = packages.Where(x => x.Selected == true).ToList();
			List<SimsPackage> selectedroot = selected.Where(x => x.RootMod).ToList();
			if (selectedroot.Count() != selected.Count()){
				if (selectedroot.Count > selected.Count()){
					root = !selectedroot[0].RootMod;
				} else {
					root = selectedroot[0].RootMod;
				}
			} else {
				root = !selectedroot[0].RootMod;
			}
			foreach (SimsPackage package in selected){
				packages[packages.IndexOf(package)].RootMod = root;
			}			
		}
		PackagesChanged.Invoke();
	}
	
	private void MakeCorrectGame(){
		bool CorrectGame = false;
		if (packages.Where(x => x.Selected == true).Count() == 0){
			packages[rcpackage].WrongGame = !packages[rcpackage].WrongGame;
		} else {
			List<SimsPackage> selected = packages.Where(x => x.Selected == true).ToList();
			List<SimsPackage> selectedCorrectGame = selected.Where(x => x.WrongGame).ToList();
			if (selectedCorrectGame.Count() != selected.Count()){
				if (selectedCorrectGame.Count > selected.Count()){
					CorrectGame = !selectedCorrectGame[0].WrongGame;
				} else {
					CorrectGame = selectedCorrectGame[0].WrongGame;
				}
			} else {
				CorrectGame = !selectedCorrectGame[0].WrongGame;
			}
			foreach (SimsPackage package in selected){
				packages[packages.IndexOf(package)].WrongGame = CorrectGame;
			}
			PackagesChanged.Invoke();
		}
	}

	private void ToggleUpdated(){
		bool ood = false;
		if (packages.Where(x => x.Selected == true).Count() == 0){
			packages[rcpackage].OutOfDate = !packages[rcpackage].OutOfDate;
		} else {
			List<SimsPackage> selected = packages.Where(x => x.Selected == true).ToList();
			List<SimsPackage> selectedood = selected.Where(x => x.OutOfDate).ToList();
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Toggling out of date change.\n --- Selected items: {0}\n --- Selected Items which are out of date: {1}", selected.Count, selectedood.Count));
			
			if (selectedood.Count() != selected.Count()){
				if (selectedood.Count > selected.Count()){
					ood = !selectedood[0].OutOfDate;
				} else {
					ood = selectedood[0].OutOfDate;
				}
			} else {
				ood = !selectedood[0].OutOfDate;
			}
			foreach (SimsPackage package in selected){
				packages[packages.IndexOf(package)].OutOfDate = ood;
			}
			PackagesChanged.Invoke();
		}
	}
	
	private void DeleteFilesPressed(){
		VBoxContainer list = GetNode<VBoxContainer>("DeleteItemsBox/MarginContainer/VBoxContainer/ScrollContainer/VBoxContainer");
		List<SimsPackage> selected = packages.Where(x => x.Selected == true).ToList();
		int i = 0;
		foreach (SimsPackage package in selected){
			if (i == 0){
				(list.GetChild(0) as LineEdit).Text = package.FileName;
			} else {
				LineEdit newLE = (LineEdit)(list.GetChild(0) as LineEdit).Duplicate();
				newLE.Text = package.FileName;
				list.AddChild(newLE);
			}	
			i++;		
		}
		deletefiles.Visible = true;
	}

	private void DeleteFileDeletionFiles(){
		VBoxContainer list = GetNode<VBoxContainer>("DeleteItemsBox/MarginContainer/VBoxContainer/ScrollContainer/VBoxContainer");
		int i = 0;
		foreach (Node node in list.GetChildren()){
			if (i != 0){
				node.QueueFree();
			}
			i++;
		}		
	}

	private void ConfirmDeleteFilesButton(){
		List<SimsPackage> selected = packages.Where(x => x.Selected == true).ToList();
		foreach (SimsPackage package in selected){
			File.Delete(package.Location);
			File.Delete(package.InfoFile);
			packages.Remove(package);
		}
		deletefiles.Visible = false;
	}

	private void CancelDeleteFilesButton(){
		deletefiles.Visible = false;
		DeleteFileDeletionFiles();
	}

	private void RenameFiles(){		
		List<SimsPackage> selected = packages.Where(x => x.Selected == true).ToList();
		foreach (SimsPackage package in selected){
			PackageListItem pli = PackageListItem.Instantiate() as PackageListItem;
			pli.package = package;
			pli.packagename = package.FileName;
			packagelist.AddChild(pli);
		}
		renamefileswindow.Visible = true;		
	}

	private void GetAllPackageNames(){		
		foreach (PackageListItem package in packagelist.GetChildren()){
			package.GetPackageName();		
		}
	}

	private void Rename_RenamePressed(){
		foreach (PackageListItem package in packagelist.GetChildren()){
			FileInfo fi = new(package.package.Location);
			string ex = fi.Extension;
			string renamedpack = string.Format("{0}{1}", package.renamedname, ex);
			string renamedinfo = string.Format("{0}{1}", package.renamedname, ".info");
			string dir = fi.DirectoryName;
			string newpackloc = Path.Combine(dir, renamedpack);
			string newinfoloc = Path.Combine(dir, renamedinfo);
			File.Move(fi.FullName, newpackloc);
			File.Move(package.package.InfoFile, newinfoloc);
			packages[packages.IndexOf(package.package)].FileName = renamedpack;
			packages[packages.IndexOf(package.package)].Location = newpackloc;
			packages[packages.IndexOf(package.package)].InfoFile = newinfoloc;			
		}
		//AllModsDisplayPackages();
		
		renamefileswindow.Visible = false;
	}

	private void RemoveRenamePackageList(){
		foreach (PackageListItem package in packagelist.GetChildren()){
			QueueFree();
		}
	}

	private void Rename_CancelPressed(){
		renamefileswindow.Visible = false;
		RemoveRenamePackageList();
	}

    private void _on_right_click_catcher_gui_input(InputEvent @event){
		if (@event.IsActionPressed("LeftClick") || @event.IsActionPressed("RightClick") || @event.IsActionPressed("MiddleClick")){
			RemoveRightClickMenu();
		}
	}

	private void RemoveRightClickMenu(){
		CurrentRightClickMenu.RightClickMenuClicked -= AMRCMenuPressed;
		CurrentRightClickMenu.QueueFree();
		rightclickcatcher.Visible = false;
	}


	//profile handlers

	private void OnProfileChanged(){

	}



	//package error handlers

	private void BrokenPackagesDetected(){
		ErrorsListing.Visible = false;
		brokenmodsvisible = true;
		brokenfiles.Visible = true;		
	}
	private void WrongGameDetected(){
		ErrorsListing.Visible = false;
		wronggamefilesvisible = true;
		if (LoadedSettings.SetSettings.Instances.Where(x => x.Game == "Sims2").Any()){
			s2moveloc = LoadedSettings.SetSettings.Instances.Where(x => x.Game == "Sims2").First().InstanceLocation;
			wronggame_sims2.GetNode<LineEdit>("Sims2Wrong_Location_LineEdit").Text = s2moveloc;
		} else {
			s2moveloc = Path.Combine(wronggamefolder, "Sims 2");
			wronggame_sims2.GetNode<LineEdit>("Sims2Wrong_Location_LineEdit").Text = s2moveloc;
		}
		if (LoadedSettings.SetSettings.Instances.Where(x => x.Game == "Sims3").Any()){
			s3moveloc = LoadedSettings.SetSettings.Instances.Where(x => x.Game == "Sims3").First().InstanceLocation;
			wronggame_sims3.GetNode<LineEdit>("Sims3Wrong_Location_LineEdit").Text = s3moveloc;
		} else {
			s3moveloc = Path.Combine(wronggamefolder, "Sims 3");
			wronggame_sims3.GetNode<LineEdit>("Sims3Wrong_Location_LineEdit").Text = s3moveloc;
		}
		if (LoadedSettings.SetSettings.Instances.Where(x => x.Game == "Sims4").Any()){
			s4moveloc = LoadedSettings.SetSettings.Instances.Where(x => x.Game == "Sims4").First().InstanceLocation;
			wronggame_sims4.GetNode<LineEdit>("Sims4Wrong_Location_LineEdit").Text = s4moveloc;
		} else {
			s4moveloc = Path.Combine(wronggamefolder, "Sims 4");
			wronggame_sims4.GetNode<LineEdit>("Sims4Wrong_Location_LineEdit").Text = s4moveloc;
		}
		GetNode<LineEdit>("MoveWrongGameFiles/MarginContainer/VBoxContainer/ScrollContainer/VBoxContainer/OtherWrong_Location/OtherWrong_Location_LineEdit").Text = wronggamefolder;
				
		if (Game == Games.Sims2){
			wronggame_sims2.Visible = false;			
		} else if (Game == Games.Sims3){
			wronggame_sims3.Visible = false;
		} else if (Game == Games.Sims4){
			wronggame_sims4.Visible = false;
		}
		
				
		wronggamefiles.Visible = true;
	}

	private void ConfirmMoveIncorrectFiles(){
		wronggamefiles.Visible = false;
		List<SimsPackage> incorrect = packages.Where(x => x.WrongGame == true).ToList();
		foreach (SimsPackage package in incorrect){
			string movefolder = wronggamefolder;
			FileInfo fi = new(package.Location);
			if (package.Game == Games.Sims2) movefolder = s2moveloc;
			if (package.Game == Games.Sims3) movefolder = s3moveloc;
			if (package.Game == Games.Sims4) movefolder = s4moveloc;
			string newpckloc = Path.Combine(movefolder, fi.Name);
			string newinfoloc = string.Format("{0}.info", newpckloc);
			if (File.Exists(newpckloc)){
				newpckloc = newpckloc.Replace(fi.Name, string.Format("{0} (0)", fi.Name));
			}
			if (File.Exists(newinfoloc)){
				newinfoloc = newinfoloc.Replace(fi.Name, string.Format("{0} (0)", fi.Name));
			}
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Moving {0} to {1}", fi.FullName, newpckloc));
			File.Move(package.Location, newpckloc);
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Moving {0} to {1}", package.InfoFile, newinfoloc));
			File.Move(package.InfoFile, newinfoloc);
			if (File.Exists(package.InfoFile)) File.Delete(package.InfoFile);
			packages.Remove(package);
		}
		wronggamefilesvisible = false;
	}

	private void CancelMoveIncorrectFiles(){
		wronggamefiles.Visible = false;
		wronggamefilesvisible = false;
		dontmovemywrongfiles = true;
	}

	private void ConfirmMoveBrokenFiles(){
		brokenfiles.Visible = false;
		List<SimsPackage> brokenpackages = packages.Where(x => x.Broken == true).ToList();
		foreach (SimsPackage package in brokenpackages){
			FileInfo fi = new(package.Location);
			string newloc = Path.Combine(brokenmodsfolder, fi.Name);
			File.Move(package.Location, newloc);
			File.Delete(package.InfoFile);
			packages.Remove(package);
		}
		brokenmodsvisible = false;
	}

	private void CancelMoveBrokenFiles(){
		brokenfiles.Visible = false;
		brokenmodsvisible = false;
		dontmovemybrokenfiles = true;
	}

    private void WrongGameSims2Loc_TxtChanged(string text)
    {
        s2moveloc = text;
    }
    private void WrongGameSims3Loc_TxtChanged(string text)
    {
        s3moveloc = text;
    }
    private void WrongGameSims4Loc_TxtChanged(string text)
    {
        s4moveloc = text;
    }
    private void WrongGameOtherLoc_TxtChanged(string text)
    {
        othergamesmoveloc = text;
    }

	private void ViewErrors(){
		ErrorsListing.Visible = true;
		Label errorslabel = GetNode<Label>("ViewErrors/MarginContainer/VBoxContainer/ScrollContainer/MarginContainer/Errors_Label");
		errorslabel.Text = errorslist.ToString();
	}

	private void CancelViewErrors(){
		ErrorsListing.Visible = false;
		GetNode<Label>("ViewErrors/MarginContainer/VBoxContainer/ScrollContainer/MarginContainer/Errors_Label").Text = "";
	}



	//button handlers

	private void ProfilesButtonClicked(){

	}

	private void AddFilesButtonClicked(){
		AddFilesDiag.Visible = true;
	}
	private void AddFilesSelected(string[] files){
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("{0} files selected to move.", files.Count()));
		Parallel.ForEach (files, new ParallelOptions { MaxDegreeOfParallelism = paralellism }, file => {
			FileInfo fileInfo = new(file);
			string name = fileInfo.Name;
			string newloc = Path.Combine(modsfolder, name);
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Moving {0} to {1}", file, newloc));
			if (File.Exists(newloc)){
				string move = Path.Combine(instancefolder, "Old Packages");
				Directory.CreateDirectory(move);
				move = Path.Combine(move, name);
				File.Move(newloc, move);
			}		
			File.Move(file, newloc);	
		});
	}
	private void AddFolderSelected(string dir){
		DirectoryInfo directoryInfo = new(dir);
		string newloc = Path.Combine(modsfolder, directoryInfo.Name);
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Moving {0} to {1}", dir, newloc));
		if (File.Exists(newloc)){
			string move = Path.Combine(instancefolder, "Old Packages");
			move = Path.Combine(move, directoryInfo.Name);
			File.Move(newloc, move);
		}
		Directory.Move(dir, newloc);
	}

	private void AddFolderButtonClicked(){
		AddFolderDiag.Visible = true;
	}
	private void RefreshPackagesButtonClicked(){

	}

	private void SortIntoSubfoldersButtonClicked(){

	}
	private void ScanAllButtonClicked(){

	}
	private void ManageCategoriesClicked(){

	}
	private void EditExesClicked(){

	}
	
	

	//game running handling

	private void InstantiateVirtualFiles(){
		if (Game == Games.Sims2) gamemodsfolder = sims2Instance.DownloadsFolder;
		if (Game == Games.Sims3) gamemodsfolder = sims3Instance.ModsFolder;
		if (Game == Games.Sims4) gamemodsfolder = sims4Instance.ModsFolder;
		List<SimsPackage> enabled = packages.Where(x => x.Enabled).ToList();
		foreach (SimsPackage package in enabled){
			if (package.Folder && package.LoadAsFolder){
				VFileSystem.MakeJunction(package.Location, gamemodsfolder);
			} else
			{
				if (package.LinkedFiles.Any()){
					foreach (string file in package.LinkedFiles){
						VF_ProcessFile(file, package.LoadOrder);
					}
				}
				if (package.LinkedFolders.Any()){
					foreach (SimsPackageSubfolder folder in package.LinkedFolders){
						VF_ProcessFolder(package.LoadOrder, folder);
					}
				}
				VF_ProcessPackage(package);	
			} 		
		}
	}

	private void VF_ProcessFolder(int loadorder, SimsPackageSubfolder folder){
		if (folder.Subfiles.Any()){
			foreach (string file in folder.Subfiles){
				VF_LinkFile(loadorder, new FileInfo(file));
			}
		}
		if (folder.Subfolders.Any()){
			foreach (SimsPackageSubfolder f in folder.Subfolders){
				VF_ProcessFolder(loadorder, f);
			}			
		}
	}

	private void VF_ProcessPackage(SimsPackage package){
		if (!package.Folder) {
			VF_LinkFile(package);
		}
	}
	private void VF_ProcessFile(string file, int loadorder){		
		VF_LinkFile(loadorder, new FileInfo(file));
	}

	private void VF_LinkFile(SimsPackage package){
		string leader = "000000";
		string newname = string.Format("{0}{1}_{2}", leader, package.LoadOrder, package.FileName);
		VFileSystem.MakeSymbolicLink(package.Location, gamemodsfolder, newname);
	}

	private void VF_LinkFile(int loadorder, FileInfo file){
		string leader = "000000";
		string newname = string.Format("{0}{1}_{2}", leader, loadorder, file.Name);
		VFileSystem.MakeSymbolicLink(file.FullName, gamemodsfolder, newname);
	}

	private void VF_LinkFile(FileInfo file){
		VFileSystem.MakeSymbolicLink(file.FullName, gamemodsfolder);
	}

	private void PurgeSymlinks(){
		List<string> ModFiles = Directory.GetFiles(gamemodsfolder).ToList();
		foreach (string file in ModFiles){
			FileInfo fileInfo = new(file);
			FileAttributes attr = fileInfo.Attributes;
			if(attr.HasFlag(FileAttributes.ReparsePoint)){
				VFileSystem.RemoveSymbolicLink(file);
			} else if (fileInfo.Extension == ".disabled"){
				string ren = file.Replace(".disabled", "");
				File.Copy(file, ren);
			}
		}	
		List<string> ModFolders = Directory.GetDirectories(gamemodsfolder).ToList();
		foreach (string dir in ModFolders){
			DirectoryInfo info = new(dir);
			FileAttributes attr = info.Attributes;
			if(attr.HasFlag(FileAttributes.ReparsePoint)){
				VFileSystem.RemoveJunction(dir);
			} else if (info.FullName.EndsWith("--DISABLED")){
				string ren = dir.Replace("--DISABLED", "");
				Directory.Move(dir, ren);
			}
		}		
	}








}
