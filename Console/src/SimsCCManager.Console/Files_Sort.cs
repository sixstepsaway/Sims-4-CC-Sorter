using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using SSAGlobals;
using SimsCCManager.Packages.Containers;

namespace SimsCCManager.Packages.Sorting
{
    public class SortingRules
    {
        public string Folder {get; set;}
        public string MatchTermType {get; set;}
        public string MatchTerm {get; set;}
    }

    public class Files_Sort
    {
        LoggingGlobals log = new LoggingGlobals();
        
        List<SortingRules> SortingRulesOverview = new List<SortingRules>();
        List<SortingRules> SortingRulesS2 = new List<SortingRules>();
        List<SortingRules> SortingRulesS3 = new List<SortingRules>();
        List<SortingRules> SortingRulesS4 = new List<SortingRules>();
        public static string SortedFolder = GlobalVariables.ModFolder + "\\_SORTED";
        public static string SortedS2Folder = SortedFolder + "\\_Sims2";
        public static string SortedS3Folder = SortedFolder + "\\_Sims3";
        public static string SortedS4Folder = SortedFolder + "\\_Sims4";
        public static string S2DupesFolder = SortedS2Folder + "\\_DUPLICATES";
        public static string S3DupesFolder = SortedS3Folder + "\\_DUPLICATES";
        public static string S4DupesFolder = SortedS4Folder + "\\_DUPLICATES";
        Methods methods = new Methods();
        
        public void Prep(){
            InitializeSortingRules();
            Methods.MakeFolder(SortedFolder);

            foreach (SimsPackage package in Containers.Containers.allSims2Packages){


            }
            foreach (SimsPackage package in Containers.Containers.allSims3Packages){


            }
            foreach (SimsPackage package in Containers.Containers.allSims4Packages){
                if (package.CatalogTags.Exists(x => x.stringval == "Func_OffTheGrid")){
                    string otgfolder = SortedS4Folder + "\\_OTG";
                    string newlocation = SortedS4Folder + "\\_OTG\\" + package.PackageName + ".package";
                    if (!File.Exists(newlocation))
                    {
                        log.MakeLog(package.PackageName + " already exists at new location of " + otgfolder + ". Moving instead to duplicates.", true);
                    }
                } else if ((package.Type == "Object Mesh") || (package.Type == "Object")){

                }
            }
        }



        





        public void InitializeSortingRules(){
            SortingRulesOverview.Add(new SortingRules{ Folder = "\\_Sims2", MatchTermType = "Game", MatchTerm = "2"});
            SortingRulesOverview.Add(new SortingRules{ Folder = "\\_Sims3", MatchTermType = "Game", MatchTerm = "3"});
            SortingRulesOverview.Add(new SortingRules{ Folder = "\\_Sims4", MatchTermType = "Game", MatchTerm = "4"});
            SortingRulesOverview.Add(new SortingRules{ Folder = "\\_BROKEN", MatchTermType = "Broken", MatchTerm = "1"});
            SortingRulesOverview.Add(new SortingRules{ Folder = "\\_OTHERGAMES", MatchTermType = "Game", MatchTerm = "12"});
            SortingRulesOverview.Add(new SortingRules{ Folder = "\\_Sims4\\_MERGED", MatchTermType = "Type", MatchTerm = "Merged Package"});

        }

    }
}