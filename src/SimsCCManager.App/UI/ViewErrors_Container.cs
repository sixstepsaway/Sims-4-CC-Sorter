using Godot;
using System;

public partial class ViewErrors_Container : MarginContainer
{
	public bool HasNotifications { get; set; } = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<MarginContainer>("Notification").Visible = HasNotifications;
	}
}
