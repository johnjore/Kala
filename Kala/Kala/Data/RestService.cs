using System;
using System.IO;
using System.Linq;
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
        HttpClient client;

        //Keep track of updates
        public static Queue<string> queueUpdates = new Queue<string>();
        public static bool boolExit = true;

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
            var uri = new Uri(string.Format("{0}://{1}:{2}/{3}", Settings.Protocol, Settings.Server, Settings.Port.ToString(), Common.Constants.Api.Sitemaps));
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            CrossLogger.Current.Debug(@"URI: '" + uri.ToString() + "'");

            try
            {
                var response = client.GetAsync(uri).Result;
                
                if (!response.IsSuccessStatusCode)
                {
                    CrossLogger.Current.Error("RestService", "Failed: " + response.StatusCode.ToString());
                    return null;
                }

                string resultString = response.Content.ReadAsStringAsync().Result;
                CrossLogger.Current.Debug("RestService", "Content Response: " + resultString.ToString());

                //OH1 and OH2 have different responses
                try
                {
                    //OH1
                    Models.Sitemaps.Sitemaps sitemaps = JsonConvert.DeserializeObject<Models.Sitemaps.Sitemaps>(resultString);
                    CrossLogger.Current.Debug("RestService", "Found " + sitemaps.sitemap.Count().ToString() + " sitemaps");
                    return sitemaps;
                }
                catch
                {
                    try
                    {
                        //OH2
                        Models.Sitemaps.Sitemaps sitemaps = new Models.Sitemaps.Sitemaps();
                        sitemaps.sitemap = ((JArray)JToken.Parse(resultString)).ToObject<List<Models.Sitemaps.Sitemap>>();

                        CrossLogger.Current.Debug("RestService", "Found " + sitemaps.sitemap.Count().ToString() + " sitemaps");
                        return sitemaps;
                    }
                    catch
                    {
                        //OH3?
                        CrossLogger.Current.Debug("RestService", "Failed to parse JSON sitemaps response");
                        return null;
                    }
                }
            }
            catch
            {
                CrossLogger.Current.Debug("RestService", "Failed to enumerate sitemaps");
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

                //Note: OH1 and OH2 have different responses
                try
                {
                    Models.Sitemap.Sitemap items = JsonConvert.DeserializeObject<Models.Sitemap.Sitemap>(resultString);
                    return items;
                }
                catch
                {
                    CrossLogger.Current.Debug("RestService", "Failed to parse JSON sitemap response");
                    return null;
                }
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Debug("Kala", @"Exception : '" + ex.ToString() + "'");
                Application.Current.MainPage.DisplayAlert("Alert", ex.Message, "OK");
                return null;
            }
        }

        public async Task SendCommand(string name, string command)
        {
            App.config.LastActivity = DateTime.Now; //Update lastActivity to reset Screensaver timer

            var uri = new Uri(string.Format("{0}://{1}:{2}/{3}", Settings.Protocol, Settings.Server, Settings.Port.ToString(), Common.Constants.Api.Items + name));
            CrossLogger.Current.Debug("Kala", "SendCommand() - URI: " + uri + ", Command:" + command);

            try
            {
                StringContent queryString = new StringContent(command, Encoding.UTF8);
                HttpResponseMessage response = await client.PostAsync(uri, queryString);
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

        public async Task GetUpdateAsync()
        {
            var uri = new Uri(string.Format("{0}://{1}:{2}/{3}", Settings.Protocol, Settings.Server, Settings.Port.ToString(), Common.Constants.Api.Events, string.Empty));
            CrossLogger.Current.Debug("Updates", "URI: " + uri);

            await Task.Run(async() =>
            {
                client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
                while (true)
                {
                    try
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get, uri);
                        using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                        {
                            using (var body = await response.Content.ReadAsStreamAsync())
                            using (var reader = new StreamReader(body))
                            {
                                //Loop through this as quickly as possible to not loose any messages
                                while (!reader.EndOfStream)
                                {
                                    string updates = reader.ReadLine();

                                    //Don't add junk to the queue...
                                    if (updates.Contains("data: {"))
                                    {
                                        lock (queueUpdates)
                                        {
                                            queueUpdates.Enqueue(updates);

                                            //Process updates?
                                            if (boolExit == true && App.config.Initialized == true && queueUpdates.Count() > 0)
                                            {
                                                Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Debug("Kala", "RestService - GetUpdateAsync() - Creating Updates() thread"));

                                                #pragma warning disable CS4014
                                                Task.Run(async () =>
                                                {
                                                    await Helpers.Updates();
                                                });
                                                #pragma warning restore CS4014
                                            }
                                            else
                                            {
                                                Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Debug("Kala", "RestService - GetUpdateAsync() - Updates() already running, or not initialized"));
                                            }

                                            Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Debug("Kala", "RestService - GetUpdateAsync() - Updates in queue: " + queueUpdates.Count.ToString() + ", new update:" + updates));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        CrossLogger.Current.Debug("Kala", "RestService - Update Crashed: "  + ex.ToString());
                    }
                }
            });
        }
    }
}
