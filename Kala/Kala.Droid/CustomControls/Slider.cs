using System;
using Android.Graphics;
using Android.Widget;
using Effects.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("Effects")]
[assembly: ExportEffect(typeof(SliderEffect), "SliderEffect")]
namespace Effects.Droid
{
    public class SliderEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            var seekBar = (SeekBar)Control;
            seekBar.ProgressDrawable.SetColorFilter(new PorterDuffColorFilter(Kala.App.config.CellColor.ToAndroid(), PorterDuff.Mode.SrcIn));
            seekBar.Thumb.SetColorFilter(new PorterDuffColorFilter(Xamarin.Forms.Color.Accent.ToAndroid(), PorterDuff.Mode.SrcIn));
        }

        protected override void OnDetached()
        {
            // Use this method if you wish to reset the control to orginal state
        }
    }
}