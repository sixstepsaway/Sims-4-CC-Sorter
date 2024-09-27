using Godot;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;

public partial class LinkFilesWindow : MarginContainer
{
	public delegate void LinkedFilesEvent(List<string> linkeditems, string primaryitem);
	public LinkedFilesEvent FilesLinked;
	public List<string> Files = new();
	PackedScene ItemRow = GD.Load<PackedScene>("res://UI/PackageDisplay_Elements/subwindows/LinkedItems_Row.tscn");
	List<LinkedItemsRow> rows = new();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		VBoxContainer rowcontainer = GetNode<VBoxContainer>("MarginContainer/VBoxContainer/MarginContainer/MarginContainer/ScrollContainer/Rows");
		foreach (string file in Files){
			LinkedItemsRow row = ItemRow.Instantiate() as LinkedItemsRow;
			row.LinkedChecked += (check, fn) => LinkedChecked(check, fn);
			row.PrimaryChecked += (check, fn) => PrimaryChecked(check, fn);
			row.SelectAll += () => SelectAll();
			row.FileName = file;
			rows.Add(row);
			rowcontainer.AddChild(row);
		}
		GetNode<Button>("MarginContainer/VBoxContainer/HBoxContainer/Confirm_Button").Pressed += () => ConfirmLinked();
		GetNode<Button>("MarginContainer/VBoxContainer/HBoxContainer/Cancel_Button").Pressed += () => CancelLinking();
	}

    private void SelectAll()
    {
        foreach (LinkedItemsRow row in rows){
			if (!row.PrimaryTicked){
				row.LinkedTicked = true;
			}
		}
    }

    private void ConfirmLinked(){
		List<string> linked = new();
		string primary = "";
		foreach (LinkedItemsRow row in rows){
			if (row.PrimaryTicked){
				primary = row.FileName;
			}
			if (row.LinkedTicked){
				linked.Add(row.FileName);
			}
		}
		FilesLinked.Invoke(linked, primary);
		QueueFree();
	}

	private void CancelLinking(){
		QueueFree();
	}

	private void LinkedChecked(bool check, string filename){
		foreach (LinkedItemsRow row in rows){
			if (row.PrimaryTicked && row.FileName == filename){
				row.PrimaryTicked = false;
			}
		}
	}
	private void PrimaryChecked(bool check, string filename){		
		foreach (LinkedItemsRow row in rows){
			if (row.PrimaryTicked && row.FileName != filename){
				row.PrimaryTicked = false;
			} else if (row.LinkedTicked && row.FileName == filename){
				row.LinkedTicked = false;
			}
		}
	}

	
	
}