﻿using System;
using Xamarin.Forms;

namespace Kala
{
    public partial class Widgets : ContentPage
    {
        private static void Error(Grid grid, int px, int py, int sx, int sy, string errorMessage)
        {
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Create Error Widget");

            try
            {
                grid.Children.Add(new Image
                {
                    Source = "ic_error_white_48dp",
                    Aspect = Aspect.AspectFill,
                    BackgroundColor = App.Config.CellColor,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                }, px, px + sx, py, py + sy);

                grid.Children.Add(new Label
                {
                    Text = errorMessage,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    TextColor = App.Config.TextColor,
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
