using Xamarin.Forms;

namespace Kala
{
    public class ItemLabel : Label
    {
        public static readonly BindableProperty NameProperty = BindableProperty.Create(nameof(Name), typeof(string), typeof(ItemLabel), null);
        public static readonly BindableProperty PreProperty = BindableProperty.Create(nameof(Pre), typeof(string), typeof(ItemLabel), null);
        public static readonly BindableProperty PostProperty = BindableProperty.Create(nameof(Post), typeof(string), typeof(ItemLabel), null);
        public static readonly BindableProperty TypeProperty = BindableProperty.Create(nameof(Type), typeof(Kala.Models.Itemtypes), typeof(ItemLabel), Kala.Models.Itemtypes.Notused);
        public static readonly BindableProperty DigitsProperty = BindableProperty.Create(nameof(Digits), typeof(int), typeof(ItemLabel), -1);
        public static readonly BindableProperty TransformProperty = BindableProperty.Create(nameof(Transformed), typeof(bool), typeof(ItemLabel), false);

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public string Pre
        {
            get { return (string)GetValue(PreProperty); }
            set { SetValue(PreProperty, value); }
        }

        public string Post
        {
            get { return (string)GetValue(PostProperty); }
            set { SetValue(PostProperty, value); }
        }

        public Models.Itemtypes Type
        {
            get { return (Models.Itemtypes)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public int Digits
        {
            get { return (int)GetValue(DigitsProperty); }
            set { SetValue(DigitsProperty, value); }
        }

        public bool Transformed
        {
            get { return (bool)GetValue(TransformProperty); }
            set { SetValue(TransformProperty, value); }
        }
    }
}
