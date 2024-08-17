using Godot;
using System;

public partial class GamePickerBox : MarginContainer
{
	[Export]
	Texture2D iconimage;
	[Export]
	string gamename = "";
	Games game = Games.Null;
	[Signal]
	public delegate void PickedGameEventHandler();
	bool picked = false;

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<Label>("Item/MarginContainer2/Label2").Text = gamename;
		GetNode<TextureRect>("Item/MarginContainer/TextureRect").Texture = iconimage;
		if (gamename == "Sims 2") game = Games.Sims2;
		if (gamename == "Sims 3") game = Games.Sims3;
		if (gamename == "Sims 1" || gamename == "The Sims") game = Games.Sims1;
		if (gamename == "Sims 4") game = Games.Sims4;
		if (gamename == "Sims Medieval") game = Games.SimsMedieval;
		if (gamename == "Sim City 5") game = Games.SimCity5;
	}
	public void Untoggle(){
		GetNode<Button>("Button").ButtonPressed = false;
		picked = false;
	}

	private void _on_button_pressed(){
		if (picked){
			picked = !picked;
			EmitSignal("PickedGame", "");
			GD.Print(string.Format("Picked Game: {0}", "Unpicked"));
		} else {
			picked = !picked;
			EmitSignal("PickedGame", gamename);
			GD.Print(string.Format("Picked Game: {0}", gamename));
		}
		int mypos = GetIndex();
		for (int i = 0; i < GetParent().GetChildCount(); i++){
			if (i != mypos){
				(GetParent().GetChild(i) as GamePickerBox).Untoggle();
			}
		}
	}
}
