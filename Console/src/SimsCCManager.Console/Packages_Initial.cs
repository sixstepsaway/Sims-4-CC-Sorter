using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SSAGlobals;
using System.Threading.Tasks;
using SimsCCManager.Packages.Containers;


namespace SimsCCManager.Packages.Initial {
    class InitialProcessing {
        
        ParallelOptions parallelSettings = new ParallelOptions() { MaxDegreeOfParallelism = 200};
        string packageExtension = "package";
        public string filename = "";
        public string statement = "";
        public int counter = -1;
        LoggingGlobals log = new LoggingGlobals();
        
        
        public void IdentifyPackages(){
            statement = "Running Identify Packages.";
            log.MakeLog(statement, true);
            string ts3pack = "sims3pack";
            string ts2pack = "sims2pack";
            string ts4script = "ts4script";
            string[] files = Directory.GetFiles(GlobalVariables.ModFolder, "*", SearchOption.AllDirectories);
            foreach (string file in files) {
                FileInfo packageFile = new FileInfo(file);
                statement = "Found " + packageFile.FullName;
                log.MakeLog(statement, true);
                if (packageExtension.Any(packageFile.Extension.Contains)) {
                    statement = "This file is a package file.";
                    log.MakeLog(statement, true);
                    GlobalVariables.justPackageFiles.Add(packageFile);
                    statement = "Items in justPackageFiles array: " + GlobalVariables.justPackageFiles.Count;
                    log.MakeLog(statement, true);
                } else if (ts3pack.Any(packageFile.Extension.Contains)) {
                    log.MakeLog("This is a Sims3package.", true);
                    NotPackage notpack = new NotPackage();
                    notpack.notPackage = true;
                    notpack.actualType = "sims3pack";
                    GlobalVariables.sims3packfiles.Add(new SimsPackage{ Location = packageFile.FullName, PackageName = packageFile.Name, NotAPackage = notpack});
                    statement = "Items in sims3package array: " + GlobalVariables.sims3packfiles.Count;
                    log.MakeLog(statement, true);
                } else if (ts2pack.Any(packageFile.Extension.Contains)) {
                    NotPackage notpack = new NotPackage();
                    notpack.notPackage = true;
                    notpack.actualType = "sims2pack";
                    GlobalVariables.sims2packfiles.Add(new SimsPackage{ Location = packageFile.FullName, PackageName = packageFile.Name, NotAPackage = notpack});
                    statement = "Items in sims2package array: " + GlobalVariables.sims3packfiles.Count;
                    log.MakeLog(statement, true);
                } else if (ts4script.Any(packageFile.Extension.Contains)) {
                    NotPackage notpack = new NotPackage();
                    notpack.notPackage = true;
                    notpack.actualType = "ts4script";
                    GlobalVariables.ts4scriptFiles.Add(new SimsPackage{ Location = packageFile.FullName, PackageName = packageFile.Name, NotAPackage = notpack});
                    statement = "Items in sims2package array: " + GlobalVariables.sims3packfiles.Count;
                    log.MakeLog(statement, true);
                } else {
                    statement = "This file is not a package file.";
                    log.MakeLog(statement, true);
                    GlobalVariables.notPackageFiles.Add(packageFile);
                    statement = "Items in notPackages array: " + GlobalVariables.notPackageFiles.Count;
                    log.MakeLog(statement, true);
                }
            }
            GlobalVariables.PackageCount = GlobalVariables.gamesPackages.Count;
            statement = "Checked all packages, returning.";
            log.MakeLog(statement, true);
        }

        void GetTypeOf<T>(T LineType) {
            //Console.WriteLine(typeof(T));
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


        public void FindBrokenPackages (string input){
            FileInfo pack = new FileInfo(input);
            FileStream packageFile = Packagefs(pack);
            BinaryReader packagereader = Packagebr(packageFile);
            string test = "";
            //Console.WriteLine("Checking " + package.FullName);
            test = Encoding.ASCII.GetString(packagereader.ReadBytes(4));
            if (test != "DBPF") {
                packagereader.Close();
                var statement = pack.FullName + " is either not a package or is broken.";
                GlobalVariables.brokenFiles.Add(new SimsPackage{ Title = pack.Name, Location = pack.FullName, Broken = true});
                log.MakeLog(statement, false);
                return;
            } else {
                packagereader.Close();
                log.MakeLog(pack.FullName + " is a package.", true);                
                GlobalVariables.workingPackageFiles.Add(new PackageFile{ Name = pack.Name, Location = pack.FullName, Broken = false});
                return;
            }
        }

        public void IdentifyGames(string input){ 
            counter++;
            FileInfo pack = new FileInfo(input);
            string statement = "";
            statement = "Identifying which game this package file is for.";
            log.MakeLog(statement, true);
            FileStream packageFile = Packagefs(pack);
            statement = "Package #" + counter + " " + pack.Name + " - Created filestream for package.";
            log.MakeLog(statement, true);
            BinaryReader packagereader = Packagebr(packageFile);
            statement = "Package #" + counter + " " + pack.Name + " - Created binaryreader for package.";
            log.MakeLog(statement, true);
            string test = "";

            test = Encoding.ASCII.GetString(packagereader.ReadBytes(4));

            uint major = packagereader.ReadUInt32();
            test = major.ToString();
            statement = pack.Name + " has " + major + " as a major.";
            log.MakeLog(statement, true);
            
            uint minor = packagereader.ReadUInt32();
            test = minor.ToString();
            statement = pack.Name + " has " + minor + " as a minor.";
            log.MakeLog(statement, true);
            if (major is 1 && minor is 1) {
                statement = pack.FullName + " is a sims 2 file.";
                log.MakeLog(statement, false);
                GlobalVariables.gamesPackages.Add(new PackageFile{ Name = pack.Name, Location = pack.FullName, Number = counter, Game = 2, Broken = false});  
            } else if (major is 2 && minor is 0) {
                statement = pack.FullName + " is a sims 3 file.";
                log.MakeLog(statement, false);
                GlobalVariables.gamesPackages.Add(new PackageFile{ Name = pack.Name, Location = pack.FullName, Number = counter, Game = 3, Broken = false});
            } else if (major is 2 && minor is 1) {
                statement = pack.FullName + " is a sims 4 file.";
                log.MakeLog(statement, false);
                GlobalVariables.gamesPackages.Add(new PackageFile{ Name = pack.Name, Location = pack.FullName, Number = counter, Game = 3, Broken = false});            
            } else if (major is 3 && minor is 0) {
                statement = pack.FullName + " is a Sim City 5 file.";
                log.MakeLog(statement, false);
                GlobalVariables.gamesPackages.Add(new PackageFile{ Name = pack.Name, Location = pack.FullName, Number = counter, Game = 12, Broken = false});  
            } else { 
                statement = pack.FullName + " was unidentifiable.";
                log.MakeLog(statement, false);
                GlobalVariables.notPackageFiles.Add(pack);
            }
        }
    }
}