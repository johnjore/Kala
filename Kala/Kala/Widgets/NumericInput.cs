using System;
using System.Collections.Generic;
using Xamarin.Forms;
using DrawShape;
using Newtonsoft.Json.Linq;
using Plugin.Logger;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;

namespace Kala
{
    public partial class Widgets : ContentPage
    {
        public static void NumericInput(Grid grid, int px, int py, int sx, int sy, string header, JObject data)
        {
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Create NumericInput Widget");

            try
            {
                Models.Sitemap.Widget3 item = data.ToObject<Models.Sitemap.Widget3>();
                Dictionary<string, string> widgetKeyValuePairs = Helpers.SplitCommand(item.Label);
                CrossLogger.Current.Debug("Input", "Label: " + widgetKeyValuePairs["label"]);

                //Master Grid for Widget
                Grid Widget_Grid = new Grid
                {
                    RowDefinitions = new RowDefinitionCollection {
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    },
                    ColumnDefinitions = new ColumnDefinitionCollection {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    },
                    RowSpacing = 0,
                    ColumnSpacing = 0,
                    BackgroundColor = App.Config.CellColor,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                };
                grid.Children.Add(Widget_Grid, px, px + sx, py, py + sy);

                //Header
                Widget_Grid.Children.Add(new Label
                {
                    Text = header,
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                    TextColor = App.Config.TextColor,
                    BackgroundColor = App.Config.CellColor,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Start
                }, 0, 0);

                //Background
                Widget_Grid.Children.Add(new ShapeView()
                {
                    ShapeType = ShapeType.Circle,
                    StrokeColor = App.Config.ValueColor,
                    Color = App.Config.ValueColor,
                    StrokeWidth = 1.0f,
                    Scale = 2.5,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }, 0, 1);

                //Image
                Widget_Grid.Children.Add(new Image
                {
                    Source = widgetKeyValuePairs["icon"],
                    Aspect = Aspect.AspectFill,
                    BackgroundColor = Color.Transparent,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                }, 0, 1);

                //Button must be added last
                Button inputButton = new Button
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Color.Transparent,
                    StyleId = item.Item.Name //StyleID not used on buttons
                };
                Widget_Grid.Children.Add(inputButton, 0, 1, 0, 2);
                inputButton.Clicked += OnInputButtonClicked;

                CrossLogger.Current.Debug("Input", "Button ID: " + inputButton.Id + " created.");
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Input", "Widgets.Input crashed: " + ex.ToString());
                Error(grid, px, py, 1, 1, ex.ToString());
            }
        }

        private static async void OnInputButtonClicked(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string name = button.StyleId;

            Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Debug("Input", "Button ID: '" + button.Id.ToString() + ", Name: '" + name + "'"));
            await PopupNavigation.Instance.PushAsync(new Pages.NumericInputPage(name));
        }
    }
}
