using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using SimsCCManager.Globals;

namespace SimsCCManager.Debugging
{
    public class Logging
    {
        /// <summary>
        /// Synchronous log file implementation. 
        /// </summary>
        
        private static string debuglog = Path.Combine(GlobalVariables.logfolder, "debug.log");
        static ReaderWriterLock locker = new ReaderWriterLock();
        static bool initialized = false;
        //Function for logging to the logfile set at the start of the program

        public static void WriteDebugLog(string statement, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = ""){            
            string time = "";
            string filepath = "";
            if (filePath != "") {
                filepath = new FileInfo(filePath).Name;
            } else {
                filepath = "unknown";
            }
            
            if (!File.Exists(debuglog)){
                initialized = true;                
                try
                {
                    new FileInfo(debuglog).Directory.Create();
                    time = DateTime.Now.ToString("h:mm:ss tt");
                    locker.AcquireWriterLock(int.MaxValue); 
                    StreamWriter addToInternalLog = new StreamWriter (debuglog, append: false);
                    addToInternalLog.WriteLine(string.Format("[L{0} | {1} {2}]: Initializing debug log file.", lineNumber, filepath, time));
                    addToInternalLog.WriteLine(string.Format("[L{0} | {1} {2}]: {3}", lineNumber, filepath, time, statement));
                    addToInternalLog.WriteLine(statement);
                    addToInternalLog.Close();
                }
                finally
                {
                    locker.ReleaseWriterLock();
                }
            } else if (!initialized) {
                initialized = true;                
                try
                {
                    string oldloc = string.Format("{0}.old", debuglog);
                    if (File.Exists(oldloc)){
                        File.Delete(oldloc);
                    }                    
                    File.Copy(debuglog, oldloc);                        
                    time = DateTime.Now.ToString("h:mm:ss tt");
                    locker.AcquireWriterLock(int.MaxValue); 
                    StreamWriter addToInternalLog = new StreamWriter (debuglog, append: false);
                    addToInternalLog.WriteLine(string.Format("[L{0} | {1} {2}]: Initializing debug log file.", lineNumber, filepath, time));
                    addToInternalLog.WriteLine(string.Format("[L{0} | {1} {2}]: {3}", lineNumber, filepath, time, statement));
                    addToInternalLog.WriteLine(statement);
                    addToInternalLog.Close();
                }
                finally
                {
                    locker.ReleaseWriterLock();
                }
            } else {            
                try
                {
                    time = DateTime.Now.ToString("h:mm:ss tt");
                    statement = string.Format("[L{0} | {1} {2}]: {3}", lineNumber, filepath, time, statement);
                    locker.AcquireWriterLock(int.MaxValue); 
                    StreamWriter addToInternalLog = new StreamWriter (debuglog, append: true);
                    addToInternalLog.WriteLine(statement);
                    addToInternalLog.Close();
                }
                finally
                {
                    locker.ReleaseWriterLock();
                } 
            }
        }
    }
}