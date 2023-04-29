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
using Newtonsoft.Json;

namespace SimsCCManager.CMD
{     
    class Program { 
        
        static void Main(string[] args)
        {
            GlobalVariables globals = new GlobalVariables();
            TypeListings typeListings = new TypeListings();
            PackageSearch searcher = new PackageSearch();
            LoggingGlobals log = new LoggingGlobals();
            ParallelOptions parallelSettings = new ParallelOptions() { MaxDegreeOfParallelism = 200};
            TypeListings.AllTypesS2 = typeListings.createS2TypeList();
            TypeListings.AllTypesS3 = typeListings.createS3TypeList();
            TypeListings.AllTypesS4 = typeListings.createS4TypeList();
            TypeListings.S2BuyFunctionSort = typeListings.createS2buyfunctionsortlist();
            TypeListings.S2BuildFunctionSort = typeListings.createS2buildfunctionsortlist();
            

            //Console.Write("File Location:   ");
            //string file = Console.ReadLine();

            log.InitializeLog();

            string location = "M:\\The Sims 4 (Documents)\\TESTING FOLDER\\currenttest\\";
            string[] files = Directory.GetFiles(location, "*.package", SearchOption.AllDirectories);
            
            var sw = Stopwatch.StartNew(); 
            
            GlobalVariables.PackageCount = files.Length;
            if (GlobalVariables.debugMode == true)
            {
                foreach (string file in files) {
                    searcher.SearchS2Packages(file);
                }
            }
            else 
            {
                Parallel.ForEach(files, parallelSettings, file => 
                {
                    searcher.SearchS2Packages(file);
                });
            }
            sw.Stop();
            log.MakeLog("Processing took " + sw.Elapsed.TotalSeconds.ToString("#,##0.00 'seconds'"), true);
            

            //searcher.SearchS2Packages("M:\\The Sims 4 (Documents)\\TESTING FOLDER\\currenttest\\2f3d15de07c65f913156f5198f226ab60001.package");
            
            foreach (SimsPackage pack in Containers.allSims2Packages){
                log.MakeLog(pack.ToString(), false);
            }
        }
    }
}