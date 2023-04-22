using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SSAGlobals;
using CCSorter.Funcs;

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
        
        //public int packageCount;
        LoggingGlobals logGlobals = new LoggingGlobals();
        //Package OpenPackage = new Package();
        //S2Packages s2methods = new S2Packages();
        ProcessSelectedFolder processFolder = new ProcessSelectedFolder();
        
        public void FindPackages(){            
            processFolder.IdentifyPackages();
        }

        public void FindPackagesToRemove(int gameNum, string modLocation){
            var packageCount = processFolder.allPackages.Count;
            var statement = "Checking " + packageCount + " package files. Larger numbers will take a while and I don't know how to do progress bars yet, so please be patient.";
            logGlobals.MakeLog(statement, false);
            foreach (FileInfo item in processFolder.allPackages) {
                processFolder.FindBrokenPackages(item);
                processFolder.IdentifyGames(item, gameNum);
            }
            statement = "Checked all package files.";
            logGlobals.MakeLog(statement, false);
            //CCSorterApp mainWindow = new CCSorterApp();
            //mainWindow.completionAlertValue("Search complete!");
        }
    }
}