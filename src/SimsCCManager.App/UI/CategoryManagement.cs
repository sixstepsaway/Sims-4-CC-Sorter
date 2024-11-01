using Godot;
using SimsCCManager.Globals;
using SimsCCManager.Settings.Loaded;
using SimsCCManager.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CategoryManagement : MarginContainer
{
	public delegate void CategoryEditing(Category cat);
	public CategoryEditing NewCat;
	public CategoryEditing EditCat;
	public CategoryEditing DeleteCat;

	PackedScene categoryItem = GD.Load<PackedScene>("res://UI/PackageDisplay_Elements/subwindows/CategoryItem.tscn");
	public List<Category> Categories = new();
	VBoxContainer catlist;
	LineEdit newcatname;
	MarginContainer newcat;
	TextEdit newcatdesc;
	ColorPickerButton newcatcolor;
	bool editing = false;
	Category selectedcat = new();
	HolderNode holdernode;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		holdernode = GetWindow().GetNode<HolderNode>("MainWindow/HolderNode");
		catlist = GetNode<VBoxContainer>("MarginContainer/VBoxContainer/HBoxContainer2/MarginContainer/MarginContainer/ScrollContainer/VBoxContainer");
		GetNode<Button>("MarginContainer/VBoxContainer/HBoxContainer2/MarginContainer2/VBoxContainer/NewCategory_Button").Pressed += () => NewCategory();
		GetNode<Button>("MarginContainer/VBoxContainer/HBoxContainer2/MarginContainer2/VBoxContainer/EditCategory_Button").Pressed += () => EditCategory();
		GetNode<Button>("MarginContainer/VBoxContainer/HBoxContainer2/MarginContainer2/VBoxContainer/DeleteCategory_Button").Pressed += () => DeleteCategory();
		newcat = GetNode<MarginContainer>("MarginContainer2");
		newcatname = GetNode<LineEdit>("MarginContainer2/MarginContainer/VBoxContainer/LineEdit");
		newcatdesc = GetNode<TextEdit>("MarginContainer2/MarginContainer/VBoxContainer/TextEdit");
		newcatcolor = GetNode<ColorPickerButton>("MarginContainer2/MarginContainer/VBoxContainer/ColorPickerButton");
		GetNode<Button>("MarginContainer2/MarginContainer/VBoxContainer/HBoxContainer/Confirm_Button").Pressed += () => ConfirmCat();
		GetNode<Button>("MarginContainer2/MarginContainer/VBoxContainer/HBoxContainer/Cancel_Button").Pressed += () => CancelCat();
		GetNode<Button>("MarginContainer/VBoxContainer/HBoxContainer/Close_Button").Pressed += () => CloseWindow();
	}

	private void CloseWindow(){
		this.Visible = false;
	}

	private void ConfirmCat(){
		if (editing){
			int idx = Categories.IndexOf(selectedcat);
			selectedcat.Name = newcatname.Text;
			selectedcat.Description = newcatdesc.Text;
			selectedcat.Background = newcatcolor.Color;
			Categories[idx] = selectedcat;
			EditCat.Invoke(selectedcat);
			ListCategories();
			editing = false;
			newcat.Visible = false;						
		} else {
			Category cat = new(){
				Name = newcatname.Text,
				Description = newcatdesc.Text,
				Background = newcatcolor.Color
			};		
			AddCat(cat);
			newcat.Visible = false;
			NewCat.Invoke(cat);
		}	
		editing = false;	
	}

	private void CancelCat(){
		newcat.Visible = false;
		editing = false;
	}
	
	private void NewCategory(){
		newcatname.Text = "";
		newcatdesc.Text = "";
		newcatcolor.Color = Color.FromHtml("D6D6D6");
		newcat.Visible = true;
	}

	private void EditCategory(){
		editing = true;
		newcatname.Text = selectedcat.Name;
		newcatdesc.Text = selectedcat.Description;
		newcatcolor.Color = selectedcat.Background;
		newcat.Visible = true;
	}

	private void DeleteCategory(){

	}
	
	private void ItemSelected(int i){
		selectedcat = (catlist.GetChild(i) as CategoryItem).category;
	}

	public void ListCategories(){
		foreach (CategoryItem item in catlist.GetChildren()){
			item.QueueFree();
		}
		foreach (Category cat in Categories){
			CategoryItem ci = categoryItem.Instantiate() as CategoryItem;
			ci.ItemSelected += (i) => ItemSelected(i);
			ci.category = cat;	
			catlist.AddChild(ci);		
		}
		//UIUtilities.UpdateTheme(GetTree());
		holdernode.UpdateTheme(GetTree());
	}

	public void AddCat(Category newc){
		CategoryItem ci = categoryItem.Instantiate() as CategoryItem;
		ci.ItemSelected += (i) => ItemSelected(i);
		ci.category = newc;	
		catlist.AddChild(ci);
		//UIUtilities.UpdateTheme(GetTree());
		holdernode.UpdateTheme(GetTree());
	}
}
