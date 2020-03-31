using System;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace Kala.Pages
{
    public partial class NumericInputPage : PopupPage
    {
        private string OHItemToUpdate;

        public NumericInputPage(string itemName)
        {
            InitializeComponent();
            OHItemToUpdate = itemName;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            entryField.Focus();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            DependencyService.Get<IScreen>().SetFullScreen(App.Config.FullScreen);
        }

        private void OnClose(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();

            if (entryField.Text != string.Empty)
            {
                Task.Run(async () =>
                {
                    await new RestService().SendCommand(OHItemToUpdate, entryField.Text);
                });
            }            
        }
    }
}
