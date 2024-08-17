using Godot;
using System;

public partial class Exe_Option : MarginContainer
{
	[Signal]
	public delegate void SelectedExeEventHandler();

	public Texture2D Icon;
	public string ExeName;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<TextureRect>("Hbox/Exe_Icon/TextureRect").Texture = Icon;
		GetNode<Label>("Hbox/Exe_Title/Label").Text = ExeName;
	}

	private void _on_exe_choice_button_pressed(){
		EmitSignal("SelectedExe", GetMeta("ExeID").ToString());
	}

}
