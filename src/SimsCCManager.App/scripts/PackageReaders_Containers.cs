using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimsCCManager.PackageReaders.Containers
{
    public class IndexEntry
    {
        public string Tag {get; set;} = "";
        public int ID {get; set;} = -1;
        public string TypeID {get; set;} = "";
        public string GroupID {get; set;} = "";
        public string InstanceID {get; set;} = "";
        public string InstanceID2 {get; set;} = "";
        public uint Offset {get; set;} = 0;
        public uint Filesize {get; set;} = 0;
        public uint Truesize {get; set;} = 0;
        public bool Compressed {get; set;} = false;
        public string CompressionType {get; set;} = "";
        public string Unused {get; set;} = "";
        public string Size {get; set;} = "";
        public int Location {get; set;} = -1;
        public uint uLocation {get; set;}
        public long LongLocation {get; set;} = -1;
        public string PackageID {get; set;} = "";
    }	

    public class EntryLocations {
        public string TypeID {get; set;} = "";
        public string Description {get; set;} = "";
        public int Location {get; set;} = -1;
    }

    public class EntryType {
        /// <summary>
        /// For "types", for example Cas Parts or Geometry.
        /// </summary>
        public string Tag {get; set;} = "";
        public string TypeID {get; set;} = "";
        public string Description {get; set;} = "";
    }

    public class S4Function{
        public string BodyType {get; set;} = "";
        public string Function {get; set;} = "";
        public string Subfunction {get; set;} = "";
    }

    public class S4CategoryTag{
        public string TypeID {get; set;} = "";
        public string Description {get; set;} = "";
        public string Function {get; set;} = "";
        public string Subfunction {get; set;} = "";
    }

    public class PackageTypeCounter {
        public string Description {get; set;}
        public string TypeID {get; set;}
        public int Count {get; set;}
    }

    public class S4AgeGenderFlags {
        /// <summary>
        /// Age and Gender flags for Sims 4 CAS items.
        /// </summary>
        public bool Adult {get; set;}
        public bool Baby {get; set;}
        public bool Child {get; set;}
        public bool Elder {get; set;}
        public bool Infant {get; set;}
        public bool Teen {get; set;}
        public bool Toddler {get; set;}
        public bool YoungAdult {get; set;}
        public bool Female {get; set;}
        public bool Male {get; set;}

        public bool Any(){
            if (this.Adult == true || this.Baby == true || this.Infant == true || this.Child == true || this.Elder == true || this.Teen == true || this.YoungAdult == true || this.Female == true || this.Male == true){
                return true;
            } else {
                return false;
            }
        }
    } 

    public class SimsOverride {
        public string ItemName {get; set;} = "";
        public string InstanceID {get; set;} = "";
        public string TypeID {get; set;} = "";
        public string GroupID {get; set;} = "";
        public string Description {get; set;} = "";
    }

    public class OverriddenList {
        public string InstanceID {get; set;} = "";
        public string GroupID {get; set;} = "";
        public string TypeID {get; set;} = "";
        public string Name {get; set;} = "";
        public string Type {get; set;} = "";
        public string Pack {get; set;} = "";
        public string Override {get; set;} = "";

        public static string OverridesListToString(List<OverriddenList> overriddenLists){
            StringBuilder strings = new();
            foreach (OverriddenList overriddenList in overriddenLists){
                strings.AppendLine(string.Format("Name: {0}, ID: {1}, Pack: {2}", overriddenList.Name, overriddenList.Override, overriddenList.Pack));
            }
            return strings.ToString();
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, Pack: {1}, Override: {2}", this.Name, this.Pack, this.Override);
        }
    }
}