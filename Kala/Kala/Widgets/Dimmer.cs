using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using Plugin.Logger;
using CircularProgressBar.FormsPlugin.Abstractions;

namespace Kala
{
    public partial class Widgets : ContentPage
    {
        public static void Dimmer(Grid grid, int px, int py, string header, JObject data)
        {
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Create Dimmer Widget");

            Models.Sitemap.Widget3 item = null;
            Dictionary<string, string> widgetKeyValuePairs = null;
            
            try
            {
                item = data.ToObject<Models.Sitemap.Widget3>();
                widgetKeyValuePairs = Helpers.SplitCommand(item.Label);
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
                    BackgroundColor = App.Config.CellColor,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand
                };
                grid.Children.Add(Widget_Grid, px, py);

                App.TrackItem i = new App.TrackItem
                {
                    Grid = Widget_Grid,
                    Header = header,
                    State = item.Item.State,
                    Unit = widgetKeyValuePairs["unit"],
                    Icon = widgetKeyValuePairs["icon"],
                    Name = item.Item.Name,
                    Type = Models.Itemtypes.Dimmer
                };
                App.Config.Items.Add(i);

                Dimmer_update(true, i);
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Dimmer", "Widgets.Dimmer crashed: " + ex.ToString());
                Error(grid, px, py, 1, 1, ex.ToString());
            }
        }

        public static void Dimmer_update(bool Create, App.TrackItem item)
        {
            item.Grid.Children.Clear();

            //Header
            item.Grid.Children.Add(new Label
            {
                Text = item.Header,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = App.Config.TextColor,
                BackgroundColor = App.Config.CellColor,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Start,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Center
            }, 0, 2, 0, 1);

            //Center image
            item.Grid.Children.Add(new Image
            {
                Source = item.Icon,
                Aspect = Aspect.AspectFill,
                BackgroundColor = App.Config.CellColor,
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

                item.Grid.Children.Add(new CircularProgressBarView
                {
                    Progress = Convert.ToInt16(item.State),
                    StrokeThickness = intStrokeThickness,
                    BackgroundColor = Color.Transparent,
                    ProgressBackgroundColor = App.Config.BackGroundColor,
                    ProgressColor = App.Config.ValueColor,
                    Scale = 0.8f
                }, 0, 2, 1, 2);
            }
            catch
            {
                    //Nothing to process
            }

            //Number value of progress bar
            switch (item.State.ToUpper())
            {
                case "ON": item.State = "100";
                    break;
                case "OFF": item.State = "0";
                    break;
            }
            item.Grid.Children.Add(new ItemLabel
            {
                Text = item.State,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = App.Config.TextColor,
                BackgroundColor = App.Config.CellColor,
                HorizontalTextAlignment = TextAlignment.End,
                VerticalTextAlignment = TextAlignment.Start,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.End,
                TranslationY = -10,
                Name = item.Name
            }, 0, 2);
            
            //Unit of progress bar
            item.Grid.Children.Add(new ItemLabel
            {
                Text = item.Unit,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = App.Config.TextColor,
                BackgroundColor = App.Config.CellColor,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.End,
                TranslationY = -10,
                Name = item.Name
            }, 1, 2);

            Button dimmerButton = new Button
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent,
                StyleId = item.Name //StyleID is not used on buttons
            };
            dimmerButton.Clicked += OnDimmerButtonClicked;
            Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Debug("Dimmer", "Button ID: " + dimmerButton.Id + " created."));
            item.Grid.Children.Add(dimmerButton, 0, 2, 0, 3);
        }
        
        private static void OnDimmerButtonClicked(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string name = button.StyleId;

            //Find item
            App.TrackItem item = App.Config.Items.Find(i => i.Name == name);
            Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Debug("Dimmer", "Button ID: '" + button.Id.ToString() + "', URL: '" + button.StyleId + "', State: '" + item.State + "'"));

            PreviousPage = Application.Current.MainPage;
            Application.Current.MainPage = CreateSliderPage(item);
        }
    }
}
