using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace Kala.Droid
{
    [Activity(Label = "Kala", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            //Fullscreen
            if (Settings.Fullscreen)
            {
                this.Window.AddFlags(WindowManagerFlags.Fullscreen);
                base.SetTheme(global::Android.Resource.Style.ThemeBlackNoTitleBar);
            }
            else
            {
                base.SetTheme(global::Android.Resource.Style.ThemeHolo);
                this.Window.ClearFlags(WindowManagerFlags.Fullscreen);
            }

            //Screensaver
            if (Settings.Screensaver > 0)
            {
                this.Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            }
            else
            {
                this.Window.ClearFlags(WindowManagerFlags.KeepScreenOn);
            }

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            //App.Speech = new Speech();

            LoadApplication(new App());
        }
    }
}
