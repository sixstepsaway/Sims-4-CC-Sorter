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
using SimsCCManager.Packages.Containers;
using SSAGlobals;

namespace SimsCCManager.Packages.Decryption
{
    public class DecryptByteStream
    {
        public int currOffset = 0;
        public byte[] byteStream;

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

    public class ReadEntries {

        public string[] packageTypes = { };
        public string[] xmlCatalogSortTypes = { };
		public string[] xmlSubtypes = { };
		public string[] xmlCategoryTypes = { };
        public string xmlType = "";
		public string xmlCategory = "";
		public string xmlSubtype = "";
		public string xmlModelName = "";
		public string xmlAge = "";
		public string xmlGender = "";
		public string xmlCatalog = "";
		public ArrayList objectGUID = new ArrayList();
		public string xmlCreator = "";

        public string title = "";
		public string description = "";
		public string pkgType = "";
		public uint pkgTypeInt = 0;
        public ArrayList guidData = new ArrayList();

        public bool debugMode = false;

        SimsPackage infovar = new SimsPackage();
		LoggingGlobals log = new LoggingGlobals();


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
			return uncdata;
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
				if (this.debugMode); 

			} 
		}

        public SimsPackage readCPFchunk(BinaryReader readFile)
		{
			
			
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
						if (this.title == "") this.title = fieldValueString;
						infovar.Title = this.title;
						break;
					case "description":
						if (this.description == "") this.description = fieldValueString;
						infovar.Description = this.description;
						break;
					case "type":
						this.xmlType = fieldValueString;
						infovar.XMLType = this.xmlType;
						break;
					case "subtype":
						this.xmlSubtype = fieldValueInt.ToString();
						infovar.XMLSubtype = this.xmlSubtype;
						break;
					case "category":
						this.xmlCategory = fieldValueInt.ToString();
						infovar.XMLCategory = this.xmlCategory;
						break;
					case "modelName":
						this.xmlModelName = fieldValueString;
						infovar.XMLModelName = this.xmlModelName;
						break;
					case "objectGUID":
						this.objectGUID.Add(fieldValueInt.ToString("X8"));
						infovar.ObjectGUID = this.objectGUID;
						break;
					case "creator":
						this.xmlCreator = fieldValueString;
						infovar.XMLCreator = this.xmlCreator;
						break;
					case "age":
						this.xmlAge = fieldValueInt.ToString();
						infovar.XMLAge = xmlAge;
						break;
					case "gender":
						this.xmlGender = fieldValueInt.ToString();
						infovar.XMLGender = xmlGender;
						break;
				}
			}
			return infovar;

		}
		public SimsPackage readCPFchunk(DecryptByteStream readFile)
		{
			
			
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
						if (this.title == "") this.title = fieldValueString;
						infovar.Title = this.title;
						break;
					case "description":
						if (this.description == "") this.description = fieldValueString;
						infovar.Description = this.description;
						break;
					case "type":
						this.xmlType = fieldValueString;
						infovar.XMLType = this.xmlType;
						break;
					case "subtype":
						this.xmlSubtype = fieldValueInt.ToString();
						infovar.XMLSubtype = this.xmlSubtype;
						break;
					case "category":
						this.xmlCategory = fieldValueInt.ToString();
						infovar.XMLCategory = this.xmlCategory;
						break;
					case "modelName":
						this.xmlModelName = fieldValueString;
						infovar.XMLModelName = this.xmlModelName;
						break;
					case "objectGUID":
						this.objectGUID.Add(fieldValueInt.ToString("X8"));
						infovar.ObjectGUID = this.objectGUID;
						break;
					case "creator":
						this.xmlCreator = fieldValueString;
						infovar.XMLCreator = this.xmlCreator;
						break;
					case "age":
						this.xmlAge = fieldValueInt.ToString();
						infovar.XMLAge = this.xmlAge;
						break;
					case "gender":
						this.xmlGender = fieldValueInt.ToString();
						infovar.XMLGender = this.xmlGender;
						break;
				}
			}
			return infovar;

		}

        public SimsPackage readSTRchunk(BinaryReader readFile)
		{		
            infovar = new SimsPackage();
            infovar.Location = "na";
            infovar.Game = 2;
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
						if ((infovar.Title != null) && (infovar.Description == null)) 
						{
							infovar.Title = tempString;
						} 
						else 
						{
							infovar.Title = tempString;
						}
					}
					if (lineNum == 2) { 
						tempString = readNullString(readFile).Replace("\n", " "); 
						infovar.Description += tempString;
					}
					readNullString(readFile);
				}
				else
				{
					readNullString(readFile);
					readNullString(readFile);
				}
				if ((infovar.Title != null) && (infovar.Description != null)) { break; }
			}
            return infovar;
		}

        public string readNullString(BinaryReader reader)
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

		public SimsPackage readSTRchunk(DecryptByteStream readFile)
		{	
            infovar = new SimsPackage();
            infovar.Location = "na";
            infovar.Game = 2;
			readFile.SkipAhead(66);

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
						if ((infovar.Title != "") && (infovar.Description == "")) 
						{
							infovar.Title += tempString;
						} 
						else 
						{
							infovar.Title += tempString;
						}
					}
					if (lineNum == 2) 
					{ 
						tempString = readFile.GetNullString().Replace("\n", " "); 
						infovar.Description += tempString; 
					}

					readFile.GetNullString();
				}
				else
				{
					readFile.GetNullString();
					readFile.GetNullString();
				}
				if ((infovar.Title != "") && (infovar.Description != "")) { break; }
			}
            return infovar;
		}

		public void readOBJDchunk(DecryptByteStream readFile)
		{
			
			
			//readFile.ReadBytes(64); // Filename - 64 bytes
			readFile.SkipAhead(64);
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
				readFile.SkipAhead(46);
				uint roomSortFlag = readFile.ReadUInt16();
				int[] functionSortFlag = new int[1];
				functionSortFlag[0] = (int)readFile.ReadUInt16();
				BitArray functionSortFlags = new BitArray(functionSortFlag);

				// No function sort, check Build Mode Sort
				if (functionSortFlag[0] == 0) 
				{
					// Skip until we hit the Build Mode sort and EP
					readFile.SkipAhead(46);
					uint expansionFlag = readFile.ReadUInt16();

					readFile.SkipAhead(8);
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
					readFile.SkipAhead(46);
					uint expansionFlag = readFile.ReadUInt16();

					readFile.SkipAhead(8);
					uint buildModeType = readFile.ReadUInt16();
					string originalGUID = readFile.ReadUInt32().ToString("X8");
					string objectModelGUID = readFile.ReadUInt32().ToString("X8");
					uint buildModeSubsort = readFile.ReadUInt16();
					readFile.SkipAhead(38);
					uint functionSubsort = readFile.ReadUInt16();

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
			}
		}

        public SimsPackage readXMLchunk(DecryptByteStream readFile)
		{
			
			uint fieldValueInt = 0;
			string fieldValueString = "";
			log.MakeLog("Reading XML", true);
			var entirestream = readFile.GetEntireStream();
			StringReader stringver = new StringReader(Encoding.UTF8.GetString(entirestream));
			XmlTextReader xmlDoc = new XmlTextReader(stringver);
			log.MakeLog(entirestream.ToString(), true);
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
							case "name":
								if (this.title == "") this.title = fieldValueString;
								log.MakeLog("Title is " + this.title, true);
								infovar.Title = this.title;
								break;
							case "description":
								if (this.description == "") this.description = fieldValueString;
								log.MakeLog("Description is " + this.description, true);
								infovar.Description = this.description;
								break;
							case "type":
								this.xmlType = fieldValueString;
								log.MakeLog("XML Type is " + this.xmlType, true);
								infovar.XMLType = this.xmlType;
								break;
							case "subsort":
								this.xmlSubtype = fieldValueInt.ToString();
								log.MakeLog("XML Subtype is " + this.xmlSubtype, true);
								infovar.XMLSubtype = this.xmlSubtype;
								break;
							case "category":
								this.xmlCategory = fieldValueInt.ToString();
								log.MakeLog("XML Category is " + this.xmlCategory, true);
								infovar.XMLCategory = this.xmlCategory;
								break;
						}
					}
				}
				//if ((this.title != "") && (this.description != "")) break;
			}
			return infovar;
		}

		public SimsPackage readXMLchunk(string xmlData)
		{
			
			uint fieldValueInt = 0;
			string fieldValueString = "";
			
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
							case "name":
								if (this.title == "") this.title = fieldValueString;
								infovar.Title = this.title;
								break;
							case "description":
								if (this.description == "") this.description = fieldValueString;
								infovar.Description = this.description;
								break;
							case "type":
								this.xmlType = fieldValueString;
								infovar.XMLType = this.xmlType;
								break;
							case "subsort":
								this.xmlSubtype = fieldValueInt.ToString();
								infovar.XMLSubtype = this.xmlSubtype;
								break;
							case "category":
								this.xmlCategory = fieldValueInt.ToString();
								infovar.XMLCategory = this.xmlCategory;
								break;
						}
					}
				}
				//if ((this.title != "") && (this.description != "")) break;
			}
			return infovar;
		}

        public void readCTSSchunk(BinaryReader readFile)
		{
			
			
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
					if (foundLang == true) { this.description = blah.Replace("\n", " "); break; }
					if (foundLang == false) { this.title = blah.Replace("\n", " "); foundLang = true; }
				}

			}
		}

		public void readCTSSchunk(DecryptByteStream readFile)
		{
			
			

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
					if (foundLang == true) { this.description = blah.Replace("\n", " "); break; }
					if (foundLang == false) { this.title = blah.Replace("\n", " "); foundLang = true; }
				}

			}
		}
    }
}