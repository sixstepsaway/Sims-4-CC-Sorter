using Godot;
using SimsCCManager.Settings.Loaded;
using SimsCCManager.UI.Themes;
using System;

public partial class socials_button : MarginContainer
{
	[Export]
	Texture2D buttonImage =new();	
	[Signal]
	public delegate void ButtonClickedEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetColors();
		TextureRect textureRect = GetNode<TextureRect>("TextureRect");
		textureRect.Texture = buttonImage;
		GetNode<ColorRect>("TextureRect/Color_Main").Visible = true;
		GetNode<ColorRect>("TextureRect/Color_Hover").Visible = false;
		GetNode<ColorRect>("TextureRect/Color_Click").Visible = false;
	}

	public void SetColors(){
		ThemeColors themeColors = LoadedSettings.SetSettings.LoadedTheme;
		GetNode<ColorRect>("TextureRect/Color_Main").Color = themeColors.ButtonMain;
		GetNode<ColorRect>("TextureRect/Color_Hover").Color = themeColors.ButtonHover;
		GetNode<ColorRect>("TextureRect/Color_Click").Color = themeColors.ButtonClick;
	}

	private void _on_social_button_mouse_entered(){
		GetNode<ColorRect>("TextureRect/Color_Main").Visible = false;
		GetNode<ColorRect>("TextureRect/Color_Hover").Visible = true;
		GetNode<ColorRect>("TextureRect/Color_Click").Visible = false;
	}
	private void _on_social_button_mouse_exited(){
		GetNode<ColorRect>("TextureRect/Color_Main").Visible = true;
		GetNode<ColorRect>("TextureRect/Color_Hover").Visible = false;
		GetNode<ColorRect>("TextureRect/Color_Click").Visible = false;		
	}

	private void _on_social_button_pressed(){
		EmitSignal("ButtonClicked");
	}
}
