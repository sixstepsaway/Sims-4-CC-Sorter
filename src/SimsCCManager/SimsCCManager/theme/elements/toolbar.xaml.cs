using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using SimsCCManager.UI.Utilities;
using System.Windows.Shapes;
using System.Windows.Media;
using SSAGlobals;

namespace SimsCCManager.Themes.UIElements
{
    public partial class Toolbar : ResourceDictionary
    {        
        double smallsizew = 0;
        double smallsizeh = 0;
        double fullwidth = SystemParameters.PrimaryScreenWidth;
        double fullheight = SystemParameters.PrimaryScreenHeight;

        public void ToolbarClickNoMax(object sender, MouseButtonEventArgs e){
            Grid grid = sender as Grid;
            Window window = grid.Tag as Window;
            window.DragMove();
        }

        public void ToolbarClick(object sender, MouseButtonEventArgs e)
        {
            Grid grid = sender as Grid;
            Window window = grid.Tag as Window;
            window.DragMove();
            Rectangle LocationRect = grid.Children.OfType<Rectangle>().First(x => x.Name == "LocationRect");
            Rectangle MaximizedRect = grid.Children.OfType<Rectangle>().First(x => x.Name == "MaximizedRect");
            Rectangle OriginalSizeRect = grid.Children.OfType<Rectangle>().First(x => x.Name == "OriginalSizeRect");
            Rectangle ResizeRect = grid.Children.OfType<Rectangle>().First(x => x.Name == "ResizeRect");
            smallsizew = OriginalSizeRect.Width;
            smallsizeh = OriginalSizeRect.Height;

            if (e.ChangedButton == MouseButton.Left){
                if(e.ClickCount >= 2)
                {
                    if (window.WindowState == WindowState.Maximized){
                        window.WindowState = WindowState.Normal;                
                        ResizeRect.Width = OriginalSizeRect.Width;
                        ResizeRect.Height = OriginalSizeRect.Height; 
                        //ResizeRect.Tag = 5; 
                        LocationRect.Tag = new Point(SystemParameters.MaximizedPrimaryScreenWidth / 4,SystemParameters.MaximizedPrimaryScreenHeight / 4);
                    } else {
                        ResizeRect.Width = SystemParameters.MaximizedPrimaryScreenWidth;
                        ResizeRect.Height = SystemParameters.MaximizedPrimaryScreenHeight; 
                        //ResizeRect.Tag = 0;
                        window.WindowState = WindowState.Maximized; 
                        LocationRect.Tag = new Point(0,0);                       
                    }
                } else {
                    if (e.ButtonState == MouseButtonState.Pressed && window.WindowState == WindowState.Normal){
                        window.DragMove();
                    };                                                      
                }
            }
        }

        void Minimize_Click(object sender, RoutedEventArgs e)
        {
            Button miniButton = sender as Button;
            Window window = miniButton.Tag as Window;
            window.WindowState = WindowState.Minimized;
        }
        void Maximize_Click(object sender, RoutedEventArgs e)
        {
            Button miniButton = sender as Button;
            Window window = miniButton.Tag as Window;
            StackPanel sp = VisualTreeHelper.GetParent(miniButton) as StackPanel;
            Grid grid1 = VisualTreeHelper.GetParent(sp) as Grid;
            Grid grid = VisualTreeHelper.GetParent(grid1) as Grid;
            Rectangle LocationRect = grid.Children.OfType<Rectangle>().First(x => x.Name == "LocationRect");
            Rectangle ResizeRect = grid.Children.OfType<Rectangle>().First(x => x.Name == "ResizeRect");
            Rectangle OriginalSizeRect = grid.Children.OfType<Rectangle>().First(x => x.Name == "OriginalSizeRect");

            if (window.WindowState == WindowState.Maximized){                
                window.WindowState = WindowState.Normal;   
                //ResizeRect.Tag = 5;             
                ResizeRect.Width = OriginalSizeRect.Width;
                ResizeRect.Height = OriginalSizeRect.Height;  
                LocationRect.Tag = new Point(SystemParameters.MaximizedPrimaryScreenWidth / 4,SystemParameters.MaximizedPrimaryScreenHeight / 4);  
                
            } else {
                ResizeRect.Width = SystemParameters.MaximizedPrimaryScreenWidth;
                ResizeRect.Height = SystemParameters.MaximizedPrimaryScreenHeight; 
                //ResizeRect.Tag = 0;
                LocationRect.Tag = new Point(0,0);   
                window.WindowState = WindowState.Maximized;               
            }    
        }
        void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.WindowsOpen--;
            Button miniButton = sender as Button;
            Window window = miniButton.Tag as Window;
            window.Close();
        }             
    }
}