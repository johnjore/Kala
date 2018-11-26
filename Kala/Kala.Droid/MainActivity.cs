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
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
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
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            //AppCenter replacing HockeyApp
            AppCenter.LogLevel = Microsoft.AppCenter.LogLevel.Verbose;

            string androidSecretKey = Application.Context.Resources.GetString(Resource.String.Microsoft_App_Center_AndroidSecretKey);
            AppCenter.Start(androidSecretKey, typeof(Analytics), typeof(Crashes), typeof(Distribute));
            
            // Init special handlers
            ScreenLayout.Init(this);

            // Library for Image handling as XF does not support authentication
            global::FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);

            // Xamarin.Forms.GoogleMaps initialization
            Xamarin.FormsGoogleMaps.Init(this, savedInstanceState);

            // Init barcode library
            ZXing.Net.Mobile.Forms.Android.Platform.Init();

            // Popup library
            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);

            // Logger
            #if DEBUG
                CrossLogger.Current.Configure("Kala.log", 3, 100, Plugin.Logger.Abstractions.LogLevel.Debug, true);
            #endif
            CrossLogger.Current.Purge();
            CrossLogger.Current.Log(Plugin.Logger.Abstractions.LogLevel.Info, "Kala", "Log Started");
            CrossLogger.Current.Log(Plugin.Logger.Abstractions.LogLevel.Info, "Kala", "Folder for Log file: " + CrossLogger.Current.GetLocalStoragePath().ToString());

            Xamarin.Forms.Forms.Init(this, savedInstanceState);
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
