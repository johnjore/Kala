using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FFImageLoading.Forms.Droid;
using Plugin.Logger;
using Plugin.Logger.Abstractions;
using Kala;
using Debug = System.Diagnostics.Debug;

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
    [Activity(Label = "Kala.Droid", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public event EventHandler<ActivityResultEventArgs> ActivityResult = delegate { };
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Xamarin.Forms.Forms.Init(this, bundle);

            // Init special handlers
            ScreenLayout.Init(this);

            // Library for Image handling as XF does not support authentication
            CachedImageRenderer.Init(true);

            // Xamarin.Forms.GoogleMaps initialization
            Xamarin.FormsGoogleMaps.Init(this, bundle);

            // Init barcode library
            ZXing.Net.Mobile.Forms.Android.Platform.Init();

            // Popup library
            Rg.Plugins.Popup.Popup.Init(this, bundle);

            // Logger
            #if DEBUG
                CrossLogger.Current.Configure("Kala.log", 3, 100, LogLevel.Debug, true);
            #endif
            CrossLogger.Current.Purge();
            CrossLogger.Current.Log(LogLevel.Info, "Kala", "Log Started");
            CrossLogger.Current.Log(LogLevel.Info, "Kala", "Folder for Log file: " + CrossLogger.Current.GetLocalStoragePath().ToString());

            LoadApplication(new App());
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            ActivityResult(this, new ActivityResultEventArgs
            {
                RequestCode = requestCode,
                ResultCode = resultCode,
                Data = data
            });
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                Debug.WriteLine("Android back button: There are some pages in the PopupStack");
            }
            else
            {
                Debug.WriteLine("Android back button: There are not any pages in the PopupStack");
            }
        }
    }
}
