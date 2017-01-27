using System;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using FFImageLoading;
using FFImageLoading.Forms;
using FFImageLoading.Cache;
using Plugin.Logger;

namespace Kala
{
    public partial class Widgets
    {
        /// <summary>
        /// Create Image Widget
        /// </summary>
        /// <returns>nothing</returns>
        public static void Image(Grid grid, string x1, string y1, string x2, string y2, string header, string straspect, JObject data)
        {
            CrossLogger.Current.Debug("Image", "Creating Image Widget");

            int px = 0; 
            int py = 0;
            int sx = 0;
            int sy = 0;
            Aspect aspect = Aspect.AspectFit;

            //If this fails, we don't know where to show the error
            try
            {
                px = Convert.ToInt16(x1);
                py = Convert.ToInt16(y1);
                sx = Convert.ToInt16(x2);
                sy = Convert.ToInt16(y2);
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Image", "Crashed: " + ex.ToString());
            }

            try
            {
                Models.image item = data.ToObject<Models.image>();
                CrossLogger.Current.Debug("Image", "URL: " + item.url);

                //Aspect ratio
                switch (straspect.ToLower())
                {
                    case "fill":
                        aspect = Aspect.Fill;
                        break;
                    case "aspectfill":
                        aspect = Aspect.AspectFill;
                        break;
                    case "aspectfit":
                        aspect = Aspect.AspectFit;
                        break;
                }

                var image = new CachedImage()
                {
                    DownsampleToViewSize = false,
                    CacheDuration = TimeSpan.FromMilliseconds(1000),
                    Aspect = aspect,
                    RetryCount = 0,
                    RetryDelay = 250,
                    BitmapOptimizations = true,
                    Source = item.url
                };
                grid.Children.Add(image, px, px + sx, py, py + sy);

                Label l_header = new Label
                {
                    Text = header,
                    FontSize = 20,
                    TextColor = App.config.TextColor,
                    BackgroundColor = Color.Transparent,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Start
                };
                grid.Children.Add(l_header, px, px + sx, py, py + sy);

                // Start the refresh time
                if (item.refresh != string.Empty)
                {
                    int refresh = Convert.ToInt32(item.refresh);
                    if (refresh > 0)
                    {
                        Device.StartTimer(TimeSpan.FromMilliseconds(refresh), () =>
                        {
                            //If not clearing the cache, image does not refresh
                            ImageService.Instance.InvalidateCacheAsync(CacheType.All);

                            CrossLogger.Current.Debug("Image", "Refresh Image: " + image.Id.ToString());
                            try
                            {
                                /**///image.ReloadImage();
                            }
                            catch (Exception ex)
                            {
                                CrossLogger.Current.Error("Image", "Failed to refresh Image: " + ex.ToString());
                            }
                            return true;
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Image", "Image crashed: " + ex.ToString());
                Error(grid, px, py, ex.ToString());
            }
        }
    }
}
