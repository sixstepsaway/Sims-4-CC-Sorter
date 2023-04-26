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
        // arrays and such to hold the information
        LoggingGlobals log = new LoggingGlobals();
        ArrayList linkData = new ArrayList();
        ArrayList indexData = new ArrayList();

        //vars to collect information in

        uint chunkOffset = 0;
        string typefound = "";
        string test = "";
        string instanceID2;
        string typeID;
        string groupID;
        string instanceID;
        string title;
        uint compfilesize;
        uint numRecords;
        string cTypeID;
        int cFileSize;

        ReadEntries readentries = new ReadEntries();        
        public static SimsPackage thisPackage = new SimsPackage();
        public static SimsPackage infovar = new SimsPackage();
        public void SearchS2Packages(String file)
        {              
            FileInfo packageFile = new FileInfo(file);
            thisPackage.Location = file;
            thisPackage.Game = 2;
            FileStream dbpfFile = new FileStream(packageFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader readFile = new BinaryReader(dbpfFile);
            test = Encoding.ASCII.GetString(readFile.ReadBytes(4));
            log.MakeLog(test, true);
            int bytes = 4;
            uint major = readFile.ReadUInt32();
            test = major.ToString();  
            log.MakeLog("Major:" + test, true);

            uint minor = readFile.ReadUInt32();
            test = minor.ToString();
            log.MakeLog("Minor:" + test, true);
            
            string reserved = Encoding.UTF8.GetString(readFile.ReadBytes(12));
            test = reserved;
            log.MakeLog("Reserved:" + test, true);
            
            uint dateCreated = readFile.ReadUInt32();
            test = dateCreated.ToString();
            log.MakeLog("Date created:" + test, true);
            
            uint dateModified = readFile.ReadUInt32();
            test = dateModified.ToString();
            log.MakeLog("Date modified:" + test, true);
            
            uint indexMajorVersion = readFile.ReadUInt32();
            test = indexMajorVersion.ToString();
            log.MakeLog("Index Major:" + test, true);
            
            uint indexCount = readFile.ReadUInt32();
            test = indexCount.ToString();
            log.MakeLog("Index Count:" + test, true);
            
            uint indexOffset = readFile.ReadUInt32();
            test = indexOffset.ToString();
            log.MakeLog("Index Offset:" + test, true);
            
            uint indexSize = readFile.ReadUInt32();
            test = indexSize.ToString();
            log.MakeLog("Index Size:" + test, true);
            
            uint holesCount = readFile.ReadUInt32();
            test = holesCount.ToString();
            log.MakeLog("Holes Count:" + test, true);

            uint holesOffset = readFile.ReadUInt32();
            test = holesOffset.ToString();
            log.MakeLog("Holes Offset:" + test, true);
            
            uint holesSize = readFile.ReadUInt32();
            test = holesSize.ToString();
            log.MakeLog("Holes Size:" + test, true);
            
            uint indexMinorVersion = readFile.ReadUInt32() -1;
            test = indexMinorVersion.ToString();
            log.MakeLog("Index Minor Version:" + test, true);
            
            string reserved2 = Encoding.UTF8.GetString(readFile.ReadBytes(32));
            log.MakeLog("Reserved 2:" + reserved2, true);

            log.MakeLog("ChunkOffset: " + chunkOffset, true);

            dbpfFile.Seek(chunkOffset + indexOffset, SeekOrigin.Begin);

            for (int i = 0; i < indexCount; i++) {
                indexEntry holderEntry = new indexEntry();

                holderEntry.typeID = readFile.ReadUInt32().ToString("X8");
                log.MakeLog("Index Entry TypeID: " + holderEntry.typeID, true);

                holderEntry.groupID = readFile.ReadUInt32().ToString("X8");
                log.MakeLog("Index Entry GroupID: " + holderEntry.groupID, true);

                holderEntry.instanceID = readFile.ReadUInt32().ToString("X8");
                log.MakeLog("InstanceID: " + holderEntry.instanceID, true);

                if ((indexMajorVersion == 7) && (indexMinorVersion == 1)) {
                    holderEntry.instanceID2 = readFile.ReadUInt32().ToString("X8");
                } else {
                    holderEntry.instanceID2 = "00000000";
                }
                log.MakeLog("InstanceID2: " + holderEntry.instanceID2, true);

                holderEntry.offset = readFile.ReadUInt32();
                log.MakeLog("Offset: " + holderEntry.offset, true);
                holderEntry.filesize = readFile.ReadUInt32();
                log.MakeLog("Filesize: " + holderEntry.filesize, true);
                holderEntry.truesize = 0;
                log.MakeLog("Truesize: " + holderEntry.truesize, true);
                holderEntry.compressed = false;

                indexData.Add(holderEntry);

                holderEntry = null;

                var entrynum = 0;

                foreach (indexEntry iEntry in indexData)
                {
                    entrynum++;
                    log.MakeLog("Entry: " + entrynum, true);
                    
                    switch (iEntry.typeID.ToLower())
                    {
                        case "fc6e1f7": linkData.Add(iEntry); log.MakeLog("Has Shpe", true); break;
                    }

                    foreach (typeList type in TypeListings.AllTypesS2) {
                        if (iEntry.typeID == type.typeID) {
                            log.MakeLog("Found " + type.desc, true);
                            typefound = type.desc;
                            break;
                        }
                    }
                    if (typefound == "DIR") {
                        dbpfFile.Seek(chunkOffset + iEntry.offset, SeekOrigin.Begin);
                        log.MakeLog("Entry offset: " + iEntry.offset, true);
                        if ((indexMajorVersion == 7) && indexMinorVersion == 1)
                        {
                            numRecords = iEntry.filesize / 20;
                        } else {
                            numRecords = iEntry.filesize / 16;
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
                                int cFileSize = 0;
                                string cTypeID = "";

                                if (typefound == "XOBJ" || typefound == "XFNC" || typefound == "XFLR" || typefound == "XMOL" || typefound == "XROF"  || typefound == "XTOL"  || typefound == "MMAT" || typefound == "XHTN"){
                                    log.MakeLog("Confirming found " + typefound + " and moving forward.", true);
                                }

                            }

                        }
                        log.MakeLog("Finished looking at entries.", true);
                    }
                }

            }
            log.MakeLog("infovar: " + infovar.Description, true);
            log.MakeLog("infovar: " + infovar.Title, true);
            log.MakeLog("This Package: " + thisPackage.Description, true);
            log.MakeLog("This Package: " + thisPackage.Title, true);
            
            Containers.Containers.allSims2Packages.Add(thisPackage);
        }
    }
}
