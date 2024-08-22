using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class DataGridHeaderCell : Control
{
	public delegate void HeaderResized(int index);
	public event HeaderResized HeaderResizedEvent;
	public delegate void HeaderSorted(int index);
	public event HeaderSorted HeaderSortedEvent;
	public string Label = "";
	public Vector2 setSize;
	SortingOptions sortingOptions = SortingOptions.NotSorted;
	Button sortingbutton;
	MarginContainer sortingiconcontrainer;
	TextureRect sortingtexture;
	Texture2D sortingAscending;
	Texture2D sortingDescending;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sortingAscending = GD.Load<Texture2D>("res://assets/icons/materialicons/twotone_arrow_drop_down_black_48dp.png");
		sortingDescending = GD.Load<Texture2D>("res://assets/icons/materialicons/twotone_arrow_drop_up_black_48dp.png");
		sortingtexture = GetNode<TextureRect>("MarginContainer3/TextureRect");
		sortingiconcontrainer = GetNode<MarginContainer>("MarginContainer3");
		sortingiconcontrainer.Visible = false;
		SetSize(setSize);
		GetNode<Label>("MarginContainer2/Label").Text = Label;
		sortingbutton = GetNode<Button>("Button");
		sortingbutton.Pressed += () => OnButtonPressed();
		//GD.Print(string.Format("Header cell width: {0}\n      Label width: {1}", Size.X, GetNode<Label>("MarginContainer2/Label").Size.X));
	}

    private void OnButtonPressed()
    {		
        HeaderSortedEvent.Invoke(GetIndex());
    }

    public void SetSize(Vector2 size){
		Set("custom_minimum_size", setSize);
		Size = setSize;
	}

	private void _on_resized(){
		HeaderResizedEvent.Invoke(GetIndex());
	}

	public void ToggleSorting(){
		if (sortingOptions == SortingOptions.NotSorted){
			sortingOptions = SortingOptions.Ascending;
		} else if (sortingOptions == SortingOptions.Ascending){
			sortingOptions = SortingOptions.Descending;
		} else if (sortingOptions == SortingOptions.Descending){
			sortingOptions = SortingOptions.NotSorted;
		}		
	}

	public void ResetSorting(){
		sortingOptions = SortingOptions.NotSorted;
	}

    public override void _Process(double delta)
    {
        if (sortingOptions == SortingOptions.NotSorted){
			sortingiconcontrainer.Visible = false;
		} else if (sortingOptions == SortingOptions.Ascending){
			sortingiconcontrainer.Visible = true;
			sortingtexture.Texture = sortingAscending;
		} else if (sortingOptions == SortingOptions.Descending){
			sortingiconcontrainer.Visible = true;
			sortingtexture.Texture = sortingDescending;
		}
    }
}

public enum SortingOptions {
	NotSorted,
	Ascending,
	Descending
}
