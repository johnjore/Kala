using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Plugin.Logger;
using FFImageLoading.Svg.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Kala
{
    public class App : Application
    {
        #region Variables
        public class TrackItem
        {
            public Grid Grid { get; set; }
            public string Min { get; set; }
            public string Max { get; set; }
            public string State { get; set; }
            public string Icon { get; set; }
            public string Header { get; set; }
            public string Unit { get; set; }
            public Models.Itemtypes Type { get; set; }
            public string Name { get; set; }
            public SvgCachedImage SvgImage { get; set; }
        }

        public class Configuration
        {
            public Models.Sitemaps.Sitemap Sitemap { get; set; }
            public bool Valid { get; set; } = false;
            public bool Initialized { get; set; } = false;
            public bool Settings { get; set; } = true;
            public long ScreenSaver { get; set; } = -1;
            public bool FullScreen { get; set; } = false;
            public Models.ScreenSaverTypes ScreenSaverType { get; set; } = Models.ScreenSaverTypes.Clock;
            public Color BackGroundColor { get; set; } = Color.FromHex("212121");
            public Color CellColor { get; set; } = Color.FromHex("424242");
            public Color TextColor { get; set; } = Color.FromHex("ffffff");
            public Color ValueColor { get; set; } = Color.Blue;
            public DateTime LastActivity { get; set; } = DateTime.Now;            
            public List<TrackItem> Items { get; set; } = new List<TrackItem>();
            public List<ItemLabel> Itemlabels { get; set; } = new List<ItemLabel>();
            public List<DrawShape.ShapeView> ItemShapeViews { get; set; } = new List<DrawShape.ShapeView>();
        }
        public static Configuration Config { get; set; } = null;
        public static TabbedPage Tp { get; set; } = new TabbedPage();
        public static Models.Sitemaps.Sitemap Sitemaps { get; set; } = null;
        #endregion Variables

        public App()
        {
            if (Config == null)
            {
                Config = new Configuration();
                IPlatformInfo platformInfo = DependencyService.Get<IPlatformInfo>();
                CrossLogger.Current.Info("Kala", "Model: " + platformInfo.GetModel());
                CrossLogger.Current.Info("Kala", "Version: " + platformInfo.GetVersion());
                CrossLogger.Current.Info("Kala", "Wifi MAC address: " + platformInfo.GetWifiMacAddress());

                CrossLogger.Current.Debug("Kala", @"URL Settings: '" + Settings.Protocol + "://" + Settings.Server + ":" + Settings.Port.ToString() + "'");
                CrossLogger.Current.Debug("Kala", @"Auth Settings: '" + Settings.Username + " / " + Settings.Password + "'");
                CrossLogger.Current.Debug("Kala", @"Sitemap Settings: '" + Settings.Sitemap + "'");
            }

            //Initialize FFImageLoading with Authentication
            FFImageLoading.AuthenticatedHttpImageClientHandler.Initialize();

            //TabbedPage setup
            if (Tp.Children.Count == 0)
            {
                Tp.BackgroundColor = Config.BackGroundColor;
                Tp.BarBackgroundColor = Config.BackGroundColor;
                Tp.BarTextColor = Config.TextColor;

                Tp.CurrentPageChanged += (sender, e) =>
                {
                    //Reset screensaver timer
                    Config.LastActivity = DateTime.Now;
                    CrossLogger.Current.Debug("Kala", "Reset Screensaver timer");
                };

                //Auto configured sitemap?
                var WifiMac = DependencyService.Get<IPlatformInfo>().GetWifiMacAddress();
                Sitemaps = Sitemap.GetActiveSitemap(WifiMac);

                if (Sitemaps == null)
                {
                    Sitemaps = Sitemap.GetActiveSitemap(Settings.Sitemap);

                    //Selected sitemap was not found, display settings page to make change
                    if (Sitemaps == null)
                    {
                        //Add settings tab
                        MainPage = new Pages.Settings();
                    }
                }

                if (Sitemaps != null)
                {
                    Sitemap sitemap = new Sitemap();
                    sitemap.GetUpdates();
                    sitemap.CreateSitemap(Sitemaps);
                    CrossLogger.Current.Debug("Kala", "Got Sitemaps");

                    if (Config.Settings)
                    {
                        //Add settings tab last
                        Tp.Children.Add(new Pages.Settings());
                    }
                    MainPage = Tp;
                }
            }
            else
            {
                MainPage = Tp;
            }
            Config.Initialized = true;
        }
        
        protected override void OnStart()
        {
            //Not currently used
        }

        protected override void OnSleep()
        {
            //Not currently used
        }

        protected override void OnResume()
        {
            //Not currently used
        }
    }
}
