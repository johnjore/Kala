using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using System;
using System.Threading.Tasks;
using System.Net.Http;

namespace Kala
{
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

                Debug.WriteLine("Label: " + label + ", Command: " + command);

                item_keyValuePairs = command.Split(',').Select(value => value.Split('=')).ToDictionary(pair => pair[0], pair => pair[1]);
                item_keyValuePairs.Add("label", label);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to parse instructions:" + ex.ToString());
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
            foreach (ItemLabel lbl in App.config.itemlabels)
            {
                if (lbl.Link.Equals(item.link))
                {
                    //Special cases
                    if (lbl.Type == Models.Itemtypes.Winddirection)
                    {
                        int w_direction = 0;
                        wind_direction.TryGetValue(item.state.ToLower(), out w_direction);
                        lbl.Rotation = w_direction;
                    }
                    else
                    {
                        lbl.Text = lbl.Pre + item.state + lbl.Post;
                    }
                }
            }
        }

        /**///RunOnUiThread(() => mylabel.Text = "Updated from other thread");
    }
}