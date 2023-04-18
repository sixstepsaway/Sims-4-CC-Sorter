using System;
using System.IO;

namespace Sims4CCSorter {
    public static class CCSorter {
        public static string ModFolder = "M:\\The Sims 4 (Documents)\\TESTING FOLDER";
        private static readonly string packageExtension = "*.package";
        static void Main(){
            Console.WriteLine("Looking for packages inside: " + ModFolder);
            try {
                string[] allFiles = Directory.GetFiles(ModFolder);
                string[] files = Directory.GetFiles(ModFolder, packageExtension);
                Console.WriteLine("Files located: {0}.", allFiles.Length);
                Console.WriteLine("Of which are packages: {0}.", files.Length);
                foreach (string file in files) {
                    Console.WriteLine(file);
                }
            }
            catch (Exception e) {
                Console.WriteLine("Hit a snag: {0}", e.ToString());
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