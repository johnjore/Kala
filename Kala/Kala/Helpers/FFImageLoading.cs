using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using FFImageLoading;
using FFImageLoading.Config;
using FFImageLoading.Cache;

namespace Kala.FFImageLoading
{
    public class AuthenticatedHttpImageClientHandler : HttpClientHandler
    {
        public static void Initialize()
        {
            ImageService.Instance.Initialize(new Configuration
            {
                HttpClient = new HttpClient(new AuthenticatedHttpImageClientHandler())
            });

            ImageService.Instance.InvalidateCacheAsync(CacheType.All);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (Settings.Username != string.Empty)
            {
                var authData = string.Format("{0}:{1}", Settings.Username, Settings.Password);
                var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(authData));
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);
            }
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);            
        }
    }
}
