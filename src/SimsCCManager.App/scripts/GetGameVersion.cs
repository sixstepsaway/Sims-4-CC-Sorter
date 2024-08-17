using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SimsCCManager.Debugging;
using SimsCCManager.Globals;

namespace SimsCCManager.Packages.Initial
{
    public class GetGameVersion
    {
        /// <summary>
        /// Universal methods for finding the game version of a package, whether the package is broken, and so on.
        /* GAME VERSION NUMBERS ARE:

                0 = null,
                1 = Sims 1,
                2 = Sims 2,
                3 = Sims 3,
                4 = Sims 4,
                11 = Spore,
                12 = SimCity 5*/
        /// </summary>
        /// 

        public static int CheckGame(string input){
            FileInfo package = new(input);
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Checking {0}'s version", input));
            using (FileStream msPackage = new(input, FileMode.Open, FileAccess.Read)){
                BinaryReader packagereader = new(msPackage);
                string test = "";
                test = Encoding.ASCII.GetString(packagereader.ReadBytes(4));
                if (test != "DBPF"){
                    return 0;
                } else {
                    uint major = packagereader.ReadUInt32();
                    uint minor = packagereader.ReadUInt32();
                    if ((major == 1 && minor == 1) || (major == 1 && minor == 2)){
                        return 2;
                    } else if (major == 2 && minor == 0){
                        return 3;
                    } else if (major == 2 && minor == 1){
                        return 4;
                    } else if (major == 3 && minor == 0){
                        return 12;
                    } else {
                        return 0;
                    }
                }
            }    
        }
    }
}