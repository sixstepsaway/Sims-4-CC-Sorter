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
using SimsCCManager.Packages.Initial;


namespace SimsCCManager.Search_All
{
    public class Packages_Search_All
    {
        // References
        LoggingGlobals log = new LoggingGlobals();
        ReadEntries readentries = new ReadEntries();   
        ParallelOptions parallelSettings = new ParallelOptions() { MaxDegreeOfParallelism = 200};
        InitialProcessing initialProcessing = new InitialProcessing();

        //Vars
        uint chunkOffset = 0;       
        int gameVer = 0; 

        
        public int PackageVersion(FileInfo file)
        {
            FileStream dbpfFile = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader readFile = new BinaryReader(dbpfFile);
            string dbpf = Encoding.ASCII.GetString(readFile.ReadBytes(4));
            uint major = readFile.ReadUInt32();
            uint minor = readFile.ReadUInt32();
            string reserved = Encoding.UTF8.GetString(readFile.ReadBytes(12));
            uint dateCreated = readFile.ReadUInt32();
            uint dateModified = readFile.ReadUInt32();
            uint indexMajorVersion = readFile.ReadUInt32();
            uint indexCount = readFile.ReadUInt32();
            uint indexOffset = readFile.ReadUInt32();
            uint indexSize = readFile.ReadUInt32();
            uint holesCount = readFile.ReadUInt32();
            uint holesOffset = readFile.ReadUInt32();
            uint holesSize = readFile.ReadUInt32();
            string reserved2 = Encoding.UTF8.GetString(readFile.ReadBytes(32));

            /* GAME VERSION NUMBERS ARE:

                0 = null,
                1 = Sims 1,
                2 = Sims 2,
                3 = Sims 3,
                4 = Sims 4,
                11 = Spore,
                12 = SimCity 5*/

            if (major is 1 && minor is 1) {
                gameVer = 2;               
            } else if (major is 2 && minor is 1) {
                gameVer = 4;               
            } else if (major is 2 && minor is 0) {
                gameVer = 3;
            } else if (major is 3 && minor is 0) {
                gameVer = 12;
            }

            return gameVer;
        }
        public void FindPackagesToRemove(){
            int packageCount = GlobalVariables.PackageFiles.Count;
            var statement = "Checking " + packageCount + " package files. Larger numbers will take a while and I don't know how to do progress bars yet, so please be patient.";
            log.MakeLog(statement, false);
            foreach (FileInfo item in GlobalVariables.PackageFiles) {
                statement = "Processing " + item.Name + ".";
                log.MakeLog(statement, true);
                initialProcessing.FindBrokenPackages(item);
                initialProcessing.IdentifyGames(item);
            }
            statement = "Checked all package files.";
            log.MakeLog(statement, false);
        }    

    }
}