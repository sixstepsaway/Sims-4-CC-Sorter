using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SSAGlobals;
using SimsCCManager.Packages.Containers;

namespace SimsCCManager.SortingUIOptions
{
    /// <summary>
    /// Options window for sorting. WIP. 
    /// </summary>
    public partial class SortingOptionsWindow : Window 
    {

        private void GetOptions() {
            
        } 
        private void SpecialCharacters_Check(object sender, RoutedEventArgs e){

        }

        private void SortFunction_Check(object sender, RoutedEventArgs e){

        }

        private void RenameS2_Check(object sender, RoutedEventArgs e){

        }

        private void UnpackS2_Check(object sender, RoutedEventArgs e){

        }

        private void SortGame_Check(object sender, RoutedEventArgs e){

        }

        private void SortCreator_Check(object sender, RoutedEventArgs e){

        }

        private void UnzipZips_Check(object sender, RoutedEventArgs e){

        }

        private void UnpackS3_Check(object sender, RoutedEventArgs e){

        }
        private void Unmerge_Check(object sender, RoutedEventArgs e){

        }
        private void MoveMerged_Check(object sender, RoutedEventArgs e){

        }
        
    }
    public class SortFolderOptions
    {
        public bool SpecialCharacters;
        public bool SortbyFunction;
        public bool RenameS2;
        public bool UnpackS2;
        public bool SortbyGame;
        public bool SortbyCreator;
        public bool UnzipZips;
        public bool UnpackS3;
       
       
       
       
       
    }

    
}