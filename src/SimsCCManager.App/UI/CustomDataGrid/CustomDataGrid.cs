using Godot;
using SimsCCManager.Debugging;
using SimsCCManager.Globals;
using SimsCCManager.Settings.Loaded;
using SimsCCManager.UI.Containers;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public partial class CustomDataGrid : MarginContainer
{
	[Signal]
	public delegate void SelectedItemEventHandler();
	[Signal]
	public delegate void UnselectedItemEventHandler();
	[Signal]
	public delegate void EnabledItemEventHandler();
	[Signal]
	public delegate void DisabledItemEventHandler();
	public List<HeaderInformation> Headers = new();
	public List<CellContent> Data = new();
	PackedScene divider = GD.Load<PackedScene>("res://UI/CustomDataGrid/DataGridHeaderResizer.tscn");
	PackedScene headercell = GD.Load<PackedScene>("res://UI/CustomDataGrid/DataGridHeaderCell.tscn");
	PackedScene header = GD.Load<PackedScene>("res://UI/CustomDataGrid/DataGridHeaderRow.tscn");
	PackedScene row = GD.Load<PackedScene>("res://UI/CustomDataGrid/DataGridRow.tscn");
	public List<Vector2> ColumnSizes = new();
	VBoxContainer GridContainer;
	DataGridHeaderRow HeaderRow;
	ScrollContainer HeaderScroll;
	ScrollContainer RowsScroll;
	public override void _Ready()
	{
		GridContainer = GetNode<VBoxContainer>("VBoxContainer/RowsScroll/DataGrid_Rows");
		ScrollContainer HeaderContainer = GetNode<ScrollContainer>("VBoxContainer/HeaderScroll");
		HeaderScroll = HeaderContainer;
		RowsScroll = GetNode<ScrollContainer>("VBoxContainer/RowsScroll");
		var headerrow = header.Instantiate() as DataGridHeaderRow;
		foreach (HeaderInformation header in Headers){
			var cellinst = headercell.Instantiate() as DataGridHeaderCell;
			if (header.Blank == false){				
				cellinst.Label = header.HeaderTitle;
				cellinst.setSize = new Vector2(70, 25);
				cellinst.Size = new Vector2(70, 25);
				ColumnSizes.Add(new Vector2(70, 25));
			} else {
				cellinst.setSize = new Vector2(30, 25);
				cellinst.Size = new Vector2(30, 25);
				ColumnSizes.Add(new Vector2(30, 25));
			}
			cellinst.Connect("HeaderResized", new Callable(this, MethodName.HeaderResized));
			headerrow.GetNode<HBoxContainer>("Row").AddChild(cellinst);
		}
		HeaderContainer.AddChild(headerrow);
		HeaderContainer.MoveChild(headerrow, 0);
		HeaderRow = headerrow;
	}

	public void RowsFromData(){
		if (Data.Count != 0){
			if (GridContainer.GetChildCount() > 1){
				for (int i = 0; i < GridContainer.GetChildCount(); i++){
					GridContainer.GetChild(i).QueueFree();
				}
			}
			
			DataGridRow dataGridRow = row.Instantiate() as DataGridRow;
			Color ColorA = LoadedSettings.SetSettings.LoadedTheme.DataGridA;
			Color ColorB = LoadedSettings.SetSettings.LoadedTheme.DataGridB;
			Color SelectedColor = LoadedSettings.SetSettings.LoadedTheme.DataGridSelected;		
			int currentrow = -1;		
			foreach (CellContent item in Data){	
				if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Adding row {0}, column {1}: {2} as cell item.", item.RowNum, item.ColumnNum, item.Content));
				if (currentrow != item.RowNum){
					if (currentrow != -1) GridContainer.AddChild(dataGridRow);
					dataGridRow = row.Instantiate() as DataGridRow;	
					dataGridRow.Connect("ItemSelected", new Callable(this, MethodName.ItemSelected));
					dataGridRow.Connect("ItemUnselected", new Callable(this, MethodName.ItemUnselected));
					dataGridRow.Connect("ItemEnabled", new Callable(this, MethodName.ItemEnabled));
					dataGridRow.Connect("ItemDisabled", new Callable(this, MethodName.ItemDisabled));
					dataGridRow.rowcolor = ColorA;
					dataGridRow.selectedcolor = SelectedColor;
					dataGridRow.Identifier = item.RowIdentifier;
					currentrow = item.RowNum;
					if (Utilities.IsEven(currentrow)){
						dataGridRow.rowcolor = ColorA;
					} else {
						dataGridRow.rowcolor = ColorB;
					}
					dataGridRow.selectedcolor = SelectedColor;
				}
				dataGridRow.AddCell(item, ColumnSizes[item.ColumnNum]);	
			}
			GridContainer.AddChild(dataGridRow);

			if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Grid container children: {0}", GridContainer.GetChildCount()));
			
			for (int i = 0; i < ColumnSizes.Count; i++){
				HeaderResized(i);
			}
		}
	}

    public override void _Process(double delta)
    {
        HeaderScroll.ScrollHorizontal = RowsScroll.ScrollHorizontal;
    }

    private void ItemSelected(string Identifier, int idx){
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Selected item {0}", Identifier));
		EmitSignal("SelectedItem", Identifier, idx);
	}
	private void ItemUnselected(string Identifier, int idx){
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Unselected item {0}", Identifier));
		EmitSignal("UnselectedItem", Identifier, idx);
	}
	private void ItemEnabled(string Identifier, int idx){
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Enabled item {0}", Identifier));
		EmitSignal("EnabledItem", Identifier, idx);
	}
	private void ItemDisabled(string Identifier, int idx){
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Disabled item {0}", Identifier));
		EmitSignal("DisabledItem", Identifier, idx);
	}

	private void HeaderResized(int column){
		//if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Column {0} was resized", column));
		DataGridHeaderCell header = HeaderRow.GetChild(1).GetChild(column) as DataGridHeaderCell;
		var size = header.Size;
		int rows = GridContainer.GetChildCount();
		for (int i = 0; i < rows; i++){
			SetSize(size, GridContainer.GetChild(i).GetChild(1).GetChild(column) as DataGridCell);
		}
	}

	private void SetSize(Vector2 size, DataGridCell cell){
		size = new Vector2(size.X - 1, size.Y);
		cell.Set("custom_minimum_size", size);
		cell.Size = size;
	}
}

public class HeaderInformation {
	public string HeaderTitle {get; set;} = "";
	public bool Blank {get; set;} = false;
	public bool Resizeable {get; set;} = false;
	public DataGridContentType ContentType {get; set;} = DataGridContentType.Null;
	public string ColumnData {get; set;} = "";
}

public class CellContent {
	public int RowNum {get; set;} = 0;
	public int ColumnNum {get; set;} = 0;
	public string Content {get; set;} = "";
	public CellOptions CellType {get; set;} = CellOptions.Text;
	public bool Icons {get; set;} = false;
	public IconOptions IconOptions {get; set;}
	public string RowIdentifier {get; set; } = "";
	public bool Selected {get; set;} = false;
	public string ContentType {get; set;} = "";

	public CellContent(){
		IconOptions = new();
	}
}

public class IconOptions {
	public bool Root {get; set;} = false;
	public bool OutOfDate {get; set;} = false;
	public bool Broken {get; set;} = false;
	public bool Conflicts {get; set;} = false;
	public bool WrongGame {get; set;} = false;
	public bool Override {get; set;} = false;
	public bool Orphan {get; set;} = false;
	public bool Fave {get; set;} = false;
	public bool Folder {get; set;} = false;	

	public int GetCount(){
		int count = 0;
		if (Root) count += 1;
		if (OutOfDate) count += 1;
		if (Broken) count += 1;
		if (Conflicts) count += 1;
		if (WrongGame) count += 1;
		if (Override) count += 1;
		if (Orphan) count += 1;
		if (Fave) count += 1;
		if (Folder) count += 1;
		return count;
	}
}

public enum CellOptions {
	Text,
	Icons,
	TrueFalse
}
