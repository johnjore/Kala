using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Views;
using Xamarin.Forms;

[assembly: Dependency(typeof(Kala.Droid.ScreenLayout))]
namespace Kala.Droid
{
    public class ScreenLayout : IScreen
    {
        static Context _context;

        public static void Init(Context context)
        {
            _context = context;
        }

        public void SetBacklight(float factor)
        {
            var activity = (Activity)_context;
            var attributes = activity.Window.Attributes;

            attributes.ScreenBrightness = factor;
            activity.Window.Attributes = attributes;
        }

        public void SetScreenOrientation(string Screenorientation)
        {
            var activity = (Activity)_context;
            activity.RequestedOrientation = (ScreenOrientation)Enum.Parse(typeof(ScreenOrientation), Screenorientation);
            activity.RequestedOrientation = ScreenOrientation.Locked;
        }

        public void ScreenSaver(long screensaver)
        {
            var activity = (Activity)_context;

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
            var activity = (Activity)_context;

            if (fullscreen)
            {
                activity.Window.AddFlags(WindowManagerFlags.Fullscreen);

                //Removes on-screen navigation buttons. Are all options required?
                activity.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(
                    SystemUiFlags.LayoutStable |
                    SystemUiFlags.HideNavigation |
                    SystemUiFlags.LayoutFullscreen |
                    SystemUiFlags.Fullscreen |
                    SystemUiFlags.LowProfile |
                    SystemUiFlags.ImmersiveSticky
                );
            }
            else
            {
                activity.Window.ClearFlags(WindowManagerFlags.Fullscreen);
            }
        }
    }
}