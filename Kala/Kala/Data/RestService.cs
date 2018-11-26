using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xamarin.Forms;
using Plugin.Logger;
using Newtonsoft.Json.Linq;

namespace Kala
{
    public class RestService : IRestService
	{
        private HttpClient Client { get; set; } = null;

        //Keep track of updates
        public static Queue<string> QueueUpdates { get; set; } = new Queue<string>();
        public static bool BoolExit { get; set; } = true;

        public RestService()
		{
            Client = new HttpClient
            {
                MaxResponseContentBufferSize = 256000
            };

            if (Settings.Username != string.Empty)
            {
                var authData = string.Format("{0}:{1}", Settings.Username, Settings.Password);
                var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(authData));
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);
            }
        }
        
        public Models.Sitemaps.Sitemaps ListSitemaps()
		{
            var uri = new Uri(string.Format("{0}://{1}:{2}/{3}", Settings.Protocol, Settings.Server, Settings.Port.ToString(), Common.Constants.Api.Sitemaps));
            Client.DefaultRequestHeaders.Add("Accept", "application/json");
            CrossLogger.Current.Debug(@"URI: '" + uri.ToString() + "'");

            try
            {
                var response = Client.GetAsync(uri).Result;
                
                if (!response.IsSuccessStatusCode)
                {
                    CrossLogger.Current.Error("RestService", "Failed: " + response.StatusCode.ToString());
                    return null;
                }

                string resultString = response.Content.ReadAsStringAsync().Result;
                CrossLogger.Current.Debug("RestService", "Content Response: " + resultString.ToString());

                try
                {
                    Models.Sitemaps.Sitemaps sitemaps = new Models.Sitemaps.Sitemaps
                    {
                        Sitemap = ((JArray)JToken.Parse(resultString)).ToObject<List<Models.Sitemaps.Sitemap>>()
                    };

                    CrossLogger.Current.Debug("RestService", "Found " + sitemaps.Sitemap.Count.ToString() + " sitemaps");
                    return sitemaps;
                }
                catch
                {
                    CrossLogger.Current.Error("RestService", "Failed to parse JSON sitemaps response");
                    return null;
                }
            }
            catch
            {
                CrossLogger.Current.Error("RestService", "Failed to enumerate sitemaps");
                return null;
            }
		}

        public Models.Sitemap.Sitemap LoadItemsFromSitemap(Models.Sitemaps.Sitemap sitemap)
        {
            try
            {
                Client.DefaultRequestHeaders.Add("Accept", "application/json");
                if (sitemap.Link == null)
                {
                    return null;
                }
                var uri = new Uri(sitemap.Link);
                Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Debug("Kala", @"URI: '" + uri.ToString() + "'"));

                var response = Client.GetAsync(uri).Result;

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                string resultString = response.Content.ReadAsStringAsync().Result.ToString();
                Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Debug("Kala", @"Content Response: '" + resultString.ToString() + "'"));

                try
                {
                    Models.Sitemap.Sitemap items = JsonConvert.DeserializeObject<Models.Sitemap.Sitemap>(resultString);
                    return items;
                }
                catch
                {
                    Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Error("RestService", "Failed to parse JSON sitemap response"));
                    return null;
                }
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Error("Kala", @"Exception : '" + ex.ToString() + "'"));
                return null;
            }
        }

        public async Task SendCommand(string link, string command)
        {
            App.Config.LastActivity = DateTime.Now; //Update lastActivity to reset Screensaver timer

            var uri = new Uri(string.Format("{0}://{1}:{2}/{3}", Settings.Protocol, Settings.Server, Settings.Port.ToString(), Common.Constants.Api.Items + link));
            Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Debug("Kala", "SendCommand() - URI: " + uri + ", Command:" + command));

            try
            {
                StringContent queryString = new StringContent(command, Encoding.UTF8);
                await Client.PostAsync(uri, queryString);
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Error("Kala", "SendCommand - Failed: " + ex.ToString()));
            }
        }

        public string GetItem(string name)
        {
            Client.DefaultRequestHeaders.Add("Accept", "application/json");
            var uri = new Uri(string.Format("{0}://{1}:{2}/{3}", Settings.Protocol, Settings.Server, Settings.Port.ToString(), Common.Constants.Api.Items + name));
            Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Debug("Kala", "GetItem() - URI: " + uri.ToString()));

            try
            {
                var response = Client.GetAsync(uri).Result;
                if (!response.IsSuccessStatusCode) return null;

                string resultString = response.Content.ReadAsStringAsync().Result;
                Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Debug("Kala", @"Content Response: '" + resultString.ToString() + "'"));

                Models.Item updatedItem = JsonConvert.DeserializeObject<Models.Item>(resultString);
                if (updatedItem.TransformedState != null)
                {
                    return updatedItem.TransformedState.ToString();
                }
                else if (updatedItem.State != null)
                { 
                    return updatedItem.State.ToString();
                }
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Error("Kala", "GetItem() - Failed: " + ex.ToString()));
            }
            return null;
        }

        public async Task GetUpdateAsync()
        {
            var uri = new Uri(string.Format("{0}://{1}:{2}/{3}", Settings.Protocol, Settings.Server, Settings.Port.ToString(), Common.Constants.Api.Events));
            Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Debug("Updates", "URI: " + uri));

            await Task.Run(async() =>
            {
                Client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
                while (true)
                {
                    try
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get, uri);
                        using (var response = await Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                        {
                            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
                            {
                                //Loop through this as quickly as possible to not loose any messages
                                while (!reader.EndOfStream)
                                {
                                    string updates = reader.ReadLine();

                                    //Don't add junk to the queue...
                                    if ((updates.Contains("data: {")) && (updates.Contains("statechanged")))
                                    {
                                        lock (QueueUpdates)
                                        {
                                            QueueUpdates.Enqueue(updates);

                                            if (BoolExit && App.Config.Initialized && QueueUpdates.Count > 0)
                                            {
                                                Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Debug("Kala", "RestService - GetUpdateAsync() - Creating Updates() thread"));
                                                Task.Run((Action)Helpers.Updates);
                                            }
                                            else
                                            {
                                                Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Debug("Kala", "RestService - GetUpdateAsync() - Updates() already running, or not initialized"));
                                            }

                                            Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Debug("Kala", "RestService - GetUpdateAsync() - Updates in queue: " + QueueUpdates.Count.ToString() + ", new update:" + updates));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Error("Kala", "RestService - Update Crashed: "  + ex.ToString()));
                    }
                }
            });
        }
    }
}
