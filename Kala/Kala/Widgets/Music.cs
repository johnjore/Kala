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
        public static void Music(Grid grid, string x1, string y1, string x2, string y2, string header, JObject data)
        {
            CrossLogger.Current.Debug("Music", "Creating Music Widget");

            int.TryParse(x1, out int px);
            int.TryParse(y1, out int py);
            int.TryParse(x2, out int sx);
            int.TryParse(y2, out int sy);

            Models.Sitemap.Widget3 item = null;
            Dictionary<string, string> widgetKeyValuePairs = null;

            try
            {
                item = data.ToObject<Models.Sitemap.Widget3>();
                widgetKeyValuePairs = Helpers.SplitCommand(item.Label);

                App.TrackItem i = new App.TrackItem
                {
                    Grid = grid,
                    Name = item.Item.Name,
                    Header = header,
                    Icon = widgetKeyValuePairs["icon"],
                    State = item.Item.State,
                    Type = Itemtypes.Sensor
                };
                App.Config.Items.Add(i);

                Music_update(true, i);
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Music", "Sensor crashed: " + ex.ToString());
                Error(grid, px, py, sx, sy, ex.ToString());
            }
        }

        private static void Music_update(bool Create, App.TrackItem item)
        {
            #region Header (Also clears the old status)
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
                        Music_Off(item);
                    }
                    else
                    {
                        Music_On(item);
                    }
                }
                catch (Exception ex)
                {
                    Error(item.Grid,0, 0, 1, 1, ex.ToString());
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
            }, 0, 0, 1, 1);
            #endregion Image

            #region Status Text
            ItemLabel l_status = new ItemLabel
            {
                Text = item.State.ToUpper(),
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = App.Config.TextColor,
                BackgroundColor = App.Config.CellColor,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                TranslationY = -10,
                Name = item.Name
            };
            item.Grid.Children.Add(l_status, 0, 0);
            #endregion Status Text
        }

        private static void Music_On(App.TrackItem item)
        {
            item.Grid.Children.Add(new ShapeView()
            {
                ShapeType = ShapeType.Circle,
                StrokeColor = App.Config.ValueColor,
                Color = App.Config.ValueColor,
                StrokeWidth = 10.0f,
                Scale = 2,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            }, 0, 0);
        }

        private static void Music_Off(App.TrackItem item)
        {
            int intStrokeThickness = 1;
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    intStrokeThickness = 2;
                    break;
                case Device.Android:
                    intStrokeThickness = 4;
                    break;
            }

            item.Grid.Children.Add(new CircularProgressBarView
            {
                Progress = 100,
                StrokeThickness = intStrokeThickness,
                BackgroundColor = Color.Transparent,
                ProgressBackgroundColor = App.Config.BackGroundColor,
                ProgressColor = App.Config.ValueColor,
                Scale = 0.5f
            }, 0, 0);
        }
    }
}
