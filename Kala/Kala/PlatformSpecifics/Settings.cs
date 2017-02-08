//https://github.com/jamesmontemagno/SettingsPlugin

using Plugin.Settings;
using Plugin.Settings.Abstractions;

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
        private static string UsernameDefault = "username";

        private const string PasswordKey = "password_key";
        private static string PasswordDefault = "password";

        private const string SitemapKey = "sitemap_key";
        private static string SitemapDefault = "Kala";

        private const string FullscreenKey = "Fullscreen_key";
        private static bool FullscreenDefault = false;

        private const string ScreensaverKey = "Screensaver_key";
        private static int ScreensaverDefault = 0;

        #endregion

        public static string Protocol
        {
            get { return AppSettings.GetValueOrDefault<string>(ProtocolKey, ProtocolDefault); }
            set { AppSettings.AddOrUpdateValue<string>(ProtocolKey, value); }
        }

        public static string Server
        {
            get { return AppSettings.GetValueOrDefault<string>(ServerKey, ServerDefault); }
            set { AppSettings.AddOrUpdateValue<string>(ServerKey, value); }
        }

        public static int Port
        {
            get { return AppSettings.GetValueOrDefault<int>(PortKey, PortDefault); }
            set { AppSettings.AddOrUpdateValue<int>(PortKey, value); }
        }

        public static string Username
        {
            get { return AppSettings.GetValueOrDefault<string>(UsernameKey, UsernameDefault); }
            set { AppSettings.AddOrUpdateValue<string>(UsernameKey, value); }
        }

        public static string Password
        {
            get { return AppSettings.GetValueOrDefault<string>(PasswordKey, PasswordDefault); }
            set { AppSettings.AddOrUpdateValue<string>(PasswordKey, value); }
        }

        public static string Sitemap
        {
            get { return AppSettings.GetValueOrDefault<string>(SitemapKey, SitemapDefault); }
            set { AppSettings.AddOrUpdateValue<string>(SitemapKey, value); }
        }

        public static bool Fullscreen
        {
            get { return AppSettings.GetValueOrDefault<bool>(FullscreenKey, FullscreenDefault); }
            set { AppSettings.AddOrUpdateValue<bool>(FullscreenKey, value); }
        }

        public static int Screensaver
        {
            get { return AppSettings.GetValueOrDefault<int>(ScreensaverKey, ScreensaverDefault); }
            set { AppSettings.AddOrUpdateValue<int>(ScreensaverKey, value); }
        }
    }
}