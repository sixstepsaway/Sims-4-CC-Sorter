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
using SimsCCManager.Packages.Containers;
using Newtonsoft.Json;
using SimsCCManager.Packages.Initial;
using SimsCCManager.Packages.Sims4Search;

namespace SimsCCManager.CMD
{     
    class Program { 
        
        static void Main(string[] args)
        {
            GlobalVariables globals = new GlobalVariables();
            TypeListings typeListings = new TypeListings();
            InitialProcessing initial = new InitialProcessing();
            LoggingGlobals log = new LoggingGlobals();
            S4PackageSearch sims4s = new S4PackageSearch();
            ParallelOptions parallelSettings = new ParallelOptions() { MaxDegreeOfParallelism = 200};
            TypeListings.AllTypesS2 = typeListings.createS2TypeList();
            TypeListings.AllTypesS3 = typeListings.createS3TypeList();
            TypeListings.AllTypesS4 = typeListings.createS4TypeList();
            TypeListings.S2BuyFunctionSort = typeListings.createS2buyfunctionsortlist();
            TypeListings.S2BuildFunctionSort = typeListings.createS2buildfunctionsortlist();
            

            

            string folder = "M:\\The Sims 4 (Documents)\\!UnmergedCC\\To Sort\\CURRENT TEST";
            
            globals.Initialize(0, folder);
            log.InitializeLog();

            initial.IdentifyPackages();

            Parallel.ForEach(GlobalVariables.justPackageFiles, parallelSettings, file => 
            {
                initial.FindBrokenPackages(file.FullName);
            });
            
            Parallel.ForEach(GlobalVariables.workingPackageFiles, parallelSettings, file => 
            {
                initial.IdentifyGames(file.Location);
            });

            Parallel.ForEach(GlobalVariables.gamesPackages, parallelSettings, file => 
            {
                initial.IdentifyGames(file.Location);
            });

            foreach (PackageFile package in GlobalVariables.gamesPackages){
                sims4s.SearchS4Packages(package.Location);
            }

            
            foreach (SimsPackage pack in Containers.allSims4Packages){
                log.MakeLog(pack.ToString(), false);
            }
        }
    }
}