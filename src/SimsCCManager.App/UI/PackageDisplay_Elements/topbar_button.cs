using Godot;
using SimsCCManager.Settings.Loaded;
using SimsCCManager.UI.Themes;
using SimsCCManager.UI.Utilities;
using System;

public partial class topbar_button : MarginContainer
{
	[Export]
	Texture2D buttonImage =new();	
	public delegate void ButtonClickedEvent();
	public ButtonClickedEvent ButtonClicked;
	[Export]
	public string Tooltip = "";
	bool InCell = false; 
	Timer TooltipTimer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TooltipTimer = new();
		TooltipTimer.OneShot = true;
		TooltipTimer.WaitTime = 0.3;
		TooltipTimer.Timeout += () => ShowTooltip();
		AddChild(TooltipTimer);
		SetColors();
		TextureRect textureRect = GetNode<TextureRect>("TextureRect");
		textureRect.Texture = buttonImage;
		GetNode<ColorRect>("TextureRect/Color_Main").Visible = true;
		GetNode<ColorRect>("TextureRect/Color_Hover").Visible = false;
		GetNode<ColorRect>("TextureRect/Color_Click").Visible = false;
		GetNode<Button>("TopbarButton_Button").TooltipText = Tooltip;
		TooltipText = Tooltip;
	}

    private void ShowTooltip()
    {
        if (InCell) {
			ToolTip tooltip = UIUtilities.CustomTooltip(TooltipText, GetGlobalMousePosition());
			GetWindow().AddChild(tooltip);
		}
    }

    public void SetColors(){
		ThemeColors themeColors = LoadedSettings.SetSettings.LoadedTheme;
		GetNode<ColorRect>("TextureRect/Color_Main").Color = themeColors.ButtonMain;
		GetNode<ColorRect>("TextureRect/Color_Hover").Color = themeColors.ButtonHover;
		GetNode<ColorRect>("TextureRect/Color_Click").Color = themeColors.ButtonClick;
	}

	private void _on_topbar_button_button_mouse_entered(){
		InCell = true;
		TooltipTimer.Start();
		GetNode<ColorRect>("TextureRect/Color_Main").Visible = false;
		GetNode<ColorRect>("TextureRect/Color_Hover").Visible = true;
		GetNode<ColorRect>("TextureRect/Color_Click").Visible = false;
	}
	private void _on_topbar_button_button_mouse_exited(){
		InCell = false;
		GetNode<ColorRect>("TextureRect/Color_Main").Visible = true;
		GetNode<ColorRect>("TextureRect/Color_Hover").Visible = false;
		GetNode<ColorRect>("TextureRect/Color_Click").Visible = false;		
	}

	private void _on_topbar_button_button_pressed(){
		ButtonClicked.Invoke();
	}
}
