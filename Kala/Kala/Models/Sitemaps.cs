using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Kala.Models.Sitemaps
{
    public class Sitemaps
    {
        public List<Sitemap> Sitemap { get; set; }
    }

    public class Sitemap
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Link { get; set; }
        public Homepage Homepage { get; set; }
    }
    
    public class Homepage
    {
        public string Link { get; set; }
        public bool Leaf { get; set; }
        public List<object> Widgets { get; set; }
    }
}
