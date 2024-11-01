using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using Godot;
using SimsCCManager.Containers;
using SimsCCManager.Debugging;
using SimsCCManager.Globals;
using SimsCCManager.Settings.Loaded;
using SimsCCManager.UI.Themes;

namespace SimsCCManager.Settings.SettingsSystem
{
    /// <summary>
    /// 
    /// A class that holds the settings applicable to this specific program.
    /// 
    /// </summary>
    public class SettingsOptions {
        public ThemeColors LoadedTheme { get; set; }
        public bool InstanceLoaded {get; set;} = false;
        public string LastInstanceLoaded {get; set;} = "";
        public List<ThemeColors> ThemeOptions {get; set;} 
        public bool DebugMode {get; set;} = false;
        public List<Instance> Instances {get; set;}
        public bool LimitCPU {get; set;} = false;
        public bool ShowTali {get; set;} = true;
        public bool LoadLatestInstance {get; set;} = false;
        public Instance CurrentInstance {get; set;}

        public SettingsOptions(){
            LoadedTheme = new();
            ThemeOptions = new();
            Instances = new();
        }

        public StringBuilder GetStringBuilder(){
            StringBuilder sw = new();
            sw.AppendLine(string.Format("{0}={1}", "LoadedTheme", this.LoadedTheme.Identifier));
            sw.AppendLine(string.Format("{0}={1}", "InstanceLoaded", this.InstanceLoaded));
            sw.AppendLine(string.Format("{0}={1}", "LastInstanceLoaded", this.LastInstanceLoaded));
            sw.AppendLine(string.Format("{0}={1}", "DebugMode", this.DebugMode));
            sw.AppendLine(string.Format("{0}={1}", "LimitCPU", this.LimitCPU));
            sw.AppendLine(string.Format("{0}={1}", "ShowTali", this.ShowTali));
            sw.AppendLine(string.Format("{0}={1}", "LoadLatestInstance", this.LoadLatestInstance));
            sw.AppendLine("[INSTANCES]");
            foreach (Instance instance in this.Instances){
                sw.AppendLine(string.Format("{0}={1}", "InstanceName", instance.Name));
                sw.AppendLine(string.Format("{0}={1}", "InstanceLocation", instance.InstanceLocation));
                sw.AppendLine(string.Format("{0}={1}", "Game", instance.Game));
                sw.AppendLine(string.Format("{0}={1}", "Identifier", instance.Identifier));
            }
            return sw;
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
                return null;
            }
        }

        public void SetProperty(string propName, dynamic input){
            Logging.WriteDebugLog(string.Format("Processing property {0}, value {1}", propName, input.ToString()));
            var prop = this.ProcessProperty(propName);
            PropertyInfo property = this.GetType().GetProperty(propName);
            Logging.WriteDebugLog(string.Format("Property type: {0}", property.PropertyType));
            if (property != null){
                if (property.PropertyType == typeof(Godot.Color)){
                    Godot.Color newcolor = Godot.Color.FromHtml(input);
                    property.SetValue(this, newcolor);
                } else if (property.PropertyType == typeof(Guid)){
                    if (input.GetType() == typeof(string)){
                        string inp = input as string;
                        property.SetValue(this, Guid.Parse(inp));
                    } else if (input.GetType() == typeof(Guid)){
                        property.SetValue(this, input);
                    }
                } else if (property.PropertyType == typeof(string)) {
                    property.SetValue(this, input as string);
                } else if (property.PropertyType == typeof(bool)) {
                    if (input.GetType() == typeof(bool)) {
                        property.SetValue(this, input);
                    } else if (input.GetType() == typeof(string)){
                        property.SetValue(this, bool.Parse(input));
                    }                    
                } else if (property.PropertyType == typeof(ThemeColors)) {
                    if (input.GetType() == typeof(Guid)){
                        Logging.WriteDebugLog(string.Format("Setting theme property from: {0}", input.ToString()));
                        property.SetValue(this, this.ThemeOptions.Where(x => x.Identifier == input).First());
                        Logging.WriteDebugLog(string.Format("Loaded theme is: {0}", LoadedTheme.ThemeName));
                    } else if (input.GetType() == typeof(ThemeColors)){
                        property.SetValue(this, input);
                    } else if (input.GetType() == typeof(string)){
                        property.SetValue(this, this.ThemeOptions.Where(x => x.Identifier == Guid.Parse(input)).First());
                    }
                } else if (property.PropertyType == typeof(List<ThemeColors>)) {
                    property.SetValue(this, input);
                } else if (property.PropertyType == typeof(List<Instance>)) {
                    property.SetValue(this, input);
                }
            }            
        }

        public object ProcessProperty(string propName){
            return this.GetType().GetProperty(propName).GetValue (this, null);
        }

        public void ChangeSetting(string setting, dynamic value){
            SetProperty(setting, value);
            SettingsFileManagement.SaveSettings();
            if (setting == "DebugMode"){
                GlobalVariables.RedoLocations();
            }
        }
        public void ChangeSetting(Guid setting, dynamic value){
            SetProperty(setting.ToString(), value);
            SettingsFileManagement.SaveSettings();
        }

        public void ChangeSetting(Instance instance){
            Instances.Add(instance);
            SettingsFileManagement.SaveSettings();
        }
    }

    public class SettingsFileManagement {
        public static string LastInstance = "";
        private static SettingsOptions defaultSettings = new(){
            InstanceLoaded = false,
            DebugMode = false,
            LimitCPU = true,
            ShowTali = true,
            LoadLatestInstance = true
        };

        public static void SaveSettings(){
            if (!Directory.Exists(GlobalVariables.AppFolder)){
                DirectoryInfo di = Directory.CreateDirectory(GlobalVariables.AppFolder);
            }
            using (StreamWriter streamWriter = new(GlobalVariables.SettingsFile)){
                StringBuilder sb = LoadedSettings.SetSettings.GetStringBuilder();
                streamWriter.Write(sb);
            }  
            return;
        }
        public static void LoadSettings(){
            if (File.Exists(GlobalVariables.SettingsFile)){
                using (StreamReader streamReader = new StreamReader(GlobalVariables.SettingsFile)){
                    bool eos = false;                
                    while (eos == false){
                        if(!streamReader.EndOfStream){
                            string setting = streamReader.ReadLine();
                            if (setting.Contains('[')){
                                string line = setting.Replace("[", "");
                                line = line.Replace("]", "");
                                NamedSetting(line, streamReader);                   
                            } else {
                                string[] line = setting.Split("=");
                                if (LoadedSettings.SetSettings.GetProperty(line[0]) != null){
                                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Setting {0}:{1}", line[0], line[1]));
                                    LoadedSettings.SetSettings.SetProperty(line[0], line[1]);
                                }
                            }
                        } else {
                            eos = true;
                        }
                    }
                    streamReader.Close();
                }
            } else {
                LoadedSettings.SetSettings = defaultSettings;
                LoadThemes();
                LoadedSettings.SetSettings.LoadedTheme = LoadedSettings.SetSettings.ThemeOptions.Where(x => x.Identifier == Guid.Parse("4305e7e5-0a03-4625-a4ab-2a688ae9d9e3")).First();
                SaveSettings();
            }
        }


        public static void NamedSetting(string name, StreamReader reader){
            bool eos = false;
            while (eos == false){
                if(!reader.EndOfStream){
                    if (name == "INSTANCES"){
                        bool instances = true;                
                        while (instances == true){
                            string setting = reader.ReadLine();
                            if (setting.Contains('[')){
                                instances = false;
                                string line = setting.Replace("[", "");
                                line = line.Replace("]", "");
                                NamedSetting(line, reader);
                            } else {
                                Instance instance = new();
                                string[] line = setting.Split("="); 
                                instance.Name = line[1];
                                setting = reader.ReadLine();
                                if (reader.EndOfStream) { 
                                    eos = true; 
                                    instances = false;
                                }
                                line = setting.Split("=");
                                instance.InstanceLocation = line[1];
                                setting = reader.ReadLine();
                                if (reader.EndOfStream) { 
                                    eos = true; 
                                    instances = false;
                                }
                                line = setting.Split("=");
                                instance.Game = line[1];
                                setting = reader.ReadLine();
                                if (reader.EndOfStream) { 
                                    eos = true; 
                                    instances = false;
                                }
                                line = setting.Split("=");
                                instance.Identifier = Guid.Parse(line[1]);
                                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Looking for inifile at {0}", instance.XMLfile()));
                                if (File.Exists(instance.XMLfile())){
                                    LoadedSettings.SetSettings.Instances.Add(instance);
                                }
                            }
                        }
                    }
                } else {
                    eos = true;
                }
            }
        }
        
        public static void LoadThemes(){       
            LoadedSettings.SetSettings.ThemeOptions = new();     
            if (Directory.Exists(Themes.ThemeFolder) && File.Exists(Path.Combine(Themes.ThemeFolder, string.Format("{0}.ini", "DefaultDark")))){                
                List<string> themefiles = Directory.GetFiles(Themes.ThemeFolder).ToList();
                foreach (string themefile in themefiles){
                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Loading theme {0}", themefile));
                    ThemeColors simpletheme = new();
                    using (StreamReader streamReader = new StreamReader(themefile)){
                        bool eos = false;                
                        while (eos == false){
                            if(!streamReader.EndOfStream){
                                string setting = streamReader.ReadLine();
                                var line = setting.Split("=");
                                simpletheme.SetProperty(line[0], line[1]);
                                
                            } else {
                                eos = true;
                            }
                        }
                        streamReader.Close();
                    }
                    LoadedSettings.SetSettings.ThemeOptions.Add(simpletheme);
                }
            } else {
                Themes.ReestablishThemes();
            }
        }
    }    
}