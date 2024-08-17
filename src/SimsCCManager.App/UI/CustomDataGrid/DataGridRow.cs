using Godot;
using SimsCCManager.Debugging;
using SimsCCManager.Globals;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class DataGridRow : MarginContainer
{
	[Signal]
	public delegate void ItemSelectedEventHandler();
	[Signal]
	public delegate void ItemUnselectedEventHandler();
	[Signal]
	public delegate void ItemEnabledEventHandler();
	[Signal]
	public delegate void ItemDisabledEventHandler();
	[Signal]
	public delegate void TextEditedEventHandler();
	HBoxContainer Row;
	ColorRect BackgroundColor;
	ColorRect SelectedColor;
	public Color rowcolor = Color.FromHtml("FFFFFF");
	public Color selectedcolor = Color.FromHtml("FFFFFF");
	public string Identifier = "";
	public bool Selected = false;
	public bool Enabled = false;
	PackedScene Cell = GD.Load<PackedScene>("res://UI/CustomDataGrid/DataGridCell.tscn");
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
					
	}

	public void AddCell(CellContent content, Vector2 columnsize){
		BackgroundColor = GetNode<ColorRect>("Colors/BGColor");
		SelectedColor = GetNode<ColorRect>("Colors/SelectedColor");
		BackgroundColor.Color = rowcolor;
		SelectedColor.Color = selectedcolor;
		SetMeta("Identifier", Identifier);
		Selected = content.Selected;
		Row = GetNode<HBoxContainer>("Row");
		DataGridCell cell = Cell.Instantiate() as DataGridCell;		
		if (content.Icons == true){
			cell.Icons = true;
			cell.iconOptions = content.IconOptions;
		}
		if (content.CellType == CellOptions.Text){
			cell.Text = true;
			cell.TextContent = content.Content;
		}
		if (content.CellType == CellOptions.TrueFalse){
			cell.TrueFalse = true;
			cell.IsTrue = bool.Parse(content.Content);
			if (content.ContentType == "Enabled"){
				cell.Enabled = true;
				cell.IsEnabled = bool.Parse(content.Content);
			}
		}
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Cell size to add: {0}", columnsize));
		cell.Size = columnsize;
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Cell size: {0}", cell.Size));
		cell.Connect("DataGridCellEnabledClicked", new Callable(this, "DetectedClickEnabled"));
		cell.Connect("DataGridCellSelectedClicked", new Callable(this, "DetectedClickSelected"));
		Row.AddChild(cell);
	}

	public void ToggleEnabled(bool enabled){
		Enabled = enabled;
		(Row.GetChild(0) as DataGridCell).IsEnabled = enabled;
	}
	
	private void DetectedClickEnabled(){
		if (!Enabled){
			EmitSignal("ItemEnabled", GetMeta("Identifier"), GetIndex());
		} else {
			EmitSignal("ItemDisabled", GetMeta("Identifier"), GetIndex());
		}
	}
	private void DetectedClickSelected(){
		if (!Selected){
			EmitSignal("ItemSelected", GetMeta("Identifier"), GetIndex());
		} else {
			EmitSignal("ItemUnselected", GetMeta("Identifier"), GetIndex());
		}
	}
	private void DetectedTextEdited(string text, int idx){
		EmitSignal("TextEdited", Identifier, text, idx);
	}

    private void _on_button_select_pressed(){		
		//BackgroundColor.Visible = !Selected;
		//SelectedColor.Visible = Selected;		
	}

	public void ShowAsWrongGame(){
		GetNode<ColorRect>("Colors/WrongGame").Visible = true;
	}

	public void ShowAsBroken(){
		GetNode<ColorRect>("Colors/Broken").Visible = true;
	}

    public override void _Process(double delta)
    {
        if (Selected != SelectedColor.Visible){
			BackgroundColor.Visible = !Selected;
			SelectedColor.Visible = Selected;
		}
    }
}
