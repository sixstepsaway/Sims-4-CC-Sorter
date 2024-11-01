using Godot;
using System;

public partial class ProfileItem : MarginContainer
{
	public delegate void ItemSelectedEvent(bool selected, string item);
	public ItemSelectedEvent ItemSelected;
	public bool selected = false;
	public string ItemName = "";
	public bool Active = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<Button>("Button").Pressed += () => ButtonPressed();
		GetNode<Label>("HBoxContainer/ProfileName_Label").Text = ItemName;
	}

    private void ButtonPressed()
    {
		selected = !selected;
        ItemSelected.Invoke(selected, ItemName);
    }


    public override void _Process(double delta)
    {
        GetNode<Label>("HBoxContainer/Active_Label").Visible = Active;
		GetNode<Label>("HBoxContainer/Inactive_Label").Visible = !Active;
		GetNode<Panel>("Panel_Norm").Visible = !selected;
		GetNode<Panel>("Panel_Highlight").Visible = selected;
    }

}
