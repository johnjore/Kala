using System;
using Xamarin.Forms;

namespace Kala
{
    public partial class Widgets : ContentPage
    {
        public static void Blank(Grid grid, int px, int py, int sx, int sy)
        {
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Create Blank Widget");

            grid.Children.Add(new Label
            {
                BackgroundColor = App.Config.CellColor
            }, px, px + sx, py, py + sy);

            //Button must be last to be added to work
            Button dummyButton = new Button
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent,
            };
            grid.Children.Add(dummyButton, px, px + sx, py, py + sy);
            dummyButton.Clicked += OnDummyButtonClicked;
        }
    }
}
