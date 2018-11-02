using System;
using Xamarin.Forms;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Plugin.Logger;

namespace Kala
{
    public partial class Widgets : ContentPage
    {
        public static void WeatherForecast(Grid grid, string x1, string y1, string x2, string y2, string header, JArray data)
        {
            CrossLogger.Current.Debug("WeatherForecast", "Creating Weather forecast :" + data.ToString());

            int.TryParse(x1, out int px);
            int.TryParse(y1, out int py);
            int.TryParse(x2, out int sx);
            int.TryParse(y2, out int sy);

            try
            {
                List<Models.Sitemap.Widget3> items = data.ToObject<List<Models.Sitemap.Widget3>>();

                #region t_grid
                Grid t_grid = new Grid
                {
                    Padding = new Thickness(0, 0, 0, 0),
                    RowSpacing = 0,
                    ColumnSpacing = 0,
                    BackgroundColor = App.Config.CellColor,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,

                    RowDefinitions = new RowDefinitionCollection
                    {
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    }
                };

                //Columns
                for (int i = 0; i < items.Count; i++)            //Each day has 3 items
                {
                    int day = 0;
                    Dictionary<string, string> widgetKeyValuePairs = Helpers.SplitCommand(items[i].Label);
                    if (widgetKeyValuePairs.ContainsKey("item"))
                    {
                        day = Convert.ToInt16(widgetKeyValuePairs["item"].Substring(3,1));
                    }
                    var digits = Digits(widgetKeyValuePairs, items[i].Item.State);

                    #region Header
                    if (i % 3 == 0)
                    {
                        string DayOfWeek = "Today";
                        int DayNumber = -1;
                        if (i / 3 != 0)
                        {                            
                            DayNumber = (int)DateTime.Now.AddDays(i / 3.0).DayOfWeek;                            
                            DayOfWeek = ((DayOfWeek)DayNumber).ToString().Substring(0, 3);
                        }

                        ItemLabel l_header = new ItemLabel
                        {
                            Digits = DayNumber,
                            Text = DayOfWeek,
                            FontSize = 20,
                            TextColor = App.Config.TextColor,
                            BackgroundColor = App.Config.CellColor,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Start,
                            Type = Models.Itemtypes.NameOfDay,                        
                        };
                        App.Config.Itemlabels.Add(l_header);
                        t_grid.Children.Add(l_header, i / 3, 0);                        
                    }
                    #endregion Header

                    #region Condition
                    if (items[i].Label.Contains("condition"))
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
                            Text = WeatherCondition(items[i].Item.State),
                            TextColor = App.Config.TextColor,
                            FontFamily = strFontFamily,
                            FontSize = 68,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center,
                            TranslationY = -5,
                            Name = items[i].Item.Name,
                            Type = Models.Itemtypes.Weathericon
                        };
                        App.Config.Itemlabels.Add(l_image);
                        t_grid.Children.Add(l_image, day, 1);
                    }
                    #endregion Condition

                    #region Temperature
                    if (items[i].Label.Contains("temperature-high"))
                    {
                        ItemLabel l_temp_high = new ItemLabel
                        {
                            FontSize = 20,
                            TextColor = App.Config.TextColor,
                            BackgroundColor = App.Config.CellColor,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.End,
                            TranslationX = -30,
                            Name = items[i].Item.Name,
                            Pre = "   ",
                            Post = "\u00B0",
                            Text = digits.Item1 + " \u00B0",
                            Digits = digits.Item2
                        };
                        App.Config.Itemlabels.Add(l_temp_high);
                        t_grid.Children.Add(l_temp_high, day, 2);
                    }

                    if (items[i].Label.Contains("temperature-low"))
                    {
                        ItemLabel l_temp_low = new ItemLabel
                        {
                            FontSize = 20,
                            TextColor = App.Config.TextColor,
                            BackgroundColor = App.Config.CellColor,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.End,
                            TranslationX = 30,
                            Name = items[i].Item.Name,
                            Post = " \u00B0",
                            Text = digits.Item1 + " \u00B0  ",
                            Digits = digits.Item2
                        };
                        App.Config.Itemlabels.Add(l_temp_low);
                        t_grid.Children.Add(l_temp_low, day, 2);
                    }
                    #endregion Temperature
                }

                //Button must be last to be added to work
                Button dummyButton = new Button
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Color.Transparent,
                };
                t_grid.Children.Add(dummyButton, px, px + sx, py, py + sy);
                dummyButton.Clicked += OnDummyButtonClicked;
                
                grid.Children.Add(t_grid, px, px + sx, py, py + sy);
                #endregion t_grid

                // Check if we've crossed midnight as first entry is now yesterday
                DayOfWeek Today = DateTime.Today.DayOfWeek;
                Device.StartTimer(TimeSpan.FromSeconds(60), () =>
                {
                    if (DateTime.Today.DayOfWeek != Today)
                    {
                        CrossLogger.Current.Debug("WeatherForecast", "Update WeatherForecast DayOfWeek as we've crossed midnight");
                        Today = DateTime.Today.DayOfWeek;

                        foreach (ItemLabel lbl in App.Config.Itemlabels)
                        {
                            if (lbl.Type.Equals(Models.Itemtypes.NameOfDay) && (lbl.Digits != -1))
                            {
                                lbl.Digits = (lbl.Digits + 1) % 7;
                                lbl.Text = ((DayOfWeek)lbl.Digits).ToString().Substring(0, 3);
                            }
                        }
                    }

                    return true;
                });
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("WeatherForecast", "Crashed: " + ex.ToString());
                Error(grid, px, py, sx, sy, ex.ToString());
            }
        }
    }
}
