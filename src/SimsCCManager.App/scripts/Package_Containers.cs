using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Godot;
using SimsCCManager.Containers;
using SimsCCManager.Debugging;
using SimsCCManager.Globals;
using SimsCCManager.Packages.Initial;
using SimsCCManager.Settings.Loaded;

namespace SimsCCManager.Packages.Containers
{
    [XmlInclude(typeof(SimsPackage))]
    [XmlInclude(typeof(SimsDownload))]
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
        [XmlIgnore]
        protected bool selected;
        [XmlIgnore]
        public abstract bool Selected {get; set;}

        public void SetInfoFile(FileInfo file){
            this.InfoFile = string.Format("{0}.info", file.FullName);
        }
        public void SetInfoFile(DirectoryInfo file){
            this.InfoFile = string.Format("{0}.info", file.FullName);
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
                string newnameinfofile = string.Format("{0}{1}", outputfile, ".info");
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

        public static long DirSize(DirectoryInfo d) 
        {    
            long size = 0;    
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis) 
            {      
                size += fi.Length;    
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis) 
            {
                size += DirSize(di);   
            }
            return size;  
        }

        public bool GetInfo(string file, bool folder = false){
            if (folder){
                DirectoryInfo directoryInfo = new(file);
                if (string.IsNullOrEmpty(this.InfoFile)){
                    SetInfoFile(directoryInfo);
                }
                if (File.Exists(this.InfoFile)){
                    return true;
                } else {                    
                    return false;
                }
            }
            FileInfo fileinfo = new(file);
            if (string.IsNullOrEmpty(this.InfoFile)){
                SetInfoFile(fileinfo);
            }
            if (File.Exists(this.InfoFile)){                
                return true;                
            } else {                
                return false;
            }
        }

        public void MakeInfo(string file, bool folder = false){
            if (folder){
                DirectoryInfo directoryInfo = new(file);
                if (string.IsNullOrEmpty(this.InfoFile)){
                    SetInfoFile(directoryInfo);
                }                
                this.FileName = directoryInfo.Name;
                this.FileSize = DirSize(directoryInfo);
                this.Location = directoryInfo.FullName;
                this.Identifier = Guid.NewGuid();
                this.FileType = TypeFromExtension("folder");
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Filetype: {0}", this.FileType));
                this.DateAdded = DateTime.Today;
                this.DateUpdated = DateTime.Today;
            } else {
                FileInfo fileinfo = new(file);
                if (string.IsNullOrEmpty(this.InfoFile)){
                    SetInfoFile(fileinfo);
                }
                this.FileName = fileinfo.Name;
                this.FileSize = fileinfo.Length;
                this.Location = fileinfo.FullName;
                this.Identifier = Guid.NewGuid();
                this.FileType = TypeFromExtension(fileinfo.Extension);                
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Filetype: {0}", this.FileType));
                this.DateAdded = DateTime.Today;
                this.DateUpdated = DateTime.Today;
            }
            ContinueCreateInfo(file);
            //WriteInfoFile();
        }

        public abstract void ContinueCreateInfo(string file);

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
            } else if (prop.GetType() == typeof(int)){
                return prop.ToString();
            } else {
                return prop;
            }
        }

        public dynamic GetSortingProperty(string propName){
            //ints return ints etc etc
            if (this.ProcessProperty(propName) == null){
                return null;
            }
            var prop = this.ProcessProperty(propName);
            if (prop.GetType() == typeof(string)){
                return prop.ToString();
            } else if (prop.GetType() == typeof(DateTime)){
                DateTime dt = (DateTime)prop;                
                return dt.ToString("yyyy/MM/dd H:mm");
            } else if (prop.GetType() == typeof(bool)){
                return prop.ToString();
            } else if (propName == "FileSize"){
                return prop;
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
            } else if (prop.GetType() == typeof(int)){
                return prop;
            } else {
                return prop;
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
            } else if (string.Equals(extension, "folder", StringComparison.OrdinalIgnoreCase)){
                return FileTypes.Folder;
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
            } else if (type == FileTypes.Folder){
                return "Folder";
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
        [XmlIgnore]
        public override bool Selected {
            get { return selected;} set {selected = value;}
        }
        public List<string> LinkedFiles {get; set;} = new();
        public List<SimsPackageSubfolder> LinkedFolders {get; set;} = new();
        public Category Category {get; set;}
        public int LoadOrder {get; set;} = -1;
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
        public bool LoadAsFolder {get; set;} = false;
        public bool Misc {get; set;} = false;
        public Texture2D Thumbnail {get; set;}
        public List<string> Conflicts {get; set;}
        public List<string> DuplicatePackages {get; set;}
        public List<string> OverriddenPackages {get; set;}        
        public bool Enabled {get; set;} = false;
        public bool Scanned {get; set;} = false;

        public ScanData ScanData {get; set;}

        public SimsPackage(){
            Conflicts = new();
            DuplicatePackages = new();
            OverriddenPackages = new();            
        }

        public override void ContinueCreateInfo(string file){
            FileInfo fileinfo = new(file);
            this.Enabled = false;
            this.Scanned = false;
            if (this.FileType == FileTypes.TS4Script){
                if (File.Exists(file.Replace(fileinfo.Extension, ".package"))){
                    return;
                }
            } else if (this.FileType == FileTypes.Package){
                if (File.Exists(file.Replace(fileinfo.Extension, ".ts4script"))){
                    this.LinkedFiles.Add(file.Replace(fileinfo.Extension, ".ts4script"));
                }
            }
            if (this.FileType == FileTypes.Package){
                int game = GetGameVersion.CheckGame(file);
                if (game == 0){
                    this.Broken = true;
                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Package {0} is broken.", file));
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
                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Package {0} is {1}.", file, game)); 
                }
            } else if (this.FileType == FileTypes.TS4Script){
                this.Game = Games.Sims4;
            } else if (this.FileType == FileTypes.Sims3Pack){
                this.Game = Games.Sims3;
            } else if (this.FileType == FileTypes.Sims2Pack){
                this.Game = Games.Sims2;
            } else if (this.FileType == FileTypes.Other) { 
                this.Misc = true;
                this.Game = Games.Null;
                this.WrongGame = false;
            } else if (this.FileType == FileTypes.Zip || this.FileType == FileTypes.PNG || this.FileType == FileTypes.Txt || this.FileType == FileTypes.JPG || this.FileType == FileTypes.Doc || this.FileType == FileTypes.SevenZip || this.FileType == FileTypes.Rar) { 
                this.Misc = true;
                this.Game = Games.Null;
                this.WrongGame = false;
            } else if (this.Folder){
                this.FileType = FileTypes.Folder;
                if (LoadedSettings.SetSettings.CurrentInstance.Game == "Sims2"){
                    this.Game = Games.Sims2;
                } else if (LoadedSettings.SetSettings.CurrentInstance.Game == "Sims3"){
                    this.Game = Games.Sims3;
                } else if (LoadedSettings.SetSettings.CurrentInstance.Game == "Sims4"){
                    this.Game = Games.Sims4;
                }                
            }        
            WriteInfoFile();
        }

        public override void WriteInfoFile()
        {
            if (File.Exists(InfoFile)){
                File.Delete(InfoFile);                
            }
            XmlSerializer packageSerializer = new XmlSerializer(this.GetType());
            try { 
                using (var writer = new StreamWriter(InfoFile))
                {
                    
                        packageSerializer.Serialize(writer, this); 
                    
                }
            } catch (Exception e) {
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Writing info file for {0} failed: {1}\n{2}\n{3}\n.", this.FileName, e.Message, e.StackTrace, e.Source));
            }
        }

        public static string ExpansionToString(Sims2Expansions expansion){
            if (expansion == Sims2Expansions.BaseGame) { return "BaseGame"; }
            if (expansion == Sims2Expansions.University) { return "University"; }
            if (expansion == Sims2Expansions.Nightlife) { return "Nightlife"; }
            if (expansion == Sims2Expansions.OpenforBusiness) { return "OpenforBusiness"; }
            if (expansion == Sims2Expansions.Pets) { return "Pets"; }
            if (expansion == Sims2Expansions.Seasons) { return "Seasons"; }
            if (expansion == Sims2Expansions.BonVoyage) { return "BonVoyage"; }
            if (expansion == Sims2Expansions.FreeTime) { return "FreeTime"; }
            if (expansion == Sims2Expansions.ApartmentLife) { return "ApartmentLife"; }
            if (expansion == Sims2Expansions.FamilyFunStuff) { return "FamilyFunStuff"; }
            if (expansion == Sims2Expansions.GlamourLifeStuff) { return "GlamourLifeStuff"; }
            if (expansion == Sims2Expansions.HappyHolidayStuff) { return "HappyHolidayStuff"; }
            if (expansion == Sims2Expansions.CelebrationStuff) { return "CelebrationStuff"; }
            if (expansion == Sims2Expansions.HMFashionStuff) { return "HMFashionStuff"; }
            if (expansion == Sims2Expansions.TeenStyleStuff) { return "TeenStyleStuff"; }
            if (expansion == Sims2Expansions.KitchenBathInteriorDesignStuff) { return "KitchenBathInteriorDesignStuff"; }
            if (expansion == Sims2Expansions.IKEAHomeStuff) { return "IKEAHomeStuff"; }
            if (expansion == Sims2Expansions.MansionGardenStuff) { return "MansionGardenStuff"; }

            return "";
        }
        public static string ExpansionToString(Sims3Expansions expansion){
            if (expansion == Sims3Expansions.BaseGame) { return "BaseGame"; }
            if (expansion == Sims3Expansions.WorldAdventures) { return "WorldAdventures"; }
            if (expansion == Sims3Expansions.Ambitions) { return "Ambitions"; }
            if (expansion == Sims3Expansions.LateNight) { return "LateNight"; }
            if (expansion == Sims3Expansions.Generations) { return "Generations"; }
            if (expansion == Sims3Expansions.Pets) { return "Pets"; }
            if (expansion == Sims3Expansions.Showtime) { return "Showtime"; }
            if (expansion == Sims3Expansions.Supernatural) { return "Supernatural"; }
            if (expansion == Sims3Expansions.Seasons) { return "Seasons"; }
            if (expansion == Sims3Expansions.UniversityLife) { return "UniversityLife"; }
            if (expansion == Sims3Expansions.IslandParadise) { return "IslandParadise"; }
            if (expansion == Sims3Expansions.IntotheFuture) { return "IntotheFuture"; }
            if (expansion == Sims3Expansions.HighEndLoftStuff) { return "HighEndLoftStuff"; }
            if (expansion == Sims3Expansions.FastLaneStuff) { return "FastLaneStuff"; }
            if (expansion == Sims3Expansions.OutdoorLivingStuff) { return "OutdoorLivingStuff"; }
            if (expansion == Sims3Expansions.TownLifeStuff) { return "TownLifeStuff"; }
            if (expansion == Sims3Expansions.MasterSuiteStuff) { return "MasterSuiteStuff"; }
            if (expansion == Sims3Expansions.KatyPerrysSweetTreats) { return "KatyPerrysSweetTreats"; }
            if (expansion == Sims3Expansions.DieselStuff) { return "DieselStuff"; }
            if (expansion == Sims3Expansions.DecadesStuff) { return "DecadesStuff"; }
            if (expansion == Sims3Expansions.MovieStuff) { return "MovieStuff"; }

            return "";
        }
        public static string ExpansionToString(Sims4Expansions expansion){
            if (expansion == Sims4Expansions.BaseGame) { return "BaseGame"; }
            if (expansion == Sims4Expansions.GettoWork) { return "GettoWork"; }
            if (expansion == Sims4Expansions.GetTogether) { return "GetTogether"; }
            if (expansion == Sims4Expansions.CityLiving) { return "CityLiving"; }
            if (expansion == Sims4Expansions.CatsDogs) { return "CatsDogs"; }
            if (expansion == Sims4Expansions.Seasons) { return "Seasons"; }
            if (expansion == Sims4Expansions.GetFamous) { return "GetFamous"; }
            if (expansion == Sims4Expansions.IslandLiving) { return "IslandLiving"; }
            if (expansion == Sims4Expansions.DiscoverUniversity) { return "DiscoverUniversity"; }
            if (expansion == Sims4Expansions.EcoLifestyle) { return "EcoLifestyle"; }
            if (expansion == Sims4Expansions.SnowyEscape) { return "SnowyEscape"; }
            if (expansion == Sims4Expansions.CottageLiving) { return "CottageLiving"; }
            if (expansion == Sims4Expansions.HighSchoolYears) { return "HighSchoolYears"; }
            if (expansion == Sims4Expansions.GrowingTogether) { return "GrowingTogether"; }
            if (expansion == Sims4Expansions.HorseRanch) { return "HorseRanch"; }
            if (expansion == Sims4Expansions.ForRent) { return "ForRent"; }
            if (expansion == Sims4Expansions.Lovestruck) { return "Lovestruck"; }
            if (expansion == Sims4Expansions.OutdoorRetreat) { return "OutdoorRetreat"; }
            if (expansion == Sims4Expansions.SpaDay) { return "SpaDay"; }
            if (expansion == Sims4Expansions.DineOut) { return "DineOut"; }
            if (expansion == Sims4Expansions.Vampires) { return "Vampires"; }
            if (expansion == Sims4Expansions.Parenthood) { return "Parenthood"; }
            if (expansion == Sims4Expansions.JungleAdventure) { return "JungleAdventure"; }
            if (expansion == Sims4Expansions.StrangerVille) { return "StrangerVille"; }
            if (expansion == Sims4Expansions.RealmofMagic) { return "RealmofMagic"; }
            if (expansion == Sims4Expansions.JourneytoBatuu) { return "JourneytoBatuu"; }
            if (expansion == Sims4Expansions.DreamHomeDecorator) { return "DreamHomeDecorator"; }
            if (expansion == Sims4Expansions.MyWeddingStories) { return "MyWeddingStories"; }
            if (expansion == Sims4Expansions.Werewolves) { return "Werewolves"; }
            if (expansion == Sims4Expansions.LuxuryPartyStuff) { return "LuxuryPartyStuff"; }
            if (expansion == Sims4Expansions.PerfectPatioStuff) { return "PerfectPatioStuff"; }
            if (expansion == Sims4Expansions.CoolKitchenStuff) { return "CoolKitchenStuff"; }
            if (expansion == Sims4Expansions.SpookyStuff) { return "SpookyStuff"; }
            if (expansion == Sims4Expansions.MovieHangoutStuff) { return "MovieHangoutStuff"; }
            if (expansion == Sims4Expansions.RomanticGardenStuff) { return "RomanticGardenStuff"; }
            if (expansion == Sims4Expansions.KidsRoomStuff) { return "KidsRoomStuff"; }
            if (expansion == Sims4Expansions.BackyardStuff) { return "BackyardStuff"; }
            if (expansion == Sims4Expansions.VintageGlamourStuff) { return "VintageGlamourStuff"; }
            if (expansion == Sims4Expansions.BowlingNightStuff) { return "BowlingNightStuff"; }
            if (expansion == Sims4Expansions.FitnessStuff) { return "FitnessStuff"; }
            if (expansion == Sims4Expansions.ToddlerStuff) { return "ToddlerStuff"; }
            if (expansion == Sims4Expansions.LaundryDayStuff) { return "LaundryDayStuff"; }
            if (expansion == Sims4Expansions.MyFirstPetStuff) { return "MyFirstPetStuff"; }
            if (expansion == Sims4Expansions.MoschinoStuff) { return "MoschinoStuff"; }
            if (expansion == Sims4Expansions.TinyLivingStuff) { return "TinyLivingStuff"; }
            if (expansion == Sims4Expansions.NiftyKnittingStuff) { return "NiftyKnittingStuff"; }
            if (expansion == Sims4Expansions.ParanormalStuff) { return "ParanormalStuff"; }
            if (expansion == Sims4Expansions.HomeChefHustleStuff) { return "HomeChefHustleStuff"; }
            if (expansion == Sims4Expansions.CrystalCreationsStuff) { return "CrystalCreationsStuff"; }
            if (expansion == Sims4Expansions.BusttheDustKit) { return "BusttheDustKit"; }
            if (expansion == Sims4Expansions.CountryKitchenKit) { return "CountryKitchenKit"; }
            if (expansion == Sims4Expansions.ThrowbackFitKit) { return "ThrowbackFitKit"; }
            if (expansion == Sims4Expansions.CourtyardOasisKit) { return "CourtyardOasisKit"; }
            if (expansion == Sims4Expansions.IndustrialLoftKit) { return "IndustrialLoftKit"; }
            if (expansion == Sims4Expansions.FashionStreetKit) { return "FashionStreetKit"; }
            if (expansion == Sims4Expansions.IncheonArrivalsKit) { return "IncheonArrivalsKit"; }
            if (expansion == Sims4Expansions.BloomingRoomsKit) { return "BloomingRoomsKit"; }
            if (expansion == Sims4Expansions.ModernMenswearKit) { return "ModernMenswearKit"; }
            if (expansion == Sims4Expansions.CarnavalStreetwearKit) { return "CarnavalStreetwearKit"; }
            if (expansion == Sims4Expansions.DécortotheMaxKit) { return "DécortotheMaxKit"; }
            if (expansion == Sims4Expansions.MoonlightChicKit) { return "MoonlightChicKit"; }
            if (expansion == Sims4Expansions.LittleCampersKit) { return "LittleCampersKit"; }
            if (expansion == Sims4Expansions.FirstFitsKit) { return "FirstFitsKit"; }
            if (expansion == Sims4Expansions.DesertLuxeKit) { return "DesertLuxeKit"; }
            if (expansion == Sims4Expansions.EverydayClutterKit) { return "EverydayClutterKit"; }
            if (expansion == Sims4Expansions.PastelPopKit) { return "PastelPopKit"; }
            if (expansion == Sims4Expansions.BathroomClutterKit) { return "BathroomClutterKit"; }
            if (expansion == Sims4Expansions.SimtimatesCollectionKit) { return "SimtimatesCollectionKit"; }
            if (expansion == Sims4Expansions.BasementTreasuresKit) { return "BasementTreasuresKit"; }
            if (expansion == Sims4Expansions.GreenhouseHavenKit) { return "GreenhouseHavenKit"; }
            if (expansion == Sims4Expansions.GrungeRevivalKit) { return "GrungeRevivalKit"; }
            if (expansion == Sims4Expansions.BookNookKit) { return "BookNookKit"; }
            if (expansion == Sims4Expansions.ModernLuxeKit) { return "ModernLuxeKit"; }
            if (expansion == Sims4Expansions.PoolsideSplashKit) { return "PoolsideSplashKit"; }
            if (expansion == Sims4Expansions.CastleEstateKit) { return "CastleEstateKit"; }
            if (expansion == Sims4Expansions.GothGaloreKit) { return "GothGaloreKit"; }
            if (expansion == Sims4Expansions.UrbanHomageKit) { return "UrbanHomageKit"; }
            if (expansion == Sims4Expansions.PartyEssentialsKit) { return "PartyEssentialsKit"; }
            if (expansion == Sims4Expansions.RivieraRetreatKit) { return "RivieraRetreatKit"; }
            if (expansion == Sims4Expansions.CozyBistroKit) { return "CozyBistroKit"; }
            return "";
        }

        public static Sims2Expansions S2ExpansionFromString (string expansion){
            if (expansion == "BaseGame") { return Sims2Expansions.BaseGame; }
            if (expansion == "University") { return Sims2Expansions.University; }
            if (expansion == "Nightlife") { return Sims2Expansions.Nightlife; }
            if (expansion == "OpenforBusiness") { return Sims2Expansions.OpenforBusiness; }
            if (expansion == "Pets") { return Sims2Expansions.Pets; }
            if (expansion == "Seasons") { return Sims2Expansions.Seasons; }
            if (expansion == "BonVoyage") { return Sims2Expansions.BonVoyage; }
            if (expansion == "FreeTime") { return Sims2Expansions.FreeTime; }
            if (expansion == "ApartmentLife") { return Sims2Expansions.ApartmentLife; }
            if (expansion == "FamilyFunStuff") { return Sims2Expansions.FamilyFunStuff; }
            if (expansion == "GlamourLifeStuff") { return Sims2Expansions.GlamourLifeStuff; }
            if (expansion == "HappyHolidayStuff") { return Sims2Expansions.HappyHolidayStuff; }
            if (expansion == "CelebrationStuff") { return Sims2Expansions.CelebrationStuff; }
            if (expansion == "HMFashionStuff") { return Sims2Expansions.HMFashionStuff; }
            if (expansion == "TeenStyleStuff") { return Sims2Expansions.TeenStyleStuff; }
            if (expansion == "KitchenBathInteriorDesignStuff") { return Sims2Expansions.KitchenBathInteriorDesignStuff; }
            if (expansion == "IKEAHomeStuff") { return Sims2Expansions.IKEAHomeStuff; }
            if (expansion == "MansionGardenStuff") { return Sims2Expansions.MansionGardenStuff; }
            return Sims2Expansions.BaseGame;
        }
        public static Sims3Expansions S3ExpansionFromString (string expansion){
            if (expansion == "BaseGame") { return Sims3Expansions.BaseGame; }
            if (expansion == "WorldAdventures") { return Sims3Expansions.WorldAdventures; }
            if (expansion == "Ambitions") { return Sims3Expansions.Ambitions; }
            if (expansion == "LateNight") { return Sims3Expansions.LateNight; }
            if (expansion == "Generations") { return Sims3Expansions.Generations; }
            if (expansion == "Pets") { return Sims3Expansions.Pets; }
            if (expansion == "Showtime") { return Sims3Expansions.Showtime; }
            if (expansion == "Supernatural") { return Sims3Expansions.Supernatural; }
            if (expansion == "Seasons") { return Sims3Expansions.Seasons; }
            if (expansion == "UniversityLife") { return Sims3Expansions.UniversityLife; }
            if (expansion == "IslandParadise") { return Sims3Expansions.IslandParadise; }
            if (expansion == "IntotheFuture") { return Sims3Expansions.IntotheFuture; }
            if (expansion == "HighEndLoftStuff") { return Sims3Expansions.HighEndLoftStuff; }
            if (expansion == "FastLaneStuff") { return Sims3Expansions.FastLaneStuff; }
            if (expansion == "OutdoorLivingStuff") { return Sims3Expansions.OutdoorLivingStuff; }
            if (expansion == "TownLifeStuff") { return Sims3Expansions.TownLifeStuff; }
            if (expansion == "MasterSuiteStuff") { return Sims3Expansions.MasterSuiteStuff; }
            if (expansion == "KatyPerrysSweetTreats") { return Sims3Expansions.KatyPerrysSweetTreats; }
            if (expansion == "DieselStuff") { return Sims3Expansions.DieselStuff; }
            if (expansion == "DecadesStuff") { return Sims3Expansions.DecadesStuff; }
            if (expansion == "MovieStuff") { return Sims3Expansions.MovieStuff; }           
           
            return Sims3Expansions.BaseGame;
        }
        public static Sims4Expansions S4ExpansionFromString (string expansion){
            if (expansion == "BaseGame") { return Sims4Expansions.BaseGame; }
            if (expansion == "GettoWork") { return Sims4Expansions.GettoWork; }
            if (expansion == "GetTogether") { return Sims4Expansions.GetTogether; }
            if (expansion == "CityLiving") { return Sims4Expansions.CityLiving; }
            if (expansion == "CatsDogs") { return Sims4Expansions.CatsDogs; }
            if (expansion == "Seasons") { return Sims4Expansions.Seasons; }
            if (expansion == "GetFamous") { return Sims4Expansions.GetFamous; }
            if (expansion == "IslandLiving") { return Sims4Expansions.IslandLiving; }
            if (expansion == "DiscoverUniversity") { return Sims4Expansions.DiscoverUniversity; }
            if (expansion == "EcoLifestyle") { return Sims4Expansions.EcoLifestyle; }
            if (expansion == "SnowyEscape") { return Sims4Expansions.SnowyEscape; }
            if (expansion == "CottageLiving") { return Sims4Expansions.CottageLiving; }
            if (expansion == "HighSchoolYears") { return Sims4Expansions.HighSchoolYears; }
            if (expansion == "GrowingTogether") { return Sims4Expansions.GrowingTogether; }
            if (expansion == "HorseRanch") { return Sims4Expansions.HorseRanch; }
            if (expansion == "ForRent") { return Sims4Expansions.ForRent; }
            if (expansion == "Lovestruck") { return Sims4Expansions.Lovestruck; }
            if (expansion == "OutdoorRetreat") { return Sims4Expansions.OutdoorRetreat; }
            if (expansion == "SpaDay") { return Sims4Expansions.SpaDay; }
            if (expansion == "DineOut") { return Sims4Expansions.DineOut; }
            if (expansion == "Vampires") { return Sims4Expansions.Vampires; }
            if (expansion == "Parenthood") { return Sims4Expansions.Parenthood; }
            if (expansion == "JungleAdventure") { return Sims4Expansions.JungleAdventure; }
            if (expansion == "StrangerVille") { return Sims4Expansions.StrangerVille; }
            if (expansion == "RealmofMagic") { return Sims4Expansions.RealmofMagic; }
            if (expansion == "JourneytoBatuu") { return Sims4Expansions.JourneytoBatuu; }
            if (expansion == "DreamHomeDecorator") { return Sims4Expansions.DreamHomeDecorator; }
            if (expansion == "MyWeddingStories") { return Sims4Expansions.MyWeddingStories; }
            if (expansion == "Werewolves") { return Sims4Expansions.Werewolves; }
            if (expansion == "LuxuryPartyStuff") { return Sims4Expansions.LuxuryPartyStuff; }
            if (expansion == "PerfectPatioStuff") { return Sims4Expansions.PerfectPatioStuff; }
            if (expansion == "CoolKitchenStuff") { return Sims4Expansions.CoolKitchenStuff; }
            if (expansion == "SpookyStuff") { return Sims4Expansions.SpookyStuff; }
            if (expansion == "MovieHangoutStuff") { return Sims4Expansions.MovieHangoutStuff; }
            if (expansion == "RomanticGardenStuff") { return Sims4Expansions.RomanticGardenStuff; }
            if (expansion == "KidsRoomStuff") { return Sims4Expansions.KidsRoomStuff; }
            if (expansion == "BackyardStuff") { return Sims4Expansions.BackyardStuff; }
            if (expansion == "VintageGlamourStuff") { return Sims4Expansions.VintageGlamourStuff; }
            if (expansion == "BowlingNightStuff") { return Sims4Expansions.BowlingNightStuff; }
            if (expansion == "FitnessStuff") { return Sims4Expansions.FitnessStuff; }
            if (expansion == "ToddlerStuff") { return Sims4Expansions.ToddlerStuff; }
            if (expansion == "LaundryDayStuff") { return Sims4Expansions.LaundryDayStuff; }
            if (expansion == "MyFirstPetStuff") { return Sims4Expansions.MyFirstPetStuff; }
            if (expansion == "MoschinoStuff") { return Sims4Expansions.MoschinoStuff; }
            if (expansion == "TinyLivingStuff") { return Sims4Expansions.TinyLivingStuff; }
            if (expansion == "NiftyKnittingStuff") { return Sims4Expansions.NiftyKnittingStuff; }
            if (expansion == "ParanormalStuff") { return Sims4Expansions.ParanormalStuff; }
            if (expansion == "HomeChefHustleStuff") { return Sims4Expansions.HomeChefHustleStuff; }
            if (expansion == "CrystalCreationsStuff") { return Sims4Expansions.CrystalCreationsStuff; }
            if (expansion == "BusttheDustKit") { return Sims4Expansions.BusttheDustKit; }
            if (expansion == "CountryKitchenKit") { return Sims4Expansions.CountryKitchenKit; }
            if (expansion == "ThrowbackFitKit") { return Sims4Expansions.ThrowbackFitKit; }
            if (expansion == "CourtyardOasisKit") { return Sims4Expansions.CourtyardOasisKit; }
            if (expansion == "IndustrialLoftKit") { return Sims4Expansions.IndustrialLoftKit; }
            if (expansion == "FashionStreetKit") { return Sims4Expansions.FashionStreetKit; }
            if (expansion == "IncheonArrivalsKit") { return Sims4Expansions.IncheonArrivalsKit; }
            if (expansion == "BloomingRoomsKit") { return Sims4Expansions.BloomingRoomsKit; }
            if (expansion == "ModernMenswearKit") { return Sims4Expansions.ModernMenswearKit; }
            if (expansion == "CarnavalStreetwearKit") { return Sims4Expansions.CarnavalStreetwearKit; }
            if (expansion == "DécortotheMaxKit") { return Sims4Expansions.DécortotheMaxKit; }
            if (expansion == "MoonlightChicKit") { return Sims4Expansions.MoonlightChicKit; }
            if (expansion == "LittleCampersKit") { return Sims4Expansions.LittleCampersKit; }
            if (expansion == "FirstFitsKit") { return Sims4Expansions.FirstFitsKit; }
            if (expansion == "DesertLuxeKit") { return Sims4Expansions.DesertLuxeKit; }
            if (expansion == "EverydayClutterKit") { return Sims4Expansions.EverydayClutterKit; }
            if (expansion == "PastelPopKit") { return Sims4Expansions.PastelPopKit; }
            if (expansion == "BathroomClutterKit") { return Sims4Expansions.BathroomClutterKit; }
            if (expansion == "SimtimatesCollectionKit") { return Sims4Expansions.SimtimatesCollectionKit; }
            if (expansion == "BasementTreasuresKit") { return Sims4Expansions.BasementTreasuresKit; }
            if (expansion == "GreenhouseHavenKit") { return Sims4Expansions.GreenhouseHavenKit; }
            if (expansion == "GrungeRevivalKit") { return Sims4Expansions.GrungeRevivalKit; }
            if (expansion == "BookNookKit") { return Sims4Expansions.BookNookKit; }
            if (expansion == "ModernLuxeKit") { return Sims4Expansions.ModernLuxeKit; }
            if (expansion == "PoolsideSplashKit") { return Sims4Expansions.PoolsideSplashKit; }
            if (expansion == "CastleEstateKit") { return Sims4Expansions.CastleEstateKit; }
            if (expansion == "GothGaloreKit") { return Sims4Expansions.GothGaloreKit; }
            if (expansion == "UrbanHomageKit") { return Sims4Expansions.UrbanHomageKit; }
            if (expansion == "PartyEssentialsKit") { return Sims4Expansions.PartyEssentialsKit; }
            if (expansion == "RivieraRetreatKit") { return Sims4Expansions.RivieraRetreatKit; }
            if (expansion == "CozyBistroKit") { return Sims4Expansions.CozyBistroKit; }            
           
           return Sims4Expansions.SpaDay;
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
        [XmlIgnore]
        public override bool Selected {
            get { return selected;} set {selected = value;}
        }

        public bool Installed {get; set;} = false;

        public override void ContinueCreateInfo(string file){
            this.Installed = false;
            WriteInfoFile();
        }
        public override void WriteInfoFile()
        {
            if (File.Exists(InfoFile)){
                File.Delete(InfoFile);                
            }
            XmlSerializer packageSerializer = new XmlSerializer(this.GetType());
            try { 
                using (var writer = new StreamWriter(InfoFile))
                {
                    
                        packageSerializer.Serialize(writer, this); 
                    
                }
            } catch (Exception e) {
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Writing info file for {0} failed: {1}\n{2}\n{3}\n.", this.FileName, e.Message, e.StackTrace, e.Source));
            }
        }
    }    

    [XmlInclude(typeof(ScanData))]
    [XmlInclude(typeof(Sims2ScanData))]
    [XmlInclude(typeof(Sims3ScanData))]
    [XmlInclude(typeof(Sims4ScanData))]
    public abstract class ScanData{
        protected string description;
        public abstract string Description {get; set;}

        protected string type;
        public abstract string Type {get; set;}

        protected string subtype;
        public abstract string Subtype {get; set;}

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

        protected string thumbnaillocation;        
        public abstract string ThumbnailLocation {get; set;}

        public abstract StringBuilder GetStringBuilder(StringBuilder sb);

        public abstract StringBuilder PackageInformationDump();

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
        public override string ThumbnailLocation{
            get { return thumbnaillocation; } set {thumbnaillocation = value; } 
        }

        public List<S2CTSS> CTSSData {get; set;} = new();
        public List<S2CPF> CPFData {get; set;} = new();
        public List<S2STR> STRData {get; set;} = new();
        public List<S2XML> XMLData {get; set;} = new();
        public List<S2OBJD> OBJDData {get; set;} = new();
        public List<S2SHPE> SHPEData {get; set;} = new();
        public List<string> InstanceIDs {get; set;} = new();
        public List<string> GUIDs {get; set;} = new();
        public List<Sims2Expansions> RequiredEPs {get; set;} = new();   
        
        
        public string PackageType {get; set;} = "";

        public override StringBuilder PackageInformationDump()
        {
            StringBuilder sb = new();
            sb.AppendLine(string.Format("Type: {0}", Type));
            sb.AppendLine(string.Format("Subtype: {0}", Subtype));
            List<string> titles = new();
            List<string> ctsstit = CTSSData.Select(x => x.Title).ToList();
            List<string> cpftit = CPFData.Select(x => x.Title).ToList();
            List<string> strtit = STRData.Select(x => x.Title).ToList();
            List<string> xmltit = XMLData.Select(x => x.Title).ToList();
            List<string> descriptions = new();
            List<string> ctssdes = CTSSData.Select(x => x.Description).ToList();
            List<string> cpfdes = CPFData.Select(x => x.Description).ToList();
            List<string> strdes = STRData.Select(x => x.Description).ToList();
            List<string> xmldes = XMLData.Select(x => x.Description).ToList();

            titles.AddRange(ctsstit);
            titles.AddRange(cpftit);
            titles.AddRange(strtit);
            titles.AddRange(xmltit);
            descriptions.AddRange(ctssdes);
            descriptions.AddRange(cpfdes);
            descriptions.AddRange(strdes);
            descriptions.AddRange(xmldes);

            titles = titles.Distinct().ToList();
            descriptions = descriptions.Distinct().ToList();
            
            if (titles.Count > 1){
                sb.AppendLine(string.Format("Internal Names:"));
                foreach (string title in titles){
                    sb.AppendLine(string.Format(title));
                }
            } else {
                sb.AppendLine(string.Format("Internal Name: {0}", titles[0]));
            }
            if (descriptions.Count > 1){
                sb.AppendLine(string.Format("Internal Descriptions:"));
                foreach (string description in descriptions){
                    sb.AppendLine(string.Format(description));
                }
            } else {
                sb.AppendLine(string.Format("Internal Description: {0}", descriptions[0]));
            }
            return sb;
        }







        //public List<TagsList> CatalogTags {get; set;}
        //public AgeGenderFlags AgeGenderFlags {get; set;}
        //public List<PackageEntries> FileHas {get; set;}
        //public List<PackageRoomSort> RoomSort {get; set;}
        //public List<PackageComponent> Components {get; set;}
        //public List<PackageTypeCounter> Entries         {get; set;}
        //public List<PackageFlag> Flags {get; set;}
        //public List<PackageMeshKeys> MeshKeys {get; set;}
        //public List<PackageCASPartKeys> CASPartKeys {get; set;}
        //public List<PackageOBJDKeys> OBJDPartKeys {get; set;}
        //public List<PackageMatchingRecolors> MatchingRecolors {get; set;}*/



        public override StringBuilder GetStringBuilder(StringBuilder sb){
            /*sb.AppendLine(string.Format("{0}={1}", "Title", GetProperty("Title")));
            sb.AppendLine(string.Format("{0}={1}", "ModelName", GetProperty("Age")));
            sb.AppendLine(string.Format("{0}={1}", "Gender", GetProperty("Gender")));
            sb.AppendLine(string.Format("{0}={1}", "Function", GetProperty("Function")));
            sb.AppendLine(string.Format("{0}={1}", "FunctionSubcategory", GetProperty("FunctionSubcategory")));
            sb.AppendLine(string.Format("{0}={1}", "XMLType", GetProperty("XMLType")));
            sb.AppendLine(string.Format("{0}={1}", "XMLSubtype", GetProperty("XMLSubtype")));
            sb.AppendLine(string.Format("{0}={1}", "XMLCategory", GetProperty("XMLCategory")));
            sb.AppendLine(string.Format("{0}={1}", "XMLModelName", GetProperty("XMLModelName")));
            sb.AppendLine(string.Format("{0}={1}", "XMLCreator", GetProperty("XMLCreator")));
            sb.AppendLine(string.Format("{0}={1}", "XMLAge", GetProperty("XMLAge")));
            sb.AppendLine(string.Format("{0}={1}", "XMLGender", GetProperty("XMLGender")));
            sb.AppendLine(string.Format("{0}={1}", "XMLFunction", GetProperty("XMLFunction")));
            sb.AppendLine(string.Format("{0}={1}", "XMLFunctionSubsort", GetProperty("XMLFunctionSubsort")));
            sb.AppendLine(string.Format("{0}={1}", "PackageType", GetProperty("PackageType")));
                        
            sb.AppendLine(string.Format("[INSTANCES]"));
            foreach (string instance in InstanceIDs){
                sb.AppendLine(string.Format(instance));
            }
            sb.AppendLine(string.Format("[GUIDS]"));
            foreach (string guid in GUIDs){
                sb.AppendLine(string.Format(guid));
            }
            sb.AppendLine(string.Format("[REQUIRED EPS]"));
            foreach (Sims2Expansions ep in RequiredEPs){
                sb.Append(string.Format(ep.ToString()));
            }                        
            sb.AppendLine(string.Format("[TAGS]"));
            foreach (TagsList tag in CatalogTags){
                sb.Append(string.Format(@"{0}\{1},", tag.Function, tag.Subfunction));
            }
*/
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
        public override string ThumbnailLocation{
            get { return thumbnaillocation; } set {thumbnaillocation = value; } 
        }

        public override StringBuilder PackageInformationDump()
        {
            throw new NotImplementedException();
        }


        public override StringBuilder GetStringBuilder(StringBuilder sb){
            //sb.AppendLine(string.Format("{0}={1}", "Description", GetProperty("Description")));
            
            
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
        public override string ThumbnailLocation{
            get { return thumbnaillocation; } set {thumbnaillocation = value; } 
        }

        public override StringBuilder PackageInformationDump()
        {
            throw new NotImplementedException();
        }

        public override StringBuilder GetStringBuilder(StringBuilder sb){
            //sb.AppendLine(string.Format("{0}={1}", "Description", GetProperty("Description")));
            
            
            return sb;
        }
    }




    public class SimsPackageSubfolder{
        public string Folder {get; set;}
        public List<string> Subfiles {get; set;} = new();
        public List<SimsPackageSubfolder> Subfolders {get; set;} = new();
    }

    public class TagsList {
        /// <summary>
        /// Used to get the tags from Sims 4 packages; catalog tags etc.
        /// </summary>
        public int Id {get; set;} = -1;
        public string Description {get; set;} = "";
        public string Function {get; set;} = "";
        public string Subfunction {get; set;} = "";
        public string TypeID {get; set;} = "";
    }

    public class FunctionSortList {
        /// <summary>
        /// Used for Sims 2 (I believe) function categorization. 
        /// </summary>
        public int flagnum {get; set;}
        public int functionsubsortnum {get; set;}
        public string Category {get; set;}
        public string Subcategory {get; set;}
    } 

    public class S2CTSS {
        public string Title {get; set;} = "";
        public string Description {get; set;} = "";
    }

    public class S2CPF {
        public string Title {get; set;} = "";
        public string Description {get; set;} = "";
        public string XMLType {get; set;} = "";
        public string XMLSubtype {get; set;} = "";
        public string XMLCategory {get; set;} = "";
        public string XMLModelName {get; set;} = "";
        public string XMLCreator {get; set;} = "";
        public string XMLAge {get; set;} = "";
        public string XMLGender {get; set;} = "";
        public List<string> GUIDs {get; set;} = new();
    }

    public class S2STR {
        public string Title {get; set;} = "";
        public string Description {get; set;} = "";
    }

    public class S2XML {
        public string Title {get; set;} = "";
        public string Description {get; set;} = "";       
        public string XMLType {get; set;} = "";    
        public string XMLSubtype {get; set;} = "";
        public string XMLCategory {get; set;} = "";
        
    }

    public class S2OBJD {
        public string XMLCategory {get; set;} = "";
        public string Function {get; set;} = "";
        public string FunctionSubcategory {get; set;} = "";
        public List<string> GUIDs {get; set;} = new();
    }

    public class S2SHPE {
        public string Type {get; set;} = "";
        public List<TagsList> CatalogTags {get; set;} = new();
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
    Folder,
    Null
}

public enum Sims2Expansions{
    BaseGame,
    University,
    Nightlife,
    OpenforBusiness,
    Pets,
    Seasons,
    BonVoyage,
    FreeTime,
    ApartmentLife,
    FamilyFunStuff,
    GlamourLifeStuff,
    HappyHolidayStuff,
    CelebrationStuff,
    HMFashionStuff,
    TeenStyleStuff,
    KitchenBathInteriorDesignStuff,
    IKEAHomeStuff,
    MansionGardenStuff
}

public enum Sims3Expansions{
    BaseGame,
    WorldAdventures,
    Ambitions,
    LateNight,
    Generations,
    Pets,
    Showtime,
    Supernatural,
    Seasons,
    UniversityLife,
    IslandParadise,
    IntotheFuture,
    HighEndLoftStuff,
    FastLaneStuff,
    OutdoorLivingStuff,
    TownLifeStuff,
    MasterSuiteStuff,
    KatyPerrysSweetTreats,
    DieselStuff,
    DecadesStuff,
    MovieStuff
}

public enum Sims4Expansions{
    BaseGame,
    GettoWork,
    GetTogether,
    CityLiving,
    CatsDogs,
    Seasons,
    GetFamous,
    IslandLiving,
    DiscoverUniversity,
    EcoLifestyle,
    SnowyEscape,
    CottageLiving,
    HighSchoolYears,
    GrowingTogether,
    HorseRanch,
    ForRent,
    Lovestruck,
    OutdoorRetreat,
    SpaDay,
    DineOut,
    Vampires,
    Parenthood,
    JungleAdventure,
    StrangerVille,
    RealmofMagic,
    JourneytoBatuu,
    DreamHomeDecorator,
    MyWeddingStories,
    Werewolves,
    LuxuryPartyStuff,
    PerfectPatioStuff,
    CoolKitchenStuff,
    SpookyStuff,
    MovieHangoutStuff,
    RomanticGardenStuff,
    KidsRoomStuff,
    BackyardStuff,
    VintageGlamourStuff,
    BowlingNightStuff,
    FitnessStuff,
    ToddlerStuff,
    LaundryDayStuff,
    MyFirstPetStuff,
    MoschinoStuff,
    TinyLivingStuff,
    NiftyKnittingStuff,
    ParanormalStuff,
    HomeChefHustleStuff,
    CrystalCreationsStuff,
    BusttheDustKit,
    CountryKitchenKit,
    ThrowbackFitKit,
    CourtyardOasisKit,
    IndustrialLoftKit,
    FashionStreetKit,
    IncheonArrivalsKit,
    BloomingRoomsKit,
    ModernMenswearKit,
    CarnavalStreetwearKit,
    DécortotheMaxKit,
    MoonlightChicKit,
    LittleCampersKit,
    FirstFitsKit,
    DesertLuxeKit,
    EverydayClutterKit,
    PastelPopKit,
    BathroomClutterKit,
    SimtimatesCollectionKit,
    BasementTreasuresKit,
    GreenhouseHavenKit,
    GrungeRevivalKit,
    BookNookKit,
    ModernLuxeKit,
    PoolsideSplashKit,
    CastleEstateKit,
    GothGaloreKit,
    UrbanHomageKit,
    PartyEssentialsKit,
    RivieraRetreatKit,
    CozyBistroKit
}