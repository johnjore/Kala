using System;
using Android.OS;
using Xamarin.Forms;
using Android.Views;
using Android.App;

[assembly: Dependency(typeof(Kala.Droid.Dim))]

namespace Kala.Droid
{
    public class Dim : IDim
    {
        public void SetBacklight(float factor)
        {
            var activity = (Activity)Forms.Context;
            var attributes = activity.Window.Attributes;

            attributes.ScreenBrightness = factor;
            activity.Window.Attributes = attributes;
        }

    }
}