using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Godot;
using SimsCCManager.Containers;
using SimsCCManager.Debugging;
using SimsCCManager.Globals;
using SimsCCManager.Packages.Initial;

namespace SimsCCManager.Packages.Containers
{

    public abstract class SimsFile {
        protected Guid identifier;
        public abstract Guid Identifier {get; set;}        
        protected string filename;
        public abstract string FileName {get; set;}
        protected string infofile;
        public abstract string InfoFile {get; set;}
        protected string location;
        public abstract string Location {get; set;}
        protected double filesize;
        public abstract double FileSize {get; set;}
        protected FileTypes filetype;
        public abstract FileTypes FileType {get; set;}
        protected DateTime dateadded;
        public abstract DateTime DateAdded {get; set;}
        protected DateTime dateupdated;
        public abstract DateTime DateUpdated {get; set;}
        protected bool selected;
        public abstract bool Selected {get; set;}

        public void SetInfoFile(FileInfo file){
            this.InfoFile = file.FullName.Replace(file.Extension, ".info");
        }

        public StringBuilder WriteCoreInfo(){
            StringBuilder sb = new();
            sb.AppendLine(string.Format("{0}={1}", "FileName", GetProperty("FileName")));
            sb.AppendLine(string.Format("{0}={1}", "Location", GetProperty("Location")));
            sb.AppendLine(string.Format("{0}={1}", "Identifier", GetProperty("Identifier")));
            sb.AppendLine(string.Format("{0}={1}", "FileSize", this.FileSize.ToString()));
            sb.AppendLine(string.Format("{0}={1}", "DateAdded", this.DateAdded));
            sb.AppendLine(string.Format("{0}={1}", "DateUpdated", this.DateUpdated));
            return sb;
        }

        public abstract void WriteInfoFile();

        public void RenameFile(string parameter, string rename){
            if (parameter == "FileName"){
                SetProperty(parameter, rename);
                FileInfo fileInfo = new(Location);
                string directory = fileInfo.DirectoryName;
                string extension = fileInfo.Extension;
                string newnamefile = string.Format("{0}{1}", rename, extension);
                string outputfile = Path.Combine(directory, newnamefile);
                string newnameinfofile = string.Format("{0}{1}", rename, ".info");
                string outputinfofile = Path.Combine(directory, newnameinfofile);
                File.Move(Location, outputfile);
                File.Move(InfoFile, outputinfofile);
                Location = outputfile;
                InfoFile = outputinfofile;                
                WriteInfoFile();
            } else {
                SetProperty(parameter, rename);
                WriteInfoFile();
            }
        }

        public void GetInfo(string file){
            FileInfo fileinfo = new(file);
            if (string.IsNullOrEmpty(this.InfoFile)){
                SetInfoFile(fileinfo);
            }
            if (File.Exists(this.InfoFile)){
                using (StreamReader streamReader = new StreamReader(infofile)){
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
                                //if (GetProperty(line[0]) != null){
                                SetProperty(line[0], line[1]);
                                //}
                            }
                        } else {
                            eos = true;
                        }
                    }
                    streamReader.Close();
                }
            } else {
                this.FileName = fileinfo.Name;
				this.FileSize = fileinfo.Length;
				this.Location = fileinfo.FullName;
				this.Identifier = Guid.NewGuid();
				this.FileType = TypeFromExtension(fileinfo.Extension);
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Filetype: {0}", this.FileType));
				this.DateAdded = DateTime.Today;
				this.DateUpdated = DateTime.Today;
				ContinueCreateInfo(fileinfo);
            }
        }

        public abstract void ContinueCreateInfo(FileInfo file);

        public abstract void NamedSetting(string line, StreamReader reader);

        public void ChangeProperty(string property, string value){
            SetProperty(property, value);
            WriteInfoFile();
        }

        public void SetProperty(string propName, dynamic input){
            var prop = this.ProcessProperty(propName);
            PropertyInfo property = this.GetType().GetProperty(propName);
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
                } else if (property.PropertyType == typeof(Games)){
                    if (input.GetType() == typeof(Games)){
                        property.SetValue(this, input);
                    } else if (input.GetType() == typeof(string)) {
                        string data = (string)input;
                        if (data == "Sims 1") {
                            property.SetValue(this, Games.Sims1);
                        } else if (data == "Sims 2") {
                            property.SetValue(this, Games.Sims2);
                        } else if (data == "Sims 3") {
                            property.SetValue(this, Games.Sims3);
                        } else if (data == "Sims 4") {
                            property.SetValue(this, Games.Sims4);
                        } else if (data == "Sims Medieval" || data == "SimsMedieval") {
                            property.SetValue(this, Games.SimsMedieval);
                        } else if (data == "Simcity 5" || data == "SimCity 5" || data == "SimCity5") {
                            property.SetValue(this, Games.SimCity5);
                        } else if (data == "none" || data == "" || data == "null") {
                            property.SetValue(this, Games.Null);
                        } else {
                            property.SetValue(this, Games.Null);
                        }
                    }             
                } else if (propName == "FileSize"){
                    property.SetValue(this, double.Parse(input));
                } else if (property.PropertyType == typeof(DateTime)){
                    property.SetValue(this, DateTime.ParseExact(input, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture));
                }
            }
        }

        public dynamic GetProperty(string propName){
            if (this.ProcessProperty(propName) == null){
                return null;
            }
            var prop = this.ProcessProperty(propName);
            if (prop.GetType() == typeof(string)){
                return prop.ToString();
            } else if (prop.GetType() == typeof(DateTime)){
                DateTime dt = (DateTime)prop;                
                return dt.ToString("MM/dd/yyyy H:mm");
            } else if (prop.GetType() == typeof(bool)){
                return prop;
            } else if (propName == "FileSize"){
                double f = (double)prop;
                return SizeSuffix((long)f, 2);
            } if (prop.GetType() == typeof(Games)){
                Games game = (Games)prop;
                if (game == Games.Sims1){
                    return "Sims 1";
                } else if (game == Games.Sims2){
                    return "Sims 2";
                } else if (game == Games.Sims3){
                    return "Sims 3";
                } else if (game == Games.Sims4){
                    return "Sims 4";
                } else if (game == Games.SimCity5){
                    return "SimCity 5";
                } else if (game == Games.SimsMedieval){
                    return "Sims Medieval";
                } else {
                    return "none";
                }
            } if (prop.GetType() == typeof(Guid)){ 
                return prop.ToString();
            } if (prop.GetType() == typeof(FileTypes)) {
                return TypeToExtension(this.FileType);
            } else {
                return "";
            }
        }

        public object ProcessProperty(string propName){
            try {
                return this.GetType().GetProperty(propName).GetValue (this, null);
            } catch {
                return null;
            }
        }

        public static FileTypes TypeFromExtension(string extension){
            extension = extension.Replace(".", "");
            if (string.Equals(extension, "zip", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Zip;
            } else if (string.Equals(extension, "rar", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Rar;
            } else if (string.Equals(extension, "package", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Package;
            } else if (string.Equals(extension, "ts4script", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.TS4Script;
            } else if (string.Equals(extension, "sims3pack", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Sims3Pack;
            } else if (string.Equals(extension, "sims2pack", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Sims2Pack;
            } else if (string.Equals(extension, "7z", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.SevenZip;
            } else if (string.Equals(extension, "pkg", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.PKG;
            } else if (string.Equals(extension, "jpg", StringComparison.OrdinalIgnoreCase) || string.Equals(extension, "jpeg", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.JPG;
            } else if (string.Equals(extension, "png", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.PNG;
            } else if (string.Equals(extension, "doc", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Doc;
            } else if (string.Equals(extension, "txt", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Txt;
            } else {
                return FileTypes.Other;
            }
        }
        
        public string TypeToExtension(FileTypes type){
            if (type == FileTypes.Zip){
                return "Zip";
            } else if (type == FileTypes.SevenZip){
                return "7z";
            } else if (type == FileTypes.Txt){
                return "Txt";
            } else if (type == FileTypes.PKG){
                return "Pkg";
            } else if (type == FileTypes.TS4Script){
                return "Ts4Script";
            } else if (type == FileTypes.Doc){
                return "Doc";
            } else if (type == FileTypes.JPG){
                return "JPG";
            } else if (type == FileTypes.PNG){
                return "PNG";
            } else if (type == FileTypes.Package){
                return "Package";
            } else if (type == FileTypes.Sims2Pack){
                return "Sims2Pack";
            } else if (type == FileTypes.Sims3Pack){
                return "Sims3Pack";
            } else if (type == FileTypes.Other){
                return "Other";
            } else {
                return "Other";
            }
        }






        static readonly string[] SizeSuffixes = 
                   { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        static string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            //From https://stackoverflow.com/a/14488941 
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeSuffix(-value, decimalPlaces); } 
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}", 
                adjustedSize, 
                SizeSuffixes[mag]);
        }        
    }

    public class SimsPackage : SimsFile {
        public override Guid Identifier{ 
            get { return identifier; } set { identifier = value; }
        }

        public override string FileName {
            get { return filename;} set {filename = value;}
        }
        public override string InfoFile {
            get { return infofile;} set {infofile = value;}
        }
        public override string Location {
            get { return location;} set {location = value;}
        }
        public override double FileSize {
            get { return filesize;} set {filesize = value;}
        }
        public override FileTypes FileType {
            get { return filetype;} set {filetype = value;}
        }
        public override DateTime DateAdded {
            get { return dateadded;} set {dateadded = value;}
        }
        public override DateTime DateUpdated {
            get { return dateupdated;} set {dateupdated = value;}
        }
        public override bool Selected {
            get { return selected;} set {selected = value;}
        }

        public string Creator {get; set;} = "";
        public string Notes {get; set;} = "";
        public Games Game {get; set;} = 0;
        public DateTime DateEnabled {get; set;}
        public bool Broken {get; set;} = false;
        public bool Mesh {get; set;} = false;
        public bool Recolor {get; set;} = false;
        public bool Orphan {get; set;} = false;
        public bool Duplicate {get; set;} = false;
        public bool Override {get; set;} = false;
        public bool RootMod {get; set;} = false;
        public bool ScriptMod {get; set;} = false;
        public bool Merged {get; set;} = false;
        public bool OutOfDate {get; set;} = false;
        public bool Fave {get; set;} = false;
        public bool WrongGame {get; set;} = false;
        public bool Folder {get; set;} = false;
        public Texture2D Thumbnail {get; set;}
        public List<string> Conflicts {get; set;}
        public List<string> DuplicatePackages {get; set;}
        public List<string> OverriddenPackages {get; set;}
        public bool Enabled {get; set;} = false;
        public bool Scanned {get; set;} = false;
        ScanData ScanData {get; set;}

        public SimsPackage(){
            Conflicts = new();
            DuplicatePackages = new();
            OverriddenPackages = new();
        }

        public override void ContinueCreateInfo(FileInfo file){
            this.Enabled = false;
            this.Scanned = false;
            int game = GetGameVersion.CheckGame(file.FullName);
            if (game == 0){
                this.Broken = true;
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Package {0} is broken.", file.Name));
            } else {
                if (game == 1){
                    this.Game = Games.Sims1;
                } else if (game == 2){
                    this.Game = Games.Sims2;
                } else if (game == 3){
                    this.Game = Games.Sims3;
                } else if (game == 4){
                    this.Game = Games.Sims4;
                } else if (game == 11){
                    this.Game = Games.Spore;
                } else if (game == 12){
                    this.Game = Games.SimCity5;
                }   
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Package {0} is {1}.", file.Name, game)); 
            }
            WriteInfoFile();
        }

        public override void WriteInfoFile()
        {
            StringBuilder sb = new();
            sb = WriteCoreInfo();
            sb.AppendLine(string.Format("{0}={1}", "Creator", GetProperty("Creator")));
            sb.AppendLine(string.Format("{0}={1}", "Notes", GetProperty("Notes")));
            sb.AppendLine(string.Format("{0}={1}", "Game", GetProperty("Game")));
            sb.AppendLine(string.Format("{0}={1}", "Scanned", GetProperty("Scanned")));
            sb.AppendLine(string.Format("{0}={1}", "DateEnabled", this.DateEnabled));
            sb.AppendLine(string.Format("{0}={1}", "Broken", GetProperty("Broken")));
            sb.AppendLine(string.Format("{0}={1}", "Mesh", GetProperty("Mesh")));
            sb.AppendLine(string.Format("{0}={1}", "Recolor", GetProperty("Recolor")));
            sb.AppendLine(string.Format("{0}={1}", "Orphan", GetProperty("Orphan")));
            sb.AppendLine(string.Format("{0}={1}", "Duplicate", GetProperty("Duplicate")));
            sb.AppendLine(string.Format("{0}={1}", "Override", GetProperty("Override")));
            sb.AppendLine(string.Format("{0}={1}", "RootMod", GetProperty("RootMod")));
            sb.AppendLine(string.Format("{0}={1}", "ScriptMod", GetProperty("ScriptMod")));
            sb.AppendLine(string.Format("{0}={1}", "Merged", GetProperty("Merged")));
            sb.AppendLine(string.Format("{0}={1}", "OutOfDate", GetProperty("OutOfDate")));
            sb.AppendLine(string.Format("{0}={1}", "Fave", GetProperty("Fave")));
            sb.AppendLine(string.Format("{0}={1}", "WrongGame", GetProperty("WrongGame")));
            sb.AppendLine(string.Format("{0}={1}", "Folder", GetProperty("Folder")));
            sb.AppendLine(string.Format("{0}={1}", "Thumbnail", GetProperty("Thumbnail")));
            sb.AppendLine(string.Format("{0}={1}", "Conflicts", GetProperty("Conflicts")));
            sb.AppendLine(string.Format("{0}={1}", "DuplicatePackages", GetProperty("DuplicatePackages")));
            sb.AppendLine(string.Format("{0}={1}", "OverriddenPackages", GetProperty("OverriddenPackages")));
            sb.AppendLine(string.Format("{0}={1}", "Enabled", GetProperty("Enabled")));
            sb.AppendLine("[SCAN DATA]");
            if (ScanData != null){
                //sb = ScanData.GetStringBuilder(sb);
            }
            using (StreamWriter streamWriter = new(this.InfoFile)){                
                streamWriter.Write(sb);
            }
        }

        public override void NamedSetting(string name, StreamReader reader)
        {
            bool eos = false;
            while (eos == false){
                if(!reader.EndOfStream){
                    if (name == "SCAN DATA"){
                        if (Game == Games.Sims2){
                            Sims2ScanData sims2ScanData = new();
                            //ReadScanData(sims2ScanData, reader);   
                        } else if (Game == Games.Sims3){
                            Sims3ScanData sims3ScanData = new();
                            //ReadScanData(sims3ScanData, reader);   
                        } else if (Game == Games.Sims3){
                            Sims4ScanData sims4ScanData = new();
                            //ReadScanData(sims4ScanData, reader);   
                        }                                         
                    }
                } else {
                    eos = true;
                }
            }
            return;
        }

    }

    public class SimsDownload : SimsFile {
        public override Guid Identifier{ 
            get { return identifier; } set { identifier = value; }
        }
        public override string FileName {
            get { return filename;} set {filename = value;}
        }
        public override string InfoFile {
            get { return infofile;} set {infofile = value;}
        }
        public override string Location {
            get { return location;} set {location = value;}
        }
        public override double FileSize {
            get { return filesize;} set {filesize = value;}
        }
        public override FileTypes FileType {
            get { return filetype;} set {filetype = value;}
        }
        public override DateTime DateAdded {
            get { return dateadded;} set {dateadded = value;}
        }
        public override DateTime DateUpdated {
            get { return dateupdated;} set {dateupdated = value;}
        }
        public override bool Selected {
            get { return selected;} set {selected = value;}
        }

        public bool Installed {get; set;} = false;

        public override void ContinueCreateInfo(FileInfo file){
            this.Installed = false;
            WriteInfoFile();
        }

        public override void NamedSetting(string name, StreamReader reader)
        {
            //should not have a named setting
        }

        public override void WriteInfoFile()
        {
            StringBuilder sb = new();
            sb = WriteCoreInfo();
            sb.AppendLine(string.Format("{0}={1}", "Installed", GetProperty("Installed")));
            
            using (StreamWriter streamWriter = new(this.InfoFile)){                
                streamWriter.Write(sb);
            }
        }
    }

    public class OldSimsPackage {
        
        public string PackageName {get; set;} = "";
        public string Location {get; set;} = "";
        public double FileSize {get; set;} = 0.0;
        public string Creator {get; set;} = "";
        public string Notes {get; set;} = "";
        public Guid PackageReference {get; set;} = Guid.NewGuid();
        public Games Game {get; set;} = 0;
        public FileTypes FileType {get; set;} = FileTypes.Null;
        public ScanData scanData {get; set;}
        public DateTime DateAdded {get; set;} = DateTime.Today;
        public DateTime DateUpdated {get; set;} = DateTime.Today;
        public DateTime DateEnabled {get; set;} = DateTime.Today;
        public bool Broken {get; set;} = false;
        public bool Mesh {get; set;} = false;
        public bool Recolor {get; set;} = false;
        public bool Orphan {get; set;} = false;
        public bool Duplicate {get; set;} = false;
        public bool Override {get; set;} = false;
        public bool RootMod {get; set;} = false;
        public bool ScriptMod {get; set;} = false;
        public bool Merged {get; set;} = false;
        public bool OutOfDate {get; set;} = false;
        public bool Fave {get; set;} = false;
        public bool WrongGame {get; set;} = false;
        public bool Folder {get; set;} = false;
        public Texture2D Thumbnail {get; set;}
        public List<string> Conflicts {get; set;}
        public List<string> DuplicatePackages {get; set;}
        public List<string> OverriddenPackages {get; set;}
        public bool Selected {get; set;} = false;
        public bool Enabled {get; set;} = false;
        public bool Scanned {get; set;} = false;
        Sims2ScanData Sims2ScanData {get; set;} = null;
        Sims3ScanData Sims3ScanData {get; set;} = null;
        Sims4ScanData Sims4ScanData {get; set;} = null;

        public OldSimsPackage(){
            Conflicts = new();
            DuplicatePackages = new();            
        }

        public void WriteInfo(){
            FileInfo fileinfo = new(Location);            
            string file = fileinfo.FullName.Replace(fileinfo.Extension, ".info");
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Writing package info to {0}.", file));
            StringBuilder sb = new();
            sb.AppendLine(string.Format("{0}={1}", "PackageName", GetProperty("PackageName")));
            sb.AppendLine(string.Format("{0}={1}", "Location", GetProperty("Location")));
            sb.AppendLine(string.Format("{0}={1}", "FileSize", this.FileSize));
            sb.AppendLine(string.Format("{0}={1}", "Creator", GetProperty("Creator")));
            sb.AppendLine(string.Format("{0}={1}", "Notes", GetProperty("Notes")));
            sb.AppendLine(string.Format("{0}={1}", "PackageReference", GetProperty("PackageReference")));
            sb.AppendLine(string.Format("{0}={1}", "Game", GetProperty("Game")));
            sb.AppendLine(string.Format("{0}={1}", "Scanned", GetProperty("Scanned")));
            sb.AppendLine(string.Format("{0}={1}", "FileType", TypeToExtension(this.FileType)));
            sb.AppendLine(string.Format("{0}={1}", "DateAdded", GetProperty("DateAdded")));
            sb.AppendLine(string.Format("{0}={1}", "DateUpdated", GetProperty("DateUpdated")));
            sb.AppendLine(string.Format("{0}={1}", "DateEnabled", GetProperty("DateEnabled")));
            sb.AppendLine(string.Format("{0}={1}", "Broken", GetProperty("Broken")));
            sb.AppendLine(string.Format("{0}={1}", "Mesh", GetProperty("Mesh")));
            sb.AppendLine(string.Format("{0}={1}", "Recolor", GetProperty("Recolor")));
            sb.AppendLine(string.Format("{0}={1}", "Orphan", GetProperty("Orphan")));
            sb.AppendLine(string.Format("{0}={1}", "Duplicate", GetProperty("Duplicate")));
            sb.AppendLine(string.Format("{0}={1}", "Override", GetProperty("Override")));
            sb.AppendLine(string.Format("{0}={1}", "RootMod", GetProperty("RootMod")));
            sb.AppendLine(string.Format("{0}={1}", "ScriptMod", GetProperty("ScriptMod")));
            sb.AppendLine(string.Format("{0}={1}", "Merged", GetProperty("Merged")));
            sb.AppendLine(string.Format("{0}={1}", "OutOfDate", GetProperty("OutOfDate")));
            sb.AppendLine(string.Format("{0}={1}", "Fave", GetProperty("Fave")));
            sb.AppendLine(string.Format("{0}={1}", "WrongGame", GetProperty("WrongGame")));
            sb.AppendLine(string.Format("{0}={1}", "Folder", GetProperty("Folder")));
            sb.AppendLine(string.Format("{0}={1}", "Thumbnail", GetProperty("Thumbnail")));
            sb.AppendLine(string.Format("{0}={1}", "Conflicts", GetProperty("Conflicts")));
            sb.AppendLine(string.Format("{0}={1}", "DuplicatePackages", GetProperty("DuplicatePackages")));
            sb.AppendLine(string.Format("{0}={1}", "OverriddenPackages", GetProperty("OverriddenPackages")));
            sb.AppendLine(string.Format("{0}={1}", "Enabled", GetProperty("Enabled")));
            sb.AppendLine("[SCAN DATA]");
            if (Sims2ScanData != null){
                sb = Sims2ScanData.GetStringBuilder(sb);
            } else if (Sims3ScanData != null){                
                sb = Sims3ScanData.GetStringBuilder(sb);
            } else if (Sims4ScanData != null){                
                sb = Sims4ScanData.GetStringBuilder(sb);
            }     
            using (StreamWriter streamWriter = new(file)){                
                streamWriter.Write(sb);
            }
        }

        public void ChangeProperty(string property, string value){
            SetProperty(property, value);
            WriteInfo();
        }
        

        public void GetInfo(string file){            
            FileInfo fileinfo = new(file);
			string infofile = fileinfo.FullName.Replace(fileinfo.Extension, ".info");
			if (File.Exists(infofile)){
                using (StreamReader streamReader = new StreamReader(infofile)){
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
                                //if (GetProperty(line[0]) != null){
                                SetProperty(line[0], line[1]);
                                //}
                            }
                        } else {
                            eos = true;
                        }
                    }
                    streamReader.Close();
                }
            } else {
                this.PackageName = fileinfo.Name;
				this.FileSize = fileinfo.Length;
				this.Location = fileinfo.FullName;
				this.PackageReference = Guid.NewGuid();
				this.FileType = TypeFromExtension(fileinfo.Extension);
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Filetype: {0}", this.FileType));
				this.DateAdded = DateTime.Today;
				this.DateUpdated = DateTime.Today;
				this.Enabled = false;
                this.Scanned = false;
                int game = GetGameVersion.CheckGame(fileinfo.FullName);
                if (game == 0){
                    this.Broken = true;
                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Package {0} is broken.", fileinfo.Name));
                } else {
                    if (game == 1){
                        this.Game = Games.Sims1;
                    } else if (game == 2){
                        this.Game = Games.Sims2;
                    } else if (game == 3){
                        this.Game = Games.Sims3;
                    } else if (game == 4){
                        this.Game = Games.Sims4;
                    } else if (game == 11){
                        this.Game = Games.Spore;
                    } else if (game == 12){
                        this.Game = Games.SimCity5;
                    }   
                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Package {0} is {1}.", fileinfo.Name, game)); 
                }
                WriteInfo();
            }
        }

        public void NamedSetting(string name, StreamReader reader) {
            bool eos = false;
            while (eos == false){
                if(!reader.EndOfStream){
                    if (name == "SCAN DATA"){
                        if (Game == Games.Sims2){
                            Sims2ScanData sims2ScanData = new();
                            ReadScanData(sims2ScanData, reader);   
                        } else if (Game == Games.Sims3){
                            Sims3ScanData sims3ScanData = new();
                            ReadScanData(sims3ScanData, reader);   
                        } else if (Game == Games.Sims3){
                            Sims4ScanData sims4ScanData = new();
                            ReadScanData(sims4ScanData, reader);   
                        }                                         
                    }
                } else {
                    eos = true;
                }
            }
            return;
        }
        
        public void ReadScanData(Sims2ScanData data, StreamReader reader){
            bool eos = false;                
            while (eos == false){
                if(!reader.EndOfStream){
                    string setting = reader.ReadLine();
                    string[] line = setting.Split("=");
                    if (data.GetProperty(line[0]) != null){
                        data.SetProperty(line[0], line[1]);
                    }
                } else {
                    eos = true;
                }
            }      
            scanData = data;      
            reader.Close();
        }

        public void ReadScanData(Sims3ScanData data, StreamReader reader){
            bool eos = false;                
            while (eos == false){
                if(!reader.EndOfStream){
                    string setting = reader.ReadLine();
                    string[] line = setting.Split("=");
                    if (data.GetProperty(line[0]) != null){
                        data.SetProperty(line[0], line[1]);
                    }
                } else {
                    eos = true;
                }
            }      
            scanData = data;      
            reader.Close();
        }

        public void ReadScanData(Sims4ScanData data, StreamReader reader){
            bool eos = false;                
            while (eos == false){
                if(!reader.EndOfStream){
                    string setting = reader.ReadLine();
                    string[] line = setting.Split("=");
                    if (data.GetProperty(line[0]) != null){
                        data.SetProperty(line[0], line[1]);
                    }
                } else {
                    eos = true;
                }
            }      
            scanData = data;      
            reader.Close();
        }

        public FileTypes TypeFromExtension(string extension){
            if (string.Equals(extension, ".zip", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Zip;
            } else if (string.Equals(extension, ".rar", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Rar;
            } else if (string.Equals(extension, ".package", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Package;
            } else if (string.Equals(extension, ".ts4script", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.TS4Script;
            } else if (string.Equals(extension, ".sims3pack", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Sims3Pack;
            } else if (string.Equals(extension, ".sims2pack", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Sims2Pack;
            } else if (string.Equals(extension, ".7z", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.SevenZip;
            } else if (string.Equals(extension, ".pkg", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.PKG;
            } else if (string.Equals(extension, ".jpg", StringComparison.OrdinalIgnoreCase) || string.Equals(extension, ".jpeg", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.JPG;
            } else if (string.Equals(extension, ".png", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.PNG;
            } else if (string.Equals(extension, ".doc", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Doc;
            } else if (string.Equals(extension, ".txt", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Txt;
            } else {
                return FileTypes.Other;
            }
        }

        public FileTypes TypeFromString(string extension){
            if (string.Equals(extension, "zip", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Zip;
            } else if (string.Equals(extension, "rar", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Rar;
            } else if (string.Equals(extension, "package", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Package;
            } else if (string.Equals(extension, "ts4script", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.TS4Script;
            } else if (string.Equals(extension, "sims3pack", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Sims3Pack;
            } else if (string.Equals(extension, "sims2pack", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Sims2Pack;
            } else if (string.Equals(extension, "7z", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.SevenZip;
            } else if (string.Equals(extension, "pkg", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.PKG;
            } else if (string.Equals(extension, "jpg", StringComparison.OrdinalIgnoreCase) || string.Equals(extension, ".jpeg", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.JPG;
            } else if (string.Equals(extension, "png", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.PNG;
            } else if (string.Equals(extension, "doc", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Doc;
            } else if (string.Equals(extension, "txt", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Txt;
            } else {
                return FileTypes.Other;
            }
        }

        
        public string TypeToExtension(FileTypes type){
            if (type == FileTypes.Zip){
                return "Zip";
            } else if (type == FileTypes.SevenZip){
                return "7z";
            } else if (type == FileTypes.Txt){
                return "Txt";
            } else if (type == FileTypes.PKG){
                return "Pkg";
            } else if (type == FileTypes.TS4Script){
                return "Ts4Script";
            } else if (type == FileTypes.Doc){
                return "Doc";
            } else if (type == FileTypes.JPG){
                return "JPG";
            } else if (type == FileTypes.PNG){
                return "PNG";
            } else if (type == FileTypes.Package){
                return "Package";
            } else if (type == FileTypes.Sims2Pack){
                return "Sims2Pack";
            } else if (type == FileTypes.Sims3Pack){
                return "Sims3Pack";
            } else if (type == FileTypes.Other){
                return "Other";
            } else {
                return "Other";
            }
        }
        

        public void SetProperty(string propName, dynamic input){
            var prop = this.ProcessProperty(propName);
            PropertyInfo property = this.GetType().GetProperty(propName);
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
                } else if (property.PropertyType == typeof(Games)){
                    if (input.GetType() == typeof(Games)){
                        property.SetValue(this, input);
                    } else if (input.GetType() == typeof(string)) {
                        string data = (string)input;
                        if (data == "Sims 1") {
                            property.SetValue(this, Games.Sims1);
                        } else if (data == "Sims 2") {
                            property.SetValue(this, Games.Sims2);
                        } else if (data == "Sims 3") {
                            property.SetValue(this, Games.Sims3);
                        } else if (data == "Sims 4") {
                            property.SetValue(this, Games.Sims4);
                        } else if (data == "Sims Medieval" || data == "SimsMedieval") {
                            property.SetValue(this, Games.SimsMedieval);
                        } else if (data == "Simcity 5" || data == "SimCity 5" || data == "SimCity5") {
                            property.SetValue(this, Games.SimCity5);
                        } else if (data == "none" || data == "" || data == "null") {
                            property.SetValue(this, Games.Null);
                        } else {
                            property.SetValue(this, Games.Null);
                        }
                    }             
                } else if (propName == "FileSize"){
                    property.SetValue(this, FileSizeFromString(input));
                }
            }            
        }

        public dynamic GetProperty(string propName){
            if (this.ProcessProperty(propName) == null){
                return null;
            }
            var prop = this.ProcessProperty(propName);
            if (prop.GetType() == typeof(string)){
                return prop.ToString();
            } else if (prop.GetType() == typeof(DateTime)){
                DateTime dt = (DateTime)prop;                
                return dt.ToString("MM/dd/yyyy H:mm");
            } else if (prop.GetType() == typeof(bool)){
                return prop;
            } else if (propName == "FileSize"){
                double f = (double)prop;
                return SizeSuffix((long)f, 2);
            } if (prop.GetType() == typeof(Games)){
                Games game = (Games)prop;
                if (game == Games.Sims1){
                    return "Sims 1";
                } else if (game == Games.Sims2){
                    return "Sims 2";
                } else if (game == Games.Sims3){
                    return "Sims 3";
                } else if (game == Games.Sims4){
                    return "Sims 4";
                } else if (game == Games.SimCity5){
                    return "SimCity 5";
                } else if (game == Games.SimsMedieval){
                    return "Sims Medieval";
                } else {
                    return "none";
                }
            } if (prop.GetType() == typeof(Guid)){ 
                return prop.ToString();
            } if (prop.GetType() == typeof(FileTypes)) {
                return TypeToExtension(this.FileType);
            } else {
                return "";
            }
        }

        public object ProcessProperty(string propName){
            try {
                return this.GetType().GetProperty(propName).GetValue (this, null);
            } catch {
                return null;
            }       
        }

        
        static double FileSizeFromString(string filesize){
            string numberOnly = Regex.Replace(filesize, "[^0-9.]", "");
            string suffixOnly = Regex.Replace(filesize, "[^A-Z]", "");
            if (suffixOnly == "B"){
                
            }
            return double.Parse(numberOnly);
        }

        static readonly string[] SizeSuffixes = 
                   { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        static string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            //From https://stackoverflow.com/a/14488941 
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeSuffix(-value, decimalPlaces); } 
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}", 
                adjustedSize, 
                SizeSuffixes[mag]);
        }
    }


    public class OldSimsDownload{
        public string FileName {get; set;} = "";
        public string Location {get; set;} = "";
        public double FileSize {get; set;} = 0.0;
        public Guid FileReference {get; set;} = Guid.NewGuid();
        public FileTypes FileType {get; set;} = FileTypes.Null; 
        public DateTime DateAdded {get; set;} = DateTime.Today;
        public DateTime DateUpdated {get; set;} = DateTime.Today;
        public bool Installed {get; set;} = false;
        public bool Selected {get; set;} = false;


        public void GetInfo(string file){            
            FileInfo fileinfo = new(file);
			string infofile = fileinfo.FullName.Replace(fileinfo.Extension, ".info");
			if (File.Exists(infofile)){
                using (StreamReader streamReader = new StreamReader(infofile)){
                    bool eos = false;                
                    while (eos == false){
                        if(!streamReader.EndOfStream){
                            string setting = streamReader.ReadLine();
                            if (setting.Contains('[')){
                                string line = setting.Replace("[", "");
                                line = line.Replace("]", "");
                                //NamedSetting(this, line, streamReader);                   
                            } else {
                                string[] line = setting.Split("=");
                                if (GetProperty(line[0]) != null){
                                    SetProperty(line[0], line[1]);
                                }
                            }
                        } else {
                            eos = true;
                        }
                    }
                    streamReader.Close();
                }
            } else {
                this.FileName = fileinfo.Name;
				this.FileSize = fileinfo.Length;
				this.Location = fileinfo.FullName;
				this.FileReference = Guid.NewGuid();
				this.FileType = TypeFromExtension(fileinfo.Extension);
				this.DateAdded = DateTime.Today;
				this.DateUpdated = DateTime.Today;
				this.Installed = false;
                WriteInfo();
            }
        }

        public void WriteInfo(){
            FileInfo fileinfo = new(Location);            
            string file = fileinfo.FullName.Replace(fileinfo.Extension, ".info");
            StringBuilder sb = new();
            sb.AppendLine(string.Format("{0}={1}", "FileName", GetProperty("FileName")));
            sb.AppendLine(string.Format("{0}={1}", "Location", GetProperty("Location")));
            sb.AppendLine(string.Format("{0}={1}", "FileSize", GetProperty("FileSize")));
            sb.AppendLine(string.Format("{0}={1}", "FileReference", GetProperty("FileReference")));
            sb.AppendLine(string.Format("{0}={1}", "FileType", TypeToExtension(this.FileType)));
            sb.AppendLine(string.Format("{0}={1}", "DateAdded", GetProperty("DateAdded")));
            sb.AppendLine(string.Format("{0}={1}", "DateUpdated", GetProperty("DateUpdated")));
            sb.AppendLine(string.Format("{0}={1}", "Installed", GetProperty("Installed")));
            
            using (StreamWriter streamWriter = new(file)){                
                streamWriter.Write(sb);
            }
        }

        public FileTypes TypeFromExtension(string extension){
            if (string.Equals(extension, ".zip", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Zip;
            } else if (string.Equals(extension, ".rar", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Rar;
            } else if (string.Equals(extension, ".package", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Package;
            } else if (string.Equals(extension, ".ts4script", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.TS4Script;
            } else if (string.Equals(extension, ".sims3pack", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Sims3Pack;
            } else if (string.Equals(extension, ".sims2pack", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Sims2Pack;
            } else if (string.Equals(extension, ".7z", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.SevenZip;
            } else if (string.Equals(extension, ".pkg", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.PKG;
            } else if (string.Equals(extension, ".jpg", StringComparison.OrdinalIgnoreCase) || string.Equals(extension, ".jpeg", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.JPG;
            } else if (string.Equals(extension, ".png", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.PNG;
            } else if (string.Equals(extension, ".doc", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Doc;
            } else if (string.Equals(extension, ".txt", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Txt;
            } else {
                return FileTypes.Other;
            }
        }
        public FileTypes TypeFromString(string extension){
            if (string.Equals(extension, "zip", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Zip;
            } else if (string.Equals(extension, "rar", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Rar;
            } else if (string.Equals(extension, "package", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Package;
            } else if (string.Equals(extension, "ts4script", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.TS4Script;
            } else if (string.Equals(extension, "sims3pack", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Sims3Pack;
            } else if (string.Equals(extension, "sims2pack", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Sims2Pack;
            } else if (string.Equals(extension, "7z", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.SevenZip;
            } else if (string.Equals(extension, "pkg", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.PKG;
            } else if (string.Equals(extension, "jpg", StringComparison.OrdinalIgnoreCase) || string.Equals(extension, ".jpeg", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.JPG;
            } else if (string.Equals(extension, "png", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.PNG;
            } else if (string.Equals(extension, "doc", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Doc;
            } else if (string.Equals(extension, "txt", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Txt;
            } else {
                return FileTypes.Other;
            }
        }

        
        public string TypeToExtension(FileTypes type){
            if (type == FileTypes.Zip){
                return "Zip";
            } else if (type == FileTypes.SevenZip){
                return "7z";
            } else if (type == FileTypes.Txt){
                return "Txt";
            } else if (type == FileTypes.PKG){
                return "Pkg";
            } else if (type == FileTypes.TS4Script){
                return "Ts4Script";
            } else if (type == FileTypes.Doc){
                return "Doc";
            } else if (type == FileTypes.JPG){
                return "JPG";
            } else if (type == FileTypes.PNG){
                return "PNG";
            } else if (type == FileTypes.Package){
                return "Package";
            } else if (type == FileTypes.Sims2Pack){
                return "Sims2Pack";
            } else if (type == FileTypes.Sims3Pack){
                return "Sims3Pack";
            } else if (type == FileTypes.Other){
                return "Other";
            } else {
                return "Other";
            }
        }

        public void NamedSetting(SimsDownload simsDownload, string name, StreamReader reader) {
            bool eos = false;
            while (eos == false){
                if(!reader.EndOfStream){
                    if (name == "SCAN DATA"){
                        bool instances = true;                
                        while (instances == true){
                            string setting = reader.ReadLine();
                            if (setting.Contains('[')){
                                instances = false;
                                string line = setting.Replace("[", "");
                                line = line.Replace("]", "");
                                NamedSetting(simsDownload, line, reader);
                            } else {
                                return;
                            }
                        }
                    } else {
                        return;
                    }
                } else {
                    eos = true;
                }
            }
            return;
        }

        
        public void ChangeProperty(string property, string value){
            SetProperty(property, value);
            WriteInfo();
        }

        public dynamic GetProperty(string propName){
            if (this.ProcessProperty(propName) == null){
                return null;
            }
            var prop = this.ProcessProperty(propName);
            if (prop.GetType() == typeof(string)){
                return prop.ToString();
            } else if (prop.GetType() == typeof(DateTime)){
                DateTime dt = (DateTime)prop;                
                return dt.ToString("MM/dd/yyyy H:mm");
            } else if (prop.GetType() == typeof(bool)){
                return prop;
            } else if (propName == "FileSize"){
                double f = (double)prop;
                return SizeSuffix((long)f, 2);
            } else if (prop.GetType() == typeof(FileTypes)){
                return TypeToExtension((FileTypes)prop);
            } else {
                return "";
            }
        }

        public void SetProperty(string propName, dynamic input){
            var prop = this.ProcessProperty(propName);
            PropertyInfo property = this.GetType().GetProperty(propName);
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
                } else if (property.PropertyType == typeof(FileTypes)){
                    property.SetValue(this, TypeFromString(input as string));
                }
            }            
        }

        public object ProcessProperty(string propName){
            try {
                return this.GetType().GetProperty(propName).GetValue (this, null);
            } catch {
                return null;
            }       
        }

        static readonly string[] SizeSuffixes = 
                   { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        static string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            //From https://stackoverflow.com/a/14488941 
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeSuffix(-value, decimalPlaces); } 
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}", 
                adjustedSize, 
                SizeSuffixes[mag]);
        }

    }

    public abstract class ScanData{
        protected string description;
        public abstract string Description {get; set;}

        protected string type;
        public abstract string Type {get; set;}

        protected string subtype;
        public abstract string Subtype {get; set;}

        protected bool scanned;
        public abstract bool Scanned {get; set;}

        protected bool scriptmod;
        public abstract bool ScriptMod {get; set;}

        protected bool category;
        public abstract bool Category {get; set;}

        protected string tuningid;
        public abstract string TuningID {get; set;}

        protected bool allowrandom;
        public abstract bool AllowRandom {get; set;}

        protected List<string> conflicts;
        public abstract List<string> Conflicts {get; set;}

        protected List<string> duplicates;
        public abstract List<string> Duplicates {get; set;}

        protected List<string> parts;
        public abstract List<string> Parts {get; set;}

        public dynamic GetProperty(string propName){
            if (this.ProcessProperty(propName) == null){
                return null;
            }
            var prop = this.ProcessProperty(propName);
            if (prop.GetType() == typeof(string)){
                return prop.ToString();
            } else if (prop.GetType() == typeof(DateTime)){
                DateTime dt = (DateTime)prop;                
                return dt.ToString("MM/dd/yyyy H:mm");
            } else if (prop.GetType() == typeof(bool)){
                return prop;
            } else if (propName == "FileSize"){
                double f = (double)prop;
                return SizeSuffix((long)f, 2);
            } else {
                return "";
            }
        }

        public void SetProperty(string propName, dynamic input){
            var prop = this.ProcessProperty(propName);
            PropertyInfo property = this.GetType().GetProperty(propName);
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
                }
            }            
        }

        public object ProcessProperty(string propName){
            try {
                return this.GetType().GetProperty(propName).GetValue (this, null);
            } catch {
                return null;
            }       
        }

        static readonly string[] SizeSuffixes = 
                   { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        static string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            //From https://stackoverflow.com/a/14488941 
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeSuffix(-value, decimalPlaces); } 
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}", 
                adjustedSize, 
                SizeSuffixes[mag]);
        }
    }

    public class Sims2ScanData : ScanData{
        public override string Description{
            get { return description; } set {description = value; } 
        }
        public override string Type{
            get { return type; } set {type = value; } 
        }
        public override string Subtype{
            get { return subtype; } set {subtype = value; } 
        }
        public override bool Scanned{
            get { return scanned; } set {scanned = value; } 
        }
        public override bool ScriptMod{
            get { return scriptmod; } set {scriptmod = value; } 
        }
        public override bool Category{
            get { return category; } set {category = value; } 
        }
        public override string TuningID{
            get { return tuningid; } set {tuningid = value; } 
        }
        public override bool AllowRandom{
            get { return allowrandom; } set {allowrandom = value; } 
        }
        public override List<string> Conflicts{
            get { return conflicts; } set {conflicts = value; } 
        }
        public override List<string> Duplicates{
            get { return duplicates; } set {duplicates = value; } 
        }
        public override List<string> Parts{
            get { return parts; } set {parts = value; } 
        }


        /*public string ModelName {get; set;}
        public string Age {get; set;}
        public string Gender {get; set;}
        public string Function {get; set;}
        public string FunctionSubcategory {get; set;}
        public List<PackageInstance> InstanceIDs {get; set;}
        public List<PackageThumbnail> ThumbnailImage {get; set;}
        public List<PackageGUID> GUIDs {get; set;}
        public List<PackageRequiredEPs> RequiredEPs {get; set;}
        public List<TagsList> CatalogTags {get; set;}
        public AgeGenderFlags AgeGenderFlags {get; set;}
        public List<PackageEntries> FileHas {get; set;}
        public List<PackageRoomSort> RoomSort {get; set;}
        public List<PackageComponent> Components {get; set;}
        public List<PackageTypeCounter> Entries         {get; set;}
        public List<PackageFlag> Flags {get; set;}
        public List<PackageMeshKeys> MeshKeys {get; set;}
        public List<PackageCASPartKeys> CASPartKeys {get; set;}
        public List<PackageOBJDKeys> OBJDPartKeys {get; set;}
        public List<PackageMatchingRecolors> MatchingRecolors {get; set;}*/


        public StringBuilder GetStringBuilder(StringBuilder sb){
            sb = SBOverrides(sb);
            
            return sb;
        }

        public StringBuilder SBOverrides(StringBuilder sb){
            sb.AppendLine(string.Format("{0}={1}", "Description", GetProperty("Description")));
            sb.AppendLine(string.Format("{0}={1}", "Type", GetProperty("Type")));
            sb.AppendLine(string.Format("{0}={1}", "Subtype", GetProperty("Subtype")));
            sb.AppendLine(string.Format("{0}={1}", "Scanned", GetProperty("Scanned")));
            sb.AppendLine(string.Format("{0}={1}", "ScriptMod", GetProperty("ScriptMod")));
            sb.AppendLine(string.Format("{0}={1}", "Category", GetProperty("Category")));
            sb.AppendLine(string.Format("{0}={1}", "TuningID", GetProperty("TuningID")));
            sb.AppendLine(string.Format("{0}={1}", "AllowRandom", GetProperty("AllowRandom")));
            sb.AppendLine(string.Format("{0}={1}", "Conflicts", GetProperty("Conflicts")));
            sb.AppendLine(string.Format("{0}={1}", "Duplicates", GetProperty("Duplicates")));
            return sb;
        }
        
    }
    public class Sims3ScanData : ScanData{
        public override string Description{
            get { return description; } set {description = value; } 
        }
        public override string Type{
            get { return type; } set {type = value; } 
        }
        public override string Subtype{
            get { return subtype; } set {subtype = value; } 
        }
        public override bool Scanned{
            get { return scanned; } set {scanned = value; } 
        }
        public override bool ScriptMod{
            get { return scriptmod; } set {scriptmod = value; } 
        }
        public override bool Category{
            get { return category; } set {category = value; } 
        }
        public override string TuningID{
            get { return tuningid; } set {tuningid = value; } 
        }
        public override bool AllowRandom{
            get { return allowrandom; } set {allowrandom = value; } 
        }
        public override List<string> Conflicts{
            get { return conflicts; } set {conflicts = value; } 
        }
        public override List<string> Duplicates{
            get { return duplicates; } set {duplicates = value; } 
        }
        public override List<string> Parts{
            get { return parts; } set {parts = value; } 
        }




        public StringBuilder GetStringBuilder(StringBuilder sb){
            sb = SBOverrides(sb);
            
            return sb;
        }

        public StringBuilder SBOverrides(StringBuilder sb){
            sb.AppendLine(string.Format("{0}={1}", "Description", GetProperty("Description")));
            sb.AppendLine(string.Format("{0}={1}", "Type", GetProperty("Type")));
            sb.AppendLine(string.Format("{0}={1}", "Subtype", GetProperty("Subtype")));
            sb.AppendLine(string.Format("{0}={1}", "Scanned", GetProperty("Scanned")));
            sb.AppendLine(string.Format("{0}={1}", "ScriptMod", GetProperty("ScriptMod")));
            sb.AppendLine(string.Format("{0}={1}", "Category", GetProperty("Category")));
            sb.AppendLine(string.Format("{0}={1}", "TuningID", GetProperty("TuningID")));
            sb.AppendLine(string.Format("{0}={1}", "AllowRandom", GetProperty("AllowRandom")));
            sb.AppendLine(string.Format("{0}={1}", "Conflicts", GetProperty("Conflicts")));
            sb.AppendLine(string.Format("{0}={1}", "Duplicates", GetProperty("Duplicates")));
            return sb;
        }
    }
    public class Sims4ScanData : ScanData{
        public override string Description{
            get { return description; } set {description = value; } 
        }
        public override string Type{
            get { return type; } set {type = value; } 
        }
        public override string Subtype{
            get { return subtype; } set {subtype = value; } 
        }
        public override bool Scanned{
            get { return scanned; } set {scanned = value; } 
        }
        public override bool ScriptMod{
            get { return scriptmod; } set {scriptmod = value; } 
        }
        public override bool Category{
            get { return category; } set {category = value; } 
        }
        public override string TuningID{
            get { return tuningid; } set {tuningid = value; } 
        }
        public override bool AllowRandom{
            get { return allowrandom; } set {allowrandom = value; } 
        }
        public override List<string> Conflicts{
            get { return conflicts; } set {conflicts = value; } 
        }
        public override List<string> Duplicates{
            get { return duplicates; } set {duplicates = value; } 
        }
        public override List<string> Parts{
            get { return parts; } set {parts = value; } 
        }


        public StringBuilder GetStringBuilder(StringBuilder sb){
            sb = SBOverrides(sb);
            
            return sb;
        }

        public StringBuilder SBOverrides(StringBuilder sb){
            sb.AppendLine(string.Format("{0}={1}", "Description", GetProperty("Description")));
            sb.AppendLine(string.Format("{0}={1}", "Type", GetProperty("Type")));
            sb.AppendLine(string.Format("{0}={1}", "Subtype", GetProperty("Subtype")));
            sb.AppendLine(string.Format("{0}={1}", "Scanned", GetProperty("Scanned")));
            sb.AppendLine(string.Format("{0}={1}", "ScriptMod", GetProperty("ScriptMod")));
            sb.AppendLine(string.Format("{0}={1}", "Category", GetProperty("Category")));
            sb.AppendLine(string.Format("{0}={1}", "TuningID", GetProperty("TuningID")));
            sb.AppendLine(string.Format("{0}={1}", "AllowRandom", GetProperty("AllowRandom")));
            sb.AppendLine(string.Format("{0}={1}", "Conflicts", GetProperty("Conflicts")));
            sb.AppendLine(string.Format("{0}={1}", "Duplicates", GetProperty("Duplicates")));
            return sb;
        }
    }
}

public enum Games {
    Sims1,
    Sims2,
    Sims3,
    Sims4,
    SimsMedieval,
    SimCity5,
    Spore,
    Null
}

public enum FileTypes {
    Package,
    TS4Script,
    Sims3Pack,
    Sims2Pack,
    Zip,
    SevenZip,
    Rar,
    PKG,
    JPG,
    PNG,
    Doc,
    Txt,
    Other,
    Null
}