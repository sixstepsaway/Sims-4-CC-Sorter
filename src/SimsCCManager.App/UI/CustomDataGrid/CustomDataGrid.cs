using Godot;
using SimsCCManager.Debugging;
using SimsCCManager.Globals;
using SimsCCManager.Settings.Loaded;
using SimsCCManager.UI.Containers;
using SimsCCManager.UI.Themes;
using SimsCCManager.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

public partial class CustomDataGrid : MarginContainer
{
	public delegate void SelectedItemEvent(string identifer, int idx);
	public SelectedItemEvent SelectedItem;
	public delegate void UnselectedItemEvent(string identifer, int idx);
	public UnselectedItemEvent UnselectedItem;
	public delegate void EnabledItemEvent(string identifer, int idx);
	public EnabledItemEvent EnabledItem;
	public delegate void DisabledItemEvent(string identifer, int idx);
	public DisabledItemEvent DisabledItem;
	public delegate void HeaderSortedEvent(int idx, SortingOptions sortingrule);
	public event HeaderSortedEvent HeaderSortedSignal;

	public delegate void MouseAffectingEvent(bool inside, int idx);
	public MouseAffectingEvent MouseAffectingGrid;


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
	public SortingOptions sortingRule = SortingOptions.NotSorted;
	List<DataGridRow> rows = new();
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
			cellinst.HeaderResizedEvent += (idx) => HeaderResized(idx); 
			cellinst.HeaderSortedEvent += (idx) => HeaderSorted(idx);
			//.Connect("HeaderResized", new Callable(this, MethodName.HeaderResized));
			headerrow.GetNode<HBoxContainer>("Row").AddChild(cellinst);
		}
		HeaderContainer.AddChild(headerrow);
		HeaderContainer.MoveChild(headerrow, 0);
		HeaderRow = headerrow;
	}

	 


    private void HeaderSorted(int idx)
    {
		if (sortingRule == SortingOptions.NotSorted){
			sortingRule = SortingOptions.Ascending;
		} else if (sortingRule == SortingOptions.Ascending){
			sortingRule = SortingOptions.Descending;
		} else if (sortingRule == SortingOptions.Descending){
			sortingRule = SortingOptions.NotSorted;
		}
		DataGridHeaderCell headercell = HeaderRow.GetNode<HBoxContainer>("Row").GetChild(idx) as DataGridHeaderCell;
		headercell.ToggleSorting();		
        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Sorting by header {0}: {1}", idx, Headers[idx].HeaderTitle));
		HeaderSortedSignal.Invoke(idx, sortingRule);
    }



	private void MouseAffectingRow(bool inside, int idx){		
		MouseAffectingGrid.Invoke(inside, idx);
	}

	private void ClearChildren(){
		if (GridContainer.GetChildCount() > 0){
			for (int i = 0; i < GridContainer.GetChildCount(); i++){
				GridContainer.GetChild(i).QueueFree();
			}
		}
	}

    public void RowsFromData(){
		if (Data.Count != 0){
			CallDeferred(nameof(ClearChildren));

			DataGridRow dataGridRow = row.Instantiate() as DataGridRow;			
			Color ColorA = LoadedSettings.SetSettings.LoadedTheme.DataGridA;
			Color ColorB = LoadedSettings.SetSettings.LoadedTheme.DataGridB;
			Color SelectedColor = LoadedSettings.SetSettings.LoadedTheme.DataGridSelected;		
			int currentrow = -1;		
			foreach (CellContent item in Data){	
				if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Adding row {0}, column {1}: {2} as cell item.", item.RowNum, item.ColumnNum, item.Content));
				if (currentrow != item.RowNum){
					if (currentrow != -1) rows.Add(dataGridRow);
					dataGridRow = row.Instantiate() as DataGridRow;	
					dataGridRow.MouseAffected += (inside, idx) => MouseAffectingRow(inside, idx);
					dataGridRow.ItemSelected += (ident, idx) => ItemSelected(ident, idx);
					dataGridRow.ItemDeselected += (ident, idx) => ItemUnselected(ident, idx);
					dataGridRow.ItemEnabled += (ident, idx) => ItemEnabled(ident, idx);
					dataGridRow.ItemDisabled += (ident, idx) => ItemDisabled(ident, idx);
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
			//dataGridRow.MouseAffected += (inside, idx) => MouseAffectingRow(inside, idx);			
			rows.Add(dataGridRow);			
			CallDeferred(nameof(PopulateRows));			
		}
	}

	private void PopulateRows(){
		foreach (DataGridRow row in rows){
			GridContainer.AddChild(row);
		}
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Grid container children: {0}", GridContainer.GetChildCount()));
		for (int i = 0; i < ColumnSizes.Count; i++){
			HeaderResized(i);
		}
	}

    public override void _Process(double delta)
    {
        HeaderScroll.ScrollHorizontal = RowsScroll.ScrollHorizontal;
    }

    private void ItemSelected(string Identifier, int idx){
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Selected item {0}", Identifier));
		SelectedItem.Invoke(Identifier, idx);
	}
	private void ItemUnselected(string Identifier, int idx){
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Unselected item {0}", Identifier));
		UnselectedItem.Invoke(Identifier, idx);
	}
	private void ItemEnabled(string Identifier, int idx){
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Enabled item {0}", Identifier));
		EnabledItem.Invoke(Identifier, idx);		
	}
	private void ItemDisabled(string Identifier, int idx){
		if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Disabled item {0}", Identifier));
		DisabledItem.Invoke(Identifier, idx);
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
	private Color _backgroundcolor;
	public Color BackgroundColor {
		get { return _backgroundcolor; }
		set { _backgroundcolor = value; 
		TextColor = UIUtilities.GetFGColor(value);}
	}
	public Color TextColor {get; set;}

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

	public string GetIcons(){
		StringBuilder sb = new();
		if (Root) sb.AppendLine("Root");
		if (OutOfDate) sb.AppendLine("Out of Date");
		if (Broken) sb.AppendLine("Broken");
		if (Conflicts) sb.AppendLine("Has Conflicts");
		if (WrongGame) sb.AppendLine("Wrong Game");
		if (Override) sb.AppendLine("Override");
		if (Orphan) sb.AppendLine("Orphan");
		if (Fave) sb.AppendLine("Favorite");
		if (Folder) sb.AppendLine("Folder");
		return sb.ToString();
	}
}

public enum CellOptions {
	Text,
	Icons,
	TrueFalse,
	Int
}
