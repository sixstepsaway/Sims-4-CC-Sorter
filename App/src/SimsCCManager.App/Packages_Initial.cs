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
        
        
        public void IdentifyPackages(string file, bool findnew){
            
            /*statement = "Running Identify Packages.";
            log.MakeLog(statement, true);
            string[] ts3pack = {".sims3pack", ".Sims3Pack"};
            string[] ts2pack = {".sims2pack", ".Sims2Pack"};
            string[] ts4script = {".ts4script", ".TS4Script"};
            string[] compressedfiles = {".zip", ".rar", ".7z", ".pkg"};
            string[] files = Directory.GetFiles(GlobalVariables.ModFolder, "*", SearchOption.AllDirectories);
            FileInfo packageFile = new FileInfo(file);
            statement = "Found " + packageFile.FullName;
            log.MakeLog(statement, true);
            if (findnew == true){
                var dbc = new SQLite.SQLiteConnection(GlobalVariables.PackagesRead);
                var packagesPending = dbc.Query<PackageFile>("SELECT * FROM Processing_Reader where Status = 'Pending'");
                var packagesProcessing = dbc.Query<PackageFile>("SELECT * FROM Processing_Reader where Status = 'Processing'");
                var packagesDone = dbc.Query<SimsPackage>("SELECT * FROM Packages");
                var notpack = dbc.Query<NotPackages>("SELECT * FROM NotPackages");

                bool foundfile;

                var isinpending = from pending in packagesPending
                    where pending.Location == packageFile.FullName
                    select pending.Location;
                    foundfile = isinpending.Any();
                var isinprocessing = from processing in packagesProcessing
                    where processing.Location == packageFile.FullName
                    select processing.Location;
                    foundfile = isinprocessing.Any();
                var isindone = from done in packagesDone
                    where done.Location == packageFile.FullName
                    select done.Location;
                    foundfile = isindone.Any();
                var isinnp = from np in notpack
                    where np.Location == packageFile.FullName
                    select np.Location;
                    foundfile = isinnp.Any();
                
                if (foundfile == true){
                    log.MakeLog("File " + packageFile.Name + " already exists in database.", true);
                } else {
                    if (Path.GetExtension(packageFile.FullName) == packageExtension) {
                    log.MakeLog(packageFile.FullName + " is a package file.", true);
                    GlobalVariables.justPackageFiles.Add(packageFile);
                    log.MakeLog("Items in justPackageFiles array: " + GlobalVariables.justPackageFiles.Count, true);
                } else if (ts3pack.Contains(Path.GetExtension(packageFile.FullName))) {
                    log.MakeLog(packageFile.FullName + " is a sims3pack file.", true);
                    NotPtoDb("sims3pack", packageFile.FullName);
                    log.MakeLog("sims3packages found: " + CountItems("sims3pack"), true);
                } else if (ts2pack.Contains(Path.GetExtension(packageFile.FullName))) {
                    log.MakeLog(packageFile.FullName + " is a sims2pack file.", true);
                    NotPtoDb("sims2pack", packageFile.FullName);
                    log.MakeLog("sims2packages found: " + CountItems("sims2pack"), true);
                } else if (ts4script.Contains(Path.GetExtension(packageFile.FullName))) {
                    log.MakeLog(packageFile.FullName + " is a ts4script file.", true);
                    NotPtoDb("ts4script", packageFile.FullName);
                    log.MakeLog("ts4scripts found: " + CountItems("ts4script"), true);
                } else if (compressedfiles.Contains(Path.GetExtension(packageFile.FullName))) {
                    log.MakeLog(packageFile.FullName + " is a compressed file.", true);
                    NotPtoDb("compressed file", packageFile.FullName);
                    log.MakeLog("compressedfiles found: " + CountItems("compressed file"), true);
                } else {
                    log.MakeLog(packageFile.FullName + " is not a package file.", true);
                    NotPtoDb("other", packageFile.FullName);
                    log.MakeLog("Other files found: " + CountItems("other"), true);
                } 
                }

            } else {
                if (Path.GetExtension(packageFile.FullName) == packageExtension) {
                    log.MakeLog(packageFile.FullName + " is a package file.", true);
                    GlobalVariables.justPackageFiles.Add(packageFile);
                    log.MakeLog("Items in justPackageFiles array: " + GlobalVariables.justPackageFiles.Count, true);
                } else if (ts3pack.Contains(Path.GetExtension(packageFile.FullName))) {
                    log.MakeLog(packageFile.FullName + " is a sims3pack file.", true);
                    NotPtoDb("sims3pack", packageFile.FullName);
                    log.MakeLog("sims3packages found: " + CountItems("sims3pack"), true);
                } else if (ts2pack.Contains(Path.GetExtension(packageFile.FullName))) {
                    log.MakeLog(packageFile.FullName + " is a sims2pack file.", true);
                    NotPtoDb("sims2pack", packageFile.FullName);
                    log.MakeLog("sims2packages found: " + CountItems("sims2pack"), true);
                } else if (ts4script.Contains(Path.GetExtension(packageFile.FullName))) {
                    log.MakeLog(packageFile.FullName + " is a ts4script file.", true);
                    NotPtoDb("ts4script", packageFile.FullName);
                    log.MakeLog("ts4scripts found: " + CountItems("ts4script"), true);
                } else if (compressedfiles.Contains(Path.GetExtension(packageFile.FullName))) {
                    log.MakeLog(packageFile.FullName + " is a compressed file.", true);
                    NotPtoDb("compressed file", packageFile.FullName);
                    log.MakeLog("compressedfiles found: " + CountItems("compressed file"), true);
                } else {
                    log.MakeLog(packageFile.FullName + " is not a package file.", true);
                    NotPtoDb("other", packageFile.FullName);
                    log.MakeLog("Other files found: " + CountItems("other"), true);
                }
            }*/
                        
        }
        
        private void NotPtoDb(string type, string location){
            log.MakeLog("Adding item to database as " + type, true);
            using (var db = new System.Data.SQLite.SQLiteConnection(GlobalVariables.PackagesReadDS)){
                db.Open();
                string cmdtext = string.Format("INSERT INTO NotPackages(type, location) VALUES('{0}', '{1}')", type, location);
                System.Data.SQLite.SQLiteCommand sqm = new System.Data.SQLite.SQLiteCommand(cmdtext, db);
                sqm.ExecuteNonQuery();                
                db.Close();
            }
            log.MakeLog("Added! Returning.", true);
        }

        private int CountItems(string type) {
            log.MakeLog("Checking count of database.", true);
            int value;
            using (var db = new System.Data.SQLite.SQLiteConnection(GlobalVariables.PackagesReadDS)){               
                db.Open();
                string cmdtext = string.Format("SELECT count(*) FROM NotPackages where type = '{0}'", type);
                System.Data.SQLite.SQLiteCommand sqm = new System.Data.SQLite.SQLiteCommand(cmdtext, db);
                value = Convert.ToInt32(sqm.ExecuteScalar());                
                db.Close();
            }
            log.MakeLog("Counted! Returning.", true);
            return value;
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
            byte[] file = File.ReadAllBytes(pack.FullName);
            MemoryStream msPackage = new MemoryStream(file);
            BinaryReader packagereader = new BinaryReader(msPackage);
            string test = "";
            test = Encoding.ASCII.GetString(packagereader.ReadBytes(4));
            if (test != "DBPF") {
                packagereader.Close();
                var statement = pack.FullName + " is either not a package or is broken.";
                //GlobalVariables.brokenFiles.Add(new SimsPackage{ Title = pack.Name, Location = pack.FullName, Broken = true});
                log.MakeLog("Adding broken file to list.", true);
                GlobalVariables.DatabaseConnection.Insert(new BrokenChecked{Name = pack.Name, Location = pack.FullName, Status = "Broken"});
                log.MakeLog(statement, false);
                packagereader.Dispose();
                msPackage.Dispose();
                return;
            } else {
                packagereader.Close();
                log.MakeLog(pack.FullName + " is a package.", true); 
                log.MakeLog("Adding working file to list.", true);
                GlobalVariables.DatabaseConnection.Insert(new BrokenChecked{Name = pack.Name, Location = pack.FullName, Status = "Working"});
                //GlobalVariables.workingPackageFiles.Add(new PackageFile{ Name = pack.Name, Location = pack.FullName, Broken = false});
                packagereader.Dispose();
                msPackage.Dispose();
                return;
            }
            
        }

        public void IdentifyGames(string input){ 
                        
            counter++;
            
            FileInfo pack = new FileInfo(input);
            log.MakeLog("Located file " + pack.Name, true);
            byte[] file = File.ReadAllBytes(pack.FullName);
            log.MakeLog("Created bytearray for " + pack.Name, true);
            MemoryStream msPackage = new MemoryStream(file);
            log.MakeLog("Created memorystream for " + pack.Name, true);
            BinaryReader packagereader = new BinaryReader(msPackage);
            log.MakeLog("Created binaryreader for " + pack.Name, true);
            packagereader.BaseStream.Position = 4;
            //string test = "";

            //test = Encoding.ASCII.GetString(packagereader.ReadBytes(4));

            uint major = packagereader.ReadUInt32();
            log.MakeLog(pack.Name + " has " + major + " as a major.", true);
            
            uint minor = packagereader.ReadUInt32();
            //test = minor.ToString();
            log.MakeLog(pack.Name + " has " + minor + " as a minor.", true);
            if (major is 1 && minor is 1) {
                log.MakeLog(pack.FullName + " is a sims 2 file.", false);
                PtoDb(pack, 2);
            } else if (major is 2 && minor is 0) {
                log.MakeLog(pack.FullName + " is a sims 3 file.", false);
                PtoDb(pack, 3);
            } else if (major is 2 && minor is 1) {
                log.MakeLog(pack.FullName + " is a sims 4 file.", false);
                PtoDb(pack, 4);
            } else if (major is 3 && minor is 0) {
                log.MakeLog(pack.FullName + " is a Sim City 5 file.", false);
                PtoDb(pack, 12);
            } else {
                log.MakeLog(pack.FullName + " was unidentifiable.", false);
                NotPtoDb("unidentifiable", pack.FullName);
            }
            packagereader.Dispose();
            msPackage.Dispose();
        }

        private void PtoDb(FileInfo f, int game){
            log.MakeLog("Adding " + f.Name + " to database as for Sims " + game, true);
            try {
              GlobalVariables.DatabaseConnection.Insert(new PackageFile { Name = f.Name, Location = f.FullName, Game = game, Broken = false, Status = "Pending"});
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            log.MakeLog("Added " + f.Name + "! Returning.", true);
        }

        private int CountPackages(int game) {
            log.MakeLog("Checking count of packages database.", true);
            int value;
            string cmdtext = string.Format("SELECT count(*) FROM Processing_Reader where game = '{0}'", game);
            var command = GlobalVariables.DatabaseConnection.CreateCommand(cmdtext);
            value = command.ExecuteScalar<Int32>();
            log.MakeLog("Counted! Returning.", true);
            return value;
        }
    }
}