using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.CompilerServices;

namespace SSAGlobals {

    public class PackageFile { //A basic summary of a package file.
        public string Name {get; set;}
        public string Location {get; set;}
        public int Number {get; set;}
        public int Version {get; set;}
        public bool Broken {get; set;}
    }

    public class SimsPackage { // A more in depth package file.
        
        public string Name {get; set;}
        public string Description {get; set;}
        public string Location {get; set;}
        public string DBPF {get; set;}
        public int Game {get; set;}
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
    }

    public class GlobalVariables {
        public static bool debugMode = true;
        public static string logfile;
        public static string ModFolder;
        public static int gameVer;

        public static List<PackageFile> packageFiles = new List<PackageFile>();
        public static List<SimsPackage> allSims2Packages = new List<SimsPackage>();
        public static List<SimsPackage> allSims3Packages = new List<SimsPackage>();
        public static List<SimsPackage> allSims4Packages = new List<SimsPackage>();


        public void Initialize(int gameNum, string modLocation){
            gameVer = gameNum;
            ModFolder = modLocation;
            logfile = modLocation + "\\SimsCCSorter.log";
            StreamWriter putContentsIntoTxt = new StreamWriter(logfile);
            putContentsIntoTxt.Close();
        }
    }          

    public class LoggingGlobals
    {
        
        private string time = "";
        private string statement = "";
        
        //Function for logging to the logfile set at the start of the program
        public void MakeLog (string Statement, bool onlyDebug, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "")
        {   
            
            if (onlyDebug) {
                if (GlobalVariables.debugMode) {
                    //Writes to a log file.
                    StreamWriter addToLog = new StreamWriter (GlobalVariables.logfile, append: true);
                    time = DateTime.Now.ToString("h:mm:ss tt");
                    statement = "[L" + lineNumber + " | " + filePath + "]" + time + ": " + Statement;
                    addToLog.WriteLine(statement);
                    addToLog.Close();
                } else {
                    //no log
                }                
            } else {
                //Writes to a log file.
                StreamWriter addToLog = new StreamWriter (GlobalVariables.logfile, append: true);
                time = DateTime.Now.ToString("h:mm:ss tt");
                statement = time + ": " + Statement;
                addToLog.WriteLine(statement);
                addToLog.Close();
            }
        }
    } 
}