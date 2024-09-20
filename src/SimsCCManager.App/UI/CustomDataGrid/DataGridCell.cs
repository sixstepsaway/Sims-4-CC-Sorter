using Godot;
using SimsCCManager.Debugging;
using SimsCCManager.Globals;
using SimsCCManager.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public partial class DataGridCell : Control
{
	[Signal]
	public delegate void DataGridCellSelectedClickedEventHandler();
	[Signal]
	public delegate void DataGridCellEnabledClickedEventHandler();
	public delegate void CellMouseEvent(bool inside);
	public CellMouseEvent MouseEvent;
	public delegate void CellChangeIntEvent(int newvalue);
	public CellChangeIntEvent CellChangeInt;
	public delegate void ClickedCellEventHandler();
	public ClickedCellEventHandler ClickedCell;
	public bool Text {get; set;} = false;
	public bool Int {get; set;} = false;
	
	public string TextContent = "";
	public string IntContent = "";
	public bool Editable {get; set;} = false;
	public bool Enabled {get; set;} = false;
	public bool IsEnabled {get; set;} = false;
	public bool Icons {get; set;} = false;
	public IconOptions iconOptions = new();
	public bool TrueFalse {get; set;} = false;
	public bool IsTrue {get; set;} = false;

	public bool ColorOption {get; set;} = false;

	public float SizeX {get; set;} = 0f;
	public float SizeY {get; set;} = 25f;

	private bool Selected = false;
	private bool ClickedOnce = false;
	DataGridHeaderCell header;
	int visibleicons = 0;
	int iconswidth = 0;
	int prevloadorder = -1;
	string tooltiptext = "";
	bool InCell = false;
	Timer TooltipTimer = new();
	Timer DoubleClickTimer = new();
	MarginContainer LoadOrderAdjuster;
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		LoadOrderAdjuster = GetNode<MarginContainer>("LoadOrderAdjuster");
		AddChild(TooltipTimer);
		TooltipTimer.Timeout += () => SpawnTooltip();	
		DoubleClickTimer.Timeout += () => DoubleClickTimeout();
		AddChild(DoubleClickTimer);	
		//header = (GetParent().GetParent().GetParent().GetChild(0).GetChild(1).GetChild(GetIndex())) as DataGridHeaderCell;
		//GD.Print(string.Format("Header content: {0}", header.Label));
		if (Enabled) TrueFalse = false;
		if (Enabled) IsTrue = false;
		/*GetNode<MarginContainer>("TextOption").Visible = Text;
		GetNode<MarginContainer>("TextOption").GetNode<LineEdit>("LineEdit").Editable = Editable;
		GetNode<MarginContainer>("TextOption").GetNode<LineEdit>("LineEdit").Text = TextContent;
		GetNode<MarginContainer>("EnabledOption").Visible = Enabled;
		GetNode<MarginContainer>("EnabledOption").GetNode<TextureRect>("Selected").Visible = IsEnabled;
		GetNode<MarginContainer>("EnabledOption").GetNode<TextureRect>("Unselected").Visible = !IsEnabled;
		GetNode<MarginContainer>("EnabledOption").GetNode<TextureRect>("Hover").Visible = false;
		GetNode<MarginContainer>("IconsOption").Visible = Icons;
		//ProcessChangedIcons();
		GetNode<MarginContainer>("TrueFalseOption").Visible = TrueFalse;
		GetNode<MarginContainer>("TrueFalseOption").GetNode<TextureRect>("True").Visible = IsTrue;
		GetNode<MarginContainer>("TrueFalseOption").GetNode<TextureRect>("False").Visible = !IsTrue;
		GetNode<MarginContainer>("ColorOption").Visible = ColorOption;*/
		Size = new Vector2(SizeX, SizeY);
		Set("custom_minimum_size", new Vector2(45, SizeY));
		if (Enabled) { 
			Size = new Vector2(30, SizeY);
			Set("custom_minimum_size", new Vector2(30, SizeY));
		}
		if (Int) { 
			Size = new Vector2(30, SizeY);
			Set("custom_minimum_size", new Vector2(30, SizeY));
		}

		/*GetNode<TextureRect>("IconsOption/Icons/Group").MouseEntered += () => SpawnTooltip("Group");
		GetNode<TextureRect>("IconsOption/Icons/RootMod").MouseEntered += () => SpawnTooltip("Root");
		GetNode<TextureRect>("IconsOption/Icons/Broken").MouseEntered += () => SpawnTooltip("Broken");
		GetNode<TextureRect>("IconsOption/Icons/Conflicts").MouseEntered += () => SpawnTooltip("Conflicts");
		GetNode<TextureRect>("IconsOption/Icons/WrongGame").MouseEntered += () => SpawnTooltip("WrongGame");
		GetNode<TextureRect>("IconsOption/Icons/Override").MouseEntered += () => SpawnTooltip("Override");
		GetNode<TextureRect>("IconsOption/Icons/Orphan").MouseEntered += () => SpawnTooltip("Orphan");
		GetNode<TextureRect>("IconsOption/Icons/Fave").MouseEntered += () => SpawnTooltip("Fave");*/		
		GetNode<Button>("Button").MouseEntered += () => _on_button_mouse_entered(); 
		tooltiptext = iconOptions.GetIcons();
		GetNode<Button>("LoadOrderAdjuster/VBoxContainer/MarginContainer/Button_Up").Pressed += () => IntChange(true);
		GetNode<Button>("LoadOrderAdjuster/VBoxContainer/MarginContainer2/Button_Down").Pressed += () => IntChange(false);
	}

	private void IntChange(bool add){
		int intcont = Int32.Parse(IntContent);
		prevloadorder = intcont;
		if (add) intcont++;
		if (!add) intcont--;
		//IntContent = intcont.ToString();
		CellChangeInt.Invoke(intcont);
	}

	private void _on_button_pressed(){	
		ClickedCell.Invoke();	
		if (Enabled){
			//IsEnabled = !IsEnabled;
			EmitSignal("DataGridCellEnabledClicked");
		} else {
			EmitSignal("DataGridCellSelectedClicked");
		}
		if (ClickedOnce){
			DoubleClickTimer.Stop();
			ClickedOnce = false;
			DoubleClicked();				
		} else {
			//GD.Print("Clicked once...");
			ClickedOnce = true;
			DoubleClickTimer.Start(0.5);			
		}		
	}

	private void DoubleClicked(){
		//GD.Print("Doubleclicked!");
		if (Int){
			//GD.Print("Gonna make the load order adjustable...");
			LoadOrderAdjuster.Visible = true;
		} 
	}

	public void HideEditors(){
		LoadOrderAdjuster.Visible = false;
	}

	private void DoubleClickTimeout(){
		//GD.Print("Unclicked.");
		ClickedOnce = false;
	}

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("Return")){
			LoadOrderAdjuster.Visible = false;
		}
    }

    public void ProcessChangedIcons(){		
		/*GetNode<MarginContainer>("IconsOption").GetNode<HBoxContainer>("Icons").GetNode<TextureRect>("RootMod").Visible = iconOptions.Root;
		GetNode<MarginContainer>("IconsOption").GetNode<HBoxContainer>("Icons").GetNode<TextureRect>("Broken").Visible = iconOptions.Broken;
		GetNode<MarginContainer>("IconsOption").GetNode<HBoxContainer>("Icons").GetNode<TextureRect>("Conflicts").Visible = iconOptions.Conflicts;
		GetNode<MarginContainer>("IconsOption").GetNode<HBoxContainer>("Icons").GetNode<TextureRect>("WrongGame").Visible = iconOptions.WrongGame;
		GetNode<MarginContainer>("IconsOption").GetNode<HBoxContainer>("Icons").GetNode<TextureRect>("Override").Visible = iconOptions.Override;
		GetNode<MarginContainer>("IconsOption").GetNode<HBoxContainer>("Icons").GetNode<TextureRect>("Orphan").Visible = iconOptions.Orphan;
		GetNode<MarginContainer>("IconsOption").GetNode<HBoxContainer>("Icons").GetNode<TextureRect>("Fave").Visible = iconOptions.Fave;
		GetNode<MarginContainer>("IconsOption").GetNode<HBoxContainer>("Icons").GetNode<TextureRect>("Group").Visible = iconOptions.Folder;		
		visibleicons = iconOptions.GetCount();
		iconswidth = (visibleicons * 15) + (visibleicons * 2);*/
	}

	private void _on_button_mouse_entered(){
		InCell = true;
		if (Icons) {
			TooltipTimer.Start(1);
		}
		MouseEvent.Invoke(true);
		GetNode<MarginContainer>("EnabledOption").GetNode<TextureRect>("Selected").Visible = false;
		GetNode<MarginContainer>("EnabledOption").GetNode<TextureRect>("Unselected").Visible = false;
		GetNode<MarginContainer>("EnabledOption").GetNode<TextureRect>("Hover").Visible = true;
	}

	private void _on_button_mouse_exited(){
		InCell = false;
		MouseEvent.Invoke(false);
		GetNode<MarginContainer>("EnabledOption").GetNode<TextureRect>("Selected").Visible = IsEnabled;
		GetNode<MarginContainer>("EnabledOption").GetNode<TextureRect>("Unselected").Visible = !IsEnabled;
		GetNode<MarginContainer>("EnabledOption").GetNode<TextureRect>("Hover").Visible = false;
	}

	private void SpawnTooltip(){
		if (InCell){
			ToolTip tooltip = UIUtilities.CustomTooltip(tooltiptext, GetGlobalMousePosition());
			GetWindow().AddChild(tooltip);
		}
	}

    public override void _Process(double delta)
    {
		if (Icons) {
			GetNode<MarginContainer>("IconsOption").Visible = Icons;	
			GetNode<MarginContainer>("IconsOption").GetNode<HBoxContainer>("Icons").GetNode<TextureRect>("RootMod").Visible = iconOptions.Root;
			GetNode<MarginContainer>("IconsOption").GetNode<HBoxContainer>("Icons").GetNode<TextureRect>("Broken").Visible = iconOptions.Broken;
			GetNode<MarginContainer>("IconsOption").GetNode<HBoxContainer>("Icons").GetNode<TextureRect>("Conflicts").Visible = iconOptions.Conflicts;
			GetNode<MarginContainer>("IconsOption").GetNode<HBoxContainer>("Icons").GetNode<TextureRect>("WrongGame").Visible = iconOptions.WrongGame;
			GetNode<MarginContainer>("IconsOption").GetNode<HBoxContainer>("Icons").GetNode<TextureRect>("Override").Visible = iconOptions.Override;
			GetNode<MarginContainer>("IconsOption").GetNode<HBoxContainer>("Icons").GetNode<TextureRect>("Orphan").Visible = iconOptions.Orphan;
			GetNode<MarginContainer>("IconsOption").GetNode<HBoxContainer>("Icons").GetNode<TextureRect>("Fave").Visible = iconOptions.Fave;
			GetNode<MarginContainer>("IconsOption").GetNode<HBoxContainer>("Icons").GetNode<TextureRect>("Group").Visible = iconOptions.Folder;		
			GetNode<MarginContainer>("IconsOption").GetNode<HBoxContainer>("Icons").GetNode<TextureRect>("LoadAsFolder").Visible = iconOptions.LoadAsFolder;	
			GetNode<MarginContainer>("IconsOption").GetNode<HBoxContainer>("Icons").GetNode<TextureRect>("LinkedFiles").Visible = iconOptions.LinkedFiles;		
			GetNode<MarginContainer>("IconsOption").GetNode<HBoxContainer>("Icons").GetNode<TextureRect>("MiscFile").Visible = iconOptions.MiscFile;	
			visibleicons = iconOptions.GetCount();
			iconswidth = (visibleicons * 15) + (visibleicons * 2);
		}
		if (Text){
			GetNode<MarginContainer>("TextOption").Visible = Text;
			GetNode<MarginContainer>("TextOption").GetNode<LineEdit>("LineEdit").Editable = Editable;
			GetNode<MarginContainer>("TextOption").GetNode<LineEdit>("LineEdit").Text = TextContent;
		}
		if (Int){
			GetNode<MarginContainer>("NumberOption").Visible = Int;
			//if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Setting cell load order to {0}.", IntContent));
			GetNode<MarginContainer>("NumberOption").GetNode<LineEdit>("MarginContainer/LineEdit").Text = IntContent;
			if (IntContent == "0") GetNode<MarginContainer>("NumberOption").GetNode<LineEdit>("MarginContainer/LineEdit").Text = "";
			if (IntContent == "-1") GetNode<MarginContainer>("NumberOption").GetNode<LineEdit>("MarginContainer/LineEdit").Text = "";
		}
		if (Enabled){
			GetNode<MarginContainer>("EnabledOption").Visible = Enabled;
			GetNode<MarginContainer>("EnabledOption").GetNode<TextureRect>("Selected").Visible = IsEnabled;
			GetNode<MarginContainer>("EnabledOption").GetNode<TextureRect>("Unselected").Visible = !IsEnabled;
			//GetNode<MarginContainer>("EnabledOption").GetNode<TextureRect>("Hover").Visible = false;
		}
        //ProcessChangedIcons();
		if (TrueFalse){
			GetNode<MarginContainer>("TrueFalseOption").Visible = TrueFalse;
			GetNode<MarginContainer>("TrueFalseOption").GetNode<TextureRect>("True").Visible = IsTrue;
			GetNode<MarginContainer>("TrueFalseOption").GetNode<TextureRect>("False").Visible = !IsTrue;
		}
		if (ColorOption){
			GetNode<MarginContainer>("ColorOption").Visible = ColorOption;
		}
    }
}
