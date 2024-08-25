using Godot;
using System;

public partial class LoadingInstance : Control
{
	public int pbarmax = 100;
	public int pbarvalue = 0;

	ProgressBar pbar;
	public string message = "";
	Label messagelabel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		messagelabel = GetNode<Label>("ProgressBar/VBoxContainer/Label");
		pbar = GetNode<ProgressBar>("ProgressBar/VBoxContainer/HBoxContainer/MarginContainer/ProgressBar");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (pbar.MaxValue != pbarmax) pbar.MaxValue = pbarmax;
		if (pbar.Value != pbarvalue) pbar.Value = pbarvalue;
		if (messagelabel.Text != message) messagelabel.Text = message;
	}
}
