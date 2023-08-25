using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SSAGlobals;
using System.Threading.Tasks;
using SimsCCManager.Manager;

namespace SimsCCManager.Packages.Initial {
    class InitialProcessing {
        /// <summary>
        /// Universal methods for finding the game version of a package, whether the package is broken, and so on.
        /* GAME VERSION NUMBERS ARE:

                0 = null,
                1 = Sims 1,
                2 = Sims 2,
                3 = Sims 3,
                4 = Sims 4,
                11 = Spore,
                12 = SimCity 5*/
        /// </summary>
        
        ParallelOptions parallelSettings = new ParallelOptions() { MaxDegreeOfParallelism = 200};
        string packageExtension = ".package";
        public string filename = "";
        public string statement = "";
        public int counter = -1;
        LoggingGlobals log = new LoggingGlobals();
        //S2PackageSearch s2s = new S2PackageSearch();
        //S3PackageSearch s3s = new S3PackageSearch();
        //S4PackageSearch s4s = new S4PackageSearch();
        
                
        
        public InitialCheck CheckThrough (string input){
            InitialCheck ic = new();
            StringBuilder LogFile = new StringBuilder();
            string LogMessage = "";            
            FileInfo pack = new FileInfo(input);
            //MemoryStream msPackage = Methods.ReadBytesToFile(input, 12);
            FileStream msPackage = new FileStream(input, FileMode.Open, FileAccess.Read);
            BinaryReader packagereader = new BinaryReader(msPackage);
            string test = "";
            test = Encoding.ASCII.GetString(packagereader.ReadBytes(4));
            if (test != "DBPF") {
                LogMessage = string.Format("Adding broken file {0} to list.", pack.Name);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                packagereader.Close();
                msPackage.Close();
                packagereader.Dispose();
                msPackage.Dispose();
                ic.Broken = true;
                return ic;
            } else {
                ic.Major = packagereader.ReadUInt32();                
                ic.Minor = packagereader.ReadUInt32();
                LogMessage = string.Format("{0} has {1} as a major and {2} as a minor.", pack.Name, ic.Major, ic.Minor);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                if ((ic.Major is 1 && ic.Minor is 1) || (ic.Major is 1 && ic.Minor is 2)) {
                    LogMessage = string.Format("{0} is a Sims 2 file.", pack.FullName);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    ic.Game = "Sims2";
                    ic.Broken = false;
                    ic.Unidentifiable = false;
                    packagereader.Dispose();
                    msPackage.Dispose();
                    return ic;
                } else if (ic.Major is 2 && ic.Minor is 0) {
                    LogMessage = string.Format("{0} is a Sims 3 file.", pack.FullName);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    ic.Game = "Sims3";
                    ic.Broken = false;
                    ic.Unidentifiable = false;
                    packagereader.Dispose();
                    msPackage.Dispose();
                    return ic;                    
                } else if (ic.Major is 2 && ic.Minor is 1) {
                    LogMessage = string.Format("{0} is a Sims 4 file.", pack.FullName);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    ic.Game = "Sims4";
                    ic.Broken = false;
                    ic.Unidentifiable = false;
                    packagereader.Dispose();
                    msPackage.Dispose();
                    return ic;                    
                } else if (ic.Major is 3 && ic.Minor is 0) {
                    LogMessage = string.Format("{0} is a Sim City 5 file.", pack.FullName);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    ic.Game = "SC5";
                    ic.Broken = false;
                    ic.Unidentifiable = false;
                    packagereader.Dispose();
                    msPackage.Dispose();
                    return ic;
                } else {
                    LogMessage = string.Format("{0} was unidentifiable.", pack.FullName);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    ic.Unidentifiable = true;
                    packagereader.Dispose();
                    msPackage.Dispose();
                    return ic;
                }                
            }
        }                
    }
}