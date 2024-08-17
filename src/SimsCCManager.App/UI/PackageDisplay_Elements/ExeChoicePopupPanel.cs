using Godot;
using Microsoft.VisualBasic;
using SimsCCManager.Globals;
using SimsCCManager.Settings.Loaded;
using System;
using System.Collections.Generic;
using System.IO;

public partial class ExeChoicePopupPanel : Control
{
	[Signal]
	public delegate void PickedExeEventHandler();
	ScrollContainer scrollContainer;
	VBoxContainer vBoxContainer;
	public List<Executable> Executables = new();
	PackedScene ExeChoiceOption = GD.Load<PackedScene>("res://UI/PackageDisplay_Elements/Exe_Option.tscn");
	public string instancedatafolder = "";
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		scrollContainer = GetNode<ScrollContainer>("ScrollContainer");
		vBoxContainer = scrollContainer.GetNode<VBoxContainer>("VBoxContainer");
	}

	public void UpdateExes(){
		for (int i = 0; i < vBoxContainer.GetChildCount(); i++){
			vBoxContainer.GetChild(i).QueueFree();
		}		
		foreach (Executable exe in Executables){
			Exe_Option option = ExeChoiceOption.Instantiate() as Exe_Option;
			if (File.Exists(Utilities.ExeIconName(exe, instancedatafolder))){
				Texture2D image = ImageTexture.CreateFromImage(Image.LoadFromFile(Utilities.ExeIconName(exe, instancedatafolder)));
            	if (exe.Selected){
					GetParent().GetParent().GetNode<TextureRect>("HBoxContainer/ExeIcon_Container/HBoxContainer/MarginContainer/ExeIcon_Image").Texture = image;
					GetParent().GetParent().GetNode<Label>("HBoxContainer/Name_Container/VBoxContainer/ExeName_Label").Text = "Default";
					GetParent().GetParent().GetNode<Label>("HBoxContainer/Name_Container/VBoxContainer/ExeExe_Label").Text = exe.Exe;
				}
				option.Icon = image;
			} else {
				Texture2D image = Utilities.ExtractIcon(exe, instancedatafolder);
				option.Icon = image;
				if (exe.Selected){
					GetParent().GetParent().GetNode<TextureRect>("HBoxContainer/ExeIcon_Container/HBoxContainer/MarginContainer/ExeIcon_Image").Texture = image;
					GetParent().GetParent().GetNode<Label>("HBoxContainer/Name_Container/VBoxContainer/ExeName_Label").Text = "Default";
					GetParent().GetParent().GetNode<Label>("HBoxContainer/Name_Container/VBoxContainer/ExeExe_Label").Text = exe.Exe;
				}
			}
			option.ExeName = exe.Exe;	
			option.SetMeta("ExeID", exe.Name);
			option.Connect("SelectedExe", new Callable(this, MethodName._on_exe_chosen));
			vBoxContainer.AddChild(option);			
		}
		int execount = Executables.Count;
		if (Executables.Count > 5) execount = 5;
		float height = execount * 75;
		Size = new(height, Size.Y);
	}

	private void _on_exe_chosen(string exename){
		EmitSignal("PickedExe", exename);
	}
}
