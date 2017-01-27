using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using Xamarin.Forms;
using Plugin.Logger;

namespace Kala
{
	public class RestService : IRestService
	{
        HttpClient client;

		public RestService()
		{
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;

            if (Settings.Username != string.Empty)
            {
                var authData = string.Format("{0}:{1}", Settings.Username, Settings.Password);
                var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(authData));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);
            }
        }
        
        public Models.Sitemaps.Sitemaps ListSitemaps()
		{
            var uri = new Uri(string.Format("{0}://{1}:{2}/{3}", Settings.Protocol, Settings.Server, Settings.Port.ToString(), Common.Constants.Api.Sitemaps, string.Empty));
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            CrossLogger.Current.Debug(@"URI: '" + uri.ToString() + "'");
            
            try
            {
                var response = client.GetAsync(uri).Result;
                
                if (!response.IsSuccessStatusCode)
                {
                    //Application.Current.MainPage.DisplayAlert("Alert", response.StatusCode.ToString(), "OK");
                    return null;
                }

                string resultString = response.Content.ReadAsStringAsync().Result;

                //CrossLogger.Current.Debug("Kala", @"Content Response: '" + resultString.ToString() + "'");
                Models.Sitemaps.Sitemaps sitemaps = JsonConvert.DeserializeObject<Models.Sitemaps.Sitemaps>(resultString);
                //CrossLogger.Current.Debug("Kala", "Sitemaps: " + sitemaps.sitemap.Count().ToString());
                return sitemaps;
            }
            catch
            {
                return null;
            }
		}

        public Models.Sitemap.Sitemap LoadItemsFromSitemap(Models.Sitemaps.Sitemap sitemap)
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var uri = new Uri(sitemap.link);
            CrossLogger.Current.Debug("Kala", @"URI: '" + uri.ToString() + "'");

            try
            {
                var response = client.GetAsync(uri).Result;

                if (!response.IsSuccessStatusCode)
                {
                    Application.Current.MainPage.DisplayAlert("Alert", response.StatusCode.ToString(), "OK");
                    throw new Exception($"{response.StatusCode} received from server");
                }

                string resultString = response.Content.ReadAsStringAsync().Result;

                CrossLogger.Current.Debug("Kala", @"Content Response: '" + resultString.ToString() + "'");

                Models.Sitemap.Sitemap items = JsonConvert.DeserializeObject<Models.Sitemap.Sitemap>(resultString);

                return items;
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Debug("Kala", @"Exception : '" + ex.ToString() + "'");
                Application.Current.MainPage.DisplayAlert("Alert", ex.Message, "OK");
                return null;
            }
        }

        public async Task SendCommand(string link, string command)
        {
            CrossLogger.Current.Debug("Kala", "Link: '" + link + "', Command: '" + command + "'");
            App.config.LastActivity = DateTime.Now; //Update lastactivity to reset Screensaver timer

            try
            {
                StringContent queryString = new StringContent(command, Encoding.UTF8);
                HttpResponseMessage response = await client.PostAsync(new Uri(link), queryString);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    CrossLogger.Current.Debug("Kala", "SendCommand - Success");
                    return;
                }
                else
                {
                    CrossLogger.Current.Debug("Kala", "Sendcommand - Failed: " + response.ToString());
                }
                
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Kala", "SendCommand - Failed: " + ex.ToString());
                return;
            }
        }

        /**///This works, but is probably not the best way. Please fix me?
        public async Task GetUpdate()
        {
            var uri = new Uri(string.Format("{0}://{1}:{2}/{3}", Settings.Protocol, Settings.Server, Settings.Port.ToString(), Common.Constants.Api.Items, string.Empty));

            client.DefaultRequestHeaders.Add("X-Atmosphere-Transport", "long-polling");
            client.DefaultRequestHeaders.Add("X-Atmosphere-tracking-id", (Helpers.GenerateRandomNo(1000, 9999)).ToString());
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
                                    
            while (true)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, uri);
                //CrossLogger.Current.Debug("Kala", "Waiting for connect");
                using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                {
                    //CrossLogger.Current.Debug("Kala", "Waiting for update");
                    using (var body = await response.Content.ReadAsStreamAsync())
                    using (var reader = new StreamReader(body))
                    {
                        while (!reader.EndOfStream)
                        {
                            try
                            {
                                //Occasionally we get multiple updates on a single line
                                string[] updates = reader.ReadLine().Split('}');

                                //Loop through each update.
                                for (int i= 0; i < updates.Count()-1; i++)
                                {
                                    CrossLogger.Current.Debug("Kala", "Got update:" + updates[i].ToString());
                                    Models.Item itemData = null;

                                    try
                                    {
                                        itemData = JsonConvert.DeserializeObject<Models.Item>(updates[i] + "}");
                                        Helpers.Label_Update(itemData);

                                        //Specials. To be removed and cleaned up later
                                        foreach (App.trackItem item in App.config.items)
                                        {
                                            if (item.link.Equals(itemData.link))
                                            {
                                                item.state = itemData.state;
                                                CrossLogger.Current.Debug("Kala", "Found: " + item.link + ", New State: " + item.state);

                                                switch (item.type)
                                                {
                                                    case "DimmerItem":
                                                        Widgets.Dimmer_update(false, item.grid, item.px, item.py, item.header, item.state, item.unit, item.icon, item.link);
                                                        break;
                                                    case "SwitchItem":
                                                        Widgets.Switch_update(false, item.grid, item.px, item.py, item.header, item.state, item.icon, item.link);
                                                        break;
                                                    default:
                                                        CrossLogger.Current.Warn("Kala", "Not processed: " + item.ToString());
                                                        break;
                                                }                                                
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        CrossLogger.Current.Error("Kala", "Error parsing update string: " + updates[i] + ", Ex: " + ex.ToString());
                                        //await Application.Current.MainPage.DisplayAlert("Alert", update.ToString(), "OK");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                CrossLogger.Current.Error("Kala", "Error in GetUpdate() : " + ex.ToString());
                                await Application.Current.MainPage.DisplayAlert("Alert", ex.ToString(), "OK");
                            }
                        }
                    }
                }
            }
        }
    }
}
