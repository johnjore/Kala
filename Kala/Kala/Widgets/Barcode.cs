using System;
using System.Collections.Generic;
using Xamarin.Forms;
using DrawShape;
using Newtonsoft.Json.Linq;
using Plugin.Logger;
using ZXing.Mobile;

namespace Kala
{
    public partial class Widgets : ContentPage
    {
        public static void Barcode(Grid grid, string x1, string y1, string x2, string y2, string header, JObject data)
        {
            int.TryParse(x1, out int px);
            int.TryParse(y1, out int py);
            int.TryParse(x2, out int sx);
            int.TryParse(y2, out int sy);

            try
            {
                Models.Sitemap.Widget3 item = data.ToObject<Models.Sitemap.Widget3>();
                Dictionary<string, string> widgetKeyValuePairs = Helpers.SplitCommand(item.Label);
                CrossLogger.Current.Debug("Barcode", "Label: " + widgetKeyValuePairs["label"]);

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
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Start
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

                //Options
                widgetKeyValuePairs.TryGetValue("UseFrontCameraIfAvailable", out string strUseFrontCameraIfAvailable);
                if (strUseFrontCameraIfAvailable == null)
                {
                    strUseFrontCameraIfAvailable = "false";
                }

                var mobileBarcodeScanningOptions = new MobileBarcodeScanningOptions
                {
                    AutoRotate = false,
                    TryHarder = false,
                    TryInverted = false,
                    DisableAutofocus = false,
                    UseNativeScanning = true,
                    UseFrontCameraIfAvailable = bool.Parse(strUseFrontCameraIfAvailable),

                    PossibleFormats = new List<ZXing.BarcodeFormat> {
                        ZXing.BarcodeFormat.EAN_13,
                        ZXing.BarcodeFormat.EAN_8
                    },

                    CameraResolutionSelector = availableResolutions =>
                    {
                        string[] resolution = new string[0];
                        if (widgetKeyValuePairs.ContainsKey("CameraResolutionSelector")) {
                            resolution = widgetKeyValuePairs["CameraResolutionSelector"].Split('x');
                        }
                        foreach (var ar in availableResolutions)
                        {
                            CrossLogger.Current.Debug("Barcode", "Resolution: " + ar.Width + "x" + ar.Height);
                            if (resolution.Length > 0 && (resolution[0] == ar.Width.ToString() && resolution[1] == ar.Height.ToString()))
                            {
                                CrossLogger.Current.Debug("Barcode", "Found match: " + ar.Width + "x" + ar.Height);
                                return ar;
                            }
                        }
                        CrossLogger.Current.Debug("Barcode", "Using default resolution");
                        return null;
                    }
                };

                //Button must be added last
                Button barcodeButton = new BarcodeButton
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Color.Transparent,
                    Name = item.Item.Name,
                    Options = mobileBarcodeScanningOptions,
                };
                barcodeButton.Clicked += OnBarcodeButtonClicked;

                Widget_Grid.Children.Add(barcodeButton, 0, 0);
                CrossLogger.Current.Debug("Barcode", "Button ID: " + barcodeButton.Id + " created.");
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Barcode", "Widgets.Barcode crashed: " + ex.ToString());
                Error(grid, px, py, 1, 1, ex.ToString());
            }
        }

        private static void OnBarcodeButtonClicked(object sender, EventArgs e)
        {
            App.Config.LastActivity = DateTime.Now;

            BarcodeButton barcodeButton = sender as BarcodeButton;
            CrossLogger.Current.Debug("Barcode", "Barcode Item: " + barcodeButton.Name);

            PreviousPage = Application.Current.MainPage;
            Application.Current.MainPage = new NavigationPage(new Pages.BarcodePage(barcodeButton.Name, barcodeButton.Options));
        }
    }
}
