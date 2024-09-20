using Godot;
using System;

public partial class ToolTip : Control
{
	public string Text = "";
	public Vector2I WindowPosition = Vector2I.Zero;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Label label = GetNode<Label>("LabelHolder/Control/MarginContainer/text");
		Node2D labelholder = GetNode<Node2D>("LabelHolder");
		Button button = GetNode<Button>("Button");
		label.Text = Text;
		labelholder.Position = WindowPosition;
		button.MouseEntered += () => CloseTooltip();
	}

	private void CloseTooltip(){
		QueueFree();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
