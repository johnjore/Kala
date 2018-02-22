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
            int.TryParse(x1, out int px);
            int.TryParse(y1, out int py);

            try
            {
                Grid t_grid = Create_GaugeHeader(header, 1, 1, 1);

                Models.Sitemap.Widget3 item = data.ToObject<Models.Sitemap.Widget3>();

                Create_Gauge(t_grid, 0, item);
                grid.Children.Add(t_grid, px, py);
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Gauge", "Crashed: " + ex.ToString());
                Error(grid, px, py, 1, 1, ex.ToString());
            }
        }

        private static Grid Create_GaugeHeader(string header, int sx, int rx, int ry)
        {
            #region t_grid
            Grid t_grid = new Grid
            {
                Padding = new Thickness(3, 3, 3, 3),
                RowSpacing = 5,
                ColumnSpacing = 5,
                BackgroundColor = App.config.CellColor,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            #endregion t_grid

            for (int r = 0; r < rx; r++)
            {
                t_grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            //Header
            t_grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            //Body
            for (int r = 0; r < ry; r++)
            {
                t_grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }

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
            t_grid.Children.Add(l_header, 0, 0 + rx, 0, 1);

            return t_grid;
        }

        private static void Create_Gauge(Grid t_grid, int i, Models.Sitemap.Widget3 item, int x, int y)
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
                }, x, y);
                #endregion Center unit

                #region Image
                string strSource = widgetKeyValuePairs["icon"];         

                t_grid.Children.Add(new Image
                {
                    Source = strSource,
                    Aspect = Aspect.AspectFill,
                    BackgroundColor = App.config.CellColor,
                    VerticalOptions = LayoutOptions.End,
                    HorizontalOptions = LayoutOptions.Center
                }, x, y);
                #endregion Image

                #region Center Text / Value
                string s_value = "N/A";
                if (!(digits.Item1.ToLower().Equals("uninitialized")))
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
                    Name = item.item.name,
                    Digits = digits.Item2
                };
                App.config.itemlabels.Add(l_value);
                t_grid.Children.Add(l_value, x, y);

                Single.TryParse(widgetKeyValuePairs["min"].Replace(".", CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator), out float min);
                Single.TryParse(widgetKeyValuePairs["max"].Replace(".", CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator), out float max);
                Single.TryParse(digits.Item1.Replace(".", CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator), out float value);

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
                }, x, y);

                ShapeView progressArc = new ShapeView
                {
                    ShapeType = ShapeType.Arc,
                    StrokeColor = App.config.ValueColor,
                    StrokeWidth = 1.0f,
                    Scale = 3.0,
                    Padding = 1,
                    IndicatorPercentage = (float)((value - min) / (max - min) * 100.0f),   //Calculate indicator percentage
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Name = item.item.name,
                    Min = min,
                    Max = max
                };
                App.config.itemShapeViews.Add(progressArc);
                t_grid.Children.Add(progressArc, x, y);
                #endregion Arc

                //Button must be last to be added to work
                Button dummyButton = new Button
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Color.Transparent,
                };
                t_grid.Children.Add(dummyButton, x, y);
                dummyButton.Clicked += OnDummyButtonClicked;
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Gauge", "Crashed: " + ex.ToString());
                Error(t_grid, i, 1, 1, 1, ex.ToString());
            }
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
                string s_value = "N/A";
                if (!(digits.Item1.ToLower().Equals("uninitialized")))
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
                    Name = item.item.name,
                    Digits = digits.Item2
                };
                App.config.itemlabels.Add(l_value);
                t_grid.Children.Add(l_value, i, 1);

                Single.TryParse(widgetKeyValuePairs["min"].Replace(".", CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator), out float min);
                Single.TryParse(widgetKeyValuePairs["max"].Replace(".", CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator), out float max);
                Single.TryParse(digits.Item1.Replace(".", CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator), out float value);

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

                ShapeView progressArc = new ShapeView
                {
                    ShapeType = ShapeType.Arc,
                    StrokeColor = App.config.ValueColor,
                    StrokeWidth = 1.0f,
                    Scale = 3.0,
                    Padding = 1,
                    IndicatorPercentage = (float)((value - min) / (max - min) * 100.0f),   //Calculate indicator percentage
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Name = item.item.name,
                    Min = min,
                    Max = max
                };
                App.config.itemShapeViews.Add(progressArc);
                t_grid.Children.Add(progressArc, i, 1);
                #endregion Arc

                //Button must be last to be added to work
                Button dummyButton = new Button
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Color.Transparent,
                };
                t_grid.Children.Add(dummyButton, i, 1);
                dummyButton.Clicked += OnDummyButtonClicked;
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Gauge", "Crashed: " + ex.ToString());
                Error(t_grid, i, 1, 1, 1, ex.ToString());
            }
        }

    }
}
