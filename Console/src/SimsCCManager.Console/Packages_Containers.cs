using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Text;

namespace SimsCCManager.Packages.Containers
{    
    public class PackageFile { //A basic summary of a package file.
        public string Name {get; set;}
        public string Location {get; set;}
        public int Number {get; set;}        
        public int Game {get; set;}
        public bool Broken {get; set;}
    }

    public class TagsList {
        public string stringval {get; set;}
        public short shortval {get; set;}
    }

    public class ThingHolder<T> : List<T> {
        public string ID {get; set;}
    }

    public class NotPackage {
        public bool notPackage {get; set;}
        public string actualType {get; set;}
    }

    public class SimsPackage { // A more in depth package file.
        
        public string Title {get; set;}        
        public string Description {get; set;}
        public string Location {get; set;}
        public string PackageName {get; set;}
        public string Type {get; set;}
        public int Game {get; set;}
        public string GameVersion {get; set;}
        public string DBPF {get; set;}
        public List<string> InstanceIDs {get; set;}
        public uint Major {get; set;}
        public uint Minor {get; set;}
        public uint DateCreated {get; set;}
        public uint DateModified {get; set;}
        public uint mnFileVersion {get; set;}
        public uint mnUserVersion {get; set;}
        public uint IndexMajorVersion {get; set;}
        public uint mnIndexRecordEntryCount {get; set;}
        public uint mnIndexRecordPositionLow {get; set;}
        public uint mnIndexRecordSize {get; set;}
        public ulong mnIndexRecordPosition {get; set;}
        public uint IndexCount {get; set;}
        public uint IndexOffset {get; set;}
        public uint IndexSize {get; set;}
        public uint HolesCount {get; set;}
        public uint HolesOffset {get; set;}
        public uint HolesSize {get; set;}
        public uint IndexMinorVersion {get; set;}
        public string XMLType {get; set;}
        public string XMLSubtype {get; set;}
        public string XMLCategory {get; set;}
        public string XMLModelName {get; set;}
        public List<string> ObjectGUID {get; set;}
        public string XMLCreator {get; set;}
        public string XMLAge {get; set;}
        public string XMLGender {get; set;}
        public List<string> RequiredEPs {get; set;}
        public string Function {get; set;}
        public string FunctionSubcategory {get; set;}
        public string AGFlag {get; set;}
        public AgeGenderFlags AgeGenderFlags {get; set;}
        public List<string> RoomSort {get; set;}
        public List<string> Components {get; set;}
        public List<TypeCounter> Entries {get; set;}
        public List<string> Flags {get; set;}
        public List<TagsList> CatalogTags {get; set;}
        public List<TagsList> SellingPointTags {get; set;}
        public List<TagsList> StyleTags {get; set;}
        public bool Broken {get; set;}
        public bool Mesh {get; set;}
        public bool Recolor {get; set;}
        public bool Orphan {get; set;}
        public bool Override {get; set;}
        public string MatchingMesh {get; set;}
        public List<string> MatchingRecolors {get; set;}
        public List<string> MatchingConflicts {get; set;}
        public NotPackage NotAPackage {get; set;}

        public SimsPackage() {
            InstanceIDs = new List<string>();
            ObjectGUID = new List<string>();
            RequiredEPs = new List<string>();
            RoomSort = new List<string>();
            MatchingRecolors = new List<string>();
            Components = new List<string>();
            MatchingConflicts = new List<string>();
            Entries = new List<TypeCounter>();
            NotAPackage = new NotPackage();
            Flags = new List<string>();
            CatalogTags = new List<TagsList>();
            StyleTags = new List<TagsList>();
            SellingPointTags = new List<TagsList>();
        }

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
                    retVal +=  tag.shortval.ToString() + ", " + tag.stringval;
                } else {
                    retVal += string.Format(", " + tag.shortval.ToString() + ": " + tag.stringval);
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
            return string.Format("Title : {0} \n Description : {1} \n Location : {2} \n PackageName : {3} \n Type : {4} \n Game : {5} \n GameVersion : {6} \n DBPF : {7} \n InstanceIDs : {8} \n Major : {9} \n Minor : {10} \n DateCreated : {11} \n DateModified : {12} \n mnFileVersion : {13} \n mnUserVersion : {14} \n IndexMajorVersion : {15} \n mnIndexRecordEntryCount : {16} \n mnIndexRecordPositionLow : {17} \n mnIndexRecordSize : {18} \n mnIndexRecordPosition : {19} \n IndexCount : {20} \n IndexOffset : {21} \n IndexSize : {22} \n HolesCount : {23} \n HolesOffset : {24} \n HolesSize : {25} \n IndexMinorVersion : {26} \n XMLType : {27} \n XMLSubtype : {28} \n XMLCategory : {29} \n XMLModelName : {30} \n ObjectGUID : {31} \n XMLCreator : {32} \n XMLAge : {33} \n XMLGender : {34} \n RequiredEPs : {35} \n Function : {36} \n FunctionSubcategory : {37} \n RoomSort : {38} \n Entries : {39} \n Flags: {40} \n CatalogTags : {41} \n SellingPointTags : {42} \n StyleTags : {43} \n Broken : {44} \n Mesh : {45} \n Recolor : {46} \n Orphan : {47} \n MatchingMesh : {48} \n MatchingRecolors : {49} \n MatchingConflicts : {50} \n NotAPackage : {51}", this.Title, this.Description, this.Location, this.PackageName, this.Type, this.Game, this.GameVersion, this.DBPF, GetFormatListString(this.InstanceIDs), this.Major, this.Minor, this.DateCreated, this.DateModified, this.mnFileVersion, this.mnUserVersion, this.IndexMajorVersion, this.mnIndexRecordEntryCount, this.mnIndexRecordPositionLow, this.mnIndexRecordSize, this.mnIndexRecordPosition, this.IndexCount, this.IndexOffset, this.IndexSize, this.HolesCount, this.HolesOffset, this.HolesSize, this.IndexMinorVersion, this.XMLType, this.XMLSubtype, this.XMLCategory, this.XMLModelName, GetFormatListString(this.ObjectGUID), this.XMLCreator, this.XMLAge, this.XMLGender, GetFormatListString(this.RequiredEPs), this.Function, this.FunctionSubcategory, GetFormatListString(this.RoomSort), GetFormatTypeCounter(this.Entries), GetFormatListString(this.Flags),GetFormatTagsList(this.CatalogTags), GetFormatTagsList(this.SellingPointTags), GetFormatTagsList(this.StyleTags), this.Broken, this.Mesh, this.Recolor, this.Orphan, this.MatchingMesh, GetFormatListString(this.MatchingRecolors), GetFormatListString(this.MatchingConflicts), this.NotAPackage);
        }

    }

    public class TypeCounter {
        public string Type;
        public int Count;
    }

    public class fileHasList {
        public string term {get; set;}
        public int location {get; set;}
    }

    public class FunctionSortList {
        public int flagnum {get; set;}
        public int functionsubsortnum {get; set;}
        public string Category {get; set;}
        public string Subcategory {get; set;}
    }   

    public class AgeGenderFlags {
        public bool Adult {get; set;}
        public bool Baby {get; set;}
        public bool Child {get; set;}
        public bool Elder {get; set;}
        public bool Infant {get; set;}
        public bool Teen {get; set;}
        public bool Toddler {get; set;}
        public bool YoungAdult {get; set;}
        public bool Female {get; set;}
        public bool Male {get; set;}
    } 
    
    public class Containers {
        public static SynchronizedCollection<PackageFile> packageFiles = new SynchronizedCollection<PackageFile>();
        public static SynchronizedCollection<SimsPackage> allSims2Packages = new SynchronizedCollection<SimsPackage>();
        public static SynchronizedCollection<SimsPackage> allSims3Packages = new SynchronizedCollection<SimsPackage>();
        public static SynchronizedCollection<SimsPackage> allSims4Packages = new SynchronizedCollection<SimsPackage>();

    }

    public class FunctionListing {
        public uint bodytype;
        public string Function;
        public string Subfunction;
    }

    public class InitializedLists {
        public static List<FunctionListing> S4BodyTypes = new List<FunctionListing>();
        public static List<FunctionListing> S4BB = new List<FunctionListing>();
        public static void InitializeLists(){
            S4BodyTypes.Add(new FunctionListing{ bodytype = 655360, Function = "Accessory", Subfunction = "Earring" });
            S4BodyTypes.Add(new FunctionListing{ bodytype = 786432, Function = "Accessory", Subfunction = "Necklace" });
            S4BodyTypes.Add(new FunctionListing{ bodytype = 917504, Function = "Accessory", Subfunction = "Bracelet" });
            S4BodyTypes.Add(new FunctionListing{ bodytype = 131072, Function = "Hair" });
            S4BodyTypes.Add(new FunctionListing{ bodytype = 458752, Function = "Clothing", Subfunction = "Bottom" });
            S4BodyTypes.Add(new FunctionListing{ bodytype = 720896, Function = "Accessory", Subfunction = "Glasses" });
            S4BodyTypes.Add(new FunctionListing{ bodytype = 393216, Function = "Clothing", Subfunction = "Top" });
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


            //S4BB.Add(new FunctionListing{ bodytype = 65536, Function = "Accessory", Subfunction = "Hat" });

        }
    }

}