using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SSAGlobals {

    public class GlobalVariables {
        public static bool debugMode = false;
        public static string logfile;
        public static string ModFolder;
        public static int gameVer;

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
        public void MakeLog (string Statement, bool onlyDebug)
        {
            if (onlyDebug) {
                if (GlobalVariables.debugMode) {
                    //Writes to a log file.
                    StreamWriter addToLog = new StreamWriter (GlobalVariables.logfile, append: true);
                    time = DateTime.Now.ToString("h:mm:ss tt");
                    statement = time + ": " + Statement;
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