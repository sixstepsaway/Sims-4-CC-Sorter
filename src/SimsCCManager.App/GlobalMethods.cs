using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Reflection;
using SimsCCManager.Packages.Containers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Data.SQLite;
using SQLitePCL;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

namespace SSAGlobals {

    [Table("TypeList")]
    public class typeList {
        /// <summary>
        /// For "types", for example Cas Parts or Geometry.
        /// </summary>
        [Column("Description")]
        public string desc {get; set;}
        [Column("TypeID")]
        public string typeID {get; set;}
        [Column("Info")]
        public string info {get; set;}
    }
    public class SortingValues {
        /// <summary>
        /// Unused rn.
        /// </summary>
        string name {set; get;}        
    }

    public class TypeListings {
        /// <summary>
        /// A list of types and function tags and so on for each game. May later be transferred to a database.
        /// </summary>
        JsonSerializer serializer = new JsonSerializer();

        public static List<typeList> AllTypesS2;
        public static List<typeList> AllTypesS3;
        public static List<typeList> AllTypesS4;
        public static ConcurrentBag<typeList> S4BBFunctionTags;
        public static List<FunctionSortList> S2BuyFunctionSort;
        public static List<FunctionSortList> S2BuildFunctionSort;
        public static List<FunctionSortList> S3BuyFunctionSort;
        public static List<FunctionSortList> S3BuildFunctionSort;
        public static List<FunctionSortList> S4BuyFunctionSort;
        public static List<FunctionSortList> S4BuildFunctionSort;
        public static string functionSortsListNm = "All_Functions_Types_Sorts.sqlite";
        public static string functionSortsListLoc = Path.Combine("data\\", functionSortsListNm);
        public static string functionSortsListDB = string.Format("Data Source={0}", functionSortsListLoc);
       
        public List<FunctionSortList> createS2buyfunctionsortlist(){
            List<FunctionSortList> s2fs = new List<FunctionSortList>();
            //seating
            s2fs.Add(new FunctionSortList{flagnum = 0, functionsubsortnum = 1, Category = "Seating", Subcategory = "Dining Room"});
            s2fs.Add(new FunctionSortList{flagnum = 0, functionsubsortnum = 2, Category = "Seating", Subcategory = "Living Room"});
            s2fs.Add(new FunctionSortList{flagnum = 0, functionsubsortnum = 4, Category = "Seating", Subcategory = "Sofas"});
            s2fs.Add(new FunctionSortList{flagnum = 0, functionsubsortnum = 8, Category = "Seating", Subcategory = "Beds"});
            s2fs.Add(new FunctionSortList{flagnum = 0, functionsubsortnum = 16, Category = "Seating", Subcategory = "Recreation"});
            s2fs.Add(new FunctionSortList{flagnum = 0, functionsubsortnum = 32, Category = "Seating", Subcategory = "Unknown I"});
            s2fs.Add(new FunctionSortList{flagnum = 0, functionsubsortnum = 64, Category = "Seating", Subcategory = "Unknown II"});
            s2fs.Add(new FunctionSortList{flagnum = 0, functionsubsortnum = 128, Category = "Seating", Subcategory = "Misc"});
            
            //surfaces
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 1, Category = "Surfaces", Subcategory = "Counters"});
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 2, Category = "Surfaces", Subcategory = "Tables"});
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 4, Category = "Surfaces", Subcategory = "End Tables"});
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 8, Category = "Surfaces", Subcategory = "Desks"});
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 16, Category = "Surfaces", Subcategory = "Coffee Tables"});
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 32, Category = "Surfaces", Subcategory = "Shelves"});
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 64, Category = "Surfaces", Subcategory = "Unknown II"});
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 128, Category = "Surfaces", Subcategory = "Misc"});
            
            //Appliances
            s2fs.Add(new FunctionSortList{flagnum = 2, functionsubsortnum = 1, Category = "Appliances", Subcategory = "Cooking"});
            s2fs.Add(new FunctionSortList{flagnum = 2, functionsubsortnum = 2, Category = "Appliances", Subcategory = "Fridges"});
            s2fs.Add(new FunctionSortList{flagnum = 2, functionsubsortnum = 4, Category = "Appliances", Subcategory = "Small"});
            s2fs.Add(new FunctionSortList{flagnum = 2, functionsubsortnum = 8, Category = "Appliances", Subcategory = "Large"});
            s2fs.Add(new FunctionSortList{flagnum = 2, functionsubsortnum = 16, Category = "Appliances", Subcategory = "Unknown I"});
            s2fs.Add(new FunctionSortList{flagnum = 2, functionsubsortnum = 32, Category = "Appliances", Subcategory = "Unknown II"});
            s2fs.Add(new FunctionSortList{flagnum = 2, functionsubsortnum = 64, Category = "Appliances", Subcategory = "Unknown III"});
            s2fs.Add(new FunctionSortList{flagnum = 2, functionsubsortnum = 128, Category = "Appliances", Subcategory = "Misc"});
            //Electronics
            s2fs.Add(new FunctionSortList{flagnum = 3, functionsubsortnum = 1, Category = "Electronics", Subcategory = "Entertainment"});
            s2fs.Add(new FunctionSortList{flagnum = 3, functionsubsortnum = 2, Category = "Electronics", Subcategory = "TV/Computer"});
            s2fs.Add(new FunctionSortList{flagnum = 3, functionsubsortnum = 4, Category = "Electronics", Subcategory = "Audio"});
            s2fs.Add(new FunctionSortList{flagnum = 3, functionsubsortnum = 8, Category = "Electronics", Subcategory = "Small"});
            s2fs.Add(new FunctionSortList{flagnum = 3, functionsubsortnum = 16, Category = "Electronics", Subcategory = "Unknown I"});
            s2fs.Add(new FunctionSortList{flagnum = 3, functionsubsortnum = 32, Category = "Electronics", Subcategory = "Unknown II"});
            s2fs.Add(new FunctionSortList{flagnum = 3, functionsubsortnum = 64, Category = "Electronics", Subcategory = "Unknown III"});
            s2fs.Add(new FunctionSortList{flagnum = 3, functionsubsortnum = 128, Category = "Electronics", Subcategory = "Misc"});
            //Plumbing
            s2fs.Add(new FunctionSortList{flagnum = 4, functionsubsortnum = 1, Category = "Plumbing", Subcategory = "Toilets"});
            s2fs.Add(new FunctionSortList{flagnum = 4, functionsubsortnum = 2, Category = "Plumbing", Subcategory = "Showers"});
            s2fs.Add(new FunctionSortList{flagnum = 4, functionsubsortnum = 4, Category = "Plumbing", Subcategory = "Sinks"});
            s2fs.Add(new FunctionSortList{flagnum = 4, functionsubsortnum = 8, Category = "Plumbing", Subcategory = "Hot Tubs"});
            s2fs.Add(new FunctionSortList{flagnum = 4, functionsubsortnum = 16, Category = "Plumbing", Subcategory = "Unknown I"});
            s2fs.Add(new FunctionSortList{flagnum = 4, functionsubsortnum = 32, Category = "Plumbing", Subcategory = "Unknown II"});
            s2fs.Add(new FunctionSortList{flagnum = 4, functionsubsortnum = 64, Category = "Plumbing", Subcategory = "Unknown III"});
            s2fs.Add(new FunctionSortList{flagnum = 4, functionsubsortnum = 128, Category = "Plumbing", Subcategory = "Misc"});

            //Decorative
            s2fs.Add(new FunctionSortList{flagnum = 5, functionsubsortnum = 1, Category = "Decorative", Subcategory = "Wall Decorations"});
            s2fs.Add(new FunctionSortList{flagnum = 5, functionsubsortnum = 2, Category = "Decorative", Subcategory = "Sculptures"});
            s2fs.Add(new FunctionSortList{flagnum = 5, functionsubsortnum = 4, Category = "Decorative", Subcategory = "Rugs"});
            s2fs.Add(new FunctionSortList{flagnum = 5, functionsubsortnum = 8, Category = "Decorative", Subcategory = "Plants"});
            s2fs.Add(new FunctionSortList{flagnum = 5, functionsubsortnum = 16, Category = "Decorative", Subcategory = "Mirrors"});
            s2fs.Add(new FunctionSortList{flagnum = 5, functionsubsortnum = 32, Category = "Decorative", Subcategory = "Curtains"});
            s2fs.Add(new FunctionSortList{flagnum = 5, functionsubsortnum = 64, Category = "Decorative", Subcategory = "Unknown II"});
            s2fs.Add(new FunctionSortList{flagnum = 5, functionsubsortnum = 128, Category = "Decorative", Subcategory = "Misc"});

            //General
            s2fs.Add(new FunctionSortList{flagnum = 6, functionsubsortnum = 1, Category = "Misc", Subcategory = "Unknown I"});
            s2fs.Add(new FunctionSortList{flagnum = 6, functionsubsortnum = 2, Category = "Misc", Subcategory = "Dressers"});
            s2fs.Add(new FunctionSortList{flagnum = 6, functionsubsortnum = 4, Category = "Misc", Subcategory = "Unknown II"});
            s2fs.Add(new FunctionSortList{flagnum = 6, functionsubsortnum = 8, Category = "Misc", Subcategory = "Party"});
            s2fs.Add(new FunctionSortList{flagnum = 6, functionsubsortnum = 16, Category = "Misc", Subcategory = "Child"});
            s2fs.Add(new FunctionSortList{flagnum = 6, functionsubsortnum = 32, Category = "Misc", Subcategory = "Cars"});
            s2fs.Add(new FunctionSortList{flagnum = 6, functionsubsortnum = 64, Category = "Misc", Subcategory = "Pets"});
            s2fs.Add(new FunctionSortList{flagnum = 6, functionsubsortnum = 128, Category = "Misc", Subcategory = "Misc"});
            //Lighting
            s2fs.Add(new FunctionSortList{flagnum = 7, functionsubsortnum = 1, Category = "Lighting", Subcategory = "Table Lamps"});
            s2fs.Add(new FunctionSortList{flagnum = 7, functionsubsortnum = 2, Category = "Lighting", Subcategory = "Floor Lamps"});
            s2fs.Add(new FunctionSortList{flagnum = 7, functionsubsortnum = 4, Category = "Lighting", Subcategory = "Wall Lamps"});
            s2fs.Add(new FunctionSortList{flagnum = 7, functionsubsortnum = 8, Category = "Lighting", Subcategory = "Ceiling Lamps"});
            s2fs.Add(new FunctionSortList{flagnum = 7, functionsubsortnum = 16, Category = "Lighting", Subcategory = "Outdoor"});
            s2fs.Add(new FunctionSortList{flagnum = 7, functionsubsortnum = 32, Category = "Lighting", Subcategory = "Unknown I"});
            s2fs.Add(new FunctionSortList{flagnum = 7, functionsubsortnum = 64, Category = "Lighting", Subcategory = "Unknown II"});
            s2fs.Add(new FunctionSortList{flagnum = 7, functionsubsortnum = 128, Category = "Lighting", Subcategory = "Misc"});
            //Hobbies
            s2fs.Add(new FunctionSortList{flagnum = 8, functionsubsortnum = 1, Category = "Hobbies", Subcategory = "Creative"});
            s2fs.Add(new FunctionSortList{flagnum = 8, functionsubsortnum = 2, Category = "Hobbies", Subcategory = "Knowledge"});
            s2fs.Add(new FunctionSortList{flagnum = 8, functionsubsortnum = 4, Category = "Hobbies", Subcategory = "Exercise"});
            s2fs.Add(new FunctionSortList{flagnum = 8, functionsubsortnum = 8, Category = "Hobbies", Subcategory = "Recreation"});
            s2fs.Add(new FunctionSortList{flagnum = 8, functionsubsortnum = 16, Category = "Hobbies", Subcategory = "Unknown I"});
            s2fs.Add(new FunctionSortList{flagnum = 8, functionsubsortnum = 32, Category = "Hobbies", Subcategory = "Unknown II"});
            s2fs.Add(new FunctionSortList{flagnum = 8, functionsubsortnum = 64, Category = "Hobbies", Subcategory = "Unknown III"});
            s2fs.Add(new FunctionSortList{flagnum = 8, functionsubsortnum = 128, Category = "Hobbies", Subcategory = "Misc"});
            //Aspiration Rewards
            s2fs.Add(new FunctionSortList{flagnum = 9, functionsubsortnum = 1, Category = "Aspiration Rewards", Subcategory = "Unknown I"});
            s2fs.Add(new FunctionSortList{flagnum = 9, functionsubsortnum = 2, Category = "Aspiration Rewards", Subcategory = "Unknown II"});
            s2fs.Add(new FunctionSortList{flagnum = 9, functionsubsortnum = 4, Category = "Aspiration Rewards", Subcategory = "Unknown III"});
            s2fs.Add(new FunctionSortList{flagnum = 9, functionsubsortnum = 8, Category = "Aspiration Rewards", Subcategory = "Unknown IV"});
            s2fs.Add(new FunctionSortList{flagnum = 9, functionsubsortnum = 16, Category = "Aspiration Rewards", Subcategory = "Unknown V"});
            s2fs.Add(new FunctionSortList{flagnum = 9, functionsubsortnum = 32, Category = "Aspiration Rewards", Subcategory = "Unknown VI"});
            s2fs.Add(new FunctionSortList{flagnum = 9, functionsubsortnum = 64, Category = "Aspiration Rewards", Subcategory = "Unknown VII"});
            s2fs.Add(new FunctionSortList{flagnum = 9, functionsubsortnum = 128, Category = "Aspiration Rewards", Subcategory = "Unknown VIII"});
            //Career Rewards
            s2fs.Add(new FunctionSortList{flagnum = 10, functionsubsortnum = 1, Category = "Career Rewards", Subcategory = "Unknown I"});
            s2fs.Add(new FunctionSortList{flagnum = 10, functionsubsortnum = 2, Category = "Career Rewards", Subcategory = "Unknown II"});
            s2fs.Add(new FunctionSortList{flagnum = 10, functionsubsortnum = 4, Category = "Career Rewards", Subcategory = "Unknown III"});
            s2fs.Add(new FunctionSortList{flagnum = 10, functionsubsortnum = 8, Category = "Career Rewards", Subcategory = "Unknown IV"});
            s2fs.Add(new FunctionSortList{flagnum = 10, functionsubsortnum = 16, Category = "Career Rewards", Subcategory = "Unknown V"});
            s2fs.Add(new FunctionSortList{flagnum = 10, functionsubsortnum = 32, Category = "Career Rewards", Subcategory = "Unknown VI"});
            s2fs.Add(new FunctionSortList{flagnum = 10, functionsubsortnum = 64, Category = "Career Rewards", Subcategory = "Unknown VII"});
            s2fs.Add(new FunctionSortList{flagnum = 10, functionsubsortnum = 128, Category = "Career Rewards", Subcategory = "Unknown VIII"});
            /*//seating
            s2fs.Add(new FunctionSortList{flagnum = 11, functionsubsortnum = 1, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 11, functionsubsortnum = 2, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 11, functionsubsortnum = 4, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 11, functionsubsortnum = 8, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 11, functionsubsortnum = 16, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 11, functionsubsortnum = 32, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 11, functionsubsortnum = 64, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 11, functionsubsortnum = 128, Category = "Seating", Subcategory = ""});
            //seating
            s2fs.Add(new FunctionSortList{flagnum = 12, functionsubsortnum = 1, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 12, functionsubsortnum = 2, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 12, functionsubsortnum = 4, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 12, functionsubsortnum = 8, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 12, functionsubsortnum = 16, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 12, functionsubsortnum = 32, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 12, functionsubsortnum = 64, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 12, functionsubsortnum = 128, Category = "Seating", Subcategory = ""});
            //seating
            s2fs.Add(new FunctionSortList{flagnum = 13, functionsubsortnum = 1, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 13, functionsubsortnum = 2, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 13, functionsubsortnum = 4, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 13, functionsubsortnum = 8, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 13, functionsubsortnum = 16, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 13, functionsubsortnum = 32, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 13, functionsubsortnum = 64, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 13, functionsubsortnum = 128, Category = "Seating", Subcategory = ""});
            //seating
            s2fs.Add(new FunctionSortList{flagnum = 14, functionsubsortnum = 1, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 14, functionsubsortnum = 2, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 14, functionsubsortnum = 4, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 14, functionsubsortnum = 8, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 14, functionsubsortnum = 16, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 14, functionsubsortnum = 32, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 14, functionsubsortnum = 64, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 14, functionsubsortnum = 128, Category = "Seating", Subcategory = ""});
            //seating
            s2fs.Add(new FunctionSortList{flagnum = 15, functionsubsortnum = 1, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 15, functionsubsortnum = 2, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 15, functionsubsortnum = 4, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 15, functionsubsortnum = 8, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 15, functionsubsortnum = 16, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 15, functionsubsortnum = 32, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 15, functionsubsortnum = 64, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 15, functionsubsortnum = 128, Category = "Seating", Subcategory = ""});
            */
            return s2fs;
        }
        
        public List<FunctionSortList> createS2buildfunctionsortlist(){
            List<FunctionSortList> s2fs = new List<FunctionSortList>();
            s2fs.Add(new FunctionSortList{flagnum = 8, functionsubsortnum = 1, Category = "Door", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 8, functionsubsortnum = 4, Category = "Window", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 8, functionsubsortnum = 100, Category = "Two Story Door", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 8, functionsubsortnum = 2, Category = "Two Story Window", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 8, functionsubsortnum = 10, Category = "Arch", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 20, Category = "Staircase", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 0, functionsubsortnum = 0, Category = "Fireplaces (?)", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 400, Category = "Garage", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 4, functionsubsortnum = 1, Category = "Trees", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 4, functionsubsortnum = 4, Category = "Flowers", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 4, functionsubsortnum = 10, Category = "Gardening", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 4, functionsubsortnum = 2, Category = "Shrubs", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 1000, Category = "Architecture", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 8, Category = "Column", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 100, Category = "Two Story Column", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 200, Category = "Connecting Column", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 40, Category = "Pools", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 8, functionsubsortnum = 8, Category = "Gates", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 800, Category = "Elevator", Subcategory = ""});
            
            

            return s2fs;
        }
        
        /*public List<FunctionSortList> createS3functionsortlist(){
            //List<FunctionSortList> s3fs = new List<FunctionSortList>()            
            //return s2fs;
        }*/

        public static Dictionary<string,string> OverrideList = new Dictionary<string, string>();
   
    }
    

    public class GlobalVariables {
        /// <summary>
        /// Global variables, anything used all over the program that requires easy access, like locations for databases, and global settings like debug mode.
        /// </summary>
        JsonSerializer serializer = new JsonSerializer();
        public static bool consolevr = false;
        public static bool debugMode = true;
        public static bool loadedSaveData = false;
        public static string ModFolder;
        public static string logfile;
        public static bool sortonthego = false;
        
        public static string currentpackage;
        public static int PackageCount = 0;       
        public static int packagesRead = 0;
        public static int packagesReadReading = 0;
        public static string S4_Overrides_All = @"data\S4_Instances.sqlite";
        public static string S4_Overrides_List = @"data\S4_SpecificOverrides.sqlite";
        private static string PackagesCacheLoc = @"Sims CC Manager\data\PackagesCache.sqlite";
        private static string InstancesCacheLoc = @"Sims CC Manager\cache\InstancesCache.sqlite";
        public static string PackagesRead = Path.Combine(LoggingGlobals.mydocs, PackagesCacheLoc); 
        public static string InstancesCache = Path.Combine(LoggingGlobals.mydocs, InstancesCacheLoc);
        public static string PackagesReadDS = string.Format("Data Source={0}", PackagesRead);


        public static SQLite.SQLiteConnection DatabaseConnection;
        public static SQLite.SQLiteConnection S4OverridesConnection;
        public static SQLite.SQLiteConnection S4SpecificOverridesConnection;
        public static SQLite.SQLiteConnection S4FunctionTypesConnection;
        public static SQLite.SQLiteConnection InstancesCacheConnection;
        public static List<OverridesList> S4OverridesList = new List<OverridesList>();
        public static List<SpecificOverrides> S4SpecificOverridesList = new List<SpecificOverrides>();
        public static List<InstancesCacheRecolors> S2InstancesCacheRecolors = new();
        public static List<InstancesCacheRecolors> S3InstancesCacheRecolors = new();
        public static List<InstancesCacheRecolors> S4InstancesCacheRecolors = new();
        public static List<InstancesCacheMeshes> S2InstancesCacheMeshes = new();
        public static List<InstancesCacheMeshes> S3InstancesCacheMeshes = new();
        public static List<InstancesCacheMeshes> S4InstancesCacheMeshes = new();
        public static ObservableCollection<SimsPackage> AddPackages = new();
        public static ObservableCollection<PackageFile> ProcessingReader = new();
        public static ObservableCollection<InstancesRecolorsS4> InstancesRecolorsS4Col = new();
        public static ObservableCollection<InstancesMeshesS4> InstancesMeshesS4Col = new();
        public static ObservableCollection<InstancesRecolorsS2> InstancesRecolorsS2Col = new();
        public static ObservableCollection<InstancesMeshesS2> InstancesMeshesS2Col = new();
        public static ObservableCollection<InstancesRecolorsS3> InstancesRecolorsS3Col = new();
        public static ObservableCollection<InstancesMeshesS3> InstancesMeshesS3Col = new();
        public static ObservableCollection<AllFiles> AllFiles = new();
        
        
        //vars that hold package files 
        public static List<FileInfo> justPackageFiles = new List<FileInfo>();
                    //this one holds every file in the folder that ends with .package
        public static List<FileInfo> notPackageFiles = new List<FileInfo>();
                    //this one holds every file in the folder that DOESN'T end with .package, except for--
        public static List<SimsPackage> ts4scriptFiles = new List<SimsPackage>();                    
                    //this one holds ts4script files
        public static List<SimsPackage> sims2packfiles = new List<SimsPackage>();                    
                    //this one holds sims2pack files
        public static List<SimsPackage> sims3packfiles = new List<SimsPackage>();                    
                    //this one holds sims3pack files
        public static List<PackageFile> workingPackageFiles = new List<PackageFile>();
                    //this one holds all .package files that came back from being tested as not broken
        public static List<SimsPackage> brokenFiles = new List<SimsPackage>();
                    //this one holds the broken packages
        public static List<PackageFile> gamesPackages = new List<PackageFile>();
                    //this one holds all the working packages that have been assigned a game
        public static List<SimsPackage> loadedData = new List<SimsPackage>();
        public static List<string> S4OverrideInstances = new List<string>();
        LoggingGlobals log = new LoggingGlobals();


        public static void Initialize(string modLocation){
            ModFolder = modLocation;
            logfile = modLocation + "\\SimsCCSorter.log";
            StreamWriter putContentsIntoTxt = new StreamWriter(logfile);
            putContentsIntoTxt.Close();
        }

        TypeListings typeListings = new TypeListings();
        
        public void InitializeVariables(){
            
            log.MakeLog("Initializing application.", true);
            TypeListings.S2BuyFunctionSort = typeListings.createS2buyfunctionsortlist();
            log.MakeLog("Created sims 2 buy function sort list.", true);
            TypeListings.S2BuildFunctionSort = typeListings.createS2buildfunctionsortlist();
            log.MakeLog("Created sims 2 build function sort.", true);         
            log.MakeLog("Finished initializing.", true);
        }   

        public void UpdateBBTags(){
           using (StreamWriter file = File.CreateText("data\\s4bbtags.json"))
            {
                serializer.Serialize(file, TypeListings.S4BBFunctionTags);
            } 
        }

        public void ConnectDatabase(bool restart){
            string cs = GlobalVariables.PackagesRead;
            if (restart == true){
                log.MakeLog("Connecting database and restarting.", true);
                if (File.Exists(cs)){
                    log.MakeLog("Database exists! Deleting.", true);
                        try {
                            File.Delete(cs);
                        } catch (Exception e) {
                            Console.WriteLine(e.Message);
                        }
                        
                        log.MakeLog("Now creating database.", true);
                        try {
                            System.Data.SQLite.SQLiteConnection.CreateFile(cs);
                        } catch (System.Data.SQLite.SQLiteException e) {
                            Console.WriteLine(e.Message);
                        }
                        
                    } else {
                        log.MakeLog("No database. Creating.", true);
                        try {
                            System.Data.SQLite.SQLiteConnection.CreateFile(cs);
                        } catch (System.Data.SQLite.SQLiteException e) {
                            Console.WriteLine(e.Message);
                        }
                    }
                } else {
                    log.MakeLog("Connecting database without restarting.", true);
                }   
            log.MakeLog("Connecting to data storage.", true);         
            DatabaseConnection = new SQLite.SQLiteConnection(PackagesRead);
            log.MakeLog("Connecting to Overrides_All.", true);   
            try {
                S4OverridesConnection = new SQLite.SQLiteConnection(S4_Overrides_All);
            } catch (Exception e){
                Console.WriteLine("Caught exception connecting to S4_Overrides_All: " + e.Message);
            }            
            log.MakeLog("Connecting to Overrides_List.", true);   
            try {
                S4SpecificOverridesConnection = new SQLite.SQLiteConnection(S4_Overrides_List);
            } catch (Exception e){
                Console.WriteLine("Caught exception connecting to S4_Overrides_List: " + e.Message);
            }            
            log.MakeLog("Connecting to S4 Function Sort.", true);        
            try {
                S4FunctionTypesConnection = new SQLite.SQLiteConnection(TypeListings.functionSortsListLoc);
            } catch (Exception e){
                Console.WriteLine("Caught exception connecting to Function Sort Lists: " + e.Message);
            }             
            log.MakeLog("Connecting to Instances Cache.", true);    
            if (!File.Exists(InstancesCache)){
                log.MakeLog("Instances Cache doesn't exist, creating.", true); 
                try {
                    System.Data.SQLite.SQLiteConnection.CreateFile(InstancesCache);
                } catch (System.Data.SQLite.SQLiteException e) {
                    Console.WriteLine(e.Message);
                }
                log.MakeLog("Connecting to created cache.", true); 
                try {
                    InstancesCacheConnection = new SQLite.SQLiteConnection(InstancesCache);
                } catch (Exception e){
                    Console.WriteLine("Caught exception connecting to Function Sort Lists: " + e.Message);
                }
                log.MakeLog("Making tables.", true);
                InstancesCacheConnection.CreateTable<InstancesMeshesS2>();
                InstancesCacheConnection.CreateTable<InstancesMeshesS3>();
                InstancesCacheConnection.CreateTable<InstancesMeshesS4>();
                InstancesCacheConnection.CreateTable<InstancesRecolorsS2>();
                InstancesCacheConnection.CreateTable<InstancesRecolorsS3>();
                InstancesCacheConnection.CreateTable<InstancesRecolorsS4>();
                var tables = new List<string>(){"PRAGMA journal_mode=MEMORY",
                    "PRAGMA synchronous=EXTRA",
                    "PRAGMA auto_vacuum=FULL",
                    "PRAGMA journal_size_limit=5000",
                    "PRAGMA default_cache_size=200"
                };
                foreach (string table in tables){
                    log.MakeLog(string.Format("Making table: {0}.", table), true);
                    InstancesCacheConnection.ExecuteScalar<string>(table);
                }                
            } else {
                try {
                    InstancesCacheConnection = new SQLite.SQLiteConnection(InstancesCache);
                } catch (Exception e){
                    Console.WriteLine("Caught exception connecting to Function Sort Lists: " + e.Message);
                } 
            } 

            var overridescountcmd = S4OverridesConnection.CreateCommand("SELECT count(*) FROM Instances");
            var overridescount = overridescountcmd.ExecuteScalar<int>();
            S4OverridesList = new List<OverridesList>(overridescount);
            S4OverridesList = S4OverridesConnection.Query<OverridesList>("SELECT * FROM Instances");
            var specoverridescountcmd = S4SpecificOverridesConnection.CreateCommand("SELECT count(*) FROM Overrides");
            var specovcount = specoverridescountcmd.ExecuteScalar<int>();
            S4SpecificOverridesList = new List<SpecificOverrides>(specovcount);
            S4SpecificOverridesList = S4SpecificOverridesConnection.Query<SpecificOverrides>("SELECT * FROM Overrides");

            GetInstanceData();
        }

        public static void GetInstanceData(){
            lock (S2InstancesCacheRecolors)
            {
                var countcmd = InstancesCacheConnection.CreateCommand("SELECT count(*) FROM Sims2Recolors");
                var count = countcmd.ExecuteScalar<int>();
                S2InstancesCacheRecolors = new List<InstancesCacheRecolors>(count);
                S2InstancesCacheRecolors = InstancesCacheConnection.Query<InstancesCacheRecolors>("SELECT * FROM Sims2Recolors");
            }

            lock (S3InstancesCacheRecolors)
            {
                var countcmd = InstancesCacheConnection.CreateCommand("SELECT count(*) FROM Sims3Recolors");
                var count = countcmd.ExecuteScalar<int>();
                S3InstancesCacheRecolors = InstancesCacheConnection.Query<InstancesCacheRecolors>("SELECT * FROM Sims3Recolors");
            }

            lock(S4InstancesCacheRecolors) 
            {
                var countcmd = InstancesCacheConnection.CreateCommand("SELECT count(*) FROM Sims4Recolors");
                var count = countcmd.ExecuteScalar<int>();
                S4InstancesCacheRecolors = InstancesCacheConnection.Query<InstancesCacheRecolors>("SELECT * FROM Sims4Recolors");
            } 

            lock(S2InstancesCacheMeshes)
            {
                var countcmd = InstancesCacheConnection.CreateCommand("SELECT count(*) FROM Sims2Meshes");
                var count = countcmd.ExecuteScalar<int>();
                S2InstancesCacheMeshes = InstancesCacheConnection.Query<InstancesCacheMeshes>("SELECT * FROM Sims2Meshes");
            }

            lock(S3InstancesCacheMeshes)
            {
                var countcmd = InstancesCacheConnection.CreateCommand("SELECT count(*) FROM Sims3Meshes");
                var count = countcmd.ExecuteScalar<int>();
                S3InstancesCacheMeshes = InstancesCacheConnection.Query<InstancesCacheMeshes>("SELECT * FROM Sims3Meshes");
            }
            
            lock(S4InstancesCacheMeshes)
            {
                var countcmd = InstancesCacheConnection.CreateCommand("SELECT count(*) FROM Sims4Meshes");
                var count = countcmd.ExecuteScalar<int>();
                S4InstancesCacheMeshes = InstancesCacheConnection.Query<InstancesCacheMeshes>("SELECT * FROM Sims4Meshes");
            }
        }
    }

    public class SaveData {
        /// <summary>
        /// Cache locations. May be merged in with Globals now databases are being used.
        /// </summary>
        public static string mydocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);        
        public static string SimsCCManagerFolder = Path.Combine(mydocs, "Sims CC Manager");
        public static string docsDataFolder = Path.Combine(mydocs, "Sims CC Manager\\data");
        public static string mainSaveData = Path.Combine(docsDataFolder, "packagedata.sqlite");
        public static string cacheFolder = Path.Combine(SimsCCManagerFolder, "Cache");
        public static string database = Path.Combine(docsDataFolder, "packagedata.sqlite");
    }

    public class CacheLocations {
        /// <summary>
        /// Holds the location of Sims cache files for future implementation.
        /// </summary>
        public string CacheName {get; set;}
        public string CacheLocation {get; set;}
        public string CacheRename {get; set;}
    }

    public class LoggingGlobals
    {
        /// <summary>
        /// Synchronous log file implementation. 
        /// </summary>
        public static bool firstrunmain = true;
        public static bool firstrundebug = true;
        public static string mydocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string internalLogFolder = Path.Combine(mydocs, "Sims CC Manager\\logs");
        private static string debuglog = Path.Combine(internalLogFolder, "debug.log");
        static ReaderWriterLock locker = new ReaderWriterLock();
        //Function for logging to the logfile set at the start of the program
        public void InitializeLog() {            
            Methods.MakeFolder(SaveData.cacheFolder);
            Methods.MakeFolder(Path.Combine(mydocs, "Sims CC Manager\\data"));
            StreamWriter addToInternalLog = new StreamWriter (debuglog, append: false);
            addToInternalLog.WriteLine("Initializing debug log file.");
            addToInternalLog.Close();
            StreamWriter addToLog = new StreamWriter (GlobalVariables.logfile, append: false);
            addToLog.WriteLine("Initializing log file.");
            addToLog.Close();
        }
        public void MakeLog (string Statement, bool debug, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "")
        {   
            string time = "";
            string statement = "";
            FileInfo filepath = new FileInfo(filePath);
            if (debug) {
                if (GlobalVariables.debugMode == true){
                   try
                    {
                        time = DateTime.Now.ToString("h:mm:ss tt");
                        statement = string.Format("[L{0} | {1}] {2}: {3}", lineNumber, filepath.Name, time, Statement);
                        locker.AcquireWriterLock(int.MaxValue); 
                        System.IO.File.AppendAllLines(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\", ""), debuglog), new[] { statement });
                    }
                    finally
                    {
                        locker.ReleaseWriterLock();
                    } 
                }                
            } else {
                try
                {
                    time = DateTime.Now.ToString("h:mm:ss tt");
                    statement = string.Format("{0}: {1}", time, Statement);
                    locker.AcquireWriterLock(int.MaxValue); 
                    System.IO.File.AppendAllLines(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\", ""), GlobalVariables.logfile), new[] { statement });
                }
                finally
                {
                    locker.ReleaseWriterLock();
                }
            }            
        }
    }

    public class Methods {
        /// <summary>
        /// Repeatedly called methods like making a folder.
        /// </summary>
        public static void MakeFolder (string directory)
        {
            try
            {
                // Determine whether the directory exists.
                if (Directory.Exists(directory))
                {
                    return;
                }

                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(directory);
            }
            catch (Exception e)
            {
                
            }
            finally {}
        }

        public static string FixApostrophesforSQL(string input){
            string output = "";
            string pattern = @"'";
            string replace = @"''";

            output = Regex.Replace(input, pattern, replace);            

            return output;
        }

        public static MemoryStream ReadBytesToFile(string file){
            FileInfo f = new FileInfo(file);
            byte[] bit = new byte[f.Length];
			using (FileStream fsSource = new FileStream(file,
            FileMode.Open, FileAccess.Read))
			{
				for (int w = 0; w < f.Length; w++){
                    bit[w] = (byte)fsSource.ReadByte();
                }                
				MemoryStream stream = new MemoryStream(bit);
				return stream;
			}
		}

		public static MemoryStream ReadBytesToFile(string file, int bytestoread){
			byte[] bit = new byte[bytestoread];
			using (FileStream fsSource = new FileStream(file,
            FileMode.Open, FileAccess.Read))
			{
				for (int w = 0; w < bytestoread; w++){
                    bit[w] = (byte)fsSource.ReadByte();
                }
				MemoryStream stream = new MemoryStream(bit);
				return stream;
			}
		}

        public static byte[] ReadEntryBytes(BinaryReader reader, int memSize){
            byte[] bit = new byte[memSize];
            for (int w = 0; w < memSize; w++){
                bit[w] = reader.ReadByte();
            }
            return bit;
        }
    }


}