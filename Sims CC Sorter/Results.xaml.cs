using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Diagnostics;

namespace FindBrokenPackages {

    public class PackageFiles {
        public string Name {get; set;}
        public string Location {get; set;}
        public int Number {get; set;}
        public int Version {get; set;}
    }

    public partial class Results : Window {
        public Results() 
        {
            
        }
    }   
}