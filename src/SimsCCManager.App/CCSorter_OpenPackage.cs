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

namespace OpenPackage {

    class Package 
    {
        public int gameVer;

        public Package()
        {

        }

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
            //dbpfFile.Seek(this.chunkOffset + indexOffset, SeekOrigin.Begin);

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
    }
    
}
