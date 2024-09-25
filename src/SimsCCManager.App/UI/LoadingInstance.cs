using Godot;
using SimsCCManager.Debugging;
using SimsCCManager.Globals;
using System;

public partial class LoadingInstance : Control
{
	public delegate void ReadyLoadEvent();
	public ReadyLoadEvent ReadyLoad;
	public int pbarmax = 100;
	public int pbarvalue = 0;

	ProgressBar pbar;
	public string message = "";
	Label messagelabel;
	bool processed = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Loading screen readying."));
		messagelabel = GetNode<Label>("ProgressBar/VBoxContainer/Label");
		pbar = GetNode<ProgressBar>("ProgressBar/VBoxContainer/HBoxContainer/MarginContainer/ProgressBar");
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Debug line ;~;"));
		Timer time = new();
		AddChild(time);
		time.Timeout += () => Load();
		time.OneShot = true;
		time.Start(1);
	}

	private void Load(){
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Letting it know we're ready!"));
		ReadyLoad.Invoke();
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (pbar.MaxValue != pbarmax) pbar.MaxValue = pbarmax;
		if (pbar.Value != pbarvalue) pbar.Value = pbarvalue;
		if (messagelabel.Text != message) messagelabel.Text = message;
	}
}
