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
using SimsCCManager.Packages.Decryption;

///disabled until i can work out how to do this myself >:(

namespace SimsCCManager.Packages.Sims4Search
{
    public static class extensions {
        public static void Increment<T>(this Dictionary<T, int> dictionary, T key)
        {
            int count;
            dictionary.TryGetValue(key, out count);
            dictionary[key] = count + 1;
        }
    }

    public class EntryHolder {
        public int[] entry {get; set;}
        public IReadOnlyList<int> header {get; set;}        
    }

    public class indexEntry
    {
        public string typeID;
        public string groupID;
        public string instanceID;
        public string instanceID2;
        public uint offset;
        public uint filesize;
        public uint truesize;
        public bool compressed;
        public string unused;
    }	
    class S4PackageSearch
    {
        
        // Class References
        LoggingGlobals log = new LoggingGlobals();
        ReadEntries readentries = new ReadEntries();   

        //Vars
        
        public void SearchS4Packages(string file) {
            /*var packageparsecount = GlobalVariables.packagesRead;   
            GlobalVariables.packagesRead++;         
            //Vars for Package Info
            string typefound = "";
            string instanceID2;
            string typeID;
            string groupID;
            string instanceID;            
        
            //Misc Vars
            string test = "";

            SimsPackage thisPackage = new SimsPackage();
            
            //Vars for sizes and such
            int ContentCount = 36;
            int ContentPosition = 64;
            int ContentPositionAlternate = 40;
            byte[] HeaderID = new byte[96];
            int MajorStart = 4;
            int MinorStart = 8;
            int Fields = 9;
            int InstanceStart = 12;
            int InstanceStartAlternate = 16;
            int ResourceGroupStart = 8;
            int ResourceTypeStart = 4;


            //Lists 
            
            List<fileHasList> fileHas = new List<fileHasList>();
            List<string> allGUIDS = new List<string>();      
            List<string> distinctGUIDS = new List<string>();  
            List<string> allInstanceIDs = new List<string>();      
            List<string> distinctInstanceIDs = new List<string>();  

            //create readers  
            FileInfo packageinfo = new FileInfo(file);
            FileStream dbpfFile = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader readFile = new BinaryReader(dbpfFile);    
            Byte[] allbytes = File.ReadAllBytes(file);
            MemoryStream ms = new MemoryStream(allbytes);       
            BinaryReader br = new BinaryReader(ms);

            //log opening file
            Console.WriteLine("Reading Package #" + packageparsecount + "/" + GlobalVariables.PackageCount + ": " + packageinfo.Name);
            log.MakeLog("Logged Package #" + packageparsecount + " as " + packageinfo.FullName, true);
            thisPackage.Location = packageinfo.FullName;            
            thisPackage.Game = 4;
            log.MakeLog("Logged Package #" + packageparsecount + " as meant for The Sims " + thisPackage.Game, true);           
            
            //start actually reading the package 
            int counter = 0;
            //dbpf
            test = Encoding.ASCII.GetString(readFile.ReadBytes(4));
            log.MakeLog("DBPF: " + test, true);
            
            //major
            uint testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Major :" + test, true);
            
            //minor
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Minor : " + test, true);
            
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unknown : " + test, true);
            
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unknown : " + test, true);
            
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unknown : " + test, true);
            
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Created : " + test, true);

            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Modified : " + test, true);
            
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Index Major : " + test, true);
            
            //entrycount
            uint entrycount = readFile.ReadUInt32();
            test = entrycount.ToString();
            log.MakeLog("Entry Count: " + test, true);
            
            //record position low
            uint indexRecordPositionLow = readFile.ReadUInt32();
            test = indexRecordPositionLow.ToString();
            log.MakeLog("indexRecordPositionLow: " + test, true);
            
            //index record size
            uint indexRecordSize = readFile.ReadUInt32();
            test = indexRecordSize.ToString();
            log.MakeLog("indexRecordSize: " + test, true);

            //unused
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unused Trash Index offset: " + test, true);
            
            //unused
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unused Trash Index size: " + test, true);
            
            //unused
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unused Index Minor Version: " + test, true);
            
            //unused but 3 for historical reasons
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unused, 3 for historical reasons: " + test, true);
            
            ulong indexRecordPosition = readFile.ReadUInt64();
            test = indexRecordPosition.ToString();
            log.MakeLog("Index Record Position: " + test, true);

            //unused
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unused Unknown:" + test, true);
            
            //unused six bytes
            test = Encoding.ASCII.GetString(readFile.ReadBytes(24));
            log.MakeLog("Unused: " + test, true);

            ms.Position = 0;
            var reader = new BinaryReader(ms);
            ms.Position = ContentPosition;
            var type = readFile.ReadUInt32();
            log.MakeLog("Type: " + type, true);
            
            var headerSize = 1;
            for (var i = 1; i < sizeof(uint); i++)
            {
                if ((type & (1 << i)) != 0)
                {
                    headerSize++;
                }
            }
            log.MakeLog("Header Size: " + headerSize, true);
            
            var header = new int[headerSize];
            header[0] = (int) type;
            for (var i = 1; i < header.Length; i++)
            {
                header[i] = readFile.ReadInt32();
            }

            log.MakeLog("Header Items: ", true);
            foreach (var item in header){
                log.MakeLog(item.ToString("X4"), true);
            }

            log.MakeLog("Content: ", true);

            var entry = new int[Fields - headerSize];
            for (var i = 0; i < ContentCount; i++){
                for (var j = 0; j < entry.Length; j++){
                    entry[j] = readFile.ReadInt32();
                    log.MakeLog(header.ToString(), true);
                    log.MakeLog(entry[j].ToString("X4"), true);
                }
            }*/

            var packageparsecount = GlobalVariables.packagesRead;   
            GlobalVariables.packagesRead++;         
            //Vars for Package Info
            string typefound = "";
            string instanceID2;
            string typeID;
            string groupID;
            string instanceID;
            uint compfilesize;
            uint numRecords;
            string cTypeID;
            int cFileSize;  
            uint myFilesize;
            string magic = "";
            int major = 0;
            int minor = 0;
            int unused4 = 0;
            
        
            //Misc Vars
            string test = "";        
            int dirnum = 0;
            List<int> objdnum = new List<int>();   
            List<int> strnm = new List<int>();  
            List<int> imgnm = new List<int>();
            int mmatloc = 0;

            SimsPackage thisPackage = new SimsPackage();
            SimsPackage infovar = new SimsPackage();
            SimsPackage ctssvar = new SimsPackage();
            SimsPackage dirvar = new SimsPackage();
            SimsPackage strvar = new SimsPackage();
            SimsPackage objdvar = new SimsPackage();
            SimsPackage mmatvar = new SimsPackage();
            

            //Lists 
            
            List<EntryHolder> entries = new List<EntryHolder>();
            List<fileHasList> fileHas = new List<fileHasList>();
            ArrayList linkData = new ArrayList();
            List<indexEntry> indexData = new List<indexEntry>();
            FileInfo packageinfo = new FileInfo(file); 
            //List<string> iids = new List<string>();
            List<string> allGUIDS = new List<string>();      
            List<string> distinctGUIDS = new List<string>();  
            List<string> allInstanceIDs = new List<string>();      
            List<string> distinctInstanceIDs = new List<string>();  

            //create readers  
            FileStream dbpfFile = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader readFile = new BinaryReader(dbpfFile);    
            Byte[] allbytes = File.ReadAllBytes(file);
            MemoryStream ms = new MemoryStream(allbytes);       

            //log opening file
            Console.WriteLine("Reading Package #" + packageparsecount + "/" + GlobalVariables.PackageCount + ": " + packageinfo.Name);
            log.MakeLog("Logged Package #" + packageparsecount + " as " + packageinfo.FullName, true);
            thisPackage.Location = packageinfo.FullName;            
            thisPackage.Game = 4;
            log.MakeLog("Logged Package #" + packageparsecount + " as meant for The Sims " + thisPackage.Game, true);           
            
            //start actually reading the package 
            int counter = 0;
            //dbpf
            test = Encoding.ASCII.GetString(readFile.ReadBytes(4));
            log.MakeLog("DBPF: " + test, true);
            
            //major
            uint testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Major :" + test, true);
            
            //minor
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Minor : " + test, true);
            
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unknown : " + test, true);
            
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unknown : " + test, true);
            
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unknown : " + test, true);
            
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Created : " + test, true);

            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Modified : " + test, true);
            
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Index Major : " + test, true);
            
            //entrycount
            uint entrycount = readFile.ReadUInt32();
            test = entrycount.ToString();
            log.MakeLog("Entry Count: " + test, true);
            
            //record position low
            uint indexRecordPositionLow = readFile.ReadUInt32();
            test = indexRecordPositionLow.ToString();
            log.MakeLog("indexRecordPositionLow: " + test, true);
            
            //index record size
            uint indexRecordSize = readFile.ReadUInt32();
            test = indexRecordSize.ToString();
            log.MakeLog("indexRecordSize: " + test, true);

            //unused
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unused Trash Index offset: " + test, true);
            
            //unused
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unused Trash Index size: " + test, true);
            
            //unused
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unused Index Minor Version: " + test, true);
            
            //unused but 3 for historical reasons
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unused, 3 for historical reasons: " + test, true);
            
            ulong indexRecordPosition = readFile.ReadUInt64();
            test = indexRecordPosition.ToString();
            log.MakeLog("Index Record Position: " + test, true);

            //unused
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unused Unknown:" + test, true);
            
            //unused six bytes
            test = Encoding.ASCII.GetString(readFile.ReadBytes(24));
            log.MakeLog("Unused: " + test, true);

            byte[] headersize = new byte[96];

            long indexseek = (long)indexRecordPosition - headersize.Length;

            if (indexseek != 0){
                dbpfFile.Seek(indexseek, SeekOrigin.Current);
                 } else {
                dbpfFile.Seek(indexRecordPositionLow, SeekOrigin.Begin);
            }
            
            //dont know what this is
            
            for (int i = 0; i < entrycount; i++){
                indexEntry holderEntry = new indexEntry();

                holderEntry.typeID = readFile.ReadUInt32().ToString("X8");
                log.MakeLog("P" + packageparsecount + " - Index Entry TypeID: " + holderEntry.typeID, true);

                foreach (typeList type in TypeListings.AllTypesS4){
                    if (type.typeID == holderEntry.typeID){
                        fileHas.Add(new fileHasList() { term = type.desc, location = i});
                    }
                }

                holderEntry.groupID = readFile.ReadUInt32().ToString("X8");
                log.MakeLog("P" + packageparsecount + " - Index Entry GroupID: " + holderEntry.groupID, true);

                string instanceid1 = (readFile.ReadUInt32() << 32).ToString("X8");                
                string instanceid2 = (readFile.ReadUInt32() << 32).ToString("X8");
                holderEntry.instanceID = instanceid1 + instanceid2;
                allInstanceIDs.Add(holderEntry.instanceID);
                log.MakeLog("P" + packageparsecount + " - InstanceID: " + holderEntry.instanceID, true);
                
                test = readFile.ReadUInt32().ToString("X8");
                log.MakeLog("Position " + test, true);
                test = readFile.ReadUInt32().ToString("X4");
                log.MakeLog("Size " + test, true);    
                test = readFile.ReadUInt16().ToString("X8");          
                log.MakeLog("Memsize: " + test, true);                
                test = readFile.ReadUInt16().ToString("X4");
                log.MakeLog("???: " + test, true);
                test = readFile.ReadUInt16().ToString("X4");
                log.MakeLog("Compression Type: " + test, true);
                test = readFile.ReadUInt16().ToString("X4");
                log.MakeLog("Committed: " + test, true);

                
                
                indexData.Add(holderEntry);

                holderEntry = null;
                packageparsecount++;
                
            }

            log.MakeLog("This package contains: ", true);
            foreach (fileHasList type in fileHas){
                log.MakeLog(type.term + " at location " + type.location, true);
            }









            log.MakeLog("All methods complete, moving on to getting info.", true);
            /*log.MakeLog("Dirvar contains: " + dirvar.ToString(), true);
            log.MakeLog("Ctssvar contains: " + ctssvar.ToString(), true);
            log.MakeLog("Mmatvar contains: " + mmatvar.ToString(), true);
            log.MakeLog("Objdvar contains: " + objdvar.ToString(), true);
            log.MakeLog("Strvar contains: " + strvar.ToString(), true);*/

            List<TypeCounter> typecount = new List<TypeCounter>();
            var typeDict = new Dictionary<string, int>();

            log.MakeLog("Making dictionary.", true);

            foreach (fileHasList item in fileHas){
                foreach (typeList type in TypeListings.AllTypesS4){
                    if (type.desc == item.term){
                        log.MakeLog("Added " + type.desc + " to dictionary.", true);
                        typeDict.Increment(type.desc);
                    }
                }
            }

            foreach (KeyValuePair<string, int> type in typeDict){
                TypeCounter tc = new TypeCounter();
                tc.Type = type.Key;
                tc.Count = type.Value;
                log.MakeLog("There are " + tc.Count + " of " + tc.Type + " in this package.", true);
                typecount.Add(tc);
            }

            thisPackage.Entries.AddRange(typecount);

            if ((typeDict.TryGetValue("CASP", out int casp_0) && casp_0 >= 1) && (typeDict.TryGetValue("GEOM", out int geom_0) && geom_0 >= 1) && (typeDict.TryGetValue("RLE2", out int rle_0) && rle_0 >= 1) && (typeDict.TryGetValue("RMAP", out int rmap_0) && rmap_0 >= 1) && (typeDict.TryGetValue("THUM", out int thum_0) && thum_0 >= 1) && (typeDict.TryGetValue("_IMG", out int img_0) && img_0 >= 1)){
                thisPackage.Type = "Hair Mesh";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);
            } else if ((typeDict.TryGetValue("_IMG", out img_0) && img_0 >= 1) && (typeDict.TryGetValue("_XML", out int xml_0) && xml_0 >= 1) && (typeDict.TryGetValue("CLHD", out int clhd_0) && clhd_0 >= 1) && (typeDict.TryGetValue("CLIP", out int clip_0) && clip_0 >= 1) && (typeDict.TryGetValue("STBL", out int stbl_0) && stbl_0 >= 1)){
                thisPackage.Type = "Pose Pack";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);
            }  else if ((typeDict.TryGetValue("_IMG", out img_0) && img_0 >= 1) && (typeDict.TryGetValue("_THM", out int thm_0) && thm_0 >= 1) && (typeDict.TryGetValue("COBJ", out int cobj_0) && cobj_0 >= 1) && (typeDict.TryGetValue("FTPT", out int ftpt_0) && ftpt_0 >= 1) && (typeDict.TryGetValue("LITE", out int lite_0) && lite_0 >= 1)){
                thisPackage.Type = "Object Mesh";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);
            } else {
                log.MakeLog("Unable to identify package.", true);
                thisPackage.Type = "UNKNOWN";
            }
            



        }        
    }
}