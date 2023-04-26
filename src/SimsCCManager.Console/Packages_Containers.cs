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

    public class fileHasList
	{
		public uint uniqueFiles = 0;
		public uint gmdc = 0;
		public uint gmnd = 0;
		public uint xobj = 0;
		public uint mmat = 0;
		public uint txtr = 0;
		public uint txmt = 0;
		public uint strlist = 0;
		public uint shpe = 0;
		public uint ctss = 0;
		public uint threedir = 0;
		public uint binx = 0;
		public uint img = 0;
		public uint xtol = 0;
		public uint clst = 0;
		public uint xhtn = 0;
		public uint gzps = 0;
		public uint lxnr = 0;
		public uint aged = 0;
		public uint xflr = 0;
		public uint xmol = 0;
		public uint nref = 0;
		public uint objd = 0;
		public uint objf = 0;
		public uint bcon = 0;
		public uint bhav = 0;
		public uint glob = 0;
		public uint ttab = 0;
		public uint slot = 0;
		public uint xstn = 0;
		public uint coll = 0;
		public uint jpeg = 0;
		public uint creg = 0;
		public uint cres = 0;
		public uint matshad = 0;
		public uint trcn = 0;
		public uint xrof = 0;
		public uint xfnc = 0;
		public uint anim = 0;
		public uint xngb = 0;
		public uint fwav = 0;
		public uint ttas = 0;
		public uint lght = 0;
		public uint xmto = 0;
		public uint hous = 0;
		public uint rtex = 0;
		public uint sdna = 0;
		public uint scor = 0;
		public uint tssg = 0;
		public uint lttx = 0;
	}

    public class SimsPackage { // A more in depth package file.
        
        public string Title {get; set;}        public string Description {get; set;}
        public string Location {get; set;}
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

    }

    public class Containers {

        public static List<PackageFile> packageFiles = new List<PackageFile>();
        public static List<SimsPackage> allSims2Packages = new List<SimsPackage>();
        public static List<SimsPackage> allSims3Packages = new List<SimsPackage>();
        public static List<SimsPackage> allSims4Packages = new List<SimsPackage>();

    }


}