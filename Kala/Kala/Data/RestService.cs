using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using Xamarin.Forms;
using System.Threading;
using System.IO;

namespace Kala
{
	public class RestService : IRestService
	{
        HttpClient client;

		public RestService()
		{
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;

            if (Settings.Username != String.Empty)
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
            Debug.WriteLine(@"URI: '" + uri.ToString() + "'");
            
            try
            {
                var response = client.GetAsync(uri).Result;
                
                if (!response.IsSuccessStatusCode)
                {
                    //Application.Current.MainPage.DisplayAlert("Alert", response.StatusCode.ToString(), "OK");
                    return null;
                }

                string resultString = response.Content.ReadAsStringAsync().Result;
                
                //Debug.WriteLine(@"Content Response: '" + resultString.ToString() + "'");
                Models.Sitemaps.Sitemaps sitemaps = JsonConvert.DeserializeObject<Models.Sitemaps.Sitemaps>(resultString);
                //Debug.WriteLine("Sitemaps: " + sitemaps.sitemap.Count().ToString());
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
            Debug.WriteLine(@"URI: '" + uri.ToString() + "'");

            try
            {
                var response = client.GetAsync(uri).Result;

                if (!response.IsSuccessStatusCode)
                {
                    Application.Current.MainPage.DisplayAlert("Alert", response.StatusCode.ToString(), "OK");
                    throw new Exception($"{response.StatusCode} received from server");
                }

                string resultString = response.Content.ReadAsStringAsync().Result;

                Debug.WriteLine(@"Content Response: '" + resultString.ToString() + "'");

                Models.Sitemap.Sitemap items = JsonConvert.DeserializeObject<Models.Sitemap.Sitemap>(resultString);

                return items;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Exception : '" + ex.ToString() + "'");
                Application.Current.MainPage.DisplayAlert("Alert", ex.Message, "OK");
                return null;
            }
        }

        public async Task SendCommand(string link, string command)
        {
            Debug.WriteLine("Link: '" + link, "', Command: '" + command + "'");
            App.config.LastActivity = DateTime.Now; //Update lastactivity to reset Screensaver timer

            try
            {
                StringContent queryString = new StringContent(command, Encoding.UTF8);
                HttpResponseMessage response = await client.PostAsync(new Uri(link), queryString);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("Success");
                    return;
                }
                else
                {
                    Debug.WriteLine("Failed: " + response.ToString());
                }
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed: " + ex.ToString());
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
                //Debug.WriteLine("Waiting for connect");
                using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                {
                    //Debug.WriteLine("Waiting for update");
                    using (var body = await response.Content.ReadAsStreamAsync())
                    using (var reader = new StreamReader(body))
                    {
                        while (!reader.EndOfStream)
                        {
                            //Debug.WriteLine("Got update");
                            try
                            {
                                /**/ //Can this be optimized a little?
                                string update = reader.ReadLine();
                                //Debug.WriteLine("Update: " + update);
                                //Occasionally we get multiple updates on a single line
                                string[] updates = update.Split('}');

                                //Loop through each update.
                                for (int i= 0; i < updates.Count()-1; i++)
                                {
                                    Models.Sitemap.Item itemData = null;
                                    try
                                    {
                                        /**/ //Fix  this...
                                        itemData = JsonConvert.DeserializeObject<Models.Sitemap.Item>(updates[i] + "}");

                                        //Check and update all labels
                                        foreach (Label lbl in App.config.labels)
                                        {
                                            if (itemData.link.Equals(lbl.StyleId))
                                            {
                                                Helpers.Label_Update(itemData);
                                            }
                                        }

                                        //Specials
                                        foreach (App.trackItem item in App.config.items)
                                        {
                                            if (item.link.Equals(itemData.link))
                                            {
                                                item.state = itemData.state;
                                                switch (item.type)
                                                {
                                                    case "NumericItem":
                                                        Widgets.Gauge_update(false, item.grid, item.px, item.py, item.header, item.min, item.max, item.state, item.unit, item.icon, item.link);
                                                        break;
                                                    case "DimmerItem":
                                                        Widgets.Dimmer_update(false, item.grid, item.px, item.py, item.header, item.state, item.unit, item.icon, item.link);
                                                        break;
                                                    case "SwitchItem":
                                                        Widgets.Switch_update(false, item.grid, item.px, item.py, item.header, item.state, item.icon, item.link);
                                                        break;
                                                    default:
                                                        Debug.WriteLine("Not processed: " + item.ToString());
                                                        break;
                                                }

                                                Debug.WriteLine("Found: " + item.link + ", New State: " + item.state);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine("Error parsing update string:" + update + ", Ex: " + ex.ToString());
                                        //await Application.Current.MainPage.DisplayAlert("Alert", update.ToString(), "OK");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Error in GetUpdate() : " + ex.ToString());
                                await Application.Current.MainPage.DisplayAlert("Alert", ex.ToString(), "OK");
                            }
                        }
                    }
                }
            }
        }
    }
}
