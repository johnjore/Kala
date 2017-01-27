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
            int px = 0;
            int py = 0;
            int sx = 0;
            int sy = 0;
            
            //If this fails, we dont know where to show an error
            try
            {
                px = Convert.ToInt16(x1);
                py = Convert.ToInt16(y1);
                sx = Convert.ToInt16(x2);
                sy = Convert.ToInt16(y2);
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Gauge-Group", "Crashed: " + ex.ToString());
            }

            try
            {
                //Create Grid w/header
                Grid t_grid = Create_GaugeHeader(header, sx);

                //Items
                List<Models.Sitemap.Widget3> items = data.ToObject<List<Models.Sitemap.Widget3>>();

                //Columns, one per item
                for (int i = 0; i < items.Count; i++)
                {
                    Create_Gauge(t_grid, i, items[i]);

                    grid.Children.Add(t_grid, px, px + sx, py, py + sy);                    
                }
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Gauge-Group", "Crashed: " + ex.ToString());
                Error(grid, px, py, ex.ToString());
            }
        }
    
        public static void Gauge_Group(Grid grid, string x1, string y1, string x2, string y2, string header, JObject data)
        {
            int px = 0;
            int py = 0;
            int sx = 0;
            int sy = 0;

            //If this fails, we dont know where to show an error
            try
            {
                px = Convert.ToInt16(x1);
                py = Convert.ToInt16(y1);
                sx = Convert.ToInt16(x2);
                sy = Convert.ToInt16(y2);
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Gauge-Group", "Crashed: " + ex.ToString());
            }

            try
            {
                //Create Grid w/header
                Grid t_grid = Create_GaugeHeader(header, 1);

                //Items
                Models.Sitemap.Widget3 item = data.ToObject<Models.Sitemap.Widget3>();

                //Column
                Create_Gauge(t_grid, 0, item);

                grid.Children.Add(t_grid, px, py);
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Gauge-Group", "Crashed: " + ex.ToString());
                Error(grid, px, py, ex.ToString());
            }
        }
    }
}
