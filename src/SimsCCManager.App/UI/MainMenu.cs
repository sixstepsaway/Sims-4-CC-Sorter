using Godot;
using SimsCCManager.Containers;
using SimsCCManager.Debugging;
using SimsCCManager.Globals;
using SimsCCManager.Settings.Loaded;
using System;
using System.Linq;

public partial class MainMenu : MarginContainer
{
	PackedScene settingswindow = GD.Load<PackedScene>("res://UI/MainMenu_Elements/main_settings.tscn");
	MarginContainer NewInstanceMenu;
	MarginContainer MainMenuContainer;
	[Signal]
	public delegate void MainMenuStartInstanceEventHandler();
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

	private void _on_mm_button_new_instance_button_clicked(){
		var newinstance = newinstancemenu.Instantiate();
		newinstance.Connect("tree_exited", Callable.From(CancelledInstance));
		newinstance.Connect("NewInstanceStartPackageManager", new Callable(this, "LoadInstance"));
		MainMenuContainer.Visible = false;		
		AddChild(newinstance);
	}
	private void _on_mm_button_load_instance_button_clicked(){
		MainMenuContainer.Visible = false;
		var loadinstance = loadinstancemenu.Instantiate();
		loadinstance.Connect("tree_exited", Callable.From(CancelledInstance));
		loadinstance.Connect("LoadInstanceStartPackageManager", new Callable(this, "LoadInstance"));
		AddChild(loadinstance);
	}

	private void LoadInstance(string instance){
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Heard the signal to load instance on Main Menu!"));
		EmitSignal("MainMenuStartInstance", instance);
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
		Sims2Instance instance = new();
		instance.Load(@"E:\Documents\Sims CC Manager\Instances\The Sims 2\Instance.ini");
		instance.TestInstance();
		instance.InstanceDataFolder = "testing";
		instance.TestInstance();
		instance.SetProperty("InstanceDataFolder", "this is my second test");
		instance.TestInstance();
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
