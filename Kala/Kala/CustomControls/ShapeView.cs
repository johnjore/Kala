using System;
using Xamarin.Forms;

namespace DrawShape
{
	public class ShapeView : BoxView
	{
        public static readonly BindableProperty ShapeTypeProperty = BindableProperty.Create(nameof(ShapeType), typeof(ShapeType), typeof(ShapeView), ShapeType.Box);
        public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create(nameof(StrokeColor), typeof(Color), typeof(ShapeView), Color.Default);
        public static readonly BindableProperty StrokeWidthProperty = BindableProperty.Create(nameof(StrokeWidth), typeof(float), typeof(ShapeView), 1f);
        public static readonly BindableProperty IndicatorPercentageProperty = BindableProperty.Create(nameof(IndicatorPercentage), typeof(float), typeof(ShapeView), 0f);
        public new static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(nameof(CornerRadius), typeof(float), typeof(ShapeView), 0f);
        public static readonly BindableProperty PaddingProperty = BindableProperty.Create(nameof(Padding), typeof(Thickness), typeof(ShapeView), default(Thickness));
        public static readonly BindableProperty NameProperty = BindableProperty.Create(nameof(Name), typeof(string), typeof(ItemLabel), null);
        public static readonly BindableProperty MinProperty = BindableProperty.Create(nameof(Min), typeof(double), typeof(ItemLabel), 0.0);
        public static readonly BindableProperty MaxProperty = BindableProperty.Create(nameof(Max), typeof(double), typeof(ItemLabel), 100.0);

        public ShapeType ShapeType {
			get { return (ShapeType)GetValue (ShapeTypeProperty); }
			set { SetValue (ShapeTypeProperty, value); }
		}

		public Color StrokeColor {
			get { return (Color)GetValue (StrokeColorProperty); }
			set { SetValue (StrokeColorProperty, value); }
		}

		public float StrokeWidth {
			get { return (float)GetValue (StrokeWidthProperty); }
			set { SetValue (StrokeWidthProperty, value); }
		}

		public float IndicatorPercentage {
			get { return (float)GetValue (IndicatorPercentageProperty); }
			set {
				if (ShapeType != ShapeType.CircleIndicator && ShapeType != ShapeType.Arc)
					throw new ArgumentException ("Can only specify this property with CircleIndicator");
				SetValue (IndicatorPercentageProperty, value);
			}
		}

		public new float CornerRadius {
			get { return (float)GetValue (CornerRadiusProperty); }
			set {
				if (ShapeType != ShapeType.Box)
					throw new ArgumentException ("Can only specify this property with Box");
				SetValue (CornerRadiusProperty, value);
			}
		}

		public Thickness Padding {
			get { return (Thickness)GetValue (PaddingProperty); }
			set { SetValue (PaddingProperty, value); }
		}

		public ShapeView ()
		{
		}

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public double Min
        {
            get { return (double)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        public double Max
        {
            get { return (double)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }
    }

    public enum ShapeType
	{
		Box,
		Circle,
		CircleIndicator,
        Arc
	}
}