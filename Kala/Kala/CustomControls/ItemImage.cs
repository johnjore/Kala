using Xamarin.Forms;

namespace Kala
{

    public class ItemImageButton : Button
    {
        public static readonly BindableProperty NameProperty = BindableProperty.Create(nameof(Name), typeof(string), typeof(ItemImageButton), null);
        public static readonly BindableProperty URLProperty = BindableProperty.Create(nameof(URL), typeof(string), typeof(ItemImageButton), null);

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public string URL
        {
            get { return (string)GetValue(URLProperty); }
            set { SetValue(URLProperty, value); }
        }
    }
}
