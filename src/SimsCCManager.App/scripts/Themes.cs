using System;
using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using System.Drawing;
using Skybrud.Colors;
using System.IO;
using SimsCCManager.Globals;
using System.Reflection;
using SimsCCManager.Debugging;
using System.Text;

namespace SimsCCManager.UI.Themes
{
    public class Themes
    {
        public static string ThemeFolder = Path.Combine(GlobalVariables.AppFolder, "themes");
        public static ThemeColors DefaultDark = new(){
            ThemeName = "Default Dark",
            Identifier = Guid.Parse("4305e7e5-0a03-4625-a4ab-2a688ae9d9e3"),
            BackgroundColor = Godot.Color.FromHtml("262529"),
            ButtonMain = Godot.Color.FromHtml("8688a9"),
            ButtonHover = Godot.Color.FromHtml("ccabd8"),
            ButtonClick = Godot.Color.FromHtml("e0cce7"),
            DataGridA = Godot.Color.FromHtml("EBEDEF"),
            DataGridB = Godot.Color.FromHtml("C9D1D9"),
            DataGridSelected = Godot.Color.FromHtml("89AEDB"),
            AccentColor = Godot.Color.FromHtml("94D183"),
            DataGridTextA = Godot.Color.FromHtml("3B424A"),
            DataGridTextB = Godot.Color.FromHtml("2F3842"),
            MainTextColor = Godot.Color.FromHtml("D5D1EB"),
            HeaderTextColor = Godot.Color.FromHtml("D9ABD6")
        };
        public static ThemeColors Blossom = new(){
            ThemeName = "Blossom",
            Identifier = Guid.Parse("a92902ea-3d8c-44ec-b858-b6111c0afa50"),
            BackgroundColor = Godot.Color.FromHtml("EDE3E4"),
            ButtonMain = Godot.Color.FromHtml("AF9FA5"),
            ButtonHover = Godot.Color.FromHtml("E4DEE4"),
            ButtonClick = Godot.Color.FromHtml("E7DFE8"),
            DataGridA = Godot.Color.FromHtml("EAE1E6"),
            DataGridB = Godot.Color.FromHtml("E9E0E7"),
            DataGridSelected = Godot.Color.FromHtml("CABFC5"),
            AccentColor = Godot.Color.FromHtml("C24D76"),
            DataGridTextA = Godot.Color.FromHtml("DAA5C2"),
            DataGridTextB = Godot.Color.FromHtml("AB8AA4"),
            MainTextColor = Godot.Color.FromHtml("3F3536"),
            HeaderTextColor = Godot.Color.FromHtml("9A7E88")
        };

        public static ThemeColors LandOfIce = new(){
            ThemeName = "Land of Ice",
            Identifier = Guid.Parse("c17768b6-0f9f-407f-af95-8b50d3333365"),
            BackgroundColor = Godot.Color.FromHtml("3E442B"),
            ButtonMain = Godot.Color.FromHtml("8D91B5"),
            ButtonHover = Godot.Color.FromHtml("979BBC"),
            ButtonClick = Godot.Color.FromHtml("A0A4C2"),
            DataGridA = Godot.Color.FromHtml("8D909B"),
            DataGridB = Godot.Color.FromHtml("AAADC4"),
            DataGridSelected = Godot.Color.FromHtml("B3D5DA"),
            AccentColor = Godot.Color.FromHtml("6A7062"),
            DataGridTextA = Godot.Color.FromHtml("2E3962"),
            DataGridTextB = Godot.Color.FromHtml("545C8D"),
            MainTextColor = Godot.Color.FromHtml("B8BCAA"),
            HeaderTextColor = Godot.Color.FromHtml("5B6085")
        };

        public static ThemeColors GreenQueen = new(){
            ThemeName = "Green Queen",
            Identifier = Guid.Parse("a330b8f8-8ceb-412f-ad38-755506ce2da2"),
            BackgroundColor = Godot.Color.FromHtml("070c09"),
            ButtonMain = Godot.Color.FromHtml("35593d"),
            ButtonHover = Godot.Color.FromHtml("263f2c"),
            ButtonClick = Godot.Color.FromHtml("17261a"),
            DataGridA = Godot.Color.FromHtml("45724f"),
            DataGridTextA = Godot.Color.FromHtml("72bf83"),
            DataGridB = Godot.Color.FromHtml("548c60"),
            DataGridTextB = Godot.Color.FromHtml("82d895"),
            DataGridSelected = Godot.Color.FromHtml("35593d"),
            AccentColor = Godot.Color.FromHtml("91f2a6"),
            MainTextColor = Godot.Color.FromHtml("b9f2c6"),
            HeaderTextColor = Godot.Color.FromHtml("72bf83")
        };
        public static ThemeColors OldValyria = new(){
            ThemeName = "Old Valyria",
            Identifier = Guid.Parse("3a18f7ef-ca20-47d4-909d-f45373a044b8"),
            BackgroundColor = Godot.Color.FromHtml("171614"),
            ButtonMain = Godot.Color.FromHtml("754043"),
            ButtonHover = Godot.Color.FromHtml("9A8873"),
            ButtonClick = Godot.Color.FromHtml("3A2618"),
            DataGridA = Godot.Color.FromHtml("673A39"),
            DataGridB = Godot.Color.FromHtml("58332E"),
            DataGridSelected = Godot.Color.FromHtml(""),
            AccentColor = Godot.Color.FromHtml("37423D"),
            DataGridTextA = Godot.Color.FromHtml("D48E8D"),
            DataGridTextB = Godot.Color.FromHtml("C5847B"),
            MainTextColor = Godot.Color.FromHtml("968C85"),
            HeaderTextColor = Godot.Color.FromHtml("B86062")
        };
        public static ThemeColors DefaultLight = new(){
            ThemeName = "Default Light",
            Identifier = Guid.Parse("affdd6c9-0b8d-4623-a814-d67e6931994b"),
            BackgroundColor = Godot.Color.FromHtml("B5D3DD"),
            ButtonMain = Godot.Color.FromHtml("EDA2C0"),
            ButtonHover = Godot.Color.FromHtml("BF9ACA"),
            ButtonClick = Godot.Color.FromHtml("8E4162"),
            DataGridA = Godot.Color.FromHtml("C7E8F3"),
            DataGridB = Godot.Color.FromHtml("C3C1DF"),
            DataGridSelected = Godot.Color.FromHtml("C1AED5"),
            AccentColor = Godot.Color.FromHtml("41393E"),
            DataGridTextA = Godot.Color.FromHtml("273A40"),
            DataGridTextB = Godot.Color.FromHtml("272539"),
            MainTextColor = Godot.Color.FromHtml("1F2F35"),
            HeaderTextColor = Godot.Color.FromHtml("D69EC5")
        };

        public static ThemeColors TealSnow = new(){
            ThemeName = "Teal Snow",
            Identifier = Guid.Parse("01541480-e16a-4808-b719-992a69f03867"),
            BackgroundColor = Godot.Color.FromHtml("FFFFFA"),
            ButtonMain = Godot.Color.FromHtml("0D5C63"),
            ButtonHover = Godot.Color.FromHtml("44A1A0"),
            ButtonClick = Godot.Color.FromHtml("78CDD7"),
            DataGridA = Godot.Color.FromHtml("A0DADD"),
            DataGridB = Godot.Color.FromHtml("7ECACA"),
            DataGridSelected = Godot.Color.FromHtml("388787"),
            AccentColor = Godot.Color.FromHtml("6397BF"),
            DataGridTextA = Godot.Color.FromHtml("4F989C"),
            DataGridTextB = Godot.Color.FromHtml("3B9393"),
            MainTextColor = Godot.Color.FromHtml("04282B"),
            HeaderTextColor = Godot.Color.FromHtml("0D5C63")
        };

        /*
        public static ThemeColors GreenQueen = new(){
            ThemeName = "Green Queen",
            Identifier = Guid.NewGuid(),
            BackgroundColor = Godot.Color.FromHtml(""),
            ButtonMain = Godot.Color.FromHtml(""),
            ButtonHover = Godot.Color.FromHtml(""),
            ButtonClick = Godot.Color.FromHtml(""),
            DataGridA = Godot.Color.FromHtml(""),
            DataGridTextA = Godot.Color.FromHtml(""),
            DataGridB = Godot.Color.FromHtml(""),
            DataGridTextB = Godot.Color.FromHtml(""),
            DataGridSelected = Godot.Color.FromHtml(""),
            AccentColor = Godot.Color.FromHtml(""),
            MainTextColor = Godot.Color.FromHtml(""),
            HeaderTextColor = Godot.Color.FromHtml("")
        };
        */



        public static void ReestablishThemes(){
            List<ThemeColors> themes = new List<ThemeColors>
            {
                TealSnow,
                Blossom,
                DefaultDark,
                DefaultLight,
                OldValyria,
                LandOfIce,
                GreenQueen
            };
            foreach (ThemeColors theme in themes){
                StringBuilder stringBuilder = new();
                stringBuilder.AppendLine(string.Format("{0}={1}", "ThemeName", theme.ThemeName));
                stringBuilder.AppendLine(string.Format("{0}={1}", "Identifier", theme.Identifier));
                stringBuilder.AppendLine(string.Format("{0}={1}", "BackgroundColor", theme.BackgroundColor));
                stringBuilder.AppendLine(string.Format("{0}={1}", "ButtonMain", theme.ButtonMain));
                stringBuilder.AppendLine(string.Format("{0}={1}", "ButtonHover", theme.ButtonHover));
                stringBuilder.AppendLine(string.Format("{0}={1}", "ButtonClick", theme.ButtonClick));
                stringBuilder.AppendLine(string.Format("{0}={1}", "DataGridA", theme.DataGridA));
                stringBuilder.AppendLine(string.Format("{0}={1}", "DataGridB", theme.DataGridB));
                stringBuilder.AppendLine(string.Format("{0}={1}", "DataGridSelected", theme.DataGridSelected));
                stringBuilder.AppendLine(string.Format("{0}={1}", "AccentColor", theme.AccentColor));
                stringBuilder.AppendLine(string.Format("{0}={1}", "DataGridTextA", theme.DataGridTextA));
                stringBuilder.AppendLine(string.Format("{0}={1}", "DataGridTextB", theme.DataGridTextB));
                stringBuilder.AppendLine(string.Format("{0}={1}", "MainTextColor", theme.MainTextColor));
                stringBuilder.AppendLine(string.Format("{0}={1}", "HeaderTextColor", theme.HeaderTextColor));
                using (StreamWriter streamWriter = new(Path.Combine(ThemeFolder, string.Format("{0}.ini", theme.ThemeName)))){
                    streamWriter.Write(stringBuilder);
                }
            }
        }

        private static string GetContrastColor(string hexinput){
            System.Drawing.Color color = ColorTranslator.FromHtml(hexinput);
            RgbColor rgb = new(color.R, color.G, color.B);
            double newcolor = 0;
            double luminance = (0.299 * rgb.R + 0.587 * rgb.G + 0.114 * rgb.B)/255;
            if (luminance > 0.5){
                newcolor = 0; // bright colors - black font
            } else {
                newcolor = 255; // dark colors - white font
            }
            RgbColor newc = new()
            {
                Red = (byte)newcolor,
                Blue = (byte)newcolor,
                Green = (byte)newcolor
            };

            return newc.ToHex();
        }
    }

    public class ThemeColors {
        public string ThemeName {get; set;} = "";
        public Guid Identifier {get; set;} = Guid.Empty;
        public Godot.Color BackgroundColor { get; set; } = Godot.Color.Color8(0, 0, 0);
        public Godot.Color ButtonMain {get; set; } = Godot.Color.Color8(0, 0, 0);
        public Godot.Color ButtonHover { get; set; } = Godot.Color.Color8(0, 0, 0);
        public Godot.Color ButtonClick { get; set; } = Godot.Color.Color8(0, 0, 0);
        public Godot.Color DataGridA { get; set; } = Godot.Color.Color8(0, 0, 0);
        public Godot.Color DataGridTextA {get; set;} = Godot.Color.Color8(0, 0, 0);
        public Godot.Color DataGridB { get; set; } = Godot.Color.Color8(0, 0, 0);
        public Godot.Color DataGridTextB {get; set;} = Godot.Color.Color8(0, 0, 0);
        public Godot.Color DataGridSelected { get; set; } = Godot.Color.Color8(0, 0, 0);
        public Godot.Color AccentColor { get; set; } = Godot.Color.Color8(0, 0, 0);
        public Godot.Color MainTextColor { get; set; } = Godot.Color.Color8(0, 0, 0);
        public Godot.Color HeaderTextColor { get; set; } = Godot.Color.Color8(0, 0, 0);

        public dynamic GetProperty(string propName){
            var prop = this.ProcessProperty(propName);
            if (prop.GetType() == typeof(string)){
                return prop.ToString();
            } else if (prop.GetType() == typeof(DateTime)){
                DateTime dt = (DateTime)prop;                
                return dt.ToString("MM/dd/yyyy H:mm");
            } else if (prop.GetType() == typeof(bool)){
                return prop;
            } else if (prop.GetType() == typeof(ThemeColors)){
                return prop;
            } else {
                return "";
            }
        }

        public void SetProperty(string propName, dynamic input){
            Logging.WriteDebugLog(string.Format("Processing property {0}, value {1}", propName, input.GetType()));
            var prop = this.ProcessProperty(propName);
            PropertyInfo property = this.GetType().GetProperty(propName);
            Logging.WriteDebugLog(string.Format("Property type: {0}", property.PropertyType));
            if (property != null){
                if (property.PropertyType == typeof(Godot.Color)){
                    Godot.Color newcolor = Godot.Color.FromHtml(input);
                    property.SetValue(this, newcolor);
                } else if (property.PropertyType == typeof(Guid)){
                    string inp = input as string;
                    property.SetValue(this, Guid.Parse(inp));
                } else if (property.PropertyType == typeof(string)) {
                    property.SetValue(this, input as string);
                }
            }            
        }

        public object ProcessProperty(string propName){
            return this.GetType().GetProperty(propName).GetValue (this, null);
        }
    }
}