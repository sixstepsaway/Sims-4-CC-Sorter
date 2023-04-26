using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.CompilerServices;
using SimsCCManager.Packages.Containers;

namespace SSAGlobals {

    public class typeList {
        public string desc;
        public string typeID;
        public string info;
    }

    public class TypeListings {

        public static List<typeList> AllTypesS2;
        public static List<typeList> AllTypesS3;
        public static List<typeList> AllTypesS4;
        
        typeList atype = new typeList();
        public List<typeList> createS2TypeList(){
            List<typeList> AllTypes = new List<typeList>();
            atype.desc = "2ARY";
            atype.typeID = "6B943B43";
            atype.info = "2D Array";
            AllTypes.Add(atype);
            atype = new typeList();
            
            atype.desc = "3ARY";
            atype.typeID = "2A51171B";
            atype.info = "3D Array";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "5DS";
            atype.typeID = "AC06A676";
            atype.info = "Lighting (Draw State Light)";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "5EL";
            atype.typeID = "6A97042F";
            atype.info = "Lighting (Environment Cube Light)";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "5LF";
            atype.typeID = "AC06A66F";
            atype.info = "Lighting (Linear Fog Light)";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "5SC";
            atype.typeID = "25232B11";
            atype.info = "Scene Node";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "ANIM";
            atype.typeID = "FB00791E";
            atype.info = "Animation Resource";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "BCON";
            atype.typeID = "42434F4E";
            atype.info = "Behaviour Constant";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "BHAV";
            atype.typeID = "42484156";
            atype.info = "Behaviour Function";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "BMP";
            atype.typeID = "424D505F";
            atype.info = "Bitmaps";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "BMP";
            atype.typeID = "856DDBAC";
            atype.info = "Bitmaps";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "CATS";
            atype.typeID = "43415453";
            atype.info = "Catalog String";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "CIGE";
            atype.typeID = "43494745";
            atype.info = "Image Link";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "CINE";
            atype.typeID = "4D51F042";
            atype.info = "Cinematic Scenes";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "CREG";
            atype.typeID = "CDB467B8";
            atype.info = "Content Registry";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "CRES";
            atype.typeID = "E519C933";
            atype.info = "Resource Node";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "CTSS";
            atype.typeID = "43545353";
            atype.info = "Catalog Description";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "DGRP";
            atype.typeID = "44475250";
            atype.info = "Drawgroup";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "DIR";
            atype.typeID = "E86B1EEF";
            atype.info = "Directory of Compressed Files";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "FACE";
            atype.typeID = "46414345";
            atype.info = "Face Properties";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "FAMh";
            atype.typeID = "46414D68";
            atype.info = "Family Data";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "FAMI";
            atype.typeID = "46414D49";
            atype.info = "Family Information";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "FAMt";
            atype.typeID = "8C870743";
            atype.info = "Family Ties";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "FCNS";
            atype.typeID = "46434E53";
            atype.info = "Global Tuning Values";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "FPL";
            atype.typeID = "AB4BA572";
            atype.info = "Fence Post Layer";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "FWAV";
            atype.typeID = "46574156";
            atype.info = "Audio Reference";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "FX";
            atype.typeID = "EA5118B0";
            atype.info = "Effects Resource Tree";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "GLOB";
            atype.typeID = "474C4F42";
            atype.info = "Glabal Data";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "GMDC";
            atype.typeID = "AC4F8687";
            atype.info = "Geometric Data Container";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "GMND";
            atype.typeID = "7BA3838C";
            atype.info = "Geometric Node";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "GZPS";
            atype.typeID = "EBCF3E27";
            atype.info = "Property Set";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "HLS";
            atype.typeID = "7B1ACFCD";
            atype.info = "Hitlist (TS2 format)";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "HOUS";
            atype.typeID = "484F5553";
            atype.info = "House Data";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "JFIF";
            atype.typeID = "4D533EDD";
            atype.info = "JPEG/JFIF Image";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "JFIF";
            atype.typeID = "856DDBAC";
            atype.info = "JPEG/JFIF Image";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "JFIF";
            atype.typeID = "8C3CE95A";
            atype.info = "JPEG/JFIF Image";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "JFIF";
            atype.typeID = "0C7E9A76";
            atype.info = "JPEG/JFIF Image";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "LDEF";
            atype.typeID = "0BF999E7";
            atype.info = "Lot or Tutorial Description";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "LGHT";
            atype.typeID = "C9C81B9B";
            atype.info = "Lighting (Ambient Light)";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "LGHT";
            atype.typeID = "C9C81BA3";
            atype.info = "Lighting (Directional Light)";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "LGHT";
            atype.typeID = "C9C81BA9";
            atype.info = "Lighting (Point Light)";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "LGHT";
            atype.typeID = "C9C81BAD";
            atype.info = "Lighting (Spot Light)";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "LIFO";
            atype.typeID = "ED534136";
            atype.info = "Level Information";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "LOT";
            atype.typeID = "6C589723";
            atype.info = "Lot Definition";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "LTTX";
            atype.typeID = "4B58975B";
            atype.info = "Lot Texture";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "LxNR";
            atype.typeID = "CCCEF852";
            atype.info = "Facial Structure";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "MATSHAD";
            atype.typeID = "CD7FE87A";
            atype.info = "Maxis Material Shader";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "MMAT";
            atype.typeID = "4C697E5A";
            atype.info = "Material Override";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "MOBJT";
            atype.typeID = "6F626A74";
            atype.info = "Main Lot Objects";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "MP3";
            atype.typeID = "2026960B";
            atype.info = "MP3 Audio";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "NGBH";
            atype.typeID = "4E474248";
            atype.info = "Neighborhood Data";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "NHTG";
            atype.typeID = "ABCB5DA4";
            atype.info = "Neighbourhood Terrain Geometry";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "NHTR";
            atype.typeID = "ABD0DC63";
            atype.info = "Neighborhood Terrain";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "NHVW";
            atype.typeID = "EC44BDDC";
            atype.info = "Neighborhood View";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "NID";
            atype.typeID = "AC8A7A2E";
            atype.info = "Neighbourhood ID";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "NMAP";
            atype.typeID = "4E6D6150";
            atype.info = "Name Map";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "NREF";
            atype.typeID = "4E524546";
            atype.info = "Name Reference";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "OBJD";
            atype.typeID = "4F424A44";
            atype.info = "Object Data";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "OBJf";
            atype.typeID = "4F424A66";
            atype.info = "Object Functions";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "ObJM";
            atype.typeID = "4F626A4D";
            atype.info = "Object Metadata";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "OBJT";
            atype.typeID = "FA1C39F7";
            atype.info = "Singular Lot Object";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "OBMI";
            atype.typeID = "4F626A4D";
            atype.info = "Object Metadata Imposter";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "PALT";
            atype.typeID = "50414C54";
            atype.info = "Image Color Palette";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "PDAT";
            atype.typeID = "AACE2EFB";
            atype.info = "Person Data (Formerly SDSC/SINF/SDAT)";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "PERS";
            atype.typeID = "50455253";
            atype.info = "Person Status";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "PMAP";
            atype.typeID = "8CC0A14B";
            atype.info = "Predictive Map";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "PNG";
            atype.typeID = "856DDBAC";
            atype.info = "PNG Image";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "POOL";
            atype.typeID = "0C900FDB";
            atype.info = "Pool Surface";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "Popups";
            atype.typeID = "2C310F46";
            atype.info = "Unknown";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "POSI";
            atype.typeID = "504F5349";
            atype.info = "Edith Positional Information (deprecated)";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "PTBP";
            atype.typeID = "50544250";
            atype.info = "Package Toolkit";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "ROOF";
            atype.typeID = "AB9406AA";
            atype.info = "Roof";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "SFX";
            atype.typeID = "8DB5E4C2";
            atype.info = "Sound Effects";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "SHPE";
            atype.typeID = "FC6EB1F7";
            atype.info = "Shape";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "SIMI";
            atype.typeID = "53494D49";
            atype.info = "Sim Information";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "SKIN";
            atype.typeID = "AC506764";
            atype.info = "Sim Outfits";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "SLOT";
            atype.typeID = "534C4F54";
            atype.info = "Object Slot";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "SMAP";
            atype.typeID = "CAC4FC40";
            atype.info = "String Map";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "SPR2";
            atype.typeID = "53505232";
            atype.info = "Sprites";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "SPX1";
            atype.typeID = "2026960B";
            atype.info = "SPX Speech";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "SREL";
            atype.typeID = "CC364C2A";
            atype.info = "Sim Relations";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "STR#";
            atype.typeID = "53545223";
            atype.info = "Text String";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "STXR";
            atype.typeID = "ACE46235";
            atype.info = "Surface Texture";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "SWAF";
            atype.typeID = "CD95548E";
            atype.info = "Sim Wants and Fears";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "TATT";
            atype.typeID = "54415454";
            atype.info = "Tree Attributes";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "TGA";
            atype.typeID = "856DDBAC";
            atype.info = "Targa Image";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "TMAP";
            atype.typeID = "4B58975B";
            atype.info = "Lot or Terrain Texture Map";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "TPRP";
            atype.typeID = "54505250";
            atype.info = "Edith SimAntics Behavior Labels";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "TRCN";
            atype.typeID = "5452434E";
            atype.info = "Behavior Constant Labels";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "TREE";
            atype.typeID = "54524545";
            atype.info = "Tree Data";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "TSSG";
            atype.typeID = "BA353CE1";
            atype.info = "The Sims SG System";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "TTAB";
            atype.typeID = "54544142";
            atype.info = "Pie Menu Functions";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "TTAs";
            atype.typeID = "54544173";
            atype.info = "Pie Menu Strings";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "TXMT";
            atype.typeID = "49596978";
            atype.info = "Material Definitions";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "TXTR";
            atype.typeID = "1C4A276C";
            atype.info = "Texture";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "UI";
            atype.typeID = "00000000";
            atype.info = "User Interface";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "VERT";
            atype.typeID = "CB4387A1";
            atype.info = "Vertex Layer";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "WFR";
            atype.typeID = "CD95548E";
            atype.info = "Wants and Fears";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "WGRA";
            atype.typeID = "0A284D0B";
            atype.info = "Wall Graph";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "WLL";
            atype.typeID = "8A84D7B0";
            atype.info = "Wall Layer";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "WRLD";
            atype.typeID = "49FF7D76";
            atype.info = "World Database";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "WTHR";
            atype.typeID = "B21BE28B";
            atype.info = "Weather Info";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "XA";
            atype.typeID = "2026960B";
            atype.info = "XA Audio";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "XHTN";
            atype.typeID = "8C1580B5";
            atype.info = "Hairtone XML";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "XMTO";
            atype.typeID = "584D544F";
            atype.info = "Material Object Class Dump";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "XOBJ";
            atype.typeID = "CCA8E925";
            atype.info = "Object Class Dump";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "XTOL";
            atype.typeID = "2C1FD8A1";
            atype.info = "Texture Overlay XML";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "UNK";
            atype.typeID = "0F9F0C21";
            atype.info = "Unknown (from Nightlife)";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "UNK";
            atype.typeID = "8B0C79D6";
            atype.info = "Unknown";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "UNK";
            atype.typeID = "9D796DB4";
            atype.info = "Unknown";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "UNK";
            atype.typeID = "CC2A6A34";
            atype.info = "Unknown";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.desc = "UNK";
            atype.typeID = "CC8A6A69";
            atype.info = "Unknown";
            AllTypes.Add(atype);
            atype = new typeList();
            
            return AllTypes;
        }

        public List<typeList> createS3TypeList(){
            List<typeList> AllTypes = new List<typeList>();

            atype.typeID="0x00AE6C67";
            atype.desc = "BONE";
            atype.info="skcon";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x00B2D882";
            atype.desc = "_IMG";
            atype.info="dds";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x00B552EA";
            atype.desc = "_SPT";
            atype.info="tree";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x015A1849";
            atype.desc = "GEOM";
            atype.info="geom";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0166038C";
            atype.desc = "NMAP";
            atype.info="nmap";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x01661233";
            atype.desc = "MODL";
            atype.info="scene";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x01A527DB";
            atype.desc = "_AUD";
            atype.info="mm";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x01D0E6FB";
            atype.desc = "VBUF";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x01D0E70F";
            atype.desc = "IBUF";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x01D0E723";
            atype.desc = "VRTF";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x01D0E75D";
            atype.desc = "MATD";
            atype.info="scene";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x01D0E76B";
            atype.desc = "SKIN";
            atype.info="scene";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x01D10F34";
            atype.desc = "MLOD";
            atype.info="scene";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x01EEF63A";
            atype.desc = "_AUD";
            atype.info="mm";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x02019972";
            atype.desc = "MTST";
            atype.info="scene";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x021D7E8C";
            atype.desc = "SPT2";
            atype.info="scene";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0229684B";
            atype.desc = "VBUF";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0229684F";
            atype.desc = "IBUF";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x022B756C";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x025C90A6";
            atype.desc = "_CSS";
            atype.info="css";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x025C95B6";
            atype.desc = "LAYO";
            atype.info="xml";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x025ED6F4";
            atype.desc = "SIMO";
            atype.info="xml";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x029E333B";
            atype.desc = "VOCE";
            atype.info="voicemix";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x02C9EFF2";
            atype.desc = "MIXR";
            atype.info="audmix";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x02D5DF13";
            atype.desc = "JAZZ";
            atype.info="jazz";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x02DC343F";
            atype.desc = "OBJK";
            atype.info="objkey";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x033260E3";
            atype.desc = "TKMK";
            atype.info="trackmask";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0333406C";
            atype.desc = "_XML";
            atype.info="xml";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x033A1435";
            atype.desc = "TXTC";
            atype.info="compositor";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x033B2B66";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0341ACC9";
            atype.desc = "TXTF";
            atype.info="fabric";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x034AEECB";
            atype.desc = "CASP";
            atype.info="caspart";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0354796A";
            atype.desc = "TONE";
            atype.info="skintone";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x03555BA8";
            atype.desc = "TONE";
            atype.info="hairtone";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0355E0A6";
            atype.desc = "BOND";
            atype.info="bonedelta";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0358B08A";
            atype.desc = "FACE";
            atype.info="faceblend";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x03B33DDF";
            atype.desc = "ITUN";
            atype.info="xml";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x03B4C61D";
            atype.desc = "LITE";
            atype.info="light";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x03D843C2";
            atype.desc = "CCHE";
            atype.info="cacheentry";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x03D86EA4";
            atype.desc = "DETL";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x03E80CDC";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0418FE2A";
            atype.desc = "CFEN";
            atype.info="fence";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x044735DD";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x044AE110";
            atype.desc = "COMP";
            atype.info="xml";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x046A7235";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x048A166D";
            atype.desc = "UNKN";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0498DA7E";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x049CA4CD";
            atype.desc = "CSTR";
            atype.info="stairs";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x04A09283";
            atype.desc = "UNKN";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x04A4D951";
            atype.desc = "WDET";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x04AC5D93";
            atype.desc = "CPRX";
            atype.info="proxyprod";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x04B30669";
            atype.desc = "CTTL";
            atype.info="terraintool";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x04C58103";
            atype.desc = "CRAL";
            atype.info="railing";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x04D82D90";
            atype.desc = "CMRU";
            atype.info="cachemru";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x04ED4BB2";
            atype.desc = "CTPT";
            atype.info="terrainpaint";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x04EE6ABB";
            atype.desc = "UNKN";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x04F3CC01";
            atype.desc = "CFIR";
            atype.info="fireplace";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x04F51033";
            atype.desc = "SBNO";
            atype.info="binoutfit";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x04F66BCC";
            atype.desc = "UNKN";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x04F88964";
            atype.desc = "SIME";
            atype.info="simexport";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x051DF2DD";
            atype.desc = "CBLN";
            atype.info="compblend";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x05512255";
            atype.desc = "UNKN";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x553EAD4";
            atype.desc = "UNKN";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0563919E";
            atype.desc = "UNKN";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0580A2B4";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0580A2B5";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0580A2B6";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0580A2CD";
            atype.desc = "SNAP";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0580A2CE";
            atype.desc = "SNAP";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0580A2CF";
            atype.desc = "SNAP";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0580A2B4";
            atype.desc = "THUM";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0580A2B5";
            atype.desc = "THUM";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0580A2B6";
            atype.desc = "THUM";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0589DC44";
            atype.desc = "THUM";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0589DC45";
            atype.desc = "THUM";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0589DC46";
            atype.desc = "THUM";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0589DC46";
            atype.desc = "THUM";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0591B1AF";
            atype.desc = "UPST";
            atype.info="usercastpreset";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x05B17698";
            atype.desc = "THUM";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x05B17699";
            atype.desc = "THUM";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x05B1769A";
            atype.desc = "THUM";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x05B1B524";
            atype.desc = "THUM";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x05B1B525";
            atype.desc = "THUM";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x05B1B526";
            atype.desc = "THUM";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x05CD4BB3";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x05DA8AF6";
            atype.desc = "WBND";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x05E4FAF7";
            atype.desc = "UNKN";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x05ED1226";
            atype.desc = "REFS";
            atype.info="references";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x05FF3549";
            atype.desc = "UNKN";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x05FF6BA4";
            atype.desc = "2ARY";
            atype.info="bnry";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0604ABDA";
            atype.desc = "DMTR";
            atype.info="dreamtree";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x060B390C";
            atype.desc = "CWAT";
            atype.info="water";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x060E1826";
            atype.desc = "UNKN";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0611B0E7";
            atype.desc = "UNKN";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x062853A8";
            atype.desc = "FAMD";
            atype.info="household";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x062C8204";
            atype.desc = "BBLN";
            atype.info="filen";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x062E9EE0";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x06302271";
            atype.desc = "CINF";
            atype.info="color";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x063261DA";
            atype.desc = "HINF";
            atype.info="haircolor";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x06326213";
            atype.desc = "OBCI";
            atype.info="objcolor";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x06393F5D";
            atype.desc = "UNKN";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x065B8B38";
            atype.desc = "UNKN";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x065BFCAC";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x065BFCAD";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x065BFCAE";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0668F628";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0668F630";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0668F635";
            atype.desc = "TWNI";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0668F639";
            atype.desc = "TWNP";
            atype.info="imgpath";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x067CAA11";
            atype.desc = "BGEO";
            atype.info="blendgeom";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x06B981ED";
            atype.desc = "OBJS";
            atype.info="objs";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x06CE4804";
            atype.desc = "META";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x06D6B112";
            atype.desc = "UNKN";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x06DC847E";
            atype.desc = "UNKN";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x073FAA07";
            atype.desc = "S3SA";
            atype.info="s3sa";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x07046B39";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x07CD07EC";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0A36F07A";
            atype.desc = "CCFP";
            atype.info="fountain";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0C37A5B5";
            atype.desc = "LOOK";
            atype.info="lookuptab";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0C07456D";
            atype.desc = "COLL";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x11E32896";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x16B17A6C";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x1F886EAD";
            atype.desc = "_INI";
            atype.info="ini";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x220557DA";
            atype.desc = "STBL";
            atype.info="stbl";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x2AD195F2";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x2653E3C8";
            atype.desc = "THUM";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x2653E3C9";
            atype.desc = "THUM";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x2653E3CA";
            atype.desc = "THUM";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x2D4284F0";
            atype.desc = "THUM";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x2D4284F1";
            atype.desc = "THUM";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x2D4284F2";
            atype.desc = "THUM";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x2E75C764";
            atype.desc = "ICON";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x2E75C765";
            atype.desc = "ICON";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x2E75C766";
            atype.desc = "ICON";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x2E75C767";
            atype.desc = "ICON";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x2F7D0002";
            atype.desc = "IMAG";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x2F7D0004";
            atype.desc = "IMAG";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x2F7D0008";
            atype.desc = "UITX";
            atype.info="uitexture";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x312E7545";
            atype.desc = "UNKN";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x316C78F2";
            atype.desc = "CFND";
            atype.info="foundation";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x319E4F1D";
            atype.desc = "OBJD";
            atype.info="object";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x32C83095";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x342778A7";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x342779A7";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x34E5247C";
            atype.desc = "UNKN";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x35A33E29";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x3A65AF29";
            atype.desc = "MINF";
            atype.info="makeup";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x3D8632D0";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x4D1A5589";
            atype.desc = "OBJN";
            atype.info="objn";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x4F09F8E1";
            atype.desc = "UNKN";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x515CA4CD";
            atype.desc = "CWAL";
            atype.info="wall";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x54372472";
            atype.desc = "TSNP";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x5DE9DBA0";
            atype.desc = "THUM";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x5DE9DBA1";
            atype.desc = "THUM";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x5DE9DBA2";
            atype.desc = "THUM";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x626F60CC";
            atype.desc = "THUM";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x626F60CD";
            atype.desc = "THUM";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x626F60CE";
            atype.desc = "THUM";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x628A788F";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x63A33EA7";
            atype.desc = "ANIM";
            atype.info="scene";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x6ABFAD26";
            atype.desc = "UNKN";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x6B20C4F3";
            atype.desc = "CLIP";
            atype.info="animation";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x6B6D837D";
            atype.desc = "SNAP";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x6B6D837E";
            atype.desc = "SNAP";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x6B6D837F";
            atype.desc = "SNAP";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x72683C15";
            atype.desc = "STPR";
            atype.info="sktonepreset";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x736884F1";
            atype.desc = "VPXY";
            atype.info="vpxy";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x73E93EEB";
            atype.desc = "_XML";
            atype.info="xml";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x7672F0C5";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x8070223D";
            atype.desc = "AUDT";
            atype.info="audtun";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x82B43584";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x8EAF13DE";
            atype.desc = "_RIG";
            atype.info="grannyrig";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x8FFB80F6";
            atype.desc = "_ADS";
            atype.info="dds";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x90620000";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x90624C1B";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x9063660D";
            atype.desc = "WTXT";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x9063660E";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x913381F2";
            atype.desc = "UNKN";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x9151E6BC";
            atype.desc = "CWST";
            atype.info="wallstyle";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x91EDBD3E";
            atype.desc = "CRST";
            atype.info="roofstyle";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x94C5D14A";
            atype.desc = "SIGR";
            atype.info="signature";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x94EC4B54";
            atype.desc = "UNKN";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xA2377025";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xA5F9FE18";
            atype.desc = "UNKN";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xA8D58BE5";
            atype.desc = "SKIL";
            atype.info="xml";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xAE39399F";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xB074ACE6";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xB125533A";
            atype.desc = "UNKN";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xB1422971";
            atype.desc = "UNKN";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xB1CC1AF6";
            atype.desc = "_VID";
            atype.info="mm";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xB4DD716B";
            atype.desc = "_INV";
            atype.info="inventory";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xB52F5055";
            atype.desc = "FBLN";
            atype.info="blendunit";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xCF84EC98";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xCF9A4ACE";
            atype.desc = "MDLR";
            atype.info="modular";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xD063545B";
            atype.desc = "LDES";
            atype.info="lotdesc";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xD3044521";
            atype.desc = "RSLT";
            atype.info="scene";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xD382BF57";
            atype.desc = "FTPT";
            atype.info="scene";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xD4D9FBE5";
            atype.desc = "PTRN";
            atype.info="patternlist";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xD84E7FC5";
            atype.desc = "ICON";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xD84E7FC6";
            atype.desc = "ICON";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xD84E7FC7";
            atype.desc = "ICON";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xD9BD0909";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xDC37E964";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xDD3223A7";
            atype.desc = "BUFF";
            atype.info="xml";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xDEA2951C";
            atype.desc = "PETB";
            atype.info="breed";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xEA5118B0";
            atype.desc = "_SWB";
            atype.info="effects";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xF0633989";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xF0FF5598";
            atype.desc = "";
            atype.info="cam";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xF12E5E12";
            atype.desc = "UNKN";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xF1EDBD86";
            atype.desc = "CRMT";
            atype.info="roofmatrl";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xF3A38370";
            atype.desc = "NGMP";
            atype.info="guidmap";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xF609FD60";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xFCEAB65B";
            atype.desc = "THUM";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();
            return AllTypes;
        }

        public List<typeList> createS4TypeList(){
            List<typeList> AllTypes = new List<typeList>();
            atype.typeID="0x00B2D882 ";
            atype.desc = "_IMG ";
            atype.info="dds ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x00DE5AC5 ";
            atype.desc = "";
            atype.info="rmi ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x010FAF71 ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x015A1849 ";
            atype.desc = "GEOM ";
            atype.info="geom";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0166038C ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x01661233 ";
            atype.desc = "MODL ";
            atype.info="scene ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x01942E2C ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x01A527DB ";
            atype.desc = "_AUD ";
            atype.info="mm ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x01D0E75D ";
            atype.desc = "MATD ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x01D10F34 ";
            atype.desc = "MLOD ";
            atype.info="scene ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x01EEF63A ";
            atype.desc = "_AUD ";
            atype.info="mm ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x02019972 ";
            atype.desc = "MTST ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x025ED6F4 ";
            atype.desc = "SIMO ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x02D5DF13 ";
            atype.desc = "JAZZ ";
            atype.info="jazz ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x033260E3 ";
            atype.desc = "TkMk ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0333406C ";
            atype.desc = "_XML ";
            atype.info="xml ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x033B2B66 ";
            atype.desc = "UNKN ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x034AEECB ";
            atype.desc = "CASP ";
            atype.info="caspart ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0354796A ";
            atype.desc = "TONE ";
            atype.info="skintone ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0355E0A6 ";
            atype.desc = "BOND ";
            atype.info="bonedelta ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x03B4C61D ";
            atype.desc = "LITE ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0418FE2A ";
            atype.desc = "CFEN ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x067CAA11 ";
            atype.desc = "BGEO ";
            atype.info="blendgeom ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x07936CE0 ";
            atype.desc = "SPLT ";
            atype.info="CTProductBlock ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0A227BCF ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0C772E27 ";
            atype.desc = "_XML ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x0D338A3A ";
            atype.desc = "THUM ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x105205BA ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x122FC66A ";
            atype.desc = "_XML ";
            atype.info="lottypeeventmap ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x12952634 ";
            atype.desc = "LDNB ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x153D2219 ";
            atype.desc = "UNKN ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x16CA6BC4 ";
            atype.desc = "_THM ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x17C0C281 ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x18F3C673 ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x19301120 ";
            atype.desc = "WCLR ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x1A8506C5 ";
            atype.desc = "_XML ";
            atype.info="mmusx? ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x1B25A024 ";
            atype.desc = "_XML ";
            atype.info="sysx ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x1C1CF1F7 ";
            atype.desc = "CRAL ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x1C99B344 ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x1CC04273 ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x1D6DF1CF ";
            atype.desc = "CCOL ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x20D81496 ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x220557DA ";
            atype.desc = "STBL ";
            atype.info="stbl";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x25796DCA ";
            atype.desc = "_OTF ";
            atype.info="otf ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x26978421 ";
            atype.desc = "_CUR ";
            atype.info="cur ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x276CA4B9 ";
            atype.desc = "_TTF ";
            atype.info="ttf ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x2A8A5E22 ";
            atype.desc = "";
            atype.info="trayitem ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x2AD195F2 ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x2F7D0004 ";
            atype.desc = "IMAG ";
            atype.info="png ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x2FAE983E ";
            atype.desc = "CFND ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x319E4F1D ";
            atype.desc = "COBJ ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x3453CF95 ";
            atype.desc = "RLE2 ";
            atype.info="DXT5RLE2 ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x370EFD6E ";
            atype.desc = "ROOM ";
            atype.info="room ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x376840D7 ";
            atype.desc = "AVI ";
            atype.info="Video ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x3924DE26 ";
            atype.desc = "_BPT ";
            atype.info="blueprint ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x3BD45407 ";
            atype.desc = "_HHI ";
            atype.info="hhi ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x3BF8FD86 ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x3C1AF1F2 ";
            atype.desc = "THUM ";
            atype.info="png ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x3C2A8647 ";
            atype.desc = "THUM ";
            atype.info="png ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x3D8632D0 ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x3EAAA87C ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x3F0C529A ";
            atype.desc = "CSPN ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x4115F9D5 ";
            atype.desc = "XML ";
            atype.info="mixbus ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x48C28979 ";
            atype.desc = "CLCT ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x4F726BBE ";
            atype.desc = "FTPT ";
            atype.info="footprint ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x545AC67A ";
            atype.desc = "DATA ";
            atype.info="data ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x56278554 ";
            atype.desc = "_SGI ";
            atype.info="sgi ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x5B282D45 ";
            atype.desc = "THUM ";
            atype.info="png";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x5BE29703 ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x6017E896 ";
            atype.desc = "_XML ";
            atype.info="buff ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x62E94D38 ";
            atype.desc = "";
            atype.info="xml ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x62ECC59A ";
            atype.desc = "GFX ";
            atype.info="gfx ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x6B20C4F3 ";
            atype.desc = "CLIP ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x6DFF1A66 ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x6F40796A ";
            atype.desc = "WRPR ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x71A449C9 ";
            atype.desc = "SKYB ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x71BDB8A2 ";
            atype.desc = "STLK ";
            atype.info="sfpusp? ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x729F6C4F ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x73CB32C2 ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x74050B1F ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x76BCF80C ";
            atype.desc = "TRIM ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x78C8BCE4 ";
            atype.desc = "WRMF ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x810A102D ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x81CA1A10 ";
            atype.desc = "MTBL ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x84C23219 ";
            atype.desc = "CFLT ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x892C4B8A ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x8B18FF6E ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x8EAF13DE ";
            atype.desc = "_RIG ";
            atype.info="skeleton ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x90624C1B ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x9063660D ";
            atype.desc = "WTXT ";
            atype.info="World texture map ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x9063660E ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x91568FD8 ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x91EDBD3E ";
            atype.desc = "CRST ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x9917EACD ";
            atype.desc = "RNDP ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x99D98089 ";
            atype.desc = "XML ";
            atype.info="imusx? ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x9A20CD1C ";
            atype.desc = "CSTR ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x9AFE47F5 ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x9C925813 ";
            atype.desc = "THUM ";
            atype.info="png ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x9D1AB874 ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0x9F5CFF10 ";
            atype.desc = "CSTL ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xA0451CBD ";
            atype.desc = "CPMP ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xA057811C ";
            atype.desc = "CFRZ ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xA1FF2FC4 ";
            atype.desc = "THUM ";
            atype.info="png ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xA576C2E7 ";
            atype.desc = "XML ";
            atype.info="mapx ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xA5DFFCF3 ";
            atype.desc = "CPLT ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xA680EA4B ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xA8F7B517 ";
            atype.desc = "CWNS ";
            atype.info="bnry ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xAC03A936 ";
            atype.desc = "CMTX ";
            atype.info="bnry ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xAC16FBEC ";
            atype.desc = "RMAP ";
            atype.info="regionmap ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xAE39399F ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xB0118C15 ";
            atype.desc = "TMLT ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xB0311D0F ";
            atype.desc = "CRTR ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xB3C438F0 ";
            atype.desc = "";
            atype.info="householdbinary ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xB4F762C9 ";
            atype.desc = "CFLR ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xB61DE6B4 ";
            atype.desc = "_XML ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xB6C8B6A0 ";
            atype.desc = "_IMG ";
            atype.info="dds ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xB734E44F ";
            atype.desc = "FTPT ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xB8444447 ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xB91E18DB ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xB93A9915 ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xBA856C78 ";
            atype.desc = "RLES ";
            atype.info="DXT5RLES ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xBC4A5044 ";
            atype.desc = "CLHD ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xBC80ED59 ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xBDD82221 ";
            atype.desc = "AUEV ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xC0084996 ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xC0DB5AE7 ";
            atype.desc = "OBJD ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xC202C770 ";
            atype.desc = "XML ";
            atype.info="trax ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xC582D2FB ";
            atype.desc = "XML ";
            atype.info="voicex ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xC5F6763E ";
            atype.desc = "UNKN ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xC71CA490 ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xCB5FDDC7 ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xCD9DE247 ";
            atype.desc = "PNG ";
            atype.info="png ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xD2DC5BAD ";
            atype.desc = "_XML ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xD3044521 ";
            atype.desc = "RSLT ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xD33C281E ";
            atype.desc = "";
            atype.info="bpi ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xD382BF57 ";
            atype.desc = "FTPT ";
            atype.info="scene ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xD5F0F921 ";
            atype.desc = "CWAL ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xD65DAFF9 ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xD99F5E5C ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xDB43E069 ";
            atype.desc = "DMAP ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xE0ED7129 ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xE2249422 ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xE231B3D8 ";
            atype.desc = "XML ";
            atype.info="objmod ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xE7ADA79D ";
            atype.desc = "CFTR ";
            atype.info="CTProductFountainTrim ";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xE882D22F ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xEA5118B0 ";
            atype.desc = "_SWB ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xEAA32ADD ";
            atype.desc = "UNKN ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xEBCBB16C ";
            atype.desc = "CTPT ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xF0633989 ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xF1EDBD86 ";
            atype.desc = "CRPT ";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xFA25B7DE ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xFCB1A1E4 ";
            atype.desc = "";
            atype.info="";
            AllTypes.Add(atype);
            atype = new typeList();

            atype.typeID="0xFD04E3BE ";
            atype.desc = "PRPX ";
            atype.info="propx ";
            AllTypes.Add(atype);
            atype = new typeList();

            return AllTypes;
        }
    }
    

    public class GlobalVariables {
        public static bool debugMode = true;
        public static string logfile = "I:\\Code\\C#\\Sims-CC-Sorter\\src\\SimsCCManager.Console\\log\\mainlog.log";
        public static string ModFolder;
        public static int gameVer;        


        public void Initialize(int gameNum, string modLocation){
            gameVer = gameNum;
            ModFolder = modLocation;
            logfile = modLocation + ".\\SimsCCSorter.log";
            StreamWriter putContentsIntoTxt = new StreamWriter(logfile);
            putContentsIntoTxt.Close();
        }
    }          

    public class LoggingGlobals
    {
        
        private string time = "";
        private string statement = "";
        public bool firstrunmain = true;
        public bool firstrundebug = true;
        private string debuglog = "I:\\Code\\C#\\Sims-CC-Sorter\\src\\SimsCCManager.Console\\log\\debug.log";
        
        //Function for logging to the logfile set at the start of the program
        public void MakeLog (string Statement, bool debug, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "")
        {   
            FileInfo filepath = new FileInfo(filePath);            

            if (debug) {
                if (firstrundebug){
                    StreamWriter addToInternalLog = new StreamWriter (debuglog, append: false);
                    addToInternalLog.WriteLine("Initializing internal log file.");
                    time = DateTime.Now.ToString("h:mm:ss tt");
                    statement = "[L" + lineNumber + " | " + filepath.Name + "] " + time + ": " + Statement;
                    addToInternalLog.WriteLine(statement);
                    addToInternalLog.Close();
                    firstrundebug = false;
                } else {
                    StreamWriter addToLog = new StreamWriter (debuglog, append: true);
                    time = DateTime.Now.ToString("h:mm:ss tt");
                    statement = "[L" + lineNumber + " | " + filepath.Name + "] " + time + ": " + Statement;
                    addToLog.WriteLine(statement);
                    addToLog.Close(); 
                }         
            } else {                
                if (firstrunmain){
                    StreamWriter addToLog = new StreamWriter (GlobalVariables.logfile, append: false);
                    addToLog.WriteLine("Initializing log file.");
                    time = DateTime.Now.ToString("h:mm:ss tt");
                    statement = "[L" + lineNumber + " | " + filepath.Name + "] " + time + ": " + Statement;
                    addToLog.WriteLine(statement);
                    addToLog.WriteLine(statement);
                    addToLog.Close();
                    firstrunmain = false;
                } else {
                    StreamWriter addToLog = new StreamWriter (GlobalVariables.logfile, append: true);
                    time = DateTime.Now.ToString("h:mm:ss tt");
                    statement = "[L" + lineNumber + " | " + filepath.Name + "] " + time + ": " + Statement;
                    addToLog.WriteLine(statement);
                    addToLog.Close();
                }
            }
        }
    } 
}