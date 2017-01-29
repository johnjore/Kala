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
                    StyleId = item.item.link //StyleID is not used on buttons
                };
                switchButton.Clicked += OnSwitchButtonClicked;
                CrossLogger.Current.Debug("Switch", "Button ID: " + switchButton.Id + " created.");
                grid.Children.Add(switchButton, px, py);

                App.trackItem i = new App.trackItem
                {
                    grid = grid,
                    px = px,
                    py = py,
                    link = item.item.link,
                    header = header,
                    icon = widgetKeyValuePairs["icon"],
                    state = item.item.state,
                    type = "SwitchItem"
                };
                App.config.items.Add(i);

                Switch_update(true, grid, px, py, i.header, i.state, i.icon, i.link);
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Switch", "Widgets.Switch crashed: " + ex.ToString());
                Error(grid, px, py, ex.ToString());
            }
        }

        public static void Switch_update(bool Create, Grid grid, int px, int py, string header, string s_state, string icon, string link)
        {
            string status = "N/A";

            //Header (Also clears the old status)
            AddHeaderText(grid, px, py, header);

            if (!s_state.Equals("Uninitialized"))
            {
                try
                {
                    int stat;
                    if (int.TryParse(s_state, out stat))
                    {
                        if (stat > 0)
                        {
                            status = Switch_On(grid, px, py);
                        }
                        else
                        {
                            status = Switch_Off(grid, px, py);
                        }
                    }
                    else
                    {
                        if (s_state.ToUpper().Equals("OFF"))
                        {
                            status = Switch_Off(grid, px, py);
                        }
                        else
                        {
                            status = Switch_On(grid, px, py);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Error(grid, px, py, ex.ToString());
                }
            }

            //Image
            grid.Children.Add(new Image
            {
                Source = Device.OnPlatform(icon, icon, "Assets/" + icon),
                Aspect = Aspect.AspectFill,
                BackgroundColor = Color.Transparent,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            }, px, px + 1, py, py + 1);

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
                Link = link
            };
            grid.Children.Add(l_status, px, py);
        }

        public static string Switch_On(Grid grid, int px, int py)
        {
            grid.Children.Add(new ShapeView()
            {
                ShapeType = ShapeType.Circle,
                StrokeColor = App.config.ValueColor,
                Color = App.config.ValueColor,
                StrokeWidth = 10.0f,
                Scale = 2,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            }, px, py);
            return "ON";
        }

        public static string Switch_Off(Grid grid, int px, int py)
        {
            grid.Children.Add(new CircularProgressBarView
            {
                Progress = 100,
                StrokeThickness = Device.OnPlatform(2, 4, 16),
                BackgroundColor = Color.Transparent,
                ProgressBackgroundColor = App.config.BackGroundColor,
                ProgressColor = App.config.ValueColor,
                Scale = 0.5f
            }, px, py);

            return "OFF";
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Await.Warning", "CS4014:Await.Warning")]
        public static void OnSwitchButtonClicked(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string link = button.StyleId;

            foreach (App.trackItem item in App.config.items)
            {
                if (item.link.Equals(link))
                {
                    if (!item.state.ToLower().Equals("uninitialized"))
                    {
                        int stat;
                        if (int.TryParse(item.state, out stat))
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
                    Switch_update(false, item.grid, item.px, item.py, item.header, item.state, item.icon, item.link);
                    new RestService().SendCommand(link, item.state);
                 }
            }
        }
    }
}
