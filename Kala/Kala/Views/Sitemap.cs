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
                    Dictionary<string, string> keywords = Helpers.SplitCommand(s.label);
                    if (keywords != null && keywords.ContainsKey("kala") && keywords["kala"].Contains("true"))
                    {
                        if (s.name.Equals(SitemapName))
                        {
                            CrossLogger.Current.Info("Kala", "Label: " + s.label);
                            CrossLogger.Current.Info("Kala", "Name: " + s.name);
                            CrossLogger.Current.Info("Kala", "Link: " + s.link);

                            return s;
                        }
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
                //Setting is stored locally, else its read too late to take effect on device
                try
                {
                    Settings.Fullscreen = Convert.ToBoolean(entry["fullscreen"]);
                }
                catch
                {
                    CrossLogger.Current.Error("Kala", "Failed to convert 'fullscreen' value: '" + entry["fullscreen"] + "'");
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
                    CrossLogger.Current.Error("Kala", "Failed to convert 'screensaver' value: '" + entry["screensaver"] + "'");
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
                    CrossLogger.Current.Error("Kala", "Failed to convert 'kala' identifier: '" + entry["kala"] + "'");
                }                
            }

            if (entry.ContainsKey("background"))
            {
                try
                {
                    App.config.BackGroundColor = Color.FromHex(entry["background"]);
                }
                catch
                {
                    CrossLogger.Current.Error("Kala", "Failed to convert 'background' value: '" + entry["background"] + "'");
                }
            }

            if (entry.ContainsKey("cell"))
            {
                try
                {
                    App.config.CellColor = Color.FromHex(entry["cell"]);
                }
                catch
                {
                    CrossLogger.Current.Error("Kala", "Failed to convert 'cell' value: '" + entry["cell"] + "'");
                }
            }
            if (entry.ContainsKey("text"))
            {
                try
                {
                    App.config.TextColor = Color.FromHex(entry["text"]);
                }
                catch
                {
                    CrossLogger.Current.Error("Kala", "Failed to convert 'text' value: '" + entry["text"] + "'");
                }
            }
            if (entry.ContainsKey("value"))
            {
                try
                {
                    App.config.ValueColor = Color.FromHex(entry["value"]);
                }
                catch
                {
                    CrossLogger.Current.Error("Kala", "Failed to convert 'value' value: '" + entry["value"] + "'");
                }
            }


            CrossLogger.Current.Info("Kala", "Fullscreen : " + Settings.Fullscreen.ToString());
            CrossLogger.Current.Info("Kala", "Screensaver: " + Settings.Screensaver.ToString());
            CrossLogger.Current.Info("Kala", "Sitemap: " + Settings.Fullscreen.ToString());

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

        /// <summary>
        /// Parse Sitemap file
        /// </summary>
        /// <returns>nothing</returns>
        private void ParseSitemap(Models.Sitemap.Sitemap items)
        {
            foreach (Models.Sitemap.Widget page in items.homepage.widget)
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
                        Grid grid = null;

                        if (pageKeyValuePairs.ContainsKey("sx") && pageKeyValuePairs.ContainsKey("sy"))
                        {
                            if (pageKeyValuePairs.ContainsKey("icon"))
                            {
                                grid = CreatePage(pageKeyValuePairs["label"], pageKeyValuePairs["sx"], pageKeyValuePairs["sy"], pageKeyValuePairs["icon"]);
                            }
                            else
                            {
                                grid = CreatePage(pageKeyValuePairs["label"], pageKeyValuePairs["sx"], pageKeyValuePairs["sy"], null);
                            }
                        }

                        //Shortcut
                        var w = page.linkedPage.widget;

                        //If more than one item page frame
                        if (w.GetType() == typeof(JArray))
                        {
                            List<Models.Sitemap.Widget3> w_items = ((JArray)w).ToObject<List<Models.Sitemap.Widget3>>();
                            foreach (Models.Sitemap.Widget3 item in w_items)
                            {
                                ParseWidgets(grid, item);
                            }
                        }
                        //If one item in page frame
                        else if (w.GetType() == typeof(JObject))
                        {
                            Models.Sitemap.Widget3 item = ((JObject)w).ToObject<Models.Sitemap.Widget3>();
                            ParseWidgets(grid, item);
                        }
                        else
                        {
                            CrossLogger.Current.Warn("Kala", "Unknown: " + w.ToString());
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

        /// <summary>
        /// Parses / creates Widgets
        /// </summary>
        /// <returns>nothing</returns>
        private void ParseWidgets(Grid grid, Models.Sitemap.Widget3 item)
        {
            try
            {
                CrossLogger.Current.Debug("Kala", "Widget : " + item.widget + ", ID: " + item.widgetId);
                Dictionary<string, string> itemKeyValuePairs = Helpers.SplitCommand(item.label);

                if (itemKeyValuePairs != null && itemKeyValuePairs.ContainsKey("widget") && itemKeyValuePairs.ContainsKey("px") && itemKeyValuePairs.ContainsKey("py"))
                {
                    switch (itemKeyValuePairs["widget"].ToUpper())
                    {
                        case "AVATAR":
                            if (itemKeyValuePairs.ContainsKey("sx") && itemKeyValuePairs.ContainsKey("sy"))
                            {
                                Widgets.Avatar(grid, itemKeyValuePairs["px"], itemKeyValuePairs["py"], itemKeyValuePairs["sx"], itemKeyValuePairs["sy"], (JObject)item.widget);
                            }
                            break;
                        case "BLANK":
                            Widgets.Blank(grid, itemKeyValuePairs["px"], itemKeyValuePairs["py"]);
                            break;
                        case "CALENDAR":
                            if (itemKeyValuePairs.ContainsKey("sx") && itemKeyValuePairs.ContainsKey("sy"))
                            {
                                Widgets.Calendar(grid, itemKeyValuePairs["px"], itemKeyValuePairs["py"], itemKeyValuePairs["sx"], itemKeyValuePairs["sy"], (JArray)item.widget);
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
                                Widgets.Dimmer(grid, itemKeyValuePairs["px"], itemKeyValuePairs["py"], itemKeyValuePairs["label"], (JObject)item.widget);
                            }
                            break;
                        case "GAUGE":
                            if (itemKeyValuePairs.ContainsKey("label"))
                            {
                                Widgets.Gauge(grid, itemKeyValuePairs["px"], itemKeyValuePairs["py"], itemKeyValuePairs["label"], (JObject)item.widget);
                            }
                            break;
                        case "GAUGE-GROUP":
                            if (itemKeyValuePairs.ContainsKey("label") && itemKeyValuePairs.ContainsKey("sx") && itemKeyValuePairs.ContainsKey("sy"))
                            {
                                var token = JToken.Parse(item.widget.ToString());
                                if (token is JArray)
                                {
                                    Widgets.Gauge_Group(grid, itemKeyValuePairs["px"], itemKeyValuePairs["py"], itemKeyValuePairs["sx"], itemKeyValuePairs["sy"], itemKeyValuePairs["label"], (JArray)item.widget);
                                }
                                else if (token is JObject)
                                {
                                    Widgets.Gauge_Group(grid, itemKeyValuePairs["px"], itemKeyValuePairs["py"], itemKeyValuePairs["sx"], itemKeyValuePairs["sy"], itemKeyValuePairs["label"], (JObject)item.widget);
                                }
                            }
                            break;
                        case "IMAGE":
                            //Parameters
                            if (!itemKeyValuePairs.ContainsKey("aspect")) { itemKeyValuePairs.Add("aspect", "aspectfill"); }

                            if (itemKeyValuePairs.ContainsKey("px") && itemKeyValuePairs.ContainsKey("py") && itemKeyValuePairs.ContainsKey("sx") && itemKeyValuePairs.ContainsKey("sy") && itemKeyValuePairs.ContainsKey("label"))
                            {
                                Widgets.Image(grid, itemKeyValuePairs["px"], itemKeyValuePairs["py"], itemKeyValuePairs["sx"], itemKeyValuePairs["sy"], itemKeyValuePairs["label"], itemKeyValuePairs["aspect"], (JObject)item.widget);
                            }
                            break;
                        case "MAP":
                            //Parameters
                            if (!itemKeyValuePairs.ContainsKey("type")) { itemKeyValuePairs.Add("type", "street"); }

                            if (itemKeyValuePairs.ContainsKey("sx") && itemKeyValuePairs.ContainsKey("sy"))
                            {
                                Widgets.Map(grid, itemKeyValuePairs["px"], itemKeyValuePairs["py"], itemKeyValuePairs["sx"], itemKeyValuePairs["sy"], itemKeyValuePairs["type"], (JArray)item.widget);
                            }
                            break;
                        case "SWITCH":
                            if (itemKeyValuePairs.ContainsKey("label"))
                            {
                                Widgets.Switch(grid, itemKeyValuePairs["px"], itemKeyValuePairs["py"], itemKeyValuePairs["label"], (JObject)item.widget);
                            }
                            break;
                        case "WEATHER":
                            if (itemKeyValuePairs.ContainsKey("sx") && itemKeyValuePairs.ContainsKey("sy") && itemKeyValuePairs.ContainsKey("label"))
                            {
                                Widgets.Weather(grid, itemKeyValuePairs["px"], itemKeyValuePairs["py"], itemKeyValuePairs["sx"], itemKeyValuePairs["sy"], itemKeyValuePairs["label"], (JArray)item.widget);
                            }
                            break;
                        case "WEATHERFORECAST":
                            if (itemKeyValuePairs.ContainsKey("sx") && itemKeyValuePairs.ContainsKey("sy") && itemKeyValuePairs.ContainsKey("label"))
                            {
                                Widgets.WeatherForecast(grid, itemKeyValuePairs["px"], itemKeyValuePairs["py"], itemKeyValuePairs["sx"], itemKeyValuePairs["sy"], itemKeyValuePairs["label"], (JArray)item.widget);
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
            cp.Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0);
            App.tp.Children.Add(cp);

            return grid;
        }

        /// <summary>
        /// Configures grid with columns and rows
        /// </summary>
        /// <returns>nothing</returns>
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