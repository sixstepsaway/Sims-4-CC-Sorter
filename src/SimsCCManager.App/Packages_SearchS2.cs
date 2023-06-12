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
using SQLitePCL;
using SQLiteNetExtensions;
using SQLiteNetExtensions.Attributes;
using SQLiteNetExtensions.Extensions;
using SQLite;
using SimsCCManager.Packages.Sorting;

namespace SimsCCManager.Packages.Sims2Search
{    
    /// <summary>
    /// Sims 2 package reading. Gets all the information from inside S2 Package files and returns it for use.
    /// </summary>
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
        
        //Vars
        uint chunkOffset = 0;        
       

        public void SearchS2Packages(FileStream dbpfFile, FileInfo packageinfo, uint minor, int packageparsecount, StringBuilder LogFile) {
            LoggingGlobals log = new LoggingGlobals();
            ReadEntries readentries = new ReadEntries(); 
            //StringBuilder LogFile = new();
            string LogMessage = "";  
            FilesSort filesort = new FilesSort();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            LogMessage = string.Format("File {0} arrived for processing as Sims 2 file.", packageinfo.Name);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            /*string txt = string.Format("SELECT * FROM Processing_Reader where Name='{0}'", Methods.FixApostrophesforSQL(packageinfo.Name));
            List<PackageFile> queries = GlobalVariables.DatabaseConnection.Query<PackageFile>(txt);
            PackageFile query = queries[0];
            GlobalVariables.DatabaseConnection.Delete(query);
            PackageFile pk = new PackageFile { ID = query.ID, Name = packageinfo.Name, Location = packageinfo.FullName, Game = 4, Broken = false, Status = "Processing"};
            GlobalVariables.DatabaseConnection.Insert(pk);
            //Vars for Package Info
            queries = new List<PackageFile>();
            query = new PackageFile();
            pk = new PackageFile();*/
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
            SimsPackage shpevar = new SimsPackage();
            
            //locations
            
            long indexmajorloc = 24;
            
            long indexminorloc = 60;
        

            //Lists 
            
            List<fileHasList> fileHas = new List<fileHasList>();
            ArrayList linkData = new ArrayList();
            List<indexEntry> indexData = new List<indexEntry>();
            //List<string> iids = new List<string>();
            List<string> allGUIDS = new List<string>();      
            List<string> distinctGUIDS = new List<string>();  
            List<string> allInstanceIDs = new List<string>();      
            List<string> distinctInstanceIDs = new List<string>();  
            if (minor == 2) indexmajorloc = 24;
            if (minor == 1) indexmajorloc = 32;
            //create readers              
            //MemoryStream dbpfFile = Methods.ReadBytesToFile(packageinfo.FullName, (int)packageinfo.Length);
            
            BinaryReader readFile = new BinaryReader(dbpfFile);
    
            thisPackage.FileSize = (int)packageinfo.Length;   
            thisPackage.PackageName = packageinfo.Name;
            thisPackage.GameString = "The Sims 2";
            
            LogMessage = string.Format("Reading package # {0}/{1}: {2}", packageparsecount, GlobalVariables.PackageCount, packageinfo.Name);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            
            thisPackage.Location = packageinfo.FullName;            
            thisPackage.Game = 2;
            LogMessage = string.Format("Package #{0} registered as {1} and meant for Sims 2", packageparsecount, packageinfo.FullName);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));          
            
            readFile.BaseStream.Position = indexmajorloc;

            uint indexMajorVersion = readFile.ReadUInt32();
            LogMessage = string.Format("P{0} - Index Major: {1}", packageparsecount, indexMajorVersion.ToString());
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            
            uint indexCount = readFile.ReadUInt32();
            LogMessage = string.Format("P{0} - Index Count: {1}", packageparsecount, indexCount.ToString());
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            
            uint indexOffset = readFile.ReadUInt32();
            LogMessage = string.Format("P{0} - Index Offset: {1}", packageparsecount, indexOffset.ToString());
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            
            uint indexSize = readFile.ReadUInt32();
            LogMessage = string.Format("P{0} - Index Size: {1}", packageparsecount, indexSize.ToString());
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            
            readFile.BaseStream.Position = indexminorloc;

            uint indexMinorVersion = readFile.ReadUInt32() -1;
            LogMessage = string.Format("P{0} - Index Minor Version: {1}", packageparsecount, indexMinorVersion.ToString());
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            LogMessage = string.Format("P{0} - Chunk Offset: {1}", packageparsecount, chunkOffset.ToString());
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

            readFile.BaseStream.Position = chunkOffset + indexOffset;
            for (int i = 0; i < indexCount; i++) {
                indexEntry holderEntry = new indexEntry();
                holderEntry.typeID = readFile.ReadUInt32().ToString("X8");               
                
                List<typeList> type = GlobalVariables.S4FunctionTypesConnection.Query<typeList>(string.Format("SELECT * FROM S2Types where TypeID='{0}'", holderEntry.typeID));
                    
                if(type.Any()){
                    fileHas.Add(new fileHasList {TypeID = type[0].desc, Location = i});
                    LogMessage = string.Format("P{0}/E{1} - {2} is at location {3}", packageparsecount, i, type[0].desc, i);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                } else {
                    fileHas.Add(new fileHasList() { TypeID = holderEntry.typeID, Location = i});
                    LogMessage = string.Format("P{0}/E{1} - {2} is unidentified and at location {2}", packageparsecount, i, holderEntry.typeID, i);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                }
                type = new List<typeList>();

                holderEntry.groupID = readFile.ReadUInt32().ToString("X8");
                holderEntry.instanceID = readFile.ReadUInt32().ToString("X8");                
                allInstanceIDs.Add(holderEntry.instanceID.ToString());

                if ((indexMajorVersion == 7) && (indexMinorVersion == 1)) {
                    holderEntry.instanceID2 = readFile.ReadUInt32().ToString("X8");
                } else {
                    holderEntry.instanceID2 = "00000000";
                }
                holderEntry.offset = readFile.ReadUInt32();
                holderEntry.filesize = readFile.ReadUInt32();
                holderEntry.truesize = 0;
                holderEntry.compressed = false;
                LogMessage = string.Format("P{0}/E{1} - Index Entry Information: \n-- TypeID: {2}\n-- GroupID: {3}\n-- InstanceID: {4}\n-- InstanceID2: {5}\n-- Offset: {6}\n-- FileSize: {7}\n-- TrueSize: {8}", packageparsecount, i, holderEntry.typeID, holderEntry.groupID, holderEntry.instanceID, holderEntry.instanceID2, holderEntry.offset, holderEntry.filesize, holderEntry.truesize);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));               


                indexData.Add(holderEntry);

                holderEntry = null;

                if (indexCount == 0) 
                {
                    LogMessage = string.Format("P{0} - Package is broken. Closing.", packageparsecount);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    readFile.Close();
                    return;
                }
                
            }


            
            string fhh = "";
            foreach (fileHasList item in fileHas) {
                if (string.IsNullOrWhiteSpace(fhh)){
                    fhh = string.Format("{0} at {1}", item.TypeID, item.Location);                    
                } else {
                    fhh += string.Format("\n {0} at {1}", item.TypeID, item.Location);  
                }
            }
            LogMessage = string.Format("P{0} - This file has: \n{1}", packageparsecount, fhh);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

            if (fileHas.Exists(x => x.TypeID == "DIR")) {       
                
                IEnumerable<int> dirget = from has in fileHas
                            where has.TypeID == "DIR"
                            select has.Location;
                List<int> dirs = dirget.ToList();
                dirget = Enumerable.Empty<int>();
                dirnum = dirs[0];                

                
                LogMessage = string.Format("P{0} - DIR is at entry [{1}]", packageparsecount, dirnum);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                //entrynum = 0;
            
                //go through dir entry specifically 
                
                numRecords = 0;
                typeID = "";
                groupID = "";
                instanceID = "";
                instanceID2 = "";
                myFilesize = 0;
                
                LogMessage = string.Format("P{0} - DIR confirmation: {1}", packageparsecount, indexData[dirnum].typeID);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

                //dbpfFile.Seek(this.chunkOffset + indexData[dirnum].offset, SeekOrigin.Begin);
                readFile.BaseStream.Position = this.chunkOffset + indexData[dirnum].offset;
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
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                log.MakeLog("P" + packageparsecount + " - Number of compressed records in entry:" + numRecords, true);
                
                for (int c = 0; c < numRecords; c++)
                {
                    indexEntry holderEntry = new indexEntry();
                    typeID = readFile.ReadUInt32().ToString("X8");
                    groupID = readFile.ReadUInt32().ToString("X8");
                    instanceID = readFile.ReadUInt32().ToString("X8");
                    holderEntry.instanceID = readFile.ReadUInt32().ToString("X8");
                    allInstanceIDs.Add(holderEntry.instanceID.ToString());
                    if (indexMajorVersion == 7 && indexMinorVersion == 1) instanceID2 = readFile.ReadUInt32().ToString("X8");
                    myFilesize = readFile.ReadUInt32();

                    LogMessage = string.Format("P{0}/DIR CR#{1} - Details: \n-- TypeID: {2}\n-- GroupID: {3}\n-- InstanceID: {4}\n-- InstanceID2: {5}\n-- FileSize: {6}", packageparsecount, c, typeID, groupID, instanceID, instanceID2, myFilesize);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

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
                                }
                            } 
                            else
                            {
                                idx.compressed = true;
                                idx.truesize = myFilesize;
                            }
                        }
                    }
                }
            }
            
            if (fileHas.Exists(x => x.TypeID == "DIR")) {
                readFile.BaseStream.Position = chunkOffset + indexData[dirnum].offset;
                if ((indexMajorVersion == 7) && indexMinorVersion == 1)
                {
                    numRecords = indexData[dirnum].filesize / 20;
                } else {
                    numRecords = indexData[dirnum].filesize / 16;
                }

                LogMessage = string.Format("P{0}/DIR - Reading compressed entries, of which there are {1}", packageparsecount, numRecords);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                
                for (int j = 0; j < numRecords; j++) {
                    indexEntry holderEntry = new indexEntry();
                    typeID = readFile.ReadUInt32().ToString("X8");
                    groupID = readFile.ReadUInt32().ToString("X8");
                    instanceID = readFile.ReadUInt32().ToString("X8");
                    holderEntry.instanceID = readFile.ReadUInt32().ToString("X8");
                    allInstanceIDs.Add(holderEntry.instanceID.ToString());
                    instanceID2 = "";
                    if (indexMajorVersion == 7 && indexMinorVersion == 1) {
                        instanceID2 = readFile.ReadUInt32().ToString("X8");
                    }
                    compfilesize = readFile.ReadUInt32();

                    LogMessage = string.Format("P{0}/DIR CR#{1} - Details: \n-- TypeID: {2}\n-- GroupID: {3}\n-- InstanceID: {4}\n-- InstanceID2: {5}\n-- FileSize: {6}", packageparsecount, j, typeID, groupID, instanceID, instanceID2, compfilesize);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

                    int idxcount = 0;      
                    foreach (indexEntry idx in indexData) {
                        typefound = "";
                        idxcount++;

                        List<typeList> type = GlobalVariables.S4FunctionTypesConnection.Query<typeList>(string.Format("SELECT * FROM S2Types where TypeID='{0}'", holderEntry.typeID));
                    
                        if(type.Any()){                        
                            LogMessage = string.Format("P{0}/DIR CR#{1} - Index Type: {1}, which is {2}", packageparsecount, j, idx.typeID, type[0].desc);
                            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                            typefound = type[0].desc;
                        }

                        LogMessage = string.Format("P{0}/DIR CR#{1} - Now reading IDX {2}", packageparsecount, j, idxcount);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        cFileSize = 0;
                        cTypeID = "";

                        if (typefound == "CTSS"){
                            LogMessage = string.Format("P{0}/DIR CR#{1} - CTSS found. Confirmation: {1}", packageparsecount, j, typefound);
                            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                            readFile.BaseStream.Position = this.chunkOffset + idx.offset;
                            cFileSize = readFile.ReadInt32();
							cTypeID = readFile.ReadUInt16().ToString("X4");
                            if (cTypeID == "FB10") 
							{
								byte[] tempBytes = readFile.ReadBytes(3);
								uint cFullSize = readentries.QFSLengthToInt(tempBytes);

								DecryptByteStream decompressed = new DecryptByteStream(readentries.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));

								ctssvar = readentries.readCTSSchunk(decompressed);
                                
							} 
							else 
							{
								readFile.BaseStream.Position = this.chunkOffset + idx.offset;
								ctssvar = readentries.readCTSSchunk(readFile);
							}
                        } else if (typefound == "XOBJ" || typefound == "XFNC" || typefound == "XFLR" || typefound == "XMOL" || typefound == "XROF"  || typefound == "XTOL"  || typefound == "XHTN"){
                            LogMessage = string.Format("P{0}/DIR CR#{1} - Now reading IDX {2}: {3}", packageparsecount, j, idxcount, typefound);
                            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                            readFile.BaseStream.Position = this.chunkOffset + idx.offset;
                            cFileSize = readFile.ReadInt32();
                            cTypeID = readFile.ReadUInt16().ToString("X4");
                            LogMessage = string.Format("P{0}/DIR CR#{1} - {2} has a size of {3} and a compression type of {4}", packageparsecount, j, idxcount, typefound, cFileSize, cTypeID);
                            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                            if (cTypeID == "FB10"){
                                log.MakeLog("FB10 confirmed.", true);
                                byte[] tempBytes = readFile.ReadBytes(3);
                                uint cFullSize = readentries.QFSLengthToInt(tempBytes);
                                string cpfTypeID = readFile.ReadUInt32().ToString("X8");
                                LogMessage = string.Format("P{0}/DIR CR#{1} - {2} has a full size of {3} and a compression type of {4}", packageparsecount, j, idxcount, cFullSize, cpfTypeID);
                                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                                if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0")){
                                    dirvar = readentries.readCPFchunk(readFile);
                                    log.MakeLog("Real CPF file. Processing as CPF chunk.",true);
                                } else {
                                    log.MakeLog("Not a real CPF. Searching for more information.", true);
                                    //dbpfFile.Seek(this.chunkOffset + idx.offset + 9, SeekOrigin.Begin);
                                    readFile.BaseStream.Position = this.chunkOffset + idx.offset + 9;
                                    DecryptByteStream decompressed = new DecryptByteStream(readentries.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));
                                    if (cpfTypeID == "E750E0E2")
                                    {
                                        // Read first four bytes
                                        cpfTypeID = decompressed.ReadUInt32().ToString("X8");
                                        if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0")) 
                                        {
                                            log.MakeLog("Real CPF. Decompressing.", true);
                                            dirvar = readentries.readCPFchunk(decompressed);
                                        } 
                                    } else 
                                    {
                                        log.MakeLog("Actually an XML. Reading.", true);
                                        dirvar = readentries.readXMLchunk(decompressed);
                                    }
                                }

                            } else {
                                log.MakeLog("Not FB10.", true);

                            }
                        }

                    }

                }
            }
            

            if (fileHas.Exists(x => x.TypeID == "OBJD")) {
                
                IEnumerable<int> obd = from has in fileHas
                            where has.TypeID == "OBJD"
                            select has.Location;
                objdnum = obd.ToList();
                obd = Enumerable.Empty<int>();

                int obb = 0;
                
                foreach (int objloc in objdnum) {
                    obb++;               
                    
                    readFile.BaseStream.Position = this.chunkOffset + indexData[objloc].offset;
                    cFileSize = readFile.ReadInt32();
                    cTypeID = readFile.ReadUInt16().ToString("X4");
                    if (cTypeID == "FB10")
                    {
                        byte[] tempBytes = readFile.ReadBytes(3);
                        uint cFullSize = readentries.QFSLengthToInt(tempBytes);

                        LogMessage = string.Format("P{0}/OBJD{1} - Details: \n-- FileSize: {2}\n-- cTypeID: {3}\n-- Full Size: {4}", packageparsecount, obb, cFileSize, cTypeID, cFullSize);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

                        DecryptByteStream decompressed = new DecryptByteStream(readentries.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));
                        objdvar = readentries.readOBJDchunk(decompressed);
                    } else { 
                        readFile.BaseStream.Position = this.chunkOffset + indexData[objloc].offset;
                        objdvar = readentries.readOBJDchunk(readFile);
                    }
                }
                
            }

            if (fileHas.Exists(x => x.TypeID == "STR#"))
            {
                IEnumerable<int> strr = from has in fileHas
                            where has.TypeID == "STR#"
                            select has.Location;
                strnm = strr.ToList();
                strr = Enumerable.Empty<int>();

                int st = 0;
                
                foreach (int strloc in strnm) {
                    st++;
                    //dbpfFile.Seek(chunkOffset + indexData[strloc].offset, SeekOrigin.Begin);
                    readFile.BaseStream.Position = chunkOffset + indexData[strloc].offset;
                    cFileSize = readFile.ReadInt32();
                    cTypeID = readFile.ReadUInt16().ToString("X4");
                    LogMessage = string.Format("P{0}/STR{1} - Details: \n-- FileSize: {2}\n-- cTypeID: {3}", packageparsecount, st, cFileSize, cTypeID);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    if (cTypeID == "FB10")
                    {
                        byte[] tempBytes = readFile.ReadBytes(3);
                        uint cFullSize = readentries.QFSLengthToInt(tempBytes);

                        DecryptByteStream decompressed = new DecryptByteStream(readentries.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));

                        strvar = readentries.readSTRchunk(decompressed);
                    } 
                    else 
                    {
                        objdvar = readentries.readSTRchunk(readFile);
                    }                    
                }
                
            }
            if (fileHas.Exists(x => x.TypeID == "MMAT"))
            {
                List<int> entryspots = new List<int>();
                IEnumerable<int> csp = from has in fileHas
                        where has.TypeID == "MMAT"
                        select has.Location;
                entryspots = csp.ToList();
                csp = Enumerable.Empty<int>();
                mmatloc = entryspots[0];
                readFile.BaseStream.Position = this.chunkOffset + indexData[mmatloc].offset;
                cFileSize = readFile.ReadInt32();
                cTypeID = readFile.ReadUInt16().ToString("X4");
                
                if (cTypeID == "FB10") 
                {
                    byte[] tempBytes = readFile.ReadBytes(3);
                    uint cFullSize = readentries.QFSLengthToInt(tempBytes);

                    string cpfTypeID = readFile.ReadUInt32().ToString("X8");
                    if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0"))
                    {
                        mmatvar = readentries.readCPFchunk(readFile);
                    } 
                    else 
                    {
                        //dbpfFile.Seek(this.chunkOffset + indexData[mmatloc].offset + 9, SeekOrigin.Begin);
                        readFile.BaseStream.Position = this.chunkOffset + indexData[mmatloc].offset + 9;
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
                    //dbpfFile.Seek(this.chunkOffset + indexData[mmatloc].offset, SeekOrigin.Begin);
                    readFile.BaseStream.Position = this.chunkOffset + indexData[mmatloc].offset;

                    string cpfTypeID = readFile.ReadUInt32().ToString("X8");
                    if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0"))
                    {
                        mmatvar = readentries.readCPFchunk(readFile);
                    }

                    if  (cpfTypeID == "6D783F3C")
                    {
                        //dbpfFile.Seek(this.chunkOffset + indexData[mmatloc].offset, SeekOrigin.Begin);
                        readFile.BaseStream.Position = this.chunkOffset + indexData[mmatloc].offset;

                        string xmlData = Encoding.UTF8.GetString(readFile.ReadBytes((int)indexData[mmatloc].filesize));
                        mmatvar = readentries.readXMLchunk(xmlData);

                    }
                }
            }

            
            if (fileHas.Exists(x => x.TypeID == "SHPE"))
            {   
                List<int> entryspots = new List<int>();
                IEnumerable<int> csp = from has in fileHas
                        where has.TypeID == "SHPE"
                        select has.Location;
                entryspots = csp.ToList();
                csp = Enumerable.Empty<int>();
                int entryloc = entryspots[0];
                
                readFile.BaseStream.Position = this.chunkOffset + indexData[entryloc].offset;
                cFileSize = readFile.ReadInt32();
                cTypeID = readFile.ReadUInt16().ToString("X4");
                
                if (cTypeID == "FB10") 
                {
                    byte[] tempBytes = readFile.ReadBytes(3);
                    uint cFullSize = readentries.QFSLengthToInt(tempBytes);

                    string cpfTypeID = readFile.ReadUInt32().ToString("X8");
                    if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0"))
                    {
                        shpevar = readentries.readSHPEchunk(readFile);
                    } 
                    else 
                    {
                        //dbpfFile.Seek(this.chunkOffset + indexData[entryloc].offset + 9, SeekOrigin.Begin);
                        readFile.BaseStream.Position = this.chunkOffset + indexData[entryloc].offset + 9;
                        DecryptByteStream decompressed = new DecryptByteStream(readentries.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));

                        if (cpfTypeID == "E750E0E2") 
                        {

                            cpfTypeID = decompressed.ReadUInt32().ToString("X8");

                            if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0")) 
                            {
                                shpevar = readentries.readSHPEchunk(decompressed);
                            }

                        } 
                        else 
                        {
                            shpevar = readentries.readSHPEchunk(decompressed);
                        }
                    }
                } 
                else 
                {
                    //dbpfFile.Seek(this.chunkOffset + indexData[entryloc].offset, SeekOrigin.Begin);
                    readFile.BaseStream.Position = this.chunkOffset + indexData[entryloc].offset;

                    string cpfTypeID = readFile.ReadUInt32().ToString("X8");
                    if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0"))
                    {
                        mmatvar = readentries.readCPFchunk(readFile);
                    }

                    if  (cpfTypeID == "6D783F3C")
                    {
                        //dbpfFile.Seek(this.chunkOffset + indexData[entryloc].offset, SeekOrigin.Begin);
                        readFile.BaseStream.Position = this.chunkOffset + indexData[entryloc].offset;

                        string xmlData = Encoding.UTF8.GetString(readFile.ReadBytes((int)indexData[entryloc].filesize));
                        mmatvar = readentries.readXMLchunk(xmlData);

                    }
                }
            }

            /*if (fileHas.Exists(x => x.term == "IMG"))
            {
                int fh = 0;
                foreach (fileHasList item in fileHas) {
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

            log.MakeLog("All methods complete, moving on to getting info.", true);
            //LogMessage = "Dirvar contains: " + dirvar.ToString();
            //if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            //if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            //LogMessage = "Ctssvar contains: " + ctssvar.ToString();
            //if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            //if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            //LogMessage = "Mmatvar contains: " + mmatvar.ToString();
            //if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            //if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            //LogMessage = "Objdvar contains: " + objdvar.ToString();
            //if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            //if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            //LogMessage = "Strvar contains: " + strvar.ToString();
            //if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            //if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

            List<TypeCounter> typecount = new List<TypeCounter>();
            Dictionary<string, int> typeDict = new Dictionary<string, int>();

            List<typeList> typse = GlobalVariables.S4FunctionTypesConnection.Query<typeList>(string.Format("SELECT * FROM S2Types"));

            foreach (fileHasList item in fileHas){
                foreach (typeList type in typse){
                    if (type.desc == item.TypeID){
                        //LogMessage = string.Format("Added {0} to dictionary.", type.desc);
                        //if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        //if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        typeDict.Increment(type.desc);
                    }
                }
            }            

            foreach (KeyValuePair<string, int> type in typeDict){
                TypeCounter tc = new TypeCounter();
                tc.Type = type.Key;
                tc.Count = type.Value;
                LogMessage = string.Format("There are {0} of {1} in this package.", tc.Type, tc.Count);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                typecount.Add(tc);
            }
            typeDict = new Dictionary<string, int>();
            typse = new List<typeList>();


            thisPackage.FileHas.AddRange(fileHas);
            thisPackage.Entries.AddRange(typecount);

            
            int xngb; int str; int txtr; int dir; int txmt; int shpe; int bcon; int bhav; int mmat; int objf; int objd; int clst; int xobj; int cres; int ctss; int coll; int trcn; int ttab; int glob; int gmdc; int gmnd; int gzps; int xhtn; 
            

            if ((typeDict.TryGetValue("XNGB", out xngb) && xngb >= 1) && (typeDict.TryGetValue("STR#", out str) && str >= 1)){
                thisPackage.Type = "Floor";
            } else if ((typeDict.TryGetValue("TXTR", out txtr) && txtr >= 1) && (typeDict.TryGetValue("STR#", out str) && str >= 1) && (typeDict.TryGetValue("DIR", out dir) && dir >= 1) && (typeDict.TryGetValue("TXMT", out txmt) && txmt >= 1) && (typeDict.TryGetValue("SHPE", out shpe) && shpe <= 0) && (typeDict.TryGetValue("BCON", out bcon) && bcon <= 0) && (typeDict.TryGetValue("BHAV", out bhav) && bhav <= 0) && (typeDict.TryGetValue("MMAT", out mmat) && mmat <= 0) && (typeDict.TryGetValue("OBJF", out objf) && objf <= 0) && (typeDict.TryGetValue("OBJD", out objd) && objd <= 0) && (typeDict.TryGetValue("CLST", out clst) && clst <= 0)){
                thisPackage.Type = "Floor";
            } else if ((typeDict.TryGetValue("TXTR", out txtr) && txtr >= 1) && (typeDict.TryGetValue("STR#", out str) && str >= 1) && (typeDict.TryGetValue("DIR", out dir) && dir >= 1) && (typeDict.TryGetValue("SHPE", out shpe) && shpe <= 0) && (typeDict.TryGetValue("BCON", out bcon) && bcon <= 0) && (typeDict.TryGetValue("BHAV", out bhav) && bhav <= 0) && (typeDict.TryGetValue("MMAT", out mmat) && mmat <= 0) && (typeDict.TryGetValue("OBJF", out objf) && objf <= 0) && (typeDict.TryGetValue("OBJD", out objd) && objd <= 0) && (typeDict.TryGetValue("CLST", out clst) && clst <= 0) && (typeDict.TryGetValue("XOBJ", out xobj) && xobj <= 0)){
                thisPackage.Type = "Terrain Paint";
            } else if ((typeDict.TryGetValue("BCON", out bcon) && bcon >= 1) && (typeDict.TryGetValue("BHAV", out bhav) && bhav >= 1) && (typeDict.TryGetValue("CRES", out cres) && cres >= 1) && (typeDict.TryGetValue("CTSS", out ctss) && ctss >= 1)){
                thisPackage.Type = "Functional Object";
            } else if ((typeDict.TryGetValue("COLL", out coll) && coll >= 1)){
                thisPackage.Type = "Collection";
            } else if ((typeDict.TryGetValue("BCON", out bcon) && bcon >= 1) && (typeDict.TryGetValue("TRCN", out trcn) && trcn >= 1) && (typeDict.TryGetValue("BHAV", out bhav) && bhav <= 0) && (typeDict.TryGetValue("TTAB", out ttab) && ttab <= 0)){
                thisPackage.Type = "Tuning Mod";
            } else if ((typeDict.TryGetValue("BHAV", out bhav) && bhav >= 1) && (typeDict.TryGetValue("GLOB", out glob) && glob >= 1) && (typeDict.TryGetValue("OBJD", out objd) && objd >= 1) && (typeDict.TryGetValue("GMDC", out gmdc) && gmdc <= 0) && (typeDict.TryGetValue("GMND", out gmnd) && gmnd <= 0)){
                thisPackage.Type = "Mod";
            } else if ((typeDict.TryGetValue("MMAT", out mmat) && mmat >= 1) && (typeDict.TryGetValue("DIR", out dir) && dir >= 1) && (typeDict.TryGetValue("TXTR", out txtr) && txtr >= 1) && (typeDict.TryGetValue("GMDC", out gmdc) && gmdc <= 0) && (typeDict.TryGetValue("GMND", out gmnd) && gmnd <= 0)){
                thisPackage.Type = "Object Recolor";
            } else if ((typeDict.TryGetValue("BHAV", out bhav) && bhav >= 1) && (typeDict.TryGetValue("GMND", out gmnd) && gmnd >= 1) && (typeDict.TryGetValue("GMDC", out gmdc) && gmdc >= 1) && (typeDict.TryGetValue("SHPE", out shpe) && shpe >= 1)){
                thisPackage.Type = "Object Mesh";
            } else if ((typeDict.TryGetValue("BHAV", out bhav) && bhav >= 1) && (typeDict.TryGetValue("GMND", out gmnd) && gmnd >= 1) && (typeDict.TryGetValue("GMDC", out gmdc) && gmdc >= 1) && (typeDict.TryGetValue("CTSS", out ctss) && ctss >= 1)){
                thisPackage.Type = "Object";
            } else if ((typeDict.TryGetValue("TXMT", out txmt) && txmt >= 1) && (typeDict.TryGetValue("TXTR", out txtr) && txtr >= 1) && (typeDict.TryGetValue("GZPS", out gzps) && gzps >= 1) && (typeDict.TryGetValue("GMND", out gmnd) && gmnd <= 0) && (typeDict.TryGetValue("GMDC", out gmdc) && gmdc <= 0) && (typeDict.TryGetValue("CRES", out cres) && cres <= 0)){
                thisPackage.Type = "Clothing Recolor";
            } else if ((typeDict.TryGetValue("SHPE", out shpe) && shpe >= 1) && (typeDict.TryGetValue("CRES", out cres) && cres >= 1) && (typeDict.TryGetValue("GMDC", out gmdc) && gmdc >= 1) && (typeDict.TryGetValue("OBJD", out objd) && objd <= 0)){
                thisPackage.Type = "Body Mesh";
            } else if ((typeDict.TryGetValue("XHTN", out xhtn) && xhtn >= 1) && (typeDict.TryGetValue("SHPE", out shpe) && shpe >= 1)){
                thisPackage.Type = "Hair";
            } else if ((typeDict.TryGetValue("XHTN", out xhtn) && xhtn >= 1) && (typeDict.TryGetValue("SHPE", out shpe) && shpe == 0)){
                thisPackage.Type = "Hair Recolor";
            } else if ((typeDict.TryGetValue("GMDC", out gmdc) && gmdc >= 1) && (typeDict.TryGetValue("GMND", out gmnd) && gmnd >= 1) && (typeDict.TryGetValue("SHPE", out shpe) && shpe >= 1)) {
                thisPackage.Type = "Misc Mesh";
            } else {
                thisPackage.Type = "Currently Unknown";
            }
            
            LogMessage = string.Format("P{0} is a {1}", packageparsecount, thisPackage.Type);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));  
        
           
            #region Get Title & Description 
            
            if (!String.IsNullOrWhiteSpace(ctssvar.Title)) {
                thisPackage.Title = ctssvar.Title;
            } else if (!String.IsNullOrWhiteSpace(dirvar.Title)){
                thisPackage.Title = dirvar.Title;
            } else if (!String.IsNullOrWhiteSpace(objdvar.Title)) {
                thisPackage.Title = objdvar.Title;
            } else if (!String.IsNullOrWhiteSpace(strvar.Title)) {
                thisPackage.Title = strvar.Title;
            } else if (!String.IsNullOrWhiteSpace(mmatvar.Title)) {
                thisPackage.Title = mmatvar.Title;
            }


            if (!String.IsNullOrWhiteSpace(ctssvar.Description)){
                thisPackage.Description = ctssvar.Description;
            } else if (!String.IsNullOrWhiteSpace(dirvar.Description)) {
                thisPackage.Description = dirvar.Description;
            } else if (!String.IsNullOrWhiteSpace(objdvar.Description)) {
                thisPackage.Description = objdvar.Description;
            } else if (!String.IsNullOrWhiteSpace(strvar.Description)) {
                thisPackage.Description = strvar.Description;
            } else if (!String.IsNullOrWhiteSpace(mmatvar.Description)) {
                thisPackage.Title = mmatvar.Title;
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
                if (dirvar.GUIDs?.Any() != true){
                    allGUIDS.AddRange(objdvar.GUIDs);
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

                if (!String.IsNullOrWhiteSpace(objdvar.Function)){
                    thisPackage.Function = objdvar.Function;
                }
                if (!String.IsNullOrWhiteSpace(objdvar.Function)){
                    thisPackage.FunctionSubcategory = objdvar.FunctionSubcategory;
                }
                thisPackage.RequiredEPs = objdvar.RequiredEPs;
                thisPackage.RoomSort = objdvar.RoomSort;
                allGUIDS.AddRange(objdvar.GUIDs);

            if (thisPackage.Type == "floor") {
                thisPackage.Type = "Floor";
            } else if (thisPackage.Type == "wallpaper") {
                thisPackage.Type = "Wallpaper";
            } else if (thisPackage.Type == "terrainPaint") {
                thisPackage.Type = "Terrain Paint";
            }
            if (!String.IsNullOrWhiteSpace(thisPackage.Type)){
                thisPackage.Function = thisPackage.Type;
                thisPackage.Type = thisPackage.Type;
            }
            if (!String.IsNullOrWhiteSpace(thisPackage.Subtype)){
                thisPackage.FunctionSubcategory = thisPackage.Subtype;
            }

            #endregion


            distinctInstanceIDs = allInstanceIDs.Distinct().ToList();
            thisPackage.InstanceIDs.AddRange(distinctInstanceIDs);
            distinctGUIDS = allGUIDS.Distinct().ToList();
            thisPackage.GUIDs.AddRange(distinctGUIDS);
            dbpfFile.Close();
            dbpfFile.Dispose();
            readFile.Close();
            readFile.Dispose();            
            if (GlobalVariables.sortonthego == true){
                thisPackage = filesort.SortPackage(thisPackage);
            }
            
            log.MakeLog(string.Format("Package Summary: {0}", thisPackage.SimsPackagetoString()), false);
            LogMessage = string.Format("P{0} - Package Summary: {1}", packageparsecount, thisPackage.SimsPackagetoString());
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            log.MakeLog(string.Format("P{0} - Package Summary: {1}", packageparsecount, thisPackage.SimsPackagetoString()), false);
            //Containers.Containers.allSimsPackages.Add(thisPackage);
            LogMessage = string.Format("Adding {0} to packages database.", thisPackage.PackageName);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            /*try {
                GlobalVariables.DatabaseConnection.InsertWithChildren(thisPackage, true);
            } catch (Exception e) {
                LogMessage = string.Format("Caught exception adding Sims 2 package to database. \n Exception: {0}", e);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            }
            
            LogMessage = string.Format("Added {0} to packages database successfully.", thisPackage.PackageName);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            txt = string.Format("SELECT * FROM Processing_Reader where Name='{0}'", Methods.FixApostrophesforSQL(packageinfo.Name));
            List<PackageFile> closingquery = GlobalVariables.DatabaseConnection.Query<PackageFile>(txt);
            GlobalVariables.DatabaseConnection.Delete(closingquery[0]);

            closingquery = new List<PackageFile>();*/
            
                
             
            LogMessage = string.Format("Closing package # {0}/{1}: {2}", packageparsecount, GlobalVariables.PackageCount, packageinfo.Name);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
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
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            log.MakeLog(string.Format("Reading file {0} took {1}", packageinfo.Name, elapsedtime), false);

            objdnum = new List<int>();   
            strnm = new List<int>();
            thisPackage = new SimsPackage();
            infovar = new SimsPackage();
            ctssvar = new SimsPackage();
            dirvar = new SimsPackage();
            strvar = new SimsPackage();
            objdvar = new SimsPackage();
            mmatvar = new SimsPackage();
            fileHas = new List<fileHasList>();
            linkData = new ArrayList();
            indexData = new List<indexEntry>();
            //packageinfo = new FileInfo(file); 
            allGUIDS = new List<string>();      
            distinctGUIDS = new List<string>();  
            allInstanceIDs = new List<string>();      
            distinctInstanceIDs = new List<string>();
            return;            
        }
    }
}