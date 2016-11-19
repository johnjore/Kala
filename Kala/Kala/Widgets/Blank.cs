using System;
using Xamarin.Forms;

namespace Kala
{
    public partial class Widgets
    {
        public static void Blank(Grid grid, string x, string y)
        {
            int px = Convert.ToInt16(x);
            int py = Convert.ToInt16(y);

            grid.Children.Add(new Label
            {
                BackgroundColor = App.config.CellColor
            }, px, py);
        }
    }
}
