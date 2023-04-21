using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CCSorter;
using ResultsWindow;
using Sims_CC_Sorter;
using SSAGlobals;
using OpenPackage;
using S2PackageMaintenance;

namespace CCSorterControls {

    class ControlOverview {
        
        public int gameChoice = 0;

        /* GAME VERSION NUMBERS ARE:

        0 = null,
        1 = Sims 1,
        2 = Sims 2,
        3 = Sims 3,
        4 = Sims 4,
        11 = Spore,
        12 = SimCity 5*/

        public string ModFolder = "";
        public string logfile = "";
        public int packageCount = 1;
        public bool debugMode = true;
        public int gameVer = 0;
        
        LoggingGlobals logGlobals = new LoggingGlobals();
        Package OpenPackage = new Package();
        S2Packages s2methods = new S2Packages();
        ParsePackageFolder parseFolder = new ParsePackageFolder();

        public void Initialize(int gameNum, string modLocation){
            var time = "";
            gameChoice = gameNum;
            ModFolder = modLocation;
            logfile = modLocation + "\\SimsCCSorter.log";
            StreamWriter putContentsIntoTxt = new StreamWriter(logfile);
            putContentsIntoTxt.Close();
        }
        
        public void FindPackages(){
            
            parseFolder.IdentifyPackages(ModFolder);
        }

        public void FindPackagesToRemove(int gameNum, string modLocation){

            var statement = "Checking " + packageCount + " package files. Larger numbers will take a while and I don't know how to do progress bars yet, so please be patient.";
            logGlobals.MakeLog(statement, debugMode, false);
            foreach (FileInfo item in parseFolder.allPackages) {
                parseFolder.FindBrokenPackages(item);
                parseFolder.IdentifyGames(item, gameNum);
            }
            statement = "Checked all package files.";
            logGlobals.MakeLog(statement, debugMode, false);
            CCSorterApp mainWindow = new CCSorterApp();
            mainWindow.completionAlertValue("Search complete!");
        }
        public List<PackageFiles> packageFiles = new List<PackageFiles>();
        public void ProcessPackages()
        
        {
            
            var counter = 0;
            var statement = "Processing " + packageCount + " package files. Larger numbers will take a while and I don't know how to do progress bars yet, so please be patient.";
            logGlobals.MakeLog(statement, debugMode, false);
            foreach (FileInfo item in parseFolder.allPackages) {
                counter++;
                int gameVer = OpenPackage.PackageVersion(item);
                var temp = new PackageFiles();
                temp.Name = item.Name;
                temp.Location = item.FullName;
                temp.Number = counter;
                temp.Version = gameVer;
                packageFiles.Add(temp);
            }
        }     

        public void RenameS2Packages(){
            foreach (PackageFiles package in packageFiles)
            {
                if (package.Version is 2)
                {
                    s2methods.s2GetLabel(package.Location);
                }
            }
        }
    }    
}