﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace Kala
{
    public partial class Widgets
    {
        public static void Dimmer(Grid grid, string x1, string y1, string header, JObject data)
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
                //Slider button
                Button dimmerButton = new Button
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Color.Transparent,
                    StyleId = item.item.link //StyleID is not used on buttons
                };
                dimmerButton.Clicked += OnDimmerButtonClicked;
                Debug.WriteLine("Button ID: " + dimmerButton.Id + " created.");
                grid.Children.Add(dimmerButton, px, py);

                App.trackItem i = new App.trackItem
                {
                    grid = grid,
                    px = px,
                    py = py,
                    header = header,
                    state = item.item.state,
                    unit = widgetKeyValuePairs["unit"],
                    icon = widgetKeyValuePairs["icon"],
                    link = item.item.link,
                    type = "DimmerItem"
                };
                App.config.items.Add(i);

                Dimmer_update(true, grid, px, py, i.header, i.state, i.unit, i.icon, i.link);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Widgets.Switch crashed: " + ex.ToString());
                Error(grid, px, py, ex.ToString());
            }
        }

        public static void Dimmer_update(bool Create, Grid grid, int px, int py, string header, string s_state, string unit, string icon, string link)
        {
            AddHeaderText(grid, px, py, header);
            AddImageCenter(grid, px, py, icon, App.config.CellColor);
            AddStatusText(grid, px, py, s_state, unit, link);

            //Capturs non-initialized item
            try
            {
                ProgressCircle(grid, px, py, Convert.ToInt16(s_state), 0.5f);
            }
            catch { }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Await.Warning", "CS4014:Await.Warning")]
        public static void OnDimmerButtonClicked(object sender, EventArgs e)
        {
            Button button = sender as Button;
            App.trackItem item = null;
            string link = button.StyleId;

            //Find current item
            foreach (App.trackItem i in App.config.items)
            {
                if (i.link.Equals(link))
                {
                    item = i;
                    break;
                }
            }

            Debug.WriteLine("Button ID: '" + button.Id.ToString() + "', URL: '" + button.StyleId + "', State: '" + item.state + "'");

            PreviousPage = Application.Current.MainPage;
            Application.Current.MainPage = CreateSliderPage(item);
        }
    }
}
