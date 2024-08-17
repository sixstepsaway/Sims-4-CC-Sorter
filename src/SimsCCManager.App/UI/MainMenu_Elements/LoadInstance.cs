using Godot;
using SimsCCManager.Settings.Loaded;
using System;

public partial class LoadInstance : MarginContainer
{
	[Signal]
	public delegate void LoadInstanceStartPackageManagerEventHandler();
	PackedScene instancepickerbox = GD.Load<PackedScene>("res://UI/MainMenu_Elements/InstancePicker.tscn");
	string instancechosen = "";
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		VBoxContainer instancepicker = GetNode<VBoxContainer>("VBoxContainer/MarginContainer/ScrollContainer/GamePicker");
		foreach (Instance instance in LoadedSettings.SetSettings.Instances){
			var ipb = instancepickerbox.Instantiate() as InstancePicker;
			if (instance.Game == "Sims1"){
				ipb.game = Games.Sims1;
			} else if (instance.Game == "Sims2"){
				ipb.game = Games.Sims2;
			} else if (instance.Game == "Sims3"){
				ipb.game = Games.Sims3;
			} else if (instance.Game == "Sims4"){
				ipb.game = Games.Sims4;
			} else if (instance.Game == "Sims Medieval" || instance.Game == "SimsMedieval"){
				ipb.game = Games.SimsMedieval;
			} else if (instance.Game == "SimCity5" || instance.Game == "SimCity 5"){
				ipb.game = Games.SimCity5;
			} 
			ipb.instancename = instance.Name;
			ipb.instanceidentifier = instance.Identifier.ToString();
			ipb.Connect("PickedInstance", new Callable(this, "PickedInstance"));
			instancepicker.AddChild(ipb);			
		}
	}	

	private void PickedInstance(string instance){
		instancechosen = instance;
	}

	private void _on_cancel_button_button_clicked(){
		QueueFree();
	}

	private void _on_confirm_button_button_clicked(){
		if (instancechosen != "") EmitSignal("LoadInstanceStartPackageManager", instancechosen);
	}
}
