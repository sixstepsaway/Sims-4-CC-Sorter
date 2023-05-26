using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
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
        
        double barvalue = 100;
        public SplashScreenWindow(){
            InitializeComponent();
            SplashProgressBar.Value = 100;
            //this.Loaded += new RoutedEventHandler(SplashScreenWindow_Loaded);
        }

        void SplashScreenWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => {
                Splash();
            });            
        }   

        void Splash(){
            System.Threading.Thread.Sleep(3000);            
            List<Action> tasklist = new List<Action>();
            List<SimsPackage> ppackages = new List<SimsPackage>();
            bool nodata = false;
            tasklist.Add(new Action (() => {
                if (!File.Exists(GlobalVariables.PackagesRead)){
                    nodata = true;                     
                }else {
                    FileInfo pr = new FileInfo(GlobalVariables.PackagesRead);
                    if (pr.Length <= 0) {
                        nodata = true; 
                    }
                }             
            }));
            tasklist.Add(new Action (() => {
                this.Dispatcher.Invoke(new Action(() => LoadingText.Content = "Initializing Components"));
                globals.Initialize(LoggingGlobals.internalLogFolder);                
            }));
            tasklist.Add(new Action (() => {
                this.Dispatcher.Invoke(new Action(() => LoadingText.Content = "Connecting To Internal Database"));
                globals.ConnectDatabase(false);                       
            }));
            tasklist.Add(new Action (() => {
                this.Dispatcher.Invoke(new Action(() => LoadingText.Content = "Initializing Internal Database"));
                globals.InitializeVariables();                       
            }));
            tasklist.Add(new Action (() => {
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
            }));
            tasklist.Add(new Action (() => {
                if (nodata == false) {
                    log.MakeLog("Checking for prior data.", true);
                    var priorPackages = GlobalVariables.DatabaseConnection.Query<SimsPackage>("SELECT * FROM Packages");
                    ppackages.AddRange(priorPackages);
                    this.Dispatcher.Invoke(new Action(() => LoadingText.Content = "Checking for Data"));
                } else {
                    log.MakeLog("No data to check.", true);
                }
                
            }));
            tasklist.Add(new Action (() => {
                if (nodata == false) {
                    if (ppackages.Count >= 1) 
                    {
                        log.MakeLog("Data found!", true);
                        MainWindow.dataExists = true;
                    } else {
                        log.MakeLog("No data was found.", true);
                    }
                    this.Dispatcher.Invoke(new Action(() => LoadingText.Content = "Checking for Data"));
                } else {
                    log.MakeLog("No data to check.", true);
                }               
                
            }));
            tasklist.Add(new Action (() => {
                log.MakeLog("Loading actual application.", true);
                this.Dispatcher.Invoke(new Action(() => LoadingText.Content = "Loading Manager"));
                this.Dispatcher.Invoke(new Action(() => {
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                }));
                this.Dispatcher.Invoke(new Action(() => this.Close()));
            }));
            foreach (Action a in tasklist){  
                int smoother = 100;                        
                for (int i = 0; i < smoother; i++){
                    Increment(tasklist.Count, smoother);
                }
                a.Invoke();
            }
        } 
        void Increment(int tasks, int smoother){
            double barincrement = (double)100 / ((double)tasks * (double)smoother);            
            //log.MakeLog(string.Format("Bar Increment: {0}", barincrement.ToString()), true);
            barvalue = barvalue - barincrement; 
            //log.MakeLog(string.Format("Bar Value: {0}", barvalue.ToString()), true);
            this.Dispatcher.Invoke(new Action(() => SplashProgressBar.Value = barvalue));
        }
    }
}