using System;
using System.Globalization;
using System.Collections.Generic;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using Plugin.Logger;
using DrawShape;

namespace Kala
{
    public partial class Widgets
    {
        public static void Gauge(Grid grid, string x1, string y1, string header, JObject data)
        {
            int px = 0;
            int py = 0;

            //If this fails, we dont know where to show an error
            try
            {
                px = Convert.ToInt16(x1);
                py = Convert.ToInt16(y1);
                
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Gauge", "Crashed: " + ex.ToString());
            }

            try
            {
                //Create Grid w/header
                Grid t_grid = Create_GaugeHeader(header, 1);

                //Item
                Models.Sitemap.Widget3 item = data.ToObject<Models.Sitemap.Widget3>();

                //Column
                Create_Gauge(t_grid, 0, item);

                grid.Children.Add(t_grid, px, py);
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Gauge", "Crashed: " + ex.ToString());
                Error(grid, px, py, ex.ToString());
            }
        }

        private static Grid Create_GaugeHeader(string header, int sx)
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
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                };
            #endregion t_grid

            //Header is in Row 0
            ItemLabel l_header = new ItemLabel
            {
                Text = header,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = App.config.TextColor,
                BackgroundColor = App.config.CellColor,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Start
            };
            t_grid.Children.Add(l_header, 0, 0 + sx, 0, 1);

            return t_grid;
        }

        private static void Create_Gauge(Grid t_grid, int i, Models.Sitemap.Widget3 item)
        {
            try
            {
                Dictionary<string, string> widgetKeyValuePairs = Helpers.SplitCommand(item.label);
                var digits = Digits(widgetKeyValuePairs, item.item.state);

                #region Center unit
                t_grid.Children.Add(new Label
                {
                    Text = widgetKeyValuePairs["unit"],
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    TextColor = App.config.TextColor,
                    BackgroundColor = App.config.CellColor,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    TranslationY = 5
                }, i, 1);
                #endregion Center unit

                #region Image
                string strSource = widgetKeyValuePairs["icon"];
                switch (Device.RuntimePlatform)
                {
                    case Device.WinPhone:
                        strSource = "Assets/" + widgetKeyValuePairs["icon"];
                        break;
                }

                t_grid.Children.Add(new Image
                {
                    Source = strSource,
                    Aspect = Aspect.AspectFill,
                    BackgroundColor = App.config.CellColor,
                    VerticalOptions = LayoutOptions.End,
                    HorizontalOptions = LayoutOptions.Center
                }, i, 1);
                #endregion Image

                #region Center Text / Value
                string s_value = String.Empty;
                if (digits.Item1.ToLower().Equals("uninitialized"))
                    s_value = "N/A";
                else
                    s_value = digits.Item1;

                ItemLabel l_value = new ItemLabel
                {
                    Text = s_value,
                    FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                    TextColor = App.config.TextColor,
                    BackgroundColor = App.config.CellColor,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    TranslationY = -20,
                    Link = item.item.link,
                    Digits = digits.Item2
                };
                App.config.itemlabels.Add(l_value);
                t_grid.Children.Add(l_value, i, 1);
                #endregion Center Text / Value

                #region Arc
                float min = 0.0f;
                float max = 0.0f;
                float value = 0.0f;
                Single.TryParse(widgetKeyValuePairs["min"].Replace(".", CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator), out min);
                Single.TryParse(widgetKeyValuePairs["max"].Replace(".", CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator), out max);
                Single.TryParse(digits.Item1.Replace(".", CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator), out value);

                //Basic sanity checks
                if (value > max) max = value;
                if (value < min) min = value;

                //Handle negative ranges
                if (min < 0)
                {
                    max += Math.Abs(min);
                    value += Math.Abs(min);
                    min = 0;
                }

                t_grid.Children.Add(new ShapeView()
                {
                    ShapeType = ShapeType.Arc,
                    StrokeColor = App.config.BackGroundColor,
                    StrokeWidth = 1.0f,
                    Scale = 3.0,
                    Padding = 1,
                    IndicatorPercentage = 100,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }, i, 1);

                if (item.item.link.Contains("Sensor_MasterBedroom_Temperature"))
                {
                    CrossLogger.Current.Error("Update", "1");
                }

                ShapeView progressArc = new ShapeView
                {
                    ShapeType = ShapeType.Arc,
                    StrokeColor = App.config.ValueColor,
                    StrokeWidth = 1.0f,
                    Scale = 3.0,
                    Padding = 1,
                    IndicatorPercentage = ((value - min) / (max - min) * 100.0f),   //Calculate indicator percentage
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Link = item.item.link,
                    Min = min,
                    Max = max
                };
                App.config.itemShapeViews.Add(progressArc);
                t_grid.Children.Add(progressArc, i, 1);
                #endregion Arc              
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Gauge", "Crashed: " + ex.ToString());
                Error(t_grid, i, 1, ex.ToString());
            }
        }
    }
}
