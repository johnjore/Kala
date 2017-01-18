using System;
using System.Diagnostics;
using Xamarin.Forms;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Plugin.Logger;

namespace Kala
{
    public partial class Widgets
    {
        public static void Calendar(Grid grid, string x1, string y1, string x2, string y2, JArray data)
        {
            CrossLogger.Current.Info("Calendar", "Widget Processing Started");
            CrossLogger.Current.Debug("Calendar", data.ToString());
            int px = 0;
            int py = 0;
            int sx = 0;
            int sy = 0;

            try
            {
                //Size of Calendar widget
                px = Convert.ToInt16(x1);
                py = Convert.ToInt16(y1);
                sx = Convert.ToInt16(x2);
                sy = Convert.ToInt16(y2);
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Calendar", "Crashed:" + ex.ToString());
            }

            try
            {
                //Items in Calendar widget
                List<Models.Sitemap.Widget3> items = data.ToObject<List<Models.Sitemap.Widget3>>();
                CrossLogger.Current.Debug("Calendar", "Nr of calendar items: " + items.Count.ToString());

                foreach (Models.Sitemap.Widget3 item in items)
                {
                    int counter = 0;
                    Dictionary<string, string> widgetKeyValuePairs = Helpers.SplitCommand(item.label);

                    if (widgetKeyValuePairs.ContainsKey("item"))
                    {
                        CrossLogger.Current.Debug("Calendar", "Item: " + widgetKeyValuePairs["item"].ToUpper());

                        switch (widgetKeyValuePairs["item"].ToUpper())
                        {
                            case "TITLE":
                                break;
                            case "LOCATION":
                                break;
                            case "START-TIME":
                                break;
                            case "END-TIME":
                                break;
                            case "default":
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Calendar", "Crashed: " + ex.ToString());
                Error(grid, px, py, ex.ToString());
            }

        }
    }
}

/*
 * [0:] Widget : [
  {
    "widgetId": "00_9_0",
    "type": "Text",
    "label": "{item=event1-title}",
    "icon": "calendar",
    "item": {
      "type": "StringItem",
      "name": "CalSharedName1",
      "state": "Blue bin",
      "link": "http://192.168.1.46:8080/rest/items/CalSharedName1"
    }
  },
  {
    "widgetId": "00_9_0_1",
    "type": "Text",
    "label": "{item=event1-location}",
    "icon": "calendar",
    "item": {
      "type": "StringItem",
      "name": "CalSharedPlace1",
      "state": "Home",
      "link": "http://192.168.1.46:8080/rest/items/CalSharedPlace1"
    }
  },
  {
    "widgetId": "00_9_0_1_2",
    "type": "Text",
    "label": "{item=event1-start-time}",
    "icon": "calendar",
    "item": {
      "type": "DateTimeItem",
      "name": "CalSharedStartTime1",
      "state": "2017-01-19T18:00:00",
      "link": "http://192.168.1.46:8080/rest/items/CalSharedStartTime1"
    }
  },
  {
    "widgetId": "00_9_0_1_2_3",
    "type": "Text",
    "label": "{item=event1-end-time}",
    "icon": "calendar",
    "item": {
      "type": "DateTimeItem",
      "name": "CalSharedEndTime1",
      "state": "2017-01-19T18:01:00",
      "link": "http://192.168.1.46:8080/rest/items/CalSharedEndTime1"
    }
  },
*/
