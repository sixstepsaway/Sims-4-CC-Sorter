using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SSAGlobals;
using System.Threading.Tasks;
using SimsCCManager.Packages.Containers;
using SQLitePCL;
using SQLite;
using System.Data.SQLite;
using SimsCCManager.Packages.Sims2Search;
using SimsCCManager.Packages.Sims3Search;
using SimsCCManager.Packages.Sims4Search;
using SimsCCManager.Packages.Sorting;
using System.Threading;

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
        S2PackageSearch s2s = new S2PackageSearch();
        S3PackageSearch s3s = new S3PackageSearch();
        S4PackageSearch s4s = new S4PackageSearch();
        
                
        
        public bool CheckThrough (string input){
            StringBuilder LogFile = new StringBuilder();
            string LogMessage = "";
            bool complete = false;
            int count = GlobalVariables.packagesReadReading;
            GlobalVariables.packagesReadReading++;
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
                string packageNameUpdated = Methods.FixApostrophesforSQL(pack.Name);
                string qcmd = string.Format("SELECT * FROM AllFiles where Name = '{0}'", packageNameUpdated);
                var fileq = GlobalVariables.DatabaseConnection.Query<AllFiles>(qcmd);
                AllFiles query = fileq[0];
                /*string qtype = query.Type;
                GlobalVariables.DatabaseConnection.Delete(query);*/ 
                packagereader.Close();
                msPackage.Close();
                packagereader.Dispose();
                msPackage.Dispose();
                if (GlobalVariables.sortonthego == true){
                    string newloc = Path.Combine(FilesSort.BrokenFiles, query.Name);
                    if(!Directory.Exists(FilesSort.BrokenFiles)){
                        Methods.MakeFolder(FilesSort.BrokenFiles);
                    }
                    File.Move(query.Location, newloc);
                    query.Location = newloc;
                    query.Status = "Broken";
                        GlobalVariables.DatabaseConnection.InsertOrReplace(query);
                } else {
                    query.Status = "Broken";
                        GlobalVariables.DatabaseConnection.InsertOrReplace(query);
                }
                complete = true;
                return complete;
            } else {
                counter++;
                uint major = packagereader.ReadUInt32();                
                uint minor = packagereader.ReadUInt32();
                LogMessage = string.Format("{0} has {1} as a major and {2} as a minor.", pack.Name, major, minor);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                if ((major is 1 && minor is 1) || (major is 1 && minor is 2)) {
                    LogMessage = string.Format("{0} is a Sims 2 file.", pack.FullName);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    PtoDb(pack, 2);
                    try {
                        s2s.SearchS2Packages(msPackage, pack, minor, count, LogFile);
                    } catch (Exception e) {
                        log.MakeLog(string.Format("Caught exception reading Sims 2 package {0}, Exception: {1}", pack.Name, e), true);
                    }
                    
                } else if (major is 2 && minor is 0) {
                    LogMessage = string.Format("{0} is a Sims 3 file.", pack.FullName);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    if (GlobalVariables.sortonthego == true){
                        //PackageFile pf = new PackageFile{Name = pack.Name, Location = pack.FullName, Game = 3, Broken = false};
                        //string newloc = Path.Combine(FilesSort.SortedS3Folder, pf.Name);
                        msPackage.Close();
                        msPackage.Dispose();
                        packagereader.Close();
                        packagereader.Dispose();
                        
                        //File.Move(pf.Location, newloc);
                        //pf.Location = newloc;                    
                        //PtoDb(pf, 3);
                    } else {
                        //PtoDb(pack, 3);
                    }  
                    if(GlobalVariables.highdebug == false) log.MakeLog(string.Format("Log Dumped from StringBuilder: \n {0}", LogFile.ToString()), true);
                    return true;
                    //s3s.SearchS3Packages(pack, count);
                } else if (major is 2 && minor is 1) {
                    LogMessage = string.Format("{0} is a Sims 4 file.", pack.FullName);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    PtoDb(pack, 4);
                    try {
                        s4s.SearchS4Packages(msPackage, pack, count, LogFile);
                    } catch (Exception e) {
                        log.MakeLog(string.Format("Caught exception reading Sims 4 package {0}, Exception: {1}", pack.Name, e), true);
                    }
                    
                } else if (major is 3 && minor is 0) {
                    LogMessage = string.Format("{0} is a Sims 5 file.", pack.FullName);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    AllFiles af = new AllFiles();         
                    af.Name  = pack.Name;
                    af.Location = pack.FullName;
                    af.Type = "package";
                    af.Status = "SimCity 5 Package";
                    GlobalVariables.DatabaseConnection.InsertOrReplace(af);
                    if(GlobalVariables.highdebug == false) log.MakeLog(string.Format("Log Dumped from StringBuilder: \n {0}", LogFile.ToString()), true);
                } else {
                    LogMessage = string.Format("{0} was unidentifiable.", pack.FullName);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    AllFiles af = new AllFiles();
                    af.Location = pack.FullName;           
                    af.Name  = pack.Name;
                    af.Location = pack.FullName;
                    af.Type = "package";
                    af.Status = "Unidentifiable version";
                    GlobalVariables.DatabaseConnection.InsertOrReplace(af);
                    if(GlobalVariables.highdebug == false) log.MakeLog(string.Format("Log Dumped from StringBuilder: \n {0}", LogFile.ToString()), true);                
                }                
                packagereader.Dispose();
                msPackage.Dispose();
                complete = true;
                return complete;
            }
        }
        

        private void PtoDb(PackageFile f, int game){
            log.MakeLog(string.Format("Adding {0} to database as for Sims {1}", f.Name, game), true);
            
            GlobalVariables.ProcessingReader.Enqueue(f);

            //Containers.Containers.identifiedPackages.Add(new PackageFile{Name = f.Name, Location = f.FullName, Game = game, Broken = false, Status = "Pending"});
            
            log.MakeLog(string.Format("Added {0}! Returning.", f.Name), true);
        }
        private void PtoDb(FileInfo f, int game){
            log.MakeLog(string.Format("Adding {0} to database as for Sims {1}", f.Name, game), true);
            //string packageNameUpdated = Methods.FixApostrophesforSQL(f.Name);
            GlobalVariables.ProcessingReader.Enqueue(new PackageFile{Name = f.Name, Location = f.FullName, Game = game, Broken = false});

            //Containers.Containers.identifiedPackages.Add(new PackageFile{Name = f.Name, Location = f.FullName, Game = game, Broken = false, Status = "Pending"});
            
            log.MakeLog(string.Format("Added {0}! Returning.", f.Name), true);
        }
    }
}