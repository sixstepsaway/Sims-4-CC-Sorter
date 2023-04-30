using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimsCCManager.Console
{
    public class Packages_Sort
    {
        public string sortingfolder_sims2wrong = "_INCORRECTGAME_Sims2";
        public string sortingfolder_sims3wrong = "_INCORRECTGAME_Sims3";
        public string sortingfolder_sims4wrong = "_INCORRECTGAME_Sims4";
        public string sortingfolder_miscwrong = "_INCORRECTGAME_Misc";
        public string sortingfolder_brokenwrong = "_BROKEN";
        public string sortingfolder_sims2 = "_Sims2";
        public string sortingfolder_sims3 = "_Sims3";
        public string sortingfolder_sims4 = "_Sims3";
        public string sortingfolder_other = "_OtherGames";

        public void SortDownloads (string folder) {
            string[] files = Directory.GetFiles(folder, SearchOption.AllDirectories);
            foreach (string file in files) {
                FileInfo packageFile = new FileInfo(file);

            }
        }
    }
}