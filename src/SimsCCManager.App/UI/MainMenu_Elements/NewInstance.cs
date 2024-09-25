using Godot;
using SimsCCManager.Containers;
using SimsCCManager.Debugging;
using SimsCCManager.Globals;
using SimsCCManager.Settings.Loaded;
using SimsCCManager.Settings.SettingsSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public partial class NewInstance : MarginContainer
{
	public delegate void NewInstanceStartPackageManagerEvent(Guid identifier);
	public NewInstanceStartPackageManagerEvent NewInstanceStart;
	Games GameChoice = Games.Null;
	private LineEdit instancenamebox;
	private LineEdit installfolderbox;
	private LineEdit documentsfolderbox;
	private LineEdit instancefolderbox;
	private LineEdit packagefolderbox;
	private LineEdit downloadsfolderbox;
	private LineEdit datacachefolderbox;
	private LineEdit profilesfolderbox;
	private bool gettinginstallfolder = false;
	private bool gettingdocsfolder = false;
	private bool gettinginstancefolder = false;
	private bool gettingpackagefolder = false;
	private bool gettingdownloadsfolder = false;
	private bool gettingdatacachefolder = false;
    private bool gettingprofilesfolder;
	private bool createfromcurrent = false;
    MarginContainer page1;
	MarginContainer page2;
	dynamic gameinstance;
	FileDialog filedialog;
	string instancename = null;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		filedialog = GetNode<FileDialog>("FileDialog");
		instancenamebox = GetNode<LineEdit>("Page1/VBoxContainer/InstanceName_LineEdit");
		installfolderbox = GetNode<LineEdit>("Page2/VBoxContainer/InstallFolder/InstallFolder_LineEdit");
		documentsfolderbox = GetNode<LineEdit>("Page2/VBoxContainer/DocsFolder/DocsFolder_LineEdit");
		instancefolderbox = GetNode<LineEdit>("Page2/VBoxContainer/InstanceFolder/InstanceFolder_LineEdit");
		packagefolderbox = GetNode<LineEdit>("Page2/VBoxContainer/PackagesFolder/PackagesFolder_LineEdit");
		downloadsfolderbox = GetNode<LineEdit>("Page2/VBoxContainer/DownloadsFolder/DownloadsFolder_LineEdit");
		datacachefolderbox = GetNode<LineEdit>("Page2/VBoxContainer/DataCacheFolder/DataCacheFolder_LineEdit");
		profilesfolderbox = GetNode<LineEdit>("Page2/VBoxContainer/ProfilesFolder/Profiles_LineEdit");
		page1 = GetNode<MarginContainer>("Page1");
		page2 = GetNode<MarginContainer>("Page2");
		page2.Visible = false;
		page1.Visible = true;
	}

	private void _on_cancel_button_clicked(){
		QueueFree();
	}

	private void _on_custom_check_button_check_toggled(bool toggled){
		createfromcurrent = toggled;
		if (createfromcurrent){
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog("Creating from current.");
		} else {
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog("No longer creating from current.");
		}
	}

	private void _on_confirmation_dialog_canceled(){
		//do nothing?
	}

	private void _on_instance_name_line_edit_text_changed(string name){
		if (name != "" && !string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(name)) instancename = name;
	}

	private void _on_confirmation_dialog_confirmed(){
		ConfirmBuild();
	}

	private void ConfirmBuild(){
		gameinstance = GenerateInstancePrep();
		page1.Visible = false;
		
		if (GameChoice == Games.Sims2){
			Sims2Instance instance = gameinstance as Sims2Instance;
			instance.InstanceName = "The Sims 2";;
			if (!string.IsNullOrEmpty(instancenamebox.Text)) instance.InstanceName = instancenamebox.Text;				
			installfolderbox.Text = instance.GameInstallFolder;			
			documentsfolderbox.Text = instance.GameDocumentsFolder;
			string v = Path.Combine(instance.InstanceFolder, instance.InstanceName);
			instance.InstanceFolder = v;
			instancefolderbox.Text = v;
			packagefolderbox.Text = @"%INSTANCE%\Packages";
			downloadsfolderbox.Text = @"%INSTANCE%\Downloads";
			datacachefolderbox.Text = @"%INSTANCE%\Data";
			profilesfolderbox.Text = @"%INSTANCE%\Profiles";
			instancename = instance.InstanceName;
		} else if (GameChoice == Games.Sims3){
			Sims3Instance instance = gameinstance as Sims3Instance;
			instance.InstanceName = "The Sims 3";
			if (!string.IsNullOrEmpty(instancenamebox.Text)) instance.InstanceName = instancenamebox.Text;
			installfolderbox.Text = instance.GameInstallFolder;			
			documentsfolderbox.Text = instance.GameDocumentsFolder;
			string v = Path.Combine(instance.InstanceFolder, instance.InstanceName);
			instance.InstanceFolder = v;
			instancefolderbox.Text = v;
			packagefolderbox.Text = @"%INSTANCE%\Packages";
			downloadsfolderbox.Text = @"%INSTANCE%\Downloads";
			datacachefolderbox.Text = @"%INSTANCE%\Data";
			profilesfolderbox.Text = @"%INSTANCE%\Profiles";
			instancename = instance.InstanceName;
		} else if (GameChoice == Games.Sims4){
			Sims4Instance instance = gameinstance as Sims4Instance;
			instance.InstanceName = "The Sims 4";
			if (!string.IsNullOrEmpty(instancenamebox.Text)) instance.InstanceName = instancenamebox.Text;
			string v = Path.Combine(instance.InstanceFolder, instance.InstanceName);
			instance.InstanceFolder = v;
			instancefolderbox.Text = v;
			installfolderbox.Text = instance.GameInstallFolder;		
			documentsfolderbox.Text = instance.GameDocumentsFolder;
			instancefolderbox.Text = instance.InstanceFolder;
			packagefolderbox.Text = @"%INSTANCE%\Packages";
			downloadsfolderbox.Text = @"%INSTANCE%\Downloads";
			datacachefolderbox.Text = @"%INSTANCE%\Data";
			profilesfolderbox.Text = @"%INSTANCE%\Profiles";
			instancename = instance.InstanceName;
		}			
		page2.Visible = true;		
	}

	private void _on_confirm_button_clicked(){	
		if (createfromcurrent){
			GetNode<ConfirmationDialog>("ConfirmationDialog").Visible = true;
		} else {
			ConfirmBuild();
		}			
	}

	private void _on_page_2_confirm_button_button_clicked(){
		if (installfolderbox.Text != gameinstance.GameInstallFolder){
			gameinstance.GameInstallFolder = installfolderbox.Text;
		}
		if (documentsfolderbox.Text != gameinstance.GameDocumentsFolder){
			gameinstance.GameDocumentsFolder = documentsfolderbox.Text;
		}
		if (instancefolderbox.Text != gameinstance.InstanceFolder){
			gameinstance.InstanceFolder = instancefolderbox.Text;
		}
		if (packagefolderbox.Text != @"%INSTANCE%\Packages"){
			gameinstance.InstancePackagesFolder = packagefolderbox.Text;
		} else {
			gameinstance.InstancePackagesFolder = Path.Combine(gameinstance.InstanceFolder, "Packages");
		}
		if (downloadsfolderbox.Text != @"%INSTANCE%\Downloads"){
			gameinstance.InstanceDownloadsFolder = downloadsfolderbox.Text;
		} else {
			gameinstance.InstancePackagesFolder = Path.Combine(gameinstance.InstanceFolder, "Downloads");
		}
		if (datacachefolderbox.Text != @"%INSTANCE%\Data"){
			gameinstance.InstanceDataFolder = datacachefolderbox.Text;
		} else {
			gameinstance.InstancePackagesFolder = Path.Combine(gameinstance.InstanceFolder, "Data");
		}
		if (profilesfolderbox.Text != @"%INSTANCE%\Profiles"){
			gameinstance.InstanceProfilesFolder = profilesfolderbox.Text;
		} else {
			gameinstance.InstancePackagesFolder = Path.Combine(gameinstance.InstanceFolder, "Profiles");
		}
		BuildInstance();
	}

	private void _on_install_folder_button_pressed(){
		gettinginstallfolder = true;
		gettingdocsfolder = false;
		gettinginstancefolder = false;
		gettingpackagefolder = false;
		gettingdownloadsfolder = false;
		gettingdatacachefolder = false;
		gettingprofilesfolder = false;
		//gettinginstallfolder = false;
		filedialog.Visible = true;
	}
	private void _on_docs_folder_button_pressed(){
		gettingdocsfolder = true;
		gettinginstancefolder = false;
		gettingpackagefolder = false;
		gettingdownloadsfolder = false;
		gettingdatacachefolder = false;
		gettingprofilesfolder = false;
		gettinginstallfolder = false;
		filedialog.Visible = true;
	}
	private void _on_instance_folder_button_pressed(){
		gettinginstallfolder = false;
		gettingdocsfolder = false;
		gettinginstancefolder = true;
		gettingpackagefolder = false;
		gettingdownloadsfolder = false;
		gettingdatacachefolder = false;
		gettingprofilesfolder = false;
		gettinginstallfolder = false;
		filedialog.Visible = true;
	}
	private void _on_packages_folder_button_pressed(){
		gettinginstallfolder = false;
		gettingdocsfolder = false;
		gettinginstancefolder = false;
		gettingpackagefolder = true;
		gettingdownloadsfolder = false;
		gettingdatacachefolder = false;
		gettingprofilesfolder = false;
		gettinginstallfolder = false;
		filedialog.Visible = true;
	}
	private void _on_downloads_folder_button_pressed(){
		gettinginstallfolder = false;
		gettingdocsfolder = false;
		gettinginstancefolder = false;
		gettingpackagefolder = false;
		gettingdownloadsfolder = true;
		gettingdatacachefolder = false;
		gettingprofilesfolder = false;
		gettinginstallfolder = false;
		filedialog.Visible = true;
	}
	private void _on_data_cache_folder_button_pressed(){
		gettinginstallfolder = false;
		gettingdocsfolder = false;
		gettinginstancefolder = false;
		gettingpackagefolder = false;
		gettingdownloadsfolder = false;
		gettingdatacachefolder = true;
		gettingprofilesfolder = false;
		gettinginstallfolder = false;
		filedialog.Visible = true;
	}
	private void _on_profiles_button_pressed(){
		gettinginstallfolder = false;
		gettingdocsfolder = false;
		gettinginstancefolder = false;
		gettingpackagefolder = false;
		gettingdownloadsfolder = false;
		gettingdatacachefolder = false;
		gettingprofilesfolder = true;
		gettinginstallfolder = false;
		filedialog.Visible = true;
	}

	private void _on_file_dialog_dir_selected(string selected){
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Selected {0}.", selected));
		if (gettingdatacachefolder){
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Selected {0} as data cache folder.", selected));
			datacachefolderbox.Text = selected;
		} else if (gettingdocsfolder){
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Selected {0} as documents folder.", selected));
			documentsfolderbox.Text = selected;
		} else if (gettingdownloadsfolder){
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Selected {0} as downloads folder.", selected));
			downloadsfolderbox.Text = selected;
		} else if (gettinginstallfolder){
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Selected {0} as install folder.", selected));
			installfolderbox.Text = selected;
		} else if (gettinginstancefolder){
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Selected {0} as instance folder.", selected));
			string v = Path.Combine(selected, instancename);
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Instance folder: {0}.", v));
			instancefolderbox.Text = v;
		} else if (gettingpackagefolder){
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Selected {0} as packages folder.", selected));
			packagefolderbox.Text = selected;
		} else if (gettingprofilesfolder){
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Selected {0} as profiles folder.", selected));
			profilesfolderbox.Text = selected;
		}
	}

	private void _on_picked_game(string game){
		if (game == "Sims 2") GameChoice = Games.Sims2;
		if (game == "Sims 3") GameChoice = Games.Sims3;
		if (game == "Sims 4") GameChoice = Games.Sims4;
		if (game == "SimCity 5") GameChoice = Games.SimCity5;
		if (game == "Sims Medieval") GameChoice = Games.SimsMedieval;
		if (game == "The Sims" || game == "Sims 1") GameChoice = Games.Sims1;
	}

	private GameInstanceBase GenerateInstancePrep(){		
		if (GameChoice == Games.Sims2){
			Sims2Instance game = new();
			game.GameChoice = Games.Sims2;
			string gameloc = @"SOFTWARE\WOW6432Node\EA GAMES\The Sims 2";  
			string loc = Utilities.GetPathForExe(gameloc);
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Exe: {0}", loc));
			DirectoryInfo fullloc = new(loc);
			ExeInfo installexe = GetSims2Exe(fullloc);
			if (Directory.Exists(Path.Combine(GlobalVariables.MyDocuments, @"EA Games\The Sims 2"))){
				game.GameDocumentsFolder = Path.Combine(GlobalVariables.MyDocuments, @"EA Games\The Sims 2");
			} else {
				game.GameDocumentsFolder = Path.Combine(GlobalVariables.MyDocuments, @"EA Games\The Sims 2 Ultimate Collection");
			}
			game.GameInstallFolder = loc;
			game.ExePath = installexe.folder;
			game.GameExe = installexe.name;
			game.InstanceName = "The Sims 2";
			if (instancename != null){
				game.InstanceName = instancename;
			}
			game.InstanceFolder = Path.Combine(GlobalVariables.AppFolder, string.Format(@"Instances\"));
			if (File.Exists(Path.Combine(game.ExePath, game.GameExe))) if (GlobalVariables.DebugMode) Logging.WriteDebugLog("Exe exists!");     
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Instance: {0}\nInstall Location: {1}\nDocs Folder: {2}\nExe Name: {3}\nInstance Folder: {4}", game.InstanceName, game.GameInstallFolder, game.GameDocumentsFolder, game.GameExe, game.InstanceFolder));
			return game;
		} else if (GameChoice == Games.Sims3){
			Sims3Instance game = new();
			game.GameChoice = Games.Sims3;
			string gameloc = @"SOFTWARE\WOW6432Node\Sims\The Sims 3"; 
			string docloc = Path.Combine(GlobalVariables.MyDocuments, @"Electronic Arts\The Sims 3");
			string loc = Utilities.GetPathForExe(gameloc);    
			game.GameInstallFolder = loc;
			loc = Path.Combine(loc, "Game");
			loc = Path.Combine(loc, "Bin");
			game.ExePath = loc;
			game.GameExe = "TS3W.Exe";
			game.GameDocumentsFolder = docloc;
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Exe: {0}", loc));
			game.InstanceName = "The Sims 3";
			if (instancename != null){
				game.InstanceName = instancename;
			}
			game.InstanceFolder = Path.Combine(GlobalVariables.AppFolder, string.Format(@"Instances\"));
			if (File.Exists(Path.Combine(game.ExePath, game.GameExe))) if (GlobalVariables.DebugMode) Logging.WriteDebugLog("Exe exists!");     
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Instance: {0}\nInstall Location: {1}\nDocs Folder: {2}\nExe Name: {3}\nInstance Folder: {4}", game.InstanceName, game.GameInstallFolder, game.GameDocumentsFolder, game.GameExe, game.InstanceFolder));
			return game;
		} else if (GameChoice == Games.Sims4){
			Sims4Instance game = new();
			game.GameChoice = Games.Sims4;
			string gameloc = @"SOFTWARE\Maxis\The Sims 4";  
			string loc = Utilities.GetPathForExe(gameloc);       
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Exe: {0}", loc));  
			string docloc = Path.Combine(GlobalVariables.MyDocuments, @"Electronic Arts\The Sims 4");
			game.GameDocumentsFolder = docloc;
			game.GameInstallFolder = loc;
			loc = Path.Combine(loc, "Game");
			loc = Path.Combine(loc, "Bin");
			game.ExePath = loc;
			game.GameExe = "TS4_x64.exe";			
			game.InstanceName = "The Sims 4";
			if (instancename != null){
				game.InstanceName = instancename;
			}
			game.InstanceFolder = Path.Combine(GlobalVariables.AppFolder, string.Format(@"Instances\"));
			if (File.Exists(Path.Combine(game.ExePath, game.GameExe))) if (GlobalVariables.DebugMode) Logging.WriteDebugLog("Exe exists!");     
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Instance: {0}\nInstall Location: {1}\nDocs Folder: {2}\nExe Name: {3}\nInstance Folder: {4}", game.InstanceName, game.GameInstallFolder, game.GameDocumentsFolder, game.GameExe, game.InstanceFolder));
			return game;
		}
		return new Sims2Instance();
	}

	private ExeInfo GetSims2Exe(DirectoryInfo directory){
		//passed in will be "The Sims 2" so... parent
		directory = directory.Parent;
		//this should be the sims 2 main folder, so Ultimate Collection etc
		List<DirectoryInfo> directories = directory.GetDirectories().ToList();
		//subfolders of this folder 
		if (directories.Where(x => x.Name.Equals("The Sims 2 Mansion and Garden Stuff")).Any()){
			string mag = directories.Where(x => x.Name.Equals("The Sims 2 Mansion and Garden Stuff")).First().FullName;
			mag = Path.Combine(mag, "TSBin");
			List<FileInfo> files = new DirectoryInfo(mag).GetFiles().ToList();
			if (files.Where(x => x.Name == "Sims2RPC.exe").Any()){
				return new ExeInfo(){name = "Sims2RPC.exe", folder = files.Where(x => x.Name == "Sims2RPC.exe").First().DirectoryName};
			} else {
				return new ExeInfo(){name = "Sims2EP9.exe", folder = files.Where(x => x.Name == "Sims2EP9.exe").First().DirectoryName};
			}
		} else if (directories.Where(x => x.Name.Equals("Fun with Pets")).Any()){
			string fwp = directories.Where(x => x.Name.Equals("Fun with Pets")).First().FullName;
			fwp = Path.Combine(fwp, "SP9");
			fwp = Path.Combine(fwp, "TSBin");
			List<FileInfo> files = new DirectoryInfo(fwp).GetFiles().ToList();
			if (files.Where(x => x.Name == "Sims2RPC.exe").Any()){
				return new ExeInfo(){name = "Sims2RPC.exe", folder = files.Where(x => x.Name == "Sims2RPC.exe").First().DirectoryName};
			} else {
				return new ExeInfo(){name = "Sims2EP9.exe", folder = files.Where(x => x.Name == "Sims2EP9.exe").First().DirectoryName};
			}
		} else {
			return new ExeInfo(){folder = "unknown", name = "unknown"};
		}
	}

	private void BuildInstance(){			
		string game = "";
		Guid identifier = Guid.NewGuid();
		if (GameChoice == Games.Sims2){
			Sims2Instance instance = gameinstance as Sims2Instance;
			instance.Identifier = identifier;
			if (createfromcurrent){
				instance.BuildInstance(true, identifier);
			} else {
				instance.BuildInstance(false, identifier);
			}
			game = "Sims2";
		} else if (GameChoice == Games.Sims3){
			Sims3Instance instance = gameinstance as Sims3Instance;
			instance.Identifier = identifier;
			if (createfromcurrent){
				instance.BuildInstance(true, identifier);
			} else {
				instance.BuildInstance(false, identifier);
			}
			game = "Sims3";
		} else if (GameChoice == Games.Sims4){
			Sims4Instance instance = gameinstance as Sims4Instance;
			instance.Identifier = identifier;
			if (createfromcurrent){
				instance.BuildInstance(true, identifier);
			} else {
				instance.BuildInstance(false, identifier);
			}
			game = "Sims4";
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Instance Ident: {0}.", instance.Identifier));
		}

		

		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Adding new instance to instance list..."));
		LoadedSettings.SetSettings.ChangeSetting(new Instance(){ Game = game, InstanceLocation = gameinstance.InstanceFolder, Name = gameinstance.InstanceName, Identifier = gameinstance.Identifier });
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Emitting signal with {0} guid.", identifier));
		NewInstanceStart.Invoke(identifier);
	}
}
