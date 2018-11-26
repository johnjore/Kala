using System;
using Xamarin.Forms;

namespace Kala
{
    public partial class Widgets : ContentPage
    {
        public static void Clock(Grid grid, int px, int py, int sx, int sy)
        {
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Create Clock Widget");

            DateTime time = DateTime.Now;
            string format1 = "HH:mm";
            string format2 = "dddd d MMMM";
            double resolution = 160.0;

            //Master Grid for Widget
            Grid Widget_Grid = new Grid
            {
                RowDefinitions = new RowDefinitionCollection {
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                },
                ColumnDefinitions = new ColumnDefinitionCollection {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                },
                RowSpacing = 0,
                ColumnSpacing = 0,
                BackgroundColor = App.Config.CellColor,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };
            grid.Children.Add(Widget_Grid, px, px + sx, py, py + sy);

            Label l1 = new Label
            {
                Text = time.ToString(format1),
                FontSize = (int)(resolution * 24.0 / 72.0),
                TextColor = App.Config.TextColor,
                BackgroundColor = App.Config.CellColor,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.EndAndExpand,
                TranslationY = 10,
            };
            Widget_Grid.Children.Add(l1, 0, 0);

            Label l2 = new Label
            {
                Text = time.ToString(format2).ToUpper(),
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                TextColor = App.Config.TextColor,
                BackgroundColor = App.Config.CellColor,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.StartAndExpand,
            };
            Widget_Grid.Children.Add(l2, 0, 1);
            
            //Button must be last to be added to work
            Button dummyButton = new Button
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent,
            };
            Widget_Grid.Children.Add(dummyButton, 0, 1, 0, 1);
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
