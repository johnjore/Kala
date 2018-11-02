using System;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Globalization;

namespace Kala
{
    public partial class Widgets : ContentPage
    {
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
                case "clear": strImage = "\uf00d"; break;
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

        private static Tuple<string, int> Digits(Dictionary<string, string> dict, string state)
        {
            string s_value = string.Empty;
            int digits = -1;

            if (dict.ContainsKey("digits"))
            {               
                int.TryParse(dict["digits"], out digits);
                Double.TryParse(state.Replace(".", CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator), out double v1);
                s_value = (Math.Round(v1, digits, MidpointRounding.AwayFromZero)).ToString("f" + digits);
            }
            else
            {
                s_value = state;
            }

            return Tuple.Create(s_value, digits);
        }

        private static void OnDummyButtonClicked(object sender, EventArgs e)
        {
            App.Config.LastActivity = DateTime.Now;
        }
    }
}
