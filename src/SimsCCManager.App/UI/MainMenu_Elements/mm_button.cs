using Godot;
using SimsCCManager.Settings.Loaded;
using SimsCCManager.UI.Themes;
using System;

public partial class mm_button : MarginContainer
{
	[Export]
	public string ButtonName { get; set; } = "";
	[Export]
	Texture2D buttonImage =new();	
	[Signal]
	public delegate void ButtonClickedEventHandler();
	ColorRect MainColor;
	ColorRect HoverColor;
	ColorRect ClickColor;

	public override void _Ready()
	{
		GetNode<Label>("Node2D/MMContainer/PanelContainer/MarginContainer/Label").Text = ButtonName;
		TextureRect textureRect = GetNode<TextureRect>("Node2D/MMContainer/MarginContainer/TextureRect");
		textureRect.Texture = buttonImage;
		MainColor = GetNode<ColorRect>("Node2D/MMContainer/MarginContainer/TextureRect/Color_Main");
		MainColor.Visible = true;
		HoverColor = GetNode<ColorRect>("Node2D/MMContainer/MarginContainer/TextureRect/Color_Hover");
		HoverColor.Visible = false;
		ClickColor = GetNode<ColorRect>("Node2D/MMContainer/MarginContainer/TextureRect/Color_Click");
		ClickColor.Visible = false;
		UpdateColors();
	}

	private void _on_button_mouse_entered(){
		GetNode<ColorRect>("Node2D/MMContainer/MarginContainer/TextureRect/Color_Main").Visible = false;
		GetNode<ColorRect>("Node2D/MMContainer/MarginContainer/TextureRect/Color_Hover").Visible = true;
		GetNode<ColorRect>("Node2D/MMContainer/MarginContainer/TextureRect/Color_Click").Visible = false;
		Godot.Vector2 scale = new((float)1.05, (float)1.05);
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(GetNode<Node2D>("Node2D"), "scale", scale, 0.5f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
		tween.Play();
	}

	private void _on_button_mouse_exited(){
		GetNode<ColorRect>("Node2D/MMContainer/MarginContainer/TextureRect/Color_Main").Visible = true;
		GetNode<ColorRect>("Node2D/MMContainer/MarginContainer/TextureRect/Color_Hover").Visible = false;
		GetNode<ColorRect>("Node2D/MMContainer/MarginContainer/TextureRect/Color_Click").Visible = false;		
		Godot.Vector2 scale = new((float)1, (float)1);
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(GetNode<Node2D>("Node2D"), "scale", scale, 0.5f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
		tween.Play();
	}	

	public void UpdateColors(){
		ThemeColors theme = LoadedSettings.SetSettings.LoadedTheme;
		MainColor.Color = theme.ButtonMain;
		HoverColor.Color = theme.ButtonHover;
		ClickColor.Color = theme.ButtonClick;
	}
	
	private void _on_button_pressed(){
		EmitSignal("ButtonClicked");
	}

}
