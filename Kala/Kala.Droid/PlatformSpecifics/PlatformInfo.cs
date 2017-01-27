using System;
using Android.OS;
using Xamarin.Forms;
using Android.Views;
using Android.App;

[assembly: Dependency(typeof(Kala.Droid.PlatformInfo))]
namespace Kala.Droid
{
    public class PlatformInfo : IPlatformInfo
    {
        public string GetModel()
        {
            return String.Format("{0} {1}", Build.Manufacturer,
                                            Build.Model);
        }

        public string GetVersion()
        {
            return Build.VERSION.Release.ToString();
        }
    }
}