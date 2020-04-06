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
    public partial class Widgets : ContentPage
    {
        public static async void FloormapAsync(Grid grid, int px, int py, int sx, int sy, string header, JObject data)
        {
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Create Floormap Widget");

            try
            {
                Floorplan flooritem = data.ToObject<Floorplan>();
                CrossLogger.Current.Debug("Floormap", "URL: " + flooritem.Url);

                var httpClient = new HttpClient();
                var svgString = await httpClient.GetStringAsync(flooritem.Url);

                //Loop through SVG and find "ID" and add as Sensor
                MatchCollection matches = Regex.Matches(svgString, "(id=\")(.*)(.\\b(on|off))", RegexOptions.IgnoreCase);
                for (int j=0; j < matches.Count; j++)
                {
                    string Itemid = matches[j].Groups[2].Value;
                    if (App.Config.Items.Find(s => s.Name == Itemid)  == null) 
                    {
                        CrossLogger.Current.Debug("Floormap", "Adding: " + Itemid);

                        RestService GetItemUpdate = new RestService();
                        string state = GetItemUpdate.GetItem(Itemid);

                        if (state != null)
                        {
                            App.TrackItem sensor = new App.TrackItem
                            {
                                Name = Itemid,
                                State = state,
                                Type = Itemtypes.Sensor
                            };
                            App.Config.Items.Add(sensor);
                        }
                    }
                }

                #region w_grid
                Grid w_grid = new Grid
                {
                    RowSpacing = 0,
                    ColumnSpacing = 0,
                    Padding = new Thickness(0, 0, 0, 0),
                    BackgroundColor = App.Config.CellColor,
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
                    TextColor = App.Config.TextColor,
                    BackgroundColor = App.Config.CellColor,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Start
                }, 0, 0);
                #endregion Header

                #region SVG
                SvgCachedImage svg = new SvgCachedImage
                {
                    DownsampleToViewSize = false,
                    CacheDuration = TimeSpan.FromMilliseconds(1000),
                    Aspect = Aspect.AspectFit,
                    BitmapOptimizations = false,
                };
                w_grid.Children.Add(svg, 0, 0);

                App.TrackItem svgImage = new App.TrackItem
                {
                    State = svgString,
                    Header = header,
                    Type = Itemtypes.Floormap,
                    SvgImage = svg
                };
                App.Config.Items.Add(svgImage);
                #endregion SVG

                CrossLogger.Current.Debug("Floormap", "SVGBefore");
                foreach (App.TrackItem item in App.Config.Items.Where(n => n.Type == Itemtypes.Sensor))
                {
                    svgImage.State = UpdateOnOff(item, svgImage.State);
                }
                svgImage.SvgImage.Source = SvgImageSource.FromSvgString(svgImage.State);
                svgImage.SvgImage.ReloadImage();
                CrossLogger.Current.Debug("Floormap", "SVGAfter");

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

        private static void Floorplan_update(App.TrackItem item)
        {
            foreach (App.TrackItem floormap in App.Config.Items.Where(n => n.Type == Itemtypes.Floormap))
            {
                floormap.State = UpdateOnOff(item, floormap.State);
                floormap.SvgImage.Source = SvgImageSource.FromSvgString(floormap.State);
                floormap.SvgImage.ReloadImage();
            }
        }

        private static string UpdateOnOff(App.TrackItem item, string svg)
        {
            int on = 1;
            int off = 0;

            if (item.State.ToUpper().Equals("CLOSED"))
            {
                on = 0;
                off = 1;
            }

            RegexOptions options = RegexOptions.IgnoreCase;
            svg = Regex.Replace(svg, "(.*" + item.Name + ".on.*stroke-opacity:)([0-1])(.*)", m => m.Groups[1].Value + on.ToString() + m.Groups[3].Value, options);
            svg = Regex.Replace(svg, "(.*" + item.Name + ".off.*stroke-opacity:)([0-1])(.*)", m => m.Groups[1].Value + off.ToString() + m.Groups[3].Value, options);

            return svg;
        }
    }
}
