using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kala
{
    public class Sitemap
    {
        //Pages
        public static TabbedPage tp = null;

        public void CreateSitemap()
        {
            Models.Sitemap.Sitemap items = new RestService().LoadItemsFromSitemap(App.config.sitemap);

            //Configuration
            Dictionary<string, string> entry = Helpers.SplitCommand(items.label);

            //Settings
            if (entry.ContainsKey("fullscreen"))
            {
                //Setting is stored locally, else its read too late to take effect on device
                try
                {
                    Settings.Fullscreen = Convert.ToBoolean(entry["fullscreen"]);
                }
                catch
                {
                    Debug.WriteLine("Failed to convert Fullscreen value: '", entry["fullscreen"] + "'");
                }
            }

            if (entry.ContainsKey("screensaver"))
            {
                try
                {
                    //Setting is stored locally, else its read too late to take effect on device
                    Settings.Screensaver = Convert.ToInt16(entry["screensaver"]);
                }
                catch
                {
                    Debug.WriteLine("Failed to convert screensaver timeout value: '", entry["screensaver"] + "'");
                }
            }

            if (entry.ContainsKey("kala"))
            {
                try
                {
                    App.config.Valid = Convert.ToBoolean(entry["kala"]);
                }
                catch
                {
                    Debug.WriteLine("Failed to convert 'kala' identifier: '", entry["kala"] + "'");
                }                
            }

            Debug.WriteLine("Fullscreen : " + Settings.Fullscreen.ToString());
            Debug.WriteLine("Screensaver: " + Settings.Screensaver.ToString());
            Debug.WriteLine("Kala Sitemap: " + Settings.Fullscreen.ToString());

            if (App.config.Valid)
            {
                ParseSitemap(items);

                //Enable screensaver?
                if (Settings.Screensaver > 0)
                {
                    Widgets.Screensaver(Settings.Screensaver);
                }
            }
        }

        private void ParseSitemap(Models.Sitemap.Sitemap items)
        {
            tp = new TabbedPage();
            tp.BackgroundColor = Color.Accent;
            tp.BarBackgroundColor = App.config.BackGroundColor;
            tp.BarTextColor = App.config.TextColor;
            tp.WidthRequest = 400;

            foreach (Models.Sitemap.Widget page in items.homepage.widget)
            {
                //Create Page
                Debug.WriteLine("Label: " + page.label);

                Dictionary<string, string> pageKeyValuePairs = Helpers.SplitCommand(page.label);
                Debug.WriteLine("Label: " + pageKeyValuePairs["label"]);

                Grid grid = CreatePage(pageKeyValuePairs["label"], pageKeyValuePairs["sx"], pageKeyValuePairs["sy"], pageKeyValuePairs["icon"]);

                //Populate Page
                foreach (Models.Sitemap.Widget2 item in page.linkedPage.widget)
                {
                    Debug.WriteLine("Widget : " + item.widget + ", ID: " + item.widgetId);
                    Dictionary<string, string> itemKeyValuePairs = Helpers.SplitCommand(item.label);
                    
                    /*Dictionary<string, string> widgetKeyValuePairs = null;
                    if (item.widget != null)
                    {
                        widgetKeyValuePairs = Helpers.SplitCommand(item.widget.label);
                        Debug.WriteLine("Label: " + widgetKeyValuePairs["label"]);
                    }*/

                    switch (itemKeyValuePairs["widget"].ToUpper())
                    {
                        case "GAUGE":
                            Widgets.Gauge(grid, itemKeyValuePairs["label"], (JObject)item.widget);
                            break;
                        case "CLOCK":
                            Widgets.Clock(grid, itemKeyValuePairs["px"], itemKeyValuePairs["py"], itemKeyValuePairs["sx"], itemKeyValuePairs["sy"]);
                            break;
                        case "BLANK":
                            Widgets.Blank(grid, itemKeyValuePairs["px"], itemKeyValuePairs["py"]);
                            break;
                        case "DIMMER":
                            Widgets.Dimmer(grid, itemKeyValuePairs["label"], (JObject)item.widget);
                            break;
                        case "SWITCH":
                            Widgets.Switch(grid, itemKeyValuePairs["label"], (JObject)item.widget);
                            break;
                        case "WEATHER":
                            Widgets.Weather(grid, itemKeyValuePairs["px"], itemKeyValuePairs["py"], itemKeyValuePairs["sx"], itemKeyValuePairs["sy"], itemKeyValuePairs["label"], (JArray)item.widget);
                            break;
                    }
                }

                
            }
        }

        public bool a = true;
        private Grid CreatePage(string title, string sx, string sy, string icon)
        {
            Grid grid = new Grid();

            CreateGrid(grid, Convert.ToInt16(sx), Convert.ToInt16(sy)); //Grid, Columns, Rows

            ContentPage cp = new ContentPage();
            cp.BackgroundColor = App.config.BackGroundColor;
            cp.Title = title;
            /**///cp.Icon = icon;
            cp.Content = grid;
            cp.Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0);
            tp.Children.Add(cp);

            return grid;
        }

        public static void CreateGrid(Grid grid, int columns, int rows)
        {
            grid.RowDefinitions = new RowDefinitionCollection();
            for (int i = 0; i < rows; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }

            grid.ColumnDefinitions = new ColumnDefinitionCollection();
            for (int i = 0; i < columns; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            grid.RowSpacing = 6;
            grid.ColumnSpacing = 6;
            grid.BackgroundColor = App.config.BackGroundColor;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Await.Warning", "CS4014:Await.Warning")]
        public void GetUpdates()
        {
            new RestService().GetUpdate();
        }
    }
}