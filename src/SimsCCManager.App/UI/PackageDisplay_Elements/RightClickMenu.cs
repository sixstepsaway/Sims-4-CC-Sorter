using Godot;
using SimsCCManager.Settings.Loaded;
using SimsCCManager.UI.Themes;
using SimsCCManager.UI.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

public partial class RightClickMenu : Control
{
	MarginContainer catoptions;
	PackedScene CatOption = GD.Load<PackedScene>("res://UI/PackageDisplay_Elements/category_option.tscn");
	public List<Category> Categories = new();
	public string TickedCat = "";
	public List<string> OpenCats = new();	
	public delegate void CategorySelectedMenuEvent(string categoryname, bool selected);
	public CategorySelectedMenuEvent CategorySelectedMenu;
	public delegate void ButtonPressedEvent(int button);
	public ButtonPressedEvent RightClickMenuClicked;
	public bool nocats = false;
	public bool someoutofdate = false;
	public bool multiplefiles = false;
	MarginContainer EditDetailsBox;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		string oodstr = "Make Out of Date";
		//we are the ood
		UIUtilities.UpdateTheme(GetTree());
		catoptions = GetNode<MarginContainer>("CategoryOptions");
		VBoxContainer catbox = GetNode<VBoxContainer>("CategoryOptions/VBoxContainer");
		catoptions.Visible = false;
		GetNode<Button>("MarginContainer/VBoxContainer/Category/Category_Button").MouseEntered += () => MouseEnteredCategoryButton();
		GetNode<Button>("MarginContainer/VBoxContainer/Category/Category_Button").MouseExited += () => MouseExitedCategoryButton();
		catoptions.MouseExited += () => MouseLeftCategoryList();
		catoptions.MouseEntered += () => MouseEnteredCategoryList();
		if (!someoutofdate) oodstr = "Make Updated";
		EditDetailsBox = GetNode<MarginContainer>("EditDetails_Options");

		float x = GetNode<MarginContainer>("MarginContainer").Size.X;
		
		catoptions.Position = new(x, catoptions.Position.Y); 
		EditDetailsBox.Position = new(x, EditDetailsBox.Position.Y);
		
		
		
		
		
		
		
		GetNode<Label>("MarginContainer/VBoxContainer/MarkAsUpdated/MarkAsUpdated_Label").Text = oodstr;
		SetButtons();

		if (multiplefiles){
			GetNode<Label>("EditDetails_Options/VBoxContainer/MoveFile/MoveFile_Label").Text = "Move Files";
			GetNode<Label>("EditDetails_Options/VBoxContainer/Delete/DeleteFile_Label").Text = "Delete Files";
			GetNode<Label>("EditDetails_Options/VBoxContainer/Rename/RenameFile_Label").Text = "Rename Files";
		} else {
			GetNode<Label>("EditDetails_Options/VBoxContainer/MoveFile/MoveFile_Label").Text = "Move File";
			GetNode<Label>("EditDetails_Options/VBoxContainer/Delete/DeleteFile_Label").Text = "Delete File";
			GetNode<Label>("EditDetails_Options/VBoxContainer/Rename/RenameFile_Label").Text = "Rename File";
		}


		foreach (Category category in Categories){
			if (category.Name != "Default"){
				CategoryOption cat = CatOption.Instantiate() as CategoryOption;
				cat.catcolor = category.Background;
				cat.catname = category.Name;
				if (cat.catname == TickedCat){
					cat.selected = true;
				} else if (OpenCats.Contains(cat.catname)){
					cat.open = true;
				}
				cat.CategorySelected += (cat, selected) => CategorySelected(cat, selected);
				catbox.AddChild(cat);
			}
			if (catbox.GetChildCount() == 0) nocats = true;			
		}
		if (nocats){
			Label label = GetNode<Label>("MarginContainer/VBoxContainer/Category/Category_Label");
			label.Modulate = Color.FromHtml("ffffff33");
			GetNode<Button>("MarginContainer/VBoxContainer/Category/Category_Button").Disabled = true;
		}
	}

	private void SetButtons(){
		GetNode<Button>("MarginContainer/VBoxContainer/Linked/AddLinked_Button").Pressed += () => OptionClicked(0);
		GetNode<Button>("MarginContainer/VBoxContainer/MakeRoot/MakeRoot_Button").Pressed += () => OptionClicked(1);
		GetNode<Button>("MarginContainer/VBoxContainer/MakeFave/MakeFave_Button").Pressed += () => OptionClicked(2);
		GetNode<Button>("MarginContainer/VBoxContainer/MarkAsUpdated/MarkAsUpdated_Button").Pressed += () => OptionClicked(3);		
		GetNode<Button>("MarginContainer/VBoxContainer/FilesFromFolder/FilesFromFolder_Button").Pressed += () => OptionClicked(4);
		GetNode<Button>("EditDetails_Options/VBoxContainer/Rename/RenameFile_Button").Pressed += () => OptionClicked(5);
		GetNode<Button>("EditDetails_Options/VBoxContainer/AddCreator/AddCreator_Button").Pressed += () => OptionClicked(6);
		GetNode<Button>("EditDetails_Options/VBoxContainer/AddSourceLink/AddSourceLink_Button").Pressed += () => OptionClicked(7);
		GetNode<Button>("EditDetails_Options/VBoxContainer/MoveFile/MoveFile_Button").Pressed += () => OptionClicked(8);
		GetNode<Button>("EditDetails_Options/VBoxContainer/Delete/DeleteFile_Button").Pressed += () => OptionClicked(9);
		GetNode<Button>("MarginContainer/VBoxContainer/LoadasFolder/LoadasFolder_Button").Pressed += () => OptionClicked(10);
		GetNode<Button>("MarginContainer/VBoxContainer/MarkAsCorrectGame/MarkAsCorrectGame_Button").Pressed += () => OptionClicked(11);


		GetNode<Button>("MarginContainer/VBoxContainer/EditDetails/EditDetails_Button").MouseEntered += () => EditDetails(true);
		GetNode<MarginContainer>("EditDetails_Options").MouseEntered += () => EditDetails(true);
		GetNode<MarginContainer>("EditDetails_Options").MouseExited += () => EditDetails(false);
	}

	private void EditDetails(bool show){
		GetNode<MarginContainer>("EditDetails_Options").Visible = show;
	}

	private void OptionClicked(int option){
		RightClickMenuClicked.Invoke(option);
	}

	private void CategorySelected(string category, bool selected){
		CategorySelectedMenu.Invoke(category, selected);
	}

    private void MouseEnteredCategoryList()
    {
		if (!nocats) catoptions.Visible = true;
    }

    private void MouseExitedCategoryButton()
    {
        //catoptions.Visible = false;
    }

    private void MouseLeftCategoryList()
    {
		if (!nocats) catoptions.Visible = false;
    }

    private void MouseEnteredCategoryButton()
    {
        if (!nocats) catoptions.Visible = true;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
