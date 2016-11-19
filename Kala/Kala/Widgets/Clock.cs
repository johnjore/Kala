using System;
using Xamarin.Forms;

namespace Kala
{
    public partial class Widgets
    {
        public static void Clock(Grid grid, string x1, string y1, string x2, string y2)
        {
            int px = Convert.ToInt16(x1);
            int py = Convert.ToInt16(y1);
            int sx = Convert.ToInt16(x2);
            int sy = Convert.ToInt16(y2);

            grid.Children.Add(new Label
            {
                BackgroundColor = App.config.CellColor
            }, px, px + sx, py, py + sy);

            DateTime time = DateTime.Now;

            string format1 = "HH:mm";
            string format2 = "dddd d MMMM";

            double resolution = 160.0;

            Label l1 = new Label
            {
                Text = time.ToString(format1),
                FontSize = (int)((double)(resolution * 24.0 / 72.0)),
                TextColor = App.config.TextColor,
                BackgroundColor = App.config.CellColor,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                TranslationY = -20
            };

            Label l2 = new Label
            {
                Text = time.ToString(format2).ToUpper(),
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                TextColor = App.config.TextColor,
                BackgroundColor = App.config.CellColor,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                TranslationY = 20
            };

            grid.Children.Add(l1, px, px + sx, py, py + sy);
            grid.Children.Add(l2, px, px + sx, py, py + sy);

            // Start the timer
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
