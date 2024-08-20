using Godot;
using SimsCCManager.Debugging;
using SimsCCManager.Globals;
using SimsCCManager.Settings.Loaded;
using SimsCCManager.UI.Themes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class MainWindow : MarginContainer
{
	PackedScene Splash = GD.Load<PackedScene>("res://windows/SplashScreen.tscn");
	PackedScene MainMenu = GD.Load<PackedScene>("res://UI/MainMenu.tscn");
	PackedScene PackageDisplay = GD.Load<PackedScene>("res://UI/PackageDisplay.tscn");
	// Called when the node enters the scene tree for the first time.
	Node splashinsance;
	MarginContainer footerpbar;
	ProgressBar footerpbarbar;
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
		
		if (LoadedSettings.SetSettings.InstanceLoaded){
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Instantiating package manager."));
			var pd = PackageDisplay.Instantiate();
			pd.Connect("SetPbarMax", new Callable(this, "SetPbarMax"));
			pd.Connect("IncrementPbar", Callable.From(IncrementPbar));
			pd.Connect("ResetPbarValue", Callable.From(ResetPbarValue));
			pd.Connect("ShowPbar", Callable.From(ShowPbar));
			pd.Connect("HidePbar", Callable.From(HidePbar));
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Adding package manager."));
			AddChild(pd);
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Moving package manager."));
			MoveChild(pd, 0);
		} else {
			ShowMainMenu();
		}
		UpdateTheme();
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Disabling transparency."));
		GetTree().Root.TransparentBg = false;
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Disabling borderless."));
		GetTree().Root.Borderless = false;
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Showing window."));
		Show();
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Removing splash."));
		sc.QueueFree();
		UpdateTheme();
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
		GetChild(0).QueueFree();
		var pd = PackageDisplay.Instantiate() as PackageDisplay;
		pd.Connect("SetPbarMax", new Callable(this, "SetPbarMax"));
		pd.Connect("IncrementPbar", Callable.From(IncrementPbar));
		pd.Connect("ResetPbarValue", Callable.From(ResetPbarValue));
		pd.Connect("ShowPbar", Callable.From(ShowPbar));
		pd.Connect("HidePbar", Callable.From(HidePbar));
		pd.ThisInstance = LoadedSettings.SetSettings.Instances.Where(x => x.Identifier == Guid.Parse(instance)).First();	
		LoadedSettings.SetSettings.InstanceLoaded = true;
		LoadedSettings.SetSettings.CurrentInstance = pd.ThisInstance;
		LoadedSettings.SetSettings.LastInstanceLoaded = pd.ThisInstance.InstanceLocation;
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Adding package manager."));
		AddChild(pd);
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Moving package manager."));
		MoveChild(pd, 0);
		UpdateTheme();
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

	public void UpdateTheme(){
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog("Updating theme!");
		bool islight = false;
		ThemeColors theme = LoadedSettings.SetSettings.LoadedTheme;
		if (theme.BackgroundColor.Luminance > 0.5) islight = true;		
		bool accentlight = false;
		if (theme.AccentColor.Luminance > 0.5) accentlight = true;
		Color fontcolor = Color.FromHtml("F7F7F7");
		if (accentlight) {
			fontcolor = Color.FromHtml("06090E");
		}
		List<Node> labels = GetTree().GetNodesInGroup("AccentLabels").ToList();
		foreach (Label label in labels){
			label.AddThemeColorOverride("font_color", fontcolor);
		}
		
		List<Node> accentbuttons = GetTree().GetNodesInGroup("AccentTextButtons").ToList();
		foreach (Button textbutton in accentbuttons){
			textbutton.AddThemeColorOverride("font_color", fontcolor);
		}
		
		List<Node> textbuttons = GetTree().GetNodesInGroup("TextButtons").ToList();
		foreach (Button button in textbuttons){
			button.AddThemeColorOverride("font_color", fontcolor);
		}
		List<Node> socials = GetTree().GetNodesInGroup("SocialsButtons").ToList();
		foreach (socials_button button in socials){
			button.SetColors();
		}
		List<Node> accents = GetTree().GetNodesInGroup("AccentColorBox").ToList();
		foreach (ColorRect accent in accents){
			accent.Color = theme.AccentColor;
		}
		List<Node> plainbg = GetTree().GetNodesInGroup("PlainBG").ToList();
		foreach (ColorRect bg in plainbg){
			bg.Color = theme.BackgroundColor;
		}
		List<Node> backgrounds = GetTree().GetNodesInGroup("Background").ToList();
		foreach (Background background in backgrounds){
			background.ChangeTheme();
		}
		List<Node> bglabels = GetTree().GetNodesInGroup("Labels").ToList();
		foreach (Label label in bglabels ){
			label.AddThemeColorOverride("font_color", theme.MainTextColor);
		}
		List<Node> checkbuttons = GetTree().GetNodesInGroup("CheckButtons").ToList();
		foreach (CustomCheckButton button in checkbuttons){
			button.UpdateColors();
		}
		List<Node> MainButtons = GetTree().GetNodesInGroup("MainButtons").ToList();
		foreach (Button button in MainButtons){
			button.AddThemeColorOverride("font_color", theme.MainTextColor.Darkened(0.4f));
		}
		List<Node> mmbuttons = GetTree().GetNodesInGroup("MMButtons").ToList();
		foreach (mm_button button in mmbuttons){
			button.UpdateColors();
		}
		List<Node> packageeditorbuttons = GetTree().GetNodesInGroup("PackageEditorButtons").ToList();
		foreach (topbar_button button in packageeditorbuttons){
			button.SetColors();
		}
		List<Node> lineedits = GetTree().GetNodesInGroup("LineEdits").ToList();
		foreach (LineEdit linedit in lineedits){
			linedit.AddThemeColorOverride("font_color", fontcolor);
			StyleBoxFlat sb = linedit.GetThemeStylebox("normal") as StyleBoxFlat;
			sb.BgColor = theme.DataGridA;
			linedit.AddThemeStyleboxOverride("normal", sb);
		}
		List<Node> panels = GetTree().GetNodesInGroup("PanelsWBorders").ToList();
		foreach (Panel panel in panels){
			StyleBoxFlat sb = panel.GetThemeStylebox("panel") as StyleBoxFlat;
			sb.BorderColor = theme.ButtonMain;
			sb.BgColor = theme.BackgroundColor;
			panel.RemoveThemeStyleboxOverride("panel");
			panel.AddThemeStyleboxOverride("panel", sb);
		}
		List<Node> buttonswithborders = GetTree().GetNodesInGroup("ButtonsWithBorders").ToList();
		foreach (Button button in buttonswithborders){
			StyleBoxFlat sb = button.GetThemeStylebox("hover") as StyleBoxFlat;
			sb.BorderColor = theme.ButtonMain;
			button.RemoveThemeStyleboxOverride("hover");
			button.AddThemeStyleboxOverride("hover", sb);
			sb = button.GetThemeStylebox("pressed") as StyleBoxFlat;
			sb.BorderColor = theme.ButtonMain;
			button.RemoveThemeStyleboxOverride("pressed");
			button.AddThemeStyleboxOverride("pressed", sb);
		}

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
