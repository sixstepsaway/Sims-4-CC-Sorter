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

                int fsfn = 0;
                foreach (var fsf in functionSortFlags){
                    log.MakeLog("Function Sort Flag [" + fsfn + "] is: " + functionSortFlags[fsfn].ToString(), true);                    
                    fsfn++;
                }


				// No function sort, check Build Mode Sort
				if (functionSortFlag[0] == 0)                 
				{
                    log.MakeLog("This is a build mode item.", true);
                    
                    infovar.XMLCategory = "Build Item";
                    log.MakeLog("This is a build mode item.", true);
					// Skip until we hit the Build Mode sort and EP
					readFile.ReadBytes(46);
					uint expansionFlag = readFile.ReadUInt16();
                    log.MakeLog("Expansion Flag: " + expansionFlag, true);

					readFile.ReadBytes(8);
					uint buildModeType = readFile.ReadUInt16();
                    log.MakeLog("Build Mode Type: " + buildModeType, true);
					string originalGUID = readFile.ReadUInt32().ToString("X8");
                    log.MakeLog("Original GUID: " + originalGUID, true);
					string objectModelGUID = readFile.ReadUInt32().ToString("X8");
                    log.MakeLog("Object Model GUID: " + objectModelGUID, true);
					uint buildModeSubsort = readFile.ReadUInt16();
                    log.MakeLog("Build Mode Subsort: " + buildModeSubsort, true);

                    log.MakeLog("Build Mode Types: " + this.xmlSubtype, true);

                    int funcflags = functionSortFlags.Length;
                    foreach (FunctionSortList category in TypeListings.S2BuildFunctionSort){                            
                        if ((buildModeType == category.flagnum) && (buildModeSubsort == category.functionsubsortnum)) {
                            infovar.Function = category.Category;
                            infovar.FunctionSubcategory = category.Subcategory;
                            log.MakeLog("Identified function: " + infovar.Function, true);
                        }                        
                    } 
				} 
				else 
				{
					infovar.XMLCategory = "Object";
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
                    foreach (FunctionSortList category in TypeListings.S2BuyFunctionSort){
                        //log.MakeLog("Catnum: " + category.flagnum, true);
                        for (int f = 0; f < funcflags; f++){
                            //log.MakeLog("Flag: " + f, true);                            
                            if ((f == category.flagnum) && (functionSortFlags[f] == true) && (category.functionsubsortnum == functionSubsort)) {
                                infovar.Function = category.Category;
                                infovar.FunctionSubcategory = category.Subcategory;
                                log.MakeLog("Identified function: " + infovar.Function + "/" + infovar.FunctionSubcategory, true);
                            }
                        }
                    } 
				}
                
				/*IEnumerator ie = functionSortFlags.GetEnumerator();
				while (ie.MoveNext() == true)
				{
					if (this.debugMode) Console.Write("{0} ", ie.Current);
				}
				if (this.debugMode) Console.WriteLine();

				if (this.debugMode) Console.WriteLine(functionSortFlag);*/
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
                log.MakeLog("Function Sort Flag: " + functionSortFlag[0], true);
                log.MakeLog("Function Sort Flags: ", true);
                /*int fsfn = 0;
                foreach (var fsf in functionSortFlags){
                    log.MakeLog("Function Sort Flag [" + fsfn + "] is: " + functionSortFlags[fsfn].ToString(), true);                    
                    fsfn++;
                }*/
                
                
                

				// No function sort, check Build Mode Sort
				if (functionSortFlag[0] == 0) 
				{
                    infovar.XMLCategory = "Build Item";
                    log.MakeLog("This is a build mode item.", true);
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

                    int funcflags = functionSortFlags.Length;
                    foreach (FunctionSortList category in TypeListings.S2BuildFunctionSort){                            
                        if ((buildModeType == category.flagnum) && (buildModeSubsort == category.functionsubsortnum)) {
                            infovar.Function = category.Category;
                            infovar.FunctionSubcategory = category.Subcategory;
                            log.MakeLog("Identified function: " + infovar.Function, true);
                        }                        
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
                        //infovar.RequiredEPs.Add("None");
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

                    int funcflags = functionSortFlags.Length;
                    foreach (FunctionSortList category in TypeListings.S2BuyFunctionSort){
                        //log.MakeLog("Catnum: " + category.flagnum, true);
                        for (int f = 0; f < funcflags; f++){
                            //log.MakeLog("Flag: " + f, true);                            
                            if ((f == category.flagnum) && (functionSortFlags[f] == true) && (category.functionsubsortnum == functionSubsort)) {
                                infovar.Function = category.Category;
                                infovar.FunctionSubcategory = category.Subcategory;
                                log.MakeLog("Identified function: " + infovar.Function + "/" + infovar.FunctionSubcategory, true);
                            }
                        }
                    }                    
				}
			}
            return infovar;
		}
    }
}