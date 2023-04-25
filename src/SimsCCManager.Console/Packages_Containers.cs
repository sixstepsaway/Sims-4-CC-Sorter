using System;
using System.Collections.Generic;
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
        public int Game {get; set;}

    }

    public class Containers {

        public static List<PackageFile> packageFiles = new List<PackageFile>();
        public static List<SimsPackage> allSims2Packages = new List<SimsPackage>();
        public static List<SimsPackage> allSims3Packages = new List<SimsPackage>();
        public static List<SimsPackage> allSims4Packages = new List<SimsPackage>();

    }


}