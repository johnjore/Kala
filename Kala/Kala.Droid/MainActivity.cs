using Android.App;
using Android.Content.PM;
using Android.OS;
using FFImageLoading.Forms.Droid;
using Plugin.Logger;
using Plugin.Logger.Abstractions;

/*
Steps to sign APK:
    "keytool.exe" -genkey -v -keystore Kala.keystore -alias Kala -keyalg RSA -keysize 2048 -validity 20000
    "jarsigner.exe" -verbose -keystore Kala.keystore .\Kala.Droid.apk Kala
    "jarsigner.exe" -verify .\Kala.Droid.apk

Install:
    adb install Kala.Droid.apk
 */

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

            //Init special handlers
            ScreenLayout.Init(this);

            // Library for Image handling as XF does not support authentication
            CachedImageRenderer.Init(true);

            // Xamarin.Forms.GoogleMaps initialization
            Xamarin.FormsGoogleMaps.Init(this, bundle);

            //Logger
            #if DEBUG
            CrossLogger.Current.Configure("Kala.log", 3, 100, LogLevel.Debug, true);
            #endif
            CrossLogger.Current.Purge();
            CrossLogger.Current.Log(LogLevel.Info, "Kala", "Log Started");
            CrossLogger.Current.Log(LogLevel.Info, "Kala", "Folder for Log file: " + CrossLogger.Current.GetLocalStoragePath().ToString());

            LoadApplication(new App());
        }
    }
}
