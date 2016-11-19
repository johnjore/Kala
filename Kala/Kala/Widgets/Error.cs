using System;
using Xamarin.Forms;

namespace Kala
{
    public partial class Widgets
    {
        public static void Error(Grid grid, int px, int py, string errorMessage)
        {
            try
            {
                AddImageCenter(grid, px, py, "ic_error_white_48dp", App.config.CellColor);

                grid.Children.Add(new Label
                {
                    Text = errorMessage,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    TextColor = App.config.TextColor,
                    BackgroundColor = Color.Transparent,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }, px, py);
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Alert", ex.ToString(), "OK");
            }
        }
    }
}
