/* 
    Credits:
        - WandaSoule who helped me crack getting into the S4 entries of it all. 
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections;
using System.Xml;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using SSAGlobals;
using SimsCCManager.Packages.Containers;
using SimsCCManager.Packages.Decryption;
using System.Data.SQLite;
using System.Windows.Forms;

namespace SimsCCManager.Packages.Sims4Search
{
    public static class extensions {
        /// <summary>
        /// Adds an incremental extension for Dictionaries, so that each time an item is added that already exists, rather than simply rejecting that item, it increments its associated int by one.
        /// </summary>
        public static void Increment<T>(this Dictionary<T, int> dictionary, T key)
        {
            int count;
            dictionary.TryGetValue(key, out count);
            dictionary[key] = count + 1;
        }
    }

    /*
            None = 0x0000,
            Zlib = 0x5A42,
            RefPack = 0xFFFF
    */

    public class EntryHolder {
        public int[] entry {get; set;}
        public IReadOnlyList<int> header {get; set;}        
    }

    public struct ResourceKeyITG {
        /// <summary>
        /// Reads resource keys and does not flip the instance ID.
        /// </summary>
        LoggingGlobals log = new LoggingGlobals();
        public ulong instance;
        public uint type;
        public uint group;
        
        public ResourceKeyITG(BinaryReader reader, StreamWriter logging){
            this.instance = reader.ReadUInt64(); 
            logging.WriteLine("GUID Instance: " + this.instance, true);
            this.type = reader.ReadUInt32(); 
            logging.WriteLine("GUID Type: " + this.type, true);
            this.group = reader.ReadUInt32();     
            logging.WriteLine("GUID Group: " + this.group, true);  
        }

        public override string ToString() => $"{type.ToString("X8")}-{group.ToString("X8")}-{instance.ToString("X16")}";
        
    }
    public struct ResourceKeyITGFlip {
        /// <summary>
        /// Reads resource keys and flips the instance ID.
        /// </summary>
        LoggingGlobals log = new LoggingGlobals();
        public ulong instance;
        public uint type;
        public uint group;
        
        public ResourceKeyITGFlip(BinaryReader reader, StreamWriter logging){
            uint left = reader.ReadUInt32();
            uint right = reader.ReadUInt32();
            ulong longleft = left;
            longleft = (longleft << 32);
            this.instance = longleft | right;
            logging.WriteLine("GUID Instance: " + this.instance, true);
            this.type = reader.ReadUInt32(); 
            logging.WriteLine("GUID Type: " + this.type, true);
            this.group = reader.ReadUInt32();  
            logging.WriteLine("GUID Group: " + this.group, true);       
        }

        public override string ToString() => $"{type.ToString("X8")}-{group.ToString("X8")}-{instance.ToString("X16")}";
    }

    public struct Tag {
        /// <summary>
        /// Reads tags.
        /// </summary>
        public int[] tagKey;  
        public int[] empty; 

        public Tag(BinaryReader reader, uint count){
            if (count == 1){
                this.tagKey = new int[count];
                this.empty = new int[count];
                this.tagKey[0] = reader.ReadUInt16();
                this.empty[0] = 0;
            } else {
                this.tagKey = new int[count];
                this.empty = new int[count];
                for (int i = 0; i < count; i++){
                    this.tagKey[i] = reader.ReadUInt16(); 
                    this.empty[i] = reader.ReadUInt16(); 
                }
            }
        }
    }
    public struct CASTag16Bit {
        /// <summary>
        /// Gets CAS Tags from package. 16bit version.
        /// </summary>
        public int[] tagKey;  
        public int[] catKey;
        public int[] empty; 

        public CASTag16Bit(BinaryReader reader, uint count){
            if (count == 1){
                this.tagKey = new int[count];
                this.catKey = new int[count];
                this.empty = new int[count];
                this.empty[0] = reader.ReadUInt16();
                this.catKey[0] = reader.ReadUInt16();
                this.tagKey[0] = reader.ReadUInt16();
                
            } else {
                this.tagKey = new int[count];
                this.catKey = new int[count];
                this.empty = new int[count];
                for (int i = 0; i < count; i++){
                    this.empty[i] = reader.ReadUInt16();
                    this.catKey[i] = reader.ReadUInt16(); 
                    this.tagKey[i] = reader.ReadUInt16(); 
                     
                }
            }
        }
    }

    public struct CASTag32Bit {
        /// <summary>
        /// Gets CAS Tags from package. 32bit version.
        /// </summary>
        public uint[] tagKey;  
        public uint[] catKey;
        public uint[] empty; 

        public CASTag32Bit(BinaryReader reader, int count){
            if (count == 1){
                this.tagKey = new uint[count];
                this.catKey = new uint[count];
                this.empty = new uint[count];
                this.empty[0] = reader.ReadUInt32();
                this.catKey[0] = reader.ReadUInt32();
                this.tagKey[0] = reader.ReadUInt32();
                
            } else {
                this.tagKey = new uint[count];
                this.catKey = new uint[count];
                this.empty = new uint[count];
                for (int i = 0; i < count; i++){
                    this.empty[i] = reader.ReadUInt32();
                    this.catKey[i] = reader.ReadUInt32(); 
                    this.tagKey[i] = reader.ReadUInt32(); 
                     
                }
            }
        }
    }

    public struct ReadCOBJ{
        /// <summary>
        /// Reads COBJD entries.
        /// </summary>
        public string GUID = "null";
        public List<TagsList> itemtags = new List<TagsList>();
        LoggingGlobals log = new LoggingGlobals();
        GlobalVariables globals = new GlobalVariables();

        public ReadCOBJ(BinaryReader readFile, int packageparsecount, int e, List<TagsList> itemtags, StreamWriter logging){
            uint version = readFile.ReadUInt32();
            logging.WriteLine("P" + packageparsecount + "/E" + e + " - Version: " + version, true);
            logging.WriteLine("-- As hex: " + version.ToString("X8"), true);
            uint commonblockversion = readFile.ReadUInt32();
            logging.WriteLine("P" + packageparsecount + "/E" + e + " - Common Block Version: " + commonblockversion, true);
            logging.WriteLine("-- As hex: " + commonblockversion.ToString("X8"), true);
            uint namehash = readFile.ReadUInt32();
            logging.WriteLine("P" + packageparsecount + "/E" + e + " - NameHash: " + namehash, true);
            logging.WriteLine("-- As hex: " + namehash.ToString("X8"), true);
            uint descriptionhash = readFile.ReadUInt32();
            logging.WriteLine("P" + packageparsecount + "/E" + e + " - DescriptionHash: " + descriptionhash, true);
            logging.WriteLine("-- As hex: " + descriptionhash.ToString("X8"), true);                            
            uint price = readFile.ReadUInt32();
            logging.WriteLine("P" + packageparsecount + "/E" + e + " - Price: " + price, true);
            logging.WriteLine("-- As hex: " + price.ToString("X8"), true);

            ulong thumbhash = readFile.ReadUInt64();
            logging.WriteLine("P" + packageparsecount + "/E" + e + " - Thumbnail Hash: " + thumbhash, true);
            logging.WriteLine("-- As hex: " + thumbhash.ToString("X8"), true);

            uint devcatflags = readFile.ReadUInt32();
            logging.WriteLine("P" + packageparsecount + "/E" + e + " - Dev Category Flags: " + devcatflags, true);
            logging.WriteLine("-- As hex: " + devcatflags.ToString("X8"), true);
            
            int tgicount = readFile.ReadByte();
            logging.WriteLine("P" + packageparsecount + "/E" + e + " - TGI Count: " + tgicount, true);

            if (tgicount != 0){
                logging.WriteLine("P" + packageparsecount + "/E" + e + " - TGI Count is not zero. Reading resources.", true);
                ResourceKeyITG resourcekey = new ResourceKeyITG(readFile, logging);
                logging.WriteLine(resourcekey.ToString(), true);
                logging.WriteLine("GUID: " + resourcekey.ToString(), true);
                GUID = resourcekey.ToString();
            }                           
            
            if (commonblockversion >= 10)
            {
                int packId = readFile.ReadInt16();
                logging.WriteLine("P" + packageparsecount + "/E" + e + " - Pack ID: " + packId, true);
                int packFlags = readFile.ReadByte();
                logging.WriteLine("P" + packageparsecount + "/E" + e + " - Pack Flags: " + packFlags, true);
                byte[] reservedBytes = readFile.ReadBytes(9);
            } else {
                int unused2 = readFile.ReadByte();
                if (unused2 > 0)
                {
                    int unused3 = readFile.ReadByte();
                }
            }

            if (commonblockversion >= 11){
                uint count = readFile.ReadUInt32();
                logging.WriteLine("P" + packageparsecount + "/E" + e + " - Tags Count: " + count, true);
                Tag tags = new Tag(readFile, count);
                for (int i = 0; i < count; i++){
                    logging.WriteLine("P" + packageparsecount + "/E" + e + " - Tag " + i + " value is: " + tags.tagKey[i], true);
                    
                    if (tags.tagKey[i] != 0){
                        var tagKeyexists = from ID in TypeListings.S4BBFunctionTags
                        where ID.typeID == tags.tagKey[i].ToString()
                        select ID.typeID;
                        bool tagfound = tagKeyexists.Any();
                        if (tagfound == true){
                            logging.WriteLine("P" + packageparsecount + "/E" + e + " - TagKey " + tags.tagKey[i] + " exists in database.", true);
                            foreach (typeList item in TypeListings.S4BBFunctionTags){
                                if(item.typeID == tags.tagKey[i].ToString()){
                                    if ((itemtags.Exists(x => x.shortval == (short)tags.tagKey[i])) || (itemtags.Exists(x => x.stringval == tags.tagKey[i].ToString()))){
                                        
                                    } else {
                                        itemtags.Add(new TagsList{ shortval = (short)tags.tagKey[i], stringval = item.info});
                                        logging.WriteLine("Tag " + i + " matched to " + item.info, true);
                                    }
                                }
                            }
                        } else {
                            itemtags.Add(new TagsList{ shortval = (short)tags.tagKey[i], stringval = "Needs Identification"});
                            logging.WriteLine("Tag " + i + " has no match.", true);
                        }
                    }
                }
            } else {
                uint count = readFile.ReadUInt32();
                logging.WriteLine("Num tags: " + count, true);
                for (int t = 0; t < count; t++){
                    uint tagvalue = readFile.ReadUInt16();
                    if (tagvalue != 0){
                        var tagKeyexists = from ID in TypeListings.S4BBFunctionTags
                        where ID.typeID == tagvalue.ToString()
                        select ID.typeID;
                        bool tagfound = tagKeyexists.Any();
                        if (tagfound == true){
                            logging.WriteLine("P" + packageparsecount + "/E" + e + " - TagKey " + tagvalue + " exists in database.", true);
                            foreach (typeList item in TypeListings.S4BBFunctionTags){
                                if(item.typeID == tagvalue.ToString()){
                                    if ((itemtags.Exists(x => x.shortval == (short)tagvalue)) || (itemtags.Exists(x => x.stringval == tagvalue.ToString()))){
                                        
                                    } else {
                                        itemtags.Add(new TagsList{ shortval = (short)tagvalue, stringval = item.info});
                                        logging.WriteLine("Tag " + t + " matched to " + item.info, true);
                                    }
                                }
                            }
                        } else {
                            itemtags.Add(new TagsList{ shortval = (short)tagvalue, stringval = "Needs Identification"});
                            logging.WriteLine("Tag " + t + " has no match.", true);
                        }
                    }
                }
            }
            long location = readFile.BaseStream.Position;
            logging.WriteLine("P" + packageparsecount + "/E" + e + " - Reader location: " + location, true);
            uint count2 = readFile.ReadUInt32();
            logging.WriteLine("P" + packageparsecount + "/E" + e + " - Selling Point Count: " + count2, true);
            if (count2 > 5){
                logging.WriteLine("Selling point count is too high, something went wrong.", true);
            } else {
                    Tag sellingtags = new Tag(readFile, count2);
                for (int i = 0; i < count2; i++){
                    logging.WriteLine("P" + packageparsecount + "/E" + e + " - Tag " + i + " value is: " + sellingtags.tagKey[i], true);
                    
                    if (sellingtags.tagKey[i] != 0){
                        var tagKeyexists = from ID in TypeListings.S4BBFunctionTags
                        where ID.typeID == sellingtags.tagKey[i].ToString()
                        select ID.typeID;
                        bool tagfound = tagKeyexists.Any();
                        if (tagfound == true){
                            logging.WriteLine("P" + packageparsecount + "/E" + e + " - TagKey " + sellingtags.tagKey[i] + " exists in database.", true);
                            foreach (typeList item in TypeListings.S4BBFunctionTags){
                                if(item.typeID == sellingtags.tagKey[i].ToString()){
                                    if ((itemtags.Exists(x => x.shortval == (short)sellingtags.tagKey[i])) || (itemtags.Exists(x => x.stringval == sellingtags.tagKey[i].ToString()))){
                                        
                                    } else {
                                        itemtags.Add(new TagsList{ shortval = (short)sellingtags.tagKey[i], stringval = item.info});
                                        logging.WriteLine("Tag " + i + " matched to " + item.info, true);
                                    }
                                }
                            }
                        } else {
                            itemtags.Add(new TagsList{ shortval = (short)sellingtags.tagKey[i], stringval = "Needs Identification"});
                            logging.WriteLine("Tag " + i + " has no match.", true);
                        }
                    }

                }
            }            

            uint unlockByHash = readFile.ReadUInt32();
            logging.WriteLine("P" + packageparsecount + "/E" + e + " - UnlockBy Hash: " + unlockByHash, true);
            
            uint unlockedByHash = readFile.ReadUInt32();
            logging.WriteLine("P" + packageparsecount + "/E" + e + " - UnlockedBy Hash: " + unlockedByHash, true);

            int swatchColorSortPriority = readFile.ReadUInt16();
            logging.WriteLine("P" + packageparsecount + "/E" + e + " - Swatch Sort Priority: " + swatchColorSortPriority, true);

            ulong varientThumbImageHash = readFile.ReadUInt64();
            logging.WriteLine("P" + packageparsecount + "/E" + e + " - Varient Thumb Image Hash: " + varientThumbImageHash, true);
        }            
    }
    
    public struct ReadOBJDIndex {
        /// <summary>
        /// Retrieves OBJD index.
        /// </summary>
        LoggingGlobals log = new LoggingGlobals();
        public int version;
        public uint refposition;
        public int count;
        public uint[] entrytype;
        public uint[] position;

        public ReadOBJDIndex(BinaryReader reader, StreamWriter logging){
            this.version = reader.ReadUInt16();
            logging.WriteLine("Version: " + this.version, true);
            if (this.version > 150){
                logging.WriteLine("Version is not legitimate.", true);
                this.refposition = 0; 
                this.count = (int)0;
                this.entrytype = new uint[0];
                this.position = new uint[0];
            } else {
                this.refposition = reader.ReadUInt32();
                reader.BaseStream.Position = refposition;
                this.count = reader.ReadUInt16();
                this.entrytype = new uint[count];
                this.position = new uint[count];
                for (int i = 0; i < count; i++){
                    this.entrytype[i] = reader.ReadUInt32();
                    this.position[i] = reader.ReadUInt32();
                }
            }   
        }
    }

    public struct ReadOBJDEntry {
        /// <summary>
        /// Reads OBJD entries.
        /// </summary>
        LoggingGlobals log = new LoggingGlobals();
        public int namelength;
        public byte[] namebit;
        public string name;
        
        public int tuningnamelength;
        public byte[] tuningbit;
        public string tuningname;        
        public ulong tuningid;
        public uint componentcount;
        public uint[] components;        
        public int materialvariantlength;
        public byte[] materialvariantbyte;
        public string materialvariant;
        public uint price;
        public string[] icon;
        public string[] rig;
        public string[] slot;
        public string[] model;
        public string[] footprint;
        private bool tuningidmissing = false;
        public ReadOBJDEntry(BinaryReader reader, string[] entries, int[] positions, StreamWriter logging){
            uint preceeding;
            uint preceedingDiv;

            namelength = 0;
            namebit = new byte[0];
            name = "";
            tuningnamelength = 0;
            tuningbit = new byte[0];
            tuningname = "";
            tuningid = 0;
            componentcount = 0;
            components = new uint[0];
            materialvariantlength = 0;
            materialvariantbyte = new byte[0];
            materialvariant = "";
            price = 0;
            icon = new string[0];
            rig = new string[0];
            slot = new string[0];
            model = new string[0];
            footprint = new string[0];
            tuningidmissing = false;

            for (int e = 0; e < entries.Length; e++){
                string entryid = entries[e];
                int entrypos = positions[e];
                switch (entryid)
                {
                    case "E7F07786": // name
                        reader.BaseStream.Position = entrypos;
                        this.namelength = reader.ReadByte();
                        logging.WriteLine("Name Length: " + namelength, true);
                        logging.WriteLine("Reading three empty bytes.", true);
                        logging.WriteLine("Byte 1: " + reader.ReadByte().ToString(), true);
                        logging.WriteLine("Byte 2: " + reader.ReadByte().ToString(), true);
                        logging.WriteLine("Byte 3: " + reader.ReadByte().ToString(), true);
                        this.namebit = reader.ReadBytes(namelength);
                        this.name = Encoding.UTF8.GetString(namebit);
                        logging.WriteLine("Name: " + name, true);
                        break;
                    case "790FA4BC": //tuning
                        reader.BaseStream.Position = entrypos;
                        this.tuningnamelength = reader.ReadByte();
                        logging.WriteLine("Tuning Name Length: " + tuningnamelength, true);
                        logging.WriteLine("Reading three empty bytes.", true);
                        logging.WriteLine("Byte 1: " + reader.ReadByte().ToString(), true);
                        logging.WriteLine("Byte 2: " + reader.ReadByte().ToString(), true);
                        logging.WriteLine("Byte 3: " + reader.ReadByte().ToString(), true);
                        this.tuningbit = reader.ReadBytes(tuningnamelength);
                        this.tuningname = Encoding.UTF8.GetString(tuningbit);
                        logging.WriteLine("Tuning Name: " + tuningname, true);
                        break;
                    case "B994039B": //TuningID
                        reader.BaseStream.Position = entrypos;
                        this.tuningid = reader.ReadUInt64(); 
                        break;
                    case "CADED888": //Icon
                        reader.BaseStream.Position = entrypos;
                        preceeding = reader.ReadUInt32();
                        logging.WriteLine("Reading preceeding UInt32: " + preceeding, true);
                        preceedingDiv = preceeding / 4;            
                        logging.WriteLine("Number of Icon GUIDs: " + preceedingDiv, true);
                        this.icon = new string[preceedingDiv];
                        for (int p = 0; p < preceedingDiv; p++){
                            ResourceKeyITGFlip ricon = new ResourceKeyITGFlip(reader, logging);
                            logging.WriteLine(ricon.ToString(), true);
                            logging.WriteLine("Icon GUID: " + ricon.ToString(), true);
                            this.icon[p] = ricon.ToString();
                        }
                        break;
                    case "E206AE4F": //Rig
                        reader.BaseStream.Position = entrypos;
                        preceeding = reader.ReadUInt32();
                        logging.WriteLine("Reading preceeding UInt32: " + preceeding, true);
                        preceedingDiv = preceeding / 4;
                        logging.WriteLine("Number of Rig GUIDs: " + preceedingDiv, true);
                        this.rig = new string[preceedingDiv];
                        for (int p = 0; p < preceedingDiv; p++){
                            ResourceKeyITGFlip rkrig = new ResourceKeyITGFlip(reader, logging);
                            logging.WriteLine(rkrig.ToString(), true);
                            logging.WriteLine("Rig GUID: " + rkrig.ToString(), true);
                            this.rig[p] = rkrig.ToString();
                        }   
                        break;
                    case "8A85AFF3": //Slot
                        reader.BaseStream.Position = entrypos;
                        preceeding = reader.ReadUInt32();
                        logging.WriteLine("Reading preceeding UInt32: " + preceeding, true);
                        preceedingDiv = preceeding / 4;
                        logging.WriteLine("Number of Slot GUIDs: " + preceedingDiv, true);
                        this.slot = new string[preceedingDiv];
                        for (int p = 0; p < preceedingDiv; p++){
                            ResourceKeyITGFlip rkslot = new ResourceKeyITGFlip(reader, logging);
                            logging.WriteLine(rkslot.ToString(), true);
                            logging.WriteLine("Slot GUID: " + rkslot.ToString(), true);
                            this.slot[p] = rkslot.ToString();
                        }            
                        break;
                    case "8D20ACC6": //Model
                        reader.BaseStream.Position = entrypos;
                        preceeding = reader.ReadUInt32();
                        logging.WriteLine("Reading preceeding UInt32: " + preceeding, true);
                        preceedingDiv = preceeding / 4;
                        logging.WriteLine("Number of Model GUIDs: " + preceedingDiv, true);
                        this.model = new string[preceedingDiv];
                        for (int p = 0; p < preceedingDiv; p++){
                            ResourceKeyITGFlip rkmodel = new ResourceKeyITGFlip(reader, logging);
                            logging.WriteLine(rkmodel.ToString(), true);
                            logging.WriteLine("Reader is at " + reader.BaseStream.Position, true);
                            logging.WriteLine("Model GUID: " + rkmodel.ToString(), true);
                            this.model[p] = rkmodel.ToString();
                        }
                        break;
                    case "6C737AD8": //Footprint
                        reader.BaseStream.Position = entrypos;
                        preceeding = reader.ReadUInt32();
                        logging.WriteLine("Reading preceeding UInt32: " + preceeding, true);
                        preceedingDiv = preceeding / 4;
                        logging.WriteLine("Number of Footprint GUIDs: " + preceedingDiv, true);
                        this.footprint = new string[preceedingDiv];
                        for (int p = 0; p < preceedingDiv; p++){
                            ResourceKeyITGFlip rkft = new ResourceKeyITGFlip(reader, logging);
                            logging.WriteLine(rkft.ToString(), true);
                            logging.WriteLine("Reader is at " + reader.BaseStream.Position, true);
                            logging.WriteLine("Footprint GUID: " + rkft.ToString(), true);
                            this.footprint[p] = rkft.ToString();                
                        }
                        break;
                    case "E6E421FB": //Components
                        reader.BaseStream.Position = entrypos;
                        this.componentcount = reader.ReadUInt32();
                        logging.WriteLine("Reader is at " + reader.BaseStream.Position, true);
                        logging.WriteLine("Component count: " + componentcount, true);
                        this.components = new uint[this.componentcount];
                        for (int i = 0; i < this.componentcount; i++){
                            components[i] = reader.ReadUInt32();
                        }
                        break;
                    case "ECD5A95F": //MaterialVariant
                        reader.BaseStream.Position = entrypos;
                        this.materialvariantlength = reader.ReadByte();
                        logging.WriteLine("Material Variant Length: " + materialvariantlength, true);
                        logging.WriteLine("Reading three empty bytes.", true);
                        logging.WriteLine("Byte 1: " + reader.ReadByte().ToString(), true);
                        logging.WriteLine("Byte 2: " + reader.ReadByte().ToString(), true);
                        logging.WriteLine("Byte 3: " + reader.ReadByte().ToString(), true);
                        this.materialvariantbyte = reader.ReadBytes(materialvariantlength);
                        this.materialvariant = Encoding.UTF8.GetString(materialvariantbyte);
                        logging.WriteLine("Material Variant: " + materialvariant, true);
                        break;
                    case "AC8E1BC0": //Unknown1
                        reader.BaseStream.Position = entrypos;

                        break;
                    case "E4F4FAA4": //SimoleonPrice
                        reader.BaseStream.Position = entrypos;
                        this.price = reader.ReadUInt32();
                        logging.WriteLine("Price: " + price, true);
                        break;
                    case "7236BEEA": //PositiveEnvironmentScore
                        reader.BaseStream.Position = entrypos;

                        break;
                    case "44FC7512": //NegativeEnvironmentScore
                        reader.BaseStream.Position = entrypos;

                        break;
                    case "4233F8A0": //ThumbnailGeometryState
                        reader.BaseStream.Position = entrypos;

                        break;
                    case "EC3712E6": //Unknown2
                        reader.BaseStream.Position = entrypos;

                        break;
                    case "2172AEBE": //EnvironmentScoreEmotionTags
                        reader.BaseStream.Position = entrypos;

                        break;
                    case "DCD08394": //EnvironmentScores
                        reader.BaseStream.Position = entrypos;

                        break;
                    case "52F7F4BC": //Unknown3
                        reader.BaseStream.Position = entrypos;

                        break;
                    case "AEE67A1C": //IsBaby
                        reader.BaseStream.Position = entrypos;

                        break;
                    case "F3936A90": //Unknown4
                        reader.BaseStream.Position = entrypos;
                        
                        break;
                }
            }
        }

        public override string ToString(){
            return string.Format("Name: {0} \n Tuning Name: {1} \n TuningID: {2} \n Components: {3} \n Material Variant: {4} \n Price: {5} \n Icon: {6} \n Rig: {7}, Model: {8}, Slot: {9}, Footprint: {10}", this.name, this.tuningname, this.tuningid.ToString("X16"), GetFormatUIntArray(this.components), this.materialvariant, this.price.ToString("X8"), this.icon, this.rig, this.model, this.slot, this.footprint);
        }

        public static string GetFormatUIntArray(uint[] ints){
            string retVal = string.Empty;
            foreach (uint number in ints){
                if (string.IsNullOrEmpty(retVal)){
                    retVal += number.ToString("X8");
                } else {
                    retVal += string.Format(", {0}", number.ToString("X8"));
                }
                
            }
            return retVal;
        }
    }

    public struct Components {
        /// <summary>
        /// Retrieves "components" from a file.
        /// </summary>
        public int[] component;  
        public int[] empty; 

        public Components(BinaryReader reader, uint count){
            if (count == 1){
                this.component = new int[count];
                this.empty = new int[count];
                this.component[0] = reader.ReadUInt16();
                this.empty[0] = 0;
            } else {
                this.component = new int[count];
                this.empty = new int[count];
                for (int i = 0; i < count; i++){
                    this.component[i] = reader.ReadUInt16(); 
                    this.empty[i] = reader.ReadUInt16(); 
                }
            }
        }
    }


    public class indexEntry
    {
        public string typeID;
        public string groupID;
        public string instanceID;
        public string instanceID2;
        public uint offset;
        public long position;
        public uint fileSize;
        public uint memSize;
        
        public uint filesize;
        public uint truesize;
        public bool compressed;
        public string unused;
        public string compressionType;
        
        

    }	
    class S4PackageSearch
    {
        /// <summary>
        /// Sims 4 package reading. Gets all the information from inside S4 Package files and returns it for use.
        /// </summary>
        
        // Class References
        LoggingGlobals log = new LoggingGlobals();
        ReadEntries readentries = new ReadEntries();   
        System.Text.Encoding encoding = System.Text.Encoding.BigEndianUnicode;
        GlobalVariables globals = new GlobalVariables();

        //lists

        public static bool hasrunbefore;       

        //Vars
        uint chunkOffset = 0;
        int contentposition = 64;
        int contentpositionalt = 40;
        int contentcount = 36;
        public string[] parameters = {"DefaultForBodyType","DefaultThumbnailPart","AllowForCASRandom","ShowInUI","ShowInSimInfoPanel","ShowInCasDemo","AllowForLiveRandom","DisableForOppositeGender","DisableForOppositeFrame","DefaultForBodyTypeMale","DefaultForBodyTypeFemale","Unk","Unk","Unk","Unk","Unk"};
        

        public void SearchS4Packages(string file, bool dump) {
            string packagelogfolder = "packagelogs";
            FileInfo packageinfo = new FileInfo(file);
            string logname = string.Format("{0}.packagelog", Path.GetFileNameWithoutExtension(packageinfo.Name));
            string logspot = Path.Combine(LoggingGlobals.internalLogFolder, packagelogfolder);
            string logfile = Path.Combine(logspot, logname);
            StreamWriter logging = new StreamWriter(logfile);
            logging.WriteLine(string.Format("File {0} arrived for processing as Sims 4 file.", packageinfo.Name), true);
            var txt = string.Format("SELECT * FROM Processing_Reader where Name='{0}'", packageinfo.Name);
            var queries = GlobalVariables.DatabaseConnection.Query<PackageFile>(txt);
            var query = queries[0];
            GlobalVariables.DatabaseConnection.Delete(query);
            var pk = new PackageFile { ID = query.ID, Name = packageinfo.Name, Location = packageinfo.FullName, Game = 4, Broken = false, Status = "Processing"};
            GlobalVariables.DatabaseConnection.Update(pk);
            var packageparsecount = GlobalVariables.packagesRead;   
            logging.WriteLine("Got package parse count.", true);
            GlobalVariables.packagesRead++;       
            logging.WriteLine("Incrementing packages read.", true);
                     
        
            //Misc Vars
            string test = "";
            
            //locations

            byte[] entrycountloc = new byte[36];
            byte[] indexRecordPositionloc = new byte[64];

            SimsPackage thisPackage = new SimsPackage();          

            //Lists 
                
            List<TagsList> itemtags = new List<TagsList>();
            List<TagsList> distinctItemTags = new List<TagsList>();
            List<string> allFlags = new List<string>();      
            List<string> distinctFlags = new List<string>(); 
            List<string> allGUIDS = new List<string>();      
            List<string> distinctGUIDS = new List<string>();  
            List<string> allInstanceIDs = new List<string>();      
            List<string> distinctInstanceIDs = new List<string>();           
            string[] objdentries;
            int[] objdpositions;   
            List<EntryHolder> entries = new List<EntryHolder>();
            List<fileHasList> fileHas = new List<fileHasList>();
            ArrayList linkData = new ArrayList();
            List<indexEntry> indexData = new List<indexEntry>();
            SimsPackage DataDelivery = new SimsPackage();

            itemtags = new List<TagsList>();
            distinctItemTags = new List<TagsList>();

            //create readers  
            byte[] filebyte = File.ReadAllBytes(packageinfo.FullName);
            MemoryStream dbpfFile = new MemoryStream(filebyte);
            BinaryReader readFile = new BinaryReader(dbpfFile);
            
            //log opening file
            //string cwl = string.Format("Reading package # {0}/{1}: {3}", packageparsecount, GlobalVariables.PackageCount, packageinfo.Name);
            //Console.WriteLine(cwl);
            thisPackage.PackageName = packageinfo.Name;
            thisPackage.Location = packageinfo.FullName;            
            thisPackage.Game = 4;
            logging.WriteLine(string.Format("Package #{0} registered as {1} and meant for Sims 4", packageparsecount, packageinfo.FullName), true);

            //start actually reading the package 
            
            /*
            //dbpf
            test = Encoding.ASCII.GetString(readFile.ReadBytes(4)); 
            logging.WriteLine("DBPF: " + test, true);
            logging.WriteLine("DBPF Location: " + readFile.BaseStream.Position, true);
            
            //major
            uint testint = readFile.ReadUInt32(); 
            test = testint.ToString();
            logging.WriteLine("Major :" + test, true);
            logging.WriteLine("Major Location: " + readFile.BaseStream.Position, true);
            
            //minor
            testint = readFile.ReadUInt32(); 
            test = testint.ToString();
            logging.WriteLine("Minor : " + test, true);
            logging.WriteLine("Minor Location: " + readFile.BaseStream.Position, true);
            
            testint = readFile.ReadUInt32(); 
            test = testint.ToString();
            logging.WriteLine("Unknown: " + test, true);
            logging.WriteLine("Unknown1 Location: " + readFile.BaseStream.Position, true);
            
            testint = readFile.ReadUInt32(); 
            test = testint.ToString();
            logging.WriteLine("Unknown: " + test, true);
            logging.WriteLine("Unknown2 Location: " + readFile.BaseStream.Position, true);
            
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            logging.WriteLine("Unknown : " + test, true);
            logging.WriteLine("Unknown3 Location: " + readFile.BaseStream.Position, true);
            
            testint = readFile.ReadUInt32(); 
            test = testint.ToString();
            logging.WriteLine("Created : " + test, true);
            logging.WriteLine("Created Location: " + readFile.BaseStream.Position, true);

            testint = readFile.ReadUInt32();
            test = testint.ToString();
            logging.WriteLine("Modified : " + test, true);
            logging.WriteLine("Modified Location: " + readFile.BaseStream.Position, true);
            
            testint = readFile.ReadUInt32(); 
            test = testint.ToString();
            logging.WriteLine("Index Major : " + test, true);
            logging.WriteLine("Index Major Location: " + readFile.BaseStream.Position, true);
            */
            //entrycount
            

            readFile.BaseStream.Position = entrycountloc.Length;

            uint entrycount = readFile.ReadUInt32();
            logging.WriteLine(string.Format("Entry Count: {0}", entrycount.ToString()), true);
            logging.WriteLine("Entry Count Location: " + readFile.BaseStream.Position, true);
            
            //record position low
            uint indexRecordPositionLow = readFile.ReadUInt32();
            logging.WriteLine(string.Format("IndexRecordPositionLow: {0}", indexRecordPositionLow.ToString()), true);
            logging.WriteLine("IndexRecordPositionLow Location: " + readFile.BaseStream.Position, true);
            
            //index record size
            uint indexRecordSize = readFile.ReadUInt32();
            logging.WriteLine(string.Format("IndexRecordSize: {0}", indexRecordSize.ToString()), true);
            logging.WriteLine("IndexRecordSize Location: " + readFile.BaseStream.Position, true);
            /*
            //unused
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            logging.WriteLine("Unused Trash Index offset: " + test, true);
            logging.WriteLine("Unused Trash Index offset Location: " + readFile.BaseStream.Position, true);
            
            //unused
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            logging.WriteLine("Unused Trash Index size: " + test, true);
            logging.WriteLine("Unused Trash Index size Location: " + readFile.BaseStream.Position, true);
            
            //unused
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            logging.WriteLine("Unused Index Minor Version: " + test, true);
            logging.WriteLine("Unused Index Minor Version Location: " + readFile.BaseStream.Position, true);
            
            //unused but 3 for historical reasons
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            logging.WriteLine("Unused, 3 for historical reasons: " + test, true);
            logging.WriteLine("Unused, 3 for historical reasons Location: " + readFile.BaseStream.Position, true);
            */
            readFile.BaseStream.Position = indexRecordPositionloc.Length;

            ulong indexRecordPosition = readFile.ReadUInt64();
            test = indexRecordPosition.ToString();
            logging.WriteLine("Index Record Position: " + test, true);
            logging.WriteLine("Index Record Position Location: " + readFile.BaseStream.Position, true);
            
            //unused
            //testint = readFile.ReadUInt32();
            //test = testint.ToString();
            //logging.WriteLine("Unused Unknown:" + test, true);
            //logging.WriteLine("Unused Unknown Location: " + readFile.BaseStream.Position, true);
            
            //unused six bytes
            //test = Encoding.ASCII.GetString(readFile.ReadBytes(24));
            //logging.WriteLine("Unused: " + test, true);
            //logging.WriteLine("Unused4 Location: " + readFile.BaseStream.Position, true);
            
            byte[] headersize = new byte[96];
            byte[] here = new byte[100];
            if (indexRecordPosition != 0){
                long indexseek = (long)indexRecordPosition - headersize.Length;
                readFile.BaseStream.Position = here.Length + indexseek;
            } else {
                readFile.BaseStream.Position = here.Length + indexRecordPositionLow;
            }
            
            byte[] movedhere = new byte[readFile.BaseStream.Position];
            

            for (int i = 0; i < entrycount; i++){
                indexEntry holderEntry = new indexEntry();                
                holderEntry.typeID = readFile.ReadUInt32().ToString("X8");
                logging.WriteLine("P" + packageparsecount + "/E" + i + " - Index Entry TypeID: " + holderEntry.typeID, true);

                if(TypeListings.AllTypesS4.Exists(x => x.typeID == holderEntry.typeID)){
                    foreach (typeList type in TypeListings.AllTypesS4){
                    if (type.typeID == holderEntry.typeID){
                            fileHas.Add(new fileHasList() { term = type.desc, location = i});
                        }
                    }
                } else {
                    fileHas.Add(new fileHasList() { term = holderEntry.typeID, location = i});
                }

                

                holderEntry.groupID = readFile.ReadUInt32().ToString("X8");
                logging.WriteLine("P" + packageparsecount + "/E" + i + " - Index Entry GroupID: " + holderEntry.groupID, true);
                
                string instanceid1 = (readFile.ReadUInt32() << 32).ToString("X8");
                string instanceid2 = (readFile.ReadUInt32() << 32).ToString("X8");
                holderEntry.instanceID = instanceid1 + instanceid2;
                allInstanceIDs.Add(holderEntry.instanceID);
                logging.WriteLine("P" + packageparsecount + "/E" + i + " - InstanceID: " + holderEntry.instanceID, true);

                uint testin = readFile.ReadUInt32();
                holderEntry.position = (long)testin;
                logging.WriteLine("P" + packageparsecount + "/E" + i + " - Position " + testin.ToString(), true);

                holderEntry.fileSize = readFile.ReadUInt32();

                logging.WriteLine("P" + packageparsecount + "/E" + i + " - File Size " + holderEntry.fileSize.ToString("X8"), true);

                holderEntry.memSize = readFile.ReadUInt32();
                logging.WriteLine("P" + packageparsecount + "/E" + i + " - Mem Size " + holderEntry.memSize.ToString("X8"), true);

                holderEntry.compressionType = readFile.ReadUInt16().ToString("X4");
                logging.WriteLine("P" + packageparsecount + "/E" + i + " - Compression Type " + holderEntry.compressionType, true);

                //readFile.ReadUInt16();
                //logging.WriteLine("P" + packageparsecount + "/E" + i + " - Confirmed: " + testint.ToString("X4"), true);

                readFile.BaseStream.Position = readFile.BaseStream.Position + 2;

                indexData.Add(holderEntry);

                holderEntry = null;
            }

            logging.WriteLine("This package contains: ", true);
            foreach (fileHasList type in fileHas){
                logging.WriteLine(type.term + " at location " + type.location, true);
            }

            if(fileHas.Exists(x => x.term == "S4SM")) {
                logging.WriteLine("P" + packageparsecount + ": " + thisPackage.PackageName + " is a Merged Package and cannot be processed until it has been unmerged.", true);
                thisPackage.Type = "Merged Package";
            } else {
                if ((fileHas.Exists(x => x.term == "CASP"))){
                    List<int> entryspots = new List<int>();
                    int fh = 0;
                    foreach (fileHasList item in fileHas) {
                        if (item.term == "CASP"){
                            entryspots.Add(fh);                       
                        }
                        fh++;
                    }    
                    int caspc = 0;
                    foreach (int e in entryspots){
                        logging.WriteLine("P" + packageparsecount + "/E" + e + " - Opening CASP #" + caspc, true);
                        if (indexData[e].compressionType == "5A42"){
                        readFile.BaseStream.Position = indexData[e].position;
                        long entryEnd = indexData[e].position + indexData[e].memSize;
                        logging.WriteLine("P" + packageparsecount + "/E" + e + " - Position: " + indexData[e].position, true);
                        logging.WriteLine("P" + packageparsecount + "/E" + e + " - Filesize: " + indexData[e].fileSize, true);
                        logging.WriteLine("P" + packageparsecount + "/E" + e + " - Memsize: " + indexData[e].memSize, true);
                        logging.WriteLine("P" + packageparsecount + "/E" + e + " - Entry ends at " + entryEnd, true);
                        byte[] entry = readFile.ReadBytes((int)indexData[e].memSize);
                        Stream decomps = S4Decryption.Decompress(entry);
                        
                        BinaryReader decompbr = new BinaryReader(decomps);

                        uint version = decompbr.ReadUInt32();
                        logging.WriteLine("P" + packageparsecount + "/E" + e + " - Version: " + version, true);
                        logging.WriteLine("-- As hex: " + version.ToString("X8"), true);
                        uint tgioffset = decompbr.ReadUInt32() +8;
                        logging.WriteLine("P" + packageparsecount + "/E" + e + " - TGI Offset: " + tgioffset, true);
                        logging.WriteLine("-- As hex: " + tgioffset.ToString("X8"), true);
                        uint numpresets = decompbr.ReadUInt32();
                        logging.WriteLine("P" + packageparsecount + "/E" + e + " - Number of Presets: " + numpresets, true);
                        logging.WriteLine("-- As hex: " + numpresets.ToString("X8"), true);
                        using (var reader = new BinaryReader(decomps, Encoding.BigEndianUnicode, true))
                        {
                            thisPackage.Title = reader.ReadString();
                        }                            
                        logging.WriteLine("P" + packageparsecount + "/E" + e + " - Name: " + thisPackage.Title, true);

                        if (dump == true){
                            distinctInstanceIDs = allInstanceIDs.Distinct().ToList();
                            thisPackage.InstanceIDs.AddRange(distinctInstanceIDs);
                            distinctGUIDS = allGUIDS.Distinct().ToList();
                            thisPackage.ObjectGUID.AddRange(distinctGUIDS);
                            MakeDumpLog(thisPackage);
                        } else {
                        
                        float sortpriority = decompbr.ReadSingle();
                        logging.WriteLine("P" + packageparsecount + "/E" + e + " - Sort Priority: " + sortpriority, true);

                        int secondarySortIndex = decompbr.ReadUInt16();
                        logging.WriteLine("P" + packageparsecount + "/E" + e + " - Secondary Sort Index: " + secondarySortIndex, true);

                        uint propertyid = decompbr.ReadUInt32();
                        logging.WriteLine("P" + packageparsecount + "/E" + e + " - Property ID: " + propertyid, true);
                        
                        uint auralMaterialHash = decompbr.ReadUInt32();
                        logging.WriteLine("P" + packageparsecount + "/E" + e + " - Aural Material Hash: " + auralMaterialHash.ToString("X8"), true);

                        if (version <= 42){
                            logging.WriteLine("Version is <= 42: " + version, true);
                            int[] parameterFlag = new int[1];
                            parameterFlag[0] = (int)decompbr.ReadUInt16();
                            BitArray parameterFlags = new BitArray(parameterFlag);
                            logging.WriteLine(parameterFlags.Length.ToString(), true);
                            for(int p = 0; p < 16; p++)
                            {
                                if (parameterFlags[p] == true) {
                                    allFlags.Add(parameters[p]);
                                }
                                logging.WriteLine("Function Sort Flag [" + p + "] is " + parameters[p] + " and is " + parameterFlags[p].ToString(), true);
                            } 
                            
                        } else if (version >= 43){
                            logging.WriteLine("Version is >= 43: " + version, true);
                            int[] parameterFlag = new int[1];
                            parameterFlag[0] = (int)decompbr.ReadUInt16();
                            BitArray parameterFlags = new BitArray(parameterFlag);

                            for(int pfc = 0; pfc < 16; pfc++){
                                if (parameterFlags[pfc] == true) {
                                    allFlags.Add(parameters[pfc]);
                                }
                                logging.WriteLine("Function Sort Flag [" + pfc + "] is: " + parameters[pfc] + " and is " + parameterFlags[pfc].ToString(), true);
                            } 
                            
                        }
                            ulong excludePartFlags = decompbr.ReadUInt64();
                            logging.WriteLine("P" + packageparsecount + "/E" + e + " - Exclude Part Flags: " + excludePartFlags.ToString("X16"), true);
                            ulong excludePartFlags2 = decompbr.ReadUInt64();
                            logging.WriteLine("P" + packageparsecount + "/E" + e + " - Exclude Part Flags2: " + excludePartFlags2.ToString("X16"), true);
                            ulong excludeModifierRegionFlags = decompbr.ReadUInt64();
                            logging.WriteLine("P" + packageparsecount + "/E" + e + " - Exclude Part Flags: " + excludeModifierRegionFlags.ToString("X16"), true);


                        if (version >= 37){
                            logging.WriteLine(">= 37", true);
                            uint count = decompbr.ReadByte();
                            logging.WriteLine("P" + packageparsecount + "/E" + e + " - Tag Count: " + count.ToString(), true);
                            decompbr.ReadByte();
                            CASTag16Bit tags = new CASTag16Bit(decompbr, count);
                            for (int i = 0; i < count; i++){
                                logging.WriteLine("P" + packageparsecount + "/E" + e + " - Function Sort Flag " + i + " value is: " + tags.tagKey[i], true);                                
                                if (tags.tagKey[i] != 0){
                                    var tagKeyexists = from ID in TypeListings.S4BBFunctionTags
                                    where ID.typeID == tags.tagKey[i].ToString()
                                    select ID.typeID;
                                    bool tagfound = tagKeyexists.Any();
                                    var catKeyexists = from ID in TypeListings.S4BBFunctionTags
                                    where ID.typeID == tags.catKey[i].ToString()
                                    select ID.typeID;
                                    bool catfound = catKeyexists.Any();
                                    if (tagfound == true){
                                        logging.WriteLine("P" + packageparsecount + "/E" + e + " - TagKey " + tags.tagKey[i] + " exists in database.", true);
                                        foreach (typeList item in TypeListings.S4BBFunctionTags){
                                            if(item.typeID == tags.tagKey[i].ToString()){
                                                if ((itemtags.Exists(x => x.shortval == (short)tags.tagKey[i])) || (itemtags.Exists(x => x.stringval == tags.tagKey[i].ToString()))){
                                                    
                                                } else {
                                                    itemtags.Add(new TagsList{ shortval = (short)tags.tagKey[i], stringval = item.info});
                                                    logging.WriteLine("Tag " + i + " matched to " + item.info, true);
                                                }
                                            }
                                        }

                                    } else if (catfound == true) {
                                        logging.WriteLine("P" + packageparsecount + "/E" + e + " - CatKey " + tags.catKey[i] + " exists in database.", true);
                                        foreach (typeList item in TypeListings.S4BBFunctionTags){
                                            if(item.typeID == tags.catKey[i].ToString()){
                                                if ((itemtags.Exists(x => x.shortval == (short)tags.catKey[i])) || (itemtags.Exists(x => x.stringval == tags.catKey[i].ToString()))){
                                                    
                                                } else {
                                                    itemtags.Add(new TagsList{ shortval = (short)tags.catKey[i], stringval = item.info});
                                                    logging.WriteLine("Tag " + i + " matched to " + item.info, true);
                                                }
                                            }
                                        }

                                    } else {
                                        itemtags.Add(new TagsList{ shortval = (short)tags.tagKey[i], stringval = "Needs Identification"});
                                        logging.WriteLine("Tag " + i + " has no match, adding it to json.", true);
                                    }
                                }
                            }
                        } else {
                            uint count = decompbr.ReadByte();
                            logging.WriteLine("P" + packageparsecount + "/E" + e + " - Tag Count: " + count.ToString(), true);
                            decompbr.ReadByte();
                            CASTag16Bit tags = new CASTag16Bit(decompbr, count);
                            for (int i = 0; i < count; i++){
                                logging.WriteLine("P" + packageparsecount + "/E" + e + " - Function Sort Flag " + i + " value is: " + tags.tagKey[i], true);                                
                                if (tags.tagKey[i] != 0){
                                    var tagKeyexists = from ID in TypeListings.S4BBFunctionTags
                                    where ID.typeID == tags.tagKey[i].ToString()
                                    select ID.typeID;
                                    bool tagfound = tagKeyexists.Any();
                                    var catKeyexists = from ID in TypeListings.S4BBFunctionTags
                                    where ID.typeID == tags.catKey[i].ToString()
                                    select ID.typeID;
                                    bool catfound = catKeyexists.Any();
                                    if (tagfound == true){
                                        logging.WriteLine("P" + packageparsecount + "/E" + e + " - TagKey " + tags.tagKey[i] + " exists in database.", true);
                                        foreach (typeList item in TypeListings.S4BBFunctionTags){
                                            if(item.typeID == tags.tagKey[i].ToString()){
                                                if ((itemtags.Exists(x => x.shortval == (short)tags.tagKey[i])) || (itemtags.Exists(x => x.stringval == tags.tagKey[i].ToString()))){
                                                    
                                                } else {
                                                    itemtags.Add(new TagsList{ shortval = (short)tags.tagKey[i], stringval = item.info});
                                                    logging.WriteLine("Tag " + i + " matched to " + item.info, true);
                                                }
                                            }
                                        }

                                    } else if (catfound == true) {
                                        logging.WriteLine("P" + packageparsecount + "/E" + e + " - CatKey " + tags.catKey[i] + " exists in database.", true);
                                        foreach (typeList item in TypeListings.S4BBFunctionTags){
                                            if(item.typeID == tags.catKey[i].ToString()){
                                                if ((itemtags.Exists(x => x.shortval == (short)tags.catKey[i])) || (itemtags.Exists(x => x.stringval == tags.catKey[i].ToString()))){
                                                    
                                                } else {
                                                    itemtags.Add(new TagsList{ shortval = (short)tags.catKey[i], stringval = item.info});
                                                    logging.WriteLine("Tag " + i + " matched to " + item.info, true);
                                                }
                                            }
                                        }

                                    } else {
                                        itemtags.Add(new TagsList{ shortval = (short)tags.tagKey[i], stringval = "Needs Identification"});
                                        logging.WriteLine("Tag " + i + " has no match.", true);
                                    }
                                }
                            }
                        }
                        }
                            

                            uint simoleonprice = decompbr.ReadUInt32();
                            logging.WriteLine("Simoleon Price: " + simoleonprice.ToString("X8"), true);
                            uint partTitleKey = decompbr.ReadUInt32();
                            logging.WriteLine("Part Title Key: " + partTitleKey.ToString("X8"), true);
                            uint partDescriptionKey = decompbr.ReadUInt32();
                            logging.WriteLine("Part Description Key: " + partDescriptionKey.ToString("X8"), true);
                            if (version >= 43) {
                                uint createDescriptionKey = decompbr.ReadUInt32();
                            }
                            int uniqueTextureSpace = decompbr.ReadByte();
                            logging.WriteLine("Unique Texture Space: " + uniqueTextureSpace.ToString("X8"), true);
                            uint bodytype = decompbr.ReadUInt32();
                            bool foundmatch = false;
                            foreach (FunctionListing item in InitializedLists.S4BodyTypes){
                                if (item.bodytype == bodytype){
                                    foundmatch = true;
                                    thisPackage.Function = item.Function;
                                    if (!String.IsNullOrWhiteSpace(item.Subfunction)) {
                                        thisPackage.FunctionSubcategory = item.Subfunction;
                                    }                                
                                }
                            }
                            if (foundmatch == false){
                                thisPackage.Function = "Unidentified function (contact SinfulSimming). Code: " + bodytype.ToString();
                            }
                            logging.WriteLine("Bodytype: " + bodytype.ToString(), true);
                            uint bodytypesubtype = decompbr.ReadUInt16();
                            logging.WriteLine("Bodytype Subtype: " + bodytypesubtype.ToString(), true);
                            decompbr.ReadUInt32();                        
                            uint agflags = decompbr.ReadUInt32();
                            logging.WriteLine("Age Gender Flags Value: " + agflags.ToString("X8"), true);

                            string AGFlag = agflags.ToString("X8");
                            
                            AgeGenderFlags agegenderset = new AgeGenderFlags();

                            if (AGFlag == "00000000") {
                                agegenderset = new AgeGenderFlags{
                                Adult = false, 
                                Baby = false, 
                                Child = false, 
                                Elder = false, 
                                Infant = false, 
                                Teen = false, 
                                Toddler = false, 
                                YoungAdult = false, 
                                Female = false, 
                                Male = false};
                                logging.WriteLine("No age/gender flags present.", true);
                            } else if (AGFlag == "00000020") {
                                agegenderset = new AgeGenderFlags{
                                Adult = true, 
                                Baby = false, 
                                Child = false, 
                                Elder = false, 
                                Infant = false, 
                                Teen = false, 
                                Toddler = false, 
                                YoungAdult = false, 
                                Female = false, 
                                Male = false};
                                logging.WriteLine("Adult (nothing else)", true);
                            } else if (AGFlag == "00002020") {
                                agegenderset = new AgeGenderFlags{
                                    Adult = true, 
                                Baby = false, 
                                Child = false, 
                                Elder = false, 
                                Infant = false, 
                                Teen = false, 
                                Toddler = false, 
                                YoungAdult = false, 
                                Female = true, 
                                Male = false};
                                logging.WriteLine("Adult Female", true);
                            } else if (AGFlag == "00020000") {
                                agegenderset = new AgeGenderFlags{
                                    Adult = true, 
                                    Baby = false, 
                                    Child = false, 
                                    Elder = false, 
                                    Infant = false, 
                                    Teen = false, 
                                    Toddler = false, 
                                    YoungAdult = false, 
                                    Female = false, 
                                    Male = true};
                                logging.WriteLine("Adult Male", true);
                            } else if (AGFlag == "00002078") {
                                agegenderset = new AgeGenderFlags{
                                    Adult = true, 
                                    Baby = false, 
                                    Child = false, 
                                    Elder = true, 
                                    Infant = false, 
                                    Teen = true, 
                                    Toddler = false, 
                                    YoungAdult = true, 
                                    Female = true, 
                                    Male = false};
                                logging.WriteLine("Adult/Elder/Teen, Female", true);
                            } else if (AGFlag == "000030FF") {
                                agegenderset = new AgeGenderFlags{
                                Adult = true, 
                                Baby = true, 
                                Child = true, 
                                Elder = true, 
                                Infant = true, 
                                Teen = true, 
                                Toddler = true, 
                                YoungAdult = true, 
                                Female = true, 
                                Male = true};
                                logging.WriteLine("Everything", true);
                            } else if (AGFlag == "00003004") {
                                agegenderset = new AgeGenderFlags{
                                Adult = false, 
                                Baby = false, 
                                Child = true, 
                                Elder = false, 
                                Infant = false, 
                                Teen = false, 
                                Toddler = false, 
                                YoungAdult = false, 
                                Female = true, 
                                Male = true};
                                logging.WriteLine("Child of either gender", true);
                            } else if (AGFlag == "00001078") {
                                agegenderset = new AgeGenderFlags{
                                Adult = true, 
                                Baby = false, 
                                Child = false, 
                                Elder = true, 
                                Infant = false, 
                                Teen = true, 
                                Toddler = false, 
                                YoungAdult = true, 
                                Female = false, 
                                Male = true};
                            } else if (AGFlag == "00003078") {
                                agegenderset = new AgeGenderFlags{
                                Adult = true, 
                                Baby = false, 
                                Child = false, 
                                Elder = true, 
                                Infant = false, 
                                Teen = true, 
                                Toddler = false, 
                                YoungAdult = true, 
                                Female = true, 
                                Male = true};
                            } else if (AGFlag == "000030BE") {
                                agegenderset = new AgeGenderFlags{
                                Adult = true, 
                                Baby = false, 
                                Child = true, 
                                Elder = false, 
                                Infant = true, 
                                Teen = true, 
                                Toddler = true, 
                                YoungAdult = true, 
                                Female = true, 
                                Male = true};
                            } else if (AGFlag == "00002002") {
                                agegenderset = new AgeGenderFlags{
                                Adult = false, 
                                Baby = false, 
                                Child = false, 
                                Elder = false, 
                                Infant = false, 
                                Teen = false, 
                                Toddler = true, 
                                YoungAdult = false, 
                                Female = true, 
                                Male = false};
                            }  else if (AGFlag == "00002004") {
                                agegenderset = new AgeGenderFlags{
                                Adult = false, 
                                Baby = false, 
                                Child = true, 
                                Elder = false, 
                                Infant = false, 
                                Teen = false, 
                                Toddler = false, 
                                YoungAdult = false, 
                                Female = true, 
                                Male = false};
                            }  else if (AGFlag == "00003002") {
                                agegenderset = new AgeGenderFlags{
                                Adult = false, 
                                Baby = false, 
                                Child = false, 
                                Elder = false, 
                                Infant = false, 
                                Teen = false, 
                                Toddler = true, 
                                YoungAdult = false, 
                                Female = true, 
                                Male = true};
                            }  else if (AGFlag == "00003004") {
                                agegenderset = new AgeGenderFlags{
                                Adult = false, 
                                Baby = false, 
                                Child = true, 
                                Elder = false, 
                                Infant = false, 
                                Teen = false, 
                                Toddler = false, 
                                YoungAdult = false, 
                                Female = true, 
                                Male = true};
                            }  else if (AGFlag == "00001002") {
                                agegenderset = new AgeGenderFlags{
                                Adult = false, 
                                Baby = false, 
                                Child = false, 
                                Elder = false, 
                                Infant = false, 
                                Teen = false, 
                                Toddler = true, 
                                YoungAdult = false, 
                                Female = false, 
                                Male = true};
                            }  else if (AGFlag == "00001004") {
                                agegenderset = new AgeGenderFlags{
                                Adult = false, 
                                Baby = false, 
                                Child = true, 
                                Elder = false, 
                                Infant = false, 
                                Teen = false, 
                                Toddler = false, 
                                YoungAdult = false, 
                                Female = false, 
                                Male = true};
                            } else if (AGFlag == "00100101") {
                                agegenderset = new AgeGenderFlags{
                                Adult = false, 
                                Baby = true, 
                                Child = false, 
                                Elder = false, 
                                Infant = false, 
                                Teen = false, 
                                Toddler = false, 
                                YoungAdult = false, 
                                Female = false, 
                                Male = false};                            
                            } else if (AGFlag == "0000307E"){
                                agegenderset = new AgeGenderFlags{
                                Adult = true, 
                                Baby = false, 
                                Child = true, 
                                Elder = true, 
                                Infant = false, 
                                Teen = true, 
                                Toddler = true, 
                                YoungAdult = true, 
                                Female = true, 
                                Male = true};
                            } else if (AGFlag == "000030FE"){
                                agegenderset = new AgeGenderFlags{
                                Adult = true, 
                                Baby = false, 
                                Child = true, 
                                Elder = true, 
                                Infant = true, 
                                Teen = true, 
                                Toddler = true, 
                                YoungAdult = true, 
                                Female = true, 
                                Male = true};
                            } else {
                                agegenderset = new AgeGenderFlags{
                                Adult = false, 
                                Baby = false, 
                                Child = false, 
                                Elder = false, 
                                Infant = false, 
                                Teen = false, 
                                Toddler = false, 
                                YoungAdult = false, 
                                Female = false, 
                                Male = false};
                            }

                            thisPackage.AgeGenderFlags = agegenderset;

                            


                            if (version >= 0x20)
                            {
                                uint species = decompbr.ReadUInt32();
                            }
                            if (version >= 34)
                            {
                                int packID = decompbr.ReadInt16();
                                int packFlags = decompbr.ReadByte();
                                for(int p = 0; p < packFlags; p++)
                                {
                                    bool check = decompbr.ReadBoolean();
                                    logging.WriteLine("Pack Flag [" + p + "] is " + check.ToString(), true);
                                } 
                                byte[] reserved2 = decompbr.ReadBytes(9);
                            }
                            else
                            {
                                int packID = 0;
                                byte unused2 = decompbr.ReadByte();
                                if (unused2 > 0) {
                                    int unused3 = decompbr.ReadByte();
                                }
                            }

                            decompbr.Dispose();
                            decomps.Dispose();

                        }

                        caspc++;
                        
                    }                     

                }


                if ((fileHas.Exists(x => x.term == "COBJ"))){
                    List<int> entryspots = new List<int>();
                    int fh = 0;
                    foreach (fileHasList item in fileHas) {
                        if (item.term == "COBJ"){
                            entryspots.Add(fh);                       
                        }
                        fh++;
                    }    
                    int cobjc = 0;
                    foreach (int e in entryspots){
                        logging.WriteLine("P" + packageparsecount + "/E" + e + " - Opening COBJ #" + cobjc, true);
                        if (indexData[e].compressionType == "5A42"){                                
                                //var here = readFile.BaseStream.Position;
                                readFile.BaseStream.Position = indexData[e].position;
                                //dbpfFile.Seek(, SeekOrigin.Begin);                            
                                int entryEnd = (int)readFile.BaseStream.Position + (int)indexData[e].memSize;
                                logging.WriteLine("P" + packageparsecount + "/E" + e + " - Position: " + indexData[e].position, true);
                                logging.WriteLine("P" + packageparsecount + "/E" + e + " - Filesize: " + indexData[e].fileSize, true);
                                logging.WriteLine("P" + packageparsecount + "/E" + e + " - Memsize: " + indexData[e].memSize, true);
                                logging.WriteLine("P" + packageparsecount + "/E" + e + " - Entry ends at " + entryEnd, true);
                                byte[] entry = readFile.ReadBytes((int)indexData[e].memSize);
                                Stream decomps = S4Decryption.Decompress(entry);
                                
                                BinaryReader decompbr = new BinaryReader(decomps);                           
                                
                                ReadCOBJ rc = new ReadCOBJ(decompbr, packageparsecount, e, itemtags, logging);

                                if((!allGUIDS.Contains(rc.GUID)) && rc.GUID != "null"){
                                    allGUIDS.Add(rc.GUID);
                                }

                                foreach (TagsList tag in rc.itemtags) {
                                    if (tag.stringval != "null"){
                                      itemtags.Add(tag);
                                    }                                    
                                }
                            decompbr.Dispose();
                            decomps.Dispose();
                                
                        } else if (indexData[e].compressionType == "00"){
                            //dbpfFile.Seek(, SeekOrigin.Begin); 
                            readFile.BaseStream.Position = indexData[e].position;                           
                            int entryEnd = (int)readFile.BaseStream.Position + (int)indexData[e].memSize;
                            logging.WriteLine("P" + packageparsecount + "/E" + e + " - Position: " + indexData[e].position, true);
                            logging.WriteLine("P" + packageparsecount + "/E" + e + " - Filesize: " + indexData[e].fileSize, true);
                            logging.WriteLine("P" + packageparsecount + "/E" + e + " - Memsize: " + indexData[e].memSize, true);
                            logging.WriteLine("P" + packageparsecount + "/E" + e + " - Entry ends at " + entryEnd, true);
                            ReadCOBJ rc = new ReadCOBJ(readFile, packageparsecount, e, itemtags, logging);                            
                            if((!allGUIDS.Contains(rc.GUID)) && rc.GUID != "null"){
                                allGUIDS.Add(rc.GUID);
                            }

                            foreach (TagsList tag in rc.itemtags) {
                                if (tag.stringval != "null"){
                                    itemtags.Add(tag);
                                }                                    
                            }
                        }

                        

                        

                        cobjc++;
                    }
                }

                if ((fileHas.Exists(x => x.term == "OBJD") && (dump == false))){
                    List<int> entryspots = new List<int>();
                    int fh = 0;
                    foreach (fileHasList item in fileHas) {
                        if (item.term == "OBJD"){
                            entryspots.Add(fh);                       
                        }
                        fh++;
                    }    
                    int objdc = 0;
                    foreach (int e in entryspots){
                        logging.WriteLine("P" + packageparsecount + "/E" + e + " - Opening OBJD #" + objdc, true);
                        if (indexData[e].compressionType == "5A42"){
                                //dbpfFile.Seek(, SeekOrigin.Begin);
                                //var here = readFile.BaseStream.Position;
                                readFile.BaseStream.Position = indexData[e].position;                       
                                int entryEnd = (int)readFile.BaseStream.Position + (int)indexData[e].memSize;
                                logging.WriteLine("P" + packageparsecount + "/E" + e + " - Position: " + indexData[e].position, true);
                                logging.WriteLine("P" + packageparsecount + "/E" + e + " - Filesize: " + indexData[e].fileSize, true);
                                logging.WriteLine("P" + packageparsecount + "/E" + e + " - Memsize: " + indexData[e].memSize, true);
                                logging.WriteLine("P" + packageparsecount + "/E" + e + " - Entry ends at " + entryEnd, true);
                                byte[] entry = readFile.ReadBytes((int)indexData[e].memSize);
                                Stream decomps = S4Decryption.Decompress(entry);
                                
                                BinaryReader decompbr = new BinaryReader(decomps);

                                ReadOBJDIndex readOBJD = new ReadOBJDIndex(decompbr, logging);
                                logging.WriteLine("There are " + readOBJD.count + " entries to read.", true);
                                objdentries = new string[readOBJD.count];
                                objdpositions = new int[readOBJD.count];
                                for (int f = 0; f < readOBJD.count; f++){
                                    logging.WriteLine("Entry " + f + ": ", true);
                                    logging.WriteLine("--- Type: " + readOBJD.entrytype[f].ToString("X8"), true);
                                    objdentries[f] = readOBJD.entrytype[f].ToString();
                                    objdpositions[f] = (int)readOBJD.position[f];
                                    logging.WriteLine("--- Position " + readOBJD.position[f], true);
                                }
                                logging.WriteLine("P" + packageparsecount + "/E" + e + " - Reader is at " + decompbr.BaseStream.Position, true);
                                decompbr.BaseStream.Position = readOBJD.position[0];
                                ReadOBJDEntry readobjdentry = new ReadOBJDEntry(decompbr, objdentries, objdpositions, logging);
                                thisPackage.Title = readobjdentry.name;
                                logging.WriteLine("Adding components to package: ", true);
                                for (int c = 0; c < readobjdentry.componentcount; c++){
                                    logging.WriteLine(readobjdentry.components[c].ToString(), true);
                                    logging.WriteLine(readobjdentry.components[c].ToString("X8"), true);
                                    thisPackage.Components.Add(readobjdentry.components[c].ToString("X8"));
                                }
                                foreach (string m in readobjdentry.model) {
                                    if(!allGUIDS.Contains(m)){
                                        allGUIDS.Add(m);
                                    }  
                                }
                                foreach (string r in readobjdentry.rig) {
                                    if(!allGUIDS.Contains(r)){
                                        allGUIDS.Add(r);
                                    }  
                                }
                                foreach (string s in readobjdentry.slot) {
                                    if(!allGUIDS.Contains(s)){
                                        allGUIDS.Add(s);
                                    }  
                                }
                                foreach (string f in readobjdentry.footprint) {
                                    if(!allGUIDS.Contains(f)){
                                        allGUIDS.Add(f);
                                    }
                                }
                                                             
                             decompbr.Dispose();   
                             decomps.Dispose();
                        }
                        
                        objdc++;
                    }
                    
                }
            




            logging.WriteLine("All methods complete, moving on to getting info.", true);

            List<TypeCounter> typecount = new List<TypeCounter>();
            var typeDict = new Dictionary<string, int>();

            logging.WriteLine("Making dictionary.", true);

            foreach (fileHasList item in fileHas){
                foreach (typeList type in TypeListings.AllTypesS4){
                    if (type.desc == item.term){
                        logging.WriteLine("Added " + type.desc + " to dictionary.", true);
                        typeDict.Increment(type.desc);
                    }
                }
            }

            foreach (KeyValuePair<string, int> type in typeDict){
                TypeCounter tc = new TypeCounter();
                tc.Type = type.Key;
                tc.Count = type.Value;
                logging.WriteLine("There are " + tc.Count + " of " + tc.Type + " in this package.", true);
                typecount.Add(tc);
            }

            thisPackage.Entries.AddRange(typecount);

            //ifs

            int casp;int geom;int rle;int rmap;int thum;int img;int xml;int clhd;int clip;int stbl;int cobj;int ftpt;int lite;int thm;int mlod;int modl;int mtbl;int objd;int rslt;int tmlt;int ssm;int lrle;int bond;int cpre;int dmap;int smod;int bgeo;int hotc;

            

            distinctFlags = allFlags.Distinct().ToList();
            thisPackage.Flags.AddRange(distinctFlags);

            distinctItemTags = itemtags.Distinct().ToList();
            thisPackage.CatalogTags.AddRange(distinctItemTags);

            distinctInstanceIDs = allInstanceIDs.Distinct().ToList();
            thisPackage.InstanceIDs.AddRange(distinctInstanceIDs);


            distinctGUIDS = allGUIDS.Distinct().ToList();
            thisPackage.ObjectGUID.AddRange(distinctGUIDS);

            
                     
            logging.WriteLine("P" + packageparsecount + ": Checking " + thisPackage.PackageName + " against override IDs.", true);
            
            

            ParallelOptions parallelSettings = new ParallelOptions() { MaxDegreeOfParallelism = 2000};
            
            bool found;

            SimsPackage overrideContainer = new SimsPackage();

            
            List<string> FoundIDs = new List<string>();
            Parallel.ForEach(thisPackage.InstanceIDs, parallelSettings, iid => { 
                string cs = string.Format("Data Source={0}", GlobalVariables.S4_Overrides_All);                   
                using (var dataConnection = new SQLiteConnection(cs)){                    
                    try
                    {
                        dataConnection.Open();
                        string cmdtxt = string.Format("SELECT * FROM Instances WHERE InstanceID = '{0}'", iid);
                        using (SQLiteCommand sqcmd = new SQLiteCommand(cmdtxt, dataConnection)){
                            using (SQLiteDataReader sqlreader = sqcmd.ExecuteReader()){
                                while (sqlreader.Read())
                                {
                                    string idm = sqlreader["InstanceID"].ToString();
                                    string ow = sqlreader["Name"].ToString();
                                    string pack = sqlreader["Pack"].ToString();
                                    string tp = sqlreader["Type"].ToString();
                                    if (!String.IsNullOrWhiteSpace(idm)){
                                        thisPackage.FunctionSubcategory = tp;
                                        thisPackage.OverriddenInstances.Add(iid);
                                        if ((!String.IsNullOrWhiteSpace(ow)) && (ow != " ")){
                                            thisPackage.OverriddenItems.Add(pack + ": " + ow);
                                        }                                        
                                        thisPackage.Override = true;
                                    }
                                }
                            }
                        }        
                    }
                    catch (System.Exception e)
                    { 
                        System.Windows.Forms.MessageBox.Show("Database Error: " + e.Message.ToString(),
                        "Error Message: Searching for Sims 4 Overrides failed, please report the issue.",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        dataConnection.Close();
                    }                    
                }
                cs = string.Format("Data Source={0}", GlobalVariables.S4_Overrides_List);
                foreach (string inst in thisPackage.OverriddenInstances){
                    using (var dataConnection = new SQLiteConnection(cs)){                    
                        try
                        {
                            dataConnection.Open();
                            string cmdtxt = string.Format("SELECT * FROM Overrides WHERE Instance = '{0}'", inst);
                            using (SQLiteCommand sqcmd = new SQLiteCommand(cmdtxt, dataConnection)){
                                using (SQLiteDataReader sqlreader = sqcmd.ExecuteReader()){
                                    while (sqlreader.Read())
                                    {
                                        string desc = sqlreader["Description"].ToString();
                                        if (!String.IsNullOrWhiteSpace(desc)){
                                            thisPackage.Type = "OVERRIDE: " + desc;                                            
                                        }
                                    }
                                }
                            }        
                        }
                        catch (System.Exception e)
                        { 
                            System.Windows.Forms.MessageBox.Show("Database Error: " + e.Message.ToString(),
                            "Error Message: Searching for Sims 4 Overrides failed, please report the issue.",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            dataConnection.Close();
                        }                    
                    }
                }
            });
            
            if (thisPackage.Override != true){
                logging.WriteLine("No overrides were found. Checking other options.", true);
                if ((typeDict.TryGetValue("S4SM", out ssm) && ssm >= 1)){
                thisPackage.Type = "Merged Package";
                logging.WriteLine("This is a " + thisPackage.Type + "!!", true);
                } else if ((typeDict.TryGetValue("BGEO", out bgeo) && bgeo >= 1) && (typeDict.TryGetValue("HOTC", out hotc) && hotc >= 1) && (typeDict.TryGetValue("SMOD", out smod) && smod >= 1)){
                    thisPackage.Type = "Slider";
                    logging.WriteLine("This is a " + thisPackage.Type + "!!", true);
                } else if ((typeDict.TryGetValue("BOND", out bond) && bond >= 1) && (typeDict.TryGetValue("CPRE", out cpre) && cpre >= 1) && (typeDict.TryGetValue("DMAP", out dmap) && dmap >= 1) && (typeDict.TryGetValue("SMOD", out smod) && smod >= 1)){
                    thisPackage.Type = "Preset";
                    logging.WriteLine("This is a " + thisPackage.Type + "!!", true);
                } else if ((typeDict.TryGetValue("RLE2", out rle) && rle >= 1) && (typeDict.TryGetValue("CASP", out casp) && casp >= 1) && (typeDict.TryGetValue("GEOM", out geom) && geom <= 0)){
                    thisPackage.Type = "CAS Recolor";
                    logging.WriteLine("This is a " + thisPackage.Type + "!!", true);
                } else if ((typeDict.TryGetValue("LRLE", out lrle) && lrle >= 1) && (typeDict.TryGetValue("CASP", out casp) && casp >= 1) && (typeDict.TryGetValue("GEOM", out geom) && geom <= 0)){
                    thisPackage.Type = "Makeup";
                    logging.WriteLine("This is a " + thisPackage.Type + "!!", true);
                } else if ((typeDict.TryGetValue("CASP", out casp) && casp <= 0) && (typeDict.TryGetValue("GEOM", out geom) && geom >= 1) && (typeDict.TryGetValue("RLE2", out rle) && rle >= 1) && (typeDict.TryGetValue("RMAP", out rmap) && rmap >= 1) && (typeDict.TryGetValue("_IMG", out img) && img <= 0)){
                    thisPackage.Type = "Hair Mesh";
                    thisPackage.Mesh = true;
                    thisPackage.Recolor = false;
                    logging.WriteLine("This is a " + thisPackage.Type + "!!", true);
                } else if ((typeDict.TryGetValue("CASP", out casp) && casp >= 1) && (typeDict.TryGetValue("GEOM", out geom) && geom <= 0) && (typeDict.TryGetValue("RLE2", out rle) && rle >= 1) && (typeDict.TryGetValue("RMAP", out rmap) && rmap >= 1) && (typeDict.TryGetValue("_IMG", out img) && img >= 1)){
                    thisPackage.Type = "Hair Recolor";
                    thisPackage.Mesh = false;
                    thisPackage.Recolor = true;
                    logging.WriteLine("This is a " + thisPackage.Type + "!!", true);
                } else if ((typeDict.TryGetValue("CASP", out casp) && casp >= 1) && (typeDict.TryGetValue("GEOM", out geom) && geom >= 1) && (typeDict.TryGetValue("RLE2", out rle) && rle >= 1) && (typeDict.TryGetValue("RMAP", out rmap) && rmap >= 1) && (typeDict.TryGetValue("_IMG", out img) && img >= 1)){
                    thisPackage.Type = "Hair";
                    thisPackage.Mesh = true;
                    thisPackage.Recolor = true;
                    logging.WriteLine("This is a " + thisPackage.Type + "!!", true);
                } else if ((typeDict.TryGetValue("_IMG", out img) && img >= 1) && (typeDict.TryGetValue("_XML", out xml) && xml >= 1) && (typeDict.TryGetValue("CLHD", out clhd) && clhd >= 1) && (typeDict.TryGetValue("CLIP", out clip) && clip >= 1) && (typeDict.TryGetValue("STBL", out stbl) && stbl >= 1)){
                    thisPackage.Type = "Pose Pack";
                    logging.WriteLine("This is a " + thisPackage.Type + "!!", true);
                } else if ((typeDict.TryGetValue("_IMG", out img) && img >= 1) && (typeDict.TryGetValue("COBJ", out cobj) && cobj >= 1) && (typeDict.TryGetValue("FTPT", out ftpt) && ftpt >= 1) && (typeDict.TryGetValue("MLOD", out mlod) && mlod >= 1) && (typeDict.TryGetValue("MODL", out modl) && modl >= 1) && (typeDict.TryGetValue("MTBL", out mtbl) && mtbl <= 0)  && (typeDict.TryGetValue("_IMG", out img) && img <= 0) && (typeDict.TryGetValue("OBJD", out objd) && objd >= 1)){
                    thisPackage.Type = "Object Mesh";
                    thisPackage.Mesh = true;
                    thisPackage.Recolor = false;
                    logging.WriteLine("This is a " + thisPackage.Type + "!!", true);
                } else if ((typeDict.TryGetValue("_IMG", out img) && img >= 1) && (typeDict.TryGetValue("COBJ", out cobj) && cobj >= 1) && (typeDict.TryGetValue("FTPT", out ftpt) && ftpt >= 1) && (typeDict.TryGetValue("MLOD", out mlod) && mlod >= 1) && (typeDict.TryGetValue("MODL", out modl) && modl >= 1) && (typeDict.TryGetValue("MTBL", out mtbl) && mtbl >= 1) && (typeDict.TryGetValue("OBJD", out objd) && objd <= 0)){
                    thisPackage.Type = "Object Recolor";
                    thisPackage.Mesh = false;
                    thisPackage.Recolor = true;
                    logging.WriteLine("This is a " + thisPackage.Type + "!!", true);
                } else if ((typeDict.TryGetValue("_IMG", out img) && img >= 1) && (typeDict.TryGetValue("COBJ", out cobj) && cobj >= 1) && (typeDict.TryGetValue("FTPT", out ftpt) && ftpt >= 1) && (typeDict.TryGetValue("MLOD", out mlod) && mlod >= 1) && (typeDict.TryGetValue("MODL", out modl) && modl >= 1) && (typeDict.TryGetValue("MTBL", out mtbl) && mtbl >= 1) && (typeDict.TryGetValue("OBJD", out objd) && objd >= 1)){
                    thisPackage.Type = "Object";
                    thisPackage.Mesh = true;
                    thisPackage.Recolor = true;
                    logging.WriteLine("This is a " + thisPackage.Type + "!!", true);
                } else if ((typeDict.TryGetValue("RLE2", out rle) && rle >= 1) && (typeDict.TryGetValue("CASP", out casp) && casp >= 1) && (typeDict.TryGetValue("GEOM", out geom) && geom >= 1)){
                    thisPackage.Type = "CAS Part";
                    logging.WriteLine("This is a " + thisPackage.Type + "!!", true);
                } else {
                    logging.WriteLine("Unable to identify package.", true);
                    thisPackage.Type = "UNKNOWN";
                }
            }

            }
            

            

            logging.WriteLine("In thisPackage: " + thisPackage.ToString(), true);
            logging.WriteLine(thisPackage.ToString(), false);
            //Containers.Containers.allSimsPackages.Add(thisPackage);
            GlobalVariables.DatabaseConnection.Insert(thisPackage, typeof(SimsPackage));
            txt = string.Format("SELECT * FROM Processing_Reader where Name='{0}'", packageinfo.Name);
            queries = GlobalVariables.DatabaseConnection.Query<PackageFile>(txt);
            query = queries[0];
            GlobalVariables.DatabaseConnection.Delete(query);
            logging.Flush();
            logging.Close();
            var read = new StreamReader(logfile);
            string logdata = read.ReadToEnd();
            log.MakeLog(logdata, true);
            File.Delete(logfile);
            readFile.Dispose();
            dbpfFile.Dispose();
            //cwl = string.Format("Closing package # {0}/{1}: {3}", packageparsecount, GlobalVariables.PackageCount, packageinfo.Name);
            //Console.WriteLine(cwl);
        }

        public void FindS4ConflictsAndMatches(SimsPackage package){
            
        }

        public static String hexToASCII(String hex)
        {
            // initialize the ASCII code string as empty.
            String ascii = "";
    
            for (int i = 0; i < hex.Length; i += 2)
            {
    
                // extract two characters from hex string
                String part = hex.Substring(i, 2);
    
                // change it into base 16 and
                // typecast as the character
                char ch = (char)Convert.ToInt32(part, 16);;
    
                // add this char to final ASCII string
                ascii = ascii + ch;
            }
            return ascii;
        } 

        public void RetunePackage(SimsPackage package, string tuning){
            //add otg tags OR add specific tuning passed in. for example, cars for transport mod
        }
           
        public void MakeDumpLog(SimsPackage thisPackage){
            string databasepath = (@"I:\Code\C#\Sims-CC-Sorter\Console\src\SimsCCManager.Console\data\BaseGame_Instances.sqlite");
            string cs = string.Format("Data Source={0}", databasepath);
            using (var dataConnection = new SQLiteConnection(cs)){
                foreach (string InstanceID in thisPackage.InstanceIDs){
                    string TGI = string.Format("034AEECB-00000000-{0}", InstanceID);
                    dataConnection.Open();
                    string cmdtxt = string.Format("INSERT INTO CASP Instances (InstanceID, Item Name, TGI) VALUES ('{0}', '{1}', '{2}')", InstanceID, thisPackage.Title, TGI);
                    dataConnection.Close();
                }
            }
        }

        
    }
}