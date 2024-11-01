using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SimsCCManager.Globals;

namespace SimsCCManager.PackageReaders.ImageTransformations
{
    internal struct MipHeader
    {
        public int CommandOffset;
        public int Offset2;
        public int Offset3;
        public int Offset0;
        public int Offset1;
        public int Offset4;

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}, {4}, {5}",
                                 this.CommandOffset,
                                 this.Offset2,
                                 this.Offset3,
                                 this.Offset0,
                                 this.Offset1,
                                 this.Offset4);
        }
    }
    public class TransformImages
    {
        /*public Bitmap LRLE(BinaryReader readFile, string LogMessage, StringBuilder LogFile, LoggingGlobals log){
            Bitmap bmp = new Bitmap(1, 1);
            var magic = readFile.ReadUInt32();
            var version = readFile.ReadUInt32();
            var width = readFile.ReadUInt16();
            var height = readFile.ReadUInt16();
            var mipCount = readFile.ReadUInt16();
            var unk = readFile.ReadUInt16();
            var mipHeaders = new MipHeader[mipCount + 1];            
            for (var i = 0; i < mipCount; i++)
            {
                mipHeaders[i] = new MipHeader
                {
                    CommandOffset = readFile.ReadInt32(),
                    Offset2 = readFile.ReadInt32(),
                    Offset3 = readFile.ReadInt32(),
                    Offset0 = readFile.ReadInt32(),
                    Offset1 = readFile.ReadInt32(),
                };
            }
            mipHeaders[mipCount] = new MipHeader
            {
                CommandOffset = mipHeaders[0].Offset2,
                Offset2 = mipHeaders[0].Offset3,
                Offset3 = mipHeaders[0].Offset0,
                Offset0 = mipHeaders[0].Offset1,
                Offset1 = (int)readFile.BaseStream.Length,
            };

            readFile.BaseStream.Position = 0;
            var temp = readFile.ReadBytes((int)readFile.BaseStream.Length);
            bmp = TransformToPNG(temp);
            return bmp;
        }     */   
        
        public Bitmap TransformToPNG(byte[] rawData, string packageName)
        {
            string alpha = Path.Combine(GlobalVariables.tempfolder, string.Format("{0}_Alpha.png", packageName));
            string thum = Path.Combine(GlobalVariables.tempfolder, string.Format("{0}_Thumb.png", packageName));
            string output = Path.Combine(GlobalVariables.tempfolder, string.Format("{0}_Output.png", packageName));
            using (MemoryStream ms = new MemoryStream(rawData))
            {
                BinaryReader r = new BinaryReader(ms);
                Bitmap colorImage;
                ms.Position = 0;
                r.ReadBytes(24);
                if (r.ReadUInt32() == 0x41464C41U)
                {
                    int length = r.ReadInt32();
                    length = (int)((length & 0xFF000000) >> 24) | (int)((length & 0x00FF0000) >> 8) | (int)((length & 0x0000FF00) << 8) | (int)((length & 0x000000FF) << 24);
                    using (MemoryStream alphaStream = new MemoryStream(r.ReadBytes(length)))
                    {
                        Bitmap alphaImage = new Bitmap(alphaStream);
                        colorImage = new Bitmap(ms);
                        if (colorImage.Width != alphaImage.Width || colorImage.Height != alphaImage.Height) throw new InvalidDataException("Not a proper TS4 Thumbnail image");
                        
                        Bitmap ci = colorImage;  
                        if (!Directory.Exists(GlobalVariables.tempfolder)){
                            Directory.CreateDirectory(GlobalVariables.tempfolder);
                        }

                        

                        ci.Save(thum);
                        alphaImage.Save(alpha);    
                        string param = string.Format("\"{0}\" \"{1}\" -alpha off -compose CopyOpacity -composite \"{2}\"", thum, alpha, output);
                        Utilities.RunNonSimsProcess(GlobalVariables.imagemagick, param);
                        File.Delete(thum);
                        File.Delete(alpha);
                        return ci;
                    }
                }
                ms.Position = 0;
                colorImage = new Bitmap(ms);
                Bitmap bp = colorImage;
                return bp;
            }
        }        
    }
}