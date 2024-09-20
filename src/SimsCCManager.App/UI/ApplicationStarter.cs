using Godot;
using SimsCCManager.Globals;
using SimsCCManager.Packages.Containers;
using System;
using System.Collections.Generic;
using System.Threading;

public partial class ApplicationStarter : Control
{
	[Signal]
	public delegate void ApplicationClosedEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	
	public void Start(string path, string arguments, List<SimsPackage> enabledpackages, Games game){
		string output = "";
		new Thread(() => {

			



			output = Utilities.RunProcess(path, arguments, game);
			CallDeferred("EmitClosed");
		}){IsBackground = true}.Start();
	}

	private void EmitClosed(){
		EmitSignal("ApplicationClosed");
	}
}
