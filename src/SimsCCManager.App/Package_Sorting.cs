using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using SSAGlobals;
using SimsCCManager.Packages.Containers;
using SQLite;

namespace SimsCCManager.Packages.Sorting
{
    /// <summary>
    /// Package sorting functionality. The main base of why this whole app was made to begin with.
    /// </summary>
    public class SortingRules
    {   [Column("Folder")]
        public string Folder {get; set;}
        [Column("MatchType")]
        public string MatchTermType {get; set;}
        [Column("Match")]
        public string MatchTerm {get; set;}
    }

    public class FilesSort
    {
        LoggingGlobals log = new LoggingGlobals();
        
        List<SortingRules> SortingRulesOverview = new List<SortingRules>();
        Dictionary<string, string> SortingRulesSS = new Dictionary<string, string>();
        Dictionary<string, string> SortingRulesGeneral = new Dictionary<string, string>();
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


        public void SortFolders(){
            Prep();
        }


        Methods methods = new Methods();
        
        public void Prep(){            
            /*InitializeSortingRules();            
            Methods.MakeFolder(SortedFolder);

            foreach (SimsPackage package in Containers.Containers.allSims2Packages){
                string mainfolder = package.Function;
                string subfolder = package.FunctionSubcategory;
                string newfolder = SortedS2Folder + mainfolder + subfolder;
                Methods.MakeFolder(newfolder);
                string newlocation = newfolder + package.PackageName;

                if (!File.Exists(newlocation)){
                    log.MakeLog(package.PackageName + " already exists at new location of " + newlocation + ". Moving instead to duplicates.", true);
                    Methods.MakeFolder(S2DupesFolder);
                    string dupesloc = S2DupesFolder + package.PackageName;
                    File.Move(package.Location, newlocation);
                } else {
                    File.Move(package.Location, newlocation);
                }
            }

            foreach (SimsPackage package in Containers.Containers.allSims3Packages){
                string mainfolder = package.Function;
                string subfolder = package.FunctionSubcategory;
                string newfolder = SortedS3Folder + mainfolder + subfolder;
                Methods.MakeFolder(newfolder);
                string newlocation = newfolder + package.PackageName;

                if (!File.Exists(newlocation)){
                    log.MakeLog(package.PackageName + " already exists at new location of " + newlocation + ". Moving instead to duplicates.", true);
                    Methods.MakeFolder(S3DupesFolder);
                    string dupesloc = S3DupesFolder + package.PackageName;
                    File.Move(package.Location, dupesloc);
                } else {
                    File.Move(package.Location, newlocation);
                }
            }
            foreach (SimsPackage package in Containers.Containers.allSims4Packages){
                if (package.CatalogTags.Exists(x => x.stringval == "Func_OffTheGrid")){
                    string otgfolder = SortedS4Folder + "_OTG";
                    string newlocation = SortedS4Folder + "_OTG\\" + package.PackageName + ".package";
                    if (!File.Exists(newlocation))
                    {   
                        string dupesloc = S4DupesFolder + package.PackageName;
                        log.MakeLog(package.PackageName + " already exists at new location of " + otgfolder + ". Moving instead to duplicates.", true);
                        File.Move(package.Location, dupesloc);
                    } else {
                       File.Move(package.Location, newlocation); 
                    }
                } else {
                    string mainfolder = package.Function;
                    string subfolder = package.FunctionSubcategory;
                    string newfolder = SortedS4Folder + mainfolder + subfolder;
                    Methods.MakeFolder(newfolder);
                    string newlocation = newfolder + package.PackageName;

                    if (!File.Exists(newlocation)){
                        log.MakeLog(package.PackageName + " already exists at new location of " + newlocation + ". Moving instead to duplicates.", true);
                        Methods.MakeFolder(S4DupesFolder);
                        string dupesloc = S4DupesFolder + package.PackageName;
                        File.Move(package.Location, dupesloc);
                    } else {
                        File.Move(package.Location, newlocation);
                    }
                }
            }*/




        }



        





        public void InitializeSortingRules(){
            
            
            #region Recolorists

            SortingRulesGeneral.Add("ombre", "Ombres");
            SortingRulesGeneral.Add("overlay", "Ombres");
            SortingRulesGeneral.Add("candle", "_OFF THE GRID");
            SortingRulesGeneral.Add("lantern", "_OFF THE GRID");
            SortingRulesGeneral.Add("AMPified", "_Recolors\\AMPified");
            SortingRulesGeneral.Add("Jewl", "_Recolors\\Jewl");
            SortingRulesGeneral.Add("Noodles", "_Recolors\\Noodles");
            SortingRulesGeneral.Add("Sorbet", "_Recolors\\Sorbet");
            SortingRulesGeneral.Add("SweetHearts", "_Recolors\\SweetHearts");
            SortingRulesGeneral.Add("WitchingHour", "_Recolors\\WitchingHour");
            SortingRulesGeneral.Add("Puppycrow", "_Recolors\\Puppycrow");
            SortingRulesGeneral.Add("Sandwich", "_Recolors\\Sandwich");
            SortingRulesGeneral.Add("TWH", "_Recolors\\TWH");
            SortingRulesGeneral.Add("Unnaturals", "_Recolors\\Unnaturals");
            SortingRulesGeneral.Add("Jewels", "_Recolors\\Jewels");
            SortingRulesGeneral.Add("Pooklet", "_Recolors\\Pooklet");
            SortingRulesGeneral.Add("Crayola", "_Recolors\\Crayola");
            SortingRulesGeneral.Add("Naturals", "_Recolors\\Naturals");
            SortingRulesGeneral.Add("Candynaturals", "_Recolors\\Candynaturals");
            SortingRulesGeneral.Add("Ombre", "_Recolors\\Ombre");
            SortingRulesGeneral.Add("Candyshoppe", "_Recolors\\Candyshoppe");
            SortingRulesGeneral.Add("Historian", "_Recolors\\Historian");
            SortingRulesGeneral.Add("CSSH", "_Recolors\\CSSH");
            SortingRulesGeneral.Add("Eezo", "_Recolors\\Eezo");
            SortingRulesGeneral.Add("wdwehtp", "_Recolors\\wdwehtp");

            #endregion

            #region Misc Sorting

            SortingRulesGeneral.Add("brooch", "_Brooches");
            SortingRulesGeneral.Add("highlight", "_HairAccessories");
            SortingRulesGeneral.Add("animation", "");
            SortingRulesGeneral.Add("eyelash", "_Eyelashes");
            SortingRulesGeneral.Add("nosemask", "_NoseMasks");
            SortingRulesGeneral.Add("vitiligo", "_Vitiligo");
            SortingRulesGeneral.Add("occult", "\\Occult");
            SortingRulesGeneral.Add("bodysuit", "_Underwear");
            SortingRulesGeneral.Add("bra", "_Underwear");
            SortingRulesGeneral.Add("acc", "_Underwear");
            SortingRulesGeneral.Add("corset", "_Underwear");
            SortingRulesGeneral.Add("fairy", "_Fairy");
            SortingRulesGeneral.Add("supergirl", "_Supergirl");
            SortingRulesGeneral.Add("superboy", "_Superboy");
            SortingRulesGeneral.Add("ghost", "_Ghost");
            SortingRulesGeneral.Add("skeleton", "_Skeleton");
            SortingRulesGeneral.Add("piercing", "_JewelryMisc");
            SortingRulesGeneral.Add("necrodog", "Necrodog");
            SortingRulesGeneral.Add("uniform", "Uniforms");
            SortingRulesGeneral.Add("preset", "__Presets");
            SortingRulesGeneral.Add("slider", "__Sliders");
            SortingRulesGeneral.Add("glasses", "_Glasses");
            SortingRulesGeneral.Add("veil", "_Veils");
            SortingRulesGeneral.Add("bonnet", "_Hats");
            SortingRulesGeneral.Add("overlay", "_ColorOverlays");
            SortingRulesGeneral.Add("eyelashes", "_Eyelashes");
            SortingRulesGeneral.Add("lashes", "_Eyelashes");
            SortingRulesGeneral.Add("armlet", "_JewelryMisc");
            SortingRulesGeneral.Add("beard", "_Beards");
            SortingRulesGeneral.Add("mustache", "_Mustaches");
            SortingRulesGeneral.Add("sideburn", "_Sideburns");
            SortingRulesGeneral.Add("hairpin", "_HairAccessories");
            SortingRulesGeneral.Add("hijab", "_Hijabs");
            SortingRulesGeneral.Add("ombre", "_ColorOverlays");
            SortingRulesGeneral.Add("skindetail", "_Misc");
            SortingRulesGeneral.Add("skinblend", "");
            SortingRulesGeneral.Add("freckles", "_Misc");
            SortingRulesGeneral.Add("nails", "_Nails");
            SortingRulesGeneral.Add("headband", "_HairAccessories");
            SortingRulesGeneral.Add("hairband", "_HairAccessories");
            SortingRulesGeneral.Add("hairclips", "_HairAccessories");
            SortingRulesGeneral.Add("clips", "_HairAccessories");
            SortingRulesGeneral.Add("kerchief", "_Misc");
            SortingRulesGeneral.Add("dyeaccessory", "_ColorOverlays");
            SortingRulesGeneral.Add("scrunchie", "_HairAccessories");
            SortingRulesGeneral.Add("mask", "_Masks");
            SortingRulesGeneral.Add("tiara", "_Crowns");
            SortingRulesGeneral.Add("crown", "_Crowns");
            SortingRulesGeneral.Add("choker", "_Necklaces");
            SortingRulesGeneral.Add("reindeer", "_Christmas");
            SortingRulesGeneral.Add("christmas", "_Christmas");
            SortingRulesGeneral.Add("santa", "_Christmas");
            SortingRulesGeneral.Add("colormix", "_ColorOverlays");
            SortingRulesGeneral.Add("acchat", "_Hats");
            SortingRulesGeneral.Add("bouquet", "_Bouquet");
            SortingRulesGeneral.Add("jacket", "_Jackets");
            SortingRulesGeneral.Add("garter", "_Underwear");
            SortingRulesGeneral.Add("backpack", "_Backpack");
            SortingRulesGeneral.Add("pompom", "_Misc");
            SortingRulesGeneral.Add("cardigan", "_Jackets");
            SortingRulesGeneral.Add("blazer", "_Jackets");
            SortingRulesGeneral.Add("hairelastic", "_HairAccessories");
            SortingRulesGeneral.Add("hairwreath", "_HairAccessories");
            SortingRulesGeneral.Add("bikinibottom", "_Underwear");
            SortingRulesGeneral.Add("bikinitop", "_Underwear");
            SortingRulesGeneral.Add("bikini", "_Underwear");
            SortingRulesGeneral.Add("babyhair", "Babyhair");
            SortingRulesGeneral.Add("hairline", "_Hairlines");
            SortingRulesGeneral.Add("streakacc", "_ColorOverlays");
            SortingRulesGeneral.Add("streaksacc", "_ColorOverlays");
            SortingRulesGeneral.Add("roots", "_Hairlines");
            SortingRulesGeneral.Add("stubble", "_Beards");
            SortingRulesGeneral.Add("goatee", "_Beards");
            SortingRulesGeneral.Add("muttonchops", "_Beards");
            SortingRulesGeneral.Add("shield", "_shield");
            SortingRulesGeneral.Add("sword", "_sword");
            SortingRulesGeneral.Add("arrow", "_arrow");
            SortingRulesGeneral.Add("hairbow", "_HairAccessories");
            SortingRulesGeneral.Add("bow", "_HairAccessories");
            SortingRulesGeneral.Add("nofoot", "_Misc");
            SortingRulesGeneral.Add("lantern", "_OTG");
            SortingRulesGeneral.Add("boat", "_Vehicles");
            SortingRulesGeneral.Add("bodyhair", "_BodyHair");
            SortingRulesGeneral.Add("birthmark", "_Misc");
            SortingRulesGeneral.Add("locket", "_Necklaces");
            SortingRulesGeneral.Add("thong", "_Underwear");
            SortingRulesGeneral.Add("bandaid", "_Misc");
            SortingRulesGeneral.Add("earbud", "_Misc");
            SortingRulesGeneral.Add("fannypack", "_Bags");
            SortingRulesGeneral.Add("contact", "_ContactLenses");
            SortingRulesGeneral.Add("helmet", "_Hats");
            SortingRulesGeneral.Add("wheelchair", "_Disability");
            SortingRulesGeneral.Add("hairstrand", "_ColorOverlays");
            SortingRulesGeneral.Add("ties", "_HairAccessories");
            SortingRulesGeneral.Add("nude", "_NSFW");
            SortingRulesGeneral.Add("belt", "_Belts");
            SortingRulesGeneral.Add("wings", "Occult\\_Wings");
            SortingRulesGeneral.Add("bag", "_Bags");
            SortingRulesGeneral.Add("lorysims", "_Vehicles");
            SortingRulesGeneral.Add("bandage", "Injuries");
            SortingRulesGeneral.Add("headphones", "_Headphones");
            SortingRulesGeneral.Add("turban", "_Hats");
            SortingRulesGeneral.Add("dogtags", "_Necklaces");
            SortingRulesGeneral.Add("mannequin", "_Mannequins");
            SortingRulesGeneral.Add("bindi", "_Misc");
            SortingRulesGeneral.Add("studs", "_Earrings");
            SortingRulesGeneral.Add("missinghand", "_Disability");
            SortingRulesGeneral.Add("mermaidtail", "_Occult\\Mermaid");
            SortingRulesGeneral.Add("tail", "_Occult");
            SortingRulesGeneral.Add("bassinet", "\\Bassinet");
            SortingRulesGeneral.Add("moustache", "_Mustaches");
            SortingRulesGeneral.Add("bentley", "_Vehicles");
            SortingRulesGeneral.Add("audi", "_Vehicles");
            SortingRulesGeneral.Add("mercedes", "_Vehicles");
            SortingRulesGeneral.Add("lexus", "_Vehicles");
            SortingRulesGeneral.Add("acura", "_Vehicles");
            SortingRulesGeneral.Add("chevrolet", "_Vehicles");
            SortingRulesGeneral.Add("ford", "_Vehicles");
            SortingRulesGeneral.Add("hennessey", "_Vehicles");
            SortingRulesGeneral.Add("jaguar", "_Vehicles");
            SortingRulesGeneral.Add("jeep", "_Vehicles");
            SortingRulesGeneral.Add("landrover", "_Vehicles");
            SortingRulesGeneral.Add("porsche", "_Vehicles");
            SortingRulesGeneral.Add("maserati", "_Vehicles");
            SortingRulesGeneral.Add("rollsroyce", "_Vehicles");
            SortingRulesGeneral.Add("bmw", "_Vehicles");
            SortingRulesGeneral.Add("ferrari", "_Vehicles");
            SortingRulesGeneral.Add("sierra", "_Vehicles");
            SortingRulesGeneral.Add("hyundai", "_Vehicles");
            SortingRulesGeneral.Add("polestar", "_Vehicles");
            SortingRulesGeneral.Add("tesla", "_Vehicles");
            SortingRulesGeneral.Add("volkswagen", "_Vehicles");
            SortingRulesGeneral.Add("volvo", "_Vehicles");
            SortingRulesGeneral.Add("bugatti", "_Vehicles");
            SortingRulesGeneral.Add("genesis", "_Vehicles");
            SortingRulesGeneral.Add("kia", "_Vehicles");
            SortingRulesGeneral.Add("nissan", "_Vehicles");
            SortingRulesGeneral.Add("rangerover", "_Vehicles");
            SortingRulesGeneral.Add("westernstar", "_Vehicles");
            SortingRulesGeneral.Add("luxor", "_Vehicles");
            SortingRulesGeneral.Add("yacht", "_Vehicles");
            SortingRulesGeneral.Add("helicopter", "_Vehicles");
            SortingRulesGeneral.Add("hotairballoon", "_Vehicles");
            SortingRulesGeneral.Add("lamborghini", "_Vehicles");
            SortingRulesGeneral.Add("bus", "_Vehicles");
            SortingRulesGeneral.Add("snowmobile", "_Vehicles");
            SortingRulesGeneral.Add("motors", "_Vehicles");
            SortingRulesGeneral.Add("train", "_Vehicles");
            SortingRulesGeneral.Add("earmuffs", "_Earmuffs");
            SortingRulesGeneral.Add("nsfw", "_NSFW");
            SortingRulesGeneral.Add("nail", "_Nails");
            SortingRulesGeneral.Add("injury", "_Injuries");
            SortingRulesGeneral.Add("invisible", "_invisible");
            SortingRulesGeneral.Add("bodyharness", "_Misc");
            SortingRulesGeneral.Add("headbad", "_Headbands");
            SortingRulesGeneral.Add("winestain", "_Vitiligo");
            SortingRulesGeneral.Add("scar", "_Scars");
            SortingRulesGeneral.Add("gun", "_Props");
            SortingRulesGeneral.Add("pacifier ", "_Pacifiers");
            SortingRulesGeneral.Add("binkie", "_Pacifiers");
            SortingRulesGeneral.Add("binky", "_Pacifiers");


            

            #endregion
            
            #region Personal Rules 

            SortingRulesSS.Add("amphora", TPAncient);
            SortingRulesSS.Add("legwarmers", TP1980s);
            SortingRulesSS.Add("postcard", TPIndustrialAge);
            SortingRulesSS.Add("faxmachine", TP1990s);
            SortingRulesSS.Add("castaway", TPAll);
            SortingRulesSS.Add("stone", "_Possibly Historical");
            SortingRulesSS.Add("bloomers", "_Possibly Historical");
            SortingRulesSS.Add("crystalball", "_Possibly Historical");
            SortingRulesSS.Add("shrine", "_Possibly Historical");
            SortingRulesSS.Add("campfire", TPAll);
            SortingRulesSS.Add("logfire", TPAll);
            SortingRulesSS.Add("well", TPAll);
            SortingRulesSS.Add("polaroid", TP1960s);
            SortingRulesSS.Add("potion", TPAncient);
            SortingRulesSS.Add("cdtower", TP1990s);
            SortingRulesSS.Add("lumber", TPAll);
            SortingRulesSS.Add("carddeck", TPMedieval);

            SortingRulesSS.Add("10s", TP1910s);
            SortingRulesSS.Add("20s", TP1920s);
            SortingRulesSS.Add("30s", TP1930s);
            SortingRulesSS.Add("40s", TP1940s);
            SortingRulesSS.Add("50s", TP1950s);
            SortingRulesSS.Add("60s", TP1960s);
            SortingRulesSS.Add("70s", TP1970s);
            SortingRulesSS.Add("80s", TP1980s);
            SortingRulesSS.Add("90s", TP1990s);
            SortingRulesSS.Add("1400", TPRenaissance);
            SortingRulesSS.Add("1401", TPRenaissance);
            SortingRulesSS.Add("1402", TPRenaissance);
            SortingRulesSS.Add("1403", TPRenaissance);
            SortingRulesSS.Add("1404", TPRenaissance);
            SortingRulesSS.Add("1405", TPRenaissance);
            SortingRulesSS.Add("1406", TPRenaissance);
            SortingRulesSS.Add("1407", TPRenaissance);
            SortingRulesSS.Add("1408", TPRenaissance);
            SortingRulesSS.Add("1409", TPRenaissance);
            SortingRulesSS.Add("1410", TPRenaissance);
            SortingRulesSS.Add("1411", TPRenaissance);
            SortingRulesSS.Add("1412", TPRenaissance);
            SortingRulesSS.Add("1413", TPRenaissance);
            SortingRulesSS.Add("1414", TPRenaissance);
            SortingRulesSS.Add("1415", TPRenaissance);
            SortingRulesSS.Add("1416", TPRenaissance);
            SortingRulesSS.Add("1417", TPRenaissance);
            SortingRulesSS.Add("1418", TPRenaissance);
            SortingRulesSS.Add("1419", TPRenaissance);
            SortingRulesSS.Add("1420", TPRenaissance);
            SortingRulesSS.Add("1421", TPRenaissance);
            SortingRulesSS.Add("1422", TPRenaissance);
            SortingRulesSS.Add("1423", TPRenaissance);
            SortingRulesSS.Add("1424", TPRenaissance);
            SortingRulesSS.Add("1425", TPRenaissance);
            SortingRulesSS.Add("1426", TPRenaissance);
            SortingRulesSS.Add("1427", TPRenaissance);
            SortingRulesSS.Add("1428", TPRenaissance);
            SortingRulesSS.Add("1429", TPRenaissance);
            SortingRulesSS.Add("1430", TPRenaissance);
            SortingRulesSS.Add("1431", TPRenaissance);
            SortingRulesSS.Add("1432", TPRenaissance);
            SortingRulesSS.Add("1433", TPRenaissance);
            SortingRulesSS.Add("1434", TPRenaissance);
            SortingRulesSS.Add("1435", TPRenaissance);
            SortingRulesSS.Add("1436", TPRenaissance);
            SortingRulesSS.Add("1437", TPRenaissance);
            SortingRulesSS.Add("1438", TPRenaissance);
            SortingRulesSS.Add("1439", TPRenaissance);
            SortingRulesSS.Add("1440", TPRenaissance);
            SortingRulesSS.Add("1441", TPRenaissance);
            SortingRulesSS.Add("1442", TPRenaissance);
            SortingRulesSS.Add("1443", TPRenaissance);
            SortingRulesSS.Add("1444", TPRenaissance);
            SortingRulesSS.Add("1445", TPRenaissance);
            SortingRulesSS.Add("1446", TPRenaissance);
            SortingRulesSS.Add("1447", TPRenaissance);
            SortingRulesSS.Add("1448", TPRenaissance);
            SortingRulesSS.Add("1449", TPRenaissance);
            SortingRulesSS.Add("1450", TPRenaissance);
            SortingRulesSS.Add("1451", TPRenaissance);
            SortingRulesSS.Add("1452", TPRenaissance);
            SortingRulesSS.Add("1453", TPRenaissance);
            SortingRulesSS.Add("1454", TPRenaissance);
            SortingRulesSS.Add("1455", TPRenaissance);
            SortingRulesSS.Add("1456", TPRenaissance);
            SortingRulesSS.Add("1457", TPRenaissance);
            SortingRulesSS.Add("1458", TPRenaissance);
            SortingRulesSS.Add("1459", TPRenaissance);
            SortingRulesSS.Add("1460", TPRenaissance);
            SortingRulesSS.Add("1461", TPRenaissance);
            SortingRulesSS.Add("1462", TPRenaissance);
            SortingRulesSS.Add("1463", TPRenaissance);
            SortingRulesSS.Add("1464", TPRenaissance);
            SortingRulesSS.Add("1465", TPRenaissance);
            SortingRulesSS.Add("1466", TPRenaissance);
            SortingRulesSS.Add("1467", TPRenaissance);
            SortingRulesSS.Add("1468", TPRenaissance);
            SortingRulesSS.Add("1469", TPRenaissance);
            SortingRulesSS.Add("1470", TPRenaissance);
            SortingRulesSS.Add("1471", TPRenaissance);
            SortingRulesSS.Add("1472", TPRenaissance);
            SortingRulesSS.Add("1473", TPRenaissance);
            SortingRulesSS.Add("1474", TPRenaissance);
            SortingRulesSS.Add("1475", TPRenaissance);
            SortingRulesSS.Add("1476", TPRenaissance);
            SortingRulesSS.Add("1477", TPRenaissance);
            SortingRulesSS.Add("1478", TPRenaissance);
            SortingRulesSS.Add("1479", TPRenaissance);
            SortingRulesSS.Add("1480", TPRenaissance);
            SortingRulesSS.Add("1481", TPRenaissance);
            SortingRulesSS.Add("1482", TPRenaissance);
            SortingRulesSS.Add("1483", TPRenaissance);
            SortingRulesSS.Add("1484", TPRenaissance);
            SortingRulesSS.Add("1485", TPTudors);
            SortingRulesSS.Add("1486", TPTudors);
            SortingRulesSS.Add("1487", TPTudors);
            SortingRulesSS.Add("1488", TPTudors);
            SortingRulesSS.Add("1489", TPTudors);
            SortingRulesSS.Add("1490", TPTudors);
            SortingRulesSS.Add("1491", TPTudors);
            SortingRulesSS.Add("1492", TPTudors);
            SortingRulesSS.Add("1493", TPTudors);
            SortingRulesSS.Add("1494", TPTudors);
            SortingRulesSS.Add("1495", TPTudors);
            SortingRulesSS.Add("1496", TPTudors);
            SortingRulesSS.Add("1497", TPTudors);
            SortingRulesSS.Add("1498", TPTudors);
            SortingRulesSS.Add("1499", TPTudors);
            SortingRulesSS.Add("1500", TPTudors);
            SortingRulesSS.Add("1501", TPTudors);
            SortingRulesSS.Add("1502", TPTudors);
            SortingRulesSS.Add("1503", TPTudors);
            SortingRulesSS.Add("1504", TPTudors);
            SortingRulesSS.Add("1505", TPTudors);
            SortingRulesSS.Add("1506", TPTudors);
            SortingRulesSS.Add("1507", TPTudors);
            SortingRulesSS.Add("1508", TPTudors);
            SortingRulesSS.Add("1509", TPTudors);
            SortingRulesSS.Add("1510", TPTudors);
            SortingRulesSS.Add("1511", TPTudors);
            SortingRulesSS.Add("1512", TPTudors);
            SortingRulesSS.Add("1513", TPTudors);
            SortingRulesSS.Add("1514", TPTudors);
            SortingRulesSS.Add("1515", TPTudors);
            SortingRulesSS.Add("1516", TPTudors);
            SortingRulesSS.Add("1517", TPTudors);
            SortingRulesSS.Add("1518", TPTudors);
            SortingRulesSS.Add("1519", TPTudors);
            SortingRulesSS.Add("1520", TPTudors);
            SortingRulesSS.Add("1521", TPTudors);
            SortingRulesSS.Add("1522", TPTudors);
            SortingRulesSS.Add("1523", TPTudors);
            SortingRulesSS.Add("1524", TPTudors);
            SortingRulesSS.Add("1525", TPTudors);
            SortingRulesSS.Add("1526", TPTudors);
            SortingRulesSS.Add("1527", TPTudors);
            SortingRulesSS.Add("1528", TPTudors);
            SortingRulesSS.Add("1529", TPTudors);
            SortingRulesSS.Add("1530", TPTudors);
            SortingRulesSS.Add("1531", TPTudors);
            SortingRulesSS.Add("1532", TPTudors);
            SortingRulesSS.Add("1533", TPTudors);
            SortingRulesSS.Add("1534", TPTudors);
            SortingRulesSS.Add("1535", TPTudors);
            SortingRulesSS.Add("1536", TPTudors);
            SortingRulesSS.Add("1537", TPTudors);
            SortingRulesSS.Add("1538", TPTudors);
            SortingRulesSS.Add("1539", TPTudors);
            SortingRulesSS.Add("1540", TPTudors);
            SortingRulesSS.Add("1541", TPTudors);
            SortingRulesSS.Add("1542", TPTudors);
            SortingRulesSS.Add("1543", TPTudors);
            SortingRulesSS.Add("1544", TPTudors);
            SortingRulesSS.Add("1545", TPTudors);
            SortingRulesSS.Add("1546", TPTudors);
            SortingRulesSS.Add("1547", TPTudors);
            SortingRulesSS.Add("1548", TPTudors);
            SortingRulesSS.Add("1549", TPTudors);
            SortingRulesSS.Add("1550", TPTudors);
            SortingRulesSS.Add("1551", TPTudors);
            SortingRulesSS.Add("1552", TPTudors);
            SortingRulesSS.Add("1553", TPTudors);
            SortingRulesSS.Add("1554", TPTudors);
            SortingRulesSS.Add("1555", TPTudors);
            SortingRulesSS.Add("1556", TPTudors);
            SortingRulesSS.Add("1557", TPTudors);
            SortingRulesSS.Add("1558", TPTudors);
            SortingRulesSS.Add("1559", TPTudors);
            SortingRulesSS.Add("1560", TPTudors);
            SortingRulesSS.Add("1561", TPTudors);
            SortingRulesSS.Add("1562", TPTudors);
            SortingRulesSS.Add("1563", TPTudors);
            SortingRulesSS.Add("1564", TPTudors);
            SortingRulesSS.Add("1565", TPTudors);
            SortingRulesSS.Add("1566", TPTudors);
            SortingRulesSS.Add("1567", TPTudors);
            SortingRulesSS.Add("1568", TPTudors);
            SortingRulesSS.Add("1569", TPTudors);
            SortingRulesSS.Add("1570", TPTudors);
            SortingRulesSS.Add("1571", TPTudors);
            SortingRulesSS.Add("1572", TPTudors);
            SortingRulesSS.Add("1573", TPTudors);
            SortingRulesSS.Add("1574", TPTudors);
            SortingRulesSS.Add("1575", TPTudors);
            SortingRulesSS.Add("1576", TPTudors);
            SortingRulesSS.Add("1577", TPTudors);
            SortingRulesSS.Add("1578", TPTudors);
            SortingRulesSS.Add("1579", TPTudors);
            SortingRulesSS.Add("1580", TPTudors);
            SortingRulesSS.Add("1581", TPTudors);
            SortingRulesSS.Add("1582", TPTudors);
            SortingRulesSS.Add("1583", TPTudors);
            SortingRulesSS.Add("1584", TPTudors);
            SortingRulesSS.Add("1585", TPTudors);
            SortingRulesSS.Add("1586", TPTudors);
            SortingRulesSS.Add("1587", TPTudors);
            SortingRulesSS.Add("1588", TPTudors);
            SortingRulesSS.Add("1589", TPTudors);
            SortingRulesSS.Add("1590", TPTudors);
            SortingRulesSS.Add("1591", TPTudors);
            SortingRulesSS.Add("1592", TPTudors);
            SortingRulesSS.Add("1593", TPTudors);
            SortingRulesSS.Add("1594", TPTudors);
            SortingRulesSS.Add("1595", TPTudors);
            SortingRulesSS.Add("1596", TPTudors);
            SortingRulesSS.Add("1597", TPTudors);
            SortingRulesSS.Add("1598", TPTudors);
            SortingRulesSS.Add("1599", TPTudors);
            SortingRulesSS.Add("1600", TPTudors);
            SortingRulesSS.Add("1601", TPTudors);
            SortingRulesSS.Add("1602", TPTudors);
            SortingRulesSS.Add("1603", TPTudors);
            SortingRulesSS.Add("1604", TPTudors);
            SortingRulesSS.Add("1605", TPTudors);
            SortingRulesSS.Add("1606", TPTudors);
            SortingRulesSS.Add("1607", TPTudors);
            SortingRulesSS.Add("1608", TPTudors);
            SortingRulesSS.Add("1609", TPTudors);
            SortingRulesSS.Add("1610", TPTudors);
            SortingRulesSS.Add("1611", TPTudors);
            SortingRulesSS.Add("1612", TPTudors);
            SortingRulesSS.Add("1613", TPTudors);
            SortingRulesSS.Add("1614", TPTudors);
            SortingRulesSS.Add("1615", TPTudors);
            SortingRulesSS.Add("1616", TPTudors);
            SortingRulesSS.Add("1617", TPTudors);
            SortingRulesSS.Add("1618", TPTudors);
            SortingRulesSS.Add("1619", TPTudors);
            SortingRulesSS.Add("1620", TPColonial);
            SortingRulesSS.Add("1621", TPColonial);
            SortingRulesSS.Add("1622", TPColonial);
            SortingRulesSS.Add("1623", TPColonial);
            SortingRulesSS.Add("1624", TPColonial);
            SortingRulesSS.Add("1625", TPColonial);
            SortingRulesSS.Add("1626", TPColonial);
            SortingRulesSS.Add("1627", TPColonial);
            SortingRulesSS.Add("1628", TPColonial);
            SortingRulesSS.Add("1629", TPColonial);
            SortingRulesSS.Add("1630", TPColonial);
            SortingRulesSS.Add("1631", TPColonial);
            SortingRulesSS.Add("1632", TPColonial);
            SortingRulesSS.Add("1633", TPColonial);
            SortingRulesSS.Add("1634", TPColonial);
            SortingRulesSS.Add("1635", TPColonial);
            SortingRulesSS.Add("1636", TPColonial);
            SortingRulesSS.Add("1637", TPColonial);
            SortingRulesSS.Add("1638", TPColonial);
            SortingRulesSS.Add("1639", TPColonial);
            SortingRulesSS.Add("1640", TPColonial);
            SortingRulesSS.Add("1641", TPColonial);
            SortingRulesSS.Add("1642", TPColonial);
            SortingRulesSS.Add("1643", TPColonial);
            SortingRulesSS.Add("1644", TPColonial);
            SortingRulesSS.Add("1645", TPColonial);
            SortingRulesSS.Add("1646", TPColonial);
            SortingRulesSS.Add("1647", TPColonial);
            SortingRulesSS.Add("1648", TPColonial);
            SortingRulesSS.Add("1649", TPColonial);
            SortingRulesSS.Add("1650", TPColonial);
            SortingRulesSS.Add("1651", TPColonial);
            SortingRulesSS.Add("1652", TPColonial);
            SortingRulesSS.Add("1653", TPColonial);
            SortingRulesSS.Add("1654", TPColonial);
            SortingRulesSS.Add("1655", TPColonial);
            SortingRulesSS.Add("1656", TPColonial);
            SortingRulesSS.Add("1657", TPColonial);
            SortingRulesSS.Add("1658", TPColonial);
            SortingRulesSS.Add("1659", TPColonial);
            SortingRulesSS.Add("1660", TPColonial);
            SortingRulesSS.Add("1661", TPColonial);
            SortingRulesSS.Add("1662", TPColonial);
            SortingRulesSS.Add("1663", TPColonial);
            SortingRulesSS.Add("1664", TPColonial);
            SortingRulesSS.Add("1665", TPColonial);
            SortingRulesSS.Add("1666", TPColonial);
            SortingRulesSS.Add("1667", TPColonial);
            SortingRulesSS.Add("1668", TPColonial);
            SortingRulesSS.Add("1669", TPColonial);
            SortingRulesSS.Add("1670", TPColonial);
            SortingRulesSS.Add("1671", TPColonial);
            SortingRulesSS.Add("1672", TPColonial);
            SortingRulesSS.Add("1673", TPColonial);
            SortingRulesSS.Add("1674", TPColonial);
            SortingRulesSS.Add("1675", TPColonial);
            SortingRulesSS.Add("1676", TPColonial);
            SortingRulesSS.Add("1677", TPColonial);
            SortingRulesSS.Add("1678", TPColonial);
            SortingRulesSS.Add("1679", TPColonial);
            SortingRulesSS.Add("1680", TPColonial);
            SortingRulesSS.Add("1681", TPColonial);
            SortingRulesSS.Add("1682", TPColonial);
            SortingRulesSS.Add("1683", TPColonial);
            SortingRulesSS.Add("1684", TPColonial);
            SortingRulesSS.Add("1685", TPColonial);
            SortingRulesSS.Add("1686", TPColonial);
            SortingRulesSS.Add("1687", TPColonial);
            SortingRulesSS.Add("1688", TPColonial);
            SortingRulesSS.Add("1689", TPColonial);
            SortingRulesSS.Add("1690", TPColonial);
            SortingRulesSS.Add("1691", TPColonial);
            SortingRulesSS.Add("1692", TPColonial);
            SortingRulesSS.Add("1693", TPColonial);
            SortingRulesSS.Add("1694", TPColonial);
            SortingRulesSS.Add("1695", TPColonial);
            SortingRulesSS.Add("1696", TPColonial);
            SortingRulesSS.Add("1697", TPColonial);
            SortingRulesSS.Add("1698", TPColonial);
            SortingRulesSS.Add("1699", TPColonial);
            SortingRulesSS.Add("1700", TPBaroque);
            SortingRulesSS.Add("1701", TPBaroque);
            SortingRulesSS.Add("1702", TPBaroque);
            SortingRulesSS.Add("1703", TPBaroque);
            SortingRulesSS.Add("1704", TPBaroque);
            SortingRulesSS.Add("1705", TPBaroque);
            SortingRulesSS.Add("1706", TPBaroque);
            SortingRulesSS.Add("1707", TPBaroque);
            SortingRulesSS.Add("1708", TPBaroque);
            SortingRulesSS.Add("1709", TPBaroque);
            SortingRulesSS.Add("1710", TPBaroque);
            SortingRulesSS.Add("1711", TPBaroque);
            SortingRulesSS.Add("1712", TPBaroque);
            SortingRulesSS.Add("1713", TPBaroque);
            SortingRulesSS.Add("1714", TPBaroque);
            SortingRulesSS.Add("1715", TPBaroque);
            SortingRulesSS.Add("1716", TPBaroque);
            SortingRulesSS.Add("1717", TPBaroque);
            SortingRulesSS.Add("1718", TPBaroque);
            SortingRulesSS.Add("1719", TPBaroque);
            SortingRulesSS.Add("1720", TPBaroque);
            SortingRulesSS.Add("1721", TPBaroque);
            SortingRulesSS.Add("1722", TPBaroque);
            SortingRulesSS.Add("1723", TPBaroque);
            SortingRulesSS.Add("1724", TPBaroque);
            SortingRulesSS.Add("1725", TPBaroque);
            SortingRulesSS.Add("1726", TPBaroque);
            SortingRulesSS.Add("1727", TPBaroque);
            SortingRulesSS.Add("1728", TPBaroque);
            SortingRulesSS.Add("1729", TPBaroque);
            SortingRulesSS.Add("1730", TPRococo);
            SortingRulesSS.Add("1731", TPRococo);
            SortingRulesSS.Add("1732", TPRococo);
            SortingRulesSS.Add("1733", TPRococo);
            SortingRulesSS.Add("1734", TPRococo);
            SortingRulesSS.Add("1735", TPRococo);
            SortingRulesSS.Add("1736", TPRococo);
            SortingRulesSS.Add("1737", TPRococo);
            SortingRulesSS.Add("1738", TPRococo);
            SortingRulesSS.Add("1739", TPRococo);
            SortingRulesSS.Add("1740", TPRococo);
            SortingRulesSS.Add("1741", TPRococo);
            SortingRulesSS.Add("1742", TPRococo);
            SortingRulesSS.Add("1743", TPRococo);
            SortingRulesSS.Add("1744", TPRococo);
            SortingRulesSS.Add("1745", TPRococo);
            SortingRulesSS.Add("1746", TPRococo);
            SortingRulesSS.Add("1747", TPRococo);
            SortingRulesSS.Add("1748", TPRococo);
            SortingRulesSS.Add("1749", TPRococo);
            SortingRulesSS.Add("1750", TPRococo);
            SortingRulesSS.Add("1751", TPRococo);
            SortingRulesSS.Add("1752", TPRococo);
            SortingRulesSS.Add("1753", TPRococo);
            SortingRulesSS.Add("1754", TPRococo);
            SortingRulesSS.Add("1755", TPRococo);
            SortingRulesSS.Add("1756", TPRococo);
            SortingRulesSS.Add("1757", TPRococo);
            SortingRulesSS.Add("1758", TPRococo);
            SortingRulesSS.Add("1759", TPRococo);
            SortingRulesSS.Add("1760", TPIndustrialAge);
            SortingRulesSS.Add("1761", TPIndustrialAge);
            SortingRulesSS.Add("1762", TPIndustrialAge);
            SortingRulesSS.Add("1763", TPIndustrialAge);
            SortingRulesSS.Add("1764", TPIndustrialAge);
            SortingRulesSS.Add("1765", TPIndustrialAge);
            SortingRulesSS.Add("1766", TPIndustrialAge);
            SortingRulesSS.Add("1767", TPIndustrialAge);
            SortingRulesSS.Add("1768", TPIndustrialAge);
            SortingRulesSS.Add("1769", TPIndustrialAge);
            SortingRulesSS.Add("1770", TPIndustrialAge);
            SortingRulesSS.Add("1771", TPIndustrialAge);
            SortingRulesSS.Add("1772", TPIndustrialAge);
            SortingRulesSS.Add("1773", TPIndustrialAge);
            SortingRulesSS.Add("1774", TPIndustrialAge);
            SortingRulesSS.Add("1775", TPIndustrialAge);
            SortingRulesSS.Add("1776", TPIndustrialAge);
            SortingRulesSS.Add("1777", TPIndustrialAge);
            SortingRulesSS.Add("1778", TPIndustrialAge);
            SortingRulesSS.Add("1779", TPIndustrialAge);
            SortingRulesSS.Add("1780", TPIndustrialAge);
            SortingRulesSS.Add("1781", TPIndustrialAge);
            SortingRulesSS.Add("1782", TPIndustrialAge);
            SortingRulesSS.Add("1783", TPIndustrialAge);
            SortingRulesSS.Add("1784", TPIndustrialAge);
            SortingRulesSS.Add("1785", TPIndustrialAge);
            SortingRulesSS.Add("1786", TPIndustrialAge);
            SortingRulesSS.Add("1787", TPIndustrialAge);
            SortingRulesSS.Add("1788", TPIndustrialAge);
            SortingRulesSS.Add("1789", TPIndustrialAge);
            SortingRulesSS.Add("1790", TPIndustrialAge);
            SortingRulesSS.Add("1791", TPIndustrialAge);
            SortingRulesSS.Add("1792", TPIndustrialAge);
            SortingRulesSS.Add("1793", TPIndustrialAge);
            SortingRulesSS.Add("1794", TPIndustrialAge);
            SortingRulesSS.Add("1795", TPIndustrialAge);
            SortingRulesSS.Add("1796", TPIndustrialAge);
            SortingRulesSS.Add("1797", TPIndustrialAge);
            SortingRulesSS.Add("1798", TPIndustrialAge);
            SortingRulesSS.Add("1799", TPIndustrialAge);
            SortingRulesSS.Add("1800", TPIndustrialAge);
            SortingRulesSS.Add("1801", TPIndustrialAge);
            SortingRulesSS.Add("1802", TPIndustrialAge);
            SortingRulesSS.Add("1803", TPIndustrialAge);
            SortingRulesSS.Add("1804", TPIndustrialAge);
            SortingRulesSS.Add("1805", TPIndustrialAge);
            SortingRulesSS.Add("1806", TPIndustrialAge);
            SortingRulesSS.Add("1807", TPIndustrialAge);
            SortingRulesSS.Add("1808", TPIndustrialAge);
            SortingRulesSS.Add("1809", TPIndustrialAge);
            SortingRulesSS.Add("1810", TPIndustrialAge);
            SortingRulesSS.Add("1811", TPIndustrialAge);
            SortingRulesSS.Add("1812", TPIndustrialAge);
            SortingRulesSS.Add("1813", TPIndustrialAge);
            SortingRulesSS.Add("1814", TPIndustrialAge);
            SortingRulesSS.Add("1815", TPIndustrialAge);
            SortingRulesSS.Add("1816", TPIndustrialAge);
            SortingRulesSS.Add("1817", TPIndustrialAge);
            SortingRulesSS.Add("1818", TPIndustrialAge);
            SortingRulesSS.Add("1819", TPIndustrialAge);
            SortingRulesSS.Add("1820", TPIndustrialAge);
            SortingRulesSS.Add("1821", TPIndustrialAge);
            SortingRulesSS.Add("1822", TPIndustrialAge);
            SortingRulesSS.Add("1823", TPIndustrialAge);
            SortingRulesSS.Add("1824", TPIndustrialAge);
            SortingRulesSS.Add("1825", TPIndustrialAge);
            SortingRulesSS.Add("1826", TPIndustrialAge);
            SortingRulesSS.Add("1827", TPIndustrialAge);
            SortingRulesSS.Add("1828", TPIndustrialAge);
            SortingRulesSS.Add("1829", TPIndustrialAge);
            SortingRulesSS.Add("1830", TPIndustrialAge);
            SortingRulesSS.Add("1831", TPIndustrialAge);
            SortingRulesSS.Add("1832", TPIndustrialAge);
            SortingRulesSS.Add("1833", TPIndustrialAge);
            SortingRulesSS.Add("1834", TPIndustrialAge);
            SortingRulesSS.Add("1835", TPIndustrialAge);
            SortingRulesSS.Add("1836", TPIndustrialAge);
            SortingRulesSS.Add("1837", TPIndustrialAge);
            SortingRulesSS.Add("1838", TPIndustrialAge);
            SortingRulesSS.Add("1839", TPIndustrialAge);
            SortingRulesSS.Add("1840", TPIndustrialAge);
            SortingRulesSS.Add("1841", TPIndustrialAge);
            SortingRulesSS.Add("1842", TPIndustrialAge);
            SortingRulesSS.Add("1843", TPIndustrialAge);
            SortingRulesSS.Add("1844", TPIndustrialAge);
            SortingRulesSS.Add("1845", TPIndustrialAge);
            SortingRulesSS.Add("1846", TPIndustrialAge);
            SortingRulesSS.Add("1847", TPIndustrialAge);
            SortingRulesSS.Add("1848", TPIndustrialAge);
            SortingRulesSS.Add("1849", TPIndustrialAge);
            SortingRulesSS.Add("1850", TPIndustrialAge);
            SortingRulesSS.Add("1851", TPIndustrialAge);
            SortingRulesSS.Add("1852", TPIndustrialAge);
            SortingRulesSS.Add("1853", TPIndustrialAge);
            SortingRulesSS.Add("1854", TPIndustrialAge);
            SortingRulesSS.Add("1855", TPIndustrialAge);
            SortingRulesSS.Add("1856", TPIndustrialAge);
            SortingRulesSS.Add("1857", TPIndustrialAge);
            SortingRulesSS.Add("1858", TPIndustrialAge);
            SortingRulesSS.Add("1859", TPIndustrialAge);
            SortingRulesSS.Add("1860", TPIndustrialAge);
            SortingRulesSS.Add("1861", TPIndustrialAge);
            SortingRulesSS.Add("1862", TPIndustrialAge);
            SortingRulesSS.Add("1863", TPIndustrialAge);
            SortingRulesSS.Add("1864", TPIndustrialAge);
            SortingRulesSS.Add("1865", TPOldWest);
            SortingRulesSS.Add("1866", TPOldWest);
            SortingRulesSS.Add("1867", TPOldWest);
            SortingRulesSS.Add("1868", TPOldWest);
            SortingRulesSS.Add("1869", TPOldWest);
            SortingRulesSS.Add("1870", TPOldWest);
            SortingRulesSS.Add("1871", TPOldWest);
            SortingRulesSS.Add("1872", TPOldWest);
            SortingRulesSS.Add("1873", TPOldWest);
            SortingRulesSS.Add("1874", TPOldWest);
            SortingRulesSS.Add("1875", TPOldWest);
            SortingRulesSS.Add("1876", TPOldWest);
            SortingRulesSS.Add("1877", TPOldWest);
            SortingRulesSS.Add("1878", TPOldWest);
            SortingRulesSS.Add("1879", TPOldWest);
            SortingRulesSS.Add("1880", TPOldWest);
            SortingRulesSS.Add("1881", TPOldWest);
            SortingRulesSS.Add("1882", TPOldWest);
            SortingRulesSS.Add("1883", TPOldWest);
            SortingRulesSS.Add("1884", TPOldWest);
            SortingRulesSS.Add("1885", TPOldWest);
            SortingRulesSS.Add("1886", TPOldWest);
            SortingRulesSS.Add("1887", TPOldWest);
            SortingRulesSS.Add("1888", TPOldWest);
            SortingRulesSS.Add("1889", TPOldWest);
            SortingRulesSS.Add("1890", TPVictorian);
            SortingRulesSS.Add("1891", TPVictorian);
            SortingRulesSS.Add("1892", TPVictorian);
            SortingRulesSS.Add("1893", TPVictorian);
            SortingRulesSS.Add("1894", TPVictorian);
            SortingRulesSS.Add("1895", TPVictorian);
            SortingRulesSS.Add("1896", TPVictorian);
            SortingRulesSS.Add("1897", TPVictorian);
            SortingRulesSS.Add("1898", TPVictorian);
            SortingRulesSS.Add("1899", TPVictorian);
            SortingRulesSS.Add("1901", TP1910s);
            SortingRulesSS.Add("1902", TP1910s);
            SortingRulesSS.Add("1903", TP1910s);
            SortingRulesSS.Add("1904", TP1910s);
            SortingRulesSS.Add("1905", TP1910s);
            SortingRulesSS.Add("1906", TP1910s);
            SortingRulesSS.Add("1907", TP1910s);
            SortingRulesSS.Add("1908", TP1910s);
            SortingRulesSS.Add("1909", TP1910s);
            SortingRulesSS.Add("1910", TP1910s);
            SortingRulesSS.Add("1911", TP1910s);
            SortingRulesSS.Add("1912", TP1910s);
            SortingRulesSS.Add("1913", TP1910s);
            SortingRulesSS.Add("1914", TP1910s);
            SortingRulesSS.Add("1915", TP1910s);
            SortingRulesSS.Add("1916", TP1910s);
            SortingRulesSS.Add("1917", TP1910s);
            SortingRulesSS.Add("1918", TP1910s);
            SortingRulesSS.Add("1919", TP1910s);
            SortingRulesSS.Add("1920", TP1920s);
            SortingRulesSS.Add("1921", TP1920s);
            SortingRulesSS.Add("1922", TP1920s);
            SortingRulesSS.Add("1923", TP1920s);
            SortingRulesSS.Add("1924", TP1920s);
            SortingRulesSS.Add("1925", TP1920s);
            SortingRulesSS.Add("1926", TP1920s);
            SortingRulesSS.Add("1927", TP1920s);
            SortingRulesSS.Add("1928", TP1920s);
            SortingRulesSS.Add("1929", TP1920s);
            SortingRulesSS.Add("1930", TP1930s);
            SortingRulesSS.Add("1931", TP1930s);
            SortingRulesSS.Add("1932", TP1930s);
            SortingRulesSS.Add("1933", TP1930s);
            SortingRulesSS.Add("1934", TP1930s);
            SortingRulesSS.Add("1935", TP1930s);
            SortingRulesSS.Add("1936", TP1930s);
            SortingRulesSS.Add("1937", TP1930s);
            SortingRulesSS.Add("1938", TP1930s);
            SortingRulesSS.Add("1939", TP1930s);
            SortingRulesSS.Add("1940", TP1940s);
            SortingRulesSS.Add("1941", TP1940s);
            SortingRulesSS.Add("1942", TP1940s);
            SortingRulesSS.Add("1943", TP1940s);
            SortingRulesSS.Add("1944", TP1940s);
            SortingRulesSS.Add("1945", TP1940s);
            SortingRulesSS.Add("1946", TP1940s);
            SortingRulesSS.Add("1947", TP1940s);
            SortingRulesSS.Add("1948", TP1940s);
            SortingRulesSS.Add("1949", TP1940s);
            SortingRulesSS.Add("1950", TP1950s);
            SortingRulesSS.Add("1951", TP1950s);
            SortingRulesSS.Add("1952", TP1950s);
            SortingRulesSS.Add("1953", TP1950s);
            SortingRulesSS.Add("1954", TP1950s);
            SortingRulesSS.Add("1955", TP1950s);
            SortingRulesSS.Add("1956", TP1950s);
            SortingRulesSS.Add("1957", TP1950s);
            SortingRulesSS.Add("1958", TP1950s);
            SortingRulesSS.Add("1959", TP1950s);
            SortingRulesSS.Add("1960", TP1960s);
            SortingRulesSS.Add("1961", TP1960s);
            SortingRulesSS.Add("1962", TP1960s);
            SortingRulesSS.Add("1963", TP1960s);
            SortingRulesSS.Add("1964", TP1960s);
            SortingRulesSS.Add("1965", TP1960s);
            SortingRulesSS.Add("1966", TP1960s);
            SortingRulesSS.Add("1967", TP1960s);
            SortingRulesSS.Add("1968", TP1960s);
            SortingRulesSS.Add("1969", TP1960s);
            SortingRulesSS.Add("1970", TP1970s);
            SortingRulesSS.Add("1971", TP1970s);
            SortingRulesSS.Add("1972", TP1970s);
            SortingRulesSS.Add("1973", TP1970s);
            SortingRulesSS.Add("1974", TP1970s);
            SortingRulesSS.Add("1975", TP1970s);
            SortingRulesSS.Add("1976", TP1970s);
            SortingRulesSS.Add("1977", TP1970s);
            SortingRulesSS.Add("1978", TP1970s);
            SortingRulesSS.Add("1979", TP1970s);
            SortingRulesSS.Add("1980", TP1980s);
            SortingRulesSS.Add("1981", TP1980s);
            SortingRulesSS.Add("1982", TP1980s);
            SortingRulesSS.Add("1983", TP1980s);
            SortingRulesSS.Add("1984", TP1980s);
            SortingRulesSS.Add("1985", TP1980s);
            SortingRulesSS.Add("1986", TP1980s);
            SortingRulesSS.Add("1987", TP1980s);
            SortingRulesSS.Add("1988", TP1980s);
            SortingRulesSS.Add("1989", TP1980s);
            SortingRulesSS.Add("1990", TP1990s);
            SortingRulesSS.Add("1991", TP1990s);
            SortingRulesSS.Add("1992", TP1990s);
            SortingRulesSS.Add("1993", TP1990s);
            SortingRulesSS.Add("1994", TP1990s);
            SortingRulesSS.Add("1995", TP1990s);
            SortingRulesSS.Add("1996", TP1990s);
            SortingRulesSS.Add("1997", TP1990s);
            SortingRulesSS.Add("1998", TP1990s);
            SortingRulesSS.Add("1999", TP1990s);
            SortingRulesSS.Add("2000", TP2000s);
            SortingRulesSS.Add("2001", TP2000s);
            SortingRulesSS.Add("2002", TP2000s);
            SortingRulesSS.Add("2003", TP2000s);
            SortingRulesSS.Add("2004", TP2000s);
            SortingRulesSS.Add("2005", TP2000s);
            SortingRulesSS.Add("2006", TP2000s);
            SortingRulesSS.Add("2007", TP2000s);
            SortingRulesSS.Add("2008", TP2000s);
            SortingRulesSS.Add("2009", TP2000s);
            SortingRulesSS.Add("2010", TP2010s);
            SortingRulesSS.Add("2011", TP2010s);
            SortingRulesSS.Add("2012", TP2010s);
            SortingRulesSS.Add("2013", TP2010s);
            SortingRulesSS.Add("2014", TP2010s);
            SortingRulesSS.Add("2015", TP2010s);
            SortingRulesSS.Add("2016", TP2010s);
            SortingRulesSS.Add("2017", TP2010s);
            SortingRulesSS.Add("2018", TP2010s);
            SortingRulesSS.Add("2019", TP2010s);
            SortingRulesSS.Add("apocalyp", TPDystopiApoc);
            SortingRulesSS.Add("baroque", TPBaroque);
            SortingRulesSS.Add("colonial", TPColonial);
            SortingRulesSS.Add("cyber", TPCyberpunk);
            SortingRulesSS.Add("cyberpunk", TPCyberpunk);
            SortingRulesSS.Add("cyfy", TPCyberpunk);
            SortingRulesSS.Add("grunge", TPGrunge);
            SortingRulesSS.Add("medieval", TPMedieval);
            SortingRulesSS.Add("renaissance", TPRenaissance);
            SortingRulesSS.Add("rococo", TPRococo);
            SortingRulesSS.Add("shabby", TPGrunge);
            SortingRulesSS.Add("steampunk", TPSteampunk);
            SortingRulesSS.Add("TSM", TPMedieval);
            SortingRulesSS.Add("tudor", TPTudors);
            SortingRulesSS.Add("victorian", TPVictorian);			

            SortingRulesSS.Add("11", "_Possibly Historical");
            SortingRulesSS.Add("11th", "_Possibly Historical");
            SortingRulesSS.Add("12", "_Possibly Historical");
            SortingRulesSS.Add("12th", "_Possibly Historical");
            SortingRulesSS.Add("13th", "_Possibly Historical");
            SortingRulesSS.Add("14", "_Possibly Historical");
            SortingRulesSS.Add("14th", "_Possibly Historical");
            SortingRulesSS.Add("15", "_Possibly Historical");
            SortingRulesSS.Add("15th", "_Possibly Historical");
            SortingRulesSS.Add("16", "_Possibly Historical");
            SortingRulesSS.Add("16th", "_Possibly Historical");
            SortingRulesSS.Add("17", "_Possibly Historical");
            SortingRulesSS.Add("17th", "_Possibly Historical");
            SortingRulesSS.Add("18", "_Possibly Historical");
            SortingRulesSS.Add("18th", "_Possibly Historical");
            SortingRulesSS.Add("19", "_Possibly Historical");
            SortingRulesSS.Add("19th", "_Possibly Historical");
            SortingRulesSS.Add("ancient", "_Possibly Historical");
            SortingRulesSS.Add("egyptian", "_Possibly Historical");
            SortingRulesSS.Add("georgian", "_Possibly Historical");
            SortingRulesSS.Add("greece", "_Possibly Historical");
            SortingRulesSS.Add("roman", "_Possibly Historical");
            SortingRulesSS.Add("rustic", "_Possibly Historical");
            SortingRulesSS.Add("valhallan", "_Possibly Historical");
            SortingRulesSS.Add("vintage", "_Possibly Historical");
            SortingRulesSS.Add("ingeli", "_Possibly Historical");
            SortingRulesSS.Add("moriel", "_Possibly Historical");
            SortingRulesSS.Add("reminisims", "_Possibly Historical");
            



            #endregion



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
                    typesortA = thisPackage.OverriddenItems[0];
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
                foreach (KeyValuePair<string, string> item in SortingRulesGeneral){
                    if (thisPackage.PackageName.Contains(item.Key)){
                        typesortB = Path.Combine(typesortA, item.Value);    
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
                foreach (KeyValuePair<string, string> item in SortingRulesSS){
                    if (thisPackage.PackageName.Contains(item.Key)){
                        typesortB = Path.Combine(typesortA, item.Value);    
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