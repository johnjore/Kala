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
                string strImage = "ic_error_white_48dp";
                grid.Children.Add(new Image
                {
                    Source = Device.OnPlatform(strImage, strImage, "Assets/" + strImage),
                    Aspect = Aspect.AspectFill,
                    BackgroundColor = App.config.CellColor,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                }, px, px + 1, py, py + 1);

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
