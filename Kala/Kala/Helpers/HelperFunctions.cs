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
        public static Dictionary<string, string> SplitCommand(string instructions)
        {
            int start = instructions.IndexOf("{") + 1;
            int end = instructions.IndexOf("}");

            string label = instructions.Substring(0, start - 1).Trim();
            string command = instructions.Substring(start, end - start);

            Debug.WriteLine("Label: " + label + ", Command: " + command);

            Dictionary<string, string> item_keyValuePairs = command.Split(',').Select(value => value.Split('=')).ToDictionary(pair => pair[0], pair => pair[1]);
            item_keyValuePairs.Add("label", label);

            return item_keyValuePairs;
        }

        public static int GenerateRandomNo(int _min, int _max)
        {
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }

        //Update label
        public static void Label_Update(Models.Sitemap.Item item)
        {
            foreach (Label lbl in App.config.labels)
            {
                if (lbl.StyleId.Equals(item.link))
                {
                    lbl.Text = lbl.AutomationId + item.state + lbl.ClassId;
                }
            }
        }
        
        /**///RunOnUiThread(() => mylabel.Text = "Updated from other thread");
    }
}