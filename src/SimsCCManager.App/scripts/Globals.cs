using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Godot;
using Microsoft.Win32;
using SimsCCManager.Settings.Loaded;

namespace SimsCCManager.Globals
{
    public class GlobalVariables
    {
        public static string AppName = "Sims CC Manager";
        public static string MyDocuments = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        public static string AppFolder = Path.Combine(MyDocuments, AppName);
        public static bool DebugMode = true;
        public static bool LoggedIn = false;
        public static string InstallDirectory = System.Environment.CurrentDirectory;
        public static string ffmpeg = Path.Combine(InstallDirectory, "tools\\ffmpeg\\bin\\ffmpeg.exe");
        public static string imagemagick = Path.Combine(InstallDirectory, "tools\\imagemagick\\magick.exe");
        //public static string AppDataFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
        //public static string AppldataFolder = Path.Combine(AppDataFolder, "Sims CC Manager");
        public static string SettingsFile = Path.Combine(AppFolder, "Settings.ini");
        public static string tempfolder = Path.Combine(AppFolder, "temp");
        public static string logfolder = Path.Combine(AppFolder, "logs");
        
        
        
        public static void RemoveTempFiles(){
            if (Directory.Exists(tempfolder)){
                Directory.Delete(tempfolder, true);
            }
        }
        
        
        

    }

    public class Utilities {
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

        public static string RunProcess(string process, string parameters)
        {
            string result = String.Empty;

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
                    p.Start();
                    while (p.HasExited == false){
                        //if (GlobalVariables.DebugMode) Logging.WriteDebugLog(p.StandardOutput.Read().ToString());
                    }
                    p.WaitForExit();
                    result = p.StandardOutput.ReadToEnd();
                }
            }
            return result;
            
        }
    }
}