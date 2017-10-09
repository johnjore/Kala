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
            int.TryParse(x1, out int px);
            int.TryParse(y1, out int py);
            
            try
            {
                Models.Sitemap.Widget3 item = data.ToObject<Models.Sitemap.Widget3>();
                Dictionary<string, string> widgetKeyValuePairs = Helpers.SplitCommand(item.label);
                CrossLogger.Current.Debug("Switch", "Label: " + widgetKeyValuePairs["label"]);

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
                grid.Children.Add(Widget_Grid, px, py);

                App.trackItem i = new App.trackItem
                {
                    grid = Widget_Grid,
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

            item.grid.Children.Clear();
            
            //Header (Also clears the old status)
            item.grid.Children.Add(new Label
            {
                Text = item.header,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = App.config.TextColor,
                BackgroundColor = App.config.CellColor,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Start
            }, 0, 0);

            if (item.state != null && !item.state.Equals("Uninitialized"))
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
                    Error(item.grid, 0, 0, 1, 1, ex.ToString());
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
            }, 0, 0);

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
            item.grid.Children.Add(l_status, 0, 0);

            //Button must be last to be added to work
            Button switchButton = new Button
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent,
                StyleId = item.name //StyleID is not used on buttons
            };
            item.grid.Children.Add(switchButton, 0, 0);
            switchButton.Clicked += OnSwitchButtonClicked;
            CrossLogger.Current.Debug("Switch", "Button ID: " + switchButton.Id + " created.");
        }

        private static void Switch_On(App.trackItem item)
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

        private static void Switch_Off(App.trackItem item)
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Await.Warning", "CS4014:Await.Warning")]
        private static void OnSwitchButtonClicked(object sender, EventArgs e)
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
