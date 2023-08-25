using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimsCCManager.Themes
{
    public partial class Toolbar
    {
        void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Grid dragBar = sender as Grid;
            Window window = dragBar.Tag as Window;
            if (e.ChangedButton == MouseButton.Left)            
                window?.DragMove();
        }
        void Minimize_Click(object sender, RoutedEventArgs e)
        {
            Button miniButton = sender as Button;
            Window window = miniButton.Tag as Window;
            window.WindowState = WindowState.Minimized;
        }
        void Maximize_Click(object sender, RoutedEventArgs e)
        {
            //Button miniButton = sender as Button;
            //Window window = miniButton.Tag as Window;
            //Maximize(window);            
        }
        void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Button miniButton = sender as Button;
            Window window = miniButton.Tag as Window;
            window.Close();
        }
    }
}