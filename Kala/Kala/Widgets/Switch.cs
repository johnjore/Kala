using System;
using System.Collections.Generic;
using Xamarin.Forms;
using DrawShape;
using Newtonsoft.Json.Linq;
using Plugin.Logger;
using CircularProgressBar.FormsPlugin.Abstractions;

namespace Kala
{
    public partial class Widgets
    {
        public static void Switch(Grid grid, string x1, string y1, string header, JObject data)
        {
            int px = 0;
            int py = 0;
            Models.Sitemap.Widget3 item = null;
            Dictionary<string, string> widgetKeyValuePairs = null;

            //If this fails, we dont know where to show an error
            try
            {
                item = data.ToObject<Models.Sitemap.Widget3>();
                widgetKeyValuePairs = Helpers.SplitCommand(item.label);
                CrossLogger.Current.Debug("Switch", "Label: " + widgetKeyValuePairs["label"]);

                px = Convert.ToInt16(x1);
                py = Convert.ToInt16(y1);
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Switch", "Crashed: " + ex.ToString());
            }

            try
            {
                //Switch (on/off) button
                Button switchButton = new Button
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Color.Transparent,
                    StyleId = item.item.name //StyleID is not used on buttons
                };
                switchButton.Clicked += OnSwitchButtonClicked;
                CrossLogger.Current.Debug("Switch", "Button ID: " + switchButton.Id + " created.");
                grid.Children.Add(switchButton, px, py);

                App.trackItem i = new App.trackItem
                {
                    grid = grid,
                    px = px,
                    py = py,
                    name = item.item.name,
                    header = header,
                    icon = widgetKeyValuePairs["icon"],
                    state = item.item.state,
                    type = Models.Itemtypes.Switch,
                };
                App.config.items.Add(i);

                Switch_update(true, i);
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Switch", "Widgets.Switch crashed: " + ex.ToString());
                Error(grid, px, py, 1, 1, ex.ToString());
            }
        }

        public static void Switch_update(bool Create, App.trackItem item)
        {
            string status = "N/A";

            //Header (Also clears the old status)
            item.grid.Children.Add(new Label
            {
                Text = item.header,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = App.config.TextColor,
                BackgroundColor = App.config.CellColor,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Start
            }, item.px, item.py);

            if (!item.state.Equals("Uninitialized"))
            {
                try
                {
                    if (int.TryParse(item.state, out int stat))
                    {
                        if (stat > 0)
                        {
                            Switch_On(item);
                            status = "ON";
                        }
                        else
                        {
                            Switch_Off(item);
                            status = "OFF";
                        }
                    }
                    else
                    {
                        if (item.state.ToUpper().Equals("OFF"))
                        {
                            Switch_Off(item);
                            status = "OFF";
                        }
                        else
                        {
                            Switch_On(item);
                            status = "ON";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Error(item.grid, item.px, item.py, item.sx, item.sy, ex.ToString());
                }
            }

            //Image
            item.grid.Children.Add(new Image
            {
                Source = item.icon,
                Aspect = Aspect.AspectFill,
                BackgroundColor = Color.Transparent,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            }, item.px, item.px + 1, item.py, item.py + 1);

            //Status
            ItemLabel l_status = new ItemLabel
            {
                Text = status.ToUpper(),
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = App.config.TextColor,
                BackgroundColor = App.config.CellColor,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                TranslationY = -10,
                Name = item.name
            };
            item.grid.Children.Add(l_status, item.px, item.py);
        }

        public static void Switch_On(App.trackItem item)
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

        public static void Switch_Off(App.trackItem item)
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Await.Warning", "CS4014:Await.Warning")]
        public static void OnSwitchButtonClicked(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string name = button.StyleId;

            foreach (App.trackItem item in App.config.items)
            {
                if (item.name.Equals(name))
                {
                    if (!item.state.ToLower().Equals("uninitialized"))
                    {
                        if (int.TryParse(item.state, out int stat))
                        {
                            item.state = (stat > 0) ? "OFF" : "ON";
                        }
                        else
                        {
                            item.state = (item.state.ToUpper().Equals("ON")) ? "OFF" : "ON";
                        }
                    }
                    else
                    {
                        item.state = "ON";
                    }

                    CrossLogger.Current.Debug("Switch", "Button ID: '" + button.Id.ToString() + "', URL: '" + button.StyleId + "', New State: '" + item.state + "'");
                    Switch_update(false, item);
                    #pragma warning disable CS4014
                    new RestService().SendCommand(name, item.state);
                    #pragma warning restore CS4014
                }
            }
        }
    }
}
