using Godot;
using SimsCCManager.Containers;
using SimsCCManager.Globals;
using SimsCCManager.UI.Utilities;
using System;
using System.Collections.Generic;

public partial class ProfileManagement : MarginContainer
{
	public delegate void ChangeProfileEvent(string profile, int change);
	public ChangeProfileEvent ChangeProfile;
	public delegate void NewProfileEvent(ProfileInfo profile, string og);	
	public NewProfileEvent MakeNewProfile;
	public NewProfileEvent EditExistingProfile;
	public delegate void ErrorEvent(string message);
	public ErrorEvent CallError;
	PackedScene profilelist = GD.Load<PackedScene>("res://UI/PackageDisplay_Elements/subwindows/ProfileItem.tscn");
	VBoxContainer profiles;
	public string selecteditem = "";
	MarginContainer namenewprof;
	string profilename = "";
	bool localdata = false;
	bool localsaves = false;
	bool localsettings = false;
	bool localmedia = false;
	List<string> profilenames = new();
	bool editing = false;
	string editingprof = "";
	ProfileInfo active = new();
	HolderNode holdernode;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		holdernode = GetWindow().GetNode<HolderNode>("MainWindow/HolderNode");
		GetNode<Button>("MarginContainer/VBoxContainer/HBoxContainer2/MarginContainer2/VBoxContainer/NewProfile_Button").Pressed += () => NewProfile();
		GetNode<Button>("MarginContainer/VBoxContainer/HBoxContainer2/MarginContainer2/VBoxContainer/EditProfile_Button").Pressed += () => EditProfile();
		GetNode<Button>("MarginContainer/VBoxContainer/HBoxContainer2/MarginContainer2/VBoxContainer/DuplicateProfile_Button").Pressed += () => DuplicateProfile();
		GetNode<Button>("MarginContainer/VBoxContainer/HBoxContainer2/MarginContainer2/VBoxContainer/DeleteProfile_Button").Pressed += () => DeleteProfile();
		GetNode<Button>("MarginContainer/VBoxContainer/HBoxContainer/Close_Button").Pressed += () => CloseScreen();
		profiles = GetNode<VBoxContainer>("MarginContainer/VBoxContainer/HBoxContainer2/MarginContainer/MarginContainer/ScrollContainer/VBoxContainer");
		namenewprof = GetNode<MarginContainer>("MarginContainer2");
		GetNode<LineEdit>("MarginContainer2/MarginContainer/VBoxContainer/LineEdit").TextChanged += (t) => ProfileNameChanged(t);
		GetNode<Button>("MarginContainer2/MarginContainer/VBoxContainer/HBoxContainer/Confirm_Button").Pressed += () => ConfirmNewProf();
		GetNode<Button>("MarginContainer2/MarginContainer/VBoxContainer/HBoxContainer/Cancel_Button").Pressed += () => CancelNewProf();


		GetNode<CustomCheckButton>("MarginContainer2/MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer/LocalSaves_CheckButton").CheckToggled += () => LocalSavesCheck();
		GetNode<CustomCheckButton>("MarginContainer2/MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer2/LocalSettings_CheckButton").CheckToggled += () => LocalSettingsCheck();
		GetNode<CustomCheckButton>("MarginContainer2/MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer3/LocalMedia_CheckButton").CheckToggled += () => LocalMediaCheck();
		GetNode<CustomCheckButton>("MarginContainer2/MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer4/LocalData_CheckButton").CheckToggled += () => LocalDataCheck();
	}

    private void LocalDataCheck()
    {
        localdata = !localdata;
    }


    private void LocalMediaCheck()
    {
        localmedia = !localmedia;
    }


    private void LocalSettingsCheck()
    {
        localsettings = !localsettings;
    }


    private void LocalSavesCheck()
    {
        localsaves = !localsaves;
    }


    public void PopulateProfiles(List<string> Profiles, ProfileInfo ActiveProfile){
		foreach (ProfileItem pi in profiles.GetChildren()){
			pi.QueueFree();
		}
		active = ActiveProfile;
		profilenames = Profiles;
		foreach (string profile in Profiles){
			bool a = false;
			if (ActiveProfile.ProfileName == profile){
				a = true;
			} else {
				a = false;
			}
			AddProfile(profile, a);
		}
		holdernode.UpdateTheme(GetTree());
		//UIUtilities.UpdateTheme(GetTree());
	}

	private void AddProfile(string name, bool active){
		ProfileItem p = profilelist.Instantiate() as ProfileItem;
		p.ItemName = name;
		p.Active = active;		
		p.ItemSelected += (x, y) => ItemSelected(x, y);
		profiles.AddChild(p);
	}

	private void ItemSelected(bool selected, string item){
		if (selected){
			foreach (ProfileItem p in profiles.GetChildren()){
				if (p.ItemName != item){
					p.selected = false;
				}
			}
			selecteditem = item;
		}			
	}

    private void CloseScreen()
    {
		if (profiles.GetChildCount() != 0) {
			for (int i = 0; i < profiles.GetChildCount(); i++){
				profiles.GetChild(i).QueueFree();
			}
		}
        this.Visible = false;
    }


    private void DeleteProfile()
    {
        if (selecteditem != "Default"){
			ChangeProfile(selecteditem, 0);
			profilenames.Remove(selecteditem);
			PopulateProfiles(profilenames, active);
		} else {
			CallError.Invoke("Can't delete default profile."); 
		}
    }


    private void DuplicateProfile()
    {
        ChangeProfile(selecteditem, 1);
		profilenames.Add(string.Format("{0}_Copy", selecteditem));
		PopulateProfiles(profilenames, active);	
    }


    private void EditProfile()
    {	
		profilename = "";
		GetNode<LineEdit>("MarginContainer2/MarginContainer/VBoxContainer/LineEdit").Text = selecteditem;
		namenewprof.Visible = true;	
		editing = true;
		editingprof = selecteditem;
		ProfileInfo pr = new(){ ProfileName = selecteditem };
		pr.MakeInfoLocation();
		pr = Utilities.LoadProfile(pr);
		GetNode<LineEdit>("MarginContainer2/MarginContainer/VBoxContainer/LineEdit").Text = selecteditem;
		GetNode<CustomCheckButton>("MarginContainer2/MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer/LocalSaves_CheckButton").IsToggled = pr.LocalSaves;
		GetNode<CustomCheckButton>("MarginContainer2/MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer2/LocalSettings_CheckButton").IsToggled = pr.LocalSettings;
		GetNode<CustomCheckButton>("MarginContainer2/MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer3/LocalMedia_CheckButton").IsToggled = pr.LocalMedia;
		GetNode<CustomCheckButton>("MarginContainer2/MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer4/LocalData_CheckButton").IsToggled = pr.LocalData;
    }

	private void ResetToggles(){
		GetNode<CustomCheckButton>("MarginContainer2/MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer/LocalSaves_CheckButton").IsToggled = false;
		GetNode<CustomCheckButton>("MarginContainer2/MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer2/LocalSettings_CheckButton").IsToggled = false;
		GetNode<CustomCheckButton>("MarginContainer2/MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer3/LocalMedia_CheckButton").IsToggled = false;
		GetNode<CustomCheckButton>("MarginContainer2/MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer4/LocalData_CheckButton").IsToggled = false;
		localdata = false;
		localsaves = false;
		localmedia = false;
		localsettings = false;
	}


    private void NewProfile()
    {
		profilename = "";
        GetNode<LineEdit>("MarginContainer2/MarginContainer/VBoxContainer/LineEdit").Text = "";
		namenewprof.Visible = true;	
    }

	private void ProfileNameChanged(string name){
		profilename = name;
	}

	private void CancelNewProf()
    {
        namenewprof.Visible = false;
		profilename = "";
		editing = false;
    }

    private void ConfirmNewProf()
    {
		if (profilenames.Contains(profilename) && profilename != editingprof){
			CallError.Invoke("This profile name is already in use."); 
		} else if (profilenames.Contains(profilename)){
			CallError.Invoke("This profile name is already in use."); 
		} else if (editing){			
			namenewprof.Visible = false;
			ProfileInfo pr = new() { ProfileName = profilename,
			LocalData = GetNode<CustomCheckButton>("MarginContainer2/MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer4/LocalData_CheckButton").IsToggled,
			LocalSaves = GetNode<CustomCheckButton>("MarginContainer2/MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer/LocalSaves_CheckButton").IsToggled,
			LocalMedia = GetNode<CustomCheckButton>("MarginContainer2/MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer3/LocalMedia_CheckButton").IsToggled,
			LocalSettings = GetNode<CustomCheckButton>("MarginContainer2/MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer2/LocalSettings_CheckButton").IsToggled};
			editing = false;
			EditExistingProfile.Invoke(pr, editingprof);
			ResetToggles();
		} else {
			//profilename = GetNode<LineEdit>("MarginContainer2/HBoxContainer/LineEdit").Text;
			namenewprof.Visible = false;
			ProfileInfo pr = new() { ProfileName = profilename,
			LocalData = GetNode<CustomCheckButton>("MarginContainer2/MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer4/LocalData_CheckButton").IsToggled,
			LocalSaves = GetNode<CustomCheckButton>("MarginContainer2/MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer/LocalSaves_CheckButton").IsToggled,
			LocalMedia = GetNode<CustomCheckButton>("MarginContainer2/MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer3/LocalMedia_CheckButton").IsToggled,
			LocalSettings = GetNode<CustomCheckButton>("MarginContainer2/MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer2/LocalSettings_CheckButton").IsToggled};
			pr.SaveProfile();
			MakeNewProfile.Invoke(pr, "");
			AddProfile(profilename, false);
			holdernode.UpdateTheme(GetTree());
			//UIUtilities.UpdateTheme(GetTree());
			ResetToggles();
		}		
    }
}
