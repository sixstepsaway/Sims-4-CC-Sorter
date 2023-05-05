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
using System.Runtime.CompilerServices;
using SSAGlobals;
using SimsCCManager.Packages.Containers;
using SimsCCManager.Packages.Decryption;

///disabled until i can work out how to do this myself >:(

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

    public class EntryHolder {
        public int[] entry {get; set;}
        public IReadOnlyList<int> header {get; set;}        
    }

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
    class S4PackageSearch
    {
        
        // References
        LoggingGlobals log = new LoggingGlobals();
        ReadEntries readentries = new ReadEntries();   

        //Vars        
        private int fields = 9;
        private int contentcount = 36;
        private int contentposition = 64;
        private int contentpositionalt = 40;
        private int headerSize = 96;
        byte[] header = new byte[96];
        List<EntryHolder> entries = new List<EntryHolder>();

        static byte[] stringToBytes(string s) { byte[] bytes = new byte[s.Length]; int i = 0; foreach (char c in s) bytes[i++] = (byte)c; return bytes; }
        void setIndexcount(BinaryWriter w, int c) { w.BaseStream.Position = 36; w.Write(c); }
        void setIndexsize(BinaryWriter w, int c) { w.BaseStream.Position = 44; w.Write(c); }
        void setIndexversion(BinaryWriter w) { w.BaseStream.Position = 60; w.Write(3); }
        void setIndexposition(BinaryWriter w, int c) { w.BaseStream.Position = 40; w.Write((int)0); w.BaseStream.Position = 64; w.Write(c); }
        void setUnused4(BinaryWriter w, int c) { w.BaseStream.Position = 0x3c; w.Write(c); }     
        public void SearchS4Packages(string file) {
            var packageparsecount = GlobalVariables.packagesRead;   
            GlobalVariables.packagesRead++;         
            //Vars for Package Info
            string typefound = "";
            string instanceID2;
            string typeID;
            string groupID;
            string instanceID;
            uint compfilesize;
            uint numRecords;
            string cTypeID;
            int cFileSize;  
            uint myFilesize;
            string magic = "";
            int major = 0;
            int minor = 0;
            int unused4 = 0;
            
        
            //Misc Vars
            string test = "";        
            int dirnum = 0;
            List<int> objdnum = new List<int>();   
            List<int> strnm = new List<int>();  
            List<int> imgnm = new List<int>();
            int mmatloc = 0;

            SimsPackage thisPackage = new SimsPackage();
            SimsPackage infovar = new SimsPackage();
            SimsPackage ctssvar = new SimsPackage();
            SimsPackage dirvar = new SimsPackage();
            SimsPackage strvar = new SimsPackage();
            SimsPackage objdvar = new SimsPackage();
            SimsPackage mmatvar = new SimsPackage();
            

            //Lists 
            
            entries = new List<EntryHolder>();
            List<fileHasList> fileHas = new List<fileHasList>();
            ArrayList linkData = new ArrayList();
            List<indexEntry> indexData = new List<indexEntry>();
            FileInfo packageinfo = new FileInfo(file); 
            //List<string> iids = new List<string>();
            List<string> allGUIDS = new List<string>();      
            List<string> distinctGUIDS = new List<string>();  
            List<string> allInstanceIDs = new List<string>();      
            List<string> distinctInstanceIDs = new List<string>();  

            //create readers  
            FileStream dbpfFile = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader readFile = new BinaryReader(dbpfFile);    
            Byte[] allbytes = File.ReadAllBytes(file);
            MemoryStream ms = new MemoryStream(allbytes);       

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

            //unused but 3 for historical reasons
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unused Trash Size: " + test, true);

           //unused but 3 for historical reasons
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unused, 3 for historical reasons: " + test, true);
            
            ulong indexRecordPosition = readFile.ReadUInt64();
            test = indexRecordPosition.ToString();
            log.MakeLog("Index Record Position: " + test, true);

                        
            //unused
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Garbage: " + test, true);


            //unused six bytes
            test = Encoding.ASCII.GetString(readFile.ReadBytes(24));
            log.MakeLog("Unused: " + test, true);

            if ((long)indexRecordPosition != 0){
                dbpfFile.Seek((long)indexRecordPosition, SeekOrigin.Current);
                 } else {
                dbpfFile.Seek((long)indexRecordPositionLow, SeekOrigin.Current);
            }
            
            //dont know what this is
            
            for (int i = 0; i < entrycount; i++){
                indexEntry holderEntry = new indexEntry();

                holderEntry.typeID = readFile.ReadUInt32().ToString("X8");
                log.MakeLog("P" + packageparsecount + " - Index Entry TypeID: " + holderEntry.typeID, true);

                holderEntry.groupID = readFile.ReadUInt32().ToString("X8");
                log.MakeLog("P" + packageparsecount + " - Index Entry GroupID: " + holderEntry.groupID, true);

                holderEntry.instanceID = readFile.ReadUInt64().ToString("X16");
                allInstanceIDs.Add(holderEntry.instanceID.ToString());
                log.MakeLog("P" + packageparsecount + " - InstanceID: " + holderEntry.instanceID, true);
                
                testint = readFile.ReadUInt32();
                log.MakeLog("Position " + testint, true);
                testint = readFile.ReadUInt32();
                log.MakeLog("Size " + testint, true);    
                test = readFile.ReadUInt16().ToString("X4");          
                log.MakeLog("Compression Type: " + test, true);                
                test = readFile.ReadUInt16().ToString("X4");
                log.MakeLog("Committed: " + test, true);
                testint = readFile.ReadUInt32();
                log.MakeLog("Size Decompressed " + testint, true);

                log.MakeLog("Entry Count is " + i, true);

                packageparsecount++;
                
                
                
                
            }

            
            

            
        }        
    }
}