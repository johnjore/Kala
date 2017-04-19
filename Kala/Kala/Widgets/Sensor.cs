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

            int px = 0;
            int py = 0;
            int sx = 0;
            int sy = 0;

            //If this fails, we don't know where to show the error
            try
            {
                px = Convert.ToInt16(x1);
                py = Convert.ToInt16(y1);
                sx = Convert.ToInt16(x2);
                sy = Convert.ToInt16(y2);
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Sensor", "Crashed: " + ex.ToString());
            }

            Models.Sitemap.Widget3 item = null;
            Dictionary<string, string> widgetKeyValuePairs = null;

            try
            {
                item = data.ToObject<Models.Sitemap.Widget3>();
                widgetKeyValuePairs = Helpers.SplitCommand(item.label);

                App.trackItem i = new App.trackItem
                {
                    grid = grid,
                    px = px,
                    py = py,
                    sx = sx,
                    sy = sy,
                    link = item.item.link,
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
                Error(grid, px, py, ex.ToString());
            }
        }

        public static void Sensor_update(bool Create, App.trackItem item)
        {
            #region Header (Also clears the old status)
            item.grid.Children.Add(new Label
            {
                Text = item.header,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = App.config.TextColor,
                BackgroundColor = App.config.CellColor,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Start
            }, item.px, item.py);
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
                    Error(item.grid, item.px, item.py, ex.ToString());
                }
            }
            else
            {
                item.state = "N/A";
            }
            #endregion State

            #region Image
            string strSource = item.icon;
            switch (Device.RuntimePlatform)
            {
                case Device.WinPhone:
                    strSource = "Assets/" + item.icon;
                    break;
            }

            item.grid.Children.Add(new Image
            {
                Source = strSource,
                Aspect = Aspect.AspectFill,
                BackgroundColor = Color.Transparent,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            }, item.px, item.px + 1, item.py, item.py + 1);
            #endregion Image

            #region Status Text
            ItemLabel l_status = new ItemLabel
            {
                Text = item.state.ToUpper(),
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = App.config.TextColor,
                BackgroundColor = App.config.CellColor,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                TranslationY = -10,
                Link = item.link
            };
            item.grid.Children.Add(l_status, item.px, item.py);
            #endregion Status Text
        }

        public static void Sensor_On(App.trackItem item)
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
            }, item.px, item.py);
        }

        public static void Sensor_Off(App.trackItem item)
        {
            int intStrokeThickness = 2;
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    intStrokeThickness = 2;
                    break;
                case Device.Android:
                    intStrokeThickness = 4;
                    break;
                case Device.WinPhone:
                    intStrokeThickness = 16;
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
            }, item.px, item.py);            
        }
    }
}
