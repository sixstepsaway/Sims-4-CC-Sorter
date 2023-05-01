using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Reflection;
using SimsCCManager.Packages.Containers;
using System.Text.Json;
using System.Text.Json.Serialization;



namespace SSAGlobals {

    public class typeList {
        public string desc;
        public string typeID;
        public string info;
    }
    public class SortingValues {
        string name {set; get;}        
    }

    public class TypeListings {

        public static List<typeList> AllTypesS2;
        public static List<typeList> AllTypesS3;
        public static List<typeList> AllTypesS4;
        public static List<FunctionSortList> S2BuyFunctionSort;
        public static List<FunctionSortList> S2BuildFunctionSort;
        public static List<FunctionSortList> S3BuyFunctionSort;
        public static List<FunctionSortList> S3BuildFunctionSort;
        public static List<FunctionSortList> S4BuyFunctionSort;
        public static List<FunctionSortList> S4BuildFunctionSort;
        
        public List<typeList> createS2TypeList(){
            List<typeList> AllTypes = new List<typeList>();
            AllTypes.Add(new typeList{typeID="6B943B43", desc="2ARY", info="2D Array"});
            AllTypes.Add(new typeList{typeID="2A51171B", desc="3ARY", info="3D Array"});
            AllTypes.Add(new typeList{typeID="AC06A676", desc="5DS", info="Lighting (Draw State Light)"});
            AllTypes.Add(new typeList{typeID="6A97042F", desc="5EL", info="Lighting (Environment Cube Light)"});
            AllTypes.Add(new typeList{typeID="AC06A66F", desc="5LF", info="Lighting (Linear Fog Light)"});
            AllTypes.Add(new typeList{typeID="25232B11", desc="5SC", info="Scene Node"});
            AllTypes.Add(new typeList{typeID="FB00791E", desc="ANIM", info="Animation Resource"});
            AllTypes.Add(new typeList{typeID="42434F4E", desc="BCON", info="Behaviour Constant"});
            AllTypes.Add(new typeList{typeID="42484156", desc="BHAV", info="Behaviour Function"});
            AllTypes.Add(new typeList{typeID="424D505F", desc="BMP", info="Bitmaps"});
            AllTypes.Add(new typeList{typeID="856DDBAC", desc="BMP", info="Bitmaps"});
            AllTypes.Add(new typeList{typeID="43415453", desc="CATS", info="Catalog String"});
            AllTypes.Add(new typeList{typeID="43494745", desc="CIGE", info="Image Link"});
            AllTypes.Add(new typeList{typeID="4D51F042", desc="CINE", info="Cinematic Scenes"});
            AllTypes.Add(new typeList{typeID="CDB467B8", desc="CREG", info="Content Registry"});
            AllTypes.Add(new typeList{typeID="E519C933", desc="CRES", info="Resource Node"});
            AllTypes.Add(new typeList{typeID="43545353", desc="CTSS", info="Catalog Description"});
            AllTypes.Add(new typeList{typeID="44475250", desc="DGRP", info="Drawgroup"});
            AllTypes.Add(new typeList{typeID="E86B1EEF", desc="DIR", info="Directory of Compressed Files"});
            AllTypes.Add(new typeList{typeID="46414345", desc="FACE", info="Face Properties"});
            AllTypes.Add(new typeList{typeID="46414D68", desc="FAMh", info="Family Data"});
            AllTypes.Add(new typeList{typeID="46414D49", desc="FAMI", info="Family Information"});
            AllTypes.Add(new typeList{typeID="8C870743", desc="FAMt", info="Family Ties"});
            AllTypes.Add(new typeList{typeID="46434E53", desc="FCNS", info="Global Tuning Values"});
            AllTypes.Add(new typeList{typeID="AB4BA572", desc="FPL", info="Fence Post Layer"});
            AllTypes.Add(new typeList{typeID="46574156", desc="FWAV", info="Audio Reference"});
            AllTypes.Add(new typeList{typeID="EA5118B0", desc="FX", info="Effects Resource Tree"});
            AllTypes.Add(new typeList{typeID="474C4F42", desc="GLOB", info="Glabal Data"});
            AllTypes.Add(new typeList{typeID="AC4F8687", desc="GMDC", info="Geometric Data Container"});
            AllTypes.Add(new typeList{typeID="7BA3838C", desc="GMND", info="Geometric Node"});
            AllTypes.Add(new typeList{typeID="EBCF3E27", desc="GZPS", info="Property Set"});
            AllTypes.Add(new typeList{typeID="7B1ACFCD", desc="HLS", info="Hitlist (TS2 format)"});
            AllTypes.Add(new typeList{typeID="484F5553", desc="HOUS", info="House Data"});
            AllTypes.Add(new typeList{typeID="4D533EDD", desc="JFIF", info="JPEG/JFIF Image"});
            AllTypes.Add(new typeList{typeID="856DDBAC", desc="JFIF", info="JPEG/JFIF Image"});
            AllTypes.Add(new typeList{typeID="8C3CE95A", desc="JFIF", info="JPEG/JFIF Image"});
            AllTypes.Add(new typeList{typeID="0C7E9A76", desc="JFIF", info="JPEG/JFIF Image"});
            AllTypes.Add(new typeList{typeID="0BF999E7", desc="LDEF", info="Lot or Tutorial Description"});
            AllTypes.Add(new typeList{typeID="C9C81B9B", desc="LGHT", info="Lighting (Ambient Light)"});
            AllTypes.Add(new typeList{typeID="C9C81BA3", desc="LGHT", info="Lighting (Directional Light)"});
            AllTypes.Add(new typeList{typeID="C9C81BA9", desc="LGHT", info="Lighting (Point Light)"});
            AllTypes.Add(new typeList{typeID="C9C81BAD", desc="LGHT", info="Lighting (Spot Light)"});
            AllTypes.Add(new typeList{typeID="ED534136", desc="LIFO", info="Level Information"});
            AllTypes.Add(new typeList{typeID="6C589723", desc="LOT", info="Lot Definition"});
            AllTypes.Add(new typeList{typeID="4B58975B", desc="LTTX", info="Lot Texture"});
            AllTypes.Add(new typeList{typeID="CCCEF852", desc="LxNR", info="Facial Structure"});
            AllTypes.Add(new typeList{typeID="CD7FE87A", desc="MATSHAD", info="Maxis Material Shader"});
            AllTypes.Add(new typeList{typeID="4C697E5A", desc="MMAT", info="Material Override"});
            AllTypes.Add(new typeList{typeID="6F626A74", desc="MOBJT", info="Main Lot Objects"});
            AllTypes.Add(new typeList{typeID="2026960B", desc="MP3", info="MP3 Audio"});
            AllTypes.Add(new typeList{typeID="4E474248", desc="NGBH", info="Neighborhood Data"});
            AllTypes.Add(new typeList{typeID="ABCB5DA4", desc="NHTG", info="Neighbourhood Terrain Geometry"});
            AllTypes.Add(new typeList{typeID="ABD0DC63", desc="NHTR", info="Neighborhood Terrain"});
            AllTypes.Add(new typeList{typeID="EC44BDDC", desc="NHVW", info="Neighborhood View"});
            AllTypes.Add(new typeList{typeID="AC8A7A2E", desc="NID", info="Neighbourhood ID"});
            AllTypes.Add(new typeList{typeID="4E6D6150", desc="NMAP", info="Name Map"});
            AllTypes.Add(new typeList{typeID="4E524546", desc="NREF", info="Name Reference"});
            AllTypes.Add(new typeList{typeID="4F424A44", desc="OBJD", info="Object Data"});
            AllTypes.Add(new typeList{typeID="4F424A66", desc="OBJf", info="Object Functions"});
            AllTypes.Add(new typeList{typeID="4F626A4D", desc="ObJM", info="Object Metadata"});
            AllTypes.Add(new typeList{typeID="FA1C39F7", desc="OBJT", info="Singular Lot Object"});
            AllTypes.Add(new typeList{typeID="4F626A4D", desc="OBMI", info="Object Metadata Imposter"});
            AllTypes.Add(new typeList{typeID="50414C54", desc="PALT", info="Image Color Palette"});
            AllTypes.Add(new typeList{typeID="AACE2EFB", desc="PDAT", info="Person Data (Formerly SDSC/SINF/SDAT)"});
            AllTypes.Add(new typeList{typeID="50455253", desc="PERS", info="Person Status"});
            AllTypes.Add(new typeList{typeID="8CC0A14B", desc="PMAP", info="Predictive Map"});
            AllTypes.Add(new typeList{typeID="856DDBAC", desc="PNG", info="PNG Image"});
            AllTypes.Add(new typeList{typeID="0C900FDB", desc="POOL", info="Pool Surface"});
            AllTypes.Add(new typeList{typeID="2C310F46", desc="Popups", info="Unknown"});
            AllTypes.Add(new typeList{typeID="504F5349", desc="POSI", info="Edith Positional Information (deprecated)"});
            AllTypes.Add(new typeList{typeID="4DCADB7E", desc="XFLR", info="Terrain Texture"});
            AllTypes.Add(new typeList{typeID="50544250", desc="PTBP", info="Package Toolkit"});
            AllTypes.Add(new typeList{typeID="AB9406AA", desc="ROOF", info="Roof"});
            AllTypes.Add(new typeList{typeID="8DB5E4C2", desc="SFX", info="Sound Effects"});
            AllTypes.Add(new typeList{typeID="FC6EB1F7", desc="SHPE", info="Shape"});
            AllTypes.Add(new typeList{typeID="53494D49", desc="SIMI", info="Sim Information"});
            AllTypes.Add(new typeList{typeID="AC506764", desc="SKIN", info="Sim Outfits"});
            AllTypes.Add(new typeList{typeID="534C4F54", desc="SLOT", info="Object Slot"});
            AllTypes.Add(new typeList{typeID="CAC4FC40", desc="SMAP", info="String Map"});
            AllTypes.Add(new typeList{typeID="53505232", desc="SPR2", info="Sprites"});
            AllTypes.Add(new typeList{typeID="2026960B", desc="SPX1", info="SPX Speech"});
            AllTypes.Add(new typeList{typeID="CC364C2A", desc="SREL", info="Sim Relations"});
            AllTypes.Add(new typeList{typeID="53545223", desc="STR#", info="Text String"});
            AllTypes.Add(new typeList{typeID="ACE46235", desc="STXR", info="Surface Texture"});
            AllTypes.Add(new typeList{typeID="CD95548E", desc="SWAF", info="Sim Wants and Fears"});
            AllTypes.Add(new typeList{typeID="54415454", desc="TATT", info="Tree Attributes"});
            AllTypes.Add(new typeList{typeID="856DDBAC", desc="TGA", info="Targa Image"});
            AllTypes.Add(new typeList{typeID="4B58975B", desc="TMAP", info="Lot or Terrain Texture Map"});
            AllTypes.Add(new typeList{typeID="54505250", desc="TPRP", info="Edith SimAntics Behavior Labels"});
            AllTypes.Add(new typeList{typeID="5452434E", desc="TRCN", info="Behavior Constant Labels"});
            AllTypes.Add(new typeList{typeID="54524545", desc="TREE", info="Tree Data"});
            AllTypes.Add(new typeList{typeID="BA353CE1", desc="TSSG", info="The Sims SG System"});
            AllTypes.Add(new typeList{typeID="54544142", desc="TTAB", info="Pie Menu Functions"});
            AllTypes.Add(new typeList{typeID="54544173", desc="TTAs", info="Pie Menu Strings"});
            AllTypes.Add(new typeList{typeID="49596978", desc="TXMT", info="Material Definitions"});
            AllTypes.Add(new typeList{typeID="1C4A276C", desc="TXTR", info="Texture"});
            AllTypes.Add(new typeList{typeID="00000000", desc="UI", info="User Interface"});
            AllTypes.Add(new typeList{typeID="CB4387A1", desc="VERT", info="Vertex Layer"});
            AllTypes.Add(new typeList{typeID="CD95548E", desc="WFR", info="Wants and Fears"});
            AllTypes.Add(new typeList{typeID="0A284D0B", desc="WGRA", info="Wall Graph"});
            AllTypes.Add(new typeList{typeID="8A84D7B0", desc="WLL", info="Wall Layer"});
            AllTypes.Add(new typeList{typeID="49FF7D76", desc="WRLD", info="World Database"});
            AllTypes.Add(new typeList{typeID="B21BE28B", desc="WTHR", info="Weather Info"});
            AllTypes.Add(new typeList{typeID="2026960B", desc="XA", info="XA Audio"});
            AllTypes.Add(new typeList{typeID="8C1580B5", desc="XHTN", info="Hairtone XML"});
            AllTypes.Add(new typeList{typeID="584D544F", desc="XMTO", info="Material Object Class Dump"});
            AllTypes.Add(new typeList{typeID="CCA8E925", desc="XOBJ", info="Object Class Dump"});
            AllTypes.Add(new typeList{typeID="2C1FD8A1", desc="XTOL", info="Texture Overlay XML"});
            AllTypes.Add(new typeList{typeID="0F9F0C21", desc="UNK", info="Unknown (from Nightlife)"});
            AllTypes.Add(new typeList{typeID="8B0C79D6", desc="UNK", info="Unknown"});
            AllTypes.Add(new typeList{typeID="9D796DB4", desc="UNK", info="Unknown"});
            AllTypes.Add(new typeList{typeID="CC2A6A34", desc="UNK", info="Unknown"});
            AllTypes.Add(new typeList{typeID="CC8A6A69", desc="UNK", info="Unknown"});            
            AllTypes.Add(new typeList{typeID="6C4F359D", desc="COLL", info="Collection"});    
            return AllTypes;
        }

        public List<typeList> createS3TypeList(){
            List<typeList> AllTypes = new List<typeList>();
            AllTypes.Add(new typeList{typeID="0x00AE6C67", desc="BONE", info="skcon"});
            AllTypes.Add(new typeList{typeID="0x00B2D882", desc="_IMG", info="dds"});
            AllTypes.Add(new typeList{typeID="0x00B552EA", desc="_SPT", info="tree"});
            AllTypes.Add(new typeList{typeID="0x015A1849", desc="GEOM", info="geom"});
            AllTypes.Add(new typeList{typeID="0x0166038C", desc="NMAP", info="nmap"});
            AllTypes.Add(new typeList{typeID="0x01661233", desc="MODL", info="scene"});
            AllTypes.Add(new typeList{typeID="0x01A527DB", desc="_AUD", info="mm"});
            AllTypes.Add(new typeList{typeID="0x01D0E6FB", desc="VBUF", info=""});
            AllTypes.Add(new typeList{typeID="0x01D0E70F", desc="IBUF", info=""});
            AllTypes.Add(new typeList{typeID="0x01D0E723", desc="VRTF", info=""});
            AllTypes.Add(new typeList{typeID="0x01D0E75D", desc="MATD", info="scene"});
            AllTypes.Add(new typeList{typeID="0x01D0E76B", desc="SKIN", info="scene"});
            AllTypes.Add(new typeList{typeID="0x01D10F34", desc="MLOD", info="scene"});
            AllTypes.Add(new typeList{typeID="0x01EEF63A", desc="_AUD", info="mm"});
            AllTypes.Add(new typeList{typeID="0x02019972", desc="MTST", info="scene"});
            AllTypes.Add(new typeList{typeID="0x021D7E8C", desc="SPT2", info="scene"});
            AllTypes.Add(new typeList{typeID="0x0229684B", desc="VBUF", info=""});
            AllTypes.Add(new typeList{typeID="0x0229684F", desc="IBUF", info=""});
            AllTypes.Add(new typeList{typeID="0x022B756C", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x025C90A6", desc="_CSS", info="css"});
            AllTypes.Add(new typeList{typeID="0x025C95B6", desc="LAYO", info="xml"});
            AllTypes.Add(new typeList{typeID="0x025ED6F4", desc="SIMO", info="xml"});
            AllTypes.Add(new typeList{typeID="0x029E333B", desc="VOCE", info="voicemix"});
            AllTypes.Add(new typeList{typeID="0x02C9EFF2", desc="MIXR", info="audmix"});
            AllTypes.Add(new typeList{typeID="0x02D5DF13", desc="JAZZ", info="jazz"});
            AllTypes.Add(new typeList{typeID="0x02DC343F", desc="OBJK", info="objkey"});
            AllTypes.Add(new typeList{typeID="0x033260E3", desc="TKMK", info="trackmask"});
            AllTypes.Add(new typeList{typeID="0x0333406C", desc="_XML", info="xml"});
            AllTypes.Add(new typeList{typeID="0x033A1435", desc="TXTC", info="compositor"});
            AllTypes.Add(new typeList{typeID="0x033B2B66", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x0341ACC9", desc="TXTF", info="fabric"});
            AllTypes.Add(new typeList{typeID="0x034AEECB", desc="CASP", info="caspart"});
            AllTypes.Add(new typeList{typeID="0x0354796A", desc="TONE", info="skintone"});
            AllTypes.Add(new typeList{typeID="0x03555BA8", desc="TONE", info="hairtone"});
            AllTypes.Add(new typeList{typeID="0x0355E0A6", desc="BOND", info="bonedelta"});
            AllTypes.Add(new typeList{typeID="0x0358B08A", desc="FACE", info="faceblend"});
            AllTypes.Add(new typeList{typeID="0x03B33DDF", desc="ITUN", info="xml"});
            AllTypes.Add(new typeList{typeID="0x03B4C61D", desc="LITE", info="light"});
            AllTypes.Add(new typeList{typeID="0x03D843C2", desc="CCHE", info="cacheentry"});
            AllTypes.Add(new typeList{typeID="0x03D86EA4", desc="DETL", info=""});
            AllTypes.Add(new typeList{typeID="0x03E80CDC", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x0418FE2A", desc="CFEN", info="fence"});
            AllTypes.Add(new typeList{typeID="0x044735DD", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x044AE110", desc="COMP", info="xml"});
            AllTypes.Add(new typeList{typeID="0x046A7235", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x048A166D", desc="UNKN", info=""});
            AllTypes.Add(new typeList{typeID="0x0498DA7E", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x049CA4CD", desc="CSTR", info="stairs"});
            AllTypes.Add(new typeList{typeID="0x04A09283", desc="UNKN", info=""});
            AllTypes.Add(new typeList{typeID="0x04A4D951", desc="WDET", info=""});
            AllTypes.Add(new typeList{typeID="0x04AC5D93", desc="CPRX", info="proxyprod"});
            AllTypes.Add(new typeList{typeID="0x04B30669", desc="CTTL", info="terraintool"});
            AllTypes.Add(new typeList{typeID="0x04C58103", desc="CRAL", info="railing"});
            AllTypes.Add(new typeList{typeID="0x04D82D90", desc="CMRU", info="cachemru"});
            AllTypes.Add(new typeList{typeID="0x04ED4BB2", desc="CTPT", info="terrainpaint"});
            AllTypes.Add(new typeList{typeID="0x04EE6ABB", desc="UNKN", info=""});
            AllTypes.Add(new typeList{typeID="0x04F3CC01", desc="CFIR", info="fireplace"});
            AllTypes.Add(new typeList{typeID="0x04F51033", desc="SBNO", info="binoutfit"});
            AllTypes.Add(new typeList{typeID="0x04F66BCC", desc="UNKN", info=""});
            AllTypes.Add(new typeList{typeID="0x04F88964", desc="SIME", info="simexport"});
            AllTypes.Add(new typeList{typeID="0x051DF2DD", desc="CBLN", info="compblend"});
            AllTypes.Add(new typeList{typeID="0x05512255", desc="UNKN", info=""});
            AllTypes.Add(new typeList{typeID="0x553EAD4", desc="UNKN", info=""});
            AllTypes.Add(new typeList{typeID="0x0563919E", desc="UNKN", info=""});
            AllTypes.Add(new typeList{typeID="0x0580A2B4", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x0580A2B5", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x0580A2B6", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x0580A2CD", desc="SNAP", info="png"});
            AllTypes.Add(new typeList{typeID="0x0580A2CE", desc="SNAP", info="png"});
            AllTypes.Add(new typeList{typeID="0x0580A2CF", desc="SNAP", info="png"});
            AllTypes.Add(new typeList{typeID="0x0580A2B4", desc="THUM", info="png"});
            AllTypes.Add(new typeList{typeID="0x0580A2B5", desc="THUM", info="png"});
            AllTypes.Add(new typeList{typeID="0x0580A2B6", desc="THUM", info="png"});
            AllTypes.Add(new typeList{typeID="0x0589DC44", desc="THUM", info="png"});
            AllTypes.Add(new typeList{typeID="0x0589DC45", desc="THUM", info="png"});
            AllTypes.Add(new typeList{typeID="0x0589DC46", desc="THUM", info="png"});
            AllTypes.Add(new typeList{typeID="0x0589DC46", desc="THUM", info="png"});
            AllTypes.Add(new typeList{typeID="0x0591B1AF", desc="UPST", info="usercastpreset"});
            AllTypes.Add(new typeList{typeID="0x05B17698", desc="THUM", info="png"});
            AllTypes.Add(new typeList{typeID="0x05B17699", desc="THUM", info="png"});
            AllTypes.Add(new typeList{typeID="0x05B1769A", desc="THUM", info="png"});
            AllTypes.Add(new typeList{typeID="0x05B1B524", desc="THUM", info="png"});
            AllTypes.Add(new typeList{typeID="0x05B1B525", desc="THUM", info="png"});
            AllTypes.Add(new typeList{typeID="0x05B1B526", desc="THUM", info="png"});
            AllTypes.Add(new typeList{typeID="0x05CD4BB3", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x05DA8AF6", desc="WBND", info=""});
            AllTypes.Add(new typeList{typeID="0x05E4FAF7", desc="UNKN", info=""});
            AllTypes.Add(new typeList{typeID="0x05ED1226", desc="REFS", info="references"});
            AllTypes.Add(new typeList{typeID="0x05FF3549", desc="UNKN", info=""});
            AllTypes.Add(new typeList{typeID="0x05FF6BA4", desc="2ARY", info="bnry"});
            AllTypes.Add(new typeList{typeID="0x0604ABDA", desc="DMTR", info="dreamtree"});
            AllTypes.Add(new typeList{typeID="0x060B390C", desc="CWAT", info="water"});
            AllTypes.Add(new typeList{typeID="0x060E1826", desc="UNKN", info=""});
            AllTypes.Add(new typeList{typeID="0x0611B0E7", desc="UNKN", info=""});
            AllTypes.Add(new typeList{typeID="0x062853A8", desc="FAMD", info="household"});
            AllTypes.Add(new typeList{typeID="0x062C8204", desc="BBLN", info="filen"});
            AllTypes.Add(new typeList{typeID="0x062E9EE0", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x06302271", desc="CINF", info="color"});
            AllTypes.Add(new typeList{typeID="0x063261DA", desc="HINF", info="haircolor"});
            AllTypes.Add(new typeList{typeID="0x06326213", desc="OBCI", info="objcolor"});
            AllTypes.Add(new typeList{typeID="0x06393F5D", desc="UNKN", info=""});
            AllTypes.Add(new typeList{typeID="0x065B8B38", desc="UNKN", info=""});
            AllTypes.Add(new typeList{typeID="0x065BFCAC", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x065BFCAD", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x065BFCAE", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x0668F628", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x0668F630", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x0668F635", desc="TWNI", info="png"});
            AllTypes.Add(new typeList{typeID="0x0668F639", desc="TWNP", info="imgpath"});
            AllTypes.Add(new typeList{typeID="0x067CAA11", desc="BGEO", info="blendgeom"});
            AllTypes.Add(new typeList{typeID="0x06B981ED", desc="OBJS", info="objs"});
            AllTypes.Add(new typeList{typeID="0x06CE4804", desc="META", info=""});
            AllTypes.Add(new typeList{typeID="0x06D6B112", desc="UNKN", info=""});
            AllTypes.Add(new typeList{typeID="0x06DC847E", desc="UNKN", info=""});
            AllTypes.Add(new typeList{typeID="0x073FAA07", desc="S3SA", info="s3sa"});
            AllTypes.Add(new typeList{typeID="0x07046B39", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x07CD07EC", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x0A36F07A", desc="CCFP", info="fountain"});
            AllTypes.Add(new typeList{typeID="0x0C37A5B5", desc="LOOK", info="lookuptab"});
            AllTypes.Add(new typeList{typeID="0x0C07456D", desc="COLL", info=""});
            AllTypes.Add(new typeList{typeID="0x11E32896", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x16B17A6C", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x1F886EAD", desc="_INI", info="ini"});
            AllTypes.Add(new typeList{typeID="0x220557DA", desc="STBL", info="stbl"});
            AllTypes.Add(new typeList{typeID="0x2AD195F2", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x2653E3C8", desc="THUM", info="png"});
            AllTypes.Add(new typeList{typeID="0x2653E3C9", desc="THUM", info="png"});
            AllTypes.Add(new typeList{typeID="0x2653E3CA", desc="THUM", info="png"});
            AllTypes.Add(new typeList{typeID="0x2D4284F0", desc="THUM", info="png"});
            AllTypes.Add(new typeList{typeID="0x2D4284F1", desc="THUM", info="png"});
            AllTypes.Add(new typeList{typeID="0x2D4284F2", desc="THUM", info="png"});
            AllTypes.Add(new typeList{typeID="0x2E75C764", desc="ICON", info="png"});
            AllTypes.Add(new typeList{typeID="0x2E75C765", desc="ICON", info="png"});
            AllTypes.Add(new typeList{typeID="0x2E75C766", desc="ICON", info="png"});
            AllTypes.Add(new typeList{typeID="0x2E75C767", desc="ICON", info="png"});
            AllTypes.Add(new typeList{typeID="0x2F7D0002", desc="IMAG", info="png"});
            AllTypes.Add(new typeList{typeID="0x2F7D0004", desc="IMAG", info="png"});
            AllTypes.Add(new typeList{typeID="0x2F7D0008", desc="UITX", info="uitexture"});
            AllTypes.Add(new typeList{typeID="0x312E7545", desc="UNKN", info=""});
            AllTypes.Add(new typeList{typeID="0x316C78F2", desc="CFND", info="foundation"});
            AllTypes.Add(new typeList{typeID="0x319E4F1D", desc="OBJD", info="object"});
            AllTypes.Add(new typeList{typeID="0x32C83095", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x342778A7", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x342779A7", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x34E5247C", desc="UNKN", info=""});
            AllTypes.Add(new typeList{typeID="0x35A33E29", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x3A65AF29", desc="MINF", info="makeup"});
            AllTypes.Add(new typeList{typeID="0x3D8632D0", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x4D1A5589", desc="OBJN", info="objn"});
            AllTypes.Add(new typeList{typeID="0x4F09F8E1", desc="UNKN", info=""});
            AllTypes.Add(new typeList{typeID="0x515CA4CD", desc="CWAL", info="wall"});
            AllTypes.Add(new typeList{typeID="0x54372472", desc="TSNP", info="png"});
            AllTypes.Add(new typeList{typeID="0x5DE9DBA0", desc="THUM", info="png"});
            AllTypes.Add(new typeList{typeID="0x5DE9DBA1", desc="THUM", info="png"});
            AllTypes.Add(new typeList{typeID="0x5DE9DBA2", desc="THUM", info="png"});
            AllTypes.Add(new typeList{typeID="0x626F60CC", desc="THUM", info="png"});
            AllTypes.Add(new typeList{typeID="0x626F60CD", desc="THUM", info="png"});
            AllTypes.Add(new typeList{typeID="0x626F60CE", desc="THUM", info="png"});
            AllTypes.Add(new typeList{typeID="0x628A788F", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x63A33EA7", desc="ANIM", info="scene"});
            AllTypes.Add(new typeList{typeID="0x6ABFAD26", desc="UNKN", info=""});
            AllTypes.Add(new typeList{typeID="0x6B20C4F3", desc="CLIP", info="animation"});
            AllTypes.Add(new typeList{typeID="0x6B6D837D", desc="SNAP", info="png"});
            AllTypes.Add(new typeList{typeID="0x6B6D837E", desc="SNAP", info="png"});
            AllTypes.Add(new typeList{typeID="0x6B6D837F", desc="SNAP", info="png"});
            AllTypes.Add(new typeList{typeID="0x72683C15", desc="STPR", info="sktonepreset"});
            AllTypes.Add(new typeList{typeID="0x736884F1", desc="VPXY", info="vpxy"});
            AllTypes.Add(new typeList{typeID="0x73E93EEB", desc="_XML", info="xml"});
            AllTypes.Add(new typeList{typeID="0x7672F0C5", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x8070223D", desc="AUDT", info="audtun"});
            AllTypes.Add(new typeList{typeID="0x82B43584", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x8EAF13DE", desc="_RIG", info="grannyrig"});
            AllTypes.Add(new typeList{typeID="0x8FFB80F6", desc="_ADS", info="dds"});
            AllTypes.Add(new typeList{typeID="0x90620000", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x90624C1B", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x9063660D", desc="WTXT", info=""});
            AllTypes.Add(new typeList{typeID="0x9063660E", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x913381F2", desc="UNKN", info=""});
            AllTypes.Add(new typeList{typeID="0x9151E6BC", desc="CWST", info="wallstyle"});
            AllTypes.Add(new typeList{typeID="0x91EDBD3E", desc="CRST", info="roofstyle"});
            AllTypes.Add(new typeList{typeID="0x94C5D14A", desc="SIGR", info="signature"});
            AllTypes.Add(new typeList{typeID="0x94EC4B54", desc="UNKN", info=""});
            AllTypes.Add(new typeList{typeID="0xA2377025", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0xA5F9FE18", desc="UNKN", info=""});
            AllTypes.Add(new typeList{typeID="0xA8D58BE5", desc="SKIL", info="xml"});
            AllTypes.Add(new typeList{typeID="0xAE39399F", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0xB074ACE6", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0xB125533A", desc="UNKN", info=""});
            AllTypes.Add(new typeList{typeID="0xB1422971", desc="UNKN", info=""});
            AllTypes.Add(new typeList{typeID="0xB1CC1AF6", desc="_VID", info="mm"});
            AllTypes.Add(new typeList{typeID="0xB4DD716B", desc="_INV", info="inventory"});
            AllTypes.Add(new typeList{typeID="0xB52F5055", desc="FBLN", info="blendunit"});
            AllTypes.Add(new typeList{typeID="0xCF84EC98", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0xCF9A4ACE", desc="MDLR", info="modular"});
            AllTypes.Add(new typeList{typeID="0xD063545B", desc="LDES", info="lotdesc"});
            AllTypes.Add(new typeList{typeID="0xD3044521", desc="RSLT", info="scene"});
            AllTypes.Add(new typeList{typeID="0xD382BF57", desc="FTPT", info="scene"});
            AllTypes.Add(new typeList{typeID="0xD4D9FBE5", desc="PTRN", info="patternlist"});
            AllTypes.Add(new typeList{typeID="0xD84E7FC5", desc="ICON", info="png"});
            AllTypes.Add(new typeList{typeID="0xD84E7FC6", desc="ICON", info="png"});
            AllTypes.Add(new typeList{typeID="0xD84E7FC7", desc="ICON", info="png"});
            AllTypes.Add(new typeList{typeID="0xD9BD0909", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0xDC37E964", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0xDD3223A7", desc="BUFF", info="xml"});
            AllTypes.Add(new typeList{typeID="0xDEA2951C", desc="PETB", info="breed"});
            AllTypes.Add(new typeList{typeID="0xEA5118B0", desc="_SWB", info="effects"});
            AllTypes.Add(new typeList{typeID="0xF0633989", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0xF0FF5598", desc="", info="cam"});
            AllTypes.Add(new typeList{typeID="0xF12E5E12", desc="UNKN", info=""});
            AllTypes.Add(new typeList{typeID="0xF1EDBD86", desc="CRMT", info="roofmatrl"});
            AllTypes.Add(new typeList{typeID="0xF3A38370", desc="NGMP", info="guidmap"});
            AllTypes.Add(new typeList{typeID="0xF609FD60", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0xFCEAB65B", desc="THUM", info="png"});
            return AllTypes;
        }

        public List<typeList> createS4TypeList(){
            List<typeList> AllTypes = new List<typeList>();
            AllTypes.Add(new typeList{typeID="0x00B2D882", desc="_IMG ", info="dds "});
            AllTypes.Add(new typeList{typeID="0x00DE5AC5", desc="", info="rmi "});
            AllTypes.Add(new typeList{typeID="0x010FAF71", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x015A1849", desc="GEOM ", info="geom"});
            AllTypes.Add(new typeList{typeID="0x0166038C", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x01661233", desc="MODL ", info="scene "});
            AllTypes.Add(new typeList{typeID="0x01942E2C", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x01A527DB", desc="_AUD ", info="mm "});
            AllTypes.Add(new typeList{typeID="0x01D0E75D", desc="MATD ", info=""});
            AllTypes.Add(new typeList{typeID="0x01D10F34", desc="MLOD ", info="scene "});
            AllTypes.Add(new typeList{typeID="0x01EEF63A", desc="_AUD ", info="mm "});
            AllTypes.Add(new typeList{typeID="0x02019972", desc="MTST ", info=""});
            AllTypes.Add(new typeList{typeID="0x025ED6F4", desc="SIMO ", info=""});
            AllTypes.Add(new typeList{typeID="0x02D5DF13", desc="JAZZ ", info="jazz "});
            AllTypes.Add(new typeList{typeID="0x033260E3", desc="TkMk ", info=""});
            AllTypes.Add(new typeList{typeID="0x0333406C", desc="_XML ", info="xml "});
            AllTypes.Add(new typeList{typeID="0x033B2B66", desc="UNKN ", info=""});
            AllTypes.Add(new typeList{typeID="0x034AEECB", desc="CASP ", info="caspart "});
            AllTypes.Add(new typeList{typeID="0x0354796A", desc="TONE ", info="skintone "});
            AllTypes.Add(new typeList{typeID="0x0355E0A6", desc="BOND ", info="bonedelta "});
            AllTypes.Add(new typeList{typeID="0x03B4C61D", desc="LITE ", info=""});
            AllTypes.Add(new typeList{typeID="0x0418FE2A", desc="CFEN ", info=""});
            AllTypes.Add(new typeList{typeID="0x067CAA11", desc="BGEO ", info="blendgeom "});
            AllTypes.Add(new typeList{typeID="0x07936CE0", desc="SPLT ", info="CTProductBlock "});
            AllTypes.Add(new typeList{typeID="0x0A227BCF", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x0C772E27", desc="_XML ", info=""});
            AllTypes.Add(new typeList{typeID="0x0D338A3A", desc="THUM ", info=""});
            AllTypes.Add(new typeList{typeID="0x105205BA", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x122FC66A", desc="_XML ", info="lottypeeventmap "});
            AllTypes.Add(new typeList{typeID="0x12952634", desc="LDNB ", info=""});
            AllTypes.Add(new typeList{typeID="0x153D2219", desc="UNKN ", info=""});
            AllTypes.Add(new typeList{typeID="0x16CA6BC4", desc="_THM ", info=""});
            AllTypes.Add(new typeList{typeID="0x17C0C281", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x18F3C673", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x19301120", desc="WCLR ", info=""});
            AllTypes.Add(new typeList{typeID="0x1A8506C5", desc="_XML ", info="mmusx? "});
            AllTypes.Add(new typeList{typeID="0x1B25A024", desc="_XML ", info="sysx "});
            AllTypes.Add(new typeList{typeID="0x1C1CF1F7", desc="CRAL ", info=""});
            AllTypes.Add(new typeList{typeID="0x1C99B344", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x1CC04273", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x1D6DF1CF", desc="CCOL ", info=""});
            AllTypes.Add(new typeList{typeID="0x20D81496", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x220557DA", desc="STBL ", info="stbl"});
            AllTypes.Add(new typeList{typeID="0x25796DCA", desc="_OTF ", info="otf "});
            AllTypes.Add(new typeList{typeID="0x26978421", desc="_CUR ", info="cur "});
            AllTypes.Add(new typeList{typeID="0x276CA4B9", desc="_TTF ", info="ttf "});
            AllTypes.Add(new typeList{typeID="0x2A8A5E22", desc="", info="trayitem "});
            AllTypes.Add(new typeList{typeID="0x2AD195F2", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x2F7D0004", desc="IMAG ", info="png "});
            AllTypes.Add(new typeList{typeID="0x2FAE983E", desc="CFND ", info=""});
            AllTypes.Add(new typeList{typeID="0x319E4F1D", desc="COBJ ", info=""});
            AllTypes.Add(new typeList{typeID="0x3453CF95", desc="RLE2 ", info="DXT5RLE2 "});
            AllTypes.Add(new typeList{typeID="0x370EFD6E", desc="ROOM ", info="room "});
            AllTypes.Add(new typeList{typeID="0x376840D7", desc="AVI ", info="Video "});
            AllTypes.Add(new typeList{typeID="0x3924DE26", desc="_BPT ", info="blueprint "});
            AllTypes.Add(new typeList{typeID="0x3BD45407", desc="_HHI ", info="hhi "});
            AllTypes.Add(new typeList{typeID="0x3BF8FD86", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x3C1AF1F2", desc="THUM ", info="png "});
            AllTypes.Add(new typeList{typeID="0x3C2A8647", desc="THUM ", info="png "});
            AllTypes.Add(new typeList{typeID="0x3D8632D0", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x3EAAA87C", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x3F0C529A", desc="CSPN ", info=""});
            AllTypes.Add(new typeList{typeID="0x4115F9D5", desc="XML ", info="mixbus "});
            AllTypes.Add(new typeList{typeID="0x48C28979", desc="CLCT ", info=""});
            AllTypes.Add(new typeList{typeID="0x4F726BBE", desc="FTPT ", info="footprint "});
            AllTypes.Add(new typeList{typeID="0x545AC67A", desc="DATA ", info="data "});
            AllTypes.Add(new typeList{typeID="0x56278554", desc="_SGI ", info="sgi "});
            AllTypes.Add(new typeList{typeID="0x5B282D45", desc="THUM ", info="png"});
            AllTypes.Add(new typeList{typeID="0x5BE29703", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x6017E896", desc="_XML ", info="buff "});
            AllTypes.Add(new typeList{typeID="0x62E94D38", desc="", info="xml "});
            AllTypes.Add(new typeList{typeID="0x62ECC59A", desc="GFX ", info="gfx "});
            AllTypes.Add(new typeList{typeID="0x6B20C4F3", desc="CLIP ", info=""});
            AllTypes.Add(new typeList{typeID="0x6DFF1A66", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x6F40796A", desc="WRPR ", info=""});
            AllTypes.Add(new typeList{typeID="0x71A449C9", desc="SKYB ", info=""});
            AllTypes.Add(new typeList{typeID="0x71BDB8A2", desc="STLK ", info="sfpusp? "});
            AllTypes.Add(new typeList{typeID="0x729F6C4F", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x73CB32C2", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x74050B1F", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x76BCF80C", desc="TRIM ", info=""});
            AllTypes.Add(new typeList{typeID="0x78C8BCE4", desc="WRMF ", info=""});
            AllTypes.Add(new typeList{typeID="0x810A102D", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x81CA1A10", desc="MTBL ", info=""});
            AllTypes.Add(new typeList{typeID="0x84C23219", desc="CFLT ", info=""});
            AllTypes.Add(new typeList{typeID="0x892C4B8A", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x8B18FF6E", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x8EAF13DE", desc="_RIG ", info="skeleton "});
            AllTypes.Add(new typeList{typeID="0x90624C1B", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x9063660D", desc="WTXT ", info="World texture map "});
            AllTypes.Add(new typeList{typeID="0x9063660E", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x91568FD8", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x91EDBD3E", desc="CRST ", info=""});
            AllTypes.Add(new typeList{typeID="0x9917EACD", desc="RNDP ", info=""});
            AllTypes.Add(new typeList{typeID="0x99D98089", desc="XML ", info="imusx? "});
            AllTypes.Add(new typeList{typeID="0x9A20CD1C", desc="CSTR ", info=""});
            AllTypes.Add(new typeList{typeID="0x9AFE47F5", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x9C925813", desc="THUM ", info="png "});
            AllTypes.Add(new typeList{typeID="0x9D1AB874", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0x9F5CFF10", desc="CSTL ", info=""});
            AllTypes.Add(new typeList{typeID="0xA0451CBD", desc="CPMP ", info=""});
            AllTypes.Add(new typeList{typeID="0xA057811C", desc="CFRZ ", info=""});
            AllTypes.Add(new typeList{typeID="0xA1FF2FC4", desc="THUM ", info="png "});
            AllTypes.Add(new typeList{typeID="0xA576C2E7", desc="XML ", info="mapx "});
            AllTypes.Add(new typeList{typeID="0xA5DFFCF3", desc="CPLT ", info=""});
            AllTypes.Add(new typeList{typeID="0xA680EA4B", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0xA8F7B517", desc="CWNS ", info="bnry "});
            AllTypes.Add(new typeList{typeID="0xAC03A936", desc="CMTX ", info="bnry "});
            AllTypes.Add(new typeList{typeID="0xAC16FBEC", desc="RMAP ", info="regionmap "});
            AllTypes.Add(new typeList{typeID="0xAE39399F", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0xB0118C15", desc="TMLT ", info=""});
            AllTypes.Add(new typeList{typeID="0xB0311D0F", desc="CRTR ", info=""});
            AllTypes.Add(new typeList{typeID="0xB3C438F0", desc="", info="householdbinary "});
            AllTypes.Add(new typeList{typeID="0xB4F762C9", desc="CFLR ", info=""});
            AllTypes.Add(new typeList{typeID="0xB61DE6B4", desc="_XML ", info=""});
            AllTypes.Add(new typeList{typeID="0xB6C8B6A0", desc="_IMG ", info="dds "});
            AllTypes.Add(new typeList{typeID="0xB734E44F", desc="FTPT ", info=""});
            AllTypes.Add(new typeList{typeID="0xB8444447", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0xB91E18DB", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0xB93A9915", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0xBA856C78", desc="RLES ", info="DXT5RLES "});
            AllTypes.Add(new typeList{typeID="0xBC4A5044", desc="CLHD ", info=""});
            AllTypes.Add(new typeList{typeID="0xBC80ED59", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0xBDD82221", desc="AUEV ", info=""});
            AllTypes.Add(new typeList{typeID="0xC0084996", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0xC0DB5AE7", desc="OBJD ", info=""});
            AllTypes.Add(new typeList{typeID="0xC202C770", desc="XML ", info="trax "});
            AllTypes.Add(new typeList{typeID="0xC582D2FB", desc="XML ", info="voicex "});
            AllTypes.Add(new typeList{typeID="0xC5F6763E", desc="UNKN ", info=""});
            AllTypes.Add(new typeList{typeID="0xC71CA490", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0xCB5FDDC7", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0xCD9DE247", desc="PNG ", info="png "});
            AllTypes.Add(new typeList{typeID="0xD2DC5BAD", desc="_XML ", info=""});
            AllTypes.Add(new typeList{typeID="0xD3044521", desc="RSLT ", info=""});
            AllTypes.Add(new typeList{typeID="0xD33C281E", desc="", info="bpi "});
            AllTypes.Add(new typeList{typeID="0xD382BF57", desc="FTPT ", info="scene "});
            AllTypes.Add(new typeList{typeID="0xD5F0F921", desc="CWAL ", info=""});
            AllTypes.Add(new typeList{typeID="0xD65DAFF9", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0xD99F5E5C", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0xDB43E069", desc="DMAP ", info=""});
            AllTypes.Add(new typeList{typeID="0xE0ED7129", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0xE2249422", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0xE231B3D8", desc="XML ", info="objmod "});
            AllTypes.Add(new typeList{typeID="0xE7ADA79D", desc="CFTR ", info="CTProductFountainTrim "});
            AllTypes.Add(new typeList{typeID="0xE882D22F", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0xEA5118B0", desc="_SWB ", info=""});
            AllTypes.Add(new typeList{typeID="0xEAA32ADD", desc="UNKN ", info=""});
            AllTypes.Add(new typeList{typeID="0xEBCBB16C", desc="CTPT ", info=""});
            AllTypes.Add(new typeList{typeID="0xF0633989", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0xF1EDBD86", desc="CRPT ", info=""});
            AllTypes.Add(new typeList{typeID="0xFA25B7DE", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0xFCB1A1E4", desc="", info=""});
            AllTypes.Add(new typeList{typeID="0xFD04E3BE", desc="PRPX ", info="propx "});
            AllTypes.Add(new typeList{typeID="0x7FB6AD8A", desc="S4SM ", info="Sims 4 Studio Merged Package Manifest"});

            return AllTypes;
        }
    
        public List<FunctionSortList> createS2buyfunctionsortlist(){
            List<FunctionSortList> s2fs = new List<FunctionSortList>();
            //seating
            s2fs.Add(new FunctionSortList{flagnum = 0, functionsubsortnum = 1, Category = "Seating", Subcategory = "Dining Room"});
            s2fs.Add(new FunctionSortList{flagnum = 0, functionsubsortnum = 2, Category = "Seating", Subcategory = "Living Room"});
            s2fs.Add(new FunctionSortList{flagnum = 0, functionsubsortnum = 4, Category = "Seating", Subcategory = "Sofas"});
            s2fs.Add(new FunctionSortList{flagnum = 0, functionsubsortnum = 8, Category = "Seating", Subcategory = "Beds"});
            s2fs.Add(new FunctionSortList{flagnum = 0, functionsubsortnum = 16, Category = "Seating", Subcategory = "Recreation"});
            s2fs.Add(new FunctionSortList{flagnum = 0, functionsubsortnum = 32, Category = "Seating", Subcategory = "Unknown I"});
            s2fs.Add(new FunctionSortList{flagnum = 0, functionsubsortnum = 64, Category = "Seating", Subcategory = "Unknown II"});
            s2fs.Add(new FunctionSortList{flagnum = 0, functionsubsortnum = 128, Category = "Seating", Subcategory = "Misc"});
            
            //surfaces
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 1, Category = "Surfaces", Subcategory = "Counters"});
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 2, Category = "Surfaces", Subcategory = "Tables"});
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 4, Category = "Surfaces", Subcategory = "End Tables"});
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 8, Category = "Surfaces", Subcategory = "Desks"});
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 16, Category = "Surfaces", Subcategory = "Coffee Tables"});
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 32, Category = "Surfaces", Subcategory = "Shelves"});
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 64, Category = "Surfaces", Subcategory = "Unknown II"});
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 128, Category = "Surfaces", Subcategory = "Misc"});
            
            //Appliances
            s2fs.Add(new FunctionSortList{flagnum = 2, functionsubsortnum = 1, Category = "Appliances", Subcategory = "Cooking"});
            s2fs.Add(new FunctionSortList{flagnum = 2, functionsubsortnum = 2, Category = "Appliances", Subcategory = "Fridges"});
            s2fs.Add(new FunctionSortList{flagnum = 2, functionsubsortnum = 4, Category = "Appliances", Subcategory = "Small"});
            s2fs.Add(new FunctionSortList{flagnum = 2, functionsubsortnum = 8, Category = "Appliances", Subcategory = "Large"});
            s2fs.Add(new FunctionSortList{flagnum = 2, functionsubsortnum = 16, Category = "Appliances", Subcategory = "Unknown I"});
            s2fs.Add(new FunctionSortList{flagnum = 2, functionsubsortnum = 32, Category = "Appliances", Subcategory = "Unknown II"});
            s2fs.Add(new FunctionSortList{flagnum = 2, functionsubsortnum = 64, Category = "Appliances", Subcategory = "Unknown III"});
            s2fs.Add(new FunctionSortList{flagnum = 2, functionsubsortnum = 128, Category = "Appliances", Subcategory = "Misc"});
            //Electronics
            s2fs.Add(new FunctionSortList{flagnum = 3, functionsubsortnum = 1, Category = "Electronics", Subcategory = "Entertainment"});
            s2fs.Add(new FunctionSortList{flagnum = 3, functionsubsortnum = 2, Category = "Electronics", Subcategory = "TV/Computer"});
            s2fs.Add(new FunctionSortList{flagnum = 3, functionsubsortnum = 4, Category = "Electronics", Subcategory = "Audio"});
            s2fs.Add(new FunctionSortList{flagnum = 3, functionsubsortnum = 8, Category = "Electronics", Subcategory = "Small"});
            s2fs.Add(new FunctionSortList{flagnum = 3, functionsubsortnum = 16, Category = "Electronics", Subcategory = "Unknown I"});
            s2fs.Add(new FunctionSortList{flagnum = 3, functionsubsortnum = 32, Category = "Electronics", Subcategory = "Unknown II"});
            s2fs.Add(new FunctionSortList{flagnum = 3, functionsubsortnum = 64, Category = "Electronics", Subcategory = "Unknown III"});
            s2fs.Add(new FunctionSortList{flagnum = 3, functionsubsortnum = 128, Category = "Electronics", Subcategory = "Misc"});
            //Plumbing
            s2fs.Add(new FunctionSortList{flagnum = 4, functionsubsortnum = 1, Category = "Plumbing", Subcategory = "Toilets"});
            s2fs.Add(new FunctionSortList{flagnum = 4, functionsubsortnum = 2, Category = "Plumbing", Subcategory = "Showers"});
            s2fs.Add(new FunctionSortList{flagnum = 4, functionsubsortnum = 4, Category = "Plumbing", Subcategory = "Sinks"});
            s2fs.Add(new FunctionSortList{flagnum = 4, functionsubsortnum = 8, Category = "Plumbing", Subcategory = "Hot Tubs"});
            s2fs.Add(new FunctionSortList{flagnum = 4, functionsubsortnum = 16, Category = "Plumbing", Subcategory = "Unknown I"});
            s2fs.Add(new FunctionSortList{flagnum = 4, functionsubsortnum = 32, Category = "Plumbing", Subcategory = "Unknown II"});
            s2fs.Add(new FunctionSortList{flagnum = 4, functionsubsortnum = 64, Category = "Plumbing", Subcategory = "Unknown III"});
            s2fs.Add(new FunctionSortList{flagnum = 4, functionsubsortnum = 128, Category = "Plumbing", Subcategory = "Misc"});

            //Decorative
            s2fs.Add(new FunctionSortList{flagnum = 5, functionsubsortnum = 1, Category = "Decorative", Subcategory = "Wall Decorations"});
            s2fs.Add(new FunctionSortList{flagnum = 5, functionsubsortnum = 2, Category = "Decorative", Subcategory = "Sculptures"});
            s2fs.Add(new FunctionSortList{flagnum = 5, functionsubsortnum = 4, Category = "Decorative", Subcategory = "Rugs"});
            s2fs.Add(new FunctionSortList{flagnum = 5, functionsubsortnum = 8, Category = "Decorative", Subcategory = "Plants"});
            s2fs.Add(new FunctionSortList{flagnum = 5, functionsubsortnum = 16, Category = "Decorative", Subcategory = "Mirrors"});
            s2fs.Add(new FunctionSortList{flagnum = 5, functionsubsortnum = 32, Category = "Decorative", Subcategory = "Curtains"});
            s2fs.Add(new FunctionSortList{flagnum = 5, functionsubsortnum = 64, Category = "Decorative", Subcategory = "Unknown II"});
            s2fs.Add(new FunctionSortList{flagnum = 5, functionsubsortnum = 128, Category = "Decorative", Subcategory = "Misc"});

            //General
            s2fs.Add(new FunctionSortList{flagnum = 6, functionsubsortnum = 1, Category = "Misc", Subcategory = "Unknown I"});
            s2fs.Add(new FunctionSortList{flagnum = 6, functionsubsortnum = 2, Category = "Misc", Subcategory = "Dressers"});
            s2fs.Add(new FunctionSortList{flagnum = 6, functionsubsortnum = 4, Category = "Misc", Subcategory = "Unknown II"});
            s2fs.Add(new FunctionSortList{flagnum = 6, functionsubsortnum = 8, Category = "Misc", Subcategory = "Party"});
            s2fs.Add(new FunctionSortList{flagnum = 6, functionsubsortnum = 16, Category = "Misc", Subcategory = "Child"});
            s2fs.Add(new FunctionSortList{flagnum = 6, functionsubsortnum = 32, Category = "Misc", Subcategory = "Cars"});
            s2fs.Add(new FunctionSortList{flagnum = 6, functionsubsortnum = 64, Category = "Misc", Subcategory = "Pets"});
            s2fs.Add(new FunctionSortList{flagnum = 6, functionsubsortnum = 128, Category = "Misc", Subcategory = "Misc"});
            //Lighting
            s2fs.Add(new FunctionSortList{flagnum = 7, functionsubsortnum = 1, Category = "Lighting", Subcategory = "Table Lamps"});
            s2fs.Add(new FunctionSortList{flagnum = 7, functionsubsortnum = 2, Category = "Lighting", Subcategory = "Floor Lamps"});
            s2fs.Add(new FunctionSortList{flagnum = 7, functionsubsortnum = 4, Category = "Lighting", Subcategory = "Wall Lamps"});
            s2fs.Add(new FunctionSortList{flagnum = 7, functionsubsortnum = 8, Category = "Lighting", Subcategory = "Ceiling Lamps"});
            s2fs.Add(new FunctionSortList{flagnum = 7, functionsubsortnum = 16, Category = "Lighting", Subcategory = "Outdoor"});
            s2fs.Add(new FunctionSortList{flagnum = 7, functionsubsortnum = 32, Category = "Lighting", Subcategory = "Unknown I"});
            s2fs.Add(new FunctionSortList{flagnum = 7, functionsubsortnum = 64, Category = "Lighting", Subcategory = "Unknown II"});
            s2fs.Add(new FunctionSortList{flagnum = 7, functionsubsortnum = 128, Category = "Lighting", Subcategory = "Misc"});
            //Hobbies
            s2fs.Add(new FunctionSortList{flagnum = 8, functionsubsortnum = 1, Category = "Hobbies", Subcategory = "Creative"});
            s2fs.Add(new FunctionSortList{flagnum = 8, functionsubsortnum = 2, Category = "Hobbies", Subcategory = "Knowledge"});
            s2fs.Add(new FunctionSortList{flagnum = 8, functionsubsortnum = 4, Category = "Hobbies", Subcategory = "Exercise"});
            s2fs.Add(new FunctionSortList{flagnum = 8, functionsubsortnum = 8, Category = "Hobbies", Subcategory = "Recreation"});
            s2fs.Add(new FunctionSortList{flagnum = 8, functionsubsortnum = 16, Category = "Hobbies", Subcategory = "Unknown I"});
            s2fs.Add(new FunctionSortList{flagnum = 8, functionsubsortnum = 32, Category = "Hobbies", Subcategory = "Unknown II"});
            s2fs.Add(new FunctionSortList{flagnum = 8, functionsubsortnum = 64, Category = "Hobbies", Subcategory = "Unknown III"});
            s2fs.Add(new FunctionSortList{flagnum = 8, functionsubsortnum = 128, Category = "Hobbies", Subcategory = "Misc"});
            //Aspiration Rewards
            s2fs.Add(new FunctionSortList{flagnum = 9, functionsubsortnum = 1, Category = "Aspiration Rewards", Subcategory = "Unknown I"});
            s2fs.Add(new FunctionSortList{flagnum = 9, functionsubsortnum = 2, Category = "Aspiration Rewards", Subcategory = "Unknown II"});
            s2fs.Add(new FunctionSortList{flagnum = 9, functionsubsortnum = 4, Category = "Aspiration Rewards", Subcategory = "Unknown III"});
            s2fs.Add(new FunctionSortList{flagnum = 9, functionsubsortnum = 8, Category = "Aspiration Rewards", Subcategory = "Unknown IV"});
            s2fs.Add(new FunctionSortList{flagnum = 9, functionsubsortnum = 16, Category = "Aspiration Rewards", Subcategory = "Unknown V"});
            s2fs.Add(new FunctionSortList{flagnum = 9, functionsubsortnum = 32, Category = "Aspiration Rewards", Subcategory = "Unknown VI"});
            s2fs.Add(new FunctionSortList{flagnum = 9, functionsubsortnum = 64, Category = "Aspiration Rewards", Subcategory = "Unknown VII"});
            s2fs.Add(new FunctionSortList{flagnum = 9, functionsubsortnum = 128, Category = "Aspiration Rewards", Subcategory = "Unknown VIII"});
            //Career Rewards
            s2fs.Add(new FunctionSortList{flagnum = 10, functionsubsortnum = 1, Category = "Career Rewards", Subcategory = "Unknown I"});
            s2fs.Add(new FunctionSortList{flagnum = 10, functionsubsortnum = 2, Category = "Career Rewards", Subcategory = "Unknown II"});
            s2fs.Add(new FunctionSortList{flagnum = 10, functionsubsortnum = 4, Category = "Career Rewards", Subcategory = "Unknown III"});
            s2fs.Add(new FunctionSortList{flagnum = 10, functionsubsortnum = 8, Category = "Career Rewards", Subcategory = "Unknown IV"});
            s2fs.Add(new FunctionSortList{flagnum = 10, functionsubsortnum = 16, Category = "Career Rewards", Subcategory = "Unknown V"});
            s2fs.Add(new FunctionSortList{flagnum = 10, functionsubsortnum = 32, Category = "Career Rewards", Subcategory = "Unknown VI"});
            s2fs.Add(new FunctionSortList{flagnum = 10, functionsubsortnum = 64, Category = "Career Rewards", Subcategory = "Unknown VII"});
            s2fs.Add(new FunctionSortList{flagnum = 10, functionsubsortnum = 128, Category = "Career Rewards", Subcategory = "Unknown VIII"});
            /*//seating
            s2fs.Add(new FunctionSortList{flagnum = 11, functionsubsortnum = 1, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 11, functionsubsortnum = 2, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 11, functionsubsortnum = 4, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 11, functionsubsortnum = 8, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 11, functionsubsortnum = 16, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 11, functionsubsortnum = 32, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 11, functionsubsortnum = 64, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 11, functionsubsortnum = 128, Category = "Seating", Subcategory = ""});
            //seating
            s2fs.Add(new FunctionSortList{flagnum = 12, functionsubsortnum = 1, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 12, functionsubsortnum = 2, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 12, functionsubsortnum = 4, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 12, functionsubsortnum = 8, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 12, functionsubsortnum = 16, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 12, functionsubsortnum = 32, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 12, functionsubsortnum = 64, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 12, functionsubsortnum = 128, Category = "Seating", Subcategory = ""});
            //seating
            s2fs.Add(new FunctionSortList{flagnum = 13, functionsubsortnum = 1, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 13, functionsubsortnum = 2, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 13, functionsubsortnum = 4, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 13, functionsubsortnum = 8, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 13, functionsubsortnum = 16, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 13, functionsubsortnum = 32, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 13, functionsubsortnum = 64, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 13, functionsubsortnum = 128, Category = "Seating", Subcategory = ""});
            //seating
            s2fs.Add(new FunctionSortList{flagnum = 14, functionsubsortnum = 1, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 14, functionsubsortnum = 2, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 14, functionsubsortnum = 4, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 14, functionsubsortnum = 8, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 14, functionsubsortnum = 16, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 14, functionsubsortnum = 32, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 14, functionsubsortnum = 64, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 14, functionsubsortnum = 128, Category = "Seating", Subcategory = ""});
            //seating
            s2fs.Add(new FunctionSortList{flagnum = 15, functionsubsortnum = 1, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 15, functionsubsortnum = 2, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 15, functionsubsortnum = 4, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 15, functionsubsortnum = 8, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 15, functionsubsortnum = 16, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 15, functionsubsortnum = 32, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 15, functionsubsortnum = 64, Category = "Seating", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 15, functionsubsortnum = 128, Category = "Seating", Subcategory = ""});
            */
            return s2fs;
        }
        
        public List<FunctionSortList> createS2buildfunctionsortlist(){
            List<FunctionSortList> s2fs = new List<FunctionSortList>();
            s2fs.Add(new FunctionSortList{flagnum = 8, functionsubsortnum = 1, Category = "Door", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 8, functionsubsortnum = 4, Category = "Window", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 8, functionsubsortnum = 100, Category = "Two Story Door", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 8, functionsubsortnum = 2, Category = "Two Story Window", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 8, functionsubsortnum = 10, Category = "Arch", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 20, Category = "Staircase", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 0, functionsubsortnum = 0, Category = "Fireplaces (?)", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 400, Category = "Garage", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 4, functionsubsortnum = 1, Category = "Trees", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 4, functionsubsortnum = 4, Category = "Flowers", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 4, functionsubsortnum = 10, Category = "Gardening", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 4, functionsubsortnum = 2, Category = "Shrubs", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 1000, Category = "Architecture", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 8, Category = "Column", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 100, Category = "Two Story Column", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 200, Category = "Connecting Column", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 40, Category = "Pools", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 8, functionsubsortnum = 8, Category = "Gates", Subcategory = ""});
            s2fs.Add(new FunctionSortList{flagnum = 1, functionsubsortnum = 800, Category = "Elevator", Subcategory = ""});
            
            

            return s2fs;
        }
        
        /*public List<FunctionSortList> createS3functionsortlist(){
            //List<FunctionSortList> s3fs = new List<FunctionSortList>()            
            //return s2fs;
        }*/
    
    }
    

    public class GlobalVariables {
        public static bool debugMode = true;
        public static string ModFolder;
        public static string logfile;
        public static int gameVer;  
        public static int PackageCount = 0;       
        public static int packagesRead = 1;
        
        public static List<PackageFile> AllPackages = new List<PackageFile>();
        public static List<FileInfo> NotPackages = new List<FileInfo>();
        public static List<FileInfo> PackageFiles = new List<FileInfo>();
        LoggingGlobals log = new LoggingGlobals();


        public void Initialize(int gameNum, string modLocation){
            gameVer = gameNum;
            ModFolder = modLocation;
            logfile = modLocation + "\\SimsCCSorter.log";
            StreamWriter putContentsIntoTxt = new StreamWriter(logfile);
            putContentsIntoTxt.Close();
            InitializeVariables(); 
        }

        TypeListings typeListings = new TypeListings();
        public void InitializeVariables(){
            log.InitializeLog();
            log.MakeLog("Initializing application.", true);
            TypeListings.AllTypesS2 = typeListings.createS2TypeList();
            log.MakeLog("Created sims 2 type list.", true);
            TypeListings.AllTypesS3 = typeListings.createS3TypeList();
            log.MakeLog("Created sims 3 type list.", true);
            TypeListings.AllTypesS4 = typeListings.createS4TypeList();
            log.MakeLog("Created sims 4 type list.", true);
            TypeListings.S2BuyFunctionSort = typeListings.createS2buyfunctionsortlist();
            log.MakeLog("Created sims 2 buy function sort list.", true);
            TypeListings.S2BuildFunctionSort = typeListings.createS2buildfunctionsortlist();
            log.MakeLog("Created sims 2 build function sort.", true);            
            log.MakeLog("Finished initializing.", true);
        }       
    }

    public class LoggingGlobals
    {
        public static bool firstrunmain = true;
        public static bool firstrundebug = true;
        public static string mydocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string internalLogFolder = mydocs + "\\Sims CC Manager\\logs";
        private static string debuglog = internalLogFolder + "\\debug.log";        
        static ReaderWriterLock locker = new ReaderWriterLock();
        //Function for logging to the logfile set at the start of the program
        public void InitializeLog() {
            Methods.MakeFolder(internalLogFolder);
            StreamWriter addToInternalLog = new StreamWriter (debuglog, append: false);
            addToInternalLog.WriteLine("Initializing internal log file.");
            addToInternalLog.Close();
            StreamWriter addToLog = new StreamWriter (GlobalVariables.logfile, append: false);
            addToLog.WriteLine("Initializing log file.");
            addToLog.Close();
        }
        public void MakeLog (string Statement, bool debug, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "")
        {   
            string time = "";
            string statement = "";
            FileInfo filepath = new FileInfo(filePath);
            if (debug) {
                try
                {
                    time = DateTime.Now.ToString("h:mm:ss tt");
                    statement = "[L" + lineNumber + " | " + filepath.Name + "] " + time + ": " + Statement;
                    locker.AcquireWriterLock(int.MaxValue); 
                    System.IO.File.AppendAllLines(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\", ""), debuglog), new[] { statement });
                }
                finally
                {
                    locker.ReleaseWriterLock();
                }
            } else {
                try
                {
                    time = DateTime.Now.ToString("h:mm:ss tt");
                    statement = time + ": " + Statement;
                    locker.AcquireWriterLock(int.MaxValue); 
                    System.IO.File.AppendAllLines(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\", ""), GlobalVariables.logfile), new[] { statement });
                }
                finally
                {
                    locker.ReleaseWriterLock();
                }
            }            
        }
    }

    public class Methods {
        public static void MakeFolder (string directory)
        {
            try
            {
                // Determine whether the directory exists.
                if (Directory.Exists(directory))
                {
                    return;
                }

                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(directory);
            }
            catch (Exception e)
            {
                
            }
            finally {}
            }
    }

    public class SortingGroupsJSON {
        public string type {get; set;}
        public string folder {get; set;}
        public string matches {get; set;}
    }

    public class OutsideInformation {
        /*public void WriteJSON()
        {
            var weatherForecast = new WeatherForecast
            {
                Date = DateTime.Parse("2019-08-01"),
                TemperatureCelsius = 25,
                Summary = "Hot"
            };

            string fileName = "WeatherForecast.json"; 
            string jsonString = JsonSerializer.Serialize(weatherForecast);
            File.WriteAllText(fileName, jsonString);

            Console.WriteLine(File.ReadAllText(fileName));
        }*/

        public void ReadJSON()
        {
            string fileName = LoggingGlobals.mydocs + "\\data\\Sorting.json";
            string jsonString = File.ReadAllText(fileName);
            SortingGroupsJSON SortingGroups = JsonSerializer.Deserialize<SortingGroupsJSON>(jsonString)!;

            
        }

    }
}