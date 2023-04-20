using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CCSorter;
using FindBrokenPackages;
using Sims_CC_Sorter;

namespace CCSorterControls {

    class BrokenIncorrect {
        public int gameChoice = 0;
        public string ModFolder = "";
        public string logfile = "";
        public int packageCount = 1;
        
        

        public void FindPackagesToRemove(int gameNum, string modLocation){
            var time = "";
            gameChoice = gameNum;
            ModFolder = modLocation;
            logfile = modLocation + "\\SimsCCSorter.log";
            ParsePackageFolder parseFolder = new ParsePackageFolder();
            parseFolder.IdentifyPackages(ModFolder);
            StreamWriter putContentsIntoTxt = new StreamWriter(logfile);
            putContentsIntoTxt.Close();
            putContentsIntoTxt = new StreamWriter (logfile, append: true);
            var statement = "";
            packageCount = parseFolder.allPackages.Count;
            time = DateTime.Now.ToString("h:mm:ss tt");
            statement = time + ": Checking " + packageCount + " package files. Larger numbers will take a while and I don't know how to do progress bars yet, so please be patient.";
            putContentsIntoTxt.WriteLine(statement);
            putContentsIntoTxt.Close();
            foreach (FileInfo item in parseFolder.allPackages) {
                parseFolder.FindBrokenPackages(item, logfile, packageCount, gameChoice);
            }
            putContentsIntoTxt = new StreamWriter (logfile, append: true);
            time = DateTime.Now.ToString("h:mm:ss tt");
            statement = time + ": Checked all package files.";
            putContentsIntoTxt.WriteLine(statement);
            putContentsIntoTxt.Close();
            CCSorterApp mainWindow = new CCSorterApp();
            mainWindow.completionAlertValue("Search complete!");
            //FindBrokenPackages.Results resultsWin = new FindBrokenPackages.Results();
            //resultsWin.Show();
        }
    }    
}