using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SimsCCManager.Containers;
using SimsCCManager.Debugging;
using SimsCCManager.Globals;
using SimsCCManager.PackageReaders.Containers;
using SimsCCManager.PackageReaders.DecryptionFunctions;
using SimsCCManager.PackageReaders.ImageTransformations;
using SimsCCManager.Packages.Containers;

namespace SimsCCManager.PackageReaders
{
    public class Sims4PackageReader
    {
        public static List<EntryType> EntryTypes = new()
        {
            new EntryType(){ Tag = "_IMG", TypeID = "00B2D882", Description = "dds " },
            new EntryType(){ Tag = "", TypeID = "00DE5AC5", Description = "rmi " },
            new EntryType(){ Tag = "", TypeID = "010FAF71", Description = "" },
            new EntryType(){ Tag = "GEOM", TypeID = "015A1849", Description = "geom" },
            new EntryType(){ Tag = "_KEY", TypeID = "0166038C", Description = "" },
            new EntryType(){ Tag = "MODL", TypeID = "01661233", Description = "scene " },
            new EntryType(){ Tag = "", TypeID = "01942E2C", Description = "" },
            new EntryType(){ Tag = "_AUD", TypeID = "01A527DB", Description = "mm " },
            new EntryType(){ Tag = "MATD", TypeID = "01D0E75D", Description = "" },
            new EntryType(){ Tag = "MLOD", TypeID = "01D10F34", Description = "scene " },
            new EntryType(){ Tag = "_AUD", TypeID = "01EEF63A", Description = "mm " },
            new EntryType(){ Tag = "MTST", TypeID = "02019972", Description = "" },
            new EntryType(){ Tag = "SIMO", TypeID = "025ED6F4", Description = "" },
            new EntryType(){ Tag = "JAZZ", TypeID = "02D5DF13", Description = "jazz " },
            new EntryType(){ Tag = "TkMk", TypeID = "033260E3", Description = "" },
            new EntryType(){ Tag = "_XML", TypeID = "0333406C", Description = "xml " },
            new EntryType(){ Tag = "UNKN", TypeID = "033B2B66", Description = "" },
            new EntryType(){ Tag = "CASP", TypeID = "034AEECB", Description = "caspart " },
            new EntryType(){ Tag = "TONE", TypeID = "0354796A", Description = "skintone " },
            new EntryType(){ Tag = "BOND", TypeID = "0355E0A6", Description = "bonedelta " },
            new EntryType(){ Tag = "LITE", TypeID = "03B4C61D", Description = "" },
            new EntryType(){ Tag = "CFEN", TypeID = "0418FE2A", Description = "" },
            new EntryType(){ Tag = "BGEO", TypeID = "067CAA11", Description = "blendgeom " },
            new EntryType(){ Tag = "SPLT", TypeID = "07936CE0", Description = "CTProductBlock " },
            new EntryType(){ Tag = "", TypeID = "0A227BCF", Description = "" },
            new EntryType(){ Tag = "_XML", TypeID = "0C772E27", Description = "" },
            new EntryType(){ Tag = "THUM", TypeID = "0D338A3A", Description = "" },
            new EntryType(){ Tag = "", TypeID = "105205BA", Description = "" },
            new EntryType(){ Tag = "_XML", TypeID = "122FC66A", Description = "lottypeeventmap " },
            new EntryType(){ Tag = "LDNB", TypeID = "12952634", Description = "" },
            new EntryType(){ Tag = "UNKN", TypeID = "153D2219", Description = "" },
            new EntryType(){ Tag = "_THM", TypeID = "16CA6BC4", Description = "" },
            new EntryType(){ Tag = "", TypeID = "17C0C281", Description = "" },
            new EntryType(){ Tag = "", TypeID = "18F3C673", Description = "" },
            new EntryType(){ Tag = "WCLR", TypeID = "19301120", Description = "" },
            new EntryType(){ Tag = "_XML", TypeID = "1A8506C5", Description = "mmusx? " },
            new EntryType(){ Tag = "_XML", TypeID = "1B25A024", Description = "sysx " },
            new EntryType(){ Tag = "CRAL", TypeID = "1C1CF1F7", Description = "" },
            new EntryType(){ Tag = "", TypeID = "1C99B344", Description = "" },
            new EntryType(){ Tag = "", TypeID = "1CC04273", Description = "" },
            new EntryType(){ Tag = "CCOL", TypeID = "1D6DF1CF", Description = "" },
            new EntryType(){ Tag = "", TypeID = "20D81496", Description = "" },
            new EntryType(){ Tag = "STBL", TypeID = "220557DA", Description = "stbl" },
            new EntryType(){ Tag = "_OTF", TypeID = "25796DCA", Description = "otf " },
            new EntryType(){ Tag = "_CUR", TypeID = "26978421", Description = "cur " },
            new EntryType(){ Tag = "_TTF", TypeID = "276CA4B9", Description = "ttf " },
            new EntryType(){ Tag = "", TypeID = "2A8A5E22", Description = "trayitem " },
            new EntryType(){ Tag = "", TypeID = "2AD195F2", Description = "" },
            new EntryType(){ Tag = "IMAG", TypeID = "2F7D0004", Description = "png " },
            new EntryType(){ Tag = "CFND", TypeID = "2FAE983E", Description = "" },
            new EntryType(){ Tag = "COBJ", TypeID = "319E4F1D", Description = "" },
            new EntryType(){ Tag = "RLE2", TypeID = "3453CF95", Description = "DXT5RLE2 " },
            new EntryType(){ Tag = "ROOM", TypeID = "370EFD6E", Description = "room " },
            new EntryType(){ Tag = "AVI", TypeID = "376840D7", Description = "Video " },
            new EntryType(){ Tag = "_BPT", TypeID = "3924DE26", Description = "blueprint " },
            new EntryType(){ Tag = "_HHI", TypeID = "3BD45407", Description = "hhi " },
            new EntryType(){ Tag = "", TypeID = "3BF8FD86", Description = "" },
            new EntryType(){ Tag = "THUM", TypeID = "3C1AF1F2", Description = "png " },
            new EntryType(){ Tag = "THUM", TypeID = "3C2A8647", Description = "png " },
            new EntryType(){ Tag = "", TypeID = "3D8632D0", Description = "" },
            new EntryType(){ Tag = "", TypeID = "3EAAA87C", Description = "" },
            new EntryType(){ Tag = "CSPN", TypeID = "3F0C529A", Description = "" },
            new EntryType(){ Tag = "XML", TypeID = "4115F9D5", Description = "mixbus " },
            new EntryType(){ Tag = "CLCT", TypeID = "48C28979", Description = "" },
            new EntryType(){ Tag = "FTPT", TypeID = "4F726BBE", Description = "footprint " },
            new EntryType(){ Tag = "DATA", TypeID = "545AC67A", Description = "data " },
            new EntryType(){ Tag = "_SGI", TypeID = "56278554", Description = "sgi " },
            new EntryType(){ Tag = "THUM", TypeID = "5B282D45", Description = "png" },
            new EntryType(){ Tag = "", TypeID = "5BE29703", Description = "" },
            new EntryType(){ Tag = "_XML", TypeID = "6017E896", Description = "buff " },
            new EntryType(){ Tag = "", TypeID = "62E94D38", Description = "xml " },
            new EntryType(){ Tag = "GFX", TypeID = "62ECC59A", Description = "gfx " },
            new EntryType(){ Tag = "", TypeID = "6DFF1A66", Description = "" },
            new EntryType(){ Tag = "WRPR", TypeID = "6F40796A", Description = "" },
            new EntryType(){ Tag = "SKYB", TypeID = "71A449C9", Description = "" },
            new EntryType(){ Tag = "STLK", TypeID = "71BDB8A2", Description = "sfpusp? " },
            new EntryType(){ Tag = "", TypeID = "729F6C4F", Description = "" },
            new EntryType(){ Tag = "", TypeID = "73CB32C2", Description = "" },
            new EntryType(){ Tag = "", TypeID = "74050B1F", Description = "" },
            new EntryType(){ Tag = "TRIM", TypeID = "76BCF80C", Description = "" },
            new EntryType(){ Tag = "WRMF", TypeID = "78C8BCE4", Description = "" },
            new EntryType(){ Tag = "", TypeID = "810A102D", Description = "" },
            new EntryType(){ Tag = "MTBL", TypeID = "81CA1A10", Description = "" },
            new EntryType(){ Tag = "CFLT", TypeID = "84C23219", Description = "" },
            new EntryType(){ Tag = "", TypeID = "892C4B8A", Description = "" },
            new EntryType(){ Tag = "HOTC", TypeID = "8B18FF6E", Description = "Sim Hotspot Control Resource" },
            new EntryType(){ Tag = "_RIG", TypeID = "8EAF13DE", Description = "skeleton " },
            new EntryType(){ Tag = "", TypeID = "90624C1B", Description = "" },
            new EntryType(){ Tag = "WTXT", TypeID = "9063660D", Description = "World texture map " },
            new EntryType(){ Tag = "", TypeID = "9063660E", Description = "" },
            new EntryType(){ Tag = "", TypeID = "91568FD8", Description = "" },
            new EntryType(){ Tag = "CRST", TypeID = "91EDBD3E", Description = "" },
            new EntryType(){ Tag = "RNDP", TypeID = "9917EACD", Description = "" },
            new EntryType(){ Tag = "XML", TypeID = "99D98089", Description = "imusx? " },
            new EntryType(){ Tag = "CSTR", TypeID = "9A20CD1C", Description = "" },
            new EntryType(){ Tag = "", TypeID = "9AFE47F5", Description = "" },
            new EntryType(){ Tag = "THUM", TypeID = "9C925813", Description = "png " },
            new EntryType(){ Tag = "", TypeID = "9D1AB874", Description = "" },
            new EntryType(){ Tag = "CSTL", TypeID = "9F5CFF10", Description = "" },
            new EntryType(){ Tag = "CPMP", TypeID = "A0451CBD", Description = "" },
            new EntryType(){ Tag = "CFRZ", TypeID = "A057811C", Description = "" },
            new EntryType(){ Tag = "THUM", TypeID = "A1FF2FC4", Description = "png " },
            new EntryType(){ Tag = "XML", TypeID = "A576C2E7", Description = "mapx " },
            new EntryType(){ Tag = "CPLT", TypeID = "A5DFFCF3", Description = "" },
            new EntryType(){ Tag = "", TypeID = "A680EA4B", Description = "" },
            new EntryType(){ Tag = "CWNS", TypeID = "A8F7B517", Description = "bnry " },
            new EntryType(){ Tag = "CMTX", TypeID = "AC03A936", Description = "bnry " },
            new EntryType(){ Tag = "RMAP", TypeID = "AC16FBEC", Description = "regionmap " },
            new EntryType(){ Tag = "", TypeID = "AE39399F", Description = "" },
            new EntryType(){ Tag = "TMLT", TypeID = "B0118C15", Description = "" },
            new EntryType(){ Tag = "CRTR", TypeID = "B0311D0F", Description = "" },
            new EntryType(){ Tag = "", TypeID = "B3C438F0", Description = "householdbinary " },
            new EntryType(){ Tag = "CFLR", TypeID = "B4F762C9", Description = "" },
            new EntryType(){ Tag = "_XML", TypeID = "B61DE6B4", Description = "" },
            new EntryType(){ Tag = "_IMG", TypeID = "B6C8B6A0", Description = "dds " },
            new EntryType(){ Tag = "FTPT", TypeID = "B734E44F", Description = "" },
            new EntryType(){ Tag = "", TypeID = "B8444447", Description = "" },
            new EntryType(){ Tag = "", TypeID = "B91E18DB", Description = "" },
            new EntryType(){ Tag = "", TypeID = "B93A9915", Description = "" },
            new EntryType(){ Tag = "RLES", TypeID = "BA856C78", Description = "DXT5RLES " },
            new EntryType(){ Tag = "CLHD", TypeID = "BC4A5044", Description = "" },
            new EntryType(){ Tag = "", TypeID = "BC80ED59", Description = "" },
            new EntryType(){ Tag = "AUEV", TypeID = "BDD82221", Description = "" },
            new EntryType(){ Tag = "", TypeID = "C0084996", Description = "" },
            new EntryType(){ Tag = "OBJD", TypeID = "C0DB5AE7", Description = "" },
            new EntryType(){ Tag = "XML", TypeID = "C202C770", Description = "trax " },
            new EntryType(){ Tag = "XML", TypeID = "C582D2FB", Description = "voicex " },
            new EntryType(){ Tag = "SMOD", TypeID = "C5F6763E", Description = "Sim Modifier" },
            new EntryType(){ Tag = "", TypeID = "C71CA490", Description = "" },
            new EntryType(){ Tag = "TRTR", TypeID = "CB5FDDC7", Description = "" },
            new EntryType(){ Tag = "PNG", TypeID = "CD9DE247", Description = "png " },
            new EntryType(){ Tag = "_XML", TypeID = "D2DC5BAD", Description = "" },
            new EntryType(){ Tag = "RSLT", TypeID = "D3044521", Description = "" },
            new EntryType(){ Tag = "", TypeID = "D33C281E", Description = "bpi " },
            new EntryType(){ Tag = "FTPT", TypeID = "D382BF57", Description = "scene " },
            new EntryType(){ Tag = "CWAL", TypeID = "D5F0F921", Description = "" },
            new EntryType(){ Tag = "", TypeID = "D65DAFF9", Description = "" },
            new EntryType(){ Tag = "", TypeID = "D99F5E5C", Description = "" },
            new EntryType(){ Tag = "DMAP", TypeID = "DB43E069", Description = "" },
            new EntryType(){ Tag = "", TypeID = "E0ED7129", Description = "" },
            new EntryType(){ Tag = "", TypeID = "E2249422", Description = "" },
            new EntryType(){ Tag = "XML", TypeID = "E231B3D8", Description = "objmod " },
            new EntryType(){ Tag = "CFTR", TypeID = "E7ADA79D", Description = "CTProductFountainTrim " },
            new EntryType(){ Tag = "INTR", TypeID = "E882D22F", Description = "Interaction Tuning Resource" },
            new EntryType(){ Tag = "_SWB", TypeID = "EA5118B0", Description = "" },
            new EntryType(){ Tag = "CPRE", TypeID = "EAA32ADD", Description = "" },
            new EntryType(){ Tag = "CTPT", TypeID = "EBCBB16C", Description = "" },
            new EntryType(){ Tag = "", TypeID = "F0633989", Description = "" },
            new EntryType(){ Tag = "CRPT", TypeID = "F1EDBD86", Description = "" },
            new EntryType(){ Tag = "", TypeID = "FA25B7DE", Description = "" },
            new EntryType(){ Tag = "", TypeID = "FCB1A1E4", Description = "" },
            new EntryType(){ Tag = "PRPX", TypeID = "FD04E3BE", Description = "propx " },
            new EntryType(){ Tag = "MCOR", TypeID = "07576A17", Description = "Model Cutout Resource" },
            new EntryType(){ Tag = "S4SM", TypeID = "7FB6AD8A", Description = "Sims 4 Studio Merged Package Manifest" },
            new EntryType(){ Tag = "LRLE", TypeID = "2BC04EDF", Description = "LRLE Image" },
            new EntryType(){ Tag = "SNTR", TypeID = "7DF2169C", Description = "Snippet Tuning Resource" },
            new EntryType(){ Tag = "CLIP", TypeID = "6B20C4F3", Description = "" },
            new EntryType(){ Tag = "TBST", TypeID = "4F739CEE", Description = "Test Based Score Tuning" },
            new EntryType(){ Tag = "RBTR", TypeID = "0904DF10", Description = "Relationship Bit Tuning Resource" },
            new EntryType(){ Tag = "SGTR", TypeID = "598F28E7", Description = "Situation Goal Tuning Resource" },
            new EntryType(){ Tag = "ASTR", TypeID = "28B64675", Description = "Aspiration Tuning Resource" },
            new EntryType(){ Tag = "PMTR", TypeID = "03E9D964", Description = "Pie Menu Tuning Resource" },
            new EntryType(){ Tag = "BCTR", TypeID = "DEBAFB73", Description = "Broadcaster Tuning Resource" },
            new EntryType(){ Tag = "RWTR", TypeID = "6FA49828", Description = "Reward Tuning Resource" },
            new EntryType(){ Tag = "STTR", TypeID = "339BC5BD", Description = "Statistics Tuning Resource" },
            new EntryType(){ Tag = "CLTR", TypeID = "2C70ADF8", Description = "Career Level Tuning Resource" },
            new EntryType(){ Tag = "CTTR", TypeID = "48C75CE3", Description = "Career Track Tuning Resource" },
            new EntryType(){ Tag = "CATR", TypeID = "73996BEB", Description = "Career Tuning Resource" },
            new EntryType(){ Tag = "OBTR", TypeID = "0069453E", Description = "Objective Tuning Resource" }
        };
    
        public static List<S4Function> Functions = new(){
            new S4Function(){ BodyType = "641354", Function = "Accessory", Subfunction = "Earrings" },
            new S4Function(){ BodyType = "100270191", Function = "Makeup", Subfunction = "Blush" },
            new S4Function(){ BodyType = "104", Function = "Hat", Subfunction = "" },
            new S4Function(){ BodyType = "1048576", Function = "Accessory", Subfunction = "Lip Ring (L)" },
            new S4Function(){ BodyType = "105", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "1114112", Function = "Accessory", Subfunction = "Lip Ring (R)" },
            new S4Function(){ BodyType = "1114260", Function = "Accessory", Subfunction = "Lip Ring (R)" },
            new S4Function(){ BodyType = "1237", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "12432", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "1310792", Function = "Accessory", Subfunction = "Brow Ring (R)" },
            new S4Function(){ BodyType = "1310797", Function = "Accessory", Subfunction = "Brow Ring (L)" },
            new S4Function(){ BodyType = "1310868", Function = "Accessory", Subfunction = "Brow Ring (L)" },
            new S4Function(){ BodyType = "131109", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "131114", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "131144", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "131175", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "131186", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "131190", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "131195", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "131197", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "131223", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "131246", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "131253", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "131313", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "131318", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "134545478", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "136", Function = "Facial Hair", Subfunction = "" },
            new S4Function(){ BodyType = "136192", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "1376404", Function = "Accessory", Subfunction = "Brow Ring (R)" },
            new S4Function(){ BodyType = "1376445", Function = "Accessory", Subfunction = "Brow Ring (R)" },
            new S4Function(){ BodyType = "1441966", Function = "Accessory", Subfunction = "Index Finger (L)" },
            new S4Function(){ BodyType = "144703597", Function = "Hat", Subfunction = "" },
            new S4Function(){ BodyType = "149504", Function = "Accessory", Subfunction = "Index Finger (L)" },
            new S4Function(){ BodyType = "153", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "15597634", Function = "Accessory", Subfunction = "Wrist (L)" },
            new S4Function(){ BodyType = "15663170", Function = "Hat", Subfunction = "" },
            new S4Function(){ BodyType = "15728706", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "1573038", Function = "Accessory", Subfunction = "Ring Finger (L)" },
            new S4Function(){ BodyType = "15794242", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "15925314", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "15990850", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "16121922", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "16318530", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "1638400", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "1638633", Function = "Accessory", Subfunction = "Ring Finger (R)" },
            new S4Function(){ BodyType = "16640", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "16777472", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "169472", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "169728", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "169984", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "1704119", Function = "Accessory", Subfunction = "Middle Finger (L)" },
            new S4Function(){ BodyType = "17236038", Function = "Accessory", Subfunction = "Earrings" },
            new S4Function(){ BodyType = "17408", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "1769472", Function = "Accessory", Subfunction = "Middle Finger (R)" },
            new S4Function(){ BodyType = "1835008", Function = "Facial Hair", Subfunction = "" },
            new S4Function(){ BodyType = "18350156", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "1835079", Function = "Facial Hair", Subfunction = "" },
            new S4Function(){ BodyType = "1835259", Function = "Facial Hair", Subfunction = "" },
            new S4Function(){ BodyType = "18432", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "18481228", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "18546764", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "18677836", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "18688", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "18743372", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "18808908", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "18874444", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "18939970", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "18944", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "1900692", Function = "Makeup", Subfunction = "Lipstick" },
            new S4Function(){ BodyType = "19071044", Function = "Accessory", Subfunction = "Brow Ring (R)" },
            new S4Function(){ BodyType = "19333201", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "19456", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "19464273", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "1966228", Function = "Makeup", Subfunction = "Eyeshadow" },
            new S4Function(){ BodyType = "19968", Function = "Makeup", Subfunction = "Eyeshadow" },
            new S4Function(){ BodyType = "20480", Function = "Makeup", Subfunction = "Eyeshadow" },
            new S4Function(){ BodyType = "20736", Function = "Makeup", Subfunction = "Eyeshadow" },
            new S4Function(){ BodyType = "2097300", Function = "Makeup", Subfunction = "Blush" },
            new S4Function(){ BodyType = "20992", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "210176", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "210432", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "21248", Function = "Makeup", Subfunction = "Lipstick" },
            new S4Function(){ BodyType = "21504", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "2162899", Function = "Face Paint", Subfunction = "" },
            new S4Function(){ BodyType = "21760", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "21954630", Function = "Necklace", Subfunction = "" },
            new S4Function(){ BodyType = "22534", Function = "Shoes", Subfunction = "" },
            new S4Function(){ BodyType = "229376", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "2359529", Function = "Accessory", Subfunction = "Socks" },
            new S4Function(){ BodyType = "23808", Function = "Hat", Subfunction = "" },
            new S4Function(){ BodyType = "238592", Function = "Earrings", Subfunction = "" },
            new S4Function(){ BodyType = "241664", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "243", Function = "Hat", Subfunction = "" },
            new S4Function(){ BodyType = "24320", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "244", Function = "Hat", Subfunction = "" },
            new S4Function(){ BodyType = "24445007", Function = "Hat", Subfunction = "" },
            new S4Function(){ BodyType = "245", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "24510547", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "24576083", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "247", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "25165908", Function = "Shoes", Subfunction = "" },
            new S4Function(){ BodyType = "2556052", Function = "Face Paint", Subfunction = "" },
            new S4Function(){ BodyType = "2556160", Function = "Skin Detail", Subfunction = "Freckles" },
            new S4Function(){ BodyType = "25624660", Function = "Shoes", Subfunction = "" },
            new S4Function(){ BodyType = "262144", Function = "Teeth", Subfunction = "" },
            new S4Function(){ BodyType = "2621512", Function = "Skin Detail", Subfunction = "Dimple (L)" },
            new S4Function(){ BodyType = "26624", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "2687124", Function = "Skin Detail", Subfunction = "Dimple (R)" },
            new S4Function(){ BodyType = "27136", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "2752647", Function = "Accessory", Subfunction = "Tights" },
            new S4Function(){ BodyType = "2752686", Function = "Accessory", Subfunction = "Tights" },
            new S4Function(){ BodyType = "280", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "2818196", Function = "Skin Detail", Subfunction = "Cheek Mole (L)" },
            new S4Function(){ BodyType = "2818259", Function = "Skin Detail", Subfunction = "Lip Mole (L)" },
            new S4Function(){ BodyType = "282", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "283", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "286", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "288", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "2883656", Function = "Skin Detail", Subfunction = "Lip Mole (R)" },
            new S4Function(){ BodyType = "289", Function = "Hat", Subfunction = "" },
            new S4Function(){ BodyType = "291", Function = "Eyes", Subfunction = "Nondefault" },
            new S4Function(){ BodyType = "2949268", Function = "Tattoo", Subfunction = "Lower Arm (L)" },
            new S4Function(){ BodyType = "3014656", Function = "Tattoo", Subfunction = "Upper Arm (L)" },
            new S4Function(){ BodyType = "3014830", Function = "Skin Detail", Subfunction = "Cheek Mole (L)" },
            new S4Function(){ BodyType = "3080192", Function = "Tattoo", Subfunction = "Lower Arm (R)" },
            new S4Function(){ BodyType = "3080340", Function = "Tattoo", Subfunction = "Lower Arm (R)" },
            new S4Function(){ BodyType = "312", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "3145728", Function = "Tattoo", Subfunction = "Upper Arm (R)" },
            new S4Function(){ BodyType = "314624", Function = "Accessory", Subfunction = "Gloves" },
            new S4Function(){ BodyType = "3153664", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "3165952", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "327681", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "327690", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "3276948", Function = "Tattoo", Subfunction = "Leg (R)" },
            new S4Function(){ BodyType = "327700", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "327710", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "327713", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "327716", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "327729", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "327741", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "327753", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "327790", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "327827", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "327841", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "327844", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "327859", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "327863", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "327873", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "327885", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "327893", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "327919", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "327936", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "335", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "335360", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "33792", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "3408020", Function = "Tattoo", Subfunction = "Upper Back" },
            new S4Function(){ BodyType = "34865248", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "3604480", Function = "Skin Detail", Subfunction = "Cheek Mole (L)" },
            new S4Function(){ BodyType = "36096", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "3735552", Function = "Skin Detail", Subfunction = "Mouth Crease" },
            new S4Function(){ BodyType = "376", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "38273120", Function = "Accessory", Subfunction = "Glasses" },
            new S4Function(){ BodyType = "38338656", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "393217", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "393218", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "393249", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "393258", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "393259", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "393261", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "393276", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "393277", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "393291", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "393292", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "393296", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "393306", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "393331", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "393336", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "393362", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "393370", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "393391", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "393412", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "393416", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "393420", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "393422", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "393429", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "393447", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "393450", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "393452", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "393459", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "393460", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "393462", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "393464", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "393467", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "395008", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "3997696", Function = "Occult", Subfunction = "Tail" },
            new S4Function(){ BodyType = "4128768", Function = "Eyes", Subfunction = "Dog" },
            new S4Function(){ BodyType = "4259840", Function = "Occult", Subfunction = "Eye Socket" },
            new S4Function(){ BodyType = "4259988", Function = "Occult", Subfunction = "Eye Socket" },
            new S4Function(){ BodyType = "43384931", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "43450468", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "43516003", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "4390912", Function = "Occult", Subfunction = "Mouth" },
            new S4Function(){ BodyType = "4391060", Function = "Occult", Subfunction = "Mouth" },
            new S4Function(){ BodyType = "4456448", Function = "Occult", Subfunction = "Left Cheek" },
            new S4Function(){ BodyType = "4456513", Function = "Makeup", Subfunction = "Eyeshadow" },
            new S4Function(){ BodyType = "4456596", Function = "Occult", Subfunction = "Left Cheek" },
            new S4Function(){ BodyType = "4521984", Function = "Occult", Subfunction = "Right Cheek" },
            new S4Function(){ BodyType = "4587520", Function = "Occult", Subfunction = "Neck Scar" },
            new S4Function(){ BodyType = "458757", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "458772", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "458789", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "458792", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "458811", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "458848", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "458856", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "458864", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "458866", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "458884", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "458895", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "458904", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "458905", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "458909", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "458915", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "458927", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "458928", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "458931", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "458932", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "458934", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "458951", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "458961", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "458965", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "458966", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "458967", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "458972", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "458973", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "458981", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "458991", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "458998", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "459000", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "459001", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "459004", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "4653124", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "4718592", Function = "Skin Detail", Subfunction = "Acne" },
            new S4Function(){ BodyType = "4718660", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "4784128", Function = "Accessory", Subfunction = "Fingernails" },
            new S4Function(){ BodyType = "4849664", Function = "Accessory", Subfunction = "Toenails" },
            new S4Function(){ BodyType = "4849733", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "4980805", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "5046342", Function = "Accessory", Subfunction = "Earrings" },
            new S4Function(){ BodyType = "5111808", Function = "Body Hair", Subfunction = "Arm" },
            new S4Function(){ BodyType = "5111878", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "5177344", Function = "Body Hair", Subfunction = "Leg" },
            new S4Function(){ BodyType = "5242880", Function = "Body Hair", Subfunction = "Chest" },
            new S4Function(){ BodyType = "524289", Function = "Shoes", Subfunction = "" },
            new S4Function(){ BodyType = "524291", Function = "Shoes", Subfunction = "" },
            new S4Function(){ BodyType = "5242950", Function = "Tattoo", Subfunction = "Upper Chest" },
            new S4Function(){ BodyType = "524302", Function = "Shoes", Subfunction = "" },
            new S4Function(){ BodyType = "524326", Function = "Shoes", Subfunction = "" },
            new S4Function(){ BodyType = "524344", Function = "Shoes", Subfunction = "" },
            new S4Function(){ BodyType = "524368", Function = "Shoes", Subfunction = "" },
            new S4Function(){ BodyType = "524374", Function = "Shoes", Subfunction = "" },
            new S4Function(){ BodyType = "524376", Function = "Shoes", Subfunction = "" },
            new S4Function(){ BodyType = "524392", Function = "Shoes", Subfunction = "" },
            new S4Function(){ BodyType = "524408", Function = "Shoes", Subfunction = "" },
            new S4Function(){ BodyType = "524422", Function = "Shoes", Subfunction = "" },
            new S4Function(){ BodyType = "524433", Function = "Shoes", Subfunction = "" },
            new S4Function(){ BodyType = "524489", Function = "Shoes", Subfunction = "" },
            new S4Function(){ BodyType = "524507", Function = "Shoes", Subfunction = "" },
            new S4Function(){ BodyType = "5308416", Function = "Body Hair", Subfunction = "Back" },
            new S4Function(){ BodyType = "5308486", Function = "Accessory", Subfunction = "Glasses" },
            new S4Function(){ BodyType = "531", Function = "Accessory", Subfunction = "Wrist (R)" },
            new S4Function(){ BodyType = "532", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "53739619", Function = "Facial Hair", Subfunction = "" },
            new S4Function(){ BodyType = "53805156", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "5439558", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "5505092", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "5570628", Function = "Accessory", Subfunction = "Middle Finger (R)" },
            new S4Function(){ BodyType = "565248", Function = "Makeup", Subfunction = "Lipstick" },
            new S4Function(){ BodyType = "5767237", Function = "Skin Detail", Subfunction = "Cheek Mole (R)" },
            new S4Function(){ BodyType = "584", Function = "Accessory", Subfunction = "Earrings" },
            new S4Function(){ BodyType = "585", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "587", Function = "Shoes", Subfunction = "" },
            new S4Function(){ BodyType = "589824", Function = "Accessory", Subfunction = "Index Finger (R)" },
            new S4Function(){ BodyType = "60928", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "6094913", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "61440", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "62062675", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "62464", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "62720", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "63488", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "64553026", Function = "Accessory", Subfunction = "Nose Ring (R)" },
            new S4Function(){ BodyType = "64618562", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "655432", Function = "Accessory", Subfunction = "Earrings" },
            new S4Function(){ BodyType = "655549", Function = "Accessory", Subfunction = "Earrings" },
            new S4Function(){ BodyType = "65562", Function = "Hat", Subfunction = "" },
            new S4Function(){ BodyType = "65564", Function = "Hat", Subfunction = "" },
            new S4Function(){ BodyType = "65587", Function = "Hat", Subfunction = "" },
            new S4Function(){ BodyType = "65608", Function = "Hat", Subfunction = "" },
            new S4Function(){ BodyType = "65613", Function = "Hat", Subfunction = "" },
            new S4Function(){ BodyType = "65684", Function = "Hat", Subfunction = "" },
            new S4Function(){ BodyType = "65688", Function = "Hat", Subfunction = "" },
            new S4Function(){ BodyType = "65710", Function = "Hat", Subfunction = "" },
            new S4Function(){ BodyType = "65725", Function = "Hat", Subfunction = "" },
            new S4Function(){ BodyType = "65768", Function = "Hat", Subfunction = "" },
            new S4Function(){ BodyType = "65769", Function = "Hat", Subfunction = "" },
            new S4Function(){ BodyType = "65790", Function = "Hat", Subfunction = "" },
            new S4Function(){ BodyType = "662", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "664", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "672727106", Function = "Skin Detail", Subfunction = "Mouth Crease" },
            new S4Function(){ BodyType = "67328", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "68", Function = "Accessory", Subfunction = "Tights" },
            new S4Function(){ BodyType = "6881345", Function = "Accessory", Subfunction = "Earrings" },
            new S4Function(){ BodyType = "7012417", Function = "Face Paint", Subfunction = "" },
            new S4Function(){ BodyType = "71680", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "72", Function = "Hair", Subfunction = " " },
            new S4Function(){ BodyType = "720968", Function = "Accessory", Subfunction = "Glasses" },
            new S4Function(){ BodyType = "721085", Function = "Accessory", Subfunction = "Glasses" },
            new S4Function(){ BodyType = "72960", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "73", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "73728", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "73728", Function = "Shoes", Subfunction = "" },
            new S4Function(){ BodyType = "73984", Function = "Accessory", Subfunction = "Wrist (R)" },
            new S4Function(){ BodyType = "74496", Function = "Skin Detail", Subfunction = "Forehead" },
            new S4Function(){ BodyType = "75", Function = "Eyebrows", Subfunction = " " },
            new S4Function(){ BodyType = "75008", Function = "Accessory", Subfunction = "Wrist (L)" },
            new S4Function(){ BodyType = "79872", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "80", Function = "Makeup", Subfunction = "Lipstick" },
            new S4Function(){ BodyType = "80543814", Function = "Makeup", Subfunction = "Eyeliner" },
            new S4Function(){ BodyType = "81068115", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "820", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "822", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "83", Function = "Makeup", Subfunction = "Eyeshadow" },
            new S4Function(){ BodyType = "84", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "85", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "852116", Function = "Accessory", Subfunction = "Gloves" },
            new S4Function(){ BodyType = "852151", Function = "Accessory", Subfunction = "Gloves" },
            new S4Function(){ BodyType = "85760", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "8585291", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "89849926", Function = "Accessory", Subfunction = "Index Finger (L)" },
            new S4Function(){ BodyType = "904", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "91", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "917678", Function = "Accessory", Subfunction = "Wrist (L)" },
            new S4Function(){ BodyType = "92", Function = "Clothing", Subfunction = "Body" },
            new S4Function(){ BodyType = "9240652", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "93", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "932", Function = "Accessory", Subfunction = "Nose Ring (R)" },
            new S4Function(){ BodyType = "983040", Function = "Accessory", Subfunction = "Wrist (R)" },
            new S4Function(){ BodyType = "983112", Function = "Accessory", Subfunction = "Wrist (R)" },
            new S4Function(){ BodyType = "983229", Function = "Accessory", Subfunction = "Wrist (R)" },
            new S4Function(){ BodyType = "983273", Function = "Accessory", Subfunction = "Wrist (R)" },
            new S4Function(){ BodyType = "986", Function = "Hat", Subfunction = "" },
            new S4Function(){ BodyType = "98816", Function = "Shoes", Subfunction = "" },
            new S4Function(){ BodyType = "655360", Function = "Accessory", Subfunction = "Earring" },
            new S4Function(){ BodyType = "786432", Function = "Accessory", Subfunction = "Necklace" },
            new S4Function(){ BodyType = "917504", Function = "Accessory", Subfunction = "Bracelet" },
            new S4Function(){ BodyType = "131072", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "458752", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "720896", Function = "Accessory", Subfunction = "Glasses" },
            new S4Function(){ BodyType = "393228", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "393216", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "851980", Function = "Accessory", Subfunction = "Gloves" },
            new S4Function(){ BodyType = "65536", Function = "Accessory", Subfunction = "Hat" },
            new S4Function(){ BodyType = "327680", Function = "Clothing", Subfunction = "Full Body" },
            new S4Function(){ BodyType = "524288", Function = "Clothing", Subfunction = "Shoes" },
            new S4Function(){ BodyType = "2359296", Function = "Accessory", Subfunction = "Socks" },
            new S4Function(){ BodyType = "240", Function = "Accessory", Subfunction = "Hat" },
            new S4Function(){ BodyType = "2162688", Function = "Face Paint", Subfunction = "" },
            new S4Function(){ BodyType = "131149", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "1441792", Function = "Accessory", Subfunction = "Index Finger (L)" },
            new S4Function(){ BodyType = "1572864", Function = "Accessory", Subfunction = "Ring Finger (L)" },
            new S4Function(){ BodyType = "1245184", Function = "Accessory", Subfunction = "Nose Ring (R)" },
            new S4Function(){ BodyType = "3342336", Function = "Tattoo", Subfunction = "Lower Back" },
            new S4Function(){ BodyType = "851968", Function = "Accessory", Subfunction = "Gloves" },
            new S4Function(){ BodyType = "458913", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "3538944", Function = "Tattoo", Subfunction = "Upper Chest" },
            new S4Function(){ BodyType = "1966080", Function = "Makeup", Subfunction = "Eyeshadow" },
            new S4Function(){ BodyType = "2162836", Function = "Face Paint", Subfunction = "" },
            new S4Function(){ BodyType = "18176", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "393399", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "131151", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "1310720", Function = "Accessory", Subfunction = "Gloves" },
            new S4Function(){ BodyType = "1900544", Function = "Makeup", Subfunction = "Lipstick" },
            new S4Function(){ BodyType = "3473408", Function = "Skin Detail", Subfunction = "Forehead Crease" },
            new S4Function(){ BodyType = "1703936", Function = "Face Paint", Subfunction = "Sunscreen" },
            new S4Function(){ BodyType = "524342", Function = "Shoes", Subfunction = "" },
            new S4Function(){ BodyType = "2883732", Function = "Face Paint", Subfunction = "" },
            new S4Function(){ BodyType = "26880", Function = "Clothing", Subfunction = "Dress" },
            new S4Function(){ BodyType = "2752512", Function = "Accessory", Subfunction = "Tights" },
            new S4Function(){ BodyType = "2097152", Function = "Makeup", Subfunction = "Blush" },
            new S4Function(){ BodyType = "2031616", Function = "Makeup", Subfunction = "Eyeliner" },
            new S4Function(){ BodyType = "1179648", Function = "Accessory", Subfunction = "Nose Ring (L)" },
            new S4Function(){ BodyType = "2621440", Function = "Skin Detail", Subfunction = "Dimple (L)" },
            new S4Function(){ BodyType = "4194304", Function = "Occult", Subfunction = "Brow" },
            new S4Function(){ BodyType = "905", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "2818048", Function = "Skin Detail", Subfunction = "Mole (L Lip)" },
            new S4Function(){ BodyType = "252416", Function = "Clothing", Subfunction = "Bottom" },
            new S4Function(){ BodyType = "2490368", Function = "Skin Detail", Subfunction = "Forehead" },
            new S4Function(){ BodyType = "2883584", Function = "Skin Detail", Subfunction = "Mole (R)" },
            new S4Function(){ BodyType = "4325376", Function = "Occult", Subfunction = "Eye Lid" },
            new S4Function(){ BodyType = "391424", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "131182", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "2293760", Function = "Eyes", Subfunction = "Nondefault" },
            new S4Function(){ BodyType = "2228224", Function = "Eyebrows", Subfunction = "" },
            new S4Function(){ BodyType = "2555904", Function = "Skin Detail", Subfunction = "Freckles" },
            new S4Function(){ BodyType = "3276800", Function = "Tattoo", Subfunction = "Leg (R)" },
            new S4Function(){ BodyType = "71", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "391680", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "3211264", Function = "Tattoo", Subfunction = "Leg (L)" },
            new S4Function(){ BodyType = "2490516", Function = "Skin Detail", Subfunction = "Forehead" },
            new S4Function(){ BodyType = "78", Function = "Makeup", Subfunction = "Lipstick" },
            new S4Function(){ BodyType = "85852269", Function = "Shoes", Subfunction = "" },
            new S4Function(){ BodyType = "2490368", Function = "Skin Detail", Subfunction = "Forehead" },
            new S4Function(){ BodyType = "3670164", Function = "Skin Detail", Subfunction = "Mole (R Cheek)" },
            new S4Function(){ BodyType = "3407872", Function = "Tattoo", Subfunction = "Upper Back" },
            new S4Function(){ BodyType = "1507328", Function = "Accessory", Subfunction = "Index Finger (R)" },
            new S4Function(){ BodyType = "393409", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "131229", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "2949120", Function = "Tattoo", Subfunction = "Lower Arm (L)" },
            new S4Function(){ BodyType = "1376256", Function = "Accessory", Subfunction = "Brow Ring (R)" },
            new S4Function(){ BodyType = "393266", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "20447301", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "3014804", Function = "Tattoo", Subfunction = "Upper Arm (L)" },
            new S4Function(){ BodyType = "1310", Function = "Makeup", Subfunction = "Eyeshadow" },
            new S4Function(){ BodyType = "5374022", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "230400", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "77", Function = "Makeup", Subfunction = "Eyeliner" },
            new S4Function(){ BodyType = "393414", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "53870692", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "2686976", Function = "Skin Detail", Subfunction = "Dimple (R)" },
            new S4Function(){ BodyType = "821", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "3670016", Function = "Skin Detail", Subfunction = "Mole (R Lip)" },
            new S4Function(){ BodyType = "807338093", Function = "Eyes", Subfunction = "Nondefault" },
            new S4Function(){ BodyType = "19712", Function = "Tattoo", Subfunction = "Lower Back" },
            new S4Function(){ BodyType = "393227", Function = "Clothing", Subfunction = "Top" },
            new S4Function(){ BodyType = "5963776", Function = "Skin Detail", Subfunction = "Birthmark (Face)" },
            new S4Function(){ BodyType = "131307", Function = "Hair", Subfunction = "" },
            new S4Function(){ BodyType = "2883584", Function = "Skin Detail", Subfunction = "Mole (R Lip)" },
            new S4Function(){ BodyType = "458790", Function = "Clothing", Subfunction = "Bottom" }
        };
    
        public static List<S4CategoryTag> CategoryTags = new(){
            new S4CategoryTag(){ TypeID = "84", Description = "AgeAppropriate_Adult", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "85", Description = "AgeAppropriate_Child", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "72", Description = "AgeAppropriate_Elder", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "291", Description = "AgeAppropriate_Teen", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1657", Description = "AgeAppropriate_Toddler", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "71", Description = "AgeAppropriate_YoungAdult", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61494", Description = "AppearanceModifier_HairMakeupChair_HairStyle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61609", Description = "AppearanceModifier_HairMakeUpChair_MakeUp", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "406", Description = "Appropriateness_Bartending", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "402", Description = "Appropriateness_Bathing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "605", Description = "Appropriateness_Cake", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1170", Description = "Appropriateness_CallToMeal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "404", Description = "Appropriateness_Cleaning", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1373", Description = "Appropriateness_Computer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "405", Description = "Appropriateness_Cooking", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "603", Description = "Appropriateness_Dancing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "604", Description = "Appropriateness_Eating", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12413", Description = "Appropriateness_FrontDesk", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "939", Description = "Appropriateness_GrabSnack", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "367", Description = "Appropriateness_Guest", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "368", Description = "Appropriateness_HiredWorker", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "370", Description = "Appropriateness_Host", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1274", Description = "Appropriateness_NotDuringWork", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1275", Description = "Appropriateness_NotDuringWork_Lunch", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1594", Description = "Appropriateness_Phone", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1626", Description = "Appropriateness_PhoneGame", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1539", Description = "Appropriateness_Playing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2156", Description = "Appropriateness_PlayInstrument", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1276", Description = "Appropriateness_ReadBooks", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "369", Description = "Appropriateness_ServiceNPC", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "352", Description = "Appropriateness_Shower", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55385", Description = "Appropriateness_Singing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "403", Description = "Appropriateness_Sleeping", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1645", Description = "Appropriateness_SocialPicker", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "530", Description = "Appropriateness_Stereo", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2155", Description = "Appropriateness_Tip", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1526", Description = "Appropriateness_Touching", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12423", Description = "Appropriateness_Trash", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1273", Description = "Appropriateness_TV_Watching", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12428", Description = "Appropriateness_View", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1497", Description = "Appropriateness_Visitor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12297", Description = "Appropriateness_Work_Scientist", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1277", Description = "Appropriateness_Workout", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "73", Description = "Archetype_African", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "75", Description = "Archetype_Asian", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "76", Description = "Archetype_Caucasian", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2206", Description = "Archetype_Island", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "312", Description = "Archetype_Latin", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "74", Description = "Archetype_MiddleEastern", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "89", Description = "Archetype_NorthAmerican", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "88", Description = "Archetype_SouthAsian", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2194", Description = "AtPo_Beach", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2204", Description = "AtPo_Beach_Walkby", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55386", Description = "AtPo_Blossom_Guru", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1571", Description = "AtPo_Busker", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1915", Description = "AtPo_Dynamic_SpawnPoint", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55399", Description = "AtPo_Fireworks", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55334", Description = "AtPo_FleaMarket_Vendor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1916", Description = "AtPo_GoForWalk", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57394", Description = "AtPo_GoForWalk_Long", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57432", Description = "AtPo_GoForWalk_Long_02", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57433", Description = "AtPo_GoForWalk_Long_03", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57436", Description = "AtPo_GoForWalk_Med_02", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57437", Description = "AtPo_GoForWalk_Med_03", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57393", Description = "AtPo_GoForWalk_Medium", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57389", Description = "AtPo_GoForWalk_Short", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57434", Description = "AtPo_GoForWalk_Short_02", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57435", Description = "AtPo_GoForWalk_Short_03", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2158", Description = "AtPo_Guitar", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2222", Description = "AtPo_MagicDueling", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1582", Description = "AtPo_Protester", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1570", Description = "AtPo_Tourist", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2230", Description = "AtPo_UniversityQuad", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1235", Description = "Bottom_Bikini", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "945", Description = "Bottom_Cropped", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "382", Description = "Bottom_Jeans", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "381", Description = "Bottom_Leggings", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "152", Description = "Bottom_Pants", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "154", Description = "Bottom_Shorts", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "153", Description = "Bottom_Skirt", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1238", Description = "Bottom_Swimshort", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1544", Description = "Bottom_Swimwear", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1543", Description = "Bottom_Underwear", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "946", Description = "Bottom_Underwear_Female", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1040", Description = "Bottom_Underwear_Male", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1830", Description = "Breed_Cat_Abyssinian", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1831", Description = "Breed_Cat_American_Bobtail", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1931", Description = "Breed_Cat_American_Longhair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1833", Description = "Breed_Cat_American_Shorthair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1834", Description = "Breed_Cat_American_Wirehair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1835", Description = "Breed_Cat_Balinese", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1836", Description = "Breed_Cat_Bengal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1837", Description = "Breed_Cat_Birman", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1838", Description = "Breed_Cat_Black_Cat", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1839", Description = "Breed_Cat_Bombay", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1840", Description = "Breed_Cat_British_Longhair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1841", Description = "Breed_Cat_British_Shorthair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1842", Description = "Breed_Cat_Burmese", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1843", Description = "Breed_Cat_Calico", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1844", Description = "Breed_Cat_Chartreux", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1845", Description = "Breed_Cat_Colorpoint_Shorthair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1832", Description = "Breed_Cat_CornishRex", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1846", Description = "Breed_Cat_Devon_Rex", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1847", Description = "Breed_Cat_Egyptian_Mau", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1848", Description = "Breed_Cat_German_Rex", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1849", Description = "Breed_Cat_Havana_Brown", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1850", Description = "Breed_Cat_Himalyan", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1851", Description = "Breed_Cat_Japanese_Bobtail", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1852", Description = "Breed_Cat_Javanese", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1853", Description = "Breed_Cat_Korat", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1854", Description = "Breed_Cat_Kurilian_Bobtail", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1855", Description = "Breed_Cat_LaPerm", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1975", Description = "Breed_Cat_Lyoki", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1856", Description = "Breed_Cat_Maine_Coon", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1857", Description = "Breed_Cat_Manx", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1926", Description = "Breed_Cat_Mixed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1858", Description = "Breed_Cat_Norwegian_Forest", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1859", Description = "Breed_Cat_Ocicat", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1860", Description = "Breed_Cat_Oriental", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1861", Description = "Breed_Cat_Oriental_Shorthair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1862", Description = "Breed_Cat_Persian", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1974", Description = "Breed_Cat_Raccoon", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1863", Description = "Breed_Cat_Ragdoll", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1864", Description = "Breed_Cat_Russian_Blue", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1865", Description = "Breed_Cat_Savannah", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1866", Description = "Breed_Cat_Scottish_Fold", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1867", Description = "Breed_Cat_Shorthair_Tabby", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1868", Description = "Breed_Cat_Siamese", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1869", Description = "Breed_Cat_Siberian", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1870", Description = "Breed_Cat_Singapura", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1871", Description = "Breed_Cat_Somali", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1886", Description = "Breed_Cat_Sphynx", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1872", Description = "Breed_Cat_Tonkinese", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1873", Description = "Breed_Cat_Turkish_Angora", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1874", Description = "Breed_Cat_Tuxedo_Cat", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1814", Description = "Breed_LargeDog_Afghan_Hound", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1745", Description = "Breed_LargeDog_Airedale_Terrier", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1746", Description = "Breed_LargeDog_Akita", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1747", Description = "Breed_LargeDog_Alaskan_Malamute", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1748", Description = "Breed_LargeDog_American_Eskimo", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1797", Description = "Breed_LargeDog_American_Foxhound", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1750", Description = "Breed_LargeDog_Australian_Cattle_Dog", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1735", Description = "Breed_LargeDog_AustralianShepherd", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1950", Description = "Breed_LargeDog_Bedlington_Terrier", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1751", Description = "Breed_LargeDog_Bernese_Mountain_Dog", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1798", Description = "Breed_LargeDog_Black_And_Tan_Coonhound", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1961", Description = "Breed_LargeDog_Black_Russian_Terrier", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1753", Description = "Breed_LargeDog_Bloodhound", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1796", Description = "Breed_LargeDog_Bluetick_Coonhound", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1736", Description = "Breed_LargeDog_BorderCollie", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1826", Description = "Breed_LargeDog_Borzoi", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1755", Description = "Breed_LargeDog_Boxer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1816", Description = "Breed_LargeDog_Brittany", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1951", Description = "Breed_LargeDog_Bullmastiff", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1952", Description = "Breed_LargeDog_Canaan", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1795", Description = "Breed_LargeDog_Chesapeake_Bay_Retriever", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1759", Description = "Breed_LargeDog_Chow_Chow", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1953", Description = "Breed_LargeDog_ChowLabMix", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1740", Description = "Breed_LargeDog_Collie", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1794", Description = "Breed_LargeDog_Curly_Coated_Retriever", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1741", Description = "Breed_LargeDog_Dalmatian", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1954", Description = "Breed_LargeDog_Dingo", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1742", Description = "Breed_LargeDog_Doberman", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1761", Description = "Breed_LargeDog_Doberman_Pinscher", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1821", Description = "Breed_LargeDog_English_Foxhound", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1819", Description = "Breed_LargeDog_English_Setter", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1762", Description = "Breed_LargeDog_English_Springer_Spaniel", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1801", Description = "Breed_LargeDog_Field_Spaniel", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1737", Description = "Breed_LargeDog_GermanPointer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1743", Description = "Breed_LargeDog_GermanShepherd", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1792", Description = "Breed_LargeDog_Giant_Schnauzer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1800", Description = "Breed_LargeDog_Golden_Doodle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1731", Description = "Breed_LargeDog_GoldenRetriever", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1955", Description = "Breed_LargeDog_Great_Pyranees", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1734", Description = "Breed_LargeDog_GreatDane", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1764", Description = "Breed_LargeDog_Greyhound", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1744", Description = "Breed_LargeDog_Husky", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1738", Description = "Breed_LargeDog_Ibizan", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1802", Description = "Breed_LargeDog_Irish_Red_And_White_Setter", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1803", Description = "Breed_LargeDog_Irish_Setter", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1828", Description = "Breed_LargeDog_Irish_Terrier", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1827", Description = "Breed_LargeDog_Irish_Wolfhound", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1767", Description = "Breed_LargeDog_Keeshond", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1956", Description = "Breed_LargeDog_Kerry_Blue_Terrier", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1957", Description = "Breed_LargeDog_Labradoodle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1768", Description = "Breed_LargeDog_Labrador_Retriever", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1804", Description = "Breed_LargeDog_Mastiff", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1928", Description = "Breed_LargeDog_Mixed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1769", Description = "Breed_LargeDog_Newfoundland", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1958", Description = "Breed_LargeDog_Norsk_Elk_Shepherd", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1771", Description = "Breed_LargeDog_Old_English_Sheepdog", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1772", Description = "Breed_LargeDog_Otterhound", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1774", Description = "Breed_LargeDog_Pharaoh_Hound", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1749", Description = "Breed_LargeDog_Pitbull", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1775", Description = "Breed_LargeDog_Pointer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1807", Description = "Breed_LargeDog_Polish_Lowland_Sheepdog", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1777", Description = "Breed_LargeDog_Poodle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1791", Description = "Breed_LargeDog_Portuguese_Water_Dog", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1810", Description = "Breed_LargeDog_Redbone_Coonhound", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1815", Description = "Breed_LargeDog_Rhodesian_Ridgeback", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1779", Description = "Breed_LargeDog_Rottweiler", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1780", Description = "Breed_LargeDog_Saint_Bernard", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1781", Description = "Breed_LargeDog_Samoyed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1732", Description = "Breed_LargeDog_Schnauzer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1959", Description = "Breed_LargeDog_Shar_Pei", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1812", Description = "Breed_LargeDog_Siberian_Husky", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1960", Description = "Breed_LargeDog_Tibetan_Mastiff", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1809", Description = "Breed_LargeDog_Vizsla", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1788", Description = "Breed_LargeDog_Weimaraner", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1808", Description = "Breed_LargeDog_Welsh_Springer_Spaniel", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1962", Description = "Breed_LargeDog_Wheatens_Terrier", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1733", Description = "Breed_None", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1817", Description = "Breed_SmallDog_Basenji", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1739", Description = "Breed_SmallDog_Beagle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1752", Description = "Breed_SmallDog_Bichon_Frise", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1963", Description = "Breed_SmallDog_Bocker", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1754", Description = "Breed_SmallDog_Boston_Terrier", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1829", Description = "Breed_SmallDog_Bull_Terrier", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1756", Description = "Breed_SmallDog_Bulldog", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1964", Description = "Breed_SmallDog_Cardigan_Welsh_Corgi", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1757", Description = "Breed_SmallDog_Cavalier_King_Charles_Spaniel", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1758", Description = "Breed_SmallDog_Chihuahua", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1760", Description = "Breed_SmallDog_Chocker_Spaniel", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1965", Description = "Breed_SmallDog_Cockapoo", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1966", Description = "Breed_SmallDog_Daschund", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1818", Description = "Breed_SmallDog_English_Cocker_Spaniel", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1967", Description = "Breed_SmallDog_English_Toy_Spaniel", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1968", Description = "Breed_SmallDog_Fox", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1763", Description = "Breed_SmallDog_French_Bulldog", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1793", Description = "Breed_SmallDog_Havanese", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1993", Description = "Breed_SmallDog_Icelandic_Sheep_Dog", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1825", Description = "Breed_SmallDog_Italian_Greyhound", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1766", Description = "Breed_SmallDog_Jack_Russel_Terrier", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1823", Description = "Breed_SmallDog_Lhasa_Apso", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1943", Description = "Breed_SmallDog_Maltese", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1805", Description = "Breed_SmallDog_Miniature_Pinscher", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1969", Description = "Breed_SmallDog_Miniature_Poodle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1806", Description = "Breed_SmallDog_Miniature_Schnauzer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1927", Description = "Breed_SmallDog_Mixed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1992", Description = "Breed_SmallDog_Norweigian_Buhund", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1773", Description = "Breed_SmallDog_Papillon", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1970", Description = "Breed_SmallDog_Parson_Russel_Terrier", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1770", Description = "Breed_SmallDog_Pekingese", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1971", Description = "Breed_SmallDog_Pembroke_Welsh_Corgi", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1776", Description = "Breed_SmallDog_Pomeranian", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1778", Description = "Breed_SmallDog_Pug", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1820", Description = "Breed_SmallDog_Puggle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1782", Description = "Breed_SmallDog_Schipperke", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1972", Description = "Breed_SmallDog_Schnoodle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1783", Description = "Breed_SmallDog_Scottish_Terrier", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1811", Description = "Breed_SmallDog_Shetland_Sheepdog", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1784", Description = "Breed_SmallDog_Shiba_Inu", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1785", Description = "Breed_SmallDog_Shih_Tzu", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1973", Description = "Breed_SmallDog_Silky_Terrier", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1813", Description = "Breed_SmallDog_Smooth_Fox_Terrier", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1991", Description = "Breed_SmallDog_Spitz", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1824", Description = "Breed_SmallDog_Staffordshire_Bull_Terrier", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1786", Description = "Breed_SmallDog_Standard_Schnauzer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1787", Description = "Breed_SmallDog_Toy_Fox_Terrier", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1822", Description = "Breed_SmallDog_West_Highland_White_Terrier", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1799", Description = "Breed_SmallDog_Whippet", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1789", Description = "Breed_SmallDog_Wire_Fox_Terrier", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1790", Description = "Breed_SmallDog_Yorkshire_Terrier", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1893", Description = "BreedGroup_Herding", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1894", Description = "BreedGroup_Hound", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1911", Description = "BreedGroup_NonSporting", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1895", Description = "BreedGroup_Sporting", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1896", Description = "BreedGroup_Terrier", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1897", Description = "BreedGroup_Toy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1898", Description = "BreedGroup_Working", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2154", Description = "Buff_AppearanceModifier_MakeUp", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1551", Description = "Buff_Business_CustomerStarRating", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1548", Description = "Buff_Business_EmployeeTraining", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49168", Description = "Buff_Cauldron_Potion_MakeGlowy_Failure_VFX", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1678", Description = "Buff_DayNightTracking", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65653", Description = "Buff_HumanoidRobot_MoodVFX", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45079", Description = "Buff_MysticalRelic_Curse", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2150", Description = "Buff_OwnableRestaurant_Customer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47139", Description = "Buff_PossessedBuffs", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47148", Description = "Buff_PossessedBuffs_NoAnimate", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49157", Description = "Buff_Spells_CastingSpell", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59446", Description = "Buff_Temperature", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "40989", Description = "Buff_VampireSunlight", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59431", Description = "Buff_Weather", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "561", Description = "Build_Arch", Function = "Build Mode", Subfunction = "Arch" },
            new S4CategoryTag(){ TypeID = "2419", Description = "Build_BBGameplayEffect_Columns_BillsDecrease", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2420", Description = "Build_BBGameplayEffect_Columns_BillsIncrease", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2413", Description = "Build_BBGameplayEffect_Columns_EcoFootprint_Minus1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2414", Description = "Build_BBGameplayEffect_Columns_EcoFootprint_Minus2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2411", Description = "Build_BBGameplayEffect_Columns_EcoFootprint_Plus1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2412", Description = "Build_BBGameplayEffect_Columns_EcoFootprint_Plus2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2417", Description = "Build_BBGameplayEffect_Columns_EnvironmentScore_Minus1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2418", Description = "Build_BBGameplayEffect_Columns_EnvironmentScore_Minus2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2415", Description = "Build_BBGameplayEffect_Columns_EnvironmentScore_Plus1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2416", Description = "Build_BBGameplayEffect_Columns_EnvironmentScore_Plus2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2409", Description = "Build_BBGameplayEffect_Fences_BillsDecrease", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2410", Description = "Build_BBGameplayEffect_Fences_BillsIncrease", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2403", Description = "Build_BBGameplayEffect_Fences_EcoFootprint_Minus1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2404", Description = "Build_BBGameplayEffect_Fences_EcoFootprint_Minus2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2401", Description = "Build_BBGameplayEffect_Fences_EcoFootprint_Plus1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2402", Description = "Build_BBGameplayEffect_Fences_EcoFootprint_Plus2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2407", Description = "Build_BBGameplayEffect_Fences_EnvironmentScore_Minus1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2408", Description = "Build_BBGameplayEffect_Fences_EnvironmentScore_Minus2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2405", Description = "Build_BBGameplayEffect_Fences_EnvironmentScore_Plus1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2406", Description = "Build_BBGameplayEffect_Fences_EnvironmentScore_Plus2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2329", Description = "Build_BBGameplayEffect_FloorPattern_DecreaseBills", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2308", Description = "Build_BBGameplayEffect_FloorPattern_EcoFootprint_Minus1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2309", Description = "Build_BBGameplayEffect_FloorPattern_EcoFootprint_Minus2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2306", Description = "Build_BBGameplayEffect_FloorPattern_EcoFootprint_Plus1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2307", Description = "Build_BBGameplayEffect_FloorPattern_EcoFootprint_Plus2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2296", Description = "Build_BBGameplayEffect_FloorPattern_EnvironmentScore_Minus1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2297", Description = "Build_BBGameplayEffect_FloorPattern_EnvironmentScore_Minus2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2294", Description = "Build_BBGameplayEffect_FloorPattern_EnvironmentScore_Plus1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2295", Description = "Build_BBGameplayEffect_FloorPattern_EnvironmentScore_Plus2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2328", Description = "Build_BBGameplayEffect_FloorPattern_IncreaseBills", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2327", Description = "Build_BBGameplayEffect_Object_DecreaseBills", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2300", Description = "Build_BBGameplayEffect_Object_EcoFootprint_Minus1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2301", Description = "Build_BBGameplayEffect_Object_EcoFootprint_Minus2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2298", Description = "Build_BBGameplayEffect_Object_EcoFootprint_Plus1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2299", Description = "Build_BBGameplayEffect_Object_EcoFootprint_Plus2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2444", Description = "Build_BBGameplayEffect_Object_EcoFootprint_PlusPark", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2288", Description = "Build_BBGameplayEffect_Object_EnvironmentScore_Minus1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2289", Description = "Build_BBGameplayEffect_Object_EnvironmentScore_Minus2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2286", Description = "Build_BBGameplayEffect_Object_EnvironmentScore_Plus1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2287", Description = "Build_BBGameplayEffect_Object_EnvironmentScore_Plus2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2326", Description = "Build_BBGameplayEffect_Object_IncreaseBills", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2314", Description = "Build_BBGameplayEffect_Object_PowerConsumer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2316", Description = "Build_BBGameplayEffect_Object_PowerProducer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2315", Description = "Build_BBGameplayEffect_Object_WaterConsumer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2317", Description = "Build_BBGameplayEffect_Object_WaterProducer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2322", Description = "Build_BBGameplayEffect_PoolSurface_PowerConsumer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2324", Description = "Build_BBGameplayEffect_PoolSurface_PowerProducer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2323", Description = "Build_BBGameplayEffect_PoolSurface_WaterConsumer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2325", Description = "Build_BBGameplayEffect_PoolSurface_WaterProducer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2333", Description = "Build_BBGameplayEffect_RoofMaterial_DecreaseBills", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2312", Description = "Build_BBGameplayEffect_RoofMaterial_EcoFootprint_Minus1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2313", Description = "Build_BBGameplayEffect_RoofMaterial_EcoFootprint_Minus2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2310", Description = "Build_BBGameplayEffect_RoofMaterial_EcoFootprint_Plus1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2311", Description = "Build_BBGameplayEffect_RoofMaterial_EcoFootprint_Plus2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2319", Description = "Build_BBGameplayEffect_RoofMaterial_EnvironmentScore_Minus1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2318", Description = "Build_BBGameplayEffect_RoofMaterial_EnvironmentScore_Plus1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2332", Description = "Build_BBGameplayEffect_RoofMaterial_IncreaseBills", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2320", Description = "Build_BBGameplayEffect_RoofMaterial_PowerProducer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2321", Description = "Build_BBGameplayEffect_RoofMaterial_WaterProducer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2331", Description = "Build_BBGameplayEffect_WallPattern_DecreaseBills", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2304", Description = "Build_BBGameplayEffect_WallPattern_EcoFootprint_Minus1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2305", Description = "Build_BBGameplayEffect_WallPattern_EcoFootprint_Minus2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2302", Description = "Build_BBGameplayEffect_WallPattern_EcoFootprint_Plus1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2303", Description = "Build_BBGameplayEffect_WallPattern_EcoFootprint_Plus2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2292", Description = "Build_BBGameplayEffect_WallPattern_EnvironmentScore_Minus1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2293", Description = "Build_BBGameplayEffect_WallPattern_EnvironmentScore_Minus2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2290", Description = "Build_BBGameplayEffect_WallPattern_EnvironmentScore_Plus1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2291", Description = "Build_BBGameplayEffect_WallPattern_EnvironmentScore_Plus2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2330", Description = "Build_BBGameplayEffect_WallPattern_IncreaseBills", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "548", Description = "Build_Block", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "242", Description = "Build_Block_Basement", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1062", Description = "Build_Block_Deck", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1070", Description = "Build_Block_Diagonal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "232", Description = "Build_Block_Fountain", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "233", Description = "Build_Block_FountainTool", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1064", Description = "Build_Block_NoWalls", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1226", Description = "Build_Block_Pool", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1227", Description = "Build_Block_PoolTool", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "653", Description = "Build_Block_WallTool", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1063", Description = "Build_Block_WithWalls", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1638", Description = "Build_Buy_Autonomy_Marker_Attractor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1637", Description = "Build_Buy_NoAutonomy_Lights", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1636", Description = "Build_Buy_NoAutonomy_Plants", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1639", Description = "Build_Buy_NoAutonomy_Rugs", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1634", Description = "Build_Buy_NoAutonomy_Sculptures", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "787", Description = "Build_Buy_World_Objects", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "538", Description = "Build_Column", Function = "Build Mode", Subfunction = "Column" },
            new S4CategoryTag(){ TypeID = "535", Description = "Build_Door", Function = "Build Mode", Subfunction = "Door" },
            new S4CategoryTag(){ TypeID = "918", Description = "Build_DoorDouble", Function = "Build Mode", Subfunction = "Door" },
            new S4CategoryTag(){ TypeID = "974", Description = "Build_DoorSingle", Function = "Build Mode", Subfunction = "Door" },
            new S4CategoryTag(){ TypeID = "1611", Description = "Build_Elevator", Function = "Build Mode", Subfunction = "Elevator" },
            new S4CategoryTag(){ TypeID = "544", Description = "Build_Fence", Function = "Build Mode", Subfunction = "Fence" },
            new S4CategoryTag(){ TypeID = "541", Description = "Build_FloorPattern", Function = "Build Mode", Subfunction = "Floor" },
            new S4CategoryTag(){ TypeID = "554", Description = "Build_FloorTrim", Function = "Build Mode", Subfunction = "Floor Trim" },
            new S4CategoryTag(){ TypeID = "556", Description = "Build_Flower", Function = "Build Mode", Subfunction = "Flower" },
            new S4CategoryTag(){ TypeID = "1068", Description = "Build_Flower_Bush", Function = "Build Mode", Subfunction = "Flower Bush" },
            new S4CategoryTag(){ TypeID = "1067", Description = "Build_Flower_GroundCover", Function = "Build Mode", Subfunction = "Flower Ground Cover" },
            new S4CategoryTag(){ TypeID = "1069", Description = "Build_Flower_Misc", Function = "Build Mode", Subfunction = "Flower Misc" },
            new S4CategoryTag(){ TypeID = "552", Description = "Build_Foundation", Function = "Build Mode", Subfunction = "Foundation" },
            new S4CategoryTag(){ TypeID = "1081", Description = "Build_FountainTrim", Function = "Build Mode", Subfunction = "Fountain Trim" },
            new S4CategoryTag(){ TypeID = "550", Description = "Build_Frieze", Function = "Build Mode", Subfunction = "Frieze" },
            new S4CategoryTag(){ TypeID = "537", Description = "Build_Gate", Function = "Build Mode", Subfunction = "Gate" },
            new S4CategoryTag(){ TypeID = "915", Description = "Build_GateDouble", Function = "Build Mode", Subfunction = "Gate" },
            new S4CategoryTag(){ TypeID = "976", Description = "Build_GateSingle", Function = "Build Mode", Subfunction = "Gate" },
            new S4CategoryTag(){ TypeID = "1596", Description = "Build_Generic", Function = "Build Mode", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1441", Description = "Build_HalfWall", Function = "Build Mode", Subfunction = "Half Wall" },
            new S4CategoryTag(){ TypeID = "1442", Description = "Build_HalfWallTrim", Function = "Build Mode", Subfunction = "Half Wall Trim" },
            new S4CategoryTag(){ TypeID = "1574", Description = "Build_IsShellBuilding", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2425", Description = "Build_Ladder", Function = "Build Mode", Subfunction = "Ladder" },
            new S4CategoryTag(){ TypeID = "251", Description = "Build_PoolStyles", Function = "Build Mode", Subfunction = "Pool" },
            new S4CategoryTag(){ TypeID = "250", Description = "Build_PoolTrim", Function = "Build Mode", Subfunction = "Pool Trim" },
            new S4CategoryTag(){ TypeID = "782", Description = "Build_Post", Function = "Build Mode", Subfunction = "Post" },
            new S4CategoryTag(){ TypeID = "547", Description = "Build_Railing", Function = "Build Mode", Subfunction = "Railing" },
            new S4CategoryTag(){ TypeID = "560", Description = "Build_Rock", Function = "Build Mode", Subfunction = "Rock" },
            new S4CategoryTag(){ TypeID = "540", Description = "Build_Roof", Function = "Build Mode", Subfunction = "Roof" },
            new S4CategoryTag(){ TypeID = "539", Description = "Build_RoofAttachment", Function = "Build Mode", Subfunction = "Roof Attachment" },
            new S4CategoryTag(){ TypeID = "975", Description = "Build_RoofAttachmentMisc", Function = "Build Mode", Subfunction = "Roof Attachment" },
            new S4CategoryTag(){ TypeID = "919", Description = "Build_RoofChimney", Function = "Build Mode", Subfunction = "Chimney" },
            new S4CategoryTag(){ TypeID = "906", Description = "Build_RoofDiagonal", Function = "Build Mode", Subfunction = "Roof Diagonal" },
            new S4CategoryTag(){ TypeID = "977", Description = "Build_RoofOrthogonal", Function = "Build Mode", Subfunction = "Roof Orthogonal" },
            new S4CategoryTag(){ TypeID = "543", Description = "Build_RoofPattern", Function = "Build Mode", Subfunction = "Roof Pattern" },
            new S4CategoryTag(){ TypeID = "551", Description = "Build_RoofTrim", Function = "Build Mode", Subfunction = "Roof Trim" },
            new S4CategoryTag(){ TypeID = "559", Description = "Build_Rug", Function = "Build Mode", Subfunction = "Rug" },
            new S4CategoryTag(){ TypeID = "557", Description = "Build_Shrub", Function = "Build Mode", Subfunction = "Shrub" },
            new S4CategoryTag(){ TypeID = "1065", Description = "Build_Shrub_Bush", Function = "Build Mode", Subfunction = "Bush" },
            new S4CategoryTag(){ TypeID = "1066", Description = "Build_Shrub_Cactus", Function = "Build Mode", Subfunction = "Cactus" },
            new S4CategoryTag(){ TypeID = "545", Description = "Build_Spandrel", Function = "Build Mode", Subfunction = "Spandrel" },
            new S4CategoryTag(){ TypeID = "546", Description = "Build_Stair", Function = "Build Mode", Subfunction = "Stairs" },
            new S4CategoryTag(){ TypeID = "549", Description = "Build_Style", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "558", Description = "Build_Tree", Function = "Build Mode", Subfunction = "Tree" },
            new S4CategoryTag(){ TypeID = "555", Description = "Build_WallAttachment", Function = "Build Mode", Subfunction = "Wall Attachment" },
            new S4CategoryTag(){ TypeID = "542", Description = "Build_WallPattern", Function = "Build Mode", Subfunction = "Wall Pattern" },
            new S4CategoryTag(){ TypeID = "981", Description = "Build_WeddingArch", Function = "Build Mode", Subfunction = "Wedding Arch" },
            new S4CategoryTag(){ TypeID = "536", Description = "Build_Window", Function = "Build Mode", Subfunction = "Window" },
            new S4CategoryTag(){ TypeID = "67591", Description = "BuyCat_CleanPower", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1044", Description = "BuyCat_Collection_Alien", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1053", Description = "BuyCat_Collection_ALL", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55378", Description = "BuyCat_Collection_CityPoster", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1041", Description = "BuyCat_Collection_Crystal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1042", Description = "BuyCat_Collection_Element", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1051", Description = "BuyCat_Collection_Fish", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1043", Description = "BuyCat_Collection_Fossil", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1052", Description = "BuyCat_Collection_Frog", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1159", Description = "BuyCat_Collection_Gardening", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1045", Description = "BuyCat_Collection_Metal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1046", Description = "BuyCat_Collection_MySim", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1049", Description = "BuyCat_Collection_Postcard", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1048", Description = "BuyCat_Collection_Slide", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55377", Description = "BuyCat_Collection_Snowglobe", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1047", Description = "BuyCat_Collection_SpacePrint", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1050", Description = "BuyCat_Collection_SpaceRock", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2043", Description = "BuyCat_Collection_Treasure", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "429", Description = "BuyCat_Columns", Function = "Column", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1352", Description = "BuyCat_Community", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "440", Description = "BuyCat_Easel", Function = "Easel", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2084", Description = "BuyCat_Holiday_All", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2085", Description = "BuyCat_Holiday_Decor_All", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "441", Description = "BuyCat_Instrument", Function = "Instrument", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55374", Description = "BuyCat_LotReq_Elevator", Function = "Elevator", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2240", Description = "BuyCat_LotReq_Elevator_BG", Function = "Elevator", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55375", Description = "BuyCat_LotReq_Mailbox", Function = "Mailbox", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2241", Description = "BuyCat_LotReq_Mailbox_BG", Function = "Mailbox", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55376", Description = "BuyCat_LotReq_TrashChute", Function = "Trash Chute", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2242", Description = "BuyCat_LotReq_TrashChute_BG", Function = "Trash Chute", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2380", Description = "BuyCat_OTG_Appliances", Function = "Off The Grid", Subfunction = "Appliances" },
            new S4CategoryTag(){ TypeID = "2381", Description = "BuyCat_OTG_Crafting", Function = "Off The Grid", Subfunction = "Crafting" },
            new S4CategoryTag(){ TypeID = "2382", Description = "BuyCat_OTG_Lighting", Function = "Off The Grid", Subfunction = "Lighting" },
            new S4CategoryTag(){ TypeID = "2383", Description = "BuyCat_OTG_Misc", Function = "Off The Grid", Subfunction = "Misc" },
            new S4CategoryTag(){ TypeID = "2384", Description = "BuyCat_OTG_OutdoorActivities", Function = "Off The Grid", Subfunction = "Outdoor Activities" },
            new S4CategoryTag(){ TypeID = "2385", Description = "BuyCat_OTG_Plumbing", Function = "Off The Grid", Subfunction = "Plumbing" },
            new S4CategoryTag(){ TypeID = "446", Description = "BuyCat_Painting", Function = "Decorations", Subfunction = "Painting" },
            new S4CategoryTag(){ TypeID = "1261", Description = "BuyCat_Shareable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "430", Description = "BuyCat_SpanrelsFriezesTrim", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1604", Description = "BuyCat_Venue_ArtsCenter", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2273", Description = "BuyCat_Venue_ArtsCommons", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1353", Description = "BuyCat_Venue_Bar", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2199", Description = "BuyCat_Venue_Beach", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24612", Description = "BuyCat_Venue_Bluffs", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24578", Description = "BuyCat_Venue_Cafe", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24611", Description = "BuyCat_Venue_Chalet", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1354", Description = "BuyCat_Venue_Club", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2438", Description = "BuyCat_Venue_CommunitySpace_Default", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2440", Description = "BuyCat_Venue_CommunitySpace_Garden", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2439", Description = "BuyCat_Venue_CommunitySpace_MakerSpace", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2441", Description = "BuyCat_Venue_CommunitySpace_Marketplace", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1362", Description = "BuyCat_Venue_DoctorClinic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1355", Description = "BuyCat_Venue_ForestPark", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1356", Description = "BuyCat_Venue_Gym", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1579", Description = "BuyCat_Venue_Karaoke", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1357", Description = "BuyCat_Venue_Library", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1358", Description = "BuyCat_Venue_Lounge", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1359", Description = "BuyCat_Venue_Museum", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1360", Description = "BuyCat_Venue_Park", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55373", Description = "BuyCat_Venue_Penthouse", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2239", Description = "BuyCat_Venue_Penthouse_BG", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1363", Description = "BuyCat_Venue_PoliceStation", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1459", Description = "BuyCat_Venue_Pool", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18436", Description = "BuyCat_Venue_RelaxationCenter", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "26625", Description = "BuyCat_Venue_Restaurant", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1361", Description = "BuyCat_Venue_Retail", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24613", Description = "BuyCat_Venue_Ruins", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2272", Description = "BuyCat_Venue_ScienceCommons", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1364", Description = "BuyCat_Venue_ScientistLab", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1580", Description = "BuyCat_Venue_StarGarden", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2229", Description = "BuyCat_Venue_UniversityHousing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57401", Description = "BuyCat_Venue_Vet", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "428", Description = "BuyCat_Windows", Function = "Window", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "970", Description = "BuyCatEE_ActiveActivity", Function = "Activities & Skills", Subfunction = "Active" },
            new S4CategoryTag(){ TypeID = "169", Description = "BuyCatEE_Alarm", Function = "Electronics", Subfunction = "Alarm" },
            new S4CategoryTag(){ TypeID = "163", Description = "BuyCatEE_Audio", Function = "Electronics", Subfunction = "Audio" },
            new S4CategoryTag(){ TypeID = "176", Description = "BuyCatEE_Bar", Function = "Activities & Skills", Subfunction = "Bar" },
            new S4CategoryTag(){ TypeID = "456", Description = "BuyCatEE_Basketball", Function = "Activities & Skills", Subfunction = "Basketball" },
            new S4CategoryTag(){ TypeID = "457", Description = "BuyCatEE_Chess_Table", Function = "Activities & Skills", Subfunction = "Chess" },
            new S4CategoryTag(){ TypeID = "171", Description = "BuyCatEE_Clock", Function = "Electronics", Subfunction = "Clock" },
            new S4CategoryTag(){ TypeID = "162", Description = "BuyCatEE_Computer", Function = "Electronics", Subfunction = "Computer" },
            new S4CategoryTag(){ TypeID = "968", Description = "BuyCatEE_CreativeActivity", Function = "Activities & Skills", Subfunction = "Creative" },
            new S4CategoryTag(){ TypeID = "2075", Description = "BuyCatEE_Gardening", Function = "Activities & Skills", Subfunction = "Gardening" },
            new S4CategoryTag(){ TypeID = "165", Description = "BuyCatEE_HobbySkill", Function = "Electronics", Subfunction = "Hobbies" },
            new S4CategoryTag(){ TypeID = "173", Description = "BuyCatEE_IndoorActivity", Function = "Activities & Skills", Subfunction = "Indoor" },
            new S4CategoryTag(){ TypeID = "174", Description = "BuyCatEE_KidActivity", Function = "Kids", Subfunction = "Activities" },
            new S4CategoryTag(){ TypeID = "167", Description = "BuyCatEE_KidFurniture", Function = "Kids", Subfunction = "Furniture" },
            new S4CategoryTag(){ TypeID = "168", Description = "BuyCatEE_KidToy", Function = "Kids", Subfunction = "Toys" },
            new S4CategoryTag(){ TypeID = "969", Description = "BuyCatEE_KnowledgeActivity", Function = "Activities & Skills", Subfunction = "Knowledge" },
            new S4CategoryTag(){ TypeID = "177", Description = "BuyCatEE_MiscElectronics", Function = "Electronics", Subfunction = "Misc" },
            new S4CategoryTag(){ TypeID = "178", Description = "BuyCatEE_MiscEntertainment", Function = "Entertainment", Subfunction = "Misc" },
            new S4CategoryTag(){ TypeID = "179", Description = "BuyCatEE_MiscKids", Function = "Kids", Subfunction = "Misc" },
            new S4CategoryTag(){ TypeID = "458", Description = "BuyCatEE_MonkeyBars", Function = "Activities & Skills", Subfunction = "Kids" },
            new S4CategoryTag(){ TypeID = "175", Description = "BuyCatEE_OutdoorActivity", Function = "Activities & Skills", Subfunction = "Outdoor" },
            new S4CategoryTag(){ TypeID = "166", Description = "BuyCatEE_Party", Function = "Electronics", Subfunction = "Party" },
            new S4CategoryTag(){ TypeID = "2014", Description = "BuyCatEE_PetActivityToys", Function = "Pets", Subfunction = "Activities" },
            new S4CategoryTag(){ TypeID = "1948", Description = "BuyCatEE_PetMisc", Function = "Pets", Subfunction = "Misc" },
            new S4CategoryTag(){ TypeID = "1944", Description = "BuyCatEE_PetToys", Function = "Pets", Subfunction = "Toys" },
            new S4CategoryTag(){ TypeID = "1947", Description = "BuyCatEE_PetVet", Function = "Pets", Subfunction = "Vets" },
            new S4CategoryTag(){ TypeID = "172", Description = "BuyCatEE_Toddlers", Function = "Kids", Subfunction = "Toddlers" },
            new S4CategoryTag(){ TypeID = "2237", Description = "BuyCatEE_Transportation", Function = "Transportation", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "161", Description = "BuyCatEE_TV", Function = "Electronics", Subfunction = "TV" },
            new S4CategoryTag(){ TypeID = "164", Description = "BuyCatEE_TVSets", Function = "Electronics", Subfunction = "TV" },
            new S4CategoryTag(){ TypeID = "1122", Description = "BuyCatEE_TVStand", Function = "Surfaces", Subfunction = "TV Stands" },
            new S4CategoryTag(){ TypeID = "55356", Description = "BuyCatEE_VideoGameConsole", Function = "Electronics", Subfunction = "Video Game Console" },
            new S4CategoryTag(){ TypeID = "979", Description = "BuyCatLD_Awning", Function = "Decorations", Subfunction = "Awning" },
            new S4CategoryTag(){ TypeID = "194", Description = "BuyCatLD_BathroomAccent", Function = "Decorations", Subfunction = "Bathroom" },
            new S4CategoryTag(){ TypeID = "2188", Description = "BuyCatLD_CeilingDecoration", Function = "Decorations", Subfunction = "Ceiling" },
            new S4CategoryTag(){ TypeID = "205", Description = "BuyCatLD_CeilingLight", Function = "Lights", Subfunction = "Ceiling" },
            new S4CategoryTag(){ TypeID = "823", Description = "BuyCatLD_Clutter", Function = "Decorations", Subfunction = "Clutter" },
            new S4CategoryTag(){ TypeID = "978", Description = "BuyCatLD_CurtainBlind", Function = "Decorations", Subfunction = "Curtain" },
            new S4CategoryTag(){ TypeID = "785", Description = "BuyCatLD_Fireplace", Function = "Decorations", Subfunction = "Fireplace" },
            new S4CategoryTag(){ TypeID = "204", Description = "BuyCatLD_FloorLamp", Function = "Lights", Subfunction = "Floor Lamp" },
            new S4CategoryTag(){ TypeID = "199", Description = "BuyCatLD_FountainDecoration", Function = "Decorations", Subfunction = "Fountain" },
            new S4CategoryTag(){ TypeID = "231", Description = "BuyCatLD_FountainEmitter", Function = "Decorations", Subfunction = "Fountain" },
            new S4CategoryTag(){ TypeID = "252", Description = "BuyCatLD_FountainObjects", Function = "Decorations", Subfunction = "Fountain" },
            new S4CategoryTag(){ TypeID = "196", Description = "BuyCatLD_KidDecoration", Function = "Kids", Subfunction = "Decor" },
            new S4CategoryTag(){ TypeID = "195", Description = "BuyCatLD_LawnOrnament", Function = "Decorations", Subfunction = "Lawn" },
            new S4CategoryTag(){ TypeID = "207", Description = "BuyCatLD_Mirror", Function = "Decorations", Subfunction = "Mirror" },
            new S4CategoryTag(){ TypeID = "965", Description = "BuyCatLD_MirrorFreestanding", Function = "Decorations", Subfunction = "Freestanding Mirror" },
            new S4CategoryTag(){ TypeID = "964", Description = "BuyCatLD_MirrorWall", Function = "Decorations", Subfunction = "Wall Mirror" },
            new S4CategoryTag(){ TypeID = "209", Description = "BuyCatLD_MiscDecoration", Function = "Decorations", Subfunction = "Misc" },
            new S4CategoryTag(){ TypeID = "208", Description = "BuyCatLD_MiscLight", Function = "Lights", Subfunction = "Misc" },
            new S4CategoryTag(){ TypeID = "1718", Description = "BuyCatLD_NightLight", Function = "Lights", Subfunction = "Night Light" },
            new S4CategoryTag(){ TypeID = "206", Description = "BuyCatLD_OutdoorLight", Function = "Lights", Subfunction = "Outdoor" },
            new S4CategoryTag(){ TypeID = "202", Description = "BuyCatLD_Plant", Function = "Decorations", Subfunction = "Plant" },
            new S4CategoryTag(){ TypeID = "1246", Description = "BuyCatLD_PoolDecorations", Function = "Decorations", Subfunction = "Pool" },
            new S4CategoryTag(){ TypeID = "1228", Description = "BuyCatLD_PoolObjects", Function = "Pool", Subfunction = "Objects" },
            new S4CategoryTag(){ TypeID = "2211", Description = "BuyCatLD_PoolObjectsInventoryable", Function = "Pool", Subfunction = "Objects" },
            new S4CategoryTag(){ TypeID = "198", Description = "BuyCatLD_Rug", Function = "Decorations", Subfunction = "Rug" },
            new S4CategoryTag(){ TypeID = "1496", Description = "BuyCatLD_RugManaged", Function = "Decorations", Subfunction = "Rug" },
            new S4CategoryTag(){ TypeID = "200", Description = "BuyCatLD_Sculpture", Function = "Decorations", Subfunction = "Sculpture" },
            new S4CategoryTag(){ TypeID = "203", Description = "BuyCatLD_TableLamp", Function = "Decorations", Subfunction = "Table Lamp" },
            new S4CategoryTag(){ TypeID = "201", Description = "BuyCatLD_WallDecoration", Function = "Decorations", Subfunction = "Wall" },
            new S4CategoryTag(){ TypeID = "310", Description = "BuyCatLD_WallLight", Function = "Lights", Subfunction = "Wall" },
            new S4CategoryTag(){ TypeID = "824", Description = "BuyCatLD_WallSculpture", Function = "Decorations", Subfunction = "Wall" },
            new S4CategoryTag(){ TypeID = "197", Description = "BuyCatLD_WindowTreatment", Function = "Decorations", Subfunction = "Window" },
            new S4CategoryTag(){ TypeID = "271", Description = "BuyCatMAG_Bathroom", Function = "Bathroom", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "272", Description = "BuyCatMAG_Bedroom", Function = "Bedroom", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "468", Description = "BuyCatMAG_Career", Function = "Career", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "273", Description = "BuyCatMAG_DiningRoom", Function = "Dining Room", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "864", Description = "BuyCatMAG_Kids", Function = "Kids", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "274", Description = "BuyCatMAG_Kitchen", Function = "Kitchen", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "270", Description = "BuyCatMAG_LivingRoom", Function = "Living Room", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "407", Description = "BuyCatMAG_Misc", Function = "Misc", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "275", Description = "BuyCatMAG_Outdoor", Function = "Outdoor", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "276", Description = "BuyCatMAG_Study", Function = "Study", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "966", Description = "BuyCatPA_CoffeeMaker", Function = "Appliances", Subfunction = "Coffee Maker" },
            new S4CategoryTag(){ TypeID = "188", Description = "BuyCatPA_Disposable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "972", Description = "BuyCatPA_DisposalIndoor", Function = "Appliances", Subfunction = "Trash" },
            new S4CategoryTag(){ TypeID = "973", Description = "BuyCatPA_DisposalOutdoor", Function = "Appliances", Subfunction = "Trash" },
            new S4CategoryTag(){ TypeID = "185", Description = "BuyCatPA_LargeAppliance", Function = "Appliances", Subfunction = "Large" },
            new S4CategoryTag(){ TypeID = "1978", Description = "BuyCatPA_LitterBox", Function = "Pets", Subfunction = "Litter Box" },
            new S4CategoryTag(){ TypeID = "967", Description = "BuyCatPA_Microwave", Function = "Appliances", Subfunction = "Microwave" },
            new S4CategoryTag(){ TypeID = "193", Description = "BuyCatPA_MiscAppliance", Function = "Appliances", Subfunction = "Misc" },
            new S4CategoryTag(){ TypeID = "192", Description = "BuyCatPA_MiscPlumbing", Function = "Plumbing", Subfunction = "Misc" },
            new S4CategoryTag(){ TypeID = "191", Description = "BuyCatPA_MiscSmallAppliance", Function = "Appliances", Subfunction = "Misc Small" },
            new S4CategoryTag(){ TypeID = "190", Description = "BuyCatPA_OutdoorCooking", Function = "Appliances", Subfunction = "Outdoor" },
            new S4CategoryTag(){ TypeID = "1945", Description = "BuyCatPA_PetCare", Function = "Pets", Subfunction = "Pet Care" },
            new S4CategoryTag(){ TypeID = "1976", Description = "BuyCatPA_PetFood", Function = "Pets", Subfunction = "Pet Food" },
            new S4CategoryTag(){ TypeID = "2042", Description = "BuyCatPA_PublicRestroom", Function = "Plumbing", Subfunction = "Public" },
            new S4CategoryTag(){ TypeID = "189", Description = "BuyCatPA_Refrigerator", Function = "Appliances", Subfunction = "Fridge" },
            new S4CategoryTag(){ TypeID = "183", Description = "BuyCatPA_Shower", Function = "Plumbing", Subfunction = "Shower" },
            new S4CategoryTag(){ TypeID = "180", Description = "BuyCatPA_Sink", Function = "Plumbing", Subfunction = "Sink" },
            new S4CategoryTag(){ TypeID = "920", Description = "BuyCatPA_SinkCounter", Function = "Plumbing", Subfunction = "Counter Sink" },
            new S4CategoryTag(){ TypeID = "182", Description = "BuyCatPA_SinkFreestanding", Function = "Plumbing", Subfunction = "Freestanding Sink" },
            new S4CategoryTag(){ TypeID = "186", Description = "BuyCatPA_SmallAppliance", Function = "Appliances", Subfunction = "Small Appliances" },
            new S4CategoryTag(){ TypeID = "187", Description = "BuyCatPA_Stove", Function = "Appliances", Subfunction = "Stove" },
            new S4CategoryTag(){ TypeID = "913", Description = "BuyCatPA_StoveHood", Function = "Appliances", Subfunction = "Stove Hood" },
            new S4CategoryTag(){ TypeID = "181", Description = "BuyCatPA_Toilet", Function = "Plumbing", Subfunction = "Toilet" },
            new S4CategoryTag(){ TypeID = "184", Description = "BuyCatPA_Tub", Function = "Plumbing", Subfunction = "Bathtub" },
            new S4CategoryTag(){ TypeID = "1123", Description = "BuyCatSS_AccentTable", Function = "Surfaces", Subfunction = "Accent Table" },
            new S4CategoryTag(){ TypeID = "224", Description = "BuyCatSS_Barstool", Function = "Comfort", Subfunction = "Stools" },
            new S4CategoryTag(){ TypeID = "225", Description = "BuyCatSS_Bed", Function = "Comfort", Subfunction = "Bed" },
            new S4CategoryTag(){ TypeID = "914", Description = "BuyCatSS_BedDouble", Function = "Comfort", Subfunction = "Double Bed" },
            new S4CategoryTag(){ TypeID = "971", Description = "BuyCatSS_BedSingle", Function = "Comfort", Subfunction = "Single Bed" },
            new S4CategoryTag(){ TypeID = "226", Description = "BuyCatSS_Bookshelf", Function = "Activities & Skills", Subfunction = "Bookshelf" },
            new S4CategoryTag(){ TypeID = "211", Description = "BuyCatSS_Cabinet", Function = "Surfaces", Subfunction = "Cabinet" },
            new S4CategoryTag(){ TypeID = "214", Description = "BuyCatSS_CoffeeTable", Function = "Surfaces", Subfunction = "Coffee Table" },
            new S4CategoryTag(){ TypeID = "210", Description = "BuyCatSS_Counter", Function = "Surfaces", Subfunction = "Counter" },
            new S4CategoryTag(){ TypeID = "215", Description = "BuyCatSS_Desk", Function = "Surfaces", Subfunction = "Desk" },
            new S4CategoryTag(){ TypeID = "222", Description = "BuyCatSS_DeskChair", Function = "Comfort", Subfunction = "Desk Chair" },
            new S4CategoryTag(){ TypeID = "217", Description = "BuyCatSS_DiningChair", Function = "Comfort", Subfunction = "Dining Chair" },
            new S4CategoryTag(){ TypeID = "212", Description = "BuyCatSS_DiningTable", Function = "Surfaces", Subfunction = "Dining Table" },
            new S4CategoryTag(){ TypeID = "963", Description = "BuyCatSS_DiningTableLong", Function = "Surfaces", Subfunction = "Dining Table Long" },
            new S4CategoryTag(){ TypeID = "962", Description = "BuyCatSS_DiningTableShort", Function = "Surfaces", Subfunction = "Dining Table Short" },
            new S4CategoryTag(){ TypeID = "216", Description = "BuyCatSS_Display", Function = "Surfaces", Subfunction = "Display" },
            new S4CategoryTag(){ TypeID = "227", Description = "BuyCatSS_Dresser", Function = "Surfaces", Subfunction = "Dresser" },
            new S4CategoryTag(){ TypeID = "1072", Description = "BuyCatSS_ElementDisplay", Function = "Surfaces", Subfunction = "Element Display" },
            new S4CategoryTag(){ TypeID = "213", Description = "BuyCatSS_EndTable", Function = "Surfaces", Subfunction = "End Table" },
            new S4CategoryTag(){ TypeID = "1126", Description = "BuyCatSS_HallwayTable", Function = "Surfaces", Subfunction = "Hallway Table" },
            new S4CategoryTag(){ TypeID = "221", Description = "BuyCatSS_LivingChair", Function = "Comfort", Subfunction = "Living Chair" },
            new S4CategoryTag(){ TypeID = "219", Description = "BuyCatSS_LoveSeat", Function = "Comfort", Subfunction = "Loveseat" },
            new S4CategoryTag(){ TypeID = "229", Description = "BuyCatSS_MiscComfort", Function = "Comfort", Subfunction = "Misc" },
            new S4CategoryTag(){ TypeID = "230", Description = "BuyCatSS_MiscStorage", Function = "Storage", Subfunction = "Misc" },
            new S4CategoryTag(){ TypeID = "228", Description = "BuyCatSS_MiscSurface", Function = "Surfaces", Subfunction = "Misc" },
            new S4CategoryTag(){ TypeID = "916", Description = "BuyCatSS_OutdoorBench", Function = "Comfort", Subfunction = "Outdoor Bench" },
            new S4CategoryTag(){ TypeID = "220", Description = "BuyCatSS_OutdoorChair", Function = "Comfort", Subfunction = "Outdoor Chair" },
            new S4CategoryTag(){ TypeID = "223", Description = "BuyCatSS_OutdoorSeating", Function = "Comfort", Subfunction = "Outdoor Seating" },
            new S4CategoryTag(){ TypeID = "917", Description = "BuyCatSS_OutdoorTable", Function = "Comfort", Subfunction = "Outdoor Table" },
            new S4CategoryTag(){ TypeID = "1977", Description = "BuyCatSS_PetBed", Function = "Pets", Subfunction = "Bed" },
            new S4CategoryTag(){ TypeID = "1946", Description = "BuyCatSS_PetFurniture", Function = "Pets", Subfunction = "Furniture" },
            new S4CategoryTag(){ TypeID = "1071", Description = "BuyCatSS_PostcardBoard", Function = "Decorations", Subfunction = "Postcard Board" },
            new S4CategoryTag(){ TypeID = "1979", Description = "BuyCatSS_ScratchingPost", Function = "Pets", Subfunction = "Scratching Post" },
            new S4CategoryTag(){ TypeID = "218", Description = "BuyCatSS_Sofa", Function = "Comfort", Subfunction = "Sofa" },
            new S4CategoryTag(){ TypeID = "43017", Description = "BuyTag_DisablePlacementOutline", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2274", Description = "BuyTag_NotAutoCounterAppliance", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1492", Description = "BuyTag_ShowIfWallsCutaway", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2213", Description = "CAS_Story_Add_Career", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2212", Description = "CAS_Story_Add_Funds", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2215", Description = "CAS_Story_Add_Occult", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2214", Description = "CAS_Story_Add_Skill", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2004", Description = "CoatPattern_Bicolor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1995", Description = "CoatPattern_Brindle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2006", Description = "CoatPattern_Calico", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2019", Description = "CoatPattern_Colorpoint", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2009", Description = "CoatPattern_Fantasy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2022", Description = "CoatPattern_Harlequin", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2001", Description = "CoatPattern_Mask", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1999", Description = "CoatPattern_Merle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2008", Description = "CoatPattern_Rosette", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1996", Description = "CoatPattern_Sable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2000", Description = "CoatPattern_Saddle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1994", Description = "CoatPattern_Solid", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1998", Description = "CoatPattern_Speckled", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1997", Description = "CoatPattern_Spotted", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2003", Description = "CoatPattern_Striped", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2002", Description = "CoatPattern_Tabby", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2007", Description = "CoatPattern_Tipped", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2005", Description = "CoatPattern_Tortoiseshell", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2021", Description = "CoatPattern_TriColor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "93", Description = "Color_Black", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "68", Description = "Color_Blue", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "91", Description = "Color_Brown", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "293", Description = "Color_BrownLight", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "90", Description = "Color_DarkBrown", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "92", Description = "Color_Gray", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "69", Description = "Color_Green", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "95", Description = "Color_Orange", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "106", Description = "Color_Pink", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "107", Description = "Color_Purple", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65", Description = "Color_Red", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "105", Description = "Color_White", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "104", Description = "Color_Yellow", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "280", Description = "ColorPalette_EarthTones", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "288", Description = "ColorPalette_GothRockPunk", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "282", Description = "ColorPalette_GrayscaleDark", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "283", Description = "ColorPalette_GrayscaleLight", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "141", Description = "ColorPalette_Jewel", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "285", Description = "ColorPalette_Spring", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "286", Description = "ColorPalette_Summer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "287", Description = "ColorPalette_Winter", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "424", Description = "Crafting_Gardening", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "447", Description = "Crafting_Song", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1892", Description = "DogSize_Large", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1891", Description = "DogSize_Small", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "264", Description = "Drink_Alcoholic", Function = "MOD: Drink", Subfunction = "Alcoholic" },
            new S4CategoryTag(){ TypeID = "269", Description = "Drink_Any", Function = "MOD: Drink", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "351", Description = "Drink_Crafted", Function = "MOD: Drink", Subfunction = "Crafted" },
            new S4CategoryTag(){ TypeID = "459", Description = "Drink_Crafted_Coffee_Tea", Function = "MOD: Drink", Subfunction = "Crafted Coffee or Tea" },
            new S4CategoryTag(){ TypeID = "18451", Description = "Drink_Fizzy", Function = "MOD: Drink", Subfunction = "Fizzy" },
            new S4CategoryTag(){ TypeID = "63538", Description = "Drink_Kava", Function = "MOD: Drink", Subfunction = "Kava" },
            new S4CategoryTag(){ TypeID = "265", Description = "Drink_NonAlcoholic", Function = "MOD: Drink", Subfunction = "Non-Alcoholic" },
            new S4CategoryTag(){ TypeID = "12290", Description = "Drink_Serum", Function = "MOD: Drink", Subfunction = "Serum" },
            new S4CategoryTag(){ TypeID = "691", Description = "Drink_SpaceEnergy", Function = "MOD: Drink", Subfunction = "Space Energy" },
            new S4CategoryTag(){ TypeID = "1661", Description = "Drink_Toddler", Function = "MOD: Drink", Subfunction = "Toddlers" },
            new S4CategoryTag(){ TypeID = "159", Description = "Drinks_Any", Function = "MOD: Drink", Subfunction = "Bar Drink" },
            new S4CategoryTag(){ TypeID = "157", Description = "Drinks_Bar_Alcoholic", Function = "MOD: Drink", Subfunction = "Alcoholic Bar Drink" },
            new S4CategoryTag(){ TypeID = "160", Description = "Drinks_Bar_Any", Function = "MOD: Drink", Subfunction = "Bar Drink" },
            new S4CategoryTag(){ TypeID = "158", Description = "Drinks_Bar_NonAlcoholic", Function = "MOD: Drink", Subfunction = "Non-Alcoholic Bar Drink" },
            new S4CategoryTag(){ TypeID = "57450", Description = "DuplicateAffordance_Counter", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49184", Description = "DuplicateAffordance_MagicHQ_BeAmazed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49185", Description = "DuplicateAffordance_MagicHQ_BrowseBooks", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2172", Description = "DuplicateAffordance_Mirror", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1173", Description = "DuplicateAffordance_Read", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57449", Description = "DuplicateAffordance_Scratch", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2096", Description = "DuplicateAffordance_Sink", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1697", Description = "DuplicateAffordance_ToysPickUp", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1696", Description = "DuplicateAffordance_ToysPlayWith", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1174", Description = "DuplicateAffordance_TraitInteractions", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1175", Description = "DuplicateAffordance_View", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57347", Description = "Ears_Down", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57348", Description = "Ears_Up", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63537", Description = "Ensemble_FinOrangeRed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63535", Description = "Ensemble_FinPastel", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63536", Description = "Ensemble_FinTealPurple", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1257", Description = "Ensemble_SwimBandeauBlack", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1258", Description = "Ensemble_SwimBandeauBlue", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1251", Description = "Ensemble_SwimBandeauCoral", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1254", Description = "Ensemble_SwimBandeauYellow", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1239", Description = "Ensemble_SwimHalterBlack", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1255", Description = "Ensemble_SwimHalterLime", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1252", Description = "Ensemble_SwimHalterRed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1256", Description = "Ensemble_SwimHalterWhite", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1259", Description = "Ensemble_SwimMetalBrown", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1260", Description = "Ensemble_SwimMetalGreen", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1250", Description = "Ensemble_SwimMetalPink", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1253", Description = "Ensemble_SwimMetalTeal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1060", Description = "EyebrowShape_Arched", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1059", Description = "EyebrowShape_Curved", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1058", Description = "EyebrowShape_Straight", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12393", Description = "EyebrowThickness_Bald", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1054", Description = "EyebrowThickness_Bushy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1057", Description = "EyebrowThickness_Medium", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1056", Description = "EyebrowThickness_Sparse", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1055", Description = "EyebrowThickness_Thin", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12392", Description = "EyeColor_Alien", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "114", Description = "EyeColor_Amber", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "115", Description = "EyeColor_Aqua", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "116", Description = "EyeColor_Black", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "117", Description = "EyeColor_Blue", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1884", Description = "EyeColor_BlueGray", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "118", Description = "EyeColor_Brown", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "119", Description = "EyeColor_DarkBrown", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "423", Description = "EyeColor_Golden", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "120", Description = "EyeColor_Gray", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "121", Description = "EyeColor_Green", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "421", Description = "EyeColor_Hazel", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "122", Description = "EyeColor_HazelBlue", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "123", Description = "EyeColor_HazelGreen", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "422", Description = "EyeColor_Honey", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "124", Description = "EyeColor_LightBlue", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "125", Description = "EyeColor_LightBrown", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "126", Description = "EyeColor_LightGreen", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1880", Description = "EyeColor_LightYellow", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "40988", Description = "EyeColor_VampireBlack", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "40980", Description = "EyeColor_VampireBlueBlack", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "40981", Description = "EyeColor_VampireGreen", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "40982", Description = "EyeColor_VampireIceBlue", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "40983", Description = "EyeColor_VampirePurple", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "40984", Description = "EyeColor_VampireRed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "40985", Description = "EyeColor_VampireRedBlack", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "40986", Description = "EyeColor_VampireWhite", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "40987", Description = "EyeColor_VampireYellow", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1879", Description = "EyeColor_Yellow", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1885", Description = "EyeColor_YellowGreen", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "532", Description = "Fabric_Cotton", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "587", Description = "Fabric_Denim", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "531", Description = "Fabric_Leather", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "932", Description = "Fabric_Metal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "585", Description = "Fabric_Silk", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "933", Description = "Fabric_Silver", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "584", Description = "Fabric_Synthetic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "586", Description = "Fabric_Wool", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1651", Description = "FaceDetail_FrecklesNose", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1650", Description = "FaceDetail_FrecklesSpread", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1647", Description = "FaceDetail_TeethBuck", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1649", Description = "FaceDetail_TeethGap", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1648", Description = "FaceDetail_TeethSnaggle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1652", Description = "FaceDetail_TeethStraight", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "378", Description = "FacialHair_Beard", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "379", Description = "FacialHair_Goatee", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "380", Description = "FacialHair_Mustache", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1925", Description = "Fire_Flammable_AutoAdded", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "298", Description = "FloorPattern_Carpet", Function = "Floor", Subfunction = "Carpet" },
            new S4CategoryTag(){ TypeID = "309", Description = "FloorPattern_DirtSand", Function = "Floor", Subfunction = "Dirt or Sand" },
            new S4CategoryTag(){ TypeID = "308", Description = "FloorPattern_Flowers", Function = "Floor", Subfunction = "Flowers" },
            new S4CategoryTag(){ TypeID = "307", Description = "FloorPattern_Grass", Function = "Floor", Subfunction = "Grass" },
            new S4CategoryTag(){ TypeID = "303", Description = "FloorPattern_Linoleum", Function = "Floor", Subfunction = "Linoleum" },
            new S4CategoryTag(){ TypeID = "302", Description = "FloorPattern_Masonry", Function = "Floor", Subfunction = "Masonry" },
            new S4CategoryTag(){ TypeID = "304", Description = "FloorPattern_Metal", Function = "Floor", Subfunction = "Metal" },
            new S4CategoryTag(){ TypeID = "305", Description = "FloorPattern_Misc", Function = "Floor", Subfunction = "Misc" },
            new S4CategoryTag(){ TypeID = "306", Description = "FloorPattern_Outdoor", Function = "Floor", Subfunction = "Outdoor" },
            new S4CategoryTag(){ TypeID = "301", Description = "FloorPattern_Stone", Function = "Floor", Subfunction = "Stone" },
            new S4CategoryTag(){ TypeID = "299", Description = "FloorPattern_Tile", Function = "Floor", Subfunction = "Tile" },
            new S4CategoryTag(){ TypeID = "300", Description = "FloorPattern_Wood", Function = "Floor", Subfunction = "Wood" },
            new S4CategoryTag(){ TypeID = "268", Description = "Food_Any", Function = "MOD: Food", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1614", Description = "Food_Aromatic", Function = "MOD: Food", Subfunction = "Aromatic" },
            new S4CategoryTag(){ TypeID = "2203", Description = "Food_BeachBum", Function = "MOD: Food", Subfunction = "Beach Bum" },
            new S4CategoryTag(){ TypeID = "1602", Description = "Food_Burrito", Function = "MOD: Food", Subfunction = "Burrito" },
            new S4CategoryTag(){ TypeID = "65551", Description = "Food_CafeteriaStation_Pranked", Function = "MOD: Food", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10263", Description = "Food_Campfire", Function = "MOD: Food", Subfunction = "Campfire" },
            new S4CategoryTag(){ TypeID = "55379", Description = "Food_Chopsticks", Function = "MOD: Food", Subfunction = "Chopsticks" },
            new S4CategoryTag(){ TypeID = "359", Description = "Food_Dessert", Function = "MOD: Food", Subfunction = "Dessert" },
            new S4CategoryTag(){ TypeID = "1980", Description = "Food_Dish_Bowl", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1981", Description = "Food_Dish_Plate", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1988", Description = "Food_Dish_ShortFood", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1987", Description = "Food_Dish_TallFood", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1675", Description = "Food_EatWithToddlerSized", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1674", Description = "Food_EatWithUtensil", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1687", Description = "Food_FoodBlob_Applesauce_LightBrown", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1688", Description = "Food_FoodBlob_FruitSalad_RedYellowBlue", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1689", Description = "Food_FoodBlob_MacCheese_YellowSpotty", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1690", Description = "Food_FoodBlob_Minestrone_ReddishBrown", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1691", Description = "Food_FoodBlob_Oatmeal_LightBrownSpotty", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1693", Description = "Food_FoodBlob_Peas_Green", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1692", Description = "Food_FoodBlob_Yogurt_PinkWhitish", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "348", Description = "Food_Fridge", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2083", Description = "Food_GrandMeal_ep05", Function = "MOD: Food", Subfunction = "Grand Meal" },
            new S4CategoryTag(){ TypeID = "1499", Description = "Food_GrilledCheese", Function = "MOD: Food", Subfunction = "Grilled Cheese" },
            new S4CategoryTag(){ TypeID = "2201", Description = "Food_HasFish", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1572", Description = "Food_HasMeat", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1573", Description = "Food_HasMeatSubstitute", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1984", Description = "Food_ICO", Function = "MOD: Food", Subfunction = "ICO" },
            new S4CategoryTag(){ TypeID = "63511", Description = "Food_Island", Function = "MOD: Food", Subfunction = "Island" },
            new S4CategoryTag(){ TypeID = "45089", Description = "Food_Jungle", Function = "MOD: Food", Subfunction = "Jungle" },
            new S4CategoryTag(){ TypeID = "63512", Description = "Food_KaluaPork", Function = "MOD: Food", Subfunction = "Kalua Pork" },
            new S4CategoryTag(){ TypeID = "1728", Description = "Food_Meal_Breakfast", Function = "MOD: Food", Subfunction = "Breakfast Meal" },
            new S4CategoryTag(){ TypeID = "1730", Description = "Food_Meal_Dinner", Function = "MOD: Food", Subfunction = "Dinner Meal" },
            new S4CategoryTag(){ TypeID = "1729", Description = "Food_Meal_Lunch", Function = "MOD: Food", Subfunction = "Lunch Meal" },
            new S4CategoryTag(){ TypeID = "347", Description = "Food_Multi", Function = "MOD: Food", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1717", Description = "Food_PickyEater_Dislike", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1712", Description = "Food_PickyEaterA_Like", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1713", Description = "Food_PickyEaterB_Like", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1714", Description = "Food_PickyEaterC_Like", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1715", Description = "Food_PickyEaterD_Like", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1716", Description = "Food_PickyEaterE_Like", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "759", Description = "Food_Prepared", Function = "MOD: Food", Subfunction = "Prepared" },
            new S4CategoryTag(){ TypeID = "2236", Description = "Food_QuickMeal", Function = "MOD: Food", Subfunction = "Quick Meal" },
            new S4CategoryTag(){ TypeID = "43025", Description = "Food_SackLunch", Function = "MOD: Food", Subfunction = "Sack Lunch" },
            new S4CategoryTag(){ TypeID = "1686", Description = "Food_Single", Function = "MOD: Food", Subfunction = "Single" },
            new S4CategoryTag(){ TypeID = "651", Description = "Food_Snack", Function = "MOD: Food", Subfunction = "Snack" },
            new S4CategoryTag(){ TypeID = "1603", Description = "Food_Spicy", Function = "MOD: Food", Subfunction = "Spicy" },
            new S4CategoryTag(){ TypeID = "1659", Description = "Food_ToddlerDislike", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1660", Description = "Food_ToddlerLike", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "951", Description = "FullBody_Apron", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "948", Description = "FullBody_Costume", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "374", Description = "FullBody_Jumpsuits", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "950", Description = "FullBody_Lingerie", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "375", Description = "FullBody_Longdress", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "947", Description = "FullBody_Outerwear", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "952", Description = "FullBody_Overall", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "949", Description = "FullBody_Robe", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "376", Description = "FullBody_Shortdress", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "377", Description = "FullBody_Suits", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1237", Description = "FullBody_Swimsuit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67625", Description = "Func_AcidMudPuddle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "688", Description = "Func_ActivityTable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "934", Description = "Func_ActivityTable_Drawing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61496", Description = "Func_ActorCareer_CellDoor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61495", Description = "Func_ActorCareer_Fridge", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61497", Description = "Func_ActorCareer_HospitalExamBed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61647", Description = "Func_ActorCareer_Movie_Medieval_StageProp", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61625", Description = "Func_ActorCareer_Movie_Pirate_StageProp", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61627", Description = "Func_ActorCareer_Movie_SuperHero_StageProp", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61498", Description = "Func_ActorCareer_Pedestal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61499", Description = "Func_ActorCareer_PirateWheel", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61500", Description = "Func_ActorCareer_StageMarkLarge", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61611", Description = "Func_ActorCareer_StageObject_All", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61633", Description = "Func_ActorCareer_StageObject_Campfire", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61641", Description = "Func_ActorCareer_StudioDoor_Private", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61626", Description = "Func_ActorCareer_TVHigh_Apocalypse_StageProp", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1284", Description = "Func_Air", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1392", Description = "Func_Alert", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12397", Description = "Func_Alien", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12436", Description = "Func_Alien_Portal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12370", Description = "Func_Alien_SatelliteDish", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1989", Description = "Func_Ambrosia", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57399", Description = "Func_AmbrosiaTreat", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "506", Description = "Func_Animal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1366", Description = "Func_Anniversary", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55333", Description = "Func_ApartmentProblem", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1195", Description = "Func_Apparition", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1109", Description = "Func_Aquarium", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24605", Description = "Func_Arcade", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45112", Description = "Func_Archaeology_CanBeStudied", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2051", Description = "Func_Archaeology_CanBeStudied_BG", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45073", Description = "Func_ArchaeologyItem_Med", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45074", Description = "Func_ArchaeologyItem_Small", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45072", Description = "Func_ArchaeologyTable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "484", Description = "Func_Art", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2209", Description = "Func_Art_Sculpture", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65548", Description = "Func_ArtsUniversityShell", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65560", Description = "Func_ArtsUniversityShell_Shell1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65561", Description = "Func_ArtsUniversityShell_Shell2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1465", Description = "Func_AshPile", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1131", Description = "Func_Astronaut", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "476", Description = "Func_Athletic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67616", Description = "Func_AtmosphericCondenser", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1394", Description = "Func_Atom", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1119", Description = "Func_Author", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61614", Description = "Func_AutographedObject", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2186", Description = "Func_AutonomyArea_Marker", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57396", Description = "Func_AutoPetFeeder", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1155", Description = "Func_Awning", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "744", Description = "Func_Baby", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2280", Description = "Func_BabyYoda", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1421", Description = "Func_Badge", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "983", Description = "Func_Bait_Crystal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "982", Description = "Func_Bait_Element", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "827", Description = "Func_Bait_FreshFlower", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "825", Description = "Func_Bait_FreshFruit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "788", Description = "Func_Bait_Frog", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "829", Description = "Func_Bait_MedFish", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "984", Description = "Func_Bait_Metal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "789", Description = "Func_Bait_Organic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "40972", Description = "Func_Bait_PlasmaFruit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "828", Description = "Func_Bait_RottenFlower", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "826", Description = "Func_Bait_RottenFruit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "796", Description = "Func_Bait_SmallFish", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "830", Description = "Func_Bait_Trash", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1385", Description = "Func_Bake", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1387", Description = "Func_Baking", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "528", Description = "Func_Ball", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "8218", Description = "Func_Banquet", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "8213", Description = "Func_BanquetTable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "498", Description = "Func_Bar", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1079", Description = "Func_Barbecue", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "36865", Description = "Func_BarGlobe", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12378", Description = "Func_Barrel", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1084", Description = "Func_Baseboard", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1110", Description = "Func_Basin", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "527", Description = "Func_Basket", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55404", Description = "Func_Basketball", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55402", Description = "Func_Basketball_Hoop", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1220", Description = "Func_Bat", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1022", Description = "Func_Bath", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1023", Description = "Func_Bathroom", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "990", Description = "Func_Bathtub", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "32770", Description = "Func_BattleStation", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1078", Description = "Func_BBQ", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63534", Description = "Func_BeachCave", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1427", Description = "Func_Beam", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "508", Description = "Func_Bear", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1216", Description = "Func_Beast", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "777", Description = "Func_Bed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "888", Description = "Func_Bed_Kid", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1542", Description = "Func_Bed_Valid_MonsterUnder_Target", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1009", Description = "Func_BedsideTable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59449", Description = "Func_Beebox", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59452", Description = "Func_BeeSwarm", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "494", Description = "Func_Bench", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "500", Description = "Func_Beverage", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1709", Description = "Func_BG_PipeOrgan", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1710", Description = "Func_BG_YogaMat", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2278", Description = "Func_Bike", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "925", Description = "Func_Bin", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2336", Description = "Func_BioFuel", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "34820", Description = "Func_BirdFeeder", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "995", Description = "Func_Bladder", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1153", Description = "Func_Blinds", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "512", Description = "Func_Blob", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "43029", Description = "Func_BlockConstructionTable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1210", Description = "Func_Bone", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2190", Description = "Func_Bonfire", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1211", Description = "Func_Bony", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "893", Description = "Func_Book", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1177", Description = "Func_Book_BookOfLife", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1080", Description = "Func_Book_Homework", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49153", Description = "Func_Book_MagicTome", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "656", Description = "Func_Book_PlayerCreated", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1389", Description = "Func_Bookcase", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "991", Description = "Func_Boombox", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "26629", Description = "Func_Booth", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "26641", Description = "Func_Booth_Banquette", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "26636", Description = "Func_Booth_Corner", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1160", Description = "Func_Bottle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1222", Description = "Func_Bowl", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "38925", Description = "Func_Bowling", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "38913", Description = "Func_BowlingLane", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1720", Description = "Func_BowlingLane_BG", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "579", Description = "Func_Box", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59408", Description = "Func_BoxOfDecorations", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1882", Description = "Func_Brewer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1086", Description = "Func_Brick", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55407", Description = "Func_Briefcase", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55310", Description = "Func_BubbleBlower", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1291", Description = "Func_Bucket", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "8217", Description = "Func_Buffet", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1163", Description = "Func_Bush", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1323", Description = "Func_Business", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1545", Description = "Func_Business_Light", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1409", Description = "Func_Cabinet", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65550", Description = "Func_CafeteriaStation", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1431", Description = "Func_Cage", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1391", Description = "Func_Cake", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1395", Description = "Func_Calendar", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12342", Description = "Func_Camera_Normal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12343", Description = "Func_Camera_Outstanding", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12341", Description = "Func_Camera_Poor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "79875", Description = "Func_Camera_Pro", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2221", Description = "Func_Camera_Slot_Tripod", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "79873", Description = "Func_Camera_Tripod", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "79877", Description = "Func_Camera_Tripod_Anchor_Mark", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1381", Description = "Func_Cameras", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10246", Description = "Func_Campfire", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10245", Description = "Func_Camping", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1207", Description = "Func_Candle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67628", Description = "Func_CandleMakingStation", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1328", Description = "Func_Candles", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1554", Description = "Func_Candy_Skull", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1555", Description = "Func_Candy_Skull_01", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1556", Description = "Func_Candy_Skull_02", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1557", Description = "Func_Candy_Skull_03", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1558", Description = "Func_Candy_Skull_04", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1559", Description = "Func_Candy_Skull_05", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1560", Description = "Func_Candy_Skull_06", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1561", Description = "Func_Candy_Skull_07", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1562", Description = "Func_Candy_Skull_08", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1563", Description = "Func_Candy_Skull_09", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1564", Description = "Func_Candy_Skull_10", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2117", Description = "Func_CandyBowl", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1297", Description = "Func_Cans", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2276", Description = "Func_CantRepo", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "573", Description = "Func_Canvas", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "922", Description = "Func_CardGame", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1316", Description = "Func_Cards", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "988", Description = "Func_CardTable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "492", Description = "Func_Carpenter", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1161", Description = "Func_Carpet", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1402", Description = "Func_Cart", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22529", Description = "Func_CarvedPumpkin", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22540", Description = "Func_CarvingStation", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1411", Description = "Func_Case", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57383", Description = "Func_CatCondo", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57429", Description = "Func_CatWand", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57453", Description = "Func_CatWand_Rainbow", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49155", Description = "Func_Cauldron", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49156", Description = "Func_Cauldron_Potion", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61475", Description = "Func_CelebrityFanTargetable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61636", Description = "Func_CelebrityTile_Original", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1378", Description = "Func_Cell", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1200", Description = "Func_Cemetery", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1303", Description = "Func_Chair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65592", Description = "Func_Chair_DebateShowdown_Audience", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65593", Description = "Func_Chair_DebateShowdown_Judge", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1426", Description = "Func_Chalkboard", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1448", Description = "Func_ChangeClothes", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1099", Description = "Func_Charisma", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1115", Description = "Func_Chef", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "26627", Description = "Func_ChefStation", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12361", Description = "Func_ChemAnalyzer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12360", Description = "func_ChemLab", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "485", Description = "Func_Chess", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1136", Description = "Func_Child", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1176", Description = "Func_ChildViolin", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1164", Description = "Func_Chimney", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1327", Description = "Func_Christmas", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "511", Description = "Func_Clay", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2114", Description = "Func_ClobbersSnowFootprints", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12344", Description = "Func_CloneNormalMin", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1139", Description = "Func_Closet", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1162", Description = "Func_Clothes", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24593", Description = "Func_Clubs", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12371", Description = "Func_Clue", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1500", Description = "Func_CoatRack", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1193", Description = "Func_Cobweb", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63518", Description = "Func_CoconutPlant", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "525", Description = "Func_Coffee", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65603", Description = "Func_CoffeeCart", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1167", Description = "Func_CoffeeMaker", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "40970", Description = "Func_Coffin", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45075", Description = "Func_CollectArtifact", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45076", Description = "Func_CollectArtifact_Fake", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45092", Description = "Func_CollectArtifact_Genuine", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45098", Description = "Func_CollectArtifact_Knife", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45077", Description = "Func_CollectArtifact_Mail", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45093", Description = "Func_CollectArtifact_Mail_Fake", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45099", Description = "Func_CollectArtifact_Mask", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45100", Description = "Func_CollectArtifact_Skull", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45101", Description = "Func_CollectArtifact_Statue", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45097", Description = "Func_CollectArtifact_Vase", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "32771", Description = "Func_Collection_Monsters", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12425", Description = "Func_Collection_Spawner", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2200", Description = "Func_ColorFromSand", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1130", Description = "Func_Comedy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "589", Description = "Func_ComedyRoutine", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "594", Description = "Func_ComedyRoutine_Long", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "593", Description = "Func_ComedyRoutine_Medium", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "592", Description = "Func_ComedyRoutine_Short", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2284", Description = "Func_CommunityBoard_BG", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "514", Description = "Func_Computer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65655", Description = "Func_ComputerGlasses", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67612", Description = "Func_Concept_EcoInvention", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67611", Description = "Func_Concept_Municipal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1092", Description = "Func_Concrete", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "524", Description = "Func_Cook", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1102", Description = "Func_Cooking", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10243", Description = "Func_Cooler", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1287", Description = "Func_Cot", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "989", Description = "Func_Couch", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1525", Description = "Func_Counter", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1375", Description = "Func_Cowplant", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "513", Description = "Func_Craft", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67622", Description = "Func_CraftedCandle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55365", Description = "Func_CraftSalesTable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2047", Description = "Func_CraftSalesTable_JungleSupplies_Fun", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2046", Description = "Func_CraftSalesTable_JungleSupplies_Furniture", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2048", Description = "Func_CraftSalesTable_JungleSupplies_Pet", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2045", Description = "Func_CraftSalesTable_JungleSupplies_Supplies", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2387", Description = "Func_CraftSalesTable_Painting", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2285", Description = "Func_CraftSalesTable_RequiredObject_BG", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2050", Description = "Func_CraftSalesTable_SecretItems_Collectibles", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2049", Description = "Func_CraftSalesTable_SecretItems_Supplies", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2386", Description = "Func_CraftSalesTable_Table", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12379", Description = "Func_Crate", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1401", Description = "Func_Crates", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57385", Description = "Func_Crates_Routable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24597", Description = "Func_Creativity", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "745", Description = "Func_Crib", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12372", Description = "Func_CrimeMap", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1113", Description = "Func_Criminal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1201", Description = "Func_Crypt", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1184", Description = "Func_CrystalBall", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "502", Description = "Func_Cube", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1114", Description = "Func_Culinary", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1546", Description = "Func_CullingPortal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1302", Description = "Func_Cup", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1012", Description = "Func_Cupboard", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1376", Description = "Func_CupcakeMachine", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1014", Description = "Func_Curtain", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1455", Description = "Func_Dancefloor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24601", Description = "Func_Dancing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24609", Description = "Func_Dartboard", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1565", Description = "Func_DayoftheDead", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "575", Description = "Func_Death", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1151", Description = "Func_Decal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12327", Description = "Func_Dectective", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2157", Description = "Func_DenizenPond", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1386", Description = "Func_Dessert", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12435", Description = "Func_Detective_ChiefChair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12318", Description = "Func_Detective_Clue_AddToMap", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12296", Description = "Func_Detective_Clue_Chemical", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12326", Description = "Func_Detective_Clue_Database", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12312", Description = "Func_Detective_Clue_Picture", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12313", Description = "Func_Detective_Clue_Sample", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67615", Description = "Func_DewCollector", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67646", Description = "Func_DewCollector_HighQuality", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1566", Description = "Func_DiaDeLosMuertos", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2216", Description = "Func_DigitalFrame", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1124", Description = "Func_Dining", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1006", Description = "Func_DiningChair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1125", Description = "Func_DiningHutch", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "509", Description = "Func_Dinosaur", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1415", Description = "Func_Diploma", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61463", Description = "Func_DirectorChair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2100", Description = "Func_DisableInLotThumbnails", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1451", Description = "Func_Dishwasher", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1419", Description = "Func_Dispenser", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1033", Description = "Func_Divider", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1456", Description = "Func_DJBooth", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24600", Description = "Func_DJing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12328", Description = "Func_Doctor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12330", Description = "Func_Doctor_item_Sample", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12329", Description = "Func_Doctor_object_ExamBed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12348", Description = "Func_Doctor_object_MedicalTreadmill", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12333", Description = "Func_Doctor_object_SurgeryTable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12332", Description = "Func_Doctor_object_XrayMachine", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "43030", Description = "Func_DoctorPlayset", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1494", Description = "Func_DoesntSpawnFire", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "580", Description = "Func_Doll", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "666", Description = "Func_Dollhouse", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61462", Description = "Func_DollyCamera", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63492", Description = "Func_Dolphin_Albino", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63493", Description = "Func_Dolphin_Merfolk", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63491", Description = "Func_Dolphin_Standard", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63494", Description = "Func_DolphinSpawner", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24608", Description = "Func_DontWakeLlama", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "778", Description = "Func_DoubleBed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "510", Description = "Func_Dragon", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "43011", Description = "Func_DrawingPosted", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1003", Description = "Func_DrawSomething", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "499", Description = "Func_Drink", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1552", Description = "Func_DrinkTray", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2063", Description = "Func_DropsLeaves_Large", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2062", Description = "Func_DropsLeaves_Medium", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2056", Description = "Func_DropsLeaves_Small", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2064", Description = "Func_DropsLeaves_XLarge", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1428", Description = "Func_Duct", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67605", Description = "Func_Dumpster", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2446", Description = "Func_Dumpster_Deal_Appliance", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2445", Description = "Func_Dumpster_Deal_BurntAndScratched", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2454", Description = "Func_Dumpster_Deal_Collectible", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2455", Description = "Func_Dumpster_Deal_Craftable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2448", Description = "Func_Dumpster_Deal_Miscellaneous", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2447", Description = "Func_Dumpster_Deal_Plumbing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2456", Description = "Func_Dumpster_Deal_UpgradePart", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67610", Description = "Func_Dumpster_HighPriceDrop", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67645", Description = "Func_Dumpster_Insect", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67609", Description = "Func_Dumpster_LowPriceDrop", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2452", Description = "Func_Dumpster_Meal_Food", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2451", Description = "Func_Dumpster_Meal_Ingredient", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2453", Description = "Func_Dumpster_Meal_Insect", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67608", Description = "Func_Dumpster_UniqueDrop", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1725", Description = "Func_EarBuds", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "482", Description = "Func_Easel", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2082", Description = "Func_EasterEgg", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2375", Description = "Func_eco_ecofriendy_appliances", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2374", Description = "Func_eco_green_gardening", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2372", Description = "Func_eco_neighborhood_utility", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2373", Description = "Func_eco_upcycling_initiative", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2376", Description = "Func_EcoFootprint_ObjectState", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67588", Description = "Func_EcoFootprint_SunRay", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "997", Description = "Func_Energy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1129", Description = "Func_Entertainer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12369", Description = "Func_EP01_AlienTransmute_Compatible", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12422", Description = "Func_EP01_Serum_AgeAway", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12421", Description = "Func_EP01_Serum_AlienAura", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12416", Description = "Func_EP01_Serum_Embiggen", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12419", Description = "Func_EP01_Serum_FixersLuck", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12417", Description = "Func_EP01_Serum_GhostGoo", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12354", Description = "Func_EP01_Serum_NeedFixer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12418", Description = "Func_EP01_Serum_OxStrength", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12420", Description = "Func_EP01_Serum_ReapersFriend", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12414", Description = "Func_EP01_Serum_RedHot", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12352", Description = "Func_EP01_Serum_RosePerfume", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12415", Description = "Func_EP01_Serum_Slimify", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12356", Description = "Func_EP01_Serum_Smart", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12353", Description = "Func_EP01_Serum_SnakeOil", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12355", Description = "Func_EP01_Serum_SparkDrive", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12351", Description = "Func_EP01_Serum_SyntheticFood", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2038", Description = "Func_EP1Collectible_BG", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1134", Description = "Func_eSportGamer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1452", Description = "Func_EspressoBar", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1454", Description = "Func_EspressoGrinder", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1453", Description = "Func_EspressoMachine", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1035", Description = "Func_Etagere", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "473", Description = "Func_Excercise", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1416", Description = "Func_Exit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "26631", Description = "Func_ExperimentalFood", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1417", Description = "Func_Extinguisher", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67587", Description = "Func_FabricatedItem", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67590", Description = "Func_FabricationDye", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67637", Description = "Func_FabricationDyeCommon", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67586", Description = "Func_Fabricator", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1214", Description = "Func_Face", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "43016", Description = "Func_FamilyBulletinBoard", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1414", Description = "Func_Fan", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2220", Description = "Func_FashionStudioSearch", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1138", Description = "Func_Faucet", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1575", Description = "Func_Festival_Autonomy_Area_Marker", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55297", Description = "Func_Festival_AutonomyArea_Marker", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55388", Description = "Func_Festival_Blossom_TeaFountain", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55369", Description = "Func_Festival_CurryContest", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55366", Description = "Func_Festival_Fireworks_DarkSide", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55367", Description = "Func_Festival_Fireworks_LightSide", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55392", Description = "Func_Festival_FleaMarketObjects", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55387", Description = "Func_Festival_Lamp_TeaFountains", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55345", Description = "Func_Festival_Tea_DarkTea", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55346", Description = "Func_Festival_Tea_LightTea", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55347", Description = "Func_Festival_Tea_Sakura", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1875", Description = "Func_Fetchable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1157", Description = "Func_Figurine", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1425", Description = "Func_Fileholder", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1305", Description = "Func_Fire", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1165", Description = "Func_FireAlarm", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1306", Description = "Func_FirePit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49183", Description = "Func_Fireplace_Magic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1578", Description = "Func_Fireworks", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1588", Description = "Func_FireworksArtsCrafts", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1583", Description = "Func_FireworksBlossom", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1586", Description = "Func_FireworksFood", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1585", Description = "Func_FireworksLamp", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1584", Description = "Func_FireworksLogic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1587", Description = "Func_FireworksMusic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1590", Description = "Func_FireworksSparkler", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55408", Description = "Func_FireworksSparklerBlossom", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55409", Description = "Func_FireworksSparklerFood", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55410", Description = "Func_FireworksSparklerLamp", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55411", Description = "Func_FireworksSparklerLogic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55412", Description = "Func_FireworksSparklerWedding", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1589", Description = "Func_FireworksWedding", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "992", Description = "Func_Fish", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63503", Description = "Func_Fish_Endangered", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "869", Description = "Func_Fish_Fishbowl", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2195", Description = "Func_Fish_Invasive", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2164", Description = "Func_FishingLocation_Any", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "937", Description = "Func_FishingLocation_Hole", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "938", Description = "Func_FishingLocation_Spot", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63528", Description = "Func_FishingSpot_Bay", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2193", Description = "Func_FishingSpot_Common", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2192", Description = "Func_FishingSpot_Rare", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63526", Description = "Func_FishingSpot_Tropical", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2191", Description = "Func_FishingSpot_Uncommon", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "474", Description = "Func_Fitness", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1403", Description = "Func_Flag", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1094", Description = "Func_Flagstone", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1314", Description = "Func_Flower", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59457", Description = "Func_FlowerArrangement", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59490", Description = "Func_Flowers_10", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59483", Description = "Func_Flowers_3", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59484", Description = "Func_Flowers_4", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59485", Description = "Func_Flowers_5", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59486", Description = "Func_Flowers_6", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59487", Description = "Func_Flowers_7", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59488", Description = "Func_Flowers_8", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59489", Description = "Func_Flowers_9", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2106", Description = "Func_Flowers_BopBeg", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2104", Description = "Func_Flowers_ChrySnap", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2102", Description = "Func_Flowers_DaiBlu", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2107", Description = "Func_Flowers_LilyDeath", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2103", Description = "Func_Flowers_RosDah", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2090", Description = "Func_Flowers_Scent", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2091", Description = "Func_Flowers_ScentRare", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2101", Description = "Func_Flowers_SnoCroc", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2105", Description = "Func_Flowers_TulChri", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1412", Description = "Func_Folders", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1304", Description = "Func_Folding", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "520", Description = "Func_Food", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2030", Description = "Func_Food_PetEdible", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "26643", Description = "Func_FoodPlatter", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24591", Description = "Func_FoosballTable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1180", Description = "Func_Fortune", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "8200", Description = "Func_FortuneTelling", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2044", Description = "Func_Fossil_Brushed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2037", Description = "Func_FossilRock", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "8216", Description = "Func_Fountain", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67599", Description = "Func_FreeLanceMaker_CarvedCandles", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67596", Description = "Func_FreeLanceMaker_Couch", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67601", Description = "Func_FreeLanceMaker_CraftedCandles", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67602", Description = "Func_FreeLanceMaker_FineWallDecor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67598", Description = "Func_FreeLanceMaker_FloorLights", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67595", Description = "Func_FreeLanceMaker_JarCandles", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67600", Description = "Func_FreeLanceMaker_KidsBed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67597", Description = "Func_FreeLanceMaker_Kombucha", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67593", Description = "Func_FreeLanceMaker_Rugs", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67594", Description = "Func_FreeLanceMaker_ToFizz", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2177", Description = "Func_Freelancer_Canvas_Character_Design", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2178", Description = "Func_Freelancer_Canvas_Environment_Design", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2183", Description = "Func_Freelancer_Canvas_Icon", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2184", Description = "Func_Freelancer_Canvas_Illustrative", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2182", Description = "Func_Freelancer_Canvas_Logo", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2179", Description = "Func_Freelancer_Canvas_Portrait", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2181", Description = "Func_Freelancer_Canvas_Recreated_Art", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2185", Description = "Func_Freelancer_Canvas_Reference", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2180", Description = "Func_Freelancer_Canvas_Splash_Art", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1002", Description = "Func_Fridge", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2233", Description = "Func_Fridge_Mini", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12331", Description = "Func_FrontDesk", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1337", Description = "Func_Frosty", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1445", Description = "Func_FruitCake", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "8214", Description = "Func_FruitPunchFountain", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2449", Description = "Func_FryingPan", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "999", Description = "Func_Fun", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "503", Description = "Func_Future", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "481", Description = "Func_Game", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1075", Description = "Func_Gaming", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "924", Description = "Func_Garbage", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1150", Description = "Func_Garden", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59447", Description = "Func_Garden_Flower", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "40971", Description = "Func_Garden_Garlic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2176", Description = "Func_Garden_Ghost_Destroy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "40973", Description = "Func_Garden_PlasmaTree", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1107", Description = "Func_Gardening", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "862", Description = "Func_Gardening_Fertilizer_Bad", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "859", Description = "Func_Gardening_Fertilizer_High", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "861", Description = "Func_Gardening_Fertilizer_Low", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "870", Description = "Func_Gardening_Fertilizer_Max", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "860", Description = "Func_Gardening_Fertilizer_Med", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1708", Description = "Func_Gardening_ForbiddenFruit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2092", Description = "Func_Gardening_Graftable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1502", Description = "Func_Gardening_Growfruit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59482", Description = "Func_Gardening_MoneyTree", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "831", Description = "Func_Gardening_Seed_Common", Function = "MOD: Seed", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "833", Description = "Func_Gardening_Seed_Rare", Function = "MOD: Seed", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "832", Description = "Func_Gardening_Seed_Uncommon", Function = "MOD: Seed", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1029", Description = "Func_Gardening_Seeds", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59437", Description = "Func_Gardening_Sprinkler", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10254", Description = "Func_Gardening_Toxic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1272", Description = "Func_Gardening_Wild", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59463", Description = "Func_GardeningFlowers", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1721", Description = "Func_GardeningSkillPlant", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1334", Description = "Func_Garland", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "40962", Description = "Func_Garlic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1390", Description = "Func_Gate", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1190", Description = "Func_Ghost", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2160", Description = "Func_GiveGift_NotGiftable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2088", Description = "Func_GiveGiftReward", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1432", Description = "Func_Glass", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1365", Description = "Func_Gnome", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2087", Description = "Func_GnomeKickReward", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24587", Description = "Func_GoDancingObject_Visibility", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57395", Description = "Func_GoForWalk_DogInteractions", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1104", Description = "Func_GourmetCooking", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55403", Description = "Func_Graffiti", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2095", Description = "Func_GrandMeal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1093", Description = "Func_Grass", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1198", Description = "Func_Grave", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1203", Description = "Func_Gravestone", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61465", Description = "Func_GreenScreen", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1247", Description = "Func_Grill_Recipe", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "565", Description = "Func_Guitar", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "562", Description = "Func_Gym", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1183", Description = "Func_Gypsy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "77826", Description = "Func_Habitat", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61442", Description = "Func_HairMakeUpChair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57411", Description = "Func_HairPile", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1179", Description = "Func_Halloween", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "75783", Description = "Func_Hamper", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "77828", Description = "Func_Hamster", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1209", Description = "Func_Hand", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1100", Description = "Func_Handiness", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1329", Description = "Func_Hanukkah", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1095", Description = "Func_Hardwood", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2126", Description = "Func_Harvestable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2072", Description = "Func_Harvestable_Rare", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2074", Description = "Func_Harvestable_SuperRare", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2073", Description = "Func_Harvestable_Uncommon", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1223", Description = "Func_Haunted", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1213", Description = "Func_Head", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "475", Description = "Func_Health", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1369", Description = "Func_Heart", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "14338", Description = "Func_HeatLamp", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1520", Description = "Func_HeatLamp_BG", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "77829", Description = "Func_Hedgehog", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10249", Description = "Func_Herbalism", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10251", Description = "Func_HerbalismIngredient", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10271", Description = "Func_HerbalismIngredient_Chamomile", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10272", Description = "Func_HerbalismIngredient_Elderberry", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10273", Description = "Func_HerbalismIngredient_Fireleaf", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10274", Description = "Func_HerbalismIngredient_Huckleberry", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10275", Description = "Func_HerbalismIngredient_MorelMushroom", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10250", Description = "Func_HerbalismPlant", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10255", Description = "Func_HerbalismPotion", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1914", Description = "Func_Hideable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1654", Description = "Func_HighChair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1695", Description = "Func_HighChairDrink", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1694", Description = "Func_HighChairFood", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59411", Description = "Func_HoildayTree_Ornaments", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1326", Description = "Func_Holiday", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2128", Description = "Func_Holiday_Candle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2098", Description = "Func_Holiday_DecoObjects", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2129", Description = "Func_Holiday_FestiveLighting", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59478", Description = "Func_HolidayCandle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2121", Description = "Func_HolidayGnome_Group01", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2122", Description = "Func_HolidayGnome_Group02", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2123", Description = "Func_HolidayGnome_Group03", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2124", Description = "Func_HolidayGnome_Group04", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2116", Description = "Func_HolidayTradition_Baking_Recipe", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2109", Description = "Func_HolidayTradition_Bonfire", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2108", Description = "Func_HolidayTradition_Deco_BeRomantic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2110", Description = "Func_HolidayTradition_HaveDecorations", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2111", Description = "Func_HolidayTradition_OpenPresents", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2112", Description = "Func_HolidayTradition_Party", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59409", Description = "Func_HolidayTree", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59412", Description = "Func_HolidayTree_Garland", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59413", Description = "Func_HolidayTree_Skirt", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59414", Description = "Func_HolidayTree_Topper", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59450", Description = "Func_Honey", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1168", Description = "Func_Hood", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "553", Description = "Func_Hoop", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1377", Description = "Func_Hospital", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "26628", Description = "Func_HostStation", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1300", Description = "Func_HotSauce", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1444", Description = "Func_HotTub", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1224", Description = "Func_House", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2388", Description = "Func_HouseholdInventoryObjectProxy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "996", Description = "Func_Hunger", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1030", Description = "Func_Hutch", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1429", Description = "Func_Hydraulic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "998", Description = "Func_Hygiene", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1249", Description = "Func_IceChest", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "20486", Description = "Func_IceCream", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "20483", Description = "Func_IceCreamBowl", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "20482", Description = "Func_IceCreamCarton", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "20484", Description = "Func_IceCreamCone", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "20481", Description = "Func_IceCreamMachine", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "20485", Description = "Func_IceCreamMilkShake", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2283", Description = "Func_ImportantItems", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18442", Description = "Func_Incense", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47129", Description = "Func_InfectedPlant", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1286", Description = "Func_Inflatable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "523", Description = "Func_Ingredient", Function = "MOD: Ingredient", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12302", Description = "Func_Ingredient_ArtisanHerbBread", Function = "MOD: Ingredient", Subfunction = "ArtisanHerbBread" },
            new S4CategoryTag(){ TypeID = "10253", Description = "Func_Ingredient_Beetle", Function = "MOD: Ingredient", Subfunction = "Beetle" },
            new S4CategoryTag(){ TypeID = "12373", Description = "Func_Ingredient_CowplantEssence", Function = "MOD: Ingredient", Subfunction = "CowplantEssence" },
            new S4CategoryTag(){ TypeID = "10241", Description = "Func_Ingredient_Crawdad", Function = "MOD: Ingredient", Subfunction = "Crawdad" },
            new S4CategoryTag(){ TypeID = "1345", Description = "Func_Ingredient_Crystal", Function = "MOD: Ingredient", Subfunction = "Crystal" },
            new S4CategoryTag(){ TypeID = "12386", Description = "Func_Ingredient_Crystal_Alien", Function = "MOD: Ingredient", Subfunction = "Alien Crystal" },
            new S4CategoryTag(){ TypeID = "1349", Description = "Func_Ingredient_Crystal_Common", Function = "MOD: Ingredient", Subfunction = "Common Crystal" },
            new S4CategoryTag(){ TypeID = "1351", Description = "Func_Ingredient_Crystal_Rare", Function = "MOD: Ingredient", Subfunction = "Rare Crystal" },
            new S4CategoryTag(){ TypeID = "1350", Description = "Func_Ingredient_Crystal_Uncommon", Function = "MOD: Ingredient", Subfunction = "Uncommon Crystal" },
            new S4CategoryTag(){ TypeID = "12305", Description = "Func_Ingredient_ExoticFruitPie", Function = "MOD: Ingredient", Subfunction = "Exotic Fruit Pie" },
            new S4CategoryTag(){ TypeID = "12301", Description = "Func_Ingredient_ExoticFruitTart", Function = "MOD: Ingredient", Subfunction = "Exotic Fruit Tart" },
            new S4CategoryTag(){ TypeID = "817", Description = "Func_Ingredient_Fish", Function = "MOD: Ingredient", Subfunction = "Fish" },
            new S4CategoryTag(){ TypeID = "55335", Description = "Func_Ingredient_Fish_Pufferfish", Function = "MOD: Ingredient", Subfunction = "Fish_Pufferfish" },
            new S4CategoryTag(){ TypeID = "12303", Description = "Func_Ingredient_FishPie", Function = "MOD: Ingredient", Subfunction = "FishPie" },
            new S4CategoryTag(){ TypeID = "67631", Description = "Func_Ingredient_FizzyJuice", Function = "MOD: Ingredient", Subfunction = "FizzyJuice" },
            new S4CategoryTag(){ TypeID = "2429", Description = "Func_Ingredient_FizzyJuice_EP09", Function = "MOD: Ingredient", Subfunction = "FizzyJuice EP09" },
            new S4CategoryTag(){ TypeID = "795", Description = "Func_Ingredient_Fruit", Function = "MOD: Ingredient", Subfunction = "Fruit" },
            new S4CategoryTag(){ TypeID = "12307", Description = "Func_Ingredient_Fruitcake_Set1", Function = "MOD: Ingredient", Subfunction = "Fruitcake Set1" },
            new S4CategoryTag(){ TypeID = "12308", Description = "Func_Ingredient_Fruitcake_Set2", Function = "MOD: Ingredient", Subfunction = "Fruitcake Set2" },
            new S4CategoryTag(){ TypeID = "12298", Description = "Func_Ingredient_FruitMuffins", Function = "MOD: Ingredient", Subfunction = "Fruit Muffins" },
            new S4CategoryTag(){ TypeID = "12299", Description = "Func_Ingredient_FruitScones", Function = "MOD: Ingredient", Subfunction = "Fruit Scones" },
            new S4CategoryTag(){ TypeID = "2432", Description = "Func_Ingredient_Grimbucha_EP09", Function = "MOD: Ingredient", Subfunction = "Grimbucha EP09" },
            new S4CategoryTag(){ TypeID = "816", Description = "Func_Ingredient_Herb", Function = "MOD: Ingredient", Subfunction = "Herb" },
            new S4CategoryTag(){ TypeID = "47142", Description = "Func_Ingredient_InfectedSpore", Function = "MOD: Ingredient", Subfunction = "Infected Spore" },
            new S4CategoryTag(){ TypeID = "1242", Description = "Func_Ingredient_Insect", Function = "MOD: Ingredient", Subfunction = "Insect" },
            new S4CategoryTag(){ TypeID = "12306", Description = "Func_Ingredient_JellyFilledDoughnuts", Function = "MOD: Ingredient", Subfunction = "Jelly Filled Doughnuts" },
            new S4CategoryTag(){ TypeID = "67632", Description = "Func_Ingredient_Kombucha", Function = "MOD: Ingredient", Subfunction = "Kombucha" },
            new S4CategoryTag(){ TypeID = "2430", Description = "Func_Ingredient_Kombucha_EP09", Function = "MOD: Ingredient", Subfunction = "Kombucha EP09" },
            new S4CategoryTag(){ TypeID = "10242", Description = "Func_Ingredient_Locust", Function = "MOD: Ingredient", Subfunction = "Locust" },
            new S4CategoryTag(){ TypeID = "1344", Description = "Func_Ingredient_Metal", Function = "MOD: Ingredient", Subfunction = "Metal" },
            new S4CategoryTag(){ TypeID = "12387", Description = "Func_Ingredient_Metal_Alien", Function = "MOD: Ingredient", Subfunction = "Alien Meta" },
            new S4CategoryTag(){ TypeID = "1346", Description = "Func_Ingredient_Metal_Common", Function = "MOD: Ingredient", Subfunction = "Common Metal" },
            new S4CategoryTag(){ TypeID = "1348", Description = "Func_Ingredient_Metal_Rare", Function = "MOD: Ingredient", Subfunction = "Rare Metal" },
            new S4CategoryTag(){ TypeID = "1347", Description = "Func_Ingredient_Metal_Uncommon", Function = "MOD: Ingredient", Subfunction = "Uncommon Metal" },
            new S4CategoryTag(){ TypeID = "1243", Description = "Func_Ingredient_Mushroom", Function = "MOD: Ingredient", Subfunction = "Mushroom" },
            new S4CategoryTag(){ TypeID = "12388", Description = "Func_Ingredient_Plant_Alien", Function = "MOD: Ingredient", Subfunction = "Alien Plant" },
            new S4CategoryTag(){ TypeID = "12309", Description = "Func_Ingredient_RainbowGelatinCake_Set1", Function = "MOD: Ingredient", Subfunction = "Rainbow Gelatin Cake Set1" },
            new S4CategoryTag(){ TypeID = "12310", Description = "Func_Ingredient_RainbowGelatinCake_Set2", Function = "MOD: Ingredient", Subfunction = "Rainbow Gelatin Cake Set2" },
            new S4CategoryTag(){ TypeID = "12364", Description = "Func_Ingredient_RoseQuartz", Function = "MOD: Ingredient", Subfunction = "Rose Quartz" },
            new S4CategoryTag(){ TypeID = "67633", Description = "Func_Ingredient_Seltzer", Function = "MOD: Ingredient", Subfunction = "Seltzer" },
            new S4CategoryTag(){ TypeID = "12304", Description = "Func_Ingredient_StandardFruitPie", Function = "MOD: Ingredient", Subfunction = "Standard Fruit Pie" },
            new S4CategoryTag(){ TypeID = "12300", Description = "Func_Ingredient_StandardFruitTart", Function = "MOD: Ingredient", Subfunction = "Standard Fruit Tart" },
            new S4CategoryTag(){ TypeID = "67634", Description = "Func_Ingredient_Suspicious", Function = "MOD: Ingredient", Subfunction = "Suspicious" },
            new S4CategoryTag(){ TypeID = "2431", Description = "Func_Ingredient_Suspicious_EP09", Function = "MOD: Ingredient", Subfunction = "Suspicious EP09" },
            new S4CategoryTag(){ TypeID = "815", Description = "Func_Ingredient_Veggie", Function = "MOD: Ingredient", Subfunction = "Veggie" },
            new S4CategoryTag(){ TypeID = "67636", Description = "Func_Ingredient_WaxBlock", Function = "MOD: Ingredient", Subfunction = "Wax Block" },
            new S4CategoryTag(){ TypeID = "1929", Description = "Func_Insane_TalkToObjects", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67592", Description = "Func_InsectFarm", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "570", Description = "Func_Instrument", Function = "Instrument", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1413", Description = "Func_Instruments", Function = "Instrument", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24588", Description = "Func_InteractiveBush", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2070", Description = "Func_InteractiveBush_BG", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24589", Description = "Func_InteractiveCloset", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12394", Description = "Func_InventionConstructor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47165", Description = "Func_Investigation_Dossier", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47147", Description = "Func_Investigation_HazmatSuit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47126", Description = "Func_Investigation_JunkPile", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47164", Description = "Func_Investigation_Keycard", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47136", Description = "Func_Investigation_SealDoor_Floor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47138", Description = "Func_Investigation_SealedDoor_Hallway", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47137", Description = "Func_Investigation_SealedDoor_MotherPlant", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47146", Description = "Func_Investigation_SporeFilter", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47135", Description = "Func_Investigation_SporeSample", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47106", Description = "Func_InvestigationEvidence", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1219", Description = "Func_Invisible", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63501", Description = "Func_IslandCanoe", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2198", Description = "Func_IslandCanoe_BeachVenue", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63497", Description = "Func_IslandSpirit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63498", Description = "Func_IslandSpirit_Inactive", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1206", Description = "Func_Jacko'lantern", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1379", Description = "Func_Jail", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1342", Description = "Func_Jig", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "43009", Description = "Func_Journal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1724", Description = "Func_Journal_BaseGame", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1118", Description = "Func_Journalist", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67629", Description = "Func_JuiceFizzer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67635", Description = "Func_JuiceFizzingProduct", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65539", Description = "Func_JuiceKeg", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65543", Description = "Func_JuiceKeg_Confident", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65542", Description = "Func_JuiceKeg_Flirty", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65544", Description = "Func_JuiceKeg_Happy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65545", Description = "Func_JuiceKeg_Playful", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24604", Description = "Func_JumpStand", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "563", Description = "Func_Jungle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1034", Description = "Func_JungleGym", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1581", Description = "Func_KaraokeMachine", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1282", Description = "Func_Kerosene", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1298", Description = "Func_Ketchup", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1221", Description = "Func_Kettle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1091", Description = "Func_Kid", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59462", Description = "Func_KiddiePool", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1140", Description = "Func_Knife", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "83992", Description = "Func_Knitting", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "83983", Description = "Func_Knitting_BabyOnesie", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "83973", Description = "Func_Knitting_Beanie", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "83988", Description = "Func_Knitting_ChildSweater", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "83993", Description = "Func_Knitting_Clothing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "83979", Description = "Func_Knitting_Decoration", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "83975", Description = "Func_Knitting_Furnishing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "83990", Description = "Func_Knitting_Gifted", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "83987", Description = "Func_Knitting_Grim", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "83980", Description = "Func_Knitting_Onesie", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "83978", Description = "Func_Knitting_Pouffe", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "83976", Description = "Func_Knitting_Rug", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "83974", Description = "Func_Knitting_Socks", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "83977", Description = "Func_Knitting_Sweater", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "83981", Description = "Func_Knitting_SweaterScarf", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "83982", Description = "Func_Knitting_Toy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2463", Description = "Func_Knitting_WIP", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1141", Description = "Func_Knives", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24595", Description = "Func_Knowledge", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1330", Description = "Func_Kwanzaa", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1400", Description = "Func_Lab", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47105", Description = "Func_LabDoor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1230", Description = "Func_Ladder", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1283", Description = "Func_Lamp", Function = "Lights", Subfunction = "Lamp" },
            new S4CategoryTag(){ TypeID = "1293", Description = "Func_LampPost", Function = "Lights", Subfunction = "Lamp Post" },
            new S4CategoryTag(){ TypeID = "67607", Description = "Func_Landfill_DumpableAppliance", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1205", Description = "Func_Lantern", Function = "Lights", Subfunction = "Lantern" },
            new S4CategoryTag(){ TypeID = "515", Description = "Func_Laptop", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1396", Description = "Func_Laser", Function = "Lights", Subfunction = "Laser" },
            new S4CategoryTag(){ TypeID = "24577", Description = "Func_LaserLight", Function = "Lights", Subfunction = "Laser" },
            new S4CategoryTag(){ TypeID = "75781", Description = "Func_Laundry_ClothesLine", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "75779", Description = "Func_Laundry_Dryer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2033", Description = "Func_Laundry_Hamper", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2032", Description = "Func_Laundry_Hero_Object", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "75777", Description = "Func_Laundry_Pile", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "75782", Description = "Func_Laundry_SearchTerm", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "75778", Description = "Func_Laundry_WashingMachine", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "75780", Description = "Func_Laundry_WashTub", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63499", Description = "Func_LavaRock", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59432", Description = "Func_LeafPile", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55405", Description = "Func_Lectern", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1446", Description = "Func_Light_CandleWithAutoLights", Function = "Lights", Subfunction = "Candle" },
            new S4CategoryTag(){ TypeID = "1325", Description = "Func_Light_NoAuto_Lights", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1374", Description = "Func_Light_NonElectric", Function = "Lights", Subfunction = "Non-Electric" },
            new S4CategoryTag(){ TypeID = "61467", Description = "Func_Lighting_NotStageLights", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2076", Description = "Func_Lightning_CanStrike", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59491", Description = "Func_Lightning_Cleanup", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59440", Description = "Func_Lightning_Object", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1338", Description = "Func_Lights", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1097", Description = "Func_Linoleum", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47145", Description = "Func_ListeningDevice_Bug", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57355", Description = "Func_LitterBox", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57360", Description = "Func_LitterBox_HighTech", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1722", Description = "Func_LiveDragAllowedWithChildren", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1005", Description = "Func_LivingChair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2187", Description = "Func_Locator_BeachPortal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1307", Description = "Func_Log", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1098", Description = "Func_Logic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1288", Description = "Func_Lotus", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61632", Description = "Func_LoungeEvent_AwardTrophy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "578", Description = "Func_Machine", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1405", Description = "Func_Magazine", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49169", Description = "Func_Magic_Broom", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1701", Description = "Func_MagicBean", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1702", Description = "Func_MagicBean_AngryRed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1707", Description = "Func_MagicBean_ConfidentLightBlue", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1705", Description = "Func_MagicBean_FlirtyPink", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1703", Description = "Func_MagicBean_PlayfulGreen", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1706", Description = "Func_MagicBean_SadNavyBlue", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1704", Description = "Func_MagicBean_UncomfortableOrange", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49159", Description = "Func_MagicPortal_DuelingtoHQ", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49160", Description = "Func_MagicPortal_HQtoDueling", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49161", Description = "Func_MagicPortal_HQtoMarket", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49162", Description = "Func_MagicPortal_HQtoVista", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49163", Description = "Func_MagicPortal_MarkettoHQ", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49164", Description = "Func_MagicPortal_VistatoHQ", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63509", Description = "Func_MahiMahi", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "954", Description = "Func_Mailbox", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2168", Description = "Func_MailboxWall", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "36868", Description = "Func_MakeupTable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1322", Description = "Func_Mannequin", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1225", Description = "Func_Mansion", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1312", Description = "Func_Map", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55298", Description = "Func_MarketStall", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1932", Description = "Func_MarketStalls", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57410", Description = "Func_MarketStalls_Dockyard_Pets", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2378", Description = "Func_MarketStalls_PurchaseFood", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2379", Description = "Func_Marketstalls_PurchaseNonFood", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1936", Description = "Func_MarketStalls_Seafood", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59404", Description = "Func_MarketStalls_Seasonal_Fall", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59403", Description = "Func_MarketStalls_Seasonal_Spring", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59402", Description = "Func_MarketStalls_Seasonal_Summer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59405", Description = "Func_MarketStalls_Seasonal_Winter", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57405", Description = "Func_MarketStalls_SquareSnacks", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57406", Description = "Func_MarketStalls_SquareSnacks_Pets", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1295", Description = "Func_Mascot", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1096", Description = "Func_Masonry", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18454", Description = "Func_Massage", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18440", Description = "Func_MassageChair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18434", Description = "Func_MassageTable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1285", Description = "Func_Mattress", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "521", Description = "Func_Meal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67648", Description = "Func_Meatwall", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65639", Description = "Func_MechSuit_Body", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65640", Description = "Func_MechSuit_Head", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18452", Description = "Func_Meditation", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18438", Description = "Func_MeditationStool", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1188", Description = "Func_Medium", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55406", Description = "Func_Megaphone", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24599", Description = "Func_Mental", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "43031", Description = "Func_Mess", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1090", Description = "Func_Metal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "488", Description = "Func_Microphone", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "857", Description = "Func_Microscope", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "526", Description = "Func_Microwave", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47143", Description = "Func_MilitaryCareer_Medal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65582", Description = "Func_MiniBots", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65641", Description = "Func_Minibots_Party", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2275", Description = "Func_Minibots_Worker", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2165", Description = "Func_Mirror_NoVanity", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1116", Description = "Func_Mixologist", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1103", Description = "Func_Mixology", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1158", Description = "Func_Model", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "564", Description = "Func_Monkey", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1001", Description = "Func_MonkeyBars", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1217", Description = "Func_Monster", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47131", Description = "Func_MotherPlant", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47144", Description = "Func_Motherplant_Pit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "480", Description = "Func_Motion", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1016", Description = "Func_MotionGamingRig", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24598", Description = "Func_Motor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1498", Description = "Func_Movie", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18456", Description = "Func_Mudbath", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59406", Description = "Func_MudPuddle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1301", Description = "Func_Mug", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55371", Description = "Func_Mural", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "491", Description = "Func_Music", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61470", Description = "Func_MusicDisc", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1083", Description = "Func_Musician", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61469", Description = "Func_MusicProductionStation", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1299", Description = "Func_Mustard", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45067", Description = "Func_MysticalRelic_Bottom", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45068", Description = "Func_MysticalRelic_Crystal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45078", Description = "Func_MysticalRelic_Fused", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45066", Description = "Func_MysticalRelic_Top", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45111", Description = "Func_MysticalRelic_Unbreakable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1527", Description = "Func_Nectar", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12400", Description = "Func_Neon", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1662", Description = "Func_NestingBlocks", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2069", Description = "Func_NeverReceivesSnow", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2210", Description = "Func_NoCleanUpFromInventory", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2144", Description = "Func_NonBarJuiceEnthusiastQuirk", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "780", Description = "Func_Object_Upgrade_Part", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "572", Description = "Func_Observatory", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2219", Description = "Func_OffTheGrid", Function = "Off The Grid", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2427", Description = "Func_OffTheGrid_Toggle_UtilityUsage", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1185", Description = "Func_Oracle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12429", Description = "Func_Orrery", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1007", Description = "Func_Ottoman", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1430", Description = "Func_Outdoor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1004", Description = "Func_OutdoorChair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1013", Description = "Func_OutdoorPlant", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1280", Description = "Func_Outdoors", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "748", Description = "Func_Oven", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "483", Description = "Func_Paint", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1120", Description = "Func_Painter", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "894", Description = "Func_Painting", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1296", Description = "Func_Pans", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1418", Description = "Func_Paper", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "43010", Description = "Func_PaperPosted", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "30721", Description = "Func_ParkFountain", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "529", Description = "Func_Party", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45060", Description = "Func_PathObstacleJungle_01_entrance", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45061", Description = "Func_PathObstacleJungle_01_exit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45062", Description = "Func_PathObstacleJungle_02_entrance", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45063", Description = "Func_PathObstacleJungle_02_exit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45080", Description = "Func_PathObstacleJungle_03_entrance", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45081", Description = "Func_PathObstacleJungle_03_exit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45082", Description = "Func_PathObstacleJungle_04_entrance", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45083", Description = "Func_PathObstacleJungle_04_exit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45084", Description = "Func_PathObstacleJungle_05_entrance", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45085", Description = "Func_PathObstacleJungle_05_exit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45086", Description = "Func_PathObstacleJungle_06_entrance", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45087", Description = "Func_PathObstacleJungle_06_exit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45095", Description = "Func_PathObstacleJungle_Pool_entrance", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45096", Description = "Func_PathObstacleJungle_Pool_exit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45064", Description = "Func_PathObstacleJungle_temple_entrance", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45065", Description = "Func_PathObstacleJungle_temple_exit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1011", Description = "Func_PatioFurniture", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1399", Description = "Func_Pedestal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55299", Description = "Func_PerformanceSpace", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57445", Description = "Func_Pet_Bush", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57448", Description = "Func_Pet_DirtMound", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57454", Description = "Func_Pet_DogToy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57446", Description = "Func_Pet_Fishpile", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57444", Description = "Func_Pet_Gift", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2031", Description = "Func_Pet_HideNoFade", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "77825", Description = "Func_Pet_Minor_Cage", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2052", Description = "Func_Pet_Minor_Cage_BG", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2028", Description = "Func_Pet_NoRouteUnder", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57361", Description = "Func_Pet_Poop", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57455", Description = "Func_Pet_Poop_NoClean", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57447", Description = "Func_Pet_Seaweed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57412", Description = "Func_PetBall", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57386", Description = "Func_PetBed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1876", Description = "Func_PetBowl", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57421", Description = "Func_PetCatnip", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57388", Description = "Func_PetCrate", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2171", Description = "Func_PetFearSounds_BG", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57380", Description = "Func_PetFiller", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57382", Description = "Func_PetFillerThree", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57381", Description = "Func_PetFillerTwo", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57379", Description = "Func_PetFood", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57415", Description = "Func_PetObstacleCourse", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57416", Description = "Func_PetObstacleCourse_Hoop", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57418", Description = "Func_PetObstacleCourse_Platform", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57417", Description = "Func_PetObstacleCourse_Ramp", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57420", Description = "Func_PetObstacleCourse_Tunnel", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57419", Description = "Func_PetObstacleCourse_WeavingFlags", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57404", Description = "Func_PetRecipe", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1930", Description = "Func_PetRecipe_Food", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1878", Description = "Func_PetScratchableFurniture", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57413", Description = "Func_PetSqueakyBall", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1877", Description = "Func_PetToy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57440", Description = "Func_PetToy_New", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57456", Description = "Func_PetToy_SmartTraitCarry", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57376", Description = "Func_PetToyBox", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57426", Description = "Func_PetTreat", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57431", Description = "Func_PetTreat_Edible", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57438", Description = "Func_PetTreat_Edible_Child", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57439", Description = "Func_PetTreat_Edible_Elder", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1194", Description = "Func_Phantom", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1382", Description = "Func_Photo", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "79874", Description = "Func_Photo_Collage", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1383", Description = "Func_Photography", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1438", Description = "Func_PhotographyDissallow", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1941", Description = "Func_PhotoStudio", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2218", Description = "Func_PhotoStudioSearch", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "566", Description = "Func_Piano", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1317", Description = "Func_Picnic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1248", Description = "Func_PicnicTable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1010", Description = "Func_Pillar", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1410", Description = "Func_Pipe", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "40963", Description = "Func_PipeOrgan", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "501", Description = "Func_Pirate", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63527", Description = "Func_PitBBQ", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "26639", Description = "Func_PlacematDrawing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1711", Description = "Func_PlacematFormal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1149", Description = "Func_PlanterBox", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1420", Description = "Func_Plaque", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1142", Description = "Func_Play", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1021", Description = "Func_Plush", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1577", Description = "Func_Podium", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65591", Description = "Func_PodiumPair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65594", Description = "Func_PodiumPair_DebateShowdown", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1404", Description = "Func_Pole", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12398", Description = "Func_Police", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1233", Description = "Func_Pool", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1240", Description = "Func_PoolLadder", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1234", Description = "Func_PoolLight", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "28674", Description = "Func_Popcorn", Function = "MOD: Food", Subfunction = "Popcorn" },
            new S4CategoryTag(){ TypeID = "28678", Description = "Func_Popcorn_Buttered", Function = "MOD: Food", Subfunction = "Buttered Popcorn" },
            new S4CategoryTag(){ TypeID = "28676", Description = "Func_Popcorn_Caramel", Function = "MOD: Food", Subfunction = "Caramel Popcorn" },
            new S4CategoryTag(){ TypeID = "28675", Description = "Func_Popcorn_Chedder", Function = "MOD: Food", Subfunction = "Chedder Popcorn" },
            new S4CategoryTag(){ TypeID = "28677", Description = "Func_Popcorn_Kettle", Function = "MOD: Food", Subfunction = "Kettle Popcorn" },
            new S4CategoryTag(){ TypeID = "28673", Description = "Func_PopcornPopper", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24602", Description = "Func_PortableBar", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1627", Description = "Func_PortableKeyboard", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1435", Description = "Func_Portal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1422", Description = "Func_Portrait", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "895", Description = "Func_Poster", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1144", Description = "Func_Pot", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "993", Description = "Func_Potion", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1664", Description = "Func_Potty", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67617", Description = "Func_PowerGenerator", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2170", Description = "Func_PregenerateDefaultMatGeoStateThumbnailOnly", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59410", Description = "Func_PresentPile", Function = "Present Pile", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59416", Description = "Func_PresentPile_Large", Function = "Present Pile", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59415", Description = "Func_PresentPile_Small", Function = "Present Pile", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2442", Description = "Func_Prevent_Recycling", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1380", Description = "Func_Prison", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61640", Description = "Func_Privacy_ObeyAppropriate", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1101", Description = "Func_Programming", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1281", Description = "Func_Propane", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1186", Description = "Func_Psychic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1340", Description = "Func_PublicBathroom", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "567", Description = "Func_Puddle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1204", Description = "Func_Pumpkin", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "477", Description = "Func_Punching", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "32769", Description = "Func_PuppetTheater", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63506", Description = "Func_Purchase_Beach", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63532", Description = "Func_Purchase_BeachAccessories", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63529", Description = "Func_Purchase_BeachFishing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63533", Description = "Func_Purchase_BeachFruits", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63530", Description = "Func_Purchase_BeachLeisure", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63531", Description = "Func_Purchase_BeachVehicles", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1037", Description = "Func_PurchasePicker_Category_Book_Emotional", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1036", Description = "Func_PurchasePicker_Category_Book_Skill", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65624", Description = "Func_Quadcopter", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1146", Description = "Func_Rack", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1169", Description = "Func_Range", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1341", Description = "Func_RangerStation", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1889", Description = "Func_RangerStation_Catagory_Fun", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1888", Description = "Func_RangerStation_Catagory_Furniture", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1890", Description = "Func_RangerStation_Catagory_Ingredient", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1887", Description = "Func_RangerStation_Catagory_Supplies", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2012", Description = "Func_RangerStation_Category_Pet", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10269", Description = "Func_RangerStation_Fun", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10268", Description = "Func_RangerStation_Furniture", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10270", Description = "Func_RangerStation_Ingredient", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10267", Description = "Func_RangerStation_Supplies", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "77830", Description = "Func_Rat", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "576", Description = "Func_Reaper", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2205", Description = "Func_RebatePlant", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "522", Description = "Func_Recipe", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12377", Description = "Func_Recipe_Baking_CupcakeFactory", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12376", Description = "Func_Recipe_Baking_Oven", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1111", Description = "Func_Recliner", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47149", Description = "Func_Recording", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67585", Description = "Func_Recycler", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "519", Description = "Func_Refrigerator", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2271", Description = "Func_RegisteredVampireLair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12395", Description = "Func_Registers", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18457", Description = "Func_Relaxation", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67644", Description = "Func_RepairBurnt", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67643", Description = "Func_RepairBurnt_VariableHeight", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2435", Description = "Func_RepairBurnt_VariableHeight_BG", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63519", Description = "Func_RequiresOceanLot", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65631", Description = "Func_ResearchMachine", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1321", Description = "Func_Retail", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12365", Description = "Func_Retail_NeonLight", Function = "Lights", Subfunction = "Neon" },
            new S4CategoryTag(){ TypeID = "12384", Description = "Func_Retail_NPC_ItemForSale", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12431", Description = "Func_RetailFridge", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12383", Description = "Func_RetailPedestal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12311", Description = "Func_RetailRegister", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61557", Description = "Func_ReviewProduct_Beauty", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61559", Description = "Func_ReviewProduct_Gadget", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61558", Description = "Func_ReviewProduct_Tech", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1121", Description = "Func_Reward", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1015", Description = "Func_Rig", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2281", Description = "Func_RoboticArm", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65565", Description = "Func_RoboticsTable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1982", Description = "Func_RobotVacuum", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57422", Description = "Func_RobotVacuum_CleanDefault", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57423", Description = "Func_RobotVacuum_CleanUpgrade", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2010", Description = "Func_RobotVacuum_Mess_DefaultClean", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2011", Description = "Func_RobotVacuum_Mess_UpgradedClean", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1983", Description = "Func_RobotVacuumBase", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1318", Description = "Func_Rock", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "71681", Description = "Func_RockClimbingWall", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "495", Description = "Func_Rocket", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1105", Description = "Func_RocketScience", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2462", Description = "Func_RockingChair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "83986", Description = "Func_RockingChair_ArmChair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "71682", Description = "Func_RockWall", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2263", Description = "Func_Roommate_Absent", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2259", Description = "Func_Roommate_Art", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2257", Description = "Func_Roommate_Baking", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2262", Description = "Func_Roommate_BathroomHog", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2264", Description = "Func_Roommate_BigCloset", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2256", Description = "Func_Roommate_Breaker", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2255", Description = "Func_Roommate_CantStopTheBeat", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2248", Description = "Func_Roommate_Cheerleader", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2251", Description = "Func_Roommate_ClingySocialite", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2260", Description = "Func_Roommate_Computer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2252", Description = "Func_Roommate_CouchPotato", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2254", Description = "Func_Roommate_EmoLoner", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2261", Description = "Func_Roommate_Fitness", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2250", Description = "Func_Roommate_Fixer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2267", Description = "Func_Roommate_LateOnRent", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2249", Description = "Func_Roommate_Mealmaker", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2258", Description = "Func_Roommate_Music", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2253", Description = "Func_Roommate_PartyPlanner", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2266", Description = "Func_Roommate_Prankster", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2265", Description = "Func_Roommate_PublicAffectionDisplayer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2247", Description = "Func_Roommate_Superneat", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "923", Description = "Func_Rubbish", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2020", Description = "Func_Rug", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1320", Description = "Func_Rustic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1406", Description = "Func_Sale", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1407", Description = "Func_Saline", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2450", Description = "Func_SaucePot", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1393", Description = "Func_Saucer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18455", Description = "Func_Sauna", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1408", Description = "Func_Sawhorse", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59459", Description = "Func_Scarecrow", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59470", Description = "Func_ScentFlower_HighSkill", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59469", Description = "Func_ScentFlower_LowSkill", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "43013", Description = "Func_SchoolProjectBox_Child", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "43014", Description = "Func_SchoolProjectBox_Teen", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1019", Description = "Func_Science", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "858", Description = "Func_ScienceTable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65549", Description = "Func_ScienceUniversityShell", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65562", Description = "Func_ScienceUniversityShell_Shell1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65563", Description = "Func_ScienceUniversityShell_Shell2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12396", Description = "Func_Scientist", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57369", Description = "Func_Scratched_All", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57368", Description = "Func_Scratched_Low", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57384", Description = "Func_ScratchingPost", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1031", Description = "Func_Screen", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "574", Description = "Func_Scythe", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1189", Description = "Func_Seance", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63508", Description = "Func_Seashell", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59445", Description = "Func_Seasonal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1127", Description = "Func_SecretAgent", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65569", Description = "Func_SecretSociety_Garden", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1187", Description = "Func_Seer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1154", Description = "Func_Shades", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1290", Description = "Func_Sheet", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1148", Description = "Func_Shelf", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2245", Description = "Func_ShellInteractive", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "496", Description = "Func_Ship", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1315", Description = "Func_Shower", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1663", Description = "Func_ShowerTub", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1397", Description = "Func_Shuttle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1027", Description = "Func_Sign", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1278", Description = "Func_SimRay", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12433", Description = "Func_SimRay_NotValid_TransformResult", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1528", Description = "Func_SimRay_NotValid_TransformResult_BG", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12362", Description = "Func_SimRay_Transform_AlienVisitor_Allow", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "489", Description = "Func_Sing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "779", Description = "Func_SingleBed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1313", Description = "Func_Sink", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1434", Description = "Func_Sit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2196", Description = "Func_SitLounge", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2202", Description = "Func_SitLoungeFloat", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59407", Description = "Func_SkatingRink", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59399", Description = "Func_SkatingRink_IceNatural", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59398", Description = "Func_SkatingRink_IceRink", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59436", Description = "Func_SkatingRink_Large", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59400", Description = "Func_SkatingRink_RollerRink", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59448", Description = "Func_SkatingRink_Seasonal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59435", Description = "Func_SkatingRink_Small", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1208", Description = "Func_Skeleton", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2159", Description = "Func_Sketchpad", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1384", Description = "Func_Skills", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1212", Description = "Func_Skull", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1143", Description = "Func_Sleep", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61624", Description = "Func_SleepingPod", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "34818", Description = "Func_SlideLawn", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "34817", Description = "Func_SlipplySlide", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2174", Description = "Func_SmartHub", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2133", Description = "Func_SnobArtAssess", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2134", Description = "Func_SnobArtAssess_NoReserve", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1333", Description = "Func_Snow", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59468", Description = "Func_Snowangel", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59438", Description = "Func_Snowdrift", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1336", Description = "Func_Snowman", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59467", Description = "Func_Snowpal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1423", Description = "Func_Soap", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65540", Description = "Func_SoccerBall", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1000", Description = "Func_Social", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67613", Description = "Func_SolarPanel", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "517", Description = "Func_Sound", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "497", Description = "Func_Space", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1132", Description = "Func_SpaceRanger", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "994", Description = "Func_Spaceship", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59453", Description = "Func_Spawner", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49165", Description = "Func_Spells_Duplicate", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2268", Description = "Func_Spells_Duplicate_BG", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49167", Description = "Func_Spells_Steal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2436", Description = "Func_Spells_Steal_BG", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1192", Description = "Func_Spider", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1197", Description = "Func_Spirit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1196", Description = "Func_Spook", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1178", Description = "Func_Spooky", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65607", Description = "Func_SportsArena_Arts", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65608", Description = "Func_SportsArena_Science", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1061", Description = "Func_Sprinkler", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2099", Description = "Func_Sprinkler_Floor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1182", Description = "Func_SPTable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61464", Description = "Func_StageGate_Actors", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61461", Description = "Func_StageLight_All", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61623", Description = "Func_StageMarkDuo_SwordFight", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61443", Description = "Func_StageMarkDuo1_1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61444", Description = "Func_StageMarkDuo1_2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61445", Description = "Func_StageMarkDuo1_3", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61481", Description = "Func_StageMarkDuo2_1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61482", Description = "Func_StageMarkDuo2_2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61483", Description = "Func_StageMarkDuo2_3", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61484", Description = "Func_StageMarkDuo3_1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61485", Description = "Func_StageMarkDuo3_2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61486", Description = "Func_StageMarkDuo3_3", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61622", Description = "Func_StageMarkSolo_Death", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61446", Description = "Func_StageMarkSolo1_1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61447", Description = "Func_StageMarkSolo1_2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61448", Description = "Func_StageMarkSolo1_3", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61487", Description = "Func_StageMarkSolo2_1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61488", Description = "Func_StageMarkSolo2_2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61489", Description = "Func_StageMarkSolo2_3", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61490", Description = "Func_StageMarkSolo3_1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61491", Description = "Func_StageMarkSolo3_2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61492", Description = "Func_StageMarkSolo3_3", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47107", Description = "Func_Stalls_CurioShop_Objects", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55342", Description = "Func_Stalls_Produce_CurryChili", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55341", Description = "Func_Stalls_Produce_Grocery", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55343", Description = "Func_Stalls_Produce_Saffron", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55344", Description = "Func_Stalls_Produce_Wasabi", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55349", Description = "Func_Stalls_Schwag_Festival_AllStalls", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55336", Description = "Func_Stalls_Schwag_Festival_Blossom", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55337", Description = "Func_Stalls_Schwag_Festival_FleaMarket", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55338", Description = "Func_Stalls_Schwag_Festival_Food", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55339", Description = "Func_Stalls_Schwag_Festival_Lamp", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55340", Description = "Func_Stalls_Schwag_Festival_Logic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1166", Description = "Func_Stand", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1137", Description = "Func_StandingLamp", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1156", Description = "Func_Statue", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24585", Description = "Func_SteamFissure", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18444", Description = "Func_SteamRoom", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63500", Description = "Func_SteamVent", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1077", Description = "Func_Steps", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "516", Description = "Func_Stereo", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2135", Description = "Func_Stereo_Public", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1152", Description = "Func_Sticker", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1089", Description = "Func_Stone", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1008", Description = "Func_Stool", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12424", Description = "Func_Store", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "487", Description = "Func_Strategy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1388", Description = "Func_Striped", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "504", Description = "Func_Stuffed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1020", Description = "Func_StuffedAnimal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1308", Description = "Func_Stump", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2130", Description = "Func_Styleboard", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1567", Description = "Func_SugarSkull", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1398", Description = "Func_Sunflower", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1294", Description = "Func_Supplies", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1231", Description = "Func_Swim", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1232", Description = "Func_SwimmingPool", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2125", Description = "Func_SwingSet", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2120", Description = "Func_SwingSetBG", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2136", Description = "Func_Swipe_Basic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2138", Description = "Func_Swipe_HighSkill", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2137", Description = "Func_Swipe_MedSkill", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2139", Description = "Func_SwipeHouseholdInventory_Basic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2141", Description = "Func_SwipeHouseholdInventory_HighSkill", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2140", Description = "Func_SwipeHouseholdInventory_MedSkill", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12430", Description = "Func_Syrums", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "518", Description = "Func_System", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "486", Description = "Func_Table", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1335", Description = "Func_TableCloth", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1538", Description = "Func_TableDiningBar", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1547", Description = "Func_TableDiningUmbrella", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1108", Description = "Func_Tablet", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1433", Description = "Func_Tank", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1289", Description = "Func_Tarp", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "577", Description = "Func_Tea", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1133", Description = "Func_TechGuru", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1073", Description = "Func_Teddy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1074", Description = "Func_TeddyBear", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "571", Description = "Func_Telescope", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "470", Description = "Func_Television", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1181", Description = "Func_Teller", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55370", Description = "Func_Temp_CraftSalesTable_CreatedObjects", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2040", Description = "Func_Temp_CraftSalesTable_CreatedObjects_BG", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2060", Description = "Func_Temperature_Cooler", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2059", Description = "Func_Temperature_Heater", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45091", Description = "Func_Temple_Chest", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45057", Description = "Func_Temple_Gate", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45058", Description = "Func_Temple_Trap", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10252", Description = "Func_Tent", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65649", Description = "Func_TermPresentation_ClassA", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65650", Description = "Func_TermPresentation_ClassB", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65651", Description = "Func_TermPresentation_ClassC", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65652", Description = "Func_TermPresentation_ClassD", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1087", Description = "Func_Terracotta", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "77827", Description = "Func_Terrarium", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59456", Description = "Func_Thermostat", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59428", Description = "Func_Throwing_Mud", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59429", Description = "Func_Throwing_Snowballs", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59427", Description = "Func_Throwing_WaterBalloons", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1088", Description = "Func_Tile", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1685", Description = "Func_Toddler", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1658", Description = "Func_Toddler_Bed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "73731", Description = "Func_Toddler_GymObject_BallPit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1727", Description = "Func_Toddler_GymObject_Full", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1726", Description = "Func_Toddler_GymObject_Slide", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "73730", Description = "Func_Toddler_GymObject_Slide_Climber", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "73732", Description = "Func_Toddler_GymObject_Tunnels", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "73729", Description = "Func_Toddler_JungleGymObject", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "73734", Description = "Func_ToddlerBallPit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1666", Description = "Func_ToddlerBookcase", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1676", Description = "Func_ToddlerSeating", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "73733", Description = "Func_ToddlerSlide", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1665", Description = "Func_ToddlerToybox", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1881", Description = "Func_Toilet", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55311", Description = "Func_Toilet_Talking", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1202", Description = "Func_Tomb", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1199", Description = "Func_Tombstone", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1147", Description = "Func_Towel", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "505", Description = "Func_Toy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1018", Description = "Func_Toybox", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "533", Description = "Func_ToyBox_ToysToCleanUp", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1646", Description = "Func_ToyboxPurchase", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65625", Description = "Func_ToyRobot", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "581", Description = "Func_Trash", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "891", Description = "Func_TrashCan", Function = "Trash Can", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2443", Description = "Func_Trashcan_HiTech", Function = "Trash Can", Subfunction = "Hi Tech" },
            new S4CategoryTag(){ TypeID = "2349", Description = "Func_TrashCan_Indoor", Function = "Trash Can", Subfunction = "Indoor" },
            new S4CategoryTag(){ TypeID = "892", Description = "Func_TrashCan_Outdoor", Function = "Trash Can", Subfunction = "Outdoor" },
            new S4CategoryTag(){ TypeID = "568", Description = "Func_TrashPile", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2335", Description = "Func_TrashPile_Compostable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2334", Description = "Func_TrashPile_Recyclable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "478", Description = "Func_Treadmill", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63510", Description = "Func_Treasure", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45113", Description = "Func_Treasure_Chest", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1332", Description = "Func_Tree", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61638", Description = "Func_Trend_Celebrity", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61547", Description = "Func_Trend_ProductReview_Beauty", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61548", Description = "Func_Trend_ProductReview_Tech", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61549", Description = "Func_Trend_ProductReview_Toy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61635", Description = "Func_Trend_Skill_Acting", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61501", Description = "Func_Trend_Skill_Archaeology", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61502", Description = "Func_Trend_Skill_Baking", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61503", Description = "Func_Trend_Skill_Bowling", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61504", Description = "Func_Trend_Skill_Charisma", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61505", Description = "Func_Trend_Skill_Comedy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61506", Description = "Func_Trend_Skill_CookingGourmet", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61507", Description = "Func_Trend_Skill_CookingHomestyle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61508", Description = "Func_Trend_Skill_Dancing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61509", Description = "Func_Trend_Skill_DJMixing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61510", Description = "Func_Trend_Skill_Fishing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61511", Description = "Func_Trend_Skill_Fitness", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61512", Description = "Func_Trend_Skill_FlowerArranging", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61513", Description = "Func_Trend_Skill_Gardening", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61514", Description = "Func_Trend_Skill_Guitar", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61515", Description = "Func_Trend_Skill_Handiness", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61516", Description = "Func_Trend_Skill_Herbalism", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67619", Description = "Func_Trend_Skill_JuiceFizzing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2460", Description = "Func_Trend_Skill_Knit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "83970", Description = "Func_Trend_Skill_Knitting", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61517", Description = "Func_Trend_Skill_LocalCulture", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61518", Description = "Func_Trend_Skill_Logic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61630", Description = "Func_Trend_Skill_MediaProduction", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61519", Description = "Func_Trend_Skill_Mischief", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61520", Description = "Func_Trend_Skill_Mixology", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61521", Description = "Func_Trend_Skill_Painting", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61522", Description = "Func_Trend_Skill_Parenting", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61523", Description = "Func_Trend_Skill_PetTraining", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61524", Description = "Func_Trend_Skill_Photography", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61525", Description = "Func_Trend_Skill_Piano", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61526", Description = "Func_Trend_Skill_PipeOrgan", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61527", Description = "Func_Trend_Skill_Programming", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65632", Description = "Func_Trend_Skill_Robotics", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61631", Description = "Func_Trend_Skill_RocketScience", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61528", Description = "Func_Trend_Skill_Singing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61529", Description = "Func_Trend_Skill_VampireLore", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61530", Description = "Func_Trend_Skill_Veterinarian", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61531", Description = "Func_Trend_Skill_VideoGaming", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61532", Description = "Func_Trend_Skill_Violin", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61533", Description = "Func_Trend_Skill_Wellness", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61534", Description = "Func_Trend_Skill_Writing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61545", Description = "Func_Trend_Tips_Beauty", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61546", Description = "Func_Trend_Tips_Fashion", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61550", Description = "Func_Trend_ToddlerChild", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61551", Description = "Func_Trend_Travel", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61535", Description = "Func_Trend_Vlog_Angry", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61536", Description = "Func_Trend_Vlog_Confident", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61537", Description = "Func_Trend_Vlog_Dazed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61538", Description = "Func_Trend_Vlog_Embarrassed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61539", Description = "Func_Trend_Vlog_Energized", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61540", Description = "Func_Trend_Vlog_Flirty", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61610", Description = "Func_Trend_Vlog_Focused", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61541", Description = "Func_Trend_Vlog_Happy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61542", Description = "Func_Trend_Vlog_Inspired", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61543", Description = "Func_Trend_Vlog_Playful", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61544", Description = "Func_Trend_Vlog_Sad", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1424", Description = "Func_Triangle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1135", Description = "Func_Trim", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1292", Description = "Func_Trunk", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1241", Description = "Func_Turtle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "471", Description = "Func_TV", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2243", Description = "Func_TVStandSearch", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "8215", Description = "Func_Twist", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59430", Description = "Func_Umbrella", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59443", Description = "Func_Umbrella_Adult", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59444", Description = "Func_Umbrella_Child", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59441", Description = "Func_UmbrellaRack", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1553", Description = "Func_UmbrellaTable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2118", Description = "Func_UmbrellaUser", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1172", Description = "Func_Unbreakable_Object", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "507", Description = "Func_Unicorn", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2235", Description = "Func_University_Text_Book", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65626", Description = "Func_UniversityHousing_Bed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65575", Description = "Func_UniversityKiosk_Academic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65574", Description = "Func_UniversityKiosk_DecoSurface", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65573", Description = "Func_UniversityKiosk_DecoWall", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65615", Description = "Func_UniversityKiosk_DecoWall_ST", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65564", Description = "Func_UniversityKiosk_Item", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65614", Description = "Func_UniversityKiosk_Item_ST", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2175", Description = "Func_Unused_USE_ME", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2228", Description = "Func_Unused_USE_ME2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1076", Description = "Func_Urn", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "814", Description = "Func_Urnstone", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1370", Description = "Func_ValentinesDay", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "40961", Description = "Func_VampireTome", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "40974", Description = "Func_VampireTome_Set1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "40975", Description = "Func_VampireTome_Set2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "40976", Description = "Func_VampireTome_Set3", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "40977", Description = "Func_VampireTome_Ultimate", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "36866", Description = "Func_VanityTable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1145", Description = "Func_Vase", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61472", Description = "Func_VaultDoor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61471", Description = "Func_VaultSafe", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2226", Description = "Func_Vehicle", Function = "Vehicle", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2227", Description = "Func_Vehicle_Bike", Function = "Vehicle", Subfunction = "Bike" },
            new S4CategoryTag(){ TypeID = "2231", Description = "Func_Vehicle_Land", Function = "Vehicle", Subfunction = "Land" },
            new S4CategoryTag(){ TypeID = "2232", Description = "Func_Vehicle_Water", Function = "Vehicle", Subfunction = "Water" },
            new S4CategoryTag(){ TypeID = "2013", Description = "Func_Venue_NotDestroyableByCleanup", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67618", Description = "Func_VerticalGarden", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57378", Description = "Func_Vet", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57375", Description = "Func_Vet_ExamTable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57428", Description = "Func_Vet_MedicineStation", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57374", Description = "Func_Vet_Podium", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57390", Description = "Func_Vet_SurgeryStation", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57430", Description = "Func_VetVendingMachine", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61479", Description = "Func_VFXMachine_ControlDesk", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61468", Description = "Func_VFXMachine_Emitter", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1644", Description = "Func_Video_Game", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "479", Description = "Func_Videogame", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55368", Description = "Func_VideoGameConsoleDisplay", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24596", Description = "Func_VideoGaming", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61474", Description = "Func_VideoRecording", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2189", Description = "Func_VideoRecording_BG", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61473", Description = "Func_VideoStation", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1128", Description = "Func_Villain", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "569", Description = "Func_Violin", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1635", Description = "Func_ViolinAdult", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61477", Description = "Func_VIPRope", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "490", Description = "Func_Vocal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "582", Description = "Func_Voodoo", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1085", Description = "Func_Wainscoting", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "26626", Description = "Func_WaiterStation", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2029", Description = "Func_Wall_TestLoS", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1112", Description = "Func_WallLamp", Function = "Lights", Subfunction = "Wall" },
            new S4CategoryTag(){ TypeID = "49173", Description = "Func_Wands", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61441", Description = "Func_WardrobePedestal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12375", Description = "Func_WarmingRack", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63490", Description = "Func_WaterScooter", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2197", Description = "Func_WaterScooter_BeachVenue", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67630", Description = "Func_WaxBlock", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1191", Description = "Func_Web", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18453", Description = "Func_Wellness", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1215", Description = "Func_Werewolf", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61480", Description = "Func_WFS", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61612", Description = "Func_WFS_PreMadeCelebrity", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "882", Description = "Func_Whirlpool_Tub", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1279", Description = "Func_Wilderness", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "34819", Description = "Func_WindChimes", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67614", Description = "Func_WindTurbine", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2437", Description = "Func_WindTurbine_UpgradedLightningRod", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "30722", Description = "Func_WishingWell", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1218", Description = "Func_Witch", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1319", Description = "Func_Wood", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1462", Description = "Func_Woodworking", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "493", Description = "Func_Workbench", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "472", Description = "Func_Workout", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1324", Description = "Func_WorkoutMachine", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1117", Description = "Func_Writer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1106", Description = "Func_Writing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1331", Description = "Func_Xmas", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "83972", Description = "Func_YarnBasket", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "83971", Description = "Func_Yarny", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "83991", Description = "Func_YarnyStatue", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18458", Description = "Func_yoga", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18447", Description = "Func_YogaClass_InstructorMat", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18448", Description = "Func_YogaClass_MemberMat", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18449", Description = "Func_YogaClass_MemberTempMat", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18433", Description = "Func_YogaMat", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57356", Description = "Fur_Chow", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57364", Description = "Fur_Collie", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57357", Description = "Fur_MediumSmooth", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57366", Description = "Fur_MediumWiry", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57358", Description = "Fur_Poodle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57359", Description = "Fur_Retriever", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57365", Description = "Fur_Spaniel", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2018", Description = "FurLength_Hairless", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2017", Description = "FurLength_LongHair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2016", Description = "FurLength_ShortHair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1530", Description = "GenderAppropriate_Female", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1529", Description = "GenderAppropriate_Male", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "877", Description = "Genre_ActivityTable_Dino", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "878", Description = "Genre_ActivityTable_Family", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "879", Description = "Genre_ActivityTable_Horse", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "880", Description = "Genre_ActivityTable_Shapes", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "881", Description = "Genre_ActivityTable_Truck", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "768", Description = "Genre_Book_Biography", Function = "Book", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "769", Description = "Genre_Book_Childrens", Function = "Book", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "790", Description = "Genre_Book_Emotion_Confident", Function = "Book", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "791", Description = "Genre_Book_Emotion_Energized", Function = "Book", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "792", Description = "Genre_Book_Emotion_Flirty", Function = "Book", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1038", Description = "Genre_Book_Emotion_Focused", Function = "Book", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1039", Description = "Genre_Book_Emotion_Inspired", Function = "Book", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "793", Description = "Genre_Book_Emotion_Playful", Function = "Book", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "794", Description = "Genre_Book_Emotion_Sad", Function = "Book", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "980", Description = "Genre_Book_Emotional", Function = "Book", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "770", Description = "Genre_Book_Fantasy", Function = "Book", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2224", Description = "Genre_Book_Magic", Function = "Book", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "866", Description = "Genre_Book_Mystery-Thriller", Function = "Book", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "771", Description = "Genre_Book_NonFiction", Function = "Book", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "772", Description = "Genre_Book_Poems", Function = "Book", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "773", Description = "Genre_Book_Romance", Function = "Book", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "774", Description = "Genre_Book_SciFi", Function = "Book", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "775", Description = "Genre_Book_ScreenPlay", Function = "Book", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "776", Description = "Genre_Book_ShortStories", Function = "Book", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1032", Description = "Genre_Book_Skill", Function = "Book", Subfunction = "Skill Book" },
            new S4CategoryTag(){ TypeID = "61493", Description = "Genre_Book_Skill_Acting", Function = "Book", Subfunction = "Skill Book" },
            new S4CategoryTag(){ TypeID = "45069", Description = "Genre_Book_Skill_Archaeology", Function = "Book", Subfunction = "Skill Book" },
            new S4CategoryTag(){ TypeID = "797", Description = "Genre_Book_Skill_Bartending", Function = "Book", Subfunction = "Skill Book" },
            new S4CategoryTag(){ TypeID = "798", Description = "Genre_Book_Skill_Charisma", Function = "Book", Subfunction = "Skill Book" },
            new S4CategoryTag(){ TypeID = "799", Description = "Genre_Book_Skill_Comedy", Function = "Book", Subfunction = "Skill Book" },
            new S4CategoryTag(){ TypeID = "800", Description = "Genre_Book_Skill_Cooking", Function = "Book", Subfunction = "Skill Book" },
            new S4CategoryTag(){ TypeID = "67621", Description = "Genre_Book_Skill_Fabrication", Function = "Book", Subfunction = "Skill Book" },
            new S4CategoryTag(){ TypeID = "921", Description = "Genre_Book_Skill_Fishing", Function = "Book", Subfunction = "Skill Book" },
            new S4CategoryTag(){ TypeID = "810", Description = "Genre_Book_Skill_Fitness", Function = "Book", Subfunction = "Skill Book" },
            new S4CategoryTag(){ TypeID = "801", Description = "Genre_Book_Skill_Gardening", Function = "Book", Subfunction = "Skill Book" },
            new S4CategoryTag(){ TypeID = "802", Description = "Genre_Book_Skill_Gourmet", Function = "Book", Subfunction = "Skill Book" },
            new S4CategoryTag(){ TypeID = "803", Description = "Genre_Book_Skill_Guitar", Function = "Book", Subfunction = "Skill Book" },
            new S4CategoryTag(){ TypeID = "804", Description = "Genre_Book_Skill_Hacking", Function = "Book", Subfunction = "Skill Book" },
            new S4CategoryTag(){ TypeID = "805", Description = "Genre_Book_Skill_Handiness", Function = "Book", Subfunction = "Skill Book" },
            new S4CategoryTag(){ TypeID = "10256", Description = "Genre_Book_Skill_Herbalism", Function = "Book", Subfunction = "Skill Book" },
            new S4CategoryTag(){ TypeID = "806", Description = "Genre_Book_Skill_Logic", Function = "Book", Subfunction = "Skill Book" },
            new S4CategoryTag(){ TypeID = "807", Description = "Genre_Book_Skill_Mischief", Function = "Book", Subfunction = "Skill Book" },
            new S4CategoryTag(){ TypeID = "808", Description = "Genre_Book_Skill_Painting", Function = "Book", Subfunction = "Skill Book" },
            new S4CategoryTag(){ TypeID = "43012", Description = "Genre_Book_Skill_Parenting", Function = "Book", Subfunction = "Skill Book" },
            new S4CategoryTag(){ TypeID = "809", Description = "Genre_Book_Skill_Piano", Function = "Book", Subfunction = "Skill Book" },
            new S4CategoryTag(){ TypeID = "2246", Description = "Genre_Book_Skill_ResearchDebate", Function = "Book", Subfunction = "Skill Book" },
            new S4CategoryTag(){ TypeID = "65623", Description = "Genre_Book_Skill_Robotics", Function = "Book", Subfunction = "Skill Book" },
            new S4CategoryTag(){ TypeID = "811", Description = "Genre_Book_Skill_RocketScience", Function = "Book", Subfunction = "Skill Book" },
            new S4CategoryTag(){ TypeID = "812", Description = "Genre_Book_Skill_VideoGaming", Function = "Book", Subfunction = "Skill Book" },
            new S4CategoryTag(){ TypeID = "813", Description = "Genre_Book_Skill_Violin", Function = "Book", Subfunction = "Skill Book" },
            new S4CategoryTag(){ TypeID = "865", Description = "Genre_Book_Skill_WooHoo", Function = "Book", Subfunction = "Skill Book" },
            new S4CategoryTag(){ TypeID = "818", Description = "Genre_Book_Skill_Writing", Function = "Book", Subfunction = "Skill Book" },
            new S4CategoryTag(){ TypeID = "819", Description = "Genre_Book_Supernatural", Function = "Book", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1656", Description = "Genre_Book_Toddler_PictureBook", Function = "Book", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45071", Description = "Genre_Book_TravelGuide", Function = "Book", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "667", Description = "Genre_Painting_Abstract", Function = "Painting", Subfunction = "Abstract" },
            new S4CategoryTag(){ TypeID = "669", Description = "Genre_Painting_Classics", Function = "Painting", Subfunction = "Classics" },
            new S4CategoryTag(){ TypeID = "670", Description = "Genre_Painting_Impressionism", Function = "Painting", Subfunction = "Impressionism" },
            new S4CategoryTag(){ TypeID = "10260", Description = "Genre_Painting_Landscape", Function = "Painting", Subfunction = "Landscape" },
            new S4CategoryTag(){ TypeID = "671", Description = "Genre_Painting_Mathematics", Function = "Painting", Subfunction = "Mathematics" },
            new S4CategoryTag(){ TypeID = "672", Description = "Genre_Painting_PopArt", Function = "Painting", Subfunction = "Pop Art" },
            new S4CategoryTag(){ TypeID = "673", Description = "Genre_Painting_Realism", Function = "Painting", Subfunction = "Realisim" },
            new S4CategoryTag(){ TypeID = "674", Description = "Genre_Painting_Surrealism", Function = "Painting", Subfunction = "Surrealism" },
            new S4CategoryTag(){ TypeID = "1436", Description = "Group_Photo_X_Actor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1437", Description = "Group_Photo_Y_Actor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2217", Description = "Group_Photo_Z_Actor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "314", Description = "Hair_Curly", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "151", Description = "Hair_Long", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "150", Description = "Hair_Medium", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "149", Description = "Hair_Short", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "313", Description = "Hair_Straight", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "315", Description = "Hair_Wavy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "896", Description = "HairColor_Auburn", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "131", Description = "HairColor_Black", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "897", Description = "HairColor_BlackSaltAndPepper", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "94", Description = "HairColor_Blonde", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "132", Description = "HairColor_Brown", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "898", Description = "HairColor_BrownSaltAndPepper", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "899", Description = "HairColor_DarkBlue", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "133", Description = "HairColor_DarkBrown", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "900", Description = "HairColor_DirtyBlond", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "134", Description = "HairColor_Gray", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "901", Description = "HairColor_Green", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "902", Description = "HairColor_HotPink", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "135", Description = "HairColor_Orange", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "96", Description = "HairColor_Platinum", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "903", Description = "HairColor_PurplePastel", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "136", Description = "HairColor_Red", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "904", Description = "HairColor_Turquoise", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "905", Description = "HairColor_White", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "664", Description = "HairLength_Long", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "820", Description = "HairLength_Medium", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "662", Description = "HairLength_Short", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2173", Description = "HairLength_Updo", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12391", Description = "HairTexture_Bald", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "821", Description = "HairTexture_Curly", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "822", Description = "HairTexture_Straight", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "663", Description = "HairTexture_Wavy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "371", Description = "Hat_Brim", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "372", Description = "Hat_Brimless", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "373", Description = "Hat_Cap", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2428", Description = "Hat_PaperBag", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "642", Description = "household_member_1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "643", Description = "household_member_2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "644", Description = "household_member_3", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "645", Description = "household_member_4", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "646", Description = "household_member_5", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "647", Description = "household_member_6", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "648", Description = "household_member_7", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "649", Description = "household_member_8", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "401", Description = "Instrument_Violin", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57441", Description = "Interaction_Adoption", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "462", Description = "Interaction_All", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "43015", Description = "Interaction_Argument", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "689", Description = "Interaction_AskToLeaveLot", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1599", Description = "Interaction_BarVenue", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2127", Description = "Interaction_Basketball_Play", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2348", Description = "Interaction_Bathtub", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "863", Description = "Interaction_BeReadTo", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24590", Description = "Interaction_Bonfire", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "757", Description = "Interaction_BrowseResearch", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10262", Description = "Interaction_Campfire", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "750", Description = "Interaction_Charity", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "342", Description = "Interaction_Chat", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "781", Description = "Interaction_Clean", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1309", Description = "Interaction_Collect", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1613", Description = "Interaction_ComedyMic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "439", Description = "Interaction_Computer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1367", Description = "Interaction_Computer_Typing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "394", Description = "Interaction_Consume", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "358", Description = "Interaction_Cook", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47134", Description = "Interaction_CurioShop_BrowseBuy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "425", Description = "Interaction_Death", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12337", Description = "Interaction_Doctor_TreatPatient", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "654", Description = "Interaction_Drink", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67603", Description = "Interaction_EcoFootprint_Green", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57391", Description = "Interaction_ExamTable_Exam", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2131", Description = "Interaction_FashionBlog", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2058", Description = "Interaction_Festive", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24581", Description = "Interaction_FoosballTable_Play", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "431", Description = "Interaction_Friendly", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "432", Description = "Interaction_Funny", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55384", Description = "Interaction_GameConsole", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "926", Description = "Interaction_GoJogging", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67589", Description = "Interaction_GreenUpgraded", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "453", Description = "Interaction_Greeting", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "71683", Description = "Interaction_Group_Workout", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24607", Description = "Interaction_GroupDanceTogether", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "435", Description = "Interaction_Hack", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1990", Description = "Interaction_Hug", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "43028", Description = "Interaction_IgnoreGrounding", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47125", Description = "Interaction_Infect_House", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "639", Description = "Interaction_InstrumentListen", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "746", Description = "Interaction_IntelligenceResearch", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12368", Description = "Interaction_InventionConstructor_Upgrade", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "417", Description = "Interaction_InviteToStay", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "871", Description = "Interaction_Joke", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2347", Description = "Interaction_JuiceKeg", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1600", Description = "Interaction_KaraokeVenue", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "350", Description = "Interaction_Kiss", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "83984", Description = "Interaction_Knitting", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2035", Description = "Interaction_Laundry_GenerateNoPile", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2034", Description = "Interaction_Laundry_PutAwayFinishedLaundry", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "420", Description = "Interaction_Leave", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "419", Description = "Interaction_LeaveMustRun", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "444", Description = "Interaction_ListenMusic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "683", Description = "Interaction_MakeApp", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1028", Description = "Interaction_MakeCoffeeOrTea", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55400", Description = "Interaction_MarketStall_Tend", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1934", Description = "Interaction_MarketStalls_Tend", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18439", Description = "Interaction_MassageTable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "433", Description = "Interaction_Mean", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "455", Description = "Interaction_Mentor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "695", Description = "Interaction_MentorMusic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "434", Description = "Interaction_Mischievous", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "461", Description = "Interaction_Mixer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "591", Description = "Interaction_Nap", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1698", Description = "Interaction_NestingBlocks", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1628", Description = "Interaction_NoisyElectronics", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1598", Description = "Interaction_Observatory", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67638", Description = "Interaction_OldDay_Fine", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "694", Description = "Interaction_Paint", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1372", Description = "Interaction_PaintByReference", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55359", Description = "Interaction_PaintMural", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1601", Description = "Interaction_ParkVenue", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2061", Description = "Interaction_Party", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "469", Description = "Interaction_PerformComedyRoutine", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57397", Description = "Interaction_PetMisbehavior", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57370", Description = "Interaction_Pets_Friendly", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57372", Description = "Interaction_Pets_Greeting", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57371", Description = "Interaction_Pets_Mean", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2015", Description = "Interaction_Pets_SS3Allowed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1942", Description = "Interaction_PhotoStudio_TakePicture", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1618", Description = "Interaction_PlayDJBooth", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "640", Description = "Interaction_PlayGame", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1615", Description = "Interaction_PlayGuitar", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1024", Description = "Interaction_PlayGuitarForTips", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "442", Description = "Interaction_PlayInstrument", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "443", Description = "Interaction_PlayInstrumentForTips", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "606", Description = "Interaction_PlayInstrumentOrComedyForTips", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "690", Description = "Interaction_PlayPiano", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1025", Description = "Interaction_PlayPianoforTips", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1339", Description = "Interaction_PlayToy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "685", Description = "Interaction_PlayVideoGames", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1616", Description = "Interaction_PlayViolin", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1026", Description = "Interaction_PlayViolinForTips", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57362", Description = "Interaction_PlayWithCat", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57363", Description = "Interaction_PlayWithDog", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61552", Description = "Interaction_Practice_Acting", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "693", Description = "Interaction_PracticeCoding", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65648", Description = "Interaction_PracticeDebate", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "692", Description = "Interaction_PracticeWriting", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "583", Description = "Interaction_Prank", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "752", Description = "Interaction_PrankObject", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "751", Description = "Interaction_Programming", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "660", Description = "Interaction_PublishBook", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "931", Description = "Interaction_ReadtoChild", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "464", Description = "Interaction_Repair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2151", Description = "Interaction_Restaurant_WaitToPlaceOrder", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12347", Description = "Interaction_Retail", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "465", Description = "Interaction_Rocket", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "438", Description = "Interaction_Rocketship_Launch", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "437", Description = "Interaction_Rocketship_Upgrade", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57443", Description = "Interaction_RunAway", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "43026", Description = "Interaction_SchoolWork", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "786", Description = "Interaction_ScienceTable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59420", Description = "Interaction_Season_Fall", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59418", Description = "Interaction_Season_Spring", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59419", Description = "Interaction_Season_Summer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59421", Description = "Interaction_Season_Winter", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "661", Description = "Interaction_SellArt", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1447", Description = "Interaction_Shower", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "427", Description = "Interaction_Showoff", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55362", Description = "Interaction_SimTV", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "79876", Description = "Interaction_Situation_Photography", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59395", Description = "Interaction_Skating_IceSkating", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59396", Description = "Interaction_Skating_RollerSkating", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59397", Description = "Interaction_Skating_Routine", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59394", Description = "Interaction_Skating_Skating", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59401", Description = "Interaction_Skating_Trick", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2132", Description = "Interaction_Sketch", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2340", Description = "Interaction_Skill_Acting", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2346", Description = "Interaction_Skill_Baking", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "835", Description = "Interaction_Skill_Bartending", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "837", Description = "Interaction_Skill_Charisma", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "853", Description = "Interaction_Skill_Child_Creativity", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "854", Description = "Interaction_Skill_Child_Mental", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "855", Description = "Interaction_Skill_Child_Motor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "856", Description = "Interaction_Skill_Child_Social", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "838", Description = "Interaction_Skill_Comedy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2343", Description = "Interaction_Skill_Dancing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2342", Description = "Interaction_Skill_DJMixing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57373", Description = "Interaction_Skill_DogTraining", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2434", Description = "Interaction_Skill_Fabrication", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "839", Description = "Interaction_Skill_Fishing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "836", Description = "Interaction_Skill_Fitness", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2344", Description = "Interaction_Skill_FlowerArrangement", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "834", Description = "Interaction_Skill_Gardening", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "840", Description = "Interaction_Skill_GourmetCooking", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "841", Description = "Interaction_Skill_Guitar", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "842", Description = "Interaction_Skill_Handiness", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2339", Description = "Interaction_Skill_Herbalism", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "843", Description = "Interaction_Skill_HomestyleCooking", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2424", Description = "Interaction_Skill_JuiceFizzing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2461", Description = "Interaction_Skill_Knitting", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "844", Description = "Interaction_Skill_Logic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2338", Description = "Interaction_Skill_MediaProduction", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "845", Description = "Interaction_Skill_Mischief", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "846", Description = "Interaction_Skill_Painting", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1938", Description = "Interaction_Skill_Photography", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "847", Description = "Interaction_Skill_Piano", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2341", Description = "Interaction_Skill_PipeOrgan", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "848", Description = "Interaction_Skill_Programming", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2345", Description = "Interaction_Skill_Robotics", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "849", Description = "Interaction_Skill_RocketScience", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55364", Description = "Interaction_Skill_Singing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1617", Description = "Interaction_Skill_SingingKaraoke", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "850", Description = "Interaction_Skill_VideoGaming", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "851", Description = "Interaction_Skill_Violin", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18465", Description = "Interaction_Skill_Wellness", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2337", Description = "Interaction_Skill_Wellness_BG", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "852", Description = "Interaction_Skill_Writing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "451", Description = "Interaction_Sleep", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2094", Description = "Interaction_SleepGroup", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59477", Description = "Interaction_SleepNap", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2093", Description = "Interaction_SniffNewObjects", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2041", Description = "Interaction_Social_Contagious", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2163", Description = "Interaction_Social_Touching", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2161", Description = "Interaction_SocialAll", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1619", Description = "Interaction_SocialMediaCheckIn", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55319", Description = "Interaction_SocialMediaPersuadeTo", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2162", Description = "Interaction_SocialMixer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1595", Description = "Interaction_SocialNetwork", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "454", Description = "Interaction_SocialSuper", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55361", Description = "Interaction_SprayGraffiti", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "876", Description = "Interaction_StereoDance", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "638", Description = "Interaction_StereoListen", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1723", Description = "Interaction_StuffedAnimal_Babble", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "460", Description = "Interaction_Super", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57392", Description = "Interaction_SurgeryStation_Exam", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1591", Description = "Interaction_Swim", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1640", Description = "Interaction_Take_Pizza", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1939", Description = "Interaction_TakePhoto", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59439", Description = "Interaction_TalkLikeAPirateDay", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1719", Description = "Interaction_TeenCareerRabbitHole", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "436", Description = "Interaction_Telescope", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "466", Description = "Interaction_TellStory", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10261", Description = "Interaction_Tent_Sleep", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59417", Description = "Interaction_Throwing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59425", Description = "Interaction_Throwing_Mud", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59424", Description = "Interaction_Throwing_Snowball", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59426", Description = "Interaction_Throwing_WaterBalloon", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "749", Description = "Interaction_Tournament", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10248", Description = "Interaction_TransferFireleafRash", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "353", Description = "Interaction_Treadmill", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "452", Description = "Interaction_TryForBaby", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65609", Description = "Interaction_University_StudyWith", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "658", Description = "Interaction_Upgrade", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "396", Description = "Interaction_UseToilet", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1641", Description = "Interaction_VideoGameLivestream", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "655", Description = "Interaction_VideoGameMoney", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1642", Description = "Interaction_VideoGameStreamLetsPlay", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "758", Description = "Interaction_ViewArt", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "449", Description = "Interaction_VisitLot", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "426", Description = "Interaction_Voodoo", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "26634", Description = "Interaction_WaitstaffIdle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1597", Description = "Interaction_WatchPerformer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "450", Description = "Interaction_WatchTV", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55320", Description = "Interaction_WatchTV_Cooking", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55321", Description = "Interaction_WatchTV_RomComAct", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59423", Description = "Interaction_Weather_Rain", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59422", Description = "Interaction_Weather_Snow", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1612", Description = "Interaction_Woodworking", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "463", Description = "Interaction_Workout", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "354", Description = "Interaction_WorkoutMachine", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1171", Description = "Interaction_WorkoutPushTheLimits", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55360", Description = "Interaction_Write", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "665", Description = "Interaction_WriteArticle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "696", Description = "Interaction_WriteJokes", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18461", Description = "Interaction_YogaClassMember", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2350", Description = "Inventory_Books_Fun", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2352", Description = "Inventory_Books_Other", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2351", Description = "Inventory_Books_Skill", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2353", Description = "Inventory_Collectible_Creature", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2354", Description = "Inventory_Collectible_Decoration", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2355", Description = "Inventory_Collectible_Natural", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2356", Description = "Inventory_Collectible_Other", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2358", Description = "Inventory_Consumable_Drink", Function = "MOD: Drink", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2357", Description = "Inventory_Consumable_Food", Function = "MOD: Food", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2359", Description = "Inventory_Consumable_Other", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2360", Description = "Inventory_Gardening_Other", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2362", Description = "Inventory_HomeSkill_Decoration", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2363", Description = "Inventory_HomeSkill_Home", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2364", Description = "Inventory_HomeSkill_LittleOnes", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2361", Description = "Inventory_HomeSkill_Skill", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2459", Description = "Inventory_Plopsy_All", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2457", Description = "Inventory_Plopsy_Listed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2458", Description = "Inventory_Plopsy_Pending_Sale", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "83989", Description = "Inventory_Plopsy_Unavailable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2371", Description = "Inventory_Scraps_Junk", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2370", Description = "Inventory_Scraps_Parts", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2368", Description = "Inventory_SimCrafted_Artwork", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2369", Description = "Inventory_SimCrafted_Other", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2365", Description = "Inventory_Special_CareerActivity", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2366", Description = "Inventory_Special_Education", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2367", Description = "Inventory_Special_Story", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2145", Description = "Job_RestaurantDiner", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1464", Description = "Job_Venue", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57442", Description = "Job_VetPatient", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1463", Description = "Job_Walkby", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "346", Description = "Mailbox", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57349", Description = "Main_Pet_Social", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "588", Description = "Mentor_ActivityTable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "365", Description = "Mentor_Easel", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "357", Description = "Mentor_Fitness", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "361", Description = "Mentor_Guitar", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55398", Description = "Mentor_Mural", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "362", Description = "Mentor_Piano", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "765", Description = "Mentor_Repair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "355", Description = "Mentor_Treadmill", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "766", Description = "Mentor_Upgrade", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "363", Description = "Mentor_Violin", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "764", Description = "Mentor_WoodworkingTable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "356", Description = "Mentor_WorkoutMachine", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "344", Description = "MicroscopeSlide_Crystal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "343", Description = "MicroscopeSlide_Fossil", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "345", Description = "MicroscopeSlide_Plant", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "317", Description = "Mood_Angry", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "318", Description = "Mood_Bored", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "319", Description = "Mood_Confident", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "320", Description = "Mood_Cranky", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "321", Description = "Mood_Depressed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "322", Description = "Mood_Drunk", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "323", Description = "Mood_Embarrassed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "324", Description = "Mood_Energized", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "331", Description = "Mood_Fine", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "325", Description = "Mood_Flirty", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "326", Description = "Mood_Focused", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "328", Description = "Mood_Happy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "329", Description = "Mood_Imaginative", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "64", Description = "Mood_Optimism", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "332", Description = "Mood_Playful", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "333", Description = "Mood_Sad", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "334", Description = "Mood_Sloshed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "327", Description = "Mood_Tense", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "330", Description = "Mood_Uncomfortable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24592", Description = "NoneEP03_PLEASE_REUSE_ME", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1917", Description = "NoseColor_Black", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1922", Description = "NoseColor_BlackPink", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1918", Description = "NoseColor_Brown", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1923", Description = "NoseColor_BrownPink", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1919", Description = "NoseColor_Liver", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1920", Description = "NoseColor_Pink", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1921", Description = "NoseColor_Tan", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1540", Description = "NudePart_Always", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1541", Description = "NudePart_MaleWithBreast", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "349", Description = "Object_Bar", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55363", Description = "Object_Mural", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12319", Description = "Occult_Alien", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1310", Description = "Occult_Human", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2208", Description = "Occult_Mermaid", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1677", Description = "Occult_Vampire", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2279", Description = "Occult_Witch", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55393", Description = "Outfit_ArtCritic_Level10", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55301", Description = "Outfit_ArtsCritic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55300", Description = "Outfit_FoodCritic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55394", Description = "Outfit_FoodCritic_Level10", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "80", Description = "OutfitCategory_Athletic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "82", Description = "OutfitCategory_Bathing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "263", Description = "OutfitCategory_Career", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2054", Description = "OutfitCategory_ColdWeather", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "77", Description = "OutfitCategory_Everyday", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "78", Description = "OutfitCategory_Formal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2053", Description = "OutfitCategory_HotWeather", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "83", Description = "OutfitCategory_Party", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1371", Description = "OutfitCategory_RetailUniforms", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "335", Description = "OutfitCategory_Situation", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "81", Description = "OutfitCategory_Sleep", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1229", Description = "OutfitCategory_Swimwear", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "79", Description = "OutfitCategory_Unused", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "8210", Description = "OutfitCategory_Witch", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "590", Description = "Pattern_Animal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1905", Description = "Pattern_Bicolor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1902", Description = "Pattern_Brindle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1912", Description = "Pattern_Calico", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1909", Description = "Pattern_Harlequin", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1907", Description = "Pattern_Merle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1910", Description = "Pattern_Sable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1903", Description = "Pattern_Saddle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1913", Description = "Pattern_Speckled", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1900", Description = "Pattern_Spotted", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1901", Description = "Pattern_Striped", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1904", Description = "Pattern_Swirled", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1899", Description = "Pattern_Tabby", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1906", Description = "Pattern_Tricolor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1908", Description = "Pattern_Tuxedo", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "130", Description = "Persona_Boho", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "129", Description = "Persona_Fashionista", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "148", Description = "Persona_Mom", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "128", Description = "Persona_Rocker", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "668", Description = "PortalDisallowance_Ungreeted", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67604", Description = "Recipe_CandleMakingStation_Candle", Function = "MOD: Recipe", Subfunction = "Candle" },
            new S4CategoryTag(){ TypeID = "1536", Description = "Recipe_Category_CakePie", Function = "MOD: Recipe", Subfunction = "Cake or Pie" },
            new S4CategoryTag(){ TypeID = "1537", Description = "Recipe_Category_Chocolate", Function = "MOD: Recipe", Subfunction = "Chocolate" },
            new S4CategoryTag(){ TypeID = "1533", Description = "Recipe_Category_Cold", Function = "MOD: Recipe", Subfunction = "Cold" },
            new S4CategoryTag(){ TypeID = "1518", Description = "Recipe_Category_Drinks", Function = "MOD: Recipe", Subfunction = "Drinks" },
            new S4CategoryTag(){ TypeID = "1531", Description = "Recipe_Category_Fizzy", Function = "MOD: Recipe", Subfunction = "Fizzy" },
            new S4CategoryTag(){ TypeID = "1532", Description = "Recipe_Category_Fruit", Function = "MOD: Recipe", Subfunction = "Fruit" },
            new S4CategoryTag(){ TypeID = "1515", Description = "Recipe_Category_Grains", Function = "MOD: Recipe", Subfunction = "Grains" },
            new S4CategoryTag(){ TypeID = "1534", Description = "Recipe_Category_Hot", Function = "MOD: Recipe", Subfunction = "Hot" },
            new S4CategoryTag(){ TypeID = "1513", Description = "Recipe_Category_Meat", Function = "MOD: Recipe", Subfunction = "Meat" },
            new S4CategoryTag(){ TypeID = "1517", Description = "Recipe_Category_Misc", Function = "MOD: Recipe", Subfunction = "Misc" },
            new S4CategoryTag(){ TypeID = "1535", Description = "Recipe_Category_Nectar", Function = "MOD: Recipe", Subfunction = "Nectar" },
            new S4CategoryTag(){ TypeID = "1519", Description = "Recipe_Category_Seafood", Function = "MOD: Recipe", Subfunction = "Seafood" },
            new S4CategoryTag(){ TypeID = "1516", Description = "Recipe_Category_Sweets", Function = "MOD: Recipe", Subfunction = "Sweets" },
            new S4CategoryTag(){ TypeID = "1514", Description = "Recipe_Category_Vegetarian", Function = "MOD: Recipe", Subfunction = "Vegetarian" },
            new S4CategoryTag(){ TypeID = "1522", Description = "Recipe_Category_Water", Function = "MOD: Recipe", Subfunction = "Water" },
            new S4CategoryTag(){ TypeID = "49154", Description = "Recipe_Cauldron_Potion", Function = "MOD: Recipe", Subfunction = "Cauldron Potion" },
            new S4CategoryTag(){ TypeID = "1521", Description = "Recipe_ChefsChoice_ChildFriendly", Function = "MOD: Recipe", Subfunction = "Chef’s Choice (Child Friendly)" },
            new S4CategoryTag(){ TypeID = "1523", Description = "Recipe_ChildRestricted", Function = "MOD: Recipe", Subfunction = "Child Restricted" },
            new S4CategoryTag(){ TypeID = "1507", Description = "Recipe_Course_Appetizer", Function = "MOD: Recipe", Subfunction = "Appetizer" },
            new S4CategoryTag(){ TypeID = "1509", Description = "Recipe_Course_Dessert", Function = "MOD: Recipe", Subfunction = "Dessert" },
            new S4CategoryTag(){ TypeID = "1524", Description = "Recipe_Course_Drink", Function = "MOD: Recipe", Subfunction = "Drink" },
            new S4CategoryTag(){ TypeID = "1508", Description = "Recipe_Course_Main", Function = "MOD: Recipe", Subfunction = "Main Course" },
            new S4CategoryTag(){ TypeID = "59472", Description = "Recipe_FlowerArrangement", Function = "MOD: Recipe", Subfunction = "Flower Arrangement" },
            new S4CategoryTag(){ TypeID = "1510", Description = "Recipe_Meal_Breakfast", Function = "MOD: Recipe", Subfunction = "Breakfast Meal" },
            new S4CategoryTag(){ TypeID = "1512", Description = "Recipe_Meal_Dinner", Function = "MOD: Recipe", Subfunction = "Dinner Meal" },
            new S4CategoryTag(){ TypeID = "1511", Description = "Recipe_Meal_Lunch", Function = "MOD: Recipe", Subfunction = "Lunch Meal" },
            new S4CategoryTag(){ TypeID = "83985", Description = "Recipe_Plopsy_Browser", Function = "MOD: Recipe", Subfunction = "Plopsy Browser" },
            new S4CategoryTag(){ TypeID = "1506", Description = "Recipe_Type_Drink", Function = "MOD: Recipe", Subfunction = "Drink" },
            new S4CategoryTag(){ TypeID = "2423", Description = "Recipe_Type_Drink_Prank", Function = "MOD: Recipe", Subfunction = "Prank Drink" },
            new S4CategoryTag(){ TypeID = "1505", Description = "Recipe_Type_Food", Function = "MOD: Recipe", Subfunction = "Food" },
            new S4CategoryTag(){ TypeID = "57425", Description = "Recipe_Type_PetDrink", Function = "MOD: Recipe", Subfunction = "Pet Drink" },
            new S4CategoryTag(){ TypeID = "57424", Description = "Recipe_Type_PetFood", Function = "MOD: Recipe", Subfunction = "Pet Food" },
            new S4CategoryTag(){ TypeID = "12437", Description = "Region_ActiveCareer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1245", Description = "Region_Camping", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45059", Description = "Region_Jungle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1244", Description = "Region_Residential", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12374", Description = "Region_Retail", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "138", Description = "RESERVED_TempBetaFixDoNotUse", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "139", Description = "RESERVED_TempBetaFixDoNotUse2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "142", Description = "Reserved_TempBetaFixDoNotUse3", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "143", Description = "RESERVED_TempBetaFixDoNotUse4", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "144", Description = "RESERVED_TempBetaFixDoNotUse5", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "147", Description = "RESERVED_TempBetaFixDoNotUse6", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "281", Description = "RESERVED_TempBetaFixDoNotUse7", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "284", Description = "RESERVED_TempBetaFixDoNotUse8", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "290", Description = "RESERVED_TempBetaFixDoNotUse9", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "767", Description = "Reward_CASPart", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2277", Description = "Role_BakeOneCake", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "277", Description = "Role_Bartender", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1924", Description = "Role_Business_Customer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "467", Description = "Role_Career", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "278", Description = "Role_Caterer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65583", Description = "Role_CollegeOrganization_Event", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12292", Description = "Role_Coworker", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2142", Description = "Role_Customer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1439", Description = "Role_Date", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12294", Description = "Role_Detective", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12295", Description = "Role_Doctor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "650", Description = "Role_Entertainer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55317", Description = "Role_FestivalArtsCrafts", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55312", Description = "Role_FestivalBlossom", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55318", Description = "Role_FestivalFleaMarket", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55315", Description = "Role_FestivalFood", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55313", Description = "Role_FestivalLamp", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55314", Description = "Role_FestivalLogic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55316", Description = "Role_FestivalMusic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "8199", Description = "Role_FortuneTeller", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "266", Description = "Role_Guest", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "267", Description = "Role_Host", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "26635", Description = "Role_HostAtStation", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "418", Description = "Role_Leave", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "279", Description = "Role_Maid", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2149", Description = "Role_Restaurant_PostPlaceOrder", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2147", Description = "Role_RestaurantDiner", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2148", Description = "Role_RestaurantEat", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "26633", Description = "Role_RestaurantStaff", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65541", Description = "Role_RoommateNPC", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12293", Description = "Role_Scientist", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "416", Description = "Role_Service", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18441", Description = "Role_SpaStaff_Bored", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57400", Description = "Role_Vet_Patient", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2143", Description = "Role_VIPRope_Allowed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18463", Description = "Role_Yoga_PreClass", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18435", Description = "Role_YogaClass_PostClass", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12434", Description = "RoleState_EP01_Patient_Treated", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "908", Description = "Royalty_Apps", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "909", Description = "Royalty_Books", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "910", Description = "Royalty_Games", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1629", Description = "Royalty_Lyrics", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "911", Description = "Royalty_Paintings", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "912", Description = "Royalty_Songs", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "383", Description = "Shoes_Booties", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "384", Description = "Shoes_Boots", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "385", Description = "Shoes_Flats", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "386", Description = "Shoes_Heels", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "387", Description = "Shoes_LaceUpAdult", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "388", Description = "Shoes_LaceUpChildren", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "389", Description = "Shoes_Loafers", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "390", Description = "Shoes_Sandals", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "391", Description = "Shoes_Slippers", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "392", Description = "Shoes_Sneakers", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "393", Description = "Shoes_Wedges", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57407", Description = "Sickness_CheckUp", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57451", Description = "Sickness_CuredBy_ExamTable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57452", Description = "Sickness_CuredBy_SurgeryStation", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57408", Description = "Sickness_Illness", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57403", Description = "Sickness_PetExam", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12358", Description = "Situation_ActiveCareer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12427", Description = "Situation_ActiveCareer_Scientist", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61553", Description = "situation_ActorCareer_Commercial", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61556", Description = "situation_ActorCareer_Movie", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61615", Description = "Situation_ActorCareer_PrepTask_Acting", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61458", Description = "Situation_ActorCareer_PrepTask_Charisma", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61456", Description = "Situation_ActorCareer_PrepTask_Comedy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61454", Description = "Situation_ActorCareer_PrepTask_CoStarRel", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61455", Description = "Situation_ActorCareer_PrepTask_DirectoRel", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61459", Description = "Situation_ActorCareer_PrepTask_Fitness", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61460", Description = "Situation_ActorCareer_PrepTask_Guitar", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61457", Description = "Situation_ActorCareer_PrepTask_Handiness", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61619", Description = "Situation_ActorCareer_PrepTask_Practice_Action", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61620", Description = "Situation_ActorCareer_PrepTask_Practice_Dramatic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61621", Description = "Situation_ActorCareer_PrepTask_Practice_Romantic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61616", Description = "Situation_ActorCareer_PrepTask_Research_Flirty", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61617", Description = "Situation_ActorCareer_PrepTask_Research_Funny", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61618", Description = "Situation_ActorCareer_PrepTask_Research_Mean", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61555", Description = "situation_ActorCareer_TVHigh", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61554", Description = "situation_ActorCareer_TVLow", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55304", Description = "Situation_ApartmentNeighbor_AnswerDoorComplaint", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55303", Description = "Situation_ApartmentNeighbor_LoudNoises", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55381", Description = "Situation_BasketBaller_A", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55382", Description = "Situation_BasketBaller_B", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10247", Description = "Situation_Bear", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24586", Description = "Situation_Bonfire", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "38919", Description = "Situation_Bowling_Group", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "38920", Description = "Situation_Bowling_Group_2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "38921", Description = "Situation_Bowling_Group_3", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "38922", Description = "Situation_Bowling_Group_4", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55308", Description = "Situation_Busker", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "36867", Description = "Situation_Butler", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61476", Description = "Situation_CelebrityFan", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55380", Description = "Situation_CityInvites", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55355", Description = "Situation_CityRepair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "955", Description = "Situation_Clown", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55425", Description = "Situation_ComplaintNoise", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1017", Description = "Situation_CookingInteractions", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "956", Description = "Situation_Criminal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24606", Description = "Situation_DanceTogether", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24582", Description = "Situation_DJPerformance", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1501", Description = "Situation_Event_NPC", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55401", Description = "Situation_Festival", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55390", Description = "Situation_Festival_Blossom_RomanticCouple", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55389", Description = "Situation_Festival_Logic_RocketShipWoohooers", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2377", Description = "Situation_Firefighter", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59476", Description = "Situation_FlowerBunny", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10259", Description = "Situation_ForestGhost", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10264", Description = "Situation_ForestRanger", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2152", Description = "Situation_Gardener", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59455", Description = "Situation_Gnome_Berserk", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59454", Description = "Situation_Gnome_Normal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47158", Description = "Situation_GP07_Walkby_Conspiracist_01", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47159", Description = "Situation_GP07_Walkby_Conspiracist_02", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47160", Description = "Situation_GP07_Walkby_Conspiracist_03", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47161", Description = "Situation_GP07_Walkby_FBI_01", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47162", Description = "Situation_GP07_Walkby_FBI_02", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47163", Description = "Situation_GP07_Walkby_FBI_03", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47150", Description = "Situation_GP07_Walkby_Military_01", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47151", Description = "Situation_GP07_Walkby_Military_02", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47152", Description = "Situation_GP07_Walkby_Military_03", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47153", Description = "Situation_GP07_Walkby_Military_04", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47154", Description = "Situation_GP07_Walkby_Scientist_01", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47155", Description = "Situation_GP07_Walkby_Scientist_02", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47156", Description = "Situation_GP07_Walkby_Scientist_03", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47157", Description = "Situation_GP07_Walkby_Scientist_04", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1461", Description = "Situation_GrillGroup", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1550", Description = "Situation_HiredNanny", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59460", Description = "Situation_Holiday", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "26642", Description = "Situation_HomeChef", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "958", Description = "Situation_HotDog", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55426", Description = "Situation_IntriguedNoise", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55427", Description = "Situation_IntriguedSmell", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63496", Description = "Situation_IslandSpirits", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55435", Description = "Situation_LivesOnStreet_A", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55436", Description = "Situation_LivesOnStreet_B", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55437", Description = "Situation_LivesOnStreet_C", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55438", Description = "Situation_LivesOnStreet_D", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "957", Description = "Situation_Maid", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1343", Description = "Situation_Mailman", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1949", Description = "Situation_MarketStall_Vendor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "889", Description = "Situation_MasterFisherman", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "890", Description = "Situation_MasterGardener", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55383", Description = "Situation_MuralPainter", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1679", Description = "Situation_NightTimeVisit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57427", Description = "Situation_PetObstacleCourse", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1460", Description = "Situation_PicnicTable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "960", Description = "Situation_Pizza", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1643", Description = "Situation_PlayerFacing_CanHost", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1493", Description = "Situation_PlayerVisiting_NPC", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47124", Description = "Situation_Possessed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24594", Description = "Situation_PromoNight", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "959", Description = "Situation_Reaper", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2153", Description = "Situation_Repairman", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2146", Description = "Situation_RestaurantDining", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12323", Description = "Situation_Retail_Customer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12324", Description = "Situation_Retail_Employee", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "684", Description = "Situation_Ring_Doorbell", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65572", Description = "Situation_RoommateNPC_Potential", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65570", Description = "Situation_SecretSociety", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22541", Description = "Situation_SpookyParty", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61634", Description = "Situation_Squad", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67647", Description = "Situation_Sun_Ray", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1504", Description = "Situation_TragicClown", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2167", Description = "Situation_Tutorial_FTUE", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2119", Description = "Situation_UmbrellaUser", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65571", Description = "Situation_UniversityHousingKickoutBlocker", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65606", Description = "Situation_UniversityRivals_Prank", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55391", Description = "Situation_Venue_Karaoke_Dueters", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57414", Description = "Situation_Vet_PlayerPetOwner", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57402", Description = "Situation_Vet_SickPet", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61613", Description = "Situation_VIPRope_Bouncer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67606", Description = "Situation_VisitorNPC_AngrySim", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2282", Description = "Situation_VisitorNPCs", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2078", Description = "Situation_Weather_Rain_Heavy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2079", Description = "Situation_Weather_Rain_Light", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2077", Description = "Situation_Weather_Rain_Storm", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2080", Description = "Situation_Weather_Snow_Heavy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2081", Description = "Situation_Weather_Snow_Storm", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55309", Description = "Situation_Weirdo", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1457", Description = "Situation_WelcomeWagon", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18462", Description = "Situation_YogaClass", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "448", Description = "Skill_All", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2097", Description = "Skill_All_Visible", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45094", Description = "Skill_Archaeology", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "86", Description = "Skill_Athletic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "137", Description = "Skill_Bartending", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "676", Description = "Skill_Charisma", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "641", Description = "Skill_Child", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1576", Description = "Skill_ComedyOrMischief", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "87", Description = "Skill_Cooking", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "336", Description = "Skill_Creative", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57367", Description = "Skill_DogTraining", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "652", Description = "Skill_FitnessOrProgramming", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59451", Description = "Skill_FlowerArranging", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1605", Description = "Skill_Gardening", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "935", Description = "Skill_GuitarorComedy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1368", Description = "Skill_Handiness", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67620", Description = "Skill_JuiceFizzing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "83969", Description = "Skill_Knitting", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45070", Description = "Skill_LocalCulture", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "677", Description = "Skill_Logic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "337", Description = "Skill_Mental", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "445", Description = "Skill_Musical", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55305", Description = "Skill_MusicOrComedy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1607", Description = "Skill_Painting", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1630", Description = "Skill_Performance", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1940", Description = "Skill_Photography", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1609", Description = "Skill_Photography_BG", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "338", Description = "Skill_Physical", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "40969", Description = "Skill_PipeOrgan", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1606", Description = "Skill_Programming", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "8194", Description = "Skill_Psychic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "678", Description = "Skill_RocketScience", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1653", Description = "Skill_SchoolTask", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1633", Description = "Skill_Singing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59393", Description = "Skill_Skating", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "339", Description = "Skill_Social", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1655", Description = "Skill_Toddler", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "675", Description = "Skill_VideoGaming", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "936", Description = "Skill_ViolinorGuitar", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18466", Description = "Skill_Wellness", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1608", Description = "Skill_Wellness_BG", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "679", Description = "Skill_Writing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12382", Description = "SkinHue_Blue", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1449", Description = "SkinHue_BlueSkin", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12389", Description = "SkinHue_Green", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1450", Description = "SkinHue_GreenSkin", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "763", Description = "SkinHue_Olive", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12390", Description = "SkinHue_Purple", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "761", Description = "SkinHue_Red", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1625", Description = "SkinHue_RedSkin", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "762", Description = "SkinHue_Yellow", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1458", Description = "SkintoneBlend_Yes", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12317", Description = "SkintoneType_Fantasy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12316", Description = "SkintoneType_Natural", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12320", Description = "SkintoneType_Sickness_1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12321", Description = "SkintoneType_Sickness_2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12322", Description = "SkintoneType_Sickness_3", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12325", Description = "SkintoneType_Sickness_Green", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "686", Description = "Social_BlackAndWhite", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "687", Description = "Social_CostumeParty", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "340", Description = "Social_Flirty", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10244", Description = "Social_WeenieRoast", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "364", Description = "Social_Woohoo", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "20487", Description = "SP03_PLEASE_REUSE_ME_I_WAS_BLANK_ON_ACCIDENT", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "20488", Description = "SP03_PLEASE_REUSE_ME_I_WAS_BLANK_ON_ACCIDENT_2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "397", Description = "Spawn_Arrival", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65622", Description = "Spawn_ArtsPark", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65619", Description = "Spawn_ArtsQuad", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65546", Description = "Spawn_ArtsUniversityShell", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65556", Description = "Spawn_ArtsUniversityShell_Shell1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65557", Description = "Spawn_ArtsUniversityShell_Shell2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47133", Description = "Spawn_BattleHelper", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2057", Description = "Spawn_Fireplace", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "987", Description = "Spawn_Grim_Reaper", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57409", Description = "Spawn_Lighthouse", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1935", Description = "Spawn_LighthouseArrival", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2223", Description = "Spawn_MagicPortal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49182", Description = "Spawn_MagicPortal_Market", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49166", Description = "Spawn_Marketstall_Magic_Broom", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49171", Description = "Spawn_Marketstall_Magic_Potion", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49172", Description = "Spawn_Marketstall_Magic_Wand", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49158", Description = "Spawn_NightStalker", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57387", Description = "Spawn_PetCrate", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "400", Description = "Spawn_RearWalkby", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65620", Description = "Spawn_ScienceQuad", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65547", Description = "Spawn_ScienceUniversityShell", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65558", Description = "Spawn_ScienceUniversityShell_Shell1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65559", Description = "Spawn_ScienceUniversityShell_Shell2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65621", Description = "Spawn_SecretSociety", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1933", Description = "Spawn_ShellArrival", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2039", Description = "Spawn_SkeletonArrival", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "399", Description = "Spawn_VisitorArrival", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "398", Description = "Spawn_Walkby", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2234", Description = "Spawn_Walkby_SportsShellEP08", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47132", Description = "Spawn_Zombie", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "127", Description = "Special_Nude", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49170", Description = "Spell_Magic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55330", Description = "Style_ArtsQuarter", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1495", Description = "Style_Bohemian", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1593", Description = "Style_Business", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2433", Description = "Style_CAS_Branded_MAC", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "239", Description = "Style_Classics", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "985", Description = "Style_Country", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55331", Description = "Style_FashionDistrict", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55348", Description = "Style_Festival_Blossom", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1623", Description = "Style_Festival_Dark", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1624", Description = "Style_Festival_Food", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1622", Description = "Style_Festival_Light", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1621", Description = "Style_Festival_Nerd", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1620", Description = "Style_Festival_Romance", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "248", Description = "Style_FormalModern", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "249", Description = "Style_FormalTrendy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "8197", Description = "Style_Frankenstein", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "238", Description = "Style_GenCitySleek", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "240", Description = "Style_GenContemporaryBasic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "241", Description = "Style_GenContemporaryDesigner", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "243", Description = "Style_GenOutdoorExplorer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "244", Description = "Style_GenPartyTrendy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "245", Description = "Style_GenPolished", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "246", Description = "Style_GenPreppy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "247", Description = "Style_GenRomantic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "237", Description = "Style_GenSummer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10265", Description = "Style_Glamping", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "289", Description = "Style_GothRockPunk", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "986", Description = "Style_Hipster", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63517", Description = "Style_IslandElemental", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63495", Description = "Style_Islander", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2036", Description = "Style_Jungle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "8196", Description = "Style_Pirate", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65597", Description = "Style_ProfessorNPC_Good", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65596", Description = "Style_ProfessorNPC_Grumpy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65595", Description = "Style_ProfessorNPC_Hip", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65598", Description = "Style_ProfessorNPC_Smart", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2066", Description = "Style_Seasonal_Fall", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2067", Description = "Style_Seasonal_Spring", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2068", Description = "Style_Seasonal_Summer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2065", Description = "Style_Seasonal_Winter", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55332", Description = "Style_SpiceMarket", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1592", Description = "Style_Street", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1681", Description = "Style_VampireArchetype_Dracula", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1682", Description = "Style_VampireArchetype_Modern", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1680", Description = "Style_VampireArchetype_Nosferatu", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1684", Description = "Style_VampireArchetype_Punk", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1683", Description = "Style_VampireArchetype_Victorian", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "40966", Description = "Style_VampireWalkby_Modern", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "40964", Description = "Style_VampireWalkby_Nosferatu", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "40968", Description = "Style_VampireWalkby_Punk", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "40967", Description = "Style_VampireWalkby_Victorian", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "8195", Description = "Style_Witch", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57350", Description = "Tail_Long", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57351", Description = "Tail_Ring", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57354", Description = "Tail_Saber", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57352", Description = "Tail_Screw", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57353", Description = "Tail_Stub", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2169", Description = "TerrainManip_All", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1082", Description = "TerrainPaint_All", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "872", Description = "TerrainPaint_Dirt", Function = "Terrain Paint", Subfunction = "Dirt" },
            new S4CategoryTag(){ TypeID = "873", Description = "TerrainPaint_Grass", Function = "Terrain Paint", Subfunction = "Grass" },
            new S4CategoryTag(){ TypeID = "875", Description = "TerrainPaint_Misc", Function = "Terrain Paint", Subfunction = "Misc" },
            new S4CategoryTag(){ TypeID = "874", Description = "TerrainPaint_Stone", Function = "Terrain Paint", Subfunction = "Stone" },
            new S4CategoryTag(){ TypeID = "732", Description = "Tooltip_AmbienceAngry", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "733", Description = "Tooltip_AmbienceBored", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "734", Description = "Tooltip_AmbienceConfident", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "735", Description = "Tooltip_AmbienceEmbarrassed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "736", Description = "Tooltip_AmbienceEnergized", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "737", Description = "Tooltip_AmbienceFlirty", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "738", Description = "Tooltip_AmbienceFocused", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "739", Description = "Tooltip_AmbienceHappy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "740", Description = "Tooltip_AmbienceImaginative", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "741", Description = "Tooltip_AmbiencePlayful", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "742", Description = "Tooltip_AmbienceSad", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "743", Description = "Tooltip_AmbienceTense", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2396", Description = "Tooltip_BillsDecrease", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2395", Description = "Tooltip_BillsIncrease", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2238", Description = "Tooltip_ColumnHeightRestricted", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "706", Description = "Tooltip_CraftingQualityCarpentry", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "703", Description = "Tooltip_CraftingQualityCooking", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "704", Description = "Tooltip_CraftingQualityDrinks", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "705", Description = "Tooltip_CraftingQualityPainting", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67624", Description = "Tooltip_EcoFootprint_Negative", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67623", Description = "Tooltip_EcoFootprint_Positive", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2389", Description = "Tooltip_EnvironmentScoreNegative", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2390", Description = "Tooltip_EnvironmentScorePositive", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2422", Description = "Tooltip_EP09_EcoFootprint_Negative", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2421", Description = "Tooltip_EP09_EcoFootprint_Positive", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2392", Description = "Tooltip_HighFireResistance", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2394", Description = "Tooltip_HighWaterResistance", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2391", Description = "Tooltip_LowFireResistance", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2393", Description = "Tooltip_LowWaterResistance", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2027", Description = "Tooltip_MiscCatsOnly", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "783", Description = "Tooltip_MiscChildrenOnly", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "784", Description = "Tooltip_MiscComfort", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2026", Description = "Tooltip_MiscDogsOnly", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2025", Description = "Tooltip_MiscPetsOnly", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "907", Description = "Tooltip_MiscReliablility", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1667", Description = "Tooltip_MiscToddlerOnly", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "731", Description = "Tooltip_MiscUnbreakable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "747", Description = "Tooltip_MiscUncomfortable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "940", Description = "Tooltip_MiscUncomfortableForAdults", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "710", Description = "Tooltip_MoodReliefAngry", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "711", Description = "Tooltip_MoodReliefBored", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "712", Description = "Tooltip_MoodReliefEmbarrassed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "709", Description = "Tooltip_MoodReliefSad", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "707", Description = "Tooltip_MoodReliefStress", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "708", Description = "Tooltip_MoodReliefUncomfortable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "701", Description = "Tooltip_MotiveBladder", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "698", Description = "Tooltip_MotiveEnergy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "699", Description = "Tooltip_MotiveFun", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "702", Description = "Tooltip_MotiveHunger", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "697", Description = "Tooltip_MotiveHygiene", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "700", Description = "Tooltip_MotiveSocial", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2207", Description = "Tooltip_OffTheGrid", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2398", Description = "Tooltip_PowerConsumer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2397", Description = "Tooltip_PowerProducer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61637", Description = "Tooltip_SkillActing", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45110", Description = "Tooltip_SkillArchaeology", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "717", Description = "Tooltip_SkillBartending", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "729", Description = "Tooltip_SkillCharisma", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "726", Description = "Tooltip_SkillComedy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1670", Description = "Tooltip_SkillCommunication", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "713", Description = "Tooltip_SkillCooking", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "927", Description = "Tooltip_SkillCreativity", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24615", Description = "Tooltip_SkillDance", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24614", Description = "Tooltip_SkillDJ", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2023", Description = "Tooltip_SkillDogTraining", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "716", Description = "Tooltip_SkillFitness", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2115", Description = "Tooltip_SkillFlowerArranging", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "728", Description = "Tooltip_SkillGardening", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "727", Description = "Tooltip_SkillGuitar", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "719", Description = "Tooltip_SkillHandiness", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1669", Description = "Tooltip_SkillImagination", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "721", Description = "Tooltip_SkillLogic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "928", Description = "Tooltip_SkillMental", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "722", Description = "Tooltip_SkillMischief", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "929", Description = "Tooltip_SkillMotor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1668", Description = "Tooltip_SkillMovement", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "718", Description = "Tooltip_SkillPainting", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "724", Description = "Tooltip_SkillPiano", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "40978", Description = "Tooltip_SkillPipeOrgan", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1672", Description = "Tooltip_SkillPotty", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "715", Description = "Tooltip_SkillProgramming", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "8212", Description = "Tooltip_SkillPsychic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2269", Description = "Tooltip_SkillResearchDebate", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2270", Description = "Tooltip_SkillRobotics", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "720", Description = "Tooltip_SkillRocketScience", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55434", Description = "Tooltip_SkillSinging", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "930", Description = "Tooltip_SkillSocial", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1671", Description = "Tooltip_SkillThinking", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2024", Description = "Tooltip_SkillVet", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "714", Description = "Tooltip_SkillVideoGaming", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "725", Description = "Tooltip_SkillViolin", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18459", Description = "Tooltip_SkillWellness", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "730", Description = "Tooltip_SkillWoohoo", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "723", Description = "Tooltip_SkillWriting", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2400", Description = "Tooltip_WaterConsumer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2399", Description = "Tooltip_WaterProducer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1236", Description = "Top_Bikini", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "155", Description = "Top_Blouse", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "944", Description = "Top_Brassiere", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "395", Description = "Top_ButtonUps", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "295", Description = "Top_Jacket", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "943", Description = "Top_Polo", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "296", Description = "Top_ShirtTee", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "942", Description = "Top_SuitJacket", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "297", Description = "Top_Sweater", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "941", Description = "Top_Sweatshirt", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "360", Description = "Top_Tanktop", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "156", Description = "Top_Vest", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "235", Description = "TraitAchievement", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "657", Description = "TraitAge", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "753", Description = "TraitGroup_Emotional", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "754", Description = "TraitGroup_Hobbies", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "755", Description = "TraitGroup_Lifestyle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "756", Description = "TraitGroup_Social", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "234", Description = "TraitPersonality", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "236", Description = "TraitWalkstyle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55413", Description = "Uniform_Activist_CrimialJustice", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55414", Description = "Uniform_Activist_EconomicGrowth", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55415", Description = "Uniform_Activist_Environment", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55416", Description = "Uniform_Activist_GlobalPeace", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55417", Description = "Uniform_Activist_TaxReform", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61561", Description = "Uniform_ActorCareer_Commercial_Hospital_Actor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61562", Description = "Uniform_ActorCareer_Commercial_Hospital_CoStar", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61564", Description = "Uniform_ActorCareer_Commercial_HouseNice_Actor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61565", Description = "Uniform_ActorCareer_Commercial_HouseNice_CoStar", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61566", Description = "Uniform_ActorCareer_Commercial_Kids_Actor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61560", Description = "Uniform_ActorCareer_Commercial_Pirate_Actor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61563", Description = "Uniform_ActorCareer_Commercial_Western_Actor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61608", Description = "Uniform_ActorCareer_Movie_City_Actor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61452", Description = "Uniform_ActorCareer_Movie_City_CoStar", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61451", Description = "Uniform_ActorCareer_Movie_City_LoveInterest", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61594", Description = "Uniform_ActorCareer_Movie_Medieval_Actor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61596", Description = "Uniform_ActorCareer_Movie_Medieval_LoveInterest", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61595", Description = "Uniform_ActorCareer_Movie_Medieval_Villain", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61591", Description = "Uniform_ActorCareer_Movie_Pirate_Actor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61593", Description = "Uniform_ActorCareer_Movie_Pirate_LoveInterest", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61592", Description = "Uniform_ActorCareer_Movie_Pirate_Villain", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61603", Description = "Uniform_ActorCareer_Movie_SuperHero_Actor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61605", Description = "Uniform_ActorCareer_Movie_SuperHero_LoveInterest", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61604", Description = "Uniform_ActorCareer_Movie_SuperHero_Villain", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61600", Description = "Uniform_ActorCareer_Movie_Victorian_Actor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61602", Description = "Uniform_ActorCareer_Movie_Victorian_CoStar", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61601", Description = "Uniform_ActorCareer_Movie_Victorian_LoveInterest", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61597", Description = "Uniform_ActorCareer_Movie_Western_Actor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61599", Description = "Uniform_ActorCareer_Movie_Western_Alien", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61598", Description = "Uniform_ActorCareer_Movie_Western_Creature", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61577", Description = "Uniform_ActorCareer_TVHigh_Apocalypse_Actor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61578", Description = "Uniform_ActorCareer_TVHigh_Apocalypse_CoStar", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61579", Description = "Uniform_ActorCareer_TVHigh_Apocalypse_Villain", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61580", Description = "Uniform_ActorCareer_TVHigh_Hospital_Actor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61582", Description = "Uniform_ActorCareer_TVHigh_Hospital_CoStar", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61581", Description = "Uniform_ActorCareer_TVHigh_Hospital_LoveInterest", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61588", Description = "Uniform_ActorCareer_TVHigh_Police_Actor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61590", Description = "Uniform_ActorCareer_TVHigh_Police_CoStar", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61589", Description = "Uniform_ActorCareer_TVHigh_Police_Villain", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61585", Description = "Uniform_ActorCareer_TVHigh_Victorian_Actor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61587", Description = "Uniform_ActorCareer_TVHigh_Victorian_CoStar", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61586", Description = "Uniform_ActorCareer_TVHigh_Victorian_LoveInterest", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61583", Description = "Uniform_ActorCareer_TVHigh_Western_Actor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61584", Description = "Uniform_ActorCareer_TVHigh_Western_Villain", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61574", Description = "Uniform_ActorCareer_TVLow_HouseLow_Actor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61575", Description = "Uniform_ActorCareer_TVLow_HouseLow_CoStar", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61570", Description = "Uniform_ActorCareer_TVLow_HouseNice_Actor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61571", Description = "Uniform_ActorCareer_TVLow_HouseNice_CoStar", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61576", Description = "Uniform_ActorCareer_TVLow_Kids_Actor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61567", Description = "Uniform_ActorCareer_TVLow_Pirate_Actor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61569", Description = "Uniform_ActorCareer_TVLow_Pirate_CoStar", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61568", Description = "Uniform_ActorCareer_TVLow_Pirate_LoveInterest", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61572", Description = "Uniform_ActorCareer_TVLow_Western_Actor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61573", Description = "Uniform_ActorCareer_TVLow_Western_CoStar", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12336", Description = "Uniform_Arrested", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55395", Description = "Uniform_ArtCritic_ShowFormal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55357", Description = "Uniform_ArtsCenterPainter", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55302", Description = "Uniform_AstronautStatueGold", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55354", Description = "Uniform_AstronautStatueSilver", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "614", Description = "Uniform_AstronautSuit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1262", Description = "Uniform_AthleticCheerleader", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1263", Description = "Uniform_AthleticLifter", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1266", Description = "Uniform_AthleticMajorLeaguer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1264", Description = "Uniform_AthleticMascot", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1267", Description = "Uniform_AthleticMinorLeaguer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1265", Description = "Uniform_AthleticTrackSuit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "887", Description = "Uniform_Babysitter", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61642", Description = "Uniform_BackgroundActor_Costume1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61643", Description = "Uniform_BackgroundActor_Costume2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61644", Description = "Uniform_BackgroundActor_Costume3", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61645", Description = "Uniform_BackgroundActor_Costume4", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61646", Description = "Uniform_BackgroundActor_Costume5", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "884", Description = "Uniform_Barista", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "621", Description = "Uniform_Bartender", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45090", Description = "Uniform_Bartender_Jungle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10258", Description = "Uniform_BearSuit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59466", Description = "Uniform_Beekeeping_Suit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2244", Description = "Uniform_BigHead", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65618", Description = "Uniform_BikeHelmet", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "682", Description = "Uniform_BlackAndWhiteParty", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "627", Description = "Uniform_BlackTurtleneck", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "38918", Description = "Uniform_Bowling_NPC", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "38914", Description = "Uniform_Bowling_Team1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "38915", Description = "Uniform_Bowling_Team2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "38916", Description = "Uniform_Bowling_Team3", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "38917", Description = "Uniform_Bowling_Team4", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "38924", Description = "Uniform_BowlingGloves", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "38923", Description = "Uniform_BowlingShoes", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1269", Description = "Uniform_BusinessCheapSuit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1270", Description = "Uniform_BusinessDecentSuit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1271", Description = "Uniform_BusinessExpensiveSuit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1268", Description = "Uniform_BusinessOfficeWorker", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "36869", Description = "Uniform_Butler", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61450", Description = "Uniform_CameraOperator", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59480", Description = "Uniform_career_Gardener_Botanist", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59481", Description = "Uniform_career_Gardener_Florist", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59479", Description = "Uniform_career_Gardener_Main", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "620", Description = "Uniform_Chef", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "43027", Description = "Uniform_ChildhoodPhase_Bear", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67627", Description = "Uniform_CivicInspector", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67641", Description = "Uniform_CivilDesigner_CivicPlanner", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67640", Description = "Uniform_CivilDesigner_GreenTechnician", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67639", Description = "Uniform_CivilDesigner_Main", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "680", Description = "Uniform_Clown", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "618", Description = "Uniform_ConcertOutfit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63523", Description = "Uniform_Conservationist_EnvironmentalManager", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63522", Description = "Uniform_Conservationist_Main", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63524", Description = "Uniform_Conservationist_MarineBiologist", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47130", Description = "Uniform_Conspiracist", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "619", Description = "Uniform_Cook", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1486", Description = "Uniform_Costume_AaylaSecura", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1700", Description = "Uniform_Costume_AlienHunter", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2113", Description = "Uniform_Costume_AnimalHood", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59475", Description = "Uniform_Costume_AnimalHoodie", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1480", Description = "Uniform_Costume_AstronautOrange", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1466", Description = "Uniform_Costume_AstronautWhite", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1475", Description = "Uniform_Costume_BobaFett", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1631", Description = "Uniform_Costume_CartoonPlumbers", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1476", Description = "Uniform_Costume_CheerleaderGreen", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1481", Description = "Uniform_Costume_ClownPink", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1467", Description = "Uniform_Costume_ClownYellow", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1632", Description = "Uniform_Costume_ColorfulAnimals", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1474", Description = "Uniform_Costume_DarthMaul", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1473", Description = "Uniform_Costume_DarthVader", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22530", Description = "Uniform_Costume_Fairy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22547", Description = "Uniform_Costume_FairyBlue", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22546", Description = "Uniform_Costume_FairyGreen", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22548", Description = "Uniform_Costume_FairyPurple", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59473", Description = "Uniform_Costume_HolidayHelper", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1468", Description = "Uniform_Costume_HotDogRed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22532", Description = "Uniform_Costume_Legonaire", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1485", Description = "Uniform_Costume_Leia", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22531", Description = "Uniform_Costume_Llama", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22549", Description = "Uniform_Costume_LlamaGirlPurple", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22544", Description = "Uniform_Costume_LlamaManBlack", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1472", Description = "Uniform_Costume_LukeSkywalker", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1483", Description = "Uniform_Costume_MaidBlack", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1470", Description = "Uniform_Costume_MaidBlue", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1479", Description = "Uniform_Costume_MailmanBlue", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1469", Description = "Uniform_Costume_MascotBlueBlack", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1482", Description = "Uniform_Costume_MascotWhite", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1699", Description = "Uniform_Costume_Monster", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22533", Description = "Uniform_Costume_Ninja", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22543", Description = "Uniform_Costume_NinjaRed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22534", Description = "Uniform_Costume_Pirate", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22559", Description = "Uniform_Costume_PirateBrown", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22542", Description = "Uniform_Costume_PirateNavy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22550", Description = "Uniform_Costume_PirateRed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22566", Description = "Uniform_Costume_PirateWhite", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1471", Description = "Uniform_Costume_PizzaOrange", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1484", Description = "Uniform_Costume_PizzaRed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22537", Description = "Uniform_Costume_Princess", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22556", Description = "Uniform_Costume_PrincessBlue", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22557", Description = "Uniform_Costume_PrincessGold", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22558", Description = "Uniform_Costume_PrincessPink", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22564", Description = "Uniform_Costume_PumpkinBrown", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22535", Description = "Uniform_Costume_PumpkinMan", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22563", Description = "Uniform_Costume_PumpkinNavy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22565", Description = "Uniform_Costume_PumpkinPlum", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2225", Description = "Uniform_Costume_RoboHat", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1489", Description = "Uniform_Costume_SausageGray", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22538", Description = "Uniform_Costume_SchoolGirl", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22539", Description = "Uniform_Costume_Skeleton", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22561", Description = "Uniform_Costume_SkeletonGreen", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22562", Description = "Uniform_Costume_SkeletonOrange", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22560", Description = "Uniform_Costume_SkeletonWhite", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1488", Description = "Uniform_Costume_SmugglerBrown", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1477", Description = "Uniform_Costume_SmugglerTan", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1487", Description = "Uniform_Costume_SpaceRangerBlack", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1478", Description = "Uniform_Costume_SpaceRangerBlue", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22551", Description = "Uniform_Costume_SpartanBrown", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22545", Description = "Uniform_Costume_SpartanGold", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59474", Description = "Uniform_Costume_TreeFir", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22536", Description = "Uniform_Costume_Witch", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22552", Description = "Uniform_Costume_WitchBlack", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22553", Description = "Uniform_Costume_WitchGreen", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22554", Description = "Uniform_Costume_WitchOrange", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1490", Description = "Uniform_Costume_Yoda", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "22555", Description = "Uniform_Costume_ZombieBlue", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55433", Description = "Uniform_CowboyStatueGold", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "623", Description = "Uniform_CrimeBoss", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "622", Description = "Uniform_CrimeLordHat", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1568", Description = "Uniform_DayoftheDead_Walkby", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1569", Description = "Uniform_DayoftheDead_Walkby_Female", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65590", Description = "Uniform_Debate_Judge", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12334", Description = "Uniform_Detective", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61449", Description = "Uniform_Director", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63515", Description = "Uniform_Diver", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24584", Description = "Uniform_DJ_High", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24583", Description = "Uniform_DJ_Low", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12340", Description = "Uniform_Doctor_high", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12339", Description = "Uniform_Doctor_low", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61639", Description = "Uniform_DramaClub", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67626", Description = "Uniform_EcoInspector", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65552", Description = "Uniform_Education", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65553", Description = "Uniform_Education_Admin", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65554", Description = "Uniform_Education_Prof", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "625", Description = "Uniform_ElbowPatchJacket", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12385", Description = "Uniform_EP01_Alien", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12357", Description = "Uniform_EP01_Doctor_mid", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12426", Description = "Uniform_EP01_PoliceChief", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12412", Description = "Uniform_EP01_RetailEmployee", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12381", Description = "Uniform_EP01_Scientist_AlienHunter", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12349", Description = "Uniform_EP01_Scientist_high", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12350", Description = "Uniform_EP01_Scientist_low", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12359", Description = "Uniform_EP01_Scientist_mid", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12399", Description = "Uniform_EP01_Scientist_veryHigh", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12401", Description = "Uniform_EP01_SuspectBlackHair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12367", Description = "Uniform_EP01_SuspectBlondeHair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12408", Description = "Uniform_EP01_SuspectBottomPants", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12411", Description = "Uniform_EP01_SuspectBottomShorts", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12409", Description = "Uniform_EP01_SuspectBottomSkirt", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12410", Description = "Uniform_EP01_SuspectBottomSlacks", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12402", Description = "Uniform_EP01_SuspectBrownHair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12432", Description = "Uniform_EP01_SuspectGreyHair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12366", Description = "Uniform_EP01_SuspectRedHair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12406", Description = "Uniform_EP01_SuspectTopBlouse", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12404", Description = "Uniform_EP01_SuspectTopJacket", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12405", Description = "Uniform_EP01_SuspectTopLongSleeve", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12403", Description = "Uniform_EP01_SuspectTopShortSleeve", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12407", Description = "Uniform_EP01_SuspectTopTank", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63525", Description = "Uniform_EP07_Vendor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65601", Description = "Uniform_ESportsPlayer_Arts", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65602", Description = "Uniform_ESportsPlayer_Science", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "8209", Description = "Uniform_Fairy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "883", Description = "Uniform_FastFood", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2071", Description = "Uniform_FatherWinter", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2086", Description = "Uniform_FatherWinter_Summer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55350", Description = "Uniform_Festival_Blossom_Shirt", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55397", Description = "Uniform_Festival_Food_CurryContest_Shirt", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55351", Description = "Uniform_Festival_Food_Shirt", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55352", Description = "Uniform_Festival_Lamp_Shirt", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55421", Description = "Uniform_Festival_LlamaBlue", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55423", Description = "Uniform_Festival_LlamaGold", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55424", Description = "Uniform_Festival_LlamaSilver", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55422", Description = "Uniform_Festival_LlamaYellow", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55353", Description = "Uniform_Festival_Logic_Shirt", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2089", Description = "Uniform_FestiveSpirit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2426", Description = "Uniform_Firefighter", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59458", Description = "Uniform_FlowerBunny", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55396", Description = "Uniform_FoodCritic_RestaurantCasual", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10266", Description = "Uniform_ForestRanger", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "8198", Description = "Uniform_FortuneTeller", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "8201", Description = "Uniform_Frankenstein", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24579", Description = "Uniform_GAMESCOM_ClosetFail", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24580", Description = "Uniform_GAMESCOM_ClosetSucceed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10291", Description = "Uniform_GP01cfTankLace", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10288", Description = "Uniform_GP01cuPocketZip", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10290", Description = "Uniform_GP01cuTeeLongShirtPants", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10287", Description = "Uniform_GP01cuTeeLongShirtShorts", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10289", Description = "Uniform_GP01cuVestDown", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10292", Description = "Uniform_GP01Walkbys1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10293", Description = "Uniform_GP01Walkbys2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10294", Description = "Uniform_GP01Walkbys3", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10296", Description = "Uniform_GP01Walkbys5", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10297", Description = "Uniform_GP01Walkbys6", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10295", Description = "Uniform_GP01Walksbys4", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10279", Description = "Uniform_GP01yfJacketFleece", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10276", Description = "Uniform_GP01yfLayers", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10277", Description = "Uniform_GP01yfLayersHat", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10281", Description = "Uniform_GP01yfTeeTied", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10278", Description = "Uniform_GP01yfVestFlannel", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10280", Description = "Uniform_GP01yfVestTee", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10285", Description = "Uniform_GP01ymFingerShirt", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10283", Description = "Uniform_GP01ymTank", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10284", Description = "Uniform_GP01ymThickLayers", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10282", Description = "Uniform_GP01ymVestCarabiner", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10286", Description = "Uniform_GP01ymVestFleece", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "316", Description = "Uniform_GrimReaper", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "366", Description = "Uniform_GrimReaperHelper", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "624", Description = "Uniform_Hacker", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61453", Description = "Uniform_HairMakeUpChair_Stylist", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47127", Description = "Uniform_HazmatSuit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47128", Description = "Uniform_HazmatSuit_WithFilter", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "10257", Description = "Uniform_Hermit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1549", Description = "Uniform_HiredNanny", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "681", Description = "Uniform_HotDog", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "626", Description = "Uniform_InvestigativeJournalist", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63516", Description = "Uniform_IslandElemental", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63513", Description = "Uniform_IslandLocal", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63514", Description = "Uniform_IslandLocal_FlowerMusic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45102", Description = "Uniform_Jungle_Vendor1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45103", Description = "Uniform_Jungle_Vendor2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45104", Description = "Uniform_Jungle_Vendor3", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24610", Description = "Uniform_KnightSuit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65628", Description = "Uniform_LawCareer_Judge", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65627", Description = "Uniform_LawCareer_Main", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65630", Description = "Uniform_LawCareer_MainHigh", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65629", Description = "Uniform_LawCareer_PA", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63502", Description = "Uniform_Lifeguard", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55358", Description = "Uniform_LoveGuru", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "262", Description = "Uniform_Maid", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "636", Description = "Uniform_MaidDEPRECATED", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "341", Description = "Uniform_Mailman", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "613", Description = "Uniform_MaintainenceWorker", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "885", Description = "Uniform_ManualLabor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65588", Description = "Uniform_Mascot_Alt_Arts", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65589", Description = "Uniform_Mascot_Alt_Science", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65586", Description = "Uniform_Mascot_Arts", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65587", Description = "Uniform_Mascot_Science", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18446", Description = "Uniform_MassageTherapist", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18450", Description = "Uniform_MassageTowel", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "867", Description = "Uniform_MasterFisherman", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "868", Description = "Uniform_MasterGardener", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47123", Description = "Uniform_Military_Covert_Headset", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47121", Description = "Uniform_Military_Covert_Suit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47122", Description = "Uniform_Military_Covert_Sunglasses", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47111", Description = "Uniform_Military_Main_Level_01", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47112", Description = "Uniform_Military_Main_Level_02", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47113", Description = "Uniform_Military_Main_Level_03", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47114", Description = "Uniform_Military_Main_Level_04", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47115", Description = "Uniform_Military_Main_Level_05", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47116", Description = "Uniform_Military_Officer_Level_01", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47117", Description = "Uniform_Military_Officer_Level_02", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47118", Description = "Uniform_Military_Officer_Level_03", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47119", Description = "Uniform_Military_Officer_Level_04", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47120", Description = "Uniform_Military_Officer_Level_05", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "8205", Description = "Uniform_NInja", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "607", Description = "Uniform_OfficeWorker", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "659", Description = "Uniform_Oracle", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65617", Description = "Uniform_Organization_ArtSociety_Member", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65616", Description = "Uniform_Organization_ArtSociety_Model", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65635", Description = "Uniform_Organization_Debate", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65642", Description = "Uniform_Organization_DebateJudge", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65643", Description = "Uniform_Organization_DebateShowdown", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65654", Description = "Uniform_Organization_DebateShowdownFoxbury", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65636", Description = "Uniform_Organization_Honor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65637", Description = "Uniform_Organization_Party", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65638", Description = "Uniform_Organization_Prank", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65634", Description = "Uniform_Organization_Robotics", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "629", Description = "Uniform_Painter", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61606", Description = "Uniform_Paparazzi", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "631", Description = "Uniform_Parts_Bride", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "630", Description = "Uniform_Parts_Groom", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "633", Description = "Uniform_Parts_Librarian", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "634", Description = "Uniform_Parts_OfficeWorker", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "635", Description = "Uniform_Parts_ParkSleeper", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63520", Description = "Uniform_PartTime_Fisherman", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "632", Description = "Uniform_Party_PartyHats", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12338", Description = "Uniform_Patient", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "8203", Description = "Uniform_Pirate", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "637", Description = "Uniform_PizzaDelivery", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "12335", Description = "Uniform_PoliceOfficer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55418", Description = "Uniform_Politician_HighLevel", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55420", Description = "Uniform_Politician_LowLevel", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55419", Description = "Uniform_Politician_MediumLevel", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "8208", Description = "Uniform_Princess", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61628", Description = "Uniform_Producer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65647", Description = "Uniform_ProfessorNPC_Good", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65646", Description = "Uniform_ProfessorNPC_Grumpy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65645", Description = "Uniform_ProfessorNPC_Hip", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65644", Description = "Uniform_ProfessorNPC_Smart", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "628", Description = "Uniform_ProGamer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "8206", Description = "Uniform_Pumpkin", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55372", Description = "Uniform_Raccoon", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18460", Description = "Uniform_Reflexologist", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1491", Description = "Uniform_Repair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65633", Description = "Uniform_RepoPerson", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "26644", Description = "Uniform_RestaurantCritic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "886", Description = "Uniform_Retail", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18437", Description = "Uniform_Robe", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "8207", Description = "Uniform_SchoolGirl", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59464", Description = "Uniform_Scout_Basic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59465", Description = "Uniform_Scout_Expert", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65566", Description = "Uniform_SecretSociety_Level1", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65567", Description = "Uniform_SecretSociety_Level2", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65568", Description = "Uniform_SecretSociety_Level3", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59471", Description = "Uniform_Skating_Generic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59433", Description = "Uniform_Skating_Ice", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59442", Description = "Uniform_Skating_Pro", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "59434", Description = "Uniform_Skating_Roller", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "8204", Description = "Uniform_Skeleton", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "45088", Description = "Uniform_Skeleton_GP06", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "616", Description = "Uniform_Smuggler", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65599", Description = "Uniform_SoccerPlayer_Arts", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65600", Description = "Uniform_SoccerPlayer_Science", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "615", Description = "Uniform_SpaceRanger", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "8211", Description = "Uniform_Spartan", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49177", Description = "Uniform_Spellcaster_Edgy", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49176", Description = "Uniform_Spellcaster_Fairytale", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49178", Description = "Uniform_Spellcaster_Sage", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49180", Description = "Uniform_Spellcaster_Sage_Mischief", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49179", Description = "Uniform_Spellcaster_Sage_Practical", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49181", Description = "Uniform_Spellcaster_Sage_Untamed", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49175", Description = "Uniform_Spellcaster_StreetModern", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "49174", Description = "Uniform_Spellcaster_Vintage", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65604", Description = "Uniform_SportsFan_Arts", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65605", Description = "Uniform_SportsFan_Science", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47109", Description = "Uniform_Stalls_CurioShop_Hat", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47110", Description = "Uniform_Stalls_CurioShop_Shirt", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47108", Description = "Uniform_Stalls_CurioShop_Vendor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55429", Description = "Uniform_Stalls_FoodFestival", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55428", Description = "Uniform_Stalls_Generic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1937", Description = "Uniform_Stalls_GenericMarketStalls", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55430", Description = "Uniform_Stalls_LampFestival", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55432", Description = "Uniform_Stalls_NerdFestival", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1986", Description = "Uniform_Stalls_PetWorld", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55431", Description = "Uniform_Stalls_RomanceFestival", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "47140", Description = "Uniform_StrangervilleScientist", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "608", Description = "Uniform_Suit", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "617", Description = "Uniform_Suit_Leisure", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "610", Description = "Uniform_SuperTuxedo", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "612", Description = "Uniform_TactialTurtleneck", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "760", Description = "Uniform_Teenager", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1673", Description = "Uniform_Toddler_DiaperOnly", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55306", Description = "Uniform_Tourist", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2166", Description = "Uniform_Tourist_Basegame", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1440", Description = "Uniform_Towel", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1503", Description = "Uniform_TragicClown", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "63521", Description = "Uniform_TurtleFanatic", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "609", Description = "Uniform_Tuxedo", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65610", Description = "Uniform_University_Graduation_Arts", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65611", Description = "Uniform_University_Graduation_Arts_NoCap", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65612", Description = "Uniform_University_Graduation_Science", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65613", Description = "Uniform_University_Graduation_Science_NoCap", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65578", Description = "Uniform_UniversityKiosk_BottomAH", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65579", Description = "Uniform_UniversityKiosk_BottomST", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65580", Description = "Uniform_UniversityKiosk_HatAH", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65581", Description = "Uniform_UniversityKiosk_HatST", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65576", Description = "Uniform_UniversityKiosk_TopAH", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65577", Description = "Uniform_UniversityKiosk_TopST", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65555", Description = "Uniform_UniversityStudent", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65584", Description = "Uniform_UniversityStudent_Arts", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "65585", Description = "Uniform_UniversityStudent_Science", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "57398", Description = "Uniform_Vet", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61629", Description = "Uniform_VFXMachine_Operator", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "611", Description = "Uniform_Villain", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61478", Description = "Uniform_VIPRope_Bouncer", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "61466", Description = "Uniform_WardrobePedestal_Stylist", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "67642", Description = "Uniform_WasteManager", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "55307", Description = "Uniform_Weirdo", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "24603", Description = "Uniform_Windenburg_Barista", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "8202", Description = "Uniform_Witch", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18445", Description = "Uniform_YogaInstructor", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "2746", Description = "BuyCat_Venue_Wedding", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "598", Description = "Venue_Object_Bench", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "961", Description = "Venue_Object_Chair", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "601", Description = "Venue_Object_Exercise", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "1443", Description = "Venue_Object_Locker", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "597", Description = "Venue_Object_Microphone", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "599", Description = "Venue_Object_MonkeyBars", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "595", Description = "Venue_Object_Painting", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "602", Description = "Venue_Object_PatioTable", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "600", Description = "Venue_Object_Playground", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "18443", Description = "Venue_Object_Relaxation", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "596", Description = "Venue_Object_Sculpture", Function = "", Subfunction = "" },
            new S4CategoryTag(){ TypeID = "412", Description = "WallPattern_Masonry", Function = "Wall", Subfunction = "Masonry" },
            new S4CategoryTag(){ TypeID = "415", Description = "WallPattern_Misc", Function = "Wall", Subfunction = "Misc" },
            new S4CategoryTag(){ TypeID = "408", Description = "WallPattern_Paint", Function = "Wall", Subfunction = "Paint" },
            new S4CategoryTag(){ TypeID = "411", Description = "WallPattern_Paneling", Function = "Wall", Subfunction = "Paneling" },
            new S4CategoryTag(){ TypeID = "413", Description = "WallPattern_RockAndStone", Function = "Wall", Subfunction = "Rock & Stone" },
            new S4CategoryTag(){ TypeID = "414", Description = "WallPattern_Siding", Function = "Wall", Subfunction = "Siding" },
            new S4CategoryTag(){ TypeID = "410", Description = "WallPattern_Tile", Function = "Wall", Subfunction = "Tile" },
            new S4CategoryTag(){ TypeID = "409", Description = "WallPattern_Wallpaper", Function = "Wall", Subfunction = "Wallpaper" },
            new S4CategoryTag(){ TypeID = "1985", Description = "WorldLog_NotInteractive", Function = "", Subfunction = "" }
        };

        public static List<SimsOverride> Overrides = new() {

        };

        public static List<string> Flags = new(){"DefaultForBodyType","DefaultThumbnailPart","AllowForCASRandom","ShowInUI","ShowInSimInfoPanel","ShowInCasDemo","AllowForLiveRandom","DisableForOppositeGender","DisableForOppositeFrame","DefaultForBodyTypeMale","DefaultForBodyTypeFemale","Unk","Unk","Unk","Unk","Unk"};

        public Sims4ScanData SearchS4Package(FileInfo package, Sims4Instance instance){            
            
            //locations
            long entrycountloc = 36;
            long indexRecordPositionloc = 64;

            Sims4ScanData thisPackage = new();          

            //Lists 
            
            List<TagsList> itemtags = new();
            List<string> allFlags = new();      
            List<string> allInstanceIDs = new();            
            List<string> objdentries;
            List<int> objdpositions;   
            List<IndexEntry> entries = new();
            IndexEntry MergedManifest = new();
            List<EntryLocations> fileHas = new();
            ArrayList linkData = new();
            List<IndexEntry> indexData = new();

            //create readers  
            //FileStream filePackage = new FileStream(package.FullName, FileMode.Open, FileAccess.Read);
            MemoryStream msPackage = ByteReaders.ReadBytesFromFile(package.FullName, (int)package.Length);
            BinaryReader readFile = new BinaryReader(msPackage);
            
            //log opening file
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Scanning package {0}", package.Name));
            
            readFile.BaseStream.Position = 0;

            readFile.BaseStream.Position = entrycountloc;

            uint entrycount = readFile.ReadUInt32();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Entry Count: {0}", entrycount.ToString()));
            
            //record position low
            uint indexRecordPositionLow = readFile.ReadUInt32();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("IndexRecordPositionLow: {0}", indexRecordPositionLow.ToString()));
            
            //index record size
            uint indexRecordSize = readFile.ReadUInt32();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("IndexRecordSize: {0}", indexRecordSize.ToString()));
            
            readFile.BaseStream.Position = indexRecordPositionloc;

            ulong indexRecordPosition = readFile.ReadUInt64();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Index Record Position: {0}", indexRecordPosition.ToString()));
            
                        
            byte[] headersize = new byte[96];
            long here = 100;
            long movedto = 0;

            /*            //dbpf
            test = Encoding.ASCII.GetString(readFile.ReadBytes(4));
            log.MakeLog("DBPF: " + test, true);
            
            //major
            uint testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Major :" + test, true);
            
            //minor
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Minor : " + test, true);
            
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unknown : " + test, true);
            
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unknown : " + test, true);
            
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unknown : " + test, true);
            
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Created : " + test, true);
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Modified : " + test, true);
            
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Index Major : " + test, true);
            
            //entrycount
            uint entrycount = readFile.ReadUInt32();
            test = entrycount.ToString();
            log.MakeLog("Entry Count: " + test, true);
            
            //record position low
            uint indexRecordPositionLow = readFile.ReadUInt32();
            test = indexRecordPositionLow.ToString();
            log.MakeLog("indexRecordPositionLow: " + test, true);
            
            //index record size
            uint indexRecordSize = readFile.ReadUInt32();
            test = indexRecordSize.ToString();
            log.MakeLog("indexRecordSize: " + test, true);
            //unused
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unused Trash Index offset: " + test, true);
            
            //unused
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unused Trash Index size: " + test, true);
            
            //unused
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unused Index Minor Version: " + test, true);
            
            //unused but 3 for historical reasons
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unused, 3 for historical reasons: " + test, true);
            
            ulong indexRecordPosition = readFile.ReadUInt64();
            test = indexRecordPosition.ToString();
            log.MakeLog("Inded Record Position: " + test, true);
            //unused
            testint = readFile.ReadUInt32();
            test = testint.ToString();
            log.MakeLog("Unused Unknown:" + test, true);
            
            //unused six bytes
            test = Encoding.ASCII.GetString(readFile.ReadBytes(24));
            log.MakeLog("Unused: " + test, true);*/

            if (indexRecordPosition != 0){
                long indexseek = (long)indexRecordPosition - headersize.Length;
                movedto = here + indexseek;
                readFile.BaseStream.Position = here + indexseek;                
            } else {
                movedto = here + indexRecordPositionLow;
                readFile.BaseStream.Position = here + indexRecordPositionLow;
            }

            readFile.BaseStream.Position = (long)indexRecordPosition;
            uint indextype = readFile.ReadUInt32();            
            
            long streamsize = readFile.BaseStream.Length;
            int indexbytes = ((int)entrycount * 32);
            long indexpos = 0;
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - Streamsize is {1}", package.Name, streamsize));
            if ((int)movedto + indexbytes != streamsize){
                int entriesfound = 0;
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - Streamsize does not match!", package.Name));
                readFile.BaseStream.Position = movedto - 400;
                List<long> entrylocs = new();
                uint item;
                
                while(readFile.BaseStream.Length > readFile.BaseStream.Position){                    
                    item = readFile.ReadUInt32();
                    //
                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Read byte at {0}: {1}", readFile.BaseStream.Position, item.ToString("X8")));
                    EntryType type = EntryTypes.Where(x => x.TypeID == item.ToString("X8")).First();

                    if (type != null){
                        IndexEntry holderEntry = new IndexEntry();
                        holderEntry.TypeID = item.ToString("X8");
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1} - Index Entry TypeID: {2}", package.Name, entriesfound, holderEntry.TypeID));

                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Found entry{2} - {0} at location {1}!", item, readFile.BaseStream.Position, entriesfound));
                        entrylocs.Add(readFile.BaseStream.Position);
                        if (entriesfound == 0){
                            indexpos = readFile.BaseStream.Position - 4;
                        }
                        entriesfound++;
                        if (entriesfound == entrycount){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Found {0} entries. Breaking.", entrycount));
                            break;
                        }
                    }
                }


                readFile.BaseStream.Position = indexpos;
                entrycount = 0;
                foreach (int loc in entrylocs){
                    IndexEntry holderEntry = new IndexEntry(); 
                    readFile.BaseStream.Position = loc - 4;
                    if (indextype == 2){
                        holderEntry.TypeID = readFile.ReadUInt32().ToString("X8");
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1} - Index Entry TypeID: {2}", package.Name, entrycount, holderEntry.TypeID));
                        
                        
                        EntryType type = EntryTypes.Where(x => x.TypeID == holderEntry.TypeID).First();
                        if(type != null){                        
                            fileHas.Add(new() {TypeID = type.TypeID, Description = type.Tag, Location = (int)entrycount});
                            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("File {0} has {1} at location {2}.", package.Name, EntryTypes.Where(x => x.TypeID == holderEntry.TypeID).First().Description, (int)entrycount));
                        } else {
                            fileHas.Add(new() { TypeID = holderEntry.TypeID, Location = (int)entrycount});
                        }                            

                        string instanceid1 = (readFile.ReadUInt32() << 32).ToString("X8");
                        string instanceid2 = (readFile.ReadUInt32() << 32).ToString("X8");
                        holderEntry.InstanceID = string.Format("{0}{1}", instanceid1,instanceid2);
                        if (holderEntry.InstanceID != "0000000000000000"){
                            allInstanceIDs.Add(holderEntry.InstanceID);
                        }
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1} - InstanceID: {2}", package.Name, entrycount, holderEntry.InstanceID));
                        
                        holderEntry.uLocation = readFile.ReadUInt32();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1} - Index Entry Position: {2}", package.Name, entrycount, holderEntry.uLocation.ToString("X8")));

                        holderEntry.Filesize = readFile.ReadUInt32();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1} - File Size: {2}", package.Name, entrycount, holderEntry.Filesize.ToString("X8")));

                        holderEntry.Truesize = readFile.ReadUInt32();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1} - Mem Size: {2}", package.Name, entrycount, holderEntry.Truesize.ToString("X8")));

                        indexData.Add(holderEntry);
                        MergedManifest = holderEntry;

                        holderEntry = null;

                        entrycount++;
                    }
                }
            } else {
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Streamsize for {0} matches.", package.Name));
                long movedhere = readFile.BaseStream.Position;
                uint testpos = readFile.ReadUInt32();
                if (testpos != 0){
                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - Read first entry TypeID and it read as {1}, returning to read entries.", package.Name, testpos.ToString("X8")));
                    readFile.BaseStream.Position = movedto;
                } else if (testpos == 80000000) {
                    long moveback = movedhere - 4;
                    readFile.BaseStream.Position = moveback;
                } else {
                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - Read first entry TypeID and it read as {1}, moving forward.", package.Name, testpos.ToString("X8")));
                }                
                for (int i = 0; i < entrycount; i++){                    
                    IndexEntry holderEntry = new IndexEntry();                
                    holderEntry.TypeID = readFile.ReadUInt32().ToString("X8");
                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1} - Index Entry TypeID: {2}", package.Name, i, holderEntry.TypeID));
                    if (holderEntry.TypeID == "7FB6AD8A"){
                        thisPackage.Type = "Merged Package";
                        thisPackage.Merged = true;

                        EntryType type = EntryTypes.Where(x => x.TypeID == holderEntry.TypeID).First();
                        
                        if(type != null){                        
                            fileHas.Add(new() {TypeID = type.TypeID, Description = type.Tag, Location = i});
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("File {0} has {1} at location {2}.", package.Name, type.Tag, i));
                        } else {
                            fileHas.Add(new() { TypeID = holderEntry.TypeID, Location = i});
                        }

                        holderEntry.GroupID = readFile.ReadUInt32().ToString("X8");
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1} - Index Entry GroupID: {2}", package.Name, i, holderEntry.GroupID));
                        
                        string instanceid1 = (readFile.ReadUInt32() << 32).ToString("X8");
                        string instanceid2 = (readFile.ReadUInt32() << 32).ToString("X8");
                        holderEntry.InstanceID = string.Format("{0}{1}", instanceid1,instanceid2);
                        allInstanceIDs.Add(holderEntry.InstanceID);
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1} - InstanceID: {2}", package.Name, i, holderEntry.InstanceID));

                        uint testin = readFile.ReadUInt32();
                        holderEntry.uLocation = testin;
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1} - Position: {2}", package.Name, i, holderEntry.uLocation));

                        holderEntry.Filesize = readFile.ReadUInt32();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1} - File Size: {2}", package.Name, i, holderEntry.Filesize));

                        holderEntry.Truesize = readFile.ReadUInt32();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1} - Mem Size: {2}", package.Name, i, holderEntry.Truesize));

                        holderEntry.CompressionType = readFile.ReadUInt16().ToString("X4");
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1} - Compression Type: {2}", package.Name, i, holderEntry.CompressionType));

                        readFile.BaseStream.Position = readFile.BaseStream.Position + 2;

                        indexData.Add(holderEntry);

                        holderEntry = null;                        

                        /*thisPackage = MakeNoNulls(thisPackage);
                        GlobalVariables.AddPackages.Enqueue(thisPackage); 
                        log.MakeLog(string.Format("Package {0} is a merged package, and cannot be processed in this manner right now. Package will either need unmerging or to be sorted manually.", thisPackage.uLocation), false);

                        readFile.Dispose();
                        sw.Stop();
                        TimeSpan tss = sw.Elapsed;
                        string elapsedtimee = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                            tss.Hours, tss.Minutes, tss.Seconds,
                                            tss.Milliseconds / 10);
                        GlobalVariables.packagesRead++;
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Reading file {0} took {1}", package.Name, elapsedtimee));

                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Closing package # {0}/{1}: {2}", package.Name, GlobalVariables.PackageCount, packageinfo.Name));
                        return;*/

                    } else {
                        EntryType type = EntryTypes.Where(x => x.TypeID == holderEntry.TypeID).First();
                        
                    
                        if(type != null){
                            fileHas.Add(new() { TypeID = type.TypeID, Description = type.Tag, Location = i });
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("File {0} has {1} at location {2}.", package.Name, type.Tag, i));
                        } else {
                            fileHas.Add(new() { TypeID = holderEntry.TypeID, Location = i});
                        } 

                        holderEntry.GroupID = readFile.ReadUInt32().ToString("X8");
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1} - Index Entry GroupID: {2}", package.Name, i, holderEntry.GroupID));
                        
                        string instanceid1 = (readFile.ReadUInt32() << 32).ToString("X8");
                        string instanceid2 = (readFile.ReadUInt32() << 32).ToString("X8");
                        holderEntry.InstanceID = string.Format("{0}{1}", instanceid1,instanceid2);
                        allInstanceIDs.Add(holderEntry.InstanceID);
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1} - InstanceID: {2}", package.Name, i, holderEntry.InstanceID));

                        uint testin = readFile.ReadUInt32();
                        holderEntry.uLocation = testin;
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1} - Position: {2}", package.Name, i, holderEntry.uLocation));

                        holderEntry.Filesize = readFile.ReadUInt32();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1} - File Size: {2}", package.Name, i, holderEntry.Filesize));

                        holderEntry.Truesize = readFile.ReadUInt32();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1} - Mem Size: {2}", package.Name, i, holderEntry.Truesize));

                        holderEntry.CompressionType = readFile.ReadUInt16().ToString("X4");
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1} - Compression Type: {2}", package.Name, i, holderEntry.CompressionType));

                        readFile.BaseStream.Position = readFile.BaseStream.Position + 2;

                        indexData.Add(holderEntry);

                        holderEntry = null;
                    }

                    
                }
            }

            /*if (thisPackage.Merged == true){
                var entryspots = (from has in fileHas
                        where has.Name =="S4SM"
                        select has.uLocation).ToList();
                
                int loc = entryspots[0];

                long entrylength = indexData[loc + 1].uLocation - indexData[loc].uLocation;

                readFile.BaseStream.Position = MergedManifest.uLocation;
                //XmlTextReader xmlDoc = new XmlTextReader(new StringReader(Encoding.UTF8.GetString(Methods.ReadEntryBytes(readFile, (int)entrylength))));
                //BinaryReader decompbr = new BinaryReader(decomps);
                for (int i = 0; i < readFile.BaseStream.Length; i++){
                    log.MakeLog(readFile.ReadByte().ToString(), true);
                }

                //Stream decomps = S4Decryption.Decompress(Methods.ReadEntryBytes(readFile, (int)entrylength));

                //BinaryReader decompbr = new BinaryReader(decomps);
            }
            
            currently unable to read merged packages or unmerge them. workign on it though.
            
            */






            if (fileHas.Exists(x => x.Description == "CASP")){


                var entryspots = fileHas.Where(x => x.Description == "CASP").Select(x => x.Location).ToList();

                thisPackage.Recolor = true;

                int caspc = 0;
                foreach (int e in entryspots){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - Opening CASP #{1}", package.Name, caspc));
                    if (indexData[e].CompressionType == "5A42"){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Compression type is {2}.", package.Name, caspc, indexData[e].CompressionType));
                        if (indexData[e].uLocation > 0){
                            readFile.BaseStream.Position = indexData[e].uLocation;
                        } else {
                            readFile.BaseStream.Position = indexData[e].uLocation;
                        }
                        long entryEnd = indexData[e].uLocation + indexData[e].Truesize;
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Position: {2}", package.Name, caspc, indexData[e].uLocation));

                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Filesize: {2}", package.Name, caspc, indexData[e].Filesize));

                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Memsize: {2}", package.Name, caspc, indexData[e].Truesize));

                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Ends at: {2}", package.Name, caspc, entryEnd));

                        Stream decomps = Sims4Decryption.Decompress(ByteReaders.ReadEntryBytes(readFile, (int)indexData[e].Truesize));
                        
                        BinaryReader decompbr = new(decomps);

                        try { 
                            ProcessCASP pcasp = new(thisPackage, package.Name, decompbr, decomps, e, itemtags, allFlags); 
                            thisPackage = pcasp.scanData;
                            allFlags = pcasp.allFlags;
                            itemtags = pcasp.itemtags;
                        } 
                        catch (Exception ex) {
                            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}: Caught exception reading package: {1}. Stack trace: {2}", package.Name, ex.Message, ex.StackTrace));
                        }

                        
                        
                        decompbr.Dispose();
                        decomps.Dispose();

                    } else if (indexData[e].CompressionType == "0000"){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Compression type is {2}.", package.Name, caspc, indexData[e].CompressionType));
                        readFile.BaseStream.Position = indexData[e].uLocation;
                        long entryEnd = indexData[e].uLocation + indexData[e].Truesize;
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Position: {2}", package.Name, caspc, indexData[e].uLocation));

                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Filesize: {2}", package.Name, caspc, indexData[e].Filesize));

                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Memsize: {2}", package.Name, caspc, indexData[e].Truesize));

                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Ends at: {2}", package.Name, caspc, entryEnd));

                        ProcessCASP pcasp = new(thisPackage, package.Name, readFile, msPackage, e, itemtags, allFlags);

                        thisPackage = pcasp.scanData;
                        allFlags = pcasp.allFlags;
                        itemtags = pcasp.itemtags;

                    }
                    caspc++;                    
                }                     

            }

            


            if (fileHas.Exists(x => x.Description == "COBJ")){

                var entryspots = fileHas.Where(x => x.Description == "COBJ").Select(x => x.Location).ToList();

                int cobjc = 0;
                foreach (int e in entryspots){
                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - Opening COBJ #{1}", package.Name, cobjc));
                    if (indexData[e].CompressionType == "5A42"){                                
                        readFile.BaseStream.Position = indexData[e].uLocation;                  
                        int entryEnd = (int)readFile.BaseStream.Position + (int)indexData[e].Truesize;
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/COBJ{1} - Position: {2}", package.Name, cobjc, indexData[e].uLocation));

                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/COBJ{1} - File Size: {2}", package.Name, cobjc, indexData[e].Filesize));

                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/COBJ{1} - Memory Size: {2}", package.Name, cobjc, indexData[e].Truesize));

                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/COBJ{1} - Entry Ends At: {2}", package.Name, cobjc, entryEnd));
                        Stream decomps = Sims4Decryption.Decompress(ByteReaders.ReadEntryBytes(readFile, (int)indexData[e].Truesize));
                        
                        BinaryReader decompbr = new BinaryReader(decomps);  

                        ProcessCOBJ pcobj = new(thisPackage, decompbr, package.Name, e, itemtags, allInstanceIDs);

                        thisPackage = pcobj.scanData;
                        allInstanceIDs = pcobj.iids;
                        itemtags = pcobj.tagl;

                        decompbr.Dispose();
                        decomps.Dispose();
                    } else if (indexData[e].CompressionType == "0000"){
                        readFile.BaseStream.Position = indexData[e].uLocation;                           
                        int entryEnd = (int)readFile.BaseStream.Position + (int)indexData[e].Truesize;
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/COBJ{1} - Position: {2}", package.Name, cobjc, indexData[e].uLocation));

                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/COBJ{1} - File Size: {2}", package.Name, cobjc, indexData[e].Filesize));

                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/COBJ{1} - Memory Size: {2}", package.Name, cobjc, indexData[e].Truesize));

                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/COBJ{1} - Entry Ends At: {2}", package.Name, cobjc, entryEnd));

                        ProcessCOBJ pcobj = new(thisPackage, readFile, package.Name, e, itemtags, allInstanceIDs); 

                        thisPackage = pcobj.scanData;
                        allInstanceIDs = pcobj.iids;
                        itemtags = pcobj.tagl;                                      
                    }
                    cobjc++;
                }
            }

            if (fileHas.Exists(x => x.Description == "OBJD")){
                var entryspots = fileHas.Where(x => x.Description == "OBJD").Select(x => x.Location).ToList();

                int objdc = 0;
                foreach (int e in entryspots){
                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - Opening OBJD #{1}", package.Name, objdc));
                    if (indexData[e].CompressionType == "5A42"){
                        readFile.BaseStream.Position = indexData[e].uLocation;                       
                        int entryEnd = (int)readFile.BaseStream.Position + (int)indexData[e].Truesize;
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1} - Position: {2}", package.Name, objdc, indexData[e].uLocation));

                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1} - File Size: {2}", package.Name, objdc, indexData[e].Filesize));

                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1} - Memory Size: {2}", package.Name, objdc, indexData[e].Truesize));

                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1} - Entry Ends At: {2}", package.Name, objdc, entryEnd));


                        Stream decomps = Sims4Decryption.Decompress(ByteReaders.ReadEntryBytes(readFile, (int)indexData[e].Truesize));
                        
                        BinaryReader decompbr = new BinaryReader(decomps);

                        ProcessOBJD pobjd = new(thisPackage, decompbr, package.Name, e, itemtags, allInstanceIDs, objdc);

                        thisPackage = pobjd.package;
                        allInstanceIDs = pobjd.iids;
                        itemtags = pobjd.tagl;
                                                                      
                        decompbr.Dispose();
                        decomps.Dispose();
                    } else if (indexData[e].CompressionType == "0000"){
                        readFile.BaseStream.Position = indexData[e].uLocation;                       
                        int entryEnd = (int)readFile.BaseStream.Position + (int)indexData[e].Truesize;
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1} - Position: {2}", package.Name, objdc, indexData[e].uLocation));

                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1} - File Size: {2}", package.Name, objdc, indexData[e].Filesize));

                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1} - Memory Size: {2}", package.Name, objdc, indexData[e].Truesize));

                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1} - Entry Ends At: {2}", package.Name, objdc, entryEnd));



                        ProcessOBJD pobjd = new(thisPackage, readFile, package.Name, e, itemtags, allInstanceIDs, objdc);

                        thisPackage = pobjd.package;
                        allInstanceIDs = pobjd.iids;
                        itemtags = pobjd.tagl;                   
                    }                
                    objdc++;
                }                
            }

            if (fileHas.Exists(x => x.Description == "GEOM")){
                var entryspots = fileHas.Where(x => x.Description == "GEOM").Select(x => x.Location).ToList();
                
                thisPackage.Mesh = true;

                foreach (int e in entryspots){
                    string key = string.Format("{0}-{1}-{2}", indexData[e].TypeID, indexData[e].GroupID, indexData[e].InstanceID);
                    if (key != "00000000-00000000-0000000000000000"){
                        if (!thisPackage.MeshKeys.Exists(x => x == key)){
                            thisPackage.MeshKeys.Add(key);
                        }
                    }                                       
                }
            }

            if (fileHas.Exists(x => x.Description == "THUM")){
                TransformImages imageTransformations = new();

                var entryspots = fileHas.Where(x => x.Description == "THUM").Select(x => x.Location).ToList();

                int c = 0;
                foreach (int e in entryspots){
                    c++;
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/THUM{1} - Reading THUM {1}, identified as: {2}.", package.Name, e, indexData[e].TypeID));

                    readFile.BaseStream.Position = indexData[e].uLocation; 
                    if (indexData[e].CompressionType == "5A42"){  
                        int entryEnd = (int)readFile.BaseStream.Position + (int)indexData[e].Truesize;                        
                        MemoryStream decomps = Sims4Decryption.DecompressMS(ByteReaders.ReadEntryBytes(readFile, (int)indexData[e].Truesize));
                        byte[] imagebyte = decomps.ToArray();                                            
                        imageTransformations.TransformToPNG(imagebyte, package.Name);
                        thisPackage.ThumbnailData = imagebyte;
                    } else if (indexData[e].CompressionType == "0000"){
                        byte[] imagebyte = ByteReaders.ReadEntryBytes(readFile, (int)indexData[e].Truesize);

                        imageTransformations.TransformToPNG(imagebyte, package.Name);
                        thisPackage.ThumbnailData = imagebyte;
                    }
                }
            } else if (fileHas.Exists(x => x.Description == "CASP")) {
                List<string> instanceids = new();

                var entryspots = fileHas.Where(x => x.Description == "CASP").Select(x => x.Location).ToList();

                foreach (int e in entryspots){
                    instanceids.Add(indexData[e].InstanceID);
                }
                ReadThumbCache rtc = new(thisPackage, fileHas, instanceids, instance, package.Name);
                thisPackage = rtc.thisPackage;
            } else if (fileHas.Exists(x => x.Description == "COBJ")) {
                List<string> instanceids = new();
                var entryspots = fileHas.Where(x => x.Description == "COBJ").Select(x => x.Location).ToList();
                foreach (int e in entryspots){
                    instanceids.Add(indexData[e].InstanceID);
                }
                ReadThumbCache rtc = new(thisPackage, fileHas, instanceids, instance, package.Name);
                thisPackage = rtc.thisPackage;
            }
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - All methods complete, moving on to getting info.", package.Name));            
            List<PackageTypeCounter> typecount = new List<PackageTypeCounter>();

            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - Making dictionary.", package.Name));

            foreach (EntryType type in EntryTypes){
                var match = fileHas.Where(x => x.TypeID == type.TypeID).Count();
                          
                if (match != 0){
                    PackageTypeCounter tc = new PackageTypeCounter();
                    tc.Description = type.Tag;
                    tc.Count = match;
                    tc.TypeID = type.TypeID;
                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}  has {1} of {2}.", package.Name, tc.Count, tc.Description));
                    typecount.Add(tc);
                } 
            }

            thisPackage.EntryTypeCount= typecount;
            thisPackage.Entries = fileHas;
            thisPackage.Flags = allFlags.Distinct().ToList();

            /*foreach (string flag in allFlags){
                var inp = thisPackage.Flags.Where(c => c.Flag == flag.Flag);
                if (!inp.Any()){
                    thisPackage.Flags.Add(flag); 
                }                
            }

            foreach (TagsList tag in itemtags){
                var inp = thisPackage.CatalogTags.Where(c => c.TypeID == tag.TypeID);
                if (!inp.Any()){
                    thisPackage.CatalogTags.Add(tag); 
                }
            }
            
            foreach (string iid in allInstanceIDs){
                var inp = thisPackage.InstanceIDs.Where(c => c == iid);
                if (!inp.Any()){
                    thisPackage.InstanceIDs.Add(iid); 
                }                
            }
            
            */

            thisPackage.InstanceIDs = allInstanceIDs.Distinct().ToList();

            

            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}: Checking {1} against override IDs.", package.Name,package.Name));

            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}: Checking instances.", package.Name));

            List<SimsOverride> overrides = (from r in Overrides
                                where thisPackage.InstanceIDs.Any(mr => r.InstanceID == mr)
                                select r).ToList();
            
            if(overrides.Count > 0){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}: Found {1} overrides!", package.Name, overrides.Count));
                thisPackage.Override = true;
                //thisPackage.Type = "OVERRIDE";
            }

            /*var specoverrides = Overrides.Where(p => overrides.Any(l => p.InstanceID == l.InstanceID)).ToList();

            List<SimsOverride> speco = new List<SimsOverride>();
            List<OverriddenList> OverridesList = new();
            string overridedesc = "";
            foreach (SimsOverride ov in overrides) {
                if (ov.InstanceID != "0000000000000000"){
                    var specco = overrides.Where(p => p.InstanceID == ov.InstanceID).ToList();                    
                    string description = "";
                    if (specco.Any()){
                        description = specco[0].Description;
                        thisPackage.Type = string.Format("OVERRIDE: {0}", description);
                        overridedesc = description;
                    }
                    OverridesList.Add(new OverriddenList(){InstanceID = ov.InstanceID, Name = ov.Name, Pack = ov.Pack, Type = ov.Type});
                }
            }

            if (OverridesList.Any()){
                thisPackage.OverridesList.AddRange(OverridesList);
            }


            if (overridedesc == "Eyes - Sim"){
                var entryspot = (from has in fileHas
                                    where has.InstanceID == "7882EE328F843230" && (has.TypeID == "2BC04EDF" || has.TypeID == "3453CF95")
                                    select has.uLocation).FirstOrDefault();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - Retrieving default {2} image from {1}", package.Name, overridedesc, indexData[entryspot].TypeID));

                if (indexData[entryspot].TypeID == "2BC04EDF"){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - {1} saved as LRLE.", package.Name, overridedesc));
                    ImageTransformations imageTransformations = new();
                    if (indexData[entryspot].TypeID == "5A42"){
                        //MemoryStream decomps = S4Decryption.DecompressMS(Methods.ReadEntryBytes(readFile, (int)indexData[entryspot].Truesize));
                        //imageTransformations.LRLE(new BinaryReader(decomps), LogMessage, LogFile, log);
                    } else if (indexData[entryspot].TypeID == "0000"){
                        //MemoryStream decomps = new (Methods.ReadEntryBytes(readFile, (int)indexData[entryspot].Truesize));
                        //imageTransformations.LRLE(readFile, LogMessage, LogFile, log);
                    }
                } else if (indexData[entryspot].TypeID == "3453CF95"){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - {1} saved as RLE2.", package.Name, overridedesc));
                }
            } else if (overridedesc == "Eyes - Vampire"){
                var entryspot = (from has in fileHas
                                    where has.InstanceID =="C6BF87F05E8A3FA7" && (has.TypeID == "2BC04EDF" || has.TypeID == "3453CF95")
                                    select has.uLocation).FirstOrDefault();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - Retrieving default {2} image from {1}", package.Name, overridedesc, indexData[entryspot].TypeID));

                if (indexData[entryspot].TypeID == "2BC04EDF"){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - {1} saved as LRLE.", package.Name, overridedesc));
                } else if (indexData[entryspot].TypeID == "3453CF95"){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - {1} saved as RLE2.", package.Name, overridedesc));
                }
            } else if (overridedesc == "Eyes - Werewolf"){
                var entryspot = (from has in fileHas
                                    where has.InstanceID =="B30EE6C1DE1BF2AD" && (has.TypeID == "2BC04EDF" || has.TypeID == "3453CF95")
                                    select has.uLocation).FirstOrDefault();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - Retrieving default {2} image from {1}", package.Name, overridedesc, indexData[entryspot].TypeID));

                if (indexData[entryspot].TypeID == "2BC04EDF"){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - {1} saved as LRLE.", package.Name, overridedesc));
                } else if (indexData[entryspot].TypeID == "3453CF95"){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - {1} saved as RLE2.", package.Name, overridedesc));
                }
            } else if (overridedesc == "Eyes - Merperson"){
                var entryspot = (from has in fileHas
                                    where has.InstanceID =="0125D3F76E073504" && (has.TypeID == "2BC04EDF" || has.TypeID == "3453CF95")
                                    select has.uLocation).FirstOrDefault();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - Retrieving default {2} image from {1}", package.Name, overridedesc, indexData[entryspot].TypeID));

                if (indexData[entryspot].TypeID == "2BC04EDF"){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - {1} saved as LRLE.", package.Name, overridedesc));
                } else if (indexData[entryspot].TypeID == "3453CF95"){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - {1} saved as RLE2.", package.Name, overridedesc));
                }
            } else if (overridedesc == "Eyes - Alien"){
                var entryspot = (from has in fileHas
                                    where has.InstanceID =="6C3AC424D2673953" && (has.TypeID == "2BC04EDF" || has.TypeID == "3453CF95")
                                    select has.uLocation).FirstOrDefault();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - Retrieving default {2} image from {1}", package.Name, overridedesc, indexData[entryspot].TypeID));

                if (indexData[entryspot].TypeID == "2BC04EDF"){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - {1} saved as LRLE.", package.Name, overridedesc));
                } else if (indexData[entryspot].TypeID == "3453CF95"){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - {1} saved as RLE2.", package.Name, overridedesc));
                }
            } else if (overridedesc == "Eyes - Cat"){
                var entryspot = (from has in fileHas
                                    where has.InstanceID =="6EC5F5CED3435737" && (has.TypeID == "2BC04EDF" || has.TypeID == "3453CF95")
                                    select has.uLocation).FirstOrDefault();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - Retrieving default {2} image from {1}", package.Name, overridedesc, indexData[entryspot].TypeID));

                if (indexData[entryspot].TypeID == "2BC04EDF"){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - {1} saved as LRLE.", package.Name, overridedesc));
                } else if (indexData[entryspot].TypeID == "3453CF95"){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - {1} saved as RLE2.", package.Name, overridedesc));
                }
            } else if (overridedesc == "Eyes - Big Dog"){
                var entryspot = (from has in fileHas
                                    where has.InstanceID =="809410A14AC0FD9A" && (has.TypeID == "2BC04EDF" || has.TypeID == "3453CF95")
                                    select has.uLocation).FirstOrDefault();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - Retrieving default {2} image from {1}", package.Name, overridedesc, indexData[entryspot].TypeID));

                if (indexData[entryspot].TypeID == "2BC04EDF"){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - {1} saved as LRLE.", package.Name, overridedesc));
                } else if (indexData[entryspot].TypeID == "3453CF95"){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - {1} saved as RLE2.", package.Name, overridedesc));
                }
            } else if (overridedesc == "Eyes - Small Dog"){
                var entryspot = (from has in fileHas
                                    where has.InstanceID =="770417D9838C8EF2" && (has.TypeID == "2BC04EDF" || has.TypeID == "3453CF95")
                                    select has.uLocation).FirstOrDefault();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - Retrieving default {2} image from {1}", package.Name, overridedesc, indexData[entryspot].TypeID));

                if (indexData[entryspot].TypeID == "2BC04EDF"){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - {1} saved as LRLE.", package.Name, overridedesc));
                } else if (indexData[entryspot].TypeID == "3453CF95"){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - {1} saved as RLE2.", package.Name, overridedesc));
                }
            }*/





            if (thisPackage.CatalogTags.Any()){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}: Checking tags list for function.", package.Name));
                TagsList tag = new();
                List<TagsList> roomsort = new();
                bool identifiedTags = false;;
                bool identifiedRS = false;
                List<string> checks = new(){ "2380","2381","2382","2383","2384","2385","2219","264","351","459","18451","63538","265","12290","691","1661","159","157","160","158","1614","2203","1602","10263","55379","359","2083","1499","1984","63511","45089","63512","1728","1730","1729","759","2236","43025","1686","651","1603","28674","28678","28676","28675","28677","12302","10253","12373","10241","1345","12386","1349","1351","1350","12305","12301","817","55335","12303","67631","2429","795","12307","12308","12298","12299","2432","816","47142","1242","12306","67632","2430","10242","1344","12387","1346","1348","1347","1243","12388","12309","12310","12364","67633","12304","12300","67634","2431","815","67636","67604","1536","1537","1533","1518","1531","1532","1515","1534","1513","1517","1535","1519","1516","1514","1522","49154","1521","1523","1507","1509","1524","1508","59472","1510","1512","1511","83985","1506","2423","1505","57425","57424","269","2358","268","65551","347","2357","523","831","833","832","667","669","670","10260","671","672","673","674","970","176","456","457","968","2075","173","969","458","175","226","966","972","973","185","967","193","191","190","189","186","187","913","1032","61493","45069","797","798","799","800","67621","921","810","801","802","803","804","805","10256","806","807","808","43012","809","2246","65623","811","812","813","865","818","561","538","535","918","974","1611","544","541","554","556","1068","1067","1069","552","1081","550","537","915","976","1441","1442","2425","251","250","782","547","560","540","539","975","919","906","977","543","551","559","557","1065","1066","545","546","558","555","542","981","536","224","225","914","971","222","217","221","219","229","916","220","223","917","218","446","979","194","2188","823","978","785","199","231","252","195","207","965","964","209","202","1246","198","1496","200","203","201","824","197","1071","440","169","163","171","162","165","177","166","161","164","55356","55374","2240","178","298","309","308","307","303","302","304","305","306","301","299","300","441","570","1413","174","167","168","179","172","196","205","204","208","1718","206","310","1283","1293","1205","1396","24577","1446","1374","12365","1112","55375","2241","2014","1948","1944","1947","1978","1945","1976","1977","1946","1979","192","2042","183","180","920","182","181","184","1228","2211","59410","59416","59415","230","1122","1123","211","214","210","215","212","963","962","216","227","1072","213" };
                List<string> checks2 = new(){"1126","228","872","873","875","874","2237","891","2443","2349","892","55376","2242","2226","2227","2231","2232","412","415","408","411","413","414","410","409","428","768","769","790","791","792","1038","1039","793","794","980","770","2224","866","771","772","773","774","775","776","819","1656","45071","1596","429"};
                checks.AddRange(checks2);
                List<string> roomchecks = new() {"271","272","468","273","864","274","270","407","275","276"};
                var tagstrings = thisPackage.CatalogTags.Select(ct => ct.TypeID);
                foreach (string c in checks){
                    if (tagstrings.Contains(c)){                        
                        identifiedTags = true;
                        tag = thisPackage.CatalogTags.Where(ct => ct.TypeID == c).FirstOrDefault();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}: Found tag {1}: {2} {3}", package.Name, tag.TypeID, tag.Function, tag.Subfunction));
                        break;
                    }
                }
                foreach (string rc in roomchecks){
                    if (tagstrings.Contains(rc)){
                        identifiedRS = true;
                        var rs = thisPackage.CatalogTags.Where(ct => ct.TypeID == rc).FirstOrDefault();
                        roomsort.Add(rs);
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}: Found roomsort {1}: {2}", package.Name, rs.TypeID, rs.Function));
                    }
                }                

                if (identifiedTags){
                    thisPackage.Function = tag.Function;
                    thisPackage.FunctionSubcategory = tag.Subfunction;
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}: Package function identified from tags list: {1}: {2}", package.Name, thisPackage.Function, thisPackage.FunctionSubcategory));
                }
                if (identifiedRS){
                    foreach (TagsList tg in roomsort){
                        thisPackage.RoomSort.Add(tag.Function);
                    }
                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}: Room sorts identified from tags list.", package.Name));
                }                
            }

            var S4SM = typecount.Where(Type => Type.Description == "S4SM").Select(Type => Type.Count).FirstOrDefault();
            var BGEO = typecount.Where(Type => Type.Description == "BGEO").Select(Type => Type.Count).FirstOrDefault();
            var HOTC = typecount.Where(Type => Type.Description == "HOTC").Select(Type => Type.Count).FirstOrDefault();
            var SMOD = typecount.Where(Type => Type.Description == "SMOD").Select(Type => Type.Count).FirstOrDefault();
            var BOND = typecount.Where(Type => Type.Description == "BOND").Select(Type => Type.Count).FirstOrDefault();
            var CPRE = typecount.Where(Type => Type.Description == "CPRE").Select(Type => Type.Count).FirstOrDefault();
            var DMAP = typecount.Where(Type => Type.Description == "DMAP").Select(Type => Type.Count).FirstOrDefault();
            var RLE2 = typecount.Where(Type => Type.Description == "RLE2").Select(Type => Type.Count).FirstOrDefault();
            var CASP = typecount.Where(Type => Type.Description == "CASP").Select(Type => Type.Count).FirstOrDefault();
            var GEOM = typecount.Where(Type => Type.Description == "GEOM").Select(Type => Type.Count).FirstOrDefault();
            var LRLE = typecount.Where(Type => Type.Description == "LRLE").Select(Type => Type.Count).FirstOrDefault();
            var RLE = typecount.Where(Type => Type.Description == "RLE").Select(Type => Type.Count).FirstOrDefault();
            var RMAP = typecount.Where(Type => Type.Description == "RMAP").Select(Type => Type.Count).FirstOrDefault();
            var CLHD = typecount.Where(Type => Type.Description == "CLHD").Select(Type => Type.Count).FirstOrDefault();
            var STBL = typecount.Where(Type => Type.Description == "STBL").Select(Type => Type.Count).FirstOrDefault();
            var IMG = typecount.Where(Type => Type.Description == "_IMG").Select(Type => Type.Count).FirstOrDefault();
            //trait tuning
            var TRTR = typecount.Where(Type => Type.Description == "TRTR").Select(Type => Type.Count).FirstOrDefault();
            //snippet tuning
            var SNTR = typecount.Where(Type => Type.Description == "SNTR").Select(Type => Type.Count).FirstOrDefault();
            //interaction tuning
            var INTR = typecount.Where(Type => Type.Description == "INTR").Select(Type => Type.Count).FirstOrDefault();
            //interaction tuning
            var GFX = typecount.Where(Type => Type.Description == "GFX").Select(Type => Type.Count).FirstOrDefault();
            //action tuning
            var ACT = typecount.Where(Type => Type.TypeID == "0C772E27").Select(Type => Type.Count).FirstOrDefault();
            //test based score tuning
            var TBST = typecount.Where(Type => Type.Description == "TBST").Select(Type => Type.Count).FirstOrDefault();
            //buff tuning
            var BUFT = typecount.Where(Type => Type.TypeID == "6017E896").Select(Type => Type.Count).FirstOrDefault();
            //sim data
            var DATA = typecount.Where(Type => Type.Description == "DATA").Select(Type => Type.Count).FirstOrDefault();
            //rel bit
            var RBTR = typecount.Where(Type => Type.Description == "RBTR").Select(Type => Type.Count).FirstOrDefault();
            //rel bit
            var SGTR = typecount.Where(Type => Type.Description == "SGTR").Select(Type => Type.Count).FirstOrDefault();
            //aspiration
            var ASTR = typecount.Where(Type => Type.Description == "ASTR").Select(Type => Type.Count).FirstOrDefault();
            //pie menu
            var PMTR = typecount.Where(Type => Type.Description == "PMTR").Select(Type => Type.Count).FirstOrDefault();
            //broadcaster
            var BCTR = typecount.Where(Type => Type.Description == "BCTR").Select(Type => Type.Count).FirstOrDefault();
            //reward traits
            var RWTR = typecount.Where(Type => Type.Description == "RWTR").Select(Type => Type.Count).FirstOrDefault();
            //statistics
            var STTR = typecount.Where(Type => Type.Description == "STTR").Select(Type => Type.Count).FirstOrDefault();
            //career levels
            var CLTR = typecount.Where(Type => Type.Description == "CLTR").Select(Type => Type.Count).FirstOrDefault();
            //career tracks
            var CTTR = typecount.Where(Type => Type.Description == "CTTR").Select(Type => Type.Count).FirstOrDefault();
            //career
            var CATR = typecount.Where(Type => Type.Description == "CATR").Select(Type => Type.Count).FirstOrDefault();
            //objectives
            var OBTR = typecount.Where(Type => Type.Description == "OBTR").Select(Type => Type.Count).FirstOrDefault();
            //model
            var MODL = typecount.Where(Type => Type.Description == "MODL").Select(Type => Type.Count).FirstOrDefault();
            //model lod
            var MLOD = typecount.Where(Type => Type.Description == "MLOD").Select(Type => Type.Count).FirstOrDefault();
            //object catlog
            var COBJ = typecount.Where(Type => Type.Description == "COBJ").Select(Type => Type.Count).FirstOrDefault();
            

            if (thisPackage.Override == false){
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("No overrides were found. Checking other options."));
                if (S4SM != 0){
                    thisPackage.Type = "Merged Package";
                } else if (!String.IsNullOrWhiteSpace(thisPackage.Function)) {
                    thisPackage.Type = thisPackage.Function;
                } else if (!String.IsNullOrWhiteSpace(thisPackage.Tuning)) {
                    if (thisPackage.Tuning.Contains("object_bassinetGEN")){
                        thisPackage.Type = "Bassinet";
                    }
                } else {                    
                    if (CLTR >= 1 || CTTR >= 1 || CATR >= 1){
                        thisPackage.Type = "MOD: Career";
                        thisPackage.IsMod = true;
                    } else if (SNTR >= 1 && INTR >= 1 && ACT >= 1 && TBST >= 1 && BUFT >= 1 && DATA >= 1 && RBTR >= 1 && SGTR >= 1 && ASTR >= 1 && PMTR >= 1 && BCTR >= 1 && RWTR >= 1 && STTR >= 1) {
                        thisPackage.Type = "MOD: Large";
                        thisPackage.IsMod = true;
                    } else if (OBTR >= 1 || SNTR >= 1 || INTR >= 1 || ACT >= 1 || TBST >= 1 || BUFT >= 1 || DATA >= 1 || RBTR >= 1 || SGTR >= 1 || ASTR >= 1 || PMTR >= 1 || BCTR >= 1 || RWTR >= 1 || STTR >= 1 || GFX >= 1) {
                        thisPackage.Type = "MOD";
                        thisPackage.IsMod = true;
                    } else if (TRTR >= 1) {
                        thisPackage.Type = "MOD: Trait";
                        thisPackage.IsMod = true;
                    } else if (BGEO >= 1 && HOTC >= 1 && SMOD >= 1){
                        thisPackage.Type = "Slider";
                    } else if (BOND >= 1 && CPRE >= 1 && DMAP >= 1 && SMOD >= 1){
                        thisPackage.Type = "CAS Preset";
                    } else if (RLE2 >= 1 && CASP >= 1 && GEOM <= 0){
                        thisPackage.Type = "CAS Recolor";
                    } else if (LRLE >= 1 && CASP >= 1 && GEOM <= 0){
                        thisPackage.Type = "Makeup";
                    } else if (IMG >= 1 && CLHD >= 1 && STBL >= 1) {
                        thisPackage.Type = "Pose Pack";
                    } else {
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}: Unable to identify package!", package.Name));
                        thisPackage.Type = "UNKNOWN";
                    }                    
                }
                if (GEOM >= 1 || MODL >= 1 || MLOD >= 1){
                    thisPackage.Mesh = true;
                    thisPackage.NoMesh = false;
                }                
                if (CASP >= 1 || COBJ >= 1){
                    thisPackage.Recolor = true;
                }             
            }

            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("This is a {0}!!", thisPackage.Type));

            if (thisPackage.AgeGenderFlags != null){
                if ((thisPackage.AgeGenderFlags.Female == true) && (thisPackage.AgeGenderFlags.Male == true)){
                    thisPackage.Gender = "Both";
                } else if (thisPackage.AgeGenderFlags.Female == true){
                    thisPackage.Gender = "Female";
                } else if (thisPackage.AgeGenderFlags.Male == true){
                    thisPackage.Gender = "Male";
                }

                string age = "";
                if (thisPackage.AgeGenderFlags.Adult == true){
                    if (string.IsNullOrEmpty(age)){
                        age = "Adult";
                    } else {
                        age += string.Format(", Adult");
                    }
                }
                if (thisPackage.AgeGenderFlags.Baby == true){
                    if (string.IsNullOrEmpty(age)){
                        age = "Baby";
                    } else {
                        age += string.Format(", Baby");
                    }
                }
                if (thisPackage.AgeGenderFlags.Child == true){
                    if (string.IsNullOrEmpty(age)){
                        age = "Child";
                    } else {
                        age += string.Format(", Child");
                    }
                }
                if (thisPackage.AgeGenderFlags.Elder == true){
                    if (string.IsNullOrEmpty(age)){
                        age = "Elder";
                    } else {
                        age += string.Format(", Elder");
                    }
                }
                if (thisPackage.AgeGenderFlags.Infant == true){
                    if (string.IsNullOrEmpty(age)){
                        age = "Infant";
                    } else {
                        age += string.Format(", Infant");
                    }
                }
                if (thisPackage.AgeGenderFlags.Teen == true){
                    if (string.IsNullOrEmpty(age)){
                        age = "Teen";
                    } else {
                        age += string.Format(", Teen");
                    }
                }
                if (thisPackage.AgeGenderFlags.Toddler == true){
                    if (string.IsNullOrEmpty(age)){
                        age = "Toddler";
                    } else {
                        age += string.Format(", Toddler");
                    }
                }
                if (thisPackage.AgeGenderFlags.YoungAdult == true){
                    if (string.IsNullOrEmpty(age)){
                        age = "Young Adult";
                    } else {
                        age += string.Format(" and Young Adult");
                    }
                }
                thisPackage.Age = age;           
            }
            //dbpfFile.Close();
            //dbpfFile.Dispose();
            readFile.Close();
            readFile.Dispose();

            if (thisPackage.NoMesh == true){
                thisPackage.Orphan = false;
            } else if (thisPackage.Mesh == false && thisPackage.Recolor == true && thisPackage.Override == false){
                thisPackage.Orphan = true;  
            } else if (thisPackage.Mesh == true && thisPackage.Recolor == false && thisPackage.Override == false){
                thisPackage.Orphan = true;
            }           
            

            if (thisPackage.Function == "Hair" && thisPackage.Mesh == false){
                thisPackage.FunctionSubcategory = "Recolor";
            }

            //DEBUG TESTING ONLY - COMMENT OUT!!
            /*SimsPackage simsPackage = new();
            simsPackage.ScanData = thisPackage;            
            simsPackage.InfoFile = Path.Combine(@"M:\SCCM\The Sims 4\debugtesting", string.Format("{0}.xml", package.Name));
            simsPackage.WriteInfoFile();*/

            return thisPackage;
        }

        public List<OverriddenList> GetSims4Overrides(FileInfo package){
            Dictionary<string, string> PackOptions = new()
            {
                { "Data", "Base Game"},
                { "EP01", "Get To Work" },
                { "EP02", "Get Together" },
                { "EP03", "City Living" },
                { "EP04", "Cats & Dogs" },
                { "EP05", "Seasons" },
                { "EP06", "Get Famous" },
                { "EP07", "Island Living" },
                { "EP08", "Discover University" },
                { "EP09", "Eco Lifestyle" },
                { "EP10", "Snowy Escape" },
                { "EP11", "Cottage Living" },
                { "EP12", "High School Years" },
                { "EP13", "Growing Together" },
                { "EP14", "Horse Ranch" },
                { "EP15", "For Rent" },
                { "EP16", "Lovestruck" },
                { "GP01", "Outdoor Retreat" },
                { "GP02", "Spa Day" },
                { "GP03", "Dine Out" },
                { "GP04", "Vampires" },
                { "GP05", "Parenthood" },
                { "GP06", "Jungle Adventure" },
                { "GP07", "StrangerVille" },
                { "GP08", "Realm of Magic" },
                { "GP09", "Star Wars: Journey to Batuu" },
                { "GP10", "Dream Home Decorator" },
                { "GP11", "My Wedding Stories" },
                { "GP12", "Werewolves" },
                { "SP01", "Luxury Party Stuff" },
                { "SP02", "Perfect Patio Stuff" },
                { "SP03", "Cool Kitchen Stuff" },
                { "SP04", "Spooky Stuff" },
                { "SP05", "Movie Hangout Stuff" },
                { "SP06", "Romantic Garden Stuff" },
                { "SP07", "Kids Room Stuff" },
                { "SP08", "Backyard Stuff" },
                { "SP09", "Vintage Glamour Stuff" },
                { "SP10", "Bowling Night Stuff" },
                { "SP11", "Fitness Stuff" },
                { "SP12", "Toddler Stuff" },
                { "SP13", "Laundry Day Stuff" },
                { "SP14", "My First Pet Stuff" },
                { "SP15", "Moshino Stuff Pack" },
                { "SP16", "Tiny Living" },
                { "SP17", "Nifty Knitting" },
                { "SP18", "Paranormal" },
                { "SP46", "Home Chef Hustle Stuff" },
                { "SP49", "Crystal Creations Stuff Pack" },
                { "SP20", "Throwback Fit Kit" },
                { "SP21", "Country Kitchen Kit" },
                { "SP22", "Bust The Dust Kit" },
                { "SP23", "Courtyard Oasis Kit" },
                { "SP24", "Fashion Street-Set" },
                { "SP25", "Industrial Loft Kit" },
                { "SP26", "Incheon Arrivals Kit" },
                { "SP28", "Modern Menswear Kit" },
                { "SP29", "Blooming Rooms Kit" },
                { "SP30", "Carnaval Streetwear Kit" },
                { "SP31", "Decor to the Max Kit" },
                { "SP32", "Moonlight Chic Kit" },
                { "SP33", "Little Campers Kit" },
                { "SP34", "First Fits Kit" },
                { "SP35", "Desert Luxe Kit" },
                { "SP36", "Pastel Pop Kit" },
                { "SP37", "Everyday Clutter Kit" },
                { "SP38", "Simtimates Collection Kit" },
                { "SP39", "Bathroom Clutter Kit" },
                { "SP40", "Greenhouse Haven Kit" },
                { "SP41", "Basement Treasures Kit" },
                { "SP42", "Grunge Revival Kit" },
                { "SP43", "Book Nook Kit" },
                { "SP44", "Poolside Splash Kit" },
                { "SP45", "Modern Luxe Kit" },
                { "SP47", "Castle Estate Kit" },
                { "SP48", "Goth Galore Kit" },
                { "SP50", "Urban Homage Kit" },
                { "SP51", "Party Essentials Kit" },
                { "SP52", "Riviera Retreat Kit" },
                { "SP53", "Cozy Bistro Kit" },
                { "SP54", "Artist Studio Kit" },
                { "SP55", "Storybook Nursery Kit" },
                { "FP01", "Holiday Celebration" }
			};
            List<OverriddenList> overrides = new();
            //locations
            long entrycountloc = 36;
            long indexRecordPositionloc = 64;

            //Lists 
                
            List<IndexEntry> entries = new();
            IndexEntry MergedManifest = new();
            List<EntryLocations> fileHas = new();
            ArrayList linkData = new();
            List<IndexEntry> indexData = new();

            //create readers  
            //FileStream filePackage = new FileStream(package.FullName, FileMode.Open, FileAccess.Read);
            MemoryStream msPackage = ByteReaders.ReadBytesFromFile(package.FullName, (int)package.Length);
            BinaryReader readFile = new BinaryReader(msPackage);
            
            //log opening file
            
            readFile.BaseStream.Position = 0;

            readFile.BaseStream.Position = entrycountloc;

            uint entrycount = readFile.ReadUInt32();
            
            //record position low
            uint indexRecordPositionLow = readFile.ReadUInt32();
            
            //index record size
            uint indexRecordSize = readFile.ReadUInt32();
            
            readFile.BaseStream.Position = indexRecordPositionloc;

            ulong indexRecordPosition = readFile.ReadUInt64();
            
                        
            byte[] headersize = new byte[96];
            long here = 100;
            long movedto = 0;

            if (indexRecordPosition != 0){
                long indexseek = (long)indexRecordPosition - headersize.Length;
                movedto = here + indexseek;
                readFile.BaseStream.Position = here + indexseek;                
            } else {
                movedto = here + indexRecordPositionLow;
                readFile.BaseStream.Position = here + indexRecordPositionLow;
            }

            readFile.BaseStream.Position = (long)indexRecordPosition;
            uint indextype = readFile.ReadUInt32();            
            
            long streamsize = readFile.BaseStream.Length;
            int indexbytes = ((int)entrycount * 32);
            long indexpos = 0;
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - Streamsize is {1}", package.Name, streamsize));
            if ((int)movedto + indexbytes != streamsize){
                /*int entriesfound = 0;
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - Streamsize does not match!", package.Name));
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0} - Position: {1}, Moved to: {2}", package.Name, readFile.BaseStream.Position, movedto));
                readFile.BaseStream.Position = movedto - 400;
                List<long> entrylocs = new();
                uint item;
                
                while(readFile.BaseStream.Length > readFile.BaseStream.Position){                    
                    item = readFile.ReadUInt32();
                    //
                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Read byte at {0}: {1}", readFile.BaseStream.Position, item.ToString("X8")));
                    EntryType type = new();
                    if (EntryTypes.Where(x => x.TypeID == item.ToString("X8")).Any()){
                        type = EntryTypes.Where(x => x.TypeID == item.ToString("X8")).First();  
                    }

                    if (type != null){
                        IndexEntry holderEntry = new IndexEntry();
                        holderEntry.TypeID = item.ToString("X8");
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1} - Index Entry TypeID: {2}", package.Name, entriesfound, holderEntry.TypeID));

                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Found entry{2} - {0} at location {1}!", item, readFile.BaseStream.Position, entriesfound));
                        entrylocs.Add(readFile.BaseStream.Position);
                        if (entriesfound == 0){
                            indexpos = readFile.BaseStream.Position - 4;
                        }
                        entriesfound++;
                        if (entriesfound == entrycount){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Found {0} entries. Breaking.", entrycount));
                            break;
                        }
                    }
                }


                readFile.BaseStream.Position = indexpos;
                entrycount = 0;
                foreach (int loc in entrylocs){
                    IndexEntry holderEntry = new IndexEntry(); 
                    readFile.BaseStream.Position = loc - 4;
                    if (indextype == 2){
                        holderEntry.TypeID = readFile.ReadUInt32().ToString("X8");                       
                        
                        EntryType type = new();
                        if (EntryTypes.Where(x => x.TypeID == holderEntry.TypeID).Any()){
                            type = EntryTypes.Where(x => x.TypeID == holderEntry.TypeID).First();  
                        }
                        if(type != null){                        
                            fileHas.Add(new() {TypeID = type.TypeID, Description = type.Tag, Location = (int)entrycount});
                        } else {
                            fileHas.Add(new() { TypeID = holderEntry.TypeID, Location = (int)entrycount});
                        }                            

                        string instanceid1 = (readFile.ReadUInt32() << 32).ToString("X8");
                        string instanceid2 = (readFile.ReadUInt32() << 32).ToString("X8");
                        holderEntry.InstanceID = string.Format("{0}{1}", instanceid1,instanceid2);
                        
                        
                        holderEntry.uLocation = readFile.ReadUInt32();

                        holderEntry.Filesize = readFile.ReadUInt32();

                        holderEntry.Truesize = readFile.ReadUInt32();

                        indexData.Add(holderEntry);
                        MergedManifest = holderEntry;

                        holderEntry = null;

                        entrycount++;
                    }
                }*/
            } else {
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Streamsize for {0} matches.", package.Name));
                long movedhere = readFile.BaseStream.Position;
                uint testpos = readFile.ReadUInt32();
                if (testpos != 0){                    
                    readFile.BaseStream.Position = movedto;
                } else if (testpos == 80000000) {
                    long moveback = movedhere - 4;
                    readFile.BaseStream.Position = moveback;
                }
                for (int i = 0; i < entrycount; i++){                    
                    IndexEntry holderEntry = new IndexEntry();                
                    holderEntry.TypeID = readFile.ReadUInt32().ToString("X8");
                    EntryType type = new();
                    if (EntryTypes.Where(x => x.TypeID == holderEntry.TypeID).Any()){
                        type = EntryTypes.Where(x => x.TypeID == holderEntry.TypeID).First();  
                    }
                
                    if(type != null){
                        fileHas.Add(new() { TypeID = type.TypeID, Description = type.Tag, Location = i });                    
                    } else {
                        fileHas.Add(new() { TypeID = holderEntry.TypeID, Location = i});
                    } 

                    holderEntry.GroupID = readFile.ReadUInt32().ToString("X8");

                    string instanceid1 = (readFile.ReadUInt32() << 32).ToString("X8");
                    string instanceid2 = (readFile.ReadUInt32() << 32).ToString("X8");
                    holderEntry.InstanceID = string.Format("{0}{1}", instanceid1,instanceid2);
                  
                    uint testin = readFile.ReadUInt32();
                    holderEntry.uLocation = testin;

                    holderEntry.Filesize = readFile.ReadUInt32();

                    holderEntry.Truesize = readFile.ReadUInt32();

                    holderEntry.CompressionType = readFile.ReadUInt16().ToString("X4");

                    readFile.BaseStream.Position = readFile.BaseStream.Position + 2;

                    indexData.Add(holderEntry);
                }
            }
            foreach (IndexEntry ie in indexData){
				string pack = "Unknown";
				foreach (KeyValuePair<string, string> pck in PackOptions){
					if (package.DirectoryName.Contains(pck.Key)) pack = pck.Value;
				}	
				EntryType et = new();
				if (EntryTypes.Where(x => x.TypeID == ie.TypeID).Any()){
					et = EntryTypes.Where(x => x.TypeID == ie.TypeID).First();
				} else {
					et = new(){
						Description = "Unknown"
					};
				}
				OverriddenList ol = new() {
					InstanceID = ie.InstanceID,
					GroupID = ie.GroupID,
					TypeID = ie.TypeID,
					Override = string.Format("{0}-{1}-{2}", ie.TypeID, ie.GroupID, ie.InstanceID),
					Type = et.Description,
					Pack = pack
				};
				overrides.Add(ol);
			}
            return overrides;
        }

        public static List<S4CategoryTag> GetTagInfo(CASTag16Bit tags, uint count){            
            List<S4CategoryTag> taglist = new();
            S4CategoryTag tagg = new();
            //List<TagsList> tagQuery = new();
            for (int i = 0; i < count; i++)
            {   
                if (tags.tagKey[i] != 0){
                    List<S4CategoryTag> tagQuery = CategoryTags.Where(x => x.TypeID == tags.tagKey[i].ToString()).ToList();
                    
                    //tagQuery = GlobalVariables.S4FunctionTypesConnection.Query<TagsList>(string.Format("SELECT * FROM S4CategoryTags where TypeID='{0}'", tags.tagKey[i]));
                    if (tagQuery.Any()){
                        tagg = tagQuery[0];
                        taglist.Add(tagg);
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Tag {0} ({1}) was matched to {2}.", i, tagg.TypeID, tagg.Description));
                    } else {
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Tag {0} matched nothing.", tags.tagKey[i]));
                    }  
                }
                if (tags.catKey[i] != 0){
                    List<S4CategoryTag> tagQuery = CategoryTags.Where(x => x.TypeID == tags.catKey[i].ToString()).ToList();
                    if (tagQuery.Any()){
                        tagg = tagQuery[0];
                        taglist.Add(tagg);
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Tag {0} ({1}) was matched to {2}.", i, tagg.TypeID, tagg.Description));
                    } else {
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Tag {0} matched nothing.", tags.catKey[i]));
                    }  
                }   
                tagg = new();                                         
            }
            return taglist;
        }
        
        public static List<S4CategoryTag> GetTagInfo(Tag tags, uint count){
            List<S4CategoryTag> taglist = new();
            S4CategoryTag tagg = new();
            for (int i = 0; i < count; i++)
            {
                List<S4CategoryTag> tagQuery = CategoryTags.Where(x => x.TypeID == tags.tagKey[i].ToString()).ToList();
                if (tagQuery.Any()){
                    tagg = tagQuery[0];
                    taglist.Add(tagg);
                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Tag {0} ({1}) was matched to {2}.", i, tags.tagKey[i], tagg.Description));
                } else {
                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Tag {0} matched nothing.", tags.tagKey[i]));
                }
                tagg = new();
            }
            return taglist;
        }        
        
        public static List<S4CategoryTag> GetTagInfo(List<uint> tags, uint count){
            List<S4CategoryTag> taglist = new();
            S4CategoryTag tagg = new();         
            for (int i = 0; i < count; i++)
            {

                List<S4CategoryTag> tagQuery = CategoryTags.Where(x => x.TypeID == tags[i].ToString()).ToList();
                if (tagQuery.Any()){
                    tagg = tagQuery[0];
                    taglist.Add(tagg);
                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Tag {0} ({2}) was matched to {1}.", i, taglist[i].Description, tags[i]));
                } else {
                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Tag {0} matched nothing.", taglist[i]));
                }
                tagg = new();
            }
            return taglist;
        }
    }
















    public struct ProcessCASP{
        public List<string> allFlags;
        public Sims4ScanData scanData;
        public List<TagsList> itemtags;

        public ProcessCASP(Sims4ScanData sd, string fileName, BinaryReader readFile, Stream dbpfFile, int caspnum, List<TagsList> tagsl, List<string> flagslist){
            this.allFlags = flagslist;
            this.scanData = sd;
            this.itemtags = tagsl;

            uint version = readFile.ReadUInt32();
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Version: {2}", fileName, caspnum, version.ToString("X8")));
            uint tgioffset = readFile.ReadUInt32() +8;
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - TGI Offset: {2} \n -- As Hex: {3}", fileName,caspnum, tgioffset, tgioffset.ToString("X8")));
            uint numpresets = readFile.ReadUInt32();
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Number of Presets: {2} \n -- As Hex: {3}", fileName,caspnum, numpresets, numpresets.ToString("X8")));
            using (BinaryReader reader = new BinaryReader(dbpfFile, Encoding.BigEndianUnicode, true))
            {
                scanData.Title = reader.ReadString();
            }
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Title: {2}", fileName,caspnum, scanData.Title));

            float sortpriority = readFile.ReadSingle();
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Sort Priority: {2}", fileName,caspnum, sortpriority));

            int secondarySortIndex = readFile.ReadUInt16();
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Secondary Sort Index: {2}", fileName,caspnum, secondarySortIndex));

            uint propertyid = readFile.ReadUInt32();
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Property ID: {2}", fileName,caspnum, propertyid));
            
            uint auralMaterialHash = readFile.ReadUInt32();
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Aural Material Hash: {2}", fileName,caspnum, sortpriority));

            if (version <= 42){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Version is <= 42: {0}", version));
                int[] parameterFlag = new int[1];
                parameterFlag[0] = (int)readFile.ReadUInt16();
                BitArray parameterFlags = new BitArray(parameterFlag);
                for(int p = 0; p < 16; p++)
                {
                    if (parameterFlags[p] == true) {
                        var af = allFlags.Where(x => x == Sims4PackageReader.Flags[p]);
                        if (af.Any()){
                            allFlags.Add(Sims4PackageReader.Flags[p]);  
                        }
                    }
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Function Sort Flag [{2}]: {3}, {4}", fileName,caspnum, p, Sims4PackageReader.Flags[p], parameterFlags[p].ToString()));
                } 
                
            } else if (version >= 43){
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Version is >= 43: {2}", fileName,caspnum, version));
                int[] parameterFlag = new int[1];
                parameterFlag[0] = (int)readFile.ReadUInt16();
                BitArray parameterFlags = new BitArray(parameterFlag);

                for(int pfc = 0; pfc < 16; pfc++){
                    if (parameterFlags[pfc] == true) {
                        var af = allFlags.Where(x => x == Sims4PackageReader.Flags[pfc]);
                        if (af.Any() && pfc == 2) {
                            allFlags.Add(Sims4PackageReader.Flags[pfc]);   
                            scanData.AllowRandom = true; 
                        } else if (af.Any()){
                            allFlags.Add(Sims4PackageReader.Flags[pfc]);  
                        }
                    }
                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Function Sort Flag [{2}]: {3}, {4}", fileName,caspnum, pfc, Sims4PackageReader.Flags[pfc], parameterFlags[pfc].ToString()));
                }                            
            }
                ulong excludePartFlags = readFile.ReadUInt64();
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Exclude Part Flags: {2}", fileName,caspnum, excludePartFlags.ToString("X16")));
                ulong excludePartFlags2 = readFile.ReadUInt64();
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Exclude Part Flags 2: {2}", fileName,caspnum, excludePartFlags2.ToString("X16")));
                ulong excludeModifierRegionFlags = readFile.ReadUInt64();
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Exclude Modifier Region Flags: {2}", fileName,caspnum, excludeModifierRegionFlags.ToString("X16")));

            if (version >= 37){
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} >= 37, Version: {0}", fileName,caspnum, version));
                uint count = readFile.ReadByte();
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Tag Count: {2}", fileName,caspnum, count.ToString()));
                readFile.ReadByte();
                CASTag16Bit tags = new CASTag16Bit(readFile, count);                            
                List<S4CategoryTag> gottags = Readers.GetTagInfo(tags, count);
                foreach (S4CategoryTag tag in gottags){
                    itemtags.Add(new() { Description = tag.Description, Function = tag.Function, TypeID = tag.TypeID, Subfunction = tag.Subfunction});
                }
            } 
            else 
            {
                uint count = readFile.ReadByte();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Tag Count: {2}", fileName,caspnum, count.ToString()));
                readFile.ReadByte();
                CASTag16Bit tags = new CASTag16Bit(readFile, count);
                
                List<S4CategoryTag> gottags = Readers.GetTagInfo(tags, count);
                foreach (S4CategoryTag tag in gottags){
                    itemtags.Add(new() { Description = tag.Description, Function = tag.Function, TypeID = tag.TypeID, Subfunction = tag.Subfunction});
                }

                }

                uint simoleonprice = readFile.ReadUInt32();
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Simoleon Price: {2}", fileName,caspnum, simoleonprice.ToString()));
                uint partTitleKey = readFile.ReadUInt32();
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Part Title Key: {2}", fileName,caspnum, partTitleKey.ToString()));
                uint partDescriptionKey = readFile.ReadUInt32();
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Part Description Key: {2}", fileName,caspnum, partDescriptionKey.ToString()));
                if (version >= 43) {
                    uint createDescriptionKey = readFile.ReadUInt32();
                }
                int uniqueTextureSpace = readFile.ReadByte();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Unique Texture Space: {2}", fileName,caspnum, uniqueTextureSpace.ToString("X8")));
                uint bodytype = readFile.ReadUInt32();
                bool foundmatch = false;

                List<S4Function> bodytypes = Sims4PackageReader.Functions.Where(x => x.BodyType == bodytype.ToString()).ToList();
                
                if (bodytypes.Any()){
                    foundmatch = true;
                    scanData.Function = bodytypes[0].Function;
                    if (!String.IsNullOrWhiteSpace(bodytypes[0].Subfunction)) {
                        scanData.FunctionSubcategory = bodytypes[0].Subfunction;
                    }
                }

                if (foundmatch == false){
                    scanData.Function = string.Format("Unidentified function (contact SinfulSimming). Code: {0}", bodytype.ToString());
                }
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Bodytype: {2}", fileName,caspnum, bodytype.ToString()));
                uint bodytypesubtype = readFile.ReadUInt16();
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Body Sub Type: {2}", fileName,caspnum, bodytypesubtype.ToString()));
                readFile.ReadUInt32();                        
                uint agflags = readFile.ReadUInt32();
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Ag Flags: {2}", fileName,caspnum, agflags.ToString("X8")));

                string AGFlag = agflags.ToString("X8");
                
                S4AgeGenderFlags agegenderset = new();

                if (AGFlag == "00000000") {
                    agegenderset = new S4AgeGenderFlags{
                    Adult = false, 
                    Baby = false, 
                    Child = false, 
                    Elder = false, 
                    Infant = false, 
                    Teen = false, 
                    Toddler = false, 
                    YoungAdult = false, 
                    Female = false, 
                    Male = false};
                } else if (AGFlag == "00000020") {
                    agegenderset = new S4AgeGenderFlags{
                    Adult = true, 
                    Baby = false, 
                    Child = false, 
                    Elder = false, 
                    Infant = false, 
                    Teen = false, 
                    Toddler = false, 
                    YoungAdult = false, 
                    Female = false, 
                    Male = false};
                } else if (AGFlag == "00002020") {
                    agegenderset = new S4AgeGenderFlags{
                        Adult = true, 
                    Baby = false, 
                    Child = false, 
                    Elder = false, 
                    Infant = false, 
                    Teen = false, 
                    Toddler = false, 
                    YoungAdult = false, 
                    Female = true, 
                    Male = false};
                } else if (AGFlag == "00020000") {
                    agegenderset = new S4AgeGenderFlags{
                        Adult = true, 
                        Baby = false, 
                        Child = false, 
                        Elder = false, 
                        Infant = false, 
                        Teen = false, 
                        Toddler = false, 
                        YoungAdult = false, 
                        Female = false, 
                        Male = true};
                } else if (AGFlag == "00002078") {
                    agegenderset = new S4AgeGenderFlags{
                        Adult = true, 
                        Baby = false, 
                        Child = false, 
                        Elder = true, 
                        Infant = false, 
                        Teen = true, 
                        Toddler = false, 
                        YoungAdult = true, 
                        Female = true, 
                        Male = false};
                } else if (AGFlag == "000030FF") {
                    agegenderset = new S4AgeGenderFlags{
                    Adult = true, 
                    Baby = true, 
                    Child = true, 
                    Elder = true, 
                    Infant = true, 
                    Teen = true, 
                    Toddler = true, 
                    YoungAdult = true, 
                    Female = true, 
                    Male = true};
                } else if (AGFlag == "00003004") {
                    agegenderset = new S4AgeGenderFlags{
                    Adult = false, 
                    Baby = false, 
                    Child = true, 
                    Elder = false, 
                    Infant = false, 
                    Teen = false, 
                    Toddler = false, 
                    YoungAdult = false, 
                    Female = true, 
                    Male = true};
                } else if (AGFlag == "00001078") {
                    agegenderset = new S4AgeGenderFlags{
                    Adult = true, 
                    Baby = false, 
                    Child = false, 
                    Elder = true, 
                    Infant = false, 
                    Teen = true, 
                    Toddler = false, 
                    YoungAdult = true, 
                    Female = false, 
                    Male = true};
                } else if (AGFlag == "00003078") {
                    agegenderset = new S4AgeGenderFlags{
                    Adult = true, 
                    Baby = false, 
                    Child = false, 
                    Elder = true, 
                    Infant = false, 
                    Teen = true, 
                    Toddler = false, 
                    YoungAdult = true, 
                    Female = true, 
                    Male = true};
                } else if (AGFlag == "000030BE") {
                    agegenderset = new S4AgeGenderFlags{
                    Adult = true, 
                    Baby = false, 
                    Child = true, 
                    Elder = false, 
                    Infant = true, 
                    Teen = true, 
                    Toddler = true, 
                    YoungAdult = true, 
                    Female = true, 
                    Male = true};
                } else if (AGFlag == "00002002") {
                    agegenderset = new S4AgeGenderFlags{
                    Adult = false, 
                    Baby = false, 
                    Child = false, 
                    Elder = false, 
                    Infant = false, 
                    Teen = false, 
                    Toddler = true, 
                    YoungAdult = false, 
                    Female = true, 
                    Male = false};
                }  else if (AGFlag == "00002004") {
                    agegenderset = new S4AgeGenderFlags{
                    Adult = false, 
                    Baby = false, 
                    Child = true, 
                    Elder = false, 
                    Infant = false, 
                    Teen = false, 
                    Toddler = false, 
                    YoungAdult = false, 
                    Female = true, 
                    Male = false};
                }  else if (AGFlag == "00003002") {
                    agegenderset = new S4AgeGenderFlags{
                    Adult = false, 
                    Baby = false, 
                    Child = false, 
                    Elder = false, 
                    Infant = false, 
                    Teen = false, 
                    Toddler = true, 
                    YoungAdult = false, 
                    Female = true, 
                    Male = true};
                }  else if (AGFlag == "00003004") {
                    agegenderset = new S4AgeGenderFlags{
                    Adult = false, 
                    Baby = false, 
                    Child = true, 
                    Elder = false, 
                    Infant = false, 
                    Teen = false, 
                    Toddler = false, 
                    YoungAdult = false, 
                    Female = true, 
                    Male = true};
                }  else if (AGFlag == "00001002") {
                    agegenderset = new S4AgeGenderFlags{
                    Adult = false, 
                    Baby = false, 
                    Child = false, 
                    Elder = false, 
                    Infant = false, 
                    Teen = false, 
                    Toddler = true, 
                    YoungAdult = false, 
                    Female = false, 
                    Male = true};
                }  else if (AGFlag == "00001004") {
                    agegenderset = new S4AgeGenderFlags{
                    Adult = false, 
                    Baby = false, 
                    Child = true, 
                    Elder = false, 
                    Infant = false, 
                    Teen = false, 
                    Toddler = false, 
                    YoungAdult = false, 
                    Female = false, 
                    Male = true};
                } else if (AGFlag == "00100101") {
                    agegenderset = new S4AgeGenderFlags{
                    Adult = false, 
                    Baby = true, 
                    Child = false, 
                    Elder = false, 
                    Infant = false, 
                    Teen = false, 
                    Toddler = false, 
                    YoungAdult = false, 
                    Female = false, 
                    Male = false};                            
                } else if (AGFlag == "0000307E"){
                    agegenderset = new S4AgeGenderFlags{
                    Adult = true, 
                    Baby = false, 
                    Child = true, 
                    Elder = true, 
                    Infant = false, 
                    Teen = true, 
                    Toddler = true, 
                    YoungAdult = true, 
                    Female = true, 
                    Male = true};
                } else if (AGFlag == "000030FE"){
                    agegenderset = new S4AgeGenderFlags{
                    Adult = true, 
                    Baby = false, 
                    Child = true, 
                    Elder = true, 
                    Infant = true, 
                    Teen = true, 
                    Toddler = true, 
                    YoungAdult = true, 
                    Female = true, 
                    Male = true};
                } else {
                    agegenderset = new S4AgeGenderFlags{
                    Adult = false, 
                    Baby = false, 
                    Child = false, 
                    Elder = false, 
                    Infant = false, 
                    Teen = false, 
                    Toddler = false, 
                    YoungAdult = false, 
                    Female = false, 
                    Male = false};
                }
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} Ages:\n -- Adult: {2}, \n -- Baby: {3}, \n -- Child: {4}, \n -- Elder: {5}, \n -- Infant: {6}, \n -- Teen: {7}, \n -- Toddler: {8}, \n -- Young Adult: {9}\n\nGender: \n -- Male: {10}\n -- Female: {11}.", fileName,caspnum, agegenderset.Adult.ToString(), agegenderset.Baby.ToString(), agegenderset.Child.ToString(), agegenderset.Elder.ToString(), agegenderset.Infant.ToString(), agegenderset.Teen.ToString(), agegenderset.Toddler.ToString(), agegenderset.YoungAdult.ToString(), agegenderset.Female.ToString(), agegenderset.Male.ToString()));

                scanData.AgeGenderFlags = agegenderset;  

                if (version >= 0x20)
                {
                    uint species = readFile.ReadUInt32();
                }
                if (version >= 34)
                {
                    int packID = readFile.ReadInt16();
                    int packFlags = readFile.ReadByte();
                    for(int p = 0; p < packFlags; p++)
                    {
                        bool check = readFile.ReadBoolean();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Packflag is: {2}", fileName,caspnum, check.ToString()));
                    } 
                    byte[] reserved2 = readFile.ReadBytes(9);
                }
                else
                {
                    byte unused2 = readFile.ReadByte();
                    if (unused2 > 0) {
                        int unused3 = readFile.ReadByte();
                    }
                }

                uint buffResKey = readFile.ReadByte();
                uint varientThumbnailKey = readFile.ReadByte();
                if (version >= 28){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Version is >= 28: {2}.", fileName,caspnum, version));
                    ulong voiceEffecthash = readFile.ReadUInt64();
                }
                if (version >= 30){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Version is >= 30: {2}.", fileName,caspnum, version));
                    uint usedMaterialCount = readFile.ReadByte();
                    if (usedMaterialCount > 0){
                        uint materialSetUpperBodyHash = readFile.ReadUInt32();
                        uint materialSetLowerBodyHash = readFile.ReadUInt32();
                        uint materialSetShoesBodyHash = readFile.ReadUInt32();
                    }
                }
                if (version >= 31){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Version is >= 31: {2}.", fileName,caspnum, version));
                    uint hideForOccultFlags = readFile.ReadUInt32();
                }

                if (version >= 38){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Version is >= 38: {2}.", fileName,caspnum, version));
                    ulong oppositeGenderPart = readFile.ReadUInt64();
                }

                if (version >= 39)
                {
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Version is >= 39: {2}.", fileName,caspnum, version));
                    ulong fallbackPart = readFile.ReadUInt64();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Fallback Part: {2}.", fileName,caspnum, fallbackPart));
                }
                if (version >= 44)
                {
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Version is >= 44: {2}.", fileName,caspnum, version));
                    
                    float opacitySliderMin = readFile.ReadSingle();
                    float opacitySliderInc = readFile.ReadSingle();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Opacity: {2}/{3}.", fileName,caspnum, opacitySliderMin, opacitySliderInc));

                    float hueMin = readFile.ReadSingle();
                    float hueMax = readFile.ReadSingle();
                    float hueInc = readFile.ReadSingle();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Hue: {2}/{3}/{4}.", fileName,caspnum, hueMin, hueMax, hueInc));

                    float satMin = readFile.ReadSingle();
                    float satMax = readFile.ReadSingle();
                    float satInc = readFile.ReadSingle();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Saturation: {2}/{3}/{4}.", fileName,caspnum, satMin, satMax, satInc));

                    float brgMin = readFile.ReadSingle();
                    float brgMax = readFile.ReadSingle();
                    float brgInc = readFile.ReadSingle();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Brightness: {2}/{3}/{4}.", fileName,caspnum, brgMin, brgMax, brgInc));
                }

                uint nakedKey = readFile.ReadByte();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Naked Key: {2}.", fileName,caspnum, nakedKey));
                uint parentKey = readFile.ReadByte();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Parent Key: {2}.", fileName,caspnum, parentKey));

                if (version == 42){
                    readFile.ReadBytes(17);
                    uint sortLayer = readFile.ReadUInt32();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Sort Layer: {2}.", fileName,caspnum, sortLayer.ToString("X8")));

                    var currentPosition = readFile.BaseStream.Position;
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - LOD Count should be at: {2}.", fileName,caspnum, currentPosition));
                    
                    var lodcount = readFile.ReadByte();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - LOD Count: {2}.", fileName,caspnum, lodcount.ToString("X2")));
                    
                    if (lodcount == 0 || lodcount == 00 || lodcount == 000){
                        scanData.NoMesh = true;
                    } else {
                        var level = readFile.ReadByte();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Level: {2}.", fileName,caspnum, level.ToString("X2")));
                        var assetlist = readFile.ReadByte();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - LOD Asset List: {2}.", fileName,caspnum, assetlist.ToString("X8")));
                        
                        var CastShadow = readFile.ReadInt32();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Cast Shadow: {2}.", fileName,caspnum, CastShadow.ToString("X8")));
                        var Sorting = readFile.ReadInt32();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Sorting: {2}.", fileName,caspnum, Sorting.ToString("X8")));
                        var SpecLevel = readFile.ReadInt32();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Spec Level: {2}.", fileName,caspnum, SpecLevel.ToString("X8")));
                        
                        readFile.ReadBytes(4);
                        var LODKeyList = readFile.ReadByte();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - LOD Key List: {2}.", fileName,caspnum, LODKeyList.ToString("X2")));

                        readFile.ReadBytes(102);

                        for (int i = 0; i < LODKeyList * lodcount; i++){
                            ulong iid1 = readFile.ReadUInt64();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - LOD {2}, IID: {3}.", fileName,caspnum, i, iid1.ToString("X16")));
                            uint gid1 = readFile.ReadUInt32();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - LOD {2}, GID: {3}.", fileName,caspnum, i, gid1.ToString("X8")));
                            uint tid1 = readFile.ReadUInt32();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - LOD {2}, tID: {3}.", fileName,caspnum, i, tid1.ToString("X8")));
                            string lodkey = string.Format("{0}-{1}-{2}", tid1.ToString("X8"), gid1.ToString("X8"), iid1.ToString("X16"));
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - LOD {2} Key: {3}", fileName,caspnum, i, lodkey));
                            if (lodkey != "00000000-00000000-0000000000000000"){
                                var match = scanData.CASPartKeys.Contains(lodkey);
                                if (!match){
                                    scanData.CASPartKeys.Add(lodkey);
                                } 
                            }
                        }
                        ulong iid2 = readFile.ReadUInt64();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Diffuse Key IID: {2}.", fileName,caspnum, iid2.ToString("X16")));


                        uint gid2 = readFile.ReadUInt32();


                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Diffuse Key GID: {2}.", fileName,caspnum, gid2.ToString("X8")));


                        uint tid2 = readFile.ReadUInt32();


                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Diffuse Key tID: {2}.", fileName,caspnum, tid2.ToString("X8")));


                        string diffusekey = string.Format("{0}-{1}-{2}", tid2.ToString("X8"), gid2.ToString("X8"), iid2.ToString("X16"));

                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Diffuse Key Key: {2}", fileName,caspnum, diffusekey));
                        
                        ulong iid3 = readFile.ReadUInt64();


                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Shadow Key IID: {2}.", fileName,caspnum, iid3.ToString("X16")));


                        uint gid3 = readFile.ReadUInt32();


                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Shadow Key GID: {2}.", fileName,caspnum, gid3.ToString("X8")));


                        uint tid3 = readFile.ReadUInt32();


                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Shadow Key tID: {2}.", fileName,caspnum, tid3.ToString("X8")));


                        string shadowkey = string.Format("{0}-{1}-{2}", tid3.ToString("X8"), gid3.ToString("X8"), iid3.ToString("X16"));


                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Shadow Key Key: {2}", fileName,caspnum, shadowkey));

                        ulong iid4 = readFile.ReadUInt64();


                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Region Key IID: {2}.", fileName,caspnum, iid4.ToString("X16")));


                        uint gid4 = readFile.ReadUInt32();


                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Region Key GID: {2}.", fileName,caspnum, gid4.ToString("X8")));


                        uint tid4 = readFile.ReadUInt32();


                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Region Key tID: {2}.", fileName,caspnum, tid4.ToString("X8")));


                        string regionkey = string.Format("{0}-{1}-{2}", tid4.ToString("X8"), gid4.ToString("X8"), iid4.ToString("X16"));


                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Region Key: {2}", fileName,caspnum, regionkey));

                    }
                } else {                    
                    uint sortLayer = readFile.ReadUInt32();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - Sort Layer: {2}.", fileName,caspnum, sortLayer.ToString("X8")));

                    var currentPosition = readFile.BaseStream.Position;
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - LOD Count should be at: {2}.", fileName,caspnum, currentPosition));
                    
                    readFile.BaseStream.Position = tgioffset;
                    var tginum = readFile.ReadByte();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - TGI Num: {2}.", fileName,caspnum, tginum));
                    for (int t = 0; t < tginum; t++){
                        ulong iid = readFile.ReadUInt64();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - TGI {2}, IID: {3}.", fileName,caspnum, t, iid));
                        uint gid = readFile.ReadUInt32();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - TGI {2}, GID: {3}.", fileName,caspnum, t, gid));
                        uint tid = readFile.ReadUInt32();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - TGI {2}, TID: {3}.", fileName,caspnum, t, tid));
                        string key = string.Format("{0}-{1}-{2}", tid.ToString("X8"), gid.ToString("X8"), iid.ToString("X16"));
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - TGI {2} Key: {3}", fileName,caspnum, t, key));
                        if (key != "00000000-00000000-0000000000000000"){
                            var match = scanData.CASPartKeys.Contains(key);
                            if (!match){                                
                                scanData.CASPartKeys.Add(key);
                            } 
                        }                                
                    }
                    
                    readFile.BaseStream.Position = currentPosition;


                    var lodcount = readFile.ReadByte();
                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - LOD Count: {2}.", fileName,caspnum, lodcount.ToString("X2")));                    
                    
                    if (lodcount == 0){
                        scanData.NoMesh = true;
                    } else {
                        for (int t = 0; t < lodcount; t++){
                            ulong iid = readFile.ReadUInt64();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - LOD {2}, IID: {3}.", fileName,caspnum, t, iid.ToString("X16")));
                            uint gid = readFile.ReadUInt32();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - LOD {2}, GID: {3}.", fileName,caspnum, t, gid.ToString("X8")));
                            uint tid = readFile.ReadUInt32();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - LOD {2}, tID: {3}.", fileName,caspnum, t, tid.ToString("X8")));
                            string key = string.Format("{0}-{1}-{2}", tid.ToString("X8"), gid.ToString("X8"), iid.ToString("X16"));
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/CASP{1} - LOD {2} Key: {3}", fileName,caspnum, t, key));
                            if (key != "00000000-00000000-0000000000000000"){
                                var match = scanData.CASPartKeys.Contains(key);
                                if (!match){                                    
                                    scanData.CASPartKeys.Add(key);
                                } 
                            }                                
                        }
                    }
                }
        }        
    }
    
    public struct ProcessCOBJ {
        public Sims4ScanData scanData;
        public List<string> iids;
        public List<TagsList> tagl;

        public ProcessCOBJ(Sims4ScanData sd, BinaryReader readFile,  string packagename, int e, List<TagsList> itemtags, List<string> allInstanceIDs){
            this.scanData = sd;
            this.iids = allInstanceIDs;
            this.tagl = itemtags;
            
            ReadCOBJ rc = new ReadCOBJ(readFile, packagename, e, itemtags);  
            scanData.OBJDPartKeys.AddRange(rc.objkeys);
            iids.Add(rc.instanceid.ToString("X8"));
            foreach (TagsList tag in rc.itemtags) {
                tagl.Add(tag);                                 
            }
        }        
    }

    public struct ReadCOBJ{
        /// <summary>
        /// Reads COBJD entries.
        /// </summary>
        public string GUID = "null";
        public List<TagsList> itemtags = new();
        GlobalVariables globals = new GlobalVariables();
        public List<string> allinstanceIDs = new();
        public List<string> objkeys = new();
        public ulong instanceid;
        public uint typeid;
        public uint groupid;

        public ReadCOBJ(BinaryReader readFile, string packagename, int e, List<TagsList> itemtags){
            uint version = readFile.ReadUInt32();
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1} - Version: {2} \n-- As hex: {3}", packagename, e, version, version.ToString("X8")));
            uint commonblockversion = readFile.ReadUInt32();
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1} - Common Block Version: {2} \n-- As hex: {3}", packagename, e, commonblockversion, commonblockversion.ToString("X8")));
            uint namehash = readFile.ReadUInt32();
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1} - Name Hash: {2} \n-- As hex: {3}", packagename, e, namehash, namehash.ToString("X8")));
            uint descriptionhash = readFile.ReadUInt32();
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1} - Description Hash: {2} \n-- As hex: {3}", packagename, e, descriptionhash, descriptionhash.ToString("X8")));
            uint price = readFile.ReadUInt32();
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1} - Price: {2} \n-- As hex: {3}", packagename, e, price, price.ToString("X8")));

            ulong thumbhash = readFile.ReadUInt64();
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1} - Thumb Hash: {2} \n-- As hex: {3}", packagename, e, thumbhash, thumbhash.ToString("X8")));

            uint devcatflags = readFile.ReadUInt32();
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1} - Dev Cat Flags: {2} \n-- As hex: {3}", packagename, e, devcatflags, devcatflags.ToString("X8")));
            
            int tgicount = readFile.ReadByte();
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1}, - TGI Count: {2}", packagename, e, tgicount));
            instanceid = 0;
            typeid = 0;
            groupid = 0;

            if (tgicount != 0){
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1}, - TGI Count is not zero. Reading Resources.", packagename, e));
                for (int i = 0; i < tgicount; i++){
                    ResourceKeyITG resourcekey = new ResourceKeyITG(readFile);
                    if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1}, - TGI #{2}: {3}", packagename, e, i, resourcekey.ToString()));
                    instanceid = resourcekey.instance;
                    typeid = resourcekey.type;
                    groupid = resourcekey.group;
                    allinstanceIDs.Add(instanceid.ToString("X8"));
                    objkeys.Add(resourcekey.ToString());
                }                
            }
            
            if (commonblockversion >= 10)
            {
                int packId = readFile.ReadInt16();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1}, - Pack ID: {2}", packagename, e, packId));
                int packFlags = readFile.ReadByte();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1}, - PackFlags: {2}", packagename, e, packFlags));
                readFile.ReadBytes(9);
            } else {
                int unused2 = readFile.ReadByte();
                if (unused2 > 0)
                {
                    readFile.ReadByte();
                }
            }

            if (commonblockversion >= 11){
                uint count = readFile.ReadUInt32();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1}, - Tags Count: {2}", packagename, e, count));
                Tag tags = new Tag(readFile, count);

                List<S4CategoryTag> gottags = Sims4PackageReader.GetTagInfo(tags, count);
                foreach (S4CategoryTag tag in gottags){
                    itemtags.Add(new() { Description = tag.Description, Function = tag.Function, Subfunction = tag.Subfunction, TypeID = tag.TypeID});                      
                }
                
            } else {
                uint count = readFile.ReadUInt32();
                List<uint> tagsread = new List<uint>();
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1}, - Num Tags: {2}", packagename, e, count));
                for (int t = 0; t < count; t++){                    
                    uint tagvalue = readFile.ReadUInt16();
                    tagsread.Add(tagvalue);
                }
                List<S4CategoryTag> gottags = Sims4PackageReader.GetTagInfo(tagsread, count);
                foreach (S4CategoryTag tag in gottags){
                    itemtags.Add(new() { Description = tag.Description, Function = tag.Function, Subfunction = tag.Subfunction, TypeID = tag.TypeID});                                          
                }
            }
            long location = readFile.BaseStream.Position;
            uint count2 = readFile.ReadUInt32();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1}, - Selling Point Count: {2}", packagename, e, count2));
            if (count2 > 100){
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1}, - Selling point count is too high, something went wrong.", packagename, e));
            } else {
                Tag sellingtags = new Tag(readFile, count2);
                List<S4CategoryTag> gottags = Sims4PackageReader.GetTagInfo(sellingtags, count2);
                foreach (S4CategoryTag tag in gottags){
                    itemtags.Add(new() { Description = tag.Description, Function = tag.Function, Subfunction = tag.Subfunction, TypeID = tag.TypeID}); 
                }
            }

            uint unlockByHash = readFile.ReadUInt32();
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1}, - UnlockBy Hash: {2}", packagename, e, unlockByHash));
            
            uint unlockedByHash = readFile.ReadUInt32();
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1}, - UnlockedBy Hash: {2}", packagename, e, unlockedByHash));

            int swatchColorSortPriority = readFile.ReadUInt16();
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1}, - Swatch Sort Priority: {2}", packagename, e, swatchColorSortPriority));

            ulong varientThumbImageHash = readFile.ReadUInt64();
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/E{1}, - Varient Thumb Image Hash: {2}", packagename, e, varientThumbImageHash));
        }   
    }
    
    public struct ProcessOBJD {
        public Sims4ScanData package;
        public List<string> iids;
        public List<TagsList> tagl;

        public ProcessOBJD(Sims4ScanData thisPackage, BinaryReader readFile, string packagename, int e, List<TagsList> itemtags, List<string> allInstanceIDs, int objdc){
            this.package = thisPackage;
            string[] objde;
            int[] objdp;
            this.iids = allInstanceIDs;
            this.tagl = itemtags;
            

            ReadOBJDIndex readOBJD = new ReadOBJDIndex(readFile, packagename, objdc);
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1} - There are {2} entries to read.", packagename, objdc, readOBJD.count));
            objde = new string[readOBJD.count];
            objdp = new int[readOBJD.count];
            for (int f = 0; f < readOBJD.count; f++){
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1} - Entry {2}: \n--- Type: {3}\n --- Position: {4}", packagename, objdc, f, readOBJD.entrytype[f], readOBJD.position[f]));
                objde[f] = readOBJD.entrytype[f].ToString();
                objdp[f] = (int)readOBJD.position[f];
            }
            readFile.BaseStream.Position = objdp[0];
            ReadOBJDEntry readobjdentry = new ReadOBJDEntry(readFile, objde, objdp, packagename, objdc);
            thisPackage.Title = readobjdentry.name;
            thisPackage.Tuning = readobjdentry.tuningname;
            thisPackage.TuningID = (int)readobjdentry.tuningid;            

            thisPackage.MeshKeys.AddRange(readobjdentry.meshes);
            
            iids.Add(readobjdentry.instance.ToString("X8"));
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Adding components to package: "));
                
            for (int c = 0; c < readobjdentry.componentcount; c++){
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1} - Components: {2}", packagename, e, readobjdentry.components[c].ToString("X8")));
                thisPackage.Components.Add(readobjdentry.components[c].ToString("X8"));
            }            
        }
    }

    public struct ReadOBJDIndex {
        /// <summary>
        /// Retrieves OBJD index.
        /// </summary>
        public int version;
        public long refposition;
        public int count;
        public uint[] entrytype;
        public uint[] position;

        public ReadOBJDIndex(BinaryReader reader, string packagename, int e){
            long entrystart = reader.BaseStream.Position;
            this.version = reader.ReadUInt16();
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1}, - Version: {2}", packagename, e, this.version));

            if (this.version > 150){
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1}, - Version is not legitimate.", packagename, e));
                this.refposition = 0; 
                this.count = (int)0;
                this.entrytype = new uint[0];
                this.position = new uint[0];
            } else {
                this.refposition = reader.ReadUInt32();
                this.refposition = refposition + entrystart;
                reader.BaseStream.Position = refposition;
                this.count = reader.ReadUInt16();
                this.entrytype = new uint[count];
                this.position = new uint[count];
                for (int i = 0; i < count; i++){
                    this.entrytype[i] = reader.ReadUInt32();
                    this.position[i] = reader.ReadUInt32();
                }
            }   
        }
    }

    public struct ReadOBJDEntry {
        /// <summary>
        /// Reads OBJD entries.
        /// </summary>
        public int namelength;
        public byte[] namebit;
        public string name;
        
        public int tuningnamelength;
        public byte[] tuningbit;
        public string tuningname;        
        public ulong tuningid;
        public uint componentcount;
        public uint[] components;        
        public int materialvariantlength;
        public byte[] materialvariantbyte;
        public string materialvariant;
        public uint price;
        public string[] icon;
        public string[] rig;
        public string[] slot;
        public string[] model;
        public string[] footprint;
        public List<string> meshes = new();
        private bool tuningidmissing = false;
        public uint type;
        public uint group;
        public ulong instance; 
        public ReadOBJDEntry(BinaryReader reader, string[] entries, int[] positions, string packageName, int e){
            uint preceeding;
            uint preceedingDiv;
            namelength = 0;
            namebit = new byte[0];
            name = "";
            tuningnamelength = 0;
            tuningbit = new byte[0];
            tuningname = "";
            tuningid = 0;
            componentcount = 0;
            components = new uint[0];
            materialvariantlength = 0;
            materialvariantbyte = new byte[0];
            materialvariant = "";
            price = 0;
            icon = new string[0];
            rig = new string[0];
            slot = new string[0];
            model = new string[0];
            footprint = new string[0];            
            tuningidmissing = false;  
            instance = 0;
            group = 0;
            type = 0;          

            for (int j = 0; j < entries.Length; j++){
                string entryid = entries[j];
                int entrypos = positions[j];
                switch (entryid)
                {
                    case "E7F07786": // name
                        reader.BaseStream.Position = entrypos;
                        this.namelength = reader.ReadByte();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1}, - Name Length: {2}", packageName, e, namelength));
                        reader.BaseStream.Position = reader.BaseStream.Position + 3;
                        //LogMessage = "Byte 1: " + reader.ReadByte().ToString();
                        //if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        //if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        //LogMessage = "Byte 2: " + reader.ReadByte().ToString();
                        //if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        //if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        //LogMessage = "Byte 3: " + reader.ReadByte().ToString();
                        //if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        //if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        this.namebit = reader.ReadBytes(namelength);
                        this.name = Encoding.UTF8.GetString(namebit);
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1}, - Name: {2}", packageName, e, name));
                        break;
                    case "790FA4BC": //tuning
                        reader.BaseStream.Position = entrypos;
                        this.tuningnamelength = reader.ReadByte();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1}, - Tuning Name Length: {2}", packageName, e, tuningnamelength));
                        reader.BaseStream.Position = reader.BaseStream.Position + 3;
                        //log.MakeLog("Reading three empty bytes.", true);
                        //LogMessage = "Byte 1: " + reader.ReadByte().ToString();
                        //if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        //if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        //LogMessage = "Byte 2: " + reader.ReadByte().ToString();
                        //if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        //if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        //LogMessage = "Byte 3: " + reader.ReadByte().ToString();
                        //if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        //if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        this.tuningbit = reader.ReadBytes(tuningnamelength);
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1}, - Tuning Name Length: {2}", packageName, e, tuningbit));
                        this.tuningname = Encoding.UTF8.GetString(tuningbit);
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1}, - Tuning Name: {2}", packageName, e, tuningname));
                        break;
                    case "B994039B": //TuningID
                        reader.BaseStream.Position = entrypos;
                        this.tuningid = reader.ReadUInt64(); 
                        break;
                    case "CADED888": //Icon
                        reader.BaseStream.Position = entrypos;
                        preceeding = reader.ReadUInt32();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1}, - Reading Preceeding UInt32: {2}", packageName, e, preceeding));
                        preceedingDiv = preceeding / 4;
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1}, - Number of Icon GUIDs: {2}", packageName, e, preceedingDiv));
                        this.icon = new string[preceedingDiv];
                        for (int p = 0; p < preceedingDiv; p++){
                            ResourceKeyITGFlip ricon = new ResourceKeyITGFlip(reader);
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1}, - Icon GUID: {2}", packageName, e, ricon.ToString()));
                            this.icon[p] = ricon.ToString();
                            this.meshes.Add(ricon.ToString());
                            this.type = ricon.type;
                            this.group = ricon.group;
                            this.instance = ricon.instance;                            
                        }
                        break;
                    case "E206AE4F": //Rig
                        reader.BaseStream.Position = entrypos;
                        preceeding = reader.ReadUInt32();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1}, - Reading preceeding UInt32: {2}", packageName, e, preceeding));
                        preceedingDiv = preceeding / 4;
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1}, - Number of Rig GUIDs: {2}", packageName, e, preceedingDiv));
                        this.rig = new string[preceedingDiv];
                        for (int p = 0; p < preceedingDiv; p++){
                            ResourceKeyITGFlip rkrig = new ResourceKeyITGFlip(reader);
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1}, - Rig GUID: {2}", packageName, e, rkrig.ToString()));
                            this.rig[p] = rkrig.ToString();
                            this.meshes.Add(rkrig.ToString());
                            this.type = rkrig.type;
                            this.group = rkrig.group;
                            this.instance = rkrig.instance;
                        }   
                        break;
                    case "8A85AFF3": //Slot
                        reader.BaseStream.Position = entrypos;
                        preceeding = reader.ReadUInt32();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1}, - Reading preceeding UInt32: {2}", packageName, e, preceeding));
                        preceedingDiv = preceeding / 4;
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1}, - Number of Slot GUIDs: {2}", packageName, e, preceedingDiv));
                        this.slot = new string[preceedingDiv];
                        for (int p = 0; p < preceedingDiv; p++){
                            ResourceKeyITGFlip rkslot = new ResourceKeyITGFlip(reader);
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1}, - Slot GUID: {2}", packageName, e, rkslot.ToString()));
                            this.slot[p] = rkslot.ToString();     
                            this.meshes.Add(rkslot.ToString());                       
                            this.type = rkslot.type;
                            this.group = rkslot.group;
                            this.instance = rkslot.instance;
                        }            
                        break;
                    case "8D20ACC6": //Model
                        reader.BaseStream.Position = entrypos;
                        preceeding = reader.ReadUInt32();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1}, - Reading preceeding UInt32: {2}", packageName, e, preceeding));
                        preceedingDiv = preceeding / 4;
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1}, - Number of Model GUIDs: {2}", packageName, e, preceedingDiv));
                        this.model = new string[preceedingDiv];
                        for (int p = 0; p < preceedingDiv; p++){
                            ResourceKeyITGFlip rkmodel = new ResourceKeyITGFlip(reader);
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1}, - Model GUID: {2}", packageName, e, rkmodel.ToString()));
                            this.model[p] = rkmodel.ToString();
                            this.meshes.Add(rkmodel.ToString());
                            this.type = rkmodel.type;
                            this.group = rkmodel.group;
                            this.instance = rkmodel.instance;
                        }
                        break;
                    case "6C737AD8": //Footprint
                        reader.BaseStream.Position = entrypos;
                        preceeding = reader.ReadUInt32();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1}, - Reading preceeding UInt32: {2}", packageName, e, preceeding));
                        preceedingDiv = preceeding / 4;
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1}, - Number of Footprint GUIDs: {2}", packageName, e, preceedingDiv));
                        this.footprint = new string[preceedingDiv];
                        for (int p = 0; p < preceedingDiv; p++){
                            ResourceKeyITGFlip rkft = new ResourceKeyITGFlip(reader);
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1}, - Footprint GUID: {2}", packageName, e, rkft.ToString()));
                            this.footprint[p] = rkft.ToString();
                            this.meshes.Add(rkft.ToString());
                            this.type = rkft.type;
                            this.group = rkft.group;
                            this.instance = rkft.instance;                
                        }
                        break;
                    case "E6E421FB": //Components
                        reader.BaseStream.Position = entrypos;
                        this.componentcount = reader.ReadUInt32();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1}, - Component Count: {2}", packageName, e, componentcount));
                        this.components = new uint[this.componentcount];
                        for (int i = 0; i < this.componentcount; i++){
                            components[i] = reader.ReadUInt32();
                        }
                        break;
                    case "ECD5A95F": //MaterialVariant
                        reader.BaseStream.Position = entrypos;
                        this.materialvariantlength = reader.ReadByte();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1}, - Material Variant Length: {2}", packageName, e, materialvariantlength));
                        reader.BaseStream.Position = reader.BaseStream.Position + 3;
                        //log.MakeLog("Reading three empty bytes.", true);
                        //LogMessage = "Byte 1: " + reader.ReadByte().ToString();
                        //if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        //if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        //LogMessage = "Byte 2: " + reader.ReadByte().ToString();
                        //if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        //if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        //LogMessage = "Byte 3: " + reader.ReadByte().ToString();
                        //if(GlobalVariables.highdebug == true) log.MakeLog(LogMessage, true);
                        //if(GlobalVariables.highdebug == false) LogFile.Append(string.Format("{0}\n", LogMessage));
                        this.materialvariantbyte = reader.ReadBytes(materialvariantlength);
                        this.materialvariant = Encoding.UTF8.GetString(materialvariantbyte);
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1}, - Material Variant: {2}", packageName, e, materialvariant));
                        break;
                    case "AC8E1BC0": //Unknown1
                        reader.BaseStream.Position = entrypos;

                        break;
                    case "E4F4FAA4": //SimoleonPrice
                        reader.BaseStream.Position = entrypos;
                        this.price = reader.ReadUInt32();
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}/OBJD{1}, - Price: {2}", packageName, e, price));
                        break;
                    case "7236BEEA": //PositiveEnvironmentScore
                        reader.BaseStream.Position = entrypos;

                        break;
                    case "44FC7512": //NegativeEnvironmentScore
                        reader.BaseStream.Position = entrypos;

                        break;
                    case "4233F8A0": //ThumbnailGeometryState
                        reader.BaseStream.Position = entrypos;

                        break;
                    case "EC3712E6": //Unknown2
                        reader.BaseStream.Position = entrypos;

                        break;
                    case "2172AEBE": //EnvironmentScoreEmotionTags
                        reader.BaseStream.Position = entrypos;

                        break;
                    case "DCD08394": //EnvironmentScores
                        reader.BaseStream.Position = entrypos;

                        break;
                    case "52F7F4BC": //Unknown3
                        reader.BaseStream.Position = entrypos;

                        break;
                    case "AEE67A1C": //IsBaby
                        reader.BaseStream.Position = entrypos;

                        break;
                    case "F3936A90": //Unknown4
                        reader.BaseStream.Position = entrypos;
                        
                        break;
                }
            }
        }

        public override string ToString(){
            return string.Format("Name: {0} \n Tuning Name: {1} \n TuningID: {2} \n Components: {3} \n Material Variant: {4} \n Price: {5} \n Icon: {6} \n Rig: {7}, Model: {8}, Slot: {9}, Footprint: {10}", this.name, this.tuningname, this.tuningid.ToString("X16"), GetFormatUIntArray(this.components), this.materialvariant, this.price.ToString("X8"), this.icon, this.rig, this.model, this.slot, this.footprint);
        }

        public static string GetFormatUIntArray(uint[] ints){
            string retVal = string.Empty;
            foreach (uint number in ints){
                if (string.IsNullOrEmpty(retVal)){
                    retVal += number.ToString("X8");
                } else {
                    retVal += string.Format(", {0}", number.ToString("X8"));
                }
                
            }
            return retVal;
        }
    }

    public struct ReadThumbCache {   
        public Sims4ScanData thisPackage;
        public ReadThumbCache(Sims4ScanData package, List<EntryLocations> fileHas, List<string> instanceids, Sims4Instance instance, string packagename){
            this.thisPackage = package;
            List<string> parts = new();

            string cache = instance.CacheFiles.Where(c => c.EndsWith("localthumbcache.package")).FirstOrDefault();
            
            if (File.Exists(cache))
            {   
                TransformImages imageTransformations = new();
                FileStream fs = new FileStream(cache, FileMode.Open, FileAccess.Read);
                BinaryReader readFile = new BinaryReader(fs);
                List<IndexEntry> indexData = new();

                long entrycountloc = 36;
                long indexRecordPositionloc = 64;

                readFile.BaseStream.Position = 0;

                readFile.BaseStream.Position = entrycountloc;

                uint entrycount = readFile.ReadUInt32();
                if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("P: {0}- THUMBCACHE Entry Count: {1}", packagename, entrycount.ToString()));
                
                //record position low
                uint indexRecordPositionLow = readFile.ReadUInt32();

                //index record size
                uint indexRecordSize = readFile.ReadUInt32();
                
                readFile.BaseStream.Position = indexRecordPositionloc;

                ulong indexRecordPosition = readFile.ReadUInt64();                
                            
                byte[] headersize = new byte[96];
                long here = 100;
                long movedto = 0;


                if (indexRecordPosition != 0){
                    long indexseek = (long)indexRecordPosition - headersize.Length;
                    movedto = here + indexseek;
                    readFile.BaseStream.Position = here + indexseek;                
                } else {
                    movedto = here + indexRecordPositionLow;
                    readFile.BaseStream.Position = here + indexRecordPositionLow;
                }

                readFile.BaseStream.Position = (long)indexRecordPosition + 4;

                for (int i = 0; i < entrycount; i++){                    
                    IndexEntry holderEntry = new IndexEntry();                
                    holderEntry.TypeID = readFile.ReadUInt32().ToString("X8");

                    EntryType type = Sims4PackageReader.EntryTypes.Where(x => x.TypeID == holderEntry.TypeID).First();
                    
                    holderEntry.GroupID = readFile.ReadUInt32().ToString("X8");
                    
                    string instanceid1 = (readFile.ReadUInt32() << 32).ToString("X8");
                    string instanceid2 = (readFile.ReadUInt32() << 32).ToString("X8");
                    holderEntry.InstanceID = string.Format("{0}{1}", instanceid1,instanceid2);
                    
                    uint testin = readFile.ReadUInt32();
                    holderEntry.uLocation = testin;

                    holderEntry.Filesize = readFile.ReadUInt32();

                    holderEntry.Truesize = readFile.ReadUInt32();

                    holderEntry.CompressionType = readFile.ReadUInt16().ToString("X4");

                    readFile.BaseStream.Position = readFile.BaseStream.Position + 2;

                    indexData.Add(holderEntry);
                }

                foreach (string id in instanceids){
                            
                    var thumb = indexData.Where(i => i.InstanceID == id).FirstOrDefault();
                    if (thumb != null){
                        readFile.BaseStream.Position = thumb.uLocation;
                        if (thumb.CompressionType == "5A42"){  
                            int entryEnd = (int)readFile.BaseStream.Position + (int)thumb.Truesize;
                            MemoryStream decomps = Sims4Decryption.DecompressMS(ByteReaders.ReadEntryBytes(readFile, (int)thumb.Truesize));
                            byte[] imagebyte = decomps.ToArray();
                            
                            //string byteasstring = Convert.ToBase64String(imagebyte);
                            imageTransformations.TransformToPNG(imagebyte, packagename);
                            thisPackage.ThumbnailData = imagebyte;
                            fs.Close();
                            fs.Dispose();
                            readFile.Close();
                            readFile.Dispose(); 
                            return;
                        } else if (thumb.CompressionType == "0000"){
                            byte[] imagebyte = ByteReaders.ReadEntryBytes(readFile, (int)thumb.Truesize);
                            imageTransformations.TransformToPNG(imagebyte, packagename);
                            thisPackage.ThumbnailData = imagebyte;
                            fs.Close();
                            fs.Dispose();
                            readFile.Close();
                            readFile.Dispose(); 
                            return;
                        }
                    }
                }
            fs.Close();
            fs.Dispose();
            readFile.Close();
            readFile.Dispose(); 
            }            
        }        
    }
    

    
    public struct ResourceKeyITG {
        /// <summary>
        /// Reads resource keys and does not flip the instance ID.
        /// </summary>
        public ulong instance;
        public uint type;
        public uint group;
        
        public ResourceKeyITG(BinaryReader reader){
            this.instance = reader.ReadUInt64();
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("GUID Instance: {0}", this.instance));
            this.type = reader.ReadUInt32();
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("GUID Type: {0}", this.type));
            this.group = reader.ReadUInt32();
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("GUID Group: {0}", this.group));
        }

        public override string ToString() => $"{type.ToString("X8")}-{group.ToString("X8")}-{instance.ToString("X16")}";
        
    }
    public struct ResourceKeyITGFlip {
        /// <summary>
        /// Reads resource keys and flips the instance ID.
        /// </summary>
        public ulong instance;
        public uint type;
        public uint group;
        
        public ResourceKeyITGFlip(BinaryReader reader){
            uint left = reader.ReadUInt32();
            uint right = reader.ReadUInt32();
            ulong longleft = left;
            longleft = (longleft << 32);
            this.instance = longleft | right;
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("GUID Instance: {0}", this.instance));
            this.type = reader.ReadUInt32();
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("GUID Type: {0}", this.type));
            this.group = reader.ReadUInt32();
            if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("GUID Group: {0}", this.group));
        }

        public override string ToString() => $"{type.ToString("X8")}-{group.ToString("X8")}-{instance.ToString("X16")}";
    }

    public struct Tag {
        /// <summary>
        /// Reads tags.
        /// </summary>
        public int[] tagKey;  
        public int[] empty; 

        public Tag(BinaryReader reader, uint count){
            if (count == 1){
                this.tagKey = new int[count];
                this.empty = new int[count];
                this.tagKey[0] = reader.ReadUInt16();
                this.empty[0] = 0;
            } else {
                this.tagKey = new int[count];
                this.empty = new int[count];
                for (int i = 0; i < count; i++){
                    this.tagKey[i] = reader.ReadUInt16(); 
                    this.empty[i] = reader.ReadUInt16(); 
                }
            }
        }
    }
    public struct CASTag16Bit {
        /// <summary>
        /// Gets CAS Tags from package. 16bit version.
        /// </summary>
        public int[] tagKey;  
        public int[] catKey;
        public int[] empty; 

        public CASTag16Bit(BinaryReader reader, uint count){
            if (count == 1){
                this.tagKey = new int[count];
                this.catKey = new int[count];
                this.empty = new int[count];
                this.empty[0] = reader.ReadUInt16();
                this.catKey[0] = reader.ReadUInt16();
                this.tagKey[0] = reader.ReadUInt16();
                
            } else {
                this.tagKey = new int[count];
                this.catKey = new int[count];
                this.empty = new int[count];
                for (int i = 0; i < count; i++){
                    this.empty[i] = reader.ReadUInt16();
                    this.catKey[i] = reader.ReadUInt16(); 
                    this.tagKey[i] = reader.ReadUInt16(); 
                     
                }
            }
        }
    }

    public struct CASTag32Bit {
        /// <summary>
        /// Gets CAS Tags from package. 32bit version.
        /// </summary>
        public uint[] tagKey;  
        public uint[] catKey;
        public uint[] empty; 

        public CASTag32Bit(BinaryReader reader, int count){
            if (count == 1){
                this.tagKey = new uint[count];
                this.catKey = new uint[count];
                this.empty = new uint[count];
                this.empty[0] = reader.ReadUInt32();
                this.catKey[0] = reader.ReadUInt32();
                this.tagKey[0] = reader.ReadUInt32();
                
            } else {
                this.tagKey = new uint[count];
                this.catKey = new uint[count];
                this.empty = new uint[count];
                for (int i = 0; i < count; i++){
                    this.empty[i] = reader.ReadUInt32();
                    this.catKey[i] = reader.ReadUInt32(); 
                    this.tagKey[i] = reader.ReadUInt32(); 
                     
                }
            }
        }
    }    

    public class Readers {
        public static List<S4CategoryTag> GetTagInfo(CASTag16Bit tags, uint count){
            
            List<S4CategoryTag> taglist = new();
            S4CategoryTag tagg = new();
            List<S4CategoryTag> tagQuery = new();
            for (int i = 0; i < count; i++)
            {   
                if (tags.tagKey[i] != 0){
                    tagQuery = Sims4PackageReader.CategoryTags.Where(x => x.TypeID == tags.tagKey[i].ToString()).ToList();
                    if (tagQuery.Any()){
                        tagg = tagQuery[0];
                        taglist.Add(tagg);
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Tag {0} ({1}) was matched to {2}.", i, tagg.TypeID, tagg.Description));
                    } else {
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Tag {0} matched nothing.", tags.tagKey[i]));
                    }  
                }
                if (tags.catKey[i] != 0){
                    tagQuery = Sims4PackageReader.CategoryTags.Where(x => x.TypeID == tags.catKey[i].ToString()).ToList();                    
                    if (tagQuery.Any()){
                        tagg = tagQuery[0];
                        taglist.Add(tagg);
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Tag {0} ({1}) was matched to {2}.", i, tagg.TypeID, tagg.Description));
                    } else {
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Tag {0} matched nothing.", tags.catKey[i]));
                    }  
                }                                            
            }
            tagg = new();
            return taglist;
        }
        
        public static List<S4CategoryTag> GetTagInfo(Tag tags, uint count){
            List<S4CategoryTag> taglist = new();
            S4CategoryTag tagg = new();
            List<S4CategoryTag> tagQuery = new();
            for (int i = 0; i < count; i++)
            {
                tagQuery = Sims4PackageReader.CategoryTags.Where(x => x.TypeID == tags.tagKey[i].ToString()).ToList();
                if (tagQuery.Any()){
                    tagg = tagQuery[0];
                    taglist.Add(tagg);
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Tag {0} ({1}) was matched to {2}.", i, tags.tagKey[i], tagg.Description));
                } else {
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Tag {0} matched nothing.", tags.tagKey[i]));
                }
            }
            return taglist;
        }        
        
        public static List<S4CategoryTag> GetTagInfo(List<uint> tags, uint count){
            List<S4CategoryTag> taglist = new();
            S4CategoryTag tagg = new();
            List<S4CategoryTag> tagQuery = new();            
            for (int i = 0; i < count; i++)
            {
                tagQuery = Sims4PackageReader.CategoryTags.Where(x => x.TypeID == tags[i].ToString()).ToList();
                if (tagQuery.Any()){
                    tagg = tagQuery[0];
                    taglist.Add(tagg);
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Tag {0} ({2}) was matched to {1}.", i, taglist[i].Description, tags[i]));
                } else {
                        if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Tag {0} matched nothing.", taglist[i]));
                }
            }
            return taglist;
        }
    }

    





}
