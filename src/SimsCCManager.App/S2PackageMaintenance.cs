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
using DBPFReading;
using SSAGlobals;

namespace S2PackageMaintenance {
    
    class S2Packages {
        private uint chunkOffset = 0;
        public string[] packageTypes = { };
		public string[] xmlCatalogSortTypes = { };
		public string[] xmlSubtypes = { };
		public string[] xmlCategoryTypes = { };
		private uint majorVersion;
		private uint minorVersion;
		private string reserved;
		private uint dateCreated;
		private uint dateModified;
		private uint indexMajorVersion;
		private uint indexMinorVersion;
		private uint indexCount;
		private uint indexOffset;
		private uint indexSize;
		private uint holesCount;
		private uint holesOffset;
		private uint holesSize;
		public string title = "";
		public string description = "";
		public string pkgType = "";
		public uint pkgTypeInt = 0;
		public string xmlType = "";
		public string xmlCategory = "";
		public string xmlSubtype = "";
		public string xmlModelName = "";
		public string xmlAge = "";
		public string xmlGender = "";
		public string xmlCatalog = "";
		public ArrayList objectGUID = new ArrayList();
		public string xmlCreator = "";
		public bool isBroken = false;
		public string md5hash = "";
		// majorType is the main type in which this package falls, used for
		// orphan stuff
		// 0 = No type
		// 1 = Mesh
		// 2 = Recolour
		// 3 = BodyShop Mesh
		public uint majorType = 0;
		private string reserved2;
		public ArrayList indexData = new ArrayList();
		public scanTypeList scanList = new scanTypeList();
		public fileHasList fileHas = new fileHasList();
		private ArrayList shpeData = new ArrayList();
		public ArrayList linkData = new ArrayList();
		public ArrayList guidData = new ArrayList();

		private string filename = "";

        //ReadByteStream readByteStream = new ReadByteStream();
        DBPFTypeRead dbpfTypeRead = new DBPFTypeRead();
        LoggingGlobals loggingGlobals = new LoggingGlobals();



        public void s2Packages(){
            
        }

        public void setParms(string filename, uint chunkOffset, string[] packageTypes, string[] xmlCatalogSortTypes, string[] xmlSubTypes, string[] xmlCategoryTypes){
            this.filename = filename;
            this.chunkOffset = chunkOffset;
            this.packageTypes = packageTypes;
            this.xmlCatalogSortTypes = xmlCatalogSortTypes;
            this.xmlSubtypes = xmlSubTypes;
            this.xmlCategoryTypes = xmlCategoryTypes;
        }

        public void s2GetLabel(String file){
            LoggingGlobals logGlobals = new LoggingGlobals();
            var statement = "";
            var IncomingInformation = new ExtractedItems();
            FileStream dbpfFile = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader readFile = new BinaryReader(dbpfFile);
            var temp = new SimsPackage();
            temp.Location = file;
            temp.Game = 2;
            string dbpf = Encoding.ASCII.GetString(readFile.ReadBytes(4));
            int bytes = 4;
            temp.DBPF = dbpf;
            uint major = readFile.ReadUInt32();
            temp.Major = major;
            uint minor = readFile.ReadUInt32();
            temp.Minor = minor;
            string reserved = Encoding.UTF8.GetString(readFile.ReadBytes(12));
            uint dateCreated = readFile.ReadUInt32();
            temp.DateCreated = dateCreated;
            uint dateModified = readFile.ReadUInt32();
            temp.DateModified = dateModified;
            uint indexMajorVersion = readFile.ReadUInt32();
            temp.IndexMajorVersion = indexMajorVersion;
            uint indexCount = readFile.ReadUInt32();
            temp.IndexCount = indexCount;
            uint indexOffset = readFile.ReadUInt32();
            temp.IndexOffset = indexOffset;
            uint indexSize = readFile.ReadUInt32();
            temp.IndexSize = indexSize;
            uint holesCount = readFile.ReadUInt32();
            temp.HolesCount = holesCount;
            uint holesOffset = readFile.ReadUInt32();
            temp.HolesOffset = holesOffset;
            uint holesSize = readFile.ReadUInt32();
            temp.HolesSize = holesSize;
            string reserved2 = Encoding.UTF8.GetString(readFile.ReadBytes(32));
            dbpfFile.Seek(this.chunkOffset + indexOffset, SeekOrigin.Begin);
            statement = "Identified package data:" + Environment.NewLine + "Package: " + temp.Location + Environment.NewLine + "Game: Sims 2" + Environment.NewLine + "DBPF: " + temp.DBPF + Environment.NewLine + "Major: " + temp.Major + Environment.NewLine + "Minor: " + temp.Minor + Environment.NewLine + "Date Created: " + temp.DateCreated + Environment.NewLine + "Date Modified: " + temp.DateModified + Environment.NewLine + "Index Major Version: " + temp.IndexMajorVersion + Environment.NewLine + "Index Count: " + temp.IndexCount + Environment.NewLine + "Index Offset: " + temp.IndexOffset + Environment.NewLine + "Index Size: " + temp.IndexSize + Environment.NewLine + "Holes Count: " + temp.HolesCount + Environment.NewLine + "Holes Offset: " + temp.HolesOffset + Environment.NewLine + "Holes Size: " + temp.HolesSize;
            loggingGlobals.MakeLog(statement, true);
            statement = "Searching for label to apply.";
            loggingGlobals.MakeLog(statement, true);
            for (int i = 0; i < indexCount; i++)
            {
                indexEntry myEntry = new indexEntry();
                myEntry.typeID = readFile.ReadUInt32().ToString("X8");
                statement = "Type ID: " + myEntry.typeID;
                loggingGlobals.MakeLog(statement, true);
                myEntry.groupID = readFile.ReadUInt32().ToString("X8");
                statement = "Group ID: " + myEntry.groupID;
                loggingGlobals.MakeLog(statement, true);
                myEntry.instanceID = readFile.ReadUInt32().ToString("X8");
                statement = "Instance ID: " + myEntry.instanceID;
                loggingGlobals.MakeLog(statement, true);
                myEntry.instanceID2 = "00000000";
                if ((this.indexMajorVersion == 7) && (this.indexMinorVersion == 1)) 
                {
                    myEntry.instanceID2 = readFile.ReadUInt32().ToString("X8");
                    statement = "Instance ID2: " + myEntry.instanceID2;
                    loggingGlobals.MakeLog(statement, true);
                }

                myEntry.offset = readFile.ReadUInt32();
                statement = "Offset: " + myEntry.offset;
                loggingGlobals.MakeLog(statement, true);
                myEntry.filesize = readFile.ReadUInt32();
                statement = "Filesize: " + myEntry.filesize;
                loggingGlobals.MakeLog(statement, true);
                myEntry.truesize = 0;
                myEntry.compressed = false;

                indexData.Add(myEntry);
                myEntry = null;   
            }
            var entrynumalpha = 0;
            statement = "Searching individual entries.";
            loggingGlobals.MakeLog(statement, true);
            foreach (indexEntry iEntry in indexData)
            {
                
                entrynumalpha++;
                uint numRecords;
                string typeID;
                string groupID;
                string instanceID;
                string instanceID2 = "";
                uint myFilesize;
                statement = "Entry " + entrynumalpha + " - TypeID: " + iEntry.typeID;
                loggingGlobals.MakeLog(statement, true);
                
                switch (iEntry.typeID.ToLower())
                {
                    case "fc6e1f7": fileHas.shpe++; linkData.Add(iEntry); break;
                }
                

                if (iEntry.typeID == "E86B1EEF")  // DIR Resource //
                {
                    statement = "Identified DIR resource (typeid " + iEntry.typeID + ").";
                    loggingGlobals.MakeLog(statement, true);
                    dbpfFile.Seek(this.chunkOffset + iEntry.offset, SeekOrigin.Begin);
                    if (indexMajorVersion == 7 && indexMinorVersion == 1)
                    {
                        numRecords = iEntry.filesize / 20;
                        statement = "Number of records:" + numRecords + ".";
                        loggingGlobals.MakeLog(statement, true);
                    }
                    else 
                    {
                        numRecords = iEntry.filesize / 16;
                        statement = "Number of records:" + numRecords + ".";
                        loggingGlobals.MakeLog(statement, true);
                    }

                    var entries = 0;

                    // Loop through getting all the compressed entries
                    for (int i = 0; i < numRecords; i++)
                    {
                        entries++;
                        statement = "Looping through compressed entries.";
                        loggingGlobals.MakeLog(statement, true);
                        typeID = readFile.ReadUInt32().ToString("X8");
                        statement = "Entry #" + entries + " - Type ID:" + typeID;
                        loggingGlobals.MakeLog(statement, true);
                        groupID = readFile.ReadUInt32().ToString("X8");
                        statement = "Entry #" + entries + " - Group ID:" + groupID;
                        loggingGlobals.MakeLog(statement, true);
                        instanceID = readFile.ReadUInt32().ToString("X8");
                        statement = "Entry #" + entries + " - Instance ID:" + instanceID;
                        loggingGlobals.MakeLog(statement, true);
                        if (indexMajorVersion == 7 && indexMinorVersion == 1) {
                            instanceID2 = readFile.ReadUInt32().ToString("X8");
                            statement = "Entry #" + entries + " - Instance ID2:" + instanceID2;
                            loggingGlobals.MakeLog(statement, true);
                        }
                        
                        myFilesize = readFile.ReadUInt32();
                        statement = "Entry #" + entries + " - Filesize:" + myFilesize;
                        loggingGlobals.MakeLog(statement, true);
                        var indexentrynum = 0;
                        
                        foreach (indexEntry idx in indexData) {
                            indexentrynum++;
                            statement = "Now reading index entry idx.";
                            loggingGlobals.MakeLog(statement, true);

                            int cFileSize = 0;
                            string cTypeID = "";

                            switch (idx.typeID)
                            {
                                case "43545353": // Catalog Description - CTSS
                                    //Console.WriteLine("    Catalog Description file");
                                    statement = "Idx Entry #" + indexentrynum + " - found CTSS.";
                                    loggingGlobals.MakeLog(statement, true);
                                    dbpfFile.Seek(this.chunkOffset + idx.offset, SeekOrigin.Begin);

                                        cFileSize = readFile.ReadInt32();
                                        statement = "Idx Entry #" + indexentrynum + " - cfilesize: " + cFileSize;
                                        loggingGlobals.MakeLog(statement, true);
                                        cTypeID = readFile.ReadUInt16().ToString("X4");
                                        statement = "Idx Entry #" + indexentrynum + " - cTypeID: " + cTypeID;
                                        loggingGlobals.MakeLog(statement, true);
                                        // check for the proper QFS type
                                        if (cTypeID == "FB10") 
                                        {                                               
                                            byte[] tempBytes = readFile.ReadBytes(3);
                                            uint cFullSize = DBPFTypeRead.QFSLengthToInt(tempBytes);
                                            statement = "Idx Entry #" + indexentrynum + " - cfullsize: " + cFullSize;
                                            loggingGlobals.MakeLog(statement, true);

                                            ReadByteStream decompressed = new ReadByteStream(DBPFTypeRead.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));                                           

                                            IncomingInformation = dbpfTypeRead.readCTSSchunk(decompressed);
                                            statement = "Idx Entry #" + indexentrynum + " - Chunk Read As: " + IncomingInformation;
                                            loggingGlobals.MakeLog(statement, true);
                                        } 
                                        else 
                                        {
                                            dbpfFile.Seek(this.chunkOffset + idx.offset, SeekOrigin.Begin);
                                            IncomingInformation = dbpfTypeRead.readCTSSchunk(readFile);
                                            statement = "Idx Entry #" + indexentrynum + " - Chunk Read As: " + IncomingInformation;
                                            loggingGlobals.MakeLog(statement, true);
                                        }
                                    break;
                                case "2CB230B8": // XFNC - Fence XML
                                case "4DCADB7E": // XFLR - Floor XML
                                case "CCA8E925": // XOBJ - Object XML
                                case "0C1FE246": // XMOL - Mesh Overlay XML
                                case "ACA8EA06": // XROF - Roof XML
                                case "2C1FD8A1": // Texture Overlay XML
                                case "4C697E5A": // Material Override - MMAT
                                case "8C1580B5": // HairTone XML
                                    //Console.WriteLine("    " + idx.typeID + " file");
                                    statement = "Idx Entry #" + indexentrynum + " - " + idx.typeID;
                                    loggingGlobals.MakeLog(statement, true);
                                    dbpfFile.Seek(this.chunkOffset + idx.offset, SeekOrigin.Begin);

                                    //if (idx.compressed == true)
                                    //{
                                        // Is this always in XML format?

                                    cFileSize = readFile.ReadInt32();
                                    statement = "Idx Entry #" + indexentrynum + " - cfilesize: " + cFileSize;
                                    loggingGlobals.MakeLog(statement, true);
                                    cTypeID = readFile.ReadUInt16().ToString("X4");
                                    statement = "Idx Entry #" + indexentrynum + " - cTypeID: " + cTypeID;
                                    loggingGlobals.MakeLog(statement, true);

                                    // check for the proper QFS type
                                    if (cTypeID == "FB10") 
                                    {
                                        byte[] tempBytes = readFile.ReadBytes(3);
                                        uint cFullSize = DBPFTypeRead.QFSLengthToInt(tempBytes);

                                        // Check for CPF type
                                        string cpfTypeID = readFile.ReadUInt32().ToString("X8");
                                        statement = "Idx Entry #" + indexentrynum + " - cpfTypeID: " + cpfTypeID;
                                        loggingGlobals.MakeLog(statement, true);
                                        if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0"))
                                        {   
                                            // Is an actual CPF file, so we have to decompress it...
                                            IncomingInformation = dbpfTypeRead.readCPFchunk(readFile);
                                            statement = "Idx Entry #" + indexentrynum + " - Chunk Read As: " + IncomingInformation;
                                            loggingGlobals.MakeLog(statement, true);
                                        } 
                                        else 
                                        {
                                            dbpfFile.Seek(this.chunkOffset + idx.offset + 9, SeekOrigin.Begin);
                                            ReadByteStream decompressed = new ReadByteStream(DBPFTypeRead.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));

                                            if (cpfTypeID == "E750E0E2") 
                                            {

                                                // Read first four bytes
                                                cpfTypeID = decompressed.ReadUInt32().ToString("X8");
                                                statement = "Idx Entry #" + indexentrynum + " - cpfTypeID: " + cpfTypeID;
                                                loggingGlobals.MakeLog(statement, true);

                                                //Console.WriteLine("CPF type id: " + cpfTypeID);
                                                if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0")) 
                                                {
                                                    statement = "Idx Entry #" + indexentrynum + " - Identified as an actual CPF file.";
                                                    loggingGlobals.MakeLog(statement, true);
                                                    // Is an actual CPF file, so we have to decompress it...
                                                    IncomingInformation = dbpfTypeRead.readCPFchunk(decompressed);
                                                    statement = "Idx Entry #" + indexentrynum + " - Chunk Read As: " + IncomingInformation;
                                                    loggingGlobals.MakeLog(statement, true);
                                                }

                                            } 
                                            else 
                                            {
                                                statement = "Idx Entry #" + indexentrynum + " - Identified as actually XML.";
                                                loggingGlobals.MakeLog(statement, true);
                                                IncomingInformation = dbpfTypeRead.readXMLchunk(decompressed);
                                                statement = "Idx Entry #" + indexentrynum + " - Chunk Read As: " + IncomingInformation;
                                                loggingGlobals.MakeLog(statement, true);
                                            }
                                        }
                                    } 
                                    else 
                                    {
                                        dbpfFile.Seek(this.chunkOffset + idx.offset, SeekOrigin.Begin);

                                        string cpfTypeID = readFile.ReadUInt32().ToString("X8");
                                        statement = "Idx Entry #" + indexentrynum + " - cpfTypeID: " + cpfTypeID;
                                        loggingGlobals.MakeLog(statement, true);
                                        if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0"))
                                        {
                                            statement = "Idx Entry #" + indexentrynum + " - Identified as an actual CPF file.";
                                            loggingGlobals.MakeLog(statement, true);
                                            // Is an actual CPF file, so we have to decompress it...
                                            IncomingInformation = dbpfTypeRead.readCPFchunk(readFile);
                                            statement = "Idx Entry #" + indexentrynum + " - Chunk Read As: " + IncomingInformation;
                                            loggingGlobals.MakeLog(statement, true);
                                        }

                                        // Actually an uncompressed XML file, so we can use the xmlChunk to 
                                        // process
                                        if  (cpfTypeID == "6D783F3C")
                                        {
                                            statement = "Idx Entry #" + indexentrynum + " - Identified as actually XML.";
                                            loggingGlobals.MakeLog(statement, true);
                                            // Backtrack 4 bytes
                                            dbpfFile.Seek(this.chunkOffset + idx.offset, SeekOrigin.Begin);

                                            // Read entire XML as a normal string
                                            string xmlData = Encoding.UTF8.GetString(readFile.ReadBytes((int)idx.filesize));
                                            IncomingInformation = dbpfTypeRead.readXMLchunk(xmlData);
                                            statement = "Idx Entry #" + indexentrynum + " - Chunk Read As: " + IncomingInformation;
                                            loggingGlobals.MakeLog(statement, true);

                                        }
                                    }
                                    break;
                                case "EBCF3E27":
                                    // Property Set - only read if no other XML/CPF resources
                                    statement = "Idx Entry #" + indexentrynum + " - Giving up and reading Property Set.";
                                    loggingGlobals.MakeLog(statement, true);
                                    if ((fileHas.xhtn == 0) && (fileHas.xobj == 0) && (fileHas.xflr == 0) && (fileHas.xfnc == 0) && (fileHas.xmol == 0) && (fileHas.xngb == 0) && (fileHas.xobj == 0) && (fileHas.xrof == 0) && (fileHas.xstn == 0) && (fileHas.xtol == 0) && (fileHas.aged == 0) )
                                    {
                                        //Console.WriteLine("    Property Set GZPS");
                                        dbpfFile.Seek(this.chunkOffset + idx.offset, SeekOrigin.Begin);

                                        cFileSize = readFile.ReadInt32();
                                        cTypeID = readFile.ReadUInt16().ToString("X4");
                                        // check for the proper QFS type
                                        if (cTypeID == "FB10") 
                                        {
                                            byte[] tempBytes = readFile.ReadBytes(3);
                                            uint cFullSize = DBPFTypeRead.QFSLengthToInt(tempBytes);

                                            ReadByteStream decompressed = new ReadByteStream(DBPFTypeRead.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));

                                            // Read first four bytes
                                            string cpfTypeID = decompressed.ReadUInt32().ToString("X8");
                                            statement = "Idx Entry #" + indexentrynum + " - cpfTypeID: " + cpfTypeID + ".";
                                            loggingGlobals.MakeLog(statement, true);

                                            //Console.WriteLine("CPF type id: " + cpfTypeID);
                                            if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0")) 
                                            {
                                                statement = "Idx Entry #" + indexentrynum + " - Identified as an actual CPF file.";
                                                loggingGlobals.MakeLog(statement, true);
                                                // Is an actual CPF file, so we have to decompress it...
                                                IncomingInformation = dbpfTypeRead.readCPFchunk(decompressed);
                                                statement = "Idx Entry #" + indexentrynum + " - Chunk Read As: " + IncomingInformation;
                                                loggingGlobals.MakeLog(statement, true);
                                            }

                                        } 
                                        else 
                                        {
                                            dbpfFile.Seek(this.chunkOffset + idx.offset, SeekOrigin.Begin);
                                            string cpfTypeID = readFile.ReadUInt32().ToString("X8");
                                            //Console.WriteLine("CPF type id: " + cpfTypeID);
                                            if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0")) 
                                            {
                                                statement = "Idx Entry #" + indexentrynum + " - Identified as an actual CPF file.";
                                                loggingGlobals.MakeLog(statement, true);
                                                // Is an actual CPF file, so we have to decompress it...
                                                IncomingInformation = dbpfTypeRead.readCPFchunk(readFile);
                                                statement = "Idx Entry #" + indexentrynum + " - Chunk Read As: " + IncomingInformation;
                                                loggingGlobals.MakeLog(statement, true);
                                            }

                                        }
                                    }
                                    break;
                            }

                            if ((this.title != "") || (this.xmlType != "")) break;

                        }
                    }
                }
            }

            
            if (IncomingInformation.Type is "Title") {
                statement = "Information type is title. Saving it to temp.Name.";
                loggingGlobals.MakeLog(statement, true);            
                temp.Name = IncomingInformation.Content;
            } else if (IncomingInformation.Type is "Description") {
                statement = "Information type is description. Saving it to temp.Description.";
                loggingGlobals.MakeLog(statement, true); 
                temp.Description = IncomingInformation.Content;
            } else {
                statement = "No title information available.";
                loggingGlobals.MakeLog(statement, true);
            }


            GlobalVariables.allSims2Packages.Add(temp);
            statement = "Adding package to All Sims 2 Packages.";
            loggingGlobals.MakeLog(statement, true);
            temp = null;

        }
    } 
}
