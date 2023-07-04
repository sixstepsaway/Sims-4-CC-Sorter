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
using SimsCCManager.App;
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
            System.Threading.Thread.Sleep(1000);            
            List<Action> tasklist = new List<Action>();
            List<PackageFile> allfiles = new List<PackageFile>();
            int priorPackages = new();
            bool nodata = false;
            tasklist.Add(new Action (() => {
                if (!File.Exists(GlobalVariables.PackagesRead)){
                    nodata = true;                     
                } else {
                    FileInfo pr = new FileInfo(GlobalVariables.PackagesRead);
                    if (pr.Length <= 0) {
                        nodata = true; 
                    }
                }
            }));
            
            tasklist.Add(new Action (() => {
                this.Dispatcher.Invoke(new Action(() => LoadingText.Content = "Initializing Components"));
                GlobalVariables.Initialize(LoggingGlobals.internalLogFolder);                
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
                string cachefolder = System.IO.Path.Combine(LoggingGlobals.mydocs, @"Sims CC Manager\cache");
                GlobalVariables.caches.Add(new CacheLocations{ CacheName = "localthumbcache.package", CacheLocation = System.IO.Path.Combine(LoggingGlobals.mydocs, @"Electronic Arts\The Sims 4\localthumbcache.package"), CacheRename = System.IO.Path.Combine(cachefolder, @"S4_localthumbcache.package")});
                GlobalVariables.caches.Add(new CacheLocations{ CacheName = "CASThumbnails.package", CacheLocation = System.IO.Path.Combine(LoggingGlobals.mydocs, @"Electronic Arts\The Sims 3\Thumbnails\CASThumbnails.package"), CacheRename = System.IO.Path.Combine(cachefolder, @"S3_CASThumbnails.package")});
                GlobalVariables.caches.Add(new CacheLocations{ CacheName = "ObjectThumbnails.package", CacheLocation = System.IO.Path.Combine(LoggingGlobals.mydocs, @"Electronic Arts\The Sims 3\Thumbnails\ObjectThumbnails.package"), CacheRename = System.IO.Path.Combine(cachefolder, @"S3_ObjectThumbnails.package")});
                GlobalVariables.caches.Add(new CacheLocations{ CacheName = "CASThumbnails.package", CacheLocation = System.IO.Path.Combine(LoggingGlobals.mydocs, @"EA Games\The Sims 2 Ultimate Collection\Thumbnails\BuildModeThumbnails.package"), CacheRename = System.IO.Path.Combine(cachefolder, @"S2_CASThumbnails.package")});
                GlobalVariables.caches.Add(new CacheLocations{ CacheName = "ObjectThumbnails.package", CacheLocation = System.IO.Path.Combine(LoggingGlobals.mydocs, @"EA Games\The Sims 2 Ultimate Collection\Thumbnails\CASThumbnails.package"), CacheRename = System.IO.Path.Combine(cachefolder, @"S2_ObjectThumbnails.package")});
                GlobalVariables.caches.Add(new CacheLocations{ CacheName = "BuildModeThumbnails.package", CacheLocation = System.IO.Path.Combine(LoggingGlobals.mydocs, @"EA Games\The Sims 2 Ultimate Collection\Thumbnails\ObjectThumbnails.package"), CacheRename = System.IO.Path.Combine(cachefolder, @"S2_BuildModeThumbnails.package")});
                
                foreach (CacheLocations cache in GlobalVariables.caches){
                    
                    string location = System.IO.Path.Combine(LoggingGlobals.mydocs, System.IO.Path.Combine(@"Sims CC Manager\cache", cache.CacheRename));
                    string unnamedversion = System.IO.Path.Combine(LoggingGlobals.mydocs, System.IO.Path.Combine(@"Sims CC Manager\cache", cache.CacheName));
                    if (File.Exists(cache.CacheLocation)) {
                        log.MakeLog(string.Format("Cache '{0}' exists. Retrieving.", cache.CacheName), true);
                        if (File.Exists(cache.CacheRename)){
                            File.Delete(cache.CacheRename);                            
                        }
                        File.Copy(cache.CacheLocation, cache.CacheRename);
                    } else {
                        log.MakeLog(string.Format("Cache '{0}' does not exist.", cache.CacheName), true);
                    }
                }
            }));
            tasklist.Add(new Action (() => {
                if (nodata == false) {
                    log.MakeLog("Checking for prior data.", true);
                    priorPackages = GlobalVariables.DatabaseConnection.ExecuteScalar<int>("select count(PackageName) from Packages");
                    this.Dispatcher.Invoke(new Action(() => LoadingText.Content = "Checking for Data"));
                } else {
                    log.MakeLog("No data to check.", true);
                }                
            }));

            tasklist.Add(new Action (() => {
                if (File.Exists(GlobalVariables.SettingsFile)) {
                    log.MakeLog("Finding Settings.", true);
                    this.Dispatcher.Invoke(new Action(() => LoadingText.Content = "Setting Settings"));
                    globals.LoadSettings();
                    var s2 = GlobalVariables.Settings.Where(x => x.SettingName == "Sims2Folder").ToList();
                    if (s2.Any()){
                        GlobalVariables.Sims2DocumentsFolder = s2[0].SettingValue;
                    }
                    var s3 = GlobalVariables.Settings.Where(x => x.SettingName == "Sims3Folder").ToList();
                    if (s3.Any()){
                        GlobalVariables.Sims3DocumentsFolder = s3[0].SettingValue;
                    }
                    var s4 = GlobalVariables.Settings.Where(x => x.SettingName == "Sims4Folder").ToList();
                    if (s4.Any()){
                        GlobalVariables.Sims4DocumentsFolder = s4[0].SettingValue;
                    }
                    var cpu = GlobalVariables.Settings.Where(x => x.SettingName == "RestrictCPU").ToList();
                    if (cpu.Any()){
                        if (cpu[0].SettingValue == "True"){
                            GlobalVariables.eatentirecpu = false;
                        } else {
                            GlobalVariables.eatentirecpu = true;
                        }
                    }
                    var lfu = GlobalVariables.Settings.Where(x => x.SettingName == "LastFolder").ToList();
                    if (lfu.Any()){
                        GlobalVariables.LastFolderUsed = lfu[0].SettingValue;
                    }
                } else {
                    log.MakeLog("No settings found.", true);
                }                
            }));

            tasklist.Add(new Action (() => {
                if (nodata == false) {
                    if (priorPackages >= 1) 
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
                    try {
                        var mainWindow = new MainWindow();
                        mainWindow.Show();
                    } catch (Exception e) {
                        Console.WriteLine("Caught exception opening Main Window: " + e.Message + " - " + e.StackTrace);
                    }


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