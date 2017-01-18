using System;
using System.Diagnostics;
using System.Collections.Generic;
using Xamarin.Forms;
using DrawShape;
using Newtonsoft.Json.Linq;

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
                Debug.WriteLine("Label: " + widgetKeyValuePairs["label"]);

                px = Convert.ToInt16(x1);
                py = Convert.ToInt16(y1);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Widgets.Switch crashed: " + ex.ToString());
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
                Debug.WriteLine("Button ID: " + switchButton.Id + " created.");
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
                Debug.WriteLine("Widgets.Switch crashed: " + ex.ToString());
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
                    int stat = Convert.ToInt16(s_state);
                    if (stat > 0)
                    {
                        status = "ON";
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
                    }
                    else
                    {
                        status = "OFF";
                        ProgressCircle(grid, px, py, 100, 0.5f);
                    }
                }
                catch (Exception ex)
                {
                    Error(grid, px, py, ex.ToString());
                }
            }

            //Image
            AddImageCenter(grid, px, py, icon, Color.Transparent);

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Await.Warning", "CS4014:Await.Warning")]
        public static void OnSwitchButtonClicked(object sender, EventArgs e)
        {
            Button button = sender as Button;

            string link = button.StyleId;
            string state = "100";

            foreach (App.trackItem item in App.config.items)
            {
                if (item.link.Equals(link))
                {
                    state = item.state;
                    Debug.WriteLine("Found: " + item.link + ", New State: " + state);
                 }
            }

            Debug.WriteLine("Button ID: '" + button.Id.ToString() + "', URL: '" + button.StyleId + "', State: '" + state + "'");


            if (!state.Equals("Uninitialized"))
            {
                try
                {

                    if (Convert.ToInt16(state) > 0)
                    {
                        state = "0";
                    }
                    else
                    {
                        state = "100";
                    }
                }
                catch
                {
                }
            }
            else
            {
                state = "100";
            }

            new RestService().SendCommand(link, state);
        }
    }
}

