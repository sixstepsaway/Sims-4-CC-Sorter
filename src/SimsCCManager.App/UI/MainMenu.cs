using Godot;
using SimsCCManager.Containers;
using SimsCCManager.Debugging;
using SimsCCManager.Globals;
using SimsCCManager.PackageReaders;
using SimsCCManager.PackageReaders.Containers;
using SimsCCManager.Packages.Containers;
using SimsCCManager.Settings.Loaded;
using SimsCCManager.UI.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using MoreLinq;
using System.Threading.Tasks.Dataflow;

public partial class MainMenu : MarginContainer
{
	PackedScene settingswindow = GD.Load<PackedScene>("res://UI/MainMenu_Elements/main_settings.tscn");
	MarginContainer NewInstanceMenu;
	MarginContainer MainMenuContainer;
	public delegate void MainMenuStartInstanceEvent(Guid instance);
	public MainMenuStartInstanceEvent MainMenuStartInstance;
	// Called when the node enters the scene tree for the first time.
	PackedScene newinstancemenu = GD.Load<PackedScene>("res://UI/MainMenu_Elements/new_instance.tscn");
	PackedScene loadinstancemenu = GD.Load<PackedScene>("res://UI/MainMenu_Elements/load_instance.tscn");
	public override void _Ready()
	{
		GetNode<MarginContainer>("Tali").Visible = LoadedSettings.SetSettings.ShowTali;
		GetNode<MarginContainer>("Menu/MarginContainer/VBoxContainer/MMButton_DevTest").Visible = LoadedSettings.SetSettings.DebugMode;
		//NewInstanceMenu = GetNode<MarginContainer>("NewInstance");
		MainMenuContainer = GetNode<MarginContainer>("Menu");
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("There are {0} instances loaded", LoadedSettings.SetSettings.Instances.Count));
		if (LoadedSettings.SetSettings.Instances.Count != 0) GetNode<MarginContainer>("Menu/MarginContainer/VBoxContainer/MMButton_LoadInstance").Visible = true;
	}

	private void _on_tali_mouse_entered(){
		string text = "In memory of my beloved Tali. My blanket will always have an empty space where you used to curl up while we coded this app.";
		ToolTip tooltip = UIUtilities.CustomTooltip(text, GetGlobalMousePosition());
		GetWindow().AddChild(tooltip);
	}
	
	private void _on_mm_button_new_instance_button_clicked(){
		var newinstance = newinstancemenu.Instantiate() as NewInstance;
		//newinstance.Connect("tree_exited", Callable.From(CancelledInstance));
		newinstance.TreeExited += () => CancelledInstance();

		//newinstance.Connect("NewInstanceStartPackageManager", new Callable(this, "LoadInstance"));
		newinstance.NewInstanceStart += (instance) => LoadInstance(instance);
		MainMenuContainer.Visible = false;		
		AddChild(newinstance);
	}
	private void _on_mm_button_load_instance_button_clicked(){
		MainMenuContainer.Visible = false;
		var loadinstance = loadinstancemenu.Instantiate() as LoadInstance;
		loadinstance.TreeExited += () => CancelledInstance();
		loadinstance.LoadInstanceStartPackageManager += (instance) => LoadInstance(instance);
		//loadinstance.Connect("tree_exited", Callable.From(CancelledInstance));
		//loadinstance.Connect("LoadInstanceStartPackageManager", new Callable(this, "LoadInstance"));
		AddChild(loadinstance);
	}

	private void LoadInstance(Guid instance){
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Heard the signal to load instance on Main Menu!"));
		MainMenuStartInstance.Invoke(instance);
	}
	private void _on_mm_button_settings_button_clicked(){
		var swindow = settingswindow.Instantiate();
		swindow.Connect("Tali", Callable.From(TaliEmitted));
		swindow.Connect("Debug", Callable.From(DebugEmitted));
		AddChild(swindow);
	}
	private void _on_mm_button_help_button_clicked(){
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog("Help");
	}
	private void _on_mm_button_quit_button_clicked(){
		GetTree().Quit();
	}

	private void _on_mm_button_dev_test_button_clicked(){
		
		List<FileInfo> fileInfos = new(){
			new(@"M:\SCCM\The Sims 2\Packages\mesh-nat-eftopfanseejacket.package"),
			new(@"M:\The Sims 2 (Documents)\!SymlinkFolders\Uncompressed\Hair\AdeAaliyahFAllColorsByY2Sims.package"),
			new(@"M:\The Sims 2 (Documents)\!SymlinkFolders\Uncompressed\Clothing\DigiAfBottompajamasdrawstringMESH.package")
		};

		foreach (FileInfo fi in fileInfos){
			Sims2PackageReader s2pr = new();
			s2pr.simsPackage = new() { FileName = fi.Name.Replace(fi.Extension, ""), Location = fi.FullName};
			s2pr.ReadSims2Package();
		}

		



		//GetS2Overrides();




		
		
		
		
	}

	private void GetS4Overrides(){
		string gameoverridefolder = Path.Combine(GlobalVariables.overridesfolder, "Sims 4");		
		string gameloc = @"SOFTWARE\Maxis\The Sims 4";
		string s4 = Utilities.GetPathForExe(gameloc);
		DirectoryInfo directoryInfo = new(s4);
		s4 = directoryInfo.FullName;
		GD.Print(s4);
		if (!Directory.Exists(gameoverridefolder) && Directory.Exists(s4)){
			Directory.CreateDirectory(gameoverridefolder);
		}
		if (!Directory.Exists(s4)){
			return;
		}
		List<FileInfo> fileinfos = new();
		XmlSerializer packageSerializer = new XmlSerializer(typeof(List<OverriddenList>));

		List<string> packs = new(){
			"Get To Work", "Get Together", "City Living", "Cats & Dogs", "Seasons", "Get Famous", "Island Living", "Discover University", "Eco Lifestyle", "Snowy Escape", "Cottage Living", "High School Years", "Growing Together", "Horse Ranch", "For Rent", "Lovestruck", "Outdoor Retreat", "Spa Day", "Dine Out", "Vampires", "Parenthood", "Jungle Adventure", "StrangerVille", "Realm of Magic", "Star Wars: Journey to Batuu", "Dream Home Decorator", "My Wedding Stories", "Werewolves", "Luxury Party Stuff", "Perfect Patio Stuff", "Cool Kitchen Stuff", "Spooky Stuff", "Movie Hangout Stuff", "Romantic Garden Stuff", "Kids Room Stuff", "Backyard Stuff", "Vintage Glamour Stuff", "Bowling Night Stuff", "Fitness Stuff", "Toddler Stuff", "Laundry Day Stuff", "My First Pet Stuff", "Moshino Stuff Pack", "Tiny Living", "Nifty Knitting", "Paranormal", "Home Chef Hustle Stuff", "Crystal Creations Stuff Pack", "Throwback Fit Kit", "Country Kitchen Kit", "Bust The Dust Kit", "Courtyard Oasis Kit", "Fashion Street-Set", "Industrial Loft Kit", "Incheon Arrivals Kit", "Modern Menswear Kit", "Blooming Rooms Kit", "Carnaval Streetwear Kit", "Decor to the Max Kit", "Moonlight Chic Kit", "Little Campers Kit", "First Fits Kit", "Desert Luxe Kit", "Pastel Pop Kit", "Everyday Clutter Kit", "Simtimates Collection Kit", "Bathroom Clutter Kit", "Greenhouse Haven Kit", "Basement Treasures Kit", "Grunge Revival Kit", "Book Nook Kit", "Poolside Splash Kit", "Modern Luxe Kit", "Castle Estate Kit", "Goth Galore Kit", "Urban Homage Kit", "Party Essentials Kit", "Riviera Retreat Kit", "Cozy Bistro Kit", "Artist Studio Kit", "Storybook Nursery Kit", "Holiday Celebration"
		};

		foreach (string dir in Directory.GetDirectories(s4)){
			List<OverriddenList> Sims4Overrides = new();
			fileinfos = new();
			foreach (string file in Directory.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories))
			{
				if (file.EndsWith(".package")){
					fileinfos.Add(new(file));
				}
			}
			foreach (FileInfo f in fileinfos){
				Sims4Overrides = new();
				Sims4PackageReader s4pr = new();
				Sims4Overrides = s4pr.GetSims4Overrides(f);
				string pack = packs.Where(x => dir.Contains(x)).First();
				string outputfile = Path.Combine(gameoverridefolder, string.Format("Sims4_{0}.xml", pack));
				//List<OverriddenList> sims4pack = Sims4Overrides.Where(x => x.Pack == pack).ToList();
				if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Pack {0} has {1} entries.", pack, Sims4Overrides.Count));
				if (Sims4Overrides.Count > 200000){				
					int num = 0;
					foreach (var batch in Sims4Overrides.Batch(200000)){
						List<OverriddenList> listver = batch.ToList();
						outputfile = Path.Combine(gameoverridefolder, string.Format("Sims4_{0}_{1}.xml", pack, num));
						StreamWriter infowriter = new(outputfile, append: false);
						packageSerializer.Serialize(infowriter, listver);
						infowriter.Close();
						num++;
					}
				} else {
					StreamWriter infowriter = new(outputfile, append: false);
					packageSerializer.Serialize(infowriter, Sims4Overrides);
					infowriter.Close();
				}
			}
		}		
	}

	private void GetS2Overrides(){
		List<string> packs = new(){
			"Base Game", "Bon Voyage", "Apartment Life", "Celebration! Stuff", "Family Fun Stuff", "FreeTime", "FreeTime", "Glamor Life Stuff", "Glamor Life Stuff", "H&M Fashion Stuff", "IKEA Home Stuff", "Kitchen & Bath Interior Design Stuff", "Mansion and Garden Stuff", "Nightlife", "Open for Business", "Fun with Pets", "Pets", "Seasons", "Teen Style Stuff", "University"
		};
		string gameoverridefolder = Path.Combine(GlobalVariables.overridesfolder, "Sims 2");
		string gameloc = @"SOFTWARE\WOW6432Node\EA GAMES\The Sims 2";  
		string s2 = Utilities.GetPathForExe(gameloc);
		DirectoryInfo directoryInfo = new(s2);
		s2 = directoryInfo.Parent.FullName;
		if (!Directory.Exists(gameoverridefolder) && Directory.Exists(s2)){
			Directory.CreateDirectory(gameoverridefolder);
		}
		if (!Directory.Exists(s2)){
			return;
		}
		List<OverriddenList> Sims2Overrides = new();
		List<FileInfo> fileinfos = new();
		XmlSerializer packageSerializer = new XmlSerializer(typeof(List<OverriddenList>));
		foreach (string dir in Directory.GetDirectories(s2)){
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Reading directory {0}", dir));
			fileinfos = new();
			foreach (string file in Directory.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories))
			{
				if (file.EndsWith(".package")){
					if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Directory {0}: Added file {1} to list.", dir, file));
					fileinfos.Add(new(file));
				}
			}
			foreach (FileInfo f in fileinfos){
				if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Directory {0}: Reading file {1}.", dir, f.Name));
				int num = 0;
				Sims2Overrides = new();
				Sims2PackageReader s2pr = new();
				//Sims2Overrides = s2pr.GetSims2Overrides(f);
				string pack = packs.Where(x => Sims2Overrides[0].Pack.Contains(x)).First();
				string outputfile = Path.Combine(gameoverridefolder, string.Format("Sims2_{0}.xml", pack));
				if (Sims2Overrides.Count > 200000){			
					if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Directory {0}: File returned with {1} overrides. Splitting.", dir, Sims2Overrides.Count));		
					foreach (var batch in Sims2Overrides.Batch(200000)){
						List<OverriddenList> listver = batch.ToList();
						outputfile = Path.Combine(gameoverridefolder, string.Format("Sims2_{0}_{1}.xml", pack, num));
						StreamWriter infowriter = new(outputfile, append: false);
						packageSerializer.Serialize(infowriter, listver);
						infowriter.Close();
						num++;
					}
				} else {
					if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Directory {0}: Adding overrides from {1} to XML.", dir, f.Name));
					outputfile = Path.Combine(gameoverridefolder, string.Format("Sims2_{0}_{1}.xml", pack, num));
					StreamWriter infowriter = new(outputfile, append: false);
					packageSerializer.Serialize(infowriter, Sims2Overrides);
					infowriter.Close();
				}
				num++;
			}
		}	
	}

	private void TaliEmitted(){
		GetNode<MarginContainer>("Tali").Visible = LoadedSettings.SetSettings.ShowTali;
	}
	private void DebugEmitted(){
		GetNode<MarginContainer>("Menu/MarginContainer/VBoxContainer/MMButton_DevTest").Visible = LoadedSettings.SetSettings.DebugMode;
	}
	private void CancelledInstance(){
		MainMenuContainer.Visible = true;
	}
}
