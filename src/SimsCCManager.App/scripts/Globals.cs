using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Godot;
using Microsoft.Win32;
using SimsCCManager.Containers;
using SimsCCManager.Debugging;
using SimsCCManager.PackageReaders;
using SimsCCManager.PackageReaders.Containers;
using SimsCCManager.Packages.Containers;
using SimsCCManager.Settings.Loaded;
using SimsCCManager.Settings.SettingsSystem;

namespace SimsCCManager.Globals
{
    public class GlobalVariables
    {
        public static string AppName = "Sims CC Manager";
        public static string MyDocuments = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        public static string AppFolder = Path.Combine(MyDocuments, AppName);
        public static string AppFolderNormal = Path.Combine(MyDocuments, AppName);
        public static string AppFolderDebug = Path.Combine(MyDocuments, string.Format("{0}_Debug", AppName));
        
        public static bool DebugMode = true;
        public static bool PortableMode = false;
        public static bool LoggedIn = false;
        public static bool GameRunning = false;
        public static string InstallDirectory = System.Environment.CurrentDirectory;public static string AppFolderStatic = Path.Combine(InstallDirectory, "Sims CC Manager");
        public static string ffmpeg = Path.Combine(InstallDirectory, "tools\\ffmpeg\\bin\\ffmpeg.exe");
        public static string imagemagick = Path.Combine(InstallDirectory, "tools\\imagemagick\\magick.exe");
        //public static string AppDataFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
        //public static string AppldataFolder = Path.Combine(AppDataFolder, "Sims CC Manager");
        public static string SettingsFile = Path.Combine(AppFolder, "Settings.ini");
        public static string tempfolder = Path.Combine(AppFolder, "temp");
        public static string logfolder = Path.Combine(AppFolder, "logs");
        public static string datafolder = Path.Combine(InstallDirectory, "Data");
        public static string overridesfolder = Path.Combine(datafolder, "Overrides");
        public static ConcurrentBag<string> noinfofile = new();

        public static void AddNoInfoFile(string file){
            if (!noinfofile.Contains(file)){
                noinfofile.Add(file);
            }
        }

        public static void RedoLocations(){
            if (DebugMode) {
                AppFolder = AppFolderDebug;
            } else if (PortableMode){
                AppFolder = AppFolderStatic;
            } else {
                AppFolder = AppFolderNormal;
            }
            SettingsFile = Path.Combine(AppFolder, "Settings.ini");
            tempfolder = Path.Combine(AppFolder, "temp");
            logfolder = Path.Combine(AppFolder, "logs");
            SettingsFileManagement.SaveSettings();
        }

        public static GameInstanceBase thisinstance;
        public static List<string> SimsFileExtensions = new(){
            ".package",
            ".sims3pack",
            ".sims2pack",
            "ts4script"
        };

        public static List<string> Sims2Exes = new(){
            "Sims2EP9",
            "Sims2EP9RPC"
        };
        public static List<string> Sims3Exes = new(){
            "TS3W",
            "TS3"
        };
        public static List<string> Sims4Exes = new(){
            "TS4_DX9_x64",
            "TS4_x64"
        };
        
        
    
        
        
        public static void RemoveTempFiles(){
            if (Directory.Exists(tempfolder)){
                Directory.Delete(tempfolder, true);
            }
        }
        
        
        

    }

    public class Utilities {

        public static void CopyDirectoryTree(DirectoryInfo directory, DirectoryInfo outputdirectory){
            string copydest = Path.Combine(outputdirectory.FullName, directory.Name);
            if (!Directory.Exists(copydest)){
                Directory.CreateDirectory(copydest);
            }
            List<string> files = Directory.GetFiles(directory.FullName).ToList();
            if (files.Any()){
                CDT_Files(files, copydest);
            }            
            List<string> directories = Directory.GetDirectories(directory.FullName).ToList();
            if (directories.Any()){
                CDT_Directories(directories, copydest);
            }            
        }

        public static void CDT_Files(List<string> files, string dir){
            DirectoryInfo d = new(dir);            
            foreach (string file in files){
                FileInfo f = new(file);
                string dest = Path.Combine(d.FullName, f.Name);
                File.Copy(file, dest);
            }
        }

        public static void CDT_Directories(List<string> directories, string dir){
            DirectoryInfo d = new(dir);
            foreach (string folder in directories){
                DirectoryInfo fold = new(folder);
                string dest = Path.Combine(d.FullName, fold.Name);
                if (!Directory.Exists(dest)){
                    Directory.CreateDirectory(dest);
                }
                List<string> files = Directory.GetFiles(folder).ToList();
                if (files.Any()){
                    CDT_Files(files, dest);
                }            
                List<string> folders = Directory.GetDirectories(folder).ToList();
                if (directories.Any()){
                    CDT_Directories(directories, dest);
                }
            }
        }


        public static string GetGameVersion(Games game, string folder){
            string ver = "";
            if (game == Games.Sims2) ver = GetSims2Version(folder);
            if (game == Games.Sims3) ver = GetSims3Version(folder);
            if (game == Games.Sims4) ver = GetSims4Version(folder);
            if (ver != "") {
                ver = Regex.Replace(ver, @"[\p{C}-[\t\r\n]]+", "");
            } 
            
            return ver;
        }

        public static string ListToString(List<string> items){
            StringBuilder sb = new();
            int i = 0;
            foreach (string item in items){
                if (i == 0){
                    sb.Append(item.ToString());
                } else {
                    sb.Append(string.Format(", {0}", item.ToString()));
                }
                i++;
            }
            return sb.ToString();
        }

        public static string GetSims4Version(string docfolder){
            string version = "";
            string versionfile = Path.Combine(docfolder, "GameVersion.txt");
            if (File.Exists(versionfile)){
                using (FileStream fileStream = new(versionfile, FileMode.Open, System.IO.FileAccess.Read)){
                    using (StreamReader streamReader = new(versionfile)){
                        version = streamReader.ReadLine();
                        streamReader.Close();
                    }
                    fileStream.Close();
                }
            }
            return version;
        }
        public static string GetSims3Version(string docfolder){
            string version = "";
            string versionfile = Path.Combine(docfolder, "Version.tag");
            if (File.Exists(versionfile)){
                using (FileStream fileStream = new(versionfile, FileMode.Open, System.IO.FileAccess.Read)){
                    using (StreamReader streamReader = new(versionfile)){
                        if (streamReader.ReadLine() == "[Version]") {
                            version = streamReader.ReadLine();
                            version = version.Replace("LatestBase = ", "");
                        };                        
                        streamReader.Close();
                    }
                    fileStream.Close();
                }
            }
            return version;
        }

        public static string GetSims2Version(string docfolder){
            return "LatestVersion";
        }



        public static Sims2Instance LoadS2Instance(string xmlfile){
            Sims2Instance s2 = new();
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Loading instance from {0}.", xmlfile));
            XmlSerializer instanceDeserializer = new XmlSerializer(typeof(Sims2Instance));
            if (File.Exists(xmlfile)){
                using (FileStream fileStream = new(xmlfile, FileMode.Open, System.IO.FileAccess.Read)){
                    using (StreamReader streamReader = new(fileStream)){
                        s2 = (Sims2Instance)instanceDeserializer.Deserialize(streamReader);
                        streamReader.Close();
                    }
                    fileStream.Close();
                }
            }
            return s2;
        }
        public static Sims3Instance LoadS3Instance(string xmlfile){
            Sims3Instance s3 = new();
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Loading instance from {0}.", xmlfile));
            XmlSerializer instanceDeserializer = new XmlSerializer(typeof(Sims3Instance));
            if (File.Exists(xmlfile)){
                using (FileStream fileStream = new(xmlfile, FileMode.Open, System.IO.FileAccess.Read)){
                    using (StreamReader streamReader = new(fileStream)){
                        s3 = (Sims3Instance)instanceDeserializer.Deserialize(streamReader);
                        streamReader.Close();
                    }
                    fileStream.Close();
                }
            }
            return s3;
        }
        public static Sims4Instance LoadS4Instance(string xmlfile){
            Sims4Instance s4 = new();
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Loading instance from {0}.", xmlfile));
            XmlSerializer instanceDeserializer = new XmlSerializer(typeof(Sims4Instance));
            if (File.Exists(xmlfile)){
                using (FileStream fileStream = new(xmlfile, FileMode.Open, System.IO.FileAccess.Read)){
                    using (StreamReader streamReader = new(fileStream)){
                        s4 = (Sims4Instance)instanceDeserializer.Deserialize(streamReader);
                        streamReader.Close();
                    }
                    fileStream.Close();
                }
            }
            return s4;
        }

        public static SimsPackage LoadPackageFile(SimsPackage package){
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Deserializing: {0}.", package.InfoFile));
            XmlSerializer packageDeserializer = new XmlSerializer(typeof(SimsPackage));
            if (File.Exists(package.InfoFile)){
                using (FileStream fileStream = new(package.InfoFile, FileMode.Open, System.IO.FileAccess.ReadWrite)){
                    using (StreamReader streamReader = new(fileStream)){
                        package = (SimsPackage)packageDeserializer.Deserialize(streamReader);
                        streamReader.Close();
                    }
                    fileStream.Close();
                }
            }
            return package;
        }
        public static SimsDownload LoadDownloadFile(SimsDownload download){
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Deserializing: {0}.", download.InfoFile));
            XmlSerializer packageDeserializer = new XmlSerializer(typeof(SimsDownload));
            if (File.Exists(download.InfoFile)){
                using (FileStream fileStream = new(download.InfoFile, FileMode.Open, System.IO.FileAccess.ReadWrite)){
                    using (StreamReader streamReader = new(fileStream)){
                        download = (SimsDownload)packageDeserializer.Deserialize(streamReader);
                        streamReader.Close();
                    }
                    fileStream.Close();
                }
            }
            return download;
        }

        public static ProfileInfo LoadProfile(ProfileInfo profile){            
            XmlSerializer profilereader = new XmlSerializer(typeof(ProfileInfo));
            if (File.Exists(profile.InfoLocation)){
                using (FileStream fileStream = new(profile.InfoLocation, FileMode.Open, System.IO.FileAccess.Read)){
                    using (StreamReader streamReader = new(fileStream)){
                        profile = (ProfileInfo)profilereader.Deserialize(streamReader);
                        streamReader.Close();
                    }
                    fileStream.Close();
                }
            }
            return profile;
        }






        public static bool IsEven(int val){
            if ((val & 0x1) == 0){
                return true;
            } else {
                return false;
            }
        }

        public static string GetPathForExe(string registryKey)
        {
            string InstallPath = "";
            RegistryKey regKey = Registry.LocalMachine.OpenSubKey(registryKey);

            if (regKey != null)
            {
                InstallPath = regKey.GetValue("Install Dir").ToString();
            }
            return InstallPath;
        }

        public static Texture2D ExtractIcon(Executable exe, string datafolder){
            string exelocation = Path.Combine(exe.Path, exe.Exe);
            System.Drawing.Bitmap icon = (System.Drawing.Bitmap)null;
            try
            {
                icon = Icon.ExtractAssociatedIcon(exelocation).ToBitmap();
            }
            catch (System.Exception)
            {
                // swallow and return nothing. You could supply a default Icon here as well
                return new Texture2D();
            }
            string saveloc = ExeIconName(exe, datafolder);
            icon.Save(saveloc, ImageFormat.Png);
            Godot.Image image = Godot.Image.LoadFromFile(saveloc);
            return ImageTexture.CreateFromImage(image);
        }        

        public static string ExeIconName(Executable exe, string datafolder){
            string exeloc = Path.Combine(exe.Path, exe.Exe);
            FileInfo exeinf = new(exeloc);
            string exename = exeinf.Name.Replace(exeinf.Extension, "");
            string iconname = string.Format("{0}.png", exename);
            string exedir = Path.Combine(datafolder, "executables");
            if (!Directory.Exists(exedir)) Directory.CreateDirectory(exedir);
            return Path.Combine(exedir, iconname);
        }

        public static string RunNonSimsProcess(string process, string parameters)
        {
            string result = String.Empty;
            FileInfo exe = new(process);
            string exename = exe.Name;

            if (!File.Exists(process)){
                //Logging.WriteDebugLog("Process was not found.");
            } else {

                //if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Process: {0}", process));
                //if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Params: {0}", parameters));
                string testresult = string.Empty;
                /*Console.WriteLine(parameters);*/

                using (Process p = new Process())
                {
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.FileName = process;
                    p.StartInfo.Arguments = parameters;
                    p.StartInfo.WorkingDirectory = new FileInfo(process).DirectoryName;
                    
                    
                    p.Start();
                    while (p.HasExited == false){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(p.StandardOutput.Read().ToString());
                    }
                    p.WaitForExit();
                    result = p.StandardOutput.ReadToEnd();
                }
            }
            return result;
        }

        public static string RunProcess(string process, string parameters, Games game)
        {
            string result = String.Empty;
            FileInfo exe = new(process);
            string exename = exe.Name;

            if (!File.Exists(process)){
                //Logging.WriteDebugLog("Process was not found.");
            } else {

                //if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Process: {0}", process));
                //if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Params: {0}", parameters));
                string testresult = string.Empty;
                /*Console.WriteLine(parameters);*/

                using (Process p = new Process())
                {
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.FileName = process;
                    p.StartInfo.Arguments = parameters;
                    p.StartInfo.WorkingDirectory = new FileInfo(process).DirectoryName;
                    
                    
                    p.Start();
                    while (p.HasExited == false){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(p.StandardOutput.Read().ToString());
                    }
                    p.WaitForExit();
                    result = p.StandardOutput.ReadToEnd();
                }
            }
            if (game == Games.Sims4){
                while (!CheckForProcess(game)){
                    //
                }                
                while (CheckForProcess(game)){
                    if (GlobalVariables.GameRunning == false) GlobalVariables.GameRunning = true;
                }
            } else { 
                GlobalVariables.GameRunning = true;
            }            

            

            //if (GlobalVariables.DebugMode) Logging.WriteDebugLog("Sims no longer running!");
            return result;
            //return result;
        }

        private static bool CheckForProcess(Games game){
            bool anything = false;
            if (game == Games.Sims2){
                foreach (string exe in GlobalVariables.Sims2Exes){
                    if (Process.GetProcessesByName(exe).Length == 0){
                        anything = false;
                    } else {
                        anything = true;
                    }
                }

            } else if (game == Games.Sims3){
                foreach (string exe in GlobalVariables.Sims3Exes){
                    if (Process.GetProcessesByName(exe).Length == 0){
                        anything = false;
                    } else {
                        anything = true;
                    }
                }

            } else if (game == Games.Sims4){
                foreach (string exe in GlobalVariables.Sims4Exes){
                    if (Process.GetProcessesByName(exe).Length == 0){
                        anything = false;
                    } else {
                        anything = true;
                    }
                }
            }
            
            return anything;            
        }

        private static void StartProcess(string processname){
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog("Process started!");
            Process[] runninggame = Process.GetProcessesByName(processname);
            if (runninggame.Length == 0){
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog("No game... Waiting!");
                StartProcess(processname);
            } else {
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog("Found the game!");
                return;
            }
        }

        private static string WaitProcess(string processname, string result){
            Process[] runninggame = Process.GetProcessesByName(processname);
            if (runninggame.Length != 0){
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog("Game is running!");
                return WaitProcess(processname, result);
            } else {
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog("Looks like the game closed!");
                return result;
            }
        }
    }

    
}