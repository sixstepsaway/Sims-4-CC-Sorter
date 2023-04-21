using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ResultsWindow;
using CCSorterControls;
using SSAGlobals;

namespace CCSorter {
    class ParsePackageFolder {
        string packageExtension = "package";
        public List<FileInfo> allPackages = new List<FileInfo>();
        public List<FileInfo> notPackages = new List<FileInfo>();
        public string filename = "";
        public int counter;

        ControlOverview appInputInformation = new ControlOverview();
        LoggingGlobals logGlobals = new LoggingGlobals();
        private BinaryReader packagereader;
        private FileStream dbpfFile;
        

        public void IdentifyPackages(string ModFolder){
            string[] files = Directory.GetFiles(ModFolder, "*." + packageExtension, SearchOption.AllDirectories);
            foreach (string file in files) {
                FileInfo packageFile = new FileInfo(file);
                if (packageExtension.Any(packageFile.Extension.Contains)) {
                    allPackages.Add(packageFile);
                } else {
                    notPackages.Add(packageFile);
                }
            }
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
                logGlobals.MakeLog(statement, logGlobals.debugModeSetting, false);
                return;
            }
        }

        public void IdentifyGames(FileInfo package, int game){     
            FileStream dbpfFile = Packagefs(package);
            BinaryReader packagereader = Packagebr(dbpfFile);
            string test = "";

            uint major = packagereader.ReadUInt32();
            test = major.ToString();
            
            uint minor = packagereader.ReadUInt32();
            test = minor.ToString();

            string statement = "";
            var packageFiles = new List<PackageFiles>();

            if (major is 1 && minor is 1) {
                if (game is 2) {
                    var temp = new PackageFiles();
                    temp.Name = package.Name;
                    temp.Location = package.FullName;
                    temp.Number = counter;
                    temp.Version = 2;
                    packageFiles.Add(temp);
                } else {
                    statement = package.FullName + " is a sims 2 file.";
                    logGlobals.MakeLog(statement, logGlobals.debugModeSetting, false);
                    var temp = new PackageFiles();
                    temp.Name = package.Name;
                    temp.Location = package.FullName;
                    temp.Number = counter;
                    temp.Version = 2;
                    packageFiles.Add(temp);
                }                
            } else if (major is 2 && minor is 1) {
                if (game is 4) {
                    var temp = new PackageFiles();
                    temp.Name = package.Name;
                    temp.Location = package.FullName;
                    temp.Number = counter;
                    temp.Version = 4;
                    packageFiles.Add(temp);
                } else {
                    statement = package.FullName + " is a sims 4 file.";
                    logGlobals.MakeLog(statement, logGlobals.debugModeSetting, false);
                    var temp = new PackageFiles();
                    temp.Name = package.Name;
                    temp.Location = package.FullName;
                    temp.Number = counter;
                    temp.Version = 4;
                    packageFiles.Add(temp);
                }                
            } else if (major is 2 && minor is 0) {
                if (game is 3) {
                    var temp = new PackageFiles();
                    temp.Name = package.Name;
                    temp.Location = package.FullName;
                    temp.Number = counter;
                    temp.Version = 3;
                    packageFiles.Add(temp);
                } else {
                    statement = package.FullName + " is a sims 4 file.";
                    logGlobals.MakeLog(statement, logGlobals.debugModeSetting, false);
                    var temp = new PackageFiles();
                    temp.Name = package.Name;
                    temp.Location = package.FullName;
                    temp.Number = counter;
                    temp.Version = 3;
                    packageFiles.Add(temp);
                }
            } else if (major is 3 && minor is 0) {
                statement = package.FullName + " is a Sim City 5 file.";
                logGlobals.MakeLog(statement, logGlobals.debugModeSetting, false);
                var temp = new PackageFiles();
                temp.Name = package.Name;
                temp.Location = package.FullName;
                temp.Number = counter;
                temp.Version = 12;
                packageFiles.Add(temp);
            }
        }        
    }
}