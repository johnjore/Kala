//From https://github.com/jamesmontemagno
using Xamarin.Forms;

namespace CircularProgressBar.FormsPlugin.Abstractions
{
    /// <summary>
    /// A view that shows a Circular Progress bar with a given percentage and colors.
    /// </summary>
    public class CircularProgressBarView : View
    {
        public static readonly BindableProperty ProgressProperty = BindableProperty.Create(nameof(Progress), typeof(float), typeof(CircularProgressBarView), 0.0f);
        public static readonly BindableProperty StrokeThicknessProperty = BindableProperty.Create(nameof(StrokeThickness), typeof(int), typeof(CircularProgressBarView), 10);
        public static readonly BindableProperty ProgressBackgroundColorProperty = BindableProperty.Create(nameof(ProgressBackgroundColor), typeof(Color), typeof(CircularProgressBarView), Color.Transparent);
        public static readonly BindableProperty ProgressColorProperty = BindableProperty.Create(nameof(ProgressColor), typeof(Color), typeof(CircularProgressBarView), Color.Transparent);
        public static readonly BindableProperty SizeProperty = BindableProperty.Create(nameof(Size), typeof(float), typeof(CircularProgressBarView), 1.0f);
        
        /// <summary>
        /// Gets or sets the current progress
        /// </summary>
        /// <value>The progress.</value>
        public float Progress
        {
            get { return (float)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the thikness of the stroke
        /// </summary>
        public int StrokeThickness
        {
            get { return (int)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        /// <summary>
        /// Gets or sets the ProgressBackgroundColorProperty
        /// </summary>
        /// <value>The color of the ProgressBackgroundColorProperty.</value>
        public Color ProgressBackgroundColor
        {
            get { return (Color)GetValue(ProgressBackgroundColorProperty); }
            set { SetValue(ProgressBackgroundColorProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the progress color
        /// </summary>
        /// <value>The color of the progress.</value>
        public Color ProgressColor
        {
            get { return (Color)GetValue(ProgressColorProperty); }
            set { SetValue(ProgressColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the scale (size) 
        /// </summary>
        /// <value>The scale.</value>
        public float Size
        {
            get { return (float)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }
    }
}
