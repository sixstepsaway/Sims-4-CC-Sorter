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
using SimsCCManager.Packages.Sims3Search;
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
            S3PackageSearch sims3s = new S3PackageSearch();
            S4PackageSearch sims4s = new S4PackageSearch();
            ParallelOptions parallelSettings = new ParallelOptions() { MaxDegreeOfParallelism = 2000};
            TypeListings.AllTypesS2 = typeListings.createS2TypeList();
            TypeListings.AllTypesS3 = typeListings.createS3TypeList();
            TypeListings.AllTypesS4 = typeListings.createS4TypeList();
            TypeListings.S2BuyFunctionSort = typeListings.createS2buyfunctionsortlist();
            TypeListings.S2BuildFunctionSort = typeListings.createS2buildfunctionsortlist();
            

            

            string folder = "M:\\The Sims 4 (Documents)\\!UnmergedCC\\To Sort\\CURRENT TEST";
            //string folder = "M:\\The Sims 4 (Documents)\\!UnmergedCC\\To Sort";
            //string folder = "C:\\Program Files (x86)\\Origin Games\\The Sims 4\\";
            globals.Initialize(0, folder);            

            initial.IdentifyPackages();

            Parallel.ForEach(GlobalVariables.justPackageFiles, parallelSettings, file => 
            {
                initial.FindBrokenPackages(file.FullName);
            });
            
            Parallel.ForEach(GlobalVariables.workingPackageFiles, parallelSettings, file => 
            {
                initial.IdentifyGames(file.Location);
            });

            foreach (PackageFile package in GlobalVariables.gamesPackages){
                if (package.Game == 3){
                    //sims3s.SearchS3Packages(package.Location);
                } else if (package.Game == 4){
                    sims4s.SearchS4Packages(package.Location, false);
                }                
            }

            
            foreach (SimsPackage pack in Containers.allSims4Packages){
                log.MakeLog(pack.ToString(), false);
            }
        }
    }
}