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
using System.Collections.Concurrent;
using SimsCCManager.Packages.Sorting;
using System.Windows.Input;
using SimsCCManager.Packages.Orphans;
using System.Windows.Data;
using System.IO.Packaging;
using System.Drawing;
using Microsoft.VisualBasic.Logging;

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
        public static List<TagsList> GetTagInfo(CASTag16Bit tags, uint count, StringBuilder LogFile){
            string LogMessage = "";
            
            List<TagsList> taglist = new();
            TagsList tagg = new();
            List<TagsList> tagQuery = new();
            for (int i = 0; i < count; i++)
            {   
                if (tags.tagKey[i] != 0){
                    tagQuery = GlobalVariables.S4FunctionTypesConnection.Query<TagsList>(string.Format("SELECT * FROM S4CategoryTags where TypeID='{0}'", tags.tagKey[i]));
                    if (tagQuery.Any()){
                        tagg = tagQuery[0];
                        taglist.Add(tagg);
                        LogMessage = string.Format("Tag {0} ({1}) was matched to {2}.", i, tagg.TypeID, tagg.Description);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    } else {
                        LogMessage = string.Format("Tag {0} matched nothing.", tags.tagKey[i]);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    }  
                }
                if (tags.catKey[i] != 0){
                    tagQuery = GlobalVariables.S4FunctionTypesConnection.Query<TagsList>(string.Format("SELECT * FROM S4CategoryTags where TypeID='{0}'", tags.catKey[i]));
                    if (tagQuery.Any()){
                        tagg = tagQuery[0];
                        taglist.Add(tagg);
                        LogMessage = string.Format("Tag {0} ({1}) was matched to {2}.", i, tagg.TypeID, tagg.Description);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    } else {
                        LogMessage = string.Format("Tag {0} matched nothing.", tags.catKey[i]);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    }  
                }                                            
            }
            tagg = new();
            return taglist;
        }
        
        public static List<TagsList> GetTagInfo(Tag tags, uint count, StringBuilder LogFile){
            string LogMessage = "";
            List<TagsList> taglist = new();
            TagsList tagg = new();
            List<TagsList> tagQuery = new();
            for (int i = 0; i < count; i++)
            {
                tagQuery = GlobalVariables.S4FunctionTypesConnection.Query<TagsList>(string.Format("SELECT * FROM S4CategoryTags where TypeID='{0}'", tags.tagKey[i]));
                if (tagQuery.Any()){
                    tagg = tagQuery[0];
                    taglist.Add(tagg);
                    LogMessage = string.Format("Tag {0} ({1}) was matched to {2}.", i, tags.tagKey[i], tagg.Description);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                } else {
                    LogMessage = string.Format("Tag {0} matched nothing.", tags.tagKey[i]);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                }
            }
            tagg = new TagsList();
            return taglist;
        }        
        
        public static List<TagsList> GetTagInfo(List<uint> tags, uint count, StringBuilder LogFile){
            string LogMessage = "";
            List<TagsList> taglist = new List<TagsList>();
            TagsList tagg = new TagsList();
            List<TagsList> tagQuery = new List<TagsList>();            
            for (int i = 0; i < count; i++)
            {
                tagQuery = GlobalVariables.S4FunctionTypesConnection.Query<TagsList>(string.Format("SELECT * FROM S4CategoryTags where TypeID='{0}'", tags[i]));
                if (tagQuery.Any()){
                    tagg = tagQuery[0];
                    taglist.Add(tagg);
                    LogMessage = string.Format("Tag {0} ({2}) was matched to {1}.", i, taglist[i].Description, tags[i]);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                } else {
                    LogMessage = string.Format("Tag {0} matched nothing.", taglist[i]);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                }
            }
            tagg = new TagsList();
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
        
        public ResourceKeyITG(BinaryReader reader, StringBuilder LogFile){
            string LogMessage = "";
            this.instance = reader.ReadUInt64(); 
            LogMessage = string.Format("GUID Instance: {0}", this.instance);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            this.type = reader.ReadUInt32(); 
            LogMessage = string.Format("GUID Type: {0}", this.type);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            this.group = reader.ReadUInt32();     
            LogMessage = string.Format("GUID Group: {0}", this.group);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));  
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
        
        public ResourceKeyITGFlip(BinaryReader reader, StringBuilder LogFile){
            string LogMessage = "";
            uint left = reader.ReadUInt32();
            uint right = reader.ReadUInt32();
            ulong longleft = left;
            longleft = (longleft << 32);
            this.instance = longleft | right;
            LogMessage = string.Format("GUID Instance: {0}", this.instance);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            this.type = reader.ReadUInt32(); 
            LogMessage = string.Format("GUID Type: {0}", this.type);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            this.group = reader.ReadUInt32();     
            LogMessage = string.Format("GUID Group: {0}", this.group);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));    
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
        public List<PackageInstance> allinstanceIDs = new();
        public List<PackageOBJDKeys> objkeys = new();
        public ulong instanceid;
        public uint typeid;
        public uint groupid;

        public ReadCOBJ(BinaryReader readFile, int packageparsecount, int e, List<TagsList> itemtags, StringBuilder LogFile){
            string LogMessage = "";
            uint version = readFile.ReadUInt32();
            LogMessage = string.Format("P{0}/E{1} - Version: {2} \n-- As hex: {3}", packageparsecount, e, version, version.ToString("X8"));
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            uint commonblockversion = readFile.ReadUInt32();
            LogMessage = string.Format("P{0}/E{1} - Common Block Version: {2} \n-- As hex: {3}", packageparsecount, e, commonblockversion, commonblockversion.ToString("X8"));
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            uint namehash = readFile.ReadUInt32();
            LogMessage = string.Format("P{0}/E{1} - Name Hash: {2} \n-- As hex: {3}", packageparsecount, e, namehash, namehash.ToString("X8"));
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            uint descriptionhash = readFile.ReadUInt32();
            LogMessage = string.Format("P{0}/E{1} - Description Hash: {2} \n-- As hex: {3}", packageparsecount, e, descriptionhash, descriptionhash.ToString("X8"));
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            uint price = readFile.ReadUInt32();
            LogMessage = string.Format("P{0}/E{1} - Price: {2} \n-- As hex: {3}", packageparsecount, e, price, price.ToString("X8"));
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

            ulong thumbhash = readFile.ReadUInt64();
            LogMessage = string.Format("P{0}/E{1} - Thumb Hash: {2} \n-- As hex: {3}", packageparsecount, e, thumbhash, thumbhash.ToString("X8"));
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

            uint devcatflags = readFile.ReadUInt32();
            LogMessage = string.Format("P{0}/E{1} - Dev Cat Flags: {2} \n-- As hex: {3}", packageparsecount, e, devcatflags, devcatflags.ToString("X8"));
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            
            int tgicount = readFile.ReadByte();
            LogMessage = string.Format("P{0}/E{1}, - TGI Count: {2}", packageparsecount, e, tgicount);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            instanceid = 0;
            typeid = 0;
            groupid = 0;

            if (tgicount != 0){
                LogMessage = string.Format("P{0}/E{1}, - TGI Count is not zero. Reading Resources.", packageparsecount, e);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                for (int i = 0; i < tgicount; i++){
                    ResourceKeyITG resourcekey = new ResourceKeyITG(readFile, LogFile);
                    LogMessage = string.Format("P{0}/E{1}, - TGI #{2}: {3}", packageparsecount, e, i, resourcekey.ToString());
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    instanceid = resourcekey.instance;
                    typeid = resourcekey.type;
                    groupid = resourcekey.group;
                    allinstanceIDs.Add(new PackageInstance(){InstanceID = instanceid.ToString("X8")});
                    objkeys.Add(new PackageOBJDKeys(){OBJDKey = resourcekey.ToString()});
                }                
            }
            
            if (commonblockversion >= 10)
            {
                int packId = readFile.ReadInt16();
                LogMessage = string.Format("P{0}/E{1}, - Pack ID: {2}", packageparsecount, e, packId);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                int packFlags = readFile.ReadByte();
                LogMessage = string.Format("P{0}/E{1}, - PackFlags: {2}", packageparsecount, e, packFlags);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                readFile.ReadBytes(9);
            } else {
                int unused2 = readFile.ReadByte();
                if (unused2 > 0)
                {
                    readFile.ReadByte();
                }
            }

            if (commonblockversion >= 11){
                uint count = readFile.ReadUInt32();
                LogMessage = string.Format("P{0}/E{1}, - Tags Count: {2}", packageparsecount, e, count);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                Tag tags = new Tag(readFile, count);

                List<TagsList> gottags = Readers.GetTagInfo(tags, count, LogFile);
                foreach (TagsList tag in gottags){
                    itemtags.Add(tag);                      
                }
                
            } else {
                uint count = readFile.ReadUInt32();
                List<uint> tagsread = new List<uint>(); 
                LogMessage = string.Format("P{0}/E{1}, - Num Tags: {2}", packageparsecount, e, count);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                for (int t = 0; t < count; t++){                    
                    uint tagvalue = readFile.ReadUInt16();
                    tagsread.Add(tagvalue);
                }
                List<TagsList> gottags = Readers.GetTagInfo(tagsread, count, LogFile);
                foreach (TagsList tag in gottags){
                    itemtags.Add(tag);                    
                }
            }
            long location = readFile.BaseStream.Position;
            uint count2 = readFile.ReadUInt32();
            LogMessage = string.Format("P{0}/E{1}, - Selling Point Count: {2}", packageparsecount, e, count2);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            if (count2 > 100){
                log.MakeLog("Selling point count is too high, something went wrong.", true);
            } else {
                Tag sellingtags = new Tag(readFile, count2);

                List<TagsList> gottags = Readers.GetTagInfo(sellingtags, count2, LogFile);
                foreach (TagsList tag in gottags){
                    itemtags.Add(tag); 
                }
            }

            uint unlockByHash = readFile.ReadUInt32();
            LogMessage = string.Format("P{0}/E{1}, - UnlockBy Hash: {2}", packageparsecount, e, unlockByHash);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            
            uint unlockedByHash = readFile.ReadUInt32();
            LogMessage = string.Format("P{0}/E{1}, - UnlockedBy Hash: {2}", packageparsecount, e, unlockedByHash);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

            int swatchColorSortPriority = readFile.ReadUInt16();
            LogMessage = string.Format("P{0}/E{1}, - Swatch Sort Priority: {2}", packageparsecount, e, swatchColorSortPriority);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

            ulong varientThumbImageHash = readFile.ReadUInt64();
            LogMessage = string.Format("P{0}/E{1}, - Varient Thumb Image Hash: {2}", packageparsecount, e, varientThumbImageHash);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
        }            
    }
    
    public struct ReadOBJDIndex {
        /// <summary>
        /// Retrieves OBJD index.
        /// </summary>
        LoggingGlobals log = new LoggingGlobals();
        public int version;
        public long refposition;
        public int count;
        public uint[] entrytype;
        public uint[] position;

        public ReadOBJDIndex(BinaryReader reader, int packageparsecount, int e, StringBuilder LogFile){
            long entrystart = reader.BaseStream.Position;
            string LogMessage = "";
            this.version = reader.ReadUInt16();
            log.MakeLog("Version: " + this.version, true);
            LogMessage = string.Format("P{0}/OBJD{1}, - Version: {2}", packageparsecount, e, this.version);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

            if (this.version > 150){
                log.MakeLog("Version is not legitimate.", true);
                this.refposition = 0; 
                this.count = (int)0;
                this.entrytype = new uint[0];
                this.position = new uint[0];
            } else {
                this.refposition = reader.ReadUInt32();
                this.refposition = refposition + entrystart;
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
        public List<PackageMeshKeys> meshes = new();
        private bool tuningidmissing = false;
        public uint type;
        public uint group;
        public ulong instance; 
        public ReadOBJDEntry(BinaryReader reader, string[] entries, int[] positions, int packageparsecount, int e, StringBuilder LogFile){
            string LogMessage = "";
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
            instance = 0;
            group = 0;
            type = 0;          

            for (int j = 0; j < entries.Length; j++){
                string entryid = entries[j];
                int entrypos = positions[j];
                switch (entryid)
                {
                    case "E7F07786": // name
                        reader.BaseStream.Position = entrypos;
                        this.namelength = reader.ReadByte();
                        LogMessage = string.Format("P{0}/OBJD{1}, - Name Length: {2}", packageparsecount, e, namelength);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        reader.BaseStream.Position = reader.BaseStream.Position + 3;
                        //LogMessage = "Byte 1: " + reader.ReadByte().ToString();
                        //if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        //if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        //LogMessage = "Byte 2: " + reader.ReadByte().ToString();
                        //if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        //if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        //LogMessage = "Byte 3: " + reader.ReadByte().ToString();
                        //if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        //if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        this.namebit = reader.ReadBytes(namelength);
                        this.name = Encoding.UTF8.GetString(namebit);
                        LogMessage = string.Format("P{0}/OBJD{1}, - Name: {2}", packageparsecount, e, name);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        break;
                    case "790FA4BC": //tuning
                        reader.BaseStream.Position = entrypos;
                        this.tuningnamelength = reader.ReadByte();
                        LogMessage = string.Format("P{0}/OBJD{1}, - Tuning Name Length: {2}", packageparsecount, e, tuningnamelength);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        reader.BaseStream.Position = reader.BaseStream.Position + 3;
                        //log.MakeLog("Reading three empty bytes.", true);
                        //LogMessage = "Byte 1: " + reader.ReadByte().ToString();
                        //if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        //if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        //LogMessage = "Byte 2: " + reader.ReadByte().ToString();
                        //if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        //if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        //LogMessage = "Byte 3: " + reader.ReadByte().ToString();
                        //if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        //if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        this.tuningbit = reader.ReadBytes(tuningnamelength);
                        LogMessage = string.Format("P{0}/OBJD{1}, - Tuning Name Length: {2}", packageparsecount, e, tuningbit);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        this.tuningname = Encoding.UTF8.GetString(tuningbit);
                        LogMessage = string.Format("P{0}/OBJD{1}, - Tuning Name: {2}", packageparsecount, e, tuningname);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        break;
                    case "B994039B": //TuningID
                        reader.BaseStream.Position = entrypos;
                        this.tuningid = reader.ReadUInt64(); 
                        break;
                    case "CADED888": //Icon
                        reader.BaseStream.Position = entrypos;
                        preceeding = reader.ReadUInt32();
                        LogMessage = string.Format("P{0}/OBJD{1}, - Reading Preceeding UInt32: {2}", packageparsecount, e, preceeding);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        preceedingDiv = preceeding / 4;            
                        LogMessage = string.Format("P{0}/OBJD{1}, - Number of Icon GUIDs: {2}", packageparsecount, e, preceedingDiv);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        this.icon = new string[preceedingDiv];
                        for (int p = 0; p < preceedingDiv; p++){
                            ResourceKeyITGFlip ricon = new ResourceKeyITGFlip(reader, LogFile);
                            LogMessage = string.Format("P{0}/OBJD{1}, - Icon GUID: {2}", packageparsecount, e, ricon.ToString());
                            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                            this.icon[p] = ricon.ToString();
                            this.meshes.Add(new PackageMeshKeys(){MeshKey = ricon.ToString()});
                            this.type = ricon.type;
                            this.group = ricon.group;
                            this.instance = ricon.instance;                            
                        }
                        break;
                    case "E206AE4F": //Rig
                        reader.BaseStream.Position = entrypos;
                        preceeding = reader.ReadUInt32();
                        LogMessage = string.Format("P{0}/OBJD{1}, - Reading preceeding UInt32: {2}", packageparsecount, e, preceeding);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));                        
                        preceedingDiv = preceeding / 4;
                        LogMessage = string.Format("P{0}/OBJD{1}, - Number of Rig GUIDs: {2}", packageparsecount, e, preceedingDiv);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        this.rig = new string[preceedingDiv];
                        for (int p = 0; p < preceedingDiv; p++){
                            ResourceKeyITGFlip rkrig = new ResourceKeyITGFlip(reader, LogFile);
                            LogMessage = string.Format("P{0}/OBJD{1}, - Rig GUID: {2}", packageparsecount, e, rkrig.ToString());
                            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                            this.rig[p] = rkrig.ToString();
                            this.meshes.Add(new PackageMeshKeys(){MeshKey = rkrig.ToString()});
                            this.type = rkrig.type;
                            this.group = rkrig.group;
                            this.instance = rkrig.instance;
                        }   
                        break;
                    case "8A85AFF3": //Slot
                        reader.BaseStream.Position = entrypos;
                        preceeding = reader.ReadUInt32();
                        LogMessage = string.Format("P{0}/OBJD{1}, - Reading preceeding UInt32: {2}", packageparsecount, e, preceeding);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));                        
                        preceedingDiv = preceeding / 4;
                        LogMessage = string.Format("P{0}/OBJD{1}, - Number of Slot GUIDs: {2}", packageparsecount, e, preceedingDiv);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        this.slot = new string[preceedingDiv];
                        for (int p = 0; p < preceedingDiv; p++){
                            ResourceKeyITGFlip rkslot = new ResourceKeyITGFlip(reader, LogFile);
                            LogMessage = string.Format("P{0}/OBJD{1}, - Slot GUID: {2}", packageparsecount, e, rkslot.ToString());
                            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                            this.slot[p] = rkslot.ToString();     
                            this.meshes.Add(new PackageMeshKeys(){MeshKey = rkslot.ToString()});                       
                            this.type = rkslot.type;
                            this.group = rkslot.group;
                            this.instance = rkslot.instance;
                        }            
                        break;
                    case "8D20ACC6": //Model
                        reader.BaseStream.Position = entrypos;
                        preceeding = reader.ReadUInt32();
                        LogMessage = string.Format("P{0}/OBJD{1}, - Reading preceeding UInt32: {2}", packageparsecount, e, preceeding);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));                        
                        preceedingDiv = preceeding / 4;
                        LogMessage = string.Format("P{0}/OBJD{1}, - Number of Model GUIDs: {2}", packageparsecount, e, preceedingDiv);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        this.model = new string[preceedingDiv];
                        for (int p = 0; p < preceedingDiv; p++){
                            ResourceKeyITGFlip rkmodel = new ResourceKeyITGFlip(reader, LogFile);                            
                            LogMessage = string.Format("P{0}/OBJD{1}, - Model GUID: {2}", packageparsecount, e, rkmodel.ToString());
                            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                            this.model[p] = rkmodel.ToString();
                            this.meshes.Add(new PackageMeshKeys(){MeshKey = rkmodel.ToString()});
                            this.type = rkmodel.type;
                            this.group = rkmodel.group;
                            this.instance = rkmodel.instance;
                        }
                        break;
                    case "6C737AD8": //Footprint
                        reader.BaseStream.Position = entrypos;
                        preceeding = reader.ReadUInt32();
                        LogMessage = string.Format("P{0}/OBJD{1}, - Reading preceeding UInt32: {2}", packageparsecount, e, preceeding);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));                        
                        preceedingDiv = preceeding / 4;
                        LogMessage = string.Format("P{0}/OBJD{1}, - Number of Footprint GUIDs: {2}", packageparsecount, e, preceedingDiv);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        this.footprint = new string[preceedingDiv];
                        for (int p = 0; p < preceedingDiv; p++){
                            ResourceKeyITGFlip rkft = new ResourceKeyITGFlip(reader, LogFile);
                            LogMessage = string.Format("P{0}/OBJD{1}, - Footprint GUID: {2}", packageparsecount, e, rkft.ToString());
                            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                            this.footprint[p] = rkft.ToString();
                            this.meshes.Add(new PackageMeshKeys(){MeshKey = rkft.ToString()});
                            this.type = rkft.type;
                            this.group = rkft.group;
                            this.instance = rkft.instance;                
                        }
                        break;
                    case "E6E421FB": //Components
                        reader.BaseStream.Position = entrypos;
                        this.componentcount = reader.ReadUInt32();                        
                        LogMessage = string.Format("P{0}/OBJD{1}, - Component Count: {2}", packageparsecount, e, componentcount);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        this.components = new uint[this.componentcount];
                        for (int i = 0; i < this.componentcount; i++){
                            components[i] = reader.ReadUInt32();
                        }
                        break;
                    case "ECD5A95F": //MaterialVariant
                        reader.BaseStream.Position = entrypos;
                        this.materialvariantlength = reader.ReadByte();                        
                        LogMessage = string.Format("P{0}/OBJD{1}, - Material Variant Length: {2}", packageparsecount, e, materialvariantlength);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        reader.BaseStream.Position = reader.BaseStream.Position + 3;
                        //log.MakeLog("Reading three empty bytes.", true);
                        //LogMessage = "Byte 1: " + reader.ReadByte().ToString();
                        //if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        //if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        //LogMessage = "Byte 2: " + reader.ReadByte().ToString();
                        //if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        //if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        //LogMessage = "Byte 3: " + reader.ReadByte().ToString();
                        //if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        //if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        this.materialvariantbyte = reader.ReadBytes(materialvariantlength);
                        this.materialvariant = Encoding.UTF8.GetString(materialvariantbyte);
                        LogMessage = string.Format("P{0}/OBJD{1}, - Material Variant: {2}", packageparsecount, e, materialvariant);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        break;
                    case "AC8E1BC0": //Unknown1
                        reader.BaseStream.Position = entrypos;

                        break;
                    case "E4F4FAA4": //SimoleonPrice
                        reader.BaseStream.Position = entrypos;
                        this.price = reader.ReadUInt32();                        
                        LogMessage = string.Format("P{0}/OBJD{1}, - Price: {2}", packageparsecount, e, price);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
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
        public long position;
        public uint fileSize;
        public uint memSize;
        public string compressionType;
        
        

    }	
    class S4PackageSearch
    {
        /// <summary>
        /// Sims 4 package reading. Gets all the information from inside S4 Package files and returns it for use.
        /// </summary>
        
        // Class References
        LoggingGlobals log = new LoggingGlobals();
        System.Text.Encoding encoding = System.Text.Encoding.BigEndianUnicode;
        FilesSort filesort = new FilesSort();
        FindOrphans findOrphans = new FindOrphans();

        //lists

        public static bool hasrunbefore;  
        private int ovcapacity = 1781483;


        //Vars
        uint chunkOffset = 0;
        public string[] parameters = {"DefaultForBodyType","DefaultThumbnailPart","AllowForCASRandom","ShowInUI","ShowInSimInfoPanel","ShowInCasDemo","AllowForLiveRandom","DisableForOppositeGender","DisableForOppositeFrame","DefaultForBodyTypeMale","DefaultForBodyTypeFemale","Unk","Unk","Unk","Unk","Unk"};
        

        public void SearchS4Packages(FileStream dbpfFile, FileInfo packageinfo, int packageparsecount, StringBuilder LogFile) {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string LogMessage = "";
            //StringBuilder LogFile = new();
            LogMessage = string.Format("File {0} arrived for processing as Sims 4 file.", packageinfo.Name);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));            
            /*string txt = string.Format("SELECT * FROM Processing_Reader where Name='{0}'", Methods.FixApostrophesforSQL(packageinfo.Name));
            List<PackageFile> queries = GlobalVariables.DatabaseConnection.Query<PackageFile>(txt);
            PackageFile query = queries[0];
            GlobalVariables.DatabaseConnection.Delete(query);
            queries = new List<PackageFile>();
            query = new PackageFile();
            PackageFile pk = new PackageFile { ID = query.ID, Name = packageinfo.Name, Location = packageinfo.FullName, Game = 4, Broken = false, Status = "Processing"};
            GlobalVariables.DatabaseConnection.Insert(pk);
            pk = new PackageFile();*/
                        
            //locations

            long entrycountloc = 36;
            long indexRecordPositionloc = 64;

            SimsPackage thisPackage = new SimsPackage();          

            //Lists 
            
            List<TagsList> itemtags = new List<TagsList>();
            List<TagsList> distinctItemTags = new List<TagsList>();
            List<PackageFlag> allFlags = new();      
            List<PackageFlag> distinctFlags = new(); 
            List<PackageInstance> allInstanceIDs = new();      
            List<PackageInstance> distinctInstanceIDs = new();           
            string[] objdentries;
            int[] objdpositions;   
            List<EntryHolder> entries = new List<EntryHolder>();
            indexEntry MergedManifest = new();
            List<PackageEntries> fileHas = new List<PackageEntries>();
            ArrayList linkData = new ArrayList();
            List<indexEntry> indexData = new List<indexEntry>();
            SimsPackage DataDelivery = new SimsPackage();

            itemtags = new List<TagsList>();
            distinctItemTags = new List<TagsList>();

            //create readers  
            //MemoryStream dbpfFile = Methods.ReadBytesToFile(packageinfo.FullName, (int)packageinfo.Length);
            BinaryReader readFile = new BinaryReader(dbpfFile);
            
            //log opening file
            LogMessage = string.Format("Reading package # {0}/{1}: {2}", packageparsecount, GlobalVariables.PackageCount, packageinfo.Name);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            
            //string packageNameUpdated = Methods.FixApostrophesforSQL(packageinfo.Name);            
            thisPackage.PackageName = packageinfo.Name;
            thisPackage.Location = packageinfo.FullName;            
            thisPackage.Game = 4;
            thisPackage.GameString = "The Sims 4";
            thisPackage.FileSize = (int)packageinfo.Length;
            LogMessage = string.Format("Package #{0} registered as {1} and meant for Sims 4", packageparsecount, packageinfo.FullName);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

            readFile.BaseStream.Position = 0;

            readFile.BaseStream.Position = entrycountloc;

            uint entrycount = readFile.ReadUInt32();
            LogMessage = string.Format("Entry Count: {0}", entrycount.ToString());
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            
            //record position low
            uint indexRecordPositionLow = readFile.ReadUInt32();
            LogMessage = string.Format("IndexRecordPositionLow: {0}", indexRecordPositionLow.ToString());
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            
            //index record size
            uint indexRecordSize = readFile.ReadUInt32();
            LogMessage = string.Format("IndexRecordSize: {0}", indexRecordSize.ToString());
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            
            readFile.BaseStream.Position = indexRecordPositionloc;

            ulong indexRecordPosition = readFile.ReadUInt64();
            LogMessage = string.Format("Index Record Position: {0}", indexRecordPosition.ToString());
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            
                        
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
            LogMessage = string.Format("P{0} - Streamsize is {1}", packageparsecount, streamsize);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            if ((int)movedto + indexbytes != streamsize){
                int entriesfound = 0;
                LogMessage = string.Format("P{0} - Streamsize does not match!", packageparsecount);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                readFile.BaseStream.Position = movedto - 400;
                List<long> entrylocs = new List<long>();
                uint item;
                List<typeList> types = new List<typeList>();
                
                while(readFile.BaseStream.Length > readFile.BaseStream.Position){                    
                    item = readFile.ReadUInt32();
                    GlobalVariables.S4FunctionTypesConnection.Query<typeList>(string.Format("SELECT * FROM S4Types WHERE TypeID='{0}'", item.ToString("X8")));
                    //LogMessage = string.Format("Read byte at {0}: {1}", readFile.BaseStream.Position, item.ToString("X8"));
                    //if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    //if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    if (types.Any()){
                        indexEntry holderEntry = new indexEntry();
                        holderEntry.typeID = item.ToString("X8");
                        LogMessage = string.Format("P{0}/E{1} - Index Entry TypeID: {2}", packageparsecount, entriesfound, holderEntry.typeID);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        LogMessage = string.Format("Found entry{2} - {0} at location {1}!", item, readFile.BaseStream.Position, entriesfound);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        entrylocs.Add(readFile.BaseStream.Position);
                        if (entriesfound == 0){
                            indexpos = readFile.BaseStream.Position - 4;
                        }
                        entriesfound++;
                        if (entriesfound == entrycount){
                            LogMessage = string.Format("Found {0} entries. Breaking.", entrycount);
                            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                            break;
                        }
                    }
                }

                types = new List<typeList>();

                readFile.BaseStream.Position = indexpos;
                entrycount = 0;
                foreach (int loc in entrylocs){
                    indexEntry holderEntry = new indexEntry(); 
                    readFile.BaseStream.Position = loc - 4;
                    if (indextype == 2){
                        holderEntry.typeID = readFile.ReadUInt32().ToString("X8");
                        LogMessage = string.Format("P{0}/E{1} - Index Entry TypeID: {2}", packageparsecount, entrycount, holderEntry.typeID);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        
                        List<typeList> type = GlobalVariables.S4FunctionTypesConnection.Query<typeList>(string.Format("SELECT * FROM S4Types where TypeID='{0}'", holderEntry.typeID));
                    
                        if(type.Any()){                        
                            fileHas.Add(new PackageEntries {TypeID = type[0].typeID, Location = (int)entrycount});
                            LogMessage = string.Format("File {0} has {1} at location {2}.", thisPackage.PackageName, type[0].desc, (int)entrycount);
                            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        } else {
                            fileHas.Add(new PackageEntries() { TypeID = holderEntry.typeID, Location = (int)entrycount});
                        }
                        type = new List<typeList>();                                    

                        string instanceid1 = (readFile.ReadUInt32() << 32).ToString("X8");
                        string instanceid2 = (readFile.ReadUInt32() << 32).ToString("X8");
                        holderEntry.instanceID = string.Format("{0}{1}", instanceid1,instanceid2);
                        if (holderEntry.instanceID != "0000000000000000"){
                            allInstanceIDs.Add(new PackageInstance(){InstanceID = holderEntry.instanceID});
                        }
                        
                        LogMessage = string.Format("P{0}/E{1} - InstanceID: {2}", packageparsecount, entrycount, holderEntry.instanceID);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        
                        holderEntry.position = readFile.ReadUInt32();
                        LogMessage = string.Format("P{0}/E{1} - Index Entry Position: {2}", packageparsecount, entrycount, holderEntry.position.ToString("X8"));
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));                        

                        holderEntry.fileSize = readFile.ReadUInt32();
                        LogMessage = string.Format("P{0}/E{1} - File Size: {2}", packageparsecount, entrycount, holderEntry.fileSize.ToString("X8"));
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

                        holderEntry.memSize = readFile.ReadUInt32();
                        LogMessage = string.Format("P{0}/E{1} - Mem Size: {2}", packageparsecount, entrycount, holderEntry.memSize.ToString("X8"));
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

                        indexData.Add(holderEntry);
                        MergedManifest = holderEntry;

                        holderEntry = null;

                        entrycount++;
                    }
                }
            } else {
                LogMessage = "Streamsize matches.";
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                long movedhere = readFile.BaseStream.Position;
                uint testpos = readFile.ReadUInt32();
                if (testpos != 0){
                    LogMessage = string.Format("P{0} - Read first entry TypeID and it read as {1}, returning to read entries.", packageparsecount, testpos.ToString("X8"));
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    readFile.BaseStream.Position = movedto;
                } else if (testpos == 80000000) {
                    long moveback = movedhere - 4;
                    readFile.BaseStream.Position = moveback;
                } else {
                    LogMessage = string.Format("P{0} - Read first entry TypeID and it read as {1}, moving forward.", packageparsecount, testpos.ToString("X8"));
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                }                
                for (int i = 0; i < entrycount; i++){                    
                    indexEntry holderEntry = new indexEntry();                
                    holderEntry.typeID = readFile.ReadUInt32().ToString("X8");
                    LogMessage = string.Format("P{0}/E{1} - Index Entry TypeID: {2}", packageparsecount, i, holderEntry.typeID);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

                    if (holderEntry.typeID == "7FB6AD8A"){
                        thisPackage.Type = "Merged Package";
                        thisPackage.Merged = true;

                        List<typeList> type = GlobalVariables.S4FunctionTypesConnection.Query<typeList>(string.Format("SELECT * FROM S4Types where TypeID='{0}'", holderEntry.typeID));
                    
                        if(type.Any()){                        
                            fileHas.Add(new PackageEntries {TypeID = type[0].typeID, Name = type[0].desc, Location = i});
                            LogMessage = string.Format("File {0} has {1} at location {2}.", thisPackage.PackageName, type[0].desc, i);
                            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        } else {
                            fileHas.Add(new PackageEntries() { TypeID = holderEntry.typeID, Location = i});
                        }
                        type = new List<typeList>();

                        holderEntry.groupID = readFile.ReadUInt32().ToString("X8");
                        LogMessage = string.Format("P{0}/E{1} - Index Entry GroupID: {2}", packageparsecount, i, holderEntry.groupID);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        
                        string instanceid1 = (readFile.ReadUInt32() << 32).ToString("X8");
                        string instanceid2 = (readFile.ReadUInt32() << 32).ToString("X8");
                        holderEntry.instanceID = string.Format("{0}{1}", instanceid1,instanceid2);
                        allInstanceIDs.Add(new PackageInstance(){InstanceID = holderEntry.instanceID});
                        LogMessage = string.Format("P{0}/E{1} - InstanceID: {2}", packageparsecount, i, holderEntry.instanceID);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

                        uint testin = readFile.ReadUInt32();
                        holderEntry.position = (long)testin;
                        LogMessage = string.Format("P{0}/E{1} - Position: {2}", packageparsecount, i, holderEntry.position);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

                        holderEntry.fileSize = readFile.ReadUInt32();
                        LogMessage = string.Format("P{0}/E{1} - File Size: {2}", packageparsecount, i, holderEntry.fileSize);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

                        holderEntry.memSize = readFile.ReadUInt32();
                        LogMessage = string.Format("P{0}/E{1} - Mem Size: {2}", packageparsecount, i, holderEntry.memSize);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

                        holderEntry.compressionType = readFile.ReadUInt16().ToString("X4");
                        LogMessage = string.Format("P{0}/E{1} - Compression Type: {2}", packageparsecount, i, holderEntry.compressionType);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

                        readFile.BaseStream.Position = readFile.BaseStream.Position + 2;

                        indexData.Add(holderEntry);

                        holderEntry = null;                        

                        /*thisPackage = MakeNoNulls(thisPackage);
                        GlobalVariables.AddPackages.Enqueue(thisPackage); 
                        log.MakeLog(string.Format("Package {0} is a merged package, and cannot be processed in this manner right now. Package will either need unmerging or to be sorted manually.", thisPackage.Location), false);

                        readFile.Dispose();
                        sw.Stop();
                        TimeSpan tss = sw.Elapsed;
                        string elapsedtimee = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                            tss.Hours, tss.Minutes, tss.Seconds,
                                            tss.Milliseconds / 10);
                        GlobalVariables.packagesRead++;
                        LogMessage = string.Format("Reading file {0} took {1}", thisPackage.PackageName, elapsedtimee);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        LogMessage = string.Format("Closing package # {0}/{1}: {2}", packageparsecount, GlobalVariables.PackageCount, packageinfo.Name);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        return;*/

                    } else {
                        List<typeList> type = GlobalVariables.S4FunctionTypesConnection.Query<typeList>(string.Format("SELECT * FROM S4Types where TypeID='{0}'", holderEntry.typeID));
                    
                        if(type.Any()){                        
                            fileHas.Add(new PackageEntries {TypeID = type[0].typeID, Name = type[0].desc, Location = i});
                            LogMessage = string.Format("File {0} has {1} at location {2}.", thisPackage.PackageName, type[0].desc, i);
                            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        } else {
                            fileHas.Add(new PackageEntries() { TypeID = holderEntry.typeID, Location = i});
                        }
                        type = new List<typeList>();

                        holderEntry.groupID = readFile.ReadUInt32().ToString("X8");
                        LogMessage = string.Format("P{0}/E{1} - Index Entry GroupID: {2}", packageparsecount, i, holderEntry.groupID);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        
                        string instanceid1 = (readFile.ReadUInt32() << 32).ToString("X8");
                        string instanceid2 = (readFile.ReadUInt32() << 32).ToString("X8");
                        holderEntry.instanceID = string.Format("{0}{1}", instanceid1,instanceid2);
                        allInstanceIDs.Add(new PackageInstance(){InstanceID = holderEntry.instanceID});
                        LogMessage = string.Format("P{0}/E{1} - InstanceID: {2}", packageparsecount, i, holderEntry.instanceID);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

                        uint testin = readFile.ReadUInt32();
                        holderEntry.position = (long)testin;
                        LogMessage = string.Format("P{0}/E{1} - Position: {2}", packageparsecount, i, holderEntry.position);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

                        holderEntry.fileSize = readFile.ReadUInt32();
                        LogMessage = string.Format("P{0}/E{1} - File Size: {2}", packageparsecount, i, holderEntry.fileSize);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

                        holderEntry.memSize = readFile.ReadUInt32();
                        LogMessage = string.Format("P{0}/E{1} - Mem Size: {2}", packageparsecount, i, holderEntry.memSize);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

                        holderEntry.compressionType = readFile.ReadUInt16().ToString("X4");
                        LogMessage = string.Format("P{0}/E{1} - Compression Type: {2}", packageparsecount, i, holderEntry.compressionType);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

                        readFile.BaseStream.Position = readFile.BaseStream.Position + 2;

                        indexData.Add(holderEntry);

                        holderEntry = null;
                    }

                    
                }
            }

            /*if (thisPackage.Merged == true){
                var entryspots = (from has in fileHas
                        where has.Name =="S4SM"
                        select has.Location).ToList();
                
                int loc = entryspots[0];

                long entrylength = indexData[loc + 1].position - indexData[loc].position;

                readFile.BaseStream.Position = MergedManifest.position;
                //XmlTextReader xmlDoc = new XmlTextReader(new StringReader(Encoding.UTF8.GetString(Methods.ReadEntryBytes(readFile, (int)entrylength))));
                //BinaryReader decompbr = new BinaryReader(decomps);
                for (int i = 0; i < readFile.BaseStream.Length; i++){
                    log.MakeLog(readFile.ReadByte().ToString(), true);
                }

                //Stream decomps = S4Decryption.Decompress(Methods.ReadEntryBytes(readFile, (int)entrylength));

                //BinaryReader decompbr = new BinaryReader(decomps);
            }
            
            currently unable to read merged packages or unmerge them. workign on it though.
            
            */






            if (fileHas.Exists(x => x.Name == "CASP")){

                var entryspots = (from has in fileHas
                        where has.Name =="CASP"
                        select has.Location).ToList();

                thisPackage.Recolor = true;

                int caspc = 0;
                foreach (int e in entryspots){
                    LogMessage = string.Format("P{0} - Opening CASP #{1}", packageparsecount, e);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    if (indexData[e].compressionType == "5A42"){
                        LogMessage = string.Format("P{0}/CASP{1} - Compression type is {2}.", packageparsecount, e, indexData[e].compressionType);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        readFile.BaseStream.Position = indexData[e].position;
                        long entryEnd = indexData[e].position + indexData[e].memSize;

                        LogMessage = string.Format("P{0}/CASP{1} - Position: {2}", packageparsecount, e, indexData[e].position);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        LogMessage = string.Format("P{0}/CASP{1} - Filesize: {2}", packageparsecount, e, indexData[e].fileSize);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        LogMessage = string.Format("P{0}/CASP{1} - Memsize: {2}", packageparsecount, e, indexData[e].memSize);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        LogMessage = string.Format("P{0}/CASP{1} - Ends at: {2}", packageparsecount, e, entryEnd);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

                        Stream decomps = S4Decryption.Decompress(Methods.ReadEntryBytes(readFile, (int)indexData[e].memSize));
                        
                        BinaryReader decompbr = new BinaryReader(decomps);

                        ProcessCASP pcasp = new(thisPackage, decompbr, decomps, LogFile, LogMessage, packageparsecount, e, itemtags, log, allFlags, parameters);

                        LogFile = pcasp.lf;
                        thisPackage = pcasp.thisPackage;
                        allFlags = pcasp.allFlags;
                        itemtags = pcasp.itemtags;
                        
                        decompbr.Dispose();
                        decomps.Dispose();

                    } else if (indexData[e].compressionType == "0000"){
                        LogMessage = string.Format("P{0}/CASP{1} - Compression type is {2}.", packageparsecount, e, indexData[e].compressionType);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        readFile.BaseStream.Position = indexData[e].position;
                        long entryEnd = indexData[e].position + indexData[e].memSize;

                        LogMessage = string.Format("P{0}/CASP{1} - Position: {2}", packageparsecount, e, indexData[e].position);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        LogMessage = string.Format("P{0}/CASP{1} - Filesize: {2}", packageparsecount, e, indexData[e].fileSize);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        LogMessage = string.Format("P{0}/CASP{1} - Memsize: {2}", packageparsecount, e, indexData[e].memSize);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        LogMessage = string.Format("P{0}/CASP{1} - Ends at: {2}", packageparsecount, e, entryEnd);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

                        ProcessCASP pcasp = new(thisPackage, readFile, dbpfFile, LogFile, LogMessage, packageparsecount, e, itemtags, log, allFlags, parameters);

                        LogFile = pcasp.lf;
                        thisPackage = pcasp.thisPackage;
                        allFlags = pcasp.allFlags;
                        itemtags = pcasp.itemtags;

                    }

                    caspc++;
                    
                }                     

            }


            if (fileHas.Exists(x => x.Name == "COBJ")){

                var entryspots = (from has in fileHas
                        where has.Name =="COBJ"
                        select has.Location).ToList();

                int cobjc = 0;
                foreach (int e in entryspots){
                    
                    LogMessage = string.Format("P{0} - Opening COBJ #{1}", packageparsecount, cobjc);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    if (indexData[e].compressionType == "5A42"){                                
                        readFile.BaseStream.Position = indexData[e].position;                  
                        int entryEnd = (int)readFile.BaseStream.Position + (int)indexData[e].memSize;

                        LogMessage = string.Format("P{0}/COBJ{1} - Position: {2}", packageparsecount, cobjc, indexData[e].position);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        LogMessage = string.Format("P{0}/COBJ{1} - File Size: {2}", packageparsecount, cobjc, indexData[e].fileSize);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        LogMessage = string.Format("P{0}/COBJ{1} - Memory Size: {2}", packageparsecount, cobjc, indexData[e].memSize);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        LogMessage = string.Format("P{0}/COBJ{1} - Entry Ends At: {2}", packageparsecount, cobjc, entryEnd);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        Stream decomps = S4Decryption.Decompress(Methods.ReadEntryBytes(readFile, (int)indexData[e].memSize));
                        
                        BinaryReader decompbr = new BinaryReader(decomps);  

                        ProcessCOBJ pcobj = new(thisPackage, decompbr, LogFile, LogMessage, packageparsecount, e, itemtags, allInstanceIDs);

                        thisPackage = pcobj.package;
                        allInstanceIDs = pcobj.iids;
                        itemtags = pcobj.tagl;
                        LogFile = pcobj.lf;

                        decompbr.Dispose();
                        decomps.Dispose();
                    } else if (indexData[e].compressionType == "0000"){
                        readFile.BaseStream.Position = indexData[e].position;                           
                        int entryEnd = (int)readFile.BaseStream.Position + (int)indexData[e].memSize;
                        
                        LogMessage = string.Format("P{0}/COBJ{1} - Position: {2}", packageparsecount, cobjc, indexData[e].position);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        LogMessage = string.Format("P{0}/COBJ{1} - File Size: {2}", packageparsecount, cobjc, indexData[e].fileSize);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        LogMessage = string.Format("P{0}/COBJ{1} - Memory Size: {2}", packageparsecount, cobjc, indexData[e].memSize);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        LogMessage = string.Format("P{0}/COBJ{1} - Entry Ends At: {2}", packageparsecount, cobjc, entryEnd);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

                        ProcessCOBJ pcobj = new(thisPackage, readFile, LogFile, LogMessage, packageparsecount, e, itemtags, allInstanceIDs); 

                        thisPackage = pcobj.package;
                        allInstanceIDs = pcobj.iids;
                        itemtags = pcobj.tagl;
                        LogFile = pcobj.lf;
                                      
                    }
                    cobjc++;
                }
            }

            if (fileHas.Exists(x => x.Name == "OBJD")){
                var entryspots = (from has in fileHas
                        where has.Name =="OBJD"
                        select has.Location).ToList();

                int objdc = 0;
                foreach (int e in entryspots){
                    LogMessage = string.Format("P{0} - Opening OBJD #{1}", packageparsecount, objdc);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    if (indexData[e].compressionType == "5A42"){
                        readFile.BaseStream.Position = indexData[e].position;                       
                        int entryEnd = (int)readFile.BaseStream.Position + (int)indexData[e].memSize;
                        LogMessage = string.Format("P{0}/OBJD{1} - Position: {2}", packageparsecount, objdc, indexData[e].position);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        LogMessage = string.Format("P{0}/OBJD{1} - File Size: {2}", packageparsecount, objdc, indexData[e].fileSize);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        LogMessage = string.Format("P{0}/OBJD{1} - Memory Size: {2}", packageparsecount, objdc, indexData[e].memSize);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        LogMessage = string.Format("P{0}/OBJD{1} - Entry Ends At: {2}", packageparsecount, objdc, entryEnd);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));


                        Stream decomps = S4Decryption.Decompress(Methods.ReadEntryBytes(readFile, (int)indexData[e].memSize));
                        
                        BinaryReader decompbr = new BinaryReader(decomps);

                        ProcessOBJD pobjd = new(thisPackage, decompbr, LogFile, LogMessage, packageparsecount, e, itemtags, allInstanceIDs, objdc, log);

                        thisPackage = pobjd.package;
                        allInstanceIDs = pobjd.iids;
                        itemtags = pobjd.tagl;
                        LogFile = pobjd.lf;
                                                                      
                        decompbr.Dispose();   
                        decomps.Dispose();
                    } else if (indexData[e].compressionType == "0000"){
                        readFile.BaseStream.Position = indexData[e].position;                       
                        int entryEnd = (int)readFile.BaseStream.Position + (int)indexData[e].memSize;
                        LogMessage = string.Format("P{0}/OBJD{1} - Position: {2}", packageparsecount, objdc, indexData[e].position);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        LogMessage = string.Format("P{0}/OBJD{1} - File Size: {2}", packageparsecount, objdc, indexData[e].fileSize);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        LogMessage = string.Format("P{0}/OBJD{1} - Memory Size: {2}", packageparsecount, objdc, indexData[e].memSize);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        LogMessage = string.Format("P{0}/OBJD{1} - Entry Ends At: {2}", packageparsecount, objdc, entryEnd);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));



                        ProcessOBJD pobjd = new(thisPackage, readFile, LogFile, LogMessage, packageparsecount, e, itemtags, allInstanceIDs, objdc, log);

                        thisPackage = pobjd.package;
                        allInstanceIDs = pobjd.iids;
                        itemtags = pobjd.tagl; 
                        LogFile = pobjd.lf;                       
                    }                
                    objdc++;
                }
                
            }

            if (fileHas.Exists(x => x.Name == "GEOM")){
                var entryspots = (from has in fileHas
                        where has.Name =="GEOM"
                        select has.Location).ToList();                
                
                thisPackage.Mesh = true;

                foreach (int e in entryspots){
                    string key = string.Format("{0}-{1}-{2}", indexData[e].typeID, indexData[e].groupID, indexData[e].instanceID);
                    if (key != "00000000-00000000-0000000000000000"){
                        var match = thisPackage.MeshKeys.Where(c => c.MeshKey == key);
                        if (!match.Any()){
                            PackageMeshKeys pcpk = new() {MeshKey = key};
                            thisPackage.MeshKeys.Add(pcpk);
                        } 
                    }                                       
                }
            }

            if (fileHas.Exists(x => x.Name == "THUM")){
                var entryspots = (from has in fileHas
                        where has.Name =="THUM"
                        select has.Location).ToList();                
                int c = 0;
                foreach (int e in entryspots){
                    c++;
                    LogMessage = string.Format("P{0}/THUM{1} - Reading THUM {1}, identified as: {2}.", packageparsecount, e, indexData[e].typeID);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

                    readFile.BaseStream.Position = indexData[e].position; 
                    if (indexData[e].compressionType == "5A42"){  
                        int entryEnd = (int)readFile.BaseStream.Position + (int)indexData[e].memSize;

                            LogMessage = string.Format("P{0}/THUM{1} - Position: {2}", packageparsecount, c, indexData[e].position);
                            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                            LogMessage = string.Format("P{0}/THUM{1} - File Size: {2}", packageparsecount, c, indexData[e].fileSize);
                            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                            LogMessage = string.Format("P{0}/THUM{1} - Memory Size: {2}", packageparsecount, c, indexData[e].memSize);
                            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                            LogMessage = string.Format("P{0}/THUM{1} - Entry Ends At: {2}", packageparsecount, c, entryEnd);
                            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                            MemoryStream decomps = S4Decryption.DecompressMS(Methods.ReadEntryBytes(readFile, (int)indexData[e].memSize));
                            byte[] imagebyte = decomps.ToArray();
                            string byteasstring = Convert.ToBase64String(imagebyte);                            
                            PackageThumbnail thumb = new PackageThumbnail() {Thumbnail = byteasstring, Type = indexData[e].typeID, Source = "Package"};
                            thisPackage.ThumbnailImage.Add(thumb);
                    } else if (indexData[e].compressionType == "0000"){
                        byte[] imagebyte = Methods.ReadEntryBytes(readFile, (int)indexData[e].memSize);
                        string byteasstring = Convert.ToBase64String(imagebyte);                            
                        PackageThumbnail thumb = new PackageThumbnail() {Thumbnail = byteasstring, Type = indexData[e].typeID, Source = "Package"};
                        thisPackage.ThumbnailImage.Add(thumb);
                    }
                }
            } else if (fileHas.Exists(x => x.Name == "CASP")) {
                List<string> instanceids = new();
                var entryspots = (from has in fileHas
                        where has.Name =="CASP"
                        select has.Location).ToList();
                foreach (int e in entryspots){
                    instanceids.Add(indexData[e].instanceID);
                }
                ReadThumbCache rtc = new(thisPackage, log, LogFile, fileHas, instanceids);
                thisPackage = rtc.thisPackage;
            } else if (fileHas.Exists(x => x.Name == "COBJ")) {
                List<string> instanceids = new();
                var entryspots = (from has in fileHas
                        where has.Name =="COBJ"
                        select has.Location).ToList();
                foreach (int e in entryspots){
                    instanceids.Add(indexData[e].instanceID);
                }
                ReadThumbCache rtc = new(thisPackage, log, LogFile, fileHas, instanceids);
                thisPackage = rtc.thisPackage;
            }

            LogMessage = string.Format("P{0} - All methods complete, moving on to getting info.", packageparsecount);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

            List<PackageTypeCounter> typecount = new List<PackageTypeCounter>();

            LogMessage = string.Format("P{0} - Making dictionary.", packageparsecount);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            
            foreach (typeList type in GlobalVariables.S4Types){
                var match = (from has in fileHas 
                            where has.TypeID == type.typeID
                            select has).ToList();                
                if (match.Count != 0){
                    PackageTypeCounter tc = new PackageTypeCounter();
                    tc.TypeDesc = type.desc;
                    tc.Count = match.Count;
                    tc.TypeID = type.typeID;
                    LogMessage = string.Format("There are {0} of {1} ({2}) in this package.", tc.Count, tc.TypeDesc, tc.TypeID);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    typecount.Add(tc);
                } 
            }

            thisPackage.Entries.AddRange(typecount);
            thisPackage.FileHas.AddRange(fileHas);

            distinctFlags = allFlags.Distinct().ToList();
            thisPackage.Flags.AddRange(distinctFlags);
            foreach (PackageFlag flag in allFlags){
                var inp = thisPackage.Flags.Where(c => c.Flag == flag.Flag);
                if (!inp.Any()){
                    thisPackage.Flags.Add(flag); 
                }                
            }
            foreach (TagsList tag in itemtags){
                var inp = thisPackage.CatalogTags.Where(c => c.TypeID == tag.TypeID);
                if (!inp.Any()){
                    thisPackage.CatalogTags.Add(tag); 
                }                
            }
            foreach (PackageInstance iid in allInstanceIDs){
                var inp = thisPackage.InstanceIDs.Where(c => c.InstanceID == iid.InstanceID);
                if (!inp.Any()){
                    thisPackage.InstanceIDs.Add(iid); 
                }                
            }
                     
            LogMessage = string.Format("P{0}: Checking {1} against override IDs.", packageparsecount,thisPackage.PackageName);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            
            LogMessage = string.Format("P{0}: Checking instances.", packageparsecount);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

            var overrides = (from r in GlobalVariables.S4OverridesList
                                where thisPackage.InstanceIDs.Any(mr => r.InstanceID == mr.InstanceID)
                                select r).ToList();
            
            if(overrides.Count > 0){
                LogMessage = string.Format("P{0}: Found {1} overrides!", packageparsecount, overrides.Count);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                thisPackage.Override = true;
                thisPackage.Type = "OVERRIDE";
            }           

            var specoverrides = GlobalVariables.S4SpecificOverridesList.Where(p => overrides.Any(l => p.Instance == l.InstanceID)).ToList();

            List<SpecificOverrides> speco = new List<SpecificOverrides>(specoverrides.Count);
            List<OverriddenList> OverridesList = new();
            foreach (OverridesList ov in overrides) {
                if (ov.InstanceID != "0000000000000000"){
                    var specco = GlobalVariables.S4SpecificOverridesList.Where(p => p.Instance == ov.InstanceID).ToList();                    
                    string description = "";
                    if (specco.Any()){
                        description = specco[0].Description;
                        thisPackage.Type = string.Format("OVERRIDE: {0}", description);
                    }
                    OverridesList.Add(new OverriddenList(){InstanceID = ov.InstanceID, Name = ov.Name, Pack = ov.Pack, Type = ov.Type});
                }
            }

            if (OverridesList.Any()){
                thisPackage.OverridesList.AddRange(OverridesList);
            }

            if (thisPackage.CatalogTags.Any()){
                LogMessage = string.Format("P{0}: Checking tags list for function.", packageparsecount);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                TagsList tag = new();
                List<TagsList> roomsort = new();
                bool identifiedTags = false;;
                bool identifiedRS = false;
                List<string> checks = new(){ "2380","2381","2382","2383","2384","2385","2219","264","351","459","18451","63538","265","12290","691","1661","159","157","160","158","1614","2203","1602","10263","55379","359","2083","1499","1984","63511","45089","63512","1728","1730","1729","759","2236","43025","1686","651","1603","28674","28678","28676","28675","28677","12302","10253","12373","10241","1345","12386","1349","1351","1350","12305","12301","817","55335","12303","67631","2429","795","12307","12308","12298","12299","2432","816","47142","1242","12306","67632","2430","10242","1344","12387","1346","1348","1347","1243","12388","12309","12310","12364","67633","12304","12300","67634","2431","815","67636","67604","1536","1537","1533","1518","1531","1532","1515","1534","1513","1517","1535","1519","1516","1514","1522","49154","1521","1523","1507","1509","1524","1508","59472","1510","1512","1511","83985","1506","2423","1505","57425","57424","269","2358","268","65551","347","2357","523","831","833","832","667","669","670","10260","671","672","673","674","970","176","456","457","968","2075","173","969","458","175","226","966","972","973","185","967","193","191","190","189","186","187","913","1032","61493","45069","797","798","799","800","67621","921","810","801","802","803","804","805","10256","806","807","808","43012","809","2246","65623","811","812","813","865","818","561","538","535","918","974","1611","544","541","554","556","1068","1067","1069","552","1081","550","537","915","976","1441","1442","2425","251","250","782","547","560","540","539","975","919","906","977","543","551","559","557","1065","1066","545","546","558","555","542","981","536","224","225","914","971","222","217","221","219","229","916","220","223","917","218","446","979","194","2188","823","978","785","199","231","252","195","207","965","964","209","202","1246","198","1496","200","203","201","824","197","1071","440","169","163","171","162","165","177","166","161","164","55356","55374","2240","178","298","309","308","307","303","302","304","305","306","301","299","300","441","570","1413","174","167","168","179","172","196","205","204","208","1718","206","310","1283","1293","1205","1396","24577","1446","1374","12365","1112","55375","2241","2014","1948","1944","1947","1978","1945","1976","1977","1946","1979","192","2042","183","180","920","182","181","184","1228","2211","59410","59416","59415","230","1122","1123","211","214","210","215","212","963","962","216","227","1072","213" };
                List<string> checks2 = new(){"1126","228","872","873","875","874","2237","891","2443","2349","892","55376","2242","2226","2227","2231","2232","412","415","408","411","413","414","410","409","428","768","769","790","791","792","1038","1039","793","794","980","770","2224","866","771","772","773","774","775","776","819","1656","45071","1596","429"};
                checks.AddRange(checks2);
                List<string> roomchecks = new() {"271","272","468","273","864","274","270","407","275","276"};
                var tagstrings = thisPackage.CatalogTags.Select(ct => ct.TypeID);
                foreach (string c in checks){
                    if (tagstrings.Contains(c)){                        
                        identifiedTags = true;
                        tag = thisPackage.CatalogTags.Where(ct => ct.TypeID == c).FirstOrDefault();
                        LogMessage = string.Format("P{0}: Found tag {1}: {2} {3}", packageparsecount, tag.TypeID, tag.Function, tag.Subfunction);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        break;
                    }
                }
                foreach (string rc in roomchecks){
                    if (tagstrings.Contains(rc)){
                        identifiedRS = true;
                        var rs = thisPackage.CatalogTags.Where(ct => ct.TypeID == rc).FirstOrDefault();
                        roomsort.Add(rs);
                        LogMessage = string.Format("P{0}: Found roomsort {1}: {2}", packageparsecount, rs.TypeID, rs.Function);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    }
                }                

                if (identifiedTags){
                    thisPackage.Function = tag.Function;
                    thisPackage.FunctionSubcategory = tag.Subfunction;
                    LogMessage = string.Format("P{0}: Package function identified from tags list: {1}: {2}", packageparsecount, thisPackage.Function, thisPackage.FunctionSubcategory);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                }
                if (identifiedRS){
                    foreach (TagsList tg in roomsort){
                        thisPackage.RoomSort.Add(new PackageRoomSort() {RoomSort = tag.Function});
                    }
                    LogMessage = string.Format("P{0}: Room sorts identified from tags list.", packageparsecount);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                }
                
            }

            var S4SM = typecount.Where(Type => Type.TypeDesc == "S4SM").Select(Type => Type.Count).FirstOrDefault();
            var BGEO = typecount.Where(Type => Type.TypeDesc == "BGEO").Select(Type => Type.Count).FirstOrDefault();
            var HOTC = typecount.Where(Type => Type.TypeDesc == "HOTC").Select(Type => Type.Count).FirstOrDefault();
            var SMOD = typecount.Where(Type => Type.TypeDesc == "SMOD").Select(Type => Type.Count).FirstOrDefault();
            var BOND = typecount.Where(Type => Type.TypeDesc == "BOND").Select(Type => Type.Count).FirstOrDefault();
            var CPRE = typecount.Where(Type => Type.TypeDesc == "CPRE").Select(Type => Type.Count).FirstOrDefault();
            var DMAP = typecount.Where(Type => Type.TypeDesc == "DMAP").Select(Type => Type.Count).FirstOrDefault();
            var RLE2 = typecount.Where(Type => Type.TypeDesc == "RLE2").Select(Type => Type.Count).FirstOrDefault();
            var CASP = typecount.Where(Type => Type.TypeDesc == "CASP").Select(Type => Type.Count).FirstOrDefault();
            var GEOM = typecount.Where(Type => Type.TypeDesc == "GEOM").Select(Type => Type.Count).FirstOrDefault();
            var LRLE = typecount.Where(Type => Type.TypeDesc == "LRLE").Select(Type => Type.Count).FirstOrDefault();
            var RLE = typecount.Where(Type => Type.TypeDesc == "RLE").Select(Type => Type.Count).FirstOrDefault();
            var RMAP = typecount.Where(Type => Type.TypeDesc == "RMAP").Select(Type => Type.Count).FirstOrDefault();
            var CLHD = typecount.Where(Type => Type.TypeDesc == "CLHD").Select(Type => Type.Count).FirstOrDefault();
            var STBL = typecount.Where(Type => Type.TypeDesc == "STBL").Select(Type => Type.Count).FirstOrDefault();
            var IMG = typecount.Where(Type => Type.TypeDesc == "_IMG").Select(Type => Type.Count).FirstOrDefault();
            //trait tuning
            var TRTR = typecount.Where(Type => Type.TypeDesc == "TRTR").Select(Type => Type.Count).FirstOrDefault();
            //snippet tuning
            var SNTR = typecount.Where(Type => Type.TypeDesc == "SNTR").Select(Type => Type.Count).FirstOrDefault();
            //interaction tuning
            var INTR = typecount.Where(Type => Type.TypeDesc == "INTR").Select(Type => Type.Count).FirstOrDefault();
            //action tuning
            var ACT = typecount.Where(Type => Type.TypeID == "0C772E27").Select(Type => Type.Count).FirstOrDefault();
            //test based score tuning
            var TBST = typecount.Where(Type => Type.TypeDesc == "TBST").Select(Type => Type.Count).FirstOrDefault();
            //buff tuning
            var BUFT = typecount.Where(Type => Type.TypeID == "6017E896").Select(Type => Type.Count).FirstOrDefault();
            //sim data
            var DATA = typecount.Where(Type => Type.TypeDesc == "DATA").Select(Type => Type.Count).FirstOrDefault();
            //rel bit
            var RBTR = typecount.Where(Type => Type.TypeDesc == "RBTR").Select(Type => Type.Count).FirstOrDefault();
            //rel bit
            var SGTR = typecount.Where(Type => Type.TypeDesc == "SGTR").Select(Type => Type.Count).FirstOrDefault();
            //aspiration
            var ASTR = typecount.Where(Type => Type.TypeDesc == "ASTR").Select(Type => Type.Count).FirstOrDefault();
            //pie menu
            var PMTR = typecount.Where(Type => Type.TypeDesc == "PMTR").Select(Type => Type.Count).FirstOrDefault();
            //broadcaster
            var BCTR = typecount.Where(Type => Type.TypeDesc == "BCTR").Select(Type => Type.Count).FirstOrDefault();
            //reward traits
            var RWTR = typecount.Where(Type => Type.TypeDesc == "RWTR").Select(Type => Type.Count).FirstOrDefault();
            //statistics
            var STTR = typecount.Where(Type => Type.TypeDesc == "STTR").Select(Type => Type.Count).FirstOrDefault();
            //career levels
            var CLTR = typecount.Where(Type => Type.TypeDesc == "CLTR").Select(Type => Type.Count).FirstOrDefault();
            //career tracks
            var CTTR = typecount.Where(Type => Type.TypeDesc == "CTTR").Select(Type => Type.Count).FirstOrDefault();
            //career
            var CATR = typecount.Where(Type => Type.TypeDesc == "CATR").Select(Type => Type.Count).FirstOrDefault();
            //objectives
            var OBTR = typecount.Where(Type => Type.TypeDesc == "OBTR").Select(Type => Type.Count).FirstOrDefault();
            //model
            var MODL = typecount.Where(Type => Type.TypeDesc == "MODL").Select(Type => Type.Count).FirstOrDefault();
            //model lod
            var MLOD = typecount.Where(Type => Type.TypeDesc == "MLOD").Select(Type => Type.Count).FirstOrDefault();
            //object catlog
            var COBJ = typecount.Where(Type => Type.TypeDesc == "COBJ").Select(Type => Type.Count).FirstOrDefault();
            

            if (thisPackage.Override == false){
                log.MakeLog("No overrides were found. Checking other options.", true);
                if (S4SM != 0){
                    thisPackage.Type = "Merged Package";
                } else if (!String.IsNullOrWhiteSpace(thisPackage.Function)) {
                    thisPackage.Type = thisPackage.Function;
                } else if (!String.IsNullOrWhiteSpace(thisPackage.Tuning)) {
                    if (thisPackage.Tuning.Contains("object_bassinetGEN")){
                        thisPackage.Type = "Bassinet";
                    }
                } else {                    
                    if (CLTR >= 1 || CTTR >= 1 || CATR >= 1){
                        thisPackage.Type = "MOD: Career";
                        thisPackage.IsMod = true;
                    } else if (SNTR >= 1 && INTR >= 1 && ACT >= 1 && TBST >= 1 && BUFT >= 1 && DATA >= 1 && RBTR >= 1 && SGTR >= 1 && ASTR >= 1 && PMTR >= 1 && BCTR >= 1 && RWTR >= 1 && STTR >= 1) {
                        thisPackage.Type = "MOD: Large";
                        thisPackage.IsMod = true;
                    } else if (OBTR >= 1 || SNTR >= 1 || INTR >= 1 || ACT >= 1 || TBST >= 1 || BUFT >= 1 || DATA >= 1 || RBTR >= 1 || SGTR >= 1 || ASTR >= 1 || PMTR >= 1 || BCTR >= 1 || RWTR >= 1 || STTR >= 1) {
                        thisPackage.Type = "MOD";
                        thisPackage.IsMod = true;
                    } else if (TRTR >= 1) {
                        thisPackage.Type = "MOD: Trait";
                        thisPackage.IsMod = true;
                    } else if (BGEO >= 1 && HOTC >= 1 && SMOD >= 1){
                        thisPackage.Type = "Slider";
                    } else if (BOND >= 1 && CPRE >= 1 && DMAP >= 1 && SMOD >= 1){
                        thisPackage.Type = "CAS Preset";
                    } else if (RLE2 >= 1 && CASP >= 1 && GEOM <= 0){
                        thisPackage.Type = "CAS Recolor";
                    } else if (LRLE >= 1 && CASP >= 1 && GEOM <= 0){
                        thisPackage.Type = "Makeup";
                    } else if (IMG >= 1 && CLHD >= 1 && STBL >= 1) {
                        thisPackage.Type = "Pose Pack";
                    } else {
                        LogMessage = string.Format("P{0}: Unable to identify package!", packageparsecount);
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        thisPackage.Type = "UNKNOWN";
                    }                    
                }
                if (GEOM >= 1 || MODL >= 1 || MLOD >= 1){
                    thisPackage.Mesh = true;
                }                
                if (CASP >= 1 || COBJ >= 1){
                    thisPackage.Recolor = true;
                }             
            }

            LogMessage = string.Format("This is a {0}!!", thisPackage.Type);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

            if (thisPackage.AgeGenderFlags.Any()){
                if ((thisPackage.AgeGenderFlags.Female == true) && (thisPackage.AgeGenderFlags.Male == true)){
                    thisPackage.Gender = "Both";
                } else if (thisPackage.AgeGenderFlags.Female == true){
                    thisPackage.Gender = "Female";
                } else if (thisPackage.AgeGenderFlags.Male == true){
                    thisPackage.Gender = "Male";
                }

                string age = "";
                if (thisPackage.AgeGenderFlags.Adult == true){
                    if (string.IsNullOrEmpty(age)){
                        age = "Adult";
                    } else {
                        age += string.Format(", Adult");
                    }
                }
                if (thisPackage.AgeGenderFlags.Baby == true){
                    if (string.IsNullOrEmpty(age)){
                        age = "Baby";
                    } else {
                        age += string.Format(", Baby");
                    }
                }
                if (thisPackage.AgeGenderFlags.Child == true){
                    if (string.IsNullOrEmpty(age)){
                        age = "Child";
                    } else {
                        age += string.Format(", Child");
                    }
                }
                if (thisPackage.AgeGenderFlags.Elder == true){
                    if (string.IsNullOrEmpty(age)){
                        age = "Elder";
                    } else {
                        age += string.Format(", Elder");
                    }
                }
                if (thisPackage.AgeGenderFlags.Infant == true){
                    if (string.IsNullOrEmpty(age)){
                        age = "Infant";
                    } else {
                        age += string.Format(", Infant");
                    }
                }
                if (thisPackage.AgeGenderFlags.Teen == true){
                    if (string.IsNullOrEmpty(age)){
                        age = "Teen";
                    } else {
                        age += string.Format(", Teen");
                    }
                }
                if (thisPackage.AgeGenderFlags.Toddler == true){
                    if (string.IsNullOrEmpty(age)){
                        age = "Toddler";
                    } else {
                        age += string.Format(", Toddler");
                    }
                }
                if (thisPackage.AgeGenderFlags.YoungAdult == true){
                    if (string.IsNullOrEmpty(age)){
                        age = "Young Adult";
                    } else {
                        age += string.Format(" and Young Adult");
                    }
                }
                thisPackage.Age = age;           
            }
            dbpfFile.Close();
            dbpfFile.Dispose();
            readFile.Close();
            readFile.Dispose();

            if (thisPackage.NoMesh == true){
                thisPackage.Orphan = false;
            } else if (thisPackage.Mesh == false && thisPackage.Recolor == true && thisPackage.Override == false){
                thisPackage.Orphan = true;  
            } else if (thisPackage.Mesh == true && thisPackage.Recolor == false && thisPackage.Override == false){
                thisPackage.Orphan = true;
            }           
            

            if (thisPackage.Function == "Hair" && thisPackage.Mesh == false){
                thisPackage.FunctionSubcategory = "Recolor";
            }


            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            string elapsedtime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                ts.Hours, ts.Minutes, ts.Seconds,
                                ts.Milliseconds / 10);
            GlobalVariables.currentpackage = packageinfo.Name;
            GlobalVariables.packagesRead++;

            //thisPackage = MakeNoNulls(thisPackage);
            GlobalVariables.AddPackages.Enqueue(thisPackage);            
            PackageFile packageFile = new PackageFile(){Name = thisPackage.PackageName, Location = thisPackage.Location, Game = thisPackage.Game};
            GlobalVariables.RemovePackages.Enqueue(packageFile);
            log.MakeLog(string.Format("Package Summary: {0}", thisPackage.SimsPackagetoString()), false);
            LogMessage = string.Format("Package Summary: {0}", thisPackage.SimsPackagetoString());
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            log.MakeLog(string.Format("Package Summary: {0}", thisPackage.SimsPackagetoString()), false);
            LogMessage = string.Format("Adding {0} to packages database.", thisPackage.PackageName);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

            LogMessage = string.Format("Reading file {0} took {1}", thisPackage.PackageName, elapsedtime);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            log.MakeLog(string.Format("Reading file {0} took {1}", thisPackage.PackageName, elapsedtime), false);           

            thisPackage = new();
            LogMessage = string.Format("Closing package # {0}/{1}: {2}", packageparsecount, GlobalVariables.PackageCount, packageinfo.Name);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            if(GlobalVariables.highdebug == false) log.MakeLog(string.Format("Log Dumped from StringBuilder: \n {0}", LogFile.ToString()), true);
            return;
        }  

        public SimsPackage MakeNoNulls(SimsPackage thisPackage){
            thisPackage.Title ??= "";
            thisPackage.Description ??= "";
            thisPackage.Subtype ??= "";
            thisPackage.Category ??= "";
            thisPackage.ModelName ??= "";
            thisPackage.PackageName ??= "";
            thisPackage.Type ??= "";
            thisPackage.GameString ??= "";
            thisPackage.Tuning ??= "";
            thisPackage.Creator ??= "";
            thisPackage.Age ??= "";
            thisPackage.Gender ??= "";
            thisPackage.MatchingMesh ??= "";
            if (!thisPackage.RequiredEPs.Any()){
                thisPackage.RequiredEPs.Add(new PackageRequiredEPs());
            }
            thisPackage.Function ??= "";
            thisPackage.FunctionSubcategory ??= "";
            if (!thisPackage.AgeGenderFlags.Any()){
                thisPackage.AgeGenderFlags = new();
            }
            if (!thisPackage.FileHas.Any()){
                thisPackage.FileHas.Add(new PackageEntries());
            }
            if (!thisPackage.RoomSort.Any()){
                thisPackage.RoomSort.Add(new PackageRoomSort());
            }
            if (!thisPackage.Components.Any()){
                thisPackage.Components.Add(new PackageComponent());
            }
            if (!thisPackage.Entries.Any()){
                thisPackage.Entries.Add(new PackageTypeCounter());
            }
            if (!thisPackage.Flags.Any()){
                thisPackage.Flags.Add(new PackageFlag());
            }
            if (!thisPackage.CatalogTags.Any()){
                thisPackage.CatalogTags.Add(new TagsList());
            }
            if (!thisPackage.Components.Any()){
                thisPackage.Components.Add(new PackageComponent ());
            }
            if (!thisPackage.OverridesList.Any()){
                thisPackage.OverridesList.Add(new OverriddenList ());
            }
            if (!thisPackage.MeshKeys.Any()){
                thisPackage.MeshKeys.Add(new PackageMeshKeys ());
            }
            if (!thisPackage.CASPartKeys.Any()){
                thisPackage.CASPartKeys.Add(new PackageCASPartKeys ());
            }
            if (!thisPackage.OBJDPartKeys.Any()){
                thisPackage.OBJDPartKeys.Add(new PackageOBJDKeys());
            }
            if (!thisPackage.MatchingRecolors.Any()){
                thisPackage.MatchingRecolors.Add(new PackageMatchingRecolors ());
            }
            if (!string.IsNullOrEmpty(thisPackage.MatchingMesh)){
                thisPackage.MatchingMesh = "";
            }
            if (!thisPackage.Conflicts.Any()){
                thisPackage.Conflicts.Add(new PackageConflicts());
            }
            

            return thisPackage;
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

    public struct ReadThumbCache {   
        public SimsPackage thisPackage;
        public StringBuilder LogFile;
        public ReadThumbCache(SimsPackage package, LoggingGlobals log, StringBuilder lf, List<PackageEntries> fileHas, List<string> instanceids){
            this.thisPackage = package;
            this.LogFile = lf;
            string LogMessage = "";
            List<string> parts = new();

            var cache = GlobalVariables.caches.Where(c => c.CacheName == "localthumbcache.package").FirstOrDefault();
            
            if (File.Exists(cache.CacheRename))
            {
                FileStream fs = new FileStream(cache.CacheRename, FileMode.Open, FileAccess.Read);
                BinaryReader readFile = new BinaryReader(fs);
                List<indexEntry> indexData = new List<indexEntry>();

                long entrycountloc = 36;
                long indexRecordPositionloc = 64;

                readFile.BaseStream.Position = 0;

                readFile.BaseStream.Position = entrycountloc;

                uint entrycount = readFile.ReadUInt32();
                LogMessage = string.Format("THUMBCACHE Entry Count: {0}", entrycount.ToString());
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                
                //record position low
                uint indexRecordPositionLow = readFile.ReadUInt32();
                LogMessage = string.Format("THUMBCACHE IndexRecordPositionLow: {0}", indexRecordPositionLow.ToString());
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                
                //index record size
                uint indexRecordSize = readFile.ReadUInt32();
                LogMessage = string.Format("THUMBCACHE IndexRecordSize: {0}", indexRecordSize.ToString());
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                
                readFile.BaseStream.Position = indexRecordPositionloc;

                ulong indexRecordPosition = readFile.ReadUInt64();
                LogMessage = string.Format("THUMBCACHE Index Record Position: {0}", indexRecordPosition.ToString());
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                
                            
                byte[] headersize = new byte[96];
                long here = 100;
                long movedto = 0;


                if (indexRecordPosition != 0){
                    long indexseek = (long)indexRecordPosition - headersize.Length;
                    movedto = here + indexseek;
                    readFile.BaseStream.Position = here + indexseek;                
                } else {
                    movedto = here + indexRecordPositionLow;
                    readFile.BaseStream.Position = here + indexRecordPositionLow;
                }

                readFile.BaseStream.Position = (long)indexRecordPosition + 4;

                for (int i = 0; i < entrycount; i++){                    
                    indexEntry holderEntry = new indexEntry();                
                    holderEntry.typeID = readFile.ReadUInt32().ToString("X8");
                    LogMessage = string.Format("THUMBCACHE E{0} - Index Entry TypeID: {1}", i, holderEntry.typeID);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

                    List<typeList> type = GlobalVariables.S4FunctionTypesConnection.Query<typeList>(string.Format("SELECT * FROM S4Types where TypeID='{0}'", holderEntry.typeID));
                    
                    holderEntry.groupID = readFile.ReadUInt32().ToString("X8");
                    LogMessage = string.Format("THUMBCACHE E{0} - Index Entry GroupID: {1}", i, holderEntry.groupID);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    
                    string instanceid1 = (readFile.ReadUInt32() << 32).ToString("X8");
                    string instanceid2 = (readFile.ReadUInt32() << 32).ToString("X8");
                    holderEntry.instanceID = string.Format("{0}{1}", instanceid1,instanceid2);
                    LogMessage = string.Format("THUMBCACHE E{0} - InstanceID: {1}", i, holderEntry.instanceID);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

                    uint testin = readFile.ReadUInt32();
                    holderEntry.position = (long)testin;
                    LogMessage = string.Format("THUMBCACHE E{0} - Position: {1}", i, holderEntry.position);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

                    holderEntry.fileSize = readFile.ReadUInt32();
                    LogMessage = string.Format("THUMBCACHE E{0} - File Size: {1}", i, holderEntry.fileSize);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

                    holderEntry.memSize = readFile.ReadUInt32();
                    LogMessage = string.Format("THUMBCACHE E{0} - Mem Size: {1}", i, holderEntry.memSize);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

                    holderEntry.compressionType = readFile.ReadUInt16().ToString("X4");
                    LogMessage = string.Format("THUMBCACHE E{0} - Compression Type: {1}", i, holderEntry.compressionType);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

                    readFile.BaseStream.Position = readFile.BaseStream.Position + 2;

                    indexData.Add(holderEntry);

                    holderEntry = null;
                }

                foreach (string id in instanceids){
                    LogMessage = string.Format("THUMBCACHE THUM - Searching for entry matching {0}", id);
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                            
                    var thumb = indexData.Where(i => i.instanceID == id).FirstOrDefault();
                    if (thumb != null){
                        readFile.BaseStream.Position = thumb.position;
                        if (thumb.compressionType == "5A42"){  
                            int entryEnd = (int)readFile.BaseStream.Position + (int)thumb.memSize;
                            LogMessage = string.Format("THUMBCACHE THUM - Position: {0}", thumb.position);
                            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                            LogMessage = string.Format("THUMBCACHE THUM - File Size: {0}", thumb.fileSize);
                            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                            LogMessage = string.Format("THUMBCACHE THUM - Memory Size: {0}", thumb.memSize);
                            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                            LogMessage = string.Format("THUMBCACHE THUM - Entry Ends At: {0}", entryEnd);
                            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                            MemoryStream decomps = S4Decryption.DecompressMS(Methods.ReadEntryBytes(readFile, (int)thumb.memSize));
                            byte[] imagebyte = decomps.ToArray();
                            string byteasstring = Convert.ToBase64String(imagebyte);                            
                            PackageThumbnail thum = new PackageThumbnail() {Thumbnail = byteasstring, Type = thumb.typeID, Source = "Thumbcache"};
                            thisPackage.ThumbnailImage.Add(thum);
                            return;
                        } else if (thumb.compressionType == "0000"){
                            byte[] imagebyte = Methods.ReadEntryBytes(readFile, (int)thumb.memSize);
                            string byteasstring = Convert.ToBase64String(imagebyte);                            
                            PackageThumbnail thum = new PackageThumbnail() {Thumbnail = byteasstring, Type = thumb.typeID, Source = "Thumbcache"};
                            thisPackage.ThumbnailImage.Add(thum);
                            return;
                        }
                    }
                }
            }
        }
    }
    

    public struct ProcessCOBJ {
        public SimsPackage package;
        public List<PackageInstance> iids;
        public List<TagsList> tagl;
        public StringBuilder lf;

        public ProcessCOBJ(SimsPackage thisPackage, BinaryReader readFile, StringBuilder LogFile, string LogMessage, int packageparsecount, int e, List<TagsList> itemtags, List<PackageInstance> allInstanceIDs){
            this.package = thisPackage;
            this.iids = allInstanceIDs;
            this.tagl = itemtags;
            
            ReadCOBJ rc = new ReadCOBJ(readFile, packageparsecount, e, itemtags, LogFile);  
            thisPackage.OBJDPartKeys.AddRange(rc.objkeys);
            iids.Add(new PackageInstance() {PackageID = rc.instanceid.ToString("X8")});
            foreach (TagsList tag in rc.itemtags) {
                tagl.Add(tag);                                 
            }
        this.lf = LogFile;
        }        
    }

    public struct ProcessOBJD {
        public SimsPackage package;
        public List<PackageInstance> iids;
        public List<TagsList> tagl;
        public StringBuilder lf;


        public ProcessOBJD(SimsPackage thisPackage, BinaryReader readFile, StringBuilder LogFile, string LogMessage, int packageparsecount, int e, List<TagsList> itemtags, List<PackageInstance> allInstanceIDs, int objdc, LoggingGlobals log){
            this.package = thisPackage;
            string[] objde;
            int[] objdp;
            this.iids = allInstanceIDs;
            this.tagl = itemtags;
            

            ReadOBJDIndex readOBJD = new ReadOBJDIndex(readFile, packageparsecount, objdc, LogFile);
            LogMessage = string.Format("P{0}/OBJD{1} - There are {2} entries to read.", packageparsecount, objdc, readOBJD.count);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            objde = new string[readOBJD.count];
            objdp = new int[readOBJD.count];
            for (int f = 0; f < readOBJD.count; f++){
                LogMessage = string.Format("P{0}/OBJD{1} - Entry {2}: \n--- Type: {3}\n --- Position: {4}", packageparsecount, objdc, f, readOBJD.entrytype[f], readOBJD.position[f]);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                objde[f] = readOBJD.entrytype[f].ToString();
                objdp[f] = (int)readOBJD.position[f];
            }
            readFile.BaseStream.Position = objdp[0];
            ReadOBJDEntry readobjdentry = new ReadOBJDEntry(readFile, objde, objdp, packageparsecount, objdc, LogFile);
            thisPackage.Title = readobjdentry.name;
            thisPackage.Tuning = readobjdentry.tuningname;
            thisPackage.TuningID = (int)readobjdentry.tuningid;            

            thisPackage.MeshKeys.AddRange(readobjdentry.meshes);
            
            iids.Add(new PackageInstance() {PackageID = readobjdentry.instance.ToString("X8")});  
            
            LogMessage = string.Format("Adding components to package: ");
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                
            for (int c = 0; c < readobjdentry.componentcount; c++){
                LogMessage = readobjdentry.components[c].ToString();
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                LogMessage = readobjdentry.components[c].ToString("X8");
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                thisPackage.Components.Add(new PackageComponent() {Component = readobjdentry.components[c].ToString("X8")});
            }
            
            this.lf = LogFile;
        }
    }

    public struct ProcessCASP{
        public StringBuilder lf;
        public List<PackageFlag> allFlags;
        public SimsPackage thisPackage;
        public List<TagsList> itemtags;

        public ProcessCASP(SimsPackage package, BinaryReader readFile, Stream dbpfFile, StringBuilder LogFile, string LogMessage, int packageparsecount, int e, List<TagsList> tagsl, LoggingGlobals log, List<PackageFlag> flagslist, string[] parameters){
            this.allFlags = flagslist;
            this.thisPackage = package;
            this.itemtags = tagsl;

            uint version = readFile.ReadUInt32();
            LogMessage = string.Format("P{0}/CASP{1} [Decompressed] - Version: {2}", packageparsecount, e, version);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            uint tgioffset = readFile.ReadUInt32() +8;
            LogMessage = string.Format("P{0}/CASP{1} [Decompressed] - TGI Offset: {2} \n -- As Hex: {3}", packageparsecount, e, tgioffset, tgioffset.ToString("X8"));
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            uint numpresets = readFile.ReadUInt32();
            LogMessage = string.Format("P{0}/CASP{1} [Decompressed] - Number of Presets: {2} \n -- As Hex: {3}", packageparsecount, e, numpresets, numpresets.ToString("X8"));
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));            
            using (BinaryReader reader = new BinaryReader(dbpfFile, Encoding.BigEndianUnicode, true))
            {
                thisPackage.Title = reader.ReadString();
            }
            LogMessage = string.Format("P{0}/CASP{1} [Decompressed] - Title: {2}", packageparsecount, e, thisPackage.Title);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

            float sortpriority = readFile.ReadSingle();
            LogMessage = string.Format("P{0}/CASP{1} [Decompressed] - Sort Priority: {2}", packageparsecount, e, sortpriority);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

            int secondarySortIndex = readFile.ReadUInt16();
            LogMessage = string.Format("P{0}/CASP{1} [Decompressed] - Secondary Sort Index: {2}", packageparsecount, e, secondarySortIndex);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

            uint propertyid = readFile.ReadUInt32();
            LogMessage = string.Format("P{0}/CASP{1} [Decompressed] - Property ID: {2}", packageparsecount, e, propertyid);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
            
            uint auralMaterialHash = readFile.ReadUInt32();
            LogMessage = string.Format("P{0}/CASP{1} [Decompressed] - Aural Material Hash: {2}", packageparsecount, e, sortpriority);
            if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
            if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

            if (version <= 42){
                LogMessage = string.Format("Version is <= 42: {0}", version);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                int[] parameterFlag = new int[1];
                parameterFlag[0] = (int)readFile.ReadUInt16();
                BitArray parameterFlags = new BitArray(parameterFlag);
                LogMessage = parameterFlags.Length.ToString();
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                for(int p = 0; p < 16; p++)
                {
                    if (parameterFlags[p] == true) {
                        var af = allFlags.Where(x => x.Flag == parameters[p]);
                        if (af.Any()){
                            allFlags.Add(new PackageFlag(){Flag = parameters[p]});  
                        }
                    }
                    LogMessage = string.Format("P{0}/CASP{1} [Decompressed] - Function Sort Flag [{2}]: {3}, {4}", packageparsecount, e, p, parameters[p], parameterFlags[p].ToString());
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                } 
                
            } else if (version >= 43){
                LogMessage = string.Format("Version is >= 43: {0}", version);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                int[] parameterFlag = new int[1];
                parameterFlag[0] = (int)readFile.ReadUInt16();
                BitArray parameterFlags = new BitArray(parameterFlag);

                for(int pfc = 0; pfc < 16; pfc++){
                    if (parameterFlags[pfc] == true) {
                        var af = allFlags.Where(x => x.Flag == parameters[pfc]);
                        if (af.Any() && pfc == 2) {
                            allFlags.Add(new PackageFlag(){Flag = parameters[pfc]});   
                            thisPackage.AllowForCASRandom = true; 
                        } else if (af.Any()){
                            allFlags.Add(new PackageFlag(){Flag = parameters[pfc]});  
                        }
                    }
                    LogMessage = string.Format("P{0}/CASP{1} [Decompressed] - Function Sort Flag [{2}]: {3}, {4}", packageparsecount, e, pfc, parameters[pfc], parameterFlags[pfc].ToString());
                    if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                    if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                }                            
            }
                ulong excludePartFlags = readFile.ReadUInt64();
                LogMessage = string.Format("P{0}/CASP{1} [Decompressed] - Exclude Part Flags: {2}", packageparsecount, e, excludePartFlags.ToString("X16"));
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                ulong excludePartFlags2 = readFile.ReadUInt64();
                LogMessage = string.Format("P{0}/CASP{1} [Decompressed] - Exclude Part Flags 2: {2}", packageparsecount, e, excludePartFlags2.ToString("X16"));
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                ulong excludeModifierRegionFlags = readFile.ReadUInt64();
                LogMessage = string.Format("P{0}/CASP{1} [Decompressed] - Exclude Modifier Region Flags: {2}", packageparsecount, e, excludeModifierRegionFlags.ToString("X16"));
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

            if (version >= 37){
                LogMessage = string.Format(">= 37, Version: {0}", version);
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                uint count = readFile.ReadByte();
                LogMessage = string.Format("P{0}/CASP{1} [Decompressed] - Tag Count: {2}", packageparsecount, e, count.ToString());
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                readFile.ReadByte();
                CASTag16Bit tags = new CASTag16Bit(readFile, count);                            
                List<TagsList> gottags = Readers.GetTagInfo(tags, count, LogFile);
                foreach (TagsList tag in gottags){
                    itemtags.Add(tag);
                }
                                            
            } 
            else 
            {
                uint count = readFile.ReadByte();
                LogMessage = string.Format("P{0}/CASP{1} [Decompressed] - Tag Count: {2}", packageparsecount, e, count.ToString());
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                readFile.ReadByte();
                CASTag16Bit tags = new CASTag16Bit(readFile, count);
                
                List<TagsList> gottags = Readers.GetTagInfo(tags, count, LogFile);
                foreach (TagsList tag in gottags){
                    itemtags.Add(tag);
                }

                }

                uint simoleonprice = readFile.ReadUInt32();
                LogMessage = string.Format("P{0}/CASP{1} [Decompressed] - Simoleon Price: {2}", packageparsecount, e, simoleonprice.ToString());
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                uint partTitleKey = readFile.ReadUInt32();
                LogMessage = string.Format("P{0}/CASP{1} [Decompressed] - Part Title Key: {2}", packageparsecount, e, partTitleKey.ToString());
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                uint partDescriptionKey = readFile.ReadUInt32();
                LogMessage = string.Format("P{0}/CASP{1} [Decompressed] - Part Description Key: {2}", packageparsecount, e, partDescriptionKey.ToString());
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));                            
                if (version >= 43) {
                    uint createDescriptionKey = readFile.ReadUInt32();
                }
                int uniqueTextureSpace = readFile.ReadByte();
                LogMessage = string.Format("P{0}/CASP{1} [Decompressed] - Unique Texture Space: {2}", packageparsecount, e, uniqueTextureSpace.ToString("X8"));
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                uint bodytype = readFile.ReadUInt32();
                bool foundmatch = false;

                List<FunctionListing> bodytypes = GlobalVariables.S4FunctionTypesConnection.Query<FunctionListing>(string.Format("SELECT * FROM S4CASFunctions where BodyType='{0}'", bodytype)); 

                if (bodytypes.Any()){
                    foundmatch = true;
                    thisPackage.Function = bodytypes[0].Function;
                    if (!String.IsNullOrWhiteSpace(bodytypes[0].Subfunction)) {
                        thisPackage.FunctionSubcategory = bodytypes[0].Subfunction;
                    } 
                }

                if (foundmatch == false){
                    thisPackage.Function = string.Format("Unidentified function (contact SinfulSimming). Code: {0}", bodytype.ToString());
                }                            
                LogMessage = string.Format("P{0}/CASP{1} [Decompressed] - Bodytype: {2}", packageparsecount, e, bodytype.ToString());
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                uint bodytypesubtype = readFile.ReadUInt16();
                LogMessage = string.Format("P{0}/CASP{1} [Decompressed] - Body Sub Type: {2}", packageparsecount, e, bodytypesubtype.ToString());
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                readFile.ReadUInt32();                        
                uint agflags = readFile.ReadUInt32();
                LogMessage = string.Format("P{0}/CASP{1} [Decompressed] - Ag Flags: {2}", packageparsecount, e, agflags.ToString("X8"));
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

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
                
                LogMessage = string.Format("P{0}/CASP{1} Ages:\n -- Adult: {2}, \n -- Baby: {3}, \n -- Child: {4}, \n -- Elder: {5}, \n -- Infant: {6}, \n -- Teen: {7}, \n -- Toddler: {8}, \n -- Young Adult: {9}\n\nGender: \n -- Male: {10}\n -- Female: {11}.", packageparsecount, e, agegenderset.Adult.ToString(), agegenderset.Baby.ToString(), agegenderset.Child.ToString(), agegenderset.Elder.ToString(), agegenderset.Infant.ToString(), agegenderset.Teen.ToString(), agegenderset.Toddler.ToString(), agegenderset.YoungAdult.ToString(), agegenderset.Female.ToString(), agegenderset.Male.ToString());
                if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));

                thisPackage.AgeGenderFlags = agegenderset;  

                if (version >= 0x20)
                {
                    uint species = readFile.ReadUInt32();
                }
                if (version >= 34)
                {
                    int packID = readFile.ReadInt16();
                    int packFlags = readFile.ReadByte();
                    for(int p = 0; p < packFlags; p++)
                    {
                        bool check = readFile.ReadBoolean();
                        LogMessage = string.Format("P{0}/CASP{1} [Decompressed] - Packflag is: {2}", packageparsecount, e, check.ToString());
                        if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                    } 
                    byte[] reserved2 = readFile.ReadBytes(9);
                }
                else
                {
                    byte unused2 = readFile.ReadByte();
                    if (unused2 > 0) {
                        int unused3 = readFile.ReadByte();
                    }
                }

                uint buffResKey = readFile.ReadByte();
                uint varientThumbnailKey = readFile.ReadByte();
                if (version >= 28){
                    ulong voiceEffecthash = readFile.ReadUInt64();
                }
                if (version >= 30){
                    uint usedMaterialCount = readFile.ReadByte();
                    if (usedMaterialCount > 0){
                        uint materialSetUpperBodyHash = readFile.ReadUInt32();
                        uint materialSetLowerBodyHash = readFile.ReadUInt32();
                        uint materialSetShoesBodyHash = readFile.ReadUInt32();
                    }
                }
                if (version >= 31){
                    uint hideForOccultFlags = readFile.ReadUInt32();
                }

                if (version >= 38){
                    ulong oppositeGenderPart = readFile.ReadUInt64();
                }

                if (version >= 39)
                {
                    ulong fallbackPart = readFile.ReadUInt64();
                }
                if (version >= 44)
                {
                    float opacitySliderMin = readFile.ReadSingle();
                    float opacitySliderInc = readFile.ReadSingle();

                    float hueMin = readFile.ReadSingle();
                    float hueMax = readFile.ReadSingle();
                    float hueInc = readFile.ReadSingle();

                    float satMin = readFile.ReadSingle();
                    float satMax = readFile.ReadSingle();
                    float satInc = readFile.ReadSingle();

                    float brgMin = readFile.ReadSingle();
                    float brgMax = readFile.ReadSingle();
                    float brgInc = readFile.ReadSingle();
                }

                uint nakedKey = readFile.ReadByte();
                uint parentKey = readFile.ReadByte();
                uint sortLayer = readFile.ReadUInt32();

                var currentPosition = readFile.BaseStream.Position;

                readFile.BaseStream.Position = tgioffset;
                var tginum = readFile.ReadByte();
                for (int t = 0; t < tginum; t++){
                    ulong iid = readFile.ReadUInt64();
                    uint gid = readFile.ReadUInt32();
                    uint tid = readFile.ReadUInt32();
                    string key = string.Format("{0}-{1}-{2}", tid.ToString("X8"), gid.ToString("X8"), iid.ToString("X16"));
                    if (key != "00000000-00000000-0000000000000000"){
                        var match = thisPackage.CASPartKeys.Where(c => c.CASPartKey == key);
                        if (!match.Any()){
                            PackageCASPartKeys pcpk = new() {CASPartKey = key};
                            thisPackage.CASPartKeys.Add(pcpk);
                        } 
                    }                                
                }
                
                readFile.BaseStream.Position = currentPosition;
                var lodcount = readFile.ReadByte();
                if (lodcount == 0){
                    thisPackage.NoMesh = true;
                } else {
                    for (int t = 0; t < lodcount; t++){
                        ulong iid = readFile.ReadUInt64();
                        uint gid = readFile.ReadUInt32();
                        uint tid = readFile.ReadUInt32();
                        string key = string.Format("{0}-{1}-{2}", tid.ToString("X8"), gid.ToString("X8"), iid.ToString("X16"));
                        if (key != "00000000-00000000-0000000000000000"){
                            var match = thisPackage.CASPartKeys.Where(c => c.CASPartKey == key);
                            if (!match.Any()){
                                PackageCASPartKeys pcpk = new() {CASPartKey = key};
                                thisPackage.CASPartKeys.Add(pcpk);
                            } 
                        }                                
                    }
                }
                







            this.lf = LogFile;
        }
        
    }
}