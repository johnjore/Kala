using System.Collections.Generic;

namespace Kala.Models.Sitemap
{
    public class Widget3
    {
        public string widgetId { get; set; }
        public string type { get; set; }
        public string label { get; set; }
        public string icon { get; set; }
        public Item item { get; set; }
        public object widget { get; set; }
    }

    public class LinkedPage
    {
        public string id { get; set; }
        public string title { get; set; }
        public string icon { get; set; }
        public string link { get; set; }
        public string leaf { get; set; }
        public object widget { get; set; }
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

