using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using Plugin.Logger;
using System.Threading.Tasks;

namespace Kala
{
    public class Sitemap
    {
        public static Models.Sitemaps.Sitemap GetActiveSitemap(string SitemapName)
        {
            Models.Sitemaps.Sitemaps sitemaps = new RestService().ListSitemaps();

            if (sitemaps != null && sitemaps.Sitemap != null)
            {
                //Loop through each sitemap
                foreach (Models.Sitemaps.Sitemap s in sitemaps.Sitemap)
                {
                    if (s.Label.Contains("kala=true") && (Helpers.SplitCommand(s.Label) != null) && (s.Name.Equals(SitemapName)))
                    {
                        CrossLogger.Current.Info("Kala", "Label: " + s.Label);
                        CrossLogger.Current.Info("Kala", "Name: " + s.Name);
                        CrossLogger.Current.Info("Kala", "Link: " + s.Link);

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
            Dictionary<string, string> entry = Helpers.SplitCommand(items.Label);

            //Settings
            if (entry.ContainsKey("fullscreen"))
            {
                try
                {
                    App.Config.FullScreen = Convert.ToBoolean(entry["fullscreen"]);
                    DependencyService.Get<IScreen>().SetFullScreen(App.Config.FullScreen);
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
                    App.Config.ScreenSaver = Convert.ToInt64(entry["screensaver"]);
                    IScreen ss = DependencyService.Get<IScreen>();                    
                    ss.ScreenSaver(App.Config.ScreenSaver);

                    App.Config.ScreenSaverType = Models.ScreenSaverTypes.Clock;
                    if (entry.ContainsKey("screensavertype"))
                    {
                        App.Config.ScreenSaverType = (Models.ScreenSaverTypes)Enum.Parse(typeof(Models.ScreenSaverTypes), entry["screensavertype"], true);
                    }

                    switch (App.Config.ScreenSaverType)
                    {
                        case Models.ScreenSaverTypes.Images:
                            if (entry.ContainsKey("screensaverurl"))
                            {
                                Widgets.SetUrl(entry["screensaverurl"]);
                            }
                            else
                            {
                                App.Config.ScreenSaverType = Models.ScreenSaverTypes.Clock;
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
                    App.Config.Valid = Convert.ToBoolean(entry["kala"]);
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
                    App.Config.BackGroundColor = Color.FromHex(entry["background"]);
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
                    App.Config.CellColor = Color.FromHex(entry["cell"]);
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
                    App.Config.TextColor = Color.FromHex(entry["text"]);
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
                    App.Config.ValueColor = Color.FromHex(entry["value"]);
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
                    App.Config.Settings = Convert.ToBoolean(entry["settings"]);
                }
                catch (Exception ex)
                {
                    CrossLogger.Current.Error("Kala", "Failed to action 'settings' value: '" + entry["settings"].ToString() + "', " + ex.ToString());
                }
            }

            if (App.Config.Valid)
            {
                ParseSitemap(items);

                //Enable screensaver?
                if (App.Config.ScreenSaver > 0)
                {
                    Widgets.Screensaver(App.Config.ScreenSaver);
                    App.Config.ScreenSaver = 0;
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
                foreach (Models.Sitemap.Widget page in items.Homepage.Widgets)
                {
                    CrossLogger.Current.Debug("Kala", "Label: " + page.Label);

                    //Populate Page, if it contains elements to parse
                    if (page.Label != string.Empty)
                    {
                        Dictionary<string, string> pageKeyValuePairs = Helpers.SplitCommand(page.Label);
                        CrossLogger.Current.Debug("Kala", "Label: " + pageKeyValuePairs["label"]);

                        #region page
                        if (page.LinkedPage != null)
                        {
                            if (pageKeyValuePairs.ContainsKey("sx") && pageKeyValuePairs.ContainsKey("sy") && pageKeyValuePairs.ContainsKey("label"))
                            {
                                if (!pageKeyValuePairs.ContainsKey("icon"))
                                {
                                    pageKeyValuePairs.Add("icon", null);
                                }

                                CrossLogger.Current.Debug("Kala", "Sitemap - Create Grid using: " + pageKeyValuePairs["label"] + ", " + pageKeyValuePairs["sx"] + ", " + pageKeyValuePairs["sy"] + ", " + pageKeyValuePairs["icon"]);
                                Grid grid = CreatePage(pageKeyValuePairs["label"], pageKeyValuePairs["sx"], pageKeyValuePairs["sy"], pageKeyValuePairs["icon"]);

                                foreach (Models.Sitemap.Widget3 item in page.LinkedPage.Widgets)
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
                CrossLogger.Current.Debug("Kala", "ID: " + item.WidgetId);
                Dictionary<string, string> itemKeyValuePairs = Helpers.SplitCommand(item.Label);

                if (itemKeyValuePairs != null && itemKeyValuePairs.ContainsKey("widget") && itemKeyValuePairs.ContainsKey("px") && itemKeyValuePairs.ContainsKey("py"))
                {
                    //Position
                    int.TryParse(itemKeyValuePairs["px"], out int px);
                    int.TryParse(itemKeyValuePairs["py"], out int py);

                    //Size
                    int sx = 1;
                    if (itemKeyValuePairs.ContainsKey("sx"))
                    {
                        int.TryParse(itemKeyValuePairs["sx"], out sx);
                    }
                    int sy = 1;
                    if (itemKeyValuePairs.ContainsKey("sy"))
                    {
                        int.TryParse(itemKeyValuePairs["sy"], out sy);
                    }

                    //Gauge-Group
                    int rx = 1;
                    if (itemKeyValuePairs.ContainsKey("rx"))
                    {
                        int.TryParse(itemKeyValuePairs["rx"], out rx);
                    }
                    int ry = 1;
                    if (itemKeyValuePairs.ContainsKey("ry"))
                    {
                        int.TryParse(itemKeyValuePairs["ry"], out ry);
                    }

                    switch (itemKeyValuePairs["widget"].ToUpper())
                    {
                        case "AVATAR":
                            Widgets.Avatar(grid, px, py, sx, sy, JArray.FromObject(item.Widgets));
                            break;
                        case "BARCODE":
                            if (itemKeyValuePairs.ContainsKey("label"))
                            {
                                Widgets.Barcode(grid, px, py, sx, sy, itemKeyValuePairs["label"], (JObject)item.Widgets[0]);
                            }
                            break;
                        case "BLIND":
                            if (itemKeyValuePairs.ContainsKey("label"))
                            {
                                Widgets.Blind(grid, px, py, sx, sy, itemKeyValuePairs["label"], (JObject)item.Widgets[0]);
                            }
                            break;
                        case "BLANK":
                            Widgets.Blank(grid, px, py, sx, sy);
                            break;
                        case "CALENDAR":
                            Widgets.Calendar(grid, px, py, sx, sy, JArray.FromObject(item.Widgets));
                            break;
                        case "CHART":
                            if (itemKeyValuePairs.ContainsKey("label"))
                            {
                                Widgets.Chart(grid, px, py, sx, sy, itemKeyValuePairs["label"], (JObject)item.Widgets[0]);
                            }
                            break;
                        case "CLOCK":
                            Widgets.Clock(grid, px, py, sx, sy);
                            break;
                        case "DIMMER":
                            if (itemKeyValuePairs.ContainsKey("label"))
                            {
                                Widgets.Dimmer(grid, px, py, itemKeyValuePairs["label"], (JObject)item.Widgets[0]);
                            }
                            break;
                        case "FLOORMAP":
                            if (itemKeyValuePairs.ContainsKey("label"))
                            {
                                Widgets.FloormapAsync(grid, px, py, sx, sy, itemKeyValuePairs["label"], (JObject)item.Widgets[0]);
                            }
                            break;
                        case "GAUGE":
                            if (itemKeyValuePairs.ContainsKey("label"))
                            {
                                Widgets.Gauge(grid, px, py, itemKeyValuePairs["label"], (JObject)item.Widgets[0]);
                            }
                            break;
                        case "GAUGE-GROUP":
                            if (itemKeyValuePairs.ContainsKey("label"))
                            {
                                Widgets.Gauge_Group(grid, px, py, sx, sy, rx, ry, itemKeyValuePairs["label"], JArray.FromObject(item.Widgets));
                            }
                            break;
                        case "IMAGE":
                            if (!itemKeyValuePairs.ContainsKey("aspect")) { itemKeyValuePairs.Add("aspect", "aspectfill"); }

                            if (itemKeyValuePairs.ContainsKey("label"))
                            {
                                Widgets.Image(grid, px, py, sx, sy, itemKeyValuePairs["label"], itemKeyValuePairs["aspect"], (JObject)item.Widgets[0]);
                            }
                            break;
                        case "LAUNCHER":
                            if (itemKeyValuePairs.ContainsKey("label"))
                            {
                                Widgets.Launcher(grid, px, py, sx, sy, itemKeyValuePairs["label"], (JObject)item.Widgets[0]);
                            }
                            break;
                        case "MAP":
                            if (!itemKeyValuePairs.ContainsKey("type")) { itemKeyValuePairs.Add("type", "street"); }
                            Widgets.Map(grid, px, py, sx, sy, itemKeyValuePairs["type"], JArray.FromObject(item.Widgets));
                            break;
                        case "NUMERICINPUT":
                            if (itemKeyValuePairs.ContainsKey("label"))
                            {
                                Widgets.NumericInput(grid, px, py, sx, sy, itemKeyValuePairs["label"], (JObject)item.Widgets[0]);
                            }
                            break;
                        case "SENSOR":
                            if (itemKeyValuePairs.ContainsKey("label"))
                            {
                                Widgets.Sensor(grid, px, py, sx, sy, itemKeyValuePairs["label"], (JObject)item.Widgets[0]);
                            }
                            break;
                        case "SWITCH":
                            if (itemKeyValuePairs.ContainsKey("label"))
                            {
                                Widgets.Switch(grid, px, py, sx, sy, itemKeyValuePairs["label"], (JObject)item.Widgets[0]);
                            }
                            break;
                        case "VOICE":
                            if (itemKeyValuePairs.ContainsKey("label"))
                            {
                                Widgets.Voice(grid, px, py, sx, sy, itemKeyValuePairs["label"], (JObject)item.Widgets[0]);
                            }
                            break;
                        case "WEATHER":
                            if (itemKeyValuePairs.ContainsKey("label"))                            
                            {
                                Widgets.Weather(grid, px, py, sx, sy, itemKeyValuePairs["label"], JArray.FromObject(item.Widgets));
                            }
                            break;
                        case "WEATHERFORECAST":
                            if (itemKeyValuePairs.ContainsKey("label"))
                            {
                                Widgets.WeatherForecast(grid, px, py, sx, sy, itemKeyValuePairs["label"], JArray.FromObject(item.Widgets));
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
                cp.IconImageSource = icon;
            }
            cp.BackgroundColor = App.Config.BackGroundColor;
            cp.Title = title;
            cp.Content = grid;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    cp.Padding = new Thickness(0, 20, 0, 0);
                    break;
                default:
                    cp.Padding = new Thickness(0, 0, 0, 0);
                    break;
            }
            App.Tp.Children.Add(cp);

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
            grid.BackgroundColor = App.Config.BackGroundColor;
        }

        public void GetUpdates()
        {
            Task.Run(async () =>
            {
                await new RestService().GetUpdateAsync();
            });
        }
    }
}