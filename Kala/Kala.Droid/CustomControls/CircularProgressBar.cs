using System.ComponentModel;
using com.refractored.monodroidtoolkit;
using CircularProgressBar.FormsPlugin.Abstractions;
using CircularProgressBar.FormsPlugin.Android;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

//From https://github.com/jamesmontemagno

[assembly: ExportRenderer(typeof(CircularProgressBarView), typeof(CircularProgressBarRenderer))]
namespace CircularProgressBar.FormsPlugin.Android
{
    public class CircularProgressBarRenderer : ViewRenderer<CircularProgressBarView, HoloCircularProgressBar>
    {
        /// <summary>
        /// Used for registration with dependency service
        /// </summary>
        public static void Init()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<CircularProgressBarView> e)
        {
            base.OnElementChanged(e);

            var progressBar = Element as CircularProgressBarView;

            if (e.OldElement != null || progressBar == null)
            {
                return;
            }
            
            var progress = new HoloCircularProgressBar(Forms.Context)
            {
                Progress = progressBar.Progress,
                ProgressColor = progressBar.ProgressColor.ToAndroid(),
                ProgressBackgroundColor = progressBar.ProgressBackgroundColor.ToAndroid(),
                CircleStrokeWidth = progressBar.StrokeThickness
            };

            //Start position /**/ This needs fixing for different icons sizes
            progressBar.Rotation = 205;

            SetNativeControl(progress);
        }
    }
}