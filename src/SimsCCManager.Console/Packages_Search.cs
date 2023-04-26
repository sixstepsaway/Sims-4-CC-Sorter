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

namespace SimsCCManager.Packages.Search
{    
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
    class PackageSearch
    {
        // References
        LoggingGlobals log = new LoggingGlobals();
        ReadEntries readentries = new ReadEntries();   

        //Lists
        public static SimsPackage thisPackage = new SimsPackage();
        public static SimsPackage infovar = new SimsPackage();
        ArrayList linkData = new ArrayList();
        ArrayList indexData = new ArrayList();
        Dictionary<string, bool> fileHas = new Dictionary<string, bool>();        

        //Vars for Package Info
        uint chunkOffset = 0;
        string typefound = "";
        string instanceID2;
        string typeID;
        string groupID;
        string instanceID;
        string title;
        uint compfilesize;
        uint numRecords;
        string cTypeID;
        int cFileSize;

        //Misc Vars
        string test = "";        
        int packageparsecount = 0;
        public void SearchS2Packages(string file) {  
            FileInfo packageinfo = new FileInfo(file);          
            packageparsecount++;
            log.MakeLog("Logged Package #" + packageparsecount + " as " + packageinfo.FullName, true);
            thisPackage.Location = packageinfo.FullName;
            thisPackage.Game = 2;
            log.MakeLog("Logged Package #" + packageparsecount + " as The Sims " + thisPackage.Game, true);           
            FileStream dbpfFile = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader readFile = new BinaryReader(dbpfFile);
            test = Encoding.ASCII.GetString(readFile.ReadBytes(4));
            log.MakeLog("P" + packageparsecount + " - DBPF Bytes: " + test, true);
            
            uint major = readFile.ReadUInt32();
            test = major.ToString();  
            log.MakeLog("P" + packageparsecount + " - Major: " + test, true);

            uint minor = readFile.ReadUInt32();
            test = minor.ToString();
            log.MakeLog("P" + packageparsecount + " - Minor: " + test, true);
            
            string reserved = Encoding.UTF8.GetString(readFile.ReadBytes(12));
            test = reserved;
            log.MakeLog("P" + packageparsecount + " - Reserved: " + test, true);
            
            uint dateCreated = readFile.ReadUInt32();
            test = dateCreated.ToString();
            log.MakeLog("P" + packageparsecount + " - Date created: " + test, true);
            
            uint dateModified = readFile.ReadUInt32();
            test = dateModified.ToString();
            log.MakeLog("P" + packageparsecount + " - Date modified: " + test, true);
            
            uint indexMajorVersion = readFile.ReadUInt32();
            test = indexMajorVersion.ToString();
            log.MakeLog("P" + packageparsecount + " - Index Major: " + test, true);
            
            uint indexCount = readFile.ReadUInt32();
            test = indexCount.ToString();
            log.MakeLog("P" + packageparsecount + " - Index Count: " + test, true);
            
            uint indexOffset = readFile.ReadUInt32();
            test = indexOffset.ToString();
            log.MakeLog("P" + packageparsecount + " - Index Offset: " + test, true);
            
            uint indexSize = readFile.ReadUInt32();
            test = indexSize.ToString();
            log.MakeLog("P" + packageparsecount + " - Index Size: " + test, true);
            
            uint holesCount = readFile.ReadUInt32();
            test = holesCount.ToString();
            log.MakeLog("P" + packageparsecount + " - Holes Count: " + test, true);

            uint holesOffset = readFile.ReadUInt32();
            test = holesOffset.ToString();
            log.MakeLog("P" + packageparsecount + " - Holes Offset: " + test, true);
            
            uint holesSize = readFile.ReadUInt32();
            test = holesSize.ToString();
            log.MakeLog("P" + packageparsecount + " - Holes Size: " + test, true);
            
            uint indexMinorVersion = readFile.ReadUInt32() -1;
            test = indexMinorVersion.ToString();
            log.MakeLog("P" + packageparsecount + " - Index Minor Version: " + test, true);
            
            string reserved2 = Encoding.UTF8.GetString(readFile.ReadBytes(32));
            log.MakeLog("P" + packageparsecount + " - Reserved 2: " + reserved2, true);

            log.MakeLog("P" + packageparsecount + " - ChunkOffset: " + chunkOffset, true);

            dbpfFile.Seek(chunkOffset + indexOffset, SeekOrigin.Begin);

            for (int i = 0; i < indexCount; i++) {
                indexEntry holderEntry = new indexEntry();
                log.MakeLog("P" + packageparsecount + " - Made index entry.", true);
                holderEntry.typeID = readFile.ReadUInt32().ToString("X8");
                log.MakeLog("P" + packageparsecount + " - Index Entry TypeID: " + holderEntry.typeID, true);

                holderEntry.groupID = readFile.ReadUInt32().ToString("X8");
                log.MakeLog("P" + packageparsecount + " - Index Entry GroupID: " + holderEntry.groupID, true);

                holderEntry.instanceID = readFile.ReadUInt32().ToString("X8");
                log.MakeLog("P" + packageparsecount + " - InstanceID: " + holderEntry.instanceID, true);

                if ((indexMajorVersion == 7) && (indexMinorVersion == 1)) {
                    holderEntry.instanceID2 = readFile.ReadUInt32().ToString("X8");
                } else {
                    holderEntry.instanceID2 = "00000000";
                }
                log.MakeLog("P" + packageparsecount + " - InstanceID2: " + holderEntry.instanceID2, true);

                holderEntry.offset = readFile.ReadUInt32();
                log.MakeLog("P" + packageparsecount + " - Offset: " + holderEntry.offset, true);
                holderEntry.filesize = readFile.ReadUInt32();
                log.MakeLog("P" + packageparsecount + " - Filesize: " + holderEntry.filesize, true);
                holderEntry.truesize = 0;
                log.MakeLog("P" + packageparsecount + " - Truesize: " + holderEntry.truesize, true);
                holderEntry.compressed = false;

                indexData.Add(holderEntry);

                holderEntry = null;

                if (indexCount == 0) 
                {
                    log.MakeLog("P" + packageparsecount + " - Package is broken. Closing.", true);
                    readFile.Close();
                    return;
                }
                
            }

            var entrynum = 0;
            foreach (indexEntry iEntry in indexData) {
                log.MakeLog("P" + packageparsecount + " - Entry #" + entrynum, true);
                entrynum++;
                uint numRecords;
                string typeID;
                string groupID;
                string instanceID;
                string instanceID2;
                uint myFilesize;

                switch (iEntry.typeID.ToLower()) 
                {                    
                    case "fc6eb1f7": fileHas.Add("SHPE", true); linkData.Add(iEntry); log.MakeLog("P" + packageparsecount + " - File has SHPE.", true); break;
                }

                foreach (typeList type in TypeListings.AllTypesS2) {
                    if (iEntry.typeID == type.typeID) {
                        log.MakeLog("P" + packageparsecount + " - Found: " + type.desc, true);
                        typefound = type.desc;
                        try {
                            fileHas.Add(type.desc, true);
                        } catch {
                            //nada
                        }
                        break;
                    }
                }
                log.MakeLog("P" + packageparsecount + " - This file has:", true);
                foreach (KeyValuePair<string, bool> kvp in fileHas) {
                    log.MakeLog(kvp.Key + ": " + kvp.Value, true);
                }








            }

            log.MakeLog("P" + packageparsecount + " - Infovar Title: " + infovar.Title, true);
            log.MakeLog("P" + packageparsecount + " - Infovar Desc: " + infovar.Description, true);
            log.MakeLog("P" + packageparsecount + " - This Package Location: " + thisPackage.Location, true);
            log.MakeLog("P" + packageparsecount + " - This Package Title: " + thisPackage.Title, true);
            log.MakeLog("P" + packageparsecount + " - This Package Desc: " + thisPackage.Description, true);
            
            
        }
    }
}