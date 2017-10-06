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

                //Master Grid for Widget
                Grid Widget_Grid = new Grid
                {
                    RowDefinitions = new RowDefinitionCollection {
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) }
                    },
                    ColumnDefinitions = new ColumnDefinitionCollection {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    },
                    RowSpacing = 6,
                    ColumnSpacing = 6,
                    BackgroundColor = App.config.CellColor,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand
                };
                grid.Children.Add(Widget_Grid, px, py);

                App.trackItem i = new App.trackItem
                {
                    grid = Widget_Grid,
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
            item.grid.Children.Clear();

            //Header
            item.grid.Children.Add(new Label
            {
                Text = item.header,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = App.config.TextColor,
                BackgroundColor = App.config.CellColor,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Start,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Center
            }, 0, 2, 0, 1);

            //Center image
            item.grid.Children.Add(new Image
            {
                Source = item.icon,
                Aspect = Aspect.AspectFill,
                BackgroundColor = App.config.CellColor,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            }, 0, 2, 1, 2);

            //Progress bar around image
            try
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
                    Progress = Convert.ToInt16(item.state),
                    StrokeThickness = intStrokeThickness,
                    BackgroundColor = Color.Transparent,
                    ProgressBackgroundColor = App.config.BackGroundColor,
                    ProgressColor = App.config.ValueColor,
                    Scale = 0.8f
                }, 0, 2, 1, 2);
            }
            catch { }

            //Number value of progress bar
            switch (item.state.ToUpper())
            {
                case "ON": item.state = "100";
                    break;
                case "OFF": item.state = "0";
                    break;
            }
            item.grid.Children.Add(new ItemLabel
            {
                Text = item.state,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = App.config.TextColor,
                BackgroundColor = App.config.CellColor,
                HorizontalTextAlignment = TextAlignment.End,
                VerticalTextAlignment = TextAlignment.Start,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.End,
                TranslationY = -10,
                Name = item.name
            }, 0, 2);
            
            //Unit of progress bar
            item.grid.Children.Add(new ItemLabel
            {
                Text = item.unit,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = App.config.TextColor,
                BackgroundColor = App.config.CellColor,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.End,
                TranslationY = -10,
                Name = item.name
            }, 1, 2);

            Button dimmerButton = new Button
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent,
                StyleId = item.name //StyleID is not used on buttons
            };
            dimmerButton.Clicked += OnDimmerButtonClicked;
            CrossLogger.Current.Debug("Dimmer", "Button ID: " + dimmerButton.Id + " created.");
            item.grid.Children.Add(dimmerButton, 0, 2, 0, 3);
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
