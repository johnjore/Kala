using System;
using Xamarin.Forms;

namespace Kala
{
    public partial class Widgets : ContentPage
    {
        public static void Clock(Grid grid, string x1, string y1, string x2, string y2)
        {
            HockeyApp.MetricsManager.TrackEvent("Create Clock Widget");

            DateTime time = DateTime.Now;
            string format1 = "HH:mm";
            string format2 = "dddd d MMMM";
            double resolution = 160.0;

            int.TryParse(x1, out int px);
            int.TryParse(y1, out int py);
            int.TryParse(x2, out int sx);
            int.TryParse(y2, out int sy);

            grid.Children.Add(new Label
            {
                BackgroundColor = App.Config.CellColor
            }, px, px + sx, py, py + sy);

            Label l1 = new Label
            {
                Text = time.ToString(format1),
                FontSize = (int)(resolution * 24.0 / 72.0),
                TextColor = App.Config.TextColor,
                BackgroundColor = App.Config.CellColor,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                TranslationY = -20
            };
            grid.Children.Add(l1, px, px + sx, py, py + sy);

            Label l2 = new Label
            {
                Text = time.ToString(format2).ToUpper(),
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                TextColor = App.Config.TextColor,
                BackgroundColor = App.Config.CellColor,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                TranslationY = 20
            };
            grid.Children.Add(l2, px, px + sx, py, py + sy);

            //Button must be last to be added to work
            Button dummyButton = new Button
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent,
            };
            grid.Children.Add(dummyButton, px, px + sx, py, py + sy);
            dummyButton.Clicked += OnDummyButtonClicked;

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                time = DateTime.Now;
                l1.Text = time.ToString(format1);
                l2.Text = time.ToString(format2).ToUpper();
                return true;
            });
        }
    }
}
