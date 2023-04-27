using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace SimsCCManager.Packages.Containers
{    
    public class PackageFile { //A basic summary of a package file.
        public string Name {get; set;}
        public string Location {get; set;}
        public int Number {get; set;}        
        public int Game {get; set;}
        public bool Broken {get; set;}
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
        public ArrayList ObjectGUID {get; set;}
        public string XMLCreator {get; set;}
        public string XMLAge {get; set;}
        public string XMLGender {get; set;}
        public string[] RequiredEPs {get; set;}
        public string Function {get; set;}
        public string FunctionSubcategory {get; set;}
        public string[] RoomSort {get; set;}

        public override string ToString()
        {
            return string.Format("Title: {0} \n Description: {1} \n Type: {2} \n InstanceID: {3} \n Location: {4} \n PackageName: {5} \n Game: {6} \n DBPF: {7} \n Major: {8} \n Minor: {9} \n DateCreated: {10} \n DateModified: {11} \n IndexMajorVersion: {12} \n IndexCount: {13} \n IndexOffset: {14} \n IndexSize: {15} \n HolesCount: {16} \n HolesOffset: {17} \n HolesSize: {18} \n IndexMinorVersion: {19} \n XMLType: {20} \n XMLSubtype: {21} \n XMLCategory: {22} \n XMLModelName: {23} \n ObjectGUID: {24} \n XMLCreator: {25} \n XMLAge: {26} \n XMLGender: {27} \n RequiredEPs: {28} \n Function: {29} \n FunctionSubcategory: {30} \n RoomSort: {31}", this.Title, this.Description, this.Type, this.InstanceIDs, this.Location, this.PackageName, this.Game, this.DBPF, this.Major, this.Minor, this.DateCreated, this.DateModified, this.IndexMajorVersion, this.IndexCount, this.IndexOffset, this.IndexSize, this.HolesCount, this.HolesOffset, this.HolesSize, this.IndexMinorVersion, this.XMLType, this.XMLSubtype, this.XMLCategory, this.XMLModelName, this.ObjectGUID, this.XMLCreator, this.XMLAge, this.XMLGender, this.RequiredEPs, this.Function, this.FunctionSubcategory, this.RoomSort);
        }

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