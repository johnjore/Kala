//From https://mindofai.github.io/Launching-Apps-thru-URI-with-Xamarin.Forms/
using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using OpenAppLaunch.Droid;
using Xamarin.Forms;
using Kala;
using Plugin.Logger;

[assembly: Xamarin.Forms.Dependency(typeof(AppLauncher))]
namespace OpenAppLaunch.Droid
{
    public class AppLauncher : Activity, IAppLauncher
    {
        public Task<bool> Launch(string stringUri)
        {
            try
            {
                Intent intent = Android.App.Application.Context.PackageManager.GetLaunchIntentForPackage(stringUri);

                if (intent == null)
                {
                    return Task.FromResult(false);
                }

                intent.AddFlags(ActivityFlags.NewTask);
                Forms.Context.StartActivity(intent);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Debug("Launcher", "Failed to launch app: '" + stringUri + "'; " + ex.ToString());

            }
            return Task.FromResult(false);
        }
    }
}
