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
using SimsCCManager.Decryption.EndianDecoding;

namespace SimsCCManager.Packages.Sims4Search
{
    public static class extensions {
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
        public ulong instance;
        public uint type;
        public uint group;
        
        public ResourceKeyITG(BinaryReader reader){
            this.instance = reader.ReadUInt64(); 
            this.type = reader.ReadUInt32(); 
            this.group = reader.ReadUInt32();       
        }

        public override string ToString() => $"{type.ToString("X8")}-{group.ToString("X8")}-{instance.ToString("X16")}";
    }
    public struct ResourceKeyITGFlip {
        public ulong instance;
        public uint type;
        public uint group;
        
        public ResourceKeyITGFlip(BinaryReader reader){
            uint left = reader.ReadUInt32();
            uint right = reader.ReadUInt32();
            ulong longleft = left;
            longleft = (longleft << 32);
            this.instance = longleft | right;
            this.type = reader.ReadUInt32(); 
            this.group = reader.ReadUInt32();       
        }

        public override string ToString() => $"{type.ToString("X8")}-{group.ToString("X8")}-{instance.ToString("X16")}";
    }

    public struct Tag {
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
    public struct CASTag {
        public int[] tagKey;  
        public int[] catKey;
        public int[] empty; 

        public CASTag(BinaryReader reader, uint count){
            if (count == 1){
                this.tagKey = new int[count];
                this.catKey = new int[count];
                this.empty = new int[count];
                this.empty[0] = 0;
                this.tagKey[0] = reader.ReadUInt16();
                this.catKey[0] = reader.ReadUInt16();
                
            } else {
                this.tagKey = new int[count];
                this.catKey = new int[count];
                this.empty = new int[count];
                for (int i = 0; i < count; i++){
                    this.empty[i] = reader.ReadUInt16();
                    this.tagKey[i] = reader.ReadUInt16(); 
                    this.catKey[i] = reader.ReadUInt16(); 
                     
                }
            }
        }
    }
    /*public struct BoolTag {
        public bool[] truefalse; 
        public string[] value;
        

        public BoolTag(BinaryReader reader, int count){
            if (count == 1){
                this.truefalse = new bool[0];
                this.truefalse[0] = reader.ReadBoolean();
                this.value = new string[0];
                this.value[0] = parameters[0];
            } else {
                this.value = new string[count];
                this.truefalse = new bool[count];
                for (int i = 0; i < count; i++){
                    var parameter = parameters[i];
                    this.value[i] = parameter;
                    this.truefalse[i] = reader.ReadBoolean();
                }
            }
        }
    }*/
    
    public struct ReadOBJDIndex {
        public int version;
        public uint refposition;
        public int count;
        public uint[] entrytype;
        public uint[] position;

        public ReadOBJDIndex(BinaryReader reader){
            this.version = reader.ReadUInt16();
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

    public struct ReadOBJDEntry {
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
        public string icon;
        public string rig;
        public string slot;
        public string model;
        public string footprint;
        public ReadOBJDEntry(BinaryReader reader){
            this.namelength = reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            //this.namelength = this.namelength * 2;
            this.namebit = reader.ReadBytes(namelength);
            this.name = Encoding.UTF8.GetString(namebit);
            this.tuningnamelength = reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            this.tuningbit = reader.ReadBytes(tuningnamelength);
            this.tuningname = Encoding.UTF8.GetString(tuningbit);
            this.tuningid = reader.ReadUInt64();
            reader.ReadUInt32();
            ResourceKeyITGFlip rkicon = new ResourceKeyITGFlip(reader);
            this.icon = rkicon.ToString();
            reader.ReadUInt32();
            ResourceKeyITGFlip rkrig = new ResourceKeyITGFlip(reader);
            this.rig = rkrig.ToString();
            reader.ReadUInt32();
            ResourceKeyITGFlip rkslot = new ResourceKeyITGFlip(reader);
            this.slot = rkslot.ToString();
            reader.ReadUInt32();
            ResourceKeyITGFlip rkmodel = new ResourceKeyITGFlip(reader);
            this.model = rkmodel.ToString();
            reader.ReadUInt32();
            ResourceKeyITGFlip rkft = new ResourceKeyITGFlip(reader);
            this.footprint = rkft.ToString();
            this.componentcount = reader.ReadUInt32();
            this.components = new uint[this.componentcount];
            for (int i = 0; i < this.componentcount; i++){
                components[i] = reader.ReadUInt32();
            }            
            this.materialvariantlength = reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            this.materialvariantbyte = reader.ReadBytes(materialvariantlength);
            this.materialvariant = Encoding.UTF8.GetString(materialvariantbyte);
            this.price = reader.ReadUInt32();
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
        
        // Class References
        LoggingGlobals log = new LoggingGlobals();
        ReadEntries readentries = new ReadEntries();   
        System.Text.Encoding encoding = System.Text.Encoding.BigEndianUnicode;
        GlobalVariables globals = new GlobalVariables();

        //Vars
        uint chunkOffset = 0;
        int contentposition = 64;
        int contentpositionalt = 40;
        int contentcount = 36;
        public string[] parameters = {"DefaultForBodyType","DefaultThumbnailPart","AllowForCASRandom","ShowInUI","ShowInSimInfoPanel","ShowInCasDemo","AllowForLiveRandom","DisableForOppositeGender","DisableForOppositeFrame","DefaultForBodyTypeMale","DefaultForBodyTypeFemale","Unk","Unk","Unk","Unk","Unk"};

        public void SearchS4Packages(string file) {
            var packageparsecount = GlobalVariables.packagesRead;   
            GlobalVariables.packagesRead++;       

            //Vars for Package Info
            string typefound = "";  
                      
        
            //Misc Vars
            string test = "";
            
            const EndianType endiant = EndianType.Little;
            byte[] uncompresseddata;
            

            SimsPackage thisPackage = new SimsPackage();            

            //Lists 
            
            List<EntryHolder> entries = new List<EntryHolder>();
            List<fileHasList> fileHas = new List<fileHasList>();
            ArrayList linkData = new ArrayList();
            List<indexEntry> indexData = new List<indexEntry>();
            FileInfo packageinfo = new FileInfo(file); 

            List<TagsList> itemtags = new List<TagsList>();
            List<TagsList> styletags = new List<TagsList>();
            List<TagsList> sellingpointlist = new List<TagsList>();
            List<TagsList> distinctItemTags = new List<TagsList>();
            List<TagsList> distinctSellingPointTags = new List<TagsList>();
            List<TagsList> distinctStyleTags = new List<TagsList>();

            List<string> allFlags = new List<string>();      
            List<string> distinctFlags = new List<string>(); 
            List<string> allGUIDS = new List<string>();      
            List<string> distinctGUIDS = new List<string>();  
            List<string> allInstanceIDs = new List<string>();      
            List<string> distinctInstanceIDs = new List<string>();  

            //create readers  
            FileStream dbpfFile = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader readFile = new BinaryReader(dbpfFile); 

            //log opening file
            Console.WriteLine("Reading Package #" + packageparsecount + "/" + GlobalVariables.PackageCount + ": " + packageinfo.Name);
            log.MakeLog("Logged Package #" + packageparsecount + " as " + packageinfo.FullName, true);
            thisPackage.Location = packageinfo.FullName;            
            thisPackage.Game = 4;
            log.MakeLog("Logged Package #" + packageparsecount + " as meant for The Sims " + thisPackage.Game, true);           
            
            //start actually reading the package 
            int counter = 0;

            //dbpf
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
            log.MakeLog("Unused: " + test, true);

            byte[] headersize = new byte[96];

            if (indexRecordPosition != 0){
                long indexseek = (long)indexRecordPosition - headersize.Length;
                dbpfFile.Seek(indexseek, SeekOrigin.Current);
            } else {
                long indexseek = indexRecordPositionLow - headersize.Length;    
                dbpfFile.Seek(indexRecordPositionLow, SeekOrigin.Current);
            }
            
            

            for (int i = 0; i < entrycount; i++){
                indexEntry holderEntry = new indexEntry();
                
                holderEntry.typeID = readFile.ReadUInt32().ToString("X8");
                log.MakeLog("P" + packageparsecount + "/E" + i + " - Index Entry TypeID: " + holderEntry.typeID, true);

                foreach (typeList type in TypeListings.AllTypesS4){
                    if (type.typeID == holderEntry.typeID){
                        fileHas.Add(new fileHasList() { term = type.desc, location = i});
                    }
                }               

                holderEntry.groupID = readFile.ReadUInt32().ToString("X8");
                log.MakeLog("P" + packageparsecount + "/E" + i + " - Index Entry GroupID: " + holderEntry.groupID, true);
                
                string instanceid1 = (readFile.ReadUInt32() << 32).ToString("X8");
                string instanceid2 = (readFile.ReadUInt32() << 32).ToString("X8");
                holderEntry.instanceID = instanceid1 + instanceid2;
                allInstanceIDs.Add(holderEntry.instanceID);
                log.MakeLog("P" + packageparsecount + "/E" + i + " - InstanceID: " + holderEntry.instanceID, true);

                int testin = readFile.ReadValueS32(endiant);
                holderEntry.position = (long)testin;
                log.MakeLog("P" + packageparsecount + "/E" + i + " - Position " + testin.ToString(), true);

                testint = readFile.ReadValueU32(endiant);
                holderEntry.fileSize = testint;
                log.MakeLog("P" + packageparsecount + "/E" + i + " - File Size " + testint.ToString("X8"), true);

                testint = readFile.ReadValueU32(endiant);
                holderEntry.memSize = testint;
                log.MakeLog("P" + packageparsecount + "/E" + i + " - Mem Size " + testint.ToString("X8"), true);

                testint = readFile.ReadValueU16(endiant);
                holderEntry.compressionType = testint.ToString("X4");
                log.MakeLog("P" + packageparsecount + "/E" + i + " - Compression Type " + testint.ToString("X4"), true);

                testint = readFile.ReadValueU16(endiant);
                log.MakeLog("P" + packageparsecount + "/E" + i + " - Confirmed: " + testint.ToString("X4"), true);

                indexData.Add(holderEntry);

                holderEntry = null;
            }

            log.MakeLog("This package contains: ", true);
            foreach (fileHasList type in fileHas){
                log.MakeLog(type.term + " at location " + type.location, true);
            }
            
            if (fileHas.Exists(x => x.term == "CASP")){
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
                    log.MakeLog("P" + packageparsecount + "/E" + e + " - Opening CASP #" + caspc, true);
                    if (indexData[e].compressionType == "5A42"){
                            dbpfFile.Seek(indexData[e].position, SeekOrigin.Begin);
                            long entryEnd = indexData[e].position + indexData[e].memSize;
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Position: " + indexData[e].position, true);
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Filesize: " + indexData[e].fileSize, true);
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Memsize: " + indexData[e].memSize, true);
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Entry ends at " + entryEnd, true);
                            byte[] entry = readFile.ReadBytes((int)indexData[e].memSize);
                            Stream decomps = S4Decryption.Decompress(entry);
                            
                            BinaryReader decompbr = new BinaryReader(decomps);

                            uint version = decompbr.ReadUInt32();
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Version: " + version, true);
                            log.MakeLog("-- As hex: " + version.ToString("X8"), true);
                            uint tgioffset = decompbr.ReadUInt32() +8;
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - TGI Offset: " + tgioffset, true);
                            log.MakeLog("-- As hex: " + tgioffset.ToString("X8"), true);
                            uint numpresets = decompbr.ReadUInt32();
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Number of Presets: " + numpresets, true);
                            log.MakeLog("-- As hex: " + numpresets.ToString("X8"), true);
                            using (var reader = new BinaryReader(decomps, Encoding.BigEndianUnicode, true))
                            {
                                thisPackage.Title = reader.ReadString();
                            }                            
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Name: " + thisPackage.Title, true);

                            float sortpriority = decompbr.ReadSingle();
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Sort Priority: " + sortpriority, true);

                            int secondarySortIndex = decompbr.ReadUInt16();
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Secondary Sort Index: " + secondarySortIndex, true);

                            uint propertyid = decompbr.ReadUInt32();
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Property ID: " + propertyid, true);
                            
                            uint auralMaterialHash = decompbr.ReadUInt32();
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Aural Material Hash: " + auralMaterialHash.ToString("X8"), true);

                            if (version <= 42){
                                log.MakeLog("Version is <= 42: " + version, true);
                                int[] parameterFlag = new int[1];
                                parameterFlag[0] = (int)decompbr.ReadUInt16();
                                BitArray parameterFlags = new BitArray(parameterFlag);
                                log.MakeLog(parameterFlags.Length.ToString(), true);
                                for(int p = 0; p < 16; p++)
                                {
                                    if (parameterFlags[p] == true) {
                                        allFlags.Add(parameters[p]);
                                    }
                                    log.MakeLog("Function Sort Flag [" + p + "] is " + parameters[p] + " and is " + parameterFlags[p].ToString(), true);
                                } 
                                
                            } else if (version >= 43){
                                log.MakeLog("Version is >= 43: " + version, true);
                                int[] parameterFlag = new int[1];
                                parameterFlag[0] = (int)decompbr.ReadUInt16();
                                BitArray parameterFlags = new BitArray(parameterFlag);

                                for(int pfc = 0; pfc < 16; pfc++){
                                    if (parameterFlags[pfc] == true) {
                                        allFlags.Add(parameters[pfc]);
                                    }
                                    log.MakeLog("Function Sort Flag [" + pfc + "] is: " + parameters[pfc] + " and is " + parameterFlags[pfc].ToString(), true);
                                } 
                                
                            }
                                ulong excludePartFlags = decompbr.ReadUInt64();
                                log.MakeLog("P" + packageparsecount + "/E" + e + " - Exclude Part Flags: " + excludePartFlags.ToString("X16"), true);
                                ulong excludePartFlags2 = decompbr.ReadUInt64();
                                log.MakeLog("P" + packageparsecount + "/E" + e + " - Exclude Part Flags2: " + excludePartFlags2.ToString("X16"), true);
                                ulong excludeModifierRegionFlags = decompbr.ReadUInt64();
                                log.MakeLog("P" + packageparsecount + "/E" + e + " - Exclude Part Flags: " + excludeModifierRegionFlags.ToString("X16"), true);
                                uint count = decompbr.ReadByte();
                                decompbr.ReadByte();
                                CASTag tags = new CASTag(decompbr, count);
                                for (int i = 0; i < count; i++){
                                    log.MakeLog("P" + packageparsecount + "/E" + e + " - Function Sort Flag " + i + " value is: " + tags.tagKey[i], true);                                
                                    if (tags.tagKey[i] != 0){
                                        if (TypeListings.S4BBFunctionTags.Exists(x => x.typeID == tags.tagKey[i].ToString())){
                                            log.MakeLog("P" + packageparsecount + "/E" + e + " - TagKey " + tags.tagKey[i] + " exists in database.", true);
                                            foreach (typeList item in TypeListings.S4BBFunctionTags){
                                                if(item.typeID == tags.tagKey[i].ToString()){
                                                    if ((itemtags.Exists(x => x.shortval == (short)tags.tagKey[i])) || (itemtags.Exists(x => x.stringval == tags.tagKey[i].ToString()))){
                                                        
                                                    } else {
                                                        itemtags.Add(new TagsList{ shortval = (short)tags.tagKey[i], stringval = item.info});
                                                        log.MakeLog("Tag " + i + " matched to " + item.info, true);
                                                    }
                                                }
                                            }

                                        } else if (TypeListings.S4BBFunctionTags.Exists(x => x.typeID == tags.catKey[i].ToString())) {
                                            log.MakeLog("P" + packageparsecount + "/E" + e + " - CatKey " + tags.catKey[i] + " exists in database.", true);
                                            foreach (typeList item in TypeListings.S4BBFunctionTags){
                                                if(item.typeID == tags.catKey[i].ToString()){
                                                    if ((itemtags.Exists(x => x.shortval == (short)tags.catKey[i])) || (itemtags.Exists(x => x.stringval == tags.catKey[i].ToString()))){
                                                        
                                                    } else {
                                                        itemtags.Add(new TagsList{ shortval = (short)tags.catKey[i], stringval = item.info});
                                                        log.MakeLog("Tag " + i + " matched to " + item.info, true);
                                                    }
                                                }
                                            }

                                        } else {
                                            itemtags.Add(new TagsList{ shortval = (short)tags.tagKey[i], stringval = "Needs Identification"});
                                            TypeListings.S4BBFunctionTags.Add(new typeList{ typeID = tags.tagKey[i].ToString(), info = "Needs Identification" });
                                            log.MakeLog("Tag " + i + " has no match, adding it to json.", true);
                                            globals.UpdateBBTags();
                                        }
                                    }
                                }
                            /*
                            
                            
                            
                            
                            
                            uint secondaryDisplayIndex = decompbr.ReadUInt16();
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Secondary Display Index: " + secondaryDisplayIndex.ToString("X4"), true);
                            uint propertyID = decompbr.ReadUInt32();
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Property ID: " + propertyID.ToString("X8"), true);
                            
                            
                            testint = decompbr.ReadUInt32();
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Test: " + testint.ToString(), true);
                            uint count = decompbr.ReadUInt32();
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Tags Count: " + count, true);
                            Tag tags = new Tag(decompbr, count);
                            for (int i = 0; i < count; i++){
                                log.MakeLog("P" + packageparsecount + "/E" + e + " - Function Sort Flag " + i + " value is: " + tags.tagKey[i], true);                                
                                if (tags.tagKey[i] != 0){
                                    if (TypeListings.S4BBFunctionTags.Exists(x => x.typeID == tags.tagKey[i].ToString())) {
                                        foreach (typeList item in TypeListings.S4BBFunctionTags){
                                            if (item.typeID == tags.tagKey[i].ToString()){
                                                if ((itemtags.Exists(x => x.shortval == tags.tagKey[i])) || (itemtags.Exists(x => x.stringval == tags.tagKey[i].ToString()))){
                                                    //do nothing
                                                } else {
                                                    itemtags.Add(new TagsList{ shortval = (short)tags.tagKey[i], stringval = item.info});
                                                    log.MakeLog("Tag " + i + " matched to " + item.info, true);
                                                }
                                            }
                                        }
                                    } else {
                                        if ((itemtags.Exists(x => x.shortval == tags.tagKey[i])) || (itemtags.Exists(x => x.stringval == tags.tagKey[i].ToString()))){
                                            //do nothing
                                        } else {
                                            itemtags.Add(new TagsList{ shortval = (short)tags.tagKey[i], stringval = "Needs Identification"});
                                            TypeListings.S4BBFunctionTags.Add(new typeList{ typeID = tags.tagKey[i].ToString(), info = "Needs Identification" });
                                            log.MakeLog("Tag " + i + " has no match, adding it to json.", true);
                                            globals.UpdateBBTags();
                                        }
                                    }
                                } else {
                                    if ((itemtags.Exists(x => x.shortval == tags.tagKey[i])) || (itemtags.Exists(x => x.stringval == tags.tagKey[i].ToString()))){
                                        //do nothing
                                    } else {
                                        itemtags.Add(new TagsList{ shortval = (short)tags.tagKey[i]});
                                    }
                                }
                            }/*

                            / */    

                            /*
                                [7-6]    not_used
                                [5]      ShowInCasDemo
                                [4]      ShowInSimInfoPanel
                                [3]      ShowInUI
                                [2]      AllowForRandom
                                [1]      DefaultThumnailPart
                                [0]      DefaultForBodyType 
                            */

                                                 
                    }



                    caspc++;
                }         
                
                            

             

            }

            long location;

            if (fileHas.Exists(x => x.term == "COBJ")){
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
                    log.MakeLog("P" + packageparsecount + "/E" + e + " - Opening COBJ #" + cobjc, true);
                    if (indexData[e].compressionType == "5A42"){
                            dbpfFile.Seek(indexData[e].position, SeekOrigin.Begin);                            
                            int entryEnd = (int)readFile.BaseStream.Position + (int)indexData[e].memSize;
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Position: " + indexData[e].position, true);
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Filesize: " + indexData[e].fileSize, true);
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Memsize: " + indexData[e].memSize, true);
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Entry ends at " + entryEnd, true);
                            byte[] entry = readFile.ReadBytes((int)indexData[e].memSize);
                            Stream decomps = S4Decryption.Decompress(entry);
                            
                            BinaryReader decompbr = new BinaryReader(decomps);

                            
                            
                            
                            uint version = decompbr.ReadUInt32();
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Version: " + version, true);
                            log.MakeLog("-- As hex: " + version.ToString("X8"), true);
                            uint commonblockversion = decompbr.ReadUInt32();
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Common Block Version: " + commonblockversion, true);
                            log.MakeLog("-- As hex: " + commonblockversion.ToString("X8"), true);
                            uint namehash = decompbr.ReadUInt32();
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - NameHash: " + namehash, true);
                            log.MakeLog("-- As hex: " + namehash.ToString("X8"), true);
                            uint descriptionhash = decompbr.ReadUInt32();
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - DescriptionHash: " + descriptionhash, true);
                            log.MakeLog("-- As hex: " + descriptionhash.ToString("X8"), true);                            
                            uint price = decompbr.ReadUInt32();
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Price: " + price, true);
                            log.MakeLog("-- As hex: " + price.ToString("X8"), true);

                            ulong thumbhash = decompbr.ReadUInt64();
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Thumbnail Hash: " + thumbhash, true);
                            log.MakeLog("-- As hex: " + thumbhash.ToString("X8"), true);

                            uint devcatflags = decompbr.ReadUInt32();
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Dev Category Flags: " + devcatflags, true);
                            log.MakeLog("-- As hex: " + devcatflags.ToString("X8"), true);
                            
                            int tgicount = decompbr.ReadByte();
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - TGI Count: " + tgicount, true);

                            ResourceKeyITG resourcekey = new ResourceKeyITG(decompbr);
                            log.MakeLog(resourcekey.ToString(), true);
                            
                            if (commonblockversion >= 10)
                            {
                                int packId = decompbr.ReadInt16();
                                log.MakeLog("P" + packageparsecount + "/E" + e + " - Pack ID: " + packId, true);
                                int packFlags = decompbr.ReadByte();
                                log.MakeLog("P" + packageparsecount + "/E" + e + " - Pack Flags: " + packFlags, true);
                                byte[] reservedBytes = decompbr.ReadBytes(9);
                            } else {
                                int unused2 = decompbr.ReadByte();
                                if (unused2 > 0)
                                {
                                    int unused3 = decompbr.ReadByte();
                                }
                            }
                            
                            uint count = decompbr.ReadUInt32();
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Tags Count: " + count, true);
                            Tag tags = new Tag(decompbr, count);
                            for (int i = 0; i < count; i++){
                                log.MakeLog("P" + packageparsecount + "/E" + e + " - Tag " + i + " value is: " + tags.tagKey[i], true);
                                
                                if (tags.tagKey[i] != 0){
                                    if (TypeListings.S4BBFunctionTags.Exists(x => x.typeID == tags.tagKey[i].ToString())) {
                                        foreach (typeList item in TypeListings.S4BBFunctionTags){
                                            if (item.typeID == tags.tagKey[i].ToString()){
                                                if ((itemtags.Exists(x => x.shortval == tags.tagKey[i])) || (itemtags.Exists(x => x.stringval == tags.tagKey[i].ToString()))){

                                                } else {
                                                    itemtags.Add(new TagsList{ shortval = (short)tags.tagKey[i], stringval = item.info});
                                                    log.MakeLog("Tag " + i + " matched to " + item.info, true);
                                                }
                                                                                                
                                            }
                                        }
                                    } else {
                                        if ((itemtags.Exists(x => x.shortval == tags.tagKey[i])) || (itemtags.Exists(x => x.stringval == tags.tagKey[i].ToString()))){

                                        } else {
                                            itemtags.Add(new TagsList{ shortval = (short)tags.tagKey[i], stringval = "Needs Identification"});
                                            TypeListings.S4BBFunctionTags.Add(new typeList{ typeID = tags.tagKey[i].ToString(), info = "Needs Identification" });
                                            log.MakeLog("Tag " + i + " has no match, adding it to json.", true);
                                            globals.UpdateBBTags();
                                        }
                                    }
                                } else {
                                    if ((itemtags.Exists(x => x.shortval == tags.tagKey[i])) || (itemtags.Exists(x => x.stringval == tags.tagKey[i].ToString()))){

                                    } else {
                                        itemtags.Add(new TagsList{ shortval = (short)tags.tagKey[i]});
                                    }
                                }
                            }
                            location = decompbr.BaseStream.Position;
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Reader location: " + location, true);
                            count = decompbr.ReadUInt32();
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Selling Point Count: " + count, true);
                            Tag sellingtags = new Tag(decompbr, count);
                            for (int i = 0; i < count; i++){
                                log.MakeLog("P" + packageparsecount + "/E" + e + " - Tag " + i + " value is: " + sellingtags.tagKey[i], true);
                                
                                if (sellingtags.tagKey[i] != 0){
                                    if (TypeListings.S4BBFunctionTags.Exists(x => x.typeID == sellingtags.tagKey[i].ToString())) {
                                        foreach (typeList item in TypeListings.S4BBFunctionTags){
                                            if (item.typeID == sellingtags.tagKey[i].ToString()){
                                                if ((sellingpointlist.Exists(x => x.shortval == tags.tagKey[i])) || (sellingpointlist.Exists(x => x.stringval == tags.tagKey[i].ToString()))){

                                                } else {
                                                sellingpointlist.Add(new TagsList{ shortval = (short)sellingtags.tagKey[i], stringval = item.info});
                                                log.MakeLog("Tag " + i + " matched to " + item.info, true);
                                                }
                                            }
                                        }
                                    } else {
                                        if ((sellingpointlist.Exists(x => x.shortval == tags.tagKey[i])) || (sellingpointlist.Exists(x => x.stringval == tags.tagKey[i].ToString()))){

                                        } else {
                                            sellingpointlist.Add(new TagsList{ shortval = (short)sellingtags.tagKey[i], stringval = "Needs Identification"});
                                            TypeListings.S4BBFunctionTags.Add(new typeList{ typeID = sellingtags.tagKey[i].ToString(), info = "Needs Identification" });
                                            log.MakeLog("Tag " + i + " has no match, adding it to json.", true);
                                            globals.UpdateBBTags();
                                        }
                                    }
                                } else {
                                    if ((sellingpointlist.Exists(x => x.shortval == tags.tagKey[i])) || (sellingpointlist.Exists(x => x.stringval == tags.tagKey[i].ToString()))){

                                    } else {
                                        sellingpointlist.Add(new TagsList{ shortval = (short)sellingtags.tagKey[i]});
                                    }
                                }

                            }

                            uint unlockByHash = decompbr.ReadUInt32();
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - UnlockBy Hash: " + unlockByHash, true);
                            
                            uint unlockedByHash = decompbr.ReadUInt32();
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - UnlockedBy Hash: " + unlockedByHash, true);

                            int swatchColorSortPriority = decompbr.ReadUInt16();
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Swatch Sort Priority: " + swatchColorSortPriority, true);

                            ulong varientThumbImageHash = decompbr.ReadUInt64();
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Varient Thumb Image Hash: " + varientThumbImageHash, true);
                            
                            
                    }
                    cobjc++;
                }
            }

            if (fileHas.Exists(x => x.term == "OBJD")){
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
                    log.MakeLog("P" + packageparsecount + "/E" + e + " - Opening OBJD #" + objdc, true);
                    if (indexData[e].compressionType == "5A42"){
                            dbpfFile.Seek(indexData[e].position, SeekOrigin.Begin);                            
                            int entryEnd = (int)readFile.BaseStream.Position + (int)indexData[e].memSize;
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Position: " + indexData[e].position, true);
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Filesize: " + indexData[e].fileSize, true);
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Memsize: " + indexData[e].memSize, true);
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Entry ends at " + entryEnd, true);
                            byte[] entry = readFile.ReadBytes((int)indexData[e].memSize);
                            Stream decomps = S4Decryption.Decompress(entry);
                            
                            BinaryReader decompbr = new BinaryReader(decomps);

                            ReadOBJDIndex readOBJD = new ReadOBJDIndex(decompbr);
                            //log.MakeLog("There are " + readOBJD.count + " entries to read.", true);
                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Reader is at " + decompbr.BaseStream.Position, true);
                            log.MakeLog(readOBJD.entrytype[0].ToString("X8") + " is at " + readOBJD.position[0], true);
                            decompbr.BaseStream.Position = readOBJD.position[0];

                            log.MakeLog("P" + packageparsecount + "/E" + e + " - Reader is at " + decompbr.BaseStream.Position, true);
                            ReadOBJDEntry readobjdentry = new ReadOBJDEntry(decompbr);
                            thisPackage.Title = readobjdentry.name;
                            for (int c = 0; c < readobjdentry.componentcount; c++){
                                thisPackage.Components.Add(readobjdentry.components[c].ToString("X8"));
                            }
                            thisPackage.ObjectGUID.Add(readobjdentry.model);
                                                                                   
                            

                    }
                    objdc++;
                }
                
            }




            //everything after this works perfectly

            log.MakeLog("All methods complete, moving on to getting info.", true);
            /*log.MakeLog("Dirvar contains: " + dirvar.ToString(), true);
            log.MakeLog("Ctssvar contains: " + ctssvar.ToString(), true);
            log.MakeLog("Mmatvar contains: " + mmatvar.ToString(), true);
            log.MakeLog("Objdvar contains: " + objdvar.ToString(), true);
            log.MakeLog("Strvar contains: " + strvar.ToString(), true);*/

            List<TypeCounter> typecount = new List<TypeCounter>();
            var typeDict = new Dictionary<string, int>();

            log.MakeLog("Making dictionary.", true);

            foreach (fileHasList item in fileHas){
                foreach (typeList type in TypeListings.AllTypesS4){
                    if (type.desc == item.term){
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

            //ifs

            int casp;int geom;int rle;int rmap;int thum;int img;int xml;int clhd;int clip;int stbl;int cobj;int ftpt;int lite;int thm;int mlod;int modl;int mtbl;int objd;int rslt;int tmlt;int ssm;int lrle;

            if ((typeDict.TryGetValue("S4SM", out ssm) && ssm >= 1)){
                thisPackage.Type = "Merged Package";
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
            }  else if ((typeDict.TryGetValue("CASP", out casp) && casp >= 1) && (typeDict.TryGetValue("GEOM", out geom) && geom <= 0) && (typeDict.TryGetValue("RLE2", out rle) && rle >= 1) && (typeDict.TryGetValue("RMAP", out rmap) && rmap >= 1) && (typeDict.TryGetValue("_IMG", out img) && img >= 1)){
                thisPackage.Type = "Hair Recolor";
                thisPackage.Mesh = false;
                thisPackage.Recolor = true;
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);
            } else if ((typeDict.TryGetValue("CASP", out casp) && casp >= 1) && (typeDict.TryGetValue("GEOM", out geom) && geom >= 1) && (typeDict.TryGetValue("RLE2", out rle) && rle >= 1) && (typeDict.TryGetValue("RMAP", out rmap) && rmap >= 1) && (typeDict.TryGetValue("_IMG", out img) && img >= 1)){
                thisPackage.Type = "Hair";
                thisPackage.Mesh = true;
                thisPackage.Recolor = true;
                log.MakeLog("This is a " + thisPackage.Type + "!!", true);
            } else if ((typeDict.TryGetValue("_IMG", out img) && img >= 1) && (typeDict.TryGetValue("_XML", out xml) && xml >= 1) && (typeDict.TryGetValue("CLHD", out clhd) && clhd >= 1) && (typeDict.TryGetValue("CLIP", out clip) && clip >= 1) && (typeDict.TryGetValue("STBL", out stbl) && stbl >= 1)){
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
            


            distinctStyleTags = styletags.Distinct().ToList();
            thisPackage.StyleTags.AddRange(distinctStyleTags);

            distinctFlags = allFlags.Distinct().ToList();
            thisPackage.Flags.AddRange(distinctFlags);

            distinctItemTags = itemtags.Distinct().ToList();
            thisPackage.CatalogTags.AddRange(distinctItemTags);
            
            distinctSellingPointTags = sellingpointlist.Distinct().ToList();
            thisPackage.SellingPointTags.AddRange(distinctSellingPointTags);

            distinctInstanceIDs = allInstanceIDs.Distinct().ToList();
            thisPackage.InstanceIDs.AddRange(distinctInstanceIDs);


            distinctGUIDS = allGUIDS.Distinct().ToList();
            thisPackage.ObjectGUID.AddRange(distinctGUIDS);


            log.MakeLog("In thisPackage: " + thisPackage.ToString(), true);
            log.MakeLog(thisPackage.ToString(), false);
            Containers.Containers.allSims4Packages.Add(thisPackage);

            readFile.Close();
            Console.WriteLine("Closing Package #" + packageparsecount + "/" + GlobalVariables.PackageCount + ": " + packageinfo.Name);
            

            packageparsecount++;

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