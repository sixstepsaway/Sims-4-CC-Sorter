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

        //Vars
        uint chunkOffset = 0;        

        
        public static SimsPackage thisPackage = new SimsPackage();
        public static SimsPackage infovar = new SimsPackage();

        public void SearchS2Packages(string file) {
            var packageparsecount = GlobalVariables.packagesRead;   
            GlobalVariables.packagesRead++;         
            //Vars for Package Info
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
            uint myFilesize;      
        
            //Misc Vars
            string test = "";        
            int dirnum = 0;
            List<int> objdnum = new List<int>();   
            List<int> strnm = new List<int>();
            infovar = new SimsPackage();
            thisPackage = new SimsPackage();

            //Lists 
            
            List<fileHasList> fileHas = new List<fileHasList>();
            ArrayList linkData = new ArrayList();
            List<indexEntry> indexData = new List<indexEntry>();
            FileInfo packageinfo = new FileInfo(file); 

            //create readers  
            FileStream dbpfFile = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader readFile = new BinaryReader(dbpfFile);

            thisPackage.PackageName = packageinfo.Name;
            

            //start actually reading the package 
            Console.WriteLine("Reading Package #" + packageparsecount + ": " + packageinfo.Name);           
            log.MakeLog("Logged Package #" + packageparsecount + " as " + packageinfo.FullName, true);
            thisPackage.Location = packageinfo.FullName;
            thisPackage.Game = 2;
            log.MakeLog("Logged Package #" + packageparsecount + " as meant for The Sims " + thisPackage.Game, true);           
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
                //Console.WriteLine(holderEntry.instanceID);
                //infovar.InstanceIDs.Add(holderEntry.instanceID);
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
                        uint cFullSize = ReadEntries.QFSLengthToInt(tempBytes);

                        DecryptByteStream decompressed = new DecryptByteStream(ReadEntries.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));

                        infovar = readentries.readSTRchunk(decompressed);
                        if (thisPackage.Title == null) thisPackage.Title = infovar.Title;
                        if (thisPackage.Description == null) thisPackage.Description = infovar.Description;
                    } 
                    else 
                    {
                        infovar = readentries.readSTRchunk(readFile);
                        if (thisPackage.Title == null) thisPackage.Title = infovar.Title;
                        if (thisPackage.Description == null) thisPackage.Description = infovar.Description;
                        
                    }                    
                }
                
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
                    log.MakeLog("P" + packageparsecount + " - Reading compressed record #" + c, true);
                    typeID = readFile.ReadUInt32().ToString("X8");
                    log.MakeLog("P" + packageparsecount + " - CR#" + c + ": Type ID is " + typeID, true);
                    groupID = readFile.ReadUInt32().ToString("X8");
                    log.MakeLog("P" + packageparsecount + " - CR#" + c + ": Group ID is " + groupID, true);
                    instanceID = readFile.ReadUInt32().ToString("X8");
                    log.MakeLog("P" + packageparsecount + " - CR#" + c + ": Instance ID is " + instanceID, true);
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
                    log.MakeLog("", true);
                    log.MakeLog("Compressed Entry #" + j, true);
                    typeID = readFile.ReadUInt32().ToString("X8");
                    log.MakeLog("TypeID: "+ typeID, true);
                    groupID = readFile.ReadUInt32().ToString("X8");
                    log.MakeLog("GroupID: "+ groupID, true);
                    instanceID = readFile.ReadUInt32().ToString("X8");
                    log.MakeLog("InstanceID: "+ instanceID, true);
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
								uint cFullSize = ReadEntries.QFSLengthToInt(tempBytes);

								DecryptByteStream decompressed = new DecryptByteStream(ReadEntries.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));

								infovar = readentries.readCTSSchunk(decompressed);
                                if (thisPackage.Title == null) thisPackage.Title = infovar.Title;
                                if (thisPackage.Description == null) thisPackage.Description = infovar.Description;
                                if (thisPackage.Location == null) thisPackage.Location = infovar.Location;
                                if (thisPackage.PackageName == null) thisPackage.PackageName = infovar.PackageName;
                                if (thisPackage.Game == null) thisPackage.Game = infovar.Game;
                                if (thisPackage.DBPF == null) thisPackage.DBPF = infovar.DBPF;
                                if (thisPackage.Major == null) thisPackage.Major = infovar.Major;
                                if (thisPackage.Minor == null) thisPackage.Minor = infovar.Minor;
                                if (thisPackage.DateCreated == null) thisPackage.DateCreated = infovar.DateCreated;
                                if (thisPackage.DateModified == null) thisPackage.DateModified = infovar.DateModified;
                                if (thisPackage.IndexMajorVersion == null) thisPackage.IndexMajorVersion = infovar.IndexMajorVersion;
                                if (thisPackage.IndexCount == null) thisPackage.IndexCount = infovar.IndexCount;
                                if (thisPackage.IndexOffset == null) thisPackage.IndexOffset = infovar.IndexOffset;
                                if (thisPackage.IndexSize == null) thisPackage.IndexSize = infovar.IndexSize;
                                if (thisPackage.HolesCount == null) thisPackage.HolesCount = infovar.HolesCount;
                                if (thisPackage.HolesOffset == null) thisPackage.HolesOffset = infovar.HolesOffset;
                                if (thisPackage.HolesSize == null) thisPackage.HolesSize = infovar.HolesSize;
                                if (thisPackage.IndexMinorVersion == null) thisPackage.IndexMinorVersion = infovar.IndexMinorVersion;
                                if (thisPackage.XMLType == null) thisPackage.XMLType = infovar.XMLType;
                                if (thisPackage.XMLSubtype == null) thisPackage.XMLSubtype = infovar.XMLSubtype;
                                if (thisPackage.XMLCategory == null) thisPackage.XMLCategory = infovar.XMLCategory;
                                if (thisPackage.XMLModelName == null) thisPackage.XMLModelName = infovar.XMLModelName;
                                if (thisPackage.ObjectGUID == null) thisPackage.ObjectGUID = infovar.ObjectGUID;
                                if (thisPackage.XMLCreator == null) thisPackage.XMLCreator = infovar.XMLCreator;
                                if (thisPackage.XMLAge == null) thisPackage.XMLAge = infovar.XMLAge;
                                if (thisPackage.XMLGender == null) thisPackage.XMLGender = infovar.XMLGender;
                                if (thisPackage.RequiredEPs == null) thisPackage.RequiredEPs = infovar.RequiredEPs;
                                if (thisPackage.Function == null) thisPackage.Function = infovar.Function;
                                if (thisPackage.FunctionSubcategory == null) thisPackage.FunctionSubcategory = infovar.FunctionSubcategory;
                                if (thisPackage.RoomSort == null) thisPackage.RoomSort = infovar.RoomSort;
							} 
							else 
							{
								dbpfFile.Seek(this.chunkOffset + idx.offset, SeekOrigin.Begin);
								infovar = readentries.readCTSSchunk(readFile);
                                if (thisPackage.Title == null) thisPackage.Title = infovar.Title;
                                if (thisPackage.Description == null) thisPackage.Description = infovar.Description;
                                if (thisPackage.Location == null) thisPackage.Location = infovar.Location;
                                if (thisPackage.PackageName == null) thisPackage.PackageName = infovar.PackageName;
                                if (thisPackage.Game == null) thisPackage.Game = infovar.Game;
                                if (thisPackage.DBPF == null) thisPackage.DBPF = infovar.DBPF;
                                if (thisPackage.Major == null) thisPackage.Major = infovar.Major;
                                if (thisPackage.Minor == null) thisPackage.Minor = infovar.Minor;
                                if (thisPackage.DateCreated == null) thisPackage.DateCreated = infovar.DateCreated;
                                if (thisPackage.DateModified == null) thisPackage.DateModified = infovar.DateModified;
                                if (thisPackage.IndexMajorVersion == null) thisPackage.IndexMajorVersion = infovar.IndexMajorVersion;
                                if (thisPackage.IndexCount == null) thisPackage.IndexCount = infovar.IndexCount;
                                if (thisPackage.IndexOffset == null) thisPackage.IndexOffset = infovar.IndexOffset;
                                if (thisPackage.IndexSize == null) thisPackage.IndexSize = infovar.IndexSize;
                                if (thisPackage.HolesCount == null) thisPackage.HolesCount = infovar.HolesCount;
                                if (thisPackage.HolesOffset == null) thisPackage.HolesOffset = infovar.HolesOffset;
                                if (thisPackage.HolesSize == null) thisPackage.HolesSize = infovar.HolesSize;
                                if (thisPackage.IndexMinorVersion == null) thisPackage.IndexMinorVersion = infovar.IndexMinorVersion;
                                if (thisPackage.XMLType == null) thisPackage.XMLType = infovar.XMLType;
                                if (thisPackage.XMLSubtype == null) thisPackage.XMLSubtype = infovar.XMLSubtype;
                                if (thisPackage.XMLCategory == null) thisPackage.XMLCategory = infovar.XMLCategory;
                                if (thisPackage.XMLModelName == null) thisPackage.XMLModelName = infovar.XMLModelName;
                                if (thisPackage.ObjectGUID == null) thisPackage.ObjectGUID = infovar.ObjectGUID;
                                if (thisPackage.XMLCreator == null) thisPackage.XMLCreator = infovar.XMLCreator;
                                if (thisPackage.XMLAge == null) thisPackage.XMLAge = infovar.XMLAge;
                                if (thisPackage.XMLGender == null) thisPackage.XMLGender = infovar.XMLGender;
                                if (thisPackage.RequiredEPs == null) thisPackage.RequiredEPs = infovar.RequiredEPs;
                                if (thisPackage.Function == null) thisPackage.Function = infovar.Function;
                                if (thisPackage.FunctionSubcategory == null) thisPackage.FunctionSubcategory = infovar.FunctionSubcategory;
                                if (thisPackage.RoomSort == null) thisPackage.RoomSort = infovar.RoomSort;
							}
                        } else if (typefound == "XOBJ" || typefound == "XFNC" || typefound == "XFLR" || typefound == "XMOL" || typefound == "XROF"  || typefound == "XTOL"  || typefound == "MMAT" || typefound == "XHTN"){
                            log.MakeLog("Confirming found " + typefound + " and moving forward.", true);
                            dbpfFile.Seek(this.chunkOffset + idx.offset, SeekOrigin.Begin);
                            cFileSize = readFile.ReadInt32();
                            cTypeID = readFile.ReadUInt16().ToString("X4");
                            log.MakeLog(typefound + " size: " + cFileSize + ", ctypeid: " + cTypeID, true);
                            if (cTypeID == "FB10"){
                                log.MakeLog("FB10 confirmed.", true);
                                byte[] tempBytes = readFile.ReadBytes(3);
                                uint cFullSize = ReadEntries.QFSLengthToInt(tempBytes);
                                log.MakeLog("cFullSize is: " + cFileSize, true);
                                string cpfTypeID = readFile.ReadUInt32().ToString("X8");
                                log.MakeLog("cpfTypeID is: " + cpfTypeID, true);
                                if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0")){
                                    infovar = readentries.readCPFchunk(readFile);
                                    log.MakeLog("Real CPF file. Processing as CPF chunk.",true);
                                } else {
                                    log.MakeLog("Not a real CPF. Searching for more information.", true);
                                    dbpfFile.Seek(this.chunkOffset + idx.offset + 9, SeekOrigin.Begin);
                                    DecryptByteStream decompressed = new DecryptByteStream(ReadEntries.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));
                                    if (cpfTypeID == "E750E0E2")
                                    {
                                        // Read first four bytes
                                        cpfTypeID = decompressed.ReadUInt32().ToString("X8");
                                        log.MakeLog("Secondary cpf type id: " + cpfTypeID, true);
                                        if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0")) 
                                        {
                                            log.MakeLog("Real CPF. Decompressing.", true);
                                            infovar = readentries.readCPFchunk(decompressed);
                                            if (thisPackage.Title == null) thisPackage.Title = infovar.Title;
                                            if (thisPackage.Description == null) thisPackage.Description = infovar.Description;
                                            if (thisPackage.Location == null) thisPackage.Location = infovar.Location;
                                            if (thisPackage.PackageName == null) thisPackage.PackageName = infovar.PackageName;
                                            if (thisPackage.Game == null) thisPackage.Game = infovar.Game;
                                            if (thisPackage.DBPF == null) thisPackage.DBPF = infovar.DBPF;
                                            if (thisPackage.Major == null) thisPackage.Major = infovar.Major;
                                            if (thisPackage.Minor == null) thisPackage.Minor = infovar.Minor;
                                            if (thisPackage.DateCreated == null) thisPackage.DateCreated = infovar.DateCreated;
                                            if (thisPackage.DateModified == null) thisPackage.DateModified = infovar.DateModified;
                                            if (thisPackage.IndexMajorVersion == null) thisPackage.IndexMajorVersion = infovar.IndexMajorVersion;
                                            if (thisPackage.IndexCount == null) thisPackage.IndexCount = infovar.IndexCount;
                                            if (thisPackage.IndexOffset == null) thisPackage.IndexOffset = infovar.IndexOffset;
                                            if (thisPackage.IndexSize == null) thisPackage.IndexSize = infovar.IndexSize;
                                            if (thisPackage.HolesCount == null) thisPackage.HolesCount = infovar.HolesCount;
                                            if (thisPackage.HolesOffset == null) thisPackage.HolesOffset = infovar.HolesOffset;
                                            if (thisPackage.HolesSize == null) thisPackage.HolesSize = infovar.HolesSize;
                                            if (thisPackage.IndexMinorVersion == null) thisPackage.IndexMinorVersion = infovar.IndexMinorVersion;
                                            if (thisPackage.XMLType == null) thisPackage.XMLType = infovar.XMLType;
                                            if (thisPackage.XMLSubtype == null) thisPackage.XMLSubtype = infovar.XMLSubtype;
                                            if (thisPackage.XMLCategory == null) thisPackage.XMLCategory = infovar.XMLCategory;
                                            if (thisPackage.XMLModelName == null) thisPackage.XMLModelName = infovar.XMLModelName;
                                            if (thisPackage.ObjectGUID == null) thisPackage.ObjectGUID = infovar.ObjectGUID;
                                            if (thisPackage.XMLCreator == null) thisPackage.XMLCreator = infovar.XMLCreator;
                                            if (thisPackage.XMLAge == null) thisPackage.XMLAge = infovar.XMLAge;
                                            if (thisPackage.XMLGender == null) thisPackage.XMLGender = infovar.XMLGender;
                                            if (thisPackage.RequiredEPs == null) thisPackage.RequiredEPs = infovar.RequiredEPs;
                                            if (thisPackage.Function == null) thisPackage.Function = infovar.Function;
                                            if (thisPackage.FunctionSubcategory == null) thisPackage.FunctionSubcategory = infovar.FunctionSubcategory;
                                            if (thisPackage.RoomSort == null) thisPackage.RoomSort = infovar.RoomSort;
                                        } 
                                    } else 
                                    {
                                        log.MakeLog("Actually an XML. Reading.", true);
                                        infovar = readentries.readXMLchunk(decompressed);
                                        if (thisPackage.Title == null) thisPackage.Title = infovar.Title;
                                        if (thisPackage.Description == null) thisPackage.Description = infovar.Description;
                                        if (thisPackage.Location == null) thisPackage.Location = infovar.Location;
                                        if (thisPackage.PackageName == null) thisPackage.PackageName = infovar.PackageName;
                                        if (thisPackage.Game == null) thisPackage.Game = infovar.Game;
                                        if (thisPackage.DBPF == null) thisPackage.DBPF = infovar.DBPF;
                                        if (thisPackage.Major == null) thisPackage.Major = infovar.Major;
                                        if (thisPackage.Minor == null) thisPackage.Minor = infovar.Minor;
                                        if (thisPackage.DateCreated == null) thisPackage.DateCreated = infovar.DateCreated;
                                        if (thisPackage.DateModified == null) thisPackage.DateModified = infovar.DateModified;
                                        if (thisPackage.IndexMajorVersion == null) thisPackage.IndexMajorVersion = infovar.IndexMajorVersion;
                                        if (thisPackage.IndexCount == null) thisPackage.IndexCount = infovar.IndexCount;
                                        if (thisPackage.IndexOffset == null) thisPackage.IndexOffset = infovar.IndexOffset;
                                        if (thisPackage.IndexSize == null) thisPackage.IndexSize = infovar.IndexSize;
                                        if (thisPackage.HolesCount == null) thisPackage.HolesCount = infovar.HolesCount;
                                        if (thisPackage.HolesOffset == null) thisPackage.HolesOffset = infovar.HolesOffset;
                                        if (thisPackage.HolesSize == null) thisPackage.HolesSize = infovar.HolesSize;
                                        if (thisPackage.IndexMinorVersion == null) thisPackage.IndexMinorVersion = infovar.IndexMinorVersion;
                                        if (thisPackage.XMLType == null) thisPackage.XMLType = infovar.XMLType;
                                        if (thisPackage.XMLSubtype == null) thisPackage.XMLSubtype = infovar.XMLSubtype;
                                        if (thisPackage.XMLCategory == null) thisPackage.XMLCategory = infovar.XMLCategory;
                                        if (thisPackage.XMLModelName == null) thisPackage.XMLModelName = infovar.XMLModelName;
                                        if (thisPackage.ObjectGUID == null) thisPackage.ObjectGUID = infovar.ObjectGUID;
                                        if (thisPackage.XMLCreator == null) thisPackage.XMLCreator = infovar.XMLCreator;
                                        if (thisPackage.XMLAge == null) thisPackage.XMLAge = infovar.XMLAge;
                                        if (thisPackage.XMLGender == null) thisPackage.XMLGender = infovar.XMLGender;
                                        if (thisPackage.RequiredEPs == null) thisPackage.RequiredEPs = infovar.RequiredEPs;
                                        if (thisPackage.Function == null) thisPackage.Function = infovar.Function;
                                        if (thisPackage.FunctionSubcategory == null) thisPackage.FunctionSubcategory = infovar.FunctionSubcategory;
                                        if (thisPackage.RoomSort == null) thisPackage.RoomSort = infovar.RoomSort;
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
                        uint cFullSize = ReadEntries.QFSLengthToInt(tempBytes);
                        log.MakeLog("P" + packageparsecount + " - OBJD size is: " + cFullSize, true);
                        DecryptByteStream decompressed = new DecryptByteStream(ReadEntries.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));
                        infovar = readentries.readOBJDchunk(decompressed);
                        if (thisPackage.Title == null) thisPackage.Title = infovar.Title;
                        if (thisPackage.Description == null) thisPackage.Description = infovar.Description;
                        if (thisPackage.Location == null) thisPackage.Location = infovar.Location;
                        if (thisPackage.PackageName == null) thisPackage.PackageName = infovar.PackageName;
                        if (thisPackage.Game == null) thisPackage.Game = infovar.Game;
                        if (thisPackage.DBPF == null) thisPackage.DBPF = infovar.DBPF;
                        if (thisPackage.Major == null) thisPackage.Major = infovar.Major;
                        if (thisPackage.Minor == null) thisPackage.Minor = infovar.Minor;
                        if (thisPackage.DateCreated == null) thisPackage.DateCreated = infovar.DateCreated;
                        if (thisPackage.DateModified == null) thisPackage.DateModified = infovar.DateModified;
                        if (thisPackage.IndexMajorVersion == null) thisPackage.IndexMajorVersion = infovar.IndexMajorVersion;
                        if (thisPackage.IndexCount == null) thisPackage.IndexCount = infovar.IndexCount;
                        if (thisPackage.IndexOffset == null) thisPackage.IndexOffset = infovar.IndexOffset;
                        if (thisPackage.IndexSize == null) thisPackage.IndexSize = infovar.IndexSize;
                        if (thisPackage.HolesCount == null) thisPackage.HolesCount = infovar.HolesCount;
                        if (thisPackage.HolesOffset == null) thisPackage.HolesOffset = infovar.HolesOffset;
                        if (thisPackage.HolesSize == null) thisPackage.HolesSize = infovar.HolesSize;
                        if (thisPackage.IndexMinorVersion == null) thisPackage.IndexMinorVersion = infovar.IndexMinorVersion;
                        if (thisPackage.XMLType == null) thisPackage.XMLType = infovar.XMLType;
                        if (thisPackage.XMLSubtype == null) thisPackage.XMLSubtype = infovar.XMLSubtype;
                        if (thisPackage.XMLCategory == null) thisPackage.XMLCategory = infovar.XMLCategory;
                        if (thisPackage.XMLModelName == null) thisPackage.XMLModelName = infovar.XMLModelName;
                        if (thisPackage.ObjectGUID == null) thisPackage.ObjectGUID = infovar.ObjectGUID;
                        if (thisPackage.XMLCreator == null) thisPackage.XMLCreator = infovar.XMLCreator;
                        if (thisPackage.XMLAge == null) thisPackage.XMLAge = infovar.XMLAge;
                        if (thisPackage.XMLGender == null) thisPackage.XMLGender = infovar.XMLGender;
                        if (thisPackage.RequiredEPs == null) thisPackage.RequiredEPs = infovar.RequiredEPs;
                        if (thisPackage.Function == null) thisPackage.Function = infovar.Function;
                        if (thisPackage.FunctionSubcategory == null) thisPackage.FunctionSubcategory = infovar.FunctionSubcategory;
                        if (thisPackage.RoomSort == null) thisPackage.RoomSort = infovar.RoomSort;
                    } else { 
                        dbpfFile.Seek(this.chunkOffset + indexData[objloc].offset, SeekOrigin.Begin);
                        infovar = readentries.readOBJDchunk(readFile);
                        if (thisPackage.Title == null) thisPackage.Title = infovar.Title;
                        if (thisPackage.Description == null) thisPackage.Description = infovar.Description;
                        if (thisPackage.Location == null) thisPackage.Location = infovar.Location;
                        if (thisPackage.PackageName == null) thisPackage.PackageName = infovar.PackageName;
                        if (thisPackage.Game == null) thisPackage.Game = infovar.Game;
                        if (thisPackage.DBPF == null) thisPackage.DBPF = infovar.DBPF;
                        if (thisPackage.Major == null) thisPackage.Major = infovar.Major;
                        if (thisPackage.Minor == null) thisPackage.Minor = infovar.Minor;
                        if (thisPackage.DateCreated == null) thisPackage.DateCreated = infovar.DateCreated;
                        if (thisPackage.DateModified == null) thisPackage.DateModified = infovar.DateModified;
                        if (thisPackage.IndexMajorVersion == null) thisPackage.IndexMajorVersion = infovar.IndexMajorVersion;
                        if (thisPackage.IndexCount == null) thisPackage.IndexCount = infovar.IndexCount;
                        if (thisPackage.IndexOffset == null) thisPackage.IndexOffset = infovar.IndexOffset;
                        if (thisPackage.IndexSize == null) thisPackage.IndexSize = infovar.IndexSize;
                        if (thisPackage.HolesCount == null) thisPackage.HolesCount = infovar.HolesCount;
                        if (thisPackage.HolesOffset == null) thisPackage.HolesOffset = infovar.HolesOffset;
                        if (thisPackage.HolesSize == null) thisPackage.HolesSize = infovar.HolesSize;
                        if (thisPackage.IndexMinorVersion == null) thisPackage.IndexMinorVersion = infovar.IndexMinorVersion;
                        if (thisPackage.XMLType == null) thisPackage.XMLType = infovar.XMLType;
                        if (thisPackage.XMLSubtype == null) thisPackage.XMLSubtype = infovar.XMLSubtype;
                        if (thisPackage.XMLCategory == null) thisPackage.XMLCategory = infovar.XMLCategory;
                        if (thisPackage.XMLModelName == null) thisPackage.XMLModelName = infovar.XMLModelName;
                        if (thisPackage.ObjectGUID == null) thisPackage.ObjectGUID = infovar.ObjectGUID;
                        if (thisPackage.XMLCreator == null) thisPackage.XMLCreator = infovar.XMLCreator;
                        if (thisPackage.XMLAge == null) thisPackage.XMLAge = infovar.XMLAge;
                        if (thisPackage.XMLGender == null) thisPackage.XMLGender = infovar.XMLGender;
                        if (thisPackage.RequiredEPs == null) thisPackage.RequiredEPs = infovar.RequiredEPs;
                        if (thisPackage.Function == null) thisPackage.Function = infovar.Function;
                        if (thisPackage.FunctionSubcategory == null) thisPackage.FunctionSubcategory = infovar.FunctionSubcategory;
                        if (thisPackage.RoomSort == null) thisPackage.RoomSort = infovar.RoomSort;

                    }
                }
            }







            
            
                
            if (thisPackage.Title == null) thisPackage.Title = infovar.Title;
            if (thisPackage.Description == null) thisPackage.Description = infovar.Description;
            if (thisPackage.Location == null) thisPackage.Location = infovar.Location;
            if (thisPackage.PackageName == null) thisPackage.PackageName = infovar.PackageName;
            if (thisPackage.Game == null) thisPackage.Game = infovar.Game;
            if (thisPackage.DBPF == null) thisPackage.DBPF = infovar.DBPF;
            if (thisPackage.Major == null) thisPackage.Major = infovar.Major;
            if (thisPackage.Minor == null) thisPackage.Minor = infovar.Minor;
            if (thisPackage.DateCreated == null) thisPackage.DateCreated = infovar.DateCreated;
            if (thisPackage.DateModified == null) thisPackage.DateModified = infovar.DateModified;
            if (thisPackage.IndexMajorVersion == null) thisPackage.IndexMajorVersion = infovar.IndexMajorVersion;
            if (thisPackage.IndexCount == null) thisPackage.IndexCount = infovar.IndexCount;
            if (thisPackage.IndexOffset == null) thisPackage.IndexOffset = infovar.IndexOffset;
            if (thisPackage.IndexSize == null) thisPackage.IndexSize = infovar.IndexSize;
            if (thisPackage.HolesCount == null) thisPackage.HolesCount = infovar.HolesCount;
            if (thisPackage.HolesOffset == null) thisPackage.HolesOffset = infovar.HolesOffset;
            if (thisPackage.HolesSize == null) thisPackage.HolesSize = infovar.HolesSize;
            if (thisPackage.IndexMinorVersion == null) thisPackage.IndexMinorVersion = infovar.IndexMinorVersion;
            if (thisPackage.XMLType == null) thisPackage.XMLType = infovar.XMLType;
            if (thisPackage.XMLSubtype == null) thisPackage.XMLSubtype = infovar.XMLSubtype;
            if (thisPackage.XMLCategory == null) thisPackage.XMLCategory = infovar.XMLCategory;
            if (thisPackage.XMLModelName == null) thisPackage.XMLModelName = infovar.XMLModelName;
            if (thisPackage.ObjectGUID == null) thisPackage.ObjectGUID = infovar.ObjectGUID;
            if (thisPackage.XMLCreator == null) thisPackage.XMLCreator = infovar.XMLCreator;
            if (thisPackage.XMLAge == null) thisPackage.XMLAge = infovar.XMLAge;
            if (thisPackage.XMLGender == null) thisPackage.XMLGender = infovar.XMLGender;
            if (thisPackage.RequiredEPs == null) thisPackage.RequiredEPs = infovar.RequiredEPs;
            if (thisPackage.Function == null) thisPackage.Function = infovar.Function;
            if (thisPackage.FunctionSubcategory == null) thisPackage.FunctionSubcategory = infovar.FunctionSubcategory;
            if (thisPackage.RoomSort == null) thisPackage.RoomSort = infovar.RoomSort;

            //if (fileHas.ExistsExists(x => x.term == "OBJD"))


            log.MakeLog("In thisPackage: " + thisPackage.ToString(), true);

            Containers.Containers.allSims2Packages.Add(thisPackage);
            thisPackage = new SimsPackage();
            infovar = new SimsPackage();

            readFile.Close();
            Console.WriteLine("Closing Package #" + packageparsecount + ": " + packageinfo.Name);
            
        }
    }
}