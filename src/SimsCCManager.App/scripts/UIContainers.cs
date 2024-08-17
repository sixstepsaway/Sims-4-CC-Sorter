using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimsCCManager.UI.Containers
{
    public class DataGridSizes
    {
        public string Column {get; set;} = "";
        public float Width {get; set;} = 0f;
    }

    public class DataGridHeader
    {
        public string ColumnName {get; set;} = "";
        public DataGridContentType ColumnType {get; set;} = DataGridContentType.Null;
        public string ColumnData {get; set; } = "";
    }

    public class UIConvertors {
        public static DataGridContentType GetDataGridContentType(string input){
            if (input == "Name"){
                return DataGridContentType.Name;
            } else if (input == "Icons"){
                return DataGridContentType.Icons;
            } else if (input == "Date"){
                return DataGridContentType.Date;
            } else if (input == "Enabled"){
                return DataGridContentType.Enabled;
            } else if (input == "Text"){
                return DataGridContentType.Text;
            } else if (input == "Bool"){
                return DataGridContentType.Bool;
            } else {
                return DataGridContentType.Null;
            }
        }
        
        public static string DataGridContentTypeToString (DataGridContentType input){
            if (input == DataGridContentType.Name){
                return "Name";
            } else if (input == DataGridContentType.Icons){
                return "Icons";
            } else if (input == DataGridContentType.Date){
                return "Date";
            } else if (input == DataGridContentType.Text){
                return "Text";
            } else if (input == DataGridContentType.Bool){
                return "Bool";
            } else {
                return "Null";
            }
        }
    }

    
}

public enum DataGridContentType {
    Null,
    Name,
    Icons,
    Date,
    Enabled, 
    Text,
    Bool
}