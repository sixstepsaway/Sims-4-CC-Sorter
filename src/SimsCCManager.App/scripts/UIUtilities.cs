using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using SimsCCManager.Debugging;
using SimsCCManager.Globals;
using SimsCCManager.Settings.Loaded;
using SimsCCManager.UI.Containers;
using SimsCCManager.UI.Themes;

namespace SimsCCManager.UI.Utilities
{
    public class UIUtilities
    {
        

        public static Godot.Color GetFGColor(Godot.Color colorinput){
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Input color: {0}.", colorinput.ToHtml()));
            float h;
            float s;
            float v;
            colorinput.ToHsv(out h, out s, out v);
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("H: {0}, S: {1}, V: {2}.", h, s, v));

            float newv;
            
            if (v > 0.5){
                newv = 0.05f; // bright colors - black font
            } else {
                newv = 0.95f; // dark colors - white font
            }

            Godot.Color newcolor = Godot.Color.FromHsv(h, s, newv);
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Output color: {0}.", newcolor.ToHtml()));
            return newcolor;
        }

        public static ToolTip CustomTooltip(string text, Vector2 Position){
            Position = new(Position.X + 10, Position.Y);
            //GD.Print("Instantiating tooltip!");
            ToolTip tooltip = GD.Load<PackedScene>("res://UI/ToolTip.tscn").Instantiate() as ToolTip;
            tooltip.Text = text;    
            tooltip.WindowPosition = (Vector2I)Position;        
            return tooltip;
        }
    }
}