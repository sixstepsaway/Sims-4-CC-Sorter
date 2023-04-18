using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FindPackages.PackageDiscovery {
    class ParsePackageFolder {
        string packageExtension = "package";
        public List<FileInfo> allPackages = new List<FileInfo>();
        public List<FileInfo> notPackages = new List<FileInfo>();
        public void IdentifyPackages(string ModFolder){
            Console.WriteLine("Looking for packages inside: " + ModFolder);
            string[] files = Directory.GetFiles(ModFolder);
            foreach (string file in files) {
                FileInfo package = new FileInfo(file);
                if (packageExtension.Any(package.Extension.Contains)) {
                    allPackages.Add(package);
                } else {
                    notPackages.Add(package);
                }
            }
        }
    }
}