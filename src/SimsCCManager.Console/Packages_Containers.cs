using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

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
        public int Game {get; set;}
        public string DBPF {get; set;}
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

        public static List<PackageFile> packageFiles = new List<PackageFile>();
        public static List<SimsPackage> allSims2Packages = new List<SimsPackage>();
        public static List<SimsPackage> allSims3Packages = new List<SimsPackage>();
        public static List<SimsPackage> allSims4Packages = new List<SimsPackage>();

    }


}