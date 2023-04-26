using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections;
using System.Xml;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;
using SSAGlobals;
using SimsCCManager.Packages.Search;
using SimsCCManager.Packages.Containers;

namespace SimsCCManager.CMD
{     
    class Program { 
        static void Main(string[] args)
        {
            GlobalVariables globals = new GlobalVariables();
            TypeListings typeListings = new TypeListings();
            PackageSearch searcher = new PackageSearch();
            TypeListings.AllTypesS2 = typeListings.createS2TypeList();
            TypeListings.AllTypesS3 = typeListings.createS3TypeList();
            TypeListings.AllTypesS4 = typeListings.createS4TypeList();

            //Console.Write("File Location:   ");
            //string file = Console.ReadLine();

            string location = "M:\\The Sims 4 (Documents)\\TESTING FOLDER\\currenttest\\";
            string[] files = Directory.GetFiles(location, "*.package", SearchOption.AllDirectories);

            foreach (string file in files) {
                searcher.SearchS2Packages(file);
            }            

            Console.WriteLine("Packages in array:");
            foreach (SimsPackage pack in Containers.allSims2Packages){
                Console.WriteLine(pack.Title);
                Console.WriteLine(pack.Description);
                Console.WriteLine(pack.Game);
                Console.WriteLine(pack.Location);
            }
        }
    }
}