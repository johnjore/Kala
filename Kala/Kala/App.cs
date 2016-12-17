using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

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
            public string min { get; set; }
            public string max { get; set; }
            public string state { get; set; }
            public string icon { get; set; }
            public string header { get; set; }
            public string unit { get; set; }
            public string type { get; set; }
            public string link { get; set; }
        }

        public class Configuration
        {
            public Models.Sitemaps.Sitemap sitemap;
            public bool Valid = false;
            public Color BackGroundColor = Color.FromHex("212121");
            public Color CellColor = Color.FromHex("424242");
            public Color TextColor = Color.FromHex("ffffff"); // Color.White;
            public DateTime LastActivity = DateTime.Now;
            public List<trackItem> items = new List<trackItem>();
            public List<ItemLabel> itemlabels = new List<ItemLabel>();
        }
        public static Configuration config = null;
        public static TabbedPage tp = new TabbedPage();

        /**/ //Future
        //public static ITextToSpeech Speech { get; set; }
        #endregion Variables

        public App()
        {
            if (config == null)
            {
                config = new Configuration();
                IPlatformInfo platformInfo = DependencyService.Get<IPlatformInfo>();
                Debug.WriteLine("Model: " + platformInfo.GetModel());
                Debug.WriteLine("Version: " + platformInfo.GetVersion());

                Debug.WriteLine(@"URL Settings: '" + Settings.Protocol + "://" + Settings.Server + ":" + Settings.Port.ToString() + "'");
                Debug.WriteLine(@"Auth Settings: '" + Settings.Username + " / " + Settings.Password + "'");
                Debug.WriteLine(@"Sitemap Settings: '" + Settings.Sitemap + "'");
            };

            //TabbedPage setup
            tp.BackgroundColor = Color.Accent;
            tp.BarBackgroundColor = App.config.BackGroundColor;
            tp.BarTextColor = App.config.TextColor;
            
            //Initialize FFImageLoading with Authentication
            FFImageLoading.AuthenticatedHttpImageClientHandler.Initialize();

            /**/ //Show a busy signal here as we can't display anything until we have downloaded the sitemap with its items. No async. Pointless..
            Models.Sitemaps.Sitemap sitemaps = Sitemap.GetActiveSitemap(Settings.Sitemap);

            //Selected sitemap was not found, display settings page to make change
            if (sitemaps == null)
            {
                //Add settings tab
                MainPage = new Views.Page1();
            }
            else
            {
                Sitemap sitemap = new Sitemap();
                sitemap.CreateSitemap(sitemaps);
                Debug.WriteLine("Got Sitemaps");

                //Add settings tab last
                App.tp.Children.Add(new Views.Page1());
                MainPage = App.tp;

                sitemap.GetUpdates();
            }
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
