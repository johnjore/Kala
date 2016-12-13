using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Serialization;

namespace Kala.Models.Sitemap
{
    public class Item
    {
        public string type { get; set; }
        public string name { get; set; }
        public string state { get; set; }
        public string link { get; set; }
    }

    public class Widget3
    {
        public string widgetId { get; set; }
        public string type { get; set; }
        public string label { get; set; }
        public string icon { get; set; }
        public Item item { get; set; }
        public object widget { get; set; }
    }

    public class Widget2
    {
        public string widgetId { get; set; }
        public string type { get; set; }
        public string label { get; set; }
        public string icon { get; set; }
        public object widget { get; set; }
    }

    public class LinkedPage
    {
        public string id { get; set; }
        public string title { get; set; }
        public string icon { get; set; }
        public string link { get; set; }
        public string leaf { get; set; }
        public List<Widget2> widget { get; set; }
    }

    public class Widget
    {
        public string widgetId { get; set; }
        public string type { get; set; }
        public string label { get; set; }
        public string icon { get; set; }
        public LinkedPage linkedPage { get; set; }
    }

    public class Homepage
    {
        public string id { get; set; }
        public string title { get; set; }
        public string link { get; set; }
        public string leaf { get; set; }
        public List<Widget> widget { get; set; }
    }

    public class Sitemap
    {
        public string name { get; set; }
        public string label { get; set; }
        public string link { get; set; }
        public Homepage homepage { get; set; }
    }
}

