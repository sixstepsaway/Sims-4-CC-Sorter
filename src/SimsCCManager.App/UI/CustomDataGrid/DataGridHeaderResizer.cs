using Godot;
using System;

public partial class DataGridHeaderResizer : VSeparator
{
	public bool Enabled {get; set;} = true;
	private bool dragging = false;
	private Vector2 MousePosition;
	private int NodePos = -1;
	Control Container;
	float boxleft;
	float boxright;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (!Enabled){
			GetNode<Button>("DataGridHeader_Resizer_Button").Visible = false;
		}
	}

	private void _on_data_grid_header_resizer_button_button_down(){
		MousePosition = GetLocalMousePosition();
		Container = GetParent().GetParent<Control>();
		if (Enabled){
			dragging = true;
		}		
	}

	private void _on_data_grid_header_resizer_button_button_up(){
		dragging = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (dragging){
			if (MousePosition != GetLocalMousePosition()){					
				if (MousePosition.X > GetLocalMousePosition().X){
					float x = GetLocalMousePosition().X - MousePosition.X;
					//MarginContainer leftnode = ContainingHbox.GetChildren()[NodePos - 1] as MarginContainer;
					float sizex = Container.Size.X;
					float sizey = Container.Size.Y;
					//if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Node to the left size: {0} x {1}", sizex, sizey));
					Vector2 newsize = new Vector2(sizex + x, sizey);
					//if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Node to the left new size: {0} x {1}", newsize.X, newsize.Y));
					Container.Set("custom_minimum_size", newsize);					
				} else {
					float x = MousePosition.X - GetLocalMousePosition().X;
					//MarginContainer leftnode = ContainingHbox.GetChildren()[NodePos - 1] as MarginContainer;
					float sizex = Container.Size.X;
					float sizey = Container.Size.Y;
					//if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Node to the left size: {0} x {1}", sizex, sizey));
					Vector2 newsize = new Vector2(sizex - x, sizey);
					//if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Node to the left new size: {0} x {1}", newsize.X, newsize.Y));
					Container.Set("custom_minimum_size", newsize);
				}			
				MousePosition = GetLocalMousePosition();
			}
		}
	}







}
