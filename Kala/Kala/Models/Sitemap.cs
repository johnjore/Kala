using System.Collections.Generic;

namespace Kala.Models.Sitemap
{
    public class Widget3
    {
        public string WidgetId { get; set; }
        public string Type { get; set; }
        public string Label { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public List<object> Mappings { get; set; }
        public Item Item { get; set; }
        public List<object> Widgets { get; set; }      
    }

    public class LinkedPage
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Link { get; set; }
        public bool Leaf { get; set; }
        public List<Widget3> Widgets { get; set; }
    }

    public class Widget
    {
        public string WidgetId { get; set; }
        public string Type { get; set; }
        public string Label { get; set; }
        public string Icon { get; set; }
        public List<object> Mappings { get; set; }
        public LinkedPage LinkedPage { get; set; }
        public List<object> Widgets { get; set; }
    }

    public class Homepage
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public bool Leaf { get; set; }
        public List<Widget> Widgets { get; set; }
    }

    public class Sitemap
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Link { get; set; }
        public Homepage Homepage { get; set; }
    }
}
