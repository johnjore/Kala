using System;
using Xamarin.Forms;

namespace Kala
{
    public partial class Widgets : ContentPage
    {
        public static void Blank(Grid grid, string x, string y)
        {
            HockeyApp.MetricsManager.TrackEvent("Create Blank Widget");

            int.TryParse(x, out int px);
            int.TryParse(y, out int py);

            grid.Children.Add(new Label
            {
                BackgroundColor = App.Config.CellColor
            }, px, py);

            //Button must be last to be added to work
            Button dummyButton = new Button
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent,
            };
            grid.Children.Add(dummyButton, px, py);
            dummyButton.Clicked += OnDummyButtonClicked;
        }
    }
}
