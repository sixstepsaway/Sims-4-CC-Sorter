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
using SSAGlobals;
using SimsCCManager.Packages.Containers;
using SimsCCManager.Packages.Decryption;
using System.Data.SQLite;

namespace SimsCCManager.Misc.TrayReader
{
    public struct ReadTrayItems{
        /*uint groupid;
        ulong 
        public ReadTrayItems(BinaryReader reader){

        }*/

    }
    public class TrayFilesReader
    {
        LoggingGlobals log = new LoggingGlobals();
        
        public void ReadTray(FileInfo file){
            //sgi = sim gallery image
            //hhi = household image
            //trayitem = ... well, what it says on the tin
            //householdbinary is the one we want!

            
            

            Console.WriteLine("Opening: " + file.Name);
            MemoryStream trayfile = Methods.ReadBytesToFile(file.FullName);
            BinaryReader readTray = new BinaryReader(trayfile);
            string instanceid1 = "";
            string instanceid2 = "";
            string fullinstance = "";
            long sim1instance = 46;
            long ninacc01 = 2208;



            /*readTray.BaseStream.Position = sim1instance;
            //instanceid1 = (readTray.ReadUInt32() << 32).ToString("X8");
            //instanceid2 = (readTray.ReadUInt32() << 32).ToString("X8");
            fullinstance = readTray.ReadUInt64().ToString("X16");

            log.MakeLog(string.Format("Sim Instance: {0}", fullinstance), true);

            for (int t = 0; t < ((readTray.BaseStream.Length / 8) - readTray.BaseStream.Position); t++){
                ulong item = readTray.ReadUInt64();
                log.MakeLog(string.Format("After Sim1Inst: Norm: {0} at {1}", item, readTray.BaseStream.Position), true);
                log.MakeLog(string.Format("After Sim1Inst: {0} at {1}", item.ToString("X8"), readTray.BaseStream.Position), true);
                var result = TypeListings.AllTypesS4.Where(x => x.typeID == item.ToString("X8"));
                if (result.Any()){
                    log.MakeLog(string.Format("After Sim1Inst: Found {0} at position {1}!", item, readTray.BaseStream.Position), true);
                }
            }

            readTray.BaseStream.Position = ninacc01;            
            fullinstance = readTray.ReadUInt64().ToString("X16");

            log.MakeLog(string.Format("NinaCC01: {0}", fullinstance), true);

            

            for (int t = 0; t < ((readTray.BaseStream.Length / 8) - readTray.BaseStream.Position); t++){
                ulong item = readTray.ReadUInt64();
                log.MakeLog(string.Format("After NinaAcc01: Norm: {0} at {1}", item, readTray.BaseStream.Position), true);
                log.MakeLog(string.Format("After NinaAcc01: {0} at {1}", item.ToString("X8"), readTray.BaseStream.Position), true);
                var result = TypeListings.AllTypesS4.Where(x => x.typeID == item.ToString("X8"));
                if (result.Any()){
                    log.MakeLog(string.Format("After NinaAcc01: Found {0} at position {1}!", item, readTray.BaseStream.Position), true);
                }
            }*/




            readTray.BaseStream.Position = 0;            
            for (int t = 0; t < (readTray.BaseStream.Length / 8); t++){
                ulong item = readTray.ReadUInt64();
                log.MakeLog(string.Format("Pass 1: Norm: {0} at {1}", item, readTray.BaseStream.Position), true);
                log.MakeLog(string.Format("Pass 1: Hex: {0} at {1}", item.ToString("X8"), readTray.BaseStream.Position), true);
                var result = TypeListings.AllTypesS4.Where(x => x.typeID == item.ToString("X8"));
                if (result.Any()){
                    log.MakeLog(string.Format("Pass 1: Found {0} at position {1}!", item, readTray.BaseStream.Position), true);
                }
                string cs = string.Format("Data Source={0}", GlobalVariables.S4_Overrides_All);                   
                using (var dataConnection = new SQLiteConnection(cs)){                    
                    try
                    {
                        dataConnection.Open();
                        string cmdtxt = string.Format("SELECT * FROM Instances WHERE InstanceID = '{0}'", item.ToString("X8"));
                        using (SQLiteCommand sqcmd = new SQLiteCommand(cmdtxt, dataConnection)){
                            using (SQLiteDataReader sqlreader = sqcmd.ExecuteReader()){
                                while (sqlreader.Read())
                                {
                                    string idm = sqlreader["InstanceID"].ToString();
                                    string ow = sqlreader["Name"].ToString();
                                    string pack = sqlreader["Pack"].ToString();
                                    string tp = sqlreader["Type"].ToString();
                                    if (!String.IsNullOrWhiteSpace(idm)){
                                        log.MakeLog(string.Format("Found match: {0} belongs to {1} and is {2}.", idm, pack, ow), true);
                                    }
                                }
                            }
                        }        
                    }
                    finally
                    {
                        dataConnection.Close();
                    }                    
                }
            }
            readTray.BaseStream.Position = 2;
            for (int t = 0; t < ((readTray.BaseStream.Length / 8) - 2); t++){
                ulong item = readTray.ReadUInt64();
                log.MakeLog(string.Format("Pass 2: Norm: {0} at {1}", item, readTray.BaseStream.Position), true);
                log.MakeLog(string.Format("Pass 2: Hex: {0} at {1}", item.ToString("X8"), readTray.BaseStream.Position), true);
                var result = TypeListings.AllTypesS4.Where(x => x.typeID == item.ToString("X8"));
                if (result.Any()){
                    log.MakeLog(string.Format("Pass 2: Found {0} at position {1}!", item, readTray.BaseStream.Position), true);
                }
                string cs = string.Format("Data Source={0}", GlobalVariables.S4_Overrides_All);                   
                using (var dataConnection = new SQLiteConnection(cs)){                    
                    try
                    {
                        dataConnection.Open();
                        string cmdtxt = string.Format("SELECT * FROM Instances WHERE InstanceID = '{0}'", item.ToString("X8"));
                        using (SQLiteCommand sqcmd = new SQLiteCommand(cmdtxt, dataConnection)){
                            using (SQLiteDataReader sqlreader = sqcmd.ExecuteReader()){
                                while (sqlreader.Read())
                                {
                                    string idm = sqlreader["InstanceID"].ToString();
                                    string ow = sqlreader["Name"].ToString();
                                    string pack = sqlreader["Pack"].ToString();
                                    string tp = sqlreader["Type"].ToString();
                                    if (!String.IsNullOrWhiteSpace(idm)){
                                        log.MakeLog(string.Format("Found match: {0} belongs to {1} and is {2}.", idm, pack, ow), true);
                                    }
                                }
                            }
                        }        
                    }
                    finally
                    {
                        dataConnection.Close();
                    }                    
                }
            }
            readTray.BaseStream.Position = 4;
            for (int t = 0; t < ((readTray.BaseStream.Length / 8) - 4); t++){
                ulong item = readTray.ReadUInt64();
                log.MakeLog(string.Format("Pass 3: Norm: {0} at {1}", item, readTray.BaseStream.Position), true);
                log.MakeLog(string.Format("Pass 3: Hex: {0} at {1}", item.ToString("X8"), readTray.BaseStream.Position), true);
                var result = TypeListings.AllTypesS4.Where(x => x.typeID == item.ToString("X8"));
                if (result.Any()){
                    log.MakeLog(string.Format("Pass 3: Found {0} at position {1}!", item, readTray.BaseStream.Position), true);
                }
                string cs = string.Format("Data Source={0}", GlobalVariables.S4_Overrides_All);                   
                using (var dataConnection = new SQLiteConnection(cs)){                    
                    try
                    {
                        dataConnection.Open();
                        string cmdtxt = string.Format("SELECT * FROM Instances WHERE InstanceID = '{0}'", item.ToString("X8"));
                        using (SQLiteCommand sqcmd = new SQLiteCommand(cmdtxt, dataConnection)){
                            using (SQLiteDataReader sqlreader = sqcmd.ExecuteReader()){
                                while (sqlreader.Read())
                                {
                                    string idm = sqlreader["InstanceID"].ToString();
                                    string ow = sqlreader["Name"].ToString();
                                    string pack = sqlreader["Pack"].ToString();
                                    string tp = sqlreader["Type"].ToString();
                                    if (!String.IsNullOrWhiteSpace(idm)){
                                        log.MakeLog(string.Format("Found match: {0} belongs to {1} and is {2}.", idm, pack, ow), true);
                                    }
                                }
                            }
                        }        
                    }
                    finally
                    {
                        dataConnection.Close();
                    }                    
                }
            }
            readTray.BaseStream.Position = 6;
            for (int t = 0; t < ((readTray.BaseStream.Length / 8) - 6); t++){
                ulong item = readTray.ReadUInt64();
                log.MakeLog(string.Format("Pass 4: Norm: {0} at {1}", item, readTray.BaseStream.Position), true);
                log.MakeLog(string.Format("Pass 4: Hex: {0} at {1}", item.ToString("X8"), readTray.BaseStream.Position), true);
                var result = TypeListings.AllTypesS4.Where(x => x.typeID == item.ToString("X8"));
                if (result.Any()){
                    log.MakeLog(string.Format("Pass 4: Found {0} at position {1}!", item, readTray.BaseStream.Position), true);
                }
                string cs = string.Format("Data Source={0}", GlobalVariables.S4_Overrides_All);                   
                using (var dataConnection = new SQLiteConnection(cs)){                    
                    try
                    {
                        dataConnection.Open();
                        string cmdtxt = string.Format("SELECT * FROM Instances WHERE InstanceID = '{0}'", item.ToString("X8"));
                        using (SQLiteCommand sqcmd = new SQLiteCommand(cmdtxt, dataConnection)){
                            using (SQLiteDataReader sqlreader = sqcmd.ExecuteReader()){
                                while (sqlreader.Read())
                                {
                                    string idm = sqlreader["InstanceID"].ToString();
                                    string ow = sqlreader["Name"].ToString();
                                    string pack = sqlreader["Pack"].ToString();
                                    string tp = sqlreader["Type"].ToString();
                                    if (!String.IsNullOrWhiteSpace(idm)){
                                        log.MakeLog(string.Format("Found match: {0} belongs to {1} and is {2}.", idm, pack, ow), true);
                                    }
                                }
                            }
                        }        
                    }
                    finally
                    {
                        dataConnection.Close();
                    }                    
                }
            }





            //uint groupid = readTray.ReadUInt32();
            //log.MakeLog("Group ID: " + groupid.ToString("X8"), true);
            //readTray.ReadUInt32();
            //readTray.ReadUInt16();
            





            Console.WriteLine("Closing: " + file.Name);
        }
    }
}