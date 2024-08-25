using Godot;
using Microsoft.VisualBasic;
using SimsCCManager.Globals;
using SimsCCManager.Settings.Loaded;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Versioning;
using System.Threading;

public partial class ExeChoicePopupPanel : Control
{
	public delegate void PickedExeEvent(string exename);
	public PickedExeEvent PickedExe;
	public delegate void ExeIconEvent(Texture2D texture, string ExeName, string ExeExe);
	public ExeIconEvent ExeIcon;
	ScrollContainer scrollContainer;
	VBoxContainer vBoxContainer;
	public List<Executable> Executables = new();
	PackedScene ExeChoiceOption = GD.Load<PackedScene>("res://UI/PackageDisplay_Elements/Exe_Option.tscn");
	public string instancedatafolder = "";
	List<Exe_Option> exeoptions = new();
	Vector2 thissize = Vector2.Zero;
	float sizey = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		scrollContainer = GetNode<ScrollContainer>("ScrollContainer");
		vBoxContainer = scrollContainer.GetNode<VBoxContainer>("VBoxContainer");
		sizey = Size.Y;
	}

	public void UpdateExes(){
		for (int i = 0; i < vBoxContainer.GetChildCount(); i++){
			vBoxContainer.GetChild(i).QueueFree();
		}
		new Thread(() => {			
			foreach (Executable exe in Executables){
				Exe_Option option = ExeChoiceOption.Instantiate() as Exe_Option;
				if (File.Exists(Utilities.ExeIconName(exe, instancedatafolder))){
					Texture2D image = ImageTexture.CreateFromImage(Image.LoadFromFile(Utilities.ExeIconName(exe, instancedatafolder)));
					if (exe.Selected){
						CallDeferred(nameof(ExeIconInvoke), image, "Default", exe.Exe);						
						//GetParent().GetParent().GetNode<TextureRect>("HBoxContainer/ExeIcon_Container/HBoxContainer/MarginContainer/ExeIcon_Image").Texture = image;
						//GetParent().GetParent().GetNode<Label>("HBoxContainer/Name_Container/VBoxContainer/ExeName_Label").Text = "Default";
						//GetParent().GetParent().GetNode<Label>("HBoxContainer/Name_Container/VBoxContainer/ExeExe_Label").Text = exe.Exe;
					}
					option.Icon = image;
				} else {
					Texture2D image = Utilities.ExtractIcon(exe, instancedatafolder);
					option.Icon = image;
					if (exe.Selected){
						CallDeferred(nameof(ExeIconInvoke), image, "Default", exe.Exe);
						//GetParent().GetParent().GetNode<TextureRect>("HBoxContainer/ExeIcon_Container/HBoxContainer/MarginContainer/ExeIcon_Image").Texture = image;
						//GetParent().GetParent().GetNode<Label>("HBoxContainer/Name_Container/VBoxContainer/ExeName_Label").Text = "Default";
						//GetParent().GetParent().GetNode<Label>("HBoxContainer/Name_Container/VBoxContainer/ExeExe_Label").Text = exe.Exe;
					}
				}
				option.ExeName = exe.Exe;	
				option.ExeID = exe.Name;
				option.SelectedExe += (exename) => _on_exe_chosen(exename);
				//option.Connect("SelectedExe", new Callable(this, MethodName._on_exe_chosen));
				exeoptions.Add(option);			
			}
			int execount = Executables.Count;
			if (Executables.Count > 5) execount = 5;
			float height = execount * 75;
			thissize = new(height, sizey);
			CallDeferred(nameof(AddExes));
		}){IsBackground = true}.Start();
	}

	private void AddExes(){
		foreach (Exe_Option exeop in exeoptions){
			vBoxContainer.AddChild(exeop);
		}
	}

	private void ExeIconInvoke(Texture2D texture, string name, string exename){
		ExeIcon.Invoke(texture, name, exename);
	}

	private void _on_exe_chosen(string exename){
		PickedExe.Invoke(exename);
		//EmitSignal("PickedExe", exename);
	}

    public override void _Process(double delta)
    {
        if (thissize != Size && thissize != Vector2.Zero){
			Size = thissize;
		}
		if (sizey != Size.Y){
			sizey = Size.Y;
		}
    }
}
