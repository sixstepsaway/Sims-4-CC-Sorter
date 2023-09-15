using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SSA.VirtualFileSystem
{
    public class VFileSystem
    {
        //https://stackoverflow.com/questions/11777924/how-to-make-a-read-only-file
        //https://stackoverflow.com/questions/1199571/how-to-hide-file-in-c
        //https://learn.microsoft.com/en-us/dotnet/api/system.io.file.createsymboliclink?view=net-7.0
        //https://stackoverflow.com/questions/3387690/how-to-create-a-hardlink-in-c
        //https://github.com/usdAG/SharpLink   
        
        public void ClearSymlinks(string folder){
            List<string> folders = Directory.GetDirectories(folder).ToList();
            List<string> files = Directory.GetFiles(folder).ToList();
            if (IsDirectory(folder)){
                DirectoryInfo directoryInfo = new DirectoryInfo(folder);
                if (directoryInfo.GetDirectories().Length != 0){
                    files = Directory.GetDirectories(folder).ToList();
                }
                if (directoryInfo.GetFiles().Length != 0){
                    if (files.Count != 0){
                        var f = Directory.GetFiles(folder).ToList();
                        files.AddRange(f);
                    }                    
                }
                
                foreach (string file in files){
                    FileInfo fileInfo = new(file);
                    FileAttributes attr = fileInfo.Attributes;
                    if(attr.HasFlag(FileAttributes.ReparsePoint)){
                        RemoveSymbolicLink(file);
                    }
                }
            }            
        }
        
        public void MakeSymbolicLink(string Original, string Destination){
            FileInfo fileInfo = new(Original);
            Destination = Path.Combine(Destination, fileInfo.Name);
            if (File.Exists(Destination)){
                File.Move(Destination, string.Format("{0}.disabled", Destination));
            }
            File.CreateSymbolicLink(Destination, Original);
        }

        public void MakeSymbolicLink(string Original, string Destination, string AsName){
            FileInfo fileInfo = new(Original);
            Destination = Path.Combine(Destination, AsName);
            if (File.Exists(Destination)){
                File.Move(Destination, string.Format("{0}.disabled", Destination));
            }
            File.CreateSymbolicLink(Destination, Original);
        }

        public void RemoveSymbolicLink(string Item){
            if (File.Exists(Item)){
                File.Delete(Item);
            }            
            if (File.Exists(string.Format("{0}.disabled", Item))){
                string og = string.Format("{0}.disabled", Item);
                string ren = og.Replace(".disabled", "");
                File.Move(og, ren);
            }
        }

        public void MakeJunction(string Original, string Destination){
            DirectoryInfo directoryInfo = new(Original);
            Destination = Path.Combine(Destination, directoryInfo.Name);
            if (Directory.Exists(Destination)){
                Directory.Move(Destination, string.Format("{0}--DISABLED", Destination));
            }
            Directory.CreateSymbolicLink(Destination, Original);
        }

        public void RemoveJunction(string Item){
            if (Directory.Exists(Item)){
                Directory.Delete(Item);
            }            
            if (Directory.Exists(string.Format("{0}--DISABLED", Item))){
                string og = string.Format("{0}--DISABLED", Item);
                string ren = og.Replace("--DISABLED", "");
                File.Move(og, ren);
            }
        }

        private static bool IsDirectory(string path){
            FileAttributes attributes = File.GetAttributes(path);
            if (attributes.HasFlag(FileAttributes.Directory)){
                return true;
            } else {
                return false;
            }
        }
    }
}