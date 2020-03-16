using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using Plugin.Logger;
using Microcharts;
using Microcharts.Forms;
using SkiaSharp;
using Entry = Microcharts.Entry;

namespace Kala
{
    public partial class Widgets : ContentPage
    {
        public static void Chart(Grid grid, int px, int py, int sx, int sy, string header, JObject data)
        {
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Create Chart Widget");
            CrossLogger.Current.Debug("Chart", "Creating Chart Widget");

            try
            {
                Models.Sitemap.Widget3 item = data.ToObject<Models.Sitemap.Widget3>();
                //List<Models.Sitemap.Widget3> items = data.ToObject<List<Models.Sitemap.Widget3>>();


                #region w_grid
                Grid w_grid = new Grid
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
                        new RowDefinition { Height = GridLength.Auto },
                    },
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    },
                };
                grid.Children.Add(w_grid, px, px + sx, py, py + sy);
                #endregion w_grid


                List<Entry> entries = new List<Entry>
                {
                    new Entry(200)
                    {
                        Label = "January",
                        ValueLabel = "200",
                        Color = SKColor.Parse("#266489")
                    },
                    new Entry(400)
                    {
                        Label = "February",
                        ValueLabel = "400",
                        Color = SKColor.Parse("#68B9C0")
                    },
                    new Entry(-100)
                    {
                        Label = "March",
                        ValueLabel = "-100",
                        Color = SKColor.Parse("#90D585")
                    }
                };
                var cv = new ChartView();
                cv.Chart = new BarChart() { Entries = entries };
                
                // or: var chart = new PointChart() { Entries = entries };
                // or: var chart = new LineChart() { Entries = entries };
                // or: var chart = new DonutChart() { Entries = entries };
                // or: var chart = new RadialGaugeChart() { Entries = entries };
                // or: var chart = new RadarChart() { Entries = entries };

                #region Button
                Button dummyButton = new Button
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Color.Transparent,
                };
                grid.Children.Add(dummyButton, px, px + sx, py, py + sy);
                dummyButton.Clicked += OnDummyButtonClicked;
                #endregion Button
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Chart", "Crashed: " + ex.ToString());
                Error(grid, px, py, 1, 1, ex.ToString());
            }
        }
    }
}
