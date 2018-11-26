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
    public partial class Widgets : ContentPage
    {
        public static void Sensor(Grid grid, int px, int py, int sx, int sy, string header, JObject data)
        {
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Create Sensor Widget");
            CrossLogger.Current.Debug("Sensor", "Creating Sensor Widget");

            Models.Sitemap.Widget3 item = null;
            Dictionary<string, string> widgetKeyValuePairs = null;

            //Master Grid for Widget
            Grid Widget_Grid = new Grid
            {   
                RowDefinitions = new RowDefinitionCollection {
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                },
                ColumnDefinitions = new ColumnDefinitionCollection {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                },
                RowSpacing = 0,
                ColumnSpacing = 0,
                BackgroundColor = App.Config.CellColor,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };
            grid.Children.Add(Widget_Grid, px, px + sx, py, py + sy);

            try
            {
                item = data.ToObject<Models.Sitemap.Widget3>();
                widgetKeyValuePairs = Helpers.SplitCommand(item.Label);

                App.TrackItem i = new App.TrackItem
                {
                    Grid = Widget_Grid,
                    Name = item.Item.Name,
                    Header = header,
                    Icon = widgetKeyValuePairs["icon"],
                    State = item.Item.State,
                    Type = Itemtypes.Sensor
                };
                App.Config.Items.Add(i);

                Sensor_update(true, i);
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Sensor", "Sensor crashed: " + ex.ToString());
                Error(grid, px, py, sx, sy, ex.ToString());
            }
        }

        public static void Sensor_update(bool Create, App.TrackItem item)
        {
            Floorplan_update(item);

            if (item.Grid == null) return;

            item.Grid.Children.Clear();

            #region Header
            item.Grid.Children.Add(new Label
            {
                Text = item.Header,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = App.Config.TextColor,
                BackgroundColor = App.Config.CellColor,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Start
            }, 0, 0);
            #endregion Header 

            #region State
            if (!item.State.ToUpper().Equals("UNINITIALIZED"))
            {
                try
                {
                    if (item.State.ToUpper().Equals("CLOSED"))
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
                    Error(item.Grid, 0, 0, 1, 1, ex.ToString());
                }
            }
            else
            {
                item.State = "N/A";
            }
            #endregion State

            #region Image
            item.Grid.Children.Add(new Xamarin.Forms.Image
            {
                Source = item.Icon,
                Aspect = Aspect.AspectFill,
                BackgroundColor = Color.Transparent,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            }, 0, 1);
            #endregion Image

            #region Status Text
            item.Grid.Children.Add (new ItemLabel
            {
                Text = item.State.ToUpper(),
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = App.Config.TextColor,
                BackgroundColor = App.Config.CellColor,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                TranslationY = -10,
                Name = item.Name
            }, 0, 2);
            #endregion Status Text

            //Button must be last to be added to work
            Button dummyButton = new Button
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent,
            };
            item.Grid.Children.Add(dummyButton, 0, 1, 0, 2);
            dummyButton.Clicked += OnDummyButtonClicked;
        }

        private static void Sensor_On(App.TrackItem item)
        {
            item.Grid.Children.Add(new ShapeView()
            {
                ShapeType = ShapeType.Circle,
                StrokeColor = App.Config.ValueColor,
                Color = App.Config.ValueColor,
                StrokeWidth = 1.0f,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Scale = 2.5f,
            }, 0, 1);
        }

        private static void Sensor_Off(App.TrackItem item)
        {
            item.Grid.Children.Add(new CircularProgressBarView
            {
                Progress = 100,
                StrokeThickness = 1,
                BackgroundColor = Color.Transparent,
                ProgressBackgroundColor = App.Config.BackGroundColor,
                ProgressColor = App.Config.ValueColor,
                Scale = 1.5f,
            }, 0, 1);
        }
    }
}
