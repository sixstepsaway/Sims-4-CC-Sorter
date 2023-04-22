using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CCSorterControls;

namespace SSAGlobals {

    class LoggingGlobals {
        private string time = "";
        private string statement = "";
        ControlOverview data = new ControlOverview();
        public bool debugModeSetting = false;
        public void MakeLog (string Statement, bool debug, bool onlyDebug)
        {
            if (onlyDebug) {
                if (debug) {
                    //Writes to a log file.
                    StreamWriter addToLog = new StreamWriter (data.logfile, append: true);
                    time = DateTime.Now.ToString("h:mm:ss tt");
                    statement = time + ": " + Statement;
                    addToLog.WriteLine(statement);
                    addToLog.Close();
                } else {
                    //no log
                }                
            } else {
                //Writes to a log file.
                StreamWriter addToLog = new StreamWriter (data.logfile, append: true);
                time = DateTime.Now.ToString("h:mm:ss tt");
                statement = time + ": " + Statement;
                addToLog.WriteLine(statement);
                addToLog.Close();
            }
        }
    }    
}