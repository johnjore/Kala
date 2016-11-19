/**/ //Ugly. Replace this with a popup of some description
using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace Kala
{
    public partial class Widgets
    {
        private static ContentPage CreateSliderPage(App.trackItem item)
        {
            //Capture un-initialized values
            double value = 50.0f;
            try
            {
                value = Convert.ToDouble(item.state);
            }
            catch
            {}
            
            Label heading = new Label
            {
                Text = item.header,
                FontSize = 72,
                TextColor = App.config.TextColor,
                BackgroundColor = App.config.BackGroundColor,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Start
            };

            Slider slider = new Slider
            {
                Minimum = 0.0f,
                Maximum = 100.0f,
                Value = value,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                StyleId = item.link
            };
            slider.ValueChanged += OnSliderValueChanged;

            Button button = new Button
            {
                Text = "Close",
                FontSize = 50,
                TextColor = App.config.TextColor,
                BackgroundColor = App.config.BackGroundColor,
                HorizontalOptions= LayoutOptions.End,
                VerticalOptions= LayoutOptions.Center
            };
            //button.Clicked += OnCloseButtonClicked;

            button.Clicked += (sender, e) => {
                Application.Current.MainPage = PreviousPage;
            };
            

            StackLayout sl = new StackLayout
            {
                Children = { heading, slider, button },
                BackgroundColor = App.config.BackGroundColor,
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            ContentPage cp = new ContentPage();
            cp.Content = sl;

            return cp;
        }

        public static void OnCloseButtonClicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = PreviousPage;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Await.Warning", "CS4014:Await.Warning")]
        static void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            Slider slider = sender as Slider;
            string link = slider.StyleId;
            string state = Convert.ToInt16(Math.Round(e.NewValue, MidpointRounding.AwayFromZero)).ToString();

            Debug.WriteLine("New state: " + state);
            new RestService().SendCommand(link, state);
        }
    }
}
