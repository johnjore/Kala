using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using DrawShape;
using DrawShape.Android;

[assembly:ExportRenderer (typeof(ShapeView), typeof(ShapeRenderer))]
namespace DrawShape.Android
{
	public class ShapeRenderer : ViewRenderer<ShapeView, Shape>
	{
        public ShapeRenderer(Context context) : base(context)
		{
		}

		protected override void OnElementChanged (ElementChangedEventArgs<ShapeView> e)
		{
			base.OnElementChanged (e);

			if (e.OldElement != null || this.Element == null)
				return;

			SetNativeControl (new Shape (Resources.DisplayMetrics.Density, Context) {
				ShapeView = Element
			});
		}
	}
}