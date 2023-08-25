using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSA.VirtualFileSystem
{
    public class FileSystem
    {
        //https://stackoverflow.com/questions/11777924/how-to-make-a-read-only-file
        //https://stackoverflow.com/questions/1199571/how-to-hide-file-in-c
        //https://learn.microsoft.com/en-us/dotnet/api/system.io.file.createsymboliclink?view=net-7.0
        //https://stackoverflow.com/questions/3387690/how-to-create-a-hardlink-in-c
        //https://github.com/usdAG/SharpLink
        
        public void MakeSymbolicLink(string Original, string Destination){
            
        }

        public void RemoveSymbolicLink(string Item){
            
        }

        public void MakeJunction(string Original, string Destination){

        }

        public void RemoveJunction(string Item){

        }
    }
}