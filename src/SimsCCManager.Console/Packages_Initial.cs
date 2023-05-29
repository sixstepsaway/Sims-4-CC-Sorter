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

        public void CheckThrough (string input){
            FileInfo pack = new FileInfo(input);
            MemoryStream msPackage = Methods.ReadBytesToFile(input);
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
                if ((major is 1 && minor is 1) || (major is 1 && minor is 2)) {
                    log.MakeLog(pack.FullName + " is a sims 2 file.", false);
                    PtoDb(pack, 2);
                    try {
                      s2s.SearchS2Packages(packagereader, pack, minor);
                    } catch (Exception e) {
                        log.MakeLog(string.Format("Caught exception reading Sims 2 package {0}, Exception: {1}", pack.Name, e), true);
                    }
                    
                } else if (major is 2 && minor is 0) {
                    log.MakeLog(pack.FullName + " is a sims 3 file.", false);
                    PtoDb(pack, 3);
                    //s3s.SearchS3Packages(packagereader, pack);
                } else if (major is 2 && minor is 1) {
                    log.MakeLog(pack.FullName + " is a sims 4 file.", false);
                    PtoDb(pack, 4);
                    try {
                        s4s.SearchS4Packages(packagereader, pack);
                    } catch (Exception e) {
                        log.MakeLog(string.Format("Caught exception reading Sims 4 package {0}, Exception: {1}", pack.Name, e), true);
                    }
                    
                } else if (major is 3 && minor is 0) {
                    log.MakeLog(pack.FullName + " is a Sim City 5 file.", false);
                    AllFiles af = new AllFiles();
                    af.Location = pack.FullName;
                    af.Name = pack.Name;
                    af.Type = "package";
                    af.Status = "SimCity 5 Package";
                    GlobalVariables.DatabaseConnection.Update(af);
                } else {
                    log.MakeLog(pack.FullName + " was unidentifiable.", false);
                    AllFiles af = new AllFiles();
                    af.Location = pack.FullName;
                    af.Name = pack.Name;
                    af.Type = "package";
                    af.Status = "Unidentifiable version";
                    GlobalVariables.DatabaseConnection.Update(af);
                }
                packagereader.Dispose();
                msPackage.Dispose();
            }
        }
        

        private void PtoDb(FileInfo f, int game){
            log.MakeLog(string.Format("Adding {0} to database as for Sims {1}", f.Name, game), true);
            
            GlobalVariables.DatabaseConnection.Insert(new PackageFile{Name = f.Name, Location = f.FullName, Game = game, Broken = false, Status = "Processing"});

            //Containers.Containers.identifiedPackages.Add(new PackageFile{Name = f.Name, Location = f.FullName, Game = game, Broken = false, Status = "Pending"});
            
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