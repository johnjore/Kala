using System;
using Android.App;
using Android.Content.PM;
using Xamarin.Forms;
using Android.Views;

[assembly: Dependency(typeof(Kala.Droid.ScreenLayout))]
namespace Kala.Droid
{
    public class ScreenLayout : IScreen
    {
        public void SetBacklight(float factor)
        {
            var activity = (Activity)Forms.Context;
            var attributes = activity.Window.Attributes;

            attributes.ScreenBrightness = factor;
            activity.Window.Attributes = attributes;
        }

        public void SetScreenOrientation(string ScreenOrientation)
        {
            var activity = (Activity)Forms.Context;
            activity.RequestedOrientation = (ScreenOrientation)Enum.Parse(typeof(ScreenOrientation), ScreenOrientation);
        }

        public void ScreenSaver(long screensaver)
        {
            var activity = (Activity)Forms.Context;

            if (screensaver > 0)
            {
                activity.Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            }
            else
            {
                activity.Window.ClearFlags(WindowManagerFlags.KeepScreenOn);
            }
        }

        public void SetFullScreen(bool fullscreen)
        {
            var activity = (Activity)Forms.Context;
            var attributes = activity.Window.Attributes;

            if (fullscreen)
            {
                activity.Window.AddFlags(WindowManagerFlags.Fullscreen);
                /**///SetTheme(Android.Resource.Style.ThemeBlackNoTitleBar);
            }
            else
            {
                /**///SetTheme(Android.Resource.Style.ThemeHolo);
                activity.Window.ClearFlags(WindowManagerFlags.Fullscreen);
            }
        }
    }
}