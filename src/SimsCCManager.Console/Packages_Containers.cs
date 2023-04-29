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

    public class ThingHolder<T> : List<T> {
        public string ID {get; set;}
    }

    public class SimsPackage { // A more in depth package file.
        
        public string Title {get; set;}        
        public string Description {get; set;}
        public string Location {get; set;}
        public string PackageName {get; set;}
        public string Type {get; set;}
        public int Game {get; set;}
        public string DBPF {get; set;}
        public List<string> InstanceIDs {get; set;}
        public uint Major {get; set;}
        public uint Minor {get; set;}
        public uint DateCreated {get; set;}
        public uint DateModified {get; set;}
        public uint IndexMajorVersion {get; set;}
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
        public List<string> RoomSort {get; set;}
        public List<TypeCounter> Entries {get; set;}
        public bool Mesh {get; set;}
        public bool Recolor {get; set;}
        public bool Orphan {get; set;}

        public SimsPackage() {
            InstanceIDs = new List<string>();
            ObjectGUID = new List<string>();
            RequiredEPs = new List<string>();
            RoomSort = new List<string>();
            Entries = new List<TypeCounter>();
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
            
            return string.Format("Title: {0} \n Description: {1} \n Type: {2} \n InstanceID: {3} \n Location: {4} \n PackageName: {5} \n Game: {6} \n DBPF: {7} \n Major: {8} \n Minor: {9} \n Date Created: {10} \n Date Modified: {11} \n Index Major Version: {12} \n Index Count: {13} \n Index Offset: {14} \n Index Size: {15} \n Holes Count: {16} \n Holes Offset: {17} \n Holes Size: {18} \n Index Minor Version: {19} \n XML Type: {20} \n XML Subtype: {21} \n XML Category: {22} \n XML Model Name: {23} \n Object GUIDs: {24} \n XML Creator: {25} \n XML Age: {26} \n XML Gender: {27} \n Required EPs: {28} \n Function: {29} \n Function Subcategory: {30} \n Room Sort: {31} \n Entries: {32} \n Has Mesh: {33} \n Is Recolor: {34} \n Orphaned: {34}", this.Title, this.Description, this.Type, GetFormatListString(this.InstanceIDs), this.Location, this.PackageName, this.Game, this.DBPF, this.Major, this.Minor, this.DateCreated, this.DateModified, this.IndexMajorVersion, this.IndexCount, this.IndexOffset, this.IndexSize, this.HolesCount, this.HolesOffset, this.HolesSize, this.IndexMinorVersion, this.XMLType, this.XMLSubtype, this.XMLCategory, this.XMLModelName, GetFormatListString(this.ObjectGUID), this.XMLCreator, this.XMLAge, this.XMLGender, GetFormatListString(this.RequiredEPs), this.Function, this.FunctionSubcategory, GetFormatListString(this.RoomSort), GetFormatTypeCounter(this.Entries), this.Mesh, this.Recolor, this.Orphan);
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
    
    public class Containers {
        public static SynchronizedCollection<PackageFile> packageFiles = new SynchronizedCollection<PackageFile>();
        public static SynchronizedCollection<SimsPackage> allSims2Packages = new SynchronizedCollection<SimsPackage>();
        public static SynchronizedCollection<SimsPackage> allSims3Packages = new SynchronizedCollection<SimsPackage>();
        public static SynchronizedCollection<SimsPackage> allSims4Packages = new SynchronizedCollection<SimsPackage>();

    }


}