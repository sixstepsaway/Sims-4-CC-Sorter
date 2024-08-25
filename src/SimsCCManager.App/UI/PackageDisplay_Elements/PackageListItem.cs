using Godot;
using SimsCCManager.Packages.Containers;
using System;
using System.Collections.Generic;

public partial class PackageListItem : HBoxContainer
{
	public SimsPackage package = new();
	public string packagename = "";
	public string renamedname = "";	
	Node2D spinner;
	LineEdit packagebox;
	List<string> titles = new();
	int currtitle = -1;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		spinner = GetNode<Node2D>("MarginContainer/ButtonSpin");
		packagebox = GetNode<LineEdit>("LineEdit");
		packagebox.Text = package.FileName.Replace(".package", "");
		packagebox.TextChanged += (text) => NameChanged(text);
		GetNode<Button>("MarginContainer/Button").Pressed += () => GetPackageName();
		if (package.Game == Games.Sims2){
			Sims2ScanData scandata = package.ScanData as Sims2ScanData;
			if (scandata.XMLData != null){
				foreach (S2XML xml in scandata.XMLData){
					if (xml.Title != null) titles.Add(xml.Title);
				}
			}
			if (scandata.CPFData != null){
				foreach (S2CPF cpf in scandata.CPFData){
					if (cpf.Title != null) titles.Add(cpf.Title);
				}
			} 
			if (scandata.CTSSData != null){
				foreach (S2CTSS ctss in scandata.CTSSData){
					if (ctss.Title != null) titles.Add(ctss.Title);
				}
			} 
			if (scandata.STRData != null){
				foreach (S2STR str in scandata.STRData){
					if (str.Title != null) titles.Add(str.Title);
				}
			}
		} else if (package.Game == Games.Sims3){
			Sims3ScanData scandata = package.ScanData as Sims3ScanData;
		} else if (package.Game == Games.Sims4){
			Sims4ScanData scandata = package.ScanData as Sims4ScanData;
		}
	}

	public void GetPackageName(){		
		Tween tween = CreateTween();
		tween.TweenProperty(spinner, "rotation", 6.2657320147f, 0.2f).SetTrans(Tween.TransitionType.Spring).SetEase(Tween.EaseType.In);
		tween.Play();
		currtitle++;
		packagebox.Text = titles[currtitle];
		renamedname = packagebox.Text;
	}

	private void NameChanged(string text){
		GD.Print(string.Format("Name changed to {0}", text));
		renamedname = text;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
