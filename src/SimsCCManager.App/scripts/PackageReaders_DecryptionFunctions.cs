using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace SimsCCManager.PackageReaders.DecryptionFunctions
{
    public class ByteReaders{
        public static MemoryStream ReadBytesFromFile(string file){
            FileInfo f = new FileInfo(file);
            byte[] bit = new byte[f.Length];
			using (FileStream fsSource = new FileStream(file,
            FileMode.Open, FileAccess.Read))
			{
				for (int w = 0; w < f.Length; w++){
                    bit[w] = (byte)fsSource.ReadByte();
                }                
				MemoryStream stream = new MemoryStream(bit);
				return stream;
			}
		}

		public static MemoryStream ReadBytesFromFile(string file, int bytestoread){
			byte[] bit = new byte[bytestoread];
			using (FileStream fsSource = new FileStream(file,
            FileMode.Open, FileAccess.Read))
			{
				for (int w = 0; w < bytestoread; w++){
                    bit[w] = (byte)fsSource.ReadByte();
                }
				MemoryStream stream = new MemoryStream(bit);
				return stream;
			}
		}

        public static byte[] ReadEntryBytes(BinaryReader reader, int memSize){
            byte[] bit = new byte[memSize];
            for (int w = 0; w < memSize; w++){
                bit[w] = reader.ReadByte();
            }
            return bit;
        }
    }
    
    public class Sims4Decryption
    {
        public static Stream Decompress(byte[] data)
		{
			var outputStream = new MemoryStream();
			using (var compressedStream = new MemoryStream(data))
			using (var inputStream = new InflaterInputStream(compressedStream))
			{
				inputStream.CopyTo(outputStream);
				outputStream.Position = 0;
				return outputStream;
			}
		}

		public static MemoryStream DecompressMS(byte[] data)
		{
			var outputStream = new MemoryStream();
			using (var compressedStream = new MemoryStream(data))
			using (var inputStream = new InflaterInputStream(compressedStream))
			{
				inputStream.CopyTo(outputStream);
				outputStream.Position = 0;
				return outputStream;
			}
		}

		public static string CompressByte(byte[] data){			

			string inputStr = Convert.ToBase64String(data);
			byte[] inputBytes = Encoding.UTF8.GetBytes(inputStr);
			
			using (var outputStream = new MemoryStream())
			{
				using (var gZipStream = new GZipStream(outputStream, CompressionMode.Compress))
					gZipStream.Write(inputBytes, 0, inputBytes.Length);
			
				var outputBytes = outputStream.ToArray();
			
				var outputbase64 = Convert.ToBase64String(outputBytes);
				return outputbase64;
			}

		}

		public static byte[] DecompressByte(string data)
		{
			byte[] inputBytes = Convert.FromBase64String(data);
			
			using (var inputStream = new MemoryStream(inputBytes))
			using (var gZipStream = new GZipStream(inputStream, CompressionMode.Decompress))
			using (var streamReader = new StreamReader(gZipStream))
			{
				var decompressed = streamReader.ReadToEnd();
				var output = Convert.FromBase64String(decompressed);
				return output;			
			}

		}
    }
}