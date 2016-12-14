using Xamarin.Forms;

public class ItemLabel : Label
{
    public static readonly BindableProperty LinkProperty = BindableProperty.Create<ItemLabel, string>(p => p.Link, "");
    public static readonly BindableProperty PreProperty = BindableProperty.Create<ItemLabel, string>(p => p.Pre, "");
    public static readonly BindableProperty PostProperty = BindableProperty.Create<ItemLabel, string>(p => p.Post, "");

    public string Link
    {
        get { return (string)GetValue(LinkProperty); }
        set { SetValue(LinkProperty, value); }
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
}
