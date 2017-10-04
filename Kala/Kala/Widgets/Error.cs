using System;
using Xamarin.Forms;

namespace Kala
{
    public partial class Widgets
    {
        public static void Error(Grid grid, int px, int py, int sx, int sy, string errorMessage)
        {
            try
            {
                string strImage = "ic_error_white_48dp";
                switch (Device.RuntimePlatform)
                {
                    case Device.WinPhone:
                        strImage = "Assets/" + strImage;
                        break;
                }

                grid.Children.Add(new Image
                {
                    Source = strImage,
                    Aspect = Aspect.AspectFill,
                    BackgroundColor = App.config.CellColor,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                }, px, px + sx, py, py + sy);

                grid.Children.Add(new Label
                {
                    Text = errorMessage,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    TextColor = App.config.TextColor,
                    BackgroundColor = Color.Transparent,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }, px, px + sx, py, py + sy);
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Alert", ex.ToString(), "OK");
            }
        }
    }
}
