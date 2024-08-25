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
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public partial class PackageDisplay : MarginContainer
{
	public delegate void SetPbarMaxEvent(int value);
	public SetPbarMaxEvent SetPbarMax;
	
	public delegate void IncrementPbarEvent();
	public IncrementPbarEvent IncrementPbar;
	
	public delegate void ResetPbarValueEvent();
	public ResetPbarValueEvent ResetPbarValue;
	public delegate void ShowPbarEvent();
	public ShowPbarEvent ShowPbar;
	public delegate void HidePbarEvent();
	public HidePbarEvent HidePbar;
	private ExeChoicePopupPanel ExeChoicePanel;

	public delegate void DoneLoadingEvent();
	public DoneLoadingEvent DoneLoading;
	//PackedScene DataGridRow = GD.Load<PackedScene>("res://UI/PackageDisplay_Elements/data_grid_row.tscn");
	Control DataGridAllMods; 
	Control DataGridDownloads;
	PackedScene DataGrid = GD.Load<PackedScene>("res://UI/CustomDataGrid/CustomDataGrid.tscn");
	PackedScene PackageInformation = GD.Load<PackedScene>("res://UI/PackageDisplay_Elements/package_viewer.tscn");
	PackedScene RightClickMenu = GD.Load<PackedScene>("res://UI/PackageDisplay_Elements/right_click_menu.tscn");
	PackedScene PackageListItem = GD.Load<PackedScene>("res://UI/PackageDisplay_Elements/package_list_item.tscn");
	public Instance ThisInstance = new();
	Games Game = Games.Null;
	Sims2Instance sims2Instance = new();
	Sims3Instance sims3Instance = new();
	Sims4Instance sims4Instance = new();
	List<Executable> Executables = new();
	List<string> Cachefiles = new();
	List<string> Thumbs = new();
	List<Category> Categories = new();
	List<string> Profiles = new();
	string instancedatafolder = "";
	string instancefolder = "";
	Control ExeChoiceControl;
	List<SimsPackage> packages = new();
	List<SimsPackage> _packages = new();
	List<SimsDownload> downloads = new();
	List<SimsDownload> _downloads = new();
	CustomDataGrid AllModsGrid = new();
	CustomDataGrid DownloadedModsGrid = new();
	string downloadsfolder = "";
	string modsfolder = "";
	bool readingpackages = false;
	bool readingdownloads = false;
	bool populatingpackages = false;
	bool populatingdownloads = false;
	bool packagessortingorderchanged = false;
	bool downloadssortingorderchanged = false;
	string packagessortingorder = "";
	string downloadssortingorder = "";
	bool firstrunpackages = false;
	bool firstrundownloads = false;

	bool holdingshift = false;
	bool holdingctrl = false;

	VBoxContainer AllModsRows;
	VBoxContainer DownloadsRows;

	int amlastselected = -1;
	int dllastselected = -1;

	ApplicationStarter applicationStarter;

	bool packagedisplayvisible = false;
	PackageScanner scanner;
	SortingOptions AMSortingRule = SortingOptions.NotSorted;
	string AMSortBy = "";
	SortingOptions DLSortingRule = SortingOptions.NotSorted;
	string DLSortBy = "";
	List<SimsPackage> unsortedpackages = new();
	List<SimsDownload> unsorteddownloads = new();
	List<DataGridHeaderCell> DLHeaders = new();
	List<DataGridHeaderCell> AMHeaders = new();
	int filesinpackagesfolder = 0;
	int filesindownloadsfolder = 0;
	PackageViewer CurrentPackageViewer;
	int packagedisplayed = -1;
	Control FloatingItemsContainer;
	RightClickMenu rightclickmenu;
	MarginContainer rightclickcatcher;
	bool mouseinAM = false;
	bool mouseinDL = false;
	int rcpackage = -1;
	int rcdownload = -1;
	MarginContainer renamefileswindow;
	VBoxContainer packagelist;
	MarginContainer deletefiles;
	bool canstart = false;
	ExeChoicePopupPanel exeChoicePopupPanel;
	int paralellism = 0;
	LineEdit AMSearch;
	LineEdit DLSearch;
	List<string> ScannedPackages = new();
	MarginContainer brokenfiles;
	MarginContainer wronggamefiles;
	HBoxContainer wronggame_sims2;
	HBoxContainer wronggame_sims3;
	HBoxContainer wronggame_sims4;

	string s2moveloc = "";
	string s3moveloc = "";
	string s4moveloc = "";
	string othergamesmoveloc = "";
	string brokenmodsfolder = "";
	string wronggamefolder = "";

	bool wronggamefilesvisible = false;
	bool brokenmodsvisible = false;

	bool dontmovemywrongfiles = false;
	bool dontmovemybrokenfiles = false;
	ViewErrors_Container viewerrorscontainer;
	StringBuilder errorslist;
	int NumErrors = 0;
	int _numerrors = 0;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ThreadPool.GetMaxThreads(out int workerThreadsCount, out int ioThreadsCount);
		if (LoadedSettings.SetSettings.LimitCPU){
			paralellism = (ioThreadsCount - 2) / 2;
		} else {
			paralellism = ioThreadsCount - 2;
		}
		viewerrorscontainer = GetNode<ViewErrors_Container>("MainWindowSizer/TopPanels/SettingsAndHelpControls/HBoxContainer/ViewErrors_Container");
		viewerrorscontainer.GetNode<topbar_button>("View Errors_SettingsHelpButtons").ButtonClicked += () => ViewErrors();

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
		DataGridAllMods = GetNode<Control>("MainWindowSizer/MainPanels/HSplitContainer/AllMods_Frame_Container/AllMods_Container/GridContainer");
		DataGridDownloads = GetNode<Control>("MainWindowSizer/MainPanels/HSplitContainer/VSplitContainer/NewDownloads_Frame_Container/NewDL_Container/GridContainer");
		
		new Thread (() => {
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog("Starting Package Display.");
			if (ThisInstance.Game == "Sims2") {
				if (GlobalVariables.DebugMode) Logging.WriteDebugLog("Sims 2!");
				Game = Games.Sims2;
				string path = Path.Combine(ThisInstance.InstanceLocation, "Instance.ini");
				if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Loading from path {0}.", path));
				sims2Instance.Load(path);
				Executables = sims2Instance.Executables;
				Thumbs = sims2Instance.ThumbnailsFiles;
				Categories = sims2Instance.Categories;
				Profiles = sims2Instance.Profiles;
				instancedatafolder = sims2Instance.InstanceDataFolder;
				modsfolder = sims2Instance.InstancePackagesFolder;
				downloadsfolder = sims2Instance.InstanceDownloadsFolder;
				instancefolder = sims2Instance.InstanceFolder;
			} else if (ThisInstance.Game == "Sims3"){
				if (GlobalVariables.DebugMode) Logging.WriteDebugLog("Sims 3!");
				Game = Games.Sims3;
				sims3Instance.Load(Path.Combine(ThisInstance.InstanceLocation, "Instance.ini"));
				Executables = sims3Instance.Executables;
				Thumbs = sims3Instance.ThumbnailsFiles;
				Categories = sims3Instance.Categories;
				Profiles = sims3Instance.Profiles;
				instancedatafolder = sims3Instance.InstanceDataFolder;
				if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Instance folder: {0}", instancedatafolder));
				modsfolder = sims3Instance.InstancePackagesFolder;
				downloadsfolder = sims3Instance.InstanceDownloadsFolder;		
				instancefolder = sims3Instance.InstanceFolder;
			} else if (ThisInstance.Game == "Sims4"){
				if (GlobalVariables.DebugMode) Logging.WriteDebugLog("Sims 4!");
				Game = Games.Sims4;
				sims4Instance.Load(Path.Combine(ThisInstance.InstanceLocation, "Instance.ini"));
				Executables = sims4Instance.Executables;
				Thumbs = sims4Instance.ThumbnailsFiles;
				Categories = sims4Instance.Categories;
				Profiles = sims4Instance.Profiles;
				instancedatafolder = sims4Instance.InstanceDataFolder;
				if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Instance folder: {0}", instancedatafolder));
				modsfolder = sims4Instance.InstancePackagesFolder;
				downloadsfolder = sims4Instance.InstanceDownloadsFolder;	
				instancefolder = sims4Instance.InstanceFolder;	
			}
			brokenmodsfolder = Path.Combine(instancefolder, "Broken Packages");
			wronggamefolder = Path.Combine(instancefolder, "Incorrect Game Files");
			SetupDisplay();
			UpdateExecutableList();			
		}){IsBackground = true}.Start();
		
	}


    private void ExeIconEvent(Texture2D texture, string exename, string exe)
    {
        GetNode<TextureRect>("MainWindowSizer/TopPanels/GameStartControls/HBoxContainer/ExeIcon_Container/HBoxContainer/MarginContainer/ExeIcon_Image").Texture = texture;
		GetNode<Label>("MainWindowSizer/TopPanels/GameStartControls/HBoxContainer/Name_Container/VBoxContainer/ExeName_Label").Text = exename;
		GetNode<Label>("MainWindowSizer/TopPanels/GameStartControls/HBoxContainer/Name_Container/VBoxContainer/ExeExe_Label").Text = exe;
    }

    private void UpdateExecutableList(){
		ExeChoicePanel.instancedatafolder = instancedatafolder;
		ExeChoicePanel.Executables = Executables;
		CallDeferred(nameof(UpdateExes));
	}

	private void UpdateExes(){
		ExeChoicePanel.UpdateExes();
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

	private void PackageScanned(string package){
		ScannedPackages.Add(package);
	}

	private SimsPackageSubfolder GetFolderInfo(string folder){
		SimsPackageSubfolder subfolder = new();
		DirectoryInfo f = new(folder);
		List<string> files = Directory.GetFiles(folder).ToList();
		subfolder.Subfiles.AddRange(files);
		List<string> folders = Directory.GetDirectories(folder).ToList();
		if (folders.Count != 0){
			foreach (string fold in folders){
				subfolder.Subfolders.Add(GetFolderInfo(fold));
			}
		}
		return subfolder;
	}

	public bool ReadPackages(){
		readingpackages = true;
		ConcurrentBag<SimsPackage> packagesbag = new();
		List<string> files = Directory.GetFiles(modsfolder).ToList();
		List<string> directories = Directory.GetDirectories(modsfolder).ToList();
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("There are {0} files to read in Packages", files.Count));
		List<string> infofiles = files.Where(x => x.EndsWith(".info", StringComparison.OrdinalIgnoreCase)).ToList();
		List<string> packagefiles = files.Where(x => !infofiles.Contains(x)).ToList();
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("There are {0} files to read that are not Info Files", packagefiles.Count));

		if (directories.Count != 0){
			Parallel.ForEach(directories, new ParallelOptions { MaxDegreeOfParallelism = paralellism }, file => {
				SimsPackage simsPackage = new();				
				bool infoexists = simsPackage.GetInfo(file, true);	
				simsPackage.Folder = true;
				List<string> filesinfolder = Directory.GetFiles(simsPackage.Location).ToList();
				simsPackage.LinkedFiles.AddRange(filesinfolder);
				List<string> foldersinfolder = Directory.GetDirectories(simsPackage.Location).ToList();
				if (foldersinfolder.Count != 0){
					foreach (string folder in foldersinfolder){
						simsPackage.LinkedFolders.Add(GetFolderInfo(folder));						
					}
				}
			});
		}

		if (packagefiles.Count != 0){			
			Parallel.ForEach(packagefiles, new ParallelOptions { MaxDegreeOfParallelism = paralellism }, file => {
				SimsPackage simsPackage = new();				
				bool infoexists = simsPackage.GetInfo(file);				
				if (infoexists){
					simsPackage = Utilities.LoadPackageFile(simsPackage);
				} else {
					DateTime now = DateTime.Now;
					if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Going to scan {0}.", simsPackage.Location));					
					scanner.Scan(simsPackage);
					while (!ScannedPackages.Contains(simsPackage.Identifier.ToString())){
						//wait.
					}
					if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Looks like {0} finished! \n             Started: {1}\n             Finished: {2}.", simsPackage.Location, now, DateTime.Now));
					if (infoexists){
						simsPackage = Utilities.LoadPackageFile(simsPackage);
					}
				}
				simsPackage.Category ??= Categories.Where(x => x.Name == "Default").First();
				if (simsPackage.Game != Game){
					simsPackage.WrongGame = true;
				}
				if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("There are {0} packages in the bag.", packagesbag.Count));
				if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("This package ({0}) is for Sims {1}.", simsPackage.FileName, simsPackage.Game));

				packagesbag.Add(simsPackage);				
			});
			packages = packagesbag.ToList();
			unsortedpackages = packages;
			if (!packagessortingorderchanged) packages = packages.OrderBy(x => x.FileName).ToList();
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("There are {0} packages in packages.", packages.Count));
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("There are {0} ordered packages in packages.", packages.Count));
		}
		filesinpackagesfolder = Directory.GetFiles(modsfolder).Count() + Directory.GetDirectories(modsfolder).Count();
		if (firstrunpackages) AllModsDisplayPackages();
		readingpackages = false;
		return true;
	}

	public bool ReadDownloads(){
		readingdownloads = true;
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
		if (firstrundownloads) DownloadsDisplayPackages();
		return true;
	}

	private void _on_hide_popup_button_pressed(){
		ExeChoiceControl.Visible = false;
	}

	private void SetupDisplay(){		
		new Thread (() => {
			AllModsGrid = DataGrid.Instantiate() as CustomDataGrid;	
			AllModsGrid.Headers = GetDefaultAMHeaders();
			DownloadedModsGrid = DataGrid.Instantiate() as CustomDataGrid;	
			DownloadedModsGrid.Headers = GetDefaultDownloadsHeaders();
			bool agh = (bool)CallDeferred(nameof(AddGrid), DataGridAllMods, AllModsGrid);
			agh = (bool)CallDeferred(nameof(AddGrid), DataGridDownloads, DownloadedModsGrid);
		
			AllModsGrid.SelectedItem += (item, idx) => AllModsItemSelected(item, idx);
			AllModsGrid.UnselectedItem += (item, idx) => AllModsItemUnselected(item, idx);
			AllModsGrid.EnabledItem += (item, idx) => AllModsItemEnabled(item, idx);
			AllModsGrid.DisabledItem += (item, idx) => AllModsItemDisabled(item, idx);		
			AllModsGrid.HeaderSortedSignal += (idx, sortingrule) => AMHeaderSorted(idx, sortingrule);
			//AllModsGrid = DataGridAllMods.GetChild(0) as CustomDataGrid;
			AllModsRows = AllModsGrid.FindChild("DataGrid_Rows") as VBoxContainer;
			AllModsGrid.MouseAffectingGrid += (inside, idx) => AMMouseEvent(inside, idx);
			DownloadedModsGrid.SelectedItem += (item, idx) => DownloadsItemSelected(item, idx);
			DownloadedModsGrid.UnselectedItem += (item, idx) => DownloadsItemUnselected(item, idx);
			DownloadedModsGrid.HeaderSortedSignal += (idx, sortingrule) => DLHeaderSorted(idx, sortingrule);
			DownloadsRows = DownloadedModsGrid.GetNode<VBoxContainer>("VBoxContainer/RowsScroll/DataGrid_Rows");
			DownloadedModsGrid.MouseAffectingGrid += (inside, idx) => DLMouseEvent(inside, idx);					
			new Thread(() => {
				readingpackages = true;
				ReadPackages();
			}){IsBackground = true}.Start();
			new Thread(() => {
				readingdownloads = true;
				ReadDownloads();
			}){IsBackground = true}.Start();
	
			while (readingpackages || readingdownloads){
				//Thread.Sleep(1);
			}
			populatingpackages = true;
			populatingdownloads = true;
			AllModsDisplayPackages();
			DownloadsDisplayPackages();
			while (populatingpackages || populatingdownloads){
				//Thread.Sleep(1);
			}
			CallDeferred(nameof(FinishSetupDisplay));
		}){IsBackground = true}.Start();
	}

	private void FinishSetupDisplay(){
		//DataGridAllMods.AddChild(AllModsGrid);
		//DataGridDownloads.AddChild(DownloadedModsGrid);
		DLGetHeaders();
		AMGetHeaders();
		canstart = true;
		DoneLoading.Invoke();
		UIUtilities.UpdateTheme(GetTree());
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
		DownloadsDisplayPackages();
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
		AllModsDisplayPackages();
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


	private void AllModsDisplayPackages(){
		populatingpackages = true;		
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("There are {0} packages.", packages.Count));
		if (packages.Count != 0){				
			List<CellContent> griddata = new();
			int rowitem = 0;
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
							Folder = package.Folder
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
					if (package.Category.Name != "Default"){
						content.BackgroundColor = package.Category.Background;
					}
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
	}

	private void AMGetHeaders(){
		AMHeaders = new();
		HBoxContainer headerrow = AllModsGrid.GetNode<HBoxContainer>("VBoxContainer/HeaderScroll/DataGrid_HeaderRow/Row");
		foreach (DataGridHeaderCell cell in headerrow.GetChildren()){
			AMHeaders.Add(cell);
		}
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
		});		
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

	private void DLGetHeaders(){
		DLHeaders = new();
		HBoxContainer headerrow = DownloadedModsGrid.GetNode<HBoxContainer>("VBoxContainer/HeaderScroll/DataGrid_HeaderRow/Row");
		foreach (DataGridHeaderCell cell in headerrow.GetChildren()){
			DLHeaders.Add(cell);
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
		Executable selected = Executables.Where(x => x.Selected == true).First();
		string path = Path.Combine(selected.Path, selected.Exe);
		GetNode<MarginContainer>("GameRunning").Visible = true;
		applicationStarter.Start(path, selected.Arguments, packages.Where(x => x.Enabled == true).ToList());
	}

	private void _on_disconnect_button_pressed(){
		GetNode<MarginContainer>("GameRunning").Visible = false;
	}

	private void _on_application_starter_application_closed(){
		GetNode<MarginContainer>("GameRunning").Visible = false;
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

	private void AddAMRightClickMenu(){		
		if (FloatingItemsContainer.GetChildCount() > 1) FloatingItemsContainer.GetChild(1).QueueFree();
		var rcm = RightClickMenu.Instantiate() as RightClickMenu;
		rcm.Position = GetViewport().GetMousePosition();	
		rcm.RightClickMenuClicked += (item) => AMRCMenuPressed(item);
		if (packages.Where(x => x.Selected == true).Where(x => x.OutOfDate).Any()){
			rcm.someoutofdate = true;
		}
		if (packages.Where(x => x.Selected).Count() > 0){
			rcm.multiplefiles = true;
		}
		//rcm.GetNode<Button>("MarginContainer/VBoxContainer/Linked/AddLinked_Button").Pressed += () => AMRCMenuPressed(0);			
		//rcm.GetNode<Button>("MarginContainer/VBoxContainer/MoveFile/MoveFile_Button").Pressed += () => AMRCMenuPressed(1);			
		//rcm.GetNode<Button>("MarginContainer/VBoxContainer/Delete/DeleteFile_Button").Pressed += () => AMRCMenuPressed(2);			
		//rcm.GetNode<Button>("MarginContainer/VBoxContainer/Rename/RenameFile_Button").Pressed += () => AMRCMenuPressed(3);			
		//rcm.GetNode<Button>("MarginContainer/VBoxContainer/FilesFromFolder/FilesFromFolder_Button").Pressed += () => AMRCMenuPressed(4);
		List<Category> categoriesforlist = Categories;
		if (packages.Where(x => x.Selected == true).Count() > 1){
			List<SimsPackage> selected = packages.Where(x => x.Selected == true).ToList();
			List<Category> cats = selected.Select(x => x.Category).ToList();
			cats = cats.Distinct().ToList();
			if (cats.Count > 1){
				//more than one category								
				rcm.OpenCats.AddRange(cats.Select(x => x.Name));
			} else if (cats.Count == 1){
				if (selected.Where(x => x.Category == cats[0]).Count() != selected.Count){
					rcm.OpenCats.Add(cats[0].Name);
				} else if (selected.Where(x => x.Category == cats[0]).Count() == selected.Count){
					rcm.TickedCat = cats[0].Name;
				}
			}			
		}
		rcm.CategorySelectedMenu += (catname, selected) => AMCategorySelected(catname, selected);
		
		rcm.Categories = Categories;
		FloatingItemsContainer.AddChild(rcm);
		rightclickcatcher.Visible = true;
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
		CallDeferred("AllModsDisplayPackages");
		}){IsBackground = true}.Start();		
    }



    private void AMRCMenuPressed(int button)
    {
		switch (button){
			case 0: 
				//add linked
				GD.Print("Adding linked.");
				break;
			case 1:
				//make root
				break;
			case 2:
				//fave
				break;
			case 3:
				ToggleUpdated();
				break;
			case 4:
				//files from folder
				break;
			case 5: 
				//rename
				RemoveRightClickMenu();
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
		}
    }

	private void ToggleUpdated(){
		bool ood = false;
		if (packages.Where(x => x.Selected == true).Count() == 0){
			packages[rcpackage].OutOfDate = !packages[rcpackage].OutOfDate;
		} else {
			List<SimsPackage> selected = packages.Where(x => x.Selected == true).ToList();
			List<SimsPackage> selectedood = selected.Where(x => x.OutOfDate).ToList();
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
		AllModsDisplayPackages();
		
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
		rightclickmenu.RightClickMenuClicked -= AMRCMenuPressed;
		FloatingItemsContainer.GetChild(1).QueueFree();
		rightclickcatcher.Visible = false;
	}

	private void BrokenPackagesDetected(){
		viewerrorscontainer.Visible = false;
		brokenmodsvisible = true;
		brokenfiles.Visible = true;		
	}
	private void WrongGameDetected(){
		viewerrorscontainer.Visible = false;
		wronggamefilesvisible = true;
		if (LoadedSettings.SetSettings.Instances.Where(x => x.Game == "Sims2").Any()){
			wronggame_sims2.GetNode<LineEdit>("Sims2Wrong_Location_LineEdit").Text = LoadedSettings.SetSettings.Instances.Where(x => x.Game == "Sims2").First().InstanceLocation;
		} else {
			wronggame_sims2.GetNode<LineEdit>("Sims2Wrong_Location_LineEdit").Text = Path.Combine(wronggamefolder, "Sims 2");
		}
		if (LoadedSettings.SetSettings.Instances.Where(x => x.Game == "Sims3").Any()){
			wronggame_sims3.GetNode<LineEdit>("Sims3Wrong_Location_LineEdit").Text = LoadedSettings.SetSettings.Instances.Where(x => x.Game == "Sims3").First().InstanceLocation;
		} else {
			wronggame_sims3.GetNode<LineEdit>("Sims3Wrong_Location_LineEdit").Text = Path.Combine(wronggamefolder, "Sims 3");
		}
		if (LoadedSettings.SetSettings.Instances.Where(x => x.Game == "Sims4").Any()){
			wronggame_sims4.GetNode<LineEdit>("Sims4Wrong_Location_LineEdit").Text = LoadedSettings.SetSettings.Instances.Where(x => x.Game == "Sims4").First().InstanceLocation;
		} else {
			wronggame_sims4.GetNode<LineEdit>("Sims4Wrong_Location_LineEdit").Text = Path.Combine(wronggamefolder, "Sims 4");
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
			string movefolder = othergamesmoveloc;
			FileInfo fi = new(package.Location);
			if (package.Game == Games.Sims2) movefolder = s2moveloc;
			if (package.Game == Games.Sims3) movefolder = s3moveloc;
			if (package.Game == Games.Sims4) movefolder = s4moveloc;
			string newpckloc = Path.Combine(movefolder, fi.Name);
			string newinfoloc = Path.Combine(movefolder, fi.Name.Replace(fi.Extension, ".info"));
			package.Location = newpckloc;
			package.InfoFile = newinfoloc;
			package.WriteInfoFile();
			File.Move(package.Location, newpckloc);
			File.Move(package.InfoFile, newinfoloc);
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
		viewerrorscontainer.Visible = true;
		Label errorslabel = GetNode<Label>("ViewErrors/MarginContainer/VBoxContainer/ScrollContainer/MarginContainer/Errors_Label");
		errorslabel.Text = errorslist.ToString();
	}

	private void CancelViewErrors(){
		viewerrorscontainer.Visible = false;
		GetNode<Label>("ViewErrors/MarginContainer/VBoxContainer/ScrollContainer/MarginContainer/Errors_Label").Text = "";
	}

    public override void _Process(double delta)
    {
		if (canstart){
			if (!readingpackages){
				new Thread(() => {
					if ((Directory.GetFiles(modsfolder).Count() + Directory.GetDirectories(modsfolder).Count()) != filesinpackagesfolder){
						CallDeferred("ReadPackages");
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
					if (Directory.GetFiles(downloadsfolder).ToList().Count != filesindownloadsfolder){
						CallDeferred("ReadDownloads");
					}
					
				}){IsBackground = true}.Start();
			}
			new Thread(() => {
				if (packagedisplayvisible){				
					if (packages.Where(x => x.Selected).Count() != 1){					
						CallDeferred("HidePackageDisplay");
					} else if (packages.IndexOf(packages.Where(x => x.Selected).First()) != packagedisplayed){
						CallDeferred("HidePackageDisplay");
						CallDeferred("ShowPackageDisplay");
					}
				} else if (!packagedisplayvisible){
					if (packages.Where(x => x.Selected).Count() == 1){					
						CallDeferred("ShowPackageDisplay");
					}
				}
			}){IsBackground = true}.Start();

			new Thread(() => {
				int broken = packages.Where(x => x.Broken == true).Count();
				int wronggame = packages.Where(x => x.WrongGame == true).Count();
				NumErrors = broken + wronggame;
				if (NumErrors != _numerrors && NumErrors != 0){
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
				} else {
					_numerrors = NumErrors;
					viewerrorscontainer.HasNotifications = false;
					viewerrorscontainer.Errors = "";
				}
			}){IsBackground = true}.Start();
		}
		
    }

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

	private void MakePackageInformation(){
		CurrentPackageViewer = PackageInformation.Instantiate() as PackageViewer;
		SimsPackage package = packages.Where(x => x.Selected).First();
		packagedisplayed = packages.IndexOf(package);
		ScanData packagedata = package.ScanData;
		if (packagedata.ThumbnailLocation != null){
			CurrentPackageViewer.hasthumbnail = true;
			CurrentPackageViewer.thumbnail = GD.Load<Texture2D>(packagedata.ThumbnailLocation);
		}
		CurrentPackageViewer.PackageName = package.FileName;
		StringBuilder sb = packagedata.PackageInformationDump();	
		CurrentPackageViewer.packageinfo = sb.ToString();	
		GetNode<MarginContainer>("MainWindowSizer/MainPanels/HSplitContainer/VSplitContainer/PackageViewer_Frame_Container/PackageViewer_Container").AddChild(CurrentPackageViewer);
	}




}
