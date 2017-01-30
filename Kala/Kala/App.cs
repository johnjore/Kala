using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Plugin.Logger;

namespace Kala
{
    public class App : Application
    {
        #region Variables
        public class trackItem
        {
            public Grid grid { get; set; }
            public int px { get; set; }
            public int py { get; set; }
            public int sx { get; set; }
            public int sy { get; set; }
            public string min { get; set; }
            public string max { get; set; }
            public string state { get; set; }
            public string icon { get; set; }
            public string header { get; set; }
            public string unit { get; set; }
            public Models.Itemtypes type { get; set; }
            public string link { get; set; }
        }

        public class Configuration
        {
            public Models.Sitemaps.Sitemap sitemap;
            public bool Valid = false;
            public bool Initialized = false;
            public Color BackGroundColor = Color.FromHex("212121");
            public Color CellColor = Color.FromHex("424242");
            public Color TextColor = Color.FromHex("ffffff"); // Color.White;
            public Color ValueColor = Color.Blue;
            public DateTime LastActivity = DateTime.Now;
            public List<trackItem> items = new List<trackItem>();
            public List<ItemLabel> itemlabels = new List<ItemLabel>();
            public List<DrawShape.ShapeView> itemShapeViews = new List<DrawShape.ShapeView>();
        }
        public static Configuration config = null;
        public static TabbedPage tp = new TabbedPage();
        public static Models.Sitemaps.Sitemap sitemaps = null;

        #endregion Variables

        public App()
        {
            if (config == null)
            {
                config = new Configuration();
                IPlatformInfo platformInfo = DependencyService.Get<IPlatformInfo>();
                CrossLogger.Current.Info("Kala", "Model: " + platformInfo.GetModel());
                CrossLogger.Current.Info("Kala", "Version: " + platformInfo.GetVersion());

                CrossLogger.Current.Debug("Kala", @"URL Settings: '" + Settings.Protocol + "://" + Settings.Server + ":" + Settings.Port.ToString() + "'");
                CrossLogger.Current.Debug("Kala", @"Auth Settings: '" + Settings.Username + " / " + Settings.Password + "'");
                CrossLogger.Current.Debug("Kala", @"Sitemap Settings: '" + Settings.Sitemap + "'");
            };

            //Initialize FFImageLoading with Authentication
            FFImageLoading.AuthenticatedHttpImageClientHandler.Initialize();

            //TabbedPage setup
            if (tp.Children.Count == 0)
            {
                tp.BackgroundColor = App.config.BackGroundColor;
                tp.BarBackgroundColor = App.config.BackGroundColor;
                tp.BarTextColor = App.config.TextColor;

                tp.CurrentPageChanged += (sender, e) =>
                {
                    //Reset screensaver timer
                    App.config.LastActivity = DateTime.Now;
                    CrossLogger.Current.Debug("Kala", "Reset Screensaver timer");
                };

                /**/ //Show a busy signal here as we can't display anything until we have downloaded the sitemap with its items. No async. Pointless..
                sitemaps = Sitemap.GetActiveSitemap(Settings.Sitemap);

                //Selected sitemap was not found, display settings page to make change
                if (sitemaps == null)
                {
                    //Add settings tab
                    MainPage = new Views.Page1();
                }
                else
                {
                    Sitemap sitemap = new Sitemap();
                    sitemap.GetUpdates();
                    sitemap.CreateSitemap(sitemaps);
                    CrossLogger.Current.Debug("Kala", "Got Sitemaps");

                    //Add settings tab last
                    App.tp.Children.Add(new Views.Page1());
                    MainPage = App.tp;
                }
            }
            else
            {
                MainPage = App.tp;
            }
            App.config.Initialized = true;
        }
        
        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

    }
}
