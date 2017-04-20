/**/ //Ugly. Replace this with a popup of some description
using System;
using Xamarin.Forms;
using Plugin.Logger;

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
                HeightRequest = 30,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                TranslationY = 100,
                StyleId = item.name,
            };
            slider.Effects.Add(Effect.Resolve("Effects.SliderEffect"));
            slider.ValueChanged += OnSliderValueChanged;

            Button button = new Button
            {
                Text = "Close",
                FontSize = 50,
                TextColor = App.config.TextColor,
                BackgroundColor = App.config.BackGroundColor,
                HorizontalOptions= LayoutOptions.End,
                VerticalOptions= LayoutOptions.End,
                TranslationY = 200
            };
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
                
        static void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            Slider slider = sender as Slider;
            string name = slider.StyleId;
            string state = Convert.ToInt16(Math.Round(e.NewValue, MidpointRounding.AwayFromZero)).ToString();

            CrossLogger.Current.Debug("Slider", "New state: " + state);
            #pragma warning disable CS4014
            new RestService().SendCommand(name, state);
            #pragma warning restore CS4014
        }
    }
}
