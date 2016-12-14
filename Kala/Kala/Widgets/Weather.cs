using System;
using System.Diagnostics;
using Xamarin.Forms;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Kala
{
    public partial class Widgets
    {
        public static void Weather(Grid grid, string x1, string y1, string x2, string y2, string header, JArray data)
        {
            Debug.WriteLine("Weather: " + data.ToString());

            try
            {
                //Size of Weather widget
                int px = Convert.ToInt16(x1);
                int py = Convert.ToInt16(y1);
                int sx = Convert.ToInt16(x2);
                int sy = Convert.ToInt16(y2);

                //Items in Weather widget
                List<Models.Sitemap.Widget3> items = data.ToObject<List<Models.Sitemap.Widget3>>();
                Debug.WriteLine("Weather: " + items.Count.ToString());

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
                    new ColumnDefinition { Width = new GridLength(100) },
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

                Grid t_grid = new Grid();
                t_grid.RowDefinitions = new RowDefinitionCollection();
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
                #endregion Child Grids

                #region Separatur
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
                    VerticalOptions = LayoutOptions.End
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
                                    Link = item.item.link
                                };
                                App.config.itemlabels.Add(l_condition);
                                w_grid.Children.Add(l_condition, 1, 1);
                                break;
                            case "TEMPERATURE":
                                double temp = Math.Round(Convert.ToDouble(item.item.state), 1, MidpointRounding.AwayFromZero);
                                ItemLabel l_temperature = new ItemLabel
                                {
                                    Text = temp.ToString() + "\u00B0" + widgetKeyValuePairs["unit"] + " ",
                                    FontSize = 40,
                                    TextColor = App.config.TextColor,
                                    BackgroundColor = App.config.CellColor,
                                    HorizontalOptions = LayoutOptions.End,
                                    VerticalOptions = LayoutOptions.FillAndExpand,
                                    Link = item.item.link,
                                    Post = "\u00B0" + widgetKeyValuePairs["unit"] + " ",
                                };
                                App.config.itemlabels.Add(l_temperature);
                                w_grid.Children.Add(l_temperature, 2, 2 + 1, 0, 0 + 2);
                                break;
                            case "CONDITION":
                                string strImage = string.Empty;
                                switch (item.item.state.ToLower())
                                {
                                    case "thunder": strImage = "\uf01e"; break;
                                    case "storm": strImage = "\uf01e"; break;
                                    case "rain-and-snow": strImage = "\uf017"; break;
                                    case "rain-and-sleet": strImage = "\uf0b5"; break;
                                    case "snow-and-sleet": strImage = "\uff0b5"; break;
                                    case "freezing-drizzle": strImage = "\uf017"; break;
                                    case "few-showers": strImage = "\uf01a"; break;
                                    case "freezing-rain": strImage = "\uf017"; break;
                                    case "rain": strImage = "\uf018"; break;
                                    case "snow-flurries": strImage = "\uf064"; break;
                                    case "light-snow": strImage = "\uf064"; break;
                                    case "blowing-snow": strImage = "\uf064"; break;
                                    case "snow": strImage = "\uf01b"; break;
                                    case "sleet": strImage = "\uf0b5"; break;
                                    case "dust": strImage = "\uf063"; break;
                                    case "fog": strImage = "\uf014"; break;
                                    case "wind": strImage = "\uf012"; break;
                                    case "cold": strImage = "\uf076"; break;
                                    case "cloudy": strImage = "\uf013"; break;
                                    case "mostly-cloudy-night": strImage = "\uf086"; break;
                                    case "mostly-cloudy-day": strImage = "\uf002"; break;
                                    case "partly-cloudy-night": strImage = "\uf083"; break;
                                    case "partly-cloudy-day": strImage = "\uf00c"; break;
                                    case "clear-night": strImage = "\uf02e"; break;
                                    case "sunny": strImage = "\uf00d"; break;
                                    case "hot": strImage = "\uf072"; break;
                                    case "scattered-thunder": strImage = "\uf01e"; break;
                                    case "scattered-showers": strImage = "\uf01a"; break;
                                    case "thundershowers": strImage = "\uf01e"; break;
                                    case "snow-showers": strImage = "\uf01e"; break;
                                    case "scattered-thundershowers": strImage = "\uf01d"; break;
                                    default: strImage = "\uf07b"; break;
                                }
                                ItemLabel l_image = new ItemLabel
                                {
                                    Text = strImage,
                                    TextColor = App.config.TextColor,
                                    FontFamily = Device.OnPlatform(null, "weathericons-regular-webfont.ttf#Weather Icons", null),
                                    FontSize = 68,
                                    HorizontalOptions = LayoutOptions.Center,
                                    VerticalOptions = LayoutOptions.StartAndExpand,
                                    Link = item.item.link
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
                                Debug.WriteLine("Wind");
                                List<Models.Sitemap.Widget3> winds = ((JArray)item.widget).ToObject<List<Models.Sitemap.Widget3>>();

                                //Wind direction and speed
                                int w_direction = 0;
                                string wind_direction_url = string.Empty;
                                string wind_speed = string.Empty;
                                string wind_speed_url = string.Empty;

                                foreach (Models.Sitemap.Widget3 wind in winds)
                                {
                                    Dictionary<string, string> windKeyValuePairs = Helpers.SplitCommand(wind.label);
                                    switch (windKeyValuePairs["item"].ToUpper())
                                    {
                                        case "WIND-DIRECTION":
                                            Helpers.wind_direction.TryGetValue(wind.item.state.ToLower(), out w_direction);
                                            wind_direction_url = wind.item.link;
                                            break;
                                        case "WIND-SPEED":
                                            wind_speed = wind.item.state + " " + windKeyValuePairs["unit"];
                                            wind_speed_url = wind.item.link;
                                            break;
                                        default:
                                            Debug.WriteLine("Unknown item");
                                            break;
                                    }
                                }

                                ItemLabel l_winddirection = new ItemLabel
                                {
                                    Type = Models.Itemtypes.Winddirection,   //Special. Rotate label, depending on item value
                                    Text = "\uf0b1",
                                    FontSize = 30,
                                    FontFamily = Device.OnPlatform(null, "weathericons-regular-webfont.ttf#Weather Icons", null),
                                    TextColor = App.config.TextColor,
                                    Rotation = w_direction,
                                    BackgroundColor = App.config.CellColor,
                                    HorizontalOptions = LayoutOptions.Center,
                                    VerticalOptions = LayoutOptions.Center,
                                    TranslationX = -45,
                                    Link = wind_direction_url
                                };
                                App.config.itemlabels.Add(l_winddirection);
                                t_grid.Children.Add(l_winddirection, Convert.ToInt16(widgetKeyValuePairs["px"]), Convert.ToInt16(widgetKeyValuePairs["py"]));

                                ItemLabel l_windspeed = new ItemLabel
                                {
                                    Text = wind_speed,
                                    FontSize = 20,
                                    TextColor = App.config.TextColor,
                                    BackgroundColor = App.config.CellColor,
                                    HorizontalOptions = LayoutOptions.Center,
                                    VerticalOptions = LayoutOptions.Center,
                                    TranslationX = 20,
                                    Link = wind_speed_url
                                };
                                App.config.itemlabels.Add(l_windspeed);
                                t_grid.Children.Add(l_windspeed, Convert.ToInt16(widgetKeyValuePairs["px"]), Convert.ToInt16(widgetKeyValuePairs["py"]));

                                break;
                            default:
                                Debug.WriteLine("Unknown frame type");
                                break;
                        }
                        
                        Debug.WriteLine("Weather: " + items.Count.ToString());
                    }
                    else
                    {
                        ItemLabel l1 = new ItemLabel
                        {
                            Text = widgetKeyValuePairs["font"] + "  " + item.item.state + " " + widgetKeyValuePairs["unit"],
                            FontSize = 20,
                            FontFamily = Device.OnPlatform(null, "weathericons-regular-webfont.ttf#Weather Icons", null),
                            TextColor = App.config.TextColor,
                            BackgroundColor = App.config.CellColor,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center,
                            Pre = widgetKeyValuePairs["font"] + "  ",
                            Link = item.item.link,
                            Post = " " + widgetKeyValuePairs["unit"]
                        };
                        App.config.itemlabels.Add(l1);
                        t_grid.Children.Add(l1, Convert.ToInt16(widgetKeyValuePairs["px"]), Convert.ToInt16(widgetKeyValuePairs["py"]));

                        Debug.WriteLine("No item defined: " + counter++.ToString() + ", font:"  + widgetKeyValuePairs["font"] + ", pos: " + widgetKeyValuePairs["px"]);
                    }
                }
                #endregion Fields
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Weather crashed:" + ex.ToString());
            }
            Debug.WriteLine("Nr Labels " + App.config.itemlabels.Count.ToString());
        }


        public static void WindDirectionUpdate()
        {
            /*
            ItemLabel l_winddirection = new ItemLabel
            {
                Text = "\uf0b1",
                FontSize = 30,
                FontFamily = Device.OnPlatform(null, "weathericons-regular-webfont.ttf#Weather Icons", null),
                TextColor = App.config.TextColor,
                Rotation = w_direction,
                BackgroundColor = App.config.CellColor,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                TranslationX = -45,
                Link = wind_direction_url
            };
            App.config.itemlabels.Add(l_winddirection);
            */

        }
    }
}
