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
using SimsCCManager.Decryption.EndianDecoding;

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
            FileStream trayfile = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader readTray = new BinaryReader(trayfile);

            uint groupid = readTray.ReadUInt32();
            log.MakeLog("Group ID: " + groupid.ToString("X8"), true);
            //readTray.ReadUInt32();
            readTray.ReadUInt16();
            for (int t = 0; t < 5000; t++){
                log.MakeLog("Norm: " + readTray.ReadUInt32(), true);
                log.MakeLog("HEX: " + readTray.ReadUInt32().ToString("X8"), true);
            }





            Console.WriteLine("Closing: " + file.Name);
        }
    }
}