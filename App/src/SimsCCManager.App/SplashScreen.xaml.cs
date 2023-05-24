using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
using System.IO;
using SSAGlobals;
using Sims_CC_Sorter;
using SimsCCManager.Packages.Containers;
using System.Collections.Concurrent;

namespace SimsCCManager.SplashScreen
{
    public partial class SplashScreenWindow : Window
    {
        GlobalVariables globals = new GlobalVariables();
        LoggingGlobals log = new LoggingGlobals();
        double barvalue = 0;
        public SplashScreenWindow(){
            InitializeComponent();
            SplashProgressBar.Value = 0;
            
            for (int i = 0; i < 100; i++){
                Task.Factory.StartNew(() =>
                {
                    barvalue++; 
                    this.Dispatcher.Invoke(new Action(() => SplashProgressBar.Value = 100 - barvalue));                
                });                
            }
            Task.Factory.StartNew(() =>
            {
                this.Dispatcher.Invoke(new Action(() => LoadingText.Content = "Initializing Components"));
                log.InitializeLog();
                
                this.Dispatcher.Invoke(new Action(() => LoadingText.Content = "Connecting To Internal Database"));
                globals.ConnectDatabase(false);
                
                this.Dispatcher.Invoke(new Action(() => LoadingText.Content = "Acquiring Caches"));
                //if file exists, get it. if we already have it, delete it then and replace it
                List<CacheLocations> caches = new List<CacheLocations>();
                caches.Add(new CacheLocations{ CacheName = "localthumbcache.package", CacheLocation = System.IO.Path.Combine(LoggingGlobals.mydocs, "\\Electronic Arts\\The Sims 4\\localthumbcache.package"), CacheRename = "S4_localthumbcache.package" });
                caches.Add(new CacheLocations{ CacheName = "CASThumbnails.package", CacheLocation = System.IO.Path.Combine(LoggingGlobals.mydocs, "\\Electronic Arts\\The Sims 3\\Thumbnails\\CASThumbnails.package"), CacheRename = "S3_CASThumbnails.package" });
                caches.Add(new CacheLocations{ CacheName = "ObjectThumbnails.package", CacheLocation = System.IO.Path.Combine(LoggingGlobals.mydocs, "\\Electronic Arts\\The Sims 3\\Thumbnails\\ObjectThumbnails.package"), CacheRename = "S3_ObjectThumbnails.package" });
                caches.Add(new CacheLocations{ CacheName = "CASThumbnails.package", CacheLocation = System.IO.Path.Combine(LoggingGlobals.mydocs, "\\EA Games\\The Sims 2 Ultimate Collection\\Thumbnails\\BuildModeThumbnails.package"), CacheRename = "S2_CASThumbnails.package" });
                caches.Add(new CacheLocations{ CacheName = "ObjectThumbnails.package", CacheLocation = System.IO.Path.Combine(LoggingGlobals.mydocs, "\\EA Games\\The Sims 2 Ultimate Collection\\Thumbnails\\CASThumbnails.package"), CacheRename = "S2_ObjectThumbnails.package" });
                caches.Add(new CacheLocations{ CacheName = "BuildModeThumbnails.package", CacheLocation = System.IO.Path.Combine(LoggingGlobals.mydocs, "\\EA Games\\The Sims 2 Ultimate Collection\\Thumbnails\\ObjectThumbnails.package"), CacheRename = "S2_BuildModeThumbnails.package" });

                foreach (CacheLocations cache in caches){
                    string cachefolder = System.IO.Path.Combine(LoggingGlobals.mydocs, "\\Sims CC Manager\\cache");
                    string location = System.IO.Path.Combine(LoggingGlobals.mydocs, System.IO.Path.Combine("\\Sims CC Manager\\cache\\", cache.CacheRename));
                    string unnamedversion = System.IO.Path.Combine(LoggingGlobals.mydocs, System.IO.Path.Combine("\\Sims CC Manager\\cache\\", cache.CacheName));
                    if (File.Exists(cache.CacheLocation)) {
                        log.MakeLog(string.Format("Cache '{0}' exists. Retrieving.", cache.CacheName), true);
                        if (File.Exists(location)){
                            File.Delete(location);
                            File.Copy(cache.CacheLocation, cachefolder);
                            Microsoft.VisualBasic.FileIO.FileSystem.RenameFile(unnamedversion, cache.CacheRename);
                        }
                    } else {
                        log.MakeLog(string.Format("Cache '{0}' does not exist.", cache.CacheName), true);
                    }
                }
                
                log.MakeLog("Checking for prior data.", true);
                var priorPackages = GlobalVariables.DatabaseConnection.Query<SimsPackage>("SELECT * FROM Packages");
                this.Dispatcher.Invoke(new Action(() => LoadingText.Content = "Checking for Data"));
                if (priorPackages.Count >= 1) 
                {
                    MainWindow.dataExists = true;
                }
                this.Dispatcher.Invoke(new Action(() => LoadingText.Content = "Checking for Data"));

                log.MakeLog("Loading actual application.", true);
                var mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            });

        }

        
    }
}