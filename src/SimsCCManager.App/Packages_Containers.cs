using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Text;
using System.Data.SQLite;
using SQLitePCL;
using SQLite;
using SQLiteNetExtensions.Attributes;
using Newtonsoft.Json;
using System.ComponentModel;

namespace SimsCCManager.Packages.Containers
{    
    /// <summary>
    /// Containing classes for holding the information the program gets.
    /// </summary>

    [Table("BrokenChecked")]
    public class BrokenChecked {
        public string Name {get; set;}
        public string Location {get; set;}
        public string Status {get; set;} //working vs broken
    }
     

    [Table ("AllFiles")]
    public class AllFiles {
        /// <summary>
        /// No specific "package" information; this just details whether a file is a package, a sims3pack, a ts4script, a sims2pack, a compressed file of some sort (zip etc) or something else, and files it with its location. 
        /// </summary>
        public string Type {get; set;}
        public string Location {get; set;}
        public string Name {get; set;}
        public string Status {get; set;}
    }
    [Table("Processing_Reader")]
    public class PackageFile { //A basic summary of a package file.
        /// <summary>
        /// Basic package file with no full information yet; game version, whether it's broken, and whether it's being processed or waiting to be processed.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int ID {get; set;}
        [Indexed]
        [Column("Name")]
        public string Name {get; set;}
        [Column("Location")]
        public string Location {get; set;}
        [Column("Game")]
        public int Game {get; set;}
        [Column("Broken")]
        public bool Broken {get; set;}
        public string Status {get; set;} //Pending or Processing, then removed
    }

    [Table("SP_TagsList")]
    public class TagsList {
        /// <summary>
        /// Used to get the tags from Sims 4 packages; catalog tags etc.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id {get; set;}
        [Column("Description")]
        public string Description {get; set;}
        [Column("TypeID")]
        public string TypeID {get; set;}
        
        [ForeignKey(typeof(SimsPackage))]
        public int PackageID {get; set;}
        [Column("SimsPackage")]
        
        [ManyToOne]
        public SimsPackage SimsPackage {get; set;}
    }

    public class ThingHolder<T> : List<T> {
        /// <summary>
        /// Unused rn?
        /// </summary>
        public string ID {get; set;}
    }

    [Table("Packages")]
    public class SimsPackage : INotifyPropertyChanged { // A more in depth package file.
        /// <summary>
        /// The full package summary used for sorting and editing the items.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int ID {get; set;}        
        [Column ("Title")]
        public string Title {get; set;}  
        [Column ("Description")]
        public string Description {get; set;}
        [Column ("Location")]
        public string Location {get; set;}
        [Column("FileSize")]
        public int FileSize {get; set;}
        [Column ("PackageName")]
        [Indexed]
        public string PackageName {get; set;}
        [Column ("Type")]
        public string Type {get; set;}
        [Column ("Game")]
        public int Game {get; set;}
        [Column ("GameString")]
        public string GameString {get; set;}
        [Column ("InstanceIDs")]
        [TextBlob("InstancesBlobbed")]
        public List<string> InstanceIDs {get; set;}
        public string InstancesBlobbed {get; set;}
        [Column ("Subtype")]
        public string Subtype {get; set;}
        [Column ("Category")]
        public string Category {get; set;}
        [Column ("ModelName")]
        public string ModelName {get; set;}
        [Column ("GUIDs")]
        [TextBlob("GuidsBlobbed")]
        public List<string> GUIDs {get; set;}
        public string GuidsBlobbed {get; set;}
        [Column ("Creator")]
        public string Creator {get; set;}
        [Column ("Age")]
        public string Age {get; set;}
        [Column ("Gender")]
        public string Gender {get; set;}
        [Column ("RequiredEPs")]
        [TextBlob("RequiredEPsBlob")]
        public List<string> RequiredEPs {get; set;}
        public string RequiredEPsBlob {get; set;}
        [Column ("Function")]
        public string Function {get; set;}
        [Column ("FunctionSubcategory")]
        public string FunctionSubcategory {get; set;}
        [Column ("AgeGenderFlags")]
        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert | CascadeOperation.CascadeDelete)]
        public AgeGenderFlags AgeGenderFlags {get; set;}
        [Column ("Entry Locations")]
        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert | CascadeOperation.CascadeDelete)]
        public List<fileHasList> FileHas {get; set;}
        [Column ("RoomSort")]        
        [TextBlob("RoomsBlobbed")]
        public List<string> RoomSort {get; set;}
        public string RoomsBlobbed {get; set;}
        [Column ("Components")]
        [TextBlob("ComponentsBlobbed")]
        public List<string> Components {get; set;}
        public string ComponentsBlobbed {get; set;}
        [Column ("Entries")]
        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert | CascadeOperation.CascadeDelete)]
        public List<TypeCounter> Entries {get; set;}        
        [Column ("Flags")]        
        [TextBlob("FlagsBlobbed")]
        public List<string> Flags {get; set;}
        public string FlagsBlobbed {get; set;}
        [Column ("CatalogTags")]
        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert | CascadeOperation.CascadeDelete)]
        public List<TagsList> CatalogTags {get; set;}
        [Column ("Broken")]
        public bool Broken {get; set;}
        [Column ("Mesh")]
        public bool Mesh {get; set;}
        [Column ("Recolor")]
        public bool Recolor {get; set;}
        [Column ("Orphan")]
        public bool Orphan {get; set;}
        [Column ("Override")]        
        public bool Override {get; set;}
        [Column ("OverriddenInstances")]
        [TextBlob("OverriddenInstancesBlobbed")]
        public List<string> OverriddenInstances {get; set;}
        public string OverriddenInstancesBlobbed {get; set;}
        [Column ("OverriddenItems")]        
        [TextBlob("OverriddenItemsBlobbed")]   
        public List<string> OverriddenItems {get; set;}
        public string OverriddenItemsBlobbed {get; set;}
        public string MatchingMesh {get; set;}
        [Column ("MatchingRecolors")]
        [TextBlob("MatchingRecolorsBlobbed")]
        public List<string> MatchingRecolors {get; set;}
        public string MatchingRecolorsBlobbed {get; set;}
        [Column ("MatchingConflicts")]
        [TextBlob("MatchingConflictsBlobbed")]
        public List<string> MatchingConflicts {get; set;}
        public string MatchingConflictsBlobbed {get; set;}

        public SimsPackage() {
            this.InstanceIDs = new List<string>();
            this.GUIDs = new List<string>();
            this.RequiredEPs = new List<string>();
            this.RoomSort = new List<string>();
            this.MatchingRecolors = new List<string>();
            this.Components = new List<string>();
            this.MatchingConflicts = new List<string>();
            this.Entries = new List<TypeCounter>();
            this.FileHas = new List<fileHasList>();
            this.Flags = new List<string>();
            this.CatalogTags = new List<TagsList>();
            this.OverriddenInstances = new List<string>();
            this.OverriddenItems = new List<string>();
            this.AgeGenderFlags = new AgeGenderFlags();
        }

        public string GetPropertyString(string propName){
            return this.ProcessProperty(propName).ToString();
        }
        public List<TagsList> GetPropertyTagsList(string propName){
            return (List<TagsList>)this.ProcessProperty(propName);
        }

        public object ProcessProperty(string propName){
            return this.GetType().GetProperty(propName).GetValue (this, null);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static string GetFormatListString(List<string> words){
            string retVal = string.Empty;
            foreach (string word in words){
                if (string.IsNullOrEmpty(retVal)){
                    retVal += word.ToString();
                } else {
                    retVal += string.Format(", {0}", word);
                }
                
            }
            return retVal;
        }

        public static string GetFormatTagsList(List<TagsList> tags){
            string retVal = string.Empty;
            foreach (TagsList tag in tags){
                if (string.IsNullOrEmpty(retVal)){
                    retVal += tag.TypeID.ToString() + ", " + tag.Description;
                } else {
                    retVal += string.Format(", " + tag.TypeID.ToString() + ": " + tag.Description);
                }
            }
            return retVal;
        }

        public static string GetFormatTypeCounter(List<TypeCounter> items){
            string retVal = string.Empty;
            foreach (TypeCounter item in items){
                string mystring = item.Type + ": " + item.Count;
                if (string.IsNullOrEmpty(retVal)){
                    retVal += mystring.ToString();
                } else {
                    retVal += string.Format("\n {0}", mystring);
                }
            }
            return retVal;
        }

        static readonly string[] SizeSuffixes = 
                   { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
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

        //public override string ToString()

        public string SimsPackagetoString(){

            //https://regex101.com/r/0VWSR7/1
            //https://regex101.com/r/9MiSh9/1
            string complete = "";
            if (!string.IsNullOrEmpty(this.PackageName)){
                complete = string.Format("Package: {0}", this.PackageName);
            }
            if (!string.IsNullOrEmpty(this.Title)){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n Title: {0}", this.Title);
                } else {
                    complete = string.Format("Title: {0}", this.Title);
                }
            }
            if (!string.IsNullOrEmpty(this.Description)){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n Description: {0}", this.Description);
                } else {
                    complete = string.Format("Description: {0}", this.Description);
                }
            }
            if (!string.IsNullOrEmpty(this.Location)){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n Location: {0}", this.Location);
                } else {
                    complete = string.Format("Location: {0}", this.Location);
                }
            }
            if (this.FileSize != 0){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n File Size: {0}", SizeSuffix(this.FileSize));
                } else {
                    complete = string.Format("File Size: {0}", SizeSuffix(this.FileSize));
                }
            }
            if (!string.IsNullOrEmpty(this.GameString)){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n Game: {0}", this.GameString);
                } else {
                    complete = string.Format("Game: {0}", this.GameString);
                }
            }
            if (!string.IsNullOrEmpty(this.Type)){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n Type: {0}", this.Type);
                } else {
                    complete = string.Format("Type: {0}", this.Type);
                }
            }
            if (!string.IsNullOrEmpty(this.Function)){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n Function: {0}", this.Function);
                } else {
                    complete = string.Format("Function: {0}", this.Function);
                }
            }
            if (!string.IsNullOrEmpty(this.FunctionSubcategory)){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n Function Subcategory: {0}", this.FunctionSubcategory);
                } else {
                    complete = string.Format("Function Subcategory: {0}", this.FunctionSubcategory);
                }
            }
            if (this.InstanceIDs.Any()){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n Instance IDs: {0}", GetFormatListString(this.InstanceIDs));
                } else {
                    complete = string.Format("Instance IDs: {0}", GetFormatListString(this.InstanceIDs));
                }
            }
            if (this.GUIDs.Any()){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n GUIDs: {0}", GetFormatListString(this.GUIDs));
                } else {
                    complete = string.Format("GUIDs: {0}", GetFormatListString(this.GUIDs));
                }
            }
            if (!string.IsNullOrEmpty(this.Creator)){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n Creator: {0}", this.Creator);
                } else {
                    complete = string.Format("Creator: {0}", this.Creator);
                }
            }
            if (!string.IsNullOrEmpty(this.Gender)){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n Gender: {0}", this.Gender);
                } else {
                    complete = string.Format("Gender: {0}", this.Gender);
                }
            }
            if (!string.IsNullOrEmpty(this.Age)){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n Age: {0}", this.Age);
                } else {
                    complete = string.Format("Age: {0}", this.Age);
                }
            }
            if (this.RoomSort.Any()){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n Rooms: ");
                    string eps = "";
                    foreach (string ep in this.RoomSort){
                        if (string.IsNullOrEmpty(eps)){
                            eps = string.Format("{0}", ep);
                        } else {
                            eps += string.Format(", {0}", ep);
                        }
                    }
                    complete += eps;
                } else {
                    complete = string.Format("Rooms: ");
                    string eps = "";
                    foreach (string ep in this.RoomSort){
                        if (string.IsNullOrEmpty(eps)){
                            eps = string.Format("{0}", ep);
                        } else {
                            eps += string.Format(", {0}", ep);
                        }
                    }
                    complete += eps;
                }                
            }
            if (this.RequiredEPs.Any()){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n Required EPs: ");
                    string eps = "";
                    foreach (string ep in this.RequiredEPs){
                        if (string.IsNullOrEmpty(eps)){
                            eps = string.Format("{0}", ep);
                        } else {
                            eps += string.Format(", {0}", ep);
                        }
                    }
                    complete += eps;
                } else {
                    complete = string.Format("Required EPs:");
                    string eps = "";
                    foreach (string ep in this.RequiredEPs){
                        if (string.IsNullOrEmpty(eps)){
                            eps = string.Format("{0}", ep);
                        } else {
                            eps += string.Format(", {0}", ep);
                        }
                    }
                    complete += eps;
                }
            }
            if (this.FileHas.Any()){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n File Has: ");
                    string eps = "";
                    foreach (fileHasList ep in this.FileHas){
                        if (string.IsNullOrEmpty(eps)){
                            eps = string.Format("{0} at {1}", ep.TypeID, ep.Location);
                        } else {
                            eps += string.Format("\n {0} at {1}", ep.TypeID, ep.Location);
                        }
                    }
                    complete += eps;
                } else {
                    complete = string.Format("File Has:");
                    string eps = "";
                    foreach (fileHasList ep in this.FileHas){
                        if (string.IsNullOrEmpty(eps)){
                            eps = string.Format("{0} at {1}", ep.TypeID, ep.Location);
                        } else {
                            eps += string.Format("\n {0} at {1}", ep.TypeID, ep.Location);
                        }
                    }
                    complete += eps;
                }
            }
            if (this.Flags.Any()){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n Flags: ");
                    string eps = "";
                    foreach (string ep in this.Flags){
                        if (string.IsNullOrEmpty(eps)){
                            eps = string.Format("{0}", ep);
                        } else {
                            eps += string.Format(", {0}", ep);
                        }
                    }
                    complete += eps;
                } else {
                    complete = string.Format("Flags:");
                    string eps = "";
                    foreach (string ep in this.Flags){
                        if (string.IsNullOrEmpty(eps)){
                            eps = string.Format("{0}", ep);
                        } else {
                            eps += string.Format(", {0}", ep);
                        }
                    }
                    complete += eps;
                }
            }
            if (this.CatalogTags.Any()){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n Tags: ");
                    string eps = "";
                    foreach (TagsList ep in this.CatalogTags){
                        if (string.IsNullOrEmpty(eps)){
                            eps = string.Format(" - {0}: {1}", ep.TypeID, ep.Description);
                        } else {
                            eps += string.Format("\n - {0}: {1}", ep.TypeID, ep.Description);
                        }
                    }
                    complete += eps;
                } else {
                    complete = string.Format("Tags:");
                    string eps = "";
                    foreach (TagsList ep in this.CatalogTags){
                        if (string.IsNullOrEmpty(eps)){
                            eps = string.Format(" - {0}: {1}", ep.TypeID, ep.Description);
                        } else {
                            eps += string.Format("\n - {0}: {1}", ep.TypeID, ep.Description);
                        }
                    }
                    complete += eps;
                }
            }
            if (this.Override == true){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n This package is an override.");
                    if (this.OverriddenInstances.Any()){
                        complete += string.Format("\n Overridden Instances: ");
                        string ov = "";
                        foreach (string instance in this.OverriddenInstances){
                            if (string.IsNullOrEmpty(ov)){
                                ov = string.Format("{0}", instance);
                            } else {
                                ov += string.Format("\n {0}", instance);
                            }
                        }
                        complete += ov;
                    }
                    if (this.OverriddenItems.Any()){
                        complete += string.Format("\n Overridden Items: ");
                        string ov = "";
                        foreach (string instance in this.OverriddenItems){
                            if (string.IsNullOrEmpty(ov)){
                                ov = string.Format("{0}", instance);
                            } else {
                                ov += string.Format("\n {0}", instance);
                            }
                        }
                        complete += ov;
                    }
                } else {
                    complete = string.Format("This package is an override.");
                }
            }
            if (this.Mesh == true && this.Recolor == false){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n This package has a mesh but no accompanying recolor.");                    
                } else {
                    complete = string.Format("This package has a mesh but no accompanying recolor.");
                }
            }
            if (this.Mesh == false && this.Recolor == true){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n This package is a recolor with no mesh.");                    
                } else {
                    complete = string.Format("This package is a recolor with no mesh.");
                }
            }
            if (this.Mesh == true && this.Recolor == true){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n This package has both a mesh and its recolor.");                    
                } else {
                    complete = string.Format("This package has both a mesh and its recolor.");
                }
            }
            if (this.Orphan == true){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n This package is an orphan.");                    
                } else {
                    complete = string.Format("This package is an orphan.");
                }
            }
            if (!string.IsNullOrEmpty(this.MatchingMesh)){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n The matching mesh for this file is in : {0}.", this.MatchingMesh);                    
                } else {
                    complete = string.Format("The matching mesh for this file is in : {0}.", this.MatchingMesh);
                }
            }
            if (this.MatchingRecolors.Any()){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n Matching recolors for this package can be found in: ");
                    string ov = "";
                    foreach (string instance in this.MatchingRecolors){
                        if (!string.IsNullOrEmpty(ov)){
                            ov += string.Format("{0}", instance);
                        } else {
                            ov = string.Format("\n {0}", instance);
                        }
                    }
                    complete += ov;
                } else {
                    complete += string.Format("Matching recolors for this package can be found in: ");
                    string ov = "";
                    foreach (string instance in this.MatchingRecolors){
                        if (!string.IsNullOrEmpty(ov)){
                            ov += string.Format("{0}", instance);
                        } else {
                            ov = string.Format("\n {0}", instance);
                        }
                    }
                    complete += ov;
                }
            }
            if (this.MatchingConflicts.Any()){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n This file conflicts with: ");
                    string ov = "";
                    foreach (string instance in this.MatchingConflicts){
                        if (!string.IsNullOrEmpty(ov)){
                            ov += string.Format("{0}", instance);
                        } else {
                            ov = string.Format("\n {0}", instance);
                        }
                    }
                    complete += ov;
                } else {
                    complete += string.Format("This file conflicts with: ");
                    string ov = "";
                    foreach (string instance in this.MatchingConflicts){
                        if (!string.IsNullOrEmpty(ov)){
                            ov += string.Format("{0}", instance);
                        } else {
                            ov = string.Format("\n {0}", instance);
                        }
                    }
                    complete += ov;
                }
            }            
            return complete;
        }   
    }

    

    [Table("SP_TypeCounter")]
    public class TypeCounter {
        /// <summary>
        /// Listing of the types within each package (eg; 2 GEOM, 3 CASP, 6 STR).
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id {get; set;}
        [Column("Type")]
        public string Type {get; set;}
        [Column("Count")]
        public int Count {get; set;}    
        [ForeignKey(typeof(SimsPackage))]
        public int PackageID {get; set;}
        [Column("SimsPackage")]
        [ManyToOne]
        public SimsPackage SimsPackage {get; set;}
    }

    [Table("SP_EntryList")]
    public class fileHasList {
        /// <summary>
        /// The more rudamentary version, with each entry's location attached. 
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id {get; set;}
        [Column("TypeID")]
        public string TypeID {get; set;}
        [Column("Location")]
        public int Location {get; set;}
        [ForeignKey(typeof(SimsPackage))]
        public int PackageID {get; set;}
        [Column("SimsPackage")]
        [ManyToOne]
        public SimsPackage SimsPackage {get; set;}
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

    [Table("SP_AgeGenderFlags")]
    public class AgeGenderFlags {
        /// <summary>
        /// Age and Gender flags for Sims 4 CAS items.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id {get; set;}
        [Column("Adult")]
        public bool Adult {get; set;}
        [Column("Baby")]
        public bool Baby {get; set;}
        [Column("Child")]
        public bool Child {get; set;}
        [Column("Elder")]
        public bool Elder {get; set;}
        [Column("Infant")]
        public bool Infant {get; set;}
        [Column("Teen")]
        public bool Teen {get; set;}
        [Column("Toddler")]
        public bool Toddler {get; set;}
        [Column("Young Adult")]
        public bool YoungAdult {get; set;}
        [Column("Female")]
        public bool Female {get; set;}
        [Column("Male")]
        public bool Male {get; set;}
        [ForeignKey(typeof(SimsPackage))]
        public int PackageID {get; set;}
        [Column("SimsPackage")]
        [OneToOne]
        public SimsPackage SimsPackage {get; set;}

        public bool Any(){
            if (this.Adult == true || this.Baby == true || this.Infant == true || this.Child == true || this.Elder == true || this.Teen == true || this.YoungAdult == true || this.Female == true || this.Male == true){
                return true;
            } else {
                return false;
            }
        }
    } 
    
    public class Containers {
        /// <summary>
        /// No longer used. Any implementations will be swapped to database.
        /// </summary>
        public static SynchronizedCollection<PackageFile> packageFiles = new SynchronizedCollection<PackageFile>();
        public static SynchronizedCollection<SimsPackage> allSimsPackages = new SynchronizedCollection<SimsPackage>();
        public static SynchronizedCollection<PackageFile> identifiedPackages = new SynchronizedCollection<PackageFile>();
    }

    public class FunctionListing {
        /// <summary>
        /// Used for getting the Sims 4 CAS part location. 
        /// </summary>
        public uint bodytype;
        public string Function;
        public string Subfunction;
    }

    public class InitializedLists {
        /// <summary>
        /// Initializes the lists used within the program. Will be swapped to a database.
        /// </summary>
        public static List<FunctionListing> S4BodyTypes = new List<FunctionListing>();
        public static List<FunctionListing> S4BB = new List<FunctionListing>();
        public static void InitializeLists(){
            S4BodyTypes.Add(new FunctionListing{ bodytype = 655360, Function = "Accessory", Subfunction = "Earring" });
            S4BodyTypes.Add(new FunctionListing{ bodytype = 786432, Function = "Accessory", Subfunction = "Necklace" });
            S4BodyTypes.Add(new FunctionListing{ bodytype = 917504, Function = "Accessory", Subfunction = "Bracelet" });
            S4BodyTypes.Add(new FunctionListing{ bodytype = 131072, Function = "Hair" });
            S4BodyTypes.Add(new FunctionListing{ bodytype = 458752, Function = "Clothing", Subfunction = "Bottom" });
            S4BodyTypes.Add(new FunctionListing{ bodytype = 720896, Function = "Accessory", Subfunction = "Glasses" });
            S4BodyTypes.Add(new FunctionListing{ bodytype = 393228, Function = "Clothing", Subfunction = "Top" });
            S4BodyTypes.Add(new FunctionListing{ bodytype = 393216, Function = "Clothing", Subfunction = "Top" });
            S4BodyTypes.Add(new FunctionListing{ bodytype = 851980, Function = "Accessory", Subfunction = "Gloves" });
            S4BodyTypes.Add(new FunctionListing{ bodytype = 65536, Function = "Accessory", Subfunction = "Hat" });
            S4BodyTypes.Add(new FunctionListing{ bodytype = 327680, Function = "Clothing", Subfunction = "Full Body" });
            S4BodyTypes.Add(new FunctionListing{ bodytype = 524288, Function = "Clothing", Subfunction = "Shoes" });
            S4BodyTypes.Add(new FunctionListing{ bodytype = 2359296, Function = "Accessory", Subfunction = "Socks" });
            S4BodyTypes.Add(new FunctionListing{ bodytype = 240, Function = "Accessory", Subfunction = "Hat" });
            S4BodyTypes.Add(new FunctionListing{ bodytype = 2162688, Function = "Face Paint"});
            S4BodyTypes.Add(new FunctionListing{ bodytype = 65536, Function = "Accessory", Subfunction = "Hat" });
            S4BodyTypes.Add(new FunctionListing{ bodytype = 131149, Function = "Hair"});
            S4BodyTypes.Add(new FunctionListing{ bodytype = 65536, Function = "Accessory", Subfunction = "Hat" });
            S4BodyTypes.Add(new FunctionListing{ bodytype = 1441792, Function = "Accessory", Subfunction = "Index Finger (L)" });
            S4BodyTypes.Add(new FunctionListing{ bodytype = 1572864, Function = "Accessory", Subfunction = "Ring Finger (L)" });
            S4BodyTypes.Add(new FunctionListing{ bodytype = 1245184, Function = "Accessory", Subfunction = "Nose Ring (R)" });
            S4BodyTypes.Add(new FunctionListing{ bodytype = 3342336, Function = "Tattoo", Subfunction = "Lower Back" });


            //S4BB.Add(new FunctionListing{ bodytype = 65536, Function = "Accessory", Subfunction = "Hat" });

        }
    }

    public class BatchTasks {
        public List<Task> Batches {get; set;}

        public BatchTasks(){
            Batches = new List<Task>();
        }
    }

    public class BatchPackages {
        public List<PackageFile> Batches {get; set;}

        public BatchPackages(){
            Batches = new List<PackageFile>();
        }
    }


}