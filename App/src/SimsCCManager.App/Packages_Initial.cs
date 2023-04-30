using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SSAGlobals;
using ResultsWindow;
using System.Threading.Tasks;
using SimsCCManager.Packages.Containers;


namespace SimsCCManager.Packages.Initial {
    class InitialProcessing {
        
        ParallelOptions parallelSettings = new ParallelOptions() { MaxDegreeOfParallelism = 200};
        string packageExtension = "package";
        public string filename = "";
        public string statement = "";
        public int counter = 0;
        LoggingGlobals log = new LoggingGlobals();
        
        
        public void IdentifyPackages(){
            statement = "Running Identify Packages.";
            log.MakeLog(statement, true);
            string[] files = Directory.GetFiles(GlobalVariables.ModFolder, "*." + packageExtension, SearchOption.AllDirectories);
            foreach (string file in files) {
                FileInfo packageFile = new FileInfo(file);
                statement = "Found " + packageFile.FullName;
                log.MakeLog(statement, true);
                if (packageExtension.Any(packageFile.Extension.Contains)) {
                    statement = "This file is a package file.";
                    log.MakeLog(statement, true);
                    GlobalVariables.PackageFiles.Add(packageFile);
                    statement = "Items in PackageFiles array: " + GlobalVariables.PackageFiles.Count;
                    log.MakeLog(statement, true);
                } else {
                    statement = "This file is not a package file.";
                    log.MakeLog(statement, true);
                    GlobalVariables.NotPackages.Add(packageFile);
                    statement = "Items in notPackages array: " + GlobalVariables.NotPackages;
                    log.MakeLog(statement, true);
                }
            }
            GlobalVariables.PackageCount = GlobalVariables.AllPackages.Count;
            statement = "Checked all packages, returning.";
            log.MakeLog(statement, true);
        }

        void GetTypeOf<T>(T LineType) {
            Console.WriteLine(typeof(T));
        }

        private FileStream Packagefs(FileInfo package)
        {
            FileStream packageFile = new FileStream(package.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
            return packageFile;
        }

        private BinaryReader Packagebr(FileStream packageFile)
        {
            BinaryReader packagereader = new BinaryReader(packageFile);
            return packagereader;
        }


        public void FindBrokenPackages (FileInfo package){
            FileStream packageFile = Packagefs(package);
            BinaryReader packagereader = Packagebr(packageFile);
            string test = "";
            //Console.WriteLine("Checking " + package.FullName);
            test = Encoding.ASCII.GetString(packagereader.ReadBytes(4));
            if (test != "DBPF") {
                packagereader.Close();
                var statement = package.FullName + " is either not a package or is broken.";
                GlobalVariables.AllPackages.Add(new PackageFile{ Name = package.Name, Location = package.FullName, Number = counter, Game = 0, Broken = true});
                log.MakeLog(statement, false);
                return;
            }
        }

        public void IdentifyGames(FileInfo package){ 
            string statement = "";
            statement = "Identifying which game this package file is for.";
            log.MakeLog(statement, true);            
            counter++;    
            FileStream packageFile = Packagefs(package);
            statement = "Package #" + counter + " - Created filestream for package.";
            log.MakeLog(statement, true);
            BinaryReader packagereader = Packagebr(packageFile);
            statement = "Package #" + counter + " - Created binaryreader for package.";
            log.MakeLog(statement, true);
            string test = "";

            test = Encoding.ASCII.GetString(packagereader.ReadBytes(4));

            uint major = packagereader.ReadUInt32();
            test = major.ToString();
            statement = package.Name + " has " + major + " as a major.";
            log.MakeLog(statement, true);
            
            uint minor = packagereader.ReadUInt32();
            test = minor.ToString();
            statement = package.Name + " has " + minor + " as a minor.";
            log.MakeLog(statement, true);
            if (major is 1 && minor is 1) {
                if (GlobalVariables.gameVer is 2) {
                    statement = "[DEBUG] " + package.FullName.ToString() + " is a sims 2 file.";
                    log.MakeLog(statement, true);
                    GlobalVariables.AllPackages.Add(new PackageFile{ Name = package.Name, Location = package.FullName, Number = counter, Game = 2, Broken = false});
                } else {
                    statement = package.FullName + " is a sims 2 file.";
                    log.MakeLog(statement, false);
                    GlobalVariables.AllPackages.Add(new PackageFile{ Name = package.Name, Location = package.FullName, Number = counter, Game = 2, Broken = false});  
                }                
            } else if (major is 2 && minor is 1) {
                if (GlobalVariables.gameVer is 4) {
                    statement = "[DEBUG] " + package.FullName + " is a sims 4 file.";
                    log.MakeLog(statement, true);
                    GlobalVariables.AllPackages.Add(new PackageFile{ Name = package.Name, Location = package.FullName, Number = counter, Game = 4, Broken = false});  
                } else {
                    statement = package.FullName + " is a sims 4 file.";
                    log.MakeLog(statement, false);
                    GlobalVariables.AllPackages.Add(new PackageFile{ Name = package.Name, Location = package.FullName, Number = counter, Game = 3, Broken = false});  
                }                
            } else if (major is 2 && minor is 0) {
                if (GlobalVariables.gameVer is 3) {
                    statement = "[DEBUG] " + package.FullName + " is a sims 3 file.";
                    log.MakeLog(statement, true);
                    GlobalVariables.AllPackages.Add(new PackageFile{ Name = package.Name, Location = package.FullName, Number = counter, Game = 3, Broken = false});
                } else {
                    statement = package.FullName + " is a sims 4 file.";
                    log.MakeLog(statement, false);
                    GlobalVariables.AllPackages.Add(new PackageFile{ Name = package.Name, Location = package.FullName, Number = counter, Game = 3, Broken = false});
                }
            } else if (major is 3 && minor is 0) {
                statement = package.FullName + " is a Sim City 5 file.";
                log.MakeLog(statement, false);
                GlobalVariables.AllPackages.Add(new PackageFile{ Name = package.Name, Location = package.FullName, Number = counter, Game = 12, Broken = false});  
            } else { 
                statement = package.FullName + " was unidentifiable.";
                log.MakeLog(statement, false);
                GlobalVariables.NotPackages.Add(package);
            }
        }
    }
}