using Godot;
using System;

public partial class DataGridHeaderCell : Control
{
	[Signal]
	public delegate void HeaderResizedEventHandler();
	public string Label = "";
	public Vector2 setSize;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetSize(setSize);
		GetNode<Label>("MarginContainer2/Label").Text = Label;
		GD.Print(string.Format("Header cell width: {0}\n      Label width: {1}", Size.X, GetNode<Label>("MarginContainer2/Label").Size.X));
	}

	public void SetSize(Vector2 size){
		Set("custom_minimum_size", setSize);
		Size = setSize;
	}

	private void _on_resized(){
		EmitSignal("HeaderResized", GetIndex());
	}
}
