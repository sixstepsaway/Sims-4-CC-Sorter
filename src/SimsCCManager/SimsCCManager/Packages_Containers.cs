using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
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
using System.IO.Packaging;
using SSAGlobals;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
        [PrimaryKey]
        public string Name {get; set;}
        public string Status {get; set;}
    }
    [Table("Processing_Reader")]
    public class PackageFile { //A basic summary of a package file.
        /// <summary>
        /// Basic package file with no full information yet; game version, whether it's broken, and whether it's being processed or waiting to be processed.
        /// </summary>
        [Indexed, PrimaryKey]
        [Column("Name")]
        public string Name {get; set;}
        [Column("Location")]
        public string Location {get; set;}
        [Column("Game")]
        public int Game {get; set;}
        [Column("Broken")]
        public bool Broken {get; set;}
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
        [Column("Function")]
        public string Function {get; set;}
        [Column("Subfunction")]
        public string Subfunction {get; set;}
        [Column("TypeID")]
        public string TypeID {get; set;}
        
        [ForeignKey(typeof(SimsPackage))]
        public string PackageID {get; set;}
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
        [Column ("Title")]
        public string Title {get; set;}  
        [Column ("Description")]
        public string Description {get; set;}
        [Column ("Location")]
        public string Location {get; set;}
        [Column("FileSize")]
        public int FileSize {get; set;}
        [Column ("PackageName")]
        [Indexed, PrimaryKey]
        public string PackageName {get; set;}
        [Column ("Type")]
        public string Type {get; set;}        
        [Column ("Subtype")]
        public string Subtype {get; set;}
        [Column ("Game")]
        public int Game {get; set;}
        [Column ("GameString")]
        public string GameString {get; set;}
        [Column ("Category")]
        public string Category {get; set;}
        [Column ("ModelName")]
        public string ModelName {get; set;}
        [Column("TuningID")]
        public int TuningID {get; set;}
        [Column("Tuning")]
        public string Tuning {get; set;}
        [Column ("Creator")]
        public string Creator {get; set;}
        [Column ("Age")]
        public string Age {get; set;}
        [Column ("Gender")]
        public string Gender {get; set;}             
        [Column ("Function")]
        public string Function {get; set;}        
        [Column ("FunctionSubcategory")]
        public string FunctionSubcategory {get; set;}        
        [Column ("Broken")]
        public bool Broken {get; set;}
        [Column ("Mesh")]
        public bool Mesh {get; set;}
        [Column ("Recolor")]
        public bool Recolor {get; set;}
        [Column ("Orphan")]
        public bool Orphan {get; set;}
        [Column ("Duplicate")]
        public bool Duplicate {get; set;}
        [Column ("Override")]        
        public bool Override {get; set;}
        [Column ("Has Conflicts")]        
        public bool HasConflicts {get; set;}
        [Column ("Is Mod")]        
        public bool IsMod {get; set;}
        [Column ("Merged")]        
        public bool Merged {get; set;}
        [Column ("AllowForCASRandom")]        
        public bool AllowForCASRandom {get; set;}
        [Ignore]        
        public bool NoMesh {get; set;}
        [Ignore]
        public ImageSource Thumbnail {get; set;}
        [Column("MatchingMesh")]
        public string MatchingMesh {get; set;}
        [Column ("InstanceIDs")]
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<PackageInstance> InstanceIDs {get; set;}
        [Column("Thumbnail")]
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<PackageThumbnail> ThumbnailImage {get; set;}
        [Column ("GUIDs")]
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<PackageGUID> GUIDs {get; set;}
        [Column ("RequiredEPs")]
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<PackageRequiredEPs> RequiredEPs {get; set;}
        [Column ("AgeGenderFlags")]
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public AgeGenderFlags AgeGenderFlags {get; set;}
        [Column ("Entry Locations")]
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<PackageEntries> FileHas {get; set;}
        [Column ("RoomSort")]        
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<PackageRoomSort> RoomSort {get; set;}
        [Column ("Components")]
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<PackageComponent> Components {get; set;}
        [Column ("Entries")]
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<PackageTypeCounter> Entries {get; set;}        
        [Column ("Flags")]
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<PackageFlag> Flags {get; set;}
        [Column ("CatalogTags")]
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<TagsList> CatalogTags {get; set;}
        [Column("OverridesList")]
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<OverriddenList> OverridesList {get; set;}
        [Column("MeshKeys")]
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<PackageMeshKeys> MeshKeys {get; set;}
        [Column("CASPartKeys")]
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<PackageCASPartKeys> CASPartKeys {get; set;}
        [Column("OBJDKeys")]
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<PackageOBJDKeys> OBJDPartKeys {get; set;}        
        [Column ("MatchingRecolors")]
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<PackageMatchingRecolors> MatchingRecolors {get; set;}
        [Column ("Conflicts")]
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<PackageConflicts> Conflicts {get; set;}
        [Column ("DuplicatePackages")]
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<PackageDuplicates> DuplicatePackages {get; set;}
        

        public SimsPackage() {
            this.InstanceIDs = new List<PackageInstance>();
            this.GUIDs = new List<PackageGUID>();
            this.RequiredEPs = new List<PackageRequiredEPs>();
            this.RoomSort = new List<PackageRoomSort>();
            this.MatchingRecolors = new List<PackageMatchingRecolors>();
            this.Components = new List<PackageComponent>();
            this.Conflicts = new List<PackageConflicts>();
            this.Entries = new List<PackageTypeCounter>();
            this.FileHas = new List<PackageEntries>();
            this.Flags = new List<PackageFlag>();
            this.CatalogTags = new List<TagsList>();
            this.AgeGenderFlags = new AgeGenderFlags();
            this.OverridesList = new List<OverriddenList>();
            this.MeshKeys = new List<PackageMeshKeys>();
            this.CASPartKeys = new List<PackageCASPartKeys>();
            this.OBJDPartKeys = new List<PackageOBJDKeys>();
            this.DuplicatePackages = new List<PackageDuplicates>();
            this.ThumbnailImage = new List<PackageThumbnail>();
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
            if (this.InstanceIDs.Any() && this.InstanceIDs.Count >= 0){
                var iids = this.InstanceIDs.Select(i => i.InstanceID);
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n Instance IDs:");
                } else {
                    complete = string.Format("Instance IDs:");
                }
                foreach (string id in iids){
                    if (!string.IsNullOrEmpty(complete)){
                        complete += string.Format(", {0}", id);
                    } else {
                        complete = string.Format(" {0}", id);
                    }
                }                
            }
            if (this.GUIDs.Any() && this.GUIDs.Count >= 0){
                var gids = this.GUIDs.Select(i => i.GuidID);
                string guid2 = "";
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n GUIDs:");
                } else {
                    complete = string.Format("GUIDs:");
                }
                foreach (string id in gids){
                    if (!string.IsNullOrEmpty(guid2)){
                        guid2 += string.Format(", {0}", id);
                    } else {
                        guid2 = string.Format(" {0}", id);
                    }
                }  

                complete += string.Format(" {0}", guid2);
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
                    foreach (PackageRoomSort ep in this.RoomSort){
                        if (string.IsNullOrEmpty(eps)){
                            eps = string.Format("{0}", ep.RoomSort);
                        } else {
                            eps += string.Format(", {0}", ep.RoomSort);
                        }
                    }
                    complete += eps;
                } else {
                    complete = string.Format("Rooms: ");
                    string eps = "";
                    foreach (PackageRoomSort ep in this.RoomSort){
                        if (string.IsNullOrEmpty(eps)){
                            eps = string.Format("{0}", ep.RoomSort);
                        } else {
                            eps += string.Format(", {0}", ep.RoomSort);
                        }
                    }
                    complete += eps;
                }                
            }
            if (this.RequiredEPs.Any()){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n Required EPs: ");
                    string eps = "";
                    foreach (PackageRequiredEPs ep in this.RequiredEPs){
                        if (string.IsNullOrEmpty(eps)){
                            eps = string.Format("{0}", ep.RequiredPack);
                        } else {
                            eps += string.Format(", {0}", ep.RequiredPack);
                        }
                    }
                    complete += eps;
                } else {
                    complete = string.Format("Required EPs:");
                    string eps = "";
                    foreach (PackageRequiredEPs ep in this.RequiredEPs){
                        if (string.IsNullOrEmpty(eps)){
                            eps = string.Format("{0}", ep.RequiredPack);
                        } else {
                            eps += string.Format(", {0}", ep.RequiredPack);
                        }
                    }
                    complete += eps;
                }
            }
            if (this.FileHas.Any()){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n File Has: ");
                    string eps = "";
                    foreach (PackageEntries ep in this.FileHas){
                        if (string.IsNullOrEmpty(eps)){
                            eps = string.Format("{0} ({1}) at {2}", ep.Name, ep.TypeID, ep.Location);
                        } else {
                            eps += string.Format("\n {0} ({1}) at {2}", ep.Name, ep.TypeID, ep.Location);
                        }
                    }
                    complete += eps;
                } else {
                    complete = string.Format("File Has:");
                    string eps = "";
                    foreach (PackageEntries ep in this.FileHas){
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
                    foreach (PackageFlag ep in this.Flags){
                        if (string.IsNullOrEmpty(eps)){
                            eps = string.Format("{0}", ep.Flag);
                        } else {
                            eps += string.Format(", {0}", ep.Flag);
                        }
                    }
                    complete += eps;
                } else {
                    complete = string.Format("Flags:");
                    string eps = "";
                    foreach (PackageFlag ep in this.Flags){
                        if (string.IsNullOrEmpty(eps)){
                            eps = string.Format("{0}", ep.Flag);
                        } else {
                            eps += string.Format(", {0}", ep.Flag);
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
                            eps = string.Format("\n - {0}: {1}", ep.TypeID, ep.Description);
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
                            eps = string.Format("\n - {0}: {1}", ep.TypeID, ep.Description);
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
                    if (this.OverridesList.Any()){
                        complete += string.Format("\n Overridden Instances and Items: ");
                        string ov = "";
                        foreach (OverriddenList instance in this.OverridesList){
                            if (string.IsNullOrEmpty(ov)){
                                ov = string.Format("InstanceID: {0}, Item: {1}", instance.InstanceID, instance.Name);
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
            } else {
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n This package is not orphan.");                    
                } else {
                    complete = string.Format("This package is not orphan.");
                }
            }   

            if (this.MeshKeys.Any()){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n Mesh Keys: ");
                    string eps = "";
                    foreach (PackageMeshKeys ep in this.MeshKeys){
                        if (string.IsNullOrEmpty(eps)){
                            eps = string.Format("{0}", ep.MeshKey);
                        } else {
                            eps += string.Format(", {0}", ep.MeshKey);
                        }
                    }
                    complete += eps;
                } else {
                    complete = string.Format("Mesh Keys: ");
                    string eps = "";
                    foreach (PackageMeshKeys ep in this.MeshKeys){
                        if (string.IsNullOrEmpty(eps)){
                            eps = string.Format("{0}", ep.MeshKey);
                        } else {
                            eps += string.Format(", {0}", ep.MeshKey);
                        }
                    }
                    complete += eps;
                }                
            }
            if (this.CASPartKeys.Any()){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n CAS Part Keys: ");
                    string eps = "";
                    foreach (PackageCASPartKeys ep in this.CASPartKeys){
                        if (string.IsNullOrEmpty(eps)){
                            eps = string.Format("{0}", ep.CASPartKey);
                        } else {
                            eps += string.Format(", {0}", ep.CASPartKey);
                        }
                    }
                    complete += eps;
                } else {
                    complete = string.Format("CAS Part Keys: ");
                    string eps = "";
                    foreach (PackageCASPartKeys ep in this.CASPartKeys){
                        if (string.IsNullOrEmpty(eps)){
                            eps = string.Format("{0}", ep.CASPartKey);
                        } else {
                            eps += string.Format(", {0}", ep.CASPartKey);
                        }
                    }
                    complete += eps;
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
                    foreach (PackageMatchingRecolors instance in this.MatchingRecolors){
                        if (!string.IsNullOrEmpty(ov)){
                            ov += string.Format("{0}", instance.MatchingRecolor);
                        } else {
                            ov = string.Format("\n {0}", instance.MatchingRecolor);
                        }
                    }
                    complete += ov;
                } else {
                    complete += string.Format("Matching recolors for this package can be found in: ");
                    string ov = "";
                    foreach (PackageMatchingRecolors instance in this.MatchingRecolors){
                        if (!string.IsNullOrEmpty(ov)){
                            ov += string.Format("{0}", instance.MatchingRecolor);
                        } else {
                            ov = string.Format("\n {0}", instance.MatchingRecolor);
                        }
                    }
                    complete += ov;
                }
            }
            if (this.Conflicts.Any()){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n This file conflicts with: ");
                    string ov = "";
                    foreach (PackageConflicts instance in this.Conflicts){
                        if (!string.IsNullOrEmpty(ov)){
                            ov += string.Format("{0}", instance.Conflict);
                        } else {
                            ov = string.Format("\n {0}", instance.Conflict);
                        }
                    }
                    complete += ov;
                } else {
                    complete += string.Format("This file conflicts with: ");
                    string ov = "";
                    foreach (PackageConflicts instance in this.Conflicts){
                        if (!string.IsNullOrEmpty(ov)){
                            ov += string.Format("{0}", instance.Conflict);
                        } else {
                            ov = string.Format("\n {0}", instance.Conflict);
                        }
                    }
                    complete += ov;
                }
            }
            if (this.DuplicatePackages.Any()){
                if (!string.IsNullOrEmpty(complete)){
                    complete += string.Format("\n This file is a duplicate of: ");
                    string ov = "";
                    foreach (PackageDuplicates instance in this.DuplicatePackages){
                        if (!string.IsNullOrEmpty(ov)){
                            ov += string.Format("{0}", instance.Duplicate);
                        } else {
                            ov = string.Format("\n {0}", instance.Duplicate);
                        }
                    }
                    complete += ov;
                } else {
                    complete += string.Format("This file conflicts with: ");
                    string ov = "";
                    foreach (PackageDuplicates instance in this.DuplicatePackages){
                        if (!string.IsNullOrEmpty(ov)){
                            ov += string.Format("{0}", instance.Duplicate);
                        } else {
                            ov = string.Format("\n {0}", instance.Duplicate);
                        }
                    }
                    complete += ov;
                }
            }            
            return complete;
        }   
    }    

    public static class ListExtensions {
        private static List<string> correctionsInput = new() { "\'", ";" };
        private static List<string> correctionsOutput = new() { "''", @"\;" };

        private static string FixedString(this string source){


            return source;
        }
        public static List<SimsPackage> ApostropheFix(this List<SimsPackage> source)
        {            
            string name = "";
            string namefix = "";
            List<SimsPackage> names = new();
            foreach (string item in correctionsInput){
                var apostrophes = source.Where(s => s.PackageName.Contains(item)).ToList();
                names.AddRange(apostrophes);
            }

            for (int i = 0; i < names.Count; i++){
                var apostrophe = source.IndexOf(names[i]);
                name = source[apostrophe].PackageName;
                namefix = Methods.FixApostrophesforSQL(name);
                source[apostrophe].PackageName = namefix;
            }
            return source;
        }

        public static List<SimsPackage> ApostropheUnFix(this List<SimsPackage> source)
        {                       
            string name = "";
            string namefix = "";
            List<SimsPackage> names = new();
            foreach (string item in correctionsOutput){
                var apostrophes = source.Where(s => s.PackageName.Contains(item)).ToList();
                names.AddRange(apostrophes);
            }

            for (int i = 0; i < names.Count; i++){
                var apostrophe = source.IndexOf(names[i]);
                name = source[apostrophe].PackageName;
                namefix = Methods.FixApostrophesforSQL(name);
                source[apostrophe].PackageName = namefix;
            }
            return source;
        }


        public static List<AllFiles> ApostropheFix(this List<AllFiles> source)
        {            
            string name = "";
            string namefix = "";
            List<AllFiles> names = new();
            foreach (string item in correctionsInput){
                var apostrophes = source.Where(s => s.Name.Contains(item)).ToList();
                names.AddRange(apostrophes);
            }

            for (int i = 0; i < names.Count; i++){
                var apostrophe = source.IndexOf(names[i]);
                name = source[apostrophe].Name;
                namefix = Methods.FixApostrophesforSQL(name);
                source[apostrophe].Name = namefix;
            }
            return source;
        }

        public static List<AllFiles> ApostropheUnFix(this List<AllFiles> source)
        {                       
            string name = "";
            string namefix = "";
            List<AllFiles> names = new();
            foreach (string item in correctionsOutput){
                var apostrophes = source.Where(s => s.Name.Contains(item)).ToList();
                names.AddRange(apostrophes);
            }

            for (int i = 0; i < names.Count; i++){
                var apostrophe = source.IndexOf(names[i]);
                name = source[apostrophe].Name;
                namefix = Methods.FixApostrophesforSQL(name);
                source[apostrophe].Name = namefix;
            }
            return source;
        }
        public static List<PackageFile> ApostropheFix(this List<PackageFile> source)
        {            
            string name = "";
            string namefix = "";
            List<PackageFile> names = new();
            foreach (string item in correctionsInput){
                var apostrophes = source.Where(s => s.Name.Contains(item)).ToList();
                names.AddRange(apostrophes);
            }

            for (int i = 0; i < names.Count; i++){
                var apostrophe = source.IndexOf(names[i]);
                name = source[apostrophe].Name;
                namefix = Methods.FixApostrophesforSQL(name);
                source[apostrophe].Name = namefix;
            }
            return source;
        }

        public static List<PackageFile> ApostropheUnFix(this List<PackageFile> source)
        {                       
            string name = "";
            string namefix = "";
            List<PackageFile> names = new();
            foreach (string item in correctionsOutput){
                var apostrophes = source.Where(s => s.Name.Contains(item)).ToList();
                names.AddRange(apostrophes);
            }

            for (int i = 0; i < names.Count; i++){
                var apostrophe = source.IndexOf(names[i]);
                name = source[apostrophe].Name;
                namefix = Methods.FixApostrophesforSQL(name);
                source[apostrophe].Name = namefix;
            }
            return source;
        }

        public static List<InstancesRecolorsS2> ApostropheFix(this List<InstancesRecolorsS2> source)
        {            
            string name = "";
            string namefix = "";
            var apostrophes = source.Where(o => o.PackageName.Contains('\'')).ToList();
            for (int i = 0; i < apostrophes.Count; i++){
                var apostrophe = source.IndexOf(apostrophes[i]);
                name = source[apostrophe].PackageName;
                namefix = Methods.FixApostrophesforSQL(name);
                source[apostrophe].PackageName = namefix;
            }
            return source;
        }

        public static List<InstancesRecolorsS2> ApostropheUnFix(this List<InstancesRecolorsS2> source)
        {                       
            string name = "";
            string namefix = "";
            var apostrophes = source.Where(o => o.PackageName.Contains("''")).ToList();
            for (int i = 0; i < apostrophes.Count; i++){
                var apostrophe = source.IndexOf(apostrophes[i]);
                name = source[apostrophe].PackageName;
                namefix = Methods.RestoreApostrophesFromSQL(name);
                source[apostrophe].PackageName = namefix;
            }
            return source;
        }
        
        public static List<InstancesRecolorsS3> ApostropheFix(this List<InstancesRecolorsS3> source)
        {            
            string name = "";
            string namefix = "";
            var apostrophes = source.Where(o => o.PackageName.Contains('\'')).ToList();
            for (int i = 0; i < apostrophes.Count; i++){
                var apostrophe = source.IndexOf(apostrophes[i]);
                name = source[apostrophe].PackageName;
                namefix = Methods.FixApostrophesforSQL(name);
                source[apostrophe].PackageName = namefix;
            }
            return source;
        }

        public static List<InstancesRecolorsS3> ApostropheUnFix(this List<InstancesRecolorsS3> source)
        {                       
            string name = "";
            string namefix = "";
            var apostrophes = source.Where(o => o.PackageName.Contains("''")).ToList();
            for (int i = 0; i < apostrophes.Count; i++){
                var apostrophe = source.IndexOf(apostrophes[i]);
                name = source[apostrophe].PackageName;
                namefix = Methods.RestoreApostrophesFromSQL(name);
                source[apostrophe].PackageName = namefix;
            }
            return source;
        }
        
        public static List<InstancesRecolorsS4> ApostropheFix(this List<InstancesRecolorsS4> source)
        {            
            string name = "";
            string namefix = "";
            var apostrophes = source.Where(o => o.PackageName.Contains('\'')).ToList();
            for (int i = 0; i < apostrophes.Count; i++){
                var apostrophe = source.IndexOf(apostrophes[i]);
                name = source[apostrophe].PackageName;
                namefix = Methods.FixApostrophesforSQL(name);
                source[apostrophe].PackageName = namefix;
            }
            return source;
        }

        public static List<InstancesRecolorsS4> ApostropheUnFix(this List<InstancesRecolorsS4> source)
        {                       
            string name = "";
            string namefix = "";
            var apostrophes = source.Where(o => o.PackageName.Contains("''")).ToList();
            for (int i = 0; i < apostrophes.Count; i++){
                var apostrophe = source.IndexOf(apostrophes[i]);
                name = source[apostrophe].PackageName;
                namefix = Methods.RestoreApostrophesFromSQL(name);
                source[apostrophe].PackageName = namefix;
            }
            return source;
        }
        
        public static List<InstancesMeshesS2> ApostropheFix(this List<InstancesMeshesS2> source)
        {            
            string name = "";
            string namefix = "";
            var apostrophes = source.Where(o => o.PackageName.Contains('\'')).ToList();
            for (int i = 0; i < apostrophes.Count; i++){
                var apostrophe = source.IndexOf(apostrophes[i]);
                name = source[apostrophe].PackageName;
                namefix = Methods.FixApostrophesforSQL(name);
                source[apostrophe].PackageName = namefix;
            }
            return source;
        }

        public static List<InstancesMeshesS2> ApostropheUnFix(this List<InstancesMeshesS2> source)
        {                       
            string name = "";
            string namefix = "";
            var apostrophes = source.Where(o => o.PackageName.Contains("''")).ToList();
            for (int i = 0; i < apostrophes.Count; i++){
                var apostrophe = source.IndexOf(apostrophes[i]);
                name = source[apostrophe].PackageName;
                namefix = Methods.RestoreApostrophesFromSQL(name);
                source[apostrophe].PackageName = namefix;
            }
            return source;
        }
        
        public static List<InstancesMeshesS3> ApostropheFix(this List<InstancesMeshesS3> source)
        {            
            string name = "";
            string namefix = "";
            var apostrophes = source.Where(o => o.PackageName.Contains('\'')).ToList();
            for (int i = 0; i < apostrophes.Count; i++){
                var apostrophe = source.IndexOf(apostrophes[i]);
                name = source[apostrophe].PackageName;
                namefix = Methods.FixApostrophesforSQL(name);
                source[apostrophe].PackageName = namefix;
            }
            return source;
        }

        public static List<InstancesMeshesS3> ApostropheUnFix(this List<InstancesMeshesS3> source)
        {                       
            string name = "";
            string namefix = "";
            var apostrophes = source.Where(o => o.PackageName.Contains("''")).ToList();
            for (int i = 0; i < apostrophes.Count; i++){
                var apostrophe = source.IndexOf(apostrophes[i]);
                name = source[apostrophe].PackageName;
                namefix = Methods.RestoreApostrophesFromSQL(name);
                source[apostrophe].PackageName = namefix;
            }
            return source;
        }
        
        public static List<InstancesMeshesS4> ApostropheFix(this List<InstancesMeshesS4> source)
        {            
            string name = "";
            string namefix = "";
            var apostrophes = source.Where(o => o.PackageName.Contains('\'')).ToList();
            for (int i = 0; i < apostrophes.Count; i++){
                var apostrophe = source.IndexOf(apostrophes[i]);
                name = source[apostrophe].PackageName;
                namefix = Methods.FixApostrophesforSQL(name);
                source[apostrophe].PackageName = namefix;
            }
            return source;
        }

        public static List<InstancesMeshesS4> ApostropheUnFix(this List<InstancesMeshesS4> source)
        {                       
            string name = "";
            string namefix = "";
            var apostrophes = source.Where(o => o.PackageName.Contains("''")).ToList();
            for (int i = 0; i < apostrophes.Count; i++){
                var apostrophe = source.IndexOf(apostrophes[i]);
                name = source[apostrophe].PackageName;
                namefix = Methods.RestoreApostrophesFromSQL(name);
                source[apostrophe].PackageName = namefix;
            }
            return source;
        }
        
        public static ObservableCollection<SimsPackage> ApostropheFix(this ObservableCollection<SimsPackage> source)
        {            
            string name = "";
            string namefix = "";
            var apostrophes = source.Where(o => o.PackageName.Contains('\'')).ToList();
            for (int i = 0; i < apostrophes.Count; i++){
                var apostrophe = source.IndexOf(apostrophes[i]);
                name = source[apostrophe].PackageName;
                namefix = Methods.FixApostrophesforSQL(name);
                source[apostrophe].PackageName = namefix;
            }
            return source;
        }

        public static ObservableCollection<SimsPackage> ApostropheUnFix(this ObservableCollection<SimsPackage> source)
        {                       
            string name = "";
            string namefix = "";
            var apostrophes = source.Where(o => o.PackageName.Contains("''")).ToList();
            for (int i = 0; i < apostrophes.Count; i++){
                var apostrophe = source.IndexOf(apostrophes[i]);
                name = source[apostrophe].PackageName;
                namefix = Methods.RestoreApostrophesFromSQL(name);
                source[apostrophe].PackageName = namefix;
            }
            return source;
        }


        public static ObservableCollection<AllFiles> ApostropheFix(this ObservableCollection<AllFiles> source)
        {            
            string name = "";
            string namefix = "";
            var apostrophes = source.Where(o => o.Name.Contains('\'')).ToList();
            for (int i = 0; i < apostrophes.Count; i++){
                var apostrophe = source.IndexOf(apostrophes[i]);
                name = source[apostrophe].Name;
                namefix = Methods.FixApostrophesforSQL(name);
                source[apostrophe].Name = namefix;
            }
            return source;
        }

        public static ObservableCollection<AllFiles> ApostropheUnFix(this ObservableCollection<AllFiles> source)
        {                       
            string name = "";
            string namefix = "";
            var apostrophes = source.Where(o => o.Name.Contains("''")).ToList();
            for (int i = 0; i < apostrophes.Count; i++){
                var apostrophe = source.IndexOf(apostrophes[i]);
                name = source[apostrophe].Name;
                namefix = Methods.RestoreApostrophesFromSQL(name);
                source[apostrophe].Name = namefix;
            }
            return source;
        }
        public static ObservableCollection<PackageFile> ApostropheFix(this ObservableCollection<PackageFile> source)
        {            
            string name = "";
            string namefix = "";
            var apostrophes = source.Where(o => o.Name.Contains('\'')).ToList();
            for (int i = 0; i < apostrophes.Count; i++){
                var apostrophe = source.IndexOf(apostrophes[i]);
                name = source[apostrophe].Name;
                namefix = Methods.FixApostrophesforSQL(name);
                source[apostrophe].Name = namefix;
            }
            return source;
        }

        public static ObservableCollection<PackageFile> ApostropheUnFix(this ObservableCollection<PackageFile> source)
        {                       
            string name = "";
            string namefix = "";
            var apostrophes = source.Where(o => o.Name.Contains("''")).ToList();
            for (int i = 0; i < apostrophes.Count; i++){
                var apostrophe = source.IndexOf(apostrophes[i]);
                name = source[apostrophe].Name;
                namefix = Methods.RestoreApostrophesFromSQL(name);
                source[apostrophe].Name = namefix;
            }
            return source;
        }

        public static ObservableCollection<InstancesRecolorsS2> ApostropheFix(this ObservableCollection<InstancesRecolorsS2> source)
        {            
            string name = "";
            string namefix = "";
            var apostrophes = source.Where(o => o.PackageName.Contains('\'')).ToList();
            for (int i = 0; i < apostrophes.Count; i++){
                var apostrophe = source.IndexOf(apostrophes[i]);
                name = source[apostrophe].PackageName;
                namefix = Methods.FixApostrophesforSQL(name);
                source[apostrophe].PackageName = namefix;
            }
            return source;
        }

        public static ObservableCollection<InstancesRecolorsS2> ApostropheUnFix(this ObservableCollection<InstancesRecolorsS2> source)
        {                       
            string name = "";
            string namefix = "";
            var apostrophes = source.Where(o => o.PackageName.Contains("''")).ToList();
            for (int i = 0; i < apostrophes.Count; i++){
                var apostrophe = source.IndexOf(apostrophes[i]);
                name = source[apostrophe].PackageName;
                namefix = Methods.RestoreApostrophesFromSQL(name);
                source[apostrophe].PackageName = namefix;
            }
            return source;
        }
        
        public static ObservableCollection<InstancesRecolorsS3> ApostropheFix(this ObservableCollection<InstancesRecolorsS3> source)
        {            
            string name = "";
            string namefix = "";
            var apostrophes = source.Where(o => o.PackageName.Contains('\'')).ToList();
            for (int i = 0; i < apostrophes.Count; i++){
                var apostrophe = source.IndexOf(apostrophes[i]);
                name = source[apostrophe].PackageName;
                namefix = Methods.FixApostrophesforSQL(name);
                source[apostrophe].PackageName = namefix;
            }
            return source;
        }

        public static ObservableCollection<InstancesRecolorsS3> ApostropheUnFix(this ObservableCollection<InstancesRecolorsS3> source)
        {                       
            string name = "";
            string namefix = "";
            var apostrophes = source.Where(o => o.PackageName.Contains("''")).ToList();
            for (int i = 0; i < apostrophes.Count; i++){
                var apostrophe = source.IndexOf(apostrophes[i]);
                name = source[apostrophe].PackageName;
                namefix = Methods.RestoreApostrophesFromSQL(name);
                source[apostrophe].PackageName = namefix;
            }
            return source;
        }
        
        public static ObservableCollection<InstancesRecolorsS4> ApostropheFix(this ObservableCollection<InstancesRecolorsS4> source)
        {            
            string name = "";
            string namefix = "";
            var apostrophes = source.Where(o => o.PackageName.Contains('\'')).ToList();
            for (int i = 0; i < apostrophes.Count; i++){
                var apostrophe = source.IndexOf(apostrophes[i]);
                name = source[apostrophe].PackageName;
                namefix = Methods.FixApostrophesforSQL(name);
                source[apostrophe].PackageName = namefix;
            }
            return source;
        }

        public static ObservableCollection<InstancesRecolorsS4> ApostropheUnFix(this ObservableCollection<InstancesRecolorsS4> source)
        {                       
            string name = "";
            string namefix = "";
            var apostrophes = source.Where(o => o.PackageName.Contains("''")).ToList();
            for (int i = 0; i < apostrophes.Count; i++){
                var apostrophe = source.IndexOf(apostrophes[i]);
                name = source[apostrophe].PackageName;
                namefix = Methods.RestoreApostrophesFromSQL(name);
                source[apostrophe].PackageName = namefix;
            }
            return source;
        }
        
        public static ObservableCollection<InstancesMeshesS2> ApostropheFix(this ObservableCollection<InstancesMeshesS2> source)
        {            
            string name = "";
            string namefix = "";
            var apostrophes = source.Where(o => o.PackageName.Contains('\'')).ToList();
            for (int i = 0; i < apostrophes.Count; i++){
                var apostrophe = source.IndexOf(apostrophes[i]);
                name = source[apostrophe].PackageName;
                namefix = Methods.FixApostrophesforSQL(name);
                source[apostrophe].PackageName = namefix;
            }
            return source;
        }

        public static ObservableCollection<InstancesMeshesS2> ApostropheUnFix(this ObservableCollection<InstancesMeshesS2> source)
        {                       
            string name = "";
            string namefix = "";
            var apostrophes = source.Where(o => o.PackageName.Contains("''")).ToList();
            for (int i = 0; i < apostrophes.Count; i++){
                var apostrophe = source.IndexOf(apostrophes[i]);
                name = source[apostrophe].PackageName;
                namefix = Methods.RestoreApostrophesFromSQL(name);
                source[apostrophe].PackageName = namefix;
            }
            return source;
        }
        
        public static ObservableCollection<InstancesMeshesS3> ApostropheFix(this ObservableCollection<InstancesMeshesS3> source)
        {            
            string name = "";
            string namefix = "";
            var apostrophes = source.Where(o => o.PackageName.Contains('\'')).ToList();
            for (int i = 0; i < apostrophes.Count; i++){
                var apostrophe = source.IndexOf(apostrophes[i]);
                name = source[apostrophe].PackageName;
                namefix = Methods.FixApostrophesforSQL(name);
                source[apostrophe].PackageName = namefix;
            }
            return source;
        }

        public static ObservableCollection<InstancesMeshesS3> ApostropheUnFix(this ObservableCollection<InstancesMeshesS3> source)
        {                       
            string name = "";
            string namefix = "";
            var apostrophes = source.Where(o => o.PackageName.Contains("''")).ToList();
            for (int i = 0; i < apostrophes.Count; i++){
                var apostrophe = source.IndexOf(apostrophes[i]);
                name = source[apostrophe].PackageName;
                namefix = Methods.RestoreApostrophesFromSQL(name);
                source[apostrophe].PackageName = namefix;
            }
            return source;
        }
        
        public static ObservableCollection<InstancesMeshesS4> ApostropheFix(this ObservableCollection<InstancesMeshesS4> source)
        {            
            string name = "";
            string namefix = "";
            var apostrophes = source.Where(o => o.PackageName.Contains('\'')).ToList();
            for (int i = 0; i < apostrophes.Count; i++){
                var apostrophe = source.IndexOf(apostrophes[i]);
                name = source[apostrophe].PackageName;
                namefix = Methods.FixApostrophesforSQL(name);
                source[apostrophe].PackageName = namefix;
            }
            return source;
        }

        public static ObservableCollection<InstancesMeshesS4> ApostropheUnFix(this ObservableCollection<InstancesMeshesS4> source)
        {                       
            string name = "";
            string namefix = "";
            var apostrophes = source.Where(o => o.PackageName.Contains("''")).ToList();
            for (int i = 0; i < apostrophes.Count; i++){
                var apostrophe = source.IndexOf(apostrophes[i]);
                name = source[apostrophe].PackageName;
                namefix = Methods.RestoreApostrophesFromSQL(name);
                source[apostrophe].PackageName = namefix;
            }
            return source;
        }








    }

    [Table ("SP_Thumbnails")]
    public class PackageThumbnail {
        [AutoIncrement, PrimaryKey]
        [Column("ID")]
        public int Id {get; set;}
        [Column ("Thumbnail")]
        public string Thumbnail {get; set;}
        [Column ("Type")]
        public string Type {get; set;}
        [Column ("Source")]
        public string Source {get; set;}
        [ForeignKey(typeof(SimsPackage))]
        public string PackageID {get; set;}
        [Column("SimsPackage")]
        [ManyToOne]
        public SimsPackage SimsPackage {get; set;}
    }


    [Table ("SP_Instances")]
    public class PackageInstance {
        [AutoIncrement, PrimaryKey]
        [Column("ID")]
        public int Id {get; set;}
        [Column ("InstanceID")]
        public string InstanceID {get; set;}
        [ForeignKey(typeof(SimsPackage))]
        public string PackageID {get; set;}
        [Column("SimsPackage")]
        [ManyToOne]
        public SimsPackage SimsPackage {get; set;}
    }
    
    [Table ("SP_Guids")]
    public class PackageGUID {
        [AutoIncrement, PrimaryKey]
        [Column("ID")]
        public int Id {get; set;}
        [Column ("GUID ID")]
        public string GuidID {get; set;}
        [ForeignKey(typeof(SimsPackage))]
        public string PackageID {get; set;}
        [Column("SimsPackage")]
        [ManyToOne]
        public SimsPackage SimsPackage {get; set;}
    }
    
    [Table ("SP_RequiredEPs")]
    public class PackageRequiredEPs {
        [AutoIncrement, PrimaryKey]
        [Column("ID")]
        public int Id {get; set;}
        [Column ("Required Pack")]
        public string RequiredPack {get; set;}
        [ForeignKey(typeof(SimsPackage))]
        public string PackageID {get; set;}
        [Column("SimsPackage")]
        [ManyToOne]
        public SimsPackage SimsPackage {get; set;}
    }

    [Table ("SP_MeshKeys")]
    public class PackageMeshKeys {
        [AutoIncrement, PrimaryKey]
        [Column("ID")]
        public int Id {get; set;}
        [Column ("Mesh Key")]
        public string MeshKey {get; set;}
        [ForeignKey(typeof(SimsPackage))]
        public string PackageID {get; set;}
        [Column("SimsPackage")]
        [ManyToOne]
        public SimsPackage SimsPackage {get; set;}
    }

    [Table ("SP_CASPartKeys")]
    public class PackageCASPartKeys {
        [AutoIncrement, PrimaryKey]
        [Column("ID")]
        public int Id {get; set;}
        [Column ("CAS Part Key")]
        public string CASPartKey {get; set;}
        [ForeignKey(typeof(SimsPackage))]
        public string PackageID {get; set;}
        [Column("SimsPackage")]
        [ManyToOne]
        public SimsPackage SimsPackage {get; set;}
    }
    [Table ("SP_OBJDKeys")]
    public class PackageOBJDKeys {
        [AutoIncrement, PrimaryKey]
        [Column("ID")]
        public int Id {get; set;}
        [Column ("Object Key")]
        public string OBJDKey {get; set;}
        [ForeignKey(typeof(SimsPackage))]
        public string PackageID {get; set;}
        [Column("SimsPackage")]
        [ManyToOne]
        public SimsPackage SimsPackage {get; set;}
    }

    [Table ("SP_MatchingRecolors")]
    public class PackageMatchingRecolors {
        [AutoIncrement, PrimaryKey]
        [Column("ID")]
        public int Id {get; set;}
        [Column ("Matching Recolor")]
        public string MatchingRecolor {get; set;}
        [ForeignKey(typeof(SimsPackage))]
        public string PackageID {get; set;}
        [Column("SimsPackage")]
        [ManyToOne]
        public SimsPackage SimsPackage {get; set;}
    }

    [Table ("SP_Conflicts")]
    public class PackageConflicts {
        [AutoIncrement, PrimaryKey]
        [Column("ID")]
        public int Id {get; set;}
        [Column ("Conflicts")]
        public string Conflict {get; set;}
        [ForeignKey(typeof(SimsPackage))]
        public string PackageID {get; set;}
        [Column("SimsPackage")]
        [ManyToOne]
        public SimsPackage SimsPackage {get; set;}
    }
    [Table ("SP_Duplicates")]
    public class PackageDuplicates {
        [AutoIncrement, PrimaryKey]
        [Column("ID")]
        public int Id {get; set;}
        [Column ("Duplicate")]
        public string Duplicate {get; set;}
        [ForeignKey(typeof(SimsPackage))]
        public string PackageID {get; set;}
        [Column("SimsPackage")]
        [ManyToOne]
        public SimsPackage SimsPackage {get; set;}
    }
    [Table ("SP_RoomSort")]
    public class PackageRoomSort {
        [AutoIncrement, PrimaryKey]
        [Column("ID")]
        public int Id {get; set;}
        [Column ("Room Sort")]
        public string RoomSort {get; set;}
        [ForeignKey(typeof(SimsPackage))]
        public string PackageID {get; set;}
        [Column("SimsPackage")]
        [ManyToOne]
        public SimsPackage SimsPackage {get; set;}
    }
    [Table ("SP_Components")]
    public class PackageComponent {
        [AutoIncrement, PrimaryKey]
        [Column("ID")]
        public int Id {get; set;}
        [Column ("Component")]
        public string Component {get; set;}
        [ForeignKey(typeof(SimsPackage))]
        public string PackageID {get; set;}
        [Column("SimsPackage")]
        [ManyToOne]
        public SimsPackage SimsPackage {get; set;}
    }
    [Table ("SP_Flags")]
    public class PackageFlag {
        [AutoIncrement, PrimaryKey]
        [Column("ID")]
        public int Id {get; set;}
        [Column ("Flag")]
        public string Flag {get; set;}
        [ForeignKey(typeof(SimsPackage))]
        public string PackageID {get; set;}
        [Column("SimsPackage")]
        [ManyToOne]
        public SimsPackage SimsPackage {get; set;}
    }

    [Table("SP_Overrides")]
    public class OverriddenList {
        [PrimaryKey, AutoIncrement]
        public int Id {get; set;}
        [Column("InstanceID")]
        public string InstanceID {get; set;}
        [Column("Name")]
        public string Name {get; set;}
        [Column("Type")]
        public string Type {get; set;}
        [Column("Pack")]
        public string Pack {get; set;}
        [Column("Override")]
        public string Override {get; set;}
        [ForeignKey(typeof(SimsPackage))]
        public string PackageID {get; set;}
        [Column("SimsPackage")]
        [ManyToOne]
        public SimsPackage SimsPackage {get; set;}
    }

    [Table("SP_TypeCounter")]
    public class PackageTypeCounter {
        /// <summary>
        /// Listing of the types within each package (eg; 2 GEOM, 3 CASP, 6 STR).
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id {get; set;}
        [Column("Type Description")]
        public string TypeDesc {get; set;}
        [Column("TypeID")]
        public string TypeID {get; set;}
        [Column("Count")]
        public int Count {get; set;}    
        [ForeignKey(typeof(SimsPackage))]
        public string PackageID {get; set;}
        [Column("SimsPackage")]
        [ManyToOne]
        public SimsPackage SimsPackage {get; set;}
    }

    [Table("SP_EntryList")]
    public class PackageEntries {
        /// <summary>
        /// The more rudamentary version, with each entry's location attached. 
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id {get; set;}
        [Column("TypeID")]
        public string TypeID {get; set;}
        [Column("GroupID")]
        public string GroupID {get; set;}
        [Column("InstanceID")]
        public string InstanceID {get; set;}
        [Column("Size")]
        public string Size {get; set;}
        [Column("Name")]
        public string Name {get; set;}
        [Column("Location")]
        public int Location {get; set;}
        [ForeignKey(typeof(SimsPackage))]
        public string PackageID {get; set;}
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
        public string PackageID {get; set;}
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
        [Column("BodyType")]
        public uint bodytype {get; set;}
        [Column("Function")]
        public string Function {get; set;}
        [Column("Subfunction")]
        public string Subfunction {get; set;}
    }

    public class BatchTasks {
        public List<Task> Batches {get; set;}

        public BatchTasks(){
            Batches = new List<Task>();
        }
    }

    public class BatchPackages {
        public List<AllFiles> Batches {get; set;}

        public BatchPackages(){
            Batches = new List<AllFiles>();
        }
    }


    public class BatchSimsPackages {
        public List<SimsPackage> Batch {get; set;}

        public BatchSimsPackages(){
            Batch = new List<SimsPackage>();
        }
    }

    public class SimsPackagesPages {
        public List<SimsPackage> Page {get; set;}

        public SimsPackagesPages(){
            Page = new List<SimsPackage>();
        }
    }

    [Table("TypeList")]
    public class typeList {
        /// <summary>
        /// For "types", for example Cas Parts or Geometry.
        /// </summary>
        [Column("Description")]
        public string desc {get; set;}
        [Column("TypeID")]
        public string typeID {get; set;}
        [Column("Info")]
        public string info {get; set;}
    }

    [Table("Instances")]
    public class OverridesList {
        [AutoIncrement, PrimaryKey]
        [Column("ID")]
        public int Id {get; set;}
        [Column("InstanceID")]
        public string InstanceID {get; set;}
        [Column("Name")]
        public string Name {get; set;}
        [Column("Type")]
        public string Type {get; set;}
        [Column("Pack")]
        public string Pack {get; set;}
    }

    public class SpecificOverrides {
        [Column("Instance")]
        public string Instance {get; set;}
        [Column("Description")]
        public string Description {get; set;}
    }

    [Table("Sims2Meshes")]
    public class InstancesMeshesS2 
    {
        [PrimaryKey]
        [Column("Key")]
        public string Key {get; set;}
        [Column("PackageName")]
        public string PackageName {get; set;}
        [Column("MatchingRecolors")]
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<InstancesRecolorsS2> MatchingRecolors {get; set;}
        public InstancesMeshesS2(){
            MatchingRecolors = new List<InstancesRecolorsS2>();
        }
    };
    [Table("Sims3Meshes")]
    public class InstancesMeshesS3 
    {
        [PrimaryKey]
        [Column("Key")]
        public string Key {get; set;}
        [Column("PackageName")]
        public string PackageName {get; set;}
        [Column("MatchingRecolors")]
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<InstancesRecolorsS3> MatchingRecolors {get; set;}
        public InstancesMeshesS3(){
            MatchingRecolors = new List<InstancesRecolorsS3>();
        }
    };
    [Table("Sims4Meshes")]
    public class InstancesMeshesS4 
    {
        [PrimaryKey]
        [Column("Key")]
        public string Key {get; set;}
        [Column("PackageName")]
        public string PackageName {get; set;}
        [Column("MatchingRecolors")]
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<InstancesRecolorsS4> MatchingRecolors {get; set;}
        public InstancesMeshesS4(){
            MatchingRecolors = new List<InstancesRecolorsS4>();
        }
    };
    [Table("Sims2Recolors")]
    public class InstancesRecolorsS2 {
        [PrimaryKey]
        [Column("Key")]
        public string Key {get; set;}
        [Column("PackageName")]
        public string PackageName {get; set;}
        [Column("MatchingMesh")]
        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public InstancesMeshesS2 MatchingMesh {get; set;}   
        [ForeignKey(typeof(InstancesMeshesS2))]
        public string MeshKey {get; set;}     
    };
    [Table("Sims3Recolors")]
    public class InstancesRecolorsS3 
    {
        [PrimaryKey]
        [Column("Key")]
        public string Key {get; set;}
        [Column("PackageName")]
        public string PackageName {get; set;}
        [Column("MatchingMesh")]
        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public InstancesMeshesS3 MatchingMesh {get; set;} 
        [ForeignKey(typeof(InstancesMeshesS3))]
        public string MeshKey {get; set;}       
    };
    [Table("Sims4Recolors")]
    public class InstancesRecolorsS4 
    {
        [PrimaryKey]
        [Column("Key")]
        public string Key {get; set;}
        [Column("PackageName")]
        public string PackageName {get; set;}
        [Column("MatchingMesh")]
        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public InstancesMeshesS4 MatchingMesh {get; set;}
        [ForeignKey(typeof(InstancesMeshesS4))]
        public string MeshKey {get; set;}
    };
}