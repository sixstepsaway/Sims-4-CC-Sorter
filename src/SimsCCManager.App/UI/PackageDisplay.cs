using Godot;
using Microsoft.VisualBasic;
using SimsCCManager.Containers;
using SimsCCManager.Debugging;
using SimsCCManager.Globals;
using SimsCCManager.Packages.Containers;
using SimsCCManager.Settings.Loaded;
using SimsCCManager.Settings.SettingsSystem;
using SimsCCManager.UI.Containers;
using SimsCCManager.UI.Properties;
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
using System.Threading;
using System.Threading.Tasks;

public partial class PackageDisplay : MarginContainer
{
	[Signal]
	public delegate void SetPbarMaxEventHandler();
	[Signal]
	public delegate void IncrementPbarEventHandler();
	[Signal]
	public delegate void ResetPbarValueEventHandler();
	[Signal]
	public delegate void ShowPbarEventHandler();
	[Signal]
	public delegate void HidePbarEventHandler();
	private ExeChoicePopupPanel ExeChoicePanel;
	//PackedScene DataGridRow = GD.Load<PackedScene>("res://UI/PackageDisplay_Elements/data_grid_row.tscn");
	Control DataGridAllMods; 
	Control DataGridDownloads;
	PackedScene DataGrid = GD.Load<PackedScene>("res://UI/CustomDataGrid/CustomDataGrid.tscn");
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
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		LoadedSettings.SetSettings.LastInstanceLoaded = LoadedSettings.SetSettings.CurrentInstance.InstanceLocation;
		ExeChoiceControl = GetNode<Control>("MainWindowSizer/TopPanels/GameStartControls/ExeChoice_PopupPanel_Control");
		ExeChoicePanel = GetNode<ExeChoicePopupPanel>("MainWindowSizer/TopPanels/GameStartControls/ExeChoice_PopupPanel_Control/ExeChoice_PopupPanel");
		ExeChoiceControl.Visible = false;
		DataGridAllMods = GetNode<Control>("MainWindowSizer/MainPanels/HSplitContainer/AllMods_Frame_Container/AllMods_Container/GridContainer");
		DataGridDownloads = GetNode<Control>("MainWindowSizer/MainPanels/HSplitContainer/VSplitContainer/NewDownloads_Frame_Container/NewDL_Container/GridContainer");
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
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Got the instance! Returning to package display..."));
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Proof of data:"));
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Packages folder: {0}", sims2Instance.InstancePackagesFolder));
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Downloads folder: {0}", sims2Instance.InstanceDownloadsFolder));
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Profiles folder: {0}", sims2Instance.InstanceProfilesFolder));
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Data folder: {0}", sims2Instance.InstanceDataFolder));
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Instance folder: {0}", instancedatafolder));
			modsfolder = sims2Instance.InstancePackagesFolder;
			downloadsfolder = sims2Instance.InstanceDownloadsFolder;
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
		}
		SetupDisplay();

		UpdateExecutableList();
	}

	private void UpdateExecutableList(){
		ExeChoicePanel.instancedatafolder = instancedatafolder;
		ExeChoicePanel.Executables = Executables;
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

	public void ReadPackages(){
		readingpackages = true;
		ConcurrentBag<SimsPackage> packagesbag = new();
		List<string> packagefiles = Directory.GetFiles(modsfolder, "*.package").ToList();
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("There are {0} files to read", packagefiles.Count));
		if (packagefiles.Count != 0){
			Parallel.ForEach(packagefiles, new ParallelOptions { MaxDegreeOfParallelism = 4 }, file => {
				SimsPackage simsPackage = new();				
				simsPackage.GetInfo(file);
				if (simsPackage.Game != Game){
					simsPackage.WrongGame = true;
				}
				if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("There are {0} packages in the bag.", packagesbag.Count));
				if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("This package is for Sims {0}.", simsPackage.Game));
				packagesbag.Add(simsPackage);
				
			});
			packages = packagesbag.ToList();
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("There are {0} packages in packages.", packages.Count));
			if (!packagessortingorderchanged) packages = packages.OrderBy(x => x.FileName).ToList();
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("There are {0} ordered packages in packages.", packages.Count));
		}
		if (firstrunpackages) AllModsDisplayPackages();
		readingpackages = false;
	}

	public void ReadDownloads(){
		readingdownloads = true;
		ConcurrentBag<SimsDownload> downloadsbag = new();
		List<string> files = Directory.GetFiles(downloadsfolder).ToList();
		if (files.Count != 0){
			Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = 4 }, file => {
				SimsDownload simsDownload = new();
				simsDownload.GetInfo(file);
				downloadsbag.Add(simsDownload);
			});
			if (!downloadssortingorderchanged) downloads = downloadsbag.ToList().OrderBy(x => x.FileName).ToList();
		}
		readingdownloads = false;
		if (firstrundownloads) DownloadsDisplayPackages();
	}

	private void _on_hide_popup_button_pressed(){
		ExeChoiceControl.Visible = false;
	}

	private void SetupDisplay(){		
		AllModsGrid = DataGrid.Instantiate() as CustomDataGrid;		
		AllModsGrid.Connect("SelectedItem", new Callable(this, "AllModsItemSelected"));
		AllModsGrid.Connect("UnselectedItem", new Callable(this, "AllModsItemUnselected"));
		AllModsGrid.Connect("EnabledItem", new Callable(this, "AllModsItemEnabled"));
		AllModsGrid.Connect("DisabledItem", new Callable(this, "AllModsItemDisabled"));
		AllModsGrid.Headers = GetDefaultAMHeaders();

		ReadPackages();

		DataGridAllMods.AddChild(AllModsGrid);
		AllModsGrid = DataGridAllMods.GetChild(0) as CustomDataGrid;
		AllModsRows = AllModsGrid.GetChild(0).GetChild(0) as VBoxContainer;
		
		AllModsDisplayPackages();

		ReadDownloads();

		DownloadedModsGrid = DataGrid.Instantiate() as CustomDataGrid;
		DownloadedModsGrid.Connect("SelectedItem", new Callable(this, "DownloadsItemSelected"));
		DownloadedModsGrid.Connect("UnselectedItem", new Callable(this, "DownloadsItemUnselected"));
		DownloadedModsGrid.Headers = GetDefaultDownloadsHeaders();

		DataGridDownloads.AddChild(DownloadedModsGrid);
		DownloadedModsGrid = DataGridDownloads.GetChild(0) as CustomDataGrid;
		DownloadsRows = DownloadedModsGrid.GetChild(0).GetChild(0) as VBoxContainer;

		DownloadsDisplayPackages();
	}

	
	private void AllModsItemEnabled(string item, int idx){
		if (packages.Where(x => x.Selected).Count() > 1){
			List<SimsPackage> selected = packages.Where(x => x.Selected).ToList();
			foreach (SimsPackage package in selected){
				idx = packages.IndexOf(package);
				packages[idx].Enabled = true;
				(AllModsRows.GetChild(idx+1) as DataGridRow).ToggleEnabled(true);
			}
		} else {
			packages[idx-1].Enabled = true;
			(AllModsRows.GetChild(idx) as DataGridRow).ToggleEnabled(true);
		}
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("There are {0} packages enabled", packages.Where(x => x.Enabled == true).Count()));
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Enabled packages:"));
		foreach (SimsPackage package in packages.Where(x => x.Enabled == true)){
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(package.FileName);
		}
	}
	private void AllModsItemDisabled(string item, int idx){
		if (packages.Where(x => x.Selected).Count() > 1){
			List<SimsPackage> selected = packages.Where(x => x.Selected).ToList();
			foreach (SimsPackage package in selected){
				idx = packages.IndexOf(package);
				packages[idx].Enabled = false;
				(AllModsRows.GetChild(idx+1) as DataGridRow).ToggleEnabled(false);
			}
		} else {
			packages[idx-1].Enabled = false;
			(AllModsRows.GetChild(idx) as DataGridRow).ToggleEnabled(false);
		}
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("There are {0} packages enabled", packages.Where(x => x.Enabled == true).Count()));
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Enabled packages:"));
		foreach (SimsPackage package in packages.Where(x => x.Enabled == true)){
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(package.FileName);
		}
	}

	private void AllModsItemSelected(string item, int idx){
		if (holdingctrl || holdingshift){
			if (holdingctrl){
				(AllModsRows.GetChild(idx) as DataGridRow).Selected = true;
				packages.Where(x => x.Identifier == Guid.Parse(item)).First().Selected = true;				
				amlastselected = idx;
			} else if (holdingshift){
				AMShiftSelection(item, idx);
			}
		} else {
			for (int i = 1; i < AllModsRows.GetChildCount(); i++){
				DataGridRow row = AllModsRows.GetChild(i) as DataGridRow;
				if (row.Selected){
					packages[i-1].Selected = false;
					row.Selected = false;
				}
			}
			(AllModsRows.GetChild(idx) as DataGridRow).Selected = true;
			packages[idx-1].Selected = true;
			amlastselected = idx;
		}
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("There are {0} packages selected", packages.Where(x => x.Selected == true).Count()));
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Selected packages:"));
		foreach (SimsPackage package in packages.Where(x => x.Selected == true)){
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(package.FileName);
		}
			
	}
	private void AllModsItemUnselected(string item, int idx){		
		if (holdingctrl){		
			(AllModsRows.GetChild(idx) as DataGridRow).Selected = false;		
			packages[idx-1].Selected = false;
			amlastselected = idx;
		} else if (holdingshift) {
			AMShiftSelection(item, idx);
 		} else {
			if (packages.Where(x => x.Selected).ToList().Count > 1){
				for (int i = 1; i < AllModsRows.GetChildCount(); i++){
					DataGridRow row = AllModsRows.GetChild(i) as DataGridRow;
					if (row.Selected){
						packages[i-1].Selected = false;
						row.Selected = false;
					}
				}
				(AllModsRows.GetChild(idx) as DataGridRow).Selected = true;		
				packages[idx-1].Selected = true;
			} else {
				for (int i = 1; i < AllModsRows.GetChildCount(); i++){
					DataGridRow row = AllModsRows.GetChild(i) as DataGridRow;
					if (row.Selected){
						row.Selected = false;
					}
				}
				packages[idx-1].Selected = false;
			}
			amlastselected = idx;			
		}
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("There are {0} packages selected", packages.Where(x => x.Selected == true).Count()));
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Selected packages:"));
		foreach (SimsPackage package in packages.Where(x => x.Selected == true)){
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(package.FileName);
		}
		
	}

	private void AMShiftSelection(string item, int idx){
		if (packages.Where(x => x.Selected == true).Any()){
			int p_idx_last = packages.IndexOf(packages.Where(x => x.Selected).Last());
			int p_idx_first = packages.IndexOf(packages.Where(x => x.Selected).First());
			int r_idx_last = p_idx_last + 1;
			int r_idx_first = p_idx_first + 1;
			for (int i = 0; i < packages.Count; i++){
				packages[i].Selected = false;
				(AllModsRows.GetChild(i+1) as DataGridRow).Selected = false;
			}

			if (r_idx_first == r_idx_last){
				int both = r_idx_first;
				if (both > idx){
					for (int i = idx; i <= both; i++){
						packages[i-1].Selected = true;
						(AllModsRows.GetChild(i) as DataGridRow).Selected = true;
					}
				} else if (idx > both){
					for (int i = both; i <= idx; i++){
						packages[i-1].Selected = true;
						(AllModsRows.GetChild(i) as DataGridRow).Selected = true;
					}
				}
			} else if (amlastselected == r_idx_last){
				if (idx > r_idx_last){
					for (int i = r_idx_first; i <= idx; i++){
						packages[i-1].Selected = true;
						(AllModsRows.GetChild(i) as DataGridRow).Selected = true;
					}
				} else if (idx < r_idx_last && idx > r_idx_first){
					for (int i = r_idx_first; i <= idx; i++){
						packages[i-1].Selected = true;
						(AllModsRows.GetChild(i) as DataGridRow).Selected = true;
					}
				} else if (idx < r_idx_last && idx < r_idx_first){
					for (int i = idx; i <= r_idx_first; i++){
						packages[i-1].Selected = true;
						(AllModsRows.GetChild(i) as DataGridRow).Selected = true;
					}
				}
			} else if (amlastselected == r_idx_first){
				if (idx > r_idx_first){
					for (int i = r_idx_first; i <= idx; i++){
						packages[i-1].Selected = true;
						(AllModsRows.GetChild(i) as DataGridRow).Selected = true;
					}
				} else if (idx < r_idx_first){
					for (int i = idx; i <= r_idx_first; i++){
						packages[i-1].Selected = true;
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
			packages.Where(x => x.Identifier == Guid.Parse(item)).First().Selected = true;				
		}
		amlastselected = idx;
	}







	private void DLShiftSelection(string item, int idx){
		if (downloads.Where(x => x.Selected == true).Any()){
			int p_idx_last = downloads.IndexOf(downloads.Where(x => x.Selected).Last());
			int p_idx_first = downloads.IndexOf(downloads.Where(x => x.Selected).First());
			int r_idx_last = p_idx_last + 1;
			int r_idx_first = p_idx_first + 1;
			for (int i = 0; i < downloads.Count; i++){
				downloads[i].Selected = false;
				(DownloadsRows.GetChild(i+1) as DataGridRow).Selected = false;
			}

			if (r_idx_first == r_idx_last){
				int both = r_idx_first;
				if (both > idx){
					for (int i = idx; i <= both; i++){
						downloads[i-1].Selected = true;
						(DownloadsRows.GetChild(i) as DataGridRow).Selected = true;
					}
				} else if (idx > both){
					for (int i = both; i <= idx; i++){
						downloads[i-1].Selected = true;
						(DownloadsRows.GetChild(i) as DataGridRow).Selected = true;
					}
				}
			} else if (amlastselected == r_idx_last){
				if (idx > r_idx_last){
					for (int i = r_idx_first; i <= idx; i++){
						downloads[i-1].Selected = true;
						(DownloadsRows.GetChild(i) as DataGridRow).Selected = true;
					}
				} else if (idx < r_idx_last && idx > r_idx_first){
					for (int i = r_idx_first; i <= idx; i++){
						downloads[i-1].Selected = true;
						(DownloadsRows.GetChild(i) as DataGridRow).Selected = true;
					}
				} else if (idx < r_idx_last && idx < r_idx_first){
					for (int i = idx; i <= r_idx_first; i++){
						downloads[i-1].Selected = true;
						(DownloadsRows.GetChild(i) as DataGridRow).Selected = true;
					}
				}
			} else if (amlastselected == r_idx_first){
				if (idx > r_idx_first){
					for (int i = r_idx_first; i <= idx; i++){
						downloads[i-1].Selected = true;
						(DownloadsRows.GetChild(i) as DataGridRow).Selected = true;
					}
				} else if (idx < r_idx_first){
					for (int i = idx; i <= r_idx_first; i++){
						downloads[i-1].Selected = true;
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
			downloads.Where(x => x.Identifier == Guid.Parse(item)).First().Selected = true;				
		}
		dllastselected = idx;
	}






	private void DownloadsItemSelected(string item, int idx){
		if (holdingctrl || holdingshift){
			if (holdingctrl){
				(DownloadsRows.GetChild(idx) as DataGridRow).Selected = true;
				downloads.Where(x => x.Identifier == Guid.Parse(item)).First().Selected = true;				
				dllastselected = idx;
			} else if (holdingshift){
				DLShiftSelection(item, idx);
			}
		} else {
			for (int i = 1; i < DownloadsRows.GetChildCount(); i++){
				DataGridRow row = DownloadsRows.GetChild(i) as DataGridRow;
				if (row.Selected){
					downloads[i-1].Selected = false;
					row.Selected = false;
				}
			}
			(DownloadsRows.GetChild(idx) as DataGridRow).Selected = true;
			downloads[idx-1].Selected = true;
			dllastselected = idx;
		}
	}
	private void DownloadsItemUnselected(string item, int idx){
		if (holdingctrl){		
			(DownloadsRows.GetChild(idx) as DataGridRow).Selected = false;		
			downloads[idx-1].Selected = false;
			dllastselected = idx;
		} else if (holdingshift) {
			DLShiftSelection(item, idx);
 		} else {
			if (downloads.Where(x => x.Selected).ToList().Count > 1){
				for (int i = 1; i < DownloadsRows.GetChildCount(); i++){
					DataGridRow row = DownloadsRows.GetChild(i) as DataGridRow;
					if (row.Selected){
						downloads[i-1].Selected = false;
						row.Selected = false;
					}
				}
				(DownloadsRows.GetChild(idx) as DataGridRow).Selected = true;		
				downloads[idx-1].Selected = true;
			} else {
				for (int i = 1; i < DownloadsRows.GetChildCount(); i++){
					DataGridRow row = DownloadsRows.GetChild(i) as DataGridRow;
					if (row.Selected){
						row.Selected = false;
					}
				}
				downloads[idx-1].Selected = false;
			}
			dllastselected = idx;			
		}
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
					if (inf.ContentType == DataGridContentType.Icons){
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
		Utilities.RunProcess(path, selected.Arguments);
	}

	private void _on_swap_instances_settings_help_buttons_button_clicked(){
		(GetParent() as MainWindow).ShowMainMenu();
		QueueFree();
	}

	private void _on_game_choice_dropdown_pressed(){
		ExeChoiceControl.Visible = true;
	}

	private void _IncrementPbar(){
		EmitSignal("IncrementPbar");
	}

	private void _SetPbarMax(int max){
		EmitSignal("SetPbarMax, max");
	}

	private void _ResetPbarValue(){
		EmitSignal("ResetPbarValue");
	}
	private void _HidePbar(){
		EmitSignal("HidePbar");
	}
	private void _ShowPbar(){
		EmitSignal("ShowPbar");
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
    }

    public override void _Process(double delta)
    {
		if (!readingpackages){
			new Thread(() => {
				if (Directory.GetFiles(modsfolder, "*.package").ToList().Count != packages.Count){
					CallDeferred("ReadPackages");
				}				
			}){IsBackground = true}.Start();
		}

		/*if (!populatingpackages){
			new Thread(() => {
				if (packages.Count != _packages.Count){
					CallDeferred("AllModsDisplayPackages");
					//_packages = packages;
				}
			}){IsBackground = true}.Start();
		}*/
		
		/*if (!populatingdownloads){
			new Thread(() => {				
				if (downloads.Count != _downloads.Count){
					CallDeferred("DownloadsDisplayPackages");
					//_downloads = downloads;
				}
			}){IsBackground = true}.Start();
		}*/
		
		if (!readingdownloads){
			new Thread(() => {
				if (Directory.GetFiles(downloadsfolder).ToList().Count != downloads.Count){
					CallDeferred("ReadDownloads");
				}
				
			}){IsBackground = true}.Start();
		}
    }
}
