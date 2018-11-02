using System;
using System.Threading.Tasks;

namespace Kala
{
    public class OpenHab
    {
        IRestService RestService { get; set; } = null;

        public OpenHab(IRestService service)
        {
            RestService = service;
        }

        public Models.Sitemaps.Sitemaps ListSitemaps()
        {
            return RestService.ListSitemaps();
        }

        public Models.Sitemap.Sitemap LoadItemsFromSitemap(Models.Sitemaps.Sitemap sitemap)
        {
            return RestService.LoadItemsFromSitemap(sitemap);
        }
        
        public Task SendCommand(string link, string command)
        {
            return RestService.SendCommand(link, command);
        }
    }
}
