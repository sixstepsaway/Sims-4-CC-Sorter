using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Text;
using SimsCCManager.Debugging;
using Godot;
using SimsCCManager.Globals;

namespace SSA.VirtualFileSystem
{
    public class VFileSystem
    {
        //https://stackoverflow.com/questions/11777924/how-to-make-a-read-only-file
        //https://stackoverflow.com/questions/1199571/how-to-hide-file-in-c
        //https://learn.microsoft.com/en-us/dotnet/api/system.io.file.createsymboliclink?view=net-7.0
        //https://stackoverflow.com/questions/3387690/how-to-create-a-hardlink-in-c
        //https://github.com/usdAG/SharpLink  
         
        
        public static void MakeSymbolicLink(string Original, string Destination){
            FileInfo fileInfo = new(Original);
            FileInfo destinfo = new(Destination);
            Destination = Path.Combine(Destination, fileInfo.Name);
            if (File.Exists(Destination)){
                try { 
                    string n = string.Format("{0}.disabled", Destination);
                    File.Move(Destination, n);                     
                } catch (Exception e) {
                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Caught exception disabling duplicate file: {0}\nException: {1}", fileInfo.Name, e.Message));
                }
            }
            try {
                File.CreateSymbolicLink(Destination, Original);                 
            } catch (Exception e) {
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Caught exception making symbolic link: {0}\nException: {1}", fileInfo.Name, e.Message));
            }            
        }
        
        public static void MakeDirectSymbolicLink(string Original, string Destination){
            FileInfo fileInfo = new(Original);
            FileInfo destinfo = new(Destination);
            //Destination = Path.Combine(Destination, fileInfo.Name);
            if (File.Exists(Destination)){
                try { 
                    string n = string.Format("{0}.disabled", Destination);
                    File.Move(Destination, n);                     
                } catch (Exception e) {
                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Caught exception disabling duplicate file: {0}\nException: {1}", fileInfo.Name, e.Message));
                }
            }
            try {
                File.CreateSymbolicLink(Destination, Original);                 
            } catch (Exception e) {
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Caught exception making symbolic link: {0}\nException: {1}", fileInfo.Name, e.Message));
            }            
        }

        public static void MakeSymbolicLink(string Original, string Destination, string AsName){
            FileInfo destinfo = new(Destination);
            FileInfo fileInfo = new(Original);
            Destination = Path.Combine(Destination, AsName);
            if (File.Exists(Destination)){
                try {
                    string n = string.Format("{0}.disabled", Destination);
                    File.Move(Destination, n);                    
                } catch (Exception e) {
                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Caught exception disabling duplicate file: {0}\nException: {1}", fileInfo.Name, e.Message));
                }
            }
            try {
                File.CreateSymbolicLink(Destination, Original);                
            } catch (Exception e) {
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Caught exception making symbolic link: {0}\nException: {1}", fileInfo.Name, e.Message));
            }
            
        }

        public static void RemoveSymbolicLink(string Item){
            if (File.Exists(Item)){
                try { 
                    File.Delete(Item); 
                } catch (Exception e) {
                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Caught exception deleting symbolic link: {0}\nException: {1}", Item, e.Message));
                }
            }
            if (File.Exists(string.Format("{0}.disabled", Item))){
                string og = string.Format("{0}.disabled", Item);
                string ren = og.Replace(".disabled", "");
                try {
                    File.Move(og, ren);
                } catch (Exception e) {
                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Caught exception renaming disabled file: {0}\nException: {1}", Item, e.Message));
                }
            }
        }

        public static void MakeJunction(string Original, string Destination){  
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Creating junction from {0} to {1}", Destination, Original));
            DirectoryInfo destinfo = new(Destination);
            DirectoryInfo directoryInfo = new(Original);
            Destination = Path.Combine(Destination, directoryInfo.Name);
            if (Directory.Exists(Destination)){
                try { 
                    Directory.Move(Destination, string.Format("{0}--DISABLED", Destination)); 
                } catch (Exception e) {
                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Caught exception disabling duplicate folder: {0}\nException: {1}", directoryInfo.Name, e.Message));
                }
            }
            try {
                Directory.CreateSymbolicLink(Destination, Original);
            } catch (Exception e) {  
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("EXCEPTION: Caught exception making junction: {0}\nException: {1}", directoryInfo.Name, e.Message));                      
            }
        }

        public static void MakeDirectJunction(string Original, string Destination){  
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Creating junction from {0} to {1}", Destination, Original));
            DirectoryInfo destinfo = new(Destination);
            DirectoryInfo directoryInfo = new(Original);
            //Destination = Path.Combine(Destination, directoryInfo.Name);
            if (Directory.Exists(Destination)){
                try { 
                    Directory.Move(Destination, string.Format("{0}--DISABLED", Destination)); 
                } catch (Exception e) {
                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Caught exception disabling duplicate folder: {0}\nException: {1}", directoryInfo.Name, e.Message));
                }
            }
            try {
                Directory.CreateSymbolicLink(Destination, Original);
            } catch (Exception e) {  
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("EXCEPTION: Caught exception making junction: {0}\nException: {1}", directoryInfo.Name, e.Message));                      
            }
        }

        public static void RemoveJunction(string Item){
            if (Directory.Exists(Item)){
                try {
                    Directory.Delete(Item);
                } catch (Exception e) {
                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("EXCEPTION: Caught exception removing junction: {0}\nException: {1}", Item, e.Message));
                }
            }            
            if (Directory.Exists(string.Format("{0}--DISABLED", Item))){
                string og = string.Format("{0}--DISABLED", Item);
                string ren = og.Replace("--DISABLED", "");
                try {
                    if(new DirectoryInfo(Item).Attributes.HasFlag(FileAttributes.ReparsePoint)){
                        Directory.Move(og, ren);
                    } else {
                        File.Move(og, ren);
                    }                    
                } catch (Exception e) {
                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Caught exception renaming disabled folder: {0}\nException: {1}", Item, e.Message));                    
                }
            }
        }
    }
}