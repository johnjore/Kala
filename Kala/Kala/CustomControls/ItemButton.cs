using Xamarin.Forms;

public class ItemButton : Button
{
    public static readonly BindableProperty NameProperty = BindableProperty.Create(nameof(Name), typeof(string), typeof(ItemButton), null);
    public static readonly BindableProperty CmdProperty = BindableProperty.Create(nameof(Cmd), typeof(string), typeof(ItemButton), null);
    
    public string Name
    {
        get { return (string)GetValue(NameProperty); }
        set { SetValue(NameProperty, value); }
    }

    public string Cmd
    {
        get { return (string)GetValue(CmdProperty); }
        set { SetValue(CmdProperty, value); }
    }
}
