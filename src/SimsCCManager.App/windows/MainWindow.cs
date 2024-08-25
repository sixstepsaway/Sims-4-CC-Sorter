using Godot;
using SimsCCManager.Debugging;
using SimsCCManager.Globals;
using SimsCCManager.Settings.Loaded;
using SimsCCManager.UI.Themes;
using SimsCCManager.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class MainWindow : MarginContainer
{
	PackedScene Splash = GD.Load<PackedScene>("res://windows/SplashScreen.tscn");
	PackedScene MainMenu = GD.Load<PackedScene>("res://UI/MainMenu.tscn");
	PackedScene PackageDisplay = GD.Load<PackedScene>("res://UI/PackageDisplay.tscn");
	PackedScene LoadingScreen = GD.Load<PackedScene>("res://UI/loading_instance.tscn");
	// Called when the node enters the scene tree for the first time.
	Node splashinsance;
	MarginContainer footerpbar;
	ProgressBar footerpbarbar;

	bool loadingPD = false;
	public override void _Ready()
	{
		footerpbar = GetNode<MarginContainer>("Footer/FooterInternalMargins/FooterHbox/FooterProgressBar");
		footerpbarbar = GetNode<ProgressBar>("Footer/FooterInternalMargins/FooterHbox/FooterProgressBar/VBoxContainer/HBoxContainer/MarginContainer/ProgressBar");
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Hiding footer."));
		GetNode<MarginContainer>("Footer").Visible = false;
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Instantiating splash."));		
		SplashScreen splash = Splash.Instantiate() as SplashScreen;
		splash.Connect("FinishedLoading", Callable.From(FinishedLoading));
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Connected loading call."));
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Making splash permanent."));
		AddChild(splash);
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Loaded splash."));
	}

	private void FinishedLoading(){
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(LoadedSettings.SetSettings.LoadedTheme.ThemeName);
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(LoadedSettings.SetSettings.LoadedTheme.Identifier.ToString());
		var sc = GetChild((GetChildren().Count) - 1);
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Making footer visible."));
		GetNode<MarginContainer>("Footer").Visible = true;
		
		if (LoadedSettings.SetSettings.InstanceLoaded && LoadedSettings.SetSettings.LoadLatestInstance){
			/*if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Instantiating package manager."));
			var pd = PackageDisplay.Instantiate();
			pd.Connect("SetPbarMax", new Callable(this, "SetPbarMax"));
			pd.Connect("IncrementPbar", Callable.From(IncrementPbar));
			pd.Connect("ResetPbarValue", Callable.From(ResetPbarValue));
			pd.Connect("ShowPbar", Callable.From(ShowPbar));
			pd.Connect("HidePbar", Callable.From(HidePbar));
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Adding package manager."));
			AddChild(pd);
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Moving package manager."));
			MoveChild(pd, 0);*/
			StartInstance(LoadedSettings.SetSettings.Instances.Where(x => x.InstanceLocation == LoadedSettings.SetSettings.LastInstanceLoaded).First().Identifier.ToString());
		} else {
			ShowMainMenu();
		}
		//UIUtilities.UpdateTheme(GetTree());
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Disabling transparency."));
		GetTree().Root.TransparentBg = false;
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Disabling borderless."));
		GetTree().Root.Borderless = false;
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Showing window."));
		Show();
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Removing splash."));
		sc.QueueFree();
		UIUtilities.UpdateTheme(GetTree());
	}

	public void ShowMainMenu(){
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Instantiating main menu."));
		var mm = MainMenu.Instantiate();
		mm.Connect("MainMenuStartInstance", new Callable(this, "StartInstance"));
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Adding main menu."));
		AddChild(mm);
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Moving main menu."));
		MoveChild(mm, 0);
	}

	private void StartInstance(string instance){
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Heard the signal to load instance on the main window!"));
		LoadingInstance ls = LoadingScreen.Instantiate() as LoadingInstance;
		ls.message = "Loading instance. Please wait.";		
		GetChild(0).QueueFree();
		AddChild(ls);
		MoveChild(ls, 0);
		var pd = PackageDisplay.Instantiate() as PackageDisplay;
		pd.Visible = false;
		pd.SetPbarMax += (value) => SetPbarMax(value);
		//pd.Connect("SetPbarMax", new Callable(this, "SetPbarMax"));
		pd.IncrementPbar += () => IncrementPbar();
		//pd.Connect("IncrementPbar", Callable.From(IncrementPbar));
		pd.ResetPbarValue += () => ResetPbarValue();
		//pd.Connect("ResetPbarValue", Callable.From(ResetPbarValue));

		pd.DoneLoading += () => PDDoneLoading();
		pd.ShowPbar += () => ShowPbar();
		pd.HidePbar += () => HidePbar();

		//pd.Connect("ShowPbar", Callable.From(ShowPbar));
		//pd.Connect("HidePbar", Callable.From(HidePbar));
		pd.ThisInstance = LoadedSettings.SetSettings.Instances.Where(x => x.Identifier == Guid.Parse(instance)).First();	
		LoadedSettings.SetSettings.InstanceLoaded = true;
		LoadedSettings.SetSettings.CurrentInstance = pd.ThisInstance;
		LoadedSettings.SetSettings.LastInstanceLoaded = pd.ThisInstance.InstanceLocation;
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Adding package manager."));
		AddChild(pd);		
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Moving package manager."));
		MoveChild(pd, 0);	
		loadingPD = true;	
		UIUtilities.UpdateTheme(GetTree());
	}

    private void PDDoneLoading()
    {
        GetNode<LoadingInstance>("LoadingInstance").QueueFree();
		GetNode<PackageDisplay>("PackageDisplay").Visible = true;
    }

    private void _on_twitter_socials_button_clicked(){
		Process.Start(new ProcessStartInfo("http://x.com/sinfulsimming") { UseShellExecute = true });
	}
	private void _on_kofi_socials_button_clicked(){
		Process.Start(new ProcessStartInfo("http://kofi.com/sinfulsimming") { UseShellExecute = true });
	}
	private void _on_tumblr_socials_button_clicked(){
		Process.Start(new ProcessStartInfo("http://tumblr.com/sinfulsimming") { UseShellExecute = true });
	}
	private void _on_github_socials_button_clicked(){
		Process.Start(new ProcessStartInfo("https://github.com/sixstepsaway/Sims-CC-Manager") { UseShellExecute = true });
	}
	private void _on_discord_socials_button_clicked(){
		//Process.Start(new ProcessStartInfo("http://x.com/sinfulsimming") { UseShellExecute = true });
	}
	private void _on_itch_socials_button_clicked(){
		Process.Start(new ProcessStartInfo("https://sixstepsaway.itch.io/") { UseShellExecute = true });
	}

	

	public void ShowPbar(){
		footerpbar.Visible = true;
	}
	public void HidePbar(){
		footerpbar.Visible = false;
	}

	private void SetPbarMax(int max){
		footerpbarbar.MaxValue = max;
	}
	private void IncrementPbar(){
		footerpbarbar.Value++;
	}
	private void ResetPbarValue(){
		footerpbarbar.Value = 0;
	}
}
