// https://github.com/xamarin/xamarin-forms-book-samples/tree/master/Chapter20/TaskDelayClock

using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;
using FFImageLoading.Forms;
using Plugin.Logger;

namespace Kala
{
    public partial class Widgets
    {
        static Random random = new Random();
        static Page PreviousPage;
        static Label l_clock;
        static bool active = false;
        static CachedImage image = null;
        public static string url = String.Empty;

        public static void Screensaver(Int64 timeOut)
        {
            CrossLogger.Current.Debug("Screensaver", "Configuring screensaver, using timeout of " + timeOut.ToString());

            Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            {
                CrossLogger.Current.Debug("Screensaver", "Check if time to show screensaver");

                if ((App.config.LastActivity.AddSeconds(timeOut) < DateTime.Now) && active==false)
                {
                    CrossLogger.Current.Debug("Screensaver", "Enable Screensaver");

                    IScreen dim = DependencyService.Get<IScreen>();
                    dim.SetBacklight(0.0f);
                    dim = null;
                    active = true;

                    PreviousPage = Application.Current.MainPage;
                    Application.Current.MainPage = CreatePage();

                    InfiniteLoop();
                }

                return true;
            });
        }

        private static void OnResumeButtonClicked(object sender, EventArgs e)
        {
            CrossLogger.Current.Debug("Screensaver", "Button Pressed");
            active = false;
            Application.Current.MainPage = PreviousPage;
            App.tp.CurrentPage = App.tp.Children[0];        //Revert to first tab when resuming
            App.config.LastActivity = DateTime.Now;         //Update lastactivity to reset Screensaver timer

            IScreen dim = DependencyService.Get<IScreen>();
            dim.SetBacklight(0.8f);
            dim = null;
        }

        private static ContentPage CreatePage()
        {
            AbsoluteLayout absoluteLayout = new AbsoluteLayout
            {
                BackgroundColor = App.config.BackGroundColor
            };

            switch (App.config.ScreenSaverType)
            {
                case Models.ScreenSaverTypes.Clock:
                    l_clock = new Label
                    {
                        FontSize = 72,
                        TextColor = App.config.TextColor,
                        BackgroundColor = App.config.BackGroundColor,
                    };
                    AbsoluteLayout.SetLayoutFlags(l_clock, AbsoluteLayoutFlags.PositionProportional);
                    absoluteLayout.Children.Add(l_clock);
                    break;
                case Models.ScreenSaverTypes.Images:
                    //Source is added later
                    image = new CachedImage()
                    {
                        DownsampleToViewSize = true,
                        CacheDuration = TimeSpan.FromMilliseconds(1000),
                        RetryCount = 0,
                        RetryDelay = 250,
                        Aspect = Aspect.AspectFit,
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        WidthRequest = 2000,
                        HeightRequest = 2000,
                        BitmapOptimizations = true,
                    };
                    AbsoluteLayout.SetLayoutBounds(image, new Rectangle(0.0, 0.0, 1.0, 1.0));
                    AbsoluteLayout.SetLayoutFlags(image, AbsoluteLayoutFlags.All);
                    absoluteLayout.Children.Add(image);
                    break;
            }

            Button resumeButton = new Button
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent,
                AnchorX = 0,
                AnchorY = 0,
                HeightRequest = 1000,
                WidthRequest = 1000,
            };
            resumeButton.Clicked += OnResumeButtonClicked;
            AbsoluteLayout.SetLayoutFlags(resumeButton, AbsoluteLayoutFlags.None);
            absoluteLayout.Children.Add(resumeButton);

            return (new ContentPage {
                Content = absoluteLayout
            });
        }

        private async static void InfiniteLoop()
        {
            while (active == true)
            {
                switch (App.config.ScreenSaverType)
                {
                    case Models.ScreenSaverTypes.Clock:
                        l_clock.Text = DateTime.Now.ToString("HH:mm");
                        AbsoluteLayout.SetLayoutBounds(l_clock, new Rectangle(random.NextDouble(), random.NextDouble(), AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
                        break;
                    case Models.ScreenSaverTypes.Images:
                        GetImage(url);
                        break;
                }
                await Task.Delay(5000);
            }
        }

        public static void GetImage(string url)
        {
            HttpClient client = new HttpClient
            {
                MaxResponseContentBufferSize = 256000
            };
            var response = client.GetAsync(url).Result;

            if (!response.IsSuccessStatusCode)
            {
                CrossLogger.Current.Error("Screensaver", "Failed: " + response.StatusCode.ToString());
                return;
            }

            string html = response.Content.ReadAsStringAsync().Result;
            Regex regex = new Regex("<a href=\".*\">(?<name>.*)</a>");
            MatchCollection matches = regex.Matches(html);
            if (matches.Count > 0)
            {
                Random random = new Random();
                int randomNumber = random.Next(0, matches.Count);
                image.Source = url + "/" + matches[randomNumber].Groups["name"].ToString();
            }
        }
    }
}
