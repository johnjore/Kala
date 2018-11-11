using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using Plugin.Logger;

namespace Kala
{
    public partial class Widgets : ContentPage
    {
        public static void Gauge_Group(Grid grid, string x1, string y1, string x2, string y2, string x3, string y3, string header, JArray data)
        {
            HockeyApp.MetricsManager.TrackEvent("Create Gauge_Group Widget");

            int.TryParse(x1, out int px);
            int.TryParse(y1, out int py);
            int.TryParse(x2, out int sx);
            int.TryParse(y2, out int sy);
            int.TryParse(x3, out int rx);
            int.TryParse(y3, out int ry);

            try
            {
                List<Models.Sitemap.Widget3> items = data.ToObject<List<Models.Sitemap.Widget3>>();

                //If rx / ry not set, make it horizontal
                if (rx == 0) rx = items.Count;
                if (ry == 0) ry = 1;

                Grid t_grid = Create_GaugeHeader(header, rx, ry);

                int item_counter = 0;
                for (int j = 1; j <= items.Count / rx; j++)
                {
                    for (int i = 0; i < items.Count / ry; i++)
                    {
                        CrossLogger.Current.Debug("Gauge", "Position x=" + i.ToString() + ", y=" + j.ToString());
                        Create_Gauge(t_grid, item_counter, items[item_counter], i, j);
                        item_counter++;
                    }
                }
                CrossLogger.Current.Debug("Gauge", "End Of Position. Items: " + items.Count.ToString());
                grid.Children.Add(t_grid, px, px + sx, py, py + sy);
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Gauge-Group", "Crashed: " + ex.ToString());
                Error(grid, px, py, sx, sy, ex.ToString());
            }
        }
    }
}
