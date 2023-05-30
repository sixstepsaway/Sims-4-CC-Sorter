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
            InstanceIDs = new List<string>();
            GUIDs = new List<string>();
            RequiredEPs = new List<string>();
            RoomSort = new List<string>();
            MatchingRecolors = new List<string>();
            Components = new List<string>();
            MatchingConflicts = new List<string>();
            Entries = new List<TypeCounter>();
            FileHas = new List<fileHasList>();
            Flags = new List<string>();
            CatalogTags = new List<TagsList>();
            OverriddenInstances = new List<string>();
            OverriddenItems = new List<string>();
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

        public override string ToString()
        {
            //https://regex101.com/r/0VWSR7/1
            //https://regex101.com/r/9MiSh9/1

            
            return string.Format("Title: {0} \n Description: {1} \n Location: {2} \n PackageName: {3} \n Type: {4} \n Game: {5} \n InstanceIDs: {6} \n Subtype: {7} \n Category: {8} \n Model Name: {9} \n GUID: {10} \n Creator: {11} \n Age: {12} \n Gender: {13} \n Required EPs: {14} \n Function: {15} \n FunctionSubcategory: {16} \n AgeGenderFlags: {17} \n RoomSort: {18} \n Components: {19} \n Entries: {20} \n Flags: {21} \n CatalogTags: {22} \n Broken: {23} \n Mesh: {24} \n Recolor: {25} \n Orphan: {26} \n Override: {27} \n OverriddenInstances: {28} \n OverriddenItems: {29} \n MatchingMesh: {30} \n MatchingRecolors: {31} \n MatchingConflicts: {32}", this.Title, this.Description, this.Location, this.PackageName, this.Type, this.Game, GetFormatListString(this.InstanceIDs), this.Subtype, this.Category, this.ModelName, GetFormatListString(this.GUIDs), this.Creator, this.Age, this.Gender, GetFormatListString(this.RequiredEPs), this.Function, this.FunctionSubcategory, this.AgeGenderFlags, GetFormatListString(this.RoomSort), GetFormatListString(this.Components), GetFormatTypeCounter(this.Entries), GetFormatListString(this.Flags), GetFormatTagsList(this.CatalogTags), this.Broken, this.Mesh, this.Recolor, this.Orphan, this.Override, GetFormatListString(this.OverriddenInstances), GetFormatListString(this.OverriddenItems), this.MatchingMesh, GetFormatListString(this.MatchingRecolors), GetFormatListString(this.MatchingConflicts));
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