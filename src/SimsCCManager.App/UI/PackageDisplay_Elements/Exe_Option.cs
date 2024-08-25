using Godot;
using System;

public partial class Exe_Option : MarginContainer
{
	public delegate void SelectedExeEvent(string exeid);
	public SelectedExeEvent SelectedExe;

	public Texture2D Icon;
	public string ExeName = "";
	public string ExeID = "";
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<TextureRect>("Hbox/Exe_Icon/TextureRect").Texture = Icon;
		GetNode<Label>("Hbox/Exe_Title/Label").Text = ExeName;
	}

	private void _on_exe_choice_button_pressed(){
		SelectedExe.Invoke(ExeID);
		EmitSignal("SelectedExe", GetMeta("ExeID").ToString());
	}

}
