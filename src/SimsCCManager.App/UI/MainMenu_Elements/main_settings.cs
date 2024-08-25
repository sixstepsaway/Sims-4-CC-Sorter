using Godot;
using SimsCCManager.Settings.Loaded;
using SimsCCManager.UI.Themes;
using SimsCCManager.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class main_settings : MarginContainer
{
	[Signal]
	public delegate void TaliEventHandler();
	[Signal]
	public delegate void DebugEventHandler();
	OptionButton themes;
	List<Guid> themesList = new();
	CustomCheckButton DebugMode;
	CustomCheckButton RestrictCPU;
	CustomCheckButton AutomaticallyLoad;
	CustomCheckButton ShowTali;
	
	public override void _Ready()
	{
		themes = GetNode<OptionButton>("MarginContainer/VBoxContainer/HBoxContainer2/OptionButton");
		GD.Print(string.Format("There are {0} loaded themes", LoadedSettings.SetSettings.ThemeOptions.Count));
		themes.AddItem(LoadedSettings.SetSettings.LoadedTheme.ThemeName);
		themesList.Add(LoadedSettings.SetSettings.LoadedTheme.Identifier);
		foreach (ThemeColors theme in LoadedSettings.SetSettings.ThemeOptions){
			if (theme.ThemeName != LoadedSettings.SetSettings.LoadedTheme.ThemeName){
				GD.Print(string.Format("Adding {0} to drop-down", theme.ThemeName));
				themes.AddItem(theme.ThemeName);
				themesList.Add(theme.Identifier);
			}
		}
		DebugMode = GetNode<CustomCheckButton>("MarginContainer/VBoxContainer/DebugMode_Setting/DebugMode_Check");
		DebugMode.ButtonPressed(LoadedSettings.SetSettings.DebugMode);
		RestrictCPU = GetNode<CustomCheckButton>("MarginContainer/VBoxContainer/RestrictCPU_Setting/RestrictCPU_Check");
		RestrictCPU.ButtonPressed(LoadedSettings.SetSettings.LimitCPU);
		AutomaticallyLoad = GetNode<CustomCheckButton>("MarginContainer/VBoxContainer/AutomaticallyLoadLatestInstance_Setting/AutomaticallyLoadLatestInstance_Check");
		AutomaticallyLoad.ButtonPressed(LoadedSettings.SetSettings.LoadLatestInstance);
		ShowTali = GetNode<CustomCheckButton>("MarginContainer/VBoxContainer/ShowTali_Setting/ShowTali_Check");
		ShowTali.ButtonPressed(LoadedSettings.SetSettings.ShowTali);
		UIUtilities.UpdateTheme(GetTree());
	}

	private void _on_option_button_item_selected(int option){
		ThemeColors prevtheme = LoadedSettings.SetSettings.LoadedTheme;
		LoadedSettings.SetSettings.ChangeSetting("LoadedTheme", themesList[option]);	
		if (LoadedSettings.SetSettings.LoadedTheme != prevtheme){						
			UIUtilities.UpdateTheme(GetTree());
		}
	}

	private void _on_settings_close_button_pressed(){
		this.QueueFree();
	}

	private void _on_debug_mode_check_pressed(bool toggled){
		LoadedSettings.SetSettings.ChangeSetting("DebugMode", toggled);
		EmitSignal("Debug");
	}
	private void _on_restrict_cpu_check_pressed(bool toggled){
		LoadedSettings.SetSettings.ChangeSetting("LimitCPU", toggled);
	}
	private void _on_automatically_load_latest_instance_check_pressed(bool toggled){
		LoadedSettings.SetSettings.ChangeSetting("LoadLatestInstance", toggled);
	}
	private void _on_show_tali_check_pressed(bool toggled){
		LoadedSettings.SetSettings.ChangeSetting("ShowTali", toggled);
		EmitSignal("Tali");
	}

}
