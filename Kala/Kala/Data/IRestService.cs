using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace Kala
{
	public interface IRestService
	{
        /// <summary>
        /// Loads all the sitemaps
        /// </summary>
        /// <returns>A list of sitemaps</returns>
        Models.Sitemaps.Sitemaps ListSitemaps();
        
        /// <summary>
        /// Loads all the items in a sitemap
        /// </summary>
        /// <param name="sitemap">The sitemap to load the items from</param>
        /// <returns>A list of items in the selected sitemap</returns>
        Models.Sitemap.Sitemap LoadItemsFromSitemap(Models.Sitemaps.Sitemap sitemap);

        /// <summary>
        /// Sends a command to an item
        /// </summary>
        /// <param name="item">The item</param>
        /// <param name="command">The Command</param>
        Task SendCommand(string link, string command);

        /// <summary>
        /// Waits for an item update
        /// </summary>
        /// <returns>An update for an item</returns>
        Task GetUpdate();

        /// <summary>
        /// Reset the connection to the OpenHAB server after changing the settings in the app
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        //Task ResetConnection();
    }
}
