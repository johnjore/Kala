using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;
using Plugin.Logger;
using DrawShape;
using Xamarin.Forms.GoogleMaps;

namespace Kala
{
    public static class DateTimeExtensions
    {
        public static string ToShortTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("t", DateTimeFormatInfo.CurrentInfo);
        }
    }

    public class Helpers : Application
    {
        public static Dictionary<string, int> wind_direction = new Dictionary<string, int>
        {
            {"n",     -0},
            {"e",    -90},
            {"s",   -180},
            {"w",   -270},
            {"nne",  -23},
            {"ese", -113},
            {"ssw", -203},
            {"wnw", -193},
            {"ne",   -45},
            {"se",  -135},
            {"sw",  -225},
            {"nw",  -313},
            {"ene",  -68},
            {"sse", -158},
            {"wsw", -248},
            {"nnw", -336}
        };

        public static Dictionary<string, string> SplitCommand(string instructions)
        {
            Dictionary<string, string> item_keyValuePairs = null;

            try
            {
                int start = instructions.IndexOf("{") + 1;
                int end = instructions.IndexOf("}");

                string label = instructions.Substring(0, start - 1).Trim();
                string command = instructions.Substring(start, end - start);

                //CrossLogger.Current.Debug("Kala", "Label: " + label + ", Command: " + command);

                item_keyValuePairs = command.Split(',').Select(value => value.Split('=')).ToDictionary(pair => pair[0], pair => pair[1]);
                item_keyValuePairs.Add("label", label);
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Kala", "Failed to parse instructions:" + ex.ToString());
            }

            return item_keyValuePairs;
        }

        public static int GenerateRandomNo(int _min, int _max)
        {
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }

        //Update label
        public static void Label_Update(Models.Item item)
        {
            #region Generic labels
            foreach (ItemLabel lbl in App.config.itemlabels)
            {
                if (lbl.Link.Equals(item.link))
                {
                    //Manage special cases
                    switch (lbl.Type)
                    {
                        case Models.Itemtypes.Winddirection:
                            int w_direction = 0;
                            wind_direction.TryGetValue(item.state.ToLower(), out w_direction);
                            lbl.Rotation = w_direction;
                            break;
                        case Models.Itemtypes.Weathericon:
                            lbl.Text = Widgets.WeatherCondition(item.state);
                            break;
                        default:
                            //If Digits is set, round off the value
                            if (lbl.Digits > -1)
                            {
                                item.state = Math.Round(Convert.ToDouble(item.state), lbl.Digits).ToString();
                            }

                            lbl.Text = lbl.Pre + item.state + lbl.Post;
                            break;
                    }
                }
            }
            #endregion Generic labels

            #region Calendar
            foreach (Models.calItems lbl in Widgets.itemCalendar)
            {
                if (lbl.Link.Equals(item.link))
                {
                    Widgets.Calendar_Update(item);
                }
            }
            #endregion Calendar

            #region ShapeViews
            List<ShapeView> tmp = new List<ShapeView>(App.config.itemShapeViews);
            foreach (ShapeView sv in tmp)
            {
                if (sv.Link.Equals(item.link))
                {
                    try
                    {
                        //if (item.link.Contains("Sensor_MasterBedroom_Temperature"))
                        {
                            float state = float.Parse(item.state);

                            //Basic sanity checks
                            if (state > sv.Max) sv.Max = state;
                            if (state < sv.Min) sv.Min = state;

                            //Handle negative ranges
                            if (sv.Min < 0)
                            {
                                sv.Max += Math.Abs(sv.Min);
                                state += (float)Math.Abs(sv.Min);
                                sv.Min = 0;
                            }

                            ShapeView progressArc = new ShapeView
                            {
                                ShapeType = ShapeType.Arc,
                                StrokeColor = App.config.ValueColor,
                                StrokeWidth = 1.0f,
                                Scale = 3.0,
                                Padding = 1,
                                IndicatorPercentage = (float)((state - sv.Min) / (sv.Max - sv.Min) * 100.0f),   //Calculate indicator percentage
                                HorizontalOptions = LayoutOptions.Center,
                                VerticalOptions = LayoutOptions.Center,
                                Min = sv.Min,
                                Max = sv.Max,
                                Link = sv.Link,
                                TranslationY = 78   /**///Why is a TranslationY needed?!?
                            };

                            //Update list with new ShapeView object
                            App.config.itemShapeViews.Remove(sv);
                            App.config.itemShapeViews.Add(progressArc);

                            //Update GUI
                            Grid g = (Grid)sv.Parent;
                            g.Children.Remove(sv);
                            g.Children.Add(progressArc);
                        }
                    }
                    catch (Exception ex)
                    {
                        CrossLogger.Current.Error("Update", "DrawShape Update Crashed: " + ex.ToString());
                    }
                }
            }
            #endregion ShapeViews

            #region Maps
            foreach (Map map in Widgets.itemMaps)
            {
                var latitudes = new List<double>();
                var longitudes = new List<double>();
                bool update = false;

                foreach (Pin pin in map.Pins)
                {
                    if (pin.Tag.Equals(item.link))
                    {
                        CrossLogger.Current.Info("Map", "Update");
                        var b = item.state.Split(',');
                        pin.Position = new Position(Convert.ToDouble(b[0]), Convert.ToDouble(b[1]));
                        update = true;
                    }

                    latitudes.Add(pin.Position.Latitude);
                    longitudes.Add(pin.Position.Longitude);
                }

                if (update)
                    Widgets.MapUpdate(latitudes, longitudes, map);
            }
            #endregion Maps
        }
    }
}