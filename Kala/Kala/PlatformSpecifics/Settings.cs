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
        private const string ProtocolKey = "protocol_key";
        private static string ProtocolDefault = "http";

        private const string ServerKey = "server_key";
        private static string ServerDefault = "openhabserver";

        private const string PortKey = "port_key";
        private static int PortDefault = 8080;

        private const string UsernameKey = "username_key";
        private static string UsernameDefault = string.Empty;

        private const string PasswordKey = "password_key";
        private static string PasswordDefault = string.Empty;

        private const string SitemapKey = "sitemap_key";
        private static string SitemapDefault = "Kala";

        private const string FullscreenKey = "Fullscreen_key";
        private static bool FullscreenDefault = false;

        private const string ScreensaverKey = "Screensaver_key";
        private static Int64 ScreensaverDefault = 0;

        private const string ScreenorientationKey = "Screenoriention_key";
        private static string ScreenorientationDefault = "Unspecified";
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

        public static bool Fullscreen
        {
            get => AppSettings.GetValueOrDefault(nameof(Fullscreen), FullscreenDefault);
            set => AppSettings.AddOrUpdateValue(nameof(Fullscreen), value);
        }

        public static Int64 Screensaver
        {
            get => AppSettings.GetValueOrDefault(nameof(Screensaver), ScreensaverDefault);
            set => AppSettings.AddOrUpdateValue(nameof(Screensaver), value);            
        }

        public static string Screenorientation
        {
            get => AppSettings.GetValueOrDefault(nameof(Screenorientation), ScreenorientationDefault);
            set => AppSettings.AddOrUpdateValue(nameof(Screenorientation), value);
        }
    }
}