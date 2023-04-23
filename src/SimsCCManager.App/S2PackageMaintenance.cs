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
            uint indexMinorVersion = readFile.ReadUInt32() -1;
            temp.indexMinorVersion = indexMinorVersion.ToString();
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

    public class ExtractedItems
    {
        public string Type;
        public string Content;

    }

    public class scanTypeList
    {
        public bool CTSS = true;
        public bool STR = true;
        public bool XOBJ = true;
        public bool XFLR = true;
    }

    public class fileHasList
    {
        public uint uniqueFiles = 0;
        public uint gmdc = 0;
        public uint gmnd = 0;
        public uint xobj = 0;
        public uint mmat = 0;
        public uint txtr = 0;
        public uint txmt = 0;
        public uint strlist = 0;
        public uint shpe = 0;
        public uint ctss = 0;
        public uint threedir = 0;
        public uint binx = 0;
        public uint img = 0;
        public uint xtol = 0;
        public uint clst = 0;
        public uint xhtn = 0;
        public uint gzps = 0;
        public uint lxnr = 0;
        public uint aged = 0;
        public uint xflr = 0;
        public uint xmol = 0;
        public uint nref = 0;
        public uint objd = 0;
        public uint objf = 0;
        public uint bcon = 0;
        public uint bhav = 0;
        public uint glob = 0;
        public uint ttab = 0;
        public uint slot = 0;
        public uint xstn = 0;
        public uint coll = 0;
        public uint jpeg = 0;
        public uint creg = 0;
        public uint cres = 0;
        public uint matshad = 0;
        public uint trcn = 0;
        public uint xrof = 0;
        public uint xfnc = 0;
        public uint anim = 0;
        public uint xngb = 0;
        public uint fwav = 0;
        public uint ttas = 0;
        public uint lght = 0;
        public uint xmto = 0;
        public uint hous = 0;
        public uint rtex = 0;
        public uint sdna = 0;
        public uint scor = 0;
        public uint tssg = 0;
        public uint lttx = 0;
    }

    public class ReadByteStream {
        
        private int currOffset = 0;
        private byte[] byteStream;

        public ReadByteStream(byte[] inputBytes) 
        {
            byteStream = inputBytes;
        }
        
        public int Offset {
            get{ return currOffset; }
            set{ currOffset = value; }
        }

        public void SkipAhead(int numToSkip) {
            this.Offset += numToSkip;
        }

        public string GetNullString() {
            string result = "";
            char c;
            for (int i = 0; i < byteStream.Length; i++) {
                if ((c = (char)byteStream[currOffset]) == 0) { currOffset++; break; }
                result += c.ToString();
                currOffset++;
            }

            return result;
        }

        public byte ReadByte(){
            byte result = new byte();
            if (currOffset > byteStream.Length) return result;
            result = byteStream[currOffset];
            currOffset++; 
            return result;
        }

        public byte[] ReadBytes(uint count) {
            byte[] result = new byte [count];
            for (int i = 0; i < count; i++)
            {
                result[i] = byteStream[currOffset];
                currOffset++;
                if (currOffset > byteStream.Length) return result;
            }
            return result;
        }

        public uint ReadUInt32() {
            uint power = 1;
            uint result = 0;

            for (int i = 0; i < 4; i++){
                if (currOffset > byteStream.Length) return result;
                result += (byteStream[currOffset] * power);
                power = power * 256;
                currOffset++;
            }
            return result;
        }

        public uint ReadUInt16(){
            uint power = 1;
            uint result = 0;

            for (int i = 0; i < 2; i++) {
                if (currOffset > byteStream.Length) return result;
                result += (byteStream[currOffset] * power);
                power = power * 256;
                currOffset++;
            }
            return result;
        }

        public bool ReadBoolean(){
            bool result = false;
            if (currOffset > byteStream.Length) return result;

            byte temp = byteStream[currOffset];
            currOffset++;

            if (temp == 1) {result = true;}
            else {result = false;}

            return result;
        }

        public string ReadString() {
            string result = "";
            //Get the first byte containing the string length
            if (currOffset > byteStream.Length) return result;

            byte stringLength = byteStream[currOffset];
            currOffset++;

            //Does the string start with 01?
            if (byteStream[currOffset] == 1) currOffset++;

            //get the length of the string
            result = Encoding.UTF8.GetString(ReadBytes((uint)stringLength));
            return result;
        }

        public byte[] GetEntireStream()
        {
            return byteStream;
        }
    }

    public class DBPFTypeRead 
    {

        private bool debugMode = false;
        public ArrayList objectGUID = new ArrayList();
        public ArrayList guidData = new ArrayList();

        // Values read in from the various XML / CPF types
        public string xmlType = "";
        public string xmlCategory = "";
        public string xmlSubtype = "";
        public string title = "";
        public string description = "";
        
        public string xmlModelName = "";
        public string xmlAge = "";
        public string xmlGender = "";
        public string xmlCatalog = "";
        public string[] packageTypes = { };
        public string[] xmlCatalogSortTypes = { };
		public string[] xmlSubtypes = { };
		public string[] xmlCategoryTypes = { };
        public string xmlCreator = "";
        public uint pkgTypeInt = 0;
        

        public static uint QFSLengthToInt(Byte[] data)
        {
            // Converts a 3 byte length to a uint
            uint power = 1;
            uint result = 0;
            for (int i = data.Length; i > 0; i--)
            {
                result += (data[i-1] * power);
                power = power * 256;
            }

            return result;
        }

        public static Byte[] Uncompress(Byte[] data, uint targetSize, int offset)
		{
			return null;
            
            /*


            Byte[] uncdata = null;
			int index = offset;			

			try 
			{
				uncdata = new Byte[targetSize];
			} 
			catch(Exception) 
			{
				uncdata = new Byte[0];
			}
			
			int uncindex = 0;
			int plaincount = 0;
			int copycount = 0;
			int copyoffset = 0;
			Byte cc = 0;
			Byte cc1 = 0;
			Byte cc2 = 0;
			Byte cc3 = 0;
			int source;
			
			try 
			{
				while ((index<data.Length) && (data[index] < 0xfc))
				{
					cc = data[index++];
				
					if ((cc&0x80)==0)
					{
						cc1 = data[index++];
						plaincount = (cc & 0x03);
						copycount = ((cc & 0x1C) >> 2) + 3;
						copyoffset = ((cc & 0x60) << 3) + cc1 +1;
					} 
					else if ((cc&0x40)==0)
					{
						cc1 = data[index++];
						cc2 = data[index++];
						plaincount = (cc1 & 0xC0) >> 6 ; 
						copycount = (cc & 0x3F) + 4 ;
						copyoffset = ((cc1 & 0x3F) << 8) + cc2 +1;							
					} 
					else if ((cc&0x20)==0)
					{
						cc1 = data[index++];
						cc2 = data[index++];
						cc3 = data[index++];
						plaincount = (cc & 0x03);
						copycount = ((cc & 0x0C) << 6) + cc3 + 5;
						copyoffset = ((cc & 0x10) << 12) + (cc1 << 8) + cc2 +1;
					} 
					else 
					{									
						plaincount = (cc - 0xDF) << 2; 
						copycount = 0;
						copyoffset = 0;				
					}

					for (int i=0; i<plaincount; i++) uncdata[uncindex++] = data[index++];

					source = uncindex - copyoffset;	
					for (int i=0; i<copycount; i++) uncdata[uncindex++] = uncdata[source++];
				}//while
			} //try
			catch(Exception ex)
			{
				//Helper.ExceptionMessage("", ex);
				throw ex;
			} 
			

			if (index<data.Length) 
			{
				plaincount = (data[index++] & 0x03);
				for (int i=0; i<plaincount; i++) 
				{
					if (uncindex>=uncdata.Length) break;
					uncdata[uncindex++] = data[index++];
				}
			}
			return uncdata;*/
		}

        public void readOBJDchunk(BinaryReader readFile)
		{

			readFile.ReadBytes(64); // Filename - 64 bytes
			uint version = readFile.ReadUInt32();
			readFile.ReadUInt16(); // Initial Stack Size
			readFile.ReadUInt16(); // Default Wall Adjacent Flags
			readFile.ReadUInt16(); // Default Placement Flags
			readFile.ReadUInt16(); // Default Wall Placement Flags
			readFile.ReadUInt16(); // Default Allowed Height Flags
			readFile.ReadUInt16(); // Interaction Table ID
			readFile.ReadUInt16(); // Interaction Group
			uint objectType = readFile.ReadUInt16(); // Type of Object
			uint masterTileMasterId = readFile.ReadUInt16();
			uint masterTileSubIndex = readFile.ReadUInt16();

			// Only check further if this is a Master ID or single id
			if ((masterTileSubIndex == 65535) || (masterTileMasterId == 0))
			{
				readFile.ReadUInt16(); // Use Default Placement Flags
				readFile.ReadUInt16(); // Look at Score
				uint objectGUID = readFile.ReadUInt32();
				this.objectGUID.Add(objectGUID.ToString("X8"));
				//this.objectGUID = objectGUID.ToString("X8");
				this.guidData.Add(this.objectGUID);
				// Skip stuff we don't need
				readFile.ReadBytes(46);
				uint roomSortFlag = readFile.ReadUInt16();
				int[] functionSortFlag = new int[1];
				functionSortFlag[0] = (int)readFile.ReadUInt16();
				BitArray functionSortFlags = new BitArray(functionSortFlag);

				// No function sort, check Build Mode Sort
				if (functionSortFlag[0] == 0) 
				{
					// Skip until we hit the Build Mode sort and EP
					readFile.ReadBytes(46);
					uint expansionFlag = readFile.ReadUInt16();

					readFile.ReadBytes(8);
					uint buildModeType = readFile.ReadUInt16();
					string originalGUID = readFile.ReadUInt32().ToString("X8");
					string objectModelGUID = readFile.ReadUInt32().ToString("X8");
					uint buildModeSubsort = readFile.ReadUInt16();

					switch (buildModeType)
					{
						case 1: this.xmlSubtype = this.xmlSubtypes[1]; break;
						case 2: this.xmlSubtype = this.xmlSubtypes[2]; break;
						case 3: this.xmlSubtype = this.xmlSubtypes[3]; break;
						case 4: this.xmlSubtype = this.xmlSubtypes[4]; break;
						case 5: this.xmlSubtype = this.xmlSubtypes[5]; break;
						case 6: this.xmlSubtype = this.xmlSubtypes[6]; break;
						case 7: this.xmlSubtype = this.xmlSubtypes[7]; break;
						case 8: 
						{
							switch (buildModeSubsort)
							{
								case 1: this.xmlSubtype = this.xmlSubtypes[1]; break;
								case 2: this.xmlSubtype = this.xmlSubtypes[2]; break;
								case 4: this.xmlSubtype = this.xmlSubtypes[2]; break;
								case 8: this.xmlSubtype = this.xmlSubtypes[1]; break;
								case 16: this.xmlSubtype = this.xmlSubtypes[1]; break;
								default: this.xmlSubtype = this.xmlSubtypes[0]; break;
							}
							break;
						}
						//this.xmlCatalog = this.xmlCategoryTypes[2] + " -> " + this.xmlSubtype;
					}
					if (this.xmlSubtype != "") this.xmlCatalog = this.xmlCategoryTypes[2] + " -> " + this.xmlSubtype;
					//if (this.xmlSubtype != "") this.xmlCatalog += this.xmlSubtype + " ";

					// Also set the xmlCategory to Object
					if (((masterTileSubIndex == 0) || (masterTileSubIndex == 65535)) && (buildModeSubsort == 0) && (buildModeType == 0) && (functionSortFlag[0] == 0))
					{
						switch (objectType)
						{
							case 7: this.pkgTypeInt = 2; break;
							case 9: this.xmlCategory = this.xmlCategoryTypes[2]; this.xmlSubtype = this.xmlSubtypes[2]; break;
							case 10: this.xmlCategory = this.xmlCategoryTypes[2]; break;
							case 20: this.xmlCategory = this.xmlCategoryTypes[5]; break;
							case 14: this.xmlCategory = this.xmlCategoryTypes[5]; break;
							default: break;
						}
					} 
					else 
					{
						this.xmlCategory = this.xmlCategoryTypes[2];
					}
				} 
				else 
				{
					// Set the xmlCategory to Object
					if (this.xmlCategory == "") this.xmlCategory = this.xmlCategoryTypes[1];

					// Also, get the catalog placement for this object
					readFile.ReadBytes(46);
					uint expansionFlag = readFile.ReadUInt16();

					readFile.ReadBytes(8);
					uint buildModeType = readFile.ReadUInt16();
					string originalGUID = readFile.ReadUInt32().ToString("X8");
					string objectModelGUID = readFile.ReadUInt32().ToString("X8");
					uint buildModeSubsort = readFile.ReadUInt16();
					readFile.ReadBytes(38);
					uint functionSubsort = readFile.ReadUInt16();
					Console.WriteLine(functionSubsort);

					if (functionSortFlags[0] == true) 
					{
						this.xmlSubtype = this.xmlSubtypes[8]; 
						switch (functionSubsort)
						{
							case 1: this.xmlCatalog = this.xmlCatalogSortTypes[1]; break;
							case 2: this.xmlCatalog = this.xmlCatalogSortTypes[2]; break;
							case 4: this.xmlCatalog = this.xmlCatalogSortTypes[3]; break;
							case 8: this.xmlCatalog = this.xmlCatalogSortTypes[4]; break;
							case 16: this.xmlCatalog = this.xmlCatalogSortTypes[5]; break;
							case 32: this.xmlCatalog = this.xmlCatalogSortTypes[6]; break;
							case 64: this.xmlCatalog = this.xmlCatalogSortTypes[7]; break;
							case 128: this.xmlCatalog = this.xmlCatalogSortTypes[8]; break;
						}
					}
					if (functionSortFlags[1] == true) 
					{
						this.xmlSubtype = this.xmlSubtypes[9]; 
						switch (functionSubsort)
						{
							case 1: this.xmlCatalog = this.xmlCatalogSortTypes[9]; break;
							case 2: this.xmlCatalog = this.xmlCatalogSortTypes[10]; break;
							case 4: this.xmlCatalog = this.xmlCatalogSortTypes[11]; break;
							case 8: this.xmlCatalog = this.xmlCatalogSortTypes[12]; break;
							case 16: this.xmlCatalog = this.xmlCatalogSortTypes[13]; break;
							case 32: this.xmlCatalog = this.xmlCatalogSortTypes[14]; break;
							case 64: this.xmlCatalog = this.xmlCatalogSortTypes[15]; break;
							case 128: this.xmlCatalog = this.xmlCatalogSortTypes[8]; break;
						}
					}
					if (functionSortFlags[2] == true) 
					{ 
						this.xmlSubtype = this.xmlSubtypes[10]; 
						switch (functionSubsort)
						{
							case 1: this.xmlCatalog = this.xmlCatalogSortTypes[16]; break;
							case 2: this.xmlCatalog = this.xmlCatalogSortTypes[17]; break;
							case 4: this.xmlCatalog = this.xmlCatalogSortTypes[18]; break;
							case 8: this.xmlCatalog = this.xmlCatalogSortTypes[19]; break;
							case 16: this.xmlCatalog = this.xmlCatalogSortTypes[20]; break;
							case 32: this.xmlCatalog = this.xmlCatalogSortTypes[21]; break;
							case 64: this.xmlCatalog = this.xmlCatalogSortTypes[22]; break;
							case 128: this.xmlCatalog = this.xmlCatalogSortTypes[8]; break;
						}

					}
					if (functionSortFlags[3] == true) 
					{
						this.xmlSubtype = this.xmlSubtypes[11]; 
						switch (functionSubsort)
						{
							case 1: this.xmlCatalog = this.xmlCatalogSortTypes[23]; break;
							case 2: this.xmlCatalog = this.xmlCatalogSortTypes[24]; break;
							case 4: this.xmlCatalog = this.xmlCatalogSortTypes[25]; break;
							case 8: this.xmlCatalog = this.xmlCatalogSortTypes[26]; break;
							case 16: this.xmlCatalog = this.xmlCatalogSortTypes[27]; break;
							case 32: this.xmlCatalog = this.xmlCatalogSortTypes[28]; break;
							case 64: this.xmlCatalog = this.xmlCatalogSortTypes[29]; break;
							case 128: this.xmlCatalog = this.xmlCatalogSortTypes[8]; break;
						}
					}
					if (functionSortFlags[4] == true) 
					{
						this.xmlSubtype = this.xmlSubtypes[12];
						switch (functionSubsort)
						{
							case 1: this.xmlCatalog = this.xmlCatalogSortTypes[30]; break;
							case 2: this.xmlCatalog = this.xmlCatalogSortTypes[31]; break;
							case 4: this.xmlCatalog = this.xmlCatalogSortTypes[32]; break;
							case 8: this.xmlCatalog = this.xmlCatalogSortTypes[33]; break;
							case 16: this.xmlCatalog = this.xmlCatalogSortTypes[34]; break;
							case 32: this.xmlCatalog = this.xmlCatalogSortTypes[35]; break;
							case 64: this.xmlCatalog = this.xmlCatalogSortTypes[36]; break;
							case 128: this.xmlCatalog = this.xmlCatalogSortTypes[8]; break;
						}
					}
					if (functionSortFlags[5] == true) 
					{
						this.xmlSubtype = this.xmlSubtypes[13];
						switch (functionSubsort)
						{
							case 1: this.xmlCatalog = this.xmlCatalogSortTypes[37]; break;
							case 2: this.xmlCatalog = this.xmlCatalogSortTypes[38]; break;
							case 4: this.xmlCatalog = this.xmlCatalogSortTypes[39]; break;
							case 8: this.xmlCatalog = this.xmlCatalogSortTypes[40]; break;
							case 16: this.xmlCatalog = this.xmlCatalogSortTypes[41]; break;
							case 32: this.xmlCatalog = this.xmlCatalogSortTypes[42]; break;
							case 64: this.xmlCatalog = this.xmlCatalogSortTypes[43]; break;
							case 128: this.xmlCatalog = this.xmlCatalogSortTypes[8]; break;
						}
					}
					if (functionSortFlags[6] == true) 
					{
						this.xmlSubtype = this.xmlSubtypes[14]; 
						switch (functionSubsort)
						{
							case 1: this.xmlCatalog = this.xmlCatalogSortTypes[44]; break;
							case 2: this.xmlCatalog = this.xmlCatalogSortTypes[45]; break;
							case 4: this.xmlCatalog = this.xmlCatalogSortTypes[46]; break;
							case 8: this.xmlCatalog = this.xmlCatalogSortTypes[47]; break;
							case 16: this.xmlCatalog = this.xmlCatalogSortTypes[48]; break;
							case 32: this.xmlCatalog = this.xmlCatalogSortTypes[49]; break;
							case 64: this.xmlCatalog = this.xmlCatalogSortTypes[50]; break;
							case 128: this.xmlCatalog = this.xmlCatalogSortTypes[8]; break;
						}
					}
					if (functionSortFlags[7] == true) 
					{
						this.xmlSubtype = this.xmlSubtypes[15];
						switch (functionSubsort)
						{
							case 1: this.xmlCatalog = this.xmlCatalogSortTypes[51]; break;
							case 2: this.xmlCatalog = this.xmlCatalogSortTypes[52]; break;
							case 4: this.xmlCatalog = this.xmlCatalogSortTypes[53]; break;
							case 8: this.xmlCatalog = this.xmlCatalogSortTypes[54]; break;
							case 16: this.xmlCatalog = this.xmlCatalogSortTypes[55]; break;
							case 32: this.xmlCatalog = this.xmlCatalogSortTypes[56]; break;
							case 64: this.xmlCatalog = this.xmlCatalogSortTypes[57]; break;
							case 128: this.xmlCatalog = this.xmlCatalogSortTypes[8]; break;
						}
					}
					if (functionSortFlags[8] == true) 
					{
						this.xmlSubtype = this.xmlSubtypes[16];
						switch (functionSubsort)
						{
							case 1: this.xmlCatalog = this.xmlCatalogSortTypes[58]; break;
							case 2: this.xmlCatalog = this.xmlCatalogSortTypes[59]; break;
							case 4: this.xmlCatalog = this.xmlCatalogSortTypes[60]; break;
							case 8: this.xmlCatalog = this.xmlCatalogSortTypes[61]; break;
							case 16: this.xmlCatalog = this.xmlCatalogSortTypes[62]; break;
							case 32: this.xmlCatalog = this.xmlCatalogSortTypes[63]; break;
							case 64: this.xmlCatalog = this.xmlCatalogSortTypes[64]; break;
							case 128: this.xmlCatalog = this.xmlCatalogSortTypes[8]; break;
						}
					}
					if (functionSortFlags[10] == true) 
					{
						this.xmlSubtype = this.xmlSubtypes[17];
					}
					if (this.xmlSubtype != "") this.xmlCatalog = this.xmlSubtype + " -> " + this.xmlCatalog;

				}

				IEnumerator ie = functionSortFlags.GetEnumerator();
				while (ie.MoveNext() == true)
				{
					if (this.debugMode) Console.Write("{0} ", ie.Current);
				}
				if (this.debugMode) Console.WriteLine();

				if (this.debugMode) Console.WriteLine(functionSortFlag);
			} 
		}

        public ExtractedItems readCPFchunk(BinaryReader readFile)
		{
			// Read an uncompressed CPF chunk and extract the name, description and type
			// Version
            var temp = new ExtractedItems();
			readFile.ReadUInt16();

			uint numItems = readFile.ReadUInt32();
			if (this.debugMode) Console.WriteLine("  CPF chunk numItems: " + numItems);

			// Read the items
			for (int i = 0; i < numItems; i++)
			{
				// Get type of the item
				
                string dataType = readFile.ReadUInt32().ToString("X8");
				uint nameLength = readFile.ReadUInt32();
				string fieldName = Encoding.UTF8.GetString(readFile.ReadBytes((int)nameLength));
				if (this.debugMode) Console.WriteLine("data type: " + dataType);
				if (this.debugMode) Console.WriteLine("field name: " + fieldName);

				uint fieldValueInt = 0;
				string fieldValueString = "";


				switch (dataType)
				{
						// Int
					case "EB61E4F7":
						fieldValueInt = readFile.ReadUInt32();
						break;
						// Int #2 - Not Used
					case "0C264712":
						fieldValueInt = readFile.ReadUInt32();
						break;
						// String
					case "0B8BEA18":
						uint stringLength = readFile.ReadUInt32();
						fieldValueString = Encoding.UTF8.GetString(readFile.ReadBytes((int)stringLength));
						break;
						// Float
					case "ABC78708":
						// Ignore for now
						uint fieldValueFloat = readFile.ReadUInt32();
						break;
						// Boolean
					case "CBA908E1":
						bool fieldValueBool = readFile.ReadBoolean();
						break;
				}
                
				switch (fieldName)
				{
					case "name":
						if (this.title == "") this.title = fieldValueString;                        
                        temp.Type = "Title";
                        temp.Content = this.title;                        
                        break;
					case "description":
						if (this.description == "") this.description = fieldValueString;
						temp.Type = "Description";
                        temp.Content = this.description;
                        break;
					case "type":
						this.xmlType = fieldValueString;
                        temp.Type = "XML Type";
                        temp.Content = this.xmlType;
                        break;
					case "subtype":
						this.xmlSubtype = fieldValueInt.ToString();
                        temp.Type = "XML Subtype";
                        temp.Content = this.xmlSubtype;
                        break;
					case "category":
						this.xmlCategory = fieldValueInt.ToString();
						temp.Type = "XML Category";
                        temp.Content = this.xmlCategory;
                        break;
					case "modelName":
						this.xmlModelName = fieldValueString;
						temp.Type = "XML Model Name";
                        temp.Content = this.xmlModelName;
                        break;
					case "objectGUID":
						this.objectGUID.Add(fieldValueInt.ToString("X8"));
						temp.Type = "objectGUID";
                        temp.Content = this.objectGUID.ToString();
                        break;
					case "creator":
						this.xmlCreator = fieldValueString;
						temp.Type = "XML Creator";
                        temp.Content = this.xmlCreator;
                        break;
					case "age":
						this.xmlAge = fieldValueInt.ToString();
						temp.Type = "Age";
                        temp.Content = this.xmlAge;break;
					case "gender":
						this.xmlGender = fieldValueInt.ToString();
						temp.Type = "Gender";
                        temp.Content = this.xmlGender;
						break;
				}
                                
			}
        return temp;
		}
		public ExtractedItems readCPFchunk(ReadByteStream readFile)
		{
			// Read a compressed CPF chunk from a byte stream and extrac the name, 
			// description and type
            var temp = new ExtractedItems();
			// Version
			readFile.ReadUInt16();

			uint numItems = readFile.ReadUInt32();
			if (this.debugMode) Console.WriteLine("  CPF chunk numItems: " + numItems);

			// Read the items
			for (int i = 0; i < numItems; i++)
			{
				// Get type of the item
                
				string dataType = readFile.ReadUInt32().ToString("X8");
				uint nameLength = readFile.ReadUInt32();
				string fieldName = Encoding.UTF8.GetString(readFile.ReadBytes(nameLength));
				if (this.debugMode) Console.WriteLine("data type: " + dataType);
				if (this.debugMode) Console.WriteLine("field name: " + fieldName);

				uint fieldValueInt = 0;
				string fieldValueString = "";

				switch (dataType)
				{
					// Int
					case "EB61E4F7":
						fieldValueInt = readFile.ReadUInt32();
						break;
					// Int #2 - Not Used
					case "0C264712":
						fieldValueInt = readFile.ReadUInt32();
						break;
					// String
					case "0B8BEA18":
						uint stringLength = readFile.ReadUInt32();
						fieldValueString = Encoding.UTF8.GetString(readFile.ReadBytes(stringLength));
						break;
					// Float
					case "ABC78708":
						// Ignore for now
						uint fieldValueFloat = readFile.ReadUInt32();
						break;
					// Boolean
					case "CBA908E1":
						bool fieldValueBool = readFile.ReadBoolean();
						break;
				}

				switch (fieldName)
				{
					case "name":
						if (this.title == "") this.title = fieldValueString;                        
                        temp.Type = "Title";
                        temp.Content = this.title;
                        break;
					case "description":
						if (this.description == "") this.description = fieldValueString;
						temp.Type = "Description";
                        temp.Content = this.description;
                        break;
					case "type":
						this.xmlType = fieldValueString;
                        temp.Type = "XML Type";
                        temp.Content = this.xmlType;
                        break;
					case "subtype":
						this.xmlSubtype = fieldValueInt.ToString();
                        temp.Type = "XML Subtype";
                        temp.Content = this.xmlSubtype;
                        break;
					case "category":
						this.xmlCategory = fieldValueInt.ToString();
						temp.Type = "XML Category";
                        temp.Content = this.xmlCategory;
                        break;
					case "modelName":
						this.xmlModelName = fieldValueString;
						temp.Type = "XML Model Name";
                        temp.Content = this.xmlModelName;
                        break;
					case "objectGUID":
						this.objectGUID.Add(fieldValueInt.ToString("X8"));
						temp.Type = "objectGUID";
                        temp.Content = this.objectGUID.ToString();
						break;
					case "creator":
						this.xmlCreator = fieldValueString;
						temp.Type = "XML Creator";
                        temp.Content = this.xmlCreator;
                        break;
					case "age":
						this.xmlAge = fieldValueInt.ToString();
						temp.Type = "Age";
                        temp.Content = this.xmlAge;
                        break;
					case "gender":
						this.xmlGender = fieldValueInt.ToString();
						temp.Type = "Gender";
                        temp.Content = this.xmlGender;
                        break;
				}
                
			}
        return temp;
		}

        public ExtractedItems readSTRchunk(BinaryReader readFile)
		{
			var temp = new ExtractedItems();
            readFile.ReadBytes(64);
			readFile.ReadBytes(2);
			uint numStrings = readFile.ReadUInt16();
			int lineNum = 0;
			string tempString = "";
			for (int j = 0; j < numStrings; j++)
			{
				byte langCode = readFile.ReadByte();
				if (langCode == 1)
				{
					lineNum++;
					if (lineNum == 1) { 
						tempString = readNullString(readFile).Replace("\n", " ");
						if ((this.title != "") && (this.description == "")) 
						{
							this.description = tempString;
                            temp.Type = "Description";
                            temp.Content = this.description;
						} 
						else 
						{
							this.title = tempString;
                            temp.Type = "Title";
                            temp.Content = this.title;						
						}
					}
					if (lineNum == 2) { 
						tempString = readNullString(readFile).Replace("\n", " "); 
						this.description += tempString; 
                        temp.Type = "Description";
                        temp.Content = this.description;
					}
					readNullString(readFile);
				}
				else
				{
					readNullString(readFile);
					readNullString(readFile);
				}
				if ((this.title != "") && (this.description != "")) { break; }
			}
            
            return temp;
		}

        public string readNullString(BinaryReader reader)
		{
			string result = "";
			char c;
			for (int i = 0; i < reader.BaseStream.Length; i++) 
			{
				//Console.WriteLine("Position: " + reader.BaseStream.Position.ToString());
				if ((c = (char) reader.ReadByte()) == 0) 
				{
					break;
				}
				result += c.ToString();
			}
			//Console.WriteLine(result);
			return result;
		}

		public ExtractedItems readSTRchunk(ReadByteStream readFile)
		{
			readFile.SkipAhead(66);
            var temp = new ExtractedItems();
			//readFile.ReadBytes(64);
			//readFile.ReadBytes(2);

			if (this.title != "") this.title += " ";
			if (this.description != "") this.description += " ";

			uint numStrings = readFile.ReadUInt16();
			int lineNum = 0;
			string tempString = "";
			for (int j = 0; j < numStrings; j++)
			{
				byte langCode = readFile.ReadByte();
				if (langCode == 1)
				{
					lineNum++;
					if (lineNum == 1) 
					{ 
						tempString = readFile.GetNullString().Replace("\n", " ");
						if ((this.title != "") && (this.description == "")) 
						{
							this.description += tempString;
                            temp.Type = "Description";
                            temp.Content = this.description;
						} 
						else 
						{
							this.title += tempString;
                            temp.Type = "Title";
                            temp.Content = this.title;
						} 
					}
					if (lineNum == 2) 
					{ 
						tempString = readFile.GetNullString().Replace("\n", " "); 
						this.description += tempString;
                        temp.Type = "Description";
                        temp.Content = this.description;
					}

					readFile.GetNullString();
				}
				else
				{
					readFile.GetNullString();
					readFile.GetNullString();
				}
				if ((this.title != "") && (this.description != "")) { break; }			
            }
            return temp;
		}

        public ExtractedItems readXMLchunk(ReadByteStream readFile)
		{
			var temp = new ExtractedItems();
			XmlTextReader xmlDoc = new XmlTextReader(new StringReader(Encoding.UTF8.GetString(readFile.GetEntireStream())));
			//xmlDoc.Load(new StringReader(xmlData));
			bool inDesc = false;
			string inAttrDesc = "";

			while (xmlDoc.Read())
			{
				if (xmlDoc.NodeType == XmlNodeType.Element) 
				{
					if (xmlDoc.Name == "AnyString")	inDesc = true;
					if (xmlDoc.Name == "AnyUint32") inDesc = true;
				}
				if (xmlDoc.NodeType == XmlNodeType.EndElement)
				{
					inDesc = false;
					inAttrDesc = "";
				}
				if (inDesc == true)
				{
					if (xmlDoc.AttributeCount > 0) 
					{
						while (xmlDoc.MoveToNextAttribute())
						{
							switch (xmlDoc.Value)
							{
								case "description":
								case "type":
								case "name":
								case "subsort":
								case "subtype":
								case "category":
									inAttrDesc = xmlDoc.Value;
									break;
							}
						}
					}
				}
				if (xmlDoc.NodeType == XmlNodeType.Text)
				{
					if (inAttrDesc != "") 
					{
						switch (inAttrDesc)
						{
							case "subtype":
								this.xmlSubtype = xmlDoc.Value;                                
								temp.Type = "XML Subtype";
                                temp.Content = this.xmlSubtype;
                                break;
							case "subsort":
								this.xmlSubtype = xmlDoc.Value;
								temp.Type = "XML Subtype";
                                temp.Content = this.xmlSubtype;
                                break;
							case "category":
								this.xmlCategory = xmlDoc.Value;
								temp.Type = "XML Category";
                                temp.Content = this.xmlCategory;
                                break;
							case "name":
								if (this.title == "") this.title = xmlDoc.Value;
								temp.Type = "Title";
                                temp.Content = this.title;
                                break;
							case "type":
								this.xmlType = xmlDoc.Value;
								temp.Type = "XML Type";
                                temp.Content = this.xmlType;
                                break;
							case "description":
								if (this.description == "") this.description = xmlDoc.Value.Replace("\n", " ");
								temp.Type = "Description";
                                temp.Content = this.description;
                                break;
						}
					}
				}
				//if ((this.title != "") && (this.description != "")) break;
			}            
            return temp;
		}

		public ExtractedItems readXMLchunk(string xmlData)
		{
			var temp = new ExtractedItems();
			XmlTextReader xmlDoc = new XmlTextReader(new StringReader(xmlData));
			//xmlDoc.Load(new StringReader(xmlData));
			bool inDesc = false;
			string inAttrDesc = "";

			while (xmlDoc.Read())
			{
				if (xmlDoc.NodeType == XmlNodeType.Element) 
				{
					if (xmlDoc.Name == "AnyString") inDesc = true;
					if (xmlDoc.Name == "AnyUint32") inDesc = true;
				}
				if (xmlDoc.NodeType == XmlNodeType.EndElement)
				{
					inDesc = false;
					inAttrDesc = "";
				}
				if (inDesc == true)
				{
					if (xmlDoc.AttributeCount > 0) 
					{
						while (xmlDoc.MoveToNextAttribute())
						{
							switch (xmlDoc.Value)
							{
								case "description":
								case "type":
								case "name":
								case "subsort":
								case "subtype":
								case "category":
									inAttrDesc = xmlDoc.Value;
									break;
							}
						}
					}
				}
				if (xmlDoc.NodeType == XmlNodeType.Text)
				{
					if (inAttrDesc != "") 
					{
						switch (inAttrDesc)
						{
							case "subtype":
								this.xmlSubtype = xmlDoc.Value;                                
								temp.Type = "XML Subtype";
                                temp.Content = this.xmlSubtype;
                                break;
							case "subsort":
								this.xmlSubtype = xmlDoc.Value;
								temp.Type = "XML Subtype";
                                temp.Content = this.xmlSubtype;
                                break;
							case "category":
								this.xmlCategory = xmlDoc.Value;
								temp.Type = "XML Category";
                                temp.Content = this.xmlCategory;
                                break;
							case "name":
								if (this.title == "") this.title = xmlDoc.Value;
								temp.Type = "Title";
                                temp.Content = this.title;
                                break;
							case "type":
								this.xmlType = xmlDoc.Value;
								temp.Type = "XML Type";
                                temp.Content = this.xmlType;
                                break;
							case "description":
								if (this.description == "") this.description = xmlDoc.Value.Replace("\n", " ");
								temp.Type = "Description";
                                temp.Content = this.description;
                                break;
						}
					}
				}
				//if ((this.title != "") && (this.description != "")) break;
			}
        return temp;
		}

        public ExtractedItems readCTSSchunk(BinaryReader readFile)
		{
            var temp = new ExtractedItems();
			readFile.ReadBytes(64);
			readFile.ReadUInt16();

			uint numStrings = readFile.ReadUInt16();
			bool foundLang = false;
			//numStrings = 2;
			for (int k = 0; k < numStrings; k++)
			{
				int langCode = Convert.ToInt32(readFile.ReadByte().ToString());

				string blah = readNullString(readFile);
				string meep = readNullString(readFile);

				if (langCode == 1) 
				{
					if (foundLang == true) { this.description = blah.Replace("\n", " "); 
                    temp.Type = "Description";
                    temp.Content = this.description;
                    }
					if (foundLang == false) { this.title = blah.Replace("\n", " "); foundLang = true; 
                    temp.Type = "Title";
                    temp.Content = this.title;
                    }
                    
				}

			}            
        return temp;
		}

		public ExtractedItems readCTSSchunk(ReadByteStream readFile)
		{   
            var temp = new ExtractedItems();

			readFile.SkipAhead(66);
			//readFile.ReadBytes(64);
			//readFile.ReadUInt16();

			uint numStrings = readFile.ReadUInt16();
			bool foundLang = false;
			//numStrings = 2;
			for (int k = 0; k < numStrings; k++)
			{
				byte[] langCode = readFile.ReadBytes(1);

				string blah = readFile.GetNullString();
				string meep = readFile.GetNullString();

				if (langCode[0] == 1) 
				{
					if (foundLang == true) { this.description = blah.Replace("\n", " "); 
                    temp.Type = "Description";
                    temp.Content = this.description;
                    return temp;}
					if (foundLang == false) { this.title = blah.Replace("\n", " "); foundLang = true; }
                    temp.Type = "Title";
                    temp.Content = this.title;
				}

			}        
        return temp;
		}


    }
    } 
}
