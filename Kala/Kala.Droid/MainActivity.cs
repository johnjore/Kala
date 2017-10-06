using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using FFImageLoading.Forms.Droid;
using Plugin.Logger;
using Plugin.Logger.Abstractions;

namespace Kala.Droid
{
    [Activity(Label = "Kala", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            //Fullscreen
            if (Settings.Fullscreen)
            {
                Window.AddFlags(WindowManagerFlags.Fullscreen);
                base.SetTheme(global::Android.Resource.Style.ThemeBlackNoTitleBar);
            }
            else
            {
                base.SetTheme(global::Android.Resource.Style.ThemeHolo);
                Window.ClearFlags(WindowManagerFlags.Fullscreen);
            }

            //Screensaver
            if (Settings.Screensaver > 0)
            {
                Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            }
            else
            {
                Window.ClearFlags(WindowManagerFlags.KeepScreenOn);
            }

            //Orientation
            try
            {
                RequestedOrientation = (ScreenOrientation)Enum.Parse(typeof(ScreenOrientation), Settings.Screenorientation);
            }
            catch { }

            base.OnCreate(bundle);

            Xamarin.Forms.Forms.Init(this, bundle);
            //App.Speech = new Speech();

            // Library for Image handling as XF does not support authentication
            CachedImageRenderer.Init();

            // Xamarin.Forms.GoogleMaps initialization
            Xamarin.FormsGoogleMaps.Init(this, bundle);

            //Logger
            CrossLogger.Current.Configure("Kala.log", 3, 100, LogLevel.Debug, true);
            CrossLogger.Current.Log(LogLevel.Info, "Kala", "Log Started");
            CrossLogger.Current.Log(LogLevel.Info, "Kala", "Folder for Log file: " + CrossLogger.Current.GetLocalStoragePath().ToString());

            LoadApplication(new App());
        }
    }
}
