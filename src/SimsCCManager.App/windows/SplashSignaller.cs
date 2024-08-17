using Godot;
using SimsCCManager.Debugging;
using SimsCCManager.Globals;
using SimsCCManager.Settings.SettingsSystem;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Threading;

public partial class SplashSignaller : Control
{
	
	[Signal]
	public delegate void UpdatePbarEventHandler();
	[Signal]
	public delegate void PbarLabelEventHandler();
	[Signal]
	public delegate void FinishLoadingEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		int taskcount = 11;
		int pbarinc = 100 / taskcount;
		new Thread (() => {
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Running log."));
			Thread.Sleep(300);
			Logging.WriteDebugLog("Initializing Sims CC Manager", pbarinc);
			Thread.Sleep(300);
			CallDeferred("Pbar", "Initializing Manager", pbarinc);
			Thread.Sleep(300);
			CallDeferred("Pbar", "Initializing Components", pbarinc);
			Thread.Sleep(300);
			CallDeferred("Pbar", "Checking for Splines", pbarinc);
			Thread.Sleep(300);
			CallDeferred("Pbar", "Finding Data", pbarinc);
			Thread.Sleep(300);
			CallDeferred("Pbar", "Initializing Data", pbarinc);
			Thread.Sleep(300);
			CallDeferred("Pbar", "Checking for Instances", pbarinc);
			Thread.Sleep(300);
			CallDeferred("Pbar", "Loading Themes", pbarinc);
			SettingsFileManagement.LoadThemes();
			Thread.Sleep(300);
			CallDeferred("Pbar", "Setting Settings", pbarinc);
			SettingsFileManagement.LoadSettings();			
			Thread.Sleep(300);
			CallDeferred("Pbar", "Loading Application", pbarinc);
			Thread.Sleep(300);
			CallDeferred("Pbar", "Finishing Up", pbarinc);
			Thread.Sleep(300);
			CallDeferred("Conclude");
		}){IsBackground = true}.Start();
	}
	
	private void Pbar(string label, int amount){
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Updating pbar: {0}", label));
		EmitSignal("UpdatePbar", amount);
		EmitSignal("PbarLabel", string.Format("{0}...", label));	
	}

	private void fixpbar(ProgressBar pbar){		
		if (pbar.Value < 100){
			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Pbar not at 100, fixing."));
			while (pbar.Value < 100){
				if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Incrementing pbar from {0} to {1}.", pbar.Value, pbar.Value + 1));
				new Thread (() => {
					CallDeferred("Pbar", "Reticulating final splines");
					Thread.Sleep(300);
				}){IsBackground = true}.Start();
			}
		} else {
			Conclude();
		}
	}

	private void Conclude(){
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Concluding."));
		EmitSignal("FinishLoading");		
	}
}
