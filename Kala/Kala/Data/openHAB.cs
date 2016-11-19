using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kala
{
    public class openHAB
    {
        IRestService restService;

        public openHAB(IRestService service)
        {
            restService = service;
        }

        public Models.Sitemaps.Sitemaps ListSitemaps()
        {
            return restService.ListSitemaps();
        }

        public Models.Sitemap.Sitemap LoadItemsFromSitemap(Models.Sitemaps.Sitemap sitemap)
        {
            return restService.LoadItemsFromSitemap(sitemap);
        }
        
        public Task SendCommand(string link, string command)
        {
            return restService.SendCommand(link, command);
        }
    }
}