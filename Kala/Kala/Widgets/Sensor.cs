using System;
using System.Collections.Generic;
using Xamarin.Forms;
using DrawShape;
using Newtonsoft.Json.Linq;
using Plugin.Logger;
using CircularProgressBar.FormsPlugin.Abstractions;
using Kala.Models;

namespace Kala
{
    public partial class Widgets
    {
        public static void Sensor(Grid grid, string x1, string y1, string x2, string y2, string header, JObject data)
        {
            CrossLogger.Current.Debug("Sensor", "Creating Sensor Widget");

            int.TryParse(x1, out int px);
            int.TryParse(y1, out int py);
            int.TryParse(x2, out int sx);
            int.TryParse(y2, out int sy);

            Models.Sitemap.Widget3 item = null;
            Dictionary<string, string> widgetKeyValuePairs = null;

            //Master Grid for Widget
            Grid Widget_Grid = new Grid
            {
                RowDefinitions = new RowDefinitionCollection {
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                },
                ColumnDefinitions = new ColumnDefinitionCollection {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                },
                RowSpacing = 0,
                ColumnSpacing = 0,
                BackgroundColor = App.config.CellColor,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };
            grid.Children.Add(Widget_Grid, px, px + sx, py, py + sy);

            try
            {
                item = data.ToObject<Models.Sitemap.Widget3>();
                widgetKeyValuePairs = Helpers.SplitCommand(item.label);

                App.trackItem i = new App.trackItem
                {
                    grid = Widget_Grid,
                    name = item.item.name,
                    header = header,
                    icon = widgetKeyValuePairs["icon"],
                    state = item.item.state,
                    type = Itemtypes.Sensor
                };
                App.config.items.Add(i);

                Sensor_update(true, i);
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Sensor", "Sensor crashed: " + ex.ToString());
                Error(grid, px, py, sx, sy, ex.ToString());
            }
        }

        public static void Sensor_update(bool Create, App.trackItem item)
        {
            item.grid.Children.Clear();

            #region Header
            item.grid.Children.Add(new Label
            {
                Text = item.header,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = App.config.TextColor,
                BackgroundColor = App.config.CellColor,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Start
            }, 0, 0);
            #endregion Header 

            #region State
            if (!item.state.ToUpper().Equals("UNINITIALIZED"))
            {
                try
                {
                    if (item.state.ToUpper().Equals("CLOSED"))
                    {
                        Sensor_Off(item);
                    }
                    else
                    {
                       Sensor_On(item);
                    }
                }
                catch (Exception ex)
                {
                    Error(item.grid, 0, 0, 1, 1, ex.ToString());
                }
            }
            else
            {
                item.state = "N/A";
            }
            #endregion State

            #region Image     
            item.grid.Children.Add(new Image
            {
                Source = item.icon,
                Aspect = Aspect.AspectFill,
                BackgroundColor = Color.Transparent,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            }, 0, 0);
            #endregion Image

            #region Status Text
            item.grid.Children.Add (new ItemLabel
            {
                Text = item.state.ToUpper(),
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = App.config.TextColor,
                BackgroundColor = App.config.CellColor,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                TranslationY = -10,
                Name = item.name
            }, 0, 0);
            #endregion Status Text

            //Button must be last to be added to work
            Button dummyButton = new Button
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent,
            };
            item.grid.Children.Add(dummyButton, 0, 0);
            dummyButton.Clicked += OnDummyButtonClicked;
        }

        private static void Sensor_On(App.trackItem item)
        {
            item.grid.Children.Add(new ShapeView()
            {
                ShapeType = ShapeType.Circle,
                StrokeColor = App.config.ValueColor,
                Color = App.config.ValueColor,
                StrokeWidth = 10.0f,
                Scale = 2,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            }, 0, 0);
        }

        private static void Sensor_Off(App.trackItem item)
        {
            int intStrokeThickness = 2;
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    intStrokeThickness = 4;
                    break;
            }

            item.grid.Children.Add(new CircularProgressBarView
            {
                Progress = 100,
                StrokeThickness = intStrokeThickness,
                BackgroundColor = Color.Transparent,
                ProgressBackgroundColor = App.config.BackGroundColor,
                ProgressColor = App.config.ValueColor,
                Scale = 0.5f
            }, 0, 0);
        }
    }
}
