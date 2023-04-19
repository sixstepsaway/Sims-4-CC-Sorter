using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace FindPackages.PackageDiscovery {
    class ParsePackageFolder {
        string packageExtension = "package";
        public List<FileInfo> allPackages = new List<FileInfo>();
        public List<FileInfo> notPackages = new List<FileInfo>();
        public string[] packageContents;
        public string filename = "";


        public void IdentifyPackages(string ModFolder){
            Console.WriteLine("Looking for packages inside: " + ModFolder);
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

        public void FindBrokenPackages (FileInfo package, String logfile, int packagecount, int game){
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
                Console.WriteLine(statement);
                putContentsIntoTxt.WriteLine(statement);
                putContentsIntoTxt.Close();
                return;
            }

            uint major = readFile.ReadUInt32();
            test = major.ToString();
            //Console.WriteLine(test);
            
            uint minor = readFile.ReadUInt32();
            test = minor.ToString();
            //Console.WriteLine(test);

            

            if (major is 1 && minor is 1) {
                if (game is 2) {
                    //
                } else {
                    time = DateTime.Now.ToString("h:mm:ss tt");
                    statement = time + ": " + package.FullName + " is a sims 2 file.";      
                    Console.WriteLine(statement);
                    putContentsIntoTxt.WriteLine(statement);
                    putContentsIntoTxt.Close();
                }                
            } else if (major is 2 && minor is 1) {
                if (game is 4) {
                    //
                } else {
                    time = DateTime.Now.ToString("h:mm:ss tt");
                    statement = time + ": " + package.Name + " is a sims 4 file.";      
                    Console.WriteLine(statement);
                    putContentsIntoTxt.WriteLine(statement);
                    putContentsIntoTxt.Close();
                }                
            } else if (major is 2 && minor is 0) {
                if (game is 3) {
                    //
                } else {
                    time = DateTime.Now.ToString("h:mm:ss tt");
                    statement = time + ": " + package.FullName + " is a sims 3 file.";      
                    Console.WriteLine(statement);
                    putContentsIntoTxt.WriteLine(statement);
                    putContentsIntoTxt.Close();
                }                
            }
            putContentsIntoTxt.Close();
        }
    }
}