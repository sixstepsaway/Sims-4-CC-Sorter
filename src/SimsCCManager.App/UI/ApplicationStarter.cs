using Godot;
using SimsCCManager.Globals;
using System;
using System.Threading;

public partial class ApplicationStarter : Control
{
	[Signal]
	public delegate void ApplicationClosedEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	
	public void Start(string path, string arguments){
		string output = "";
		new Thread(() => {
			output = Utilities.RunProcess(path, arguments);
			CallDeferred("EmitClosed");
		}){IsBackground = true}.Start();
	}

	private void EmitClosed(){
		EmitSignal("ApplicationClosed");
	}
}
