using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using Plugin.Logger;

namespace Kala
{
    public partial class Widgets
    {
        public static void Weather(Grid grid, string x1, string y1, string x2, string y2, string header, JArray data)
        {
            try
            {
                //Size of Weather widget
                int.TryParse(x1, out int px);
                int.TryParse(y1, out int py);
                int.TryParse(x2, out int sx);
                int.TryParse(y2, out int sy);

                //Items in Weather widget
                List<Models.Sitemap.Widget3> items = data.ToObject<List<Models.Sitemap.Widget3>>();
                CrossLogger.Current.Debug("Weather", "Items: " + items.Count.ToString());

                #region w_grid
                //Create grid layout
                Grid w_grid = new Grid();
                grid.Children.Add(w_grid, px, px+sx, py, py+sy);

                w_grid.RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(2) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                };
                w_grid.ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = new GridLength(110) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = GridLength.Auto },
                };
                w_grid.Padding = new Thickness(0, 20, 0, 2);
                w_grid.RowSpacing = 0;
                w_grid.ColumnSpacing = 0;
                w_grid.BackgroundColor = App.config.CellColor;
                w_grid.VerticalOptions = LayoutOptions.FillAndExpand;
                w_grid.HorizontalOptions = LayoutOptions.FillAndExpand;
                #endregion w_grid

                #region t_grid
                //Create grid layout for status bar
                //How many rows & columns in status field
                int status_c = 0;
                int status_r = 0;
                foreach (Models.Sitemap.Widget3 item in items)
                {
                    Dictionary<string, string> widgetKeyValuePairs = Helpers.SplitCommand(item.label);
                    if (!widgetKeyValuePairs.ContainsKey("item"))
                    {
                        int tmp_c = Convert.ToInt16(widgetKeyValuePairs["px"]);
                        int tmp_r = Convert.ToInt16(widgetKeyValuePairs["py"]);
                        if (tmp_c > status_c) status_c = tmp_c;
                        //if (tmp_r > status_r) status_r = tmp_r;
                    }
                }

                Grid t_grid = new Grid
                {
                    RowDefinitions = new RowDefinitionCollection()
                };
                for (int i = 0; i <= status_r; i++)
                {
                    t_grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                };

                t_grid.ColumnDefinitions = new ColumnDefinitionCollection();
                for (int i = 0; i <= status_c; i++)
                {
                    t_grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                }
                t_grid.Padding = new Thickness(0, 0, 0, 0);
                t_grid.RowSpacing = 0;
                t_grid.ColumnSpacing = 0;
                t_grid.BackgroundColor = App.config.CellColor;
                t_grid.VerticalOptions = LayoutOptions.FillAndExpand;
                t_grid.HorizontalOptions = LayoutOptions.FillAndExpand;
                w_grid.Children.Add(t_grid, 0, 0 + 3, 3, 3 + 1); //Add in bottom row, across all columns in w_grid
                #endregion t_grid

                #region Separator
                //Boxview (Line)
                BoxView bv = new BoxView
                {
                    Color = App.config.BackGroundColor
                };
                w_grid.Children.Add(bv, 0, 0+3, 2, 2+1);
                #endregion Separator

                #region Header
                //Header (Location)
                ItemLabel l_header = new ItemLabel
                {
                    Text = header,
                    FontSize = 30,
                    TextColor = App.config.TextColor,
                    BackgroundColor = App.config.CellColor,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.End,
                    TranslationY = -5
                };
                w_grid.Children.Add(l_header, 1, 0);
                #endregion Header

                #region Fields
                foreach (Models.Sitemap.Widget3 item in items)
                {
                    int counter = 0;
                    Dictionary<string, string> widgetKeyValuePairs = Helpers.SplitCommand(item.label);
                                        
                    if (widgetKeyValuePairs.ContainsKey("item"))
                    #region mandatory
                    {
                        switch (widgetKeyValuePairs["item"].ToUpper())
                        {
                            case "CONDITION-CAPTION":
                                ItemLabel l_condition = new ItemLabel
                                {
                                    Text = item.item.state,
                                    FontSize = 20,
                                    TextColor = App.config.TextColor,
                                    BackgroundColor = App.config.CellColor,
                                    HorizontalOptions = LayoutOptions.Start,
                                    VerticalOptions = LayoutOptions.Start,
                                    Name = item.item.name,
                                    TranslationY = -5
                                };
                                App.config.itemlabels.Add(l_condition);
                                w_grid.Children.Add(l_condition, 1, 1);
                                break;
                            case "TEMPERATURE":
                                var temp = Digits(widgetKeyValuePairs, item.item.state);

                                ItemLabel l_temperature = new ItemLabel
                                {
                                    Post = "\u00B0" + widgetKeyValuePairs["unit"] + " ",
                                    Text = temp.Item1.ToString() + "\u00B0" + widgetKeyValuePairs["unit"] + " ",
                                    FontSize = 40,
                                    TextColor = App.config.TextColor,
                                    BackgroundColor = App.config.CellColor,
                                    HorizontalOptions = LayoutOptions.End,
                                    VerticalOptions = LayoutOptions.FillAndExpand,
                                    Digits = temp.Item2,
                                    Name = item.item.name,
                                    TranslationY = -5
                                };
                                App.config.itemlabels.Add(l_temperature);
                                w_grid.Children.Add(l_temperature, 2, 2 + 1, 0, 0 + 2);
                                break;
                            case "CONDITION":
                                string strFontFamily = null;
                                switch (Device.RuntimePlatform)
                                {
                                    case Device.Android:
                                        strFontFamily = "weathericons-regular-webfont.ttf#Weather Icons";
                                        break;
                                }

                                ItemLabel l_image = new ItemLabel
                                {
                                    Text = WeatherCondition(item.item.state),
                                    TextColor = App.config.TextColor,
                                    FontFamily = strFontFamily,
                                    FontSize = 68,
                                    HorizontalOptions = LayoutOptions.Center,
                                    VerticalOptions = LayoutOptions.StartAndExpand,
                                    Name = item.item.name,
                                    Type = Models.Itemtypes.Weathericon,
                                    TranslationY = -15
                                };
                                App.config.itemlabels.Add(l_image);
                                w_grid.Children.Add(l_image, 0, 0 + 1, 0, 0 + 2);
                                break;
                            case "default":
                                break;
                        }
                    }
                    #endregion mandatory
                    else if (widgetKeyValuePairs.ContainsKey("widget"))
                    {
                        switch (widgetKeyValuePairs["widget"].ToUpper())
                        {
                            case "WIND":
                                CrossLogger.Current.Debug("Weather", "Wind");
                                List<Models.Sitemap.Widget3> winds = (JArray.FromObject(item.widgets)).ToObject<List<Models.Sitemap.Widget3>>();
                                
                                //Wind direction and speed
                                int w_direction = 0;
                                string wind_direction_name = string.Empty;
                                string wind_speed = string.Empty;
                                string wind_speed_name = string.Empty;
                                int digits = -1;
                                string wind_speed_post = string.Empty;

                                foreach (Models.Sitemap.Widget3 wind in winds)
                                {
                                    Dictionary<string, string> windKeyValuePairs = Helpers.SplitCommand(wind.label);
                                    switch (windKeyValuePairs["item"].ToUpper())
                                    {
                                        case "WIND-DIRECTION":
                                            Helpers.wind_direction.TryGetValue(wind.item.state.ToLower(), out w_direction);
                                            wind_direction_name = wind.item.name;
                                            break;
                                        case "WIND-SPEED":
                                            var tmpdigits = Digits(windKeyValuePairs, wind.item.state);
                                            digits = tmpdigits.Item2;
                                            wind_speed = tmpdigits.Item1;
                                            wind_speed_name = wind.item.name;
                                            wind_speed_post = " " + windKeyValuePairs["unit"];
                                            break;
                                        default:
                                            CrossLogger.Current.Warn("Weather", "Unknown item");
                                            break;
                                    }
                                }
                                
                                string strFontFamily = null;
                                switch (Device.RuntimePlatform)
                                {
                                    case Device.Android:
                                        strFontFamily = "weathericons-regular-webfont.ttf#Weather Icons";
                                        break;
                                }

                                ItemLabel l_winddirection = new ItemLabel
                                {
                                    Type = Models.Itemtypes.Winddirection,   //Special. Rotate label, depending on item value
                                    Text = "\uf0b1",
                                    FontSize = 30,
                                    FontFamily = strFontFamily,
                                    TextColor = App.config.TextColor,
                                    Rotation = w_direction,
                                    BackgroundColor = App.config.CellColor,
                                    HorizontalOptions = LayoutOptions.Center,
                                    VerticalOptions = LayoutOptions.Center,
                                    TranslationX = -45,
                                    Name = wind_direction_name
                                };
                                App.config.itemlabels.Add(l_winddirection);
                                t_grid.Children.Add(l_winddirection, Convert.ToInt16(widgetKeyValuePairs["px"]), Convert.ToInt16(widgetKeyValuePairs["py"]));

                                ItemLabel l_windspeed = new ItemLabel
                                {
                                    Text = wind_speed + wind_speed_post,
                                    FontSize = 20,
                                    TextColor = App.config.TextColor,
                                    BackgroundColor = App.config.CellColor,
                                    HorizontalOptions = LayoutOptions.Center,
                                    VerticalOptions = LayoutOptions.Center,
                                    TranslationX = 20,
                                    Name = wind_speed_name,
                                    Post = wind_speed_post,
                                    Digits = digits
                                };
                                App.config.itemlabels.Add(l_windspeed);
                                t_grid.Children.Add(l_windspeed, Convert.ToInt16(widgetKeyValuePairs["px"]), Convert.ToInt16(widgetKeyValuePairs["py"]));
                                break;
                            default:
                                CrossLogger.Current.Warn("Weather", "Unknown frame type");
                                break;
                        }

                        CrossLogger.Current.Debug("Weather", "Items: " + items.Count.ToString());
                    }
                    else
                    {
                        var digits = Digits(widgetKeyValuePairs, item.item.state);

                        string strFontFamily = null;
                        switch (Device.RuntimePlatform)
                        {
                            case Device.Android:
                                strFontFamily = "weathericons-regular-webfont.ttf#Weather Icons";
                                break;
                        }

                        ItemLabel l1 = new ItemLabel
                        {
                            Text = widgetKeyValuePairs["font"] + "  " + digits.Item1 + " " + widgetKeyValuePairs["unit"],
                            FontSize = 20,
                            FontFamily = strFontFamily,
                            TextColor = App.config.TextColor,
                            BackgroundColor = App.config.CellColor,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center,
                            Pre = widgetKeyValuePairs["font"] + "  ",
                            Name = item.item.name,
                            Post = " " + widgetKeyValuePairs["unit"],
                            Digits = digits.Item2
                        };
                        App.config.itemlabels.Add(l1);
                        t_grid.Children.Add(l1, Convert.ToInt16(widgetKeyValuePairs["px"]), Convert.ToInt16(widgetKeyValuePairs["py"]));

                        CrossLogger.Current.Debug("Weather", "No item defined: " + counter++.ToString() + ", font:"  + widgetKeyValuePairs["font"] + ", pos: " + widgetKeyValuePairs["px"]);
                    }
                }
                #endregion Fields
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Weather", "Crashed:" + ex.ToString());
            }
        }
    }
}
