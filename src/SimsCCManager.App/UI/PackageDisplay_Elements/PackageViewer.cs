using Godot;
using System;

public partial class PackageViewer : MarginContainer
{
	public Texture2D thumbnail = new();
	public string PackageName = "";
	public string packageinfo = "";
	public bool hasthumbnail = false;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<TextureRect>("MarginContainer/HBoxContainer/MarginContainer/TextureRect").Texture = thumbnail;
		GetNode<Label>("MarginContainer/HBoxContainer/ScrollContainer/MarginContainer2/VBoxContainer/PackageViewer_PackageName").Text = PackageName;
		GetNode<RichTextLabel>("MarginContainer/HBoxContainer/ScrollContainer/MarginContainer2/VBoxContainer/PackageViewer_Information").Text = packageinfo;
		GetNode<TextureRect>("MarginContainer/HBoxContainer/MarginContainer/TextureRect").Visible = hasthumbnail;
	}
}
