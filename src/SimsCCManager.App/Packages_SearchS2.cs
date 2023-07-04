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
using System.IO.Packaging;
using Microsoft.VisualBasic.Logging;

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
            
            List<PackageEntries> fileHas = new List<PackageEntries>();
            ArrayList linkData = new ArrayList();
            List<indexEntry> indexData = new List<indexEntry>();
            //List<string> iids = new List<string>();
            List<PackageGUID> allGUIDS = new();      
            List<PackageGUID> distinctGUIDS = new();  
            List<PackageInstance> allInstanceIDs = new();      
            List<PackageInstance> distinctInstanceIDs = new();  
            if (minor == 2) indexmajorloc = 24;
            if (minor == 1) indexmajorloc = 32;
            //create readers              
            //MemoryStream dbpfFile = Methods.ReadBytesToFile(packageinfo.FullName, (int)packageinfo.Length);
            dbpfFile.Position = 0;
            BinaryReader readFile = new BinaryReader(dbpfFile);
    
            thisPackage.FileSize = (int)packageinfo.Length;   
            //string packageNameUpdated = Methods.FixApostrophesforSQL(packageinfo.Name);            
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

            /*for (int i = 0; i < 1000; i++) {
                log.MakeLog(readFile.ReadUInt32().ToString("X8"), true);
            }*/

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
            readFile.BaseStream.Position = readFile.BaseStream.Position + 32;
            long headerend = readFile.BaseStream.Position;
            readFile.BaseStream.Position = chunkOffset + indexOffset;
            for (int i = 0; i < indexCount; i++) {
                indexEntry holderEntry = new indexEntry();
                holderEntry.typeID = readFile.ReadUInt32().ToString("X8");               
                
                List<typeList> type = GlobalVariables.S4FunctionTypesConnection.Query<typeList>(string.Format("SELECT * FROM S2Types where TypeID='{0}'", holderEntry.typeID));
                    
                if(type.Any()){
                    fileHas.Add(new PackageEntries {TypeID = type[0].typeID, Name = type[0].desc, Location = i});
                    LogMessage = string.Format("P{0}/E{1} - {2} is at location {3}", packageparsecount, i, type[0].desc, i);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                } else {
                    fileHas.Add(new PackageEntries() { TypeID = holderEntry.typeID, Location = i});
                    LogMessage = string.Format("P{0}/E{1} - {2} is unidentified and at location {3}", packageparsecount, i, holderEntry.typeID, i);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                }
                type = new List<typeList>();

                holderEntry.groupID = readFile.ReadUInt32().ToString("X8");
                holderEntry.instanceID = readFile.ReadUInt32().ToString("X8");                
                allInstanceIDs.Add(new PackageInstance(){InstanceID = holderEntry.instanceID.ToString()});

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

            long indexend = readFile.BaseStream.Position;

            string fhh = "";
            foreach (PackageEntries item in fileHas) {
                if (string.IsNullOrWhiteSpace(fhh)){
                    fhh = string.Format("{0} at {1}", item.Name, item.Location);                    
                } else {
                    fhh += string.Format("\n {0} at {1}", item.Name, item.Location);  
                }
            }

            LogMessage = string.Format("P{0} - This file has: \n{1}", packageparsecount, fhh);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

            if (fileHas.Exists(x => x.Name == "DIR")) {                
                List<int> entryspots = (from has in fileHas
                            where has.Name =="DIR"
                            select has.Location).ToList();
                
                foreach (int loc in entryspots){                    
                    LogMessage = string.Format("P{0} - DIR is at entry [{1}]", packageparsecount, loc);
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
                    
                    LogMessage = string.Format("P{0} - DIR confirmation: {1}", packageparsecount, indexData[loc].typeID);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

                    //dbpfFile.Seek(this.chunkOffset + indexData[loc].offset, SeekOrigin.Begin);
                    readFile.BaseStream.Position = this.chunkOffset + indexData[loc].offset;
                    if (indexMajorVersion == 7 && indexMinorVersion == 1)
                    {
                        numRecords = indexData[loc].filesize / 20;
                    }
                    else 
                    {
                        numRecords = indexData[loc].filesize / 16;
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
                        allInstanceIDs.Add(new PackageInstance(){InstanceID = holderEntry.instanceID.ToString()});
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

                    readFile.BaseStream.Position = chunkOffset + indexData[loc].offset - 2;
                    if ((indexMajorVersion == 7) && indexMinorVersion == 1)
                    {
                        numRecords = indexData[loc].filesize / 20;
                    } else {
                        numRecords = indexData[loc].filesize / 16;
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
                    allInstanceIDs.Add(new PackageInstance(){InstanceID = holderEntry.instanceID.ToString()});
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

                        List<typeList> type = new();
                        //List<typeList> type = GlobalVariables.S4FunctionTypesConnection.Query<typeList>(string.Format("SELECT * FROM S2Types where TypeID='{0}'", holderEntry.typeID));
                    
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
                            LogMessage = string.Format("P{0}/DIR CR#{1} - {2} has a size of {3} and a compression type of {4}", packageparsecount, j, idxcount, cFileSize, cTypeID);
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
            }        

            if (fileHas.Exists(x => x.Name == "CTSS")) {                
                List<int> entryspots = (from has in fileHas
                            where has.Name =="CTSS"
                            select has.Location).ToList();
                
                int cts = 0;
                
                foreach (int loc in entryspots){     
                    cts++;               
                    readFile.BaseStream.Position = chunkOffset + indexData[loc].offset;
                    LogMessage = string.Format("P{0}/CTSS{1} - Reader is at {2}", packageparsecount, cts, readFile.BaseStream.Position);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    cFileSize = readFile.ReadInt32();
                    cTypeID = readFile.ReadUInt16().ToString("X4");
                    LogMessage = string.Format("P{0}/CTSS{1} - Details: \n-- TypeID: {2}\n-- GroupID: {3} \n-- Location: {4} \n-- Offset: {5} \n-- FileSize: {6}\n-- cTypeID: {7}", packageparsecount, cts, indexData[loc].typeID, indexData[loc].groupID, loc, indexData[loc].offset, cFileSize, cTypeID);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    
                    if (cTypeID == "FB10") 
                    {
                        LogMessage = string.Format("P{0}/CTSS{1} - cTypeID is FB10", packageparsecount, cts);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        byte[] tempBytes = readFile.ReadBytes(3);
                        uint cFullSize = readentries.QFSLengthToInt(tempBytes);

                        DecryptByteStream decompressed = new DecryptByteStream(readentries.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));

                        ctssvar = readentries.readCTSSchunk(decompressed);
                        LogMessage = string.Format("P{0}/CTSS{1} - Finished reading CTSS{1}; returned with data: {2}", packageparsecount, cts, ctssvar.SimsPackagetoString());
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    } 
                    else 
                    {
                        LogMessage = string.Format("P{0}/CTSS{1} - cTypeID is 0000", packageparsecount, cts);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        readFile.BaseStream.Position = chunkOffset + indexData[loc].offset;
                        ctssvar = readentries.readCTSSchunk(readFile);
                        LogMessage = string.Format("P{0}/CTSS{1} - Finished reading CTSS{1}; returned with data: {2}", packageparsecount, cts, ctssvar.SimsPackagetoString());
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    }
                }
            } 


            if (fileHas.Exists(x => x.Name == "XOBJ" || x.Name == "XFNC" || x.Name == "XFLR" || x.Name == "XMOL" || x.Name == "XROF" || x.Name == "XTOL" || x.Name == "XHTN")) {
                List<int> entryspots = (from x in fileHas
                            where x.Name == "XOBJ" || x.Name == "XFNC" || x.Name == "XFLR" || x.Name == "XMOL" || x.Name == "XROF" || x.Name == "XTOL" || x.Name == "XHTN"
                            select x.Location).ToList();

                int xo = 0;
                
                foreach (int loc in entryspots) {
                    xo++;                    
                    readFile.BaseStream.Position = 0;
                    readFile.BaseStream.Position = chunkOffset + indexData[loc].offset;
                    LogMessage = string.Format("P{0}/CTSS{1} - Reader is at {2}", packageparsecount, xo, readFile.BaseStream.Position);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    cFileSize = readFile.ReadInt32();
                    cTypeID = readFile.ReadUInt16().ToString("X4");
                    LogMessage = string.Format("P{0}/XOBJ{1} - Details: \n-- TypeID: {2}\n-- GroupID: {3} \n-- Location: {4} \n-- Offset: {5} \n-- FileSize: {6}\n-- cTypeID: {7}", packageparsecount, xo, indexData[loc].typeID, indexData[loc].groupID, loc, indexData[loc].offset, cFileSize, cTypeID);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    if (cTypeID == "FB10"){
                        log.MakeLog("FB10 confirmed.", true);
                        byte[] tempBytes = readFile.ReadBytes(3);
                        uint cFullSize = readentries.QFSLengthToInt(tempBytes);
                        string cpfTypeID = readFile.ReadUInt32().ToString("X8");                        
                        if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0")){
                            dirvar = readentries.readCPFchunk(readFile);
                            log.MakeLog("Real CPF file. Processing as CPF chunk.",true);
                        } else {
                            log.MakeLog("Not a real CPF. Searching for more information.", true);
                            //dbpfFile.Seek(this.chunkOffset + idx.offset + 9, SeekOrigin.Begin);
                            readFile.BaseStream.Position = this.chunkOffset + indexData[loc].offset + 9;
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
               

            if (fileHas.Exists(x => x.Name == "OBJD")) {
                
                List<int> entryspots = (from has in fileHas
                            where has.Name =="OBJD"
                            select has.Location).ToList();

                int obb = 0;
                
                foreach (int loc in entryspots) {
                    obb++;               
                    readFile.BaseStream.Position = 0;
                    readFile.BaseStream.Position = chunkOffset + indexData[loc].offset ;
                    LogMessage = string.Format("P{0}/OBJD{1} - Reader is at {2}", packageparsecount, obb, readFile.BaseStream.Position);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    cFileSize = readFile.ReadInt32();
                    cTypeID = readFile.ReadUInt16().ToString("X4");
                    LogMessage = string.Format("P{0}/OBJD{1} - Details: \n-- TypeID: {2}\n-- GroupID: {3} \n-- Location: {4} \n-- Offset: {5} \n-- FileSize: {6}\n-- cTypeID: {7}", packageparsecount, obb, indexData[loc].typeID, indexData[loc].groupID, loc, indexData[loc].offset, cFileSize, cTypeID);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
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
                        
                        LogMessage = string.Format("P{0}/OBJD{1} - Reading non-compressed entry.", packageparsecount, obb);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

                        readFile.BaseStream.Position = chunkOffset + indexData[loc].offset ;
                        objdvar = readentries.readOBJDchunk(readFile);
                    }
                }
                
            }

            if (fileHas.Exists(x => x.Name == "STR#"))
            {
                List<int> entryspots = (from has in fileHas
                            where has.Name =="STR#"
                            select has.Location).ToList();

                int st = 0;
                
                foreach (int loc in entryspots) {
                    st++;
                    readFile.BaseStream.Position = chunkOffset + indexData[loc].offset ;
                    LogMessage = string.Format("P{0}/CTSS{1} - Reader is at {2}", packageparsecount, st, readFile.BaseStream.Position);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    cFileSize = readFile.ReadInt32();
                    cTypeID = readFile.ReadUInt16().ToString("X4");
                    LogMessage = string.Format("P{0}/STR{1} - Details: \n-- TypeID: {2}\n-- GroupID: {3} \n-- Location: {4} \n-- Offset: {5} \n-- FileSize: {6}\n-- cTypeID: {7}", packageparsecount, st, indexData[loc].typeID, indexData[loc].groupID, loc, indexData[loc].offset, cFileSize, cTypeID);
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


            if (fileHas.Exists(x => x.Name == "MMAT"))
            {
                List<int> entryspots = (from has in fileHas
                        where has.Name =="MMAT"
                        select has.Location).ToList();
                int mm = 0;
                foreach (int loc in entryspots){
                    mm++;
                    readFile.BaseStream.Position = chunkOffset + indexData[loc].offset;
                    LogMessage = string.Format("P{0}/CTSS{1} - Reader is at {2}", packageparsecount, mm, readFile.BaseStream.Position);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    cFileSize = readFile.ReadInt32();
                    cTypeID = readFile.ReadUInt16().ToString("X4");
                    LogMessage = string.Format("P{0}/MMAT{1} - Details: \n-- TypeID: {2}\n-- GroupID: {3} \n-- Location: {4} \n-- Offset: {5} \n-- FileSize: {6}\n-- cTypeID: {7}", packageparsecount, mm, indexData[loc].typeID, indexData[loc].groupID, loc, indexData[loc].offset, cFileSize, cTypeID);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
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
                            readFile.BaseStream.Position = this.chunkOffset + indexData[loc].offset + 9;
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
                        readFile.BaseStream.Position = this.chunkOffset + indexData[loc].offset;

                        string cpfTypeID = readFile.ReadUInt32().ToString("X8");
                        if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0"))
                        {
                            mmatvar = readentries.readCPFchunk(readFile);
                        }

                        if  (cpfTypeID == "6D783F3C")
                        {
                            //dbpfFile.Seek(this.chunkOffset + indexData[mmatloc].offset, SeekOrigin.Begin);
                            readFile.BaseStream.Position = this.chunkOffset + indexData[loc].offset;

                            string xmlData = Encoding.UTF8.GetString(readFile.ReadBytes((int)indexData[loc].filesize));
                            mmatvar = readentries.readXMLchunk(xmlData);

                        }
                    }
                }
                
                
            }

            
            if (fileHas.Exists(x => x.Name == "SHPE"))
            {   
                List<int> entryspots = (from has in fileHas
                        where has.Name =="SHPE"
                        select has.Location).ToList();
                int sh = 0;
                foreach (int loc in entryspots){
                    sh++;
                    readFile.BaseStream.Position = chunkOffset + indexData[loc].offset;
                    LogMessage = string.Format("P{0}/CTSS{1} - Reader is at {2}", packageparsecount, sh, readFile.BaseStream.Position);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    cFileSize = readFile.ReadInt32();
                    cTypeID = readFile.ReadUInt16().ToString("X4");
                    LogMessage = string.Format("P{0}/SHPE{1} - Details: \n-- TypeID: {2}\n-- GroupID: {3} \n-- Location: {4} \n-- Offset: {5} \n-- FileSize: {6}\n-- cTypeID: {7}", packageparsecount, sh, indexData[loc].typeID, indexData[loc].groupID, loc, indexData[loc].offset, cFileSize, cTypeID);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
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
                            readFile.BaseStream.Position = this.chunkOffset + indexData[loc].offset + 9;
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
                        readFile.BaseStream.Position = this.chunkOffset + indexData[loc].offset;

                        string cpfTypeID = readFile.ReadUInt32().ToString("X8");
                        if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0"))
                        {
                            mmatvar = readentries.readCPFchunk(readFile);
                        }

                        if  (cpfTypeID == "6D783F3C")
                        {
                            //dbpfFile.Seek(this.chunkOffset + indexData[entryloc].offset, SeekOrigin.Begin);
                            readFile.BaseStream.Position = this.chunkOffset + indexData[loc].offset;

                            string xmlData = Encoding.UTF8.GetString(readFile.ReadBytes((int)indexData[loc].filesize));
                            mmatvar = readentries.readXMLchunk(xmlData);

                        }
                    }
                }
                
                
            }

            /*if (fileHas.Exists(x => x.Name == "IMG"))
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
                if (dirvar.GUIDs?.Any() != true){
                    foreach (PackageGUID guid in dirvar.GUIDs){
                      allGUIDS.Add(new PackageGUID(){GuidID = guid.GuidID});  
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

            if (!String.IsNullOrWhiteSpace(objdvar.Function)){
                thisPackage.Function = objdvar.Function;
            }
            if (!String.IsNullOrWhiteSpace(objdvar.Function)){
                thisPackage.FunctionSubcategory = objdvar.FunctionSubcategory;
            }
            thisPackage.RequiredEPs = objdvar.RequiredEPs;
            thisPackage.RoomSort = objdvar.RoomSort;
            allGUIDS.AddRange(objdvar.GUIDs);
            
            if (!String.IsNullOrWhiteSpace(thisPackage.Type)){
                thisPackage.Function = thisPackage.Type;
                thisPackage.Type = thisPackage.Type;
            }
            if (!String.IsNullOrWhiteSpace(thisPackage.Subtype)){
                thisPackage.FunctionSubcategory = thisPackage.Subtype;
            }
            if (thisPackage.Type == "floor") {
                thisPackage.Type = "Floor";
            } else if (thisPackage.Type == "wallpaper") {
                thisPackage.Type = "Wallpaper";
            } else if (thisPackage.Type == "terrainPaint") {
                thisPackage.Type = "Terrain Paint";
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
            
            log.MakeLog(string.Format("Package Summary: {0}", thisPackage.SimsPackagetoString()), false);
            LogMessage = string.Format("P{0} - Package Summary: {1}", packageparsecount, thisPackage.SimsPackagetoString());
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            log.MakeLog(string.Format("P{0} - Package Summary: {1}", packageparsecount, thisPackage.SimsPackagetoString()), false);

            LogMessage = string.Format("Adding {0} to packages database.", thisPackage.PackageName);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                                      
             
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
            distinctGUIDS = new();  
            allInstanceIDs = new();      
            distinctInstanceIDs = new();
            return;            
        }

        public SimsPackage MakeNoNulls(SimsPackage thisPackage){
            thisPackage.Title ??= "";
            thisPackage.Description ??= "";
            thisPackage.Subtype ??= "";
            thisPackage.Category ??= "";
            thisPackage.ModelName ??= "";
            thisPackage.PackageName ??= "";
            thisPackage.Type ??= "";
            thisPackage.GameString ??= "";
            if (!thisPackage.GUIDs.Any()){
                thisPackage.GUIDs.Add(new PackageGUID());
            }            
            thisPackage.Tuning ??= "";
            thisPackage.Creator ??= "";
            thisPackage.Age ??= "";
            thisPackage.Gender ??= "";
            thisPackage.MatchingMesh ??= "";
            if (!thisPackage.RequiredEPs.Any()){
                thisPackage.RequiredEPs.Add(new PackageRequiredEPs());
            }
            thisPackage.Function ??= "";
            thisPackage.FunctionSubcategory ??= "";
            if (!thisPackage.AgeGenderFlags.Any()){
                thisPackage.AgeGenderFlags = new();
            }
            if (!thisPackage.FileHas.Any()){
                thisPackage.FileHas.Add(new PackageEntries());
            }
            if (!thisPackage.RoomSort.Any()){
                thisPackage.RoomSort.Add(new PackageRoomSort());
            }
            if (!thisPackage.Components.Any()){
                thisPackage.Components.Add(new PackageComponent());
            }
            if (!thisPackage.Entries.Any()){
                thisPackage.Entries.Add(new PackageTypeCounter());
            }
            if (!thisPackage.Flags.Any()){
                thisPackage.Flags.Add(new PackageFlag());
            }
            if (!thisPackage.CatalogTags.Any()){
                thisPackage.CatalogTags.Add(new TagsList());
            }
            if (!thisPackage.Components.Any()){
                thisPackage.Components.Add(new PackageComponent ());
            }
            if (!thisPackage.OverridesList.Any()){
                thisPackage.OverridesList.Add(new OverriddenList ());
            }
            if (!thisPackage.MeshKeys.Any()){
                thisPackage.MeshKeys.Add(new PackageMeshKeys ());
            }
            if (!thisPackage.CASPartKeys.Any()){
                thisPackage.CASPartKeys.Add(new PackageCASPartKeys ());
            }
            if (!thisPackage.OBJDPartKeys.Any()){
                thisPackage.OBJDPartKeys.Add(new PackageOBJDKeys());
            }
            if (!thisPackage.MatchingRecolors.Any()){
                thisPackage.MatchingRecolors.Add(new PackageMatchingRecolors ());
            }
            if (!string.IsNullOrEmpty(thisPackage.MatchingMesh)){
                thisPackage.MatchingMesh = "";
            }
            if (!thisPackage.Conflicts.Any()){
                thisPackage.Conflicts.Add(new PackageConflicts());
            }
            

            return thisPackage;
        }
    }
}