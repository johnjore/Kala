using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using FFImageLoading.Forms;
using FFImageLoading.Transformations;
using FFImageLoading.Work;
using Plugin.Logger;

namespace Kala
{
    public partial class Widgets
    {
        public static void Avatar(Grid grid, string x1, string y1, string x2, string y2, JObject data)
        {
            CrossLogger.Current.Debug("Avatar", "Creating Avatar Widget");

            int px = 0;
            int py = 0;
            int sx = 0;
            int sy = 0;

            //If this fails, we don't know where to show the error
            try
            {
                px = Convert.ToInt16(x1);
                py = Convert.ToInt16(y1);
                sx = Convert.ToInt16(x2);
                sy = Convert.ToInt16(y2);
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Avatar", "Crashed: " + ex.ToString());
            }

            try
            {
                Models.avatar item = data.ToObject<Models.avatar>();

                #region w_grid
                //Create grid layout
                Grid w_grid = new Grid();
                grid.Children.Add(w_grid, px, px + sx, py, py + sy);

                w_grid.RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = GridLength.Auto },
                };
                w_grid.ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                };
                w_grid.Padding = new Thickness(0, 0, 0, 0);
                w_grid.RowSpacing = 0;
                w_grid.ColumnSpacing = 0;
                w_grid.BackgroundColor = App.config.CellColor;
                w_grid.VerticalOptions = LayoutOptions.FillAndExpand;
                w_grid.HorizontalOptions = LayoutOptions.FillAndExpand;
                #endregion w_grid

                var image = new CachedImage()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.StartAndExpand,
                    DownsampleToViewSize = false,
                    CacheDuration = TimeSpan.FromMilliseconds(1000),
                    Aspect = Aspect.AspectFill,
                    RetryCount = 0,
                    RetryDelay = 250,
                    BitmapOptimizations = true,
                    Transformations = new List<ITransformation>()
                    {
                        new CircleTransformation()
                    },
                    Source = item.url,
                };
                w_grid.Children.Add(image, 0, 0);

                ItemLabel l_mode = new ItemLabel
                {
                    Text = item.item.state,
                    FontSize = 20,
                    TextColor = App.config.TextColor,
                    BackgroundColor = Color.Transparent,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.End,
                    Link = item.item.link
                };
                App.config.itemlabels.Add(l_mode);
                w_grid.Children.Add(l_mode, 0, 1);
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Avatar", "Crashed: " + ex.ToString());
                Error(grid, px, py, ex.ToString());
            }
        }
    }
}
