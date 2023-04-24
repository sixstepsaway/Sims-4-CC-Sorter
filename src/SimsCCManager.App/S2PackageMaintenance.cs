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
        
        string packageExtension = "package";

        private uint chunkOffset = 0;
        public string[] packageTypes = { };
		public string[] xmlCatalogSortTypes = { };
		public string[] xmlSubtypes = { };
		public string[] xmlCategoryTypes = { };
		private uint majorVersion = 239083;
		private uint minorVersion = 239083;
		private string reserved = " = 239083 including the equals because it made me laugh <3";
		private uint dateCreated = 239083;
		private uint dateModified = 239083;
		private uint indexMajorVersion = 239083;
		private uint indexMinorVersion = 239083;
		private uint indexCount = 239083;
		private uint indexOffset = 239083;
		private uint indexSize = 239083;
		private string flags = "239083";
		private uint holesCount = 239083;
		private uint holesOffset = 239083;
		private uint holesSize = 239083;
		public string title = "";
		public string description = "";
		public string pkgType = "";
		public uint pkgTypeInt = 0;

		// Values read in from the various XML / CPF types
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

		// shpeData stores lists of SHPE files from 3DIRs
		private ArrayList shpeData = new ArrayList();
		// linkData stores lists of unique SHPE files
		public ArrayList linkData = new ArrayList();

		// objectGUID list is used for processing Maxis objects.package files
		public ArrayList guidData = new ArrayList();

		private string filename = "";
		private bool debugMode = true;
        private bool myDebugMode = true;
        private string statement = "";

        LoggingGlobals loggingGlobals = new LoggingGlobals();
        DBPFTypeRead dbpfTypeRead = new DBPFTypeRead();
        
        public S2Packages()
		{
		}

        public void setParms(string filename, bool debugMode, uint chunkOffset, string[] packageTypes, string[] xmlCatalogSortTypes, string[] xmlSubTypes, string[] xmlCategoryTypes)
		{
			this.filename = filename;
			this.debugMode = debugMode;
			this.chunkOffset = chunkOffset;
			this.packageTypes = packageTypes;
			this.xmlCatalogSortTypes = xmlCatalogSortTypes;
			this.xmlSubtypes = xmlSubTypes;
			this.xmlCategoryTypes = xmlCategoryTypes;
		}

        public void s2GetLabel(String file)
        {
            var record = new SimsPackage();
            var IncomingInformation = new ExtractedItems();
            var AllSimsPackages = new List<SimsPackage>();
            FileInfo packageFile = new FileInfo(file);
            FileStream dbpfFile = new FileStream(packageFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader readFile = new BinaryReader(dbpfFile);
            statement = "----------------------------------------------------";
            loggingGlobals.MakeLog(statement, true); 
            statement = "STARTING NEW PACKAGE FILE";
            loggingGlobals.MakeLog(statement, true);
            statement = "----------------------------------------------------";
            loggingGlobals.MakeLog(statement, true); 
            statement = "Searching package " + packageFile.Name + " for label to apply.";
            loggingGlobals.MakeLog(statement, true); 
            
            statement = "Original Vars:";
            loggingGlobals.MakeLog(statement, true); 
            statement = "Major - " + majorVersion;
            loggingGlobals.MakeLog(statement, true);
            statement = "Minor - " + minorVersion;
            loggingGlobals.MakeLog(statement, true);
            statement = "Reserved - " + reserved;
            loggingGlobals.MakeLog(statement, true);
            statement = "Date Created - " + dateCreated;
            loggingGlobals.MakeLog(statement, true);
            statement = "Date Mod - " + dateModified;
            loggingGlobals.MakeLog(statement, true);
            statement = "I Maj - " + indexMajorVersion;
            loggingGlobals.MakeLog(statement, true);
            statement = "i Min - " + indexMinorVersion;
            loggingGlobals.MakeLog(statement, true);
            statement = "i count - " + indexCount;
            loggingGlobals.MakeLog(statement, true);
            statement = "i offset - " + indexOffset;
            loggingGlobals.MakeLog(statement, true);
            statement = "i size - " + indexSize;
            loggingGlobals.MakeLog(statement, true);
            statement = "flags - " + flags;
            loggingGlobals.MakeLog(statement, true);
            statement = "holes count - " + holesCount;
            loggingGlobals.MakeLog(statement, true);
            statement = "h offset - " + holesOffset;
            loggingGlobals.MakeLog(statement, true);
            statement = "h size - " + holesSize;
            loggingGlobals.MakeLog(statement, true);
            /* Reading through all the bytes to get to where we need to be. */

            record.Game = 2;
        
            //checks it's a package
            record.DBPF = Encoding.ASCII.GetString(readFile.ReadBytes(4));
            int bytes = 4;
            statement = "Read first four bytes - " + record.DBPF;
            loggingGlobals.MakeLog(statement, true); 

            //gets the major version
            majorVersion = readFile.ReadUInt32();
            record.Major = majorVersion;
            statement = "Reading major - " + record.Major;
            loggingGlobals.MakeLog(statement, true); 

            //gets the minor version
            minorVersion = readFile.ReadUInt32();
            record.Minor = minorVersion;
            statement = "Reading minor - " + record.Minor;
            loggingGlobals.MakeLog(statement, true); 

            //gets the flags (unused)
            flags = Encoding.UTF8.GetString(readFile.ReadBytes(12));
            statement = "Reading flags - " + flags;
            loggingGlobals.MakeLog(statement, true); 

            //gets the date of creation (usually 0)
            dateCreated = readFile.ReadUInt32();
            record.DateCreated = dateCreated;
            statement = "Reading creation date - " + record.DateCreated;
            loggingGlobals.MakeLog(statement, true); 

            //gets the modification date (usually 0)
            dateModified = readFile.ReadUInt32();
            record.DateModified = dateModified;
            statement = "Reading modification date - " + record.DateModified;
            loggingGlobals.MakeLog(statement, true); 

            //gets the index major version
            indexMajorVersion = readFile.ReadUInt32();
            record.IndexMajorVersion = indexMajorVersion;
            statement = "Reading index major version - " + record.IndexMajorVersion;
            loggingGlobals.MakeLog(statement, true); 

            //gets the count of index entries, so you know how many to read through
            indexCount = readFile.ReadUInt32();
            record.IndexCount = indexCount;
            statement = "Reading index count - " + record.IndexCount;
            loggingGlobals.MakeLog(statement, true); 

            //gets the index offset (aka location)
            indexOffset = readFile.ReadUInt32();
            record.IndexOffset = indexOffset;
            statement = "Reading index offset - " + record.IndexOffset;
            loggingGlobals.MakeLog(statement, true);

            //gets the index size
            indexSize = readFile.ReadUInt32();
            record.IndexSize = indexSize;
            statement = "Reading index size - " + record.IndexSize;
            loggingGlobals.MakeLog(statement, true); 

            //gets the holes count (usually 0)
            holesCount = readFile.ReadUInt32();
            record.HolesCount = holesCount;
            statement = "Reading holes count - " + record.HolesCount;
            loggingGlobals.MakeLog(statement, true); 

            //gets the holes offset (usually 0)
            holesOffset = readFile.ReadUInt32();
            record.HolesOffset = holesOffset;
            statement = "Reading holes offset - " + record.HolesOffset;
            loggingGlobals.MakeLog(statement, true); 

            //gets the holes size (usually 0)
            holesSize = readFile.ReadUInt32();
            record.HolesSize = holesSize;
            statement = "Reading holes size - " + record.HolesSize;
            loggingGlobals.MakeLog(statement, true); 

            //gets the minor version of the index
            indexMinorVersion = readFile.ReadUInt32() -1;
            record.IndexMinorVersion = indexMinorVersion;
            statement = "Reading index minor version - " + record.IndexMinorVersion;
            loggingGlobals.MakeLog(statement, true);

            //gets the 32 bytes of reserved matter
            reserved2 = Encoding.UTF8.GetString(readFile.ReadBytes(32));

            statement = "Chunk offset:  " + this.chunkOffset;
            loggingGlobals.MakeLog(statement, true); 

            //scoots through to where the entries begin
            dbpfFile.Seek(this.chunkOffset + indexOffset, SeekOrigin.Begin);
            
            statement = "Reading index entries.";
            loggingGlobals.MakeLog(statement, true); 
            for (int i = 0; i < indexCount; i++)
            {
                indexEntry myEntry = new indexEntry();
                //statement = "Entry " + i + ": Entry Instance ID 2 at this point: " + myEntry.instanceID2;
                //loggingGlobals.MakeLog(statement, true);
                myEntry.typeID = readFile.ReadUInt32().ToString("X8");
                statement = "Entry " + i + ": Type ID: " + myEntry.typeID;
                loggingGlobals.MakeLog(statement, true);
                myEntry.groupID = readFile.ReadUInt32().ToString("X8");
                statement = "Entry " + i + ": Type ID: " + myEntry.groupID;
                loggingGlobals.MakeLog(statement, true);
                myEntry.instanceID = readFile.ReadUInt32().ToString("X8");
                statement = "Entry " + i + ": Instance ID: " + myEntry.instanceID;
                loggingGlobals.MakeLog(statement, true);                
                //statement = "Record's index major version: " + record.IndexMajorVersion;
                //loggingGlobals.MakeLog(statement, true);
                //statement = "Record's index minor version: " + record.IndexMinorVersion;
                //loggingGlobals.MakeLog(statement, true);
                //statement = "This's index major version: " + this.indexMajorVersion;
                //loggingGlobals.MakeLog(statement, true);
                //statement = "This's index minor version: " + this.indexMinorVersion;
                //loggingGlobals.MakeLog(statement, true);
                
                //statement = "Entry " + i + ": Entry Instance ID 2 after being set to 00000000: " + myEntry.instanceID2;
                //loggingGlobals.MakeLog(statement, true);

                if ((this.indexMajorVersion == 7) && (this.indexMinorVersion == 1))
                {
                    statement = "Minor and major versions match.";
                    loggingGlobals.MakeLog(statement, true);
                    myEntry.instanceID2 = readFile.ReadUInt32().ToString("X8");
                } else {
                    myEntry.instanceID2 = "00000000";
                }

                statement = "Entry " + i + ": Entry Instance ID 2: " + myEntry.instanceID2;
                loggingGlobals.MakeLog(statement, true);
                myEntry.offset = readFile.ReadUInt32();
                statement = "Entry " + i + "offset: " + myEntry.offset;
                loggingGlobals.MakeLog(statement, true);
                myEntry.filesize = readFile.ReadUInt32();
                statement = "Entry " + i + "  filesize: " + myEntry.filesize;
                loggingGlobals.MakeLog(statement, true);
                myEntry.truesize = 0;
                statement = "Entry " + i + " truesize: " + myEntry.truesize;
                loggingGlobals.MakeLog(statement, true);
                myEntry.compressed = false;

                indexData.Add(myEntry);
                statement = "Added entry to indexData.";
                loggingGlobals.MakeLog(statement, true);
                myEntry = null;   
            }
            var entrynum = 0;
            foreach (indexEntry iEntry in indexData)
            {
                statement = "Searching entry" + entrynum;
                loggingGlobals.MakeLog(statement, true);
                entrynum++;
                uint numRecords;
                string typeID;
                string groupID;
                string instanceID;
                string instanceID2 = "";
                uint myFilesize;

                switch (iEntry.typeID.ToLower())
                {
                    case "fc6e1f7": fileHas.shpe++; linkData.Add(iEntry); statement = "File has shpe."; loggingGlobals.MakeLog(statement, true); break;
                }

                if (iEntry.typeID == "E86B1EEF")  // DIR Resource
                {
                    dbpfFile.Seek(this.chunkOffset + iEntry.offset, SeekOrigin.Begin);
                    if (indexMajorVersion == 7 && indexMinorVersion == 1)
                    {
                        numRecords = iEntry.filesize / 20;
                        statement = "Index Major is 7, minor is 1. Setting numRecords to: " + numRecords;
                        loggingGlobals.MakeLog(statement, true);
                    }
                    else 
                    {
                        numRecords = iEntry.filesize / 16;
                        statement = "Setting numRecords to: " + numRecords;
                        loggingGlobals.MakeLog(statement, true);
                    }

                    // Loop through getting all the compressed entries
                    for (int i = 0; i < numRecords; i++)
                    {   
                        statement = "Getting compressed entry #" + i;
                        loggingGlobals.MakeLog(statement, true);
                        typeID = readFile.ReadUInt32().ToString("X8");
                        statement = "Compressed entry " + i + " typeID is: " + typeID;
                        loggingGlobals.MakeLog(statement, true);
                        groupID = readFile.ReadUInt32().ToString("X8");
                        statement = "Compressed entry " + i + " groupID is: " + groupID;
                        loggingGlobals.MakeLog(statement, true);                        
                        instanceID = readFile.ReadUInt32().ToString("X8");
                        statement = "Compressed entry " + i + " instanceID is: " + instanceID;
                        loggingGlobals.MakeLog(statement, true);                          
                        if (indexMajorVersion == 7 && indexMinorVersion == 1) {
                            instanceID2 = readFile.ReadUInt32().ToString("X8");
                            statement = "Compressed entry " + i + "index major is 7, minor is 1, setting InstanceID2 to " + instanceID2;
                            loggingGlobals.MakeLog(statement, true);
                        }
                        myFilesize = readFile.ReadUInt32();
                        statement = "Compressed entry " + i + "filesize is " + myFilesize;
                        loggingGlobals.MakeLog(statement, true);
                        int idxc = 0;
                        foreach (indexEntry idx in indexData) 
                        {   idxc++;
                            statement = "Now reading IDX " + idxc;
                            loggingGlobals.MakeLog(statement, true);

                            int cFileSize = 0;
                            string cTypeID = "";

                            switch (idx.typeID)
                            {
                                case "43545353": // Catalog Description - CTSS
                                    //Console.WriteLine("    Catalog Description file");
                                    statement = "IDX #" + idxc + " catalog description file found.";
                                    loggingGlobals.MakeLog(statement, true);
                                    dbpfFile.Seek(this.chunkOffset + idx.offset, SeekOrigin.Begin);

                                        cFileSize = readFile.ReadInt32();
                                        cTypeID = readFile.ReadUInt16().ToString("X4");
                                        // check for the proper QFS type
                                        if (cTypeID == "FB10") 
                                        {
                                            byte[] tempBytes = readFile.ReadBytes(3);
                                            uint cFullSize = DBPFTypeRead.QFSLengthToInt(tempBytes);

                                            ReadByteStream decompressed = new ReadByteStream(DBPFTypeRead.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));                                           

                                            IncomingInformation = dbpfTypeRead.readCTSSchunk(decompressed);
                                            statement = "IDX #" + idxc + " decompressed and read as " + IncomingInformation;
                                            loggingGlobals.MakeLog(statement, true);
                                        } 
                                        else 
                                        {
                                            dbpfFile.Seek(this.chunkOffset + idx.offset, SeekOrigin.Begin);
                                            IncomingInformation = dbpfTypeRead.readCTSSchunk(readFile);
                                            statement = "IDX #" + idxc + " decompressed and read as " + IncomingInformation;
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
                                    statement = "IDX #" + idxc + " found typeID of " + idx.typeID;
                                    loggingGlobals.MakeLog(statement, true);
                                    dbpfFile.Seek(this.chunkOffset + idx.offset, SeekOrigin.Begin);
                                    statement = "IDX #" + idxc + " seeking through idx entry.";
                                    loggingGlobals.MakeLog(statement, true);

                                    //if (idx.compressed == true)
                                    //{
                                        // Is this always in XML format?
                                    cFileSize = readFile.ReadInt32();
                                    statement = "IDX #" + idxc + " cfilesize = " + cFileSize;
                                    loggingGlobals.MakeLog(statement, true);
                                    cTypeID = readFile.ReadUInt16().ToString("X4");
                                    statement = "IDX #" + idxc + " cTypeID = " + ctypeID;
                                    loggingGlobals.MakeLog(statement, true);
                                    // check for the proper QFS type
                                    if (cTypeID == "FB10") 
                                    {
                                        statement = "IDX #" + idxc + " found QFS type of " + cTypeID;
                                        loggingGlobals.MakeLog(statement, true);
                                        byte[] tempBytes = readFile.ReadBytes(3);
                                        statement = "IDX #" + idxc + " read some temp bytes of " + tempBytes;
                                        loggingGlobals.MakeLog(statement, true);
                                        uint cFullSize = DBPFTypeRead.QFSLengthToInt(tempBytes);
                                        statement = "IDX #" + idxc + " got cfullsize of " + cFullSize;
                                        loggingGlobals.MakeLog(statement, true);

                                        // Check for CPF type
                                        string cpfTypeID = readFile.ReadUInt32().ToString("X8");
                                        statement = "IDX #" + idxc + " cpf type is " + cpfTypeID;
                                        loggingGlobals.MakeLog(statement, true);
                                        if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0"))
                                        {
                                            // Is an actual CPF file, so we have to decompress it...
                                            statement = "IDX #" + idxc + " read as actual CPF type.";
                                            loggingGlobals.MakeLog(statement, true);
                                            IncomingInformation = dbpfTypeRead.readCPFchunk(readFile);
                                            statement = "IDX #" + idxc + " decompressed and read as " + IncomingInformation;
                                            loggingGlobals.MakeLog(statement, true);
                                        } 
                                        else 
                                        {
                                            dbpfFile.Seek(this.chunkOffset + idx.offset + 9, SeekOrigin.Begin);
                                            statement = "IDX #" + idxc + " seeking through again.";
                                            loggingGlobals.MakeLog(statement, true);
                                            ReadByteStream decompressed = new ReadByteStream(DBPFTypeRead.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));
                                            statement = "IDX #" + idxc + " uncompressing bytes.";
                                            loggingGlobals.MakeLog(statement, true);

                                            if (cpfTypeID == "E750E0E2") 
                                            {
                                                statement = "IDX #" + idxc + " identified cpftypeid " + cpfTypeID;
                                                loggingGlobals.MakeLog(statement, true);

                                                // Read first four bytes
                                                cpfTypeID = decompressed.ReadUInt32().ToString("X8");
                                                statement = "IDX #" + idxc + " read first four bites as " + cpfTypeID;
                                                loggingGlobals.MakeLog(statement, true);

                                                //Console.WriteLine("CPF type id: " + cpfTypeID);
                                                if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0")) 
                                                {
                                                    statement = "IDX #" + idxc + " identified as actual CPF file."
                                                    loggingGlobals.MakeLog(statement, true);
                                                    // Is an actual CPF file, so we have to decompress it...
                                                    IncomingInformation = dbpfTypeRead.readCPFchunk(decompressed);
                                                    statement = "IDX #" + idxc + " decompressed and read as " + IncomingInformation;
                                                    loggingGlobals.MakeLog(statement, true);
                                                }

                                            } 
                                            else 
                                            {
                                                IncomingInformation = dbpfTypeRead.readXMLchunk(decompressed);
                                                statement = "IDX #" + idxc + " decompressed and read as " + IncomingInformation;
                                                loggingGlobals.MakeLog(statement, true);
                                            }
                                        }
                                    } 
                                    else 
                                    {
                                        dbpfFile.Seek(this.chunkOffset + idx.offset, SeekOrigin.Begin);
                                        statement = "IDX #" + idxc + " seeking through.";
                                        loggingGlobals.MakeLog(statement, true);

                                        string cpfTypeID = readFile.ReadUInt32().ToString("X8");
                                        statement = "IDX #" + idxc + " got cpftypeid as " + cpfTypeID;
                                        loggingGlobals.MakeLog(statement, true);
                                        //Console.WriteLine("CPF type id: " + cpfTypeID);
                                        if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0"))
                                        {
                                            statement = "IDX #" + idxc + " identified as cpf.";
                                            loggingGlobals.MakeLog(statement, true);
                                            // Is an actual CPF file, so we have to decompress it...
                                            IncomingInformation = dbpfTypeRead.readCPFchunk(readFile);
                                            statement = "IDX #" + idxc + " decompressed and read as " + IncomingInformation;
                                            loggingGlobals.MakeLog(statement, true);
                                        }

                                        // Actually an uncompressed XML file, so we can use the xmlChunk to 
                                        // process
                                        if  (cpfTypeID == "6D783F3C")
                                        {
                                            statement = "IDX #" + idxc + " discovered this is an XML file.";
                                            loggingGlobals.MakeLog(statement, true);
                                            // Backtrack 4 bytes
                                            dbpfFile.Seek(this.chunkOffset + idx.offset, SeekOrigin.Begin);
                                            statement = "IDX #" + idxc + " backtracked 4 bytes.";
                                            loggingGlobals.MakeLog(statement, true);

                                            // Read entire XML as a normal string
                                            string xmlData = Encoding.UTF8.GetString(readFile.ReadBytes((int)idx.filesize));
                                            statement = "IDX #" + idxc + " converted to normal string.";
                                            loggingGlobals.MakeLog(statement, true);
                                            IncomingInformation = dbpfTypeRead.readXMLchunk(xmlData);
                                            statement = "IDX #" + idxc + " decompressed and read as " + IncomingInformation;
                                            loggingGlobals.MakeLog(statement, true);

                                        }
                                    }
                                    break;
                                case "EBCF3E27":
                                    // Property Set - only read if no other XML/CPF resources
                                    if ((fileHas.xhtn == 0) && (fileHas.xobj == 0) && (fileHas.xflr == 0) && (fileHas.xfnc == 0) && (fileHas.xmol == 0) && (fileHas.xngb == 0) && (fileHas.xobj == 0) && (fileHas.xrof == 0) && (fileHas.xstn == 0) && (fileHas.xtol == 0) && (fileHas.aged == 0) )
                                    {
                                        statement = "IDX #" + idxc + " idx has no other resources, reading Property Set.";
                                        loggingGlobals.MakeLog(statement, true);
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

                                            //Console.WriteLine("CPF type id: " + cpfTypeID);
                                            if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0")) 
                                            {
                                                // Is an actual CPF file, so we have to decompress it...
                                                IncomingInformation = dbpfTypeRead.readCPFchunk(decompressed);
                                            }

                                        } 
                                        else 
                                        {
                                            dbpfFile.Seek(this.chunkOffset + idx.offset, SeekOrigin.Begin);
                                            string cpfTypeID = readFile.ReadUInt32().ToString("X8");
                                            //Console.WriteLine("CPF type id: " + cpfTypeID);
                                            if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0")) 
                                            {
                                                // Is an actual CPF file, so we have to decompress it...
                                                IncomingInformation = dbpfTypeRead.readCPFchunk(readFile);
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
                record.Name = IncomingInformation.Content;
            } else if (IncomingInformation.Type is "Description") {
                record.Description = IncomingInformation.Content;
            } else {
                //
            }

            record.Location = file;
            
            AllSimsPackages.Add(record);
            record = null;

            foreach (SimsPackage package in AllSimsPackages) {
                statement = package.Location;
                loggingGlobals.MakeLog(statement, false);
                statement = package.Name;
                loggingGlobals.MakeLog(statement, false);
                statement = package.Description;
                loggingGlobals.MakeLog(statement, false);
            }     

        }
    } 
}
