using System;
using Android.OS;
using Xamarin.Forms;
using Android.Views;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Net.Wifi;
using Java.Util;
using System.Text;

[assembly: Dependency(typeof(Kala.Droid.PlatformInfo))]
namespace Kala.Droid
{
    public class PlatformInfo : IPlatformInfo
    {
        public string GetModel()
        {
            return String.Format("{0} {1}", Build.Manufacturer, Build.Model);
        }

        public string GetVersion()
        {
            return Build.VERSION.Release.ToString();
        }

        public string GetWifiMacAddress()
        {
            //https://stackoverflow.com/questions/39456962/how-do-i-get-the-mac-address-for-and-android-device-using-6-0-or-higher-in-c
            var all = Collections.List(Java.Net.NetworkInterface.NetworkInterfaces);

            foreach (var i in all)
            {
                var macBytes = (i as Java.Net.NetworkInterface).GetHardwareAddress();
                if (macBytes == null) continue;

                var sb = new StringBuilder();
                foreach (var b in macBytes)
                {
                    sb.Append((b & 0xFF).ToString("X2"));
                }
                return sb.ToString().ToLower();
            }
            return null;
        }
    }
}