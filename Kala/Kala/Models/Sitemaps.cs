using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Kala.Models.Sitemaps
{
    public class Sitemaps
    {
        public List<Sitemap> sitemap { get; set; }
    }

    public class Sitemap
    {
        public string name { get; set; }
        public string label { get; set; }
        public string link { get; set; }
        public Homepage homepage { get; set; }
    }
    
    public class Homepage
    {
        public string link { get; set; }
        public bool leaf { get; set; }
        public List<object> widgets { get; set; }
    }
}
