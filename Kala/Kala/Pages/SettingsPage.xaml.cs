using System;
using Xamarin.Forms;

namespace Kala.Pages
{
    public partial class Settings : ContentPage
    {
        public Settings()
        {
            InitializeComponent();
            BindingContext = new SettingsViewModel();
        }
    }
}
