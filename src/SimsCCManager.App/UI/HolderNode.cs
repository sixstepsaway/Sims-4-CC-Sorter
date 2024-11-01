using Godot;
using SimsCCManager.Debugging;
using SimsCCManager.Globals;
using SimsCCManager.Settings.Loaded;
using SimsCCManager.UI.Themes;
using SimsCCManager.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class HolderNode : Control
{
	
	public override void _Ready()
	{
	}

	public void UpdateTheme(SceneTree tree){
		CallDeferred(nameof(UpdateThemeRedirected), tree);
	}

	private void UpdateThemeRedirected(SceneTree tree){
            try {
				if (GlobalVariables.DebugMode) Logging.WriteDebugLog("Updating theme!");
				bool islight = false;
				ThemeColors theme = LoadedSettings.SetSettings.LoadedTheme;
				if (theme.BackgroundColor.Luminance > 0.5) islight = true;		
				bool accentlight = false;
				if (theme.AccentColor.Luminance > 0.5) accentlight = true;
				Color fontcolor = Color.FromHtml("F7F7F7");
				if (accentlight) {
					fontcolor = Color.FromHtml("06090E");
				}
				List<Node> labels = tree.GetNodesInGroup("AccentLabels").ToList();
				foreach (Label label in labels){
					label.AddThemeColorOverride("font_color", fontcolor);
				}
				
				List<Node> accentbuttons = tree.GetNodesInGroup("AccentTextButtons").ToList();
				foreach (Button textbutton in accentbuttons){
					textbutton.AddThemeColorOverride("font_color", fontcolor);
				}
				
				List<Node> textbuttons = tree.GetNodesInGroup("TextButtons").ToList();
				foreach (Button button in textbuttons){
					button.AddThemeColorOverride("font_color", fontcolor);
				}
				List<Node> socials = tree.GetNodesInGroup("SocialsButtons").ToList();
				foreach (socials_button button in socials){
					button.SetColors();
				}
				List<Node> accents = tree.GetNodesInGroup("AccentColorBox").ToList();
				foreach (ColorRect accent in accents){
					accent.Color = theme.AccentColor;
				}
				List<Node> plainbg = tree.GetNodesInGroup("PlainBG").ToList();
				foreach (ColorRect bg in plainbg){
					bg.Color = theme.BackgroundColor;
				}
				List<Node> backgrounds = tree.GetNodesInGroup("Background").ToList();
				foreach (Background background in backgrounds){
					background.ChangeTheme();
				}
				List<Node> bglabels = tree.GetNodesInGroup("Labels").ToList();
				foreach (Label label in bglabels ){
					label.AddThemeColorOverride("font_color", theme.MainTextColor);
				}
				List<Node> checkbuttons = tree.GetNodesInGroup("CheckButtons").ToList();
				foreach (CustomCheckButton button in checkbuttons){
					button.UpdateColors();
				}
				List<Node> MainButtons = tree.GetNodesInGroup("MainButtons").ToList();
				foreach (Button button in MainButtons){
					button.AddThemeColorOverride("font_color", theme.MainTextColor.Darkened(0.4f));
				}
				List<Node> mmbuttons = tree.GetNodesInGroup("MMButtons").ToList();
				foreach (mm_button button in mmbuttons){
					button.UpdateColors();
				}
				List<Node> packageeditorbuttons = tree.GetNodesInGroup("PackageEditorButtons").ToList();
				foreach (topbar_button button in packageeditorbuttons){
					button.SetColors();
				}
				List<Node> lineedits = tree.GetNodesInGroup("LineEdits").ToList();
				foreach (LineEdit linedit in lineedits){
					Color font = UIUtilities.GetFGColor(theme.DataGridA);
					linedit.AddThemeColorOverride("font_color", font);
					//GD.Print(string.Format("Lineedit background color: {0}, V: {1}", theme.DataGridA.ToHtml(), theme.DataGridA.V));
					if (theme.DataGridA.Luminance > 0.5) {
						font.V += 0.5f;
					} else {
						font.V -= 0.5f;
					}                
					linedit.AddThemeColorOverride("font_placeholder_color", font);
					StyleBoxFlat sb = linedit.GetThemeStylebox("normal") as StyleBoxFlat;
					sb.BgColor = theme.DataGridA;
					linedit.AddThemeStyleboxOverride("normal", sb);
				}
				List<Node> panels = tree.GetNodesInGroup("PanelsWBorders").ToList();
				foreach (Panel panel in panels){
					StyleBoxFlat sb = panel.GetThemeStylebox("panel") as StyleBoxFlat;
					sb.BorderColor = theme.ButtonMain;
					sb.BgColor = theme.BackgroundColor;
					panel.RemoveThemeStyleboxOverride("panel");
					panel.AddThemeStyleboxOverride("panel", sb);
				}
				List<Node> buttonswithborders = tree.GetNodesInGroup("ButtonsWithBorders").ToList();
				foreach (Button button in buttonswithborders){
					StyleBoxFlat sb = button.GetThemeStylebox("hover") as StyleBoxFlat;
					sb.BorderColor = theme.ButtonMain;
					button.RemoveThemeStyleboxOverride("hover");
					button.AddThemeStyleboxOverride("hover", sb);
					sb = button.GetThemeStylebox("pressed") as StyleBoxFlat;
					sb.BorderColor = theme.ButtonMain;
					button.RemoveThemeStyleboxOverride("pressed");
					button.AddThemeStyleboxOverride("pressed", sb);
				}
				List<Node> contlabels = tree.GetNodesInGroup("SimpleContrastLabel").ToList();
				foreach (Label label in contlabels ){
					Color font = UIUtilities.GetFGColor(theme.BackgroundColor);
					label.AddThemeColorOverride("font_color", font);
				}
				List<Node> rcm_labels = tree.GetNodesInGroup("RCM_Labels").ToList();
				foreach (Label label in rcm_labels ){
					label.AddThemeFontSizeOverride("font_size", 14);
					Color font = UIUtilities.GetFGColor(theme.BackgroundColor);
					label.AddThemeColorOverride("font_color", font);
				}



				
				List<Node> scrollbar_bar = tree.GetNodesInGroup("Scrollbar_Bar").ToList();
				foreach (Panel panel in scrollbar_bar ){
					StyleBoxFlat sb = panel.GetThemeStylebox("panel") as StyleBoxFlat;
					sb.BgColor = theme.AccentColor;
					panel.AddThemeStyleboxOverride("panel", sb);
				}
				List<Node> scrollbar_bg = tree.GetNodesInGroup("Scrollbar_BG").ToList();
				foreach (ColorRect rect in scrollbar_bg ){                
					Color bg = theme.BackgroundColor.Lightened(0.1f);
					if (islight) bg = theme.BackgroundColor.Darkened(0.1f);
					rect.Color = bg;
				}
				List<Node> scrollbar_button_bg = tree.GetNodesInGroup("ScrollBar_ButtonBG").ToList();
				foreach (Button panel in scrollbar_button_bg ){
					StyleBoxFlat sb = panel.GetThemeStylebox("panel") as StyleBoxFlat;                
					sb.BgColor = theme.DataGridSelected;
					panel.AddThemeStyleboxOverride("panel", sb);
				}

            } catch (Exception e) {
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("UI Theme Change Exception: {0}", e.StackTrace));
            }

        }
}
