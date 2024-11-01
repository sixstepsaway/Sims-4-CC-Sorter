using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SimsCCManager.Packages.Containers;
using System.Xml;
using SimsCCManager.Globals;
using System.Drawing;
using SimsCCManager.Settings.Loaded;
using SimsCCManager.Debugging;
using SimsCCManager.PackageReaders.Containers;

namespace SimsCCManager.PackageReaders
{
    public class Sims2PackageReader
    {
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
            
        uint chunkOffset = 0;

		FileStream msPackage;
		BinaryReader readFile;
		uint major;
		uint minor;
		uint dateCreated;
		uint dateModified;
		uint indexMajorVersion;
		uint indexCount;
		uint indexOffset;
		uint indexSize;
		uint holesCount;
		uint holesOffset;
		uint holesSize;
		uint indexMinorVersion;
		uint numrecords;
        int dirnum = -1;
		string instanceID2 = "";
		List<IndexEntry> PackageEntries = new();
		public SimsPackage simsPackage = new();
		Sims2ScanData scanData = new();
		
		public SimsPackage ReadSims2Package(){
			
			msPackage = new FileStream(simsPackage.Location, FileMode.Open, FileAccess.Read);
            readFile = new BinaryReader(msPackage);

			Encoding.ASCII.GetString(readFile.ReadBytes(4));
            major = readFile.ReadUInt32();                
            minor = readFile.ReadUInt32();

            string reserved = Encoding.UTF8.GetString(readFile.ReadBytes(12));
            dateCreated = readFile.ReadUInt32();
            dateModified = readFile.ReadUInt32();
            indexMajorVersion = readFile.ReadUInt32();
            indexCount = readFile.ReadUInt32();
            indexOffset = readFile.ReadUInt32();
            indexSize = readFile.ReadUInt32();
            holesCount = readFile.ReadUInt32();
            holesOffset = readFile.ReadUInt32();
            holesSize = readFile.ReadUInt32();
            indexMinorVersion = readFile.ReadUInt32() -1;
            string reserved2 = Encoding.UTF8.GetString(readFile.ReadBytes(32));
            int dirnum = -1;
            
			msPackage.Seek(chunkOffset + indexOffset, SeekOrigin.Begin);

			if (indexCount == 0) 
			{
				readFile.Close();
				return simsPackage;
			}

			GetEntries();
			GetDirectory();
            ReadEntries();








			simsPackage.ScanData = scanData;
			simsPackage.WriteInfoFile();
			return simsPackage;
		}

		public void GetEntries(){
			int entrynum = 0;
			for (int i = 0; i < indexCount; i++){
                IndexEntry holderEntry = new IndexEntry();
                holderEntry.TypeID = readFile.ReadUInt32().ToString("X8");

                holderEntry.GroupID = readFile.ReadUInt32().ToString("X8");

                holderEntry.InstanceID = readFile.ReadUInt32().ToString("X8"); 

                if ((indexMajorVersion == 7) && (indexMinorVersion == 1)) {
                    holderEntry.InstanceID2 = readFile.ReadUInt32().ToString("X8");
                } else {
                    holderEntry.InstanceID2 = "00000000";
                }

                holderEntry.Offset = readFile.ReadUInt32();
                holderEntry.Filesize = readFile.ReadUInt32();
                holderEntry.Truesize = 0;
                holderEntry.Compressed = false;
				holderEntry.Location = entrynum;
				List<EntryType> e = Sims2EntryTypes.Where(x => x.TypeID == holderEntry.TypeID).ToList();
				if (e.Any()){					
					if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Package {0} has {1}.", simsPackage.FileName, e[0].Tag));
                    holderEntry.Tag = e[0].Tag;
					holderEntry.Location = entrynum;
					holderEntry.TypeID = e[0].TypeID;
                }
                PackageEntries.Add(holderEntry);         
				entrynum++;       
            }
		}

		private void GetDirectory(){
			uint myFilesize;
			if (PackageEntries.Exists(x => x.Tag == "DIR")){
				dirnum = PackageEntries.Where(x => x.Tag == "DIR").First().Location;
				msPackage.Seek(chunkOffset + PackageEntries[dirnum].Offset, SeekOrigin.Begin);
				if (indexMajorVersion == 7 && indexMinorVersion == 1){
					numrecords = PackageEntries[dirnum].Filesize / 20;
				} else {
					numrecords = PackageEntries[dirnum].Filesize / 16;
				}

				for (int i = 0; i < numrecords; i++){
					IndexEntry holderEntry = new();
					holderEntry.TypeID = readFile.ReadUInt32().ToString("X8");
                    holderEntry.GroupID = readFile.ReadUInt32().ToString("X8");
                    string instanceID = readFile.ReadUInt32().ToString("X8");
                    holderEntry.InstanceID = readFile.ReadUInt32().ToString("X8");
                    if (indexMajorVersion == 7 && indexMinorVersion == 1) instanceID2 = readFile.ReadUInt32().ToString("X8");
                    myFilesize = readFile.ReadUInt32();
                    foreach (IndexEntry idx in PackageEntries){
						if ((idx.TypeID == holderEntry.TypeID) && (idx.GroupID == holderEntry.GroupID) && (idx.InstanceID == holderEntry.InstanceID))
						{
							if (indexMajorVersion == 7 && indexMinorVersion == 1){
								if (idx.InstanceID2 == instanceID2){
									idx.Compressed = true;
									idx.Filesize = myFilesize;
									break;
								}
							}
						} else {
							idx.Compressed = true;
							idx.Truesize = myFilesize;
							break;
						}
					}
				}
			} else if (PackageEntries.Exists(x => x.Tag == "UI")){
                dirnum = PackageEntries.Where(x => x.Tag == "UI").First().Location;				

                msPackage.Seek(chunkOffset + PackageEntries[dirnum].Offset, SeekOrigin.Begin);
                if (indexMajorVersion == 7 && indexMinorVersion == 1){
                    numrecords = PackageEntries[dirnum].Filesize / 20;
                } else {
                    numrecords = PackageEntries[dirnum].Filesize / 16;
                }

                for (int i = 0; i < numrecords; i++){
                    IndexEntry holderEntry = new();
                    string typeID = readFile.ReadUInt32().ToString("X8");
                    string groupID = readFile.ReadUInt32().ToString("X8");
                    string instanceID = readFile.ReadUInt32().ToString("X8");
                    holderEntry.InstanceID = readFile.ReadUInt32().ToString("X8");
                    myFilesize = readFile.ReadUInt32();


                    foreach (IndexEntry idx in PackageEntries){
                        if ((idx.TypeID == typeID) && (idx.GroupID == groupID) && (idx.InstanceID == instanceID))
                        {
                            if (indexMajorVersion == 7 && indexMinorVersion == 1){
                                if (idx.InstanceID2 == instanceID2){
                                    idx.Compressed = true;
                                    idx.Filesize = myFilesize;
                                    break;
                                }
                            }
                        } else {
                            idx.Compressed = true;
                            idx.Truesize = myFilesize;
                            break;
                        }
                    }
                }
            } else if (PackageEntries.Exists(x => x.TypeID == "286B1F03")){
                dirnum = PackageEntries.Where(x => x.TypeID == "286B1F03").First().Location;				

                msPackage.Seek(chunkOffset + PackageEntries[dirnum].Offset, SeekOrigin.Begin);
                if (indexMajorVersion == 7 && indexMinorVersion == 1){
                    numrecords = PackageEntries[dirnum].Filesize / 20;
                } else {
                    numrecords = PackageEntries[dirnum].Filesize / 16;
                }

                for (int i = 0; i < numrecords; i++){
                    IndexEntry holderEntry = new();
                    string typeID = readFile.ReadUInt32().ToString("X8");
                    string groupID = readFile.ReadUInt32().ToString("X8");
                    string instanceID = readFile.ReadUInt32().ToString("X8");
                    holderEntry.InstanceID = readFile.ReadUInt32().ToString("X8");
                    myFilesize = readFile.ReadUInt32();


                    foreach (IndexEntry idx in PackageEntries){
                        if ((idx.TypeID == typeID) && (idx.GroupID == groupID) && (idx.InstanceID == instanceID))
                        {
                            if (indexMajorVersion == 7 && indexMinorVersion == 1){
                                if (idx.InstanceID2 == instanceID2){
                                    idx.Compressed = true;
                                    idx.Filesize = myFilesize;
                                    break;
                                }
                            }
                        } else {
                            idx.Compressed = true;
                            idx.Truesize = myFilesize;
                            break;
                        }
                    }
                }
            }
		}
	
        private void ReadEntries(){
            List<EntryType> EntryTypes = new();
            EntryType e = Sims2EntryTypes.Where(x => x.Tag == "CTSS").First();
            if (PackageEntries.Where(x => x.TypeID == e.Tag).Any()){
                List<IndexEntry> indexes = PackageEntries.Where(x => x.TypeID == e.Tag).ToList();
                foreach (IndexEntry idx in indexes){
                    msPackage.Seek(chunkOffset + idx.Offset, SeekOrigin.Begin);
                    int cFileSize = readFile.ReadInt32();
                    string cTypeID = readFile.ReadUInt16().ToString("X4");
                    if (cTypeID == "FB10") 
                    {
                        byte[] tempBytes = readFile.ReadBytes(3);
                        uint cFullSize = EntryReaders.QFSLengthToInt(tempBytes);
                        DecryptByteStream decompressed = new DecryptByteStream(EntryReaders.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));
                        ReadCTSSChunk cts = new(decompressed);     
                        scanData.CTSSData.Add(cts.ctss);
                    } 
                    else 
                    {
                        msPackage.Seek(chunkOffset + idx.Offset, SeekOrigin.Begin);
                        ReadCTSSChunk cts = new(readFile);
                        scanData.CTSSData.Add(cts.ctss);
                    }
                }
            }
            List<string> typeids = new(){ Sims2EntryTypes.Where(x => x.Tag == "XOBJ").First().TypeID, Sims2EntryTypes.Where(x => x.Tag == "XFNC").First().TypeID, Sims2EntryTypes.Where(x => x.Tag == "XFLR").First().TypeID, Sims2EntryTypes.Where(x => x.Tag == "XMOL").First().TypeID, Sims2EntryTypes.Where(x => x.Tag == "XROF").First().TypeID, Sims2EntryTypes.Where(x => x.Tag == "XTOL").First().TypeID, Sims2EntryTypes.Where(x => x.Tag == "XHTN").First().TypeID};
            if (PackageEntries.Where(x => typeids.Any(z => z == x.TypeID)).Any()){
                List<IndexEntry> indexes = new();
                foreach (string typeid in typeids){
                    indexes.AddRange(PackageEntries.Where(x => x.TypeID == typeid).ToList());
                }
                foreach (IndexEntry idx in indexes){
                    msPackage.Seek(chunkOffset + idx.Offset, SeekOrigin.Begin);
                    int cFileSize = readFile.ReadInt32();
                    string cTypeID = readFile.ReadUInt16().ToString("X4");
                    if (cTypeID == "FB10"){
                        byte[] tempBytes = readFile.ReadBytes(3);
                        uint cFullSize = EntryReaders.QFSLengthToInt(tempBytes);
                        string cpfTypeID = readFile.ReadUInt32().ToString("X8");

                        if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0")){
                            ReadCPFChunk cpfchunk = new(readFile);
                            scanData.CPFData.Add(cpfchunk.cpf);
                        } else {
                            msPackage.Seek(chunkOffset + idx.Offset + 9, SeekOrigin.Begin);
                            DecryptByteStream decompressed = new DecryptByteStream(EntryReaders.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));

                            if (cpfTypeID == "E750E0E2")
                            {
                                cpfTypeID = decompressed.ReadUInt32().ToString("X8");
                                if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0")) 
                                {
                                    ReadCPFChunk cpfchunk = new(decompressed);      
                                    scanData.CPFData.Add(cpfchunk.cpf);                              
                                } 
                            } else {
                                ReadCPFChunk cpfchunk = new(decompressed);          
                                scanData.CPFData.Add(cpfchunk.cpf);                      
                            }
                        }
                    } else {
                        msPackage.Seek(chunkOffset + idx.Offset, SeekOrigin.Begin);
                        ReadCPFChunk cpfchunk = new(readFile);
                        scanData.CPFData.Add(cpfchunk.cpf);
                    }
                }                
                   
            }


        }
    
    }


    public class EntryReaders{
        public static uint QFSLengthToInt(byte[] data)
        {			
            // Converts a 3 byte length to a uint
            uint power = 1;
            uint result = 0;
            for (int i = data.Length; i > 0; i--)
            {
                result += (data[i-1] * power);
                power = power * 256;
            }

            return result;
        }

        public static byte[] Uncompress(byte[] data, uint targetSize, int offset)
		{			
			byte[] uncdata = null;
			int index = offset;			

			try 
			{
				uncdata = new Byte[targetSize];
			} 
			catch(Exception) 
			{
				uncdata = new Byte[0];
			}
			
			int uncindex = 0;
			int plaincount = 0;
			int copycount = 0;
			int copyoffset = 0;
			Byte cc = 0;
			Byte cc1 = 0;
			Byte cc2 = 0;
			Byte cc3 = 0;
			int source;
			
			try 
			{
				while ((index<data.Length) && (data[index] < 0xfc))
				{
					cc = data[index++];
				
					if ((cc&0x80)==0)
					{
						cc1 = data[index++];
						plaincount = (cc & 0x03);
						copycount = ((cc & 0x1C) >> 2) + 3;
						copyoffset = ((cc & 0x60) << 3) + cc1 +1;
					} 
					else if ((cc&0x40)==0)
					{
						cc1 = data[index++];
						cc2 = data[index++];
						plaincount = (cc1 & 0xC0) >> 6 ; 
						copycount = (cc & 0x3F) + 4 ;
						copyoffset = ((cc1 & 0x3F) << 8) + cc2 +1;							
					} 
					else if ((cc&0x20)==0)
					{
						cc1 = data[index++];
						cc2 = data[index++];
						cc3 = data[index++];
						plaincount = (cc & 0x03);
						copycount = ((cc & 0x0C) << 6) + cc3 + 5;
						copyoffset = ((cc & 0x10) << 12) + (cc1 << 8) + cc2 +1;
					} 
					else 
					{									
						plaincount = (cc - 0xDF) << 2; 
						copycount = 0;
						copyoffset = 0;				
					}

					for (int i=0; i<plaincount; i++) uncdata[uncindex++] = data[index++];

					source = uncindex - copyoffset;	
					for (int i=0; i<copycount; i++) uncdata[uncindex++] = uncdata[source++];
				}
			} 
			catch(Exception ex)
			{
				throw ex;
			} 
			

			if (index<data.Length) 
			{
				plaincount = (data[index++] & 0x03);
				for (int i=0; i<plaincount; i++) 
				{
					if (uncindex>=uncdata.Length) break;
					uncdata[uncindex++] = data[index++];
				}
			}
			return uncdata;
		}

        public static string ReadNullString(BinaryReader reader)
		{
            string result = "";
			char c;
			for (int i = 0; i < reader.BaseStream.Length; i++) 
			{
				if ((c = (char) reader.ReadByte()) == 0) 
				{
					break;
				}
				result += c.ToString();
			}
			return result;
		}
    }

    public struct ReadCTSSChunk{
        public S2CTSS ctss = new();
        string Description = "";
        string Title = "";

        public ReadCTSSChunk(BinaryReader readFile){
            readFile.ReadBytes(64);
			readFile.ReadUInt16();

			uint numStrings = readFile.ReadUInt16();
			bool foundLang = false;
            
			for (int k = 0; k < numStrings; k++)
			{
				int langCode = Convert.ToInt32(readFile.ReadByte().ToString());

				string blah = EntryReaders.ReadNullString(readFile);
				string meep = EntryReaders.ReadNullString(readFile);

				if (langCode == 1) 
				{
					if (foundLang == true) { Description = blah.Replace("\n", " "); break; }
					if (foundLang == false) { Title = blah.Replace("\n", " "); foundLang = true; }
				}

			}
            if (!string.IsNullOrWhiteSpace(Description) && !string.IsNullOrWhiteSpace(Description)){
                ctss.Description = Description;
            }
            if (!string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(Title)){
                ctss.Title = Title;
            }
        }
        public ReadCTSSChunk(DecryptByteStream readFile){
            readFile.SkipAhead(66);

			uint numStrings = readFile.ReadUInt16();
			bool foundLang = false;
            
			for (int k = 0; k < numStrings; k++)
			{
				byte[] langCode = readFile.ReadBytes(1);

				string blah = readFile.GetNullString();
				string meep = readFile.GetNullString();

				if (langCode[0] == 1) 
				{
					if (foundLang == true) { Description = blah.Replace("\n", " "); break; }
					if (foundLang == false) { Title = blah.Replace("\n", " "); foundLang = true; }
				}

			}
            if (!string.IsNullOrWhiteSpace(Description) && !string.IsNullOrWhiteSpace(Description)){
                ctss.Description = Description;
            }
            if (!string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(Title)){
                ctss.Title = Title;
            }           
        }
    }

    public struct ReadCPFChunk{
        public S2CPF cpf = new();
        string title = "";
        string description = "";
        string xmltype = "";
        string xmlsubtype = "";
        string xmlcat = "";
        string xmlmodelname = "";
        string xmlcreator = "";
        string xmlage = "";
        string xmlgender = "";

        public ReadCPFChunk(BinaryReader readFile){
            // Read an uncompressed CPF chunk and extract the name, description and type
			// Version
			readFile.ReadUInt16();

			uint numItems = readFile.ReadUInt32();

			// Read the items
			for (int i = 0; i < numItems; i++)
			{
				// Get type of the item
				string dataType = readFile.ReadUInt32().ToString("X8");
				uint nameLength = readFile.ReadUInt32();
				string fieldName = Encoding.UTF8.GetString(readFile.ReadBytes((int)nameLength));

				uint fieldValueInt = 0;
				string fieldValueString = "";


				switch (dataType)
				{
						// Int
					case "EB61E4F7":
						fieldValueInt = readFile.ReadUInt32();
						break;
						// Int #2 - Not Used
					case "0C264712":
						fieldValueInt = readFile.ReadUInt32();
						break;
						// String
					case "0B8BEA18":
						uint stringLength = readFile.ReadUInt32();
						fieldValueString = Encoding.UTF8.GetString(readFile.ReadBytes((int)stringLength));
						break;
						// Float
					case "ABC78708":
						// Ignore for now
						uint fieldValueFloat = readFile.ReadUInt32();
						break;
						// Boolean
					case "CBA908E1":
						bool fieldValueBool = readFile.ReadBoolean();
						break;
				}

				switch (fieldName)
				{
					case "name":                    
						title = fieldValueString;
						break;
					case "description":
						description = fieldValueString;
						break;
					case "type":
						xmltype = fieldValueString;
						break;
					case "subtype":
						xmlsubtype = fieldValueInt.ToString();
						break;
					case "category":
						xmlcat = fieldValueInt.ToString();
						break;
					case "modelName":
						xmlmodelname = fieldValueString;
						break;
					case "objectGUID":
						cpf.GUIDs.Add(fieldValueInt.ToString("X8"));
						break;
					case "creator":
						xmlcreator = fieldValueString;
						break;
					case "age":
						xmlage = fieldValueInt.ToString();
						break;
					case "gender":
						xmlgender = fieldValueInt.ToString();
						break;
				}
			}
            if (string.IsNullOrEmpty(xmlage) && string.IsNullOrWhiteSpace(xmlage)) cpf.XMLAge = xmlage;
            if (string.IsNullOrEmpty(xmlage) && string.IsNullOrWhiteSpace(xmlage)) cpf.XMLAge = xmlage;
            if (string.IsNullOrEmpty(xmlage) && string.IsNullOrWhiteSpace(xmlage)) cpf.XMLAge = xmlage;
            if (string.IsNullOrEmpty(xmlage) && string.IsNullOrWhiteSpace(xmlage)) cpf.XMLAge = xmlage;
            if (string.IsNullOrEmpty(xmlage) && string.IsNullOrWhiteSpace(xmlage)) cpf.XMLAge = xmlage;
            if (string.IsNullOrEmpty(xmlage) && string.IsNullOrWhiteSpace(xmlage)) cpf.XMLAge = xmlage;
             
        }
        public ReadCPFChunk(DecryptByteStream readFile){
            // Read a compressed CPF chunk from a byte stream and extrac the name, 
			// description and type

			// Version
			readFile.ReadUInt16();

			uint numItems = readFile.ReadUInt32();

			// Read the items
			for (int i = 0; i < numItems; i++)
			{
				// Get type of the item
				string dataType = readFile.ReadUInt32().ToString("X8");
				uint nameLength = readFile.ReadUInt32();
				string fieldName = Encoding.UTF8.GetString(readFile.ReadBytes(nameLength));

				uint fieldValueInt = 0;
				string fieldValueString = "";

				switch (dataType)
				{
					// Int
					case "EB61E4F7":
						fieldValueInt = readFile.ReadUInt32();
						break;
					// Int #2 - Not Used
					case "0C264712":
						fieldValueInt = readFile.ReadUInt32();
						break;
					// String
					case "0B8BEA18":
						uint stringLength = readFile.ReadUInt32();
						fieldValueString = Encoding.UTF8.GetString(readFile.ReadBytes(stringLength));
						break;
					// Float
					case "ABC78708":
						// Ignore for now
						uint fieldValueFloat = readFile.ReadUInt32();
						break;
					// Boolean
					case "CBA908E1":
						bool fieldValueBool = readFile.ReadBoolean();
						break;
				}

				switch (fieldName)
				{
					case "name":                    
						title = fieldValueString;
						break;
					case "description":
						description = fieldValueString;
						break;
					case "type":
						xmltype = fieldValueString;
						break;
					case "subtype":
						xmlsubtype = fieldValueInt.ToString();
						break;
					case "category":
						xmlcat = fieldValueInt.ToString();
						break;
					case "modelName":
						xmlmodelname = fieldValueString;
						break;
					case "objectGUID":
						cpf.GUIDs.Add(fieldValueInt.ToString("X8"));
						break;
					case "creator":
						xmlcreator = fieldValueString;
						break;
					case "age":
						xmlage = fieldValueInt.ToString();
						break;
					case "gender":
						xmlgender = fieldValueInt.ToString();
						break;
				}
			}
        }
    }


    public class DecryptByteStream{
        private int currOffset = 0;
        private byte[] byteStream;

		public DecryptByteStream(byte[] inputBytes) 
        {
            byteStream = inputBytes;
        }
        
        public int Offset {
            get{ return currOffset; }
            set{ currOffset = value; }
        }

        public void SkipAhead(int numToSkip) {
            this.Offset += numToSkip;
        }

        public string GetNullString() {
            string result = "";
            char c;
            for (int i = 0; i < byteStream.Length; i++) {
                if ((c = (char)byteStream[currOffset]) == 0) { currOffset++; break; }
                result += c.ToString();
                currOffset++;
            }

            return result;
        }

        public byte ReadByte(){
            byte result = new byte();
            if (currOffset > byteStream.Length) return result;
            result = byteStream[currOffset];
            currOffset++; 
            return result;
        }

        public byte[] ReadBytes(uint count) {
            byte[] result = new byte [count];
            for (int i = 0; i < count; i++)
            {
                result[i] = byteStream[currOffset];
                currOffset++;
                if (currOffset > byteStream.Length) return result;
            }
            return result;
        }

        public uint ReadUInt32() {
            uint power = 1;
            uint result = 0;

            for (int i = 0; i < 4; i++){
                if (currOffset > byteStream.Length) return result;
                result += (byteStream[currOffset] * power);
                power = power * 256;
                currOffset++;
            }
            return result;
        }

        public uint ReadUInt16(){
            uint power = 1;
            uint result = 0;

            for (int i = 0; i < 2; i++) {
                if (currOffset > byteStream.Length) return result;
                result += (byteStream[currOffset] * power);
                power = power * 256;
                currOffset++;
            }
            return result;
        }

        public bool ReadBoolean(){
            bool result = false;
            if (currOffset > byteStream.Length) return result;

            byte temp = byteStream[currOffset];
            currOffset++;

            if (temp == 1) {result = true;}
            else {result = false;}

            return result;
        }

        public string ReadString() {
            string result = "";
            //Get the first byte containing the string length
            if (currOffset > byteStream.Length) return result;

            byte stringLength = byteStream[currOffset];
            currOffset++;

            //Does the string start with 01?
            if (byteStream[currOffset] == 1) currOffset++;

            //get the length of the string
            result = Encoding.UTF8.GetString(ReadBytes((uint)stringLength));
            return result;
        }

        public byte[] GetEntireStream()
        {
            return byteStream;
        }
    }
    
}