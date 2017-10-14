using System;
using Android.App;
using Android.Content.PM;
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
