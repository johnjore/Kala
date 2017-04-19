using System;
using Xamarin.Forms;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Plugin.Logger;

namespace Kala
{
    public partial class Widgets
    {
        public static void WeatherForecast(Grid grid, string x1, string y1, string x2, string y2, string header, JArray data)
        {
            CrossLogger.Current.Debug("WeatherForecast", "Creating Weather forecast :" + data.ToString());

            List<Models.Sitemap.Widget3> items = null;
            int px = 0;
            int py = 0;
            int sx = 0;
            int sy = 0;

            try
            {
                //Position & Size
                px = Convert.ToInt16(x1);
                py = Convert.ToInt16(y1);
                sx = Convert.ToInt16(x2);
                sy = Convert.ToInt16(y2);

                //Items
                items = data.ToObject<List<Models.Sitemap.Widget3>>();
                CrossLogger.Current.Debug("WeatherForecast", "Nr of items: " + items.Count.ToString());
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("WeatherForecast", "Crashed:" + ex.ToString());
            }

            try
            {
                #region t_grid
                Grid t_grid = new Grid();
                t_grid.Padding = new Thickness(0, 0, 0, 0);
                t_grid.RowSpacing = 0;
                t_grid.ColumnSpacing = 0;
                t_grid.BackgroundColor = App.config.CellColor;
                t_grid.VerticalOptions = LayoutOptions.FillAndExpand;
                t_grid.HorizontalOptions = LayoutOptions.FillAndExpand;

                //Rows
                t_grid.RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                    //new RowDefinition { Height = GridLength.Auto },
                    //new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                };
                
                //Columns
                for (int i = 0; i < items.Count; i++)            //Each day has 3 items
                {
                    int day = 0;
                    Dictionary<string, string> widgetKeyValuePairs = Helpers.SplitCommand(items[i].label);
                    if (widgetKeyValuePairs.ContainsKey("item"))
                    {
                        day = Convert.ToInt16(widgetKeyValuePairs["item"].Substring(3,1));
                    }
                    var digits = Digits(widgetKeyValuePairs, items[i].item.state);

                    #region Header
                    if (i % 3 == 0)
                    {
                        string DayOfWeek = "Today";
                        if (i / 3 != 0)
                        {
                            DayOfWeek = DateTime.Now.AddDays(i/3).DayOfWeek.ToString().Substring(0, 3);
                        }

                        ItemLabel l_header = new ItemLabel
                        {
                            Text = DayOfWeek,
                            FontSize = 20,
                            TextColor = App.config.TextColor,
                            BackgroundColor = App.config.CellColor,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Start
                        };
                        t_grid.Children.Add(l_header, i / 3, 0);
                    }
                    #endregion Header

                    #region Condition
                    if (items[i].label.Contains("condition"))
                    {
                        string strFontFamily = null;
                        switch (Device.RuntimePlatform)
                        {
                            case Device.Android:
                                strFontFamily = "weathericons-regular-webfont.ttf#Weather Icons";
                                break;
                        }

                        ItemLabel l_image = new ItemLabel
                        {
                            Text = WeatherCondition(items[i].item.state),
                            TextColor = App.config.TextColor,
                            FontFamily = strFontFamily,
                            FontSize = 68,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center,
                            TranslationY = -5,
                            Link = items[i].item.link,
                            Type = Models.Itemtypes.Weathericon
                        };
                        App.config.itemlabels.Add(l_image);
                        t_grid.Children.Add(l_image, day, 0);
                    }
                    #endregion Condition

                    #region Temperature
                    if (items[i].label.Contains("temperature-high"))
                    {
                        ItemLabel l_temp_high = new ItemLabel
                        {
                            FontSize = 20,
                            TextColor = App.config.TextColor,
                            BackgroundColor = App.config.CellColor,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.End,
                            TranslationX = -30,
                            Link = items[i].item.link,
                            Pre = "   ",
                            Post = "\u00B0",
                            Text = digits.Item1 + " \u00B0",
                            Digits = digits.Item2
                        };
                        App.config.itemlabels.Add(l_temp_high);
                        t_grid.Children.Add(l_temp_high, day, 0);
                    }

                    if (items[i].label.Contains("temperature-low"))
                    {
                        ItemLabel l_temp_low = new ItemLabel
                        {
                            FontSize = 20,
                            TextColor = App.config.TextColor,
                            BackgroundColor = App.config.CellColor,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.End,
                            TranslationX = 30,
                            Link = items[i].item.link,
                            Post = " \u00B0",
                            Text = digits.Item1 + " \u00B0  ",
                            Digits = digits.Item2
                        };
                        App.config.itemlabels.Add(l_temp_low);
                        t_grid.Children.Add(l_temp_low, day, 0);
                    }
                    #endregion Temperature
                }

                grid.Children.Add(t_grid, px, px + sx, py, py + sy);
                #endregion t_grid
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("WeatherForecast", "Crashed: " + ex.ToString());
                Error(grid, px, py, ex.ToString());
            }
        }
    }
}
