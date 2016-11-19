using System;
using System.Diagnostics;
using Xamarin.Forms;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Kala
{
    public partial class Widgets
    {
        /*
        Frame label = "London {widget=weather,px=2,py=0,sx=2,sy=1}" {
            Text item = Weather_CommonId_London label="{item=condition}"
            Text item = Weather_Condition_London label="{item=condition-caption}"
            Text item = Weather_Temperature_London label="{item=temperature}"

            Text item = Weather_Wind_Direction_London label="{item=wind-direction}"			
            Text item = Weather_Wind_Speed_London label="{item:wind-speed,unit=kph}"

            Text item = Weather_Precip_Probability_London label="{item:precipitation}"
            Text item = Weather_Humidity_London label="{item:humidity}"
        }*/
        
        public static void Weather(Grid grid, string x1, string y1, string x2, string y2, string header, JArray data)
        {
            Debug.WriteLine("Weather: " + data.ToString());

            try
            {
                int px = Convert.ToInt16(x1);
                int py = Convert.ToInt16(y1);
                int sx = Convert.ToInt16(x2);
                int sy = Convert.ToInt16(y2);

                List <Models.Sitemap.Widget3> items = data.ToObject<List<Models.Sitemap.Widget3>>();
                Debug.WriteLine("Weather: " + items.Count.ToString());

                //Create grid layout
                Grid w = new Grid();
                Sitemap.CreateGrid(w, 4, 4);
                w.Padding = 0;
                w.ColumnSpacing = 0;
                w.RowSpacing = 0;
                w.BackgroundColor = App.config.CellColor;

                //Header (Location)
                Label l_header = new Label
                {
                    Text = header,
                    FontSize = 30,
                    TextColor = App.config.TextColor,
                    BackgroundColor = App.config.CellColor,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    TranslationY = -20
                };
                w.Children.Add(l_header, 1, 3, 1, 2);

                foreach (Models.Sitemap.Widget3 item in items)
                {
                    int counter = 0;
                    Dictionary<string, string> widgetKeyValuePairs = Helpers.SplitCommand(item.label);
                    switch (widgetKeyValuePairs["item"])
                    {                        
                        #region mandatory
                        case "condition-caption":
                            Label l_condition = new Label
                            {
                                Text = item.item.state,
                                FontSize = 20,
                                TextColor = App.config.TextColor,
                                BackgroundColor = App.config.CellColor,
                                HorizontalOptions = LayoutOptions.Center,
                                VerticalOptions = LayoutOptions.Center,
                                TranslationY = 20
                            };
                            w.Children.Add(l_condition, 1, 1);
                            break;
                        case "temperature":

                            double temp = Math.Round(Convert.ToDouble(item.item.state), 1, MidpointRounding.AwayFromZero);
                            Label l_temperature = new Label
                            {
                                Text = temp.ToString() + "\u00B0" + widgetKeyValuePairs["unit"],
                                FontSize = 30,
                                TextColor = App.config.TextColor,
                                BackgroundColor = App.config.CellColor,
                                HorizontalOptions = LayoutOptions.End,
                                VerticalOptions = LayoutOptions.Center
                            };
                            w.Children.Add(l_temperature, 3, 1);
                            break;
                        case "condition":
                            string strImage = string.Empty;
                            switch (item.item.state)
                            {
                                case "thunder": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "storm": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "rain-and-snow": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "rain-and-sleet": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "snow-and-sleet": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "freezing-drizzle": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "few-showers": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "freezing-rain": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "rain": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "snow-flurries": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "light-snow": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "blowing-snow": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "snow": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "sleet": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "dust": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "fog": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "wind": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "cold": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "cloudy": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "mostly-cloudy-night": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "mostly-cloudy-day": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "partly-cloudy-night": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "partly-cloudy-day": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "clear-night": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "sunny": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "hot": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "scattered-thunder": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "scattered-showers": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "thundershowers": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "snow-showers": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "scattered-thundershowers": strImage = "ic_cloud_off_white_48dp.png"; break;
                                case "unknown": strImage = "ic_cloud_off_white_48dp.png"; break;
                                default: strImage = "ic_cloud_off_white_48dp.png"; break;
                            }

                            w.Children.Add(new Image
                            {
                                Source = Device.OnPlatform(strImage, strImage, "Assets/" + strImage),
                                Aspect = Aspect.AspectFit,
                                BackgroundColor = App.config.CellColor
                                //VerticalOptions = LayoutOptions.Center
                                //HorizontalOptions = LayoutOptions.Center
                            }, 0, 2, 1, 3);
                            break;
                        #endregion mandatory
                        case "default":
                            counter++;

                            break;

                    }

                }

                grid.Children.Add(w, px, px + sx, py, py + sy);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Weather crashed:" + ex.ToString());
            }


            for (int i = 0; i < data.Count; i++)
            {
                Debug.WriteLine(data[i].ToString());
            }

            /*
            Models.Sitemap.Widget3 item = null;
            Dictionary<string, string> widgetKeyValuePairs = null;

            //If this fails, we dont know where to show an error
            try
            {
                item = data.ToObject<Models.Sitemap.Widget3>();
                widgetKeyValuePairs = Helpers.SplitCommand(item.label);
                Debug.WriteLine("Label: " + widgetKeyValuePairs["label"]);

                px = Convert.ToInt16(widgetKeyValuePairs["px"]);
                py = Convert.ToInt16(widgetKeyValuePairs["py"]);
                sx = Convert.ToInt16(widgetKeyValuePairs["sx"]);
                sy = Convert.ToInt16(widgetKeyValuePairs["sy"]);

                grid.Children.Add(l1, px, px + sx, py, py + sy);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Widgets.Switch crashed: " + ex.ToString());
            }*/
        }
    }
}
