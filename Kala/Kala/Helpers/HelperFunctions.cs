using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;
using Plugin.Logger;

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
            //Generic labels
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
                            lbl.Text = lbl.Pre + item.state + lbl.Post;
                            break;
                    }
                }
            }

            //Calendar
            foreach (Models.calItems lbl in Widgets.itemCalendar)
            {
                if (lbl.Link.Equals(item.link))
                {
                    Widgets.Calendar_Update(item);
                }
            }
        }

        /**///RunOnUiThread(() => mylabel.Text = "Updated from other thread");
    }
}