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
	[Signal]
	public delegate void NewInstanceStartPackageManagerEventHandler();
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
		gameinstance = GenerateInstancePrep();
		page1.Visible = false;
		if (GameChoice == Games.Sims2){
			Sims2Instance instance = gameinstance as Sims2Instance;
			installfolderbox.Text = instance.GameInstallFolder;			
			documentsfolderbox.Text = instance.GameDocumentsFolder;
			instancefolderbox.Text = instance.InstanceFolder;
			packagefolderbox.Text = @"%INSTANCE%\Packages";
			downloadsfolderbox.Text = @"%INSTANCE%\Downloads";
			datacachefolderbox.Text = @"%INSTANCE%\Data";
			profilesfolderbox.Text = @"%INSTANCE%\Profiles";
		} else if (GameChoice == Games.Sims3){
			Sims3Instance instance = gameinstance as Sims3Instance;
			installfolderbox.Text = instance.GameInstallFolder;			
			documentsfolderbox.Text = instance.GameDocumentsFolder;
			instancefolderbox.Text = instance.InstanceFolder;
			packagefolderbox.Text = @"%INSTANCE%\Packages";
			downloadsfolderbox.Text = @"%INSTANCE%\Downloads";
			datacachefolderbox.Text = @"%INSTANCE%\Data";
			profilesfolderbox.Text = @"%INSTANCE%\Profiles";
		} else if (GameChoice == Games.Sims4){
			Sims4Instance instance = gameinstance as Sims4Instance;
			installfolderbox.Text = instance.GameInstallFolder;			
			documentsfolderbox.Text = instance.GameDocumentsFolder;
			instancefolderbox.Text = instance.InstanceFolder;
			packagefolderbox.Text = @"%INSTANCE%\Packages";
			downloadsfolderbox.Text = @"%INSTANCE%\Downloads";
			datacachefolderbox.Text = @"%INSTANCE%\Data";
			profilesfolderbox.Text = @"%INSTANCE%\Profiles";
		}		
		page2.Visible = true;
	}

	private void _on_confirm_button_clicked(){	
		if (createfromcurrent){
			GetNode<ConfirmationDialog>("ConfirmationDialog").Visible = true;
		} else {
			gameinstance = GenerateInstancePrep();
			page1.Visible = false;
			if (GameChoice == Games.Sims2){
				Sims2Instance instance = gameinstance as Sims2Instance;
				installfolderbox.Text = instance.GameInstallFolder;			
				documentsfolderbox.Text = instance.GameDocumentsFolder;
				instancefolderbox.Text = instance.InstanceFolder;
				packagefolderbox.Text = @"%INSTANCE%\Packages";
				downloadsfolderbox.Text = @"%INSTANCE%\Downloads";
				datacachefolderbox.Text = @"%INSTANCE%\Data";
				profilesfolderbox.Text = @"%INSTANCE%\Profiles";
			} else if (GameChoice == Games.Sims3){
				Sims3Instance instance = gameinstance as Sims3Instance;
				installfolderbox.Text = instance.GameInstallFolder;			
				documentsfolderbox.Text = instance.GameDocumentsFolder;
				instancefolderbox.Text = instance.InstanceFolder;
				packagefolderbox.Text = @"%INSTANCE%\Packages";
				downloadsfolderbox.Text = @"%INSTANCE%\Downloads";
				datacachefolderbox.Text = @"%INSTANCE%\Data";
				profilesfolderbox.Text = @"%INSTANCE%\Profiles";
			} else if (GameChoice == Games.Sims4){
				Sims4Instance instance = gameinstance as Sims4Instance;
				installfolderbox.Text = instance.GameInstallFolder;			
				documentsfolderbox.Text = instance.GameDocumentsFolder;
				instancefolderbox.Text = instance.InstanceFolder;
				packagefolderbox.Text = @"%INSTANCE%\Packages";
				downloadsfolderbox.Text = @"%INSTANCE%\Downloads";
				datacachefolderbox.Text = @"%INSTANCE%\Data";
				profilesfolderbox.Text = @"%INSTANCE%\Profiles";
			}		
			page2.Visible = true;
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
		if (createfromcurrent){
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog("dont forget to set up creating from current!");
		} else {
			//create the package display thing
		}
	}

	private void _on_install_folder_button_pressed(){
		GetNoFolder(true);
		gettinginstallfolder = true;
	}
	private void _on_docs_folder_button_pressed(){
		GetNoFolder(true);
		gettingdocsfolder = true;
	}
	private void _on_instance_folder_button_pressed(){
		GetNoFolder(true);
		gettinginstancefolder = true;
	}
	private void _on_packages_folder_button_pressed(){
		GetNoFolder(true);
		gettingpackagefolder = true;
	}
	private void _on_downloads_folder_button_pressed(){
		GetNoFolder(true);
		gettingdownloadsfolder = true;
	}
	private void _on_data_cache_folder_button_pressed(){
		GetNoFolder(true);
		gettingdatacachefolder = true;
	}
	private void _on_profiles_button_pressed(){
		GetNoFolder(true);
		gettingprofilesfolder = true;
	}

	private void _on_file_dialog_dir_selected(string selected){
		if (gettingdatacachefolder){
			datacachefolderbox.Text = selected;
		} else if (gettingdocsfolder){
			documentsfolderbox.Text = selected;
		} else if (gettingdownloadsfolder){
			downloadsfolderbox.Text = selected;
		} else if (gettinginstallfolder){
			installfolderbox.Text = selected;
		} else if (gettinginstancefolder){
			installfolderbox.Text = selected;
		} else if (gettingpackagefolder){
			packagefolderbox.Text = selected;
		} else if (gettingprofilesfolder){
			profilesfolderbox.Text = selected;
		}
		GetNoFolder(false);
	}

	private void GetNoFolder(bool showfiledialog){
		if (showfiledialog) filedialog.Visible = true;
		gettinginstallfolder = false;
		gettingdocsfolder = false;
		gettinginstancefolder = false;
		gettingpackagefolder = false;
		gettingdownloadsfolder = false;
		gettingdatacachefolder = false;
		gettingprofilesfolder = false;
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
			game.InstanceFolder = Path.Combine(GlobalVariables.AppFolder, string.Format(@"instances\{0}", game.InstanceName));
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
			game.InstanceFolder = Path.Combine(GlobalVariables.AppFolder, string.Format(@"instances\{0}", game.InstanceName));
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
			game.InstanceFolder = Path.Combine(GlobalVariables.AppFolder, string.Format(@"instances\{0}", game.InstanceName));
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
		StringBuilder sb = new();	
		if (GameChoice == Games.Sims2){
			Sims2Instance instance = gameinstance as Sims2Instance;
			instance.BuildInstance();
			sb = instance.WriteIni();
			game = "Sims2";
		} else if (GameChoice == Games.Sims3){
			Sims3Instance instance = gameinstance as Sims3Instance;
			instance.BuildInstance();
			sb = instance.WriteIni();
			game = "Sims3";
		} else if (GameChoice == Games.Sims4){
			Sims4Instance instance = gameinstance as Sims4Instance;
			instance.BuildInstance();
			sb = instance.WriteIni();
			game = "Sims4";
		}		
		string inifile = Path.Combine(gameinstance.InstanceFolder, "Instance.ini");
		if (File.Exists(inifile)){
			string ren = inifile.Replace("Instance.ini", "Instance.ini.bk");
			File.Move(inifile, ren);
		}
		using (StreamWriter streamWriter = new(inifile)){
			streamWriter.Write(sb);
		}  
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Adding new instance to instance list..."));
		LoadedSettings.SetSettings.ChangeSetting(new Instance(){ Game = game, InstanceLocation = gameinstance.InstanceFolder, Name = gameinstance.InstanceName, Identifier = gameinstance.Identifier });
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Emitting signal with {0} guid.", gameinstance.Identifier.ToString()));
		EmitSignal("NewInstanceStartPackageManager", gameinstance.Identifier.ToString());
	}
}
