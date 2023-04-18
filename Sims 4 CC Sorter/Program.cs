using System;
using System.IO;

namespace Sims4CCSorter {
    public static class CCSorter {
        public static string ModFolder = "M:\\The Sims 4 (Documents)\\TESTING FOLDER";
        private static readonly string packageExtension = "package";
        static void Main(){
            Console.WriteLine("Looking for packages inside: " + ModFolder);
            string[] files = Directory.GetFiles(ModFolder);
            foreach (string file in files) {
                FileInfo package = new FileInfo(file);
                if (packageExtension.Any(package.Extension.Contains)) {
                    Console.WriteLine("File " + package.Name + " is a package.");
                } else {
                    Console.WriteLine("File " + package.Name + " is NOT package.");
                }
            }
        }
    }    
}


/*public class Sims4CCSorter {
    // See https://aka.ms/new-console-template for more information
    static string ModFolder = "M:\\The Sims 4 (Documents)\\TESTING FOLDER";
    public static void Main()
    {
        Console.WriteLine("Program initialized.");        
        try
        {
            string[] dirs = Directory.GetFiles(ModFolder);
            Console.WriteLine("Files found: {0}.", dirs.Length);
            foreach (string dir in dirs)
            {
                Console.WriteLine(dir);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("The process failed: {0}", e.ToString());
        }
    }
}*/