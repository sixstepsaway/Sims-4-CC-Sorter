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
using System.Runtime.CompilerServices;
using SSAGlobals;
using SimsCCManager.Packages.Containers;
using Newtonsoft.Json;
using SimsCCManager.Packages.Initial;
using SimsCCManager.Packages.Sims2Search;
using SimsCCManager.Packages.Sims3Search;
using SimsCCManager.Packages.Sims4Search;
using SimsCCManager.Misc.TrayReader;
using System.Data.SQLite;

namespace SimsCCManager.CMD
{     
    class Program { 
        static void Main(string[] args)
        {
            GlobalVariables globals = new GlobalVariables();
            TypeListings typeListings = new TypeListings();
            InitialProcessing initial = new InitialProcessing();
            LoggingGlobals log = new LoggingGlobals();
            ParallelOptions parallelSettings = new ParallelOptions() { MaxDegreeOfParallelism = 2000};                
            S2PackageSearch sims2s = new S2PackageSearch();
            S3PackageSearch sims3s = new S3PackageSearch();
            S4PackageSearch sims4s = new S4PackageSearch();
            TypeListings.AllTypesS2 = typeListings.createS2TypeList();
            TypeListings.AllTypesS3 = typeListings.createS3TypeList();
            TypeListings.AllTypesS4 = typeListings.createS4TypeList();
            TypeListings.S2BuyFunctionSort = typeListings.createS2buyfunctionsortlist();
            TypeListings.S2BuildFunctionSort = typeListings.createS2buildfunctionsortlist();
            

            

            string folder = "M:\\The Sims 4 (Documents)\\!UnmergedCC\\To Sort\\CURRENT TEST";
            
            //string folder = "M:\\The Sims 4 (Documents)\\!UnmergedCC\\To Sort";
            //string folder = "C:\\Program Files (x86)\\Origin Games\\The Sims 4\\";
                       
            globals.Initialize(folder);
            initial.IdentifyPackages();

            Parallel.ForEach(GlobalVariables.justPackageFiles, parallelSettings, file => 
            {
                initial.FindBrokenPackages(file.FullName);
            });
            
            Parallel.ForEach(GlobalVariables.workingPackageFiles, parallelSettings, file => 
            {
                initial.IdentifyGames(file.Location);
            });

            Parallel.ForEach(GlobalVariables.gamesPackages, parallelSettings, package => {
                if (package.Game == 2){
                    sims2s.SearchS2Packages(package.Location);
                } else if (package.Game == 3){
                    //sims3s.SearchS3Packages(package.Location);
                } else if (package.Game == 4){
                    sims4s.SearchS4Packages(package.Location, false);
                }
            });

            /*foreach (PackageFile package in GlobalVariables.gamesPackages){
                if (package.Game == 3){
                    //sims3s.SearchS3Packages(package.Location);
                } else if (package.Game == 4){
                    sims4s.SearchS4Packages(package.Location, false);
                }
            }*/

            
            foreach (SimsPackage pack in Containers.allSims4Packages){
                log.MakeLog(pack.ToString(), false);
            }

            /*TrayFilesReader tfr = new TrayFilesReader();
            string folder = @"M:\The Sims 4 (Documents)\!Current\The Sims 4\Tray";
            globals.Initialize(0, folder); 
            string[] files = Directory.GetFiles(folder);

            foreach (string file in files){
                FileInfo afile = new FileInfo(file);
                if (afile.Extension == ".householdbinary"){
                    tfr.ReadTray(afile);
                }                
            }*/

            /*Console.WriteLine("Hello!");
            string databasepath = @"I:\Code\C#\Sims-CC-Sorter\Console\src\SimsCCManager.Console\data\Sims4_BaseGame_Instances.sqlite";
            string cs = string.Format("Data Source={0}", databasepath);
            string[] typelists = new string[]{
                "Ambience","AnimationBoundaryConditionCache","AnimationConstraintCache","AnimationStateMachine","ApartmentThumbnail1","ApartmentThumbnail2","AudioConfiguration","AudioEffects","AudioVocals","AVI","BatchFixHistory","BlendGeometry","Block","BlueprintImage","Blueprint","BodypartThumbnail","BonePose","BuyBuildThumbnail","CasPartThumbnail","CASPreset","ClipDataList","ClipExtraData","ClipHeader","Clip","CMRF","ColorBlendedTerrain","ColorList","ColorTimelineData","Column","CombinedBinaryTuning","Credits","Cursor","CutoutInfoTable","DDSImage","DecoTrim","DeformerMap","DSTImage","Fence","Floor","FloorTrim","FontConfiguration","Footprint3","Footprint","Foundation","FountainTrim","Frieze","GenericMTX","Geometry","GpIni","HalfWall","HalfWallTrim","HouseholdDescription","HouseholdTemplate","IndexBuffer","IndexBufferShadow","Ladder","Light","LocomotionBuilder","LocomotionConfig","LotDescription","LotFootprintReference","LotPreviewThumbnail","LotTypeEventMap","LRLEImage","Magalog","MagazineCollection","MaterialDefinition","MaterialSet","MaxisWorldPipeline1","MaxisWorldPipeline2","Misc1","ModalMusicMapping","Model","ModelCutout","ModelLOD","ModelVertexFormat","ModularPiece","MTXCatalog","MusicData","NameMap","ObjectCatalog","ObjectCatalogSet","ObjectDefinition","ObjectModifiers","OpenTypeFont","PetBreedCoatPatternThumbnail","PetCoatBrush","PetCoatPattern","PetFacePresetThumbnail","PetPeltLayer","Platform","PNGImage","Pond","PoolTrim","QueryableWorldMaskManifest","QueryableWorldMask","Railing","RegionDescription","RegionMap","Rig","RLE2Image","RLESImage","RNDP","RoadDefinition","RoofPattern","Roof","RoofTrim","Room","RoomThumbnail","SaveGameData","SaveThumbnail1","SaveThumbnail2","SaveThumbnail3","SaveThumbnail4","SaveThumbnail5","ScaleFormGFX","SculpSetList","Sculpt","SimBustThumbnail","SimData","SimFeaturedOutfitThumbnail","SimGalleryImage","SimHotspotControl","SimHouseholdThumbnail","SimInfo","SimMannequinOutfitThumbnail","SimModifier","SimPortraitThumbnail","SimPreset","SimPresetThumbnail","SimThumbnail","SimTravelThumbnail","SkinController","Skintone","SkyBoxTextureData","Slot","SoundMixProperties","SoundModifierMapping","SoundProperties","Spandrel","Spawner","Stairs","StringTable","StyledLook","Style","TerrainBlendMap","TerrainData","TerrainHeightMap","TerrainKDTree","TerrainMesh","TerrainPaint","TerrainSizeInfo","TerrainTool","TGAImage","ThumbnailCache","ThumbnailExtra1","ThumbnailExtra2","TrackMask","TrayItem","Trim","TrueTypeFont","UIControlEventMap","UIEventModeMapping","UnknownWorld2","UnknownWorld3","UnknownWorld5","VertexBuffer","VertexBufferShadow","VisualEffectsInstanceMap","VisualEffectsMerged","VisualEffects","VoiceEffect","VoicePlugin","Walkstyle","Wall","WallTrim","WaterManifest","WaterMaskList","WaterMask","WindowSet","WMRFReference","WorldCameraInfo","WorldCameraMesh","WorldConditionalData","WorldData","WorldDescription","WorldFileHeader","WorldLandingStrip","WorldLightsInfo","WorldLotArchitecture","WorldLotObjects","WorldLotParameterInfo","WorldManifest","WorldmapLotThumbnail","WorldMap","WorldObjectData","WorldOffLotMesh","WorldRoadPolys","WorldTimelineColor","WorldVisualEffectsInfo","WorldWaterManifest","ZoneObjectData","Unknown1","Unknown2"
            };
            foreach (string type in typelists){
                using (var dataConnection = new SQLiteConnection(cs)){
                    string cmdtxt = string.Format("CREATE TABLE {0} (override TEXT instance TEXT name TEXT image BLOB)", type);
                    SQLiteCommand sqcmd = new SQLiteCommand(cmdtxt, dataConnection);
                    dataConnection.Open();
                    Console.WriteLine("Creating table for " + type);
                    sqcmd.ExecuteNonQuery();
                    dataConnection.Close();
                }
            }*/
        }
    }
}