using Godot;
using SimsCCManager.UI.Utilities;
using System;

public partial class ViewErrors_Container : MarginContainer
{
	public bool HasNotifications = false;
	public string Errors = "";
	private bool _hasnotifications;
	private string _errors = "";
	Button button;
	bool InCell = false;
	string tttext = "";
	Timer ToolTipTimer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ToolTipTimer = new();
		GetNode<MarginContainer>("Notification").Visible = HasNotifications;
		button = GetNode<Button>("View Errors_SettingsHelpButtons/TopbarButton_Button");
		button.MouseEntered += () => MouseEntered();
		button.MouseExited += () => MouseExited();
		AddChild(ToolTipTimer);
		ToolTipTimer.OneShot = true;
		ToolTipTimer.WaitTime = 0.5;
		ToolTipTimer.Timeout += () => SpawnTooltip();	
	}

	private void MouseEntered(){
		InCell = true;
		if (HasNotifications) ToolTipTimer.Start();
	}

	private void MouseExited(){
		InCell = false;
	}

	private void SpawnTooltip(){
		if (InCell){
			ToolTip tooltip = UIUtilities.CustomTooltip(tttext, GetGlobalMousePosition());
			GetWindow().AddChild(tooltip);			
		}
	}

    public override void _Process(double delta)
    {
        if (HasNotifications != _hasnotifications){
			GetNode<MarginContainer>("Notification").Visible = HasNotifications;
		}
		if (Errors != _errors){
			tttext = Errors;
		}
    }
}
