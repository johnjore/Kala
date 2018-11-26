//From https://mindofai.github.io/Launching-Apps-thru-URI-with-Xamarin.Forms/
using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using OpenAppLauncher.Droid;
using Xamarin.Forms;
using Kala;
using Plugin.Logger;
using Xamarin.Forms.Platform.Android;

[assembly: Dependency(typeof(AppLauncher))]
namespace OpenAppLauncher.Droid
{
    public class AppLauncher : Activity, IAppLauncher
    {
        public Task<bool> Launch(string stringUri)
        {
            try
            {
                Intent intent = Android.App.Application.Context.PackageManager.GetLaunchIntentForPackage(stringUri);

                if (intent != null)
                {
                    intent.AddFlags(ActivityFlags.NewTask);
                    Android.App.Application.Context.StartActivity(intent);
                    return Task.FromResult(true);
                }
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Debug("Launcher", "Failed to launch app: '" + stringUri + "'; " + ex.ToString());
            }
            return Task.FromResult(false);
        }
    }
}
