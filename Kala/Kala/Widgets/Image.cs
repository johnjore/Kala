using System;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using FFImageLoading;
using FFImageLoading.Forms;
using FFImageLoading.Cache;
using Plugin.Logger;

namespace Kala
{
    public partial class Widgets : ContentPage
    {
        /// <summary>
        /// Create Image Widget
        /// </summary>
        /// <returns>nothing</returns>
        public static void Image(Grid grid, int px, int py, int sx, int sy, string header, string straspect, JObject data)
        {
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Create Image Widget");
            CrossLogger.Current.Debug("Image", "Creating Image Widget");


            try
            {
                Models.Image item = data.ToObject<Models.Image>();
                CrossLogger.Current.Debug("Image", "URL: " + item.Url);

                //Aspect ratio
                Aspect aspect = Aspect.AspectFit;
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
                
                var img = new CachedImage()
                {
                    DownsampleToViewSize = false,
                    CacheDuration = TimeSpan.FromMilliseconds(1000),
                    //Aspect = aspect,
                    RetryCount = 999,
                    RetryDelay = 2000,
                    BitmapOptimizations = true,
                    Source = item.Url
                };
                grid.Children.Add(img, px, px + sx, py, py + sy);

                Label l_header = new Label
                {
                    Text = header,
                    FontSize = 20,
                    TextColor = App.Config.TextColor,
                    BackgroundColor = Color.Transparent,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Start
                };
                grid.Children.Add(l_header, px, px + sx, py, py + sy);

                //Button must be last to be added to work
                ItemImageButton ImageButton = new ItemImageButton
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Color.Transparent,
                    Name = header,
                    URL = item.Url,
                };
                grid.Children.Add(ImageButton, px, px + sx, py, py + sy);
                ImageButton.Clicked += OnImageButtonClicked;

                // Start the refresh time
                if (item.Refresh != string.Empty)
                {
                    int refresh = Convert.ToInt32(item.Refresh);
                    if (refresh > 0)
                    {
                        Device.StartTimer(TimeSpan.FromMilliseconds(refresh), () =>
                        {
                            Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Debug("Image", "Refresh Image: " + item.Url));                          
                            try
                            {
                                //If not clearing the cache, image does not refresh
                                ImageService.Instance.InvalidateCacheAsync(CacheType.All);
                                img.ReloadImage();
                            }
                            catch (Exception ex)
                            {
                                Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Error("Image", "Failed to refresh Image: " + ex.ToString()));
                            }
                            return true;
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Image", "Image crashed: " + ex.ToString());
                Error(grid, px, py, sx, sy, ex.ToString());
            }
        }

        private static void OnImageButtonClicked(object sender, EventArgs e)
        {
            ItemImageButton button = sender as ItemImageButton;

            Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Info("Image", "Button ID: '" + button.Id.ToString() + ", Image ID: " + button.URL + ", Name: " + button.Name));

            PreviousPage = Application.Current.MainPage;
            Application.Current.MainPage = CreateImagePage(button);
        }

        private static ContentPage CreateImagePage(ItemImageButton ImageInfo)
        {
            App.Config.LastActivity = DateTime.Now;

            Label label_1 = new Label
            {
                Text = ImageInfo.Name,
                FontSize = 24,
                TextColor = App.Config.TextColor,
                BackgroundColor = Color.Transparent,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Start
            };

            CachedImage img = new CachedImage()
            {
                DownsampleToViewSize = false,
                CacheDuration = TimeSpan.FromMilliseconds(1000),
                Aspect = Aspect.AspectFit,
                RetryCount = 999,
                RetryDelay = 1000,
                BitmapOptimizations = true,
                Source = ImageInfo.URL,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };

            Label label_2 = new Label
            {
                Text = "Close",
                FontSize = 50,
                TextColor = App.Config.TextColor,
                BackgroundColor = Color.Transparent,
                HorizontalTextAlignment = TextAlignment.End,
                VerticalTextAlignment = TextAlignment.End
            };

            Button button = new Button
            {
                BackgroundColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };
            button.Clicked += (sender, e) => {
                Application.Current.MainPage = PreviousPage;
            };

            return (new ContentPage
            {
                Content = new Grid
                {
                    Children = { img, label_1, label_2, button },
                    BackgroundColor = App.Config.CellColor,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand
                }
            });
        }
    }
}
