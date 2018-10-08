using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using FFImageLoading.Svg.Forms;
using Newtonsoft.Json.Linq;
using Plugin.Logger;
using Kala.Models;

namespace Kala
{
    public partial class Widgets
    {
        public static async void FloormapAsync(Grid grid, string x1, string y1, string x2, string y2, string header, JObject data)
        {
            int.TryParse(x1, out int px);
            int.TryParse(y1, out int py);
            int.TryParse(x2, out int sx);
            int.TryParse(y2, out int sy);

            try
            {
                Floorplan flooritem = data.ToObject<Floorplan>();
                CrossLogger.Current.Debug("Image", "URL: " + flooritem.Url);

                var httpClient = new HttpClient();
                var svgString = await httpClient.GetStringAsync(flooritem.Url);

                #region w_grid
                Grid w_grid = new Grid
                {
                    RowSpacing = 0,
                    ColumnSpacing = 0,
                    Padding = new Thickness(0, 0, 0, 0),
                    BackgroundColor = App.config.CellColor,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                };
                grid.Children.Add(w_grid, px, px + sx, py, py + sy);
                #endregion w_grid

                #region Header
                w_grid.Children.Add(new Label
                {
                    Text = header,
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                    TextColor = App.config.TextColor,
                    BackgroundColor = App.config.CellColor,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Start
                }, 0, 0);
                #endregion Header

                SvgCachedImage svg = new SvgCachedImage
                {
                    DownsampleToViewSize = false,
                    CacheDuration = TimeSpan.FromMilliseconds(1000),
                    Aspect = Aspect.AspectFit,
                    BitmapOptimizations = false,
                    Source = SvgImageSource.FromSvgString(svgString)
                };
                w_grid.Children.Add(svg, 0, 0);

                App.trackItem i = new App.trackItem
                {
                    state = svgString,
                    header = header,
                    type = Itemtypes.Floormap,
                    svgImage = svg
                };
                App.config.items.Add(i);

                foreach (App.trackItem item in App.config.items.Where(n => n.type == Itemtypes.Sensor))
                {
                    Floorplan_update(item);
                }

                //Button must be last to be added to work
                Button dummyButton = new Button
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Color.Transparent,
                };
                w_grid.Children.Add(dummyButton, 0, 0);
                dummyButton.Clicked += OnDummyButtonClicked;
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Floormap", "Widgets.Floormap crashed: " + ex.ToString());
                Error(grid, px, py, sx, sy, ex.ToString());
            }
        }

        private static void Floorplan_update(App.trackItem item)
        {
            foreach (App.trackItem floormap in App.config.items.Where(n => n.type == Itemtypes.Floormap))
            {
                floormap.state = UpdateOnOff(item, floormap.state);
                floormap.svgImage.Source = SvgImageSource.FromSvgString(floormap.state);
                floormap.svgImage.ReloadImage();
            }
        }

        private static string UpdateOnOff(App.trackItem item, string svg)
        {
            int on = 1;
            int off = 0;

            if (item.state.ToUpper().Equals("CLOSED"))
            {
                on = 0;
                off = 1;
            }

            RegexOptions options = RegexOptions.IgnoreCase;
            svg = Regex.Replace(svg, "(.*" + item.name + ".on.*stroke-opacity:)([0-1])(.*)", m => m.Groups[1].Value + on.ToString() + m.Groups[3].Value, options);
            svg = Regex.Replace(svg, "(.*" + item.name + ".off.*stroke-opacity:)([0-1])(.*)", m => m.Groups[1].Value + off.ToString() + m.Groups[3].Value, options);

            return svg;
        }
    }
}
