using Godot;
using System;

public partial class PackageViewer : MarginContainer
{
	Texture2D thumbnail = new();
	string PackageName = "";
	string packageinfo = "";
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<TextureRect>("MarginContainer/HBoxContainer/MarginContainer/TextureRect").Texture = thumbnail;
		GetNode<Label>("MarginContainer/HBoxContainer/ScrollContainer/MarginContainer2/VBoxContainer/PackageViewer_PackageName").Text = PackageName;
		GetNode<RichTextLabel>("MarginContainer/HBoxContainer/ScrollContainer/MarginContainer2/VBoxContainer/PackageViewer_Information").Text = packageinfo;
	}
}
