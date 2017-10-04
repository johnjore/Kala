using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using Plugin.Logger;
using CircularProgressBar.FormsPlugin.Abstractions;

namespace Kala
{
    public partial class Widgets
    {
        public static void Dimmer(Grid grid, string x1, string y1, string header, JObject data)
        {
            int.TryParse(x1, out int px);
            int.TryParse(y1, out int py);

            Models.Sitemap.Widget3 item = null;
            Dictionary<string, string> widgetKeyValuePairs = null;
            
            try
            {
                item = data.ToObject<Models.Sitemap.Widget3>();
                widgetKeyValuePairs = Helpers.SplitCommand(item.label);
                CrossLogger.Current.Debug("Dimmer", "Label: " + widgetKeyValuePairs["label"]);

                //Slider button
                Button dimmerButton = new Button
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Color.Transparent,
                    StyleId = item.item.name //StyleID is not used on buttons
                };
                dimmerButton.Clicked += OnDimmerButtonClicked;
                CrossLogger.Current.Debug("Dimmer", "Button ID: " + dimmerButton.Id + " created.");
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
                    name = item.item.name,
                    type = Models.Itemtypes.Dimmer
                };
                App.config.items.Add(i);

                Dimmer_update(true, i);
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Dimmer", "Widgets.Dimmer crashed: " + ex.ToString());
                Error(grid, px, py, 1, 1, ex.ToString());
            }
        }

        public static void Dimmer_update(bool Create, App.trackItem item)
        {
            item.grid.Children.Add(new Label
            {
                Text = item.header,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = App.config.TextColor,
                BackgroundColor = App.config.CellColor,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Start
            }, item.px, item.py);

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
                BackgroundColor = App.config.CellColor,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            }, item.px, item.px + 1, item.py, item.py + 1);

            switch (item.state.ToUpper())
            {
                case "ON":  item.state = "100";
                    break;
                case "OFF": item.state = "0";
                    break;
            }

            ItemLabel l_status = new ItemLabel
            {
                Text = item.state,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = App.config.TextColor,
                BackgroundColor = App.config.CellColor,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.End,
                TranslationY = -10,
                Name = item.name
            };

            ItemLabel l_unit = new ItemLabel
            {
                Text = item.unit,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = App.config.TextColor,
                BackgroundColor = App.config.CellColor,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.End,
                TranslationY = -10,
                Name = item.name
            };

            //Grid for status text at the bottom
            Grid g = new Grid
            {
                RowDefinitions = new RowDefinitionCollection {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                },
                ColumnDefinitions = new ColumnDefinitionCollection {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                },
                RowSpacing = 6,
                ColumnSpacing = 6,
                BackgroundColor = App.config.CellColor,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,                
            };
            g.Children.Add(l_status, 0, 0);
            g.Children.Add(l_unit, 1, 0);
            item.grid.Children.Add(g, item.px, item.py);
       
            //Capturs non-initialized item
            try
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
                    Progress = Convert.ToInt16(item.state),
                    StrokeThickness = intStrokeThickness,
                    BackgroundColor = Color.Transparent,
                    ProgressBackgroundColor = App.config.BackGroundColor,
                    ProgressColor = App.config.ValueColor,
                    Scale = 0.5f
                }, item.px, item.py);
            }
            catch { }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Await.Warning", "CS4014:Await.Warning")]
        public static void OnDimmerButtonClicked(object sender, EventArgs e)
        {
            Button button = sender as Button;
            App.trackItem item = null;
            string name = button.StyleId;

            //Find current item
            foreach (App.trackItem i in App.config.items)
            {
                if (i.name.Equals(name))
                {
                    item = i;
                    break;
                }
            }

            CrossLogger.Current.Debug("Dimmer", "Button ID: '" + button.Id.ToString() + "', URL: '" + button.StyleId + "', State: '" + item.state + "'");

            PreviousPage = Application.Current.MainPage;
            Application.Current.MainPage = CreateSliderPage(item);
        }
    }
}
