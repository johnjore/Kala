using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using Plugin.Logger;

namespace Kala
{
    public partial class Widgets
    {
        public static void Gauge_Group(Grid grid, string x1, string y1, string x2, string y2, string header, JArray data)
        {
            int.TryParse(x1, out int px);
            int.TryParse(y1, out int py);
            int.TryParse(x2, out int sx);
            int.TryParse(y2, out int sy);

            try
            {
                Grid t_grid = Create_GaugeHeader(header, sx);

                List<Models.Sitemap.Widget3> items = data.ToObject<List<Models.Sitemap.Widget3>>();

                for (int i = 0; i < items.Count; i++)
                {
                    Create_Gauge(t_grid, i, items[i]);
                    grid.Children.Add(t_grid, px, px + sx, py, py + sy);                    
                }
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Gauge-Group", "Crashed: " + ex.ToString());
                Error(grid, px, py, sx, sy, ex.ToString());
            }
        }
    
        public static void Gauge_Group(Grid grid, string x1, string y1, string x2, string y2, string header, JObject data)
        {
            int.TryParse(x1, out int px);
            int.TryParse(y1, out int py);
            int.TryParse(x2, out int sx);
            int.TryParse(y2, out int sy);

            try
            {                
                Grid t_grid = Create_GaugeHeader(header, 1);

                Models.Sitemap.Widget3 item = data.ToObject<Models.Sitemap.Widget3>();
             
                Create_Gauge(t_grid, 0, item);

                grid.Children.Add(t_grid, px, py);
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Gauge-Group", "Crashed: " + ex.ToString());
                Error(grid, px, py, sx, sy, ex.ToString());
            }
        }
    }
}
