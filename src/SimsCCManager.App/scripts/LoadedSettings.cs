using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SimsCCManager.Debugging;
using SimsCCManager.Globals;
using SimsCCManager.Settings.SettingsSystem;
using SimsCCManager.UI.Themes;

namespace SimsCCManager.Settings.Loaded
{
    public class LoadedSettings
    {
        public static SettingsOptions SetSettings = new(){
            LoadedTheme = Themes.DefaultDark,
            InstanceLoaded = false,
            DebugMode = false
        };
    }

    public class Instance {
        public string Name { get; set;} = "";
        public string InstanceLocation {get; set;} = "";  
        public string Game {get; set;} = "";
        public Guid Identifier {get; set;} = Guid.Empty;      

        public string XMLfile(){
            return Path.Combine(InstanceLocation, "Instance.xml");
        }

        public dynamic GetProperty(string propName){
            var prop = this.ProcessProperty(propName);
            if (prop.GetType() == typeof(string)){
                return prop.ToString();
            } else if (prop.GetType() == typeof(DateTime)){
                DateTime dt = (DateTime)prop;                
                return dt.ToString("MM/dd/yyyy H:mm");
            } else if (prop.GetType() == typeof(bool)){
                return prop;
            } else if (prop.GetType() == typeof(ThemeColors)){
                return prop;
            } else {
                return prop;
            }
        }

        public void SetProperty(string propName, dynamic input){
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Processing property {0}, value {1}", propName, input.GetType()));
            var prop = this.ProcessProperty(propName);
            PropertyInfo property = this.GetType().GetProperty(propName);
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Property type: {0}", property.PropertyType));
            if (property != null){
                if (property.PropertyType == typeof(Godot.Color)){
                    Godot.Color newcolor = Godot.Color.FromHtml(input);
                    property.SetValue(this, newcolor);
                } else if (property.PropertyType == typeof(Guid)){
                    string inp = input as string;
                    property.SetValue(this, Guid.Parse(inp));
                } else if (property.PropertyType == typeof(string)) {
                    property.SetValue(this, input as string);
                } else if (property.PropertyType == typeof(bool)) {
                    property.SetValue(this, input);
                } else if (property.PropertyType == typeof(ThemeColors)) {
                    property.SetValue(this, input);
                } else if (property.PropertyType == typeof(List<ThemeColors>)) {
                    property.SetValue(this, input);
                } else if (property.PropertyType == typeof(List<Instance>)) {
                    property.SetValue(this, input);
                } else {
                    property.SetValue(this, input);
                }
            }            
        }

        public object ProcessProperty(string propName){
            return this.GetType().GetProperty(propName).GetValue (this, null);
        }
    }

    public class Category {
        public string Name {get; set;} = "";
        public Guid Identifier {get; set;} = Guid.NewGuid();
        public string Description {get; set;} = "";
        public Godot.Color Background {get; set;} = Godot.Color.FromHtml("FFFFFF");
        public Godot.Color TextColor {get; set;} = Godot.Color.FromHtml("000000");
        public int Packages {get; set;} = 0;
    }

    public class Executable {
        public string Path {get; set;} = "";
        public string Exe {get; set;} = "";
        public string Arguments {get; set;} = "N/a";
        public bool Selected {get; set;} = false;
        public string Name {get; set;} = "";
    }
}