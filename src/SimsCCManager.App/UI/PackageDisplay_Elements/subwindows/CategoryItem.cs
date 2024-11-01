using Godot;
using SimsCCManager.Settings.Loaded;
using System;

public partial class CategoryItem : MarginContainer
{
	public delegate void ItemSelectedEvent(int idx);
	public ItemSelectedEvent ItemSelected;
	public Category category = new();
	//public int packagescount = 0;
	public bool Selected = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<Label>("HBoxContainer/MarginContainer/CatName_Label").Text = category.Name;
		GetNode<Label>("HBoxContainer/MarginContainer2/PackagesCount_Label").Text = string.Format("{0} Packages in Category", category.Packages);
		GetNode<ColorRect>("HBoxContainer/MarginContainer3/MarginContainer/MarginContainer/CatColor").Color = category.Background;
		GetNode<Button>("Button").Pressed += () => SelectedClick();
	}

    private void SelectedClick()
    {
        Selected = !Selected;
		ItemSelected.Invoke(GetIndex());
    }


    public override void _Process(double delta)
    {
        GetNode<Panel>("Panel_Norm").Visible = !Selected;
		GetNode<Panel>("Panel_Highlight").Visible = Selected;
    }

}
