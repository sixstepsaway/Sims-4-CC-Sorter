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
            parseFolder.IdentifyPackages(ModFolder);
            foreach (FileInfo package in parseFolder.allPackages) {
                Console.WriteLine(package.Name);
                parseFolder.GetPackageInfo(package);
            }
        }
    }
}
