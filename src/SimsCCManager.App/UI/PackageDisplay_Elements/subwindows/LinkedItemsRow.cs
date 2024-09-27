using Godot;
using System;

public partial class LinkedItemsRow : MarginContainer
{
	public delegate void ItemTickedEvent(bool check, string filename);
	public ItemTickedEvent LinkedChecked;
	public ItemTickedEvent PrimaryChecked;
	public delegate void SelectAllEvent();
	public SelectAllEvent SelectAll;
	
	MarginContainer PrimaryCheckCheck;
	MarginContainer LinkCheckCheck;
	public bool LinkedTicked = false;
	public bool PrimaryTicked = false;
	public string FileName = "";
	bool linkedfilemousein = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PrimaryCheckCheck = GetNode<MarginContainer>("HBoxContainer/PrimaryFile/MarginContainer/CheckBox_Checked");
		LinkCheckCheck = GetNode<MarginContainer>("HBoxContainer/LinkedFile/MarginContainer/CheckBox_Checked");
		GetNode<Button>("HBoxContainer/LinkedFile/LinkFile_Button").Pressed += () => LinkedFilePressed();
		GetNode<Button>("HBoxContainer/LinkedFile/LinkFile_Button").MouseEntered += () => LinkedFileMouseIn(true);
		GetNode<Button>("HBoxContainer/LinkedFile/LinkFile_Button").MouseExited += () => LinkedFileMouseIn(false);
		GetNode<Button>("HBoxContainer/PrimaryFile/PrimaryFile_Button").Pressed += () => PrimaryFilePressed();
		GetNode<Label>("HBoxContainer/FileNameContainer/MarginContainer/FileNameLabel").Text = FileName;
	}

    private void LinkedFileMouseIn(bool v)
    {
        linkedfilemousein = v;
    }

    private void LinkedFilePressed(){
		LinkedTicked = !LinkedTicked;
		LinkedChecked.Invoke(LinkedTicked, FileName);
	}

	private void PrimaryFilePressed(){
		PrimaryTicked = !PrimaryTicked;
		PrimaryChecked.Invoke(PrimaryTicked, FileName);
	}

    public override void _Input(InputEvent @event)
    {
        if (linkedfilemousein){
			if (@event.IsActionPressed("ShiftClick")){
				SelectAll.Invoke();
			}
		}
    }

    public override void _Process(double delta)
    {
        PrimaryCheckCheck.Visible = PrimaryTicked;
		LinkCheckCheck.Visible = LinkedTicked;
    }

}
