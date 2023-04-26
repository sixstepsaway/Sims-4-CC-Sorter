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

        
        

        public SimsPackage readOBJDchunk(BinaryReader readFile)
		{
            infovar = new SimsPackage();
			if (this.debugMode) Console.WriteLine("  Reading un-compressed OBJD...");

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
					//if (this.xmlCategory == "") this.xmlCategory = this.xmlCategoryTypes[1];

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
            return infovar;
		}

		public SimsPackage readOBJDchunk(DecryptByteStream readFile)
		{
            infovar = new SimsPackage();
            log.MakeLog("Reading compressed OBJD.", true);
			//readFile.ReadBytes(64); // Filename - 64 bytes
			readFile.SkipAhead(64);
			uint version = readFile.ReadUInt32();
            log.MakeLog("Version: " + version, true);
			var test = readFile.ReadUInt16(); // Initial Stack Size
            log.MakeLog("Initial Stack Size: " + test, true);
			test = readFile.ReadUInt16(); // Default Wall Adjacent Flags
            log.MakeLog("Default Wall Adjacent Flags: " + test, true);
			test = readFile.ReadUInt16(); // Default Placement Flags
            log.MakeLog("Default Placement Flags: " + test, true);
			test = readFile.ReadUInt16(); // Default Wall Placement Flags
            log.MakeLog("Default Wall Placement Flags: " + test, true);
			test = readFile.ReadUInt16(); // Default Allowed Height Flags
            log.MakeLog("Default Allowed Height Flags: " + test, true);
			test = readFile.ReadUInt16(); // Interaction Table ID
            log.MakeLog("Interaction Table ID: " + test, true);
			test = readFile.ReadUInt16(); // Interaction Group
            log.MakeLog("Interaction Group: " + test, true);
			uint objectType = readFile.ReadUInt16(); // Type of Object
            log.MakeLog("Type of Object: " + objectType, true);
			uint masterTileMasterId = readFile.ReadUInt16();
            log.MakeLog("Master Tile Master ID: " + masterTileMasterId, true);
			uint masterTileSubIndex = readFile.ReadUInt16();
            log.MakeLog("Master Tile Sub Index: " + masterTileSubIndex, true);

			// Only check further if this is a Master ID or single id
			if ((masterTileSubIndex == 65535) || (masterTileMasterId == 0))
			{
				test = readFile.ReadUInt16(); // Use Default Placement Flags
                log.MakeLog("Use Default Placement Flags: " + test, true);
				test = readFile.ReadUInt16(); // Look at Score
                log.MakeLog("Score: " + test, true);
				uint objectGUID = readFile.ReadUInt32();                
				this.objectGUID.Add(objectGUID.ToString("X8"));
                log.MakeLog("ObjectGUID: " + objectGUID, true);
				//this.objectGUID = objectGUID.ToString("X8");
				this.guidData.Add(this.objectGUID);
				// Skip stuff we don't need
				readFile.SkipAhead(46);
                log.MakeLog("Skipping 46 bytes.", true);
				uint roomSortFlag = readFile.ReadUInt16();
                log.MakeLog("Room Sort Flag: " + roomSortFlag, true);
				int[] functionSortFlag = new int[1];
				functionSortFlag[0] = (int)readFile.ReadUInt16();
				BitArray functionSortFlags = new BitArray(functionSortFlag);
                log.MakeLog("Function Sort Flags: ", true);
                log.MakeLog("Function Sort Flags Length: " + functionSortFlags.Length, true);

				// No function sort, check Build Mode Sort
				if (functionSortFlag[0] == 0) 
				{
                    log.MakeLog("No function sort flag.", true);
					// Skip until we hit the Build Mode sort and EP
					readFile.SkipAhead(46);
					uint expansionFlag = readFile.ReadUInt16();
                    log.MakeLog("Expansion Flag: " + expansionFlag, true);

					readFile.SkipAhead(8);
					uint buildModeType = readFile.ReadUInt16();
                    log.MakeLog("Build Mode Type: " + buildModeType, true);
					string originalGUID = readFile.ReadUInt32().ToString("X8");
                    log.MakeLog("Original GUID: " + originalGUID, true);
					string objectModelGUID = readFile.ReadUInt32().ToString("X8");
                    log.MakeLog("Object Model GUID: " + objectModelGUID, true);
					uint buildModeSubsort = readFile.ReadUInt16();
                    log.MakeLog("Build Mode Subsort: " + buildModeSubsort, true);

                    log.MakeLog("Build Mode Types: " + this.xmlSubtype, true);

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
                    infovar.XMLCategory = "Object";
					
                    //if (this.xmlCategory == "") this.xmlCategory = this.xmlCategoryTypes[1];
					// Also, get the catalog placement for this object
					readFile.SkipAhead(46);
					uint expansionFlag = readFile.ReadUInt16();
                    if (expansionFlag == 0){
                        //infovar.RequiredEPs = "None";
                    }
                    log.MakeLog("Expansion Flag: " + expansionFlag, true);

					readFile.SkipAhead(8);
					uint buildModeType = readFile.ReadUInt16();
                    log.MakeLog("Build Mode Type: " + buildModeType, true);
					string originalGUID = readFile.ReadUInt32().ToString("X8");
                    log.MakeLog("Original GUID: " + originalGUID, true);
					string objectModelGUID = readFile.ReadUInt32().ToString("X8");
                    log.MakeLog("Object Model GUID: " + objectModelGUID, true);
					uint buildModeSubsort = readFile.ReadUInt16();
                    log.MakeLog("Build Mode Subsort: " + buildModeSubsort, true);
					readFile.SkipAhead(38);
					uint functionSubsort = readFile.ReadUInt16();
                    log.MakeLog("Function Subsort: " + functionSubsort, true);

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
            return infovar;
		}












    }
}