using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections;
using System.Xml;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.Security.Cryptography;
using SimsCCManager.Packages.Containers;
using SSAGlobals;

namespace SimsCCManager.Packages.Decryption
{
	/// <summary>
	/// Decryption functions for Sims 2 packs. Credits to:
	/// - https://modthesims.info/d/227925/delphy-s-download-organiser-v1-2-6365-updated-6th-june-2017.html
	/// - https://github.com/LazyDuchess/CC-Merger/tree/master/CCMerger
	/// - https://github.com/whoward69/Sims2Tools
	/// for a lot of the code I cannibalized for my own uses. 
	/// </summary>
    public class DecryptByteStream
    {
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
		private List<string> objectGUID = new List<string>();
		public string xmlCreator = "";
        public string xmlFunction = "";
        public string xmlFunctionSubsort = "";

        public string title = "";
		public string description = "";
		public string pkgType = "";
		public uint pkgTypeInt = 0;
        private  ArrayList guidData = new ArrayList();

        public bool debugMode = false;

		LoggingGlobals log = new LoggingGlobals();


        public uint QFSLengthToInt(Byte[] data)
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

        public Byte[] Uncompress(Byte[] data, uint targetSize, int offset)
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

        public SimsPackage readCPFchunk(BinaryReader readFile)
		{
			packageTypes = new string[0];
			xmlCatalogSortTypes = new string[0];
			xmlSubtypes = new string[0];
			xmlCategoryTypes = new string[0];
			xmlType = "";
			xmlCategory = "";
			xmlSubtype = "";
			xmlModelName = "";
			xmlAge = "";
			xmlGender = "";
			xmlCatalog = "";
			objectGUID = new List<string>();
			xmlCreator = "";
			xmlFunction = "";
			xmlFunctionSubsort = "";
			title = "";
			description = "";
			pkgType = "";
			pkgTypeInt = 0;
			SimsPackage infovar = new SimsPackage();
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
						this.title = fieldValueString;
						break;
					case "description":
						this.description = fieldValueString;
						break;
					case "type":
						this.xmlType = fieldValueString;
						break;
					case "subtype":
						this.xmlSubtype = fieldValueInt.ToString();
						break;
					case "category":
						this.xmlCategory = fieldValueInt.ToString();
						break;
					case "modelName":
						this.xmlModelName = fieldValueString;
						break;
					case "objectGUID":
						this.objectGUID.Add(fieldValueInt.ToString("X8"));
						break;
					case "creator":
						this.xmlCreator = fieldValueString;
						break;
					case "age":
						this.xmlAge = fieldValueInt.ToString();
						break;
					case "gender":
						this.xmlGender = fieldValueInt.ToString();
						break;
				}
			}
            if ((this.title != " ") && (this.title != ""))
            {
                infovar.Title = this.title;
            }
            if ((this.description != " ") && (this.description != ""))
            {
                infovar.Description = this.description;
            }
            if ((this.xmlType != " ") && (this.xmlType != ""))
            {
                infovar.Type = this.xmlType;
            }
            if ((this.xmlSubtype != " ") && (this.xmlSubtype != ""))
            {
                infovar.Subtype = this.xmlSubtype;
            }
            if ((this.xmlCategory != " ") && (this.xmlCategory != ""))
            {
                infovar.Category = this.xmlCategory;
            }
            if ((this.xmlModelName != " ") && (this.xmlModelName != ""))
            {
                infovar.ModelName = this.xmlModelName;
            }
            infovar.GUIDs.AddRange(this.objectGUID);
            if ((this.xmlCreator != " ") && (this.xmlCreator != ""))
            {
                infovar.Creator = this.xmlCreator;
            }
            if ((this.xmlAge != " ") && (this.xmlAge != ""))
            {
                infovar.Age = this.xmlAge;
            }
            if ((this.xmlGender != " ") && (this.xmlGender != ""))
            {
                infovar.Gender = this.xmlGender;
            }
			return infovar;
		}
		public SimsPackage readCPFchunk(DecryptByteStream readFile)
		{
			packageTypes = new string[0];
			xmlCatalogSortTypes = new string[0];
			xmlSubtypes = new string[0];
			xmlCategoryTypes = new string[0];
			xmlType = "";
			xmlCategory = "";
			xmlSubtype = "";
			xmlModelName = "";
			xmlAge = "";
			xmlGender = "";
			xmlCatalog = "";
			objectGUID = new List<string>();
			xmlCreator = "";
			xmlFunction = "";
			xmlFunctionSubsort = "";
			title = "";
			description = "";
			pkgType = "";
			pkgTypeInt = 0;
			SimsPackage infovar = new SimsPackage();
			
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
						this.title = fieldValueString;
						break;
					case "description":
						this.description = fieldValueString;
						break;
					case "type":
						this.xmlType = fieldValueString;
						break;
					case "subtype":
						this.xmlSubtype = fieldValueInt.ToString();
						break;
					case "category":
						this.xmlCategory = fieldValueInt.ToString();
						break;
					case "modelName":
						this.xmlModelName = fieldValueString;
						break;
					case "objectGUID":
						this.objectGUID.Add(fieldValueInt.ToString("X8"));
						break;
					case "creator":
						this.xmlCreator = fieldValueString;
						break;
					case "age":
						this.xmlAge = fieldValueInt.ToString();                        
						break;
					case "gender":
						this.xmlGender = fieldValueInt.ToString();                        
						break;
				}
			}
            if ((this.title != " ") && (this.title != ""))
            {
                infovar.Title = this.title;
            }
            if ((this.description != " ") && (this.description != ""))
            {
                infovar.Description = this.description;
            }
            if ((this.xmlType != " ") && (this.xmlType != ""))
            {
                infovar.Type = this.xmlType;
            }
            if ((this.xmlSubtype != " ") && (this.xmlSubtype != ""))
            {
                infovar.Subtype = this.xmlSubtype;
            }
            if ((this.xmlCategory != " ") && (this.xmlCategory != ""))
            {
                infovar.Category = this.xmlCategory;
            }
            if ((this.xmlModelName != " ") && (this.xmlModelName != ""))
            {
                infovar.ModelName = this.xmlModelName;
            }
            infovar.GUIDs.AddRange(this.objectGUID);
            if ((this.xmlCreator != " ") && (this.xmlCreator != ""))
            {
                infovar.Creator = this.xmlCreator;
            }
            if ((this.xmlAge != " ") && (this.xmlAge != ""))
            {
                infovar.Age = this.xmlAge;
            }
            if ((this.xmlGender != " ") && (this.xmlGender != ""))
            {
                infovar.Gender = this.xmlGender;
            }
			return infovar;

		}
        

        public SimsPackage readSTRchunk(BinaryReader readFile)
		{		
            packageTypes = new string[0];
			xmlCatalogSortTypes = new string[0];
			xmlSubtypes = new string[0];
			xmlCategoryTypes = new string[0];
			xmlType = "";
			xmlCategory = "";
			xmlSubtype = "";
			xmlModelName = "";
			xmlAge = "";
			xmlGender = "";
			xmlCatalog = "";
			objectGUID = new List<string>();
			xmlCreator = "";
			xmlFunction = "";
			xmlFunctionSubsort = "";
			title = "";
			description = "";
			pkgType = "";
			pkgTypeInt = 0;
			SimsPackage infovar = new SimsPackage();
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
                        if ((tempString != " ") && (tempString != "")){
                            this.title = tempString;
                        }
					}
					if (lineNum == 2) { 
						tempString = readNullString(readFile).Replace("\n", " "); 
                        if ((tempString != " ") && (tempString != "")){
                            this.description += tempString;
                        }
						
					}
					readNullString(readFile);
				}
				else
				{
					readNullString(readFile);
					readNullString(readFile);
				}
				if ((this.title != null) && (this.description != null)) { break; }
			}
            if ((this.description != "") && (this.description != " ")){
                infovar.Description = this.description;
            }

            if ((this.title != "") && (this.title != " ")){
                infovar.Title = this.title;
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
            packageTypes = new string[0];
			xmlCatalogSortTypes = new string[0];
			xmlSubtypes = new string[0];
			xmlCategoryTypes = new string[0];
			xmlType = "";
			xmlCategory = "";
			xmlSubtype = "";
			xmlModelName = "";
			xmlAge = "";
			xmlGender = "";
			xmlCatalog = "";
			objectGUID = new List<string>();
			xmlCreator = "";
			xmlFunction = "";
			xmlFunctionSubsort = "";
			title = "";
			description = "";
			pkgType = "";
			pkgTypeInt = 0;
			SimsPackage infovar = new SimsPackage();

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
                            this.title += tempString;
                        }
                        
					}
					if (lineNum == 2) 
					{ 
						tempString = readFile.GetNullString().Replace("\n", " "); 
                        if ((tempString != " ") && (tempString != "")){
                            this.description += tempString;
                        }
						                         
					}

					readFile.GetNullString();
				}
				else
				{
					readFile.GetNullString();
					readFile.GetNullString();
				}
				if ((this.title != null) && (this.description != null)) { break; }
			}
            if ((this.description != "") && (this.description != " ")){
                infovar.Description = this.description;
            }

            if ((this.title != "") && (this.title != " ")){
                infovar.Title = this.title;
            }
            return infovar;
		}

        public SimsPackage readXMLchunk(DecryptByteStream readFile)
		{
			packageTypes = new string[0];
			xmlCatalogSortTypes = new string[0];
			xmlSubtypes = new string[0];
			xmlCategoryTypes = new string[0];
			xmlType = "";
			xmlCategory = "";
			xmlSubtype = "";
			xmlModelName = "";
			xmlAge = "";
			xmlGender = "";
			xmlCatalog = "";
			objectGUID = new List<string>();
			xmlCreator = "";
			xmlFunction = "";
			xmlFunctionSubsort = "";
			title = "";
			description = "";
			pkgType = "";
			pkgTypeInt = 0;
			SimsPackage infovar = new SimsPackage();
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
								this.xmlSubtype = xmlDoc.Value;
								break;
							case "subsort":
								this.xmlSubtype = xmlDoc.Value;
								break;
							case "category":
								this.xmlCategory = xmlDoc.Value;
								break;
							case "name":
								this.title = xmlDoc.Value;
								break;
							case "type":
								this.xmlType = xmlDoc.Value;
								break;
							case "description":
								this.description = xmlDoc.Value.Replace("\n", " ");
								break;
						}
					}
				}
				//if ((this.title != "") && (this.description != "")) break;
			}

            if ((this.description != "") && (this.description != " ")){
                infovar.Description = this.description;
            }

            if ((this.title != "") && (this.title != " ")){
                infovar.Title = this.title;
            }
            if ((this.xmlType != "") && (this.xmlType != " ")){
                infovar.Type = this.xmlType;
            }
            
            if ((this.xmlCategory != "") && (this.xmlCategory != " ")){
                infovar.Category = this.xmlCategory;  
            }
            if ((this.xmlSubtype != "") && (this.xmlSubtype != " ")){
                infovar.Subtype = this.xmlSubtype;
            }                          
            return infovar;
		}

		public SimsPackage readXMLchunk(string xmlData)
		{
			packageTypes = new string[0];
			xmlCatalogSortTypes = new string[0];
			xmlSubtypes = new string[0];
			xmlCategoryTypes = new string[0];
			xmlType = "";
			xmlCategory = "";
			xmlSubtype = "";
			xmlModelName = "";
			xmlAge = "";
			xmlGender = "";
			xmlCatalog = "";
			objectGUID = new List<string>();
			xmlCreator = "";
			xmlFunction = "";
			xmlFunctionSubsort = "";
			title = "";
			description = "";
			pkgType = "";
			pkgTypeInt = 0;
			SimsPackage infovar = new SimsPackage();
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
							case "subtype":
								this.xmlSubtype = xmlDoc.Value;
								break;
							case "subsort":
								this.xmlSubtype = xmlDoc.Value;
								break;
							case "category":
								this.xmlCategory = xmlDoc.Value;
								break;
							case "name":
								this.title = xmlDoc.Value;
								break;
							case "type":
								this.xmlType = xmlDoc.Value;
								break;
							case "description":
								this.description = xmlDoc.Value.Replace("\n", " ");
								break;
						}
					}
				}
				//if ((this.title != "") && (this.description != "")) break;
            }
			
            if ((this.description != "") && (this.description != " ")){
                infovar.Description = this.description;
            }

            if ((this.title != "") && (this.title != " ")){
                infovar.Title = this.title;
            }
            if ((this.xmlType != "") && (this.xmlType != " ")){
                infovar.Type = this.xmlType;
            }
            
            if ((this.xmlCategory != "") && (this.xmlCategory != " ")){
                infovar.Category = this.xmlCategory;  
            }
            if ((this.xmlSubtype != "") && (this.xmlSubtype != " ")){
                infovar.Subtype = this.xmlSubtype;
            }                          
            return infovar;
		}

        public SimsPackage readCTSSchunk(BinaryReader readFile)
		{		
			packageTypes = new string[0];
			xmlCatalogSortTypes = new string[0];
			xmlSubtypes = new string[0];
			xmlCategoryTypes = new string[0];
			xmlType = "";
			xmlCategory = "";
			xmlSubtype = "";
			xmlModelName = "";
			xmlAge = "";
			xmlGender = "";
			xmlCatalog = "";
			objectGUID = new List<string>();
			xmlCreator = "";
			xmlFunction = "";
			xmlFunctionSubsort = "";
			title = "";
			description = "";
			pkgType = "";
			pkgTypeInt = 0;
			SimsPackage infovar = new SimsPackage();
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
            if ((this.description != "") && (this.description != " ")){
                infovar.Description = this.description;
            }
            if ((this.title != "") && (this.title != " ")){
                infovar.Title = this.title;
            }
            return infovar;
		}

		public SimsPackage readCTSSchunk(DecryptByteStream readFile)
		{
            packageTypes = new string[0];
			xmlCatalogSortTypes = new string[0];
			xmlSubtypes = new string[0];
			xmlCategoryTypes = new string[0];
			xmlType = "";
			xmlCategory = "";
			xmlSubtype = "";
			xmlModelName = "";
			xmlAge = "";
			xmlGender = "";
			xmlCatalog = "";
			objectGUID = new List<string>();
			xmlCreator = "";
			xmlFunction = "";
			xmlFunctionSubsort = "";
			title = "";
			description = "";
			pkgType = "";
			pkgTypeInt = 0;
			SimsPackage infovar = new SimsPackage();
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
            if ((this.description != "") && (this.description != " ")){
                infovar.Description = this.description;
            }
            if ((this.title != "") && (this.title != " ")){
                infovar.Title = this.title;
            }
            
            
            return infovar;
		}

        public SimsPackage readOBJDchunk(BinaryReader readFile)
		{
            packageTypes = new string[0];
			xmlCatalogSortTypes = new string[0];
			xmlSubtypes = new string[0];
			xmlCategoryTypes = new string[0];
			xmlType = "";
			xmlCategory = "";
			xmlSubtype = "";
			xmlModelName = "";
			xmlAge = "";
			xmlGender = "";
			xmlCatalog = "";
			objectGUID = new List<string>();
			xmlCreator = "";
			xmlFunction = "";
			xmlFunctionSubsort = "";
			title = "";
			description = "";
			pkgType = "";
			pkgTypeInt = 0;
			SimsPackage infovar = new SimsPackage();

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
                    fsfn++;
                }


				// No function sort, check Build Mode Sort
				if (functionSortFlag[0] == 0)                 
				{
                    this.xmlCategory = "Build Item";
					// Skip until we hit the Build Mode sort and EP
					readFile.ReadBytes(46);
					uint expansionFlag = readFile.ReadUInt16();

					readFile.ReadBytes(8);
					uint buildModeType = readFile.ReadUInt16();
					string originalGUID = readFile.ReadUInt32().ToString("X8");
					string objectModelGUID = readFile.ReadUInt32().ToString("X8");
					uint buildModeSubsort = readFile.ReadUInt16();


                    int funcflags = functionSortFlags.Length;
                    foreach (FunctionSortList category in TypeListings.S2BuildFunctionSort){                            
                        if ((buildModeType == category.flagnum) && (buildModeSubsort == category.functionsubsortnum)) {
                            infovar.Function = category.Category;
                            infovar.FunctionSubcategory = category.Subcategory;
                        }                        
                    } 
				} 
				else 
				{
					this.xmlCategory = "Object";
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
                        for (int f = 0; f < funcflags; f++){
                            if ((f == category.flagnum) && (functionSortFlags[f] == true) && (category.functionsubsortnum == functionSubsort)) {
                                this.xmlFunction = category.Category;
                                this.xmlFunctionSubsort = category.Subcategory;
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
            if ((this.xmlCategory != " ") && (this.xmlCategory != "")) {
                infovar.Category = this.xmlCategory;
            }
            if ((this.xmlFunction != " ") && (this.xmlFunction != "")) {
                infovar.Function = this.xmlFunction;
            }
            if ((this.xmlFunctionSubsort != " ") && (this.xmlFunctionSubsort != "")) {
                infovar.FunctionSubcategory = this.xmlFunctionSubsort;
            }     
            infovar.GUIDs.AddRange(this.objectGUID);

            for (int i = 0; i < infovar.GUIDs.Count(); i++)
            {
            }
            return infovar;
		}

		public SimsPackage readOBJDchunk(DecryptByteStream readFile)
		{
            packageTypes = new string[0];
			xmlCatalogSortTypes = new string[0];
			xmlSubtypes = new string[0];
			xmlCategoryTypes = new string[0];
			xmlType = "";
			xmlCategory = "";
			xmlSubtype = "";
			xmlModelName = "";
			xmlAge = "";
			xmlGender = "";
			xmlCatalog = "";
			objectGUID = new List<string>();
			xmlCreator = "";
			xmlFunction = "";
			xmlFunctionSubsort = "";
			title = "";
			description = "";
			pkgType = "";
			pkgTypeInt = 0;
			SimsPackage infovar = new SimsPackage();
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
                /*int fsfn = 0;
                foreach (var fsf in functionSortFlags){
                    fsfn++;
                }*/
                
                
                

				// No function sort, check Build Mode Sort
				if (functionSortFlag[0] == 0) 
				{
                    this.xmlCategory = "Build Item";
					// Skip until we hit the Build Mode sort and EP
					readFile.SkipAhead(46);
					uint expansionFlag = readFile.ReadUInt16();

					readFile.SkipAhead(8);
					uint buildModeType = readFile.ReadUInt16();
					string originalGUID = readFile.ReadUInt32().ToString("X8");
					string objectModelGUID = readFile.ReadUInt32().ToString("X8");
					uint buildModeSubsort = readFile.ReadUInt16();


                    int funcflags = functionSortFlags.Length;
                    foreach (FunctionSortList category in TypeListings.S2BuildFunctionSort){                            
                        if ((buildModeType == category.flagnum) && (buildModeSubsort == category.functionsubsortnum)) {
                            infovar.Function = category.Category;
                            infovar.FunctionSubcategory = category.Subcategory;
                        }                        
                    } 
				} 
				else 
				{
					// Set the xmlCategory to Object
                    this.xmlCategory = "Object";
					
                    //if (this.xmlCategory == null) this.xmlCategory = this.xmlCategoryTypes[1];
					// Also, get the catalog placement for this object
					readFile.SkipAhead(46);
					uint expansionFlag = readFile.ReadUInt16();
                    if (expansionFlag == 0){
                        //infovar.RequiredEPs.Add("None");
                    }

					readFile.SkipAhead(8);
					uint buildModeType = readFile.ReadUInt16();
					string originalGUID = readFile.ReadUInt32().ToString("X8");
					string objectModelGUID = readFile.ReadUInt32().ToString("X8");
					uint buildModeSubsort = readFile.ReadUInt16();
					readFile.SkipAhead(38);
					uint functionSubsort = readFile.ReadUInt16();

                    int funcflags = functionSortFlags.Length;
                    foreach (FunctionSortList category in TypeListings.S2BuyFunctionSort){
                        for (int f = 0; f < funcflags; f++){
                            if ((f == category.flagnum) && (functionSortFlags[f] == true) && (category.functionsubsortnum == functionSubsort)) {
                                this.xmlFunction = category.Category;
                                this.xmlFunctionSubsort = category.Subcategory;
                            }
                        }
                    }                    
				}
			}

            if ((this.xmlCategory != " ") && (this.xmlCategory != "")) {
                infovar.Category = this.xmlCategory;
            }
            if ((this.xmlFunction != " ") && (this.xmlFunction != "")) {
                infovar.Function = this.xmlFunction;
            }
            if ((this.xmlFunctionSubsort != " ") && (this.xmlFunctionSubsort != "")) {
                infovar.FunctionSubcategory = this.xmlFunctionSubsort;
            }     
            infovar.GUIDs.AddRange(this.objectGUID);

            for (int i = 0; i < infovar.GUIDs.Count(); i++)
            {
            }
            return infovar;
		}

		public SimsPackage readSHPEchunk(BinaryReader readFile)
		{
			SimsPackage thisPackage = new SimsPackage();
			string version = readFile.ReadUInt32().ToString("X8");
			if (this.debugMode) Console.WriteLine(version);
			if (version == "FFFF0001") 
			{
				uint numFileLinks = readFile.ReadUInt32();
				for (int i = 0; i < numFileLinks; i++)
				{
					string groupID = readFile.ReadUInt32().ToString("X8");
					string instanceID = readFile.ReadUInt32().ToString("X8");
					string resourceID = readFile.ReadUInt32().ToString("X8");
					string typeID = readFile.ReadUInt32().ToString("X8");

					if (this.debugMode) Console.WriteLine(groupID + " " + instanceID + " " + resourceID + " " + typeID);
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

					if (this.debugMode) Console.WriteLine(groupID + " " + instanceID + " " + resourceID + " " + typeID);
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
					if (this.debugMode) Console.WriteLine("Material count: " + shpeMatCount);
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
								if (thisPackage.Type == "") thisPackage.Type = shpePartType + "Mesh";
								break;
							default:
								//addModelName = true;
								if ((shpePartType.IndexOf("accessory") > -1) || (shpePartDesc.IndexOf("accessory") > -1)) thisPackage.Type = "frameMesh";
								break;
						}

						if (addModelName == true)
						{
							TagsList modelName = new TagsList();
							if (shpePartDesc.IndexOf("!") > -1) { modelName.Description = shpePartDesc.Substring(shpePartDesc.IndexOf("!")+1); } 
							else { modelName.Description = shpePartDesc; }
							thisPackage.CatalogTags.Add(modelName);
							modelName = null;
						}

					}
				}
			}
			return thisPackage;
		}

		public SimsPackage readSHPEchunk(DecryptByteStream readFile)
		{
			SimsPackage thisPackage = new SimsPackage();
			string version = readFile.ReadUInt32().ToString("X8");
			if (this.debugMode) Console.WriteLine(version);
			if (version == "FFFF0001") 
			{
				uint numFileLinks = readFile.ReadUInt32();
				for (int i = 0; i < numFileLinks; i++)
				{
					string groupID = readFile.ReadUInt32().ToString("X8");
					string instanceID = readFile.ReadUInt32().ToString("X8");
					string resourceID = readFile.ReadUInt32().ToString("X8");
					string typeID = readFile.ReadUInt32().ToString("X8");

					if (this.debugMode) Console.WriteLine(groupID + " " + instanceID + " " + resourceID + " " + typeID);
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

					if (this.debugMode) Console.WriteLine(groupID + " " + instanceID + " " + resourceID + " " + typeID);
				}
			}

			uint itemCount = readFile.ReadUInt32();
			for (int j = 0; j < itemCount; j++)
			{
				string rcolID = readFile.ReadUInt32().ToString("X8");
				if (this.debugMode) Console.WriteLine(rcolID);
			}

			for (int k = 0; k < itemCount; k++)
			{
				string blockName = readFile.ReadString();
				if (this.debugMode) Console.WriteLine(blockName);
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
								if (thisPackage.Type == "") thisPackage.Type = shpePartType + "Mesh";
								break;
							default:
								//addModelName = true;
								if ((shpePartType.IndexOf("accessory") > -1) || (shpePartDesc.IndexOf("accessory") > -1)) thisPackage.Type = "frameMesh";
								break;
						}

						if (addModelName == true)
						{
							TagsList modelName = new TagsList();
							if (shpePartDesc.IndexOf("!") > -1) { modelName.Description = shpePartDesc.Substring(shpePartDesc.IndexOf("!")+1); } 
							else { modelName.Description = shpePartDesc; }
							thisPackage.CatalogTags.Add(modelName);
							modelName = null;
						}

					}
				}
			}
			return thisPackage;
		}









    }

	public class S4Decryption {
		public static Stream Decompress(byte[] data)
		{
			var outputStream = new MemoryStream();
			using (var compressedStream = new MemoryStream(data))
			using (var inputStream = new InflaterInputStream(compressedStream))
			{
				inputStream.CopyTo(outputStream);
				outputStream.Position = 0;
				return outputStream;
			}
		}
	}
}