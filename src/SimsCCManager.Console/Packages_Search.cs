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
    class PackageSearch
    {
        
        // References
        LoggingGlobals log = new LoggingGlobals();
        ReadEntries readentries = new ReadEntries();   

        //Vars
        uint chunkOffset = 0;        

        
        

        public void SearchS2Packages(string file) {
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
        
            //Misc Vars
            string test = "";        
            int dirnum = 0;
            List<int> objdnum = new List<int>();   
            List<int> strnm = new List<int>();
            int mmatloc = 0;

            SimsPackage thisPackage = new SimsPackage();
            SimsPackage infovar = new SimsPackage();
            SimsPackage ctssvar = new SimsPackage();
            SimsPackage dirvar = new SimsPackage();
            SimsPackage strvar = new SimsPackage();
            SimsPackage objdvar = new SimsPackage();
            SimsPackage mmatvar = new SimsPackage();
            

            //Lists 
            
            List<fileHasList> fileHas = new List<fileHasList>();
            ArrayList linkData = new ArrayList();
            List<indexEntry> indexData = new List<indexEntry>();
            FileInfo packageinfo = new FileInfo(file); 
            //List<string> iids = new List<string>();
            List<string> allGUIDS = new List<string>();      
            List<string> distinctGUIDS = new List<string>();  

            //create readers  
            FileStream dbpfFile = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader readFile = new BinaryReader(dbpfFile);

            thisPackage.PackageName = packageinfo.Name;
            thisPackage.Game = 2;
            

            //start actually reading the package 
            Console.WriteLine("Reading Package #" + packageparsecount + "/" + GlobalVariables.PackageCount + ": " + packageinfo.Name);
            log.MakeLog("Logged Package #" + packageparsecount + " as " + packageinfo.FullName, true);
            thisPackage.Location = packageinfo.FullName;            
            thisPackage.Game = 2;
            log.MakeLog("Logged Package #" + packageparsecount + " as meant for The Sims " + thisPackage.Game, true);           
            test = Encoding.ASCII.GetString(readFile.ReadBytes(4));
            log.MakeLog("P" + packageparsecount + " - DBPF Bytes: " + test, true);
            thisPackage.DBPF = test;
            
            uint major = readFile.ReadUInt32();
            test = major.ToString();  
            thisPackage.Major = major;
            log.MakeLog("P" + packageparsecount + " - Major: " + test, true);

            uint minor = readFile.ReadUInt32();
            test = minor.ToString();
            thisPackage.Minor = minor;
            log.MakeLog("P" + packageparsecount + " - Minor: " + test, true);
            
            string reserved = Encoding.UTF8.GetString(readFile.ReadBytes(12));
            test = reserved;
            log.MakeLog("P" + packageparsecount + " - Reserved: " + test, true);
            
            uint dateCreated = readFile.ReadUInt32();
            test = dateCreated.ToString();
            thisPackage.DateCreated = dateCreated;
            log.MakeLog("P" + packageparsecount + " - Date created: " + test, true);
            
            uint dateModified = readFile.ReadUInt32();
            test = dateModified.ToString();
            thisPackage.DateModified = dateModified;
            log.MakeLog("P" + packageparsecount + " - Date modified: " + test, true);
            
            uint indexMajorVersion = readFile.ReadUInt32();
            test = indexMajorVersion.ToString();
            thisPackage.IndexMajorVersion = indexMajorVersion;
            log.MakeLog("P" + packageparsecount + " - Index Major: " + test, true);
            
            uint indexCount = readFile.ReadUInt32();
            test = indexCount.ToString();
            thisPackage.IndexCount = indexCount;
            log.MakeLog("P" + packageparsecount + " - Index Count: " + test, true);
            
            uint indexOffset = readFile.ReadUInt32();
            test = indexOffset.ToString();
            thisPackage.IndexOffset = indexOffset;
            log.MakeLog("P" + packageparsecount + " - Index Offset: " + test, true);
            
            uint indexSize = readFile.ReadUInt32();
            test = indexSize.ToString();
            thisPackage.IndexSize = indexSize;
            log.MakeLog("P" + packageparsecount + " - Index Size: " + test, true);
            
            uint holesCount = readFile.ReadUInt32();
            test = holesCount.ToString();
            thisPackage.HolesCount = holesCount;
            log.MakeLog("P" + packageparsecount + " - Holes Count: " + test, true);

            uint holesOffset = readFile.ReadUInt32();
            test = holesOffset.ToString();
            thisPackage.HolesOffset = holesOffset;
            log.MakeLog("P" + packageparsecount + " - Holes Offset: " + test, true);
            
            uint holesSize = readFile.ReadUInt32();
            test = holesSize.ToString();
            thisPackage.HolesSize = holesSize;
            log.MakeLog("P" + packageparsecount + " - Holes Size: " + test, true);
            
            uint indexMinorVersion = readFile.ReadUInt32() -1;
            test = indexMinorVersion.ToString();
            thisPackage.IndexMinorVersion = indexMinorVersion;
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
                //Console.WriteLine(holderEntry.instanceID);
                thisPackage.InstanceIDs.Add(holderEntry.instanceID.ToString());
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


            


            //Console.WriteLine(indexData[0].typeID);

            var entrynum = 0;
            foreach (indexEntry iEntry in indexData) {
                log.MakeLog("P" + packageparsecount + " - Entry [" + entrynum + "]", true);
                
                

                switch (iEntry.typeID.ToLower()) 
                {                    
                    case "fc6eb1f7": linkData.Add(iEntry); log.MakeLog("P" + packageparsecount + " - File has SHPE.", true); break;
                }
                
                foreach (typeList type in TypeListings.AllTypesS2) {
                    //log.MakeLog("P" + packageparsecount + " - Checking entry " + entrynum + " (type ID: " + iEntry.typeID + ") for: " + type.desc, true);
                    if (iEntry.typeID == type.typeID) {
                        log.MakeLog("P" + packageparsecount + " - Found: " + type.desc, true);
                        typefound = type.desc;
                        try {
                            fileHas.Add(new fileHasList() { term = type.desc, location = entrynum});
                        } catch {
                            //nada
                        }
                        break;
                    }
                    
                }
                entrynum++;
            }

            log.MakeLog("P" + packageparsecount + " - This file has:", true);
            foreach (fileHasList item in fileHas) {
                log.MakeLog("--- " + item.term + " at: " + item.location, true);
            }

            if (fileHas.Exists(x => x.term == "DIR")) {       
                int fh = 0;
                foreach (fileHasList item in fileHas) {
                    if (item.term == "DIR"){
                        dirnum = fh;
                    }
                    fh++;
                }
                log.MakeLog("P" + packageparsecount + " - DIR is at entry [" + dirnum + "]", true);
                entrynum = 0;
            
                //go through dir entry specifically 
                
                numRecords = 0;
                typeID = "";
                groupID = "";
                instanceID = "";
                instanceID2 = "";
                myFilesize = 0;
                
                log.MakeLog("P" + packageparsecount + " - DIR entry confirmation: ", true);
                log.MakeLog(indexData[dirnum].typeID, true);

                dbpfFile.Seek(this.chunkOffset + indexData[dirnum].offset, SeekOrigin.Begin);
                if (indexMajorVersion == 7 && indexMinorVersion == 1)
                {
                    numRecords = indexData[dirnum].filesize / 20;
                }
                else 
                {
                    numRecords = indexData[dirnum].filesize / 16;
                }  

                log.MakeLog("P" + packageparsecount + " - Number of compressed records in entry:" + numRecords, true);
                
                for (int c = 0; c < numRecords; c++)
                {
                    indexEntry holderEntry = new indexEntry();
                    log.MakeLog("P" + packageparsecount + " - Reading compressed record #" + c, true);
                    typeID = readFile.ReadUInt32().ToString("X8");
                    log.MakeLog("P" + packageparsecount + " - CR#" + c + ": Type ID is " + typeID, true);
                    groupID = readFile.ReadUInt32().ToString("X8");
                    log.MakeLog("P" + packageparsecount + " - CR#" + c + ": Group ID is " + groupID, true);
                    instanceID = readFile.ReadUInt32().ToString("X8");
                    log.MakeLog("P" + packageparsecount + " - CR#" + c + ": Instance ID is " + instanceID, true);
                    holderEntry.instanceID = readFile.ReadUInt32().ToString("X8");
                    //Console.WriteLine(holderEntry.instanceID);
                    thisPackage.InstanceIDs.Add(holderEntry.instanceID.ToString());
                    log.MakeLog("P" + packageparsecount + " - InstanceID: " + holderEntry.instanceID, true);
                    if (indexMajorVersion == 7 && indexMinorVersion == 1) instanceID2 = readFile.ReadUInt32().ToString("X8");
                    log.MakeLog("P" + packageparsecount + " - CR#" + c + ": InstanceID2 is " + instanceID2, true);
                    myFilesize = readFile.ReadUInt32();
                    log.MakeLog("P" + packageparsecount + " - CR#" + c + ": Filesize is " + myFilesize, true);

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
                                    log.MakeLog("P" + packageparsecount + " - CR#" + c + ": Index entry filesize is " + myFilesize, true);
                                    break;
                                }
                            } 
                            else
                            {
                                idx.compressed = true;
                                idx.truesize = myFilesize;
                                log.MakeLog("P" + packageparsecount + " - CR#" + c + ": Index entry filesize is " + myFilesize, true);
                                break;
                            }
                        }
                    }
                }
            }
            
            if (fileHas.Exists(x => x.term == "DIR")) {
                dbpfFile.Seek(chunkOffset + indexData[dirnum].offset, SeekOrigin.Begin);
                log.MakeLog("Entry offset: " + indexData[dirnum].offset, true);
                if ((indexMajorVersion == 7) && indexMinorVersion == 1)
                {
                    numRecords = indexData[dirnum].filesize / 20;
                } else {
                    numRecords = indexData[dirnum].filesize / 16;
                }

                log.MakeLog("Reading compressed entries from " + typefound, true);
                log.MakeLog("Number of records: " + numRecords, true);
                
                for (int j = 0; j < numRecords; j++) {
                    indexEntry holderEntry = new indexEntry();
                    log.MakeLog("", true);
                    log.MakeLog("Compressed Entry #" + j, true);
                    typeID = readFile.ReadUInt32().ToString("X8");
                    log.MakeLog("TypeID: "+ typeID, true);
                    groupID = readFile.ReadUInt32().ToString("X8");
                    log.MakeLog("GroupID: "+ groupID, true);
                    instanceID = readFile.ReadUInt32().ToString("X8");
                    log.MakeLog("InstanceID: "+ instanceID, true);
                    holderEntry.instanceID = readFile.ReadUInt32().ToString("X8");
                    //Console.WriteLine(holderEntry.instanceID);
                    thisPackage.InstanceIDs.Add(holderEntry.instanceID.ToString());
                    log.MakeLog("P" + packageparsecount + " - InstanceID: " + holderEntry.instanceID, true);
                    if (indexMajorVersion == 7 && indexMinorVersion == 1) {
                        instanceID2 = readFile.ReadUInt32().ToString("X8");
                        log.MakeLog("InstanceID2: "+ instanceID2, true);
                    }
                    compfilesize = readFile.ReadUInt32();
                    log.MakeLog("Filesize: "+ compfilesize, true);

                    int idxcount = 0;      
                    foreach (indexEntry idx in indexData) {
                        typefound = "";
                        idxcount++;
                        log.MakeLog("This idx type is: " + idx.typeID, true);
                        foreach (typeList type in TypeListings.AllTypesS2) {
                            if (idx.typeID == type.typeID) {
                                log.MakeLog("Matched to: " + type.desc, true);
                                typefound = type.desc;
                            }
                        }
                        log.MakeLog("Now reading IDX " + idxcount, true);
                        cFileSize = 0;
                        cTypeID = "";

                        if (typefound == "CTSS"){
                            log.MakeLog("Confirming found " + typefound, true);
                            dbpfFile.Seek(this.chunkOffset + idx.offset, SeekOrigin.Begin);
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
								dbpfFile.Seek(this.chunkOffset + idx.offset, SeekOrigin.Begin);
								ctssvar = readentries.readCTSSchunk(readFile);
							}
                        } else if (typefound == "XOBJ" || typefound == "XFNC" || typefound == "XFLR" || typefound == "XMOL" || typefound == "XROF"  || typefound == "XTOL"  || typefound == "XHTN"){
                            log.MakeLog("Confirming found " + typefound + " and moving forward.", true);
                            dbpfFile.Seek(this.chunkOffset + idx.offset, SeekOrigin.Begin);
                            cFileSize = readFile.ReadInt32();
                            cTypeID = readFile.ReadUInt16().ToString("X4");
                            log.MakeLog(typefound + " size: " + cFileSize + ", ctypeid: " + cTypeID, true);
                            if (cTypeID == "FB10"){
                                log.MakeLog("FB10 confirmed.", true);
                                byte[] tempBytes = readFile.ReadBytes(3);
                                uint cFullSize = readentries.QFSLengthToInt(tempBytes);
                                log.MakeLog("cFullSize is: " + cFileSize, true);
                                string cpfTypeID = readFile.ReadUInt32().ToString("X8");
                                log.MakeLog("cpfTypeID is: " + cpfTypeID, true);
                                if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0")){
                                    dirvar = readentries.readCPFchunk(readFile);
                                    log.MakeLog("Real CPF file. Processing as CPF chunk.",true);
                                } else {
                                    log.MakeLog("Not a real CPF. Searching for more information.", true);
                                    dbpfFile.Seek(this.chunkOffset + idx.offset + 9, SeekOrigin.Begin);
                                    DecryptByteStream decompressed = new DecryptByteStream(readentries.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));
                                    if (cpfTypeID == "E750E0E2")
                                    {
                                        // Read first four bytes
                                        cpfTypeID = decompressed.ReadUInt32().ToString("X8");
                                        log.MakeLog("Secondary cpf type id: " + cpfTypeID, true);
                                        if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0")) 
                                        {
                                            log.MakeLog("Real CPF. Decompressing.", true);
                                            dirvar = readentries.readCPFchunk(decompressed);
                                            log.MakeLog("dirvar returned with: " + dirvar.ToString(), true);
                                        } 
                                    } else 
                                    {
                                        log.MakeLog("Actually an XML. Reading.", true);
                                        dirvar = readentries.readXMLchunk(decompressed);
                                        log.MakeLog("dirvar returned with: " + dirvar.ToString(), true);
                                    }
                                }

                            } else {
                                log.MakeLog("Not FB10.", true);

                            }
                        }

                    }

                }
            }
            

            if (fileHas.Exists(x => x.term == "OBJD")) {
                
                    
                int fh = 0;
                foreach (fileHasList item in fileHas) {
                    if (item.term == "OBJD"){
                        objdnum.Add(fh);
                    }
                    fh++;
                }
                log.MakeLog("P" + packageparsecount + " - OBJDs are at entries:",true);
                foreach (int objloc in objdnum) {
                    log.MakeLog("---------- [" + objloc + "]", true);               
                    
                    dbpfFile.Seek(this.chunkOffset + indexData[objloc].offset, SeekOrigin.Begin);
                    cFileSize = readFile.ReadInt32();
                    log.MakeLog("P" + packageparsecount + " - OBJD filesize is: " + cFileSize, true);
                    cTypeID = readFile.ReadUInt16().ToString("X4");
                    log.MakeLog("P" + packageparsecount + " - OBJD ctypeID is: " + cTypeID, true);
                    if (cTypeID == "FB10")
                    {
                        log.MakeLog("P" + packageparsecount + " - OBJD ctypeID confirmed as: " + cTypeID, true);
                        byte[] tempBytes = readFile.ReadBytes(3);
                        log.MakeLog("P" + packageparsecount + " - OBJD temp bytes are: " + tempBytes, true);
                        uint cFullSize = readentries.QFSLengthToInt(tempBytes);
                        log.MakeLog("P" + packageparsecount + " - OBJD size is: " + cFullSize, true);
                        DecryptByteStream decompressed = new DecryptByteStream(readentries.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));
                        objdvar = readentries.readOBJDchunk(decompressed);
                        log.MakeLog("objdvar returned with: " + objdvar.ToString(), true);
                    } else { 
                        dbpfFile.Seek(this.chunkOffset + indexData[objloc].offset, SeekOrigin.Begin);
                        objdvar = readentries.readOBJDchunk(readFile);
                        log.MakeLog("objdvar returned with: " + objdvar.ToString(), true);
                    }
                }
                
            }

            if (fileHas.Exists(x => x.term == "STR#"))
            {
                int fh = 0;
                foreach (fileHasList item in fileHas) {
                    if (item.term == "STR#"){
                        strnm.Add(fh);
                    }
                    fh++;
                }
                
                foreach (int strloc in strnm) {
                    log.MakeLog("P" + packageparsecount + " - STR entry confirmation: ", true);
                    log.MakeLog(indexData[strloc].typeID, true);
                    dbpfFile.Seek(chunkOffset + indexData[strloc].offset, SeekOrigin.Begin);
                    cFileSize = readFile.ReadInt32();
                    log.MakeLog("P" + packageparsecount + " - STR entry size: " + cFileSize, true);
                    cTypeID = readFile.ReadUInt16().ToString("X4");
                    log.MakeLog("P" + packageparsecount + " - STR entry typeid: " + cTypeID, true);
                    if (cTypeID == "FB10")
                    {
                        byte[] tempBytes = readFile.ReadBytes(3);
                        uint cFullSize = readentries.QFSLengthToInt(tempBytes);

                        DecryptByteStream decompressed = new DecryptByteStream(readentries.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));

                        strvar = readentries.readSTRchunk(decompressed);
                        log.MakeLog("strvar returned with: " + strvar.ToString(), true);
                    } 
                    else 
                    {
                        objdvar = readentries.readSTRchunk(readFile);
                        log.MakeLog("strvar returned with: " + strvar.ToString(), true);
                    }                    
                }
                
            }
            if (fileHas.Exists(x => x.term == "MMAT"))
            {
                int fh = 0;
                foreach (fileHasList item in fileHas) {
                    if (item.term == "MMAT"){
                        mmatloc = fh;
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
                        mmatvar = readentries.readCPFchunk(readFile);
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

                
            }

            log.MakeLog("All methods complete, moving on to getting info.", true);
            log.MakeLog("Dirvar contains: " + dirvar.ToString(), true);
            log.MakeLog("Ctssvar contains: " + ctssvar.ToString(), true);
            log.MakeLog("Mmatvar contains: " + mmatvar.ToString(), true);
            log.MakeLog("Objdvar contains: " + objdvar.ToString(), true);
            log.MakeLog("Strvar contains: " + strvar.ToString(), true);

            List<TypeCounter> typecount = new List<TypeCounter>();
            var typeDict = new Dictionary<string, int>();

            foreach (fileHasList item in fileHas){
                foreach (typeList type in TypeListings.AllTypesS2){
                    if (type.desc == item.term){
                        typeDict.Increment(type.desc);
                    }
                }
            }

            foreach (KeyValuePair<string, int> type in typeDict){
                TypeCounter tc = new TypeCounter();
                tc.Type = type.Key;
                tc.Count = type.Value;
                log.MakeLog("There are " + tc.Type + " of " + tc.Count + " in this package.", true);
                typecount.Add(tc);
            }
            
            thisPackage.Entries.AddRange(typecount);

            /*foreach (TypeCounter item in typecount) {
                thisPackage.Entries.Add(typecount);
            }*/
            
            
            

            if ((typeDict.TryGetValue("TXTR", out int txtr_0) && txtr_0 >= 1) && (typeDict.TryGetValue("STR#", out int str_0) && str_0 >= 1) && (typeDict.TryGetValue("DIR", out int dir_0) && dir_0 >= 1) && (typeDict.TryGetValue("TXMT", out int txmt_0) && txmt_0 >= 1) && (typeDict.TryGetValue("SHPE", out int shpe_0) && shpe_0 <= 0) && (typeDict.TryGetValue("BCON", out int bcon_0) && bcon_0 <= 0) && (typeDict.TryGetValue("BHAV", out int bhav_0) && bhav_0 <= 0) && (typeDict.TryGetValue("MMAT", out int mmat_0) && mmat_0 <= 0) && (typeDict.TryGetValue("OBJF", out int objf_0) && objf_0 <= 0) && (typeDict.TryGetValue("OBJD", out int objd_0) && objd_0 <= 0) && (typeDict.TryGetValue("CLST", out int clst_0) && clst_0 <= 0)){
                thisPackage.Type = "Floor";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);  
            } else if ((typeDict.TryGetValue("TXTR", out int txtr_1) && txtr_1 >= 1) && (typeDict.TryGetValue("STR#", out int str_1) && str_1 >= 1) && (typeDict.TryGetValue("DIR", out int dir_1) && dir_1 >= 1) && (typeDict.TryGetValue("SHPE", out int shpe_1) && shpe_1 <= 0) && (typeDict.TryGetValue("BCON", out int bcon_1) && bcon_1 <= 0) && (typeDict.TryGetValue("BHAV", out int bhav_1) && bhav_1 <= 0) && (typeDict.TryGetValue("MMAT", out int mmat_1) && mmat_1 <= 0) && (typeDict.TryGetValue("OBJF", out int objf_1) && objf_1 <= 0) && (typeDict.TryGetValue("OBJD", out int objd_1) && objd_1 <= 0) && (typeDict.TryGetValue("CLST", out int clst_1) && clst_1 <= 0) && (typeDict.TryGetValue("XOBJ", out int xobj_0) && xobj_0 <= 0)){
                thisPackage.Type = "Terrain Paint";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);  
            } else if ((typeDict.TryGetValue("BCON", out int bcon_2) && bcon_2 >= 1) && (typeDict.TryGetValue("BHAV", out int bhav_2) && bhav_2 >= 1) && (typeDict.TryGetValue("CRES", out int cres_0) && cres_0 >= 1) && (typeDict.TryGetValue("CTSS", out int ctss_0) && ctss_0 >= 1)){
                thisPackage.Type = "Functional Object";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);  
            } else if ((typeDict.TryGetValue("COLL", out int coll_0) && coll_0 >= 1)){
                thisPackage.Type = "Collection";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);  
            } else if ((typeDict.TryGetValue("BCON", out int bcon_3) && bcon_3 >= 1) && (typeDict.TryGetValue("TRCN", out int trcn_0) && trcn_0 >= 1) && (typeDict.TryGetValue("BHAV", out int bhav_3) && bhav_3 <= 0) && (typeDict.TryGetValue("TTAB", out int ttab_0) && ttab_0 <= 0)){
                thisPackage.Type = "Tuning Mod";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);  
            } else if ((typeDict.TryGetValue("BHAV", out int bhav_4) && bhav_4 >= 1) && (typeDict.TryGetValue("GLOB", out int glob_0) && glob_0 >= 1) && (typeDict.TryGetValue("OBJD", out int objd_2) && objd_2 >= 1) && (typeDict.TryGetValue("GMDC", out int gmdc_0) && gmdc_0 <= 0) && (typeDict.TryGetValue("GMND", out int gmnd_0) && gmnd_0 <= 0)){
                thisPackage.Type = "Mod";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);  
            } else if ((typeDict.TryGetValue("MMAT", out int mmat_2) && mmat_2 >= 1) && (typeDict.TryGetValue("DIR", out int dir_2) && dir_2 >= 1) && (typeDict.TryGetValue("TXTR", out int txtr_2) && txtr_2 >= 1) && (typeDict.TryGetValue("GMDC", out int gmdc_1) && gmdc_1 <= 0) && (typeDict.TryGetValue("GMND", out int gmnd_1) && gmnd_1 <= 0)){
                thisPackage.Type = "Object Recolor";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);  
            } else if ((typeDict.TryGetValue("BHAV", out int bhav_5) && bhav_5 >= 1) && (typeDict.TryGetValue("GMND", out int gmnd_2) && gmnd_2 >= 1) && (typeDict.TryGetValue("GMDC", out int gmdc_2) && gmdc_2 >= 1) && (typeDict.TryGetValue("SHPE", out int shpe_2) && shpe_2 >= 1)){
                thisPackage.Type = "Object Mesh";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);  
            } else if ((typeDict.TryGetValue("BHAV", out int bhav_6) && bhav_6 >= 1) && (typeDict.TryGetValue("GMND", out int gmnd_3) && gmnd_3 >= 1) && (typeDict.TryGetValue("GMDC", out int gmdc_3) && gmdc_3 >= 1) && (typeDict.TryGetValue("CTSS", out int ctss_1) && ctss_1 >= 1)){
                thisPackage.Type = "Object";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);  
            } else if ((typeDict.TryGetValue("TXMT", out int txmt_1) && txmt_1 >= 1) && (typeDict.TryGetValue("TXTR", out int txtr_3) && txtr_3 >= 1) && (typeDict.TryGetValue("GZPS", out int gzps_0) && gzps_0 >= 1) && (typeDict.TryGetValue("GMND", out int gmnd_4) && gmnd_4 <= 0) && (typeDict.TryGetValue("GMDC", out int gmdc_4) && gmdc_4 <= 0) && (typeDict.TryGetValue("CRES", out int cres_1) && cres_1 <= 0)){
                thisPackage.Type = "Clothing Recolor";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);  
            } else if ((typeDict.TryGetValue("SHPE", out int shpe_3) && shpe_3 >= 1) && (typeDict.TryGetValue("CRES", out int cres_2) && cres_2 >= 1) && (typeDict.TryGetValue("GMDC", out int gmdc_5) && gmdc_5 >= 1) && (typeDict.TryGetValue("OBJD", out int objd_3) && objd_3 <= 0)){
                thisPackage.Type = "Body Mesh";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);  
            } else if ((typeDict.TryGetValue("XHTN", out int xhtn_0) && xhtn_0 >= 1) && (typeDict.TryGetValue("SHPE", out int shpe_4) && shpe_4 >= 1)){
                thisPackage.Type = "Hair";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);  
            } else if ((typeDict.TryGetValue("XHTN", out int xhtn_1) && xhtn_1 >= 1) && (typeDict.TryGetValue("SHPE", out int shpe_5) && shpe_5 == 0)){
                thisPackage.Type = "Hair Recolor";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);           
            } else if ((typeDict.TryGetValue("GMDC", out int gmdc_6) && gmdc_6 >= 1) && (typeDict.TryGetValue("GMND", out int gmnd_5) && gmnd_5 >= 1) && (typeDict.TryGetValue("SHPE", out int shpe_6) && shpe_6 >= 1)) {
                thisPackage.Type = "Misc Mesh";
                log.MakeLog("This is some kind of mesh!!", true);
            } else {
                thisPackage.Type = "Currently Unknown";
                log.MakeLog("Logging as currently unknown.", true);
            }
        
           
            #region Get Title & Description 
            
            if (!String.IsNullOrWhiteSpace(ctssvar.Title)) {
                log.MakeLog("Getting title " + ctssvar.Title + " from ctssvar.", true);
                thisPackage.Title = ctssvar.Title;
            } else if (!String.IsNullOrWhiteSpace(dirvar.Title)){
                log.MakeLog("Getting title " + dirvar.Title + " from dirvar.", true);
                thisPackage.Title = dirvar.Title;
            } else if (!String.IsNullOrWhiteSpace(objdvar.Title)) {
                log.MakeLog("Getting title " + objdvar.Title + " from objdvar.", true);
                thisPackage.Title = objdvar.Title;
            } else if (!String.IsNullOrWhiteSpace(strvar.Title)) {
                log.MakeLog("Getting title " + strvar.Title + " from strvar.", true);
                thisPackage.Title = strvar.Title;
            } else if (!String.IsNullOrWhiteSpace(mmatvar.Title)) {
                Console.WriteLine("Mmatvar has content.");
                log.MakeLog("Getting title " + mmatvar.Title + " from mmatvar.", true);
            }


            if (!String.IsNullOrWhiteSpace(ctssvar.Description)){
                log.MakeLog("Getting description " + ctssvar.Description + " from ctssvar.", true);
                thisPackage.Description = ctssvar.Description;
            } else if (!String.IsNullOrWhiteSpace(dirvar.Description)) {
                log.MakeLog("Getting description " + dirvar.Description + " from dirvar.", true);
                thisPackage.Description = dirvar.Description;
            } else if (!String.IsNullOrWhiteSpace(objdvar.Description)) {
                log.MakeLog("Getting title " + objdvar.Title + " from objdvar.", true);
                thisPackage.Description = objdvar.Description;
            } else if (!String.IsNullOrWhiteSpace(strvar.Description)) {
                log.MakeLog("Getting title " + strvar.Title + " from strvar.", true);
                thisPackage.Description = strvar.Description;
            } else if (!String.IsNullOrWhiteSpace(mmatvar.Description)) {
                thisPackage.Title = mmatvar.Title;
                log.MakeLog("Getting title " + mmatvar.Title + " from mmatvar.", true);
                thisPackage.Description = mmatvar.Description;
            }

            #endregion

            #region Get Info

                if (!String.IsNullOrWhiteSpace(dirvar.XMLSubtype)){
                    log.MakeLog("Getting XMLSubtype " + dirvar.XMLSubtype + " from dirvar.", true);
                    thisPackage.XMLSubtype = dirvar.XMLSubtype;
                }
                if (!String.IsNullOrWhiteSpace(dirvar.XMLType)){
                    log.MakeLog("Getting Type " + dirvar.XMLType + " from dirvar.", true);
                    thisPackage.XMLType = dirvar.XMLType;
                }
                if (!String.IsNullOrWhiteSpace(dirvar.XMLCategory)){
                    log.MakeLog("Getting xmlCategory " + dirvar.XMLCategory + " from dirvar.", true);
                    thisPackage.XMLCategory = dirvar.XMLCategory;
                }
                if (!String.IsNullOrWhiteSpace(dirvar.XMLModelName)){
                    log.MakeLog("Getting XMLModelName " + dirvar.XMLModelName + " from dirvar.", true);
                    thisPackage.XMLModelName = dirvar.XMLModelName;
                }
                if (dirvar.ObjectGUID?.Any() != true){
                    log.MakeLog("Getting ObjectGUID " + dirvar.ObjectGUID.ToString() + " from dirvar.", true);
                    allGUIDS.AddRange(objdvar.ObjectGUID);
                }
                if (!String.IsNullOrWhiteSpace(dirvar.XMLCreator)){
                    log.MakeLog("Getting XMLCreator " + dirvar.XMLCreator + " from dirvar.", true);
                    thisPackage.XMLCreator = dirvar.XMLCreator;
                }
                if (!String.IsNullOrWhiteSpace(dirvar.XMLAge)){
                    log.MakeLog("Getting XMLAge " + dirvar.XMLAge + " from dirvar.", true);
                    thisPackage.XMLAge = dirvar.XMLAge;
                }
                if (!String.IsNullOrWhiteSpace(dirvar.XMLGender)){
                    log.MakeLog("Getting XMLGender " + dirvar.XMLGender + " from dirvar.", true);
                    thisPackage.XMLGender = dirvar.XMLGender;
                }

            #endregion
            
            #region Get Function

                if (!String.IsNullOrWhiteSpace(objdvar.Function)){
                    log.MakeLog("Getting Function " + objdvar.Function + " from objdvar.", true);
                    thisPackage.Function = objdvar.Function;
                }
                if (!String.IsNullOrWhiteSpace(objdvar.Function)){
                    log.MakeLog("Getting FunctionSubcategory " + objdvar.FunctionSubcategory + " from objdvar.", true);
                    thisPackage.FunctionSubcategory = objdvar.FunctionSubcategory;
                }
                log.MakeLog("Getting RequiredEPs " + objdvar.RequiredEPs.ToString() + " from objdvar.", true);
                thisPackage.RequiredEPs = objdvar.RequiredEPs;
                log.MakeLog("Getting RoomSort " + objdvar.RoomSort.ToString() + " from objdvar.", true);
                thisPackage.RoomSort = objdvar.RoomSort;                
                log.MakeLog("Getting ObjectGUID " + objdvar.ObjectGUID.ToString() + " from objdvar.", true);
                allGUIDS.AddRange(objdvar.ObjectGUID);

            if ((thisPackage.XMLType == "floor" || thisPackage.XMLType == "wallpaper")) {
                thisPackage.FunctionSubcategory = thisPackage.XMLSubtype;
            }
            if (!String.IsNullOrWhiteSpace(thisPackage.XMLType)){
                thisPackage.Function = thisPackage.XMLType;
                thisPackage.Type = thisPackage.XMLType;
            }

            //log.MakeLog("In infovar: " + infovar.ToString(), true);

            #endregion

            //if (fileHas.ExistsExists(x => x.term == "OBJD"))

            distinctGUIDS = allGUIDS.Distinct().ToList();
            thisPackage.ObjectGUID.AddRange(distinctGUIDS);
            log.MakeLog("In thisPackage: " + thisPackage.ToString(), true);
            log.MakeLog(thisPackage.ToString(), false);
            Containers.Containers.allSims2Packages.Add(thisPackage);

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
            packageinfo = new FileInfo(file); 
            allGUIDS = new List<string>();      
            distinctGUIDS = new List<string>();  

            readFile.Close();
            Console.WriteLine("Closing Package #" + packageparsecount + "/" + GlobalVariables.PackageCount + ": " + packageinfo.Name);
            
        }
    }
}