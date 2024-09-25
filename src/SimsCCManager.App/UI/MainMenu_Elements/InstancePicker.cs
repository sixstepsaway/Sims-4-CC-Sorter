using Godot;
using SimsCCManager.Debugging;
using SimsCCManager.Globals;
using System;

public partial class InstancePicker : MarginContainer
{
	Texture2D iconimage;

	public string instancename = "";

	public Games game = Games.Null;
	public delegate void PickedInstanceEvent(string instance);
	public PickedInstanceEvent PickedInstance;
	public string instanceidentifier = "";
	bool picked = false;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<Label>("Item/MarginContainer2/Label2").Text = instancename;
		GetNode<TextureRect>("Item/MarginContainer/TextureRect").Texture = iconimage;
		if (game == Games.Sims1) iconimage = GD.Load<Texture2D>("res://assets/images/Sims-icon-2812816478.png");
		if (game == Games.Sims2) iconimage = GD.Load<Texture2D>("res://assets/images/s2.png");
		if (game == Games.Sims3) iconimage = GD.Load<Texture2D>("res://assets/images/s3.png");
		if (game == Games.Sims4) iconimage = GD.Load<Texture2D>("res://assets/images/s4.png");
		if (game == Games.SimsMedieval) iconimage = GD.Load<Texture2D>("res://assets/images/TSM_Icon-2854286609.png");
		if (game == Games.SimCity5) iconimage = GD.Load<Texture2D>("res://assets/images/simcity_icon_by_dudekpro_d7n82am-fullview-2356837741.png");
	}
	public void Untoggle(){
		GetNode<Button>("Button").ButtonPressed = false;
		picked = false;
	}

	private void _on_button_pressed(){
		if (picked){
			picked = !picked;
			PickedInstance.Invoke("");
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Picked Game: {0}", "Unpicked"));
		} else {
			picked = !picked;
			PickedInstance.Invoke(instanceidentifier);
		}
		int mypos = GetIndex();
		for (int i = 0; i < GetParent().GetChildCount(); i++){
			if (i != mypos){
				(GetParent().GetChild(i) as InstancePicker).Untoggle();
			}
		}
	}
}
