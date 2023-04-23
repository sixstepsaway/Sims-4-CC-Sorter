using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CCSorter.Controls;
using SSAGlobals;
using ResultsWindow;


namespace CCSorter.Funcs {
    class ProcessSelectedFolder {
        
        string packageExtension = "package";
        public static List<FileInfo> allPackages = new List<FileInfo>();
        public static List<FileInfo> notPackages = new List<FileInfo>();
        public string filename = "";
        public string statement = "";
        public int counter = 0;
        LoggingGlobals loggingGlobals = new LoggingGlobals();
        
        
        public void IdentifyPackages(){
            statement = "Running Identify Packages.";
            loggingGlobals.MakeLog(statement, true);
            string[] files = Directory.GetFiles(GlobalVariables.ModFolder, "*." + packageExtension, SearchOption.AllDirectories);
            foreach (string file in files) {
                FileInfo packageFile = new FileInfo(file);
                statement = "Found " + packageFile.FullName;
                loggingGlobals.MakeLog(statement, true);
                if (packageExtension.Any(packageFile.Extension.Contains)) {
                    statement = "This file is a package file.";
                    loggingGlobals.MakeLog(statement, true);
                    allPackages.Add(packageFile);
                    statement = "Items in allPackages array: " + allPackages.Count;
                    loggingGlobals.MakeLog(statement, true);
                } else {
                    statement = "This file is not a package file.";
                    loggingGlobals.MakeLog(statement, true);
                    notPackages.Add(packageFile);
                    statement = "Items in notPackages array: " + allPackages.Count;
                    loggingGlobals.MakeLog(statement, true);
                }
            }
            statement = "Checked all packages, returning.";
            loggingGlobals.MakeLog(statement, true);
        }

        void GetTypeOf<T>(T LineType) {
            Console.WriteLine(typeof(T));
        }

        private FileStream Packagefs(FileInfo package)
        {
            FileStream dbpfFile = new FileStream(package.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
            return dbpfFile;
        }

        private BinaryReader Packagebr(FileStream dbpfFile)
        {
            BinaryReader packagereader = new BinaryReader(dbpfFile);
            return packagereader;
        }


        public void FindBrokenPackages (FileInfo package){
            FileStream dbpfFile = Packagefs(package);
            BinaryReader packagereader = Packagebr(dbpfFile);
            string test = "";
            //Console.WriteLine("Checking " + package.FullName);
            test = Encoding.ASCII.GetString(packagereader.ReadBytes(4));
            if (test != "DBPF") {
                packagereader.Close();
                var statement = package.FullName + " is either not a package or is broken.";
                var temp = new PackageFile();
                    temp.Name = package.Name;
                    temp.Location = package.FullName;
                    temp.Number = counter;
                    temp.Version = 0;
                    temp.Broken = true;
                    GlobalVariables.packageFiles.Add(temp);
                loggingGlobals.MakeLog(statement, false);
                return;
            }
        }

        public void IdentifyGames(FileInfo package){ 
            string statement = "";
            statement = "Identifying which game this package file is for.";
            loggingGlobals.MakeLog(statement, true);            
            counter++;    
            FileStream dbpfFile = Packagefs(package);
            statement = "Package #" + counter + " - Created filestream for package.";
            loggingGlobals.MakeLog(statement, true);
            BinaryReader packagereader = Packagebr(dbpfFile);
            statement = "Package #" + counter + " - Created binaryreader for package.";
            loggingGlobals.MakeLog(statement, true);
            string test = "";

            test = Encoding.ASCII.GetString(packagereader.ReadBytes(4));

            uint major = packagereader.ReadUInt32();
            test = major.ToString();
            statement = package.Name + " has " + major + " as a major.";
            loggingGlobals.MakeLog(statement, true);
            
            uint minor = packagereader.ReadUInt32();
            test = minor.ToString();
            statement = package.Name + " has " + minor + " as a minor.";
            loggingGlobals.MakeLog(statement, true);
            var temp = new PackageFile();
            if (major is 1 && minor is 1) {
                if (GlobalVariables.gameVer is 2) {
                    statement = "[DEBUG] " + package.FullName + " is a sims 2 file.";
                    loggingGlobals.MakeLog(statement, true);                    
                    temp.Name = package.Name;
                    temp.Location = package.FullName;
                    temp.Number = counter;
                    temp.Version = 2;
                    temp.Broken = false;
                    GlobalVariables.packageFiles.Add(temp);
                    temp = null;
                } else {
                    statement = package.FullName + " is a sims 2 file.";
                    loggingGlobals.MakeLog(statement, false);
                    temp.Name = package.Name;
                    temp.Location = package.FullName;
                    temp.Number = counter;
                    temp.Version = 2;
                    temp.Broken = false;
                    GlobalVariables.packageFiles.Add(temp);
                    temp = null;
                }                
            } else if (major is 2 && minor is 1) {
                if (GlobalVariables.gameVer is 4) {
                    statement = "[DEBUG] " + package.FullName + " is a sims 4 file.";
                    loggingGlobals.MakeLog(statement, true);
                    temp.Name = package.Name;
                    temp.Location = package.FullName;
                    temp.Number = counter;
                    temp.Version = 4;
                    temp.Broken = false;
                    GlobalVariables.packageFiles.Add(temp);
                    temp = null;
                } else {
                    statement = package.FullName + " is a sims 4 file.";
                    loggingGlobals.MakeLog(statement, false);
                    temp.Name = package.Name;
                    temp.Location = package.FullName;
                    temp.Number = counter;
                    temp.Version = 4;
                    temp.Broken = false;
                    GlobalVariables.packageFiles.Add(temp);
                    temp = null;
                }                
            } else if (major is 2 && minor is 0) {
                if (GlobalVariables.gameVer is 3) {
                    statement = "[DEBUG] " + package.FullName + " is a sims 3 file.";
                    loggingGlobals.MakeLog(statement, true);
                    temp.Name = package.Name;
                    temp.Location = package.FullName;
                    temp.Number = counter;
                    temp.Version = 3;
                    temp.Broken = false;
                    GlobalVariables.packageFiles.Add(temp);
                    temp = null;
                } else {
                    statement = package.FullName + " is a sims 4 file.";
                    loggingGlobals.MakeLog(statement, false);
                    temp.Name = package.Name;
                    temp.Location = package.FullName;
                    temp.Number = counter;
                    temp.Version = 3;
                    temp.Broken = false;
                    GlobalVariables.packageFiles.Add(temp);
                    temp = null;
                }
            } else if (major is 3 && minor is 0) {
                statement = package.FullName + " is a Sim City 5 file.";
                loggingGlobals.MakeLog(statement, false);
                temp.Name = package.Name;
                temp.Location = package.FullName;
                temp.Number = counter;
                temp.Version = 12;
                temp.Broken = false;
                GlobalVariables.packageFiles.Add(temp);
                temp = null;
            } else { 
                statement = package.FullName + " was unidentifiable.";
                loggingGlobals.MakeLog(statement, false);
                temp = null;
            }
        }
    }
}