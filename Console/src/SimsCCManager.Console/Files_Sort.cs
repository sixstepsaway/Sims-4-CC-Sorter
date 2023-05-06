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
        
        List<SortingRules> SortingRulesOverview = new List<SortingRules>();
        List<SortingRules> SortingRulesS2 = new List<SortingRules>();
        List<SortingRules> SortingRulesS3 = new List<SortingRules>();
        List<SortingRules> SortingRulesS4 = new List<SortingRules>();
        string SortedFolder = GlobalVariables.ModFolder + "\\_SORTED";
        Methods methods = new Methods();
        
        public void Prep(){
            InitializeSortingRules();
            Methods.MakeFolder(SortedFolder);

            foreach (SimsPackage package in Containers.Containers.allSims2Packages){

            }
            foreach (SimsPackage package in Containers.Containers.allSims3Packages){

            }
            foreach (SimsPackage package in Containers.Containers.allSims4Packages){

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