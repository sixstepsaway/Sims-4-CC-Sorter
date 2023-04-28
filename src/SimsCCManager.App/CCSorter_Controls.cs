using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SSAGlobals;
using CCSorter.Funcs;
using ResultsWindow;
using S2PackageMaintenance;

namespace CCSorter.Controls {

    class ControlOverview
    {
        /* GAME VERSION NUMBERS ARE:

        0 = null,
        1 = Sims 1,
        2 = Sims 2,
        3 = Sims 3,
        4 = Sims 4,
        11 = Spore,
        12 = SimCity 5*/
        
        public int packageCount;
        string statement = "";
        LoggingGlobals loggingGlobals = new LoggingGlobals();
        //Package OpenPackage = new Package();
        S2Packages s2methods = new S2Packages();
        ProcessSelectedFolder processFolder = new ProcessSelectedFolder();
        

        public void FindPackagesToRemove(){
            packageCount = ProcessSelectedFolder.allPackages.Count;
            var statement = "Checking " + packageCount + " package files. Larger numbers will take a while and I don't know how to do progress bars yet, so please be patient.";
            loggingGlobals.MakeLog(statement, false);
            foreach (FileInfo item in ProcessSelectedFolder.allPackages) {
                statement = "Processing " + item.Name + ".";
                loggingGlobals.MakeLog(statement, true);
                processFolder.FindBrokenPackages(item);
                processFolder.IdentifyGames(item);
            }
            statement = "Checked all package files.";
            loggingGlobals.MakeLog(statement, false);
        }    

        public void RenameS2Packages(){
            statement = "Retrieving packages from All Packages array.";
            loggingGlobals.MakeLog(statement, true);

            statement = "Items in array: " + ProcessSelectedFolder.allPackages.Count;
            loggingGlobals.MakeLog(statement, true);
            foreach (FileInfo packagef in ProcessSelectedFolder.allPackages) {
                statement = "Identifying " + packagef.Name;
                loggingGlobals.MakeLog(statement, true);
                processFolder.IdentifyGames(packagef);
            }
            foreach (PackageFile package in GlobalVariables.packageFiles)
            {
                if (package.Version is 2)
                {   
                    statement = "Looking for Sims 2 packages to rename.";
                    loggingGlobals.MakeLog(statement, false);
                    s2methods.s2GetLabel(package.Location);
                }
            }
            foreach (SimsPackage package in GlobalVariables.allSims2Packages) {
                if (package.Name != "") {
                    statement = "Pretending to rename " + package.Location + " to " + package.Name;
                } else if (package.Description != "") { 
                    statement = "Pretending to rename " + package.Location + " to " + package.Description;
                } else {
                    statement = package.Location  + " had no identifying internals.";
                }                
                loggingGlobals.MakeLog(statement, false);
            }
        }
    }
}