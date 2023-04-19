using System;
using System.IO;
using FindPackages.PackageDiscovery;


namespace Sims4CCSorter {
    public static class Program {
        static void Main(){
            /*string ModFolder;
            Console.Write("Modfolder location:");
            ModFolder = Console.ReadLine();*/
            string ModFolder = "M:\\The Sims 4 (Documents)\\TESTING FOLDER";
            ParsePackageFolder parseFolder = new ParsePackageFolder();
            string testPackageContents = "I:\\Code\\C#\\PackageContents.txt";
            StreamWriter putContentsIntoTxt = new StreamWriter(testPackageContents, append: true);
            parseFolder.IdentifyPackages(ModFolder);
            /*foreach (FileInfo packageFile in parseFolder.allPackages) {
                Console.WriteLine(packageFile.Name);
                parseFolder.GetPackageInfo(packageFile);           
                putContentsIntoTxt.WriteLine(packageFile.Name);
                foreach (string line in parseFolder.packageContents) {
                    putContentsIntoTxt.WriteLine(line);
                }

            }*/
            Console.WriteLine(parseFolder.allPackages[0].Name);
            parseFolder.GetPackageInfo(parseFolder.allPackages[0]);           
            putContentsIntoTxt.WriteLine(parseFolder.allPackages[0].Name);
            foreach (string line in parseFolder.packageContents) {
                putContentsIntoTxt.WriteLine(line);
            }
            putContentsIntoTxt.Close();    
        }
    }
}
