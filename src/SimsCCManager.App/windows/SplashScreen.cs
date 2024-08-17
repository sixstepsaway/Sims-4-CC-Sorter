using Godot;
using SimsCCManager.Debugging;
using SimsCCManager.Globals;
using System;
using System.Threading;

public partial class SplashScreen : MarginContainer
{
	[Signal]
	public delegate void FinishedLoadingEventHandler();
	PackedScene packedScene = GD.Load<PackedScene>("res://windows/MainWindow.tscn");
	ProgressBar progressBar;
	Label pbartext;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Making splash transparent."));
		GetTree().Root.TransparentBg = true;
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Getting pbar."));
		progressBar = GetNode<ProgressBar>("MarginContainer/ProgressBar");	
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Getting pbar label."));
		pbartext = GetNode<Label>("MarginContainer/pbartext");	
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Setting pbar to 0."));
		progressBar.Value = 0;
	}

	private void _on_splash_signaller_finish_loading(){
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Done loading!."));
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Emitting final signal."));
		EmitSignal("FinishedLoading");
	}

	private void _on_splash_signaller_update_pbar(int amount){
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Incrementing pbar to {0}.", progressBar.Value + 1));
		progressBar.Value += amount;
	}

	private void _on_splash_signaller_pbar_label(string label){
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Changing pbar label to {0}.", label));
		pbartext.Text = label;
	}
}
