using Xamarin.Forms;
using ZXing.Mobile;

public class BarcodeButton : Button
{
    public static readonly BindableProperty NameProperty = BindableProperty.Create(nameof(Name), typeof(string), typeof(ItemButton), null);
    public static readonly BindableProperty OptionsProperty = BindableProperty.Create(nameof(Options), typeof(MobileBarcodeScanningOptions), typeof(BarcodeButton), null);

    public string Name
    {
        get { return (string)GetValue(NameProperty); }
        set { SetValue(NameProperty, value); }
    }

    public MobileBarcodeScanningOptions Options
    {
        get { return (MobileBarcodeScanningOptions)GetValue(OptionsProperty); }
        set { SetValue(OptionsProperty, value); }
    }
}
