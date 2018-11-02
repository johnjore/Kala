/**/ //Ugly. Replace this with a popup of some description
using System;
using Xamarin.Forms;
using Plugin.Logger;
using System.Threading.Tasks;

namespace Kala
{
    public partial class Widgets : ContentPage
    {
        private static ContentPage CreateSliderPage(App.TrackItem item)
        {
            double.TryParse(item.State, out double value);

            Slider slider = new Slider
            {
                Minimum = 0.0f,
                Maximum = 100.0f,
                Value = value,
                HeightRequest = 100,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                TranslationY = 100,
                StyleId = item.Name,
            };
            slider.Effects.Add(Effect.Resolve("Effects.SliderEffect"));
            slider.ValueChanged += OnSliderValueChanged;

            Button button = new Button
            {
                Text = "Close",
                FontSize = 50,
                TextColor = App.Config.TextColor,
                BackgroundColor = App.Config.BackGroundColor,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.End,
                TranslationY = 200
            };
            button.Clicked += (sender, e) => {
                Application.Current.MainPage = PreviousPage;
            };

            App.Config.LastActivity = DateTime.Now;

            return (new ContentPage {
                Content = new StackLayout
                {
                    Children = { new Label
                        {
                            Text = item.Header,
                            FontSize = 72,
                            TextColor = App.Config.TextColor,
                            BackgroundColor = App.Config.BackGroundColor,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Start
                        },
                        slider, button },
                    BackgroundColor = App.Config.BackGroundColor,
                    Orientation = StackOrientation.Vertical,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                }
            });
        }
                
        private static void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            App.Config.LastActivity = DateTime.Now;

            Slider slider = sender as Slider;
            string name = slider.StyleId;
            string state = Convert.ToInt16(Math.Round(e.NewValue, MidpointRounding.AwayFromZero)).ToString();

            Task.Run(async () =>
            {
                await new RestService().SendCommand(name, state);
            });
        }
    }
}
