/* 
    Credits:
        - WandaSoule who helped me crack getting into the S4 entries of it all. 
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections;
using System.Xml;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;
using SSAGlobals;
using SimsCCManager.Packages.Containers;
using SimsCCManager.Packages.Decryption;
using SimsCCManager.Decryption.EndianDecoding;

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

    /*
            None = 0x0000,
            Zlib = 0x5A42,
            RefPack = 0xFFFF
    */

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
        public long position;
        public uint fileSize;
        public uint memSize;
        
        public uint filesize;
        public uint truesize;
        public bool compressed;
        public string unused;
        public string compressionType;

    }	
    class S4PackageSearch
    {
        
        // Class References
        LoggingGlobals log = new LoggingGlobals();
        ReadEntries readentries = new ReadEntries();   
        System.Text.Encoding encoding = System.Text.Encoding.BigEndianUnicode;

        //Vars
        uint chunkOffset = 0;
        int contentposition = 64;
        int contentpositionalt = 40;
        int contentcount = 36;
        
        public void SearchS4Packages(string file) {
            var packageparsecount = GlobalVariables.packagesRead;   
            GlobalVariables.packagesRead++;       

            //Vars for Package Info
            string typefound = "";            
        
            //Misc Vars
            string test = "";
            
            const EndianType endiant = EndianType.Little;
            byte[] uncompresseddata;
            

            SimsPackage thisPackage = new SimsPackage();            

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
            log.MakeLog("Inded Record Position: " + test, true);

            //unused
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unused Unknown:" + test, true);
            
            //unused six bytes
            test = Encoding.ASCII.GetString(readFile.ReadBytes(24));
            log.MakeLog("Unused: " + test, true);

            byte[] headersize = new byte[96];

            if (indexRecordPosition != 0){
                long indexseek = (long)indexRecordPosition - headersize.Length;
                dbpfFile.Seek(indexseek, SeekOrigin.Current);
            } else {
                long indexseek = indexRecordPositionLow - headersize.Length;    
                dbpfFile.Seek(indexRecordPositionLow, SeekOrigin.Current);
            }
            
            //everything before this works perfectly
            

            for (int i = 0; i < entrycount; i++){
                indexEntry holderEntry = new indexEntry();
                
                holderEntry.typeID = readFile.ReadUInt32().ToString("X8");
                log.MakeLog("P" + packageparsecount + "/E" + i + " - Index Entry TypeID: " + holderEntry.typeID, true);

                foreach (typeList type in TypeListings.AllTypesS4){
                    if (type.typeID == holderEntry.typeID){
                        fileHas.Add(new fileHasList() { term = type.desc, location = i});
                    }
                }               

                holderEntry.groupID = readFile.ReadUInt32().ToString("X8");
                log.MakeLog("P" + packageparsecount + "/E" + i + " - Index Entry GroupID: " + holderEntry.groupID, true);
                
                string instanceid1 = (readFile.ReadUInt32() << 32).ToString("X8");
                string instanceid2 = (readFile.ReadUInt32() << 32).ToString("X8");
                holderEntry.instanceID = instanceid1 + instanceid2;
                allInstanceIDs.Add(holderEntry.instanceID);
                log.MakeLog("P" + packageparsecount + "/E" + i + " - InstanceID: " + holderEntry.instanceID, true);

                int testin = readFile.ReadValueS32(endiant);
                holderEntry.position = (long)testin;
                log.MakeLog("P" + packageparsecount + "/E" + i + " - Position " + testin.ToString(), true);

                testint = readFile.ReadValueU32(endiant);
                holderEntry.fileSize = testint;
                log.MakeLog("P" + packageparsecount + "/E" + i + " - File Size " + testint.ToString("X8"), true);

                testint = readFile.ReadValueU32(endiant);
                holderEntry.memSize = testint;
                log.MakeLog("P" + packageparsecount + "/E" + i + " - Mem Size " + testint.ToString("X8"), true);

                testint = readFile.ReadValueU16(endiant);
                holderEntry.compressionType = testint.ToString("X4");
                log.MakeLog("P" + packageparsecount + "/E" + i + " - Compression Type " + testint.ToString("X4"), true);

                testint = readFile.ReadValueU16(endiant);
                log.MakeLog("P" + packageparsecount + "/E" + i + " - Confirmed: " + testint.ToString("X4"), true);

                indexData.Add(holderEntry);

                holderEntry = null;
            }

            log.MakeLog("This package contains: ", true);
            foreach (fileHasList type in fileHas){
                log.MakeLog(type.term + " at location " + type.location, true);
            }
            
            if (fileHas.Exists(x => x.term == "CASP")){
                List<int> entryspots = new List<int>();
                int fh = 0;
                foreach (fileHasList item in fileHas) {
                    if (item.term == "CASP"){
                        entryspots.Add(fh);                       
                    }
                    fh++;
                }    
                int caspc = 0;
                foreach (int e in entryspots){
                    log.MakeLog("Opening CASP #" + caspc, true);
                    if (indexData[e].compressionType == "5A42"){
                            dbpfFile.Seek(indexData[e].position, SeekOrigin.Begin);
                            long entryEnd = indexData[e].position + indexData[e].memSize;
                            log.MakeLog("Position: " + indexData[e].position, true);
                            log.MakeLog("Filesize: " + indexData[e].fileSize, true);
                            log.MakeLog("Memsize: " + indexData[e].memSize, true);
                            log.MakeLog("Entry ends at " + entryEnd, true);
                            byte[] entry = readFile.ReadBytes((int)indexData[e].memSize);
                            Stream decomps = S4Decryption.Decompress(entry);
                            
                            BinaryReader decompbr = new BinaryReader(decomps);

                            uint version = decompbr.ReadUInt32();
                            log.MakeLog("Version: " + version, true);
                            log.MakeLog("-- As hex: " + version.ToString("X8"), true);
                            uint datasize = decompbr.ReadUInt32();
                            log.MakeLog("Datasize: " + datasize, true);
                            log.MakeLog("-- As hex: " + datasize.ToString("X8"), true);
                            uint numPresets = decompbr.ReadUInt32();
                            log.MakeLog("Presets: " + numPresets, true);
                            log.MakeLog("-- As hex: " + numPresets.ToString("X8"), true);

                            using (var reader = new BinaryReader(decomps, Encoding.BigEndianUnicode, true))
                            {
                                thisPackage.Title = reader.ReadString();
                            }
                            log.MakeLog("Name: " + thisPackage.Title, true);
                            float catalogSort = decompbr.ReadSingle();
                            log.MakeLog("Catalog Sort: " + catalogSort, true);
                            uint secondaryDisplayIndex = decompbr.ReadUInt16();
                            log.MakeLog("Secondary Display Index: " + secondaryDisplayIndex.ToString("X4"), true);
                            uint propertyID = decompbr.ReadUInt32();
                            log.MakeLog("Property ID: " + propertyID.ToString("X8"), true);
                            
                            testint = decompbr.ReadUInt32();
                            log.MakeLog("Test: " + testint.ToString("X8"), true);
                            //testint = decompbr.ReadUInt16();
                            //log.MakeLog("Struct?: " + testint.ToString("X8"), true);

                            int[] parameterFlag = new int[1];
                            parameterFlag[0] = (int)decompbr.ReadUInt16();
                            BitArray parameterFlags = new BitArray(parameterFlag);

                            int pfc = 0;
                            foreach (var pf in parameterFlags){
                                log.MakeLog("Function Sort Flag [" + pfc + "] is: " + parameterFlags[pfc].ToString(), true);                    
                                pfc++;
                            }      

                            /*
                                [7-6]    not_used
                                [5]      ShowInCasDemo
                                [4]      ShowInSimInfoPanel
                                [3]      ShowInUI
                                [2]      AllowForRandom
                                [1]      DefaultThumnailPart
                                [0]      DefaultForBodyType 
                            */

                            ulong testlong = decompbr.ReadUInt64();
                            log.MakeLog("Test: " + testlong.ToString("X8"), true);                           
                    }



                    caspc++;
                }         
                
                            

             

            }

            if (fileHas.Exists(x => x.term == "COBJ")){
                List<int> entryspots = new List<int>();
                int fh = 0;
                foreach (fileHasList item in fileHas) {
                    if (item.term == "COBJ"){
                        entryspots.Add(fh);                       
                    }
                    fh++;
                }    
                int cobjc = 0;
                foreach (int e in entryspots){
                    log.MakeLog("Opening COBJ #" + cobjc, true);
                    if (indexData[e].compressionType == "5A42"){
                            dbpfFile.Seek(indexData[e].position, SeekOrigin.Begin);
                            long entryEnd = indexData[e].position + indexData[e].memSize;
                            log.MakeLog("Position: " + indexData[e].position, true);
                            log.MakeLog("Filesize: " + indexData[e].fileSize, true);
                            log.MakeLog("Memsize: " + indexData[e].memSize, true);
                            log.MakeLog("Entry ends at " + entryEnd, true);
                            byte[] entry = readFile.ReadBytes((int)indexData[e].memSize);
                            Stream decomps = S4Decryption.Decompress(entry);
                            
                            BinaryReader decompbr = new BinaryReader(decomps);

                            uint version = decompbr.ReadUInt32();
                            log.MakeLog("Version: " + version, true);
                            log.MakeLog("-- As hex: " + version.ToString("X8"), true);
                            uint namehash = decompbr.ReadUInt32();
                            log.MakeLog("NameHash: " + namehash, true);
                            log.MakeLog("-- As hex: " + namehash.ToString("X8"), true);
                            uint descriptionhash = decompbr.ReadUInt32();
                            log.MakeLog("DescriptionHash: " + descriptionhash, true);
                            log.MakeLog("-- As hex: " + descriptionhash.ToString("X8"), true);                            
                            uint price = decompbr.ReadUInt32();
                            log.MakeLog("Price: " + price, true);
                            log.MakeLog("-- As hex: " + price.ToString("X8"), true);
                            //bunch of stuff i dont need rn
                            decompbr.ReadBytes(12);
                            decompbr.ReadBytes(14);
                            decompbr.ReadBytes(2);
                            decompbr.ReadBytes(32);

                            uint thumbhash = decompbr.ReadUInt32();
                            log.MakeLog("Thumbnail Hash: " + thumbhash, true);
                            log.MakeLog("-- As hex: " + thumbhash.ToString("X8"), true);

                            
                    }



                    cobjc++;
                }         
                
                            

             

            }




            //everything after this works perfectly

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

            //ifs

            int casp;int geom;int rle;int rmap;int thum;int img;int xml;int clhd;int clip;int stbl;int cobj;int ftpt;int lite;int thm;int mlod;int modl;int mtbl;int objd;int rslt;int tmlt;int ssm;int lrle;

            if ((typeDict.TryGetValue("S4SM", out ssm) && ssm >= 1)){
                thisPackage.Type = "Merged Package";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);
            } else if ((typeDict.TryGetValue("RLE2", out rle) && rle >= 1) && (typeDict.TryGetValue("CASP", out casp) && casp >= 1) && (typeDict.TryGetValue("GEOM", out geom) && geom <= 0)){
                thisPackage.Type = "CAS Recolor";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);
            } else if ((typeDict.TryGetValue("LRLE", out lrle) && lrle >= 1) && (typeDict.TryGetValue("CASP", out casp) && casp >= 1) && (typeDict.TryGetValue("GEOM", out geom) && geom <= 0)){
                thisPackage.Type = "Makeup";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);
            } else if ((typeDict.TryGetValue("CASP", out casp) && casp >= 1) && (typeDict.TryGetValue("GEOM", out geom) && geom >= 1) && (typeDict.TryGetValue("RLE2", out rle) && rle >= 1) && (typeDict.TryGetValue("RMAP", out rmap) && rmap >= 1) && (typeDict.TryGetValue("_IMG", out img) && img >= 1)){
                thisPackage.Type = "Hair Mesh";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);
            } else if ((typeDict.TryGetValue("_IMG", out img) && img >= 1) && (typeDict.TryGetValue("_XML", out xml) && xml >= 1) && (typeDict.TryGetValue("CLHD", out clhd) && clhd >= 1) && (typeDict.TryGetValue("CLIP", out clip) && clip >= 1) && (typeDict.TryGetValue("STBL", out stbl) && stbl >= 1)){
                thisPackage.Type = "Pose Pack";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);
            } else if ((typeDict.TryGetValue("_IMG", out img) && img >= 1) && (typeDict.TryGetValue("COBJ", out cobj) && cobj >= 1) && (typeDict.TryGetValue("FTPT", out ftpt) && ftpt >= 1) && (typeDict.TryGetValue("LITE", out lite) && lite >= 1) && (typeDict.TryGetValue("MLOD", out mlod) && mlod >= 1) && (typeDict.TryGetValue("MODL", out modl) && modl >= 1) && (typeDict.TryGetValue("MTBL", out mtbl) && mtbl >= 1) && (typeDict.TryGetValue("OBJD", out objd) && objd >= 1) && (typeDict.TryGetValue("RSLT", out rslt) && rslt >= 1) && (typeDict.TryGetValue("STBL", out stbl) && stbl >= 1) && (typeDict.TryGetValue("TMLT", out tmlt) && tmlt >= 1)){
                thisPackage.Type = "Object Mesh";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);
            } else if ((typeDict.TryGetValue("RLE2", out rle) && rle >= 1) && (typeDict.TryGetValue("CASP", out casp) && casp >= 1) && (typeDict.TryGetValue("GEOM", out geom) && geom >= 1)){
                thisPackage.Type = "CAS Part";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);
            } else {
                log.MakeLog("Unable to identify package.", true);
                thisPackage.Type = "UNKNOWN";
            }
            


            distinctInstanceIDs = allInstanceIDs.Distinct().ToList();
            thisPackage.InstanceIDs.AddRange(distinctInstanceIDs);
            distinctGUIDS = allGUIDS.Distinct().ToList();
            thisPackage.ObjectGUID.AddRange(distinctGUIDS);
            log.MakeLog("In thisPackage: " + thisPackage.ToString(), true);
            log.MakeLog(thisPackage.ToString(), false);
            Containers.Containers.allSims4Packages.Add(thisPackage);

            readFile.Close();
            Console.WriteLine("Closing Package #" + packageparsecount + "/" + GlobalVariables.PackageCount + ": " + packageinfo.Name);
            

            packageparsecount++;

        }

        public static String hexToASCII(String hex)
        {
            // initialize the ASCII code string as empty.
            String ascii = "";
    
            for (int i = 0; i < hex.Length; i += 2)
            {
    
                // extract two characters from hex string
                String part = hex.Substring(i, 2);
    
                // change it into base 16 and
                // typecast as the character
                char ch = (char)Convert.ToInt32(part, 16);;
    
                // add this char to final ASCII string
                ascii = ascii + ch;
            }
            return ascii;
        }        
    }
}