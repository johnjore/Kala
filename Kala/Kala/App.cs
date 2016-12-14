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
            }

            //Initialize FFImageLoading with Authentication
            FFImageLoading.AuthenticatedHttpImageClientHandler.Initialize();

            /**/ //Show a busy signal here as we can't display anything until we have downloaded the sitemap with its items. No async. Pointless..
            if (GetActiveSitemap() == false)
            {
                MainPage = new Views.Page1();
            }
            else
            {
                Sitemap sitemap = new Sitemap();
                sitemap.CreateSitemap();

                Debug.WriteLine("Config - Sitemap : " + config.sitemap.name);
                Debug.WriteLine("Got ActiveSitemap");

                //Add settings tab last
                Sitemap.tp.Children.Add(new Views.Page1());
                MainPage = Sitemap.tp;

                sitemap.GetUpdates();
            }
        }

        public bool GetActiveSitemap()
        {
            Models.Sitemaps.Sitemaps sitemaps = new RestService().ListSitemaps();

            if (sitemaps.sitemap != null)
            {
                foreach (Models.Sitemaps.Sitemap s in sitemaps.sitemap)
                {
                    Dictionary<string, string> keywords = Helpers.SplitCommand(s.label);
                    if (keywords != null && keywords.ContainsKey("kala") && keywords["kala"].Contains("true"))
                    {
                        Debug.WriteLine("Name: " + s.name);

                        if (s.name.Equals(Settings.Sitemap))
                        {
                            /**/ //Filter-out non-kala=true sitemaps
                            config.sitemap = s;
                            Debug.WriteLine("Label: " + s.label);
                            Debug.WriteLine("Name: " + s.name);
                            Debug.WriteLine("Link: " + s.link);

                            Debug.WriteLine("Link (link): " + config.sitemap.link);

                            return true;
                        }
                    }
                }
            }

            return false;
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
