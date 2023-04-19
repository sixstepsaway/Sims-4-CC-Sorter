using System;
using System.IO;
using FindPackages.PackageDiscovery;


namespace Sims4CCSorter {
    public static class Program {
        static void Main(){
            string gameChoice = "";
            string ModFolder = "";
            string logfile = "";
            int gameInt = 0;
            Console.Write("Which sims game are you checking? (Single number only please; 2 for sims 2, 3 for sims 3 etc)");
            gameChoice = Console.ReadLine();
            Console.Write("Modfolder location:");
            ModFolder = Console.ReadLine();
            Console.Write("Output info location: (.txt file is best, no need to make one, something like 'C:\\logfile.txt' will work fine.)");
            logfile = Console.ReadLine();
            //string ModFolder = "M:\\The Sims 4 (Documents)\\TESTING FOLDER";
            ParsePackageFolder parseFolder = new ParsePackageFolder();
            parseFolder.IdentifyPackages(ModFolder);
            //parseFolder.ReadPackage(parseFolder.allPackages[20]);
            //Random rnd = new Random();
            /*int max = parseFolder.allPackages.Count;            
            int randomNumber = rnd.Next(5, 20);
            for (int i = 0; i < randomNumber; i++) {
                int randomPackage = rnd.Next(0, max);
                Console.Write(randomPackage);
                //Console.Write(i);
                parseFolder.ReadPackage(parseFolder.allPackages[randomPackage]);
            }*/
            StreamWriter putContentsIntoTxt = new StreamWriter(logfile);
            putContentsIntoTxt.Close();
            putContentsIntoTxt = new StreamWriter(logfile, append: true);
            var statement = "";
            if (gameChoice is "2") {
                gameInt = 2;
            } else if (gameChoice is "3") {
                gameInt = 3;
            } else if (gameChoice is "4") {
                gameInt = 4;
            } else {
                statement = "Unidentified game chosen. Exiting.";
                putContentsIntoTxt.WriteLine(statement);
                Console.WriteLine("Unidentified game chosen. Hit any key to exit.");
                putContentsIntoTxt.Close();
                Console.ReadKey();
                Environment.Exit(0);
            }
            
            var packagecount = parseFolder.allPackages.Count;
            statement = "Checking " + packagecount + " package files. Larger numbers will take a while and I don't know how to do progress bars yet, so please be patient.";            
            Console.WriteLine(statement);
            putContentsIntoTxt.WriteLine(statement);
            putContentsIntoTxt.Close();
            
            foreach (FileInfo item in parseFolder.allPackages) {
                parseFolder.FindBrokenPackages(item, logfile, packagecount, gameInt);
            }
            
        }
    }
}
