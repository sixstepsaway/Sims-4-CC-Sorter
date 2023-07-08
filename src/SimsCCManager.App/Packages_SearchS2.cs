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

namespace SimsCCManager.Packages.Sims2Search
{    
    public static class extensions {
        public static void Increment<T>(this Dictionary<T, int> dictionary, T key)
        {
            int count;
            dictionary.TryGetValue(key, out count);
            dictionary[key] = count + 1;
        }
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
    class S2PackageSearch
    {
        
        // References
        LoggingGlobals log = new LoggingGlobals();
        ReadEntries readentries = new ReadEntries();   

        //Vars
        uint chunkOffset = 0;        
       

        public void SearchS2Packages(FileStream dbpfFile, FileInfo packageinfo, uint minor, int packageparsecount, StringBuilder LogFile) {
            //var packageparsecount = GlobalVariables.packagesRead;   
            //GlobalVariables.packagesRead++;         
            //Vars for Package Info
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string LogMessage = "";
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
        
            //Misc Vars
            string test = "";        
            List<int> imgnm = new List<int>();
            int dirnum = 0;

            SimsPackage thisPackage = new SimsPackage();
            SimsPackage infovar = new SimsPackage();
            SimsPackage ctssvar = new SimsPackage();
            SimsPackage dirvar = new SimsPackage();
            SimsPackage strvar = new SimsPackage();
            SimsPackage objdvar = new SimsPackage();
            SimsPackage mmatvar = new SimsPackage();
            

            //Lists 
            
            List<PackageEntries> fileHas = new List<PackageEntries>();
            ArrayList linkData = new ArrayList();
            List<indexEntry> indexData = new List<indexEntry>();
            //FileInfo packageinfo = new FileInfo(file); 
            //List<string> iids = new List<string>();
            List<PackageGUID> allGUIDS = new();   
            List<PackageInstance> allInstanceIDs = new();   

            //create readers  
            //FileStream dbpfFile = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader readFile = new BinaryReader(dbpfFile);

            thisPackage.PackageName = packageinfo.Name;
            thisPackage.Game = 2;
            
            dbpfFile.Position = 0;

            //start actually reading the package 
            //iteLine("Reading Package #" + packageparsecount + "/" + GlobalVariables.PackageCount + ": " + packageinfo.Name);
            LogMessage = string.Format("Logged Package #{0} as {1}.", packageparsecount, packageinfo.FullName);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
            thisPackage.Location = packageinfo.FullName;            
            thisPackage.Game = 2;
            LogMessage = string.Format("Logged Package #{0} as meant for The Sims {1}.", packageparsecount, thisPackage.Game);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));           
            test = Encoding.ASCII.GetString(readFile.ReadBytes(4));
            LogMessage = string.Format("P{0} - DBPF Bytes: {1}.", packageparsecount, test);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
            
            uint major = readFile.ReadUInt32();
            test = major.ToString();  
            LogMessage = string.Format("P{0} - Major: {1}", packageparsecount, test);  
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);  
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));

            minor = readFile.ReadUInt32();
            test = minor.ToString();
            LogMessage = string.Format("P{0} - Minor: {1}", packageparsecount, test);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
            
            string reserved = Encoding.UTF8.GetString(readFile.ReadBytes(12));
            test = reserved;
            LogMessage = string.Format("P{0} - Reserved: {1}", packageparsecount, test);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
            
            uint dateCreated = readFile.ReadUInt32();
            test = dateCreated.ToString();
            LogMessage = string.Format("P{0} - Date Created: {1}", packageparsecount, test);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
            
            uint dateModified = readFile.ReadUInt32();
            test = dateModified.ToString();
            LogMessage = string.Format("P{0} - Date modified: {1}", packageparsecount, test);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
            
            uint indexMajorVersion = readFile.ReadUInt32();
            test = indexMajorVersion.ToString();
            LogMessage = string.Format("P{0} - Index Major: {1}", packageparsecount, test);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
            
            uint indexCount = readFile.ReadUInt32();
            test = indexCount.ToString();
            LogMessage = string.Format("P{0} - Index Count: {1}", packageparsecount, test);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
            
            uint indexOffset = readFile.ReadUInt32();
            test = indexOffset.ToString();
            LogMessage = string.Format("P{0} - Index Offset: {1}", packageparsecount, test);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
            
            uint indexSize = readFile.ReadUInt32();
            test = indexSize.ToString();
            LogMessage = string.Format("P{0} - Index Size: {1}", packageparsecount, test);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
            
            uint holesCount = readFile.ReadUInt32();
            test = holesCount.ToString();
            LogMessage = string.Format("P{0} - Holes Count: {1}", packageparsecount, test);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));

            uint holesOffset = readFile.ReadUInt32();
            test = holesOffset.ToString();
            LogMessage = string.Format("P{0} - Holes Offset: {1}", packageparsecount, test);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
            
            uint holesSize = readFile.ReadUInt32();
            test = holesSize.ToString();
            LogMessage = string.Format("P{0} - Holes Size: {1}", packageparsecount, test);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
            
            uint indexMinorVersion = readFile.ReadUInt32() -1;
            test = indexMinorVersion.ToString();
            LogMessage = string.Format("P{0} - Index Minor Version: {1}", packageparsecount, test);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
            
            string reserved2 = Encoding.UTF8.GetString(readFile.ReadBytes(32));
            LogMessage = string.Format("P{0} - Reserved 2: {1}", packageparsecount, reserved2);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));

            LogMessage = string.Format("P{0} - Chunk Offset: {1}", packageparsecount, chunkOffset);

            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);

            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));

            dbpfFile.Seek(chunkOffset + indexOffset, SeekOrigin.Begin);
            for (int i = 0; i < indexCount; i++) {
                indexEntry holderEntry = new indexEntry();
                LogMessage = string.Format("P{0} - Made Index Entry {1}", packageparsecount, i);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                holderEntry.typeID = readFile.ReadUInt32().ToString("X8");
                LogMessage = string.Format("P{0} - Index Entry TypeID: {1}", packageparsecount, holderEntry.typeID);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                
                holderEntry.groupID = readFile.ReadUInt32().ToString("X8");
                LogMessage = string.Format("P{0} - Index Entry GroupID: {1}", packageparsecount, holderEntry.groupID);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                holderEntry.instanceID = readFile.ReadUInt32().ToString("X8");
                //Console.WriteLine(holderEntry.instanceID);
                allInstanceIDs.Add(new PackageInstance() {InstanceID = holderEntry.instanceID.ToString()});
                LogMessage = string.Format("P{0} - Index Entry InstanceID: {1}", packageparsecount, holderEntry.instanceID);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));

                if ((indexMajorVersion == 7) && (indexMinorVersion == 1)) {
                    holderEntry.instanceID2 = readFile.ReadUInt32().ToString("X8");
                } else {
                    holderEntry.instanceID2 = "00000000";
                }
                LogMessage = string.Format("P{0} - Index Entry InstanceID2: {1}", packageparsecount, holderEntry.instanceID2);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));

                holderEntry.offset = readFile.ReadUInt32();
                LogMessage = string.Format("P{0} - Offset: {1}", packageparsecount, holderEntry.offset);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                holderEntry.filesize = readFile.ReadUInt32();
                LogMessage = string.Format("P{0} - FileSize: {1}", packageparsecount, holderEntry.filesize);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                holderEntry.truesize = 0;
                LogMessage = string.Format("P{0} - TrueSize: {1}", packageparsecount, holderEntry.truesize);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                holderEntry.compressed = false;

                indexData.Add(holderEntry);

                holderEntry = null;

                if (indexCount == 0) 
                {
                    LogMessage = string.Format("P{0} - Package is Broken. Closing.", packageparsecount);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                    readFile.Close();
                    return;
                }
                
            }


            


            //Console.WriteLine(indexData[0].typeID);

            var entrynum = 0;
            foreach (indexEntry iEntry in indexData) {
                LogMessage = string.Format("P{0} - Entry [{1}].", packageparsecount, entrynum);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                
                

                switch (iEntry.typeID.ToLower()) 
                {                    
                    case "fc6eb1f7": 
                        linkData.Add(iEntry);
                        LogMessage = string.Format("P{0} - File has SHPE.", packageparsecount);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage)); 
                        break;
                }

                List<typeList> types = GlobalVariables.S4FunctionTypesConnection.Query<typeList>(string.Format("SELECT * FROM S2Types"));
                
                foreach (typeList type in types) {
                    //LogMessage = string.Format("P" + packageparsecount + " - Checking entry " + entrynum + " (type ID: " + iEntry.typeID + ") for: " + type.desc;if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                    if (iEntry.typeID == type.typeID) {
                        LogMessage = string.Format("P{0}/E{1} - Found: {2}", packageparsecount, entrynum, type.desc);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                        typefound = type.desc;
                        try {
                            fileHas.Add(new PackageEntries() { Name = type.desc, Location = entrynum, TypeID = type.typeID});
                        } catch {
                            //nada
                        }
                        break;
                    }
                    
                }
                entrynum++;
            }
            
            if (fileHas.Exists(x => x.Name == "DIR")) {
                dirnum = (from has in fileHas
                        where has.Name =="STR#"
                        select has.Location).FirstOrDefault();
                LogMessage = string.Format("P{0} - DIR is at entry {1}", packageparsecount, dirnum);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                entrynum = 0;
            
                //go through dir entry specifically 
                
                numRecords = 0;
                typeID = "";
                groupID = "";
                instanceID = "";
                instanceID2 = "";
                myFilesize = 0;
                
                LogMessage = string.Format("P{0}/DIR: {1} at {2}", packageparsecount, indexData[dirnum].typeID, indexData[dirnum].offset);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);                
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));

                dbpfFile.Seek(this.chunkOffset + indexData[dirnum].offset, SeekOrigin.Begin);
                if (indexMajorVersion == 7 && indexMinorVersion == 1)
                {
                    numRecords = indexData[dirnum].filesize / 20;
                }
                else 
                {
                    numRecords = indexData[dirnum].filesize / 16;
                }  

                LogMessage = string.Format("P{0}/DIR - Number of compressed records in entry: {1}", packageparsecount, numRecords);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);  
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                
                for (int c = 0; c < numRecords; c++)
                {
                    indexEntry holderEntry = new indexEntry();
                    LogMessage = string.Format("P{0}/DIR - Reading compressed record #{1}", packageparsecount, c);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                    typeID = readFile.ReadUInt32().ToString("X8");
                    LogMessage = string.Format("P{0}/DIR  - CR#{1}: TypeID: {2}", packageparsecount, c, typeID);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                    groupID = readFile.ReadUInt32().ToString("X8");
                    LogMessage = string.Format("P{0}/DIR  - CR#{1}: GroupID: {2}", packageparsecount, c, groupID);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                    instanceID = readFile.ReadUInt32().ToString("X8");
                    LogMessage = string.Format("P{0}/DIR  - CR#{1}: InstanceID: {2}", packageparsecount, c, instanceID);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                    holderEntry.instanceID = readFile.ReadUInt32().ToString("X8");
                    //Console.WriteLine(holderEntry.instanceID);
                    allInstanceIDs.Add(new PackageInstance() {InstanceID = holderEntry.instanceID.ToString()});
                    if (indexMajorVersion == 7 && indexMinorVersion == 1) instanceID2 = readFile.ReadUInt32().ToString("X8");
                    LogMessage = string.Format("P{0}/DIR  - CR#{1}: InstanceID2: {2}", packageparsecount, c, instanceID2);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                    myFilesize = readFile.ReadUInt32();
                    LogMessage = string.Format("P{0}/DIR  - CR#{1}: FileSize: {2}", packageparsecount, c, myFilesize);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));

                    foreach (indexEntry idx in indexData) 
                    {
                        if ((idx.typeID == typeID) && (idx.groupID == groupID) && (idx.instanceID == instanceID))
                        {
                            if (indexMajorVersion == 7 && indexMinorVersion == 1) 
                            {
                                if (idx.instanceID2 == instanceID2) 
                                {
                                    idx.compressed = true;
                                    idx.truesize = myFilesize;
                                    LogMessage = string.Format("P{0}/DIR  - CR#{1}: Index Entry FileSize: {2}", packageparsecount, c, myFilesize);
                                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                                    break;
                                }
                            } 
                            else
                            {
                                idx.compressed = true;
                                idx.truesize = myFilesize;
                                LogMessage = string.Format("P{0}/DIR  - CR#{1}: Index Entry FileSize: {2}", packageparsecount, c, myFilesize);
                                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                                break;
                            }
                        }
                    }
                }
            }
            
            if (fileHas.Exists(x => x.Name == "DIR")) {
                dbpfFile.Seek(chunkOffset + indexData[dirnum].offset, SeekOrigin.Begin);
                LogMessage = string.Format("P{0}/DIR - Entry offset: {1}", packageparsecount, indexData[dirnum].offset);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                if ((indexMajorVersion == 7) && indexMinorVersion == 1)
                {
                    numRecords = indexData[dirnum].filesize / 20;
                } else {
                    numRecords = indexData[dirnum].filesize / 16;
                }

                LogMessage = string.Format("P{0}/DIR - Number of Records: {1}", packageparsecount, numRecords);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                
                for (int j = 0; j < numRecords; j++) {
                    indexEntry holderEntry = new indexEntry();
                    LogMessage = string.Format("P{0}/DIR - Compressed Entry {1}", packageparsecount, j);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                    typeID = readFile.ReadUInt32().ToString("X8");
                    LogMessage = string.Format("P{0}/DIR/CR{1} - Type ID: {2}", packageparsecount, j, typeID);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                    groupID = readFile.ReadUInt32().ToString("X8");
                    LogMessage = string.Format("P{0}/DIR/CR{1} - Group ID: {2}", packageparsecount, j, groupID);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                    instanceID = readFile.ReadUInt32().ToString("X8");
                    LogMessage = string.Format("P{0}/DIR/CR{1} - Insance ID: {2}", packageparsecount, j, instanceID);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                    holderEntry.instanceID = readFile.ReadUInt32().ToString("X8");
                    //Console.WriteLine(holderEntry.instanceID);
                    allInstanceIDs.Add(new PackageInstance() {InstanceID = holderEntry.instanceID.ToString()});
                    if (indexMajorVersion == 7 && indexMinorVersion == 1) {
                        instanceID2 = readFile.ReadUInt32().ToString("X8");
                        LogMessage = string.Format("P{0}/DIR/CR{1} - InstanceID2: {2}", packageparsecount, j, instanceID2);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                    }
                    compfilesize = readFile.ReadUInt32();
                    LogMessage = string.Format("P{0}/DIR/CR{1} - FileSize: {2}", packageparsecount, j, compfilesize);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));

                    int idxcount = 0;      
                    foreach (indexEntry idx in indexData) {
                        typefound = "";
                        idxcount++;
                        LogMessage = string.Format("P{0}/DIR/CR{1} - IDX Type: {2}", packageparsecount, j, idx.typeID);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                        foreach (typeList type in TypeListings.AllTypesS2) {
                            if (idx.typeID == type.typeID) {
                                LogMessage = string.Format("P{0}/DIR/CR{1} - IDX Type matched to: {2}", packageparsecount, j, type.info);
                                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                                typefound = type.desc;
                            }
                        }
                        LogMessage = string.Format("P{0}/DIR/CR{1} - Reading IDX {2}", packageparsecount, j, idxcount);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                        cFileSize = 0;
                        cTypeID = "";

                        if (typefound == "CTSS"){
                            LogMessage = string.Format("P{0}/DIR/CR{1}/IDX{2} - Found CTSS", packageparsecount, j, idxcount);
                            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                            dbpfFile.Seek(this.chunkOffset + idx.offset, SeekOrigin.Begin);
                            cFileSize = readFile.ReadInt32();
							cTypeID = readFile.ReadUInt16().ToString("X4");
                            if (cTypeID == "FB10") 
							{
                                LogMessage = string.Format("P{0}/DIR/CR{1}/IDX{2} - cTypeID: {3}", packageparsecount, j, idxcount, cTypeID);
								byte[] tempBytes = readFile.ReadBytes(3);
								uint cFullSize = readentries.QFSLengthToInt(tempBytes);

								DecryptByteStream decompressed = new DecryptByteStream(readentries.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));

								ctssvar = readentries.readCTSSchunk(decompressed);
                                LogMessage = string.Format("P{0}/DIR/CR{1}/IDX{2} - CTSSVar returned with: {3}", packageparsecount, j, idxcount, ctssvar.SimsPackagetoString());
                                
							} 
							else 
							{
                                LogMessage = string.Format("P{0}/DIR/CR{1}/IDX{2} - cTypeID: {3}", packageparsecount, j, idxcount, cTypeID);
								dbpfFile.Seek(this.chunkOffset + idx.offset, SeekOrigin.Begin);
								ctssvar = readentries.readCTSSchunk(readFile);
                                LogMessage = string.Format("P{0}/DIR/CR{1}/IDX{2} - CTSSVar returned with: {3}", packageparsecount, j, idxcount, ctssvar.SimsPackagetoString());
							}
                        } else if (typefound == "XOBJ" || typefound == "XFNC" || typefound == "XFLR" || typefound == "XMOL" || typefound == "XROF"  || typefound == "XTOL"  || typefound == "XHTN"){
                            LogMessage = string.Format("P{0}/DIR/CR{1}/IDX{2} - Type: {3}", packageparsecount, j, idxcount, typefound);
                            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                            dbpfFile.Seek(this.chunkOffset + idx.offset, SeekOrigin.Begin);
                            cFileSize = readFile.ReadInt32();
                            cTypeID = readFile.ReadUInt16().ToString("X4");
                            LogMessage = string.Format("P{0}/DIR/CR{1}/IDX{2} - Type: {3}, FileSize: {4}, cTypeID: {5}", packageparsecount, j, idxcount, typefound, cFileSize, cTypeID);
                            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                            if (cTypeID == "FB10"){
                                LogMessage = string.Format("P{0}/DIR/CR{1}/IDX{2} - {3} confirmed.", packageparsecount, j, idxcount, cTypeID);
                                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                                byte[] tempBytes = readFile.ReadBytes(3);
                                uint cFullSize = readentries.QFSLengthToInt(tempBytes);
                                LogMessage = string.Format("P{0}/DIR/CR{1}/IDX{2} - cFullSize: {3}.", packageparsecount, j, idxcount, cFileSize);
                                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                                string cpfTypeID = readFile.ReadUInt32().ToString("X8");
                                LogMessage = string.Format("P{0}/DIR/CR{1}/IDX{2} - cpfTypeID: {3}.", packageparsecount, j, idxcount, cpfTypeID);
                                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                                if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0")){
                                    dirvar = readentries.readCPFchunk(readFile);
                                    LogMessage = "Real CPF file. Processing as CPF chunk.";
                                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                                } else {
                                    LogMessage = string.Format("Not a real CPF. Searching for more information.");
                                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                                    dbpfFile.Seek(this.chunkOffset + idx.offset + 9, SeekOrigin.Begin);
                                    DecryptByteStream decompressed = new DecryptByteStream(readentries.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));
                                    if (cpfTypeID == "E750E0E2")
                                    {
                                        // Read first four bytes                                        
                                        cpfTypeID = decompressed.ReadUInt32().ToString("X8");
                                        LogMessage = string.Format("P{0}/DIR/CR{1}/IDX{2} - Secondary cpfTypeID: {3}.", packageparsecount, j, idxcount, cpfTypeID);
                                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                                        if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0")) 
                                        {
                                            LogMessage = string.Format("Real CPF. Decompressing.");
                                            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                                            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                                            dirvar = readentries.readCPFchunk(decompressed);
                                            LogMessage = string.Format("P{0}/DIR/CR{1}/IDX{2} - DirVar returned with: {3}.", packageparsecount, j, idxcount, dirvar.SimsPackagetoString());
                                            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                                            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                                        } 
                                    } else                                     {
                                        LogMessage = string.Format("Actually an XML. Reading.");
                                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                                        dirvar = readentries.readXMLchunk(decompressed);
                                        LogMessage = string.Format("P{0}/DIR/CR{1}/IDX{2} - DirVar returned with: {3}.", packageparsecount, j, idxcount, dirvar.SimsPackagetoString());
                                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                                    }
                                }

                            } else {
                                LogMessage = string.Format("P{0}/DIR/CR{1}/IDX{2} - cTypeID is not FB10: {3}.", packageparsecount, j, idxcount, cTypeID);
                                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));

                            }
                        }

                    }

                }
            }
            

            if (fileHas.Exists(x => x.Name == "OBJD")) {
                LogMessage = string.Format("P{0} - This file has an OBJD.", packageparsecount);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));       
                int fh = 0;
                var objdnum = (from has in fileHas
                        where has.Name =="OBJD"
                        select has.Location).ToList();
                int c = 0;
                foreach (int objloc in objdnum) {                                
                    
                    dbpfFile.Seek(this.chunkOffset + indexData[objloc].offset, SeekOrigin.Begin);
                    cFileSize = readFile.ReadInt32();
                    LogMessage = string.Format("P{0}/OBJD{1} - FileSize: {2}.", packageparsecount, c, cFileSize);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                    cTypeID = readFile.ReadUInt16().ToString("X4");
                    LogMessage = string.Format("P{0}/OBJD{1} - cTypeID: {2}.", packageparsecount, c, cTypeID);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                    if (cTypeID == "FB10")
                    {
                        LogMessage = string.Format("P{0}/OBJD{1} - cTypeID confirmed as: {2}.", packageparsecount, c, cTypeID);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                        byte[] tempBytes = readFile.ReadBytes(3);
                        LogMessage = string.Format("P{0}/OBJD{1} - Temp Bytes: {2}.", packageparsecount, c, tempBytes);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                        uint cFullSize = readentries.QFSLengthToInt(tempBytes);
                        LogMessage = string.Format("P{0}/OBJD{1} - Full Size: {2}.", packageparsecount, c, cFullSize);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                        DecryptByteStream decompressed = new DecryptByteStream(readentries.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));
                        objdvar = readentries.readOBJDchunk(decompressed);
                        LogMessage = string.Format("P{0}/OBJD{1} - OBJDVar returned with: {2}.", packageparsecount, c, objdvar.SimsPackagetoString());
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                    } else { 
                        dbpfFile.Seek(this.chunkOffset + indexData[objloc].offset, SeekOrigin.Begin);
                        objdvar = readentries.readOBJDchunk(readFile);
                        LogMessage = string.Format("P{0}/OBJD{1} - OBJDVar returned with: {2}.", packageparsecount, c, objdvar.SimsPackagetoString());
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                    }
                }
                
            }

            if (fileHas.Exists(x => x.Name == "STR#"))
            {
                LogMessage = string.Format("P{0} - This file has a STR.", packageparsecount);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));       
                var strnm = (from has in fileHas
                        where has.Name =="STR#"
                        select has.Location).ToList();
                int c = 0;
                
                foreach (int strloc in strnm) {
                    LogMessage = string.Format("P{0}/STR#{1} - TypeID: {2}.", packageparsecount, c, indexData[strloc].typeID);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                    dbpfFile.Seek(chunkOffset + indexData[strloc].offset, SeekOrigin.Begin);
                    cFileSize = readFile.ReadInt32();
                    LogMessage = string.Format("P{0}/STR#{1} - cFileSize: {2}.", packageparsecount, c, cFileSize);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                    cTypeID = readFile.ReadUInt16().ToString("X4");
                    LogMessage = string.Format("P{0}/STR#{1} - cTypeID: {2}.", packageparsecount, c, cTypeID);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                    if (cTypeID == "FB10")
                    {
                        LogMessage = string.Format("P{0}/STR#{1} - cTypeID Confirmed as: {2}.", packageparsecount, c, cTypeID);
                        byte[] tempBytes = readFile.ReadBytes(3);
                        uint cFullSize = readentries.QFSLengthToInt(tempBytes);

                        DecryptByteStream decompressed = new DecryptByteStream(readentries.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));

                        strvar = readentries.readSTRchunk(decompressed);
                        LogMessage = string.Format("P{0}/STR#{1} - STRVar Returned With: {2}.", packageparsecount, c, strvar.SimsPackagetoString());
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                    } 
                    else 
                    {
                        strvar = readentries.readSTRchunk(readFile);
                        LogMessage = string.Format("P{0}/STR#{1} - STRVar Returned With: {2}.", packageparsecount, c, strvar.SimsPackagetoString());
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                    }                    
                }
                c++;
            }
            if (fileHas.Exists(x => x.Name == "MMAT"))
            {
                LogMessage = string.Format("P{0} - This file has an MMAT.", packageparsecount);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));

                var entryspots = (from has in fileHas
                        where has.Name =="MMAT"
                        select has.Location).ToList();
                int mm = 0;
                foreach (int mmatloc in entryspots){
                    dbpfFile.Seek(this.chunkOffset + indexData[mmatloc].offset, SeekOrigin.Begin);
                    cFileSize = readFile.ReadInt32();
                    LogMessage = string.Format("P{0}/MMAT{2} - FileSize: {1}.", packageparsecount, cFileSize, mm);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                    cTypeID = readFile.ReadUInt16().ToString("X4");
                    LogMessage = string.Format("P{0}/MMAT{2} - cTypeID: {1}.", packageparsecount, cTypeID, mm);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                    
                    if (cTypeID == "FB10") 
                    {
                        byte[] tempBytes = readFile.ReadBytes(3);
                        uint cFullSize = readentries.QFSLengthToInt(tempBytes);

                        LogMessage = string.Format("P{0}/MMAT{2} - cFullSize: {1}.", packageparsecount, cFullSize, mm);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                        string cpfTypeID = readFile.ReadUInt32().ToString("X8");
                        LogMessage = string.Format("P{0}/MMAT{2} - cpfTypeID: {1}.", packageparsecount, cpfTypeID, mm);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                        if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0"))
                        {
                            mmatvar = readentries.readCPFchunk(readFile);
                            LogMessage = string.Format("P{0}/MMAT{2} - MMATVar Returned with: {1}.", packageparsecount, mmatvar.SimsPackagetoString(), mm);
                            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));                        
                        } 
                        else 
                        {
                            dbpfFile.Seek(this.chunkOffset + indexData[mmatloc].offset + 9, SeekOrigin.Begin);
                            DecryptByteStream decompressed = new DecryptByteStream(readentries.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));

                            if (cpfTypeID == "E750E0E2") 
                            {

                                cpfTypeID = decompressed.ReadUInt32().ToString("X8");

                                if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0")) 
                                {
                                    mmatvar = readentries.readCPFchunk(decompressed);
                                    LogMessage = string.Format("P{0}/MMAT{2} - MMATVar Returned with: {1}.", packageparsecount, mmatvar.SimsPackagetoString(), mm);
                                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                                }

                            } 
                            else 
                            {
                                mmatvar = readentries.readXMLchunk(decompressed);
                                LogMessage = string.Format("P{0}/MMAT{2} - MMATVar Returned with: {1}.", packageparsecount, mmatvar.SimsPackagetoString(), mm);
                                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                            }
                        }
                    } 
                    else 
                    {
                        dbpfFile.Seek(this.chunkOffset + indexData[mmatloc].offset, SeekOrigin.Begin);

                        string cpfTypeID = readFile.ReadUInt32().ToString("X8");
                        if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0"))
                        {
                            mmatvar = readentries.readCPFchunk(readFile);
                            LogMessage = string.Format("P{0}/MMAT{2} - MMATVar Returned with: {1}.", packageparsecount, mmatvar.SimsPackagetoString(), mm);
                            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
                        }

                        if  (cpfTypeID == "6D783F3C")
                        {
                            dbpfFile.Seek(this.chunkOffset + indexData[mmatloc].offset, SeekOrigin.Begin);

                            string xmlData = Encoding.UTF8.GetString(readFile.ReadBytes((int)indexData[mmatloc].filesize));
                            mmatvar = readentries.readXMLchunk(xmlData);
                            LogMessage = string.Format("P{0}/MMAT{2} - MMATVar Returned with: {1}.", packageparsecount, mmatvar.SimsPackagetoString(), mm);
                            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));

                        }
                    }
                    mm++;
                }
                                
                
            }

            /*if (fileHas.Exists(x => x.term == "IMG"))
            {
                int fh = 0;
                foreach (PackageEntries item in fileHas) {
                    if (item.term == "IMG"){
                        imgnm.Add(fh);
                    }
                    fh++;
                }

                dbpfFile.Seek(this.chunkOffset + indexData[mmatloc].offset, SeekOrigin.Begin);
                cFileSize = readFile.ReadInt32();
                cTypeID = readFile.ReadUInt16().ToString("X4");




                if (cTypeID == "FB10") 
                {
                    byte[] tempBytes = readFile.ReadBytes(3);
                    uint cFullSize = readentries.QFSLengthToInt(tempBytes);

                    string cpfTypeID = readFile.ReadUInt32().ToString("X8");
                    if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0"))
                    {
                        imgvar = readentries.readCPFchunk(readFile);
                    } 
                    else 
                    {
                        dbpfFile.Seek(this.chunkOffset + indexData[mmatloc].offset + 9, SeekOrigin.Begin);
                        DecryptByteStream decompressed = new DecryptByteStream(readentries.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));

                        if (cpfTypeID == "E750E0E2") 
                        {

                            cpfTypeID = decompressed.ReadUInt32().ToString("X8");

                            if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0")) 
                            {
                                mmatvar = readentries.readCPFchunk(decompressed);
                            }

                        } 
                        else 
                        {
                            mmatvar = readentries.readXMLchunk(decompressed);
                        }
                    }
                } 
                else 
                {
                    dbpfFile.Seek(this.chunkOffset + indexData[mmatloc].offset, SeekOrigin.Begin);

                    string cpfTypeID = readFile.ReadUInt32().ToString("X8");
                    if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0"))
                    {
                        mmatvar = readentries.readCPFchunk(readFile);
                    }

                    if  (cpfTypeID == "6D783F3C")
                    {
                        dbpfFile.Seek(this.chunkOffset + indexData[mmatloc].offset, SeekOrigin.Begin);

                        string xmlData = Encoding.UTF8.GetString(readFile.ReadBytes((int)indexData[mmatloc].filesize));
                        mmatvar = readentries.readXMLchunk(xmlData);

                    }
                }
            }*/

            LogMessage = string.Format("All methods complete, moving on to getting info.");

            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
            LogMessage = string.Format("P{0} - DirVar Contains: {1}.", packageparsecount, dirvar.SimsPackagetoString());
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
            LogMessage = string.Format("P{0} - CTSSVar Contains: {1}.", packageparsecount, ctssvar.SimsPackagetoString());
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
            LogMessage = string.Format("P{0} - MMATVar Contains: {1}.", packageparsecount, mmatvar.SimsPackagetoString());
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
            LogMessage = string.Format("P{0} - OBJDVar Contains: {1}.", packageparsecount, objdvar.SimsPackagetoString());
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));
            LogMessage = string.Format("P{0} - Strvar Contains: {1}.", packageparsecount, strvar.SimsPackagetoString());
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}", LogMessage));

            List<PackageTypeCounter> typecount = new();

            foreach (typeList type in GlobalVariables.S2Types){
                var match = (from has in fileHas 
                            where has.TypeID == type.typeID
                            select has).ToList();
                if (match.Count >= 1){
                    PackageTypeCounter tc = new()
                    {
                        TypeDesc = type.desc,
                        Count = match.Count,
                        TypeID = type.typeID
                    };
                    LogMessage = string.Format("There are {0} of {1} ({2}) in this package.", tc.Count, tc.TypeDesc, tc.TypeID);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    typecount.Add(tc);
                }                
            }

            thisPackage.FileHas.AddRange(fileHas);
            thisPackage.Entries.AddRange(typecount);

            var XNGB = typecount.Where(Type => Type.TypeDesc == "XNGB").Select(Type => Type.Count).FirstOrDefault();
            var STR = typecount.Where(Type => Type.TypeDesc == "STR").Select(Type => Type.Count).FirstOrDefault();
            var TXTR = typecount.Where(Type => Type.TypeDesc == "TXTR").Select(Type => Type.Count).FirstOrDefault();
            var DIR = typecount.Where(Type => Type.TypeDesc == "DIR").Select(Type => Type.Count).FirstOrDefault();
            var TXMT = typecount.Where(Type => Type.TypeDesc == "TXMT").Select(Type => Type.Count).FirstOrDefault();
            var SHPE = typecount.Where(Type => Type.TypeDesc == "SHPE").Select(Type => Type.Count).FirstOrDefault();
            var BCON = typecount.Where(Type => Type.TypeDesc == "BCON").Select(Type => Type.Count).FirstOrDefault();
            var BHAV = typecount.Where(Type => Type.TypeDesc == "BHAV").Select(Type => Type.Count).FirstOrDefault();
            var MMAT = typecount.Where(Type => Type.TypeDesc == "MMAT").Select(Type => Type.Count).FirstOrDefault();
            var OBJF = typecount.Where(Type => Type.TypeDesc == "OBJF").Select(Type => Type.Count).FirstOrDefault();
            var OBJD = typecount.Where(Type => Type.TypeDesc == "OBJD").Select(Type => Type.Count).FirstOrDefault();
            var CLST = typecount.Where(Type => Type.TypeDesc == "CLST").Select(Type => Type.Count).FirstOrDefault();
            var XOBJ = typecount.Where(Type => Type.TypeDesc == "XOBJ").Select(Type => Type.Count).FirstOrDefault();
            var CRES = typecount.Where(Type => Type.TypeDesc == "CRES").Select(Type => Type.Count).FirstOrDefault();
            var CTSS = typecount.Where(Type => Type.TypeDesc == "CTSS").Select(Type => Type.Count).FirstOrDefault();
            var COLL = typecount.Where(Type => Type.TypeDesc == "COLL").Select(Type => Type.Count).FirstOrDefault();
            var TRCN = typecount.Where(Type => Type.TypeDesc == "TRCN").Select(Type => Type.Count).FirstOrDefault();
            var TTAB = typecount.Where(Type => Type.TypeDesc == "TTAB").Select(Type => Type.Count).FirstOrDefault();
            var GLOB = typecount.Where(Type => Type.TypeDesc == "GLOB").Select(Type => Type.Count).FirstOrDefault();
            var GMDC = typecount.Where(Type => Type.TypeDesc == "GMDC").Select(Type => Type.Count).FirstOrDefault();
            var GMND = typecount.Where(Type => Type.TypeDesc == "GMND").Select(Type => Type.Count).FirstOrDefault();
            var GZPS = typecount.Where(Type => Type.TypeDesc == "GZPS").Select(Type => Type.Count).FirstOrDefault();
            var XHTN = typecount.Where(Type => Type.TypeDesc == "XHTN").Select(Type => Type.Count).FirstOrDefault();

            if (thisPackage.Override == false){
                if (XNGB >= 1 && STR >= 1){
                    thisPackage.Type = "Floor";
                } else if (TXTR >= 1 && STR >= 1 && DIR >= 1 && TXMT >= 1 && SHPE <= 0 && BCON <= 0 && BHAV <= 0 && MMAT <= 0 && OBJF <= 0 && CLST <= 0){
                    thisPackage.Type = "Floor";
                } else if (TXTR >= 1 && STR >= 1 && DIR >= 1 && SHPE <= 0 && BCON <= 0 && BHAV <= 0 && MMAT <= 0 && OBJF <= 0 && OBJD <= 0 && CLST <= 0 && XOBJ <= 0){
                    thisPackage.Type = "Terrain Paint";
                } else if (BCON >= 1 && BHAV >= 1 && CRES >= 1 && CTSS >= 1){
                    thisPackage.Type = "Functional Object";
                } else if (COLL >= 1){
                    thisPackage.Type = "Collection";
                } else if (BCON >= 1 && TRCN >= 1 && BHAV <= 0 && TTAB <= 0){
                    thisPackage.Type = "Tuning Mod";
                } else if (BHAV >= 1 && GLOB >= 1 && OBJD >= 1 && GMDC <= 0 && GMND <= 0){
                    thisPackage.Type = "Mod";
                } else if (MMAT >= 1 && DIR >= 1 && TXTR >= 1 && GMDC <= 0 && GMND <= 0){
                    thisPackage.Type = "Object Recolor";
                } else if (BHAV >= 1 && GMND >= 1 && GMDC >= 1 && SHPE >= 1){
                    thisPackage.Type = "Object Mesh";
                } else if (BHAV >= 1 && GMND >= 1 && GMDC >= 1 && CTSS >= 1){
                    thisPackage.Type = "Object";
                } else if (TXMT >= 1 && TXTR >= 1 && GZPS >= 1 && GMND <= 0 && GMDC <= 0 && CRES <= 0){
                    thisPackage.Type = "Clothing Recolor";
                } else if (SHPE >= 1 && CRES >= 1 && GMDC >= 1 && OBJD <= 0){
                    thisPackage.Type = "Body Mesh";
                } else if (XHTN >= 1 && SHPE >= 1){
                    thisPackage.Type = "Hair";
                } else if (XHTN >= 1 && SHPE == 0){
                    thisPackage.Type = "Hair Recolor";
                } else if (GMDC >= 1 && GMND >= 1 && SHPE >= 1) {
                    thisPackage.Type = "Misc Mesh";
                } else {
                    thisPackage.Type = "Currently Unknown";
                }
            }
            
            LogMessage = string.Format("P{0} is a {1}", packageparsecount, thisPackage.Type);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));  
        
           
            #region Get Title & Description 
            
            if (!String.IsNullOrWhiteSpace(ctssvar.Title) && string.IsNullOrWhiteSpace(thisPackage.Title)) {
                thisPackage.Title = ctssvar.Title;
            } else if (!String.IsNullOrWhiteSpace(dirvar.Title) && string.IsNullOrWhiteSpace(thisPackage.Title)){
                thisPackage.Title = dirvar.Title;
            } else if (!String.IsNullOrWhiteSpace(objdvar.Title) && string.IsNullOrWhiteSpace(thisPackage.Title)) {
                thisPackage.Title = objdvar.Title;
            } else if (!String.IsNullOrWhiteSpace(strvar.Title) && string.IsNullOrWhiteSpace(thisPackage.Title)) {
                thisPackage.Title = strvar.Title;
            } else if (!String.IsNullOrWhiteSpace(mmatvar.Title) && string.IsNullOrWhiteSpace(thisPackage.Title)) {
                thisPackage.Title = mmatvar.Title;
            }


            if (!String.IsNullOrWhiteSpace(ctssvar.Description) && string.IsNullOrWhiteSpace(thisPackage.Description)){
                thisPackage.Description = ctssvar.Description;
            } else if (!String.IsNullOrWhiteSpace(dirvar.Description) && string.IsNullOrWhiteSpace(thisPackage.Description)) {
                thisPackage.Description = dirvar.Description;
            } else if (!String.IsNullOrWhiteSpace(objdvar.Description) && string.IsNullOrWhiteSpace(thisPackage.Description)) {
                thisPackage.Description = objdvar.Description;
            } else if (!String.IsNullOrWhiteSpace(strvar.Description) && string.IsNullOrWhiteSpace(thisPackage.Description)) {
                thisPackage.Description = strvar.Description;
            } else if (!String.IsNullOrWhiteSpace(mmatvar.Description) && string.IsNullOrWhiteSpace(thisPackage.Description)) {
                thisPackage.Description = mmatvar.Description;
            }

            #endregion

            #region Get Info

                if (!String.IsNullOrWhiteSpace(dirvar.Subtype)){
                    thisPackage.Subtype = dirvar.Subtype;
                }
                if (!String.IsNullOrWhiteSpace(dirvar.Type)){
                    thisPackage.Type = dirvar.Type;
                }
                if (!String.IsNullOrWhiteSpace(dirvar.Category)){
                    thisPackage.Category = dirvar.Category;
                }
                if (!String.IsNullOrWhiteSpace(dirvar.ModelName)){
                    thisPackage.ModelName = dirvar.ModelName;
                }
                if (dirvar.GUIDs?.Any() == true){
                    foreach (PackageGUID guid in dirvar.GUIDs){
                      allGUIDS.Add(new PackageGUID(){GuidID = guid.GuidID});
                      thisPackage.MeshKeys.Add(new PackageMeshKeys() {MeshKey = guid.GuidID});
                    }
                }
                if (mmatvar.GUIDs?.Any() == true){
                    foreach (PackageGUID guid in mmatvar.GUIDs){
                      allGUIDS.Add(new PackageGUID(){GuidID = guid.GuidID});
                      thisPackage.MeshKeys.Add(new PackageMeshKeys() {MeshKey = guid.GuidID});
                    }
                }
                if (!String.IsNullOrWhiteSpace(dirvar.Creator)){
                    thisPackage.Creator = dirvar.Creator;
                }
                if (!String.IsNullOrWhiteSpace(dirvar.Age)){
                    thisPackage.Age = dirvar.Age;
                }
                if (!String.IsNullOrWhiteSpace(dirvar.Gender)){
                    thisPackage.Gender = dirvar.Gender;
                }

            #endregion
            
            #region Get Function

            if (thisPackage.Title.Contains("bedding")){
                thisPackage.Type = "Bedding Recolor";
            }

            if (!String.IsNullOrWhiteSpace(objdvar.Function)){
                thisPackage.Function = objdvar.Function;
            }
            if (!String.IsNullOrWhiteSpace(objdvar.Function)){
                thisPackage.FunctionSubcategory = objdvar.FunctionSubcategory;
            }
            thisPackage.RequiredEPs = objdvar.RequiredEPs;
            thisPackage.RoomSort = objdvar.RoomSort;
            allGUIDS.AddRange(objdvar.GUIDs);
            
            if (String.IsNullOrWhiteSpace(thisPackage.Type)){
                thisPackage.Type = thisPackage.Function;
            }
            if (!String.IsNullOrWhiteSpace(thisPackage.Subtype)){
                thisPackage.Subtype = thisPackage.FunctionSubcategory;
            }
            if (thisPackage.Type == "floor") {
                thisPackage.Type = "Floor";
            } else if (thisPackage.Type == "wallpaper") {
                thisPackage.Type = "Wallpaper";
            } else if (thisPackage.Type == "terrainPaint") {
                thisPackage.Type = "Terrain Paint";
            }
            #endregion


            foreach (PackageInstance iid in allInstanceIDs){
                var inp = thisPackage.InstanceIDs.Where(c => c.InstanceID == iid.InstanceID);
                if (!inp.Any()){
                    thisPackage.InstanceIDs.Add(iid); 
                }                
            }
            foreach (PackageGUID gid in allGUIDS){
                var ing = thisPackage.GUIDs.Where(c => c.GuidID == gid.GuidID);
                if (!ing.Any()){
                    thisPackage.GUIDs.Add(gid); 
                }                
            }            

            dbpfFile.Close();
            dbpfFile.Dispose();
            readFile.Close();
            readFile.Dispose();
            
            log.MakeLog(string.Format("Package Summary: {0}", thisPackage.SimsPackagetoString()), false);
            LogMessage = string.Format("P{0} - Package Summary: {1}", packageparsecount, thisPackage.SimsPackagetoString());
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            log.MakeLog(string.Format("P{0} - Package Summary: {1}", packageparsecount, thisPackage.SimsPackagetoString()), false);

            LogMessage = string.Format("Adding {0} to packages database.", thisPackage.PackageName);
            if (GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                                      
             
            LogMessage = string.Format("Closing package # {0}/{1}: {2}", packageparsecount, GlobalVariables.PackageCount, packageinfo.Name);
            if (GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                       
            
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            string elapsedtime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                ts.Hours, ts.Minutes, ts.Seconds,
                                ts.Milliseconds / 10);
            GlobalVariables.currentpackage = packageinfo.Name;
            GlobalVariables.packagesRead++;
            GlobalVariables.AddPackages.Enqueue(thisPackage);
            PackageFile packageFile = new PackageFile(){Name = thisPackage.PackageName, Location = thisPackage.Location, Game = thisPackage.Game};
            GlobalVariables.RemovePackages.Enqueue(packageFile);
            
            LogMessage = string.Format("Reading file {0} took {1}", packageinfo.Name, elapsedtime);
            if (GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
           // log.MakeLog(string.Format("Reading file {0} took {1}", packageinfo.Name, elapsedtime), false);

            thisPackage = new SimsPackage();
            infovar = new SimsPackage();
            ctssvar = new SimsPackage();
            dirvar = new SimsPackage();
            strvar = new SimsPackage();
            objdvar = new SimsPackage();
            mmatvar = new SimsPackage();
            fileHas = new List<PackageEntries>();
            linkData = new ArrayList();
            indexData = new List<indexEntry>();
            //packageinfo = new FileInfo(file); 
            allGUIDS = new();      
            allInstanceIDs = new();      
            return;    
        }
    }
}