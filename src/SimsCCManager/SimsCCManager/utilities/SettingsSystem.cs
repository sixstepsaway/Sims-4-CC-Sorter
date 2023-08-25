using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimsCCManager.Settings
{
    /// <summary>
    /// A simple class for saving and retrieving settings.
    /// </summary>
    public class Setting {
        public string SettingName {get; set;}
        public string SettingValue {get; set;}
    }

    /// <summary>
    /// Creates and initializes a settings file into which I can save settings and information.
    /// Replace "NameSpace" with app, replace "App" in AppName with App's name.
    /// </summary>
    public class SettingsFile
    {
        public static string AppName = "Sims CC Manager";
        public static string MyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string AppFolder = Path.Combine(MyDocuments, AppName);
        public static string SettingFile = Path.Combine(AppFolder, "Settings.ini");
        public static string LastInstance = "";
        public static List<Setting> Settings = new();

        public static void SaveSetting(string name, string value){
            Setting setting = Settings.Where(x => x.SettingName == name).FirstOrDefault();
            if (setting != null){
                Settings.Remove(setting);
            }
            Settings.Add(new Setting(){SettingName = name, SettingValue = value});                    
            SaveSettingsFile();
        }

        public static void AddInstance(string value){
            if (LoadSetting("Instances") == null){
                Setting setting = new(){SettingName = "Instances", SettingValue = value};
                SaveSetting(setting.SettingName, setting.SettingValue);
            } else {
                Setting setting = new() {SettingName = "Instances", SettingValue = LoadSetting("Instances")};
                setting.SettingValue = string.Format("{0};{1}", setting.SettingValue, value);
                SaveSetting(setting.SettingName, setting.SettingValue);
            }
        }

        public static string LoadSetting (string name){
            //var setting = Settings.Where(x => x.SettingName == name).First().SettingValue;
            if (Settings.Where(x => x.SettingName == name).Count() != 0){
                return Settings.Where(x => x.SettingName == name).First().SettingValue;
            } else {
                return null;
            }
        }
        
        public static void SaveSettingsFile(){
            if (!Directory.Exists(AppFolder)){
                DirectoryInfo di = Directory.CreateDirectory(AppFolder);
            }
            using (StreamWriter streamWriter = new(SettingFile)){
                foreach (Setting setting in Settings){
                    streamWriter.WriteLine(string.Format("{0}={1}", setting.SettingName, setting.SettingValue));
                }
                streamWriter.Flush();
                streamWriter.Close();
            }  
            return;          
        }

        public static void LoadSettingsFile(){            
            if (File.Exists(SettingFile)){
                Settings.Clear();
                using (StreamReader streamReader = new StreamReader(SettingFile)){
                    bool eos = false;                
                    while (eos == false){
                        if(!streamReader.EndOfStream){
                            string setting = streamReader.ReadLine();
                            var line = setting.Split("=");
                            Setting newsetting = new()
                            {
                                SettingName = line[0],
                                SettingValue = line[1]
                            };
                            Settings.Add(newsetting);
                        } else {
                            eos = true;
                        }
                    }
                    streamReader.Close();
                }
                return;
            } else {
                return;
            }            
        }
    }
}