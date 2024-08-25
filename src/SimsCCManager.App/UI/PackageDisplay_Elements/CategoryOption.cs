using Godot;
using SimsCCManager.UI.Utilities;
using System;

public partial class CategoryOption : MarginContainer
{
	// Called when the node enters the scene tree for the first time.
	public Color catcolor;
	public string catname = "";
	public bool selected = false;
	public bool open = false;
	public delegate void CategorySelectedEvent(string categoryname, bool selected);
	public CategorySelectedEvent CategorySelected;

	public override void _Ready()
	{
		UIUtilities.UpdateTheme(GetTree());
		GetNode<ColorRect>("CatColor").Color = catcolor;
		GetNode<Label>("HBoxContainer/Label/CatName_Label").Text = catname;
		GetNode<Button>("Button").Pressed += () => ButtonPressed();
	}

	private void ButtonPressed(){
		if (selected) { Deselect(); } else { Select(); }
		CategorySelected.Invoke(catname, selected);
	}

	public void Select(){
		selected = true;
		open = false;
	}

	public void Deselect(){
		selected = false;
		open = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (selected){
			GetNode<MarginContainer>("HBoxContainer/MarginContainer/CheckBox/SelectedBox").Visible = true;
			GetNode<MarginContainer>("HBoxContainer/MarginContainer/CheckBox/OpenBox").Visible = false;
		} else if (open){
			GetNode<MarginContainer>("HBoxContainer/MarginContainer/CheckBox/SelectedBox").Visible = false;
			GetNode<MarginContainer>("HBoxContainer/MarginContainer/CheckBox/OpenBox").Visible = true;
		}
	}
}
