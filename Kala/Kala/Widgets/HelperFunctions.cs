using System;
using Xamarin.Forms;
using CircularProgressBar.FormsPlugin.Abstractions;

namespace Kala
{
    public partial class Widgets
    {
        public static void AddHeaderText(Grid grid, int x, int y, string Label)
        {
            grid.Children.Add(new Label
            {
                Text = Label,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = App.config.TextColor,
                BackgroundColor = App.config.CellColor,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Start
            }, x, y);
        }

        public static void AddStatusText(Grid grid, int x, int y, string Label, string Unit, string link)
        {
            ItemLabel l_status = new ItemLabel
            {
                Text = Label,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = App.config.TextColor,
                BackgroundColor = App.config.CellColor,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                TranslationY = -10,
                Link = link
            };

            l_status.HorizontalOptions = LayoutOptions.End;

            ItemLabel l_unit = new ItemLabel
            {
                Text = Unit,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = App.config.TextColor,
                BackgroundColor = App.config.CellColor,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.End,
                TranslationY = -10,
                Link = link
            };

            //Grid for status text at the bottom
            Grid g = new Grid();
            g.RowDefinitions = new RowDefinitionCollection();
            g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            g.ColumnDefinitions = new ColumnDefinitionCollection();
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            g.RowSpacing = 6;
            g.ColumnSpacing = 6;
            g.BackgroundColor = App.config.CellColor;
            g.HorizontalOptions = LayoutOptions.Center;
            g.VerticalOptions = LayoutOptions.End;

            g.Children.Add(l_status, 0, 0);
            g.Children.Add(l_unit, 1, 0);

            grid.Children.Add(g, x, y);
        }
    
        public static void AddImageEnd(Grid grid, int x, int y, string strImage)
        {
            grid.Children.Add(new Image
            {
                Source = Device.OnPlatform(strImage, strImage, "Assets/" + strImage),
                Aspect = Aspect.AspectFill,
                BackgroundColor = App.config.CellColor,
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.Center
            },
                x,          // left
                x + 1,        // right
                y,          // top
                y + 1);       // bottom
        }

        public static void AddImageCenter(Grid grid, int x, int y, string strImage, Color BackGround)
        {
            grid.Children.Add(new Image
            {
                Source = Device.OnPlatform(strImage, strImage, "Assets/" + strImage),
                Aspect = Aspect.AspectFill,
                BackgroundColor = BackGround,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            },
                x,          // left
                x + 1,        // right
                y,          // top
                y + 1);       // bottom
        }

        public static void ProgressCircle(Grid grid, int x, int y, int value, float scale)
        {
            if (value >= 0 && value <= 100)
            {
                grid.Children.Add(new CircularProgressBarView
                {
                    Progress = value,
                    StrokeThickness = Device.OnPlatform(2, 4, 16),
                    BackgroundColor = Color.Transparent,
                    ProgressBackgroundColor = App.config.BackGroundColor,
                    ProgressColor = App.config.ValueColor,
                    Scale = scale
                }, x, y);
            }
        }

        public static void ProgressArc(Grid grid, int x, int y, int value, float scale)
        {
            if (value >= 0 && value <= 100)
            {
                //Not drawing a full circle, but an arc, 85% of a circle
                value = (int)((float)value / 100.0 * 85.0);
                grid.Children.Add(new CircularProgressBarView
                {
                    Progress = value,
                    StrokeThickness = Device.OnPlatform(2, 4, 16),
                    BackgroundColor = Color.Transparent,
                    ProgressBackgroundColor = App.config.BackGroundColor,
                    ProgressColor = App.config.ValueColor,
                    Scale = scale
                }, x, y);
            }
        }

        public static string WeatherCondition(string state)
        {
            string strImage = string.Empty;
            switch (state.ToLower())
            {
                case "thunder": strImage = "\uf01e"; break;
                case "storm": strImage = "\uf01e"; break;
                case "rain-and-snow": strImage = "\uf017"; break;
                case "rain-and-sleet": strImage = "\uf0b5"; break;
                case "snow-and-sleet": strImage = "\uff0b5"; break;
                case "freezing-drizzle": strImage = "\uf017"; break;
                case "few-showers": strImage = "\uf01a"; break;
                case "freezing-rain": strImage = "\uf017"; break;
                case "rain": strImage = "\uf018"; break;
                case "snow-flurries": strImage = "\uf064"; break;
                case "light-snow": strImage = "\uf064"; break;
                case "blowing-snow": strImage = "\uf064"; break;
                case "snow": strImage = "\uf01b"; break;
                case "sleet": strImage = "\uf0b5"; break;
                case "dust": strImage = "\uf063"; break;
                case "fog": strImage = "\uf014"; break;
                case "wind": strImage = "\uf012"; break;
                case "cold": strImage = "\uf076"; break;
                case "cloudy": strImage = "\uf013"; break;
                case "mostly-cloudy-night": strImage = "\uf086"; break;
                case "mostly-cloudy-day": strImage = "\uf002"; break;
                case "partly-cloudy-night": strImage = "\uf083"; break;
                case "partly-cloudy-day": strImage = "\uf00c"; break;
                case "clear-night": strImage = "\uf02e"; break;
                case "sunny": strImage = "\uf00d"; break;
                case "hot": strImage = "\uf072"; break;
                case "scattered-thunder": strImage = "\uf01e"; break;
                case "scattered-showers": strImage = "\uf01a"; break;
                case "thundershowers": strImage = "\uf01e"; break;
                case "snow-showers": strImage = "\uf01e"; break;
                case "scattered-thundershowers": strImage = "\uf01d"; break;
                default: strImage = "\uf07b"; break;
            }
            return strImage;
        }
    }
}
