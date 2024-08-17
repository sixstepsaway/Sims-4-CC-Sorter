using Godot;
using SimsCCManager.Settings.Loaded;
using System;

public partial class Background : Control
{
	ColorRect bgcolor;
	ColorRect patterncolor;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		bgcolor = GetNode<ColorRect>("BG_Color");
		patterncolor = GetNode<ColorRect>("BG_Pattern/BG_PatternColor");
		ChangeTheme();
	}

	public void ChangeTheme(){
		bgcolor.Color = LoadedSettings.SetSettings.LoadedTheme.BackgroundColor;
		if (bgcolor.Color.Luminance < 0.5){
			patterncolor.Color = bgcolor.Color.Lightened(0.2f);
		} else {
			patterncolor.Color = bgcolor.Color.Darkened(0.2f);
		}
	}
}
