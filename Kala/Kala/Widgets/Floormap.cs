using System;
using System.Collections.Generic;
using Xamarin.Forms;
using FFImageLoading.Svg.Forms;
using Newtonsoft.Json.Linq;
using Plugin.Logger;

namespace Kala
{
    public partial class Widgets
    {
        public static void Floormap(Grid grid, string x1, string y1, string x2, string y2, string header, JArray data)
        {
            int.TryParse(x1, out int px);
            int.TryParse(y1, out int py);
            int.TryParse(x2, out int sx);
            int.TryParse(y2, out int sy);

            List<Models.Sitemap.Widget3> items = null;
            Dictionary<string, string> widgetKeyValuePairs = null;

            try
            {
                var a = (JArray)data.SelectToken("url");

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

                #region Floormap graphics
                var image = new SvgCachedImage
                {
                    DownsampleToViewSize = false,
                    CacheDuration = TimeSpan.FromMilliseconds(1000),
                    Aspect = Aspect.AspectFit,
                    BitmapOptimizations = true,
                    Source = SvgImageSource.FromUri(new Uri("http://192.168.1.44/file1.svg")),
                    //Source = SvgImageSource.FromUri(new Uri(a)),
                };
                w_grid.Children.Add(image, 0, 0);
                #endregion Floormap graphics
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Floormap", "Widgets.Floormap crashed: " + ex.ToString());
                Error(grid, px, py, sx, sy, ex.ToString());
            }

            /*
            try
            {
                items = data.ToObject<List<Models.Sitemap.Widget3>>();

                foreach(Models.Sitemap.Widget3 item in items)
                {
                    Dictionary<string, string> widgetKeyValuePairs = Helpers.SplitCommand(item.label);
                }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Floormap", "Crashed: " + ex.ToString());
            }
            */
        }
    }
}
