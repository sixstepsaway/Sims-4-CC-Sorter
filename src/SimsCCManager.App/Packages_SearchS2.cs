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
        LoggingGlobals log = new LoggingGlobals();
        ReadEntries readentries = new ReadEntries();   

        //Vars
        uint chunkOffset = 0;        
       

        public void SearchS2Packages(BinaryReader readFile, FileInfo packageinfo, uint minor, int packageparsecount) {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            log.MakeLog(string.Format("File {0} arrived for processing as Sims 2 file.", packageinfo.Name), true);
            string txt = string.Format("SELECT * FROM Processing_Reader where Name='{0}'", Methods.FixApostrophesforSQL(packageinfo.Name));
            var queries = GlobalVariables.DatabaseConnection.Query<PackageFile>(txt);
            var query = queries[0];
            GlobalVariables.DatabaseConnection.Delete(query);
            var pk = new PackageFile { ID = query.ID, Name = packageinfo.Name, Location = packageinfo.FullName, Game = 4, Broken = false, Status = "Processing"};
            GlobalVariables.DatabaseConnection.Insert(pk);
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
            if (minor == 0) indexmajorloc = 32;
            //create readers              
            //MemoryStream dbpfFile = Methods.ReadBytesToFile(packageinfo.FullName);
            //BinaryReader readFile = new BinaryReader(dbpfFile);
    
            thisPackage.FileSize = (int)packageinfo.Length;   
            thisPackage.PackageName = packageinfo.Name;
            thisPackage.GameString = "The Sims 2";
            
            log.MakeLog(string.Format("Reading package # {0}/{1}: {2}", packageparsecount, GlobalVariables.PackageCount, packageinfo.Name), true);
            
            thisPackage.Location = packageinfo.FullName;            
            thisPackage.Game = 2;
            log.MakeLog(string.Format("Package #{0} registered as {1} and meant for Sims 2", packageparsecount, packageinfo.FullName), true);          
            
            readFile.BaseStream.Position = indexmajorloc;

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
            
            readFile.BaseStream.Position = indexminorloc;

            uint indexMinorVersion = readFile.ReadUInt32() -1;
            test = indexMinorVersion.ToString();
            log.MakeLog("P" + packageparsecount + " - Index Minor Version: " + test, true);
            
            log.MakeLog("P" + packageparsecount + " - ChunkOffset: " + chunkOffset, true);

            readFile.BaseStream.Position = chunkOffset + indexOffset;
            for (int i = 0; i < indexCount; i++) {
                indexEntry holderEntry = new indexEntry();
                log.MakeLog("P" + packageparsecount + " - Made index entry.", true);
                holderEntry.typeID = readFile.ReadUInt32().ToString("X8");
                log.MakeLog("P" + packageparsecount + "/E" + i + " - Index Entry TypeID: " + holderEntry.typeID, true);
                
                holderEntry.groupID = readFile.ReadUInt32().ToString("X8");
                log.MakeLog("P" + packageparsecount + " - Index Entry GroupID: " + holderEntry.groupID, true);
                holderEntry.instanceID = readFile.ReadUInt32().ToString("X8");
                
                allInstanceIDs.Add(holderEntry.instanceID.ToString());
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

                var typef = from tp in TypeListings.AllTypesS2
                            where tp.typeID == iEntry.typeID
                            select tp.desc;
                
                
                if(typef.Any()){
                    List<string> tps = new List<string>();
                    tps.AddRange(typef);
                    fileHas.Add(new fileHasList() { TypeID = tps[0], Location = entrynum});
                    log.MakeLog(string.Format("P{0} - Found: {1}", packageparsecount, tps[0]), true);
                }
                entrynum++;
            }

            log.MakeLog("P" + packageparsecount + " - This file has:", true);
            foreach (fileHasList item in fileHas) {
                log.MakeLog("--- " + item.TypeID + " at: " + item.Location, true);
            }

            if (fileHas.Exists(x => x.TypeID == "DIR")) {       
                
                var dirget = from has in fileHas
                            where has.TypeID == "DIR"
                            select has.Location;
                var dirs = dirget.ToList();
                dirnum = dirs[0];                

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
                    allInstanceIDs.Add(holderEntry.instanceID.ToString());
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
            
            if (fileHas.Exists(x => x.TypeID == "DIR")) {
                //dbpfFile.Seek(chunkOffset + indexData[dirnum].offset, SeekOrigin.Begin);
                readFile.BaseStream.Position = chunkOffset + indexData[dirnum].offset;
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
                    allInstanceIDs.Add(holderEntry.instanceID.ToString());
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
                            //dbpfFile.Seek(this.chunkOffset + idx.offset, SeekOrigin.Begin);
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
								//dbpfFile.Seek(this.chunkOffset + idx.offset, SeekOrigin.Begin);
                                readFile.BaseStream.Position = this.chunkOffset + idx.offset;
								ctssvar = readentries.readCTSSchunk(readFile);
							}
                        } else if (typefound == "XOBJ" || typefound == "XFNC" || typefound == "XFLR" || typefound == "XMOL" || typefound == "XROF"  || typefound == "XTOL"  || typefound == "XHTN"){
                            log.MakeLog("Confirming found " + typefound + " and moving forward.", true);
                            //dbpfFile.Seek(this.chunkOffset + idx.offset, SeekOrigin.Begin);
                            readFile.BaseStream.Position = this.chunkOffset + idx.offset;
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
                                    //dbpfFile.Seek(this.chunkOffset + idx.offset + 9, SeekOrigin.Begin);
                                    readFile.BaseStream.Position = this.chunkOffset + idx.offset + 9;
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
            

            if (fileHas.Exists(x => x.TypeID == "OBJD")) {
                
                var obd = from has in fileHas
                            where has.TypeID == "OBJD"
                            select has.Location;
                objdnum = obd.ToList();


                
                foreach (int objloc in objdnum) {
                    log.MakeLog(string.Format("---------- [{0}]", objloc), true);               
                    
                    readFile.BaseStream.Position = this.chunkOffset + indexData[objloc].offset;
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
                        readFile.BaseStream.Position = this.chunkOffset + indexData[objloc].offset;
                        objdvar = readentries.readOBJDchunk(readFile);
                        log.MakeLog("objdvar returned with: " + objdvar.ToString(), true);
                    }
                }
                
            }

            if (fileHas.Exists(x => x.TypeID == "STR#"))
            {
                var strr = from has in fileHas
                            where has.TypeID == "STR#"
                            select has.Location;
                strnm = strr.ToList();
                
                foreach (int strloc in strnm) {
                    log.MakeLog("P" + packageparsecount + " - STR entry confirmation: ", true);
                    log.MakeLog(indexData[strloc].typeID, true);
                    //dbpfFile.Seek(chunkOffset + indexData[strloc].offset, SeekOrigin.Begin);
                    readFile.BaseStream.Position = chunkOffset + indexData[strloc].offset;
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
            if (fileHas.Exists(x => x.TypeID == "MMAT"))
            {
                List<int> entryspots = new List<int>();
                var csp = from has in fileHas
                        where has.TypeID == "MMAT"
                        select has.Location;
                entryspots = csp.ToList();
                mmatloc = entryspots[0];
                //dbpfFile.Seek(this.chunkOffset + indexData[mmatloc].offset, SeekOrigin.Begin);
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
                var csp = from has in fileHas
                        where has.TypeID == "SHPE"
                        select has.Location;
                entryspots = csp.ToList();
                int entryloc = entryspots[0];
                //dbpfFile.Seek(this.chunkOffset + indexData[mmatloc].offset, SeekOrigin.Begin);
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
            //log.MakeLog("Dirvar contains: " + dirvar.ToString(), true);
            //log.MakeLog("Ctssvar contains: " + ctssvar.ToString(), true);
            //log.MakeLog("Mmatvar contains: " + mmatvar.ToString(), true);
            //log.MakeLog("Objdvar contains: " + objdvar.ToString(), true);
            //log.MakeLog("Strvar contains: " + strvar.ToString(), true);

            List<TypeCounter> typecount = new List<TypeCounter>();
            var typeDict = new Dictionary<string, int>();

            foreach (fileHasList item in fileHas){
                foreach (typeList type in TypeListings.AllTypesS2){
                    if (type.desc == item.TypeID){
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
            thisPackage.FileHas.AddRange(fileHas);
            thisPackage.Entries.AddRange(typecount);

            
            int xngb; int str; int txtr; int dir; int txmt; int shpe; int bcon; int bhav; int mmat; int objf; int objd; int clst; int xobj; int cres; int ctss; int coll; int trcn; int ttab; int glob; int gmdc; int gmnd; int gzps; int xhtn; 
            

            if ((typeDict.TryGetValue("XNGB", out xngb) && xngb >= 1) && (typeDict.TryGetValue("STR#", out str) && str >= 1)){
                thisPackage.Type = "Floor";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);  
            } else if ((typeDict.TryGetValue("TXTR", out txtr) && txtr >= 1) && (typeDict.TryGetValue("STR#", out str) && str >= 1) && (typeDict.TryGetValue("DIR", out dir) && dir >= 1) && (typeDict.TryGetValue("TXMT", out txmt) && txmt >= 1) && (typeDict.TryGetValue("SHPE", out shpe) && shpe <= 0) && (typeDict.TryGetValue("BCON", out bcon) && bcon <= 0) && (typeDict.TryGetValue("BHAV", out bhav) && bhav <= 0) && (typeDict.TryGetValue("MMAT", out mmat) && mmat <= 0) && (typeDict.TryGetValue("OBJF", out objf) && objf <= 0) && (typeDict.TryGetValue("OBJD", out objd) && objd <= 0) && (typeDict.TryGetValue("CLST", out clst) && clst <= 0)){
                thisPackage.Type = "Floor";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);  
            } else if ((typeDict.TryGetValue("TXTR", out txtr) && txtr >= 1) && (typeDict.TryGetValue("STR#", out str) && str >= 1) && (typeDict.TryGetValue("DIR", out dir) && dir >= 1) && (typeDict.TryGetValue("SHPE", out shpe) && shpe <= 0) && (typeDict.TryGetValue("BCON", out bcon) && bcon <= 0) && (typeDict.TryGetValue("BHAV", out bhav) && bhav <= 0) && (typeDict.TryGetValue("MMAT", out mmat) && mmat <= 0) && (typeDict.TryGetValue("OBJF", out objf) && objf <= 0) && (typeDict.TryGetValue("OBJD", out objd) && objd <= 0) && (typeDict.TryGetValue("CLST", out clst) && clst <= 0) && (typeDict.TryGetValue("XOBJ", out xobj) && xobj <= 0)){
                thisPackage.Type = "Terrain Paint";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);  
            } else if ((typeDict.TryGetValue("BCON", out bcon) && bcon >= 1) && (typeDict.TryGetValue("BHAV", out bhav) && bhav >= 1) && (typeDict.TryGetValue("CRES", out cres) && cres >= 1) && (typeDict.TryGetValue("CTSS", out ctss) && ctss >= 1)){
                thisPackage.Type = "Functional Object";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);  
            } else if ((typeDict.TryGetValue("COLL", out coll) && coll >= 1)){
                thisPackage.Type = "Collection";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);  
            } else if ((typeDict.TryGetValue("BCON", out bcon) && bcon >= 1) && (typeDict.TryGetValue("TRCN", out trcn) && trcn >= 1) && (typeDict.TryGetValue("BHAV", out bhav) && bhav <= 0) && (typeDict.TryGetValue("TTAB", out ttab) && ttab <= 0)){
                thisPackage.Type = "Tuning Mod";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);  
            } else if ((typeDict.TryGetValue("BHAV", out bhav) && bhav >= 1) && (typeDict.TryGetValue("GLOB", out glob) && glob >= 1) && (typeDict.TryGetValue("OBJD", out objd) && objd >= 1) && (typeDict.TryGetValue("GMDC", out gmdc) && gmdc <= 0) && (typeDict.TryGetValue("GMND", out gmnd) && gmnd <= 0)){
                thisPackage.Type = "Mod";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);  
            } else if ((typeDict.TryGetValue("MMAT", out mmat) && mmat >= 1) && (typeDict.TryGetValue("DIR", out dir) && dir >= 1) && (typeDict.TryGetValue("TXTR", out txtr) && txtr >= 1) && (typeDict.TryGetValue("GMDC", out gmdc) && gmdc <= 0) && (typeDict.TryGetValue("GMND", out gmnd) && gmnd <= 0)){
                thisPackage.Type = "Object Recolor";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);  
            } else if ((typeDict.TryGetValue("BHAV", out bhav) && bhav >= 1) && (typeDict.TryGetValue("GMND", out gmnd) && gmnd >= 1) && (typeDict.TryGetValue("GMDC", out gmdc) && gmdc >= 1) && (typeDict.TryGetValue("SHPE", out shpe) && shpe >= 1)){
                thisPackage.Type = "Object Mesh";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);  
            } else if ((typeDict.TryGetValue("BHAV", out bhav) && bhav >= 1) && (typeDict.TryGetValue("GMND", out gmnd) && gmnd >= 1) && (typeDict.TryGetValue("GMDC", out gmdc) && gmdc >= 1) && (typeDict.TryGetValue("CTSS", out ctss) && ctss >= 1)){
                thisPackage.Type = "Object";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);  
            } else if ((typeDict.TryGetValue("TXMT", out txmt) && txmt >= 1) && (typeDict.TryGetValue("TXTR", out txtr) && txtr >= 1) && (typeDict.TryGetValue("GZPS", out gzps) && gzps >= 1) && (typeDict.TryGetValue("GMND", out gmnd) && gmnd <= 0) && (typeDict.TryGetValue("GMDC", out gmdc) && gmdc <= 0) && (typeDict.TryGetValue("CRES", out cres) && cres <= 0)){
                thisPackage.Type = "Clothing Recolor";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);  
            } else if ((typeDict.TryGetValue("SHPE", out shpe) && shpe >= 1) && (typeDict.TryGetValue("CRES", out cres) && cres >= 1) && (typeDict.TryGetValue("GMDC", out gmdc) && gmdc >= 1) && (typeDict.TryGetValue("OBJD", out objd) && objd <= 0)){
                thisPackage.Type = "Body Mesh";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);  
            } else if ((typeDict.TryGetValue("XHTN", out xhtn) && xhtn >= 1) && (typeDict.TryGetValue("SHPE", out shpe) && shpe >= 1)){
                thisPackage.Type = "Hair";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);  
            } else if ((typeDict.TryGetValue("XHTN", out xhtn) && xhtn >= 1) && (typeDict.TryGetValue("SHPE", out shpe) && shpe == 0)){
                thisPackage.Type = "Hair Recolor";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);           
            } else if ((typeDict.TryGetValue("GMDC", out gmdc) && gmdc >= 1) && (typeDict.TryGetValue("GMND", out gmnd) && gmnd >= 1) && (typeDict.TryGetValue("SHPE", out shpe) && shpe >= 1)) {
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
                //Console.WriteLine("Mmatvar has content.");
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

                if (!String.IsNullOrWhiteSpace(dirvar.Subtype)){
                    log.MakeLog("Getting Subtype " + dirvar.Subtype + " from dirvar.", true);
                    thisPackage.Subtype = dirvar.Subtype;
                }
                if (!String.IsNullOrWhiteSpace(dirvar.Type)){
                    log.MakeLog("Getting Type " + dirvar.Type + " from dirvar.", true);
                    thisPackage.Type = dirvar.Type;
                }
                if (!String.IsNullOrWhiteSpace(dirvar.Category)){
                    log.MakeLog("Getting Category " + dirvar.Category + " from dirvar.", true);
                    thisPackage.Category = dirvar.Category;
                }
                if (!String.IsNullOrWhiteSpace(dirvar.ModelName)){
                    log.MakeLog("Getting ModelName " + dirvar.ModelName + " from dirvar.", true);
                    thisPackage.ModelName = dirvar.ModelName;
                }
                if (dirvar.GUIDs?.Any() != true){
                    log.MakeLog("Getting GUID " + dirvar.GUIDs.ToString() + " from dirvar.", true);
                    allGUIDS.AddRange(objdvar.GUIDs);
                }
                if (!String.IsNullOrWhiteSpace(dirvar.Creator)){
                    log.MakeLog("Getting Creator " + dirvar.Creator + " from dirvar.", true);
                    thisPackage.Creator = dirvar.Creator;
                }
                if (!String.IsNullOrWhiteSpace(dirvar.Age)){
                    log.MakeLog("Getting Age " + dirvar.Age + " from dirvar.", true);
                    thisPackage.Age = dirvar.Age;
                }
                if (!String.IsNullOrWhiteSpace(dirvar.Gender)){
                    log.MakeLog("Getting Gender " + dirvar.Gender + " from dirvar.", true);
                    thisPackage.Gender = dirvar.Gender;
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
                log.MakeLog("Getting GUID " + objdvar.GUIDs.ToString() + " from objdvar.", true);
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
            log.MakeLog("Package Summary: " + thisPackage.ToString(), true);
            log.MakeLog(thisPackage.ToString(), false);
            //Containers.Containers.allSimsPackages.Add(thisPackage);
            log.MakeLog(string.Format("Adding {0} to packages database.", thisPackage.PackageName), true);
            try {
                GlobalVariables.DatabaseConnection.InsertWithChildren(thisPackage, true);
            } catch (Exception e) {
                log.MakeLog(string.Format("Caught exception adding Sims 2 package to database. \n Exception: {0}", e), true);
            }
            
            log.MakeLog(string.Format("Added {0} to packages database successfully.", thisPackage.PackageName), true);
            txt = string.Format("SELECT * FROM Processing_Reader where Name='{0}'", Methods.FixApostrophesforSQL(packageinfo.Name));
            var closingquery = GlobalVariables.DatabaseConnection.Query<PackageFile>(txt);
            GlobalVariables.DatabaseConnection.Delete(closingquery[0]);

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
            readFile.Dispose();
            //dbpfFile.Dispose();
            log.MakeLog(string.Format("Closing package # {0}/{1}: {2}", packageparsecount, GlobalVariables.PackageCount, packageinfo.Name), true);
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            string elapsedtime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                ts.Hours, ts.Minutes, ts.Seconds,
                                ts.Milliseconds / 10);
            log.MakeLog(string.Format("Reading file {0} took {1}", packageinfo.Name, elapsedtime), true);
            
        }

        public void S2FindOrphans(SimsPackage package) {  
            List<string> guids = new List<string>();  
            log.MakeLog("Reading " + package.PackageName + " and checking for orphaned recolors.", true);        
            if ((package.Mesh == false) && (package.Recolor == true)){
                log.MakeLog(package.PackageName + ": Package has no mesh.", true);                
                foreach (string guid in package.GUIDs) {

                }
            }
            if ((package.Mesh == true) && (package.Recolor == false)){
                log.MakeLog(package.PackageName + ": Package has a mesh and no recolor.", true);  
                foreach (string guid in package.GUIDs) {
                    
                }
            }
        }

        public void S2RenamePackages(SimsPackage package){
            FileInfo og = new FileInfo(package.Location);
            string newlocation = og.Directory + "\\" + package.Title + ".package";
            if (File.Exists(newlocation)){
                //do nothing, throw an error at some point
            } else {
                System.IO.File.Move(package.Location, newlocation);
            }
        }
    }
}