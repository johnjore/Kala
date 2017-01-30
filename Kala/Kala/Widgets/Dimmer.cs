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
            int px = 0;
            int py = 0;
            Models.Sitemap.Widget3 item = null;
            Dictionary<string, string> widgetKeyValuePairs = null;

            //If this fails, we dont know where to show an error
            try
            {
                item = data.ToObject<Models.Sitemap.Widget3>();
                widgetKeyValuePairs = Helpers.SplitCommand(item.label);
                CrossLogger.Current.Debug("Dimmer", "Label: " + widgetKeyValuePairs["label"]);

                px = Convert.ToInt16(x1);
                py = Convert.ToInt16(y1);
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Dimmer", "Widgets.Switch crashed: " + ex.ToString());
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
                    link = item.item.link,
                    type = Models.Itemtypes.Dimmer
                };
                App.config.items.Add(i);

                Dimmer_update(true, i);
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Dimmer", "Widgets.Switch crashed: " + ex.ToString());
                Error(grid, px, py, ex.ToString());
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

            item.grid.Children.Add(new Image
            {
                Source = Device.OnPlatform(item.icon, item.icon, "Assets/" + item.icon),
                Aspect = Aspect.AspectFill,
                BackgroundColor = App.config.CellColor,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            }, item.px, item.px + 1, item.py, item.py + 1);

            ItemLabel l_status = new ItemLabel
            {
                Text = item.state,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = App.config.TextColor,
                BackgroundColor = App.config.CellColor,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                TranslationY = -10,
                Link = item.link
            };

            l_status.HorizontalOptions = LayoutOptions.End;

            ItemLabel l_unit = new ItemLabel
            {
                Text = item.unit,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = App.config.TextColor,
                BackgroundColor = App.config.CellColor,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.End,
                TranslationY = -10,
                Link = item.link
            };

            //Grid for status text at the bottom
            Grid g = new Grid();
            g.RowDefinitions = new RowDefinitionCollection();
            g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            g.ColumnDefinitions = new ColumnDefinitionCollection();
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            g.RowSpacing = 6;
            g.ColumnSpacing = 6;
            g.BackgroundColor = App.config.CellColor;
            g.HorizontalOptions = LayoutOptions.Center;
            g.VerticalOptions = LayoutOptions.End;

            g.Children.Add(l_status, 0, 0);
            g.Children.Add(l_unit, 1, 0);

            item.grid.Children.Add(g, item.px, item.py);
       
            //Capturs non-initialized item
            try
            {
                item.grid.Children.Add(new CircularProgressBarView
                {
                    Progress = Convert.ToInt16(item.state),
                    StrokeThickness = Device.OnPlatform(2, 4, 16),
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

            CrossLogger.Current.Debug("Dimmer", "Button ID: '" + button.Id.ToString() + "', URL: '" + button.StyleId + "', State: '" + item.state + "'");

            PreviousPage = Application.Current.MainPage;
            Application.Current.MainPage = CreateSliderPage(item);
        }
    }
}
