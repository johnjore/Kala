//https://github.com/jamesmontemagno/SettingsPlugin

using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;

namespace Kala
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants
        private const string ProtocolDefault = "http";
        private const string ServerDefault = "openhabserver.local";
        private const int PortDefault = 8080;
        private const string UsernameDefault = "";
        private const string PasswordDefault = "";
        private const string SitemapDefault = "Kala";
        #endregion

        public static string Protocol
        {
            get => AppSettings.GetValueOrDefault(nameof(Protocol), ProtocolDefault);
            set => AppSettings.AddOrUpdateValue(nameof(Protocol), value);
        }

        public static string Server
        {
            get => AppSettings.GetValueOrDefault(nameof(Server), ServerDefault);
            set => AppSettings.AddOrUpdateValue(nameof(Server), value);
        }

        public static int Port
        {
            get => AppSettings.GetValueOrDefault(nameof(Port), PortDefault);
            set => AppSettings.AddOrUpdateValue(nameof(Port), value);
        }

        public static string Username
        {
            get => AppSettings.GetValueOrDefault(nameof(Username), UsernameDefault);
            set => AppSettings.AddOrUpdateValue(nameof(Username), value);
        }

        public static string Password
        {
            get => AppSettings.GetValueOrDefault(nameof(Password), PasswordDefault);
            set => AppSettings.AddOrUpdateValue(nameof(Password), value);
        }

        public static string Sitemap
        {
            get => AppSettings.GetValueOrDefault(nameof(Sitemap), SitemapDefault);
            set => AppSettings.AddOrUpdateValue(nameof(Sitemap), value);
        }
    }
}
