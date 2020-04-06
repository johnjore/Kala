using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using Plugin.Logger;
using FFImageLoading.Svg.Forms;

namespace Kala
{
    public partial class Widgets : ContentPage
    {
        public static void Launcher(Grid grid, int px, int py, int sx, int sy, string header, JObject data)
        {
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Create Launcher Widget");

            try
            {
                Models.Sitemap.Widget3 item = data.ToObject<Models.Sitemap.Widget3>();
                Dictionary<string, string> widgetKeyValuePairs = Helpers.SplitCommand(item.Label);
                CrossLogger.Current.Debug("Launcher", "Label: " + widgetKeyValuePairs["label"]);

                //Master Grid for Widget
                Grid Widget_Grid = new Grid
                {
                    RowDefinitions = new RowDefinitionCollection {
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                        new RowDefinition { Height = new GridLength(2, GridUnitType.Star) },
                    },
                    ColumnDefinitions = new ColumnDefinitionCollection {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    },
                    RowSpacing = 0,
                    ColumnSpacing = 0,
                    BackgroundColor = App.Config.CellColor,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    Padding = new Thickness(0, 0, 0, 10),
                };

                //Header
                Widget_Grid.Children.Add(new Forms9Patch.Label
                {
                    Text = header.Replace("\"", " "),
                    FontSize = 100,
                    TextColor = App.Config.TextColor,
                    BackgroundColor = App.Config.CellColor,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Start,
                    LineBreakMode = LineBreakMode.NoWrap,
                    Lines = 1,
                    AutoFit = Forms9Patch.AutoFit.Width,
                }, 0, 0);

                //Circle
                SvgCachedImage svg = new SvgCachedImage
                {
                    DownsampleToViewSize = false,
                    Aspect = Aspect.AspectFit,
                    BitmapOptimizations = false,
                    Source = SvgImageSource.FromSvgString(@"<svg viewBox=""0 0 100 100""><circle cx=""50"" cy=""50"" r=""50"" fill=""" + App.Config.ValueColor.ToHex().ToString() + @""" /></svg>"),
                };
                Widget_Grid.Children.Add(svg, 0, 1);

                //Image
                Widget_Grid.Children.Add(new Image
                {
                    Source = widgetKeyValuePairs["icon"],
                    Aspect = Aspect.AspectFit,
                    BackgroundColor = Color.Transparent,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                }, 0, 1);

                //Button must be last to be added to work
                Button launcherButton = new Button
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Color.Transparent,
                    StyleId = widgetKeyValuePairs["url"] //StyleID is not used on buttons                    
                };
                Widget_Grid.Children.Add(launcherButton, 0, 1, 0, 2);

                launcherButton.Clicked += OnLauncherButtonClicked;
                CrossLogger.Current.Debug("Launcher", "Button ID: " + launcherButton.Id + " created.");

                grid.Children.Add(Widget_Grid, px, px + sx, py, py + sy);
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Launcher", "Widgets.Launcher crashed: " + ex.ToString());
                Error(grid, px, py, sx, sy, ex.ToString());
            }
        }

        private static void OnLauncherButtonClicked(object sender, EventArgs e)
        {
            App.Config.LastActivity = DateTime.Now;

            Button button = sender as Button;
            string name = button.StyleId;

            Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Debug("Launcher", "Button ID: '" + button.Id.ToString() + "', URL: '" + name + "'"));

            IAppLauncher appLauncher = DependencyService.Get<IAppLauncher>();
            appLauncher.Launch(name);
        }
    }
}
