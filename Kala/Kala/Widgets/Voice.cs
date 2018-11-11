//https://github.com/xamarin/monodroid-samples/blob/master/PlatformFeatures/SpeechToText/SpeechToText/MainActivity.cs
//https://github.com/ihassantariq/VoiceRecognitionSystem
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using DrawShape;
using Newtonsoft.Json.Linq;
using Plugin.Logger;
using System.Threading.Tasks;

namespace Kala
{
    public partial class Widgets : ContentPage
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Await.Warning", "CS4014:Await.Warning")]
        public static void Voice(Grid grid, string x1, string y1, string x2, string y2, string header, JObject data)
        {
            HockeyApp.MetricsManager.TrackEvent("Create Voice Widget");

            int.TryParse(x1, out int px);
            int.TryParse(y1, out int py);
            int.TryParse(x2, out int sx);
            int.TryParse(y2, out int sy);

            try
            {
                Models.Sitemap.Widget3 item = data.ToObject<Models.Sitemap.Widget3>();
                Dictionary<string, string> widgetKeyValuePairs = Helpers.SplitCommand(item.Label);
                CrossLogger.Current.Debug("Voice", "Label: " + widgetKeyValuePairs["label"]);

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
                    HorizontalTextAlignment = Xamarin.Forms.TextAlignment.Center,
                    VerticalTextAlignment = Xamarin.Forms.TextAlignment.Start
                }, 0, 0);

                //Background
                Widget_Grid.Children.Add(new ShapeView()
                {
                    ShapeType = ShapeType.Circle,
                    StrokeColor = App.Config.ValueColor,
                    Color = App.Config.ValueColor,
                    StrokeWidth = 10.0f,
                    Scale = 2,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }, 0, 0);

                //Image
                Widget_Grid.Children.Add(new Image
                {
                    Source = widgetKeyValuePairs["icon"],
                    Aspect = Aspect.AspectFill,
                    BackgroundColor = Color.Transparent,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                }, 0, 0);

                //Button must be added last
                var voiceButton = new VoiceButton
                {                    
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    StyleId = item.Item.Name //StyleID not used on buttons
                };
                Widget_Grid.Children.Add(voiceButton, 0, 0);

                voiceButton.OnTextChanged += (s) =>
                {
                    CrossLogger.Current.Debug("Voice", "Text: " + s);
                    Task.Run(async () =>
                    {
                        await new RestService().SendCommand(voiceButton.StyleId, s);
                    });                    
                };
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Voice", "Widgets.Voice crashed: " + ex.ToString());
                Error(grid, px, py, 1, 1, ex.ToString());
            }
        }
    }
}
