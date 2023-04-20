using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FindBrokenPackages;
using Sims_CC_Sorter;

namespace CCSorter {
    class ParsePackageFolder {
        string packageExtension = "package";
        public List<FileInfo> allPackages = new List<FileInfo>();
        public List<FileInfo> notPackages = new List<FileInfo>();
        public string[] packageContents;
        public string filename = "";
        public int counter;

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
        public void GetPackageInfo(FileInfo packageFile){
            /*FileStream getInfo = new FileStream(packageFile.FullName, FileMode.OpenOrCreate);
            Console.WriteLine("Opened " + packageFile.Name);
            dynamic packageCont = File.ReadAllLines(packageFile.FullName);
            foreach (dynamic item in packageCont) {
                GetTypeOf(item);
                //Console.Write(item);
            }
            var fileContents = System.IO.File.OpenRead(packageFile);
            filename = fileContents;
            Console.Write(fileContents.GetType);
            FileStream packageContents = File.OpenRead(packageFile.FullName);
            GetPackageContents(packageContents);            
            GetTypeOf(fs);*/
            
        }

        public void FindBrokenPackages (FileInfo package, String logfile, int packageCount, int game){
            FileStream dbpfFile = new FileStream(package.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader readFile = new BinaryReader(dbpfFile);
            StreamWriter putContentsIntoTxt = new StreamWriter(logfile, append: true);
            //Console.WriteLine("Checking " + package.FullName);
            string test;
            test = Encoding.ASCII.GetString(readFile.ReadBytes(4));
            //Console.WriteLine(test);
            var statement = "";
            var time = "";
            if (test != "DBPF") {
                readFile.Close();
                time = DateTime.Now.ToString("h:mm:ss tt");
                statement = time + ": " + package.FullName + " is either not a package or is broken.";
                putContentsIntoTxt.WriteLine(statement);
                putContentsIntoTxt.Close();
                return;
            }

            uint major = readFile.ReadUInt32();
            test = major.ToString();
            
            uint minor = readFile.ReadUInt32();
            test = minor.ToString();

            counter++;

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
                    time = DateTime.Now.ToString("h:mm:ss tt");
                    statement = time + ": " + package.FullName + " is a sims 2 file.";
                    putContentsIntoTxt.WriteLine(statement);
                    putContentsIntoTxt.Close();
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
                    time = DateTime.Now.ToString("h:mm:ss tt");
                    statement = time + ": " + package.FullName + " is a sims 4 file.";
                    putContentsIntoTxt.WriteLine(statement);
                    putContentsIntoTxt.Close();
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
                    time = DateTime.Now.ToString("h:mm:ss tt");
                    statement = time + ": " + package.FullName + " is a sims 3 file.";
                    putContentsIntoTxt.WriteLine(statement);
                    putContentsIntoTxt.Close();
                    var temp = new PackageFiles();
                    temp.Name = package.Name;
                    temp.Location = package.FullName;
                    temp.Number = counter;
                    temp.Version = 3;
                    packageFiles.Add(temp);
                }
            }
        putContentsIntoTxt.Close();
        }        
    }
}