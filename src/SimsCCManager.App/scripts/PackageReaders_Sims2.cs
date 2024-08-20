using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SimsCCManager.Packages.Containers;
using System.Xml;
using SimsCCManager.Globals;

namespace SimsCCManager.PackageReaders
{
    public class Sims2PackageReader
    {
        public static SimsPackage ReadSims2Package(SimsPackage package){
            Sims2ScanData scanData = new();
            List<string> AllInstanceIDS = new();
			List<IndexEntry> PackageEntries = new();
            IndexEntry SHPE = new();
            uint chunkOffset = 0;
            List<IndexEntry> IndexData = new();
            FileInfo pack = new FileInfo(package.Location);
            //MemoryStream msPackage = Methods.ReadBytesToFile(input, 12);
            FileStream msPackage = new FileStream(package.Location, FileMode.Open, FileAccess.Read);
            BinaryReader readFile = new BinaryReader(msPackage);
            //"DBPF" but we already know this is fine
            Encoding.ASCII.GetString(readFile.ReadBytes(4));
            uint major = readFile.ReadUInt32();                
            uint minor = readFile.ReadUInt32();

            string reserved = Encoding.UTF8.GetString(readFile.ReadBytes(12));
            uint dateCreated = readFile.ReadUInt32();
            uint dateModified = readFile.ReadUInt32();
            uint indexMajorVersion = readFile.ReadUInt32();
            uint indexCount = readFile.ReadUInt32();
            uint indexOffset = readFile.ReadUInt32();
            uint indexSize = readFile.ReadUInt32();
            uint holesCount = readFile.ReadUInt32();
            uint holesOffset = readFile.ReadUInt32();
            uint holesSize = readFile.ReadUInt32();
            uint indexMinorVersion = readFile.ReadUInt32() -1;
            string reserved2 = Encoding.UTF8.GetString(readFile.ReadBytes(32));
            int dirnum = -1;

            msPackage.Seek(chunkOffset + indexOffset, SeekOrigin.Begin);
            for (int i = 0; i < indexCount; i++){
                IndexEntry holderEntry = new IndexEntry();
                holderEntry.TypeID = readFile.ReadUInt32().ToString("X8");

                holderEntry.GroupID = readFile.ReadUInt32().ToString("X8");

                holderEntry.InstanceID = readFile.ReadUInt32().ToString("X8");               

                if ((indexMajorVersion == 7) && (indexMinorVersion == 1)) {
                    holderEntry.InstanceID2 = readFile.ReadUInt32().ToString("X8");
                } else {
                    holderEntry.InstanceID2 = "00000000";
                }

                holderEntry.Offset = readFile.ReadUInt32();
                holderEntry.Filesize = readFile.ReadUInt32();
                holderEntry.Truesize = 0;
                holderEntry.Compressed = false;
                IndexData.Add(holderEntry);               
                if (indexCount == 0) 
                {
                    readFile.Close();
                    return package;
                }
            }

            int entrynum = 0;
            foreach (IndexEntry iEntry in IndexData){
                switch (iEntry.TypeID.ToLower())
                {
                    case "fc6eb1f7":
                        SHPE = iEntry;
                        break;
                }

                EntryType e = GlobalVariables.Sims2EntryTypes.Where(x => x.TypeID == iEntry.TypeID).First();
                if (e != null){
                    PackageEntries.Add(new IndexEntry() {Tag = e.Tag, Location = entrynum, TypeID = e.TypeID});
                }
                entrynum++;
            }

            uint numrecords = -0; 
            string typeID = "";
            string groupID = "";
            string instanceID = "";
            string instanceID2 = "";
            uint myFilesize = -0;
            uint compfilesize;

            if (PackageEntries.Exists(x => x.Tag == "DIR")){
                dirnum = PackageEntries.Where(x => x.Tag == "STR#").First().Location;


                msPackage.Seek(chunkOffset + IndexData[dirnum].Offset, SeekOrigin.Begin);
                if (indexMajorVersion == 7 && indexMinorVersion == 1){
                    numrecords = IndexData[dirnum].Filesize / 20;
                } else {
                    numrecords = IndexData[dirnum].Filesize / 16;
                }

                for (int i = 0; i < numrecords; i++){
                    IndexEntry holderEntry = new();
                    typeID = readFile.ReadUInt32().ToString("X8");
                    groupID = readFile.ReadUInt32().ToString("X8");
                    instanceID = readFile.ReadUInt32().ToString("X8");
                    holderEntry.InstanceID = readFile.ReadUInt32().ToString("X8");
                    AllInstanceIDS.Add(holderEntry.InstanceID);
                    myFilesize = readFile.ReadUInt32();


                    foreach (IndexEntry idx in IndexData){
                        if ((idx.TypeID == typeID) && (idx.GroupID == groupID) && (idx.InstanceID == instanceID))
                        {
                            if (indexMajorVersion == 7 && indexMinorVersion == 1){
                                if (idx.InstanceID2 == instanceID2){
                                    idx.Compressed = true;
                                    idx.Filesize = myFilesize;
                                    break;
                                }
                            }
                        } else {
                            idx.Compressed = true;
                            idx.Truesize = myFilesize;
                            break;
                        }
                    }
                }
            }

            if (PackageEntries.Exists(x => x.Tag == "DIR")){
                msPackage.Seek(chunkOffset + IndexData[dirnum].Offset, SeekOrigin.Begin);
                if ((indexMajorVersion == 7) && indexMinorVersion == 1)
                {
                    numrecords = IndexData[dirnum].Filesize / 20;
                } else {
                    numrecords = IndexData[dirnum].Filesize / 16;
                }

                for (int i = 0; i < numrecords; i++){
                    IndexEntry holderEntry = new();
                    typeID = readFile.ReadUInt32().ToString("X8");
                    groupID = readFile.ReadUInt32().ToString("X8");
                    instanceID = readFile.ReadUInt32().ToString("X8");
                    holderEntry.InstanceID = readFile.ReadUInt32().ToString("X8");
                    AllInstanceIDS.Add(holderEntry.InstanceID);

                    if (indexMajorVersion == 7 && indexMinorVersion == 1){
                        instanceID2 = readFile.ReadUInt32().ToString("X8");
                    }
                    compfilesize = readFile.ReadUInt32();
                    int idxcount = 0;

                    

                    foreach (IndexEntry idx in IndexData){
                        string typefound = "";
                        idxcount++;
                        EntryType e = GlobalVariables.Sims2EntryTypes.Where(x => x.TypeID == idx.TypeID).First();
                        if (e != null){
                            typefound = e.Tag;
                        }

                        int cFileSize = 0;
                        string cTypeID = "";

                        if (typefound == "CTSS"){
                            msPackage.Seek(chunkOffset + idx.Offset, SeekOrigin.Begin);
                            cFileSize = readFile.ReadInt32();
							cTypeID = readFile.ReadUInt16().ToString("X4");
                            if (cTypeID == "FB10") 
							{
                                byte[] tempBytes = readFile.ReadBytes(3);
								uint cFullSize = EntryReaders.QFSLengthToInt(tempBytes);



								DecryptByteStream decompressed = new DecryptByteStream(EntryReaders.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));

								scanData = EntryReaders.ReadCTSSChunk(decompressed, scanData);                                
							} 
							else 
							{
                                msPackage.Seek(chunkOffset + idx.Offset, SeekOrigin.Begin);
								scanData = EntryReaders.ReadCTSSChunk(readFile, scanData);                                
							}
                        } else if (typefound == "XOBJ" || typefound == "XFNC" || typefound == "XFLR" || typefound == "XMOL" || typefound == "XROF"  || typefound == "XTOL"  || typefound == "XHTN"){
                            msPackage.Seek(chunkOffset + idx.Offset, SeekOrigin.Begin);
                            cFileSize = readFile.ReadInt32();
                            cTypeID = readFile.ReadUInt16().ToString("X4");
                            if (cTypeID == "FB10"){
                                byte[] tempBytes = readFile.ReadBytes(3);
                                uint cFullSize = EntryReaders.QFSLengthToInt(tempBytes);
                                string cpfTypeID = readFile.ReadUInt32().ToString("X8");

                                if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0")){
                                    scanData = EntryReaders.ReadCPFChunk(readFile, scanData);
                                } else {
                                    msPackage.Seek(chunkOffset + idx.Offset + 9, SeekOrigin.Begin);
                                    DecryptByteStream decompressed = new DecryptByteStream(EntryReaders.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));

                                    if (cpfTypeID == "E750E0E2")
                                    {
                                        cpfTypeID = decompressed.ReadUInt32().ToString("X8");
                                        if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0")) 
                                        {
                                            scanData = EntryReaders.ReadCPFChunk(decompressed, scanData);
                                            
                                        } 
                                    } else {
                                        scanData = EntryReaders.ReadXMLChunk(decompressed, scanData);
                                    }
                                }
                            } else {
                                // it's not that?? idk my code from a year ago ends here LOL
                            }
                        }
                    }
                }
            }

            if (PackageEntries.Exists(x => x.Tag == "OBJD")){
                int cFileSize = -1;
                string cTypeID = "";
                List<int> objdnum = PackageEntries.Where(x => x.Tag == "OBJD").Select(x => x.Location).ToList();
                foreach (int loc in objdnum){
                    msPackage.Seek(chunkOffset + IndexData[loc].Offset, SeekOrigin.Begin);
                    cFileSize = readFile.ReadInt32();
                    cTypeID = readFile.ReadUInt16().ToString("X4");
                    if (cTypeID == "FB10")
                    { 
                        byte[] tempBytes = readFile.ReadBytes(3);
                        uint cFullSize = EntryReaders.QFSLengthToInt(tempBytes);
                        DecryptByteStream decompressed = new DecryptByteStream(EntryReaders.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));
                        scanData = EntryReaders.ReadOBJDChunk(decompressed, scanData);
                        
                    } else {
                        msPackage.Seek(chunkOffset + IndexData[loc].Offset, SeekOrigin.Begin);
                        scanData = EntryReaders.ReadOBJDChunk(readFile, scanData);
                        
                    }
                }
            }


            if (PackageEntries.Exists(x => x.Tag == "STR#")){
                int cFileSize = -1;
                string cTypeID = "";
                List<int> strnm = PackageEntries.Where(x => x.Tag == "STR#").Select(x => x.Location).ToList();
                
                foreach (int loc in strnm){
                    msPackage.Seek(chunkOffset + IndexData[loc].Offset, SeekOrigin.Begin);
                    cFileSize = readFile.ReadInt32();
                    cTypeID = readFile.ReadUInt16().ToString("X4");
                    
                    if (cTypeID == "FB10")
                    {
                        byte[] tempBytes = readFile.ReadBytes(3);
                        uint cFullSize = EntryReaders.QFSLengthToInt(tempBytes);

                        DecryptByteStream decompressed = new DecryptByteStream(EntryReaders.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));

                        scanData = EntryReaders.ReadSTRChunk(decompressed, scanData);                        
                    } 
                    else 
                    {
                        scanData = EntryReaders.ReadSTRChunk(readFile, scanData);                        
                    }
                }
            }

            if (PackageEntries.Exists(x => x.Tag == "MMAT")){
                int cFileSize = -1;
                string cTypeID = "";
                List<int> matnm = PackageEntries.Where(x => x.Tag == "MMAT").Select(x => x.Location).ToList();
                
                foreach (int loc in matnm){
                    msPackage.Seek(chunkOffset + IndexData[loc].Offset, SeekOrigin.Begin);
                    cFileSize = readFile.ReadInt32();
                    cTypeID = readFile.ReadUInt16().ToString("X4");
                    
                    if (cTypeID == "FB10") 
                    {
                        byte[] tempBytes = readFile.ReadBytes(3);
                        uint cFullSize = EntryReaders.QFSLengthToInt(tempBytes);

                        string cpfTypeID = readFile.ReadUInt32().ToString("X8");
                        if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0"))
                        {
                            scanData = EntryReaders.ReadCPFChunk(readFile, scanData);
                        } 
                        else 
                        {
                            msPackage.Seek(chunkOffset + IndexData[loc].Offset + 9, SeekOrigin.Begin);
                            DecryptByteStream decompressed = new DecryptByteStream(EntryReaders.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));

                            if (cpfTypeID == "E750E0E2") 
                            {
                                cpfTypeID = decompressed.ReadUInt32().ToString("X8");

                                if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0")) 
                                {
                                    scanData = EntryReaders.ReadCPFChunk(decompressed, scanData);                                    
                                }

                            } 
                            else 
                            {
                                scanData = EntryReaders.ReadXMLChunk(decompressed, scanData);                                
                            }
                        }
                    } else {
                        msPackage.Seek(chunkOffset + IndexData[loc].Offset, SeekOrigin.Begin);

                        string cpfTypeID = readFile.ReadUInt32().ToString("X8");
                        if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0"))
                        {
                            scanData = EntryReaders.ReadCPFChunk(readFile, scanData);                            
                        }

                        if  (cpfTypeID == "6D783F3C")
                        {
                            msPackage.Seek(chunkOffset + IndexData[loc].Offset, SeekOrigin.Begin);

                            string xmlData = Encoding.UTF8.GetString(readFile.ReadBytes((int)IndexData[loc].Filesize));
                            scanData = EntryReaders.ReadXMLChunk(xmlData, scanData);                        

                        }
                    }
                }
            }
            
            if (PackageEntries.Exists(x => x.Tag == "IMG"))
            {
                int cFileSize = -1;
                string cTypeID = "";
                List<int> imgnm = PackageEntries.Where(x => x.Tag == "IMG").Select(x => x.Location).ToList();
                
                foreach (int loc in imgnm){
                    msPackage.Seek(chunkOffset + IndexData[loc].Offset, SeekOrigin.Begin);
                    cFileSize = readFile.ReadInt32();
                    cTypeID = readFile.ReadUInt16().ToString("X4");

                    if (cTypeID == "FB10") 
                    {
                        byte[] tempBytes = readFile.ReadBytes(3);
                        uint cFullSize = EntryReaders.QFSLengthToInt(tempBytes);

                        string cpfTypeID = readFile.ReadUInt32().ToString("X8");
                        if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0"))
                        {
                            scanData = EntryReaders.ReadCPFChunk(readFile, scanData);
                        } 
                        else 
                        {
                            msPackage.Seek(chunkOffset + IndexData[loc].Offset + 9, SeekOrigin.Begin);
                            DecryptByteStream decompressed = new DecryptByteStream(EntryReaders.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));

                            if (cpfTypeID == "E750E0E2") 
                            {

                                cpfTypeID = decompressed.ReadUInt32().ToString("X8");

                                if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0")) 
                                {
                                    scanData = EntryReaders.ReadCPFChunk(decompressed, scanData);
                                }

                            } 
                            else 
                            {
                                scanData = EntryReaders.ReadXMLChunk(decompressed, scanData);
                            }
                        }
                    } 
                    else 
                    {
                        msPackage.Seek(chunkOffset + IndexData[loc].Offset, SeekOrigin.Begin);

                        string cpfTypeID = readFile.ReadUInt32().ToString("X8");
                        if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0"))
                        {
                            scanData = EntryReaders.ReadCPFChunk(readFile, scanData);
                        }

                        if  (cpfTypeID == "6D783F3C")
                        {
                            msPackage.Seek(chunkOffset + IndexData[loc].Offset, SeekOrigin.Begin);

                            string xmlData = Encoding.UTF8.GetString(readFile.ReadBytes((int)IndexData[loc].Filesize));
                            scanData = EntryReaders.ReadXMLChunk(xmlData, scanData);

                        }
                    }
                }
            }
            
            msPackage.Close();
            readFile.Close();
            msPackage.Dispose();
            readFile.Dispose();
			package.ScanData = scanData;
			package.Scanned = true;
			package.WriteInfoFile();			
            return package;

            
		}
	}

    public class DecryptByteStream{
        private int currOffset = 0;
        private byte[] byteStream;

		public DecryptByteStream(byte[] inputBytes) 
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

    public class EntryReaders{
        
        public static uint QFSLengthToInt(byte[] data)
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

        public static byte[] Uncompress(byte[] data, uint targetSize, int offset)
		{			
			byte[] uncdata = null;
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
				}
			} 
			catch(Exception ex)
			{
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
			return uncdata;
		}

        public static Sims2ScanData ReadCTSSChunk(BinaryReader readFile, Sims2ScanData scanData)
		{	
			S2CTSS ctss = new();	
            string description = "";
            string title = "";

			readFile.ReadBytes(64);
			readFile.ReadUInt16();

			uint numStrings = readFile.ReadUInt16();
			bool foundLang = false;
            
			for (int k = 0; k < numStrings; k++)
			{
				int langCode = Convert.ToInt32(readFile.ReadByte().ToString());

				string blah = ReadNullString(readFile);
				string meep = ReadNullString(readFile);

				if (langCode == 1) 
				{
					if (foundLang == true) { description = blah.Replace("\n", " "); break; }
					if (foundLang == false) { title = blah.Replace("\n", " "); foundLang = true; }
				}

			}
            if (!string.IsNullOrWhiteSpace(description) && !string.IsNullOrWhiteSpace(description)){
                ctss.Description = description;
            }
            if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(title)){
                ctss.Title = title;
            }
			scanData.CTSSData.Add(ctss);
            return scanData;
		}

		public static Sims2ScanData ReadCTSSChunk(DecryptByteStream readFile, Sims2ScanData scanData)
		{
            S2CTSS ctss = new();
			string description = "";
            string title = "";

			readFile.SkipAhead(66);

			uint numStrings = readFile.ReadUInt16();
			bool foundLang = false;
            
			for (int k = 0; k < numStrings; k++)
			{
				byte[] langCode = readFile.ReadBytes(1);

				string blah = readFile.GetNullString();
				string meep = readFile.GetNullString();

				if (langCode[0] == 1) 
				{
					if (foundLang == true) { description = blah.Replace("\n", " "); break; }
					if (foundLang == false) { title = blah.Replace("\n", " "); foundLang = true; }
				}

			}
            if (!string.IsNullOrWhiteSpace(description) && !string.IsNullOrWhiteSpace(description)){
                ctss.Description = description;
            }
            if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(title)){
                ctss.Title = title;
            }           
            scanData.CTSSData.Add(ctss);
            return scanData;
		}

        public static Sims2ScanData ReadCPFChunk(BinaryReader readFile, Sims2ScanData scanData)
		{
			S2CPF cpf = new();
			// Read an uncompressed CPF chunk and extract the name, description and type
			// Version
			readFile.ReadUInt16();

			uint numItems = readFile.ReadUInt32();

			// Read the items
			for (int i = 0; i < numItems; i++)
			{
				// Get type of the item
				string dataType = readFile.ReadUInt32().ToString("X8");
				uint nameLength = readFile.ReadUInt32();
				string fieldName = Encoding.UTF8.GetString(readFile.ReadBytes((int)nameLength));

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
						cpf.Title = fieldValueString;
						break;
					case "description":
						cpf.Description = fieldValueString;
						break;
					case "type":
						cpf.XMLType = fieldValueString;
						break;
					case "subtype":
						cpf.XMLSubtype = fieldValueInt.ToString();
						break;
					case "category":
						cpf.XMLCategory = fieldValueInt.ToString();
						break;
					case "modelName":
						cpf.XMLModelName = fieldValueString;
						break;
					case "objectGUID":
						scanData.GUIDs.Add(fieldValueInt.ToString("X8"));
						cpf.GUIDs.Add(fieldValueInt.ToString("X8"));
						break;
					case "creator":
						cpf.XMLCreator = fieldValueString;
						break;
					case "age":
						cpf.XMLAge = fieldValueInt.ToString();
						break;
					case "gender":
						cpf.XMLGender = fieldValueInt.ToString();
						break;
				}
			}
			scanData.CPFData.Add(cpf);
            return scanData;            
		}
		public static Sims2ScanData ReadCPFChunk(DecryptByteStream readFile, Sims2ScanData scanData)
		{
			S2CPF cpf = new();
			// Read a compressed CPF chunk from a byte stream and extrac the name, 
			// description and type

			// Version
			readFile.ReadUInt16();

			uint numItems = readFile.ReadUInt32();

			// Read the items
			for (int i = 0; i < numItems; i++)
			{
				// Get type of the item
				string dataType = readFile.ReadUInt32().ToString("X8");
				uint nameLength = readFile.ReadUInt32();
				string fieldName = Encoding.UTF8.GetString(readFile.ReadBytes(nameLength));

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
						cpf.Title = fieldValueString;
						break;
					case "description":
						cpf.Description = fieldValueString;
						break;
					case "type":
						cpf.XMLType = fieldValueString;
						break;
					case "subtype":
						cpf.XMLSubtype = fieldValueInt.ToString();
						break;
					case "category":
						cpf.XMLCategory = fieldValueInt.ToString();
						break;
					case "modelName":
						cpf.XMLModelName = fieldValueString;
						break;
					case "objectGUID":
						scanData.GUIDs.Add(fieldValueInt.ToString("X8"));
						cpf.GUIDs.Add(fieldValueInt.ToString("X8"));
						break;
					case "creator":
						cpf.XMLCreator = fieldValueString;
						break;
					case "age":
						cpf.XMLAge = fieldValueInt.ToString();
						break;
					case "gender":
						cpf.XMLGender = fieldValueInt.ToString();
						break;
				}
			}          
			scanData.CPFData.Add(cpf);  
			return scanData;
		}
        

        public static Sims2ScanData ReadSTRChunk(BinaryReader readFile, Sims2ScanData scanData)
		{		
            
			S2STR str = new();
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
						tempString = ReadNullString(readFile).Replace("\n", " ");
                        if ((tempString != " ") && (tempString != "")){
                            str.Title = tempString;
                        }
					}
					if (lineNum == 2) { 
						tempString = ReadNullString(readFile).Replace("\n", " "); 
                        if ((tempString != " ") && (tempString != "")){
                            str.Description += tempString;
                        }
						
					}
					ReadNullString(readFile);
				}
				else
				{
					ReadNullString(readFile);
					ReadNullString(readFile);
				}
				if ((str.Title != null) && (str.Description != null)) { break; }
			}
			scanData.STRData.Add(str);
            return scanData;
		}

        

		public static Sims2ScanData ReadSTRChunk(DecryptByteStream readFile, Sims2ScanData scanData)
		{	
            S2STR str = new();
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
					if (lineNum == 1) 
					{ 
						tempString = readFile.GetNullString().Replace("\n", " ");
                        if ((tempString != " ") && (tempString != "")){
                            str.Title += tempString;
                        }
                        
					}
					if (lineNum == 2) 
					{ 
						tempString = readFile.GetNullString().Replace("\n", " "); 
                        if ((tempString != " ") && (tempString != "")){
                            str.Description += tempString;
                        }
						                         
					}

					readFile.GetNullString();
				}
				else
				{
					readFile.GetNullString();
					readFile.GetNullString();
				}
				
			}
            scanData.STRData.Add(str);
            return scanData;
		}

        public static Sims2ScanData ReadXMLChunk(DecryptByteStream readFile, Sims2ScanData scanData)
		{
			S2XML s2XML = new();			
			XmlTextReader xmlDoc = new XmlTextReader(new StringReader(Encoding.UTF8.GetString(readFile.GetEntireStream())));
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
								s2XML.XMLSubtype = xmlDoc.Value;
								break;
							case "subsort":
								s2XML.XMLSubtype = xmlDoc.Value;
								break;
							case "category":
								s2XML.XMLCategory = xmlDoc.Value;
								break;
							case "name":
								s2XML.Title = xmlDoc.Value;
								break;
							case "type":
								s2XML.XMLType = xmlDoc.Value;
								break;
							case "description":
								s2XML.Description = xmlDoc.Value.Replace("\n", " ");
								break;
						}
					}
				}
				//if ((this.title != "") && (this.description != "")) break;
			}
			scanData.XMLData.Add(s2XML);
            return scanData;
		}

		public static Sims2ScanData ReadXMLChunk(string xmlData, Sims2ScanData scanData)
		{
			S2XML s2XML = new();
			
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
								s2XML.XMLSubtype = xmlDoc.Value;
								break;
							case "subsort":
								s2XML.XMLSubtype = xmlDoc.Value;
								break;
							case "category":
								s2XML.XMLCategory = xmlDoc.Value;
								break;
							case "name":
								s2XML.Title = xmlDoc.Value;
								break;
							case "type":
								s2XML.XMLType = xmlDoc.Value;
								break;
							case "description":
								s2XML.Description = xmlDoc.Value.Replace("\n", " ");
								break;
						}
					}
				}
				//if ((this.title != "") && (this.description != "")) break;
            }       
			scanData.XMLData.Add(s2XML);           
            return scanData;
		}


        public static Sims2ScanData ReadOBJDChunk(BinaryReader readFile, Sims2ScanData scanData)
		{
            S2OBJD objd = new();
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
				scanData.GUIDs.Add(readFile.ReadUInt32().ToString("X8"));
				objd.GUIDs.Add(readFile.ReadUInt32().ToString("X8"));
				//this.objectGUID = objectGUID.ToString("X8");
				//this.guidData.Add(this.objectGUID);
				// Skip stuff we don't need
				readFile.ReadBytes(46);
				uint roomSortFlag = readFile.ReadUInt16();
				int[] functionSortFlag = new int[1];
				functionSortFlag[0] = (int)readFile.ReadUInt16();
				BitArray functionSortFlags = new BitArray(functionSortFlag);

                int fsfn = 0;
                foreach (var fsf in functionSortFlags){
                    fsfn++;
                }


				// No function sort, check Build Mode Sort
				if (functionSortFlag[0] == 0)                 
				{
                    objd.XMLCategory = "Build Item";
					// Skip until we hit the Build Mode sort and EP
					readFile.ReadBytes(46);
					uint expansionFlag = readFile.ReadUInt16();

					readFile.ReadBytes(8);
					uint buildModeType = readFile.ReadUInt16();
					string originalGUID = readFile.ReadUInt32().ToString("X8");
					string objectModelGUID = readFile.ReadUInt32().ToString("X8");
					uint buildModeSubsort = readFile.ReadUInt16();


                    int funcflags = functionSortFlags.Length;
					FunctionSortList thissort = GlobalVariables.Sims2BuildFunctionSortList.Where(x => x.flagnum == buildModeType && x.functionsubsortnum == buildModeSubsort).First();
                    objd.Function = thissort.Category;
					objd.FunctionSubcategory = thissort.Subcategory;
					
					/*foreach (FunctionSortList category in GlobalVariables.Sims2BuildFunctionSortList){                            
                        if ((buildModeType == category.flagnum) && (buildModeSubsort == category.functionsubsortnum)) {
                            scandata.Function = category.Category;
                            scandata.FunctionSubcategory = category.Subcategory;
                        }
                    }*/
				}
				else 
				{
					objd.XMLCategory = "Object";
					readFile.ReadBytes(46);
					uint expansionFlag = readFile.ReadUInt16();

					readFile.ReadBytes(8);
					uint buildModeType = readFile.ReadUInt16();
					string originalGUID = readFile.ReadUInt32().ToString("X8");
					string objectModelGUID = readFile.ReadUInt32().ToString("X8");
					uint buildModeSubsort = readFile.ReadUInt16();
					readFile.ReadBytes(38);
					uint functionSubsort = readFile.ReadUInt16();

                    

                    int funcflags = functionSortFlags.Length;					
                    foreach (FunctionSortList category in GlobalVariables.Sims2BuyFunctionSortList){
                        for (int f = 0; f < funcflags; f++){
                            if ((f == category.flagnum) && (functionSortFlags[f] == true) && (category.functionsubsortnum == functionSubsort)) {
                                objd.Function = category.Category;
                                objd.FunctionSubcategory = category.Subcategory;
                            }
                        }
                    } 
				}
            } 
			scanData.OBJDData.Add(objd);
            return scanData;
		}

		public static Sims2ScanData ReadOBJDChunk(DecryptByteStream readFile, Sims2ScanData scanData)
		{
           	S2OBJD objd = new();
			List<string> PackageTypes = new();        
            List<string> XMLCatalogSortTypes = new(); 
            List<string> xmlCatalogSortTypes = new();
			List<string> xmlSubtypes = new();
			List<string> xmlCategoryTypes = new();
            List<string> objectGUID = new();
			//readFile.ReadBytes(64); // Filename - 64 bytes
			readFile.SkipAhead(64);
			uint version = readFile.ReadUInt32();
			var test = readFile.ReadUInt16(); // Initial Stack Size
			test = readFile.ReadUInt16(); // Default Wall Adjacent Flags
			test = readFile.ReadUInt16(); // Default Placement Flags
			test = readFile.ReadUInt16(); // Default Wall Placement Flags
			test = readFile.ReadUInt16(); // Default Allowed Height Flags
			test = readFile.ReadUInt16(); // Interaction Table ID
			test = readFile.ReadUInt16(); // Interaction Group
			uint objectType = readFile.ReadUInt16(); // Type of Object
			uint masterTileMasterId = readFile.ReadUInt16();
			uint masterTileSubIndex = readFile.ReadUInt16();

			// Only check further if this is a Master ID or single id
			if ((masterTileSubIndex == 65535) || (masterTileMasterId == 0))
			{
				test = readFile.ReadUInt16(); // Use Default Placement Flags
				test = readFile.ReadUInt16(); // Look at Score
                scanData.GUIDs.Add(readFile.ReadUInt32().ToString("X8"));
				objd.GUIDs.Add(readFile.ReadUInt32().ToString("X8"));
				//this.objectGUID = objectGUID.ToString("X8");
				//this.guidData.Add(this.objectGUID);
				// Skip stuff we don't need
				readFile.SkipAhead(46);
				uint roomSortFlag = readFile.ReadUInt16();
				int[] functionSortFlag = new int[1];
				functionSortFlag[0] = (int)readFile.ReadUInt16();
				BitArray functionSortFlags = new BitArray(functionSortFlag);
                /*int fsfn = 0;
                foreach (var fsf in functionSortFlags){
                    fsfn++;
                }*/
                
                
                

				// No function sort, check Build Mode Sort
				if (functionSortFlag[0] == 0) 
				{
                    objd.XMLCategory = "Build Item";
					// Skip until we hit the Build Mode sort and EP
					readFile.ReadBytes(46);
					uint expansionFlag = readFile.ReadUInt16();

					readFile.ReadBytes(8);
					uint buildModeType = readFile.ReadUInt16();
					string originalGUID = readFile.ReadUInt32().ToString("X8");
					string objectModelGUID = readFile.ReadUInt32().ToString("X8");
					uint buildModeSubsort = readFile.ReadUInt16();


                    int funcflags = functionSortFlags.Length;
                    FunctionSortList thissort = GlobalVariables.Sims2BuildFunctionSortList.Where(x => x.flagnum == buildModeType && x.functionsubsortnum == buildModeSubsort).First();
                    objd.Function = thissort.Category;
					objd.FunctionSubcategory = thissort.Subcategory;
					 
				} 
				else 
				{
					// Set the xmlCategory to Object
                    objd.XMLCategory = "Object";
					
                    //if (this.xmlCategory == null) this.xmlCategory = this.xmlCategoryTypes[1];
					// Also, get the catalog placement for this object
					readFile.SkipAhead(46);
					uint expansionFlag = readFile.ReadUInt16();
                    if (expansionFlag == 0){
                        //infovar.RequiredEPs.Add("None");
                    } else {

					}

					readFile.SkipAhead(8);
					uint buildModeType = readFile.ReadUInt16();
					string originalGUID = readFile.ReadUInt32().ToString("X8");
					string objectModelGUID = readFile.ReadUInt32().ToString("X8");
					uint buildModeSubsort = readFile.ReadUInt16();
					readFile.SkipAhead(38);
					uint functionSubsort = readFile.ReadUInt16();

                    int funcflags = functionSortFlags.Length;
                    foreach (FunctionSortList category in GlobalVariables.Sims2BuyFunctionSortList){
                        for (int f = 0; f < funcflags; f++){
                            if ((f == category.flagnum) && (functionSortFlags[f] == true) && (category.functionsubsortnum == functionSubsort)) {
                                objd.Function = category.Category;
                                objd.FunctionSubcategory = category.Subcategory;
                            }
                        }
                    }                    
				}
			}
			scanData.OBJDData.Add(objd);
            return scanData;
		}

		public static Sims2ScanData ReadSHPEChunk(BinaryReader readFile, Sims2ScanData scanData)
		{
			S2SHPE shpe = new();
			string version = readFile.ReadUInt32().ToString("X8");
			//if (this.debugMode) Console.WriteLine(version);
			if (version == "FFFF0001") 
			{
				uint numFileLinks = readFile.ReadUInt32();
				for (int i = 0; i < numFileLinks; i++)
				{
					string groupID = readFile.ReadUInt32().ToString("X8");
					string instanceID = readFile.ReadUInt32().ToString("X8");
					string resourceID = readFile.ReadUInt32().ToString("X8");
					string typeID = readFile.ReadUInt32().ToString("X8");

					//if (this.debugMode) Console.WriteLine(groupID + " " + instanceID + " " + resourceID + " " + typeID);
				}
			}

			if (version != "FFFF0001") 
			{
				uint numFileLinks = Convert.ToUInt32(version);
				for (int i = 0; i < numFileLinks; i++)
				{
					string groupID = readFile.ReadUInt32().ToString("X8");
					string instanceID = readFile.ReadUInt32().ToString("X8");
					string resourceID = "";
					string typeID = readFile.ReadUInt32().ToString("X8");

					//if (this.debugMode) Console.WriteLine(groupID + " " + instanceID + " " + resourceID + " " + typeID);
				}
			}

			uint itemCount = readFile.ReadUInt32();
			for (int j = 0; j < itemCount; j++)
			{
				string rcolID = readFile.ReadUInt32().ToString("X8");
			}

			for (int k = 0; k < itemCount; k++)
			{
				string blockName = readFile.ReadString();
				string blockRcolID = readFile.ReadUInt32().ToString("X8");
				uint blockVersion = readFile.ReadUInt32();
								
				// SHPE block
				if (blockRcolID == "FC6EB1F7")
				{
					
									
					// cSGResource first
					string cSGResourceName = readFile.ReadString();
					string cSGResourceBlockID = readFile.ReadUInt32().ToString("X8");
					uint cSGResourceVersion = readFile.ReadUInt32();
					string cSGResourceFilename = readFile.ReadString();
					

					// cReferentNode
					string cReferentNodeName = readFile.ReadString();
					string cReferentBlockID = readFile.ReadUInt32().ToString("X8");
					uint cReferentVersion = readFile.ReadUInt32();
					

					// cObjectGraphNode
					string cObjectGraphNodeName = readFile.ReadString();
					string cObjectGraphNodeClassID = readFile.ReadUInt32().ToString("X8");
					uint cObjectGraphNodeVersion = readFile.ReadUInt32();
					uint cObjectGraphNodeNumExtensions = readFile.ReadUInt32();
								
					for (int l = 0; l < cObjectGraphNodeNumExtensions; l++)
					{
						byte cObjectGraphNodeEnabled = readFile.ReadByte();
						byte cObjectGraphNodeDepend = readFile.ReadByte();
						uint cObjectGraphNodeIndex = readFile.ReadUInt32();
					}

					string cObjectGraphNodeFilename = readFile.ReadString();

					if (blockVersion != 6)
					{
						uint unknown = readFile.ReadUInt32();
						for (int i = 0; i < unknown; i++)
						{
							readFile.ReadUInt32();
						}
					}

					// Shape Item
					if (blockVersion == 7)
					{
						uint shpeNumLods3 = readFile.ReadUInt32();
						for (int m = 0; m < shpeNumLods3; m++)
						{
							readFile.ReadUInt32();
							readFile.ReadByte();
							readFile.ReadByte();
							readFile.ReadUInt32();
						}
					}
					if (blockVersion == 8)
					{
						uint shpeNumLods2 = readFile.ReadUInt32();
						for (int m = 0; m < shpeNumLods2; m++)
						{
							readFile.ReadUInt32();
							readFile.ReadByte();
							string shpeGMND = readFile.ReadString();
						}
					}

					// Shape Parts
					uint shpeMatCount = readFile.ReadUInt32();
					//if (this.debugMode) Console.WriteLine("Material count: " + shpeMatCount);
					for (int i = 0; i < shpeMatCount; i++)
					{
						string shpePartType = readFile.ReadString();
						string shpePartDesc = readFile.ReadString();
						readFile.ReadBytes(9);
						bool addModelName = false;

						switch (shpePartType)
						{
							case "southwallshadow":
							case "northwallshadow":
							case "eastwallshadow":
							case "westwallshadow":
							case "groundshadow":
								break;
							case "hair":
							case "body":
							case "frame":
							case "top":
							case "bottom":
								//addModelName = true;
								shpe.Type = shpePartType + "Mesh";
								break;
							default:
								//addModelName = true;
								if ((shpePartType.IndexOf("accessory") > -1) || (shpePartDesc.IndexOf("accessory") > -1)) shpe.Type = "frameMesh";
								break;
						}

						if (addModelName == true)
						{
							TagsList modelName = new TagsList();
							if (shpePartDesc.IndexOf("!") > -1) { modelName.Description = shpePartDesc.Substring(shpePartDesc.IndexOf("!")+1); } 
							else { modelName.Description = shpePartDesc; }
							shpe.CatalogTags.Add(modelName);
						}

					}
				}
			}
			scanData.SHPEData.Add(shpe);
			return scanData;
		}

		public static Sims2ScanData ReadSHPEChunk(DecryptByteStream readFile, Sims2ScanData scanData)
		{
			S2SHPE shpe = new();
			string version = readFile.ReadUInt32().ToString("X8");
			//if (this.debugMode) Console.WriteLine(version);
			if (version == "FFFF0001") 
			{
				uint numFileLinks = readFile.ReadUInt32();
				for (int i = 0; i < numFileLinks; i++)
				{
					string groupID = readFile.ReadUInt32().ToString("X8");
					string instanceID = readFile.ReadUInt32().ToString("X8");
					string resourceID = readFile.ReadUInt32().ToString("X8");
					string typeID = readFile.ReadUInt32().ToString("X8");

					//if (this.debugMode) Console.WriteLine(groupID + " " + instanceID + " " + resourceID + " " + typeID);
				}
			}

			if (version != "FFFF0001") 
			{
				uint numFileLinks = Convert.ToUInt32(version);
				for (int i = 0; i < numFileLinks; i++)
				{
					string groupID = readFile.ReadUInt32().ToString("X8");
					string instanceID = readFile.ReadUInt32().ToString("X8");
					string resourceID = "";
					string typeID = readFile.ReadUInt32().ToString("X8");

					//if (this.debugMode) Console.WriteLine(groupID + " " + instanceID + " " + resourceID + " " + typeID);
				}
			}

			uint itemCount = readFile.ReadUInt32();
			for (int j = 0; j < itemCount; j++)
			{
				string rcolID = readFile.ReadUInt32().ToString("X8");
				//if (this.debugMode) Console.WriteLine(rcolID);
			}

			for (int k = 0; k < itemCount; k++)
			{
				string blockName = readFile.ReadString();
				//if (this.debugMode) Console.WriteLine(blockName);
				string blockRcolID = readFile.ReadUInt32().ToString("X8");
				uint blockVersion = readFile.ReadUInt32();
								
				// SHPE block
				if (blockRcolID == "FC6EB1F7")
				{
									
					// cSGResource first
					string cSGResourceName = readFile.ReadString();
					string cSGResourceBlockID = readFile.ReadUInt32().ToString("X8");
					uint cSGResourceVersion = readFile.ReadUInt32();
					string cSGResourceFilename = readFile.ReadString();

					// cReferentNode
					string cReferentNodeName = readFile.ReadString();
					string cReferentBlockID = readFile.ReadUInt32().ToString("X8");
					uint cReferentVersion = readFile.ReadUInt32();

					// cObjectGraphNode
					string cObjectGraphNodeName = readFile.ReadString();
					string cObjectGraphNodeClassID = readFile.ReadUInt32().ToString("X8");
					uint cObjectGraphNodeVersion = readFile.ReadUInt32();
					uint cObjectGraphNodeNumExtensions = readFile.ReadUInt32();
								
					for (int l = 0; l < cObjectGraphNodeNumExtensions; l++)
					{
						byte cObjectGraphNodeEnabled = readFile.ReadByte();
						byte cObjectGraphNodeDepend = readFile.ReadByte();
						uint cObjectGraphNodeIndex = readFile.ReadUInt32();
					}

					string cObjectGraphNodeFilename = readFile.ReadString();

					if (blockVersion != 6)
					{
						uint unknown = readFile.ReadUInt32();
						for (int i = 0; i < unknown; i++)
						{
							readFile.ReadUInt32();
						}
					}

					// Shape Item
					if (blockVersion == 7)
					{
						uint shpeNumLods3 = readFile.ReadUInt32();
						for (int m = 0; m < shpeNumLods3; m++)
						{
							readFile.ReadUInt32();
							readFile.ReadByte();
							readFile.ReadByte();
							readFile.ReadUInt32();
						}
					}
					if (blockVersion == 8)
					{
						uint shpeNumLods2 = readFile.ReadUInt32();
						for (int m = 0; m < shpeNumLods2; m++)
						{
							readFile.ReadUInt32();
							readFile.ReadByte();
							string shpeGMND = readFile.ReadString();
						}
					}

					// Shape Parts
					uint shpeMatCount = readFile.ReadUInt32();
					for (int i = 0; i < shpeMatCount; i++)
					{
						string shpePartType = readFile.ReadString();
						string shpePartDesc = readFile.ReadString();
						readFile.ReadBytes(9);
						
						bool addModelName = false;

						switch (shpePartType)
						{
							case "southwallshadow":
							case "northwallshadow":
							case "eastwallshadow":
							case "westwallshadow":
							case "groundshadow":
								break;
							case "hair":
							case "body":
							case "frame":
							case "top":
							case "bottom":
								//addModelName = true;
								shpe.Type = shpePartType + "Mesh";
								break;
							default:
								//addModelName = true;
								if ((shpePartType.IndexOf("accessory") > -1) || (shpePartDesc.IndexOf("accessory") > -1)) scanData.Type = "frameMesh";
								break;
						}

						if (addModelName == true)
						{
							TagsList modelName = new TagsList();
							if (shpePartDesc.IndexOf("!") > -1) { modelName.Description = shpePartDesc.Substring(shpePartDesc.IndexOf("!")+1); } 
							else { modelName.Description = shpePartDesc; }
							shpe.CatalogTags.Add(modelName);
						}

					}
				}
			}
			scanData.SHPEData.Add(shpe);
			return scanData;
		}

        public static string ReadNullString(BinaryReader reader)
		{
            string result = "";
			char c;
			for (int i = 0; i < reader.BaseStream.Length; i++) 
			{
				if ((c = (char) reader.ReadByte()) == 0) 
				{
					break;
				}
				result += c.ToString();
			}
			return result;
		}

    }



    public class IndexEntry
    {
        public string Tag {get; set;} = "";
        public int ID {get; set;} = -1;
        public string TypeID {get; set;} = "";
        public string GroupID {get; set;} = "";
        public string InstanceID {get; set;} = "";
        public string InstanceID2 {get; set;} = "";
        public uint Offset {get; set;} = 0;
        public uint Filesize {get; set;} = 0;
        public uint Truesize {get; set;} = 0;
        public bool Compressed {get; set;} = false;
        public string Unused {get; set;} = "";
        public string Size {get; set;} = "";
        public int Location {get; set;} = -1;
        public string PackageID {get; set;} = "";
    }	

    public class EntryType {
        /// <summary>
        /// For "types", for example Cas Parts or Geometry.
        /// </summary>
        public string Tag {get; set;}
        public string TypeID {get; set;}
        public string Description {get; set;}
    }
}