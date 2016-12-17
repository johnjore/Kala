// https://github.com/xamarin/xamarin-forms-book-samples/tree/master/Chapter20/TaskDelayClock

using System;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace Kala
{
    public partial class Widgets
    {
        static Random random = new Random();
        static Page PreviousPage;
        static Label l_clock;
        static bool active = false;

        public static void Screensaver(int timeOut)
        {
            Debug.WriteLine("Configuring screensaver, using timeout of " + timeOut.ToString());

            Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            {
                Debug.WriteLine("Check if time to show screensaver");

                if (App.config.LastActivity.AddSeconds(timeOut) < DateTime.Now && active==false)
                {
                    Debug.WriteLine("Enable Screensaver");
                    PreviousPage = Application.Current.MainPage;

                    IDim dim = DependencyService.Get<IDim>();
                    dim.SetBacklight(0.0f);
                    active = true;

                    Application.Current.MainPage = CreatePage();
                    InfiniteLoop();

                    dim = null;
                }

                return true;
            });
        }

        private static void OnResumeButtonClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("Button Pressed");
            active = false;
            Application.Current.MainPage = PreviousPage;
            App.tp.CurrentPage = App.tp.Children[0];        //Revert to first tab when resuming
            App.config.LastActivity = DateTime.Now;                 //Update lastactivity to reset Screensaver timer

            IDim dim = DependencyService.Get<IDim>();
            dim.SetBacklight(0.8f);
            dim = null;
        }
        
        private static ContentPage CreatePage()
        {
            AbsoluteLayout absoluteLayout = new AbsoluteLayout
            {
                BackgroundColor = App.config.BackGroundColor
            };

            l_clock = new Label
            {
                FontSize = 72,
                TextColor = App.config.TextColor,
                BackgroundColor = App.config.BackGroundColor,
            };
            AbsoluteLayout.SetLayoutFlags(l_clock, AbsoluteLayoutFlags.PositionProportional);

            Button resumeButton = new Button
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent,
                AnchorX = 0,
                AnchorY = 0,
                HeightRequest = 1000,
                WidthRequest = 1000
            };
            AbsoluteLayout.SetLayoutFlags(resumeButton, AbsoluteLayoutFlags.None);
            resumeButton.Clicked += OnResumeButtonClicked;

            absoluteLayout.Children.Add(l_clock);
            absoluteLayout.Children.Add(resumeButton);

            ContentPage cp = new ContentPage();
            cp.Content = absoluteLayout;

            return cp;
        }

        async static void InfiniteLoop()
        {
            while (active == true)
            {
                l_clock.Text = DateTime.Now.ToString("HH:mm");
                AbsoluteLayout.SetLayoutBounds(l_clock, new Rectangle(random.NextDouble(),
                                                                    random.NextDouble(),
                                                                    AbsoluteLayout.AutoSize,
                                                                    AbsoluteLayout.AutoSize));
                await Task.Delay(5000);
            }
        }
    }
}
