using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;
using Plugin.Logger;
using DrawShape;
using Xamarin.Forms.GoogleMaps;
using Newtonsoft.Json;
using System.Threading.Tasks;

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
            {"n",      0},
            {"nne",   23},
            {"ne",    45},
            {"ene",   68},
            {"e",     90},
            {"ese",  113},
            {"se",   135},
            {"sse",  158},
            {"s",    180},
            {"ssw",  203},
            {"sw",   225},
            {"wsw",  248},
            {"w",    270},
            {"wnw",  293},
            {"nw",   315},
            {"nnw",  338}
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

                CrossLogger.Current.Debug("Kala", "Label: " + label + ", Command: " + command);

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

        //Process Update Messages
        #pragma warning disable CS1998
        public static async Task Updates()
        {
            RestService.boolExit = false;
            try
            {
                Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Debug("Updates", "Processing updates"));

                while (RestService.boolExit == false)
                {
                    lock (RestService.queueUpdates)
                    {
                        while (RestService.queueUpdates.Count > 0)
                        {
                            string tmpS = string.Empty;
                            try
                            {
                                tmpS = RestService.queueUpdates.Dequeue();
                                //System.Diagnostics.Debug.WriteLine("Updates in queue: " + RestService.queueUpdates.Count.ToString() + ", old update:" + tmpS);

                                if (tmpS.StartsWith("data: "))
                                {
                                    tmpS = tmpS.Remove(0, 6);
                                    System.Diagnostics.Debug.WriteLine("Updates " + tmpS);
                                }

                                try
                                {
                                    Models.Events itemData = JsonConvert.DeserializeObject<Models.Events>(tmpS);

                                    //Rewrite topic to only use item name
                                    var tmpA = itemData.topic.Split('/');
                                    itemData.topic = tmpA[tmpA.Count() - 2];
                                    
                                    //Add value to item
                                    Models.Payload payload = JsonConvert.DeserializeObject<Models.Payload>(itemData.payload);
                                    itemData.value = payload.value;
                                    System.Diagnostics.Debug.WriteLine("Found: " + itemData.topic + ", New State: " + itemData.value);

                                    Device.BeginInvokeOnMainThread(() => GUI_Update(itemData));

                                    //Specials. To be removed and cleaned up later
                                    foreach (App.trackItem item in App.config.items)
                                    {
                                        if (item.name.Equals(itemData.topic) && itemData.value != null)
                                        {
                                            item.state = itemData.value;

                                            switch (item.type)
                                            {
                                                case Models.Itemtypes.Dimmer:
                                                    Device.BeginInvokeOnMainThread(() => Widgets.Dimmer_update(false, item));
                                                    break;
                                                case Models.Itemtypes.Switch:
                                                    Device.BeginInvokeOnMainThread(() => Widgets.Switch_update(false, item));
                                                    break;
                                                case Models.Itemtypes.Sensor:
                                                    Device.BeginInvokeOnMainThread(() => Widgets.Sensor_update(false, item));
                                                    break;
                                                default:
                                                    Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Warn("Updates", "Not processed: " + item.ToString()));
                                                    break;
                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                            catch (Exception ex)
                            {
                                Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Error("Updates", "Crashed on " + tmpS + ", " + ex.ToString()));
                            }

                            Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Debug("Updates", "Messages in queue: " + RestService.queueUpdates.Count.ToString()));

                            //Get out of here asap if exit signal is sent
                            if (RestService.boolExit)
                            {
                                Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Info("Updates", "Asked to exit"));
                                return;
                            }
                        }

                        RestService.boolExit = true;
                        Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Debug("Updates", "No more messages to send. Task will now end"));
                    }
                }
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Error("Updates", "Crashed: " + ex.ToString()));
            }
            return;
        }
        #pragma warning restore CS1998

        //Update GUI
        public static void GUI_Update(Models.Events item)
        {
            #region Generic labels
            foreach (ItemLabel lbl in App.config.itemlabels)
            {
                if (lbl.Name != null && lbl.Name.Equals(item.topic) && item.value != null)
                {
                    //Manage special cases
                    switch (lbl.Type)
                    {
                        case Models.Itemtypes.Winddirection:
                            int w_direction = 0;
                            wind_direction.TryGetValue(item.value.ToLower(), out w_direction);
                            lbl.Rotation = w_direction;
                            break;
                        case Models.Itemtypes.Weathericon:
                            lbl.Text = Widgets.WeatherCondition(item.value);
                            break;
                        default:
                            //If Digits is set, round off the value
                            if (lbl.Digits > -1)
                            {
                                item.value = Math.Round(Convert.ToDouble(item.value), lbl.Digits).ToString("f" + lbl.Digits);
                            }

                            lbl.Text = lbl.Pre + item.value + lbl.Post;
                            break;
                    }
                }
            }
            #endregion Generic labels

            #region Calendar
            foreach (Models.calItems lbl in Widgets.itemCalendar)
            {
                if (lbl.Name.Equals(item.topic))
                {
                    Widgets.Calendar_Update(item);
                }
            }
            #endregion Calendar

            #region ShapeViews
            List<ShapeView> tmp = new List<ShapeView>(App.config.itemShapeViews);
            foreach (ShapeView sv in tmp)
            {
                if (sv.Name.Equals(item.topic) && item.value != null)
                {
                    try
                    {
                        float.TryParse(item.value, out float state);

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

                        sv.IndicatorPercentage = (float)((state - sv.Min) / (sv.Max - sv.Min) * 100.0f);

                        //Update GUI
                        Grid g = (Grid)sv.Parent;
                        g.Children.Remove(sv);
                        g.Children.Add(sv);                        
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
                try
                {
                    var latitudes = new List<double>();
                    var longitudes = new List<double>();
                    bool update = false;

                    foreach (Pin pin in map.Pins)
                    {
                        if (pin.Tag.Equals(item.topic) && item.value != null)
                        {
                            CrossLogger.Current.Info("Map", "Update");
                            var b = item.value.Split(',');
                            if (b.Count() > 2)
                            {
                                pin.Position = new Position(Convert.ToDouble(b[0]), Convert.ToDouble(b[1]));
                                update = true;
                            }
                        }

                        latitudes.Add(pin.Position.Latitude);
                        longitudes.Add(pin.Position.Longitude);
                    }

                    if (update)
                        Widgets.MapUpdate(latitudes, longitudes, map);
                }
                catch (Exception ex)
                {
                    CrossLogger.Current.Error("Kala", "Map Update crashed: " + ex.ToString());
                }
            }
            #endregion Maps
        }
    }
}