using Godot;
using SimsCCManager.Debugging;
using SimsCCManager.Globals;
using SimsCCManager.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class DataGridRow : MarginContainer
{
	public delegate void ItemSelectedEvent(string identifier, int idx);
	public ItemSelectedEvent ItemSelected;
	public ItemSelectedEvent ItemDeselected;
	public ItemSelectedEvent ItemEnabled;
	public ItemSelectedEvent ItemDisabled;	
	public delegate void IntChangedEvent(string identifier, int newvalue);
	public IntChangedEvent IntChanged;
	[Signal]
	public delegate void TextEditedEventHandler();
	public delegate void MouseAffectingEvent(bool inside, int idx);
	public MouseAffectingEvent MouseAffected;
	HBoxContainer Row;
	ColorRect BackgroundColor;
	ColorRect SelectedColor;
	public Color rowcolor = Color.FromHtml("FFFFFF");
	public Color selectedcolor = Color.FromHtml("FFFFFF");
	public string Identifier = "";
	public bool Selected = false;
	public bool Enabled = false;
	public int LoadOrder = -1;
	public int Index = -1;
	PackedScene Cell = GD.Load<PackedScene>("res://UI/CustomDataGrid/DataGridCell.tscn");

	public float rowheight = 25f;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("This row size y is: {0}.", Size.Y));
	}

	private void MouseAffecting(bool inside){
		MouseAffected.Invoke(inside, Index);		
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
		cell.SizeY = rowheight;
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Cell size to add: {0}", columnsize));
		cell.SizeX = columnsize.X;
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
				Enabled = cell.IsEnabled;
			}
		}
		if (content.CellType == CellOptions.Int){
			cell.Int = true;
			cell.IntContent = content.Content;
		}
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Cell size: {0}", cell.Size));
		
		cell.Connect("DataGridCellEnabledClicked", new Callable(this, "DetectedClickEnabled"));
		cell.Connect("DataGridCellSelectedClicked", new Callable(this, "DetectedClickSelected"));
		cell.ClickedCell += () => ClickedCell();
		cell.MouseEvent += (inside) => MouseAffecting(inside);
		cell.CellChangeInt += (val) => CellIntChanged(val);
		Row.AddChild(cell);
	}

	private void ClickedCell(){
		foreach (DataGridCell cell in Row.GetChildren()){
			cell.HideEditors();
		}
	}

	private void CellIntChanged(int val){
		LoadOrder = val;
		IntChanged.Invoke(Identifier, val);
	}

	public void ChangeLoadOrder(int loadorder){
		LoadOrder = loadorder;
		(Row.GetChild(1) as DataGridCell).IntContent = LoadOrder.ToString();
	}

	public void ToggleEnabled(bool enabled){
		Enabled = enabled;
		(Row.GetChild(0) as DataGridCell).IsEnabled = enabled;
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Row load order: {0}", LoadOrder));
		(Row.GetChild(1) as DataGridCell).IntContent = LoadOrder.ToString();
	}
	
	private void DetectedClickEnabled(){
		if (!Enabled){
			ItemEnabled.Invoke(Identifier, GetIndex());
		} else {
			ItemDisabled.Invoke(Identifier, GetIndex());
		}
	}
	private void DetectedClickSelected(){
		if (!Selected){
			ItemSelected.Invoke(Identifier, GetIndex());
			//EmitSignal("ItemSelected", GetMeta("Identifier"), GetIndex());
		} else {
			ItemDeselected.Invoke(Identifier, GetIndex());
			//EmitSignal("ItemUnselected", GetMeta("Identifier"), GetIndex());
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
