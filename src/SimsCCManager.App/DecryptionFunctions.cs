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

namespace DBPFReading {
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