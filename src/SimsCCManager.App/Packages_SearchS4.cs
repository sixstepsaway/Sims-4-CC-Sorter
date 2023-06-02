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
using System.Text.RegularExpressions;
using SQLiteNetExtensions.Attributes;
using SQLiteNetExtensions.Extensions;

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

    public class Readers {
        static LoggingGlobals log = new LoggingGlobals();
        public static List<TagsList> GetTagInfo(CASTag16Bit tags, uint count){
            
            List<TagsList> taglist = new List<TagsList>();

            for (int i = 0; i < count; i++)
            {
                using (var dataConnection = new SQLiteConnection(TypeListings.functionSortsListDB)){                    
                    try
                    {
                        dataConnection.Open();
                        string cmdtxt = string.Format("SELECT * FROM S4CategoryTags where typeID='{0}' OR typeID='{1}'", tags.tagKey[i], tags.catKey[i]);

                        using (SQLiteCommand sqcmd = new SQLiteCommand(cmdtxt, dataConnection)){
                            using (SQLiteDataReader sqlreader = sqcmd.ExecuteReader()){
                                while (sqlreader.Read())
                                {
                                    string tid = sqlreader["typeID"].ToString();
                                    string info = sqlreader["info"].ToString();
                                    taglist.Add(new TagsList{ TypeID = tid, Description = info});
                                    log.MakeLog(string.Format("Tag {0} was matched to {1}.", i, info), true);
                                }
                            }
                        }        
                    }
                    catch (System.Exception ex)
                    { 
                        System.Windows.Forms.MessageBox.Show("Database Error: " + ex.Message.ToString(),
                        "Tagsearch failed. Please notify developer..",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        dataConnection.Close();
                    }                    
                }
            }
            return taglist;            
        }
        
        public static List<TagsList> GetTagInfo(Tag tags, uint count){
            List<TagsList> taglist = new List<TagsList>();

            for (int i = 0; i < count; i++)
            {

                using (var dataConnection = new SQLiteConnection(TypeListings.functionSortsListDB)){                    
                    try
                    {
                        dataConnection.Open();
                        string cmdtxt = string.Format("SELECT * FROM S4CategoryTags where typeID='{0}'", tags.tagKey[i]);

                        using (SQLiteCommand sqcmd = new SQLiteCommand(cmdtxt, dataConnection)){
                            using (SQLiteDataReader sqlreader = sqcmd.ExecuteReader()){
                                while (sqlreader.Read())
                                {
                                    string tid = sqlreader["typeID"].ToString();
                                    string info = sqlreader["info"].ToString();
                                    taglist.Add(new TagsList{ TypeID = tid, Description = info});
                                    log.MakeLog(string.Format("Tag {0} was matched to {1}.", i, info), true);
                                }
                            }
                        }        
                    }
                    catch (System.Exception ex)
                    { 
                        System.Windows.Forms.MessageBox.Show("Database Error: " + ex.Message.ToString(),
                        "Tagsearch failed. Please notify developer..",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        dataConnection.Close();
                    }                    
                }
            }
            return taglist;            
        }        
        
        public static List<TagsList> GetTagInfo(List<uint> tags, uint count){
            List<TagsList> taglist = new List<TagsList>();

            for (int i = 0; i < count; i++)
            {
                using (var dataConnection = new SQLiteConnection(TypeListings.functionSortsListDB)){                    
                    try
                    {
                        dataConnection.Open();
                        string cmdtxt = string.Format("SELECT * FROM S4CategoryTags where typeID='{0}'", tags[i]);

                        using (SQLiteCommand sqcmd = new SQLiteCommand(cmdtxt, dataConnection)){
                            using (SQLiteDataReader sqlreader = sqcmd.ExecuteReader()){
                                while (sqlreader.Read())
                                {
                                    string tid = sqlreader["typeID"].ToString();
                                    string info = sqlreader["info"].ToString();
                                    taglist.Add(new TagsList{ TypeID = tid, Description = info});
                                    log.MakeLog(string.Format("Tag {0} was matched to {1}.", i, info), true);
                                }
                            }
                        }        
                    }
                    catch (System.Exception ex)
                    { 
                        System.Windows.Forms.MessageBox.Show("Database Error: " + ex.Message.ToString(),
                        "Tagsearch failed. Please notify developer..",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        dataConnection.Close();
                    }                    
                }
            }
            return taglist;            
        }
    }


    public struct ResourceKeyITG {
        /// <summary>
        /// Reads resource keys and does not flip the instance ID.
        /// </summary>
        LoggingGlobals log = new LoggingGlobals();
        public ulong instance;
        public uint type;
        public uint group;
        
        public ResourceKeyITG(BinaryReader reader){
            this.instance = reader.ReadUInt64(); 
            log.MakeLog("GUID Instance: " + this.instance, true);
            this.type = reader.ReadUInt32(); 
            log.MakeLog("GUID Type: " + this.type, true);
            this.group = reader.ReadUInt32();     
            log.MakeLog("GUID Group: " + this.group, true);  
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
        
        public ResourceKeyITGFlip(BinaryReader reader){
            uint left = reader.ReadUInt32();
            uint right = reader.ReadUInt32();
            ulong longleft = left;
            longleft = (longleft << 32);
            this.instance = longleft | right;
            log.MakeLog("GUID Instance: " + this.instance, true);
            this.type = reader.ReadUInt32(); 
            log.MakeLog("GUID Type: " + this.type, true);
            this.group = reader.ReadUInt32();  
            log.MakeLog("GUID Group: " + this.group, true);       
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

        public ReadCOBJ(BinaryReader readFile, int packageparsecount, int e, List<TagsList> itemtags){
            uint version = readFile.ReadUInt32();
            log.MakeLog("P" + packageparsecount + "/E" + e + " - Version: " + version, true);
            log.MakeLog("-- As hex: " + version.ToString("X8"), true);
            uint commonblockversion = readFile.ReadUInt32();
            log.MakeLog("P" + packageparsecount + "/E" + e + " - Common Block Version: " + commonblockversion, true);
            log.MakeLog("-- As hex: " + commonblockversion.ToString("X8"), true);
            uint namehash = readFile.ReadUInt32();
            log.MakeLog("P" + packageparsecount + "/E" + e + " - NameHash: " + namehash, true);
            log.MakeLog("-- As hex: " + namehash.ToString("X8"), true);
            uint descriptionhash = readFile.ReadUInt32();
            log.MakeLog("P" + packageparsecount + "/E" + e + " - DescriptionHash: " + descriptionhash, true);
            log.MakeLog("-- As hex: " + descriptionhash.ToString("X8"), true);                            
            uint price = readFile.ReadUInt32();
            log.MakeLog("P" + packageparsecount + "/E" + e + " - Price: " + price, true);
            log.MakeLog("-- As hex: " + price.ToString("X8"), true);

            ulong thumbhash = readFile.ReadUInt64();
            log.MakeLog("P" + packageparsecount + "/E" + e + " - Thumbnail Hash: " + thumbhash, true);
            log.MakeLog("-- As hex: " + thumbhash.ToString("X8"), true);

            uint devcatflags = readFile.ReadUInt32();
            log.MakeLog("P" + packageparsecount + "/E" + e + " - Dev Category Flags: " + devcatflags, true);
            log.MakeLog("-- As hex: " + devcatflags.ToString("X8"), true);
            
            int tgicount = readFile.ReadByte();
            log.MakeLog("P" + packageparsecount + "/E" + e + " - TGI Count: " + tgicount, true);

            if (tgicount != 0){
                log.MakeLog("P" + packageparsecount + "/E" + e + " - TGI Count is not zero. Reading resources.", true);
                ResourceKeyITG resourcekey = new ResourceKeyITG(readFile);
                log.MakeLog(resourcekey.ToString(), true);
                log.MakeLog("GUID: " + resourcekey.ToString(), true);
                GUID = resourcekey.ToString();
            }                           
            
            if (commonblockversion >= 10)
            {
                int packId = readFile.ReadInt16();
                log.MakeLog("P" + packageparsecount + "/E" + e + " - Pack ID: " + packId, true);
                int packFlags = readFile.ReadByte();
                log.MakeLog("P" + packageparsecount + "/E" + e + " - Pack Flags: " + packFlags, true);
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
                log.MakeLog("P" + packageparsecount + "/E" + e + " - Tags Count: " + count, true);
                Tag tags = new Tag(readFile, count);

                List<TagsList> gottags = Readers.GetTagInfo(tags, count);
                foreach (TagsList tag in gottags){
                                if (!itemtags.Exists(x => x.TypeID == tag.TypeID) && !itemtags.Exists(x => x.Description == tag.Description)){
                                    itemtags.Add(tag);  
                                }
                            }
                
            } else {
                uint count = readFile.ReadUInt32();
                List<uint> tagsread = new List<uint>();                
                log.MakeLog("Num tags: " + count, true);
                for (int t = 0; t < count; t++){                    
                    uint tagvalue = readFile.ReadUInt16();
                    tagsread.Add(tagvalue);
                }

                List<TagsList> gottags = Readers.GetTagInfo(tagsread, count);
                foreach (TagsList tag in gottags){
                                if (!itemtags.Exists(x => x.TypeID == tag.TypeID) && !itemtags.Exists(x => x.Description == tag.Description)){
                                    itemtags.Add(tag);  
                                }
                            }
            }
            long location = readFile.BaseStream.Position;
            log.MakeLog("P" + packageparsecount + "/E" + e + " - Reader location: " + location, true);
            uint count2 = readFile.ReadUInt32();
            log.MakeLog("P" + packageparsecount + "/E" + e + " - Selling Point Count: " + count2, true);
            if (count2 > 5){
                log.MakeLog("Selling point count is too high, something went wrong.", true);
            } else {
                Tag sellingtags = new Tag(readFile, count2);

                List<TagsList> gottags = Readers.GetTagInfo(sellingtags, count2);
                foreach (TagsList tag in gottags){
                                if (!itemtags.Exists(x => x.TypeID == tag.TypeID) && !itemtags.Exists(x => x.Description == tag.Description)){
                                    itemtags.Add(tag);  
                                }
                            }
                
            }            

            uint unlockByHash = readFile.ReadUInt32();
            log.MakeLog("P" + packageparsecount + "/E" + e + " - UnlockBy Hash: " + unlockByHash, true);
            
            uint unlockedByHash = readFile.ReadUInt32();
            log.MakeLog("P" + packageparsecount + "/E" + e + " - UnlockedBy Hash: " + unlockedByHash, true);

            int swatchColorSortPriority = readFile.ReadUInt16();
            log.MakeLog("P" + packageparsecount + "/E" + e + " - Swatch Sort Priority: " + swatchColorSortPriority, true);

            ulong varientThumbImageHash = readFile.ReadUInt64();
            log.MakeLog("P" + packageparsecount + "/E" + e + " - Varient Thumb Image Hash: " + varientThumbImageHash, true);
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

        public ReadOBJDIndex(BinaryReader reader){
            this.version = reader.ReadUInt16();
            log.MakeLog("Version: " + this.version, true);
            if (this.version > 150){
                log.MakeLog("Version is not legitimate.", true);
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
        public ReadOBJDEntry(BinaryReader reader, string[] entries, int[] positions){
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
                        log.MakeLog("Name Length: " + namelength, true);
                        log.MakeLog("Reading three empty bytes.", true);
                        log.MakeLog("Byte 1: " + reader.ReadByte().ToString(), true);
                        log.MakeLog("Byte 2: " + reader.ReadByte().ToString(), true);
                        log.MakeLog("Byte 3: " + reader.ReadByte().ToString(), true);
                        this.namebit = reader.ReadBytes(namelength);
                        this.name = Encoding.UTF8.GetString(namebit);
                        log.MakeLog("Name: " + name, true);
                        break;
                    case "790FA4BC": //tuning
                        reader.BaseStream.Position = entrypos;
                        this.tuningnamelength = reader.ReadByte();
                        log.MakeLog("Tuning Name Length: " + tuningnamelength, true);
                        log.MakeLog("Reading three empty bytes.", true);
                        log.MakeLog("Byte 1: " + reader.ReadByte().ToString(), true);
                        log.MakeLog("Byte 2: " + reader.ReadByte().ToString(), true);
                        log.MakeLog("Byte 3: " + reader.ReadByte().ToString(), true);
                        this.tuningbit = reader.ReadBytes(tuningnamelength);
                        this.tuningname = Encoding.UTF8.GetString(tuningbit);
                        log.MakeLog("Tuning Name: " + tuningname, true);
                        break;
                    case "B994039B": //TuningID
                        reader.BaseStream.Position = entrypos;
                        this.tuningid = reader.ReadUInt64(); 
                        break;
                    case "CADED888": //Icon
                        reader.BaseStream.Position = entrypos;
                        preceeding = reader.ReadUInt32();
                        log.MakeLog("Reading preceeding UInt32: " + preceeding, true);
                        preceedingDiv = preceeding / 4;            
                        log.MakeLog("Number of Icon GUIDs: " + preceedingDiv, true);
                        this.icon = new string[preceedingDiv];
                        for (int p = 0; p < preceedingDiv; p++){
                            ResourceKeyITGFlip ricon = new ResourceKeyITGFlip(reader);
                            log.MakeLog(ricon.ToString(), true);
                            log.MakeLog("Icon GUID: " + ricon.ToString(), true);
                            this.icon[p] = ricon.ToString();
                        }
                        break;
                    case "E206AE4F": //Rig
                        reader.BaseStream.Position = entrypos;
                        preceeding = reader.ReadUInt32();
                        log.MakeLog("Reading preceeding UInt32: " + preceeding, true);
                        preceedingDiv = preceeding / 4;
                        log.MakeLog("Number of Rig GUIDs: " + preceedingDiv, true);
                        this.rig = new string[preceedingDiv];
                        for (int p = 0; p < preceedingDiv; p++){
                            ResourceKeyITGFlip rkrig = new ResourceKeyITGFlip(reader);
                            log.MakeLog(rkrig.ToString(), true);
                            log.MakeLog("Rig GUID: " + rkrig.ToString(), true);
                            this.rig[p] = rkrig.ToString();
                        }   
                        break;
                    case "8A85AFF3": //Slot
                        reader.BaseStream.Position = entrypos;
                        preceeding = reader.ReadUInt32();
                        log.MakeLog("Reading preceeding UInt32: " + preceeding, true);
                        preceedingDiv = preceeding / 4;
                        log.MakeLog("Number of Slot GUIDs: " + preceedingDiv, true);
                        this.slot = new string[preceedingDiv];
                        for (int p = 0; p < preceedingDiv; p++){
                            ResourceKeyITGFlip rkslot = new ResourceKeyITGFlip(reader);
                            log.MakeLog(rkslot.ToString(), true);
                            log.MakeLog("Slot GUID: " + rkslot.ToString(), true);
                            this.slot[p] = rkslot.ToString();
                        }            
                        break;
                    case "8D20ACC6": //Model
                        reader.BaseStream.Position = entrypos;
                        preceeding = reader.ReadUInt32();
                        log.MakeLog("Reading preceeding UInt32: " + preceeding, true);
                        preceedingDiv = preceeding / 4;
                        log.MakeLog("Number of Model GUIDs: " + preceedingDiv, true);
                        this.model = new string[preceedingDiv];
                        for (int p = 0; p < preceedingDiv; p++){
                            ResourceKeyITGFlip rkmodel = new ResourceKeyITGFlip(reader);
                            log.MakeLog(rkmodel.ToString(), true);
                            log.MakeLog("Reader is at " + reader.BaseStream.Position, true);
                            log.MakeLog("Model GUID: " + rkmodel.ToString(), true);
                            this.model[p] = rkmodel.ToString();
                        }
                        break;
                    case "6C737AD8": //Footprint
                        reader.BaseStream.Position = entrypos;
                        preceeding = reader.ReadUInt32();
                        log.MakeLog("Reading preceeding UInt32: " + preceeding, true);
                        preceedingDiv = preceeding / 4;
                        log.MakeLog("Number of Footprint GUIDs: " + preceedingDiv, true);
                        this.footprint = new string[preceedingDiv];
                        for (int p = 0; p < preceedingDiv; p++){
                            ResourceKeyITGFlip rkft = new ResourceKeyITGFlip(reader);
                            log.MakeLog(rkft.ToString(), true);
                            log.MakeLog("Reader is at " + reader.BaseStream.Position, true);
                            log.MakeLog("Footprint GUID: " + rkft.ToString(), true);
                            this.footprint[p] = rkft.ToString();                
                        }
                        break;
                    case "E6E421FB": //Components
                        reader.BaseStream.Position = entrypos;
                        this.componentcount = reader.ReadUInt32();
                        log.MakeLog("Reader is at " + reader.BaseStream.Position, true);
                        log.MakeLog("Component count: " + componentcount, true);
                        this.components = new uint[this.componentcount];
                        for (int i = 0; i < this.componentcount; i++){
                            components[i] = reader.ReadUInt32();
                        }
                        break;
                    case "ECD5A95F": //MaterialVariant
                        reader.BaseStream.Position = entrypos;
                        this.materialvariantlength = reader.ReadByte();
                        log.MakeLog("Material Variant Length: " + materialvariantlength, true);
                        log.MakeLog("Reading three empty bytes.", true);
                        log.MakeLog("Byte 1: " + reader.ReadByte().ToString(), true);
                        log.MakeLog("Byte 2: " + reader.ReadByte().ToString(), true);
                        log.MakeLog("Byte 3: " + reader.ReadByte().ToString(), true);
                        this.materialvariantbyte = reader.ReadBytes(materialvariantlength);
                        this.materialvariant = Encoding.UTF8.GetString(materialvariantbyte);
                        log.MakeLog("Material Variant: " + materialvariant, true);
                        break;
                    case "AC8E1BC0": //Unknown1
                        reader.BaseStream.Position = entrypos;

                        break;
                    case "E4F4FAA4": //SimoleonPrice
                        reader.BaseStream.Position = entrypos;
                        this.price = reader.ReadUInt32();
                        log.MakeLog("Price: " + price, true);
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
        

        public void SearchS4Packages(BinaryReader readFile, FileInfo packageinfo, int packageparsecount) {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            log.MakeLog(string.Format("File {0} arrived for processing as Sims 4 file.", packageinfo.Name), true);            
            string txt = string.Format("SELECT * FROM Processing_Reader where Name='{0}'", Methods.FixApostrophesforSQL(packageinfo.Name));
            var queries = GlobalVariables.DatabaseConnection.Query<PackageFile>(txt);
            var query = queries[0];
            GlobalVariables.DatabaseConnection.Delete(query);
            var pk = new PackageFile { ID = query.ID, Name = packageinfo.Name, Location = packageinfo.FullName, Game = 4, Broken = false, Status = "Processing"};
            GlobalVariables.DatabaseConnection.Insert(pk);
            log.MakeLog("Got package parse count.", true);   
            log.MakeLog("Incrementing packages read.", true);
            
                     
        
            //Misc Vars
            string test = "";
            
            //locations

            long entrycountloc = 36;
            long indexRecordPositionloc = 64;

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
            //MemoryStream dbpfFile = Methods.ReadBytesToFile(packageinfo.FullName);
            //BinaryReader readFile = new BinaryReader(dbpfFile);
            
            //log opening file
            log.MakeLog(string.Format("Reading package # {0}/{1}: {2}", packageparsecount, GlobalVariables.PackageCount, packageinfo.Name), true);
            thisPackage.PackageName = packageinfo.Name;
            thisPackage.Location = packageinfo.FullName;            
            thisPackage.Game = 4;
            thisPackage.GameString = "The Sims 4";
            thisPackage.FileSize = (int)packageinfo.Length;
            log.MakeLog(string.Format("Package #{0} registered as {1} and meant for Sims 4", packageparsecount, packageinfo.FullName), true);

            readFile.BaseStream.Position = 0;
            //start actually reading the package             
            //entrycount

            /*for (int l = 0; l < 1000; l++){
                uint x = readFile.ReadUInt32();
                log.MakeLog(string.Format("Browsing file: {0} ({1})", x, x.ToString("X8")), true);
            }*/

            readFile.BaseStream.Position = entrycountloc;

            uint entrycount = readFile.ReadUInt32();
            log.MakeLog(string.Format("Entry Count: {0}", entrycount.ToString()), true);
            log.MakeLog("Entry Count Location: " + readFile.BaseStream.Position, true);
            
            //record position low
            uint indexRecordPositionLow = readFile.ReadUInt32();
            log.MakeLog(string.Format("IndexRecordPositionLow: {0}", indexRecordPositionLow.ToString()), true);
            
            //index record size
            uint indexRecordSize = readFile.ReadUInt32();
            log.MakeLog(string.Format("IndexRecordSize: {0}", indexRecordSize.ToString()), true);
            
            readFile.BaseStream.Position = indexRecordPositionloc;

            ulong indexRecordPosition = readFile.ReadUInt64();
            log.MakeLog(string.Format("Index Record Position: {0}", indexRecordPosition.ToString()), true);
            
                        
            byte[] headersize = new byte[96];
            long here = 100;
            long movedto = 0;

            /*            //dbpf
            test = Encoding.ASCII.GetString(readFile.ReadBytes(4));
            log.MakeLog("DBPF: " + test, true);
            
            //major
            uint testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Major :" + test, true);
            
            //minor
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Minor : " + test, true);
            
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unknown : " + test, true);
            
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unknown : " + test, true);
            
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unknown : " + test, true);
            
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Created : " + test, true);
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Modified : " + test, true);
            
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Index Major : " + test, true);
            
            //entrycount
            uint entrycount = readFile.ReadUInt32();
            test = entrycount.ToString();
            log.MakeLog("Entry Count: " + test, true);
            
            //record position low
            uint indexRecordPositionLow = readFile.ReadUInt32();
            test = indexRecordPositionLow.ToString();
            log.MakeLog("indexRecordPositionLow: " + test, true);
            
            //index record size
            uint indexRecordSize = readFile.ReadUInt32();
            test = indexRecordSize.ToString();
            log.MakeLog("indexRecordSize: " + test, true);
            //unused
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unused Trash Index offset: " + test, true);
            
            //unused
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unused Trash Index size: " + test, true);
            
            //unused
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unused Index Minor Version: " + test, true);
            
            //unused but 3 for historical reasons
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unused, 3 for historical reasons: " + test, true);
            
            ulong indexRecordPosition = readFile.ReadUInt64();
            test = indexRecordPosition.ToString();
            log.MakeLog("Inded Record Position: " + test, true);
            //unused
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unused Unknown:" + test, true);
            
            //unused six bytes
            test = Encoding.ASCII.GetString(readFile.ReadBytes(24));
            log.MakeLog("Unused: " + test, true);*/

            if (indexRecordPosition != 0){
                long indexseek = (long)indexRecordPosition - headersize.Length;
                movedto = here + indexseek;
                readFile.BaseStream.Position = here + indexseek;                
            } else {
                movedto = here + indexRecordPositionLow;
                readFile.BaseStream.Position = here + indexRecordPositionLow;
            }

            readFile.BaseStream.Position = (long)indexRecordPosition;
            uint indextype = readFile.ReadUInt32();            

            long streamsize = readFile.BaseStream.Length;
            int indexbytes = ((int)entrycount * 32);
            long indexpos = 0;
            log.MakeLog(string.Format("Streamsize is {0}", streamsize), true);
            if ((int)movedto + indexbytes != streamsize){
                int entriesfound = 0;
                long spacebetween = 0;
                log.MakeLog("Streamsize does not match!", true);
                readFile.BaseStream.Position = movedto - 400;
                List<long> entrylocs = new List<long>();
                uint item;
                while(readFile.BaseStream.Length > readFile.BaseStream.Position){                    
                    item = readFile.ReadUInt32();
                    var result = TypeListings.AllTypesS4.Where(x => x.typeID == item.ToString("X8"));
                    //log.MakeLog(string.Format("Read byte at {0}: {1}", readFile.BaseStream.Position, item.ToString("X8")), true);
                    if (result.Any()){
                        indexEntry holderEntry = new indexEntry();
                        holderEntry.typeID = item.ToString("X8");
                        log.MakeLog(string.Format("P{0}/E{1} - Index Entry TypeID: {2}", packageparsecount, entriesfound, holderEntry.typeID), true);
                        log.MakeLog(string.Format("Found entry{2} - {0} at location {1}!", item, readFile.BaseStream.Position, entriesfound), true);
                        entrylocs.Add(readFile.BaseStream.Position);
                        if (entriesfound == 0){
                            indexpos = readFile.BaseStream.Position - 4;
                        }
                        entriesfound++;
                        if (entriesfound == entrycount){
                            log.MakeLog(string.Format("Found {0} entries. Breaking.", entrycount), true);
                            break;
                        }
                    }
                }
                readFile.BaseStream.Position = indexpos;
                entrycount = 0;
                foreach (int loc in entrylocs){
                    indexEntry holderEntry = new indexEntry(); 
                    readFile.BaseStream.Position = loc - 4;
                    if (indextype == 2){
                        holderEntry.typeID = readFile.ReadUInt32().ToString("X8");
                        log.MakeLog(string.Format("P{0}/E{1} - Index Entry TypeID: {2}", packageparsecount, entrycount, holderEntry.typeID), true);

                        if(TypeListings.AllTypesS4.Exists(x => x.typeID == holderEntry.typeID)){
                        var tid = from types in TypeListings.AllTypesS4
                                where types.typeID == holderEntry.typeID
                                select types.desc;
                        var ih = tid.ToList();
                        fileHas.Add(new fileHasList {TypeID = ih[0], Location = loc});
                        log.MakeLog(string.Format("File {0} has {1} at location {2}.", thisPackage.PackageName, ih[0], loc), true);
                        } else {
                            fileHas.Add(new fileHasList() { TypeID = holderEntry.typeID, Location = loc});
                        }              

                        string instanceid1 = (readFile.ReadUInt32() << 32).ToString("X8");
                        string instanceid2 = (readFile.ReadUInt32() << 32).ToString("X8");
                        holderEntry.instanceID = string.Format("{0}{1}", instanceid1,instanceid2);
                        if (holderEntry.instanceID != "0000000000000000"){
                            allInstanceIDs.Add(holderEntry.instanceID);
                        }
                        
                        log.MakeLog(string.Format("P{0}/E{1} - InstanceID: {2}", packageparsecount, entrycount, holderEntry.instanceID), true);
                        
                        holderEntry.position = readFile.ReadUInt32();
                        log.MakeLog(string.Format("P{0}/E{1} - Index Entry Position: {2}", packageparsecount, entrycount, holderEntry.position.ToString("X8")), true);                        

                        holderEntry.fileSize = readFile.ReadUInt32();
                        log.MakeLog(string.Format("P{0}/E{1} - File Size: {2}", packageparsecount, entrycount, holderEntry.fileSize.ToString("X8")), true);

                        holderEntry.memSize = readFile.ReadUInt32();
                        log.MakeLog(string.Format("P{0}/E{1} - Mem Size: {2}", packageparsecount, entrycount, holderEntry.memSize.ToString("X8")), true);

                        //holderEntry.compressionType = readFile.ReadUInt16().ToString("X4");
                        //log.MakeLog(string.Format("P{0}/E{1} - Compression Type: {2}", packageparsecount, entrycount, holderEntry.compressionType), true);

                        //readFile.BaseStream.Position = readFile.BaseStream.Position + 2;

                        indexData.Add(holderEntry);

                        holderEntry = null;




                        entrycount++;
                    }
                }
            } else {
                log.MakeLog("Streamsize matches.", true);
                long movedhere = readFile.BaseStream.Position;
                uint testpos = readFile.ReadUInt32();
                if (testpos != 0){
                    log.MakeLog(string.Format("Read first entry TypeID and it read as {0}, returning to read entries.", testpos.ToString("X8")), true);
                    readFile.BaseStream.Position = movedto;
                } else if (testpos == 80000000) {
                    long moveback = movedhere - 4;
                    readFile.BaseStream.Position = moveback;
                } else {
                    log.MakeLog(string.Format("Read first entry TypeID and it read as {0}, moving forward.", testpos.ToString("X8")), true);
                }

                for (int i = 0; i < entrycount; i++){
                    indexEntry holderEntry = new indexEntry();                
                    holderEntry.typeID = readFile.ReadUInt32().ToString("X8");
                    log.MakeLog(string.Format("P{0}/E{1} - Index Entry TypeID: {2}", packageparsecount, i, holderEntry.typeID), true);

                    if (holderEntry.typeID == "7FB6AD8A"){
                        thisPackage.Type = "Merged Package";
                        GlobalVariables.DatabaseConnection.InsertWithChildren(thisPackage);
                        log.MakeLog(string.Format("Package {0} is a merged package, and cannot be processed in this manner right now. Package will either need unmerging or to be sorted manually.", thisPackage.Location), false);
                        log.MakeLog(string.Format("Added {0} to packages database successfully.", thisPackage.PackageName), true);
                        txt = string.Format("SELECT * FROM Processing_Reader where Name='{0}'", Methods.FixApostrophesforSQL(packageinfo.Name));
                        var mergedquery = GlobalVariables.DatabaseConnection.Query<PackageFile>(txt);                    
                        GlobalVariables.DatabaseConnection.Delete(mergedquery[0]);            
                        readFile.Dispose();
                        sw.Stop();
                        TimeSpan tss = sw.Elapsed;
                        string elapsedtimee = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                            tss.Hours, tss.Minutes, tss.Seconds,
                                            tss.Milliseconds / 10);
                        GlobalVariables.packagesRead++;
                        log.MakeLog(string.Format("Reading file {0} took {1}", thisPackage.PackageName, elapsedtimee), true);
                        log.MakeLog(string.Format("Closing package # {0}/{1}: {2}", packageparsecount, GlobalVariables.PackageCount, packageinfo.Name), true);
                        return;
                    }

                    if(TypeListings.AllTypesS4.Exists(x => x.typeID == holderEntry.typeID)){
                        var tid = from types in TypeListings.AllTypesS4
                                where types.typeID == holderEntry.typeID
                                select types.desc;
                        var ih = tid.ToList();
                        fileHas.Add(new fileHasList {TypeID = ih[0], Location = i});
                        log.MakeLog(string.Format("File {0} has {1} at location {2}.", thisPackage.PackageName, ih[0], i), true);
                    } else {
                        fileHas.Add(new fileHasList() { TypeID = holderEntry.typeID, Location = i});
                    }                

                    holderEntry.groupID = readFile.ReadUInt32().ToString("X8");
                    log.MakeLog(string.Format("P{0}/E{1} - Index Entry GroupID: {2}", packageparsecount, i, holderEntry.groupID), true);
                    
                    string instanceid1 = (readFile.ReadUInt32() << 32).ToString("X8");
                    string instanceid2 = (readFile.ReadUInt32() << 32).ToString("X8");
                    holderEntry.instanceID = string.Format("{0}{1}", instanceid1,instanceid2);
                    allInstanceIDs.Add(holderEntry.instanceID);
                    log.MakeLog(string.Format("P{0}/E{1} - InstanceID: {2}", packageparsecount, i, holderEntry.instanceID), true);

                    uint testin = readFile.ReadUInt32();
                    holderEntry.position = (long)testin;
                    log.MakeLog(string.Format("P{0}/E{1} - Position: {2}", packageparsecount, i, holderEntry.position), true);

                    holderEntry.fileSize = readFile.ReadUInt32();
                    log.MakeLog(string.Format("P{0}/E{1} - File Size: {2}", packageparsecount, i, holderEntry.fileSize), true);

                    holderEntry.memSize = readFile.ReadUInt32();
                    log.MakeLog(string.Format("P{0}/E{1} - Mem Size: {2}", packageparsecount, i, holderEntry.memSize), true);

                    holderEntry.compressionType = readFile.ReadUInt16().ToString("X4");
                    log.MakeLog(string.Format("P{0}/E{1} - Compression Type: {2}", packageparsecount, i, holderEntry.compressionType), true);

                    readFile.BaseStream.Position = readFile.BaseStream.Position + 2;

                    indexData.Add(holderEntry);

                    holderEntry = null;
                }
            }

            //log.MakeLog("This package contains: ", true);            
            /*foreach (fileHasList type in fileHas){
                log.MakeLog(type.term + " at location " + type.location, true);
            }*/

            if(fileHas.Exists(x => x.TypeID == "S4SM")) {
                log.MakeLog(string.Format("P{0} - {2} is a merged package and cannot be processed until it has been unmerged.", packageparsecount, thisPackage.PackageName), true);
                thisPackage.Type = "Merged Package";
            } else {
                if ((fileHas.Exists(x => x.TypeID == "CASP"))){
                    List<int> entryspots = new List<int>();
                    /*int fh = 0;
                    foreach (fileHasList item in fileHas) {
                        if (item.term == "CASP"){
                            entryspots.Add(fh);                       
                        }
                        fh++;
                    }*/

                    var csp = from has in fileHas
                            where has.TypeID == "CASP"
                            select has.Location;
                    entryspots = csp.ToList();

                    int caspc = 0;
                    foreach (int e in entryspots){
                        log.MakeLog(string.Format("P{0} - Opening CASP #{1}", packageparsecount, e), true);
                        if (indexData[e].compressionType == "5A42"){
                        readFile.BaseStream.Position = indexData[e].position;
                        long entryEnd = indexData[e].position + indexData[e].memSize;

                        log.MakeLog(string.Format("P{0}/CASP{1} - Position: {2}", packageparsecount, e, indexData[e].position), true);
                        log.MakeLog(string.Format("P{0}/CASP{1} - Filesize: {2}", packageparsecount, e, indexData[e].fileSize), true);
                        log.MakeLog(string.Format("P{0}/CASP{1} - Memsize: {2}", packageparsecount, e, indexData[e].memSize), true);
                        log.MakeLog(string.Format("P{0}/CASP{1} - Ends at: {2}", packageparsecount, e, entryEnd), true);

                        Stream decomps = S4Decryption.Decompress(Methods.ReadEntryBytes(readFile, (int)indexData[e].memSize));
                        
                        BinaryReader decompbr = new BinaryReader(decomps);

                        uint version = decompbr.ReadUInt32();
                        log.MakeLog(string.Format("P{0}/CASP{1} [Decompressed] - Version: {2}", packageparsecount, e, version), true);
                        uint tgioffset = decompbr.ReadUInt32() +8;
                        log.MakeLog(string.Format("P{0}/CASP{1} [Decompressed] - TGI Offset: {2} \n -- As Hex: {3}", packageparsecount, e, tgioffset, tgioffset.ToString("X8")), true);
                        uint numpresets = decompbr.ReadUInt32();
                        log.MakeLog(string.Format("P{0}/CASP{1} [Decompressed] - Number of Presets: {2} \n -- As Hex: {3}", packageparsecount, e, numpresets, numpresets.ToString("X8")), true);
                        using (var reader = new BinaryReader(decomps, Encoding.BigEndianUnicode, true))
                        {
                            thisPackage.Title = reader.ReadString();
                        }
                        log.MakeLog(string.Format("P{0}/CASP{1} [Decompressed] - Title: {2}", packageparsecount, e, thisPackage.Title), true);

                        float sortpriority = decompbr.ReadSingle();
                        log.MakeLog(string.Format("P{0}/CASP{1} [Decompressed] - Sort Priority: {2}", packageparsecount, e, sortpriority), true);

                        int secondarySortIndex = decompbr.ReadUInt16();
                        log.MakeLog(string.Format("P{0}/CASP{1} [Decompressed] - Secondary Sort Index: {2}", packageparsecount, e, secondarySortIndex), true);

                        uint propertyid = decompbr.ReadUInt32();
                        log.MakeLog(string.Format("P{0}/CASP{1} [Decompressed] - Property ID: {2}", packageparsecount, e, propertyid), true);
                        
                        uint auralMaterialHash = decompbr.ReadUInt32();
                        log.MakeLog(string.Format("P{0}/CASP{1} [Decompressed] - Aural Material Hash: {2}", packageparsecount, e, sortpriority), true);

                        if (version <= 42){
                            log.MakeLog(string.Format("Version is <= 42: {0}", version), true);
                            int[] parameterFlag = new int[1];
                            parameterFlag[0] = (int)decompbr.ReadUInt16();
                            BitArray parameterFlags = new BitArray(parameterFlag);
                            log.MakeLog(parameterFlags.Length.ToString(), true);
                            for(int p = 0; p < 16; p++)
                            {
                                if (parameterFlags[p] == true) {
                                    allFlags.Add(parameters[p]);
                                }
                                log.MakeLog(string.Format("P{0}/CASP{1} [Decompressed] - Function Sort Flag [{2}]: {3}, {4}", packageparsecount, e, p, parameters[p], parameterFlags[p].ToString()), true);
                            } 
                            
                        } else if (version >= 43){
                            log.MakeLog(string.Format("Version is >= 43: {0}", version), true);
                            int[] parameterFlag = new int[1];
                            parameterFlag[0] = (int)decompbr.ReadUInt16();
                            BitArray parameterFlags = new BitArray(parameterFlag);

                            for(int pfc = 0; pfc < 16; pfc++){
                                if (parameterFlags[pfc] == true) {
                                    allFlags.Add(parameters[pfc]);
                                }
                                log.MakeLog(string.Format("P{0}/CASP{1} [Decompressed] - Function Sort Flag [{2}]: {3}, {4}", packageparsecount, e, pfc, parameters[pfc], parameterFlags[pfc].ToString()), true);
                            }                            
                        }
                            ulong excludePartFlags = decompbr.ReadUInt64();
                            log.MakeLog(string.Format("P{0}/CASP{1} [Decompressed] - Exclude Part Flags: {2}", packageparsecount, e, excludePartFlags.ToString("X16")), true);
                            ulong excludePartFlags2 = decompbr.ReadUInt64();
                            log.MakeLog(string.Format("P{0}/CASP{1} [Decompressed] - Exclude Part Flags 2: {2}", packageparsecount, e, excludePartFlags2.ToString("X16")), true);
                            ulong excludeModifierRegionFlags = decompbr.ReadUInt64();
                            log.MakeLog(string.Format("P{0}/CASP{1} [Decompressed] - Exclude Modifier Region Flags: {2}", packageparsecount, e, excludeModifierRegionFlags.ToString("X16")), true);

                        if (version >= 37){
                            log.MakeLog(string.Format(">= 37, Version: {0}", version), true);
                            uint count = decompbr.ReadByte();
                            log.MakeLog(string.Format("P{0}/CASP{1} [Decompressed] - Tag Count: {2}", packageparsecount, e, count.ToString()), true);
                            decompbr.ReadByte();
                            CASTag16Bit tags = new CASTag16Bit(decompbr, count);                            
                            List<TagsList> gottags = Readers.GetTagInfo(tags, count);
                            foreach (TagsList tag in gottags){
                                if (!itemtags.Exists(x => x.TypeID == tag.TypeID) && !itemtags.Exists(x => x.Description == tag.Description)){
                                    itemtags.Add(tag);  
                                }
                            }
                                                      
                        } 
                        else 
                        {
                            uint count = decompbr.ReadByte();
                            log.MakeLog(string.Format("P{0}/CASP{1} [Decompressed] - Tag Count: {2}", packageparsecount, e, count.ToString()), true);
                            decompbr.ReadByte();
                            CASTag16Bit tags = new CASTag16Bit(decompbr, count);
                            
                            List<TagsList> gottags = Readers.GetTagInfo(tags, count);
                            foreach (TagsList tag in gottags){
                                if (!itemtags.Exists(x => x.TypeID == tag.TypeID) && !itemtags.Exists(x => x.Description == tag.Description)){
                                    itemtags.Add(tag);  
                                }
                            }

                            }

                            uint simoleonprice = decompbr.ReadUInt32();
                            log.MakeLog(string.Format("P{0}/CASP{1} [Decompressed] - Simoleon Price: {2}", packageparsecount, e, simoleonprice.ToString()), true);
                            uint partTitleKey = decompbr.ReadUInt32();
                            log.MakeLog(string.Format("P{0}/CASP{1} [Decompressed] - Part Title Key: {2}", packageparsecount, e, partTitleKey.ToString()), true);
                            uint partDescriptionKey = decompbr.ReadUInt32();
                            log.MakeLog(string.Format("P{0}/CASP{1} [Decompressed] - Part Description Key: {2}", packageparsecount, e, partDescriptionKey.ToString()), true);                            
                            if (version >= 43) {
                                uint createDescriptionKey = decompbr.ReadUInt32();
                            }
                            int uniqueTextureSpace = decompbr.ReadByte();
                            log.MakeLog(string.Format("P{0}/CASP{1} [Decompressed] - Unique Texture Space: {2}", packageparsecount, e, uniqueTextureSpace.ToString("X8")), true);
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
                                thisPackage.Function = string.Format("Unidentified function (contact SinfulSimming). Code: {0}", bodytype.ToString());
                            }                            
                            log.MakeLog(string.Format("P{0}/CASP{1} [Decompressed] - Bodytype: {2}", packageparsecount, e, bodytype.ToString()), true);
                            uint bodytypesubtype = decompbr.ReadUInt16();
                            log.MakeLog(string.Format("P{0}/CASP{1} [Decompressed] - Body Sub Type: {2}", packageparsecount, e, bodytypesubtype.ToString()), true);
                            decompbr.ReadUInt32();                        
                            uint agflags = decompbr.ReadUInt32();
                            log.MakeLog(string.Format("P{0}/CASP{1} [Decompressed] - Ag Flags: {2}", packageparsecount, e, agflags.ToString("X8")), true);

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
                                log.MakeLog("No age/gender flags present.", true);
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
                                log.MakeLog("Adult (nothing else)", true);
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
                                log.MakeLog("Adult Female", true);
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
                                log.MakeLog("Adult Male", true);
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
                                log.MakeLog("Adult/Elder/Teen, Female", true);
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
                                log.MakeLog("Everything", true);
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
                                log.MakeLog("Child of either gender", true);
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
                                    log.MakeLog(string.Format("P{0}/CASP{1} [Decompressed] - Packflag is: {2}", packageparsecount, e, check.ToString()), true);
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


                if ((fileHas.Exists(x => x.TypeID == "COBJ"))){
                    List<int> entryspots = new List<int>();
                    /*int fh = 0;
                    foreach (fileHasList item in fileHas) {
                        if (item.term == "CASP"){
                            entryspots.Add(fh);                       
                        }
                        fh++;
                    }*/

                    var csp = from has in fileHas
                            where has.TypeID == "COBJ"
                            select has.Location;
                    entryspots = csp.ToList();

                    /*int fh = 0;
                    foreach (fileHasList item in fileHas) {
                        if (item.term == "COBJ"){
                            entryspots.Add(fh);                       
                        }
                        fh++;
                    }    */
                    int cobjc = 0;
                    foreach (int e in entryspots){
                        log.MakeLog("P" + packageparsecount + "/E" + e + " - Opening COBJ #" + cobjc, true);
                        if (indexData[e].compressionType == "5A42"){                                
                                //var here = readFile.BaseStream.Position;
                                readFile.BaseStream.Position = indexData[e].position;
                                //dbpfFile.Seek(, SeekOrigin.Begin);                            
                                int entryEnd = (int)readFile.BaseStream.Position + (int)indexData[e].memSize;
                                log.MakeLog("P" + packageparsecount + "/E" + e + " - Position: " + indexData[e].position, true);
                                log.MakeLog("P" + packageparsecount + "/E" + e + " - Filesize: " + indexData[e].fileSize, true);
                                log.MakeLog("P" + packageparsecount + "/E" + e + " - Memsize: " + indexData[e].memSize, true);
                                log.MakeLog("P" + packageparsecount + "/E" + e + " - Entry ends at " + entryEnd, true);
                                Stream decomps = S4Decryption.Decompress(Methods.ReadEntryBytes(readFile, (int)indexData[e].memSize));
                                
                                BinaryReader decompbr = new BinaryReader(decomps);                           
                                
                                ReadCOBJ rc = new ReadCOBJ(decompbr, packageparsecount, e, itemtags);

                                if((!allGUIDS.Contains(rc.GUID)) && rc.GUID != "null"){
                                    allGUIDS.Add(rc.GUID);
                                }

                                foreach (TagsList tag in rc.itemtags) {
                                    if (!itemtags.Exists(x => x.TypeID == tag.TypeID) && !itemtags.Exists(x => x.Description == tag.Description)){
                                        itemtags.Add(tag);  
                                    }                                    
                                }
                            decompbr.Dispose();
                            decomps.Dispose();
                                
                        } else if (indexData[e].compressionType == "00"){
                            //dbpfFile.Seek(, SeekOrigin.Begin); 
                            readFile.BaseStream.Position = indexData[e].position;                           
                            int entryEnd = (int)readFile.BaseStream.Position + (int)indexData[e].memSize;
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Position: " + indexData[e].position, true);
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Filesize: " + indexData[e].fileSize, true);
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Memsize: " + indexData[e].memSize, true);
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Entry ends at " + entryEnd, true);
                            ReadCOBJ rc = new ReadCOBJ(readFile, packageparsecount, e, itemtags);                            
                            if((!allGUIDS.Contains(rc.GUID)) && rc.GUID != "null"){
                                allGUIDS.Add(rc.GUID);
                            }

                            foreach (TagsList tag in rc.itemtags) {
                                if (!itemtags.Exists(x => x.TypeID == tag.TypeID) && !itemtags.Exists(x => x.Description == tag.Description)){
                                    itemtags.Add(tag);  
                                }                                    
                            }
                        }

                        

                        

                        cobjc++;
                    }
                }

                if ((fileHas.Exists(x => x.TypeID == "OBJD"))){
                    List<int> entryspots = new List<int>();
                    int fh = 0;
                    foreach (fileHasList item in fileHas) {
                        if (item.TypeID == "OBJD"){
                            entryspots.Add(fh);                       
                        }
                        fh++;
                    }    
                    int objdc = 0;
                    foreach (int e in entryspots){
                        log.MakeLog("P" + packageparsecount + "/E" + e + " - Opening OBJD #" + objdc, true);
                        if (indexData[e].compressionType == "5A42"){
                                //dbpfFile.Seek(, SeekOrigin.Begin);
                                //var here = readFile.BaseStream.Position;
                                readFile.BaseStream.Position = indexData[e].position;                       
                                int entryEnd = (int)readFile.BaseStream.Position + (int)indexData[e].memSize;
                                log.MakeLog("P" + packageparsecount + "/E" + e + " - Position: " + indexData[e].position, true);
                                log.MakeLog("P" + packageparsecount + "/E" + e + " - Filesize: " + indexData[e].fileSize, true);
                                log.MakeLog("P" + packageparsecount + "/E" + e + " - Memsize: " + indexData[e].memSize, true);
                                log.MakeLog("P" + packageparsecount + "/E" + e + " - Entry ends at " + entryEnd, true);
                                Stream decomps = S4Decryption.Decompress(Methods.ReadEntryBytes(readFile, (int)indexData[e].memSize));
                                
                                BinaryReader decompbr = new BinaryReader(decomps);

                                ReadOBJDIndex readOBJD = new ReadOBJDIndex(decompbr);
                                log.MakeLog("There are " + readOBJD.count + " entries to read.", true);
                                objdentries = new string[readOBJD.count];
                                objdpositions = new int[readOBJD.count];
                                for (int f = 0; f < readOBJD.count; f++){
                                    log.MakeLog("Entry " + f + ": ", true);
                                    log.MakeLog("--- Type: " + readOBJD.entrytype[f].ToString("X8"), true);
                                    objdentries[f] = readOBJD.entrytype[f].ToString();
                                    objdpositions[f] = (int)readOBJD.position[f];
                                    log.MakeLog("--- Position " + readOBJD.position[f], true);
                                }
                                log.MakeLog("P" + packageparsecount + "/E" + e + " - Reader is at " + decompbr.BaseStream.Position, true);
                                decompbr.BaseStream.Position = readOBJD.position[0];
                                ReadOBJDEntry readobjdentry = new ReadOBJDEntry(decompbr, objdentries, objdpositions);
                                thisPackage.Title = readobjdentry.name;
                                log.MakeLog("Adding components to package: ", true);
                                for (int c = 0; c < readobjdentry.componentcount; c++){
                                    log.MakeLog(readobjdentry.components[c].ToString(), true);
                                    log.MakeLog(readobjdentry.components[c].ToString("X8"), true);
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
            




            log.MakeLog("All methods complete, moving on to getting info.", true);

            List<TypeCounter> typecount = new List<TypeCounter>();
            var typeDict = new Dictionary<string, int>();

            log.MakeLog("Making dictionary.", true);

            foreach (fileHasList item in fileHas){
                foreach (typeList type in TypeListings.AllTypesS4){
                    if (type.desc == item.TypeID){
                        log.MakeLog("Added " + type.desc + " to dictionary.", true);
                        typeDict.Increment(type.desc);
                    }
                }
            }

            foreach (KeyValuePair<string, int> type in typeDict){
                TypeCounter tc = new TypeCounter();
                tc.Type = type.Key;
                tc.Count = type.Value;
                log.MakeLog("There are " + tc.Count + " of " + tc.Type + " in this package.", true);
                typecount.Add(tc);
            }

            thisPackage.Entries.AddRange(typecount);
            thisPackage.FileHas.AddRange(fileHas);
            //ifs

            int casp;int geom;int rle;int rmap;int thum;int img;int xml;int clhd;int clip;int stbl;int cobj;int ftpt;int lite;int thm;int mlod;int modl;int mtbl;int objd;int rslt;int tmlt;int ssm;int lrle;int bond;int cpre;int dmap;int smod;int bgeo;int hotc;

            

            distinctFlags = allFlags.Distinct().ToList();
            thisPackage.Flags.AddRange(distinctFlags);

            distinctItemTags = itemtags.Distinct().ToList();
            thisPackage.CatalogTags.AddRange(distinctItemTags);

            distinctInstanceIDs = allInstanceIDs.Distinct().ToList();
            thisPackage.InstanceIDs.AddRange(distinctInstanceIDs);


            distinctGUIDS = allGUIDS.Distinct().ToList();
            thisPackage.GUIDs.AddRange(distinctGUIDS);

            
                     
            log.MakeLog(string.Format("P{0}: Checking {1} against override IDs.", packageparsecount,thisPackage.PackageName), true);
            
            

            ParallelOptions parallelSettings = new ParallelOptions() { MaxDegreeOfParallelism = 2000};
            
            bool found;

            SimsPackage overrideContainer = new SimsPackage();

            
            List<string> FoundIDs = new List<string>();
            foreach (string iid in thisPackage.InstanceIDs){ 
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
                                        log.MakeLog(string.Format("P{0}: {1} is an override.", packageparsecount, iid), true);
                                        thisPackage.FunctionSubcategory = tp;
                                        thisPackage.OverriddenInstances.Add(iid);
                                        if ((!String.IsNullOrWhiteSpace(ow)) && (ow != " ")){
                                            thisPackage.OverriddenItems.Add(pack + ": " + ow);
                                        }                                        
                                        thisPackage.Override = true;
                                        thisPackage.Type = "OVERRIDE";
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
            };

            

            
            if (thisPackage.Override != true){
                log.MakeLog("No overrides were found. Checking other options.", true);
                if ((typeDict.TryGetValue("S4SM", out ssm) && ssm >= 1)){
                thisPackage.Type = "Merged Package";
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);
                } else if (!String.IsNullOrWhiteSpace(thisPackage.Function)) {
                    thisPackage.Type = thisPackage.Function;
                } else if ((typeDict.TryGetValue("BGEO", out bgeo) && bgeo >= 1) && (typeDict.TryGetValue("HOTC", out hotc) && hotc >= 1) && (typeDict.TryGetValue("SMOD", out smod) && smod >= 1)){
                    thisPackage.Type = "Slider";
                    log.MakeLog("This is a " + thisPackage.Type + "!!", true);
                } else if ((typeDict.TryGetValue("BOND", out bond) && bond >= 1) && (typeDict.TryGetValue("CPRE", out cpre) && cpre >= 1) && (typeDict.TryGetValue("DMAP", out dmap) && dmap >= 1) && (typeDict.TryGetValue("SMOD", out smod) && smod >= 1)){
                    thisPackage.Type = "Preset";
                    log.MakeLog("This is a " + thisPackage.Type + "!!", true);
                } else if ((typeDict.TryGetValue("RLE2", out rle) && rle >= 1) && (typeDict.TryGetValue("CASP", out casp) && casp >= 1) && (typeDict.TryGetValue("GEOM", out geom) && geom <= 0)){
                    thisPackage.Type = "CAS Recolor";
                    log.MakeLog("This is a " + thisPackage.Type + "!!", true);
                } else if ((typeDict.TryGetValue("LRLE", out lrle) && lrle >= 1) && (typeDict.TryGetValue("CASP", out casp) && casp >= 1) && (typeDict.TryGetValue("GEOM", out geom) && geom <= 0)){
                    thisPackage.Type = "Makeup";
                    log.MakeLog("This is a " + thisPackage.Type + "!!", true);
                } else if ((typeDict.TryGetValue("CASP", out casp) && casp <= 0) && (typeDict.TryGetValue("GEOM", out geom) && geom >= 1) && (typeDict.TryGetValue("RLE2", out rle) && rle >= 1) && (typeDict.TryGetValue("RMAP", out rmap) && rmap >= 1) && (typeDict.TryGetValue("_IMG", out img) && img <= 0)){
                    thisPackage.Type = "Hair Mesh";
                    thisPackage.Mesh = true;
                    thisPackage.Recolor = false;
                    log.MakeLog("This is a " + thisPackage.Type + "!!", true);
                } else if ((typeDict.TryGetValue("CASP", out casp) && casp >= 1) && (typeDict.TryGetValue("GEOM", out geom) && geom <= 0) && (typeDict.TryGetValue("RLE2", out rle) && rle >= 1) && (typeDict.TryGetValue("RMAP", out rmap) && rmap >= 1) && (typeDict.TryGetValue("_IMG", out img) && img >= 1)){
                    thisPackage.Type = "Hair Recolor";
                    thisPackage.Mesh = false;
                    thisPackage.Recolor = true;
                    log.MakeLog("This is a " + thisPackage.Type + "!!", true);
                } else if ((typeDict.TryGetValue("CASP", out casp) && casp >= 1) && (typeDict.TryGetValue("GEOM", out geom) && geom >= 1) && (typeDict.TryGetValue("RLE2", out rle) && rle >= 1) && (typeDict.TryGetValue("RMAP", out rmap) && rmap >= 1) && (typeDict.TryGetValue("_IMG", out img) && img >= 1)){
                    thisPackage.Type = "Hair";
                    thisPackage.Mesh = true;
                    thisPackage.Recolor = true;
                    log.MakeLog("This is a " + thisPackage.Type + "!!", true);
                } else if ((typeDict.TryGetValue("_IMG", out img) && img >= 1) && (typeDict.TryGetValue("CLHD", out clhd) && clhd >= 1) && (typeDict.TryGetValue("STBL", out stbl) && stbl >= 1)){
                    thisPackage.Type = "Pose Pack";
                    log.MakeLog("This is a " + thisPackage.Type + "!!", true);
                } else if ((typeDict.TryGetValue("_IMG", out img) && img >= 1) && (typeDict.TryGetValue("COBJ", out cobj) && cobj >= 1) && (typeDict.TryGetValue("FTPT", out ftpt) && ftpt >= 1) && (typeDict.TryGetValue("MLOD", out mlod) && mlod >= 1) && (typeDict.TryGetValue("MODL", out modl) && modl >= 1) && (typeDict.TryGetValue("MTBL", out mtbl) && mtbl <= 0)  && (typeDict.TryGetValue("_IMG", out img) && img <= 0) && (typeDict.TryGetValue("OBJD", out objd) && objd >= 1)){
                    thisPackage.Type = "Object Mesh";
                    thisPackage.Mesh = true;
                    thisPackage.Recolor = false;
                    log.MakeLog("This is a " + thisPackage.Type + "!!", true);
                } else if ((typeDict.TryGetValue("_IMG", out img) && img >= 1) && (typeDict.TryGetValue("COBJ", out cobj) && cobj >= 1) && (typeDict.TryGetValue("FTPT", out ftpt) && ftpt >= 1) && (typeDict.TryGetValue("MLOD", out mlod) && mlod >= 1) && (typeDict.TryGetValue("MODL", out modl) && modl >= 1) && (typeDict.TryGetValue("MTBL", out mtbl) && mtbl >= 1) && (typeDict.TryGetValue("OBJD", out objd) && objd <= 0)){
                    thisPackage.Type = "Object Recolor";
                    thisPackage.Mesh = false;
                    thisPackage.Recolor = true;
                    log.MakeLog("This is a " + thisPackage.Type + "!!", true);
                } else if ((typeDict.TryGetValue("_IMG", out img) && img >= 1) && (typeDict.TryGetValue("COBJ", out cobj) && cobj >= 1) && (typeDict.TryGetValue("FTPT", out ftpt) && ftpt >= 1) && (typeDict.TryGetValue("MLOD", out mlod) && mlod >= 1) && (typeDict.TryGetValue("MODL", out modl) && modl >= 1) && (typeDict.TryGetValue("MTBL", out mtbl) && mtbl >= 1) && (typeDict.TryGetValue("OBJD", out objd) && objd >= 1)){
                    thisPackage.Type = "Object";
                    thisPackage.Mesh = true;
                    thisPackage.Recolor = true;
                    log.MakeLog("This is a " + thisPackage.Type + "!!", true);
                } else if ((typeDict.TryGetValue("RLE2", out rle) && rle >= 1) && (typeDict.TryGetValue("CASP", out casp) && casp >= 1) && (typeDict.TryGetValue("GEOM", out geom) && geom >= 1)){
                    thisPackage.Type = "CAS Part";
                    log.MakeLog("This is a " + thisPackage.Type + "!!", true);
                } else {
                    log.MakeLog("Unable to identify package.", true);
                    thisPackage.Type = "UNKNOWN";
                }
            }

            }

            if (thisPackage.AgeGenderFlags.Any()){
                if ((thisPackage.AgeGenderFlags.Female == true) && (thisPackage.AgeGenderFlags.Male == true)){
                    thisPackage.Gender = "Both";
                } else if (thisPackage.AgeGenderFlags.Female == true){
                    thisPackage.Gender = "Female";
                } else if (thisPackage.AgeGenderFlags.Male == true){
                    thisPackage.Gender = "Male";
                }


                string age = string.Empty;
                if (thisPackage.AgeGenderFlags.Adult == true){
                    if (string.IsNullOrEmpty(age)){
                        age += "Adult";
                    } else {
                        age += string.Format(", Adult");
                    }
                } else if (thisPackage.AgeGenderFlags.Baby == true){
                    if (string.IsNullOrEmpty(age)){
                        age += "Baby";
                    } else {
                        age += string.Format(", Baby");
                    }
                } else if (thisPackage.AgeGenderFlags.Child == true){
                    if (string.IsNullOrEmpty(age)){
                        age += "Child";
                    } else {
                        age += string.Format(", Child");
                    }
                } else if (thisPackage.AgeGenderFlags.Elder == true){
                    if (string.IsNullOrEmpty(age)){
                        age += "Elder";
                    } else {
                        age += string.Format(", Elder");
                    }
                } else if (thisPackage.AgeGenderFlags.Infant == true){
                    if (string.IsNullOrEmpty(age)){
                        age += "Infant";
                    } else {
                        age += string.Format(", Infant");
                    }
                } else if (thisPackage.AgeGenderFlags.Teen == true){
                    if (string.IsNullOrEmpty(age)){
                        age += "Teen";
                    } else {
                        age += string.Format(", Teen");
                    }
                } else if (thisPackage.AgeGenderFlags.Toddler == true){
                    if (string.IsNullOrEmpty(age)){
                        age += "Toddler";
                    } else {
                        age += string.Format(", Toddler");
                    }
                } else if (thisPackage.AgeGenderFlags.YoungAdult == true){
                    if (string.IsNullOrEmpty(age)){
                        age += "Young Adult";
                    } else {
                        age += string.Format(", Young Adult");
                    }
                }

                thisPackage.Age = age;           
            }
            
            

            log.MakeLog("Package Summary: " + thisPackage.SimsPackagetoString(), true);
            log.MakeLog(thisPackage.SimsPackagetoString(), false);
            //Containers.Containers.allSimsPackages.Add(thisPackage);
            log.MakeLog(string.Format("Adding {0} to packages database.", thisPackage.PackageName), true);
            try {
                GlobalVariables.DatabaseConnection.InsertWithChildren(thisPackage, true);
            } catch (SQLiteException e){
                log.MakeLog(string.Format("Caught exception adding Sims 2 package to database. \n Exception: {0}", e), true);
            }

            log.MakeLog(string.Format("Added {0} to packages database successfully.", thisPackage.PackageName), true);
            txt = string.Format("SELECT * FROM Processing_Reader where Name='{0}'", Methods.FixApostrophesforSQL(packageinfo.Name));
            var closingquery = GlobalVariables.DatabaseConnection.Query<PackageFile>(txt);            
            GlobalVariables.DatabaseConnection.Delete(closingquery[0]);            
            readFile.Dispose();
            //dbpfFile.Dispose();
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            string elapsedtime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                ts.Hours, ts.Minutes, ts.Seconds,
                                ts.Milliseconds / 10);
            GlobalVariables.currentpackage = packageinfo.Name;
            GlobalVariables.packagesRead++;
            log.MakeLog(string.Format("Reading file {0} took {1}", thisPackage.PackageName, elapsedtime), true);
            log.MakeLog(string.Format("Closing package # {0}/{1}: {2}", packageparsecount, GlobalVariables.PackageCount, packageinfo.Name), true);
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
           
        
                
    }
}