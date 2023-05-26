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
                packagereader.Dispose();
                msPackage.Dispose();
                return;
            }
            
        }

        public void CheckThrough (string input){
            FileInfo pack = new FileInfo(input);
            byte[] file = File.ReadAllBytes(pack.FullName);
            MemoryStream msPackage = new MemoryStream(file);
            BinaryReader packagereader = new BinaryReader(msPackage);
            string test = "";
            test = Encoding.ASCII.GetString(packagereader.ReadBytes(4));
            if (test != "DBPF") {
                var statement = pack.FullName + " is either not a package or is broken.";
                log.MakeLog("Adding broken file to list.", true);
                string qcmd = string.Format("SELECT * FROM AllFiles where Name='{0}'", pack.FullName);
                var fileq = GlobalVariables.DatabaseConnection.Query<AllFiles>(qcmd);
                AllFiles query = fileq[0];
                string qtype = query.Type;
                GlobalVariables.DatabaseConnection.Delete(query);
                GlobalVariables.DatabaseConnection.Insert(new AllFiles{Name = pack.Name, Location = pack.FullName, Type = qtype, Status = "Broken"});
                log.MakeLog(statement, false);
                packagereader.Dispose();
                msPackage.Dispose();
                return;
            } else {
                counter++;
                uint major = packagereader.ReadUInt32();
                log.MakeLog(pack.Name + " has " + major + " as a major.", true);                
                uint minor = packagereader.ReadUInt32();
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
                    string qcmd = string.Format("SELECT * FROM AllFiles where Name='{0}'", pack.FullName);
                    var fileq = GlobalVariables.DatabaseConnection.Query<AllFiles>(qcmd);
                    AllFiles query = fileq[0];
                    string qtype = query.Type;
                    GlobalVariables.DatabaseConnection.Delete(query);
                    GlobalVariables.DatabaseConnection.Insert(new AllFiles{Name = pack.Name, Location = pack.FullName, Type = qtype, Status = "Unidentifiable Version"});
                }
                packagereader.Dispose();
                msPackage.Dispose();
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
            log.MakeLog(string.Format("Adding {0} to database as for Sims {1}", f.Name, game), true);
            
            Containers.Containers.identifiedPackages.Add(new PackageFile{Name = f.Name, Location = f.FullName, Game = game, Broken = false, Status = "Pending"});
            
            log.MakeLog(string.Format("Added {0}! Returning.", f.Name), true);
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