using Godot;
using SimsCCManager.Settings.Loaded;
using SimsCCManager.UI.Themes;
using System;

public partial class CustomCheckButton : MarginContainer
{
	[Signal]
	public delegate void CheckToggledEventHandler();
	[Export]
	public bool IsToggled = false;
	MarginContainer toggledmask;
	MarginContainer untoggledmask;
	ColorRect maincolor;
	ColorRect toggledcolor;
	ColorRect untoggledcolor;

	public override void _Ready()
	{
		toggledmask = GetNode<MarginContainer>("ToggledMask");
		untoggledmask = GetNode<MarginContainer>("UntoggledMask");
		maincolor = GetNode<ColorRect>("Mask/BGColor");
		toggledcolor = GetNode<ColorRect>("ToggledMask/Mask/ToggledBlipColor");
		untoggledcolor = GetNode<ColorRect>("UntoggledMask/Mask/UntoggledBlipColor");
		UpdateColors();
		Toggle();
	}

	private void _on_button_pressed(){
		IsToggled = !IsToggled;
		Toggle();
		EmitSignal("CheckToggled", IsToggled);
		GD.Print("toggled");
	}

	public void ButtonPressed(bool pressed){
		IsToggled = pressed;
		Toggle();
	}

	private void Toggle(){
		toggledmask.Visible = IsToggled;
		untoggledmask.Visible = !IsToggled;		
	}

	public void UpdateColors(){
		bool islight = false;
		ThemeColors theme = LoadedSettings.SetSettings.LoadedTheme;
		if (theme.BackgroundColor.Luminance > 0.5){
			islight = true;
		}
		Color bgc = theme.BackgroundColor.Lightened(0.1f);
		if (islight) bgc = theme.BackgroundColor.Darkened(0.1f);
		maincolor.Color = bgc;
		toggledcolor.Color = theme.ButtonClick;
		untoggledcolor.Color = theme.ButtonMain;
	}
}
