using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using SSAGlobals;
using SimsCCManager.Packages.Containers;
using SQLite;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace SimsCCManager.Packages.Sorting
{
    /// <summary>
    /// Package sorting functionality. The main base of why this whole app was made to begin with.
    /// </summary>
    public class SortingRules
    {   [Column("Folder")]
        public string Folder {get; set;}
        [Column("MatchType")]
        public string MatchType {get; set;}
        [Column("Match")]
        public string MatchTerm {get; set;}
    }

    public class FilesSort
    {
        LoggingGlobals log = new LoggingGlobals();
        
        //public static List<SortingRules> SortingRulesOverview = new();
        public static ObservableCollection<SortingRules> SortingRules = new();
        //public static List<SortingRules> SortingRulesGeneral = new();
        public static string SortedFolder = Path.Combine(GlobalVariables.ModFolder, "_SORTED");
        public static string ZipFilesFolder = Path.Combine(SortedFolder, "_CompressedFiles");
        public static string ZipFileDupesFolder = Path.Combine(ZipFilesFolder, "_DUPLICATES");
        public static string NonSimsFiles = Path.Combine(SortedFolder, "_NONSIMS");
        public static string BrokenFiles = Path.Combine(SortedFolder, "_BROKEN");
        public static string SortedS2Folder = Path.Combine(SortedFolder, "_Sims2");
        public static string S2PacksFolder = Path.Combine(SortedS2Folder, "__Sims2Packs");
        public static string SortedS3Folder = Path.Combine(SortedFolder, "_Sims3");
        public static string S3PacksFolder = Path.Combine(SortedS3Folder, "__Sims3Packs");
        public static string SortedS4Folder = Path.Combine(SortedFolder, "_Sims4");
        public static string S4ScriptsFolder = Path.Combine(SortedS4Folder, "__TS4Scripts");
        public static string TrayFilesFolder = Path.Combine(SortedS4Folder, "__Tray Files");
        public static string TrayFileDupesFolder = Path.Combine(TrayFilesFolder, "_DUPLICATES");
        public static string S2DupesFolder = Path.Combine(SortedS2Folder, "_DUPLICATES");
        public static string S3DupesFolder = Path.Combine(SortedS3Folder, "_DUPLICATES");
        public static string S4DupesFolder = Path.Combine(SortedS4Folder, "_DUPLICATES");
        public static string DuplicatesFolderGeneral = Path.Combine(SortedFolder, "_DUPLICATES");
        string MergedSubFolder = "_MERGED";
        string OrphanSubFolder = "_ORPHANS";
        //time periods

        public static string TPAll = "_00_All";
        public static string TPFantasy = "_00a_Fantasy";
        public static string TPSciFi = "_00b_SciFi";
        public static string TPFuturistic = "_00c_Futuristic";
        public static string TPGrunge = "_00d_Grunge";
        public static string TPDystopiApoc = "_00f_DystopianApocalypse";
        public static string TPCyberpunk = "_00g_Cyberpunk";
        public static string TPSteampunk = "_11b_Steampunk";
        
        public static string TPAncient = "_01_Ancient";
        public static string TPEarlyCiv = "_02_EarlyCiv";
        public static string TPMedieval = "_03_Medieval (476CE)";
        public static string TPRenaissance = "_04_Renaissance (1400)";
        public static string TPTudors = "_05_Tudors (1485)";
        public static string TPColonial = "_06_Colonial (1620)";
        public static string TPBaroque = "_07_Baroque (1700)";
        public static string TPRococo = "_08_Rococo (1730)";
        public static string TPIndustrialAge = "_09_IndustrialAge (1760)";
        public static string TPOldWest = "_10_OldWest (1865)";
        public static string TPVictorian = "_11a_Victorian (1890)";
        public static string TP1910s = "_12_1910s";
        public static string TP1920s = "_13_1920s";
        public static string TP1930s = "_14_1930s";
        public static string TP1940s = "_15_1940s";
        public static string TP1950s = "_16_1950s";
        public static string TP1960s = "_17_1960s";
        public static string TP1970s = "_18_1970s";
        public static string TP1980s = "_19_1980s";
        public static string TP1990s = "_20_1990s";
        public static string TP2000s = "_21_2000s";
        public static string TP2010s = "_22_2010s";
        public static string TPModern = "_Modern";


        public void SortFolders(){
            InitializeSortingRules(); 
        }


        Methods methods = new Methods();

        public void InitializeSortingRules(){
            
            
            /*#region Recolorists

            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Recolorists", MatchTerm="ombre", Folder="Ombres" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Recolorists", MatchTerm="overlay", Folder="Ombres" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Recolorists", MatchTerm="candle", Folder="_OFF THE GRID" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Recolorists", MatchTerm="lantern", Folder="_OFF THE GRID" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Recolorists", MatchTerm="AMPified", Folder="_Recolors\\AMPified" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Recolorists", MatchTerm="Jewl", Folder="_Recolors\\Jewl" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Recolorists", MatchTerm="Noodles", Folder="_Recolors\\Noodles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Recolorists", MatchTerm="Sorbet", Folder="_Recolors\\Sorbet" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Recolorists", MatchTerm="SweetHearts", Folder="_Recolors\\SweetHearts" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Recolorists", MatchTerm="WitchingHour", Folder="_Recolors\\WitchingHour" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Recolorists", MatchTerm="Puppycrow", Folder="_Recolors\\Puppycrow" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Recolorists", MatchTerm="Sandwich", Folder="_Recolors\\Sandwich" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Recolorists", MatchTerm="TWH", Folder="_Recolors\\TWH" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Recolorists", MatchTerm="Unnaturals", Folder="_Recolors\\Unnaturals" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Recolorists", MatchTerm="Jewels", Folder="_Recolors\\Jewels" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Recolorists", MatchTerm="Pooklet", Folder="_Recolors\\Pooklet" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Recolorists", MatchTerm="Crayola", Folder="_Recolors\\Crayola" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Recolorists", MatchTerm="Naturals", Folder="_Recolors\\Naturals" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Recolorists", MatchTerm="Candynaturals", Folder="_Recolors\\Candynaturals" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Recolorists", MatchTerm="Ombre", Folder="_Recolors\\Ombre" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Recolorists", MatchTerm="Candyshoppe", Folder="_Recolors\\Candyshoppe" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Recolorists", MatchTerm="Historian", Folder="_Recolors\\Historian" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Recolorists", MatchTerm="CSSH", Folder="_Recolors\\CSSH" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Recolorists", MatchTerm="Eezo", Folder="_Recolors\\Eezo" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Recolorists", MatchTerm="wdwehtp", Folder="_Recolors\\wdwehtp" });

            #endregion

            #region Misc Sorting

            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="brooch", Folder="_Brooches" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="highlight", Folder="_HairAccessories" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="animation", Folder="" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="eyelash", Folder="_Eyelashes" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="nosemask", Folder="_NoseMasks" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="vitiligo", Folder="_Vitiligo" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="occult", Folder="\\Occult" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="bodysuit", Folder="_Underwear" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="bra", Folder="_Underwear" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="acc", Folder="_Underwear" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="corset", Folder="_Underwear" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="fairy", Folder="_Fairy" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="supergirl", Folder="_Supergirl" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="superboy", Folder="_Superboy" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="ghost", Folder="_Ghost" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="skeleton", Folder="_Skeleton" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="piercing", Folder="_JewelryMisc" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="necrodog", Folder="Necrodog" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="uniform", Folder="Uniforms" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="preset", Folder="__Presets" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="slider", Folder="__Sliders" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="glasses", Folder="_Glasses" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="veil", Folder="_Veils" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="bonnet", Folder="_Hats" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="overlay", Folder="_ColorOverlays" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="eyelashes", Folder="_Eyelashes" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="lashes", Folder="_Eyelashes" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="armlet", Folder="_JewelryMisc" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="beard", Folder="_Beards" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="mustache", Folder="_Mustaches" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="sideburn", Folder="_Sideburns" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="hairpin", Folder="_HairAccessories" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="hijab", Folder="_Hijabs" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="ombre", Folder="_ColorOverlays" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="skindetail", Folder="_Misc" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="skinblend", Folder="" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="freckles", Folder="_Misc" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="nails", Folder="_Nails" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="headband", Folder="_HairAccessories" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="hairband", Folder="_HairAccessories" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="hairclips", Folder="_HairAccessories" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="clips", Folder="_HairAccessories" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="kerchief", Folder="_Misc" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="dyeaccessory", Folder="_ColorOverlays" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="scrunchie", Folder="_HairAccessories" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="mask", Folder="_Masks" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="tiara", Folder="_Crowns" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="crown", Folder="_Crowns" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="choker", Folder="_Necklaces" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="reindeer", Folder="_Christmas" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="christmas", Folder="_Christmas" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="santa", Folder="_Christmas" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="colormix", Folder="_ColorOverlays" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="acchat", Folder="_Hats" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="bouquet", Folder="_Bouquet" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="jacket", Folder="_Jackets" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="garter", Folder="_Underwear" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="backpack", Folder="_Backpack" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="pompom", Folder="_Misc" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="cardigan", Folder="_Jackets" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="blazer", Folder="_Jackets" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="hairelastic", Folder="_HairAccessories" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="hairwreath", Folder="_HairAccessories" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="bikinibottom", Folder="_Underwear" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="bikinitop", Folder="_Underwear" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="bikini", Folder="_Underwear" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="babyhair", Folder="Babyhair" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="hairline", Folder="_Hairlines" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="streakacc", Folder="_ColorOverlays" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="streaksacc", Folder="_ColorOverlays" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="roots", Folder="_Hairlines" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="stubble", Folder="_Beards" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="goatee", Folder="_Beards" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="muttonchops", Folder="_Beards" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="shield", Folder="_shield" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="sword", Folder="_sword" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="arrow", Folder="_arrow" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="hairbow", Folder="_HairAccessories" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="bow", Folder="_HairAccessories" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="nofoot", Folder="_Misc" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="lantern", Folder="_OTG" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="boat", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="bodyhair", Folder="_BodyHair" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="birthmark", Folder="_Misc" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="locket", Folder="_Necklaces" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="thong", Folder="_Underwear" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="bandaid", Folder="_Misc" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="earbud", Folder="_Misc" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="fannypack", Folder="_Bags" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="contact", Folder="_ContactLenses" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="helmet", Folder="_Hats" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="wheelchair", Folder="_Disability" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="hairstrand", Folder="_ColorOverlays" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="ties", Folder="_HairAccessories" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="nude", Folder="_NSFW" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="belt", Folder="_Belts" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="wings", Folder="Occult\\_Wings" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="bag", Folder="_Bags" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="lorysims", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="bandage", Folder="Injuries" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="headphones", Folder="_Headphones" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="turban", Folder="_Hats" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="dogtags", Folder="_Necklaces" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="mannequin", Folder="_Mannequins" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="bindi", Folder="_Misc" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="studs", Folder="_Earrings" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="missinghand", Folder="_Disability" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="mermaidtail", Folder="_Occult\\Mermaid" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="tail", Folder="_Occult" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="bassinet", Folder="\\Bassinet" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="moustache", Folder="_Mustaches" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="bentley", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="audi", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="mercedes", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="lexus", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="acura", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="chevrolet", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="ford", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="hennessey", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="jaguar", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="jeep", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="landrover", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="porsche", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="maserati", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="rollsroyce", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="bmw", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="ferrari", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="sierra", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="hyundai", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="polestar", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="tesla", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="volkswagen", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="volvo", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="bugatti", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="genesis", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="kia", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="nissan", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="rangerover", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="westernstar", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="luxor", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="yacht", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="helicopter", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="hotairballoon", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="lamborghini", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="bus", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="snowmobile", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="motors", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="train", Folder="_Vehicles" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="earmuffs", Folder="_Earmuffs" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="nsfw", Folder="_NSFW" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="nail", Folder="_Nails" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="injury", Folder="_Injuries" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="invisible", Folder="_invisible" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="bodyharness", Folder="_Misc" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="headbad", Folder="_Headbands" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="winestain", Folder="_Vitiligo" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="scar", Folder="_Scars" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="gun", Folder="_Props" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="pacifier ", Folder="_Pacifiers" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="binkie", Folder="_Pacifiers" });
            SortingRulesGeneral.Add(new SortingRules(){ MatchType = "Specifics", MatchTerm="binky", Folder="_Pacifiers" });           

            #endregion
            
            #region Personal Rules 

            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="amphora", Folder=TPAncient });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="legwarmers", Folder=TP1980s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="postcard", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="faxmachine", Folder=TP1990s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="castaway", Folder=TPAll });

            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="stone", Folder="_Possibly Historical" });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="bloomers", Folder="_Possibly Historical" });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="crystalball", Folder="_Possibly Historical" });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="shrine", Folder="_Possibly Historical" });

            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="campfire", Folder=TPAll });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="logfire", Folder=TPAll });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="well", Folder=TPAll });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="polaroid", Folder=TP1960s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="potion", Folder=TPAncient });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="cdtower", Folder=TP1990s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="lumber", Folder=TPAll });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="carddeck", Folder=TPMedieval });

            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="10s", Folder=TP1910s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="20s", Folder=TP1920s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="30s", Folder=TP1930s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="40s", Folder=TP1940s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="50s", Folder=TP1950s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="60s", Folder=TP1960s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="70s", Folder=TP1970s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="80s", Folder=TP1980s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="90s", Folder=TP1990s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1400", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1401", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1402", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1403", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1404", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1405", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1406", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1407", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1408", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1409", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1410", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1411", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1412", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1413", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1414", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1415", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1416", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1417", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1418", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1419", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1420", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1421", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1422", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1423", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1424", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1425", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1426", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1427", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1428", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1429", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1430", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1431", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1432", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1433", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1434", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1435", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1436", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1437", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1438", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1439", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1440", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1441", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1442", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1443", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1444", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1445", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1446", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1447", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1448", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1449", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1450", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1451", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1452", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1453", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1454", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1455", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1456", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1457", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1458", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1459", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1460", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1461", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1462", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1463", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1464", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1465", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1466", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1467", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1468", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1469", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1470", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1471", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1472", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1473", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1474", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1475", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1476", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1477", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1478", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1479", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1480", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1481", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1482", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1483", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1484", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1485", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1486", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1487", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1488", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1489", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1490", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1491", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1492", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1493", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1494", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1495", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1496", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1497", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1498", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1499", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1500", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1501", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1502", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1503", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1504", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1505", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1506", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1507", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1508", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1509", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1510", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1511", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1512", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1513", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1514", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1515", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1516", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1517", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1518", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1519", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1520", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1521", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1522", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1523", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1524", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1525", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1526", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1527", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1528", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1529", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1530", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1531", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1532", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1533", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1534", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1535", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1536", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1537", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1538", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1539", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1540", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1541", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1542", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1543", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1544", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1545", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1546", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1547", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1548", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1549", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1550", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1551", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1552", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1553", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1554", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1555", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1556", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1557", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1558", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1559", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1560", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1561", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1562", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1563", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1564", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1565", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1566", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1567", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1568", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1569", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1570", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1571", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1572", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1573", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1574", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1575", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1576", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1577", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1578", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1579", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1580", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1581", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1582", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1583", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1584", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1585", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1586", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1587", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1588", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1589", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1590", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1591", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1592", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1593", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1594", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1595", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1596", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1597", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1598", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1599", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1600", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1601", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1602", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1603", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1604", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1605", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1606", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1607", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1608", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1609", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1610", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1611", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1612", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1613", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1614", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1615", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1616", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1617", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1618", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1619", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1620", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1621", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1622", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1623", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1624", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1625", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1626", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1627", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1628", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1629", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1630", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1631", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1632", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1633", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1634", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1635", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1636", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1637", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1638", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1639", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1640", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1641", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1642", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1643", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1644", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1645", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1646", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1647", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1648", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1649", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1650", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1651", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1652", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1653", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1654", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1655", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1656", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1657", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1658", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1659", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1660", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1661", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1662", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1663", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1664", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1665", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1666", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1667", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1668", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1669", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1670", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1671", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1672", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1673", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1674", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1675", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1676", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1677", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1678", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1679", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1680", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1681", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1682", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1683", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1684", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1685", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1686", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1687", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1688", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1689", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1690", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1691", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1692", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1693", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1694", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1695", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1696", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1697", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1698", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1699", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1700", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1701", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1702", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1703", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1704", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1705", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1706", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1707", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1708", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1709", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1710", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1711", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1712", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1713", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1714", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1715", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1716", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1717", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1718", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1719", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1720", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1721", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1722", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1723", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1724", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1725", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1726", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1727", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1728", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1729", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1730", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1731", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1732", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1733", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1734", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1735", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1736", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1737", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1738", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1739", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1740", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1741", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1742", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1743", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1744", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1745", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1746", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1747", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1748", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1749", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1750", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1751", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1752", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1753", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1754", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1755", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1756", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1757", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1758", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1759", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1760", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1761", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1762", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1763", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1764", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1765", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1766", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1767", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1768", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1769", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1770", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1771", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1772", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1773", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1774", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1775", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1776", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1777", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1778", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1779", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1780", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1781", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1782", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1783", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1784", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1785", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1786", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1787", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1788", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1789", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1790", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1791", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1792", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1793", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1794", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1795", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1796", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1797", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1798", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1799", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1800", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1801", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1802", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1803", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1804", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1805", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1806", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1807", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1808", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1809", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1810", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1811", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1812", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1813", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1814", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1815", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1816", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1817", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1818", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1819", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1820", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1821", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1822", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1823", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1824", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1825", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1826", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1827", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1828", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1829", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1830", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1831", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1832", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1833", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1834", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1835", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1836", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1837", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1838", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1839", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1840", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1841", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1842", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1843", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1844", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1845", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1846", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1847", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1848", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1849", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1850", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1851", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1852", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1853", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1854", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1855", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1856", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1857", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1858", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1859", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1860", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1861", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1862", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1863", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1864", Folder=TPIndustrialAge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1865", Folder=TPOldWest });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1866", Folder=TPOldWest });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1867", Folder=TPOldWest });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1868", Folder=TPOldWest });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1869", Folder=TPOldWest });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1870", Folder=TPOldWest });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1871", Folder=TPOldWest });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1872", Folder=TPOldWest });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1873", Folder=TPOldWest });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1874", Folder=TPOldWest });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1875", Folder=TPOldWest });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1876", Folder=TPOldWest });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1877", Folder=TPOldWest });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1878", Folder=TPOldWest });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1879", Folder=TPOldWest });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1880", Folder=TPOldWest });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1881", Folder=TPOldWest });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1882", Folder=TPOldWest });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1883", Folder=TPOldWest });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1884", Folder=TPOldWest });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1885", Folder=TPOldWest });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1886", Folder=TPOldWest });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1887", Folder=TPOldWest });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1888", Folder=TPOldWest });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1889", Folder=TPOldWest });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1890", Folder=TPVictorian });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1891", Folder=TPVictorian });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1892", Folder=TPVictorian });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1893", Folder=TPVictorian });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1894", Folder=TPVictorian });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1895", Folder=TPVictorian });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1896", Folder=TPVictorian });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1897", Folder=TPVictorian });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1898", Folder=TPVictorian });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1899", Folder=TPVictorian });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1901", Folder=TP1910s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1902", Folder=TP1910s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1903", Folder=TP1910s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1904", Folder=TP1910s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1905", Folder=TP1910s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1906", Folder=TP1910s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1907", Folder=TP1910s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1908", Folder=TP1910s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1909", Folder=TP1910s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1910", Folder=TP1910s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1911", Folder=TP1910s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1912", Folder=TP1910s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1913", Folder=TP1910s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1914", Folder=TP1910s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1915", Folder=TP1910s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1916", Folder=TP1910s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1917", Folder=TP1910s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1918", Folder=TP1910s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1919", Folder=TP1910s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1920", Folder=TP1920s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1921", Folder=TP1920s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1922", Folder=TP1920s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1923", Folder=TP1920s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1924", Folder=TP1920s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1925", Folder=TP1920s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1926", Folder=TP1920s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1927", Folder=TP1920s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1928", Folder=TP1920s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1929", Folder=TP1920s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1930", Folder=TP1930s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1931", Folder=TP1930s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1932", Folder=TP1930s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1933", Folder=TP1930s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1934", Folder=TP1930s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1935", Folder=TP1930s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1936", Folder=TP1930s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1937", Folder=TP1930s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1938", Folder=TP1930s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1939", Folder=TP1930s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1940", Folder=TP1940s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1941", Folder=TP1940s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1942", Folder=TP1940s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1943", Folder=TP1940s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1944", Folder=TP1940s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1945", Folder=TP1940s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1946", Folder=TP1940s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1947", Folder=TP1940s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1948", Folder=TP1940s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1949", Folder=TP1940s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1950", Folder=TP1950s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1951", Folder=TP1950s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1952", Folder=TP1950s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1953", Folder=TP1950s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1954", Folder=TP1950s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1955", Folder=TP1950s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1956", Folder=TP1950s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1957", Folder=TP1950s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1958", Folder=TP1950s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1959", Folder=TP1950s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1960", Folder=TP1960s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1961", Folder=TP1960s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1962", Folder=TP1960s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1963", Folder=TP1960s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1964", Folder=TP1960s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1965", Folder=TP1960s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1966", Folder=TP1960s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1967", Folder=TP1960s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1968", Folder=TP1960s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1969", Folder=TP1960s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1970", Folder=TP1970s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1971", Folder=TP1970s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1972", Folder=TP1970s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1973", Folder=TP1970s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1974", Folder=TP1970s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1975", Folder=TP1970s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1976", Folder=TP1970s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1977", Folder=TP1970s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1978", Folder=TP1970s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1979", Folder=TP1970s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1980", Folder=TP1980s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1981", Folder=TP1980s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1982", Folder=TP1980s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1983", Folder=TP1980s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1984", Folder=TP1980s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1985", Folder=TP1980s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1986", Folder=TP1980s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1987", Folder=TP1980s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1988", Folder=TP1980s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1989", Folder=TP1980s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1990", Folder=TP1990s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1991", Folder=TP1990s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1992", Folder=TP1990s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1993", Folder=TP1990s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1994", Folder=TP1990s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1995", Folder=TP1990s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1996", Folder=TP1990s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1997", Folder=TP1990s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1998", Folder=TP1990s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="1999", Folder=TP1990s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="2000", Folder=TP2000s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="2001", Folder=TP2000s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="2002", Folder=TP2000s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="2003", Folder=TP2000s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="2004", Folder=TP2000s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="2005", Folder=TP2000s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="2006", Folder=TP2000s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="2007", Folder=TP2000s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="2008", Folder=TP2000s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="2009", Folder=TP2000s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="2010", Folder=TP2010s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="2011", Folder=TP2010s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="2012", Folder=TP2010s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="2013", Folder=TP2010s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="2014", Folder=TP2010s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="2015", Folder=TP2010s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="2016", Folder=TP2010s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="2017", Folder=TP2010s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="2018", Folder=TP2010s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="2019", Folder=TP2010s });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="apocalyp", Folder=TPDystopiApoc });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="baroque", Folder=TPBaroque });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="colonial", Folder=TPColonial });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="cyber", Folder=TPCyberpunk });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="cyberpunk", Folder=TPCyberpunk });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="cyfy", Folder=TPCyberpunk });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="grunge", Folder=TPGrunge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="medieval", Folder=TPMedieval });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="renaissance", Folder=TPRenaissance });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="rococo", Folder=TPRococo });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="shabby", Folder=TPGrunge });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="steampunk", Folder=TPSteampunk });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="TSM", Folder=TPMedieval });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="tudor", Folder=TPTudors });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="victorian", Folder=TPVictorian });
            
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="11", Folder="_Possibly Historical" });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="11th", Folder="_Possibly Historical" });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="12", Folder="_Possibly Historical" });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="12th", Folder="_Possibly Historical" });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="13th", Folder="_Possibly Historical" });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="14", Folder="_Possibly Historical" });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="14th", Folder="_Possibly Historical" });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="15", Folder="_Possibly Historical" });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="15th", Folder="_Possibly Historical" });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="16", Folder="_Possibly Historical" });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="16th", Folder="_Possibly Historical" });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="17", Folder="_Possibly Historical" });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="17th", Folder="_Possibly Historical" });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="18", Folder="_Possibly Historical" });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="18th", Folder="_Possibly Historical" });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="19", Folder="_Possibly Historical" });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="19th", Folder="_Possibly Historical" });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="ancient", Folder="_Possibly Historical" });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="egyptian", Folder="_Possibly Historical" });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="georgian", Folder="_Possibly Historical" });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="greece", Folder="_Possibly Historical" });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="roman", Folder="_Possibly Historical" });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="rustic", Folder="_Possibly Historical" });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="valhallan", Folder="_Possibly Historical" });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="vintage", Folder="_Possibly Historical" });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="ingeli", Folder="_Possibly Historical" });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="moriel", Folder="_Possibly Historical" });
            SortingRulesSS.Add(new SortingRules(){ MatchType = "CustomRules", MatchTerm="reminisims", Folder="_Possibly Historical" });
            
            #endregion   */        
            
            if (File.Exists(GlobalVariables.CustomSortingOptions)){
                using (StreamReader file = File.OpenText(GlobalVariables.CustomSortingOptions))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    SortingRules = (ObservableCollection<SortingRules>)serializer.Deserialize(file, typeof(ObservableCollection<SortingRules>));
                }
            } else {
                using (StreamReader file = File.OpenText(GlobalVariables.SortingOptionsDefault))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    SortingRules = (ObservableCollection<SortingRules>)serializer.Deserialize(file, typeof(ObservableCollection<SortingRules>));
                }
            }
            
            
            
        }




        public SimsPackage SortPackage(SimsPackage package){
            SimsPackage thisPackage = package;       
            string moveloc = "";
            string newloc = "";
            string typesortA = "";
            string typesortB = "";

            if (thisPackage.Type == "Merged Package"){
                if(thisPackage.Game == 2){
                    moveloc = Path.Combine(SortedS2Folder, MergedSubFolder);
                    if (!Directory.Exists(moveloc)){
                        Methods.MakeFolder(moveloc);
                    }
                    newloc = Path.Combine(moveloc, thisPackage.PackageName);
                }
                if(thisPackage.Game == 3){
                    moveloc = Path.Combine(SortedS3Folder, MergedSubFolder);
                    if (!Directory.Exists(moveloc)){
                        Methods.MakeFolder(moveloc);
                    }
                    newloc = Path.Combine(moveloc, thisPackage.PackageName);
                }
                if(thisPackage.Game == 4){
                    moveloc = Path.Combine(SortedS4Folder, MergedSubFolder);
                    if (!Directory.Exists(moveloc)){
                        Methods.MakeFolder(moveloc);
                    }
                    newloc = Path.Combine(moveloc, thisPackage.PackageName);
                }
                thisPackage = MoveFile(thisPackage.Location, newloc, moveloc, thisPackage);
                return thisPackage;
            }
            if (thisPackage.Orphan == true){
                if(thisPackage.Game == 2){
                    moveloc = Path.Combine(SortedS2Folder, OrphanSubFolder);
                    if (!Directory.Exists(moveloc)){
                        Methods.MakeFolder(moveloc);
                    }
                    newloc = Path.Combine(moveloc, thisPackage.PackageName);
                }
                if(thisPackage.Game == 3){
                    moveloc = Path.Combine(SortedS3Folder, OrphanSubFolder);
                    if (!Directory.Exists(moveloc)){
                        Methods.MakeFolder(moveloc);
                    }
                    newloc = Path.Combine(moveloc, thisPackage.PackageName);
                }
                if(thisPackage.Game == 4){
                    moveloc = Path.Combine(SortedS4Folder, OrphanSubFolder);
                    if (!Directory.Exists(moveloc)){
                        Methods.MakeFolder(moveloc);
                    }
                    newloc = Path.Combine(moveloc, thisPackage.PackageName);
                }
                thisPackage = MoveFile(thisPackage.Location, newloc, moveloc, thisPackage);
                return thisPackage;
            }
            if (!String.IsNullOrWhiteSpace(thisPackage.Type)){
                if (thisPackage.Type.Contains("OVERRIDE")){                
                    typesortA = thisPackage.OverridesList[0].Name;
                    typesortB = Path.Combine("_OVERRIDES", typesortA);
                    if(thisPackage.Game == 2){
                        moveloc = Path.Combine(SortedS2Folder, typesortB);
                        if (!Directory.Exists(moveloc)){
                            Methods.MakeFolder(moveloc);
                        }
                        newloc = Path.Combine(moveloc, thisPackage.PackageName);
                    }
                    if(thisPackage.Game == 3){
                        moveloc = Path.Combine(SortedS3Folder, typesortB);
                        if (!Directory.Exists(moveloc)){
                            Methods.MakeFolder(moveloc);
                        }
                        newloc = Path.Combine(moveloc, thisPackage.PackageName);
                    }
                    if(thisPackage.Game == 4){
                        moveloc = Path.Combine(SortedS4Folder, typesortB);
                        if (!Directory.Exists(moveloc)){
                            Methods.MakeFolder(moveloc);
                        }
                        newloc = Path.Combine(moveloc, thisPackage.PackageName);
                    }
                    thisPackage = MoveFile(thisPackage.Location, newloc, moveloc, thisPackage);
                    return thisPackage;
                }
            }

            

            if (!String.IsNullOrWhiteSpace(thisPackage.Function)){
                typesortA = thisPackage.Function;
                if (thisPackage.Function.Contains("Accessory") && thisPackage.PackageName.Contains("Hair")){
                    typesortB = "Hair Accessory";
                    if(thisPackage.Game == 2){
                        moveloc = Path.Combine(SortedS2Folder, typesortB);
                        newloc = Path.Combine(moveloc, thisPackage.PackageName);                        
                    }
                    if(thisPackage.Game == 3){
                        moveloc = Path.Combine(SortedS3Folder, typesortB);
                        newloc = Path.Combine(moveloc, thisPackage.PackageName);
                    }
                    if(thisPackage.Game == 4){
                        moveloc = Path.Combine(SortedS4Folder, typesortB);
                        newloc = Path.Combine(moveloc, thisPackage.PackageName);
                    }
                    thisPackage = MoveFile(thisPackage.Location, newloc, moveloc, thisPackage);
                    return thisPackage;
                }
                if (thisPackage.Function.Contains("Unidentified")){
                    typesortB = "Unidentified";
                    if(thisPackage.Game == 2){
                        moveloc = Path.Combine(SortedS2Folder, typesortB);
                        newloc = Path.Combine(moveloc, thisPackage.PackageName);                        
                    }
                    if(thisPackage.Game == 3){
                        moveloc = Path.Combine(SortedS3Folder, typesortB);
                        newloc = Path.Combine(moveloc, thisPackage.PackageName);
                    }
                    if(thisPackage.Game == 4){
                        moveloc = Path.Combine(SortedS4Folder, typesortB);
                        newloc = Path.Combine(moveloc, thisPackage.PackageName);
                    }
                    thisPackage = MoveFile(thisPackage.Location, newloc, moveloc, thisPackage);
                    return thisPackage;
                }
                if (thisPackage.Function.Contains("Accessory") && thisPackage.PackageName.Contains("Overlay")){
                    typesortB = "Color Overlay";
                    if(thisPackage.Game == 2){
                        moveloc = Path.Combine(SortedS2Folder, typesortB);
                        newloc = Path.Combine(moveloc, thisPackage.PackageName);
                    }
                    if(thisPackage.Game == 3){
                        moveloc = Path.Combine(SortedS3Folder, typesortB);
                        newloc = Path.Combine(moveloc, thisPackage.PackageName);
                    }
                    if(thisPackage.Game == 4){
                        moveloc = Path.Combine(SortedS4Folder, typesortB);
                        newloc = Path.Combine(moveloc, thisPackage.PackageName);
                    }
                    thisPackage = MoveFile(thisPackage.Location, newloc, moveloc, thisPackage);
                    return thisPackage;
                }
                foreach (SortingRules item in SortingRules){
                    if (thisPackage.PackageName.Contains(item.MatchTerm)){
                        typesortB = Path.Combine(typesortA, item.Folder);    
                        if(thisPackage.Game == 2){
                            moveloc = Path.Combine(SortedS2Folder, typesortB);
                            newloc = Path.Combine(moveloc, thisPackage.PackageName);
                        }
                        if(thisPackage.Game == 3){
                            moveloc = Path.Combine(SortedS3Folder, typesortB);
                            newloc = Path.Combine(moveloc, thisPackage.PackageName);
                        }
                        if(thisPackage.Game == 4){
                            moveloc = Path.Combine(SortedS4Folder, typesortB);
                            newloc = Path.Combine(moveloc, thisPackage.PackageName);
                        }
                        thisPackage = MoveFile(thisPackage.Location, newloc, moveloc, thisPackage);
                        return thisPackage;
                    }
                }
                foreach (SortingRules item in SortingRules){
                    if (thisPackage.PackageName.Contains(item.MatchTerm)){
                        typesortB = Path.Combine(typesortA, item.Folder);    
                        if(thisPackage.Game == 2){
                            moveloc = Path.Combine(SortedS2Folder, typesortB);
                            newloc = Path.Combine(moveloc, thisPackage.PackageName);
                        }
                        if(thisPackage.Game == 3){
                            moveloc = Path.Combine(SortedS3Folder, typesortB);
                            newloc = Path.Combine(moveloc, thisPackage.PackageName);
                        }
                        if(thisPackage.Game == 4){
                            moveloc = Path.Combine(SortedS4Folder, typesortB);
                            newloc = Path.Combine(moveloc, thisPackage.PackageName);
                        }
                        thisPackage = MoveFile(thisPackage.Location, newloc, moveloc, thisPackage);
                        return thisPackage;
                    }
                }
                if (!String.IsNullOrWhiteSpace(thisPackage.FunctionSubcategory)){
                    typesortB = Path.Combine(typesortA, thisPackage.FunctionSubcategory);
                    if(thisPackage.Game == 2){
                        moveloc = Path.Combine(SortedS2Folder, typesortB);
                        newloc = Path.Combine(moveloc, thisPackage.PackageName);
                    }
                    if(thisPackage.Game == 3){
                        moveloc = Path.Combine(SortedS3Folder, typesortB);
                        newloc = Path.Combine(moveloc, thisPackage.PackageName);
                    }
                    if(thisPackage.Game == 4){
                        moveloc = Path.Combine(SortedS4Folder, typesortB);
                        newloc = Path.Combine(moveloc, thisPackage.PackageName);
                    }
                    thisPackage = MoveFile(thisPackage.Location, newloc, moveloc, thisPackage);
                    return thisPackage;   
                } 
                if(thisPackage.Game == 2){
                    moveloc = Path.Combine(SortedS2Folder, typesortA);
                    newloc = Path.Combine(moveloc, thisPackage.PackageName);
                }
                if(thisPackage.Game == 3){
                    moveloc = Path.Combine(SortedS3Folder, typesortA);
                    newloc = Path.Combine(moveloc, thisPackage.PackageName);
                }
                if(thisPackage.Game == 4){
                    moveloc = Path.Combine(SortedS4Folder, typesortA);                        
                    newloc = Path.Combine(moveloc, thisPackage.PackageName);
                }
                thisPackage = MoveFile(thisPackage.Location, newloc, moveloc, thisPackage);
                return thisPackage;      
            }
            if(thisPackage.Game == 2){
                moveloc = Path.Combine(SortedS2Folder, "_MISC");
                newloc = Path.Combine(moveloc, thisPackage.PackageName);
                thisPackage = MoveFile(thisPackage.Location, newloc, moveloc, thisPackage);
                return thisPackage;  
            }
            if(thisPackage.Game == 3){
                moveloc = Path.Combine(SortedS3Folder, "_MISC");
                newloc = Path.Combine(moveloc, thisPackage.PackageName);
                thisPackage = MoveFile(thisPackage.Location, newloc, moveloc, thisPackage);
                return thisPackage;  
            }
            if(thisPackage.Game == 4){
                moveloc = Path.Combine(SortedS4Folder, "_MISC");
                newloc = Path.Combine(moveloc, thisPackage.PackageName);
                thisPackage = MoveFile(thisPackage.Location, newloc, moveloc, thisPackage);
                return thisPackage;  
            }
            return thisPackage;
        }


        public SimsPackage MoveFile(string oldloc, string newloc, string dirloc, SimsPackage thisPackage){
            if (!Directory.Exists(dirloc)){
                Methods.MakeFolder(dirloc);
            }
            if (!File.Exists(oldloc)){
                log.MakeLog(string.Format("Can't find file {0} for moving.", thisPackage.PackageName), true);
                thisPackage.Location = "not found";
                return thisPackage;
            } else if (File.Exists(newloc)){                
                log.MakeLog("File already exists, moving to duplicates instead.", true);
                if (thisPackage.Game == 2){
                    newloc = Path.Combine(S2DupesFolder, thisPackage.PackageName);
                    if (!Directory.Exists(S2DupesFolder)){
                        Methods.MakeFolder(S2DupesFolder);
                    }
                    File.Move(oldloc, newloc);
                    thisPackage.Location = newloc;
                    return thisPackage;
                } else if (thisPackage.Game == 3){
                    newloc = Path.Combine(S3DupesFolder, thisPackage.PackageName);
                    if (!Directory.Exists(S3DupesFolder)){
                        Methods.MakeFolder(S3DupesFolder);
                    }
                    File.Move(oldloc, newloc);
                    thisPackage.Location = newloc;
                    return thisPackage;
                } else if (thisPackage.Game == 4){
                    newloc = Path.Combine(S4DupesFolder, thisPackage.PackageName);
                    if (!Directory.Exists(S4DupesFolder)){
                        Methods.MakeFolder(S4DupesFolder);
                    }
                    File.Move(oldloc, newloc);
                    thisPackage.Location = newloc;
                    return thisPackage;
                } else {
                    newloc = Path.Combine(DuplicatesFolderGeneral, thisPackage.PackageName);
                    if (!Directory.Exists(DuplicatesFolderGeneral)){
                        Methods.MakeFolder(DuplicatesFolderGeneral);
                    }
                    File.Move(oldloc, newloc);
                    thisPackage.Location = newloc;
                    return thisPackage;
                }            
            } else {
                File.Move(oldloc, newloc);
                thisPackage.Location = newloc;
                return thisPackage;
            }       
        }

        public AllFiles MoveFile(string oldloc, string newloc, string dirloc, AllFiles thisFile){
            if (!Directory.Exists(dirloc)){
                Methods.MakeFolder(dirloc);
            }
            if (File.Exists(newloc)){                
                log.MakeLog("File already exists, moving to duplicates instead.", true);
                if (thisFile.Type == "sims2pack"){
                    newloc = Path.Combine(S2DupesFolder, thisFile.Name);
                    if (!Directory.Exists(S2DupesFolder)){
                        Methods.MakeFolder(S2DupesFolder);
                    }
                    if(File.Exists(oldloc)){
                        File.Move(oldloc, newloc);
                        thisFile.Location = newloc;
                        return thisFile;
                    } else {
                        thisFile.Location = "not found";
                        thisFile.Status = "not found";
                        return thisFile;
                    }                    
                } else if (thisFile.Type == "sims3pack"){
                    newloc = Path.Combine(S3DupesFolder, thisFile.Name);
                    if (!Directory.Exists(S3DupesFolder)){
                        Methods.MakeFolder(S3DupesFolder);
                    }                    
                    if(File.Exists(oldloc)){
                        File.Move(oldloc, newloc);
                        thisFile.Location = newloc;
                        return thisFile;
                    } else {
                        thisFile.Location = "not found";
                        thisFile.Status = "not found";
                        return thisFile;
                    }  
                } else if (thisFile.Type == "ts4script"){
                    newloc = Path.Combine(S4DupesFolder, thisFile.Name);
                    if (!Directory.Exists(S4DupesFolder)){
                        Methods.MakeFolder(S4DupesFolder);
                    }                    
                    if(File.Exists(oldloc)){
                        File.Move(oldloc, newloc);
                        thisFile.Location = newloc;
                        return thisFile;
                    } else {
                        thisFile.Location = "not found";
                        thisFile.Status = "not found";
                        return thisFile;
                    }  
                } else if (thisFile.Type == "compressed file"){
                    newloc = Path.Combine(ZipFileDupesFolder, thisFile.Name);
                    if (!Directory.Exists(ZipFileDupesFolder)){
                        Methods.MakeFolder(ZipFileDupesFolder);
                    }
                    if(File.Exists(oldloc)){
                        File.Move(oldloc, newloc);
                        thisFile.Location = newloc;
                        return thisFile;
                    } else {
                        thisFile.Location = "not found";
                        thisFile.Status = "not found";
                        return thisFile;
                    }  
                } else if (thisFile.Type == "tray file"){
                    newloc = Path.Combine(TrayFileDupesFolder, thisFile.Name);
                    if (!Directory.Exists(TrayFileDupesFolder)){
                        Methods.MakeFolder(TrayFileDupesFolder);
                    }
                    if(File.Exists(oldloc)){
                        File.Move(oldloc, newloc);
                        thisFile.Location = newloc;
                        return thisFile;
                    } else {
                        thisFile.Location = "not found";
                        thisFile.Status = "not found";
                        return thisFile;
                    }  
                } else if (thisFile.Type == "other"){
                    newloc = Path.Combine(DuplicatesFolderGeneral, thisFile.Name);
                    if (!Directory.Exists(DuplicatesFolderGeneral)){
                        Methods.MakeFolder(DuplicatesFolderGeneral);
                    }
                    if(File.Exists(oldloc)){
                        File.Move(oldloc, newloc);
                        thisFile.Location = newloc;
                        return thisFile;
                    } else {
                        thisFile.Location = "not found";
                        thisFile.Status = "not found";
                        return thisFile;
                    }  
                } else {
                    newloc = Path.Combine(DuplicatesFolderGeneral, thisFile.Name);
                    if (!Directory.Exists(DuplicatesFolderGeneral)){
                        Methods.MakeFolder(DuplicatesFolderGeneral);
                    }
                    if(File.Exists(oldloc)){
                        File.Move(oldloc, newloc);
                        thisFile.Location = newloc;
                        return thisFile;
                    } else {
                        thisFile.Location = "not found";
                        thisFile.Status = "not found";
                        return thisFile;
                    }  
                }
            } else if (!File.Exists(oldloc)){
                log.MakeLog(string.Format("Can't find file {0} for moving.", thisFile.Name), true);
                thisFile.Location = "not found";
                return thisFile;
            } else {
                if(File.Exists(oldloc)){
                    File.Move(oldloc, newloc);
                    thisFile.Location = newloc;
                    return thisFile;
                } else {
                    thisFile.Location = "not found";
                    thisFile.Status = "not found";
                    return thisFile;
                }  
            }       
        }
    }
}