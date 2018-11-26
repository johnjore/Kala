using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using Plugin.Logger;
using System.Threading.Tasks;

namespace Kala
{
    public partial class Widgets : ContentPage
    {
        public static void Blind(Grid grid, int px, int py, int sx, int sy, string header, JObject data)
        {
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Create Blind Widget");

            Models.Sitemap.Widget3 item = null;
            Dictionary<string, string> widgetKeyValuePairs = null;

            try
            {
                item = data.ToObject<Models.Sitemap.Widget3>();
                widgetKeyValuePairs = Helpers.SplitCommand(item.Label);
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Blind", "Crashed: " + ex.ToString());
            }

            try
            {
                #region w_grid
                Grid w_grid = new Grid
                {
                    RowSpacing = 0,
                    ColumnSpacing = 0,
                    Padding = new Thickness(0, 0, 0, 0),
                    BackgroundColor = App.Config.CellColor,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                };
                grid.Children.Add(w_grid, px, px + sx, py, py + sy);

                //Vertical
                if (sx < sy)
                {
                    w_grid.ColumnDefinitions = new ColumnDefinitionCollection {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    };

                    w_grid.RowDefinitions = new RowDefinitionCollection {
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }, //Up
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }, //Stop
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }, //Down
                    };

                    //Header
                    w_grid.Children.Add(new Label
                    {
                        Text = header,
                        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                        TextColor = App.Config.TextColor,
                        BackgroundColor = App.Config.CellColor,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Start
                    }, 0, 0);

                    //Control buttons
                    AddControlImage(w_grid, widgetKeyValuePairs["up_icon"], 0, 0, item.Item.Name, "UP");
                    AddControlImage(w_grid, widgetKeyValuePairs["icon"], 0, 1, item.Item.Name, "STOP");
                    AddControlImage(w_grid, widgetKeyValuePairs["down_icon"], 0, 2, item.Item.Name, "DOWN");
                }
                else if (sx > sy) //Horizontal
                {
                    w_grid.ColumnDefinitions = new ColumnDefinitionCollection {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, //Up
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, //Stop
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, //Down
                    };

                    w_grid.RowDefinitions = new RowDefinitionCollection {
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    };

                    //Header
                    w_grid.Children.Add(new Label
                    {
                        Text = header,
                        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                        TextColor = App.Config.TextColor,
                        BackgroundColor = App.Config.CellColor,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Start
                    }, 1, 0);
                    
                    AddControlImage(w_grid, widgetKeyValuePairs["up_icon"], 0, 0, item.Item.Name, "UP");
                    AddControlImage(w_grid, widgetKeyValuePairs["icon"], 1, 0, item.Item.Name, "STOP");
                    AddControlImage(w_grid, widgetKeyValuePairs["down_icon"], 2, 0, item.Item.Name, "DOWN");
                }
                else
                {
                    w_grid.ColumnDefinitions = new ColumnDefinitionCollection {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    };

                    w_grid.RowDefinitions = new RowDefinitionCollection {
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    };

                    //Header
                    w_grid.Children.Add(new Label
                    {
                        Text = header,
                        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                        TextColor = App.Config.TextColor,
                        BackgroundColor = App.Config.CellColor,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Start
                    }, 1, 0);

                    //Control buttons
                    AddControlImage(w_grid, widgetKeyValuePairs["up_icon"], 0, 0, item.Item.Name, "UP");
                    AddControlImage(w_grid, widgetKeyValuePairs["icon"], 1, 1, item.Item.Name, "STOP");
                    AddControlImage(w_grid, widgetKeyValuePairs["down_icon"], 2, 2, item.Item.Name, "DOWN");
                }
                #endregion w_grid
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Blind", "Widgets.Blind crashed: " + ex.ToString());
                Error(grid, px, py, sx, sy, ex.ToString());
            }
        }
        
        private static void AddControlImage(Grid w_grid, String Source, int x, int y, string name, string cmd)
        {
            try
            {
                w_grid.Children.Add(new Image
                {
                    Source = Source,
                    Aspect = Aspect.AspectFill,
                    BackgroundColor = Color.Transparent,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                }, x, y);
                Button blindButton = new ItemButton
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Color.Transparent,
                    Name = name, 
                    Cmd = cmd,
                };
                blindButton.Clicked += OnBlindButtonClicked;
                w_grid.Children.Add(blindButton, x, y);
                CrossLogger.Current.Debug("Blind", "Button ID: " + blindButton.Id + " created.");
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Blind", "Widgets.Blind crashed: " + ex.ToString());
            }
        }

        private static void OnBlindButtonClicked(object sender, EventArgs e)
        {
            ItemButton button = sender as ItemButton;
            Device.BeginInvokeOnMainThread(() => CrossLogger.Current.Debug("Blind", "BlindButton: " + button.Name + ", Cmd: " + button.Cmd));

            Task.Run(async () =>
            {
                await new RestService().SendCommand(button.Name, button.Cmd);
            });          
        }
    }
}
