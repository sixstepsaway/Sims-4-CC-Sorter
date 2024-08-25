using Godot;
using System;

public partial class ViewErrors_Container : MarginContainer
{
	public bool HasNotifications = false;
	public string Errors = "";
	private bool _hasnotifications;
	private string _errors = "";
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<MarginContainer>("Notification").Visible = HasNotifications;
		
	}

    public override void _Process(double delta)
    {
        if (HasNotifications != _hasnotifications){
			GetNode<MarginContainer>("Notification").Visible = HasNotifications;
		}
		if (Errors != _errors){
			TooltipText = Errors;
		}
    }
}
