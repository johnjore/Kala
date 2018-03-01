using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using Plugin.Logger;

namespace Kala
{
    public class Sitemap
    {
        public static Models.Sitemaps.Sitemap GetActiveSitemap(string SitemapName)
        {
            Models.Sitemaps.Sitemaps sitemaps = new RestService().ListSitemaps();

            if (sitemaps != null && sitemaps.sitemap != null)
            {
                //Loop through each sitemap
                foreach (Models.Sitemaps.Sitemap s in sitemaps.sitemap)
                {
                    if (s.label.Contains("kala=true") && (Helpers.SplitCommand(s.label) != null) && (s.name.Equals(SitemapName)))
                    {
                        CrossLogger.Current.Info("Kala", "Label: " + s.label);
                        CrossLogger.Current.Info("Kala", "Name: " + s.name);
                        CrossLogger.Current.Info("Kala", "Link: " + s.link);

                        return s;
                    }
                }
            }

            return null;
        }
        
        /// <summary>
        /// Create GUI
        /// </summary>
        /// <returns>nothing</returns>
        public void CreateSitemap(Models.Sitemaps.Sitemap sitemap)
        {
            Models.Sitemap.Sitemap items = new RestService().LoadItemsFromSitemap(sitemap);

            //Configuration
            Dictionary<string, string> entry = Helpers.SplitCommand(items.label);

            //Settings
            if (entry.ContainsKey("fullscreen"))
            {
                try
                {
                    App.config.FullScreen = true;
                    IScreen fs = DependencyService.Get<IScreen>();
                    fs.SetFullScreen(Convert.ToBoolean(entry["fullscreen"]));
                    fs = null;                    
                }
                catch (Exception ex)
                {
                    CrossLogger.Current.Error("Kala", "Failed to convert 'fullscreen' value: '" + entry["fullscreen"].ToString() + "', " + ex.ToString());
                }
            }

            if (entry.ContainsKey("screensaver"))
            {
                try
                {
                    App.config.ScreenSaver = Convert.ToInt64(entry["screensaver"]);
                    IScreen ss = DependencyService.Get<IScreen>();                    
                    ss.ScreenSaver(App.config.ScreenSaver);
                    ss = null;

                    App.config.ScreenSaverType = Models.ScreenSaverTypes.Clock;
                    if (entry.ContainsKey("screensavertype"))
                    {
                        App.config.ScreenSaverType = (Models.ScreenSaverTypes)Enum.Parse(typeof(Models.ScreenSaverTypes), entry["screensavertype"], true);
                    }

                    switch (App.config.ScreenSaverType)
                    {
                        case Models.ScreenSaverTypes.Images:
                            if (entry.ContainsKey("screensaverurl"))
                            {
                                Widgets.url = entry["screensaverurl"];
                            }
                            else
                            {
                                App.config.ScreenSaverType = Models.ScreenSaverTypes.Clock;
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    CrossLogger.Current.Error("Kala", "Failed to convert 'screensaver' value: '" + entry["screensaver"].ToString() + "', " + ex.ToString());
                }
            }

            if (entry.ContainsKey("kala"))
            {
                try
                {
                    App.config.Valid = Convert.ToBoolean(entry["kala"]);
                }
                catch (Exception ex)
                {
                    CrossLogger.Current.Error("Kala", "Failed to convert 'kala' identifier: '" + entry["kala"].ToString() + "', " + ex.ToString());
                }                
            }

            if (entry.ContainsKey("background"))
            {
                try
                {
                    App.config.BackGroundColor = Color.FromHex(entry["background"]);
                }
                catch (Exception ex)
                {
                    CrossLogger.Current.Error("Kala", "Failed to convert 'background' value: '" + entry["background"].ToString() + "', " + ex.ToString());
                }
            }

            if (entry.ContainsKey("cell"))
            {
                try
                {
                    App.config.CellColor = Color.FromHex(entry["cell"]);
                }
                catch (Exception ex)
                {
                    CrossLogger.Current.Error("Kala", "Failed to convert 'cell' value: '" + entry["cell"].ToString() + "', " + ex.ToString());
                }
            }

            if (entry.ContainsKey("text"))
            {
                try
                {
                    App.config.TextColor = Color.FromHex(entry["text"]);
                }
                catch (Exception ex)
                {
                    CrossLogger.Current.Error("Kala", "Failed to convert 'text' value: '" + entry["text"].ToString() + "', " + ex.ToString());
                }
            }

            if (entry.ContainsKey("value"))
            {
                try
                {
                    App.config.ValueColor = Color.FromHex(entry["value"]);
                }
                catch (Exception ex)
                {
                    CrossLogger.Current.Error("Kala", "Failed to convert 'value' value: '" + entry["value"].ToString() + "', " + ex.ToString());
                }
            }

            if (entry.ContainsKey("screenorientation"))
            {
                try
                {
                    IScreen so = DependencyService.Get<IScreen>();
                    so.SetScreenOrientation(entry["screenorientation"]);
                    so = null;
                }
                catch (Exception ex)
                {                    
                    CrossLogger.Current.Error("Kala", "Failed to action 'screenorientation' value: '" + entry["screenorientation"].ToString() + "', " + ex.ToString());
                }
            }
            
            if (entry.ContainsKey("settings"))
            {
                try
                {
                    App.config.Settings = Convert.ToBoolean(entry["settings"]);
                }
                catch (Exception ex)
                {
                    CrossLogger.Current.Error("Kala", "Failed to action 'settings' value: '" + entry["settings"].ToString() + "', " + ex.ToString());
                }
            }

            if (App.config.Valid)
            {
                ParseSitemap(items);

                //Enable screensaver?
                if (App.config.ScreenSaver > 0)
                {
                    Widgets.Screensaver(App.config.ScreenSaver);
                    App.config.ScreenSaver = 0;
                }
            }
        }

        /// <summary>
        /// Parse Sitemap file
        /// </summary>
        /// <returns>nothing</returns>
        private void ParseSitemap(Models.Sitemap.Sitemap items)
        {
            try
            {
                foreach (Models.Sitemap.Widget page in items.homepage.widgets)
                {
                    CrossLogger.Current.Debug("Kala", "Label: " + page.label);

                    //Populate Page, if it contains elements to parse
                    if (page.label != string.Empty)
                    {
                        Dictionary<string, string> pageKeyValuePairs = Helpers.SplitCommand(page.label);
                        CrossLogger.Current.Debug("Kala", "Label: " + pageKeyValuePairs["label"]);

                        #region page
                        if (page.linkedPage != null)
                        {
                            if (pageKeyValuePairs.ContainsKey("sx") && pageKeyValuePairs.ContainsKey("sy") && pageKeyValuePairs.ContainsKey("label"))
                            {
                                if (!pageKeyValuePairs.ContainsKey("icon"))
                                {
                                    pageKeyValuePairs.Add("icon", null);
                                }

                                CrossLogger.Current.Debug("Kala", "Sitemap - Create Grid using: " + pageKeyValuePairs["label"] + ", " + pageKeyValuePairs["sx"] + ", " + pageKeyValuePairs["sy"] + ", " + pageKeyValuePairs["icon"]);
                                Grid grid = CreatePage(pageKeyValuePairs["label"], pageKeyValuePairs["sx"], pageKeyValuePairs["sy"], pageKeyValuePairs["icon"]);

                                foreach (Models.Sitemap.Widget3 item in page.linkedPage.widgets)
                                {
                                    ParseWidgets(grid, item);
                                }
                            }
                        }
                        #endregion page
                        else
                        {
                            CrossLogger.Current.Warn("Kala", "Unknown: " + ToString());

                            switch (pageKeyValuePairs["widget"].ToUpper())
                            {
                                case "SITEMAP":
                                    CrossLogger.Current.Debug("Kala", "Sitemap:" + pageKeyValuePairs["name"]);

                                    Models.Sitemaps.Sitemap sitemaps = GetActiveSitemap(pageKeyValuePairs["name"]);
                                    if (sitemaps != null)
                                    {
                                        Sitemap sitemap = new Sitemap();
                                        sitemap.CreateSitemap(sitemaps);

                                        CrossLogger.Current.Debug("Kala", "Got ActiveSitemap");
                                    }
                                    break;
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Sitemap", "ParseSitemap() crashed: " + ex.ToString());
            }
        }

        /// <summary>
        /// Parses / creates Widgets
        /// </summary>
        /// <returns>nothing</returns>
        private void ParseWidgets(Grid grid, Models.Sitemap.Widget3 item)
        {
            try
            {
                CrossLogger.Current.Debug("Kala", "ID: " + item.widgetId);
                Dictionary<string, string> itemKeyValuePairs = Helpers.SplitCommand(item.label);

                if (itemKeyValuePairs != null && itemKeyValuePairs.ContainsKey("widget") && itemKeyValuePairs.ContainsKey("px") && itemKeyValuePairs.ContainsKey("py"))
                {
                    switch (itemKeyValuePairs["widget"].ToUpper())
                    {
                        case "AVATAR":
                            if (itemKeyValuePairs.ContainsKey("sx") && itemKeyValuePairs.ContainsKey("sy"))
                            {
                                Widgets.Avatar(grid, itemKeyValuePairs["px"], itemKeyValuePairs["py"], itemKeyValuePairs["sx"], itemKeyValuePairs["sy"], JArray.FromObject(item.widgets));
                            }
                            break;
                        case "BLIND":
                            if (itemKeyValuePairs.ContainsKey("label") && itemKeyValuePairs.ContainsKey("sx") && itemKeyValuePairs.ContainsKey("sy"))
                            {
                                Widgets.Blind(grid, itemKeyValuePairs["px"], itemKeyValuePairs["py"], itemKeyValuePairs["sx"], itemKeyValuePairs["sy"], itemKeyValuePairs["label"], (JObject)item.widgets[0]);
                            }
                            break;
                        case "BLANK":
                            Widgets.Blank(grid, itemKeyValuePairs["px"], itemKeyValuePairs["py"]);
                            break;
                        case "CALENDAR":
                            if (itemKeyValuePairs.ContainsKey("sx") && itemKeyValuePairs.ContainsKey("sy"))
                            {
                                Widgets.Calendar(grid, itemKeyValuePairs["px"], itemKeyValuePairs["py"], itemKeyValuePairs["sx"], itemKeyValuePairs["sy"], JArray.FromObject(item.widgets));
                            }
                            break;
                        case "CLOCK":
                            if (itemKeyValuePairs.ContainsKey("sx") && itemKeyValuePairs.ContainsKey("sy"))
                            {
                                Widgets.Clock(grid, itemKeyValuePairs["px"], itemKeyValuePairs["py"], itemKeyValuePairs["sx"], itemKeyValuePairs["sy"]);
                            }
                            break;
                        case "DIMMER":
                            if (itemKeyValuePairs.ContainsKey("label"))
                            {
                                Widgets.Dimmer(grid, itemKeyValuePairs["px"], itemKeyValuePairs["py"], itemKeyValuePairs["label"], (JObject)item.widgets[0]);
                            }
                            break;
                        case "FLOORMAP":
                            if (itemKeyValuePairs.ContainsKey("label"))
                            {
                                Widgets.Floormap(grid, itemKeyValuePairs["px"], itemKeyValuePairs["py"], itemKeyValuePairs["sx"], itemKeyValuePairs["sy"], 
                                    itemKeyValuePairs["label"], JArray.FromObject(item.widgets));
                            }
                            break;
                        case "SENSOR":
                            string sx = "1";
                            string sy = "1";
                            if (itemKeyValuePairs.ContainsKey("sx") && itemKeyValuePairs.ContainsKey("sy"))
                            {
                                sx = itemKeyValuePairs["sx"];
                                sy = itemKeyValuePairs["sy"];
                            }

                            if (itemKeyValuePairs.ContainsKey("label"))
                            {
                                Widgets.Sensor(grid, itemKeyValuePairs["px"], itemKeyValuePairs["py"], itemKeyValuePairs["sx"], itemKeyValuePairs["sy"], itemKeyValuePairs["label"], (JObject)item.widgets[0]);
                            }
                            break;
                        case "GAUGE":
                            if (itemKeyValuePairs.ContainsKey("label"))
                            {
                                Widgets.Gauge(grid, itemKeyValuePairs["px"], itemKeyValuePairs["py"], itemKeyValuePairs["label"], (JObject)item.widgets[0]);
                            }
                            break;
                        case "GAUGE-GROUP":
                            string rx = String.Empty;
                            string ry = String.Empty;
                            if (itemKeyValuePairs.ContainsKey("rx") && itemKeyValuePairs.ContainsKey("ry"))
                            {
                                rx = itemKeyValuePairs["rx"];
                                ry = itemKeyValuePairs["ry"];
                            }

                            if (itemKeyValuePairs.ContainsKey("label") && itemKeyValuePairs.ContainsKey("sx") && itemKeyValuePairs.ContainsKey("sy"))
                            {
                                Widgets.Gauge_Group(grid, itemKeyValuePairs["px"], itemKeyValuePairs["py"], itemKeyValuePairs["sx"], itemKeyValuePairs["sy"], rx, ry, itemKeyValuePairs["label"], JArray.FromObject(item.widgets));
                            }
                            break;
                        case "IMAGE":
                            if (!itemKeyValuePairs.ContainsKey("aspect")) { itemKeyValuePairs.Add("aspect", "aspectfill"); }

                            if (itemKeyValuePairs.ContainsKey("px") && itemKeyValuePairs.ContainsKey("py") && itemKeyValuePairs.ContainsKey("sx") && itemKeyValuePairs.ContainsKey("sy") && itemKeyValuePairs.ContainsKey("label"))
                            {
                                Widgets.Image(grid, itemKeyValuePairs["px"], itemKeyValuePairs["py"], itemKeyValuePairs["sx"], itemKeyValuePairs["sy"], itemKeyValuePairs["label"], itemKeyValuePairs["aspect"], (JObject)item.widgets[0]);
                            }
                            break;
                        case "MAP":
                            if (!itemKeyValuePairs.ContainsKey("type")) { itemKeyValuePairs.Add("type", "street"); }

                            if (itemKeyValuePairs.ContainsKey("sx") && itemKeyValuePairs.ContainsKey("sy"))
                            {
                                Widgets.Map(grid, itemKeyValuePairs["px"], itemKeyValuePairs["py"], itemKeyValuePairs["sx"], itemKeyValuePairs["sy"], itemKeyValuePairs["type"], JArray.FromObject(item.widgets));
                            }
                            break;
                        case "SWITCH":
                            if (itemKeyValuePairs.ContainsKey("label"))
                            {
                                Widgets.Switch(grid, itemKeyValuePairs["px"], itemKeyValuePairs["py"], itemKeyValuePairs["label"], (JObject)item.widgets[0]);
                            }
                            break;
                        case "WEATHER":
                            if (itemKeyValuePairs.ContainsKey("sx") && itemKeyValuePairs.ContainsKey("sy") && itemKeyValuePairs.ContainsKey("label"))                            
                            {
                                Widgets.Weather(grid, itemKeyValuePairs["px"], itemKeyValuePairs["py"], itemKeyValuePairs["sx"], itemKeyValuePairs["sy"], itemKeyValuePairs["label"], JArray.FromObject(item.widgets));
                            }
                            break;
                        case "WEATHERFORECAST":
                            if (itemKeyValuePairs.ContainsKey("sx") && itemKeyValuePairs.ContainsKey("sy") && itemKeyValuePairs.ContainsKey("label"))
                            {
                                Widgets.WeatherForecast(grid, itemKeyValuePairs["px"], itemKeyValuePairs["py"], itemKeyValuePairs["sx"], itemKeyValuePairs["sy"], itemKeyValuePairs["label"], JArray.FromObject(item.widgets));
                            }
                            break;
                        default:
                            CrossLogger.Current.Warn("Kala", "Failed to parse widget. Unknown type: " + item.ToString());
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Kala", "Failed to parse widget: " + ex.ToString());
            }
        }

        /// <summary>
        /// Creates content page with full screen grid
        /// </summary>
        /// <returns>nothing</returns>
        private Grid CreatePage(string title, string sx, string sy, string icon)
        {
            Grid grid = new Grid();

            CreateGrid(grid, Convert.ToInt16(sx), Convert.ToInt16(sy)); //Grid, Columns, Rows

            ContentPage cp = new ContentPage();
            if (icon != null)
            {
                cp.Icon = icon;
            }
            cp.BackgroundColor = App.config.BackGroundColor;
            cp.Title = title;
            cp.Content = grid;

            cp.Padding = new Thickness(0, 0, 0, 0);
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    new Thickness(0, 20, 0, 0);
                    break;
            }
            App.tp.Children.Add(cp);

            return grid;
        }

        /// <summary>
        /// Configures grid with columns and rows
        /// </summary>
        /// <returns>nothing</returns>
        private static void CreateGrid(Grid grid, int columns, int rows)
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

        #pragma warning disable CS4014
        public void GetUpdates()
        {
            new RestService().GetUpdateAsync();
        }
        #pragma warning restore CS4014
    }
}