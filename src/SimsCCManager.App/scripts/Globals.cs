using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Godot;
using Microsoft.Win32;
using SimsCCManager.Containers;
using SimsCCManager.Debugging;
using SimsCCManager.PackageReaders;
using SimsCCManager.Packages.Containers;
using SimsCCManager.Settings.Loaded;
using static SimsCCManager.PackageReaders.Sims2PackageReader;

namespace SimsCCManager.Globals
{
    public class GlobalVariables
    {
        public static string AppName = "Sims CC Manager";
        public static string MyDocuments = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        public static string AppFolder = Path.Combine(MyDocuments, AppName);
        public static bool DebugMode = true;
        public static bool LoggedIn = false;
        public static string InstallDirectory = System.Environment.CurrentDirectory;
        public static string ffmpeg = Path.Combine(InstallDirectory, "tools\\ffmpeg\\bin\\ffmpeg.exe");
        public static string imagemagick = Path.Combine(InstallDirectory, "tools\\imagemagick\\magick.exe");
        //public static string AppDataFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
        //public static string AppldataFolder = Path.Combine(AppDataFolder, "Sims CC Manager");
        public static string SettingsFile = Path.Combine(AppFolder, "Settings.ini");
        public static string tempfolder = Path.Combine(AppFolder, "temp");
        public static string logfolder = Path.Combine(AppFolder, "logs");

        public static Instance CurrentInstance = new();
        public static GameInstanceBase thisinstance;
        public static List<string> SimsFileExtensions = new(){
            ".package",
            ".sims3pack",
            ".sims2pack",
            "ts4script"
        };

        public static List<string> Sims2Exes = new(){
            "Sims2EP9",
            "Sims2EP9RPC"
        };
        public static List<string> Sims3Exes = new(){
            "TS3W",
            "TS3"
        };
        public static List<string> Sims4Exes = new(){
            "TS4_DX9_x64",
            "TS4_x64"
        };
        
        public static List<EntryType> Sims2EntryTypes = new()
        {
            new EntryType(){ Tag = "2ARY", TypeID = "6B943B43", Description = "2D Array" },
            new EntryType(){ Tag = "3ARY", TypeID = "2A51171B", Description = "3D Array" },
            new EntryType(){ Tag = "5DS", TypeID = "AC06A676", Description = "Lighting (Draw State Light)" },
            new EntryType(){ Tag = "5EL", TypeID = "6A97042F", Description = "Lighting (Environment Cube Light)" },
            new EntryType(){ Tag = "5LF", TypeID = "AC06A66F", Description = "Lighting (Linear Fog Light)" },
            new EntryType(){ Tag = "5SC", TypeID = "25232B11", Description = "Scene Node" },
            new EntryType(){ Tag = "ANIM", TypeID = "FB00791E", Description = "Animation Resource" },
            new EntryType(){ Tag = "BCON", TypeID = "42434F4E", Description = "Behaviour Constant" },
            new EntryType(){ Tag = "BHAV", TypeID = "42484156", Description = "Behaviour Function" },
            new EntryType(){ Tag = "BMP", TypeID = "424D505F", Description = "Bitmaps" },
            new EntryType(){ Tag = "BMP", TypeID = "856DDBAC", Description = "Bitmaps" },
            new EntryType(){ Tag = "CATS", TypeID = "43415453", Description = "Catalog String" },
            new EntryType(){ Tag = "CIGE", TypeID = "43494745", Description = "Image Link" },
            new EntryType(){ Tag = "CINE", TypeID = "4D51F042", Description = "Cinematic Scenes" },
            new EntryType(){ Tag = "CREG", TypeID = "CDB467B8", Description = "Content Registry" },
            new EntryType(){ Tag = "CRES", TypeID = "E519C933", Description = "Resource Node" },
            new EntryType(){ Tag = "CTSS", TypeID = "43545353", Description = "Catalog Description" },
            new EntryType(){ Tag = "DGRP", TypeID = "44475250", Description = "Drawgroup" },
            new EntryType(){ Tag = "DIR", TypeID = "E86B1EEF", Description = "Directory of Compressed Files" },
            new EntryType(){ Tag = "FACE", TypeID = "46414345", Description = "Face Properties" },
            new EntryType(){ Tag = "FAMh", TypeID = "46414D68", Description = "Family Data" },
            new EntryType(){ Tag = "FAMI", TypeID = "46414D49", Description = "Family Information" },
            new EntryType(){ Tag = "FAMt", TypeID = "8C870743", Description = "Family Ties" },
            new EntryType(){ Tag = "FCNS", TypeID = "46434E53", Description = "Global Tuning Values" },
            new EntryType(){ Tag = "FPL", TypeID = "AB4BA572", Description = "Fence Post Layer" },
            new EntryType(){ Tag = "FWAV", TypeID = "46574156", Description = "Audio Reference" },
            new EntryType(){ Tag = "FX", TypeID = "EA5118B0", Description = "Effects Resource Tree" },
            new EntryType(){ Tag = "GLOB", TypeID = "474C4F42", Description = "Glabal Data" },
            new EntryType(){ Tag = "GMDC", TypeID = "AC4F8687", Description = "Geometric Data Container" },
            new EntryType(){ Tag = "GMND", TypeID = "7BA3838C", Description = "Geometric Node" },
            new EntryType(){ Tag = "GZPS", TypeID = "EBCF3E27", Description = "Property Set" },
            new EntryType(){ Tag = "HLS", TypeID = "7B1ACFCD", Description = "Hitlist (TS2 format)" },
            new EntryType(){ Tag = "HOUS", TypeID = "484F5553", Description = "House Data" },
            new EntryType(){ Tag = "JFIF", TypeID = "4D533EDD", Description = "JPEG/JFIF Image" },
            new EntryType(){ Tag = "JFIF", TypeID = "856DDBAC", Description = "JPEG/JFIF Image" },
            new EntryType(){ Tag = "JFIF", TypeID = "8C3CE95A", Description = "JPEG/JFIF Image" },
            new EntryType(){ Tag = "JFIF", TypeID = "0C7E9A76", Description = "JPEG/JFIF Image" },
            new EntryType(){ Tag = "LDEF", TypeID = "0BF999E7", Description = "Lot or Tutorial Description" },
            new EntryType(){ Tag = "LGHT", TypeID = "C9C81B9B", Description = "Lighting (Ambient Light)" },
            new EntryType(){ Tag = "LGHT", TypeID = "C9C81BA3", Description = "Lighting (Directional Light)" },
            new EntryType(){ Tag = "LGHT", TypeID = "C9C81BA9", Description = "Lighting (Point Light)" },
            new EntryType(){ Tag = "LGHT", TypeID = "C9C81BAD", Description = "Lighting (Spot Light)" },
            new EntryType(){ Tag = "LIFO", TypeID = "ED534136", Description = "Level Information" },
            new EntryType(){ Tag = "LOT", TypeID = "6C589723", Description = "Lot Definition" },
            new EntryType(){ Tag = "LTTX", TypeID = "4B58975B", Description = "Lot Texture" },
            new EntryType(){ Tag = "LxNR", TypeID = "CCCEF852", Description = "Facial Structure" },
            new EntryType(){ Tag = "MATSHAD", TypeID = "CD7FE87A", Description = "Maxis Material Shader" },
            new EntryType(){ Tag = "MMAT", TypeID = "4C697E5A", Description = "Material Override" },
            new EntryType(){ Tag = "MOBJT", TypeID = "6F626A74", Description = "Main Lot Objects" },
            new EntryType(){ Tag = "MP3", TypeID = "2026960B", Description = "MP3 Audio" },
            new EntryType(){ Tag = "NGBH", TypeID = "4E474248", Description = "Neighborhood Data" },
            new EntryType(){ Tag = "NHTG", TypeID = "ABCB5DA4", Description = "Neighbourhood Terrain Geometry" },
            new EntryType(){ Tag = "NHTR", TypeID = "ABD0DC63", Description = "Neighborhood Terrain" },
            new EntryType(){ Tag = "NHVW", TypeID = "EC44BDDC", Description = "Neighborhood View" },
            new EntryType(){ Tag = "NID", TypeID = "AC8A7A2E", Description = "Neighbourhood ID" },
            new EntryType(){ Tag = "NMAP", TypeID = "4E6D6150", Description = "Name Map" },
            new EntryType(){ Tag = "NREF", TypeID = "4E524546", Description = "Name Reference" },
            new EntryType(){ Tag = "OBJD", TypeID = "4F424A44", Description = "Object Data" },
            new EntryType(){ Tag = "OBJf", TypeID = "4F424A66", Description = "Object Functions" },
            new EntryType(){ Tag = "ObJM", TypeID = "4F626A4D", Description = "Object Metadata" },
            new EntryType(){ Tag = "OBJT", TypeID = "FA1C39F7", Description = "Singular Lot Object" },
            new EntryType(){ Tag = "OBMI", TypeID = "4F626A4D", Description = "Object Metadata Imposter" },
            new EntryType(){ Tag = "PALT", TypeID = "50414C54", Description = "Image Color Palette" },
            new EntryType(){ Tag = "PDAT", TypeID = "AACE2EFB", Description = "Person Data (Formerly SDSC/SINF/SDAT)" },
            new EntryType(){ Tag = "PERS", TypeID = "50455253", Description = "Person Status" },
            new EntryType(){ Tag = "PMAP", TypeID = "8CC0A14B", Description = "Predictive Map" },
            new EntryType(){ Tag = "PNG", TypeID = "856DDBAC", Description = "PNG Image" },
            new EntryType(){ Tag = "POOL", TypeID = "0C900FDB", Description = "Pool Surface" },
            new EntryType(){ Tag = "Popups", TypeID = "2C310F46", Description = "Unknown" },
            new EntryType(){ Tag = "POSI", TypeID = "504F5349", Description = "Edith Positional Information (deprecated)" },
            new EntryType(){ Tag = "XFLR", TypeID = "4DCADB7E", Description = "Terrain Texture" },
            new EntryType(){ Tag = "PTBP", TypeID = "50544250", Description = "Package Toolkit" },
            new EntryType(){ Tag = "ROOF", TypeID = "AB9406AA", Description = "Roof" },
            new EntryType(){ Tag = "SFX", TypeID = "8DB5E4C2", Description = "Sound Effects" },
            new EntryType(){ Tag = "SHPE", TypeID = "FC6EB1F7", Description = "Shape" },
            new EntryType(){ Tag = "SIMI", TypeID = "53494D49", Description = "Sim Information" },
            new EntryType(){ Tag = "SKIN", TypeID = "AC506764", Description = "Sim Outfits" },
            new EntryType(){ Tag = "SLOT", TypeID = "534C4F54", Description = "Object Slot" },
            new EntryType(){ Tag = "SMAP", TypeID = "CAC4FC40", Description = "String Map" },
            new EntryType(){ Tag = "SPR2", TypeID = "53505232", Description = "Sprites" },
            new EntryType(){ Tag = "SPX1", TypeID = "2026960B", Description = "SPX Speech" },
            new EntryType(){ Tag = "SREL", TypeID = "CC364C2A", Description = "Sim Relations" },
            new EntryType(){ Tag = "STR#", TypeID = "53545223", Description = "Text String" },
            new EntryType(){ Tag = "STXR", TypeID = "ACE46235", Description = "Surface Texture" },
            new EntryType(){ Tag = "SWAF", TypeID = "CD95548E", Description = "Sim Wants and Fears" },
            new EntryType(){ Tag = "TATT", TypeID = "54415454", Description = "Tree Attributes" },
            new EntryType(){ Tag = "TGA", TypeID = "856DDBAC", Description = "Targa Image" },
            new EntryType(){ Tag = "TMAP", TypeID = "4B58975B", Description = "Lot or Terrain Texture Map" },
            new EntryType(){ Tag = "TPRP", TypeID = "54505250", Description = "Edith SimAntics Behavior Labels" },
            new EntryType(){ Tag = "TRCN", TypeID = "5452434E", Description = "Behavior Constant Labels" },
            new EntryType(){ Tag = "TREE", TypeID = "54524545", Description = "Tree Data" },
            new EntryType(){ Tag = "TSSG", TypeID = "BA353CE1", Description = "The Sims SG System" },
            new EntryType(){ Tag = "TTAB", TypeID = "54544142", Description = "Pie Menu Functions" },
            new EntryType(){ Tag = "TTAs", TypeID = "54544173", Description = "Pie Menu Strings" },
            new EntryType(){ Tag = "TXMT", TypeID = "49596978", Description = "Material Definitions" },
            new EntryType(){ Tag = "TXTR", TypeID = "1C4A276C", Description = "Texture" },
            new EntryType(){ Tag = "UI", TypeID = "00000000", Description = "User Interface" },
            new EntryType(){ Tag = "VERT", TypeID = "CB4387A1", Description = "Vertex Layer" },
            new EntryType(){ Tag = "WFR", TypeID = "CD95548E", Description = "Wants and Fears" },
            new EntryType(){ Tag = "WGRA", TypeID = "0A284D0B", Description = "Wall Graph" },
            new EntryType(){ Tag = "WLL", TypeID = "8A84D7B0", Description = "Wall Layer" },
            new EntryType(){ Tag = "WRLD", TypeID = "49FF7D76", Description = "World Database" },
            new EntryType(){ Tag = "WTHR", TypeID = "B21BE28B", Description = "Weather Info" },
            new EntryType(){ Tag = "XA", TypeID = "2026960B", Description = "XA Audio" },
            new EntryType(){ Tag = "XHTN", TypeID = "8C1580B5", Description = "Hairtone XML" },
            new EntryType(){ Tag = "XMTO", TypeID = "584D544F", Description = "Material Object Class Dump" },
            new EntryType(){ Tag = "XOBJ", TypeID = "CCA8E925", Description = "Object Class Dump" },
            new EntryType(){ Tag = "XTOL", TypeID = "2C1FD8A1", Description = "Texture Overlay XML" },
            new EntryType(){ Tag = "UNK", TypeID = "0F9F0C21", Description = "Unknown (from Nightlife)" },
            new EntryType(){ Tag = "UNK", TypeID = "8B0C79D6", Description = "Unknown" },
            new EntryType(){ Tag = "UNK", TypeID = "9D796DB4", Description = "Unknown" },
            new EntryType(){ Tag = "UNK", TypeID = "CC2A6A34", Description = "Unknown" },
            new EntryType(){ Tag = "UNK", TypeID = "CC8A6A69", Description = "Unknown" },
            new EntryType(){ Tag = "COLL", TypeID = "6C4F359D", Description = "Collection" }
        };

        public static List<FunctionSortList> Sims2BuyFunctionSortList = new(){
                        //seating   
            new FunctionSortList(){flagnum = 0, functionsubsortnum = 1, Category = "Seating", Subcategory = "Dining Room"},
            new FunctionSortList(){flagnum = 0, functionsubsortnum = 2, Category = "Seating", Subcategory = "Living Room"},
            new FunctionSortList(){flagnum = 0, functionsubsortnum = 4, Category = "Seating", Subcategory = "Sofas"},
            new FunctionSortList(){flagnum = 0, functionsubsortnum = 8, Category = "Seating", Subcategory = "Beds"},
            new FunctionSortList(){flagnum = 0, functionsubsortnum = 16, Category = "Seating", Subcategory = "Recreation"},
            new FunctionSortList(){flagnum = 0, functionsubsortnum = 32, Category = "Seating", Subcategory = "Unknown I"},
            new FunctionSortList(){flagnum = 0, functionsubsortnum = 64, Category = "Seating", Subcategory = "Unknown II"},
            new FunctionSortList(){flagnum = 0, functionsubsortnum = 128, Category = "Seating", Subcategory = "Misc"},            
                        //surfaces
            new FunctionSortList(){flagnum = 1, functionsubsortnum = 1, Category = "Surfaces", Subcategory = "Counters"},
            new FunctionSortList(){flagnum = 1, functionsubsortnum = 2, Category = "Surfaces", Subcategory = "Tables"},
            new FunctionSortList(){flagnum = 1, functionsubsortnum = 4, Category = "Surfaces", Subcategory = "End Tables"},
            new FunctionSortList(){flagnum = 1, functionsubsortnum = 8, Category = "Surfaces", Subcategory = "Desks"},
            new FunctionSortList(){flagnum = 1, functionsubsortnum = 16, Category = "Surfaces", Subcategory = "Coffee Tables"},
            new FunctionSortList(){flagnum = 1, functionsubsortnum = 32, Category = "Surfaces", Subcategory = "Shelves"},
            new FunctionSortList(){flagnum = 1, functionsubsortnum = 64, Category = "Surfaces", Subcategory = "Unknown II"},
            new FunctionSortList(){flagnum = 1, functionsubsortnum = 128, Category = "Surfaces", Subcategory = "Misc"},            
                        //Appliances
            new FunctionSortList(){flagnum = 2, functionsubsortnum = 1, Category = "Appliances", Subcategory = "Cooking"},
            new FunctionSortList(){flagnum = 2, functionsubsortnum = 2, Category = "Appliances", Subcategory = "Fridges"},
            new FunctionSortList(){flagnum = 2, functionsubsortnum = 4, Category = "Appliances", Subcategory = "Small"},
            new FunctionSortList(){flagnum = 2, functionsubsortnum = 8, Category = "Appliances", Subcategory = "Large"},
            new FunctionSortList(){flagnum = 2, functionsubsortnum = 16, Category = "Appliances", Subcategory = "Unknown I"},
            new FunctionSortList(){flagnum = 2, functionsubsortnum = 32, Category = "Appliances", Subcategory = "Unknown II"},
            new FunctionSortList(){flagnum = 2, functionsubsortnum = 64, Category = "Appliances", Subcategory = "Unknown III"},
            new FunctionSortList(){flagnum = 2, functionsubsortnum = 128, Category = "Appliances", Subcategory = "Misc"},
                        //Electronics
            new FunctionSortList(){flagnum = 3, functionsubsortnum = 1, Category = "Electronics", Subcategory = "Entertainment"},
            new FunctionSortList(){flagnum = 3, functionsubsortnum = 2, Category = "Electronics", Subcategory = "TV/Computer"},
            new FunctionSortList(){flagnum = 3, functionsubsortnum = 4, Category = "Electronics", Subcategory = "Audio"},
            new FunctionSortList(){flagnum = 3, functionsubsortnum = 8, Category = "Electronics", Subcategory = "Small"},
            new FunctionSortList(){flagnum = 3, functionsubsortnum = 16, Category = "Electronics", Subcategory = "Unknown I"},
            new FunctionSortList(){flagnum = 3, functionsubsortnum = 32, Category = "Electronics", Subcategory = "Unknown II"},
            new FunctionSortList(){flagnum = 3, functionsubsortnum = 64, Category = "Electronics", Subcategory = "Unknown III"},
            new FunctionSortList(){flagnum = 3, functionsubsortnum = 128, Category = "Electronics", Subcategory = "Misc"},
                        //Plumbing
            new FunctionSortList(){flagnum = 4, functionsubsortnum = 1, Category = "Plumbing", Subcategory = "Toilets"},
            new FunctionSortList(){flagnum = 4, functionsubsortnum = 2, Category = "Plumbing", Subcategory = "Showers"},
            new FunctionSortList(){flagnum = 4, functionsubsortnum = 4, Category = "Plumbing", Subcategory = "Sinks"},
            new FunctionSortList(){flagnum = 4, functionsubsortnum = 8, Category = "Plumbing", Subcategory = "Hot Tubs"},
            new FunctionSortList(){flagnum = 4, functionsubsortnum = 16, Category = "Plumbing", Subcategory = "Unknown I"},
            new FunctionSortList(){flagnum = 4, functionsubsortnum = 32, Category = "Plumbing", Subcategory = "Unknown II"},
            new FunctionSortList(){flagnum = 4, functionsubsortnum = 64, Category = "Plumbing", Subcategory = "Unknown III"},
            new FunctionSortList(){flagnum = 4, functionsubsortnum = 128, Category = "Plumbing", Subcategory = "Misc"},
                        //Decorative
            new FunctionSortList(){flagnum = 5, functionsubsortnum = 1, Category = "Decorative", Subcategory = "Wall Decorations"},
            new FunctionSortList(){flagnum = 5, functionsubsortnum = 2, Category = "Decorative", Subcategory = "Sculptures"},
            new FunctionSortList(){flagnum = 5, functionsubsortnum = 4, Category = "Decorative", Subcategory = "Rugs"},
            new FunctionSortList(){flagnum = 5, functionsubsortnum = 8, Category = "Decorative", Subcategory = "Plants"},
            new FunctionSortList(){flagnum = 5, functionsubsortnum = 16, Category = "Decorative", Subcategory = "Mirrors"},
            new FunctionSortList(){flagnum = 5, functionsubsortnum = 32, Category = "Decorative", Subcategory = "Curtains"},
            new FunctionSortList(){flagnum = 5, functionsubsortnum = 64, Category = "Decorative", Subcategory = "Unknown II"},
            new FunctionSortList(){flagnum = 5, functionsubsortnum = 128, Category = "Decorative", Subcategory = "Misc"},
                        //General
            new FunctionSortList(){flagnum = 6, functionsubsortnum = 1, Category = "Misc", Subcategory = "Unknown I"},
            new FunctionSortList(){flagnum = 6, functionsubsortnum = 2, Category = "Misc", Subcategory = "Dressers"},
            new FunctionSortList(){flagnum = 6, functionsubsortnum = 4, Category = "Misc", Subcategory = "Unknown II"},
            new FunctionSortList(){flagnum = 6, functionsubsortnum = 8, Category = "Misc", Subcategory = "Party"},
            new FunctionSortList(){flagnum = 6, functionsubsortnum = 16, Category = "Misc", Subcategory = "Child"},
            new FunctionSortList(){flagnum = 6, functionsubsortnum = 32, Category = "Misc", Subcategory = "Cars"},
            new FunctionSortList(){flagnum = 6, functionsubsortnum = 64, Category = "Misc", Subcategory = "Pets"},
            new FunctionSortList(){flagnum = 6, functionsubsortnum = 128, Category = "Misc", Subcategory = "Misc"},
                        //Lighting
            new FunctionSortList(){flagnum = 7, functionsubsortnum = 1, Category = "Lighting", Subcategory = "Table Lamps"},
            new FunctionSortList(){flagnum = 7, functionsubsortnum = 2, Category = "Lighting", Subcategory = "Floor Lamps"},
            new FunctionSortList(){flagnum = 7, functionsubsortnum = 4, Category = "Lighting", Subcategory = "Wall Lamps"},
            new FunctionSortList(){flagnum = 7, functionsubsortnum = 8, Category = "Lighting", Subcategory = "Ceiling Lamps"},
            new FunctionSortList(){flagnum = 7, functionsubsortnum = 16, Category = "Lighting", Subcategory = "Outdoor"},
            new FunctionSortList(){flagnum = 7, functionsubsortnum = 32, Category = "Lighting", Subcategory = "Unknown I"},
            new FunctionSortList(){flagnum = 7, functionsubsortnum = 64, Category = "Lighting", Subcategory = "Unknown II"},
            new FunctionSortList(){flagnum = 7, functionsubsortnum = 128, Category = "Lighting", Subcategory = "Misc"},
                        //Hobbies
            new FunctionSortList(){flagnum = 8, functionsubsortnum = 1, Category = "Hobbies", Subcategory = "Creative"},
            new FunctionSortList(){flagnum = 8, functionsubsortnum = 2, Category = "Hobbies", Subcategory = "Knowledge"},
            new FunctionSortList(){flagnum = 8, functionsubsortnum = 4, Category = "Hobbies", Subcategory = "Exercise"},
            new FunctionSortList(){flagnum = 8, functionsubsortnum = 8, Category = "Hobbies", Subcategory = "Recreation"},
            new FunctionSortList(){flagnum = 8, functionsubsortnum = 16, Category = "Hobbies", Subcategory = "Unknown I"},
            new FunctionSortList(){flagnum = 8, functionsubsortnum = 32, Category = "Hobbies", Subcategory = "Unknown II"},
            new FunctionSortList(){flagnum = 8, functionsubsortnum = 64, Category = "Hobbies", Subcategory = "Unknown III"},
            new FunctionSortList(){flagnum = 8, functionsubsortnum = 128, Category = "Hobbies", Subcategory = "Misc"},
                        //Aspiration Rewards
            new FunctionSortList(){flagnum = 9, functionsubsortnum = 1, Category = "Aspiration Rewards", Subcategory = "Unknown I"},
            new FunctionSortList(){flagnum = 9, functionsubsortnum = 2, Category = "Aspiration Rewards", Subcategory = "Unknown II"},
            new FunctionSortList(){flagnum = 9, functionsubsortnum = 4, Category = "Aspiration Rewards", Subcategory = "Unknown III"},
            new FunctionSortList(){flagnum = 9, functionsubsortnum = 8, Category = "Aspiration Rewards", Subcategory = "Unknown IV"},
            new FunctionSortList(){flagnum = 9, functionsubsortnum = 16, Category = "Aspiration Rewards", Subcategory = "Unknown V"},
            new FunctionSortList(){flagnum = 9, functionsubsortnum = 32, Category = "Aspiration Rewards", Subcategory = "Unknown VI"},
            new FunctionSortList(){flagnum = 9, functionsubsortnum = 64, Category = "Aspiration Rewards", Subcategory = "Unknown VII"},
            new FunctionSortList(){flagnum = 9, functionsubsortnum = 128, Category = "Aspiration Rewards", Subcategory = "Unknown VIII"},
                        //Career Rewards
            new FunctionSortList(){flagnum = 10, functionsubsortnum = 1, Category = "Career Rewards", Subcategory = "Unknown I"},
            new FunctionSortList(){flagnum = 10, functionsubsortnum = 2, Category = "Career Rewards", Subcategory = "Unknown II"},
            new FunctionSortList(){flagnum = 10, functionsubsortnum = 4, Category = "Career Rewards", Subcategory = "Unknown III"},
            new FunctionSortList(){flagnum = 10, functionsubsortnum = 8, Category = "Career Rewards", Subcategory = "Unknown IV"},
            new FunctionSortList(){flagnum = 10, functionsubsortnum = 16, Category = "Career Rewards", Subcategory = "Unknown V"},
            new FunctionSortList(){flagnum = 10, functionsubsortnum = 32, Category = "Career Rewards", Subcategory = "Unknown VI"},
            new FunctionSortList(){flagnum = 10, functionsubsortnum = 64, Category = "Career Rewards", Subcategory = "Unknown VII"},
            new FunctionSortList(){flagnum = 10, functionsubsortnum = 128, Category = "Career Rewards", Subcategory = "Unknown VIII"}
                        /*//seating
            new FunctionSortList(){flagnum = 11, functionsubsortnum = 1, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 11, functionsubsortnum = 2, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 11, functionsubsortnum = 4, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 11, functionsubsortnum = 8, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 11, functionsubsortnum = 16, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 11, functionsubsortnum = 32, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 11, functionsubsortnum = 64, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 11, functionsubsortnum = 128, Category = "Seating", Subcategory = ""},
                        //seating
            new FunctionSortList(){flagnum = 12, functionsubsortnum = 1, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 12, functionsubsortnum = 2, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 12, functionsubsortnum = 4, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 12, functionsubsortnum = 8, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 12, functionsubsortnum = 16, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 12, functionsubsortnum = 32, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 12, functionsubsortnum = 64, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 12, functionsubsortnum = 128, Category = "Seating", Subcategory = ""},
                        //seating
            new FunctionSortList(){flagnum = 13, functionsubsortnum = 1, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 13, functionsubsortnum = 2, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 13, functionsubsortnum = 4, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 13, functionsubsortnum = 8, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 13, functionsubsortnum = 16, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 13, functionsubsortnum = 32, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 13, functionsubsortnum = 64, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 13, functionsubsortnum = 128, Category = "Seating", Subcategory = ""},
                        //seating
            new FunctionSortList(){flagnum = 14, functionsubsortnum = 1, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 14, functionsubsortnum = 2, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 14, functionsubsortnum = 4, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 14, functionsubsortnum = 8, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 14, functionsubsortnum = 16, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 14, functionsubsortnum = 32, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 14, functionsubsortnum = 64, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 14, functionsubsortnum = 128, Category = "Seating", Subcategory = ""},
                        //seating
            new FunctionSortList(){flagnum = 15, functionsubsortnum = 1, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 15, functionsubsortnum = 2, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 15, functionsubsortnum = 4, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 15, functionsubsortnum = 8, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 15, functionsubsortnum = 16, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 15, functionsubsortnum = 32, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 15, functionsubsortnum = 64, Category = "Seating", Subcategory = ""},
            new FunctionSortList(){flagnum = 15, functionsubsortnum = 128, Category = "Seating", Subcategory = ""},
            */
        };

        public static List<FunctionSortList> Sims2BuildFunctionSortList = new(){
            new FunctionSortList(){flagnum = 8, functionsubsortnum = 1, Category = "Door", Subcategory = ""},
            new FunctionSortList(){flagnum = 8, functionsubsortnum = 4, Category = "Window", Subcategory = ""},
            new FunctionSortList(){flagnum = 8, functionsubsortnum = 100, Category = "Two Story Door", Subcategory = ""},
            new FunctionSortList(){flagnum = 8, functionsubsortnum = 2, Category = "Two Story Window", Subcategory = ""},
            new FunctionSortList(){flagnum = 8, functionsubsortnum = 10, Category = "Arch", Subcategory = ""},
            new FunctionSortList(){flagnum = 1, functionsubsortnum = 20, Category = "Staircase", Subcategory = ""},
            new FunctionSortList(){flagnum = 0, functionsubsortnum = 0, Category = "Fireplaces (?)", Subcategory = ""},
            new FunctionSortList(){flagnum = 1, functionsubsortnum = 400, Category = "Garage", Subcategory = ""},
            new FunctionSortList(){flagnum = 4, functionsubsortnum = 1, Category = "Trees", Subcategory = ""},
            new FunctionSortList(){flagnum = 4, functionsubsortnum = 4, Category = "Flowers", Subcategory = ""},
            new FunctionSortList(){flagnum = 4, functionsubsortnum = 10, Category = "Gardening", Subcategory = ""},
            new FunctionSortList(){flagnum = 4, functionsubsortnum = 2, Category = "Shrubs", Subcategory = ""},
            new FunctionSortList(){flagnum = 1, functionsubsortnum = 1000, Category = "Architecture", Subcategory = ""},
            new FunctionSortList(){flagnum = 1, functionsubsortnum = 8, Category = "Column", Subcategory = ""},
            new FunctionSortList(){flagnum = 1, functionsubsortnum = 100, Category = "Two Story Column", Subcategory = ""},
            new FunctionSortList(){flagnum = 1, functionsubsortnum = 200, Category = "Connecting Column", Subcategory = ""},
            new FunctionSortList(){flagnum = 1, functionsubsortnum = 40, Category = "Pools", Subcategory = ""},
            new FunctionSortList(){flagnum = 8, functionsubsortnum = 8, Category = "Gates", Subcategory = ""},
            new FunctionSortList(){flagnum = 1, functionsubsortnum = 800, Category = "Elevator", Subcategory = ""}
        };
            
    
        
        
        public static void RemoveTempFiles(){
            if (Directory.Exists(tempfolder)){
                Directory.Delete(tempfolder, true);
            }
        }
        
        
        

    }

    public class Utilities {
        public static string GetGameVersion(Games game, string folder){
            string ver = "";
            if (game == Games.Sims2) ver = GetSims2Version(folder);
            if (game == Games.Sims3) ver = GetSims3Version(folder);
            if (game == Games.Sims4) ver = GetSims4Version(folder);
            if (ver != "") {
                ver = Regex.Replace(ver, @"[\p{C}-[\t\r\n]]+", "");
            } 
            
            return ver;
        }
        public static string GetSims4Version(string docfolder){
            string version = "";
            string versionfile = Path.Combine(docfolder, "GameVersion.txt");
            if (File.Exists(versionfile)){
                using (FileStream fileStream = new(versionfile, FileMode.Open, System.IO.FileAccess.Read)){
                    using (StreamReader streamReader = new(versionfile)){
                        version = streamReader.ReadLine();
                        streamReader.Close();
                    }
                    fileStream.Close();
                }
            }
            return version;
        }
        public static string GetSims3Version(string docfolder){
            string version = "";
            string versionfile = Path.Combine(docfolder, "Version.tag");
            if (File.Exists(versionfile)){
                using (FileStream fileStream = new(versionfile, FileMode.Open, System.IO.FileAccess.Read)){
                    using (StreamReader streamReader = new(versionfile)){
                        if (streamReader.ReadLine() == "[Version]") {
                            version = streamReader.ReadLine();
                            version = version.Replace("LatestBase = ", "");
                        };                        
                        streamReader.Close();
                    }
                    fileStream.Close();
                }
            }
            return version;
        }

        public static string GetSims2Version(string docfolder){
            return "LatestVersion";
        }



        public static Sims2Instance LoadS2Instance(string xmlfile){
            Sims2Instance s2 = new();
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Loading instance from {0}.", xmlfile));
            XmlSerializer instanceDeserializer = new XmlSerializer(typeof(Sims2Instance));
            if (File.Exists(xmlfile)){
                using (FileStream fileStream = new(xmlfile, FileMode.Open, System.IO.FileAccess.Read)){
                    using (StreamReader streamReader = new(fileStream)){
                        s2 = (Sims2Instance)instanceDeserializer.Deserialize(streamReader);
                        streamReader.Close();
                    }
                    fileStream.Close();
                }
            }
            return s2;
        }
        public static Sims3Instance LoadS3Instance(string xmlfile){
            Sims3Instance s3 = new();
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Loading instance from {0}.", xmlfile));
            XmlSerializer instanceDeserializer = new XmlSerializer(typeof(Sims3Instance));
            if (File.Exists(xmlfile)){
                using (FileStream fileStream = new(xmlfile, FileMode.Open, System.IO.FileAccess.Read)){
                    using (StreamReader streamReader = new(fileStream)){
                        s3 = (Sims3Instance)instanceDeserializer.Deserialize(streamReader);
                        streamReader.Close();
                    }
                    fileStream.Close();
                }
            }
            return s3;
        }
        public static Sims4Instance LoadS4Instance(string xmlfile){
            Sims4Instance s4 = new();
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Loading instance from {0}.", xmlfile));
            XmlSerializer instanceDeserializer = new XmlSerializer(typeof(Sims4Instance));
            if (File.Exists(xmlfile)){
                using (FileStream fileStream = new(xmlfile, FileMode.Open, System.IO.FileAccess.Read)){
                    using (StreamReader streamReader = new(fileStream)){
                        s4 = (Sims4Instance)instanceDeserializer.Deserialize(streamReader);
                        streamReader.Close();
                    }
                    fileStream.Close();
                }
            }
            return s4;
        }

        public static SimsPackage LoadPackageFile(SimsPackage package){
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Deserializing: {0}.", package.InfoFile));
            XmlSerializer packageDeserializer = new XmlSerializer(typeof(SimsPackage));
            if (File.Exists(package.InfoFile)){
                using (FileStream fileStream = new(package.InfoFile, FileMode.Open, System.IO.FileAccess.Read)){
                    using (StreamReader streamReader = new(fileStream)){
                        package = (SimsPackage)packageDeserializer.Deserialize(streamReader);
                        streamReader.Close();
                    }
                    fileStream.Close();
                }
            }
            return package;
        }
        public static SimsDownload LoadDownloadFile(SimsDownload download){
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Deserializing: {0}.", download.InfoFile));
            XmlSerializer packageDeserializer = new XmlSerializer(typeof(SimsDownload));
            if (File.Exists(download.InfoFile)){
                using (FileStream fileStream = new(download.InfoFile, FileMode.Open, System.IO.FileAccess.Read)){
                    using (StreamReader streamReader = new(fileStream)){
                        download = (SimsDownload)packageDeserializer.Deserialize(streamReader);
                        streamReader.Close();
                    }
                    fileStream.Close();
                }
            }
            return download;
        }
        public static bool IsEven(int val){
            if ((val & 0x1) == 0){
                return true;
            } else {
                return false;
            }
        }

        public static string GetPathForExe(string registryKey)
        {
            string InstallPath = "";
            RegistryKey regKey = Registry.LocalMachine.OpenSubKey(registryKey);

            if (regKey != null)
            {
                InstallPath = regKey.GetValue("Install Dir").ToString();
            }
            return InstallPath;
        }

        public static Texture2D ExtractIcon(Executable exe, string datafolder){
            string exelocation = Path.Combine(exe.Path, exe.Exe);
            System.Drawing.Bitmap icon = (System.Drawing.Bitmap)null;
            try
            {
                icon = Icon.ExtractAssociatedIcon(exelocation).ToBitmap();
            }
            catch (System.Exception)
            {
                // swallow and return nothing. You could supply a default Icon here as well
                return new Texture2D();
            }
            string saveloc = ExeIconName(exe, datafolder);
            icon.Save(saveloc, ImageFormat.Png);
            Godot.Image image = Godot.Image.LoadFromFile(saveloc);
            return ImageTexture.CreateFromImage(image);
        }        

        public static string ExeIconName(Executable exe, string datafolder){
            string exeloc = Path.Combine(exe.Path, exe.Exe);
            FileInfo exeinf = new(exeloc);
            string exename = exeinf.Name.Replace(exeinf.Extension, "");
            string iconname = string.Format("{0}.png", exename);
            string exedir = Path.Combine(datafolder, "executables");
            if (!Directory.Exists(exedir)) Directory.CreateDirectory(exedir);
            return Path.Combine(exedir, iconname);
        }

        public static string RunProcess(string process, string parameters, Games game)
        {
            string result = String.Empty;
            FileInfo exe = new(process);
            string exename = exe.Name;

            if (!File.Exists(process)){
                //Logging.WriteDebugLog("Process was not found.");
            } else {

                //if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Process: {0}", process));
                //if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Params: {0}", parameters));
                string testresult = string.Empty;
                /*Console.WriteLine(parameters);*/

                using (Process p = new Process())
                {
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.FileName = process;
                    p.StartInfo.Arguments = parameters;
                    p.StartInfo.WorkingDirectory = new FileInfo(process).DirectoryName;
                    
                    p.Start();
                    while (p.HasExited == false){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(p.StandardOutput.Read().ToString());
                    }
                    p.WaitForExit();
                    result = p.StandardOutput.ReadToEnd();
                }
            }
            while (!CheckForProcess(game)){
                //
            }
            while (CheckForProcess(game)){
                //
            }

            

            //if (GlobalVariables.DebugMode) Logging.WriteDebugLog("Sims no longer running!");
            return result;            
            //return result;
        }

        private static bool CheckForProcess(Games game){
            bool anything = false;
            if (game == Games.Sims2){
                foreach (string exe in GlobalVariables.Sims2Exes){
                    if (Process.GetProcessesByName(exe).Length == 0){
                        anything = false;
                    } else {
                        anything = true;
                    }
                }

            } else if (game == Games.Sims3){
                foreach (string exe in GlobalVariables.Sims3Exes){
                    if (Process.GetProcessesByName(exe).Length == 0){
                        anything = false;
                    } else {
                        anything = true;
                    }
                }

            } else if (game == Games.Sims4){
                foreach (string exe in GlobalVariables.Sims4Exes){
                    if (Process.GetProcessesByName(exe).Length == 0){
                        anything = false;
                    } else {
                        anything = true;
                    }
                }
            }
            
            return anything;            
        }

        private static void StartProcess(string processname){
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog("Process started!");
            Process[] runninggame = Process.GetProcessesByName(processname);
            if (runninggame.Length == 0){
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog("No game... Waiting!");
                StartProcess(processname);
            } else {
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog("Found the game!");
                return;
            }
        }

        private static string WaitProcess(string processname, string result){
            Process[] runninggame = Process.GetProcessesByName(processname);
            if (runninggame.Length != 0){
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog("Game is running!");
                return WaitProcess(processname, result);
            } else {
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog("Looks like the game closed!");
                return result;
            }
        }
    }

    
}