using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class DataGridCell : Control
{
	[Signal]
	public delegate void DataGridCellSelectedClickedEventHandler();
	[Signal]
	public delegate void DataGridCellEnabledClickedEventHandler();
	public bool Text {get; set;} = false;
	public string TextContent = "";
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
	DataGridHeaderCell header;
	int visibleicons = 0;
	int iconswidth = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{		
		header = (GetParent().GetParent().GetParent().GetChild(0).GetChild(1).GetChild(GetIndex())) as DataGridHeaderCell;
		GD.Print(string.Format("Header content: {0}", header.Label));
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
		Size = new Vector2(45, 25);
		Set("custom_minimum_size", new Vector2(45, 25));
		if (Enabled) Size = new Vector2(30, 25);
		if (Enabled) Set("custom_minimum_size", new Vector2(30, 25));
	}

	private void _on_button_pressed(){		
		if (Enabled){
			//IsEnabled = !IsEnabled;
			EmitSignal("DataGridCellEnabledClicked");
		} else {
			EmitSignal("DataGridCellSelectedClicked");
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
		GetNode<MarginContainer>("EnabledOption").GetNode<TextureRect>("Selected").Visible = false;
		GetNode<MarginContainer>("EnabledOption").GetNode<TextureRect>("Unselected").Visible = false;
		GetNode<MarginContainer>("EnabledOption").GetNode<TextureRect>("Hover").Visible = true;
	}

	private void _on_button_mouse_exited(){
		GetNode<MarginContainer>("EnabledOption").GetNode<TextureRect>("Selected").Visible = IsEnabled;
		GetNode<MarginContainer>("EnabledOption").GetNode<TextureRect>("Unselected").Visible = !IsEnabled;
		GetNode<MarginContainer>("EnabledOption").GetNode<TextureRect>("Hover").Visible = false;
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
			visibleicons = iconOptions.GetCount();
			iconswidth = (visibleicons * 15) + (visibleicons * 2);
		}
		if (Text){
			GetNode<MarginContainer>("TextOption").Visible = Text;
			GetNode<MarginContainer>("TextOption").GetNode<LineEdit>("LineEdit").Editable = Editable;
			GetNode<MarginContainer>("TextOption").GetNode<LineEdit>("LineEdit").Text = TextContent;
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
